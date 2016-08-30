$(document).ready(function () {
    $('input:radio[name=rdBroadCastType]').click(function () {
        if ($(this).attr("id") == 'rdMessages') {
            $('#AnnouncementsContainer').hide();
            $('#SendMessagesContainer').show();
            $('#rdAnnouncements').removeAttr("checked");
            initializeMessage();
            $('#IsMessagesAlerts_Message').focus();
        }
        else {
            $('#SendMessagesContainer').hide();
            $('#AnnouncementsContainer').show();
            $('#rdMessages').removeAttr("checked");
            initializeAnnouncement();
           $('#Message').focus();
        }
    });

    $('input:radio[name=AllSuperUsers]').click(function () {
        $('#rdAllUsers').removeAttr("checked");
        $('#rdAllSuperUsers').val(true);
    });

    $('input:radio[name=AllUsers]').click(function () {
        $('#rdAllSuperUsers').removeAttr("checked");
        $('#rdAllUsers').val(true);
    });

    
    // Validations for 'Broadcast Announcements' tab
    $("#Announcements").validate({
        rules: {
            Message: "required",
            TimeHourMinutes: {
                required: function (element) {
                    var time = $('#TimeHourMinutes').val();
                    var hour = time.substring(0, 2);
                    var min = time.substring(3);
                    var colon = time.substring(2, 3);

                    if (time == '' || time.length != 5 || colon != ':' || isNaN(hour) || isNaN(min)) {
                        $('#TimeHourMinutes').val('');                        
                        return true;
                    }

                    var splittime = $('#TimeHourMinutes').val().split(":");

                    if (splittime[0] > 23 || splittime[1] > 59) {                        
                        $('#TimeHourMinutes').val('');                        
                        return true;
                    }

                    else {                        
                        return false;
                    }
                }
            }
        },
        messages: {
            Message: "Detail Required",
            TimeHourMinutes: "Valid Time Required in Format of HH:mm."
        }
    });

    // Validations for 'Send Messages' tab
    $("#SendMessages").validate({
        rules: {
            "IsMessagesAlerts.Message": "required",
            MemberCategory: "required"
        },
        messages: {
            "IsMessagesAlerts.Message": "Detail Required",
            MemberCategory: "From Date And Time Required"
        }
    });
});

function initializeAnnouncement() {
    $('#rdAnnouncements').attr('checked', 'checked');   
    $('#rdMessages').removeAttr('checked');
    $('#SendMessagesContainer').hide();
 //   $('#Message').val('');
//    var today = new Date();
//    $('#StartDateTimeValue').val(today.toString("dd-mmm-yy'T'HH:MM:ss"));
//    $('#StartDateTimeValue').watermark();
    $('#messageExpiryDate').val($('#DefaultExpiryDate').val());
}

function initializeMessage() {
    $('#rdAllSuperUsers').attr('checked', 'checked');
    $('#rdAllSuperUsers').val(true);
    $('#rdAllUsers').prop('checked', false); 
    $('#IsMessagesAlerts_Message').val('');
    $('#MemberCategory').val('');
}

function initMessage() {
    $('#AnnouncementsContainer').hide();
    $('#SendMessagesContainer').show();
    $('#rdAnnouncements').removeAttr("checked");
    $('#rdMessages').attr('checked', 'checked');
    $('#rdAllSuperUsers').attr('checked', 'checked');
    $('#rdAllSuperUsers').val(true);
    $('#rdAllUsers').prop('checked', false);
    $('#IsMessagesAlerts_Message').val('');
    $('#MemberCategory').val('');
}

