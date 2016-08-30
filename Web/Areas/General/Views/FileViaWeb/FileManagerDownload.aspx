<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    File Manager Download
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Download File
    </h1>
    <% Html.RenderPartial("DownloadView"); %>
    <% Html.RenderPartial("DailyOutputDownloadView", ViewData["DailyOutputFileDownloadSearch"] as Iata.IS.Model.Common.DailyOutputFileDownloadSearch); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidateDate.js")%>"></script>
    <script type="text/javascript">
     
        $.ajaxSetup({ cache: false });
        function PostData(datatosend, mode) {

            var actionMethod = '<%:Url.Action("DownloadFile", "FileViaWeb", new { area = "General" })%>';

            var myForm = document.createElement("form");
            myForm.method = "post";
            myForm.action = actionMethod;
            var myInput = document.createElement("input");

            myInput.setAttribute("name", "FileToDownload");
            myInput.setAttribute("value", datatosend);

            myForm.appendChild(myInput);
            document.body.appendChild(myForm);
            myForm.submit();
            document.body.removeChild(myForm);
        };
    </script>
    <script type="text/javascript">
        var BillingMonthFromToRestriction = "<%= Iata.IS.Web.AppSettings.BillingMonthFromToRestriction%>";
        var BillingPeriodFromToRestriction = "<%= Iata.IS.Web.AppSettings.BillingPeriodFromToRestriction%>";
        $(document).ready(function () {

            $("#FileFormatId option[value='']").text('All');
            $("#BillingMonthFrom option[value='-1']").remove();
            $("#BillingMonthTo option[value='-1']").remove();
            $("#BillingPeriodFrom option[value='-1']").remove();
            $("#BillingPeriodTo option[value='-1']").remove();
            $("#BillingYear option[value='']").remove();
            $('input[type=submit]').click(function () {
                var fmon = parseInt($("#BillingMonthFrom").val());
                var tmon = parseInt($("#BillingMonthTo").val());
                var fperiod = parseInt($("#BillingPeriodFrom").val());
                var tperiod = parseInt($("#BillingPeriodTo").val());
                var year = parseInt($("#BillingYear").val());
                var startdate = new Date(year, fmon, fperiod);
                var enddate = new Date(year, tmon, tperiod);
                if (startdate > enddate) {
                    alert("Billing Period To (PP-MMM-YYYY) must be greater than or equal to Billing Period From (PP-MMM-YYYY)");
                    return false;
                }
                else {
                    return true;
                }
            
            });


                //CMP #655: IS-WEB Display per Location ID              
                    $("#AssociatedLocation option").each(function (i, selected) {
                        var selectedLocation = $("#MiscLocationCode").val();
                        var selectedLocationArray = selectedLocation.split(',');
                        if (selectedLocation == '') {
                            $(selected).attr('selected', 'selected');
                        } else {
                            var found = $.inArray(selected.text, selectedLocationArray) > -1;
                            if (found) {
                                $(selected).attr('selected', 'selected');
                            }
                        }
                    });
                    var firstOption = $("#AssociatedLocation option:selected:first").attr('title');
                    $("#AssociatedLocation option:selected:first").filter(function () {
                        $(this).removeAttr('selected');
                        return $(this).text() == firstOption;
                    }).attr('selected', true);

                    $("#DailyAssociatedLocation option").each(function (i, selected) {
                            var selectedLocation = $("#MiscLocCode").val();
                            var selectedLocationArray = selectedLocation.split(',');
                            if (selectedLocation == '') {
                                $(selected).attr('selected', 'selected');
                            } else {
                                var found = $.inArray(selected.text, selectedLocationArray) > -1;
                                if (found) {
                                    $(selected).attr('selected', 'selected');
                                }
                            }
                    });
                        
                        var firstOption = $("#DailyAssociatedLocation option:selected:first").attr('title');
                        $("#DailyAssociatedLocation option:selected:first").filter(function () {
                            $(this).removeAttr('selected');
                            return $(this).text() == firstOption;
                        }).attr('selected', true);


                    var title = "At least one Location ID must be selected for successful search results. If no Location IDs are shown here, it means that you are not associated with any Location of your organization. Please contact your organization’s user(s) who have access to the Location Association module to review and associate you with appropriate Location(s).";
                    $("#AssociatedLocation").attr("title", title);
                    $("#AssociatedLocation option").attr("title", title);

                    $("#DailyAssociatedLocation").attr("title", title);
                    $("#DailyAssociatedLocation option").attr("title", title);

                    $("#submitOutputFileSearch").bind('click', function () {
                        BindSelectedLocation();
                    });

                     $("#submitDailyOpSearch").bind('click', function () {
                         BindSelectedLocation();
                    });

                 
        // End Code for CMP#655


            $("#DailyOpFileDownloadSearch").validate({
                rules: {
                    DeliveryDateFrom: "required",
                    DeliveryDateTo: "required",
                    DailyAssociatedLocation : "required"
                },
                messages: {
                    DeliveryDateFrom: "DeliveryDateFrom is required",
                    DeliveryDateTo: "DeliveryDateTo is required",
                    DailyAssociatedLocation : "Location ID required"
                }
            });
        });       


         function BindSelectedLocation() {
                    var selectedLocationIds = '';
                    $("#AssociatedLocation option:selected").each(function () {
                        selectedLocationIds = selectedLocationIds + ',' + $(this).text();
                    });
                     if(selectedLocationIds == '')
                    {
                    selectedLocationIds = ' ';
                    }
                    $("#MiscLocationCode").val(selectedLocationIds);

                    var selectedDailyLocationIds = '';
                    $("#DailyAssociatedLocation option:selected").each(function () {
                        selectedDailyLocationIds = selectedDailyLocationIds + ',' + $(this).text();
                    });
                     if(selectedDailyLocationIds == '')
                    {
                     selectedDailyLocationIds = ' ';
                    }
                    $("#MiscLocCode").val(selectedDailyLocationIds);
                  }   


        $("#DeliveryDateFrom").change(function () {
            ValidateFromToDate('DeliveryDateFrom');
        });

        // Following function is used to check whether To date is greater than From date.
        $("#DeliveryDateTo").change(function () {
            ValidateFromToDate('DeliveryDateTo');
        });

        function ValidateFromToDate(controlId) {
            var dateComparisonResult = validateDateRange('DeliveryDateFrom', 'DeliveryDateTo');
            if (!dateComparisonResult) {
                alert("Delivery Date To should be equal to or later than Delivery Date From");
                $('#' + controlId).val('');
            }
        }

        // Commented below for CMP #655: IS-WEB Display per Location ID
       //CMP#622: MISC Outputs Split as per Location IDs
      // registerAutocompleteWithOutCache('MiscLocationCode', 'MiscLocationCode', '<%:Url.Action("GetMemberLocationList", "Data", new { area = "" })%>', 0, true,null, <%: Convert.ToInt32(SessionUtil.MemberId) %> ,null, null); 
      // registerAutocompleteWithOutCache('MiscLocCode', 'MiscLocCode', '<%:Url.Action("GetMemberLocationList", "Data", new { area = "" })%>', 0, true,null, <%: Convert.ToInt32(SessionUtil.MemberId) %> ,null, null); 
    </script>
    <script type="text/javascript">

        function formatlink(cellValue, options, rowObject) {
            var cellHtml = cellValue;
            var divHtml = "<div style='text-align: center;'> " + cellHtml + "</div>"
            return divHtml;
        }

        function unformatlink(cellValue, options, cellObject) {

            return $(cellObject.html()).attr("originalValue");
        }
    </script>
</asp:Content>
