///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />


function KeyValuePair(key, value) {
    this.key = key;
    this.value = value;
}


var ProtectionClass = new function () {
    var standardProtectionClassValues = new Array();
    standardProtectionClassValues.push(new KeyValuePair('1', '1'));
    standardProtectionClassValues.push(new KeyValuePair('2', '2'));
    standardProtectionClassValues.push(new KeyValuePair('3', '3'));
    standardProtectionClassValues.push(new KeyValuePair('4', '4'));
    standardProtectionClassValues.push(new KeyValuePair('5', '5'));
    standardProtectionClassValues.push(new KeyValuePair('6', '6'));
    standardProtectionClassValues.push(new KeyValuePair('7', '7'));
    standardProtectionClassValues.push(new KeyValuePair('8', '8'));
    standardProtectionClassValues.push(new KeyValuePair('9', '9'));
    standardProtectionClassValues.push(new KeyValuePair('10', '10'));
    standardProtectionClassValues.push(new KeyValuePair('11', '11'));
    //Comm
    //standardProtectionClassValues.push(new KeyValuePair('01', '12'));
    //standardProtectionClassValues.push(new KeyValuePair('02', '13'));
    //standardProtectionClassValues.push(new KeyValuePair('03', '14'));
    //standardProtectionClassValues.push(new KeyValuePair('04', '15'));
    //standardProtectionClassValues.push(new KeyValuePair('05', '16'));
    //standardProtectionClassValues.push(new KeyValuePair('06', '17'));
    //standardProtectionClassValues.push(new KeyValuePair('07', '18'));
    //standardProtectionClassValues.push(new KeyValuePair('08', '19'));
    //standardProtectionClassValues.push(new KeyValuePair('8b', '20'));
    //standardProtectionClassValues.push(new KeyValuePair('09', '21'));
    //standardProtectionClassValues.push(new KeyValuePair('10', '22'));

    this.GetProtectionClass = function(city, county, stateId, ddProtectionId, txtfeetId, txtMilesId, trFeetId, trMilesId) {
        $("#" + ddProtectionId).parent().parent().next().hide();
        var isCity = true;

        var cityorCounty = '';
        if (city.trim() != '') {
            cityorCounty = city;
            isCity = true;
        }
        else {
            cityorCounty = county;
            isCity = false;
        }
        if (cityorCounty.trim() == '') {
            // if city andd county are empty just make sure that the dropdown has the right values
            $("#" + ddProtectionId).parent().parent().next().show();
            var currentValue = $("#hiddenSelectedProtectionClassId").val();
            var ddHadSelection = currentValue != '';
            $("#" + ddProtectionId).empty();
            $("#" + ddProtectionId).append($('<option></option>').val('').html('').attr('title', 'Make a selection.'));
            AddStandardProtectionClassValues(ddProtectionId);
            if (ddHadSelection) {
                $("#" + ddProtectionId).val(currentValue); //set it back to what it was
            }
            return;
        }

        VRData.ProtectionClass.GetProtectionClass(cityorCounty, isCity, stateId, function (data) {
            var currentValue = "";
            if ($("#hiddenSelectedProtectionClassId").val() != null) {
                currentValue = $("#hiddenSelectedProtectionClassId").val().toString().trim();
            }
            var ddHadSelection = currentValue != '';
            if (ddProtectionId == "") { return; }  // Needed in the case where protection class is not a ddl MGB 8/16/16
            $("#" + ddProtectionId).empty();
            $("#" + ddProtectionId).append($('<option></option>').val('').html('').attr('title', 'Make a selection.'));
            AddStandardProtectionClassValues(ddProtectionId);
            //var feetFromHyrant = $("#" + txtfeetId).val();  not used MGB 8/16/16
            //var milesFromFire = $("#" + txtMilesId).val();  not used MGB 8/16/16

            if (data.length > 0) {
                for (var ii = 0; ii < data.length; ii++) {
                    var township = data[ii].Township;
                    var pc = data[ii].ProtectionClass;
                    var footnote = data[ii].FootNote;
                    var pcId = data[ii].PC_ID;
                    
                    $("#" + ddProtectionId).append($('<option></option>').val(pcId).html(township + ' (' + pc + ')').attr('title', footnote));
                }
            }
            //AddStandardProtectionClassValues(ddProtectionId);
            if (ddHadSelection) {
                //$("#" + ddProtectionId).val(currentValue); //set it back to what it was
                $("#" + ddProtectionId + " option").filter(function (index) { return $(this).text() === currentValue; }).attr('selected', 'selected');
                //$("#" + ddProtectionId + " option[text='" + currentValue + "']").attr("selected", "selected");
                NeedtoShowMiles(trFeetId, trMilesId, txtfeetId);
            }
        });



    }
        
    function AddStandardProtectionClassValues(ddList) {
        for (var ii = 0; ii
            < standardProtectionClassValues.length; ii++) {
            $("#" + ddList).append($('<option></option>').val(standardProtectionClassValues[ii].value).html(standardProtectionClassValues[ii].key));
        }
    }

    function NeedtoShowMiles(trFeetId, trMilesId, txtfeetId) {
        var currentVal = $("#hiddenSelectedProtectionClassId").val().toString().trim();
        if (currentVal.indexOf("/") > -1) {
            $("#" + trFeetId).show();
            $("#" + trMilesId).show();
            if(master_LobId !== '3')
            { //Not sure this is necessary. Ends up causing some funky tabbing order stuff for DFR. - Daniel Gugenheim
                $("#" + txtfeetId).focus();
            }
            
        }
    }

    // Set the value of the hidden protection class id - VIEW A
    this.ProtectionClassChanged = function (sender, txtfeetId, txtMilesId, trFeetId, trMilesId) {
        var selectedText = $("#" + sender + " option:selected").text();
        $("#hiddenSelectedProtectionClassId").val(selectedText);
        if ($("#" + sender).val().toString().indexOf("|") > -1)// | $("#" + txtfeetId).val() != '' | $("#" + txtMilesId).val() != '')
        {
            $("#" + trFeetId).show();
            $("#" + trMilesId).show();
            $("#" + txtfeetId).focus();
        }
        else {
            $("#" + trFeetId).hide();
            $("#" + trMilesId).hide();
        }
    }

    // Set the value of the hidden protection class id - VIEW B
    this.ProtectionClassChangedB = function (sender, ddlfeetId, ddlMilesId) {
        var selectedText = $("#" + sender + " option:selected").text();
        $("#hiddenSelectedProtectionClassId").val(selectedText);
        //if ($("#" + sender).val().toString().indexOf("|") > -1)// | $("#" + txtfeetId).val() != '' | $("#" + txtMilesId).val() != '')
        //{
        //    $("#" + txtfeetId).focus();
        //}
    }

    // Set the value of the hidden protection class id - VIEW C
    this.ProtectionClassChangedC = function (sender) {
        var selectedText = $("#" + sender).val
        //var selectedText = $("#" + sender + " option:selected").text();
        $("#hiddenSelectedProtectionClassId").val(selectedText);
        //if ($("#" + sender).val().toString().indexOf("|") > -1)// | $("#" + txtfeetId).val() != '' | $("#" + txtMilesId).val() != '')
        //{
        //    $("#" + txtfeetId).focus();
        //}
    }

    this.MilesToFDChanged = function (sender) {
        var selectedText = $("#" + sender).val
        $("#HiddenSelectedMilesToFireDepartmentID").val(selectedText);
    }

};

