
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />

var VrGlClassCodes = new function () {

    //this.PerformSearch = function (classCodeBindingIndex, searchTypeId, searchText, lobTypeId, programTypeId, resultsDiv) {
    //    var b = VrGlClassCodes.UiBindings[classCodeBindingIndex];

    //    // Note lobTypeId is a cast from quickquote.commonobjects.quickquoteobject.lobtypes to int and not the lobid from the diamond database
    //    // it is not me.quote.lobid

    //    $("#" + resultsDiv).html("Performing Search...");
    //    ifm.vr.vrdata.GlClassCodes.searchCodes(searchTypeId, searchText, lobTypeId, programTypeId, function (data) {
    //        if (data != null) {
    //            var html = "<table style='width:100%'>";
    //            html += "<tr style='background-color:orange;color:white;font-weight:700;'>";
    //            html += "<td style='width:60px;'>&nbsp;</td >";
    //            html += "<td style='width:90px;text-align:left;'>Class Code</td>";
    //            html += "<td style='text-align:left;'>Class Code Description</td>";
    //            html += "</tr>";

    //            for (var x = 0; x < data.length; x++) {
    //                html += "<tr>";
    //                html += "<td>";
    //                html += "<input type='button' onclick='VrGlClassCodes.PopulateClassCodeById(" + classCodeBindingIndex + "," + data[x].CodeId.toString() + "," + lobTypeId + "," + programTypeId + ");' value='Select'/>";
    //                html += "</td>";
    //                html += "<td>";
    //                html += data[x].ClassCode;
    //                html += "</td>";
    //                html += "<td>";
    //                html += data[x].Description;
    //                html += "</td>";
    //                html += "</tr>";
    //            }
    //            html += "</table>";

    //            // http://www.datatables.net/examples/index
    //            // JS is referenced via a CDN on the masterpage
    //            $("#" + resultsDiv).html(html);
    //            $("#" + resultsDiv + " table").DataTable({
    //                columnDefs: [
    //                    { orderable: false, targets: [0, 1] }
    //                ],
    //                order: [2, 'asc'],
    //                searching: false,
    //                pageLength: 8
    //            }); //, jQueryUI: true
    //        }
    //    });
    //}; // END PerformSearch

    this.PerformSearch = function (classCodeBindingIndex, searchTypeId, searchText, lobTypeId, programTypeId, resultsDiv, IsIneligibleMotelHotelCodes, isClassCodeAssignmentAvailable) {
        var b = VrGlClassCodes.UiBindings[classCodeBindingIndex];

        // Note lobTypeId is a cast from quickquote.commonobjects.quickquoteobject.lobtypes to int and not the lobid from the diamond database
        // it is not me.quote.lobid

        $("#" + resultsDiv).html("Performing Search...");
        ifm.vr.vrdata.GlClassCodes.searchCodes(searchTypeId, searchText, lobTypeId, programTypeId, function (data) {
            if (data != null)
            {
                var html = "<table style='width:100%'>";
                html += "<thead style='text-align:left;height:20px;background-color:orange;color:white;font-weight:700;padding:0px;'>";
                html += "<th style='width:12%;padding-top:5px;padding-bottom:5px;padding-left:0px;padding-right:0px;text-align:left;'>&nbsp;</th>";
                html += "<th style='width:23%;padding-top:5px;padding-bottom:5px;padding-left:0px;padding-right:0px;text-align:left;'>Class Code</th>";
                html += "<th style='width:65%;padding-top:5px;padding-bottom:5px;padding-left:0px;padding-right:0px;text-align:left;'>Class Code Description</th>";
                html += "</thead>";
                html += "<tbody>";
                for (var x = 0; x < data.length; x++) {
                    if (ifm.vr.currentQuote.isEndorsement == false && IsIneligibleMotelHotelCodes == "True") {
                    //if (IsIneligibleMotelHotelCodes == "True" && ! IsEndorsementTransaction) {
                        if (data[x].CPPMA !== "Motel/Hotel") {
                            html += "<tr>";
                            html += "<td style='width:12%;padding-top:5px;padding-left:0px;padding-right:0px;padding-bottom:5px;'>";
                            html += "<input type='button' onclick='VrGlClassCodes.PopulateClassCodeById(" + classCodeBindingIndex + "," + data[x].CodeId.toString() + "," + lobTypeId + "," + programTypeId + "," + isClassCodeAssignmentAvailable.toString().toLowerCase() + ");' value='Select'/>";
                            html += "</td>";
                            html += "<td style='width:23%;padding-top:5px;padding-left:0px;padding-right:0px;padding-bottom:5px;'>";
                            html += data[x].ClassCode;
                            html += "</td>";
                            html += "<td style='width:65%;padding-top:5px;padding-left:0px;padding-right:0px;padding-bottom:5px;'>";
                            html += data[x].Description;
                            html += "</td>";
                            html += "</tr>";
                        }
                    } else {
                        html += "<tr>";
                        html += "<td style='width:12%;padding-top:5px;padding-left:0px;padding-right:0px;padding-bottom:5px;'>";
                        html += "<input type='button' onclick='VrGlClassCodes.PopulateClassCodeById(" + classCodeBindingIndex + "," + data[x].CodeId.toString() + "," + lobTypeId + "," + programTypeId + "," + isClassCodeAssignmentAvailable.toString().toLowerCase() + ");' value='Select'/>";
                        html += "</td>";
                        html += "<td style='width:23%;padding-top:5px;padding-left:0px;padding-right:0px;padding-bottom:5px;'>";
                        html += data[x].ClassCode;
                        html += "</td>";
                        html += "<td style='width:65%;padding-top:5px;padding-left:0px;padding-right:0px;padding-bottom:5px;'>";
                        html += data[x].Description;
                        html += "</td>";
                        html += "</tr>";
                    }
                }
                html += "</tbody>";
                html += "</table>";

                // http://www.datatables.net/examples/index
                // JS is referenced via a CDN on the masterpage
                $("#" + resultsDiv).html(html);
                $("#" + resultsDiv + " table").DataTable({
                    columnDefs: [
                        { orderable: false, targets:[0,1] }                        
                    ],
                    order: [2, 'asc'],
                    searching: false,
                    pageLength: 8
                }); //, jQueryUI: true 
            }
        });
    }; // END PerformSearch

    this.PopulateClassCodeById = function (classCodeBindingIndex, classCodeId, lobTypeId, programTypeId, isClassCodeAssignmentAvailable) {
        // Note lobTypeId is a cast from quickquote.commonobjects.quickquoteobject.lobtypes to int and not the lobid from the diamond database
        // it is not me.quote.lobid
        ifm.vr.vrdata.GlClassCodes.findSpecificCodeById(classCodeId, lobTypeId, programTypeId, function (data) {
            if (data != null)
            {
                var d = data[0];
                //Adding ALert for 59049 BB
                if (d.ClassCode.toString() == "18707" || d.ClassCode.toString() == "18708" || d.ClassCode.toString() == "99760") {
                    alert("This classification excludes products-completed operations coverage for any vaping & CBD oil related products.");
                }
                var b = VrGlClassCodes.UiBindings[classCodeBindingIndex];
                $(b.DivClassCodeInfo).show();
                $(b.TxtClassCode).val(d.ClassCode.toString());
                VrGlClassCodes.GasolinePopupCheck(d.ClassCode.toString());
                VrGlClassCodes.EPLICheck(d.ClassCode.toString(), b.HDNRemoveEPLIID);
                $(b.TxtClassCodeDescription).val(d.Description);
                if (isClassCodeAssignmentAvailable === true) {
                    //This will set the Class Code Assignment drop down to Location if the Premium Exposure Description (d.PremiumBase) begins with "Area". Full description is "Area, Products/Completed Operations" but most are cut off at Op or Oper.
                    if ((d.PremiumBase).substring(0, 4).toUpperCase() == "AREA") {
                        //Area, Products/Completed Operations
                        $(b.DdlAssignment).val('2'); //2=Location
                        $(b.DdlAssignment).change();
                        $(b.DdlAssignment).attr('disabled', 'disabled');
                    } else {
                        $(b.DdlAssignment).removeAttr('disabled');
                    }
                } else {
                    $(b.DdlAssignment).removeAttr('disabled');
                }
                $(b.DivFootNote).html(d.FootNote);
                $(b.TxtBasis).val(d.PremiumBase);
                $(b.LblBasisShort).val(d.PremiumBaseShort);
                //$(b.TxtPremiumDescription).val(d.CPPMA);
                $(b.TxtPremiumDescription).val(d.PremiumBase);

                VrGlClassCodes.SetArateSection(d, b, true)
                VrGlClassCodes.AddLinksToFootnote(d, b, classCodeBindingIndex, classCodeId, lobTypeId, programTypeId);
            }
        });

    }; // PopulateClassCodeById


    // Probably will never be used. 
    this.PopulateClassCodeByClassCodeNumber = function (classCodeBindingIndex, classCode, lobTypeId, programTypeId) {
        var classCodeId = '';
        ifm.vr.vrdata.GlClassCodes.searchCodes('2', classCode, lobTypeId, programTypeId, function (data) {
            if (data != null)
            {
                if (data.length > 0) {
                    classCodeId = data[0].CodeId.toString();
                    // Note lobTypeId is a cast from quickquote.commonobjects.quickquoteobject.lobtypes to int and not the lobid from the diamond database
                    // it is not me.quote.lobid
                    ifm.vr.vrdata.GlClassCodes.findSpecificCodeById(classCodeId, lobTypeId, programTypeId, function (data) {
                        if (data != null) {
                            var d = data[0];
                            var b = VrGlClassCodes.UiBindings[classCodeBindingIndex];
                            $(b.DivClassCodeInfo).show();
                            $(b.TxtClassCode).val(d.ClassCode.toString());
                            VrGlClassCodes.GasolinePopupCheck(d.ClassCode.toString());
                            VrGlClassCodes.EPLICheck(d.ClassCode.toString(), b.HDNRemoveEPLIID);
                            $(b.TxtClassCodeDescription).val(d.Description);
                            $(b.DivFootNote).html(d.FootNote);
                            $(b.TxtBasis).val(d.PremiumBase);
                            $(b.LblBasisShort).val(d.PremiumBaseShort);
                            $(b.TxtPremiumDescription).val(d.CPPMA);

                            VrGlClassCodes.SetArateSection(d, b, true)
                            VrGlClassCodes.AddLinksToFootnote(d, b, classCodeBindingIndex, classCodeId, lobTypeId, programTypeId);
                        }
                    });
                }
            }
        });
    }; // PopulateClassCodeByClassCodeNumber


    // Used to reload some of the information that is not saved after a save into Diamond but we can basically requery for some of the original information
    this.PopulateClassCodeByClassCodeNumber_Limited = function (classCodeBindingIndex, classCode, lobTypeId, programTypeId) {
        var classCodeId = '';
        ifm.vr.vrdata.GlClassCodes.searchCodes('2', classCode, lobTypeId, programTypeId, function (dt) {
            if (dt != null) {
                if (dt.length > 0) {
                    classCodeId = dt[0].CodeId.toString();
                    // Note lobTypeId is a cast from quickquote.commonobjects.quickquoteobject.lobtypes to int and not the lobid from the diamond database
                    // it is not me.quote.lobid
                    ifm.vr.vrdata.GlClassCodes.findSpecificCodeById(classCodeId, lobTypeId, programTypeId, function (data) {
                        if (data != null) {
                            var d = data[0];
                            var b = VrGlClassCodes.UiBindings[classCodeBindingIndex];
                            $(b.DivClassCodeInfo).show();

                            VrGlClassCodes.SetArateSection(d, b, false)
                            VrGlClassCodes.AddLinksToFootnote(d, b, classCodeBindingIndex, classCodeId, lobTypeId, programTypeId);
                        }
                    });
                }
            }
        });
    }; // PopulateClassCodeByClassCodeNumber_Limited

    this.PopulateFromFootNote = function (classCodeBindingIndex, ccIndex, lobTypeId, programTypeId) {
        var cc = null;
        var b =  VrGlClassCodes.UiBindings[classCodeBindingIndex];

        if (b.ccArray) {
            cc = b.ccArray[ccIndex];
        }

        if (cc) {
            ifm.vr.vrdata.GlClassCodes.findSpecificCodeByClassCode(cc, lobTypeId, programTypeId, function (data) {
                if (data != null && data.length > 0) {
                    $(b.DivClassCodeInfo).show();
                    $(b.TxtClassCode).val(data[0].ClassCode.toString());
                    VrGlClassCodes.GasolinePopupCheck(data[0].ClassCode.toString());
                    VrGlClassCodes.EPLICheck(data[0].ClassCode.toString(), b.HDNRemoveEPLIID);
                    $(b.TxtClassCodeDescription).val(data[0].Description);
                    $(b.DivFootNote).html(data[0].FootNote);
                }
                else {
                    $(b.TxtClassCode).val('');
                    $(b.TxtClassCodeDescription).val('');
                    $(b.DivFootNote).html('');
                }
            });
        }

    }; // END PopulateFromFootNote

    this.UiBindings = new Array();
    this.ClassCodeUiBinding = function (classCodeIndex, txtClassCode, txtDescription, ddAssignment, txtPremiumExposure, txtBasis, lblBasisShort, trArate, txtPremisesArate, txtProductArate, txtPremiumDescription, divClassCodeInfo, hdnaRatePrem, hdnaRateProd, divFootNote, tdAratePremCol, tdArateProdCol, trArateInfoRow, hdnRemoveEPLI) {
        this.ClassCodeIndex = classCodeIndex;

        this.TxtClassCode = '#' + txtClassCode;
        this.TxtClassCodeDescription = '#' + txtDescription;
        this.DdlAssignment = '#' + ddAssignment;
        this.TxtPremiumExposure = '#' + txtPremiumExposure;
        this.TxtBasis = '#' + txtBasis;
        this.LblBasisShort = '#' + lblBasisShort;
        this.TrArate = '#' + trArate;
        this.TrArateClientID = trArate;
        this.TxtPremisesArate = '#' + txtPremisesArate;
        this.TxtPremisesArateClientId = txtPremisesArate;
        this.TxtProductArate = '#' + txtProductArate;
        this.TxtProductArateClientId = txtProductArate;
        this.TxtPremiumDescription = '#' + txtPremiumDescription;
        this.DivClassCodeInfo = '#' + divClassCodeInfo;
        this.HdnaRatePremClientID = hdnaRatePrem;
        this.HdnaRateProdClientID = hdnaRateProd;
        this.DivFootNoteClientID = divFootNote;
        this.DivFootNote = '#' + divFootNote;
        this.TDAratePremColID = tdAratePremCol;
        this.TDArateProdColID = tdArateProdCol;
        this.TRARateInfoRowID = trArateInfoRow;
        this.HDNRemoveEPLIID = hdnRemoveEPLI;
        this.ccArray = new Array();
    };

    this.GasolinePopupCheck = function (ClassCode) {
        if (ClassCode.toString() == '13453' || ClassCode.toString() == '13454' || ClassCode.toString() == '13455') {
            $("#dialog").dialog({
                modal: true,
                buttons: [
                    {
                        text: "OK",
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
        }
    };

    this.EPLICheck = function (ClassCode, hdnRemoveEPLI) {
        var hdnEPLI = document.getElementById(hdnRemoveEPLI);
        if (ClassCode.toString() == '66123'
            //|| ClassCode.toString() == '61222'   mistyped in the BRD - corrected to 66122 below
            || ClassCode.toString() == '66122'
            || ClassCode.toString() == '66309'   // Added 3/18/18 MGB - Missing from BRD
        //|| ClassCode.toString() == '66561' //WS-3578 Allow EPLI Coverage when risk is a medical
            || ClassCode.toString() == '47474'
            || ClassCode.toString() == '67513'
            || ClassCode.toString() == '67512'
            || ClassCode.toString() == '67509'
            || ClassCode.toString() == '67508'
            || ClassCode.toString() == '47468'
            || ClassCode.toString() == '47476'
            || ClassCode.toString() == '47475'
            || ClassCode.toString() == '47471'
            || ClassCode.toString() == '47478'
            || ClassCode.toString() == '47477'
            || ClassCode.toString() == '47473'
            || ClassCode.toString() == '43200'
            || ClassCode.toString() == '44100'
            || ClassCode.toString() == '44101'
            || ClassCode.toString() == '44102'
            || ClassCode.toString() == '44103'
            || ClassCode.toString() == '44104'
            || ClassCode.toString() == '44105'
            || ClassCode.toString() == '44106'
            || ClassCode.toString() == '44107'
            || ClassCode.toString() == '44108'
            || ClassCode.toString() == '44109'
            || ClassCode.toString() == '44110'
            || ClassCode.toString() == '44111'
            || ClassCode.toString() == '44112'
            || ClassCode.toString() == '44113'
            || ClassCode.toString() == '98751'
            || ClassCode.toString() == '43990'
            || ClassCode.toString() == '43991'
            || ClassCode.toString() == '95305'
            || ClassCode.toString() == '44500'
            || ClassCode.toString() == '44501'
            || ClassCode.toString() == '44427'
            || ClassCode.toString() == '44428'
            || ClassCode.toString() == '44429'
            || ClassCode.toString() == '44430'
            || ClassCode.toString() == '44431'
            || ClassCode.toString() == '44432'
            || ClassCode.toString() == '44433'
            || ClassCode.toString() == '44434'
            || ClassCode.toString() == '44435'
            || ClassCode.toString() == '44436'
            || ClassCode.toString() == '44437'
            || ClassCode.toString() == '44438'
            || ClassCode.toString() == '44439'
            || ClassCode.toString() == '44440'
        ) {
            // Class code can't have EPLI
            if (hdnEPLI) {
                hdnEPLI.value = '1';
            }
            $("#dialog_ccCheck").dialog({
                modal: true,
                buttons: [
                    {
                        text: "OK",
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
        }
        else {  
            // EPLI OK with this Class Code
            if (hdnEPLI) {
                hdnEPLI.value = '';
            }
        }
    };


    this.SetArateSection = function (d, b, WipeDataFields) {
        var traRate = document.getElementById(b.TrArateClientID);
        var txtPrem = document.getElementById(b.TxtPremisesArateClientId);
        var txtProd = document.getElementById(b.TxtProductArateClientId);
        var hdnPrem = document.getElementById(b.HdnaRatePremClientID);
        var hdnProd = document.getElementById(b.HdnaRateProdClientID);
        var TDARatePrem = document.getElementById(b.TDAratePremColID);
        var TDARateProd = document.getElementById(b.TDArateProdColID);
        var TRARateInfo = document.getElementById(b.TRARateInfoRowID);

        if (d) {
            if (traRate && txtPrem && txtProd && hdnProd && hdnPrem && TDARatePrem && TDARateProd && TRARateInfo) {
                TRARateInfo.style.display = 'none';
                if (d.PremRate == "Refer to Company" || d.Prodrate == "Refer to Company") {
                    traRate.style.display = '';
                    TRARateInfo.style.display = '';

                    if (d.PremRate == "Refer to Company") {
                        txtPrem.disabled = false;
                        if (WipeDataFields) { txtPrem.value = ''; }
                        hdnPrem.value = '1';
                        TDARatePrem.style.display = '';
                    }
                    else {
                        txtPrem.disabled = true;
                        txtPrem.value = d.PremRate;
                        hdnPrem.value = '';
                        TDARatePrem.style.display = 'none';
                    }

                    if (d.Prodrate == "Refer to Company") {
                        txtProd.disabled = false;
                        if (WipeDataFields) { txtProd.value = ''; }
                        hdnProd.value = '1';
                        TDARateProd.style.display = '';
                    }
                    else {
                        txtProd.disabled = true;
                        txtProd.value = d.ProdRate;
                        hdnProd.value = '';
                        TDARateProd.style.display = 'none';
                    }
                }
                else {
                    traRate.style.display = 'none';
                    txtPrem.value = '';
                    txtProd.value = '';
                    hdnPrem.value = '';
                    hdnProd.value = '';
                }
            }
        }
    };


    this.AddLinksToFootnote = function (d, b, classCodeBindingIndex, classCodeId, lobTypeId, programTypeId) {
        if (d != null && b != null && classCodeBindingIndex != null && classCodeId != null && lobTypeId != null && programTypeId != null) {
            var b = VrGlClassCodes.UiBindings[classCodeBindingIndex];
            var fnText = d.FootNote;
            var repl = /\d{5}/;
            var m = null;
            var cnt = 0;

            b.ccArray = new Array()            
            fnText = d.FootNote;
            // Loop through all of the 5-digit numbers in the string and add a link for each
            do {
                m = repl.exec(fnText);
                if (m) {
                    // Build a placeholder for the class code in the link.  
                    // If we didn't do this the loop would keep picking up the class codes in the replacement string
                    var ph = "{CC(" + cnt.toString() + ")}";
                    b.ccArray.push(m);
                    var repltxt = "<a style='color:blue;font-weight:700;' href='#' onclick='VrGlClassCodes.PopulateFromFootNote(" + classCodeBindingIndex + "," + cnt + "," + lobTypeId + "," + programTypeId + ");'>" + ph.toString() + "</a>";
                    fnText = fnText.replace(m, repltxt);
                }
                cnt += 1;
            } while (m);

            // Replace all the placeholders with their corresponding class codes
            for (i = 0; i < b.ccArray.length; i++) {
                var strToReplace = "{CC(" + i.toString() + ")}";
                fnText = fnText.replace(strToReplace, b.ccArray[i]);
            }

            // Replace the contents of the footnot div with the string (with links) we just built
            var divFN = document.getElementById(b.DivFootNoteClientID);
            if (divFN) {
                if (fnText == "") {
                    divFN.innerHTML = "No foot note.";
                }
                else {
                    divFN.innerHTML = fnText;
                }
            }
        }
    };

    this.EnterLogic = function (event, txtButtonId) {
        if (event.which || event.keyCode) {
            if (event.which == 13 || event.keyCode == 13) {
                document.getElementById(txtButtonId).click();
                event.preventDefault();
            }
        }
    };


}; // End VrGlClassCodes