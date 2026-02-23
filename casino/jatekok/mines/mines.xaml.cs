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
using System.Xml.Linq;

namespace casino
{
    /// <summary>
    /// Interaction logic for mines.xaml
    /// </summary>
    public partial class mines : Page
    {
        // ── Mezők ─────────────────────────────────────────────────────────
        private MinesGame _game;
        private decimal _balance = 1000m;
        private const int GridSize = 5;

        // ── Konstruktor ───────────────────────────────────────────────────
        public mines()
        {
            InitializeComponent();
            _game = new MinesGame(GridSize, 3);
            _game.CellRevealed += OnCellRevealed;
            _game.MultiplierUpdated += OnMultiplierUpdated;
            _game.GameStateChanged += OnGameStateChanged;
            BuildGrid();
            UpdateBalanceDisplay();
        }

        // ── Grid felépítése ───────────────────────────────────────────────
        private void BuildGrid()
        {
            GameGrid.Children.Clear();
            GameGrid.Rows = GridSize;
            GameGrid.Columns = GridSize;

            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    var btn = new Button
                    {
                        Content = "?",
                        FontSize = 22,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Background = new SolidColorBrush(Color.FromRgb(15, 52, 96)),
                        BorderThickness = new Thickness(2),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(30, 80, 150)),
                        Margin = new Thickness(4),
                        Cursor = System.Windows.Input.Cursors.Hand,
                        IsEnabled = false,
                        Tag = (r, c)
                    };
                    btn.Click += CellButton_Click;
                    GameGrid.Children.Add(btn);
                }
            }
        }

        private Button GetButton(int row, int col)
        {
            return (Button)GameGrid.Children[row * GridSize + col];
        }

        // ── Cella kattintás ───────────────────────────────────────────────
        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var (row, col) = ((int, int))btn.Tag;
            _game.Reveal(row, col);
        }

        // ── Eseménykezelők ────────────────────────────────────────────────
        private void OnCellRevealed(int row, int col, bool isMine)
        {
            var btn = GetButton(row, col);
            if (isMine)
            {
                btn.Content = "💣";
                btn.Background = new SolidColorBrush(Color.FromRgb(180, 30, 30));
                btn.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 50, 50));
            }
            else
            {
                btn.Content = "💎";
                btn.Background = new SolidColorBrush(Color.FromRgb(20, 100, 80));
                btn.BorderBrush = new SolidColorBrush(Color.FromRgb(78, 204, 163));
            }
            btn.IsEnabled = false;
        }

        private void OnMultiplierUpdated(decimal multiplier)
        {
            MultiplierText.Text = $"{multiplier:F2}x";
            WinText.Text = $"${_game.CurrentWin:F2}";
            CashoutButton.IsEnabled = true;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Lost)
            {
                SetGridEnabled(false);
                RevealAllMines();
                CashoutButton.IsEnabled = false;
                StatusText.Text = "💥 Aknára léptél! Játssz újra!";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(233, 69, 96));
            }
            else if (state == GameState.Won)
            {
                SetGridEnabled(false);
                CashoutButton.IsEnabled = false;
                decimal win = _game.CurrentWin;
                _balance += win;
                UpdateBalanceDisplay();
                StatusText.Text = $"🎉 Nyertél! ${win:F2}";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163));
            }
            else if (state == GameState.Playing)
            {
                StatusText.Text = "Kattints egy cellára!";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170));
            }
        }

        // ── Segédfüggvények ───────────────────────────────────────────────
        private void SetGridEnabled(bool enabled)
        {
            foreach (Button btn in GameGrid.Children)
            {
                var (row, col) = ((int, int))btn.Tag;
                btn.IsEnabled = enabled && _game.Board[row, col] == CellState.Hidden;
            }
        }

        private void RevealAllMines()
        {
            for (int r = 0; r < GridSize; r++)
                for (int c = 0; c < GridSize; c++)
                    if (_game.IsMine(r, c) && _game.Board[r, c] != CellState.Mine)
                    {
                        var btn = GetButton(r, c);
                        btn.Content = "💣";
                        btn.Background = new SolidColorBrush(Color.FromRgb(100, 20, 20));
                        btn.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 30, 30));
                    }
        }

        private void UpdateBalanceDisplay()
        {
            BalanceText.Text = $"${_balance:F2}";
        }

        // ── Gomb eseménykezelők ───────────────────────────────────────────
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(BetInput.Text, out decimal bet) || bet <= 0)
            {
                MessageBox.Show("Érvénytelen tét!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (bet > _balance)
            {
                MessageBox.Show("Nincs elég egyenleged!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int mineCount = int.Parse(((ComboBoxItem)MineCountCombo.SelectedItem).Content.ToString());
            _game.Configure(GridSize, mineCount);
            _game.StartGame(bet);

            _balance -= bet;
            UpdateBalanceDisplay();

            BuildGrid();
            SetGridEnabled(true);

            MultiplierText.Text = "1.00x";
            WinText.Text = "$0.00";
            StartButton.Content = "🔄 Újrajáték";
            CashoutButton.IsEnabled = false;
        }

        private void CashoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal win = _game.Cashout();
                _balance += win;
                UpdateBalanceDisplay();
                MessageBox.Show($"Kifizetve! Nyeremény: ${win:F2}", "Nyertél! 🎉", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MineCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Üres, csak a XAML miatt kell
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        // ── Enumok ────────────────────────────────────────────────────────
        public enum CellState { Hidden, Revealed, Mine, SafeHighlight }
        public enum GameState { Idle, Playing, Won, Lost }

        // ── MinesGame osztály ─────────────────────────────────────────────
        public class MinesGame
        {
            public int GridSize { get; private set; }
            public int MineCount { get; private set; }
            public GameState State { get; private set; } = GameState.Idle;
            public CellState[,] Board { get; private set; }
            public int RevealedCount { get; private set; }
            public decimal Bet { get; private set; }
            public decimal CurrentMultiplier { get; private set; } = 1m;
            public decimal CurrentWin => Bet * CurrentMultiplier;

            public event Action<int, int, bool> CellRevealed;
            public event Action<decimal> MultiplierUpdated;
            public event Action<GameState> GameStateChanged;

            private readonly Random _rng = new Random();
            private readonly HashSet<int> _minePositions = new HashSet<int>();
            private int TotalCells => GridSize * GridSize;
            private int SafeCells => TotalCells - MineCount;

            public MinesGame(int gridSize = 5, int mineCount = 3) => Configure(gridSize, mineCount);

            public void Configure(int gridSize, int mineCount)
            {
                if (gridSize < 2 || gridSize > 10) throw new ArgumentOutOfRangeException(nameof(gridSize));
                if (mineCount < 1 || mineCount >= gridSize * gridSize) throw new ArgumentOutOfRangeException(nameof(mineCount));
                GridSize = gridSize;
                MineCount = mineCount;
                Board = new CellState[gridSize, gridSize];
                State = GameState.Idle;
            }

            public void StartGame(decimal bet)
            {
                if (bet <= 0) throw new ArgumentException("A tét pozitív szám kell legyen.");
                Bet = bet;
                RevealedCount = 0;
                CurrentMultiplier = 1m;
                _minePositions.Clear();
                for (int r = 0; r < GridSize; r++)
                    for (int c = 0; c < GridSize; c++)
                        Board[r, c] = CellState.Hidden;
                while (_minePositions.Count < MineCount)
                    _minePositions.Add(_rng.Next(TotalCells));
                State = GameState.Playing;
                GameStateChanged?.Invoke(State);
            }

            public bool Reveal(int row, int col)
            {
                if (State != GameState.Playing) throw new InvalidOperationException("Nincs aktív játék.");
                if (Board[row, col] != CellState.Hidden) return false;
                int pos = row * GridSize + col;
                if (_minePositions.Contains(pos))
                {
                    Board[row, col] = CellState.Mine;
                    _revealAllMines();
                    State = GameState.Lost;
                    CellRevealed?.Invoke(row, col, true);
                    GameStateChanged?.Invoke(State);
                    return false;
                }
                Board[row, col] = CellState.Revealed;
                RevealedCount++;
                CurrentMultiplier = CalculateMultiplier(RevealedCount);
                CellRevealed?.Invoke(row, col, false);
                MultiplierUpdated?.Invoke(CurrentMultiplier);
                if (RevealedCount >= SafeCells)
                {
                    State = GameState.Won;
                    GameStateChanged?.Invoke(State);
                }
                return true;
            }

            public decimal Cashout()
            {
                if (State != GameState.Playing || RevealedCount == 0)
                    throw new InvalidOperationException("Nincs mit kifizetni.");
                State = GameState.Won;
                _revealAllMines();
                GameStateChanged?.Invoke(State);
                return CurrentWin;
            }

            public decimal CalculateMultiplier(int revealed)
            {
                if (revealed <= 0) return 1m;
                decimal mult = 1m;
                for (int i = 0; i < revealed; i++)
                {
                    int remainingCells = TotalCells - i;
                    int remainingSafe = SafeCells - i;
                    decimal prob = (decimal)remainingSafe / remainingCells;
                    mult *= (1m / prob) * 0.97m;
                }
                return Math.Round(mult, 2);
            }

            private void _revealAllMines()
            {
                foreach (int pos in _minePositions)
                {
                    int r = pos / GridSize;
                    int c = pos % GridSize;
                    if (Board[r, c] == CellState.Hidden)
                        Board[r, c] = CellState.Mine;
                }
            }

            public bool IsMine(int row, int col) => _minePositions.Contains(row * GridSize + col);

            public int RemainingHidden =>
                (from r in Enumerable.Range(0, GridSize)
                 from c in Enumerable.Range(0, GridSize)
                 where Board[r, c] == CellState.Hidden
                 select 1).Count();
        }
    }
}
