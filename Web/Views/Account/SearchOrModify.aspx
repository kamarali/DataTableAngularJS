<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SIS.Web.UIModels.Account.SearchBarModel>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Profile and User Management :: Manage Users
</asp:Content>
<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        $.ajaxSetup({ cache: false });
       function PostData(datatosend, mode) {
            var myForm = document.createElement("form");
            myForm.method = "post";
            myForm.action = "SearchOrModify";
            var myInput = document.createElement("input");
            myInput.setAttribute("name", mode);
            myInput.setAttribute("value", datatosend);
            myForm.appendChild(myInput);
            document.body.appendChild(myForm);
            myForm.submit();
            document.body.removeChild(myForm);
        };

        registerAutocomplete('MemberName', 'MemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);



        $('#UserCategoryId').change(function () {
            var CategoryId = $("#UserCategoryId").val();
            if (CategoryId != null) {
                if (CategoryId == '4') {
                    $('#MemberName').show();
                    $('#divMember').show();
                    $('#ddVisible').val("1");
                    $('#ddVisible').hide();
                }
                else {
                    $('#MemberName').hide();
                    $('#divMember').hide();
                    $('#MemberName').val('');
                    $('#MemberId').val('');
                    $('#ddVisible').val("0");
                    $('#ddVisible').hide();
                }
            }

        });


        $(document).ready(function () {
            $('#UserCategoryId').focus();
            var CategoryId = $("#UserCategoryId").val();
            if (CategoryId != null) {
                if (CategoryId == '4') {
                    $('#MemberName').show();
                    $('#divMember').show();
                    $('#ddVisible').val("1");
                    $('#ddVisible').hide();
                } else {
                    $('#MemberName').hide();
                    $('#divMember').hide();
                    $('#MemberName').val('');
                    $('#MemberId').val('');
                    $('#ddVisible').val("0");
                    $('#ddVisible').hide();
                }
            }            

        });


    </script>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Manage Users</h1>
    <p>
        Use the below form to search and modify a selected user.
    </p>
    <% Html.RenderPartial("~/Views/Account/UserSearchControl.ascx"); %>
</asp:Content>
