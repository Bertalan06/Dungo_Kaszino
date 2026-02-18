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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;

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
        }

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
        int kor, nehez;
        private double hattereltolas,csirkeeltolas,csirkelepeskoz,hatterlepeskoz;
        private void Jatek_Click(object sender, RoutedEventArgs e)
        {
            kor = 0;
            kor++;
            nehez = 0;
            hattereltolas = 0;
            csirkeeltolas = 0;
            csirkelepeskoz = 230;
            hatterlepeskoz = 110;
            csirkeeltolas += csirkelepeskoz;
            Animate(csirkeTransform, csirkeeltolas);
            jatek.Visibility = Visibility.Collapsed;
            kezelo.Visibility = Visibility.Visible;
        }
        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            nehezseg.Content = "Nehezség: Könnyű";
            hatter.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,"jatekok", "chicken", "easy.png")));
            nehez = 0;
        }

        private void Normal_Click(object sender, RoutedEventArgs e)
        {
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            nehezseg.Content = "Nehezség: Normál";
            hatter.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,"jatekok", "chicken", "normal.png")));
            nehez = 1;
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            Animate(csirkeTransform, 0);
            Animate(hatterTransform, 0);
            nehezseg.Content = "Nehezség: Nehéz";
            hatter.Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,"jatekok", "chicken", "hard.png")));
            nehez = 2;
        }
        private void cahsout_Click(object sender, RoutedEventArgs e)
        {

        }
        int vege = 0;
        private void go_Click(object sender, RoutedEventArgs e)
        {
            switch (nehez)
            {
                case 0:
                    vege = 24;
                    break;
                case 1:
                    vege = 15;
                    break;
                case 2:
                    vege = 14;
                    csirkelepeskoz+= 50;
                    hatterlepeskoz += 50;
                    break;
            }
            if (kor != vege)
            {
                if (kor >= 3)
                {
                    if (kor == 3)
                    {
                        hattereltolas -= 480;
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
            }
        }
    }
}



