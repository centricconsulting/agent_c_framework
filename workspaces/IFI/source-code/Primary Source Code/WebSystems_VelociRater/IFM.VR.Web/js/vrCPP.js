
$(document).ready(function () {


});


var Cpp = new function () {

    /// Calculates total limit when transportation increased limit value changes
    this.TransportationIncreasedLimitChanged = function (txtLimitId, txtIncreasedLimitId, txtTotalLimitId) {
        var txtLimit = document.getElementById(txtLimitId);
        var txtIncreasedLimit = document.getElementById(txtIncreasedLimitId);
        var txtTotalLimit = document.getElementById(txtTotalLimitId);

        if (txtLimit && txtIncreasedLimit && txtTotalLimit) {
            var limit = parseInt(txtLimit.value.replace(',',''));
            var inclimit = parseInt(txtIncreasedLimit.value.replace(',', ''));
            if (!isNaN(limit) && !isNaN(inclimit)) {
                var totalLim = limit + inclimit;
                totalLim = numberWithCommas(totalLim);
                txtTotalLimit.value = totalLim;
                txtIncreasedLimit.value = numberWithCommas(inclimit);
            }
            else {
                // Unable to calculate total limit because one of the number fields is invalid
                txtTotalLimit.value = '';
            }
        }

        return true;
    }

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    /// Handles changes to the Package Modification dropdown
    //Updated 6/29/2022 for task 75037 MLW
    //this.PackageModificationChanged = function (ddId, AptInfoRowId, ContInfoRowId, RestInfoRowId, ChkPropEnhID, ChkContEnhID, ChkManufEnhId, trManufInfoRowId, trContEnhInfoRowId, chkFoodManufEnhId, trFoodManufInfoRowId) {
    this.PackageModificationChanged = function (ddId, AptInfoRowId, ContInfoRowId, RestInfoRowId, ChkPropEnhID, ChkContEnhID, ChkManufEnhId, trManufInfoRowId, trContEnhInfoRowId, chkFoodManufEnhId, trFoodManufInfoRowId, ChkPropEnhPlusID, trPkgModHotelTypeInfoRowId) {
        var dd = document.getElementById(ddId);
        var AptInfoRow = document.getElementById(AptInfoRowId);
        var ContInfoRow = document.getElementById(ContInfoRowId);
        var RestInfoRow = document.getElementById(RestInfoRowId);
        var chkPropEnh = document.getElementById(ChkPropEnhID);
        var chkContEnh = document.getElementById(ChkContEnhID);
        var chkManufEnh = document.getElementById(ChkManufEnhId);
        var trManufInfoRow = document.getElementById(trManufInfoRowId);
        var trContEnhInfoRow = document.getElementById(trContEnhInfoRowId);
        var chkFoodManufEnh = document.getElementById(chkFoodManufEnhId);
        var trFoodManufInfoRow = document.getElementById(trFoodManufInfoRowId);
        var chkPropEnhPlus = document.getElementById(ChkPropEnhPlusID); //Added 6/29/2022 for task 75037 MLW
        var trPkgModHotelTypeInfoRow = document.getElementById(trPkgModHotelTypeInfoRowId);

        // Hide the info rows
        if (AptInfoRow) { AptInfoRow.style.display = 'none'; }
        if (ContInfoRow) { ContInfoRow.style.display = 'none'; }
        if (RestInfoRow) { RestInfoRow.style.display = 'none'; }
        if (trManufInfoRow) { trManufInfoRow.style.display = 'none'; }
        if (trFoodManufInfoRow) { trFoodManufInfoRow.style.display = 'none'; }
        if (trPkgModHotelTypeInfoRow) { trPkgModHotelTypeInfoRow.style.display = 'none'; }

        // Enable the enhancement checkboxes initially
        if (chkPropEnh) { chkPropEnh.disabled = false; }
        if (chkContEnh) { chkContEnh.disabled = false; }
        if (chkManufEnh) { chkManufEnh.disabled = false; }
        if (chkFoodManufEnh) { chkFoodManufEnh.disabled = false; }
        if (chkPropEnhPlus) { chkPropEnhPlus.disabled = false; } //Added 6/29/2022 for task 75037 MLW

        if (dd) {
            switch (dd.value) {
                case "1": {
                    // Apartment
                    if (AptInfoRow) { AptInfoRow.style.display = ''; }
                    // When Apartment, Uncheck and Disable the enhancement checkboxes
                    if (chkPropEnh && chkContEnh && chkManufEnh) {
                        chkPropEnh.checked = false;
                        chkPropEnh.disabled = true;
                        chkContEnh.checked = false;
                        chkContEnh.disabled = true;
                        chkManufEnh.checked = false;
                        chkManufEnh.disabled = true;
                        chkFoodManufEnh.checked = false;
                        chkFoodManufEnh.disabled = true;
                        // Hide any displayed info text rows as well
                        if (ContInfoRow) { ContInfoRow.style.display = 'none'; }
                        if (RestInfoRow) { RestInfoRow.style.display = 'none'; }
                        if (trManufInfoRow) { trManufInfoRow.style.display = 'none'; }
                        if (trFoodManufInfoRow) { trFoodManufInfoRow.style.display = 'none'; }
                        if (trContEnhInfoRow) { trContEnhInfoRow.style.display = 'none'; }
                    }
                    if (chkPropEnhPlus) {
                        //Added 6/29/2022 for task 75037 MLW
                        chkPropEnhPlus.checked = false;
                        chkPropEnhPlus.disabled = true;
                    }
                    break;
                }
                case "2": {
                    // Contractors
                    if (ContInfoRow) { ContInfoRow.style.display = ''; }
                    break;
                }
                case "5": {
                    // Mercantile
                    if (RestInfoRow) { RestInfoRow.style.display = ''; }
                    break;
                
                }
                case "6": {
                    // Motel/Hotel
                    if (trPkgModHotelTypeInfoRow) { trPkgModHotelTypeInfoRow.style.display = ''; }
                }
            }
        }
    };

    // Handles changes to the Property Enhancement checkboxes
    //Updated 6/29/2022 for task 75037 MLW
    //this.PropertyOrContractorsOrManufacturersEnhancementChanged = function (sender, chkPropId, chkContId, trContInfoId, chkManufId, trManufInfoId, chkFoodManufId, trFoodManufInfoId) {
    this.PropertyOrContractorsOrManufacturersEnhancementChanged = function (sender, chkPropId, chkContId, trContInfoId, chkManufId, trManufInfoId, chkFoodManufId, trFoodManufInfoId, chkPropPlusId) {
        var chkProp = document.getElementById(chkPropId);
        var chkCont = document.getElementById(chkContId);
        var trContInfo = document.getElementById(trContInfoId);
        var chkManuf = document.getElementById(chkManufId);
        var trManufInfo = document.getElementById(trManufInfoId);
        var chkFoodManuf = document.getElementById(chkFoodManufId);
        var trFoodManufInfo = document.getElementById(trFoodManufInfoId);
        var chkPropPlus = document.getElementById(chkPropPlusId); //Added 6/29/2022 for task 75037 MLW
      

        if (chkProp && chkCont && trContInfo && chkManuf && trManufInfo && chkFoodManuf && trFoodManufInfo && sender) {
            //chkProp.disabled = false;
            //chkCont.disabled = false;
            //chkManuf.disabled = false;
            //trContInfo.style.display = 'none';
            //trManufInfo.style.display = 'none';

            switch (sender.toUpperCase()) {
                case "PROP": {
                    if (chkProp.checked) {
                        // If property enhancement is selected clear & disable the contractors & manufacturers
                        chkCont.checked = false;
                        chkCont.disabled = true;
                        chkManuf.checked = false;
                        chkManuf.disabled = true
                        chkFoodManuf.checked = false;
                        chkFoodManuf.disabled = true;
                        //trContInfo.style.display = '';
                        if (chkPropPlus) {
                            //Added 6/29/2022 for task 75037 MLW
                            chkPropPlus.checked = false;
                            chkPropPlus.disabled = true;
                        }
                    }
                    else {
                        // Property enhancement not checked
                        if (confirm('Are you sure you want to delete the Property Enhancement?') == true) {
                            chkProp.disabled = false;
                            chkCont.disabled = false;
                            chkManuf.disabled = false;
                            chkFoodManuf.disabled = false;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.disabled = false;
                            }
                            return true;
                        }
                        else {
                            chkCont.checked = false;
                            chkCont.disabled = true;
                            chkManuf.checked = false;
                            chkManuf.disabled = true
                            chkFoodManuf.checked = false;
                            chkFoodManuf.disabled = true
                            //trContInfo.style.display = '';
                            chkProp.checked = true;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.checked = false;
                                chkPropPlus.disabled = true;
                            }
                            return false;
                        }
                    }
                    break;
                }
                case "CONT": {
                    if (chkCont.checked) {
                        // If contractors enhancement is selected, show the contractors info row, clear & disable the manufacturers & property
                        trContInfo.style.display = '';
                        chkProp.checked = false;
                        chkProp.disabled = true;
                        chkManuf.checked = false;
                        chkManuf.disabled = true;
                        chkFoodManuf.checked = false;
                        chkFoodManuf.disabled = true;
                        if (chkPropPlus) {
                            //Added 6/29/2022 for task 75037 MLW
                            chkPropPlus.checked = false;
                            chkPropPlus.disabled = true;
                        }
                    }
                    else {
                        // Contractors enhancement not checked
                        if (confirm('Are you sure you want to delete the Contractors Enhancement?') == true) {
                            trContInfo.style.display = 'none';
                            chkProp.disabled = false;
                            chkCont.disabled = false;
                            chkManuf.disabled = false;
                            chkFoodManuf.disabled = false;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.disabled = false;
                            }
                            return true;
                        }
                        else {
                            trContInfo.style.display = '';
                            chkProp.checked = false;
                            chkProp.disabled = true;
                            chkManuf.checked = false;
                            chkManuf.disabled = true;
                            chkFoodManuf.checked = false;
                            chkFoodManuf.disabled = true;
                            chkCont.checked = true;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.checked = false;
                                chkPropPlus.disabled = true;
                            }
                            return false;
                        }
                    }
                    break;
                }
                case "MANUF": {
                    if (chkManuf.checked) {
                        // If manufacturers enhancement is selected, show the manufacturers info row, clear & disable the contractors & property
                        trManufInfo.style.display = '';
                        chkCont.checked = false;
                        chkCont.disabled = true;
                        chkProp.checked = false;
                        chkProp.disabled = true;
                        chkFoodManuf.checked = false;
                        chkFoodManuf.disabled = true;
                        if (chkPropPlus) {
                            //Added 6/29/2022 for task 75037 MLW
                            chkPropPlus.checked = false;
                            chkPropPlus.disabled = true;
                        }
                    }
                    else {
                        // Manufacturers enhancement not checked
                        if (confirm('Are you sure you want to delete the Manufacturers Enhancement?') == true) {
                            trManufInfo.style.display = 'none';
                            chkProp.disabled = false;
                            chkCont.disabled = false;
                            chkManuf.disabled = false;
                            chkFoodManuf.disabled = false;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.disabled = false;
                            }
                            return true;
                        }
                        else {
                            trManufInfo.style.display = '';
                            chkCont.checked = false;
                            chkCont.disabled = true;
                            chkProp.checked = false;
                            chkProp.disabled = true;
                            chkManuf.checked = true;
                            chkFoodManuf.checked = false;
                            chkFoodManuf.disabled = true;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.checked = false;
                                chkPropPlus.disabled = true;
                            }
                            return false;
                        }
                    }
                    break;
                }
                case "FOODMANUF": {
                    if (chkFoodManuf.checked) {
                        // If food manufacturers enhancement is selected, show the food manufacturers info row, clear & disable the other enhancement checkboxes
                        trFoodManufInfo.style.display = '';
                        chkCont.checked = false;
                        chkCont.disabled = true;
                        chkProp.checked = false;
                        chkProp.disabled = true;
                        chkManuf.checked = false;
                        chkManuf.disabled = true;
                        if (chkPropPlus) {
                            //Added 6/29/2022 for task 75037 MLW
                            chkPropPlus.checked = false;
                            chkPropPlus.disabled = true;
                        }
                    }
                    else {
                        // Food Manufacturers enhancement not checked
                        if (confirm('Are you sure you want to delete the Food Manufacturers Enhancement?') == true) {
                            trFoodManufInfo.style.display = 'none';
                            chkProp.disabled = false;
                            chkCont.disabled = false;
                            chkManuf.disabled = false;
                            chkFoodManuf.disabled = false;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.disabled = false;
                            }
                            return true;
                        }
                        else {
                            trFoodManufInfo.style.display = '';
                            chkCont.checked = false;
                            chkCont.disabled = true;
                            chkProp.checked = false;
                            chkProp.disabled = true;
                            chkManuf.checked = false;
                            chkManuf.disabled = true;
                            chkFoodManuf.checked = true;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.checked = false;
                                chkPropPlus.disabled = true;
                            }
                            return false;
                        }
                    }
                    break;
                }
                case "PROPPLUS": {
                    //Added 6/29/2022 for task 75037 MLW
                    if (chkPropPlus.checked) {
                        // If property plus enhancement is selected clear & disable the contractors & manufacturers
                        chkProp.checked = false;
                        chkProp.disabled = true;
                        chkCont.checked = false;
                        chkCont.disabled = true;
                        chkManuf.checked = false;
                        chkManuf.disabled = true
                        chkFoodManuf.checked = false;
                        chkFoodManuf.disabled = true;                       
                    }
                    else {
                        // Property plus enhancement not checked
                        if (confirm('Are you sure you want to delete the Property PLUS Enhancement?') == true) {
                            chkProp.disabled = false;
                            chkCont.disabled = false;
                            chkManuf.disabled = false;
                            chkFoodManuf.disabled = false;
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.disabled = false;
                            }
                            return true;
                        }
                        else {
                            chkProp.checked = false;
                            chkProp.disabled = true;
                            chkCont.checked = false;
                            chkCont.disabled = true;
                            chkManuf.checked = false;
                            chkManuf.disabled = true
                            chkFoodManuf.checked = false;
                            chkFoodManuf.disabled = true                            
                            if (chkPropPlus) {
                                //Added 6/29/2022 for task 75037 MLW
                                chkPropPlus.checked = true;
                            }
                            return false;
                        }
                    }
                    break;
                }
            }
        }
    };
    // Called when an item is selected on the Crime Class Code Lookup form - Populates the fields on the lookup form
    this.SubmitCrimeClassCodeSelection = function (ndx) {
        //var b = Cpr.UiBindings[ndx];
        var b = Cpp.UiBindings[0];  // There's only 1 class code per building

        // Hidden Field Values
        var hdnClassCodeValue = document.getElementById(b.SourceHdnClassCode).value;
        var hdnccIdValue = document.getElementById(b.SourceHdnDIA_Id).value;
        var hdnDescriptionValue = document.getElementById(b.SourceHdnDescription).value;
        var hdnPMAValue = document.getElementById(b.SourceHdnPMA).value;
        var hdnPMAsValue = document.getElementById(b.SourceHdnPMAs).value;
        var hdnGroupRateValue = document.getElementById(b.SourceHdnGroupRate).value;
        var hdnClassLimitValue = document.getElementById(b.SourceHdnClassLimit).value;
        var hdnFootNoteValue = document.getElementById(b.SourceHdnFootNote).value;
        // Input Fields
        var txtClassCode = document.getElementById(b.SourceTxtClassCode);
        var txtID = document.getElementById(b.SourceTxtDIA_ID);
        var txtDesc = document.getElementById(b.SourceTxtDescription);
        var ddPMA = document.getElementById(b.SourceDdlPMA);
        var txtGroupRate = document.getElementById(b.SourceTxtGroupRate);
        var txtClassLimit = document.getElementById(b.SourceTxtClassLimit);

        // Divs and error rows
        var divFootNote = document.getElementById(b.DivFootNote);
        var trFootnoteInfoRow = document.getElementById(b.SourceTrFootnoteInfoRow);

        // Buttons
        var btnApply = document.getElementById(b.SourceApplyButton);

        // CLEAR ALL THE VALUES
        txtClassCode.value = "";
        txtID.value = "";
        txtDesc.value = "";
        if (ddPMA.options) { ddPMA.options.length = 0; }
        txtGroupRate.value = "";
        txtClassLimit.value = "";
        divFootNote.innerHTML = "";

        // ADD THE LINKS TO THE FOOTNOTE TEXT HERE!  ASP DOES NOT LIKE SCRIPT IN THE HIDDEN FIELD VALUE!
        if (trFootnoteInfoRow) { trFootnoteInfoRow.style.display = 'none'; }  // hide the footnote info row
        var FootNote = hdnFootNoteValue
        if (hdnFootNoteValue) {
            FootNote = VRClassCode.AddLinksToFootnoteCrime(b.BuildingIndex, hdnFootNoteValue, b.LobId, b.ProgramId)
        }

        // Set the fields on the lookup control       
        txtClassCode.value = hdnClassCodeValue;
        txtID.value = hdnccIdValue;
        txtDesc.value = hdnDescriptionValue;

        //--- PMA ---
        // IF MULTIPLE PMA's LOAD THE DROPDOWN WITH THEM, SELECT NOTHING AND ENABLE THE DROPDOWN

        // disable the PMA dropdown to start with
        ddPMA.disabled = true;

        // Clear any existing values
        ddPMA.options.length = 0;

        // If there's a value in the hdnPMAs field we heva multiple PMA's so load and enable the dropdown
        if (hdnPMAsValue != 'null' && hdnPMAsValue != '') {
            ddPMA.disabled = false;
            var parts = hdnPMAsValue.split(',')
            var myopt = document.createElement('option');
            // Add an empty item
            myopt.text = myopt.value = '';
            ddPMA.add(myopt)
            for (i = 0; i < parts.length; i++) {
                var part = parts[i];
                part = part.replace('**', ',')  // Put any embedded commas back in the PMA
                myopt = null;
                myopt = document.createElement('option');
                myopt.text = myopt.value = part;
                ddPMA.add(myopt)
            }
            // Select the empty item
            ddPMA.value = '';
        }
        else {
            // No value in the hdnPMAs field which means we only have one
            // If there's a PMA value load it into the ddl and enable the control
            if (hdnPMAValue != 'null' && hdnPMAValue != '') {
                ddPMA.disabled = false;
                var myopt = document.createElement('option');
                myopt.text = myopt.value = hdnPMAValue;
                ddPMA.add(myopt);
            }
            else {
                // No PMA
                ddPMA.disabled = true;
            }
        }
        // --- END PMA ---

        txtGroupRate.value = hdnGroupRateValue;
        txtClassLimit.value = hdnClassLimitValue;
        divFootNote.innerHTML = FootNote

        // Enable the Apply button only if there is a class code in the class code textbox
        // otherwise it will be enabled when the user selects a class code from the footnotes
        if (txtClassCode.value != '') {
            if (btnApply) {
                btnApply.disabled = '';
                // You have to re-apply the OnClientClick script when you enable the button because 
                // when the button is disabled it loses it
                // NOTE: Setting the onclick property directly does not work
                btnApply.setAttribute('onClick', 'javascript: return Cpp.ValidateCrimeCLassificationLookupForm();');
            }
        }

        // Scroll to the data fields section on select
        location.href = "#"
        location.href = "#" + b.divCCInfo;

        return true;
    };

    // THIS FUNCTION WILL COPY THE SELECTED CLASS CODE INFO FROM THE CLASS CODE LOOKUP FORM TO THE CPR BUILDING CONTROL CLASS CODE SECTION
    // THIS IS THE NEW VERSION WHICH USES THE UIBINDINGS STRUCTURE INSTEAD OF PASSING ALL THE VARIABLES
    this.ApplyCrimeClassCode = function (BldgNdx) {
        var b = Cpp.UiBindings[0];

        // Get the source values
        var srcCCValue = document.getElementById(b.SourceTxtClassCode).value;
        var srcDescValue = document.getElementById(b.SourceTxtDescription).value;
        var srcIDValue = document.getElementById(b.SourceTxtDIA_ID).value;
        var srcPMAValue = document.getElementById(b.SourceDdlPMA).value;
        var srcGroupRateValue = document.getElementById(b.SourceTxtGroupRate).value;
        var srcClassLimitValue = document.getElementById(b.SourceTxtClassLimit).value;
        // Get the target controls
        var TargetTxtCC = document.getElementById(b.TargetTxtClassCode);
        var TargetTxtDesc = document.getElementById(b.TargetTxtDescription);
        var TargetHdnID = document.getElementById(b.TargetDIA_ID);
        var TargetHdnPMA = document.getElementById(b.TargetHdnPMA);
        var TargetHdnGroupRate = document.getElementById(b.TargetHdnGroupRate);
        var TargetHdnClassLimit = document.getElementById(b.TargetHdnClassLimit);

        // Set the target control values
        if (TargetTxtCC) { TargetTxtCC.value = srcCCValue; }
        if (TargetHdnID) { TargetHdnID.value = srcIDValue; }
        if (TargetTxtDesc) { TargetTxtDesc.value = srcDescValue; }
        if (TargetHdnPMA) { TargetHdnPMA.value = srcPMAValue; }
        if (TargetHdnGroupRate) { TargetHdnGroupRate.value = srcGroupRateValue; }
        if (TargetHdnClassLimit) { TargetHdnClassLimit.value = srcClassLimitValue; }

        return true;
    };

    // Performs field validation on the Building Classification Lookup form
    this.ValidateCrimeCLassificationLookupForm = function () {
        var b = Cpp.UiBindings[0];
        var ErrorRow = null;
        var ErrorsFound = false;

        // PMA
        var ddPMA = document.getElementById(b.SourceDdlPMA);
        ErrorRow = document.getElementById(b.SourcePMAValidationRow);
        if (ddPMA && ErrorRow) {
            ddPMA.style.borderColor = '';
            ErrorRow.style.display = 'none';
            if (ddPMA.options.length > 1 && ddPMA.value == '') {
                ddPMA.style.borderColor = "red";
                ErrorRow.style.display = '';
                ErrorsFound = true;
            }
        }

        if (ErrorsFound) { return false; } else { return true; }
    }

    // Clears all of the fields on the Building Classification Lookup form
    this.ClearCrimeCLassificationLookupForm = function () {
        var b = Cpp.UiBindings[0];

        var txtClassCode = document.getElementById(b.SourceTxtClassCode);
        var txtDscr = document.getElementById(b.SourceTxtDescription);
        var ddPMA = document.getElementById(b.SourceDdlPMA);
        var txtRateGroup = document.getElementById(b.SourceTxtGroupRate);
        var txtClassLimit = document.getElementById(b.SourceTxtClassLimit);
        var PMAErrRow = document.getElementById(b.SourcePMAValidationRow);
        var ddPMA = document.getElementById(b.SourceDdlPMA)

        if (PMAErrRow && ddPMA) {
            PMAErrRow.style.display = 'none';
            ddPMA.style.borderColor = '';
        }

        // Clear the fields
        if (txtClassCode) { txtClassCode.value = ''; }
        if (txtDscr) { txtDscr.value = ''; }
        if (ddPMA) { ddPMA.options.length = 0; }
        if (txtRateGroup) { txtRateGroup.value = ''; }
        if (txtClassLimit) { txtClassLimit.value = ''; }
        if (ddPMA) { ddPMA.value = '-1'; }

        // Disable the Apply button
        var btnApply = document.getElementById(b.SourceApplyButton);
        if (btnApply) { btnApply.disabled = true; }

        return true;
    }

    this.UiBindings = new Array();
    this.CrimeClassCodeUiBinding = function (VRLobId, VRProgramId, TgtDia_Id, SrcDIA_Id, TgtTxtClassCodeId, SrcTxtClassCodeId, TgtTxtDescriptionId, SrcTxtDescriptionId, SrcApplyButtonId, TgtHdnPMAId, SrcDdPMAId, TgtHdnGroupRateId, SrcTxtGroupRateId, TgtHdnClassLimitId, SrcTxtClassLimitId, divFootNoteId, TgtBCUseSpecificRow, TgtBCUseSpecificInfoRow, TgtBCUseSpecificGroupIRow, TgtBCUseSpecificGroupIIRow, TgtBICUseSpecificRow, TgtBICUseSpecificInfoRow, TgtBICUseSpecificGroupIRow, TgtBICUseSpecificGroupIIRow, TgtPPCUseSpecificRow, TgtPPCUseSpecificInfoRow, TgtPPCUseSpecificGroupIRow, TgtPPCUseSpecificGroupIIRow, TgtPPOUseSpecificRow, TgtPPOUseSpecificInfoRow, TgtPPOUseSpecificGroupIRow, TgtPPOUseSpecificGroupIIRow, HdnClassCodeId, HdnDescriptionId, HdnPMAId, HdnPMAsId, HdnDIAId_Id, HdnGroupRateId, HdnClassLimitId, HdnFootNoteId, trPMAValidationRowId, divCCInfoId, SrcTrFootNoteInfoRowId) {
        // Building Index - Not used for CPP - Crime, but is an unused argument for some functions.
        this.BuildingIndex = "0";
        this.LobId = VRLobId;
        this.ProgramId = VRProgramId;

        // Target controls - These are the controls on the BUILDING control that will be populated when we come back from the class code lookup
        this.TargetTxtClassCode = TgtTxtClassCodeId;
        this.TargetTxtDescription = TgtTxtDescriptionId;
        this.TargetHdnPMA = TgtHdnPMAId;
        this.TargetHdnGroupRate = TgtHdnGroupRateId;
        this.TargetHdnClassLimit = TgtHdnClassLimitId;
        this.TargetDIA_ID = TgtDia_Id;

        // Use Specific Rates rows
        this.TargetBCUseSpecificRow = TgtBCUseSpecificRow;
        this.TargetBCUseSpecificInfoRow = TgtBCUseSpecificInfoRow;
        this.TargetBCUseSpecificGroupIRow = TgtBCUseSpecificGroupIRow;
        this.TargetBCUseSpecificGroupIIRow = TgtBCUseSpecificGroupIIRow;
        this.TargetBICUseSpecificRow = TgtBICUseSpecificRow;
        this.TargetBICUseSpecificInfoRow = TgtBICUseSpecificInfoRow;
        this.TargetBICUseSpecificGroupIRow = TgtBICUseSpecificGroupIRow;
        this.TargetBICUseSpecificGroupIIRow = TgtBICUseSpecificGroupIIRow;
        this.TargetPPCUseSpecificRow = TgtPPCUseSpecificRow;
        this.TargetPPCUseSpecificInfoRow = TgtPPCUseSpecificInfoRow;
        this.TargetPPCUseSpecificGroupIRow = TgtPPCUseSpecificGroupIRow;
        this.TargetPPCUseSpecificGroupIIRow = TgtPPCUseSpecificGroupIIRow;
        this.TargetPPOUseSpecificRow = TgtPPOUseSpecificRow;
        this.TargetPPOUseSpecificInfoRow = TgtPPOUseSpecificInfoRow;
        this.TargetPPOUseSpecificGroupIRow = TgtPPOUseSpecificGroupIRow;
        this.TargetPPOUseSpecificGroupIIRow = TgtPPOUseSpecificGroupIIRow;

        // Source Controls - These are the controls on the class code lookup control whose values will be copied to the building control
        this.SourceTxtClassCode = SrcTxtClassCodeId;
        this.SourceTxtDescription = SrcTxtDescriptionId;
        this.SourceDdlPMA = SrcDdPMAId;
        this.SourceTxtGroupRate = SrcTxtGroupRateId;
        this.SourceTxtClassLimit = SrcTxtClassLimitId;
        this.SourceTxtDIA_ID = SrcDIA_Id;
        this.SourceApplyButton = SrcApplyButtonId;
        this.SourceTrFootnoteInfoRow = SrcTrFootNoteInfoRowId;

        // Divs
        this.DivFootNote = divFootNoteId;
        this.divCCInfo = divCCInfoId;

        // Hidden fields
        this.SourceHdnClassCode = HdnClassCodeId;
        this.SourceHdnDescription = HdnDescriptionId;
        this.SourceHdnPMA = HdnPMAId;
        this.SourceHdnPMAs = HdnPMAsId;
        this.SourceHdnDIA_Id = HdnDIAId_Id;
        this.SourceHdnGroupRate = HdnGroupRateId;
        this.SourceHdnClassLimit = HdnClassLimitId;
        this.SourceHdnFootNote = HdnFootNoteId;

        // Validation Rows
        this.SourcePMAValidationRow = trPMAValidationRowId;

        // Class Code Array
        this.ccArray = new Array();
    };


};