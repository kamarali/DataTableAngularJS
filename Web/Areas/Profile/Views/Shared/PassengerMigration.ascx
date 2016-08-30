<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.PassengerConfiguration>" %>
<script type="text/javascript">

  $('#NonSamplePrimeBillingIsIdecMigrationStatusId').change(function () {
    if ($('#NonSamplePrimeBillingIsIdecMigrationStatusId').val() == 3) {
      $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').datepicker('enable');
      $('#NonSamplePrimeBillingIsIdecMigratedDate').removeAttr('disabled');

    }
    else {
      $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').datepicker('disable');
      $('#NonSamplePrimeBillingIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').val("");
      $('#NonSamplePrimeBillingIsIdecMigratedDate').val("");
      $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#NonSamplePrimeBillingIsIdecMigratedDate').watermark(_periodFormat);

    }
  });
  $('#NonSamplePrimeBillingIsxmlMigrationStatusId').change(function () {
    if ($('#NonSamplePrimeBillingIsxmlMigrationStatusId').val() == 3) {
      $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').datepicker('enable');
      $('#NonSamplePrimeBillingIsxmlMigratedDate').removeAttr('disabled');

    }
    else {
      $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').datepicker('disable');
      $('#NonSamplePrimeBillingIsxmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').val("");
      $('#NonSamplePrimeBillingIsxmlMigratedDate').val("");
      $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').watermark(_dateWatermark);
      $('#NonSamplePrimeBillingIsxmlMigratedDate').watermark(_periodFormat);
    }

  });
  $('#SamplingProvIsIdecMigrationStatusId').change(function () {
    if ($('#SamplingProvIsIdecMigrationStatusId').val() == 3) {

      $('#PaxSamplingProvIsIdecCerfifiedOn').datepicker('enable');
      $('#SamplingProvIsIdecMigratedDate').removeAttr('disabled');

    }
    else {
      $('#PaxSamplingProvIsIdecCerfifiedOn').datepicker('disable');
      $('#SamplingProvIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxSamplingProvIsIdecCerfifiedOn').val("");
      $('#SamplingProvIsIdecMigratedDate').val("");
      $('#PaxSamplingProvIsIdecCerfifiedOn').watermark(_dateWatermark);
      $('#SamplingProvIsIdecMigratedDate').watermark(_periodFormat);
    }

  });
  $('#SamplingProvIsxmlMigrationStatusId').change(function () {
    if ($('#SamplingProvIsxmlMigrationStatusId').val() == 3) {
      $('#PaxSamplingProvIsxmlCertifiedOn').datepicker('enable');
      $('#SamplingProvIsxmlMigratedDate').removeAttr('disabled');

    }
    else {
      $('#PaxSamplingProvIsxmlCertifiedOn').datepicker('disable');
      $('#SamplingProvIsxmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxSamplingProvIsxmlCertifiedOn').val("");
      $('#SamplingProvIsxmlMigratedDate').val("");
      $('#PaxSamplingProvIsxmlCertifiedOn').watermark(_dateWatermark);
      $('#SamplingProvIsxmlMigratedDate').watermark(_periodFormat);


    }
  });
  $('#NonSampleRmIsIdecMigrationStatusId').change(function () {
    if ($('#NonSampleRmIsIdecMigrationStatusId').val() == 3) {
      $('#PaxNonSampleRmIsIdecCertifiedOn').datepicker('enable');
      $('#NonSampleRmIsIdecMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxNonSampleRmIsIdecCertifiedOn').datepicker('disable');
      $('#NonSampleRmIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSampleRmIsIdecCertifiedOn').val("");
      $('#NonSampleRmIsIdecMigratedDate').val("");
      $('#PaxNonSampleRmIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#NonSampleRmIsIdecMigratedDate').watermark(_periodFormat);

    }
  });
  $('#NonSampleRmIsXmlMigrationStatusId').change(function () {
    if ($('#NonSampleRmIsXmlMigrationStatusId').val() == 3) {
      $('#PaxNonSampleRmIsXmlCertifiedOn').datepicker('enable');
      $('#NonSampleRmIsXmlMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxNonSampleRmIsXmlCertifiedOn').datepicker('disable');
      $('#NonSampleRmIsXmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSampleRmIsXmlCertifiedOn').val("");
      $('#NonSampleRmIsXmlMigratedDate').val("");
      $('#PaxNonSampleRmIsXmlCertifiedOn').watermark(_dateWatermark);
      $('#NonSampleRmIsXmlMigratedDate').watermark(_periodFormat);
    }
  });
  $('#NonSampleBmIsIdecMigrationStatusId').change(function () {
    if ($('#NonSampleBmIsIdecMigrationStatusId').val() == 3) {
      $('#PaxNonSampleBmIsIdecCertifiedOn').datepicker('enable');
      $('#NonSampleBmIsIdecMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxNonSampleBmIsIdecCertifiedOn').datepicker('disable');
      $('#NonSampleBmIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSampleBmIsIdecCertifiedOn').val("");
      $('#NonSampleBmIsIdecMigratedDate').val("");
      $('#PaxNonSampleBmIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#NonSampleBmIsIdecMigratedDate').watermark(_periodFormat);

    }
  });
  $('#NonSampleBmIsXmlMigrationStatusId').change(function () {
    if ($('#NonSampleBmIsXmlMigrationStatusId').val() == 3) {
      $('#PaxNonSampleBmIsxmlCertifiedOn').datepicker('enable');
      $('#NonSampleBmIsXmlMigratedDate').removeAttr('disabled');

    }
    else {
      $('#PaxNonSampleBmIsxmlCertifiedOn').datepicker('disable');
      $('#NonSampleBmIsXmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSampleBmIsxmlCertifiedOn').val("");
      $('#NonSampleBmIsXmlMigratedDate').val("");
      $('#PaxNonSampleBmIsxmlCertifiedOn').watermark(_dateWatermark);
      $('#NonSampleBmIsXmlMigratedDate').watermark(_periodFormat);
    }
  });
  $('#NonSampleCmIsIdecMigrationStatusId').change(function () {
    if ($('#NonSampleCmIsIdecMigrationStatusId').val() == 3) {
      $('#PaxNonSampleCmIsIdecCertifiedOn').datepicker('enable');
      $('#NonSampleCmIsIdecMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxNonSampleCmIsIdecCertifiedOn').datepicker('disable');
      $('#NonSampleCmIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSampleCmIsIdecCertifiedOn').val("");
      $('#NonSampleCmIsIdecMigratedDate').val("");
      $('#PaxNonSampleCmIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#NonSampleCmIsIdecMigratedDate').watermark(_periodFormat);

    }
  });

  $('#NonSampleCmIsXmlMigrationStatusId').change(function () {
    if ($('#NonSampleCmIsXmlMigrationStatusId').val() == 3) {
      $('#PaxNonSampleCmIsXmlCertifiedOn').datepicker('enable');
      $('#NonSampleCmIsXmlMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxNonSampleCmIsXmlCertifiedOn').datepicker('disable');
      $('#NonSampleCmIsXmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxNonSampleCmIsXmlCertifiedOn').val("");
      $('#NonSampleCmIsXmlMigratedDate').val("");
      $('#PaxNonSampleCmIsXmlCertifiedOn').watermark(_dateWatermark);
      $('#NonSampleCmIsXmlMigratedDate').watermark(_periodFormat);
    }
  });

  $('#SampleFormCIsIdecMigrationStatusId').change(function () {
    if ($('#SampleFormCIsIdecMigrationStatusId').val() == 3) {
      $('#PaxSampleFormCIsIdecCertifiedOn').datepicker('enable');
      $('#SampleFormCIsIdecMigratedDate').removeAttr('disabled');

    }
    else {
      $('#PaxSampleFormCIsIdecCertifiedOn').datepicker('disable');
      $('#SampleFormCIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxSampleFormCIsIdecCertifiedOn').val("");
      $('#SampleFormCIsIdecMigratedDate').val("");
      $('#PaxSampleFormCIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#SampleFormCIsIdecMigratedDate').watermark(_periodFormat);

    }
  });
  $('#SampleFormCIsxmlMigrationStatusId').change(function () {
    if ($('#SampleFormCIsxmlMigrationStatusId').val() == 3) {
      $('#PaxSampleFormCIsxmlCertifiedOn').datepicker('enable');
      $('#SampleFormCIsxmlMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxSampleFormCIsxmlCertifiedOn').datepicker('disable');
      $('#SampleFormCIsxmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxSampleFormCIsxmlCertifiedOn').val("");
      $('#SampleFormCIsxmlMigratedDate').val("");
      $('#PaxSampleFormCIsxmlCertifiedOn').watermark(_dateWatermark);
      $('#SampleFormCIsxmlMigratedDate').watermark(_periodFormat);
    }
  });
  $('#SampleFormDeIsIdecMigrationStatusId').change(function () {
    if ($('#SampleFormDeIsIdecMigrationStatusId').val() == 3) {
      $('#PaxSampleFormDeIsIdecCertifiedOn').datepicker('enable');
      $('#SampleFormDeIsIdecMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxSampleFormDeIsIdecCertifiedOn').datepicker('disable');
      $('#SampleFormDeIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxSampleFormDeIsIdecCertifiedOn').val("");
      $('#SampleFormDeIsIdecMigratedDate').val("");
      $('#PaxSampleFormDeIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#SampleFormDeIsIdecMigratedDate').watermark(_periodFormat);
    }
  });

  $('#SampleFormDEisxmlMigrationStatusId').change(function () {
    if ($('#SampleFormDEisxmlMigrationStatusId').val() == 3) {
      $('#PaxSampleFormDeIsxmlCertifiedOn').datepicker('enable');
      $('#SampleFormDeIsxmlMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxSampleFormDeIsxmlCertifiedOn').datepicker('disable');
      $('#SampleFormDeIsxmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxSampleFormDeIsxmlCertifiedOn').val("");
      $('#SampleFormDeIsxmlMigratedDate').val("");
      $('#PaxSampleFormDeIsxmlCertifiedOn').watermark(_dateWatermark);
      $('#SampleFormDeIsxmlMigratedDate').watermark(_periodFormat);
    }
  });
  $('#SampleFormFxfIsIdecMigrationStatusId').change(function () {
    if ($('#SampleFormFxfIsIdecMigrationStatusId').val() == 3) {
      $('#PaxSampleFormFxfIsIdecCertifiedOn').datepicker('enable');
      $('#SampleFormFxfIsIdecMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxSampleFormFxfIsIdecCertifiedOn').datepicker('disable');
      $('#SampleFormFxfIsIdecMigratedDate').attr('disabled', 'disabled');
      $('#PaxSampleFormFxfIsIdecCertifiedOn').val("");
      $('#SampleFormFxfIsIdecMigratedDate').val("");
      $('#PaxSampleFormFxfIsIdecCertifiedOn').watermark(_dateWatermark);
      $('#SampleFormFxfIsIdecMigratedDate').watermark(_periodFormat);
    }
  });
  $('#SampleFormFxfIsxmlMigratedStatusId').change(function () {
    if ($('#SampleFormFxfIsxmlMigratedStatusId').val() == 3) {
      $('#PaxSampleFormFxfIsxmlCertifiedOn').datepicker('enable');
      $('#SampleFormFxfIsxmlMigratedDate').removeAttr('disabled');
    }
    else {
      $('#PaxSampleFormFxfIsxmlCertifiedOn').datepicker('disable');
      $('#SampleFormFxfIsxmlMigratedDate').attr('disabled', 'disabled');
      $('#PaxSampleFormFxfIsxmlCertifiedOn').val("");
      $('#SampleFormFxfIsxmlMigratedDate').val("");
      $('#PaxSampleFormFxfIsxmlCertifiedOn').watermark(_dateWatermark);
      $('#SampleFormFxfIsxmlMigratedDate').watermark(_periodFormat);
    }
    });
$('#NonSamplePrimeBillingIswebMigratedDate').watermark(_periodFormat);
$('#SamplingProvIswebMigratedDate').watermark(_periodFormat);
$('#NonSampleRmIswebMigratedDate').watermark(_periodFormat);
$('#NonSampleBmIswebMigratedDate').watermark(_periodFormat);
$('#NonSampleCmIswebMigratedDate').watermark(_periodFormat);
$('#SampleFormCIswebMigratedDate').watermark(_periodFormat);
$('#SampleFormDeIswebMigratedDate').watermark(_periodFormat);
$('#SampleFormFxfIswebMigratedDate').watermark(_periodFormat);
    </script>
<div>
  <table>
    <thead align="center" valign="middle">
      <tr>
        <td style="width: 220px;">
        </td>
        <td style="font-weight: bold; width: 150px;">
          IS-IDEC Certification Status
        </td>
        <td style="font-weight: bold; width: 200px; text-align:left;">
          IS-IDEC Certified On
        </td>
        <td style="font-weight: bold; width: 200px;text-align:left;">
          IS-IDEC Migration Period
        </td>
        <td style="font-weight: bold; width: 150px;text-align:left;">
          IS-XML Certification Status
        </td>
        <td style="font-weight: bold; width: 200px;text-align:left;">
          IS-XML Certified On
        </td>
        <td style="font-weight: bold; width: 200px;text-align:left;">
          IS-XML Migration Period
        </td>
         <td style="font-weight: bold; width: 200px;text-align:left;">
          IS-WEB Migration Period
        </td>
      </tr>
    </thead>
    <tbody align="center" valign="middle">
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Non-Sampling Prime Billing
        </td>
        <td style="text-align:left;" >
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIsIdecMigrationStatusId,null,SessionUtil.UserCategory,null,null,null,null,true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object>{{"class","datePicker mediumTextField"},{"id","PaxNonSamplePrimeBillingIsIdecCertifiedOn"},{"readOnly","true"},{"datepicker","disable"}}, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object>{{"class","smallTextField"}}, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIsxmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIsxmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker" }, { "id", "PaxNonSamplePrimeBillingIsxmlCertifiedOn" }, { "readOnly", "true" }}, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIsxmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSamplePrimeBillingIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Sampling Provisional Billing
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIsIdecCerfifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker" }, { "id", "PaxSamplingProvIsIdecCerfifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIsxmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIsxmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSamplingProvIsxmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIsxmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
       <%--  <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SamplingProvIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>--%>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Non-Sampling RM
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxNonSampleRmIsIdecCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIsXmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxNonSampleRmIsXmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;" >
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIsXmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;" >
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleRmIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Non-Sampling BM
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxNonSampleBmIsIdecCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIsXmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxNonSampleBmIsxmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIsXmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
         <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleBmIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Non-Sampling CM
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxNonSampleCmIsIdecCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIsXmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxNonSampleCmIsXmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIsXmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.NonSampleCmIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Sampling Form C
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSampleFormCIsIdecCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIsxmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIsxmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSampleFormCIsxmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIsxmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormCIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Sampling Form D/E
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDeIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDeIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSampleFormDeIsIdecCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDeIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDEisxmlMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDeIsxmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSampleFormDeIsxmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDeIsxmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
         <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormDeIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
      <tr>
        <td style="font-weight: bold; text-align: left;">
          Sampling Form F/XF
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIsIdecMigrationStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIsIdecCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSampleFormFxfIsIdecCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIsIdecMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIsxmlMigratedStatusId, null, SessionUtil.UserCategory, null, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIsxmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "datePicker smallTextField" }, { "id", "PaxSampleFormFxfIsxmlCertifiedOn" }, { "readOnly", "true" } }, null, null, null, true)%>
        </td>
        <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIsxmlMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
         <td style="text-align:left;">
          <%:Html.ProfileFieldFor(paxMigrate => paxMigrate.SampleFormFxfIswebMigratedDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "smallTextField" } }, null, null, null, true)%>
        </td>
      </tr>
    </tbody>
  </table>
</div>
<script type="text/javascript">
  $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').datepicker('disable');
  $('#PaxSamplingProvIsIdecCerfifiedOn').datepicker('disable');
  $('#PaxSamplingProvIsxmlCertifiedOn').datepicker('disable');
  $('#PaxNonSampleRmIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSampleRmIsXmlCertifiedOn').datepicker('disable');
  $('#PaxNonSampleBmIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSampleBmIsxmlCertifiedOn').datepicker('disable');
  $('#PaxNonSampleCmIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSampleCmIsXmlCertifiedOn').datepicker('disable');
  $('#PaxSampleFormCIsIdecCertifiedOn').datepicker('disable');
  $('#PaxSampleFormCIsxmlCertifiedOn').datepicker('disable');
  $('#PaxSampleFormDeIsIdecCertifiedOn').datepicker('disable');
  $('#PaxSampleFormDeIsxmlCertifiedOn').datepicker('disable');
  $('#PaxSampleFormFxfIsIdecCertifiedOn').datepicker('disable');
  $('#PaxSampleFormFxfIsxmlCertifiedOn').datepicker('disable');
</script>
