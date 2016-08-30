<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
 SIS :: Master Maintenance :: General :: Legal Archive Retrieval :: Search and Retrieve
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

 <%: ScriptHelper.GenerateArchiveRetrivalJobDetailsScript(Url, ControlIdConstants.SearchGrid,
                    Url.Action("DownloadFile", "ArchiveRetrieval", new { area = "LegalArchive" }))%>

<script type="text/javascript" language="javascript" src="<%:Url.Content("~/Scripts/LegalArchive/RetrivalArchive.js")%>"></script>

<script type="text/javascript" >
    var summaryId;

    function setSelection(id) {
        var grid = jQuery('#RetrivalJobSummaryGrid');
        grid.resetSelection();
        var ids = grid.getDataIDs();
        for (var i = 0, il = ids.length; i < il; i++) {
            if (ids[i] == id) {

                grid.jqGrid('setSelection', id);
                //grid.setSelection(ids[i], false);
            }
            else {
                grid.jqGrid('setSelection', -1);
            }
        }
    }

    function DownloadLegalZip(cellValue) {        
        PostData(cellValue,'');
    }


    $.ajaxSetup({ cache: false });
    function PostData(datatosend, mode) {
        var summaryGridCurrentPageNo = $('#RetrivalJobSummaryGrid').getGridParam('page');
        var summaryGridCurrentPageSize = $('#RetrivalJobSummaryGrid').getGridParam('rowNum');
        var detailGridCurrentPageNo = $('#ArchiveRetrivalJobDetailsGridControl').getGridParam('page');
        var detailGridCurrentPageSize = $('#ArchiveRetrivalJobDetailsGridControl').getGridParam('rowNum');

        var data = '$' + summaryGridCurrentPageNo + '$' + summaryGridCurrentPageSize + '$' + detailGridCurrentPageNo + '$' + detailGridCurrentPageSize;

        var datatopost = datatosend + data;
        var actionMethod = '<%:Url.Action("DownloadFile", "ArchiveRetrieval", new { area = "LegalArchive" })%>';

        var myForm = document.createElement("form");
        myForm.method = "post";
        myForm.action = actionMethod;
        var myInput = document.createElement("input");

        myInput.setAttribute("name", "FileToDownload");
        myInput.setAttribute("value", datatopost);

        myForm.appendChild(myInput);
        document.body.appendChild(myForm);
        myForm.submit();
        document.body.removeChild(myForm);
    };

</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>Legal Archive - Download Retrieved Invoices</h2>
<br/>
<%if (TempData.ContainsKey("DownloadFileError")) %>
<%{ %>
<font color="red">
  <%= Html.Encode(TempData["DownloadFileError"])%></font>
<%} %>
<br/>
<br/>
<div>
        <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RetrivalJobSummaryGridViewData]); %>
</div>
<br />
<br />
<div>
        <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RetrivalJobDetailGridViewData]); %>
</div>
<br/>
<div class="buttonContainer">
    <input class="primaryButton" type="button"  onclick="refreshDetailGrid();"  value="Refresh" />
      <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.General.LegalArchive.SearchRetrieve))
       {%>
    <%:Html.LinkButton("Search and Retrieve", Url.Action("Search", "ArchiveRetrieval"))%>
    <%
       }%>
</div>
</asp:Content>


