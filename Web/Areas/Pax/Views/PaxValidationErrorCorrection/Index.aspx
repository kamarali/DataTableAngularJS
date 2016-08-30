<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ValidationErrorCorrection>" %>

<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Pax :: Receivables :: Validation Error Correction
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%=Url.Content("~/Scripts/Pax/ValidationErrorCorrection.js")%>" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts/Pax/SamplingValidationErrorCorrection.js")%>" type="text/javascript"></script>
    <script type="text/javascript">
      $(document).ready(function () {
        // When user enters corrected value change it to uppercase and set value of text box, so we can get it in uppercase in action method
        $("#NewValue").keyup(function () {
          var newValue = $("#NewValue").val();
          $("#NewValue").val(newValue.toUpperCase());
        });

        // Set focus on update button when user enters New value
        $("#NewValue").blur(function () { $('#UpdateButton1').focus() });
      });
        $(function () {

            samplingSearchResult = '<%:Url.Action("ShowSearchResultPax","PaxValidationErrorCorrection", new {area = "Pax"}) %>';
            nonSamplingSearchResult = '<%:Url.Action("ShowSearchResult","PaxValidationErrorCorrection", new {area = "Pax"}) %>';
           
            $("#tabs").tabs({

                cache: false,
                load: function (event, ui) {
                    // Format all the date controls on the page.
                    formatDateControls();

                    // Decorate the '*' on the mandatory fields with red color.
                    highlightMandatory();
                }
            //IATA SIS-P4-Third party : Jquery And Jquery UI 1.12.3 Upgradation
            }).on('tabsactivate', function (event, ui) {
                $("#clientErrorMessageContainer").css("display", "none");
                $("#clientSuccessMessageContainer").css("display", "none");
                $("#errorNote").html("");

            });


        })
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Validation Error Correction</h1>
    <br />
    <div id="tabs">
        <ul class="ui-tabs-nav">
            <%: Html.Tab("Invoices/Credit Notes", "ValidationErrorCorrection", "PaxValidationErrorCorrection", new { area = "Pax" }, new { id = "PaxValidationErrorCorrectionValidationErrorCorrection-tab" }, UserCategory.Member)%>
            <%: Html.Tab("Form Cs", "SamplingValidationErrorCorrection", "PaxValidationErrorCorrection", new { area = "Pax" }, new { id = "PaxValidationErrorCorrectionSamplingValidationErrorCorrection-tab" }, UserCategory.Member)%>

        </ul>
    </div>
</asp:Content>