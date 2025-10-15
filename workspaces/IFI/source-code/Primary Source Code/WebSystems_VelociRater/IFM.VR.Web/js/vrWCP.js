
$(document).ready(function () {


});

var Wcp = new function () {

    this.NoOwnedLocationsCheckboxChanged = function(CheckboxId, txtStreetNumId, txtStreetNameId, txtCityId, ddStateId, txtZipId, txtCountyId, txtNumEmpsId, SaveBtnId, ClearBtnId, AddBtnId, SaveBtn2Id, ClearBtn2Id){
        var cb = document.getElementById(CheckboxId);
        var txtStreetNum = document.getElementById(txtStreetNumId);
        var txtStreetName = document.getElementById(txtStreetNameId);
        var txtCity = document.getElementById(txtCityId);
        var ddState = document.getElementById(ddStateId);
        var txtZip = document.getElementById(txtZipId);
        var txtCounty = document.getElementById(txtCountyId);
        var txtNumEmps = document.getElementById(txtNumEmpsId);
        var SaveBtn = document.getElementById(SaveBtnId);
        var ClearBtn = document.getElementById(ClearBtnId);
        var AddBtn = document.getElementById(AddBtnId);
        var SaveBtn2 = document.getElementById(SaveBtn2Id);
        var ClearBtn2 = document.getElementById(ClearBtn2Id);

        if (cb.checked == true) {
            // CHECKED
            // Disable controls
            txtStreetNum.disabled = true;
            txtStreetName.disabled = true;
            txtCity.disabled = true;
            ddState.disabled = true;
            txtZip.disabled = true;
            txtCounty.disabled = true;
            txtNumEmps.disabled = true;
            // Hide buttons
            SaveBtn.style.visibility = 'hidden';
            ClearBtn.style.visibility = 'hidden';
            AddBtn.style.visibility = 'hidden';
            SaveBtn2.style.visibility = 'hidden';
            ClearBtn2.style.visibility = 'hidden';
        }
        else {
            // UNCHECKED
            // Enable controls
            txtStreetNum.disabled = false;
            txtStreetName.disabled = false;
            txtCity.disabled = false;
            //ddState.disabled = false;
            txtZip.disabled = false;
            txtCounty.disabled = false;
            txtNumEmps.disabled = false;
            // Show buttons
            SaveBtn.style.visibility = 'visible';
            ClearBtn.style.visibility = 'visible';
            AddBtn.style.visibility = 'visible';
            SaveBtn2.style.visibility = 'visible';
            ClearBtn2.style.visibility = 'visible';
        }
    }

    this.CoverageCheckboxChanged = function (sender, CheckBoxId, DataTableRowId, DataTableRowId2, DataTableRowId3, DataTableRowId4, trInfoRowId) {
        var cb = document.getElementById(CheckBoxId);
        var datarow = document.getElementById(DataTableRowId);
        var datarow2 = document.getElementById(DataTableRowId2);
        var datarow3 = document.getElementById(DataTableRowId3);
        var datarow4 = document.getElementById(DataTableRowId4);
        var trInfoRow = document.getElementById(trInfoRowId);

        if (cb.checked == true) {
            // Checkbox is checked
            if (datarow) { datarow.style.display = ''; }
            if (sender == 'INCLSOLE') {
                alert('Sole Proprietors, Partners & LLC Members who elect to be included must provide written proof of health insurance coverage via the Upload tool in VelociRater or sent to your Underwriter.');
            }
            if (trInfoRow) { trInfoRow.style.display = ''; }
        } else {
            // Checkbox is NOT checked
            if (confirm('Are you sure you want to delete this coverage?') == true) {
                if (datarow) {
                    datarow.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId)
                }
                if (datarow2) {
                    datarow2.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId2)
                }
                if (datarow3) {
                    datarow3.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId3)
                }
                if (datarow4) {
                    datarow4.style.display = 'none';
                    this.ClearCoverageFields(DataTableRowId4)
                }
                if (trInfoRow) { trInfoRow.style.display = 'none'; }
            } else {
                cb.checked = true;
                return false;
            }
        };
    };


    this.ClearCoverageFields = function (sender) {
        $('#' + sender + ' input[type=text]').each(function () {
            this.value = '';
        });

        $('#' + sender + ' input[type=checkbox]').each(function () {
            this.checked = false;
        });

        $('#' + sender + ' select').each(function () {
            this.value = '';
        });
    };

    this.ExperienceModificationValueChanged = function (ExpModTextBoxId, ExpModDateControlId) {
        var txtExpMod = document.getElementById(ExpModTextBoxId);
        var txtDt = document.getElementById(ExpModDateControlId);

        // Only allow decimal values with 1 decimal point
        setTimeout(function () {
            var regex = /\d*\.?\d?/g;
            txtDt.value = regex.exec(txtDt.value);
        })

        // If exp mod = 1 then disable the date field otherwise enable it
        //if (txtExpMod && txtDt) {
        //    txtDt.setAttribute("disabled", "false");
        //    var expmodval = txtExpMod.value;
        //    if (!isNaN(expmodval)) {
        //        if (expmodval == 1) {
        //            txtDt.setAttribute("disabled", "true");
        //        }
        //    }
        //}
    };

    // If Employers Liability Limit is 1,000,000 show an alert
    this.EmployersLiabilityLimitsChanged = function (EBLDdlId) {
        var ddlEBL = document.getElementById(EBLDdlId);

        if (ddlEBL) {
            //removed 10/13/2020 w/ Interoperability updates
            //if (ddlEBL.value == "314") {
            //    alert("The user does not have the authority to quote, bind or issue coverage for this risk.  Please refer to your Commercial Underwriter.");
            //}
        }
    };

};

