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
    /// Interaction logic for feltoltesablak.xaml
    /// </summary>
    public partial class feltoltesablak : Window
    {
        public decimal FeltoltottOsszeg { get; private set; } = 0;

        public feltoltesablak()
        {
            InitializeComponent();
        }

        private void Feltoltes_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(osszegTB.Text, out decimal osszeg) && osszeg > 0)
            {
                FeltoltottOsszeg = osszeg;
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
