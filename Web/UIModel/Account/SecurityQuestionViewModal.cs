using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SIS.Web.UIModels.Account
{
    public class SecurityQuestionViewModal{
       
        [Required]
        public string Question1 { get; set; }
        [Required]
        public string Question2 { get; set; }
        [Required]
        public string Question3 { get; set; }
        [Required]
        public string Answer1 { get; set; }
        [Required]
        public string Answer2 { get; set; }
        [Required]
        public string Answer3 { get; set; }
    }
}