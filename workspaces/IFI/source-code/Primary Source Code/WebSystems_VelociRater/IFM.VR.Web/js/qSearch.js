
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
/// <reference path="vr.core.js" />



var ifm_qSearch = new function () {
    function SearchPanelParms(typeid, viewableLobIds, newQuoteLobId, lobTextName, lobIconUrl) {
        this.typeid = typeid;
        this.viewableLobIds = viewableLobIds;
        this.newQuoteLobId = newQuoteLobId;
        this.lobTextName = lobTextName;
        this.lobIconUrl = lobIconUrl;
    }

    var SearchPanelTypeList = new Array();
    SearchPanelTypeList.push(new SearchPanelParms(1, "1,1001", 1, "Auto Personal", "images/dark_auto.png"))
    SearchPanelTypeList.push(new SearchPanelParms(2, "2,3,1001", 2, "Home Personal", "images/dark_home.png"))
    SearchPanelTypeList.push(new SearchPanelParms(3, "17,1003", 17, "Farm", "images/policy_farm.png"))
    SearchPanelTypeList.push(new SearchPanelParms(4, "9,20,25,23,21,28,1002", 9, "Commercial", "images/policy_comm.png"))

    function GetSearchParmsByTypeid(typeId) {
        for (var ff = 0; ff < SearchPanelTypeList.length; ff++) {
            if (SearchPanelTypeList[ff].typeid == typeId)
                return SearchPanelTypeList[ff];
        }
        return null;
    }

    var col_Actions = 'Actions';
    var col_Id = 'Quote Id'
    var col_QuoteNum = 'Quote #';
    var col_Customer = 'Customer';
    var col_Premium = 'Premium';
    var col_Status = 'Status';

    // you will only use one of these at a time but it is ok to have both here as long as they are slightly different
    var col_LastModified = 'Last Modified';
    var col_LastModified_FriendlyDate = "Last Modified ";//space is important to discern it from the other 'Last Modified' column that is not friendly

    var col_LastModifiedBy = 'Last Modified User';
    var col_LobName = 'Line of Business';

    //function PriorsearchRequest(divResultId, agencyID, pageIndex, perPageCount, sortColumn, sortOrderDesc, quoteId, agentUserName, clientName, statusIds, lobIds, excludeLobIds) {
    //    this.divResultId = divResultId;
    //    this.agencyID = agencyID;
    //    pageIndex, perPageCount, sortColumn, sortOrderDesc, quoteId, agentUserName, clientName, statusIds, lobIds, excludeLobIds
    //}

    this.PerformQSearch = function(divResultId, agencyID, pageIndex, perPageCount, sortColumn, sortOrderDesc, quoteId, quoteNumber,
        agentUserName, clientName, statusIds, lobIds, excludeLobIds, SearchPanelParmIndex) {
        if (isNaN(perPageCount))
            perPageCount = 4;

        if (perPageCount > 200)
            perPageCount = 200;

        var panelParms = GetSearchParmsByTypeid(SearchPanelParmIndex)
        viewableLobs = panelParms.viewableLobIds;

        $("#" + divResultId).fadeTo('fast', .33);

        var viewableLobsList = viewableLobs.splitCSV();
        var searchLobs = lobIds.splitCSV();
        var searchAbleLobs = null;//searchLobs.combine(viewableLobsList);

        if (lobIds == "") {
            // show everything it can
            searchAbleLobs = viewableLobsList;
            lobIds = viewableLobsList.join();
        }
        else {
            // show only those specifically asked for                
            searchAbleLobs = ifm.vr.arrays.intersect(searchLobs, viewableLobsList); //searchLobs.combine(viewableLobsList);
            lobIds = searchAbleLobs.join();
        }
        var showable = false;
        showable = searchAbleLobs.length > 0;

        if (showable)
        { $("#" + divResultId).show(); } else {
            $("#" + divResultId).hide();
            return;
        }

        $("#" + divResultId + "_results").empty();
        $("#" + divResultId + "_results").append('<span style="font-size: 16pt; font-weight: bold;">Gathering results.....<span>');

        ifm.vr.vrdata.Quotes.GetQuotes(agencyID, pageIndex, perPageCount, sortColumn, sortOrderDesc, quoteId, quoteNumber, agentUserName, clientName, statusIds, lobIds, excludeLobIds, function (data) {
            data.controlId = divResultId;
            $("#" + divResultId + "_results").empty();
            //user editable search fields
            var txtClientNameId = 'txtClientName_' + divResultId;
            var txtQuoteId = 'txtQuoteId_' + divResultId;
            var txtQuoteNumberId = 'txtQuoteNum_' + divResultId;
            var ddStatusId = 'ddStatus_' + divResultId;
            var ddLobNameId = 'ddLobName_' + divResultId;
            var ddViewableLobNameId = 'ddViewableLobName_' + divResultId;
            var ddShowLobNameId = 'ddShowLobName_' + divResultId;
            var ddAgentUserId = 'ddAgentUserName_' + divResultId;
            var txtSortColumnNameId = 'txtSortColumn_' + divResultId;
            var txtSortDescId = 'txtSortDesc_' + divResultId;

            //pager labels - not directly editable
            var txtPageIndexId = 'txtPageIndex_' + divResultId;
            var txtPerPageCountId = 'txtPerPageCount_' + divResultId;

            try {
                if ($("#" + txtPerPageCountId).val().toString().trim() == '') // makes sure a blank per page can't be inserted
                {
                    $("#" + txtPerPageCountId).val('4');
                }
            }
            catch (err) {
            }
            var txtMaxPageId = 'txtMaxPage_' + divResultId; // not used here

            // do not url encode these that will be done on invocation of the method
            var doSearchFunction = 'ifm_qSearch.PerformQSearch("' + divResultId + '","' + agencyID + '",parseInt($("#' + txtPageIndexId + '").text()) -1,parseInt($("#' + txtPerPageCountId
                + '").text()),$("#' + txtSortColumnNameId + '").val(),$("#' + txtSortDescId + '").val(),$("#' + txtQuoteId + '").val(),$("#' + txtQuoteNumberId + '").val(),$("#' + ddAgentUserId
                + '").val(),$("#' + txtClientNameId + '").val(),$("#' + ddStatusId + '").val(),$("#' + ddLobNameId + '").val(),"' + excludeLobIds + '",' + SearchPanelParmIndex + ');';

            //    var doSearchFunction = 'ifm_qSearch.PerformQSearch("' + divResultId + '","' + agencyID + '","' + pageIndex + '","' + perPageCount + '","' + sortColumn
            //    + '","' + sortOrderDesc + '","' + quoteId + '","' + agentUserName + '","' + clientName + '","' + statusIds + '","' +
            //    lobIds + '","' + excludeLobIds + '");';

            html += '<script>';
            html += doSearchFunction;

            html += '</script>';
            data.searchJunc_Text = doSearchFunction;

            var html = '';
            html += '<div class="findTopLevelResults" id="' + divResultId + '_advancesearch' + '" style="display:none; width:100%; margin-bottom: 5px;" onkeypress=\'if (e.keyCode == 13){' + doSearchFunction + '}\' >'; // return false;

            html += '<table style="margin-left: 50px; width: 550px;">';

            html += '<tr>';
            html += '<td>';
            html += 'Customer: <br /> <input class="findCustomerName" id="' + txtClientNameId + '" value="' + clientName + '" type="text"/>';
            html += '</td>';
            html += '<td>';
            html += 'Quote Id: <br /> <input class="findQuoteId" id="' + txtQuoteId + '" value="' + quoteId + '" type="text"/>';
            html += '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td>';
            html += 'Quote #: <br /> <input id="' + txtQuoteNumberId + '" value="' + quoteNumber + '" type="text"/>';
            html += '</td>';
            html += '<td>';
            html += 'Sort  column: <br /> <input id="' + txtSortColumnNameId + '" value="' + sortColumn + '" type="text"/>';
            html += '<input style="width: 40px;" id="' + txtSortDescId + '" value="' + sortOrderDesc + '" title="Order Desc" type="text"/>';
            html += '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td>';
            html += 'Statuses: <br/> <input class="findStatuses" id="' + ddStatusId + '" value="' + statusIds + '" type="text"/>';
            html += '</td>';
            html += '<td>';
            html += 'Find Lobs: <br/> <input class="findLobs" id="' + ddLobNameId + '" value="' + lobIds + '" type="text"/>';
            html += '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td>';
            html += 'Agent User: <br/> <input class="findAgentIds" id="' + ddAgentUserId + '" value="' + agentUserName + '" type="text"/>';
            html += '</td>';
            html += '<td>';
            html += 'Viewable Lobs: <br/> <input class="findViewableLobs" id="' + ddViewableLobNameId + '" value="' + viewableLobs + '" type="text"/>';
            html += '</td>';
            html += '</tr>';

            html += '</table>';

            html += '</div>';

            html += "<div style='float:right;'>";
            html += "<span title='Expand/Collapse " + panelParms.lobTextName + "' onclick='' class='ExpandCollapseTriangle ui-accordion-header-icon ui-icon ui-icon-triangle-1-s'></span>";
            html += "</div>";
            html += "<div style='float:right; margin-right:30px;'>";
            html += "<input class='StandardButton' onclick='window.location.href = \"MyVelocirater.aspx?newQuote=" + panelParms.newQuoteLobId + "\";' style='text-align: center; width:220px;height:25px;'value='Create New " + panelParms.lobTextName + " Quote' />";
            html += "</div>";
            html += "<div title='Expand/Collapse " + panelParms.lobTextName + " ' style='cursor:pointer;' onclick=''>";
            html += "<img style='with:25px;height:26px;' src='" + panelParms.lobIconUrl + "'/>";
            html += panelParms.lobTextName;
            html += "</div>";
            html += "<hr />";

            html += '<table id="' + divResultId + '_results" class="clientList" mydivid="' + divResultId + '" style="color: black;background-color: White; border-color: White; border-width: 0px; border-style: None; border-collapse: collapse;" cellspacing="0" cellpadding="4">';
            var columns = new Array();

            if (isStaff && columns.contains(col_Id) == false)
                columns.push(col_Id);
            columns.push(col_Actions);
            columns.push(col_QuoteNum);
            columns.push(col_Customer);
            columns.push(col_Premium);
            columns.push(col_Status);
            //columns.push(col_LastModified);
            if (isStaff && columns.contains(col_LastModified) == false) {
                //show one of these not both
                //columns.push(col_LastModified);
                columns.push(col_LastModified_FriendlyDate);
            }

            var sortableColumns = new Array();
            sortableColumns.push(col_Id);
            sortableColumns.push(col_QuoteNum);
            sortableColumns.push(col_Customer);
            sortableColumns.push(col_Premium);
            sortableColumns.push(col_Status);

            //show one of these not both
            //sortableColumns.push(col_LastModified);
            sortableColumns.push(col_LastModified_FriendlyDate);

            if (data.QueryFullCount > 0) {
                // has results
                html += ifm_qSearch.CreateHeader(columns, sortableColumns, txtSortColumnNameId, txtPageIndexId, txtSortDescId, doSearchFunction);
                for (var i = 0; i < data.Results.length; i++) {
                    var result = data.Results[i];
                    html += CreateDataRow(result, columns);
                }
            }
            else {
                // no results
                html += "No results returned."
            }
            html += '</table>';
            html += ifm_qSearch.CreateGridPager(data, pageIndex, perPageCount);
            // even if no data is returned
            $("#" + data.controlId).html(html);
            $("#" + data.controlId).fadeTo('fast', 1.0);
        });



    }

    this.CreateGridPager = function (data, pageIndex, perPageCount) {
        //data.searchJunc_Text
        var txtPageIndexId = 'txtPageIndex_' + data.controlId;
        var txtPerPageCountId = 'txtPerPageCount_' + data.controlId;
        var txtMaxPageId = 'txtMaxPage_' + data.controlId;
        var availablePages = AvailablePages(data.QueryFullCount, perPageCount);

        var html = '<br/>';

        html += '<div style="width:100%; text-align: right;"  onkeypress=\'if (e.keyCode == 13){e.stopPropagation();' + data.searchJunc_Text + ' return false;}\'>';
        html += '<div style="">';

        html += 'Total Results: ' + data.QueryFullCount.toString();
        html += '<br />';
        html += 'Page: <label id="' + txtPageIndexId + '">' + (pageIndex + 1).toString() + '</label> of '
            + '<label id="' + txtMaxPageId + '">' + availablePages.toString() + '</label>';
        html += '<br />';
        html += 'Page Size: <label style="cursor: pointer;text-decoration:underline;" title="Click to change page size." id="' + txtPerPageCountId + '" onclick=\'ifm_qSearch.ChangePagerSize($(this).attr("id"),"' + txtPageIndexId + '");\'>' + perPageCount.toString() + ' </label><br/><br/>';

        var pagerStartPage = 0;
        if (pageIndex >= 2) {
            pagerStartPage = pageIndex - 2;
        }

        // page back
        if (pageIndex > 0) {
            html += '<span title="First Page" style="cursor: pointer;text-decoration:underline;margin-left: 2px; font-size: 10px;" onclick=\'ifm_qSearch.Page_GoToPage("' + txtPageIndexId + '","1");' + data.searchJunc_Text + '\' > &lt&lt </span>';
            html += '<span title="Previous Page" style="cursor: pointer;text-decoration:underline;margin-left: 2px; font-size: 10px;" onclick=\'if(ifm_qSearch.Page_GoBack("' + txtPageIndexId + '"))' + data.searchJunc_Text + '\' > ... </span>';
        }
        else {
            //html += '<span title="Previous Page" style="margin-left: 20px;" onclick=\'\' > &lt </span>';
        }

        // do page jump links
        // < 1 2 3 4 5 >

        var endPagerPage = pagerStartPage + 5;
        for (pagerStartPage; pagerStartPage < endPagerPage; pagerStartPage++) {
            if (pagerStartPage < availablePages) {
                if (pagerStartPage == pageIndex) {
                    html += '<span title="" style="cursor: pointer;text-decoration:underline;margin-left: 2px;font-size: 10px;color:blue;" onclick=\'ifm_qSearch.Page_GoToPage("' + txtPageIndexId + '",$(this).text());' + data.searchJunc_Text + '\' > ' + (pagerStartPage + 1).toString() + ' </span>';
                }
                else {
                    html += '<span title="" style="cursor: pointer;text-decoration:underline;margin-left: 2px;font-size: 10px;" onclick=\'ifm_qSearch.Page_GoToPage("' + txtPageIndexId + '",$(this).text());' + data.searchJunc_Text + '\' > ' + (pagerStartPage + 1).toString() + ' </span>';
                }
            }
        }

        //page forward
        if (pageIndex + 1 < availablePages) {
            html += '<span title="Next Page" style="cursor: pointer;text-decoration:underline;margin-left: 2px;font-size: 10px;" onclick=\'if(ifm_qSearch.Page_Forward("' + txtPageIndexId + '","' + txtMaxPageId + '"))' + data.searchJunc_Text + '\' > ... </span>';
            html += '<span title="Last Page" style="cursor: pointer;text-decoration:underline;margin-left: 2px;font-size: 10px;" onclick=\'ifm_qSearch.Page_GoToPage("' + txtPageIndexId + '","' + availablePages + '");' + data.searchJunc_Text + '\' > &gt&gt </span>';
        }
        else {
            //html += '<span title="Next Page" style="margin-left: 20px;" onclick=\'\' > &gt </span>';
        }

        html += '</div>';
        html += '</div>';
        return html;
    }

    this.ChangePagerSize = function (senderId, txtPageIndexId) {
        $("#" + senderId).hide();
        var s = "#" + senderId;
        var currentPageSize = $(s).text();
        var tempPagerBoxId = "tempPageSize" + Math.random().toString().replace('.', '');
        $("#" + senderId).after('<input value="' + currentPageSize + '" id="' + tempPagerBoxId + '" title="Press Enter when done." type="text" style="width: 40px;" onkeyup=\'$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));$("' + s + '").text($(this).val());\' onblur=\'$(this).remove();$("#' + senderId + '").show(); \' />');
        $("#" + tempPagerBoxId).focus();
        $("#" + txtPageIndexId).text("1");
    }

    function AvailablePages(resultCount, perPageCount) {
        return Math.ceil(resultCount / perPageCount);
    }

    this.CreateHeader = function (columns, sortableColumns, txtSortColumnNameId, txtPageIndexId, txtSortDescId, doSearchFunction) {
        var html = '';
        html += '<tr class="ui-widget-header" style="min-height: 20px;">';
        //html += '<tr style="background-color: #f49000;height: 20px;" class="">';
        for (var i = 0; i < columns.length; i++) {
            html += '<th>';
            if (sortableColumns.contains(columns[i])) {
                var line = '<span style="text-decoration:underline; cursor: pointer;" onclick=\'ifm_qSearch.ColumnClicked("' + txtSortColumnNameId + '","' + txtSortDescId + '","' + txtPageIndexId + '","' + columns[i] + '");' + doSearchFunction + '\'>' + columns[i] + '</span>';
                html += line;
            }
            else {
                html += '<span style="">' + columns[i] + '</span>';
            }

            html += '</th>';
        }
        html += '</tr>';
        return html;
    }

    this.ColumnClicked = function (txtSortColumnNameId, txtSortDescId, txtPageIndexId, ColumnName) {
        if ($("#" + txtSortColumnNameId).val() == ColumnName) {
            if ($("#" + txtSortDescId).val() == 'true') {
                $("#" + txtSortDescId).val('false');
            }
            else {
                $("#" + txtSortDescId).val('true');
            }
        }
        else {
            $("#" + txtSortDescId).val('false');
        }

        $("#" + txtSortColumnNameId).val(ColumnName);
        $("#" + txtPageIndexId).text('1');
    }

    function CreateDataRow(data, columns) {
        html = '';
        html += '<tr>';
        var tdStyle = 'color: #666666; border:1px solid #EFEFEF; border-collapse:collapse;';

        if (columns.contains(col_Id)) {
            html += '<td style="' + tdStyle + 'width: 90px; text-align: center;">';
            html += data.QuoteId;
            html += '</td>'
        }

        if (columns.contains(col_Actions)) {
            var onchangeLogic = 'if($(this).children("option").filter(":selected").text() == "Archive"){if(confirm("Archive this quote?")){Post(this);}else{$(this).removeAttr("selected").find("option:first").attr("selected", "selected");} return true;}else {Post(this);}';
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += '<select onchange=\'' + onchangeLogic + '\' style="width: 90px;"">';
            for (var i = 0; i < data.AvilableActions.length; i++) {
                html += '<option value="' + data.AvilableActions[i].value + '">' + data.AvilableActions[i].label + '</option>';
            }
            html += '</select>';
            html += '</td>'
        }

        if (columns.contains(col_QuoteNum)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += data.QuoteNumAndDescription;
            html += '</td>'
        }

        if (columns.contains(col_Customer)) {
            html += '<td style="' + tdStyle + '">';
            html += data.ClientName;
            html += '</td>'
        }

        if (columns.contains(col_Premium)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += data.FormatedPremium;
            html += '</td>'
        }

        if (columns.contains(col_Status)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += data.FriendlyStatus;
            html += '</td>'
        }

        if (columns.contains(col_LastModified)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += data.LastModified;
            html += '</td>'
        }
        if (columns.contains(col_LastModified_FriendlyDate)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += data.LastModified_FriendlyDate;
            html += '</td>'
        }

        if (columns.contains(col_LastModifiedBy)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            if (data.LastModifiedByaIfmStaffMember)
                html += data.LastModifiedByUsername + '_IFMStaff';
            else
                html += data.LastModifiedByUsername;
            html += '</td>'
        }

        if (columns.contains(col_LobName)) {
            html += '<td style="' + tdStyle + 'width: 90px;">';
            html += data.LobName;
            html += '</td>'
        }

        html += '</tr>';
        return html;
    }

    this.Page_GoToPage = function (txtPageIndexId, pageNumber) {
        $("#" + txtPageIndexId).text(pageNumber);
    }

    this.Page_GoBack = function (txtPageIndexId) {
        var currentPage = parseInt($("#" + txtPageIndexId).text());
        var newPage = currentPage - 1;
        if (newPage > 0) {
            $("#" + txtPageIndexId).text(newPage);
            return true;
        }

        return false;
    }

    this.Page_Forward = function (txtPageIndexId, txtMaxPageId) {
        var maxpage = parseInt($("#" + txtMaxPageId).text());
        var currentPage = parseInt($("#" + txtPageIndexId).text());
        var newPage = (currentPage + 1.0).toString();
        if (currentPage < maxpage) {
            $("#" + txtPageIndexId).text(newPage);
            return true;
        }

        return false;
    }
}; // qSearch END

