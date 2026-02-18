using casino;
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
    /// Interaction logic for blackjack.xaml
    /// </summary>
    public partial class blackjack : Page
    {
        private int _lastBet = 0;
        public blackjack()
        {
            InitializeComponent();
            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
        }

        string[] suits = { "C", "D", "H", "S" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        Random rand = new Random();
        List<string> voltakJatekos = new List<string>();
        List<string> voltakOszto = new List<string>();
        int x = 325;
        private double _cardOffsetX = -10;

        //gombok müködése
        private void vissza_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        private void stand_Click(object sender, RoutedEventArgs e) => StandLogic();
        private void hit_Click(object sender, RoutedEventArgs e)
        {
            randomPathGenerator("Játékos");
            int ertek = CalculateHandValue(voltakJatekos);
            if (ertek > 21)
            {
                eredmenyLabel.Content = "Bust! Vesztettél!";
                GameOver(false);
            }
            else if (ertek == 21)
                StandLogic();
        }
        private void deal_Click(object sender, RoutedEventArgs e)
        {
            start();
        }
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            bet.Content = "0 Ft";
            chip.Visibility = Visibility.Hidden;
        }

        //bet értékek
        private void min_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(100);
        }
        private void minusx_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val -= 500;
            SetBetValue(val);
        }
        private void minus_Click(object sender, RoutedEventArgs e)
        {
            int val = GetBetValue();
            val -= 100;
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
        private void max_Click(object sender, RoutedEventArgs e)
        {
            SetBetValue(500000);
        }

        //játék indítása
        private void start()
        {
            eredmenyLabel.Background = null;
            JatekosKartyakOsszge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80000000"));
            OsztoKartyakOsszge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80000000"));
            int tet = GetBetValue();
            if (tet == 0) return;
            _lastBet = tet; // ← új

            // take the bet from the player's balance immediately
            EgyenlegManager.Balance.Egyenleg -= _lastBet;

            KartyakTorlese();
            voltakJatekos.Clear();
            voltakOszto.Clear();
            eredmenyLabel.Content = "";
            rebet.Visibility = Visibility.Hidden;           
            x =325;
            _cardOffsetX = -10;
            OsztoKartyakOsszge.Content = "";
            OsztoKartyakOsszge.Visibility = Visibility.Hidden;
            JatekosKartyakOsszge.Content = "";

            // update displayed balance to the actual balance after taking the bet
            egyenlegTB.Text = "Egyenleg: " + home.Egyenleg.ToString("N0") + " Ft";

            int.TryParse(bet.Content.ToString().Replace(" Ft", ""), NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out tet);
            if (tet !=0)
            {
                chip.Visibility = Visibility.Visible;
                tetkezelo.Visibility = Visibility.Hidden;
                vissza.Visibility = Visibility.Hidden;
                kezelo.Visibility = Visibility.Visible;
                // removed the previous temporary subtraction display; actual balance already updated above
                randomPathGenerator("Játékos");
                randomPathGenerator("Osztó");
                randomPathGenerator("Játékos");
                randomPathGenerator("Osztó");
            }
        }

        //kártya számítás
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
            chip.Content = value.ToString("N0");
            if (value != 0)
            {
                chip.Visibility = Visibility.Visible;
            }
            else
            {
                chip.Visibility = Visibility.Hidden;
            }
        }
        private int GetCardValue(string rank)
        {
            if (int.TryParse(rank, out int value))
                return value; // 2-10 esetén a szám értéke
            if (rank == "J" || rank == "Q" || rank == "K")
                return 10;
            if (rank == "A")
                return 11; // Ásznak először 11-et adunk, majd ha kell visszaváltjuk 1-re
            return 0;
        }
        private int CalculateHandValue(List<string> cards)
        {
            int ossz = 0;
            int aszok = 0;

            foreach (string card in cards)
            {
                string rank = System.IO.Path.GetFileNameWithoutExtension(card).TrimEnd('C', 'D', 'H', 'S');
                int value = GetCardValue(rank);
                if (rank == "A") aszok++;
                ossz += value;
            }

            // Ha túlmegy 21-en és van ász, visszaváltjuk 1-re
            while (ossz > 21 && aszok > 0)
            {
                ossz -= 10;
                aszok--;
            }

            return ossz;
        }
        private void randomPathGenerator(string JatekosVOszto)
        {
            string suit = suits[rand.Next(suits.Length)];
            string rank = ranks[rand.Next(ranks.Length)];
            string path = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "blackjack", "cards", $"{rank}{suit}.png");
            while (true) {
                if (!voltakJatekos.Contains(path) || !voltakOszto.Contains(path))
                {

                    if (JatekosVOszto == "Játékos") {
                        voltakJatekos.Add(path);
                        JatekosKartyakOsszge.Content = CalculateHandValue(voltakJatekos);
                    }
                    else
                    {
                        voltakOszto.Add(path);
                        OsztoKartyakOsszge.Content = CalculateHandValue(voltakOszto);
                    }
                    DealCardFromDeck(path, JatekosVOszto);
                    //MessageBox.Show(System.IO.Path.GetFullPath(path));
                    break;
                }
                else
                {
                    suit = suits[rand.Next(suits.Length)];
                    rank = ranks[rand.Next(ranks.Length)];
                    path = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "blackjack", "cards", $"{rank}{suit}.png");
                }
            }

        }

        //kártya animációja
        private void ShowCardAtFinalPosition(string path, string JatekosVOszto)
        {
            Uri dungo = new Uri(path, UriKind.Absolute);
            ImageSource bitmap = new BitmapImage(dungo);
            Image finalCard = new Image
            {
                Source = bitmap,
                Height = 160,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Tag = "card"
            };
            double finalTop = 237;
            Canvas.SetLeft(finalCard, x);
            Canvas.SetTop(finalCard, finalTop);
            if (JatekosVOszto == "Játékos")
            {
                JatekosCanva.Children.Add(finalCard);
            }
            else
            {
                OsztoCanva.Children.Add(finalCard);
            }

            x += 20;
        }
        private void rebet_Click(object sender, RoutedEventArgs e)
        {
            OsztoKartyakOsszge.Content = "";
            JatekosKartyakOsszge.Content = "";
            bet.Content = rebet.Content;
            chip.Content = rebet.Content;
            start();
        }

        private void DealCardFromDeck(string path, string JatekosVOszto)
        {
            _cardOffsetX += 20;

            string absolutePath = System.IO.Path.GetFullPath(path);
            Uri dungo = new Uri(absolutePath, UriKind.Absolute);
            ImageSource bitmap = new BitmapImage(dungo);

            Image animatedCard = new Image
            {
                Source = bitmap,
                Height = 160,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            var transformGroup = new TransformGroup();
            var translateTransform = new TranslateTransform();
            transformGroup.Children.Add(translateTransform);
            animatedCard.RenderTransform = transformGroup;
            if (JatekosVOszto == "Játékos")
            {
                Canvas.SetLeft(animatedCard, 1120);
                Canvas.SetTop(animatedCard, 0);
                JatekosCanva.Children.Add(animatedCard);
            }
            else {
                Canvas.SetLeft(animatedCard, 1100);
                Canvas.SetTop(animatedCard, 0);
                if (voltakOszto.Count == 2)
                {
                    animatedCard.Tag = bitmap;
                    animatedCard.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "blackjack", "icon", $"back.png"), UriKind.Absolute));
                }
                OsztoCanva.Children.Add(animatedCard);
            }



            var xAnimation = new DoubleAnimation
            {
                From = 0,
                To = -549 + _cardOffsetX,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var yAnimation = new DoubleAnimation
            {
                From = 0,
                To = 30,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            translateTransform.BeginAnimation(TranslateTransform.XProperty, xAnimation);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, yAnimation);

            xAnimation.Completed += (s, e) =>
            {
                if (JatekosVOszto == "Játékos")
                {
                    JatekosCanva.Children.Remove(animatedCard);
                }
                else
                {
                    OsztoCanva.Children.Remove(animatedCard);
                }


                ShowCardAtFinalPosition(path, JatekosVOszto);
            };
        }

        private void GameOver(bool? nyert)
        {
            var forditott = OsztoCanva.Children.OfType<Image>().Where(x => x.Source.ToString().Contains("back.png")).FirstOrDefault();
            forditott.Source = forditott.Tag as ImageSource;
            eredmenyLabel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80000000"));
            OsztoKartyakOsszge.Visibility = Visibility.Visible;
            if (nyert == true)
                EgyenlegManager.Balance.Egyenleg += _lastBet * 2;
            else if (nyert == null)
                EgyenlegManager.Balance.Egyenleg += _lastBet;

            egyenlegTB.Text = "Egyenleg: " + EgyenlegManager.Balance.Egyenleg.ToString("N0") + " Ft";
            JatekosKartyakOsszge.Content = CalculateHandValue(voltakJatekos);
            OsztoKartyakOsszge.Content = CalculateHandValue(voltakOszto);
            rebet.Content = chip.Content;
            rebet.Visibility = Visibility.Visible;
            tetkezelo.Visibility = Visibility.Visible;
            vissza.Visibility = Visibility.Visible;
            kezelo.Visibility = Visibility.Hidden;
            chip.Visibility = Visibility.Hidden;
            chip.Content = "0";
            bet.Content = "0 Ft";
            _cardOffsetX = -10;
            x = 325;

        }
        private void KartyakTorlese()
        {
            var jatekosKartyak = JatekosCanva.Children.OfType<Image>().Where(x => x.Name != "DeckImage").ToList();
            foreach (var kartya in jatekosKartyak)
                JatekosCanva.Children.Remove(kartya);

            var osztoKartyak = OsztoCanva.Children.OfType<Image>().Where(x => x.Name != "DeckImage").ToList();
            foreach (var kartya in osztoKartyak)
                OsztoCanva.Children.Remove(kartya);
        }
        private void StandLogic()
        {
            while (CalculateHandValue(voltakOszto) < 17)
                randomPathGenerator("Osztó");

            int jatekosErtek = CalculateHandValue(voltakJatekos);
            int osztoErtek = CalculateHandValue(voltakOszto);

            if (osztoErtek > 21)
            {
                eredmenyLabel.Content = "Osztó bust! Nyertél! 🎉";
                GameOver(true);
            }
            else if (jatekosErtek > osztoErtek)
            {
                eredmenyLabel.Content = "Nyertél! 🎉";
                GameOver(true);
            }
            else if (jatekosErtek == osztoErtek)
            {
                eredmenyLabel.Content = "Döntetlen! Visszakapod a téted.";
                GameOver(null);
            }
            else
            {
                eredmenyLabel.Content = "Osztó nyert!";
                GameOver(false);
            }
        }
    }
}
