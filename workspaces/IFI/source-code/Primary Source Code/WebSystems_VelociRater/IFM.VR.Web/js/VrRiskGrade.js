
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />

var VRRiskGrade = new function () {

    this.PerformRiskGradeLookup = function (lobId, searchTypeId, searchTerm, versionid, divResults, hiddenRiskGrade, hiddenRiskGrade2, hiddenRiskGradeLookup) {
        ifm.vr.vrdata.RiskGrade.SearchCommRiskGrades($(searchTypeId).val(), $(searchTerm).val(), versionid, function (data) {

            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";

            html += "<th style='width:60px;'>";
            html += "</th>";
            html += "<th style='width:60px;'>";
            html += "GL Class Code";
            html += "</th>";
            html += "<th style='width:100%'>";
            html += "Description"
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "Prop Grade";
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "Gl Grade";
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "Auto Grade"
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "WC Grade";
            html += "</th>";

            html += "</thead>";
            html += "<tbody>";
            if (data != null && data.length > 0) {
                for (var x = 0; x < data.length; x++) {
                    var result = data[x];
                    html += "<tr>";

                    var lookupId = data[x].RiskGradeLookupId;

                    var grade = data[x].PropertyGrade;
                    var grade2 = data[x].GlGrade; // only used for cpp
                    switch (lobId) {
                        case 9: //cgl
                            grade = data[x].GlGrade;
                            break;
                        case 20:
                            grade = data[x].AutoGrade
                            break;
                        case 23: //cpp
                            grade = data[x].PropertyGrade;
                            grade2 = data[x].GlGrade;
                            break;
                        case 28: //cpr
                            grade = data[x].PropertyGrade;
                            break;
                        case 34:
                            grade = data[x].WcGrade
                            break;
                    }




                    html += "<td>";
                    if (lobId != 23) {
                        html += "<input type='button' onclick='$(\"" + hiddenRiskGradeLookup + "\").val(\"" + lookupId + "\");$(\"" + hiddenRiskGrade + "\").val(\"" + grade + "\"); SubmitRiskGradeForm();' value='Select' />";
                    }
                    else {
                        //cpp
                        html += "<input type='button' onclick='$(\"" + hiddenRiskGradeLookup + "\").val(\"" + lookupId + "\");$(\"" + hiddenRiskGrade + "\").val(\"" + grade + "\");$(\"" + hiddenRiskGrade2 + "\").val(\"" + grade2 + "\"); SubmitRiskGradeForm();' value='Select' />";
                    }

                    html += "</td>";

                    html += "<td>";
                    html += result.GlClasscode.toString();
                    html += "</td>";

                    html += "<td>";
                    html += result.Description.toString();
                    html += "</td>";

                    html += "<td>";
                    html += result.PropertyGrade.toString();
                    html += "</td>";

                    html += "<td>";
                    html += result.GlGrade.toString();
                    html += "</td>";

                    html += "<td>";
                    html += result.AutoGrade.toString();
                    html += "</td>";

                    html += "<td>";
                    html += result.WcGrade.toString();
                    html += "</td>";

                    html += "</tr>";
                }
            }
            html += "</tbody>";
            html += "</table>";
            $(divResults).html(html);

            $(divResults + " table").DataTable({
                columnDefs: [
                    { orderable: false, targets: [0, 1, 3, 4, 5, 6] }
                ],
                order: [2, 'asc'],
                searching: false
                //pageLength: 20  // You don't want to set this value or the initial number of records displayed will not match what the number of rows selection says on the control MGB 1/4/2018 
            }); //, jQueryUI: true 

        });
    }

    this.PerformRiskGradeLookupNoMotelHotel = function (lobId, searchTypeId, searchTerm, versionid, divResults, hiddenRiskGrade, hiddenRiskGrade2, hiddenRiskGradeLookup)
    {
        ifm.vr.vrdata.RiskGrade.SearchCommRiskGrades($(searchTypeId).val(), $(searchTerm).val(), versionid, function (data) {

            $(divResults).html(html);
            var html = "<table style='width:100%;'>";
            html += "<thead>";

            html += "<th style='width:60px;'>";
            html += "</th>";
            html += "<th style='width:60px;'>";
            html += "GL Class Code";
            html += "</th>";
            html += "<th style='width:100%'>";
            html += "Description"
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "Prop Grade";
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "Gl Grade";
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "Auto Grade"
            html += "</th>";
            html += "<th style='width:50px;'>";
            html += "WC Grade";
            html += "</th>";

            html += "</thead>";
            html += "<tbody>";
            if (data != null && data.length > 0)
            {
                for (var x = 0; x < data.length; x++) {
                    var result = data[x];
                    if (data[x].GlClasscode !== "64074" && data[x].GlClasscode !== "64075" && data[x].GlClasscode !== "45190" && data[x].GlClasscode !== "45191" && data[x].GlClasscode !== "45192" && data[x].GlClasscode !== "45193") {

                        html += "<tr>";

                        var lookupId = data[x].RiskGradeLookupId;

                        var grade = data[x].PropertyGrade;
                        var grade2 = data[x].GlGrade; // only used for cpp
                        switch (lobId) {
                            case 9: //cgl
                                grade = data[x].GlGrade;
                                break;
                            case 20:
                                grade = data[x].AutoGrade
                                break;
                            case 23: //cpp
                                grade = data[x].PropertyGrade;
                                grade2 = data[x].GlGrade;
                                break;
                            case 28: //cpr
                                grade = data[x].PropertyGrade;
                                break;
                            case 34:
                                grade = data[x].WcGrade
                                break;
                        }

                        html += "<td>";
                        if (lobId != 23) {
                            html += "<input type='button' onclick='$(\"" + hiddenRiskGradeLookup + "\").val(\"" + lookupId + "\");$(\"" + hiddenRiskGrade + "\").val(\"" + grade + "\"); SubmitRiskGradeForm();' value='Select' />";
                        }
                        else {
                            //cpp
                            html += "<input type='button' onclick='$(\"" + hiddenRiskGradeLookup + "\").val(\"" + lookupId + "\");$(\"" + hiddenRiskGrade + "\").val(\"" + grade + "\");$(\"" + hiddenRiskGrade2 + "\").val(\"" + grade2 + "\"); SubmitRiskGradeForm();' value='Select' />";
                        }

                        html += "</td>";

                        html += "<td>";
                        html += result.GlClasscode.toString();
                        html += "</td>";

                        html += "<td>";
                        html += result.Description.toString();
                        html += "</td>";

                        html += "<td>";
                        html += result.PropertyGrade.toString();
                        html += "</td>";

                        html += "<td>";
                        html += result.GlGrade.toString();
                        html += "</td>";

                        html += "<td>";
                        html += result.AutoGrade.toString();
                        html += "</td>";

                        html += "<td>";
                        html += result.WcGrade.toString();
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
                    { orderable: false, targets: [0, 1,3,4,5,6] }
                ],
                order: [2, 'asc'],
                searching: false
                //pageLength: 20  // You don't want to set this value or the initial number of records displayed will not match what the number of rows selection says on the control MGB 1/4/2018 
            }); //, jQueryUI: true 

        });
    }

}; // END VRRiskGrade