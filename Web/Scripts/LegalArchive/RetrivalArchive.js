
function refreshDetailGrid()
{
    DisplayDetails(summaryId);
   refreshDetailInvoiceStatusGrid();
}


// Following function is used to refresh InvoiceStatus grid for specified time interval
function refreshDetailInvoiceStatusGrid() {
jQuery("#RetrivalJobSummaryGrid").jqGrid('setGridParam', { url: "RetrivalJobSummaryGridData" }).trigger('reloadGrid');
}

// display selected member (on row click) details
function DisplayDetails(ids) {

summaryId = ids;
    if (ids == null) {
        ids = 0;
        if (jQuery("#ArchiveRetrivalJobDetailsGridControl").jqGrid('getGridParam', 'records') > 0) {

            jQuery("#ArchiveRetrivalJobDetailsGridControl").jqGrid('setCaption', "Job Details: " + ids)
				.trigger('reloadGrid');
        }
    } else {
       
        jQuery("#ArchiveRetrivalJobDetailsGridControl").jqGrid('setGridParam', { url: "GetSelectedJobSummaryDetail?jobSummaryId=" + ids}).trigger('reloadGrid');
    }
}

 function getParameterByName( name )
{
  name = name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
  var regexS = "[\\?&]"+name+"=([^&#]*)";
  var regex = new RegExp( regexS );
  var results = regex.exec( window.location.href );
  if( results == null )
    return "";
  else
    return decodeURIComponent(results[1].replace(/\+/g, " "));
}


$(document).ready(function () {

    
    var grid = jQuery('#RetrivalJobSummaryGrid');

    grid.jqGrid('setGridParam', {
        gridComplete: function () {
            var ids = grid.getDataIDs();
            var id = getParameterByName("jobSyId");
            if (ids.length > 0) {
                var IsSelected = false;
                for (var i = 0, il = ids.length; i < il; i++) {
                    if (ids[i] == id) {
                        IsSelected = true;
                        DisplayDetails(ids[i]);
                        setSelection(ids[i]);
                    }
                }

                if (IsSelected == false) {
                    DisplayDetails(ids[0]);
                    setSelection(ids[0]);
                }
            }
        }
    }).trigger("reloadGrid");

    grid.jqGrid('setGridParam', {
        beforeSelectRow: function (id, e) {
            setSelection(id);
            DisplayDetails(id);
        }
    }).trigger("reloadGrid");

}); 
