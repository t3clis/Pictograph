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
    public enum TokenColor
    {
        Token,
        Red,
        Green,
        Blue,
        Yellow,
        Brown,
        Pink,
        Black,
        Grey
    }

    public partial class SurvivorToken : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SurvivorToken));
        public static readonly DependencyProperty AppearanceProperty = DependencyProperty.Register("Appearance", typeof(TokenColor), typeof(SurvivorToken));

        private DateTime _downPress;

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                string formattedValue;
                if (value.Length == 0)
                    formattedValue = "S";
                else if (value.Length == 1)
                    formattedValue = value.ToUpper();
                else
                    formattedValue = value.Substring(0, 1).ToUpper() + value.Substring(1, 1);
                SetValue(TextProperty, formattedValue);
            }
        }

        public TokenColor Appearance
        {
            get { return (TokenColor)GetValue(AppearanceProperty); }
            set
            {
                SolidColorBrush fill, stroke;

                switch (value)
                {
                    case TokenColor.Black:
                        {
                            fill = (SolidColorBrush)FindResource("brushBlackFill");
                            stroke = (SolidColorBrush)FindResource("brushBlackStroke");
                        }
                        break;
                    case TokenColor.Blue:
                        {
                            fill = (SolidColorBrush)FindResource("brushBlueFill");
                            stroke = (SolidColorBrush)FindResource("brushBlueStroke");
                        }
                        break;
                    case TokenColor.Brown:
                        {
                            fill = (SolidColorBrush)FindResource("brushBrownFill");
                            stroke = (SolidColorBrush)FindResource("brushBrownStroke");
                        }
                        break;
                    case TokenColor.Green:
                        {
                            fill = (SolidColorBrush)FindResource("brushGreenFill");
                            stroke = (SolidColorBrush)FindResource("brushGreenStroke");
                        }
                        break;
                    case TokenColor.Grey:
                        {
                            fill = (SolidColorBrush)FindResource("brushGreyFill");
                            stroke = (SolidColorBrush)FindResource("brushGreyStroke");
                        }
                        break;
                    case TokenColor.Pink:
                        {
                            fill = (SolidColorBrush)FindResource("brushPinkFill");
                            stroke = (SolidColorBrush)FindResource("brushPinkStroke");
                        }
                        break;
                    case TokenColor.Red:
                        {
                            fill = (SolidColorBrush)FindResource("brushRedFill");
                            stroke = (SolidColorBrush)FindResource("brushRedStroke");
                        }
                        break;
                    case TokenColor.Yellow:
                        {
                            fill = (SolidColorBrush)FindResource("brushYellowFill");
                            stroke = (SolidColorBrush)FindResource("brushYellowStroke");
                        }
                        break;
                    case TokenColor.Token:
                    default:
                        {
                            fill = (SolidColorBrush)FindResource("brushTokenFill");
                            stroke = (SolidColorBrush)FindResource("brushTokenStroke");
                        }
                        break;
                }

                SetValue(AppearanceProperty, value);

                tokenBox.Fill = fill;
                tokenBox.Stroke = stroke;
                tokenText.Foreground = stroke;
                txName.Foreground = stroke;
            }
        }

        public SurvivorToken()
        {
            InitializeComponent();
            DataContext = this;
            Text = "S";
            Appearance = TokenColor.Token;
        }

        private void tokenBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            int appearanceValue = (int)Appearance;
            appearanceValue++;
            if (Enum.IsDefined(typeof(TokenColor), appearanceValue))
                Appearance = (TokenColor)appearanceValue;
            else
                Appearance = TokenColor.Token;
        }

        private void tokenText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _downPress = DateTime.Now;
        }

        private void tokenText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((DateTime.Now - _downPress).TotalMilliseconds < 500)
            {
                tokenText.Visibility = Visibility.Hidden;
                txName.Visibility = Visibility.Visible;
                txName.Text = Text;
                txName.Select(0, txName.Text.Length);
                txName.Focus();
            }

        }

        private void txName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Text = txName.Text;
            }

            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                txName.Visibility = Visibility.Hidden;
                tokenText.Visibility = Visibility.Visible;
            }
        }

        private void txName_LostFocus(object sender, RoutedEventArgs e)
        {
            Text = txName.Text;
            txName.Visibility = Visibility.Hidden;
            tokenText.Visibility = Visibility.Visible;
        }
    }
}
