<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.EBillingConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.Calendar" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<script type="text/javascript">

  var $BillingMemberDSdialog;
  var $BilledMemberDSdialog;
  var $AchivingLocationsDialog;
  var $AchivingPayLocationsDialog;
  var $eBillingSaveAnywayPopup;

  var AssociationTypeValue = 0;  
  var LoggedInUserLocAssoType = 0;  
  var InconsistencyMessageId =0;
  var PayAssociationTypeValue = 0;
  var dbUnAssignPayableLocations = [];
  var dbAssignPayableLocations = [];
  var dbUnAssignReceivableLocations = [];
  var dbAssignReceivableLocations = []; 

  $(document).ready(function () {

  // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveEBilling").attr('disabled', 'disabled');   
    $("#btnSaveEBilling").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveEBilling").removeAttr('disabled'); 
     $("#btnSaveEBilling").addClass('ignoredirty');     
   }

  $('#firstClick').focus();
  	$BillingMemberDSdialog = $('<div></div>')
		.html($("#divBillingMemberDSReq"))
		.dialog({
		  autoOpen: false,
		  title: 'As a Billing Member DS Required for Invoices',
		  height: 250,
		  width: 500,
		  modal: true,
		  resizable: false,
           close: function(event, ui) {
            
                    $('#hiddenBillingCountryIdAdd').val($("#BillingCountiesToAdd").val());
                    $('#hiddenBillingCountryIdRemove').val($("#BillingCountiesToRemove").val());
                    $('#firstClick').focus();
    },
      open: function () {
       $('#BillingMemberDSSupportedByAtosFrom option').each(function(i, option){ $(option).remove(); })
       jQuery('#BillingMemberDSSupportedByAtosFromHidden option').clone().appendTo('#BillingMemberDSSupportedByAtosFrom'); 
      
       $('#BillingMemberDSSupportedByAtosTo option').each(function(i, option){ $(option).remove(); })
       jQuery('#BillingMemberDSSupportedByAtosToHidden option').clone().appendTo('#BillingMemberDSSupportedByAtosTo'); 
     
      if (($('#eBillingMemberId').val() > 0) && ($('#BillingMemberDSSupportedByAtosToDisabled option').length > 0))
      {
        var periodValue = $('#BillingFuturePeriod').val();
        if(!periodValue || periodValue == _periodFormat)
            $('#BillingFuturePeriod').val($('#nextPeriod').val());
        
        $('#billingFuturePeriod').show();
        }
      },

      buttons: {
		    Save: {
		      className: 'primaryButton',
		      text: 'Save',
		      click: function () {
              
		         if(SaveBillingDsRequiredCountries('<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>')) 
             {
                 // Set the form as dirty and close the dialog.
              $parentForm.setDirty();
             $(this).dialog('close');
             }
		      }
		    },
        Close: {
        className: 'secondaryButton',
        text: 'Close',
        click: function () {
          $(this).dialog('close');
        }
        }
		  }

		});

    $BilledMemberDSdialog = $('<div></div>')
		.html($("#divBilledMemberDsreq"))
		.dialog({
		  autoOpen: false,
		  title: 'As a Billed Member DS Required for Invoices',
		  height: 250,
		  width: 500,
		  modal: true,
		  resizable: false,
          close: function(event, ui) {
                $('#hiddenCountryIdAdd').val($("#BilledCountiesToAdd").val());
                $('#hiddenCountryIdRemove').val($("#BilledCountiesToRemove").val());
                 $('#secondClick').focus();
                },
      open: function () {
        $('#BilledMemberDSSupportedByAtosFrom option').each(function(i, option){ $(option).remove(); })
        jQuery('#BilledMemberDSSupportedByAtosFromHidden option').clone().appendTo('#BilledMemberDSSupportedByAtosFrom'); 
         $('#BilledMemberDSSupportedByAtosTo option').each(function(i, option){ $(option).remove(); })
        jQuery('#BilledMemberDSSupportedByAtosToHidden option').clone().appendTo('#BilledMemberDSSupportedByAtosTo'); 
        if (($('#eBillingMemberId').val() > 0) && ($('#BilledMemberDSSupportedByAtosToDisabled option').length > 0)) 
        {
          var periodValue = $('#BilledFuturePeriod').val();
          if(!periodValue || periodValue == _periodFormat)
            $('#BilledFuturePeriod').val($('#nextPeriod').val());

          $('#billedFuturePeriod').show();
       }

      },
         buttons: {
		    Save: {
		      className: 'primaryButton',
		      text: 'Save',
		      click: function () {
              
		         if(SaveBilledDsRequiredCountries('<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>')) 
             {
                 // Set the form as dirty and close the dialog.
          $parentForm.setDirty();
                $(this).dialog('close');
                
             }
		      }
		    },
        Close: {
        className: 'secondaryButton',
        text: 'Close',
        click: function () {
          $(this).dialog('close');
        
        }
        }
		  } 
		});

       //CMP#666: CMP #666: MISC Legal Archiving Per Location ID
       //None = 1,
       //AllLocation = 2,
       //SpecificLocation=3

       var footer = '<div id="dialog_footer">Changes will be saved only after click of button <B>Save e-Billing Configuration</B> in the page from which this popup was opened.</div>';

        $AchivingLocationsDialog = $('<div></div>')
    .html($("#divArchivingLocations"))
    .dialog({
        autoOpen: false,
        dialogClass: 'myDialog',
        title: 'Location Specifications for MISC Receivables Archiving',
        height: 450,
        width: 500,
        modal: true,
        resizable: false,
        close: function(event, ui) {

            $('#LocAssociationType').css({
                'display': 'none'
            });
            $('#locationListBox').css("display", "none");
        },
        create: function() {
            $(".myDialog").append(footer);      
            $("#ReceivableOk").css("visibility", "hidden");          
        },
        open: function() {
            <%if (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.Member) {%>
            $.ajax({
                type: "Get",
                url: '/Profile/LocationAssociation/GetLoggedinUserLocsAssociation',
                dataType: "json",
                async: false,
                success: function(response) {
                    // 0-None 2-Specific     
                    LoggedInUserLocAssoType = response;
                    if (response == '0' || response == '2') {
                        $("#radNone").prop('disabled', true);
                        $("#radAllLocation").prop('disabled', true);
                        $("#radSpecificLocation").prop('disabled', true);
                        $("#add").prop('disabled', true);
                        $("#addAll").prop('disabled', true);
                        $("#remove").prop('disabled', true);
                        $("#removeAll").prop('disabled', true);
                        $("#ReceivableOk").attr("style", "");
                        $("#ReceivableOk").css("visibility", "hidden");                       

                    }else
                    {
                    $("#ReceivableOk").attr("style", "");
                    $("#ReceivableOk").css("visibility", "true");
                    }

                },
                failure: function(response) {
                    showClientErrorMessage(response.Message);
                }
            });
            <%}else {%>
                             
                 $("#ReceivableOk").attr("style", "");
                 $("#ReceivableOk").css("visibility", "true");
            <%}%>
            $('#LocAssociationType').css({
                'display': 'none'
            });
            $('#locationListBox').css("display", "none");
            var memberId = $('#eBillingMemberId').val();            
            if (AssociationTypeValue > 0 && (dbUnAssignReceivableLocations.length > 0 || dbAssignReceivableLocations.length > 0)) {   
                    $("#AssociatedLocation").html('');
                    $("#UnAssociatedLocation").html('');
                                      
                $.each(dbUnAssignReceivableLocations, function(key, value) {
                   var loc = value.split("||");
                    var inHTML = '<option value="' + loc[0] + '">' + loc[1] + '</option>';
                    $("#UnAssociatedLocation").append(inHTML);
                });

                $.each(dbAssignReceivableLocations, function(key, value) {
                var loc = value.split("||");
                    var inHTML = '<option value="' + loc[0] + '">' + loc[1] + '</option>';
                    $("#AssociatedLocation").append(inHTML);
                });

                if (AssociationTypeValue == '1') {
                    $("#radNone").attr('checked', 'checked');
                    $("#radNone").focus();
                    $('#LocAssociationType').css({
                        'display': 'block'
                    });
                    $('#locationListBox').css("display", "none");
                } else if (AssociationTypeValue == '2') {
                    $("#radAllLocation").attr('checked', 'checked');
                    $("#radAllLocation").focus();
                    $('#LocAssociationType').css({
                        'display': 'block'
                    });
                    $('#locationListBox').css("display", "none");
                } else {
                    $("#radSpecificLocation").attr('checked', 'checked');
                    $("#radSpecificLocation").focus();
                    $('#locationListBox').css({
                        'display': 'block'
                    });
                    $('#LocAssociationType').css({
                        'display': 'block'
                    });
                }
                return;
            }


            $.ajax({
                type: "Get",
                url: '/Profile/LocationAssociation/GetArchivalLocations',
                data: {
                    memberId: memberId,
                    archivalType: 1
                },
                dataType: "json",
                success: function(response) {

                    $("#AssociatedLocation").html('');
                    $("#UnAssociatedLocation").html('');

                    if (response.length > 0) {

                        AssociationTypeValue = response[0].AssociationType;
                        if (AssociationTypeValue == '1') {
                            $("#radNone").attr('checked', 'checked');
                            $("#radNone").focus();
                            $('#LocAssociationType').css({
                                'display': 'block'
                            });
                            $('#locationListBox').css("display", "none");
                        } else if (AssociationTypeValue == '2') {
                            $("#radAllLocation").attr('checked', 'checked');
                            $("#radAllLocation").focus();
                            $('#LocAssociationType').css({
                                'display': 'block'
                            });
                            $('#locationListBox').css("display", "none");
                        } else {
                            $("#radSpecificLocation").attr('checked', 'checked');
                            $("#radSpecificLocation").focus();
                            $('#locationListBox').css({
                                'display': 'block'
                            });
                            $('#LocAssociationType').css({
                                'display': 'block'
                            });
                        }
                    }

                    for (var i = 0, l = response.length; i < l; i++) {
                        if (response[i].ArchivalLocId == '0') {
                            // Unassigned List Box                        
                            var inHTML = '<option value="' + response[i].LocationId + '">' + response[i].LocationName + '</option>';
                            $("#UnAssociatedLocation").append(inHTML);                            
                        } else {
                            // Assigned List Box
                            var inHTML = '<option value="' + response[i].LocationId + '">' + response[i].LocationName + '</option>';
                            $("#AssociatedLocation").append(inHTML);                            
                        }
                    }

                },
                failure: function(response) {
                    showClientErrorMessage(response.Message);
                }
            });
        },

        buttons: {
            Save: {
                class: 'primaryButton',
                text: 'OK',
                id: 'ReceivableOk',
                click: function() {

                    <%if ( SessionUtil.MemberId > 0) {%>
                    if (LoggedInUserLocAssoType == '0' || LoggedInUserLocAssoType == '2') {
                        $(this).dialog('close');
                        return;
                    }
                    <%}%>

                    var selectedLocationIds = '00';

                    if (AssociationTypeValue == 3) {
                        var selectedLocationIds = '';
                        $("#AssociatedLocation").each(function() {
                            $('option', this).each(function() {
                                selectedLocationIds = selectedLocationIds + ',' + $(this).val();
                            });
                        });
                        if (selectedLocationIds == '') {
                            alert('Archiving should be defined for at least one Location ID if “Specific Location(s)” is chosen.');
                            return false;
                        }
                    }


                      dbUnAssignReceivableLocations.length=0;
                      $("#UnAssociatedLocation").each(function() {
                            $('option', this).each(function() {
                              var loc = $(this).val() + "||"+ $(this).text();
                             dbUnAssignReceivableLocations.push(loc);                                
                            });
                        });

                        dbAssignReceivableLocations.length=0;
                        $("#AssociatedLocation").each(function() {
                            $('option', this).each(function() {
                                var loc = $(this).val() + "||"+ $(this).text();
                                dbAssignReceivableLocations.push(loc);
                            });
                        });


                    $('#MiscRecArchivingLocs').val(selectedLocationIds);
                    $('#RecAssociationType').val(AssociationTypeValue);
                    // Set the form as dirty and close the dialog.                 
                    $parentForm.setDirty();
                    $(this).dialog('close');

                }
            },
            Close: {
                class: 'secondaryButton',
                text: 'Cancel',
                click: function() {
                    $(this).dialog('close');
                }
            }
        }
    });

        // Payable Popup
$AchivingPayLocationsDialog = $('<div></div>')
    .html($("#divArchivingPayLocations"))
    .dialog({
        autoOpen: false,
        dialogClass: 'myDialogPay',
        title: 'Location Specifications for MISC Payables Archiving',
        height: 450,
        width: 500,
        modal: true,
        resizable: false,
        close: function(event, ui) {

            $('#LocAssociationTypePay').css({
                'display': 'none'
            });
            $('#locationListBoxPay').css("display", "none");
        },
        create: function() {
            $(".myDialogPay").append(footer);
            $("#PayableOk").css("display", "none");
        },
        open: function() {

            <%if ( SessionUtil.MemberId > 0) {%>
            $.ajax({
                type: "Get",
                url: '/Profile/LocationAssociation/GetLoggedinUserLocsAssociation',
                dataType: "json",
                async: false,
                success: function(response) {
                    // 0-None 2-Specific   
                    LoggedInUserLocAssoType = response;
                    if (response == '0' || response == '2') {
                        $("#radNonePay").prop('disabled', true);
                        $("#radAllLocationPay").prop('disabled', true);
                        $("#radSpecificLocationPay").prop('disabled', true);
                        $("#addPay").prop('disabled', true);
                        $("#addAllPay").prop('disabled', true);
                        $("#removePay").prop('disabled', true);
                        $("#removeAllPay").prop('disabled', true);
                        $("#PayableOk").attr("style", "");
                        $("#PayableOk").css("visibility", "hidden");   
                    }
                    else
                    {
                    $("#PayableOk").attr("style", "");
                    $("#PayableOk").css("visibility", "true");   
                    }

                },
                failure: function(response) {
                    showClientErrorMessage(response.Message);
                }
            });
            <%} else { %>
                     $("#PayableOk").attr("style", "");
                    $("#PayableOk").css("visibility", "true"); 
            <%}  %>
            $('#LocAssociationTypePay').css({
                'display': 'none'
            });
            $('#locationListBoxPay').css("display", "none");
            var memberId = $('#eBillingMemberId').val();            
            if (PayAssociationTypeValue > 0 && (dbUnAssignPayableLocations.length > 0 || dbAssignPayableLocations.length > 0)) {
                    $("#AssociatedPayLocation").html('');
                    $("#UnAssociatedPayLocation").html('');
                $.each(dbUnAssignPayableLocations, function(key, value) {
                       var loc = value.split("||");
                    var inHTML = '<option value="' + loc[0] + '">' + loc[1] + '</option>';
                    $("#UnAssociatedPayLocation").append(inHTML);
                });

                $.each(dbAssignPayableLocations, function(key, value) {
                    var loc = value.split("||");
                    var inHTML = '<option value="' + loc[0] + '">' + loc[1] + '</option>';
                    $("#AssociatedPayLocation").append(inHTML);
                });

                if (PayAssociationTypeValue == '1') {
                    $("#radNonePay").attr('checked', 'checked');
                    $("#radNonePay").focus();
                    $('#LocAssociationTypePay').css({
                        'display': 'block'
                    });
                    $('#locationListBoxPay').css("display", "none");
                } else if (PayAssociationTypeValue == '2') {
                    $("#radAllLocationPay").attr('checked', 'checked');
                    $("#radAllLocationPay").focus();
                    $('#LocAssociationTypePay').css({
                        'display': 'block'
                    });
                    $('#locationListBoxPay').css("display", "none");
                } else {
                    $("#radSpecificLocationPay").attr('checked', 'checked');
                    $("#radSpecificLocationPay").focus();
                    $('#locationListBoxPay').css({
                        'display': 'block'
                    });
                    $('#LocAssociationTypePay').css({
                        'display': 'block'
                    });
                }

                return;
            }



            $.ajax({
                type: "Get",
                url: '/Profile/LocationAssociation/GetArchivalLocations',
                data: {
                    memberId: memberId,
                    archivalType: 2
                },
                dataType: "json",
                success: function(response) {

                    $("#AssociatedPayLocation").html('');
                    $("#UnAssociatedPayLocation").html('');

                    if (response.length > 0) {

                        PayAssociationTypeValue = response[0].AssociationType;
                        if (PayAssociationTypeValue == '1') {
                            $("#radNonePay").attr('checked', 'checked');
                            $("#radNonePay").focus();
                            $('#LocAssociationTypePay').css({
                                'display': 'block'
                            });
                            $('#locationListBoxPay').css("display", "none");
                        } else if (PayAssociationTypeValue == '2') {
                            $("#radAllLocationPay").attr('checked', 'checked');
                            $("#radAllLocationPay").focus();
                            $('#LocAssociationTypePay').css({
                                'display': 'block'
                            });
                            $('#locationListBoxPay').css("display", "none");
                        } else {
                            $("#radSpecificLocationPay").attr('checked', 'checked');
                            $("#radSpecificLocationPay").focus();
                            $('#locationListBoxPay').css({
                                'display': 'block'
                            });
                            $('#LocAssociationTypePay').css({
                                'display': 'block'
                            });
                        }
                    }

                    for (var i = 0, l = response.length; i < l; i++) {
                        if (response[i].ArchivalLocId == '0') {
                            // Unassigned List Box                        
                            var inHTML = '<option value="' + response[i].LocationId + '">' + response[i].LocationName + '</option>';
                            $("#UnAssociatedPayLocation").append(inHTML);
                           
                        } else {
                            // Assigned List Box
                            var inHTML = '<option value="' + response[i].LocationId + '">' + response[i].LocationName + '</option>';
                            $("#AssociatedPayLocation").append(inHTML);                            
                        }
                    }

                },
                failure: function(response) {
                    showClientErrorMessage(response.Message);
                }
            });

        },

        buttons: {
            Save: {
                class: 'primaryButton',
                text: 'OK',
                id: 'PayableOk',
                click: function() {
                    <%if ( SessionUtil.MemberId > 0) {%>
                    if (LoggedInUserLocAssoType == '0' || LoggedInUserLocAssoType == '2') {
                        $(this).dialog('close');
                        return;
                    }
                    <%}%>

                    var selectedLocationIds = '00';

                    if (PayAssociationTypeValue == 3) {
                        var selectedLocationIds = '';
                        $("#AssociatedPayLocation").each(function() {
                            $('option', this).each(function() {
                                selectedLocationIds = selectedLocationIds + ',' + $(this).val();
                            });
                        });
                        if (selectedLocationIds == '') {
                            alert('Archiving should be defined for at least one Location ID if “Specific Location(s)” is chosen.');
                            return false;
                        }
                    }
                      
                      dbUnAssignPayableLocations.length=0;
                      $("#UnAssociatedPayLocation").each(function() {
                            $('option', this).each(function() {
                             var Payloc1 = $(this).val() + "||"+ $(this).text();
                             dbUnAssignPayableLocations.push(Payloc1);                                
                            });
                        });

                        dbAssignPayableLocations.length=0;
                        $("#AssociatedPayLocation").each(function() {
                            $('option', this).each(function() {
                                var Payloc2 = $(this).val() + "||"+ $(this).text();
                                dbAssignPayableLocations.push(Payloc2);
                            });
                        });



                    $('#MiscPayArchivingLocs').val(selectedLocationIds);
                    $('#PayAssociationType').val(PayAssociationTypeValue);
                    // Set the form as dirty and close the dialog.                 
                    $parentForm.setDirty();
                    $(this).dialog('close');

                }
            },
            Close: {
                class: 'secondaryButton',
                text: 'Cancel',
                click: function() {
                    $(this).dialog('close');
                }
            }
        }
    });      
        //Operation in Tab E-billing And Its Relationship with Rec/Payable Popups
        $eBillingSaveAnywayPopup = $('<div></div>')		
        .html($("#divArchivalLocsEbilling"))
		.dialog({
		  autoOpen: false,
		  title: 'Warning',
		  height: 170,
		  width: 450,
		  modal: true,          
		  resizable: false,           
          open: function () {  
            if (InconsistencyMessageId == '1') {
                    $('#WarningMsgOne').css({ 'display': 'block' });
                    $('#WarningMsgTwo').css({ 'display': 'none' });
                    $('#WarningMsgThree').css({ 'display': 'none' });    
                    $('#ErrorMsg').css({ 'display': 'none' });    
                } else if (InconsistencyMessageId == '2') {
                   $('#WarningMsgOne').css({ 'display': 'none' });
                    $('#WarningMsgTwo').css({ 'display': 'block' });
                    $('#WarningMsgThree').css({ 'display': 'none' });   
                    $('#ErrorMsg').css({ 'display': 'none' });    
                } else if (InconsistencyMessageId == '3') {
                    $('#WarningMsgOne').css({ 'display': 'none' });
                    $('#WarningMsgTwo').css({ 'display': 'none' });
                    $('#WarningMsgThree').css({ 'display': 'block' });   
                    $('#ErrorMsg').css({ 'display': 'none' });    
                } else{
                   $('#WarningMsgOne').css({ 'display': 'none' });
                    $('#WarningMsgTwo').css({ 'display': 'none' });
                    $('#WarningMsgThree').css({ 'display': 'none' });   
                    $('#ErrorMsg').css({ 'display': 'block' });    
                }                
          },
         buttons: {
                Save: { class: 'primaryButton', text: 'Save Anyway',
            click: function () {
                         $(this).dialog('close');
                         $("#eBilling").submit();                                                          
		          }
		            },
               Close: { class: 'secondaryButton', text: 'Cancel',
              click: function () {           
                        $(this).dialog('close');
                                }
                      }
                }
		});
       
        // Code End

    <%if (Model.MemberId > 0)
      {%>
        $(".futureEditLink").show();
       // $(".LegalArchRequiredfutureEditLink").show();
        $('.currentFieldValue').attr("disabled", true);

          //updated by upendra yadav as new filed added
          var IsLegalArchievingRequired = $("#IsLegalArchievingRequired").val();
          if (IsLegalArchievingRequired == 'True') {
           $(".LegalArchRequiredfutureEditLink").show();

         
              $('#IncludeListingsPaxRecArch').removeAttr('disabled');
              $('#IncludeListingsPaxPayArch').removeAttr('disabled');
              $('#IncludeListingsCgoRecArch').removeAttr('disabled');
              $('#IncludeListingsCgoPayArch').removeAttr('disabled');
              $('#IncludeListingsMiscRecArch').removeAttr('disabled');
              $('#IncludeListingsMiscPayArch').removeAttr('disabled');
              $('#IncludeListingsUatpRecArch').removeAttr('disabled');
              $('#IncludeListingsUatpPayArch').removeAttr('disabled'); 

          }
          else{
           $(".LegalArchRequiredfutureEditLink").hide();}
       
    <%}%>

      <%if (!string.IsNullOrEmpty(Model.DSReqCountriesAsBillingFuturePeriod))
      {%>
        $("#BillingMemberFutureDateInd").show();
      <%}%>

      <%if (!string.IsNullOrEmpty(Model.DSReqCountriesAsBilledFuturePeriod))
      {%>
        $("#BilledMemberFutureDateInd").show();
      <%}%>

      setFutureUpdateFieldValue("#BillingMemberDSSupportedByAtosToDisabled", "#BillingMemberDSSupportedByAtosTo","#BillingFutureValue");
      setFutureUpdateFieldValue("#BilledMemberDSSupportedByAtosToDisabled", "#BilledMemberDSSupportedByAtosTo","#BilledFutureValue");
        
      //When Legal Archiving period value is true , only the enable Optional legal archiving for payables receivables checkboxes
      $('#IsLegalArchievingRequired').change(function () { 
      if ($(this).prop('checked') == true) {
        $('#IsPayableLegalArchievingOptional').removeAttr('disabled');
        $('#IsRecievableLegalArchievingOptional').removeAttr('disabled');        
      }
      else {
        $('#IsPayableLegalArchievingOptional').attr('disabled', 'disabled');
        $('#IsRecievableLegalArchievingOptional').attr('disabled', 'disabled');
        $("#IsPayableLegalArchievingOptional").attr({ "checked": false });
        $("#IsRecievableLegalArchievingOptional").attr({ "checked": false });
        $("#PayableLegalArchievingPeriod").attr('readonly', 'true');
        $("#RecievableLegalArchievingPeriod").attr('readonly', 'true');
         $("#PayableLegalArchievingPeriod").val("");
          $("#RecievableLegalArchievingPeriod").val("");
        
      }

    });
  });

   function showBillingMemberDSReqDialog() {
    $('#BillingFuturePeriod').watermark(_periodFormat);
    $BillingMemberDSdialog.dialog('open');
    return false;
  }

   function showBilledMemberDSReqDialog() {
    $('#BilledFuturePeriod').watermark(_periodFormat);
    $BilledMemberDSdialog.dialog('open');
    return false;
  }

   function showArchivingLocationsDialog() {    
    $AchivingLocationsDialog.dialog('open');
    return false;
  }   

  function showArchivingPayLocationsDialog() {    
    $AchivingPayLocationsDialog.dialog('open');
    return false;
  }   


  $("input[name='AssociationType']").change(function () {
        AssociationTypeValue = $(this).val();
        if (AssociationTypeValue == "3") {
            $('#locationListBox').css({ 'display': 'block' });
        } else {
            $('#locationListBox').css("display", "none");
        }
    });

    $("#add").click(function () {
        var selected = $("#UnAssociatedLocation").find(':selected').val();
        $("#UnAssociatedLocation option:selected").appendTo("#AssociatedLocation");
        SortListItems("AssociatedLocation");
        $("#AssociatedLocation").val(selected);
    });

    //If you want to move all item from availableFields to selectedFields
    $("#addAll").click(function () {
        $("#UnAssociatedLocation option").appendTo("#AssociatedLocation");
        SortListItems("AssociatedLocation");
    });

    //If you want to remove selected item from selectedFields to availableFields
    $("#remove").click(function () {
        var selected = $("#AssociatedLocation").find(':selected').val();
        $("#AssociatedLocation option:selected").each(function () {
            $(this).appendTo("#UnAssociatedLocation");
        });
        SortListItems("UnAssociatedLocation");
        $("#UnAssociatedLocation").val(selected);
    });

    //If you want to remove all items from selectedFields to availableFields
    $("#removeAll").click(function () {
        $("#AssociatedLocation option").appendTo("#UnAssociatedLocation");
        SortListItems("UnAssociatedLocation");
    });


    $("input[name='PayAssociationType']").change(function () {
        PayAssociationTypeValue = $(this).val();
        if (PayAssociationTypeValue == "3") {
            $('#locationListBoxPay').css({ 'display': 'block' });
        } else {
            $('#locationListBoxPay').css("display", "none");
        }
    });

    $("#addPay").click(function () {
        var selected = $("#UnAssociatedPayLocation").find(':selected').val();
        $("#UnAssociatedPayLocation option:selected").appendTo("#AssociatedPayLocation");
        SortListItems("AssociatedPayLocation");
        $("#AssociatedPayLocation").val(selected);
    });

    //If you want to move all item from availableFields to selectedFields
    $("#addAllPay").click(function () {
        $("#UnAssociatedPayLocation option").appendTo("#AssociatedPayLocation");
        SortListItems("AssociatedPayLocation");
    });

    //If you want to remove selected item from selectedFields to availableFields
    $("#removePay").click(function () {
        var selected = $("#AssociatedPayLocation").find(':selected').val();
        $("#AssociatedPayLocation option:selected").each(function () {
            $(this).appendTo("#UnAssociatedPayLocation");
        });
        SortListItems("UnAssociatedPayLocation");
        $("#UnAssociatedPayLocation").val(selected);
    });

    //If you want to remove all items from selectedFields to availableFields
    $("#removeAllPay").click(function () {
        $("#AssociatedPayLocation option").appendTo("#UnAssociatedPayLocation");
        SortListItems("UnAssociatedPayLocation");
    });



    function SortListItems(ListBoxId) {

        var Location = ["MAIN", "UATP"];

        var listIntLocation = $("#" + ListBoxId + " option");
        var listStringLocation = listIntLocation;

        for (var i = listStringLocation.length - 1; i >= 0; --i) {
            var itemText = listStringLocation[i].innerHTML;
            itemText = itemText.split('-')[0];

            var found = $.inArray(itemText.toUpperCase(), Location) > -1;

            if (!found) {

                listStringLocation = jQuery.grep(listStringLocation, function (value) {
                    return value != listStringLocation[i];
                });

            }
        }

        for (var i = listIntLocation.length - 1; i >= 0; --i) {
            var itemText = listIntLocation[i].innerHTML;
            itemText = itemText.split('-')[0];
            var found = $.inArray(itemText.toUpperCase(), Location) > -1;

            if (found) {
                listIntLocation = jQuery.grep(listIntLocation, function (value) {
                    return value != listIntLocation[i];
                });

            }
        }

        listStringLocation.sort(function (a, b) {
            var firstItem = a.innerHTML.split('-')[0];
            var secondItem = b.innerHTML.split('-')[0];
            if (firstItem > secondItem) return 1;
            else if (firstItem < secondItem) return -1;
        });


        listIntLocation.sort(function (a, b) {
            var firstItem = a.innerHTML.split('-')[0];
            var secondItem = b.innerHTML.split('-')[0];
            if (parseInt(firstItem) > parseInt(secondItem)) return 1;
            else if (parseInt(firstItem) < parseInt(secondItem)) return -1;

        });
        $("#" + ListBoxId).empty().append(listStringLocation);
        $("#" + ListBoxId).append(listIntLocation);
    }

     $("#btnSaveEBilling").click(function() {
     var returnType = false;
     var archReqMiscRecInvFuture = $('#LegalArchRequiredforMiscRecInvFutureValue').val();
     var archReqMiscPayInvFuture = $('#LegalArchRequiredforMiscPayInvFutureValue').val();
     var archRequiredforMiscRecInvCurrent = $('#LegalArchRequiredforMiscRecInv').prop('checked');
     var archRequiredforMiscPayInvCurrent = $('#LegalArchRequiredforMiscPayInv').prop('checked');

     var memberId = $('#eBillingMemberId').val();
     $.ajax({
         type: "Get",
         url: '/Profile/LocationAssociation/GetArchivalLocsInconsistency',
         data: {
             memberId: memberId,
             archReqMiscRecInvCurrent:archRequiredforMiscRecInvCurrent,
             archReqMiscPayInvCurrent:archRequiredforMiscPayInvCurrent,
             archReqMiscRecInvFuture: archReqMiscRecInvFuture,
             archReqMiscPayInvFuture: archReqMiscPayInvFuture,
             recAssociationType: AssociationTypeValue,
             payAssociationType: PayAssociationTypeValue
         },
         dataType: "json",
         async: false,
         success: function(response) {
             if (response != 0) {
                 InconsistencyMessageId = response;
                 $eBillingSaveAnywayPopup.dialog('open');
                 returnType = false;
             } else {
                 returnType = true;
             }
         },
         failure: function(response) {
             showClientErrorMessage(response.Message);
         }
     });
     if (returnType) {
         return true;
     } else {
         return false;
     }
 });
</script>
<%
    using (Html.BeginForm("EBilling", "Member", FormMethod.Post, new { id = "eBilling" }))
    {%>
    <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
    <%
        if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
        {%>
    <div class="fieldContainer horizontalFlow">
        <h2>
            Legal Services</h2>
        <div class="bottomLine">
            <div class="wrappingLabel">
                <%:Html.HiddenFor(eBilling => eBilling.IsDigitalSignatureRequired)%>
                <label>
                    Digital Signature Application Service:</label>
                <div class="subscriptionRequired">
                    <%:Model.IsDigitalSignatureRequiredDisplay%></div>
            </div>
            <div class="wrappingLabel">
                <%:Html.HiddenFor(eBilling => eBilling.IsDsVerificationRequired)%>
                <label>
                    Digital Signature Verification Service:</label>
                <div class="subscriptionRequired">
                    <%:Model.IsDsVerificationRequiredDisplay%></div>
            </div>
            <div>
                <label class="wrappingLabel">
                    Receivable Invoices<br />
                    DS to be applied for;</label>
                <div>
                    <%=Html.ListBox("BillingMemberDSSupportedByAtosToDisabled", (MultiSelectList) ViewData["currentdsRequiredBillingCountries"], new { width = "500", size = "6", disabled = true })%>
                    <%=Html.ListBox("BillingMemberDSSupportedByAtosFromHidden", (MultiSelectList) ViewData["DSSupportedBillingCountryListFrom"], new { @class = "hidden", size = "8", disabled = true })%>
                    <%=Html.ListBox("BillingMemberDSSupportedByAtosToHidden", (MultiSelectList) ViewData["FutureBillingDSSupportedCountryTo"], new { @class = "hidden", size = "8", disabled = true })%>
                    <image id="BillingMemberFutureDateInd" src="<%=Url.Content("~/Content/Images/Exclamation.gif")%>"
                        class="hidden" onclick="displayFutureUpdateDetails('#Billing', 1, 19);" />
                    <%=Html.HiddenFor(eBilling => eBilling.CountryList)%>
                    <%=Html.HiddenFor(eBilling => eBilling.DSReqCountriesAsBillingFuturePeriod)%>
                    <input type="hidden" id="BillingFutureValue" />
                </div>
                <a class="ignoredirty" href="#" id="firstClick" onclick="return showBillingMemberDSReqDialog()">
                    Add/Edit</a>
            </div>
            <div>
                <label class="wrappingLabel">
                    Payable invoices<br />
                    DS to be applied for;</label>
                <div>
                    <%=Html.ListBox("BilledMemberDSSupportedByAtosToDisabled", (MultiSelectList) ViewData["currentdsRequiredBilledCountries"], new { size = "6", disabled = true })%>
                    <%=Html.ListBox("BilledMemberDSSupportedByAtosFromHidden", (MultiSelectList) ViewData["DSSupportedBilledCountryListFrom"], new { @class = "hidden", size = "8", disabled = true })%>
                    <%=Html.ListBox("BilledMemberDSSupportedByAtosToHidden", (MultiSelectList) ViewData["FutureBilledDSSupportedCountryTo"], new { @class = "hidden", size = "8", disabled = true })%>
                    <image id="BilledMemberFutureDateInd" src="<%=Url.Content("~/Content/Images/Exclamation.gif")%>"
                        class="hidden" onclick="displayFutureUpdateDetails('#Billed', 1, 19);" />
                    <%=Html.HiddenFor(eBilling => eBilling.DSReqCountriesAsBilledFuturePeriod)%>
                    <input type="hidden" id="BilledFutureValue" />
                </div>
                <a class="ignoredirty" href="#" id="secondClick" onclick="return showBilledMemberDSReqDialog()">
                    Add/Edit</a>
            </div>
        </div>
    </div>
    <%
      }%>
    <div class="fieldContainer horizontalFlow">
        <h2>
            Default Invoice Footer Text</h2>
        <%:Html.ProfileFieldFor(ebilling => ebilling.LegalText,
                                           "The below text will appear on all invoices unless overridden by the Invoice data supplied in IS format or by the details in the Location tab",
                                           SessionUtil.UserCategory,
                                           new Dictionary<string, object> { { "maxLength", "700" }, { "id", "LegalText" }, { "class", "currentFieldValue" }, { "rows", "5" }, { "cols", "138" } },
                                           null,
                                           new FutureUpdate
                                             {
                                               FieldId = "LegalText",
                                               FieldName = "LegalText",
                                               FieldType = 5,
                                               CurrentValue = Model.LegalText,
                                               FutureValue = Model.LegalTextFutureValue,
                                               HasFuturePeriod = true,
                                               FuturePeriod = Model.LegalTextFuturePeriod
                                             })%>
    </div>
    <div class="fieldContainer horizontalFlow">
        <%if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
          {%>
        <div class="horizontalFlow">
            <h2>
                Legal Archiving</h2>
            <div class="wrappingLabel">
                <%:Html.HiddenFor(eBilling => eBilling.IsLegalArchievingRequired)%>
                <label>
                    Legal Archiving Service:</label>
                <div class="subscriptionRequired" style="width: 50%;">
                    <%:Model.IsLegalArchievingRequiredDisplay%></div>
            </div>
        </div>
        <%
          }%>
        <%--  <div>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IsPayableLegalArchievingOptional, "Optional Legal Archiving - Payables", SessionUtil.UserCategory, new Dictionary<string, object> { { "disabled", "disabled" } }, null, null, new Dictionary<string, object> { { "class", "wrappingLabel" } })%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.PayableLegalArchievingPeriod, "Legal Archiving Period - Payables(months)", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "numericOnly" }, { "maxLength", "3" }, { "readOnly", "readOnly" } }, null, null, new Dictionary<string, object> { { "class", "wrappingLabel" } })%>
            months
            <%: Html.ProfileFieldFor(eBilling => eBilling.IsRecievableLegalArchievingOptional, "Optional Legal Archiving - Receivables", SessionUtil.UserCategory, new Dictionary<string, object> { { "disabled", "disabled" } }, null, null, new Dictionary<string, object> { { "class", "wrappingLabel" } })%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.RecievableLegalArchievingPeriod, "Legal Archiving Period - Receivables(months)", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "numericOnly" }, { "maxLength", "3" }, { "readOnly", "readOnly" } }, null, null, new Dictionary<string, object> { { "class", "wrappingLabel" } })%>
        </div>--%>
        <div>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforPaxRecInv, "Legal Archiving Required for PAX Receivables Invoices", 
                                                      SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforPaxRecInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforPaxRecInv",
                                                          FieldName = "LegalArchRequiredforPaxRecInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforPaxRecInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforPaxRecInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforPaxRecInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforPaxPayInv, "Legal Archiving Required for PAX Payables Invoices",
                                                      SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforPaxPayInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforPaxPayInv",
                                                          FieldName = "LegalArchRequiredforPaxPayInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforPaxPayInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforPaxPayInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforPaxPayInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforCgoRecInv, "Legal Archiving Required for CGO Receivables Invoices",
                                                      SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforCgoRecInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforCgoRecInv",
                                                          FieldName = "LegalArchRequiredforCgoRecInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforCgoRecInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforCgoRecInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforCgoRecInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforCgoPayInv, "Legal Archiving Required for CGO Payables Invoices", 
                                                        SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforCgoPayInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforCgoPayInv",
                                                          FieldName = "LegalArchRequiredforCgoPayInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforCgoPayInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforCgoPayInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforCgoPayInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
        </div>
        <div>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforMiscRecInv, "Legal Archiving Required for MISC Receivables Invoices", 
                                                       SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforMiscRecInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforMiscRecInv",
                                                          FieldName = "LegalArchRequiredforMiscRecInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforMiscRecInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforMiscRecInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforMiscRecInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforMiscPayInv, "Legal Archiving Required for MISC Payables Invoices", 
                                                       SessionUtil.UserCategory,
                                                                      new Dictionary<string, object> { { "id", "LegalArchRequiredforMiscPayInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforMiscPayInv",
                                                          FieldName = "LegalArchRequiredforMiscPayInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforMiscPayInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforMiscPayInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforMiscPayInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforUatpRecInv, "Legal Archiving Required for UATP Receivables Invoices", 
                                                   SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforUatpRecInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforUatpRecInv",
                                                          FieldName = "LegalArchRequiredforUatpRecInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforUatpRecInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforUatpRecInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforUatpRecInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.LegalArchRequiredforUatpPayInv, "Legal Archiving Required for UATP Payables Invoices",
                                                      SessionUtil.UserCategory,
                                                              new Dictionary<string, object> { { "id", "LegalArchRequiredforUatpPayInv" }, { "disabled", "disabled" } },
                                                      null,
                                                      new FutureUpdate
                                                      {
                                                          FieldId = "LegalArchRequiredforUatpPayInv",
                                                          FieldName = "LegalArchRequiredforUatpPayInv",
                                                          FieldType = 2,
                                                          CurrentValue = Model.LegalArchRequiredforUatpPayInv.ToString(),
                                                          FutureValue = Model.LegalArchRequiredforUatpPayInvFutureValue.ToString(),
                                                          HasFuturePeriod = true,
                                                          FuturePeriod = Model.LegalArchRequiredforUatpPayInvFuturePeriod,
                                                          EditLinkClass = "LegalArchRequiredfutureEditLink"
                                                      },
                                                      new Dictionary<string, object> { { "class", "wrappingLabel" } }
                                                      )%>
        </div>
        <div>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsPaxRecArch, "Include Listings in PAX Receivables Archives", 
                                                   SessionUtil.UserCategory, 
                                                   new Dictionary<string, object> {{ "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsPaxPayArch, "Include Listings in PAX Payables Archives", 
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsCgoRecArch, "Include Listings in CGO Receivables Archives",
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsCgoPayArch, "Include Listings in CGO Payables Archives",
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
        </div>
        <div>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsMiscRecArch, "Include Listings in MISC Receivables Archives", 
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsMiscPayArch, "Include Listings in MISC Payables Archives", 
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsUatpRecArch, "Include Listings in UATP Receivables Archives", 
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
            <%: Html.ProfileFieldFor(eBilling => eBilling.IncludeListingsUatpPayArch, "Include Listings in UATP Payables Archives", 
                                                   SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "disabled", "disabled" } },
                                                   null,
                                                   null,
                                                   new Dictionary<string, object> { { "class", "wrappingLabel" }})%>
        </div>
        <%
        if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.Member) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
        {%>
        <div>
            <div style="line-height: 40px; width: 23%;">
                <a class="ignoredirty" href="#" id="miscReceivableArcLoc" onclick="return showArchivingLocationsDialog();">
                MISC Receivables Archiving Required for Locations </a>
            </div>
            <div style="line-height: 40px; width: 23%;">
                <a class="ignoredirty" href="#" id="miscPayableArcLoc" onclick="return showArchivingPayLocationsDialog();">
                MISC Payables Archiving Required for Locations </a>
            </div>
        </div>
        <% } %>
        </div>
    <div class="fieldContainer horizontalFlow">
        <h2>
            User Identification in Data Changes Logs</h2>
        <%: Html.ProfileFieldFor(eBilling => eBilling.IsHideUserNameInAuditTrail, "Hide User Names In Audit Trail", SessionUtil.UserCategory, null,new Dictionary<string, object> { { "class", "bottomLine" } })%>
    </div>
    <div class="fieldContainer horizontalFlow">
        <div class="bottomLine">
            <h2>
                Receipt of Files in iiNET Accounts</h2>
            <table>
                <thead align="center" valign="middle">
                    <tr>
                        <td style="width: 150px;">
                        </td>
                        <td style="width: 200px; text-align: left;">
                            Account ID
                        </td>
                        <td style="width: 200px; text-align: left;" class="wrappingLabel">
                            Change Information for
                            <br />
                            Reference Data Updates
                        </td>
                        <td style="width: 150px; text-align: left;">
                            Complete Reference Data
                        </td>
                        <td style="width: 200px; text-align: left;">
                            Complete Contacts Data
                        </td>
                    </tr>
                </thead>
                <tbody align="center" valign="middle">
                    <tr>
                        <td style="text-align: left;">
                            Passenger
                        </td>
                        <td style="text-align: left;">
                            <%:Html.TextBoxFor(eBilling => eBilling.IinetAccountIdPax, new { @readonly = "true" })%>
                        </td>
                       
                        <td style="text-align: left;">
                             <%:Html.MemProfileDataChkBox("ChangeInfoRefDataPax", Model.ChangeInfoRefDataPax, Model.IinetAccountIdPax)%>
                        </td>
                        <td style="text-align: left;">
                            <%:Html.MemProfileDataChkBox("CompleteRefDataPax", Model.CompleteRefDataPax, Model.IinetAccountIdPax)%>
                        </td>
                        <td style="text-align: left;">
                           <%:Html.MemProfileDataChkBox("CompleteContactsDataPax", Model.CompleteContactsDataPax, Model.IinetAccountIdPax)%>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: left;">
                            Cargo
                        </td>
                        <td style="text-align: left;">
                          <%:Html.TextBoxFor(eBilling => eBilling.IinetAccountIdCgo, new { @readonly = "true" })%>
                        </td>
                      <td style="text-align: left;">
                        <%:Html.MemProfileDataChkBox("ChangeInfoRefDataCgo", Model.ChangeInfoRefDataCgo, Model.IinetAccountIdCgo)%>
                        </td>
                                               
                           <td style="text-align: left;">
                          <%:Html.MemProfileDataChkBox("CompleteRefDataCgo", Model.CompleteRefDataCgo, Model.IinetAccountIdCgo)%>
                        </td>
                       <td style="text-align: left;">
                         <%:Html.MemProfileDataChkBox("CompleteContactsDataCgo", Model.CompleteContactsDataCgo, Model.IinetAccountIdCgo)%>
                            
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: left;">
                            Miscellaneous
                        </td>
                        <td style="text-align: left;">
                          <%:Html.TextBoxFor(eBilling => eBilling.IinetAccountIdMisc, new { @readonly = "true" })%>
                        </td>
                    <td style="text-align: left;">
                       <%:Html.MemProfileDataChkBox("ChangeInfoRefDataMisc", Model.ChangeInfoRefDataMisc, Model.IinetAccountIdMisc)%>
                        </td>
                        <td style="text-align: left;">
                           <%:Html.MemProfileDataChkBox("CompleteRefDataMisc", Model.CompleteRefDataMisc, Model.IinetAccountIdMisc)%>
                        </td>
                        <td style="text-align: left;">
                                 <%:Html.MemProfileDataChkBox("CompleteContactsDataMisc", Model.CompleteContactsDataMisc, Model.IinetAccountIdMisc)%>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: left;">
                            UATP
                        </td>
                        <td style="text-align: left;">
                          <%:Html.TextBoxFor(eBilling => eBilling.IinetAccountIdUatp, new { @readonly = "true" })%>
                        </td>
                        <td style="text-align: left;">
                             <%:Html.MemProfileDataChkBox("ChangeInfoRefDataUatp", Model.ChangeInfoRefDataUatp, Model.IinetAccountIdUatp)%>
                        </td>
                        <td style="text-align: left;">
                             <%:Html.MemProfileDataChkBox("CompleteRefDataUatp", Model.CompleteRefDataUatp, Model.IinetAccountIdUatp)%>
                        </td>
                        <td style="text-align: left;">
                             <%:Html.MemProfileDataChkBox("CompleteContactsDataUatp", Model.CompleteContactsDataUatp, Model.IinetAccountIdUatp)%>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="fieldContainer verticalFlow ">
        <div class="halfWidthColumn">
            <div>
                <h2>
                    IS Contacts</h2>
                <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%=Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>','#divContactAssignmentSearchResult','E_BILLING','<%=Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile"}) %>',$('#selectedMemberId').val());">
                    View/Edit</a>
            </div>
        </div>
        <div class="clear" />
    </div>
    <div class="clear">
    </div>
</div>
<div class="buttonContainer">
    
    <div>
        <%= Html.HiddenFor(eBilling => eBilling.BilledCountiesToAdd)%>
        <%= Html.HiddenFor(eBilling => eBilling.BilledCountiesToRemove)%>
        <%= Html.HiddenFor(eBilling => eBilling.BillingCountiesToAdd)%>
        <%= Html.HiddenFor(eBilling => eBilling.BillingCountiesToRemove)%>
        <%= Html.TextBoxFor(eBilling => eBilling.MemberId, new { @class = "hidden", id = "eBillingMemberId" })%>
        <%= Html.TextBoxFor(eBilling => eBilling.HasDSReqCountriesAsBilling, new { @class = "hidden"})%>
        <%= Html.TextBoxFor(eBilling => eBilling.HasDSReqCountriesAsBilled, new { @class = "hidden" })%>
        <%= Html.HiddenFor(eBilling => eBilling.MiscRecArchivingLocs)%>
        <%= Html.HiddenFor(eBilling => eBilling.MiscPayArchivingLocs)%>
        <%= Html.HiddenFor(eBilling => eBilling.RecAssociationType)%>
        <%= Html.HiddenFor(eBilling => eBilling.PayAssociationType)%>
        <%if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
          {%>
        <input class="primaryButton"  id="btnSaveEBilling" type="submit" value="Save e-Billing Configuration" />
        <%
          }%>
    </div>
    
    <img alt="Future Updates Pending" src="<%:Url.Content("~/Content/Images/Exclamation.gif")%>" />
    Future Updates Pending
    <div class="subscriptionRequired" style="margin-left: 5px;">
        Subscription required to activate the optional service.</div>
</div>
<%
    }%>
<div id="divBilledMemberDsreq" class="removelist">
    <% Html.RenderPartial("BilledDSSupportedByAtos");%>
</div>
<div id="divBillingMemberDSReq" class="removelist">
    <% Html.RenderPartial("DSSupportedByAtos");%>
</div>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
    <% Html.RenderPartial("SearchResultControl");%></div>

<div id="divArchivingLocations">
    <% Html.RenderPartial("ArchivingRecLocations");%></div>

<div id="divArchivingPayLocations">
    <% Html.RenderPartial("ArchivingPayLocations");%></div>

<div id="divArchivalLocsEbilling">
    <div id="WarningMsgOne" style="display:none;">
    Legal Archiving is activated for MISC Receivables, but no Locations have been defined for which archiving should be performed. No Receivables Invoices/Credit Notes will be archived due to this.
    If you would like to correct this, please click Cancel and define the Locations using link ‘MISC Receivables Archiving Required for Locations’. 
    Else, click Save Anyway to keep Legal Archiving activated without defining any Locations.
    </div>
    <div id="WarningMsgTwo" style="display:none;">
        Legal Archiving is activated for MISC Payables, but no Locations have been defined for which archiving should be performed. No Payables Invoices/Credit Notes will be archived due to this.
        If you would like to correct this, please click Cancel and define the Locations using link ‘MISC Payables Archiving Required for Locations’. 
        Else, click Save Anyway to keep Legal Archiving activated without defining any Locations.

    </div>
    <div id="WarningMsgThree" style="display:none;">
    Legal Archiving is activated for both MISC Receivables and Payables, but no Locations have been defined for which archiving should be performed. No Receivables and Payables Invoices/Credit Notes will be archived due to this.
    If you would like to correct this, please click Cancel and define the Locations using links ‘MISC Receivables Archiving Required for Locations’ and ‘MISC Payables Archiving Required for Locations’
    Else, click Save Anyway to keep Legal Archiving activated without defining any Locations.
    </div>
    <div id="ErrorMsg" style="display:none;">
      There was an error in the MISC Legal Archiving Per Location ID. Please try again, or contact SIS Ops.
    </div>
</div>
