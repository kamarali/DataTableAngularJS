using System;

namespace Iata.IS.Model.MemberProfile
{
    public class ISLoginUser
    {
        public int IS_USER_ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int UserCategoryId { get; set; }

        public string MembercommercialName { get; set; }

        public int IsUserActive { get; set; }

        public int IsUserLocked { get; set; }

        public int MemberId { get; set; }

        public int IsMemberStatusId { get; set; }

        //CMP-665-User Related Enhancements-FRS-v1.2.doc
        //[Sec 2.8 Conditional Redirection of Users upon Login in IS-WEB]
        //Add New Property as IsMemberSubStatusId
        public int IsMemberSubStatusId { get; set; }

        public int LoginStutusId { get; set; }

        public int ActiveSessionCount { get; set; }

        //CMP685
        public int ActiveSessionCountOfUser { get; set; }

        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime PasswordExpirationDateTime { get; set; }

        public bool PasswordIsExpired { get; set; }

        public string LanguageCode { get; set; }
    }
}
