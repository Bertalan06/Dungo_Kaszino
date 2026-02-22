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
    /// Interaction logic for home.xaml
    /// </summary>

    public class EgyenlegManager { public static EgyenlegManager Balance = new EgyenlegManager { Egyenleg = 50000 }; private decimal egyenleg; public decimal Egyenleg { get => egyenleg; set { egyenleg = value; OnEgyenlegChanged?.Invoke(this, EventArgs.Empty); } } public event EventHandler OnEgyenlegChanged; }
    public partial class home : Page
    {
        public home()
        {
            InitializeComponent();
            EgyenlegManager.Balance.OnEgyenlegChanged += (s, e) => FrissEgyenleg();
            FrissEgyenleg();
            SizeChanged += Home_SizeChanged;
        }
        private void Home_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 900)
            {
                jatekGrid.Columns = 2;
                gombokPanel.Visibility = Visibility.Collapsed;
                gombokPanel2.Visibility = Visibility.Visible;
                egyenlegPanel2.Visibility = Visibility.Visible;
                egyenlegPanel.Visibility = Visibility.Hidden;
                masodikSor.Height = new GridLength(65);
            }
            else
            {
                jatekGrid.Columns = 3;
                gombokPanel.Visibility = Visibility.Visible;
                gombokPanel2.Visibility = Visibility.Collapsed;
                egyenlegPanel2.Visibility = Visibility.Hidden;
                egyenlegPanel.Visibility = Visibility.Visible;
                masodikSor.Height = new GridLength(0);
            }
        }
        private void blackjack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new blackjack());
        }

        private void chickenroad_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new chicken());
        }

        private void dice_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new dice());
        }

        private void hilo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new hilo());
        }

        private void lezeus_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new lezeus());
        }

        private void mines_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new mines());
        }

        private void feltoltes_Click(object sender, RoutedEventArgs e)
        {
            feltoltesablak feltolt = new feltoltesablak();
            feltolt.Owner = Window.GetWindow(this);

            if(feltolt.ShowDialog() == true)
            {
                EgyenlegManager.Balance.Egyenleg += feltolt.FeltoltottOsszeg;
                FrissEgyenleg();
            }
        }

        private void kifizetes_Click(object sender, RoutedEventArgs e)
        {
            kifizetes kifizet = new kifizetes();
            kifizet.Owner = Window.GetWindow(this);

            if (kifizet.ShowDialog() == true)
            {
                if (kifizet.KivettOsszeg > EgyenlegManager.Balance.Egyenleg)
                {
                    MessageBox.Show("Nincs elég egyenleg a kifizetéshez!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                EgyenlegManager.Balance.Egyenleg -= kifizet.KivettOsszeg;
                FrissEgyenleg();
            }
        }

        public void FrissEgyenleg()
        {
            egyenlegertek.Text = $"{EgyenlegManager.Balance.Egyenleg:N0} Ft";
            egyenlegertek2.Text = $"{EgyenlegManager.Balance.Egyenleg:N0} Ft";

            if (EgyenlegManager.Balance.Egyenleg <= 0)
            {
                egyenlegertek.Foreground = new SolidColorBrush(Colors.Red);
                egyenlegertek2.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (EgyenlegManager.Balance.Egyenleg <= 10000)
            {
                egyenlegertek.Foreground = new SolidColorBrush(Colors.Orange);
                egyenlegertek2.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                egyenlegertek.Foreground = new SolidColorBrush(Colors.Green);
                egyenlegertek2.Foreground = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
