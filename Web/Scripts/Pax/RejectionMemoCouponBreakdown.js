_amountDecimals = 2;
_percentDecimals = 3;
var rmStage;
var fimSourceCode = 44;

$(document).ready(function () {

    SetPageWaterMark();
    $("#FlightDate").watermark("DD-MM");
    setVatBreakdownVisibility();

    // If pageMode is equal to Create or Edit, register Control events and Validation rules
    if (!$isOnView) {
        $("#rejectionMemoCouponBreakdown").validate({
            rules: {
                TicketOrFimIssuingAirline: "required",
                TicketDocOrFimNumber: "required",
                TicketOrFimCouponNumber: "required",
                CheckDigit: "required"
            },
            messages: {
                TicketOrFimIssuingAirline: { required: "Ticket Issuing Airline Required" },
                TicketDocOrFimNumber: { required: "Valid Ticket/Document Number Required" },
                TicketOrFimCouponNumber: { required: "Coupon Number Required" },
                CheckDigit: { required: "Valid Check Digit Required" },
                AllowedIscAmount: "Value is required and should be between -99999999999.9 to 99999999999.9",
                AcceptedIscAmount: "Value is required and should be between -99999999999.9 to 99999999999.9",
                AllowedIscPercentage: "Value should be between -99.999 and 99.999",
                AcceptedIscPercentage: "Value should be between -99.999 and 99.999",
                AllowedUatpPercentage: "Value should be between -99.999 and 99.999",
                AllowedUatpAmount: "Value is required and should be between -99999999999.9 to 99999999999.9",
                AcceptedUatpPercentage: "Value should be between -99.999 and 99.999",
                AcceptedUatpAmount: "Value is required and should be between -99999999999.9 to 99999999999.9",
                AllowedOtherCommissionPercentage: "Value should be between -99.999 and 99.999",
                AllowedOtherCommission: "Value is required and should be between -99999999999.9 to 99999999999.9",
                AcceptedOtherCommissionPercentage: "Value should be between -99.999 and 99.999",
                AcceptedOtherCommission: "Value is required and should be between -99999999999.9 to 99999999999.9",
                NetRejectAmount: "Value should be positive or 0"
            },
            submitHandler: function (form) {
                $('#TicketOrFimCouponNumber', '#content').attr('disabled', false);

                //calculateAmounts();
                // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
                onSubmitHandler(form);
            }
        });

        trackFormChanges('rejectionMemoCouponBreakdown');

        $('#TicketDocOrFimNumber').change(function () {
            var ticDocNumber = $('#TicketDocOrFimNumber').val();
            if (!isNaN(ticDocNumber))
                $('#TicketDocOrFimNumber').val(ticDocNumber);
        });


        $("#GrossAmountAccepted").blur(function () {
            if ($("#GrossAmountAccepted").val() === "" || $("#GrossAmountAccepted").val() === "0.00") {
                $("#AcceptedIscAmount").attr("readonly", true);
                $("#AcceptedIscAmount").attr("class", "smallTextField amountTextfield amt_12_3 amount");
                $("#AcceptedUatpAmount").attr("readonly", true);
                $("#AcceptedUatpAmount").attr("class", "smallTextField amountTextfield amt_12_3 amount");
            }
            else {
                $("#AcceptedIscAmount").attr("readonly", false);
                $("#AcceptedUatpAmount").attr("readonly", false);
                //                if ($("#SourceCodeId").val() == 91 || $("#SourceCodeId").val() == 92 || $("#SourceCodeId").val() == 93) {
                //                    $("#AcceptedIscAmount").attr("readonly", false);
                //                    $("#AcceptedUatpAmount").attr("readonly", false);
                //                }
            }
            setAmountDiff("#GrossAmountBilled", "#GrossAmountAccepted", "#GrossAmountDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");

            // Recalculate all values
            $("#AcceptedIscPercentage").blur();
            $("#AcceptedUatpPercentage").blur();
            $("#AcceptedOtherCommissionPercentage").blur();
        });

        $("#GrossAmountAccepted").blur();

        $("#GrossAmountBilled").blur(function () {

            if ($("#GrossAmountBilled").val() === "" || $("#GrossAmountBilled").val() === "0.00") {
                $("#AllowedIscAmount").attr("readonly", true);
                $("#AllowedIscAmount").attr("class", "smallTextField amountTextfield amt_12_3 amount");
                $("#AllowedUatpAmount").attr("readonly", true);
                $("#AllowedUatpAmount").attr("class", "smallTextField amountTextfield amt_12_3 amount");
            }
            else {
                $("#AllowedIscAmount").attr("readonly", false);
                $("#AllowedUatpAmount").attr("readonly", false);
                //                if ($("#SourceCodeId").val() == 91 || $("#SourceCodeId").val() == 92 || $("#SourceCodeId").val() == 93) {
                //                    $("#AllowedIscAmount").attr("readonly", false);
                //                    $("#AllowedUatpAmount").attr("readonly", false);
                //                }
            }

            setAmountDiff("#GrossAmountBilled", "#GrossAmountAccepted", "#GrossAmountDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");

            // Recalculate all values
            $("#AllowedIscPercentage").blur();
            $("#AllowedUatpPercentage").blur();
            $("#AllowedOtherCommissionPercentage").blur();
        });

        $("#GrossAmountBilled").blur();

        $("#TaxAmountDifference").blur(function () {
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#VatAmountDifference").blur(function () {
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#VatAmountBilled").blur(function () {
            setAmountDiff("#VatAmountBilled", "#VatAmountAccepted", "#VatAmountDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");

            setVatBreakdownVisibility();
        });

        $("#VatAmountAccepted").blur(function () {
            setAmountDiff("#VatAmountBilled", "#VatAmountAccepted", "#VatAmountDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
            setVatBreakdownVisibility();
        });

        $("#AllowedIscPercentage").blur(function () {
            setPercentage("#AllowedIscPercentage", "#GrossAmountBilled", "#AllowedIscAmount", 2);
            setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        //        $("#AllowedIscAmount").blur(function () {
        //            if ($("#SourceCodeId").val() == 91 || $("#SourceCodeId").val() == 92 || $("#SourceCodeId").val() == 93) {
        //                setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference", 2);
        //                calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        //            }
        //        });

        $("#AcceptedIscPercentage").blur(function () {
            setPercentage("#AcceptedIscPercentage", "#GrossAmountAccepted", "#AcceptedIscAmount", 2);
            setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        //        $("#AcceptedIscAmount").blur(function () {
        //            if ($("#SourceCodeId").val() == 91 || $("#SourceCodeId").val() == 92 || $("#SourceCodeId").val() == 93) {
        //                setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference", 2);
        //                calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        //            }
        //        });

        $("#AllowedUatpPercentage").blur(function () {
            setPercentage("#AllowedUatpPercentage", "#GrossAmountBilled", "#AllowedUatpAmount", 2);
            setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        //        $("#AllowedUatpAmount").blur(function () {
        //            if ($("#SourceCodeId").val() == 91 || $("#SourceCodeId").val() == 92 || $("#SourceCodeId").val() == 93) {
        //                setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpDifference", 2);
        //                calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        //            }
        //        });

        $("#AcceptedUatpPercentage").blur(function () {
            setPercentage("#AcceptedUatpPercentage", "#GrossAmountAccepted", "#AcceptedUatpAmount", 2);
            setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        //        $("#AcceptedUatpAmount").blur(function () {
        //            if ($("#SourceCodeId").val() == 91 || $("#SourceCodeId").val() == 92 || $("#SourceCodeId").val() == 93) {
        //                setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpDifference", 2);
        //                calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        //            }
        //        });

        $("#AllowedHandlingFee").blur(function () {
            setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#AcceptedHandlingFee").blur(function () {
            setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#AllowedOtherCommissionPercentage").blur(function () {
            setPercentage("#AllowedOtherCommissionPercentage", "#GrossAmountBilled", "#AllowedOtherCommission", 2);
            setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#AllowedOtherCommission").blur(function () {
            setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#AcceptedOtherCommission").blur(function () {
            setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#AcceptedOtherCommissionPercentage").blur(function () {
            setPercentage("#AcceptedOtherCommissionPercentage", "#GrossAmountAccepted", "#AcceptedOtherCommission", 2);
            setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference", 2);
            calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
        });

        $("#ProrateSlipDetails").bind("keypress", function () { maxLength(this, 4000) });
        $("#ProrateSlipDetails").bind("paste", function () { maxLengthPaste(this, 4000) });

        //Call method on Fetch button click
        $("#FetchButton").click(function () {
            //Search criteria for getting coupon Details

            TicketIssuingAirline = $.trim($("#TicketOrFimIssuingAirline").val());
            TicketNumber = $.trim($("#TicketDocOrFimNumber").val());
            CouponNumber = $.trim($("#TicketOrFimCouponNumber").val());
            //Search result only on the basis of all three values
            if (TicketIssuingAirline != '' && TicketNumber != '' && CouponNumber != '') {
                //Call server method through Ajax 
                $.ajax({
                    type: 'POST',
                    url: _getLinkingDetailsMethod,
                    data: { issuingAirline: TicketIssuingAirline, couponNo: CouponNumber, ticketDocNo: TicketNumber, rmId: _RejectionMemoId, billingMemberId: _BillingMemberId, billedMemberId: _BilledMemberId },
                    dataType: "json",
                    error: function (obj) {
                        //Set the error is true.
                        setAjaxError();
                    },
                    success: function (result) {
                        PopulateLinkingDetails(result);
                        //TFS9904:Visibility of the button is displayed incorrectly.
                        $("#SaveAndBackToOverview").removeAttr('readonly');
                        $("#SaveAndDuplicate").removeAttr('readonly');
                        $("#SaveAndAddNew").removeAttr('readonly');
                    }
                });
            }
            else {
                alert("Please enter all required fields.");
                $("#TicketOrFimIssuingAirline").focus();
            }
        });
    } // end if()

    //TFS9904:Visibility of the button is displayed incorrectly.
    if ($('#FetchButton').is(':visible')) {
        $("#SaveAndBackToOverview").attr("readonly", "readonly");
        $("#SaveAndDuplicate").attr("readonly", "readonly");
        $("#SaveAndAddNew").attr("readonly", "readonly");
    }
});

function calculateAmounts() {
    setAmountDiff("#GrossAmountBilled", "#GrossAmountAccepted", "#GrossAmountDifference", _amountDecimals);
    setAmountDiff("#VatAmountBilled", "#VatAmountAccepted", "#VatAmountDifference", _amountDecimals);

    if ($("#SourceCodeId").val() != 91 && $("#SourceCodeId").val() != 92 && $("#SourceCodeId").val() != 93) {
        setPercentage("#AllowedIscPercentage", "#GrossAmountBilled", "#AllowedIscAmount", _amountDecimals);
        setPercentage("#AcceptedIscPercentage", "#GrossAmountAccepted", "#AcceptedIscAmount", _amountDecimals);
        setPercentage("#AllowedUatpPercentage", "#GrossAmountBilled", "#AllowedUatpAmount", _amountDecimals);
        setPercentage("#AcceptedUatpPercentage", "#GrossAmountAccepted", "#AcceptedUatpAmount", _amountDecimals);
    }

    setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference", _amountDecimals);
    setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpDifference", _amountDecimals);
    setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingDifference", _amountDecimals);

    calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");
}

var TicketIssuingAirline;
var TicketNumber;
var CouponNumber;

function PopulateLinkingDetails(response) {

    var hasError = false;
    if (response != null) {
        if (response.ErrorMessage != '') {
            if (response.ErrorMessage.indexOf("Exception") != -1) {
                hasError = true;
                //Set the error is true.
                setAjaxError();
            }

            showClientErrorMessage(response.ErrorMessage);
        }
        else {
            clearMessageContainer();
        }
        if (hasError != true) {
            if (response.RMLinkedCoupons.length > 1) {
                var rejectionMemo = eval(response.RMLinkedCoupons);
                displayRecords(rejectionMemo);
            }
            else if (response.Details != null) {
                populateFields(response.Details);
            }

            // Enable "Save" button if linking is successful
            $("#btnSave").attr("disabled", false);
        }
    }
}

//Populate all the control after fetching data from the repository
function populateFields(selectedRecord) {
    $('#CheckDigit').val(selectedRecord.CheckDigit);
    $('#FromAirportOfCoupon').val(selectedRecord.FromAirportOfCoupon);
    $('#ToAirportOfCoupon').val(selectedRecord.ToAirportOfCoupon);

    $('#SettlementAuthorizationCode').val(selectedRecord.SettlementAuthorizationCode);
    $('#AgreementIndicatorSupplied').val(selectedRecord.AgreementIndicatorSupplied);
    $('#AgreementIndicatorValidated').val(selectedRecord.AgreementIndicatorValidated);

    $('#OriginalPmi').val(selectedRecord.OriginalPmi);
    $('#ValidatedPmi').val(selectedRecord.ValidatedPmi);

    $('#GrossAmountBilled').val(selectedRecord.GrossAmountBilled.toFixed(_amountDecimals));
    $('#GrossAmountAccepted').val(selectedRecord.GrossAmountAccepted.toFixed(_amountDecimals));
    $('#GrossAmountDifference').val(selectedRecord.GrossAmountDifference.toFixed(_amountDecimals));

    $('#TaxAmountBilled').val(selectedRecord.TaxAmountBilled.toFixed(_amountDecimals));
    $('#TaxAmountAccepted').val(selectedRecord.TaxAmountAccepted.toFixed(_amountDecimals));
    $('#TaxAmountDifference').val(selectedRecord.TaxAmountDifference.toFixed(_amountDecimals));

    totalTaxAmountBilled = $('#TaxAmountBilled').val();
    totalTaxAmountAccepted = $('#TaxAmountAccepted').val();
    totalTaxAmountDifference = $('#TaxAmountDifference').val();

    $('#VatAmountBilled').val(selectedRecord.VatAmountBilled.toFixed(_amountDecimals));
    $('#VatAmountAccepted').val(selectedRecord.VatAmountAccepted.toFixed(_amountDecimals));
    $('#VatAmountDifference').val(selectedRecord.VatAmountDifference.toFixed(_amountDecimals));

    $('#AllowedIscPercentage').val(selectedRecord.AllowedIscPercentage.toFixed(_amountDecimals));
    $('#AllowedIscAmount').val(selectedRecord.AllowedIscAmount.toFixed(_amountDecimals));
    $('#AcceptedIscPercentage').val(selectedRecord.AcceptedIscPercentage.toFixed(_percentDecimals));
    $('#AcceptedIscAmount').val(selectedRecord.AcceptedIscAmount.toFixed(_amountDecimals));
    $('#IscDifference').val(selectedRecord.IscDifference.toFixed(_amountDecimals));

    $('#AllowedUatpPercentage').val(selectedRecord.AllowedUatpPercentage.toFixed(_percentDecimals));
    $('#AllowedUatpAmount').val(selectedRecord.AllowedUatpAmount.toFixed(_amountDecimals));
    $('#AcceptedUatpPercentage').val(selectedRecord.AcceptedUatpPercentage.toFixed(_percentDecimals));
    $('#AcceptedUatpAmount').val(selectedRecord.AcceptedUatpAmount.toFixed(_amountDecimals));
    $('#UatpDifference').val(selectedRecord.UatpDifference.toFixed(_amountDecimals));

    $('#AllowedHandlingFee').val(selectedRecord.AllowedHandlingFee.toFixed(_amountDecimals));
    $('#AcceptedHandlingFee').val(selectedRecord.AcceptedHandlingFee.toFixed(_amountDecimals));
    $('#HandlingDifference').val(selectedRecord.HandlingDifference.toFixed(_amountDecimals));

    $('#AllowedOtherCommissionPercentage').val(selectedRecord.AllowedOtherCommissionPercentage.toFixed(_percentDecimals));
    $('#AllowedOtherCommission').val(selectedRecord.AllowedOtherCommission.toFixed(_amountDecimals));
    $('#AcceptedOtherCommissionPercentage').val(selectedRecord.AcceptedOtherCommissionPercentage.toFixed(_percentDecimals));
    $('#AcceptedOtherCommission').val(selectedRecord.AcceptedOtherCommission.toFixed(_amountDecimals));
    $('#OtherCommissionDifference').val(selectedRecord.OtherCommissionDifference.toFixed(_amountDecimals));

    calculateNetAmount("#GrossAmountDifference", "#TaxAmountDifference", "#VatAmountDifference", "#IscDifference", "#UatpDifference", "#HandlingDifference", "#OtherCommissionDifference", "#NetRejectAmount");

    deleteTaxRecords();
    if (selectedRecord.TaxBreakdown.length > 0) {
        var taxData = eval(selectedRecord.TaxBreakdown);
        // Populate data in tax grid with existing tax records
        if (taxData != null) {
            $taxCurrent = 1;
            for ($taxCurrent; $taxCurrent < taxData.length + 1; $taxCurrent++) {
                row = { Id: $taxCurrent, TaxCode: taxData[$taxCurrent - 1]["TaxCode"], Amount: taxData[$taxCurrent - 1]["Amount"], AmountAccepted: taxData[$taxCurrent - 1]["AmountAccepted"], AmountDifference: taxData[$taxCurrent - 1]["AmountDifference"] };
                $taxList.jqGrid('addRowData', $taxCurrent, row);
                addTaxFields($taxCurrent, "", row["TaxCode"], row["Amount"], taxData[$taxCurrent - 1]["AmountAccepted"], taxData[$taxCurrent - 1]["AmountDifference"]);
            }
        }
    }
}

//If more than 1 coupon exist in the repository,Declare variables for open a dialog box
var selectedRecord = -1;
var linkedRecords;
var linkedRecordsGrid;
// data fields
var BatchNumberDF = 'BatchNumber';
var RecordSequenceNumberDF = 'RecordSeqNumber';
var RdbColumn = 'RdbColumn';
var BreakdownSerialNoDF = 'BreakdownSerialNo';

// display names
var BatchNumberDN = 'Batch Number';
var RecordSequenceNumberDN = 'Record Sequence Number';
var BreakdownSerialNoDN = 'Breakdown SerialNo';

//Create and Fill the grid
function displayRecords(records) {
    linkedRecordsGrid = $('#linkedRecordsGrid');

    if (records[0]["BreakdownSerialNo"] != '') {
        linkedRecordsGrid.jqGrid({
            autoencode: true,
            datatype: 'local',
            width: 475,
            height: 250,
            colNames: ['', BreakdownSerialNoDN],
            colModel: [
                { name: RdbColumn, index: RdbColumn, sortable: false, width: 30, formatter: rdbFormatter }, // for radio button
                {name: BreakdownSerialNoDF, index: BreakdownSerialNoDF, sortable: false }
              ]
        });
    }
    else {
        linkedRecordsGrid.jqGrid({
            autoencode: true,
            datatype: 'local',
            width: 475,
            height: 250,
            colNames: ['', BatchNumberDN, RecordSequenceNumberDN],
            colModel: [
                { name: RdbColumn, index: RdbColumn, sortable: false, width: 30, formatter: rdbFormatter }, // for radio button
                {name: BatchNumberDF, index: BatchNumberDF, sortable: false },
                { name: RecordSequenceNumberDF, index: RecordSequenceNumberDF, sortable: false }
              ]
        });
    }

    //Set the Dialog property
    $('#linkedRecords').dialog({ closeOnEscape: false, title: '', height: 400, width: 500, modal: true, resizable: false });
    // get IDs of all the rows of jqGrid
    var rowIds = linkedRecordsGrid.jqGrid('getDataIDs');

    // iterate through the rows and delete each of them
    for (var i = 0, len = rowIds.length; i < len; i++) {
        var currRow = rowIds[i];
        linkedRecordsGrid.jqGrid('delRowData', currRow);
    }
    selectedRecord = -1;

    if (records != null) {
        records = eval(records);
        linkedRecords = records;
        recordCurrent = 1;
        for (recordCurrent; recordCurrent < records.length + 1; recordCurrent++) {
            if (records[0]["BreakdownSerialNo"] != '') {
                row = { RdbColumn: recordCurrent - 1, BreakdownSerialNo: records[recordCurrent - 1]["BreakdownSerialNo"] };
            }
            else {
                row = { RdbColumn: recordCurrent - 1, BatchNumber: records[recordCurrent - 1]["BatchNumber"], RecordSeqNumber: records[recordCurrent - 1]["RecordSeqNumber"] };
            }
            linkedRecordsGrid.jqGrid('addRowData', recordCurrent, row);
        }
    }
}

function rdbFormatter(cellValue, options, cellObject) {
    return '<input type="radio" name="rdbRecord" value=cellValue onclick="setSelectedRecord(' + cellValue + ');" />';
}

function setSelectedRecord(selectedIndex) {
    selectedRecord = selectedIndex;
}

function onLinkingDialogClose() {
    //populate details of selected index
    if (selectedRecord == -1) {
        alert('Please select at least one record.');
        return;
    }
    //Call server method through Ajax
    $.ajax({
        type: 'POST',
        url: _getLinkingSingleCouponDetailsMethod,
        data: { couponId: linkedRecords[selectedRecord].CouponId, rejectionMemoId: _RejectionMemoId, billingMemberId: _BillingMemberId, billedMemberId: _BilledMemberId },
        dataType: "json",
        error: function () {
            setAjaxError();

        },
        success: function (result) {
            PopulateLinkingDetails(result);
        }
    });
    closeDialog('#linkedRecords');
}

//Disable some control in Edit mode
function InitializeLinkingFieldsInEditMode(isLinkingSuccessful, rejectionMemoSourceCode) {
    if (isLinkingSuccessful == "True" && rejectionMemoSourceCode != fimSourceCode) {
        $("#FetchButton").hide();
        SetControlAccessibility();
        DisableFieldsPopulatedInLinking();
    }
}

function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId, decimalPlaces) {
    if ($(sourceControl1Id).val() != '' && $(sourceControl2Id).val() != '') {
        var sourceControl1Value = $(sourceControl1Id).val();
        var sourceControl2Value = $(sourceControl2Id).val();
        var percent = (sourceControl2Value * sourceControl1Value) / 100;
        
        percent = parseFloat(roundNumber(percent, decimalPlaces));

        if (!isNaN(percent))
            $(targetControlId).val(percent);
    }
    else {
        $(targetControlId).val(0.00);
    }
}

function roundNumber(startValue, digits) {
  var decimalValue = 0;
  startValue = startValue * Math.pow(10, digits + 1);

  // Math.floor only in case of positive value and ignore decimals in case of negative value 
  // Math.floor rounds up the number to the integer closests to zero. 
  // Therefore, Math.floor(-10005.4)  = -100.06 which is logically incorrect as per ISPG standards
  // Thus parseInt(-10005.4) will return -10005 with decimal value 4 and the calculated startValue will be -100.50 here which is logically correct
  if (startValue >= 0) {
    decimalValue = parseInt(Math.floor(startValue) - Math.floor(startValue / 10) * 10);
    startValue = Math.floor(startValue / 10);    
  }
  else {
    decimalValue = parseInt(startValue - parseInt(startValue / 10) * 10);
    if (decimalValue < 0)
      decimalValue = -(decimalValue);
    startValue = parseInt(startValue / 10);    
  }
  // Add 1 in case of Positive value and subtract 1 in case of Negative value
  if (decimalValue >= 5) {
    if(startValue >= 0)
    {      
      startValue = startValue + 1;   
    }
    else {         
      startValue = startValue - 1;        
    }
  }
  startValue = startValue / parseFloat(Math.pow(10, (digits)));
  return startValue;  
}

function setAmountDiff(sourceControl1Id, sourceControl2Id, targetControlId, decimalPlaces) {

    var sourceControl1Value = 0;
    sourceControl1Value = $(sourceControl1Id).val();
    var sourceControl2Value = 0;
    sourceControl2Value = $(sourceControl2Id).val();
    var difference = 0;

    if (rmStage == 2 || rmStage == "2")
        difference = sourceControl2Value - sourceControl1Value;
    else
        difference = sourceControl1Value - sourceControl2Value;

    if (!isNaN(difference))
        $(targetControlId).val(difference.toFixed(decimalPlaces));
    else
        $(targetControlId).val(Number(0).toFixed(_amountDecimals));
}

function calculateNetAmount(sourceControl1, sourceControl2, sourceControl3, sourceControl4, sourceControl5, sourceControl6, sourceControl7, targetControl) {
    var grossAmount = 0;
    var taxAmount = 0;
    var vatAmount = 0;
    var iscAmount = 0;
    var uatpAmount = 0;
    var hfAmount = 0;
    var ocAmount = 0;

    if (!isNaN(Number($(sourceControl1).val())))
        grossAmount = Number($(sourceControl1).val());

    if (!isNaN(Number($(sourceControl2).val())))
        taxAmount = Number($(sourceControl2).val());

    if (!isNaN(Number($(sourceControl3).val())))
        vatAmount = Number($(sourceControl3).val());

    if (!isNaN(Number($(sourceControl4).val())))
        iscAmount = Number($(sourceControl4).val());

    if (!isNaN(Number($(sourceControl5).val())))
        uatpAmount = Number($(sourceControl5).val());

    if (!isNaN(Number($(sourceControl6).val())))
        hfAmount = Number($(sourceControl6).val());

    if (!isNaN(Number($(sourceControl7).val())))
        ocAmount = Number($(sourceControl7).val());

    var totalNetRejectAmount = grossAmount + iscAmount + ocAmount + uatpAmount + hfAmount + taxAmount + vatAmount;
    if (!isNaN(totalNetRejectAmount))
        $(targetControl).val(totalNetRejectAmount.toFixed(_amountDecimals));
}

function SetControlAccessibility() {
    $("#TicketOrFimIssuingAirline").attr("readonly", "true");
    $("#TicketDocOrFimNumber").attr("readonly", "true");
    $("#TicketDocOrFimNumber").attr("maxLength", "11");
    $("#TicketDocOrFimNumber").attr("max", "99999999999");
    $("#TicketOrFimCouponNumber").attr("disabled", "true");
}

function InitializeCreateRMCoupon(pageMode) {
    if (pageMode == 'create') {
        $('input[type=text]:not(.populated)').removeAttr('value');
        $('#CheckDigit').val('');
        $('#TicketOrFimIssuingAirline').removeAttr('value'); // Temporary code. To be removed when this field is pre-filled.
        $.watermark.showAll();
    }
    else
        $.watermark.showAll();
}




function setVatBreakdownVisibility() {
    if ($('#VatAmountDifference').val() == 0)
        $('#vatBreakdown').hide();
    else
        $('#vatBreakdown').show();
}

// Initial function for setting global variable
// Billing and billed members are sent reversed.
function InitialiseLinking(getLinkingDetailsMethod, getLinkingSingleCouponDetailsMethod, RejectionMemoId, BillingMemberId, BilledMemberId, isLinkingSuccessful, rejectionMemoSoureCode, isExceptionOccured) {
    _getLinkingDetailsMethod = getLinkingDetailsMethod;
    _getLinkingSingleCouponDetailsMethod = getLinkingSingleCouponDetailsMethod;
    _RejectionMemoId = RejectionMemoId;
    _BillingMemberId = BillingMemberId;
    _BilledMemberId = BilledMemberId;

    if (isLinkingSuccessful == 'True' && rejectionMemoSoureCode != fimSourceCode) {
        DisableFieldsPopulatedInLinking();
        // Disable "Save" button on page load, it will be enabled when user clicks on Fetch button.
        // Button should be enabled in case of server-side validation error.
        if (isExceptionOccured != 'True')
            $("#btnSave").attr("disabled", true);
    }
}

function DisableFieldsPopulatedInLinking() {
    $('.linkingPopulated', '#content').attr('readonly', true);
    $("#TicketOrFimIssuingAirline").blur(OnTicketOrFimIssuingAirlineBlur);
    $("#TicketDocOrFimNumber").blur(OnTicketDocOrFimNumberBlur);
    $("#TicketOrFimCouponNumber").blur(OnTicketOrFimCouponNumberBlur);
}


function OnTicketOrFimIssuingAirlineBlur(element) {
  // Note:- Save button is disabled on page load but gets enabled when ajax call for "Ticket Issuing Airline" is completed. So Disable "Save" button again.
    if (($("#TicketOrFimIssuingAirline").is(':disabled') == false) && ($("#TicketOrFimIssuingAirline").prop('readonly') != true)) {
    if ($("#TicketOrFimIssuingAirline").val() != TicketIssuingAirline) {
      $("#btnSave").attr("disabled", true);
      $("#SaveAndBackToOverview").attr("disabled", true);
      $("#SaveAndDuplicate").attr("disabled", true);
      $("#SaveAndAddNew").attr("disabled", true);
    }
  }
}

function OnTicketDocOrFimNumberBlur(element) {
  // Note:- Save button is disabled on page load but gets enabled when ajax call for "Ticket Document Number" is change. So Disable "Save" button again.
    if (($("#TicketDocOrFimNumber").is(':disabled') == false) && ($("#TicketDocOrFimNumber").prop('readonly') != true)) {
    if ($("#TicketDocOrFimNumber").val() != TicketNumber) {
      $("#btnSave").attr("disabled", true);
      $("#SaveAndBackToOverview").attr("disabled", true);
      $("#SaveAndDuplicate").attr("disabled", true);
      $("#SaveAndAddNew").attr("disabled", true);
    }
  }
}

function OnTicketOrFimCouponNumberBlur(element) {
  // Note:- Save button is disabled on page load but gets enabled when ajax call for "Coupon Number" is change. So Disable "Save" button again.
    if (($("#TicketOrFimCouponNumber").is(':disabled') == false) && ($("#TicketOrFimCouponNumber").prop('readonly') != true)) {
    if ($("#TicketOrFimCouponNumber").val() != CouponNumber) {
      $("#btnSave").attr("disabled", true);
      $("#SaveAndBackToOverview").attr("disabled", true);
      $("#SaveAndDuplicate").attr("disabled", true);
      $("#SaveAndAddNew").attr("disabled", true);
    }
  }
}

function deleteTaxRecords() {

    // Delete record from grid
    var $taxList = $('#taxGrid');
    var rowIds = $taxList.jqGrid('getDataIDs');

    // iterate through the rows and delete each of them
    for (var i = 0, len = rowIds.length; i < len; i++) {
        var currRow = rowIds[i];
        $taxList.jqGrid('delRowData', currRow);
        //delete record entries from hidden fields
        var taxCode = '#' + taxFields.TaxCode.Id + currRow;
        $(taxCode).remove();
        var taxAmountBilled = '#' + taxFields.TaxAmountBilled.Id + currRow;
        $(taxAmountBilled).remove();
        var taxAmountAccepted = '#' + taxFields.TaxAmountAccepted.Id + currRow;
        $(taxAmountAccepted).remove();
        var taxAmountDifference = '#' + taxFields.TaxAmountDifference.Id + currRow;
        $(taxAmountDifference).remove();
        var taxId = '#' + taxFields.TaxId.Id + currRow;
        $(taxId).remove();
    }
}
