<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.EBillingConfiguration>" %>

<div id="dialogLocationAssociation">

<div class="buttonContainer">
                        <div style="width: 100%; height: 20px;">                            
                            <input type="hidden" id= "hdnUserID" />
                            <input type="hidden" id= "hdnEmailId" />
                            <input type="hidden" id= "hdnMemberId" />
                        </div>
                        <br />                       
                       <div  style="width: 380px;  display: inherit; float: none;">
                          
                            <div id="LocAssociationType" >
                            Location(s) for which MISC Receivables Archiving Required:<br />
                                <div>
                                    <input type="radio" name="AssociationType" id="radNone" value="1" />                          
                                    &nbsp;None 
                                </div>
                                <br />
                                <div>
                                    <input type="radio" name="AssociationType" id="radAllLocation" value="2" />                                    
                                    &nbsp;All Locations
                                </div>
                                <br />
                                <div>
                                    <input type="radio" name="AssociationType" id="radSpecificLocation" value="3" />                                    
                                    &nbsp;Specific Location(s)
                                </div>
                          </div>
                            </div>
                            <br />                            
                            <div id="locationListBox" style="width: 280px; margin-top:10px;">
                                <table>
                                    <tr>
                                        <td>
                                            MISC Receivables Archiving Not Required for Location IDs:
                                            <%:Html.ListBox("UnAssociatedLocation", (MultiSelectList)ViewData["UnAssociatedLocation"], new {@style = "width: 200px;height:180px;" })%>
                                        </td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value=">" id="add"/>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value=">>" id="addAll" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value="<" id="remove" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value="<<" id="removeAll" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>                                        
                                        <td>
                                            MISC Receivables Archiving Required for Location IDs:
                                            <%:Html.ListBox("AssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new {@style = "width: 200px;height:180px;" })%>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>                            
      
</div>

