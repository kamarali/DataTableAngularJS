<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>

<script type="text/javascript">
    function formatFileName(cellValue, options, rowObject) {
        var html = cellValue;
        if (cellValue == "AB_PAX_JUL09_P2_2") {
            html = cellValue + '&nbsp;<img src="<%:Url.Content("~/Content/Images/status_deleted.png")%>" title="Deleted"/>';
        }
        return html;
    }
    function unformatFileName(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }
    function formatFileStatus(cellValue, options, rowObject) {
        var present = rowObject.NumberOfInvoicesInFile;
        var passed = rowObject.NumberOfValidInvoicesInFile;
        var failed = rowObject.NumberOfInvalidInvoicesInFile;
        var linkHtml = '<a href="#" onclick=showDialog(' + present + ',' + passed + ',' + failed + ')>' + cellValue + '</a>';
        return linkHtml;
    }

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files 
       Desc: Client side function executing on image click. */
    function ShowFileProgressStatus(IsFileLogId) {
        
        var actionMethod = '<%:Url.Action("GetFileProgressDetails", "Data", new { area = "" })%>';
        $.ajax({
            type: "POST",
            url: actionMethod,
            data: { fileLogId: IsFileLogId },
            dataType: "json",
            success: function (response) {
                
                if (response.IsSuccess == true) {
                    showFileProgressStatusDialog(response.Process, response.State, response.Position);
                }
                else {
                    alert('Unable to retriever File Processing Progress Details or File Status may have recently changed, please refresh grid and then try again.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.statusText);
                alert(thrownError);
            }
        });        
    }

    // Following function is used to refresh FileStatus grid for specified time interval
    function refreshFileStatusGrid() {
        if ($("#isInvoiceTabClicked").val() == "false") {
            var grid = $("#FSSearchResultListGrid");
            grid.trigger("reloadGrid");
            t = setTimeout("refreshFileStatusGrid()", 60000);
        }
    } // end refreshFileStatusGrid()

    // Call refreshFileStatusGrid() function which refreshes FileStatus grid
    refreshFileStatusGrid();
</script>
<style type="text/css">
    .boldText
    {
        font-weight: bold;
    }
</style>
<div id="fileStatus">
    <table>
        <tr>
            <td style="text-align: right">
                Invoice Present:
            </td>
            <td id="present">
                8
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                Passed Validation:
            </td>
            <td id="passed">
                7
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                Failed Validation:
            </td>
            <td id="failed">
                1
            </td>
        </tr>
    </table>
</div>

<%--CMP #675: Progress Status Bar for Processing of Billing Data Files--%>
<div id="FileProgressDiv">
       <%
        Html.RenderPartial("ProgressBar"); %>
</div>

<div class="solidBox dataEntry">
  <div class="gridContainer">
    <%= Html.Trirand().JQGrid(ViewData.Model, "FSSearchResultListGrid")%>
  </div>
</div>
<script type="text/javascript">
    var $dialog;
    var $progressdialog;

    $(document).ready(function () {
        $dialog = $('<div></div>')
		.html($("#fileStatus"))
		.dialog({
		    autoOpen: false,
		    title: 'Additional Details of File',
		    height: 60,
		    width: 240,
		    modal: true,
		    resizable: false
		});

		/* CMP #675: Progress Status Bar for Processing of Billing Data Files.
		Desc: Hiding pop-up div on page load, default behaviour. */
        $progressdialog = $('<div></div>')
        .html($("#FileProgressDiv"))
        .dialog({
        	autoOpen: false,
        	title: 'Processing Progress Status',
        	height: 60,
        	width: 240,
        	modal: true,
        	resizable: false
        });
    });

    function showDialog(present, passed, failed) {
        $("#present").html(present);
        $("#passed").html(passed);
        $("#failed").html(failed);
        $dialog = $('<div></div>')
		.html($("#fileStatus"))
		.dialog({
		    autoOpen: true,
		    title: 'Additional Details of File',
		    height: 100,
		    width: 270,
		    modal: true,
		    resizable: false
		});

        return false;
    }

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files
    Desc: Function to create File Status Dialog on this page. */
    function showFileProgressStatusDialog(Process_Name, Process_State, Queue_Position) {

        if (Process_Name == 'NONE' || Process_State == 'NONE') {
            alert('Unable to retriever File Processing Progress Details or File Status may have recently changed, please refresh grid and then try again.');
            return false;
        }
        if (Process_State == 'PENDING' && Queue_Position == '-1') {
            alert('Unable to retriever File Processing Progress Details or File Status may have recently changed, please refresh grid and then try again.');
            return false;
        }

        $("#FileProgressDiv").val('');

        $progressdialog = $('<div></div>')
	    .html($("#FileProgressDiv"))
	    .dialog({
	        autoOpen: true,
	        title: 'Processing Progress Status',
	        height: 200,
	        width: 700,
	        modal: true,
	        resizable: false
	    });

	    /* Common logic to render progress bar. */
	    renderProgressBar(Process_Name, Process_State, Queue_Position);	    
        return false;
    }
    
</script>
