///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />


var BopBuilding = new function () {

    this.populateClassifications = function (ddprogramType,ddClassification, txtClassCode) {
        $('#' + ddClassification).empty();
        $('#' + txtClassCode).val('');
        var programType = $('#' + ddprogramType).val();
        
        
        ifm.vr.vrdata.BopBuildingClassifications.searchClassifications(programType, function (data) {
            if (data) {
                

                $("<option />", {
                    val: "",
                    text: ""
                }).appendTo($('#' + ddClassification));

                $(data).each(function () {
                    $("<option />", {
                        val: this.Value,
                        text: this.Key
                    }).appendTo($('#' + ddClassification));
                });

            }
        });
    }; // END populateClassifications

    this.populateClassCode = function (ddClassification, txtClassCode) {
        var selectedClassification = $('#' + ddClassification).val();
        if (selectedClassification.length > 0)
            $('#' + txtClassCode).val(selectedClassification.split('-')[0]);
        else
            $('#' + txtClassCode).val('');

    }; // END populateClassCode
    

};