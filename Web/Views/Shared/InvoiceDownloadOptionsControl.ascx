<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="solidBox dataEntry">
  <div class=" fieldContainer horizontalFlow">
    <table border="0" width="100%">
      <%
        var downloadOptionsDataTable = EnumMapper.GetInvoiceDownloadOptions((Iata.IS.Model.Enums.InvoiceDownloadOptions)Model);
      %>
      <%
        for (var i = 0; i < downloadOptionsDataTable.Count; i++)
        {
      %>
      <tr>
        <td>
          <%:downloadOptionsDataTable.ElementAt(i).Text%>
        </td>
        <td>
          <input type="checkbox" name="downloadOptions" id="<%:downloadOptionsDataTable.ElementAt(i).Value%>" />
        </td>
      </tr>
      <%
        }
      %>
    </table>
  </div>
  <div class="secondaryButtonContainer">
    <input type="submit" class="secondaryButton rightAlignedButton" id="selectAll" value="Select All" onclick="selectAllCheckBox();" />
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="button" value="Download Invoice" onclick="downloadInvoice();" />
  <input class="secondaryButton" type="button" value="Close" onclick="closeInvoiceDownloadOptions();" />
</div>
