using Labb3.Command;
using Labb3.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
/*
 * This ViewModel manages the configuration of question packs.
 * It allows users to create, play, delete, and modify question packs and their questions.
 * I decided to bake in dialogs here for simplicity, rather than creating separate ViewModels for each dialog.
 * Again, code is a bit rough around the edges but works well enough for the purpose of this application. Time constraints were a factor.
 */
namespace Labb3.ViewModels
    {
    class ConfigurationViewModel : BaseViewModel
        {
        private readonly MainWindowViewModel? mainWindowViewModel;
        private QuestionPackViewModel? _selectedPack;
        private Question? _selectedQuestion;

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
            {
            this.mainWindowViewModel = mainWindowViewModel;

            NewPackCommand = new DelegateCommand(_ => CreateNewPack());
            DeletePackCommand = new DelegateCommand(_ => DeletePack(), _ => SelectedPack != null);
            AddQuestionCommand = new DelegateCommand(_ => AddQuestion(), _ => SelectedPack != null);
            RemoveQuestionCommand = new DelegateCommand(_ => RemoveQuestion(), _ => SelectedQuestion != null);
            PlayPackCommand = new DelegateCommand(_ => PlayPack(), _ => SelectedPack != null);
            ImportPackCommand = new DelegateCommand(_ => ImportPack());
            }

        public ObservableCollection<QuestionPackViewModel> QuestionPacks =>
            mainWindowViewModel?.Packs ?? new ObservableCollection<QuestionPackViewModel>();

        public QuestionPackViewModel? SelectedPack
            {
            get => _selectedPack;
            set
                {
                _selectedPack = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(SelectedQuestion));

                ( DeletePackCommand as DelegateCommand )?.RaiseCanExecuteChanged();
                ( AddQuestionCommand as DelegateCommand )?.RaiseCanExecuteChanged();
                ( OpenPackOptionsCommand as DelegateCommand )?.RaiseCanExecuteChanged();
                ( PlayPackCommand as DelegateCommand )?.RaiseCanExecuteChanged(); 
                }
            }

        public Question? SelectedQuestion
            {
            get => _selectedQuestion;
            set
                {
                _selectedQuestion = value;
                RaisePropertyChanged();

                ( RemoveQuestionCommand as DelegateCommand )?.RaiseCanExecuteChanged();
                }
            }
        private void ImportPack()
            {
            var dialog = new Dialogs.ImportPackDialog(mainWindowViewModel)
                {
                Owner = Application.Current.MainWindow
                };
            dialog.ShowDialog();
            }

        public ICommand NewPackCommand { get; }
        public ICommand DeletePackCommand { get; }
        public ICommand PlayPackCommand { get; }
        public ICommand AddQuestionCommand { get; }
        public ICommand RemoveQuestionCommand { get; }
        public ICommand OpenPackOptionsCommand { get; }
        public ICommand ImportPackCommand { get; }

        public ObservableCollection<string> DifficultyLevels { get; set; } =
            new ObservableCollection<string> { "Easy", "Medium", "Hard" };

        private void CreateNewPack()
            {
            var newPack = new QuestionPack("New Question Pack");
            var packViewModel = new QuestionPackViewModel(newPack);

            mainWindowViewModel?.Packs.Add(packViewModel);
            SelectedPack = packViewModel;
            }

        private void DeletePack()
            {
            if (SelectedPack != null && mainWindowViewModel != null)
                {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{SelectedPack.Name}'?",
                    "Delete Pack",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    {
                    mainWindowViewModel.Packs.Remove(SelectedPack);
                    SelectedPack = null;
                    }
                }
            }

        private void AddQuestion()
            {
            if (SelectedPack != null)
                {
                var newQuestion = new Question(
                    "New Question",
                    "Correct Answer",
                    "Wrong Answer 1",
                    "Wrong Answer 2",
                    "Wrong Answer 3");

                SelectedPack.Questions.Add(newQuestion);
                SelectedQuestion = newQuestion;
                }
            }

        private void RemoveQuestion()
            {
            if (SelectedQuestion != null && SelectedPack != null)
                {
                SelectedPack.Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                }
            }

        private void PlayPack()
            {
            if (SelectedPack != null && mainWindowViewModel != null)
                {
                mainWindowViewModel.ActivePack = SelectedPack;
                mainWindowViewModel.CurrentView = mainWindowViewModel.PlayerViewModel;
                }
            }
        }
    }
