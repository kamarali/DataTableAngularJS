_amountDecimals = 2;
_percentDecimals = 3;
var IsSMI = 'False';
var IsFormAB = 'False';
var ISFormC = 'False';
$(document).ready(function () {
    SetPageWaterMark();
 
    if (!$isOnView) {
        $("#formE").validate({
            rules: {
                GrossTotalOfUniverse: { required: function () {
                    return CheckForm(IsFormAB);
                }
                },
                GrossTotalOfUaf: { required: function () {
                    return CheckForm(IsFormC);
                }
                },
                UniverseAdjustedGrossAmount: "required",
                GrossTotalOfSample: "required",
                GrossTotalOfUafSampleCoupon: "required",
                SampleAdjustedGrossAmount: "required",
                SamplingConstant: "required",
                TotalOfGrossAmtXSamplingConstant: "required",
                TotalOfIscAmtXSamplingConstant: "required",
                TotalOfOtherCommissionAmtXSamplingConstant: "required",
                UatpCouponTotalXSamplingConstant: "required",
                HandlingFeeTotalAmtXSamplingConstant: "required",
                TaxCouponTotalsXSamplingConstant: "required",
                VatCouponTotalsXSamplingConstant: "required",
                NetAmountDue: "required",
                NetAmountDueInCurrencyOfBilling: "required",
                TotalAmountFormB: "required",
                NetBilledCreditedAmount: "required",
                NumberOfBillingRecords: "required",

                ProvisionalFormBGrossBilled: { required: function () {
                    return CheckSMI();
                }
                },

                ProvisionalFormBIscAmount: { required: function () {
                    return CheckSMI();
                }
                },
                ProvisionalFormBOtherCommissionAmount: { required: function () {
                    return CheckSMI();
                }
                },
                ProvisionalFormBUatpAmount: { required: function () {
                    return CheckSMI();
                }
                },
                ProvisionalFormBHandlingFeeAmountBilled: { required: function () {
                    return CheckSMI();
                }
                },
                ProvisionalFormBTaxAmount: { required: function () {
                    return CheckSMI();
                }
                },
                ProvisionalFormBVatAmountBilled: { required: function () {
                    return CheckSMI();
                }
                }
            },
            messages: {
                GrossTotalOfUniverse: { required: "Gross Total Of Universe Required" },
                GrossTotalOfUaf: "Gross Total Of Form C Required",
                UniverseAdjustedGrossAmount: "Universe Adjusted Gross Amount Required",
                GrossTotalOfSample: "Gross Total Of Sample Required",
                GrossTotalOfUafSampleCoupon: "Gross Total Of Form C Sample Coupon Required",
                SampleAdjustedGrossAmount: "Sample Adjusted Gross Amount Required",
                SamplingConstant: { required: "Sampling Constant Required" },
                TotalOfGrossAmtXSamplingConstant: "Total Of Gross Amt. x Sampling Constant Required",
                TotalOfIscAmtXSamplingConstant: "Total Of ISC Amt. x Sampling Constant Required",
                TotalOfOtherCommissionAmtXSamplingConstant: "Total Of Other Comm. Amt. x Sampling Constant required",
                UatpCouponTotalXSamplingConstant: "UATP Coupon Total x Sampling Constant required",
                HandlingFeeTotalAmtXSamplingConstant: "Handling Fee Total x Sampling Constant Required",
                TaxCouponTotalsXSamplingConstant: "Tax Coupon Total x Sampling Constant Required",
                VatCouponTotalsXSamplingConstant: "VAT Coupon Total x Sampling Constant Required",
                NetAmountDue: "Net Amount Due In Currency of Listing Required",
                NetAmountDueInCurrencyOfBilling: "Net Amount Due In Currency Of Billing Required",
                TotalAmountFormB: { required: "FormB Total Amount Required" },
                NetBilledCreditedAmount: "Net Billed/Credited Amount Required",
                NumberOfBillingRecords: "Number Of Billing Records Required",

                ProvisionalFormBGrossBilled: "Gross Billed amount Required",
                ProvisionalFormBIscAmount: "ISC amount Required",
                ProvisionalFormBOtherCommissionAmount: "Other Commission amount Required",
                ProvisionalFormBUatpAmount: "UATP amount Required",
                ProvisionalFormBHandlingFeeAmountBilled: "Handling Fee Required",
                ProvisionalFormBTaxAmount: " Tax amount Required",
                ProvisionalFormBVatAmountBilled: "VAT amount Required"
            }
        });

        trackFormChanges('formE');

        $('#GrossTotalOfUniverse').blur(function () {
            if (!$("#GrossTotalOfUniverse").prop("readonly")) {
                calcUAGAmt();
                calcSamplingConst();
            }
        })

        $('#GrossTotalOfUaf').blur(function () {
            if (!$("#GrossTotalOfUaf").prop("readonly")) {
                calcUAGAmt();
                calcSamplingConst();
            }
        })


        $('#GrossTotalOfUafSampleCoupon').blur(function () {
            if (!$("#GrossTotalOfUafSampleCoupon").prop("readonly")) {
                calcSampleAdjGrossAmt();
                calcSamplingConst();
            }
        })


        //        $('#ProvisionalFormBGrossBilled').blur(function () {
        //            if (!$("#ProvisionalFormBGrossBilled").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#ProvisionalFormBIscAmount').blur(function () {
        //            if (!$("#ProvisionalFormBIscAmount").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#ProvisionalFormBOtherCommissionAmount').blur(function () {
        //            if (!$("#ProvisionalFormBOtherCommissionAmount").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#ProvisionalFormBUatpAmount').blur(function () {
        //            if (!$("#ProvisionalFormBUatpAmount").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#ProvisionalFormBHandlingFeeAmountBilled').blur(function () {
        //            if (!$("#ProvisionalFormBHandlingFeeAmountBilled").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#ProvisionalFormBTaxAmount').blur(function () {
        //            if (!$("#ProvisionalFormBTaxAmount").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#ProvisionalFormBVatAmountBilled').blur(function () {
        //            if (!$("#ProvisionalFormBVatAmountBilled").attr("readonly")) {
        //                calcFormBNetAmt();
        //                calcNetBilledCreditedAmt();
        //            }
        //        })

        //        $('#TotalAmountFormB').blur(function () {
        //            if (!$("#TotalAmountFormB").attr("readonly")) {
        //                calcNetBilledCreditedAmt();
        //            }
        //        })
        calcFormDTotals();
        $('#Save').click(function () {
            calculateAmounts();
        });
    }
});

function setDifference(firstOperandId, secondOperandId, differenceId) {
    var firstOperandVal = $(firstOperandId).val();
    var secondOperandVal = $(secondOperandId).val();

    var difference = Number(firstOperandVal) - Number(secondOperandVal);

    if (!isNaN(difference))
        $(differenceId).val(difference.toFixed(_amountDecimals));
}

function setDivision(numeratorId, denominatorId, quotientId, decimalPlaces) {
    var firstOperandVal = getVal(numeratorId);
    var secondOperandVal = getVal(denominatorId);
    var quotient = 0;
    if (secondOperandVal == 0)
        $(quotientId).val(quotient.toFixed(decimalPlaces));
    else {
        quotient = Number(firstOperandVal) / Number(secondOperandVal);

        if (!isNaN(quotient))
            $(quotientId).val(quotient.toFixed(decimalPlaces));
    }
}

function setProduct(operand1Id, operand2Id, productId) {
    var product = Number(getVal(operand1Id)) * Number(getVal(operand2Id));

    if (!isNaN(product))
        $(productId).val(product.toFixed(_amountDecimals));
}

function setSum(operand1, operand2, operand3, operand4, operand5, operand6, operand7, sumId) {

    var sum = Number(getVal(operand1)) + Number(getVal(operand2)) + Number(getVal(operand3)) + Number(getVal(operand4))
  + Number(getVal(operand5)) + Number(getVal(operand6)) + Number(getVal(operand7));

    if (!isNaN(sum))
        $(sumId).val(sum.toFixed(_amountDecimals));
}

function getVal(controlId) {
    return $(controlId).val();
}

// Function to calculate Universe Adjusted Gross Amount
function calcUAGAmt() {
    setDifference('#GrossTotalOfUniverse', '#GrossTotalOfUaf', '#UniverseAdjustedGrossAmount');
}

function calcSampleAdjGrossAmt() {
    // changed as per discussion with robin.
    var sum = Number($('#GrossTotalOfUafSampleCoupon').val()) + Number($('#SampleAdjustedGrossAmount').val());
    if (!isNaN(sum))
        $('#GrossTotalOfSample').val(sum.toFixed(_amountDecimals));
}

function calcFormBNetAmt() {
    if (($('#ProvisionalFormBGrossBilled').val() != "0.00" && $('#ProvisionalFormBGrossBilled').val() != "")
    || ($('#ProvisionalFormBIscAmount').val() != "0.00" && $('#ProvisionalFormBIscAmount').val() != "")
     || ($('#ProvisionalFormBOtherCommissionAmount').val() != "0.00" && $('#ProvisionalFormBOtherCommissionAmount').val() != "")
    || ($('#ProvisionalFormBUatpAmount').val() != "0.00" && $('#ProvisionalFormBUatpAmount').val() != "")
    || ($('#ProvisionalFormBHandlingFeeAmountBilled').val() != "0.00" && $('#ProvisionalFormBHandlingFeeAmountBilled').val() != "")
  || ($('#ProvisionalFormBTaxAmount').val() != "0.00" && $('#ProvisionalFormBTaxAmount').val() != "")
 || ($('#ProvisionalFormBVatAmountBilled').val() != "0.00" && $('#ProvisionalFormBVatAmountBilled').val() != "")) {
        setSum('#ProvisionalFormBGrossBilled', '#ProvisionalFormBIscAmount', '#ProvisionalFormBOtherCommissionAmount',
  '#ProvisionalFormBUatpAmount', '#ProvisionalFormBHandlingFeeAmountBilled', '#ProvisionalFormBTaxAmount',
  '#ProvisionalFormBVatAmountBilled', '#TotalAmountFormB');
    }
    else {
        $('#TotalAmountFormB').val($('#FormBAmmount').val());
    }
}

function calcSamplingConst() {
    setDivision('#UniverseAdjustedGrossAmount', '#SampleAdjustedGrossAmount', '#SamplingConstant', 3);
    calcFormDTotals();
}

function calcNetBilledCreditedAmt() {
  setDifference('#TotalAmountFormB', '#NetAmountDue', '#NetBilledCreditedAmount');
  var exchangeRate = $('#ExchangeRate').val();

  var netBillCreditAmount = $('#NetBilledCreditedAmount').val();

  if (exchangeRate != '' && exchangeRate > 0 && netBillCreditAmount != '' && !isNaN(netBillCreditAmount)) {

    netBillCreditAmount = (netBillCreditAmount / exchangeRate);

    $('#NetBilledCreditedAmount').val(netBillCreditAmount.toFixed(_amountDecimals));
  }
}

function calcFormDTotals() {
    var sampConstCntrlId = "#SamplingConstant";
    setProduct('#hidGrossValue', sampConstCntrlId, '#TotalOfGrossAmtXSamplingConstant');
    setProduct('#hidIscAmt', sampConstCntrlId, '#TotalOfIscAmtXSamplingConstant');
    setProduct('#hidOtherCommissionAmt', sampConstCntrlId, '#TotalOfOtherCommissionAmtXSamplingConstant');
    setProduct('#hidUatpAmt', sampConstCntrlId, '#UatpCouponTotalXSamplingConstant');
    setProduct('#hidHandlingFee', sampConstCntrlId, '#HandlingFeeTotalAmtXSamplingConstant');
    setProduct('#hidTaxAmt', sampConstCntrlId, '#TaxCouponTotalsXSamplingConstant');
    setProduct('#hidVatAmt', sampConstCntrlId, '#VatCouponTotalsXSamplingConstant');

    setSum('#TotalOfGrossAmtXSamplingConstant', '#TotalOfIscAmtXSamplingConstant', '#TotalOfOtherCommissionAmtXSamplingConstant',
  '#UatpCouponTotalXSamplingConstant', '#HandlingFeeTotalAmtXSamplingConstant', '#TaxCouponTotalsXSamplingConstant',
  '#VatCouponTotalsXSamplingConstant', '#NetAmountDue');
    // Removed commented WRT issue Issue regarding 'NetAmountDueInBillingCurrency' in 'Sampling form E' 
    //SCP32104
     setDivision('#NetAmountDue', '#hidListingToBillingRate', '#NetAmountDueInCurrencyOfBilling', _amountDecimals);
    calcNetBilledCreditedAmt();
}

function InializeLinking() {
    if (IsFormAB == "True") {
        disableABLinkingFields()
    }
    if (IsFormC == "True") {
        disableCLinkingFields()
    }
}

function setGlobalVariables(SMI, FormAB, FormC) {
    IsSMI = SMI;
    IsFormAB = FormAB;
    IsFormC = FormC;
}

function calculateAmounts() {
    calcUAGAmt();
    calcSampleAdjGrossAmt();
    calcSamplingConst();
    //calcFormBNetAmt();
    calcNetBilledCreditedAmt();
}

function disableABLinkingFields() {
    $('#GrossTotalOfUniverse', '#content').attr('Enabled', false);
    $('#ProvisionalFormBGrossBilled', '#content').attr('readOnly', true);
    $('#ProvisionalFormBIscAmount', '#content').attr('readOnly', true);
    $('#ProvisionalFormBOtherCommissionAmount', '#content').attr('readOnly', true);
    $('#ProvisionalFormBUatpAmount', '#content').attr('readOnly', true);
    $('#ProvisionalFormBHandlingFeeAmountBilled', '#content').attr('readOnly', true);
    $('#ProvisionalFormBTaxAmount', '#content').attr('readOnly', true);
    $('#ProvisionalFormBVatAmountBilled', '#content').attr('readOnly', true);

}

function disableCLinkingFields() {
    $('#GrossTotalOfUaf', '#content').attr('readOnly', true);
}

function CheckForm(IsForm) {
    if (IsForm == 'False') {
        return true;
    }
    else
        return false;
}

function CheckSMI() {
    if (IsSMI == 'True') {
        return true;
    }
    else
        return false;
}