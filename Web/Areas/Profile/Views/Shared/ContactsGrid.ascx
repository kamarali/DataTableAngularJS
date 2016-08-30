<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%
  var contactsDataTable = ViewData["ContactsData"] as DataTable;
        
%>
<%
  if ((contactsDataTable != null) && (contactsDataTable.Rows != null) && (contactsDataTable.Rows.Count >= 1))
  { %>
<table>
    <thead>
        <tr>
            <td>
                <b>Contact Name </b>
            </td>
            <%
                

        foreach (DataColumn column in contactsDataTable.Columns)
        {
          if (column.ColumnName == "CONTACT_ID" || column.ColumnName == "ContactName")
            continue;
      %>
      <td>
        <b>
          <%:column.ColumnName.Replace("'", "")%>
        </b>
      </td>
      <%
        }
                
      %>
    </tr>
  </thead>
  <tbody>
    <%
        
      foreach (DataRow dataRow in contactsDataTable.Rows)
      {
    %>
    <tr>
      <td>
        <%:dataRow["ContactName"].ToString() %>
      </td>
      <%
        {
          foreach (DataColumn column in contactsDataTable.Columns)
          {
            if (column.ColumnName == "CONTACT_ID" || column.ColumnName == "ContactName")
              continue;
      %>
      <td>
        <input iscontact="true" type="checkbox" id="<%:dataRow["CONTACT_ID"].ToString()%>_<%:column.ColumnName.Replace("'","")%>"
          <%if (dataRow[column.ColumnName].ToString() == "1")
{%>checked="checked" <%
}%> />
      </td>
      <%
}
              
      %>
      <td>
      </td>
    </tr>
    <%
      }
            }
    %></tbody>
</table>
<%} %>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Save" onclick="saveContactAssignment('<%:Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile"}) %>');" />
</div>
