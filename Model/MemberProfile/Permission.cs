using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
    public class Permission:EntityBase<int>
    {

      
        public int PermissionId { get; set; }

        public string PermissionName { get; set; }

        public string PermissionDescription { get; set; }

        public int UserCategoryId { get; set; }

        public int ParentPermissionId { get; set; }

        public string SelectedIDs { get; set; }

        public string TemplateName { get; set; }

        public int TemplateID { get; set; }

        public UserCategory UserCategory { get; set; }

        public int Is_Permission_Assigned { get; set; }

        public int? MemeberID { get; set; }
       
    }
}
