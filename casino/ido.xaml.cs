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
using System.Windows.Threading;

namespace casino
{
    /// <summary>
    /// Interaction logic for ido.xaml
    /// </summary>
    public partial class ido : UserControl
    {
        private DispatcherTimer timer;
        public ido()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            idoTB.Text = DateTime.Now.ToString("MMMM dd HH:mm");
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            idoTB.Text = DateTime.Now.ToString("MMMM dd HH:mm");
        }
    }
}
