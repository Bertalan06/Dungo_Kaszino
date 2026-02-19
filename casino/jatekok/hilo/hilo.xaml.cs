using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for hilo.xaml
    /// </summary>
    public partial class hilo : Page
    {
        public hilo()
        {
            InitializeComponent();
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            randomCard();
        }
        private void vissza_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        private void min_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(100);
        }
        private void minusx_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(GetBetValue() - 500);
        }
        private void minus_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(GetBetValue() - 100);
        }
        private void plus_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(GetBetValue() + 100);
        }
        private void plusx_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(GetBetValue() + 500);
        }
        private void max_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(500000);
        }
        private void SetBetValue(int value)
        {
            if (value < 0)
                value = 0;

            int egyenleg = (int)EgyenlegManager.Balance.Egyenleg;
            if (value > egyenleg)
                value = egyenleg;

            bet.Content = value.ToString("N0") + " Ft";
        }
        private int GetBetValue()
        {
            int value;
            string content = bet.Content.ToString().Replace(" Ft", "");
            if (int.TryParse(content, NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out value))
                return value;

            return 0;
        }
        string[] suits = { "C", "D", "H", "S" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        string elozo = "";
        string suit = "";
        private void randomCard()
        {
            Random rand = new Random();
            string rank;

            do
            {
                rank = ranks[rand.Next(ranks.Length)];
            }
            while (rank == elozo);

            elozo = rank;
            suit = suits[rand.Next(suits.Length)];
            var path = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "blackjack", "cards", $"{rank}{suit}.png"), UriKind.Absolute));
            kartya.Source = path;
        }

        private void regenerate_Click(object sender, RoutedEventArgs e)
        {
            randomCard();
        }

        private void lower_Click(object sender, RoutedEventArgs e)
        {

        }

        private void higher_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
