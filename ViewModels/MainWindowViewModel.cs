using Labb3.Command;
using Labb3.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Labb3.ViewModels
    {
    class MainWindowViewModel : BaseViewModel
        {
        private const string SettingsFileName = "lastpath.txt"; // File to store the last used file path, used localpath before but think ahead and keep it simple and crossplatform:)

        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

        private QuestionPackViewModel _activePack;
        private object _currentView;

        public object CurrentView
            {
            get => _currentView;
            set { _currentView = value; RaisePropertyChanged(); }
            }

        public ICommand ShowConfigViewCommand { get; }
        public ICommand ShowPlayerViewCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SavePacksCommand { get; }
        public ICommand LoadPacksCommand { get; }
        public ICommand ToggleFullscreenCommand { get; }
       
        public QuestionPackViewModel ActivePack
            {
            get => _activePack;
            set
                {
                _activePack = value;
                RaisePropertyChanged();

                PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
                }
            }

        public PlayerViewModel? PlayerViewModel { get; }
        public ConfigurationViewModel? ConfigurationViewModel { get; }

        public MainWindowViewModel()
            {
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);

            ShowConfigViewCommand = new DelegateCommand(_ => CurrentView = ConfigurationViewModel);
            ShowPlayerViewCommand = new DelegateCommand(_ =>
            {
                SetRandomPack();
                CurrentView = PlayerViewModel;
            });
            ExitCommand = new DelegateCommand(_ => Application.Current.Shutdown());
            SavePacksCommand = new DelegateCommand(_ => SavePacks());
            LoadPacksCommand = new DelegateCommand(_ => LoadPacks());
            ToggleFullscreenCommand = new DelegateCommand(_ => ToggleFullscreen());
            TryLoadLastFile();

            CurrentView = ConfigurationViewModel;
            }

        private void TryLoadLastFile()
            {
            try
                {
                if (File.Exists(SettingsFileName))
                    {
                    var lastPath = File.ReadAllText(SettingsFileName);

                    if (!string.IsNullOrEmpty(lastPath) && File.Exists(lastPath))
                        {
                        LoadPacksFromFile(lastPath);
                        }
                    }
                }
            catch
                {
                // The winds of fate have blown us off course; we shall not load the last file.
                }
            }

        private void SaveLastFilePath(string path)
            {
            try
                {
                File.WriteAllText(SettingsFileName, path);
                }
            catch
                {

                }
            }

        private void SavePacks()
            {
            try
                {
                var saveDialog = new SaveFileDialog
                    {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = "questionpacks.json"
                    };

                if (saveDialog.ShowDialog() != true)
                    return;

                var packsData = new List<QuestionPackData>();

                foreach (var packVM in Packs)
                    {
                    var packData = new QuestionPackData
                        {
                        Name = packVM.Name,
                        Difficulty = packVM.Difficulty.ToString(),
                        TimeLimitInSeconds = packVM.TimeLimitInSeconds,
                        Questions = new List<QuestionData>()
                        };

                    foreach (var question in packVM.Questions)
                        {
                        packData.Questions.Add(new QuestionData
                            {
                            Query = question.Query,
                            Answer = question.Answer,
                            WrongAnswers = question.WrongAnswers
                            });
                        }

                    packsData.Add(packData);
                    }

                var json = JsonSerializer.Serialize(packsData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveDialog.FileName, json);

                SaveLastFilePath(saveDialog.FileName);

                MessageBox.Show("Question packs saved successfully!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error saving packs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void LoadPacks()
            {
            try
                {
                var openDialog = new OpenFileDialog
                    {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json"
                    };

                if (openDialog.ShowDialog() != true)
                    return;

                LoadPacksFromFile(openDialog.FileName);
                SaveLastFilePath(openDialog.FileName);

                MessageBox.Show("Question packs loaded successfully!", "Load", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error loading packs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void LoadPacksFromFile(string filePath)
            {
            var json = File.ReadAllText(filePath);
            var packsData = JsonSerializer.Deserialize<List<QuestionPackData>>(json);

            if (packsData == null)
                throw new Exception("Failed to deserialize question packs.");

            Packs.Clear();

            foreach (var packData in packsData)
                {
                var difficulty = Enum.Parse<Difficulty>(packData.Difficulty);
                var pack = new QuestionPack(packData.Name, difficulty, packData.TimeLimitInSeconds);

                foreach (var questionData in packData.Questions)
                    {
                    pack.Questions.Add(new Question(
                        questionData.Query,
                        questionData.Answer,
                        questionData.WrongAnswers[0],
                        questionData.WrongAnswers[1],
                        questionData.WrongAnswers[2]
                    ));
                    }

                Packs.Add(new QuestionPackViewModel(pack));
                }

            if (Packs.Count > 0)
                ActivePack = Packs[0];
            }

        private void SetRandomPack()
            {
            if (Packs.Count > 0)
                {
                var randomIndex = Random.Shared.Next(Packs.Count);
                ActivePack = Packs[randomIndex];
                }
            else
                {
                var pack = new QuestionPack("Simple Questions");
                var packViewModel = new QuestionPackViewModel(pack);

                packViewModel.Questions.Add(new Question("Whats 1*0?", "0", "1", "-1", "5"));
                packViewModel.Questions.Add(new Question("What is the capital of Sweden?", "Stockholm", "Malmö", "Kiruna", "Göteborg"));

                Packs.Add(packViewModel);
                ActivePack = packViewModel;
                }
            }
        private void ToggleFullscreen()
            {
            var window = Application.Current.MainWindow;
            if (window != null)
                {
                if (window.WindowState == WindowState.Maximized && window.WindowStyle == WindowStyle.None)
                    {
                    window.WindowState = WindowState.Normal;
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    }
                else
                    {
                    window.WindowState = WindowState.Maximized;
                    window.WindowStyle = WindowStyle.None;
                    }
                }
            }
        }

    class QuestionPackData
        {
        public string Name { get; set; }
        public string Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public List<QuestionData> Questions { get; set; }
        }

    class QuestionData
        {
        public string Query { get; set; }
        public string Answer { get; set; }
        public string[] WrongAnswers { get; set; }
        }
    }