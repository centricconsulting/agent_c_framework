
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />


// Calculate coverages From Dwelling Limit
function CalculateFromDFRLimit(txtDwLimit, txtDwellingTotalLimit, txtRPSLimit, txtRPSChgInLimit, txtRPSTotalLimit,
    txtPPLimit, txtPPChgInLimit, txtPPTotalLimit, txtLossLimit, txtLossChgInLimit, txtLossTotalLimit, selectedFormType,
    hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal, hiddenCovCLimit, hiddenCovCChg,
    hiddenCovCTotal, hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal) {
    var formType = selectedFormType;

    // Calculate Limits from Coverage A
    var covALimit = RoundValue(parseInt(document.getElementById(txtDwLimit).value.replace(/,/g, "")));
    var covCChange = RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));
    isNaN(covALimit) ? covALimit = 0 : covALimit;
    isNaN(covCChange) ? covCChange = 0 : covCChange;
    var calcCovBLimit = parseInt(covALimit) * .10;
    var calcCovCLimit = 0;
    var calcCovCChg = 0;
    var calcCovDLimit = parseInt(covALimit) * .10;

    var covBChg = isNaN(parseInt(document.getElementById(txtRPSChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtRPSChgInLimit).value.replace(/,/g, "")));

    if (covBChg < 0) {
        alert("Must be a positive number");
        covBChg = 0;
        document.getElementById(txtRPSChgInLimit).focus();
    }

    //var covCChg = RoundValue(calcCovCChg);
    var covCChg = isNaN(parseInt(document.getElementById(txtPPChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));

    if (covCChg < 0) {
        alert("Must be a positive number");
        covCChg = 0;
        document.getElementById(txtPPChgInLimit).focus();
    }

    var covDChg = isNaN(parseInt(document.getElementById(txtLossChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtLossChgInLimit).value.replace(/,/g, "")));

    if (covDChg < 0) {
        alert("Must be a positive number");
        covDChg = 0;
        document.getElementById(txtLossChgInLimit).focus();
    }

    var calcCovBTotal = calcCovBLimit + covBChg;
    var calcCovCTotal = calcCovCLimit + covCChg;
    var calcCovDTotal = calcCovDLimit + covDChg;

    // Set Limits
    $("#" + txtDwLimit).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtRPSLimit).val(calcCovBLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtPPLimit).val(calcCovCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtLossLimit).val(calcCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovALimit).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovBLimit).val(calcCovBLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovCLimit).val(calcCovCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDLimit).val(calcCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    // Add Increase Limit
    $("#" + txtRPSChgInLimit).val(covBChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtPPChgInLimit).val(covCChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtLossChgInLimit).val(covDChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovBChg).val(covBChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovCChg).val(covCChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDChg).val(covDChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    // Update Totals
    $("#" + txtDwellingTotalLimit).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtRPSTotalLimit).val(calcCovBTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtPPTotalLimit).val(calcCovCTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtLossTotalLimit).val(calcCovDTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovATotal).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovBTotal).val(calcCovBTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovCTotal).val(calcCovCTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDTotal).val(calcCovDTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
}

// Calculate coverages From Personal Property Limit
function CalculateFromPPLimit(txtPPLimit, txtPPTotalLimit, txtLossLimit, txtLossChgInLimit, txtLossTotalLimit, hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal,
    hiddenCovCLimit, hiddenCovCChg, hiddenCovCTotal, hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal) {
    // Calculate Limits from Coverage C
    var covCLimit = document.getElementById(txtPPLimit).value.replace(/,/g, "");
    var calcCovDLimit = parseInt(covCLimit) * .40;

    var covDChg = RoundValue(parseInt(document.getElementById(txtLossChgInLimit).value.replace(/,/g, "")));

    var calcCovDTotal = calcCovDLimit + covDChg;

    // Set Limits
    $("#" + txtPPLimit).val(covCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtLossLimit).val(calcCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovCLimit).val(covCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDLimit).val(calcCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    // Add Increase Limit
    $("#" + txtLossChgInLimit).val(covDChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDChg).val(covDChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    // Update Totals
    $("#" + txtPPTotalLimit).val(covCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + txtLossTotalLimit).val(calcCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovCTotal).val(covCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDTotal).val(calcCovDTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
}

//
// Round a number to nearest 1,000
//
function RoundValue(number) {
    var returnNum = 0;

    if (number < 0) {
        number = number * -1;
        returnNum = Math.ceil(number / 1000) * 1000;

        returnNum = returnNum * -1;
    }
    else {
        returnNum = Math.ceil(number / 1000) * 1000;
    }

    return returnNum;
}

function ToggleEarthquake(chkEarthquake, dvEarthquakeDeduct, ddlEarthquakeDeduct) {
    var client = document.getElementById(ddlEarthquakeDeduct);

    if (document.getElementById(chkEarthquake).checked) {
        document.getElementById(dvEarthquakeDeduct).style.display = "block";
        document.getElementById(ddlEarthquakeDeduct).focus();
    }
    else {
        if (ConfirmDFRDialog()) {
            document.getElementById(dvEarthquakeDeduct).style.display = "none";
            client.selectedIndex = 0;
        }
        else
            document.getElementById(chkEarthquake).checked = true;
    }
}

// Displays dialog when coverage is removed
function ConfirmDFRDialog() {
    if (confirm("Are you sure you want to delete this item?"))
        return true;
    else
        return false;
}

// Toggle Checkbox Only
function ToggleCheckboxOnly(chkControl) {
    if (document.getElementById(chkControl).checked) {
        ;
    }
    else {
        if (ConfirmDFRDialog()) {
            ;
        }
        else {
            document.getElementById(chkControl).checked = true;
        }
    }
}

function ToggleMineSub(selectedControl, chkMineSubA, chkMineSubAB) {
    var splitID = selectedControl.id.split("_");
    if (selectedControl.checked) {
        if (splitID[splitID.length - 1] == "chkMineSubA")
            document.getElementById(chkMineSubAB).disabled = true;
        else
            document.getElementById(chkMineSubA).disabled = true;
    }
    else {
        if (ConfirmDFRDialog()) {
            document.getElementById(chkMineSubA).disabled = false;
            document.getElementById(chkMineSubAB).disabled = false;
        }
        else
            selectedControl.checked = true;
    }
}

function ToggleOccupancy(ddlStructureLeftClientID, ddlOccupancyClientID) {
    var structure = document.getElementById(ddlStructureLeftClientID);
    var occupancy = document.getElementById(ddlOccupancyClientID);
    //Both MOBILE MANUFACTURED and OWNER OCCUPIED ids are 14, not a copy-paste error
    if (structure && occupancy) { 
        if (structure.value == '14') {
            //StructureTypeId = "14" MOBILE MANUFACTURED
            occupancy.value = '14'; //OWNER OCCUPIED
            occupancy.disabled = true;
        } else {
            occupancy.disabled = false;
        }
    }
}

function ToggleStructure(ddlStructureLeftClientID, ddlOccupancyClientID) {
    var structure = document.getElementById(ddlStructureLeftClientID);
    var occupancy = document.getElementById(ddlOccupancyClientID);
    if (structure && occupancy) {
        if (occupancy.value == '15') {
            //OccupancyCodeId = "15" TENANT OCCUPIED
            $("#" + ddlStructureLeftClientID + " option[value='14']").attr('disabled', 'disabled'); //MOBILE MANUFACTURED
        } else {
            $("#" + ddlStructureLeftClientID + " option[value='14']").removeAttr('disabled'); //MOBILE MANUFACTURED
        }
    }
}