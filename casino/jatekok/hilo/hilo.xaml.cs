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
        private int GetCardValue(string rank)
        {
            if (int.TryParse(rank, out int value))
                return value; // 2-10 esetén a szám értéke
            if (rank == "J") 
            {
                return 11; // Bubi értéke 11
            } 
            if (rank == "Q")
            {
                return 12; // Dáma értéke 12
            }
            if (rank == "K")
            {
                return 13; // Király értéke 13
            }
            if (rank == "A")
                return 14; // Ász értéke 14
            return 0;
        }
        public static double szorzoSzamol(int cardValue, string nagyobbVkisebb, double hazelony = 0.05)
        {
            // cardValue: 2-10, 11=J, 12=Q, 13=K, 14=Ász
            int osszkartyak = 51;
            int nyerokartyak;

            if (nagyobbVkisebb == "higher")
            {
                // hány lap nagyobb ennél?
                nyerokartyak = (14 - cardValue) * 4;
            }
            else
            {
                // hány lap kisebb ennél?
                nyerokartyak = (cardValue - 2) * 4;
            }

            if (nyerokartyak <= 0) return 0; // lehetetlen tipp (Ász-ra NAGYOBB, 2-re KISEBB)

            double valoszinuseg = (double)nyerokartyak / osszkartyak;
            double osszeg = (1.0 / valoszinuseg) * (1.0 - hazelony);

            return Math.Round(osszeg, 2);
        }
        string[] suits = { "C", "D", "H", "S" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        string elozo = "";
        string suit = "";
        int ertek;
        bool legkisebb = false;
        bool legnagyobb = false;
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
            ertek = GetCardValue(elozo);
            legkisebb = false;
            legnagyobb = false;
            if (ertek == 2)
            {
                legkisebb = true;
            }
            else if (ertek == 14)
            {
                legnagyobb = true;
            }
        }

        private void regenerate_Click(object sender, RoutedEventArgs e)
        {
            randomCard();
        }
        private void buttonEnabled()
        {
            if (legkisebb)
            {
                higher.IsEnabled = true;
                lower.IsEnabled = false;
            }
            else if (legnagyobb)
            {
                higher.IsEnabled = false;
                lower.IsEnabled = true;
            }
            else
            {
                higher.IsEnabled = true;
                lower.IsEnabled = true;
            }
        }
        private void dontes(object sender, RoutedEventArgs e)
        {
            int elozoertek = ertek;
            Button megnyomott = sender as Button;
            randomCard();
            buttonEnabled();
            double tet = GetBetValue();
            if (ertek < elozoertek && megnyomott.Name == "lower")
            {
                bet.Content = (tet * szorzoSzamol(elozoertek, megnyomott.Name)).ToString("N0") + " Ft";
            }
            else if (ertek > elozoertek && megnyomott.Name == "higher")
            {
                bet.Content = (tet * szorzoSzamol(elozoertek, megnyomott.Name)).ToString("N0") + " Ft";
            }
            else
            {
                vissza.Visibility = Visibility.Visible;
                regenerate.Visibility = Visibility.Visible;
                tetkezelo.Visibility = Visibility.Visible;
                kezelo.Visibility = Visibility.Collapsed;
                bet.Content = "0 Ft";
                MessageBox.Show("Vesztettél! Próbáld újra!");
            }
        }

        private void deal_Click(object sender, RoutedEventArgs e)
        {
            if (bet.Content.ToString() != "0 Ft")
            {
                vissza.Visibility = Visibility.Hidden;
                regenerate.Visibility = Visibility.Hidden;
                tetkezelo.Visibility = Visibility.Collapsed;
                kezelo.Visibility = Visibility.Visible;
                buttonEnabled();
                EgyenlegManager.Balance.Egyenleg -= GetBetValue();
                egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            }
        }

        private void cashout_Click(object sender, RoutedEventArgs e)
        {
            vissza.Visibility = Visibility.Visible;
            regenerate.Visibility = Visibility.Visible;
            tetkezelo.Visibility = Visibility.Visible;
            kezelo.Visibility = Visibility.Collapsed;
            EgyenlegManager.Balance.Egyenleg += GetBetValue();
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            bet.Content = "0 Ft";
        }
    }
}
