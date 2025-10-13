
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
/// <reference path="vr.core.js" />



$(document).ready(function () {
    $("#div_EffectiveDate").dialog({
        title: "Verify Effective Date", width: 270, height: 150, autoOpen: false,
        open: function (type, data) { $(this).parent().appendTo("form"); }, close: function () { EnableFormOnSaveRemoves(); }
    });

    $("#div_blanketAcreage_warning").dialog({
        title: "Blanket Acreage", width: 350, height: 150, autoOpen: false,
        open: function (type, data) { $(this).parent().appendTo("form"); }, close: function () { EnableFormOnSaveRemoves(); }
    });

    // copy effective date
    setInterval("AppRateEffectiveDate.CopyEffectiveDate()", 100);
});

var AppRateEffectiveDate = new function () {

    //this.OpenEffectiveDatePopup = function () {
    //    $("#div_EffectiveDate").dialog("open");
    //    DisableFormOnSaveRemoves();
    //    $('#txtEffectiveDate').val('');
    //    $("#txtEffectiveDate_Copy").val('');
    //}

    // Added effective date parameter 6/3/21 MGB Task 52940
    this.OpenEffectiveDatePopup = function (effDt) {
        $("#div_EffectiveDate").dialog("open");
        DisableFormOnSaveRemoves();
        $('#txtEffectiveDate').val(effDt);
        $("#txtEffectiveDate_Copy").val(effDt);
    }

    this.btnEffectiveDone = function () {
        var validationMsg = EffectiveDateValidationMessageForAppRate();
        if (validationMsg && validationMsg.length > 0) {
            //var minEffDate = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDate");
            //$("#txtEffectiveDate").val(minEffDate.toString())
            //updated 5/17/2023
            var originalEffDate = dateValueForJQueryFieldSelector("#hdnAppOriginalEffectiveDate");
            if (originalEffDate) {
                $("#txtEffectiveDate").val(originalEffDate.toString())
            } else {
                var minEffDate = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDate");
                $("#txtEffectiveDate").val(minEffDate.toString())
            }
            return;
        }
        //******note 9/18/2015: still need to do something here
        var effectiveDate = $("#txtEffectiveDate_Copy").val();
        var blanketAcreageAvailableDate = dateValueForJQueryFieldSelector("#hdnBlanketAcreageAvailableDate");
        var hdnHasBlanketAcreage = $("#hdnHasBlanketAcreage").val();

        if (ifm.vr.vrDateTime.isModernDate(effectiveDate) === true) {
            if (hdnHasBlanketAcreage.trim().toUpperCase() === "TRUE" && ifm.vr.vrDateTime.isModernDate(blanketAcreageAvailableDate) === true && ifm.vr.vrDateTime.compareDateString(effectiveDate, blanketAcreageAvailableDate) === true) {
                this.OpenBlanketAcreageWarningPopup();
                return;
            }
            else {
                $("#btnFinalRate").click();
                $("#btnEffectiveDateDone").prop("disabled", true);
                $("#btnEffectiveDateDone").prop("value", "Rating...");
            }

        }
        else {
            ifm.vr.ui.FlashFocusThenScrollToElement($("#txtEffectiveDate").attr('id'));
        }
    }

    this.OpenBlanketAcreageWarningPopup = function () {
        $("#div_EffectiveDate").dialog("close");
        $("#div_blanketAcreage_warning").dialog("open");
        DisableFormOnSaveRemoves();
    }

    this.btnBlanketAcreageOk = function () {
        $("#btnFinalRate").click();
        $("#btnblanketAcreageWarningOk").prop("disabled", true);
        $("#btnblanketAcreageWarningOk").prop("value", "Rating...");
    }

    this.CopyEffectiveDate = function () {
        var effectiveDate = $("#txtEffectiveDate").val();
        effectiveDate = effectiveDate.trim();
        if (effectiveDate !== null && effectiveDate !== undefined && effectiveDate !== "")
            $("#txtEffectiveDate_Copy").val(effectiveDate);
        $("#cphMain_ctlTreeView_lblEffectiveDate").text(effectiveDate);
    }

    //added 9/17/2015 for effDate validation
    function EffectiveDateValidationMessageForAppRate() {
        var msg = "";
        var newDt = "";
        var setToNewDt = false;
        var isReset = false;

        var effDt = dateValueForJQueryFieldSelector("#txtEffectiveDate_Copy");
        if (effDt) {
            var minEffDate = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDate");
            var maxEffDate = dateValueForJQueryFieldSelector("#hdnAppMaximumEffectiveDate");
            if (minEffDate && maxEffDate) {
                if (isDateBetweenMinimumAndMaximumDates(effDt, minEffDate, maxEffDate) === true) {
                    //valid date; everything okay
                } else {
                    //quote date is not between minDate and maxDate
                    msg = "effective date must be between " + minEffDate + " and " + maxEffDate;
                    if (isDate1LessThanDate2(effDt, minEffDate) === true) {
                        var updateToMinDate = false;
                        var minQuoteDateIsGreaterThanAllDate = boolValueForJQueryFieldSelector("#hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes");
                        var minEffDtAllQuotes = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDateAllQuotes");
                        var quoteHasMinimunEffDt = boolValueForJQueryFieldSelector("#hdnAppQuoteHasMinimumEffectiveDate");
                        if (minQuoteDateIsGreaterThanAllDate === true) {
                            updateToMinDate = true;
                        } else if (minEffDate === minEffDtAllQuotes && quoteHasMinimunEffDt === true) {
                            updateToMinDate = true;
                        } else {
                            updateToMinDate = false; //redundant
                        }
                        if (updateToMinDate === true) {
                            var originalEffDate = dateValueForJQueryFieldSelector("#hdnAppOriginalEffectiveDate");
                            newDt = minEffDate;
                            setToNewDt = true;
                            if (isDate1EqualToDate2(originalEffDate, minEffDate) === true) {
                                msg = "Contact Underwriting for quotes before " + minEffDate + "; date reverted back to " + minEffDate + ".";
                                isReset = true;
                            } else {
                                msg = "Contact Underwriting for quotes before " + minEffDate + "; date set to " + minEffDate + ".";
                            }
                        } else {
                            //quote date is less than minDate
                            //can customize message but already defaulted above
                            var beforeDtMsg = $("#hdnAppBeforeDateMsg").val();
                            if (StringHasSomething(beforeDtMsg) == true) {
                                msg = appendText(msg, beforeDtMsg, "\n\n");
                            }
                        }
                    } else if (isDate1GreaterThanDate2(effDt, maxEffDate) === true) {
                        //quote date is more than maxDate
                        //can customize message but already defaulted above
                        var afterDtMsg = $("#hdnAppAfterDateMsg").val();
                        if (StringHasSomething(afterDtMsg) == true) {
                            msg = appendText(msg, afterDtMsg, "\n\n");
                        }
                    } else {
                        //shouldn't get here
                        //can customize message but already defaulted above
                    }
                }
            }
        } else {
            msg = "invalid effective date";
        }

        if (msg) {
            if (msg.length > 0) {
                alert(msg);
                if (newDt && setToNewDt) {
                    if (newDt.length > 0 && setToNewDt === true) {
                        //set calendar date to newDt
                        //******note 9/18/2015: still need to set calendar to new date here
                        if (isReset === true) {
                            //date reverted back to previous date; just a note since msg already reflects this
                        }
                    }
                }
            }
        }

        return msg;
    }

}; // AppRateEffectiveDate END

