<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.IsClearingHouseCalendar>" %>
<h2>
    Search Criteria</h2>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Calendar Year:</label>
                <%: Html.InvoiceYearDropdownListFor(searchCriteria => searchCriteria.ClearanceYear)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Calendar Type:</label>
                <select id="EventCategory" name="EventCategory">
                    <option value="">Please Select</option>
                    <option value="1">IS</option>
                    <option value="2">ICH</option>
                    <option value="3">ACH</option>
                    <option value="4">IS+ICH</option>
                    <option value="5">IS+ACH</option>
                    <option value="6">ICH+ACH</option>
                    <option value="7">IS+ICH+ACH</option>
                </select>
            </div>
            <div>
                <label>
                    Time Zone</label>
                <%: Html.DropDownListFor(searchCriteria => searchCriteria.TimeZone, ViewData["TimeZones"] as SelectList,new { style = "width:250px" })%> 
            </div>            
        </div>
    </div>
    <div class="clear">
    </div>
</div>
