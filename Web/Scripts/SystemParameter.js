


function initSystemVariablesValidations(obj) {

    conditionallyValidateTicketOrFimCouponNumber();


    $("#formid").validate({
        rules: {
            "UIParameters.PageSizeOptions": {
            //ValidTicketOrFimCouponNumber: ['^\[\d,*\]$']
        },
        "UIParameters.DefaultPageSize": {
            ValidTicketOrFimCouponNumber: ['^\\d*$']
        },
        "BVCDetails.AIASLANoOfRecords": {
            ValidTicketOrFimCouponNumber: ['^\\d*$']
        },
        "BVCDetails.AIASLATimeInSeconds": {
            ValidTicketOrFimCouponNumber: ['^\\d*$']
        },
        "BVCDetails.MaxCouponRecordsPerVCF": {
            ValidTicketOrFimCouponNumber: ['^\\d*$']
        },
        "ICHDetails.IchAdmininstratorEmail": {
        //  ValidTicketOrFimCouponNumber: ['^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$']
    },
    "ICHDetails.IchOpsEmail": {
    //ValidTicketOrFimCouponNumber: ['^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$']
},
"ICHDetails.ManualControlOnIchLateSubmission": {
//ValidTicketOrFimCouponNumber: ['^[a-zA-Z]*$']
},
"ICHDetails.MaxNumberOfInvoicesInIchSettlementFile": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"ICHDetails.MaxNumberOfRetriesToSendIchSettlementFile": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"ACHDetails.ACHAdmininstratorEmail": {
//ValidTicketOrFimCouponNumber: ['^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$']
},
"ACHDetails.AchOpsEmail": {
//ValidTicketOrFimCouponNumber: ['^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$']
},
"ACHDetails.ManualControlOnACHLateSubmission": {
// ValidTicketOrFimCouponNumber: ['^[a-zA-Z]*$']
},
"SIS_OpsDetails.ISAdmininstratorEmail": {
//ValidTicketOrFimCouponNumber: ['^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$']
},
"SIS_OpsDetails.SisOpsEmail": {
//ValidTicketOrFimCouponNumber: ['^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$']
},
"General.DefaultExpiryDateforMessages": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.FtpFileUploadMaxAttempt": {
//  ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.MaxLoginAllowed": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.HtmlToPdfTimeoutInMilliSeconds": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.IgnoreValidationOnMigrationPeriod": {
//  ValidTicketOrFimCouponNumber: ['^[a-zA-Z]*$']
},
"General.MaxBillingFilesAllowedPerDay": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.BillingYearToStartWith": {
    ValidTicketOrFimCouponNumber: ['^\\d{4}$']
},
"General.DefaultExpiryDaysforAnnoucements": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.MemberContactMaxRowCount": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"General.SuppDocMaxFileSizeWeb": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"iiNet.ServerName": {
//  ValidTicketOrFimCouponNumber: ['^\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}$']
},
"iiNet.Port": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"Ich.ServerName": {
//  ValidTicketOrFimCouponNumber: ['^\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}$']
},
"Ich.Port": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"Ach.ServerName": {
//   ValidTicketOrFimCouponNumber: ['^\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}$']
},
"Ach.Port": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"Atpco.ServerName": {
//   ValidTicketOrFimCouponNumber: ['^\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}$']
},
"Atpco.Port": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"PurgingPeriodDetails.OutputDataFilesPurgePeriod": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"PurgingPeriodDetails.UnlinkedSupportDocFilesPurgePeriod": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"PurgingPeriodDetails.TemporaryFilesPurgePeriod": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"PurgingPeriodDetails.LegalArchievePurgePeriod": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"PurgingPeriodDetails.LogFilesPurgePeriod": {
    ValidTicketOrFimCouponNumber: ['^\\d*$']
},
"PurgingPeriodDetails.LogFilePath": {
//ValidTicketOrFimCouponNumber: ['^\\d*$']
},
//CMP529 : Daily Output Generation for MISC Bilateral Invoices
"PurgingPeriodDetails.DailyMiscBilateralFilesPurgingPeriod": {
    ValidTicketOrFimCouponNumber: ['^\\d{1,3}$']
}
},
messages: {
// "UIParameters.PageSizeOptions": "Invalid IBAN Value"

}

});
    $('#errorContainer').hide();
}


function conditionallyValidateTicketOrFimCouponNumber() {
   
    // Following code is used for Conditional validation of TicketOrFimCouponNumber field
    $.validator.addMethod("ValidTicketOrFimCouponNumber", function (value, element, param) {
      
        if (value != '') {
            //Ascii is defined as the characters in the range of 000-177 (octal). In egular expression, Octal number for range 32 to 126 are used
            var re = new RegExp(param[0]);

            if (!value.match(re)) {
                this.settings.messages[element.name] = "Value contains invalid characters.";
                return false;
            }
            else
                return true;
        }
        else
            return true;
    });

    // Following code is used for Conditional validation of TicketOrFimCouponNumber field
    $.validator.addMethod("ValidateEmail", function (value, element) {

        
        valid = true;

        valid = valid && jQuery.validator.methods.email.call(this, value, element);

        return valid;
    }, 'email addresses are invalid');


}