using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.ViewModels
{
    public class RespondentSurveyVM
    {
        public Survey Survey { get; set; }

        public List<QuestionRowVM> Questions { get; set; } = new List<QuestionRowVM>();

        public Dictionary<int, int> Answers { get; set; } = new Dictionary<int, int>();
    }

}
