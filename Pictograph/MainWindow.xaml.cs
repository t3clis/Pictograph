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
using PictographControls;
using System.Timers;
using System.Runtime.InteropServices;

namespace Pictograph
{
    public partial class MainWindow : Window
    {
        private const string DEFAULT_DIR = "Pictograph Snapshots";
        private const string DEFAULT_FILE = "pg";

        private Point _toolDragStart;
        private Window _dragdropWindow;

        public string SaveDirectory { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SaveDirectory = GetSaveDirectory();
            Icon = new BitmapImage(new Uri("Resources/Pictograph.png", UriKind.Relative));

            for (int i = 0; i < 22; i++)
                for (int j = 0; j < 16; j++)
                {
                    TiledButton t = new TiledButton();
                    Canvas.SetLeft(t, i * 100);
                    Canvas.SetTop(t, j * 100);
                    gViewport.Children.Add(t);
                }

            floorScaleSlider.MouseDoubleClick += new MouseButtonEventHandler(RestoreScalingFactor);
        }

        private string GetSaveDirectory()
        {
            string baseDir = Directory.GetCurrentDirectory();
            string dir = baseDir + "\\" + DEFAULT_DIR + "\\";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
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
                string fn = file_prefix + i++.ToString().PadLeft(3, '0') + ".png";

                if (!File.Exists(fn))
                {
                    return fn;
                }

            } while (i < 1000);

            throw new InvalidOperationException("Cannot find an available file name");
        }

        private void listTools_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            _toolDragStart = e.GetPosition(null);
        }

        private void listTools_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _toolDragStart - mousePos;
            string itemFormat = null;
            object item = null;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem = (ListViewItem)listView.SelectedItem;

                if (liSurvivor.IsSelected)
                {
                    itemFormat = "survivor";
                    item = new SurvivorToken();
                    CreateDragDropWindow((SurvivorToken)item);
                }

                if (item != null)
                {
                    DragDropEffects de;
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject(itemFormat, item);
                    de = DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);

                    if (de == DragDropEffects.None)
                        _dragdropWindow?.Close();
                }
            }
        }

        private void gViewport_Drop(object sender, DragEventArgs e)
        {
            Point dropPoint = e.GetPosition(gViewport);

            if (e.Data.GetDataPresent("survivor"))
            {
                int top, left;
                SurvivorToken item = e.Data.GetData("survivor") as SurvivorToken;

                top = (((int)dropPoint.Y) / 100) * 100;
                left = (((int)dropPoint.X) / 100) * 100;

                Canvas.SetTop(item, top);
                Canvas.SetLeft(item, left);
                gViewport.Children.Add(item);
                _dragdropWindow.Close();
            }
        }

        private void gViewport_DragEnter(object sender, DragEventArgs e)
        {
            if (!IsDataAcceptable(e) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
                Snap_Click(this, e);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private void gViewport_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
        }


        private void CreateDragDropWindow(Visual dragElement)
        {
            this._dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            Rectangle r = new Rectangle();
            if (dragElement is SurvivorToken)
            {
                r.Width = 100;
                r.Height = 100;
                r.Fill = new VisualBrush(dragElement);
            }

            

            _dragdropWindow.Content = r;

            ScaleTransform scale = new ScaleTransform(floorScaleSlider.Value, floorScaleSlider.Value);
            r.LayoutTransform = scale;
            r.UpdateLayout();


            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);


            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
            this._dragdropWindow.Show();
        }

        private void bDelete_Drop(object sender, DragEventArgs e)
        {
            Point dropPoint = e.GetPosition(gViewport);
            _dragdropWindow.Close();
        }

        private void bDelete_DragEnter(object sender, DragEventArgs e)
        {
            if (!IsDataAcceptable(e) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private bool IsDataAcceptable(DragEventArgs e)
        {
            bool result = e.Data.GetDataPresent("survivor");
            result |= e.Data.GetDataPresent("monster50mm");
            result |= e.Data.GetDataPresent("monster100mm");
            result |= e.Data.GetDataPresent("monster135mm");

            return result;
        }

        private void gViewport_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Point posInCanvas = e.GetPosition(gViewport);

            Vector diff = _toolDragStart - mousePos;
            string itemFormat = null;
            object item = null;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                FrameworkElement directlyOver = (FrameworkElement)Mouse.DirectlyOver;

                if (directlyOver is TextBox)
                    return;

                FrameworkElement element = FindRelevantAncestor(directlyOver, gViewport);

                

                if (element is SurvivorToken)
                {
                    itemFormat = "survivor";
                    item = element;
                    CreateDragDropWindow((SurvivorToken)item);
                    gViewport.Children.Remove(element);
                }

                if (item != null)
                {
                    DragDropEffects de;
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject(itemFormat, item);
                    bDelete.Visibility = Visibility.Visible;
                    de = DragDrop.DoDragDrop(gViewport, dragData, DragDropEffects.Move);

                    if (de == DragDropEffects.None)
                        _dragdropWindow?.Close();
                    bDelete.Visibility = Visibility.Hidden;
                }
            }
        }

        private FrameworkElement FindRelevantAncestor(FrameworkElement element, FrameworkElement stopAt)
        {
            FrameworkElement lastElement = null;
            FrameworkElement currentElement = element;
            while (currentElement != stopAt)
            {
                if (currentElement == null)
                    return null;

                lastElement = currentElement;
                currentElement = (FrameworkElement)currentElement.Parent;
            }

            return lastElement;
        }
    }
}
