using Labb3.Command;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;

namespace Labb3.ViewModels
    {
    class ImportPackDialogViewModel : BaseViewModel
        {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly Window dialog;
        private readonly HttpClient httpClient;

        private int numberOfQuestions = 10;
        private TriviaCategory selectedCategory;
        private string selectedDifficulty = "Any";
        private string selectedQuestionType = "Multiple Choice";
        private string statusMessage = "";
        private string statusColor = "Black";
        private string packName = "Imported Pack";

        public ImportPackDialogViewModel(MainWindowViewModel mainWindowViewModel, Window dialog)
            {
            this.mainWindowViewModel = mainWindowViewModel;
            this.dialog = dialog;
            this.httpClient = new HttpClient();

            ImportCommand = new DelegateCommand(async _ => await ImportQuestions());

            LoadCategories();
            }

        public ObservableCollection<TriviaCategory> Categories { get; set; } = new();
        public ObservableCollection<string> Difficulties { get; set; } = new() { "Any", "Easy", "Medium", "Hard" };
        public ObservableCollection<string> QuestionTypes { get; set; } = new() { "Any", "Multiple Choice", "True/False" };

        public int NumberOfQuestions
            {
            get => numberOfQuestions;
            set
                {
                numberOfQuestions = value;
                RaisePropertyChanged();
                }
            }

        public string PackName
            {
            get => packName;
            set
                {
                packName = value;
                RaisePropertyChanged();
                }
            }

        public TriviaCategory SelectedCategory
            {
            get => selectedCategory;
            set
                {
                selectedCategory = value;
                RaisePropertyChanged();

                if (selectedCategory != null && selectedCategory.id > 0)
                    {
                    PackName = $"Imported: {selectedCategory.name}";
                    }
                else
                    {
                    PackName = "Imported Pack";
                    }
                }
            }

        public string SelectedDifficulty
            {
            get => selectedDifficulty;
            set
                {
                selectedDifficulty = value;
                RaisePropertyChanged();
                }
            }

        public string SelectedQuestionType
            {
            get => selectedQuestionType;
            set
                {
                selectedQuestionType = value;
                RaisePropertyChanged();
                }
            }

        public string StatusMessage
            {
            get => statusMessage;
            set
                {
                statusMessage = value;
                RaisePropertyChanged();
                }
            }

        public string StatusColor
            {
            get => statusColor;
            set
                {
                statusColor = value;
                RaisePropertyChanged();
                }
            }

        public ICommand ImportCommand { get; }

        private async void LoadCategories()
            {
            try
                {
                Categories.Add(new TriviaCategory { id = 0, name = "Any Category" });

                var response = await httpClient.GetStringAsync("https://opentdb.com/api_category.php");
                var categoryData = JsonSerializer.Deserialize<TriviaCategoryResponse>(response);

                if (categoryData?.trivia_categories != null)
                    {
                    foreach (var category in categoryData.trivia_categories)
                        {
                        Categories.Add(category);
                        }
                    }

                SelectedCategory = Categories[0];
                }
            catch (Exception ex)
                {
                StatusMessage = $"Failed to load categories: {ex.Message}";
                StatusColor = "Red";
                }
            }

        private async Task ImportQuestions()
            {
            try
                {
                StatusMessage = "Importing questions...";
                StatusColor = "Blue";

                var url = BuildApiUrl();
                var response = await httpClient.GetStringAsync(url);
                var triviaResponse = JsonSerializer.Deserialize<TriviaApiResponse>(response);

                if (triviaResponse?.response_code != 0)
                    {
                    StatusMessage = "Failed to retrieve questions. Try different parameters.";
                    StatusColor = "Red";
                    return;
                    }

                if (triviaResponse.results == null || triviaResponse.results.Count == 0)
                    {
                    StatusMessage = "No questions returned from API.";
                    StatusColor = "Red";
                    return;
                    }

                var pack = new QuestionPack(PackName);

                var difficultyMap = new Dictionary<string, Difficulty>
                {
                    { "easy", Difficulty.Easy },
                    { "medium", Difficulty.Medium },
                    { "hard", Difficulty.Hard }
                };

                foreach (var result in triviaResponse.results)
                    {
                    if (result.incorrect_answers == null || result.incorrect_answers.Count < 3)
                        {
                        if (result.incorrect_answers == null || result.incorrect_answers.Count == 0)
                            continue;

                        while (result.incorrect_answers.Count < 3)
                            {
                            result.incorrect_answers.Add($"Wrong Answer {result.incorrect_answers.Count + 1}");
                            }
                        }

                    var question = new Question(
                        HttpUtility.HtmlDecode(result.question),
                        HttpUtility.HtmlDecode(result.correct_answer),
                        HttpUtility.HtmlDecode(result.incorrect_answers[0]),
                        HttpUtility.HtmlDecode(result.incorrect_answers[1]),
                        HttpUtility.HtmlDecode(result.incorrect_answers[2])
                    );

                    pack.Questions.Add(question);

                    if (difficultyMap.ContainsKey(result.difficulty.ToLower()))
                        {
                        pack.Difficulty = difficultyMap[result.difficulty.ToLower()];
                        }
                    }

                mainWindowViewModel.Packs.Add(new QuestionPackViewModel(pack));

                StatusMessage = $"Successfully imported {pack.Questions.Count} questions!";
                StatusColor = "Green";

                await Task.Delay(1500);
                dialog.DialogResult = true;
                dialog.Close();
                }
            catch (Exception ex)
                {
                StatusMessage = $"Error: {ex.Message}";
                StatusColor = "Red";
                }
            }

        private string BuildApiUrl()
            {
            var url = $"https://opentdb.com/api.php?amount={NumberOfQuestions}";

            if (SelectedCategory?.id > 0)
                url += $"&category={SelectedCategory.id}";

            if (SelectedDifficulty != "Any")
                url += $"&difficulty={SelectedDifficulty.ToLower()}";

            url += "&type=multiple";

            return url;
            }
        }

    class TriviaCategory
        {
        public int id { get; set; }
        public string name { get; set; }
        }

    class TriviaCategoryResponse
        {
        public List<TriviaCategory> trivia_categories { get; set; }
        }

    class TriviaApiResponse
        {
        public int response_code { get; set; }
        public List<TriviaResult> results { get; set; }
        }

    class TriviaResult
        {
        public string category { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public List<string> incorrect_answers { get; set; }
        }
    }