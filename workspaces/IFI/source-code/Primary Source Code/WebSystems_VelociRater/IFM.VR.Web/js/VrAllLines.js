///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />


//
// Inland Marine
//
//
// Round a number up by 100 & validate that it is not more than the deductible
//
function RoundIMValue100Validate(number, deductible) {
    var client = document.getElementById(deductible);
    var deductibleLimit = client.options[client.selectedIndex].text;
    var deductibleInt = parseInt(deductibleLimit);
    //var numberInt = parseInt(noNAN($("#" + number).val()).toString().replace(",",""));
    var numberInt = noNAN(($("#" + number).val().toString()).replace(",", ""));
    var returnNum = 0;
    if (ifm.vr.currentQuote.isEndorsement) {
        // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
        var returnNum = 0;

        if (numberInt < 0) {
            numberInt = numberInt * -1;
            returnNum = Math.ceil(numberInt / 5) * 5;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(numberInt / 5) * 5;
        }

        $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        if (deductibleLimit != "") {
            if (deductibleInt > returnNum)
                alert("Limit cannot be less than the Deductible");
        }
        //return returnNum;
    }
    else {
        if (numberInt < 0) {
            uqunit
            numberInt = numberInt * -1;
            returnNum = Math.ceil(numberInt / 100) * 100;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(numberInt / 100) * 100;
        }
    }

    $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    if (deductibleLimit != "") {
        if (deductibleInt > returnNum)
            alert("Limit cannot be less than the Deductible");
    }
}


//function RoundIMValue100Validate(txtbox, deductible) {
//    var txt = document.getElementById(txtbox);
//    var client = document.getElementById(deductible);
//    var deductibleLimit = client.options[client.selectedIndex].text;
//    var deductibleInt = parseInt(deductibleLimit);
//    //var numberInt = parseInt(noNAN($("#" + number).val()).toString().replace(",",""));
//    var number = txt.value;
//    var numberInt = noNAN(($("#" + number).val().toString()).replace(",", ""));
//    var returnNum = 0;
//    if (ifm.vr.currentQuote.isEndorsement) {
//        // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
//        var returnNum = 0;

//        if (number < 0) {
//            number = number * -1;
//            returnNum = Math.ceil(number / 5) * 5;

//            returnNum = returnNum * -1;
//        }
//        else {
//            returnNum = Math.ceil(number / 5) * 5;
//        }

//        return returnNum;
//    }
//    else {
//        if (numberInt < 0) {
//            uqunit
//            numberInt = numberInt * -1;
//            returnNum = Math.ceil(numberInt / 100) * 100;

//            returnNum = returnNum * -1;
//        }
//        else {
//            returnNum = Math.ceil(numberInt / 100) * 100;
//        }
//    }

//    $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

//    if (deductibleLimit != "") {
//        if (deductibleInt > returnNum)
//            alert("Limit cannot be less than the Deductible");
//    }
//}


//
// RV/Watercraft
//

function toggleRVWatercraftType(watercraftType, coverageOpt, vehYearPnl, vehLengthPnl, vehCostNewPnl, motorType,
    motorTypeList, motorTypeIO, propDeductiblePnl, liabilityOnlyPnl, vehSerialNumPnl, vehMakePnl, vehModelPnl,
    descriptionPnl, motorPnl, horsePowerPnl, motorYearPnl, motorCostNewPnl, motorSerialNumPnl, motorMakePnl,
    motorModelPnl, bodilyInjuryPnl, under25OperatorPnl, coverageOptList, coverageOptPD, coverageOptPDL, coverageOptLib,
    motorTypeIn, motorTypeOut, propDeductibleList, vehYear, vehLength, vehCostNew, bodilyInjuryList, under25Oper,
    vehSerialNum, vehMake, vehModel, horsePower, motorYear, motorCostNew, motorSerialNum, motorMake, motorModel,
    under25OperMsg, costNewLabel, limitLabel, hiddenSelectedCoverage, hiddenSelectedForm, descriptionText,
    horsePowerCCRSLabel, hiddenWatercraftType, divVehTypeoWatercraftText) {


    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    var coverageClient = document.getElementById(coverageOptList);
    var currentControlID = coverageClient.id.split("_")[coverageClient.id.split("_").length - 1];
    var selectedCoverage = coverageClient.options[coverageClient.selectedIndex].text;
    var physicalDamageClient = document.getElementById(propDeductibleList);
    var selectedPD = physicalDamageClient.options[physicalDamageClient.selectedIndex].text;
    var bodilyInjuryClient = document.getElementById(bodilyInjuryList);
    var selectedBI = bodilyInjuryClient.options[bodilyInjuryClient.selectedIndex].text;
    var occupancyCodeID = $('[id*=hiddenOccupancyCodeID]').first().val();
    var pnlOwnerOtherThanInsured = $('[id*=pnlOwnerOtherThanInsured_' + currentControlID + ']').attr('id');
    var pnlCollisionCoverage = $('[id*=pnlCollisionCoverage_' + currentControlID + ']').attr('id');
    var pnlPropertyDeductibleTextBox = $('[id*=pnlPropertyDeductibleTextBox_' + currentControlID + ']').attr('id');
    var txtPropertyDeductibleTextBox = $('[id*=txtPropertyDeductibleTextBox_' + currentControlID + ']').attr('id');
    var watercraftTypeText = $('#' + watercraftType + ' :selected').text();
    var checkCollisionCoverage = $('[id*=chkCollisionCoverage_' + currentControlID + ']').attr("id");
    var checkOwnerOtherThanInsured = $('[id*=chkOwnerOtherThanInsured_' + currentControlID + ']').attr("id");
    var coverageLiabOnlyDdlOption = $("#" + coverageOptList).children("option[value='LIABILITY ONLY']");

    var useUpdatedHOM2018CodePath = "";
    if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") === true) {
        useUpdatedHOM2018CodePath = true;
    } else {
        useUpdatedHOM2018CodePath = false;
    }

    // Add Liability Only to Coverage Dropdown box, if needed Farm Only
    if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId) {
        if (coverageClient.childElementCount < 4) {
            var opt = document.createElement("option");
            coverageClient.options.add(opt);
            opt.text = "LIABILITY ONLY";
            opt.value = "LIABILITY ONLY";
        }
    }



    if (watercraftTypeText !== $('#' + hiddenWatercraftType).val()) {
        if ($('#' + hiddenWatercraftType).val() === "") {
            confirmValue.value = "Yes";
        } else {
            //For testing the big if statement below, you can uncomment this section to see what is happening. Otherwise, keep this commented out.
            //if (selectedCoverage && $("#" + coverageOptList).is(":visible")) {
            //    alert('Visible and SelectedCoverage=' + selectedCoverage);
            //} else if (selectedCoverage === "" && $("#" + coverageOptList).is(":visible") === false) {
            //    alert('Not Visible and SelectedCoverage=' + selectedCoverage);
            //}
            //if (selectedPD) { alert('SelectedPD=' + selectedPD); }
            //if ($("#" + vehYear).val()) { alert('VehicleYear=' + $("#" + vehYear).val()); }
            //if ($("#" + vehLength).val()) { alert('VehicleLength=' + $("#" + vehLength).val()); }
            //if ($("#" + vehCostNew).val()) { alert('VehicleCostNew=' + $("#" + vehCostNew).val()); }
            //if (selectedBI) { alert('SelectedBI=' + selectedBI); }
            //if (document.getElementById(under25Oper).checked != false) { alert('Under250IsFalse?=' + document.getElementById(under25Oper).checked); }
            //if ($("#" + vehSerialNum).val()) { alert('VehicleSerialNum=' + $("#" + vehSerialNum).val()); }
            //if ($("#" + vehMake).val()) { alert('VehicleMake=' + $("#" + vehMake).val()); }
            //if ($("#" + vehModel).val()) { alert('VehicleModel=' + $("#" + vehModel).val()); }
            //if ($("#" + horsePower).val()) { alert('HorsePower=' + $("#" + horsePower).val()); }
            //if ($("#" + motorYear).val()) { alert('MotorYear=' + $("#" + motorYear).val()); }
            //if ($("#" + motorCostNew).val()) { alert('MotorCostNew=' + $("#" + motorCostNew).val()); }
            //if ($("#" + motorSerialNum).val()) { alert('MotorSerialNum=' + $("#" + motorSerialNum).val()); }
            //if ($("#" + motorMake).val()) { alert('MotorMake=' + $("#" + motorMake).val()); }
            //if ($("#" + motorModel).val()) { alert('MotorModel=' + $("#" + motorModel).val()); }

            if (((selectedCoverage == "" && $("#" + coverageOptList).is(":visible")) || selectedCoverage && $("#" + coverageOptList).is(":visible") === false) && selectedPD == "" && $("#" + vehYear).val() == "" && $("#" + vehLength).val() == "" && $("#" + vehCostNew).val() == "" &&
                selectedBI == "" && document.getElementById(under25Oper).checked == false && $("#" + vehSerialNum).val() == "" && $("#" + vehMake).val() == "" &&
                $("#" + vehModel).val() == "" && $("#" + horsePower).val() == "" && $("#" + motorYear).val() == "" && $("#" + motorCostNew).val() == "" &&
                $("#" + motorSerialNum).val() == "" && $("#" + motorMake).val() == "" && $("#" + motorModel).val() == "") {
                confirmValue.value = "Yes";
            } else {
                if (confirm("Changing the RV/Watercraft type will remove all previously entered Coverage Options, related information and Motor information"))
                    confirmValue.value = "Yes";
                else
                    confirmValue.value = "No";
            }
        }

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
        document.getElementById(propDeductibleList).style.display = "block"; //For some reason this is getting set to none. I haven't been able to find how this is happening. Hoping this helps fix the problem.'

        document.getElementById(pnlOwnerOtherThanInsured).style.display = "none";
        document.getElementById(pnlCollisionCoverage).style.display = "none";
        document.getElementById(pnlPropertyDeductibleTextBox).style.display = "none";

        if (useUpdatedHOM2018CodePath === true) {
            $("#" + horsePowerCCRSLabel).text("*Horsepower/CCs");
        } else {
            if ($("#" + hiddenSelectedForm).val() != "ML-2" && $("#" + hiddenSelectedForm).val() != "ML-4")
                $("#" + horsePowerCCRSLabel).text("*Horsepower/CCs");
            else
                $("#" + horsePowerCCRSLabel).text("*Rated Speed in MPH");

            if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId)
                $("#" + horsePowerCCRSLabel).text("*Rated Speed in MPH");
        }

        //Check if option 3 "Liability Only" exists, if not add it
        if ($("#" + coverageOptList + " option:contains('LIABILITY ONLY')").length < 1) {
            var o = new Option("LIABILITY ONLY", "LIABILITY ONLY");
            /// jquerify the DOM object 'o' so we can use the html method
            $(o).html("LIABILITY ONLY");
            $("#" + coverageOptList).append(o);
        }
        document.getElementById(divVehTypeoWatercraftText).style.display = "none";

        switch (selectedWatercraft) {
            case "WATERCRAFT":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(vehLengthPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                //document.getElementById(vehCostNewPnl).style.display = "block"; // Matt A 4-21-16
                document.getElementById(motorType).style.display = "block";
                document.getElementById(motorTypeList).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(bodilyInjuryPnl).style.display = "block";
                // Check form Farm
                if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId.toString()) {
                    document.getElementById(under25OperatorPnl).style.display = "none";
                    document.getElementById(divVehTypeoWatercraftText).style.display = "block";
                }
                else {
                    document.getElementById(under25OperatorPnl).style.display = "block";
                    document.getElementById(divVehTypeoWatercraftText).style.display = "none";
                }
                break;
            case "SAILBOAT":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(vehLengthPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(bodilyInjuryPnl).style.display = "block";
                break;
            case "BOAT MOTOR ONLY":
                document.getElementById(coverageOpt).style.display = "block";
                // Check for Farm LOB
                if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId) {
                    document.getElementById(coverageOptList).style.display = "block";
                    coverageClient.remove(3);
                } else {
                    document.getElementById(coverageOptPDL).style.display = "block";
                    //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE AND LIABILITY");
                }

                //document.getElementById(coverageOptPDL).style.display = "block";
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
                //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE AND LIABILITY");
                motorClient.selectedIndex = 3;
                break;
            case "4 WHEEL ATV":
                document.getElementById(coverageOpt).style.display = "block";

                // Check for Farm LOB - updated to also check for HOM and new version of HOM
                if ((ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId) || (useUpdatedHOM2018CodePath === true)) {
                    document.getElementById(coverageOptList).style.display = "block";
                    document.getElementById(propDeductiblePnl).style.display = "none"; //Added 5/15/18 for Bug 26544 MLW
                    document.getElementById(vehCostNewPnl).style.display = "none"; //Added 5/15/18 for Bug 26544 MLW
                } else {
                    document.getElementById(coverageOptPD).style.display = "block";
                    //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                    document.getElementById(propDeductiblePnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                    document.getElementById(vehCostNewPnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                }
                //document.getElementById(propDeductiblePnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                //document.getElementById(vehCostNewPnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                break;
            case "GOLF CART":
                if (useUpdatedHOM2018CodePath === true && ifm.vr.currentQuote.lobId === "2") {
                    document.getElementById(coverageOptList).style.display = "block";
                    //Updated 5/15/18 for Bug 26544 MLW
                    //document.getElementById(pnlPropertyDeductibleTextBox).style.display = "block";
                    document.getElementById(pnlPropertyDeductibleTextBox).style.display = "none";
                    document.getElementById(vehCostNewPnl).style.display = "none"; //Added 5/15/18 for Bug 26544 MLW

                    //if (occupancyCodeID !== "4" && occupancyCodeID !== "5") {
                    //    if ($("#" + hiddenSelectedCoverage).val() === "PHYSICAL DAMAGE ONLY") {
                    //        document.getElementById(pnlOwnerOtherThanInsured).style.display = "block";
                    //    }

                    //    if ($("#" + hiddenSelectedCoverage).val() === "LIABILITY ONLY") {
                    //        document.getElementById(pnlCollisionCoverage).style.display = "block";
                    //    }
                    //}
                } else {
                    // Added for task 79067 and 77972 01/25/22 BD
                    if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId.toString()) {
                        document.getElementById(coverageOpt).style.display = "block";
                        document.getElementById(coverageOptList).style.display = "block";
                    }
                    document.getElementById(propDeductiblePnl).style.display = "block";
                    document.getElementById(vehCostNewPnl).style.display = "block";

                    //Moved 5/15/18 for Bug 26544 MLW
                    if (ifm.vr.currentQuote.lobId !== IFMLOBEnum.FAR.LobId.toString()) {
                        if ($("#" + hiddenSelectedForm).val() != "ML-2" && $("#" + hiddenSelectedForm).val() != "ML-4") {
                            document.getElementById(coverageOptPDL).style.display = "block";
                            //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE AND LIABILITY");
                        }
                        else {
                            document.getElementById(coverageOptPD).style.display = "block";
                            //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                        }
                    }
                }

                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";

                break;
            case "OTHER RV":
                if (useUpdatedHOM2018CodePath === true) {
                    document.getElementById(coverageOptList).style.display = "block";
                    document.getElementById(propDeductiblePnl).style.display = "none"; //Added 5/15/18 for Bug 26544 MLW
                    document.getElementById(vehCostNewPnl).style.display = "none"; //Added 5/15/18 for Bug 26544 MLW
                } else {
                    document.getElementById(coverageOptPD).style.display = "block";
                    //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                    document.getElementById(propDeductiblePnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                    document.getElementById(vehCostNewPnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                }
                //document.getElementById(propDeductiblePnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                //document.getElementById(vehCostNewPnl).style.display = "block"; //Moved 5/15/18 for Bug 26544 MLW
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";

                if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId.toString()) {
                    coverageLiabOnlyDdlOption.remove();
                }

                break;
            case "BOAT TRAILER":
            case "SNOWMOBILE - TRAILER":
                document.getElementById(coverageOptPD).style.display = "block";
                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                document.getElementById(vehCostNewPnl).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                break;
            case "ACCESSORIES & EQUIPMENT":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptPD).style.display = "block";
                document.getElementById(propDeductiblePnl).style.display = "block";
                document.getElementById(limitLabel).style.display = "block";
                document.getElementById(vehCostNewPnl).style.display = "block";
                document.getElementById(descriptionPnl).style.display = "block";
                //$("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
                if (!ifm.vr.currentQuote.isEndorsement)
                    $("#" + descriptionText).val("Accessories and Equipment Description");
                break;
            case "JET SKIS & WAVERUNNERS":
                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                //document.getElementById(vehCostNewPnl).style.display = "block"; // Matt A 4-21-16
                document.getElementById(vehLengthPnl).style.display = "block";
                document.getElementById(motorType).style.display = "block";
                document.getElementById(motorTypeIn).style.display = "block";
                document.getElementById(bodilyInjuryPnl).style.display = "block";
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(motorPnl).style.display = "block";
                document.getElementById(horsePowerPnl).style.display = "block";

                // Check form Farm
                if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId.toString())
                    document.getElementById(under25OperatorPnl).style.display = "none";
                else
                    document.getElementById(under25OperatorPnl).style.display = "block";
                break;
            case "SNOWMOBILE - NAMED PERILS":
            case "SNOWMOBILE - SPECIAL COVERAGE":
                //if (useUpdatedHOM2018CodePath === true) {
                //    if (occupancyCodeID !== "4" && occupancyCodeID !== "5") {
                //        if ($("#" + hiddenSelectedCoverage).val() === "PHYSICAL DAMAGE ONLY") {
                //            document.getElementById(pnlOwnerOtherThanInsured).style.display = "block";
                //        }
                //    }
                //}

                document.getElementById(coverageOpt).style.display = "block";
                document.getElementById(coverageOptList).style.display = "block";
                document.getElementById(vehYearPnl).style.display = "block";
                document.getElementById(costNewLabel).style.display = "block";
                //document.getElementById(vehCostNewPnl).style.display = "block"; // Matt A 4-21-16
                document.getElementById(vehSerialNumPnl).style.display = "block";
                document.getElementById(vehMakePnl).style.display = "block";
                document.getElementById(vehModelPnl).style.display = "block";
                document.getElementById(descriptionPnl).style.display = "block";
                if (!ifm.vr.currentQuote.isEndorsement)
                    $("#" + descriptionText).val(selectedWatercraft);
                break;
        }

        if ($("#" + coverageOptPDL).is(":visible") === true) {
            $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE AND LIABILITY");
        } else if ($("#" + coverageOptPD).is(":visible") === true) {
            $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY");
        } else if ($("#" + coverageOptLib).is(":visible") === true) {
            $("#" + hiddenSelectedCoverage).val("LIABILITY ONLY");
        }

        if ($("#" + coverageOptList).is(":visible") === false) {
            SetDropDownByText(coverageOptList, $("#" + hiddenSelectedCoverage).val());
        }

        if (useUpdatedHOM2018CodePath === true) {
            // Checks css for display:[none|block], ignores visibility:[true|false]
            if ($("#" + pnlCollisionCoverage).is(":visible") === false) {
                $("#" + checkCollisionCoverage).prop("checked", false);
            }

            if ($("#" + pnlOwnerOtherThanInsured).is(":visible") === false) {
                $("#" + checkOwnerOtherThanInsured).prop("checked", false);
            }

            if ((occupancyCodeID === "4" || occupancyCodeID === "5")) {
                if (document.getElementById(coverageOptList).style.display === "block") {
                    document.getElementById(coverageOptPD).style.display = "block";
                    $('#' + coverageOptList + ' option:contains(PHYSICAL DAMAGE ONLY)').attr('selected', 'selected');
                    document.getElementById(coverageOptList).style.display = "none";
                    $("#" + hiddenSelectedCoverage).val("PHYSICAL DAMAGE ONLY"); //Added 7/10/18 for HOM2011 Upgrade post implementation changes MLW
                    document.getElementById(coverageOptList).selectedIndex = 2; //Added 7/11/18 for HOM2011 Upgrade post implementation changes MLW

                    toggleRVWaterCoverageOptions(coverageOptList, propDeductiblePnl, vehCostNewPnl, propDeductibleList, vehCostNew, hiddenSelectedCoverage, watercraftType);
                }
            }
        }

        $("#" + hiddenWatercraftType).val(selectedWatercraft.toUpperCase());
    }
    else {
        //revert the dropdown back to the value set before the user attempted to change vehicle type.
        SetDropDownByText(watercraftType, $("#" + hiddenWatercraftType).val());
    }
}

function ClearRVWaterMotorEntries(horsePower, vehYear, vehCostNew, vehSerialNum, vehMake, vehModel) {
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

function checkCollisionCoverage(collisionCheckboxId) {
    var collisionCheckbox = document.getElementById(collisionCheckboxId);
    var currentControlID = collisionCheckbox.id.split("_")[collisionCheckbox.id.split("_").length - 1]
    var txtPropertyDeductible = $('[id*=txtPropertyDeductibleTextBox_' + currentControlID + ']');
    var ddlPropertyDeductible = $('[id*=ddlPropertyDeductible_' + currentControlID + ']');

    switch ($('[id*=ddlCoverageOptions_' + currentControlID + ']').val()) {
        case "PHYSICAL DAMAGE AND LIABILITY":
            if (collisionCheckbox.checked === false) {
                document.getElementById($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id')).style.display = "block";
                document.getElementById($('[id*=lblCoverageOptionPDL_' + currentControlID + ']').attr('id')).style.display = "none";
                //Updated 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                //document.getElementById($('[id*=pnlCollisionCoverage_' + currentControlID + ']').attr('id')).style.display = "none";
                document.getElementById($('[id*=pnlCollisionCoverage_' + currentControlID + ']').attr('id')).style.display = "block";
            } else {
                //Added 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                if (collisionCheckbox.checked === true) {
                    changeRVWaterCoverageOption("PHYSICAL DAMAGE AND LIABILITY", currentControlID);
                    document.getElementById($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id')).style.display = "none";
                    document.getElementById($('[id*=lblCoverageOptionPDL_' + currentControlID + ']').attr('id')).style.display = "block";

                    if (ddlPropertyDeductible) {
                        SetDropDownByText(ddlPropertyDeductible.attr('id'), "500");
                    }
                }
            }
            break;
        case "PHYSICAL DAMAGE ONLY":
            //Added 7/5/18 for HOM2011 Upgrade post go-live changes MLW
            if (collisionCheckbox.checked === true) {
                changeRVWaterCoverageOption("PHYSICAL DAMAGE AND LIABILITY", currentControlID);
                document.getElementById($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id')).style.display = "none";
                document.getElementById($('[id*=lblCoverageOptionPDL_' + currentControlID + ']').attr('id')).style.display = "block";

                if (ddlPropertyDeductible) {
                    SetDropDownByText(ddlPropertyDeductible.attr('id'), "500");
                }
            }
            break;
        case "LIABILITY ONLY":
            if (collisionCheckbox.checked === true) {
                changeRVWaterCoverageOption("PHYSICAL DAMAGE AND LIABILITY", currentControlID);
                document.getElementById($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id')).style.display = "none";
                document.getElementById($('[id*=lblCoverageOptionPDL_' + currentControlID + ']').attr('id')).style.display = "block";

                if (ddlPropertyDeductible) {
                    SetDropDownByText(ddlPropertyDeductible.attr('id'), "500");
                }
            }
            break;
    }

    txtPropertyDeductible.val(ddlPropertyDeductible.children(':selected').text());
}

function checkOwnedBySomeoneOtherThanInsured(otherOwnerCheckboxId) {
    var otherOwnerCheckbox = document.getElementById(otherOwnerCheckboxId);
    var currentControlID = otherOwnerCheckbox.id.split("_")[otherOwnerCheckbox.id.split("_").length - 1]

    switch ($('[id*=ddlCoverageOptions_' + currentControlID + ']').val()) {
        case "PHYSICAL DAMAGE AND LIABILITY":
            if (otherOwnerCheckbox.checked === false) {
                document.getElementById($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id')).style.display = "block";
                document.getElementById($('[id*=lblCoverageOptionPDL_' + currentControlID + ']').attr('id')).style.display = "none";
                document.getElementById($('[id*=pnlOwnerOtherThanInsured_' + currentControlID + ']').attr('id')).style.display = "none";
            }
            break;
        case "PHYSICAL DAMAGE ONLY":
            if (otherOwnerCheckbox.checked === true) {
                changeRVWaterCoverageOption("PHYSICAL DAMAGE AND LIABILITY", currentControlID);
                document.getElementById($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id')).style.display = "none";
                document.getElementById($('[id*=lblCoverageOptionPDL_' + currentControlID + ']').attr('id')).style.display = "block";
            }
            break;
        case "LIABILITY ONLY":
            //nothing to do... Not a valid option.
            break;
    }
}

function changeRVWaterCoverageOption(coverageOptionText, currentControlID) {
    $('[id*=ddlCoverageOptions_' + currentControlID + ']').val(coverageOptionText);
    toggleRVWaterCoverageOptions($('[id*=ddlCoverageOptions_' + currentControlID + ']').attr('id'), $('[id*=pnlPropertyDeductible_' + currentControlID + ']').attr('id'), $('[id*=pnlVehCostNew_' + currentControlID + ']').attr('id'), $('[id*=ddlPropertyDeductible_' + currentControlID + ']').attr('id'), $('[id*=txtVehCostNew_' + currentControlID + ']').attr('id'), $('[id*=hiddenSelectedCoverage_' + currentControlID + ']').attr('id'), $('[id*=ddlVehType_' + currentControlID + ']').attr('id'));
}

function toggleRVWaterCoverageOptions(coverageOption, physicalDamagePnl, vehCostNewPnl, physicalDamageList, vehCostNew, hiddenSelectedCoverage, watercraftType) {
    var watercraftDDL = document.getElementById(watercraftType);
    var selectedWatercraft = watercraftDDL.options[watercraftDDL.selectedIndex].text;
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    var physicalDamageClient = document.getElementById(physicalDamageList);
    var selectedPD = physicalDamageClient.options[physicalDamageClient.selectedIndex].text;
    var coveragesDDL = document.getElementById(coverageOption);
    var selectedCoverageOption = coveragesDDL.options[coveragesDDL.selectedIndex].text;
    var currentControlID = coverageOption.split("_")[coverageOption.split("_").length - 1];
    var pnlOwnerOtherThanInsured = $('[id*=pnlOwnerOtherThanInsured_' + currentControlID + ']').attr('id');
    var pnlCollisionCoverage = $('[id*=pnlCollisionCoverage_' + currentControlID + ']').attr('id');
    var pnlPropertyDeductibleTextBox = $('[id*=pnlPropertyDeductibleTextBox_' + currentControlID + ']').attr('id');
    var txtPropertyDeductibleTextBox = $('[id*=txtPropertyDeductibleTextBox_' + currentControlID + ']').attr('id');
    var selectedCoverageText = $('#' + coverageOption + ' :selected').text();
    var checkCollisionCoverage = $("[id*=chkCollisionCoverage_" + currentControlID + "]").attr("id");
    var checkOwnerOtherThanInsured = $("[id*=chkOwnerOtherThanInsured_" + currentControlID + "]").attr("id");

    document.getElementById(pnlCollisionCoverage).style.display = "none";
    document.getElementById(pnlOwnerOtherThanInsured).style.display = "none";

    if ($('#' + hiddenSelectedCoverage).val() === "" && selectedCoverageText === "") {
        //Do nothing
        document.getElementById($('[id*=pnlCollisionCoverage_' + currentControlID + ']').attr('id')).style.display = "none";
        document.getElementById($('[id*=pnlOwnerOtherThanInsured_' + currentControlID + ']').attr('id')).style.display = "none";
    } else {
        if ($('#' + hiddenSelectedCoverage).val() !== selectedCoverageText) {
            //alert(selectedPD);
            //alert(selectedWatercraft);
            if ((selectedPD == "" || (selectedPD === "500" && selectedWatercraft === "GOLF CART")) && $("#" + vehCostNew).val() == "") //added (selectedPD === "500" && selectedWatercraft === "GOLF CART") to the if statement so that the built in value of 500 doesn't bring up this prompt when changing coverage options.'
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
        } else {
            confirmValue.value = "Yes";
        }

        if (confirmValue.value == "Yes") {
            document.getElementById(physicalDamagePnl).style.display = "none";
            document.getElementById(vehCostNewPnl).style.display = "none";
            document.getElementById(pnlPropertyDeductibleTextBox).style.display = "none";
            var occupancyCodeID = $('[id*=hiddenOccupancyCodeID]').first().val();

            if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "") {
                document.getElementById(physicalDamagePnl).style.display = "block";

                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") === true) {

                    if (selectedWatercraft == "GOLF CART" || selectedWatercraft == "SNOWMOBILE - NAMED PERILS" || selectedWatercraft == "SNOWMOBILE - SPECIAL COVERAGE") {
                        if (occupancyCodeID !== "4" && occupancyCodeID !== "5") {
                            //Removed 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                            //if (selectedCoverageOption === "PHYSICAL DAMAGE ONLY") {
                            //    document.getElementById(pnlOwnerOtherThanInsured).style.display = "block";
                            //}
                            if (selectedCoverageOption === "PHYSICAL DAMAGE AND LIABILITY") {
                                if (document.getElementById($('[id*=chkCollisionCoverage_' + currentControlID + ']').attr('id')).checked === true) {
                                    document.getElementById($('[id*=pnlCollisionCoverage_' + currentControlID + ']').attr('id')).style.display = "block";
                                }
                                //Removed 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                                //if (document.getElementById($('[id*=chkOwnerOtherThanInsured_' + currentControlID + ']').attr('id')).checked === true) {
                                //    document.getElementById($('[id*=pnlOwnerOtherThanInsured_' + currentControlID + ']').attr('id')).style.display = "block";
                                //}
                            }
                        }
                        if (selectedWatercraft === "GOLF CART") {
                            if (ifm.vr.currentQuote.lobId !== IFMLOBEnum.FAR.LobId.toString()) {

                                document.getElementById(physicalDamagePnl).style.display = "none";
                                SetDropDownByText(physicalDamageList, "500");
                                document.getElementById(pnlPropertyDeductibleTextBox).style.display = "block";
                            }
                            //Added 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                            if (occupancyCodeID !== "4" && occupancyCodeID !== "5") {
                                if (ifm.vr.currentQuote.lobId === "2") {
                                    document.getElementById(pnlCollisionCoverage).style.display = "block";
                                } else {
                                    document.getElementById($('[id*=chkCollisionCoverage_' + currentControlID + ']').attr('id')).checked = false;
                                    document.getElementById(pnlCollisionCoverage).style.display = "none";
                                }
                            } else {
                                document.getElementById($('[id*=chkCollisionCoverage_' + currentControlID + ']').attr('id')).checked = false;
                                document.getElementById(pnlCollisionCoverage).style.display = "none";
                            }
                        }
                    }
                }

                if (selectedWatercraft != "BOAT MOTOR ONLY")
                    document.getElementById(vehCostNewPnl).style.display = "block";
            }
            else {
                physicalDamageClient.selectedIndex = 0;
                $("#" + vehCostNew).val("");
                if (selectedWatercraft == "GOLF CART") {
                    //Updated 7/9/18 for HOM2011 Upgrade post go-live changes MLW - only show for HOM LOB
                    if (ifm.vr.currentQuote.lobId === "2" && doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") === true) {
                        document.getElementById(pnlCollisionCoverage).style.display = "block";
                    } else {
                        document.getElementById(pnlCollisionCoverage).style.display = "none";
                    }
                }
            }

            $("#" + hiddenSelectedCoverage).val(selectedCoverageOption);
        }
        else {
            //revert the dropdown back to the value set before the user attempted to change coverage type.
            SetDropDownByText(coverageOption, $("#" + hiddenSelectedCoverage).val());
        }
    }

    // Checks css for display:[none|block], ignores visibility:[true|false]
    if ($("#" + pnlCollisionCoverage).is(":visible") === false) {
        $("#" + checkCollisionCoverage).prop("checked", false);
    }

    if ($("#" + pnlOwnerOtherThanInsured).is(":visible") === false) {
        $("#" + checkOwnerOtherThanInsured).prop("checked", false);
    }

    if ($("#" + coverageOption).is(":visible") === false) {
        SetDropDownByText(coverageOption, $('#' + hiddenSelectedCoverage).val());
    }

    $('#' + txtPropertyDeductibleTextBox).val(physicalDamageClient.options[physicalDamageClient.selectedIndex].text);
}

function toggleRVWaterUnder25Message(under25OperatorPnl, under25OperatorPnlMsg) {
    if (document.getElementById(under25OperatorPnl).checked) {
        document.getElementById(under25OperatorPnlMsg).style.display = "block";
    }
    else {
        document.getElementById(under25OperatorPnlMsg).style.display = "none";
    }
}

function toggleRVWaterMotorType(motorType, motorPanel, horsePowerPnl, motorYearPnl,
    motorCostNewPnl, motorSerialNumPnl, motorMakePnl, motorModelPnl, horsePower,
    motorYear, motorCostNew, motorSerialNum, motorMake, motorModel, hiddenSelectedForm,
    horsePowerCCRSLabel, hiddenMotorType, hiddenMotorButton) {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    var motorTypeText = $('#' + motorType + ' :selected').text();

    if (motorTypeText === "" && $('#' + hiddenMotorType).val() === "") {
        //do nothing
    }
    else {
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

            if (ifm.vr.currentQuote.lobId === IFMLOBEnum.FAR.LobId)
                $("#" + horsePowerCCRSLabel).text("*Rated Speed in MPH");

            $("#" + hiddenMotorType).val(selectedMotorType.toUpperCase());

            switch (selectedMotorType.toUpperCase()) {
                case "INBOARD":
                case "INBOARD/OUTBOARD":
                    document.getElementById(motorPanel).style.display = "block";
                    document.getElementById(horsePowerPnl).style.display = "block";
                    break;
                case "OUTBOARD":
                    if (isRvWatercraftMotorAvailable) {
                        client.selectedIndex = 4;
                        var MotorMessage = "Outboard has been selected for the motor type for this watercraft. The Outboard Motor needs to be added separately from the Watercraft. Please add Boat Motor Only. The motor will only have Liability coverage if the motor is not added separately."
                        alert(MotorMessage);
                        $('#' + hiddenMotorButton).click();
                    }
                    else {
                        document.getElementById(motorPanel).style.display = "block";
                        document.getElementById(horsePowerPnl).style.display = "block";
                        document.getElementById(motorYearPnl).style.display = "block";
                        document.getElementById(motorCostNewPnl).style.display = "block";
                        document.getElementById(motorSerialNumPnl).style.display = "block";
                        document.getElementById(motorMakePnl).style.display = "block";
                        document.getElementById(motorModelPnl).style.display = "block";
                        client.selectedIndex = 4;
                    }


                    break;
            }


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
}

function updateRVWatercraftHeaderText(headerId, driverNum, firstNameId, lastNameID, yearID) {
    var firstNameText = $("#" + firstNameId).val();
    var lastNameText = $("#" + lastNameID).val();
    var yearText = $("#" + yearID).val();
    var newHeaderText = ("RV/WATERCRAFT #" + (driverNum + 1).toString() + " - " + yearText + " " + firstNameText + " " + lastNameText).toUpperCase();

    //document.getElementById(headerId).setAttribute('value', newHeaderText);
    $("#" + headerId).val(newHeaderText);

    newHeaderText = newHeaderText.toUpperCase();
    if (newHeaderText.length > 30) {
        $("#" + headerId).text(newHeaderText.substring(0, 30) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }
}

//
// Round a number up by 100
//
function RoundRVWaterValue100(number) {
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

function TabIndex_SetFocusToControl(e, myControl, controls, shiftControls) {
    var keyCode = e.keyCode || e.which;
    if (keyCode == 9) {
        if (controls) {
            controlIndex = TabIndex_GetArrayIndexOfControl(myControl, controls);
            if (controlIndex > -1) {
                if (e.shiftKey) {
                    if (shiftControls) {
                        if (controlIndex > 0) {
                            if (shiftControls[controlIndex + 1] === "True") {
                                for (var i = (controlIndex - 2); i >= 0; i -= 2) {
                                    if (TabIndex_SetFocus(e, shiftControls[i])) {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if (controlIndex < (controls.length - 1)) {
                        if (controls[controlIndex + 1] === "True") {
                            for (var i = (controlIndex + 2); i < controls.length; i += 2) {
                                if (TabIndex_SetFocus(e, controls[i])) {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

function TabIndex_SetFocus(e, control) {
    if (control) {
        control = TabIndex_CheckIDForSelector(control);
        if (control.indexOf("[") === -1) {
            if ($(control).length) {
                if (isControlVisible(control)) {
                    e.preventDefault();
                    $(control).focus();
                    //alert($(control).attr("id"));
                    return true;
                }
            }
        } else {
            var returnValue = null
            $(control).each(function () {
                if (isControlVisible(this.id)) {
                    e.preventDefault();
                    this.focus();
                    //alert(this.attr("id"));
                    returnValue = true;
                }
            });
            if (returnValue === true) {
                return true;
            }
        }
    }
    return false;
}

function TabIndex_GetArrayIndexOfControl(control, array) {
    var index = null
    for (i = 0; i < array.length; i += 2) {
        if (array[i] === control) {
            index = i;
            break;
        }
    }
    if (index === null) {
        return -1;
    } else {
        return index;
    }
}

function TabIndex_CheckIDForSelector(myString) {
    if (myString.indexOf("[") > -1) {
        return myString;
    } else {
        if (myString.indexOf("#") === 0) {
            return myString;
        } else {
            return '#' + myString;
        }
    }
}

function isControlVisible(id) {
    id = TabIndex_CheckIDForSelector(id);
    if ($(id).parents().filter(":hidden").length === 0) {
        if ($(id).css('display') !== "none") {
            //if (isAccordionActive(id)) { 
            //    return true;
            //} else {
            //    return false;
            //}
            return true;
        } else {
            return false;
        }
    }
}

function CountDescLength(description, Counter, hiddenCounter, maxLength) {
    var myElem = document.getElementById(description);
    if (typeof (maxLength) !== 'number') {
        maxLength = 250;
    }
    if (myElem) {
        var IMDesc = myElem.value;
        if (IMDesc.length <= maxLength) {
            $("#" + Counter).text(maxLength - IMDesc.length);
            $("#" + hiddenCounter).val(maxLength - IMDesc.length);
        }
        else {
            $("#" + description).text(IMDesc.substring(0, maxLength));
            $("#" + Counter).text(0);
            $("#" + hiddenCounter).val(0);
        }
    }
}

// Calculate the Scheduled Amount - used for Endorsements
function CalculateContractorEquipmentScheduledAmount() {
    var scheduledTotal = 0.0;
    $('.txtCTEQLimit').each(function () {
        scheduledTotal += $(this).val().toInt();
    });
    $('.txtCTSched').first().val((ifm.vr.stringFormating.asNumberWithCommas(scheduledTotal.toString())));
    $('.AmountRemainingAndWarningRow').first().hide();
}



function CalculateContractorEquipmentRemaining() {
    var scheduledTotal = 0.0;
    $('.txtCTEQLimit').each(function () {
        scheduledTotal += $(this).val().toInt();
    });
    var remaining = OriginalTotalContractorsScheduledEquipmentAmount - scheduledTotal;
    $('.txtCTRemain').first().val((ifm.vr.stringFormating.asNumberWithCommas(remaining.toString())));

    if (remaining != 0) {
        $('.txtCTWarning').first().show();
        //ifm.vr.ui.FlashElement(".txtCTRemain");
    }
    else {
        $('.txtCTWarning').first().hide();
    }
}

function ContractorEquipmentHeaderUpdate(itemIndex, lblHeaderId, txtLimitId, txtDescriptionId) {
    var text = 'Item #' + (itemIndex + 1).toString();
    text += ' $' + ifm.vr.stringFormating.asNumberWithCommas($('#' + txtLimitId).val());
    text += ' - ' + $('#' + txtDescriptionId).val();

    $('#' + lblHeaderId).text(text.ellipsis(42).toUpperCase());
}

function SetDropDownByText(dropDownId, textToSelect) {
    var myDDL = document.getElementById(dropDownId);
    var opts = myDDL.options.length;
    for (var i = 0; i < opts; i++) {
        if (myDDL.options[i].text == textToSelect) {
            myDDL.options[i].selected = true;
            break;
        }
    }
}

function MultistateCheckboxChanged(sender, chkState1Id, chkState2Id) {
    var chkState1 = document.getElementById(chkState1Id);
    var chkState2 = document.getElementById(chkState2Id);
    if (chkState1 && chkState2) {
        if ((sender === "State1" && chkState1.checked && chkState2.checked === false) || (sender === "State2" && chkState1.checked === false && chkState2.checked)) {
            alert("Multistate risks with UMPD coverage will be routed to Underwriting for review.");
        }
    }
}

//Added 03/19/2020 for Home Endorsements Task 38919 MLW
$('input[id*=chkUnder25Operator]').on('change', function () {
    var youngOperator = $(this);
    var isEndorsementQuote = $('[id*=hiddenIsEndorsementQuote]').val().toLowerCase();
    if (youngOperator.prop('checked') && isEndorsementQuote === 'true') {
        $('[id*=dvYoungestOperator]').show();
    } else {
        if ($('input[id*=chkUnder25Operator]' + ":checked").length > 0 && isEndorsementQuote === 'true') {
            $('[id*=dvYoungestOperator]').show();
        } else {
            $('[id*=dvYoungestOperator]').hide();
        }
    }
});

