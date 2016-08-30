using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile; 
namespace Iata.IS.Data.MemberProfile.Impl
{
    public class TemplatePermissionRepository : Repository<Template>, ITemplatePermissionRepository
    {


        public IQueryable<Template> Get(Permission permission)
        {
            var templatePermissionsList = EntityObjectSet.Include("UserCategory").Where(t => t.SystemDefined == 0);
            //.Where(t=>t.TemplateName == permission.TemplateName || t.UserCategoryId  == permission.UserCategoryId );

            if (!string.IsNullOrEmpty(permission.TemplateName))
            {
                templatePermissionsList = templatePermissionsList.Where(t => t.TemplateName.Contains(permission.TemplateName)) as ObjectQuery<Template>;
            }

            if (permission.UserCategoryId > 0)
            {
                if (templatePermissionsList != null)
                    templatePermissionsList = templatePermissionsList.Where(t => t.UserCategoryId == permission.UserCategoryId) as ObjectQuery<Template>;
            }

            if (permission.MemeberID > 0)
            {
                if (templatePermissionsList != null)
                    templatePermissionsList = templatePermissionsList.Where(t => t.MemberId == permission.MemeberID) as ObjectQuery<Template>;
            }

            return templatePermissionsList;

        }

        public IQueryable<Template> GetTemplateNameByUserCategoryId(int userCategoryId, int memberId, int CurrentUserCategory)
        {
            IQueryable<Template> templateList;

            if (userCategoryId == 1 || userCategoryId == 2 || userCategoryId == 3)
            {
                if (CurrentUserCategory == userCategoryId)
                {
                    templateList =
                       EntityObjectSet.Include("UserCategory").Where(
                           t => t.UserCategoryId == userCategoryId && t.UserType == 0)
                       as ObjectQuery<Template>;
                }
                else
                {

                    templateList =
                        EntityObjectSet.Include("UserCategory").Where(
                            t => t.UserCategoryId == userCategoryId && t.UserType == 0 && t.SystemDefined == 1)
                        as ObjectQuery<Template>;
                }
            }
            else
            {
                templateList =
                    EntityObjectSet.Include("UserCategory").Where(
                        t =>
                        t.UserCategoryId == userCategoryId && t.UserType == 0 &&
                        (t.MemberId == memberId || t.SystemDefined == 1))
                    as ObjectQuery<Template>;
            }

            return templateList;

        }
    }
}
