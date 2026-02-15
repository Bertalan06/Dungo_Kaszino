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
using System.Windows.Shapes;

namespace casino
{
    /// <summary>
    /// Interaction logic for kifizetes.xaml
    /// </summary>
    public partial class kifizetes : Window
    {
        public decimal KivettOsszeg { get; private set; } = 0;

        public kifizetes()
        {
            InitializeComponent();
        }

        private void Kifizetes_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(osszTB.Text, out decimal osszeg) && osszeg > 0)
            {
                KivettOsszeg = osszeg;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Kérlek, adj meg egy pozitív számot!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Megse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
