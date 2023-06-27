using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnakeGame
{
    struct WinState
    {
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public double Border;
    }

    public partial class MainWindow : Window
    {
        private bool fullscreen = false;
        WinState winState;

        Border contentBorder;
        Border winBorder;
        Grid mainGrid;

        public GameSettings gameSettings { get; set; } = GameSettings.Default;

#pragma warning disable CS8618
        public MainWindow()
#pragma warning restore CS8618
        {
            InitializeComponent();

            PreviewKeyDown += MainWindow_PreviewKeyDown;
            StateChanged += MainWindow_StateChanged;
            Activated += MainWindow_Activated;

            WindowState = WindowState.Normal;

            Template = (ControlTemplate)Resources["normalWindowTemplate"];
        }

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            winBorder = (Border)Template.FindName("windowBorder", this);
            mainGrid = (Grid)Template.FindName("mainGrid", this);
            contentBorder = (Border)Template.FindName("ContentBorder", this);
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F11 && !e.IsRepeat)
            {
                if (WindowState != WindowState.Maximized)
                    WindowState = WindowState.Maximized;


                else
                    WindowState = WindowState.Normal;
            }
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Template = (ControlTemplate)Resources["maximaizedWindowTemplate"];
            }
            else
            {
                Template = (ControlTemplate)Resources["normalWindowTemplate"];
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void hideButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void fullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!fullscreen)
            {
                winState = new WinState { Top = Top, Left = Left, Height = Height, Width = Width, Border = 0.5 };
                Top = 0;
                Left = 0;
                Width = SystemParameters.FullPrimaryScreenWidth;
                Height = SystemParameters.FullPrimaryScreenHeight + SystemParameters.CaptionHeight;
                winBorder.BorderThickness = new Thickness(0);


                fullscreen = true;
            }
            else
            {
                Top = winState.Top;
                Left = winState.Left;
                Height = winState.Height;
                Width = winState.Width;
                winBorder.BorderThickness = new Thickness(winState.Border);

                fullscreen = false;
            }
        }

        private void TopPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!fullscreen && WindowState == WindowState.Normal)
                try
                {
                    DragMove();
                }
                catch { }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            SetGameScreen();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetSettingsScreen();
        }

        /// <summary>
        /// Shows game screen and starts the game
        /// </summary>
        public void SetGameScreen()
        {
            Game game;
            try
            {
                game = new Game(gameSettings);
            }
            catch
            {
                SetSettingsScreen();
                return;
            }

            screenGrid.Children.Clear();
            screenGrid.Children.Add(game.Border);
            screenGrid.Children.Add(game.InfoWindow);

            game.Start();

            Title = "Game";
        }

        /// <summary>
        /// Shows app enter screen
        /// </summary>
        public void SetHelloScreen()
        {
            screenGrid.Children.Clear();
            screenGrid.Children.Add(helloScreen);


            Title = "Main screen";
        }

        /// <summary>
        /// Shows settings changing screen
        /// </summary>
        public void SetSettingsScreen()
        {
            screenGrid.Children.Clear();
            screenGrid.Children.Add(GameSettings.GetSettingsChanger());

            Title = "Settings";
        }
    }
}