using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.Models
{
    class Question
    {
        public Question(string query, string answer, string wrongAnswer1, string wrongAnswer2, string wrongAnswer3)
        {
            Query = query;
            Answer = answer;
            WrongAnswers = [wrongAnswer1, wrongAnswer2, wrongAnswer3];
        }

        public string Query { get; set; }
        public string Answer { get; set; }
        public string[] WrongAnswers { get; set; }

        }
}
