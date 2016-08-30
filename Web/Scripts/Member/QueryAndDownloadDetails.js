var fetchedMemberId = 0;
var isContactDataFetched = false;
var isAvailableOwnFieldsPopulated = false;
var availFieldURL = '';
var getContactDetailsURL = '';
//SCP 460936: SRM - Session Expired issue
var errorMessage = 'An exception occurred in the server; or this session might have expired. Please log in again to perform this action. If this issue persists, please contact SIS Help Desk.';

//Function to get contact sub groups for given group ids.
function getAvailableFields(posturl, postdata) {
  $.ajax({
    type: "POST",
    url: posturl,
    dataType: "json",
    data: { reportType: postdata, memberId: $("#Id").val() },
    success: function (response) {
      var subgroups = response;

      var objSelect = $('#availableFields');
      $('option', objSelect).remove();

      var myOptions = "";
      // iterate over subgroups
      for (var x = 0; x < subgroups.length; x++) {
        var twt = subgroups[x];
        myOptions = myOptions + '<option title=\"' + twt.Text + '\" value=\"' + twt.Value + '\">' + twt.Text + '</option>';
      }
      objSelect.html(myOptions);
      $("#availableFields").attr("disabled", false);
      $(".disableTemp").attr("disabled", false);

    },
    error: function (xhr, textStatus, errorThrown) {
        alert(errorMessage);
    },
    beforeSend: function (xmlRequest) {
      $("#availableFields").attr("disabled", true);
      $(".disableTemp").attr("disabled", true);
    }
  });
}

//Function to get contact sub groups for given group ids.
function getSubGroups(posturl, postdata) {
  $.ajax({
    type: "POST",
    url: posturl,
    dataType: "json",
    data: { groupIds: postdata },
    success: function (response) {
      var subgroups = response;

      var objSelect = $('#contactTypeSubGroup');
      $('option', objSelect).remove();

      var myOptions = "";
      // iterate over subgroups
      for (var x = 0; x < subgroups.length; x++) {
        var twt = subgroups[x];
        myOptions = myOptions + '<option title=\"' + twt.Name + '\" value=\"' + twt.Id + '\">' + twt.Name + '</option>';
      }
      objSelect.html(myOptions);
    },
    error: function (xhr, textStatus, errorThrown) {
        alert(errorMessage);
    }
  });
}

//Function to get contact types for given group ids and sub group ids.
function getContactTypes(posturl, postdata1, postdata2) {
  $.ajax({
    type: "POST",
    url: posturl,
    dataType: "json",
    data: { groupIds: postdata1, subGroupIds: postdata2 },
    success: function (response) {
      var subgroups = response;

      var objSelect = $('#contactType');
      $('option', objSelect).remove();

      var myOptions = "";
      // iterate over subgroups
      for (var x = 0; x < subgroups.length; x++) {
        var twt = subgroups[x];
        myOptions = myOptions + '<option title=\"' + twt.ContactTypeName + '\" value=\"' + twt.Id + '\">' + twt.ContactTypeName + '</option>';
      }
      objSelect.html(myOptions);
    },
    error: function (xhr, textStatus, errorThrown) {
        alert(errorMessage);
    }
  });
}

//Function to get contact types for given group ids and sub group ids.
function getContactDetails(posturl, postdata) {
  $.ajax({
    type: "POST",
    url: posturl,
    dataType: "json",
    data: { memberId: postdata },
    success: function (response) {
      var details = response;

      var objSelect = $('#ContactName');
      $('option', objSelect).remove();
      var objSelectEmail = $('#Email');
      $('option', objSelectEmail).remove();

      //Populate contact names
      if (details.ContactName) {
        var myOptions = "<option value=\"\" title=\"Please Select\">Please Select</option>";
        for (var x = 0; x < details.ContactName.length; x++) {
          var twt = details.ContactName[x];
          myOptions = myOptions + '<option title=\"' + twt.Name + '\" value=\"' + twt.Id + '\">' + twt.Name + '</option>';
        }
        objSelect.html(myOptions);
      }

      //Populate contact email addresses
      if (details.ContactEmail) {
        myOptions = "<option value=\"\" title=\"Please Select\">Please Select</option>";
        for (var x = 0; x < details.ContactEmail.length; x++) {
          var twt = details.ContactEmail[x];
          myOptions = myOptions + '<option title=\"' + twt.Email + '\" value=\"' + twt.Id + '\">' + twt.Email + '</option>';
        }
        objSelectEmail.html(myOptions);
      }
    },
    error: function (xhr, textStatus, errorThrown) {
        alert(errorMessage);
    }
  });
}

//Populate search criteria.
function PopulateSearchCriteria() {
  setRequiredInformation();
  var metaIds = '' + $.map($('#selectedFields option'), function (e) { return $(e).val(); });
  var contactTypeMetaIds = '' + $.map($('#selectedFields option[value^="T"]'), function (e) { return $(e).val(); });
  contactTypeMetaIds = contactTypeMetaIds.replace(/C/gi, '');
  //if (metaIds.length == 0 && contactTypeMetaIds.length == 0 && $("#reportType").val() != '3') {  
  if (metaIds.length == 0 && contactTypeMetaIds.length == 0 && $("#reportType").val() != '4') {
    alert('Minimum one field from the Available Fields list should be selected.');
    return null;
  }

  var countryId = $('#Country').val();

  //Get search criteria for contact details.
  var contactId = '', emailId = '';
  var groupIdList = '', subGroupIdList = '', contactTypeIdList = '';
  if ($("#rdContactDetails").prop('checked')) {
    contactId = $('#ContactName').val();
    if ($("#Email option:selected").val().length > 0)
      emailId = $("#Email option:selected").val();

    if (contactId == '' && emailId == '') {
      //contactTypeIdList = '' + $.map($('#contactType option:selected'), function (e) { return $(e).val(); });
      // SCP68345: No 'Download' option for 'Contact Details' Report 
      contactTypeIdList = '' + contactTypeMetaIds.replace(/T/gi, '');
      if (contactTypeIdList.length <= 0) {
        subGroupIdList = '' + $.map($('#contactTypeSubGroup option:selected'), function (e) { return $(e).val(); });
        if (subGroupIdList.length <= 0) {
          groupIdList = '' + $.map($('#contactTypeGroup option:selected'), function (e) { return $(e).val(); });
        }
      }
    }
  }

  //Get sort information.
  var sortIds = '', sortOrder = '';
  $("#sortGrid select").each(function () {
    var id = $(this).attr('id');
    sortIds = sortIds + id + ',';
    sortOrder = sortOrder + $(this).val() + ',';
  });

  var r = sortIds.length - 1;
  if (r > 1)
    sortIds = sortIds.substring(0, r);
  r = sortOrder.length - 1;
  if (r > 1)
    sortOrder = sortOrder.substring(0, r);

  var searchCriteria = {
    userCategoryId: $("#userCategoryID").val(),
    reportType: $("#reportType").val(),
    metaIdList: metaIds,
    typeMetaIdList: contactTypeMetaIds,
    memberId: $("#Id").val(),
    countryId: countryId,
    isIch: $('#ich').prop('checked'),
    isAch: $('#ach').prop('checked'),
    isIata: $('#iata').prop('checked'),
    isDual: $('#dual').prop('checked'),
    isNonCh: $('#nonch').prop('checked'),
    contactId: contactId,
    emailId: emailId,
    groupIdList: groupIdList,
    subGroupIdList: subGroupIdList,
    contactTypeIdList: contactTypeIdList,
    sortIds: sortIds,
    sortOrder: sortOrder
  };
  return searchCriteria;
}

//Get post data for given search criteria.
function getPostData(searchCriteria) {
  return {
    userCategoryId: searchCriteria.userCategoryId,
    reportType: searchCriteria.reportType,
    metaIdList: searchCriteria.metaIdList,
    memberId: searchCriteria.memberId,
    countryId: searchCriteria.countryId,
    isIch: searchCriteria.isIch,
    isAch: searchCriteria.isAch,
    isIata: searchCriteria.isIata,
    isDual: searchCriteria.isDual,
    isNonCh: searchCriteria.isNonCh,
    contactId: searchCriteria.contactId,
    emailId: searchCriteria.emailId,
    groupIdList: searchCriteria.groupIdList,
    subGroupIdList: searchCriteria.subGroupIdList,
    contactTypeIdList: searchCriteria.contactTypeIdList,
    sortIds: searchCriteria.sortIds,
    sortOrder: searchCriteria.sortOrder
  };
}

//Function to get search results for given search criteria.
function searchMemberAndContactDetails(posturl, searchCriteria) {
  $.ajax({
    type: "POST",
    url: posturl,
    dataType: "json",
    data: getPostData(searchCriteria),
    success: function (response) {
      // this will give us an array of objects
      var searchResults = jQuery.parseJSON(response);

      if (searchCriteria.reportType == "3") {
        var myOptions = searchResults[0]['Html'];

        //If search result contains records then only show the download button.
        if (myOptions.length > 0)
          $("#btnDownloadPDF").show();
        else
          $("#btnDownloadPDF").hide();

        $('#addressLabelFormat').html(myOptions);
        //Show modal popup.
        $AddressLabelResultdialog.dialog('open');
      }
      else {
        $("#btnDownloadCSV").hide();
        ProcessSearchResult(searchResults, searchCriteria, posturl);
        //Show modal popup.
        $Resultdialog.dialog('open');

        // Set title of popup based on member/contact report.
        var title = 'Member Information';
        if (searchCriteria.reportType == "2") {
          title = 'Contact Information';
        }
        $Resultdialog.dialog("option", "title", title);
      }
    },
    error: function (xhr, textStatus, errorThrown) {
        alert(errorMessage);
    }   //      failure: function (data) {alert('failure');}
  });
}


//Show search results in grid format.
function ProcessSearchResult(searchResults, postData, posturl) {
  posturl = posturl.replace(/Search/gi, 'GetMyGridData');
  var colN = searchResults[0]['colNames'];
  var colM = searchResults[0]['colModel'];
  jQuery("#list").jqGrid({
    url: posturl,
    datatype: 'json',
    mtype: 'POST',
    colNames: colN,
    colModel: colM,
    pager: $('#pager'),
    height: 250,
    autowidth: true,
    rowNum: 10,
    rowList: [5, 10, 20, 50],
    scrollOffset: 70,
    viewrecords: true,
    loadComplete: function () {
      //If no records then hide the download button
      if ($("#gbox_list").width() < 450) {
          $("#list").setGridWidth(450);
      }

      //If 'allowToDownloadCSVFile' is set for member and search result contains records then only show the download button
      var recs = $("#list").getGridParam("records");

      // If report has records as well as it is member details report or contact details report (With AllowToDownloadCSVFile flag for member) then only show Download button
      if (($("#reportType").val() == '1' || ($("#allowToDownloadCSVFile").val() == "True" && $("#reportType").val() == '2')) && recs > 0)
        $("#btnDownloadCSV").show();

      $("#list").closest(".ui-jqgrid-bdiv").css({ 'overflow-y': 'scroll' });
      //TFS#9918 : IE:Version 11- Member/Contact Report - Text is overrided on Member Information Popup.
      $("#pager_center").width(297);  
    }
  }).navGrid($('#pager'), { edit: false, add: false, del: false, refresh: true, search: false });
}

function downloadReport() {

  var metaIds = '' + $.map($('#selectedFields option'), function (e) { return $(e).val(); });
  var contactTypeMetaIds = '' + $.map($('#selectedFields option[value^="T"]'), function (e) { return $(e).val(); });
  contactTypeMetaIds = contactTypeMetaIds.replace(/C/gi, '');
  if (metaIds.length == 0 && contactTypeMetaIds.length == 0 && $("#reportType").val() != '3') {
    alert('Minimum one field from the Available Fields list should be selected.');
    return false;
  }

  $("#selectedMetaList").val(metaIds + '#' + contactTypeMetaIds);

  //Get sort information.
  var sortIds = '', sortOrder = '';
  $("#sortGrid select").each(function () {
    var id = $(this).attr('id');
    sortIds = sortIds + id + ',';
    sortOrder = sortOrder + $(this).val() + ',';
  });

  var r = sortIds.length - 1;
  if (r > 1)
    sortIds = sortIds.substring(0, r);
  $("#sortIds").val(sortIds);
  r = sortOrder.length - 1;
  if (r > 1)
    sortOrder = sortOrder.substring(0, r);
  $("#sortOrder").val(sortOrder);

  setRequiredInformation();
  $('#formSearch').submit();
  return true;
}


function setRequiredInformation() {
    if ($("#rdMemberDetails").prop('checked'))
    $("#reportType").val('1');
    else if ($("#rdContactDetails").prop('checked') && $("#tabularFormat").prop('checked'))
    $("#reportType").val('2');
    else if ($("#rdContactDetails").prop('checked') && $("#addressLabelFormat").prop('checked'))
    $("#reportType").val('3');
  else
    $("#reportType").val('0');

}

function setSortGrid() {
  jQuery("#sortGrid").jqGrid({
    datatype: 'clientSide',
    colNames: ['Sort On', 'Sort Order'],
    colModel: [
        { name: 'sortOn', index: 'sortOn', sortable: false },
        { name: 'sortOrder', index: 'order', width: 100, align: 'left', sortable: false}],
    pager: $('#sortPager'),
    rowNum: 5,
    height: 150,
    width: 450,
    scroll: true,
    viewrecords: true
  });
}
function addRow() {
  $("#selectedFields option:selected").each(function () {
    var metaid = $(this).val();
    if ($("#sortGrid select[id='" + metaid + "']").length < 1) {
      var myrow = {
        sortOn: "<label title='" + $(this).text() + "'>" + $(this).text() + "</label>",
        sortOrder: "<select id='" + metaid + "'><option value='Asc'>Ascending</option><option value='Desc'>Descending</option></select>"
      };
      $("#sortGrid").addRowData(metaid, myrow);
    }
  });
}

function deleteRow() {
  var selrowId = $("#sortGrid").getGridParam('selrow');
  if (selrowId <= 0 || selrowId==null) {
    alert('Please select row to exclude.');
  }
  else {
    $("#sortGrid").delRowData(selrowId);
  }
}


//Clear elements int the given element name.
function clear_form_elements(ele) {
  $(ele).find(':input').each(function () {
    switch (this.type) {
      case 'select-multiple':
      case 'select-one':
      case 'text':
        $(this).val('');
        if (this.id == "CommercialName") {
          $("#Id").val('');
          onBlankMemberName();
        }
        break;
      case 'checkbox':
      case 'radio':
        if (this.name != "rdDetails") {
          if (this.id == "tabularFormat")
            this.checked = true;
          else
            this.checked = false;
        }
    }
  });
  $('#ach,#ich,#dual,#nonch').each(function () { $(this).removeAttr('disabled') });
}

//Ready function for query and download page.
function readyQueryAndDownload(urlGetAvailMemberMeta, urlGetContactDetails, urlGetContactTypes, urlGetContactTypeSubGroups) {

  availFieldURL = urlGetAvailMemberMeta;
  getContactDetailsURL = urlGetContactDetails;

  $('#rdMemberDetails').attr("checked", "checked");
  $('#rdContactDetails').removeAttr("checked");
  $('#contactDetails').hide();
  $('#contactDetailsSeparator').hide();

  //Allow to select ContactName or Email only if member criteria is given.
  $('#ContactName,#Email').focus(function (event) {
    if ($("#CommercialName").val().length < 1) {
      alert('Please specify the member name.');
    }
  });

  //If ach, ich or dual options are selected then disable the nonch option.
  $('#ach,#ich,#dual').change(function () {

    if ($(this).attr('id') == 'dual') {
        if ($('#dual').prop('checked')) {
        $("#nonch").attr('disabled', 'disabled');
        $("#ach").attr('disabled', 'disabled');
        $("#ich").attr('disabled', 'disabled');
      }
      else {
        $("#nonch").attr('disabled', false);
        $("#ach").attr('disabled', false);
        $("#ich").attr('disabled', false);
      }
      return;
    }

  if ($('#ach').prop('checked') || $('#ich').prop('checked')) {
      $("#nonch").attr('disabled', 'disabled');
      $("#dual").attr('disabled', 'disabled');
    }
    else {
        $("#nonch").attr('disabled', false);
        $("#dual").attr('disabled', false);
    }
  });

  //If nonch option is selected then disable the ach, ich and dual options.
  $('#nonch').change(function () {
      if ($(this).prop('checked')) {
      $("#ach").attr('disabled', 'disabled');
      $("#ich").attr('disabled', 'disabled');
      $("#dual").attr('disabled', 'disabled');
    }
    else {
        $("#ach").attr('disabled', false);
        $("#ich").attr('disabled', false);
        $("#dual").attr('disabled', false);
    }
  });

  //Show hide the member profile elements for address label format.
  $('input:radio[name=tabularFormat]').click(function () {
    if ($(this).attr("id") == 'tabularFormat') {
      $("#sortDiv").show();
      getAvailableFields(urlGetAvailMemberMeta, 2);
    }
    else {
      $("#sortDiv").hide();
      getAvailableFields(urlGetAvailMemberMeta, 3);
    }
    var optionToRemove = $('option', $('#selectedFields'));
    if (optionToRemove)
      optionToRemove.remove();
    $("#sortGrid").clearGridData();
  });

  //Show hide the contact details as per search criteria selection.
  $('input:radio[name=rdDetails]').click(function () {
    if ($(this).attr("id") == 'rdContactDetails') {
      $('#contactDetails').show();
      $('#contactDetailsSeparator').show();
      getAvailableFields(urlGetAvailMemberMeta, 2);
    }
    else {
      $('#contactDetails').hide();
      $('#contactDetailsSeparator').hide();
      getAvailableFields(urlGetAvailMemberMeta, 1);
    }
    var optionToRemove = $('option', $('#selectedFields'));
    if (optionToRemove)
      optionToRemove.remove();
    $("#sortGrid").clearGridData();
  });

}

function onMemberNameChange(id) {
  if (fetchedMemberId != id)
    getContactDetails(getContactDetailsURL, $("#Id").val());

  fetchedMemberId = $("#Id").val();

  $("#ContactName").removeAttr('disabled');
  $("#Email").removeAttr('disabled');

  var userCategoryID = $("#userCategoryID").val();
  var memberId = $("#MemberId").val();

  //If select the member name then repopulate the 'Available Fields'.
  if (userCategoryID == 4) {
    if (memberId == id && !isAvailableOwnFieldsPopulated) {
      populateFields();
      isAvailableOwnFieldsPopulated = true;
    }
    else if (memberId != id && isAvailableOwnFieldsPopulated) {
      populateFields();
      isAvailableOwnFieldsPopulated = false;
    }
  }
}

function onBlankMemberName() {
  $('#ContactName').attr('disabled', 'disabled');
  $('#ContactName').val('');
  $('#Email').attr('disabled', 'disabled');
  $('#Email').val('');

  //If clear the member name then repopulate the 'Available Fields'.
  if (isAvailableOwnFieldsPopulated)
    populateFields();
}

function populateFields() {
  var reportType = 1;
  var reportElement = $('input:radio[name=rdDetails]:checked');
  if (reportElement && reportElement.attr("id") == 'rdContactDetails')
    reportType = 2;

  getAvailableFields(availFieldURL, reportType);

  isAvailableOwnFieldsPopulated = false;
  var optionToRemove = $('option', $('#selectedFields'));
  if (optionToRemove)
    optionToRemove.remove();
  $("#sortGrid").clearGridData();
}