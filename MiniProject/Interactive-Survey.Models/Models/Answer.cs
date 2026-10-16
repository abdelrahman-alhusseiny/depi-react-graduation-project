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
    public class Answer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        [Display(Name = "Answer ID")] 
        public int AnswerId { get; set; }


        [Required]
        [ValidateNever]
        public int OptionId { get; set; }

        [ForeignKey("OptionId")]
        [ValidateNever]
        public Option Option { get; set; }


        [Required]
        [ValidateNever]
        public int RespondentId { get; set; }

        [ForeignKey("RespondentId")]
        [ValidateNever]
        public Respondent Respondent { get; set; }
    }
}
