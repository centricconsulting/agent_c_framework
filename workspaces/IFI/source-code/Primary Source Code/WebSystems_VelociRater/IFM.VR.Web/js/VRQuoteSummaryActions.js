///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />


function UncheckAllButSender(sender, tableid)
{
    var $chkboxes = $("#" + tableid).find("tr input[type='checkbox']:checked");
    jQuery.each($chkboxes, function () {
        var senderId = $(sender).attr('id');
        var chkBoxId = $(this).attr('id');
        if (senderId != chkBoxId)
        {
            $(this).attr('checked',false);
        }
    });
}

function DobtnFinalizeLinkedApps_click() {
    //tblLinkedApps
    var selectedQuoteIds = GetAllSelectedAppcheckBoxes('tblLinkedApps');

    if (selectedQuoteIds.length > 0) {
        var quoteIds_text = "";
        for (var ii = 0; ii < selectedQuoteIds.length; ii++) {
            quoteIds_text += selectedQuoteIds[ii];
            if (ii + 1 < selectedQuoteIds.length)
                quoteIds_text += "|";
        }

        var url = 'VR3Finalization.aspx?quoteids=' + quoteIds_text;
        window.location = url;
    }
}

function DobtnLoadLinkedQuotes_click() {
    //tblLinkedQuotes
    var selectedCheckBoxes = GetAllSelectedQuoteCheckboxes('tblLinkedQuotes');

    // only going to use the first
    if (selectedCheckBoxes.length > 0) {
        var quoteid = parseInt($("#" + selectedCheckBoxes[0]).attr('quoteid'));
        var lobid = parseInt($("#" + selectedCheckBoxes[0]).attr('quotelobid'));
        var statusid = parseInt($("#" + selectedCheckBoxes[0]).attr('quotestatusid'));
        var url = GetPageUrlForQuoteStatus(quoteid, lobid, statusid);
        if (url != "")
            window.location = url;
    }
}

function GetPageUrlForQuoteStatus(quoteid, quotelobid, quotestatusid)
{
    /// these edit/app page url variables are set on the masterpage
    var url = "";
    if (quotestatusid == 2 | quotestatusid == 4 | quotestatusid == 5 | quotestatusid == 6)
    {
        // edit
        switch (quotelobid) {
            case 1: //ppa
                url = QuickQuote_PPA_Input + quoteid;
                break;
            case 2://hom
                url = QuickQuote_HOM_Input + quoteid;
                break;
            case 3://dfr
                url = QuickQuote_DFR_Input + quoteid;
                break;
            case 17://far
                url = QuickQuote_FAR_Input + quoteid;
                break;
        }
    }
    if (quotestatusid == 3)
    {
        // quote summary
        switch (quotelobid) {
            case 1: //ppa
                url = QuickQuote_PPA_Input + quoteid + '&Workflow=summary';
                break;
            case 2://hom
                url = QuickQuote_HOM_Input + quoteid + '&Workflow=summary';
                break;
            case 3://dfr
                url = QuickQuote_DFR_Input + quoteid + '&Workflow=summary';
                break;
            case 17://far
                url = QuickQuote_FAR_Input + quoteid + '&Workflow=summary';
                break;
        }
    }

    if (quotestatusid == 7 | quotestatusid == 9 | quotestatusid == 10 | quotestatusid == 11)
    {
        // app edit
            switch (quotelobid) {
                case 1: //ppa
                    url = QuickQuote_PPA_App + quoteid;
                    break;
                case 2://hom
                    url = QuickQuote_HOM_App + quoteid;
                    break;
                case 3://dfr
                    url = QuickQuote_DFR_App + quoteid;
                    break;
                case 17://far
                    url = QuickQuote_FAR_App + quoteid;
                    break;
            }
    }

    if (quotestatusid == 8)
    {
        // app rated
        switch (quotelobid) {
            case 1: //ppa
                url = QuickQuote_PPA_App + quoteid + '&Workflow=summary';
                break;
            case 2://hom
                url = QuickQuote_HOM_App + quoteid + '&Workflow=summary';
                break;
            case 3://dfr
                url = QuickQuote_DFR_App + quoteid + '&Workflow=summary';
                break;
            case 17://far
                url = QuickQuote_FAR_App + quoteid + '&Workflow=summary';
                break;
        }
    }

    return url;
}

function SummaryActionQuoteItem(quoteid,lobid,statusid)
{
    this.quoteId = quoteid;
    this.lobId = lobid;
    this.statusId = statusid;
}

function GetAllSelectedAppcheckBoxes(tableid)
{
    var $chkboxes = $("#" + tableid).find("tr input[type='checkbox']:checked");
    return $chkboxes.map(function () { return $(this).attr('quoteid') }).toArray();
}

function GetAllSelectedQuoteCheckboxes(tableid) {
    var $chkboxes = $("#" + tableid).find("tr input[type='checkbox']:checked");
    return $chkboxes.map(function () { return $(this).attr('id') }).toArray();
}