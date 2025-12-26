using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WeatherPresenter.App.Models;

namespace WeatherPresenter.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private const int LineThickness = 5;
    private static readonly Bitmap EmptyImage = new RenderTargetBitmap(new PixelSize(100, 100));
    private static readonly IBrush DefaultBrush = Brushes.Purple;

    private static readonly string[] ButtonTags =
    [
        "Temperature", "Dewpoint", "300mb", "500mb", "Vorticity",
        "700mbR", "700mbT", "850mb", "Cin", "Cape",
        "Shear", "Srh", "Scp", "Lcl", "Stp",
        "StormMotion"
    ];

    private static readonly string[] ButtonNames =
    [
        "Tmp", "Dpt", "300", "500", "Vort",
        "700rh", "700t", "850", "CIN", "CAPE",
        "Shear", "SRH", "SCP", "LCL", "STP",
        "Storm Motion"
    ];

    private double _scale = 1;
    private double _xPos = 1;
    private double _yPos = 1;
    private int _lastButton = -1;
    private Point _lastPoint;
    private readonly LineState _lineState = new();
    private readonly List<ImageModel> _images = [];

    public MainWindowViewModel()
    {
        CommandClearCanvas = ReactiveCommand.Create(LineClearAll);
        CommandOpenImages = ReactiveCommand.CreateFromTask(ImageLoadAsync);
        CommandResetZoom = ReactiveCommand.Create(ImageIdentity);
        CommandProduct = ReactiveCommand.Create<int>(ChangeProduct);

        CallbackImageSelectPrompt = () => Task.FromResult(new List<ImageModel>());

        for (var i = 0; i < ButtonTags.Length; i++)
            ProductButtons.Add(new ProductButtonModel
            {
                Id = ProductButtons.Count,
                Tag = ButtonTags[i],
                Name = ButtonNames[i],
                Background = Brushes.DarkSlateGray
            });
    }

    #region Properties

    public Func<Task<List<ImageModel>>> CallbackImageSelectPrompt { get; set; }

    #endregion

    #region Bindings

    public ObservableCollection<Line> CanvasItems
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    public Bitmap CurrentImage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = EmptyImage;

    public double ImageHeight
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 100;

    public TransformGroup ImageTransform
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    public double ImageWidth
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 100;

    public ObservableCollection<ProductButtonModel> ProductButtons
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> CommandClearCanvas { get; }
    public ReactiveCommand<Unit, Unit> CommandOpenImages { get; }
    public ReactiveCommand<Unit, Unit> CommandResetZoom { get; }
    public ReactiveCommand<int, Unit> CommandProduct { get; }

    #endregion

    private void ChangeProduct(int id)
    {
        if (id == _lastButton) return;

        var item = ProductButtons[id];

        item.Background = Brushes.Red;
        ImageSelectByTag(item.Tag);

        ButtonResetLast(id);
    }

    #region Line Related

    public void LineAddSegment(Point point, bool isTriggered, bool isActive)
    {
        if (!isActive) return;

        var line = _lineState.AddSegment(point, isTriggered, DefaultBrush, LineThickness);
        if (line is not null) CanvasItems.Add(line);
    }

    private void LineClearAll()
    {
        CanvasItems.Clear();
    }

    #endregion

    #region Image Related

    public void ImageScale(double deltaY)
    {
        if (deltaY > 0)
        {
            _scale += 0.1;
            if (_scale > 6) _scale = 6;
        }
        else
        {
            _scale -= 0.1;
            if (_scale < 0.5) _scale = 0.5;
        }

        ImageUpdateTransform();
    }

    public void ImageTranslate(Point point, bool isActive)
    {
        if (isActive)
        {
            _xPos += point.X - _lastPoint.X;
            _yPos += point.Y - _lastPoint.Y;

            ImageUpdateTransform();
        }

        _lastPoint = point;
    }

    #region Private Helpers

    private void ImageSelectByTag(string tag)
    {
        var image = _images
            .FirstOrDefault(w => w.Filename.Contains(tag, StringComparison.OrdinalIgnoreCase));

        if (image == null) return;

        CurrentImage = image.Bitmap;
    }

    private void ImageIdentity()
    {
        _scale = 1.0;
        _xPos = 0;
        _yPos = 0;
        ImageUpdateTransform();
    }

    private void ImageUpdateTransform()
    {
        var tg = new TransformGroup();
        tg.Children.Add(new ScaleTransform(_scale, _scale));
        tg.Children.Add(new TranslateTransform(_xPos, _yPos));
        ImageTransform = tg;
    }

    private async Task ImageLoadAsync()
    {
        var images = await CallbackImageSelectPrompt();
        if (images.Count == 0) return;

        CurrentImage = EmptyImage;

        foreach (var image in _images) image.Dispose();

        _images.Clear();
        _images.AddRange(images);

        ImageIdentity();

        var bitmap = _images[0].Bitmap;

        CurrentImage = bitmap;
        ImageWidth = bitmap.Size.Width;
        ImageHeight = bitmap.Size.Height;

        ButtonResetAll();
    }

    #endregion

    #endregion

    #region Button Related

    private void ButtonResetAll()
    {
        foreach (var productButton in ProductButtons) productButton.Background = Brushes.DarkSlateGray;
    }

    private void ButtonResetLast(int id)
    {
        if (_lastButton >= 0)
        {
            var item = ProductButtons[_lastButton];
            item.Background = Brushes.DarkSlateGray;
        }

        _lastButton = id;
    }

    #endregion
}