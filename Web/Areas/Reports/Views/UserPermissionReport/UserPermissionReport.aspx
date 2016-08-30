<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.UserPermission>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
  SIS :: Reports :: User Pemission Report
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<% Html.BeginForm("UserPermissionReport", "UserPermissionReport", FormMethod.Post, new { id = "userPermissionReport" }); %>
<h1>User Permission Report</h1>

    <div>
        <% Html.RenderPartial("SearchControlForUserPermissionReport", ViewData); %>
    </div>
    
    <div>
    </div>

 <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report" onclick="UserPermissionReport();"  />
  </div>
<%Html.EndForm(); %>
</asp:Content>
<asp:Content runat="server" ID="Script" ContentPlaceHolderID="Script">
 <script type="text/javascript" src='<%:Url.Content("~/Scripts/UserPermissionReport.js")%>'></script>
    <script type="text/javascript">
        function UserPermissionReport() {
            ValidateUserPermissionReport("userPermissionReport");
        }
        $(document).ready(function () {

            var selectedUserCategory = $("#UserCategoryId").val();

            if (selectedUserCategory == '') {
                $('#divMember').hide();
                $('#divUser').hide();
                $('#btnProxyLogin').hide();
            }

            $('#UserCategoryId').change(function () {
                $('#errorContainer').hide();
                $.watermark.showAll();
                $('#clientErrorMessageContainer').hide();
                $('#clientSuccessMessageContainer').hide();

                var images = document.getElementsByTagName('img');
                for (var i = 0; i < images.length; i++) {
                    if (images[i].nameProp == "error_icon.png")
                        images[i].style.display = "none";
                };
                

                $('#divMember').show();
                $('#divUser').show();

                var selectedItem = $("#UserCategoryId").val();

                $('#MemberName').hide();
                $('#UserEmail').hide();
                $('#lblMember').hide();
                $('#lblUser').hide();

                if (selectedItem != 0 && selectedItem != 4) {

                } else {
                    $('#MemberName').val('');
                    $('#MemberName').show();
                    $('#lblMember').show();
                    $("#MemberName").focus();

                }

                if (selectedItem == 0) {
                    $('#MemberName').val('');
                    $('#MemberName').hide();
                    $('#lblMember').hide();

                }

                $("#CategoryData").val($("#CategoryName").val());
            });

            registerAutocomplete('MemberName', 'MemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);

        });
 </script>
</asp:Content>
