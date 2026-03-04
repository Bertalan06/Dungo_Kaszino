using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        public static MainWindow Instance;
        List<Adatok> adatok = new List<Adatok>();
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            szulDatum.SelectedDate = DateTime.Now.AddYears(-18);
            szulDatum.DisplayDateEnd = DateTime.Now.AddYears(-18);
            if (File.Exists("adatok.txt"))
            {
                foreach (string sor in File.ReadAllLines("adatok.txt"))
                {
                    if (!string.IsNullOrWhiteSpace(sor))
                        adatok.Add(new Adatok(sor));
                }
            }
        }
        private void signupScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }
        private void tovabb_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new home());
        }

        private void lnkBejelentkezes_Click(object sender, RoutedEventArgs e)
        {
            login.Visibility = Visibility.Visible;
            signup.Visibility = Visibility.Collapsed;
            pbJelszo.Clear();
            fnevemail.Clear();
        }
        private void lnkRegisztracio_Click(object sender, RoutedEventArgs e)
        {
            login.Visibility = Visibility.Collapsed;
            signup.Visibility = Visibility.Visible;
            tnev.Clear();
            email.Clear();
            fnev.Clear();
            tszam.Clear();
            psswrd.Clear();
            psswrdUjra.Clear();
        }

        private void regisztracio_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tnev.Text) ||
        string.IsNullOrWhiteSpace(email.Text) ||
        string.IsNullOrWhiteSpace(fnev.Text) ||
        string.IsNullOrWhiteSpace(tszam.Text) ||
        psswrd.Password.Length == 0 ||
        psswrdUjra.Password.Length == 0 ||
        szulDatum.SelectedDate == null)
            {
                MessageBox.Show("Minden mező kitöltése kötelező!");
                return;
            }

            if (!Regex.IsMatch(tnev.Text, @"^[A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]+([ -][A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]+)*$"))
            {
                MessageBox.Show("A név formátuma helytelen! (Pl.: Kovács János)");
                return;
            }

            if (!Regex.IsMatch(email.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Az email cím formátuma helytelen! (Pl.: pelda@gmail.com)");
                return;
            }

            if (!Regex.IsMatch(tszam.Text, @"^(\+?[1-9]\d{6,14}|06\d{8,9})$"))
            {
                MessageBox.Show("A telefonszám formátuma helytelen! (Pl.: +36201234567)");
                return;
            }

            if (!Regex.IsMatch(psswrd.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=]).{8,}$"))
            {
                MessageBox.Show("A jelszónak legalább 8 karakter, 1 nagy betű, 1 szám és 1 speciális karakter kell!");
                return;
            }

            if (psswrd.Password != psswrdUjra.Password)
            {
                MessageBox.Show("A két jelszó nem egyezik!");
                return;
            }

            if (adatok.Any(x => x.Email == email.Text))
            {
                MessageBox.Show("Ez az email cím már foglalt!");
                return;
            }

            if (adatok.Any(x => x.FelhasznaloNev == fnev.Text))
            {
                MessageBox.Show("Ez a felhasználónév már foglalt!");
                return;
            }

            if (adatok.Any(x => x.Telefonszam == tszam.Text))
            {
                MessageBox.Show("Ez a telefonszám már foglalt!");
                return;
            }

            adatok.Add(new Adatok($"{tnev.Text};{email.Text};{fnev.Text};{tszam.Text};{psswrd.Password};{szulDatum.SelectedDate.Value.ToShortDateString()}"));
            File.AppendAllLines("adatok.txt", new[] { $"{tnev.Text};{email.Text};{fnev.Text};{tszam.Text};{psswrd.Password};{szulDatum.SelectedDate.Value:yyyy-MM-dd}" });
            MessageBox.Show("Sikeres regisztráció!");

            tnev.Clear();
            email.Clear();
            fnev.Clear();
            tszam.Clear();
            psswrd.Clear();
            psswrdUjra.Clear();
            lnkBejelentkezes_Click(sender, e);
        }

        private void btnBejelentkezes_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(fnevemail.Text) || string.IsNullOrEmpty(pbJelszo.Password))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!");
                return;
            }

            bool found = adatok.Any(x => (x.Email == fnevemail.Text || x.FelhasznaloNev == fnevemail.Text) && x.Jelszo == pbJelszo.Password);

            if (found)
            {
                EgyenlegManager.Balance.Egyenleg = adatok.Where(x=>(x.Email == fnevemail.Text || x.FelhasznaloNev == fnevemail.Text)).Select(x => x.Egyenleg).FirstOrDefault();
                EgyenlegManager.Name.Nev = adatok.Where(x => (x.Email == fnevemail.Text || x.FelhasznaloNev == fnevemail.Text)).Select(x => x.FelhasznaloNev).FirstOrDefault();
                MainFrame.Navigate(new home());
            }
            else
            {
                MessageBox.Show("Hibás felhasználónév/email vagy jelszó!");
            }
        }

    }
}
