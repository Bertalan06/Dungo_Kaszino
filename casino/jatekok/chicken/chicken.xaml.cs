using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace casino
{
    /// <summary>
    /// Interaction logic for chicken.xaml
    /// </summary>
    public partial class chicken : Page
    {
        public chicken()
        {
            InitializeComponent();
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
        }
        string[] easy = { "1.03", "1.07", "1.12", "1.17", "1.23", "1.29", "1.36", "1.44", "1.53", "1.63", "1.75", "1.88", "2.04", "2.22", "2.45", "2.72", "3.06", "3.5", "4.08", "4.9", "6.13", "6.61", "9.81", "19.44" };
        string[] hard = { "1.23", "1.55", "1.98", "2.56", "3.36", "4.49", "5.49", "7.53", "10.56", "15.21", "22.59", "34.79", "55.97", "94.99", "172.42", "341.4", "760.46", "2007.63", "6956.47", "41321.43" };

        private void vissza_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        private void Animate(TranslateTransform transform, double eltolas)
        {
            var animation = new DoubleAnimation
            {
                To = eltolas,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
        int kor, nehez, vege;
        double tet;
        private double hattereltolas, csirkeeltolas, csirkelepeskoz, hatterlepeskoz;

        private void Jatek_Click(object sender, RoutedEventArgs e)
        {
            if (!(bet.Content.ToString() == "0 Ft"))
            {
                tet = GetBetValue();
                EgyenlegManager.Balance.Egyenleg -= GetBetValue();
                egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
                switch (nehez)
                {
                    case 0:
                        vege = 24;
                        break;
                    case 1:
                        vege = 20;
                        break;
                }
                vissza.Visibility = Visibility.Hidden;
                tetkezelo.Visibility = Visibility.Hidden;
                Easy.IsEnabled = false;
                Hard.IsEnabled = false;
                nullazas();
                halalSorsor(vege);
                csirkeeltolas += csirkelepeskoz;
                Animate(csirkeTransform, csirkeeltolas);
                jatek.Visibility = Visibility.Collapsed;
                kezelo.Visibility = Visibility.Visible;
                kor = 1;
                alive(kor);
            }
        }
        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            nehezseg.Content = "Nehezség: Könnyű";
            hatter.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "chicken", "easy.png")));
            nehez = 0;
            nullazas();
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            nehezseg.Content = "Nehezség: Nehéz";
            hatter.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "chicken", "hard.png")));
            nehez = 1;
            nullazas();
        }
        private void cahsout_Click(object sender, RoutedEventArgs e)
        {
            EgyenlegManager.Balance.Egyenleg += (int)(tet * double.Parse(nehez == 0 ? easy[kor - 1] : hard[kor - 1], CultureInfo.InvariantCulture));
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            MessageBox.Show("Kifizetve: " + ((int)(tet * double.Parse(nehez == 0 ? easy[kor - 1] : hard[kor - 1], CultureInfo.InvariantCulture))).ToString("N0") + " Ft");
            gameOver();
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
            if (value < 0)
                value = 0;

            int egyenleg = (int)EgyenlegManager.Balance.Egyenleg;
            if (value > egyenleg)
                value = egyenleg;

            bet.Content = value.ToString("N0") + " Ft";
        }
        private void nullazas()
        {
            kor = 0;
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            hattereltolas = 0;
            csirkeeltolas = 0;
            csirkelepeskoz = 200;
            hatterlepeskoz = 90;
        }
        private void go_Click(object sender, RoutedEventArgs e)
        {
            if (kor < vege)
            {
                if (kor >= 3)
                {
                    if (kor == 3)
                    {
                        hattereltolas -= 390;
                        Animate(hatterTransform, hattereltolas);
                        csirkeeltolas = 210;
                        Animate(csirkeTransform, csirkeeltolas);
                        csirkelepeskoz = 110;
                    }
                    if (kor == 10)
                    {
                        hattereltolas -= 760;
                        Animate(hatterTransform, hattereltolas);
                        csirkeeltolas = 210;
                        Animate(csirkeTransform, csirkeeltolas);
                    }
                    if (kor == 17)
                    {
                        hattereltolas -= 980;
                        Animate(hatterTransform, hattereltolas);
                        csirkeeltolas = 210;
                        Animate(csirkeTransform, csirkeeltolas);
                    }
                    else
                    {
                        csirkeeltolas += csirkelepeskoz;
                        Animate(csirkeTransform, csirkeeltolas);
                        hattereltolas -= hatterlepeskoz;
                        Animate(hatterTransform, hattereltolas);
                    }
                }
                else
                {
                    csirkeeltolas += csirkelepeskoz;
                    Animate(csirkeTransform, csirkeeltolas);
                }
                kor++;
                alive(kor);
            }
        }

        private static readonly Random _random = new Random();
        private int halalmezo;

        private int halalSorsor(int max)
        {
            halalmezo = _random.Next(1, max + 1);
            return halalmezo;
        }
        private void gameOver()
        {
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            vissza.Visibility = Visibility.Visible;
            tetkezelo.Visibility = Visibility.Visible;
            Easy.IsEnabled = true;
            Hard.IsEnabled = true;
            kezelo.Visibility = Visibility.Collapsed;
            jatek.Visibility = Visibility.Visible;
            bet.Content = "0 Ft";
        }
        private void alive(int aktualis)
        {
            betFrissit();
            if (aktualis == halalmezo)
            {
                MessageBox.Show("Meghaltál! A csirke a " + halalmezo + ". mezőn meghalt.");
                gameOver();
                nullazas();
            }
        }
        private void betFrissit()
        {
            if (nehez == 0)
            {
                bet.Content = ((int)(tet * double.Parse(easy[kor - 1], CultureInfo.InvariantCulture))).ToString("N0") + " Ft";
            }
            else
            {
                bet.Content = ((int)(tet * double.Parse(hard[kor - 1], CultureInfo.InvariantCulture))).ToString("N0") + " Ft";
            }
        }
    }
}



