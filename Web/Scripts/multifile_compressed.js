// Multiple file selector by Stickman -- http://www.the-stickman.com 
// with thanks to: [for Safari fixes] Luis Torrefranca -- http://www.law.pitt.edu and Shawn Parker & John Pennypacker -- http://www.fuzzycoconut.com [for duplicate name bug] 'neal'
function MultiSelector(list_target, max) {
    this.list_target = list_target; this.count = 0; this.id = 0; if (max) { this.max = max; } else { this.max = -1; };this.addElement = function (element) {
        if (element.tagName == 'INPUT' && element.type == 'file') {
            element.name = 'file_' + this.id++; element.multi_selector = this; element.onchange = function () {
                //var new_element = document.createElement('input'); new_element.type = 'file'; this.parentNode.insertBefore(new_element, this); this.multi_selector.addElement(new_element); this.multi_selector.addListRow(this); this.style.position = 'absolute'; this.style.left = '-1000px';
                if (element.id !== "memberLogoUpload") { //OnChangeMemberLogo(); else
                    if (!TestFileType(this)) return false;
                }
                else {
                    if (IsFileImageType(this)) {
                        return false;
                    }
                }
            };
            if (this.max != -1 && this.count >= this.max) { element.disabled = true; }; this.count++; fileIps = this.count; this.current_element = element;
        } else { alert('Error: not a file input element'); };
    };      this.addListRow = function (element) {
        var new_row = document.createElement('div'); var new_row_button = currentIPFileE = document.createElement('input'); new_row_button.type = 'button'; new_row_button.value = 'Delete'; new_row.element = element; new_row_button.onclick = function () {
            this.parentNode.element.parentNode.removeChild(this.parentNode.element); this.parentNode.parentNode.removeChild(this.parentNode); this.parentNode.element.multi_selector.count--; fileIps = this.parentNode.element.multi_selector.count; this.parentNode.element.multi_selector.current_element.disabled = false; return false;
        }; new_row.innerHTML = element.value; new_row.appendChild(new_row_button); this.list_target.appendChild(new_row);
    };
};
function TestFileType(curElement) {
    var isFileExists = CheckIfFileNameExists(curElement); var isValidExtn = false; $isToUpload = true; 
    if (!isFileExists) {
        var i, fileName; if (!curElement || curElement.value.length == 0) return false; fileName = curElement.value;
        isValidExtn = IsFileExtentionValid(fileName);
        if (!isValidExtn) {
            alert("Please only upload files that end in types: " + (fileExtTypes) + "\n\nPlease select a new file and try again.");
            ResetFileIpElement();
        }
        return isValidExtn;
    } else { ResetFileIpElement(); alert("File already exists. \n\n Please select a new file with different name."); }
}
function ResetFileIpElement() {
    $("#lblMultiFileUpload").empty().html('File to Upload:<input id="file_element" type="file" name="file" />'); multi_selector = new MultiSelector(document.getElementById('files_list'), 1); multi_selector.addElement(document.getElementById('file_element'));
}
function CheckIfFileNameExists(curElement) {
    var i, fileName; if (!curElement || curElement.value.length == 0) return false; fileName = curElement.value;
    var isToUpload = IsDuplicateFile(fileName);
    return isToUpload;
}

function IsDuplicateFile(fileName) {
    var dots = fileName.split("\\");
    fileName = dots[dots.length - 1];
    if ($.Attachment != null) {
        for (i = 0; i < $.Attachment.FileNames.length; i++) {
            if ($.Attachment.FileNames[i].DisplayName.toLowerCase() === fileName.toLowerCase()) {
                $isToUpload = false; return true;
            }
        }
    }
    $isToUpload = true;
    return false;
}
function IsFileExtentionValid(fileName) {
   var isValidExtn = false;
   var dots = fileName.split("."); fileType = dots[dots.length - 1].toLowerCase(); if (fileType.length > 1 && fileExtTypes.indexOf(fileType) != -1) isValidExtn = true;
    else {
        $isToUpload = false; return false;
    } return isValidExtn;

}

function IsFileImageType(curElement) {
    var imgExtTypes = "|jpg|png|gif|";
    var i, fileName; if (!curElement || curElement.value.length == 0) return false; fileName = curElement.value; var dots = fileName.split("."); fileType = dots[dots.length - 1].toLowerCase();if (fileType.length > 1 && imgExtTypes.indexOf(fileType) != -1) isValidExtn = true;
    else {
        $("#containerMemberLogo").html($("#containerMemberLogo").html()); multi_selector = new MultiSelector(document.getElementById('files_list'), 1);
        multi_selector.addElement(document.getElementById('memberLogoUpload'));
        alert("Please only upload files that end in types: " + (imgExtTypes) + "\n\nPlease select a new file and try again."); return false;
    }
}
