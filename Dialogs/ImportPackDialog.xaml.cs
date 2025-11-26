using System.Windows;
using Labb3.ViewModels;

namespace Labb3.Dialogs
    {
    internal partial class ImportPackDialog : Window
        {
        internal ImportPackDialog(MainWindowViewModel mainWindowViewModel)
            {
            InitializeComponent();
            DataContext = new ImportPackDialogViewModel(mainWindowViewModel, this);
            }
        }
    }