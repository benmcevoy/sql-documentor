using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SQLDocumentor.DiagramRenderer.Graph;

namespace SQLDocumentor.DiagramRenderer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DiagramView : Window
    {
        public TableGraph GraphToVisualize
        {
            get { return ( TableGraph)GetValue(GraphToVisualizeProperty); }
            set { SetValue(GraphToVisualizeProperty, value); }
        }

        public static readonly DependencyProperty GraphToVisualizeProperty =
            DependencyProperty.Register("GraphToVisualize", typeof( TableGraph), typeof(DiagramView));

        public DiagramView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            graphLayout.Relayout();
        }

        private void SavePNG_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.Filter = "png|*.png";
            dialog.RestoreDirectory = true;

            var result = dialog.ShowDialog();
            
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                var stream = dialog.OpenFile();

                if (stream != null)
                {
                    PngFromVisual(graphLayout, new Size(1000, 1000)).CopyTo(stream);
                    stream.Close();
                }
            }
        }

        private const double DEFAULT_DPI = 96.0;

        public Stream PngFromVisual(FrameworkElement element, Size desiredSize)
        {
            var dpi = new Point(150,150);

            var renderBitmap =
                new RenderTargetBitmap(
                    (int)(element.ActualWidth / DEFAULT_DPI * dpi.X), (int)(element.ActualHeight / DEFAULT_DPI * dpi.Y),
                    dpi.X, dpi.Y, PixelFormats.Pbgra32);

            renderBitmap.Render(element);

            var encoder = new BmpBitmapEncoder();
            var stream = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
