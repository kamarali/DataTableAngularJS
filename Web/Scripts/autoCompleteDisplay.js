
function SourceCodeId_SetAutocompleteDisplay(item) {
  var sourceCode = item.SourceCodeName + '-' + item.SourceCodeDescription;
  return { label: sourceCode, value: item.SourceCodeName, id: item.Id };
}

function ReasonCodeId_SetAutocompleteDisplay(item) {
  var reasonCode = item.Code + '-' + item.Description;
  return { label: reasonCode, value: item.Code, id: item.Id };
}

function TicketOrFimIssuingAirline_SetAutocompleteDisplay(item) {
  return { label: item.MemberCodeNumeric, value: item.MemberCodeNumeric, id: item.Id };
}

function ProvisionalBillingMemberText_SetAutocompleteDisplay(item) {
  var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "-" + item.CommercialName;
  return { label: memberCode, value: memberCode, id: item.Id };
}

function BilledMemberText_SetAutocompleteDisplay(item) {
  var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "-" + item.CommercialName + "";
  return { label: memberCode, value: memberCode, id: item.Id };
}
//SCP 121308 : Reason Codes in PAX billing history screen appear multiple times.
//Purpose: Need to remove cached list and get field values using it's ids.
function registerReasonCodeAutocomplete(displayFieldId, idField, actionUrl, minCharLength, exactMatch, onItemSelected, extraParam1, extraParam2, dependentcontrolId, onItemCleared) {
  
   $("#" + displayFieldId).autocomplete(actionUrl, {
       minChars: minCharLength,
       max: 1000,
       autoFill: false,
       extraParams: {
           extraparam1: function () {
               if (extraParam1 && $(extraParam1))
                   return $(extraParam1).val();
           },
           extraparam2: function () {
               if (extraParam2 && $(extraParam2))
                   return $(extraParam2).val();
           },
           dependentValue: function () {
               if (dependentcontrolId && $(dependentcontrolId))
                   return $(dependentcontrolId).val();
           }
       },
       cacheLength: 0,
       mustMatch: exactMatch,
       matchContains: true,
       formatItem: function (data, index, max) {
           return data[0];
       },
       formatResult: function (data, index, max) {
           return data[0];
       },
       formatMatch: function (data, index, max) {
           return data[0];
       }
   }).result(function (event, data, formatted) {
       if (data) {

           $("#" + idField).val(data[1]);

           if (onItemSelected) {
               onItemSelected(data[1], data[0], formatted);
           }
       }
       else {
           $("#" + idField).val('');
           $("#" + displayFieldId).val('');

           //If no data then call function.
           if (onItemCleared) {
               onItemCleared();
           }
       }
   });
}

function registerAutocomplete(displayFieldId, idField, actionUrl, minCharLength, exactMatch, onItemSelected, extraParam1, extraParam2, dependentcontrolId, onItemCleared) {
  actionUrl = actionUrl.replace("&amp;", "&");
  $("#" + displayFieldId).autocomplete(actionUrl, {
    minChars: minCharLength,
    max: 1000,
    autoFill: false,
    extraParams: {
      extraparam1: function () {
        return extraParam1;
      },
      extraparam2: function () {
        return extraParam2;
      },
      dependentValue: function () {
        if (dependentcontrolId && $(dependentcontrolId))
          return $(dependentcontrolId).val();
      }
    },
    mustMatch: exactMatch,
    matchContains: true,
    formatItem: function (data, index, max) {
      return data[0];
    },
    formatResult: function (data, index, max) {
      return data[0];
    },
    formatMatch: function (data, index, max) {
      return data[0];
    }
  }).result(function (event, data, formatted) {
    if (data) {

      $("#" + idField).val(data[1]);

      if (onItemSelected) {
        onItemSelected(data[1], data[0], formatted);
      }
    }
    else {
      $("#" + idField).val('');
      $("#" + displayFieldId).val('');

      //If no data then call function.
      if (onItemCleared) {
        onItemCleared();
      }
    }
  });

  // SCP190774: ERROR SAVING RM
  // To prevent the spacebar key for Issuing Airline field from IS-WEB.
  PreventSpace(idField);
}

//CMP#502 : Rejection Reason for MISC Invoices
function registerMiscReasonCodeAutocomplete(displayFieldId, idField, actionUrl, minCharLength, exactMatch, onItemSelected, extraParam1, extraParam2, dependentcontrolId, onItemCleared) {
  actionUrl = actionUrl.replace("&amp;", "&");
  $("#" + displayFieldId).autocomplete(actionUrl, {
    minChars: minCharLength,
    max: 1000,
    autoFill: false,
    extraParams: {
      extraparam1: function () {
        return extraParam1;
      },
      extraparam2: function () {
        return extraParam2;
      },
      dependentValue: function () {
        if (dependentcontrolId && $(dependentcontrolId))
          return $(dependentcontrolId).val();
      }
    },
    cacheLength: 0,
    mustMatch: exactMatch,
    matchContains: true,
    formatItem: function (data, index, max) {
      return data[0];
    },
    formatResult: function (data, index, max) {
      return data[0];
    },
    formatMatch: function (data, index, max) {
      return data[0];
    }
  }).result(function (event, data, formatted) {
    if (data) {
      $("#" + idField).val(data[1]);

      if (onItemSelected) {
        onItemSelected(data[1], data[0], formatted);
      }
    }
    else {
      $("#" + idField).val('');
      $("#" + displayFieldId).val('');

      //If no data then call function.
      if (onItemCleared) {
        onItemCleared();
      }
    }
  });

}

//CMP#622 
function registerAutocompleteWithOutCache(displayFieldId, idField, actionUrl, minCharLength, exactMatch, onItemSelected, extraParam1, extraParam2, dependentcontrolId, onItemCleared) {
  actionUrl = actionUrl.replace("&amp;", "&");
  $("#" + displayFieldId).autocomplete(actionUrl, {
    minChars: minCharLength,
    max: 1000,
    autoFill: false,
    extraParams: {
      extraparam1: function () {
        return extraParam1;
      },
      extraparam2: function () {
        return extraParam2;
      },
      dependentValue: function () {
        if (dependentcontrolId && $(dependentcontrolId))
          return $(dependentcontrolId).val();
      }
    },
    cacheLength: 0,
    mustMatch: exactMatch,
    matchContains: true,
    formatItem: function (data, index, max) {
      return data[0];
    },
    formatResult: function (data, index, max) {
      return data[0];
    },
    formatMatch: function (data, index, max) {
      return data[0];
    }
  }).result(function (event, data, formatted) {
    if (data) {
      $("#" + idField).val(data[1]);

      if (onItemSelected) {
        onItemSelected(data[1], data[0], formatted);
      }
    }
    else {
      $("#" + idField).val('');
      $("#" + displayFieldId).val('');

      //If no data then call function.
      if (onItemCleared) {
        onItemCleared();
      }
    }
  });

}

// SCP190774: ERROR SAVING RM
// To prevent the spacebar key for Issuing Airline field from IS-WEB.
function PreventSpace(idField) {
  if (idField == "AwbIssueingAirline" || idField == "TicketOrFimIssuingAirline" || idField == "TicketIssuingAirline") {
    $("#" + idField).keydown(function (e) {
      if (e.keyCode == 32) {
         return false;
      }
    });
  } 
}