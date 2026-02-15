using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;

namespace casino
{
    /// <summary>
    /// Interaction logic for blackjack.xaml
    /// </summary>
    public partial class blackjack : Page
    {
        public blackjack()
        {
            InitializeComponent();
            egyenlegTB.Text = "Egyenleg: " +  home.Egyenleg.ToString("N0") + " Ft";
        }
        private void vissza_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private int GetBetValue()
        {
            int value;
            string content = bet.Content.ToString().Replace(" Ft", "");
            if (int.TryParse(content, NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out value))
                return value;

            return 0;
        }

        private void SetBetValue(int value)
        {
            if (value <0)
                value =0;

            int egyenleg = (int)home.Egyenleg;
            if (value > egyenleg)
                value = egyenleg;

            bet.Content = value.ToString("N0") + " Ft";
            chip.Content = value.ToString("N0");
            if (value != 0)
            {
                chip.Visibility = Visibility.Visible;
            }
            else            {
                chip.Visibility = Visibility.Hidden;
            }
        }

        private void minus_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val -=100;
            SetBetValue(val);
        }

        private void minusx_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val -=500;
            SetBetValue(val);
        }

        private void plus_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val +=100;
            SetBetValue(val);
        }

        private void plusx_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val +=500;
            SetBetValue(val);
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            bet.Content = "0 Ft";
            chip.Visibility = Visibility.Hidden;
        }

        private void deal_Click(object sender, RoutedEventArgs e)
        {
            int tet;
            int.TryParse(bet.Content.ToString().Replace(" Ft", ""), NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out tet);
            if (tet != 0)
            {
                tetkezelo.Visibility = Visibility.Hidden;
                vissza.Visibility = Visibility.Hidden;
                kezelo.Visibility = Visibility.Visible;
                egyenlegTB.Text = "Egyenleg: " + (home.Egyenleg - GetBetValue()).ToString("N0") + " Ft";
                
            }
            
        }
    }
}
