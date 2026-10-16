using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.ViewModels
{
    public class SurveyVM
    {
        public Survey Survey { get; set; }
        public string? SourceView { get; set; }


        public string? SearchString { get; set; }
        public string? CurrentSort { get; set; }
        public int? CurrentPage { get; set; }
    }
}
