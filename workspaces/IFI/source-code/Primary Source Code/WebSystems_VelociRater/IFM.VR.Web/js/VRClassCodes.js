var VRClassCode = new function () {

    this.PerformClassCodeLookup = function (lobId, searchTypeId, searchTerm, divResults, hiddenClassCode, hiddenDescription, hiddenDIAClass_Id, ddStateId, validt) {
        // Validate the form - if it doesn't pass validation then don't populate the return data
        if (!VRClassCode.ValidateWCPClassCodeLookupControl(ddStateId, validt)) { return false; }
        var ddState = document.getElementById(ddStateId);
        var StateId = ddState.value;

        ifm.vr.vrdata.ClassCode.SearchClassCodes($(searchTypeId).val(), $(searchTerm).val(), function (data) {
            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";

            html += "<th style='width:60px;'>";
            html += "</th>";
            html += "<th style='width:60px;'>";
            html += "Class Code";
            html += "</th>";
            html += "<th style='width:100%'>";
            html += "Description"
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "ID";
            html += "</th>";

            html += "</thead>";
            html += "<tbody>";

            if (data != null && data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    var result = data[x];
                    html += "<tr>";

                    //var lookupId = data[x].BOPClass_Id;
                    var classcode = result.ClassCode;
                    var rawdesc = result.Description;
                    var desc = rawdesc.replace("'", "&apos;");
                    var diaId = result.DIAClass_Id;

                    html += "<td>";

                    html += "<input type='button' onclick=";
                    html += "'$(\"" + hiddenClassCode + "\").val(\"" + classcode + "\");";
                    html += "$(\"" + hiddenDescription + "\").val(\"" + desc + "\");";
                    html += "$(\"" + hiddenDIAClass_Id + "\").val(\"" + diaId + "\"); ";
                    //html += "$(\"" + hdnStateId + "\").val(\"" + StateId + "\"); ";
                    //html += "$(\"" + hiddenBOPClass_Id + "\").val(\"" + lookupId + "\"); ";

                    html += "SubmitRiskGradeForm();' value='Select';"
                    html += "/>";   // closing tag for the input

                    html += "</td>";

                    html += "<td>";
                    html += result.ClassCode.toString();
                    html += "</td>";

                    html += "<td>";
                    html += result.Description.toString();
                    html += "</td>";

                    html += "<td>";
                    html += diaId.toString();
                    html += "</td>";

                    html += "</tr>";
                }
            }
            html += "</tbody>";
            html += "</table>";
            $(divResults).html(html);

            $(divResults + " table").DataTable({
                columnDefs: [
                    //{ orderable: false, targets: [0, 1, 3, 4, 5, 6] }
                    { orderable: true, targets: [0, 1, 2] }
                ],
                order: [1, 'asc'],
                searching: false,
                //pageLength: 20
            }); //, jQueryUI: true 

        });
    }

    // When the state dropdown is displayed on the Class Code Lookup control, we need to validate it
    // ddStateId = client id of the state dropdown
    // validt = string param should contain TRUE or FALSE - will only validate the dropdown value if this is set to TRUE
    this.ValidateWCPClassCodeLookupControl = function(ddStateID, validt){
        var ddState = document.getElementById(ddStateID);
        var val = null;

        if (ddState && validt) {
            if (validt == 'true' || validt == 'TRUE'){
                if (ddState.value == '0') {
                    alert("You must select a state.");
                    return false
                }
            }
        }
        return true;
    };


    this.PerformPIOClassCodeLookup = function (lobId, stateId, EffectiveDate, searchTypeId, searchTerm, divResults, hiddenDescription, hiddenDIAClass_Id, txtClassCode, txtId, companyId) {
        ifm.vr.vrdata.ClassCode.SearchPIOClassCodes($(searchTypeId).val(), $(searchTerm).val(), stateId, EffectiveDate, companyId, function (data) {
            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";

            html += "<th style='width:60px;'>";
            html += "</th>";
            html += "<th style='width:50px;text-align:left;'>";
            html += "Item Number";
            html += "</th>";
            html += "<th style='width:60px;text-align:left;'>";
            html += "Class Code";
            html += "</th>";
            html += "<th style='width:100%;text-align:left;'>";
            html += "Description"
            html += "</th>";

            html += "</thead>";
            html += "<tbody>";
            if (data != null && data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    var result = data[x];
                    html += "<tr>";

                    // Need to update the id's for the current control
                    hdnPIODescription = hiddenDescription;
                    hdnPIOID = hiddenDIAClass_Id;
                    txtPIOClassCode = txtClassCode;
                    txtPIOID = txtId;

                    //var lookupId = data[x].BOPClass_Id;
                    var classcode = data[x].ClassCode;
                    var rawdesc = data[x].Description;
                    var desc = rawdesc.replace("'", "&apos;");
                    var diaId = data[x].DIAClass_Id;
                    var itemnum = data[x].ItemNum;

                    html += "<td>";

                    html += "<input type='button' style='width:100%;' onclick='";
                    html += "$(\"" + "#" + hiddenDIAClass_Id + "\").val(\"" + diaId + "\"); ";
                    html += "$(\"" + "#" + hiddenDescription + "\").val(\"" + desc + "\");";
                    //html += "$(\"" + hiddenClassCode + "\").val(\"" + classcode + "\");";
                    html += "Cpr.SubmitPIOClassCodeSelection();CloseCCLookupForm();' value='Select' />";

                    html += "</td>";

                    html += "<td>";
                    html += itemnum.toString();
                    html += "</td>";

                    html += "<td>";
                    html += classcode.toString();
                    html += "</td>";

                    html += "<td>";
                    html += desc.toString();
                    html += "</td>";

                    html += "</tr>";
                }
            }
            html += "</tbody>";
            html += "</table>";
            $(divResults).html(html);

            $(divResults + " table").DataTable({
                columnDefs: [
                    { orderable: true, targets: [0, 1, 2] }
                ],
                order: [1, 'asc'],
                searching: false,
                //pageLength: 20
            }); //, jQueryUI: true 

        });
    }

    this.PerformCPRBuildingClassCodeLookup = function (BuildingIndex, lobId, ProgramId, searchTypeId, searchTerm, divResults, IsIneligibleMotelHotelCodes) {
        Cpr.ClearBuildingCLassificationLookupForm();
        ifm.vr.vrdata.ClassCode.SearchCPRBuildingClassCodes($(searchTypeId).val(), $(searchTerm).val(), function (data) {
            var b = Cpr.UiBindings[0];
            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";

            html += "<th style='width:60px;'>";
            html += "</th>";
            html += "<th style='width:60px;text-align:left;'>";
            html += "Class Code";
            html += "</th>";
            html += "<th style='width:100%;text-align:left;'>";
            html += "Description"
            html += "</th>";

            html += "</thead>";
            html += "<tbody>";
            if (data != null && data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    var dItem = data[x];
                    if (IsIneligibleMotelHotelCodes == "True") {
                        if (data[x].CPPMA !== "Motel/Hotel") {
                            html += "<tr>";

                            // Set vars from data
                            var classcode = dItem.ClassCode;
                            var diaId = dItem.CodeId;
                            var rawdesc = dItem.Description;
                            var desc = rawdesc.replace("'", "&apos;");
                            var PMA = dItem.CPPMA;
                            var PMAs = dItem.PMAs;
                            var grouprate = dItem.RateGroup;
                            var ClassLimit = dItem.ClassLimit;

                            var rawFootnote = dItem.FootNote;
                            var FootNote = rawFootnote.replace(/'/g, "&apos;");
                            html += "<td>";

                            html += "<input type='button' style='width:100%;' onclick='";
                            html += "$(\"" + "#" + b.SourceHdnClassCode + "\").val(\"" + classcode + "\"); ";
                            html += "$(\"" + "#" + b.SourceHdnDIA_Id + "\").val(\"" + diaId + "\"); ";
                            html += "$(\"" + "#" + b.SourceHdnDescription + "\").val(\"" + desc + "\");";
                            html += "$(\"" + "#" + b.SourceHdnPMA + "\").val(\"" + PMA + "\");";
                            html += "$(\"" + "#" + b.SourceHdnPMAs + "\").val(\"" + PMAs + "\");";
                            html += "$(\"" + "#" + b.SourceHdnGroupRate + "\").val(\"" + grouprate + "\");";
                            html += "$(\"" + "#" + b.SourceHdnClassLimit + "\").val(\"" + ClassLimit + "\");";
                            html += "$(\"" + "#" + b.SourceHdnFootNote + "\").val(\"" + FootNote + "\");";
                            html += "Cpr.SubmitBuildingClassCodeSelection(" + BuildingIndex + ");' value='Select' />";

                            html += "</td>";

                            html += "<td>";
                            html += classcode.toString();
                            html += "</td>";

                            html += "<td>";
                            html += desc.toString();
                            html += "</td>";

                            html += "</tr>";
                        }
                    } else {
                        html += "<tr>";

                        // Set vars from data
                        var classcode = dItem.ClassCode;
                        var diaId = dItem.CodeId;
                        var rawdesc = dItem.Description;
                        var desc = rawdesc.replace("'", "&apos;");
                        var PMA = dItem.CPPMA;
                        var PMAs = dItem.PMAs;
                        var grouprate = dItem.RateGroup;
                        var ClassLimit = dItem.ClassLimit;

                        var rawFootnote = dItem.FootNote;
                        var FootNote = rawFootnote.replace(/'/g, "&apos;");
                        html += "<td>";

                        html += "<input type='button' style='width:100%;' onclick='";
                        html += "$(\"" + "#" + b.SourceHdnClassCode + "\").val(\"" + classcode + "\"); ";
                        html += "$(\"" + "#" + b.SourceHdnDIA_Id + "\").val(\"" + diaId + "\"); ";
                        html += "$(\"" + "#" + b.SourceHdnDescription + "\").val(\"" + desc + "\");";
                        html += "$(\"" + "#" + b.SourceHdnPMA + "\").val(\"" + PMA + "\");";
                        html += "$(\"" + "#" + b.SourceHdnPMAs + "\").val(\"" + PMAs + "\");";
                        html += "$(\"" + "#" + b.SourceHdnGroupRate + "\").val(\"" + grouprate + "\");";
                        html += "$(\"" + "#" + b.SourceHdnClassLimit + "\").val(\"" + ClassLimit + "\");";
                        html += "$(\"" + "#" + b.SourceHdnFootNote + "\").val(\"" + FootNote + "\");";
                        html += "Cpr.SubmitBuildingClassCodeSelection(" + BuildingIndex + ");' value='Select' />";

                        html += "</td>";

                        html += "<td>";
                        html += classcode.toString();
                        html += "</td>";

                        html += "<td>";
                        html += desc.toString();
                        html += "</td>";

                        html += "</tr>";


                    }
                }
            }
            html += "</tbody>";
            html += "</table>";
            $(divResults).html(html);

            $(divResults + " table").DataTable({
                columnDefs: [
                    { orderable: true, targets: [0, 1, 2] }
                ],
                order: [1, 'asc'],
                searching: false,
                //pageLength: 20
            }); 

        });   
    }

    this.PerformCPPCrimeClassCodeLookup = function (lobId, ProgramId, searchTypeId, searchTerm, divResults) {
        //Cpr.ClearBuildingCLassificationLookupForm();
        ifm.vr.vrdata.ClassCode.SearchCPPCrimeClassCodes($(searchTypeId).val(), $(searchTerm).val(), function (data) {
            var b = Cpp.UiBindings[0];
            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";

            html += "<th style='width:60px;'>";
            html += "</th>";
            html += "<th style='width:60px;text-align:left;'>";
            html += "Class Code";
            html += "</th>";
            html += "<th style='width:100%;text-align:left;'>";
            html += "Description"
            html += "</th>";

            html += "</thead>";
            html += "<tbody>";
            if (data != null && data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    var dItem = data[x];
                    html += "<tr>";

                    // Set vars from data
                    var classcode = dItem.ClassCode;
                    var diaId = dItem.CodeId;
                    var rawdesc = dItem.Description;
                    var desc = rawdesc.replace("'", "&apos;");
                    var PMA = dItem.CPPMA;
                    var PMAs = dItem.PMAs;
                    var grouprate = dItem.RateGroup;
                    var ClassLimit = dItem.ClassLimit;

                    var rawFootnote = dItem.FootNote;
                    var FootNote = rawFootnote.replace(/'/g, "&apos;");
                    html += "<td>";

                    html += "<input type='button' style='width:100%;' onclick='";
                    html += "$(\"" + "#" + b.SourceHdnClassCode + "\").val(\"" + classcode + "\"); ";
                    html += "$(\"" + "#" + b.SourceHdnDIA_Id + "\").val(\"" + diaId + "\"); ";
                    html += "$(\"" + "#" + b.SourceHdnDescription + "\").val(\"" + desc + "\");";
                    html += "$(\"" + "#" + b.SourceHdnPMA + "\").val(\"" + PMA + "\");";
                    html += "$(\"" + "#" + b.SourceHdnPMAs + "\").val(\"" + PMAs + "\");";
                    html += "$(\"" + "#" + b.SourceHdnGroupRate + "\").val(\"" + grouprate + "\");";
                    html += "$(\"" + "#" + b.SourceHdnClassLimit + "\").val(\"" + ClassLimit + "\");";
                    html += "$(\"" + "#" + b.SourceHdnFootNote + "\").val(\"" + FootNote + "\");";
                    html += "Cpp.SubmitCrimeClassCodeSelection(" + b.BuildingIndex + ");' value='Select' />";

                    html += "</td>";

                    html += "<td>";
                    html += classcode.toString();
                    html += "</td>";

                    html += "<td>";
                    html += desc.toString();
                    html += "</td>";

                    html += "</tr>";
                }
            }
            html += "</tbody>";
            html += "</table>";
            $(divResults).html(html);

            $(divResults + " table").DataTable({
                columnDefs: [
                    { orderable: true, targets: [0, 1, 2] }
                ],
                order: [1, 'asc'],
                searching: false,
                //pageLength: 20
            });

        });
    }

    this.PerformCPREQClassificationLookup = function (sender, BldgNbr, LocIndex, BuildingIndex, searchTypeId, searchTermId, divResults, hdnDIAClass_Id, hdnDescriptionId, hdnEQRateGrade) {
        if ($(searchTypeId).val() == "3") { $(searchTypeId).val("1"); }  // Search by rate group is really just a 'begins with' search
        ifm.vr.vrdata.ClassCode.SearchCPREQClassifications($(searchTypeId).val(), $(searchTermId).val(), function (data) {
            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";
            html += "<th style='width:10%;'>&nbsp</th>";
            html += "<th style='width:75%;text-align:left;'>Occupancy and/or Personal Property</th>";
            html += "<th style='width:15%;text-align:center;'>Rate Grade</th>";
            html += "</thead>";
            html += "<tbody>";
            if (data != null && data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    var result = data[x];
                    html += "<tr>";

                    // Set vars from data
                    var diaId = data[x].ID;
                    var rawdesc = data[x].EditedDescription;
                    var desc = '';
                    if (rawdesc) { desc = rawdesc.replace("'", "&apos;"); }
                    var rategrade = data[x].RateGrade;

                    html += "<td>";
                    html += "<input type='button' style='width:100%;' onclick='";
                    html += "$(\"" + "#" + hdnDIAClass_Id + "\").val(\"" + diaId + "\");";
                    html += "$(\"" + "#" + hdnDescriptionId + "\").val(\"" + desc + "\");";
                    html += "$(\"" + "#" + hdnEQRateGrade + "\").val(\"" + rategrade + "\");";
                    if (sender == "PPC") {
                        html += "Cpr.ApplyEQClassification(1," + BldgNbr + "," + LocIndex + "," + BuildingIndex + ");CloseEQCCLookupForm();' value='Select' />";
                    }
                    else {
                        html += "Cpr.ApplyEQClassification(2," + BldgNbr + "," + LocIndex + "," + BuildingIndex + ");CloseEQCCLookupForm();' value='Select' />";
                    }
                    html += "</td>";

                    html += "<td>";
                    html += desc.toString();
                    html += "</td>";

                    html += "<td style='text-align:center;'>";
                    if (rategrade) {
                        html += rategrade.toString();
                    }
                    else {
                        html += '&nbsp;';
                    }
                    html += "</td>";

                    html += "</tr>";
                }
            }
            html += "</tbody>";
            html += "</table>";
            $(divResults).html(html);

            $(divResults + " table").DataTable({
                columnDefs: [
                    { orderable: true, targets: [0, 1, 2] }
                ],
                order: [1, 'asc'],
                searching: false
            });

        });
    }

    // Applies to CPR Building Classification Lookup control
    this.AddLinksToFootnote = function (BuildingIndex, footNoteText, lobTypeId, programTypeId) {
        //var b = Cpr.UiBindings[BuildingIndex];
        var b = Cpr.UiBindings[0];  // Will always be index 0
        var repl = /\d{4}/;   // Note that the class codes we're using here are 4 digits not 5
        var m = null;
        var cnt = 0;
        b.ccArray = new Array()  // this array holds the class code id's'

        // Loop through all of the 4-digit numbers in the string and add a link for each
        do {
            m = repl.exec(footNoteText);
            if (m) {
                // Build a placeholder for the class code in the link.  
                // If we didn't do this the loop would keep picking up the class codes in the replacement string
                var ph = "{CC(" + cnt.toString() + ")}";
                b.ccArray.push(m);
                //var repltxt = "<a href='#' onclick='VrClassCode.PopulateFromFootNote(" + BuildingIndex + "," + cnt + "," + lobTypeId + "," + programTypeId + ");'>" + ph.toString() + "</a>";
                var repltxt = "<a style='color:blue;text-weight:700;' href='#' onclick='VRClassCode.PopulateFromFootNote(" + BuildingIndex + "," + cnt + "," + lobTypeId + "," + programTypeId + ");'>" + ph.toString() + "</a>";
                footNoteText = footNoteText.replace(m, repltxt);
            }
            cnt += 1;
        } while (m);

        // Replace all the placeholders with their corresponding class codes
        for (i = 0; i < b.ccArray.length; i++) {
            var strToReplace = "{CC(" + i.toString() + ")}";
            footNoteText = footNoteText.replace(strToReplace, b.ccArray[i]);
        }

        // Replace the contents of the footnote div with the string (with links) we just built
        var divFN = document.getElementById(b.DivFootNote);
        if (divFN) {
            if (footNoteText == "") {
                divFN.innerHTML = "No foot note.";
            }
            else {
                divFN.innerHTML = footNoteText;
            }
        }

        // If there were any links, show the info text
        if (cnt > 0 && b.SourceTrFootnoteInfoRow) {
            document.getElementById(b.SourceTrFootnoteInfoRow).style.display = '';
        }
        else {
            document.getElementById(b.SourceTrFootnoteInfoRow).style.display = 'none';
        }

        return footNoteText;
    };  // End AddLinksToFootNote

    // Applies to the CPR Building Classification Lookup control
    this.PopulateFromFootNote = function (BuildingIndex, ccIndex, lobTypeId, programTypeId) {
        var cc = null;
        //var b = Cpr.UiBindings[BuildingIndex];
        var b = Cpr.UiBindings[0];  // Will always be index 0
        var ddPMA = document.getElementById(b.SourceDdlPMA);

        // Enable the Apply Button when footnote class code is clicked
        var ApplyButton = document.getElementById(b.SourceApplyButton);
        if (ApplyButton) { ApplyButton.disabled = false; }

        if (ddPMA) { ddPMA.disabled = true;}

        if (b.ccArray) {
            cc = b.ccArray[ccIndex];
        }

        if (cc) {
            // When a footnote link is selected:
            // * Copy the class code to the class code text box
            // * DO NOT change the description text box (it will be set to the value of the class code originally selected)
            // * Parse the PMA(s) from the footnote, populates the PMA dropdown and...
            //      - If there's only 1 PMA, populates the PMA dropdown with it, selects it and disables the dropdown.
            //      - If theres more than 1 PMA, populates the PMA dropdown with them and enables the dropdown.
            //
            // Copy the selected class code to the class code textbox
            var txtCC = document.getElementById(b.SourceTxtClassCode);
            if (txtCC) { txtCC.value = cc; }
            // Parse out the PMA
            var FootNoteValue = document.getElementById(b.DivFootNote).innerHTML;
            if (FootNoteValue) {
                var PMAArray = VRClassCode.GetPMAFromFootnote(FootNoteValue, cc);
                if (ddPMA) { ddPMA.options.length = 0;}
                for (i = 0; i <= PMAArray.length-1; i++) {
                    if (ddPMA) {
                        // If the string contains 'Use Code' then it's part of the next class code, don't use it.
                        // It was much easier to ignore it here than try to remove it when I built the array.
                        if (PMAArray[i].indexOf('use Code') < 0) {
                            var option = document.createElement('option');
                            option.text = option.value = PMAArray[i];
                            ddPMA.add(option);
                        }
                    }
                }
                //Added 8/24/2022 for task 74934 MLW - Updated 9/19/2022 for bug 77438 MLW
                if (ddPMA.options.length == 2) {
                    for (var i = ddPMA.options.length - 1; i >= 0; i--) {
                        if (ddPMA.options[i].value == "") {
                            ddPMA.remove(i);
                        }
                    }
                }
                // Enable the dropdown if more than one item
                if (ddPMA.options.length > 1) { ddPMA.disabled = false;}
            }
        }
    }; // END PopulateFromFootNote

    // Applies to CPP Crime Classification Lookup control
    this.AddLinksToFootnoteCrime = function (BuildingIndex, footNoteText, lobTypeId, programTypeId) {
        //var b = Cpr.UiBindings[BuildingIndex];
        var b = Cpp.UiBindings[0];  // Will always be index 0
        var repl = /\d{4}/;   // Note that the class codes we're using here are 4 digits not 5
        var m = null;
        var cnt = 0;
        b.ccArray = new Array()  // this array holds the class code id's'

        // Loop through all of the 4-digit numbers in the string and add a link for each
        do {
            m = repl.exec(footNoteText);
            if (m) {
                // Build a placeholder for the class code in the link.  
                // If we didn't do this the loop would keep picking up the class codes in the replacement string
                var ph = "{CC(" + cnt.toString() + ")}";
                b.ccArray.push(m);
                //var repltxt = "<a href='#' onclick='VrClassCode.PopulateFromFootNote(" + BuildingIndex + "," + cnt + "," + lobTypeId + "," + programTypeId + ");'>" + ph.toString() + "</a>";
                var repltxt = "<a style='color:blue;text-weight:700;' href='#' onclick='VRClassCode.PopulateFromFootNoteCrime(" + BuildingIndex + "," + cnt + "," + lobTypeId + "," + programTypeId + ");'>" + ph.toString() + "</a>";
                footNoteText = footNoteText.replace(m, repltxt);
            }
            cnt += 1;
        } while (m);

        // Replace all the placeholders with their corresponding class codes
        for (i = 0; i < b.ccArray.length; i++) {
            var strToReplace = "{CC(" + i.toString() + ")}";
            footNoteText = footNoteText.replace(strToReplace, b.ccArray[i]);
        }

        // Replace the contents of the footnote div with the string (with links) we just built
        var divFN = document.getElementById(b.DivFootNote);
        if (divFN) {
            if (footNoteText == "") {
                divFN.innerHTML = "No foot note.";
            }
            else {
                divFN.innerHTML = footNoteText;
            }
        }

        // If there were any links, show the info text
        if (cnt > 0 && b.SourceTrFootnoteInfoRow) {
            document.getElementById(b.SourceTrFootnoteInfoRow).style.display = '';
        }
        else {
            document.getElementById(b.SourceTrFootnoteInfoRow).style.display = 'none';
        }

        return footNoteText;
    };  // End AddLinksToFootNote

    // Applies to the CPP Crime Classification Lookup control
    this.PopulateFromFootNoteCrime = function (BuildingIndex, ccIndex, lobTypeId, programTypeId) {
        var cc = null;
        //var b = Cpr.UiBindings[BuildingIndex];
        var b = Cpp.UiBindings[0];  // Will always be index 0
        var ddPMA = document.getElementById(b.SourceDdlPMA);

        // Enable the Apply Button when footnote class code is clicked
        var ApplyButton = document.getElementById(b.SourceApplyButton);
        if (ApplyButton) { ApplyButton.disabled = false; }

        if (ddPMA) { ddPMA.disabled = true; }

        if (b.ccArray) {
            cc = b.ccArray[ccIndex];
        }

        if (cc) {
            // When a footnote link is selected:
            // * Copy the class code to the class code text box
            // * DO NOT change the description text box (it will be set to the value of the class code originally selected)
            // * Parse the PMA(s) from the footnote, populates the PMA dropdown and...
            //      - If there's only 1 PMA, populates the PMA dropdown with it, selects it and disables the dropdown.
            //      - If theres more than 1 PMA, populates the PMA dropdown with them and enables the dropdown.
            //
            // Copy the selected class code to the class code textbox
            var txtCC = document.getElementById(b.SourceTxtClassCode);
            if (txtCC) { txtCC.value = cc; }
            // Parse out the PMA
            var FootNoteValue = document.getElementById(b.DivFootNote).innerHTML;
            if (FootNoteValue) {
                var PMAArray = VRClassCode.GetPMAFromFootnote(FootNoteValue, cc);
                if (ddPMA) { ddPMA.options.length = 0; }
                for (i = 0; i <= PMAArray.length - 1; i++) {
                    if (ddPMA) {
                        // If the string contains 'Use Code' then it's part of the next class code, don't use it.
                        // It was much easier to ignore it here than try to remove it when I built the array.
                        if (PMAArray[i].indexOf('use Code') < 0) {
                            var option = document.createElement('option');
                            option.text = option.value = PMAArray[i];
                            ddPMA.add(option);
                        }
                    }
                }
                // Enable the dropdown if more than one item
                if (ddPMA.options.length > 1) { ddPMA.disabled = false; }
            }
        }
    }; // END PopulateFromFootNote

    // This function parses through the footnote text and returns an array of PMA 
    // items for the selected class code
    this.GetPMAFromFootnote = function (FootNote, cc) {
        var PMAArray = new Array();
        var rgx = /\d{4}/;   
        var cnt = 1;
        var PMAText = FootNote;

        if (FootNote && cc) {
            // Parse out the PMA's
            do {
                m = rgx.exec(FootNote);
                if (m) {
                    var ndx = FootNote.search(cc);
                    if (ndx)
                    {
                        PMAText = FootNote.substring(ndx + cc.length);
                        // Get the start and end positions of the PMA text
                        var aTagNdx = PMAText.search("</a>");   // Start index - the </a> tag
                        var dotNdx = PMAText.search("[.]");     // End index - the period
                        if (dotNdx) {
                            PMAText = PMAText.substring(aTagNdx + 4, dotNdx);
                        }
                        // Get rid of all the junk we don't need in the PMA text
                        PMAText = PMAText.replace(' and CPP PMA ', '');
                        PMAText = PMAText.replace('CPP PMA ', '');
                        PMAText = PMAText.replace(' or ', ',');
                        PMAText = PMAText.trim();
                        PMAArray.push(PMAText);
                        // Check to see if there's more than one PMA for this class code
                        if (PMAText.indexOf(',') >= 0) {
                            PMAArray = new Array();
                            PMAArray = PMAText.split(",");
                            PMAArray.unshift("");
                        }
                        //Added 8/24/2022 for task 74934 MLW
                        for (i = 0; i <= PMAArray.length - 1; i++) {
                            if (PMAArray[i].indexOf('use Code') < 0) {
                                if (PMAArray[i].indexOf(';') >= 0) {
                                    PMAArray[i] = PMAArray[i].split(";")[0];
                                }
                            }
                        }
                        break;
                    }
                }
                cnt += 1;
            } while (m);
        }
        return PMAArray;
    }; // END GetPMAFromFootnote

}; // END VRClassCode