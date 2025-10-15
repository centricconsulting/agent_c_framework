///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="VRDataFetcher.js" />
///<reference path="vr.core.js" />

var AdditionalInterest = new function () {

    var DialogInitedAddMini = false;
    var lastAdditionalInterestLookup = null;

    function InitAdditionalInterestMini() {
        if (DialogInitedAddMini == false) {
            DialogInitedAddMini = true;
            $("#divAdditionalInterestSearch").dialog({
                title: "Search Results",
                position: { my: "left top", at: "left+40 top+150", of: window },
                width: 400,
                height: 480,
                //draggable: false,
                autoOpen: false,
                //modal: true,
                //dialogClass: "no-close",
                open: function (type, data) { $(this).parent().appendTo("form"); $(this).parent().css({ "position": "fixed" }); }
            });
        }
    }

    function ShowAdditionalInterestSearch() {
        InitAdditionalInterestMini();
        $("#divAdditionalInterestSearch").dialog("open");
    }

    function HideAdditionalInterestSearch() {
        InitAdditionalInterestMini();
        $("#divAdditionalInterestSearch").dialog("close");
    }

    var arrayOfBindings = new Array();
    function AiBinding(AIcontrolId, AIL_CommNameId, AIL_FirstNameId, AIL_MiddleNameId, AIL_LastNameId, AIL_PhoneId, AIL_PhoneExtensionId, AIL_PoBoxId, AIL_AptNumId, AIL_HouseNumId, AIL_StreetNameId, AIL_CityId, AIL_StateId, AIL_ZipId, AIL_AiIDClientId, AIL_IsEditableId) {
        this.AIControlID = AIcontrolId;
        this.Id = AIL_AiIDClientId;

        // control Ids
        this.CommName = AIL_CommNameId;
        this.FirstName = AIL_FirstNameId;
        this.MiddleName = AIL_MiddleNameId;
        this.LastName = AIL_LastNameId;
        this.Phone = AIL_PhoneId;
        this.PhoneExtension = AIL_PhoneExtensionId;
        this.PoBox = AIL_PoBoxId;
        this.AptNum = AIL_AptNumId;
        this.HouseNum = AIL_HouseNumId;
        this.StreetName = AIL_StreetNameId;
        this.City = AIL_CityId;
        this.State = AIL_StateId;
        this.Zip = AIL_ZipId;
        this.IsEditable = AIL_IsEditableId;
    }

    this.SetAdditionalInterestLookupBindings = function(AIcontrolId, AIL_CommNameId, AIL_FirstNameId, AIL_MiddleNameId, AIL_LastNameId, AIL_PhoneId, AIL_PhoneExtensionId, AIL_PoBoxId, AIL_AptNumId, AIL_HouseNumId, AIL_StreetNameId, AIL_CityId, AIL_StateId, AIL_ZipId, AIL_AiIDClientId, AIL_IsEditableID) {
        arrayOfBindings.push(new AiBinding(AIcontrolId, AIL_CommNameId, AIL_FirstNameId, AIL_MiddleNameId, AIL_LastNameId, AIL_PhoneId, AIL_PhoneExtensionId, AIL_PoBoxId, AIL_AptNumId, AIL_HouseNumId, AIL_StreetNameId, AIL_CityId, AIL_StateId, AIL_ZipId, AIL_AiIDClientId, AIL_IsEditableID));
    }

    var DoingAdditionalInterestLookup = false;
    this.DoAdditionalInterestLookup = function(controlId, senderId, commNameId, firstNameId, middleNameId, lastNameId) {
        HideAdditionalInterestSearch(); // always hide then reshow if results are available
        ClearAdditionalInterestResults();
        if (DoingAdditionalInterestLookup == false) {
            var commName = $("#" + commNameId).val();
            var firstName = $("#" + firstNameId).val();
            var middleName = $("#" + middleNameId).val();
            var lastName = $("#" + lastNameId).val();
            
            

            if (agencyID_AdditionalInterest != "-1") { // agency restrictions will be imposed on the server side
                if (commName.length > 2 || firstName.length > 2 || middleName.length > 2 || lastName.length > 2) {
                    VRData.AdditionalInterest.GetAdditionalInterests(commName, firstName, middleName, lastName, function (data) {
                        lastAdditionalInterestLookup = data;
                        DoingAdditionalInterestLookup = false;
                        var focusedElement = ifm.vr.ui.GetCurrentFocusedElementId();
                        for (var i = 0; i < data.length; i++) {
                            if (i == 0)
                                ShowAdditionalInterestSearch();
                            var html = '';
                            html += '<div AdditionalInterestIndex="' + i + '" AiControlID="' + controlId + '" onclick="AdditionalInterest.LoadAdditionalInterestResult($(this));" style="border:solid .4px grey; cursor: pointer; margin-bottom: 5px; color: white; background-color: grey;" class="ui-corner-all">';
                            html += '<table style="width: 100%; margin: 8px;">';

                            html += '<tr>';
                            html += '<td style="width: 100%">';
                            html += '<b>Name:</b> ' + data[i].DisplayName;
                            html += "<br/>";
                            html += "Phone Number: " + data[i].PhoneNumber;
                            html += "<br/>";
                            if (data[i].PhoneExtension != "0" & data[i].PhoneExtension.length > 0) {
                                html += "Phone Extension: " + data[i].PhoneExtension;
                            }
                            html += '</td>';

                            html += '</tr>';

                            html += '<tr>';
                            html += '<td>';
                            html += "<br />";
                            if (data[i].Address_PoBox == "") {
                                if (data[i].Address_AptNum == "") {
                                    //Non apartment
                                    html += data[i].Address_HouseNum + " " + data[i].Address_StreetName;
                                    html += "<br />";
                                    html += data[i].Address_City + ", " + data[i].Address_State + " " + data[i].Address_Zip
                                }
                                else {
                                    // apartment
                                    html += data[i].Address_AptNum + " " + data[i].Address_StreetName;
                                    html += "<br />";
                                    html += data[i].Address_City + ", " + data[i].Address_State + " " + data[i].Address_Zip
                                }
                            }
                            else {
                                // poBox
                                html += "PO Box " + data[i].Address_PoBox + " " + data[i].Address_StreetName;
                                html += "<br />";
                                html += data[i].Address_City + ", " + data[i].Address_State + " " + data[i].Address_Zip
                            }
                            html += '</td>';

                            html += '</table>';
                            html += '</div>';
                                                        
                            $("#divAdditionalInterestResults").append(html);
                            $("#" + focusedElement).focus();
                        }
                    });

                    var focusedElement = ifm.vr.ui.GetCurrentFocusedElementId();
                    $("#" + focusedElement).focus();
                }
                else {
                    var focusedElement = ifm.vr.ui.GetCurrentFocusedElementId();
                    $("#" + focusedElement).focus();
                    DoingAdditionalInterestLookup = false;
                }
            }
            else {
                var focusedElement = ifm.vr.ui.GetCurrentFocusedElementId();
                $("#" + focusedElement).focus();
                DoingAdditionalInterestLookup = false;
            }
        }
    }

    this.LoadAdditionalInterestResult = function(senderEle) {
        var index = $(senderEle).attr('AdditionalInterestIndex');
        var AiControlId = $(senderEle).attr('AiControlID');

        var resultItem = lastAdditionalInterestLookup[index];

        var currentIndexOfExistingBindings = -1;

        // will get the last version of the binding based on the AIControl Id sent

        for (var iii in arrayOfBindings) {
            if (arrayOfBindings[iii].AIControlID == AiControlId) {
                currentIndexOfExistingBindings = iii;
            }
        }

        var binding = arrayOfBindings[currentIndexOfExistingBindings];

        $("#" + binding.Id).val(resultItem.Id);
        if (resultItem.NameTypeId == 2) {
            //com
            $("#" + binding.CommName).val(resultItem.CommercialName);
        }
        else {
            //pers
            $("#" + binding.FirstName).val(resultItem.FirstName);
            $("#" + binding.MiddleName).val(resultItem.MiddleName);
            $("#" + binding.LastName).val(resultItem.LastName);
        }

        $("#" + binding.Phone).val(resultItem.PhoneNumber);
        $("#" + binding.PhoneExtension).val(resultItem.PhoneExtension);
        $("#" + binding.PoBox).val(resultItem.Address_PoBox);
        $("#" + binding.AptNum).val(resultItem.Address_AptNum);
        $("#" + binding.HouseNum).val(resultItem.Address_HouseNum);
        $("#" + binding.StreetName).val(resultItem.Address_StreetName);
        $("#" + binding.City).val(resultItem.Address_City);
        $("#" + binding.State).val(resultItem.Address_StateId);
        $("#" + binding.Zip).val(resultItem.Address_Zip);
        $("#" + binding.IsEditable).val(resultItem.IsEditable);

        HideAdditionalInterestSearch();
    }

    function ClearAdditionalInterestResults() {
        lastAdditionalInterestLookup = null;
        $("#divAdditionalInterestResults").empty(); //clear div
    }


};




