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
    public class Survey
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        [HiddenInput] 
        public int SurveyId { get; set; }

        [Required(ErrorMessage = "Survey title is required")] 
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")] 
        [Display(Name = "Survey Title")] 
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")] 
        [Display(Name = "Survey Description")] 
        public string Description { get; set; }

        [Required]
        [ValidateNever]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        [Required]
        [ValidateNever]
        public int CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        [ValidateNever]
        public Creator Creator { get; set; }



        [ValidateNever]
        public ICollection<Question> Questions { get; set; }
    }
}
