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
    public class Option
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        [HiddenInput] 
        public int OptionId { get; set; }

        [Required(ErrorMessage = "Option text is required")] 
        [StringLength(200, ErrorMessage = "Option text cannot exceed 200 characters")] 
        [Display(Name = "Option Text")] 
        public string OptionText { get; set; }

        //[Required] 
        //[Display(Name = "Order Index")] 
        //public int OrderIndex { get; set; }


        [Required]
        [ValidateNever]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        [ValidateNever]
        public Question Question { get; set; }


        [ValidateNever]
        public virtual ICollection<Answer> Answers { get; set; } 
    }
}
