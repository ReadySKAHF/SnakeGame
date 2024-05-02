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
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        event Action _goToPreviousPage;
        SettingsContainer _settingsContainer;

        public SettingsPage(Action goToPreviousPage, SettingsContainer settingsContainer)
        {
            InitializeComponent();
            _goToPreviousPage = goToPreviousPage;
            _settingsContainer = settingsContainer;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Focusable = false;
                _goToPreviousPage?.Invoke();
            }
            
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(lengthBox.Text, out int result))
                {
                    if (result < 2)
                    {
                        MessageBox.Show("Длина змей должна быть целым положительным числом и больше чем 2.");
                        return;
                    }

                    _settingsContainer.SnakeLength = result;
                    MessageBox.Show("Длина змей успешно сохранена.");
                }
                else
                {
                    MessageBox.Show("Длина змей должна быть целым положительным числом и больше чем 2.");
                }
            }
        }
    }
}
