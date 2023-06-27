using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace SnakeGame
{
    /// <summary>
    /// Represents game start parametrs (map sizes, snake start position, snake direction, snake speed). Controls visual and loginc of changinns start settings
    /// </summary>
    public class GameSettings
    {
        public static GameSettings Default { get; } = new GameSettings(
            30,
            20,
            10,
            10,
            Direction.Right,
            new int[] { 300, 200, 150, 100, 90, 80, 70, 60, 50 },
            new int[] { 30, 30, 30, 30, 30, 30, 30, 30, },
            "Default"
            );

        public static List<GameSettings> settingsList { get; set; } = new List<GameSettings>();
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int HeadStartX { get; set; }
        public int HeadStartY { get; set; }
        public Direction StartDirection { get; set; }
        public int[] SpeedLeveTimes { get; set; }
        public int[] ChangeSpeedLevelOn { get; set; }
        public string Name { get; set; }

        public GameSettings(int mapWidth, int mapHeight, int headStartX, int headStartY, Direction startDirection, int[] speedLeveTimes, int[] changeSpeedLevelOn, string name)
        {
            MapHeight = mapHeight;
            MapWidth = mapWidth;
            HeadStartX = headStartX;
            HeadStartY = headStartY;
            StartDirection = startDirection;
            SpeedLeveTimes = speedLeveTimes;
            ChangeSpeedLevelOn = changeSpeedLevelOn;
            Name = name;
        }

        static GameSettings()
        {
            StreamReader reader = new StreamReader("CustomSettings.json");

            string text = reader.ReadToEnd();

            reader.Close();


            List<GameSettings>? gameSettings = JsonSerializer.Deserialize<List<GameSettings>>(text);
            settingsList = gameSettings != null ? gameSettings : new List<GameSettings>() { Default };
            if (settingsList.Count == 0)
            {
                settingsList.Add(Default);
                settingsList.Add(new GameSettings(0, 0, 0, 0, 0, new int[0], new int[0], "bruh"));
            }

            for (int i = 0; i < settingsList.Count; i++)
            {
                if (settingsList[i].Name != Default.Name)
                    continue;

                if (settingsList[i].StartDirection != Default.StartDirection)
                    continue;

                if (settingsList[i].HeadStartX != Default.HeadStartX)
                    continue;

                if (settingsList[i].HeadStartY != Default.HeadStartY)
                    continue;

                if (settingsList[i].MapWidth != Default.MapWidth)
                    continue;

                if (settingsList[i].MapHeight != Default.MapHeight)
                    continue;

                if (!settingsList[i].ChangeSpeedLevelOn.ListEquals(Default.ChangeSpeedLevelOn))
                    continue;

                if (!settingsList[i].SpeedLeveTimes.ListEquals(Default.SpeedLeveTimes))
                    continue;

                if(i != 0)
                {
                    settingsList.RemoveAt(i);
                }
                else
                {
                    settingsList[i] = Default;
                }
            }
        }

        /// <summary>
        /// Saves settingsList to CustomSettings.json file
        /// </summary>
        public static void SaveSettings()
        {
            string json = JsonSerializer.Serialize(settingsList);
            StreamWriter writer = new StreamWriter("CustomSettings.json", false);
            writer.Write(json);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Creates the settings control screen
        /// </summary>
        /// <returns> Settings screen. Grid </returns>
        public static Grid GetSettingsChanger()
        {
            Grid grid = new Grid();


            TabControl tabControl = new TabControl();
            foreach (GameSettings settings in settingsList)
            {
                if (settings == Default) tabControl.Items.Add(settings.CreateDefaultTab());

                else tabControl.Items.Add(settings.CreateTab());
            }
            tabControl.VerticalAlignment = VerticalAlignment.Stretch;
            tabControl.SelectionChanged += TabChanged;
            tabControl.SelectedIndex = settingsList.IndexOf(((MainWindow)Application.Current.MainWindow).gameSettings);

            MyBtn menuButton = new MyBtn();
            menuButton.Margin = new Thickness(5, 5, 30, 20);
            menuButton.VerticalAlignment = VerticalAlignment.Bottom;
            menuButton.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock menuButtonText = new TextBlock();
            menuButtonText.Text = "В меню";
            menuButtonText.Foreground = new SolidColorBrush(Colors.BlueViolet);
            menuButtonText.FontSize = 14;
            menuButton.Content = menuButtonText;
            menuButton.Click += MenuButtonClick;

            MyBtn newTabButton = new MyBtn();
            newTabButton.Margin = new Thickness(30, 5, 5, 20);
            newTabButton.VerticalAlignment = VerticalAlignment.Bottom;
            newTabButton.HorizontalAlignment = HorizontalAlignment.Left;
            TextBlock newTabButtonText = new TextBlock();
            newTabButtonText.Text = "Новая вкладка";
            newTabButtonText.Foreground = new SolidColorBrush(Colors.BlueViolet);
            newTabButtonText.FontSize = 14;
            newTabButton.Content = newTabButtonText;
            newTabButton.Click += NewTabButtonClick;

            double bottom = menuButton.FontSize + 10 + menuButton.Margin.Bottom + menuButton.Margin.Top;
            tabControl.Margin = new Thickness(0, 0, 0, bottom);

            grid.Children.Add(tabControl);
            grid.Children.Add(menuButton);
            grid.Children.Add(newTabButton);

            return grid;
        }

        private TabItem CreateTab()
        {
            TabItem tabItem = new TabItem();
            Grid grid = new Grid();
            grid.ShowGridLines = false;

            for (int i = 0; i < 6; i++) grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            TextBox nameBox = new TextBox();
            nameBox.Text = Name;
            nameBox.TextChanged += NameChanged;
            nameBox.HorizontalAlignment = HorizontalAlignment.Left;
            nameBox.VerticalAlignment = VerticalAlignment.Center;
            nameBox.Margin = new Thickness(30, 0, 0, 0);
            nameBox.FontSize = 14;

            TextBlock header = new TextBlock();

            MyBtn myBtn = new MyBtn();
            myBtn.Content = "×";
            myBtn.Margin = new Thickness(5, 0, 0, 0);
            myBtn.HorizontalAlignment = HorizontalAlignment.Center;
            myBtn.VerticalAlignment = VerticalAlignment.Center;
            myBtn.Click += tabCloseClick;

            stackPanel.Children.Add(header);
            stackPanel.Children.Add(myBtn);

            tabItem.Header = stackPanel;

            Binding binding = new Binding();
            binding.Source = nameBox;
            binding.Mode = BindingMode.OneWay;
            binding.Path = new PropertyPath("Text");
            header.SetBinding(TextBlock.TextProperty, binding);

            AddPropertyValueChanger(grid);
            AddPropertyHeaders(grid);
            grid.Children.Add(nameBox);


            tabItem.Content = grid;
            return tabItem;
        }

        private TabItem CreateDefaultTab()
        {
            TabItem tabItem = new TabItem();
            Grid grid = new Grid();
            grid.ShowGridLines = false;

            for (int i = 0; i < 6; i++) grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());


            TextBlock nameBox = new TextBlock();
            nameBox.Text = Name;
            nameBox.HorizontalAlignment = HorizontalAlignment.Left;
            nameBox.VerticalAlignment = VerticalAlignment.Center;
            nameBox.Margin = new Thickness(30, 0, 0, 0);
            nameBox.FontSize = 14;

            AddPropertyHeaders(grid);
            AddPropertyValuePreview(grid);


            tabItem.Header = Name;
            grid.Children.Add(nameBox);

            tabItem.Content = grid;
            return tabItem;
        }

        private void AddPropertyHeaders(Grid grid)
        {
            TextBlock mapSection = new TextBlock();
            mapSection.Text = "Настройки поля";
            mapSection.TextDecorations = TextDecorations.Underline;
            mapSection.FontSize = 14;
            mapSection.HorizontalAlignment = HorizontalAlignment.Left;
            mapSection.VerticalAlignment = VerticalAlignment.Bottom;
            mapSection.Margin = new Thickness(20, 0, 0, 5);
            Grid.SetRow(mapSection, 1);

            TextBlock mapWidth = new TextBlock();
            mapWidth.Text = "Ширина поля:";
            mapWidth.FontSize = 14;
            mapWidth.HorizontalAlignment = HorizontalAlignment.Left;
            mapWidth.VerticalAlignment = VerticalAlignment.Top;
            mapWidth.Margin = new Thickness(20, 10, 0, 0);
            Grid.SetRow(mapWidth, 2);

            TextBlock mapHeight = new TextBlock();
            mapHeight.Text = "Выста поля:";
            mapHeight.FontSize = 14;
            mapHeight.HorizontalAlignment = HorizontalAlignment.Left;
            mapHeight.VerticalAlignment = VerticalAlignment.Bottom;
            mapHeight.Margin = new Thickness(20, 0, 0, 10);
            Grid.SetRow(mapHeight, 2);

            TextBlock startSection = new TextBlock();
            startSection.Text = "Позиция головы на старте";
            startSection.TextDecorations = TextDecorations.Underline;
            startSection.FontSize = 14;
            startSection.HorizontalAlignment = HorizontalAlignment.Left;
            startSection.VerticalAlignment = VerticalAlignment.Bottom;
            startSection.Margin = new Thickness(20, 0, 0, 5);
            Grid.SetRow(startSection, 3);

            TextBlock xStart = new TextBlock();
            xStart.Text = "От левого края:";
            xStart.FontSize = 14;
            xStart.HorizontalAlignment = HorizontalAlignment.Left;
            xStart.VerticalAlignment = VerticalAlignment.Top;
            xStart.Margin = new Thickness(20, 10, 0, 0);
            Grid.SetRow(xStart, 4);

            TextBlock yStart = new TextBlock();
            yStart.Text = "От верхнего края:";
            yStart.FontSize = 14;
            yStart.HorizontalAlignment = HorizontalAlignment.Left;
            yStart.VerticalAlignment = VerticalAlignment.Bottom;
            yStart.Margin = new Thickness(20, 0, 0, 10);
            Grid.SetRow(yStart, 4);

            grid.Children.Add(mapSection);
            grid.Children.Add(mapWidth);
            grid.Children.Add(mapHeight);
            grid.Children.Add(startSection);
            grid.Children.Add(xStart);
            grid.Children.Add(yStart);
        }

        private void AddPropertyValuePreview(Grid grid)
        {
            TextBlock mapWidth = new TextBlock();
            mapWidth.Text = $"{Default.MapWidth}";
            mapWidth.FontSize = 14;
            mapWidth.HorizontalAlignment = HorizontalAlignment.Right;
            mapWidth.VerticalAlignment = VerticalAlignment.Top;
            mapWidth.Margin = new Thickness(0, 10, 20, 0);
            Grid.SetRow(mapWidth, 2);
            Grid.SetColumn(mapWidth, 1);

            TextBlock mapHeight = new TextBlock();
            mapHeight.Text = $"{Default.MapHeight}";
            mapHeight.FontSize = 14;
            mapHeight.HorizontalAlignment = HorizontalAlignment.Right;
            mapHeight.VerticalAlignment = VerticalAlignment.Bottom;
            mapHeight.Margin = new Thickness(0, 0, 20, 10);
            Grid.SetRow(mapHeight, 2);
            Grid.SetColumn(mapHeight, 1);

            TextBlock xStart = new TextBlock();
            xStart.Text = $"{Default.HeadStartX}";
            xStart.FontSize = 14;
            xStart.HorizontalAlignment = HorizontalAlignment.Right;
            xStart.VerticalAlignment = VerticalAlignment.Top;
            xStart.Margin = new Thickness(0, 10, 20, 0);
            Grid.SetRow(xStart, 4);
            Grid.SetColumn(xStart, 1);

            TextBlock yStart = new TextBlock();
            yStart.Text = $"{Default.HeadStartY}";
            yStart.FontSize = 14;
            yStart.HorizontalAlignment = HorizontalAlignment.Right;
            yStart.VerticalAlignment = VerticalAlignment.Bottom;
            yStart.Margin = new Thickness(0, 0, 20, 10);
            Grid.SetRow(yStart, 4);
            Grid.SetColumn(yStart, 1);


            grid.Children.Add(mapWidth);
            grid.Children.Add(mapHeight);
            grid.Children.Add(xStart);
            grid.Children.Add(yStart);
        }

        private void AddPropertyValueChanger(Grid grid)
        {
            TextBox mapWidth = new TextBox();
            mapWidth.Text = $"{MapWidth}";
            mapWidth.FontSize = 14;
            mapWidth.HorizontalAlignment = HorizontalAlignment.Right;
            mapWidth.VerticalAlignment = VerticalAlignment.Top;
            mapWidth.Margin = new Thickness(0, 10, 20, 0);
            mapWidth.Width = 150;
            mapWidth.HorizontalContentAlignment = HorizontalAlignment.Right;
            Grid.SetRow(mapWidth, 2);
            Grid.SetColumn(mapWidth, 1);
            mapWidth.TextChanged += (o, e) => {
                if (new Regex(@"^\s*\d+\s*$", RegexOptions.Multiline).Matches(((TextBox)e.Source).Text).Count > 0)
                {
                    try
                    {
                        MapWidth = int.Parse(((TextBox)o).Text);
                        ((TextBox)o).Background = new SolidColorBrush(Colors.White);
                    }
                    catch (OverflowException)
                    {
                        ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                    }
                }
                else
                {
                    ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                }
            };


            TextBox mapHeight = new TextBox();
            mapHeight.Text = $"{MapHeight}";
            mapHeight.FontSize = 14;
            mapHeight.HorizontalAlignment = HorizontalAlignment.Right;
            mapHeight.VerticalAlignment = VerticalAlignment.Bottom;
            mapHeight.Margin = new Thickness(0, 0, 20, 10);
            mapHeight.Width = 150;
            mapHeight.HorizontalContentAlignment = HorizontalAlignment.Right;
            Grid.SetRow(mapHeight, 2);
            Grid.SetColumn(mapHeight, 1);
            mapHeight.TextChanged += (o, e) => {
                if (new Regex(@"^\s*\d+\s*$", RegexOptions.Multiline).Matches(((TextBox)e.Source).Text).Count > 0)
                {
                    try
                    {
                        MapHeight = int.Parse(((TextBox)o).Text);
                        ((TextBox)o).Background = new SolidColorBrush(Colors.White);
                    }
                    catch (OverflowException)
                    {
                        ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                    }
                }
                else
                {
                    ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

                }
            };

            TextBox xStart = new TextBox();
            xStart.Text = $"{HeadStartX}";
            xStart.FontSize = 14;
            xStart.HorizontalAlignment = HorizontalAlignment.Right;
            xStart.VerticalAlignment = VerticalAlignment.Top;
            xStart.Margin = new Thickness(0, 10, 20, 0);
            xStart.Width = 150;
            xStart.HorizontalContentAlignment = HorizontalAlignment.Right;
            Grid.SetRow(xStart, 4);
            Grid.SetColumn(xStart, 1);
            xStart.TextChanged += (o, e) => {
                if (new Regex(@"^\s*\d+\s*$", RegexOptions.Multiline).Matches(((TextBox)e.Source).Text).Count > 0)
                {
                    try
                    {
                        HeadStartX = int.Parse(((TextBox)o).Text);
                        ((TextBox)o).Background = new SolidColorBrush(Colors.White);
                    }
                    catch (OverflowException)
                    {
                        ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                    }
                }
                else
                {
                    ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

                }
            };

            TextBox yStart = new TextBox();
            yStart.Text = $"{HeadStartY}";
            yStart.FontSize = 14;
            yStart.HorizontalAlignment = HorizontalAlignment.Right;
            yStart.VerticalAlignment = VerticalAlignment.Bottom;
            yStart.Margin = new Thickness(0, 0, 20, 10);
            yStart.Width = 150;
            yStart.HorizontalContentAlignment = HorizontalAlignment.Right;
            Grid.SetRow(yStart, 4);
            Grid.SetColumn(yStart, 1);
            yStart.TextChanged += (o, e) => {
                if (new Regex(@"^\s*\d+\s*$", RegexOptions.Multiline).Matches(((TextBox)e.Source).Text).Count > 0)
                {
                    try
                    {
                        HeadStartY = int.Parse(((TextBox)o).Text);
                        ((TextBox)o).Background = new SolidColorBrush(Colors.White);
                    }
                    catch (OverflowException)
                    {
                        ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                    }
                }
                else
                {
                    ((TextBox)o).Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                }
            };


            grid.Children.Add(mapWidth);
            grid.Children.Add(mapHeight);
            grid.Children.Add(xStart);
            grid.Children.Add(yStart);
        }

        private static void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((TabControl)sender).SelectedIndex < 0)
                return;

            ((MainWindow)Application.Current.MainWindow).gameSettings = settingsList[((TabControl)e.Source).SelectedIndex];
        }

        private void NameChanged(object sender, TextChangedEventArgs e)
        {
            Name = ((TextBox)e.Source).Text;
        }

        private static void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ((MainWindow)Application.Current.MainWindow).SetHelloScreen();
        }

        private static void NewTabButtonClick(object sender, RoutedEventArgs e)
        {
            GameSettings settings = new GameSettings(
            30,
            20,
            10,
            10,
            Direction.Right,
            new int[] { 300, 200, 150, 100, 90, 80, 70, 60, 50 },
            new int[] { 30, 30, 30, 30, 30, 30, 30, 30, },
            "New"
            );

            settingsList.Add(settings);
            ((TabControl)((Grid)((MyBtn)sender).Parent).Children[0]).Items.Add(settings.CreateTab());
        }

        private void tabCloseClick(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)((StackPanel)((MyBtn)sender).Parent).Parent;
            TabControl tabControl = (TabControl)tab.Parent;
            tabControl.Items.Remove(tab);

            settingsList.Remove(this);
        }

        /// <summary>
        /// Returns name
        /// </summary>
        /// <returns> Name </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
