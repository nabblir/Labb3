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

namespace Labb3.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
        {
        MainWindow mainWindow = (Labb3.MainWindow)Application.Current.MainWindow;
        public MenuView()
            {
            InitializeComponent();
            }
        private void NewQuestionPack_Click(object sender, RoutedEventArgs e)
            {
            // Handle New Question Pack click
            }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
            {
            // Handle Add Question click
            }

        private void Play_Click(object sender, RoutedEventArgs e)
            {
            mainWindow.ClearContentArea();
            var playView = new PlayerView();
            mainWindow.SetContentArea(playView);
            UpdateEnabledState(PlayMenuItem);
            }

        private void SetupQuiz_Click(object sender, RoutedEventArgs e)
            {
            mainWindow.ClearContentArea();
            var configView = new ConfigurationView();
            mainWindow.SetContentArea(configView);
            UpdateEnabledState(SetupQuizMenuItem);
            }
        private void UpdateEnabledState(object sender)
            {
            string menuItemName = ((MenuItem)sender).Name;
            switch (menuItemName)
                {
                case "SetupQuizMenuItem":
                    SetupQuizMenuItem.IsEnabled = false;
                    PlayMenuItem.IsEnabled = true;
                    mainWindow.Title = "Quizler - Setup Quiz";
                    break;
                case "PlayMenuItem":
                    PlayMenuItem.IsEnabled = false;
                    SetupQuizMenuItem.IsEnabled = true;
                    mainWindow.Title = "Quizler - Play Quiz";
                    break;
                };
            }

    }
}
