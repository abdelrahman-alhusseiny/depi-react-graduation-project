using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.ViewModels
{
    public class OptionResultVM
    {
        public Option Option { get; set; }
        public int AnswersCount { get; set; }
    }
    public class QuestionResultVM
    {
        public Question Question { get; set; }
        public List<OptionResultVM> OptionsResults { get; set; } = new List<OptionResultVM>();
    }

    public class SurveyResultsVM
    {
        public Survey Survey { get; set; }
        public List<QuestionResultVM> QuestionsResults { get; set; } = new List<QuestionResultVM>();
    }
}
