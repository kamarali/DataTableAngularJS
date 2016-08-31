namespace Iata.IS.Business.Security.Permissions
{
  public struct Profile
  {

    public const int CreateOrManageMemberAccess = 551810;

    public const int ManageMemberAccess = 552310;

    public const int CreateOrManageUsersAccess = 551910;

    public const int ManageUserPermissionAccess = 552510;

    public const int ViewProfileChangesAccess = 553910;

    public const int ViewIchProfileChangesAccess = 553810;

    public const int ViewAchProfileChangesAccess = 553710;

    public const int ContactAdminAccess = 551510;

    public const int ProxyLoginAccess = 552910;

    //CMP#655(2.1.1)IS-WEB Display per Location
    public const int LocationAssociationAccess = 2223201;
  }
}
