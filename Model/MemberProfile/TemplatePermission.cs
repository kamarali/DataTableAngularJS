using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
    public class TemplatePermission : EntityBase<int>
    {
       public int TemplatePermissionId { get; set; }
       
       public int TemplateId { get; set; }
       
       public int PermissionId { get; set; }

       public Permission Permission { get; set; }

       public Template Template { get; set; }

       
        public string TemplateName
        {
            get { return this.Template.TemplateName; }

        }

        public string UserCateroryName
        {

            get { return this.Template.UserCategory.CategoryName; }
        }
    }
}
