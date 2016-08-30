<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Iata.IS.Business.Common.Impl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Upload Member Profile Data
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Upload New Member Profile Data</h1>
  
  <% Html.RenderPartial("MemberProfileCSVData"); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  
  <script type="text/javascript">

      $.ajaxSetup({ cache: false });
      function PostData(datatosend, mode) {

          var actionMethod = '<%:Url.Action("DownloadFile", "UploadMemberProfileCSVData", new { area = "ISOps" })%>';

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
      $(function () {
          $("#ajaxUploadForm").ajaxForm({
              iframe: true,
              dataType: "json",
              beforeSubmit: function () {
                  $('#clientErrorMessageContainer').hide();
                  var filename = $("#uploadedFile").val();
                  if ($.trim(filename) == '') {
                      showClientErrorMessage('No file chosen');
                      return false;
                  }
                  if (filename.toUpperCase().indexOf('.ZIP') == -1) {
                      showClientErrorMessage('Please choose a file in compressed format with a .zip extension');
                      return false;
                  }
                  else {
                      var index = filename.lastIndexOf("\\");
                      if (index > 0) {
                          filename = filename.substr(index + 1);
                          filename = filename.toUpperCase().replace('.ZIP', '');
                          var rege = new RegExp('^[a-zA-Z0-9._+-]*$');

                          if (!rege.test(filename)) {
                              showClientErrorMessage('Invalid File Type or File Name.');
                              return false;
                          }
                      }
                  }
                  var iLength = filename.length;
                  if (iLength > 50) {
                      showClientErrorMessage('File name is too long. The maximum permissible length is 50 including the extension');
                      return false;
                  }

                  if ($.browser.msie) {

                      var fileupload = document.getElementById('uploadedFile').size;
                      var size = 0;
                  } else {
                      var iSize = ($("#uploadedFile")[0].files[0].size / 1024);
                      var size = $("#uploadedFile")[0].files[0].size;
                      //alert(size);
                      iSize = (Math.round(iSize * 100) / 100)
                      if (iSize <= 0) {

                          showClientErrorMessage('Upload File Size should be greater than 0KB ');
                          return false;
                      }

                      var MaxLimit = 25600;
                      if (iSize > MaxLimit) {
                          showClientErrorMessage('Upload File Max. Size : 25 MB. \nCurrent File Size in MB :' + Math.ceil(iSize / 1024) + 'MB');
                          return false;
                      }
                  }
              },
              success: function (result) {

                  if (result.flag == 'false') {
                      // response.redirect contains the string URL to redirect to 
                      window.location.href = result.redirect;
                  }
                  else {
                      $("#ajaxUploadForm").resetForm();

                      //Spira IN008334- file upload message appears as a red warning
                      if (result.isFailed == 'false') {
                          showClientSuccessMessage(result.message);
                          $('#UploadGridDataRequested').trigger('reloadGrid');
                      }
                      else {
                          showClientErrorMessage(result.message);
                      }
                  }
              },
              error: function (xhr, textStatus, errorThrown) {
                  alert("There was an error in uploading member profile data process. Please try again, or contact your administrator.");
              }
          });
      });
    </script>  

    <script type="text/javascript">

        function formatlink(cellValue, options, rowObject) {
            var cellHtml = cellValue;

            return cellHtml;
        }

        function unformatlink(cellValue, options, cellObject) {
            return $(cellObject.html()).attr("originalValue");
        }
  </script>

</asp:Content>
