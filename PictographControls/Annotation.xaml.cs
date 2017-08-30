using System;
using System.Collections.Generic;
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

namespace PictographControls
{
    public enum AnnotationColourScheme
    {
        Note,
        Memo,
        Warning,
        Alarm,
        BadNews
    }

    public enum TextDirection
    {
        LeftToRight,
        TopToBottom
    }

    /// <summary>
    /// Interaction logic for Annotation.xaml
    /// </summary>
    public partial class Annotation : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Annotation));
        public static readonly DependencyProperty ColourSchemeProperty = DependencyProperty.Register("ColourScheme", typeof(AnnotationColourScheme), typeof(Annotation));
        public static readonly DependencyProperty TextOrientationProperty = DependencyProperty.Register("TextOrientation", typeof(TextDirection), typeof(Annotation));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public new double FontSize
        {
            get { return tNote.FontSize; }
            set
            {
                tNote.FontSize = value;
                txNote.FontSize = value;
            }
        }

        public TextDirection TextOrientation
        {
            get { return (TextDirection)GetValue(TextOrientationProperty); }
            set
            {
                TextDirection current = TextOrientation;
                if (value != current)
                {
                    if (LayoutTransform != null && LayoutTransform is RotateTransform)
                    {
                        double angle = ((RotateTransform)LayoutTransform).Angle;
                        if (value == TextDirection.LeftToRight)
                            angle += 90.0;
                        else
                            angle -= 90.0;
                        ((RotateTransform)LayoutTransform).Angle = angle;
                    }
                    else
                    {
                        RotateTransform rotation = new RotateTransform();
                        rotation.Angle = value == TextDirection.LeftToRight ? 0.0 : -90.0;
                        LayoutTransform = rotation;
                    }
                    

                    SetValue(TextOrientationProperty, value);
                    UpdateLayout();
                }
            }
        }

        public AnnotationColourScheme ColourScheme
        {
            get { return (AnnotationColourScheme)GetValue(ColourSchemeProperty); }
            set { SetValue(ColourSchemeProperty, value); }
        }

        public Annotation()
        {
            InitializeComponent();
            Text = "Your note here";
            ColourScheme = AnnotationColourScheme.Note;
        }

        public void GoToEdit()
        {
            tNote_MouseLeftButtonUp(null, null);
        }

        private void tNote_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txNote.Text = Text;
            txNote.Visibility = Visibility.Visible;
            tNote.Visibility = Visibility.Hidden;
            txNote.Select(0, txNote.Text.Length);
            txNote.Focus();
        }

        private void txNote_LostFocus(object sender, RoutedEventArgs e)
        {
            Text = txNote.Text;
            txNote.Visibility = Visibility.Hidden;
            tNote.Visibility = Visibility.Visible;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (ColourScheme)
            {
                case AnnotationColourScheme.Note:
                    {
                        ColourScheme = AnnotationColourScheme.Memo;
                        rBox.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xAF, 0xF4, 0xF7));
                        rBox.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x16, 0x9C, 0xC3));
                        tNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x16, 0x9C, 0xC3));
                        txNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x16, 0x9C, 0xC3));
                    }
                    break;
                case AnnotationColourScheme.Memo:
                    {
                        ColourScheme = AnnotationColourScheme.Warning;
                        rBox.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        rBox.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xC7, 0x1F, 0x1F));
                        tNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xC7, 0x1F, 0x1F));
                        txNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xC7, 0x1F, 0x1F));
                    }
                    break;
                case AnnotationColourScheme.Warning:
                    {
                        ColourScheme = AnnotationColourScheme.Alarm;
                        rBox.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xC7, 0x1F, 0x1F));
                        rBox.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        tNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        txNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    }
                    break;
                case AnnotationColourScheme.Alarm:
                    {
                        ColourScheme = AnnotationColourScheme.BadNews;
                        rBox.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
                        rBox.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        tNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        txNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    }
                    break;
                case AnnotationColourScheme.BadNews:
                default:
                    {
                        ColourScheme = AnnotationColourScheme.Note;
                        rBox.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF9, 0xEE, 0xA7));
                        rBox.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFD, 0xB4, 0x19));
                        tNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFD, 0xB4, 0x19));
                        txNote.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFD, 0xB4, 0x19));
                    }
                    break;
            }
        }
    }
}
