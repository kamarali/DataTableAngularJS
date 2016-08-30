<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
 <%--CMP #675: Progress Status Bar for Processing of Billing Data Files. 
 Desc: User control for Progress Bar. --%>

<div class="progress" style="height:110px;width:600px;" align="left">
    
    <div class="circle done">
    <span class="label" id="sanity" >1</span>
    <div style="padding:5px"></div>
    <span class="title" style="left">Sanity Check</span>
	<span class="Status" id="sanityStatus"></span>	    
	<span class="Status" id="sanityQStatus"></span>	    
    </div>

    <span class="bar done"> </span>

    <div class="circle done">
    <span class="label" id="validation" >2</span>
    <div style="padding:5px"></div>
    <span class="title" >Validation</span>
	<span class="Status" id="validationStatus"></span>
    <span class="Status" id="validationQStatus"></span>	    
    </div>

    <span class="bar"></span>
    
    <div class="circle active">
    <span class="label"  id="loading">3</span>
    <div style="padding:5px"></div>
    <span class="title" >Loading</span>
	<span class="Status" id="loadingStatus"></span>
	<span class="Status" id="loadingQStatus"></span>    
    </div>

    <span class="bar"></span>
    
    <div class="circle">
    <span class="label" id="finalUpdate" >4</span>
    <div style="padding:5px"></div>
    <span class="title">Final Update</span>
	<span class="Status"id="finalUpdateStatus"></span>
	<span class="Status"id="finalUpdateQStatus"></span>    
    </div>

</div>
