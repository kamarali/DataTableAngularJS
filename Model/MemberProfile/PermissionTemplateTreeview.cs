using System.Collections.Generic;

namespace Iata.IS.Model.MemberProfile
{
    public class PermissionTemplateTreeview
    {
        public string data;
        public TreeviewAttribute attributes;
        public List<PermissionTemplateTreeview> children;
        public string UserCategory { get; set; }
        public string CategoryId { get; set; }
        public string SelectedIds { get; set; }
    }

    public class TreeviewAttribute
    {
        public string id;
        public bool selected;
    }

 

    


 








}
