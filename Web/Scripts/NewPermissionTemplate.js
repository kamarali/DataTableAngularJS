/// <reference path="jquery-1.5.1.min.js" />
var SelectedCheckedValues = '';
var IsPermissionSelected = false;
var GetTemplatePermissionListMethod = '/Permission/GetTemplatePermissionList';
var GetPermissionListForTreeView = '';
var TreeViewcontrolId = '';
var NonSISOpsUserCategoryId = '';

function SetPostUrl(postUrl) {
  GetPermissionListForTreeView = postUrl;
}

function SetUserCategory(UserCategory) {
  NonSISOpsUserCategoryId = UserCategory;

}

function SetTemplatePermissionListUrl(postUrl) {
  GetTemplatePermissionListMethod = postUrl;
}

function SetTreeviewControlId() {
  $("#TreeChoosePermission").html('');
  var TimeStamp = new Date().getTime();

  TreeViewcontrolId = "TreeviewDiv" + TimeStamp;

  var TreeDivControl = '<div id=' + TreeViewcontrolId + ' > </div>';

  $("#TreeChoosePermission").append(TreeDivControl);
  

  return true;
}


$(function () {



    $("#frmTree").validate({
        rules: {
            TemplateName: "required",
            UserCategoryId: "required"
        },
        messages: {
            TemplateName: "Template Name required",
            UserCategoryId: "select User Category"

        }
    });

    $("#lblselectLabel").hide();
    var UserCategoryId =  $("#UserCategoryId").val();
    if ($.trim(UserCategoryId) != '') {
        GetTreeviewHierarchy(UserCategoryId);
    } else {
        if ($.trim(NonSISOpsUserCategoryId) != '') {
            GetTreeviewHierarchy(NonSISOpsUserCategoryId);
        }

        }




//    $('#UserCategoryId').change(function () {
//        var selectedItem = $("#UserCategoryId").val();
//        if ($.trim(selectedItem) != '') {
//            GetTreeviewHierarchy(selectedItem);
//        }
//    });

});


    function GetTreeviewHierarchy(UserCategoryID) {
      SetTreeviewControlId();

      $("#lblselectLabel").show();
      $("#" + TreeViewcontrolId).tree({
          ui: {
              theme_name: "checkbox"
          },
          data: {
              type: "json",
              opts: {
                  method: "POST",
                  url: GetPermissionListForTreeView + "=" + UserCategoryID

              }
          },
          plugins: {
              checkbox: {}
          },
          callback: {
              onload: function (tree) {
               
                  $('li[selected=true]').each(function () {
                    
                      $.tree.plugins.checkbox.check(this);
                  });
              }
          }
      });

    return true;
  }


  


function GetButtonClick() {

  generateHiddenFieldsForTree(TreeViewcontrolId);
    if (!IsPermissionSelected) {
        alert('Please Select Permission');
        return false;
    }
    $("#frmTree").submit();

}


function generateHiddenFieldsForTree(treeId) {
    if ($("#" + treeId).length === 0) {
        alert("invalid treeId for generateHiddenFieldsForTree");
        return;
    }

    $.tree.plugins.checkbox.get_checked($.tree.reference("#" + treeId)).each(function () {
        var checkedId = this.id;
        $("<input>").attr("type", "hidden").attr("name", checkedId).val("on").appendTo("#" + treeId);
        SelectedCheckedValues = SelectedCheckedValues + ',' + checkedId;
        $("#SelectedIDs").val(SelectedCheckedValues);
        IsPermissionSelected = true;
    });

    
}


$("#btnAddToPermissionList").bind('click', function () {
  generateHiddenFieldsForTree(TreeViewcontrolId);
    GetTemplatePermissionList($("#SelectedIDs").val(), $("#TemplateId").val());
   

});

function GetTemplatePermissionList(SelectedPermissionIds, TemplateID) {

    if (TemplateID != "") {

        $.ajax({
            type: "POST",
            url: GetTemplatePermissionListMethod,
            data: { permissionSelectedIds: SelectedPermissionIds, templateId: TemplateID },
            dataType: "json",
            success: function (response) {
                if (response.Message) {
                    showClientErrorMessage(response.Message);
                }
                else {
                    clearMessageContainer();

                    var strElement = $("#" + TreeViewcontrolId)[0].innerHTML;

                

                    for (var i = 0; i < $('#' + TreeViewcontrolId + 'li a').length; i++) {
                        CheckLeafNode($('#' + TreeViewcontrolId + 'li a')[i], response);

                    }

                }
            },
            failure: function (response) {

            }
        });

       
    }
    else {
         
    }


}

function CheckLeafNode(obj,data) {

    $.tree.plugins.checkbox.check(obj);

    $.each(data, function (i, item) {
        alert(item.data);
    });



    return null;
}




 