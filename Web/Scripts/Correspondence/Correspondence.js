//SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
//JS Function added over closeCorrespondence() in order to ensure that BM doesn't exist on correspondence.
function CheckIfBMExistsThenCloseCorrespondence(_correspondenceRefNumber, _correspondenceId, _isAuthorityToBill, _isBMExistsAction, _scenarioId) {
    if (_isAuthorityToBill == 'True') {
        $.ajax({
            type: "POST",
            url: _isBMExistsAction,
            data: { correspondenceRefNumber: _correspondenceRefNumber },
            dataType: "json",
            success: function (response) {
                if (response.IsFailed == false) {
                    closeCorrespondence(_correspondenceId, _scenarioId);
                }
                else {
                    alert(response.Message);
                }
            }
        });
    }
    else {
        //Authority to bill does,t exist so no need to check if BM exist on this correspondence.
        closeCorrespondence(_correspondenceId, _scenarioId);
    }
}

function closeCorrespondence(_correspondenceId, _scenarioId) {
    /*
    SCENARIO 1: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = SAVED
    SCENARIO 2: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    SCENARIO 3: STAGE = 1,   STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED
    SCENARIO 4: STAGE = >=2, STATUS = OPEN     AND  SUB-STATUS = RECEVIED/RESPONDED
    SCENARIO 5: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = SAVED
    SCENARIO 6: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    SCENARIO 7: STAGE = >2,  STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED 
    */
    //Bug-8863: CMP527-Warning message to be corrected while closing correspondence
    var msg = '';
    if (_scenarioId == 1 || _scenarioId == 2) // if stage is 1 
    {
        msg = "Are you sure if you want to close this correspondence?";
    }
    else {
        msg = "Are you sure if you want to close this correspondence? You will not be able to reopen it.";
    }
    if (confirm(msg)) {
        //SCP281499 - CMP 527, Closure of Correspondence
        //For Scenario 3, we make changes to include scenario 3 for acceptance comments not delete directly.
        if ( _scenarioId == 3 || _scenarioId == 4 || _scenarioId == 5 || _scenarioId == 6 || _scenarioId == 7) {
            //Acceptance comment needed.
            $('#CloseCorrespondence').dialog({ closeOnEscape: false, title: 'Acceptance Comments', height: 350, width: 800, modal: true, resizable: false });
        }
        else {
            $("#frmCloseCorrespondence").submit();
         }
    }
    return false;

}

function validationTextArea() {
    var acceptanceComment = $.trim($("#userAcceptanceComment").val()); 
    if ( acceptanceComment != '') {
        if (acceptanceComment.length > 2000) {
            alert("Acceptance Comments cannot be greater then 2000 chars.");
            return false;
        }
    }
    else {
        alert("Acceptance Comments are required.");
        return false;
    }
}

