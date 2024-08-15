![WPF UI Banner Dark](https://user-images.githubusercontent.com/13592821/174165081-9c62d188-ecb6-4200-abd8-419afbaf32c2.png#gh-dark-mode-only)

![WPF UI Banner Light](https://user-images.githubusercontent.com/13592821/174165388-921c4745-90ed-4396-9a4b-9c86478f7447.png#gh-light-mode-only)

# WPF UI Violeta

[![GitHub license](https://img.shields.io/github/license/emako/wpfui.violeta)](https://github.com/emako/wpfui.violeta/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/v/WPF-UI.Violeta.svg)](https://nuget.org/packages/WPF-UI.Violeta) [![VS 2022 Downloads](https://img.shields.io/visual-studio-marketplace/i/lepo.WPF-UI?label=vs-2022)](https://marketplace.visualstudio.com/items?itemName=lepo.WPF-UI) [![Actions](https://github.com/emako/wpfui.violeta/actions/workflows/library.nuget.yml/badge.svg)](https://github.com/emako/wpfui.violeta/actions/workflows/library.nuget.yml) [![Platform](https://img.shields.io/badge/platform-Windows-blue?logo=windowsxp&color=1E9BFA)](https://dotnet.microsoft.com/zh-cn/download/dotnet/latest/runtime)

WPF UI Violeta is based on [WPF UI](https://github.com/lepoco/wpfui), and provides the Fluent experience in your known and loved WPF framework. Some new immersive controls like `Toast`, `Flyout`, `ContentDialog`, `MessageBox` and etc.

When I decided to create this project I was listening to the song `Violeta`.

### 🚀 Getting started

Similar to WPF UI.

```xaml
<Application
    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
    xmlns:vio="http://schemas.lepo.co/wpfui/2022/xaml/violeta">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ThemesDictionary Theme="Dark" />
                <ui:ControlsDictionary />
                <vio:ControlsDictionary />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 👋Example

[Wpf.Ui.Test](https://github.com/emako/wpfui.violeta/tree/master/src/Wpf.Ui.Test)

- **Toast**

  > `Toast` is an independent popup notification that automatically disappears after a specified time.

  ```c#
  Toast.Information("I am information message");
  Toast.Error("I am error message");
  Toast.Success("I am success message");
  Toast.Warning("I am warning message");
  Toast.Show(owner: null!, "I am any message",  new ToastConfig());
  ```

- **Flyout**
  
  > The `FlyoutService` enables you to attach `Flyout` menus or tooltips to various controls such as `Button`, providing a flexible and intuitive way to display additional content or options.
  
  ```xaml
  <ui:Button Content="Show Flyout">
      <ui:FlyoutService.Flyout>
          <ui:Flyout Placement="Bottom">
              <ui:Card>
                  <StackPanel>
                      <TextBlock
                          HorizontalAlignment="Left"
                          Text="Show the flyout message here" />
                      <Button
                         Command="{Binding GotItCommand}"
                         Content="Got it" />
                  </StackPanel>
              </ui:Card>
          </ui:Flyout>
      </ui:FlyoutService.Flyout>
  </ui:Button>
  ```

- **ContentDialog**

  > The `ContentDialogHostService` simplifies the creation and management of `ContentDialog` instances in your application.

  ```c#
  ContentDialog dialog =
      new()
      {
          Title = "My sample dialog",
          Content = "Content of the dialog",
          CloseButtonText = "Close button",
          PrimaryButtonText = "Primary button",
          SecondaryButtonText = "Secondary button"
      };
  
  // Setting the dialog container
  dialog.DialogHost = ContentDialogHostService.ContentPresenterForDialogs;
  
  // Showing the dialog
  await dialog.ShowAsync(CancellationToken.None);
  ```

- **MessageBox**

  > To utilize Win32's classic `MessageBox` methods while supporting modern UI themes like Mica and dark mode.

  ```c#
  using MessageBox = Wpf.Ui.Violeta.Controls.MessageBox;
  
  MessageBox.Information("This is a information message");
  MessageBox.Warning("This is a warning message");
  MessageBox.Error("This is a error message");
  
  MessageBoxResult result =  MessageBox.Question("This is a question and do you want to click OK?");
  ```

### 📷 Screenshots

Under construction

### Thanks

- [🔗 Fischless](https://github.com/GenshinMatrix/Fischless)
- [🔗 ModernWpf](https://github.com/Kinnara/ModernWpf)

