
$(document).ready(function () {


});

var Cgl = new function () {

    // OH - Stop Gap clicks (policy coverage page)
    this.HandleStopGapClicks = function (chkId, trDataId) {
        var chk = document.getElementById(chkId);
        var trData = document.getElementById(trDataId);

        if (chk && trData) {
            if (chk.checked) {
                trData.style.display = '';
            }
            else {
                if (!confirm('Are you sure you want to delete this coverage?')) {
                    chk.checked = true;
                }
                else {
                    trData.style.display = 'none';
                }
            }
        }
        return true;
    }

    // When the 'IL - Contractors Home Repair & Remodeling' checkbox is checked or unchecked
    // show or hide the info row
    this.ILContractorsHomeRepairCheckboxChanged = function (chkId, trInfoRowId) {
        var chk = document.getElementById(chkId);
        var trInfo = document.getElementById(trInfoRowId);

        if (chk && trInfo) {
            if (chk.checked) {
                trInfo.style.display = '';
            }
            else {
                trInfo.style.display = 'none';
            }
        }
    };

    // Updates the hidden stateid field with whatever was chosen in the dropdown
    this.ClassCodeStateDropdownChanged = function (ddId, hdnId) {
        var dd = document.getElementById(ddId);
        var hdn = document.getElementById(hdnId);

        if (dd && hdn) {
            hdn.value = dd.value;
        }
        return true;
    };

    // When the Classification assigned location changes we need to update the state dropdown on the lookup control
    this.ClassCodeAssignedLocationChanged = function (ddAssignedLocationId, ddStateId, hdnStateId) {
        var ddAssignedLocation = document.getElementById(ddAssignedLocationId);
        var ddState = document.getElementById(ddStateId);
        var hdnState = document.getElementById(hdnStateId);

        if (ddAssignedLocation && ddState && hdnState) {
            var AssignedState = Cgl.ParseLocationTextState(ddAssignedLocation);
            if (AssignedState == "IN") {
                // Indiana
                ddState.value = "16"
                hdnState.value = "16"
            }
            else {
                if (AssignedState =="IL") {
                    // Illinois
                    ddState.value = "15"
                    hdnState.value = "15"
                }
            }
        }
    };

    // Handles Class Code Location Assignment dropdown change
    // This is the dropdown that selects location or policy level
    this.AssignmentDDLChanged = function (DdlId, InfoRowId, ddStateId, ddAssignedLocationId) {
        var DDL = document.getElementById(DdlId);
        var InfoRow = document.getElementById(InfoRowId);
        var ddState = document.getElementById(ddStateId);
        var ddAssignedLocation = document.getElementById(ddAssignedLocationId);

        // If the assignment is location (2), show the info row & disable the state dropdown 
        if (DDL && InfoRow && ddState && ddAssignedLocation) {
            if (DDL.value == "2") {
                // Location - display the info row, disable the state dropdown
                InfoRow.style.display = '';
                ddState.disabled = true;
                // Set the state based on the selected location
                var AssignedState = Cgl.ParseLocationTextState(ddAssignedLocation);
                if (AssignedState) {
                    switch (AssignedState) {
                        case "IN": {
                            ddState.value = "16"
                            break;
                        }
                        case "IL": {
                            ddState.value = "15"
                            break;
                        }
                    }
                }
            }
            else {
                // Policy - hide the info row, enable the state dropdown
                InfoRow.style.display = 'none';
                ddState.disabled = false;
            }
        }
        return true;
    };

    // Parses the state from the passed location dropdown selected item
    this.ParseLocationTextState = function (ddAssignedLocation) {
        var AssignedText = ddAssignedLocation.options[ddAssignedLocation.selectedIndex].text;
        if (AssignedText.indexOf(" IN ") != -1) {
            return "IN";
        }
        if (AssignedText.indexOf(" IL ") != -1) {
            return "IL";
        }
        return "";
    };

    // This version uses the cached page control values instead of passing in a bazillion parameters
    // Pass in the states on the quote in the StatesOnQuote parameter, separated by a comma ("IN,IL" or "IN")
    this.OccurrenceLiabilityLimitChanged = function(StatesOnQuote, isNewCo){
        var PageBindings = Cgl.CGLCoveragPageBindings[0];

        var OLLDdl = document.getElementById(PageBindings.ddOLL);
        var GenAggDdl = document.getElementById(PageBindings.ddGenAgg);
        var OpsAggDdl = document.getElementById(PageBindings.ddOpnsAgg);
        var PersInjAggDdl = document.getElementById(PageBindings.ddPersonalInjury);
        var LiqLiabOLLTxt_IN = document.getElementById(PageBindings.txtLiquorLimit_IN);
        var LiqLiabOLLTxt_IL = document.getElementById(PageBindings.txtLiquorLimit_IL);
        var txtEBLLimit = document.getElementById(PageBindings.txtEBLLimitId);

        var states = StatesOnQuote.split(",");
        var HasIndiana = false;
        var HasIllinois = false;
        var HasOhio = false;

        for (var i = 0; i < states.length; ++i) {
            if (states[i] == "IN"){HasIndiana = true;}
            if (states[i] == "IL") { HasIllinois = true; }
            if (states[i] == "OH") { HasOhio = true; }
        }

        var CanHaveLiquorLiability = false;

        // Determine the values that are double and the same as the selected OLL
        // 25,000 = 8
        // 50,000 = 9
        // 100,000 = 10
        // 200,000 = 32
        // 300,000 = 33
        // 500,000 = 34
        // 600,000 = 178
        // 1,000,000 = 56
        // 1,500,000 = 185
        // 2,000,000 = 65

        var dblVal = null;
        var sameVal = null;
        var samevaltext = null;

        sameVal = OLLDdl.value;
        switch (OLLDdl.value) {
            case "8":  // 25,000
                dblVal = "9";
                samevaltext = "25,000";
                CanHaveLiquorLiability = false;
                break;
            case "9":  // 50,000
                dblVal = "10";
                samevaltext = "50,000";
                CanHaveLiquorLiability = false;
                break;
            case "10":  // 100,000
                dblVal = "32";
                samevaltext = "100,000";
                CanHaveLiquorLiability = false;
                break;
            case "32":  // 200,000
                dblVal = "34";  // Double value is 500k for OLL of 200k per BRD
                samevaltext = "200,000";
                CanHaveLiquorLiability = false;
                break;
            case "33":  // 300,000
                dblVal = "178";
                samevaltext = "300,000";
                CanHaveLiquorLiability = true;
                break;
            case "34":  // 500,000
                dblVal = "56";
                samevaltext = "500,000";
                CanHaveLiquorLiability = true;
                break;
            case "56":  // 1,000,000
                dblVal = "65";
                samevaltext = "1,000,000";
                CanHaveLiquorLiability = true;
                break;
        }

        
        if (isNewCo == "True") {
            //load General Aggregate drop down options based on Occurrence Liability Limit selected value
            //load Products/Completed Operations Aggregate based on Occurrence Liability Limit and General Aggregate selected values
            //load Personal And Advertising Injury Limit based on Occurrence Liability Limit selected values
            var ddGenAggVal = GenAggDdl.value;
            var ddOpsAggVal = OpsAggDdl.value;
            var ddPersInjAggVal = PersInjAggDdl.value;
            GenAggDdl.options.length = 0;  // Clear the list
            OpsAggDdl.options.length = 0;
            PersInjAggDdl.options.length = 0;
            var option = null;

            option = null;
            option = document.createElement('option');
            option.text = 'EXCLUDED';
            option.value = '327';
            if (ddOpsAggVal == '327') {
                option.selected = 'selected';
            }
            OpsAggDdl.add(option);

            option = null;
            option = document.createElement('option');
            option.text = 'EXCLUDED';
            if (ddPersInjAggVal == '327') {
                option.selected = 'selected';
            }
            option.value = '327';
            PersInjAggDdl.add(option);

            switch (OLLDdl.value) {
                case "33": //300,000                    
                    option = null;
                    option = document.createElement('option');
                    option.text = '600,000';
                    option.value = '178';
                    option.selected = 'selected';
                    GenAggDdl.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = '600,000';
                    option.value = '178';
                    if (ddOpsAggVal != '327') {
                        option.selected = 'selected';
                    }
                    OpsAggDdl.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = '300,000';
                    option.value = '33';
                    if (ddPersInjAggVal != '327') {
                        option.selected = 'selected';
                    }
                    PersInjAggDdl.add(option);

                    break;
                case "34": //500,000
                    option = null;
                    option = document.createElement('option');
                    option.text = '1,000,000';
                    option.value = '56';
                    option.selected = 'selected';
                    GenAggDdl.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = '1,000,000';
                    option.value = '56';
                    if (ddOpsAggVal != '327') {
                        option.selected = 'selected';
                    }
                    OpsAggDdl.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = '500,000';
                    option.value = '34';
                    if (ddPersInjAggVal != '327') {
                        option.selected = 'selected';
                    }
                    PersInjAggDdl.add(option);

                    break;
                case "56": //1,000,000
                    option = null;
                    option = document.createElement('option');
                    option.text = '2,000,000';
                    option.value = '65';
                    if (ddGenAggVal == '65') {
                        option.selected = 'selected';
                    }
                    GenAggDdl.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = '3,000,000';
                    option.value = '167';
                    if (ddGenAggVal == '167') {
                        option.selected = 'selected';
                    }
                    GenAggDdl.add(option);

                    if (ddGenAggVal == '167') {
                        option = null;
                        option = document.createElement('option');
                        option.text = '3,000,000';
                        option.value = '167';
                        if (ddOpsAggVal != '327') {
                            option.selected = 'selected';
                        }
                        OpsAggDdl.add(option);
                    } else {
                        option = null;
                        option = document.createElement('option');
                        option.text = '2,000,000';
                        option.value = '65';
                        if (ddOpsAggVal != '327') {
                            option.selected = 'selected';
                        }
                        OpsAggDdl.add(option);
                    }

                    option = null;
                    option = document.createElement('option');
                    option.text = '1,000,000';
                    option.value = '56';
                    if (ddPersInjAggVal != '327') {
                        option.selected = 'selected';
                    }
                    PersInjAggDdl.add(option);

                    break;
            }
        } else {
            // Set General Aggregate and Product Aggregate to twice the selected OLL limit
            if (GenAggDdl) { GenAggDdl.value = dblVal; }
            if (OpsAggDdl) { OpsAggDdl.value = dblVal; }

            // Set Personal Injury Aggregate to the same value as the selected OLL limit
            if (PersInjAggDdl) { PersInjAggDdl.value = sameVal; }
        }
                
        // *****************************************
        // LIQUOR LIABILITY
        // *****************************************
        // Get all of the controls we're going to need for Liquor Liability
        var trLiquorCheckboxRow_IN = document.getElementById(PageBindings.trLiquorLiabilityCheckBoxRow_IN);
        var trLiquorInfoRow_IN = document.getElementById(PageBindings.trLiquorInfoRow_IN);
        var trLiquorDataRow1_IN = document.getElementById(PageBindings.trLiquorDataRow1_IN);
        var trLiquorInfoRow2_IN = document.getElementById(PageBindings.trLiquorInfoRow2_IN);
        var trLiquorCheckboxRow_IL = document.getElementById(PageBindings.trLiquorLiabilityCheckboxRow_IL);
        var trLiquorInfoRow_IL = document.getElementById(PageBindings.trLiquorInfoRow_IL);
        var trLiquorInfoRow2_IL = document.getElementById(PageBindings.trLiquorInfoRow2_IL);
        var trLiquorDataRow1_IL = document.getElementById(PageBindings.trLiquorDataRow1_IL);
        var trLiquorInfoRow3_IL = document.getElementById(PageBindings.trLiquorInfoRow3_IL);
        var chkLiquor_IN = document.getElementById(PageBindings.chkLiquor_IN);
        var chkLiquor_IL = document.getElementById(PageBindings.chkLiquor_IL);

        // Hide everything initially
        trLiquorCheckboxRow_IN.style.display = "none";
        trLiquorInfoRow_IN.style.display = "none";
        trLiquorDataRow1_IN.style.display = "none";
        trLiquorInfoRow2_IN.style.display = "none";
        trLiquorCheckboxRow_IL.style.display = "none";
        trLiquorInfoRow_IL.style.display = "none";
        trLiquorInfoRow2_IL.style.display = "none";
        trLiquorDataRow1_IL.style.display = "none";
        trLiquorInfoRow3_IL.style.display = "none";

        this.ToggleLiquorLiabilityControls('none', 'IL')
        this.ToggleLiquorLiabilityControls('none', 'IN')

        // If the OLL is < 300000 then we need to hide the liquor liability sections, the coverage is only available
        // if the OLL s 300k or higher.  Note that we set the CanHaveLiquorLiability variable above in the OLL case statement
        if (CanHaveLiquorLiability) {
            // ********************************
            // ELIGIBLE FOR LIQUOR LIABILITY
            // ********************************

            // Show the Indiana liquor Liability rows
            if (HasIndiana || HasOhio) {
                trLiquorCheckboxRow_IN.style.display = "";
                if (chkLiquor_IN.checked) {
                    this.ToggleLiquorLiabilityControls('', 'IN')
                    trLiquorInfoRow_IN.style.display = "";
                    trLiquorDataRow1_IN.style.display = "";
                    trLiquorInfoRow2_IN.style.display = "";
                }
            }

            // Show the Illnois liquor Liability rows
            if (HasIllinois) {
                trLiquorCheckboxRow_IL.style.display = "";
                if (chkLiquor_IL.checked) {
                    this.ToggleLiquorLiabilityControls('', 'IL')
                    trLiquorInfoRow_IL.style.display = "";
                    trLiquorInfoRow2_IL.style.display = "";
                    trLiquorDataRow1_IL.style.display = "";
                    trLiquorInfoRow3_IL.style.display = "";
                }
            }

            // INDIANA - Set the Liquor Liability Occurrence Liability Limit to the same value as Policy Occurrence Liability Limit
            if (LiqLiabOLLTxt_IN) { LiqLiabOLLTxt_IN.value = samevaltext; }

            // ILLINOIS - Set the Liquor Liability Limit to a value based on what was chosen in OLL
            // TODO: set these values correctly!
            if (LiqLiabOLLTxt_IN && OLLDdl) {
                switch (OLLDdl.value) {
                    //case "10":  // 100,000
                    //    LiqLiabOLLTxt_IL.value = "69/69/85/600"
                    //    break;
                    //case "32":  // 200,000
                    //    LiqLiabOLLTxt_IL.value = "69/69/85/600"
                    //    break;
                    case "33":  // 300,000
                        LiqLiabOLLTxt_IL.value = "STATE STATUTORY LIMITS WILL APPLY"
                        break;
                    case "34":  // 500,000
                        LiqLiabOLLTxt_IL.value = "STATE STATUTORY LIMITS WILL APPLY"
                        break;
                    case "56":  // 1,000,000
                        LiqLiabOLLTxt_IL.value = "STATE STATUTORY LIMITS WILL APPLY"
                        break;
                }
            }
        }
        // *****************************************
        // END OF LIQUOR LIABILITY
        // *****************************************

        // Set the Employee Benefits Liability limit to the same value as Policy Occurrence Liability Limit
        if (txtEBLLimit) { txtEBLLimit.value = samevaltext; }

        return true;
    };

    this.GeneralAggregateChanged = function (isNewCo) {
        //load Products/Completed Operations Aggregate drop down options based on General Aggregate selected value
        if (isNewCo == "True") {
            var PageBindings = Cgl.CGLCoveragPageBindings[0];
            var ddGenAggVal = document.getElementById(PageBindings.ddGenAgg).value;
            var OpsAggDdl = document.getElementById(PageBindings.ddOpnsAgg);

            OpsAggDdl.options.length = 0;  // Clear the list
            var option = null;
            option = null;
            option = document.createElement('option');
            option.text = 'EXCLUDED';
            option.value = '327';
            OpsAggDdl.add(option);
            
            switch (ddGenAggVal) {
                case "178": //600,000
                    option = null;
                    option = document.createElement('option');
                    option.text = '600,000';
                    option.value = '178';
                    OpsAggDdl.add(option);
                    break;
                case "56": //1,000,000
                    option = null;
                    option = document.createElement('option');
                    option.text = '1,000,000';
                    option.value = '56';
                    option.selected = 'selected';
                    OpsAggDdl.add(option);
                    break;
                case "65": //2,000,000
                    option = null;
                    option = document.createElement('option');
                    option.text = '2,000,000';
                    option.value = '65';
                    option.selected = 'selected';
                    OpsAggDdl.add(option);
                    break;
                case "167": //3,000,000
                    option = null;
                    option = document.createElement('option');
                    option.text = '3,000,000';
                    option.value = '167';
                    option.selected = 'selected';
                    OpsAggDdl.add(option);
                    break;
            }
        }
    }

    this.MedicalExpenseChanged = function (ddMedExpId, PlusENHCheckBoxId, ENHCheckBoxId, ContractENHCheckBoxId, ManufENHCheckBoxId) {
        var MedExpId = document.getElementById(ddMedExpId);
        var ENHcb = document.getElementById(ENHCheckBoxId);
        var PlusENHcb = document.getElementById(PlusENHCheckBoxId)
        var ContractENH = document.getElementById(ContractENHCheckBoxId);
        var ManufENH = document.getElementById(ManufENHCheckBoxId)

        var MedExpValue = MedExpId.value;
        // If Id 327(excluded) uncheck and disable GLEnhancementCheckbox and GLPlusEnhancementCheckbox
        if (MedExpValue === '327') {
            ENHcb.checked = false;
            ENHcb.disabled = true;
            PlusENHcb.checked = false;
            PlusENHcb.disabled = true;
            ContractENH.checked = false;
            ContractENH.disabled = true;
            ManufENH.checked = false;
            ManufENH.disabled = true;
        }
        else {
            PlusENHcb.disabled = false;
            ENHcb.disabled = false;
        }
    };

    this.EnhancementCheckboxChanged = function (ENHCheckBoxId, AddBlanketRowId, RentedTxtId, ddMedExpId, PlusENHCheckBoxId) {
        var ENHcb = document.getElementById(ENHCheckBoxId);
        var AddBlanketRow = document.getElementById(AddBlanketRowId);
        var PlusENHcb = document.getElementById(PlusENHCheckBoxId)
        var RentedTxt = document.getElementById(RentedTxtId);
        var MedExpId = document.getElementById(ddMedExpId);
        var MedExpValue = MedExpId.value;

        // If the enhancement is selected, show the 'Add blanket waiver...' row
        if (MedExpValue === '327') {
            ENHcb.checked = false;
            ENHcb.disabled = true;
            PlusENHcb.checked = false;
            PlusENHcb.disabled = true;
        } else {
            if (ENHcb.checked) {
                AddBlanketRow.style.display = '';
                PlusENHcb.checked = false;
                PlusENHcb.disabled = true;
            }
            else {
                AddBlanketRow.style.display = 'none';
                PlusENHcb.checked = false;
                PlusENHcb.disabled = false;
            }
        }
    };

    this.PlusEnhancementCheckboxChanged = function (PlusENHCheckBoxId, ENHCheckBoxId, ddMedExpId) {
        var ENHcb = document.getElementById(ENHCheckBoxId);
        var PlusENHcb = document.getElementById(PlusENHCheckBoxId)
        var MedExpId = document.getElementById(ddMedExpId);
        var MedExpValue = MedExpId.value;
        // If the plus is clicked enhancement is selected, disable ENHcb.
        if (MedExpValue === '327') {
            ENHcb.checked = false;
            ENHcb.disabled = true;
            PlusENHcb.checked = false;
            PlusENHcb.disabled = true;
        } else {
            if (PlusENHcb.checked) {
                ENHcb.checked = false;
                ENHcb.disabled = true;
            }
            else {
                ENHcb.checked = false;
                ENHcb.disabled = false;
            }
        }
    };

    this.BlanketWaiverOfSubroChanged = function (BlanketWaiverDdlId, EnhMsgRowId) {
        var BlanketWaiverDdl = document.getElementById(BlanketWaiverDdlId);
        var EnhMsgRow = document.getElementById(EnhMsgRowId);

        // If the "add blanket waiver" response is YES or YES WITH COMPLETED OPS, show the enhancement message
        if (BlanketWaiverDdl.value == "1" || BlanketWaiverDdl.value == "2"){
            EnhMsgRow.style.display = '';
        }
        else {
            EnhMsgRow.style.display = 'none';
        }
    };

    this.GLLiabilityDeductibleChanged = function (AddADedDdlId, DedDivId) {
        var AddDedDdl = document.getElementById(AddADedDdlId);
        var DedDiv = document.getElementById(DedDivId);

        // If 'Add a General Liability Deductible' dropdown value is YES (1) show the deductible fields
        if (AddDedDdl.value == "1") {
            DedDiv.style.display = '';
        }
        else {
            DedDiv.style.display = 'none';
        }
        return true;
    };

    this.CoverageCheckboxChanged = function (sender, CovChkId, DataRow1Id, DataRow2Id, DataRow3Id, InfoRow1Id, InfoRow2Id, InfoRow3Id, IsNewCyberCoverageNameAvail) {
        var CovChk = document.getElementById(CovChkId);
        var DataRow1 = document.getElementById(DataRow1Id);
        var DataRow2 = document.getElementById(DataRow2Id);
        var DataRow3 = document.getElementById(DataRow3Id);
        var InfoRow1 = document.getElementById(InfoRow1Id);
        var InfoRow2 = document.getElementById(InfoRow2Id);
        var InfoRow3 = document.getElementById(InfoRow3Id);
        var CovName = 'none';

        // Hide everything 
        if (DataRow1) { DataRow1.style.display = 'none'; }
        if (DataRow2) { DataRow2.style.display = 'none'; }
        if (DataRow3) { DataRow3.style.display = 'none'; }
        if (InfoRow1) { InfoRow1.style.display = 'none'; }
        if (InfoRow2) { InfoRow2.style.display = 'none'; }
        if (InfoRow3) { InfoRow3.style.display = 'none'; }

        switch (sender) {
            case "AI": {
                CovName = "Additional Interest Coverage";
                break;
            }
            case "EBL": {
                CovName = "Employee Benefits Liability Coverage";
                break;
            }
            case "EPLI": {
                CovName = "EPLI Coverage";
                break;
            }
            case "CLI": {
                if (IsNewCyberCoverageNameAvail == "True") {
                    CovName = "Cyber Coverage";
                } else {
                    CovName = "Cyber Liability Coverage";
                }               
                break;
            }
            case "HNO": {
                CovName = "Hired/Non-Owned Coverage";
                break;
            }
            case "LIQ_IN": {
                CovName = "Liquor Liability Coverage";
                this.ToggleLiquorLiabilityControls('none', 'IN')
                break;
            }
            case "LIQ_IL": {
                CovName = "Liquor Liability Coverage";
                this.ToggleLiquorLiabilityControls('none', 'IL')
                break;
            }
            case "CDO": {
                CovName = "Condo D&O Coverage";
                break;
            }
            case "MAN": {
                CovName = "Manufacturer, WholeSalers & Distributors Liquor Liability Coverage"
            }
            case "RES": {
                CovName = "Restaurants Or Hotels Liquor Liability Coverage"
            }
            case "PAC": {
                CovName = "Package Stores Liquor Liability Coverage"
            }
            case "CLU": {
                CovName = "Clubs Liquor Liability Coverage"
            }
        }
        // Show if the element exists
        if (CovChk)
        {
            if (CovChk.checked) {
                if (DataRow1) { DataRow1.style.display = ''; }
                if (DataRow2) { DataRow2.style.display = ''; }
                if (DataRow3) { DataRow3.style.display = ''; }
                if (InfoRow1) { InfoRow1.style.display = ''; }
                if (InfoRow2) { InfoRow2.style.display = ''; }
                if (InfoRow3) { InfoRow3.style.display = ''; }
                if (sender == "CDO") {
                    // If the condo d&o coverage is checked, show info popup
                    var msg = '';
                    msg += 'Condo D&O Eligibility Guidelines \n \n';
                    msg += 'The following are eligibility guidelines to qualify for Condominium, Co-Ops, Associations - Directors and Officers Liability Coverage: \n \n';
                    msg += '* Have supporting CPP. \n';
                    msg += '* Have GL occurrence limit equal or greater than $1,000,000. \n';
                    msg += '* A developer/builder of the property cannot be a member of the Board of Directors. \n';
                    msg += '* A copy of loss run submitted. \n';
                    msg += '* A copy of prior carriers DEC when Retroactive Date is requested to support continuous prior coverage. \n';
                    msg += '* Our Condominium Directors and Officers Application completed and submitted. \n';
                    alert(msg);

                }
                else if (sender === "LIQ_IN") {
                    this.ToggleLiquorLiabilityControls('', 'IN')
                }
                else if (sender === "LIQ_IL") {
                    this.ToggleLiquorLiabilityControls('', 'IL')
                }
            }
            else {
                // Coverage checkbox is UNchecked
                if (confirm('Are you sure you want to delete ' + CovName + '?')) {
                    // Delete coverage
                    if (DataRow1) { DataRow1.style.display = 'none'; }
                    if (DataRow2) { DataRow2.style.display = 'none'; }
                    if (DataRow3) { DataRow3.style.display = 'none'; }
                    if (InfoRow1) { InfoRow1.style.display = 'none'; }
                    if (InfoRow2) { InfoRow2.style.display = 'none'; }
                    if (InfoRow3) { InfoRow3.style.display = 'none'; }
                    if (sender === "LIQ_IN") {
                        this.ToggleLiquorLiabilityControls('none', 'IN')
                    }
                    else if (sender === "LIQ_IL") {
                        this.ToggleLiquorLiabilityControls('none', 'IL')
                    }
                }
                else {
                    // Cancel deletion of coverage
                    CovChk.checked = true;
                    if (DataRow1) { DataRow1.style.display = ''; }
                    if (DataRow2) { DataRow2.style.display = ''; }
                    if (DataRow3) { DataRow3.style.display = ''; }
                    if (InfoRow1) { InfoRow1.style.display = ''; }
                    if (InfoRow2) { InfoRow2.style.display = ''; }
                    if (InfoRow3) { InfoRow3.style.display = ''; }
                    if (sender === "LIQ_IN") {
                        this.ToggleLiquorLiabilityControls('', 'IN')
                    }
                    else if (sender === "LIQ_IL") {
                        this.ToggleLiquorLiabilityControls('', 'IL')
                    }
                }
            }
        }
    };

    // Cache all of the coverage page control id's here to cut down on the number of parameters that need to be passed to certain functions
    this.CGLCoveragPageBindings = new Array();
    this.CGLCoverageUiBinding = function (ddProgramTypeId, ddOLLId, ddGenAggId, txtRentedId, ddOpnsAggId, ddMedExpenseId, ddPersonalInjuryId, chkGLEnhId, ddAddlBlanketSubroId, chkContractorsEnhId, chkManufEnhID, ddAddGenLiabDedId, ddGLDedTypeId, ddGLDedAmtId, ddGLDedBasisId, chkAddlInsuredsId, ddNumberOfAddlInsuredsId, chkCondoId, txtNamedAssocId, txtCondoLimitId, ddCondoDedId, chkEBLId, txtEBLLimitId, txtEBLNumEmployeesId, chkCLIId, chkEPLIId, chkHiredNonOwnedId, chkLiquor_IN_Id, txtLiquorLimit_IN_Id, chkLiquor_IL_Id, txtLiquorLimit_IL_Id, chkHomeRepair_IL_Id, trProgramTypeRowId, trAddlGLBlanketWaiverOfSubroRowId, trContractorsGLEnhancementRowId, trManufacturersGLEnhancementRowId, trAddlInsuredDataRowId, trCondoDAndORowId, trCondoDAndODataRow1Id, trCondoDAndODataRow2Id, trCondoDAndODataRow3Id, trEmployeeBenefitsLiabilityDataRow1Id, trEmployeeBenefitsLiabilityDataRow2Id, trEmployeeBenefitsLiabilityInfoRowId, trCLIInfoRow1Id, trCLIInfoRow2Id, trEPLIInfoRow1Id, trEPLIInfoRow2Id, trLiquorLiabilityCheckBoxRow_IN_Id, trLiquorInfoRow_IN_Id, trLiquorDataRow1_IN_Id, trLiquorInfoRow2_IN_Id, trLiquorLiabilityCheckboxRow_IL_Id, trLiquorInfoRow_IL_Id, trLiquorInfoRow2_IL_Id, trLiquorDataRow1_IL_Id, trLiquorInfoRow3_IL_Id, trContractorsHomeRepairAndRemodelingRow_Id, trContractorsHomeRepairAndRemodelingInfoRow_Id, chkManufacturerLiquorSalesTableRow_IN_Id, chkRestaurantLiquorSalesTableRow_IN_Id, chkPackageStoreLiquorSalesTableRow_IN_Id, chkClubLiquorSalesTableRow_IN_Id, chkManufacturerLiquorSales_IN_Id, chkRestaurantLiquorSales_IN_Id, chkPackageStoreLiquorSales_IN_Id, chkClubLiquorSales_IN_Id, txtManufacturerLiquorSalesTableRow_IN_Id, txtRestaurantLiquorSalesTableRow_IN_Id, txtPackageStoreLiquorSalesTableRow_IN_Id, txtClubLiquorSalesTableRow_IN_Id, txtManufacturerLiquorSales_IN_Id, txtRestaurantLiquorSales_IN_Id, txtPackageStoreLiquorSales_IN_Id, txtClubLiquorSales_IN_Id, chkManufacturerLiquorSalesTableRow_IL_Id, chkRestaurantLiquorSalesTableRow_IL_Id, chkPackageStoreLiquorSalesTableRow_IL_Id, chkClubLiquorSalesTableRow_IL_Id, chkManufacturerLiquorSales_IL_Id, chkRestaurantLiquorSales_IL_Id, chkPackageStoreLiquorSales_IL_Id, chkClubLiquorSales_IL_Id, txtManufacturerLiquorSalesTableRow_IL_Id, txtRestaurantLiquorSalesTableRow_IL_Id, txtPackageStoreLiquorSalesTableRow_IL_Id, txtClubLiquorSalesTableRow_IL_Id, txtManufacturerLiquorSales_IL_Id, txtRestaurantLiquorSales_IL_Id, txtPackageStoreLiquorSales_IL_Id, txtClubLiquorSales_IL_Id) {
        // Form Controls
        this.ddProgramType = ddProgramTypeId;
        this.ddOLL = ddOLLId;
        this.ddGenAgg = ddGenAggId;
        this.txtRented = txtRentedId;
        this.ddOpnsAgg = ddOpnsAggId;
        this.ddMedExpense = ddMedExpenseId;
        this.ddPersonalInjury = ddPersonalInjuryId;
        this.chkGLEnh = chkGLEnhId;
        this.ddAddlBlanketSubro = ddAddlBlanketSubroId;
        this.chkContractorsEnh = chkContractorsEnhId;
        this.chkManufEnh = chkManufEnhID;
        this.ddAddlGenLiabDed =  ddAddGenLiabDedId;
        this.ddGLDedType = ddGLDedTypeId;
        this.ddGLDedAmt = ddGLDedAmtId;
        this.ddGLDedBasis = ddGLDedBasisId;
        this.chkAddlInsureds = chkAddlInsuredsId;
        this.ddNumberOfAddlInsureds = ddNumberOfAddlInsuredsId;
        //this.txtNumAddlInsureds = txtNumAddlInsuredsId;
        this.chkCondo = chkCondoId;
        this.txtNamedAssoc = txtNamedAssocId;
        this.txtCondoLimit = txtCondoLimitId;
        this.ddCondoDed = ddCondoDedId;
        this.chkEBL = chkEBLId;
        this.txtEBLLimit = txtEBLLimitId;
        this.txtEBLNumEmployees = txtEBLNumEmployeesId;
        this.chkCLI = chkCLIId; //zts
        this.chkEPLI = chkEPLIId;
        this.chkHiredNonOwned = chkHiredNonOwnedId;
        this.chkLiquor_IN = chkLiquor_IN_Id;
        this.txtLiquorLimit_IN = txtLiquorLimit_IN_Id;
        this.chkLiquor_IL = chkLiquor_IL_Id;
        this.txtLiquorLimit_IL = txtLiquorLimit_IL_Id;
        this.chkHomeRepair_IL = chkHomeRepair_IL_Id;

        // Table Rows
        this.trProgramTypeRow = trProgramTypeRowId;
        this.trAddlGLBlanketWaiverOfSubroRow = trAddlGLBlanketWaiverOfSubroRowId;
        this.trContractorsGLEnhancementRow = trContractorsGLEnhancementRowId;
        this.trManufacturersGLEnhancementRow = trManufacturersGLEnhancementRowId;
        this.trAddlInsuredDataRow = trAddlInsuredDataRowId;
        //this.trAddlInsuredsInfoRow = trAddlInsuredsInfoRowId;
        this.trCondoDAndORow = trCondoDAndORowId;
        this.trCondoDAndODataRow1 = trCondoDAndODataRow1Id;
        this.trCondoDAndODataRow2 = trCondoDAndODataRow2Id;
        this.trCondoDAndODataRow3 = trCondoDAndODataRow3Id;
        this.trEmployeeBenefitsLiabilityDataRow1 = trEmployeeBenefitsLiabilityDataRow1Id;
        this.trEmployeeBenefitsLiabilityDataRow2 = trEmployeeBenefitsLiabilityDataRow2Id;
        this.trEmployeeBenefitsLiabilityInfoRow = trEmployeeBenefitsLiabilityInfoRowId;
        this.trCLIInfoRow1 = trCLIInfoRow1Id; //zts
        this.trCLIInfoRow2 = trCLIInfoRow2Id; //zts
        this.trEPLIInfoRow1 = trEPLIInfoRow1Id;
        this.trEPLIInfoRow2 = trEPLIInfoRow2Id;
        this.trLiquorLiabilityCheckBoxRow_IN = trLiquorLiabilityCheckBoxRow_IN_Id;
        this.trLiquorInfoRow_IN = trLiquorInfoRow_IN_Id;
        this.trLiquorDataRow1_IN = trLiquorDataRow1_IN_Id;
        this.trLiquorInfoRow2_IN = trLiquorInfoRow2_IN_Id;
        this.trLiquorLiabilityCheckboxRow_IL = trLiquorLiabilityCheckboxRow_IL_Id;
        this.trLiquorInfoRow_IL = trLiquorInfoRow_IL_Id;
        this.trLiquorInfoRow2_IL = trLiquorInfoRow2_IL_Id;
        this.trLiquorDataRow1_IL = trLiquorDataRow1_IL_Id;
        this.trLiquorInfoRow3_IL = trLiquorInfoRow3_IL_Id;
        this.trContractorsHomeRepairAndRemodelingRow = trContractorsHomeRepairAndRemodelingRow_Id;
        this.trContractorsHomeRepairAndRemodelingInfoRow = trContractorsHomeRepairAndRemodelingInfoRow_Id;
        this.chkManufacturerLiquorSalesTableRow_IN = chkManufacturerLiquorSalesTableRow_IN_Id;
        this.chkRestaurantLiquorSalesTableRow_IN = chkRestaurantLiquorSalesTableRow_IN_Id;
        this.chkPackageStoreLiquorSalesTableRow_IN = chkPackageStoreLiquorSalesTableRow_IN_Id;
        this.chkClubLiquorSalesTableRow_IN = chkClubLiquorSalesTableRow_IN_Id;
        this.chkManufacturerLiquorSales_IN = chkManufacturerLiquorSales_IN_Id;
        this.chkRestaurantLiquorSales_IN = chkRestaurantLiquorSales_IN_Id;
        this.chkPackageStoreLiquorSales_IN = chkPackageStoreLiquorSales_IN_Id;
        this.chkClubLiquorSales_IN = chkClubLiquorSales_IN_Id;
        this.txtManufacturerLiquorSalesTableRow_IN = txtManufacturerLiquorSalesTableRow_IN_Id;
        this.txtRestaurantLiquorSalesTableRow_IN = txtRestaurantLiquorSalesTableRow_IN_Id;
        this.txtPackageStoreLiquorSalesTableRow_IN = txtPackageStoreLiquorSalesTableRow_IN_Id;
        this.txtClubLiquorSalesTableRow_IN = txtClubLiquorSalesTableRow_IN_Id;
        this.txtManufacturerLiquorSales_IN = txtManufacturerLiquorSales_IN_Id;
        this.txtRestaurantLiquorSales_IN = txtRestaurantLiquorSales_IN_Id;
        this.txtPackageStoreLiquorSales_IN = txtPackageStoreLiquorSales_IN_Id;
        this.txtClubLiquorSales_IN = txtClubLiquorSales_IN_Id;
        this.chkManufacturerLiquorSalesTableRow_IL = chkManufacturerLiquorSalesTableRow_IL_Id;
        this.chkRestaurantLiquorSalesTableRow_IL = chkRestaurantLiquorSalesTableRow_IL_Id;
        this.chkPackageStoreLiquorSalesTableRow_IL = chkPackageStoreLiquorSalesTableRow_IL_Id;
        this.chkClubLiquorSalesTableRow_IL = chkClubLiquorSalesTableRow_IL_Id;
        this.chkManufacturerLiquorSales_IL = chkManufacturerLiquorSales_IL_Id;
        this.chkRestaurantLiquorSales_IL = chkRestaurantLiquorSales_IL_Id;
        this.chkPackageStoreLiquorSales_IL = chkPackageStoreLiquorSales_IL_Id;
        this.chkClubLiquorSales_IL = chkClubLiquorSales_IL_Id;
        this.txtManufacturerLiquorSalesTableRow_IL = txtManufacturerLiquorSalesTableRow_IL_Id;
        this.txtRestaurantLiquorSalesTableRow_IL = txtRestaurantLiquorSalesTableRow_IL_Id;
        this.txtPackageStoreLiquorSalesTableRow_IL = txtPackageStoreLiquorSalesTableRow_IL_Id;
        this.txtClubLiquorSalesTableRow_IL = txtClubLiquorSalesTableRow_IL_Id;
        this.txtManufacturerLiquorSales_IL = txtManufacturerLiquorSales_IL_Id;
        this.txtRestaurantLiquorSales_IL = txtRestaurantLiquorSales_IL_Id;
        this.txtPackageStoreLiquorSales_IL = txtPackageStoreLiquorSales_IL_Id;
        this.txtClubLiquorSales_IL = txtClubLiquorSales_IL_Id;

        return true;
    };

    //Added 09/02/2021 for bug 51550 MLW
    this.ToggleOccurrenceLiabLimit = function (CovChkId, ddlOccurrenceLiabLimitId) {
        var CovChk = document.getElementById(CovChkId);
        var ddlOccurrenceLiabLimit = document.getElementById(ddlOccurrenceLiabLimitId);
        if (CovChk) {
            if (CovChk.checked) {
                ddlOccurrenceLiabLimit.value = '56'; //56=1,000,000
                ddlOccurrenceLiabLimit.disabled = true;
            }
            else {
                ddlOccurrenceLiabLimit.disabled = false;
            }
        }
    };

    this.ToggleLiquorLiabilityControls = function (displayValue, state) {
        var PageBindings = Cgl.CGLCoveragPageBindings[0];

        if (state === 'IL') {
            var chkManufacturerLiquorSalesTableRow_IL = document.getElementById(PageBindings.chkManufacturerLiquorSalesTableRow_IL);
            var chkRestaurantLiquorSalesTableRow_IL = document.getElementById(PageBindings.chkRestaurantLiquorSalesTableRow_IL);
            var chkPackageStoreLiquorSalesTableRow_IL = document.getElementById(PageBindings.chkPackageStoreLiquorSalesTableRow_IL);
            var chkClubLiquorSalesTableRow_IL = document.getElementById(PageBindings.chkClubLiquorSalesTableRow_IL);
            var chkManufacturerLiquorSales_IL = document.getElementById(PageBindings.chkManufacturerLiquorSales_IL);
            var chkRestaurantLiquorSales_IL = document.getElementById(PageBindings.chkRestaurantLiquorSales_IL);
            var chkPackageStoreLiquorSales_IL = document.getElementById(PageBindings.chkPackageStoreLiquorSales_IL);
            var chkClubLiquorSales_IL = document.getElementById(PageBindings.chkClubLiquorSales_IL);
            var txtManufacturerLiquorSalesTableRow_IL = document.getElementById(PageBindings.txtManufacturerLiquorSalesTableRow_IL);
            var txtRestaurantLiquorSalesTableRow_IL = document.getElementById(PageBindings.txtRestaurantLiquorSalesTableRow_IL);
            var txtPackageStoreLiquorSalesTableRow_IL = document.getElementById(PageBindings.txtPackageStoreLiquorSalesTableRow_IL);
            var txtClubLiquorSalesTableRow_IL = document.getElementById(PageBindings.txtClubLiquorSalesTableRow_IL);

            if (chkManufacturerLiquorSalesTableRow_IL) { chkManufacturerLiquorSalesTableRow_IL.style.display = displayValue; }
            if (chkRestaurantLiquorSalesTableRow_IL) { chkRestaurantLiquorSalesTableRow_IL.style.display = displayValue; }
            if (chkPackageStoreLiquorSalesTableRow_IL) { chkPackageStoreLiquorSalesTableRow_IL.style.display = displayValue; }
            if (chkClubLiquorSalesTableRow_IL) { chkClubLiquorSalesTableRow_IL.style.display = displayValue; }

            if (txtManufacturerLiquorSalesTableRow_IL) {
                if (chkManufacturerLiquorSales_IL && chkManufacturerLiquorSales_IL.checked) {
                    txtManufacturerLiquorSalesTableRow_IL.style.display = displayValue;
                }
                else {
                    txtManufacturerLiquorSalesTableRow_IL.style.display = 'none';
                }
            }

            if (txtRestaurantLiquorSalesTableRow_IL) {
                if (chkRestaurantLiquorSales_IL && chkRestaurantLiquorSales_IL.checked) {
                    txtRestaurantLiquorSalesTableRow_IL.style.display = displayValue;
                }
                else {
                    txtRestaurantLiquorSalesTableRow_IL.style.display = 'none';
                }

            }

            if (txtPackageStoreLiquorSalesTableRow_IL) {
                if (chkPackageStoreLiquorSales_IL && chkPackageStoreLiquorSales_IL.checked) {
                    txtPackageStoreLiquorSalesTableRow_IL.style.display = displayValue;
                }
                else {
                    txtPackageStoreLiquorSalesTableRow_IL.style.display = 'none';
                }

            }

            if (txtClubLiquorSalesTableRow_IL) {
                if (chkClubLiquorSales_IL && chkClubLiquorSales_IL.checked) {
                    txtClubLiquorSalesTableRow_IL.style.display = displayValue;
                }
                else {
                    txtClubLiquorSalesTableRow_IL.style.display = 'none';
                }

            }
        }
        else {
            var chkManufacturerLiquorSalesTableRow_IN = document.getElementById(PageBindings.chkManufacturerLiquorSalesTableRow_IN);
            var chkRestaurantLiquorSalesTableRow_IN = document.getElementById(PageBindings.chkRestaurantLiquorSalesTableRow_IN);
            var chkPackageStoreLiquorSalesTableRow_IN = document.getElementById(PageBindings.chkPackageStoreLiquorSalesTableRow_IN);
            var chkClubLiquorSalesTableRow_IN = document.getElementById(PageBindings.chkClubLiquorSalesTableRow_IN);
            var chkManufacturerLiquorSales_IN = document.getElementById(PageBindings.chkManufacturerLiquorSales_IN);
            var chkRestaurantLiquorSales_IN = document.getElementById(PageBindings.chkRestaurantLiquorSales_IN);
            var chkPackageStoreLiquorSales_IN = document.getElementById(PageBindings.chkPackageStoreLiquorSales_IN);
            var chkClubLiquorSales_IN = document.getElementById(PageBindings.chkClubLiquorSales_IN);
            var txtManufacturerLiquorSalesTableRow_IN = document.getElementById(PageBindings.txtManufacturerLiquorSalesTableRow_IN);
            var txtRestaurantLiquorSalesTableRow_IN = document.getElementById(PageBindings.txtRestaurantLiquorSalesTableRow_IN);
            var txtPackageStoreLiquorSalesTableRow_IN = document.getElementById(PageBindings.txtPackageStoreLiquorSalesTableRow_IN);
            var txtClubLiquorSalesTableRow_IN = document.getElementById(PageBindings.txtClubLiquorSalesTableRow_IN);

            if (chkManufacturerLiquorSalesTableRow_IN) { chkManufacturerLiquorSalesTableRow_IN.style.display = displayValue; }
            if (chkRestaurantLiquorSalesTableRow_IN) { chkRestaurantLiquorSalesTableRow_IN.style.display = displayValue; }
            if (chkPackageStoreLiquorSalesTableRow_IN) { chkPackageStoreLiquorSalesTableRow_IN.style.display = displayValue; }
            if (chkClubLiquorSalesTableRow_IN) { chkClubLiquorSalesTableRow_IN.style.display = displayValue; }

            if (txtManufacturerLiquorSalesTableRow_IN) {
                if (chkManufacturerLiquorSales_IN && chkManufacturerLiquorSales_IN.checked) {
                    txtManufacturerLiquorSalesTableRow_IN.style.display = displayValue;
                }
                else {
                    txtManufacturerLiquorSalesTableRow_IN.style.display = 'none';
                }
            }

            if (txtRestaurantLiquorSalesTableRow_IN) {
                if (chkRestaurantLiquorSales_IN && chkRestaurantLiquorSales_IN.checked) {
                    txtRestaurantLiquorSalesTableRow_IN.style.display = displayValue;
                }
                else {
                    txtRestaurantLiquorSalesTableRow_IN.style.display = 'none';
                }

            }

            if (txtPackageStoreLiquorSalesTableRow_IN) {
                if (chkPackageStoreLiquorSales_IN && chkPackageStoreLiquorSales_IN.checked) {
                    txtPackageStoreLiquorSalesTableRow_IN.style.display = displayValue;
                }
                else {
                    txtPackageStoreLiquorSalesTableRow_IN.style.display = 'none';
                }

            }

            if (txtClubLiquorSalesTableRow_IN) {
                if (chkClubLiquorSales_IN && chkClubLiquorSales_IN.checked) {
                    txtClubLiquorSalesTableRow_IN.style.display = displayValue;
                }
                else {
                    txtClubLiquorSalesTableRow_IN.style.display = 'none';
                }

            }
        }
    }

};

