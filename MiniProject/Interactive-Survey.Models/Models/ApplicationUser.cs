using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        [ValidateNever]
        public Creator Creator { get; set; }

        [ValidateNever]
        public Respondent Respondent { get; set; }
    }
}
