using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SIS.Web.UIModels.Account{
    public class CountryListItem{
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}