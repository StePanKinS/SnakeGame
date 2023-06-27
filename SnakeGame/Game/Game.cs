using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SnakeGame.GameState;

namespace SnakeGame
{
    /// <summary>
    /// Represents game status 
    /// </summary>
    public enum GameStatus
    {
        WaitStart,
        Runs,
        Paused,
        Stopped
    }

    /// <summary>
    /// Main game class. Controls keyboard input, game visual, game timer and game start/stop/pause.
    /// </summary>
    public class Game
    {
        public GameState game { get; protected set; }
        public GameStatus status { get; protected set; }
        public int[] speedLevelTimes { get; protected set; }
        public Timer gameTimer { get; protected set; }
        public Direction direction { get; protected set; }
        public Grid gameScreen { get; protected set; }
        public Border Border { get; protected set; }
        public Border InfoWindow { get; protected set; }

        private static MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        /// <summary>
        /// Creates the game with default settings
        /// </summary>
        public Game() : this(GameSettings.Default) { }
        /// <summary>
        /// Creates the game with custom settings
        /// </summary>
        /// <param name="settings"> Game settings (map sizes, snake start position, ...). GameSettings </param>
        public Game(GameSettings settings)
        {
            speedLevelTimes = settings.SpeedLeveTimes;
            game = new GameState(settings);
            status = GameStatus.WaitStart;
            direction = settings.StartDirection;

            gameTimer = new Timer(Tick);
            game.speedLevelChanged += () => gameTimer.Change(0, speedLevelTimes[game.speedLevel - 1]);

            InfoWindow = new Border();
            ConstractInfoBorder();

            gameScreen = new Grid();
            for (int i = 0; i < settings.MapWidth; i++)
            {
                gameScreen.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < settings.MapHeight; i++)
            {
                gameScreen.RowDefinitions.Add(new RowDefinition());
            }

            Border = new Border();
            ConstractGameBorder();


            mainWindow.PreviewKeyDown += KeyDown;
            mainWindow.screenGrid.SizeChanged += ScreenSizeChanged;
        }

        private void ConstractInfoBorder()
        {
            InfoWindow.BorderBrush = new SolidColorBrush(Colors.Transparent);
            InfoWindow.Background = new SolidColorBrush(Colors.Transparent);
            InfoWindow.HorizontalAlignment = HorizontalAlignment.Center;
            InfoWindow.VerticalAlignment = VerticalAlignment.Center;
            InfoWindow.BorderThickness = new Thickness(1);
        }

        private Grid getCrackWindow(string infoText)
        {
            Grid crackWindow = new Grid();

            crackWindow.HorizontalAlignment = HorizontalAlignment.Center;
            crackWindow.VerticalAlignment = VerticalAlignment.Center;
            crackWindow.Margin = new Thickness(10);

            crackWindow.RowDefinitions.Add(new RowDefinition());
            crackWindow.RowDefinitions.Add(new RowDefinition());
            crackWindow.ColumnDefinitions.Add(new ColumnDefinition());
            crackWindow.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock text = new TextBlock();
            text.Text = infoText;
            text.Margin = new Thickness(5);
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.FontSize = 24;
            text.Foreground = new SolidColorBrush(Colors.Red);
            Grid.SetColumnSpan(text, 2);

            MyBtn restartButton = new MyBtn();
            restartButton.Margin = new Thickness(5);
            restartButton.VerticalAlignment = VerticalAlignment.Center;
            restartButton.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock restartButtonText = new TextBlock();
            restartButtonText.Text = "Нова игра";
            restartButtonText.Foreground = new SolidColorBrush(Colors.BlueViolet);
            restartButtonText.FontSize = 14;
            restartButton.Content = restartButtonText;
            Grid.SetRow(restartButton, 1);
            restartButton.Click += RestartButtonClick;

            MyBtn menuButton = new MyBtn();
            menuButton.Margin = new Thickness(5);
            menuButton.VerticalAlignment = VerticalAlignment.Center;
            menuButton.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock menuButtonText = new TextBlock();
            menuButtonText.Text = "В меню";
            menuButtonText.Foreground = new SolidColorBrush(Colors.BlueViolet);
            menuButtonText.FontSize = 14;
            menuButton.Content = menuButtonText;
            Grid.SetColumn(menuButton, 1);
            Grid.SetRow(menuButton, 1);
            menuButton.Click += MenuButtonClick;

            crackWindow.Children.Add(text);
            crackWindow.Children.Add(restartButton);
            crackWindow.Children.Add(menuButton);

            return crackWindow;
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            mainWindow.SetHelloScreen();
        }

        private void RestartButtonClick(object sender, RoutedEventArgs e)
        {
            mainWindow.SetGameScreen();
        }

        private void ResumeButtonClick(object sender, RoutedEventArgs e)
        {
            Resume();
        }

        private void ConstractGameBorder()
        {
            Grid grid = mainWindow.screenGrid;
            Border.Child = gameScreen;
            Border.BorderBrush = new SolidColorBrush(Colors.DarkRed);
            Border.BorderThickness = new Thickness(3);
            Border.HorizontalAlignment = HorizontalAlignment.Center;
            Border.VerticalAlignment = VerticalAlignment.Center;
            if (grid.ActualHeight / grid.ActualWidth > (double)game.height / game.width)
            {
                Border.Width = grid.ActualWidth - 10;
                Border.Height = Border.Width * game.height / game.width;
            }
            else
            {
                Border.Height = grid.ActualHeight - 10;
                Border.Width = Border.Height * game.width / game.height;
            }
        }

        private void ScreenSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid grid = mainWindow.screenGrid;

            if (grid.ActualHeight / grid.ActualWidth > (double)game.height / game.width)
            {
                Border.Width = grid.ActualWidth - 10;
                Border.Height = Border.Width * game.height / game.width;
            }
            else
            {
                Border.Height = grid.ActualHeight - 10;
                Border.Width = Border.Height * game.width / game.height;
            }

            Update();

        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        direction = Direction.Up;
                        break;
                    }
                case Key.Down:
                    {
                        direction = Direction.Down;
                        break;
                    }
                case Key.Right:
                    {
                        direction = Direction.Right;
                        break;
                    }
                case Key.Left:
                    {
                        direction = Direction.Left;
                        break;
                    }
                case Key.Escape:
                    {
                        if (status == GameStatus.Runs)           Pause();
                        else if (status == GameStatus.Paused)    Resume();
                        break;
                    }

            }
        }

        #region GameControl
        /// <summary>
        /// Starts the game if it is not already started
        /// </summary>
        public void Start()
        {
            if (status != GameStatus.WaitStart)
                return;

            gameTimer.Change(0, speedLevelTimes[game.speedLevel - 1]);

            status = GameStatus.Runs;
        }

        /// <summary>
        /// Pauses the game if it is running
        /// </summary>
        public void Pause()
        {
            if (status != GameStatus.Runs)
                return;

            gameTimer.Change(-1, -1);

            Grid window = getCrackWindow("Пауза");
            MyBtn btn = (MyBtn)window.Children[1];
            ((TextBlock)btn.Content).Text = "Продолжить";
            btn.Click -= RestartButtonClick;
            btn.Click += ResumeButtonClick;

            Application.Current.Dispatcher.Invoke(() =>
            {
                InfoWindow.Child = window;
                ((SolidColorBrush)InfoWindow.Background).Color = Color.FromArgb(128, 240, 240, 240);
                ((SolidColorBrush)InfoWindow.BorderBrush).Color = Color.FromArgb(128, 15, 15, 15);
            });

            status = GameStatus.Paused;
        }

        /// <summary>
        /// Resumes the game if it is paused
        /// </summary>
        public void Resume()
        {
            if (status != GameStatus.Paused)
                return;

            gameTimer.Change(0, speedLevelTimes[game.speedLevel - 1]);

            Application.Current.Dispatcher.Invoke(() =>
            {
                InfoWindow.Child = null;
                ((SolidColorBrush)InfoWindow.Background).Color = Colors.Transparent;
                ((SolidColorBrush)InfoWindow.BorderBrush).Color = Colors.Transparent;
            });

            status = GameStatus.Runs;
        }

        /// <summary>
        /// Stops the game
        /// </summary>
        public void Stop()
        {
            gameTimer.Dispose();
            status = GameStatus.Stopped;
        }
        #endregion

        private void Tick(object? obj)
        {
            try
            {
                game.Go(direction);
            }
            catch (SelfCracked)
            {
                Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    InfoWindow.Child = getCrackWindow("Вы разбились об себя");
                    ((SolidColorBrush)InfoWindow.Background).Color = Color.FromArgb(128, 240, 240, 240);
                    ((SolidColorBrush)InfoWindow.BorderBrush).Color = Color.FromArgb(128, 15, 15, 15);
                });
            }
            catch (WallCracked)
            {
                Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    InfoWindow.Child = getCrackWindow("Вы разбились об стену");
                    ((SolidColorBrush)InfoWindow.Background).Color = Color.FromArgb(128, 240, 240, 240);
                    ((SolidColorBrush)InfoWindow.BorderBrush).Color = Color.FromArgb(128, 15, 15, 15);
                });
            }
            catch (NoMorePlase)
            {
                Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    InfoWindow.Child = getCrackWindow("На поле закончилось место");
                    ((SolidColorBrush)InfoWindow.Background).Color = Color.FromArgb(128, 240, 240, 240);
                    ((SolidColorBrush)InfoWindow.BorderBrush).Color = Color.FromArgb(128, 15, 15, 15);
                });
            }

            Application.Current.Dispatcher.Invoke(Update);
        }

        /// <summary>
        /// Redraws the game screen
        /// </summary>
        public void Update()
        {
            gameScreen.Children.Clear();

            Border head = new Border();
            Grid.SetColumn(head, game.head.x);
            Grid.SetRow(head, game.head.y);

            head.Background = new SolidColorBrush(Colors.Red);

            gameScreen.Children.Add(head);

            foreach (Body body in game.body)
            {
                Border border = new Border();
                border.Background = new SolidColorBrush(Colors.Gray);

                Grid.SetColumn(border, body.x);
                Grid.SetRow(border, body.y);

                gameScreen.Children.Add(border);
            }

            Border tail = new Border();
            tail.Background = new SolidColorBrush(Colors.Gray);
            Grid.SetColumn(tail, game.tail.x);
            Grid.SetRow(tail, game.tail.y);
            gameScreen.Children.Add(tail);


            Border apple = new Border();
            apple.Background = new SolidColorBrush(Colors.LightGreen);
            Grid.SetColumn(apple, game.apple.x);
            Grid.SetRow(apple, game.apple.y);
            gameScreen.Children.Add(apple);
        }
    }
}
