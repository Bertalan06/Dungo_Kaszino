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
            this.Loaded += Profile_Loaded;
        }
        private void Profile_Loaded(object sender, RoutedEventArgs e)
        {
            // Ablak SizeChanged eseményére iratkozunk fel
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.SizeChanged += ParentWindow_SizeChanged;
                // Azonnal beállítjuk
                adatokGrid.Columns = parentWindow.ActualWidth < 900 ? 1 : 2;
            }
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (adatokGrid != null)
                adatokGrid.Columns = e.NewSize.Width < 900 ? 1 : 2;
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
            avatarBetu.Text = EgyenlegManager.Name.Nev.Substring(0, 1).ToUpper();
            usernameTB.Text = EgyenlegManager.Name.Nev;
            adatokPanel.Visibility = Visibility.Visible;
            adatFullNameTB.Text = MainWindow.adatok.Where(x => x.FelhasznaloNev == EgyenlegManager.Name.Nev).FirstOrDefault().Nev;
            adatEmailTB.Text = MainWindow.adatok.Where(x => x.FelhasznaloNev == EgyenlegManager.Name.Nev).FirstOrDefault().Email;
            adatSzulTB.Text = MainWindow.adatok.Where(x => x.FelhasznaloNev == EgyenlegManager.Name.Nev).FirstOrDefault().SzuletesiDatum.ToString("yyyy-MM-dd");
            adatTelefonTB.Text = MainWindow.adatok.Where(x => x.FelhasznaloNev == EgyenlegManager.Name.Nev).FirstOrDefault().Telefonszam;
        }

        private void MutaPanel(string panel)
        {
            adatokPanel.Visibility = Visibility.Collapsed;
            feltoltesPanel.Visibility = Visibility.Collapsed;
            kifizetesPanel.Visibility = Visibility.Collapsed;

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
            if (!IbanEllenorzes(feltoltesIbanTB, rbFeltKartya, rbFeltUtalas))
                return;
            EgyenlegManager.Balance.Egyenleg += osszeg;
            egyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            MessageBox.Show($"Sikeresen feltöltve: {osszeg:N0} Ft\nÚj egyenleg: {EgyenlegManager.Balance.Egyenleg:N0} Ft");
            feltoltesOsszegTB.Clear();
            MainWindow.adatok.Where(x => x.FelhasznaloNev == EgyenlegManager.Name.Nev).FirstOrDefault().Egyenleg = EgyenlegManager.Balance.Egyenleg;
            faljbairas();
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
            if (!IbanEllenorzes(kifizetesIbanTB, rbKifKartya, rbKifUtalas))
                return;
            EgyenlegManager.Balance.Egyenleg -= osszeg;
            egyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            kifizetesEgyenlegTB.Text = EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            MessageBox.Show($"Kifizetési kérelem elküldve: {osszeg:N0} Ft");
            kifizetesOsszegTB.Clear();
            kifizetesIbanTB.Clear();
            MainWindow.adatok.Where(x => x.FelhasznaloNev == EgyenlegManager.Name.Nev).FirstOrDefault().Egyenleg = EgyenlegManager.Balance.Egyenleg;
            faljbairas();
        }
        private bool IbanEllenorzes(TextBox szoveg, RadioButton kartya, RadioButton utalas)
        {
            string szamlaszam = szoveg.Text.Trim().Replace(" ", "").Replace("-", "");

            if (string.IsNullOrWhiteSpace(szamlaszam))
            {
                MessageBox.Show("Kérjük add meg a bankszámlaszámodat vagy a kártyaszámodat!");
                return false;
            }

            if (kartya.IsChecked == true)
            {
                if (!szamlaszam.All(char.IsDigit) || szamlaszam.Length != 16)
                {
                    MessageBox.Show("Érvénytelen kártyaszám! A kártyaszámnak 16 számjegyből kell állnia.");
                    return false;
                }
            }

            if (utalas.IsChecked == true)
            {
                if (szamlaszam.Length >= 2 && char.IsLetter(szamlaszam[0]) && char.IsLetter(szamlaszam[1]))
                {
                    if (szamlaszam.Length < 15 || szamlaszam.Length > 34)
                    {
                        MessageBox.Show("Érvénytelen IBAN! Az IBAN 15-34 karakter hosszú lehet.\nPélda: HU42117730161111101800000000");
                        return false;
                    }
                    if (!szamlaszam.Skip(2).All(char.IsDigit))
                    {
                        MessageBox.Show("Érvénytelen IBAN! Az országkód után csak számok állhatnak.");
                        return false;
                    }
                }
                else
                {
                    if (!szamlaszam.All(char.IsDigit) || (szamlaszam.Length != 16 && szamlaszam.Length != 24))
                    {
                        MessageBox.Show("Érvénytelen bankszámlaszám!\n" +
                            "- Magyar formátum: 16 vagy 24 számjegy (pl. 12345678-12345678)\n" +
                            "- Nemzetközi IBAN (pl. HU42117730161111101800000000)");
                        return false;
                    }
                }
            }

            return true;
        }
        private void faljbairas()
        {
            StreamWriter sw = new StreamWriter("adatok.txt");
            foreach (var item in MainWindow.adatok)
            {
                sw.WriteLine($"{item.Nev};{item.Email};{item.FelhasznaloNev};{item.Telefonszam};{item.Jelszo};{item.SzuletesiDatum.ToString("yyyy-MM-dd")};{item.Egyenleg}");
            }
            sw.Close();
        }
        private void kijelentkezes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Content = null;
        }

        private void rbKifUtalas_Click(object sender, RoutedEventArgs e)
        {
            kifizetesIbanTB.Clear();
        }

        private void rbKifKartya_Click(object sender, RoutedEventArgs e)
        {
            kifizetesIbanTB.Clear();
        }
    }
}
