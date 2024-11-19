﻿using System;

namespace Wpf.Ui.Violeta.Controls;

public interface IPendingHandler : IDisposable
{
    public event EventHandler? Closed;

    public event EventHandler? Cancel;

    public string? Message { get; set; }
    public bool Cancelable { get; set; }
    public bool Canceled { get; }

    public void Show();

    public bool? ShowDialog();

    public void Close();
}