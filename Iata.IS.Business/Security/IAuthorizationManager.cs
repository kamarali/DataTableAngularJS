namespace Iata.IS.Business.Security
{
  public interface IAuthorizationManager
  {
    bool IsAuthorized(int userId, int permissionId);
  }
}