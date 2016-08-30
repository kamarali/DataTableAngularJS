<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Base.InvoiceBase>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  ManageSuspendedInvoices
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Manage Suspended Invoices</h1>
  <div>
    <%Html.RenderPartial("ManageSuspendedInvoicesSearchControl"); %>
  </div>
  <div class="buttonContainer">
    <div>
      <input class="primaryButton" type="button" id="searchButton" value="Search" onclick="seachSuspendedInvoices(true);" />
    </div>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("ManageSuspendedInvoicesSearchResultControl", ViewData["ManageSuspendedInvoicesSearchResultGridData"]); %>
  </div>
  <div class="buttonContainer">
    <div>
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.General.ManageSuspendedInvoices.Resubmit))
      {%>
      <input class="primaryButton" type="button" id="btnResubmit" value="Resubmit" onclick="MarkInvoiceAsResubmitted('<%: Url.Action("CheckIfLateSubmissionWindowOpen", "ManageSuspendedInvoices", new { area = "Reports"}) %>');" />
     <%}%>
     <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.General.ManageSuspendedInvoices.Update))
      {%>
      <input class="primaryButton hidden" type="button" id="btnBilateral" value="Bilateral"
        onclick="MarkInvoiceAsBilaterallySettled('<%: Url.Action("MarkInvoiceAsBilaterallySettled", "ManageSuspendedInvoices", new { area = "Reports"}) %>');" />
        <%}%>
      <input class="primaryButton hidden" type="button" id="btnUndoBilateral" value="Undo Bilateral"
        onclick="UndoBilateral('<%: Url.Action("UndoBilateral", "ManageSuspendedInvoices", new { area = "Reports"}) %>');" />
    </div>
  </div>
  <div id="addResubmissionRemark">
    <%Html.RenderPartial("AddResubmissionRemark"); %>
  </div>
  <div id="ichAchresubmissionPeriod">
    
    <div>      
      <div id="ichAchLateSubmissionMsg" style="max-width: 270px; max-height: 150px; overflow: auto;
        font-weight: bold;">
      </div>
      <div>
        <div class="fieldContainer horizontalFlow">
          <%: Html.Label("")%>Resubmission Period
          <%: Html.RadioButton("rbIchAchResubmissionPeriod", "Current", true)%>Current
          <%: Html.RadioButton("rbIchAchResubmissionPeriod", "Previous", false)%>Previous
        </div>
      </div>
    </div>
  </div>
  <div id="ichresubmissionPeriod">
    <div>
      <div id="ichLateSubmissionMsg" style="max-width: 270px; max-height: 150px; overflow: auto;
        font-weight: bold;">
      </div>
      <div>
        <div class="fieldContainer horizontalFlow">
          <%: Html.Label("")%>Resubmission Period
          <%: Html.RadioButton("rbIchResubmissionPeriod", "Current", true)%>Current
          <%: Html.RadioButton("rbIchResubmissionPeriod", "Previous", false)%>Previous
        </div>
      </div>
    </div>
  </div>
  <div id="achresubmissionPeriod">
    <div class="solidBox">
      <div id="achLateSubmissionMsg" style="max-width: 270px; max-height: 150px; overflow: auto;
        font-weight: bold;">
      </div>
      <div>
        <div class="fieldContainer horizontalFlow">
          <%: Html.Label("")%>Resubmission Period
          <%: Html.RadioButton("rbAchResubmissionPeriod", "Current", true)%>Current
          <%: Html.RadioButton("rbAchResubmissionPeriod", "Previous", false)%>Previous
        </div>
      </div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/ManageSuspendedInvoices.js")%>"></script>
  <script type="text/javascript">

     function formatInvoice(cellValue, options, rowObject) {
      var invoiceId = rowObject.Id;
      var billingCategory = rowObject.BillingCategoryId;
       if(billingCategory==1)
      var linkHtml = "<a target='_blank' href='<%: Url.Action("ViewInvoice", "ManagePaxPayablesInvoice", new { area = "PaxPayables" }) %>/" + invoiceId + "?fc=true'>" + cellValue + "</a>";
       if(billingCategory==2)
      var linkHtml = "<a target='_blank' href='<%: Url.Action("ViewInvoice", "PayablesInvoiceSearch", new { area = "CargoPayables" }) %>/" + invoiceId + "?fc=true'>" + cellValue + "</a>";
        if(billingCategory==3)
      var linkHtml = "<a target='_blank' href='<%: Url.Action("ViewInvoice", "ManageMiscPayablesInvoice", new { area = "MiscPayables" }) %>/" + invoiceId + "?fc=true'>" + cellValue + "</a>";
       if(billingCategory==4)
      var linkHtml = "<a target='_blank' href='<%: Url.Action("ViewInvoice", "ManageUatpPayablesInvoice", new { area = "UatpPayables" }) %>/" + invoiceId + "?fc=true'>" + cellValue + "</a>"; 
      return linkHtml;
    }


    function unformatInvoice(cellValue, options, rowObject) {
      return $(cellObject.html()).attr("originalValue");
    }

    function formatRemarkLink(cellValue, options, rowObject) {
      var invoiceId = rowObject.Id;var linkHtml = '<a href="#" onclick=showRemarkDilog("<%: Url.Action("GetInvoiceRemark", "ManageSuspendedInvoices", new { area = "Reports" })%>","' + invoiceId + '")> Remark </a>';
      return linkHtml;
    }

    function unformatRemarkMember(cellValue, options, cellObject) {
      return $(cellObject.html()).attr("originalValue");
    }


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

    var $remarkDialog;



    $(document).ready(function () {
      $("#btnBilateral").show();
      setResubmitUrl('<%: Url.Action("MarkInvoiceAsResubmitted", "ManageSuspendedInvoices", new { area = "Reports"}) %>')
      setsearchUrl('<%:Url.Action("SearchResultGridData", "ManageSuspendedInvoices", new { area = "Reports"}) %>');
      setResubmissionStatusUrl('<%:Url.Action("GetInvoiceResubmissionStatus", "ManageSuspendedInvoices", new { area = "Reports"}) %>')
      $('#ResubmissionStatusId').change(function () {
//        if ($("#ResubmissionStatusId").val() == 2) {
//          $("#cb_ManageSuspendedInvoicesSearchResultGrid").attr('disabled', 'disabled');
//          $("#btnUndoBilateral").show();
//          $("#btnBilateral").hide();
//        }
//        if ($("#ResubmissionStatusId").val() == 1) {
//          $("#btnBilateral").attr("disabled", true);
//          $("#btnUndoBilateral").attr("disabled", true);
//          $("#btnResubmit").attr("disabled", true); 
//        }
//        else {
//          $("#btnBilateral").show();
//          $("#btnUndoBilateral").hide();
//          $("#btnBilateral").attr("disabled", false);
//          $("#btnUndoBilateral").attr("disabled", false);
//          $("#btnResubmit").attr("disabled", false); 
//          $("#cb_ManageSuspendedInvoicesSearchResultGrid").removeAttr("disabled");
//        }
      });
      $remarkDialog = $('<div></div>')
		.html($("#addResubmissionRemark"))
		.dialog({
		  autoOpen: false,
		  title: 'Add Remark',
		  height: 270,
		  width: 270,
		  modal: true,
		  buttons: {
		    Close: function () {
        var remark = $("#ResubmissionRemarks").val();
        var hdnRemark = $("#hiddenResubmissionRemark").val();
        if(remark != hdnRemark)
        {
          if (confirm("You have unsaved remarks. Do you want to continue without saving?"))
           {
              $(this).dialog('close');
           }
          
         }
         else{
          $(this).dialog('close');
          }
		    },
		    Save: function () {
		      SaveRemark('<%:Url.Action("UpdateInvoiceRemark", "ManageSuspendedInvoices", new { area = "Reports" })%>');
		    }
		  },
		  resizable: false
		});

    });


  </script>
 
</asp:Content>
