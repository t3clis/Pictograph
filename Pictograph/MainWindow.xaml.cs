using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pictograph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULT_DIR = "Pictograph_";
        private const string DEFAULT_FILE = "pg";
        public string SaveDirectory { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SaveDirectory = GetSaveDirectory();
            Icon = new BitmapImage(new Uri("Resources/Pictograph.png", UriKind.Relative));
            floorScaleSlider.MouseDoubleClick += new MouseButtonEventHandler(RestoreScalingFactor);
        }

        private string GetSaveDirectory()
        {
            string baseDir = Directory.GetCurrentDirectory();
            int i = 0;

            do
            {
                string dir = baseDir + "\\" + DEFAULT_DIR + i++.ToString().PadLeft(2, '0') + "\\";

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    return dir;
                }
            } while (i < 100);

            throw new InvalidOperationException("Cannot find an available directory");
        }

        void RestoreScalingFactor(object sender, MouseButtonEventArgs args)
        {

            ((Slider)sender).Value = 1.0;
        }

        private void Snap_Click(object sender, RoutedEventArgs e)
        {
            string filePath = GetFilePath();

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)sViewport.ActualWidth, (int)sViewport.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(gViewport);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream fileStream = File.Create(filePath))
            {
                pngImage.Save(fileStream);
            }

            tSnapResult.Text = System.IO.Path.GetFileName(filePath) + " saved.";
        }

        private string GetFilePath()
        {
            string file_prefix = SaveDirectory + DEFAULT_FILE;
            int i = 0;

            do
            {
                string fn = file_prefix +  i++.ToString().PadLeft(3, '0') + ".png";

                if (!File.Exists(fn))
                {
                    return fn;
                }

            } while (i < 1000);

            throw new InvalidOperationException("Cannot find an available file name");
        }
    }
}
