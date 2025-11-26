using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Labb3.Models
    {
    class Question : INotifyPropertyChanged
        {
        private string query;
        private string answer;
        private string[] wrongAnswers;

        public Question(string query, string answer, string wrongAnswer1, string wrongAnswer2, string wrongAnswer3)
            {
            Query = query;
            Answer = answer;
            WrongAnswers = [wrongAnswer1, wrongAnswer2, wrongAnswer3];
            }

        public string Query
            {
            get => query;
            set
                {
                query = value;
                OnPropertyChanged();
                }
            }

        public string Answer
            {
            get => answer;
            set
                {
                answer = value;
                OnPropertyChanged();
                }
            }

        public string[] WrongAnswers
            {
            get => wrongAnswers;
            set
                {
                wrongAnswers = value;
                OnPropertyChanged();
                }
            }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }