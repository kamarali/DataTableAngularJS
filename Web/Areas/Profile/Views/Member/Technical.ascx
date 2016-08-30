<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.TechnicalConfiguration>" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {

        // Control here Save button on the basis of Selected Member ID 
        if ($('#selectedMemberId').val() == 0) {
            $("#btnSaveTechDetails").attr('disabled', 'disabled');
            $("#btnSaveTechDetails").removeClass('ignoredirty');
        }
        else {
            $("#btnSaveTechDetails").removeAttr('disabled');
            $("#btnSaveTechDetails").addClass('ignoredirty');
        }

        $('#PaxAccountId').focus();
      });

      function ValidateAccountIds() {
          //Discussed with shambhu ji, we will display message for each condition where account id is blank or not.
          //These changes have done while working CMP #625
          if (confirm("These changes will be reflected in EBilling-Tab. Do you want to continue?")) {
            $("#technical").submit();
          }    
      }
</script>
<%
  using (Html.BeginForm("Technical", "Member", FormMethod.Post, new { id = "technical" }))
  {%>
  <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
    </div>
    <div>
      <h2>
        Output Files via iiNet</h2>
    </div>
  </div>
  <div class="fieldContainer horizontalFlow">
    <table>
      <thead align="center" valign="middle">
        <tr>
          <td style="width: 70px;">
          </td>
          <td style="font-weight: bold; width: 150px;">
            Account ID
          </td>
        </tr>
      </thead>
      <tbody align="left" valign="middle">
        <tr>
          <td style="font-weight: bold; text-align: left;">
            PAX
          </td>
          <td>
            <%: Html.ProfileFieldFor(techModel => techModel.PaxAccountId, "", Model.UserCategory, skipContainer: true)%>
          </td>
        </tr>
        <tr>
          <td style="font-weight: bold; text-align: left;">
            CGO
          </td>
          <td>
            <%: Html.ProfileFieldFor(techModel => techModel.CgoAccountId, "", Model.UserCategory,  skipContainer: true)%>
          </td>
        </tr>
        <tr>
          <td style="font-weight: bold; text-align: left;">
            MISC
          </td>
          <td>
            <%: Html.ProfileFieldFor(techModel => techModel.MiscAccountId, "", Model.UserCategory,  skipContainer: true)%>
          </td>
        </tr>
        <tr>
          <td style="font-weight: bold; text-align: left;">
            UATP
          </td>
          <td>
            <%: Html.ProfileFieldFor(techModel => techModel.UatpAccountId, "", Model.UserCategory,  skipContainer: true)%>
          </td>
        </tr>
        <tr>
          <td> <%: Html.Hidden("MemberId", Model.MemberId)%>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
  <div class="clear">
  </div>
</div>

<div class="buttonContainer">
   <input class="primaryButton" id="btnSaveTechDetails" type="button" value="Save Technical Details"
        onclick="return ValidateAccountIds();" />
</div>

<%}  %>