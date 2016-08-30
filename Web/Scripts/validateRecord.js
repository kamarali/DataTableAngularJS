/// <reference path="site.js" />
function validateRecord(methodName, value, gridId) {
  if (confirm("Are you sure you want to validate this record?")) {
      $.ajax({
          type: "POST",
          url: methodName,
          data: "id=" + value,
          success: function (result) {
              if (result.IsFailed == false) {
                  var message = '';
                  if (result.IsAlert == true) {
                      message = '<div>This invoice contained Sequence Numbers that did not increment by 1 as compared to the previous transaction in the same batch. This have been renumbered to increment by 1.</div>'
                  }
                  if (result.IsRecalAlert == true) {
                      message = '<div>Form E values were recalculated; and one or more fields in the Form E did not match with the earlier values. Please review the Form E before submission of the invoice.</div>'
                  } 
                  showClientSuccessMessage(result.Message + message);
              }
              else {
                  showClientErrorMessage(result.Message)
              }


              $(gridId).trigger("reloadGrid");
          }
      });
  }
}
