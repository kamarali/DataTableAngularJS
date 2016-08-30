using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SIS.Web.UIModels.Account{
    public partial class FogotPassword{
        [Required(ErrorMessage = "User Name Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Security Question Required")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Secuirty Answer Required")]
        public string Answer { get; set; }
    }
}