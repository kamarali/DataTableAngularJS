namespace Iata.IS.Model.MemberProfile
{
   public class TreeViewPermissions  
   {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; }

        public int PermissionLevel { get; set; }

        public int Is_Permission_Assigned { get; set; }

        public int ParentPermissionId { get; set; }

        public bool IsLeafNode { get; set; }

        public int UserCategoryId { get; set; }

        //public decimal RecordCount { get; set; }

      
   }
}
