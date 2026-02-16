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
                randomPathGenerator();
            }
            
            
        }

        string[] suits = { "C", "D", "H", "S" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        Random rand = new Random();
        List<string> voltak = new List<string>();
        private void randomPathGenerator()
        {
            
            string suit = suits[rand.Next(suits.Length)];
            string rank = ranks[rand.Next(ranks.Length)];
            string path = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "jatekok", "blackjack", "cards", $"{rank}{suit}.png");
            while (true) { 
                if (!voltak.Contains(path))
                {
                    DealCardFromDeck(path);
                    MessageBox.Show(System.IO.Path.GetFullPath(path));
                    voltak.Add(path);
                    break;
                }
                else
                {
                    suit = suits[rand.Next(suits.Length)];
                    rank = ranks[rand.Next(ranks.Length)];
                    path = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,"jatekok", "blackjack", "cards", $"{rank}{suit}.png");
                }
            }
            
        }

        int x = 325;
        private void ShowCardAtFinalPosition(string path)
        {
            Uri dungo = new Uri(path, UriKind.Absolute);
            ImageSource bitmap = new BitmapImage(dungo);
            Image finalCard = new Image
            {
                Source = bitmap,
                Height = 160,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            double finalTop = 237;
            Canvas.SetLeft(finalCard, x);
            Canvas.SetTop(finalCard, finalTop);
            GameCanvas.Children.Add(finalCard);
            x += 20;
        }
        private double _cardOffsetX = 0;

        private void DealCardFromDeck(string path)
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

            Canvas.SetLeft(animatedCard, 1120);
            Canvas.SetTop(animatedCard, 225);
            GameCanvas.Children.Add(animatedCard);

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
                GameCanvas.Children.Remove(animatedCard);
                ShowCardAtFinalPosition(path);
            };
        }


        private void hit_Click(object sender, RoutedEventArgs e)
        {
            randomPathGenerator();
        }
    }
}
