/* CMP #675: Progress Status Bar for Processing of Billing Data Files. 
* Desc: JS function to apply apt CSS stiles for Progress Bar components. */
function renderProgressBar(Process_Name, Process_State, Queue_Position) {

    /* Default Settings Begin */

    /* By default mark every process as pending */
    $('#sanityStatus').text('Pending');
    $('#validationStatus').text('Pending');
    $('#loadingStatus').text('Pending');
    $('#finalUpdateStatus').text('Pending');

    /* By default apply Pending syle CSS to every process */
    $('.progress .circle').removeClass().addClass('circle');
    $('.progress .bar').removeClass().addClass('bar');
    $('.progress .circle:nth-of-type(' + (1) + ')').removeClass('done').addClass('pending');
    $('.progress .circle:nth-of-type(' + (1) + ') .label').html('&#9746;');
    $('.progress .circle:nth-of-type(' + (2) + ')').removeClass('done').addClass('pending');
    $('.progress .circle:nth-of-type(' + (2) + ') .label').html('&#9746;');
    $('.progress .circle:nth-of-type(' + (3) + ')').removeClass('done').addClass('pending');
    $('.progress .circle:nth-of-type(' + (3) + ') .label').html('&#9746;');
    $('.progress .circle:nth-of-type(' + (4) + ')').removeClass('done').addClass('pending');
    $('.progress .circle:nth-of-type(' + (4) + ') .label').html('&#9746;');

    /* Clear of any text for Queue Status */
    $('#sanityQStatus').text('');
    $('#validationQStatus').text('');
    $('#loadingQStatus').text('');
    $('#finalUpdateQStatus').text('');

    /* Apply default text as --- as a queue position */
    $('#sanityQStatus').text('---');
    $('#validationQStatus').text('---');
    $('#loadingQStatus').text('---');
    $('#finalUpdateQStatus').text('---');

    /* Default Settings End */

    /* Switch on basis of process */
    switch (Process_Name) {
        case 'SANITY CHECK':

            /* SANITY CHECK Process Settings Begin */

            /* Switch on basis of state */
            switch (Process_State) {

                case 'PENDING':

                    /* Virtually imposible state - Written only for better code complition and readability */
                    /* Underlying SP (PROC_GET_FILE_PROGRESS_DETAILS) never returns this status - Virtually dead code */
                    $('#sanityStatus').text('Pending');
                    $('#sanityQStatus').text('');
                    $('#sanityQStatus').text('Position in Queue: ' + Queue_Position);
                    $('.progress .circle:nth-of-type(' + (1) + ')').removeClass('pending').addClass('pendingQ');
                    $('.progress .circle:nth-of-type(' + (1) + ') .label').html('&#9746;');
                    break;

                case 'IN PROGRESS':

                    /* This is Sanity Check - In Progress Block */
                    $('#sanityStatus').text('In Progress');
                    $('.progress .circle:nth-of-type(' + (1) + ')').removeClass('pending').addClass('active');
                    $('.progress .circle:nth-of-type(' + (1) + ') .label').html('&#10144;');
                    break;

                case 'COMPLETED':
                    
                    /* Virtually imposible state - Written only for better code complition and readability */
                    /* Underlying SP (PROC_GET_FILE_PROGRESS_DETAILS) never returns this status - Virtually dead code */
                    markSanityCheckProcessAsCompleted();                    
                    break;
            }

            /* SANITY CHECK Process Settings End */
            break;

        case 'VALIDATION':

            /* VALIDATION Process Settings Begin */

            /* Switch on basis of state */
            switch (Process_State) {

                case 'PENDING':

                    /* This is Validation - Pending Block */
                    $('#validationStatus').text('Pending');
                    $('#validationQStatus').text('');
                    $('#validationQStatus').text('Position in Queue: ' + Queue_Position);
                    $('.progress .circle:nth-of-type(' + (2) + ')').removeClass('pending').addClass('pendingQ');
                    $('.progress .circle:nth-of-type(' + (2) + ') .label').html('&#9746;');
                    break;

                case 'IN PROGRESS':

                    /* This is Validation - In Progress Block */
                    $('#validationStatus').text('In Progress');
                    $('.progress .circle:nth-of-type(' + (2) + ')').removeClass('pending').addClass('active');
                    $('.progress .circle:nth-of-type(' + (2) + ') .label').html('&#10144;');
                    break;

                case 'COMPLETED':

                    /* Virtually imposible state - Written only for better code complition and readability */
                    /* Underlying SP (PROC_GET_FILE_PROGRESS_DETAILS) never returns this status - Virtually dead code */
                    markValidationProcessAsCompleted();
                    break;
            }

            /* By default mark previous processes as completed */
            markSanityCheckProcessAsCompleted();

            /* VALIDATION Process Settings End */
            break;

        case 'LOADING':

            /* LOADING Process Settings Begin */

            /* Switch on basis of state */
            switch (Process_State) {

                case 'PENDING':

                    /* This is Loading - Pending Block */
                    $('#loadingStatus').text('Pending');
                    $('#loadingQStatus').text('');
                    $('#loadingQStatus').text('Position in Queue: ' + Queue_Position);
                    $('.progress .circle:nth-of-type(' + (3) + ')').removeClass('pending').addClass('pendingQ');
                    $('.progress .circle:nth-of-type(' + (3) + ') .label').html('&#9746;');
                    break;

                case 'IN PROGRESS':

                    /* This is Loading - In Progress Block */
                    $('#loadingStatus').text('In Progress');
                    $('.progress .circle:nth-of-type(' + (3) + ')').removeClass('pending').addClass('active');
                    $('.progress .circle:nth-of-type(' + (3) + ') .label').html('&#10144;');
                    break;

                case 'COMPLETED':
                    
                    /* Virtually imposible state - Written only for better code complition and readability */
                    /* Underlying SP (PROC_GET_FILE_PROGRESS_DETAILS) never returns this status - Virtually dead code */
                    markLoadingProcessAsCompleted();
                    break;
            }

            /* By default mark previous processes as completed */
            markSanityCheckProcessAsCompleted();
            markValidationProcessAsCompleted();

            /* LOADING Process Settings End */
            break;

        case 'FINAL UPDATES':

            /* FINAL UPDATES Process Settings Begin */

            /* Switch on basis of state */
            switch (Process_State) {

                case 'PENDING':

                    /* Virtually imposible state - Written only for better code complition and readability */
                    /* Underlying SP (PROC_GET_FILE_PROGRESS_DETAILS) never returns this status - Virtually dead code */
                    $('#finalUpdateStatus').text('Pending');
                    $('#finalUpdateQStatus').text('');
                    $('#finalUpdateQStatus').text('Position in Queue: ' + Queue_Position);
                    $('.progress .circle:nth-of-type(' + (4) + ')').removeClass('pending').addClass('pendingQ');
                    $('.progress .circle:nth-of-type(' + (4) + ') .label').html('&#9746;');
                    break;

                case 'IN PROGRESS':

                    /* This is Final Update - In Progress Block */
                    $('#finalUpdateStatus').text('In Progress');
                    $('.progress .circle:nth-of-type(' + (4) + ')').removeClass('pending').addClass('active');
                    $('.progress .circle:nth-of-type(' + (4) + ') .label').html('&#10144;');
                    break;

                case 'COMPLETED':
                    /* Virtually imposible state - Written only for better code complition and readability */
                    /* Underlying SP (PROC_GET_FILE_PROGRESS_DETAILS) never returns this status - Virtually dead code */
                    markFinalUpdateProcessAsCompleted();
                    break;
            }

            /* By default mark previous processes as completed */
            markSanityCheckProcessAsCompleted();
            markValidationProcessAsCompleted();
            markLoadingProcessAsCompleted();

            /* FINAL UPDATES Process Settings End */
            break;
    }

    /* Apply apt CSS class for Queue Position Text */
    resetQueuePositionTextColor(Queue_Position);

}

/* Function to mark Sanity Check Process as Complete */
function markSanityCheckProcessAsCompleted() {

    $('#sanityStatus').text('Completed');
    $('.progress .circle:nth-of-type(' + (1) + ')').removeClass('pending').addClass('done');
    $('.progress .circle:nth-of-type(' + (1) + ') .label').html('&#10004;');

}

/* Function to mark Validation Process as Complete */
function markValidationProcessAsCompleted() {

    $('#validationStatus').text('Completed');
    $('.progress .circle:nth-of-type(' + (2) + ')').removeClass('pending').addClass('done');
    $('.progress .circle:nth-of-type(' + (2) + ') .label').html('&#10004;');

}

/* Function to mark Loading Process as Complete */
function markLoadingProcessAsCompleted() {

    $('#loadingStatus').text('Completed');
    $('.progress .circle:nth-of-type(' + (3) + ')').removeClass('pending').addClass('done');
    $('.progress .circle:nth-of-type(' + (3) + ') .label').html('&#10004;');

}

/* Function to mark Final Update Process as Complete */
function markFinalUpdateProcessAsCompleted() {

    $('#finalUpdateStatus').text('Completed');
    $('.progress .circle:nth-of-type(' + (4) + ')').removeClass('pending').addClass('done');
    $('.progress .circle:nth-of-type(' + (4) + ') .label').html('&#10004;');

}

/* Function to Apply apt CSS class for Queue Position Text */
function resetQueuePositionTextColor(Queue_Position) {

    /* '---' is a default text in XXXXXQStatus Span Control. 
    In case of default text -> Apply queuePositionWhite CSS to apperently make this as invisible.
    In case of anything but default text -> Apply queuePositionBlack CSS to make this text visible. 
    */

    if ($('#sanityQStatus').text() == '---') {
        $('#sanityQStatus').removeClass().addClass('queuePositionWhite');
    }
    else {
        $('#sanityQStatus').removeClass().addClass('queuePositionBlack');
    }

    if ($('#validationQStatus').text() == '---') {
        $('#validationQStatus').removeClass().addClass('queuePositionWhite');
    }
    else {
        $('#validationQStatus').removeClass().addClass('queuePositionBlack');
    }
        
    if ($('#loadingQStatus').text() == '---') {        
        $('#loadingQStatus').removeClass().addClass('queuePositionWhite');
    }
    else {
        $('#loadingQStatus').removeClass().addClass('queuePositionBlack');
    }

    if ($('#finalUpdateQStatus').text() == '---') {
        $('#finalUpdateQStatus').removeClass().addClass('queuePositionWhite');
    }
    else {
        $('#finalUpdateQStatus').removeClass().addClass('queuePositionBlack');
    }

    /* In case of Process Status In Progress and/or Completed, Queue position is irrelevant, 
       so make all Queue status spans invisible by applying queuePositionWhite CSS */
    if (Queue_Position == '-1') {

        $('#sanityQStatus').removeClass().addClass('queuePositionWhite');
        $('#validationQStatus').removeClass().addClass('queuePositionWhite');
        $('#loadingQStatus').removeClass().addClass('queuePositionWhite');
        $('#finalUpdateQStatus').removeClass().addClass('queuePositionWhite');
    
    }    
}