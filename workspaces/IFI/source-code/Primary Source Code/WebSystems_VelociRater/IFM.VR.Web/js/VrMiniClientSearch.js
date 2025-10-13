
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />


var eleI1ID_FirstName = null;
var eleI1ID_MiddleName = null;
var eleI1ID_LastName = null;
var eleI1ID_Suffix = null;
var eleI1ID_Sex = null;
var eleI1ID_SSN = null;
var eleI1ID_DOB = null;
var eleI1ID_Email = null;
var eleI1ID_esigEmail = null; //Added 1/18/2022 for bug 67521 MLW
var eleI1ID_Phone = null;
var eleI1ID_Phone_Type = null;
var eleI1ID_StreetNum = null;
var eleI1ID_StreetName = null;
var eleI1ID_AptNum = null;
var eleI1ID_CareOf = null;
var eleI1ID_OtherPrefix = null;
var eleI1ID_POBox = null;
var eleI1ID_City = null;
var eleI1ID_State = null;
var eleI1ID_Zip = null;
var eleI1ID_County = null;
var eleI1ID_Client = null;

var eleI2ID_FirstName = null;
var eleI2ID_MiddleName = null;
var eleI2ID_LastName = null;
var eleI2ID_Suffix = null;
var eleI2ID_Sex = null;
var eleI2ID_SSN = null;
var eleI2ID_DOB = null;
var eleI2ID_Email = null;
var eleI2ID_Phone = null;
var eleI2ID_Phone_Type = null;
var eleI2ID_StreetNum = null;
var eleI2ID_StreetName = null;
var eleI2ID_AptNum = null;
var eleI2ID_CareOf = null;
var eleI2ID_OtherPrefix = null;
var eleI2ID_POBox = null;
var eleI2ID_City = null;
var eleI2ID_State = null;
var eleI2ID_Zip = null;
var eleI2ID_County = null;
var eleI2ID_Client = null;

var eleI1ID_BusinessName = null;
var eleI1ID_DBA = null;
var eleI1ID_Entity = null;
var eleI1ID_FEIN = null;
var eleI1ID_CommSSN = null; //added 10/5/2017
var eleI1ID_CommTaxType = null; //added 10/6/2017
var eleI1ID_liCommFEIN = null; //added 10/6/2017
var eleI1ID_liCommSSN = null; //added 10/6/2017
var eleI1ID_Email_Type = null; //added 10/7/2017
var eleI2ID_Email_Type = null; //added 10/7/2017
var eleI1ID_PhoneExt = null; //added 10/7/2017
var eleI2ID_PhoneExt = null; //added 10/7/2017
var eleI1ID_CommDescOfOps = null; //added 10/6/2017
var eleI1ID_CommBusStartedDt = null; //added 10/6/2017
var eleI1ID_CommYearsExp = null; //added 10/6/2017
var eleI1ID_liCommYearsExp = null; //added 10/6/2017
var eleI1ID_txtOtherEntityDescription = null; //Added 2/22/2022 for bug 63511 MLW
var eleI1ID_liOtherEntityDescription = null; //Added 2/22/2022 for bug 63511 MLW

//added 8/28/2023 for CommDataPrefill
var commDataPrefill_BusinessName = null;
var commDataPrefill_DBA = null;
var commDataPrefill_Entity = null;
var commDataPrefill_FEIN = null;
var commDataPrefill_TaxType = null; //added 10/6/2017
var commDataPrefill_Email_Type = null; //added 10/7/2017
var commDataPrefill_PhoneExt = null; //added 10/7/2017
var commDataPrefill_CommDescOfOps = null; //added 10/6/2017
var commDataPrefill_CommBusStartedDt = null; //added 10/6/2017
var commDataPrefill_CommYearsExp = null; //added 10/6/2017
var commDataPrefill_txtOtherEntityDescription = null; //Added 2/22/2022 for bug 63511 MLW
var commDataPrefill_Email = null;
var commDataPrefill_Phone = null;
var commDataPrefill_Phone_Type = null;
var commDataPrefill_StreetNum = null;
var commDataPrefill_StreetName = null;
var commDataPrefill_AptNum = null;
var commDataPrefill_Other = null;
var commDataPrefill_POBox = null;
var commDataPrefill_City = null;
var commDataPrefill_State = null;
var commDataPrefill_Zip = null;
var commDataPrefill_County = null;
var commDataPrefill_Client = null;

var ClientSearch = new function () {
    var lastClientLookup = null;
    var DialogInitedMini = false;
    this.InitMini = function() {
        if (DialogInitedMini == false) {
            $("#divClientSearch").hide();
            DialogInitedMini = true;
            var popUpDiv = $("#divForPopups");
            $("#divClientSearch").appendTo(popUpDiv);
        }
    }

    function ShowClientSearch() {
        if ($("#" + enabledClientId).val() == "true") {
            ClientSearch.InitMini();
            $("#divClientSearch").show();
        }
        else { HideClientSearch(); }
    }

    this.HideClientSearch = function () { HideClientSearch(); };

    function HideClientSearch() {
        ClientSearch.InitMini();
        $("#divClientSearch").hide();
    }

    function ClearResults() {
        lastClientLookup = null;
        $("#divResults").empty(); //clear div
    }



    this.SelectionMade = function (ele) {
        var dataIndex = $(ele).attr("qqClientID");
        //alert("Selected clientid " + lastClientLookup[dataIndex].ClientId);
        if (lastClientLookup[dataIndex].Name2 != null && lastClientLookup[dataIndex].Name2.FirstName != "") {
            if (confirm('Apply both policyholders information?')) {
                // yes apply
                ApplyInsuredOne(dataIndex);
                ApplyInsuredTwo(dataIndex);
                $("#" + enabledClientId).val("false");
                HideClientSearch();
                CopyAddressFromPolicyHolder();
                Ph1UpdateHeaderText();
                Ph2UpdateHeaderText();
            }
        }
        else {
            if (confirm('Apply policyholder information?')) {
                // yes apply
                if (lastClientLookup[dataIndex].Name.TypeId != "2") {
                    ApplyInsuredOne(dataIndex);
                }
                else {
                    ApplyInsuredOneComm(dataIndex);
                }

                $("#" + enabledClientId).val("false");
                HideClientSearch();
                CopyAddressFromPolicyHolder();
                Ph1UpdateHeaderText();
            }
        }
    };

    //added 8/28/2023 for CommDataPrefill
    this.CommDataPrefillSelectionMade = function (ele) {
        var dataIndex = $(ele).attr("qqClientID");
        if (confirm('Apply policyholder information?')) {
            // yes apply
            ApplyInsuredOneCommDataPrefill(dataIndex);

            $("#" + enabledClientId).val("false");
            HideClientSearch();
        }
    };

    // Personal - debounce is in vrPersonal.js
    this.DoSearchWithEleNames = debounce(function (keycode, senderId, eleFirst, eleLast, eleCity, eleZip, eleSSN) {
        if (keycode != 9 && keycode != 17 && keycode != 37) { //do not invoke on Tab, Shift, ALT, or CTRL key presses
            MiniSearchClients(senderId, $("#" + eleFirst).val(), $("#" + eleLast).val(), $("#" + eleCity).val(), $("#" + eleZip).val(), $("#" + eleSSN).val());
        }
    }, 500);

    function MiniSearchClients(senderId, firstname, lastname, city, zip, ssn) {
        //mostRecentKeyId = senderId;
        ClearResults();
        HideClientSearch(); // always hide and reshow if returns are available

        //$("#" + mostRecentKeyId).focus();
        // if not enabled just get out of here
        if ($("#" + enabledClientId).val() == "false") {
            return;
        }

        //var focusedElement = senderId;//GetCurrentFocusedElementId();
        // you must select an agency - the search would be too slow otherwise
        if (agencyID != "-1") {
            if ((lastname.length > 2 || (lastname.length > 1 & firstname.length > 0)) | ssn.length == 11) {
                if (zip.length > 5) {
                    zip = zip.substring(0, 5);
                }

                if (ssn != null)
                    ssn = ssn.replace(/-/g, "")

                // agencyID will still be checked against session data to confirm that the current has access to the agency
                VRData.Client.GetPersonalClients(agencyID, firstname, lastname, city, zip, ssn, function (data) {
                    ClearResults(); // CH 10/10/2017 - Timing issue sometimes failed to clear; Added here to insure list is cleared with new data
                    lastClientLookup = data;
                    //alert('found results');
                    if (data.length > 0) {
                        ShowClientSearch();
                    }

                    for (var ii = 0; ii < data.length; ii++) {
                        if (data[ii].Name.TaxNumber == "000-00-0000" | data[ii].Name.TaxNumber == "0000-00-0000" | data[ii].Name.TaxNumber == "0000000000" | data[ii].Name.TaxNumber == "00-0000000" | data[ii].Name.TaxNumber == "000000000") {
                            data[ii].Name.TaxNumber = ""
                        }
                        if (data[ii].Name2.DisplayName != "") {
                            if (data[ii].Name2.TaxNumber == "000-00-0000" | data[ii].Name2.TaxNumber == "0000-00-0000" | data[ii].Name2.TaxNumber == "0000000000" | data[ii].Name2.TaxNumber == "00-0000000" | data[ii].Name2.TaxNumber == "000000000") {
                                data[ii].Name2.TaxNumber = ""
                            }
                        }

                        var html = '';
                        html += '<div qqClientID="' + ii + '" onclick="ClientSearch.SelectionMade($(this));" style="" class="ui-corner-all">';
                        html += '<table style="width: 100%; margin: 8px;">';

                        html += '<tr>';
                        html += '<td style="width: 50%">';
                        html += '<b>Name:</b> ' + data[ii].Name.DisplayName;
                        html += "<br/>";
                        html += "DOB: " + data[ii].Name.BirthDate;
                        html += "<br/>";
                        html += "SSN: " + data[ii].Name.TaxNumber;
                        html += '</td>';
                        html += '<td style="width: 50%">';
                        if (data[ii].Name2.DisplayName != "") {
                            html += '<b>Name:</b> ' + data[ii].Name2.DisplayName;
                            html += "<br/>";
                            html += "DOB: " + data[ii].Name2.BirthDate;
                            html += "<br/>";
                            html += "SSN: " + data[ii].Name2.TaxNumber;
                        }
                        html += '</td>';
                        html += '</tr>';

                        if (data[ii].Address.Other != "") {
                            html += '<tr>';
                            html += '<td colspan="2">';
                            html += '<b>Other:</b> ' + data[ii].Address.Other;
                            html += '</td>';
                            html += '</tr>';
                        }

                        html += '<tr>';
                        html += '<td colspan="2">';
                        if (data[ii].Address.POBox == "") {
                            if (data[ii].Address.ApartmentNumber == "") {
                                //Non apartment
                                html += data[ii].Address.HouseNum + " " + data[ii].Address.StreetName;
                                html += "<br />";
                                html += data[ii].Address.City + ", " + data[ii].Address.State + " " + data[ii].Address.Zip
                            }
                            else {
                                // apartment
                                html += data[ii].Address.ApartmentNumber + " " + data[ii].Address.StreetName;
                                html += "<br />";
                                html += data[ii].Address.City + ", " + data[ii].Address.State + " " + data[ii].Address.Zip
                            }
                        }
                        else {
                            // poBox
                            html += "POBox " + data[ii].Address.POBox + " " + data[ii].Address.StreetName;
                            html += "<br />";
                            html += data[ii].Address.City + ", " + data[ii].Address.State + " " + data[ii].Address.Zip
                        }
                        html += '</td>';
                        html += '</tr>';

                        //if (data[ii].PrimaryPhone != "" && data[ii].PrimaryEmail != "") {
                        //    html += '<tr>';
                        //    html += '<td colspan="2">';
                        //    if (data[ii].PrimaryPhone != "")
                        //        html += '<b>Phone:</b> ' + data[ii].PrimaryPhone + "<br />";

                        //    if (data[ii].PrimaryEmail != "")
                        //        html += "Email: " + data[ii].PrimaryEmail;
                        //    html += '</td>';
                        //    html += '</tr>';
                        //}
                        //updated 10/7/2017
                        if (data[ii].FirstPhoneNumberWithExtension != "" || data[ii].FirstEmailAddress != "") {
                            html += '<tr>';
                            html += '<td colspan="2">';
                            var hasPhone = false;
                            if (data[ii].FirstPhoneNumberWithExtension != "") {
                                html += '<b>Phone:</b> ' + data[ii].FirstPhoneNumberWithExtension;
                                hasPhone = true;
                            }
                            if (data[ii].FirstEmailAddress != "") {
                                if (hasPhone == true) {
                                    html += '<br />';
                                }
                                html += "<b>Email:</b> " + data[ii].FirstEmailAddress;
                            }
                            html += '</td>';
                            html += '</tr>';
                        }

                        html += '</table>';
                        html += '</div>';
                        $("#divResults").append(html);
                    }
                });

            }
            else {
                ClearResults();
                //$("#" + mostRecentKeyId).focus();
            }
        }
        else {
            ClearResults();
            //$("#" + mostRecentKeyId).focus();
        }
    }

    //Updated 1/18/2022 for bug 67521 MLW - added eSigEmail
    //this.SetBindings = function (isInsuredOne, firstname, middlename, lastname, suffix, sex, ssn, dob, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType) {
    //updated 10/7/2017 for eleI1ID_Email_Type and eleI1ID_PhoneExt
    //this.SetBindings = function (isInsuredOne, firstname, middlename, lastname, suffix, sex, ssn, dob, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType, emailType, phoneExt) {
    this.SetBindings = function (isInsuredOne, firstname, middlename, lastname, suffix, sex, ssn, dob, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType, emailType, phoneExt, eSigEmail, CareOf, OtherPrefix) {
        if (isInsuredOne) {
            eleI1ID_FirstName = firstname;
            eleI1ID_MiddleName = middlename;
            eleI1ID_LastName = lastname;
            eleI1ID_Suffix = suffix;
            eleI1ID_Sex = sex;
            eleI1ID_SSN = ssn;
            eleI1ID_DOB = dob;
            eleI1ID_Email = email;
            eleI1ID_Phone = phone;
            eleI1ID_Phone_Type = phoneType;
            eleI1ID_StreetNum = streetNum;
            eleI1ID_StreetName = streetName;
            eleI1ID_AptNum = apt;
            eleI1ID_CareOf = CareOf;
            eleI1ID_OtherPrefix = OtherPrefix;
            eleI1ID_POBox = pobox;
            eleI1ID_City = city;
            eleI1ID_State = state;
            eleI1ID_Zip = zip;
            eleI1ID_County = county;
            eleI1ID_Client = client;
            eleI1ID_Email_Type = emailType; //added 10/7/2017
            eleI1ID_PhoneExt = phoneExt; //added 10/7/2017
            eleI1ID_esigEmail = eSigEmail; //Added 1/18/2022 for bug 67521 MLW
        }
        else {
            eleI2ID_FirstName = firstname;
            eleI2ID_MiddleName = middlename;
            eleI2ID_LastName = lastname;
            eleI2ID_Suffix = suffix;
            eleI2ID_Sex = sex;
            eleI2ID_SSN = ssn;
            eleI2ID_DOB = dob;
            eleI2ID_Email = email;
            eleI2ID_Phone = phone;
            eleI2ID_Phone_Type = phoneType;
            eleI2ID_StreetNum = streetNum;
            eleI2ID_StreetName = streetName;
            eleI2ID_AptNum = apt;
            eleI2ID_CareOf = CareOf;
            eleI1ID_OtherPrefix = OtherPrefix;
            eleI2ID_POBox = pobox;
            eleI2ID_City = city;
            eleI2ID_State = state;
            eleI2ID_Zip = zip;
            eleI2ID_County = county;
            eleI2ID_Client = client;
            eleI2ID_Email_Type = emailType; //added 10/7/2017
            eleI2ID_PhoneExt = phoneExt; //added 10/7/2017
        }
    };
    
    function ApplyInsuredOne(index) {
        if (lastClientLookup[index].Name.FirstName != "")
            $("#" + eleI1ID_FirstName).val(lastClientLookup[index].Name.FirstName);
        if (lastClientLookup[index].Name.MiddleName != "")
            $("#" + eleI1ID_MiddleName).val(lastClientLookup[index].Name.MiddleName);
        if (lastClientLookup[index].Name.LastName != "")
            $("#" + eleI1ID_LastName).val(lastClientLookup[index].Name.LastName);

        if (lastClientLookup[index].Name.SuffixName != "")
            $("#" + eleI1ID_Suffix).val(lastClientLookup[index].Name.SuffixName).attr("selected", "selected");;

        if (lastClientLookup[index].Name.TaxNumber != "")
            $("#" + eleI1ID_SSN).val(lastClientLookup[index].Name.TaxNumber);
        if (lastClientLookup[index].Name.BirthDate != "")
            $("#" + eleI1ID_DOB).val(lastClientLookup[index].Name.BirthDate);

        if (lastClientLookup[index].Name.SexId != "")
            $("#" + eleI1ID_Sex).val(lastClientLookup[index].Name.SexId).attr("selected", "selected");

        //if (lastClientLookup[index].PrimaryEmail != "")
        //    $("#" + eleI1ID_Email).val(lastClientLookup[index].PrimaryEmail);
        //if (lastClientLookup[index].PrimaryPhone != "")
        //    $("#" + eleI1ID_Phone).val(lastClientLookup[index].PrimaryPhone);

        //if (lastClientLookup[index].Phones != null && lastClientLookup[index].Phones.length > 0 && lastClientLookup[index].Phones[0].PhoneId != "")
        //    $("#" + eleI1ID_Phone_Type).val(lastClientLookup[index].Phones[0].PhoneId).attr("selected", "selected");
        //updated 10/7/2017
        if (lastClientLookup[index].FirstEmailAddress != "")
            $("#" + eleI1ID_Email).val(lastClientLookup[index].FirstEmailAddress); 
            //Updated 1/18/2022 for bug 67521 MLW
            var txtESigEmailId_Lookup = document.getElementById(eleI1ID_esigEmail);
            if (txtESigEmailId_Lookup) {
                $("#" + eleI1ID_esigEmail).val(lastClientLookup[index].FirstEmailAddress);  
            }
            //$("#" + esigEmail).val(lastClientLookup[index].FirstEmailAddress); //Added 12/23/2019 for eSignature Project task 41686 MLW
        if (lastClientLookup[index].FirstEmailTypeId != "")
            $("#" + eleI1ID_Email_Type).val(lastClientLookup[index].FirstEmailTypeId);
        if (lastClientLookup[index].FirstPhoneNumber != "")
            $("#" + eleI1ID_Phone).val(lastClientLookup[index].FirstPhoneNumber);
        if (lastClientLookup[index].FirstPhoneExt != "" && lastClientLookup[index].FirstPhoneExt != "0")
            $("#" + eleI1ID_PhoneExt).val(lastClientLookup[index].FirstPhoneExt);
        if (lastClientLookup[index].FirstPhoneTypeId != "" && lastClientLookup[index].FirstPhoneTypeId != "0")
            $("#" + eleI1ID_Phone_Type).val(lastClientLookup[index].FirstPhoneTypeId).attr("selected", "selected");

        if (lastClientLookup[index].Address.HouseNum != "")
            $("#" + eleI1ID_StreetNum).val(lastClientLookup[index].Address.HouseNum);
        if (lastClientLookup[index].Address.StreetName != "")
            $("#" + eleI1ID_StreetName).val(lastClientLookup[index].Address.StreetName);
        if (lastClientLookup[index].Address.ApartmentNumber != "")
            $("#" + eleI1ID_AptNum).val(lastClientLookup[index].Address.ApartmentNumber);
        if (lastClientLookup[index].Address.POBox != "")
            $("#" + eleI1ID_POBox).val(lastClientLookup[index].Address.POBox);
        if (lastClientLookup[index].Address.City != "")
            $("#" + eleI1ID_City).val(lastClientLookup[index].Address.City);

        if (lastClientLookup[index].Address.StateId != "")
            $("#" + eleI1ID_State).val(lastClientLookup[index].Address.StateId).attr("selected", "selected");

        if (lastClientLookup[index].Address.Zip != "") {
            if (lastClientLookup[index].Address.Zip.length > 5) {
                $("#" + eleI1ID_Zip).val(lastClientLookup[index].Address.Zip);
                $("#" + eleI1ID_Zip).click(); // in case the county is empty do a zip code lookup
            }
            else {
                $("#" + eleI1ID_Zip).val(lastClientLookup[index].Address.Zip);
                $("#" + eleI1ID_Zip).click(); // in case the county is empty do a zip code lookup
            }
        }

        if (lastClientLookup[index].Address.County != "")
            $("#" + eleI1ID_County).val(lastClientLookup[index].Address.County);

        //if (lastClientLookup[index].ClientId != "")
        //    $("#" + eleI1ID_Client).val(lastClientLookup[index].ClientId);
        //updated 5/3/2019 to not overwrite existing clientId
        if (lastClientLookup[index].ClientId != "") {
            var hasClientId = false;
            var txtClientId_Lookup = document.getElementById(eleI1ID_Client);
            if (txtClientId_Lookup) {
                if (txtClientId_Lookup.value.trim().length > 0) {
                    hasClientId = true;
                }
            }
            if (hasClientId == false) {
                $("#" + eleI1ID_Client).val(lastClientLookup[index].ClientId);
            }
        }

        //if (lastClientLookup[index].Address.Other != "")
        //    $("#" + eleI1ID_CareOf).val(lastClientLookup[index].Address.Other);

        if (lastClientLookup[index].Address.Other != "")
            $("#" + eleI1ID_CareOf).val(ProcessAddressOther(lastClientLookup[index].Address.Other));
    }

    function ApplyInsuredTwo(index) {
        if (lastClientLookup[index].Name2.FirstName != "")
            $("#" + eleI2ID_FirstName).val(lastClientLookup[index].Name2.FirstName);
        if (lastClientLookup[index].Name2.MiddleName != "")
            $("#" + eleI2ID_MiddleName).val(lastClientLookup[index].Name2.MiddleName);
        if (lastClientLookup[index].Name2.LastName != "")
            $("#" + eleI2ID_LastName).val(lastClientLookup[index].Name2.LastName);

        if (lastClientLookup[index].Name2.SuffixName != "")
            $("#" + eleI2ID_Suffix).val(lastClientLookup[index].Name.SuffixName).attr("selected", "selected");;

        if (lastClientLookup[index].Name2.TaxNumber != "")
            $("#" + eleI2ID_SSN).val(lastClientLookup[index].Name2.TaxNumber);
        if (lastClientLookup[index].Name2.BirthDate != "")
            $("#" + eleI2ID_DOB).val(lastClientLookup[index].Name2.BirthDate);

        if (lastClientLookup[index].Name2.SexId != "")
            $("#" + eleI2ID_Sex).val(lastClientLookup[index].Name2.SexId).attr("selected", "selected");

        //if (lastClientLookup[index].PrimaryEmail != "")
        //    $("#" + eleI2ID_Email).val(lastClientLookup[index].PrimaryEmail);

        //if (lastClientLookup[index].PrimaryPhone != "")
        //    $("#" + eleI2ID_Phone).val(lastClientLookup[index].PrimaryPhone);
        //if (lastClientLookup[index].Phones != null && lastClientLookup[index].Phones.length > 0 && lastClientLookup[index].Phones[0].PhoneId != "")
        //    $("#" + eleI2ID_Phone_Type).val(lastClientLookup[index].Phones[0].PhoneId).attr("selected", "selected");
        //updated 10/7/2017
        if (lastClientLookup[index].FirstEmailAddress != "")
            $("#" + eleI2ID_Email).val(lastClientLookup[index].FirstEmailAddress);
        if (lastClientLookup[index].FirstEmailTypeId != "")
            $("#" + eleI2ID_Email_Type).val(lastClientLookup[index].FirstEmailTypeId);
        if (lastClientLookup[index].FirstPhoneNumber != "")
            $("#" + eleI2ID_Phone).val(lastClientLookup[index].FirstPhoneNumber);
        if (lastClientLookup[index].FirstPhoneExt != "" && lastClientLookup[index].FirstPhoneExt != "0")
            $("#" + eleI2ID_PhoneExt).val(lastClientLookup[index].FirstPhoneExt);
        if (lastClientLookup[index].FirstPhoneTypeId != "" && lastClientLookup[index].FirstPhoneTypeId != "0")
            $("#" + eleI2ID_Phone_Type).val(lastClientLookup[index].FirstPhoneTypeId).attr("selected", "selected");


        if (lastClientLookup[index].Address.HouseNum != "")
            $("#" + eleI2ID_StreetNum).val(lastClientLookup[index].Address.HouseNum);
        if (lastClientLookup[index].Address.StreetName != "")
            $("#" + eleI2ID_StreetName).val(lastClientLookup[index].Address.StreetName);
        if (lastClientLookup[index].Address.ApartmentNumber != "")
            $("#" + eleI2ID_AptNum).val(lastClientLookup[index].Address.ApartmentNumber);
        if (lastClientLookup[index].Address.POBox != "")
            $("#" + eleI2ID_POBox).val(lastClientLookup[index].Address.POBox);
        if (lastClientLookup[index].Address.City != "")
            $("#" + eleI2ID_City).val(lastClientLookup[index].Address.City);

        if (lastClientLookup[index].Address.Zip != "") {
            if (lastClientLookup[index].Address.Zip.length > 5)
                $("#" + eleI1ID_Zip).val(lastClientLookup[index].Address.Zip);
            else
                $("#" + eleI1ID_Zip).val(lastClientLookup[index].Address.Zip);
        }

        if (lastClientLookup[index].Address.County != "")
            $("#" + eleI2ID_County).val(lastClientLookup[index].Address.County);

        if (lastClientLookup[index].Address.StateId != "")
            $("#" + eleI1ID_State).val(lastClientLookup[index].Address.StateId).attr("selected", "selected");

        if (lastClientLookup[index].Address.Other != "")
            $("#" + eleI2ID_CareOf).val(lastClientLookup[index].Address.Other);

        //$("#" + eleI2ID_State).val(lastClientLookup[index].Address.State);
    }


    // Commercial - debounce is in vrPersonal.js
    this.DoSearchWithEleNamesComm = debounce(function (keycode, senderId, businessName, eleCity, eleZip, eleSSN, commDataPrefill) {
        if (keycode != 9 && keycode != 17 && keycode != 37) { //do not invoke on Tab, Shift, ALT, or CTRL key presses
            if (typeof (commDataPrefill) == "undefined") {
                commDataPrefill = false;
            }
            MiniSearchClientsComm(senderId, $("#" + businessName).val(), $("#" + eleCity).val(), $("#" + eleZip).val(), $("#" + eleSSN).val(), commDataPrefill);
        }
    }, 500);

    //Updated 2/22/2022 for bug 63511 MLW - added txtOtherEntityDescription and liOtherEntityDescription
    //Updated 1/18/2022 for bug 67521 MLW - added eSigEmail
    ////this.SetBindingsComm = function (businessName, DBA, Entity, FEIN, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType) {
    ////updated 10/5/2017 for eleI1ID_CommSSN and eleI1ID_CommTaxType; eleI1ID_liCommFEIN, eleI1ID_liCommSSN 10/6/2017; eleI1ID_Email_Type, eleI1ID_PhoneExt, eleI1ID_CommDescOfOps, eleI1ID_CommBusStartedDt, , eleI1ID_CommYearsExp, and eleI1ID_liCommYearsExp 10/7/2017
    //this.SetBindingsComm = function (businessName, DBA, Entity, FEIN, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType, commSSN, commTaxType, liCommFEIN, liCommSSN, emailType, phoneExt, descOfOps, busStartedDt, yrsExp, liYrsExp) {
    this.SetBindingsComm = function (businessName, DBA, Entity, FEIN, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType, commSSN, commTaxType, liCommFEIN, liCommSSN, emailType, phoneExt, descOfOps, busStartedDt, yrsExp, liYrsExp, eSigEmail, txtOtherEntityDescription, liOtherEntityDescription, CareOf, OtherPrefix) {
        eleI1ID_BusinessName = businessName;
        eleI1ID_DBA = DBA;
        eleI1ID_Entity = Entity;
        eleI1ID_FEIN = FEIN;

        eleI1ID_Email = email;
        eleI1ID_Phone = phone;
        eleI1ID_Phone_Type = phoneType;
        eleI1ID_StreetNum = streetNum;
        eleI1ID_StreetName = streetName;
        eleI1ID_AptNum = apt;
        eleI1ID_CareOf = CareOf;
        eleI1ID_OtherPrefix = OtherPrefix;
        eleI1ID_POBox = pobox;
        eleI1ID_City = city;
        eleI1ID_State = state;
        eleI1ID_Zip = zip;
        eleI1ID_County = county;
        eleI1ID_Client = client;
        eleI1ID_CommSSN = commSSN; //added 10/5/2017
        eleI1ID_CommTaxType = commTaxType //added 10/6/2017
        eleI1ID_liCommFEIN = liCommFEIN //added 10/6/2017
        eleI1ID_liCommSSN = liCommSSN //added 10/6/2017
        eleI1ID_Email_Type = emailType //added 10/7/2017
        eleI1ID_PhoneExt = phoneExt; //added 10/7/2017
        eleI1ID_CommDescOfOps = descOfOps; //added 10/6/2017
        eleI1ID_CommBusStartedDt = busStartedDt; //added 10/6/2017
        eleI1ID_CommYearsExp = yrsExp; //added 10/6/2017
        eleI1ID_liCommYearsExp = liYrsExp; //added 10/6/2017
        eleI1ID_esigEmail = eSigEmail; //Added 1/18/2022 for bug 67521 MLW
        eleI1ID_txtOtherEntityDescription = txtOtherEntityDescription; //Added 2/22/22 for bug 63511 MLW
        eleI1ID_liOtherEntityDescription = liOtherEntityDescription; //Added 2/22/22 for bug 63511 MLW
    };

    //added 8/28/2023 for CommDataPrefill
    this.SetBindingsCommDataPrefill = function (businessName, DBA, Entity, FEIN, email, phone, streetNum, streetName, apt, pobox, city, state, zip, county, client, phoneType, commTaxType, emailType, phoneExt, descOfOps, busStartedDt, yrsExp, txtOtherEntityDescription, other) {
        commDataPrefill_BusinessName = businessName;
        commDataPrefill_DBA = DBA;
        commDataPrefill_Entity = Entity;
        commDataPrefill_FEIN = FEIN;

        commDataPrefill_Email = email;
        commDataPrefill_Phone = phone;
        commDataPrefill_Phone_Type = phoneType;
        commDataPrefill_StreetNum = streetNum;
        commDataPrefill_StreetName = streetName;
        commDataPrefill_AptNum = apt;
        commDataPrefill_Other = other;
        commDataPrefill_POBox = pobox;
        commDataPrefill_City = city;
        commDataPrefill_State = state;
        commDataPrefill_Zip = zip;
        commDataPrefill_County = county;
        commDataPrefill_Client = client;
        commDataPrefill_CommTaxType = commTaxType //added 10/6/2017
        commDataPrefill_Email_Type = emailType //added 10/7/2017
        commDataPrefill_PhoneExt = phoneExt; //added 10/7/2017
        commDataPrefill_CommDescOfOps = descOfOps; //added 10/6/2017
        commDataPrefill_CommBusStartedDt = busStartedDt; //added 10/6/2017
        commDataPrefill_CommYearsExp = yrsExp; //added 10/6/2017
        commDataPrefill_txtOtherEntityDescription = txtOtherEntityDescription; //Added 2/22/22 for bug 63511 MLW
    };

    function MiniSearchClientsComm(senderId, businessName, city, zip, ssn, commDataPrefill) {
        //mostRecentKeyId = senderId;
        ClearResults();
        HideClientSearch(); // always hide and reshow if returns are available

        if (typeof (commDataPrefill) == "undefined") {
            commDataPrefill = false;
        }

        //$("#" + mostRecentKeyId).focus();
        // if not enabled just get out of here
        if ($("#" + enabledClientId).val() == "false") {
            return;
        }

        //var focusedElement = senderId;//GetCurrentFocusedElementId();
        // you must select an agency - the search would be too slow otherwise
        if (agencyID != "-1") {
            if (businessName.length > 4 | ssn.length == 11) {
                if (zip.length > 5) {
                    zip = zip.substring(0, 5);
                }

                if (ssn != null)
                    ssn = ssn.replace(/-/g, "")

                // agencyID will still be checked against session data to confirm that the current has access to the agency
                VRData.Client.GetCommercialClients(agencyID, businessName, city, zip, ssn, function (data) {
                    ClearResults();  // CH 10/10/2017 - Timing issue? sometimes failed to clear; Added here to insure list is cleared with new data
                    lastClientLookup = data;

                    if (data.length > 0) {
                        ShowClientSearch();
                    }

                    for (var ii = 0; ii < data.length; ii++) {
                        if (data[ii].Name.TaxNumber == "000-00-0000" | data[ii].Name.TaxNumber == "0000-00-0000" | data[ii].Name.TaxNumber == "0000000000" | data[ii].Name.TaxNumber == "00-0000000" | data[ii].Name.TaxNumber == "000000000") {
                            data[ii].Name.TaxNumber = ""
                        }
                        if (data[ii].Name2.DisplayName != "") {
                            if (data[ii].Name2.TaxNumber == "000-00-0000" | data[ii].Name2.TaxNumber == "0000-00-0000" | data[ii].Name2.TaxNumber == "0000000000" | data[ii].Name2.TaxNumber == "00-0000000" | data[ii].Name2.TaxNumber == "000000000") {
                                data[ii].Name2.TaxNumber = ""
                            }
                        }

                        var html = '';
                        if (commDataPrefill == true) {
                            html += '<div qqClientID="' + ii + '" onclick="ClientSearch.CommDataPrefillSelectionMade($(this));" style="" class="ui-corner-all">';
                        } else {
                            html += '<div qqClientID="' + ii + '" onclick="ClientSearch.SelectionMade($(this));" style="" class="ui-corner-all">';
                        }                        
                        html += '<table style="width: 100%; margin: 8px;">';

                        html += '<tr>';
                        html += '<td colspan="2">';
                        html += '<b>Name:</b> ' + data[ii].Name.DisplayName;
                        html += "<br/>";
                        //html += "FEIN: " + data[ii].Name.TaxNumber;
                        //updated 10/6/2017
                        if (data[ii].Name.TaxTypeId != "" && data[ii].Name.TaxTypeId == "1") {
                            html += "SSN: " + data[ii].Name.TaxNumber;
                        } else {
                            html += "FEIN: " + data[ii].Name.TaxNumber;
                        }
                        html += '</td>';

                        html += '</tr>';

                        if (data[ii].Address.Other != "") {
                            html += '<tr>';
                            html += '<td colspan="2">';
                            html += '<b>Other:</b> ' + data[ii].Address.Other;
                            html += '</td>';
                            html += '</tr>';
                        }

                        html += '<tr>';
                        html += '<td colspan="2">';
                        if (data[ii].Address.POBox == "") {
                            if (data[ii].Address.ApartmentNumber == "") {
                                //Non apartment
                                html += data[ii].Address.HouseNum + " " + data[ii].Address.StreetName;
                                html += "<br />";
                                html += data[ii].Address.City + ", " + data[ii].Address.State + " " + data[ii].Address.Zip
                            }
                            else {
                                // apartment
                                html += data[ii].Address.ApartmentNumber + " " + data[ii].Address.StreetName;
                                html += "<br />";
                                html += data[ii].Address.City + ", " + data[ii].Address.State + " " + data[ii].Address.Zip
                            }
                        }
                        else {
                            // poBox
                            html += "POBox " + data[ii].Address.POBox + " " + data[ii].Address.StreetName;
                            html += "<br />";
                            html += data[ii].Address.City + ", " + data[ii].Address.State + " " + data[ii].Address.Zip
                        }
                        html += '</td>';
                        html += '</tr>';

                        //if (data[ii].PrimaryPhone != "" && data[ii].PrimaryEmail != "") {
                        //    html += '<tr>';
                        //    html += '<td colspan="2">';
                        //    if (data[ii].PrimaryPhone != "")
                        //        html += '<b>Phone:</b> ' + data[ii].PrimaryPhone + "<br />";

                        //    if (data[ii].PrimaryEmail != "")
                        //        html += "Email: " + data[ii].PrimaryEmail;
                        //    html += '</td>';
                        //    html += '</tr>';
                        //}
                        //updated 10/7/2017
                        if (data[ii].FirstPhoneNumberWithExtension != "" || data[ii].FirstEmailAddress != "") {
                            html += '<tr>';
                            html += '<td colspan="2">';
                            var hasPhone = false;
                            if (data[ii].FirstPhoneNumberWithExtension != "") {
                                html += '<b>Phone:</b> ' + data[ii].FirstPhoneNumberWithExtension;
                                hasPhone = true;
                            }
                            if (data[ii].FirstEmailAddress != "") {
                                if (hasPhone == true) {
                                    html += '<br />';
                                }
                                html += "<b>Email:</b> " + data[ii].FirstEmailAddress;
                            }                                
                            html += '</td>';
                            html += '</tr>';
                        }

                        html += '</table>';
                        html += '</div>';
                        $("#divResults").append(html);
                    }

                });
            }
            else {
                ClearResults();
            }
        }
        else {
            ClearResults();
        }
    }

    function ApplyInsuredOneComm(index) {
        if (lastClientLookup[index].Name.CommercialName1 != "")
            $("#" + eleI1ID_BusinessName).val(lastClientLookup[index].Name.CommercialName1);

        //if (lastClientLookup[index].Name.CommercialDBAname != "")
        //    $("#" + eleI1ID_DBA).val(lastClientLookup[index].Name.CommercialDBAname);
        //updated 10/6/2017
        if (lastClientLookup[index].Name.DoingBusinessAsName != "")
            $("#" + eleI1ID_DBA).val(lastClientLookup[index].Name.DoingBusinessAsName);

        if (lastClientLookup[index].Name.EntityTypeId != "") {
            $("#" + eleI1ID_Entity).val(lastClientLookup[index].Name.EntityTypeId).attr("selected", "selected");
            //Added 2/22/2022 for bug 63511 MLW
            //Only visible for BOP, CAP, CPP, CPR, CGL and WCP
            var txtOtherEntityId_Lookup = document.getElementById(eleI1ID_txtOtherEntityDescription);
            var liOtherEntityId_Lookup = document.getElementById(eleI1ID_liOtherEntityDescription);
            if (txtOtherEntityId_Lookup && liOtherEntityId_Lookup) {
                if (lastClientLookup[index].Name.EntityTypeId == "5") {
                    $("#" + eleI1ID_txtOtherEntityDescription).val(lastClientLookup[index].Name.OtherLegalEntityDescription);
                    $("#" + eleI1ID_liOtherEntityDescription).show();
                } else {
                    $("#" + eleI1ID_txtOtherEntityDescription).val();
                    $("#" + eleI1ID_liOtherEntityDescription).hide();
                }
            }
        }
            

        //if (lastClientLookup[index].Name.TaxNumber != "")
        //    //$("#" + eleI1ID_FEIN).val(lastClientLookup[index].Name.TaxNumber);
        //    //updated 10/4/2017
        //    $("#" + eleI1ID_FEIN).val(lastClientLookup[index].Name.TaxNumber.replace(new RegExp('-', 'g'), ''));
        //rewrote 10/5/2017 - 10/6/2017
        var taxTypeSelected = false;
        var isSSN = false;
        var numHyphens;
        if (lastClientLookup[index].Name.TaxTypeId != "") {
            if (lastClientLookup[index].Name.TaxTypeId == "1") {
                //SSN
                isSSN = true;
                numHyphens = 2;
                taxTypeSelected = true;
            } else if (lastClientLookup[index].Name.TaxTypeId == "2") {
                //FEIN
                numHyphens = 1;
                taxTypeSelected = true;
            }
        }
        if (lastClientLookup[index].Name.TaxNumber != "") {
            var taxNumNoHyphens = lastClientLookup[index].Name.TaxNumber.replace(new RegExp('-', 'g'), '').trim();
            if (taxNumNoHyphens.length > 9) {
                taxNumNoHyphens = taxNumNoHyphens.substring(0, 9);
            }
            var taxNumToUse;
            //var isSSN = false;
            //var numHyphens = (lastClientLookup[index].Name.TaxNumber.match(/-/g) || []).length;
            if (taxTypeSelected == false) {
                numHyphens = (lastClientLookup[index].Name.TaxNumber.match(/-/g) || []).length;
                taxTypeSelected = true; //will be determined below
            }
            //alert('originalTaxNum: ' + lastClientLookup[index].Name.TaxNumber + '; taxNumNoHyphens: ' + taxNumNoHyphens);
            if (taxNumNoHyphens.length == 9){
                if (numHyphens == 1) {
                    //FEIN
                    taxNumToUse = taxNumNoHyphens.substr(0, 2) + '-' + taxNumNoHyphens.substr(2, 7); //substring only works the same when starting on 0 index (substring gets the text between 2 indices, excluding the char at the last index); this would actually be taxNumNoHyphens.substring(0, 2) + '-' + taxNumNoHyphens.substring(2, 9)
                } else if (numHyphens == 2){
                    //SSN
                    taxNumToUse = taxNumNoHyphens.substr(0, 3) + '-' + taxNumNoHyphens.substr(3, 2) + '-' + taxNumNoHyphens.substr(5, 4); //substring only works the same when starting on 0 index (substring gets the text between 2 indices, excluding the char at the last index); this would actually be taxNumNoHyphens.substr(0, 3) + '-' + taxNumNoHyphens.substr(3, 5) + '-' + taxNumNoHyphens.substr(5, 9)
                    isSSN = true;
                }else{
                    //use FEIN; just use taxNum w/o hyphens
                    taxNumToUse = taxNumNoHyphens;
                }
            }else{
                //use FEIN; just use taxNum w/o hyphens
                taxNumToUse = taxNumNoHyphens;
            }
            //alert('taxNumToUse: ' + taxNumToUse);
            //var liCommSSN = document.getElementById(eleI1ID_liCommSSN);
            //var liCommFEIN = document.getElementById(eleI1ID_liCommFEIN);
            if (isSSN == true) {
                //SSN
                $("#" + eleI1ID_CommSSN).val(taxNumToUse);
                //$("#" + eleI1ID_CommTaxType).val("1").attr("selected", "selected");
                //liCommSSN.style.display = '';
                //liCommFEIN.style.display = 'none';
            } else {
                //FEIN
                $("#" + eleI1ID_FEIN).val(taxNumToUse);
                //$("#" + eleI1ID_CommTaxType).val("2").attr("selected", "selected");
                //liCommFEIN.style.display = '';
                //liCommSSN.style.display = 'none';                
            }
        }
        if (taxTypeSelected == true) {
            var liCommSSN = document.getElementById(eleI1ID_liCommSSN);
            var liCommFEIN = document.getElementById(eleI1ID_liCommFEIN);
            if (isSSN == true) {
                //SSN
                $("#" + eleI1ID_CommTaxType).val("1").attr("selected", "selected");
                liCommSSN.style.display = '';
                liCommFEIN.style.display = 'none';
            } else {
                //FEIN
                $("#" + eleI1ID_CommTaxType).val("2").attr("selected", "selected");
                liCommFEIN.style.display = '';
                liCommSSN.style.display = 'none';
            }
        }

        //if (lastClientLookup[index].PrimaryEmail != "")
        //    $("#" + eleI1ID_Email).val(lastClientLookup[index].PrimaryEmail);
        //if (lastClientLookup[index].PrimaryPhone != "")
        //    $("#" + eleI1ID_Phone).val(lastClientLookup[index].PrimaryPhone);
        //if (lastClientLookup[index].Phones != null && lastClientLookup[index].Phones.length > 0 && lastClientLookup[index].Phones[0].PhoneId != "")
        //    $("#" + eleI1ID_Phone_Type).val(lastClientLookup[index].Phones[0].PhoneId).attr("selected", "selected");
        //updated 10/7/2017
        if (lastClientLookup[index].FirstEmailAddress != "")
            $("#" + eleI1ID_Email).val(lastClientLookup[index].FirstEmailAddress);
            //Updated 1/18/2022 for bug 67521 MLW
            var txtESigEmailId_Lookup = document.getElementById(eleI1ID_esigEmail);
            if (txtESigEmailId_Lookup) {
                $("#" + eleI1ID_esigEmail).val(lastClientLookup[index].FirstEmailAddress);
            }
            //$("#" + esigEmail).val(lastClientLookup[index].FirstEmailAddress); //Added 1/29/2019 for eSignature Project task 41686 MLW
        if (lastClientLookup[index].FirstEmailTypeId != "")
            $("#" + eleI1ID_Email_Type).val(lastClientLookup[index].FirstEmailTypeId);
        if (lastClientLookup[index].FirstPhoneNumber != "")
            $("#" + eleI1ID_Phone).val(lastClientLookup[index].FirstPhoneNumber);
        if (lastClientLookup[index].FirstPhoneExt != "" && lastClientLookup[index].FirstPhoneExt != "0")
            $("#" + eleI1ID_PhoneExt).val(lastClientLookup[index].FirstPhoneExt);
        if (lastClientLookup[index].FirstPhoneTypeId != "" && lastClientLookup[index].FirstPhoneTypeId != "0")
            $("#" + eleI1ID_Phone_Type).val(lastClientLookup[index].FirstPhoneTypeId).attr("selected", "selected");

        if (lastClientLookup[index].Address.HouseNum != "")
            $("#" + eleI1ID_StreetNum).val(lastClientLookup[index].Address.HouseNum);
        if (lastClientLookup[index].Address.StreetName != "")
            $("#" + eleI1ID_StreetName).val(lastClientLookup[index].Address.StreetName);
        if (lastClientLookup[index].Address.ApartmentNumber != "")
            $("#" + eleI1ID_AptNum).val(lastClientLookup[index].Address.ApartmentNumber);
        if (lastClientLookup[index].Address.POBox != "")
            $("#" + eleI1ID_POBox).val(lastClientLookup[index].Address.POBox);
        if (lastClientLookup[index].Address.City != "")
            $("#" + eleI1ID_City).val(lastClientLookup[index].Address.City);

        if (lastClientLookup[index].Address.StateId != "")
            $("#" + eleI1ID_State).val(lastClientLookup[index].Address.StateId).attr("selected", "selected");

        if (lastClientLookup[index].Address.Zip != "") {
            if (lastClientLookup[index].Address.Zip.length > 5) {
                $("#" + eleI1ID_Zip).val(lastClientLookup[index].Address.Zip);
                $("#" + eleI1ID_Zip).click(); // in case the county is empty do a zip code lookup
            }
            else {
                $("#" + eleI1ID_Zip).val(lastClientLookup[index].Address.Zip);
                $("#" + eleI1ID_Zip).click(); // in case the county is empty do a zip code lookup
            }
        }

        if (lastClientLookup[index].Address.County != "")
            $("#" + eleI1ID_County).val(lastClientLookup[index].Address.County);

        //if (lastClientLookup[index].Address.Other != "")
        //    $("#" + eleI1ID_CareOf).val(lastClientLookup[index].Address.Other);

        if (lastClientLookup[index].Address.Other != "")
            $("#" + eleI1ID_CareOf).val(ProcessAddressOther(lastClientLookup[index].Address.Other));

        if (lastClientLookup[index].ClientId != "")
            $("#" + eleI1ID_Client).val(lastClientLookup[index].ClientId);

        //added 10/7/2017
        if (lastClientLookup[index].Name.DescriptionOfOperations != "")
            $("#" + eleI1ID_CommDescOfOps).val(lastClientLookup[index].Name.DescriptionOfOperations);
        //if (lastClientLookup[index].Name.DateBusinessStarted != "")
        //    $("#" + eleI1ID_CommBusStartedDt).val(lastClientLookup[index].Name.DateBusinessStarted);
        //if (lastClientLookup[index].Name.YearsOfExperience != "" && lastClientLookup[index].Name.YearsOfExperience != "0")
        //    $("#" + eleI1ID_CommYearsExp).val(lastClientLookup[index].Name.YearsOfExperience);
        if (lastClientLookup[index].Name.DateBusinessStarted != "") {
            $("#" + eleI1ID_CommBusStartedDt).val(lastClientLookup[index].Name.DateBusinessStarted);
            if (lastClientLookup[index].Name.YearsOfExperience != "" && lastClientLookup[index].Name.YearsOfExperience != "0") {
                $("#" + eleI1ID_CommYearsExp).val(lastClientLookup[index].Name.YearsOfExperience);
            }
            Bop.BusinessStartedLessThanThreeYearsAgo(eleI1ID_CommBusStartedDt, eleI1ID_liCommYearsExp, eleI1ID_CommYearsExp);
        }
    }

    //added 8/28/2023 for CommDataPrefill
    function ApplyInsuredOneCommDataPrefill(index) {
        if (lastClientLookup[index].Name.CommercialName1 != "")
            $("#" + commDataPrefill_BusinessName).val(lastClientLookup[index].Name.CommercialName1);

        if (lastClientLookup[index].Name.DoingBusinessAsName != "")
            $("#" + commDataPrefill_DBA).val(lastClientLookup[index].Name.DoingBusinessAsName);

        if (lastClientLookup[index].Name.EntityTypeId != "") {
            $("#" + commDataPrefill_Entity).val(lastClientLookup[index].Name.EntityTypeId).attr("selected", "selected");
            var txtOtherEntityId_Lookup = document.getElementById(commDataPrefill_txtOtherEntityDescription);
            if (txtOtherEntityId_Lookup) {
                if (lastClientLookup[index].Name.EntityTypeId == "5") {
                    $("#" + commDataPrefill_txtOtherEntityDescription).val(lastClientLookup[index].Name.OtherLegalEntityDescription);
                } else {
                    $("#" + commDataPrefill_txtOtherEntityDescription).val();
                }
            }
        }

        var taxTypeSelected = false;
        var isSSN = false;
        var numHyphens;
        if (lastClientLookup[index].Name.TaxTypeId != "") {
            if (lastClientLookup[index].Name.TaxTypeId == "1") {
                //SSN
                isSSN = true;
                numHyphens = 2;
                taxTypeSelected = true;
            } else if (lastClientLookup[index].Name.TaxTypeId == "2") {
                //FEIN
                numHyphens = 1;
                taxTypeSelected = true;
            }
        }
        if (lastClientLookup[index].Name.TaxNumber != "") {
            var taxNumNoHyphens = lastClientLookup[index].Name.TaxNumber.replace(new RegExp('-', 'g'), '').trim();
            if (taxNumNoHyphens.length > 9) {
                taxNumNoHyphens = taxNumNoHyphens.substring(0, 9);
            }
            var taxNumToUse;
            if (taxTypeSelected == false) {
                numHyphens = (lastClientLookup[index].Name.TaxNumber.match(/-/g) || []).length;
                taxTypeSelected = true; //will be determined below
            }
            if (taxNumNoHyphens.length == 9) {
                if (numHyphens == 1) {
                    //FEIN
                    taxNumToUse = taxNumNoHyphens.substr(0, 2) + '-' + taxNumNoHyphens.substr(2, 7); //substring only works the same when starting on 0 index (substring gets the text between 2 indices, excluding the char at the last index); this would actually be taxNumNoHyphens.substring(0, 2) + '-' + taxNumNoHyphens.substring(2, 9)
                } else if (numHyphens == 2) {
                    //SSN
                    taxNumToUse = taxNumNoHyphens.substr(0, 3) + '-' + taxNumNoHyphens.substr(3, 2) + '-' + taxNumNoHyphens.substr(5, 4); //substring only works the same when starting on 0 index (substring gets the text between 2 indices, excluding the char at the last index); this would actually be taxNumNoHyphens.substr(0, 3) + '-' + taxNumNoHyphens.substr(3, 5) + '-' + taxNumNoHyphens.substr(5, 9)
                    isSSN = true;
                } else {
                    //use FEIN; just use taxNum w/o hyphens
                    taxNumToUse = taxNumNoHyphens;
                }
            } else {
                //use FEIN; just use taxNum w/o hyphens
                taxNumToUse = taxNumNoHyphens;
            }
            $("#" + commDataPrefill_FEIN).val(taxNumToUse);
        }
        if (taxTypeSelected == true) {
            if (isSSN == true) {
                //SSN
                $("#" + commDataPrefill_CommTaxType).val("1").attr("selected", "selected");
            } else {
                //FEIN
                $("#" + commDataPrefill_CommTaxType).val("2").attr("selected", "selected");
            }
        }

        if (lastClientLookup[index].FirstEmailAddress != "")
            $("#" + commDataPrefill_Email).val(lastClientLookup[index].FirstEmailAddress);
        if (lastClientLookup[index].FirstEmailTypeId != "")
            $("#" + commDataPrefill_Email_Type).val(lastClientLookup[index].FirstEmailTypeId);
        if (lastClientLookup[index].FirstPhoneNumber != "")
            $("#" + commDataPrefill_Phone).val(lastClientLookup[index].FirstPhoneNumber);
        if (lastClientLookup[index].FirstPhoneExt != "" && lastClientLookup[index].FirstPhoneExt != "0")
            $("#" + commDataPrefill_PhoneExt).val(lastClientLookup[index].FirstPhoneExt);
        if (lastClientLookup[index].FirstPhoneTypeId != "" && lastClientLookup[index].FirstPhoneTypeId != "0")
            $("#" + commDataPrefill_Phone_Type).val(lastClientLookup[index].FirstPhoneTypeId).attr("selected", "selected");

        if (lastClientLookup[index].Address.HouseNum != "")
            $("#" + commDataPrefill_StreetNum).val(lastClientLookup[index].Address.HouseNum);
        if (lastClientLookup[index].Address.StreetName != "")
            $("#" + commDataPrefill_StreetName).val(lastClientLookup[index].Address.StreetName);
        if (lastClientLookup[index].Address.ApartmentNumber != "")
            $("#" + commDataPrefill_AptNum).val(lastClientLookup[index].Address.ApartmentNumber);
        if (lastClientLookup[index].Address.POBox != "")
            $("#" + commDataPrefill_POBox).val(lastClientLookup[index].Address.POBox);
        if (lastClientLookup[index].Address.City != "")
            $("#" + commDataPrefill_City).val(lastClientLookup[index].Address.City);

        if (lastClientLookup[index].Address.StateId != "")
            $("#" + commDataPrefill_State).val(lastClientLookup[index].Address.StateId).attr("selected", "selected");

        if (lastClientLookup[index].Address.Zip != "") {
            if (lastClientLookup[index].Address.Zip.length > 5) {
                $("#" + commDataPrefill_Zip).val(lastClientLookup[index].Address.Zip);
                $("#" + commDataPrefill_Zip).click(); // in case the county is empty do a zip code lookup
            }
            else {
                $("#" + commDataPrefill_Zip).val(lastClientLookup[index].Address.Zip);
                $("#" + commDataPrefill_Zip).click(); // in case the county is empty do a zip code lookup
            }
        }

        if (lastClientLookup[index].Address.County != "")
            $("#" + commDataPrefill_County).val(lastClientLookup[index].Address.County);

        if (lastClientLookup[index].Address.Other != "")
            $("#" + commDataPrefill_Other).val(lastClientLookup[index].Address.Other);

        if (lastClientLookup[index].ClientId != "")
            $("#" + commDataPrefill_Client).val(lastClientLookup[index].ClientId);

        //added 10/7/2017
        if (lastClientLookup[index].Name.DescriptionOfOperations != "")
            $("#" + commDataPrefill_CommDescOfOps).val(lastClientLookup[index].Name.DescriptionOfOperations);
        if (lastClientLookup[index].Name.DateBusinessStarted != "") {
            $("#" + commDataPrefill_CommBusStartedDt).val(lastClientLookup[index].Name.DateBusinessStarted);
            if (lastClientLookup[index].Name.YearsOfExperience != "" && lastClientLookup[index].Name.YearsOfExperience != "0") {
                $("#" + commDataPrefill_CommYearsExp).val(lastClientLookup[index].Name.YearsOfExperience);
            }
        }
    }

    function ProcessAddressOther(input) {
        const CareOfRegex = "^\s*c(are |.|\/) ?o(\.|f)?:?\s*";
        const AttnRegex = "^\s*a(ttn|ttention) ?:?\s*";
        $("#" + eleI1ID_OtherPrefix).val('1').attr("selected", "selected")
        $("#" + eleI2ID_OtherPrefix).val('1').attr("selected", "selected")
        if (input.match(new RegExp(CareOfRegex, 'gmi'))) {

            return input.replace(new RegExp(CareOfRegex, 'gmi'), "");
        }
        if (input.match(new RegExp(AttnRegex, 'gmi'))) {
            $("#" + eleI1ID_OtherPrefix).val('2').attr("selected", "selected")
            $("#" + eleI2ID_OtherPrefix).val('2').attr("selected", "selected")
            return input.replace(new RegExp(AttnRegex, 'gmi'), "");
        }
        return input;
    }

}; // ClientSearch END












