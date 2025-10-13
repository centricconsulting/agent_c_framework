
$(document).ready(function () {


});

var Cpr = new function () {

    // Handles changes in the blanket rating checkbox on the CPR coverages control
    this.BlanketRatingChanged = function (BlanketDllId, CauseOfLossRowId, CoinsuranceRowId, ValuationRowId, AgreedAmountRowId, AgreedAmountInfoRowId, trDeductibleRowId) {
        var BlanketDll = document.getElementById(BlanketDllId);
        var CauseOfLossRow = document.getElementById(CauseOfLossRowId);
        var CoinsuranceRow = document.getElementById(CoinsuranceRowId);
        var ValuationRow = document.getElementById(ValuationRowId);
        var AgreedAmountRow = document.getElementById(AgreedAmountRowId);
        var AgreedAmountInfoRow = document.getElementById(AgreedAmountInfoRowId);
        var trDeductibleRow = document.getElementById(trDeductibleRowId);

        if (BlanketDll && CauseOfLossRow && CoinsuranceRow && ValuationRow && AgreedAmountRow && AgreedAmountInfoRow && trDeductibleRow) {
            if (BlanketDll.value != "0") {
                // Blanket selected - Show the blanket subrows
                CauseOfLossRow.style.display = '';
                CoinsuranceRow.style.display = '';
                ValuationRow.style.display = '';
                AgreedAmountRow.style.display = '';
                AgreedAmountInfoRow.style.display = '';
                trDeductibleRow.style.display = '';
            }
            else {
                // Blanket not selected - hide the blanket subrows
                CauseOfLossRow.style.display = 'none';
                CoinsuranceRow.style.display = 'none';
                ValuationRow.style.display = 'none';
                AgreedAmountRow.style.display = 'none';
                AgreedAmountInfoRow.style.display = 'none';
                trDeductibleRow.style.display = 'none';
            }
        }

        return true;
    };

    // Handles clicks to the Business Income ALS checkbox on the CPR coverages page
    this.BusinessIncomeALSChanged = function (ALSChkId, ALSDataRowId, ALSInfoRowId, ALSWaitingRowId, hdnEffDateId) {
        var ALSChk = document.getElementById(ALSChkId);
        var DataRow = document.getElementById(ALSDataRowId);
        var InfoRow = document.getElementById(ALSInfoRowId);
        var WaitRow = document.getElementById(ALSWaitingRowId);
        var hdnDate = document.getElementById(hdnEffDateId);
        var effDate = new Date(hdnDate.value);
        var ALSDate = new Date('3/1/2020');

        // Show if the element exists
        if (ALSChk) {
            if (ALSChk.checked) {
                if (DataRow) { DataRow.style.display = ''; }
                if (InfoRow) { InfoRow.style.display = ''; }
                // Only show the waiting period row if effective date is 3/1/2020 or later
                if (WaitRow) {
                    if (effDate >= ALSDate) {
                        WaitRow.style.display = '';
                    }
                    else {
                        WaitRow.style.display = 'none';
                    }
                }
            }
            else {
                if (confirm("Are you sure you want to delete Business Income Coverage?") == true)
                {
                    if (DataRow) { DataRow.style.display = 'none'; }
                    if (InfoRow) { InfoRow.style.display = 'none'; }
                    if (WaitRow) { WaitRow.style.display = 'none'; }
                }
                else {
                    if (DataRow) { DataRow.style.display = ''; }
                    if (InfoRow) { InfoRow.style.display = ''; }
                    // Only show the waiting period row if effective date is 3/1/2020 or later
                    if (WaitRow) {
                        if (effDate >= ALSDate) {
                            WaitRow.style.display = '';
                        }
                        else {
                            WaitRow.style.display = 'none';
                        }
                    }
                    ALSChk.checked = true;
                    return false;
                }
            }
        }
        return true;
    };

    this.SubmitPIOClassCodeSelection = function () {
        // Hidden Field Values
        // Note that the values for hdnPIODescription and hdnPIOID were set for the current control in the call to this function from PerformPIOClassCodeLookup
        var hdnDescriptionValue = document.getElementById(hdnPIODescription).value;
        var hdnccIdValue = document.getElementById(hdnPIOID).value;
        // Input Fields
        var txtClassCode = document.getElementById(txtPIOClassCode);
        var txtDesc = document.getElementById(txtPIODescription);
        var txtID = document.getElementById(txtPIOID);

        if (hdnDescriptionValue && hdnccIdValue && txtClassCode && txtDesc && txtID) {
            txtClassCode.value = hdnDescriptionValue;
            txtID.value = hdnccIdValue;
        }
        return true;
    };

    // Called when an item is selected on the Building Class Code Lookup form - Populates the fields on the lookup form
    this.SubmitBuildingClassCodeSelection = function (ndx) {
        //var b = Cpr.UiBindings[ndx];
        var b = Cpr.UiBindings[0];  // There's only 1 class code per building

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
        var trYardRate = document.getElementById(b.SourceYardRateRowId);
        var ddYardRate = document.getElementById(b.SourceDdlYardRate);
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
            FootNote = VRClassCode.AddLinksToFootnote(b.BuildingIndex, hdnFootNoteValue, b.LobId, b.ProgramId)
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

        // YARD RATE - Applies to class codes 1400, 1650, 1700
        if (trYardRate && ddYardRate) {
            trYardRate.style.display = 'none';
            ddYardRate.value = -1;
            if (hdnClassCodeValue == '1400' || hdnClassCodeValue == '1650' || hdnClassCodeValue == '1700') {
                trYardRate.style.display = '';
            }
        }

        // Enable the Apply button only if there is a class code in the class code textbox
        // otherwise it will be enabled when the user selects a class code from the footnotes
        if (txtClassCode.value != '') {
            if (btnApply) {
                btnApply.disabled = '';
                // You have to re-apply the OnClientClick script when you enable the button because 
                // when the button is disabled it loses it
                // NOTE: Setting the onclick property directly does not work
                btnApply.setAttribute('onClick', 'javascript: return Cpr.ValidateBuildingCLassificationLookupForm();');
            }
        }

        // Scroll to the data fields section on select
        location.href = "#"
        location.href = "#" + b.divCCInfo;

        return true;
    };

    // THIS FUNCTION WILL COPY THE SELECTED CLASS CODE INFO FROM THE CLASS CODE LOOKUP FORM TO THE CPR BUILDING CONTROL CLASS CODE SECTION
    // THIS IS THE NEW VERSION WHICH USES THE UIBINDINGS STRUCTURE INSTEAD OF PASSING ALL THE VARIABLES
    this.ApplyBuildingClassCode = function (BldgNdx) {
        var b = Cpr.UiBindings[0];

        // Get the source values
        var srcCCValue = document.getElementById(b.SourceTxtClassCode).value;
        var srcDescValue = document.getElementById(b.SourceTxtDescription).value;
        var srcIDValue = document.getElementById(b.SourceTxtDIA_ID).value;   
        var srcPMAValue = document.getElementById(b.SourceDdlPMA).value;
        var srcGroupRateValue = document.getElementById(b.SourceTxtGroupRate).value;
        var srcClassLimitValue = document.getElementById(b.SourceTxtClassLimit).value;
        var srcYardRateValue = document.getElementById(b.SourceDdlYardRate).value;
        // Get the target controls
        var TargetTxtCC = document.getElementById(b.TargetTxtClassCode);
        var TargetTxtDesc = document.getElementById(b.TargetTxtDescription);
        var TargetHdnID = document.getElementById(b.TargetDIA_ID);  
        var TargetHdnPMA = document.getElementById(b.TargetHdnPMA);
        var TargetHdnGroupRate = document.getElementById(b.TargetHdnGroupRate);
        var TargetHdnClassLimit = document.getElementById(b.TargetHdnClassLimit);
        var TargetHdnYardRate = document.getElementById(b.TargetHdnYardRateId);

        // Set the target control values
        if (TargetTxtCC) { TargetTxtCC.value = srcCCValue; }
        if (TargetHdnID) { TargetHdnID.value = srcIDValue; }
        if (TargetTxtDesc) { TargetTxtDesc.value = srcDescValue; }
        if (TargetHdnPMA) { TargetHdnPMA.value = srcPMAValue; }
        if (TargetHdnGroupRate) { TargetHdnGroupRate.value = srcGroupRateValue; }
        if (TargetHdnClassLimit) { TargetHdnClassLimit.value = srcClassLimitValue; }
        if (TargetHdnYardRate) { TargetHdnYardRate.value = srcYardRateValue; }

        return true;
    };

    // Performs field validation on the Building Classification Lookup form
    this.ValidateBuildingCLassificationLookupForm = function () {
        var b = Cpr.UiBindings[0];
        var ErrorRow = null;
        var ErrorsFound = false;

        // YARD RATE
        var trYardRate = document.getElementById(b.SourceYardRateRowId);
        var ddYardRate = document.getElementById(b.SourceDdlYardRate);
        ErrorRow = document.getElementById(b.SourceYardRateValidationRow);
        if (trYardRate && ddYardRate && ErrorRow) {
            ddYardRate.style.borderColor = '';
            ErrorRow.style.display = 'none';

            if (trYardRate.style.display == '') {
                if (ddYardRate.value == '' || ddYardRate.value == '-1'){
                    ddYardRate.style.borderColor = "red";
                    ErrorRow.style.display = '';
                    ErrorsFound = true;
                }
            }
        }

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
    this.ClearBuildingCLassificationLookupForm = function () {
        var b = Cpr.UiBindings[0];

        var txtClassCode = document.getElementById(b.SourceTxtClassCode);
        var txtDscr = document.getElementById(b.SourceTxtDescription);
        var ddPMA = document.getElementById(b.SourceDdlPMA);
        var txtRateGroup = document.getElementById(b.SourceTxtGroupRate);
        var txtClassLimit = document.getElementById(b.SourceTxtClassLimit);
        var ddYardRate = document.getElementById(b.SourceDdlYardRate);
        var YardRateErrRow = document.getElementById(b.SourceYardRateValidationRow);
        var PMAErrRow = document.getElementById(b.SourcePMAValidationRow);
        var ddPMA = document.getElementById(b.SourceDdlPMA)

        // Hide the error row(s)
        if (YardRateErrRow && ddYardRate) {
            YardRateErrRow.style.display = 'none';
            ddYardRate.style.borderColor = '';
        }

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

    // THIS FUNCTION WILL COPY THE SELECTED EARTHQUAKE CLASSIFICATION INFO FROM THE EQ CLASSIFICATION LOOKUP FORM TO THE CPR BUILDING CONTROL PPC or PPC COVERAGE SECTION
    this.ApplyEQClassification = function (sender, BldgNbr, LocNdx, BldgNdx) {
        //alert('Apply EQ Classification! Loc# = ' + LocNdx + "Bldg# = " + BldgNdx);
        // Get the correct EQ UI Binding
        // There should be only one but it's possible that there could be more
        var b = null;
        for (i = 0; i < Cpr.EQUiBindings.length; i++) {
            var x = Cpr.EQUiBindings[i];
            if (x.LocationIndex == LocNdx && x.BuildingIndex == BldgNdx) {
                b = x;
                break;
            }
        }

        var bldg = Cpr.BuildingCoverageBindings[BldgNbr];

        if (b && bldg) {
            //// Get the source values
            var srcIDValue = document.getElementById(b.SourceHdnId).value;
            var srcDescValue = document.getElementById(b.SourceHdnDescription).value;
            var srcRateGroupValue = document.getElementById(b.SourceHdnRateGroup).value;
            //// Get the target controls
            var tgtTxtDesc = document.getElementById(b.TargetTxtClassificationDesc);
            var tgtHdnID = document.getElementById(b.TargetDIA_ID);
            var tgtHdnRateGroup = document.getElementById(b.TargetHdnRateGroup);

            //// Set the target control values
            if (tgtHdnID) { tgtHdnID.value = srcIDValue; }
            if (tgtTxtDesc) { tgtTxtDesc.value = srcDescValue; }
            if (tgtHdnRateGroup) { tgtHdnRateGroup.value = srcRateGroupValue; }

            // If the other personal property coverage is selected, hide it's EQ lookup row
            var chkPPC = document.getElementById(bldg.chkPPC);
            var trPPCEQLookupRow = document.getElementById(bldg.trPPCEQLookupRow);
            var chkPPO = document.getElementById(bldg.chkPPO);
            var trPPOEQLookupRow = document.getElementById(bldg.trPPOEQLookupRow);

            switch (sender) {   // Sender 1 = PPC, Sender 2 = PPO
                case 1:
                    if (chkPPO.checked) {
                        trPPOEQLookupRow.style.display = 'none';
                    }
                    break;
                case 2:
                    if (chkPPC.checked) {
                        trPPCEQLookupRow.style.display = 'none';
                    }
                    break;
            }
        }  // end if b && bldg

        return true;
    };

    // Agreed amount changed on the coverages page
    this.CPRAgreedAmountChanged = function (chkId, hdnId, ddCoinsId) {
        var chkAA = document.getElementById(chkId);
        var hdnAA = document.getElementById(hdnId);
        var ddCoins = document.getElementById(ddCoinsId);

        if (chkAA && hdnAA && ddCoins) {
            // If checked set the hidden  field
            if (chkAA.checked) {
                hdnAA.value = "1"
                // When Agreed Amount is checked, we need to set the Coinsurance value to 100% and disable it
                ddCoins.value = "7"
                ddCoins.disabled = true;
            }
            else {
                if (confirm("Are you sure you want to delete Agreed Amount coverage?") == true) {
                    hdnAA.value = '';
                    ddCoins.disabled = false;
                }
                else {
                    chkAA.checked = true;
                    return false;
                }
            }
        }
        return true;
    };

    // 8-14-2018 Bug 28331
    // If Building Construction Type is one of the following...
    //   - MASONRY-NON COMBUSTIBLE   (id 14)
    //   - MODIFIED FIRE RESISTIVE   (id 15)
    //   - FIRE RESISTIVE   (id 16)
    // ...then we need to show specific rates regardless of what the class code is.
    this.BuildingConstructionChanged = function (BldgNbr) {
        var b = Cpr.BuildingCoverageBindings[BldgNbr];

        if (b) {
            var BCEligible = false;
            var BICEligible = false;
            var PPCEligible = false;
            var PPOEligible = false;

            // Get the construction and class code values
            var ddConst = document.getElementById(b.ddINFConstruction);
            var cval = ddConst.value;
            var txtCC = document.getElementById(b.txtINFClassCode);
            var cc = txtCC.value;

            // Get all of the controls we'll need
            // BC
            var chkBC = document.getElementById(b.chkBC);
            var hdnBCUseSpecific = document.getElementById(b.hdnBCUseSpecificChecked);
            var trBCUseSpecificRow = document.getElementById(b.trBCUseSpecificRow);
            var trBCUseSpecificInfoRow = document.getElementById(b.trBCUseSpecificInfoRow);
            var chkBCUseSpecific = document.getElementById(b.chkBCUseSpecific);
            var trBCGroupIRow = document.getElementById(b.trBCGroupIRow);
            var trBCGroupIIRow = document.getElementById(b.trBCGroupIIRow);

            // BIC
            var chkBIC = document.getElementById(b.chkBIC);
            var hdnBICUseSpecific = document.getElementById(b.hdnBICUseSpecificChecked);
            var trBICUseSpecificRow = document.getElementById(b.trBICUseSpecificRow);
            var trBICUseSpecificInfoRow = document.getElementById(b.trBICUseSpecificInfoRow);
            var chkBICUseSpecific = document.getElementById(b.chkBICUseSpecific);
            var trBICGroupIRow = document.getElementById(b.trBICGroupIRow);
            var trBICGroupIIRow = document.getElementById(b.trBICGroupIIRow);

            // PPC
            var chkPPC = document.getElementById(b.chkPPC);
            var hdnPPCUseSpecific = document.getElementById(b.hdnPPCUseSpecificChecked);
            var trPPCUseSpecificRow = document.getElementById(b.trPPCUseSpecificRow);
            var trPPCUseSpecificInfoRow = document.getElementById(b.trPPCUseSpecificInfoRow);
            var chkPPCUseSpecific = document.getElementById(b.chkPPCUseSpecific);
            var trPPCGroupIRow = document.getElementById(b.trPPCGroupIRow);
            var trPPCGroupIIRow = document.getElementById(b.trPPCGroupIIRow);

            // PPO
            var chkPPO = document.getElementById(b.chkPPO);
            var hdnPPOUseSpecific = document.getElementById(b.hdnPPOUseSpecificChecked);
            var trPPOUseSpecificRow = document.getElementById(b.trPPOUseSpecificRow);
            var trPPOUseSpecificInfoRow = document.getElementById(b.trPPOUseSpecificInfoRow);
            var chkPPOUseSpecific = document.getElementById(b.chkPPOUseSpecific);
            var trPPOGroupIRow = document.getElementById(b.trPPOGroupIRow);
            var trPPOGroupIIRow = document.getElementById(b.trPPOGroupIIRow);

            // Determine whether specific rates apply either by the construntion type or class code
            if (chkBC.checked){BCEligible = Cpr.IsBuildingEligibleForSpecificRates(cc, cval);}
            if (chkBIC.checked) { BICEligible = Cpr.IsBuildingEligibleForSpecificRates(cc, cval); }
            if (chkPPC.checked) { PPCEligible = Cpr.IsBuildingEligibleForSpecificRates(cc, cval); }
            if (chkPPO.checked) { PPOEligible = Cpr.IsBuildingEligibleForSpecificRates(cc, cval); }

            // Now that we've determined whether or not each coverage is eligible for specific rates or not, set them accordingly
            if (BCEligible) {
                trBCUseSpecificInfoRow.style.display = '';
                trBCUseSpecificInfoRow.style.display = '';
                chkBCUseSpecific.checked = true;
                trBCGroupIRow.style.display = '';
                trBCGroupIIRow.style.display = '';
                hdnBCUseSpecific.value = 'true'; 
            }
            else {
                trBCUseSpecificInfoRow.style.display = 'none';
                trBCUseSpecificInfoRow.style.display = 'none';
                chkBCUseSpecific.checked = false;
                trBCGroupIRow.style.display = 'none';
                trBCGroupIIRow.style.display = 'none';
                hdnBCUseSpecific.value = 'false'; 
            }

            if (BICEligible) {
                trBICUseSpecificInfoRow.style.display = '';
                trBICUseSpecificInfoRow.style.display = '';
                chkBICUseSpecific.checked = true;
                trBICGroupIRow.style.display = '';
                trBICGroupIIRow.style.display = '';
                hdnBICUseSpecific.value = 'true'; 
            }

            if (PPCEligible) {
                trPPCUseSpecificInfoRow.style.display = '';
                trPPCUseSpecificInfoRow.style.display = '';
                chkPPCUseSpecific.checked = true;
                trPPCGroupIRow.style.display = '';
                trPPCGroupIIRow.style.display = '';
                hdnPPCUseSpecific.value = 'true'; 
            }

            if (PPOEligible) {
                trPPOUseSpecificInfoRow.style.display = '';
                trPPOUseSpecificInfoRow.style.display = '';
                chkPPOUseSpecific.checked = true;
                trPPOGroupIRow.style.display = '';
                trPPOGroupIIRow.style.display = '';
                hdnPPOUseSpecific.value = 'true'; 
            }

        }  // end b is not nothing
        else { return false; }  //b was nothing

        // If we got here all is good
        return true;
    }


    // Handles the Agreed Amount & Blanket checkboxes on the Special Class Code lookup control (PIO Class Code)
    // If either one is checked show the info row
    this.PIOAgreedAmountOrBlanketCheckboxChanged = function (chkBlanketId, chkAgreedId, inforowId, ddCOLId, ddCOINSId, ddVALId, ddDEDId, COLVal, COINSVal, VALVal, DEDVal, AgreedVal) {
        var chkBlanket = document.getElementById(chkBlanketId);
        var chkAgreed = document.getElementById(chkAgreedId);
        var inforow = document.getElementById(inforowId);
        var ddCOL = document.getElementById(ddCOLId);
        var ddCOINS = document.getElementById(ddCOINSId);
        var ddVAL = document.getElementById(ddVALId);
        var ddDED = document.getElementById(ddDEDId);

        if (chkBlanket && ddCOL && ddCOINS && ddVAL && ddDED) {
            if (chkBlanket.checked) {
                // Blanket checkbox is checked, disable the Cause of Loss, Co-Insurance, Valuation and Deductible 
                // dropdowns and set their value to the passed values
                // Set defaults
                if (COLVal && COINSVal && VALVal && DEDVal) {
                    ddCOL.value = COLVal;
                    ddCOINS.value = COINSVal;
                    ddVAL.value = VALVal;
                    ddDED.value = DEDVal;
                    // Set Agreed Amount checkbox
                    if (AgreedVal && AgreedVal.toUpperCase() == "TRUE") {
                        chkAgreed.checked = true;
                    }
                    else {
                        chkAgreed.checked = false;
                    }
                }
                // Disable controls
                ddCOL.disabled = true;
                ddCOINS.disabled = true;
                ddVAL.disabled = true;
                ddDED.disabled = true;
                // Always disable agreed amount if blanket is checked because it must match whats on the policy coverages page
                var p = chkAgreed.parentElement.tagName;
                if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                chkAgreed.disabled = true;

                //if (chkAgreedId && chkAgreed.checked == true) {
                //    // If blanket is checked and Agreed is checked then disable the agreed amount checkbox
                //    // Only disable the checkbox if the blanket does not have the agreed amount checked on the coverages page
                //    var p = chkAgreed.parentElement.tagName;
                //    if (HasBlanketAgreed.toUpperCase() == "TRUE") {
                //        if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                //        chkAgreed.disabled = true;
                //    }
                //    else {
                //        if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                //        chkAgreed.disabled = false;
                //    }
                //}
            }
            else {
                // Blanket is NOT checked - enable the Cause of Loss, Co-Insurance, Valuation, Deductible dropdowns ands the Agreed Amount checkbox
                ddCOL.disabled = '';
                ddCOINS.disabled = '';
                ddVAL.disabled = '';
                ddDED.disabled = '';
                if (chkAgreedId) {
                    var p = chkAgreed.parentElement.tagName;
                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                    chkAgreed.disabled = false;
                }
            }
        }

        // If either checkbox is checked show the info row, otherwise hide it
        if (chkBlanket && chkAgreed) {
            if (chkBlanket.checked || chkAgreed.checked) {
                if (inforow) {
                    inforow.style.display = '';
                }
            }
            else {
                if (inforow) {
                    inforow.style.display = 'none';
                }
            }
            return true;
        }
    };

    // Handles the blanket checkboxes on the CPR building coverages
    this.BlanketCheckboxChanged = function (sender, ChkId, chkAgreedId, InfoRowId, ddCOLId, ddCOINSId, ddVALId, ddDEDId, COLValue, COINSValue, VALValue, DEDValue, AgreedValue, BlanketText, IsBuildingZero, hdnAgreedId, locDropDown, hideDeductible) {
        var Chk = document.getElementById(ChkId);
        var chkAgreed = document.getElementById(chkAgreedId);
        var InfoRow = document.getElementById(InfoRowId);
        var ddCOL = document.getElementById(ddCOLId);
        var ddCOINS = document.getElementById(ddCOINSId);
        var ddVAL = document.getElementById(ddVALId);
        var ddDED = document.getElementById(ddDEDId);
        var hdnAgreed = document.getElementById(hdnAgreedId);
        var locddDED = document.getElementById(locDropDown);


        // Hide the info row
        if (InfoRow) { InfoRow.style.display = 'none'; }
        // Show the info row if the checkbox is checked
        if (Chk && InfoRow) {
            if (Chk.checked) {
                InfoRow.style.display = '';
            }
        }

        // If checked disable the Cause of Loss, Coinsurance, Valuation, Deductible, and Location Deductible rows and set their defaults to the passed values
        if (Chk) {
            if (Chk.checked) {
                if (ddCOL) {
                    ddCOL.disabled = true;
                    if (COLValue) {
                        ddCOL.value = COLValue;
                    }
                }
                if (ddCOINS) {
                    ddCOINS.disabled = true;
                    if (COINSValue) {
                        ddCOINS.value = COINSValue;
                    }
                }
                if (ddVAL) {
                    ddVAL.disabled = true;
                    if (VALValue) {
                        ddVAL.value = VALValue;
                    }
                }               
                //if (hideDeductible == "True") {
                //    if (locddDED) {
                //        locddDED.disabled = true;
                //        if (DEDValue) {
                //            locddDED.value = DEDValue;
                //        }
                //    }
                //} else {
                if (ddDED) {
                    ddDED.disabled = true;
                    if (DEDValue) {
                        ddDED.value = DEDValue;
                    }
                }
                //}

                if (sender && AgreedValue && chkAgreed && BlanketText && IsBuildingZero) {
                    if (AgreedValue.toUpperCase() == "TRUE") {
                        // QUOTE HAS AGREED AMOUNT
                        // Check the Agreed Amount checkbox and disable it
                        chkAgreed.checked = true;
                        if (hdnAgreed) { hdnAgreed.value = true; }
                        var p = chkAgreed.parentElement.tagName;
                        if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                        chkAgreed.disabled = true;
                    }
                    else {
                        // QUOTE DOES NOT HAVE AGREED AMOUNT
                        switch (sender.toUpperCase()) {
                            case "BC": {
                                if (BlanketText.toUpperCase() == "BUILDING" || BlanketText.toUpperCase() == "COMBINED") {
                                    // If called from Building Coverage and the blanket is building or combined then uncheck the 
                                    // Agreed Amount checkbox and disable it
                                    chkAgreed.checked = false;
                                    if (hdnAgreed) { hdnAgreed.value = false; }
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                                    chkAgreed.disabled = true
                                }
                                else {
                                    // Called from Building Coverage but blanket is Personal Property Only
                                    // Uncheck the checkbox and enable it
                                    chkAgreed.checked = false;
                                    if (hdnAgreed) { hdnAgreed.value = false; }
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                                    chkAgreed.disabled = false;
                                }
                                break;
                            }
                            case "PPC": {
                                if (BlanketText.toUpperCase() == "PROPERTY" || BlanketText.toUpperCase() == "COMBINED") {
                                    // If called from Personal Property Coverage and the blanket is property only then uncheck the 
                                    // Agreed Amount checkbox and disable it
                                    chkAgreed.checked = false;
                                    if (hdnAgreed) { hdnAgreed.value = false; }
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                                    chkAgreed.disabled = true;
                                }
                                else {
                                    // Called from Personal Property Coverage but blanket is building or combined
                                    // Uncheck the checkbox and enable it
                                    chkAgreed.checked = false;
                                    if (hdnAgreed) { hdnAgreed.value = false; }
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                                    chkAgreed.disabled = false;
                                }
                                break;
                            }
                        }
                    }
                }
            }  // end if chk.checked
            else {
                if (ddCOL) { ddCOL.disabled = false; }
                if (ddCOINS) { ddCOINS.disabled = false; }
                if (ddVAL) { ddVAL.disabled = false; }
                if (ddDED) { ddDED.disabled = false; }
                if (locddDED) { locddDED.disabled = false; }
                if (chkAgreed) {
                    if (IsBuildingZero.toUpperCase() == "TRUE") {
                        // Building Zero
                        switch (sender) {
                            case "BC": {
                                if (BlanketText.toUpperCase() == "BUILDING" || BlanketText.toUpperCase() == "COMBINED") {
                                    // Blanket is building or combined, disable the Agreed Amount checkbox 
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                                    chkAgreed.disabled = true;
                                    if (chkAgreed.checked && InfoRow) { InfoRow.style.display = ''; }
                                }
                                else {
                                    // Blanket is NOT building or combined, enable the Agreed Amount checkbox 
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                                    chkAgreed.disabled = false;
                                    if (chkAgreed.checked && InfoRow) { InfoRow.style.display = ''; }
                                }
                                break;
                            }
                            case "PPC": {
                                if (BlanketText.toUpperCase() == "PROPERTY")  {
                                    // Blanket is property, disable the Agreed Amount checkbox 
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = true; }
                                    chkAgreed.disabled = true;
                                    if (chkAgreed.checked && InfoRow) { InfoRow.style.display = ''; }
                                }
                                else {
                                    // Blanket is NOT Property or combined, enable the Agreed Amount checkbox 
                                    var p = chkAgreed.parentElement.tagName;
                                    if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                                    chkAgreed.disabled = false;
                                    if (chkAgreed.checked && InfoRow) { InfoRow.style.display = ''; }
                                }
                                break;
                            }
                        }  // end Switch
                    } // End if IsBuildingZero
                    else {
                        // NOT BUILDING ZERO - always enable the 
                        var p = chkAgreed.parentElement.tagName;
                        if (p == 'SPAN') { chkAgreed.parentElement.disabled = false; }
                        chkAgreed.disabled = false;
                        if (chkAgreed.checked && InfoRow) { InfoRow.style.display = ''; }
                    } // end else if builing zero
                }  // End if chkAgreed
            } //end else (if chk.checked)
        } // end if Chk
        Cpr.checkBlanketBoxUpdates()
        return true;
    };

    // Handles clicks to the building coverage agreed amount checkboxes
    this.BuildingAgreedAmountCheckboxChanged = function(chkCoverageId, chkBlanketId, chkAgreedId, trAgreedAmountInfoRowId, AgreedAmtValue, AgreedAmtEnabled, hdnId) {
        var chkCoverage = document.getElementById(chkCoverageId);
        var chkAgreed = document.getElementById(chkAgreedId);
        var chkBlanket = document.getElementById(chkBlanketId)
        var trInfo = document.getElementById(trAgreedAmountInfoRowId);
        var hdn = document.getElementById(hdnId);

        if (chkCoverage && chkAgreed && chkBlanket && trInfo && hdn) {
            if (chkAgreed.checked) {
                trInfo.style.display = '';
                hdn.value = "true";
            }
            else {
                hdn.value = "false";
                trInfo.style.display = 'none';
            }
        }
        return true;
    }

    // Gets called when the Location Equipment Breakdown or PIO Eartquake coverage checkboxes change
    this.LocOrPIOCoverageCheckboxChanged = function(sender, ChkId, dataRowId) {
        var ans = false;
        var chk = document.getElementById(ChkId);
        var dataRow = document.getElementById(dataRowId);

        if (chk && sender) {
            if (!chk.checked) {
                switch (sender) {
                    case "PIOEQ": {
                        // PIO Eartquake coverage
                        ans = confirm("Are you sure you want to delete the property in the open Earthquake Coverage?")
                        break;
                    }
                    case "EQB": {
                        // Equipment breakdown coverage
                        ans = confirm("Are you sure you want to delete Equipment Breakdown Coverage?")
                        break;
                    }
                }
                if (ans == false) {
                    chk.checked = true;
                    if (dataRow) { datarow.style.display = '';}
                }
            }
        }
        return true;
    }

    // Gets called when the Property in the Open Wind Hail Deductible % checkbox changes, Building Coverages Wind Hail Deductible % checkbox changes
    this.WindHailCheckboxChanged = function (chkWindHailId) {
        var chk = document.getElementById(chkWindHailId);

        if (chk) {
            if (!chk.checked) {
                if (!confirm('Are you sure you want to delete this coverage?')) {
                    chk.checked = true;
                    return false;
                }
            }
        }
        return true;
    }

    // Gets called when the Property in the Open Earthquake coverage checkbox changes, Building Information Earthquake checkbox changes - NewCo only
    this.ToggleEarthquakeDeductible = function (chkEarthquakeId, trEarthquakeDeductibleId) {
        var chk = document.getElementById(chkEarthquakeId);
        var eqRow = document.getElementById(trEarthquakeDeductibleId);

        if (chk && eqRow) {
            if (chk.checked) {
                eqRow.style.display = '';
            } else {
                eqRow.style.display = 'none';
            }
        } 
        return true;
    }

    // Gets called when the Building Information Earthquake Classification drop down changes - NewCo only
    this.ResetEarthquakeDeductibleMinimum = function (ddEarthquakeClassificationId, ddEarthquakeDeductibleId) {
        var eqClassification = document.getElementById(ddEarthquakeClassificationId);
        var eqDeductible = document.getElementById(ddEarthquakeDeductibleId);
        var eqClassificationValue = eqClassification.value;
        //var msgDeductibleChanged = "Earthquake Classification has been changed, therefore, the Earthquake Deductible Options have been reset."; //Keeping this in in case they change their mind on showing the message every time
        if (eqDeductible && eqClassificationValue) {
            alert('Earthquake Classification has been changed, therefore, the Earthquake Deductible Options have been reset.'); //current requirements state that even if the classification is changed from one 5% deductible classification to another 5% deductible classification and 5% is the selected deductible, still show the message. Always showing the message.
            if (eqClassificationValue == '16' || eqClassificationValue == '19' || eqClassificationValue == '20' || eqClassificationValue == '22' || eqClassificationValue == '23' || eqClassificationValue == '24') {
                //These classifications need to be 10% by default, all others 5% by default: 3C=16 4C=19, 4D=20, 5AA=22, 5B=23, or 5C=24 - See QuickQuoteBuilding EarthquakeBuildingClassificationTypeId in DiamondStaticData.xml
                //Requirements state that even if they have the deductible set to a higher percent, when changing the classification they get a popup and the deductible reset to the minimum default.
                $("#" + ddEarthquakeDeductibleId + " option[value = '34']").attr('disabled', 'disabled');
                if (eqDeductible.value != "36") {
                    eqDeductible.value = "36"; //10%
                    //alert(msgDeductibleChanged); //Keeping this in in case they change their mind on showing the message every time
                }               
            } else {
                $("#" + ddEarthquakeDeductibleId + " option[value = '34']").removeAttr('disabled');
                if (eqDeductible.value != "34") {
                    eqDeductible.value = "34"; //5%
                    //alert(msgDeductibleChanged); //Keeping this in in case they change their mind on showing the message every time
                }
            }
        }
        return true;
    }

    // Handles the Building Coverage checkboxes on the Building conttrol
    this.BuildingCoverageCheckboxChanged = function (sender, BldgNbr, ChkId, DataRowId, DataRow2Id, InfoRowID, defCOL, defCOINS, defVAL, defDED, defGroupI, defGroupII, AgreedAmtVal, AgreedAmtEnabled, QuoteHasBlanket, BlanketType, isBlanketAvailable, isValuationACVAvailable, trOwnerOccupiedID, showOwnerOccupiedPercentage, ddOwnerOccPercId, classCodeFound, isOkToDefaultWindHail, BCWindHailTextId) {
        var ddCOL = null;
        var ddCOINS = null;
        var ddVAL = null;
        var ddDED = null;
        var txtGroup1 = null;
        var txtGroup2 = null;
        var chkAgreed = null;
        var chkUseSpecific = null;
        var trUseSpecificRow = null;
        var trUseSpecificInfoRow = null;
        var trGroupIRow = null;
        var trGroupIIRow = null;
        var trEQRow = null;
        var cc = null;
        var SpecificUseEligible = false;
        var hdnUseSpecificChecked = null;
        var trBlanket = null;
        var trBlanketInfo = null;
        var chkBlanket = null;
        var trWindHailRow = null;
        var chkWindHail = null;
      
        var EarthquakeEnabled = false;

        var b = Cpr.BuildingCoverageBindings[BldgNbr];

        // Get the selected class code
        if (b.txtINFClassCode) {
            var cc = document.getElementById(b.txtINFClassCode).value;
            var constructionval = document.getElementById(b.ddINFConstruction).value;
            // Check to see if the class code is eligible for specific rates
            if (cc)
            {
                SpecificUseEligible = Cpr.IsBuildingEligibleForSpecificRates(cc, constructionval);  
                if (sender) {
                    if (sender == "BC" || sender == "PPC" || sender == "PPO") {
                        if (isValuationACVAvailable == "True" && (cc == '0196' || cc == '0197' || cc == '0198')) {
                            //if dwelling class code, set Valuation to 2 = Actual Cash Value
                            defVAL = '2';
                        }
                    }
                }
            }
        }


        //Get the checkbox, info & data rows
        var Chk = document.getElementById(ChkId);
        var DataRow = document.getElementById(DataRowId);
        var DataRow2 = document.getElementById(DataRow2Id);
        var InfoRow = document.getElementById(InfoRowID);
        var OwnerOccupiedRow = document.getElementById(trOwnerOccupiedID);

        var chkBC = document.getElementById(b.chkBC);
        var chkBIC = document.getElementById(b.chkBIC);
        var chkPPC = document.getElementById(b.chkPPC);
        var chkPPO = document.getElementById(b.chkPPO);

        if (sender) {
            // Hide everything 
            if (DataRow) { DataRow.style.display = 'none'; }
            if (DataRow2) { DataRow2.style.display = 'none'; }
            if (InfoRow) { InfoRow.style.display = 'none'; }
            //if (OwnerOccupiedRow) { OwnerOccupiedRow.style.display = 'none'; }
            

            // Individual controls
            var trBCUseSpecific = document.getElementById(b.trBCUseSpecificRow);
            var trBCUseSpecificInfo = document.getElementById(b.trBCUseSpecificInfoRow);
            var chkBCUseSpecific = document.getElementById(b.chkBCUseSpecific);
            var txtBCGroup1 = document.getElementById(b.txtBCGroup1);
            var txtBCGroup2 = document.getElementById(b.txtBCGroup2);
            var trBCGroupIRow = document.getElementById(b.trBCGroupIRow);
            var trBCGroupIIRow = document.getElementById(b.trBCGroupIIRow);
            var hdnBCUseSpecificChecked = document.getElementById(b.hdnBCUseSpecificChecked);

            var trBICUseSpecific = document.getElementById(b.trBICUseSpecificRow);
            var trBICUseSpecificInfo = document.getElementById(b.trBICUseSpecificInfoRow);
            var chkBICUseSpecific = document.getElementById(b.chkBICUseSpecific);
            var txtBICGroup1 = document.getElementById(b.txtBICGroup1);
            var txtBICGroup2 = document.getElementById(b.txtBICGroup2);
            var trBICGroupIRow = document.getElementById(b.trBICGroupIRow);
            var trBICGroupIIRow = document.getElementById(b.trBICGroupIIRow);
            var hdnBICUseSpecificChecked = document.getElementById(b.hdnBICUseSpecificChecked);

            var trPPCUseSpecific = document.getElementById(b.trPPCUseSpecificRow);
            var trPPCUseSpecificInfo = document.getElementById(b.trPPCUseSpecificInfoRow);
            var chkPPCUseSpecific = document.getElementById(b.chkPPCUseSpecific);
            var txtPPCGroup1 = document.getElementById(b.txtPPCGroup1);
            var txtPPCGroup2 = document.getElementById(b.txtPPCGroup2);
            var trPPCGroupIRow = document.getElementById(b.trPPCGroupIRow);
            var trPPCGroupIIRow = document.getElementById(b.trPPCGroupIIRow);
            var trPPCEQLookupRow = document.getElementById(b.trPPCEQLookupRow);
            var chkPPCEQ = document.getElementById(b.chkPPCEQ);
            var hdnPPCUseSpecificChecked = document.getElementById(b.hdnPPCUseSpecificChecked);

            var trPPOUseSpecific = document.getElementById(b.trPPOUseSpecificRow);
            var chkPPOUseSpecific = document.getElementById(b.chkPPOUseSpecific);
            var trPPOUseSpecificInfo = document.getElementById(b.trPPOUseSpecificInfoRow);
            var txtPPOGroup1 = document.getElementById(b.txtPPOGroup1);
            var txtPPOGroup2 = document.getElementById(b.txtPPOGroup2);
            var trPPOGroupIRow = document.getElementById(b.trPPOGroupIRow);
            var trPPOGroupIIRow = document.getElementById(b.trPPOGroupIIRow);
            var trPPOEQLookupRow = document.getElementById(b.trPPOEQLookupRow);
            var chkPPOEQ = document.getElementById(b.chkPPOEQ);
            var chkINFEQ = document.getElementById(b.chkINFEarthquake);
            var hdnPPOUseSpecificChecked = document.getElementById(b.hdnPPOUseSpecificChecked);

            // Set the EartquakeEnabled switch
            if (chkINFEQ) {
                if (chkINFEQ.checked) { EarthquakeEnabled = true; }
            }

            // Get the controls for the passed coverage
            switch (sender) {
                case "BC": {
                    ddCOL = document.getElementById(b.ddBCCOL);
                    ddCOINS = document.getElementById(b.ddBCCOINS);
                    ddVAL = document.getElementById(b.ddBCVAL);
                    ddDED = document.getElementById(b.ddBCDED);
                    chkAgreed = document.getElementById(b.chkBCAgreedAmount);
                    hdnAgreed = document.getElementById(b.hdnAgreedBC);
                    trUseSpecificRow = trBCUseSpecific;
                    trUseSpecificInfoRow = trBCUseSpecificInfo;
                    chkUseSpecific = chkBCUseSpecific;
                    txtGroup1 = document.getElementById(b.txtBCGroup1);
                    txtGroup2 = document.getElementById(b.txtBCGroup2);
                    trGroupIRow = document.getElementById(b.trBCGroupIRow);
                    trGroupIIRow = document.getElementById(b.trBCGroupIIRow);
                    hdnUseSpecificChecked = hdnBCUseSpecificChecked;
                    trBlanket = document.getElementById(b.trBCBlanketRow);
                    chkBlanket = document.getElementById(b.chkBCBlanket);
                    trBlanketInfo = document.getElementById(b.trBCBlanketInfoRow);
                    //trEQRow = document.getElementById(b.trBCEQRow);
                    trWindHailRow = document.getElementById(b.trBCWindHailRow);
                    chkWindHail = document.getElementById(b.chkBCWindHail);
                    ownerOccPercId = document.getElementById(b.ddOwnerOccPercId);
                    break;
                }
                case "BIC": {
                    ddCOL = document.getElementById(b.ddBICCOL);
                    trUseSpecificRow = trBICUseSpecific;
                    trUseSpecificInfoRow = trBICUseSpecificInfo;
                    chkUseSpecific = chkBICUseSpecific;
                    txtGroup1 = document.getElementById(b.txtBICGroup1);
                    txtGroup2 = document.getElementById(b.txtBICGroup2);
                    trGroupIRow = document.getElementById(b.trBICGroupIRow);
                    trGroupIIRow = document.getElementById(b.trBICGroupIIRow);
                    trEQRow = document.getElementById(b.trBICEQRow);
                    hdnUseSpecificChecked = hdnBICUseSpecificChecked;
                    break;
                }
                case "PPC": {
                    ddCOL = document.getElementById(b.ddPPCCOL);
                    ddCOINS = document.getElementById(b.ddPPCCOINS);
                    ddVAL = document.getElementById(b.ddPPCVAL);
                    ddDED = document.getElementById(b.ddPPCDED);
                    chkAgreed = document.getElementById(b.chkPPCAgreedAmount);
                    hdnAgreed = document.getElementById(b.hdnAgreedPPC);
                    chkUseSpecific = chkPPCUseSpecific;
                    trUseSpecificRow = trPPCUseSpecific;
                    trUseSpecificInfoRow = trPPCUseSpecificInfo;
                    txtGroup1 = document.getElementById(b.txtPPCGroup1);
                    txtGroup2 = document.getElementById(b.txtPPCGroup2);
                    trGroupIRow = document.getElementById(b.trPPCGroupIRow);
                    trGroupIIRow = document.getElementById(b.trPPCGroupIIRow);
                    trEQRow = document.getElementById(b.trPPCEQRow);
                    hdnUseSpecificChecked = hdnPPCUseSpecificChecked;
                    trBlanket = document.getElementById(b.trPPCBlanketRow);
                    chkBlanket = document.getElementById(b.chkPPCBlanket);
                    trBlanketInfo = document.getElementById(b.trPPCBlanketInfoRow);
                    trWindHailRow = document.getElementById(b.trPPCWindHailRow);
                    chkWindHail = document.getElementById(b.chkPPCWindHail);
                    break;
                }
                case "PPO": {
                    ddCOL = document.getElementById(b.ddPPOCOL);
                    ddCOINS = document.getElementById(b.ddPPOCOINS);
                    ddVAL = document.getElementById(b.ddPPOVAL);
                    ddDED = document.getElementById(b.ddPPODED);
                    chkUseSpecific = chkPPOUseSpecific;
                    trUseSpecificRow = trPPOUseSpecific;
                    trUseSpecificInfoRow = trPPOUseSpecificInfo;
                    txtGroup1 = document.getElementById(b.txtPPOGroup1);
                    txtGroup2 = document.getElementById(b.txtPPOGroup2);
                    trGroupIRow = document.getElementById(b.trPPOGroupIRow);
                    trGroupIIRow = document.getElementById(b.trPPOGroupIIRow);
                    trEQRow = document.getElementById(b.trPPOEQRow);
                    hdnUseSpecificChecked = hdnPPOUseSpecificChecked;
                    trBlanket = document.getElementById(b.trPPOBlanketRow);
                    chkBlanket = document.getElementById(b.chkPPOBlanket);
                    trBlanketInfo = document.getElementById(b.trPPOBlanketInfoRow);
                    trWindHailRow = document.getElementById(b.trPPOWindHailRow);
                    chkWindHail = document.getElementById(b.chkPPOWindHail);
                    break;
                }
            }  // end switch

            // Show if the element exists
            if (Chk) {
                if (Chk.checked) {
                    if (DataRow) { DataRow.style.display = ''; }
                    if (DataRow2) { DataRow2.style.display = ''; }
                    if (InfoRow) { InfoRow.style.display = ''; }
                    if (showOwnerOccupiedPercentage.toUpperCase() == "TRUE") {
                        if (OwnerOccupiedRow) { OwnerOccupiedRow.style.display = ''; }
                    } else {
                        if (OwnerOccupiedRow) { OwnerOccupiedRow.style.display = 'none'; }
                    }

                    // Set defaults - Don't do it for the first building (Building Number 0)
                    if (BldgNbr != 0) {

                        // Set the coverage defaults if applicable
                        if (ddCOL && defCOL) { ddCOL.value = defCOL; }
                        if (ddCOINS && defCOINS) { ddCOINS.value = defCOINS; }
                        if (ddVAL && defVAL) { ddVAL.value = defVAL; }
                        if (ddDED && defDED) { ddDED.value = defDED; }
                        if (txtGroup1) { txtGroup1.value = defGroupI;}
                        if (txtGroup2) { txtGroup2.value = defGroupII; }
                    } // End If (BldgNbr != 0)

                    // Enable the blanket row if necessary
                    if (QuoteHasBlanket && BlanketType && trBlanket && chkBlanket) {
                        trBlanket.style.display = 'none';
                        chkBlanket.checked = false;
                        chkBlanket.disabled = false;
                        if (trBlanketInfo) { trBlanketInfo.style.display = 'none'; }
                        if (ddCOL) {
                            ddCOL.disabled = false;
                        }
                        if (ddCOINS) {
                            ddCOINS.disabled = false;
                        }
                        if (ddVAL) {
                            ddVAL.disabled = false;
                        }
                        if (ddDED) {
                            ddDED.disabled = false;
                        }
                        if (QuoteHasBlanket.toUpperCase() == "TRUE") {
                            if (b.txtINFClassCode) {
                                var cc = document.getElementById(b.txtINFClassCode).value;
                                switch (BlanketType.toUpperCase()) {
                                    case "COMBINED": {
                                        trBlanket.style.display = '';
                                        if (isBlanketAvailable == "True") {
                                            if (cc == "0196" || cc == "0197" || cc == "0198") {
                                                chkBlanket.disabled = true;
                                            } else {
                                                chkBlanket.disabled = false;
                                            }
                                        } else {
                                            chkBlanket.disabled = false;
                                        }
                                        
                                        break;
                                    }
                                    case "PROPERTY": {
                                        if (sender == "PPO" || sender == "PPC") {
                                            trBlanket.style.display = '';
                                            if (isBlanketAvailable == "True") {
                                                if (cc == "0196" || cc == "0197" || cc == "0198") {
                                                    chkBlanket.disabled = true;
                                                } else {
                                                    chkBlanket.disabled = false;
                                                }
                                            } else {
                                                chkBlanket.disabled = false;
                                            }
                                        }
                                        break;
                                    }
                                    case "BUILDING": {
                                        if (sender == "BC") {
                                            trBlanket.style.display = '';
                                            if (isBlanketAvailable == "True") {
                                                if (cc == "0196" || cc == "0197" || cc == "0198") {
                                                    chkBlanket.disabled = true;
                                                } else {
                                                    chkBlanket.disabled = false;
                                                }
                                            } else {
                                                chkBlanket.disabled = false;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // SET AGREED AMOUNT
                    // BLANKET SHOULD NOT BE CHECKED IF WE JUST ADDED THE COVERAGE
                    // Enable and uncheck the Agreed Amount checkbox
                    if (chkAgreed) {
                        chkAgreed.checked = false;
                        if (hdnAgreed) { hdnAgreed.value = false; }
                        chkAgreed.disabled = false;
                    }

                    // **** USE SPECIFIC RATES GROUP FIELDS ******
                    // Determine if we need to show the group rows 
                    // Only check if we haven't already said to hide them
                    ShowGroupRows = SpecificUseEligible;
                    if (SpecificUseEligible) {
                        switch (sender) {
                            case "BC": {
                                if (chkBIC.checked) {
                                    if (txtBICGroup1.value != '' || txtBICGroup2.value != '') {
                                        ShowGroupRows = false;
                                    }
                                }
                                break;
                            }
                            case "BIC": {
                                if (chkBC.checked) {
                                    if (txtBCGroup1.value != '' || txtBCGroup2.value != '') {
                                        ShowGroupRows = false;
                                    }
                                }
                                break;
                            }
                            case "PPC": {
                                if (chkPPO.checked) {
                                    if (txtPPOGroup1.value != '' || txtPPOGroup2.value != '') {
                                        ShowGroupRows = false;
                                    }
                                }
                                break;
                            }
                            case "PPO": {
                                if (chkPPC.checked) {
                                    if (txtPPCGroup1.value != '' || txtPPCGroup2.value != '') {
                                        ShowGroupRows = false;
                                    }
                                }
                                break;
                            }
                        }
                    }

                    // Set the Group I/Group II rows visibilty for all coverages, including Building Number 0 (the first one)
                    // (this is where the ShowGroupRows var is used)
                    if (trGroupIRow && trGroupIIRow && trUseSpecificRow && chkUseSpecific && trUseSpecificInfoRow) {
                        if (ShowGroupRows) {
                            chkUseSpecific.checked = true;
                            if (hdnUseSpecificChecked){ hdnUseSpecificChecked.value = "true"; }
                            trUseSpecificRow.style.display = '';
                            trUseSpecificInfoRow.style.display = '';
                            trGroupIRow.style.display = '';
                            trGroupIIRow.style.display = '';
                        }
                        else {
                            if (SpecificUseEligible) {
                                // If the class code is eligible for specific rates you still need to check the second personal property 
                                // specific use checkbox (even though the specific rates fields will be hidden) so it will be processed correctly
                                // on save in the code- behind.
                                chkUseSpecific.checked = true;
                                trUseSpecificRow.style.display = '';
                                trUseSpecificInfoRow.style.display = '';
                                if (hdnUseSpecificChecked) { hdnUseSpecificChecked.value = "true"; }
                            }
                            else {
                                // Class code is NOT eligible for specific rates so don't show or check the specific rates checkbox
                                chkUseSpecific.checked = false;
                                trUseSpecificRow.style.display = 'none';
                                trUseSpecificInfoRow.style.display = 'none';
                                if (hdnUseSpecificChecked) { hdnUseSpecificChecked.value = "false"; }
                            }
                            //chkUseSpecific.checked = false;
                            //trUseSpecificRow.style.display = 'none';
                            //trUseSpecificInfoRow.style.display = 'none';
                            trGroupIRow.style.display = 'none';
                            trGroupIIRow.style.display = 'none';
                        }
                    }

                    // Set the Earthquake visibility for all coverages.
                    if (trEQRow) {
                        if (EarthquakeEnabled) {
                            // Earthquake is checked in the building info section - show the earthquake checkbox
                            trEQRow.style.display = '';
                        }
                        else {
                            // Earthquake is NOT checked in the building info section - hide the earthquake checkbox
                            trEQRow.style.display = 'none';
                        }
                    }

                    // **** PPC/PPO EARTHQUAKE ROWS ******
                    switch (sender) {
                        case "PPC": {
                            // Uncheck the checkbox, hide the EQ lookup row
                            chkPPCEQ.checked = false;
                            trPPCEQLookupRow.style.display = 'none';

                            // PPC COVERAGE IS CHECKED
                            // Check to see if PPO EQ is checked
                            if (chkPPO.checked) {
                                // If PPO EQ is checked check the PPC EQ checkbox
                                if (chkPPOEQ.checked) {
                                    chkPPCEQ.checked = true;
                                }
                                else {
                                    // If PPC EQ is NOT checked, enable the PPO EQ lookup row if the EQ checkbox is checked
                                    if (chkPPCEQ.checked) { trPPCEQLookupRow.style.display = ''; }
                                }
                            }
                            else {
                                // PPO NOT checked - Show the EQ classification row if the EQ checkbox is checked
                                if (chkPPCEQ.checked) { trPPCEQLookupRow.style.display = ''; }
                            }
                            break;
                        }
                        case "PPO": {
                            // Uncheck the checkbox, hide the EQ lookup row
                            chkPPOEQ.checked = false
                            trPPOEQLookupRow.style.display = 'none';

                            // Check to see if PPC EQ is checked
                            if (chkPPC.checked) {
                                // If PPC EQ is checked check the PPO EQ checkbox
                                if (chkPPCEQ.checked) {
                                    chkPPOEQ.checked = true;
                                }
                                else {
                                    // If PPC EQ is NOT checked, enable the PPO EQ lookup row if the EQ checkbox is checked
                                    if (chkPPOEQ.checked) { trPPOEQLookupRow.style.display = ''; }
                                }
                            }
                            else {
                                // PPC NOT checked - Show the EQ classification row if the EQ checkbox is checked
                                if (chkPPOEQ.checked) { trPPOEQLookupRow.style.display = ''; }
                            }
                            break;
                        }
                    }
                    //Wind Hail Deductible % - for BC, PPC, PPO only
                    if (trWindHailRow) {
                        switch (sender) {
                            case "BC": {
                                var windHailText = document.getElementById(BCWindHailTextId);
                                if (locationWindHailAvailable.toUpperCase() == "TRUE") {
                                    trWindHailRow.style.display = '';
                                    var locWindHailVal = '';
                                    var ownerOccPercVal = '';
                                    var locWindHail = document.getElementById(LocWindHailDeductibleId); //LocDeductibleId
                                    var valueToRemove = "0";
                                    
                                    if (locWindHail) {
                                        locWindHailVal = locWindHail.value;
                                    }
                                    if (ownerOccPercId) {
                                        ownerOccPercVal = ownerOccPercId.value;
                                    }
                                    if (chkWindHail && locWindHail && windHailText) {
                                        if (isOkToDefaultWindHail.toUpperCase() == "TRUE") {
                                            if (classCodeFound.toUpperCase() == "FALSE") {
                                                if (locWindHailVal != '') {
                                                    if (ownerOccPercVal == '30' || ownerOccPercVal == '31') {
                                                        chkWindHail.checked = true;
                                                        chkWindHail.disabled = true;
                                                        if (locWindHailVal == '0') {
                                                            locWindHail.value = '32'; // 1% Wind Hail
                                                        }
                                                        windHailText.style.display = '';
                                                        for (var i = 0; i < locWindHail.options.length; i++) {
                                                            if (locWindHail.options[i].value === valueToRemove) {
                                                                locWindHail.remove(i);
                                                                break; // Stop after removing the first match
                                                            }
                                                        }
                                                    } else {
                                                        if (locWindHailVal == '0') {
                                                            chkWindHail.disabled = true;
                                                            chkWindHail.checked = false;
                                                        } else {
                                                            chkWindHail.disabled = false;
                                                            chkWindHail.checked = false;
                                                        }
                                                        windHailText.style.display = 'none';
                                                    }
                                                }
                                            } else {
                                                if (locWindHailVal == '0') {
                                                    chkWindHail.checked = false;
                                                    chkWindHail.disabled = true;
                                                } else {
                                                    chkWindHail.disabled = false;
                                                    chkWindHail.checked = false;

                                                }
                                                windHailText.style.display = 'none';
                                            }
                                        } else {
                                                windHailText.style.display = 'none';
                                        }
                                    }
                                } else {
                                    if (windHailText) {
                                        windHailText.style.display = 'none';
                                    }
                                }   
                                break;
                            }
                            case "PPC":
                            case "PPO": {
                                if (locationWindHailAvailable.toUpperCase() == "TRUE") {
                                    trWindHailRow.style.display = '';
                                    var locWindHailVal = '';
                                    var locWindHail = document.getElementById(LocWindHailDeductibleId);
                                    if (locWindHail) {
                                        locWindHailVal = locWindHail.value;
                                    }
                                    if (chkWindHail) {
                                        if (locWindHailVal == '' || locWindHailVal == '0') {
                                            chkWindHail.disabled = true;
                                            chkWindHail.checked = false;
                                        } else {
                                            chkWindHail.disabled = false;
                                        }
                                    }
                                } else {
                                    trWindHailRow.style.display = 'none';
                                }
                                break;
                            }
                        }
                    }

                } // End if (Chk.checked)
                else {
                    // COVERAGE CHECKBOX NOT CHECKED
                    switch (sender) {
                        case "BC": {
                            // BC COVERAGE IS NOT CHECKED
                            if (confirm('Are you sure you want to delete Building Coverage?') == true) {
                                if (chkBIC.checked) {
                                    // Need to show the BIC Specific Rates rows and populate them
                                    if (SpecificUseEligible) {
                                        trBICUseSpecific.style.display = '';
                                        trBICGroupIRow.style.display = '';
                                        trBICGroupIIRow.style.display = '';
                                        txtBICGroup1.value = txtBCGroup1.value;
                                        txtBICGroup2.value = txtBCGroup2.value;
                                    }
                                }
                            }
                            else {
                                Chk.checked = true;
                                if (DataRow) { DataRow.style.display = ''; }
                                if (DataRow2) { DataRow2.style.display = ''; }
                                if (InfoRow) { InfoRow.style.display = ''; }
                                if (showOwnerOccupiedPercentage.toUpperCase() == "TRUE") {
                                    if (OwnerOccupiedRow) { OwnerOccupiedRow.style.display = ''; }
                                } else {
                                    if (OwnerOccupiedRow) { OwnerOccupiedRow.style.display = 'none'; }
                                }
                                if (locationWindHailAvailable.toUpperCase() == "TRUE") {
                                    trWindHailRow.style.display = '';
                                } else {
                                    trWindHailRow.style.display = 'none';
                                }
                                return false
                            }
                            break;
                        }
                        case "BIC": {
                            // BIC COVERAGE IS NOT CHECKED
                            if (confirm('Are you sure you want to delete Business Income Coverage?') == true) {
                                if (chkBC.checked) {
                                    // Need to show the BC Specific Rates rows and populate them
                                    if (SpecificUseEligible) {
                                        trBCUseSpecific.style.display = '';
                                        trBCGroupIRow.style.display = '';
                                        trBCGroupIIRow.style.display = '';
                                        if (txtBICGroup1.value != '') { txtBCGroup1.value = txtBICGroup1.value; }
                                        if (txtBICGroup2.value != '') { txtBCGroup2.value = txtBICGroup2.value; }
                                    }
                                }
                            }
                            else {
                                Chk.checked = true;
                                if (DataRow) { DataRow.style.display = ''; }
                                if (DataRow2) { DataRow2.style.display = ''; }
                                if (InfoRow) { InfoRow.style.display = ''; }
                                return false
                            }
                            break;
                        }
                        case "PPC": {
                            // PPC COVERAGE IS NOT CHECKED
                            if (confirm('Are you sure you want to delete Personal Property Coverage?') == true) {
                                if (chkPPO.checked) {
                                    // If PPO coverage is present, display it's EQ Lookup Row
                                    if (chkPPOEQ.checked) {
                                        trPPOEQLookupRow.style.display = '';
                                        var hdnVal = document.getElementById(b.hdnPPCEQCC).value;
                                        var txtVal = document.getElementById(b.txtPPCEQDesc).value;
                                        var hdn = document.getElementById(b.hdnPPOEQCC);
                                        var txt = document.getElementById(b.txtPPOEQDesc);
                                        if (hdn && hdnVal) { hdn.value = hdnVal; }
                                        if (txt && txtVal) { txt.value = txtVal; }
                                    }
                                    // Need to show the PPO Specific Rates rows and populate them
                                    if (SpecificUseEligible) {
                                        trPPOUseSpecific.style.display = '';
                                        trPPOGroupIRow.style.display = '';
                                        trPPOGroupIIRow.style.display = '';
                                        txtPPOGroup1.value = txtPPCGroup1.value;
                                        txtPPOGroup2.value = txtPPCGroup2.value;
                                    }
                                }
                            }
                            else {
                                Chk.checked = true;
                                if (DataRow) { DataRow.style.display = ''; }
                                if (DataRow2) { DataRow2.style.display = ''; }
                                if (InfoRow) { InfoRow.style.display = ''; }
                                if (locationWindHailAvailable.toUpperCase() == "TRUE") {
                                    trWindHailRow.style.display = '';
                                } else {
                                    trWindHailRow.style.display = 'none';
                                }
                                return false
                            }
                            break;
                        }
                        case "PPO": {
                            // PPO COVERAGE IS NOT CHECKED
                            if (confirm('Are you sure you want to delete Personal Property of Others Coverage?') == true) {
                                if (chkPPC.checked) {
                                    // If PPO coverage is present, display it's EQ Lookup Row
                                    // Also copy the value from the PPO lookup if present
                                    if (chkPPCEQ.checked) {
                                        trPPCEQLookupRow.style.display = '';
                                        var hdnVal = document.getElementById(b.hdnPPOEQCC).value;
                                        var txtVal = document.getElementById(b.txtPPOEQDesc).value;
                                        var hdn = document.getElementById(b.hdnPPCEQCC);
                                        var txt = document.getElementById(b.txtPPCEQDesc);
                                        if (hdn && hdnVal) { hdn.value = hdnVal; }
                                        if (txt && txtVal) { txt.value = txtVal; }
                                    }
                                    // Need to show the PPC Specific Rates rows and populate them
                                    if (SpecificUseEligible) {
                                        trPPCUseSpecific.style.display = '';
                                        trPPCGroupIRow.style.display = '';
                                        trPPCGroupIIRow.style.display = '';
                                        txtPPCGroup1.value = txtPPCGroup1.value;
                                        txtPPCGroup2.value = txtPPCGroup2.value;
                                    }
                                }
                            }
                            else {
                                Chk.checked = true;
                                if (DataRow) { DataRow.style.display = ''; }
                                if (DataRow2) { DataRow2.style.display = ''; }
                                if (InfoRow) { InfoRow.style.display = ''; }
                                if (locationWindHailAvailable.toUpperCase() == "TRUE") {
                                    trWindHailRow.style.display = '';
                                } else {
                                    trWindHailRow.style.display = 'none';
                                }
                                return false
                            }
                            break;
                        }
                    }
                } // End coverage checkbox not checked
            }  // End if (Chk)
            else {
                // CHK is null!
                trUseSpecificRow.style.display = 'none';
                trGroupIRow.style.display = 'none';
                trGroupIIRow.style.display = 'none';
            } // End If Chk ELSE
        }  // End if (sender)


        return true;
    };

    // Returns true if the passed class code is eligible for specific rates, false if not
    this.IsBuildingEligibleForSpecificRates = function (ClassCode, ConstructionValue) {
        // 8-14-2018 Bug 28331
        // If Building Construction Type is one of the following...
        //   - MASONRY-NON COMBUSTIBLE   (id 14)
        //   - MODIFIED FIRE RESISTIVE   (id 15)
        //   - FIRE RESISTIVE   (id 16)
        // ...then we need to show specific rates regardless of what the class code is.
        if (ConstructionValue) {
            if (ConstructionValue == "14" || ConstructionValue == "15" || ConstructionValue == "16") { return true;}
        }

        // Check the class code
        if (ClassCode) {
            var NoSpecificRateCCs = new Array();
            NoSpecificRateCCs.push("0074");
            NoSpecificRateCCs.push("0075");
            NoSpecificRateCCs.push("0076");
            NoSpecificRateCCs.push("0077");
            NoSpecificRateCCs.push("0078");
            NoSpecificRateCCs.push("0196");
            NoSpecificRateCCs.push("0197");
            NoSpecificRateCCs.push("0198");
            NoSpecificRateCCs.push("0311");
            NoSpecificRateCCs.push("0312");
            NoSpecificRateCCs.push("0313");
            NoSpecificRateCCs.push("0321");
            NoSpecificRateCCs.push("0322");
            NoSpecificRateCCs.push("0323");
            NoSpecificRateCCs.push("0331");
            NoSpecificRateCCs.push("0745");
            NoSpecificRateCCs.push("0746");
            NoSpecificRateCCs.push("0747");
            NoSpecificRateCCs.push("0511");
            NoSpecificRateCCs.push("0512");
            NoSpecificRateCCs.push("0520");
            NoSpecificRateCCs.push("0531");
            NoSpecificRateCCs.push("0532");
            NoSpecificRateCCs.push("0541");
            NoSpecificRateCCs.push("0550");
            NoSpecificRateCCs.push("0561");
            NoSpecificRateCCs.push("0562");
            NoSpecificRateCCs.push("0563");
            NoSpecificRateCCs.push("0564");
            NoSpecificRateCCs.push("0565");
            NoSpecificRateCCs.push("0566");
            NoSpecificRateCCs.push("0567");
            NoSpecificRateCCs.push("0570");
            NoSpecificRateCCs.push("0580");
            NoSpecificRateCCs.push("0581");
            NoSpecificRateCCs.push("0582");
            NoSpecificRateCCs.push("0701");
            NoSpecificRateCCs.push("0702");
            NoSpecificRateCCs.push("0755");
            NoSpecificRateCCs.push("0756");
            NoSpecificRateCCs.push("0757");
            NoSpecificRateCCs.push("0831");
            NoSpecificRateCCs.push("0832");
            NoSpecificRateCCs.push("0833");
            NoSpecificRateCCs.push("0834");
            NoSpecificRateCCs.push("0841");
            NoSpecificRateCCs.push("0843");
            NoSpecificRateCCs.push("0844");
            NoSpecificRateCCs.push("0845");
            NoSpecificRateCCs.push("0846");
            NoSpecificRateCCs.push("0851");
            NoSpecificRateCCs.push("0852");
            NoSpecificRateCCs.push("0900");
            NoSpecificRateCCs.push("0911");
            NoSpecificRateCCs.push("0912");
            NoSpecificRateCCs.push("0913");
            NoSpecificRateCCs.push("0921");
            NoSpecificRateCCs.push("0922");
            NoSpecificRateCCs.push("0923");
            NoSpecificRateCCs.push("0931");
            NoSpecificRateCCs.push("0932");
            NoSpecificRateCCs.push("0933");
            NoSpecificRateCCs.push("0934");
            NoSpecificRateCCs.push("0940");
            NoSpecificRateCCs.push("0952");
            NoSpecificRateCCs.push("1000");
            NoSpecificRateCCs.push("1051");
            NoSpecificRateCCs.push("1052");
            NoSpecificRateCCs.push("1070");
            NoSpecificRateCCs.push("1150");
            NoSpecificRateCCs.push("1211");
            NoSpecificRateCCs.push("1212");
            NoSpecificRateCCs.push("1213");
            NoSpecificRateCCs.push("1220");
            NoSpecificRateCCs.push("1230");
            NoSpecificRateCCs.push("1400");
            NoSpecificRateCCs.push("1650");
            NoSpecificRateCCs.push("1700");
            NoSpecificRateCCs.push("1751");
            NoSpecificRateCCs.push("1752");
            NoSpecificRateCCs.push("0533");
            NoSpecificRateCCs.push("2200");
            NoSpecificRateCCs.push("2350");
            NoSpecificRateCCs.push("2459");
            NoSpecificRateCCs.push("2800");
            NoSpecificRateCCs.push("3409");
            NoSpecificRateCCs.push("4809");

            for (x = 0; x <= NoSpecificRateCCs.length - 1; ++x){
                var ccode = NoSpecificRateCCs[x];
                if (ClassCode == ccode) {
                    return false
                }
            }
            // If we got here then the class code IS eligible for specific rates
            return true;
        } // End of the class code loop
        // If we got here then 1) The construction was not eligible, and 2) the class code was not eligible
        return false;
    };

    // Handles clicks to the Building BIC coverage limit type radio buttons
    this.BICLimitTypeChanged = function (sender, rbId, ddMId, ddCId) {
        rb = document.getElementById(rbId);
        ddM = document.getElementById(ddMId);
        ddC = document.getElementById(ddCId);

        if (sender && rb && ddM && ddC) {
            if (rb.checked) {
                // checked
                switch (sender) {
                    case 'M': {
                        ddM.style.display = '';
                        ddC.style.display = 'none';
                        break;
                    }
                    case "C": {
                        ddM.style.display = 'none';
                        ddC.style.display = '';
                        break;
                    }
                }
            }
        }

        return true;
    }

    // If the Earthquake checkbox is not checked at the building level, hide and uncheck all of the building coverages earthquake checkboxes
    this.LocationEQCheckboxChanged = function (BldgNbr) {
        var b = Cpr.BuildingCoverageBindings[BldgNbr];

        var Chk = document.getElementById(b.chkINFEarthquake);
        var chkBC = document.getElementById(b.chkBC);
        //var BCEQRow = document.getElementById(b.trBCEQRow);
        var chkBIC = document.getElementById(b.chkBIC);
        var chkBICEQ = document.getElementById(b.chkBICEQ);
        var BICEQRow = document.getElementById(b.trBICEQRow);
        var chkPPC = document.getElementById(b.chkPPC);
        var PPCEQRow = document.getElementById(b.trPPCEQRow);
        var PPCEQLookupRow = document.getElementById(b.trPPCEQLookupRow);
        var chkPPCEQ = document.getElementById(b.chkPPCEQ);
        var chkPPO = document.getElementById(b.chkPPO);
        var PPOEQrow = document.getElementById(b.trPPOEQRow);
        var PPOEQLookupRow = document.getElementById(b.trPPOEQLookupRow);
        var chkPPOEQ = document.getElementById(b.chkPPOEQ);
        var INFEQLookupRow = document.getElementById(b.trINFEQLookupRow);

        // Hide everything 
        //if (BCEQRow) { BCEQRow.style.display = 'none'; }
        if (BICEQRow ) { BICEQRow.style.display = 'none'; }
        if (PPCEQRow) { PPCEQRow.style.display = 'none'; }
        if (PPOEQrow) { PPOEQrow.style.display = 'none'; }
        if (PPCEQLookupRow) { PPCEQLookupRow.style.display = 'none'; }
        if (PPOEQLookupRow) { PPOEQLookupRow.style.display = 'none'; }
        if (INFEQLookupRow) { INFEQLookupRow.style.display = 'none'; }

        // Show the eartquake rows if the main earthquake checkbox is checked and the coverage is selected
        if (Chk) {
            if (Chk.checked) {
                if (INFEQLookupRow) { INFEQLookupRow.style.display = ''; }
                //if (BCEQRow && chkBC.checked) {
                //    BCEQRow.style.display = '';
                //}
                if (BICEQRow && chkBIC.checked) {
                    BICEQRow.style.display = '';
                }
                if (PPCEQRow && chkPPC.checked) {
                    PPCEQRow.style.display = '';
                }
                if (PPOEQrow && chkPPO.checked) {
                    PPOEQrow.style.display = '';
                }
            }
            else {
                // UNCHECKED
                if (confirm('Are you sure you want to delete the building Earthquake coverage?') == true) {
                    // INFO EQ checkbox is NOT checked and user wants it that way
                    // Uncheck the EQ checkboxes
                    if (chkBICEQ && chkPPCEQ && chkPPOEQ) {
                        chkBICEQ.checked = false;
                        chkPPCEQ.checked = false;
                        chkPPOEQ.checked = false;
                    }
                }
                else {
                    // User wants to cancel removing the coverage
                    Chk.checked = true
                    if (INFEQLookupRow) { INFEQLookupRow.style.display = ''; }
                    if (BICEQRow && chkBIC.checked) {
                        BICEQRow.style.display = '';
                    }
                    if (PPCEQRow && chkPPC.checked) {
                        PPCEQRow.style.display = '';
                    }
                    if (PPOEQrow && chkPPO.checked) {
                        PPOEQrow.style.display = '';
                    }
                }

            }
        }
        return true;
    };

    this.CoverageCheckboxChanged = function (CovChkId, DataRow1Id, DataRow2Id, DataRow3Id, InfoRow1Id, InfoRow2Id) {
        var CovChk = document.getElementById(CovChkId);
        var DataRow1 = document.getElementById(DataRow1Id);
        var DataRow2 = document.getElementById(DataRow2Id);
        var DataRow3 = document.getElementById(DataRow3Id);
        var InfoRow1 = document.getElementById(InfoRow1Id);
        var InfoRow2 = document.getElementById(InfoRow2Id);

        // Hide everything 
        if (DataRow1) { DataRow1.style.display = 'none'; }
        if (DataRow2) { DataRow2.style.display = 'none'; }
        if (DataRow3) { DataRow3.style.display = 'none'; }
        if (InfoRow1) { InfoRow1.style.display = 'none'; }
        if (InfoRow2) { InfoRow2.style.display = 'none'; }

        // Show if the element exists
        if (CovChk) {
            if (CovChk.checked) {
                if (DataRow1) { DataRow1.style.display = ''; }
                if (DataRow2) { DataRow2.style.display = ''; }
                if (DataRow3) { DataRow3.style.display = ''; }
                if (InfoRow1) { InfoRow1.style.display = ''; }
                if (InfoRow2) { InfoRow2.style.display = ''; }
            }
        }
    };

    /// Handles clicks to the CPR/CPP enhancement checkboxes
    this.EnhancementCheckboxChanged = function(sender, chkID, DataRowId, InfoRowId) {
        var chkEnh = document.getElementById(chkID);
        var trData = document.getElementById(DataRowId);
        var trInfo = document.getElementById(InfoRowId);
        var ans = false;

        if (chkEnh && sender) {
            if (chkEnh.checked) {
                return true;
            }
            else {
                switch (sender.toUpperCase()) {
                    case "PROP": {
                        //ans = confirm('Are you sure you want to delete the Property Enhancement? WWW');
                        ans = confirm('Are you sure you want to delete the Property Enhancement?');
                        break;
                    }
                    case "MANUF": {
                        ans = confirm('Are you sure you want to delete the Manufacturers Enhancement?');
                        break;
                    }
                    case "CONT": {
                        ans = confirm('Are you sure you want to delete the Contractors Enhancement?');
                        break;
                    }
                    case "LIAB": {
                        ans = confirm('Are you sure you want to delete the Liability Enhancement?');
                        break;
                    }
                }            
                if (ans) {
                    if (ans == true) {
                        if (trData) { trData.style.display = 'none'; }
                        if (trInfo) { trInfo.stye.display = 'none'; }
                        return true;
                    }
                    else {
                        chkEnh.checked = true;
                        return false;
                    }
                }
            }
        } return true;
    }

    // Called when the GroupI/GroupII field values change
    // When entering BIC/BC the one we're NOT entering gets hidden.
    // PPC/PPO also work the same
    this.GroupFieldValueChanged = function(BldgNbr, sender){
        var b = Cpr.BuildingCoverageBindings[BldgNbr];

        // Get the controls
        var chkBC = document.getElementById(b.chkBC);
        var txtBCGroupI = document.getElementById(b.txtBCGroup1);
        var txtBCGroupII = document.getElementById(b.txtBCGroup2);
        var trBCGroupIRow = document.getElementById(b.trBCGroupIRow);
        var trBCGroupIIRow = document.getElementById(b.trBCGroupIIRow);

        var chkBIC = document.getElementById(b.chkBIC);
        var txtBICGroupI = document.getElementById(b.txtBICGroup1);
        var txtBICGroupII = document.getElementById(b.txtBICGroup2);
        var trBICGroupIRow = document.getElementById(b.trBICGroupIRow);
        var trBICGroupIIRow = document.getElementById(b.trBICGroupIIRow);

        var chkPPC = document.getElementById(b.chkPPC);
        var txtPPCGroupI = document.getElementById(b.txtPPCGroup1);
        var txtPPCGroupII = document.getElementById(b.txtPPCGroup2);
        var trPPCGroupIRow = document.getElementById(b.trPPCGroupIRow);
        var trPPCGroupIIRow = document.getElementById(b.trPPCGroupIIRow);

        var chkPPO = document.getElementById(b.chkPPO);
        var txtPPOGroupI = document.getElementById(b.txtPPOGroup1);
        var txtPPOGroupII = document.getElementById(b.txtPPOGroup2);
        var trPPOGroupIRow = document.getElementById(b.trPPOGroupIRow);
        var trPPOGroupIIRow = document.getElementById(b.trPPOGroupIIRow);

        var hdnBCUseVisible = document.getElementById(b.BCUseSpecificVisible);
        var hdnBICUseVisible = document.getElementById(b.BICUseSpecificVisible);
        var hdnPPCUseVisible = document.getElementById(b.PPCUseSpecificVisible);
        var hdnPPOUseVisible = document.getElementById(b.PPOUseSpecificVisible);

        switch (sender) {
            case "BC": {
                if (txtBCGroupI.value != '' || txtBCGroupII.value != '') {
                    txtBICGroupI.value = '';
                    txtBICGroupII.value = ''
                    if (chkBIC.checked) {
                        trBICGroupIRow.style.display = 'none';
                        trBICGroupIIRow.style.display = 'none';
                        hdnBICUseVisible.value = 'false';
                    }
                }
                else {
                    if (chkBIC.checked) {
                        trBICGroupIRow.style.display = '';
                        trBICGroupIIRow.style.display = '';
                        hdnBICUseVisible.value = 'true';
                    }
                }
                break;
            }
            case "BIC": {
                if (txtBICGroupI.value != '' || txtBICGroupII.value != '') {
                    txtBCGroupI.value = '';
                    txtBCGroupII.value = ''
                    if (chkBC.checked) {
                        trBCGroupIRow.style.display = 'none';
                        trBCGroupIIRow.style.display = 'none';
                        hdnBCUseVisible.value = 'false';
                    }
                }
                else {
                    if (chkBC.checked) {
                        trBCGroupIRow.style.display = '';
                        trBCGroupIIRow.style.display = '';
                        hdnBCUseVisible.value = 'true';
                    }
                }
                break;
            }
            case "PPC": {
                if (txtPPCGroupI.value != '' || txtPPCGroupII.value != '') {
                    txtPPOGroupI.value = '';
                    txtPPOGroupII.value = ''
                    if (chkPPO.checked) {
                        trPPOGroupIRow.style.display = 'none';
                        trPPOGroupIIRow.style.display = 'none';
                        hdnPPOUseVisible.value = 'false';
                    }
                }
                else {
                    if (chkPPO.checked) {
                        trPPOGroupIRow.style.display = '';
                        trPPOGroupIIRow.style.display = '';
                        hdnPPOUseVisible.value = 'true';
                    }
                }
                break;
            }
            case "PPO": {
                if (txtPPOGroupI.value != '' || txtPPOGroupII.value != '') {
                    txtPPCGroupI.value = '';
                    txtPPCGroupII.value = ''
                    if (chkPPC.checked) {
                        trPPCGroupIRow.style.display = 'none';
                        trPPCGroupIIRow.style.display = 'none';
                        hdnPPCUseVisible.value = 'false';
                    }
                }
                else {
                    if (chkPPC.checked) {
                        trPPCGroupIRow.style.display = '';
                        trPPCGroupIIRow.style.display = '';
                        hdnPPCUseVisible.value = 'true';
                    }
                }
                break;
            }
        }

        return true;
    };

    // Called when the PPC/PPO Earthquake checkboxes are changed
    this.EQClassificationCheckboxChanged = function(BldgNbr, sender){
        var b = Cpr.BuildingCoverageBindings[BldgNbr];

        // Get the controls
        var chkPPCEQ = document.getElementById(b.chkPPCEQ);
        var trPPCEQLookupRow = document.getElementById(b.trPPCEQLookupRow);
        var chkPPOEQ = document.getElementById(b.chkPPOEQ);
        var trPPOEQLookupRow = document.getElementById(b.trPPOEQLookupRow);
        var txtPPCEQDesc = document.getElementById(b.txtPPCEQDesc);
        var txtPPOEQDesc = document.getElementById(b.txtPPOEQDesc);

        if (chkPPCEQ && trPPCEQLookupRow && chkPPOEQ && trPPOEQLookupRow && txtPPCEQDesc && txtPPOEQDesc) {
            // Hide the lookup rows
            trPPCEQLookupRow.style.display = 'none';
            trPPOEQLookupRow.style.display = 'none';
            var EQText = '';
            if (txtPPCEQDesc.value != '') {
                EQText = txtPPCEQDesc.value;
            }
            else {
                if (txtPPOEQDesc.value != '') {
                    EQText = txtPPOEQDesc.value;
                }
            } 

            if (chkPPCEQ.checked && chkPPOEQ.checked) {
                // Both PPC and PPO EQ checkboxes checked
                trPPCEQLookupRow.style.display = '';
                txtPPCEQDesc.value = EQText;
            }
            else {
                if (chkPPCEQ.checked) {
                    // Only PPC EQ checked
                    trPPCEQLookupRow.style.display = '';
                    txtPPCEQDesc.value = EQText;
                    //if (txtPPOEQDesc.value != ''){txtPPCEQDesc.value = txtPPOEQDesc.value}
                }
                else {
                    if (chkPPOEQ.checked) {
                        // Only PPO EQ Checked
                        trPPOEQLookupRow.style.display = '';
                        txtPPOEQDesc.value = EQText;
                        //if (txtPPCEQDesc.value != '') { txtPPOEQDesc.value = txtPPCEQDesc.value }
                    }
                        else {
                    }
                }
            } 
        }
    };

    // Truncates and adds 3 ellipses to the passed string if it's length is greater than the passed max length.
    // If the passed string's length is less that the passed max length just returns what was passed.
    // NOTE: If this function adds ellipses the returned string's length will be max length + 3 (because of the ellipses)
    this.TruncateEllipse = function (instr, maxlen) {
        if (instr && maxlen) {
            if (instr.length > maxlen) {
                return instr.substring(0, maxlen) + '...';
            }
            else { return instr; }
        }
        else { return ''; }
    };

    this.UiBindings = new Array();
    this.BuildingClassCodeUiBinding = function (BldgNdx, VRLobId, VRProgramId, TgtDia_Id, SrcDIA_Id, TgtTxtClassCodeId, SrcTxtClassCodeId, SrcYardRateRowId, SrcDdYardRateId, TgtHdnYardRateId, TgtTxtDescriptionId, SrcTxtDescriptionId, SrcApplyButtonId, TgtHdnPMAId, SrcDdPMAId, TgtHdnGroupRateId, SrcTxtGroupRateId, TgtHdnClassLimitId, SrcTxtClassLimitId, divFootNoteId, TgtBCUseSpecificRow, TgtBCUseSpecificInfoRow, TgtBCUseSpecificGroupIRow, TgtBCUseSpecificGroupIIRow, TgtBICUseSpecificRow, TgtBICUseSpecificInfoRow, TgtBICUseSpecificGroupIRow, TgtBICUseSpecificGroupIIRow, TgtPPCUseSpecificRow, TgtPPCUseSpecificInfoRow, TgtPPCUseSpecificGroupIRow, TgtPPCUseSpecificGroupIIRow, TgtPPOUseSpecificRow, TgtPPOUseSpecificInfoRow, TgtPPOUseSpecificGroupIRow, TgtPPOUseSpecificGroupIIRow, HdnClassCodeId, HdnDescriptionId, HdnPMAId, HdnPMAsId, HdnDIAId_Id, HdnGroupRateId, HdnClassLimitId, HdnFootNoteId, trYardRateValidationRowId, trPMAValidationRowId, divCCInfoId, SrcTrFootNoteInfoRowId ) {
        // Building Index 
        this.BuildingIndex = BldgNdx;
        this.LobId = VRLobId;
        this.ProgramId = VRProgramId;

        // Target controls - These are the controls on the BUILDING control that will be populated when we come back from the class code lookup
        this.TargetTxtClassCode = TgtTxtClassCodeId;
        this.TargetTxtDescription = TgtTxtDescriptionId;
        this.TargetHdnPMA = TgtHdnPMAId;
        this.TargetHdnGroupRate = TgtHdnGroupRateId;
        this.TargetHdnClassLimit = TgtHdnClassLimitId;
        this.TargetDIA_ID = TgtDia_Id;
        this.TargetHdnYardRateId = TgtHdnYardRateId;

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
        this.SourceYardRateRowId = SrcYardRateRowId;
        this.SourceDdlYardRate = SrcDdYardRateId;
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
        this.SourceYardRateValidationRow = trYardRateValidationRowId;
        this.SourcePMAValidationRow = trPMAValidationRowId;

        // Class Code Array
        this.ccArray = new Array();
    };

    //----------------------------
    this.EQUiBindings = new Array();
    this.BuildingEQClassificationUIBinding = function (LookupTypeVal, LocNdx, BldgNdx, TgtTxtClassCodeDescId, TgtDia_Id, TgtHdnRateGroup, SrcHdnId, SrcHdnRateGroupId, SrcHdnDescId, PPCDataRow1Id, PPODataRow1Id) {
        this.LocationIndex = LocNdx;
        this.BuildingIndex = BldgNdx;

        this.LookupType = LookupTypeVal;

        // Target controls - These are the controls on the BUILDING control that will be populated when we come back from the class code lookup
        this.TargetTxtClassificationDesc = TgtTxtClassCodeDescId;
        this.TargetDIA_ID = TgtDia_Id;
        this.TargetHdnRateGroup = TgtHdnRateGroup;

        // Source Controls - These are the controls on the class code lookup control whose values will be copied to the building control
        this.SourceHdnId = SrcHdnId;
        this.SourceHdnRateGroup = SrcHdnRateGroupId;
        this.SourceHdnDescription = SrcHdnDescId;

        // Earthquake Rows = PPC & PPO
        if (LookupTypeVal == "PPC") {
            this.PPC_DataRow1 = PPCDataRow1Id;
            this.PPO_DataRow1 = null;
        }
        if (LookupTypeVal == "PPO") {
            this.PPO_DataRow1 = PPODataRow1Id;
            this.PPC_DataRow1 = null;
        }
    };

    this.BuildingCoverageBindings = new Array();
    this.BuildingCoverageUiBinding = function (BldgNdx, trINFEQLookupRowId, ddBCCOLId, ddBCCOINSId, ddBCVALId, ddBCDEDId, txtBCGroup1Id, txtBCGroup2Id, ddBICCOLId, txtBICGroup1Id, txtBICGroup2Id, trBICEQRowId, chkBICEQId, ddPPCCOLId, ddPPCCOINSId, ddPPCVALId, ddPPCDEDId, txtPPCGroup1Id, txtPPCGroup2Id, trPPCEQRowId, ddPPOCOLId, ddPPOCOINSId, ddPPOVALId, ddPPODEDId, txtPPOGroup1Id, txtPPOGroup2Id, trPPOEQRowId, chkBCAgreedAmountId, chkPPCAgreedAmountId, trBCGroupIRowId, trBCGroupIIRowId, trBICGroupIRowId, trBICGroupIIRowId, trPPCGroupIRowId, trPPCGroupIIRowId, trPPOGroupIRowId, trPPOGroupIIRowId, chkBCId, chkBICId, chkPPCId, chkPPOId, hdnBCUseSpecificVisibleId, hdnBICUseSpecificVisibleId, hdnPPCUseSpecificVisibleId, hdnPPOUseSpecificVisibleId, chkPPCEQId, txtPPCEQDescId, trPPCEQLookupRowId, hdnPPCEQCCId, chkPPOEQId, txtPPOEQDescId, trPPOEQLookupRowId, hdnPPOEQCCId, txtINFClassCodeId, txtINFDescriptionId, ddINFConstructionId, chkINFEarthquakeId, trBCUseSpecificRowId, trBICUseSpecificRowId, trPPCUseSpecificRowId, trPPOUseSpecificRowId, chkBCUseSpecificId, chkBICUseSpecificId, chkPPCUseSpecificId, chkPPOUseSpecificId, trBCUseSpecificInfoRowId, trBICUseSpecificInfoRowId, trPPCUseSpecificInfoRowId, trPPOUseSpecificInfoRowId, hdnPPCSpecificUseCheckedId, hdnPPOSpecificUseCheckedId, hdnBCSpecificUseCheckedId, hdnBICSpecificUseCheckedId, trBCBlanketRowId, chkBCBlanketId, trPPCBlanketRowId, chkPPCBlanketId, trPPOBlanketRowId, chkPPOBlanketId, trBCBlanketInfoRowId, trPPCBlanketInfoRowId, trPPOBlanketInfoRowId, hdnAgreedBCId, hdnAgreedPPCId, trBCWindHailRowId, trPPCWindHailRowId, trPPOWindHailRowId, chkBCWindHailId, chkPPCWindHailId, chkPPOWindHailId) {
        // Building Index
        this.BuildingIndex = BldgNdx;

        // Building Info Section
        this.txtINFClassCode = txtINFClassCodeId;
        this.txtINFDescription = txtINFDescriptionId;
        this.ddINFConstruction = ddINFConstructionId;
        this.chkINFEarthquake = chkINFEarthquakeId;
        this.trINFEQLookupRow = trINFEQLookupRowId;

        // Building Coverage
        this.chkBC = chkBCId;
        this.ddBCCOL = ddBCCOLId;
        this.ddBCCOINS = ddBCCOINSId;
        this.ddBCVAL = ddBCVALId;
        this.ddBCDED = ddBCDEDId;
        this.txtBCGroup1 = txtBCGroup1Id;
        this.txtBCGroup2 = txtBCGroup2Id;
        this.chkBCAgreedAmount = chkBCAgreedAmountId;
        this.trBCUseSpecificRow = trBCUseSpecificRowId;
        this.trBCUseSpecificInfoRow = trBCUseSpecificInfoRowId;
        this.chkBCUseSpecific = chkBCUseSpecificId;
        this.trBCGroupIRow = trBCGroupIRowId;
        this.trBCGroupIIRow = trBCGroupIIRowId;
        this.trBCBlanketRow = trBCBlanketRowId;
        this.chkBCBlanket = chkBCBlanketId;
        this.trBCBlanketInfoRow = trBCBlanketInfoRowId;
        this.hdnAgreedBC = hdnAgreedBCId;
        this.hdnBCUseSpecificChecked = hdnBCSpecificUseCheckedId;
        //this.trBCEQRow = trBCEQRowId;
        this.trBCWindHailRow = trBCWindHailRowId;
        this.chkBCWindHail = chkBCWindHailId;

        // Business Income Coverage
        this.chkBIC = chkBICId;
        this.ddBICCOL = ddBICCOLId;
        this.txtBICGroup1 = txtBICGroup1Id;
        this.txtBICGroup2 = txtBICGroup2Id;
        this.trBICUseSpecificRow = trBICUseSpecificRowId;
        this.trBICUseSpecificInfoRow = trBICUseSpecificInfoRowId;
        this.chkBICUseSpecific = chkBICUseSpecificId;
        this.trBICGroupIRow = trBICGroupIRowId;
        this.trBICGroupIIRow = trBICGroupIIRowId;
        this.trBICEQRow = trBICEQRowId;
        this.chkBICEQ = chkBICEQId;
        this.hdnBICUseSpecificChecked = hdnBICSpecificUseCheckedId;

        // Personal Property Coverage
        this.chkPPC = chkPPCId;
        this.ddPPCCOL = ddPPCCOLId;
        this.ddPPCCOINS = ddPPCCOINSId;
        this.ddPPCVAL = ddPPCVALId;
        this.ddPPCDED = ddPPCDEDId;
        this.txtPPCGroup1 = txtPPCGroup1Id;
        this.txtPPCGroup2 = txtPPCGroup2Id;
        this.chkPPCAgreedAmount = chkPPCAgreedAmountId;
        this.trPPCUseSpecificRow = trPPCUseSpecificRowId;
        this.chkPPCUseSpecific = chkPPCUseSpecificId;
        this.trPPCUseSpecificInfoRow = trPPCUseSpecificInfoRowId;
        this.trPPCGroupIRow = trPPCGroupIRowId;
        this.trPPCGroupIIRow = trPPCGroupIIRowId;
        this.chkPPCEQ = chkPPCEQId;
        this.txtPPCEQDesc = txtPPCEQDescId;
        this.trPPCEQLookupRow = trPPCEQLookupRowId;
        this.hdnPPCEQCC = hdnPPCEQCCId;
        this.trPPCEQRow = trPPCEQRowId;
        this.hdnPPCUseSpecificChecked = hdnPPCSpecificUseCheckedId;
        this.trPPCBlanketRow = trPPCBlanketRowId;
        this.chkPPCBlanket = chkPPCBlanketId;
        this.trPPCBlanketInfoRow = trPPCBlanketInfoRowId;
        this.hdnAgreedPPC = hdnAgreedPPCId;
        this.trPPCWindHailRow = trPPCWindHailRowId;
        this.chkPPCWindHail = chkPPCWindHailId;

        // Personal Property of Others Coverage
        this.chkPPO = chkPPOId;
        this.ddPPOCOL = ddPPOCOLId;
        this.ddPPOCOINS = ddPPOCOINSId;
        this.ddPPOVAL = ddPPOVALId;
        this.ddPPODED = ddPPODEDId;
        this.txtPPOGroup1 = txtPPOGroup1Id;
        this.txtPPOGroup2 = txtPPOGroup2Id;
        this.trPPOUseSpecificRow = trPPOUseSpecificRowId;
        this.trPPOUseSpecificInfoRow = trPPOUseSpecificInfoRowId;
        this.chkPPOUseSpecific = chkPPOUseSpecificId;
        this.trPPOGroupIRow = trPPOGroupIRowId;
        this.trPPOGroupIIRow = trPPOGroupIIRowId;
        this.chkPPOEQ = chkPPOEQId;
        this.txtPPOEQDesc = txtPPOEQDescId;
        this.trPPOEQLookupRow = trPPOEQLookupRowId;
        this.hdnPPOEQCC = hdnPPOEQCCId;
        this.trPPOEQRow = trPPOEQRowId;
        this.hdnPPOUseSpecificChecked = hdnPPOSpecificUseCheckedId;
        this.trPPOBlanketRow = trPPOBlanketRowId;
        this.chkPPOBlanket = chkPPOBlanketId;
        this.trPPOBlanketInfoRow = trPPOBlanketInfoRowId;
        this.trPPOWindHailRow = trPPOWindHailRowId;
        this.chkPPOWindHail = chkPPOWindHailId;

        // Hidden Fields
        this.BCUseSpecificVisible = hdnBCUseSpecificVisibleId;
        this.BICUseSpecificVisible = hdnBICUseSpecificVisibleId;
        this.PPCUseSpecificVisible = hdnPPCUseSpecificVisibleId;
        this.PPOUseSpecificVisible = hdnPPOUseSpecificVisibleId;

        return true;
    };

    this.checkBlanketBoxUpdates = function () {
        $('.parentLocation').each(function () {
            var HasBlanket = false;

            $(this).find('input[id*=chkBCBlanketApplied]').each(function () {
                if ($(this).prop('checked')) {
                    HasBlanket = true;
                    return false;
                }
            });
            if (HasBlanket == false) {
                $(this).find('input[id*=chkPPCBlanketApplied]').each(function () {
                    if ($(this).prop('checked')) {
                        HasBlanket = true;
                        return false;
                    }
                });
            }
            if (HasBlanket == false) {
                $(this).find('input[id*=chkPPOBlanketApplied]').each(function () {
                    if ($(this).prop('checked')) {
                        HasBlanket = true;
                        return false;
                    }
                });
            }
            var PropertyDeductible = $(this).find('select[id*=ddLocationPropertyDeductible]').first();
            if (BlanketDeductibleId == undefined) {
                BlanketDeductibleId = "9";
            }
            if (HasBlanket == true) {
                $(PropertyDeductible).val(BlanketDeductibleId);
                ifm.vr.ui.SingleElementDisable($(PropertyDeductible).prop('id'));
            }
            else {
                ifm.vr.ui.SingleElementEnable($(PropertyDeductible).prop('id'));
            }

        });
    };

};

$(document).ready(function () {

    // Performs these actions if the location's wind hail drop down changes
    $('[id*=ddWindHailDeductible]').on('change', function () {
        checkWindHailApplied($(this))
        LocationWindHailPercentChanged($(this))
    });

    $('[id*=ddOwnerOccupiedPercentage]').on('change', function () {
        BuildingOccupiedPercentChanged($(this))
    });

    //Check Risk Grade on page load for IL Condo's
    checkCondoItems()
    checkMineSub() //Added 11/28/18 for multi state MLW
    

    // Perform these actions if the ZIP, State, County, or Building Coverage checkbox change
    $('[id*=txtZipCode],[id*=ddStateAbbrev],[id*=txtGaragedCounty],[id*=chkBuildingCoverage]').on('change', function () {
        checkMineSub()
    });

    // Recheck Risk Grade for IL Condo's if any of these items change. - Added 11/28/18 for multi state MLW
    $('[id*=txtZipCode],[id*=ddStateAbbrev],[id*=chkBuildingCoverage]').on('change', function () {
        checkCondoItems()
    });

    function LocationWindHailPercentChanged(ddWindHailDed) {
        if (locationWindHailDefaultingAvailable.toUpperCase() == "TRUE") {

            var valueToRemove = "0";

            var parentLocation = $(ddWindHailDed).closest('.parentLocation')[0];
            var buildings = $(parentLocation).find('.building');

            $.each(buildings, function () {
                var building = $(this);
                var ownerPercentDeductible = $(building).find('[id*=ddOwnerOccupiedPercentage]');
                var windHailBCCheckBox = $(building).find('[id*=chkBCWindHail]');
                var windHailText = $(building).find('[id*=trBCWindHailText]');
                var classCodeText = $(building).closest('.building').prevAll('.classcode').first();
                var locationWindHailDeductible = $(parentLocation).find('[id*=ddWindHailDeductible]');

                if (ownerPercentDeductible && windHailBCCheckBox.length && windHailText.length && classCodeText.length && locationWindHailDeductible) {
                    var classCodeValue = classCodeText.find('input').val();
                    var locationWindHailDeductibleValue = locationWindHailDeductible.val();
                    var ownerPercentDeductibleValue = ownerPercentDeductible.val();

                    var excludedClassCodes = ["0196", "0197", "0198", "0311", "0312", "0313", "0331", "0332", "0333"];
                    var isOwnerOccupied = ownerPercentDeductibleValue == "30" || ownerPercentDeductibleValue == "31";
                    if (locationWindHailDeductibleValue) {
                        if (classCodeValue && !excludedClassCodes.includes(classCodeValue)) {
                            if (isOwnerOccupied) {
                                $(windHailBCCheckBox).prop('checked', true).prop('disabled', true);
                                if (locationWindHailDeductibleValue == '0') {
                                    locationWindHailDeductible.val('32');
                                }
                                $(windHailText).hide();
                                locationWindHailDeductible.find('option').each(function () {
                                    if ($(this).val() === valueToRemove) {
                                        $(this).remove();
                                        return false; // break loop
                                    }
                                });
                                $(windHailText).show();
                            } else {
                                if (locationWindHailDeductibleValue == '0') {
                                    $(windHailBCCheckBox).prop('checked', false).prop('disabled', true);
                                } else {
                                    $(windHailBCCheckBox).prop('checked', false).prop('disabled', false);
                                    locationWindHailDeductible.prepend(
                                        $('<option>', {
                                            value: valueToRemove,
                                            text: 'N/A'
                                        })
                                    );
                                    locationWindHailDeductible.val(locationWindHailDeductibleValue);
                                }
                                $(windHailText).hide();
                            }
                        } else {
                            if (locationWindHailDeductibleValue == '0') {
                                $(windHailBCCheckBox).prop('checked', false).prop('disabled', true);
                            } else {
                                $(windHailBCCheckBox).prop('checked', false).prop('disabled', false);
                            }
                            $(windHailText).hide();
                        }

                    }
                }
            });
        } 
    }
    
    function BuildingOccupiedPercentChanged(ddBuildingOwnerOccupiedDed) {

        if (locationWindHailDefaultingAvailable.toUpperCase() == "TRUE") {
            var valueToRemove = "0";

            var ownerPercentDeductibleValue = $(ddBuildingOwnerOccupiedDed).val();
            var parentLocation = $(ddBuildingOwnerOccupiedDed).closest('.parentLocation')[0];
            var building = $(ddBuildingOwnerOccupiedDed).closest('.building')[0];
            var windHailBCCheckBox = $(building).find('[id*=chkBCWindHail]');
            var windHailText = $(building).find('[id*=trBCWindHailText]');
            var classCodeText = $(building).closest('.building').prevAll('.classcode').first();
            var locationWindHailDeductible = $(parentLocation).find('[id*=ddWindHailDeductible]');

            if (locationWindHailDeductible && ownerPercentDeductibleValue && windHailBCCheckBox.length && windHailText.length && classCodeText.length) {
                var locationWindHailDeductibleValue = locationWindHailDeductible.val();
                var classCodeTextValue = classCodeText.find('input').val();
                if (locationWindHailDeductibleValue) {
                    if (classCodeTextValue && !["0196", "0197", "0198", "0311", "0312", "0313", "0331", "0332", "0333"].includes(classCodeTextValue)) {

                        if (ownerPercentDeductibleValue && (ownerPercentDeductibleValue == "30" || ownerPercentDeductibleValue == "31")) {
                            $(windHailBCCheckBox).prop('checked', true).prop('disabled', true);
                            if (locationWindHailDeductibleValue == '0') {
                                locationWindHailDeductible.val('32');
                            }
                            $(windHailText).hide();
                            locationWindHailDeductible.find('option').each(function () {
                                if ($(this).val() === valueToRemove) {
                                    $(this).remove();
                                    return false; // break loop
                                }
                            });
                            $(windHailText).show();
                        } else {
                            if (locationWindHailDeductibleValue == '0') {
                                $(windHailBCCheckBox).prop('disabled', true).prop('checked', false);
                            } else {
                                $(windHailBCCheckBox).prop('disabled', false).prop('checked', false);
                                locationWindHailDeductible.prepend(
                                    $('<option>', {
                                        value: valueToRemove,
                                        text: 'N/A'
                                    })
                                );
                                locationWindHailDeductible.val(locationWindHailDeductibleValue);
                            }
                            $(windHailText).hide();
                        }
                    } else {
                        if (locationWindHailDeductibleValue == '0') {
                            $(windHailBCCheckBox).prop('disabled', true).prop('checked', false);
                        } else {
                            $(windHailBCCheckBox).prop('disabled', false).prop('checked', false);
                        }
                        $(windHailText).hide();
                    }
                } else {
                    $(windHailText).hide();
                }
            }
        } else {
            if (windHailText) {
                $(windHailText).hide();
            }
        }
    }
    
    Cpr.checkBlanketBoxUpdates()

    function checkWindHailApplied(ddWindHailDed) {
        //allows for the location's wind hail deductible % to be applied to the pito coverage, building cov, pers prop cov, or pers prop of others
        if (locationWindHailAvailable.toUpperCase() == "TRUE") {
            var locationWindHailDedValue = $(ddWindHailDed).val();
            var parentLocation = $(ddWindHailDed).closest('.parentLocation')[0];
            var windHailPitoCheckboxkList = $(parentLocation).find('[id*=chkWindHail]');
            var windHailBCCheckboxkList = $(parentLocation).find('[id*=chkBCWindHail]');
            var windHailPPCCheckboxkList = $(parentLocation).find('[id*=chkPPCWindHail]');
            var windHailPPOCheckboxkList = $(parentLocation).find('[id*=chkPPOWindHail]');
            if (locationWindHailDedValue == '' || locationWindHailDedValue == '0') {
                setWindHailList(windHailPitoCheckboxkList, true)
                setWindHailList(windHailBCCheckboxkList, true)
                setWindHailList(windHailPPCCheckboxkList, true)
                setWindHailList(windHailPPOCheckboxkList, true)
            } else {
                setWindHailList(windHailPitoCheckboxkList, false)
                setWindHailList(windHailBCCheckboxkList, false)
                setWindHailList(windHailPPCCheckboxkList, false)
                setWindHailList(windHailPPOCheckboxkList, false)
            }
        }
    }

    function setWindHailList(myList, disabledStatus) {
        $(myList).each(function () {
            if (disabledStatus == true) {
                $(this).prop('checked', false);
                $(this).prop("disabled", true);
            } else {
                $(this).prop("disabled", false);
            }
        })
    }

    //Given any element under a location and this will return the value for the State Selection
    function getLocationStateValue(element) {
        return $(element).closest('[id*=divContents]').find('[id*=ctlProperty_Address]').find('[id*=ddStateAbbrev]').val();
    }

    //Given any element under a location and this will return the value for the ZIP Code Text
    function getLocationZipText(element) {
        return $(element).closest('[id*=divContents]').find('[id*=ctlProperty_Address]').find('[id*=txtZipCode]').val();
    }

    //Given any element under a location and this will return the value for the County Text
    function getLocationCountyText(element) {
        return $(element).closest('[id*=divContents]').find('[id*=ctlProperty_Address]').find('[id*=txtGaragedCounty]').val();
    }

    //Specific for IL Condo RiskGrade checking.  Certain RG's must be defaulted.
    function checkCondoItems() {
        $('select[class*=IL_CondoRiskGrade]').each(function () {
            if ($(this).closest('[id*=tblBuildingCoverages]').find('[id*=chkBuildingCoverage]').prop('checked')) {
                //Check State for IL = 15
                if (getLocationStateValue(this) == 15) {
                    //Cause of Loss
                    if ($(this).hasClass("IL_CondoRiskGrade_Col")) {
                        //disable + set default(3)
                        $(this).prop("disabled", true);
                        $(this).val(3) //Special Form Including Theft
                    }
                    // Valuation
                    if ($(this).hasClass("IL_CondoRiskGrade_Val")) {
                        //disable + set default(1)
                        $(this).prop("disabled", true);
                        $(this).val(1) //Replacement Cost
                    }
                }
                else {
                    $(this).prop("disabled", false);
                };
            };
        });
    }

    // Returns a date object for the passed date string
    // Delimiter must be a slash
    function ConvertStringToDate(strDt) {
        var dtparts = strDt.split("/");
        var mo = dtparts[0]; 
        // Convert month as js is zero-based
        switch (mo) {
            case "1", "01":
                mo = "0";
                break;
            case "2", "02":
                mo = "1";
                break;
            case "3", "03":
                mo = "2";
                break;
            case "4", "04":
                mo = "3";
                break;
            case "5", "05":
                mo = "4";
                break;
            case "6", "06":
                mo = "5";
                break;
            case "7", "07":
                mo = "6";
                break;
            case "8", "08":
                mo = "7";
                break;
            case "9", "09":
                mo = "8";
                break;
            case "10", "10":
                mo = "9";
                break;
            case "11", "11":
                mo = "10";
                break;
            case "12", "12":
                mo = "11";
                break;
        }
        var newdt = new Date(dtparts[2], mo, dtparts[1]);
        return newdt;
    }

    function isMineSubCounty(stateId, county) {
        ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(stateId, function (data) {
            if (data.contains(county)) {
                return true
            }
            return false
        });
    }
    //Added 11/28/18 for multi state MLW
    function checkMineSub() {
        var mineSubNotReqCounter = 0;
        var ndx = -1;
        $('input[id*=chkMineSubsidence]').each(function () {
            ndx += 1;
            var mineSubElement = $(this);
            mineSubElement.closest('[id*=trMineSubsidenceRow]').hide();
            mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_IL]').hide();
            mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL]').hide();
            if (multiStateEnabled == true) {
                if (mineSubElement.closest('[id*=tblBuildingCoverages]').find('[id*=chkBuildingCoverage]').prop('checked')) {
                    var mineSubState = getLocationStateValue(mineSubElement);
                    var mineSubCounty = getLocationCountyText(mineSubElement);
                    switch (mineSubState) {
                        case '15':
                            mineSubElement.closest('[id*=trMineSubsidenceRow]').show();
                            ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                if (data.contains(mineSubCounty)) {
                                    mineSubElement.prop("checked", true);
                                    mineSubElement.prop("disabled", true);
                                    mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_IL]').show();
                                }
                                else {
                                    mineSubNotReqCounter += 1;
                                    if (mineSubElement.prop('checked')) {
                                        if (mineSubNotReqCounter > 1) {
                                            mineSubElement.prop("disabled", true);
                                            mineSubElement.prop("checked", true)
                                        } else {
                                            mineSubElement.prop("disabled", false);
                                        }
                                    } else {
                                        mineSubElement.prop("disabled", false);
                                    }
                                    if (mineSubElement.prop('checked')) {
                                        mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL]').show();
                                    }
                                }
                            });                          
                            break;
                        case '16':
                            ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                if (data.contains(mineSubCounty)) {
                                    mineSubElement.closest('[id*=trMineSubsidenceRow]').show();
                                }
                            });                                                    
                            break;
                        case '36':
                            // -- OHIO MINE SUB --
                            // Parse Quote Effective Date
                            // CPRQuoteEffectiveDate was set in the code-behind
                            var effdt = ConvertStringToDate(CPRQuoteEffectiveDate);
                            // Parse Ohio Effective Date
                            // CPROhioEffectiveDate was set in the code-behind
                            var OhioDate = ConvertStringToDate(CPROhioEffectiveDate);
                            // Get the class code for this building by reading the classcode textbox value.
                            // If the class code index equals the control index then we know it's the right one.
                            var cc = '';
                            var ccndx = -1;
                            $('input[id*=txtINFClassCode]').each(function () {
                                ccndx += 1;
                                if (ccndx == ndx) {
                                    cc = $(this)[0].value;
                                }
                            });
                        
                            if (effdt >= OhioDate) {
                                // On or after Ohio Eff Date
                                // If mine sub mandatory county AND has eligible class code:
                                //   - Show, check and disable the mine sub checkbox
                                //   - Show the Ohio info row
                                //   - The VB code behind will handle the number of units section.
                                ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                    if (data.contains(mineSubCounty)) {
                                        if (cc == "0196" || cc == "0197" || cc == "0198" || cc == "0311" || cc == "0331") {
                                            mineSubElement.closest('[id*=trMineSubsidenceRow]').show();
                                            mineSubElement.prop("checked", true);
                                            mineSubElement.prop("disabled", true);
                                            mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_OH]').show();
                                        }
                                        else {
                                            // Not Eligible - hide mine sub
                                            mineSubElement.closest('[id*=trMineSubsidenceRow]').hide();
                                            mineSubElement.prop("checked", false);
                                            mineSubElement.prop("disabled", false);
                                            mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_OH]').hide();
                                        }
                                    }
                                });
                            }
                            else {
                                // Before Ohio Eff Date - hide mine sub
                                mineSubElement.closest('[id*=trMineSubsidenceRow]').hide();
                                mineSubElement.prop("checked", false);
                                mineSubElement.prop("disabled", false);
                                mineSubElement.closest('[id*=trBuildingCoverageDataRow]').find('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_OH]').hide();
                            }
                            break;
                    }

                };
            }
        });
    }
    //Added 11/28/18 for multi state MLW
    $('input[id*=chkMineSubsidence]').click(function () {
        if (multiStateEnabled == true) {
            if (getLocationStateValue(this) == '15') {
                if ($(this).prop('checked')) {
                    $('input[id*=chkMineSubsidence]').each(function () {
                        var chkMineSub = $(this);
                        var mineSubState = getLocationStateValue(chkMineSub)
                        switch (mineSubState) {
                            case '15':
                                ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                    if (!data.contains(getLocationCountyText(chkMineSub))) {
                                        chkMineSub.prop('checked', true)
                                    }
                                });
                                break;
                        }
                    });
                    checkMineSub();
                } else {
                    $('input[id*=chkMineSubsidence]').each(function () {
                        var chkMineSub = $(this);
                        var mineSubState = getLocationStateValue(chkMineSub)
                        switch (mineSubState) {
                            case '15':
                            ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                    if (!data.contains(getLocationCountyText(chkMineSub))) {
                                        chkMineSub.prop('checked', false)
                                    }
                                });
                                break;
                        }                      
                    });
                    checkMineSub();
                }
            }
        }
    });
    //Added 11/28/18 for multi state MLW
    $('input[id*=chkBuildingCoverage]').click(function () {
        if (multiStateEnabled == true) {
            var chkBldgCov = $(this);
            var hasMineSubChecked = false;
            var chkMine = chkBldgCov.closest('[id*=tblBuildingCoverages]').find('[id*=chkMineSubsidence]');
            if (getLocationStateValue(chkBldgCov) == '15') {
                chkMine.prop("checked", false);
                if (chkBldgCov.prop('checked')) {
                    $('input[id*=chkMineSubsidence]').each(function () {
                        var chkMineSub = $(this);
                        var mineSubState = getLocationStateValue(chkMineSub)
                        switch (mineSubState) {
                            case '15':                               
                                ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                    if (hasMineSubChecked == false && !data.contains(getLocationCountyText(chkMineSub))) {
                                        if (chkMineSub.prop('checked')) {
                                            hasMineSubChecked = true; // Used to skip processing of other counties
                                            chkMine.prop("checked", true);
                                        }
                                    }
                                });
                                break;
                        }                    
                    });
                }
                checkMineSub();
            }
        }
    });
});


