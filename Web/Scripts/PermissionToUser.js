/// <reference path="jquery-1.5.1.min.js" />

var SelectedCheckedValues = '';
var IsPermissionSelected = false;
var GetTemplatePermissionListMethod = '/Permission/GetTemplatePermissionList';
var GetUserCategoryIdByUserNameMethod = '/Permission/GetUserCategoryIdByUserName';
var GetTemplateNameByUserCategoryId = '/Permission/GetTemplateNameByUserCategoryId';
var GetAssignedPermissionListByUserIdMethod = '/Permission/GetAssignedUserPermissionList';
var GetPermissionListToUser = '';
var TreeViewcontrolId = '';

function SetPostUrl(postUrl) {
  GetPermissionListToUser = postUrl;
}

function SetTemplatePermissionListUrl(postUrl) {
  GetTemplatePermissionListMethod = postUrl;
}

function SetUserCategoryIdByUserNameUrl(postUrl) {
  GetUserCategoryIdByUserNameMethod = postUrl;
}

function SetTemplateNameByUserCategoryIdUrl(postUrl) {
  GetTemplateNameByUserCategoryId = postUrl;
}

function SetAssignedPermissionListByUserIdUrl(postUrl) {
  GetAssignedPermissionListByUserIdMethod = postUrl;
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
      TemplateName: "required"
    },
    messages: {
      TemplateName: "Template Name required"
    }
  });

  var AssignedUserID = $("#UserId").val();
  var UserCategoryId = $("#UserCategoryId").val();
  if (AssignedUserID != '') {
    if (UserCategoryId != '') {
      GetTemplate(UserCategoryId);
      SetTreeviewControlId();
      GetTreeviewHierarchy(UserCategoryId, AssignedUserID);
     
    }

  }

});


function generateHiddenFieldsForTree(treeId) {
  if ($("#" + treeId).length === 0) {
    alert("There was an error in the permission assign process. Please try again, or contact your administrator.");
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

    if (($('#UserId').val() == null || $('#UserId').val() == '')) {
        alert("Please select username");
    }
    else if (($('#TemplateId').val() == null || $('#TemplateId').val() == '0')) {
        alert("Please select Template");
    }
    else {
        generateHiddenFieldsForTree(TreeViewcontrolId);
        GetTemplatePermissionList($("#SelectedIDs").val(), $("#TemplateId").val());
    }


});

$("#btnReplacePermissionList").bind('click', function () {
    if (($('#UserId').val() == null || $('#UserId').val() == '')) {
        alert("Please select username");
    }
    else if (($('#TemplateId').val() == null || $('#TemplateId').val() == '0')) {
        alert("Please select Template");
    }
    else {
        unCheckNode();
        GetTemplatePermissionList($("#SelectedIDs").val(), $("#TemplateId").val());
    }

});


$("#btnCopyUserPermission").bind('click', function () {
    if (($('#UserId').val() == null || $('#UserId').val() == '')) {
        alert("Please select Username");
    }
    else if (($('#CopyUserName').val() == null || $('#CopyUserName').val() == '')) {
        alert("Please select Copy User Name");
    }
    else {
        
        GetAssignedPermissionListByUserId($("#CopyUserId").val());
    }

});


$("#btnSave").bind('click', function () {
  $("#SelectedIDs").val('');
  generateHiddenFieldsForTree(TreeViewcontrolId);
  if (!IsPermissionSelected) {
    showClientErrorMessage('Please Select Permission');
    return false;
  }

  $("#frmTree").submit();

});


function GetAssignedPermissionListByUserId(userId) {
  if (userId != "") {
    var AssignedUserID = $("#UserId").val();

    if (AssignedUserID == '') {
      showClientErrorMessage('Please Enter Assigned User Name');
      return false;
    }

    $.ajax({
      type: "POST",
      url: GetAssignedPermissionListByUserIdMethod,
      data: { userId: userId },
      dataType: "json",
      success: function (response) {
        if (response.Message) {
          showClientErrorMessage(response.Message);
        }
        else {
          clearMessageContainer();

          var strElement = $("#" + TreeViewcontrolId)[0].innerHTML;

          for (var i = 0; i < $('#' + TreeViewcontrolId +' li a').length; i++) {
            AssignedCopiedPermission($('#' + TreeViewcontrolId + ' li a')[i], response);

          }

        }
      },
      failure: function (response) {

      }
    });


  }




}

function UserName_AutoCompleteValueChange(selecteduserId) {

  if (selecteduserId != "") {
    $.ajax({
      type: "POST",
      url: GetUserCategoryIdByUserNameMethod,
      data: { userId: selecteduserId },
      dataType: "json",
      success: function (response) {
        if (response.Message) {
          showClientErrorMessage(response.Message);
        }
        else {
          clearMessageContainer();
          GetTemplate(response.UserCategoryId);
          SetTreeviewControlId();
          GetTreeviewHierarchy(response.UserCategoryId, selecteduserId);
          $("#UserName").val(response.UserName);
          $("#UserCategoryId").val(response.UserCategoryId);
          $("#CopyUserName").val('');
          $("#btnSave").show();
        }
      },
      failure: function (response) {

      }
    });

    }

}


 

function GetTemplate(UserCategoryId) {

  $.ajax({
    type: "POST",
    url: GetTemplateNameByUserCategoryId,
    data: { userCategoryId: UserCategoryId },
    dataType: "json",
    success: function (data) {
      if (data.Message) {
        showClientErrorMessage(data.Message);
      }
      else {
        //clearMessageContainer();

        if (data) {
          var items = "";
          var firstItem = "<option value='0'>Select</option>";
          $.each(data, function (i, data) {
            var TemplateID = data["Id"];
            items += "<option value='" + TemplateID + "'>" + data["TemplateName"] + "</option>";
          });

          if (items != "") {
            var TemplateData = $('#TemplateId').val();
            items = firstItem + items;
            $("#TemplateId").html(items);
            $("#TemplateId").removeAttr('disabled');
            if (TemplateData) { $('#TemplateId').val(TemplateData); }

          }
          else {
            $("#TemplateId").html(firstItem);
          }


        }

      }
    },
    failure: function (data) {

    }
  });

  return true;
}

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

         

          for (var i = 0; i < $('#' + TreeViewcontrolId + ' li a').length; i++) {
            CheckLeafNode($('#' + TreeViewcontrolId + ' li a')[i], response);

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

function CheckLeafNode(obj, data) {
  var nodeText = '';
  
  if (document.all) {
    nodeText = obj.innerText;
  } else {
    nodeText = obj.textContent;
  }

  for (var rowCount = 0; rowCount < data.length; rowCount++) {
    if ($.trim(nodeText) == $.trim(data[rowCount].PermissionName)) {

      if (data[rowCount].Is_Permission_Assigned) {
        $.tree.plugins.checkbox.check(obj);

      }
    }

  }

  return true;
}

function AssignedCopiedPermission(obj, data) {
  var nodeText = '';
  if (document.all) {
    nodeText = obj.innerText;
  } else {
    nodeText = obj.textContent;
  }

  for (var rowCount = 0; rowCount < data.length; rowCount++) {
    if ($.trim(nodeText) == $.trim(data[rowCount].PermissionName)) {
      $.tree.plugins.checkbox.check(obj);

    }

  }

  return true;

}


function unCheckNode() {

  for (var i = 0; i < $('#' + TreeViewcontrolId + ' li a').length; i++) {
    $.tree.plugins.checkbox.uncheck($('#' + TreeViewcontrolId + ' li a')[i]);
    return true;
  }


}




function GetTreeviewHierarchy(UserCategoryID, UserId) {
  $("#" + TreeViewcontrolId).tree({
    ui: {
      theme_name: "checkbox"
    },
    data: {
      type: "json",
      opts: {
        method: "POST",
        url: GetPermissionListToUser + "=" + UserCategoryID + "&UserId=" + UserId

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


function GetTreeviewHierarchyForOwnPermission(UserCategoryID, UserId) {
  $("#" + TreeViewcontrolId).tree({
    ui: {
      theme_name: "checkbox"
    },
    data: {
      type: "json",
      opts: {
        method: "POST",
        url: GetPermissionListToUser + "=" + UserCategoryID + "&UserId=" + UserId

      }
    },
        plugins: {
          checkbox: {}
        },
    callback: {
      onload: function (tree) {
        $('li[selected=true]').each(function () {
          $.tree.plugins.checkbox.check(this);
          $(this).attr('readonly', true);
        });
      }
    }
  });

  return true;
}