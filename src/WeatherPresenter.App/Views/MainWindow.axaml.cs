using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using WeatherPresenter.App.Models;
using WeatherPresenter.App.ViewModels;

namespace WeatherPresenter.App.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel =>
        DataContext as MainWindowViewModel ?? throw new ApplicationException("DataContext is not MainWindowViewModel");

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        ViewModel.CallbackImageSelectPrompt = CallbackImageSelectPrompt;
    }

    #region Callbacks

    private async Task<List<ImageModel>> CallbackImageSelectPrompt()
    {
        try
        {
            return await OlieCommon.LoadImages(StorageProvider);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());

            return [];
        }
    }

    #endregion

    #region Event Handlers

    private void Window_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        ViewModel.ImageScale(e.Delta.Y);
    }

    private void Window_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var isControl = (e.KeyModifiers & KeyModifiers.Control) != 0;

        ViewModel.ImageTranslate(e.GetPosition(this), isControl);
        ViewModel.LineAddSegment(e.GetPosition(IcOverlay), e.Properties.IsLeftButtonPressed, !isControl);
    }

    #endregion
}