<%@ Page Title="Cached Keys" Language="C#" Inherits="System.Web.Mvc.ViewPage<StringCollection>"
    MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
    <h1>
        Cached Items</h1>
    <%
        if (Model == null)
        {	
    %>
    <h2>
        Caching may not be configured...</h2>
    <%
        return;
    }
    else
    {
    %>
    <h2>
        Remove Cached Items</h2>
    <%
    }
    %>
    <table id="table-3">
        <tr>
            <th>
                Sr.No.
            </th>
            <th>
                Item
            </th>
            <th>
                View
            </th>
            <th>
                Remove
            </th>
        </tr>
        <% var i = 1;
           foreach (var key in Model)
           {
        %>
        <tr>
            <td>
                <%: i %>
            </td>
            <td>
                <%: string.Format("{0}", key)%>
            </td>
            <td>
               <a href="#" onclick="Javascript:ViewCacheDetail('<%: string.Format("{0}", key)%>')" > View </a>
              
            </td>
            <td>
                <%: Html.ActionLink(string.Format("Remove"), "RemoveCacheKey", new { key })%>
            </td>
        </tr>
        <%i++;
      } 
        %>
    </table>


    <div id="dialogCacheDetails">
      <div id="CacheObject" style="overflow:auto;" />
      
    </div>

</asp:Content>
<asp:Content runat="server" ID="Script" ContentPlaceHolderID="Script">

<script language="javascript" type="text/javascript">
    function ViewCacheDetail(Key) {

        $("#dialogCacheDetails").dialog({
            autoOpen: true,
            title: Key,
            height: 600,
            width: 700,
            modal: true,
            resizable: false
        });

        $("#CacheObject").text('');
        $.ajax({
            type: "POST",
            url: '<%:Url.Action("GetCacheObjectDetails", "Home", new { area = "" })%>',
            dataType: "json",
            data: { Key: Key },
            success: function (result) {
              $("#CacheObject").text(result);
            }
        });
    
     return false;

 }


</script>


    <style type="text/css">
        #table-3
        {
            border: 1px solid #DFDFDF;
            background-color: #F9F9F9;
            width: 30%;
            -moz-border-radius: 3px;
            -webkit-border-radius: 3px;
            font-family: Arial, "Bitstream Vera Sans" ,Helvetica,Verdana,sans-serif;
        }
        #table-3 td, #table-3 th
        {
            border-top-color: white;
            border-bottom: 1px solid #DFDFDF;
        }
        #table-3 th
        {
            font-family: Georgia, "Times New Roman" , "Bitstream Charter" ,Times,serif;
            font-weight: normal;
            padding: 7px 7px 8px;
            text-align: left;
            line-height: 1.3em;
            font-size: 14px;
            background: #B3D5F1;
        }
        #table-3 td
        {
            font-size: 12px;
            padding: 4px 7px 2px;
            vertical-align: top;
        }
    </style>
</asp:Content>
