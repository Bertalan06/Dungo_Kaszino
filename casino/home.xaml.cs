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
    

    public class EgyenlegManager
    {
        private decimal egyenleg;
        public decimal Egyenleg
        {
            get => egyenleg;
            set
            {
                egyenleg = value;
                OnEgyenlegChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler OnEgyenlegChanged;
    }
    public partial class home : Page
    {

        public static decimal Egyenleg = 50000;

        public home()
        {
            InitializeComponent();
            FrissEgyenleg();
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
                Egyenleg += feltolt.FeltoltottOsszeg;
                FrissEgyenleg();
            }
        }

        private void kifizetes_Click(object sender, RoutedEventArgs e)
        {
            kifizetes kifizet = new kifizetes();
            kifizet.Owner = Window.GetWindow(this);

            if (kifizet.ShowDialog() == true)
            {
                if (kifizet.KivettOsszeg > Egyenleg)
                {
                    MessageBox.Show("Nincs elég egyenleg a kifizetéshez!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Egyenleg -= kifizet.KivettOsszeg;
                FrissEgyenleg();
            }
        }

        private void FrissEgyenleg()
        {
            egyenlegertek.Text = $"{Egyenleg:N0} Ft";

            if(Egyenleg <= 0)
            {
                egyenlegertek.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if(Egyenleg <= 10000)
            {
                egyenlegertek.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                egyenlegertek.Foreground = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
