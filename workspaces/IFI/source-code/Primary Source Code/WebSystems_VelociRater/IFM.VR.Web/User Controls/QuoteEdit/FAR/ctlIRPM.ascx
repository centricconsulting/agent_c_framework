<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlIRPM.ascx.vb" Inherits="IFM.VR.Web.ctlIRPM" %>

<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %><%--added 12/3/2020 (Interoperability)--%>

<!-- #region javascript -->

<script type="text/javascript">

    // GloabalFunctions
    function CheckConstants() {
        $('.neverShow').hide();
        $('.alwaysShow').show();
    };

    // This function will show or hide a description (additional info) element based on the Riskbox element
    function ShowOrHideAdditionalInputPanel(element, ShowOrHide) {
        var AddInfo = $(element.closest("table").next('.DescriptionTable'));

        if (ShowOrHide == "SHOW") {
            if (!$(AddInfo).hasClass("neverShow")) {
                $(AddInfo).show();
            }
        }
        else {
            if (!$(AddInfo).hasClass("alwaysShow")) {
                $(AddInfo).hide();
                $(AddInfo).find('textarea').val('')
                $(AddInfo).find('span').text('Additional Information')

                $(AddInfo).find('textarea').attr('style', 'border: none;');
                $(AddInfo).find('span').css("color", "black");
            }
        }
        CheckConstants();
    }

    //IRPM Questions Description Box max character check
    function CheckMaxTextNoDisable(textarea, maxLength) {
        var len = $(textarea).val().length;
        var char = maxLength - len;
        var descTextId = $(textarea).attr("id") + "_warning";
        if (len >= maxLength) {
            //$('[id$=btnSubmit]').attr('disabled', 'disabled');
            $(textarea).parent().find('textarea').attr('style', 'border: 1px solid red;');
            //$(textarea).parent().find('textarea').attr('disabled', 'disabled');
            $(textarea).parent().find('span')[0].innerText = "Maximum of " + maxLength + " characters exceeded";
            $(textarea).parent().find('span').first().css("color", "red");
            $(textarea).parent().find('textarea').focus()
            //$(textarea).parent().find('textarea').attr('disabled', false);
            $(textarea).parent().find('textarea').keydown(function (e) {
                if (e.keyCode == 8 || char < maxLength)
                    return true;
            })
            return false
        }
        else {
            var char = maxLength - len;
            //$('[id$=btnSubmit]').removeAttr('disabled');
            $(textarea).parent().find('textarea').first().attr('style', 'border: none;');
            $(textarea).parent().find('span')[0].innerText = char + " characters remaining";
            $(textarea).parent().find('span').first().css("color", "black");
            $(textarea).parent().find('span').first().innerText = "Additional Information Response Required"
        }
        return true
    }
    
    // This function checks that all required Additional Info textboxes have values.
    // Draws a red border around any required textboxes without a value.
    // Changes the text of the description label to 'Additional Information Response Required' if a required value is not present.
    // Returns true if all are answered, false if not
    // Opens collapsed accordions if they contain errors
    function AllOpenTextboxesHaveValues() {
        var allHaveAnAnswers = true;
        var maxLength = 125;

        $('.RiskTextBox').each(function () {
            var addInfo = $(this).closest("table").next('.DescriptionTable');
            var textArea = $(addInfo.find('textarea'))
            var spanArea = $(addInfo.find('span'))

            if ($(this).val() != 0 || addInfo.hasClass("alwaysShow")) {
                if (textArea.val() == '') {
                    allHaveAnAnswers = false;
                    textArea.attr('style', 'border: 1px solid red;');
                    spanArea.text("Additional Information Response Required");
                    spanArea.css("color", "red");
                }
                else if (!CheckMaxTextNoDisable(textArea, maxLength)) {
                    allHaveAnAnswers = false;
                }
                else {
                    textArea.attr('style', 'border: none;');
                    spanArea.css("color", "black");
                    spanArea.text("Additional Information");
                }

            };

        });

        if (allHaveAnAnswers) {
            $('.QuestionsAnsweredError').hide()
        }
        else {
            $('.QuestionsAnsweredError').show()
        }

        
        return allHaveAnAnswers;
    }

    /// Form validation
    /// Called from the SUBMIT button OnClientClick event
    function ValidateForm() {
        var b = AllOpenTextboxesHaveValues();
        return (b);
    }

    // Initial Page load.
    $(function () {

        //Check for items that should always/never display
        CheckConstants();

        // Select additional Info based off item value
        $('.RiskTextBox').each(function () {
            var riskValue = parseFloat($(this).val());

            if (isNaN(riskValue)) {
                $(this).val(0);
                return
            }

            if (riskValue == 0) {
                ShowOrHideAdditionalInputPanel($(this), 'HIDE')
            }
            else {
                ShowOrHideAdditionalInputPanel($(this), 'SHOW')
            }

        });

    });

    // Live functionality
    var isTabbed = false
    $(function () {
        $('.RiskTextBox').on('keydown', function (event) {
            isTabbed = false
            var key = window.event ? event.keyCode : event.which;
            if (key == 9) {
                isTabbed = true
            }
        });


        // Text Change
        $('.RiskTextBox').on('change', function (event) {
            TextBoxControl($(this))
        });

        function TextBoxControl(element) {
            var riskValue = parseFloat(element.val());
            if (isNaN(riskValue))
            {
                element.val(0);
                ShowOrHideAdditionalInputPanel(element, 'HIDE')
                AllOpenTextboxesHaveValues()
                // Calc Total changes
                //CalcTotals(); //removed 10/8/2020 since the function doesn't exist here; is present on Commercial IRPM control
                return
            }

            if (riskValue == 0)
            {
                ShowOrHideAdditionalInputPanel(element, 'HIDE')
                AllOpenTextboxesHaveValues()
            }
            else
            {
                ShowOrHideAdditionalInputPanel(element, 'SHOW')
                if (isTabbed == true) {
                    element.closest("table").next('.DescriptionTable').find('.DescriptionTextBox').focus();
                }
            }
            isTabbed = false
        };

    });

       
</script>

<!-- #endregion -->

<div id="dvFarmIRPM" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblMainHeader" runat="server" Text="IRPM Information (Credits/Debits)"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearFarmIRPM" runat="server" OnClientClick="var confirmed = confirm('Clear IRPM?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset IRPM to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveFarmIRPM" runat="server" ToolTip="Save IRPM" CssClass="RemovePanelLink" OnClientClick="return ValidateForm();">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="dvFarmIRPMDesc" runat="server">
            <h3>
                <asp:Label ID="lblDescHdr" runat="server" Text="DESCRIPTION"></asp:Label>
            </h3>
            <div>
                <table id="tbSupportingBusiness" runat="server" class="tableBorder">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnSupportingBusiness" runat="server" TabIndex="-1">Supporting Business</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblSupportingBusinessRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtSupportingBusiness" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbSupportingBusinessDesc" runat="server" class="DescriptionTable" style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblSupportingBusinessDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtSupportingBusinessDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
               
                <table id="tbCareCondition" runat="server" class="tablegray">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnCareCondition" runat="server" TabIndex="-1">Care and condition of premises and equipment</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblCareConditionRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtCareCondition" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbCareConditionDesc" runat="server" class="DescriptionTable tablegray" style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblCareConditionDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtCareConditionDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbDamage" runat="server" class="tableBorder">
                    <tr>
                        <td style="width: 70%">
                            <asp:Label ID="lblDamage" runat="server" Text="Damage susceptibility"></asp:Label>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblDamageRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtDamage" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbDamageDesc" runat="server" class="DescriptionTable " style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblDamageDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtDamageDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbConcentration" runat="server" class="tablegray">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnConcentration" runat="server" TabIndex="-1">Dispersion or concentration</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblConcentrationRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtConcentration" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbConcentrationDamageDesc" runat="server" class="DescriptionTable tablegray" style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblConcentrationDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtConcentrationDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbLocation" runat="server" class="tableBorder">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnLocation" runat="server" TabIndex="-1">Location</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblLocationRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtLocation" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbLocationDesc" runat="server" class="DescriptionTable " style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblLocationDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtLocationDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbMisc" runat="server" class="tablegray">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnMisc" runat="server" TabIndex="-1">Miscellaneous protective features or hazards</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblMiscRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtMisc" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbMiscDesc" runat="server" class="DescriptionTable tablegray" style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblMiscDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtMiscDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbRoof" runat="server" class="tableBorder">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnRoof" runat="server" TabIndex="-1">Roof condition and other windstorm exposures</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblRoofRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtRoof" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbRoofDesc" runat="server" class="DescriptionTable " style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblRoofDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtRoofDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbStruct" runat="server" class="tablegray">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnStruct" runat="server" TabIndex="-1">Superior or inferior structural features</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblStructRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtStruct" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbStructDesc" runat="server" class="DescriptionTable tablegray" style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblStructDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtStructDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbPastLosses" runat="server" class="tableBorder">
                    <tr>
                        <td style="width: 70%">
                            <asp:LinkButton ID="lbtnPastLosses" runat="server" TabIndex="-1">Past Losses</asp:LinkButton>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblPastLossesRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtPastLosses" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbPastLossesDesc" runat="server" class="DescriptionTable " style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblPastLossesDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtPastLossesDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbRiceHulls" runat="server" class="tablegray">
                    <tr>
                        <td style="width: 70%">
                            <asp:Label ID="lblRiceHulls" runat="server" Text="Use of rice hulls or flame retardant bedding"></asp:Label>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblRiceHullsRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtRiceHulls" runat="server" Text="0" MaxLength="4" Width="20px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbRiceHullsDesc" runat="server" class="DescriptionTable tablegray" style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblRiceHullsDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtRiceHullsDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbPoultry" runat="server" class="tableBorder">
                    <tr>
                        <td style="width: 70%">
                            <asp:Label ID="lblPoultry" runat="server" Text="Regular on-site inspections by the company providing the poultry"></asp:Label>
                        </td>
                        <td style="width: 20%" align="center">
                            <asp:Label ID="lblPoultryRange" runat="server" Text="min -5% / max 5%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtPoultry" runat="server" Text="0" MaxLength="4" Width="25px" CssClass="RiskTextBox">0</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbPoultryDesc" runat="server" class="DescriptionTable " style="display: none;">
                    <tr style="padding: auto 5px;">
                        <td class="TableDescriptionColumn">
                            <br />
                            <asp:Label ID="lblPoultryDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                            <asp:TextBox ID="txtPoultryDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table id="tbTotal" runat="server" class="tablegray">
                    <tr>
                        <td style="width: 69%">
                            <asp:Label ID="lblTotal" runat="server" Text="Total"></asp:Label>
                        </td>
                        <td style="width: 23%" align="center">
                            <asp:Label ID="lblTotalRange" runat="server" Text="min -15% / max 15%"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblTotalValue" runat="server" Text="0" Width="25px" BorderStyle="Solid" BorderWidth="1px" BorderColor="#999999" Height="20px"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tableBorder">
                    <tr>
                        <td><%--Please note, the total credit for Property or General Liability cannot exceed 15%
                            Total credits greater than 15% will require Underwriting Approval--%>
                            IRPM credits or debits may be considered based on the qualities of the risk presented.
                        </td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnSaveRate" runat="server" Text="Save and Re-Rate" CssClass="StandardSaveButton" OnClientClick="return ValidateForm();" Enabled="false" />
                        </td>
                        <td></td>
                        <td>
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="StandardSaveButton" />
                        </td>
                    </tr>
                </table>
                <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" /><%--added 12/3/2020 (Interoperability)--%>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hiddenFarmIRPM" runat="server" />
    <asp:HiddenField ID="hiddenDesc" runat="server" />
</div>

<%--Information Popups--%>

<div id="dvSupportingBusiness" style="display: none">
    <div>
        Credit may be given if the insured has other insurance (e.g. auto or business policies) with Indiana Farmers Mutual
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnSBOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvCareConditionInfoPopup" style="display: none">
    <div>
        a. Maintenance of buildings
        <br />
        b. Removal of weeds, grass and debris from buildings
        <br />
        c. Maintenance and adequacy of fences for the type livestock kept
        <br />
        d. Machinery well maintained and all guards in place
        <br />
        e. Age of machinery and degree obsolescence
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnCCOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvConcentration" style="display: none">
    <div>
        a. Separation of 100 feet between major buildings to reduce spread of fire
        <br />
        b. Dispersion of farm personal property over the farm with no high concentration of value in one building or small area
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnDCOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvLocation" style="display: none">
    <div>
        a. Distance from and adequacy of responding fire department; water supply for fire fighting
        <br />
        b. Accessibility by emergency vehicles
        <br />
        c. Isolated from neighbors
        <br />
        d. Congested areas; proximity of neighboring exposures
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnLocOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvMisc" style="display: none">
    <div>
        a. Lightning rods
        <br />
        b. Smoke or fire alarms in barns and outbuildings
        <br />
        c. Fire extinguishers (in buildings and on self-propelled farm machinery)
        <br />
        d. Lighting of premises to discourage theft and vandalism
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnMiscOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvRoof" style="display: none">
    <div>
        a. Age and condition of roof
        <br />
        b. Roof coverings properly secured
        <br />
        c. Siding and gutters properly secured
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnRoofOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvStruct" style="display: none">
    <div>
        a. Proper installation and maintenance of electrical wiring, fixtures and controls
        <br />
        b. Heating systems designed for agricultural use; properly installed and maintained
        <br />
        c. Fire wall divisions in large buildings
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnStruct" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<div id="dvPastLosses" style="display: none">
    <div>
        a. Number of losses reported the previous 3 years
        <br />
        b. Type of losses evaluated as to whether insured could have prevented them with reasonable caution
        <br />
        c. Insured's cooperation with suggestions to reduce future losses
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnPLOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>