using Labb3.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Labb3.ViewModels;

namespace Labb3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        public MainWindow()
            {
            InitializeComponent();
            var menuView = new Views.MenuView();
            mainMenuPanel.Children.Add(menuView);
            var configView = new Views.ConfigurationView();
            mainContentArea.Children.Add(configView);
            
            var pack = new QuestionPack("MyQuestionPack");

            DataContext = new QuestionPackViewModel(pack);
            }

        public void ClearContentArea()
            {
            mainContentArea.Children.Clear();
            }
        public void SetContentArea(UserControl newContent)
            {
            mainContentArea.Children.Add(newContent);
            }
        }
}