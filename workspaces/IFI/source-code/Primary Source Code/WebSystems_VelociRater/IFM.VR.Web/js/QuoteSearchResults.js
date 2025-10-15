//frmMaster
var theForm = document.forms['frmMaster'];

if (!theForm) {
    theForm = document.form1;
}

//function __doPostBack(eventTarget, eventArgument) {
//    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
//        theForm.__EVENTTARGET.value = eventTarget;
//        theForm.__EVENTARGUMENT.value = eventArgument;
//        //alert(eventArgument);
//        theForm.submit();
//    }
//}

function Post(ele, parentId) {
    __doPostBack(parentId, $(ele).find("option:selected").text() + "_" + $(ele).val());
}

function ToggleSearchResultsViewResults(divId, hiddenId) {
    if ($("#" + hiddenId).val() != "1") {
        // was hidden
        $("#" + hiddenId).val("1");
    }
    else {
        // was shown
        $("#" + hiddenId).val("0");
    }
    ShowHideSearchResultsDiv(divId, hiddenId);
}

function ShowHideSearchResultsDiv(divId, hiddenId) {
    if ($("#" + hiddenId).val() == "0") {
        $("#" + divId).hide("fast");
    }
    else {
        $("#" + divId).show("fast");
    }
}