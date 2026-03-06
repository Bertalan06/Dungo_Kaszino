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
    public partial class slot : Page
    {
        private static readonly string[] Szimb = {
            "7", "⭐", "🔔", "🍉", "🍇", "🍑", "🍋", "🍊", "🍒"
        };

        // Kifizetési szorzók (kredit, amit 3x nyerésnél kap a tét alapján)
        private static readonly int[] Kifizetés = {
            1000, 500, 200, 150, 150, 80, 80, 80, 80
        };

        // Súlyozás: esélyek a Szimb tombhoz kepest (alacsonyabb = értékesebb)
        private static readonly int[] Sullyok = {
            1, 2, 4, 4, 4, 6, 6, 6, 6
        };

        //  [vonalIdx][reelIdx] 
        private static readonly int[,] Vonalak = {
            { 1, 1, 1 },   // 1. középső
            { 0, 0, 0 },   // 2. felső
            { 2, 2, 2 },   // 3. alsó
            { 0, 1, 2 },   // 4. átló ↘
            { 2, 1, 0 },   // 5. átló ↗
        };

        //Random és a egyenleg
        private readonly Random _rng = new Random();
        private decimal _egyenleg = EgyenlegManager.Balance.Egyenleg;

        //tétválasztó 
        private static readonly decimal[] ErmeErtekek = { 100m, 500m, 1000m, 2000m, 5000m, 10000m, 20000m };
        private static readonly int[] TetErtekek = { 50, 100, 200, 500, 1000 };
        private int _ermeIdx = 0;   
        private int _tetIdx = 0;   

        private bool _porgo = false;
        private bool _turbo = false;
        private bool _auto = false;

        // Statisztika
        private int _porgesek = 0;
        private int _nyeresek = 0;
        private decimal _legjobbNyeremeny = 0;
        private decimal _osszes = 0;

        // Rács: [oszlop][sor] 
        private readonly int[,] _racs = new int[3, 3];

        // Slot kijelzője
        private TextBlock[,] _szovegek = null;
        private Border[,] _cellak = null;
        private Border[] _vonalJelzok = null;

        public slot()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object s, RoutedEventArgs e)
        {
            // Rács textblockjai
            _szovegek = new TextBlock[3, 3]
            {
                { S00, S01, S02 },
                { S10, S11, S12 },
                { S20, S21, S22 }
            };
            _cellak = new Border[3, 3]
            {
                { C00, C01, C02 },
                { C10, C11, C12 },
                { C20, C21, C22 }
            };
            _vonalJelzok = new Border[] { V1, V2, V3, V4, V5 };

            FrissítKijelző();
        }

        // Pörgetés
        private async void Porget_Click(object s, RoutedEventArgs e) => await Pörget();

        private async Task Pörget()
        {
            if (_porgo) return;
            decimal valósTét = ValósTét();
            if (_egyenleg < valósTét)
            {
                tbUzenet.Text = "NINCS ELÉG EGYENLEG!";
                tbgAuto.IsChecked = false;
                _auto = false;
                return;
            }

            _porgo = true;
            _egyenleg -= valósTét;
            _porgesek++;
            TöröldKiemeléseket();
            tbUzenet.Text = "PÖRGETÉS...";
            btnPorget.IsEnabled = false;

            int animMs = _turbo ? 280 : 750;

            // Animáció
            await AnimálRacsot(animMs);

            // Tényleges eredmény
            for (int r = 0; r < 3; r++)
            {
                for (int sor = 0; sor < 3; sor++)
                { 
                    _racs[r, sor] = VéletlenSzimb(); 
                }
            }
            RacsKirajzol();

            await Task.Delay(_turbo ? 80 : 200);

            // Kiértékelés
            decimal nyeremény = 0;
            var nyeroVonalak = new List<int>();
            for (int v = 0; v < 5; v++)
            {
                int sz0 = _racs[0, Vonalak[v, 0]];
                int sz1 = _racs[1, Vonalak[v, 1]];
                int sz2 = _racs[2, Vonalak[v, 2]];
                if (sz0 == sz1 && sz1 == sz2)
                {
                    nyeroVonalak.Add(v);
                    nyeremény += Kifizetés[sz0] * valósTét / 10m;
                }
            }

            if (nyeremény > 0)
            {
                _egyenleg += nyeremény;
                _nyeresek++;
                _osszes += nyeremény;
                if (nyeremény > _legjobbNyeremeny) _legjobbNyeremeny = nyeremény;

                tbUzenet.Text = $"🏆 NYEREMÉNY: {nyeremény:F2} Ft!";
                tbUtolso.Text = $"{nyeremény:F2} Ft";
                KiemelVonalak(nyeroVonalak);
                await VillogtatAsync(nyeroVonalak);
            }
            else
            {
                tbUzenet.Text = "TEGYE MEG A TÉTJÉT";
                tbUtolso.Text = "0 Ft";
            }

            FrissítKijelző();
            _porgo = false;
            btnPorget.IsEnabled = true;

            if (_auto && _egyenleg >= ValósTét())
            {
                await Task.Delay(_turbo ? 150 : 500);
                await Pörget();
            }
            else if (_auto)
            {
                tbgAuto.IsChecked = false;
                _auto = false;
                tbUzenet.Text = "NINCS ELÉG EGYENLEG";
            }
        }

        private async Task AnimálRacsot(int ms)
        {
            int lépések = ms / 45;
            for (int i = 0; i < lépések; i++)
            {
                for (int r = 0; r < 3; r++)
                {
                    for (int sor = 0; sor < 3; sor++)
                        ÁllítSzimb(r, sor, VéletlenSzimb());
                }
                    
                await Task.Delay(45);
            }
        }

        private void RacsKirajzol()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int sor = 0; sor < 3; sor++) {
                    ÁllítSzimb(r, sor, _racs[r, sor]);
                }
                    
            }
                
        }

        private void ÁllítSzimb(int r, int sor, int idx)
        {
            var tb = _szovegek[r, sor];
            tb.Text = Szimb[idx];
            // 7-es piroson marad, többi fehér
            tb.Foreground = idx == 0 ? new SolidColorBrush(Color.FromRgb(0xFF, 0x33, 0x33)) : new SolidColorBrush(Colors.White);
            tb.FontWeight = idx == 0 ? FontWeights.Bold : FontWeights.Normal;
        }

        private void KiemelVonalak(List<int> vonalak)
        {
            // Szimbólum cellák
            foreach (int v in vonalak)
            {
                _vonalJelzok[v].Opacity = 1.0;
                for (int r = 0; r < 3; r++)
                {
                    int sor = Vonalak[v, r];
                    var cella = _cellak[r, sor];
                    cella.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xD7, 0x00));
                    cella.BorderThickness = new Thickness(2);
                    cella.Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Color.FromRgb(0xFF, 0xD7, 0x00),
                        BlurRadius = 14,
                        ShadowDepth = 0,
                        Opacity = 0.85
                    };
                }
            }
        }

        private async Task VillogtatAsync(List<int> vonalak)
        {
            for (int i = 0; i < 4; i++)
            {
                // Ki
                foreach (int v in vonalak)
                {
                    for (int r = 0; r < 3; r++)
                    { 
                        _cellak[r, Vonalak[v, r]].Opacity = 0.3;
                    }
                }
                await Task.Delay(160);
                // Be
                foreach (int v in vonalak)
                {
                    for (int r = 0; r < 3; r++)
                        {
                        _cellak[r, Vonalak[v, r]].Opacity = 1.0; 
                    }
                }
                await Task.Delay(140);
            }
        }

        private void TöröldKiemeléseket()
        {
            for (int r = 0; r < 3; r++)
                for (int sor = 0; sor < 3; sor++)
                {
                    _cellak[r, sor].BorderBrush = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
                    _cellak[r, sor].BorderThickness = new Thickness(1);
                    _cellak[r, sor].Effect = null;
                    _cellak[r, sor].Opacity = 1.0;
                }
            foreach (var v in _vonalJelzok)
                v.Opacity = 0.3;
        }

        
        private decimal ValósTét() => TetErtekek[_tetIdx] * ErmeErtekek[_ermeIdx] / 100m;

        private int VéletlenSzimb()
        {
            
            var pool = new List<int>();
            for (int i = 0; i < Szimb.Length; i++)
            {
                for (int j = 0; j < Sullyok[i]; j++)
                {
                    pool.Add(i);
                }
                    
            }
                
            return pool[_rng.Next(pool.Count)];
        }

        private void FrissítKijelző()
        {
            tbEgyenleg.Text = $"{_egyenleg:F2} Ft";
            tbErme.Text = $"{ErmeErtekek[_ermeIdx]:F2} Ft";
            tbTet.Text = TetErtekek[_tetIdx].ToString();
            tbPorgesek.Text = _porgesek.ToString();
            tbNyeresek.Text = _nyeresek.ToString();
            tbLegjobbNyeremeny.Text = $"{_legjobbNyeremeny:F2} Ft";
            tbOsszes.Text = $"{_osszes:F2} Ft";
        }

        // Tét kezelő
        private void ErmeCs_Click(object s, RoutedEventArgs e)
        {
            if (_ermeIdx > 0) _ermeIdx--;
            FrissítKijelző();
        }
        private void ErmeN_Click(object s, RoutedEventArgs e)
        {
            if (_ermeIdx < ErmeErtekek.Length - 1) _ermeIdx++;
            FrissítKijelző();
        }
        private void TetCs_Click(object s, RoutedEventArgs e)
        {
            if (_tetIdx > 0) _tetIdx--;
            FrissítKijelző();
        }
        private void TetN_Click(object s, RoutedEventArgs e)
        {
            if (_tetIdx < TetErtekek.Length - 1) _tetIdx++;
            FrissítKijelző();
        }

        // Turbo meg az Automata
        private void TurboValt(object s, RoutedEventArgs e)
            => _turbo = tbgTurbo.IsChecked == true;

        private void AutoValt(object s, RoutedEventArgs e)
        {
            _auto = tbgAuto.IsChecked == true;
            if (_auto && !_porgo) _ = Pörget();
        }

        // Vissza gomb
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) { 
                EgyenlegManager.Balance.Egyenleg = _egyenleg; 
                NavigationService.GoBack();
            }
        }
    }
}