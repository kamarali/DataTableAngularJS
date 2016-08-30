$(document).ready(function () {

  // Removed gridContainer class from grid to remove horizontal scroll bar   
  $('.gridContainer').removeClass();
  $("#LegalArchiveSearchForm").validate({
    rules: {
      Type: "required",
      BillingYear: "required",

      MemberText: {
        required: function (element) {
          var value = $("#MemberText").val();
          var BM = $("#BillingMonth").val();

          if (value == '' && BM == '-1') {
            return true;
          }
          else {
            return false;
          }
        }
      }

    },
    messages: {
      Type: "Type required",
      BillingYear: "Billing year required",
      MemberText: "If “All” is chosen in ‘Billing Month’, then ‘Member’ becomes mandatory Conversely, if ‘Member’ is left blank, then ‘Billing Month’ cannot be “All”"
    }
  });

    //CMP #666: MISC Legal Archiving Per Location ID    
    $("#AssociatedLocation option").each(function (i, selected) {
        var selectedLocation = $("#ArchivalLocationId").val();
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


    var title = "At least one Location ID must be selected for successful search results of MISC archives. If no Location IDs are shown here, it means that you are not associated with any Location of your organization. Please contact your organization’s user(s) who have access to the Location Association module to review and associate you with appropriate Location(s)";
    $("#AssociatedLocation").attr("title", title);
    $("#AssociatedLocation option").attr("title", title);    

});

 

function resetSearchCriteria() {
        $(':input', '#SupportingDocSearchForm')
            .not(':button, :submit, :reset, :hidden')
            .val('')
            .removeAttr('selected');
        //$("#BillingYearMonth").val("-1");
        $("#BillingPeriod").val("-1");
        $("#BillingCode").val("-1");
        $("#SupportingDocumentTypeId").val("1");
        OnSupportingDocumentTypeChange();
        $("#AttachmentIndicatorOriginal").val("3");
}

function RowCount(gridId) {
    var grid = jQuery(gridId);
    grid.resetSelection();
    var ids = grid.getDataIDs();
    if (ids.length >= 1)
        return true;
    else
        return false;
}

/*comment to solve issue 8432 at tfs */
//function DeSelectAll(gridId) {
  //  var grid = jQuery(gridId);
  //  grid.resetSelection();
//}

function RetriveAll(gridId, urlAction) {
    if(RowCount(gridId)) {
        $.ajax({
            type: "POST",
            url: urlAction,
            data: { model: $('form#LegalArchiveSearchForm').serialize() },
            dataType: "json",
            success: function(result) {
                if (result.IsFailed == false) {
                    showClientSuccessMessage(result.Message);
                } else {
                    showClientErrorMessage(result.Message);
                }
            },
            error: function(result) {

            }
        });
    } else {
        showClientErrorMessage("No record to retrive.");
    }
}

function RetriveArchives(gridId, urlAction) {
    var selectedIds = jQuery(gridId).getGridParam('selarrrow');

    var selectedLocationIds = '';
    $("#AssociatedLocation option:selected").each(function () {
        selectedLocationIds = selectedLocationIds + ',' + $(this).text();
    });

    $.ajax({
        type: "POST",
        url: urlAction,
        data: { id: selectedIds.toString(), selectedLocations : selectedLocationIds, model: $('form#LegalArchiveSearchForm').serialize() },
        dataType: "json",
        success: function (result) {
            if (result.IsFailed == false) {
                showClientSuccessMessage(result.Message);
                $(gridId).trigger("reloadGrid");
            }
            else {
                showClientErrorMessage(result.Message);
            }
        },
        error: function (result) {

        }
    });
}

