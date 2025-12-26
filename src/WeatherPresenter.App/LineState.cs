using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace WeatherPresenter.App;

public class LineState
{
    private Point _lastPoint;
    private bool _lastState;

    public Line? AddSegment(Point point, bool left, IBrush brush, int thickness)
    {
        // Starting a new line
        if (left && !_lastState)
        {
            _lastPoint = point;
            _lastState = left;

            return null;
        }

        // Ending a line
        if (!left && _lastState)
        {
            _lastState = left;

            return null;
        }

        // Not drawing a line
        if (!left) return null;

        // Adding a segment to existing line
        var distance = Math.Sqrt(Math.Pow(_lastPoint.X - point.X, 2) + Math.Pow(_lastPoint.Y - point.Y, 2));

        // Wait for the mouse to move more before drawing
        if (!(distance > 5)) return null;

        Line line = new()
        {
            StrokeThickness = thickness,
            Stroke = brush,
            StartPoint = new Point(_lastPoint.X, _lastPoint.Y),
            EndPoint = new Point(point.X, point.Y)
        };

        _lastPoint = point;

        return line;
    }
}