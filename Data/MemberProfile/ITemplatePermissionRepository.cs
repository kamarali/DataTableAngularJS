using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
    public  interface ITemplatePermissionRepository
    {

        IQueryable<Template> Get(Permission permission);
      IQueryable<Template> GetTemplateNameByUserCategoryId(int userCategoryId, int memberId, int CurrentUserCategory);
    }
}
