<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomeCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlHomeCoverages" %>

<%@ Register Src="~/User Controls/QuoteEdit/ctlIMRVWatercraft.ascx" TagPrefix="uc1" TagName="ctlIMRVWatercraft" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/ctlHomSectionCoverages.ascx" TagPrefix="uc1" TagName="ctlHomSectionCoverages" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>


<script type="text/javascript">

    $(document).ready(function () {
        var formTypeId = $("#<%=Me.hiddenFormTypeId.ClientID%>").val();
        //Added 12/2018 for HOM Upgrade MLW - updated 1/16/18 with js methods from Dan MLW
        if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
            showHideSections();
            $('#<%=Me.ddlPersonalLiability.ClientID%>').change(function () {
                showHideSections();
            });
            $('#<%=Me.ddlMedicalPayments.ClientID%>').change(function () {
                showHideSections();
            });
        }     

        // show/hide Section II and Section I & II if PersonalLiability is N/A or Medical Payments is N/A - Added 12/2018 for HOM Upgrade MLW
        function showHideSections() {
            var ddPL = $("#<%=Me.ddlPersonalLiability.ClientID%> option:selected").text();
            var ddMP = $("#<%=Me.ddlMedicalPayments.ClientID%> option:selected").text();
            if (ddPL == 'N/A' || ddMP == 'N/A') {
                $("[id*=divNASectionII]").show();
                $("[id*=divSectionII]").hide();
                $("[id*=divNASectionIandII]").show();
                $("[id*=divSectionIandII]").hide();
            }
            else 
            {
                $("[id*=divNASectionII]").hide();
                $("[id*=divSectionII]").show();
                $("[id*=divNASectionIandII]").hide();
                $("[id*=divSectionIandII]").show();
            }
        }
    });


    // Calculate fields based on Dwelling
    function CalculateFromDwLimit() {
        var formName = $("#<%=Me.hiddenSelectedForm.ClientID%>").val();
        var formTypeId = $("#<%=Me.hiddenFormTypeId.ClientID%>").val();
        switch (formName) {
            case "HO-6": 
                // Round Dwelling Change to nearest 1,000
                var roundedDwChgLimit = RoundValue($("#<%=Me.txtDwellingChangeInLimit.ClientID%>").val().replace(/,/g, ''));
                roundedDwChgLimit = noNAN(roundedDwChgLimit);
                $("#<%=Me.txtDwellingChangeInLimit.ClientID%>").val(roundedDwChgLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenDwellingChange.ClientID%>").val(roundedDwChgLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                var dwLimitTotal = parseInt($("#<%=Me.txtDwLimit.ClientID%>").val().replace(/,/g, '')) + roundedDwChgLimit;
                dwLimitTotal = noNAN(dwLimitTotal);
                $("#<%=Me.txtDwellingTotalLimit.ClientID%>").val(dwLimitTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenDwellingTotal.ClientID%>").val(dwLimitTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                break;
            case "ML-2":
                // Round Dwelling to the nearest 1,000
                var roundedDwLimit = RoundValue($("#<%=Me.txtDwLimit.ClientID%>").val().replace(/,/g, ''));
                var dwChange = parseInt($("#<%=Me.txtDwellingChangeInLimit.ClientID%>").val().replace(/,/g, ''));
                roundedDwLimit = noNAN(roundedDwLimit);
                var dwTotal = roundedDwLimit + dwChange
                $("#<%=Me.txtDwLimit.ClientID%>").val(roundedDwLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtDwellingChangeInLimit.ClientID%>").val(dwChange.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtDwellingTotalLimit.ClientID%>").val(dwTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenDwellingLimit.ClientID%>").val(roundedDwLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenDwellingChange.ClientID%>").val(dwChange.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenDwellingTotal.ClientID%>").val(dwTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                // Private Structures, calculated at 10% of Dwelling Total
                var rpsLimit = dwTotal * .1;
                var rpsChange = parseInt($("#<%=Me.txtRPSChgInLimit.ClientID%>").val().replace(/,/g, ''));
                var rpsTotal = rpsLimit + rpsChange;
                rpsTotal = noNAN(rpsTotal);
                $("#<%=Me.txtRPSLimit.ClientID%>").val(rpsLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtRPSTotalLimit.ClientID%>").val(rpsTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenRPSLimit.ClientID%>").val(rpsLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenRPSTotal.ClientID%>").val(rpsTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                //Personal Property - Updated 12/19/17 for HOM Upgrade MLW - updated 1/16/18 with js methods from Dan MLW
                var ppLimit;
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && formTypeId == "22") {
                    //new form uses 70% of Dwelling Total - MLW //updated 4/10/18 for Bug 26086 MLW
                    ppLimit = dwTotal * .7;
                    ppLimit = Math.ceil(ppLimit / 1) * 1
                } else {
                    // Personal Property, calculated at 50% of Dwelling Total
                    ppLimit = dwTotal * .5;
                }
                
                var ppChange = parseInt($("#<%=Me.txtPPChgInLimit.ClientID%>").val().replace(/,/g, ''));
                var ppTotal = ppLimit + ppChange;
                ppTotal = noNAN(ppTotal);
                $("#<%=Me.txtPPLimit.ClientID%>").val(ppLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtPPTotalLimit.ClientID%>").val(ppTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenPPLimit.ClientID%>").val(ppLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenPPTotal.ClientID%>").val(ppTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                // Loss of Use, calculated at 20% of Dwelling Total
                var lossLimit = dwTotal * .2;
                var lossTotal = lossLimit + parseInt($("#<%=Me.txtLossChgInLimit.ClientID%>").val().replace(/,/g, ''));
                lossTotal = noNAN(lossTotal);
                $("#<%=Me.txtLossLimit.ClientID%>").val(lossLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtLossTotalLimit.ClientID%>").val(lossTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenLossLimit.ClientID%>").val(lossLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenLossTotal.ClientID%>").val(lossTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                //Added 2/12/18 for HOM Upgrade MLW
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                    //Personal Property Self Storage Coverage uses txtPPTotalLimit to determine the coverage included limit       
                    CalculatePPSSLimit();
                    //Personal Property at Other Residences coverage uses txtPPTotalLimit to determine the coverage included limit
                    CalculatePPORLimit();
                }
                break;
            default:
                // Round Dwelling to the nearest 1,000
                var roundedDwLimit = RoundValue($("#<%=Me.txtDwLimit.ClientID%>").val().replace(/,/g, ''));
                var dwChange = parseInt($("#<%=Me.txtDwellingChangeInLimit.ClientID%>").val().replace(/,/g, ''));
                roundedDwLimit = noNAN(roundedDwLimit);
                var dwTotal = roundedDwLimit + dwChange

                $("#<%=Me.txtDwLimit.ClientID%>").val(roundedDwLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtDwellingChangeInLimit.ClientID%>").val(dwChange.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtDwellingTotalLimit.ClientID%>").val(dwTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenDwellingLimit.ClientID%>").val(roundedDwLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenDwellingChange.ClientID%>").val(dwChange.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenDwellingTotal.ClientID%>").val(dwTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                // Private Structures, calculated at 10% of Dwelling Total
                var rpsLimit = dwTotal * .1;
                var rpsChange = parseInt($("#<%=Me.txtRPSChgInLimit.ClientID%>").val().replace(/,/g, ''));
                var rpsTotal = rpsLimit + rpsChange;
                rpsTotal = noNAN(rpsTotal);
                $("#<%=Me.txtRPSLimit.ClientID%>").val(rpsLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtRPSTotalLimit.ClientID%>").val(rpsTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenRPSLimit.ClientID%>").val(rpsLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenRPSTotal.ClientID%>").val(rpsTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));


                //Personal Property - Updated 12/19/17 for HOM Upgrade MLW - updated 1/16/18 with js methods from Dan MLW
                var ppLimit;
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24")) {
                    //new forms 2,3,5 use 70% of Dwelling Total - MLW //updated 4/10/18 for Bug 26086 MLW
                    ppLimit = dwTotal * .7;
                    ppLimit = Math.ceil(ppLimit/1)*1
                } else {
                    // Personal Property, calculated at 60% of Dwelling Total
                    ppLimit = dwTotal * .6;
                }
 
                var ppChange = parseInt($("#<%=Me.txtPPChgInLimit.ClientID%>").val().replace(/,/g, ''));
                var ppTotal = ppLimit + ppChange;
                ppTotal = noNAN(ppTotal);

                $("#<%=Me.txtPPLimit.ClientID%>").val(ppLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtPPTotalLimit.ClientID%>").val(ppTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenPPLimit.ClientID%>").val(ppLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenPPTotal.ClientID%>").val(ppTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                //Added 2/12/18 for HOM Upgrade MLW
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                    //Personal Property Self Storage Coverage uses txtPPTotalLimit to determine the coverage included limit       
                    CalculatePPSSLimit();
                    //Personal Property at Other Residences coverage uses txtPPTotalLimit to determine the coverage included limit
                    CalculatePPORLimit();
                }
                break;
        }
    }

    // Calculate fields based on Structure
    function CalculateFromRPSLimit() {
        // Round Structure Change to nearest 1,000
        var roundedRpsChgLimit = RoundValue($("#<%=Me.txtRPSChgInLimit.ClientID%>").val().replace(/,/g, ''));
        roundedRpsChgLimit = noNAN(roundedRpsChgLimit);
        $("#<%=Me.txtRPSChgInLimit.ClientID%>").val(roundedRpsChgLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        $("#<%=Me.hiddenRPSChange.ClientID%>").val(roundedRpsChgLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        var rpsLimitTotal = parseInt($("#<%=Me.txtRPSLimit.ClientID%>").val().replace(/,/g, '')) + roundedRpsChgLimit
        rpsLimitTotal = noNAN(rpsLimitTotal);
        $("#<%=Me.txtRPSTotalLimit.ClientID%>").val(rpsLimitTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        $("#<%=Me.hiddenRPSTotal.ClientID%>").val(rpsLimitTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }

    // Calculate fields based on Property
    function CalculateFromPPLimit() {
        var formName = $("#<%=Me.hiddenSelectedForm.ClientID%>").val();
        var formTypeId = $("#<%=Me.hiddenFormTypeId.ClientID%>").val();
        switch (formName) {
            case "HO-4":
            case "HO-6":
                // Round Property Limit to nearest 1,000
                var roundedPPLimit = RoundValue($("#<%=Me.txtPPLimit.ClientID%>").val().replace(/,/g, ''));
                roundedPPLimit = noNAN(roundedPPLimit);
                $("#<%=Me.txtPPLimit.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtPPTotalLimit.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenPPLimit.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenPPTotal.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                localStorage.setItem("CoverageCLimit", $("#<%=Me.hiddenPPTotal.ClientID%>").val());
                //Added 2/12/18 for HOM Upgrade MLW
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                    //Personal Property Self Storage Coverage uses txtPPTotalLimit to determine the coverage included limit       
                    CalculatePPSSLimit();
                    //Personal Property at Other Residences coverage uses txtPPTotalLimit to determine the coverage included limit
                    CalculatePPORLimit();
                }
                break;
            case "ML-4":
                // Round Property Limit to nearest 1,000
                var roundedPPLimit = RoundValue($("#<%=Me.txtPPLimit.ClientID%>").val().replace(/,/g, ''));
                roundedPPLimit = noNAN(roundedPPLimit);
                $("#<%=Me.txtPPLimit.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtPPTotalLimit.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenPPLimit.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenPPTotal.ClientID%>").val(roundedPPLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                // Loss of Use, calculated at 20% of Property Total
                var lossLimit = roundedPPLimit * .2;
                var lossChange = parseInt($("#<%=Me.txtLossChgInLimit.ClientID%>").val().replace(/,/g, ''));
                var lossTotal = lossLimit + lossChange;
                lossTotal = noNAN(lossTotal);
                $("#<%=Me.txtLossLimit.ClientID%>").val(lossLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.txtLossTotalLimit.ClientID%>").val(lossTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenLossLimit.ClientID%>").val(lossLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%=Me.hiddenLossTotal.ClientID%>").val(lossTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                //Added 2/12/18 for HOM Upgrade MLW
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                    //Personal Property Self Storage Coverage uses txtPPTotalLimit to determine the coverage included limit       
                    CalculatePPSSLimit();
                    //Personal Property at Other Residences coverage uses txtPPTotalLimit to determine the coverage included limit
                    CalculatePPORLimit();
                }
                break;
            default:
                // Round Property Change to the nearest 1,000
                var ppLimit = parseInt($("#<%=Me.txtPPLimit.ClientID%>").val().replace(/,/g, ''));
                $("#<%=Me.hiddenPPLimit.ClientID%>").val(ppLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                var roundedPPChg = RoundValue($("#<%=Me.txtPPChgInLimit.ClientID%>").val().replace(/,/g, ''));
                roundedPPChg = noNAN(roundedPPChg);
                $("#<%=Me.txtPPChgInLimit.ClientID%>").val(roundedPPChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenPPChange.ClientID%>").val(roundedPPChg.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                // Calculate Property Total
                var ppTotal = ppLimit + roundedPPChg;
                ppTotal = noNAN(ppTotal);
                $("#<%=Me.txtPPTotalLimit.ClientID%>").val(ppTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                $("#<%=Me.hiddenPPTotal.ClientID%>").val(ppTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                //Added 2/12/18 for HOM Upgrade MLW
                if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                    //Personal Property Self Storage Coverage uses txtPPTotalLimit to determine the coverage included limit       
                    CalculatePPSSLimit();
                    //Personal Property at Other Residences coverage uses txtPPTotalLimit to determine the coverage included limit
                    CalculatePPORLimit();
                }
                break;
        }
    }

    //Calculate personal property self storage included limit
    //added 2/12/18 for HOM Upgrade MLW
    function CalculatePPSSLimit() {
        var formTypeId = $("#<%=Me.hiddenFormTypeId.ClientID%>").val();
        if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
            var ppssCC = $("[id*=hdnCoverageCode]").val();
            var ppssCCid = $("[id*=hdnCoverageCode]").attr('id');
            var ppssIncludeLimitId = ppssCCid.replace("hdnCoverageCode", "txtIncludedLimit");
            var ppssIncreasedLimitId = ppssCCid.replace("hdnCoverageCode", "txtIncreaseLimit");
            if (ppssCC == "80260") {
                var PPLimit = $("#<%=Me.txtPPTotalLimit.ClientID%>").val();
                PPLimit = ifm.vr.stringFormating.asNumberNoCommas(PPLimit);
                var PPLimit10 = PPLimit * .1;
                if (PPLimit10 > 1000) {
                    PPLimit10 = PPLimit10.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $('#' + ppssIncludeLimitId).val(PPLimit10);
                    $('#' + ppssIncludeLimitId).val(PPLimit10);
                    $('#' + ppssIncreasedLimitId).change()
                } else {
                    $('#' + ppssIncludeLimitId).val('1,000');
                    $('#' + ppssIncreasedLimitId).change()
                }
            }
            
        }

    }

    //Calculate Personal Property at Other Residences included limit
    function CalculatePPORLimit() {
        var formTypeId = $("#<%=Me.hiddenFormTypeId.ClientID%>").val();
        if (doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
            var PPLimitId = $("#<%=Me.txtPPTotalLimit.ClientID%>");
            if (PPLimitId) {
                var PPLimitVal = PPLimitId.val();
                PPLimitVal = ifm.vr.stringFormating.asNumberNoCommas(PPLimitVal);
                var PPORIncreasedLimit = $('#' + txtId_IncreaseLimit_PersPropOtherRes);
                var PPORIncreasedLimitVal = 0;
                if (PPORIncreasedLimit) {
                    PPORIncreasedLimitVal = PPORIncreasedLimit.val();
                    if (PPORIncreasedLimitVal == "") {
                        PPORIncreasedLimitVal = 0;
                    }
                }
                PPORIncreasedLimitVal = ifm.vr.stringFormating.asNumberNoCommas(PPORIncreasedLimitVal);
                var PPLimit10 = PPLimitVal * .1;
                if (PPLimit10 > 1000) {
                    var totalPPORLimit = (parseInt(PPLimit10) + parseInt(PPORIncreasedLimitVal)).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    PPLimit10 = PPLimit10.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $('#' + txtId_IncludedLimit_PersPropOtherRes).val(PPLimit10);
                    $('#' + txtId_TotalLimit_PersPropOtherRes).val(totalPPORLimit);
                } else {
                    $('#' + txtId_IncludedLimit_PersPropOtherRes).val('1,000');
                    var totalPPORLimit = (1000 + parseInt(PPORIncreasedLimitVal)).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $('#' + txtId_TotalLimit_PersPropOtherRes).val(totalPPORLimit);
                }
            }
        }
    }

    // Calculate fields based on Loss of Use
    function CalculateFromLossLimit() {
        var lossLimit = $("#<%=Me.txtLossLimit.ClientID%>").val().replace(/,/g, '');
        lossLimit = noNAN(lossLimit);

        // Round Loss Change to nearest 1,000
        var roundedLossChgLimit = RoundValue($("#<%=Me.txtLossChgInLimit.ClientID%>").val().replace(/,/g, ''));
        roundedLossChgLimit = noNAN(roundedLossChgLimit);
        $("#<%=Me.txtLossChgInLimit.ClientID%>").val(roundedLossChgLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        $("#<%=Me.hiddenLossChange.ClientID%>").val(roundedLossChgLimit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        var lossLimitTotal = parseInt($("#<%=Me.txtLossLimit.ClientID%>").val().replace(/,/g, '')) + roundedLossChgLimit
        lossLimitTotal = noNAN(lossLimitTotal);
        $("#<%=Me.txtLossTotalLimit.ClientID%>").val(lossLimitTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

        $("#<%=Me.hiddenLossTotal.ClientID%>").val(lossLimitTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }

    //
    // Round a number up by 1,000
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

    //
    // Check for null value
    //
    function noNAN(value) {
        return isNaN(value) || !value ? 0 : value;
    }
</script>

<div id="dvCoverages" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblCoverageHdr" runat="server" Text="COVERAGES - "></asp:Label>
        <asp:Label ID="lblCoverageForm" runat="server"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearCoverages" runat="server" OnClientClick="var confirmed = confirm('Clear Page?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear ALL Coverages">Clear Page</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveCoverages" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="dvDeductible" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
            <h3>
                <asp:Label ID="lblDeductiblehDR" runat="server" Text="DEDUCTIBLES"></asp:Label>
                <span style="float: right;">
                    <asp:LinkButton ID="lnkClearDeductibles" runat="server" CssClass="RemovePanelLink" OnClientClick="var confirmed = confirm('Clear Deductibles?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" Style="margin-left: 20px" ToolTip="Clear Deductibles">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveDeductibles" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div id="dvDeductibleContent">
                <asp:Panel ID="pnlCoverages" runat="server" BorderStyle="None">
                    <table style="width: 100%">
                        <tr>
                            <td style="vertical-align: top">
                                <table style="width: 100%" id="tblCoveragesGeneralInfo">
                                    <tr>
                                        <td style="width: 150px">
                                            <asp:Label ID="lblPolicyDeduct" runat="server" Text="Policy"></asp:Label>
                                            <br />
                                            <asp:DropDownList ID="ddlDeductible" runat="server" Width="150px" TabIndex="0"></asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblWindHailDeduct" runat="server" Text="Wind/Hail"></asp:Label>
                                            <br />
                                            <asp:DropDownList ID="ddlWindHailDeductible" runat="server" Width="150px" TabIndex="1"></asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <%-- Its better to rename this panle name to pnlReplacementCC to make it generic--%>
                                <asp:Panel ID="pnlReplacementCC" runat="server">
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnReplacementCC" runat="server" Text="Replacement Cost Calculator" CssClass="StandardButtonWrap" Height="40px" Width="120px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblCalValue" runat="server" Text="Calculated Value" Font-Italic="true"></asp:Label>
                                                <br />
                                                <asp:Label ID="lblReplacementCCValue" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
        <div id="dvBaseCoverage" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
            <h3>
                <asp:Label ID="lblPolicyBaseCoverageHdr" runat="server" Text="POLICY BASE COVERAGES"></asp:Label>
                <span style="float: right;">
                    <asp:LinkButton ID="lnkClearBase" runat="server" OnClientClick="var confirmed = confirm('Clear Policy Base Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Base Coverages">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveBase" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <div id="dvBaseCoveragesContent">
                    <asp:Panel ID="pnlBaseCoverage" runat="server" BorderStyle="None">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 36%">&nbsp;</td>
                                <td>
                                    <table>
                                        <tr>
                                            <td align="center" style="width: 80px" class="CovTableColumn">
                                                <asp:Label ID="lblLimit" runat="server" Text="Limit" CssClass="CovTableHeaderLabel"></asp:Label>
                                            </td>
                                            <td align="center" style="width: 80px" class="CovTableColumn">
                                                <asp:Label ID="lblChange" runat="server" Text="Change" CssClass="CovTableHeaderLabel"></asp:Label>
                                            </td>
                                            <td align="center" style="width: 80px" class="CovTableColumn">
                                                <asp:Label ID="lblTotal" runat="server" Text="Total" CssClass="CovTableHeaderLabel"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCovA" runat="server">
                        <table style="width: 100%">
                            <tr id="trDwelling" runat="server">
                                <td style="width: 36%">
                                    <asp:Label ID="lblDwellingReq" runat="server" Text="*"></asp:Label>
                                    <asp:Label ID="lblDwelling" runat="server" Text="A - Dwelling"></asp:Label>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtDwLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                            </td>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtDwellingChangeInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                            </td>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtDwellingTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCovB" runat="server">
                        <table style="width: 100%">
                            <tr id="trStructures" runat="server">
                                <td style="width: 36%">
                                    <asp:Label ID="lblPvtStructures" runat="server" Text="B - Related Private Structures"></asp:Label>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtRPSLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                            </td>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtRPSChgInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                            </td>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtRPSTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCovC" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 36%">
                                    <asp:Label ID="lblPersPropReq" runat="server" Text="*" Visible="false"></asp:Label>
                                    <asp:Label ID="lblPersonalProp" runat="server" Text="C - Personal Property"></asp:Label>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtPPLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                            </td>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtPPChgInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                            </td>
                                            <td class="CovTableColumn">
                                                <asp:TextBox ID="txtPPTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCovD" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 36%">
                                    <asp:Label ID="lblLivingCost" runat="server" Text="D - Loss of Use"></asp:Label>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td id="tdLossLimit" runat="server" class="CovTableColumn" visible="false">
                                                <asp:TextBox ID="txtLossLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                            </td>
                                            <td id="tdLossChange" runat="server" class="CovTableColumn" visible="false">
                                                <asp:TextBox ID="txtLossChgInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                            </td>
                                            <td id="tdLossTotal" runat="server" class="CovTableColumn" visible="false">
                                                <asp:TextBox ID="txtLossTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: 100%">
                                        <tr>
                                            <td id="tdLossOfUse" runat="server">
                                                <asp:Label ID="lblLossOfUse" runat="server" Text="Actual Sustained Loss" CssClass="CovTableHeaderLabel"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="pnlCovEF" runat="server" BorderStyle="none" Width="500px">
                        <table id="tblCovEF" runat="server" style="width: 100%">
                            <tr>
                                <td colspan="3">
                                    <asp:Label ID="lblPrimaryResidenceLiab" runat="server" CssClass="informationalText" Visible="false" style="text-align:center;width:365px;" Text="Liability is extended from the Primary Residence. Make sure to add this location to the Primary Residence. It is not automatically added to the Primary Residence."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPersonalLiab" runat="server" Text="E - Personal Liability"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlPersonalLiability" runat="server" Width="150px"></asp:DropDownList>
                                </td>
                                <td id="PersonalLiabilitylimitTextHome" runat="server" Visible="false">
                                  <span>We require a Personal Liability limit of<br />$300,000 when quoting an umbrella.</span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblMedPymts" runat="server" Text="F - Medical Payments"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlMedicalPayments" runat="server" Width="150px"></asp:DropDownList>
                                </td>
                              <td></td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
                <asp:Panel ID="pnlBasicButtons" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:LinkButton ID="lnkSpecialLimitsOfLiability" runat="server" CssClass="CovTableHeaderLabel">Included Special Limits of Liability</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnSaveBase" runat="server" Text="Save Base Policy Coverages" CssClass="StandardSaveButton" />
                            </td>
                            <td>
                                <asp:Button ID="btnRateBase" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" />
                                <asp:Button TabIndex="33" ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
                                <asp:Button TabIndex="34" ID="btnViewGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Billing Information Page" />
                                <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" /><br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
        <uc1:ctlHomSectionCoverages runat="server" id="ctlHomSectionCoverages" />   
        <uc1:ctlIMRVWatercraft runat="server" id="ctlIMRVWatercraft" />

    </div>
    <asp:HiddenField ID="hiddenSelectedForm" runat="server" />
    <asp:HiddenField ID="hiddenFormTypeId" runat="server" /><%-- added 12/19/17 for HOM Upgrade MLW --%>

    <%--Adding hidden fields for each base coverage because it will not work correctly when the field is disabled and there is risk of the user
    changing the field if it is not disabled. So I will be using the hidden fields for calculation and storage to the DB--%>
    <asp:HiddenField ID="hiddenDwellingLimit" runat="server" />
    <asp:HiddenField ID="hiddenDwellingChange" runat="server" />
    <asp:HiddenField ID="hiddenDwellingTotal" runat="server" />
    <asp:HiddenField ID="hiddenRPSLimit" runat="server" />
    <asp:HiddenField ID="hiddenRPSChange" runat="server" />
    <asp:HiddenField ID="hiddenRPSTotal" runat="server" />
    <asp:HiddenField ID="hiddenPPLimit" runat="server" />
    <asp:HiddenField ID="hiddenPPChange" runat="server" />
    <asp:HiddenField ID="hiddenPPTotal" runat="server" />
    <asp:HiddenField ID="hiddenLossLimit" runat="server" />
    <asp:HiddenField ID="hiddenLossChange" runat="server" />
    <asp:HiddenField ID="hiddenLossTotal" runat="server" />

    <%--Tracking active panels--%>
    <asp:HiddenField ID="hiddenCoverage" runat="server" />
    <asp:HiddenField ID="hiddenDeductibles" runat="server" />
    <asp:HiddenField ID="hiddenBase" runat="server" />
    <asp:HiddenField ID="hiddenOptional" runat="server" />
    <asp:HiddenField ID="hiddenInlandMarine" runat="server" />
    <asp:HiddenField ID="hiddenRVWatercraft" runat="server" />
    <asp:HiddenField ID="hiddenRVWater" runat="server" />
</div>