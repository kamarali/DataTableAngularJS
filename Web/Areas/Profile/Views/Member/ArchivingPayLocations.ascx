<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.EBillingConfiguration>" %>

<div id="dialogLocationAssociation">

<div class="buttonContainer">
                        <div style="width: 100%; height: 20px;">                                                        
                            <input type="hidden" id= "hdnMemberId" />
                        </div>
                        <br />                       
                       <div  style="width: 380px;  display: inherit; float: none;">
                          
                            <div id="LocAssociationTypePay" >
                            Location(s) for which MISC Payables Archiving Required:<br />
                                <div>
                                    <input type="radio" name="PayAssociationType" id="radNonePay" value="1" />                          
                                    &nbsp;None 
                                </div>
                                <br />
                                <div>
                                    <input type="radio" name="PayAssociationType" id="radAllLocationPay" value="2" />                                    
                                    &nbsp;All Locations
                                </div>
                                <br />
                                <div>
                                    <input type="radio" name="PayAssociationType" id="radSpecificLocationPay" value="3" />                                    
                                    &nbsp;Specific Location(s)
                                </div>
                          </div>
                            </div>
                            <br />                            
                            <div id="locationListBoxPay" style="width: 280px; margin-top:10px;">
                                <table>
                                    <tr>
                                        <td>
                                            MISC Payables Archiving Not Required for Location IDs:
                                            <%:Html.ListBox("UnAssociatedPayLocation", (MultiSelectList)ViewData["UnAssociatedPayLocation"], new { @style = "width: 200px;height:180px;" })%>
                                        </td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value=">" id="addPay"/>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value=">>" id="addAllPay" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value="<" id="removePay" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <input type="button" class="shiftButton" value="<<" id="removeAllPay" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>                                        
                                        <td>
                                            MISC Payables Archiving Required for Location IDs:
                                            <%:Html.ListBox("AssociatedPayLocation", (MultiSelectList)ViewData["AssociatedPayLocation"], new { @style = "width: 200px;height:180px;" })%>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>                            
      
</div>

