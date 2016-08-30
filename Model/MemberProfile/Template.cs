using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
    public  class Template:EntityBase<int>
    {

        public int TemplateId { get; set; }

        public string TemplateName { get; set; }

        public int UserCategoryId { get; set; }

        public int? MemberId { get; set; }

        public int SystemDefined { get; set; }

        public Member Member { get; set; }

        public UserCategory UserCategory { get; set; }

        public string CategoryName
        {
            get
            {
              return UserCategory.CategoryName == null? string.Empty :this.UserCategory.CategoryName ;
            }

        }

        public int UserType { get; set; }

        //public DateTime lastUpdatedOn { get; set; }

               

    }
}
