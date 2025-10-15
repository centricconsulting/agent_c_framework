
///<reference path="VRStringHelpers.js" />
///<reference path="jquery-2.1.0-vsdoc.js" />
/// <reference path="vr.core.js" />


//// Replaced by String.prototype.toInt
//function TryParseInt(str) {
//    var retValue = 0;
//    str = FormatAsNumberNoCommaFormatting(str);
//    if (str !== null) {
//        if (str.length > 0) {
//            if (!isNaN(str)) {
//                retValue = parseInt(str);
//            }
//        }
//    }
//    return retValue;
//}

//// replaced by String.prototype.toFloat
//function TryParseFloat(str) {
//    var retValue = 0.0;
//    str = FormatAsNumberNoCommaFormatting(str);
//    if (str !== null) {
//        if (str.length > 0) {
//            if (!isNaN(str)) {
//                retValue = parseFloat(str);
//            }
//        }
//    }
//    return retValue;
//}

//// replaced by ifm.vr.ui.GetCurrentFocusedElementId
//function GetCurrentFocusedElementId()
//{
//    var focused = document.activeElement;
//    if (!focused || focused == document.body)
//        focused = null;
//    else if (document.querySelector)
//        focused = document.querySelector(":focus");
//    return $(focused).attr('id');
//}


function AddDivToAccordionList(divToAddId,divAccordID)
{
    var homClueLinkDiv = $('#' + divToAddId);
    homClueLinkDiv.detach();
    $('#' + divAccordID + ' h3:first').before(homClueLinkDiv);
    //$(homClueLinkDiv).prependTo('#' + divAccordID + ' h3:first');
}

function HOMLossListClueReportLink(divToAddId, divAccordID, hasItems) {
    if (hasItems)
    {
        AddDivToAccordionList(divToAddId, divAccordID);
    }
    else
    {
        // move link to correct location
        var homClueLinkDiv = $('#' + divToAddId);
        homClueLinkDiv.detach();
        $(homClueLinkDiv).appendTo('#' + divAccordID);
    }
}


// replaced by ifm.vr.ui.FlashFocusThenScrollToElement
function DoValidationSummaryErrorJump(senderId) {
    $("#" + senderId).scrollTop();
    $("#" + senderId).focus();
    var originalBackgroundColor = $("#" + senderId).css("backgroundColor")
    $("#" + senderId).css("backgroundColor", "red").stop().animate({ backgroundColor: originalBackgroundColor }, 1200);
}

//replaced by ifm.vr.ui.DisableInputAreaAndTree
function DisableFormOnSaveRemoves() {
    EditModeaDiv("page", true);
}

//replaced by ifm.vr.ui.EnableInputAreaAndTree
function EnableFormOnSaveRemoves() {
    EditModeaDiv("page", false);
}

//replaced by ifm.vr.ui.DisableEntirePage
function DisableMainFormOnSaveRemoves() {
    EditModeaDiv("main", true);
}

// replaced by wrappers
function EditModeaDiv(divId, apply) {
    var $somediv = $('#' + divId),
    pos = $.extend({
        width: $somediv.outerWidth(),
        height: $somediv.outerHeight()
    }, $somediv.position());

    if (apply) {
        // if overlay does not exist already
        if ($("#" + divId + "_overlay").length == 0) {
            // create new div over tree to stop button clicks
            $('<div>', {
                id: divId + "_overlay",
                title: 'Disabled until the current activity is completed.',
                css: {
                    position: 'absolute',
                    top: pos.top,
                    left: pos.left,
                    width: pos.width,
                    height: pos.height,
                    backgroundColor: '#000'
                    , opacity: 0.0
                }
            }).appendTo($somediv);
        }
        $("#" + divId).addClass('disabledTree');
    }
    else {
        $("#" + divId + "_overlay").remove();
        $("#" + divId).removeClass('disabledTree');
    }
}

function DoAddressWarning(ShowAddressWarning, divId, txtstreetnumId, txtstreetnameId, txtpoboxId) {
    if (ShowAddressWarning) {
        $("#" + divId).stop().fadeIn('slow');

        var originalBackgroundColor = "lightblue";
        $("#" + txtstreetnumId).css("backgroundColor", "lightblue");
        $("#" + txtstreetnameId).css("backgroundColor", "lightblue");
        $("#" + txtpoboxId).css("backgroundColor", "lightblue");
    }
    else {
        $("#" + divId).stop().fadeOut('fast');

        var originalBackgroundColor = "white";
        $("#" + txtstreetnumId).css("backgroundColor", "white");
        $("#" + txtstreetnameId).css("backgroundColor", "white");
        $("#" + txtpoboxId).css("backgroundColor", "white");
    }
}

//replaced by ifm.vr.ui.SetActiveIndexOfAccordion
function SetActiveAccordionIndex(accordionId, index)
{
    $("#" + accordionId).accordion("option", "active", index);
}

//replaced by ifm.vr.ui.GetActiveIndexOfAccordion
function GetActiveAccordionIndex(accordionId) {
    return $("#" + accordionId).accordion("option", "active");
}

$.urlParam = function (name, url) {
    if (!url) {
        url = window.location.href;
    }
    var results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(url);
    if (!results) {
        return 0;
    }
    return results[1] || 0;
}

//replaced by ifm.vr.ui.StopEventPropagation
function StopEventPropagation(e) {
    if (!e)
        e = window.event;

    //IE9 & Other Browsers
    if (e.stopPropagation) {
        e.stopPropagation();
    }
        //IE8 and Lower
    else {
        e.cancelBubble = true;
    }
}

//replaced by ifm.vr.vrDateTime.isModernDate
//function IsModernDate(input) {
//    var status = false;
//    if (!input || input.length <= 0) {
//        status = false;
//    } else {
//        var result = new Date(input);
//        if (result == 'Invalid Date') {
//            status = false;
//        } else {
//            var year = result.getFullYear();
//            if (year > 1000)
//                status = true;
//        }
//    }
//    return status;
//}
//

// replaced by ifm.vr.vrDateTime.IsDate
function IsDate (input) {
    var status = false;
    if (!input || input.length <= 0) {
        status = false;
    } else {
        var result = new Date(input);
        if (result == 'Invalid Date') {
            status = false;
        } else {
            status = true;
        }
    }
    return status;
}

//replaced by ifm.vr.stringFormating.asDate
function dateFormat(val) {
    return val.replace(/^([\d]{2})([\d]{2})([\d]{4})$/, "$1/$2/$3");
}

//replaced by ifm.vr.stringFormating.asPostalCode
function formatPostalcode(pcode) {
    pcode = pcode.replace('-', '');
    if (pcode.length > 6) {
        if (pcode[6] == '-' || pcode[6] == ' ') {
            // look for both because it either then you are done
            pcode[6] = '-';
        }
        else {
            if (pcode.length >= 10) {
                // it doesn't have a - or a space so it has to many digits
                pcode = pcode;
            }
            else {
                var left = pcode.substring(0, 5);
                var right = pcode.substring(5, (pcode.length > 9) ? 9 : pcode.length);
                pcode = left + '-' + right;
            }
        }
    }

        return pcode;
}

//replaced by ifm.vr.stringFormating.asNumberWithCommas
function FormatAsNumber(nStr) {
    var Exclude = " ,[A-Za-z],$";
    nStr = nStr.replace(/,/gi, "");
    var removes = new Array();
    removes = Exclude.split(","); //split unwanted list
    for (var i = 0; i < removes.length; i++) {
        // '$' needs an escape \
        if (removes[i] == "$")
        { nStr = nStr.replace(/\$/g, ""); } // remove the unwanted char
        else {
            var regExp = new RegExp(removes[i]); // create dynamic regex
            nStr = nStr.replace(regExp, ""); // remove the unwanted char
        }
    }
    nStr += '';
    x = nStr.split('.');
    x1 = x[0]; // is left of the period
    x2 = x.length > 1 ? '.' + x[1] : ''; // is anything right of the period
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

//replaced by ifm.vr.stringFormating.asNumberNoCommas
function FormatAsNumberNoCommaFormatting(nStr) {
    Exclude = " ,[A-Za-z],$";
    nStr = nStr.replace(/,/gi, "");
    var removes = new Array();
    removes = Exclude.split(","); //split unwanted list
    var l = nStr.length; //1-1-15
    for (var bb = 0; bb < l; bb++) //1-1-15
    {
        for (var i = 0; i < removes.length; i++) {
            // '$' needs an escape \
            if (removes[i] == "$")
            { nStr = nStr.replace(/\$/g, ""); } // remove the unwanted char
            else {
                var regExp = new RegExp(removes[i]); // create dynamic regex
                nStr = nStr.replace(regExp, ""); // remove the unwanted char
            }
        }
    }
    return nStr;
}

//replaced by ifm.vr.stringFormating.asPositiveNumberNoCommas
function FormatAsPositiveNumberNoCommaFormatting(nStr) {
    Exclude = " ,[A-Za-z],$,-";
    nStr = nStr.replace(/,/gi, "");
    var removes = new Array();
    removes = Exclude.split(","); //split unwanted list
    var l = nStr.length; //1-1-15
    for (var bb = 0; bb < l; bb++) //1-1-15
    {
        for (var i = 0; i < removes.length; i++) {
            // '$' needs an escape \
            if (removes[i] == "$")
            { nStr = nStr.replace(/\$/g, ""); } // remove the unwanted char
            else {
                var regExp = new RegExp(removes[i]); // create dynamic regex
                nStr = nStr.replace(regExp, ""); // remove the unwanted char
            }
        }
    }
    return nStr;
}

//replaced by ifm.vr.stringFormating.asCurrency
function FormatAsCurrency(nStr) {
    var formatted = '$' + FormatAsNumber(nStr);
    if (formatted != '$') {
        x = formatted.split('.');
        x1 = x[0]; // is left of the period
        x2 = x.length > 1 ? '.' + x[1] : ''; // is anything right of the period
        var allowedDigits = 2 + 1; // add one for period
        if (x2.length > allowedDigits) {
            x2 = x2.substring(0, allowedDigits);
        }
        if (x2.length == 2)
            x2 = x2 + "0";
        return x1 + x2;
    }
    return "";
}

function FormatAsCurrencyCheckMaxLength(tb) {
    maxLength = tb.attr('maxLength');
    var formatted = FormatAsCurrency(tb.val());
    if (formatted.length > maxLength) {
        formatted = formatted.substring(0, maxLength)
    }
    return formatted;
}

//replaced by ifm.vr.stringFormating/FormatAsAlphabeticOnly
// Only works on a correct as you type event.
function FormatAsAlphabeticOnly(nStr)
{
    var matched = nStr.match(/[a-zA-Z ]+/);
    if (matched != null)
        return matched;
    else
        return "";
}

//replaced by ifm.vr.stringFormating.asAlphabeticNumeric
// Only works on a correct as you type event.
function FormatAsAlphabeticNumeric(nStr) {
    var matched = nStr.match(/[a-zA-Z0-9 ]+/);
    if (matched != null)
        return matched;
    else
        return "";
}

function FormatAsNumericDigitsOnly(nStr) {
    Exclude = " ,[A-Za-z],$,-";
    nStr = nStr.replace(/,/gi, "");
    var removes = new Array();
    removes = Exclude.split(","); //split unwanted list
    var l = nStr.length; //1-1-15
    for (var bb = 0; bb < l; bb++) //1-1-15
        {
        for (var i = 0; i < removes.length; i++) {
            // '$' needs an escape \
            if (removes[i] == "$")
            { nStr = nStr.replace(/\$/g, ""); } // remove the unwanted char
            else {
                var regExp = new RegExp(removes[i]); // create dynamic regex
                nStr = nStr.replace(regExp, ""); // remove the unwanted char
            }
        }
    }
    return nStr;
}


//replaced by ifm.vr.stringFormating.asCurrencyNoCents
// *******************   DO NOT USE in a format as you type   **********************
// because then what would have been $25.50 will turn into $2,550 !!!!!
function FormatAsCurrencyNoCents(nStr) {
    try
    {
        var formatted = '$' + FormatAsNumber(nStr);
        if (formatted != '$') {
            x = formatted.split('.');
            x1 = x[0]; // is left of the period
            return x1;
        }
    }
    catch(err)
    {}
    return "";
}

function ToggleHiddenField(elementID)
{
    // used on single pane jqueryui accordions
    var currentVal = $("#" + elementID).val();
    if (currentVal == "0")
    {
        $("#" + elementID).val("false");
    }
    else
    {
        $("#" + elementID).val("0");
    }
}

// Enable/Disable Vehicle Coverage Fields
function ToggleVehicleEnable(ddComp, ddTransport, txtMotor, chkTripInterrupt) {
    var compValue = $("#" + ddComp).val();
    var setTransport = $("#" + ddTransport).val();

    if (compValue == "0") {
        $("#" + ddTransport).prop("disabled", true);
        $("#" + txtMotor).prop("disabled", true);
        $("#" + chkTripInterrupt).prop("disabled", true);
        setTransport = "0";
    }
    else {
        $("#" + ddTransport).prop("disabled", false);
        $("#" + txtMotor).prop("disabled", false);
        $("#" + chkTripInterrupt).prop("disabled", false);
    }
}

// Show/Hide Vehicle Coverage Fields based on Vehicle Plan
function ToggleVehicleInformation(ddPolicy, pnlDisClaim, pnlComp, pnlColl, pnlTow, pnlTrans, pnlTravel, pnlRadio, pnlAV, pnlMedia, pnlLoan, pnlPolution, pnlMotor) {
    var vehPlan = $("#" + ddPolicy).val();
}

function updateInsuredHeaderText(headerId, driverNum, firstNameId, lastNameID,middleNameId,InsuredIndex,suffixId) {
    var firstNameText = $("#" + firstNameId).val();
    var middleNameText = $("#" + middleNameId).val();
    var lastNameText = $("#" + lastNameID).val();
    var suffixText = $("#" + suffixId).val();

    if (middleNameText == undefined) { middleNameText = ""; }
    if (lastNameText == undefined) { lastNameText = ""; }
    if (suffixText == undefined) { suffixText = "";}

    var newHeaderText = "Policyholder #" + driverNum + " - " + firstNameText + " " + lastNameText + " " + suffixText;

    var TreeText = firstNameText + " " + lastNameText + " " + suffixText;

    if (middleNameText != "") {
        newHeaderText = "Policyholder #" + driverNum + " - " + firstNameText + " " + middleNameText + " " + lastNameText + " " + suffixText;
        TreeText = firstNameText + " " + middleNameText + " " + lastNameText + " " + suffixText;
    }
    newHeaderText = newHeaderText.toUpperCase();
    //document.getElementById(headerId).setAttribute('value', newHeaderText);
    $("#" + headerId).val(newHeaderText);
    if (newHeaderText.length > 50)
    {
        $("#" + headerId).text(newHeaderText.substring(0,50) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }

    if (TreeText.length == 1) {
        TreeText = "Policyholder " + driverNum.toString();
    }
    TreeText = TreeText.toUpperCase();

    // The tree should define a variable that exposes this labels clientId - Matt A
    $("#cphMain_ctlTreeView_rptPolicyholders_lblPolicyholderDescription_" + InsuredIndex).text(TreeText);
    $("#cphMain_ctl_WorkflowManager_Quote_Farm_ctlTreeView_rptPolicyholders_lblPolicyholderDescription_" + InsuredIndex).text(TreeText);
}

function updateInlandMarineHeaderText(headerId, IMNum, IMIndex) {
    var newHeaderText = "Inland Marine #" + IMNum.toString();
    var TreeText = "Inland Marine #" + IMNum.toString();

    $("#" + headerId).val(newHeaderText);

    if (newHeaderText.length > 45) {
        $("#" + headerId).text(newHeaderText.substring(0, 45) + "...");
    }
    else {
        $("#" + headerId).text(newHeaderText);
    }

    if (TreeText.length == 1) {
        TreeText = "Inland Marine " + IMNum.toString();
    }

    $("#cphMain_ctlTreeView_rptInlandMarines_lblInlandMaroneDescription_" + IMIndex.toString()).text(TreeText);
}

function DoCityCountyLookup(zipcodeID,cityID_dd,cityID_txt,CountryID,StateId)
{
    var zipcode = $("#" + zipcodeID).val();
    if (zipcode.length == 5) {

        ifm.vr.vrdata.ZipCode.GetZipCodeInformation(zipcode, function (data) {
            //$("#" + zipcodeID).next().remove();
            
            $("select[id$=" + cityID_dd + "] > option").remove();
            if (data.length > 0) {
                var state = data[0].StateAbbrev;
                var stateEnabled = $('#' + StateId).is(':enabled');
                var currentStateSelection = $('#' + StateId).find(":selected").text();
                if (stateEnabled !== true && state !== currentStateSelection) {
                    alert('Zip Code must be in current state');
                    return;
                }
                $('#' + StateId + ':enabled').find("option:contains('" + state + "')").each(function () {
                    $(this).attr("selected", "selected");
                });               

                $("#" + CountryID).val(data[0].County.toUpperCase());

                for (var i = 0; i < data.length; i++) {
                    $("#" + cityID_dd).append($('<option></option>').val(data[i].City.toUpperCase()).html(data[i].City));
                }
                //$("#" + VinClientId).val(data[0].Vin);
                $("#" + cityID_txt).val(data[0].City);
                if (data.length > 1) {
                    //hide textbox
                    //
                    $("#" + cityID_dd).append($('<option></option>').val("0").html("-- OTHER --"));
                    $("#" + cityID_txt).hide();
                    $("#" + cityID_dd).show();
                }
                else {
                    // show textbox
                    $("#" + cityID_txt).show();
                    $("#" + cityID_dd).hide();
                }
                //Added 9/19/2022 for task 76748 MLW
                //master_LobId defined on VelociRater.Master
                if (master_LobId == IFMLOBEnum.FAR.LobId.toString() && currentStateSelection !== state) {
                    locationStateChanged(StateId);
                }               
            }
            else {
                // show textbox
                $("#" + cityID_txt).show();
                $("#" + cityID_dd).hide();
            }

            return data;
        });

      
    }
}

var streetNumClientID1;
var streetNameClientID1;
var aptNumClientID1;
var POBoxClientID1;
var CityClientID1;
var StateClientID1;
var ZipClientID1;
var CountyClientID1;
var streetNumClientID2;
var streetNameClientID2;
var aptNumClientID2;
var POBoxClientID2;
var CityClientID2;
var StateClientID2;
var ZipClientID2;
var CountyClientID2;

function CopyAddressFromPolicyHolder()
{
    $("#" + streetNumClientID2).val($("#" + streetNumClientID1).val());
    $("#" + streetNameClientID2).val($("#" + streetNameClientID1).val());
    $("#" + aptNumClientID2).val($("#" + aptNumClientID1).val());
    $("#" + POBoxClientID2).val($("#" + POBoxClientID1).val());
    $("#" + CityClientID2).val($("#" + CityClientID1).val());

    $("#" + StateClientID2).val($("#" + StateClientID1).val());

    $("#" + ZipClientID2).val($("#" + ZipClientID1).val());
    $("#" + CountyClientID2).val($("#" + CountyClientID1).val());
}

var ddRelationships = new Array();
var PolicyHolderCount = 0;
function CheckDriverRelationshipToPolicyHolder()
{
    return;
    // used to make sure only one driver is either Policyholder or Policyholder#2
    var ph1Used = false;
    var ph1UsedIndex = -1;
    var ph2Used = false;
    var ph2UsedIndex = -1;

    var len = ddRelationships.length;
    for (var i = 0; i < len; i++) {
        // 8 = policyholder#1
        // 5 = policyholder#2
        var ddVal = $("#" + ddRelationships[i]).val();
        //alert(ddVal);
        if (ddVal == SD_RelationshipTypeId_Policyholder && ph1Used == false)
        {
            var ph1Used = true;
            var ph1UsedIndex = i;
        }
        if (ddVal == SD_RelationshipTypeId_Policyholder2 && ph2Used == false) {
            var ph2Used = true;
            var ph2UsedIndex = i;
        }
    }

    for (var i = 0; i < len; i++) {
        if (ph1Used && i != ph1UsedIndex)
        {
            // disable ph1 option
            var currentVal= $("#" + ddRelationships[i]).val();
            $("#" + ddRelationships[i] + " option[value='" + SD_RelationshipTypeId_Policyholder + "']").attr('disabled', 'disabled');
            $("#" + ddRelationships[i]).val(currentVal);
        }
        if (ph1Used == false) {
            // disable ph1 option
            var currentVal = $("#" + ddRelationships[i]).val();
            $("#" + ddRelationships[i] + " option[value='" + SD_RelationshipTypeId_Policyholder + "']").removeAttr('disabled');
            $("#" + ddRelationships[i]).val(currentVal);
        }

        if (PolicyHolderCount > 1) {
            if (ph2Used && i != ph2UsedIndex) {
                // disable ph1 option
                var currentVal = $("#" + ddRelationships[i]).val();
                $("#" + ddRelationships[i] + " option[value='" + SD_RelationshipTypeId_Policyholder2 + "']").attr('disabled', 'disabled');
                $("#" + ddRelationships[i]).val(currentVal);
            }

            if (ph2Used == false) {
                // disable ph1 option
                var currentVal = $("#" + ddRelationships[i]).val();
                $("#" + ddRelationships[i] + " option[value='" + SD_RelationshipTypeId_Policyholder2 + "']").removeAttr('disabled');
                $("#" + ddRelationships[i]).val(currentVal);
            }
        }
        else
        {
            var currentVal = $("#" + ddRelationships[i]).val();
            $("#" + ddRelationships[i] + " option[value='" + SD_RelationshipTypeId_Policyholder2 + "']").attr('disabled', 'disabled');
            $("#" + ddRelationships[i]).val(currentVal);
        }
    }
}

var indiana_Counties = ["Adams","Allen","Bartholomew","Benton","Blackford","Boone","Brown","Carroll",
    "Cass","Clark","Clay","Clinton","Crawford","Daviess","Dearborn","Decatur","DeKalb","Delaware",
    "Dubois","Elkhart","Fayette","Floyd","Fountain","Franklin","Fulton","Gibson","Grant","Greene",
    "Hamilton","Hancock","Harrison","Hendricks","Henry","Howard","Huntington","Jackson","Jasper",
    "Jay","Jefferson","Jennings","Johnson","Knox","Kosciusko","LaGrange","Lake","La Porte","Lawrence",
	"Madison","Marion","Marshall","Martin","Miami","Monroe","Montgomery","Morgan","Newton","Noble",
    "Ohio","Orange","Owen","Parke","Perry","Pike","Porter","Posey","Pulaski","Putnam","Randolph",
    "Ripley","Rush","Scott","Shelby","Spencer","St. Joseph","Starke","Steuben","Sullivan","Switzerland",
    "Tippecanoe","Tipton","Union","Vanderburgh","Vermillion","Vigo","Wabash","Warren","Warrick","Washington",
    "Wayne", "Wells", "White", "Whitley"];

var INCities = ["AKRON","ALAMO","ALBANY","ALBION","ALEXANDRIA","ALFORDSVILLE","ALTON","ALTONA","AMBIA","AMBOY","AMO","ANDERSON","ANDREWS","ANGOLA","ARCADIA","ARGOS",
"ASHLEY","ATLANTA","ATTICA","AUBURN","AURORA","AUSTIN","AVILLA","AVON","BAINBRIDGE","BARGERSVILLE","BATESVILLE","BATTLE GROUND","BEDFORD","BEECH GROVE",
"BERNE","BETHANY","BEVERLY SHORES","BICKNELL","BIRDSEYE","BLOOMFIELD","BLOOMINGDALE","BLOOMINGTON","BLOUNTSVILLE","BLUFFTON","BOONVILLE","BORDEN",
"BOSTON","BOSWELL","BOURBON","BRAZIL","BREMEN","BRIDGEPORT","BRISTOL","BROOK","BROOKLYN","BROOKSBURG","BROOKSTON","BROOKVILLE","BROWNSBURG","BROWNSTOWN",
"BRUCEVILLE","BRYANT","BUNKER HILL","BURKET","BURLINGTON","BURNETTSVILLE","BURNS HARBOR","BUTLER","CADIZ","CAMBRIDGE CITY","CAMDEN","CAMPBELLSBURG",
"CANNELBURG","CANNELTON","CARBON","CARLISLE","CARMEL","CARTHAGE","CAYUGA","CEDAR GROVE",
"CEDAR LAKE","CENTER POINT","CENTERVILLE","CHALMERS","CHANDLER","CHARLESTOWN","CHESTERFIELD","CHESTERTON","CHILI","CHRISNEY","CHURUBUSCO","CICERO","CLARKS HILL",
"CLARKSVILLE","CLAY CITY","CLAYPOOL","CLAYTON","CLEAR LAKE","CLERMONT","CLIFFORD","CLINTON","CLOVERDALE","COATESVILLE","COLFAX","COLUMBIA CITY","COLUMBUS",
"CONNERSVILLE","CONVERSE","CORUNNA","CORYDON","COUNTRY CLUB HEIGHTS","COVINGTON","CRANDALL","CRANE","CRAWFORDSVILLE","CROMWELL","CROTHERSVILLE","CROWN POINT",
"CROWS NEST","CULVER","CUMBERLAND","CYNTHIANA","DALE","DALEVILLE","DANA","DANVILLE","DARLINGTON","DARMSTADT","DAYTON","DE MOTTE","DECATUR","DECKER","DELPHI",
"DENVER","DILLSBORO","DUBLIN","DUGGER","DUNE ACRES","DUNKIRK","DUNREITH","DUPONT","DYER","EARL PARK","EAST CHICAGO","EAST GERMANTOWN","EATON","ECONOMY","EDGEWOOD",
"EDINBURGH","EDWARDSPORT","EDWARDSPORT","ELBERFELD","ELIZABETH","ELKHART","ELLETTSVILLE","ELNORA","ELWOOD","EMINANCE","ENGLISH","ETNA GREEN","EVANSVILLE",
"FAIRMOUNT","FAIRVIEW PARK","FARMERSBURG","FARMLAND","FAYETTE","FERDINAND","FILLMORE","FISHERS","FLORA","FORT BRANCH","FORT WAYNE","FORTVILLE",
"FOUNTAIN CITY","FOWLER","FOWLERTON","FRANCESVILLE","FRANCISCO","FRANKFORT","FRANKLIN","FRANKTON","FREDERICKSBURG","FREMONT","FRENCH LICK",
"FULTON","GALENA","GALVESTON","GARRETT","GARY","GAS CITY","GASTON","GENEVA","GENTRYVILLE","GEORGETOWN","GLENWOOD","GOODLAND","GOSHEN","GOSPORT","GRABILL",
"GRANDVIEW","GREENCASTLE","GREENCASTLE","GREENDALE","GREENFIELD","GREENFIELD","GREENS FORK","GREENSBORO","GREENSBURG","GREENSBURG","GREENTOWN","GREENVILLE",
"GREENWOOD","GREENWOOD","GRIFFIN","GRIFFITH","HAGERSTOWN","HAMILTON","HAMLET","HAMMOND","HAMMOND","HANNA","HANOVER","HARDINSBURG","HARMONY","HARTFORD CITY",
"HARTSVILLE","HAUBSTADT","HAZLETON","HEBRON","HIGHLAND","HILLSBORO","HOBART","HOLLAND","HOLTON","HOMECROFT","HOPE","HUDSON","HUNTERTOWN","HUNTINGBURG",
"HUNTINGTON","HYMERA","INDIAN VILLAGE","INDIANAPOLIS","INGALLS","JAMESTOWN","JASONVILLE","JASPER","JEFFERSONVILLE","JONESBORO","JONESVILLE","JUDYVILLE",
"KEMPTON","KENDALLVILLE","KENNARD","KENTLAND","KEWANNA","KINGMAN","KINGSBURY","KINGSFORD HEIGHTS","KIRKLIN","KNIGHTSTOWN","KNIGHTSVILLE","KNOX",
"KOKOMO","KOUTS","LA CROSSE","LA FONTAINE","LA PAZ","LA PORTE","LACONIA","LADOGA","LAFAYETTE","LAGRANGE","LAGRO","LAKE STATION","LAKEVILLE",
"LANESVILLE","LAPEL","LARWILL","LAUREL","LAWRENCE","LAWRENCEBURG","LEAVENWORTH","LEBANON","LEESBURG","LEO-CEDARVILLE","LEWISVILLE","LIBERTY","LIGONIER",
"LINCOLN","LINDEN","LINTON","LITTLE YORK","LIVONIA","LIZTON","LOGANSPORT","LONG BEACH","LOOGOOTEE","LOSANTVILLE","LOWELL","LYNN","LYNNVILLE","LYONS",
"MACKEY","MACY","MADISON","MARENGO","MARION","MARKLE","MARKLEVILLE","MARSHALL","MARTINSVILLE","MATTHEWS","MAUCKPORT","MCCORDSVILLE","MECCA","MEDARYVILLE",
"MEDORA","MELLOTT","MENTONE","MERIDIAN HILLS","MEROM","MERRILLVILLE","MICHIANA SHORES","MICHIGAN CITY","MICHIGANTOWN","MIDDLEBURY","MIDDLETOWN",
"MILAN","MILFORD","MILLERSBURG","MILLHOUSEN","MILLTOWN","MILTON","MISHAWAKA","MITCHELL","MODOC","MONON","MONROE","MONROE CITY","MONROEVILLE","MONROVIA",
"MONTEREY","MONTEZUMA","MONTGOMERY","MONTICELLO","MONTPELIER","MOORELAND","MOORES HILL","MOORESVILLE","MORGANTOWN","MOROCCO","MORRISTOWN","MOUNT AUBURN",
"MOUNT AYR","MOUNT CARMEL","MOUNT ETNA","MOUNT SUMMIT","MOUNT VERNON","MULBERRY","MUNCIE","MUNSTER","NAPOLEON","NAPPANEE","NASHVILLE","NEW ALBANY",
"NEW AMSTERDAM","NEW CARLISLE","NEW CASTLE","NEW CHICAGO","NEW DURHAM","NEW HARMONY","NEW HAVEN","NEW MARKET","NEW MIDDLETOWN","NEW PALESTINE","NEW PEKIN",
"NEW POINT","NEW RICHMOND","NEW ROSS","NEW WHITELAND","NEWBERRY","NEWBURGH","NEWPORT","NEWTOWN","NOBLESVILLE","NORTH CROWS NEST","NORTH JUDSON","NORTH LIBERTY",
"NORTH MANCHESTER","NORTH SALEM","NORTH VERNON","NORTH WEBSTER","OAKLAND CITY","OAKTOWN","ODON","OGDEN DUNES","OLDENBURG","ONWARD","OOLITIC","ORESTES","ORLAND",
"ORLEANS","OSCEOLA","OSGOOD","OSSIAN","OSWEGO","OTTERBEIN","OWENSVILLE","OXFORD","PALMYRA","PAOLI","PARAGON","PARKER CITY","PATOKA","PATRIOT","PENDLETON",
"PENNVILLE","PERRYSVILLE","PERU","PETERSBURG","PIERCETON","PINE VILLAGE","PITTSBORO","PLAINFIELD","PLAINVILLE","PLYMOUTH","PONETO","PORTAGE","PORTER",
"PORTLAND","POSEYVILLE","POTTAWATTAMIE PARK","PRINCES LAKES","PRINCETON","REDKEY","REMINGTON","RENSSELAER","REYNOLDS","RICHMOND","RIDGEVILLE","RILEY",
"RISING SUN","RIVER FOREST","ROACHDALE","ROANN","ROANOKE","ROCHESTER","ROCKPORT","ROCKVILLE","ROCKY RIPPLE","ROME CITY","ROSEDALE","ROSELAND","ROSSVILLE",
"ROYAL CENTER","RUSHVILLE","RUSSELLVILLE","RUSSIAVILLE","SALAMONIA","SALEM","SALTILLO","SANDBORN","SANTA CLAUS","SARATOGA","SCHERERVILLE","SCHNEIDER",
"SCOTTSBURG","SEELYVILLE","SELLERSBURG","SELMA","SEYMOUR","SHADELAND","SHAMROCK LAKES","SHARPSVILLE","SHELBURN","SHELBY","SHELBYVILLE","SHERIDAN",
"SHIPSHEWANA","SHIRLEY","SHOALS","SIDNEY","SILVER LAKE","SOMERVILLE","SOUTH BEND","SOUTH WHITLEY","SOUTHPORT","SPEEDWAY","SPENCER","SPICELAND","SPRING GROVE",
"SPRING HILL","SPRING LAKE","SPRINGFIELD","SPRINGPORT","SPURGEON","ST. JOE","ST. JOHN","ST. LEON","ST. PAUL","STATE LINE CITY","STAUNTON","STILESVILLE",
"STINESVILLE","STRAUGHN","SULLIVAN","SULPHUR SPRINGS","SUMMITVILLE","SUNMAN","SWAYZEE","SWEETSER","SWITZ CITY","SYRACUSE","TELL CITY","TENNYSON",
"TERRE HAUTE","THORNTOWN","TIPTON","TOPEKA","TOWN OF PINES","TRAFALGAR","TRAIL CREEK","TROY","ULEN","UNION CITY","UNIONDALE","UNIVERSAL","UPLAND","UTICA",
"VALPARAISO","VAN BUREN","VEEDERSBURG","VERA CRUZ","VERNON","VERSAILLES","VEVAY","VINCENNES","WABASH","WAKARUSA","WALKERTON","WALLACE","WALTON","WANATAH",
"WARREN","WARREN PARK","WARSAW","WASHINGTON","WATERLOO","WAVELAND","WAYNETOWN","WEST BADEN SPRINGS","WEST COLLEGE CORNER","WEST HARRISON","WEST LAFAYETTE",
"WEST LEBANON","WEST TERRE HAUTE","WESTFIELD","WESTFIELD","WESTPORT","WESTVILLE","WHEATFIELD","WHEATLAND","WHITELAND","WHITESTOWN","WHITEWATER","WHITING",
"WILKINSON","WILLIAMS","WILLIAMS CREEK","WILLIAMSPORT","WINAMAC","WINCHESTER","WINDFALL CITY","WINFIELD","WINGATE","WINONA LAKE","WINSLOW","WOLCOTT","WOLCOTTVILLE",
"WOODBURN", "WOODLAWN HEIGHTS", "WORTHINGTON", "WYNNEDALE", "YORKTOWN", "ZANESVILLE", "ZIONSVILLE"];

function GetFarmLocationOccupantList()
{
    //updates_applicantNames
    //medicalPaymentNames_textboxIds
    var occupantName = new Array();
    occupantName.push("");// One Empty at top of list
    for(var i = 0;i < updates_applicantNames.length;i++)
    {
        occupantName.push(updates_applicantNames[i]);
    }
    if (!(typeof(medicalPaymentNames_textboxIds) === "undefined")) {
            for (var i = 0; i < medicalPaymentNames_textboxIds.length; i++) {
                var name = $('#' + medicalPaymentNames_textboxIds[i][0]).val() + " " + $('#' + medicalPaymentNames_textboxIds[i][1]).val();
                if (name.trim() != "" && occupantName.contains(name) == false)
                    occupantName.push(name);
            }
    }
    occupantName.push("OTHER"); // Other at bottom of the list
    return occupantName;
}

function LoadFramLocationOccupantDropdown(dropdownId,hdnFieldId)
{
    var currentVal = $('#' + hdnFieldId).val();

    var occupantList = GetFarmLocationOccupantList();

    $('#' + dropdownId).val(''); // reste set value
    $('#' + dropdownId + ' >option').remove(); // remove all
    for(var i = 0; i < occupantList.length;i++)
    {
        $('#' + dropdownId).append($('<option> </option>').val(occupantList[i].toUpperCase()).html(occupantList[i].toUpperCase()));
    }

    if (occupantList.contains(currentVal))
        $('#' + dropdownId).val(currentVal); // reset to same value if it is still there
}

function BindMedicalPaymentNametoOccupant(dropdownId, hdnFieldId)
{
    if (!typeof(medicalPaymentNames_textboxIds) === "undefined") {
        for (var i = 0; i < medicalPaymentNames_textboxIds.length; i++) {
            $("#" + medicalPaymentNames_textboxIds[i][0]).keyup(function () { LoadFramLocationOccupantDropdown(dropdownId, hdnFieldId); });
            $("#" + medicalPaymentNames_textboxIds[i][1]).keyup(function () { LoadFramLocationOccupantDropdown(dropdownId, hdnFieldId); });
        }
    }
}

// Returns a function, that, as long as it continues to be invoked, will not
// be triggered. The function will be called after it stops being called for
// N milliseconds. If `immediate` is passed, trigger the function on the
// leading edge, instead of the trailing.
function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
};

// Returns a function, that, when invoked, will only be triggered at most once
// during a given window of time. Normally, the throttled function will run
// as much as it can, without ever going more than once per `wait` duration;
// but if you'd like to disable the execution on the leading edge, pass
// "{'leading': false}". To disable execution on the "trailing" edge, ditto.
function throttle(func, wait, options) {
    var context, args, result;
    var timeout = null;
    var previous = 0;
    if (!options) options = {};
    var later = function () {
        previous = options.leading === false ? 0 : new Date().getTime();
        timeout = null;
        result = func.apply(context, args);
        if (!timeout) context = args = null;
    };
    return function () {
        var now = Date.now();
        if (!previous && options.leading === false) previous = now;
        var remaining = wait - (now - previous);
        context = this;
        args = arguments;
        if (remaining <= 0 || remaining > wait) {
            if (timeout) {
                clearTimeout(timeout);
                timeout = null;
            }
            previous = now;
            result = func.apply(context, args);
            if (!timeout) context = args = null;
        } else if (!timeout && options.trailing !== false) {
            timeout = setTimeout(later, remaining);
        }
        return result;
    };
};

//copy of the IFM.VR.Common.Workflow.Workflow.WorkflowSection enum.
var IFMWorkflowEnum = {
    NA: "na",
    Blank: "",
    PolicyHolders: "policyHolders",
    Drivers: "drivers",
    Vehicles: "vehicles",
    Coverages: "coverages",
    Summary: "summary",
    UWQuestions: "uwQuestions",
    App: "app",
    Property: "property_",
    Location: "location",
    FarmIRPM: "farmIRPM",
    FarmPP: "farmPP",
    InlandMarine: "InlandMarine",
    Buildings: "buildings",
    FileUpload: "fileUpload",
    DocumentPrinting: "documentPrinting"
}

var IFMLOBVersionsEnum = {
    HOM2018Upgrade: "HOM2018Upgrade"
}

var IFMLOBEnum = {
    BOP: {
        FullName: "Commercial BOP",
        AbbreviatedName: "BOP",
        NameNoSpaces: "CommercialBOP",
        LobId: 25
    },
    CAP: {
        FullName: "Commercial Auto",
        AbbreviatedName: "CAP",
        NameNoSpaces: "CommercialAuto",
        LobId: 22
    },
    CGL: {
        FullName: "Commercial General Liability",
        AbbreviatedName: "CGL",
        NameNoSpaces: "CommercialGeneralLiability",
        LobId: 9
    },
    CIM: {
        FullName: "Commercial Inland Marine",
        AbbreviatedName: "CIM",
        NameNoSpaces: "CommercialInlandMarine",
        LobId: 29
    },
    CPP: {
        FullName: "Commercial Package",
        AbbreviatedName: "CPP",
        NameNoSpaces: "CommercialPackage",
        LobId: 23
    },
    CPR: {
        FullName: "Commercial Property",
        AbbreviatedName: "CPR",
        NameNoSpaces: "CommercialProperty",
        LobId: 28
    },
    CRM: {
        FullName: "Commercial Crime",
        AbbreviatedName: "CRM",
        NameNoSpaces: "CommercialCrime",
        LobId: 26
    },
    CUP: {
        FullName: "Commercial Umbrella",
        AbbreviatedName: "CUP",
        NameNoSpaces: "CommercialUmbrella",
        LobId: 27
    },
    DFR: {
        FullName: "Dwelling Fire Personal",
        AbbreviatedName: "DFR",
        NameNoSpaces: "DwellingFirePersonal",
        LobId: 3
    },
    FAR: {
        FullName: "Farm",
        AbbreviatedName: "FAR",
        NameNoSpaces: "Farm",
        LobId: 17
    },
    FUP: {
        FullName: "Umbrella Personal",
        AbbreviatedName: "FUP",
        NameNoSpaces: "UmbrellaPersonal",
        LobId: 14
    },
    GAR: {
        FullName: "Commercial Garage",
        AbbreviatedName: "GAR",
        NameNoSpaces: "CommercialGarage",
        LobId: 24
    },
    HOM: {
        FullName: "Home Personal",
        AbbreviatedName: "HOM",
        NameNoSpaces: "HomePersonal",
        LobId: 2
    },
    PIM: {
        FullName: "Inland Marine Personal",
        AbbreviatedName: "PIM",
        NameNoSpaces: "InlandMarinePersonal",
        LobId: 16
    },
    PPA: {
        FullName: "Auto Personal",
        AbbreviatedName: "PPA",
        NameNoSpaces: "AutoPersonal",
        LobId: 1
    },
    PUP: {
        FullName: "Umbrella Personal",
        AbbreviatedName: "PUP",
        NameNoSpaces: "UmbrellaPersonal",
        LobId: 14
    },
    WCP: {
        FullName: "Workers Compensation",
        AbbreviatedName: "WCP",
        NameNoSpaces: "WorkersCompensation",
        LobId: 21
    }
}

function doUseNewVersionOfLOB(IFMLOBVersion, QuoteLobId, fallBackVersionDate) {
    var effectiveDate = "";
    var versionStartDate = "";
    var returnVar = false;

    if (QuoteLobId && checkLobIdExistsInNewVersionLOBsArray(IFMLOBVersion, QuoteLobId)) {
        if ($('#hdnLOBVersionDate_' + IFMLOBVersion).val()) {
            versionStartDate = new Date($('#hdnLOBVersionDate_' + IFMLOBVersion.toString()).val());
        } else {
            if (fallBackVersionDate) {
                versionStartDate = new Date(fallBackVersionDate);
            }
        }

        if (ifm.vr.currentQuote.treeviewEffectiveDate()) {
            effectiveDate = new Date(ifm.vr.currentQuote.treeviewEffectiveDate());
        } else {
            effectiveDate = new Date(Date.now());
        }

        if (versionStartDate && effectiveDate) {
            if (effectiveDate >= versionStartDate) {
                returnVar = true;
            } else {
                returnVar = false;
            }
        } else {
            returnVar = false;
        }
    } else {
        returnVar = false;
    }
    
    return returnVar;
}

function checkLobIdExistsInNewVersionLOBsArray(IFMLOBVersion, LobId) {
    var LOBArray = getNewVersionLOBsAsArray(IFMLOBVersion);

    if (LOBArray && LOBArray.length > 0) {
        for (var i = 0; i < LOBArray.length; i++) {
            for (var prop in IFMLOBEnum) {
                if (IFMLOBEnum[prop].NameNoSpaces === LOBArray[i]) {
                    return true;
                }
            }
        }
    }

    return false;
}

function getNewVersionStartDate(IFMLOBVersions, fallBackVersionDate) {
    var versionStartDate = "";
    if ($('#hdnLOBVersionDate_' + IFMLOBVersions).val()) {
        versionStartDate = new Date($('#hdnLOBVersionDate_' + IFMLOBVersions.toString()).val());
    } else {
        if (fallBackVersionDate) {
            versionStartDate = new Date(fallBackVersionDate);
        }
    }

    return versionStartDate;
}

function getNewVersionLOBsAsArray(IFMLOBVersion) {
    var LOBString = "";
    var LOBArray = [];

    if ($('#hdnLOBVersionAllowedLOBs_' + IFMLOBVersion).val()) {
        LOBString = $('#hdnLOBVersionAllowedLOBs_' + IFMLOBVersion).val()
        LOBArray = LOBString.split(",");
    }

    return LOBArray;
}

function hideVersionedFields() {
    var myEffDate = new Date(ifm.vr.currentQuote.treeviewEffectiveDate());
    var myVersionedFieldsArray = [];
    var myVersionDate = "";
    $("[data-versionedfield]").each(function () {
        if (myVersionedFieldsArray.indexOf($(this).data("versionedfield")) === -1) {
            myVersionedFieldsArray.push($(this).data("versionedfield"));
        }
    });
    if (myVersionedFieldsArray.length > 0) {
        for (var i = 0; i < myVersionedFieldsArray.length; i++) {
            var arrValue = myVersionedFieldArray[i];
            for (var j = 0; j < IFMLOBVersionsEnum.length; j++) {
                var enumValue = IFMLOBVersionsEnum[i];
                if (arrValue === enumValue) {
                    myVersionDate = new Date(getNewVersionStartDate(enumValue));
                    if (myVersionDate) {
                        if (myEffDate < myVersionDate) {
                            $("[data-versionedfield]=" + enumValue).hide();
                        }
                    }
                    break;
                }
            }
        }
    }
}

//Added 2/14/2022 for bug 63511 MLW
function ShowHideOtherEntityType(ddBusinessTypeId, liOtherEntityId, txtOtherEntityId) {
    //Only show the Other Legal Entity textbox (txtOtherEntity) if the Legal Entity (ddBusinessType) is Other (Other = 5)
    if ($('#' + ddBusinessTypeId).val() == '5') {
        $("#" + liOtherEntityId).show();
    } else {
        $("#" + liOtherEntityId).hide();
        $("#" + txtOtherEntityId).val("");
    }
}