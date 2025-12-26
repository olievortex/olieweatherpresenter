using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace WeatherPresenter.App.Models;

public class ProductButtonModel : INotifyPropertyChanged
{
    public IBrush Background
    {
        get;
        set => SetField(ref field, value);
    } = Brushes.Transparent;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}