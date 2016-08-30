﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.FileFormat>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Downloadable:</label>
                        <%: Html.DropDownListFor(model => model.SearchFileDownloadable, ViewData["SearchFileDownloadable"] as SelectList)%> 
                    </div>
                    <div>
                        <label>
                            Description:</label>
                        <%: Html.TextBoxFor(model => model.Description, new { @id = "Description", @maxLength = 255 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
