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

namespace casino
{
    /// <summary>
    /// Interaction logic for profile.xaml
    /// </summary>
    public partial class profile : Page
    {
        public profile()
        {
            InitializeComponent();
            BetoltAdatok();
        }
        private void vissza_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void BetoltAdatok()
        {
            egyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            kifizetesEgyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            adatEgyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";

            
        }

        private void MutaPanel(string panel)
        {
            adatokPanel.Visibility = Visibility.Collapsed;
            feltoltesPanel.Visibility = Visibility.Collapsed;
            kifizetesPanel.Visibility = Visibility.Collapsed;
            uresAllapot.Visibility = Visibility.Collapsed;

            switch (panel)
            {
                case "adatok":
                    adatokPanel.Visibility = Visibility.Visible;
                    break;
                case "feltoltes":
                    feltoltesPanel.Visibility = Visibility.Visible;
                    break;
                case "kifizetes":
                    kifizetesEgyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
                    kifizetesPanel.Visibility = Visibility.Visible;
                    break;
                default:
                    uresAllapot.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void adatok_Click(object sender, RoutedEventArgs e)
        {
            BetoltAdatok();
            MutaPanel("adatok");
        }

        private void feltoltes_Click(object sender, RoutedEventArgs e)
        {
            MutaPanel("feltoltes");
        }

        private void kifizetes_Click(object sender, RoutedEventArgs e)
        {
            MutaPanel("kifizetes");
        }

        // Feltöltés – gyorsgombok
        private void FeltoltesOsszeg_Click(object sender, RoutedEventArgs e)
        {
            Button gomb = sender as Button;
            if (gomb != null)
                feltoltesOsszegTB.Text = gomb.Tag.ToString();
        }

        // Feltöltés végrehajtása
        private void FeltoltesVegrehajt_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(feltoltesOsszegTB.Text, out int osszeg) || osszeg <= 0)
            {
                MessageBox.Show("Kérjük adj meg érvényes összeget!");
                return;
            }

            EgyenlegManager.Balance.Egyenleg += osszeg;
            egyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            adatEgyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            MessageBox.Show($"Sikeresen feltöltve: {osszeg:N0} Ft\nÚj egyenleg: {EgyenlegManager.Balance.Egyenleg:N0} Ft");
            feltoltesOsszegTB.Text = "0";
        }

        // Kifizetés végrehajtása
        private void KifizetesVegrehajt_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(kifizetesOsszegTB.Text, out int osszeg) || osszeg <= 0)
            {
                MessageBox.Show("Kérjük adj meg érvényes összeget!");
                return;
            }

            if (osszeg > EgyenlegManager.Balance.Egyenleg)
            {
                MessageBox.Show("Nincs elegendő egyenleged!");
                return;
            }

            if (string.IsNullOrWhiteSpace(kifizetesIbanTB.Text))
            {
                MessageBox.Show("Kérjük add meg a bankszámlaszámodat!");
                return;
            }

            EgyenlegManager.Balance.Egyenleg -= osszeg;
            egyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            kifizetesEgyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            MessageBox.Show($"Kifizetési kérelem elküldve: {osszeg:N0} Ft");
            kifizetesOsszegTB.Text = "0";
        }

        private void kijelentkezes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Content = null;
        }
    }
}
