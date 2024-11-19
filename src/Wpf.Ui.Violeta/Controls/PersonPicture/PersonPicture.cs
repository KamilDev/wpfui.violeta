﻿using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace Wpf.Ui.Violeta.Controls;

public partial class PersonPicture : Control
{
    static PersonPicture()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PersonPicture), new FrameworkPropertyMetadata(typeof(PersonPicture)));
    }

    public PersonPicture()
    {
        TemplateSettings = new PersonPictureTemplateSettings();

        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PersonPictureAutomationPeer(this);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        m_initialsTextBlock = (TextBlock)GetTemplateChild("InitialsTextBlock");

        m_badgeNumberTextBlock = (TextBlock)GetTemplateChild("BadgeNumberTextBlock");
        m_badgeGlyphIcon = (FontIcon)GetTemplateChild("BadgeGlyphIcon");
        m_badgingEllipse = (Ellipse)GetTemplateChild("BadgingEllipse");
        m_badgingBackgroundEllipse = (Ellipse)GetTemplateChild("BadgingBackgroundEllipse");

        UpdateBadge();
        UpdateIfReady();
    }

    /// <summary>
    /// Helper to determine the initials that should be shown.
    /// </summary>
    string GetInitials()
    {
        if (!string.IsNullOrEmpty(Initials))
        {
            return Initials;
        }
        else if (!string.IsNullOrEmpty(m_displayNameInitials))
        {
            return m_displayNameInitials;
        }
        else
        {
            return m_contactDisplayNameInitials;
        }
    }

    /// <summary>
    /// Helper to determine the image source that should be shown.
    /// </summary>
    ImageSource GetImageSource()
    {
        if (ProfilePicture != null)
        {
            return ProfilePicture;
        }
        else
        {
            return m_contactImageSource;
        }
    }

    /// <summary>
    /// Updates Control elements, if available, with the latest values.
    /// </summary>
    void UpdateIfReady()
    {
        string initials = GetInitials();
        ImageSource imageSrc = GetImageSource();

        var templateSettings = TemplateSettings;
        templateSettings.ActualInitials = initials;
        if (imageSrc != null)
        {
            var imageBrush = templateSettings.ActualImageBrush;
            if (imageBrush == null)
            {
                imageBrush = new ImageBrush();
                imageBrush.Stretch = Stretch.UniformToFill;
                templateSettings.ActualImageBrush = imageBrush;
            }

            imageBrush.ImageSource = imageSrc;
        }
        else
        {
            templateSettings.ActualImageBrush = null!;
        }

        // If the control is converted to 'Group-mode', we'll clear individual-specific information.
        // When IsGroup evaluates to false, we will restore state.
        if (IsGroup)
        {
            VisualStateManager.GoToState(this, "Group", false);
        }
        else
        {
            if (imageSrc != null)
            {
                VisualStateManager.GoToState(this, "Photo", false);
            }
            else if (!string.IsNullOrEmpty(initials))
            {
                VisualStateManager.GoToState(this, "Initials", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoPhotoOrInitials", false);
            }
        }

        UpdateAutomationName();
    }

    /// <summary>
    /// Updates the state of the Badging element.
    /// </summary>
    void UpdateBadge()
    {
        if (BadgeImageSource != null)
        {
            UpdateBadgeImageSource();
        }
        else if (BadgeNumber != 0)
        {
            UpdateBadgeNumber();
        }
        else if (!string.IsNullOrEmpty(BadgeGlyph))
        {
            UpdateBadgeGlyph();
        }
        // No badge properties set, so clear the badge XAML
        else
        {
            VisualStateManager.GoToState(this, "NoBadge", false);

            var badgeNumberTextBlock = m_badgeNumberTextBlock;
            if (badgeNumberTextBlock != null)
            {
                badgeNumberTextBlock.Text = "";
            }

            var badgeGlyphIcon = m_badgeGlyphIcon;
            if (badgeGlyphIcon != null)
            {
                badgeGlyphIcon.Glyph = "";
            }
        }

        UpdateAutomationName();
    }

    /// <summary>
    /// Updates Badging Number text element.
    /// </summary>
    void UpdateBadgeNumber()
    {
        if (m_badgingEllipse == null || m_badgeNumberTextBlock == null)
        {
            return;
        }

        int badgeNumber = BadgeNumber;

        if (badgeNumber <= 0)
        {
            VisualStateManager.GoToState(this, "NoBadge", false);
            m_badgeNumberTextBlock.Text = "";
            return;
        }

        // should have badging number to show if we are here
        VisualStateManager.GoToState(this, "BadgeWithoutImageSource", false);

        if (badgeNumber <= 99)
        {
            m_badgeNumberTextBlock.Text = badgeNumber.ToString();
        }
        else
        {
            m_badgeNumberTextBlock.Text = "99+";
        }
    }

    /// <summary>
    /// Updates Badging Glyph element.
    /// </summary>
    void UpdateBadgeGlyph()
    {
        if (m_badgingEllipse == null || m_badgeGlyphIcon == null)
        {
            return;
        }

        string badgeGlyph = BadgeGlyph;

        if (string.IsNullOrEmpty(badgeGlyph))
        {
            VisualStateManager.GoToState(this, "NoBadge", false);
            m_badgeGlyphIcon.Glyph = "";
            return;
        }

        // should have badging Glyph to show if we are here
        VisualStateManager.GoToState(this, "BadgeWithoutImageSource", false);

        m_badgeGlyphIcon.Glyph = badgeGlyph;
    }

    /// <summary>
    /// Updates Badging Image element.
    /// </summary>
    void UpdateBadgeImageSource()
    {
        if (m_badgeImageBrush == null)
        {
            m_badgeImageBrush = (ImageBrush)GetTemplateChild("BadgeImageBrush");
        }

        if (m_badgingEllipse == null || m_badgeImageBrush == null)
        {
            return;
        }

        m_badgeImageBrush.ImageSource = BadgeImageSource;

        if (BadgeImageSource != null)
        {
            VisualStateManager.GoToState(this, "BadgeWithImageSource", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "NoBadge", false);
        }
    }

    /// <summary>
    /// Sets the UI Automation name for the control based on contact name and badge state.
    /// </summary>
    void UpdateAutomationName()
    {
        string automationName;
        string contactName;

        // The AutomationName for the control is in the format: PersonName, BadgeInformation.
        // PersonName is set based on the name / initial properties in the order below.
        // if none exist, it defaults to "Person"
        if (IsGroup)
        {
            contactName = "Group";
        }
        else if (!string.IsNullOrEmpty(DisplayName))
        {
            contactName = DisplayName;
        }
        else if (!string.IsNullOrEmpty(Initials))
        {
            contactName = Initials;
        }
        else
        {
            contactName = "Person";
        }

        // BadgeInformation portion of the AutomationName is set to 'n items' if there is a BadgeNumber,
        // or 'icon' for BadgeGlyph or BadgeImageSource. If BadgeText is specified, it will override
        // the string 'items' or 'icon'
        if (BadgeNumber > 0)
        {
            if (!string.IsNullOrEmpty(BadgeText))
            {
                automationName = string.Format(
                    "{0}, {1}",
                    contactName,
                    BadgeNumber,
                    BadgeText);
            }
            else
            {
                automationName = string.Format(
                    GetLocalizedPluralBadgeItemStringResource(BadgeNumber),
                    contactName,
                    BadgeNumber);
            }
        }
        else if (!string.IsNullOrEmpty(BadgeGlyph) || BadgeImageSource != null)
        {
            if (!string.IsNullOrEmpty(BadgeText))
            {
                automationName = string.Format(
                    "{0}, {1}",
                    contactName,
                    BadgeText);
            }
            else
            {
                automationName = string.Format(
                    "{0}, icon",
                    contactName);
            }
        }
        else
        {
            automationName = contactName;
        }

        AutomationProperties.SetName(this, automationName);
    }

    // Helper functions
    string GetLocalizedPluralBadgeItemStringResource(int numericValue)
    {
        int valueMod10 = numericValue % 10;
        string value;

        if (numericValue == 1)  // Singular
        {
            value = "{0}, {1:d} item";
        }
        else if (numericValue == 2) // 2
        {
            value = "{0}, {1:d} items";
        }
        else if (numericValue == 3 || numericValue == 4) // 3,4
        {
            value = "{0}, {1:d} items";
        }
        else if (numericValue >= 5 && numericValue <= 10) // 5-10
        {
            value = "{0}, {1:d} items";
        }
        else if (numericValue >= 11 && numericValue <= 19) // 11-19
        {
            value = "{0}, {1:d} items";
        }
        else if (valueMod10 == 1) // 21, 31, 41, etc.
        {
            value = "{0}, {1:d} items";
        }
        else if (valueMod10 >= 2 && valueMod10 <= 4) // 22-24, 32-34, 42-44, etc.
        {
            value = "{0}, {1:d} items";
        }
        else // Everything else... 0, 20, 25-30, 35-40, etc.
        {
            value = "{0}, {1:d} items";
        }

        return value;
    }

    void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        DependencyProperty property = args.Property;

        if (property == BadgeNumberProperty ||
            property == BadgeGlyphProperty ||
            property == BadgeImageSourceProperty)
        {
            UpdateBadge();
        }
        else if (property == BadgeTextProperty)
        {
            UpdateAutomationName();
        }
        else if (property == DisplayNameProperty)
        {
            OnDisplayNameChanged(args);
        }
        else if (property == ProfilePictureProperty ||
            property == InitialsProperty ||
            property == IsGroupProperty)
        {
            UpdateIfReady();
        }
        // No additional action required for s_PreferSmallImageProperty
    }

    // DependencyProperty changed event handlers
    void OnDisplayNameChanged(DependencyPropertyChangedEventArgs args)
    {
        m_displayNameInitials = InitialsGenerator.InitialsFromDisplayName(DisplayName);

        UpdateIfReady();
    }

    // Event handlers
    void OnSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        {
            bool widthChanged = args.NewSize.Width != args.PreviousSize.Width;
            bool heightChanged = args.NewSize.Height != args.PreviousSize.Height;
            double newSize;

            if (widthChanged && heightChanged)
            {
                // Maintain circle by enforcing the new size on both Width and Height.
                // To do so, we will use the minimum value.
                newSize = (args.NewSize.Width < args.NewSize.Height) ? args.NewSize.Width : args.NewSize.Height;
            }
            else if (widthChanged)
            {
                newSize = args.NewSize.Width;
            }
            else if (heightChanged)
            {
                newSize = args.NewSize.Height;
            }
            else
            {
                return;
            }

            Height = newSize;
            Width = newSize;
        }

        // Calculate the FontSize of the control's text. Design guidelines have specified the
        // font size to be 42% of the container. Since it's circular, 42% of either Width or Height.
        // Note that we cap it to a minimum of 1, since a font size of less than 1 is an invalid value
        // that will result in a failure.
        double fontSize = Math.Max(1.0, Width * .42);

        var initialsTextBlock = m_initialsTextBlock;
        if (initialsTextBlock != null)
        {
            initialsTextBlock.FontSize = fontSize;
        }

        if (m_badgingEllipse != null && m_badgingBackgroundEllipse != null && m_badgeNumberTextBlock != null && m_badgeGlyphIcon != null)
        {
            // Maintain badging circle and font size by enforcing the new size on both Width and Height.
            // Design guidelines have specified the font size to be 60% of the badging plate, and we want to keep
            // badging plate to be about 50% of the control so that don't block the initial/profile picture.
            double newSize = (args.NewSize.Width < args.NewSize.Height) ? args.NewSize.Width : args.NewSize.Height;
            m_badgingEllipse.Height = newSize * 0.5;
            m_badgingEllipse.Width = newSize * 0.5;
            m_badgingBackgroundEllipse.Height = newSize * 0.5;
            m_badgingBackgroundEllipse.Width = newSize * 0.5;
            m_badgeNumberTextBlock.FontSize = Math.Max(1.0, m_badgingEllipse.Height * 0.6);
            m_badgeGlyphIcon.FontSize = Math.Max(1.0, m_badgingEllipse.Height * 0.6);
        }
    }

    void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        //if (auto profilePictureReadAsync = m_profilePictureReadAsync.get())
        //{
        //    profilePictureReadAsync.Cancel();
        //}
    }

    /// <summary>
    /// XAML Element for the first TextBlock matching x:Name of InitialsTextBlock.
    /// </summary>
    private TextBlock m_initialsTextBlock = null!;

    /// <summary>
    /// XAML Element for the first TextBlock matching x:Name of BadgeNumberTextBlock.
    /// </summary>
    private TextBlock m_badgeNumberTextBlock = null!;

    /// <summary>
    /// XAML Element for the first TextBlock matching x:Name of BadgeGlyphIcon.
    /// </summary>
    private FontIcon m_badgeGlyphIcon = null!;

    /// <summary>
    /// XAML Element for the first ImageBrush matching x:Name of BadgeImageBrush.
    /// </summary>
    private ImageBrush m_badgeImageBrush = null!;

    /// <summary>
    /// XAML Element for the first Ellipse matching x:Name of BadgingBackgroundEllipse.
    /// </summary>
    private Ellipse m_badgingEllipse = null!;

    /// <summary>
    /// XAML Element for the first Ellipse matching x:Name of BadgingEllipse.
    /// </summary>
    private Ellipse m_badgingBackgroundEllipse = null!;

    /// <summary>
    /// The async operation object representing the loading and assignment of the Thumbnail.
    /// </summary>
    //tracker_ref<winrt::IAsyncOperation<winrt::IRandomAccessStreamWithContentType>> m_profilePictureReadAsync;

    /// <summary>
    /// The initials from the DisplayName property.
    /// </summary>
    private string m_displayNameInitials = null!;

    /// <summary>
    /// The initials from the Contact property.
    /// </summary>
    private string m_contactDisplayNameInitials = null!;

    /// <summary>
    /// The ImageSource from the Contact property.
    /// </summary>
    private ImageSource m_contactImageSource = null!;
}
