using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.ViewModels
{
    public class QuestionVM
    {
        public Question Question { get; set; }
        public List<Option> Options { get; set; } = new List<Option>();
        public int SurveyId { get; set; }

        public string? SearchString { get; set; }
    }
}
