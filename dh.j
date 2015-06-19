<!DOCTYPE html>

<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width">
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" href="css/bootstrap-theme.min.css">
    <!--[if lt IE 9]>
    <script src="http://code.jquery.com/jquery-1.10.2.min.js"></script>
    <![endif]-->
    <link rel="stylesheet" href="css/ng-table.css">
    <link rel="stylesheet" href="css/custom.css">
</head>

<body ng-app="main" >
    <div ng-controller="Promotion_ctrl">
        <div id="wrapper">
            <nav class="navbar main_header">
                <div class="navbar-header">
                    <span class="navbar-brand sub_head">Select BU : 
						<select class="select_list" ng-model="ddlbu" ng-options="d.BUCode as d.BUDescription for d in data" ng-change = "binddropdown()">
							<option value="">Select a BU</option>
						</select>
					</span>
                    <span class="navbar-brand sub_head">Select DU : 
						
						<select class="select_list" ng-model="ddldu" ng-change = "bindempgrid(); bindmgrdropdownlist(); bindbudgetdetails()">
							<option value="">Select a DU</option>
							<option value ="ALL">All</option>
							<option ng-repeat= "item in duarray" value="{{item.DUCode}}">{{item.DUDescription}}</option>
						</select>
					
						<!-- <select class="select_list" ng-model="ddldu" ng-options="item.DUCode as item.DUDescription for item in duarray" ng-change = "bindempgrid(); bindmgrdropdownlist(); bindbudgetdetails()">
							<option value="">Select a DU</option>
						</select> -->
					</span>
                    <span class="navbar-brand sub_head">Select Manager : 
						<select class="select_list" ng-model="ddlmgrdetail" ng-options="item.Position as item.MgrName for item in mgrdata" ng-change="showemployees(ddlmgrdetail)">
							<option value="">Select Manager</option>
						</select>
					</span>
                </div>
                <span class="navbar-brand main_logo pull-right">APPRAISAL SYSTEM</span>
            </nav>
        </div>
		<input id="startspin" type="button" ng-click="startSpin()" style="visibility: hidden;" value="Start spinner" />
		<input id="stopspin" type="button" ng-click="stopSpin()" style="visibility: hidden;" value="Stop spinner" />

		<span us-spinner="{radius:30, width:8, length: 16}" spinner-key="spinner-1"></span>
		
		
        <div class="container-fluid" ng-hide="show"  >
		<div class="panel panel-default chart_panel">
               <div class="panel-heading">
                  <h4 class="panel-title">
                     <a data-toggle="collapse" data-parent="#accordion" href="#collapseChart">
						<strong>Budget (Utilized / Unutilized)  :  {{getTotal() | currency:"&#8377;"}}  /  {{totalBudget - getTotal() | currency:"&#8377;"}}</strong>
                     </a>
                  </h4>
               </div><!--/.panel-heading -->
               <div id="collapseChart" class="panel-collapse collapse in">
                  <div class="panel-body">
						<!-- Charts section goes here -->
            <div class="row chart_container">
                <div class="col-xs-3">
                    <span class="chart_header_text">PROMOTION + INCREMENT BUDGET</span>
                    <div class="row budget_div">
                        <div class="col-xs-4">
                            <div google-chart chart="piechart_budget" id="piechart_budget">
                            </div>
                        </div>
                        <div class="col-xs-8 budget_donut_legend">
                            <div><span class="label label-info">Available </span>&nbsp;&nbsp;<span class="legend_text">{{ totalBudget | currency:"&#8377;"}}</span></div>
                            <div><span class="label consumed">Consumed</span>&nbsp;&nbsp;<span class="legend_text">{{ getTotal() | currency:"&#8377;"}}</span></div>
                            <div><span class="label unutilized">Unutilized </span> &nbsp;&nbsp;<span class="legend_text">{{ totalBudget - getTotal() | currency:"&#8377;"}}</span></div>
                        </div>
                    </div>
                </div>
				

                
                <div class="col-xs-6" >
                    <span class="chart_header_text">PROMOTION DISTRIBUTION</span>
                    <div class="row promotion_div" ng-init="initPromotionChart()">
                        <div class="col-xs-2 location__distribution">
                            <strong>Location Wise</strong>
                            <div google-chart chart="piechart_promotion" id="piechart_promotion">
                            </div>
                        </div>
                        <div class="col-xs-2 promotion_donut_legend">
                            <div><span class="label nagpuar">Nagpur </span>&nbsp;&nbsp;<span class="legend_text">{{n | number:0}}%</span></div>
                            <div><span class="label hydrabad">Hyderabad</span>&nbsp;&nbsp;<span class="legend_text">{{h | number:0}}%</span></div>
                            <div><span class="label pune">Pune </span>&nbsp;&nbsp;<span class="legend_text">{{p | number:0}}%</span></div>
                        </div>
                        <div class="col-xs-6 grade__distribution" google-chart chart="barchart" style="height:120px; width:300px;"><strong>Grade Distribution</strong></div>
                        <div class="col-xs-2 gender_distribution"><strong>Gender </strong>
                            <br> <img src="images/female.png"> Female {{female_percent | number:0}}%
                            <br> <img src="images/male.png"> Male {{male_percent | number:0}}%</div>
                    </div>
                </div>

                <div class="col-xs-3">
                    <span class="chart_header_text">PYRAMID DISTRIBUTION</span>
                    <div class="pyramid_div">
                        <img class="" ng-src="images/pyramid.png" />
                    </div>
                </div>

            </div>
            <!-- End charts  section -->
                  </div><!--/.panel-body -->
               </div><!--/.panel-collapse -->
            </div><!-- /.panel -->
			
            <div class="row table_filters">
				<input class="hidden limit_counter" value={{totalRecords}} type="text" />
                 <span class="table_header_text">LIST OF EMPLOYEES</span>&nbsp;&nbsp;&nbsp;<span class="label label-total">Total Records : {{totalRecords}}</span>
                <div class="pull-right inline-right-buttons">
                    Select By :
                    <select class="select_list" id="list" ng-model="selectBy">
                        <option value="$">--All Records--</option>
                        <option value="empname">Employee Name</option>
                        <option value="grade">Grade</option>
                        <option value="band">Band</option>
                        <option value="empid">Employee ID</option>
						<option value="increment">Promotion Increment Amount</option>
						<option value="AnnualIncrement">Annual Increment Amount</option>
                        <option value="eligible">Eligible</option>
						<option value="IsIncrementEligible">Increment Eligible</option>
                        <option value="promote">Promote</option>
                        <option value="ctc_pa">CTC P.A</option>
                    </select>
                    <input type="text" class="advance_search_text" ng-model="search[selectBy]" placeholder="Search">
                    <input class="advance-search-icon" type="image" src="images/search.png">
                </div>
            </div>
            <div class="table-responsive" id="scrollable-area">
                <table ng-table="tableParams" class="table table-hover custom_table" fixed-table-headers="scrollable-area">
                   <tr class="{{selected}}" ng-class ="{'userclass' : user.promote == 'Y' ,'' : user.promote == 'N'}" ng-repeat="user in empdata | filter:search" ng-init="initDetails(user,$index)"  ng-click="moreDetails(user)">
                        <td data-title="'ID'" sortable="'empid'"><span>{{user.empid}}</span></td>
                        <td data-title="'Employee'" sortable="'empname'" ng-click="closePanel()" style="cursor:pointer;"><span>{{user.empname}}</span></td>
                        <td data-title="'Grade'" sortable="'grade'" class="center"><span>{{user.grade}}</span></td>
                        <td data-title="'Band'" sortable="'band'" class="center"><span>{{user.band || 'NA'}}</span></td>
                        <td data-title="'Promotion Eligible'" sortable="'eligible'" class="center"><span>{{user.eligible}}</span></td>
						<td data-title="'Increment Eligible'" sortable="'IsIncrementEligible'" class="center"><span>{{user.IsIncrementEligible}}</span></td>
                         <td data-title="'Promote'" sortable="'promote'" class="center">
							<span ng-if="!user.$editPromote" ng-click="user.$editPromote = restrictGrades(user)"><u>{{user.promote}}</u></span>
                            <div ng-if="user.$editPromote">
								<select class="form-control"  ng-model="user.promote" id="sel1" ng-blur="user.$editPromote = false;eligiblecheck(user)" ng-change="promote(user);update(user)">
										<option>Y</option>
										<option>N</option>
								 </select>
                            </div>
						</td>
                        <td data-title="'Increment %'" ng-model = "increment_percent" sortable="'increment_percent'" class="center"><span>{{user.increment_percent =((user.increment + user.AnnualIncrement)/user.ctc_pa)*100 | number : 1}}</span></td>
                       
						<td  data-title="'Promotion Increment'" sortable="'increment'" class="right">
                            <span ng-if="!user.$edit" ng-click="user.$edit = restrictGrades(user)"><u>{{user.increment}}</u></span>
                            <div ng-if="user.$edit">
                                <input class="form-control" type="number" step="{{increment_step}}" ng-model="user.increment" ng-blur="user.$edit = false; incrementedit(user);" ng-change="update(user)"  />
                            </div>
                        </td>
						
						<td  data-title="'Annual Increment'" sortable="'AnnualIncrement'" class="right">
                            <span ng-if="!user.$edit1" ng-click="user.$edit1 = restrictGrades(user)"><u>{{user.AnnualIncrement}}</u></span>
                            <div ng-if="user.$edit1">
                                <input class="form-control" type="number" step="{{increment_step}}" ng-model="user.AnnualIncrement" ng-blur="user.$edit1 = false; annualincrementedit(user)" ng-change="update(user)" />
                            </div>
                        </td>
						
                        <td data-title="'Current Anual Gross'" sortable="'current_AG'" class="right"><span>{{user.current_AG}}</span></td>
                        <td data-title="'CTC P.A'" sortable="'ctc_pa'" class="right"><span>{{user.ctc_pa}}</span></td>
                        <td data-title="'New CTC'" sortable="'new_ctc+promo_increment+anual_increment'" class="right"><span>{{ user.increment + user.AnnualIncrement + user.ctc_pa}}</span></td>
                        <td data-title="'Actions'" width="140">
                            <a ng-hide="{{'!USER_DP'}}" ng-click="finalized(user)"><img class="accept" title="Accept" id="accept" ng-model="user.finalized" ng-src="{{user.finalized == 1 && 'images/accepted.png' || 'images/accept_green.png'}}" /></a>
                            <a ng-hide="{{'!USER_DP'}}" ng-click="rejected(user)"><img class="reject" title="Reject" ng-model="user.rejected" ng-src="{{user.rejected == 1 && 'images/rejected.png' || 'images/reject_green.png'}}" /></a>
                          <!--   <a ng-hide="{{'!USER_DP'}}" ng-click="sentback(user)"><img class="back" title="Send Back" ng-model="user.sentback" ng-src="{{user.sentback == 1 && 'images/sentback.png' || 'images/send_back_green.png'}}" /></a> --> 
							<!-- <a ng-hide="true" ng-click="sentback(user)"><img class="back" title="Send Back" ng-model="user.sentback" ng-src="{{user.sentback == 1 && 'images/sentback.png' || 'images/send_back_green.png'}}" /></a> -->
                        </td>
                    </tr>
                </table>
            </div>
            <!-- Start More Details -->
            <div class="row more_details" ng-show="{{'showMoreDetails'}}">
                <div class="col-xs-1">
                    <img class="profile_pic" ng-src="{{empDetalis['more'][0].profile_pic || 'images/default.png'}}" />
                </div>
                <div class="col-xs-2">
                    Last Promotion Date:
                    <br>
                    <strong>{{empDetalis['more'].LastPromotionDate || 'NA'}}</strong>
                    <br> Months in Grade:
                    <br>
                    <strong>{{empDetalis['more'].MonthsInGrade || 'NA'}}</strong>
                    <br> Previous Band:
                    <br>
                    <strong>{{empDetalis['more'].LastBand || 'NA'}}</strong>
                </div>
                <div class="col-xs-2">
                    Leave Balance:
                    <br>
                    <strong>{{empDetalis['more'].LeaveBalance || 'NA'}}</strong>
                    <br> Joining Date:
                    <br>
                    <strong>{{empDetalis['more'].JoiningDate || 'NA'}}</strong>
                    <br> Total Experience:
                    <br>
                    <strong>{{empDetalis['more'].TotalExperience || 'NA'}}</strong>
                </div>
                <div class="col-xs-2">
                    Persistent Experience:
                    <br>
                    <strong>{{empDetalis['more'].PersistentExp || 'NA'}}</strong>
                    <br> Highest Qualification:
                    <br>
                    <strong>{{empDetalis['more'].HighestQualification || 'NA'}}</strong>
                    <br> Equivalent Year:
                    <br>
                    <strong>{{empDetalis['more'].EquivalentYear || 'NA'}}</strong>
                </div>
                <div class="col-xs-2">
                    25th Percentile:
                    <br>
                    <strong>{{empDetalis['more'].Percentile25 || 'NA'}}</strong>
                    <br> 75th Percentile:
                    <br>
                    <strong>{{empDetalis['more'].Percentile75 || 'NA'}}</strong>
                    <br> Position after Increment:
                    <br>
                    <strong>{{empDetalis['more'].PositionAfterIncrement || 'NA'}}</strong>
                </div>
                <div class="col-xs-3">
                    Previous Bands:
                    <div class="pull-right">
						 <a ng-click="closePanel()"><img class="closebtn_panel pull-right" title="Close" ng-src="{{'images/close_modal.png'}}" /></a>
                        <input type="button" ng-click="allDetails(empDetalis)" class="tms-button view-more" value="View More">
                    </div>
                    <br>
                    <strong>{{empDetalis['more'].LastBandsHistory || 'NA'}}</strong>
                    <br> Total Credit Points:
                    <br>
                    <strong>{{empDetalis['more'].TotalCreditPoints || 'NA'}}</strong>
                    <br> Last Year Credit Points:
                    <br>
                    <strong>{{empDetalis['more'].LastFYCreditPoints || 'NA'}}</strong>
                </div>
            </div>
            <!-- End more details -->
			  <div class="row action_buttons">
                <div class="pull-right inline-right-buttons">
                    <div class="action_legend">
                        <img class="accept" ng-src="images/accepted.png" /> Accept
                        <img class="reject" ng-src="images/rejected.png" /> Reject
                        <img ng-hide="true" class="back" ng-src="images/sentback.png" />
                    </div>
                    <input type="button" class="tms-button" value="Save Draft" ng-click="senddraftemplist()">
                    <input type="button" class="tms-button" value="Discard">
                    <input type="button" class="tms-button" value="Commit" ng-click="savecommitemplist()">
                </div>
            </div>
			
			

            <!-- Popup modal template -->
            <script type='text/ng-template' id='myModalContent'>
                <div class="modal-body custom-modal-body">
                    <a ng-click="cancel()"><img class="close_modal pull-right" title="Close" ng-src="{{'images/close_modal.png'}}" /></a>
                    <div class="tab_container" ng-controller="TabController">
                        <!-- Only required for left/right tabs -->
                        <ul class="nav nav-tabs">
                            <li ng-class="{active:isSet(1)}"><a href ng-click="setTab(1)"><strong>ABOUT</strong></a></li>
                            <li ng-class="{active:isSet(2)}"><a href ng-click="setTab(2)"><strong>ELIGIBILITY</strong></a></li>
                            <li ng-class="{active:isSet(3)}"><a href ng-click="setTab(3)"><strong>INCREMENT</strong></a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane" ng-class="{active:isSet(1)}">
                                <div class="col-xs-6 tab-data">
                                    <div class="col-xs-6">Track : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].Track || 'NA'}}</strong></div>
                                    <div class="col-xs-6">Function : </div>
                                    <div class="col-xs-6"><strong>{{ allInfo['more'].Function || 'NA'}}</strong></div>
                                    <div class="col-xs-6">SubTrack :</div>
                                    <div class="col-xs-6"> <strong>{{allInfo['more'].SubTrack || 'NA'}}</strong></div>
                                    <div class="col-xs-6">Basic Qualification : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].BasicQual}}</strong></div>
                                    <div class="col-xs-6">Basic Qualification Year : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].BasicQualYr}}</strong></div>
                                    <div class="col-xs-6">Highest Qualification : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].HighestQualification}}</strong></div>
									<div class="col-xs-6">Highest Qualification Year: </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].HighestQualYr}}</strong></div>
                                </div>
                                <div class="col-xs-6 tab-data">
                                    <div class="col-xs-6">Project : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].Project || 'NA'}}</strong></div>
                                    <div class="col-xs-6">DOB : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].DateOfBirth}}</strong></div>
                                    <div class="col-xs-6">Gender :</div>
                                    <div class="col-xs-6"> <strong>{{(allInfo.gender ==="m" || allInfo.gender === "M") ? 'Male' : 'Female'}}</strong></div>
                                    <div class="col-xs-6">ELTP : </div>
                                    <div class="col-xs-6"><strong>{{(allInfo['more'].IsELTP === 0 ? 'No' : 'Yes')}}</strong></div>
                                    <div class="col-xs-6">Company : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].CurrentCompany | uppercase}}</strong></div>
                                    <div class="col-xs-6">Joining Company : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].JoiningCompany | uppercase}}</strong></div>
                                    <div class="col-xs-6">Location :</div>
                                    <div class="col-xs-6"> <strong>{{allInfo.BaseLocation +" "+ allInfo['more'].CurrentFacility}}</strong></div>
                                    <div class="col-xs-6">Onsite / Offshore : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].OnsiteOrOffshore === 0 ? 'Offshore' : 'Onsite'}}</strong></div>

                                </div>
                            </div>
                            <div class="tab-pane" ng-class="{active:isSet(2)}">
                                <div class="col-xs-6 tab-data">
                                   <div class="row">
										<div class="col-xs-6">Stay at grade in months (Current/Required)  : </div>
										<div class="col-xs-6"><strong>{{allInfo['more'].MonthsInGrade}} / {{allInfo['more'].ReqStayInGrade}}</strong></div>
									</div>
									<div class="row">									
										<div class="col-xs-6">Total Credit points (Applicable/Required)  : </div>
										<div class="col-xs-6"><strong>{{allInfo['more'].TotalCreditPoints}} / {{allInfo['more'].ReqTotalCP}}</strong></div>
									</div>
									<div class="row">
										<div class="col-xs-6">Last Year Credit Points (Applicable/Required)  :</div>
										<div class="col-xs-6"><strong>{{allInfo['more'].LastFYCreditPoints}} / {{allInfo['more'].LastReqFYCreditPoints}}</strong></div>
									</div>
                                </div>
                                <div class="col-xs-6 tab-data">
                                   
                                </div>
                            </div>
                            <div class="tab-pane" ng-class="{active:isSet(3)}">
                                <div class="col-xs-6 tab-data">
                                    <div class="col-xs-6">Accelerite Grade Mapping : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].AccGradeMapping || 'NA'}}</strong></div>
                                    <div class="col-xs-6">Salary Revision? : </div>
                                    <div class="col-xs-6"><strong>{{(allInfo['more'].SalaryRevision === 0) ? 'NO' : 'YES'}}</strong></div>
                                    <div class="col-xs-6">Salary Fitment? :</div>
                                    <div class="col-xs-6"> <strong>{{(allInfo['more'].salaryFitment === 0) ? 'NO' : 'YES'}}</strong></div>
                                    <div class="col-xs-6">Salary Date : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].SalaryDate}}</strong></div>
                                    <div class="col-xs-6">Salary Reason : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].SalaryReason || 'NA'}}</strong></div>
                                    <div class="col-xs-6">Salary % : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].SalaryPercent}}%</strong></div>
                                    <div class="col-xs-6">Food and Insurance : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].FoodInsAmt}}</strong></div>
									<div class="col-xs-6">Annual Gross : </div>
                                    <div class="col-xs-6"><strong>{{allInfo.current_AG || 'NA'}}</strong></div>
                                    <div class="col-xs-6">CTC : </div>
                                    <div class="col-xs-6"><strong>{{allInfo.ctc_pa || 'NA'}}</strong></div>
                                    <div class="col-xs-6">PR : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].PerformanceReward}}</strong></div>
                                </div>
                                <div class="col-xs-6 tab-data">
                                    <div class="col-xs-6">Annual Cash Pay  : </div>
                                    <div class="col-xs-6"><strong>{{allInfo.current_AG || 'NA'}}</strong></div>
                                    <div class="col-xs-6">Std Salary Increment : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].StdSalIncr}}</strong></div>
                                    <div class="col-xs-6">BM Range :</div>
                                    <div class="col-xs-6"> <strong>{{allInfo['more'].BMRange || 'NA'}}</strong></div>
                                    <div class="col-xs-6">BM Adjustment Cap : </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].BMAdjustmentCAP}}</strong></div>
                                    <div class="col-xs-6">Increment : </div>
                                    <div class="col-xs-6"><strong>{{allInfo.increment}}</strong></div>
                                    <div class="col-xs-6">DH Input: </div>
                                    <div class="col-xs-6"><strong>{{allInfo['more'].DHInput}}</strong></div>
									<div class="col-xs-6">Revised annual gross : </div>
                                    <div class="col-xs-6"><strong>{{ allInfo.increment + allInfo.current_AG}}</strong></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </script>
            <!-- End popup modal template -->
			

        </div>
        <!-- End Container Fluid -->
    </div>

</body>
<script type="text/javascript" src="js/jquery-1.9.1.min.js"></script>
<script type="text/javascript" src="js/jquery.slimscroll.min.js"></script>
<script src="js/angular.min.js"></script>
<script src="js/bootstrap.min.js"></script>
<script src="js/ui-bootstrap-tpls-0.6.0.js" type="text/javascript"></script>
<script src="js/ng-table.js"></script>
<script src="js/jquery.stickytableheaders.js"></script>
<script src="js/ng-google-chart.js"></script>
<script src="js/spin.min.js"></script>
<script src="js/angular-spinner.js"></script>
<script src="app/Config.js"></script>
<script src="app/main.js"></script>
<script src="app/TabController.js"></script>
<script src="app/factory/getBUDUListFactory.js"></script>
<script src="app/factory/getMgrListFactory.js"></script>
<script src="app/factory/getEmpDataFactory.js"></script>
<script src="app/factory/getEmployeeDetails.js"></script>
<script src="app/factory/gradeDistributionFactory.js"></script>
<script src="app/directives/stickyHeaderDirective.js"></script>
<script>
    $(document).ready(function() {
        $("#startspin").trigger('click');
		 $('#scrollable-area').slimScroll({
            height: '500px',
            size: '6px',
            distance: '10px'
        }); 
		
	
		
		$(".more_details").hide();
		
        $('#collapseChart').collapse('hide');
		
    });
	$(window).load(function() {
      $("#stopspin").trigger('click');	
});
	//$('.panel-title a').click(accordionfix);
	<!-- $('#collapseChart').collapse('show', function(){
			//$('.tableFloatingHeaderOriginal')css('top', '305px');
		//}); -->
	<!-- function accordionfix(){
		 //$('#collapseChart').collapse('show');
		//$('.tableFloatingHeaderOriginal')css('top', '305px');
	//} -->
</script>

</html>
