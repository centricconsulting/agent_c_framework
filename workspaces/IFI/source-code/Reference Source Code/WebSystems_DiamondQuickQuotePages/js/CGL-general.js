/* File Created: July 9, 2012 */

function FormatAsNumber(nStr,Exclude) {
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

function FormatAsNumberNoCommaFormatting(nStr, Exclude) {
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
    return nStr;
}

function FormatAsCurrency(nStr) {
    var formatted = '$' + FormatAsNumber(nStr, " ,[A-Za-z],$");
    if (formatted != '$') {
        x = formatted.split('.');
        x1 = x[0]; // is left of the period
        x2 = x.length > 1 ? '.' + x[1] : ''; // is anything right of the period
        var allowedDigits = 2 + 1; // add one for period
        if (x2.length > allowedDigits) {
            x2 = x2.substring(0, allowedDigits);
         }
         return x1 + x2;
    
    }    
    return "";
}

function DisableButtonOnclick(btn, submittingText) {
    try{
        if (submittingText != undefined) {
            $(btn).attr('value', submittingText);        
        }
        $(btn).attr('disabled', 'true');    
    }
    catch(err){}
}


function DisableButtonOnclick(btn) {
    try {
        $(btn).attr('disabled', 'true');
    }
    catch (err) { }
    }


// rather than using this just catch it and ask to fix... otherwise  they try to enter '4.25' and this returns '425'
//function FormatAsWholeNumber(nStr) {
//    var formatted = FormatAsNumber(nStr, " ,[A-Za-z],$");
//    if (formatted != '') {
//        x = formatted.split('.');
//        x1 = x[0]; // is left of the period
//        x2 = x.length > 1 ? '.' + x[1] : ''; // is anything right of the period
//        var allowedDigits = 0; // add one for period
//        if (x2.length > allowedDigits) {
//            x2 = x2.substring(0, allowedDigits)
//        }
//        return x1 + x2;

//    }
//    return "";
//}
var INCities = new Array(
 "AKRON",
"ALAMO",
"ALBANY",
"ALBION",
"ALEXANDRIA",
"ALFORDSVILLE",
"ALTON",
"ALTONA",
"AMBIA",
"AMBOY",
"AMO",
"ANDERSON",
"ANDREWS",
"ANGOLA",
"ARCADIA",
"ARGOS",
"ASHLEY",
"ATLANTA",
"ATTICA",
"AUBURN",
"AURORA",
"AUSTIN",
"AVILLA",
"AVON",
"BAINBRIDGE",
"BARGERSVILLE",
"BATESVILLE",
"BATTLE GROUND",
"BEDFORD",
"BEECH GROVE",
"BERNE",
"BETHANY",
"BEVERLY SHORES",
"BICKNELL",
"BIRDSEYE",
"BLOOMFIELD",
"BLOOMINGDALE",
"BLOOMINGTON",
"BLOUNTSVILLE",
"BLUFFTON",
"BOONVILLE",
"BORDEN",
"BOSTON",
"BOSWELL",
"BOURBON",
"BRAZIL",
"BREMEN",
"BRIDGEPORT",
"BRISTOL",
"BROOK",
"BROOKLYN",
"BROOKSBURG",
"BROOKSTON",
"BROOKVILLE",
"BROWNSBURG",
"BROWNSTOWN",
"BRUCEVILLE",
"BRYANT",
"BUNKER HILL",
"BURKET",
"BURLINGTON",
"BURNETTSVILLE",
"BURNS HARBOR",
"BUTLER",
"CADIZ",
"CAMBRIDGE CITY",
"CAMDEN",
"CAMPBELLSBURG",
"CANNELBURG",
"CANNELTON",
"CARBON",
"CARLISLE",
"CARMEL",
"CARTHAGE",
"CAYUGA",
"CEDAR GROVE",
"CEDAR LAKE",
"CENTER POINT",
"CENTERVILLE",
"CHALMERS",
"CHANDLER",
"CHARLESTOWN",
"CHESTERFIELD",
"CHESTERTON",
"CHILI",
"CHRISNEY",
"CHURUBUSCO",
"CICERO",
"CLARKS HILL",
"CLARKSVILLE",
"CLAY CITY",
"CLAYPOOL",
"CLAYTON",
"CLEAR LAKE",
"CLERMONT",
"CLIFFORD",
"CLINTON",
"CLOVERDALE",
"COATESVILLE",
"COLFAX",
"COLUMBIA CITY",
"COLUMBUS",
"CONNERSVILLE",
"CONVERSE",
"CORUNNA",
"CORYDON",
"COUNTRY CLUB HEIGHTS",
"COVINGTON",
"CRANDALL",
"CRANE",
"CRAWFORDSVILLE",
"CROMWELL",
"CROTHERSVILLE",
"CROWN POINT",
"CROWS NEST",
"CULVER",
"CUMBERLAND",
"CYNTHIANA",
"DALE",
"DALEVILLE",
"DANA",
"DANVILLE",
"DARLINGTON",
"DARMSTADT",
"DAYTON",
"DE MOTTE",
"DECATUR",
"DECKER",
"DELPHI",
"DENVER",
"DILLSBORO",
"DUBLIN",
"DUGGER",
"DUNE ACRES",
"DUNKIRK",
"DUNREITH",
"DUPONT",
"DYER",
"EARL PARK",
"EAST CHICAGO",
"EAST GERMANTOWN",
"EATON",
"ECONOMY",
"EDGEWOOD",
"EDINBURGH",
"EDWARDSPORT",
"EDWARDSPORT",
"ELBERFELD",
"ELIZABETH",
"ELKHART",
"ELLETTSVILLE",
"ELNORA",
"ELWOOD",
"EMINANCE",
"ENGLISH",
"ETNA GREEN",
"EVANSVILLE",
"FAIRMOUNT",
"FAIRVIEW PARK",
"FARMERSBURG",
"FARMLAND",
"FAYETTE",
"FERDINAND",
"FILLMORE",
"FISHERS",
"FLORA",
"FORT BRANCH",
"FORT WAYNE",
"FORTVILLE",
"FOUNTAIN CITY",
"FOWLER",
"FOWLERTON",
"FRANCESVILLE",
"FRANCISCO",
"FRANKFORT",
"FRANKLIN",
"FRANKTON",
"FREDERICKSBURG",
"FREMONT",
"FRENCH LICK",
"FULTON",
"GALENA",
"GALVESTON",
"GARRETT",
"GARY",
"GAS CITY",
"GASTON",
"GENEVA",
"GENTRYVILLE",
"GEORGETOWN",
"GLENWOOD",
"GOODLAND",
"GOSHEN",
"GOSPORT",
"GRABILL",
"GRANDVIEW",
"GREENCASTLE",
"GREENCASTLE",
"GREENDALE",
"GREENFIELD",
"GREENFIELD",
"GREENS FORK",
"GREENSBORO",
"GREENSBURG",
"GREENSBURG",
"GREENTOWN",
"GREENVILLE",
"GREENWOOD",
"GREENWOOD",
"GRIFFIN",
"GRIFFITH",
"HAGERSTOWN",
"HAMILTON",
"HAMLET",
"HAMMOND",
"HAMMOND",
"HANNA",
"HANOVER",
"HARDINSBURG",
"HARMONY",
"HARTFORD CITY",
"HARTSVILLE",
"HAUBSTADT",
"HAZLETON",
"HEBRON",
"HIGHLAND",
"HILLSBORO",
"HOBART",
"HOLLAND",
"HOLTON",
"HOMECROFT",
"HOPE",
"HUDSON",
"HUNTERTOWN",
"HUNTINGBURG",
"HUNTINGTON",
"HYMERA",
"INDIAN VILLAGE",
"INDIANAPOLIS",
"INGALLS",
"JAMESTOWN",
"JASONVILLE",
"JASPER",
"JEFFERSONVILLE",
"JONESBORO",
"JONESVILLE",
"JUDYVILLE",
"KEMPTON",
"KENDALLVILLE",
"KENNARD",
"KENTLAND",
"KEWANNA",
"KINGMAN",
"KINGSBURY",
"KINGSFORD HEIGHTS",
"KIRKLIN",
"KNIGHTSTOWN",
"KNIGHTSVILLE",
"KNOX",
"KOKOMO",
"KOUTS",
"LA CROSSE",
"LA FONTAINE",
"LA PAZ",
"LA PORTE",
"LACONIA",
"LADOGA",
"LAFAYETTE",
"LAGRANGE",
"LAGRO",
"LAKE STATION",
"LAKEVILLE",
"LANESVILLE",
"LAPEL",
"LARWILL",
"LAUREL",
"LAWRENCE",
"LAWRENCEBURG",
"LEAVENWORTH",
"LEBANON",
"LEESBURG",
"LEO-CEDARVILLE",
"LEWISVILLE",
"LIBERTY",
"LIGONIER",
"LINCOLN",
"LINDEN",
"LINTON",
"LITTLE YORK",
"LIVONIA",
"LIZTON",
"LOGANSPORT",
"LONG BEACH",
"LOOGOOTEE",
"LOSANTVILLE",
"LOWELL",
"LYNN",
"LYNNVILLE",
"LYONS",
"MACKEY",
"MACY",
"MADISON",
"MARENGO",
"MARION",
"MARKLE",
"MARKLEVILLE",
"MARSHALL",
"MARTINSVILLE",
"MATTHEWS",
"MAUCKPORT",
"MCCORDSVILLE",
"MECCA",
"MEDARYVILLE",
"MEDORA",
"MELLOTT",
"MENTONE",
"MERIDIAN HILLS",
"MEROM",
"MERRILLVILLE",
"MICHIANA SHORES",
"MICHIGAN CITY",
"MICHIGANTOWN",
"MIDDLEBURY",
"MIDDLETOWN",
"MILAN",
"MILFORD",
"MILLERSBURG",
"MILLHOUSEN",
"MILLTOWN",
"MILTON",
"MISHAWAKA",
"MITCHELL",
"MODOC",
"MONON",
"MONROE",
"MONROE CITY",
"MONROEVILLE",
"MONROVIA",
"MONTEREY",
"MONTEZUMA",
"MONTGOMERY",
"MONTICELLO",
"MONTPELIER",
"MOORELAND",
"MOORES HILL",
"MOORESVILLE",
"MORGANTOWN",
"MOROCCO",
"MORRISTOWN",
"MOUNT AUBURN",
"MOUNT AYR",
"MOUNT CARMEL",
"MOUNT ETNA",
"MOUNT SUMMIT",
"MOUNT VERNON",
"MULBERRY",
"MUNCIE",
"MUNSTER",
"NAPOLEON",
"NAPPANEE",
"NASHVILLE",
"NEW ALBANY",
"NEW AMSTERDAM",
"NEW CARLISLE",
"NEW CASTLE",
"NEW CHICAGO",
"NEW DURHAM",
"NEW HARMONY",
"NEW HAVEN",
"NEW MARKET",
"NEW MIDDLETOWN",
"NEW PALESTINE",
"NEW PEKIN",
"NEW POINT",
"NEW RICHMOND",
"NEW ROSS",
"NEW WHITELAND",
"NEWBERRY",
"NEWBURGH",
"NEWPORT",
"NEWTOWN",
"NOBLESVILLE",
"NORTH CROWS NEST",
"NORTH JUDSON",
"NORTH LIBERTY",
"NORTH MANCHESTER",
"NORTH SALEM",
"NORTH VERNON",
"NORTH WEBSTER",
"OAKLAND CITY",
"OAKTOWN",
"ODON",
"OGDEN DUNES",
"OLDENBURG",
"ONWARD",
"OOLITIC",
"ORESTES",
"ORLAND",
"ORLEANS",
"OSCEOLA",
"OSGOOD",
"OSSIAN",
"OSWEGO",
"OTTERBEIN",
"OWENSVILLE",
"OXFORD",
"PALMYRA",
"PAOLI",
"PARAGON",
"PARKER CITY",
"PATOKA",
"PATRIOT",
"PENDLETON",
"PENNVILLE",
"PERRYSVILLE",
"PERU",
"PETERSBURG",
"PIERCETON",
"PINE VILLAGE",
"PITTSBORO",
"PLAINFIELD",
"PLAINVILLE",
"PLYMOUTH",
"PONETO",
"PORTAGE",
"PORTER",
"PORTLAND",
"POSEYVILLE",
"POTTAWATTAMIE PARK",
"PRINCES LAKES",
"PRINCETON",
"REDKEY",
"REMINGTON",
"RENSSELAER",
"REYNOLDS",
"RICHMOND",
"RIDGEVILLE",
"RILEY",
"RISING SUN",
"RIVER FOREST",
"ROACHDALE",
"ROANN",
"ROANOKE",
"ROCHESTER",
"ROCKPORT",
"ROCKVILLE",
"ROCKY RIPPLE",
"ROME CITY",
"ROSEDALE",
"ROSELAND",
"ROSSVILLE",
"ROYAL CENTER",
"RUSHVILLE",
"RUSSELLVILLE",
"RUSSIAVILLE",
"SALAMONIA",
"SALEM",
"SALTILLO",
"SANDBORN",
"SANTA CLAUS",
"SARATOGA",
"SCHERERVILLE",
"SCHNEIDER",
"SCOTTSBURG",
"SEELYVILLE",
"SELLERSBURG",
"SELMA",
"SEYMOUR",
"SHADELAND",
"SHAMROCK LAKES",
"SHARPSVILLE",
"SHELBURN",
"SHELBY",
"SHELBYVILLE",
"SHERIDAN",
"SHIPSHEWANA",
"SHIRLEY",
"SHOALS",
"SIDNEY",
"SILVER LAKE",
"SOMERVILLE",
"SOUTH BEND",
"SOUTH WHITLEY",
"SOUTHPORT",
"SPEEDWAY",
"SPENCER",
"SPICELAND",
"SPRING GROVE",
"SPRING HILL",
"SPRING LAKE",
"SPRINGFIELD",
"SPRINGPORT",
"SPURGEON",
"ST. JOE",
"ST. JOHN",
"ST. LEON",
"ST. PAUL",
"STATE LINE CITY",
"STAUNTON",
"STILESVILLE",
"STINESVILLE",
"STRAUGHN",
"SULLIVAN",
"SULPHUR SPRINGS",
"SUMMITVILLE",
"SUNMAN",
"SWAYZEE",
"SWEETSER",
"SWITZ CITY",
"SYRACUSE",
"TELL CITY",
"TENNYSON",
"TERRE HAUTE",
"THORNTOWN",
"TIPTON",
"TOPEKA",
"TOWN OF PINES",
"TRAFALGAR",
"TRAIL CREEK",
"TROY",
"ULEN",
"UNION CITY",
"UNIONDALE",
"UNIVERSAL",
"UPLAND",
"UTICA",
"VALPARAISO",
"VAN BUREN",
"VEEDERSBURG",
"VERA CRUZ",
"VERNON",
"VERSAILLES",
"VEVAY",
"VINCENNES",
"WABASH",
"WAKARUSA",
"WALKERTON",
"WALLACE",
"WALTON",
"WANATAH",
"WARREN",
"WARREN PARK",
"WARSAW",
"WASHINGTON",
"WATERLOO",
"WAVELAND",
"WAYNETOWN",
"WEST BADEN SPRINGS",
"WEST COLLEGE CORNER",
"WEST HARRISON",
"WEST LAFAYETTE",
"WEST LEBANON",
"WEST TERRE HAUTE",
"WESTFIELD",
"WESTFIELD",
"WESTPORT",
"WESTVILLE",
"WHEATFIELD",
"WHEATLAND",
"WHITELAND",
"WHITESTOWN",
"WHITEWATER",
"WHITING",
"WILKINSON",
"WILLIAMS",
"WILLIAMS CREEK",
"WILLIAMSPORT",
"WINAMAC",
"WINCHESTER",
"WINDFALL CITY",
"WINFIELD",
"WINGATE",
"WINONA LAKE",
"WINSLOW",
"WOLCOTT",
"WOLCOTTVILLE",
"WOODBURN",
"WOODLAWN HEIGHTS",
"WORTHINGTON",
"WYNNEDALE",
"YORKTOWN",
"ZANESVILLE",
"ZIONSVILLE"
); // represent over 4.1 million Indiana residents

function HighLightDefaults(onlyCauseOfLoss) {

    if (onlyCauseOfLoss == undefined || onlyCauseOfLoss == false)
    {
    $("#ddCauseOfLoss").stop(true,false).animate({
        backgroundColor: "yellow"
    }, 250);

    $("#ddDeductible").stop(true, false).animate({
        backgroundColor: "yellow"
    }, 250);
    $("#ddCoInsurance").stop(true, false).animate({
        backgroundColor: "yellow"
    }, 250);
    $("#ddValuation").stop(true, false).animate({
        backgroundColor: "yellow"
    }, 250);
    }
    else
    {
    $("#ddCauseOfLoss").stop(true,false).animate({
        backgroundColor: "yellow"
    }, 250);
    }
}

function UnHighLightDefaults() {
    $("#ddCauseOfLoss").stop(true, false).animate({
        backgroundColor: "white"
    }, 500);

    $("#ddDeductible").stop(true, false).animate({
        backgroundColor: "white"
    }, 500);
    $("#ddCoInsurance").stop(true, false).animate({
        backgroundColor: "white"
    }, 500);
    $("#ddValuation").stop(true, false).animate({
        backgroundColor: "white"
    }, 500);
}





var timerID = null;
var loadingAniEle = null;
var baseAniText = null;
function StartLoadingTextAnimation(ele,baseText) { //7-1-2013
    animationStepIndex = -1;
    baseAniText = baseText;
    loadingAniEle = ele;
    timerID = setInterval("DoLoadingAnimation()", 400);
}

var animationStepIndex = 0;
function DoLoadingAnimation() { //7-1-2013
    if (animationStepIndex == -1) {
        $('input:not(.disb), textarea, table, select').attr('disabled', 'disabled');
        $(loadingAniEle).css('font-weight', 'bolder');
        $('body').css('cursor', 'wait');
        $(loadingAniEle).css('cursor', 'wait');
    }

    switch (animationStepIndex) {
        case 0:
            $(loadingAniEle).val(baseAniText + "     ");
            animationStepIndex += 1;
            break;
        case 1:
            $(loadingAniEle).val(baseAniText + " .   ");
            animationStepIndex += 1;
            break;
        case 2:
            $(loadingAniEle).val(baseAniText + " ..  ");
            animationStepIndex += 1;
            break;
        case 3:
            $(loadingAniEle).val(baseAniText + " ... ");
            animationStepIndex += 1;
            break;
        default:
            $(loadingAniEle).val(baseAniText + " ....");
            animationStepIndex = 0;

            break;
    }
}

    function DisableAllInputElements() {       
                $('input:not(.disb), textarea, table, select').attr('disabled', 'disabled');      
    }

    function EnableInputElements(ele) {
        $(ele).attr('disabled', 'false');
    }

    