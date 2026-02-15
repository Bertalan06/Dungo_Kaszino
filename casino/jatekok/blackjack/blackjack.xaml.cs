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
            egyenlegTB.Text = "Egyenleg: " +  home.Egyenleg.ToString("N0") + " Ft";            // Ensure buttons exist before trying to use 
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
            if (bet?.Content == null)
                return 0;

            int value;
            string content = bet.Content.ToString().Replace("Ft","");
            if (int.TryParse(content, NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out value))
                return value;

            return 0;
        }

        private void SetBetValue(int value)
        {
            if (value < 0)
                value = 0;
            else if (value > home.Egyenleg)
                bet.Content = home.Egyenleg.ToString("N0") + " Ft";
            else
                bet.Content = value.ToString("N0") + " Ft";
        }

        private void minus_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val -= 100;
            SetBetValue(val);
        }

        private void minusx_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val -= 500;
            SetBetValue(val);
        }

        private void plus_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val += 100;
            SetBetValue(val);
        }

        private void plusx_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val += 500;
            SetBetValue(val);
        }
    }
}
