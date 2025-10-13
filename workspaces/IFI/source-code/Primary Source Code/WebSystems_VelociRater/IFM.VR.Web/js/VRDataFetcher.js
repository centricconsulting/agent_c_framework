
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />



// Provides many methods to obtain various types of data commonly used.
var VRData = new function () {
      
    
    this.TownShip = new function () {

        // Gathers a list of townships based on the county name provided. This only works for Indiana counties.
        // Usage: GetTownshipsByCountyName('Marion',function(data){});
        // Result: A array of Ids/Townships - data = array[int(id),string(townshipname)]
        this.GetTownshipsByCountyName = function (cntyName, versionId, stateId, lobId, effDate, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/GetTownshipsForCountyName.ashx?countyname=' + encodeURIComponent(cntyName.trim())
            if (versionId) {
                genHandler = genHandler + '&versionId=' + encodeURIComponent(versionId.trim());
            } else {
                genHandler = genHandler + '&stateId=' + encodeURIComponent(stateId) + '&lobId=' + encodeURIComponent(lobId) + '&effDate=' + encodeURIComponent(effDate.trim());
            }
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) {
                callBack(data);
            });
        };

        // this function should not be in here - find somewhere else for it
        this.GetTownshipsByCountyNameBindToDropdown = function (ddTownshipId, countyName, versionId, hiddenId, stateId, lobId, effDate) {
            $("#" + ddTownshipId + " option").each(function (i, option) { $(option).remove(); }); // removes all current options
            if (versionId) {
                this.GetTownshipsByCountyName(countyName, versionId, function (data) {
                    if (data.length > 0) {
                        $("#" + ddTownshipId).append($('<option></option>').val('').html('-- Select --'));
                        for (var ii = 0; ii < data.length; ii++) {
                            $("#" + ddTownshipId).append($('<option></option>').val(data[ii][0]).html(data[ii][1]));
                        }
                        $("#" + ddTownshipId).val($("#" + hiddenId).val());
                    } else {
                        //Added 11/13/2019 for bug 41224 MLW - some IL counties return no townships, use Various as township
                        if (stateId === "15") {
                            $("#" + ddTownshipId).append($('<option></option>').val('').html('-- Select --'));
                            $("#" + ddTownshipId).append($('<option></option>').val('421').html('Various'));
                            $("#" + ddTownshipId).val($("#" + hiddenId).val());
                        }
                    }
                });
            }
            else {
                this.GetTownshipsByCountyName(countyName, versionId, stateId, lobId, effDate, function (data) {
                    if (data.length > 0) {
                        $("#" + ddTownshipId).append($('<option></option>').val('').html('-- Select --'));
                        for (var ii = 0; ii < data.length; ii++) {
                            $("#" + ddTownshipId).append($('<option></option>').val(data[ii][0]).html(data[ii][1]));
                        }
                        $("#" + ddTownshipId).val($("#" + hiddenId).val());
                    } else {
                        //Added 11/13/2019 for bug 41224 MLW - some IL counties return no townships, use Various as township
                        if (stateId === "15") {
                            $("#" + ddTownshipId).append($('<option></option>').val('').html('-- Select --'));
                            $("#" + ddTownshipId).append($('<option></option>').val('421').html('Various'));
                            $("#" + ddTownshipId).val($("#" + hiddenId).val());
                        }
                    }
                });
            }
        }
    } // TownShip END

    this.AdditionalInterest = new function () {

        // Searches for additional interest records. If you provide the commName it will assume you are searching by commercial name otherwise it will search by first/middle/last names.
        // Usage: GetAdditionalInterests('','Tom','M','Smith',function(data){});
        // Result: A array of IFM.VR.Common.Helpers.AdditionalInterest() 
        this.GetAdditionalInterests = function (commName, firstName, middleName, lastName, callBack) {

            if (ifm.vr.currentQuote.agencyId != "-1") { // agency restrictions will be imposed on the server side
                if (commName.trim().length > 2 || firstName.trim().length > 2 || middleName.trim().length > 2 || lastName.trim().length > 2) {
                    var genHandler = '';

                    //updated 6/27/2019 for Endorsements: replaced master_quoteID with master_quoteIdOrPolicyIdAndImageNum
                    if (commName.trim().length > 2) {
                        //do commercial lookup
                        genHandler = 'GenHandlers/Vr_Pers/AdditionalInterestLookup.ashx?quoteid=' + master_quoteIdOrPolicyIdAndImageNum + '&agencyId=' + ifm.vr.currentQuote.agencyId + '&commercialName=' + encodeURIComponent(commName) + '&ppopo=' + Math.random().toString().replace('.', '');
                    }
                    else {
                        // do non-commercial lookup
                        genHandler = 'GenHandlers/Vr_Pers/AdditionalInterestLookup.ashx?quoteid=' + master_quoteIdOrPolicyIdAndImageNum + '&agencyId=' + ifm.vr.currentQuote.agencyId + '&firstName=' + encodeURIComponent(firstName) + '&middleName=' + encodeURIComponent(middleName) + '&lastName=' + encodeURIComponent(lastName) + '&ppopo=' + Math.random().toString().replace('.', '');
                    }

                    DoingAdditionalInterestLookup = true;
                    $.getJSON(genHandler, {
                        dataType: "json",
                        data: "",
                        cache: false,
                        format: "json"
                    })
                    .done(function (data) {
                        callBack(data);
                    });
                }

            }
        };

    };// AdditionalInterest END
    
    this.Applicant = new function () {
        // Searches for Applicant records based on the provided parms.
        // Usage: GetApplicants(101,'Tom','Smith','','',function(data){});
        // Results: A array of QuickQuote.CommonObjects.QuickQuoteApplicant()
        this.GetApplicants = function (agencyID,firstNameText, lastNameText, zipCodeText, ssnText,callBack) {
            if (ssnText.trim().length == 11 | zipCodeText.trim().length >= 5 | (firstNameText.trim().length >= 0 & lastNameText.trim().length >= 2)) {
                if (ssnText != null)
                    ssnText = ssnText.replace(/-/g, "")

                // agencyID will still be checked against session data to confirm that the current has access to the agency
                var genHandler = 'GenHandlers/Vr_Pers/applicantlookup.ashx?agencyId=' + encodeURIComponent(agencyID) + '&firstName=' + encodeURIComponent(firstNameText) + '&lastName=' + encodeURIComponent(lastNameText) + '&zip=' + encodeURIComponent(zipCodeText) + '&ssn=' + encodeURIComponent(ssnText) + '&ppopo=' + Math.random().toString().replace('.', '');
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                .done(function (data) {
                    callBack(data);
                });
            }
        }; //this.GetApplicants END

    }; // Applicant END

    this.Client = new function () {

        // Searches for Personal Client records.
        // Usage: GetPersonalClients(101,'Tom','Smith','','','',function(data){});
        // Result: A array of QuickQuote.CommonObjects.QuickQuoteClient()
        this.GetPersonalClients = function (agencyID, firstname, lastname, city, zip, ssn, callBack) {
            if (agencyID != "-1") {
                if ((lastname.trim().length > 2 || (lastname.trim().length > 1 & firstname.trim().length > 0)) | ssn.trim().length == 11) {
                    if (zip.trim().length > 5) {
                        zip = zip.substring(0, 5);
                    }

                    if (ssn != null)
                        ssn = ssn.replace(/-/g, "")

                    // agencyID will still be checked against session data to confirm that the current has access to the agency
                    var genHandler = 'GenHandlers/Vr_Pers/ClientNameAutoComplete.ashx?miniClientSearch=yes&agencyId=' + encodeURIComponent(agencyID) + '&firstname=' + encodeURIComponent(firstname) + '&lastname=' + encodeURIComponent(lastname) + '&city=' + encodeURIComponent(city) + '&zip=' + encodeURIComponent(zip) + '&ssn=' + encodeURIComponent(ssn) + '&ppopo=' + Math.random().toString().replace('.', '');
                    $.getJSON(genHandler, {
                        dataType: "json",
                        data: "",
                        cache: false,
                        format: "json"
                    })
                    .done(function (data) { callBack(data); });
                }
            }

        }; // this.GetPersonalClients END

        // Searches for Commercial Client records.
        // Usage: GetCommercialClients(101,'A1 Landscape','','','',function(data){});
        // Result: A array of QuickQuote.CommonObjects.QuickQuoteClient()
        // Data Structure - Look it up yourself or just set break point 
        this.GetCommercialClients = function (agencyID,businessName, city, zip,ssn,callBack) {

            if (agencyID != "-1") {
                if (businessName.trim().length > 4 | ssn.trim().length == 11) {
                    if (zip.length > 5) {
                        zip = zip.substring(0, 5);
                    }

                    if (ssn != null)
                        ssn = ssn.replace(/-/g, "")

                    // agencyID will still be checked against session data to confirm that the current has access to the agency
                    var genHandler = 'GenHandlers/Vr_Pers/ClientNameAutoComplete.ashx?miniClientSearch=yes&agencyId=' + encodeURIComponent(agencyID) + '&businessname=' + encodeURIComponent(businessName) + '&city=' + encodeURIComponent(city) + '&zip=' + encodeURIComponent(zip) + '&ssn=' + encodeURIComponent(ssn) + '&ppopo=' + Math.random().toString().replace('.', '');
                    $.getJSON(genHandler, {
                        dataType: "json",
                        data: "",
                        cache: false,
                        format: "json"
                    })
                    .done(function (data) { callBack(data); });
                }
            }


        }; // this.GetCommercialClients END

    };  // Client END

    this.Quotes = new function () {

        // Searches for quotes.
        // Usage: GetQuotes(...., callBack(data){});
        // Results: An array QuoteResultSet which will contain a property of QuoteLookupSearch().
        this.GetQuotes = function (agencyID, pageIndex, perPageCount, sortColumn, sortOrderDesc, quoteId, quoteNumber, agentUserName, clientName, statusIds, lobIds, excludeLobIds, callBack) {

            var genHandler = 'GenHandlers/Vr_Pers/QuoteLookup.ashx?';
            genHandler += 'agencyID=' + encodeURIComponent(agencyID);
            genHandler += '&pageIndex=' + encodeURIComponent(pageIndex);
            genHandler += '&perPageCount=' + encodeURIComponent(perPageCount);
            genHandler += '&sortColumn=' + encodeURIComponent(sortColumn);
            genHandler += '&sortOrderDesc=' + encodeURIComponent(sortOrderDesc);
            genHandler += '&quoteId=' + encodeURIComponent(quoteId);
            genHandler += '&quoteNumber=' + encodeURIComponent(quoteNumber) + "%";
            genHandler += '&agentUserName=' + encodeURIComponent(agentUserName);
            genHandler += '&clientName=' + encodeURIComponent(clientName);
            genHandler += '&statusIds=' + encodeURIComponent(statusIds);
            genHandler += '&lobIds=' + encodeURIComponent(lobIds);
            genHandler += '&excludeLobIds=' + encodeURIComponent(excludeLobIds);
            genHandler += '&rnd=' + encodeURIComponent(Math.random().toString().replace('.', '')); // rnd eliminates browser caching

            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) {
                callBack(data);
            });

        }; // GetQuotes END

    }; // Quotes END

    this.ProtectionClass = new function () {

        // Searches for Protection classes based on city or county name.
        // Usage GetProtectionClass('Fort Wayne',true, 16, function(data){});
        // Result: An array ProtectionClassLookupResult() 
        // Data Structure - Township[string], County[string], ProtectionClass[string], FootNote[string], PC_ID[string]
        this.GetProtectionClass = function (cityOrCountyName,isCity, stateId, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/ProtectionClassLookup.ashx?city=' + encodeURIComponent(cityOrCountyName.trim()) + '&isCity=' + encodeURIComponent(isCity) + '&isComm=false&stateId=' + encodeURIComponent(stateId);
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) { callBack(data);});

        }; // GetProtectionClass END

    }; // ProtectionClass END

    this.RiskGrade = new function () {

        // Searches for RiskGrades based on riskType and a search-term.
        // SearchCommRiskGrades(1,'');
        // Result: An array of IFM.VR.Common.Helpers.CGL.RiskGradeLookupResult
        this.SearchCommRiskGrades = function (riskTypeId, searchText, versionID, callBack)
        {
            var genHandler = 'GenHandlers/Vr_Pers/CommRiskGrades.ashx?query=yes&riskTypeId=' + encodeURIComponent(riskTypeId) + '&searchText=' + encodeURIComponent(searchText) + '&versionid=' + encodeURIComponent(versionID) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) {
                callBack(data);
            });
        }

    }; // END RiskGrade

    this.ClassCode = new function () {

        // Searches for Class Codes based on Class Code and a search-term.
        // Result: An array of IFM.VR.Common.Helpers.WCP.WCPClassCodeLookupResult
        this.SearchClassCodes = function (ClassCode, SearchText, callBack) {
            var genHandler = 'GenHandlers/Vr_Comm/WCPClassCodeSearch.ashx?query=yes&ClassCode=' + encodeURIComponent(ClassCode) + '&searchText=' + encodeURIComponent(SearchText) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) {
                callBack(data);
            });
        }

        // Searches for PIO (Property in the Open) Class Codes based on Class Code and a search-term.
        // Result: An array of IFM.VR.Common.Helpers.CPR.PIOClassCodeLookupResult
        this.SearchPIOClassCodes = function (ClassCode, SearchText, StateId, EffectiveDate, CompanyId, callBack) {
            var genHandler = 'GenHandlers/Vr_Comm/PIOClassCodeLookup.ashx?query=yes&ClassCode=' + encodeURIComponent(ClassCode) + '&searchText=' + encodeURIComponent(SearchText) + '&StateId=' + encodeURIComponent(StateId) + "&EffectiveDate=" + encodeURIComponent(EffectiveDate) + "&CompanyId=" + CompanyId + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
                .done(function (data) {
                    callBack(data);
                });
        }

        // Searches for Building Class Codes based on Class Code and a search-term.
        // Result: An array of IFM.VR.Common.Helpers.CPR.CPRBuildingClassCodeLookupResult
        this.SearchCPRBuildingClassCodes = function (ClassCode, SearchText, callBack) {
            var genHandler = 'GenHandlers/Vr_Comm/CPRBuildingClassCodeLookup.ashx?query=yes&ClassCode=' + encodeURIComponent(ClassCode) + '&searchText=' + encodeURIComponent(SearchText) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
                .done(function (data) {
                    callBack(data);
                });
        }

        // Searches for CPR Earthquake Classifications based on Class Code and a search-term.
        // Result: An array of IFM.VR.Common.Helpers.CPR.CPRBuildingClassCodeLookupResult
        this.SearchCPREQClassifications = function (SearchId, SearchText, callBack) {
            var genHandler = 'GenHandlers/Vr_Comm/CPREQClassificationLookup.ashx?query=yes&SearchId=' + encodeURIComponent(SearchId) + '&searchText=' + encodeURIComponent(SearchText) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
                .done(function (data) {
                    callBack(data);
                });
        }

        // Searches for Crime Class Codes based on Class Code and a search-term.
        // Result: An array of IFM.VR.Common.Helpers.CPR.CPPCrimeClassCodeLookupResult
        this.SearchCPPCrimeClassCodes = function (ClassCode, SearchText, callBack) {
            var genHandler = 'GenHandlers/Vr_Comm/CPPCrimeClassCodeLookup.ashx?query=yes&ClassCode=' + encodeURIComponent(ClassCode) + '&searchText=' + encodeURIComponent(SearchText) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
                .done(function (data) {
                    callBack(data);
                });
        }

    }; // END ClassCode



    this.RoutingNumber = new function () {

        // Searches for Bank Name based on the provided routing number.
        // Usage: GetBankNameFromRoutingNumber(123456789,function(data){});
        // Results: "Fake Bank LLC"
        this.GetBankNameFromRoutingNumber = function (routingNumber, callBack) {
            if (routingNumber.trim().length == 9) {
                var genHandler = 'GenHandlers/Vr_Pers/BankNameLookup.ashx?routingnumber=' + encodeURIComponent(routingNumber);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                .done(function (data) {
                    callBack(data);
                });
            }
            else {
                callBack(null);
            }
        };//GetBankNameFromRoutingNumber END

    }; // RoutingNumber END

    this.VIN = new function () {

        // Searches for vehicle information based on the provided parms.
        // Usage: GetFromVinOrMakeModelYear('','Dodge','Neon','1998',function(data){}); - Searches by Make/Model/Year
        // Alternative Usage: GetFromVinOrMakeModelYear('63ge6d6g363g6g63','','','',function(data){}); - Searches by VIN
        // Results: An array of VinLookupResult()
        // Data Structure - Vin[string], Make[string], Model[string],Year[int],BodyTypeId[string],AntiTheftDescription[string],RestraintDescription[string],CollisionSymbol[string],CompSymbol[string],PerformanceTypeText[string],BodyTypeText[string]
        this.GetFromVinOrMakeModelYear = function (Vin, Make, Model, Year, VersionId, PolicyId, PolicyImageNum, VehicleNum, IsNewBusiness, effectiveDate, callBack) {
            
            
            if (Vin.trim().length > 0 || (Model.trim().length > 0 && Make.trim().length > 0 && Year.trim().length == 4)) {
                //Updated 10/13/2022 for task 75263 MLW
                //Updated 07/22/2021 to send VersionId MLW - versioning added with CAP Endorsements
                //var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Vin=' + encodeURIComponent(Vin) + '&Model=' + encodeURIComponent(Model) + '&Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make) + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate);
                //var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Vin=' + encodeURIComponent(Vin) + '&Model=' + encodeURIComponent(Model) + '&Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make) + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate) + '&VersionId=' + encodeURIComponent(VersionId);
                var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Vin=' + encodeURIComponent(Vin) + '&Model=' + encodeURIComponent(Model) + '&Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make) + '&EffectiveDate=' + encodeURIComponent(effectiveDate) + '&VersionId=' + encodeURIComponent(VersionId) + '&PolicyId=' + encodeURIComponent(PolicyId) + '&PolicyImageNum=' + encodeURIComponent(PolicyImageNum) + '&VehicleNum=' + encodeURIComponent(VehicleNum) + '&IsNewBusiness=' + encodeURIComponent(IsNewBusiness);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                .done(function (data) {
                    callBack(data);
                });
            }
            else {
                callBack(null);
            }
        };

        // Searches for vehicle information based on the VIN.        
        // Usage: GetFromVinOrMakeModelYear('63ge6d6g363g6g63','','','',function(data){}); - Searches by VIN
        // Results: An array of VinLookupResult()
        // Data Structure - Vin[string], Make[string], Model[string],Year[int],BodyTypeId[string],AntiTheftDescription[string],RestraintDescription[string],CollisionSymbol[string],CompSymbol[string],PerformanceTypeText[string],BodyTypeText[string]
        this.GetFromVIN = function (Vin, VersionId, PolicyId, PolicyImageNum, VehicleNum, IsNewBusiness, effectiveDate, callBack) {
            if (Vin.trim().length > 0) {
                //Updated 10/13/2022 for task 75263 MLW
                //var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Vin=' + encodeURIComponent(Vin) + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate);
                var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Vin=' + encodeURIComponent(Vin) + '&EffectiveDate=' + encodeURIComponent(effectiveDate) + '&VersionId=' + encodeURIComponent(VersionId) + '&PolicyId=' + encodeURIComponent(PolicyId) + '&PolicyImageNum=' + encodeURIComponent(PolicyImageNum) + '&VehicleNum=' + encodeURIComponent(VehicleNum) + '&IsNewBusiness=' + encodeURIComponent(IsNewBusiness);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                .done(function (data) { callBack(data); });
            }
            else
            {
                callBack(null);
            }
        }; // GetFromVIN END

        // Searches for vehicle information based on the Make/Model/Year.
        // Usage: GetFromVinOrMakeModelYear('','Dodge','Neon','1998',function(data){}); - Searches by Make/Model/Year        
        // Results: An array of VinLookupResult()
        // Data Structure - Vin[string], Make[string], Model[string],Year[int],BodyTypeId[string],AntiTheftDescription[string],RestraintDescription[string],CollisionSymbol[string],CompSymbol[string],PerformanceTypeText[string],BodyTypeText[string]
        this.GetFromMakeModelYear = function (Model, Make, Year, VersionId, PolicyId, PolicyImageNum, VehicleNum, IsNewBusiness, effectiveDate, callBack) {
            if (Model.trim().length > 0 && Make.trim().length > 0 && Year.trim().length == 4) {
                //Updated 10/13/2022 for task 75263 MLW
                //var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Model=' + encodeURIComponent(Model) + '&Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make) + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate);
                var genHandler = 'GenHandlers/Vr_Pers/VinHandler.ashx?Model=' + encodeURIComponent(Model) + '&Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make) + '&EffectiveDate=' + encodeURIComponent(effectiveDate) + '&VersionId=' + encodeURIComponent(VersionId) + '&PolicyId=' + encodeURIComponent(PolicyId) + '&PolicyImageNum=' + encodeURIComponent(PolicyImageNum) + '&VehicleNum=' + encodeURIComponent(VehicleNum) + '&IsNewBusiness=' + encodeURIComponent(IsNewBusiness);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                .done(function (data) {
                    callBack(data);
                });
            }
            else
            {
                callBack(null);
            }
        }; // GetFromMakeModelYear END

        //Finds all available Models for a given Make on a given year.
        //Usage: GetAvailableModelsFromMakeAndYear('Dodge',1998,function(data){});
        //Result: An array of AutoCompleteEntry().
        //Data Structure: id[int],label[string],value[string]
        this.GetAvailableModelsFromMakeAndYear = function (Make, Year, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/MakeModelLookup.ashx?GetModels=' + encodeURIComponent(Make) + '&Year=' + encodeURIComponent(Year);
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) { callBack(data); });
        }; //GetAvailableModelsFromMakeAndYear END

        this.GetMakeListFromYear = function (Year, callBack) {
            if ((Year.trim().length == 4)) {
                var genHandler = 'GenHandlers/Vr_Pers/MakeListHandler.ashx?Year=' + encodeURIComponent(Year);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                    .done(function (data) {
                        callBack(data);
                    });
            }
            else {
                callBack(null);
            }
        };

        this.GetModelListFromMake = function (Year, Make, callBack) {
            if ((Make.trim().length > 0 && Year.trim().length == 4)) {
                var genHandler = 'GenHandlers/Vr_Pers/ModelListHandler.ashx?Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                    .done(function (data) {
                        callBack(data);
                    });
            }
            else {
                callBack(null);
            }
        };

    }; //VIN END

    //Added 06/30/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    this.VINCAP = new function () {

        // Searches for RACA Symbols based on VIN
        //Results:
        // Data Structure - Vin[string]
        this.GetRACASymbolsFromVIN = function (Vin, callBack) {
            if (Vin.trim().length > 0) {
                var genHandler = 'GenHandlers/Vr_Comm/RACAHandler.ashx?Vin=' + encodeURIComponent(Vin);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                    .done(function (data) {
                        callBack(data);
                    });
            }
            else {
                callBack(null);
            }
        };

        // Searches for vehicle information based on the provided parms.
        // Usage: GetFromVinOrMakeModelYear('','Dodge','Neon','1998','','','','',function(data){}); - Searches by Make/Model/Year
        // Alternative Usage: GetFromVinOrMakeModelYear('63ge6d6g363g6g63','','','','','','','',function(data){}); - Searches by VIN
        // Results: An array of VinLookupResult()
        // Data Structure - Vin[string], Make[string], Model[string],Year[int],PolicyId[string],PolicyImageNum[string],VehicleNum[string],VersionId[string]
        this.GetFromVinOrMakeModelYear = function (Vin, Make, Model, Year, PolicyId, PolicyImageNum, VehicleNum, VersionId, callBack) {            
            if (Vin.trim().length > 0 || (Model.trim().length > 0 && Make.trim().length > 0 && Year.trim().length == 4)) {
                var genHandler = 'GenHandlers/Vr_Comm/VinHandlerCAP.ashx?Vin=' + encodeURIComponent(Vin) + '&Model=' + encodeURIComponent(Model) + '&Year=' + encodeURIComponent(Year) + '&Make=' + encodeURIComponent(Make) + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate) + '&VersionId=' + encodeURIComponent(VersionId) + '&PolicyId=' + encodeURIComponent(PolicyId) + '&PolicyImageNum=' + encodeURIComponent(PolicyImageNum) + '&VehicleNum=' + encodeURIComponent(VehicleNum);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                    .done(function (data) {
                        callBack(data);
                    });
            }
            else {
                callBack(null);
            }
        };

        // Searches for vehicle information based on the VIN.        
        // Usage: GetFromVIN('63ge6d6g363g6g63','','','', '', function(data){}); - Searches by VIN
        // Results: An array of VinLookupResult()
        // Data Structure - Vin[string], PolicyId[string], PolicyImageNum[string], VehicleNum[string], VersionId[string]
        this.GetFromVIN = function (Vin, PolicyId, PolicyImageNum, VehicleNum, VersionId, callBack) {
            if (Vin.trim().length > 0) {
                var genHandler = 'GenHandlers/Vr_Comm/VinHandlerCAP.ashx?Vin=' + encodeURIComponent(Vin) + '&Model=' + '&Year=' + '&Make=' + '&EffectiveDate=' + encodeURIComponent(master_effectiveDate) + '&VersionId=' + encodeURIComponent(VersionId) + '&PolicyId=' + encodeURIComponent(PolicyId) + '&PolicyImageNum=' + encodeURIComponent(PolicyImageNum) + '&VehicleNum=' + encodeURIComponent(VehicleNum);
                $.getJSON(genHandler, {
                    dataType: "json",
                    data: "",
                    cache: false,
                    format: "json"
                })
                    .done(function (data) {
                        callBack(data);
                    });
            }
            else {
                callBack(null);
            }            
        }; // GetFromVIN END       
        
    }; //VIN END

    this.MineSubsidence = new function () {
        // Returns an array of Indiana county names that allow mine subsidence coverages.
        this.MineSubsidenceCapableCountyNames = null;

        this.GetMineSubsidenceCapableCountyNamesByStateAbbreviation = function (stateAbbreviation, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/GetMineSubsidenceByState.ashx';
            $.getJSON(genHandler, {
                cache: true,
                dataType: "json",
                data: "",
                format: "json",                
                stateAbbreviation: stateAbbreviation
            })
                .fail(function (jqXHR, textStatus, errorThrown) { alert('GetMineSubsidenceCapableCountyNamesByStateAbbreviation request failed! ' + textStatus); })
                .done(function (data) { callBack(data); });

        };

        this.GetMineSubsidenceCapableCountyNamesByStateId = function (stateId, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/GetMineSubsidenceByState.ashx';
            $.getJSON(genHandler, {
                cache: true,
                dataType: "json",
                data: "",
                format: "json",
                stateId: stateId
            })
                .fail(function (jqXHR, textStatus, errorThrown) { alert('GetMineSubsidenceCapableCountyNamesByStateId request failed! ' + textStatus); })
                .done(function (data) { callBack(data); });

        };


    }; // MineSubsidence END
        
    this.QuotingStats = new function () {
        // Gathers quote statistics for the given agency between the specified dates and for the specific lobs.
        // Usage: GetVrQuoteStats(101,'10/01/2015','10-30-2015','1,2,3,17',function(data){});
        // Result: Returns a array of TimeSliceDataSet().
        this.GetVrQuoteStats = function (agencyId, startDate, endDate, lobsIds,callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/QuoteStats.ashx';
            $.getJSON(genHandler, {
                cache: false,
                dataType: "json",
                data: "",
                format: "json",
                agencyId: agencyId,
                startDate: startDate,
                endDate: endDate,
                lobIds: lobsIds
            })
                .fail(function (jqXHR, textStatus, errorThrown) { alert('Stats request failed! ' + textStatus); })
                .done(function (data) { callBack(data); });

        };// GetVrQuoteStats END
    }; // QuotingStats END

    this.ZipCode = new function () {
        // Retrieves the city name, county name, state abbreviation for a provided 5 digit zipcode.
        // Usage: GetZipCodeInformation('90210',function(data){});
        // Result: An array of ZipLookupResult(). Often it only has one result but it can have multiple.
        // Data Structure: ZipCode[string],County[string],StateAbbrev[string],City[string]
        this.GetZipCodeInformation = function (zipcode,callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/CityCountyLookup.ashx?zipcode=' + encodeURIComponent(zipcode);
            $.getJSON(genHandler, {
                dataType: "json",
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) { callBack(data); });

        };//GetZipCodeInformation END

    }; // ZipCode END

    this.GlClassCodes = new function () {
        
        // Returns an array of quickquote.commonobject.quickquoteGlClassification.        
        this.searchCodes = function (searchType, searchText, lobId, programTypeId, callBack) {
            if (searchText.length < 2)
                searchText = 'zz';
            var genHandler = 'GenHandlers/Vr_Pers/GetCommClassCodes.ashx?query=yes&searchText=' + encodeURIComponent(searchText) + '&searchType=' + searchType + '&lobId=' + lobId + '&programTypeId=' + programTypeId + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                cache: false,
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) { callBack(data); });

        }; // END searchCodes

        // Returns an array of quickquote.commonobject.quickquoteGlClassification.        
        this.findSpecificCodeById = function (classCodeId, lobId, programTypeId, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/GetCommClassCodes.ashx?query=yes&classCodeId=' + classCodeId + '&lobId=' + lobId + '&programTypeId=' + programTypeId + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                cache: false,
                data: "",
                format: "json"
            })
            .done(function (data) { callBack(data); });

        }; // END search by ID

        // Returns an array of quickquote.commonobject.quickquoteGlClassification.        
        this.findSpecificCodeByClassCode = function (classCode, lobId, programTypeId, callBack) {
            var genHandler = 'GenHandlers/Vr_Pers/GetCommClassCodes.ashx?query=yes&classCode=' + classCode + '&lobId=' + lobId + '&programTypeId=' + programTypeId + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",
                cache: false,
                data: "",
                format: "json"
            })
                .done(function (data) { callBack(data); });

        }; // END search by Code

    };// END GlClassCodes

    this.BopBuildingClassifications = new function () {

        // Returns an array of keyVairPair(of string, string) (classdesc, {classcode,classdesc,{[dia_ClassificationType_id]}}).        
        this.searchClassifications = function (programType, callBack) {            
            var genHandler = 'GenHandlers/Vr_Pers/BopBuildingClassification.ashx?programType=' + encodeURIComponent(programType) + '&ppopo=' + Math.random().toString().replace('.', '');
            $.getJSON(genHandler, {
                dataType: "json",                
                data: "",
                cache: false,
                format: "json"
            })
            .done(function (data) {
                callBack(data);
            });

        }; // END searchCodes

    };// END BopBuildingClassifications

} // VRData END

