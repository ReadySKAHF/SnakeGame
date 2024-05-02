using SnakeGameWpf.pages;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameLibrary;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        MainPage _mainPage;

        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            //WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;

            _mainPage = new MainPage(MainFrame, () => Close());
            MainFrame.Content = _mainPage;
        }
    }
}