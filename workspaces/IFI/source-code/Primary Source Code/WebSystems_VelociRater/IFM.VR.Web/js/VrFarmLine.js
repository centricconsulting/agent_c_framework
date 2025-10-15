
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />

$(document).ready(function () {
    // Custom Feeding - Cattle
    $(".ddlCFLimit_Cattle").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.value;
            $(".ddlCFLimit_Cattle").each(function () {
                if (this != sender) {
                    this.value = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    $(".txtCFDesc_Cattle").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtCFDesc_Cattle").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Custom Feeding - Equine
    $(".ddlCFLimit_Equine").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.value;
            $(".ddlCFLimit_Equine").each(function () {
                if (this != sender) {
                    this.value = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    $(".txtCFDesc_Equine").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtCFDesc_Equine").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    //// Custom Feeding - Poultry
    $(".ddlCFLimit_Poultry").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.value;
            $(".ddlCFLimit_Poultry").each(function () {
                if (this != sender) {
                    this.value = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    $(".txtCFDesc_Poultry").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtCFDesc_Poultry").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Custom Feeding - Swine
    $(".ddlCFLimit_Swine").each(function () {
        $(this).change(function () {
            var sender = this;
            var ddlValue = sender.value;
            $(".ddlCFLimit_Swine").each(function () {
                if (this != sender) {
                    this.value = ddlValue;
                    this.disabled = true;
                }
            });
        });
    });
    $(".txtCFDesc_Swine").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtCFDesc_Swine").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });


    // Suffocation of Livestock or Poultry - Cattle
    // Limit
    $(".txtSuffLimit_Cattle").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffLimit_Cattle").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Description
    $(".txtSuffDesc_Cattle").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffDesc_Cattle").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // Suffocation of Livestock or Poultry - Equine
    // Limit
    $(".txtSuffLimit_Equine").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffLimit_Equine").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Description
    $(".txtSuffDesc_Equine").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffDesc_Equine").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // Suffocation of Livestock or Poultry - Poultry
    // Limit
    $(".txtSuffLimit_Poultry").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffLimit_Poultry").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Description
    $(".txtSuffDesc_Poultry").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffDesc_Poultry").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });

    // Suffocation of Livestock or Poultry - Swine
    // Limit
    $(".txtSuffLimit_Swine").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffLimit_Swine").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });
    // Description
    $(".txtSuffDesc_Swine").each(function () {
        $(this).keyup(function () {
            var sender = this;
            var textBoxData = sender.value;
            $(".txtSuffDesc_Swine").each(function () {
                if (this != sender) {
                    this.value = textBoxData;
                    this.disabled = true;
                }
            });
        });
    });



});

// Personal Liability popup - open
function ShowPersLiabPopup(divName) {
    $("#" + divName).dialog({
        title: 'Personal Liability',
        width: 400,
        draggable: true,
        autoOpen: true,
        modal: true,
        dialogClass: "no-close"
    });
}

// Personal Liability popup - close 
function ClosePersLiabPopup(divName) {
    $("#" + divName).dialog('close');
    return true;
}

// Information Popup
function InitFarmPopupInfo(divName, popTitle) {
    $("#" + divName).dialog({
        title: popTitle,
        width: 400,
        draggable: true,
        autoOpen: true,
        modal: true,
        dialogClass: "no-close"
    });

    DisplayFarmPopupInfo(divName);
}

function DisplayFarmPopupInfo(divName) {
    $("#" + divName).dialog('open');
}

function CloseFarmPopupInfo(divName) {
    $("#" + divName).dialog('close');
}

// OH - Stop Gap clicks
function HandleStopGapClicks(chkId, ddLimitId, divDataId) {
    var chk = document.getElementById(chkId);
    var ddLimit = document.getElementById(ddLimitId);
    var divData = document.getElementById(divDataId);

    if (chk && ddLimit && divData) {
        if (chk.checked) {
            divData.style.display = "block";
            ddLimit.style.display = "block";
        }
        else {
            if (!confirm('Are you sure you want to delete this item?')) {
                chk.checked = true;
            }
            else {
                ddLimit.style.display = "none";
                divData.style.display = "none";
            }
        }
    }
    return true;
}

// OH - Handle Cosmetic Damage Clicks.  Dwellings and Buildings.
function HandleCosmeticDamageClicks(chkId, divDataId) {
    var chk = document.getElementById(chkId);
    var divData = document.getElementById(divDataId);

    if (chk && divData) {
        if (chk.checked) {
            divData.style.display = 'block';
        }
        else {
            if (!confirm('Are you sure you want to delete this item?')) {
                chk.checked = true;
            }
            else {
                divData.style.display = "none";
            }
        }
    }
    return true;
}

function HandleCosmeticDamageBuildings(buildingId, isCosmeticDamageHidden, isCosmeticDamagePreexistingOnBuilding, hdnHasCosmeticDamagePreexisting) {
    var buildingElement = $("#" + buildingId);
    var state = $(buildingElement).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=ddStateAbbrev]').val();
    var cosmeticDiv = $(buildingElement).closest('[id*=divContents]').find('[id*=dvBuildingCoverages]').find('div[id*=dvCosmeticDamageExclusion]').first();
    var cosmeticChk = $(cosmeticDiv).find('input');
    var cosmeticPreexisting = document.getElementById(hdnHasCosmeticDamagePreexisting);
    if (cosmeticChk && cosmeticDiv && cosmeticPreexisting && isCosmeticDamageHidden.toUpperCase() == 'TRUE') {
        var bldgType = buildingElement.val();
        switch (bldgType) {
            case '17':
            case '18':
            case '26':
            case '27':
            case '28':
            case '33':
            case '34':
            case '35':
            case '36':
            case '37':
                //17=Mobile Home Dwelling, 18=Farm Dwelling, 26=Well Pumps, 27=Grain Dryer, 28=Grain Legs
                //33=Tanks, 34=Private Power & Light Poles, 35=Radio & Television Equipment,
                //36=Windmills and Chargers, 37=Outbuilding with Living Quarters
                if ((ifm.vr.currentQuote.isEndorsement && isCosmeticDamagePreexistingOnBuilding.toUpperCase() == 'FALSE') || !ifm.vr.currentQuote.isEndorsement) {
                    cosmeticChk.prop("checked", false);
                }
                cosmeticDiv.hide();
                break;
            default:
                cosmeticDiv.show();
                cosmeticPreexisting.value = "False";
                break;
        }
    } else {
        if (buildingElement.val() == "17" || buildingElement.val() == "18") { //Mobile
            cosmeticChk.prop("checked", false);
            cosmeticDiv.hide();
        }
        else {
            cosmeticDiv.show();
            //if (state != 16) {
            //    /*cosmeticChk.prop("checked", true);*/
            //}
        }
    }
}

// OH - Pesticide/Herbicide checkbox (coverage page)
function HandlePestClicks(chkId) {
    var chk = document.getElementById(chkId);
    if (chk) {
        if (!chk.checked) {
            if (!confirm('Are you sure you want to delete this item?')) {
                chk.checked = true;
            }
        }
    }
    return true;
}

// OH - Handles mine subsidence checkbox clicks on the  dwelling and farm buildings
function HandleOHMineSubClicks(chkId, IsOptionalMineSubCounty, LocNdx) {
    var chk = document.getElementById(chkId);
    var opt = false;
    var tagname = ".chkOHMineSubOptional_" + LocNdx;
    if (IsOptionalMineSubCounty == '1') { opt = true; }
    if (chk && opt) {
        if (chk.checked) {
            if (opt) {
                $(tagname).each(function () {
                    var chk = this.children[0];
                    chk.checked = true;
                });
                alert('Mine Subsidence added to all qualified dwellings at this location.');
            }
        }
        else {
            if (!confirm('Are you sure you want to delete this item?')) {
                chk.checked = true;
            }
            else {
                if (opt) {
                    $(tagname).each(function () {
                        var chk = this.children[0];
                        chk.checked = false;
                    });
                    alert('Mine Subsidence removed from all qualified dwellings at this location.');
                }
            }
        }
    }
    return true;
}

function HandleILMineSubClicks(chkId, LocNdx) {
    var chk = document.getElementById(chkId);
    var tagname = ".chkILMineSubsidence_" + LocNdx;
    if (chk) {
        if (chk.checked) {
            $(tagname).each(function () {
                var chk = this.children[0];
                chk.checked = true;
            });
            alert('Mine Subsidence added to all qualified dwellings at this location.');

        }
        else {
            if (!confirm('Are you sure you want to delete this item?')) {
                chk.checked = true;
            }

            else {
                $(tagname).each(function () {
                    var chk = this.children[0];
                    chk.checked = false;
                });
                alert('You have chosen to opt out of Mine Subsidence coverage. Your quote will need to be routed to underwriting for review.');
            }
        }
    }
    return true;
}

function HandleCustomFeedingCheckboxClicks(chkId, dataDivId) {
    var chk = document.getElementById(chkId);
    var dataDiv = document.getElementById(dataDivId);

    if (chk) {
        if (chk.checked) {
            $(".chkCF").each(function () {
                var chk2 = this.children[0];
                if (chk2 != chk) {
                    chk2.checked = true;
                }
            });
            $(".divCFData").each(function () {
                this.style.display = "block";
            });
            $(".ddlCFLimit_Cattle").each(function () {
                this.style.disabled = false;
            });
            $(".txtCFDesc_Cattle").each(function () {
                this.style.disabled = false;
            });
            $(".ddlCFLimit_Equine").each(function () {
                this.style.disabled = false;
            });
            $(".txtCFDesc_Equine").each(function () {
                this.style.disabled = false;
            });
            $(".ddlCFLimit_Poultry").each(function () {
                this.style.disabled = false;
            });
            $(".txtCFDesc_Poultry").each(function () {
                this.style.disabled = false;
            });
            $(".ddlCFLimit_Swine").each(function () {
                this.style.disabled = false;
            });
            $(".txtCFDesc_Swine").each(function () {
                this.style.disabled = false;
            });
        }
        else {
            if (!confirm("This will delete the coverage from all buildings on this quote.  Are you sure you want to delete this item?")) {
                chk.checked = true;
                return true;
            }
            $(".chkCF").each(function () {
                var chk2 = this.children[0];
                if (chk2 != chk) {
                    chk2.checked = false;
                }
            });
            $(".divCFData").each(function () {
                this.style.display = "none";
            });
            $(".ddlCFLimit_Cattle").each(function () {
                this.value = "0";
            });
            $(".txtCFDesc_Cattle").each(function () {
                this.value = "";
            });
            $(".ddlCFLimit_Equine").each(function () {
                this.value = "0";
            });
            $(".txtCFDesc_Equine").each(function () {
                this.value = "";
            });
            $(".ddlCFLimit_Poultry").each(function () {
                this.value = "0";
            });
            $(".txtCFDesc_Poultry").each(function () {
                this.value = "";
            });
            $(".ddlCFLimit_Swine").each(function () {
                this.value = "0";
            });
            $(".txtCFDesc_Swine").each(function () {
                this.value = "";
            });
        }
        return true;
    }
}

function HandleSuffocationOfLivestockCheckboxClicks(chkId) {
    var chk = document.getElementById(chkId);

    if (chk) {
        if (chk.checked) {
            $(".chkSuffocation").each(function () {
                var chk2 = this.children[0];
                if (chk2 != chk) {
                    chk2.checked = true;
                }
            });
            $(".divSuffData").each(function () {
                this.style.display = "block";
            });
        }
        else {
            if (!confirm("This will delete the coverage from all buildings on this quote.  Are you sure you want to delete this item?")) {
                chk.checked = true;
                return true;
            }
            $(".chkSuffocation").each(function () {
                var chk2 = this.children[0];
                if (chk2 != chk) {
                    chk2.checked = false;
                }
            });
            $(".divSuffData").each(function () {
                this.style.display = "none";
            });
            $(".ddlSuffLimit_Cattle").each(function () {
                this.value = "";
            });
            $(".txtSuffDesc_Cattle").each(function () {
                this.value = "";
            });
            $(".txtSuffLimit_Equine").each(function () {
                this.value = "";
            });
            $(".txtSuffDesc_Equine").each(function () {
                this.value = "";
            });
            $(".txtSuffLimit_Poultry").each(function () {
                this.value = "";
            });
            $(".txtSuffDesc_Poultry").each(function () {
                this.value = "";
            });
            $(".txtSuffLimit_Swine").each(function () {
                this.value = "";
            });
            $(".txtSuffDesc_Swine").each(function () {
                this.value = "";
            });
        }
    }
    return true;
}

// Handles GL-9 checkbox clicks on the location controls
function HandleGL9PersonalLiabilityCheckboxClicks(chkId, divId, AddrDivId, ShowHideButtonId, rvExists, imExists, LastLocationWithGL9, hdnCheckboxValueId) {
    var chk = document.getElementById(chkId);
    var div = document.getElementById(divId);
    var AddrDiv = document.getElementById(AddrDivId);
    var ShowHideButton = document.getElementById(ShowHideButtonId)
    var hdnCheckboxValue = document.getElementById(hdnCheckboxValueId);

    if (chk && div) {
        if (chk.checked) {
            div.style.display = 'block';
            hdnCheckboxValue.value = "True";
            chk.disabled = true; // disable checkbox on check
            if (AddrDiv) { AddrDiv.style.display = 'block'; }
            if (ShowHideButton) { ShowHideButton.innerHTML = "Hide Address" }
        }
        else {
            hdnCheckboxValue.value = "False";
        }
    }
    return true;
}

function HandleAddlResidenceCheckboxClicks(chkId, divId) {
    var chk = document.getElementById(chkId);
    var div = document.getElementById(divId);

    if (chk && div) {
        if (chk.checked) {
            div.style.display = 'block';
        }
        else {
            div.style.display = 'none';
        }
    }
}

function HandleEditAddressClicks(divId, lbId) {
    var div = document.getElementById(divId);
    var lb = document.getElementById(lbId);
    if (div && lb) {
        //div.style.display = 'block';
        //lb.innerHTML = 'Hello!';
        if (lb.innerHTML == 'Edit Address') {
            // show
            div.style.display = 'block';
            lb.innerHTML = 'Hide Address';
        }
        else {
            // hide
            div.style.display = 'none';
            lb.innerHTML = 'Edit Address';
        }
        return true;
    }
}

//Removed 08/03/2023 - using code behind ddlLiabCovType_SelectedIndexChanged to handle functionality as we need to save and redirect the user to another page
//// Select Liability Coverage Form for a Select-O-Matic ONLY
//function ToggleExtraData(ddCoverageType, dvLiability, dvMedPay, dvEmpLiab, dvBusinessPursuits, dvFamilyMedPay, dvCustomFarming, dvFarmPollution, dvEPLI, dvAdditionalIns, dvIdentityFraud, dvPersLiab,
//    dvFarmAllStar, dvEquipBreak, chkEPLI, hiddenPolicyHolderType) {
//    var client = document.getElementById(ddCoverageType);

//    if (client.options[client.selectedIndex].text == "None") {
//        document.getElementById(dvLiability).style.display = "none";
//        document.getElementById(dvMedPay).style.display = "none";
//        document.getElementById(dvEmpLiab).style.display = "none";
//        document.getElementById(dvCustomFarming).style.display = "none";
//        document.getElementById(dvFarmPollution).style.display = "none";
//        document.getElementById(dvEPLI).style.display = "none";
//        //Maintain on Endorsements - 01/29/2021 CAH - Bug 59434
//        //document.getElementById(chkEPLI).checked = false;
//        setEPLI(chkEPLI, false)
//        document.getElementById(dvAdditionalIns).style.display = "none";
//    }
//    else {
//        document.getElementById(dvLiability).style.display = "block";
//        document.getElementById(dvMedPay).style.display = "block";
//        document.getElementById(dvEmpLiab).style.display = "block";
//        document.getElementById(dvCustomFarming).style.display = "block";
//        document.getElementById(dvFarmPollution).style.display = "block";
//        document.getElementById(dvAdditionalIns).style.display = "block";

//        if (document.getElementById(hiddenPolicyHolderType).value == "2") {
//            document.getElementById(dvEPLI).style.display = "block";
//            //Maintain on Endorsements - 01/29/2021 CAH - Bug 59434
//            //document.getElementById(chkEPLI).checked = true;
//            setEPLI(chkEPLI, true)
//        }
//    }

//    document.getElementById(dvBusinessPursuits).style.display = "none";
//    document.getElementById(dvFamilyMedPay).style.display = "none";
//    document.getElementById(dvIdentityFraud).style.display = "none";
//    //document.getElementById(dvPersLiab).style.display = "none";
//    document.getElementById(dvFarmAllStar).style.display = "none";
//    document.getElementById(dvEquipBreak).style.display = "none";
//}

// Displays dialog when coverage is removed
function ConfirmFarmDialog() {
    if (confirm("Are you sure you want to delete this item?"))
        return true;
    else
        return false;
}

// Toggle Employee Liability Data
function ToggleEmpLiab(chkEmpLiab, dvNumEmployees, txtFTEmp, txtPT41Days, txtPT40Days) {
    if (document.getElementById(chkEmpLiab).checked) {
        document.getElementById(dvNumEmployees).style.display = "block";
        document.getElementById(txtFTEmp).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvNumEmployees).style.display = "none";
            $("#" + txtFTEmp).val("");
            $("#" + txtPT41Days).val("");
            $("#" + txtPT40Days).val("");
        }
        else
            document.getElementById(chkEmpLiab).checked = true;
    }
}

// Toggle Business Pursuits Data
function ToggleBusinessPursuits(chkBusinessPursuits, dvBPInfo, ddlBPType, txtAnnualReceipts) {
    var client = document.getElementById(ddlBPType);

    if (document.getElementById(chkBusinessPursuits).checked) {
        document.getElementById(dvBPInfo).style.display = "block";
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvBPInfo).style.display = "none";
            //client.selectedIndex = 0;
            $("#" + txtAnnualReceipts).val("");
        }
        else
            document.getElementById(chkBusinessPursuits).checked = true;
    }
}

// Toggle Family Medical Pay Data
function ToggleFamilyMedPay(chkFamMedPay, dvFMPNumPer, txtFMPNumPer) {
    if (document.getElementById(chkFamMedPay).checked) {
        document.getElementById(dvFMPNumPer).style.display = "block";
        document.getElementById(txtFMPNumPer).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvFMPNumPer).style.display = "none";
            $("#" + txtFMPNumPer).val("");
        }
        else
            document.getElementById(chkFamMedPay).checked = true;
    }
}

// Toggle Canine Exclusion Data
function ToggleCanineExclusion(chkCanine, dvCanineInfo) {
    if (document.getElementById(chkCanine).checked) {
        document.getElementById(dvCanineInfo).style.display = "block";
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvCanineInfo).style.display = "none";
        }
        else
            document.getElementById(chkCanine).checked = true;
    }
}

//Added 6/15/2022 for task 72947 MLW
// Toggle Number of Units for Trampoline, Swimming Pools and Woodburning Stove
function ToggleNumOfUnits(chkItem, dvItem, txtNumOfUnits) {
    if (document.getElementById(chkItem).checked) {
        document.getElementById(dvItem).style.display = "block";
        document.getElementById(txtNumOfUnits).value = "1";
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvItem).style.display = "none";
            document.getElementById(txtNumOfUnits).value = "0";
        }
        else
            document.getElementById(chkItem).checked = true;
    }
}

// Toggle Generic Textbox input item
function ToggleGenTextboxInput(chkItem, dvItem) {
    if (document.getElementById(chkItem).checked) {
        document.getElementById(dvItem).style.display = "block";
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvItem).style.display = "none";
        }
        else
            document.getElementById(chkItem).checked = true;
    }
}

// Confirm Unselected checkbox
function ConfirmDeSelectedChkbox(chkItem) {
    if (document.getElementById(chkItem).checked == false) {
        if (ConfirmFarmDialog() == false) {
            document.getElementById(chkItem).checked = true;
        }
    }
}


// Toggle Custom Farming Data
function ToggleCustomfarming(chkCustomFarming, dvCFInfo, ddlCFType, txtCFAnnualReceipts) {
    var client = document.getElementById(ddlCFType);

    if (document.getElementById(chkCustomFarming).checked) {
        document.getElementById(dvCFInfo).style.display = "block";
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvCFInfo).style.display = "none";
            //client.selectedIndex = 0;
            $("#" + txtCFAnnualReceipts).val("");
        }
        else
            document.getElementById(chkCustomFarming).checked = true;
    }
}

// Toggle Farm Pollution Data
function ToggleFarmPollution(chkFarmPollution, dvFPIncreasedLimits, ddlFPLimit, policyHolderType, ddLiability) {
    var client = document.getElementById(ddlFPLimit);
    var clientLiability = document.getElementById(ddLiability);

    if (document.getElementById(chkFarmPollution).checked) {
        document.getElementById(dvFPIncreasedLimits).style.display = "block";
    }
    else {
        //if (policyHolderType != "2") {
        if (ConfirmFarmDialog())
            document.getElementById(dvFPIncreasedLimits).style.display = "none";
        else
            document.getElementById(chkFarmPollution).checked = true;
        //}
        //else {
        //    if (clientLiability.value == "0") {
        //        if (ConfirmFarmDialog())
        //            document.getElementById(dvFPIncreasedLimits).style.display = "none";
        //        else
        //            document.getElementById(chkFarmPollution).checked = true;
        //    }
        //    else {
        //        alert("Liability Enhancement Endorsement is required for ALL Commercial Policies that have a Coverage L Limit");
        //        document.getElementById(chkFarmPollution).checked = true;
        //    }
        //}
    }
}

// Toggle Extra Expense Data
function ToggleExtraExpense(chkExtaExpense, dvExtraExpenseLimit, txtExtraExpenseLimit) {
    if (document.getElementById(chkExtaExpense).checked) {
        document.getElementById(dvExtraExpenseLimit).style.display = "block";
        document.getElementById(txtExtraExpenseLimit).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvExtraExpenseLimit).style.display = "none";
            $("#" + txtExtraExpenseLimit).val("");
        }
        else
            document.getElementById(chkExtaExpense).checked = true;
    }
}

// Toggle Extra Expense with Included, Increased and Total Limits
function ToggleExtraExpenseWithIncreasedLimits(chkExtraExpense, dvExtraExpenseIncreasedLimits, txtExtraExpenseIncludedLimit, txtExtraExpenseIncreasedLimit, txtExtraExpenseTotalLimit) {
    if (document.getElementById(chkExtraExpense).checked) {
        document.getElementById(chkExtraExpense).disabled = false;
        document.getElementById(dvExtraExpenseIncreasedLimits).style.display = "block";
        document.getElementById(txtExtraExpenseIncludedLimit).value = "";
        document.getElementById(txtExtraExpenseIncreasedLimit).value = "0";
        document.getElementById(txtExtraExpenseTotalLimit).value = "0";
        document.getElementById(txtExtraExpenseIncreasedLimit).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvExtraExpenseIncreasedLimits).style.display = "none";
            document.getElementById(txtExtraExpenseIncludedLimit).value = "";
            document.getElementById(txtExtraExpenseIncreasedLimit).value = "";
            document.getElementById(txtExtraExpenseTotalLimit).value = "";
        }
        else
            document.getElementById(chkExtraExpense).checked = true;
    }
}

//Need to handle enable/disable of Extra Expense here and not in populate code behind, because populate disabling causes issue at save when Farm Extender unchecked (sees it as still checked) after Farm Extender being checked and saved. When Farm Extender checked it auto-checks Extra Expense. When Farm Extender unchecked, it auto-unchecks Extra Expense.
function FarmExtenderEnableDisableExtraExpense(chkFarmExtender, chkExtraExpense) {
    if (document.getElementById(chkFarmExtender).checked) {
        document.getElementById(chkExtraExpense).disabled = true;
    } else {
        document.getElementById(chkExtraExpense).disabled = false;
    }
}

//When Farm Extender selected, require Extra Expense with 5k included limit
function FarmExtenderUpdatesExtraExpense(chkFarmExtender, chkExtraExpense, dvExtraExpenseIncreasedLimits, txtExtraExpenseIncludedLimit, txtExtraExpenseIncreasedLimit, txtExtraExpenseTotalLimit) {
    var origEEIncreasedLimit = document.getElementById(txtExtraExpenseIncreasedLimit).value.replace(",", "");
    var newEEIncreasedLimit = 0;
    if (document.getElementById(chkFarmExtender).checked) {
        if (!isNaN(origEEIncreasedLimit) && parseInt(origEEIncreasedLimit) > 5000) {
            newEEIncreasedLimit = parseInt(origEEIncreasedLimit) - 5000;
        }
        document.getElementById(chkExtraExpense).checked = true;
        document.getElementById(chkExtraExpense).disabled = true;
        document.getElementById(dvExtraExpenseIncreasedLimits).style.display = "block";
        document.getElementById(txtExtraExpenseIncludedLimit).value = "5,000";
        document.getElementById(txtExtraExpenseIncreasedLimit).value = ifm.vr.stringFormating.asNumberWithCommas(newEEIncreasedLimit);
        document.getElementById(txtExtraExpenseTotalLimit).value = ifm.vr.stringFormating.asNumberWithCommas(newEEIncreasedLimit + 5000);
    } else {
        if (ConfirmFarmDialog()) {
            document.getElementById(chkExtraExpense).disabled = false;
            document.getElementById(txtExtraExpenseIncludedLimit).value = "";
            if (!isNaN(origEEIncreasedLimit) && parseInt(origEEIncreasedLimit) > 0) {
                document.getElementById(chkExtraExpense).checked = true;
                newEEIncreasedLimit = parseInt(origEEIncreasedLimit) + 5000;
                document.getElementById(txtExtraExpenseIncreasedLimit).value = newEEIncreasedLimit;
                document.getElementById(txtExtraExpenseTotalLimit).value = ifm.vr.stringFormating.asNumberWithCommas(newEEIncreasedLimit);
            } else {
                document.getElementById(chkExtraExpense).checked = false;
                document.getElementById(dvExtraExpenseIncreasedLimits).style.display = "none";
                document.getElementById(txtExtraExpenseIncreasedLimit).value = "";
                document.getElementById(txtExtraExpenseTotalLimit).value = "";
            }
        } else {
            document.getElementById(chkFarmExtender).checked = true;
        }
    }
}

// Toggle Liability Enhancement Endorsement
function ToggleLiabilityEnhancement(ddlLiability, chkFarmPollution, dvFPIncreasedLimits, ddlLimit, hiddenFarmPollution) {
    var client = document.getElementById(ddlLimit);

    if (document.getElementById(ddlLiability).value != "0") {
        document.getElementById(chkFarmPollution).checked = true;
        document.getElementById(dvFPIncreasedLimits).style.display = "block";
        //$("#" + hiddenFarmPollution).val("True");
        //document.getElementById(chkFarmPollution).disabled = true;
    }
    else {
        document.getElementById(chkFarmPollution).checked = false;
        client.selectedIndex = 0;
        document.getElementById(dvFPIncreasedLimits).style.display = "none";
        //$("#" + hiddenFarmPollution).val("False");
        //document.getElementById(chkFarmPollution).disabled = false;
    }
}

function ConfirmGL9Removal(imExists, rvExists, LastLocationWithGL9) {
    var confirmMsg = "";

    if (LastLocationWithGL9 && imExists && rvExists) {
        if (LastLocationWithGL9 == "True") {
            if (imExists == "True" && rvExists == "True")
                confirmMsg = "Deleting this coverage will also delete all Inland Marine and RV/Watercraft coverages and information on this quote. Are you sure you want to delete this item?"
            if (imExists == "False" && rvExists == "True")
                confirmMsg = "Deleting this coverage will also delete all RV/Watercraft coverages and information on this quote. Are you sure you want to delete this item?"
            if (imExists == "True" && rvExists == "False")
                confirmMsg = "Deleting this coverage will also delete all Inland Marine coverages and information on this quote. Are you sure you want to delete this item?"
            if (imExists == "False" && rvExists == "False")
                confirmMsg = "Are you sure you want to remove all GL9s from this Location?"
        }
        else {
            confirmMsg = "Are you sure you want to remove all GL9s from this Location?"
        }
    }

    if (confirm(confirmMsg))
        return true;
    else
        return false;
}

// Toggle Backup of Sewer or Drain
function ToggleFarmAllStar(chkFarmAllStarClientId, dvBackSewerDrainClientId, ddlBSDLimitClientId, ddlWaterDamageClientId, tblWaterBUWaterDamageClientId, lblBackSewerDrainClientId, trWaterDamageClientId, trWaterDamageLimitCliendId, isWaterDamageAvailable) {
    var chkFarmAllStar = document.getElementById(chkFarmAllStarClientId);
    var dvBackSewerDrain = document.getElementById(dvBackSewerDrainClientId);
    var ddWaterBackup = document.getElementById(ddlBSDLimitClientId);
    var ddWaterDamage = document.getElementById(ddlWaterDamageClientId);
    var tblWaterBUWaterDamage = document.getElementById(tblWaterBUWaterDamageClientId);
    var lblWaterBackup = document.getElementById(lblBackSewerDrainClientId);
    var trWaterDamage = document.getElementById(trWaterDamageClientId);
    var trWaterDamageLimit = document.getElementById(trWaterDamageLimitCliendId);

    if (chkFarmAllStar.checked) {
        dvBackSewerDrain.style.display = "block";
        if (isWaterDamageAvailable == "True") {
            lblWaterBackup.innerText = "Water Backup"
            trWaterDamage.style.display = "";
            trWaterDamageLimit.style.display = "";
            tblWaterBUWaterDamage.removeAttribute('style');
        } else {
            lblWaterBackup.innerText = "Backup of Sewer or Drain"
            trWaterDamage.style.display = "none";
            trWaterDamageLimit.style.display = "none";
            tblWaterBUWaterDamage.style.width = "100%";
        }
        ddWaterBackup.selectedIndex = 0;
        ddWaterDamage.selectedIndex = 0;
    }
    else {
        if (ConfirmFarmDialog()) {
            dvBackSewerDrain.style.display = "none";
            ddWaterBackup.selectedIndex = 0;
            ddWaterDamage.selectedIndex = 0;
        }
        else
            chkFarmAllStar.checked = true;
    }
}

// Toggle Checkbox Only
function ToggleCheckboxOnly(chkControl) {
    if (document.getElementById(chkControl).checked) {
        ;
    }
    else {
        if (ConfirmFarmDialog()) {
            ;
        }
        else {
            document.getElementById(chkControl).checked = true;
        }
    }
}

// Confirm Personal Property Delete Dialog Box
function ConfirmPersPropDialog(chkControl, peakSeason, propType) {
    var deleteMsg = "";
    if (peakSeason != "False") {
        if (document.getElementById(peakSeason).value == "true") {
            if (document.getElementById(propType).value != 0) {
                var controlSplt = chkControl.split("_");
                var controlName = "";

                switch (controlSplt[controlSplt.length - 6]) {
                    case "ctlLivestock":
                        controlName = "Livestock";
                        break;
                    case "ctlGrainBuilding":
                        controlName = "Grain in Buildings";
                        break;
                    case "ctlGrainOpen":
                        controlName = "Grain in the Open";
                        break;
                    case "ctlHayBuilding":
                        controlName = "Hay in Buildings";
                        break;
                    case "ctlHayOpen":
                        controlName = "Hay in the Open";
                        break;
                }

                deleteMsg = "This will remove " + controlName + " coverage and associated Peak Season coverage";
            }
            else
                deleteMsg = "This will remove Blanket coverage and associated Peak Season coverage";
        }
        else
            deleteMsg = "Are you sure you want to delete this item?";
    }
    else
        deleteMsg = "Are you sure you want to delete this item?";

    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";
    if (confirm(deleteMsg))
        confirmValue.value = "Yes";
    else
        confirmValue.value = "No";

    document.forms[0].appendChild(confirmValue);
}

// Confirm Delete Dialog Box
function ConfirmDialog(section) {
    var confirmValue = document.createElement("INPUT");
    confirmValue.type = "hidden";
    confirmValue.name = "confirmValue";

    switch (section) {
        case "Location":
            if (confirm("Are you sure you want to delete this Location and all associated Dwellings and Buildings?"))
                confirmValue.value = "Yes";
            else
                confirmValue.value = "No";
            break;
        case "Residence":
            if (confirm("Are you sure you want to delete this Dwelling?"))
                confirmValue.value = "Yes";
            else
                confirmValue.value = "No";
            break;
        default:
            if (confirm("Are you sure you want to delete this item?"))
                confirmValue.value = "Yes";
            else
                confirmValue.value = "No";
            break;
    }

    document.forms[0].appendChild(confirmValue);
}

// Set NULL value to blank
function noNAN(value) {
    return isNaN(value) ? "0" : value;
}

//Updated 10/31/18 for multi state - added dvMineSubsidence, chkMineSubsidence, dvMineSubsidenceReqHelpInfo, dvMineSubsidenceNotReqHelpInfo, ddState, txtCounty, hiddenMineSubIsChecked, hiddenMineSubIsEnabled
// Toggle Coverages
function ToggleCoverageByForm(selectedForm, txtDwLimit, txtDwellingChgInLimit, txtDwellingTotalLimit, txtRPSLimit, txtRPSChgInLimit, txtRPSTotalLimit,
    txtPPLimit, txtPPChgInLimit, txtPPTotalLimit, txtLossLimit, txtLossChgInLimit, txtLossTotalLimit, chkReplaceCost,
    hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal, hiddenCovCLimit, hiddenCovCChg, hiddenCovCTotal,
    hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal, dvCovA, dvCovB, lblPPReq, hiddenProgramType, hiddenFormType, dvExpandReplace, chkExpandReplace,
    hiddenBuiltYear, chkACV, dvReplacementCC, dvReplaceCost, dvReplaceCostIncl, dvReplacement, chkReplacement, chkRPS, ddlStructure, isVerisk360Enabled) {

    //var TCBF_txtDWLimit = "";

    //if (ifm.vr.currentQuote.isEndorsement) {
    //    TCBF_txtDWLimit = $("#" + txtDwLimit).val();
    //}


    $("#" + hiddenProgramType).val(selectedForm.value);
    $("#" + hiddenFormType).val(selectedForm.value);

    // Set Limits
    $("#" + txtDwLimit).val("0");
    $("#" + txtRPSLimit).val("0");
    $("#" + txtPPLimit).val("0");
    $("#" + txtLossLimit).val("0");
    $("#" + hiddenCovALimit).val("0");
    $("#" + hiddenCovBLimit).val("0");
    $("#" + hiddenCovCLimit).val("0");
    $("#" + hiddenCovDLimit).val("0");

    // Add Increase Limit
    $("#" + txtDwellingChgInLimit).val("0");
    $("#" + txtRPSChgInLimit).val("0");
    $("#" + txtPPChgInLimit).val("0");
    $("#" + txtLossChgInLimit).val("0");
    $("#" + hiddenCovBChg).val("0");
    $("#" + hiddenCovCChg).val("0");
    $("#" + hiddenCovDChg).val("0");

    // Update Totals
    $("#" + txtDwellingTotalLimit).val("0");
    $("#" + txtRPSTotalLimit).val("0");
    $("#" + txtPPTotalLimit).val("0");
    $("#" + txtLossTotalLimit).val("0");
    $("#" + hiddenCovATotal).val("0");
    $("#" + hiddenCovBTotal).val("0");
    $("#" + hiddenCovCTotal).val("0");
    $("#" + hiddenCovDTotal).val("0");

    document.getElementById(chkReplaceCost).disabled = false;
    document.getElementById(chkReplaceCost).checked = true;
    document.getElementById(dvReplaceCost).style.display = "block";
    document.getElementById(dvReplaceCostIncl).style.display = "none";

    if (selectedForm.value == "17") {
        document.getElementById(dvCovA).style.display = "none";
        document.getElementById(dvCovB).style.display = "none";
        document.getElementById(dvReplacementCC).style.display = "none";

        // Set Limits
        $("#" + txtPPLimit).val("15,000");
        $("#" + txtLossLimit).val("6,000");
        $("#" + hiddenCovCLimit).val("15,000");
        $("#" + hiddenCovDLimit).val("6,000");

        // Update Totals
        $("#" + txtPPTotalLimit).val("15,000");
        $("#" + txtLossTotalLimit).val("6,000");
        $("#" + hiddenCovCTotal).val("15,000");
        $("#" + hiddenCovDTotal).val("6,000");

        document.getElementById(dvReplacement).style.display = "none";
        document.getElementById(chkReplacement).checked = false;

        document.getElementById(chkExpandReplace).disabled = true;
        document.getElementById(chkExpandReplace).checked = false;

        ////Added 10/17/18 for multi state MLW
        ////only hide dwelling mine sub coverage for FO-4, building mine sub coverage stays
        //document.getElementById(dvMineSubsidence).style.display = "none"; 
        //document.getElementById(chkMineSubsidence).checked = false;
        //document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
        //document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
        //document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
        //document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
        //document.getElementById(hiddenMineSubIsChecked).text = "False"
    }
    else {
        document.getElementById(dvReplacementCC).style.display = "block";

        if (parseInt(document.getElementById(hiddenBuiltYear).value) >= 1947) {
            document.getElementById(dvReplacement).style.display = "none";
            document.getElementById(chkReplacement).checked = false;
        }
        else
            document.getElementById(dvReplacement).style.display = "block";



        if (selectedForm.value == "16" || selectedForm.value == "18") {
            var ErDate = new Date();
            // if (parseInt(document.getElementById(hiddenBuiltYear).value) < 1951) {
            if (parseInt(document.getElementById(hiddenBuiltYear).value) < ErDate.getFullYear() - 40) {
                document.getElementById(chkExpandReplace).disabled = true;
                document.getElementById(chkExpandReplace).checked = false;
            }
            else {
                if (document.getElementById(chkACV).checked) {
                    document.getElementById(chkExpandReplace).disabled = true;
                    document.getElementById(chkExpandReplace).checked = false;
                }
                else
                    document.getElementById(chkExpandReplace).disabled = false;
            }

            // FO-5
            if (selectedForm.value == "18") {
                document.getElementById(dvReplaceCost).style.display = "none";
                document.getElementById(chkReplaceCost).checked = false;
                document.getElementById(dvReplaceCostIncl).style.display = "block";
            }
        }
        else {
            if (selectedForm.value == "17") {
                document.getElementById(dvReplacementCC).style.display = "none";
                document.getElementById(dvReplacement).style.display = "none";
                document.getElementById(chkReplacement).checked = false;
            }
            else {
                document.getElementById(dvReplacementCC).style.display = "block";
            }

            document.getElementById(chkExpandReplace).disabled = true;
            document.getElementById(chkExpandReplace).checked = false;
        }

        // Verisk 360 - hide 
        var selectedStructure = document.getElementById(ddlStructure);
        if (isVerisk360Enabled == "True" && (selectedStructure.value == "8" || selectedStructure.value == "2")) {
            document.getElementById(dvReplacementCC).style.display = "none";
        }

        document.getElementById(dvCovA).style.display = "block";
        document.getElementById(dvCovB).style.display = "block";
        $("#" + lblPPReq).text("");
        document.getElementById(txtPPLimit).disabled = true;
        document.getElementById(txtPPLimit).style.backgroundColor = "LightGray";
        document.getElementById(txtPPLimit).style.color = "Gray";
        document.getElementById(txtPPChgInLimit).disabled = false;
        document.getElementById(txtPPChgInLimit).style.backgroundColor = "White";
        document.getElementById(txtPPChgInLimit).style.color = "Black";

        //Added 10/31/18 for multi state MLW
        //switch (ddState) {
        //    case "IL", "15":               
        //        //if IL show checkbox regardless of county
        //        document.getElementById(dvMineSubsidence).style.display = "block";
        //        //for IL only - show checkboxes checked/unchecked accordingly
        //        ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateAbbreviation('IL', function (data) {
        //            if (data.contains(txtCounty)) {
        //                document.getElementById(chkMineSubsidence).checked = true;
        //                document.getElementById(chkMineSubsidence).disabled = true;
        //                document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "block";
        //                document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
        //                document.getElementById(chkMineSubsidence).className = "chkMine_IL_Reqd";
        //                document.getElementById(dvMineSubsidenceReqHelpInfo).className = "dvInfo_IL_Reqd informationalText";
        //                document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
        //                document.getElementById(hiddenMineSubIsChecked).text = "True"
        //            } else {
        //                if (document.getElementById(hiddenMineSubIsChecked).value == "True") {
        //                    document.getElementById(chkMineSubsidence).checked = true;
        //                    if (document.getElementById(hiddenMineSubIsEnabled).value == "True") {
        //                        document.getElementById(chkMineSubsidence).disabled = false;
        //                    } else {
        //                        document.getElementById(chkMineSubsidence).disabled = true;
        //                    }
        //                    document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
        //                    document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "block";
        //                } else {
        //                    document.getElementById(chkMineSubsidence).checked = false;
        //                    document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
        //                    document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
        //                }
        //                document.getElementById(chkMineSubsidence).className = "chkMine_IL_Enabled_NotReqd";
        //                document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
        //                document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "dvInfo_IL_NotReqd informationalText";
        //            }
        //        });
        //        //}
        //        break;
        //    case "IN", "16":
        //        //Added 10/31/18 for bug 29623 MLW
        //        //if mine county, then show mine sub checkbox. Otherwise, do not show
        //        ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateAbbreviation('IN', function (data) {
        //            if (data.contains(txtCounty)) {
        //                document.getElementById(dvMineSubsidence).style.display = "block";
        //            } else {
        //                document.getElementById(dvMineSubsidence).style.display = "none";
        //                document.getElementById(chkMineSubsidence).checked = false;
        //                document.getElementById(dvMineSubsidenceReqHelpInfo).style.display = "none";
        //                document.getElementById(dvMineSubsidenceNotReqHelpInfo).style.display = "none";
        //            }
        //            document.getElementById(chkMineSubsidence).className = "chkMine_IN";
        //            document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
        //            document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
        //        });
        //        break;
        //    default:
        //        //not yet supported, do not show mine sub checkbox, do not show info texts req & not
        //        document.getElementById(dvMineSubsidence).style.display = "none";
        //        document.getElementById(chkMineSubsidence).checked = false;
        //        document.getElementById(chkMineSubsidence).className = "chkMine_IN";
        //        document.getElementById(dvMineSubsidenceReqHelpInfo).className = "informationalText";
        //        document.getElementById(dvMineSubsidenceNotReqHelpInfo).className = "informationalText";
        //        break;
        //} 
    }

    //if (ifm.vr.currentQuote.isEndorsement) {
    //    $("#" + txtDwLimit).val(TCBF_txtDWLimit)
    //    CalculateFromDwellingLimit(TCBF_txtDWLimit, txtDwellingTotalLimit, txtRPSLimit, txtRPSChgInLimit, txtRPSTotalLimit,
    //        txtPPLimit, txtPPChgInLimit, txtPPTotalLimit, txtLossLimit, txtLossChgInLimit, txtLossTotalLimit, chkReplaceCost,
    //        hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal, hiddenCovCLimit, hiddenCovCChg, hiddenCovCTotal,
    //        hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal, hiddenFormType, chkRPS);
    //    return;
    //}
}



function ToggleEquipBreak(selectedForm, chkEquipBreak) {
    if (selectedForm.value == "17" && ifm.vr.currentQuote.isEndorsement == "false") {
        if (chkEquipBreak == "True") {
            alert("Equipment Breakdown not allowed on F04. Coverage has been Removed");
            chkEquipBreak = "False"
        }
    }
}

// Calculate coverages From Dwelling Limit
function CalculateFromDwellingLimit(txtDwLimit, txtDwellingTotalLimit, txtRPSLimit, txtRPSChgInLimit, txtRPSTotalLimit,
    txtPPLimit, txtPPChgInLimit, txtPPTotalLimit, txtLossLimit, txtLossChgInLimit, txtLossTotalLimit, chkReplaceCost,
    hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal, hiddenCovCLimit, hiddenCovCChg, hiddenCovCTotal,
    hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal, hiddenFormType, chkRPS) {
    var formType = document.getElementById(hiddenFormType).value;

    // Calculate Limits from Coverage A
    var covALimit = RoundValue(parseInt(document.getElementById(txtDwLimit).value.replace(/,/g, "")));
    var covCChange = RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));
    isNaN(covALimit) ? covALimit = 0 : covALimit;
    isNaN(covCChange) ? covCChange = 0 : covCChange;
    var calcCovBLimit = parseInt(covALimit) * .10;
    var calcCovCLimit = RoundValue(parseInt(covALimit) * .50);
    var calcCovCChg = 0;
    var calcCovDLimit = 0;

    if (formType == "17") {
        //var covCLimit = RoundValue(parseInt(document.getElementById(txtPPLimit).value.replace(/,/g, "")));
        //calcCovCLimit = covCLimit;
        //calcCovDLimit = parseInt(covCLimit) * .40;

        // Modified Code
        calcCovCLimit = 15000;
        calcCovCChg = isNaN(parseInt(document.getElementById(txtPPChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));
        calcCovDLimit = (calcCovCLimit + calcCovCChg) * .40;
    }
    else {
        if (formType == "18") {
            calcCovCLimit = RoundFarmValue100(parseInt(covALimit) * .70);
            calcCovCChg = isNaN(parseInt(document.getElementById(txtPPChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));
        }
        else {
            if (document.getElementById(chkReplaceCost).checked) {
                if (document.getElementById(txtPPChgInLimit).value.replace(",", "") > (parseInt(covALimit) * .20))
                    calcCovCChg = isNaN(parseInt(document.getElementById(txtPPChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));
                else
                    calcCovCChg = parseInt(covALimit) * .20;
            }
            else {
                calcCovCChg = isNaN(parseInt(document.getElementById(txtPPChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtPPChgInLimit).value.replace(/,/g, "")));
            }
        }

        calcCovDLimit = parseInt(covALimit) * .20;
    }

    var covBChg = isNaN(parseInt(document.getElementById(txtRPSChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtRPSChgInLimit).value.replace(/,/g, "")));

    if (covBChg < 0) {
        alert("Must be a positive number");
        covBChg = 0;
        document.getElementById(txtRPSChgInLimit).focus();
    }

    var covCChg = RoundValue(calcCovCChg);
    var covDChg = isNaN(parseInt(document.getElementById(txtLossChgInLimit).value)) ? 0 : RoundValue(parseInt(document.getElementById(txtLossChgInLimit).value.replace(/,/g, "")));

    if (covDChg < 0) {
        alert("Must be a positive number");
        covDChg = 0;
        document.getElementById(txtLossChgInLimit).focus();
    }

    if (covBChg == 0)
        document.getElementById(chkRPS).checked = false
    else
        document.getElementById(chkRPS).checked = true

    //var calcCovBTotal = calcCovBLimit + covBChg;
    var calcCovBTotal = calcCovBLimit;
    //alert(covCChg);
    //alert(calcCovCLimit);
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

// Calculate new Related Private Structures from Change
function CalculateFromRPSChange(rpsLimit, rpsChange, rpsTotal, hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal, hiddenCovCLimit, hiddenCovCChg, hiddenCovCTotal,
    hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal) {
    // Adjust Related Private Structures total from Coverage B Change
    var covBLimit = document.getElementById(rpsLimit).value.replace(/,/g, "");
    var covBChange = document.getElementById(rpsChange).value.replace(/,/g, "");

    if (parseInt(covBChange) < 0) {
        alert("Total cannot be less than limit");
        covBChange = "0";
        document.getElementById(rpsChange).focus();
    }

    // Calculate new Related Private Structures
    //var calcCovBTotal = parseInt(covBLimit) + parseInt(noNAN(covBChange) == "" ? "0" : covBChange);
    var calcCovBTotal = parseInt(covBLimit) == "" ? "0" : covBLimit;

    // Set Change
    $("#" + rpsChange).val(covBChange.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovBChg).val(covBChange.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    // Update Total
    $("#" + rpsTotal).val(calcCovBTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovBTotal).val(calcCovBTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

    // Set hidden values for unchanged values
    $("#" + hiddenCovALimit).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovBLimit).val(calcCovBLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovCLimit).val(calcCovCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#" + hiddenCovDLimit).val(calcCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
}

// Modify Coverage C Limit when Replacement Cost is toggled
function CalculateCovCReplaceCost(chkReplaceCost, txtDwLimit, txtPPLimit, txtPPChange, txtPPTotal,
    hiddenCovALimit, hiddenCovATotal, hiddenCovBLimit, hiddenCovBChg, hiddenCovBTotal,
    hiddenCovCLimit, hiddenCovCChg, hiddenCovCTotal, hiddenCovDLimit, hiddenCovDChg, hiddenCovDTotal,
    hiddenFormType) {
    var covALimit = document.getElementById(txtDwLimit).value.replace(/,/g, "");
    var hidCovBLimit = document.getElementById(hiddenCovBLimit).value.replace(/,/g, "");
    var hidCovBChg = document.getElementById(hiddenCovBChg).value.replace(/,/g, "");
    var hidCovBTotal = document.getElementById(hiddenCovBTotal).value.replace(/,/g, "");
    var calcCovCLimit = parseInt(covALimit) * .50;
    var calcCovCChg = "0";
    var covCChange = document.getElementById(txtPPChange).value.replace(/,/g, "");

    if (document.getElementById(hiddenFormType).value != "FO-4") {
        if (covALimit != "0") {
            if (!document.getElementById(chkReplaceCost).checked) {
                if (ConfirmFarmDialog())
                    calcCovCChg = covCChange;
                else {
                    document.getElementById(chkReplaceCost).checked = true;
                    calcCovCChg = covCChange;
                }
            }
            else {
                calcCovCChg = parseInt(covALimit) * .20;

                if (covCChange >= calcCovCChg)
                    calcCovCChg = covCChange
                else
                    alert("Coverage C Limit has been increased to 70% of Coverage A");
            }
        }

        // Calculate new Related Private Structures
        //var calcCovCTotal = parseInt(calcCovCLimit) + parseInt(noNAN(covCChange) == "" ? "0" : covCChange);
        var calcCovCTotal = parseInt(calcCovCLimit) + parseInt(noNAN(calcCovCChg) == "" ? "0" : calcCovCChg);

        var hidCovDLimit = document.getElementById(hiddenCovDLimit).value.replace(/,/g, "");
        var hidCovDChg = document.getElementById(hiddenCovDChg).value.replace(/,/g, "");
        var hidCovDTotal = document.getElementById(hiddenCovDTotal).value.replace(/,/g, "");

        $("#" + txtPPLimit).val(calcCovCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovCLimit).val(calcCovCLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        $("#" + txtPPChange).val(calcCovCChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovCChg).val(calcCovCChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        $("#" + txtPPTotal).val(calcCovCTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovCTotal).val(calcCovCTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        // Populate hidden values not changed
        $("#" + hiddenCovALimit).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovATotal).val(covALimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovBLimit).val(hidCovBLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovBChg).val(hidCovBChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovBTotal).val(hidCovBTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovDLimit).val(hidCovDLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovDChg).val(hidCovDChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $("#" + hiddenCovDTotal).val(hidCovDTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }
}

// Toggle Theft of Building Materials
function ToggleTheft(chkTheft, dvTheftLimit, txtTheftLimit) {
    if (document.getElementById(chkTheft).checked) {
        document.getElementById(dvTheftLimit).style.display = "block";
        document.getElementById(txtTheftLimit).focus();
        $("#" + txtTheftLimit).val("1,000");
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvTheftLimit).style.display = "none";
            $("#" + txtTheftLimit).val("");
        }
        else
            document.getElementById(chkTheft).checked = true;
    }
}

// Toggle Expanded Replacement Cost Terms
function ToggleExtendReplace(chkACV, dvExpandReplace, chkExpandReplace, hiddenFormType, hiddenYearBuilt) {
    if (chkACV.checked) {
        document.getElementById(dvExpandReplace).disabled = true;
        document.getElementById(chkExpandReplace).checked = false;
    }
    else {
        if (ConfirmFarmDialog()) {
            if (document.getElementById(hiddenFormType).value == "16" || document.getElementById(hiddenFormType).value == "18") {
                var ErDate = new Date();
                if (parseInt(document.getElementById(hiddenYearBuilt).value) < ErDate.getFullYear() - 40) {
                    document.getElementById(dvExpandReplace).disabled = true;
                    document.getElementById(chkExpandReplace).checked = false;
                }
                else
                    document.getElementById(dvExpandReplace).disabled = false;
            }
            else {
                document.getElementById(dvExpandReplace).disabled = true;
                document.getElementById(chkExpandReplace).checked = false;
            }
        }
        else
            chkACV.checked = true;
    }
}

function ToggleExpandReplaceByYear(txtYearBuilt, hiddenFormType, dvExpandReplace, chkExpandReplace, chkACV, hiddenYearBuilt, dvReplacement, chkReplacement) {
    document.getElementById(hiddenYearBuilt).value = txtYearBuilt.value

    if (document.getElementById(hiddenFormType).value == "16" || document.getElementById(hiddenFormType).value == "18") {
        var ErDate = new Date();
        if (parseInt(txtYearBuilt.value) < ErDate.getFullYear() - 40) {
            document.getElementById(chkExpandReplace).disabled = true;
            document.getElementById(chkExpandReplace).checked = false;
        }
        else {
            if (document.getElementById(chkACV).checked) {
                document.getElementById(chkExpandReplace).disabled = true;
                document.getElementById(chkExpandReplace).checked = false;
            }
            else
                document.getElementById(chkExpandReplace).disabled = false;
        }
    }

    if (document.getElementById(hiddenFormType).value != "17") {
        if (parseInt(txtYearBuilt.value) >= 1947) {
            document.getElementById(dvReplacement).style.display = "none";
            document.getElementById(chkReplacement).checked = false;
        }
        else
            document.getElementById(dvReplacement).style.display = "block";
    }
    else {
        document.getElementById(dvReplacement).style.display = "none";
        document.getElementById(chkReplacement).checked = false;
    }
}

function ToggleExpandReplaceByChkbox(txtYearBuilt, hiddenFormType, dvExpandReplace, chkExpandReplace, chkACV, hiddenYearBuilt, dvReplacement, chkReplacement) {
    var BuildYear = document.getElementById(hiddenYearBuilt).value

    if (document.getElementById(hiddenFormType).value == "16" || document.getElementById(hiddenFormType).value == "18") {
        var ErDate = new Date();
        if (parseInt(BuildYear) < ErDate.getFullYear() - 40) {
            document.getElementById(chkExpandReplace).disabled = true;
            document.getElementById(chkExpandReplace).checked = false;
        }
        else {
            if (document.getElementById(chkACV).checked) {
                document.getElementById(chkExpandReplace).disabled = true;
                document.getElementById(chkExpandReplace).checked = false;
            }
            else
                document.getElementById(chkExpandReplace).disabled = false;
        }
    }

    if (document.getElementById(hiddenFormType).value != "17") {
        if (parseInt(txtYearBuilt.value) >= 1947) {
            document.getElementById(dvReplacement).style.display = "none";
            document.getElementById(chkReplacement).checked = false;
        }
        else
            document.getElementById(dvReplacement).style.display = "block";
    }
    else {
        document.getElementById(dvReplacement).style.display = "none";
        document.getElementById(chkReplacement).checked = false;
    }
}

// Toggle Functional Replacement Cost Loss Settlement
function ToggleFuncReplaceCost(chkACV, dvReplacement, chkReplacement, DwellingClassification) {
    var TypeOneClassification = "22"; // Type 1 Dwelling Classification code
    document.getElementById(chkReplacement).disabled = true;
    if (chkACV.checked) {
        document.getElementById(chkReplacement).checked = false;
    }
    else {
        document.getElementById(chkReplacement).disabled = false;
    }
}

//
// Round a number up by 1,000
//
function RoundValue(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
        var returnNum = 0;

        if (number < 0) {
            number = number * -1;
            returnNum = Math.ceil(number / 5) * 5;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(number / 5) * 5;
        }

        return returnNum;
    }
    else {

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
}

function RoundFarmValue100(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
        var returnNum = 0;

        if (number < 0) {
            number = number * -1;
            returnNum = Math.ceil(number / 5) * 5;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(number / 5) * 5;
        }

        return returnNum;
    }
    else {
        var returnNum = 0;

        if (number < 0) {
            number = number * -1;
            returnNum = Math.ceil(number / 100) * 100;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(number / 100) * 100;
        }

        return returnNum;

    }

}

//
// Round a number up by 1,000
//
function FarmRoundValue(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
        var numberInt = parseInt($("#" + number).val()).toString();
        var returnNum = 0;

        if (numberInt < 0) {
            numberInt = numberInt * -1;
            returnNum = Math.ceil(numberInt / 5) * 5;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(numberInt / 5) * 5;
        }

        $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
    }
    else {
        var numberInt = parseInt($("#" + number).val()).toString();
        var returnNum = 0;

        if (numberInt < 0) {
            numberInt = numberInt * -1;
            returnNum = Math.ceil(numberInt / 1000) * 1000;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(numberInt / 1000) * 1000;
        }

        $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))

    }
}

//
// Round a number up by 100
//
function FarmRoundValue100(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
        var numberInt = $("#" + number).val().replace(",", "");
        var returnNum = 0;

        if (numberInt < 0) {
            numberInt = numberInt * -1;
            returnNum = Math.ceil(numberInt / 5) * 5;

            returnNum = returnNum * -1;
        }
        else {
            returnNum = Math.ceil(numberInt / 5) * 5;
        }

        $("#" + number).val(noNAN(returnNum).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
    }
    else {
        var numberInt = $("#" + number).val().replace(",", "");
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

}

// Round a number to nearest whole number
function RoundToNearestNumber(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        var numberDec = parseFloat($("#" + number).val()).toString();
        var numberInt = Math.ceil(numberDec / 5) * 5;

        $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
    }
    else {
        var numberDec = parseFloat($("#" + number).val()).toString();
        var numberInt = Math.round(numberDec);

        $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
    }
    //if (ifm.vr.currentQuote.isEndorsement) {
    //    var numberInt = $("#" + number).val().replace(",", "");
    //    $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
    //}
    //else {
    //    var numberDec = parseFloat($("#" + number).val()).toString();
    //    var numberInt = Math.round(numberDec);

    //    $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
    //}
}

//Added 4/12/2022 for bug 74250 MLW
//Round to nearest whole number, even for endorsements - i.e. number of employees shouldn't round from 1 to 5 on endorsements
function AlwaysRoundToNearestNumber(number) {
    var numberDec = parseFloat($("#" + number).val()).toString();
    var numberInt = Math.round(numberDec);

    $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))
}

// Round a number up to next whole number
function RoundToNextNumber(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        var numberDec = parseFloat($("#" + number).val()).toString();
        var numberInt = Math.ceil(numberDec / 5) * 5;

        $("#" + number).val(noNAN(numberInt).toString())
    }
    else {
        var numberDec = parseFloat($("#" + number).val()).toString();
        var numberInt = Math.ceil(numberDec);

        $("#" + number).val(noNAN(numberInt).toString())
    }

    //if (ifm.vr.currentQuote.isEndorsement) {
    //    var numberInt = $("#" + number).val().replace(",", "");
    //    $("#" + number).val(noNAN(numberInt).toString())
    //}
    //else {
    //    var numberDec = parseFloat($("#" + number).val()).toString();
    //    var numberInt = Math.ceil(numberDec);

    //    $("#" + number).val(noNAN(numberInt).toString())

    //}

}

// Round a number up to next whole number 09/20/2020 BD used for for both NB and Endorsement
function AlwaysRoundToNextNumber(number) {

    var numberDec = parseFloat($("#" + number).val()).toString();
    var numberInt = Math.ceil(numberDec);

    $("#" + number).val(noNAN(numberInt).toString())
}
function TxtAcerageChange(txtAcerage, divBlanketAcreage, chkBlanketAcreage, divtxtTotalBlanketAcreage) {
    AlwaysRoundToNextNumber(txtAcerage)
    if (parseInt($("#" + txtAcerage).val()) > 0) {
        $("#" + divBlanketAcreage).show();
    }
    else {
        $("#" + divBlanketAcreage).hide();
    }

    if ($("#" + chkBlanketAcreage).prop('checked')) {
        $("#" + divtxtTotalBlanketAcreage).show();
    }
    else {
        $("#" + divtxtTotalBlanketAcreage).hide();
    }
    return;
}

function ToggleBlanketAcreageTxtbox(chkBlanketAcreage, divtxtTotalBlanketAcreage) {
    if ($("#" + chkBlanketAcreage).prop('checked')) {
        $("#" + divtxtTotalBlanketAcreage).show();
    }
    else {
        if (ConfirmFarmDialog()) {
            $("#" + divtxtTotalBlanketAcreage).hide();
        }
        else
            $("#" + chkBlanketAcreage).prop('checked', true);
    }
    return;
}


function ToggleAcreageOnly(chkAcreageOnly, lblMainHeader, locationIndex, streetNum, streetName, city, section, township, range) {
    if (chkAcreageOnly.checked)
        $("#" + lblMainHeader).text(String.format("Acreage Only LOCATION #{0} - {1} {2}, {3}", parseInt(locationIndex) + 1, streetNum, streetName, city));
    else
        $("#" + lblMainHeader).text(String.format("LOCATION #{0} - {1} {2}, {3} S/T/R: {4}/{5}/{6}", parseInt(locationIndex) + 1, streetNum, streetName, city, section, township, range));
}

// Round a number up to a whole number greater than 1,000
function RoundToNearestNumberGT1000(number) {
    if (ifm.vr.currentQuote.isEndorsement) {
        var numberDec = parseFloat($("#" + number).val()).toString();
        var numberInt = Math.ceil(numberDec / 5) * 5;

        $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))

        return;
    }
    else {
        var numberDec = parseFloat($("#" + number).val()).toString();
        var numberInt = Math.round(numberDec);

        if (numberInt < 1000 || $("#" + number).val() == "")
            numberInt = 1000

        $("#" + number).val(noNAN(numberInt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","))

    }

}

function ToggleAcreageOnly(chkAcreageOnly, lblMainHeader, locationIndex, streetNum, streetName, city, section, township, range) {
    if (chkAcreageOnly.checked)
        $("#" + lblMainHeader).text(String.format("Acreage Only LOCATION #{0} - {1} {2}, {3}", parseInt(locationIndex) + 1, streetNum, streetName, city));
    else
        $("#" + lblMainHeader).text(String.format("LOCATION #{0} - {1} {2}, {3} S/T/R: {4}/{5}/{6}", parseInt(locationIndex) + 1, streetNum, streetName, city, section, township, range));
}

//
// IRPM Calculations
//
function CalculateTotalIRPM(txtSupportingBusiness, txtCareCondition, txtDamage, txtConcentration, txtLocation,
    txtMisc, txtRoof, txtStruct, txtPastLosses, txtRiceHulls, txtPoultry, lblTotalValue, btnSaveRate) {
    var sb = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtSupportingBusiness).value)));
    var cc = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtCareCondition).value)));
    var damage = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtDamage).value)));
    var concentration = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtConcentration).value)));
    var location = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtLocation).value)));
    var misc = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtMisc).value)));
    var roof = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtRoof).value)));
    var struct = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtStruct).value)));
    var pl = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtPastLosses).value)));
    var rh = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtRiceHulls).value)));
    var poultry = parseFloat(noNANZero(FormatAsNumberWithOneDecimal(document.getElementById(txtPoultry).value)));

    if (irpmRange(sb, txtSupportingBusiness))
        $("#" + txtSupportingBusiness).val(noNANZero(sb));
    else
        $("#" + txtSupportingBusiness).val("0");

    if (irpmRange(cc, txtCareCondition))
        $("#" + txtCareCondition).val(noNANZero(cc));
    else
        $("#" + txtCareCondition).val("0");

    if (irpmRange(damage, txtDamage))
        $("#" + txtDamage).val(noNANZero(damage));
    else
        $("#" + txtDamage).val("0");

    if (irpmRange(concentration, txtConcentration))
        $("#" + txtConcentration).val(noNANZero(concentration));
    else
        $("#" + txtConcentration).val("0");

    if (irpmRange(location, txtLocation))
        $("#" + txtLocation).val(noNANZero(location));
    else
        $("#" + txtLocation).val("0");

    if (irpmRange(misc, txtMisc))
        $("#" + txtMisc).val(noNANZero(misc));
    else
        $("#" + txtMisc).val("0");

    if (irpmRange(roof, txtRoof))
        $("#" + txtRoof).val(noNANZero(roof));
    else
        $("#" + txtRoof).val("0");

    if (irpmRange(struct, txtStruct))
        $("#" + txtStruct).val(noNANZero(struct));
    else
        $("#" + txtStruct).val("0");

    if (irpmRange(pl, txtPastLosses))
        $("#" + txtPastLosses).val(noNANZero(pl));
    else
        $("#" + txtPastLosses).val("0");

    if (irpmRange(rh, txtRiceHulls))
        $("#" + txtRiceHulls).val(noNANZero(rh));
    else
        $("#" + txtRiceHulls).val("0");

    if (irpmRange(poultry, txtPoultry))
        $("#" + txtPoultry).val(noNANZero(poultry));
    else
        $("#" + txtPoultry).val("0");

    var irpmTotal = sb + cc + damage + concentration + location + misc + roof + struct + pl + rh + poultry

    var irpmtotalrounded = parseFloat(irpmTotal).toFixed(1);

    if (parseInt(irpmtotalrounded) > -16 && parseInt(irpmtotalrounded) < 16) {
        document.getElementById(lblTotalValue).style.color = "Black";
        document.getElementById(btnSaveRate).disabled = false;
    }
    else {
        document.getElementById(lblTotalValue).style.color = "Red";
        //document.getElementById(btnSaveRate).disabled = true;
        //updated 10/8/2020 w/ Interoperability project
        document.getElementById(btnSaveRate).disabled = false;
    }

    $("#" + lblTotalValue).text(irpmtotalrounded.toString());
}

// Set NULL value to 0
function noNANZero(value) {
    return (!value || /^\s*$/.test(value) || value.length == 0) ? "0" : value;
}

// Check Number Range
function irpmRange(number, control) {
    var returnValue = false;

    if (parseInt(number) > -6 && parseInt(number) < 6) {
        returnValue = true;
    }
    else {
        alert("Number must be a whole number between -5 and 5");
        document.getElementById(control).focus();
    }

    return returnValue;
}

function FormatAsNumberWithOneDecimal(input) {
    // Remove non-numeric characters
    //let value = input.value.replace(/[^0-9.]/g, '');

    // If there are more than one decimal point, keep only the first one
    const decimalIndex = input.indexOf('.');
    if (decimalIndex !== -1) {
        input = input.slice(0, decimalIndex + 2);
    }

    // Limit to one decimal place
    const parts = input.split('.');
    if (parts[1] && parts[1].length > 1) {
        input = `${parts[0]}.${parts[1].charAt(0)}`;
    }

    // return input value
    return input;
}

//
// Location-Buildings Coverage Limit Toggle
//
function ToggleBldngCovLimit(checkBox, dvLimitControl, limitControl) {
    var client = document.getElementById(limitControl);

    if (document.getElementById(checkBox).checked) {
        document.getElementById(dvLimitControl).style.display = "block";
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvLimitControl).style.display = "none";
        }
        else
            document.getElementById(checkBox).checked = true;
    }
}

function SetDwellingClass(dd_Residence_CoverageForm, ddlStructure, ddlClassification, dvReplacementCC, isVerisk360Enabled) {
    var struct = document.getElementById(ddlStructure);
    var dwellingClass = document.getElementById(ddlClassification);
    var coverageForm = document.getElementById(dd_Residence_CoverageForm);

    if (struct.selectedIndex == 3) {
        dwellingClass.selectedIndex = 2
        dwellingClass.disabled = true
    }
    else
        dwellingClass.disabled = false
    document.getElementById(dvReplacementCC).style.display = "block";
    //2-MOBILE HOME, 8-MOBILE MANUFACTURED
    if (isVerisk360Enabled == "True" && (struct.value == "8" || struct.value == "2" || coverageForm.value == "17")) {
        document.getElementById(dvReplacementCC).style.display = "none";
    }
}

function SetPrivateLightPolesIncludedLimit(dd_Residence_CoverageForm, txtPrivatePowerPolesIncludedLimit, txtPrivatePowerPolesIncreaseLimit, txtPrivatePowerPolesTotalLimit) {
    var coverageForm = document.getElementById(dd_Residence_CoverageForm);
    const lightPolesIncludedLimit = document.getElementById(txtPrivatePowerPolesIncludedLimit);
    const lightPolesIncreasedLimit = document.getElementById(txtPrivatePowerPolesIncreaseLimit);
    const lightPolesTotalLimit = document.getElementById(txtPrivatePowerPolesTotalLimit);
    if (coverageForm.value == 13 || coverageForm.value == 15 || coverageForm.value == 16) {
        lightPolesIncludedLimit.value = "1500";
        if (lightPolesIncreasedLimit.value == "" || lightPolesIncreasedLimit.value == "0") {
            lightPolesTotalLimit.value = lightPolesIncludedLimit.value
        }
        else {
            lightPolesTotalLimit.value = parseInt(lightPolesIncludedLimit.value) + parseInt(lightPolesIncreasedLimit.value);
        }
    }
    else {
        lightPolesIncludedLimit.value = "2500";
        if (lightPolesIncreasedLimit.value == "" || lightPolesIncreasedLimit.value == "0") {
            lightPolesTotalLimit.value = lightPolesIncludedLimit.value
        }
        else {
            lightPolesTotalLimit.value = parseInt(lightPolesIncludedLimit.value) + parseInt(lightPolesIncreasedLimit.value);
        }
    }
}

function TogglePrivateLightPoles(dd_Residence_CoverageForm, dvPrivatePowerPoles, chkPrivatePowerPoles, tblPrivatePowerPoles) {
    var coverageForm = document.getElementById(dd_Residence_CoverageForm);
    var dvPrivatePowerPoles = document.getElementById(dvPrivatePowerPoles);
    var tblPrivatePowerPoles = document.getElementById(tblPrivatePowerPoles);
    var chkPrivatePowerPoles = document.getElementById(chkPrivatePowerPoles);
    if (coverageForm.value == 17) {
        dvPrivatePowerPoles.style.display = "none";
        chkPrivatePowerPoles.checked = false;
    }
    else {
        dvPrivatePowerPoles.style.display = "block";
        tblPrivatePowerPoles.style.display = "block";
        chkPrivatePowerPoles.checked = true;
    }
}

function updateBldngHeaderText(headerId, bldngNum) {
    //var firstNameText = $("#" + firstNameId).val();
    //var lastNameText = $("#" + lastNameID).val();
    //var yearText = $("#" + yearID).val();
    //var newHeaderText = ("RV/WATERCRAFT #" + (driverNum + 1).toString() + " - " + yearText + " " + firstNameText + " " + lastNameText).toUpperCase();

    var newHeaderText = ("Building #" + (bldngNum + 1).toString());

    $("#" + headerId).val(newHeaderText);

    newHeaderText = newHeaderText.toUpperCase();
    if (newHeaderText.length > 34) {
        $("#" + headerId).text(newHeaderText.substring(0, 38) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }
}

//
// Disables un-needed fields when Acreage Only is selected
//
function ToggleAcreageOnlyFields(chkAcreOnly, txtSqrFeet, ddlNumberofFamilies, ddlStructure, ddlOccupancy, ddlConstruction, ddlStyle, ddlDwellingClass, txtYearBuilt) {
    if (chkAcreOnly.checked) {
        var families = document.getElementById(ddlNumberofFamilies);
        var structure = document.getElementById(ddlStructure);
        var occupancy = document.getElementById(ddlOccupancy);
        var construction = document.getElementById(ddlConstruction);
        var style = document.getElementById(ddlStyle);
        var dwellingClass = document.getElementById(ddlDwellingClass);
        var yearBuilt = document.getElementById(txtYearBuilt);

        $("#" + txtSqrFeet).val("");
        document.getElementById(txtSqrFeet).disabled = true;
        families.selectedIndex = 0;
        document.getElementById(ddlNumberofFamilies).disabled = true;
        structure.selectedIndex = 0;
        document.getElementById(ddlStructure).disabled = true;
        occupancy.selectedIndex = 0;
        document.getElementById(ddlOccupancy).disabled = true;
        construction.selectedIndex = 0;
        document.getElementById(ddlConstruction).disabled = true;
        style.selectedIndex = 0;
        document.getElementById(ddlStyle).disabled = true;
        dwellingClass.selectedIndex = 0;
        document.getElementById(ddlDwellingClass).disabled = true;
        $("#" + txtYearBuilt).val("");
        document.getElementById(txtYearBuilt).disabled = true;
    }
    else {
        document.getElementById(txtSqrFeet).disabled = false;
        document.getElementById(ddlNumberofFamilies).disabled = false;
        document.getElementById(ddlStructure).disabled = false;
        document.getElementById(ddlOccupancy).disabled = false;
        document.getElementById(ddlConstruction).disabled = false;
        document.getElementById(ddlStyle).disabled = false;
        document.getElementById(ddlDwellingClass).disabled = false;
        document.getElementById(txtYearBuilt).disabled = false;
    }
}

function ToggleAcreageOnlyCheck(chkAcreageOnly, hiddenAcreageOnly) {
    if (chkAcreageOnly.checked)
        document.getElementById(hiddenAcreageOnly).value = "true";
    else
        document.getElementById(hiddenAcreageOnly).value = "false";
}

//
// Farm Personal Property Controls
//

// With Peak Season
function ToggleScheduledPeakSeason(dvControl, chkControl, dvPeakControl, chkPeakControl) {
    if (document.getElementById(chkControl).checked) {
        document.getElementById(dvControl).style.display = "block"; document.getElementById(chkControl).disabled = true
        document.getElementById(chkPeakControl).disabled = true
    }
    else {
        document.getElementById(dvControl).style.display = "none";
        document.getElementById(dvPeakControl).style.display = "none";
        document.getElementById(chkPeakControl).checked = false
        document.getElementById(chkPeakControl).disabled = false
    }
}

// Without Peak Season
function ToggleScheduledLimit(dvControl, chkControl) {
    if (document.getElementById(chkControl).checked) {
        document.getElementById(dvControl).style.display = "block";
        document.getElementById(chkControl).disabled = true
    }
    else {
        document.getElementById(dvControl).style.display = "none";
        document.getElementById(chkControl).disabled = false
    }
}

// ToggleSheepPerils
function ToggleSheepPerilsLimit(dvControl, chkControl, txtLimit) {
    if (document.getElementById(chkControl).checked) {
        document.getElementById(dvControl).style.display = "block";
        document.getElementById(dvControl).value = "true"
        document.getElementById(txtLimit).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvControl).style.display = "none";
            document.getElementById(dvControl).value = "false"
        }
        else
            document.getElementById(chkControl).checked = true;
    }
}

function ToggleGlassBreakCabs(tblControl, chkControl, txtLimit) {
    if (document.getElementById(chkControl).checked) {
        document.getElementById(tblControl).style.display = "block";
        document.getElementById(txtLimit).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(tblControl).style.display = "none";
            document.getElementById(txtLimit).value = ""
        }
        else
            document.getElementById(chkControl).checked = true;
    }
}

function ToggleGenericPersonalPropertyControl(tblControl, chkControl, txtLimit) {
    if (document.getElementById(chkControl).checked) {
        document.getElementById(tblControl).style.display = "block";
        document.getElementById(txtLimit).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(tblControl).style.display = "none";
            document.getElementById(txtLimit).value = ""
        }
        else
            document.getElementById(chkControl).checked = true;
    }
}

// Toggle More/Less
function ToggleFarmMoreLess(dvMoreLess, lnkMoreLess, hiddenMoreLess, hiddenMoreLessCnt) {
    if (document.getElementById(hiddenMoreLess).value == "collapsed") {
        document.getElementById(dvMoreLess).style.display = "block";
        $("#" + lnkMoreLess).text("-Less");
        document.getElementById(hiddenMoreLess).value = "expanded"
    }
    else {
        document.getElementById(dvMoreLess).style.display = "none";
        $("#" + lnkMoreLess).text("+More (" + document.getElementById(hiddenMoreLessCnt).value + ")");
        document.getElementById(hiddenMoreLess).value = "collapsed"
    }
}

// Validate Blanket Unscheduled Personal Property Limit
function ValidateUnSchedPersPropLimit(blanketLimit, chkBlanketPeak, hiddenOldLimit) {
    blanketLimit.value = blanketLimit.value.replace(",", ""); //Dan Gugenheim - 5/12/2016 - Found that the comma character messes up the parseInt function by causing it to truncate everything after the comma. 
    if ((blanketLimit.value == "" || blanketLimit.value) <= 0 && document.getElementById(chkBlanketPeak).checked) {
        alert("Peak Season must be removed before Blanket Limit is removed");
        blanketLimit.value = document.getElementById(hiddenOldLimit).value
    }
    else {
        if (parseInt(blanketLimit.value) == 0 || blanketLimit.value == "") {
            blanketLimit.value = "";
            document.getElementById(chkBlanketPeak).checked = false
            document.getElementById(chkBlanketPeak).disabled = true
        }
        else {
            if (parseInt(blanketLimit.value) < 15000) {
                alert("Blanket must be a minimum of $15,000");
                blanketLimit.value = "15,000";
            }
            else
                blanketLimit.value = RoundValue5000(parseInt(blanketLimit.value));
        }
    }
}

function ToggleUnSchedPeak(txtBlanketLimit, chkPeakSeason) {
    if (!isNaN(txtBlanketLimit.value))
        document.getElementById(chkPeakSeason).disabled = false
    else
        document.getElementById(chkPeakSeason).disabled = true
}
function ToggleUnSchedEarthquake(txtBlanketLimit, chkEarthquake) {
    var blanketVal = document.getElementById(txtBlanketLimit).value;
    if (blanketVal != '' && blanketVal != '0')
        document.getElementById(chkEarthquake).disabled = false
    else
        document.getElementById(chkEarthquake).disabled = true
}

// Round a number up by 5,000
function RoundValue5000(number) {
    var returnNum = 0;

    if (number == 0)
        returnNum = 0;
    else {
        if (number < 15000) {
            returnNum = 15000;
        }
        else {
            returnNum = Math.ceil(number / 5000) * 5000;
        }
    }

    return FormatAsNumber(returnNum.toString());

}

// Set hidden field in case error is encountered
function SetUnSchedOldLimit(blanketLimit, hiddenOldLimit) {
    document.getElementById(hiddenOldLimit).value = blanketLimit.value
}

function BuildingType_DwellingDisplay(ddBuildingId, txtLimitId, ddBuildingTypeId, dwellingContentsRowId, replacementCostSectionId, isCoverageEstructureAvailable, isVerisk360Enabled) {
    Dwelling_ReplacementCost_Display(ddBuildingId, dwellingContentsRowId, replacementCostSectionId, isVerisk360Enabled)
    //if (!ifm.vr.currentQuote.isEndorsement) {
    //    SuggestFarm_BuildingType(ddBuildingId, txtLimitId, ddBuildingTypeId)
    //}
    SuggestFarm_BuildingType(ddBuildingId, txtLimitId, ddBuildingTypeId, isCoverageEstructureAvailable)
}

//Pulled out Interoperability Code so we can better control BuildingType Code. CAH 20210215 
function Dwelling_ReplacementCost_Display(ddBuildingId, dwellingContentsRowId, replacementCostSectionId, isVerisk360Enabled) {
    var farmStructureTypeId = $("#" + ddBuildingId).val();
    //added 10/26/2020 (Interoperability)
    var dwellingContentsRow
    if (dwellingContentsRowId) {
        if (typeof (dwellingContentsRowId) == 'undefined') {
            dwellingContentsRowId = '';
        }
        if (dwellingContentsRowId.length > 0) {
            dwellingContentsRow = document.getElementById(dwellingContentsRowId);
            if (dwellingContentsRow) {
                dwellingContentsRow.style.display = "none";
            }
        }
    }
    var replacementCostSection
    if (replacementCostSectionId) {
        if (typeof (replacementCostSectionId) == 'undefined') {
            replacementCostSectionId = '';
        }
        if (replacementCostSectionId.length > 0) {
            replacementCostSection = document.getElementById(replacementCostSectionId);
            if (replacementCostSection) {
                replacementCostSection.style.top = "-250px";
            }
        }
    }

    if (farmStructureTypeId == "17" || farmStructureTypeId == "18" || farmStructureTypeId == "37") //Farm Dwelling, MOBILE HOME DWELLING, OUTBUILDING Or Empty
    {
        //added 10/26/2020 (Interoperability)
        if (dwellingContentsRow) {
            dwellingContentsRow.style.display = "table-row";
        }
        if (replacementCostSection) {
            replacementCostSection.style.top = "-260px";
        }
    }
    if (farmStructureTypeId == "10" || farmStructureTypeId == "11" || farmStructureTypeId == "13" || farmStructureTypeId == "14" || farmStructureTypeId == "15" || farmStructureTypeId == "") {
        if (replacementCostSection) {
            replacementCostSection.style.top = "-240px";
        }
    }

    if (isVerisk360Enabled == "True") {
        if (farmStructureTypeId == "17" || farmStructureTypeId == "34" || farmStructureTypeId == "35" || farmStructureTypeId == "36" || farmStructureTypeId == "37" || farmStructureTypeId == "26" || farmStructureTypeId == "21" || farmStructureTypeId == "33") {
            if (replacementCostSection) {
                replacementCostSection.style.display = "none";
            }
        }
        else {
            // if (replacementCostSection) {
            replacementCostSection.style.display = "table-row"; //if this is does not work make it true Monika todo
            //}
        }
    }

}

function SuggestFarm_BuildingType(ddBuildingId, txtLimitId, ddBuildingTypeId, isFARMBuildingTypeForBuildingsAvailable) {
    //var startingVal = $("#" + ddBuildingTypeId).val();
    //var limit = $("#" + txtLimitId).val().toString().toFloat();
    var farmStructureTypeId = $("#" + ddBuildingId).val();
    //var farmTypeId = $("#" + ddBuildingTypeId).val();

    $("#" + ddBuildingTypeId).attr('title', '');

    $("#" + ddBuildingTypeId + ' :not(option[value="a"])').removeAttr('disabled'); // enable all options by default

    if (!ifm.vr.currentQuote.isEndorsement) {

        if (farmStructureTypeId == "12") //Grain Bin
        {
            //Updated 12/09/2019 for bug 41276 MLW
            $("#" + ddBuildingTypeId + ' option[value="8"]').attr('disabled', 'disabled'); //disables specified value
        }

        if (farmStructureTypeId == "27") //Grain Dryer
        {
            // send N/A [5]
            $("#" + ddBuildingTypeId).val('5');
            $("#" + ddBuildingTypeId + ' :not(option[value="5"])').attr('disabled', 'disabled'); //disable all
            $("#" + ddBuildingTypeId).attr('title', 'No other options available based on type of building.');
        }

        //Added 5/21/18 for Bug 20411 MLW
        if (farmStructureTypeId == "34") //Private Power & Light Pole
        {
            // send N/A [5]
            $("#" + ddBuildingTypeId).val('5');
            $("#" + ddBuildingTypeId + ' :not(option[value="5"])').attr('disabled', 'disabled'); //disable all
            $("#" + ddBuildingTypeId).attr('title', 'No other options available based on type of building.');
        }

        if (farmStructureTypeId == "19") //Hoop Building
        {
            // suggest Type 2 Open[8] but can still be Type 3[3]
            //if (startingVal == '' || startingVal == '1' || startingVal == '2') //suggest if
            // Bug# 6534 - Added functionality to allow agent to override the default building type
            if ($("#" + ddBuildingTypeId).val() == "")
                $("#" + ddBuildingTypeId).val('8');

            $("#" + ddBuildingTypeId + ' option[value="5"]').attr('disabled', 'disabled'); //disables specified value
            $("#" + ddBuildingTypeId + ' option[value="1"]').attr('disabled', 'disabled'); //disables specified value
            $("#" + ddBuildingTypeId + ' option[value="2"]').attr('disabled', 'disabled'); //disables specified value
            //$("#" + ddBuildingTypeId + ' option[value="3"]').attr('disabled', 'disabled'); //disables specified value
            $("#" + ddBuildingTypeId).attr('title', 'Options limited based on type of building.');
        }

        if (farmStructureTypeId == "32") //Green House
        {
            // must be Type 3
            $("#" + ddBuildingTypeId).val('3');
            $("#" + ddBuildingTypeId + ' :not(option[value="3"])').attr('disabled', 'disabled'); //disable all
            $("#" + ddBuildingTypeId).attr('title', 'No other options available based on type of building.');
        }

        //Added 5/21/18 for Bug 20410 MLW
        if (farmStructureTypeId == "18") //Farm Dwelling
        {
            // suggest Type 2[2], but do not allow N/A [5], Type 1[1] and Type 2 Open[8]
            $("#" + ddBuildingTypeId).val('2');
            $("#" + ddBuildingTypeId + ' option[value="5"]').attr('disabled', 'disabled'); //disables specified value
            $("#" + ddBuildingTypeId + ' option[value="1"]').attr('disabled', 'disabled'); //disables specified value
            $("#" + ddBuildingTypeId + ' option[value="8"]').attr('disabled', 'disabled'); //disables specified value
            $("#" + ddBuildingTypeId).attr('title', 'No other options available based on type of building.');
        }

    }

    if (isFARMBuildingTypeForBuildingsAvailable == "True") {


        var farmStructureTypeId1Array = ["10", "11", "13", "14", "15", "20"]; // 10- BARN,11 - CONFINEMENT BUILDING, 13- IMPLEMENT/MACHINE SHED, 14- OUTBUILDING, 15 - POLE BUILDING,20 - QUONSET BUILDING
        if (farmStructureTypeId1Array.includes(farmStructureTypeId)) {
            $("#" + ddBuildingTypeId + ' option[value="5"]').attr('disabled', 'disabled'); //disables specified value
        }
        var farmStructureTypeId2Array = ["16", "28", "29", "30", "31", "37"]; // 16 -SILO, 28 - GRAIN LEG, 29 -POULTRY BUILDING, 30 - CRIB, 31- GRANARY, 37 - OUTBUILDING WITH LIVING QUARTERS

        if (farmStructureTypeId2Array.includes(farmStructureTypeId)) {
            if (ifm.vr.currentQuote.isEndorsement) {
                var selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                if (selectedDDBuildingTypeId !== "5") {
                    $("#" + ddBuildingTypeId + ' option[value="5"]').attr('disabled', 'disabled'); //disables specified value
                }
                if (selectedDDBuildingTypeId !== "8") {
                    $("#" + ddBuildingTypeId + ' option[value="8"]').attr('disabled', 'disabled'); //disables specified value
                }


            }
            else {
                $("#" + ddBuildingTypeId + ' option[value="5"]').attr('disabled', 'disabled'); //disables specified value
                $("#" + ddBuildingTypeId + ' option[value="8"]').attr('disabled', 'disabled'); //disables specified value
            }
        }

        const farmStructureTypeId3Array = ["21", "26"]; // 21 - PORTABLE STRUCTURE, 26 - WELL PUMP
        const farmStructureType3ForbiddenArray = ["5", "1", "2", "8"];

        if (farmStructureTypeId3Array.includes(farmStructureTypeId)) {


            if (ifm.vr.currentQuote.isEndorsement) {
                const selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                farmStructureType3ForbiddenArray.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });
                if (farmStructureType3ForbiddenArray.includes(selectedDDBuildingTypeId)) {
                    farmStructureType3ForbiddenArray.forEach(id => {
                        if (selectedDDBuildingTypeId === id) {
                            $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', false);
                        }
                    });
                } else {
                    $("#" + ddBuildingTypeId).val('3');   // default value for endorsement
                }
            } else {
                $("#" + ddBuildingTypeId).val('3');   // default value for new business
                farmStructureType3ForbiddenArray.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });

            }
        }


        const farmStructureTypeId5Array = ["35", "36"];  //35 - RADIO &amp; TELEVISION EQUIPMENT, 36 - WINDMILLS AND CHARGERS
        const farmStructureType5ForbiddenArray = ["1", "2", "3", "8"];

        if (farmStructureTypeId5Array.includes(farmStructureTypeId)) {
            if (ifm.vr.currentQuote.isEndorsement) {
                const selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                farmStructureType5ForbiddenArray.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });
                if (farmStructureType5ForbiddenArray.includes(selectedDDBuildingTypeId)) {
                    farmStructureType5ForbiddenArray.forEach(id => {
                        if (selectedDDBuildingTypeId === id) {
                            $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', false);
                        }
                    });
                } else {
                    $("#" + ddBuildingTypeId).val('5');   // default value for endorsement
                }
            } else {
                $("#" + ddBuildingTypeId).val('5');   // default value for new business
                farmStructureType5ForbiddenArray.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });

            }
        }

        var farmStructureTypeId4Array = ["33"];      //33-TANK 

        if (farmStructureTypeId4Array.includes(farmStructureTypeId)) {
            if (ifm.vr.currentQuote.isEndorsement) {
                var selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                if (selectedDDBuildingTypeId !== "8") {
                    $("#" + ddBuildingTypeId + ' option[value="8"]').attr('disabled', 'disabled'); //disables specified value
                }
            }

            else {
                $("#" + ddBuildingTypeId + ' option[value="8"]').attr('disabled', 'disabled'); //disables specified value
            }
        }


        if (ifm.vr.currentQuote.isEndorsement) {

            if (farmStructureTypeId == "18") {  //Farm Dwelling
                const farmStructureType18DisabledValues = ["5", "1", "8"];  //disable specific type values

                var selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                farmStructureType18DisabledValues.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });
                if (farmStructureType18DisabledValues.includes(selectedDDBuildingTypeId)) {
                    farmStructureType3ForbiddenArray.forEach(id => {
                        if (selectedDDBuildingTypeId === id) {
                            $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', false);
                        }
                    });
                } else {
                    $("#" + ddBuildingTypeId).val('2');   // default value for endorsement
                }
            }

            if (farmStructureTypeId == "27") {  //Grain Dryer
                const farmStructureType27DisabledValues = ["1", "2", "8", "3"];  //disable specific type values

                var selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                farmStructureType27DisabledValues.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });
                if (farmStructureType27DisabledValues.includes(selectedDDBuildingTypeId)) {
                    farmStructureType27DisabledValues.forEach(id => {
                        if (selectedDDBuildingTypeId === id) {
                            $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', false);
                        }
                    });
                } else {
                    $("#" + ddBuildingTypeId).val('5');   // default value for endorsement
                }
            }

            if (farmStructureTypeId == "34") {  //Private Power & Light Pole
                const farmStructureType34DisabledValues = ["1", "2", "8", "3"];  //disable specific type values

                var selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                farmStructureType34DisabledValues.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });
                if (farmStructureType34DisabledValues.includes(selectedDDBuildingTypeId)) {
                    farmStructureType34DisabledValues.forEach(id => {
                        if (selectedDDBuildingTypeId === id) {
                            $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', false);
                        }
                    });
                } else {
                    $("#" + ddBuildingTypeId).val('5');   // default value for endorsement
                }
            }

            if (farmStructureTypeId == "32") {  //Green House
                const farmStructureType32DisabledValues = ["5", "1", "2", "8"];  //disable specific type values

                var selectedDDBuildingTypeId = $("#" + ddBuildingTypeId).val();
                farmStructureType32DisabledValues.forEach(id => {
                    $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', 'disabled');
                });
                if (farmStructureType32DisabledValues.includes(selectedDDBuildingTypeId)) {
                    farmStructureType32DisabledValues.forEach(id => {
                        if (selectedDDBuildingTypeId === id) {
                            $("#" + ddBuildingTypeId + ` option[value="${id}"]`).attr('disabled', false);
                        }
                    });
                } else {
                    $("#" + ddBuildingTypeId).val('3');   // default value for endorsement
                }
            }

        }
    }

}
// Validate Start Peak Season Dates
function ValidateStartPeakDates(txtPeakStart, txtPeakEnd, control, effectiveDate) {
    var splitEff = effectiveDate.split("/");
    var effMonthDayInt = parseInt(splitEff[0].toString() + splitEff[1].toString());
    var newDateFormat = "";

    if (document.getElementById(txtPeakStart).value != "") {
        var splitStart = document.getElementById(txtPeakStart).value.split("/");

        if (splitStart.length > 1) {
            var startInt = parseInt(splitStart[0].toString() + splitStart[1].toString());
            var year = parseInt(splitEff[2].toString());

            if (parseInt(splitStart[0].toString()) > 0 && parseInt(splitStart[0].toString()) <= 12) {
                if (splitStart[0].length < 2) {
                    if (parseInt(splitStart[0].toString()) < 10) {
                        newDateFormat = '0' + splitStart[0];
                    }
                    else {
                        newDateFormat = splitStart[0];
                    }
                }
                else {
                    newDateFormat = splitStart[0];
                }

                if (parseInt(splitStart[0].toString()) < parseInt(splitEff[0].toString()))
                    year += 1;

                var daysInMonth = DaysInMonth(parseInt(splitStart[0].toString()), year);

                if (parseInt(splitStart[0].toString()) < 1 || parseInt(splitStart[1].toString()) > daysInMonth) {
                    alert("Invalid Day");
                    document.getElementById(txtPeakStart).focus();
                }
                else {
                    if (splitStart[1] != "0") {
                        if (splitStart[1].length < 2) {
                            if (parseInt(splitStart[1].toString()) < 10) {
                                $("#" + txtPeakStart).val(newDateFormat + '/0' + splitStart[1]);
                            }
                            else {
                                if (splitStart[1] == "") {
                                    alert("Missing Day")
                                    document.getElementById(txtPeakStart).focus();
                                }
                                else
                                    $("#" + txtPeakStart).val(newDateFormat + '/' + splitStart[1]);
                            }
                        }
                        else {
                            $("#" + txtPeakStart).val(newDateFormat + '/' + splitStart[1]);
                        }
                    }
                    else {
                        alert("Invalid Day");
                        document.getElementById(txtPeakStart).focus();
                    }
                }
            }
            else {
                alert("Invalid Month");
                document.getElementById(txtPeakStart).focus();
            }
        }
        else {
            alert("Invalid Date");
            document.getElementById(txtPeakStart).focus();
        }
    }
}

// Validate End Peak Season Dates
function ValidateEndPeakDates(txtPeakStart, txtPeakEnd, control, effectiveDate) {
    var splitEff = effectiveDate.split("/");
    var effMonthDayInt = parseInt(splitEff[0].toString() + splitEff[1].toString());
    var newDateFormat = "";

    if (document.getElementById(txtPeakEnd).value != "") {
        var splitEnd = document.getElementById(txtPeakEnd).value.split("/");

        if (splitEnd.length > 1) {
            var endInt = parseInt(splitEnd[0].toString() + splitEnd[1].toString());
            var year = parseInt(splitEff[2].toString());

            if (parseInt(splitEnd[0].toString()) > 0 && parseInt(splitEnd[0].toString()) <= 12) {
                if (splitEnd[0].length < 2) {
                    if (parseInt(splitEnd[0].toString()) < 10) {
                        newDateFormat = '0' + splitEnd[0];
                    }
                    else {
                        newDateFormat = splitEnd[0];
                    }
                }
                else {
                    newDateFormat = splitEnd[0];
                }

                if (parseInt(splitEnd[0].toString()) < parseInt(splitEff[0].toString()))
                    year += 1;

                var daysInMonth = DaysInMonth(parseInt(splitEnd[0].toString()), year);

                if (parseInt(splitEnd[0].toString()) < 1 || parseInt(splitEnd[1].toString()) > daysInMonth) {
                    alert("Invalid Day");
                    document.getElementById(txtPeakEnd).focus();
                }
                else {
                    if (splitEnd[1] != "0") {
                        if (splitEnd[1].length < 2) {
                            if (parseInt(splitEnd[1].toString()) < 10) {
                                $("#" + txtPeakEnd).val(newDateFormat + '/0' + splitEnd[1]);
                            }
                            else {
                                if (splitEnd[1] == "") {
                                    alert("Missing Day")
                                    document.getElementById(txtPeakEnd).focus();
                                }
                                else
                                    $("#" + txtPeakEnd).val(newDateFormat + '/' + splitEnd[1]);
                            }
                        }
                        else {
                            $("#" + txtPeakEnd).val(newDateFormat + '/' + splitEnd[1]);
                        }
                    }
                    else {
                        alert("Invalid Day");
                        document.getElementById(txtPeakEnd).focus();
                    }
                }
            }
            else {
                alert("Invalid Month");
                document.getElementById(txtPeakEnd).focus();
            }
        }
        else {
            alert("Invalid Date");
            document.getElementById(txtPeakEnd).focus();
        }
    }
}

// Get Number of Days in the Month
function DaysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}

function ChangeBuildingDimensionLabel(ddId, lblId, lblSqrFeetId) {
    var buildingTypes_bushels = new Array('12');
    var buildingTypes_Dimensions_notrequired = new Array('18', '21', '26', '27', '28', '33', '34', '35', '36');

    var buildindTypeId = $("#" + ddId).val();

    if (buildingTypes_bushels.contains(buildindTypeId)) {
        $("#" + lblId).text("*Bushels");
    }
    else {

        if (buildingTypes_Dimensions_notrequired.contains(buildindTypeId)) {
            $("#" + lblId).text("Dimensions");
        }
        else {
            $("#" + lblId).text("*Dimensions");
        }
    }

    if (buildindTypeId == '18')
        $("#" + lblSqrFeetId).text("*Square Feet");
    else
        $("#" + lblSqrFeetId).text("Square Feet");

}


function AdjustPrimaryAcres(arrayOfIds, senderIndex, startingAcresInt) {
    var primaryId = arrayOfIds[0];

    var secondarySum = 0;
    for (var i = 1; i < arrayOfIds.length; i++) {
        var acresVal = $("#" + arrayOfIds[i]).val();
        if (!acresVal == "")
            secondarySum += parseInt(acresVal);
    }

    var endingPrimaryAcres = startingAcresInt - secondarySum;

    if (endingPrimaryAcres < 1) {
        // you can't do this
        $("#" + arrayOfIds[senderIndex]).val('0');
        AdjustPrimaryAcres(arrayOfIds, senderIndex, startingAcresInt);
        alert('You can not enter an acres value greater than or equal to the remaining acres from the primary location.');
    }
    else {
        $("#" + primaryId).val(endingPrimaryAcres.toString());
    }
}

function ToggleLossIncome(chkLossIncome, txtLossIncomeLimit, dvLossIncome, dvLossIncomeLimit, ddlCoinsurance, ddlLossExt, structureID) {
    var clientCI = document.getElementById(ddlCoinsurance);
    var clientLE = document.getElementById(ddlLossExt);

    if (document.getElementById(chkLossIncome).checked) {
        if (structureID == "18" || structureID == "17") {
            document.getElementById(dvLossIncomeLimit).style.display = "block";
            document.getElementById(dvLossIncome).style.display = "none";
            clientCI.selectedIndex = 0;
            clientLE.selectedIndex = 0;
        }
        else {
            document.getElementById(dvLossIncomeLimit).style.display = "block";
            document.getElementById(dvLossIncome).style.display = "block";
        }
        document.getElementById(txtLossIncomeLimit).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            $("#" + txtLossIncomeLimit).val('');
            document.getElementById(dvLossIncomeLimit).style.display = "none";
            document.getElementById(dvLossIncome).style.display = "none";
            clientCI.selectedIndex = 0;
            clientLE.selectedIndex = 0;
        }
        else
            document.getElementById(chkLossIncome).checked = true;

    }
}

function ToggleContractGrowers(chkContractGrowers, dvContractLimit, ddlContractLimit) {
    var client = document.getElementById(ddlContractLimit);

    if (document.getElementById(chkContractGrowers).checked) {
        document.getElementById(dvContractLimit).style.display = "block";
        client.selectedIndex = 0;
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvContractLimit).style.display = "none";
            $("#" + ddlContractLimit).val('');
        }
        else
            document.getElementById(chkContractGrowers).checked = true;
    }
}

function ToggleSuffocation(chkSuffocation, dvSuffocationLimit, txtSuffocation) {
    if (document.getElementById(chkSuffocation).checked) {
        document.getElementById(dvSuffocationLimit).style.display = "block";
        document.getElementById(txtSuffocation).focus();
    }
    else {
        if (ConfirmFarmDialog()) {
            document.getElementById(dvSuffocationLimit).style.display = "none";
            $("#" + txtSuffocation).val('');
        }
        else
            document.getElementById(chkSuffocation).checked = true;
    }
}

function ToggleExpandAddlLossIncome(control, chkLossIncome, dvLossIncomeData, dvLossIncomeLimit, txtLossLimit, ddlCoinsurance, ddlLossExt) {
    var clientCoinsurance = document.getElementById(ddlCoinsurance);
    var clientLossExt = document.getElementById(ddlLossExt);

    //if (document.getElementById(chkLossIncome).checked) {
    //    document.getElementById(dvLossIncomeLimit).style.display = "block";
    //    if (control.value != "18")
    //        document.getElementById(dvLossIncomeData).style.display = "block";
    //    else
    //        document.getElementById(dvLossIncomeData).style.display = "none";
    //}
    //else {
    document.getElementById(dvLossIncomeLimit).style.display = "none";
    document.getElementById(dvLossIncomeData).style.display = "none";
    $("#" + txtLossLimit).val('');
    clientCoinsurance.selectedIndex = 0;
    clientLossExt.selectedIndex = 0;
    document.getElementById(chkLossIncome).checked = false;
    //}
}

function ToggleFarmMachinerySpecialCoverageG(limitID, farmMachineryID, labelID) {
    var farmCheckbox = document.getElementById(farmMachineryID);
    if (farmCheckbox !== null) {
        var limitTextbox = document.getElementById(limitID);
        if (limitTextbox.value === null || limitTextbox.value === "") {
            farmCheckbox.disabled = true;
            var titleText = "Requires Unscheduled Farm Personal Property Blanket Limit"
            farmCheckbox.title = titleText;
            document.getElementById(labelID).title = titleText;
            if (farmCheckbox.checked === true) {
                alert("Farm Machinery - Special Coverage - Coverage G requires Unscheduled Farm Personal Property Blanket Limit. Coverage will be removed at next save/rate if no limit is entered.");
            }
        } else {
            farmCheckbox.disabled = false;
            farmCheckbox.title = "";
            document.getElementById(labelID).title = "";
        }
    }
}


//Added 12/03/18 for multi state CAH
//Minesub on Location/Dwelling & Com. Property
//$(document).ready(function () {

//    if (master_LobId == IFMLOBEnum.FAR.LobId) {

//        var StateArray = [];
//        var CountyObject = {};

//        var ms_st = null;
//        if (typeof FAR_MineSubState === 'undefined') {
//            ms_st = '';
//        }
//        else {
//            ms_st = FAR_MineSubState;
//        }

//        // Don't use this method if the state is OH
//        if (ms_st != 'OH') {
//            //Check Mine Sub options on page load
//            callMineSubData()

//            // JavaScript source code - Call MS Updates when these change
//            $('[id*=txtZipCode],[id*=ddStateAbbrev],[id*=txtGaragedCounty],[id*=dd_Residence_CoverageForm]').on('change', function () {
//                //callMineSubFunctions()
//                callMineSubData()
//            });

//            // JavaScript source code - On MS Selections with Confirmation
//            $('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]').on('change', function () {
//                var mineSubElement = $(this);
//                var mineSubState = getLocationStateValue(mineSubElement);
//                var mineSubCounty = getLocationCountyText(mineSubElement);
//                if (!mineSubElement.prop('checked')) {
//                    if (!ConfirmFarmDialog()) {
//                        mineSubElement.prop("checked", true);
//                    }
//                    else {
//                        switch (mineSubState) {
//                            case '15':
//                                if (!CountyObject[mineSubState].contains(mineSubCounty)) {
//                                    disableNonMineSubElements("ENABLE");
//                                }
//                                break;
//                        }
//                    }
//                }
//                callMineSubData()
//            });
//        }

//        //Given any element under a location and this will return the value for the State Selection
//        function getLocationStateValue(element) {
//            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=ddStateAbbrev]').val();
//        }

//        //Given any element under a location and this will return the value for the ZIP Code Text
//        function getLocationZipText(element) {
//            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=txtZipCode]').val();
//        }

//        //Given any element under a location and this will return the value for the County Text
//        function getLocationCountyText(element) {
//            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=txtGaragedCounty]').val();
//        }

//        //Given any element under a location and this will return the value for the Coverage Form Selection
//        function getCoverageFormValue(element) {
//            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ResidenceDiv]').find('[id*=dd_Residence_CoverageForm]').val();
//        }

//        //Calls all the functions for Mine Sub
//        function callMineSubFunctions() {
//            checkMineSubOnLocation();
//            checkMineSubOnBuilding();
//            checkForNonMineSubPositive()
//        }

//        //Gets all mine sub counties for states on the policy and calls the mineSub functions
//        function callMineSubData() {
//            getMineSubStates()
//            StateArray.forEach(function (item) {
//                if (CountyObject == undefined || CountyObject.item == undefined || CountyObject[item].length == 0) {
//                    ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(item, function (data) {
//                        CountyObject[item] = data;
//                        getMineSubStates() //Must call this again, because after AJAX return StateArray is empty
//                        if (Object.keys(CountyObject).length == StateArray.length) {
//                            callMineSubFunctions()
//                        }
//                    });

//                }
//            });

//        }

//        //gets the distinct States
//        function getMineSubStates() {
//            StateArray = [];
//            $('[id*=ddStateAbbrev]').each(function () {
//                if (StateArray.contains($(this).val()) == false) {
//                    StateArray.push($(this).val());
//                }
//            });
//        }

//        // JavaScript source code - checkMineSubOnLocation
//        function checkMineSubOnLocation() {
//            var mineSubNotReqCounter = 0;
//            $('input[id*=chkMineSubsidence]').each(function () {
//                var mineSubElement = $(this);
//                mineSubElement.closest('[id*=dvMineSubsidence]').hide();
//                mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceReqHelpInfo]').hide();
//                mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').hide();
//                var mineSubState = getLocationStateValue(mineSubElement);
//                var mineSubCounty = getLocationCountyText(mineSubElement);
//                var mineSubCoverage = getCoverageFormValue(mineSubElement);
//                var hasMineSubChecked = false;
//                switch (mineSubState) {
//                    case '15':
//                        if (multiStateEnabled == true) {
//                            mineSubElement.closest('[id*=dvMineSubsidence]').show();
//                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
//                                mineSubElement.prop("checked", true);
//                                mineSubElement.prop("disabled", true);
//                                mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceReqHelpInfo]').show();
//                                mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceReqHelpInfo_OH]').hide();
//                            }
//                            else {
//                                //for added non-mine building after mine sub checked on other non-mine building
//                                if (mineSubElement.prop("checked")) {
//                                    mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').show();
//                                }
//                            }
//                        }
//                        break;
//                    case '16':
//                        if (multiStateEnabled == true && mineSubCoverage !== "17") {
//                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
//                                mineSubElement.closest('[id*=dvMineSubsidence]').show();
//                            }
//                        }
//                        else {
//                            if (mineSubCoverage == "17") {
//                                mineSubElement.prop("checked", false);
//                                mineSubElement.prop("disabled", false);
//                            }
//                        }
//                        break;
//                }
//            });
//        }

//        // JavaScript source code - checkMineSubOnBuilding
//        function checkMineSubOnBuilding() {
//            var mineSubNotReqCounter = 0;
//            $('input[id*=chkMineBuilding]').each(function () {
//                var mineSubElement = $(this);
//                mineSubElement.closest('[id*=divMineSub]').hide();
//                mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubReqHelpInfo]').hide();
//                mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').hide();
//                if (multiStateEnabled == true) {
//                    var mineSubState = getLocationStateValue(mineSubElement);
//                    var mineSubCounty = getLocationCountyText(mineSubElement);
//                    switch (mineSubState) {
//                        case '15':
//                            mineSubElement.closest('[id*=divMineSub]').show();
//                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
//                                mineSubElement.prop("checked", true);
//                                mineSubElement.prop("disabled", true);
//                                mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubReqHelpInfo]').show();
//                                mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubReqHelpInfo_OH]').hide();
//                            }
//                            else {
//                                if (mineSubElement.prop('checked')) {
//                                    mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').show();
//                                }
//                            }
//                            break;
//                        case '16':
//                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
//                                mineSubElement.closest('[id*=divMineSub]').show();
//                            }
//                            break;
//                    }

//                }
//            });
//        }

//        // JavaScript source code - Check for Non-Mine Counties that are selected
//        function checkForNonMineSubPositive() {
//            if (multiStateEnabled == true) {
//                var hasNonMineSubChecked = false;
//                var hasProcessesedNonMine = false;
//                $('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]').each(function () {
//                    mineSubElement = $(this);
//                    if (hasNonMineSubChecked == false) {  // Used to skip processing of other elements
//                        var mineSubState = getLocationStateValue(mineSubElement);
//                        var mineSubCounty = getLocationCountyText(mineSubElement);
//                        switch (mineSubState) {
//                            case '15':
//                                if (hasNonMineSubChecked == false && CountyObject[mineSubState].contains(mineSubCounty) == false) {
//                                    if (mineSubElement.prop('checked')) {
//                                        hasNonMineSubChecked = true; // Used to skip processing of other counties
//                                        hasProcessesedNonMine = true;  //Used to skip multiple Non-Mine items
//                                        disableNonMineSubElements("disable");
//                                        return false;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    else {
//                        return true;
//                    }
//                });
//                if (hasNonMineSubChecked == false && hasProcessesedNonMine == false) {
//                    hasProcessesedNonMine = true;  //Used to skip multiple Non-Mine items
//                    disableNonMineSubElements("ENABLE");
//                }
//            }
//        }

//        // JavaScript source code - Handle all Non-Mines Enables and Disables by "action"
//        function disableNonMineSubElements(action) {
//            if (multiStateEnabled == true) {
//                var mineSubNotReqCounter = 0;
//                $('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]').each(function () {
//                    var mineSubElement = $(this);
//                    var mineSubState = getLocationStateValue(mineSubElement);
//                    var mineSubCounty = getLocationCountyText(mineSubElement);
//                    var mineSubCoverageF04 = getCoverageFormValue(mineSubElement) == 17;
//                    var test4 = mineSubElement[0].id.substring(90);
//                    switch (mineSubState) {
//                        case '15':
//                            if (mineSubElement[0].id.indexOf('chkMineSubsidence') !== -1) {
//                                if (!CountyObject[mineSubState].contains(mineSubCounty)) {
//                                    mineSubNotReqCounter += 1;
//                                    if (action.toUpperCase() == "ENABLE") {
//                                        mineSubElement.prop("disabled", false);
//                                        mineSubElement.prop("checked", false)
//                                        mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').hide();
//                                    }
//                                    else {

//                                        mineSubElement.prop("checked", true);
//                                        mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').show();
//                                        if (mineSubNotReqCounter > 1) {
//                                            mineSubElement.prop("disabled", true);
//                                        } else {
//                                            mineSubElement.prop("disabled", false);
//                                        }
//                                    }
//                                }
//                            }
//                            else {
//                                if (!CountyObject[mineSubState].contains(mineSubCounty)) {
//                                    mineSubNotReqCounter += 1;
//                                    if (action.toUpperCase() == "ENABLE") {
//                                        mineSubElement.prop("disabled", false);
//                                        mineSubElement.prop("checked", false)
//                                        mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').hide();
//                                    }
//                                    else {

//                                        mineSubElement.prop("checked", true);
//                                        mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').show();
//                                        if (mineSubNotReqCounter > 1) {
//                                            mineSubElement.prop("disabled", true);
//                                        } else {
//                                            mineSubElement.prop("disabled", false);
//                                        }

//                                    }
//                                }
//                            }
//                            break;
//                    }

//                });
//            }
//        }

//    }
//});

$(document).ready(function () {

    if (master_LobId == IFMLOBEnum.FAR.LobId) {

        var StateArray = [];
        var CountyObject = {};

        //Check Mine Sub options on page load
        callMineSubData()

        // JavaScript source code - Call MS Updates when these change
        $('[id*=txtZipCode],[id*=ddStateAbbrev],[id*=txtGaragedCounty],[id*=dd_Residence_CoverageForm]').on('change', function () {
            var FirstLocationElement = getEveryMineSubElementForALocation($(this)).first();
            if (AreILManditoryCountiesUnselectedSubOptions(FirstLocationElement)) {
                // Do Nothing because Manditory Counties have been removed;
            }
            else {
                if (IsIlManditory(FirstLocationElement)) { //Is IL Manditory County?
                    checkAllMineSubElements(FirstLocationElement, true);
                }
            }
            callMineSubFunctions(true)

        });

        // JavaScript source code - On MS Selections with Confirmation
        $('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]').on('change', function () {
            var mineSubElement = $(this);
            var mineSubState = getLocationStateValue(mineSubElement);
            var mineSubCounty = getLocationCountyText(mineSubElement);

            if (mineSubState == '36') {
                return;
            }

            if (!mineSubElement.prop('checked')) { //unchecked
                if (!ConfirmFarmDialog()) { //Cancel
                    if (mineSubState == '15') {
                        decideOnIlMineSub(mineSubElement, true)
                    }
                    else {
                        mineSubElement.prop("checked", true);
                    }
                }
                else { //OK
                    switch (mineSubState) {
                        case '15':
                            decideOnIlMineSub(mineSubElement, false)
                            break;
                    }
                }
            }
            else { //checked
                if (mineSubState == '15') {
                    decideOnIlMineSub(mineSubElement, true)
                }
            }
            callMineSubFunctions(true)
        });

        //Given any element under a location and this will return the value for the State Selection
        function getLocationStateValue(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=ddStateAbbrev]').val();
        }

        //Given any element under a location and this will return the value for the ZIP Code Text
        function getLocationZipText(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=txtZipCode]').val();
        }

        //Given any element under a location and this will return the value for the County Text
        function getLocationCountyText(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ctlProperty_Address]').find('[id*=txtGaragedCounty]').val();
        }

        //Given any element under a location and this will return the value for the Coverage Form Selection
        function getCoverageFormValue(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('[id*=ResidenceDiv]').find('[id*=dd_Residence_CoverageForm]').val();
        }

        function getLocationMineSubValue(element) {
            return getLocationMineSubElement(element).val();
        }

        function getLocationMineSubElement(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('input[id*=chkMineSubsidence]').first();
        }

        function getLocationAllMineSubElements(element) {
            return $(element).closest('[id*=dvFarmLocations]').find('input[id*=chkMineSubsidence]');
        }

        function getEveryMineSubElement(element) {
            return $(element).closest('[id*=dvFarmLocations]').find('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]');
        }
        function getEveryMineSubElementForALocation(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]');
        }

        function getBuildingMineSubElements(element) {
            return $(element).closest('[id*=farmLocationDataContainer]').find('input[id*=chkMineBuilding]');
        }

        function isBuildingNew(element) {
            return isBuildingNewElement(element).val() == "True";
        }

        function isBuildingNewElement(element) {
            return $(element).closest('[id*=divContents]').find('input[id*=isNewBuilding]').first()
        }

        //Makes MineSub selections by location
        function checkLocationMineSubElement(element, setting) {
            var locMineSub = getLocationMineSubElement(element);
            if (setting != false) {
                setting = true;
            }
            locMineSub.prop("checked", setting);
        }

        //Makes Minesub selections by building.
        function checkBuildingMineSubElements(element, setting) {
            var buildMineSubs = getBuildingMineSubElements(element);
            if (setting != false) {
                setting = true;
            }
            buildMineSubs.each(function () {
                $(this).prop("checked", setting);
                if (setting == false) {
                    if (isBuildingNewElement($(this))) {
                        isBuildingNewElement($(this)).val("False");
                    }
                }
            })
        }

        //Sets Minesub choice by state
        function checkAllMineSubElements(element, setting) {
            checkLocationMineSubElement(element, setting);
            checkBuildingMineSubElements(element, setting);
        }

        //Sets Minesub choice by policy (instead of by state)
        function setPolicyMineSubOptions(element, setting) {
            var allMineSubs = getEveryMineSubElement(element);
            if (setting != false) {
                setting = true;
            }
            allMineSubs.each(function () {
                if ((IsIlManditory($(this)) && setting == true) || (getLocationStateValue($(this)) == "15" && setting == false)) {
                    checkAllMineSubElements($(this), setting);
                }
            });

        }

        function AreILManditoryCountiesUnselectedSubOptions(element) {
            var mineSubState = getLocationStateValue(element);
            var allMineSubs = getEveryMineSubElement(element);
            var thisLocationSubs = getEveryMineSubElementForALocation(element);
            var result = false

            //Remove the currently added Locations MineSubs because these aren't checked yet.
            thisLocationSubs.each(function () {
                allMineSubs = allMineSubs.not($(this))
            });

            allMineSubs.each(function () {
                if (IsIlManditory($(this))) {
                    result = !$(this).prop("checked");
                }
            });
            return result;
        }

        function AreILManditoryCountiesUnselectedInBuildings(element) {
            var buildingElements = getEveryMineSubElement(element);
            var result = 0;
            buildingElements.each(function () {
                if (IsIlManditory($(this))) {
                    if (!$(this).prop("checked")) {
                        result = result + 1;
                    }
                }
            });
            return result > 1;
        }

        //Checks if IL is Manditory
        function IsIlManditory(element) {
            var mineSubElement = $(element);
            var mineSubState = getLocationStateValue(mineSubElement);
            var mineSubCounty = getLocationCountyText(mineSubElement);
            getMineSubStates()
            StateArray.forEach(function (item) {
                if (CountyObject == undefined || CountyObject.item == undefined || CountyObject[item].length == 0) {
                    ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(item, function (data) {
                        CountyObject[item] = data;
                        getMineSubStates() //Must call this again, because after AJAX return StateArray is empty
                    });
                }
            });
            if (mineSubState == "15") {
                if (CountyObject[mineSubState].contains(mineSubCounty)) {
                    return true;
                }
            }
            return false;
        }


        //Decides how to handle Mine sub based of IL
        function decideOnIlMineSub(element, setting) {
            if (IsIlManditory(element)) { //Is IL Manditory County checked
                setPolicyMineSubOptions(element, setting)
            }
            else { //Is Non-Manditory IL County checked
                if (AreILManditoryCountiesUnselectedSubOptions(element)) {
                    alert("Mine Subsidence coverage cannot be added to this location because it has been rejected in a mandatory county.");
                    $(element).prop("checked", false);
                }
                else {
                    checkAllMineSubElements(element, setting);
                }
            }
        }

        //Calls all the functions for Mine Sub
        function callMineSubFunctions(isLoading) {
            checkMineSubOnLocation(isLoading);
            checkMineSubOnBuilding(isLoading);
            handleNonMineSubElements(isLoading);
        }

        //Gets all mine sub counties for states on the policy and calls the mineSub functions
        //Don't run this for OHIO since it is all in code behind.
        function callMineSubData(mineSubState, doUpdate) {
            if (typeof mineSubState == "undefined" || mineSubState == '16' || (mineSubState == '15' && doUpdate == true)) {
                getMineSubStates()
                StateArray.forEach(function (item) {
                    if (CountyObject == undefined || CountyObject.item == undefined || CountyObject[item].length == 0) {
                        ifm.vr.vrdata.MineSubsidence.GetMineSubsidenceCapableCountyNamesByStateId(item, function (data) {
                            CountyObject[item] = data;
                            getMineSubStates() //Must call this again, because after AJAX return StateArray is empty
                            if (Object.keys(CountyObject).length == StateArray.length) {
                                //callMineSubFunctions()
                                if (typeof mineSubState == "undefined") {
                                    //The page is loading. Don't force checkboxes
                                    callMineSubFunctions(true)
                                }
                                else {
                                    callMineSubFunctions()
                                }
                            }
                        });

                    }
                });
            }

        }

        //gets the distinct States
        function getMineSubStates() {
            StateArray = [];
            $('[id*=ddStateAbbrev]').each(function () {
                if (StateArray.contains($(this).val()) == false) {
                    StateArray.push($(this).val());
                }
            });
        }

        // JavaScript source code - checkMineSubOnLocation
        function checkMineSubOnLocation(isLoading) {
            $('input[id*=chkMineSubsidence]').each(function () {
                var mineSubElement = $(this);
                var mineSubState = getLocationStateValue(mineSubElement);
                var mineSubCounty = getLocationCountyText(mineSubElement);
                var mineSubCoverage = getCoverageFormValue(mineSubElement);
                // OHIO is in code behind
                if (mineSubState != "36") {
                    mineSubElement.closest('[id*=dvMineSubsidence]').hide();
                    mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceReqHelpInfo]').hide();
                    mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').hide();
                }
                switch (mineSubState) {
                    case '15':
                        if (multiStateEnabled == true) {
                            mineSubElement.closest('[id*=dvMineSubsidence]').show();
                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
                                if (!isLoading) {
                                    mineSubElement.prop("checked", true);
                                }
                                mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceReqHelpInfo]').show();
                                mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceReqHelpInfo_OH]').hide();
                            }
                            else {
                                //for added non-mine building after mine sub checked on other non-mine building
                                if (mineSubElement.prop("checked")) {
                                    mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').show();
                                }
                            }
                        }
                        break;
                    case '16':
                        if (multiStateEnabled == true && mineSubCoverage !== "17") {
                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
                                mineSubElement.closest('[id*=dvMineSubsidence]').show();
                            }
                        }
                        else {
                            if (mineSubCoverage == "17") {
                                mineSubElement.prop("checked", false);
                                mineSubElement.prop("disabled", false);
                            }
                        }
                        break;
                }
            });
        }

        // JavaScript source code - checkMineSubOnBuilding
        function checkMineSubOnBuilding(isLoading) {
            $('input[id*=chkMineBuilding]').each(function () {
                var mineSubElement = $(this);
                var mineSubState = getLocationStateValue(mineSubElement);
                var mineSubCounty = getLocationCountyText(mineSubElement);
                var MineSubLocation = getLocationMineSubElement(mineSubElement);
                // OHIO is in code behind
                if (mineSubState != "36") {
                    mineSubElement.closest('[id*=divMineSub]').hide();
                    mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubReqHelpInfo]').hide();
                    mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').hide();
                }
                if (multiStateEnabled == true) {
                    switch (mineSubState) {
                        case '15':
                            mineSubElement.closest('[id*=divMineSub]').show();
                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
                                if (!isLoading || MineSubLocation.prop("checked") || (isBuildingNew(mineSubElement) && AreILManditoryCountiesUnselectedInBuildings(mineSubElement) == false)) {
                                    mineSubElement.prop("checked", true);
                                }
                                mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubReqHelpInfo]').show();
                                mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubReqHelpInfo_OH]').hide();
                            }
                            else {
                                if (mineSubElement.prop('checked')) {
                                    mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').show();
                                }
                            }

                            break;
                        case '16':
                            if (CountyObject[mineSubState].contains(mineSubCounty)) {
                                mineSubElement.closest('[id*=divMineSub]').show();
                            }
                            break;
                    }

                }
            });
        }

        function handleNonMineSubElements(isLoading) {
            if (multiStateEnabled == true) {
                $('input[id*=chkMineSubsidence], input[id*=chkMineBuilding]').each(function () {
                    var mineSubElement = $(this);
                    var mineSubState = getLocationStateValue(mineSubElement);
                    var mineSubCounty = getLocationCountyText(mineSubElement);
                    var mineSubCoverageF04 = getCoverageFormValue(mineSubElement) == 17;
                    var test4 = mineSubElement[0].id.substring(90);
                    switch (mineSubState) {
                        case '15':
                            if (!CountyObject[mineSubState].contains(mineSubCounty)) {
                                if (mineSubElement.prop("checked") == false) {
                                    if (!isLoading) {
                                        checkAllMineSubElements(mineSubElement, false);
                                    }
                                    mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').hide();
                                    mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').hide();
                                }
                                else {
                                    if (!isLoading) {
                                        checkAllMineSubElements(mineSubElement, true);
                                    }
                                    mineSubElement.closest('[id*=dvMineSubsidence]').siblings('[id*=dvMineSubsidenceNotReqHelpInfo]').show();
                                    mineSubElement.closest('[id*=divMineSub]').siblings('[id*=divMineSubNotReqHelpInfo]').show();
                                }
                            }
                            break;
                    }

                });
            }
        }

    }
});

//This is all about showing or hiding the Farm Pollution Endorsement based on effective date of the policy. The Endorsement is available for policies effective on or after 10/1/2016
//I don't believe this actually gets used at the moment. I believe the code behind has everything covered.
//function ShowHideFarmPollutionUpdatedDDLOptions(ddlFarmPollutionClientID, UpdatedFarmPollutionEffectiveDate, ddlLiabTypeClientID) {
//    var displayErrorDateMessage = false;
//    ddlFarmPollution = document.getElementById(ddlFarmPollutionClientID);
//    if (ddlFarmPollution) {
//        var effDate = new Date($("[id*='hdnOriginalEffectiveDate']").val());
//        var FarmPollutionStartDate = new Date(UpdatedFarmPollutionEffectiveDate);
//        if (effDate >= FarmPollutionStartDate) {
//            if (ddlFarmPollution.length === 4) {
//                var opt1 = document.createElement("option");
//                var opt2 = document.createElement("option");

//                // Assign text and value to Option object
//                opt1.text = '250,000';
//                opt1.value = '54';
//                opt2.text = '500,000';
//                opt2.value = '390';

//                // Add an Option object to Drop Down/List Box
//                ddlFarmPollution.options.add(opt1);
//                ddlFarmPollution.options.add(opt1);
//            }
//        } else {
//            var selectedValue = ddlFarmPollution.options[ddlFarmPollution.selectedIndex].value;
//            for (var i = 0; i < ddlFarmPollution.length; ++i) {
//                if (ddlFarmPollution.options[i].value === '54' || ddlFarmPollution.options[i].value === '390') {
//                    if (ddlFarmPollution.options[i].value === selectedValue) {
//                        displayErrorDateMessage = true;
//                        ddlFarmPollution.selectedIndex = 3;
//                    }
//                    ddlFarmPollution[i].remove;
//                }
//            }
//        }
//    }
//    if (displayErrorDateMessage === true) {
//        var errorMessage = ""
//        ddlLiabType = document.getElementById(ddlLiabTypeClientID);
//        if (ddlLiabType) {
//            var liabTypeValue = ddlLiabType.options[ddlLiabType.selectedIndex].value
//            if (liabTypeValue = '1') {
//                //Personal
//                errorMessage += "Limited Farm Pollution - "
//            } else {
//                //Commercial
//                errorMessage += "Liability Enhancement Endorsement - "
//            }
//        }
//        errorMessage += "The values '250,000' and '500,000' can only be selected with an effective date on or after 10/1/2016. The value has been switched to '100,000'"
//        alert(errorMessage);
//    }
//}



//Family Medical Payments - Names
//CAH Bug 51959:Test 3.3.4-3.3.7 Family Medical Payment on Policy Level Coverage
function DisableAddNew(boolValue) {
    boolValue = typeof boolValue !== 'undefined' ? boolValue : false;
    var target = $('div[id*="dvFamMedPayAddNew"] a[id*="lnkAdd"]')
    $(target).prop("disabled", boolValue)
    if (boolValue) {
        $(target).hide();
    }
    else {
        $(target).show();
    }
}

function CheckForValue() {
    var MissingValues = false
    $('div[id*="divRecords"] input:text').each(function () {

        var test = $(this).val();

        if ($(this).val().trim().length < 1) {
            MissingValues = true;
            return false; //breaks jquery each loop
        }
    });

    return MissingValues;
}

//Maintain on Endorsements - 01/29/2021 CAH - Bug 59434
function setEPLI(element, bool) {
    if (ifm.vr.currentQuote.isEndorsement == false) {
        document.getElementById(element).checked = bool;
    }
}

// Initial Page load.
$(function () {

    DisableAddNew(CheckForValue());

});


$(function () {
    // Text Change
    $('div[id*="divRecords"]').on('change', function (event) {
        DisableAddNew(CheckForValue());
    });
});

//Added 9/19/2022 for bug 76748 MLW
function locationStateChanged(ddStateAbbrev) {
    var stateId = $('#' + ddStateAbbrev);
    if (stateId) {
        var st = stateId.val();
        if (st != '16') {
            stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "dvCosmeticDamageExclusion"]').show();
            stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "divBuildingList"]').find('[id *= "dvCosmeticDamageExclusion"]').attr('style', 'margin-left:20px;');
            stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "dvCosmeticDamageExclusionData"]').hide();
        }
        else {
            stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "dvCosmeticDamageExclusion"]').hide();
            stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "dvCosmeticDamageExclusionData"]').hide();
        }
        stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "chkCosmeticDamageExclusion"]').prop("checked", false);
        stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "chkCDEExteriorDoorAndWindowSurfacing"]').prop("checked", false);
        stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "chkCDEExteriorWallSurfacing"]').prop("checked", false);
        stateId.closest('[id *= "farmLocationDataContainer"]').find('[id *= "chkCDERoofSurfacing"]').prop("checked", false);
    }
}

//Added 10/5/2022 for bug 75312 MLW
function DisableCoverageFormFO4(dd_Residence_CoverageForm) {
    $("#" + dd_Residence_CoverageForm + " option:contains('FO-4')").attr('disabled', 'disabled');
}

// CAH 67992 - Glass Breakage For Cabs - Increased Limit
function UpdateGlassCab(includeElement, increaseElement, totalElement) {
    var numberIncrease = +$("#" + increaseElement).val().replace(",", "");
    var numberInclude = +$("#" + includeElement).val().replace(",", "");

    numberIncrease = Math.ceil(numberIncrease / 100) * 100;

    $("#" + increaseElement).val(ifm.vr.stringFormating.asNumberWithCommas(numberIncrease))
    $("#" + totalElement).val(ifm.vr.stringFormating.asNumberWithCommas(numberIncrease + numberInclude));
}

function UpdateGenericPersonalPropertyControl(includeElement, increaseElement, totalElement) {
    var numberIncrease = +$("#" + increaseElement).val().replace(",", "");
    var numberInclude = +$("#" + includeElement).val().replace(",", "");

    numberIncrease = Math.ceil(numberIncrease / 100) * 100;

    $("#" + increaseElement).val(ifm.vr.stringFormating.asNumberWithCommas(numberIncrease))
    $("#" + totalElement).val(ifm.vr.stringFormating.asNumberWithCommas(numberIncrease + numberInclude));
}
