
$(document).ready(function () {


});

var Cap = new function () {

    this.HiredBorrowedNonOwnedCheckboxChanged = function (chkId, trInfoId, chHasIllinois, trDataRow1Id, trDataRow2Id, trDataRow3Id, trDataRow4Id) {
        var chk = document.getElementById(chkId);
        var trInfo = document.getElementById(trInfoId);
        var HasIllinois = false;
        var trDataRow1 = document.getElementById(trDataRow1Id);
        var trDataRow2 = document.getElementById(trDataRow2Id);
        var trDataRow3 = document.getElementById(trDataRow3Id);
        var trDataRow4 = document.getElementById(trDataRow4Id);


        if (chk && trInfo) {
            trInfo.style.display = 'none';
            if (chHasIllinois == "T") { HasIllinois = true; }
            if (HasIllinois) { trInfo.style.display = ''; }  // Always show if has illinois
            if (trDataRow1) { trDataRow1.style.display = ''; }
            if (chk.checked) {
                // When checked, if the quote has Illinois then show the info row
                //if (HasIllinois) { trInfo.style.display = ''; }
            }
            else {
                // unchecked - make sur ethey really want to delete the coverage
                if (confirm('Are you sure you want to delete this coverage?')) {
                    if (trDataRow1) {
                        trDataRow1.style.display = 'none';
                        this.ClearCoverageFields(trDataRow1Id);
                    }
                    if (trDataRow2) {
                        trDataRow2.style.display = 'none';
                        this.ClearCoverageFields(trDataRow2Id);
                    }
                    if (trDataRow3) {
                        trDataRow3.style.display = 'none';
                        this.ClearCoverageFields(trDataRow3Id);
                    }
                    if (trDataRow4) {
                        trDataRow4.style.display = 'none';
                        this.ClearCoverageFields(trDataRow4Id);
                    }
                }
                else {
                    chk.checked = true;
                    return false;
                }
            }
        }
    };

    this.CoverageCheckboxChanged = function (caller, CheckBoxId, DataTableRowId, DataTableRowId2, DataTableRowId3, DataTableRowId4, chkRentId, chkTowId) {
        var cb = document.getElementById(CheckBoxId);
        var datarow = document.getElementById(DataTableRowId);
        var datarow2 = document.getElementById(DataTableRowId2);
        var datarow3 = document.getElementById(DataTableRowId3);
        var datarow4 = document.getElementById(DataTableRowId4);
        var chkRent = document.getElementById(chkRentId);
        var chkTow = document.getElementById(chkTowId);

        if (cb.checked == true) {
            if (datarow != null) { datarow.style.display = ''; }
            //if (caller) {
            //    if (caller == 'COMP') {
            //        // When Comprehensive is checked, Rental and Towing are enabled
            //        if (chkRent) { chkRent.disabled = false;}
            //        if (chkTow) { chkTow.disabled = false; }
            //    }
            //}
        } else {
            if (confirm('Are you sure you want to delete this coverage?') == true) {
                if (datarow != null) {
                    datarow.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId)
                }
                if (datarow2 != null) {
                    datarow2.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId2)
                }
                if (datarow3 != null) {
                    datarow3.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId3)
                }
                if (datarow4 != null) {
                    datarow4.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId4)
                }
                //if (caller == 'COMP') {
                //    // When Comprehensive is unchecked, Rental and Towing are disabled
                //    if (chkRent) {
                //        chkRent.disabled = true;
                //        chkRent.checked = false;
                //    }
                //    if (chkTow) {
                //        chkTow.disabled = true;
                //        chkTow.checked = false;
                //    }
                //}
            } else {
                cb.checked = true;
                return false;
            }
        };
    };

    this.ClearCoverageFields = function (sender) {
        $('#' + sender + ' input[type=text]').each(function () {
            this.value = '';
        });

        $('#' + sender + ' input[type=checkbox]').each(function () {
            this.checked = false;
        });

        $('#' + sender + ' select').each(function () {
            this.value = '';
        });
    };

    // We need to uncheck the Blanket Waiver of Subro when the Enhancement is unchecked
    this.EnhancementCheckboxChanged = function (ENHCheckBoxId, BWSCheckBoxId) {
        var ENHcb = document.getElementById(ENHCheckBoxId);
        var BWScb = document.getElementById(BWSCheckBoxId);

        if (!ENHcb.checked) {
            if (confirm('Are you sure you want to delete this coverage?')) {
                ENHcb.checked = false;
                BWScb.checked = false;
                return false;
            }
            else {
                ENHcb.checked = true;
                return false;
            }
        }
    };

    // Blanket Waiver of Subro cannot be selected unless the Enhancement has been selected
    this.BlanketWaiverOfSubroCheckboxChanged = function (BWSCheckBoxId, ENHCheckBoxId) {
        var BWScb = document.getElementById(BWSCheckBoxId);
        var ENHcb = document.getElementById(ENHCheckBoxId);

        if (BWScb.checked) {
            if (!ENHcb.checked) {
                alert('The Enhancement Endorsement must be purchased with the Blanket Waiver of Subro');
                BWScb.checked = false;
                return false;
            }
        }
        else {
            if (!confirm('Are you sure you want to delete this coverage?')) {
                BWScb.checked = true;
                return false;
            }
        }
    };

    // Used on the CAP vehicle page
    // Tests to see of the Seasonal Farm Use checkbox needs to be shown
    this.TestForSeasonalFarmUse = function (chkSeasonalFarmUse, ddlSc, SecondaryClassValue, ddlSz, SizeValue, TrailerTypeValue, divSFU) {
        if (ddlSc) {
            if (SecondaryClassValue == '27') { // Secondary Class 27 - Farmers
                if (ddlSz) {
                    if (SizeValue != '18') {
                        if (SizeValue == '30') {  // Trailers
                            if (TrailerTypeValue == '2' || TrailerTypeValue == '3') {
                                // 2 = SemiTrailer, 3 = Trailer
                                // Show checkbox
                                divSFU.style.display = '';
                                chkSeasonalFarmUse.checked = false;
                            }
                            else {
                                // Other trailer types - do not show checkbox
                                divSFU.style.display = 'none';
                                chkSeasonalFarmUse.checked = false;
                            }
                        }
                        else {
                            // Not a Trailer - show the checkbox
                            divSFU.style.display = '';
                            chkSeasonalFarmUse.checked = false;
                        }
                    }
                    else {
                        // Size is 18 - Light Trucks - do not show checkbox
                        divSFU.style.display = 'none';
                        chkSeasonalFarmUse.checked = false;
                    }
                }
            }
        }
    };

    // Used on the CAP vehicle page
    // Tests to see of the Dumping Operations checkbox needs to be shown
    // Any 5 digit class code with '7' in position 4 (xxx7x) requires the dumping operations checkbox
    this.TestForDumpingOperations = function (chkDumpingOps, divDumping, lblCC) {
        if (lblCC && lblCC.innerHTML.length > 4) {
            if (lblCC.innerHTML.substr(3, 1) == '7') {
                divDumping.style.display = '';
                chkDumpingOps.checked = true;
            }
            else {
                divDumping.style.display = 'none';
                chkDumpingOps.checked = false;
            }
        }
        else {
            divDumping.style.display = 'none';
            chkDumpingOps.checked = false;
        }
    };

    // This MASSIVE function handles all of the Vehicle Class Code lookup logic on the CAP vehicle input page
    // Looks up vehicle class code by the passed field values
    // The fields are:
    // - Year
    // - Make
    // - Model
    // - Rating Type
    // - Use Type
    // - Operator Type
    // - Operator Use Type
    // - Vehicle Size
    // - Trailer Type
    // - Radius
    // - Secondary Class
    // - Secondary Class Type
    //
    // All fields are optional and if the lookup doesn't yield anything an empty string will be returned
    // MGB 7-13-2017
    //Updated 07/22/2021 to include hdnSizeId (stored size from VIN lookup in order to set the VIN lookup size as the size default when the size has not yet been set) for CAP Endorsements tasks 53028 and 53030
    //Updated 11/29/2021 for bug 66920 MLW - to include hdnValidVin - when class code begins with 6 (trailer), mark it as a valid vin even if it isn't. Allowing all trailers to pass through VIN lookup with or without using the VIN lookup.
    this.LookupVehicleClassCode = function (yrId, makeId, modelId, rtId, useId, OpId, OpTypId, szId, ttId, rdId, scId, sctId, txtCC1Id, txtCC2Id, caller, trUseCodeId, trOpId, trOpTypId, trSizeId, trTTId, trRadiusId, trSecondaryClassId, trSecondaryClassTypeId, lblCostNewId, chkDumpingOpsId, chkSeasonalFarmUseId, divDumpingId, divSFUId, hdnSizeId, hdnValidVinId) {
        //*** DECLARATIONS ***
        // Controls
        var txtYr = null;
        var txtMake = null;
        var txtModel = null;
        var ddlRt = null;
        var ddlUse = null;
        var ddlOp = null;
        var ddlOpTyp = null;
        var ddlSz = null;
        var ddlTt = null;
        var ddlRd = null;
        var ddlSc = null;
        var ddlSct = null;
        var txtCC1 = null;
        var txtCC2 = null;
        var lblCostNew = null;
        var chkDumpingOps = null;
        var chkSeasonalFarmUse = null;
        var divDumping = null;
        var divSFU = null;
        var hdnSize = null; //Added 07/22/2021 for CAP Endorsements tasks 53028 and 53030 MLW

        // Rows
        var trUseCodeRow = null;
        var trOpRow = null;
        var trOpTypRow = null;
        var trSizeRow = null;
        var trTTRow = null;
        var trRadiusRow = null;
        var trSecondaryClassRow = null;
        var trSecondaryClassTypeRow = null;

        // Selected values
        var YearValue = null;
        var MakeValue = null;
        var ModelValue = null;
        var RatingTypeValue = null;
        var UseValue = null;
        var OperatorValue = null;
        var OperatorTypeValue = null;
        var SizeValue = null;
        var TrailerTypeValue = null;
        var RadiusValue = null;
        var SecondaryClassValue = null;
        var SecondaryClassSelectedItemTextValue = null;
        var SecondaryClassTypeValue = null;
        var DumpingOpsValue = null;
        var SeasonalFarmUseValue = null;
        var ClassCodeValue = null;
        var hdnSizeValue = null; //Added 07/22/2021 for CAP Endorsements tasks 53028 and 53030 MLW

        // *** INITIALIZATION


        // Get the divs
        if (divDumpingId) {
            divDumping = document.getElementById(divDumpingId);
        }
        if (divSFUId) {
            divSFU = document.getElementById(divSFUId);
        }

        // Get the rows
        if (trUseCodeId) {
            trUseCodeRow = document.getElementById(trUseCodeId);
        }
        if (trOpId) {
            trOpRow = document.getElementById(trOpId);
        }
        if (trOpTypId) {
            trOpTypRow = document.getElementById(trOpTypId);
        }
        if (trSizeId) {
            trSizeRow = document.getElementById(trSizeId);
        }
        if (trTTId) {
            trTTRow = document.getElementById(trTTId);
        }
        if (trRadiusId) {
            trRadiusRow = document.getElementById(trRadiusId);
        }
        if (trSecondaryClassId) {
            trSecondaryClassRow = document.getElementById(trSecondaryClassId);
        }
        if (trSecondaryClassTypeId) {
            trSecondaryClassTypeRow = document.getElementById(trSecondaryClassTypeId);
        }

        // Get the Classcode textboxes
        if (txtCC1Id) {
            txtCC1 = document.getElementById(txtCC1Id);
        }
        // Note that txtCC2 is actually the class code label in the lookup section
        if (txtCC2Id) {
            txtCC2 = document.getElementById(txtCC2Id);
        }

        // Cost New label
        if (lblCostNewId) {
            lblCostNew = document.getElementById(lblCostNewId);
        }

        // Get the class code lookup controls & their values
        if (yrId) {
            txtYr = document.getElementById(yrId);
        }
        if (modelId) {
            txtModel = document.getElementById(modelId);
        }
        if (makeId) {
            txtMake = document.getElementById(makeId);
        }
        if (rtId) {
            ddlRt = document.getElementById(rtId);
        }
        if (useId) {
            ddlUse = document.getElementById(useId);
        }
        if (OpId) {
            ddlOp = document.getElementById(OpId);
        }
        if (OpTypId) {
            ddlOpTyp = document.getElementById(OpTypId);
        }
        if (szId) {
            ddlSz = document.getElementById(szId);
        }
        if (ttId) {
            ddlTt = document.getElementById(ttId);
        }
        if (rdId) {
            ddlRd = document.getElementById(rdId);
        }
        if (scId) {
            ddlSc = document.getElementById(scId);
        }
        if (sctId) {
            ddlSct = document.getElementById(sctId);
        }
        if (chkDumpingOpsId) {
            chkDumpingOps = document.getElementById(chkDumpingOpsId);
        }
        if (chkSeasonalFarmUseId) {
            chkSeasonalFarmUse = document.getElementById(chkSeasonalFarmUseId);
        }
        //Added 07/22/2021 for CAP Endorsements tasks 53028 and 53030 MLW
        if (hdnSizeId) {
            hdnSize = document.getElementById(hdnSizeId);
        }

        // Get the control values
        // Get the control values
        if (txtCC2) { ClassCodeValue = txtCC2.innerHTML; }
        if (txtYr) { YearValue = txtYr.value; }
        if (txtModel) { ModelValue = txtModel.value; }
        if (txtMake) { MakeValue = txtMake.value; }
        if (ddlRt) { RatingTypeValue = ddlRt.value; }
        if (ddlUse) { UseValue = ddlUse.value; }
        if (ddlOp) { OperatorValue = ddlOp.value; }
        if (ddlOpTyp) { OperatorTypeValue = ddlOpTyp.value; }
        if (ddlSz) { SizeValue = ddlSz.value; }
        if (ddlTt) { TrailerTypeValue = ddlTt.value; }
        if (ddlRd) { RadiusValue = ddlRd.value; }
        if (ddlSc) {
            SecondaryClassValue = ddlSc.value;
            SecondaryClassSelectedItemTextValue = ddlSc.options[ddlSc.selectedIndex].text;
        }
        if (ddlSct) { SecondaryClassTypeValue = ddlSct.value; }
        if (chkDumpingOps) { DumpingOpsValue = chkDumpingOps.checked; }
        if (chkSeasonalFarmUse) { SeasonalFarmUseValue = chkSeasonalFarmUse.checked; }
        if (hdnSize) { hdnSizeValue = hdnSize.value; } //Added 07/22/2021 for CAP Endorsements tasks 53028 and 53030 MLW

        // *** SET THE CLASS CODE LOOKUP DROPDOWNS
        // - Load dropdowns based on selected values 
        // - Hide/show appropriate dropdowns based on selected values
        if (caller) {
            var option = null;

            Cap.HideAndClearLookupRows(caller, trUseCodeRow, ddlUse, trOpRow, ddlOp, trOpTypRow, ddlOpTyp, trSizeRow, ddlSz, trTTRow, ddlTt, trRadiusRow, ddlRd, trSecondaryClassRow, ddlSc, trSecondaryClassTypeRow, ddlSct, divSFU, divDumping)

            switch (caller) {
                case 'VRT':   // Vehicle Rating Type
                    Cap.LoadUseTypeValues(RatingTypeValue, ddlUse, trUseCodeRow);

                    break;
                case 'USE':  // Use Code
                    // If Antique Auto (22) Cost New is required
                    if (UseValue) {
                        if (UseValue == '22') {
                            lblCostNew.innerHTML = '*Cost New';
                            lblCostNew.style.color = 'red';
                        }
                        else {
                            lblCostNew.innerHTML = 'Cost New';
                            lblCostNew.style.color = 'black';
                        }
                    }

                    // If Vehicle Use Code is 'Private Passenger Type' AND...
                    // - Use Code is Anything but 'Business' (20) or 'Antique Auto' (22) or 'Farm' (23), Operator Use and Operator Type ARE required
                    // - Use Code is 'Business' or 'Antique Auto' then Operator Use and Operator Type ARE NOT required
                    switch (RatingTypeValue) {
                        case '1':
                            if (UseValue == '20' || UseValue == '22' || UseValue == '23' || UseValue == '') {
                                trOpRow.style.display = 'none';
                                ddlOp.value = '';
                                trOpTypRow.style.display = 'none';
                                ddlOpTyp.value = '';
                            }
                            else {
                                trOpRow.style.display = '';
                            }
                            break;
                        case '9':
                            // Show the size row
                            trSizeRow.style.display = '';
                            //Added 07/22/2021 for CAP Endorsements tasks 53028 and 53030 MLW - size returned from VIN lookup is stored in hdnSize. Need to set the drop down to the VIN lookup size and not let the user change it. Change() is needed to update the class code and show the next drop down, if necessary.
                            if (hdnSizeValue != "") {
                                SizeValue = hdnSizeValue;
                                ddlSz.value = SizeValue;
                                $("#" + szId).change();
                                $("#" + szId).attr('disabled', 'disabled');
                            }
                            break;
                        default:
                            trOpRow.style.display = 'none';
                            trOpTypRow.style.display = 'none';
                            trSizeRow.style.display = 'none';
                            trRadiusRow.style.display = 'none';
                            trSecondaryClassRow.style.display = 'none';
                            trSecondaryClassTypeRow.style.display = 'none';
                            break;
                    }

                    break;
                case "OP":  // Operator 
                    if (OperatorValue) {
                        trOpTypRow.style.display = '';
                    }
                    else {
                        trOpTypRow.style.display = 'none';
                    }
                    break;
                case "OPTYP":  // Operator Type
                    break;
                case 'SIZE':   // Vehicle Size
                    if (SizeValue) {
                        switch (SizeValue) {
                            case '30':
                                //Added 08/23/2021 for Bug 64448 MLW
                                // Don't show use code for size 30 - Trailer Types
                                trUseCodeRow.style.display = "none";
                                // Trailer was selected - show the trailer row
                                if (trTTRow) { trTTRow.style.display = ''; }
                                else { trTTRow.style.display = 'none'; }
                                break;
                            case '21':
                                // Don't show use code for size 21 - Extra Heavy Truck
                                trUseCodeRow.style.display = "none";
                                trRadiusRow.style.display = '';
                                break;
                            case '23':
                                //Added 08/23/2021 for Bug 64448 MLW
                                // Don't show use code for size 23 - Extra Heavy Truck-Tractors
                                trUseCodeRow.style.display = "none";
                                trRadiusRow.style.display = '';
                                break;
                            case '':
                                trRadiusRow.style.display = 'none';
                                break;
                            default:
                                trUseCodeRow.style.display = '';
                                trRadiusRow.style.display = '';
                        }
                    }

                    break;
                case 'TT':   // Trailer Type
                    if (TrailerTypeValue) {
                        trRadiusRow.style.display = '';
                    }
                    else {
                        trRadiusRow.style.display = 'none';
                    }

                    break;
                case 'RD':   // Radius
                    if (RadiusValue) {
                        trSecondaryClassRow.style.display = '';
                    }
                    else {
                        trSecondaryClassRow.style.display = 'none';
                    }
                    break;
                case 'SC':   // Secondary Class
                    Cap.LoadSecondaryClassTypeValues(SecondaryClassSelectedItemTextValue, ddlSct, trSecondaryClassTypeRow)

                    break;
                case 'SCT':  // Secondary Class Type
                    //Cap.TestForDumpingOperations(chkDumpingOps, divDumping, txtCC2)
                    break;
                default:
                    break;
            }
        }

        // Set the genhandler command & parms
        var genHandler = 'GenHandlers/Vr_Comm/VehicleClassCodeLookup.ashx'
        if (YearValue) { genHandler += '?yr=' + encodeURIComponent(YearValue.trim()); } else { genHandler += '?yr=' + encodeURIComponent(null); }
        if (MakeValue) { genHandler += "&mk=" + encodeURIComponent(MakeValue.trim()); }
        if (ModelValue) { genHandler += "&md=" + encodeURIComponent(ModelValue.trim()); }
        if (RatingTypeValue) { genHandler += "&rtId=" + encodeURIComponent(RatingTypeValue.trim()); }
        if (UseValue) { genHandler += "&ucId=" + encodeURIComponent(UseValue.trim()); }
        if (OperatorValue) { genHandler += "&opId=" + encodeURIComponent(OperatorValue.trim()); }
        if (OperatorTypeValue) { genHandler += "&optypId=" + encodeURIComponent(OperatorTypeValue.trim()); }
        if (SizeValue) { genHandler += "&szId=" + encodeURIComponent(SizeValue.trim()); }
        if (TrailerTypeValue) { genHandler += "&ttId=" + encodeURIComponent(TrailerTypeValue.trim()); }
        if (RadiusValue) { genHandler += "&rdId=" + encodeURIComponent(RadiusValue.trim()); }
        if (SecondaryClassValue) { genHandler += "&scId=" + encodeURIComponent(SecondaryClassValue.trim()); }
        if (SecondaryClassTypeValue) { genHandler += "&sctId=" + encodeURIComponent(SecondaryClassTypeValue.trim()); }

        // Call the generic handler
        $.getJSON(genHandler, {
            dataType: "json",
            data: "",
            cache: false,
            format: "json"
        })
            .done(function (data) {
                // Use the data returned
                if (data && data.length > 3) {
                    // Set the class code textboxes
                    if (txtCC1) { txtCC1.value = data; }
                    if (txtCC2) { txtCC2.innerHTML = data; }
                    if (data.charAt(0) == '6') { hdnValidVinId.value = 'True'; } //Added 11/29/2021 for bug 66920 MLW - all trailers (first character of class code is 6) will bypass the VIN lookup
                    Cap.TestForDumpingOperations(chkDumpingOps, divDumping, txtCC2)
                    Cap.TestForSeasonalFarmUse(chkSeasonalFarmUse, ddlSc, SecondaryClassValue, ddlSz, SizeValue, TrailerTypeValue, divSFU);
                }
                else {
                    // Clear the class code textboxes
                    // Set the class code textboxes
                    if (txtCC1) { txtCC1.value = ''; }
                    if (txtCC2) { txtCC2.innerHTML = ''; }
                    Cap.TestForDumpingOperations(chkDumpingOps, divDumping, txtCC2)
                    Cap.TestForSeasonalFarmUse(chkSeasonalFarmUse, ddlSc, SecondaryClassValue, ddlSz, SizeValue, TrailerTypeValue, divSFU);
                }
                //clear Error
            }).error(function (err) {
                // Error handler
                alert("Generic Handler encountered an error!");
            });

    };

    this.HideAndClearLookupRows = function (caller, trUseCodeRow, ddlUse, trOpRow, ddlOp, trOpTypRow, ddlOpTyp, trSizeRow, ddlSz, trTTRow, ddlTt, trRadiusRow, ddlRd, trSecondaryClassRow, ddlSc, trSecondaryClassTypeRow, ddlSct, divSFU, divDumping) {
        // Always hide the Dumping and Seasonal Farm Use rows
        if (divSFU) { divSFU.style.display = 'none'; }
        if (divDumping) { divDumping.style.display = 'none' }

        // Hide all the lookup rows after the caller
        switch (caller) {
            case "VRT":
                if (trUseCodeRow) { trUseCodeRow.style.display = 'none'; }
                if (ddlUse) { ddlUse.selectedIndex = '0'; }
                if (trOpRow) { trOpRow.style.display = 'none'; }
                if (ddlOp) { ddlOp.selectedIndex = '0'; }
                if (trOpTypRow) { trOpTypRow.style.display = 'none'; }
                if (ddlOpTyp) { ddlOpTyp.selectedIndex = '0'; }
                if (trSizeRow) { trSizeRow.style.display = 'none'; }
                if (ddlSz) { ddlSz.selectedIndex = '0'; }
                if (trTTRow) { trTTRow.style.display = 'none'; }
                if (ddlTt) { ddlTt.selectedIndex = '0'; }
                if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
                if (ddlRd) { ddlRd.selectedIndex = '0'; }
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "USE":
                if (trOpRow) { trOpRow.style.display = 'none'; }
                if (ddlOp) { ddlOp.selectedIndex = '0'; }
                if (trOpTypRow) { trOpTypRow.style.display = 'none'; }
                if (ddlOpTyp) { ddlOpTyp.selectedIndex = '0'; }
                if (trSizeRow) { trSizeRow.style.display = 'none'; }
                if (ddlSz) { ddlSz.selectedIndex = '0'; }
                if (trTTRow) { trTTRow.style.display = 'none'; }
                if (ddlTt) { ddlTt.selectedIndex = '0'; }
                if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
                if (ddlRd) { ddlRd.selectedIndex = '0'; }
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "OP":
                if (trOpTypRow) { trOpTypRow.style.display = 'none'; }
                if (ddlOpTyp) { ddlOpTyp.selectedIndex = '0'; }
                if (trSizeRow) { trSizeRow.style.display = 'none'; }
                if (ddlSz) { ddlSz.selectedIndex = '0'; }
                if (trTTRow) { trTTRow.style.display = 'none'; }
                if (ddlTt) { ddlTt.selectedIndex = '0'; }
                if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
                if (ddlRd) { ddlRd.selectedIndex = '0'; }
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "OPTYP":
                if (trSizeRow) { trSizeRow.style.display = 'none'; }
                if (ddlSz) { ddlSz.selectedIndex = '0'; }
                if (trTTRow) { trTTRow.style.display = 'none'; }
                if (ddlTt) { ddlTt.selectedIndex = '0'; }
                if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
                if (ddlRd) { ddlRd.selectedIndex = '0'; }
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "SIZE":
                if (trTTRow) { trTTRow.style.display = 'none'; }
                if (ddlTt) { ddlTt.selectedIndex = '0'; }
                if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
                if (ddlRd) { ddlRd.selectedIndex = '0'; }
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "TT":
                if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
                if (ddlRd) { ddlRd.selectedIndex = '0'; }
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "RD":
                if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
                if (ddlSc) { ddlSc.selectedIndex = '0'; }
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "SC":
                if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
                if (ddlSct) { ddlSct.selectedIndex = '0'; }
                break;
            case "SCT":
                break;
            default:
                break;
        }
    };

    this.ReverseClassCodeLookup = function (txtCCId, lblCCId, rtId, useId, OpId, OpTypId, szId, ttId, rdId, scId, sctId, trUseCodeId, trOpId, trOpTypId, trSizeId, trTTId, trRadiusId, trSecondaryClassId, trSecondaryClassTypeId, chkDumpingOpsId, chkSeasonalFarmUseId, divDumpingId, divSFUId) {
        // Controls
        var ddlRt = null;
        var ddlUse = null;
        var ddlOp = null;
        var ddlOpTyp = null;
        var ddlSz = null;
        var ddlTt = null;
        var ddlRd = null;
        var ddlSc = null;
        var ddlSct = null;
        var txtClassCode = null;
        var lblClassCode = null;
        var chkDumpingOps = null;
        var chkSeasonalFarmUse = null;
        var divDumping = null;
        var divSFU = null;

        // Rows
        var trUseCodeRow = null;
        var trOpRow = null;
        var trOpTypRow = null;
        var trSizeRow = null;
        var trTTRow = null;
        var trRadiusRow = null;
        var trSecondaryClassRow = null;
        var trSecondaryClassTypeRow = null;

        // Selected values
        var RatingTypeValue = null;
        var UseValue = null;
        var OperatorValue = null;
        var OperatorTypeValue = null;
        var SizeValue = null;
        var TrailerTypeValue = null;
        var RadiusValue = null;
        var SecondaryClassValue = null;
        var SecondaryClassSelectedItemTextValue = null;
        var SecondaryClassTypeValue = null;
        var DumpingOpsValue = null;
        var SeasonalFarmUseValue = null;
        var ClassCodeValue = null;

        // *** INITIALIZATION

        // Get the divs
        if (divDumpingId) {
            divDumping = document.getElementById(divDumpingId);
        }
        if (divSFUId) {
            divSFU = document.getElementById(divSFUId);
        }

        // Get the rows
        if (trUseCodeId) {
            trUseCodeRow = document.getElementById(trUseCodeId);
        }
        if (trOpId) {
            trOpRow = document.getElementById(trOpId);
        }
        if (trOpTypId) {
            trOpTypRow = document.getElementById(trOpTypId);
        }
        if (trSizeId) {
            trSizeRow = document.getElementById(trSizeId);
        }
        if (trTTId) {
            trTTRow = document.getElementById(trTTId);
        }
        if (trRadiusId) {
            trRadiusRow = document.getElementById(trRadiusId);
        }
        if (trSecondaryClassId) {
            trSecondaryClassRow = document.getElementById(trSecondaryClassId);
        }
        if (trSecondaryClassTypeId) {
            trSecondaryClassTypeRow = document.getElementById(trSecondaryClassTypeId);
        }

        // Get the Classcode textbox and label
        if (txtCCId) {
            txtClassCode = document.getElementById(txtCCId);
            if (txtClassCode) { ClassCodeValue = txtClassCode.value; }
        }

        if (lblCCId) {
            lblClassCode = document.getElementById(lblCCId);
            // Clear the lookup class code label
            lblClassCode.innerHTML = '';
        }

        // Get the class code lookup controls & their values
        if (rtId) {
            ddlRt = document.getElementById(rtId);
        }
        if (useId) {
            ddlUse = document.getElementById(useId);
        }
        if (OpId) {
            ddlOp = document.getElementById(OpId);
        }
        if (OpTypId) {
            ddlOpTyp = document.getElementById(OpTypId);
        }
        if (szId) {
            ddlSz = document.getElementById(szId);
        }
        if (ttId) {
            ddlTt = document.getElementById(ttId);
        }
        if (rdId) {
            ddlRd = document.getElementById(rdId);
        }
        if (scId) {
            ddlSc = document.getElementById(scId);
        }
        if (sctId) {
            ddlSct = document.getElementById(sctId);
        }
        if (chkDumpingOpsId) {
            chkDumpingOps = document.getElementById(chkDumpingOpsId);
        }
        if (chkSeasonalFarmUseId) {
            chkSeasonalFarmUse = document.getElementById(chkSeasonalFarmUseId);
        }

        // DON'T TRY THE LOOKUP UNLESS CLASS CODE LENGTH IS AT LEAST 4 CHARACTERS
        if (ClassCodeValue == null || ClassCodeValue.length < 4) {
            // Reset the vehicle rating type dropdown
            ddlRt.selectedIndex = -1;
            return;
        }

        // Hide all the lookup fields - we will display them as needed
        if (trUseCodeRow) { trUseCodeRow.style.display = 'none'; }
        if (trOpRow) { trOpRow.style.display = 'none'; }
        if (trOpTypRow) { trOpTypRow.style.display = 'none'; }
        if (trSizeRow) { trSizeRow.style.display = 'none'; }
        if (trTTRow) { trTTRow.style.display = 'none'; }
        if (trRadiusRow) { trRadiusRow.style.display = 'none'; }
        if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; }
        if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; }
        if (divSFU) {
            divSFU.display = 'none';
            chkSeasonalFarmUse.checked = false;
        }
        if (divDumping) {
            divDumping.display = 'none';
            chkDumpingOps.checked = false;
        }

        // Set the genhandler command & parms
        var genHandler = 'GenHandlers/Vr_Comm/ReverseVehicleClassCodeLookup.ashx'
        if (ClassCodeValue) { genHandler += '?ClassCode=' + encodeURIComponent(ClassCodeValue.trim()); } else { genHandler += '?ClassCode=' + encodeURIComponent(null); }

        // Call the generic handler
        $.getJSON(genHandler, {
            dataType: "json",
            data: "",
            cache: false,
            format: "json"
        })
            .done(function (data) {
                // Use the data returned
                if (data) {


                    // Check Class code for exceptable use
                    // CAH 10/03/2017
                    var badCAPCodes = ["21", "22", "23", "24", "25", "26", "29", "41", "42", "43", "44", "49", "51", "52", "53", "54", "55"];
                    //var badCAPCodeMsg = "THIS CLASS CODE IS NOT ELIGIBLE FOR QUOTING IN VELOCIRATER. PLEASE CONTACT YOUR UNDERWRITER.";
                    var badCAPHideCode = "VRT"
                    if (ClassCodeValue.length == 5) {
                        var test = ClassCodeValue.slice(3)
                        if (jQuery.inArray(ClassCodeValue.slice(3), badCAPCodes) !== -1) {
                            // Class code is one we don't allow
                            Cap.HideAndClearLookupRows(badCAPHideCode, trUseCodeRow, ddlUse, trOpRow, ddlOp, trOpTypRow, ddlOpTyp, trSizeRow, ddlSz, trTTRow, ddlTt, trRadiusRow, ddlRd, trSecondaryClassRow, ddlSc, trSecondaryClassTypeRow, ddlSct, divSFU, divDumping)
                            ddlRt.selectedIndex = -1
                            txtClassCode.value = ""
                            lblClassCode.innerHTML = '';
                            alert(badCAPCodeMsg);
                            return
                        }
                    }
                    //Added 2/10/2022 for bug 63488 MLW
                    if (removeAntiqueAutoFlag.toUpperCase() == 'TRUE' && ClassCodeValue == '9620' && (ifm.vr.currentQuote.isEndorsement == false || (ifm.vr.currentQuote.isEndorsement == true && myVehicleIsNewVehicleOnEndorsement.toUpperCase() == 'TRUE'))) {
                        // Class code is antique auto and we don't allow
                        Cap.HideAndClearLookupRows(badCAPHideCode, trUseCodeRow, ddlUse, trOpRow, ddlOp, trOpTypRow, ddlOpTyp, trSizeRow, ddlSz, trTTRow, ddlTt, trRadiusRow, ddlRd, trSecondaryClassRow, ddlSc, trSecondaryClassTypeRow, ddlSct, divSFU, divDumping);
                        ddlRt.selectedIndex = -1;
                        txtClassCode.value = "";
                        lblClassCode.innerHTML = '';
                        alert(badCAPCodeMsg);
                        return
                    }



                    // LOAD THE CLASS LOOKUP CONTROLS

                    // Rating Type
                    if (data.VehicleRatingTypeId && ddlRt) {
                        ddlRt.value = data.VehicleRatingTypeId;
                        RatingTypeValue = ddlRt.value;
                    }
                    else {
                        ddlRt.selectedIndex = -1;
                    }

                    // Use Code
                    if (data.UseCodeId && ddlUse) {
                        Cap.LoadUseTypeValues(RatingTypeValue, ddlUse, trUseCodeRow, ddlSz, ddlRd, ddlSc)
                        if (Cap.checkValidDDLListItem(data.UseCodeId, ddlUse)) {
                            ddlUse.value = data.UseCodeId;
                            //Added 08/23/2021 for Bug 62765 MLW
                            trUseCodeRow.style.display = '';
                        } else {
                            //Added 08/23/2021 for Bug 62765 MLW
                            trUseCodeRow.style.display = 'none';
                        }
                    }

                    // Operator Type
                    if (Cap.checkValidDDLListItem(data.OperatorTypeId, ddlOp)) {
                        ddlOp.value = data.OperatorTypeId;
                        trOpRow.style.display = '';
                    }
                    else { if (trOpTypRow) { trOpTypRow.style.display = 'none'; } }

                    // Operator Use
                    if (Cap.checkValidDDLListItem(data.OperatorUseId, ddlOpTyp)) {
                        ddlOpTyp.value = data.OperatorUseId;
                        trOpTypRow.style.display = '';
                    }
                    else { if (trOpTypRow) { trOpTypRow.style.display = 'none'; } }

                    // Size
                    if (Cap.checkValidDDLListItem(data.SizeId, ddlSz)) {
                        ddlSz.value = data.SizeId;
                        trSizeRow.style.display = '';
                    }
                    else { if (trSizeRow) { trSizeRow.style.display = 'none'; } }

                    // Trailer Type
                    // Don't show N/A (id 0)
                    if (Cap.checkValidDDLListItem(data.TrailerTypeId, ddlTt)) {
                        ddlTt.value = data.TrailerTypeId;
                        trTTRow.style.display = '';
                    }
                    else { if (trTTRow) { trTTRow.style.display = 'none'; } }

                    // Radius
                    if (Cap.checkValidDDLListItem(data.RadiusId, ddlRd)) {
                        ddlRd.value = data.RadiusId;
                        trRadiusRow.style.display = '';
                    }
                    else { if (trRadiusRow) { trRadiusRow.style.display = 'none'; } }

                    // Secondary Class
                    if (Cap.checkValidDDLListItem(data.SecondaryClassId, ddlSc)) {
                        ddlSc.value = data.SecondaryClassId;
                        SecondaryClassSelectedItemTextValue = ddlSc.options[ddlSc.selectedIndex].text;
                        trSecondaryClassRow.style.display = '';
                    }
                    else { if (trSecondaryClassRow) { trSecondaryClassRow.style.display = 'none'; } }

                    // Secondary Class Type
                    if (data.SecondaryClassTypeId && ddlSct) {
                        Cap.LoadSecondaryClassTypeValues(SecondaryClassSelectedItemTextValue, ddlSct, trSecondaryClassTypeRow)
                        if (Cap.checkValidDDLListItem(data.SecondaryClassTypeId, ddlSct)) {
                            ddlSct.value = data.SecondaryClassTypeId;
                            trSecondaryClassTypeRow.style.display = '';
                        }
                        else { if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; } }
                    }
                    else { if (trSecondaryClassTypeRow) { trSecondaryClassTypeRow.style.display = 'none'; } }

                    if (ddlRt.selectedIndex != -1) { lblClassCode.innerHTML = txtClassCode.value; }

                    // Get the control values
                    //if (txtClassCode) { ClassCodeValue = txtClassCode.value; }
                    //if (txtYr) { YearValue = txtYr.value; }
                    //if (txtModel) { ModelValue = txtModel.value; }
                    //if (txtMake) { MakeValue = txtMake.value; }
                    if (ddlRt) { RatingTypeValue = ddlRt.value; }
                    if (ddlUse) { UseValue = ddlUse.value; }
                    if (ddlOp) { OperatorValue = ddlOp.value; }
                    if (ddlOpTyp) { OperatorTypeValue = ddlOpTyp.value; }
                    if (ddlSz) { SizeValue = ddlSz.value; }
                    if (ddlTt) { TrailerTypeValue = ddlTt.value; }
                    if (ddlRd) { RadiusValue = ddlRd.value; }
                    if (ddlSc) {
                        SecondaryClassValue = ddlSc.value;
                        SecondaryClassSelectedItemTextValue = ddlSc.options[ddlSc.selectedIndex].text;
                    }
                    if (ddlSct) { SecondaryClassTypeValue = ddlSct.value; }
                    if (chkDumpingOps) { DumpingOpsValue = chkDumpingOps.checked; }
                    if (chkSeasonalFarmUse) { SeasonalFarmUseValue = chkSeasonalFarmUse.checked; }

                    //Added 11/29/2021 for bug 66920 MLW
                    if (ClassCodeValue.charAt(0) == '6') { hdnValidVinId.value = 'True'; } //Added 11/29/2021 for bug 66920 MLW - all trailers (first character of class code is 6) will bypass the VIN lookup

                    // Set the Seasonal Farm Use & Dumping Operations checkboxes
                    Cap.TestForDumpingOperations(chkDumpingOps, divDumping, lblClassCode);
                    Cap.TestForSeasonalFarmUse(chkSeasonalFarmUse, ddlSc, SecondaryClassValue, ddlSz, SizeValue, TrailerTypeValue, divSFU);
                }
                else {
                    // No data returned - do something
                    return;
                }
            }).error(function (err) {
                // Error handler
                alert("Generic Handler encountered an error!");
            });

    };

    this.LoadUseTypeValues = function (RatingTypeValue, ddlUse, trUseCodeRow, ddlSz, ddlRd, ddlSc) {
        var option = null;

        switch (RatingTypeValue) {
            case '1':  // Private Passenger Type
                if (ddlUse) {
                    // Set Use Code dropdown values
                    ddlUse.options.length = 0;  // Clear the list

                    option = null;
                    option = document.createElement('option');
                    option.text = '';
                    option.value = '';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Business';
                    option.value = '20';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Personal';
                    option.value = '21';
                    ddlUse.add(option);

                    //Updated 2/10/2022 for bug 63488 MLW
                    if (removeAntiqueAutoFlag.toUpperCase() == 'TRUE') {
                        if (ifm.vr.currentQuote.isEndorsement == true && myVehicleIsNewVehicleOnEndorsement.toUpperCase() == 'FALSE') {
                            //Only allow Antique Auto as an option on existing vehicles on endorsements, all others remove this option
                            option = null;
                            option = document.createElement('option');
                            option.text = 'Antique Auto';
                            option.value = '22';
                            ddlUse.add(option);
                        }
                    } else {
                        option = null;
                        option = document.createElement('option');
                        option.text = 'Antique Auto';
                        option.value = '22';
                        ddlUse.add(option);
                    }

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Farm';
                    option.value = '23';
                    ddlUse.add(option);

                    // show the use code row
                    if (trUseCodeRow) { trUseCodeRow.style.display = ''; }
                }
                break;
            case '7':  // Funeral Director
                if (ddlUse) {
                    // Set Use Code dropdown values
                    ddlUse.options.length = 0;  // Clear the list

                    option = null;
                    option = document.createElement('option');
                    option.text = '';
                    option.value = '';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Limousine';
                    option.value = '2';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Hearse or Flower Car';
                    option.value = '31';
                    ddlUse.add(option);

                    // show the use code row
                    if (trUseCodeRow) { trUseCodeRow.style.display = ''; }
                }
                break;
            case '9':
                //Truck, trailer, tractor
                // Set Use Code dropdown values
                if (ddlUse) {
                    ddlUse.options.length = 0;  // Clear the list

                    option = null;
                    option = document.createElement('option');
                    option.text = '';
                    option.value = '';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Service';
                    option.value = '28';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Retail';
                    option.value = '29';
                    ddlUse.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Commercial';
                    option.value = '30';
                    ddlUse.add(option);

                    // show the use code row
                    if (trUseCodeRow) { trUseCodeRow.style.display = ''; }
                }

                if (ddlSz) {
                    // Set size dropdown values
                    ddlSz.options.length = 0;  // Clear the list

                    option = null;
                    option = document.createElement('option');
                    option.text = '';
                    option.value = '';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Light Truck < or equal 10,000 Pounds GVW';
                    option.value = '18';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Medium Truck 10,001 to 20,000 Pounds GVW';
                    option.value = '19';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Heavy Truck 20,001 to 45,000 Pounds GVW';
                    option.value = '20';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Extra Heavy Truck > 45,000 Pounds GVW';
                    option.value = '21';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Heavy Truck-Tractors < or equal 45,000 Pounds GVW';
                    option.value = '22';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Extra Heavy Truck-Tractors > 45,000 Pounds GVW';
                    option.value = '23';
                    ddlSz.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Trailer Types';
                    option.value = '30';
                    ddlSz.add(option);
                }

                if (ddlRd) {
                    // Set radius dropdown values
                    ddlRd.options.length = 0;  // Clear the list

                    option = null;
                    option = document.createElement('option');
                    option.text = '';
                    option.value = '';
                    ddlRd.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Local, up to 50 miles';
                    option.value = '1';
                    ddlRd.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Intermediate, 51 to 200 miles';
                    option.value = '2';
                    ddlRd.add(option);
                }

                if (ddlSc) {
                    // Set secondary class dropdown values
                    ddlSc.options.length = 0;  // Clear the list

                    option = null;
                    option = document.createElement('option');
                    option.text = '';
                    option.value = '';
                    ddlSc.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Food Delivery';
                    option.value = '26';
                    ddlSc.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Farmers';
                    option.value = '27';
                    ddlSc.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Dump and Transit Mix';
                    option.value = '28';
                    ddlSc.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Contractors';
                    option.value = '29';
                    ddlSc.add(option);

                    option = null;
                    option = document.createElement('option');
                    option.text = 'Not Otherwise Specified';
                    option.value = '30';
                    ddlSc.add(option);
                }
                break;
            default:
                break;
        }
    };

    this.LoadSecondaryClassTypeValues = function (SecondaryClassSelectedItemTextValue, ddlSct, trSecondaryClassTypeRow) {
        var option = null;

        // Load the secondary class type ddl
        ddlSct.options.length = 0;  // Clear the list
        switch (SecondaryClassSelectedItemTextValue) {
            case 'Food Delivery':
                option = null;
                option = document.createElement('option');
                option.text = '';
                option.value = '';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Canneries';
                option.value = '10';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Fish and Seafood';
                option.value = '11';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Frozen Foods';
                option.value = '12';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Fruit and Vegetables';
                option.value = '13';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Meat or Poultry';
                option.value = '14';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'All Others';
                option.value = '9';
                ddlSct.add(option);

                trSecondaryClassTypeRow.style.display = '';

                break;
            case 'Farmers':
                option = null;
                option = document.createElement('option');
                option.text = '';
                option.value = '';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Individual or Family Owned Corporation (not hauling livestock)';
                option.value = '23';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Livestock';
                option.value = '24';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'All Other';
                option.value = '9';
                ddlSct.add(option);

                trSecondaryClassTypeRow.style.display = '';

                break;
            case "Dump and Transit Mix":
                option = null;
                option = document.createElement('option');
                option.text = '';
                option.value = '';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Excavating';
                option.value = '25';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Sand and Gravel (other than quarrying)';
                option.value = '26';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Mining';
                option.value = '27';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Quarrying';
                option.value = '28';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'All Other';
                option.value = '9';
                ddlSct.add(option);

                trSecondaryClassTypeRow.style.display = '';

                break;
            case "Contractors":
                option = null;
                option = document.createElement('option');
                option.text = '';
                option.value = '';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Building - Commercial';
                option.value = '29';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Building - Private Dwelling';
                option.value = '30';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Repair or Service';
                option.value = '31';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Excavating';
                option.value = '25';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Street and Road';
                option.value = '32';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'All Other';
                option.value = '9';
                ddlSct.add(option);

                trSecondaryClassTypeRow.style.display = '';

                break;
            case "Not Otherwise Specified":
                option = null;
                option = document.createElement('option');
                option.text = '';
                option.value = '';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'Logging and Lumbering';
                option.value = '33';
                ddlSct.add(option);

                option = null;
                option = document.createElement('option');
                option.text = 'All Other';
                option.value = '9';
                ddlSct.add(option);

                trSecondaryClassTypeRow.style.display = '';

                break;
            default:
                trSecondaryClassTypeRow.style.display = 'none';
                break;
        }
    };

    /// Added 9/6/18 to handle Multistate Zipcode lookup  MGB
    this.DoMultiStateCityCountyLookup = function (txtZipId, txtCityId, txtStateId, txtCountyId, StatesOnQuote, trUMPDRowId, trUMPDLimitRowId, chkUMPDId, txtUMPDLimitId, isUMPDChangeAvailable, quoteState, ILUMPDLimit) {
        var txtZip = document.getElementById(txtZipId);
        var txtCity = document.getElementById(txtCityId);
        var txtState = document.getElementById(txtStateId);
        var txtCounty = document.getElementById(txtCountyId);
        var trUMPD = document.getElementById(trUMPDRowId);
        var trUMPDLimit = document.getElementById(trUMPDLimitRowId);
        var chkUMPD = document.getElementById(chkUMPDId);
        var txtUMPDLimit = document.getElementById(txtUMPDLimitId);
        var ZipValue = null;
        var found = false;
        var StatesText = '';

        if (txtZip) { ZipValue = txtZip.value; }

        if (ZipValue && StatesOnQuote) {
            if (ZipValue.length >= 5) {
                // Split the list of states
                var states = StatesOnQuote.split(",");

                switch (states.length) {
                    case 1:
                        StatesText = states[0];
                        break;
                    case 2:
                        StatesText = states[0] + " or " + states[1];
                        break;
                    case 3:
                        StatesText = states[0] + " or " + states[1] + " or " + states[2];
                        break;
                    default:
                        StatesText = StatesOnQuote;
                        break;
                }

                ifm.vr.vrdata.ZipCode.GetZipCodeInformation(ZipValue, function (data) {
                    if (data) {
                        if (data.length > 0) {
                            // Loop through all of the states on the quote and see if the zip code is within those states
                            var zipstate = data[0].StateAbbrev.toUpperCase();
                            for (var i = 0; i < states.length; i++) {
                                var qstate = states[i];
                                if (qstate.toUpperCase() == zipstate.toUpperCase()) {
                                    found = true;
                                    break;
                                }
                            }

                            // Zip code is not in the states on the quote
                            if (!found) {
                                alert('Invalid ZIP, location must be in ' + StatesText);
                                return data;
                            }

                            var city = data[0].City.toUpperCase();
                            if (txtCity) { txtCity.value = city; }

                            txtState.value = zipstate;

                            var county = data[0].County.toUpperCase();
                            txtCounty.value = county;

                            // If state is Illinois or Ohio, show the UMPD fields
                            if (trUMPD && trUMPDLimit && chkUMPD) {
                                trUMPD.style.display = 'none';
                                trUMPDLimit.style.display = 'none';
                                if (isUMPDChangeAvailable.toUpperCase() == 'TRUE') {
                                    trUMPD.style.display = '';
                                    switch (quoteState) {
                                        case 'Indiana':
                                            if (txtState.value == 'IN') {
                                                chkUMPD.checked = true;
                                            } else {
                                                chkUMPD.checked = false;
                                            }
                                            $("#" + chkUMPDId).attr('disabled', 'disabled');
                                            break;
                                        case 'Illinois':
                                            if (txtState.value == 'IL') {
                                                if (chkUMPD.checked) {                                                
                                                    trUMPDLimit.style.display = '';
                                                    if (ILUMPDLimit) {
                                                        txtUMPDLimit.value = ILUMPDLimit;
                                                    }
                                                }
                                                $("#" + chkUMPDId).attr('disabled', false);
                                            } else {
                                                chkUMPD.checked = false;
                                                $("#" + chkUMPDId).attr('disabled', 'disabled');
                                                }
                                            break;
                                        case 'Ohio':
                                            if (txtState.value == 'OH') {
                                                if (chkUMPD.checked) {
                                                    trUMPDLimit.style.display = '';
                                                    txtUMPDLimit.value = '7,500';
                                                } 
                                                $("#" + chkUMPDId).attr('disabled', false);
                                            } else {
                                                chkUMPD.checked = false;
                                                $("#" + chkUMPDId).attr('disabled', 'disabled');
                                                }
                                            break;
                                        default:
                                            
                                            break;
                                    }
                                } else {
                                    if (txtState.value == 'IL' || txtState.value == 'OH') {
                                        trUMPD.style.display = '';
                                        if (chkUMPD.checked) {
                                            trUMPDLimit.style.display = '';
                                        }
                                        if (txtState.value == 'IL') {
                                            // Illinois Limit
                                            txtUMPDLimit.value = '15,000';
                                        }
                                        else {
                                            // Ohio Limit
                                            txtUMPDLimit.value = '7,500';
                                        }
                                    }
                                    else {
                                        // State is not Illinois or Ohio so uncheck the check box
                                        if (chkUMPD) { chkUMPD.checked = false }
                                    }
                                }
                            }
                        }
                        else {
                            alert('Invalid ZIP, location must be in ' + StatesText);
                            if (txtCity) { txtCity.value = ''; }
                            if (txtState) { txtState.value = ''; }
                            if (txtCounty) { txtCounty.value = ''; }
                        }
                        return data;
                    }
                });
            }
        }

    }

    this.DoCityCountyLookup = function (txtZipId, txtCityId, txtStateId, txtCountyId) {
        var txtZip = document.getElementById(txtZipId);
        var txtCity = document.getElementById(txtCityId);
        var txtState = document.getElementById(txtStateId);
        var txtCounty = document.getElementById(txtCountyId);
        var ZipValue = null;

        if (txtZip) { ZipValue = txtZip.value; }

        if (ZipValue) {
            if (ZipValue.length >= 5) {
                ifm.vr.vrdata.ZipCode.GetZipCodeInformation(ZipValue, function (data) {
                    if (data) {
                        if (data.length > 0) {
                            var state = data[0].StateAbbrev.toUpperCase();

                            // Location MUST be in Indiana
                            if (state != 'IN') {
                                alert('Invalid ZIP, location must be in IN');
                                return data;
                            }

                            var city = data[0].City.toUpperCase();
                            if (txtCity) { txtCity.value = city; }

                            txtState.value = state;

                            var county = data[0].County.toUpperCase();
                            txtCounty.value = county;
                        }
                        else {
                            alert('Invalid ZIP, location must be in IN');
                            if (txtCity) { txtCity.value = ''; }
                            if (txtState) { txtState.value = ''; }
                            if (txtCounty) { txtCounty.value = ''; }
                        }
                        return data;
                    }
                });
            }
        }
    }

    this.ShowUMPDMultistateRouteToUWMessage = function () {
        alert('Multistate risks with UMPD coverage will be routed to Underwriting for review.');
    }

    //used for policy level coverages page, vehicle button - Ohio Only
    this.VehicleButtonClicked = function (isUMPDChangesAvailable, isMultistateQuote) {
        if (isUMPDChangesAvailable.toUpperCase() == "TRUE" && isMultistateQuote.toUpperCase() == "TRUE") {
            this.ShowUMPDMultistateRouteToUWMessage();
        }
    }

    //used for policy level UMPD Limit drop down - Illinois Only
    this.PolicyLevelUMPDLimitChanged = function (ddUMPDLimit) {
        var UMPDLimit = document.getElementById(ddUMPDLimit);
        if (UMPDLimit) {
            var UMPDLimitVal = UMPDLimit.value;
            if (UMPDLimitVal != null && UMPDLimitVal != '' && UMPDLimitVal != '0') {
                this.ShowUMPDMultistateRouteToUWMessage();
            }
        }
    }

    //used for policy level coverages UMPD checkbox - Indiana Only
    this.PolicyLevelUMPDCheckboxChanged = function (chkId, divUMPDdedOptionsId, isMultistateQuote) {
        var chk = document.getElementById(chkId);
        var divUMPDdedOpt = document.getElementById(divUMPDdedOptionsId);

        if (chk) {
            if (chk.checked) {
                if (divUMPDdedOpt) { divUMPDdedOpt.style.display = 'inline' }
                if (isMultistateQuote.toUpperCase() == 'TRUE') { this.ShowUMPDMultistateRouteToUWMessage(); }     
            } else {
                if (divUMPDdedOpt) { divUMPDdedOpt.style.display = 'none' }
            }
        }
    }

    //used for vehicle level UMPD checkbox
    this.UMPDCheckboxChanged = function (chkId, UMPDLimitRowId) {
        var chk = document.getElementById(chkId);
        var UMPDLimitRow = document.getElementById(UMPDLimitRowId);

        if (chk) {
            if (chk.checked) {
                if (UMPDLimitRow) { UMPDLimitRow.style.display = '' }
            }
            else {
                if (UMPDLimitRow) { UMPDLimitRow.style.display = 'none' }
            }
        }
        return true;
    };

    this.UMPDDropDownChanged = function (ddId, txtUMPDHiddenValueId) {
        var dd = document.getElementById(ddId);
        var txtUMPDHidden = document.getElementById(txtUMPDHiddenValueId);
        if (dd && txtUMPDHidden) {
            if (dd.value == "" || dd.value == "0") {
                // set to blank - make sure they really want to delete the coverage
                if (confirm('Are you sure you want to delete this coverage?')) {
                    //set hidden value since they switched it
                    txtUMPDHidden.value = dd.options[dd.selectedIndex].text;
                }
                else {
                    //Cancel button, retain original drop down value (not blank)
                    for (i = 0; i < dd.options.length; i++) {
                        if (dd.options[i].text == txtUMPDHidden.value) {
                            dd.selectedIndex = i;
                        }
                    }
                    return false;
                }
            } else {
                //set hidden value since they switched it
                txtUMPDHidden.value = dd.options[dd.selectedIndex].text;
            }
        }
    };

    this.UMDropdownValueChanged = function (ddId, txtUMPDId) {
        var dd = document.getElementById(ddId);
        var txtUMPD = document.getElementById(txtUMPDId);

        if (dd && txtUMPD) {
            txtUMPD.value = dd.options[dd.selectedIndex].text;
        }
        return true;
    };


    this.SymbolCheckboxChangedOLD = function (rowName, chkCallerId, chk1Id, chk2Id, chk3Id, chk4Id, chk7Id, chk8Id, chk9Id) {
        var chkCaller = null;
        var chkCallerValue = null;
        var chk1 = null;
        var chk2 = null;
        var chk3 = null;
        var chk4 = null;
        var chk7 = null;
        var chk8 = null;
        var chk9 = null;

        if (rowName && chkCallerId) {
            // Get controls and any needed values
            chkCaller = document.getElementById(chkCallerId);
            if (chkCaller) { chkCallerValue = chkCaller.checked; }
            if (chk1Id) { chk1 = document.getElementById(chk1Id); }
            if (chk2Id) { chk2 = document.getElementById(chk2Id); }
            if (chk3Id) { chk3 = document.getElementById(chk3Id); }
            if (chk4Id) { chk4 = document.getElementById(chk4Id); }
            if (chk7Id) { chk7 = document.getElementById(chk7Id); }
            if (chk8Id) { chk8 = document.getElementById(chk8Id); }
            if (chk9Id) { chk9 = document.getElementById(chk9Id); }

            switch (rowName) {
                case "LIAB":
                    if (chkCaller == chk1) {
                        // Liability 1 changed
                        // When Liability1 is checked, 7, 8, 9 are unchecked
                        if (chk1.checked) {
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            if (chk9) { chk9.checked = false; }
                        }
                    }
                    if (chkCaller == chk2 || chkCaller == chk3 || chkCaller == chk4) {
                        // Liability 2, 3, 4 changed
                        // When checked 7 becomes unchcked
                        if (chk2 && chk2.checked) { chk7.checked = false; }
                        if (chk3 && chk3.checked) { chk7.checked = false; }
                        if (chk4 && chk4.checked) { chk7.checked = false; }
                    }
                    break;
                case "MEDPAY":
                    break;
                case "UMUIM":
                    break;
                case "COMP":
                    break;
                case "COLL":
                    break;
                case "TOW":
                    break;
                default:
                    break;
            }
        }
    };

    this.SymbolCheckboxChanged = function (rowName, chkCallerId, chk1Id, chk2Id, chk3Id, chk4Id, chk7Id, chk8Id, chk9Id, WouldHaveSymbol8, WouldHaveSymbol9) {
        var chkCaller = null;
        var chkCallerValue = null;
        var chk1 = null;
        var chk2 = null;
        var chk3 = null;
        var chk4 = null;
        var chk7 = null;
        var chk8 = null;
        var chk9 = null;

        if (rowName && chkCallerId) {
            // Get controls and any needed values
            chkCaller = document.getElementById(chkCallerId);
            if (chkCaller) { chkCallerValue = chkCaller.checked; }
            if (chk1Id) { chk1 = document.getElementById(chk1Id); }
            if (chk2Id) { chk2 = document.getElementById(chk2Id); }
            if (chk3Id) { chk3 = document.getElementById(chk3Id); }
            if (chk4Id) { chk4 = document.getElementById(chk4Id); }
            if (chk7Id) { chk7 = document.getElementById(chk7Id); }
            if (chk8Id) { chk8 = document.getElementById(chk8Id); }
            if (chk9Id) { chk9 = document.getElementById(chk9Id); }

            switch (rowName) {
                case "LIAB":
                    switch (chkCaller) {
                        case chk1:
                            // Liability 1 changed
                            if (chk1.checked) {
                                if (chk2) { chk2.checked = false; }
                                if (chk3) { chk3.checked = false; }
                                if (chk4) { chk4.checked = false; }
                                if (chk7) { chk7.checked = false; }
                                if (chk8) { chk8.checked = false; }
                                if (chk9) { chk9.checked = false; }
                            }
                            break;
                        case chk2:
                            if (chk2.checked) {
                                if (chk1) { chk1.checked = false; }
                                if (chk3) { chk3.checked = false; }
                                if (chk4) { chk4.checked = false; }
                                if (chk7) { chk7.checked = false; }
                                if (chk8) {
                                    if (WouldHaveSymbol8.toUpperCase() == 'TRUE') { chk8.checked = true; } else { chk8.checked = false; }
                                }
                                if (chk9) {
                                    if (WouldHaveSymbol9.toUpperCase() == 'TRUE') { chk9.checked = true; } else { chk9.checked = false; }
                                }
                            }
                            else {
                                if (chk8) { chk8.checked = false; }
                                if (chk9) { chk9.checked = false; }
                            }
                            break;
                        case chk3:
                            if (chk3.checked) {
                                if (chk1) { chk1.checked = false; }
                                if (chk2) { chk2.checked = false; }
                                if (chk4) { chk4.checked = false; }
                                if (chk7) { chk7.checked = false; }
                                if (chk8) {
                                    if (WouldHaveSymbol8.toUpperCase() == 'TRUE') { chk8.checked = true; } else { chk8.checked = false; }
                                }
                                if (chk9) {
                                    if (WouldHaveSymbol9.toUpperCase() == 'TRUE') { chk9.checked = true; } else { chk9.checked = false; }
                                }
                            }
                            else {
                                if (chk8) { chk8.checked = false; }
                                if (chk9) { chk9.checked = false; }
                            }
                            break;
                        case chk4:
                            if (chk4.checked) {
                                if (chk1) { chk1.checked = false; }
                                if (chk2) { chk2.checked = false; }
                                if (chk3) { chk3.checked = false; }
                                if (chk7) { chk7.checked = false; }
                                if (chk8) {
                                    if (WouldHaveSymbol8.toUpperCase() == 'TRUE') { chk8.checked = true; } else { chk8.checked = false; }
                                }
                                if (chk9) {
                                    if (WouldHaveSymbol9.toUpperCase() == 'TRUE') { chk9.checked = true; } else { chk9.checked = false; }
                                }
                            }
                            else {
                                if (chk8) { chk8.checked = false; }
                                if (chk9) { chk9.checked = false; }
                            }
                            break;
                        case chk7:
                            if (chk7.checked) {
                                if (chk1) { chk1.checked = false; }
                                if (chk2) { chk2.checked = false; }
                                if (chk3) { chk3.checked = false; }
                                if (chk4) { chk4.checked = false; }
                                if (chk8) {
                                    if (WouldHaveSymbol8.toUpperCase() == 'TRUE') { chk8.checked = true; } else { chk8.checked = false; }
                                }
                                if (chk9) {
                                    if (WouldHaveSymbol9.toUpperCase() == 'TRUE') { chk9.checked = true; } else { chk9.checked = false; }
                                }
                            }
                            else {
                                if (chk8) { chk8.checked = false; }
                                if (chk9) { chk9.checked = false; }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "MEDPAY":
                    switch (chkCaller) {
                        case chk2:
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        case chk3:
                            if (chk2) { chk2.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        case chk4:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        case chk7:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            break;
                        default:
                            break;
                    }
                    break;
                case "UMUIM":
                case "UM":
                case "UIM":
                    switch (chkCaller) {
                        case chk2:
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        case chk3:
                            if (chk2) { chk2.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        case chk4:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        case chk7:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            break;
                        default:
                            break;
                    }
                    break;
                case "COMP":
                    switch (chkCaller) {
                        case chk2:
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk3:
                            if (chk2) { chk2.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk4:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk7:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk8:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        default:
                            break;
                    }
                    break;
                case "COLL":
                    switch (chkCaller) {
                        case chk2:
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk3:
                            if (chk2) { chk2.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk4:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk7:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk8) { chk8.checked = false; }
                            break;
                        case chk8:
                            if (chk2) { chk2.checked = false; }
                            if (chk3) { chk3.checked = false; }
                            if (chk4) { chk4.checked = false; }
                            if (chk7) { chk7.checked = false; }
                            break;
                        default:
                            break;
                    }
                    break;
                case "TOW":
                    break;
                default:
                    break;
            }
        }
    };

    this.UsePrimaryGaragingAddressCheckboxChanged = function (chkId, ZipId, CityId, CountyId) {
        var chk = document.getElementById(chkId);
        var Zip = document.getElementById(ZipId);
        var City = document.getElementById(CityId);
        var County = document.getElementById(CountyId);

        if (chk) {
            if (!chk.checked) {
                if (Zip) { Zip.value = ''; }
                if (City) { City.value = ''; }
                if (County) { County.value = ''; }
            }
        }
        return true;
    };

    this.checkValidDDLListItem = function (data, ddlElement) {
        //if (data && ddlElement && data != '0') {
        if (data && ddlElement && data != '0' && ddlElement.querySelector('[value="' + data + '"]')) {
            return true;
        }
        else { return false; }
    }

    //Added 06/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    var isSearchingVin = false;
    var lastVinSearchResults = null;
    this.SetupVinSearch = function (vinClientId, makeClientID, modelClientId, yearClientId, senderID, divVinLookupID, divVinLookupContent, ddUseCodeId, ddSizeId, hdnSizeId, ddRadiusId, ddSecClassId, ddSecClassTypeId, txtCostNewId, policyId, policyImageNum, vehicleNum, versionId, divVinLookupValidationClientId, hdnValidVinId, lblClassCodeId, txtClassCodeId, ddVehicleRatingTypeId, hdnOriginalCostNewId, chkCustomPaintJobOrWrapComprehensiveId, chkCustomPaintJobOrWrapCollisionId, racaSymbolsAvailable, hdnOtherThanCollisionSymbolId, hdnCollisionSymbolId, hdnLiabilitySymbolId, hdnOtherThanCollisionOverrideId, hdnCollisionOverrideId, hdnLiabilityOverrideId) {
        if (isSearchingVin == false) {
            var vin = $("#" + vinClientId).val();
            var make = $("#" + makeClientID).val();
            var model = $("#" + modelClientId).val();
            var year = $("#" + yearClientId).val();

            // or make sure make, model and year are not empty
            if (vin.length >= 0 || (make.length > 0 && model.length > 0 && year.length == 4 && senderID != vinClientId)) {

                isSearchingVin = true;
                // agencyID will still be checked against session data to confirm that the current has access to the agency
                VRData.VINCAP.GetFromVinOrMakeModelYear(vin, make, model, year, policyId, policyImageNum, vehicleNum, versionId, function (data) {
                    lastVinSearchResults = data;
                    isSearchingVin = false;

                    if (senderID == vinClientId || senderID == makeClientID || senderID == modelClientId || senderID == yearClientId) {
                        if (data != null && data.length > 0) {
                            var lookupHTML = "";
                            lookupHTML += "<table style='border-collapse: collapse;'>";
                            lookupHTML += "<tr>";
                            lookupHTML += "<th>";
                            lookupHTML += "";
                            lookupHTML += "</th>";

                            lookupHTML += "<th style='text-align: right;margin-right: 10px;'>";
                            lookupHTML += "VIN";
                            lookupHTML += "</th>";

                            lookupHTML += "<th style='text-align: center;margin-right: 10px;'>";
                            lookupHTML += "Year";
                            lookupHTML += "</th>";

                            lookupHTML += "<th style='text-align: right;margin-right: 10px;'>";
                            lookupHTML += "Make/Model";
                            lookupHTML += "</th>";

                            lookupHTML += "<th style='text-align: left;margin-right: 10px;padding-left: 10px;'>";
                            lookupHTML += "Size";
                            lookupHTML += "</th>";

                            lookupHTML += "<th style='text-align: right;'>";
                            lookupHTML += "Cost New";
                            lookupHTML += "</th>";
                            lookupHTML += "</tr>";
                            for (var i = 0; i < data.length; i++) {
                                var onClickCode = "Cap.SetVinData(" + i.toString() + ", \"" + vinClientId.toString() + "\", \"" + makeClientID.toString() + "\", \"" + modelClientId.toString() + "\", \"" + yearClientId.toString() + "\", \"" + senderID.toString() + "\", \"" + divVinLookupID.toString() + "\", \"" + divVinLookupContent.toString() + "\", \"" + ddUseCodeId.toString() + "\", \"" + ddSizeId.toString() + "\", \"" + hdnSizeId.toString() + "\", \"" + ddRadiusId.toString() + "\", \"" + ddSecClassId.toString() + "\", \"" + ddSecClassTypeId.toString() + "\", \"" + txtCostNewId.toString() + "\", \"" + lblClassCodeId.toString() + "\", \"" + txtClassCodeId.toString() + "\", \"" + hdnValidVinId.toString() + "\", \"" + ddVehicleRatingTypeId.toString() + "\", \"" + hdnOriginalCostNewId.toString() + "\", \"" + chkCustomPaintJobOrWrapComprehensiveId.toString() + "\", \"" + chkCustomPaintJobOrWrapCollisionId.toString() + "\", \"" + racaSymbolsAvailable.toString() + "\",\"" + hdnOtherThanCollisionSymbolId.toString() + "\",\"" + hdnCollisionSymbolId.toString() + "\",\"" + hdnLiabilitySymbolId.toString() + "\",\"" + hdnOtherThanCollisionOverrideId + "\",\"" + hdnCollisionOverrideId + "\",\"" + hdnLiabilityOverrideId + "\"); $(\"#" + senderID + "\").parent().find(\"input\").first().focus();";
                                if ((i % 2) == 0) {
                                    lookupHTML += "<tr onmouseover='$(this).css(\"font-weight\",\"bold\")' onmouseout='$(this).css(\"font-weight\",\"normal\")' style='border-bottom: solid 1px black;background-color: white;cursor: pointer; font-size: 11px; ' class='clickable' onclick='" + onClickCode + "' title='Click to apply this result.'>"
                                }
                                else {
                                    lookupHTML += "<tr onmouseover='$(this).css(\"font-weight\",\"bold\")' onmouseout='$(this).css(\"font-weight\",\"normal\")' style='border-bottom: solid 1px black;cursor: pointer; font-size: 11px;' class='clickable' onclick='" + onClickCode + "' title='Click to apply this result.'>"
                                }

                                lookupHTML += "<td style='width: 70px; '>"
                                lookupHTML += "<span style='margin-left: 10px;'><input id='btnVin" + i.toString() + "' type='button' class='StandardButton' value='Select' /></span>"
                                lookupHTML += "</td>"

                                lookupHTML += "<td style='width: 100px;text-align: right;margin-right: 10px;'>"
                                lookupHTML += data[i].Vin.toString();
                                lookupHTML += "</td>"

                                lookupHTML += "<td style='width: 50px;text-align: center;margin-right: 10px;'>"
                                lookupHTML += data[i].Year.toString();
                                lookupHTML += "</td>"

                                lookupHTML += "<td style='width: 100px;text-align: right;margin-right: 10px;'>"
                                lookupHTML += data[i].Make.toUpperCase().toString() + "<br />"; //+ "\n"
                                lookupHTML += data[i].Model.toUpperCase().toString();
                                lookupHTML += "</td>"

                                lookupHTML += "<td style='width: 200px;text-align: left;margin-left: 10px;margin-right: 10px;padding-left: 10px;'>"
                                lookupHTML += data[i].Size.toString();
                                lookupHTML += "</td>"

                                lookupHTML += "<td style='width: 100px;text-align: right;'>"
                                lookupHTML += ifm.vr.stringFormating.asCurrency(data[i].CostNew.toString());
                                lookupHTML += "</td>"

                                lookupHTML += "</tr>"
                            }
                            lookupHTML += "</table>";
                            $("#" + divVinLookupContent).css("color", "black")
                            $("#" + divVinLookupContent).html(lookupHTML);
                            $("#" + divVinLookupID).show();
                            $("#" + senderID).parent().find("input").first().focus(); // put focus on button
                        }
                        else {
                            // no data
                            $("#" + divVinLookupContent).html("No results from the lookup available. Check VIN or Year/Make/Model or enter a <span style='cursor: pointer;text-decoration:underline;' onclick='ifm.vr.ui.FlashFocusThenScrollToElement(\"" + txtCostNewId + "\");'>cost new</span>.");
                            //Updated 11/29/2021 for bug 66920 MLW
                            var classCode = $("#" + txtClassCodeId).val();
                            if (classCode != null && classCode.length > 4) {
                                if (classCode.charAt(0) == '6') {
                                    $("#" + hdnValidVinId).val("True");
                                } else {
                                    $("#" + divVinLookupID).show();
                                    $("#" + hdnValidVinId).val("False");
                                }
                            } else {
                                $("#" + divVinLookupID).show();
                                if (!isNaN(year) && year != 0 && year < 1981) {
                                    $("#" + hdnValidVinId).val("True")
                                } else {
                                    $("#" + hdnValidVinId).val("False");
                                }

                            }
                            //$("#" + divVinLookupID).show();
                            //$("#" + hdnValidVinId).val("False");

                            if (ifm.vr.currentQuote.isEndorsement == true) {
                                ShowVINValidationPopup(divVinLookupValidationClientId);
                            }
                            ToggleValidVIN(false);

                            //set RACA symbols, send EX when no select button present to due the RACA symbol lookup (no data found from Vin lookup)
                            if (racaSymbolsAvailable.toUpperCase() == 'TRUE') {
                                var notFoundSymbolCode = "EX";
                                $("#" + hdnOtherThanCollisionSymbolId).val(notFoundSymbolCode);
                                $("#" + hdnCollisionSymbolId).val(notFoundSymbolCode);
                                $("#" + hdnLiabilitySymbolId).val(notFoundSymbolCode);
                                $("#" + hdnOtherThanCollisionOverrideId).val(notFoundSymbolCode);
                                $("#" + hdnCollisionOverrideId).val(notFoundSymbolCode);
                                $("#" + hdnLiabilityOverrideId).val(notFoundSymbolCode);
                            }

                            $("#" + txtCostNewId).parent().show('fast');
                            $("#" + txtCostNewId).focus();
                        }
                    }
                    return data;
                });
            }
        }
    }

    //Added 06/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    this.SetVinData = function (dataindex, vinClientId, makeClientID, modelClientId, yearClientId, senderID, divVinLookupID, divVinLookupContent, ddUseCodeId, ddSizeId, hdnSizeId, ddRadiusId, ddSecClassId, ddSecClassTypeId, txtCostNewId, lblClassCodeId, txtClassCodeId, hdnValidVinId, ddVehicleRatingTypeId, hdnOriginalCostNewId, chkCustomPaintJobOrWrapComprehensiveId, chkCustomPaintJobOrWrapCollisionId, racaSymbolsAvailable, hdnOtherThanCollisionSymbolId, hdnCollisionSymbolId, hdnLiabilitySymbolId, hdnOtherThanCollisionOverrideId, hdnCollisionOverrideId, hdnLiabilityOverrideId) {
        var data = lastVinSearchResults;
        var origSize = $("#" + ddSizeId).val();
        var origVRT = $("#" + ddVehicleRatingTypeId).val();
        var notFoundSymbolCode = "EX";

        $("#" + divVinLookupContent).html("");
        $("#" + divVinLookupID).hide();

        if (data.length > 0) {
            if ($("#" + vinClientId).val() == '' && senderID != vinClientId) // don't want to overwrite vin because of latency
            { $("#" + vinClientId).val(data[dataindex].Vin); }

            var vin = $("#" + vinClientId).val();
            GetRACASymbols(vin, racaSymbolsAvailable, hdnOtherThanCollisionSymbolId, hdnCollisionSymbolId, hdnLiabilitySymbolId, hdnOtherThanCollisionOverrideId, hdnCollisionOverrideId, hdnLiabilityOverrideId, notFoundSymbolCode)
            
            $("#" + hdnValidVinId).val("True");
            var origSize = $("#" + ddSizeId).val();

            $("#" + yearClientId).val(data[dataindex].Year);
            $("#" + makeClientID).val(data[dataindex].Make.toUpperCase());
            $("#" + modelClientId).val(data[dataindex].Model.toUpperCase());
            var cn = data[dataindex].CostNew;
            //if cost new is not returned in lookup, allow the cost new to be enabled so they can add it.
            if (cn != null && cn != "0" && cn != "") {
                $("#" + txtCostNewId).val(cn);
                this.SetTxtCostNew(txtCostNewId);
                $("#" + hdnOriginalCostNewId).val(cn);
                $("#" + txtCostNewId).attr('disabled', 'disabled');
            } else {
                $("#" + txtCostNewId).attr('disabled', false);
            }

            //enable all options first so newVRT is not null when it is attempting to select an option that was previously disabled
            this.EnableVehicleRatingTypeOption(ddVehicleRatingTypeId);

            if (data[dataindex].RatingType != "") {
                $("#" + ddVehicleRatingTypeId).val(data[dataindex].RatingType);
            }
            var newVRT = $("#" + ddVehicleRatingTypeId).val();
            if (newVRT == "1") {
                //1=Private Passenger Type - when PPT selected, TTT option disabled
                this.DisableVehicleRatingTypeOption(ddVehicleRatingTypeId, '9');
            } else if (newVRT == "9") {
                //9 =Truck, Trailer, Tractor - when TTT selected, PPT option disabled
                this.DisableVehicleRatingTypeOption(ddVehicleRatingTypeId, '1');
            }
            var performChangeEvent = true;
            if (origVRT != newVRT) {
                //do a vehicle rating type onchange to call LookupVehicleClassCode (updates the class code, if necessary)
                $("#" + ddVehicleRatingTypeId).change();
                performChangeEvent = false; //This is so we do not do another lookup later, doing lookup in the change() above
            }

            if (data[dataindex].Size != "") {
                $("#" + ddSizeId + " option:contains('" + data[dataindex].Size + "')").prop('selected', true);

            } else {
                $("#" + ddSizeId).val("");
            }
            //save size value in a hidden field for use when the size drop down is not yet visible on the page or when VRT or Use Code drop downs are switched
            $("#" + hdnSizeId).val($("#" + ddSizeId).val());
            var newSize = $("#" + ddSizeId).val();
            //do a size onchange to call LookupVehicleClassCode (updates the class code, if necessary) & do not let the user change the size drop down
            if (newSize == "") { //for PPT
                $("#" + ddSizeId).val("");
                $("#" + ddRadiusId).val("");
                $("#" + ddSecClassId).val("");
                $("#" + ddSecClassTypeId).val("");
                $("#" + lblClassCodeId).text('');
                $("#" + txtClassCodeId).val('');
                if (performChangeEvent == true) {
                    $("#" + ddUseCodeId).change();
                }
            } else if (origSize != newSize && $("#" + ddUseCodeId).val() != "") {
                $("#" + ddRadiusId).val("");
                $("#" + ddSecClassId).val("");
                $("#" + ddSecClassTypeId).val("");
                if (performChangeEvent == true) {
                    $("#" + ddSizeId).change();
                }
                $("#" + ddSizeId).attr('disabled', 'disabled');
                $("#" + lblClassCodeId).text('');
                $("#" + txtClassCodeId).val('');
            }
            document.getElementById(chkCustomPaintJobOrWrapCollisionId).checked = false;
            document.getElementById(chkCustomPaintJobOrWrapComprehensiveId).checked = false;
        } else {
            $("#" + txtCostNewId).parent().show('fast');
            if (racaSymbolsAvailable.toUpperCase() == 'TRUE') {
                $("#" + hdnOtherThanCollisionSymbolId).val(notFoundSymbolCode);
                $("#" + hdnCollisionSymbolId).val(notFoundSymbolCode);
                $("#" + hdnLiabilitySymbolId).val(notFoundSymbolCode);
                $("#" + hdnOtherThanCollisionOverrideId).val(notFoundSymbolCode);
                $("#" + hdnCollisionOverrideId).val(notFoundSymbolCode);
                $("#" + hdnLiabilityOverrideId).val(notFoundSymbolCode);
            }
        }
        ToggleValidVIN(false);
    }

    function GetRACASymbols(vin, racaSymbolsAvailable, hdnOtherThanCollisionSymbolId, hdnCollisionSymbolId, hdnLiabilitySymbolId, hdnOtherThanCollisionOverrideId, hdnCollisionOverrideId, hdnLiabilityOverrideId, notFoundSymbolCode) {
        if (racaSymbolsAvailable.toUpperCase() == 'TRUE') {
            if (vin && vin.length == 17) {
                VRData.VINCAP.GetRACASymbolsFromVIN(vin, function (symbolData) {
                    if (symbolData != null && symbolData.length > 0) {
                        $("#" + hdnOtherThanCollisionSymbolId).val(symbolData[0].ComprehensiveSymbol.toUpperCase());
                        $("#" + hdnCollisionSymbolId).val(symbolData[0].CollisionSymbol.toUpperCase());
                        $("#" + hdnLiabilitySymbolId).val(symbolData[0].LiabilitySymbol.toUpperCase());
                        $("#" + hdnOtherThanCollisionOverrideId).val(symbolData[0].ComprehensiveSymbol.toUpperCase());
                        $("#" + hdnCollisionOverrideId).val(symbolData[0].CollisionSymbol.toUpperCase());
                        $("#" + hdnLiabilityOverrideId).val(symbolData[0].LiabilitySymbol.toUpperCase());
                    } else {
                        //no response, send EX
                        $("#" + hdnOtherThanCollisionSymbolId).val(notFoundSymbolCode);
                        $("#" + hdnCollisionSymbolId).val(notFoundSymbolCode);
                        $("#" + hdnLiabilitySymbolId).val(notFoundSymbolCode);
                        $("#" + hdnOtherThanCollisionOverrideId).val(notFoundSymbolCode);
                        $("#" + hdnCollisionOverrideId).val(notFoundSymbolCode);
                        $("#" + hdnLiabilityOverrideId).val(notFoundSymbolCode);
                    }
                });
            } else {
                //no vin and partial vin do not return results with the RACA endpoint, send EX
                $("#" + hdnOtherThanCollisionSymbolId).val(notFoundSymbolCode);
                $("#" + hdnCollisionSymbolId).val(notFoundSymbolCode);
                $("#" + hdnLiabilitySymbolId).val(notFoundSymbolCode);
                $("#" + hdnOtherThanCollisionOverrideId).val(notFoundSymbolCode);
                $("#" + hdnCollisionOverrideId).val(notFoundSymbolCode);
                $("#" + hdnLiabilityOverrideId).val(notFoundSymbolCode);
            }
        }
    }

    //Added 07/22/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    function ShowVINValidationPopup(divName) {
        $("#" + divName).dialog({
            title: 'VIN Validation Error',
            width: 400,
            draggable: true,
            autoOpen: true,
            modal: true,
            dialogClass: "no-close"
        });
    }

    //Added 07/22/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    this.CloseVINValidationPopup = function (divName) {
        $("#" + divName).dialog('close');
        return true;
    }

    //Added 07/26/2021 for CAP Endorsements Task 53028 MLW
    this.ClearVINLookupFields = function (senderId, vinId, yearId, makeId, modelId, costNewId, sizeId, hdnSizeId, hdnValidVinId, lblClassCodeId, txtClassCodeId, ddVehicleRatingTypeId) {
        //Used for endorsements, not new business - when a user changes the VIN or year/make/model after a VIN lookup
        var yr = parseInt($("#" + yearId).val()); //test this to make sure it doesn't clear the fields - need to make button enabled
        if (isNaN(yr) || yr > 1980) {
            var clearOtherLookupFields = "True";
            switch (senderId) {
                case vinId:
                    $("#" + yearId).val("");
                    $("#" + makeId).val("");
                    $("#" + modelId).val("");
                    break;
                case yearId:
                    $("#" + vinId).val("");
                    break;
                case makeId:
                    $("#" + vinId).val("");
                    break;
                case modelId:
                    if ($("#" + sizeId).val() == "30") {
                        //30 = Trailer Types - come back with blank model in VIN lookup. Model is required, so the user has to enter something. If they change the trailer model, we do not want the rest of the data to clear out.
                        clearOtherLookupFields = "False";
                    } else {
                        $("#" + vinId).val("");
                    }
                    break;
                //user cannot change the size once the lookup happens, so it will not clear out any fields
                //user can change the cost new once the lookup happens if it returns 0 or blank, but changing the cost new should not clear out any fields
                default:
                    break;
            }
            if (clearOtherLookupFields == "True") {
                $("#" + ddVehicleRatingTypeId).val("");
                $("#" + ddVehicleRatingTypeId).change();
                $("#" + costNewId).val("");
                $("#" + sizeId).val("");
                $("#" + hdnSizeId).val(""); //used to set the size drop down, particularly if the size drop down was not yet visible when doing the VIN lookup.
                $("#" + hdnValidVinId).val("False"); //use to determine whether the user did a VIN lookup (required) and had a valid VIN returned. No data returned from the lookup means the VIN was not valid.
                $("#" + lblClassCodeId).text("");
                $("#" + txtClassCodeId).val("");
                //$("#" + costNewId).attr('disabled', false); //not ever enabling for endorsements, make them do another lookup to change these fields.
                $("#" + sizeId).attr('disabled', false);
                this.EnableVehicleRatingTypeOption(ddVehicleRatingTypeId);

            }
        } else if (yr != 0 && yr < 1981) {
            $("#" + hdnValidVinId).val("True");
        }
        ToggleValidVIN(false);
    }

    this.EnableVehicleRatingTypeOption = function (ddVehicleRatingTypeId) {
        //re-enabling 1=PPT (Private Passenger Type) and 9=TTT (Truck, Trailer, Tractor) in the Vehicle Rating Type drop down
        $("#" + ddVehicleRatingTypeId + " option[value = '1']").removeAttr('disabled');
        $("#" + ddVehicleRatingTypeId + " option[value = '9']").removeAttr('disabled');
    }
    this.DisableVehicleRatingTypeOption = function (ddVehicleRatingTypeId, optionToDisable) {
        if (optionToDisable) {
            if (optionToDisable == '1') {
                //Private Passenger Type
                $("#" + ddVehicleRatingTypeId + " option[value = '1']").attr('disabled', 'disabled');
            } else if (optionToDisable == '9') {
                //Truck, Trailer, Tractor
                $("#" + ddVehicleRatingTypeId + " option[value = '9']").attr('disabled', 'disabled');
            }
        }       
    }

    //Added 07/28/2021 for CAP Endorsements Task 53030 (NB) MLW
    this.EnableLookupFields = function (senderId, vinId, sizeId, costNewId, hdnValidVinId, ddVehicleRatingTypeId) {
        //Used for new business, not endorsements - when a user changes the VIN or year/make/model after a VIN lookup
        var vin = $("#" + vinId).val();
        if (senderId == vinId) {
            if (vin == null || vin == "") {
                //Allows the user to rate when no VIN at new business quote side, VIN not require on NB quote side
                $("#" + hdnValidVinId).val('True');
                $("#" + costNewId).attr('disabled', false);
                $("#" + sizeId).attr('disabled', false);
            } else {
                //Updated 11/29/2021 for bug 66920 MLW
                if ($("#" + sizeId).val() == "30") {
                    //If trailer, pass as a valid vin always, do not force use of the lookup
                    $("#" + hdnValidVinId).val('True');
                } else {
                    //If the user changes the VIN, they must do another lookup or clear out the VIN field on new business quote side
                    $("#" + hdnValidVinId).val('False');
                }
                ////If the user changes the VIN, they must do another lookup or clear out the VIN field on new business quote side
                //$("#" + hdnValidVinId).val('False');
            }
        } else {
            $("#" + hdnValidVinId).val('True');
            if ($("#" + sizeId).val() != "30") {
                //30 = Trailer Types - come back with blank model in VIN lookup. Model is required, so the user has to enter something. If they change the trailer model, we do not want enable anything.
                $("#" + costNewId).attr('disabled', false);
                $("#" + sizeId).attr('disabled', false);
            }            
        }
        ToggleValidVIN(false);
    }  

    //Updated 08/23/2021 for Bug 64413 MLW - using vrVehicleNum in ShowPopupVINLookupChangedFields
    //Added 07/23/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    this.AppGapVINLookup = function (VinClientId, SenderID, PolicyId, PolicyImageNum, VehicleNum, VrVehicleNum, VersionId, divVinLookupValidationClientId, hdnVehicleYearId, hdnVehicleMakeId, hdnVehicleModelId, hdnVehicleSizeId, hdnVehicleCostNewId, hdnVehicleClassCodeId, hdnValidVinId, hdnRateTypeId, hdnUseCodeId, hdnOperatorId, hdnOperatorTypeId, hdnTrailerTypeId, hdnRadiusId, hdnSecClassId, hdnSecClassTypeId, racaSymbolsAvailable, hdnOtherThanCollisionSymbolId, hdnCollisionSymbolId, hdnLiabilitySymbolId, hdnOtherThanCollisionOverrideId, hdnCollisionOverrideId, hdnLiabilityOverrideId) {
        if (isSearchingVin == false) {
            var Vin = $("#" + VinClientId).val();
            var origClassCode = $("#" + hdnVehicleClassCodeId).val(); //Added 11/30/2021 for bug 66920 MLW
            var origYear = $("#" + hdnVehicleYearId).val(); //Added 11/30/2021 for bug 66920 MLW
            var origMake = $("#" + hdnVehicleMakeId).val(); //Added 11/30/2021 for bug 66920 MLW
            var notFoundSymbolCode = "EX";
            if (Vin.length >= 0) {
                isSearchingVin = true;
                VRData.VINCAP.GetFromVIN(Vin, PolicyId, PolicyImageNum, VehicleNum, VersionId, function (data) {
                    lastVinSearchResults = data;
                    isSearchingVin = false;
                    if (SenderID == VinClientId) {
                        if (data != null && data.length > 0) {
                            for (var dataindex = 0; dataindex < data.length; dataindex++) {
                                if (dataindex == 0) {
                                    //$("#" + hdnVehicleYearId).val(data[dataindex].Year);
                                    //$("#" + hdnVehicleMakeId).val(data[dataindex].Make.toUpperCase());
                                    //$("#" + hdnVehicleModelId).val(data[dataindex].Model.toUpperCase());
                                    //$("#" + hdnVehicleCostNewId).val(data[dataindex].CostNew);
                                    var sizeId = "";
                                    switch (data[dataindex].Size.toUpperCase()) {
                                        case "LIGHT TRUCK < OR EQUAL 10,000 POUNDS GVW":
                                            sizeId = "18";
                                            break;
                                        case "MEDIUM TRUCK 10,001 TO 20,000 POUNDS GVW":
                                            sizeId = "19";
                                            break;
                                        case "HEAVY TRUCK 20,001 TO 45,000 POUNDS GVW":
                                            sizeId = "20";
                                            break;
                                        case "EXTRA HEAVY TRUCK > 45,000 POUNDS GVW":
                                            sizeId = "21";
                                            break;
                                        case "HEAVY TRUCK-TRACTORS < OR EQUAL 45,000 POUNDS GVW":
                                            sizeId = "22";
                                            break;
                                        case "EXTRA HEAVY TRUCK-TRACTORS > 45,000 POUNDS GVW":
                                            sizeId = "23";
                                            break;
                                        case "TRAILER TYPES":
                                            sizeId = "30";
                                            break;
                                        default:
                                            sizeId = "";
                                            break;
                                    }
                                    //Get correct class code if enough data available, otherwise clear the class code
                                    //Updated 11/30/2021 for bug 66920 MLW - if trailer (i.e. class code begins with 6) only replace the vehicle info if the year and make are different from the originally entered data on quote side.
                                    //if (data[dataindex].Size.toUpperCase() == "TRAILER TYPES") {
                                    if (origClassCode.charAt(0) == '6') {
                                        if (origYear == data[dataindex].Year && origMake.toUpperCase() == data[dataindex].Make.toUpperCase()) {
                                            //Do nothing - retain original information & allow user to rate
                                        } else {
                                            replaceVehicleDataFromVINLookup(data, dataindex, hdnVehicleYearId, hdnVehicleMakeId, hdnVehicleModelId, hdnVehicleCostNewId)
                                            $("#" + hdnVehicleSizeId).val(sizeId);
                                            //Show special message for trailers since their model returns blank and cost new returns 0.
                                            ShowPopupVINLookupChangedFields(VrVehicleNum);
                                            $("#" + hdnVehicleClassCodeId).val('');
                                        }
                                        //$("#" + hdnVehicleSizeId).val(sizeId);
                                        ////Show special message for trailers since their model returns blank and cost new returns 0.
                                        //ShowPopupVINLookupChangedFields(VrVehicleNum);
                                        //$("#" + hdnVehicleClassCodeId).val('');
                                    } else {
                                        replaceVehicleDataFromVINLookup(data, dataindex, hdnVehicleYearId, hdnVehicleMakeId, hdnVehicleModelId, hdnVehicleCostNewId)

                                        if ($("#" + hdnRateTypeId).val() != data[dataindex].RatingType) {
                                            $("#" + hdnRateTypeId).val(data[dataindex].RatingType);
                                            $("#" + hdnVehicleSizeId).val(sizeId);
                                            //if the vehicle rating type changes in the lookup, need to reset the class code because they require different drop down selections to get the correct class code. The class code lookup will return the incorrect class code. User will need to go back to the quote side and select the correct drop downs to get the correct class code.
                                            $("#" + hdnVehicleClassCodeId).val('');
                                            ShowPopupVINLookupChangedFields(VrVehicleNum);
                                        } else {
                                            if (data[dataindex].CostNew == "" || data[dataindex].CostNew == "0") {
                                                $("#" + hdnVehicleSizeId).val(sizeId);
                                                ShowPopupVINLookupChangedFields(VrVehicleNum);
                                            } else if ($("#" + hdnVehicleSizeId).val() != sizeId) {
                                                $("#" + hdnVehicleSizeId).val(sizeId);
                                                ShowPopupVINLookupChangedFields(VrVehicleNum);
                                            }
                                            AppGapClassCode(hdnVehicleYearId, hdnVehicleMakeId, hdnVehicleModelId, hdnRateTypeId, hdnUseCodeId, hdnOperatorId, hdnOperatorTypeId, hdnVehicleSizeId, hdnTrailerTypeId, hdnRadiusId, hdnSecClassId, hdnSecClassTypeId, hdnVehicleClassCodeId);
                                        }
                                    }
                                    $("#" + hdnValidVinId).val("True");
                                    ToggleValidVIN(true);

                                    GetRACASymbols(Vin, racaSymbolsAvailable, hdnOtherThanCollisionSymbolId, hdnCollisionSymbolId, hdnLiabilitySymbolId, hdnOtherThanCollisionOverrideId, hdnCollisionOverrideId, hdnLiabilityOverrideId)
                                }
                            }
                        }
                        else {
                            // no data
                            //Updated 11/30/2021 for bug 66920 MLW
                            var origYearInt = parseInt(origYear);
                            if (origClassCode != null && origClassCode.length > 4) {
                                if (origClassCode.charAt(0) == '6') {
                                    $("#" + hdnValidVinId).val("True");
                                } else {
                                    //Updated 8/18/2022 for task 73960 MLW
                                    if (isNaN(origYearInt) || origYearInt > 1980) {
                                        ShowVINValidationPopup(divVinLookupValidationClientId);
                                        $("#" + VinClientId).focus();
                                        $("#" + hdnValidVinId).val("False");
                                    } else {
                                        $("#" + hdnValidVinId).val("True");
                                    }
                                    //ShowVINValidationPopup(divVinLookupValidationClientId);
                                    //$("#" + VinClientId).focus();
                                    //$("#" + hdnValidVinId).val("False");
                                }
                            } else {
                                //Updated 8/18/2022 for task 73960 MLW
                                if (isNaN(origYearInt) || origYearInt > 1980) {
                                    ShowVINValidationPopup(divVinLookupValidationClientId);
                                    $("#" + VinClientId).focus();
                                    $("#" + hdnValidVinId).val("False");
                                } else {
                                    $("#" + hdnValidVinId).val("True");
                                }
                                //ShowVINValidationPopup(divVinLookupValidationClientId);
                                //$("#" + VinClientId).focus();
                                //$("#" + hdnValidVinId).val("False");
                            }
                            //ShowVINValidationPopup(divVinLookupValidationClientId);
                            //$("#" + VinClientId).focus();
                            //$("#" + hdnValidVinId).val("False");
                            ToggleValidVIN(true);

                            if (racaSymbolsAvailable.toUpperCase() == 'TRUE') {
                                $("#" + hdnOtherThanCollisionSymbolId).val(notFoundSymbolCode);
                                $("#" + hdnCollisionSymbolId).val(notFoundSymbolCode);
                                $("#" + hdnLiabilitySymbolId).val(notFoundSymbolCode);
                                $("#" + hdnOtherThanCollisionOverrideId).val(notFoundSymbolCode);
                                $("#" + hdnCollisionOverrideId).val(notFoundSymbolCode);
                                $("#" + hdnLiabilityOverrideId).val(notFoundSymbolCode);
                            }
                        }
                    }

                    return data;
                });
            } else {
                if (racaSymbolsAvailable.toUpperCase() == 'TRUE') {
                    $("#" + hdnOtherThanCollisionSymbolId).val(notFoundSymbolCode);
                    $("#" + hdnCollisionSymbolId).val(notFoundSymbolCode);
                    $("#" + hdnLiabilitySymbolId).val(notFoundSymbolCode);
                    $("#" + hdnOtherThanCollisionOverrideId).val(notFoundSymbolCode);
                    $("#" + hdnCollisionOverrideId).val(notFoundSymbolCode);
                    $("#" + hdnLiabilityOverrideId).val(notFoundSymbolCode);
                }
            }
        }
    }

    //Added 11/30/2021 for bug 66920 MLW
    function replaceVehicleDataFromVINLookup(data, dataindex, hdnVehicleYearId, hdnVehicleMakeId, hdnVehicleModelId, hdnVehicleCostNewId) {
        $("#" + hdnVehicleYearId).val(data[dataindex].Year);
        $("#" + hdnVehicleMakeId).val(data[dataindex].Make.toUpperCase());
        $("#" + hdnVehicleModelId).val(data[dataindex].Model.toUpperCase());
        $("#" + hdnVehicleCostNewId).val(data[dataindex].CostNew);
    }

    //Added 08/13/2021 for CAP Endorsements Task 53030 MLW
    function ShowPopupVINLookupChangedFields(VehicleNum) {
        alert("Vehicle # " + VehicleNum + " - VIN Lookup has updated your vehicle details. If model, cost new or class code was removed, please reenter.");
    }

    //Added 08/02/2021 for CAP Endorsements Task 53028 MLW
    //Will search all hdnValidVin fields - if find one that is False, then disable the save and rate button, show the "to rate" message, show route to UW button
    function ToggleValidVIN(isOnAppPage) {
        var hasAllValidVins = true;
        //if (isOnAppPage || ifm.vr.currentQuote.isEndorsement == true) { //cannot use this because it will never fire on new business quote side and it needs to
            var vehInfo = '';
            $('[id*=hdnValidVin]').each(function () {
                var validVin = $(this);
                if (validVin.val() == "False") {
                    hasAllValidVins = false;
                    var currVehInfo = '';
                    if (isOnAppPage) {
                        var vehNum = validVin.parent().find('[id*=hdnVehicleNum]').val();
                        var vin = validVin.parent().find('[id*=txtVinNumber]').val();
                        var year = validVin.parent().find('[id*=hdnVehicleYear]').val();
                        var make = validVin.parent().find('[id*=hdnVehicleMake]').val();
                        var model = validVin.parent().find('[id*=hdnVehicleModel]').val();
                        currVehInfo = ' Vehicle ' + vehNum + ' ' + constructCurrVehInfo(vin, year, make, model);
                    } else {
                        var vin = validVin.parent().find('[id*=txtVIN]').val();
                        var year = validVin.parent().find('[id*=txtVehicleYear]').val();
                        var make = validVin.parent().find('[id*=txtVehicleMake]').val();
                        var model = validVin.parent().find('[id*=txtVehicleModel]').val();
                        currVehInfo = constructCurrVehInfo(vin, year, make, model);                       
                    }
                    if (currVehInfo != null && currVehInfo.length > 0) {
                        if (vehInfo.length > 0) {
                            vehInfo = vehInfo + ', ';
                        }
                        vehInfo += currVehInfo;
                    }
                }
            });
            $('[id*="hdnVehicleInfo"]').val(vehInfo); //vehInfo sent in route message when routing to UW.
        //}
        if (hasAllValidVins == true) {
            if (isOnAppPage) {
                $('input[id*="btnShowEffectiveDate"]').css("display", "");
                $('[id*="spanRouteToUWContainer"]').css("display", "none");
            } else {
                $('div[id*="divUseVINLookupMessage"]').css("display", "none");                
                $('input[id*="btnSaveAndRate"]').attr('disabled', false);
                $('[id*="spanRouteToUWContainer"]').css("display", "none");
                if (ifm.vr.currentQuote.isEndorsement == true) {
                    $('input[id*="btnEmailForUWAssistance"]').css("display", "");
                }
            }
        } else {
            if (isOnAppPage) {
                $('input[id*="btnShowEffectiveDate"]').css("display", "none");
                $('[id*="spanRouteToUWContainer"]').css("display", "");
            } else {
                    $('div[id*="divUseVINLookupMessage"]').css("display", "");
                $('input[id*="btnSaveAndRate"]').attr('disabled', 'disabled');
                if (ifm.vr.currentQuote.isEndorsement == true) {
                    $('input[id*="btnEmailForUWAssistance"]').css("display", "none");
                    $('[id*="spanRouteToUWContainer"]').css("display", "");
                }
            }
        }
    }

    //Added 08/09/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    function constructCurrVehInfo(vin, year, make, model) {
        var currVehInfo = '';
        if (vin != "") {
            currVehInfo += vin;
            if (year != "" || make != "" || model != "") {
                currVehInfo += ' ';
            }
        }
        if (year != "") {
            currVehInfo += year;
            if (make != "" || model != "") {
                currVehInfo += ' ';
            }
        }
        if (make != "") {
            currVehInfo += make;
            if (model != "") {
                currVehInfo += ' ';
            }
        }
        if (model != "") {
            currVehInfo += model;
        }
        return currVehInfo;
    }

    //Added 08/02/2021 for CAP Endorsements Task 53028 MLW
    this.ToggleValidVIN = function (isOnAppPage) {
        ToggleValidVIN(isOnAppPage);
    }  

    //Added 08/03/2021 for CAP Endorsements Task 53030 MLW
    //this.AppGapClassCode = function (YearId, MakeId, ModelId, RateTypeId, UseCodeId, OperatorId, OperatorTypeId, SizeId, TrailerTypeId, RadiusId, SecClassId, SecClassTypeId, classCodeId) {
    function AppGapClassCode(YearId, MakeId, ModelId, RateTypeId, UseCodeId, OperatorId, OperatorTypeId, SizeId, TrailerTypeId, RadiusId, SecClassId, SecClassTypeId, ClassCodeId) {
        var YearValue = $("#" + YearId).val();
        var MakeValue = $("#" + MakeId).val();
        var ModelValue = $("#" + ModelId).val();
        var RatingTypeValue = $("#" + RateTypeId).val();
        var UseValue = $("#" + UseCodeId).val();
        var OperatorValue = $("#" + OperatorId).val();
        var OperatorTypeValue = $("#" + OperatorTypeId).val();
        var SizeValue = $("#" + SizeId).val();
        var TrailerTypeValue = $("#" + TrailerTypeId).val();
        var RadiusValue = $("#" + RadiusId).val();
        var SecondaryClassValue = $("#" + SecClassId).val();
        var SecondaryClassTypeValue = $("#" + SecClassTypeId).val();
        //var YearValue = $("#" + YearId).val();
        //var YearValue = $("#" + YearId).val();

        // Set the genhandler command & parms
        var genHandler = 'GenHandlers/Vr_Comm/VehicleClassCodeLookup.ashx'
        if (YearValue) { genHandler += '?yr=' + encodeURIComponent(YearValue.trim()); } else { genHandler += '?yr=' + encodeURIComponent(null); }
        if (MakeValue) { genHandler += "&mk=" + encodeURIComponent(MakeValue.trim()); }
        if (ModelValue) { genHandler += "&md=" + encodeURIComponent(ModelValue.trim()); }
        if (RatingTypeValue) { genHandler += "&rtId=" + encodeURIComponent(RatingTypeValue.trim()); }
        if (UseValue) { genHandler += "&ucId=" + encodeURIComponent(UseValue.trim()); }
        if (OperatorValue) { genHandler += "&opId=" + encodeURIComponent(OperatorValue.trim()); }
        if (OperatorTypeValue) { genHandler += "&optypId=" + encodeURIComponent(OperatorTypeValue.trim()); }
        if (SizeValue) { genHandler += "&szId=" + encodeURIComponent(SizeValue.trim()); }
        if (TrailerTypeValue) { genHandler += "&ttId=" + encodeURIComponent(TrailerTypeValue.trim()); }
        if (RadiusValue) { genHandler += "&rdId=" + encodeURIComponent(RadiusValue.trim()); }
        if (SecondaryClassValue) { genHandler += "&scId=" + encodeURIComponent(SecondaryClassValue.trim()); }
        if (SecondaryClassTypeValue) { genHandler += "&sctId=" + encodeURIComponent(SecondaryClassTypeValue.trim()); }

        // Call the generic handler
        $.getJSON(genHandler, {
            dataType: "json",
            data: "",
            cache: false,
            format: "json"
        })
            .done(function (data) {
                // Use the data returned
                if (data && data.length > 3) {
                    // Set the class code textbox
                    $("#" + ClassCodeId).val(data);
                }
                else {
                    // Clear the class code textbox
                    $("#" + ClassCodeId).val("");
                }
                //clear Error
            }).error(function (err) {
                // Error handler
                alert("Generic Handler encountered an error!");
            });
    }

    //Added 11/29/2021 for bugk 66920 MLW
    //When on the quote side, the class code beings with 6 (i.e. trailer) and a VIN is entered, allow the user to continue to rate with or without using the VIN lookup.
    this.ForceVehicleValidVIN = function (hdnValidVinId, txtClassCodeId) {
        var validVINFlag = $("#" + hdnValidVinId).val();
        var classCode = $("#" + txtClassCodeId).val();
        if (classCode != null && classCode.length > 4) {
            if (classCode.charAt(0) == '6') {
                $("#" + hdnValidVinId).val("True");
                ToggleValidVIN(false);
            }
        }
    }

    this.ComprehensiveOrCollisionChanged = function (collisionChkId, comprehensiveChkId, trCustomPaintJobOrWrapCollisionId, trCustomPaintJobOrWrapComprehensiveId, chkCustomPaintJobOrWrapCollisionId, chkCustomPaintJobOrWrapComprehensiveId, txtCostNewId, hdnOriginalCostNewId) {
        var collisionChk = document.getElementById(collisionChkId);
        var comprehensiveChk = document.getElementById(comprehensiveChkId);
        var trCustomPaintJobOrWrapCollision = document.getElementById(trCustomPaintJobOrWrapCollisionId);
        var trCustomPaintJobOrWrapComprehensive = document.getElementById(trCustomPaintJobOrWrapComprehensiveId);
        var chkCustomPaintJobOrWrapCollision = document.getElementById(chkCustomPaintJobOrWrapCollisionId);
        var chkCustomPaintJobOrWrapComprehensive = document.getElementById(chkCustomPaintJobOrWrapComprehensiveId);
        var txtCostNew = document.getElementById(txtCostNewId);
        var originalCost = document.getElementById(hdnOriginalCostNewId);

        if (collisionChk && comprehensiveChk && trCustomPaintJobOrWrapCollision && trCustomPaintJobOrWrapComprehensive && txtCostNew && originalCost) {
            if ((chkCustomPaintJobOrWrapComprehensive.checked && !comprehensiveChk.checked) || (chkCustomPaintJobOrWrapCollision.checked && !collisionChk.checked)) {
                txtCostNew.value = originalCost.value;
                this.SetTxtCostNew(txtCostNewId);
            }

            if (collisionChk.checked && comprehensiveChk.checked === false) {
                trCustomPaintJobOrWrapCollision.style.display = '';
                trCustomPaintJobOrWrapComprehensive.style.display = 'none';
                chkCustomPaintJobOrWrapComprehensive.checked = false;
            }
            else if (comprehensiveChk.checked && collisionChk.checked === false) {
                trCustomPaintJobOrWrapComprehensive.style.display = '';
                trCustomPaintJobOrWrapCollision.style.display = 'none';
                chkCustomPaintJobOrWrapCollision.checked = false;
            }
            else if (comprehensiveChk.checked === false && collisionChk.checked === false) {
                trCustomPaintJobOrWrapComprehensive.style.display = 'none';
                trCustomPaintJobOrWrapCollision.style.display = 'none';
                chkCustomPaintJobOrWrapComprehensive.checked = false;
                chkCustomPaintJobOrWrapCollision.checked = false;
            }
        }
    }

    this.IncreaseVehicleCostNew = function (chkCustomPaintJobOrWrapCollisionId, chkCustomPaintJobOrWrapComprehensiveId, txtCostNewId, hdnOriginalCostNewId) {
        var chkCustomPaintJobOrWrapCollision = document.getElementById(chkCustomPaintJobOrWrapCollisionId);
        var chkCustomPaintJobOrWrapComprehensive = document.getElementById(chkCustomPaintJobOrWrapComprehensiveId);
        var txtCostNew = document.getElementById(txtCostNewId);
        var originalCost = document.getElementById(hdnOriginalCostNewId).value;
        var increaseAmount = 5000;

        if (chkCustomPaintJobOrWrapCollision && chkCustomPaintJobOrWrapComprehensive && txtCostNew && originalCost && originalCost != '') {
            var costNewAsString = parseFloat(txtCostNew.value.replace(/[^0-9.-]+/g, ''))

            if ((chkCustomPaintJobOrWrapCollision.checked || chkCustomPaintJobOrWrapComprehensive.checked) && costNewAsString == originalCost) {
                txtCostNew.value = parseInt(originalCost) + parseInt(increaseAmount);
                this.SetTxtCostNew(txtCostNewId);
            }
            else if (!chkCustomPaintJobOrWrapCollision.checked && !chkCustomPaintJobOrWrapComprehensive == 0 && costNewAsString != originalCost) {
                txtCostNew.value = originalCost;
                this.SetTxtCostNew(txtCostNewId);
            }
        }
    }

    this.HandleCostNewChange = function (txtCostNewId, hdnOriginalCostNewId) {
        var txtCostNew = document.getElementById(txtCostNewId);
        var hdnOriginalCostNew = document.getElementById(hdnOriginalCostNewId);

        if (txtCostNew && hdnOriginalCostNew) {
            if (txtCostNew.value != hdnOriginalCostNew.value) {
                // If the cost new has changed, update the hidden field
                hdnOriginalCostNew.value = txtCostNew.value;
            }
            this.SetTxtCostNew(txtCostNewId);
        }
    }

    this.SetTxtCostNew = function (txtCostNewId) {
        var txtCostNew = document.getElementById(txtCostNewId);

        if (txtCostNew) {
            txtCostNew.value = ifm.vr.stringFormating.asCurrency(txtCostNew.value);
        }
    }
};

