using Labb3.Command;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Labb3.ViewModels
    {
    class PlayerViewModel : BaseViewModel
        {
        private MainWindowViewModel mainWindowViewModel;

        private DispatcherTimer questionTimer;
        private DispatcherTimer transitionTimer;

        private List<Question> shuffledQuestions;
        private string[] currentAnswerOptions;
        private int currentQuestionIndex;
        private int correctAnswerIndex;
        private int score;
        private bool canAnswer;

        private int transitionCountdown;

        public PlayerViewModel(MainWindowViewModel mainWindowViewModel)
            {
            this.mainWindowViewModel = mainWindowViewModel;

            questionTimer = new DispatcherTimer();
            questionTimer.Interval = TimeSpan.FromSeconds(1);
            questionTimer.Tick += QuestionTimer_Tick;

            transitionTimer = new DispatcherTimer();
            transitionTimer.Interval = TimeSpan.FromSeconds(1);
            transitionTimer.Tick += TransitionTimer_Tick;

            AnswerCommand = new DelegateCommand(
                p => SubmitAnswer(p),
                p => canAnswer
            );

            StartQuizCommand = new DelegateCommand(
                p => StartQuiz(),
                p => ActivePack?.Questions.Count > 0
            );

            TimerDisplay = ActivePack?.TimeLimitInSeconds ?? 30;
            }

        public QuestionPackViewModel ActivePack => mainWindowViewModel?.ActivePack;

        private Question currentQuestion;
        public Question CurrentQuestion
            {
            get => currentQuestion;
            set
                {
                currentQuestion = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(QuestionText));
                }
            }

        public string QuestionText => CurrentQuestion?.Query ?? "Click 'Start Quiz' to begin!";
        public string Option1 => currentAnswerOptions?[0] ?? "";
        public string Option2 => currentAnswerOptions?[1] ?? "";
        public string Option3 => currentAnswerOptions?[2] ?? "";
        public string Option4 => currentAnswerOptions?[3] ?? "";

        private string feedbackText = "";
        public string FeedbackText
            {
            get => feedbackText;
            set { feedbackText = value; RaisePropertyChanged(); }
            }

        private string feedbackColor = "Green";
        public string FeedbackColor
            {
            get => feedbackColor;
            set { feedbackColor = value; RaisePropertyChanged(); }
            }

        private int timerDisplay;
        public int TimerDisplay
            {
            get => timerDisplay;
            set { timerDisplay = value; RaisePropertyChanged(); }
            }

        private string timerLabel = "Time per question:";
        public string TimerLabel
            {
            get => timerLabel;
            set { timerLabel = value; RaisePropertyChanged(); }
            }

        public string QuizStatus => shuffledQuestions != null
            ? $"Question {currentQuestionIndex + 1} of {shuffledQuestions.Count}"
            : "";

        private bool isQuizActive;
        public bool IsQuizActive
            {
            get => isQuizActive;
            set
                {
                isQuizActive = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(QuizStatus));
                RaisePropertyChanged(nameof(FeedbackButtonVisible));
                }
            }

        public bool FeedbackButtonVisible => !isQuizActive;

        public DelegateCommand AnswerCommand { get; }
        public DelegateCommand StartQuizCommand { get; }

        // QUIZ START
        private void StartQuiz()
            {
            if (ActivePack == null || ActivePack.Questions.Count == 0)
                return;

            shuffledQuestions = ActivePack.Questions.OrderBy(q => Random.Shared.Next()).ToList();
            currentQuestionIndex = 0;
            score = 0;
            IsQuizActive = true;

            ShowNextQuestion();
            }

        private void ShowNextQuestion()
            {
            if (currentQuestionIndex >= shuffledQuestions.Count)
                {
                EndQuiz();
                return;
                }

            CurrentQuestion = shuffledQuestions[currentQuestionIndex];
            PrepareAnswerOptions();

            canAnswer = true;
            AnswerCommand.RaiseCanExecuteChanged();

            FeedbackText = "";

            TimerLabel = "Time remaining:";
            TimerDisplay = ActivePack?.TimeLimitInSeconds ?? 30;

            questionTimer.Start();
            }

        private void PrepareAnswerOptions()
            {
            var allAnswers = new List<string> { CurrentQuestion.Answer };
            allAnswers.AddRange(CurrentQuestion.WrongAnswers);
            currentAnswerOptions = allAnswers.OrderBy(a => Random.Shared.Next()).ToArray();
            correctAnswerIndex = Array.IndexOf(currentAnswerOptions, CurrentQuestion.Answer);

            RaisePropertyChanged(nameof(Option1));
            RaisePropertyChanged(nameof(Option2));
            RaisePropertyChanged(nameof(Option3));
            RaisePropertyChanged(nameof(Option4));
            }

        private void QuestionTimer_Tick(object sender, EventArgs e)
            {
            TimerDisplay--;

            if (TimerDisplay <= 0)
                {
                questionTimer.Stop();
                SubmitAnswer(null);
                }
            }

        private void TransitionTimer_Tick(object sender, EventArgs e)
            {
            transitionCountdown--;
            TimerDisplay = transitionCountdown;

            if (transitionCountdown <= 0)
                {
                transitionTimer.Stop();
                currentQuestionIndex++;
                RaisePropertyChanged(nameof(QuizStatus));
                ShowNextQuestion();
                }
            }

        private void SubmitAnswer(object parameter)
            {
            if (!canAnswer)
                return;

            canAnswer = false;
            AnswerCommand.RaiseCanExecuteChanged();

            questionTimer.Stop();

            int selectedIndex = parameter != null ? int.Parse(parameter.ToString()) : -1;

            if (selectedIndex == correctAnswerIndex)
                {
                score++;
                FeedbackText = "✓ Correct!";
                FeedbackColor = "Green";
                }
            else
                {
                FeedbackText = $"✗ Wrong! Correct answer: {CurrentQuestion?.Answer}";
                FeedbackColor = "Red";
                }

            if (currentQuestionIndex >= shuffledQuestions.Count - 1)
                {
                var finalDelay = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                finalDelay.Tick += (s, e) =>
                {
                    finalDelay.Stop();
                    EndQuiz();
                };
                finalDelay.Start();
                }
            else
                {
                TimerLabel = "Next question in:";
                transitionCountdown = 3;
                TimerDisplay = 3;

                transitionTimer.Start();
                }
            }

        // QUIZ END
        private void EndQuiz()
            {
            IsQuizActive = false;
            questionTimer.Stop();
            transitionTimer.Stop();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBox.Show(
                    $"Quiz Complete!\n\nYour score: {score} / {shuffledQuestions.Count}",
                    "Results",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }));

            CurrentQuestion = null;
            currentAnswerOptions = null;
            FeedbackText = "";
            TimerLabel = "Time per question:";
            TimerDisplay = ActivePack?.TimeLimitInSeconds ?? 30;
            }
        }
    }
