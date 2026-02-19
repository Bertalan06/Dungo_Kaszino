using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace casino
{
    /// <summary>
    /// Interaction logic for dice.xaml
    /// </summary>
    public partial class dice : Page
    {
        
        public dice()
        {
            InitializeComponent();
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";

            //BetSorHozzaad("pNOsFQ", "x1.96", "0.39");
            Slider.Resources.Add("BalSzin", new SolidColorBrush(Color.FromRgb(0xe0, 0x55, 0x55)));

            Slider.Resources.Add("JobbSzin", new SolidColorBrush(Color.FromRgb(0x4c, 0xaf, 0x50)));


        }

        private void vissza_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        //előzményekhez hozzáadása
        private void BetSorHozzaad(string nev, string multiplier, string nyeremeny)
        {

            while (BetLista.Children.Count > 40) // 20 pár = 40 elem
            {
                BetLista.Children.RemoveAt(BetLista.Children.Count - 1); // vonal
                BetLista.Children.RemoveAt(BetLista.Children.Count - 1); // sor
            }

            Grid sor = new Grid();
            sor.Margin = new Thickness(0, 6, 0, 6);
            sor.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            sor.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            sor.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Avatar + név + összeg
            StackPanel bal = new StackPanel { Orientation = Orientation.Horizontal };
            Border avatar = new Border
            {
                Width = 28,
                Height = 28,
                CornerRadius = new CornerRadius(14),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a4e")),
                Margin = new Thickness(0, 0, 8, 0)
            };
            avatar.Child = new TextBlock
            {
                Text = nev[0].ToString().ToUpper(),
                Foreground = Brushes.White,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            StackPanel nevOsszeg = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            nevOsszeg.Children.Add(new TextBlock { Text = nev, Foreground = Brushes.White, FontSize = 13 });

            TextBlock osszegText = new TextBlock { FontSize = 13 };
            osszegText.Inlines.Add(new Run(nyeremeny + " Ft")
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a8e63d"))
            });
            nevOsszeg.Children.Add(osszegText);

            bal.Children.Add(avatar);
            bal.Children.Add(nevOsszeg);
            Grid.SetColumn(bal, 0);

            // Multiplier
            TextBlock multText = new TextBlock
            {
                Text = multiplier,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888899")),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(multText, 1);

            // Nyeremény
            TextBlock nyeremenyText = new TextBlock
            {
                Text = nyeremeny,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(nyeremeny.StartsWith("+") ? "#a8e63d" : "#e05555")),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };
            Grid.SetColumn(nyeremenyText, 2);

            sor.Children.Add(bal);
            sor.Children.Add(multText);
            sor.Children.Add(nyeremenyText);
          
            
            Rectangle vonal = new Rectangle
            {
                Height = 1,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a2a3e")),
                Margin = new Thickness(0, 2, 0, 2)
            };

            BetLista.Children.Insert(0, vonal);
            BetLista.Children.Insert(0, sor);
        }
        //Slider
        private void Szam_Slider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Nagyszam == null || Kisszam == null || Szazalek == null) return;
            if (!int.TryParse(FeltettOsszeg.Text, out int tet)) return;

            Nagyszam.Text = Slider.Value.ToString("F0");
            Kisszam.Text = Slider.Value.ToString("F0");

            Szazalek.Text = (Irany ? (100 - Slider.Value) : (Slider.Value - 1)).ToString("F0") + "%";
            Szorzo.Text = "x" + GetSzorzo((int)Slider.Value, Irany).ToString("F2");
            LehetFizet.Text = (tet * GetSzorzo((int)Slider.Value, Irany)).ToString("N0") + " Ft";


        }
        //Játékmenet
        Random rnd = new Random();
        private void Dobas(object sender, RoutedEventArgs e)
        {
            
            int tet = int.Parse(FeltettOsszeg.Text);
            if (tet > EgyenlegManager.Balance.Egyenleg)
            {
                MessageBox.Show("Nincs elég egyenleged!");
                return;
            }
            int szam = (int)Slider.Value;
            double szorzo = GetSzorzo(szam, Irany);

           
            int dobas = rnd.Next(1, 100);

            bool nyert = Irany ? dobas > szam : dobas < szam;

            
            Dobottszam.Text = dobas.ToString();

            
            Color nyertSzin = (Color)ColorConverter.ConvertFromString("#c8f040");
            Color veszitettSzin = (Color)ColorConverter.ConvertFromString("#e05555");
            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new System.Windows.Point(0, 0);
            gradient.EndPoint = new System.Windows.Point(0, 1);
            gradient.GradientStops.Add(new GradientStop(nyert ? nyertSzin : veszitettSzin, 0));
            gradient.GradientStops.Add(new GradientStop(nyert ? nyertSzin : veszitettSzin, 1));
            Dobottszam.Foreground = gradient;

            
            double nyeremeny = nyert ? tet * szorzo : -tet;

            
            string nyeremenyStr = nyert  ? $"+{nyeremeny:F0} Ft" : $"-{tet:F0} Ft";
            
            EgyenlegManager.Balance.Egyenleg += (int)nyeremeny; 
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";

            BetSorHozzaad( "Játékos",  $"x{szorzo:F2}", nyeremenyStr);
            
        }
                                //szam =  slider értéke, irany = felette vagy alatta
        private double GetSzorzo(int szam, bool irany)
        {
            double esely = irany ? (99.0 - szam) / 96.0: (szam - 2.0) / 96.0;
            if (esely <= 0) return 100;
            return 1.0 / esely;
        }

        //Gombok
        bool Irany = true;
        private void Felette_Click(object sender, RoutedEventArgs e)
        {
            if (!Irany)
            {
                Felette.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3d3d50"));
                Felette.Foreground = Brushes.White;
                Felette.FontWeight = FontWeights.SemiBold;
                Alatta.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a4e"));
                Alatta.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aaaaaa"));
                Alatta.FontWeight = FontWeights.Normal;
                Irany = true;

                Slider.Resources["BalSzin"] = new SolidColorBrush(Color.FromRgb(0xe0, 0x55, 0x55));

                Slider.Resources["JobbSzin"] = new SolidColorBrush(Color.FromRgb(0x4c, 0xaf, 0x50));



            }
        }

        private void Alatta_Click(object sender, RoutedEventArgs e)
        {
            if (Irany)
            {
                Alatta.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3d3d50"));
                Alatta.Foreground = Brushes.White;
                Alatta.FontWeight = FontWeights.SemiBold;
                Felette.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a4e"));
                Felette.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aaaaaa"));
                Felette.FontWeight = FontWeights.Normal;
                Irany = false;

                Slider.Resources["BalSzin"] = new SolidColorBrush(Color.FromRgb(0x4c, 0xaf, 0x50));

                Slider.Resources["JobbSzin"] = new SolidColorBrush(Color.FromRgb(0xe0, 0x55, 0x55));


            }
        }

        private void Szorzas_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(FeltettOsszeg.Text) * 2) <= EgyenlegManager.Balance.Egyenleg) { 
                FeltettOsszeg.Text = Convert.ToString(int.Parse(FeltettOsszeg.Text) * 2);
            }
        }

        private void Osztas_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(FeltettOsszeg.Text) / 2) >= 100) {
                FeltettOsszeg.Text = Convert.ToString(int.Parse(FeltettOsszeg.Text) / 2);
            }
        }

        private void Minusz_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(FeltettOsszeg.Text)-1) >= 100)
            {
                FeltettOsszeg.Text = Convert.ToString(int.Parse(FeltettOsszeg.Text) - 1);
            }
        }

        private void Plusz_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(FeltettOsszeg.Text) + 1) <= EgyenlegManager.Balance.Egyenleg)
            {
                FeltettOsszeg.Text = Convert.ToString(int.Parse(FeltettOsszeg.Text) + 1);
            }
        }
        private bool _frissites = false;
        private void FeltettOsszeg_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_frissites) return;
            if (!int.TryParse(FeltettOsszeg.Text, out int ertek)) return;

            _frissites = true;

            if (string.IsNullOrEmpty(FeltettOsszeg.Text))
            {
                FeltettOsszeg.Text = "100";
            }
            else if (ertek >= EgyenlegManager.Balance.Egyenleg)
            {
                FeltettOsszeg.Text = EgyenlegManager.Balance.Egyenleg.ToString();
            }
            else if (ertek <= 0)
            {
                FeltettOsszeg.Text = FeltettOsszeg.Text.TrimStart('-');
            }
            
            if (LehetFizet != null && int.TryParse(FeltettOsszeg.Text, out int tet)) { 
                LehetFizet.Text = (tet * GetSzorzo((int)Slider.Value, Irany)).ToString("N0") + " Ft";
            }
            _frissites = false;
        }
        bool veg = false;
        private async void AutoBet_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoBetDB.Text == null || AutoBetDB.Text == "0" || !int.TryParse(AutoBetDB.Text, out int Darab))
            {
                AutoBet.IsChecked = false;
                return;
            }
            Letilt();
            if (!veg)
            {
                int tet = 0;
                
                while(AutoBet.IsChecked == true)
                {
                    for (int i = 0; i < Darab; i++)
                    {
                        tet = int.Parse(FeltettOsszeg.Text);
                        if (tet > EgyenlegManager.Balance.Egyenleg)
                        {
                            MessageBox.Show("Nincs elég egyenleged!");
                            Engedelyezes();
                            return;
                        } else if(!int.TryParse(AutoBetDB.Text, out int x) || x <= 0)
                        {
                            AutoBet.IsChecked = false;
                            Engedelyezes();
                            return;
                        }

                        Dobas(null, null);
                        AutoBetDB.Text = (int.Parse(AutoBetDB.Text) - 1).ToString();
                        await Task.Delay(1000);
                    }
                }
                AutoBet.IsChecked = false;
                Engedelyezes();

            }
            
            
        }

        private void Pluszot_Click(object sender, RoutedEventArgs e)
        {
            AutoBetDB.Text = "5";
        }

        private void Plusztiz_Click(object sender, RoutedEventArgs e)
        {
            AutoBetDB.Text = "10";
        }
        private bool _vegtelen = false;
        private async void Vegtelen_Click(object sender, RoutedEventArgs e)
        {
            

            // Visszaengedés
            
            if (_vegtelen)
            {
                _vegtelen = false;
                veg = false;
                AutoBetDB.Text = "5";
                AutoBet.IsChecked = false;
                return;
            }

            Letilt();

            _vegtelen = true;
            veg = true;
            int tet = 0;
            AutoBet.IsChecked = true;
            AutoBetDB.Text = "∞";
            while (AutoBet.IsChecked == true && _vegtelen)
            {
                tet = int.Parse(FeltettOsszeg.Text);
                if (tet > EgyenlegManager.Balance.Egyenleg)
                {
                    MessageBox.Show("Nincs elég egyenleged!");
                    _vegtelen = false;
                    veg = false;
                    AutoBetDB.Text = "5";
                    AutoBet.IsChecked = false;
                    Engedelyezes();
                    return;
                }
                Dobas(null, null);
                await Task.Delay(1000);
            }
            veg = false;
            AutoBetDB.Text = "5";
            Engedelyezes();
        }
        private void Engedelyezes()
        {
            Slider.IsEnabled = true;
            FeltettOsszeg.IsEnabled = true;
            Felette.IsEnabled = true;
            Alatta.IsEnabled = true;
            Osztas.IsEnabled = true;
            Szorzas.IsEnabled = true;
            Pluszot.IsEnabled = true;
            Plusztiz.IsEnabled = true;
            Dob.IsEnabled = true;
            Plusz.IsEnabled = true;
            Minusz.IsEnabled = true;
            vissza.IsEnabled = true;
        }
        private void Letilt()
        {
            Slider.IsEnabled = false;
            FeltettOsszeg.IsEnabled = false;
            Felette.IsEnabled = false;
            Alatta.IsEnabled = false;
            Osztas.IsEnabled = false;
            Szorzas.IsEnabled = false;
            Pluszot.IsEnabled = false;
            Plusztiz.IsEnabled = false;
            Dob.IsEnabled = false;
            Minusz.IsEnabled = false;
            Plusz.IsEnabled = false;
            vissza.IsEnabled = false;
        }
    }
}
