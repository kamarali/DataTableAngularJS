namespace Iata.IS.Model.MemberProfile
{
    public class PermissionToUserList
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; }

        public int ParentPermissionId { get; set; }
        
        public int UserId { get; set; }
        

    }
}
