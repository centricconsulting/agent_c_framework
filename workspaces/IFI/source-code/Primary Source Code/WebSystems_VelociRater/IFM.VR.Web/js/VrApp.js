

///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />

function ShowAccountBillAvailText(ddPayPlanId, divAccountBillAvailTextId) {
    var accountBillAvailRow = document.getElementById(divAccountBillAvailTextId);
    if (accountBillAvailRow) {
        if ($("#" + ddPayPlanId + " option:contains('MONTHLY')").prop('selected') == true || $("#" + ddPayPlanId + " option:contains('RENEWAL EFT MONTHLY')").prop('selected') == true || $("#" + ddPayPlanId + " option:contains('EFT MONTHLY')").prop('selected') == true) {
            accountBillAvailRow.style.display = '';
        } else {
            accountBillAvailRow.style.display = 'none';       
        } 
    }
}

function BilltoMethodChanged(ddMethodId, ddPayPlanId, ddBillToId, divBillingInfoAddressId, isQuoteEndorsement) {
    if ($("#" + ddMethodId + " option:contains('AGENCY BILL')").prop('selected')) //if ($("#" + ddMethodId).val() == "1")
    {
        //agency bill
        $("#" + divBillingInfoAddressId).hide();// can no show
        $("#" + ddBillToId + " option:contains('AGENT')").prop('selected', true); // only billTo that is available
        $("#" + ddBillToId + " option:contains('AGENT')").removeAttr('disabled');
        $("#" + ddBillToId + " option:contains('INSURED')").attr('disabled', 'disabled');
        $("#" + ddBillToId + " option:contains('OTHER')").attr('disabled', 'disabled');
        $("#" + ddBillToId + " option:contains('MORTGAGEE')").attr('disabled', 'disabled'); // Only here on HOM

        if ($("#" + ddPayPlanId + " option:contains('MONTHLY')").prop('selected') == true || $("#" + ddPayPlanId + " option:contains('RENEWAL EFT MONTHLY')").prop('selected') == true || $("#" + ddPayPlanId + " option:contains('EFT MONTHLY')").prop('selected') == true) {
            // neither of these options are avaiable will agenyc bill
            $("#" + ddPayPlanId + " option[selected='selected']").removeAttr('selected');
            $("#" + ddPayPlanId + " option:first").prop('selected', true);
        }

        // disable unavailable pay plans
        $("#" + ddPayPlanId + " option:contains('MONTHLY')").attr('disabled', 'disabled');
        $("#" + ddPayPlanId + " option:contains('RENEWAL EFT MONTHLY')").attr('disabled', 'disabled');
        $("#" + ddPayPlanId + " option:contains('EFT MONTHLY')").attr('disabled', 'disabled');
        $("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").attr('disabled', 'disabled');

        if (master_LobId === "3") {
            if ($("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").prop('selected') === true) {
                $("#" + ddPayPlanId + " option[selected='selected']").removeAttr('selected');
                $("#" + ddPayPlanId + " option:first").prop('selected', true);
            }
            $("#" + divBillingInfoAddressId).hide();
            $("#" + eftBillingDivId).hide(); $("#divEFTInfoSub").hide();
        }
        //Added 7/31/2019 for bug 39576 MLW
        // RCC
        if ($("#" + ddPayPlanId + " option:contains('CREDIT CARD')").prop('selected')) {
            //Updated 8/21/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
            if (isQuoteEndorsement == "True") {
                $("#divRccPayPlanText").show();
                $("#divRccReminderText").hide();
            } else {
                $("#divRccReminderText").show();
                $("#divRccPayPlanText").hide();
            }
            //$("#divRccReminderText").show();
        }
        else {
            $("#divRccReminderText").hide();
            $("#divRccPayPlanText").hide();
        }
    }

    else {
        // Direct Bill

        $("#" + ddBillToId + " option:contains('AGENT')").attr('disabled', 'disabled'); // not available on direct bill
        //agent billto is disabled - if it is selected then change to blank
        if ($("#" + ddBillToId + " option:contains('AGENT')").prop('selected')) {
            $("#" + ddBillToId + " option:contains('AGENT')").prop('selected', false);
            $("#" + ddBillToId + " option:first").prop('selected', true); //default to blank
        }

        if ($("#" + ddBillToId + " option:contains('OTHER')").prop('selected'))
        {
            $("#" + divBillingInfoAddressId).show();
            $("#" + divBillingInfoAddressId).children('div').removeAttr('style');
        }
        else
        { $("#" + divBillingInfoAddressId).hide(); }

        // eftBillingDivId - Set via code behind
        if ($("#" + ddPayPlanId + " option:contains('RENEWAL EFT MONTHLY')").prop('selected') || $("#" + ddPayPlanId + " option:contains('EFT MONTHLY')").prop('selected'))
        {
            $("#" + eftBillingDivId).show();
            if ($("#" + eftAgreeCheckId).is(':checked'))
            {
                $("#divEFTInfoSub").show();
            }
            else
            { $("#divEFTInfoSub").hide(); }
        }
        else
        { $("#" + eftBillingDivId).hide(); $("#divEFTInfoSub").hide(); }

        // RCC
        if ($("#" + ddPayPlanId + " option:contains('CREDIT CARD')").prop('selected')) {
            //Updated 8/21/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
            if ((isQuoteEndorsement == "True") && (master_LobId == "2" || master_LobId == "17" || master_LobId == "3")) {
                $("#divRccPayPlanText").show();
                $("#divRccReminderText").hide();
           } else {
                $("#divRccReminderText").show();
                $("#divRccPayPlanText").hide();
            }
            //$("#divRccReminderText").show();
        }
        else
        {
            $("#divRccReminderText").hide();
            $("#divRccPayPlanText").hide();
        }


        

        $("#" + ddPayPlanId + " option:contains('MONTHLY')").removeAttr('disabled');
        $("#" + ddPayPlanId + " option:contains('RENEWAL EFT MONTHLY')").removeAttr('disabled');
        $("#" + ddPayPlanId + " option:contains('EFT MONTHLY')").removeAttr('disabled');

        if (master_LobId == "2" | master_LobId == "17" | master_LobId == "3" | master_LobId == "25")//if (hasMortgageeTypeAI & master_LobId == "2")
        {
            $("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").attr('title', 'Only available on policies with a Bill To of type Mortgagee');
            $("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").removeAttr('disabled');
        }
        else
        {
            $("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").attr('title', 'Only available on policies with a Additional Interest/Insured of type Mortgagee');
            $("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").prop('selected', false);
            $("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").attr('disabled', 'disabled');
        }

        switch (master_LobId)
        {
            case "17": // Farm same as Home
            case "2": // home
            case "3": // DFR
            case "25": //BOP
                // if there is an AI as First Mortgagee() then Annual MTG(23) is required
                if ($("#" + ddPayPlanId + " option:contains('ANNUAL MTG')").prop('selected'))
                {
                    $("#" + ddBillToId + " option:contains('INSURED')").attr('disabled', 'disabled');
                    $("#" + ddBillToId + " option:contains('OTHER')").attr('disabled', 'disabled');

                    $("#" + ddBillToId + " option:contains('MORTGAGEE')").removeAttr('disabled'); // only here on HOM
                    $("#" + ddBillToId + " option:contains('MORTGAGEE')").prop('selected', true);
                    if (master_LobId === "3") {
                        $("#" + divBillingInfoAddressId).hide();
                    }
                }
                else
                {
                    $("#" + ddBillToId + " option:contains('INSURED')").removeAttr('disabled');
                    $("#" + ddBillToId + " option:contains('OTHER')").removeAttr('disabled');

                    $("#" + ddBillToId + " option:contains('MORTGAGEE')").attr('disabled', 'disabled');

                    // Mortgagee can not be selected so if it is then blank out select for billto
                    if ($("#" + ddBillToId + " option:contains('MORTGAGEE')").prop('selected')) {
                        $("#" + ddBillToId + " option:contains('MORTGAGEE')").prop('selected', false);
                        $("#" + ddBillToId + " option:first").prop('selected', true); //default to blank
                    }
                }
                break;
            default:
                $("#" + ddBillToId + " option:contains('INSURED')").removeAttr('disabled');
                $("#" + ddBillToId + " option:contains('MORTGAGEE')").removeAttr('disabled'); // only here on HOM
                $("#" + ddBillToId + " option:contains('OTHER')").removeAttr('disabled');

                $("#" + ddPayPlanId + " option:contains('MONTHLY')").removeAttr('disabled'); //re-enable if it was disabled prior
                $("#" + ddPayPlanId + " option:contains('RENEWAL EFT MONTHLY')").removeAttr('disabled'); //re-enable if it was disabled prior
                $("#" + ddPayPlanId + " option:contains('EFT MONTHLY')").removeAttr('disabled'); //re-enable if it was disabled prior

                break;
        }
    }
}

function EffDayChangeWarning(txtEftDeductionDate, eftDayValue) {
    var newDate = document.getElementById(txtEftDeductionDate).value;

    if (newDate && eftDayValue) {
        eftDayValue = parseInt(eftDayValue);
        newDate = parseInt(newDate);
        if (isNaN(eftDayValue) || isNaN(newDate)) {
            return;
        }

        if (eftDayValue !== newDate) {
            // Display a confirmation dialog
            var userConfirmed = confirm("Changing the draw date to something other than the effective date may result in 11 installments instead of 12.");

            if (userConfirmed) {
                // Update the previous date if user confirms
                document.getElementById(txtEftDeductionDate).value = newDate;
            } else {
                // Revert to the original date if user cancels
                document.getElementById(txtEftDeductionDate).value = eftDayValue;
            }
        }
    }
}

function AiBillToToggled(senderId, isQuoteEndorsement) {
    var isChecked = $("#" + senderId).is(':checked');

    if (isChecked) {
        for (ii = 0; ii < AIBillToCheckBoxClientIdArray.length; ii++) {
            if (AIBillToCheckBoxClientIdArray[ii] != senderId) {
                $("#" + AIBillToCheckBoxClientIdArray[ii]).removeAttr('checked');
            }
        }

        $("select[id*='ddMethod'] option:contains('AGENCY BILL')").attr('disabled', 'disabled');

        $("select[id*='ddPayPlan'] option:contains('ANNUAL')").attr('disabled', 'disabled');
        $("select[id*='ddPayPlan'] option:contains('SEMI ANNUAL')").attr('disabled', 'disabled');
        $("select[id*='ddPayPlan'] option:contains('QUARTERLY')").attr('disabled', 'disabled');
        $("select[id*='ddPayPlan'] option:contains('MONTHLY')").attr('disabled', 'disabled');
        $("select[id*='ddPayPlan'] option:contains('EFT MONTHLY')").attr('disabled', 'disabled');
        $("select[id*='ddPayPlan'] option:contains('RENEWAL EFT MONTHLY')").attr('disabled', 'disabled');

        $("select[id*='ddBillTo'] option:contains('INSURED')").attr('disabled', 'disabled');
        $("select[id*='ddBillTo'] option:contains('AGENT')").attr('disabled', 'disabled');
        $("select[id*='ddBillTo'] option:contains('OTHER')").attr('disabled', 'disabled');

        $("select[id*='ddMethod'] option:contains('DIRECT BILL')").prop('selected', true);
        $("select[id*='ddPayPlan'] option:contains('ANNUAL MTG')").prop('selected', true);
        $("select[id*='ddBillTo'] option:contains('MORTGAGEE')").prop('selected', true);

        $("select[id*='ddPayPlan'] option:contains('ANNUAL MTG')").removeAttr('disabled');
        $("select[id*='ddBillTo'] option:contains('MORTGAGEE')").removeAttr('disabled');

    } else {

        $("select[id*='ddMethod'] option:contains('AGENCY BILL')").removeAttr('disabled');

        $("select[id*='ddPayPlan'] option:contains('ANNUAL')").removeAttr('disabled');
        $("select[id*='ddPayPlan'] option:contains('SEMI ANNUAL')").removeAttr('disabled');
        $("select[id*='ddPayPlan'] option:contains('QUARTERLY')").removeAttr('disabled');
        $("select[id*='ddPayPlan'] option:contains('MONTHLY')").removeAttr('disabled');
        $("select[id*='ddPayPlan'] option:contains('EFT MONTHLY')").removeAttr('disabled');
        $("select[id*='ddPayPlan'] option:contains('RENEWAL EFT MONTHLY')").removeAttr('disabled');

        $("select[id*='ddBillTo'] option:contains('INSURED')").removeAttr('disabled');
        $("select[id*='ddBillTo'] option:contains('AGENT')").removeAttr('disabled');
        $("select[id*='ddBillTo'] option:contains('OTHER')").removeAttr('disabled');

        //Updated 02/12/2020 for Bug 43977 MLW
        if (isQuoteEndorsement === "True") {
            $("select[id*='ddMethod'] option:contains('DIRECT BILL')").prop('selected', true);
        } else {
            $("select[id*='ddMethod'] option:first").prop('selected', true);
        }
        $("select[id*='ddPayPlan'] option:first").prop('selected', true);
        $("select[id*='ddBillTo'] option:first").prop('selected', true);

    }
}

//UW Questions Description Box max character check
function CheckMaxText(textarea, maxLength) {
    var len = textarea.value.length;
    var char = maxLength - len;
    var descTextId = $(textarea).attr("id") + "_warning";    
    if (len >= maxLength) {
        $('[id$=btnSubmit]').attr('disabled', 'disabled');       
        $(textarea).parent().find('textarea').attr('style', 'border: 1px solid red;');
        $(textarea).parent().find('textarea').attr('disabled', 'disabled');
        $(textarea).parent().find('span')[0].innerText = "Maximum of " + maxLength + " characters exceeded";
        $(textarea).parent().find('span').first().css("color", "red");
        $(textarea).parent().find('textarea').focus()
        $(textarea).parent().find('textarea').attr('disabled', false);        
        $(textarea).parent().find('textarea').keydown(function (e) {
            if (e.keyCode == 8 || char < maxLength)
                $('[id$=btnSubmit]').removeAttr('disabled');
        })
    }    
       else {
            var char = maxLength - len;
            $('[id$=btnSubmit]').removeAttr('disabled');
            $(textarea).parent().find('textarea').first().attr('style', 'border: none;');
            $(textarea).parent().find('span')[0].innerText = char + " characters remaining";
            $(textarea).parent().find('span').first().css("color", "black");
        }   
}

function CheckMaxTextFarm(textarea, maxLength) {
    var len = textarea.value.length;
    var char = maxLength - len;
    var descTextId = $(textarea).attr("id") + "_warning";
    var descText = $(textarea).parent().find('#' + descTextId);
    if (len >= maxLength) {
        $('[id$=btnCancel]').attr('disabled', 'disabled');  // actually for disable Save button       
        $(textarea).parent().find('textarea').attr('disabled', 'disabled');
        $(textarea).attr('style', 'border: 1px solid red;');        
        var countText = "Maximum of " + maxLength + " characters exceeded<br />";
        if (descText.length > 0) {
            $(descText).first().innerText = countText;            
        } else {           
            $(textarea).parent().prepend("<div style='color:red' id='" + descTextId + "'> " + countText + " <div>");           
        }        
        $(textarea).parent().find('textarea').focus()
        $(textarea).parent().find('textarea').attr('disabled', false);
        $(textarea).parent().find('textarea').keydown(function (e) {
            if (e.keyCode == 8 || char < maxLength)
                $('[id$=btnCancel]').removeAttr('disabled');
        })
    }       
    else {
        $('[id$=btnCancel]').removeAttr('disabled');
        $(descText).first().remove();
        $(textarea).parent().find('textarea').first().attr('style', 'border: none;');       
        $(textarea).first().css("color", "black");
    }
}

function AiTypeChanged(senderId, billToCheckBoxId) {
    var AiTypeId = $("#" + senderId).val();
    if (AiTypeId == "42" | AiTypeId == "11" | AiTypeId == "15") {
        $("#" + billToCheckBoxId).prop("disabled", false);
    }
    else {
        //  not valid to be checked so disable and uncheck
        $("#" + billToCheckBoxId).removeAttr('checked');
        $("#" + billToCheckBoxId).prop("disabled", true);
    }
}

function GetBankNameFromRoutingNumber(assignValuetoThisElementId,routingNumber)
{
    VRData.RoutingNumber.GetBankNameFromRoutingNumber(routingNumber, function (data) {
        if (data != null)
            $("#" + assignValuetoThisElementId).text(data);
        else
            $("#" + assignValuetoThisElementId).text("");
    });
}





