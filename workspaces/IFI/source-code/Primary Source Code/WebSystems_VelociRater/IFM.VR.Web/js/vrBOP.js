
$(document).ready(function () {

    // Contractors Equipment Amount Remaining
    //$(".txtCTEQLimit").each(function () {
    //    $(this).keyup(function () {
    //        var tot = 0;
    //        var schedtot = 0;
    //        // Get scheduled total
    //        $(".txtCTSched").each(function () {
    //            var sender = this;
    //            var textBoxData = sender.value.split(",").join("").split("$").join("");
    //            if (!isNaN(textBoxData)) {
    //                schedtot = +schedtot + +textBoxData;
    //            }
    //        });
    //        // Get total of all limits on form
    //        $(".txtCTEQLimit").each(function () {
    //            var sender = this;
    //            var textBoxData = sender.value.split(",").join("").split("$").join("");
    //            if (!isNaN(textBoxData)) {
    //                tot = +tot + +textBoxData;
    //            }
    //        });
    //        // Calculate and display the remaining amount
    //        $(".txtCTRemain").each(function () {
    //            var remain = schedtot - tot;
    //            // Scheduled total can't be less that total
    //            // If it is, adjust the scheduled total to match total
    //            if (schedtot < tot) {
    //                $(".txtCTSched").each(function () {
    //                    var sender = this;
    //                    sender.value = "$" + tot;
    //                });
    //                remain = 0;
    //            }
    //            this.value = "$" + remain;
    //        });
    //    });
    //});


    // Photography Scheduled Items Limit Total
    $(".txtPhotogLimit").each(function () {
        $(this).keyup(function () {
            var tot = 0;
            $(".txtPhotogLimit").each(function () {
                var sender = this;
                var textBoxData = sender.value;
                if (!isNaN(textBoxData)) {
                    tot = +tot + +textBoxData;
                }
            });
            $(".lblPhotogTotal").each(function () {
                this.innerHTML = "Total of All Scheduled Limits: " + "$" + tot;
            });
        });
    });

    // PROFESSIONAL LIABILITY HANDLERS
    // Add a section for each professional liability building coverage user field

    // LIQUOR LIABILITY
    // Indiana
    $(".txtLiquorSales_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtLiquorSales_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    $(".txtLiquorSales_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtLiquorSales_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // APARTMENTS
    // Indiana
    // Number of locations
    $(".txtAptNumLocs_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtAptNumLocs_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Limit of Liability
    $(".ddAptLimit_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddAptLimit_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Deductible
    $(".ddAptDed_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddAptDed_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    $(".txtAptNumLocs_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtAptNumLocs_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Limit of Liability
    $(".ddAptLimit_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddAptLimit_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Deductible
    $(".ddAptDed_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddAptDed_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });

    // BARBERS
    // Indiana
    // Full-Time employees
    $(".txtBarbersFullTimeEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBarbersFullTimeEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Part-Time employees
    $(".txtBarbersPartTimeEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBarbersPartTimeEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Full-Time employees
    $(".txtBarbersFullTimeEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBarbersFullTimeEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Part-Time employees
    $(".txtBarbersPartTimeEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBarbersPartTimeEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // BEAUTICIANS
    // Indiana
    // Full-Time employees
    $(".txtBeautyFullTimeEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBeautyFullTimeEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Part-Time employees
    $(".txtBeautyPartTimeEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBeautyPartTimeEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Full-Time employees
    $(".txtBeautyFullTimeEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBeautyFullTimeEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Part-Time employees
    $(".txtBeautyPartTimeEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtBeautyPartTimeEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    //FUNERAL DIRECTORS
    // Indiana
    // Number of Employees
    $(".txtFuneralNumEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtFuneralNumEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Number of Employees
    $(".txtFuneralNumEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtFuneralNumEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    //MOTEL
    // Indiana
    // Limit of Liability
    $(".ddMotelLiabLimit_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddMotelLiabLimit_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Safe Deposit Liability Limit
    $(".ddMotelSafeLimit_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddMotelSafeLimit_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Safe Deposit Deductible
    $(".ddMotelSafeDed_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddMotelSafeDed_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Limit of Liability
    $(".ddMotelLiabLimit_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddMotelLiabLimit_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Safe Deposit Liability Limit
    $(".ddMotelSafeLimit_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddMotelSafeLimit_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Safe Deposit Deductible
    $(".ddMotelSafeDed_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddMotelSafeDed_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });

    //OPTICAL & HEARING AID
    // Indiana
    // Number of Employees
    $(".txtOpticalNumEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtOpticalNumEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Number of Employees
    $(".txtOpticalNumEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtOpticalNumEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    //PHARMACISTS
    // Indiana
    // Receipts
    $(".txtPharmacistsReceipts_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtPharmacistsReceipts_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Receipts
    $(".txtPharmacistsReceipts_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtPharmacistsReceipts_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    //PRINTERS
    // Indiana
    // Number of locations
    $(".txtPrintersNumLocs_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtPrintersNumLocs_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Number of locations
    $(".txtPrintersNumLocs_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtPrintersNumLocs_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    //RESTAURANTS
    // Indiana
    // Limit of Liability
    $(".ddlRestaurantLimitOfLiability_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddlRestaurantLimitOfLiability_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Deductible
    $(".ddlRestaurantDeductible_IN").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddlRestaurantDeductible_IN").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Limit of Liability
    $(".ddlRestaurantLimitOfLiability_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddlRestaurantLimitOfLiability_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    // Deductible
    $(".ddlRestaurantDeductible_IL").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.selectedIndex;
            $(".ddlRestaurantDeductible_IL").each(function () {
                if (this != sender) {
                    this.selectedIndex = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });

    // SELF-STORAGE
    // Indiana
    // Storage Limit
    $(".txtSelfStorageLimit_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSelfStorageLimit_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Storage Limit
    $(".txtSelfStorageLimit_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSelfStorageLimit_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // VETERINARIANS
    // Indiana
    // Number of Employees
    $(".txtVetNumEmp_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtVetNumEmp_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Illinois
    // Number of Employees
    $(".txtVetNumEmp_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtVetNumEmp_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // PHOTOGRAPHY
    // Indiana
    // Scheduled Equipment Limit
    $(".txtPhotogLimit_IN").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtPhotogLimit_IN").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    //Illinois
    $(".txtPhotogLimit_IL").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtPhotogLimit_IL").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

});

var BopBuilding = new function () {

    // For ILLINOIS ONLY
    // If Mine subsidence is NOT required then:
    // - if the mine checkbox is checked show the info row
    // - if the mine checkbox is not checked, hide the info row
    this.MineSubsidenceCheckboxChanged = function (chkId, stateAbbrev, IsEnabled, trInfoRowId) {
        var trInfoRow = document.getElementById(trInfoRowId);
        var chk = document.getElementById(chkId);
        var chkEnabled = false;

        if (chk && stateAbbrev && IsEnabled && trInfoRow) {
            trInfoRow.style.display = 'none';
            if (stateAbbrev == "IL") {
                if (IsEnabled == "TRUE") { chkEnabled = true; }
                if (IsEnabled) {
                    if (chk.checked) {
                        // Enabled & Checked - show the info row
                        trInfoRow.style.display = '';
                        // Ok now we need to check all Mine checkboxes for any checkboxes that are enabled, then disable all but the first one
                        var i = 0;
                        $(".chkMine_IL_Enabled_NotReqd [type='checkbox']").each(function () {
                            i += 1;
                            this.checked = true;   
                            if (i > 1) { this.disabled = true; } else { this.disabled = false;}  // Don't disable the first one
                        });
                        // Show all the 'not required' informational text rows
                        $(".trMineInfo_IL_NotReqd").each(function () {
                            this.style.display = "";
                        });
                    }
                    else {
                        // Not checked - need to uncheck and enable all the Mine checkboxes that are not 
                        $(".chkMine_IL_Enabled_NotReqd [type='checkbox']").each(function () {
                            this.checked = false;
                            this.disabled = false;
                        });
                        // Hide all the 'not required' informational text rows
                        $(".trMineInfo_IL_NotReqd").each(function () {
                            this.style.display = "none";
                        });
                    }
                }
            }
        }
        return true;
    };

    // When the Limitations on Roof Surfacing checkbox is checked, show the Roof Settlement Options row
    this.LimitationsOnRoofSurfacingCheckboxChanged = function (chkId, trDataRowId) {
        var chk = document.getElementById(chkId);
        var trDataRow = document.getElementById(trDataRowId);

        if (chk && trDataRow) {
            if (chk.checked) {
                trDataRow.style.display = '';
            }
            else {
                trDataRow.style.display = 'none';
            }
        }
    };

    this.populateClassifications = function (ddprogramType, ddClassification, txtClassCode) {
        $('#' + ddClassification).empty();
        $('#' + txtClassCode).val('');
        var programType = $('#' + ddprogramType).val();


        ifm.vr.vrdata.BopBuildingClassifications.searchClassifications(programType, function (data) {
            if (data) {


                $("<option />", {
                    val: "",
                    text: ""
                }).appendTo($('#' + ddClassification));

                $(data).each(function () {
                    $("<option />", {
                        val: this.Value,
                        text: this.Key
                    }).appendTo($('#' + ddClassification));
                });

            }
        });
    }; // END populateClassifications

    this.populateClassCode = function (ddClassification, txtClassCode) {
        var selectedClassification = $('#' + ddClassification).val();
        if (selectedClassification.length > 0)
            $('#' + txtClassCode).val(selectedClassification.split('-')[0]);
        else
            $('#' + txtClassCode).val('');

    }; // END populateClassCode


};

var Bop = new function () {

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

    // Rounds the passed number to the next whole number.  If the result is zero returns an empty string
    // Checks the values of the following BOP fields on the ctlProperty_Address control:
    //  - Number of Amusement Areas
    //  - Number of Playgrounds
    //  - Number of Swimming Pools
    // Rounds the number up to the next whole number.
    // If zero is the result clears the text field.
    // Sets the text field if valid number.
    this.CheckBOPPropertyValues = function (txtNumberId) {
        var txtNumber = document.getElementById(txtNumberId);
        if (txtNumber) {
            var val = txtNumber.value;
            if (val == '') {
                txtNumber.value = '';
                return true;
            }
            if (isNaN(val)) {
                // Not a number, clear the textbox
                txtNumber.value = '';
                return true;
            }
            else {
                // Is a number
                var numberDec = parseFloat(val);
                var numberInt = Math.ceil(numberDec);
                if (numberInt <= 0) {
                    // Number is less than or equal to zero, clear the textbox
                    txtNumber.value = '';
                    return true;
                }
                else {
                    // Rounded number is valid, set the textbox to it
                    txtNumber.value = numberInt;
                    return true;
                }
            }
        }

        //if (number == '') { return ''; }  // If empty string passed send empty string back
        //var numberDec = parseFloat($("#" + number).val()).toString();
        //if (isNaN(numberDec)){return '';}
        //var numberInt = Math.ceil(numberDec);
        //if (numberInt == "0") { return ''; } else { return (numberInt);}
    };

    // When the Illinois Contractors Home Repair & Remodeling is checked, show the limit row 
    // If unchecked hide it
    this.ILContractorsHomeRepairAndRemodelingCheckboxChanged = function (chkId, trInfoId){
        var chk = document.getElementById(chkId);
        var trInfo = document.getElementById(trInfoId);

        if (chk && trInfo) {
            if (chk.checked) {
                trInfo.style.display = '';
            }
            else {
                trInfo.style.display = 'none';
            }
        }
    };

    this.SetBOPOrdOrLawFields = function (Demo, Const, Comb) {
        var A = document.getElementById(Demo);
        var B = document.getElementById(Const);
        var C = document.getElementById(Comb);

        if (A.value.length == 0 && B.value.length == 0 && C.value.length == 0) {
            A.disabled = false;
            B.disabled = false;
            C.disabled = false;
            return true;
        }

        if (A.value.length > 0 || B.value.length > 0) {
            A.disabled = false;
            B.disabled = false;
            C.value = '';
            C.disabled = true
            return true;
        }

        if (C.value.length > 0) {
            C.disabled = false;
            A.value = '';
            B.value = '';
            A.disabled = true;
            B.disabled = true;
            return true;
        }
    };

    this.AICheckboxChanged = function (CheckBoxId, InfoRow1Id, CheckBox2Id, cb2InfoRowId, dataRowId, disableCheckboxId, hideinforowId) {
        var cb = document.getElementById(CheckBoxId);
        var inforow = document.getElementById(InfoRow1Id);
        var cb2 = document.getElementById(CheckBox2Id);
        var cb2inforow = document.getElementById(cb2InfoRowId);
        var datarow = document.getElementById(dataRowId);
        var disablecheckbox = document.getElementById(disableCheckboxId);
        var hideinforow = document.getElementById(hideinforowId);

        if (cb.checked == true) {
            if (inforow) { inforow.style.display = ''; }
            if (datarow) { datarow.style.display = ''; }
            if (cb2) {
                cb2.checked = true;
                cb2.disabled = true;
                if (cb2inforow) { cb2inforow.style.display = ''; }
            }
            if (disablecheckbox) {
                disablecheckbox.checked = false;
                disablecheckbox.disabled = true;
            }
            if (hideinforow) {
                hideinforow.style.display = 'none';
            }
        }
        else {
            if (inforow) { inforow.style.display = 'none'; }
            if (datarow) { datarow.style.display = 'none'; }
            if (cb2) {
                cb2.checked = false;
                cb2.disabled = false;
                if (cb2inforow) { cb2inforow.style.display = 'none'; }
            }
            if (disablecheckbox) {
                disablecheckbox.disabled = false;
            }
            if (hideinforow) {
                hideinforow.style.display = 'none';
            }
        }
    };

    // Handles clicks on the Professional Liability checkboxes (BOP buildings)
    this.ProfessionalLiabilityCheckboxChanged = function (sender, BldgNdx, QuoteState) {
        // Get the coverage bindings for the building
        var PLBindings = Bop.BOPPLCoverageBindings[BldgNdx];

        // Create empty variables for each type of data we will be using
        var cb = null;
        var cb2 = null
        var cb3 = null;
        var checkboxrow = null;
        var datarow = null;
        var datarow2 = null;
        var datarow3 = null;
        var txtLimit = null;
        var txtLimit2 = null;
        var txtDed = null;
        var txtFTEmp = null;
        var txtPTEmp = null;
        var txtNumEmps = null;
        var txtNumLocs = null;
        var txtSalesAndReceipts = null;
        var lbl1 = null;
        var lbl2 = null;
        var ChkClientId = null;

        switch (sender) {
            case "APTS": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkApt);
                ChkClientId = PLBindings.chkApt;
                // Coverage specific code goes here
                break;
            }
            case "BARBER": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkBarbers);
                datarow = document.getElementById(PLBindings.trBarbersDataRow);
                ChkClientId = PLBindings.chkBarbers;
                // Coverage specific code goes here
                break;
            }
            case "BEAUTY": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkBeauty);
                datarow = document.getElementById(PLBindings.trBeautyDataRow);
                ChkClientId = PLBindings.chkBeauty;
                // Coverage specific code goes here
                break;
            }
            case "FINE": {
                cb = document.getElementById(PLBindings.chkFineArts);
                ChkClientId = PLBindings.chkFineArts;
                // Coverage specific code goes here
                break;
            }
            case "FUNERAL": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkFuneral);
                datarow = document.getElementById(PLBindings.trFuneralDataRow);
                ChkClientId = PLBindings.chkFuneral;
                // Coverage specific code goes here
                break;
            }
            case "LIQUOR": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkLiquor);
                datarow = document.getElementById(PLBindings.trLiquorDataRow);
                ChkClientId = PLBindings.chkLiquor;

                // Coverage specific code goes here
                // Set the hard limit values based on state
                // lbl1 is the limit
                if (lbl1 && QuoteState) {
                    if (QuoteState == "IN") {
                        lbl1.innerHTML = "$1,000,000";
                    }
                    else {
                        lbl1.innerHTML = "69/69/85";
                    }
                }
                break;
            }
            case "MOTEL": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkMotel);
                datarow = document.getElementById(PLBindings.trMotelDataRow);
                ChkClientId = PLBindings.chkMotel;
                // Coverage specific code goes here
                break;
            }
            case "OPTICAL": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkOptical);
                datarow = document.getElementById(PLBindings.trOpticalDataRow);
                ChkClientId = PLBindings.chkOptical;
                // Coverage specific code goes here
                break;
            }
            case "PHARMACIST": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkPharmacists);
                datarow = document.getElementById(PLBindings.trPharmacistsDataRow);
                ChkClientId = PLBindings.chkPharmacists;
                // Coverage specific code goes here
                break;
            }
            case "PHOTOG": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkPhotog);
                datarow = document.getElementById(PLBindings.trPhotogDataRow);
                //datarow2 = document.getElementById(PLBindings.trP);
                datarow3 = document.getElementById(PLBindings.trPhotogMakeupCheckboxRow);
                ChkClientId = PLBindings.chkPhotog;
                // Coverage specific code goes here
                break;
            }
            case "PHOTOG_SCHED": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkPhotogSchedEquip);
                datarow = document.getElementById(PLBindings.trPhotogSchedEquipDataRow);
                ChkClientId = PLBindings.chkPhotogSchedEquip;
                break;
            }
            case "PHOTOG_MAKEUP": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkPhotogMakeup);
                ChkClientId = PLBindings.chkPhotogMakeup;
                break;
            }
            case "PRINTERS": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkPrinters);
                datarow = document.getElementById(PLBindings.trPrintersDataRow);
                ChkClientId = PLBindings.chkPrinters;
                // Coverage specific code goes here
                break;
            }
            case "RESCLEAN": {
                cb = document.getElementById(PLBindings.chkResCleaning);
                ChkClientId = PLBindings.chkResCleaning;
                // Coverage specific code goes here
                break;
            }
            case "REST": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkRestaurants);
                datarow = document.getElementById(PLBindings.trRestaurantsDataRow);
                ChkClientId = PLBindings.chkRestaurants;
                // Coverage specific code goes here
                break;
            }
            case "SELFSTORAGE": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkSelfStorage);
                datarow = document.getElementById(PLBindings.trSelfStorageDataRow);
                ChkClientId = PLBindings.chkSelfStorage;
                // Coverage specific code goes here
                break;
            }
            case "VET": {
                // Get all of the controls we're going to need
                cb = document.getElementById(PLBindings.chkVet);
                datarow = document.getElementById(PLBindings.trVetDataRow);
                ChkClientId = PLBindings.chkVet;
                // Coverage specific code goes here
                break;
            }
        };

        // This code does the work using the values we set above
        if (cb.checked == true) {
            if (datarow) { datarow.style.display = ''; }
            if (datarow2) { datarow2.style.display = ''; }
            if (datarow3) { datarow3.style.display = ''; }
        } else {
            if (confirm('Are you sure you want to delete this coverage?') == true) {
                if (datarow) {
                    datarow.style.display = 'none';
                    this.ClearCoverageFields(sender)
                }
                if (datarow2) {
                    datarow2.style.display = 'none';
                    this.ClearCoverageFields(sender)
                }
                if (datarow3) {
                    datarow.style.display = 'none';
                    this.ClearCoverageFields(sender)
                }
            } else {
                cb.checked = true;
                return false;
            }
        };
        Bop.FormatProfessionalLiabilityOnBuildingsInSameState(sender, QuoteState, ChkClientId)
        return true;
    };

    // When a professional liability coverage is selected/unselected, all of the same coverages on the other buildings are to be selected/unselected as well.
    // Buildings other than the first will have the coverage checkbox and details disabled, only the first building's coverage will be editable
    this.FormatProfessionalLiabilityOnBuildingsInSameState = function (sender, StateAbbrev, CheckBoxId) {
        var ndx = -1;
        var textValue = null;
        var checkboxValue = null;
        var cb = document.getElementById(CheckBoxId);
        var selValTr = null;
        var selValTr2 = null;
        var selValTr3 = null;
        var selValChk = null;
        var selValChk2 = null;
        var selValChk3 = null;
        var selValTxt = null;
        var selValTxt2 = null;
        var selValDD1 = null;
        var selValDD2 = null;
        var selValDD3 = null;

        if (cb && StateAbbrev) {
            switch (sender) {
                case "APTS": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkApt_IL input[type=checkbox]";
                            selValTr = ".trAptDataRow_IL";
                            selValTxt = ".txtAptNumLocs_IL";
                            selValDD1 = ".ddAptLimit_IL";
                            selValDD2 = "ddAptDed_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkApt_IN input[type=checkbox]";
                            selValTr = ".trAptDataRow_IN";
                            selValTxt = ".txtAptNumLocs_IN";
                            selValDD1 = ".ddAptLimit_IN";
                            selValDD2 = "ddAptDed_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkApt_OH input[type=checkbox]";
                            selValTr = ".trAptDataRow_OH";
                            selValTxt = ".txtAptNumLocs_OH";
                            selValDD1 = ".ddAptLimit_OH";
                            selValDD2 = "ddAptDed_OH";
                            break;
                        }
                    }
                    break;
                }
                case "BARBER": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkBarbers_IL input[type=checkbox]";
                            selValTr = ".trBarbersDataRow_IL";
                            selValTxt = ".txtBarbersFullTimeEmp_IL";
                            selValTxt2 = ".txtBarbersPartTimeEmp_IL"
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkBarbers_IN input[type=checkbox]";
                            selValTr = ".trBarbersDataRow_IN";
                            selValTxt = ".txtBarbersFullTimeEmp_IN";
                            selValTxt2 = ".txtBarbersPartTimeEmp_IN"
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkBarbers_OH input[type=checkbox]";
                            selValTr = ".trBarbersDataRow_OH";
                            selValTxt = ".txtBarbersFullTimeEmp_OH";
                            selValTxt2 = ".txtBarbersPartTimeEmp_OH"
                            break;
                        }
                    }
                    break;
                }
                case "BEAUTY": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkBeauty_IL input[type=checkbox]";
                            selValTr = ".trBeautyDataRow_IL";
                            selValTxt = ".txtBeautyFullTimeEmp_IL";
                            selValTxt2 = ".txtBeautyPartTimeEmp_IL"
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkBeauty_IN input[type=checkbox]";
                            selValTr = ".trBeautyDataRow_IN";
                            selValTxt = ".txtBeautyFullTimeEmp_IN";
                            selValTxt2 = ".txtBeautyPartTimeEmp_IN"
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkBeauty_OH input[type=checkbox]";
                            selValTr = ".trBeautyDataRow_OH";
                            selValTxt = ".txtBeautyFullTimeEmp_OH";
                            selValTxt2 = ".txtBeautyPartTimeEmp_OH"
                            break;
                        }
                    }
                    break;
                }
                case "FINE": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkFineArts_IL input[type=checkbox]";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkFineArts_IN input[type=checkbox]";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkFineArts_OH input[type=checkbox]";
                            break;
                        }
                    }
                    break;
                }
                case "FUNERAL": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkFuneral_IL input[type=checkbox]";
                            selValTr = ".trFuneralDataRow_IL";
                            selValTxt = ".txtFuneralNumEmp_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkFuneral_IN input[type=checkbox]";
                            selValTr = ".trFuneralDataRow_IN";
                            selValTxt = ".txtFuneralNumEmp_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkFuneral_OH input[type=checkbox]";
                            selValTr = ".trFuneralDataRow_OH";
                            selValTxt = ".txtFuneralNumEmp_OH";
                            break;
                        }
                    }
                    break;
                }
                case "LIQUOR": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValTr = ".trLiquor_IL";
                            selValChk = ".chkLiquorSelector_IL input[type=checkbox]";
                            selValTxt = ".txtLiquorSales_IL";
                            break;
                        }
                        case "IN": {
                            selValTr = ".trLiquor_IN";
                            selValChk = ".chkLiquorSelector_IN input[type=checkbox]";
                            selValTxt = ".txtLiquorSales_IN";
                            break;
                        }
                        case "OH": {
                            selValTr = ".trLiquor_OH";
                            selValChk = ".chkLiquorSelector_OH input[type=checkbox]";
                            selValTxt = ".txtLiquorSales_OH";
                            break;
                        }
                    }
                    break;
                }
                case "MOTEL": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValTr = ".trMotelDataRow_IL";
                            selValChk = ".chkMotel_IL input[type=checkbox]";
                            selValDD1 = ".ddMotelLiabLimit_IL"; // Motel Liability Limit
                            selValDD2 = ".ddMotelSafeLimit_IL"; // Safe Limit
                            selValDD3 = ".ddMotelSafeDed_IL";   // Safe Deductible
                            break;
                        }
                        case "IN": {
                            selValTr = ".trMotelDataRow_IN";
                            selValChk = ".chkMotel_IN input[type=checkbox]";
                            selValDD1 = ".ddMotelLiabLimit_IN"; // Motel Liability Limit
                            selValDD2 = ".ddMotelSafeLimit_IN"; // Safe Limit
                            selValDD3 = ".ddMotelSafeDed_IN";   // Safe Deductible
                            break;
                        }
                        case "OH": {
                            selValTr = ".trMotelDataRow_OH";
                            selValChk = ".chkMotel_OH input[type=checkbox]";
                            selValDD1 = ".ddMotelLiabLimit_OH"; // Motel Liability Limit
                            selValDD2 = ".ddMotelSafeLimit_OH"; // Safe Limit
                            selValDD3 = ".ddMotelSafeDed_OH";   // Safe Deductible
                            break;
                        }
                    }
                    break;
                }
                case "OPTICAL": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValTr = ".trOpticalDataRow_IL";
                            selValChk = ".chkOptical_IL input[type=checkbox]";
                            selValTxt = "txtOpticalNumEmp_IL";
                            break;
                        }
                        case "IN": {
                            selValTr = ".trOpticalDataRow_IN";
                            selValChk = ".chkOptical_IN input[type=checkbox]";
                            selValTxt = "txtOpticalNumEmp_IN";
                            break;
                        }
                        case "OH": {
                            selValTr = ".trOpticalDataRow_OH";
                            selValChk = ".chkOptical_OH input[type=checkbox]";
                            selValTxt = "txtOpticalNumEmp_OH";
                            break;
                        }
                    }
                    break;
                }
                case "PHARMACIST": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValTr = ".trPharmacistsDataRow_IL";
                            selValChk = ".chkPharmacists_IL input[type=checkbox]";
                            selValTxt = "txtPharmacistsReceipts_IL";
                            break;
                        }
                        case "IN": {
                            selValTr = ".trPharmacistsDataRow_IN";
                            selValChk = ".chkPharmacists_IN input[type=checkbox]";
                            selValTxt = "txtPharmacistsReceipts_IN";
                            break;
                        }
                        case "OH": {
                            selValTr = ".trPharmacistsDataRow_OH";
                            selValChk = ".chkPharmacists_OH input[type=checkbox]";
                            selValTxt = "txtPharmacistsReceipts_OH";
                            break;
                        }
                    }
                    break;
                }
                case "PHOTOG": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkPhotog_IL input[type=checkbox]";
                            selValChk2 = ".chkPhotogSchedEquip_IL";
                            selValChk3 = ".chkPhotogMakeup_IL";
                            selValTr = ".trPhotogDataRow_IL";
                            selValTr2 = ".trPhotogMakeupAndHairCheckboxRow_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkPhotog_IN input[type=checkbox]";
                            selValChk2 = ".chkPhotogSchedEquip_IN";
                            selValChk3 = ".chkPhotogMakeup_IN";
                            selValTr = ".trPhotogDataRow_IN";
                            selValTr2 = ".trPhotogMakeupAndHairCheckboxRow_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkPhotog_OH input[type=checkbox]";
                            selValChk2 = ".chkPhotogSchedEquip_OH";
                            selValChk3 = ".chkPhotogMakeup_OH";
                            selValTr = ".trPhotogDataRow_OH";
                            selValTr2 = ".trPhotogMakeupAndHairCheckboxRow_OH";
                            break;
                        }
                    }
                    break;
                }
                case "PHOTOG_SCHED": {   // Photographers Schedule Equipment
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkPhotogSchedEquip_IL input[type=checkbox]";
                            selValTr = ".trPhotogScheduleEquipDataRow_IL";
                            selValTxt = ".txtPhotogLimit_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkPhotogSchedEquip_IN input[type=checkbox]";
                            selValTr = ".trPhotogScheduleEquipDataRow_IN";
                            selValTxt = ".txtPhotogLimit_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkPhotogSchedEquip_OH input[type=checkbox]";
                            selValTr = ".trPhotogScheduleEquipDataRow_OH";
                            selValTxt = ".txtPhotogLimit_OH";
                            break;
                        }
                    }
                    break;
                }
                case "PHOTOG_MAKEUP": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkPhotogMakeup_IL input[type=checkbox]";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkPhotogMakeup_IN input[type=checkbox]";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkPhotogMakeup_OH input[type=checkbox]";
                            break;
                        }
                    }
                    break;
                }
                case "PRINTERS": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkPrinters_IL input[type=checkbox]";
                            selValTr = ".trPrintersDataRow_IL";
                            selValTxt = ".txtPrintersNumLocs_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkPrinters_IN input[type=checkbox]";
                            selValTr = ".trPrintersDataRow_IN";
                            selValTxt = ".txtPrintersNumLocs_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkPrinters_OH input[type=checkbox]";
                            selValTr = ".trPrintersDataRow_OH";
                            selValTxt = ".txtPrintersNumLocs_OH";
                            break;
                        }
                    }
                    break;
                }
                case "RESCLEAN": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkResCleaning_IL input[type=checkbox]";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkResCleaning_IN input[type=checkbox]";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkResCleaning_OH input[type=checkbox]";
                            break;
                        }
                    }
                    break;
                }
                case "REST": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValTr = ".trRestaurantDataRow_IL";
                            selValChk = ".chkRestaurant_IL input[type=checkbox]";
                            selValDD1 = ".ddlRestaurantLimitOfLiability_IL"; // Motel Liability Limit
                            selValDD2 = ".ddlRestaurantDeductible_IL"; // Safe Limit
                            break;
                        }
                        case "IN": {
                            selValTr = ".trRestaurantDataRow_IN";
                            selValChk = ".chkRestaurant_IN input[type=checkbox]";
                            selValDD1 = ".ddlRestaurantLimitOfLiability_IN"; // Motel Liability Limit
                            selValDD2 = ".ddlRestaurantDeductible_IN"; // Safe Limit
                            break;
                        }
                        case "OH": {
                            selValTr = ".trRestaurantDataRow_OH";
                            selValChk = ".chkRestaurant_OH input[type=checkbox]";
                            selValDD1 = ".ddlRestaurantLimitOfLiability_OH"; // Motel Liability Limit
                            selValDD2 = ".ddlRestaurantDeductible_OH"; // Safe Limit
                            break;
                        }
                    }
                    break;
                }
                case "SELFSTORAGE": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkSelfStorage_IL input[type=checkbox]";
                            selValTr = ".trSelfStorageDataRow_IL";
                            selValTxt = ".txtSelfStorageLimit_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkSelfStorage_IN input[type=checkbox]";
                            selValTr = ".trSelfStorageDataRow_IN";
                            selValTxt = ".txtSelfStorageLimit_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkSelfStorage_OH input[type=checkbox]";
                            selValTr = ".trSelfStorageDataRow_OH";
                            selValTxt = ".txtSelfStorageLimit_OH";
                            break;
                        }
                    }
                    break;
                }
                case "VET": {
                    // Set the classes to search on.  The classes were set in the code-behind on populate
                    switch (StateAbbrev) {
                        case "IL": {
                            selValChk = ".chkVet_IL input[type=checkbox]";
                            selValTr = ".trVetDataRow_IL";
                            selValTxt = ".txtVetNumEmp_IL";
                            break;
                        }
                        case "IN": {
                            selValChk = ".chkVet_IN input[type=checkbox]";
                            selValTr = ".trVetDataRow_IN";
                            selValTxt = ".txtVetNumEmp_IN";
                            break;
                        }
                        case "OH": {
                            selValChk = ".chkVet_OH input[type=checkbox]";
                            selValTr = ".trVetDataRow_OH";
                            selValTxt = ".txtVetNumEmp_OH";
                            break;
                        }
                    }
                    break;
                }
            };
        }  //if (cb && StateAbbrev)

        // Now that we have all the selectors set we can manipulate the fields
        if (cb.checked) {
            // Checkbox Checked - Check and disable all but the first (in the same state)
            ndx = -1;
            if (selValChk) {
                $(selValChk).each(function () {
                    ndx += 1;
                    this.checked = true;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
            ndx = -1;
            if (selValChk2) {
                $(selValChk2).each(function () {
                    ndx += 1;
                    this.checked = true;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
            ndx = -1;
            if (selValChk3) {
                $(selValChk3).each(function () {
                    ndx += 1;
                    this.checked = true;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
            if (selValTr) {
                $(selValTr).each(function () {
                    this.style.display = '';
                });
            }
            if (selValTr2) {
                $(selValTr2).each(function () {
                    this.style.display = '';
                });
            }
            ndx = -1;
            if (selValTxt) {
                $(selValTxt).each(function () {
                    ndx += 1;
                    if (ndx > 0) { this.disabled = true;}
                });
            }
            ndx = -1;
            if (selValTxt2) {
                $(selValTxt2).each(function () {
                    ndx += 1;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
            ndx = -1;
            if (selValDD1) {
                $(selValDD1).each(function () {
                    ndx += 1;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
            ndx = -1;
            if (selValDD2) {
                $(selValDD2).each(function () {
                    ndx += 1;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
            ndx = -1;
            if (selValDD3) {
                $(selValDD3).each(function () {
                    ndx += 1;
                    if (ndx > 0) { this.disabled = true; }
                });
            }
        }
        else {
            // Checkbox unchecked - Uncheck and enable the first one, disable the rest (in the same state)
            ndx = -1;
            if (selValChk) {
                $(selValChk).each(function () {
                    ndx += 1;
                    if (ndx == 0) {
                        // Enable the first one
                        this.disabled = false;
                    }
                    else {
                        // Disable all but the first
                        this.disabled = true;
                    }
                    //this.parentElement.disabled = false;
                    this.checked = false;
                });
            }
            if (selValTr) {
                $(selValTr).each(function () {
                    this.style.display = 'none';
                });
            }
            if (selValTr2) {
                $(selValTr2).each(function () {
                    this.style.display = 'none';
                });
            }
            if (selValTxt) {
                $(selValTxt).each(function () {
                    this.disabled = false;
                    this.value = '';
                });
            }
            if (selValTxt2) {
                $(selValTxt2).each(function () {
                    this.disabled = false;
                    this.value = '';
                });
            }
            if (selValDD1) {
                $(selValDD1).each(function () {
                    this.disabled = false;
                });
            }
            if (selValDD2) {
                $(selValDD2).each(function () {
                    this.disabled = false; 
                });
            }
            if (selValDD3) {
                $(selValDD3).each(function () {
                    this.disabled = false;
                });
            }
        }
        return true;
    };

    // Handles all non-professional liability BOP building coverage checkbox clicks 
    // Liability coverages have theior own script (ProfessionalLiabilityCheckboxChanged)
    this.CoverageCheckboxChanged = function (CheckBoxId, DataTableRowId)
    {
        var cb = document.getElementById(CheckBoxId);
        var datarow = document.getElementById(DataTableRowId);

        if (cb.checked == true)
        {
            if (datarow != null) { datarow.style.display = ''; }
        } else
        {
            if (confirm('Are you sure you want to delete this coverage?') == true) {
                if (datarow != null) {
                    datarow.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId)
                }
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

    this.BusinessStartedLessThanThreeYearsAgo = function(StartedDateTxtId, YrsExpLIId, YrsExpTxtId) {
        var dt = document.getElementById(StartedDateTxtId).value;
        var li = document.getElementById(YrsExpLIId);
        var txt = document.getElementById(YrsExpTxtId);
        if (!moment(dt, 'MM/DD/YYYY', true).isValid() || !moment(dt, 'M/D/YYYY', true).isValid()) {
            li.style.display='none';
            return false;
        } else {
            var mydt = new Date(dt);
            var dtnow = new Date();
            var yr = dtnow.getFullYear() - 3;
            // Do not add month.  This changes the output of the later Date by one month. Javascript date Months are zero based.
            //var mo = dtnow.getMonth() + 1;
            var mo = dtnow.getMonth();
            var dy = dtnow.getDate();
            var dtminusthreeyears = new Date(yr, mo, dy);
            //Updated 9/20/2022 for bug 67839 MLW
            //if (mydt > dtminusthreeyears)
            if (mydt > dtminusthreeyears && mydt <= new Date(dtnow.getFullYear(), mo, dy))
            {
                //alert('Date business started Is less than 3 years ago. Please enter years of experience In this type of business operation.');
                //updated 10/7/2017
                if (txt.value.length == 0 || txt.value == '' || txt.value == '0'){
                    alert('Date business started is less than 3 years ago. Please enter years of experience in this type of business operation.');
                }
                li.style.display='';
            }
            else 
            {
                li.style.display='none';
                txt.value = '';
            }
            return true;
        }
    };

    this.ContractorsBlanketLimitChanged = function (txtId, trId) {
        var txt1 = document.getElementById(txtId);
        var trRow = document.getElementById(trId);
        if (txt1.value != '') {
            trRow.style.display = '';
        }
        else {
            trRow.style.display = 'none';
        }
    }

    // Page bindings for the BOP PL Coverages
    this.BOPPLCoverageBindings = new Array();
    this.BOPPLCoveragUIBinding = function (chkAptId, txtAptNumLocsId, trAptDataRowId, ddAptLimitId, ddAptDedId, chkBarbersId, trBarbersDataRowId, txtBarbersFullTimeEmpId, txtBarbersPartTimeEmpId, chkBeautyId, trBeautyDataRowId, txtBeautyFullTimeEmpId, txtBeautyPartTimeEmpId, chkFineArtsId, chkFuneralId, trFuneralDataRowId, txtFuneralNumEmpId, chkLiquorId, trLiquorDataRowId, lblLiquorLimitId, lblLiquorClassCodeId, txtLiquorSalesId, chkMotelId, trMotelDataRowId, ddMotelLimitId, ddMotelSafeLimitId, ddMotelSafeDedId, chkOpticalId, trOpticalDataRowId, txtOpticalNumEmpId, chkPharmacistsId, trPharmacistsDataRowId, txtPharmacistsReceiptsId, chkPhotogId, trPhotogDataRowId, chkPhotogSchedEquipId, trPhotogSchedEquipDataRowId, txtPhotogSchedEquipLimitId, trPhotogMakeupCheckboxRowId, chkPhotogMakeupId, chkPrintersId, trPrintersDataRowId, txtPrintersNumLocsId, chkResCleaningId, chkRestaurantsId, trRestaurantsDataRowId, ddRestaurantLiabId, ddRestaurantDedId, chkSelfStorageId, trSelfStorageDataRowId, txtSelfStorageLimitId, chkVetId, trVetDataRowId, txtVetNumEmpId) {
        // Apartment
        this.chkApt = chkAptId;
        this.txtAptNumLocs = txtAptNumLocsId;
        this.trAptDataRow = trAptDataRowId;
        this.ddAptLimit = ddAptLimitId;
        this.ddAptDed = ddAptDedId;
        // Barbers
        this.chkBarbers = chkBarbersId;
        this.trBarbersDataRow = trBarbersDataRowId;
        this.txtBarbersFullTimeEmp = txtBarbersFullTimeEmpId;
        this.txtBarbersPartTimeEmp = txtBarbersPartTimeEmpId;
        // Beauticians
        this.chkBeauty = chkBeautyId;
        this.trBeautyDataRow = trBeautyDataRowId;
        this.txtBeautyFullTimeEmp = txtBeautyFullTimeEmpId;
        this.txtBeautyPartTimeEmp = txtBeautyPartTimeEmpId;
        // Fine Arts
        this.chkFineArts = chkFineArtsId;
        // Funeral Directors
        this.chkFuneral = chkFuneralId;
        this.trFuneralDataRow = trFuneralDataRowId;
        this.txtFuneralNumEmp = txtFuneralNumEmpId;
        // Liquor Liability
        this.chkLiquor = chkLiquorId;
        this.trLiquorDataRow = trLiquorDataRowId;
        this.lblLiquorLimit = lblLiquorLimitId;
        this.lblLiquorClassCode = lblLiquorClassCodeId;
        this.txtLiquorSales = txtLiquorSalesId;
        // Motel
        this.chkMotel = chkMotelId;
        this.trMotelDataRow = trMotelDataRowId;
        this.ddMotelLimit = ddMotelLimitId;
        this.ddMotelSafeLimit = ddMotelSafeLimitId;
        this.ddMotelSafeDed = ddMotelSafeDedId;
        // Optical
        this.chkOptical = chkOpticalId;
        this.trOpticalDataRow = trOpticalDataRowId;
        this.txtOpticalNumEmp = txtOpticalNumEmpId;
        // Pharmacists
        this.chkPharmacists = chkPharmacistsId;
        this.trPharmacistsDataRow = trPharmacistsDataRowId;
        this.txtPharmacistsReceipts = txtPharmacistsReceiptsId;
        // Photograhy
        this.chkPhotog = chkPhotogId;
        this.trPhotogDataRow = trPhotogDataRowId;
        this.chkPhotogSchedEquip = chkPhotogSchedEquipId;
        this.trPhotogSchedEquipDataRow = trPhotogSchedEquipDataRowId;
        this.txtPhotogSchedEquipLimit = txtPhotogSchedEquipLimitId;
        this.chkPhotogMakeup = chkPhotogMakeupId;
        this.trPhotogMakeupCheckboxRow = trPhotogMakeupCheckboxRowId;
        // Printers
        this.chkPrinters = chkPrintersId;
        this.trPrintersDataRow = trPrintersDataRowId;
        this.txtPrintersNumLocs = txtPrintersNumLocsId;
        // Residential Cleaning
        this.chkResCleaning = chkResCleaningId;
        // Restaurants
        this.chkRestaurants = chkRestaurantsId;
        this.trRestaurantsDataRow = trRestaurantsDataRowId;
        this.ddRestaurantLiab = ddRestaurantLiabId;
        this.ddRestaurantDed = ddRestaurantDedId;
        // Self Storage
        this.chkSelfStorage = chkSelfStorageId;
        this.trSelfStorageDataRow = trSelfStorageDataRowId;
        this.txtSelfStorageLimit = txtSelfStorageLimitId;
        // Veterinarians
        this.chkVet = chkVetId;
        this.trVetDataRow = trVetDataRowId;
        this.txtVetNumEmp = txtVetNumEmpId;
    };

    // This doesn't work right - can't get the calendar image to disable
    //this.IssuedBoundOnChanged = function (chkId, dtId) {
    //    var chkbox = document.getElementById(chkId);
    //    var dt = document.getElementById(dtId);
    //    var dtTxt = document.getElementById(dtId + "_TextBox");
    //    var dtBtn = document.getElementById(dtId + "_Image");
    //    if (chkbox.checked) {
    //        dtTxt.disabled = false;
    //        dt.disabled = false;
    //        dt.ReadOnly = false;
    //    }
    //    else {
    //        dt.ReadOnly = true;
    //        dtTxt.value = "";
    //        dtTxt.disabled = true;
    //        dt.disabled = true;
    //    }
    //}
};

////Added 11/6/18 for multi state MLW
//function ToggleMineSubsidence(ddState, txtCounty, dvMineSubsidence, chkMineSubsidence,
//    dvMineSubsidenceReqHelpInfo, dvMineSubsidenceNotReqHelpInfo, hiddenMineSubIsChecked, hiddenMineSubIsEnabled) {
//    switch (ddState) {
//        case "IL", "15":
//            //if IL show checkbox regardless of county
//            document.getElementById(dvMineSubsidence).style.display = "";
//            //for IL only - show checkboxes checked/unchecked accordingly
//            ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateAbbreviation('IL', function (data) {
//                if (data.contains(txtCounty)) {
//                    document.getElementById(chkMineSubsidence).checked = true;
//                    document.getElementById(chkMineSubsidence).disabled = true;
//                    document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "";
//                    document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
//                    document.getElementById(chkMineSubsidence).className = "chkMine_IL_Reqd";
//                    document.getElementById(dvMineSubsidenceReqHelpInfo).className = "trMineInfo_IL_Reqd informationalText";
//                    document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
//                } else {
//                    if (document.getElementById(hiddenMineSubIsChecked).value == "True") {
//                        document.getElementById(chkMineSubsidence).checked = true;
//                        if (document.getElementById(hiddenMineSubIsEnabled).value == "True") {
//                            document.getElementById(chkMineSubsidence).disabled = false;
//                        } else {
//                            document.getElementById(chkMineSubsidence).disabled = true;
//                        }
//                        document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
//                        document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "";
//                    } else {
//                        document.getElementById(chkMineSubsidence).checked = false;
//                        document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
//                        document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
//                    }
//                    document.getElementById(chkMineSubsidence).className = "chkMine_IL_Bldg_Enabled_NotReqd";
//                    document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
//                    document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "trMineInfo_IL_NotReqd informationalText";
//                }
//            });
//            break;
//        case "IN", "16":
//            //if mine county, then show mine sub checkbox. Otherwise, do not show
//            ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateAbbreviation('IN', function (data) {
//                if (data.contains(txtCounty)) {
//                    document.getElementById(dvMineSubsidence).style.display = "";
//                    document.getElementById(chkMineSubsidence).checked = false;
//                } else {
//                    document.getElementById(dvMineSubsidence).style.display = "none";
//                    document.getElementById(chkMineSubsidence).checked = false;
//                }
//            });
//            document.getElementById(chkMineSubsidence).className = "chkMine_IN";
//            document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
//            document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
//            document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
//            document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
//            break;
//        default:
//            //not yet supported, do not show mine sub checkbox, do not show info texts req & not
//            document.getElementById(dvMineSubsidence).style.display = "none";
//            document.getElementById(chkMineSubsidence).checked = false;
//            document.getElementById(chkMineSubsidence).className = "chkMine_IN";
//            document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
//            document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
//            document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
//            document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
//            break;
//    }
//}

//Added 11/29/18 for multi state MLW
$(document).ready(function () {

    //For BOP only
    if (master_LobId == IFMLOBEnum.BOP.LobId) {
        //Check Mine Sub options on page load
        checkMineSub()

        // Perform these actions if the ZIP, State, or County checkbox change
        $('[id*=txtZipCode],[id*=ddStateAbbrev],[id*=txtGaragedCounty]').on('change', function () {
            checkMineSub()
        });

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

        function checkMineSub() {
            var mineSubNotReqCounter = 0;
            $('input[id*=chkMineSubsidence]').each(function () {
                var mineSubElement = $(this);
                var mineSubState = getLocationStateValue(mineSubElement);
                var mineSubCounty = getLocationCountyText(mineSubElement);
                if (mineSubState == '36') { return true;}  // Don't use this function for OH
                mineSubElement.closest('[id*=trMineSubsidenceRow]').hide();
                mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_IL]').hide();
                mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL]').hide();
                var hasMineSubChecked = false;
                switch (mineSubState) {
                    case '15':
                        if (multiStateEnabled == true) {
                            mineSubElement.closest('[id*=trMineSubsidenceRow]').show();
                            ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                                if (data.contains(mineSubCounty)) {
                                    mineSubElement.prop("checked", true);
                                    mineSubElement.prop("disabled", true);
                                    mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoForRequiredMineSubsidence_IL]').show();
                                }
                                else {
                                    mineSubNotReqCounter += 1;
                                    //for added non-mine building after mine sub checked on other non-mine building
                                    if (mineSubElement.prop("checked")) {
                                        hasMineSubChecked = true;
                                    }
                                    if (hasMineSubChecked == false) {
                                        $('input[id*=chkMineSubsidence]').each(function () {
                                            var chkMine = $(this);
                                            var mineState = getLocationStateValue(chkMine)
                                            var mineCounty = getLocationCountyText(chkMine)
                                            switch (mineState) {
                                                case '15':
                                                    ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineState, function (data) {
                                                        if (hasMineSubChecked == false && !data.contains(mineCounty)) {
                                                            if (chkMine.prop('checked')) {
                                                                hasMineSubChecked = true; // Used to skip processing of other counties
                                                                mineSubElement.prop("checked", true);
                                                                if (mineSubNotReqCounter > 1) {
                                                                    mineSubElement.prop("disabled", true);
                                                                    mineSubElement.prop("checked", true)
                                                                } else {
                                                                    mineSubElement.prop("disabled", false);
                                                                }
                                                                mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL]').show();
                                                            }
                                                        }
                                                    });
                                                    break;
                                            }
                                        });
                                    }
                                    if (hasMineSubChecked) {
                                        mineSubElement.prop("checked", true);
                                    } else {
                                        mineSubElement.prop("checked", false);
                                    }
                                    //Let the first non-mine building mine sub coverage checkbox be enabled, all others disabled                                   
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
                                        mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL]').show();
                                    }
                                }
                            });    
                        }
                        break;
                    //case '36':  
                    //    // I put this code here for Ohio but at this time Ohio does not have optional counties,
                    //    // It may be useful if Ohio starts allowing Mine Sub on optional counties in the future.  MGB 8/25/2020
                    //    if (multiStateEnabled == true) {
                    //        mineSubElement.closest('[id*=trMineSubsidenceRow]').show();
                    //        ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                    //            if (data.contains(mineSubCounty)) {
                    //                mineSubElement.prop("checked", true);
                    //                mineSubElement.prop("disabled", true);
                    //                mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfo_OH]').show();
                    //            }
                    //            else {
                    //                mineSubNotReqCounter += 1;
                    //                //for added non-mine building after mine sub checked on other non-mine building
                    //                if (mineSubElement.prop("checked")) {
                    //                    hasMineSubChecked = true;
                    //                }
                    //                if (hasMineSubChecked == false) {
                    //                    $('input[id*=chkMineSubsidence]').each(function () {
                    //                        var chkMine = $(this);
                    //                        var mineState = getLocationStateValue(chkMine)
                    //                        var mineCounty = getLocationCountyText(chkMine)
                    //                        switch (mineState) {
                    //                            case '36':
                    //                                ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineState, function (data) {
                    //                                    if (hasMineSubChecked == false && !data.contains(mineCounty)) {
                    //                                        if (chkMine.prop('checked')) {
                    //                                            hasMineSubChecked = true; // Used to skip processing of other counties
                    //                                            mineSubElement.prop("checked", true);
                    //                                            if (mineSubNotReqCounter > 1) {
                    //                                                mineSubElement.prop("disabled", true);
                    //                                                mineSubElement.prop("checked", true)
                    //                                            } else {
                    //                                                mineSubElement.prop("disabled", false);
                    //                                            }
                    //                                            mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoNOT_OH]').show();
                    //                                        }
                    //                                    }
                    //                                });
                    //                                break;
                    //                        }
                    //                    });
                    //                }
                    //                if (hasMineSubChecked) {
                    //                    mineSubElement.prop("checked", true);
                    //                } else {
                    //                    mineSubElement.prop("checked", false);
                    //                }
                    //                //Let the first non-mine building mine sub coverage checkbox be enabled, all others disabled                                   
                    //                if (mineSubElement.prop('checked')) {
                    //                    if (mineSubNotReqCounter > 1) {
                    //                        mineSubElement.prop("disabled", true);
                    //                        mineSubElement.prop("checked", true)
                    //                    } else {
                    //                        mineSubElement.prop("disabled", false);
                    //                    }
                    //                } else {
                    //                    mineSubElement.prop("disabled", false);
                    //                }
                    //                if (mineSubElement.prop('checked')) {
                    //                    mineSubElement.closest('[id*=trMineSubsidenceRow]').siblings('[id*=trMineSubsidenceInfoNOT_OH]').show();
                    //                }
                    //            }
                    //        });
                    //    }
                    //    break;
                    case '16':
                        ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(mineSubState, function (data) {
                            if (data.contains(mineSubCounty)) {
                                mineSubElement.closest('[id*=trMineSubsidenceRow]').show();
                            }
                        });
                        break;
                }
            });
        }

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
    }
});

