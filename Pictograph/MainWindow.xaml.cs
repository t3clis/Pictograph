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
        private Visual _dragged;
        private TokenColor _nextColor;

        public string SaveDirectory { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SaveDirectory = GetSaveDirectory();
            _nextColor = TokenColor.Blue;

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

                    ((SurvivorToken)item).Appearance = _nextColor;

                    if (_nextColor != TokenColor.Token)
                    {
                        switch (_nextColor)
                        {
                            case TokenColor.Blue:
                                _nextColor = TokenColor.Brown;
                                ((SurvivorToken)item).Text = "A";
                                break;
                            case TokenColor.Brown:
                                _nextColor = TokenColor.Green;
                                ((SurvivorToken)item).Text = "B";
                                break;
                            case TokenColor.Green:
                                _nextColor = TokenColor.Yellow;
                                ((SurvivorToken)item).Text = "C";
                                break;
                            case TokenColor.Yellow:
                                ((SurvivorToken)item).Text = "D";
                                _nextColor = TokenColor.Token;
                                break;
                            default:
                                _nextColor = TokenColor.Token;
                                break;
                        }
                    }

                    CreateDragDropWindow((SurvivorToken)item);
                    _dragged = (SurvivorToken)item;
                }
                else if (liMonster50mm.IsSelected)
                {
                    itemFormat = "monster50mm";
                    item = new MonsterToken50mm();
                    CreateDragDropWindow((MonsterToken50mm)item);
                    _dragged = (MonsterToken50mm)item;
                }
                else if (liMonster100mm.IsSelected)
                {
                    itemFormat = "monster100mm";
                    item = new MonsterToken100mm();
                    CreateDragDropWindow((MonsterToken100mm)item);
                    _dragged = (MonsterToken100mm)item;
                }
                else if (liMonster135mm.IsSelected)
                {
                    itemFormat = "monster135mm";
                    item = new MonsterToken135mm();
                    CreateDragDropWindow((MonsterToken135mm)item);
                    _dragged = (MonsterToken135mm)item;
                }
                else if (liAnnotation.IsSelected)
                {
                    itemFormat = "annotation";
                    item = new Annotation();
                    CreateDragDropWindow((Annotation)item);
                    _dragged = (Annotation)item;
                }

                if (item != null)
                {
                    DragDropEffects de;
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject(itemFormat, item);
                    de = DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);

                    if (de == DragDropEffects.None)
                        _dragdropWindow?.Close();
                    else if (item is Annotation)
                        ((Annotation)item).GoToEdit();
                }
            }
        }

        private void gViewport_Drop(object sender, DragEventArgs e)
        {
            Point dropPoint = e.GetPosition(gViewport);
            int deltaX = 0, deltaY = 0, top = 0, left = 0, zindex = 0;
            bool snap = true;
            FrameworkElement item = null;

            if (e.Data.GetDataPresent("survivor"))
            {
                item = e.Data.GetData("survivor") as FrameworkElement;
                deltaX = 0;
                deltaY = 0;
            }
            else if (e.Data.GetDataPresent("monster50mm"))
            {
                item = e.Data.GetData("monster50mm") as FrameworkElement;
                deltaX = 200;
                deltaY = 200;
            }
            else if (e.Data.GetDataPresent("monster100mm"))
            {
                item = e.Data.GetData("monster100mm") as FrameworkElement;
                deltaX = 300;
                deltaY = 300;
            }
            else if (e.Data.GetDataPresent("monster135mm"))
            {
                item = e.Data.GetData("monster135mm") as FrameworkElement;
                deltaX = 400;
                deltaY = 400;
            }
            else if (e.Data.GetDataPresent("annotation"))
            {
                item = e.Data.GetData("annotation") as FrameworkElement;
                deltaX = 0;
                deltaY = 0;
                snap = false;
                zindex = 1;
            }

            top = snap ? (((int)dropPoint.Y) / 100) * 100 : (int)dropPoint.Y;
            left = snap ? (((int)dropPoint.X) / 100) * 100 : (int)dropPoint.X;

            if (deltaX > 0)
            {
                if (left + deltaX > 2200)
                    left = 2200 - deltaX;
            }

            if (deltaY > 0)
            {
                if (top + deltaY > 1600)
                    top = 1600 - deltaY;
            }

            if (item != null)
            {
                Canvas.SetTop(item, top);
                Canvas.SetLeft(item, left);
                Canvas.SetZIndex(item, zindex);
                gViewport.Children.Add(item);
            }

            _dragdropWindow.Close();
        }

        private void gViewport_DragEnter(object sender, DragEventArgs e)
        {
            if (!IsDataAcceptable(e) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
            else
                e.Effects = DragDropEffects.Move;
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


            bool keyUp, keyDown, keyLeft, keyRight, update;
            keyUp = Keyboard.IsKeyDown(Key.Up);
            keyDown = Keyboard.IsKeyDown(Key.Down);
            keyLeft = Keyboard.IsKeyDown(Key.Left);
            keyRight = Keyboard.IsKeyDown(Key.Right);
            update = keyUp || keyLeft || keyRight || keyDown;

            if (update)
            {
                MonsterToken50mm m50 = _dragged as MonsterToken50mm;
                MonsterToken100mm m100 = _dragged as MonsterToken100mm;
                MonsterToken135mm m135 = _dragged as MonsterToken135mm;
                Annotation note = _dragged as Annotation;
                MonsterFacing direction = MonsterFacing.South;

                if (keyUp)
                    direction = MonsterFacing.North;
                if (keyLeft)
                    direction = MonsterFacing.West;
                if (keyRight)
                    direction = MonsterFacing.East;
                if (keyDown)
                    direction = MonsterFacing.South;

                if (m50 != null)
                {
                    m50.Facing = direction;
                    ((Rectangle)_dragdropWindow.Content).Fill = new VisualBrush(m50);
                }

                if (m100 != null)
                {
                    m100.Facing = direction;
                    ((Rectangle)_dragdropWindow.Content).Fill = new VisualBrush(m100);
                }

                if (m135 != null)
                {
                    m135.Facing = direction;
                    ((Rectangle)_dragdropWindow.Content).Fill = new VisualBrush(m135);
                }

                if (note != null)
                {
                    bool changed = false;

                    if (note.TextOrientation == TextDirection.LeftToRight)
                    {
                        if (keyDown)
                        {
                            note.TextOrientation = TextDirection.TopToBottom;
                            ((Rectangle)_dragdropWindow.Content).Fill = new VisualBrush(note);
                            changed = true;
                        }
                    }
                    else if (note.TextOrientation == TextDirection.TopToBottom)
                    {
                        if (keyRight)
                        {
                            note.TextOrientation = TextDirection.LeftToRight;
                            ((Rectangle)_dragdropWindow.Content).Fill = new VisualBrush(note);
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        Rectangle r = (Rectangle)_dragdropWindow.Content;
                        double width = r.ActualHeight;
                        double height = r.ActualWidth;

                        r.Width = width;
                        r.Height = height;
                    }
                }

                ScaleTransform scale = new ScaleTransform(floorScaleSlider.Value, floorScaleSlider.Value);
                ((Rectangle)_dragdropWindow.Content).UpdateLayout();

            }

            _dragdropWindow.Left = w32Mouse.X;
            _dragdropWindow.Top = w32Mouse.Y;
        }


        private void CreateDragDropWindow(Visual dragElement)
        {
            _dragdropWindow = new Window();
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
            else if (dragElement is MonsterToken50mm)
            {
                r.Width = 200;
                r.Height = 200;
                r.Fill = new VisualBrush(dragElement);
            }
            else if (dragElement is MonsterToken100mm)
            {
                r.Width = 300;
                r.Height = 300;
                r.Fill = new VisualBrush(dragElement);
            }
            else if (dragElement is MonsterToken135mm)
            {
                r.Width = 400;
                r.Height = 400;
                r.Fill = new VisualBrush(dragElement);
            }
            else if (dragElement is Annotation)
            {
                r.Width = ((Annotation)dragElement).ActualWidth;
                if (r.Width == 0)
                    r.Width = 120;
                r.Height = ((Annotation)dragElement).ActualHeight;
                if (r.Height == 0)
                    r.Height = 30;

                if (((Annotation)dragElement).TextOrientation == TextDirection.TopToBottom)
                {
                    double width = r.Height;
                    double height = r.Width;

                    r.Width = width;
                    r.Height = height;
                }

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
            result |= e.Data.GetDataPresent("annotation");

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
                    _dragged = (SurvivorToken)item;
                    gViewport.Children.Remove(element);
                }
                else if (element is MonsterToken50mm)
                {
                    itemFormat = "monster50mm";
                    item = element;
                    CreateDragDropWindow((MonsterToken50mm)item);
                    _dragged = (MonsterToken50mm)item;
                    gViewport.Children.Remove(element);
                }
                else if (element is MonsterToken100mm)
                {
                    itemFormat = "monster100mm";
                    item = element;
                    CreateDragDropWindow((MonsterToken100mm)item);
                    _dragged = (MonsterToken100mm)item;
                    gViewport.Children.Remove(element);
                }
                else if (element is MonsterToken135mm)
                {
                    itemFormat = "monster135mm";
                    item = element;
                    CreateDragDropWindow((MonsterToken135mm)item);
                    _dragged = (MonsterToken135mm)item;
                    gViewport.Children.Remove(element);
                }
                else if (element is Annotation)
                {
                    itemFormat = "annotation";
                    item = element;
                    CreateDragDropWindow((Annotation)item);
                    _dragged = (Annotation)item;
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

        private void TextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.ShowDialog();
        }

        private void gViewport_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
