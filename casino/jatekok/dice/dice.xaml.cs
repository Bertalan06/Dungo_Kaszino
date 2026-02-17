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
    /// Interaction logic for dice.xaml
    /// </summary>
    public partial class dice : Page
    {
        public dice()
        {
            InitializeComponent();
            //BetSorHozzaad("pNOsFQ", "x1.96", "0.39");
 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        //előzményekhez hozzáadása
        private void BetSorHozzaad(string nev, string multiplier, string nyeremeny)
        {
            //törli a legrégebbit
            if (BetLista.Children.Count > 20)
                BetLista.Children.RemoveAt(0);

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
            osszegText.Inlines.Add(new Run("0.") { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888899")) });
            osszegText.Inlines.Add(new Run("20") { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a8e63d")) });
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
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a8e63d")),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(nyeremenyText, 2);

            sor.Children.Add(bal);
            sor.Children.Add(multText);
            sor.Children.Add(nyeremenyText);

            // Elválasztó vonal
            Rectangle vonal = new Rectangle
            {
                Height = 1,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a2a3e")),
                Margin = new Thickness(0, 2, 0, 2)
            };

            BetLista.Children.Add(sor);      // BetLista = a StackPanel x:Name-je
            BetLista.Children.Add(vonal);
        }

    }
}
