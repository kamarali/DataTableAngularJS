var achCurrencyUrl;
function InitialiseAchCurrency(achCurrencyURL) {

    achCurrencyUrl = achCurrencyURL;

    $("#AchCurrencySetUpMaster").validate({
        rules: {
            Id: {
                required: true
            }
        },
        messages: {
            Id: "Currency Of Clearance Required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });
}

$("#Id").change(function () {
    $("#Name").val($("#Id").val().split('|')[1]);
    $("#achCurrencyCode").val($("#Id").val().split('|')[0]);
});
