using System;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace SQLDocumentor.DiagramRenderer.Converter
{
    /// <summary>
    /// Converts the position and sizes of the source and target points, and the route informations
    /// of an edge to a path.
    /// The edge can bend, or it can be straight line.
    /// </summary>
    public class EdgeLabelLeftConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(values != null && values.Length == 9, "EdgeRouteToPathConverter should have 9 parameters: pos (1,2), size (3,4) of source; pos (5,6), size (7,8) of target; routeInformation (9).");

            Point sourcePos = new Point()
            {
                X = (values[0] != DependencyProperty.UnsetValue ? (double)values[0] : 0.0),
                Y = (values[1] != DependencyProperty.UnsetValue ? (double)values[1] : 0.0)
            };

            Point targetPos = new Point()
            {
                X = (values[4] != DependencyProperty.UnsetValue ? (double)values[4] : 0.0),
                Y = (values[5] != DependencyProperty.UnsetValue ? (double)values[5] : 0.0)
            };

            if (sourcePos.X > targetPos.X)
            {
                return sourcePos.X - ((sourcePos.X - targetPos.X)/2.0);
            }

            return sourcePos.X + ((targetPos.X - sourcePos.X) / 2.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Point CalculateAttachPoint(Point s, Size sourceSize, Point t)
        {
            double[] sides = new double[4];
            sides[0] = (s.X - sourceSize.Width / 2.0 - t.X) / (s.X - t.X);
            sides[1] = (s.Y - sourceSize.Height / 2.0 - t.Y) / (s.Y - t.Y);
            sides[2] = (s.X + sourceSize.Width / 2.0 - t.X) / (s.X - t.X);
            sides[3] = (s.Y + sourceSize.Height / 2.0 - t.Y) / (s.Y - t.Y);

            double fi = 0;
            for (int i = 0; i < 4; i++)
            {
                if (sides[i] <= 1)
                    fi = Math.Max(fi, sides[i]);
            }

            return t + fi * (s - t);
        }
    }
}