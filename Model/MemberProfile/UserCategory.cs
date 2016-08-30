using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class UserCategory : EntityBase<int>
    {

        public int UserCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

       // public int LastUpdatedBy { get; set; }

       // public DateTime LastUpdatedOn { get; set; }


    }
}
