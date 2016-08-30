<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.CargoConfiguration>" %>
<%@ Import Namespace="Iata.IS.Web.UIModel" %>
<script type="text/javascript">

    $('#PrimeBillingIsIdecMigrationStatusId').change(function () {
        if ($('#PrimeBillingIsIdecMigrationStatusId').val() == 3) {
            $('#cgoPrimeBillingIsIdecCertifiedOn').datepicker('enable');
            $('#PrimeBillingIsIdecMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoPrimeBillingIsIdecCertifiedOn').datepicker('disable');
            $('#PrimeBillingIsIdecMigratedDate').attr('disabled', 'disabled');
            $('#cgoPrimeBillingIsIdecCertifiedOn').val("");
            $('#PrimeBillingIsIdecMigratedDate').val("");
            $('#cgoPrimeBillingIsIdecCertifiedOn').watermark(_dateWatermark);
            $('#PrimeBillingIsIdecMigratedDate').watermark(_periodFormat);
        }
    });
    $('#PrimeBillingIsxmlMigrationStatusId').change(function () {
        if ($('#PrimeBillingIsxmlMigrationStatusId').val() == 3) {
            $('#cgoPrimeBillingIsxmlCertifiedOn').datepicker('enable');
            $('#PrimeBillingIsxmlMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoPrimeBillingIsxmlCertifiedOn').datepicker('disable');
            $('#PrimeBillingIsxmlMigratedDate').attr('disabled', 'disabled');
            $('#cgoPrimeBillingIsxmlCertifiedOn').val("");
            $('#PrimeBillingIsxmlMigratedDate').val("");
            $('#cgoPrimeBillingIsxmlCertifiedOn').watermark(_dateWatermark);
            $('#PrimeBillingIsxmlMigratedDate').watermark(_periodFormat);
        }
    });
    $('#RmIsIdecMigrationStatusId').change(function () {
        if ($('#RmIsIdecMigrationStatusId').val() == 3) {
            $('#cgoRmIsIdecCertifiedOn').datepicker('enable');
            $('#RmIsIdecMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoRmIsIdecCertifiedOn').datepicker('disable');
            $('#RmIsIdecMigratedDate').attr('disabled', 'disabled');
            $('#cgoRmIsIdecCertifiedOn').val("");
            $('#RmIsIdecMigratedDate').val("");
            $('#cgoRmIsIdecCertifiedOn').watermark(_dateWatermark);
            $('#RmIsIdecMigratedDate').watermark(_periodFormat);
        }
    });
    $('#RmIsXmlMigrationStatusId').change(function () {
        if ($('#RmIsXmlMigrationStatusId').val() == 3) {
            $('#cgoRmIsXmlCertifiedOn').datepicker('enable');
            $('#RmIsXmlMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoRmIsXmlCertifiedOn').datepicker('disable');
            $('#RmIsXmlMigratedDate').attr('disabled', 'disabled');
            $('#cgoRmIsXmlCertifiedOn').val("");
            $('#RmIsXmlMigratedDate').val("");
            $('#cgoRmIsXmlCertifiedOn').watermark(_dateWatermark);
            $('#RmIsXmlMigratedDate').watermark(_periodFormat);
        }
    });
    $('#BmIsIdecMigrationStatusId').change(function () {
        if ($('#BmIsIdecMigrationStatusId').val() == 3) {
            $('#cgoBmIsIdecCertifiedOn').datepicker('enable');
            $('#BmIsIdecMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoBmIsIdecCertifiedOn').datepicker('disable');
            $('#BmIsIdecMigratedDate').attr('disabled', 'disabled');
            $('#cgoBmIsIdecCertifiedOn').val("");
            $('#BmIsIdecMigratedDate').val("");
            $('#cgoBmIsIdecCertifiedOn').watermark(_dateWatermark);
            $('#BmIsIdecMigratedDate').watermark(_periodFormat);
        }
    });
    $('#BmIsXmlMigrationStatusId').change(function () {
        if ($('#BmIsXmlMigrationStatusId').val() == 3) {
            $('#cgoBmIsXmlCertifiedOn').datepicker('enable');
            $('#BmIsXmlMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoBmIsXmlCertifiedOn').datepicker('disable');
            $('#BmIsXmlMigratedDate').attr('disabled', 'disabled');
            $('#cgoBmIsXmlCertifiedOn').val("");
            $('#BmIsXmlMigratedDate').val("");
            $('#cgoBmIsXmlCertifiedOn').watermark(_dateWatermark);
            $('#BmIsXmlMigratedDate').watermark(_periodFormat);
        }
    });
    $('#CmIsIdecMigrationStatusId').change(function () {
        if ($('#CmIsIdecMigrationStatusId').val() == 3) {
            $('#cgoCmIsIdecCertifiedOn').datepicker('enable');
            $('#CmIsIdecMigratedDate').removeAttr('disabled');
        }
        else {
            $('#cgoCmIsIdecCertifiedOn').datepicker('disable');
            $('#CmIsIdecMigratedDate').attr('disabled', 'disabled');
            $('#cgoCmIsIdecCertifiedOn').val("");
            $('#CmIsIdecMigratedDate').val("");
            $('#cgoCmIsIdecCertifiedOn').watermark(_dateWatermark);
            $('#CmIsIdecMigratedDate').watermark(_periodFormat);
        }
    });
    $('#CmIsXmlMigrationStatusId').change(function () {
        if ($('#CmIsXmlMigrationStatusId').val() == 3) {
            $('#cgoCmIsXmlCertifiedOn').datepicker('enable');
            $('#CmIsXmlMigratedDate').removeAttr('disabled');

        }
        else {
            $('#cgoCmIsXmlCertifiedOn').datepicker('disable');
            $('#CmIsXmlMigratedDate').attr('disabled', 'disabled');
            $('#cgoCmIsXmlCertifiedOn').val("");
            $('#CmIsXmlMigratedDate').val("");
            $('#cgoCmIsXmlCertifiedOn').watermark(_dateWatermark);
            $('#CmIsXmlMigratedDate').watermark(_periodFormat);

        }
    });
    $("#cgoPrimeBillingIsxmlCertifiedOn").datepicker("option", "disabled", true);
    $('#PrimeBillingIswebMigratedDate').watermark(_periodFormat);
    $('#RmIswebMigratedDate').watermark(_periodFormat);
    $('#BmIswebMigratedDate').watermark(_periodFormat);
    $('#CmIswebMigratedDate').watermark(_periodFormat);

</script>
<div>
    <table>
        <thead align="center" valign="middle">
            <tr>
                <td style="width: 150px;">
                </td>
                <td style="font-weight: bold; width: 150px;">
                    IS-IDEC Certification Status
                </td>
                <td style="font-weight: bold; width: 200px; text-align: left;">
                    IS-IDEC Certified On
                </td>
                <td style="font-weight: bold; width: 200px; text-align: left;">
                    IS-IDEC Migration Period
                </td>
                <td style="font-weight: bold; width: 150px; text-align: left;">
                    IS-XML Certification Status
                </td>
                <td style="font-weight: bold; width: 200px; text-align: left;">
                    IS-XML Certified On
                </td>
                <td style="font-weight: bold; width: 200px; text-align: left;">
                    IS-XML Migration Period
                </td>
                 <td style="font-weight: bold; width: 200px; text-align: left;">
                    IS-WEB Migration Period
                </td>
            </tr>
        </thead>
        <tbody align="center" valign="middle">
            <tr>
                <td style="font-weight: bold; text-align: left;">
                    Prime Billing
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIsIdecMigrationStatusId, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "PrimeBillingIsIdecMigrationStatusId" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoPrimeBillingIsIdecCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "PrimeBillingIsIdecMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIsxmlMigrationStatusId, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "PrimeBillingIsxmlMigrationStatusId" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIsxmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoPrimeBillingIsxmlCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIsxmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "PrimeBillingIsxmlMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                 <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.PrimeBillingIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "PrimeBillingIswebMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; text-align: left;">
                    RM
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoRmIsIdecCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "RmIsIdecMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIsXmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoRmIsXmlCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIsXmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "RmIsXmlMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                 <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.RmIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "RmIswebMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; text-align: left;">
                    BM
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoBmIsIdecCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BmIsIdecMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIsXmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoBmIsXmlCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIsXmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BmIsXmlMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                 <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.BmIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BmIswebMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; text-align: left;">
                    CM
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoCmIsIdecCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CmIsIdecMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIsXmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "cgoCmIsXmlCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                </td>
                <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIsXmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CmIsXmlMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
                 <td style="text-align: left;">
                    <%:Html.ProfileFieldFor(cgoMigrate => cgoMigrate.CmIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CmIswebMigratedDate" }, { "class", "smallTextField" } }, null, null, null, true)%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
