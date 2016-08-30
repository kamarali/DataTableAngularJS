<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<style type="text/css">
    .link
    {
        cursor: pointer;
        text-decoration: underline;
    }
</style>
<script type="text/javascript">

    // Following function is used to refresh InvoiceStatus grid for specified time interval
    function refreshInvoiceStatusGrid() {
        if ($("#isInvoiceTabClicked").val() == "true") {
            var grid = $("#ISSearchResultListGrid");
            grid.trigger("reloadGrid");
            t = setTimeout("refreshInvoiceStatusGrid()", 300000);
        }
    } // end refreshInvoiceStatusGrid()

    // Call refreshInvoiceStatusGrid() function which refreshes InvoiceStatus grid
    refreshInvoiceStatusGrid();
    
    // The FormatFunction for CustomFormatter gets three parameters           
    // cellValue - the original value of the cell
    // options - as set of options, e.g
    //   options.rowId - the primary key of the row
    //   options.colModel - colModel of the column
    // rowData - array of cell data for the row, so you can access other cells in the row if needed

    function formatStatusColumn(cellValue, options, rowObject) {
     
        var imageHtml;
        if (cellValue == "0") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_failed.png") %>" />';
        }
        else if (cellValue == "1") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_pending.png") %>" />';
        }
        else if (cellValue == "2") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_succesfully_completed.png") %>" />';
        }
        else {
            imageHtml = '<b>-</b>';
        }
        return imageHtml;
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    function formatDailyDeliveryColumn(cellValue, options, rowObject) {

        var imageHtml;
        
        if (cellValue == "1") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_pending.png") %>" />';
        }
        else if (cellValue == "2") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_succesfully_completed.png") %>" />';
        }
        else {
            imageHtml = '<b>-</b>';
        }
        return imageHtml;
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    function unformatDailyDeliveryColumn(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }

    //CMP559 : Add Submission Method Column to Processing Dashboard
    function UnformatSubmissionMethod(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }

    //CMP559 : Add Submission Method Column to Processing Dashboard
    function formatSubmissionMethod(cellValue, options, rowObject) {
        var displayVal = '';
        var fileName = cellValue;
        var submissionMethodId = rowObject.SubmissionMethodId;
        var isPurged = rowObject.isPurged;
        var fileLogId = rowObject.FileLogId;
        var isSisOpsUser = '<%= ViewData["IsSISOpsUser"] %>';
        var isIsIdecOrIsXml = false;
        switch (submissionMethodId)
        {
            case "1":
                displayVal = "IS-IDEC ";
                isIsIdecOrIsXml = true;
                break;
            case "2":
                displayVal = "IS-XML ";
                isIsIdecOrIsXml = true;
                break;
            case "3": 
                displayVal = "IS-WEB ";
                break;
            case "4": 
                displayVal = "Auto-Billing ";
                break;
            default:
                break;
        }
        if (isPurged == "0" && fileName != '' && isSisOpsUser == "true" && isIsIdecOrIsXml) {            
            displayVal = displayVal + GetLinkForFileName(fileName, fileLogId); //add link if sis ops and file not purged and fileName!= '' (ie of IS-WEB)
        }
        else {
            displayVal = displayVal + fileName;
        }
        //testing purpose
        //displayVal = displayVal + GetLinkForFileName(fileName, fileLogId); //add link if sis ops        
        return displayVal;
    }//eof

    //CMP559 : Add Submission Method Column to Processing Dashboard
    function GetLinkForFileName(fileName, fileLogId) {

        var url = '<%: Url.Action("DownloadFile", "ProcessingDashboard", new { area = "Reports" })%>';        
        var link = '<a style="cursor:hand;" target=_parent href="'+url+'?fileLogId=' + fileLogId + '">' + fileName + '</a>';        
        return link;
    }

    function formatSuspendedLateSubmitted(cellValue, options, rowObject) {
//      if ((IsSuspendedLateSubmitted == "") || (IsSuspendedLateSubmitted == null))
//        return "&nbsp;";

//      if ((cellValue != "") || (cellValue != null)) return "Yes";
//      return "NA";
        var imageHtml;
        if (cellValue == "00") {
            if ((rowObject.SettlementFileStatus == "1" || rowObject.SettlementFileStatus == "2" || rowObject.SettlementFileStatus == "5") && (rowObject.DigitalSignatureStatus == "4"))
                imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_late_submission.png") %>"/>';
           else
             imageHtml = '&nbsp;';
        }
        else if (cellValue == "01") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_late_submission.png") %>"/>';
        }
        else if (cellValue == "10") {
            if ((rowObject.SettlementFileStatus == "1" || rowObject.SettlementFileStatus == "2" || rowObject.SettlementFileStatus == "5") && (rowObject.DigitalSignatureStatus == "4"))
                imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_suspended_member.png") %>" />' + '&nbsp;<img src="<%:Url.Content("~/Content/Images/status_late_submission.png") %>"';
            else
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_suspended_member.png") %>" />';
        }
        else if (cellValue == "11") {
            imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_suspended_member.png") %>" />' + '&nbsp;<img src="<%:Url.Content("~/Content/Images/status_late_submission.png") %>"';
        }
        else {
            if ((rowObject.SettlementFileStatus == "1" || rowObject.SettlementFileStatus == "2" || rowObject.SettlementFileStatus == "5") && (rowObject.DigitalSignatureStatus == "4"))
                imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_late_submission.png") %>"/>';
            else
                imageHtml = '<b>-</b>';
        }

        return imageHtml;
    }

    function formatBillingMemberColumn(cellValue, options, rowObject) {
        var html;
        //hardcoding for billing members 123124(3rd row),124323(5th row)
        if (rowObject.BilledMemberName == "123124") {
            html = cellValue + '&nbsp;<img src="<%:Url.Content("~/Content/Images/status_deleted.png") %>" title="Deleted"/>'
        }
        else if (rowObject.BilledMemberName == "124323") {
            html = cellValue + '&nbsp;<img src="<%:Url.Content("~/Content/Images/status_late_submission.png") %>" title="Late Submission"/>'
        }
        else {
            html = cellValue;
        }
        return html;
    }

    function formatInvoice(cellValue, options, rowObject) {
        var invoiceId = rowObject.InvoiceId;
       var linkHtml = '<a href="#" onclick=showDialog("<%: Url.Action("GetInvoiceDetail", "ProcessingDashboard", new { area = "Reports" })%>","' + invoiceId + '")>' + cellValue + '</a>';
        return linkHtml;
    }


 

    // The FormatFunction for CustomFormatter gets three parameters
    // cellValue - the original value of the cell
    // options - as set of options, e.g
    //   options.rowId - the primary key of the row
    //   options.colModel - colModel of the column
    // cellObject - the HMTL of the cell (td) holding the actual value

    function unformatBillingMemberColumn(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }
    function formatInvoiceNumber(cellValue, options, rowObject) {
        return '<a onclick="showDialog()" class = "link">' + cellValue + '</a>';
    }
    function unformatInvoiceNumber(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }

    function unformatSuspendedLateSubmitted(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }

    function unformatStatusColumn(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }
    function unformatInvoice(cellValue, options, rowObject) {
        return $(cellObject.html()).attr("originalValue");
    }
    
</script>
<div class="gridContainer">
    <%= Html.Trirand().JQGrid(ViewData.Model, "ISSearchResultListGrid")%>
</div>
<div id="divInvoiceDetails">
</div>
<script type="text/javascript">
    var $dialog;
    $(document).ready(function () {
        $dialog = $('<div></div>')
		.html($("#divInvoiceDetails"))
		.dialog({
		    autoOpen: false,
		    title: 'Additional Details of Invoice',
		    height: 345,
		    width: 600,
		    modal: true,
		    resizable: false
		});
    });

    function showDialog(iurl, id) {

        $.ajax({
            type: "POST",
            url: iurl,
            dataType: "html",
            data: { invoiceId: id },
            success: function (response) {
                $dialog = $('<div></div>')
		        .html(response)
		    .dialog({
		        autoOpen: true,
		        title: 'Additional Details of Invoice',
		        height: 345,
		        width: 600,
		        modal: true,
		        resizable: false
		    });
            },
            error: function (xhr, textStatus, errorThrown) {
                alert('An error occurred! ' + errorThrown);
            }
        });

        return false;
    }

    $(document).ready(function () {
       $(".ui-tabs-selected:first").find('A').focus();
       
    });
</script>
