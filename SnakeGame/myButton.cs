using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnakeGame
{
    /// <summary>
    /// Extention to the button. Provides color changing on mouse enter / click
    /// </summary>
    class MyBtn : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty BackgroundMouseOverProperty;
        public static readonly DependencyProperty BackgroundMouseClickProperty;
        public static readonly DependencyProperty BorderMouseOverProperty;
        public static readonly DependencyProperty BorderMouseClickProperty;


        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public Brush BackgroundMouseOver
        {
            get => (Brush)GetValue(BackgroundMouseOverProperty);
            set => SetValue(BackgroundMouseOverProperty, value);
        }
        public Brush BackgroundMouseClick
        {
            get => (Brush)GetValue(BackgroundMouseClickProperty);
            set => SetValue(BackgroundMouseClickProperty, value);
        }
        public Brush BorderMouseOver
        {
            get => (Brush)GetValue(BorderMouseOverProperty);
            set => SetValue(BorderMouseOverProperty, value);
        }
        public Brush BorderMouseClick
        {
            get => (Brush)GetValue(BorderMouseClickProperty);
            set => SetValue(BorderMouseClickProperty, value);
        }

        private Brush bg = new SolidColorBrush();
        private Brush brd = new SolidColorBrush();

        static MyBtn()
        {
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MyBtn));
            BackgroundMouseOverProperty = DependencyProperty.Register("BackgroundMouseOver", typeof(Brush), typeof(MyBtn));
            BackgroundMouseClickProperty = DependencyProperty.Register("BackgroundMouseClick", typeof(Brush), typeof(MyBtn));
            BorderMouseOverProperty = DependencyProperty.Register("BorderMouseOver", typeof(Brush), typeof(MyBtn));
            BorderMouseClickProperty = DependencyProperty.Register("BorderMouseClick", typeof(Brush), typeof(MyBtn));
        }

        public MyBtn() : base()
        {

            MouseEnter += btn_MouseEnter;
            MouseLeave += btn_MouseLeave;
            PreviewMouseLeftButtonDown += btn_MouseDown;
            PreviewMouseLeftButtonUp += btn_MouseUp;
        }

        private void btn_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMouseOver)
            {
                Background = BackgroundMouseOver;
                BorderBrush = BorderMouseOver;
            }
            else
            {
                Background = bg;
                BorderBrush = brd;
            }
        }

        private void btn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Background = bg;
            BorderBrush = brd;
        }

        private void btn_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Background = BackgroundMouseClick;
            BorderBrush = BorderMouseClick;
        }

        private void btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            bg = Background;
            brd = BorderBrush;

            Background = BackgroundMouseOver;
            BorderBrush = BorderMouseOver;
        }
    }
}
