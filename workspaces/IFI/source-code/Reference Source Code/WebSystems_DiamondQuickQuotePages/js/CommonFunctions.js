function SubmitConfirm(control, confirmMessage, submitText) {
    if ((typeof (confirmMessage) == "undefined") || (confirmMessage == (null || ""))) {
        confirmMessage = 'Are you sure you want to submit?';
    }
    if ((typeof (submitText) == "undefined") || (submitText == (null || ""))) {
        submitText = 'Submitting...';
    }
    var name = confirm(confirmMessage)
    if (name) {
        btnSubmit_Click(control, submitText);
        return true;
    }
    else {
        document.getElementById(control.id).disabled = false;
        return false;
    }
}

function SubmitConfirm_ClientValidate(control, confirmMessage, submitText) {
    var ok = IsPageOkay();
    if (ok == true) {
        if ((typeof (confirmMessage) == "undefined") || (confirmMessage == (null || ""))) {
            confirmMessage = 'Are you sure you want to submit?';
        }
        if ((typeof (submitText) == "undefined") || (submitText == (null || ""))) {
            submitText = 'Submitting...';
        }
        var name = confirm(confirmMessage)
        if (name) {
            btnSubmit_Click(control, submitText);
            return true;
        }
        else {
            document.getElementById(control.id).disabled = false;
            return false;
        }
    }
}

function DisableControl(controlId, submitText) {
    document.getElementById(controlId).disabled = true;
    document.getElementById(controlId).value = submitText;
}

function DisableControl_SetTimeout(controlId, submitText, interval) {
    setTimeout("DisableControl('" + controlId + "', '" + submitText + "')", interval);
}

function btnSubmit_Click(control, submitText) {
    if ((typeof (submitText) == "undefined") || (submitText == (null || ""))) {
        submitText = 'Submitting...';
    }
    DisableControl_SetTimeout(control.id, submitText, 100);
}

function btnSubmit_Click_ClientValidate(control, submitText) {
    var ok = IsPageOkay();
    if (ok == true) {
        if ((typeof (submitText) == "undefined") || (submitText == (null || ""))) {
            submitText = 'Submitting...';
        }
        DisableControl_SetTimeout(control.id, submitText, 100);
    }
}

function ImgBtnSubmit_Click_ClientValidate(control, submitImgSrc, valGrp) {
    var ok = IsPageOkay(valGrp);
    if (ok == true) {
//        if ((typeof (submitImgSrc) == "undefined") || (submitImgSrc == (null || ""))) {
//            submitImgSrc = '';
//        }
        ImgBtnDisableControl_SetTimeout(control.id, submitImgSrc, 100);
    }
}
function ImgBtnDisableControl(controlId, imgSrc) {
    document.getElementById(controlId).disabled = true;
    document.getElementById(controlId).src = imgSrc;
}

function ImgBtnDisableControl_SetTimeout(controlId, imgSrc, interval) {
    setTimeout("ImgBtnDisableControl('" + controlId + "', '" + imgSrc + "')", interval);
}

function IsPageOkay(valGrp) {

    if ((typeof (valGrp) == "undefined") || (valGrp == (null || ""))) {
        Page_ClientValidate();
    } else {
        Page_ClientValidate(valGrp);
    }
    
    if ((typeof (Page_IsValid) != "undefined") && (Page_IsValid == false)) {
        return false;
    }
    else {
        return true;
    }
}

function stopback() {
    history.forward();
}