/// <reference path="C:\Users\maamo\Documents\Visual Studio 2015\Projects\V1_2\IFM.VR.Web\js/vr.core.js" />

ifmApp.controller('CtrlQuoteSearchMaster', function ($scope) {
    var vm = this;
    vm.controllerName = "CtrlQuoteSearchMaster";
      
    vm.quoteNumber = "";
    vm.customerName = "";
    vm.status = "";
    vm.lob = 'All LOBs';
    vm.agentUser = '';
    vm.timeFrame = '90';
    
    vm.sections = [1, 2, 3, 4];

   
    vm.sectionName = "Personal Auto";
    vm.supportedLobs = [1];
    vm.resultsData = null;
    vm.perPageCount = '8';
    vm.pageIndex = '0';
    vm.sortColumn = '';
    vm.performingQuoteSearch = false;

    

    
    
    vm.performSearch = function () {
        vm.resultsData = null;
        vm.performingQuoteSearch = true;

        var agencyid = "", sortOderDesc = "", quoteid = "",  agentUsername = "", statusIds = "", lobIds = "", excludeLobids = "";
        agencyid = "17";               
        lobIds = "1";
                

        ifm.vr.vrdata.Quotes.GetQuotes(agencyid, vm.pageIndex, vm.perPageCount, vm.sortColumn, sortOderDesc, quoteid, vm.quoteNumber, agentUsername, vm.customerName, vm.status, lobIds, excludeLobids, function (data) {
            vm.performingQuoteSearch = false;

            if (data && data != null) {
                $scope.$apply(function () {
                    vm.resultsData = data;
                });
            }
            else {
                $scope.$apply(function () {
                    vm.resultsData = null;
                });
            }

        });

    };

    //vm.performSearch();
    

}); // End Controller
