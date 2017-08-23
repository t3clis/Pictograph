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
    public enum TiledButtonState
    {
        Base,
        Move,
        Dash,
        BlindSpot,
        SolidObject,
        Hole,
        Block,
        Range
    }

    public partial class TiledButton : UserControl
    {
        public TiledButtonState TileState { get; set; }

        public TiledButton()
        {
            InitializeComponent();
            TileState = TiledButtonState.Base;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            switch (TileState)
            {
                case TiledButtonState.Base:
                    {
                        b.Style = (Style)FindResource("styleMove");
                        TileState = TiledButtonState.Move;
                    }
                    break;
                case TiledButtonState.Move:
                    {
                        b.Style = (Style)FindResource("styleDash");
                        TileState = TiledButtonState.Dash;
                    }
                    break;
                case TiledButtonState.Dash:
                    {
                        b.Style = (Style)FindResource("styleBlindSpot");
                        TileState = TiledButtonState.BlindSpot;
                    }
                    break;
                case TiledButtonState.BlindSpot:
                    {
                        b.Style = (Style)FindResource("styleSolidObject");
                        TileState = TiledButtonState.SolidObject;
                    }
                    break;
                case TiledButtonState.SolidObject:
                    {
                        b.Style = (Style)FindResource("styleHole");
                        TileState = TiledButtonState.Hole;
                    }
                    break;
                case TiledButtonState.Hole:
                    {
                        b.Style = (Style)FindResource("styleBlock");
                        TileState = TiledButtonState.Block;
                    }
                    break;
                case TiledButtonState.Block:
                    {
                        b.Style = (Style)FindResource("styleRange");
                        TileState = TiledButtonState.Range;
                    }
                    break;
                case TiledButtonState.Range:
                default:
                    {
                        b.Style = (Style)FindResource("styleBase");
                        TileState = TiledButtonState.Base;
                    }
                    break;
            }
        }

        private void Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button b = sender as Button;
            b.Style = (Style)FindResource("styleBase");
            TileState = TiledButtonState.Base;
        }
    }
}
