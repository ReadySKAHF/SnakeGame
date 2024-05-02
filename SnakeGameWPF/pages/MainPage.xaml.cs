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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private int _initialLength = 3;
        Frame _mainFrame;
        GamePage _gamePage;
        InformationPage _informationPage;
        SettingsPage _settingsPage;
        Action _closeWindow;
        SettingsContainer _settingsContainer;

        public MainPage(Frame mainFrame, Action closeWindow)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _settingsContainer = new SettingsContainer( 3 );
            _gamePage = new GamePage(() => { _mainFrame.Content = this; }, _settingsContainer);
            _informationPage = new InformationPage(() => { _mainFrame.Content = this; });
            _settingsPage = new SettingsPage(() => { _mainFrame.Content = this; }, _settingsContainer);
            _closeWindow = closeWindow;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButton.Focusable = false;
            _mainFrame.Content =  _gamePage;
            _gamePage.StartGame();
        }

        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Content = _informationPage;
            _informationPage.Focusable = true;
            _informationPage.Focus();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Content = _settingsPage;
            _settingsPage.Focusable = true;
            _settingsPage.Focus();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _closeWindow?.Invoke();
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _closeWindow.Invoke();
            }
        }
    }
}
