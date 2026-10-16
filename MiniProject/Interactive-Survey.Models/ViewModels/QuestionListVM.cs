using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.ViewModels
{
    public class QuestionRowVM
    {
        public Question Question { get; set; }
        public IEnumerable<Option> Options { get; set; } = Enumerable.Empty<Option>();
    }

    public class QuestionListVM
    {
        public IEnumerable<QuestionRowVM> Questions { get; set; }
        public int SurveyId { get; set; }

        public string? SearchString { get; set; }
    }
}
