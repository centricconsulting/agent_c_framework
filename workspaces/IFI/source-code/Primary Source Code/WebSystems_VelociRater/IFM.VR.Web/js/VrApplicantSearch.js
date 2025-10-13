
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="VRStringPrototypes.js" />
///<reference path="VrArrays.js" />
///<reference path="vr.core.js" />
///<reference path="VRDataFetcher.js" />

var applicantUiBindings = new Array();

function ApplicantBinding(applicantIndex
                            , headerLabelId
                            , firstNameId
                            , middleNameId
                            , lastNameId
                            , suffixId // dd
                            , ssnId
                            , sexId //dd
                            , dobId
                            , streetNumId
                            , streetNameId
                            , aptNumId
                            , poBoxId
                            , cityId
                            , stateId //dd
                            , zipId
                            , countyId) {
    this.HeaderLabelId = headerLabelId;
    this.ApplicantIndex = applicantIndex;
    this.FirstNameId = firstNameId;
    this.MiddleNameId = middleNameId;
    this.LastNameId = lastNameId;
    this.SuffixId = suffixId;
    this.SSNId = ssnId;
    this.SexId = sexId;
    this.DOBId = dobId;
    this.StreetNumId = streetNumId;
    this.StreetNameId = streetNameId;
    this.AptNumId = aptNumId;
    this.PoBoxId = poBoxId;
    this.CityId = cityId;
    this.StateId = stateId;
    this.ZipId = zipId;
    this.CountyId = countyId;
}

var ApplicantSearch = new function () {
    var lastApplicantLookup = null;
    var DialogInitedMini = false;
    function InitMini() {
        if (DialogInitedMini == false) {
            $("#divClientSearch").hide();
            DialogInitedMini = true;
            var popUpDiv = $("#divForPopups");
            $("#divClientSearch").appendTo(popUpDiv);
        }
    }

    function ShowClientSearch() {
        if ($("#" + enabledClientId).val() == "true") {
            InitMini();
            $("#divClientSearch").show();
        }
        else { HideClientSearch(); }
    }

    function HideClientSearch() {
        InitMini();
        $("#divClientSearch").hide();
    }

    function ClearResults() {
        lastClientLookup = null;
        $("#divResults").empty(); //clear div
    }
    

    this.updateApplicantHeaderText = function(applicantIndex) {
        var bindings = applicantUiBindings[applicantIndex];

        var firstNameText = $("#" + bindings.FirstNameId).val();
        var middleNameText = $("#" + bindings.MiddleNameId).val();
        var lastNameText = $("#" + bindings.LastNameId).val();
        var suffixText = $("#" + bindings.SuffixId).val();

        if (middleNameText == undefined) { middleNameText = ""; }
        if (lastNameText == undefined) { lastNameText = ""; }
        if (suffixText == undefined) { suffixText = ""; }

        var newHeaderText = "Applicant #" + (applicantIndex + 1).toString() + " - " + firstNameText + " " + lastNameText + " " + suffixText;

        var TreeText = firstNameText + " " + lastNameText + " " + suffixText;

        if (middleNameText != "") {
            newHeaderText = "Applicant #" + (applicantIndex + 1).toString() + " - " + firstNameText + " " + middleNameText + " " + lastNameText + " " + suffixText;
            TreeText = firstNameText + " " + middleNameText + " " + lastNameText + " " + suffixText;
        }
        newHeaderText = newHeaderText.toUpperCase();
        //document.getElementById(headerId).setAttribute('value', newHeaderText);
        $("#" + bindings.HeaderLabelId).val(newHeaderText);
        if (newHeaderText.length > 65) {
            $("#" + bindings.HeaderLabelId).text(newHeaderText.substring(0, 65) + "...");
        }
        else {
            $("#" + bindings.HeaderLabelId).text(newHeaderText);
        }

        if (TreeText.length == 1) {
            TreeText = "Applicant " + (applicantIndex + 1).toString();
        }
        TreeText = TreeText.toUpperCase();

        // The tree should define a variable that exposes this labels clientId - Matt A
        $("#cphMain_ctlTreeView_rptPolicyholders_lblPolicyholderDescription_" + applicantIndex).text(TreeText);
    }
        
    this.doApplicantSearch = function(senderId, keycode, applicantIndex) {
        ClearResults();
        HideClientSearch(); // always hide and reshow if returns are available

        if (keycode != 9 && keycode != 17 && keycode != 37) { //do not invoke on Tab, Shift, ALT, or CTRL key presses
            var bindings = applicantUiBindings[applicantIndex];
            var agencyId = master_AgencyId
            var firstNameText = $("#" + bindings.FirstNameId).val();
            var lastNameText = $("#" + bindings.LastNameId).val();
            var zipCodeText = $("#" + bindings.ZipId).val();
            var ssnText = $("#" + bindings.SSNId).val();

            if (ssnText.length == 11 | zipCodeText.length >= 5 | (firstNameText.length >= 0 & lastNameText.length >= 2)) {
                if (ssnText != null)
                    ssnText = ssnText.replace(/-/g, "")

                // agencyID will still be checked against session data to confirm that the current has access to the agency
                VRData.Applicant.GetApplicants(agencyId, firstNameText, lastNameText, zipCodeText, ssnText, function (data) {
                    lastApplicantLookup = data;

                    if (data.length > 0) {
                        $("#" + enabledClientId).val("true");
                        ShowClientSearch();
                    }

                    var html = '';

                    for (var ii = 0; ii < data.length; ii++) {
                        if (data[ii].Name.TaxNumber == "000-00-0000" | data[ii].Name.TaxNumber == "0000-00-0000" | data[ii].Name.TaxNumber == "0000000000" | data[ii].Name.TaxNumber == "00-0000000" | data[ii].Name.TaxNumber == "000000000") {
                            data[ii].Name.TaxNumber = ""
                        }

                        html += "<div onclick='ApplicantSearch.PopulateSelectedApplicant($(this));' lastApplicantLookupindex='" + ii.toString() + "' style='padding-left: 20px; padding-bottom: 10px;'>";

                        html += "</br>" + data[ii].Name.FirstName + " " + data[ii].Name.LastName + " " + data[ii].Name.SuffixName;

                        html += "</br>" + data[ii].Address.HouseNum + " " + data[ii].Address.StreetName;
                        html += "</br>" + data[ii].Address.City + " " + data[ii].Address.State + " " + data[ii].Address.Zip;

                        html += "</br>DOB: " + data[ii].Name.BirthDate;
                        html += "</br>";

                        html += "</div>";
                    }

                    $("#divResults").append(html);

                });
            }
        }
    }

    this.PopulateSelectedApplicant = function(sender) {
        var accordIndex = parseInt(GetActiveAccordionIndex(applicantsAccordId));
        var dataIndex = parseInt($(sender).attr("lastApplicantLookupindex"));
        if (confirm('Apply to Applicant #' + (accordIndex + 1).toString())) {
            var bindings = applicantUiBindings[accordIndex];
            $("#" + bindings.FirstNameId).val(lastApplicantLookup[dataIndex].Name.FirstName);
            $("#" + bindings.MiddleNameId).val(lastApplicantLookup[dataIndex].Name.MiddleName);
            $("#" + bindings.LastNameId).val(lastApplicantLookup[dataIndex].Name.LastName);

            $("#" + bindings.SuffixId).val(lastApplicantLookup[dataIndex].Name.SuffixName).attr("selected", "selected");

            $("#" + bindings.SexId).val(lastApplicantLookup[dataIndex].Name.SexId).attr("selected", "selected");

            $("#" + bindings.SSNId).val(lastApplicantLookup[dataIndex].Name.TaxNumber);
            $("#" + bindings.DOBId).val(lastApplicantLookup[dataIndex].Name.BirthDate);

            $("#" + bindings.StreetNumId).val(lastApplicantLookup[dataIndex].Address.HouseNum);
            $("#" + bindings.StreetNameId).val(lastApplicantLookup[dataIndex].Address.StreetName);
            $("#" + bindings.AptNumId).val(lastApplicantLookup[dataIndex].Address.ApartmentNumber);
            $("#" + bindings.PoBoxId).val(lastApplicantLookup[dataIndex].Address.POBox);
            $("#" + bindings.CityId).val(lastApplicantLookup[dataIndex].Address.City);
            $("#" + bindings.StateId).val(lastApplicantLookup[dataIndex].Address.StateId).attr("selected", "selected");
            $("#" + bindings.ZipId).val(lastApplicantLookup[dataIndex].Address.Zip);
            $("#" + bindings.CountyId).val(lastApplicantLookup[dataIndex].Address.County);

            ClearResults();
            HideClientSearch();
        }
    }

    this.CopyPh1AddressToApplicant = function(applicantIndex) {
        var binding = applicantUiBindings[parseInt(applicantIndex)];

        $("#" + binding.StreetNumId).val($("#" + eleI1ID_StreetNum).val());
        $("#" + binding.StreetNameId).val($("#" + eleI1ID_StreetName).val());
        $("#" + binding.AptNumId).val($("#" + eleI1ID_AptNum).val());
        $("#" + binding.PoBoxId).val($("#" + eleI1ID_POBox).val());
        $("#" + binding.CityId).val($("#" + eleI1ID_City).val());
        $("#" + binding.StateId).val($("#" + eleI1ID_State).val()).attr("selected", "selected");
        $("#" + binding.ZipId).val($("#" + eleI1ID_Zip).val());
    }

}; //ApplicantSearch END



