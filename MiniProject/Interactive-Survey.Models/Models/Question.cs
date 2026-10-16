using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.Models
{
    public class Question
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        [HiddenInput] 
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Question text is required")] 
        [StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters")] 
        [Display(Name = "Question Text")] 
        public string QuestionText { get; set; }

        //[Required]
        //[Display(Name = "Order Index")]
        //public int OrderIndex { get; set; }



        [Required]
        [ValidateNever]
        public int SurveyId { get; set; }

        [ForeignKey("SurveyId")]
        [ValidateNever]
        public Survey Survey { get; set; }



        [ValidateNever]
        public ICollection<Option> Options { get; set; }
    }
}
