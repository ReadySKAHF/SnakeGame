using GameLibrary;
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

namespace SnakeGameWpf.pages
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private Action _gotToPreviousPage;
        private SettingsContainer _settingsContainer;

        public GamePage(Action gotToPreviousPage, SettingsContainer settingsContainer)
        {
            InitializeComponent();
            _gotToPreviousPage = gotToPreviousPage;
            _settingsContainer = settingsContainer;
        }

        public void StartGame()
        {
            Keyboard.ClearFocus();
            MainGrid.Children.Clear();
            var SnakeGameElement = new GameElement(() => _gotToPreviousPage(), _settingsContainer);
            MainGrid.Children.Add(SnakeGameElement);

            SnakeGameElement.Focusable = true;
            SnakeGameElement.Focus();
            SnakeGameElement.Run();
        }
    }
}

