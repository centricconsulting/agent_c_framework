///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />

//Added 11/20/2019 for bug 27734 MLW
//Canine Liability Exclusion - Section II Optional Coverage - only show the message on the last entry of multiple canines - used on app gap
//NOTE: if the verbiage changes on ctl_Coverages_HOM_App_Item.ascx.vb populate, then check below that the message still shows the contains text below
$(document).ready(function () {
    $('[id*=divAppSpecialText]' + ":contains('Canine Liability Exclusion')").last().show();
});
//

//
// Additional Questions Scripts
//

// 11/30/21
// Handles clicks on the Woodburning Stove checkbox in the HOM property additional questions section
function HandleWoodburningStoveClicks(chkID, divID) {
    var chk = document.getElementById(chkID);
    var div = document.getElementById(divID);

    if (chk && div) {
        if (chk.checked) {
            div.style.display = '';
        }
        else {
            div.style.display = 'none';
        }
    }
    return true;
}

//
// Optional Coverages Scripts
//

// 10/25/21
// Handles clicks on the 'Add Address' button in the Special Event coverage section
function HandleSpecialEventAddButtonClick(trAddressSectionId, lblAddButtonID, lblDeleteButtonID) {
    var trAddressSection = document.getElementById(trAddressSectionId);
    var lblAddButton = document.getElementById(lblAddButtonID);
    var lblDeleteButton = document.getElementById(lblDeleteButtonID);

    if (trAddressSection && lblAddButton && lblDeleteButton) {
        trAddressSection.style.display = '';
        lblAddButton.style.display = 'none';
        lblDeleteButton.style.display = 'none';
    }
}

// 10/25/21 - handling on the page now but left this script just in case
// Handles clicks on the 'Delete' button in the Special Event coverage section
//function HandleSpecialEventDeleteButtonClick(trAddressSectionId, txtStreetNumId, txtStreetNameId, txtAptSuiteId, txtZipId, txtCityId, ddStateId, txtCountyId, lblAddButtonID, lblDeleteButtonID) {
//    var AddressSection = document.getElementById(trAddressSectionId);
//    var txtStreetNum = document.getElementById(txtStreetNumId);
//    var txtStreetName = document.getElementById(txtStreetNameId);
//    var txtAptSuite = document.getElementById(txtAptSuiteId);
//    var txtZip = document.getElementById(txtZipId);
//    var txtCity = document.getElementById(txtCityId);
//    var ddState = document.getElementById(ddStateId);
//    var txtCounty = document.getElementById(txtCountyId);
//    var lblAddButton = document.getElementById(lblAddButtonID);
//    var lblDeleteButton = document.getElementById(lblDeleteButtonID);

//    if (AddressSection && txtStreetNum && txtStreetName && txtAptSuite && txtZip && ddState && txtCounty && lblAddButton) {
//        if (confirm('Delete the Special Event Address information?')) {
//            txtStreetNum.innerText = '';
//            txtStreetName.innerText = '';
//            txtAptSuite.innerText = '';
//            txtZip.innerText = '';
//            txtCity.innerText = '';
//            ddState.selectedIndex = -1;
//            txtCounty.innerText = '';
//            lblAddButton.innerText = 'Add Address';
//            AddressSection.style.display = 'none';
//            lblAddButton.style.display = '';
//            lblDeleteButton.style.display = '';
//            alert('Special Event Address data deleted.');
//        }
//    }
//}


// 7/6/2020
// Handles Homeowners Plus checkbox clicks on the HOM coverage page
// The control variables are all set in the ctlSectionCoverageItem control code-behind.
//Updated 5/2/2022 for task 74106 MLW
//function HandleHOPLusCheckboxClicks(chkId, PreCyber, SeasonalOrSecondary) {
function HandleHOPLusCheckboxClicks(chkId, PreCyber, SeasonalOrSecondary, HPEEWaterBUEnabled, PreHPEEWaterBackup) {
    var chk = document.getElementById(chkId);
    if (typeof trBatteryBackupText_HPEEWaterBackup !== 'undefined' && trBatteryBackupText_HPEEWaterBackup !== null) {
        $('#' + trBatteryBackupText_HPEEWaterBackup).hide();
    } 
   
    if (chk && PreCyber && SeasonalOrSecondary) {
        if (chk.checked) {
            // HO+ is checked
            if (SeasonalOrSecondary == "1") {
                // Occupancy is seasonal or secondary - cannot have HO+ or Water Damage - we should never get here
                alert("HandleHOPlusCheckboxClicks - Should not be here - 1!!");
            }
            else {
                // HO+ is checked and not seasonal or secondary
                // Check the water damage checkbox
                if (typeof checkBoxId_waterDamage !== 'undefined' && checkBoxId_waterDamage !== null) {
                    $('#' + checkBoxId_waterDamage).prop('checked', true);
                    $('#' + checkBoxId_waterDamage).change();
                    $('#' + checkBoxId_waterDamage).attr('disabled', true);
                    $('#' + divDetails_waterDamage).show();
                }
                //Added 5/2/2022 for task 74106 MLW
                //Check the water backup checkbox
                if (typeof checkBoxId_HPEEWaterBackup !== 'undefined' && checkBoxId_HPEEWaterBackup !== null) {
                    if (HPEEWaterBUEnabled == 'True' || PreHPEEWaterBackup == "0") {
                        $('#' + checkBoxId_HPEEWaterBackup).prop('checked', true);
                        $('#' + checkBoxId_HPEEWaterBackup).change();
                        $('#' + checkBoxId_HPEEWaterBackup).attr('disabled', true);
                        $('#' + divDetails_HPEEWaterBackup).show();
                    } else {
                        $('#' + checkBoxId_HPEEWaterBackup).prop('checked', false);
                        $('#' + checkBoxId_HPEEWaterBackup).change();
                        $('#' + checkBoxId_HPEEWaterBackup).attr('disabled', true);
                        $('#' + divDetails_HPEEWaterBackup).hide();
                    }

                }

                // Uncheck and disable HOE and Sewer backup (sewer is always disabled)
                $('#' + checkBoxId_enhancement).prop('checked', false);
                $('#' + checkBoxId_enhancement).attr('disabled', true);
                $('#' + checkBoxId_backup).prop('checked', false);
                $('#' + checkBoxId_backup).change();
                $('#' + checkBoxId_backup).attr('disabled', true);
                $('#' + divDetails_backup).hide();
                if (PreCyber == "1") {
                    // for pre-cyber quotes uncheck and disable identity fraud
                    // Post-cyber quotes aren't eligible for identity fraud
                    if (typeof checkBoxId_identityFraud !== 'undefined' && checkBoxId_identityFraud !== null) {
                        $('#' + checkBoxId_identityFraud).prop('checked', false);
                        $('#' + checkBoxId_identityFraud).attr('disabled', true);
                    }
                }
            }
        }
        else {
            // HO+ is NOT checked
            // Uncheck and disable water damage if it exists
            if (typeof checkBoxId_waterDamage !== 'undefined' && checkBoxId_waterDamage !== null) {
                $('#' + checkBoxId_waterDamage).prop('checked', false);
                $('#' + checkBoxId_waterDamage).change();
                $('#' + checkBoxId_waterDamage).attr('disabled', true);
                $('#' + divDetails_waterDamage).hide();
            }

            //Added 5/2/2022 for task 74106 MLW
            //Uncheck and disable the water backup checkbox if it exists
            if (typeof checkBoxId_HPEEWaterBackup !== 'undefined' && checkBoxId_HPEEWaterBackup !== null) {                
                $('#' + checkBoxId_HPEEWaterBackup).prop('checked', false);
                $('#' + checkBoxId_HPEEWaterBackup).change();
                $('#' + checkBoxId_HPEEWaterBackup).attr('disabled', true);
                $('#' + divDetails_HPEEWaterBackup).hide();
            }

            // Enable the enhancement checkbox
            $('#' + checkBoxId_enhancement).attr('disabled', false);
            // If pre-cyber, enable the identity fraud coverage
            if (PreCyber == "1") {
                if (typeof checkBoxId_identityFraud !== 'undefined' && checkBoxId_identityFraud !== null) {
                    $('#' + checkBoxId_identityFraud).prop('checked', false);
                    $('#' + checkBoxId_identityFraud).attr('disabled', false);
                }
            }
            // Sewer Backup is always disabled
            $('#' + checkBoxId_backup).attr('disabled', true);
        }
    }
}

// 7/6/2020
// Handles Homeowners Enhancement checkbox clicks on the HOM coverage page
// The control variables are all set in the ctlSectionCoverageItem control code-behind.
function HandleHOEnhancementCheckboxClicks(chkId, PreCyber, SeasonalOrSecondary) {
    var chk = document.getElementById(chkId);
    if (typeof trBatteryBackupText_backup !== 'undefined' && trBatteryBackupText_backup !== null) {
        $('#' + trBatteryBackupText_backup).hide();
    }

     if (chk && PreCyber && SeasonalOrSecondary) {
         if (chk.checked) {
             // HO Enhancement is checked

             if (SeasonalOrSecondary == "1") {
                 // Occupancy is seasonal or secondary - cannot have HO+ or Water Damage, uncheck and disable them
                 $('#' + checkBoxId_backup).prop('checked', true);
                 $('#' + checkBoxId_backup).change();
                 $('#' + checkBoxId_backup).attr('disabled', true);
                 $('#' + divDetails_backup).show();
             }
             else {

                 // Checked but not seasonal or secondary
                 // Check the sewer backup checkbox
                 $('#' + checkBoxId_backup).prop('checked', true);
                 $('#' + checkBoxId_backup).change();
                 $('#' + checkBoxId_backup).attr('disabled', true);
                 $('#' + divDetails_backup).show();
                 // Uncheck and disable HO+ and Water Damage (Water Damage is always disabled)
                 if (typeof checkBoxId_plusenhancement !== 'undefined' && checkBoxId_plusenhancement !== null) {
                     $('#' + checkBoxId_plusenhancement).prop('checked', false);
                     $('#' + checkBoxId_plusenhancement).attr('disabled', true);
                 }
                 if (typeof checkBoxId_waterDamage !== 'undefined' && checkBoxId_waterDamage !== null) {
                     $('#' + checkBoxId_waterDamage).prop('checked', false);
                     $('#' + checkBoxId_waterDamage).attr('disabled', true);
                     $('#' + divDetails_waterDamage).hide();
                 }

                 //Added 5/2/2022 for task 74106 MLW
                 if (typeof checkBoxId_HPEEWaterBackup !== 'undefined' && checkBoxId_HPEEWaterBackup !== null) {
                     $('#' + checkBoxId_HPEEWaterBackup).prop('checked', false);
                     $('#' + checkBoxId_HPEEWaterBackup).change();
                     $('#' + checkBoxId_HPEEWaterBackup).attr('disabled', true);
                     $('#' + divDetails_HPEEWaterBackup).hide();
                 }

                 //$('#' + checkBoxId_plusenhancement).prop('checked', false);
                 //$('#' + checkBoxId_plusenhancement).attr('disabled', true);
                 //$('#' + checkBoxId_waterDamage).prop('checked', false);
                 //$('#' + checkBoxId_waterDamage).attr('disabled', true);
                 //if (PreCyber == "1") {
                 //    // for pre-cyber quotes uncheck and disable identity fraud
                 //    // Post-cyber quotes aren't eligible for identity fraud
                 //    $('#' + checkBoxId_identityFraud).prop('checked', false);
                 //    $('#' + checkBoxId_identityFraud).attr('disabled', true);
                 //}
             }
         }
         else {
             // HO Enhancement is NOT checked

            // Uncheck Sewer Backup
            $('#' + checkBoxId_backup).prop('checked', false);
            $('#' + checkBoxId_backup).change();
            $('#' + checkBoxId_backup).attr('disabled', true);
            $('#' + divDetails_backup).hide();

            // Enable HO+ 
            if (typeof checkBoxId_plusenhancement !== 'undefined' && checkBoxId_plusenhancement !== null) {
                //$('#' + checkBoxId_plusenhancement).prop('checked', false);
                $('#' + checkBoxId_plusenhancement).attr('disabled', false);
            }
        }
    }
}

// 3/27/2024
// Handles Personal Injury checkbox clicks on the HOM coverage page
function HandlePersonalInjuryCheckbox(chkId) {
    var chk = document.getElementById(chkId);

    if (chk) {
        // HO Enhancement or HO Plus is checked
        // Check and Disable Personal Injury
        if (chk.checked) {
            if (typeof checkBoxId_PersonalInjury !== 'undefined' && checkBoxId_PersonalInjury !== null) {
                $('#' + checkBoxId_PersonalInjury).prop('checked', true);
                $('#' + checkBoxId_PersonalInjury).attr('disabled', true);
            }
        } else {
            if (typeof checkBoxId_PersonalInjury !== 'undefined' && checkBoxId_PersonalInjury !== null) {
                $('#' + checkBoxId_PersonalInjury).prop('checked', false);
                $('#' + checkBoxId_PersonalInjury).attr('disabled', false);
            }

        }
    }
}


function CalculateIncreasedLimits(formName, increasedLimit, includedLimit, totalLimit) {

    switch (formName) {
        // Business Property Increased Limits
        case "HO_312":
            var client = document.getElementById(increasedLimit);
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            var increasedLimit = client.options[client.selectedIndex].text;
            var increasedLimitInt = increasedLimit.replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimitInt));
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage
        case "HO_53":
            var client = document.getElementById(increasedLimit);
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            var increasedLimit = client.options[client.selectedIndex].text;
            var increasedLimitInt = increasedLimit.replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimitInt));
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Firearms
        case "HO_65_HO_221_Firearms":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(parseInt(increasedLimit).toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Jewelry, Watches & Furs
        case "HO_61_Jewelry":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(parseInt(increasedLimit).toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Money
        case "HO_65_HO_221":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(parseInt(increasedLimit).toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Securities
        case "HO_65_HO_221_Securities":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(parseInt(increasedLimit).toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Silverware, Goldware, Pewterware
        case "HO_61_Silverware":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(parseInt(increasedLimit).toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Backup of Sewer or Drain
        case "92_173":
            var client = document.getElementById(increasedLimit);
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            var increasedLimit = client.options[client.selectedIndex].text;
            RoundValue100(increasedLimit);
            var increasedLimitInt = increasedLimit.replace(/,/g, "");
            var total = 5000 + parseInt(noNAN(increasedLimitInt));
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Loss Assessment
        case "HO_35":
            var client = document.getElementById(increasedLimit);
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            var increasedLimit = client.options[client.selectedIndex].text;
            RoundValue100(increasedLimit);
            var increasedLimitInt = increasedLimit.replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimitInt));
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Building Additions and Alterations
        case "HO_51":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Theft of Building Materials
        case "92_367":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Fire Department Service Charge
        case "ML_306":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;

        // Refrigerated Food Products
        case "92_267_Food":
            var client = document.getElementById(increasedLimit);
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            var increasedLimit = client.options[client.selectedIndex].text;
            RoundValue100(increasedLimit);
            var increasedLimitInt = increasedLimit.replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimitInt));
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Debris Removal
        case "92_267DR":
            var client = document.getElementById(increasedLimit);
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            var increasedLimit = client.options[client.selectedIndex].text;
            RoundValue100(increasedLimit);
            var increasedLimitInt = increasedLimit.replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimitInt));
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Increased Limits Motorized Vehicles
        case "ML_65":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Loss Assessment - Earthquake
        case "HO_35B":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Ordinance or Law
        case "HOM_1000":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
        // Outdoor Antennas
        case "ML_49":
            var includedLimit = $("#" + includedLimit).val().replace(/,/g, "");
            RoundValue100(increasedLimit);
            var increasedLimit = $("#" + increasedLimit).val().replace(/,/g, "");
            var total = parseInt(includedLimit) + parseInt(noNAN(increasedLimit));
            $("#" + increasedLimit).val(total.toLocaleString());
            $("#" + totalLimit).val(total.toLocaleString());
            break;
    }
}

//
// RVWatercraft Scripts
//
function updateWatercraftHeaderText(headerId, driverNum, firstNameId, lastNameID, yearID) {
    var firstNameText = $("#" + firstNameId).val();
    var lastNameText = $("#" + lastNameID).val();
    var yearText = $("#" + yearID).val();
    var newHeaderText = ("RV/WATERCRAFT #" + (driverNum + 1).toString() + " - " + yearText + " " + firstNameText + " " + lastNameText).toUpperCase();

    //document.getElementById(headerId).setAttribute('value', newHeaderText);
    $("#" + headerId).val(newHeaderText);

    newHeaderText = newHeaderText.toUpperCase();
    if (newHeaderText.length > 30) {
        $("#" + headerId).text(newHeaderText.substring(0, 32) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }

    //var TreeText = yearText + " " + firstNameText + " " + lastNameText;

    //if (TreeText.length == 2) {
    //    TreeText = "Vehicle " + (driverNum + 1).toString();
    //}
    //TreeText = TreeText.toUpperCase();
    //$("#cphMain_ctlTreeView_rptVehicles_lblVehicledescriptionPnl_" + driverNum.toString()).text(TreeText);
}

function toggleUnder25Message(under25OperatorPnl, under25OperatorPnlMsg) {
    if (document.getElementById(under25OperatorPnl).checked) {
        document.getElementById(under25OperatorPnlMsg).style.display = "block";
    }
    else {
        document.getElementById(under25OperatorPnlMsg).style.display = "none";
    }
}

function toggleMotorType(motorType, motorPanel, horsePowerPnl, motorYearPnl,
    motorCostNewPnl, motorSerialNumPnl, motorMakePnl, motorModelPnl, horsePower,
    motorYear, motorCostNew, motorSerialNum, motorMake, motorModel, hiddenSelectedForm,
    horsePowerCCRSLabel, hiddenMotorType) {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";

    if ($("#" + horsePower).val() == "" && $("#" + motorYear).val() == "" && $("#" + motorCostNew).val() == "" && $("#" + motorSerialNum).val() == "" && $("#" + motorMake).val() == "" && $("#" + motorModel).val() == "")
        confirmValue.value = "Yes"
    else {
        if (confirm("Changing Motor Type will remove all previously entered Motor values"))
            confirmValue.value = "Yes";
        else
            confirmValue.value = "No";
    }

    var client = document.getElementById(motorType);
    var selectedMotorType = client.options[client.selectedIndex].text;

    if (confirmValue.value == "Yes") {
        $("#" + horsePower).val("");
        $("#" + motorYear).val("");
        $("#" + motorCostNew).val("");
        $("#" + motorSerialNum).val("");
        $("#" + motorMake).val("");
        $("#" + motorModel).val("");
        $("#" + horsePowerCCRSLabel).val("");

        document.getElementById(motorPanel).style.display = "none";
        document.getElementById(horsePowerPnl).style.display = "none";
        document.getElementById(motorYearPnl).style.display = "none";
        document.getElementById(motorCostNewPnl).style.display = "none";
        document.getElementById(motorSerialNumPnl).style.display = "none";
        document.getElementById(motorMakePnl).style.display = "none";
        document.getElementById(motorModelPnl).style.display = "none";

        if ($("#" + hiddenSelectedForm).val() != "ML-2" && $("#" + hiddenSelectedForm).val() != "ML-4")
            $("#" + horsePowerCCRSLabel).text("*Horsepower/CCs");
        else
            $("#" + horsePowerCCRSLabel).text("*Rated Speed in MPH");

        switch (selectedMotorType.toUpperCase()) {
            case "INBOARD":
            case "INBOARD/OUTBOARD":
                document.getElementById(motorPanel).style.display = "block";
                document.getElementById(horsePowerPnl).style.display = "block";
                break;
            case "OUTBOARD":
                document.getElementById(motorPanel).style.display = "block";
                document.getElementById(horsePowerPnl).style.display = "block";
                document.getElementById(motorYearPnl).style.display = "block";
                document.getElementById(motorCostNewPnl).style.display = "block";
                document.getElementById(motorSerialNumPnl).style.display = "block";
                document.getElementById(motorMakePnl).style.display = "block";
                document.getElementById(motorModelPnl).style.display = "block";
                client.selectedIndex = 4;
                break;
        }

        $("#" + hiddenMotorType).val(selectedMotorType.toUpperCase());
    }
    else {
        switch ($("#" + hiddenMotorType).val()) {
            case "INBOARD":
                client.selectedIndex = 2;
                selectedMotorType = "INBOARD";
                break;
            case "INBOARD/OUTBOARD":
                client.selectedIndex = 3;
                selectedMotorType = "INBOARD/OUTBOARD";
                break;
            case "OUTBOARD":
                client.selectedIndex = 4;
                selectedMotorType = "OUTBOARD";
                break;
        }
    }
}

function toggleCoverageOptions(coverageOption, physicalDamagePnl, vehCostNewPnl, selectedCoverage, physicalDamageList,
    vehCostNew, hiddenSelectedCoverage) {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    var physicalDamageClient = document.getElementById(physicalDamageList);
    var selectedPD = physicalDamageClient.options[physicalDamageClient.selectedIndex].text;
    var client = document.getElementById(coverageOption);
    var selectedCoverageOption = client.options[client.selectedIndex].text;

    if (selectedPD == "" && $("#" + vehCostNew).val() == "")
        confirmValue.value = "Yes"
    else {
        if (selectedCoverageOption == "LIABILITY ONLY") {
            if (confirm("Changing Coverage Options will remove all previously entered Coverage values"))
                confirmValue.value = "Yes";
            else
                confirmValue.value = "No";
        }
        else
            confirmValue.value = "Yes";
    }

    if (confirmValue.value == "Yes") {
        document.getElementById(selectedCoverage).value = selectedCoverageOption;
        document.getElementById(physicalDamagePnl).style.display = "none";
        document.getElementById(vehCostNewPnl).style.display = "none";

        if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "") {
            document.getElementById(physicalDamagePnl).style.display = "block";
            document.getElementById(vehCostNewPnl).style.display = "block";
        }
        else {
            physicalDamageClient.selectedIndex = 0;
            $("#" + vehCostNew).val("");
        }

        $("#" + hiddenSelectedCoverage).val(selectedCoverageOption);
    }
    else {
        switch ($("#" + hiddenSelectedCoverage).val()) {
            case "PHYSICAL DAMAGE AND LIABILITY":
                client.selectedIndex = 1;
                selectedCoverageOption = "PHYSICAL DAMAGE AND LIABILITY";
                break;
            case "PHYSICAL DAMAGE ONLY":
                client.selectedIndex = 2;
                selectedWatercraft = "PHYSICAL DAMAGE ONLY";
                break;
            case "LIABILITY ONLY":
                client.selectedIndex = 3;
                selectedWatercraft = "LIABILITY ONLY";
                break;
        }
    }
}

function toggleWatercraftType(watercraftType, coverageOpt, vehYearPnl, vehLengthPnl, vehCostNewPnl, motorType,
    motorTypeList, motorTypeIO, propDeductiblePnl, liabilityOnlyPnl, vehSerialNumPnl, vehMakePnl, vehModelPnl,
    descriptionPnl, motorPnl, horsePowerPnl, motorYearPnl, motorCostNewPnl, motorSerialNumPnl, motorMakePnl,
    motorModelPnl, bodilyInjuryPnl, under25OperatorPnl, coverageOptList, coverageOptPD, coverageOptPDL, coverageOptLib,
    motorTypeIn, motorTypeOut, propDeductibleList, vehYear, vehLength, vehCostNew, bodilyInjuryList, under25Oper,
    vehSerialNum, vehMake, vehModel, horsePower, motorYear, motorCostNew, motorSerialNum, motorMake, motorModel,
    under25OperMsg, costNewLabel, limitLabel, hiddenSelectedCoverage, hiddenSelectedForm, descriptionText,
    horsePowerCCRSLabel, hiddenWatercraftType) {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    var coverageClient = document.getElementById(coverageOptList);
    var selectedCoverage = coverageClient.options[coverageClient.selectedIndex].text;
    var physicalDamageClient = document.getElementById(propDeductibleList);
    var selectedPD = physicalDamageClient.options[physicalDamageClient.selectedIndex].text;
    var bodilyInjuryClient = document.getElementById(bodilyInjuryList);
    var selectedBI = bodilyInjuryClient.options[bodilyInjuryClient.selectedIndex].text;

    if (selectedCoverage == "" && selectedPD == "" && $("#" + vehYear).val() == "" && $("#" + vehLength).val() == "" && $("#" + vehCostNew).val() == "" &&
        selectedBI == "" && document.getElementById(under25Oper).checked == false && $("#" + vehSerialNum).val() == "" && $("#" + vehMake).val() == "" &&
        $("#" + vehModel).val() == "" && $("#" + horsePower).val() == "" && $("#" + motorYear).val() == "" && $("#" + motorCostNew).val() == "" &&
        $("#" + motorSerialNum).val() == "" && $("#" + motorMake).val() == "" && $("#" + motorModel).val() == "")
        confirmValue.value = "Yes"
    else {
        if (confirm("Changing the RV/Watercraft type will remove all previously entered Coverage Options, related information and Motor information"))
            confirmValue.value = "Yes";
        else
            confirmValue.value = "No";
    }

    var client = document.getElementById(watercraftType);
    var selectedWatercraft = client.options[client.selectedIndex].text;

    if (confirmValue.value == "Yes") {
        // Clear all fields
        coverageClient.selectedIndex = 0;

        var motorClient = document.getElementById(motorTypeList);
        motorClient.selectedIndex = 0;

        physicalDamageClient.selectedIndex = 0;
        bodilyInjuryClient.selectedIndex = 0;

        $("#" + vehYear).val("");
        $("#" + vehLength).val("");
        $("#" + vehCostNew).val("");
        document.getElementById(under25Oper).checked = false;
        $("#" + vehSerialNum).val("");
        $("#" + vehMake).val("");
        $("#" + vehModel).val("");
        $("#" + horsePower).val("");
        $("#" + motorYear).val("");
        $("#" + motorCostNew).val("");
        $("#" + motorSerialNum).val("");
        $("#" + motorMake).val("");
        $("#" + motorModel).val("");
        $("#" + hiddenSelectedCoverage).val("");
        $("#" + descriptionText).val("");
        $("#" + horsePowerCCRSLabel).val("");

        // Hide all fields and then display only the needed ones
        document.getElementById(coverageOpt).style.display = "none";
        document.getElementById(coverageOptList).style.display = "none";
        document.getElementById(coverageOptPD).style.display = "none";
        document.getElementById(coverageOptPDL).style.display = "none";
        document.getElementById(coverageOptLib).style.display = "none";
        document.getElementById(vehYearPnl).style.display = "none";
        document.getElementById(vehLengthPnl).style.display = "none";
        document.getElementById(vehCostNewPnl).style.display = "none";
        document.getElementById(motorType).style.display = "none";
        document.getElementById(motorTypeList).style.display = "none";
        document.getElementById(motorTypeIO).style.display = "none";
        document.getElementById(motorTypeIn).style.display = "none";
        document.getElementById(motorTypeOut).style.display = "none";
        document.getElementById(propDeductiblePnl).style.display = "none";
        document.getElementById(liabilityOnlyPnl).style.display = "none";
        document.getElementById(vehSerialNumPnl).style.display = "none";
        document.getElementById(vehMakePnl).style.display = "none";
        document.getElementById(vehModelPnl).style.display = "none";
        document.getElementById(descriptionPnl).style.display = "none";
        document.getElementById(motorPnl).style.display = "none";
        document.getElementById(horsePowerPnl).style.display = "none";
        document.getElementById(motorYearPnl).style.display = "none";
        document.getElementById(motorCostNewPnl).style.display = "none";
        document.getElementById(motorSerialNumPnl).style.display = "none";
        document.getElementById(motorMakePnl).style.display = "none";
        document.getElementById(motorModelPnl).style.display = "none";
        document.getElementById(bodilyInjuryPnl).style.display = "none";
        document.getElementById(under25OperatorPnl).style.display = "none";
        document.getElementById(under25OperMsg).style.display = "none";
        document.getElementById(costNewLabel).style.display = "none";
        document.getElementById(limitLabel).style.display = "none";

        if ($("#" + hiddenSelectedForm).val() != "ML-2" && $("#" + hiddenSelectedForm).val() != "ML-4")
            $("#" + horsePowerCCRSLabel).text("*Horsepower/CCs");
        else
            $("#" + horsePowerCCRSLabel).text("*Rated Speed in MPH");

        switch (selectedWatercraft) {
            case "WATERCRAFT":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(vehLengthPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(motorType).style.display = "block";
                document.getElementById(motorTypeList).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(bodilyInjuryPnl).style.display = "block";
                document.getElementById(under25OperatorPnl).style.display = "block";
                break;
            case "BOAT MOTOR ONLY":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptPDL).style.display = "block";
                document.getElementById(motorType).style.display = "block";
                document.getElementById(motorTypeOut).style.display = "block";
                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(motorPnl).style.display = "block";
                document.getElementById(horsePowerPnl).style.display = "block";
                document.getElementById(motorYearPnl).style.display = "block";
                document.getElementById(motorCostNewPnl).style.display = "block";
                document.getElementById(motorSerialNumPnl).style.display = "block";
                document.getElementById(motorMakePnl).style.display = "block";
                document.getElementById(motorModelPnl).style.display = "block";
                $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE AND LIABILITY");
                motorClient.selectedIndex = 3;
                break;
            case "4 WHEEL ATV":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptPD).style.display = "block";
                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(vehCostNewPnl).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                break;
            case "GOLF CART":
                document.getElementById(coverageOpt).style.display = "block";

                if ($("#" + hiddenSelectedForm).val() != "ML-2" && $("#" + hiddenSelectedForm).val() != "ML-4") {
                    document.getElementById(coverageOptPDL).style.display = "block";
                    $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE AND LIABILITY");
                }
                else {
                    document.getElementById(coverageOptPD).style.display = "block";
                    $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                }

                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehCostNewPnl).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                break;
            case "BOAT TRAILER":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptPD).style.display = "block";
                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehCostNewPnl).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                break;
            case "ACCESSORIES & EQUIPMENT":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptPD).style.display = "block";
                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(limitLabel).style.display = "block";
                document.getElementById(vehCostNewPnl).style.display = "block";
                document.getElementById(descriptionPnl).style.display = "block";
                $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                $("#" + descriptionText).val("Accessories and Equipment Description");
                break;
            case "JET SKIS & WAVERUNNERS":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehLengthPnl).style.display = "block";
                document.getElementById(motorType).style.display = "block";
                document.getElementById(motorTypeIn).style.display = "block";
                document.getElementById(bodilyInjuryPnl).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(motorPnl).style.display = "block";
                document.getElementById(horsePowerPnl).style.display = "block";
                document.getElementById(under25OperatorPnl).style.display = "block";
                break;
            case "SNOWMOBILE - NAMED PERILS":
            case "SNOWMOBILE - SPECIAL COVERAGE":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(descriptionPnl).style.display = "block";
                $("#" + descriptionText).val(selectedWatercraft);
                break;            
        }

        $("#" + hiddenWatercraftType).val(selectedWatercraft.toUpperCase());
    }
    else {
        switch ($("#" + hiddenWatercraftType).val()) {
            case "WATERCRAFT":
                client.selectedIndex = 1;
                selectedWatercraft = "WATERCRAFT";
                break;
            case "BOAT MOTOR ONLY":
                client.selectedIndex = 2;
                selectedWatercraft = "BOAT MOTOR ONLY";
                break;
            case "BOAT TRAILER":
                client.selectedIndex = 3;
                selectedWatercraft = "BOAT TRAILER";
                break;
            case "ACCESSORIES & EQUIPMENT":
                client.selectedIndex = 4;
                selectedWatercraft = "ACCESSORIES & EQUIPMENT";
                break;
            case "GOLF CART":
                client.selectedIndex = 5;
                selectedWatercraft = "WATERCRAFT";
                break;
            case "JET SKIS & WAVERUNNERS":
                client.selectedIndex = 6;
                selectedWatercraft = "WATERCRAFT";
                break;
            case "4 WHEEL ATV":
                client.selectedIndex = 7;
                selectedWatercraft = "WATERCRAFT";
                break;
        }
    }
}

function ClearMotorEntries(horsePower, vehYear, vehCostNew, vehSerialNum, vehMake, vehModel) {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";

    if ($("#" + horsePower).val() == "" && $("#" + vehYear).val() == "" && $("#" + vehCostNew).val() == "" && $("#" + vehSerialNum).val() == "" && $("#" + vehMake).val() == "" && $("#" + vehModel).val() == "")
        confirmValue.value = "Yes"
    else {
        if (confirm("Are you sure you want to clear Motor information?"))
            confirmValue.value = "Yes";
        else
            confirmValue.value = "No";
    }

    if (confirmValue.value == "Yes") {
        $("#" + horsePower).val("")
        $("#" + vehYear).val("")
        $("#" + vehCostNew).val("")
        $("#" + vehSerialNum).val("")
        $("#" + vehMake).val("")
        $("#" + vehModel).val("")
    }
}

//
// General Scripts
//

//
// Round a number up by 100 & validate that it is not more than the deductible
//
function RoundValue100Validate(number, deductible) {
    var client = document.getElementById(deductible);
    var deductibleLimit = client.options[client.selectedIndex].text;
    var deductibleInt = parseInt(deductibleLimit);
    var numberInt = parseInt(noNAN($("#" + number).val()).toString());
    var returnNum = 0;

    if (numberInt < 0) {
        numberInt = numberInt * -1;
        returnNum = Math.ceil(numberInt / 100) * 100;

        returnNum = returnNum * -1;
    }
    else {
        returnNum = Math.ceil(numberInt / 100) * 100;
    }

    $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))

    if (deductibleLimit != "") {
        if (deductibleInt > returnNum)
            alert("Limit cannot be less than the Deductible");
    }
}

//
// Round a number up by 100
//
function RoundValue100(number) {
    var numberInt = parseInt($("#" + number).val()).toString();
    var returnNum = 0;

    if (numberInt < 0) {
        numberInt = numberInt * -1;
        returnNum = Math.ceil(numberInt / 100) * 100;

        returnNum = returnNum * -1;
    }
    else {
        returnNum = Math.ceil(numberInt / 100) * 100;
    }

    $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
}

//
// Round a number up by 1,000
//
function RoundValue1000(number) {
    var returnNum = 0;

    if (number < 0) {
        number = number * -1;
        returnNum = Math.ceil(number / 1000) * 1000;

        returnNum = returnNum * -1;
    }
    else {
        returnNum = Math.ceil(number / 1000) * 1000;
    }

    $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
}

//
// Check for null value
//
function noNAN(value) {
    return isNaN(value) || !value ? 0 : value;
}

function ConfirmDialog() {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    if (confirm("Are you sure you want to delete this item?"))
        confirmValue.value = "Yes";
    else
        confirmValue.value = "No";

    document.forms[0].appendChild(confirmValue);
}

//Added 5/3/2022 for task 74106 MLW
function ToggleHPEELimits(sender) {
    //For Home Plus Enhancement Endorsement (1020) only, the water damage and water backup must match values until water damage increased limit is 20000
    var includedLimit_waterDamage = document.getElementById(txtIncludedLimit_waterDamage);
    var includedLimit_waterDamageValue = parseInt(noNAN(includedLimit_waterDamage.value.replace(/,/g, "")));
    var increasedLimit_waterDamage = document.getElementById(ddIncreasedLimit_waterDamage);
    var increasedLimit_waterDamageValue = parseInt(noNAN(increasedLimit_waterDamage.options[increasedLimit_waterDamage.selectedIndex].text.replace(/,/g, "")));
    var totalLimit_waterDamage = document.getElementById(txtTotalLimit_waterDamage);
    var includedLimit_waterBackup = document.getElementById(txtIncludedLimit_HPEEWaterBackup);
    var includedLimit_waterBackupValue = parseInt(noNAN(includedLimit_waterBackup.value.replace(/,/g, "")));
    var increasedLimit_waterBackup = document.getElementById(ddIncreasedLimit_HPEEWaterBackup);
    var increasedLimit_waterBackupValue = parseInt(noNAN(increasedLimit_waterBackup.options[increasedLimit_waterBackup.selectedIndex].text.replace(/,/g, "")));
    var totalLimit_waterBackup = document.getElementById(txtTotalLimit_HPEEWaterBackup);
    if (increasedLimit_waterBackupValue < 20000) {
        if (sender == "WaterBackup") {
            increasedLimit_waterDamage.value = increasedLimit_waterBackup.options[increasedLimit_waterBackup.selectedIndex].value;
            var newTotalLimit_waterDamageValue = includedLimit_waterDamageValue + increasedLimit_waterBackupValue;
            totalLimit_waterDamage.value = ifm.vr.stringFormating.asNumberWithCommas(newTotalLimit_waterDamageValue);
        } else {
            increasedLimit_waterBackup.value = increasedLimit_waterDamage.options[increasedLimit_waterDamage.selectedIndex].value;
            var newTotalLimit_waterBackupValue = includedLimit_waterBackupValue + increasedLimit_waterDamageValue;
            totalLimit_waterBackup.value = ifm.vr.stringFormating.asNumberWithCommas(newTotalLimit_waterBackupValue);
        }
    } else {
        //Added 6/8/2022 for task 74106 MLW
        if (sender == "WaterBackup") {
            //Updated 6/24/2022 for bug 75727 MLW
            if (!(IsEndorsementTransaction && increasedLimit_waterDamage.selectedIndex > 3)) {
                increasedLimit_waterDamage.selectedIndex = 3;
                var newIncreasedLimit_waterDamageValue = parseInt(noNAN(increasedLimit_waterDamage.options[increasedLimit_waterDamage.selectedIndex].text.replace(/,/g, "")));
                var newTotalLimit_waterDamageValue = includedLimit_waterDamageValue + newIncreasedLimit_waterDamageValue;
                totalLimit_waterDamage.value = ifm.vr.stringFormating.asNumberWithCommas(newTotalLimit_waterDamageValue);
            }
        }
    }
}

function ShowBatteryBackupMsg(sender) {
    if (sender == 'HPEE') {
        if (typeof trBatteryBackupText_HPEEWaterBackup !== 'undefined' && trBatteryBackupText_HPEEWaterBackup !== null) {
            $('#' + trBatteryBackupText_HPEEWaterBackup).hide();
        }
        var increasedLimit_HPEEWaterBackup = document.getElementById(ddIncreasedLimit_HPEEWaterBackup);
        var increasedLimit_HPEEWaterBackupValue = parseInt(noNAN(increasedLimit_HPEEWaterBackup.options[increasedLimit_HPEEWaterBackup.selectedIndex].text.replace(/,/g, "")));
        if (increasedLimit_HPEEWaterBackupValue >= 20000) {
            if (typeof trBatteryBackupText_HPEEWaterBackup !== 'undefined' && trBatteryBackupText_HPEEWaterBackup !== null) {
                $('#' + trBatteryBackupText_HPEEWaterBackup).show();
            }
        }
    } else {
        if (typeof trBatteryBackupText_backup !== 'undefined' && trBatteryBackupText_backup !== null) {
            $('#' + trBatteryBackupText_backup).hide();
        }
        var increasedLimit_waterBackup = document.getElementById(ddIncreasedLimit_backup);
        var increasedLimit_waterBackupValue = parseInt(noNAN(increasedLimit_waterBackup.options[increasedLimit_waterBackup.selectedIndex].text.replace(/,/g, "")));
        if (increasedLimit_waterBackupValue >= 20000) {
            if (typeof trBatteryBackupText_backup !== 'undefined' && trBatteryBackupText_backup !== null) {
                $('#' + trBatteryBackupText_backup).show();
            }
        }
    }
}
function ShowBatteryBackupMsgOld(ddIncreasedLimitId, trBatteryBackupId) {
    if (typeof trBatteryBackupId !== 'undefined' && trBatteryBackupId !== null) {
        $('#' + trBatteryBackupId).hide();
    }
    var increasedLimit_waterBackup = document.getElementById(ddIncreasedLimitId);
    var increasedLimit_waterBackupValue = parseInt(noNAN(increasedLimit_waterBackup.options[increasedLimit_waterBackup.selectedIndex].text.replace(/,/g, "")));
    if (increasedLimit_waterBackupValue >= 20000) {
        if (typeof trBatteryBackupId !== 'undefined' && trBatteryBackupId !== null) {
            $('#' + trBatteryBackupId).show();
        }
    }
}

// Added 07/11/2023 for task WS-1286
function calculateStructureAge(txtYearBuilt, divDwellingAgeText) {
    var dvDwellingAgeText = document.getElementById(divDwellingAgeText);
    dvDwellingAgeText.style.display = "none";
    var yearBuilt = document.getElementById(txtYearBuilt).value;
    var today = new Date().getFullYear()
    if (yearBuilt != null && yearBuilt != '' && !isNaN(yearBuilt)) {
        var ageOfStructure = today - yearBuilt
        if (parseInt(ageOfStructure) >= 75) {
            dvDwellingAgeText.style.display = "block";
        }
    }
}