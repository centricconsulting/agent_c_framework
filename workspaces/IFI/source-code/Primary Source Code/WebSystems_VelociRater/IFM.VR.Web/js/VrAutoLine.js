

///<reference path="vr.core.js" />
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />


function updateDriverHeaderText(headerId, driverNum, firstNameId, lastNameID, middleNameId, DriverIndex) {
    var firstNameText = $("#" + firstNameId).val();
    var middleNameText = $("#" + middleNameId).val();
    var lastNameText = $("#" + lastNameID).val();
    var newHeaderText = "Driver #" + driverNum + " - " + firstNameText + " " + lastNameText;

    var TreeText = firstNameText + " " + lastNameText;
    if (middleNameText != "") {
        newHeaderText = "Driver #" + driverNum + " - " + firstNameText + " " + middleNameText + " " + lastNameText;
        TreeText = firstNameText + " " + middleNameText + " " + lastNameText;
    }
    newHeaderText = newHeaderText.toUpperCase();
    //document.getElementById(headerId).setAttribute('value', newHeaderText);
    $("#" + headerId).val(newHeaderText);

    if (newHeaderText.length > 40) {
        $("#" + headerId).text(newHeaderText.substring(0, 40) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }

    if (TreeText.length == 1) {
        TreeText = "Driver " + driverNum.toString();
    }
    TreeText = TreeText.toUpperCase();
    $("#cphMain_ctlTreeView_rptDrivers_lblDriverDescription_" + DriverIndex).text(TreeText);
}

function updateVehicleHeaderText(headerId, driverNum, firstNameId, lastNameID, yearID) {
    var firstNameText = $("#" + firstNameId).val();
    var lastNameText = $("#" + lastNameID).val();
    var yearText = $("#" + yearID).val();
    var newHeaderText = ("Vehicle #" + (driverNum + 1).toString() + " - " + yearText + " " + firstNameText + " " + lastNameText).toUpperCase();

    //document.getElementById(headerId).setAttribute('value', newHeaderText);
    $("#" + headerId).val(newHeaderText);

    newHeaderText = newHeaderText.toUpperCase();
    if (newHeaderText.length > 40) {
        $("#" + headerId).text(newHeaderText.substring(0, 40) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }

    var TreeText = yearText + " " + firstNameText + " " + lastNameText;

    if (TreeText.length == 2) {
        TreeText = "Vehicle " + (driverNum + 1).toString();
    }
    TreeText = TreeText.toUpperCase();
    $("#cphMain_ctlTreeView_rptVehicles_lblVehicleDescription_" + driverNum.toString()).text(TreeText);
}

function diffInYearsAndDays(startDate, endDate) {
    // Copy and normalise dates
    var d0 = new Date(startDate);
    d0.setHours(12, 0, 0, 0);
    var d1 = new Date(endDate);
    d1.setHours(12, 0, 0, 0);

    // Make d0 earlier date
    // Can remember a sign here to make -ve if swapped
    if (d0 > d1) {
        var t = d0;
        d0 = d1;
        d1 = t;
    }

    // Initial estimate of years
    var dY = d1.getFullYear() - d0.getFullYear();

    // Modify start date
    d0.setYear(d0.getFullYear() + dY);

    // Adjust if required
    if (d0 > d1) {
        d0.setYear(d0.getFullYear() - 1);
        --dY;
    }

    // Get remaining difference in days
    var dD = (d1 - d0) / 8.64e7;

    // If sign required, deal with it here
    return [dY, dD];
}

function GetDriverAge(driverInputID) {
    var currentDate = new Date();
    if (ifm.vr.vrDateTime.isModernDate(effectiveDate)) {
        currentDate = effectiveDate;
    }

    var regex = /\d{1,2}\/\d{1,2}\/\d{4}/;
    if (regex.test($("#" + driverInputID).val())) {
        var _driverDateBirth = new Date($("#" + driverInputID).val());

        if (ifm.vr.vrDateTime.isModernDate(_driverDateBirth)) {
            var yearsOld = diffInYearsAndDays(currentDate, _driverDateBirth)[0];
            if (!isNaN(yearsOld)) { return yearsOld };
        }
    }
    return NaN;
}

function ToggleGoodDriverDiv(divGoodDriverID, driverInputID) {
    var driverAge = GetDriverAge(driverInputID);
    if (isNaN(driverAge) || driverAge > 24) {
        // hide
        $("#" + divGoodDriverID).hide('fast');
    }
    else {
        // show - will be check on server side before allowing
        $("#" + divGoodDriverID).show('fast');
    }
}

function ToggleAccidentCourse(courseTxtId, driverInputID) {
    //txtAccidentPreventionCourse
    var driverAge = GetDriverAge(driverInputID);
    if (isNaN(driverAge) || driverAge < 55) {
        // hide
        $("#" + courseTxtId).parent().hide('fast');
        $("#" + courseTxtId).val('');
    }
    else {
        // show - will be check on server side before allowing
        $("#" + courseTxtId).parent().show('fast');
    }
}

function DLDateCalc(txtDLDateClientID, txtBirthDateClientID) {
    if ($("#" + txtDLDateClientID).val() == '') {
        var currentDate = new Date();
        var regex = /\d{1,2}\/\d{1,2}\/\d{4}/;
        if (regex.test($("#" + txtBirthDateClientID).val())) {
            var _driverDateBirth = new Date($("#" + txtBirthDateClientID).val());

            if (ifm.vr.vrDateTime.isModernDate(_driverDateBirth)) {
                var sDate = ((_driverDateBirth.getMonth() + 1).toString() + "/" + _driverDateBirth.getDate().toString() + "/" + (_driverDateBirth.getFullYear() + 16).toString());
                _driverDateBirth = new Date(sDate);
                $("#" + txtDLDateClientID).val(_driverDateBirth.formatMMDDYYYY());
            }
        }
    }

    //$("#" + txtDLDateClientID)
    //$("#" + txtBirthDateClientID)
}

function ToggleDistantDriver(chkDistantStudentClientID, trDistanceToSchoolClientID, txtMilesClientID) {
    //$("#" + chkDistantStudentClientID).change(function(){
    var isCheck = $("#" + chkDistantStudentClientID).is(':checked');
    //$("#" + trDistanceToSchoolClientID).toggle(isCheck);
    if (!isCheck) {
        $("#" + trDistanceToSchoolClientID).hide('fast');
        $("#" + txtMilesClientID).val('');
    }
    else { $("#" + trDistanceToSchoolClientID).show('fast'); }
    //});
}

function ToggleOtherVehicleInformation(ddBodyTypeID, divOtherInfoID, ddUseID, txtActualCashID, txtStatedAmtID, txtCostNewID, ddMotorCycleTypeID, txtHorsePowerID, divDriverAssignmentID, NoMsgs, txtSymbolID, chkExtenedNonOwnedID, chkNamedNonOwnerID, HiddenLookupWasFiredID, txtVinNumberID, txtYearID, trCustomEquipment, isNewRAPALookupAvailable, isCustomEquipmentAvailable) {
    var VinNumber = $("#" + txtVinNumberID).val();
    var vehicleYear = parseInt($("#" + txtYearID).val());
    var bodyTypeId = $("#" + ddBodyTypeID + " option:selected").val();
    var costNew = $("#" + txtCostNewID).val();
    var symbolsText = $("#" + txtSymbolID).val();
    var isExtendedNonOwned = $("#" + chkExtenedNonOwnedID).prop('checked');
    var isNamedNonOwner = $("#" + chkNamedNonOwnerID).prop('checked');

    if (NoMsgs == false) {
        if ((bodyTypeId == SD_bodyTypeId_Van | bodyTypeId == SD_bodyTypeId_PICKUPWCAMPER | bodyTypeId == SD_bodyTypeId_PICKUPWOCamper) & ($("#" + ddUseID).val() == SD_vehicleUseId_Business | $("#" + ddUseID).val() == SD_vehicleUseId_Farm)) {
            alert('This combination of body type and vehicle use will require Underwriting review prior to issuance.');
        }

        if (bodyTypeId == SD_bodyTypeId_AntiqueAuto | bodyTypeId == SD_bodyTypeId_ClassicAuto) {
            alert('This body type will require Underwriting review prior to issuance.');
        }
    }

    if (bodyTypeId == SD_bodyTypeId_MotorHome | bodyTypeId == SD_bodyTypeId_MotorCycle | bodyTypeId == SD_bodyTypeId_PICKUPWCAMPER | bodyTypeId == SD_bodyTypeId_RecTrailer | bodyTypeId == SD_bodyTypeId_OtherTrailer | bodyTypeId == SD_bodyTypeId_ClassicAuto) {
        $("#" + txtCostNewID).parent().show('fast');
    }
    else {
        var symbols = symbolsText.split('/') // added 12-16-2015 Matt A - To check if the first char of either symbol is 'P'
        var sy1 = (symbols.length > 1) ? symbols[0] : ""; // added 12-16-2015 Matt A - To check if the first char of either symbol is 'P'
        var sy2 = (symbols.length > 1) ? symbols[1] : ""; // added 12-16-2015 Matt A - To check if the first char of either symbol is 'P'
        var sy3 = (symbols.length > 2) ? symbols[2] : ""; // added 12-16-2015 Matt A - To check if the first char of either symbol is 'P'

        if (isNewRAPALookupAvailable == "True") {
            if (bodyTypeId != "" && isExtendedNonOwned == false && isNamedNonOwner == false && ((costNew != "" && costNew != "$0") || symbolsText == "00/00")) {
                $("#" + txtCostNewID).parent().show('fast');
            }
            else {
                if ((VinNumber && VinNumber.length > 0 && VinNumber.length < 10) || (vehicleYear && !isNaN(vehicleYear) && vehicleYear > 0 && vehicleYear < 1981)) {
                    $("#" + txtCostNewID).parent().show('fast');
                } else {
                    $("#" + txtCostNewID).parent().hide('fast');
                }
            }
        } else {
            if (bodyTypeId != "" && isExtendedNonOwned == false && isNamedNonOwner == false && ((costNew != "" && costNew != "$0") || symbolsText == "00/00" || sy1.startsWith('P') || sy2.startsWith('P'))) {
                $("#" + txtCostNewID).parent().show('fast');
            }
            else {
                $("#" + txtCostNewID).parent().hide('fast');
            }
        }
        //if (bodyTypeId != "" && ((costNew != "" && costNew != "$0") || symbolsText == "00/00" || (sy1.startsWith('P') || sy2.startsWith('P')) && (isExtendedNonOwned == false && isNamedNonOwner == false))) {
        //    $("#" + txtCostNewID).parent().show();
        //}
        //else {
        //    $("#" + txtCostNewID).parent().hide('fast');
        //}
    }

    // this determines when cost new should be shown to offset a missing symbols
    // basically is cost new is available always show
    // if there is no symbols and vin isnot empty and Vin lookup has been invoked then show
    // do not show is named non-owned they never have symbols

    if ((costNew != "" || symbolsText == "")) // do below after logic above - this is a catch all
    {
        if ($("#" + HiddenLookupWasFiredID).val() == '1' && VinNumber != '' && (isExtendedNonOwned == false && isNamedNonOwner == false)) {
            $("#" + txtCostNewID).parent().show('fast');
        }
    }

    if (bodyTypeId == SD_bodyTypeId_RecTrailer || bodyTypeId == SD_bodyTypeId_OtherTrailer) {
        // hide driver assignment
        $("#" + divDriverAssignmentID).hide('fast');
    }
    else {
        // show driver assignment
        $("#" + divDriverAssignmentID).show('fast');
    }

    $("#" + ddMotorCycleTypeID).parent().hide('fast').prev().hide('fast');
    $("#" + txtHorsePowerID).parent().hide('fast').prev().hide('fast');

    $("#" + txtActualCashID).parent().hide('fast').prev().hide('fast');
    $("#" + txtStatedAmtID).parent().hide('fast').prev().hide('fast');

    if (bodyTypeId == SD_bodyTypeId_MotorCycle | bodyTypeId == SD_bodyTypeId_OtherTrailer | bodyTypeId == SD_bodyTypeId_AntiqueAuto | bodyTypeId == SD_bodyTypeId_ClassicAuto) { // removed classic auto 1/23/2017 Matt A Bug 8104 // removed bodyTypeId == SD_bodyTypeId_ClassicAuto 'Added back 1/14/2020 CAH B39589
        if (isCustomEquipmentAvailable != "True") { 
            $("#" + divOtherInfoID).show();
        }
        //show motorcycle fields
        if (bodyTypeId == SD_bodyTypeId_MotorCycle) {
            $("#" + ddMotorCycleTypeID).parent().show('fast').prev().show('fast');
            $("#" + txtHorsePowerID).parent().show('fast').prev().show('fast');
        }

        // show actual cash - This changed per Bug 3681, it now shows stated amount for classic auto - see below MGB
        //if (bodyTypeId == SD_bodyTypeId_ClassicAuto) {
        //    $("#" + txtActualCashID).parent().show('fast').prev().show('fast');
        //}

        // show stated amount - Added Classic Auto to the list 9/15/14 MGB Bug 3681
        // removed classic auto 1/23/2017 Matt A Bug 8104 'Added back 1/14/2020 CAH B39589
        if (bodyTypeId == SD_bodyTypeId_OtherTrailer | bodyTypeId == SD_bodyTypeId_AntiqueAuto | bodyTypeId == SD_bodyTypeId_ClassicAuto) {
            $("#" + txtStatedAmtID).parent().show('fast').prev().show('fast');
        }
    }
    else {
        if (isCustomEquipmentAvailable != "True") {
            $("#" + divOtherInfoID).hide();
        }
    }
    if (bodyTypeId == SD_bodyTypeId_OtherTrailer) {
        // hide custom equipment
        $("#" + trCustomEquipment).hide('fast');
    } else {
        // Unhide custom equipment
        $("#" + trCustomEquipment).show('fast');
    }
}

function ToggleVinLookupButton(btnVinLookupClientId, txtVinNumberClientId, txtYearClientId, txtCostNewClientId, ddBodyTypeClientId) {
    var Vin = $("#" + txtVinNumberClientId).val();
    var Year = $("#" + txtYearClientId).val();
    var costNew = $("#" + txtCostNewClientId).val();
    var bodyTypeId = $("#" + ddBodyTypeClientId + " option:selected").val();
    var showButton = false;
    var showCostNew = false;

    if (YearInRange(Year)) {
        showButton = true;
    } else {
        if (Year == '' && VinInRange(Vin)) {
            showButton = true;
        }
    }
    
    if (showButton) {
        $("#" + btnVinLookupClientId).show();
        if (costNew && costNew != "" && costNew != "$0") {
            showCostNew = true;
        } else {
            showCostNew = false;
        }
    } else {
        $("#" + btnVinLookupClientId).hide();
        if ((Vin && Vin.length > 0 && Vin.length < 10) || (Year && Year > 0 && Year < 1981)) {
            showCostNew = true;
        } else {
            showCostNew = false;
        }
    }
    if (bodyTypeId == SD_bodyTypeId_MotorHome | bodyTypeId == SD_bodyTypeId_MotorCycle | bodyTypeId == SD_bodyTypeId_PICKUPWCAMPER | bodyTypeId == SD_bodyTypeId_RecTrailer | bodyTypeId == SD_bodyTypeId_OtherTrailer | bodyTypeId == SD_bodyTypeId_ClassicAuto) {
        showCostNew = true;
    }

    if (showCostNew) {
        $("#" + txtCostNewClientId).parent().show('fast');
    } else {
        $("#" + txtCostNewClientId).parent().hide('fast');
    }
}

function VinInRange(Vin) {
    if (Vin.length >= 10 && Vin.length <= 17) {
        return true;
    } else {
        return false;
    }
}

function YearInRange(Year) {
    if (!isNaN(Year) && Year > 1980) {
        return true;
    } else {
        return false;
    }
}

function SetVinLookupFieldsFromParent(HiddenLookupWasFired, vehicleIndex) {
    //stores the clientId of the parent page's hidden field needed to repopulate back to after a search
    var parentLookupFired = $("#" + HiddenLookupWasFired).selector;
    $('[id*=HiddenParentLookupWasFired]').val(parentLookupFired);

    //sends the search values from the page to the popup
    $('[id*=hdnVehicleIndex]').val(vehicleIndex);
    var vinSearch = $('.VehicleVinNum_' + vehicleIndex).val();
    var yearSearch = $('.VehicleYear_' + vehicleIndex).val();
    var makeSearch = $('.VehicleMake_' + vehicleIndex).val();
    var modelSearch = $('.VehicleModel_' + vehicleIndex).val();
    $('[id*=hdnSearchYear]').val(yearSearch);
    $('[id*=hdnSearchMake]').val(makeSearch);
    $('[id*=hdnSearchModel]').val(modelSearch);
    $('.txtSearchVehicleVin').val(vinSearch);
    var hiddenSearchYearVal = $('[id*=hdnSearchYear]').val();
    if (YearInRange(hiddenSearchYearVal)) {
        $('.ddSearchVehicleYear').val(hiddenSearchYearVal);
        GetMakeList('btnVinLookup');
        GetModelList('btnVinLookup');
    } else {
        $('.ddSearchVehicleYear').val('');
        $('.ddSearchVehicleMake').empty();
        $('.ddSearchVehicleModel').empty();
    }

    ////use if we are going to auto-lookup with the selected vin/ymm from the parent page - this is currently not a requirement. If it is desired it can be added back in.
    ////currently works for VIN, but not YMM. It does not see the dd values for make and model to do the lookup.
    ////If it is decided to auto-lookup, will have to revisit this to get the YMM to work.
    //if (vinSearch && vinSearch.length >= 10) {
    //    $('.btnVehiclePopupLookup').click();
    //} else {
    //    if (YearInRange(hiddenSearchYearVal) && makeSearch.length > 0 && modelSearch.length > 0) {
    //        $('.btnVehiclePopupLookup').click();

    //    }
    //}
}

function GetMakeList(source) {
    //var makeVal = $('[id*=ddSearchMake]').val(); //left here in case we need to retain the previously selected make before clearing the make drop down values
    $('.ddSearchVehicleMake').empty();

    //var hiddenYear = $('[id*=hdnSearchYear]').val(); //in case we decide we need it later
    var yearVal = $('.ddSearchVehicleYear').val(); //this should always be the drop down value, not hidden value
    var hiddenMake = $('[id*=hdnSearchMake]').val(); //want this the hidden value so we can populate from parent if needed

    var selectMakeList = $('.ddSearchVehicleMake');
    selectMakeList.append($('<option>', {
        value: "",
        text: ""
    }, '</option>'));
    
    var yr = parseInt(yearVal);
    if (yr > 1980) {
        VRData.VIN.GetMakeListFromYear(yearVal, function (data) {
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var makeUpper = data[i].Make.toString().toUpperCase();
                    selectMakeList.append($('<option>', {
                        value: makeUpper,
                        text: makeUpper
                    }, '</option>'));
                }
                if (source == 'btnVinLookup') {
                    $('.ddSearchVehicleMake').val(hiddenMake.toUpperCase());
                }
                //lef this here in case we decide to keep the make selection when switching the year. Right now it clears out the makes and resets it so they have to select the make every time they change the year.
                //if (makeVal == null || makeVal == '' || makeVal.length <= 0) {
                //    $('[id*=ddSearchMake]').val(hiddenMake.toUpperCase());
                //} else {
                //    $('[id*=ddSearchMake]').val(makeVal.toUpperCase());
                //}
            }
        });
    }
    
}

function GetModelList(source) {
    var yearVal = $('.ddSearchVehicleYear').val();
    var makeVal = $('.ddSearchVehicleMake').val();
    var hiddenMake = $('[id*=hdnSearchMake]').val();
    var hiddenModel = $('[id*=hdnSearchModel]').val();
    //var modelVal = $('[id*=ddSearchModel]').val();  //left here in case we need to retain the model value when switching between makes or year before clearing the model drop down
    $('.ddSearchVehicleModel').empty();
    var yr = parseInt(yearVal);
    var selectModelList = $('.ddSearchVehicleModel');
    selectModelList.append($('<option>', {
        value: "",
        text: ""
    }, '</option>'));
    var makeToUse = makeVal;
    if (source == 'btnVinLookup') {
        makeToUse = hiddenMake;
    }
    if (yr > 1980 && makeToUse.length > 0) {
        VRData.VIN.GetModelListFromMake(yearVal, makeToUse, function (data) {
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var modelUpper = data[i].Model.toString().toUpperCase();
                    selectModelList.append($('<option>', {
                        value: modelUpper,
                        text: modelUpper
                    }, '</option>'));
                }
                if (source == 'btnVinLookup') {
                    $('.ddSearchVehicleModel').val(hiddenModel.toUpperCase());
                }
                //left this in here in case we decide to retain the model previously selected when switching the make or year
                //if (modelVal == null || modelVal == '' || modelVal.length <= 0) {
                //    $('[id*=ddSearchModel]').val(hiddenModel.toUpperCase());
                //} else {
                //    $('[id*=ddSearchModel]').val(modelVal.toUpperCase());
                //}
            }
        });
    }
}

function NoResultsFromLookup(hdnParentLookupWasFired, vehicleIndex, newVin, newYear, newMake, newModel) {
    $('#vinLookupPopupContent').dialog('close');
    $('.divSearchVehicleLookupContents').html('');
    $('.divSearchVehicleLookup').hide();

    var parentLookupFired = $("#" + hdnParentLookupWasFired).val();
    $(parentLookupFired).val('1');

    $('.VehicleVinNum_' + vehicleIndex).val(newVin);
    $('.VehicleYear_' + vehicleIndex).val(newYear);

    if (newMake && newMake != 'null') {
        $('.VehicleMake_' + vehicleIndex).val(newMake);
    } else {
        $('.VehicleMake_' + vehicleIndex).val('');
    }

    if (newModel && newModel != 'null') {
        $('.VehicleModel_' + vehicleIndex).val(newModel);
    } else {
        $('.VehicleModel_' + vehicleIndex).val('');
    }

    $('.VehiclePerformance_' + vehicleIndex + " option:contains('STANDARD')").prop('selected', true);
    $('.VehicleSymbol_' + vehicleIndex).val('');

    $('.VehicleCostNew_' + vehicleIndex).parent().show('fast');
    $('.VehicleCostNew_' + vehicleIndex).scrollTop();
    $('.VehicleCostNew_' + vehicleIndex).focus();
    var originalBackgroundColor = $('.VehicleCostNew_' + vehicleIndex).css("backgroundColor")
    $('.VehicleCostNew_' + vehicleIndex).css("backgroundColor", "red").stop().animate({ backgroundColor: originalBackgroundColor }, 1200);
}

function CopyVinYMMDataBackToParent(dataindex, vehicleIndex, isNewRAPALookupAvailable, isNewSymbolsAvailable) {
    var data = lastVinSearchResults;
    $('#vinLookupPopupContent').dialog('close');
    $('.divSearchVehicleLookupContents').html('');
    $('.divSearchVehicleLookup').hide();

    if (data.length > 0) {
        $('.VehicleVinNum_' + vehicleIndex).val(data[dataindex].Vin);
        $('.VehicleYear_' + vehicleIndex).val(data[dataindex].Year);
        $('.VehicleMake_' + vehicleIndex).val(data[dataindex].Make.toUpperCase());

        if (data[dataindex].Description.toString() != '') {
            $('.VehicleModel_' + vehicleIndex).val(data[dataindex].Description.toUpperCase());
        }
        else {
            $('.VehicleModel_' + vehicleIndex).val(data[dataindex].Model.toUpperCase());
        }

        $('.VehicleBodyType_' + vehicleIndex + " option:contains('" + data[dataindex].ISOBodyStyle.toUpperCase() + "')").prop('selected', true);
        $('.VehicleAntiTheft_' + vehicleIndex + " option:contains('" + data[dataindex].AntiTheftDescription.toUpperCase() + "')").prop('selected', true);
        $('.VehicleAirBags_' + vehicleIndex + " option:contains('" + data[dataindex].RestraintDescription.toUpperCase() + "')").prop('selected', true);
        $('.VehiclePerformance_' + vehicleIndex + " option:contains('" + data[dataindex].PerformanceTypeText.toUpperCase() + "')").prop('selected', true);

        $('.VehicleCostNew_' + vehicleIndex).parent().hide('fast');
        $('.VehicleCostNew_' + vehicleIndex).val(''); // if a result is available then cost new is not valid

        if (data[dataindex].CollisionSymbol.toString().length < 1) {
            $('.VehicleCostNew_' + vehicleIndex).parent().show('fast');
            $('.VehicleSymbol_' + vehicleIndex).val('');
         }
         else {
            if (isNewRAPALookupAvailable == "True") {
                if (data[dataindex].CollisionSymbol.toString().trim() == "00" || data[dataindex].CompSymbol.toString().trim() == "00") {
                    $('.VehicleCostNew_' + vehicleIndex).parent().show('fast');
                }
            } else {
                if (data[dataindex].CollisionSymbol.toString().trim() == "00" || data[dataindex].CollisionSymbol.toString().trim().startsWith('P') || data[dataindex].CompSymbol.toString().trim() == "00" || data[dataindex].CompSymbol.toString().trim().startsWith('P')) {
                    $('.VehicleCostNew_' + vehicleIndex).parent().show('fast');
                }
            }
            if (data[dataindex].LiabilitySymbol && data[dataindex].LiabilitySymbol != null && data[dataindex].LiabilitySymbol != '') {
                if (isNewSymbolsAvailable == "True") {
                    $('.VehicleSymbol_' + vehicleIndex).val(data[dataindex].CompSymbol.toString().trim() + '/' + data[dataindex].CollisionSymbol.toString().trim() + '/' + data[dataindex].LiabilitySymbol.toString().trim() + '/' + data[dataindex].BodilyInjurySymbol.toString().trim() + '/' + data[dataindex].PropertyDamageSymbol.toString().trim() + '/' + data[dataindex].MedPaySymbol.toString().trim());

                } else {
                    $('.VehicleSymbol_' + vehicleIndex).val(data[dataindex].CompSymbol.toString().trim() + '/' + data[dataindex].CollisionSymbol.toString().trim() + '/' + data[dataindex].LiabilitySymbol.toString().trim());
                }
                //$('.VehicleSymbol_' + vehicleIndex).val(data[dataindex].CompSymbol.toString().trim() + '/' + data[dataindex].CollisionSymbol.toString().trim() + '/' + data[dataindex].LiabilitySymbol.toString().trim());
            }
            else {
                $('.VehicleSymbol_' + vehicleIndex).val(data[dataindex].CollisionSymbol.toString().trim() + '/' + data[dataindex].CompSymbol.toString().trim());
            }
        }

    } else {
        $('.VehiclePerformance_' + vehicleIndex + " option:contains('STANDARD')").prop('selected', true);
        $('.VehicleSymbol_' + vehicleIndex).val('');
        $('.VehicleCostNew_' + vehicleIndex).parent().show('fast');
        $('.VehicleCostNew_' + vehicleIndex).focus();
    }
}

var isSearchingVin = false;
var lastVinSearchResults = null;
function SetupVinYMMSearch(VinClientId, MakeClientID, ModelClientId, YearClientId, SenderID, divVinLookupID, divVinLookupContent, hdnParentLookupWasFired, hdnVehicleIndexClientId, VersionId, PolicyId, PolicyImageNum, VehicleNum, isNewBusiness, effectiveDate, isNewRAPALookupAvailable, isNewSymbolsAvailable) {
    if (isSearchingVin == false) {
        var vehicleIndex = $("#" + hdnVehicleIndexClientId).val();
        var Vin = $("#" + VinClientId).val();
        var Make = $("#" + MakeClientID).val();
        var Model = $("#" + ModelClientId).val();
        var Year = $("#" + YearClientId).val();

        if (Vin.length >= 0 || (Make.length > 0 && Model.length > 0 && Year.length == 4 && SenderID != VinClientId)) {
            isSearchingVin = true;
            // agencyID will still be checked against session data to confirm that the current has access to the agency
            VRData.VIN.GetFromVinOrMakeModelYear(Vin, Make, Model, Year, VersionId, PolicyId, PolicyImageNum, VehicleNum, isNewBusiness, effectiveDate, function (data) { //Updated 07/22/2021 to include VersionId MLW
                lastVinSearchResults = data;
                isSearchingVin = false;

                if (data.length > 0) {
                    var lookupHTML = "";
                    lookupHTML += "<table style='border-collapse: collapse;'>";
                    lookupHTML += "<tr>";
                    lookupHTML += "<th>";
                    lookupHTML += "";
                    lookupHTML += "</th>";

                    //lookupHTML += "<th style='text-align: right;'>";
                    //lookupHTML += "Year";
                    //lookupHTML += "</th>";

                    //lookupHTML += "<th style='text-align: right;'>";
                    //lookupHTML += "Make";
                    //lookupHTML += "</th>";

                    lookupHTML += "<th style='text-align: right;'>";
                    lookupHTML += "VIN";
                    lookupHTML += "</th>";

                    lookupHTML += "<th style='text-align: right; width:200px'>";
                    lookupHTML += "Model";
                    lookupHTML += "</th>";

                    lookupHTML += "<th style='text-align: right;'>";
                    lookupHTML += "Body Style";
                    lookupHTML += "</th>";

                    lookupHTML += "<th style='text-align: right;'>";
                    lookupHTML += "Engine";
                    lookupHTML += "</th>";

                    //9/23/2024 - Removed symbols from results in Jira task WS-2987, parent WS-2963
                    //lookupHTML += "<th style='text-align: right;'>";
                    //lookupHTML += "Symbol";
                    //lookupHTML += "</th>";
                    lookupHTML += "</tr>";
                    for (var i = 0; i < data.length; i++) {

                        if (data[i].Model == 'FORTWO' && data[i].Make == 'UNDETERMINED') { data[i].Make = 'SMART'; }

                        var onClickCode = "CopyVinYMMDataBackToParent(" + i.toString() + ", \"" + vehicleIndex + "\", \"" + isNewRAPALookupAvailable + "\", \"" + isNewSymbolsAvailable + "\");"
                        if ((i % 2) == 0) {
                            lookupHTML += "<tr onmouseover='$(this).css(\"font-weight\",\"bold\")' onmouseout='$(this).css(\"font-weight\",\"normal\")' style='border-bottom: solid 1px black;background-color: white;cursor: pointer; font-size: 11px; ' class='clickable' onclick='" + onClickCode + "' title='Click to apply this result.'>"
                        }
                        else {
                            lookupHTML += "<tr onmouseover='$(this).css(\"font-weight\",\"bold\")' onmouseout='$(this).css(\"font-weight\",\"normal\")' style='border-bottom: solid 1px black;cursor: pointer; font-size: 11px;' class='clickable' onclick='" + onClickCode + "' title='Click to apply this result.'>"
                        }

                        lookupHTML += "<td style='width: 70px; '>"
                        //lookupHTML += "<span style='margin-left: 10px;'><b>Select</b></span>"
                        lookupHTML += "<span style='margin-left: 10px;'><input id='btnVin" + i.toString() + "' type='button' class='StandardButton' value='Select' /></span>"
                        lookupHTML += "</td>"

                        //lookupHTML += "<td style='width: 50px;text-align: right;'>"
                        //lookupHTML += data[i].Year.toString();
                        //lookupHTML += "</td>"

                        //lookupHTML += "<td style='width: 100px;text-align: right;'>"
                        //lookupHTML += data[i].Make.toUpperCase().toString();
                        //lookupHTML += "</td>"

                        lookupHTML += "<td style='width: 100px;text-align: right;'>"
                        lookupHTML += data[i].Vin.toString();
                        lookupHTML += "</td>"

                        lookupHTML += "<td style='width: 100px;text-align: right;'>"
                        //lookupHTML += data[i].Model.toUpperCase().toString();
                        if (data[i].Description.toUpperCase().toString() != '') {
                            lookupHTML += data[i].Description.toUpperCase().toString();
                        }
                        else {
                            lookupHTML += data[i].Model.toUpperCase().toString();
                        }
                        lookupHTML += "</td>"

                        lookupHTML += "<td style='width: 100px;text-align: right;'>"
                        lookupHTML += data[i].ISOBodyStyle.toString();
                        lookupHTML += "</td>"

                        lookupHTML += "<td style='width: 100px;text-align: right;'>"
                        lookupHTML += data[i].CyclinderDescription.toString();
                        lookupHTML += "</td>"

                        //9/23/2024 - Removed symbols from results in Jira task WS-2987, parent WS-2963
                        //lookupHTML += "<td style='width: 80px; text-align: right;'>"
                        ////lookupHTML += data[i].CollisionSymbol.toString() + "/" + data[i].CompSymbol.toString();         
                        //if (data[i].LiabilitySymbol && data[i].LiabilitySymbol !== "")
                        //    lookupHTML += data[i].CompSymbol.toString() + "/" + data[i].CollisionSymbol.toString() + "/" + data[i].LiabilitySymbol.toString();
                        //else
                        //    lookupHTML += data[i].CompSymbol.toString() + "/" + data[i].CollisionSymbol.toString();

                        //lookupHTML += "</td>"

                        lookupHTML += "</tr>"
                    }
                    lookupHTML += "</table>";
                    $("#" + divVinLookupContent).css("color", "black")
                    $("#" + divVinLookupContent).html(lookupHTML);
                    $("#" + divVinLookupID).show();
                    $("#" + SenderID).parent().find("input").first().focus(); // put focus on button
                }
                else {
                    // no data
                    //$("#" + divVinLookupContent).html("No results from the lookup available. Check VIN or Year/Make/Model or enter a <span style='cursor: pointer;text-decoration:underline;' onclick='ifm.vr.ui.FlashFocusThenScrollToElement(\"" + txtCostNewId + "\");'>cost new</span>.");
                    $("#" + divVinLookupContent).html("No results from the lookup available. Check VIN or Year/Make/Model or enter a <span style='cursor: pointer;text-decoration:underline;' onclick='NoResultsFromLookup(\"" + hdnParentLookupWasFired + "\",\"" + vehicleIndex + "\",\"" + Vin + "\",\"" + Year + "\",\"" + Make + "\",\"" + Model + "\");'>cost new</span>.");
                    $("#" + divVinLookupID).show();
                }

                return data;
            });
        }
    }
}
//updated 07/22/2021 to send VersionId for lookup MLW - versioning added with CAP Endorsements - Updated 10/18/2022 for task  75263 MLW policyId through isNewBusiness
function SetupVinSearch(VinClientId, MakeClientID, ModelClientId, YearClientId, SenderID, ddBodyTypeClientId, ddAntiTheftId, ddRestraints, divVinLookupID, divVinLookupContent, ddPerformanceId, txtSymbolId, txtCostNewId, VersionId, PolicyId, PolicyImageNum, VehicleNum, isNewBusiness, effectiveDate, isNewRAPALookupAvailable) {
    if (isSearchingVin == false) {
        var Vin = $("#" + VinClientId).val();
        var Make = $("#" + MakeClientID).val();
        var Model = $("#" + ModelClientId).val();
        var Year = $("#" + YearClientId).val();

        // or make sure 'Make',model and year are not empty
        //if ((Vin.length >= 0 && SenderID == VinClientId) || (Make.length > 0 && Model.length > 0 && Year.length == 4 && SenderID != VinClientId)) {
        if (Vin.length >= 0 || (Make.length > 0 && Model.length > 0 && Year.length == 4 && SenderID != VinClientId)) {
            //if (SenderID == VinClientId || SenderID == MakeClientID || SenderID == ModelClientId || SenderID == YearClientId) {
            //    $("#" + SenderID).after('<span>VIN Lookup...</span>');
            //}

            isSearchingVin = true;
            // agencyID will still be checked against session data to confirm that the current has access to the agency
            VRData.VIN.GetFromVinOrMakeModelYear(Vin, Make, Model, Year, VersionId, PolicyId, PolicyImageNum, VehicleNum, isNewBusiness, effectiveDate, function (data) { //Updated 07/22/2021 to include VersionId MLW
                lastVinSearchResults = data;
                isSearchingVin = false;

                //if (SenderID == VinClientId || SenderID == MakeClientID || SenderID == ModelClientId || SenderID == YearClientId) {
                //    $("#" + SenderID).next().remove();
                //}

                if (SenderID == VinClientId || SenderID == MakeClientID || SenderID == ModelClientId || SenderID == YearClientId) {
                    if (data.length > 0) {
                        var lookupHTML = "";
                        lookupHTML += "<table style='border-collapse: collapse;'>";
                        lookupHTML += "<tr>";
                        lookupHTML += "<th>";
                        lookupHTML += "";
                        lookupHTML += "</th>";

                        //lookupHTML += "<th style='text-align: right;'>";
                        //lookupHTML += "Year";
                        //lookupHTML += "</th>";

                        //lookupHTML += "<th style='text-align: right;'>";
                        //lookupHTML += "Make";
                        //lookupHTML += "</th>";

                        lookupHTML += "<th style='text-align: right;'>";
                        lookupHTML += "VIN";
                        lookupHTML += "</th>";

                        lookupHTML += "<th style='text-align: right; width:200px'>";
                        lookupHTML += "Model";
                        lookupHTML += "</th>";

                        lookupHTML += "<th style='text-align: right;'>";
                        lookupHTML += "Body Style";
                        lookupHTML += "</th>";

                        lookupHTML += "<th style='text-align: right;'>";
                        lookupHTML += "Engine";
                        lookupHTML += "</th>";

                        lookupHTML += "<th style='text-align: right;'>";
                        lookupHTML += "Symbol";
                        lookupHTML += "</th>";
                        lookupHTML += "</tr>";
                        for (var i = 0; i < data.length; i++) {

                            if (data[i].Model == 'FORTWO' && data[i].Make == 'UNDETERMINED') { data[i].Make = 'SMART'; }


                            var onClickCode = "SetVinData(" + i.toString() + ", \"" + VinClientId.toString() + "\", \"" + MakeClientID.toString() + "\", \"" + ModelClientId.toString() + "\", \"" + YearClientId.toString() + "\", \"" + SenderID.toString() + "\",\"" + ddBodyTypeClientId.toString() + "\", \"" + ddAntiTheftId + "\", \"" + ddRestraints.toString() + "\", \"" + divVinLookupID.toString() + "\", \"" + divVinLookupContent.toString() + "\", \"" + ddPerformanceId.toString() + "\", \"" + txtSymbolId.toString() + "\", \"" + txtCostNewId.toString() + "\", \"" + isNewRAPALookupAvailable + "\"); $(\"#" + SenderID + "\").parent().find(\"input\").first().focus();";
                            if ((i % 2) == 0) {
                                lookupHTML += "<tr onmouseover='$(this).css(\"font-weight\",\"bold\")' onmouseout='$(this).css(\"font-weight\",\"normal\")' style='border-bottom: solid 1px black;background-color: white;cursor: pointer; font-size: 11px; ' class='clickable' onclick='" + onClickCode + "' title='Click to apply this result.'>"
                            }
                            else {
                                lookupHTML += "<tr onmouseover='$(this).css(\"font-weight\",\"bold\")' onmouseout='$(this).css(\"font-weight\",\"normal\")' style='border-bottom: solid 1px black;cursor: pointer; font-size: 11px;' class='clickable' onclick='" + onClickCode + "' title='Click to apply this result.'>"
                            }

                            lookupHTML += "<td style='width: 70px; '>"
                            //lookupHTML += "<span style='margin-left: 10px;'><b>Select</b></span>"
                            lookupHTML += "<span style='margin-left: 10px;'><input id='btnVin" + i.toString() + "' type='button' class='StandardButton' value='Select' /></span>"
                            lookupHTML += "</td>"

                            //lookupHTML += "<td style='width: 50px;text-align: right;'>"
                            //lookupHTML += data[i].Year.toString();
                            //lookupHTML += "</td>"

                            //lookupHTML += "<td style='width: 100px;text-align: right;'>"
                            //lookupHTML += data[i].Make.toUpperCase().toString();
                            //lookupHTML += "</td>"

                            lookupHTML += "<td style='width: 100px;text-align: right;'>"
                            lookupHTML += data[i].Vin.toString();
                            lookupHTML += "</td>"

                            lookupHTML += "<td style='width: 100px;text-align: right;'>"
                            //lookupHTML += data[i].Model.toUpperCase().toString();
                            if (data[i].Description.toUpperCase().toString() != '') {
                                lookupHTML += data[i].Description.toUpperCase().toString();
                            }
                            else {
                                lookupHTML += data[i].Model.toUpperCase().toString();
                            }
                            lookupHTML += "</td>"

                            lookupHTML += "<td style='width: 100px;text-align: right;'>"
                            lookupHTML += data[i].ISOBodyStyle.toString();
                            lookupHTML += "</td>"

                            lookupHTML += "<td style='width: 100px;text-align: right;'>"
                            lookupHTML += data[i].CyclinderDescription.toString();
                            lookupHTML += "</td>"


                            lookupHTML += "<td style='width: 80px; text-align: right;'>"
                            //lookupHTML += data[i].CollisionSymbol.toString() + "/" + data[i].CompSymbol.toString();         
                            if (data[i].LiabilitySymbol && data[i].LiabilitySymbol !== "")
                                lookupHTML += data[i].CompSymbol.toString() + "/" + data[i].CollisionSymbol.toString() + "/" + data[i].LiabilitySymbol.toString();
                            else
                                lookupHTML += data[i].CompSymbol.toString() + "/" + data[i].CollisionSymbol.toString();

                            lookupHTML += "</td>"

                            lookupHTML += "</tr>"
                        }
                        lookupHTML += "</table>";
                        $("#" + divVinLookupContent).css("color", "black")
                        $("#" + divVinLookupContent).html(lookupHTML);
                        $("#" + divVinLookupID).show();
                        $("#" + SenderID).parent().find("input").first().focus(); // put focus on button
                    }
                    else {
                        // no data
                        $("#" + divVinLookupContent).html("No results from the lookup available. Check VIN or Year/Make/Model or enter a <span style='cursor: pointer;text-decoration:underline;' onclick='ifm.vr.ui.FlashFocusThenScrollToElement(\"" + txtCostNewId + "\");'>cost new</span>.");
                        $("#" + divVinLookupID).show();

                        $("#" + ddPerformanceId + " option:contains('STANDARD')").prop('selected', true);
                        $("#" + txtSymbolId).val('');
                        $("#" + txtCostNewId).parent().show('fast');
                        $("#" + txtCostNewId).focus();
                        //$("#" + SenderID).parent().find("input").first().focus(); // put focus on button
                    }
                }
                else {
                    // not doing grid based display
                    $("#" + divVinLookupContent).html('');
                    $("#" + divVinLookupID).hide();
                    //if (data.length > 0)
                    //{
                    // has data- This is a vin lookup only not a year/make/model
                    // use first result
                    SetVinData(0, VinClientId.toString(), MakeClientID.toString(), ModelClientId.toString(), YearClientId.toString(), SenderID.toString(), ddBodyTypeClientId.toString(), ddAntiTheftId, ddRestraints.toString(), divVinLookupID.toString(), divVinLookupContent.toString(), ddPerformanceId.toString(), txtSymbolId.toString(), txtCostNewId, isNewRAPALookupAvailable);
                    //}
                }

                return data;
            });
        }
    }
}

function SetVinData(dataindex, VinClientId, MakeClientID, ModelClientId, YearClientId, SenderID, ddBodyTypeClientId, ddAntiTheftId, ddRestraints, divVinLookupID, divVinLookupContent, ddPerformanceId, txtSymbolId, txtCostNewId, isNewRAPALookupAvailable) {
    var data = lastVinSearchResults;

    $("#" + divVinLookupContent).html("");
    $("#" + divVinLookupID).hide();

    if (data.length > 0) {
        if ($("#" + VinClientId).val() == '' && SenderID != VinClientId) // don't want to overwrite vin because of latency
        { $("#" + VinClientId).val(data[dataindex].Vin); }

        $("#" + MakeClientID).val(data[dataindex].Make.toUpperCase());
        if (data[dataindex].Description.toString() != '') {
            $("#" + ModelClientId).val(data[dataindex].Description.toUpperCase());
        }
        else {
            $("#" + ModelClientId).val(data[dataindex].Model.toUpperCase());
        }
        $("#" + YearClientId).val(data[dataindex].Year);
        //$("#" + ddBodyTypeClientId).val(data[0].BodyTypeId); the return id does not map to diamond ??
        $("#" + ddBodyTypeClientId + " option:contains('" + data[dataindex].ISOBodyStyle.toUpperCase() + "')").prop('selected', true);
        $("#" + ddAntiTheftId + " option:contains('" + data[dataindex].AntiTheftDescription.toUpperCase() + "')").prop('selected', true);

        // not mapping correctly
        $("#" + ddRestraints + " option:contains('" + data[dataindex].RestraintDescription.toUpperCase() + "')").prop('selected', true);

        $("#" + ddPerformanceId + " option:contains('" + data[dataindex].PerformanceTypeText.toUpperCase() + "')").prop('selected', true);

        $("#" + txtCostNewId).parent().hide('fast');
        $("#" + txtCostNewId).val(''); // if a result is available then cost new is not valid

        if (data[dataindex].CollisionSymbol.toString().length < 1) {
            $("#" + txtCostNewId).parent().show('fast');
            $("#" + txtSymbolId).val('');
        }
        else {
            if (isNewRAPALookupAvailable == "True") {
                if (data[dataindex].CollisionSymbol.toString().trim() == "00" || data[dataindex].CompSymbol.toString().trim() == "00") {
                    $("#" + txtCostNewId).parent().show('fast');
                }
            } else {
                if (data[dataindex].CollisionSymbol.toString().trim() == "00" || data[dataindex].CollisionSymbol.toString().trim().startsWith('P') || data[dataindex].CompSymbol.toString().trim() == "00" || data[dataindex].CompSymbol.toString().trim().startsWith('P')) {
                    $("#" + txtCostNewId).parent().show('fast');
                }
            }         
            //if (data[dataindex].CollisionSymbol.toString().trim() == "00" || data[dataindex].CollisionSymbol.toString().trim().startsWith('P') || data[dataindex].CompSymbol.toString().trim() == "00" || data[dataindex].CompSymbol.toString().trim().startsWith('P')) {
            //    $("#" + txtCostNewId).parent().show('fast');
            //}
            if (data[dataindex].LiabilitySymbol && data[dataindex].LiabilitySymbol != null && data[dataindex].LiabilitySymbol != '')
                $("#" + txtSymbolId).val(data[dataindex].CompSymbol.toString().trim() + '/' + data[dataindex].CollisionSymbol.toString().trim() + '/' + data[dataindex].LiabilitySymbol.toString().trim());
            else
                $("#" + txtSymbolId).val(data[dataindex].CollisionSymbol.toString().trim() + '/' + data[dataindex].CompSymbol.toString().trim());
        }
    }
    else {
        $("#" + ddPerformanceId + " option:contains('STANDARD')").prop('selected', true);
        $("#" + txtSymbolId).val('');
        $("#" + txtCostNewId).parent().show('fast');
    }
}

function VinConfirm(txtVinId, divResults) {
    var html = "";
    if ($("#" + txtVinId).val().indexOf('&') == -1 || $("#" + txtVinId).val().length < 4) {
        var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Vin=' + encodeURIComponent($("#" + txtVinId).val()) + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate);
        genHandler += '&rnd=' + encodeURIComponent(Math.random().toString().replace('.', '')); // rnd eliminates browser caching
        $.getJSON(genHandler, {
            dataType: "json",
            data: "",
            format: "json"
        })
            .done(function (data) {
                if (data.length > 0) {
                    html += "<table>";

                    html += "<tr>";
                    html += "<td style=\"width: 130px;text-align:right;\">Year:";
                    html += "</td>";
                    html += "<td><span style=\"margin-left: 12px;\">" + data[0].Year + "</span>";
                    html += "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td style=\"text-align:right;\">Make:";
                    html += "</td>";
                    html += "<td><span style=\"margin-left: 12px;\">" + data[0].Make.toUpperCase() + "</span>";
                    html += "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td style=\"text-align:right;\">Model:";
                    html += "</td>";
                    if (data[0].Description.toUpperCase() != '') {
                        html += "<td><span style=\"margin-left: 12px;\">" + data[0].Description.toUpperCase() + "</span>";
                    }
                    else {
                        html += "<td><span style=\"margin-left: 12px;\">" + data[0].Model.toUpperCase() + "</span>";
                    }
                    html += "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td style=\"text-align:right;\">Symbols:";
                    html += "</td>";
                    // swapped 6-9-14
                    if (data[i].LiabilitySymbol && data[i].LiabilitySymbol !== "")
                        html += "<td> <span style=\"margin-left: 12px;\">" + data[0].CompSymbol.toString() + "/" + data[0].CollisionSymbol.toString() + "/" + data[0].LiabilitySymbol.toString() + "</span.";
                    else
                        html += "<td> <span style=\"margin-left: 12px;\">" + data[0].CompSymbol.toString() + "/" + data[0].CollisionSymbol.toString() + "</span.";


                    html += "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td style=\"text-align:right;\">Performance Level:";
                    html += "</td>";
                    html += "<td><span style=\"margin-left: 12px;\">" + data[0].PerformanceTypeText + "</span>";
                    html += "</td>";
                    html += "</tr>";

                    html += "</table>";
                }
                $("#" + divResults).html(html);
                return data;
            });
    }
    else {
        $("#" + divResults).html(html);
        alert('Invalid VIN number.');
    }
}

function TabLogicTxtDefensiveDriver(event, txtAccidentPreventionCourseID, h3GoodStudentID, h3MotorCycleID) {
    if (event.which || event.keyCode) {
        if ((event.which == 9) || (event.keyCode == 9)) {
            if ($("#" + txtAccidentPreventionCourseID).is(':visible')) {
                $("#" + txtAccidentPreventionCourseID).focus();
                return false;
            }
            else {
                if ($("#" + h3GoodStudentID).is(':visible')) {
                    $("#" + h3GoodStudentID).focus();
                    return false;
                }
                else {
                    if ($("#" + h3MotorCycleID).is(':visible')) {
                        $("#" + h3MotorCycleID).focus();
                        return false;
                    }
                }
            }
        }
    }
    else { return true };
}

function TabLogicDDUse(event, txtCostNewID, chkNamedNonOwnerID) {
    if (event.which || event.keyCode) {
        if ((event.which == 9) || (event.keyCode == 9)) {
            if ($("#" + txtCostNewID).is(':visible')) {
                var ctrlId = "#" + txtCostNewID;
                // need the timeout
                setTimeout('$("' + ctrlId + '").focus()', 50);
                return false;
            }
            else {
                if ($("#" + chkNamedNonOwnerID).is(':visible')) {
                    $("#" + chkNamedNonOwnerID).focus();
                    return false;
                }
            }
        }
    }
    else { return true };
}

function SetUpModelAutoComplete(TxtModelID, TxtMakeId, TxtYearId) {
    $("#" + TxtModelID).autocomplete({
        minLength: 1, delay: 300
    });

    // you have to set source everytime there is about to be a search because you need the most current model text
    $("#" + TxtModelID).on("autocompletesearch", function (event, ui) {
        $("#" + TxtModelID).autocomplete({ source: 'GenHandlers/Vr_Pers/MakeModelLookup.ashx?GetModels=' + $("#" + TxtMakeId).val() + '&Year=' + $("#" + TxtYearId).val() });
    });
}

function DoScheduledItemsRemainingMath(arrayOfAmts_Ids, originalSum, txtRemainingId) {
    var currentSumOfItems = 0;

    for (var ii = 0; ii < arrayOfAmts_Ids.length; ii++) {
        //Check that the array item still exists on the page before computing
        if ($("#" + arrayOfAmts_Ids[parseInt(ii)]).length) {
            var valFromTxt = $("#" + arrayOfAmts_Ids[parseInt(ii)]).val().replace('$', '').replace(',', '');
            var val = parseInt(valFromTxt);
            if (!isNaN(val)) { currentSumOfItems += val; }
        }
    }

    var remaining = originalSum - currentSumOfItems;
    $("#" + txtRemainingId).val(FormatAsCurrency((remaining).toString()));
    if (remaining != 0) {
        $("#" + txtRemainingId).css('color', 'red');
        $("#" + txtRemainingId).next().remove();
        $("#" + txtRemainingId).after('<span style="color:red;"><br/>Equipment Amount is different than the Additional Scheduled Amount.<br/> Ensure Equipment Amount is correct</span>');
    }
    else {
        $("#" + txtRemainingId).css('color', 'black');
        $("#" + txtRemainingId).next().remove();
    }
}

function DriverCopyObject(firstname, middlename, lastname, suffix, gender, birthdate) {
    this.firstname = firstname;
    this.middlename = middlename;
    this.lastname = lastname;
    this.suffix = suffix;
    this.gender = gender;
    this.birthdate = birthdate;
}

var driverCopyOpen = false;
function GetDriverCopyDropDown(btnId) {
    if (driverCopyOpen == false) {
        driverCopyOpen = true;
        var html = "<select onblur='driverCopyOpen = false;$(this).remove();' onchange='DoDRiverCopyToPolicyHolder($(this));'>";
        html += "<option value = ''>-- Select Driver--</option>";
        for (var ii = 0; ii < drivers_copy.length; ii++) {
            html += "<option value = '" + ii.toString() + "'>Driver #";
            html += (ii + 1).toString() + " - ";
            var d = drivers_copy[ii];
            html += d.firstname + " ";
            html += d.lastname + " ";
            html += d.suffix;
            html += "</option>";
        }
        html += "</select>";
        $("#" + btnId).after(html);
    }
}

function DoDRiverCopyToPolicyHolder(sender) {
    driverCopyOpen = false;
    var driverIndex = parseInt($(sender).val());
    //if ($(this).val() != \"\"){DoDRiverCopyToPolicyHolder(parseInt($(this).val()));$(this).remove();}
    var d = drivers_copy[driverIndex];

    $("#" + ph2FirstNameId).val(d.firstname);
    $("#" + ph2MiddleNameId).val(d.middlename);
    $("#" + ph2LastNameId).val(d.lastname);
    $("#" + ph2SuffixId).val(d.suffix);
    $("#" + ph2GenderId).val(d.gender);
    $("#" + ph2BirthDateId).val(d.birthdate);
    $(sender).remove();
    ifm.vr.ui.LockTree_Freeze();
}

function GaragedCopyObject(streetnumId, streetnameId, aptnumberId, cityId, stateId, zipId, countyId) {
    this.streetnameId = streetnameId;
    this.streetnumId = streetnumId;
    this.aptnumberId = aptnumberId;
    this.cityId = cityId;
    this.stateId = stateId;
    this.zipId = zipId;
    this.countyId = countyId;
}

function GetGaragingDropDown(sender, vehicleIndex) {
    var n = !$(sender).next().is('select');
    if (n == undefined || n) { // if next element is 'select' then dropdown already open
        var html = "<select onblur='$(this).remove();' onchange='if($(this).val() != \"\"){DoGarageAddressCopy(parseInt($(this).val())," + vehicleIndex.toString() + "); $(this).remove();}'>";
        html += "<option value=''>--  Copy from Vehicle Below --</option>"
        for (var ii = 0; ii < garaged_copy.length; ii++) {
            var item = garaged_copy[ii];
            var optionText = ($("#" + item.streetnumId).val() + " " + $("#" + item.streetnameId).val() + " " + $("#" + item.zipId).val()).trim();
            if (ii == vehicleIndex || optionText == '') {
                html += "<option value='" + ii.toString() + "' disabled='disabled'>Vehicle #" + (ii + 1).toString() + " - " + $("#" + item.streetnumId).val() + " " + $("#" + item.streetnameId).val() + " " + $("#" + item.zipId).val() + "</option>"
            }
            else {
                html += "<option value='" + ii.toString() + "'>Vehicle #" + (ii + 1).toString() + " - " + $("#" + item.streetnumId).val() + " " + $("#" + item.streetnameId).val() + " " + $("#" + item.zipId).val() + "</option>"
            }
        }
        html += "</select>";
        $(sender).after(html);
    }
}

function DoGarageAddressCopy(fromvehicleIndex, tovehicleIndex) {
    var from = garaged_copy[fromvehicleIndex];
    var to = garaged_copy[tovehicleIndex];

    $("#" + to.streetnameId).val($("#" + from.streetnameId).val());
    $("#" + to.streetnumId).val($("#" + from.streetnumId).val());
    $("#" + to.aptnumberId).val($("#" + from.aptnumberId).val());
    $("#" + to.cityId).val($("#" + from.cityId).val());
    $("#" + to.stateId).val($("#" + from.stateId).val());
    $("#" + to.zipId).val($("#" + from.zipId).val());
    $("#" + to.countyId).val($("#" + from.countyId).val());
    ifm.vr.ui.LockTree_Freeze();
}

var vehicle_Makes = ["Acura", "Aston Martin", "Audi", "Bentley", "BMW", "Cadillac", "Chevrolet",
    "Chrysler", "Dodge", "Ferrari", "Fiat", "Ford", "GMC", "Honda", "Hyundai", "Infiniti",
    "Jaguar", "Jeep", "Kia", "Lamborghini", "Land Rover", "Lexus", "Lincoln", "Lotus", "Maserati",
    "Mazda", "Mercedes-Benz", "Mini", "Mitsubishi", "Nissan", "Porsche", "Rolls-Royce", "Scion", "Subaru",
    "Suzuki", "Toyota", "Volkswagen", "Volvo", "Pontiac"];

var topUs_Models = ["Silverado", "Camry", "RAM", "Accord", "Civic", "Altima", "CR-V", "Fusion", "Corolla", "Escape", "RAV4",
    "Equinox", "Cruze", "Elantra", "Sonata", "Prius", "Focus", "Grand Cherokee", "Malibu", "Sierra", "Impala", "Forester", "Explorer",
    "3 Series", "Soul", "Tacoma", "Jetta", "Wrangler", "Sentra", "Highlander", "Town & Country", "Optima", "Rogue", "Econoline", "Tundra",
    "Santa fe", "Grand Caravan", "RX", "Odyssey", "Passat", "Outback", "Sienna", "Edge", "M Class", "Sorento", "E Class", "Versa",
    "Pilot", "C Class", "3", "Acadia", "Tahoe", "Maxima", "Charger", "Traverse", "Terrain", "Pathfinder", "Dart", "CX-5", "ES", "MDX",
    "Frontier", "Avalon", "Express", "Murano", "200", "Durango", "Mustang", "XV Crosstrek", "Impreza", "Suburban", "Journey", "Patriot",
    "Camaro", "5 Series", "Avenger", "SRX", "Sonic", "Sportage", "Enclave", "4Runner", "Fiesta", "Accent", "Captiva", "X5", "Forte",
    "Taurus", "Fit", "300", "X3", "IS", "RDX", "Q5", "A4", "Compass", "Expedition", "Juke", "ATS"];

//used prior to state expansion - no longer used? Not sure if this is used anywhere else, so keeping for now MLW
function ToggleLiabilityType(ddlType, dvR1C1, dvR1C2, dvR2C1, dvR2C2, dvR3C2, ddlBI, ddlPD, ddlSSL, ddlMP, ddlUMSSL, ddlUMBI, ddlUMPD, ddlUMPDDeduct,
    chkMultiPolicy, chkMarketCredit, chkAutoEnhance, autoEnhancement, dvMarketCredit, childDriver, p1OverForty, p2OverForty) {
    var clientBI = document.getElementById(ddlBI);
    var clientPD = document.getElementById(ddlPD);
    var clientSSL = document.getElementById(ddlSSL);
    var clientMP = document.getElementById(ddlMP);
    var clientUMSSL = document.getElementById(ddlUMSSL);
    var clientUMBI = document.getElementById(ddlUMBI);
    var clientUMPD = document.getElementById(ddlUMPD);
    var clientUMPDDeduct = document.getElementById(ddlUMPDDeduct);

    if (ddlType.value != "1") {
        document.getElementById(dvR1C1).style.display = "block";
        clientBI.selectedIndex = 4;
        clientPD.selectedIndex = 5;
        document.getElementById(dvR1C2).style.display = "none";
        clientSSL.selectedIndex = 0;
        document.getElementById(dvR2C1).style.display = "none";
        clientUMSSL.selectedIndex = 0;
        document.getElementById(dvR2C2).style.display = "block";
        clientUMBI.selectedIndex = 3;
        document.getElementById(dvR3C2).style.display = "block";
        clientUMPD.selectedIndex = 4;
    }
    else {
        document.getElementById(dvR1C1).style.display = "none";
        clientBI.selectedIndex = 0;
        clientPD.selectedIndex = 0;
        document.getElementById(dvR1C2).style.display = "block";
        clientSSL.selectedIndex = 5;
        document.getElementById(dvR2C1).style.display = "block";
        clientUMSSL.selectedIndex = 5;
        document.getElementById(dvR2C2).style.display = "none";
        clientUMBI.selectedIndex = 0;
        document.getElementById(dvR3C2).style.display = "none";
        clientUMPD.selectedIndex = 0;
    }

    clientMP.selectedIndex = 4;
    clientUMPDDeduct.selectedIndex = 2;

    //if (childDriver == "False") {
    //    if (p1OverForty == "True")
    //        document.getElementById(dvMarketCredit).style.display = "block";
    //    else {
    //        if (p2OverForty == "True")
    //            document.getElementById(dvMarketCredit).style.display = "block";
    //        else {
    //            document.getElementById(chkMarketCredit).checked = false;
    //            document.getElementById(dvMarketCredit).style.display = "none";
    //        }
    //    }
    //}
    //else {
    //    document.getElementById(chkMarketCredit).checked = false;
    //    document.getElementById(dvMarketCredit).style.display = "none";
    //}
}

//Added 9/26/18 for Multi State MLW - replaces ToggleLiabilityType above
function ToggleLiabilityTypeStateExpansion(ddlType, quoteState, dvR1C1, dvR1C2, dvR1C2SE, dvR2C1, dvR2C2, dvR2C2SE, dvR3C2, dvR4C2, ddlBI, ddlPD, ddlSSL, ddlMP, ddlUMSSL, ddlUMCSL, txtUIMCSL, ddlUMBI, ddlUMBISE, txtUIMBI, ddlUMPD, ddlUMPDDeduct,
    chkMultiPolicy, perslimitTextPPASPLIT, perslimitTextPPACSL, chkMarketCredit, chkAutoEnhance, autoEnhancement, dvMarketCredit, childDriver, p1OverForty, p2OverForty) {
    var clientBI = document.getElementById(ddlBI);
    var clientPD = document.getElementById(ddlPD);
    var clientSSL = document.getElementById(ddlSSL);
    var clientMP = document.getElementById(ddlMP);
    var clientUMSSL = document.getElementById(ddlUMSSL);
    var clientUMCSL = document.getElementById(ddlUMCSL);
    var clientUIMCSL = document.getElementById(txtUIMCSL);
    var clientUMBI = document.getElementById(ddlUMBI);
    var clientUMBISE = document.getElementById(ddlUMBISE);
    var clientUIMBI = document.getElementById(txtUIMBI);
    var clientUMPD = document.getElementById(ddlUMPD);
    var clientUMPDDeduct = document.getElementById(ddlUMPDDeduct);

    if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
        if (ddlType.value != "1") {
            document.getElementById(dvR1C1).style.display = "block";
            clientBI.selectedIndex = 4;
            document.getElementById(dvR1C2).style.display = "none";
            clientSSL.selectedIndex = 0;
            document.getElementById(dvR1C2SE).style.display = "none";
            clientUMCSL.selectedIndex = 0;
            $("#" + txtUIMCSL).val('');
            document.getElementById(dvR2C1).style.display = "none";
            clientUMSSL.selectedIndex = 0;
            document.getElementById(dvR2C2).style.display = "none";
            clientUMBI.selectedIndex = 0;
            document.getElementById(dvR2C2SE).style.display = "block";
            document.getElementById(perslimitTextPPACSL).style.display = "none";
            if (ifm.vr.currentQuote.isEndorsement != true) {
                document.getElementById(perslimitTextPPASPLIT).style.display = "block";
            }
            //Updated 1/18/2022 for OH task 66101 MLW
            if (quoteState == "OH") {
                clientPD.selectedIndex = 3;
                clientUMBISE.selectedIndex = 3; //100/300 is default, copied to textbox
                $("#" + txtUIMBI).val(document.getElementById(ddlUMBISE).options[document.getElementById(ddlUMBISE).selectedIndex].text);
            } else {
                clientPD.selectedIndex = 3; //Updated 4/4/2022 for bug 68773 MLW
                clientUMBISE.value = 4; //N/A is default, copied to textbox
                $("#" + txtUIMBI).val(clientUMBISE.options[clientUMBISE.selectedIndex].text);
            }

            //clientUMBISE.selectedIndex = 0; //N/A is default, copied to textbox
            //$("#" + txtUIMBI).val('N/A');
            ////$("#" + txtUIMBI).val(document.getElementById(ddlUMBI).options[document.getElementById(ddlUMBI).selectedIndex].text);   
            document.getElementById(dvR3C2).style.display = "none";
            clientUMPD.selectedIndex = 0;
            clientUMPDDeduct.selectedIndex = 0;
        }
        else {
            document.getElementById(dvR1C1).style.display = "none";
            clientBI.selectedIndex = 0;
            clientPD.selectedIndex = 0;
            document.getElementById(dvR1C2).style.display = "none";
            clientSSL.selectedIndex = 5;
            document.getElementById(dvR1C2SE).style.display = "block";
            clientUMCSL.selectedIndex = 5; //500,000 is default, copied to textbox
            $("#" + txtUIMCSL).val(document.getElementById(ddlUMCSL).options[document.getElementById(ddlUMCSL).selectedIndex].text);
            document.getElementById(dvR2C1).style.display = "block";
            clientUMSSL.selectedIndex = 5;
            document.getElementById(dvR2C2).style.display = "none";
            clientUMBI.selectedIndex = 0;
            document.getElementById(dvR2C2SE).style.display = "none";
            clientUMBISE.selectedIndex = 0;
            $("#" + txtUIMBI).val('');
            document.getElementById(dvR3C2).style.display = "none";
            clientUMPD.selectedIndex = 0;
            clientUMPDDeduct.selectedIndex = 0; //should this be 0? was 2
            if (ifm.vr.currentQuote.isEndorsement != true) {
                document.getElementById(perslimitTextPPACSL).style.display = "block";
            }
            document.getElementById(perslimitTextPPASPLIT).style.display = "none";
        }

        clientMP.selectedIndex = 4;
        clientUIMCSL.disabled = "True";
        clientUIMBI.disabled = "True";
        document.getElementById(dvR4C2).style.display = "none";
        clientUMPDDeduct.selectedIndex = 0;
    }
    else {

        if (ddlType.value != "1") {
            document.getElementById(dvR1C1).style.display = "block";
            clientBI.selectedIndex = 4;
            clientPD.selectedIndex = 3; //Updated 4/4/2022 for bug 68773 MLW
            document.getElementById(dvR1C2).style.display = "none";
            clientSSL.selectedIndex = 0;
            document.getElementById(dvR2C1).style.display = "none";
            clientUMSSL.selectedIndex = 0;
            document.getElementById(dvR2C2).style.display = "block";
            clientUMBI.selectedIndex = 3;
            document.getElementById(dvR3C2).style.display = "block";
            clientUMPD.selectedIndex = 3; //Updated 4/4/2022 for bug 68773 MLW
            document.getElementById(perslimitTextPPACSL).style.display = "none";
            if (ifm.vr.currentQuote.isEndorsement != true) {
                document.getElementById(perslimitTextPPASPLIT).style.display = "block";
            }
        }
        else {
            document.getElementById(dvR1C1).style.display = "none";
            clientBI.selectedIndex = 0;
            clientPD.selectedIndex = 0;
            document.getElementById(dvR1C2).style.display = "block";
            clientSSL.selectedIndex = 5;
            document.getElementById(dvR2C1).style.display = "block";
            clientUMSSL.selectedIndex = 5;
            document.getElementById(dvR2C2).style.display = "none";
            clientUMBI.selectedIndex = 0;
            document.getElementById(dvR3C2).style.display = "none";
            clientUMPD.selectedIndex = 0;
            if (ifm.vr.currentQuote.isEndorsement != true) {
                document.getElementById(perslimitTextPPACSL).style.display = "block";
            }
            document.getElementById(perslimitTextPPASPLIT).style.display = "none";
        }

        clientMP.selectedIndex = 4;
        clientUMPDDeduct.selectedIndex = 2;
        document.getElementById(dvR1C2SE).style.display = "none";
        clientUMCSL.selectedIndex = 0;
        $("#" + txtUIMCSL).val('');
        clientUIMCSL.disabled = "True";
        document.getElementById(dvR2C2SE).style.display = "none";
        clientUMBISE.selectedIndex = 0;
        $("#" + txtUIMBI).val('');
        clientUIMBI.disabled = "True";
        document.getElementById(dvR4C2).style.display = "block";

        //if (childDriver == "False") {
        //    if (p1OverForty == "True")
        //        document.getElementById(dvMarketCredit).style.display = "block";
        //    else {
        //        if (p2OverForty == "True")
        //            document.getElementById(dvMarketCredit).style.display = "block";
        //        else {
        //            document.getElementById(chkMarketCredit).checked = false;
        //            document.getElementById(dvMarketCredit).style.display = "none";
        //        }
        //    }
        //}
        //else {
        //    document.getElementById(chkMarketCredit).checked = false;
        //    document.getElementById(dvMarketCredit).style.display = "none";
        //}
    }
}

//Added 9/26/18 for multi state MLW
function ToggleUMCSL(ddUMCSL, txtUIMCSL, quoteState) {
    var clientUMCSL = document.getElementById(ddUMCSL);
    //Added 9/12/22 to default UM BI CSL to N/A when CSL UM BI is 50k
    if ((quoteState == "IL" || quoteState == "OH") && (clientUMCSL.value == '0')) {
        $("#" + txtUIMCSL).val("N/A")
    } else {
        $("#" + txtUIMCSL).val(clientUMCSL.options[clientUMCSL.selectedIndex].text);
    }
}


//Added 9/26/18 for multi state MLW
function ToggleUMBI(ddUMBI, txtUIMBI, quoteState) {
    var clientUMBI = document.getElementById(ddUMBI);
    if ((quoteState == "IL" || quoteState == "OH") && (clientUMBI.value == '0')) { 
        //Added 9/12/22 to default SL UIM BI to N/A when UM is 25/50k
        $("#" + txtUIMBI).val("N/A")
    } else {
        $("#" + txtUIMBI).val(clientUMBI.options[clientUMBI.selectedIndex].text);
    }
}

//Added 10/8/18 for multi state MLW
function ToggleRelatedHomeFarm(chkAutoHomeDiscount_Parachute, tr_RelatedHomeFarm, txtMoreInfo) {
    var clientAutoHomeDiscount = document.getElementById(chkAutoHomeDiscount_Parachute);
    var clientTRRelatedHomeFarm = document.getElementById(tr_RelatedHomeFarm);
    var clientQuotePolicyInfo = document.getElementById(txtMoreInfo);

    if (clientAutoHomeDiscount.checked === true) {
        clientTRRelatedHomeFarm.style.display = "block";
    }
    else {
        clientTRRelatedHomeFarm.style.display = "none";
    }
}

//Updated 9/27/18 for multi state MLW - added quoteStaet, dvUMPD, chkUMPD & txtUMPDLimit
//8/26/19 for bug 32399 ZTS - added CostNew
function ToggleVehiclePolicy(ddPolicy, quoteState, dvComp, dvColl, dvTowing, dvTransportation, dvTravel, dvRadio, dvAVEquip, dvMedia, dvCostNew, dvLoanLease, dvUMPD, dvPollution, dvMotorcycle, dvDisclaim,
    ddComp, ddColl, ddTowing, ddTransportation, chkTravel, ddRadio, ddAVEquip, ddMedia, chkLoanLease, chkUMPD, txtUMPDLimit, chkPollution, txtMotorEquip, vehicleYear, vehicleType, vehicleUse,
    nameNonOwned, hiddenVehiclePlan, dvCompDisclaim, chkAutoPlusEnhancementClientId, chkAutoEnhanceClientId, collisionAndUMPDAvail, UMPDLimitsAvail, ddUMPDClientId, trUMPDLimitTBClientId, trUMPDLimitDDClientId, trUMPDLimitMsgClientId, IsTransportationAvailable) {
    var splitLimit = "0";
    var csl = "1";
    var liabilityOnly = "1";
    var compOnly = "2";
    var pickupwCamperType = "39";
    var pickupwoCamperType = "40";
    var recTrailerType = "19";
    var otherTrailerType = "20";
    var motorCycleType = "42";
    var antiqueAuto = "22";
    var classicAuto = "24";
    var farmUsage = "4";

    var currentDate = new Date();
    var productionYear = (currentDate.getMonth() + 1) >= 10 ? currentDate.getFullYear() + 1 : currentDate.getFullYear();
    var vehicleProdYear = 0;
    var clientComp = document.getElementById(ddComp);
    var clientColl = document.getElementById(ddColl);
    var clientTowing = document.getElementById(ddTowing);
    var clientTransporation = document.getElementById(ddTransportation);
    var clientRadio = document.getElementById(ddRadio);
    var clientAVEquip = document.getElementById(ddAVEquip);
    var clientMedia = document.getElementById(ddMedia);
    var clientUMPD = document.getElementById(chkUMPD); //Added 9/27/18 for multi state MLW
    var clientUMPDLimit = document.getElementById(txtUMPDLimit); //Added 9/27/18 for multi state MLW
    var clientAutoPlusEnhance = document.getElementById(chkAutoPlusEnhancementClientId);
    var clientAutoEnhance = document.getElementById(chkAutoEnhanceClientId);
    var effDate = new Date($("[id*='hdnOriginalEffectiveDate']").val());
    var AutoPlusEnhancementEffectiveDate = document.getElementById("hiddenAutoPlusEnhancementEffectiveDate");
    var AutoPlusStartDate = new Date($("[id*='hiddenAutoPlusEnhancementEffectiveDate']").val());

    if (vehicleYear != null) {
        if (vehicleYear.length > 0) {
            if (!isNaN(vehicleYear))
                vehicleProdYear = vehicleYear;
        }
    }

    document.getElementById(dvCompDisclaim).style.display = "none";
    $("#" + hiddenVehiclePlan).val(ddPolicy.value);
    if (ddPolicy.value == liabilityOnly) {
        document.getElementById(dvComp).style.display = "none";
        document.getElementById(dvColl).style.display = "none";
        document.getElementById(dvTowing).style.display = "none";
        document.getElementById(dvTransportation).style.display = "none";
        document.getElementById(dvTravel).style.display = "none";
        document.getElementById(dvRadio).style.display = "none";
        document.getElementById(dvAVEquip).style.display = "none";
        document.getElementById(dvMedia).style.display = "none";
        document.getElementById(dvCostNew).style.display = "none";
        document.getElementById(dvLoanLease).style.display = "none";
        document.getElementById(dvUMPD).style.display = "none"; //Added 9/27/18 for multi state MLW
        document.getElementById(dvPollution).style.display = "none";
        document.getElementById(dvMotorcycle).style.display = "none";
        clientComp.selectedIndex = 0;
        clientColl.selectedIndex = 0;
        clientTowing.selectedIndex = 0;
        clientTransporation.selectedIndex = 0;
        clientRadio.selectedIndex = 0;
        clientAVEquip.selectedIndex = 0;
        clientMedia.selectedIndex = 0;
        if (document.getElementById(vehicleType).value == motorCycleType) {
            $("#" + txtMotorEquip).val("0");
        }
        //Updated 12/17/18 for multi state bug 30381       
        if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
            document.getElementById(dvUMPD).style.display = "block";
            document.getElementById(chkUMPD).disabled = false;
            if (document.getElementById(chkUMPD).checked == true) {
                document.getElementById(chkUMPD).checked = true;
                SetUMPDDefaultLimit(quoteState, txtUMPDLimit, UMPDLimitsAvail, ddUMPDClientId)
                if (UMPDLimitsAvail == "True") {
                    document.getElementById(trUMPDLimitDDClientId).style.display = "block";
                    document.getElementById(trUMPDLimitMsgClientId).style.display = "block";
                }
            } else {
                document.getElementById(chkUMPD).checked = false;
                if (UMPDLimitsAvail == "True") {
                    document.getElementById(ddUMPDClientId).value = "";
                    document.getElementById(trUMPDLimitDDClientId).style.display = "none";
                    document.getElementById(trUMPDLimitMsgClientId).style.display = "none";
                } else {
                    $("#" + txtUMPDLimit).val("");
                }               
            }
        } else {
            document.getElementById(chkUMPD).checked = false; //Added 9/27/18 for multi state MLW
            $("#" + txtUMPDLimit).val(''); //Added 9/27/18 for multi state MLW
        }
        document.getElementById(chkTravel).checked = false;
        document.getElementById(chkLoanLease).checked = false;
        document.getElementById(chkPollution).checked = false;
    }
    else {
        if (ddPolicy.value != compOnly) {
            // Check for Farm Vehicle
            if ((document.getElementById(vehicleType).value == pickupwCamperType || document.getElementById(vehicleType).value == pickupwoCamperType) && document.getElementById(vehicleUse).value == farmUsage)
                document.getElementById(dvPollution).style.display = "block";
            else {
                document.getElementById(dvPollution).style.display = "none";
                document.getElementById(chkPollution).checked = false;
            }

            // Check for Motorcycle
            if (document.getElementById(vehicleType).value != motorCycleType) {
                document.getElementById(dvComp).style.display = "block";
                document.getElementById(dvColl).style.display = "block";
                document.getElementById(dvTowing).style.display = "block";
                if ([recTrailerType, otherTrailerType, antiqueAuto, classicAuto].indexOf(document.getElementById(vehicleType).value) > -1) // TFS Bug 20129 - 6-13-17
                    document.getElementById(dvTransportation).style.display = "none";
                else
                    document.getElementById(dvTransportation).style.display = "block";
                document.getElementById(dvTravel).style.display = "none";
                document.getElementById(dvRadio).style.display = "block";
                document.getElementById(dvAVEquip).style.display = "block";
                document.getElementById(dvMedia).style.display = "block";
                document.getElementById(dvCostNew).style.display = "block";
                document.getElementById(dvMotorcycle).style.display = "none";
                clientComp.selectedIndex = 4;
                clientColl.selectedIndex = 2;
                clientTransporation.disabled = false;
                clientTowing.selectedIndex = 1;
                if (IsTransportationAvailable == "False") {
                    clientTransporation.selectedIndex = 1;
                } else {
                    clientTransporation.selectedIndex = 0;
                }
                clientRadio.selectedIndex = 0;
                clientAVEquip.selectedIndex = 0;
                clientMedia.selectedIndex = 0;

                if (nameNonOwned == "True") {
                    document.getElementById(dvRadio).style.display = "none";
                    document.getElementById(dvAVEquip).style.display = "none";
                    document.getElementById(dvTransportation).style.display = "none";

                    clientRadio.selectedIndex = 0;
                    clientAVEquip.selectedIndex = 0;
                    clientTransporation.selectedIndex = 0;
                }
                else {
                    // Check for Trailer
                    if (document.getElementById(vehicleType).value == recTrailerType || document.getElementById(vehicleType).value == otherTrailerType) {
                        document.getElementById(dvDisclaim).style.display = "block";
                        document.getElementById(dvRadio).style.display = "none";
                        document.getElementById(dvAVEquip).style.display = "none";
                        document.getElementById(dvTransportation).style.display = "none";
                        document.getElementById(dvTowing).style.display = "none";
                        document.getElementById(dvLoanLease).style.display = "none";
                        document.getElementById(dvMedia).style.display = "none";
                        //document.getElementById(dvCostNew).style.display = "none";

                        clientTowing.selectedIndex = 0;
                        clientTransporation.selectedIndex = 0;
                        clientRadio.selectedIndex = 0;
                        clientAVEquip.selectedIndex = 0;
                        clientMedia.selectedIndex = 0;
                        document.getElementById(chkLoanLease).checked = false;
                    }
                    else {
                        if ((productionYear - vehicleProdYear) < 5) {
                            document.getElementById(dvLoanLease).style.display = "block";
                            document.getElementById(chkLoanLease).checked = true;
                        }
                        else {
                            document.getElementById(dvLoanLease).style.display = "none";
                            document.getElementById(chkLoanLease).checked = false;
                        }

                        document.getElementById(dvDisclaim).style.display = "none";
                    }
                }
            }
            else {
                document.getElementById(dvComp).style.display = "block";
                document.getElementById(dvColl).style.display = "block";
                document.getElementById(dvTowing).style.display = "block";
                document.getElementById(dvTravel).style.display = "block";
                document.getElementById(dvRadio).style.display = "none";
                document.getElementById(dvTransportation).style.display = "none";
                document.getElementById(dvLoanLease).style.display = "none";
                document.getElementById(chkLoanLease).checked = false;
                document.getElementById(dvMedia).style.display = "block";
                document.getElementById(dvCostNew).style.display = "block";
                document.getElementById(dvMotorcycle).style.display = "block";
                document.getElementById(dvAVEquip).style.display = "none";

                clientComp.selectedIndex = 4;
                clientColl.selectedIndex = 2;
                clientTransporation.disabled = false;
                clientTowing.selectedIndex = 1;
                clientRadio.selectedIndex = 0;
                clientTransporation.selectedIndex = 0;
                clientMedia.selectedIndex = 0;
                $("#" + txtMotorEquip).val("0");
                clientAVEquip.selectedIndex = 0;
                document.getElementById(chkTravel).checked = true;
            }
            //Added 9/27/18 for multi state MLW
            if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
                document.getElementById(dvUMPD).style.display = "block";
                if (clientColl.selectedIndex == 0) {
                    if (document.getElementById(chkUMPD).checked == true) {
                        document.getElementById(chkUMPD).checked = true;
                        document.getElementById(chkUMPD).disabled = false;
                        SetUMPDDefaultLimit(quoteState, txtUMPDLimit, UMPDLimitsAvail, ddUMPDClientId)
                        switch (quoteState) {
                            case "IL":
                                if (collisionAndUMPDAvail == "True") {
                                    clientColl.disabled = false;
                                } else {
                                    clientColl.disabled = true;
                                }
                                if (UMPDLimitsAvail == "True") {
                                    document.getElementById(trUMPDLimitDDClientId).style.display = "block";
                                    document.getElementById(trUMPDLimitMsgClientId).style.display = "block";
                                }
                                break;
                            default:
                                clientColl.disabled = true;
                                break;
                        }
                    } else {
                        document.getElementById(chkUMPD).checked = false;
                        document.getElementById(chkUMPD).disabled = false;
                        if (UMPDLimitsAvail == "True") {
                            document.getElementById(ddUMPDClientId).value = "";
                            document.getElementById(trUMPDLimitDDClientId).style.display = "none";
                            document.getElementById(trUMPDLimitMsgClientId).style.display = "none";
                        } else {
                            $("#" + txtUMPDLimit).val("");
                        }
                        clientColl.disabled = false;
                    }
                } else {
                    if (quoteState == "IL" && collisionAndUMPDAvail == "True") {
                        document.getElementById(chkUMPD).disabled = false;
                    } else {
                        document.getElementById(chkUMPD).checked = false;
                        document.getElementById(chkUMPD).disabled = true;
                        $("#" + txtUMPDLimit).val("");
                    }
                    clientColl.disabled = false;
                }
            } else {
                document.getElementById(dvUMPD).style.display = "none";
                document.getElementById(chkUMPD).checked = false;
                $("#" + txtUMPDLimit).val("");
                clientColl.disabled = false;
            }

        }
        else {
            document.getElementById(dvComp).style.display = "block";
            document.getElementById(dvColl).style.display = "none";
            document.getElementById(dvTowing).style.display = "none";
            document.getElementById(dvTransportation).style.display = "none";
            document.getElementById(dvTravel).style.display = "none";
            document.getElementById(dvRadio).style.display = "none";
            document.getElementById(dvAVEquip).style.display = "none";
            document.getElementById(dvMedia).style.display = "none";
            document.getElementById(dvCostNew).style.display = "none";
            document.getElementById(dvLoanLease).style.display = "none";
            document.getElementById(dvUMPD).style.display = "none"; //Added 9/27/18 for multi state MLW
            document.getElementById(dvPollution).style.display = "none";
            document.getElementById(dvMotorcycle).style.display = "none";
            clientComp.selectedIndex = 4;
            clientColl.selectedIndex = 0;
            clientTowing.selectedIndex = 0;
            clientTransporation.selectedIndex = 0;
            document.getElementById(chkUMPD).checked = false; //Added 9/27/18 for multi state MLW
            if (UMPDLimitsAvail == "True") {
                document.getElementById(ddUMPDClientId).value = "";
                document.getElementById(trUMPDLimitDDClientId).style.display = "none";
                document.getElementById(trUMPDLimitMsgClientId).style.display = "none";
            } else {
                $("#" + txtUMPDLimit).val(""); //Added 9/27/18 for multi state MLW
            }
            if (clientAutoPlusEnhance) {
                if (clientAutoPlusEnhance.checked === true) {
                    if (IsTransportationAvailable == "False") {
                        clientTransporation.selectedIndex = 2;
                    } else {
                        clientTransporation.selectedIndex = 1;
                    }
                }
            }
            clientRadio.selectedIndex = 0;
            clientAVEquip.selectedIndex = 0;
            clientMedia.selectedIndex = 0;
            document.getElementById(chkTravel).checked = false;
            document.getElementById(chkLoanLease).checked = false;
            document.getElementById(chkPollution).checked = false;
            document.getElementById(dvCompDisclaim).style.display = "block";
        }
    }
}

//Updated 9/27/18 for multi state MLW - added ddlUMCSLStateExpansion and ddlUMBIStateExpansion
function PopulateVehiclePolicyCoverage(callingControl, quoteState, vehIdx, dvPolicyCoverage, hiddenVehCovPlan, bodyTypeID,
    namedNonOwned, nonOwned, dvAutoEnhance, chkAutoEnhance, ddlBI, ddlPD, ddlSLL, ddlMedPay, ddlUMSSL, ddlUMCSLStateExpansion, txtUIMCSL, ddlUMBI, ddlUMBIStateExpansion, txtUIMBI, ddlUMPD,
    ddlUMPDDeduct, ddlLiabilityType, biValue, pdValue, sllValue, medPayValue, umSLLValue, umCSLSEValue, umBIValue, umBISEValue, umPDValue, umPDDValue, chkAutoPlusEnhanceClientId, IsTransportationAvailable) {
    var splitLimitType = "0"
    var CSLType = "1"
    var fullCoverage = "0";
    var liabilityOnly = "1";
    var compOnly = "2";
    var recTrailerType = "19";
    var otherTrailerType = "20";
    var AutoPlusStartDate = new Date($("[id*='hiddenAutoPlusEnhancementEffectiveDate']").val());
    var effDate = new Date($("[id*='hdnOriginalEffectiveDate']").val());

    var clientBI = document.getElementById(ddlBI);
    var clientPD = document.getElementById(ddlPD);
    var clientSLL = document.getElementById(ddlSLL);
    var clientMedPay = document.getElementById(ddlMedPay);
    var clientUMSSL = document.getElementById(ddlUMSSL);
    var clientUMCSLStateExpansion = document.getElementById(ddlUMCSLStateExpansion); //Added 9/27/18 for multi state MLW
    var clientUMBI = document.getElementById(ddlUMBI);
    var clientUMBIStateExpansion = document.getElementById(ddlUMBIStateExpansion); //Added 9/27/18 for multi state MLW
    var clientUMPD = document.getElementById(ddlUMPD);
    var clientUMPDDeduct = document.getElementById(ddlUMPDDeduct);
    var clientLiabType = document.getElementById(ddlLiabilityType);
    var liabTypeSelectedValue = clientLiabType.options[clientLiabType.selectedIndex].value;

    var vehCoveragePlanList = document.getElementById(hiddenVehCovPlan).value.split("|");
    var jnx = 0;

    if (callingControl.value != undefined) {
        for (idx = 0; idx < vehCoveragePlanList.length; idx++) {
            jnx = idx;
            if (parseInt(vehCoveragePlanList[idx][0]) == parseInt(vehIdx)) {
                var vehCoveragePlan = vehCoveragePlanList[idx].split("!");
                vehCoveragePlan[1] = callingControl.value;
                var newVehCoveragePlan = vehCoveragePlan.join("!");
                vehCoveragePlan = newVehCoveragePlan;
                vehCoveragePlanList[idx] = vehCoveragePlan;
            }
        }
    }

    $("#" + hiddenVehCovPlan).val(vehCoveragePlanList.join("|"));

    // if records has  not been saved yet, then this will be blank
    if (vehCoveragePlanList[jnx][0] != undefined) {
        // Check to see if body type is a trailer
        if (bodyTypeID != recTrailerType && bodyTypeID != otherTrailerType) {
            try {
                // Not a trailer
                if (vehCoveragePlanList[vehIdx].split("!")[1] != compOnly) {
                    // Not a parked car
                    //document.getElementById(dvPolicyCoverage).style.display = "block";
                    //document.getElementById(dvCompOnly).style.display = "none";

                    // Set Default Values
                    if (biValue == "0" && sllValue == "0") {
                        if (liabTypeSelectedValue != CSLType) {
                            clientBI.selectedIndex = 4;
                            clientPD.selectedIndex = 5;
                            clientSLL.selectedIndex = 0;
                            //Updated 9/27/18 for multi state MLW
                            if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
                                clientUMCSLStateExpansion.selectedIndex = 0; //copied to textbox
                                $("#" + txtUIMCSL).val(document.getElementById(ddlUMCSLStateExpansion).options[document.getElementById(ddlUMCSLStateExpansion).selectedIndex].text);
                                if (quoteState == "OH") {
                                    clientUMBIStateExpansion.selectedIndex = 3; //100/300 is default, copied to textbox
                                } else {
                                    clientUMBIStateExpansion.selectedIndex = 0; //N/A is default, copied to textbox
                                }
                                //clientUMBIStateExpansion.selectedIndex = 0; //N/A is default, copied to textbox
                                $("#" + txtUIMBI).val(document.getElementById(ddlUMBIStateExpansion).options[document.getElementById(ddlUMBIStateExpansion).selectedIndex].text);
                                clientUMPD.selectedIndex = 0;
                            } else {
                                clientUMSSL.selectedIndex = 0;
                                clientUMBI.selectedIndex = 3;
                                clientUMPD.selectedIndex = 4;
                            }
                        }
                        else {
                            clientBI.selectedIndex = 0;
                            clientPD.selectedIndex = 0;
                            clientSLL.selectedIndex = 5;
                            //Updated 9/28/18 for multi state MLW
                            if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
                                clientUMCSLStateExpansion.selectedIndex = 5; //500,000 is default, copied to textbox
                                $("#" + txtUIMCSL).val(document.getElementById(ddlUMCSLStateExpansion).options[document.getElementById(ddlUMCSLStateExpansion).selectedIndex].text);
                                clientUMBIStateExpansion.selectedIndex = 0; //copied to textbox
                                $("#" + txtUIMBI).val(document.getElementById(ddlUMBIStateExpansion).options[document.getElementById(ddlUMBIStateExpansion).selectedIndex].text);
                                clientUMPD.selectedIndex = 0;
                            } else {
                                clientUMSSL.selectedIndex = 5;
                                clientUMBI.selectedIndex = 0;
                                clientUMPD.selectedIndex = 0;
                            }
                        }
                    }
                    else {
                        clientBI.value = biValue
                        clientPD.value = pdValue
                        clientSLL.value = sllValue
                        clientMedPay.value = medPayValue
                        //Updated 9/28/18 for multi state MLW
                        if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
                            clientUMCSLStateExpansion.value = umCSLSEValue //copied to textbox
                            $("#" + txtUIMCSL).val(document.getElementById(ddlUMCSLStateExpansion).options[document.getElementById(ddlUMCSLStateExpansion).selectedIndex].text);
                            clientUMBIStateExpansion.value = umBISEValue //copied to textbox
                            $("#" + txtUIMBI).val(document.getElementById(ddlUMBIStateExpansion).options[document.getElementById(ddlUMBIStateExpansion).selectedIndex].text);
                            clientUMPD.value = 0
                            clientUMPDDeduct.value = 0
                        } else {
                            clientUMSSL.value = umSLLValue
                            clientUMBI.value = umBIValue
                            clientUMPD.value = umPDValue
                            clientUMPDDeduct.value = umPDDValue
                        }
                    }

                    //clientMedPay.selectedIndex = 4;
                    //Updated 9/28/18 for multi state MLW
                    if (quoteState == "IL" || quoteState == "OH") { //Updated 1/17/2022 for OH task 66101 MLW
                        clientUMPDDeduct.selectedValue = 0;
                    }
                    else {
                        clientUMPDDeduct.selectedIndex = 2;
                    }

                    if (namedNonOwned == "True") {
                        document.getElementById(dvAutoEnhance).style.display = "none";
                    } else {
                        document.getElementById(dvAutoEnhance).style.display = "block";
                        changeTransportationExpenseDDLOptions(chkAutoPlusEnhanceClientId, IsTransportationAvailable);
                        if (effDate < AutoPlusStartDate) {
                            document.getElementById(chkAutoEnhance).checked = true;
                        }
                    }
                }
                else {
                    // Is a parked car
                    //document.getElementById(dvPolicyCoverage).style.display = "none";
                    //document.getElementById(dvCompOnly).style.display = "block";
                }
            }
            catch (err) { // Not Physical Damage Only
                //document.getElementById(dvPolicyCoverage).style.display = "block";
                //document.getElementById(dvCompOnly).style.display = "none";

                if (namedNonOwned == "True")
                    document.getElementById(dvAutoEnhance).style.display = "none";
                else {
                    document.getElementById(dvAutoEnhance).style.display = "block";
                    changeTransportationExpenseDDLOptions(chkAutoPlusEnhanceClientId, IsTransportationAvailable);
                    if (effDate < AutoPlusStartDate) {
                        document.getElementById(chkAutoEnhance).checked = true;
                    }
                }
            }
        }
        else { // Is a trailer
            //document.getElementById(dvPolicyCoverage).style.display = "none";
            //document.getElementById(dvCompOnly).style.display = "block";
        }
    }
    else {
        switch (callingControl.value) {
            case compOnly:
                //document.getElementById(dvPolicyCoverage).style.display = "none";
                //document.getElementById(dvCompOnly).style.display = "block";
                break;
            case fullCoverage:
            case liabilityOnly:
                //document.getElementById(dvPolicyCoverage).style.display = "block";
                //document.getElementById(dvCompOnly).style.display = "none";

                if (namedNonOwned == "True")
                    document.getElementById(dvAutoEnhance).style.display = "none";
                else {
                    document.getElementById(dvAutoEnhance).style.display = "block";

                    if (effDate < AutoPlusStartDate) {
                        document.getElementById(chkAutoEnhance).checked = true;
                    }
                }
                break;
            default:
                if (bodyTypeID != recTrailerType && bodyTypeID != otherTrailerType) {
                    try {
                        // Not a trailer
                        if (vehCoveragePlanList[vehIdx].split("!")[1] != compOnly) {
                            // Not a parked car
                            //document.getElementById(dvPolicyCoverage).style.display = "block";
                            //document.getElementById(dvCompOnly).style.display = "none";

                            if (namedNonOwned == "True")
                                document.getElementById(dvAutoEnhance).style.display = "none";
                            else {
                                document.getElementById(dvAutoEnhance).style.display = "block";
                                if (effDate < AutoPlusStartDate) {
                                    document.getElementById(chkAutoEnhance).checked = true;
                                }
                            }
                        }
                        else {
                            // Is a parked car
                            //document.getElementById(dvPolicyCoverage).style.display = "none";
                            //document.getElementById(dvCompOnly).style.display = "block";
                        }
                    }
                    catch (err) { // Not Physical Damage Only
                        //document.getElementById(dvPolicyCoverage).style.display = "block";
                        //document.getElementById(dvCompOnly).style.display = "none";

                        if (namedNonOwned == "True")
                            document.getElementById(dvAutoEnhance).style.display = "none";
                        else {
                            document.getElementById(dvAutoEnhance).style.display = "block";
                            if (effDate < AutoPlusStartDate) {
                                document.getElementById(chkAutoEnhance).checked = true;
                            }
                        }
                    }
                }
                else { // Is a trailer
                    //document.getElementById(dvPolicyCoverage).style.display = "none";
                    //document.getElementById(dvCompOnly).style.display = "block";
                }
        }
    }
}

//Added 9/27/18 for multi state MLW
function ToggleVehicleUMPDCollision(ddlColl, chkUMPD, txtUMPDLimit, quoteState, collisionAndUMPDAvail, UMPDLimitsAvail, ddUMPDClientId, trUMPDLimitTBClientId, trUMPDLimitDDClientId, trUMPDLimitMsgClientId) {
    var clientColl = document.getElementById(ddlColl);
    var clientUMPD = document.getElementById(chkUMPD);
    var clientUMPDLimit = document.getElementById(txtUMPDLimit);
    var collSelectedValue = clientColl.options[clientColl.selectedIndex].value;
    
    if (clientColl.selectedIndex == 0) {
        if (document.getElementById(chkUMPD).checked === true) {
            chkUMPD.checked = true;
            clientUMPD.disabled = false;
            SetUMPDDefaultLimit(quoteState, txtUMPDLimit, UMPDLimitsAvail, ddUMPDClientId)
            switch (quoteState) {
                case "IL":
                    if (collisionAndUMPDAvail == "True") {
                        clientColl.disabled = false;
                    } else {
                        clientColl.disabled = true;
                    }
                    if (UMPDLimitsAvail == "True") {
                        document.getElementById(trUMPDLimitDDClientId).style.display = "block";
                        document.getElementById(trUMPDLimitMsgClientId).style.display = "block";
                    }
                    break;
                default:
                    clientColl.disabled = true;
                    break;
            }
        }
        else {
            chkUMPD.checked = false;
            clientUMPD.disabled = false;
            if (UMPDLimitsAvail == "True") {
                document.getElementById(ddUMPDClientId).value = "";
                document.getElementById(trUMPDLimitDDClientId).style.display = "none";
                document.getElementById(trUMPDLimitMsgClientId).style.display = "none";
            } else {
                $("#" + txtUMPDLimit).val("");
            }
            
            clientColl.disabled = false;
        }
    } else {
        switch (quoteState) {
            case "IL":
                if (collisionAndUMPDAvail == "True") {
                    clientUMPD.disabled = false;
                    if (document.getElementById(chkUMPD).checked === true) {
                        SetUMPDDefaultLimit(quoteState, txtUMPDLimit, UMPDLimitsAvail, ddUMPDClientId)
                        if (UMPDLimitsAvail == "True") {
                            document.getElementById(trUMPDLimitDDClientId).style.display = "block";
                            document.getElementById(trUMPDLimitMsgClientId).style.display = "block";
                        }                      
                    } else {
                        if (UMPDLimitsAvail == "True") {
                            document.getElementById(ddUMPDClientId).value = "";
                            document.getElementById(trUMPDLimitDDClientId).style.display = "none";
                            document.getElementById(trUMPDLimitMsgClientId).style.display = "none";
                        } else {
                            $("#" + txtUMPDLimit).val("");
                        }
                    }
                } else {
                    clientUMPD.checked = false;
                    clientUMPD.disabled = true;
                    if (UMPDLimitsAvail == "True") {
                        document.getElementById(ddUMPDClientId).value = "";
                        document.getElementById(trUMPDLimitDDClientId).style.display = "none";
                        document.getElementById(trUMPDLimitMsgClientId).style.display = "none";
                    } else {
                        $("#" + txtUMPDLimit).val("");
                    }
                }
                break;
            default:
                clientUMPD.checked = false;
                clientUMPD.disabled = true;
                $("#" + txtUMPDLimit).val("");
                break;
        }
        clientColl.disabled = false;
    }
}

function SetUMPDDefaultLimit(quoteState, txtUMPDLimit, UMPDLimitsAvail, ddUMPDClientId) {
    switch (quoteState) {
        case "IL":
            if (UMPDLimitsAvail == "True") {
                var umpdDefault = "48"; //15,000
                $(".UMPD_IL_LimitMatch").each(function () {
                    if (this.value != "48" && this.value != "0" && this.value != null && this.value != "") {
                        umpdDefault = this.value;
                    }
                });
                document.getElementById(ddUMPDClientId).value = umpdDefault;
            } else {
                $("#" + txtUMPDLimit).val("15,000");
            }
            break;
        case "OH":
            $("#" + txtUMPDLimit).val("7,500");
            break;
        default:
            $("#" + txtUMPDLimit).val("");
            break;
    }
}

function SetILUMPDLimits(ddUMPDClientId) {
    var umpdLimitVal = $("#" + ddUMPDClientId).val();
    $(".UMPD_IL_LimitMatch").each(function () {
        this.value = umpdLimitVal;
    });
}

//8/26/19 for bug 32399 ZTS - added CostNew
function ToggleVehicleCostNewOnCoverages(ddlCollID, ddlCompID, dvCostNewID) {
    var collID = $("#" + ddlCollID);
    var compID = $("#" + ddlCompID);
    var costNewID = $("#" + dvCostNewID);
    //var costNew = $("#" + dvCostNewID).val();

    if (collID.val() == 0 && compID.val() == 0) {
        costNewID.hide('fast');
    } else {
        costNewID.show('fast');
    }
}

function ToggleVehicleComp(controlValue, dvTransportation, ddlTransportation, dvColl, ddlColl, dvTowing, ddlTowing, dvSound, ddlSound,
    dvAVEquip, ddlAVEquip, dvMedia, ddlMedia, chkInterruptTravel, dvMotorcycle, txtCustEquip, chkAutoPlusEnhanceClientId, IsTransportationAvailable) {
    var fullCoverage = "0";
    var clientTransportation = document.getElementById(ddlTransportation);

    if (controlValue.value == fullCoverage) {
        //var options = clientTransportation.getElementsByTagName("option");
        //for (var i = 0; i < options.length; ++i) {
        //    if (options[i].text === "N/A") {
        //        options[i].selected = true;
        //    }
        //}
        clientTransportation.disabled = true;
        document.getElementById(dvMotorcycle).disabled = true;
        $("#" + txtCustEquip).val("0");
    }
    else {
        clientTransportation.disabled = false;
        document.getElementById(dvMotorcycle).disabled = false;
    }

    changeTransportationExpenseDDLOptions(chkAutoPlusEnhanceClientId, IsTransportationAvailable);
}

function UpdateSoundEquipList(controlValue, ddlMedia, ddlAVEquipSelectedValue) {
    var clientMedia = document.getElementById(ddlMedia);
    var clientAV = document.getElementById(ddlAVEquipSelectedValue);
    var totalItems = clientMedia.options.length;
    var mediaSelectedValue = clientMedia.options[clientMedia.selectedIndex].value;
    var mediaIndex = 0;
    var avSelectedValue = clientAV.options[clientAV.selectedIndex].value;

    // Remove existing values
    for (var cntr = totalItems; cntr >= 0; cntr--) {
        clientMedia.remove(cntr);
    }

    if (controlValue.value != "[1,000 INC]") {
        var includedOption = document.createElement("option");
        includedOption.text = "[200 INC]"
        includedOption.value = "0";
        clientMedia.options.add(includedOption);
        mediaIndex = 1;
    }
    else if (avSelectedValue == "N/A") {
        var includedOption = document.createElement("option");
        includedOption.text = "N/A"
        includedOption.value = "0";
        clientMedia.options.add(includedOption);

        var addlOption = document.createElement("option");
        addlOption.text = "200";
        addlOption.value = "212";
        clientMedia.options.add(addlOption);
        mediaIndex = 2;
    }
    else {
        var includedOption = document.createElement("option");
        includedOption.text = "[200 INC]"
        includedOption.value = "0";
        clientMedia.options.add(includedOption);
        mediaIndex = 1;
    }

    var addlOption = document.createElement("option");
    addlOption.text = "400";
    addlOption.value = "219";
    clientMedia.options.add(addlOption);

    if (mediaSelectedValue == "219") {
        clientMedia.selectedIndex = mediaIndex;
    }
}

function UpdateElectronicEquipList(controlValue, ddlMedia, ddlSoundSelectedValue) {
    var clientMedia = document.getElementById(ddlMedia);
    var clientSound = document.getElementById(ddlSoundSelectedValue);
    var totalItems = clientMedia.options.length;
    var mediaSelectedValue = clientMedia.options[clientMedia.selectedIndex].value;
    var mediaIndex = 0;
    var soundSelectedValue = clientSound.options[clientSound.selectedIndex].value;

    // Remove existing values
    for (var cntr = totalItems; cntr >= 0; cntr--) {
        clientMedia.remove(cntr);
    }

    if (controlValue.value != "N/A") {
        var includedOption = document.createElement("option");
        includedOption.text = "[200 INC]"
        includedOption.value = "0";
        clientMedia.options.add(includedOption);
        mediaIndex = 1;
    }
    else if (soundSelectedValue == "[1,000 INC]") {
        var includedOption = document.createElement("option");
        includedOption.text = "N/A"
        includedOption.value = "0";
        clientMedia.options.add(includedOption);

        var addlOption = document.createElement("option");
        addlOption.text = "200";
        addlOption.value = "212";
        clientMedia.options.add(addlOption);
        mediaIndex = 2;
    }
    else {
        var includedOption = document.createElement("option");
        includedOption.text = "[200 INC]"
        includedOption.value = "0";
        clientMedia.options.add(includedOption);
        mediaIndex = 1;
    }

    var addlOption = document.createElement("option");
    addlOption.text = "400";
    addlOption.value = "219";
    clientMedia.options.add(addlOption);

    if (mediaSelectedValue == "219") {
        clientMedia.selectedIndex = mediaIndex;
    }
}

//This is all about showing or hiding the Auto Plus Enhancement Endorsement based on effective date of the policy. The Endorsement is available for policies effective on or after 10/1/2016
function showHideAutoPlusEnhancement(divAutoPlusEnhanceClientID, chkAutoPlusEnhanceClientId, chkAutoEnhanceClientId, IsTransportationAvailable) {
    var divAutoPlusEnhance = document.getElementById(divAutoPlusEnhanceClientID);
    var chkAutoPlusEnhance = document.getElementById(chkAutoPlusEnhanceClientId);
    var chkAutoEnhance = document.getElementById(chkAutoEnhanceClientId);
    var displayErrorDateMessage = false;
    if (divAutoPlusEnhance) {
        var effDate = new Date($("[id*='hdnOriginalEffectiveDate']").val());
        var AutoPlusStartDate = new Date($("[id*='hiddenAutoPlusEnhancementEffectiveDate']").val());
        if (effDate >= AutoPlusStartDate) {
            if (divAutoPlusEnhance.style.display === "none") {
                divAutoPlusEnhance.style.display = "block";
                chkAutoPlusEnhance.checked = true;
                chkAutoEnhance.checked = false;
            }
        } else {
            if (chkAutoPlusEnhance) {
                if (chkAutoPlusEnhance.checked === true) {
                    chkAutoPlusEnhance.checked = false;
                    displayErrorDateMessage = true;
                }
            }
            divAutoPlusEnhance.style.display = "none";
            //chkAutoEnhance.checked = true;
        }
        changeTransportationExpenseDDLOptions(chkAutoPlusEnhanceClientId, IsTransportationAvailable);
    }
    if (displayErrorDateMessage === true) {
        alert("Auto Plus Enhancement Endorsement requires the effective date to be on or after 10/1/2016. Auto Plus Enhancement Endorsement has been removed.");
    }
}

function changeTransportationExpenseDDLOptions(chkAutoPlusEnhanceClientId, IsTransportationAvailable) {
    var chkAutoPlusEnhance = document.getElementById(chkAutoPlusEnhanceClientId);
    if (chkAutoPlusEnhance) {
        $("select[id*='ddTransportation']").each(function () {
            var transportationExpenseDDL = document.getElementById(this.id);
            if (transportationExpenseDDL) {
                if (transportationExpenseDDL.disabled === false) {
                    var options = transportationExpenseDDL.getElementsByTagName("option");
                    for (var i = 0; i < options.length; ++i) {
                        if (chkAutoPlusEnhance.checked === true) {
                            //Put if new var is true use new code
                            if (IsTransportationAvailable == "False") {
                                if (options[i].selected === true) {
                                    if (options[i].text !== "40/1200" && options[i].text !== "50/1500") {
                                        options[2].selected = true;
                                    }
                                }
                                if (options[i].text !== "40/1200" && options[i].text !== "50/1500") {
                                    options[i].disabled = true;
                                }
                            } else {
                                if (options[i].selected === true) {
                                    if (options[i].text == "[40/1200 INC] 30 DAYS") {
                                        options[1].selected = true;
                                    }
                                }
                                if (options[i].text !== "[40/1200 INC] 30 DAYS" && options[i].text !== "50/1500 30 DAYS") {
                                    options[0].disabled = true;
                                }
                            }
                        } else {
                            options[i].disabled = false;
                        }
                        //options[i].style.display = "inline";
                      
                    }
                }
            }
        });
    }
}

function addTitleToTransportation(ddTransportation, IsTransportationAvailable) {
        var transportationExpenseDDL = document.getElementById(ddTransportation);
        if (transportationExpenseDDL) {
            if (transportationExpenseDDL.disabled === false) {
                var options = transportationExpenseDDL.getElementsByTagName("option");
                for (var i = 0; i < options.length; ++i) {
                    if (IsTransportationAvailable != "False") {
                            //var titleText = options[i].text;
                            if (options[i].selected === true) {
                                var textToUse = options[i].text
                                $("select[id*='ddTransportation']").attr('title', textToUse);
                            }
                    }
                }
            }
        }
}  

function switchChecksAutoPlusEnhanceAndAutoEnhance(senderControl, chkAutoPlusEnhanceClientId, chkAutoEnhanceClientId, IsTransportationAvailable) {
    var chkAutoPlusEnhance = document.getElementById(chkAutoPlusEnhanceClientId);
    var chkAutoEnhance = document.getElementById(chkAutoEnhanceClientId);
    if (chkAutoPlusEnhance && chkAutoEnhance) {
        if (senderControl.checked === true) {
            if (senderControl.id === chkAutoPlusEnhanceClientId) {
                chkAutoEnhance.checked = false;
            }
            if (senderControl.id === chkAutoEnhanceClientId) {
                chkAutoPlusEnhance.checked = false;
            }
            changeTransportationExpenseDDLOptions(chkAutoPlusEnhanceClientId, IsTransportationAvailable);
        }
    }
}

//Added 4/26/2022 for bug 51567 MLW
function DisableBodyTypeOptions(ddBodyTypeClientId) {
    var bodyTypeValue = $("#" + ddBodyTypeClientId).val();
    //used for restricting existing vehicles on endorsements
    //body types that use cost new at rate to obtain vehicle symbols should not be available for selection on endorsement existing vehicles so that the symbols do not clear
    //can edit other vehicle body types (car, suv, van, pickup w/camper and pickup w/o camper) that when edited (VIN, year, make, model) clears their symbols
    if (bodyTypeValue !== SD_bodyTypeId_RecTrailer && bodyTypeValue !== SD_bodyTypeId_OtherTrailer && bodyTypeValue !== SD_bodyTypeId_AntiqueAuto && bodyTypeValue !== SD_bodyTypeId_ClassicAuto && bodyTypeValue !== SD_bodyTypeId_MotorHome && bodyTypeValue !== SD_bodyTypeId_MotorCycle) {
        $("#" + ddBodyTypeClientId + " option:contains('REC. TRAILER')").attr('disabled', 'disabled');
        $("#" + ddBodyTypeClientId + " option:contains('OTHER TRAILER')").attr('disabled', 'disabled');
        $("#" + ddBodyTypeClientId + " option:contains('ANTIQUE AUTO')").attr('disabled', 'disabled');
        $("#" + ddBodyTypeClientId + " option:contains('CLASSIC AUTO')").attr('disabled', 'disabled');
        $("#" + ddBodyTypeClientId + " option:contains('MOTOR HOME')").attr('disabled', 'disabled');
        $("#" + ddBodyTypeClientId + " option:contains('MOTORCYCLE')").attr('disabled', 'disabled');
    }
}