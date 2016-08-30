using System;
using System.ComponentModel.DataAnnotations;

namespace SIS.Web.UIModels.Account
{
    public class CreateUserView
    {
        public int Salutation { get; set; }
  
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Department { get; set; }
        
        public string PositionTitle { get; set; }
        
        public string StaffID { get; set; }

        
        public string LocationID { get; set; }

        public string CountryName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }
      
        public string Address3 { get; set; }

        public string CityName { get; set; }

        public string SubDivisionName { get; set; }
      
        public string PostalCode { get; set; }

        public string Divison { get; set; }

        public string Telephone1 { get; set; }
  
        public string Telephone2 { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
      
      
        public string EmailAddress { get; set; }

        public string HiddenEmailAddress { get; set; }

        
        public string SITAAddress { get; set; }

        
        public string SubDivisionData { get; set; }
        public string CityData { get; set; }
        
      
        public string CountryData { get; set; }

        public string CountryCode { get; set; }

        public string SubDivisionCode { get; set; }

        
        public string UserCategory { get; set; }

        /// <summary>
        /// Selected User Category
        /// </summary>
        public string UserCategoryData { get; set; }

        public string UserLanguageCode { get; set; }

        /// <summary>
        /// Selected User Language
        /// </summary>
        public string UserLanguageData { get; set; }


        /// <summary>
        /// Name of drop list for member list 
        /// </summary>
        
        public string MembersList { get; set; }

        public string MemberName { get; set; }

        public int MemberId { get; set; }
        /// <summary>
        /// Select Member
        /// </summary>
        public string MemberData { get; set; }


      /// <summary>
      /// Defined User Type
      /// </summary>
        public bool UserType { get; set; }

        public bool IsSuperUserCreation { get; set; }

        // CMP#668: Archival of IS-WEB Users and Removal from Screens
        /// <summary>
        /// This boolean property is used to define is user archived or not
        /// </summary>
        public bool IsArchived { get; set; }
    }
}
