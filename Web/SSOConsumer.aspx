<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SSOConsumer.aspx.cs" Inherits="Iata.IS.Web.SSOConsumer" %>

<script src="<%:Url.Content("~/Scripts/jquery-1.12.3.min.js")%>" type="text/javascript"></script>
<script src="<%:Url.Content("~/Scripts/jquery-migrate-1.2.1.min.js")%>" type="text/javascript"></script>

<script type="text/javascript">

    $(document).ready(function () {

        //Get value of isRedirectUponLogin
        var isRedirectUponLogin = $("#" + '<%= isRedirectUponLogin.ClientID %>').val();

        //Get base URL and remove last slash.
        var baseUrl = window.location.origin + '<%: Request.ApplicationPath %>';
        baseUrl = baseUrl.replace(/\/$/, '');

        //Ajax Call for update set time zone.
        $.ajax({
            type: "POST",
            url: baseUrl + '/Account/SetTimeZone',
            data: { timeZoneId: getTimezoneName() },
            dataType: "json",
            async: false,
            success: function (response) {
                if (isRedirectUponLogin == 'true') {
                    location.href = baseUrl + '/MiscPayables/ManageMiscDailyPayablesInvoice?IsRedirectUponLogin=true';
                }
                else {
                    location.href = baseUrl + '/Home/Index';
                }
            }
        });
    });

    //get time zone name.
    function getTimezoneName() {

        tmSummer = new Date(Date.UTC(2005, 6, 30, 0, 0, 0, 0));
        so = -1 * tmSummer.getTimezoneOffset();

        tmWinter = new Date(Date.UTC(2005, 12, 30, 0, 0, 0, 0));
        wo = -1 * tmWinter.getTimezoneOffset();

        if (-660 == so && -660 == wo) return 'Alaskan Standard Time';
        if (-600 == so && -600 == wo) return 'Hawaiian Standard Time';
        if (-570 == so && -570 == wo) return 'Alaskan Standard Time';
        if (-540 == so && -600 == wo) return 'Hawaiian Standard Time';
        if (-540 == so && -540 == wo) return 'Alaskan Standard Time';
        if (-480 == so && -540 == wo) return 'Alaskan Standard Time';
        if (-480 == so && -480 == wo) return 'Pacific Standard Time';
        if (-420 == so && -480 == wo) return 'Pacific Standard Time';
        if (-420 == so && -420 == wo) return 'Central America Standard Time';
        if (-360 == so && -420 == wo) return 'Mountain Standard Time';
        if (-360 == so && -360 == wo) return 'Central America Standard Time';
        if (-360 == so && -300 == wo) return 'SA Pacific Standard Time';
        if (-300 == so && -360 == wo) return 'Central Standard Time';
        if (-300 == so && -300 == wo) return 'SA Pacific Standard Time';
        if (-240 == so && -300 == wo) return 'Eastern Standard Time';
        if (-240 == so && -240 == wo) return 'Paraguay Standard Time';
        if (-240 == so && -180 == wo) return 'Pacific SA Standard Time';
        if (-180 == so && -240 == wo) return 'Atlantic Standard Time';
        if (-180 == so && -180 == wo) return 'Montevideo Standard Time';
        if (-180 == so && -120 == wo) return 'Pacific SA Standard Time';
        if (-150 == so && -210 == wo) return 'Pacific SA Standard Time';
        if (-120 == so && -180 == wo) return 'UTC-02';
        if (-120 == so && -120 == wo) return 'UTC-02';
        if (-60 == so && -60 == wo) return 'Mid-Atlantic Standard Time';
        if (0 == so && -60 == wo) return 'Mid-Atlantic Standard Time';
        if (0 == so && 0 == wo) return 'Greenwich Standard Time';
        if (60 == so && 0 == wo) return 'W. Europe Standard Time';
        if (60 == so && 60 == wo) return 'W. Central Africa Standard Time';
        if (60 == so && 120 == wo) return 'South Africa Standard Time';
        if (120 == so && 60 == wo) return 'W. Europe Standard Time';
        if (120 == so && 120 == wo) return 'South Africa Standard Time';
        if (180 == so && 120 == wo) return 'E. Europe Standard Time';
        if (180 == so && 180 == wo) return 'E. Africa Standard Time';
        if (240 == so && 180 == wo) return 'Russian Standard Time';
        if (240 == so && 240 == wo) return 'Arabian Standard Time';
        if (270 == so && 210 == wo) return 'Afghanistan Standard Time';
        if (270 == so && 270 == wo) return 'Afghanistan Standard Time';
        if (300 == so && 240 == wo) return 'Azerbaijan Standard Time';
        if (300 == so && 300 == wo) return 'Pakistan Standard Time';
        if (330 == so && 330 == wo) return 'India Standard Time';
        if (345 == so && 345 == wo) return 'Nepal Standard Time';
        if (360 == so && 300 == wo) return 'Central Asia Standard Time';
        if (360 == so && 360 == wo) return 'Central Asia Standard Time';
        if (390 == so && 390 == wo) return 'Myanmar Standard Time';
        if (420 == so && 360 == wo) return 'North Asia Standard Time';
        if (420 == so && 420 == wo) return 'SE Asia Standard Time';
        if (480 == so && 420 == wo) return 'North Asia Standard Time';
        if (480 == so && 480 == wo) return 'W. Australia Standard Time';
        if (540 == so && 480 == wo) return 'North Asia East Standard Time';
        if (540 == so && 540 == wo) return 'Tokyo Standard Time';
        if (570 == so && 570 == wo) return 'AUS Central Standard Time';
        if (570 == so && 630 == wo) return 'Cen. Australia Standard Time';
        if (600 == so && 540 == wo) return 'Yakutsk Standard Time';
        if (600 == so && 600 == wo) return 'E. Australia Standard Time';
        if (600 == so && 660 == wo) return 'AUS Eastern Standard Time';
        if (630 == so && 660 == wo) return 'AUS Eastern Standard Time';
        if (660 == so && 600 == wo) return 'Vladivostok Standard Time';
        if (660 == so && 660 == wo) return 'Central Pacific Standard Time';
        if (690 == so && 690 == wo) return 'Central Pacific Standard Time';
        if (720 == so && 660 == wo) return 'Central Pacific Standard Time';
        if (720 == so && 720 == wo) return 'Fiji Standard Time';
        if (720 == so && 780 == wo) return 'New Zealand Standard Time';
        if (765 == so && 825 == wo) return 'Tonga Standard Time';
        if (780 == so && 780 == wo) return 'Tonga Standard Time'
        if (840 == so && 840 == wo) return 'Tonga Standard Time';
        return 'Pacific Standard Time';
    }

</script>
<html>
<head>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:hiddenfield id="isRedirectUponLogin" runat="server" />
    </form>
</body>
</html>
