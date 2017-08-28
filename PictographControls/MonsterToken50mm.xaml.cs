using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    public partial class MonsterToken50mm : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MonsterToken50mm));
        public static readonly DependencyProperty AppearanceProperty = DependencyProperty.Register("Appearance", typeof(TokenColor), typeof(MonsterToken50mm));
        public static readonly DependencyProperty FacingNorthProperty = DependencyProperty.Register("FacingNorth", typeof(Visibility), typeof(MonsterToken50mm));
        public static readonly DependencyProperty FacingEastProperty = DependencyProperty.Register("FacingEast", typeof(Visibility), typeof(MonsterToken50mm));
        public static readonly DependencyProperty FacingWestProperty = DependencyProperty.Register("FacingWest", typeof(Visibility), typeof(MonsterToken50mm));
        public static readonly DependencyProperty FacingSouthProperty = DependencyProperty.Register("FacingSouth", typeof(Visibility), typeof(MonsterToken50mm));
        public static readonly DependencyProperty FacingProperty = DependencyProperty.Register("Facing", typeof(MonsterFacing), typeof(MonsterToken50mm));
        private DateTime _downPress;
        private DateTime _downPressRight;

        public Visibility FacingNorth
        {
            get
            {
                return (Visibility)GetValue(FacingNorthProperty);
            }

            set
            {
                if (value == Visibility.Collapsed)
                    return;

                SetValue(FacingNorthProperty, value);

                if (value == Visibility.Visible)
                {
                    SetValue(FacingProperty, MonsterFacing.North);
                    SetValue(FacingSouthProperty, Visibility.Hidden);
                    SetValue(FacingWestProperty, Visibility.Hidden);
                    SetValue(FacingEastProperty, Visibility.Hidden);
                }
            }
        }

        public Visibility FacingSouth
        {
            get
            {
                return (Visibility)GetValue(FacingSouthProperty);
            }

            set
            {
                if (value == Visibility.Collapsed)
                    return;

                SetValue(FacingSouthProperty, value);

                if (value == Visibility.Visible)
                {
                    SetValue(FacingProperty, MonsterFacing.South);
                    SetValue(FacingNorthProperty, Visibility.Hidden);
                    SetValue(FacingWestProperty, Visibility.Hidden);
                    SetValue(FacingEastProperty, Visibility.Hidden);
                }
            }
        }

        public Visibility FacingEast
        {
            get
            {
                return (Visibility)GetValue(FacingEastProperty);
            }

            set
            {
                if (value == Visibility.Collapsed)
                    return;

                SetValue(FacingEastProperty, value);
                if (value == Visibility.Visible)
                {
                    SetValue(FacingProperty, MonsterFacing.East);
                    SetValue(FacingNorthProperty, Visibility.Hidden);
                    SetValue(FacingWestProperty, Visibility.Hidden);
                    SetValue(FacingSouthProperty, Visibility.Hidden);
                }
            }
        }

        public Visibility FacingWest
        {
            get
            {
                return (Visibility)GetValue(FacingWestProperty);
            }

            set
            {
                if (value == Visibility.Collapsed)
                    return;

                SetValue(FacingWestProperty, value);
                if (value == Visibility.Visible)
                {
                    SetValue(FacingProperty, MonsterFacing.West);
                    SetValue(FacingNorthProperty, Visibility.Hidden);
                    SetValue(FacingEastProperty, Visibility.Hidden);
                    SetValue(FacingSouthProperty, Visibility.Hidden);
                }
            }
        }

        public MonsterFacing Facing
        {
            get
            {
                return (MonsterFacing)GetValue(FacingProperty);
            }
            set
            {
                SetValue(FacingProperty, value);
                SetValue(FacingNorthProperty, value == MonsterFacing.North ? Visibility.Visible : Visibility.Hidden);
                SetValue(FacingEastProperty, value == MonsterFacing.East ? Visibility.Visible : Visibility.Hidden);
                SetValue(FacingWestProperty, value == MonsterFacing.West ? Visibility.Visible : Visibility.Hidden);
                SetValue(FacingSouthProperty, value == MonsterFacing.South ? Visibility.Visible : Visibility.Hidden);
            }
        }

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
                    formattedValue = "M";
                else if (value.Length == 1)
                    formattedValue = value.ToUpper();
                else if (value.Length == 2)
                    formattedValue = value.Substring(0, 1).ToUpper() + value.Substring(1, 1);
                else
                    formattedValue = value.Substring(0, 1).ToUpper() + value.Substring(1, 2);
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
                fNorth.Foreground = stroke;
                fEast.Foreground = stroke;
                fSouth.Foreground = stroke;
                fWest.Foreground = stroke;
            }
        }

        public MonsterToken50mm()
        {
            InitializeComponent();
            DataContext = this;
            Text = "M";
            Appearance = TokenColor.Token;
            Facing = MonsterFacing.South;
        }

        private void tokenBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((DateTime.Now - _downPressRight).TotalMilliseconds < 800)
            {
                int appearanceValue = (int)Appearance;
                appearanceValue++;
                if (Enum.IsDefined(typeof(TokenColor), appearanceValue))
                    Appearance = (TokenColor)appearanceValue;
                else
                    Appearance = TokenColor.Token;
            }
            else
            {
                switch (Facing)
                {
                    case MonsterFacing.North: Facing = MonsterFacing.East; break;
                    case MonsterFacing.East: Facing = MonsterFacing.South; break;
                    case MonsterFacing.South: Facing = MonsterFacing.West; break;
                    case MonsterFacing.West: Facing = MonsterFacing.North; break;
                }
            }
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

        private void tokenBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _downPressRight = DateTime.Now;
        }
    }
}
