using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
    public class PermissionToUser : EntityBase<int>
       
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; }

        public int ParentPermissionId { get; set; }

       public string UserName { get; set; }
       
       public int UserId { get; set; }
       
       public int TemplateId { get; set; }

       public Permission Permission { get; set; }

       public string SelectedIDs { get; set; }

       public string CopyUserName { get; set; }

       public int CopyUserId { get; set; }

       public int UserCategoryId { get; set; }


       
    }
}
