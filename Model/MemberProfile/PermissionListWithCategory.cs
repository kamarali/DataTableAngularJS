namespace Iata.IS.Model.MemberProfile
{
  public  class PermissionListWithCategory
  {
    public int PermissionId { get; set; }

    public string PermissionName { get; set; }

    public int ParentPermissionId { get; set; }

    public int UserCategoryId { get; set; }

    public string UserName { get; set; }
    
    
  }
}
