<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  File Manager Upload
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Upload File
  </h1>
  <% Html.RenderPartial("UploadFile"); %>

  <%--CMP #675: Progress Status Bar for Processing of Billing Data Files.
      Desc: Newly added div, rendering progress bar. --%>
	<div id="FileProgressDiv">
    <%
        Html.RenderPartial("ProgressBar"); %>
	</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<%--CMP #675: Progress Status Bar for Processing of Billing Data Files. 
    Desc: Newly added script referred here, ut is used for rendering progress bar UI after applying appropriate css classes. --%>
<script src="<%=Url.Content("~/Scripts/ProgressBar.js")%>" type="text/javascript"></script>

  <script type="text/javascript">

      $.ajaxSetup({ cache: false });
      function PostData1(datatosend, mode) {

          var myForm = document.createElement("form");
          myForm.method = "post";
          myForm.action = mode;

          var myInput = document.createElement("input");
          myInput.setAttribute("name", "FileToDownload");
          myInput.setAttribute("value", datatosend);
          myForm.appendChild(myInput);
          document.body.appendChild(myForm);
          myForm.submit();
          document.body.removeChild(myForm);
      };

      function CheckFilesize() {
          fileupload = document.getElementById('uploadedFile');
          if (fileupload.files) {
              var size = fileupload.files.item(0).fileSize;
          }

          var sizeInKB = Math.ceil(size / 1024);
          var MaxLimit = 25600;
          if (sizeInKB > MaxLimit) {
              alert('Upload File Max. Size : 25 MB. \nCurrent File Size in MB :' + Math.ceil(sizeInKB / 1024) + 'MB');
              return false;
          }
          return true;
      }

   
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
                      showClientErrorMessage('Upload file /filename is empty.');
                      return false;
                  }
                  if (filename.toUpperCase().indexOf('.ZIP') == -1) {
                      showClientErrorMessage('File being uploaded must be zipped (with \".zip\" extension).');
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
                  if ($.browser.msie) {

                      var fileupload = document.getElementById('uploadedFile').size;


                      var size = 0;






                      //            if (fileupload.files) {
                      //              size = fileupload.files.item(0).fileSize;
                      //            }
                      //            if (size <= 0) {


                      //              alert('Upload File Size should be greater than 0KB ');
                      //              return false;
                      //            }




                      //            var sizeInKB = Math.ceil(size / 1024);
                      //            var MaxLimit = 25600;
                      //            if (sizeInKB > MaxLimit) {
                      //              alert('Upload File Max. Size : 25 MB. \nCurrent File Size in MB :' + Math.ceil(sizeInKB / 1024) + 'MB');
                      //              return false;
                      //            }



                      //            var iSize = ($("#uploadedFile")[0].files[0].size / 1024);
                      //            var size = $("#uploadedFile")[0].files[0].size;
                      //            iSize = (Math.round(iSize * 100) / 100)
                      //            if (iSize <= 0) {

                      //              alert('Upload File Size should be greater than 0KB ');
                      //              return false;
                      //            }



                      //            var MaxLimit = 25600;
                      //            if (iSize > MaxLimit) {
                      //              alert('Upload File Max. Size : 25 MB. \nCurrent File Size in MB :' + Math.ceil(iSize / 1024) + 'MB');
                      //              return false;
                      //            }





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
                      }
                      else {
                          showClientErrorMessage(result.message);
                      }
                  }
              },




              error: function (xhr, textStatus, errorThrown) {
                  if (errorThrown.description && errorThrown.description.length > 1 && errorThrown.description.indexOf("Access is denied") != -1)
                      alert("Error in uploading file. Please make sure that file size is less than 25MB.");
                  else alert("There was an error in the file upload process Or File size is larger than 25MB. Please try again, or contact your administrator.");
              }
          });

      });

    </script>
  <script type="text/javascript">
      var BillingMonthFromToRestriction = "<%= Iata.IS.Web.AppSettings.BillingMonthFromToRestriction%>";
      var BillingPeriodFromToRestriction = "<%= Iata.IS.Web.AppSettings.BillingPeriodFromToRestriction%>";
      function validateDateRange(startDateId, endDateId) {
          var startDateVal = $('#' + startDateId).datepicker("getDate");
          var endDateVal = $('#' + endDateId).datepicker("getDate");
          if (startDateVal == null || endDateVal == null) {
              return false;
          }
          return endDateVal >= startDateVal;
      }
      $(document).ready(function () {
          $('#btnSearch').click(function () {
              if (validateDateRange('FileSubmissionFrom', 'FileSubmissionTo'))
                  return true;
              else {
                  alert("From date must be less than or equal to the To Date");
                  return false;
              }

          });

          /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
             Desc: Hiding pop-up div on page load, default behaviour. */
          $dialog = $('<div></div>')
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
  </script>
  <script type="text/javascript">

      function formatlink(cellValue, options, rowObject) {
          var cellHtml = cellValue;

          return cellHtml;
      }

      function unformatlink(cellValue, options, cellObject) {
          return $(cellObject.html()).attr("originalValue");
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

          $dialog = $('<div></div>')
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
</asp:Content>
