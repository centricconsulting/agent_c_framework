<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCommercial_IRPM.ascx.vb" Inherits="IFM.VR.Web.ctlCommercial_IRPM" %>

<%--<!-- #region javascript -->

<script type="text/javascript">

    // GloabalFunctions
    function CheckConstants() {
        $('.neverShow').hide();
        $('.alwaysShow').show();
    };

    // This function will show or hide a description (additional info) element based on the Riskbox element
    function ShowOrHideAdditionalInputPanel(element, ShowOrHide) {
        var AddInfo = $(element).closest('.ItemGroup').find('.DescriptionTable');

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

    //UW Questions Description Box max character check
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
    
    // Calculate the totals of the Risks, Warn if over
    function CalcTotals() {
        var minTotal = parseInt($('.TotalTextBox').first().data('totalmin'));
        var maxTotal = parseInt($('.TotalTextBox').first().data('totalmax'));
        var calcdTotal = 0

        $('.RiskTextBox').each(function () {
            var riskValue = parseInt($(this).val());

            if (isNaN(riskValue)) {
                riskValue = 0;
            }

            calcdTotal = calcdTotal + riskValue;
        });

        $('.TotalTextBox').val(calcdTotal);

        if (calcdTotal < minTotal || calcdTotal > maxTotal) {
            //alert("Total IRPM must be between " + minTotal + " and " + maxTotal);  //removed 10/8/2020 w/ Interoperability project
            $('.TotalTextBox').addClass("ErrorBorder");
            //return false
            //updated 10/8/2020 w/ Interoperability project
            return true;
        }
        else {
            $('.TotalTextBox').removeClass("ErrorBorder");
            return true; //added semi-colon 10/8/2020
        }

    }

    // Clears the risk values (sets them to zero)
    function ClearForm() {
        $('.RiskTextBox').each(function () {
            $(this).val(0);
            // $(this).change();
            ShowOrHideAdditionalInputPanel($(this), 'HIDE')
        });
        CalcTotals();
        AllOpenTextboxesHaveValues();
        return false;
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
            var addInfo = $(this).closest('.ItemGroup').find('.DescriptionTable');
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
        //var a = AllQuestionsAreAnswered();
        var a = CalcTotals();
        var b = AllOpenTextboxesHaveValues();
        return (a && b);
    }

    // Initial Page load.
    $(function () {

        //Check for items that should always/never display
        CheckConstants();

        // Select additional Info based off item value
        $('.RiskTextBox').each(function () {
            var riskValue = parseInt($(this).val());

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

        // Calc Total changes
        CalcTotals();

        // Alternat color backgrounds for table items.
        $(".questionTable:odd, .DescriptionTable:odd").css("background-color", "#dddddd");
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
            var riskValue = parseInt(element.val());
            var minValue = parseInt(element.data('min'));
            var maxValue = parseInt(element.data('max'));

            if (isNaN(riskValue))
            {
                element.val(0);
                ShowOrHideAdditionalInputPanel(element, 'HIDE')
                AllOpenTextboxesHaveValues()
                // Calc Total changes
                CalcTotals();
                return
            }

            if (riskValue < minValue || riskValue > maxValue) {
                element.val(0);
                alert("Number must be a whole number between " + minValue + " and " + maxValue);
                element.focus();
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
                    element.closest('.ItemGroup').find('.DescriptionTextBox').focus();
                }
            }

            // this will re-set the values and remove any leading zeros
            element.val(riskValue)

            var result = CalcTotals();
            isTabbed = false
        };

    });

       
</script>

<!-- #endregion -->--%>

<header>
    <style type="text/css">

        .IRPMDiv {
            clear: both;
        }

        .IRPMDiv, table.questionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        .IRPMDiv .DescriptionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
            width: 100%
        }


            .IRPMDiv .TableLeftColumn {
                width: 400px;
                padding-left: 4px;
            }

            .IRPMDiv .TableRightColumn {
                width: 200px;
            }

            .IRPMDiv .TableAddlInfoLabelColumn {
                width: 100px;
                vertical-align: top;
                text-align: right;
            }

            .IRPMDiv .TableDescriptionColumn {
                width: 100%;
            }

            .IRPMDiv table.DescriptionTable.DescriptionTextBox {
                width: 450px;
                height: 50px;
            }

            .IRPMDiv .RiskTextBox {
                width: 2.5em;
                margin-left: 10px;
                text-align: center;
            }

            .IRPMDiv .RiskTextBoxDual {
                width:2.5em;
                margin-left: 10px;
                text-align: center;
            }

            .IRPMDiv .TotalTextBox {
                width: 2.5em;
                margin-left: 10px;
                text-align: center;
            }

            .IRPMDiv .TotalTextBoxDual {
                width: 2.5em;
                margin-left: 10px;
                text-align: center;
            }


            .IRPMDiv table.questionTable.RiskRange {
                width: 100px;
            }

            .IRPMDiv .ErrorBorder {
                border: solid;
                border-width: 1px;
                border-color: red;
                color: red
            }

            .IRPMDiv .NormalBorder {
                border: none;
            }

            .IRPMDiv .DescriptionTable td {
                padding: 0 4px;
            }

            .IRPMDiv .IRPMSection {
                margin: auto 30px;
            }

            .IRPMDiv td {
                /*padding: 4px 4px;*/
            }

            .IRPMDiv .RiskDetails {
                float: right;
            }

            .IRPMDiv .TotalDetails {
                float: right;
            }

            .IRPMDiv .Note {
                margin: auto 60px;
                text-align: center;
                width: 400px;
            }
    </style>
</header>

<div id="IRPMDiv" class="IRPMDiv">  <%--Do Not RunAt="server" without code updates.--%>
    <h3>
        <asp:Label ID="lblTitle" runat="server" Text="IRPM Information (Credits/Debits)"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear IRPM Data" OnClientClick="return ClearForm()">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" runat="server" CssClass="RemovePanelLink" ToolTip="Save IRPM Data" OnClientClick="return ValidateForm();">Save</asp:LinkButton>  
        </span>
    </h3>
    <div id="sectionMain" runat="server">
        <script type="text/javascript">

    // GloabalFunctions
    function CheckConstants() {
        $('.neverShow').hide();
        $('.alwaysShow').show();
    };

    // This function will show or hide a description (additional info) element based on the Riskbox element
    function ShowOrHideAdditionalInputPanel(element, ShowOrHide) {
        var AddInfo = $(element).closest('.ItemGroup').find('.DescriptionTable');

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

    //UW Questions Description Box max character check
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
    
    // Calculate the totals of the Risks, Warn if over
    function CalcTotals() {
        var minTotal = parseInt($('.TotalTextBox').first().data('totalmin'));
        var maxTotal = parseInt($('.TotalTextBox').first().data('totalmax'));
        var calcdTotal = 0

        $('.RiskTextBox').each(function () {
            var riskValue = parseInt($(this).val());

            if (isNaN(riskValue)) {
                riskValue = 0;
            }

            calcdTotal = calcdTotal + riskValue;
        });

        $('.TotalTextBox').val(calcdTotal);

        if (calcdTotal < minTotal || calcdTotal > maxTotal) {
            //alert("Total IRPM must be between " + minTotal + " and " + maxTotal);  //removed 10/8/2020 w/ Interoperability project
            $('.TotalTextBox').addClass("ErrorBorder");
            //return false
            //updated 10/8/2020 w/ Interoperability project
            return true;
        }
        else {
            $('.TotalTextBox').removeClass("ErrorBorder");
            return true; //added semi-colon 10/8/2020
        }

    }

    // Clears the risk values (sets them to zero)
    function ClearForm() {
        $('.RiskTextBox').each(function () {
            $(this).val(0);
            // $(this).change();
            ShowOrHideAdditionalInputPanel($(this), 'HIDE')
        });
        CalcTotals();
        AllOpenTextboxesHaveValues();
        return false;
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
            var addInfo = $(this).closest('.ItemGroup').find('.DescriptionTable');
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
        //var a = AllQuestionsAreAnswered();
        var a = CalcTotals();
        var b = AllOpenTextboxesHaveValues();
        return (a && b);
    }

    // Initial Page load.
    $(function () {

        //Check for items that should always/never display
        CheckConstants();

        // Select additional Info based off item value
        $('.RiskTextBox').each(function () {
            var riskValue = parseInt($(this).val());

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

        // Calc Total changes
        CalcTotals();

        // Alternat color backgrounds for table items.
        $(".questionTable:odd, .DescriptionTable:odd").css("background-color", "#dddddd");
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
            var riskValue = parseInt(element.val());
            var minValue = parseInt(element.data('min'));
            var maxValue = parseInt(element.data('max'));

            if (isNaN(riskValue))
            {
                element.val(0);
                ShowOrHideAdditionalInputPanel(element, 'HIDE')
                AllOpenTextboxesHaveValues()
                // Calc Total changes
                CalcTotals();
                return
            }

            if (riskValue < minValue || riskValue > maxValue) {
                element.val(0);
                alert("Number must be a whole number between " + minValue + " and " + maxValue);
                element.focus();
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
                    element.closest('.ItemGroup').find('.DescriptionTextBox').focus();
                }
            }

            // this will re-set the values and remove any leading zeros
            element.val(riskValue)

            var result = CalcTotals();
            isTabbed = false
        };

    });

       
        </script>
        <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td valign="top">
                <div align="center">
                    <asp:Label ID="Label7" runat="server" Text="All required information must be answered." Font-Bold="true" Font-Size="14px" ClientIDMode="Static" ForeColor="Red" Style="display: none" CssClass="QuestionsAnsweredError"></asp:Label>
                    <br />
                    <br />
                </div>
                <%--Repeat for each item--%>

                <section class="IRPMSection">
                    <h3>
                        <asp:Label ID="Label8" runat="server" Text="Description"></asp:Label>
                    </h3>
                    <div style="border-collapse: collapse; width: 100%; display: table;">
                        <asp:Repeater ID="rptIRPM" runat="server">
                            <ItemTemplate>
                                <asp:panel id="ItemGroup" runat="server" CssClass="ItemGroup"  data-riskCharacteristicTypeId='<%# DataBinder.Eval(Container.DataItem, "RiskCharacteristicTypeId")%>' data-scheduleRatingTypeId='<%# DataBinder.Eval(Container.DataItem, "ScheduleRatingTypeID")%>' data-lob='<%#IFM.VR.Common.Helpers.LOBHelper.GetAbbreviatedLOBPrefix(Me.Quote.LobType)%>'>
                                    <table id="tblIRPM" runat="server" class='questionTable'>
                                        <tr>
                                            <td class="TableLeftColumn">
                                                <asp:Label ID="lblQuestionText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label>
                                            </td>
                                            <td class="TableRightColumn">
                                                <table id="tblRisks" runat="server"  class="RiskDetails">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblAsterisk" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px" Style="display: none" CssClass="requiredLabelError"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblRiskRange" runat="server" Style="" CssClass="RiskRange" Text='<%# "min " + DataBinder.Eval(Container.DataItem, "Minimum") + "% / max " + DataBinder.Eval(Container.DataItem, "Maximum") + "%" %>'></asp:Label>
                                                        </td>
                                                        <td class="RiskBox">
                                                            <asp:TextBox type="number" ID="txtRisk" runat="server" CssClass="RiskTextBox" MaxLength="3" OnKeyUp="" Text='<%# DiamondToVRConversion(DataBinder.Eval(Container.DataItem, "RiskFactor")) %>' data-min='<%# DataBinder.Eval(Container.DataItem, "Minimum") %>' data-max='<%# DataBinder.Eval(Container.DataItem, "Maximum") %>'></asp:TextBox>
                                                        </td>
                                                    </tr>

                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="tblIRPMDesc" runat="server" class="DescriptionTable" style="display: none;">
                                    <tr style="padding: auto 5px;">
                                        <td class="TableDescriptionColumn">
                                            <br />
                                            <asp:Label ID="lblIRPMDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                                            <asp:TextBox ID="txtIRPMDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);" Text='<%# DataBinder.Eval(Container.DataItem, "Remark")%>'></asp:TextBox>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                                </asp:panel>
                            </ItemTemplate>
                        </asp:Repeater>
                        <table id="Table5" runat="server" class='questionTable'>
                            <tr>
                                <td class="TableLeftColumn">
                                    <asp:Label ID="Label9" runat="server" Text='Total'></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table6" runat="server" class="TotalDetails">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label10" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px" Style="display: none" CssClass="requiredLabelError"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label11" runat="server" Style="" CssClass="TotalRange" Text='<%# "min " + GetTotalMin() + "% / max " + GetTotalMax() + "%" %>'></asp:Label>
                                            </td>
                                            <td class="RiskBox">
                                                <asp:TextBox  disabled="disabled" ID="TextBox2" runat="server" CssClass="TotalTextBox" OnKeyUp="" Text='0' data-totalmin='<%# GetTotalMin()  %>' data-totalmax='<%# GetTotalMax()  %>'></asp:TextBox>
                                            </td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </section>
                <br />
                    <asp:Panel runat="server" CssClass="Note">Please note, the total credit cannot exceed <%#"min " + GetTotalMin() + "% / max " + GetTotalMax() %>%, total credits greater than <%# GetTotalMax() %>% will require Underwriting approval.</asp:Panel>
                <br />
                <div align="center">
                    
                    <asp:Button ID="btnSubmitRate" runat="server" Text="Save and Re-Rate" CssClass="StandardSaveButton" OnClientClick="return ValidateForm();" Width="150px" TabIndex="500" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="StandardSaveButton" OnClientClick="" Width="150px" TabIndex="501" />
                    <input type="button" id="Button6" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" /><%--added 12/3/2020 (Interoperability)--%>
                </div>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
    </div>

    <div id="sectionMainDual" runat="server" style="width: 550px;">
    <script type="text/javascript">

    // GloabalFunctions
    function CheckConstants() {
        $('.neverShow').hide();
        $('.alwaysShow').show();
    };

    // This function will show or hide a description (additional info) element based on the Riskbox element
    function ShowOrHideAdditionalInputPanel(element, ShowOrHide) {
        var AddInfo = $(element).closest('.ItemGroup').find('.DescriptionTable');

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

    //UW Questions Description Box max character check
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
    
    // Calculate the totals of the Risks, Warn if over
    function CalcTotals() {
        var minTotal = parseInt($('.TotalTextBox').first().data('totalmin'));
        var maxTotal = parseInt($('.TotalTextBox').first().data('totalmax'));
        var calcdTotal = 0
        var calcdTotalDual = 0

        $('.RiskTextBox').each(function () {
            var riskValue = + parseFloat($(this).val()).toFixed(1);

            if (isNaN(riskValue)) {
                riskValue = 0;
            }

            calcdTotal = calcdTotal + riskValue;
        });

        $('.RiskTextBoxDual').each(function () {
            var riskValue = + parseFloat($(this).val()).toFixed(1);

            if (isNaN(riskValue)) {
                riskValue = 0;
            }

            calcdTotalDual = calcdTotalDual + riskValue;
        });

        $('.TotalTextBox').val(calcdTotal);
        $('.TotalTextBoxDual').val(calcdTotalDual);

        if (calcdTotal < minTotal || calcdTotal > maxTotal) {
            //alert("Total IRPM must be between " + minTotal + " and " + maxTotal);  //removed 10/8/2020 w/ Interoperability project
            $('.TotalTextBox').addClass("ErrorBorder");
            //return false
            //updated 10/8/2020 w/ Interoperability project
            //return true;
        }
        else {
            $('.TotalTextBox').removeClass("ErrorBorder");
            //return true; //added semi-colon 10/8/2020
        }

        if (calcdTotalDual < minTotal || calcdTotalDual > maxTotal) {
            //alert("Total IRPM must be between " + minTotal + " and " + maxTotal);  //removed 10/8/2020 w/ Interoperability project
            $('.TotalTextBoxDual').addClass("ErrorBorder");
            //return false
            //updated 10/8/2020 w/ Interoperability project
            //return true;
        }
        else {
            $('.TotalTextBoxDual').removeClass("ErrorBorder");
            //return true; //added semi-colon 10/8/2020
        }
        return true;

    }

    // Clears the risk values (sets them to zero)
    function ClearForm() {
        $('.RiskTextBox').each(function () {
            $(this).val(0);
            // $(this).change();
            ShowOrHideAdditionalInputPanel($(this), 'HIDE')
        });
        $('.RiskTextBoxDual').each(function () {
            $(this).val(0);
            // $(this).change();
            ShowOrHideAdditionalInputPanel($(this), 'HIDE')
        });
        CalcTotals();
        AllOpenTextboxesHaveValues();
        return false;
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
            var addInfo = $(this).closest('.ItemGroup').find('.DescriptionTable');
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
        //var a = AllQuestionsAreAnswered();
        var a = CalcTotals();
        var b = AllOpenTextboxesHaveValues();
        return (a && b);
    }

    // Initial Page load.
    $(function () {

        //Check for items that should always/never display
        CheckConstants();

        // Select additional Info based off item value
        $('.RiskTextBox').each(function () {
            var RtbDual = $(this).closest($('.RiskDetails')).first().find($('.RiskTextBoxDual')).first()
            var riskValue = + parseFloat($(this).val());
            var riskValueDual = + parseFloat(RtbDual.val());

            if (isNaN(riskValue)) {
                riskValue.val(0);
            }

            if (isNaN(riskValueDual)) {
                riskValueDual.val(0);
            }

            if (riskValue == 0 && riskValueDual == 0) {
                ShowOrHideAdditionalInputPanel($(this), 'HIDE')
            }
            else {
                ShowOrHideAdditionalInputPanel($(this), 'SHOW')
            }

        });

        $('.RiskTextBoxDual').each(function () {
            ifm.vr.ui.ElementDisabler($(this));
        });

        // Calc Total changes
        CalcTotals();

        // Alternat color backgrounds for table items.
        $(".questionTable:odd, .DescriptionTable:odd").css("background-color", "#dddddd");
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

            var RtbDual = $(this).closest($('.RiskDetails')).first().find($('.RiskTextBoxDual')).first()
            var rounded = + parseFloat($(this).val()).toFixed(1)
            RtbDual.val(rounded)

            TextBoxControl($(this))
            
        });

        function TextBoxControl(element) {
            var riskValue = + parseFloat(element.val()).toFixed(1);
            var minValue = + parseFloat(element.data('min')).toFixed(1);
            var maxValue = + parseFloat(element.data('max')).toFixed(1);

            if (isNaN(riskValue))
            {
                element.val(0);
                ShowOrHideAdditionalInputPanel(element, 'HIDE')
                AllOpenTextboxesHaveValues()
                // Calc Total changes
                CalcTotals();
                return
            }

            if (riskValue < minValue || riskValue > maxValue) {
                element.val(0);
                alert("Number must be a whole number between " + minValue + " and " + maxValue);
                element.focus();
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
                    element.closest('.ItemGroup').find('.DescriptionTextBox').focus();
                }
            }

            // this will re-set the values and remove any leading zeros
            element.val(riskValue)

            var result = CalcTotals();
            isTabbed = false
        };

    });

       
    </script>
        <table id="tblMainDual" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td valign="top">
                <div align="center">
                    <asp:Label ID="Label1" runat="server" Text="All required information must be answered." Font-Bold="true" Font-Size="14px" ClientIDMode="Static" ForeColor="Red" Style="display: none" CssClass="QuestionsAnsweredError"></asp:Label>
                    <br />
                    <br />
                </div>
                <%--Repeat for each item--%>

                <section class="IRPMSection" style="margin: auto 20px;">
                    <h3>
                        <table style="width:100%;">
                            <tr>
                                <td style="width: 25%">
                                    <asp:Label ID="Label2" runat="server" Text="Description" Style="margin-right: 100px;"></asp:Label>
                                </td>
                                <%--<asp:Label ID="lblMinMax" runat="server" Text="Minimum/Maximum" style="width: 25%;margin-right: 50px;"></asp:Label>--%>
                                <%--<asp:Label ID="lblLiab" runat="server" Text="Liability" style="width: 25%;margin-right: 18px;"></asp:Label>--%>
                                <%--<asp:Label ID="lblPhysDamage" runat="server" Text="Physical Damage" style="width: 25%;"></asp:Label>--%>
                                <td style="width: 75%">
                                    <table id="Table2" runat="server" class="" style="width: 100%;">
                                        <tr>
                                            <td style="width: 25%;">
                                                <asp:Label ID="MinMaxLabel" runat="server" Style="" CssClass="" Text="Minimum/Maximum"></asp:Label>
                                            </td>
                                            <td class="RiskBox" style="width: 25%; text-align: center;">
                                                <asp:Label ID="FirstLabel" runat="server" CssClass="" OnKeyUp="" Text='Liability'></asp:Label>
                                            </td>
                                            <td class="RiskBoxPD" style="width: 25%; text-align: center;">
                                                <asp:Label ID="SecondLabel" runat="server" CssClass="" OnKeyUp="" Text='Physical Damage'></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </h3>
                    <div style="border-collapse: collapse; width: 100%; display: table;">
                        <asp:Repeater ID="rptIrpmDual" runat="server">
                            <ItemTemplate>
                                <asp:panel id="ItemGroup" runat="server" CssClass="ItemGroup"  data-riskCharacteristicTypeId='<%# DataBinder.Eval(Container.DataItem, "RiskCharacteristicTypeId")%>' data-scheduleRatingTypeId='<%# DataBinder.Eval(Container.DataItem, "ScheduleRatingTypeID")%>' data-lob='<%#IFM.VR.Common.Helpers.LOBHelper.GetAbbreviatedLOBPrefix(Me.Quote.LobType)%>'>
                                    <table id="tblIRPM" runat="server" class='questionTable' style="width:100%;">
                                        <tr>
                                            <td class="" style="width: 150px;">
                                                <asp:Label ID="lblQuestionText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>' style="width:25%;"></asp:Label>
                                                <asp:HiddenField ID="hdnRiskCharacteristicTypeId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RiskCharacteristicTypeId")%>' />
                                            </td>
                                            <td class="">
                                                <table id="tblRisks" runat="server"  class="RiskDetails">
                                                    <tr>
                                                        <td style="width: 5%;text-align: center;">
                                                            <asp:Label ID="lblAsterisk" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px" Style="display: none;width:10%;" CssClass="requiredLabelError"></asp:Label>
                                                        </td>
                                                        <td style="width: 20%;">
                                                            <asp:Label ID="lblRiskRange" runat="server" Style="text-align: left;" CssClass="RiskRange" Text='<%# "min " + DataBinder.Eval(Container.DataItem, "Minimum") + "% / max " + DataBinder.Eval(Container.DataItem, "Maximum") + "%" %>'></asp:Label>
                                                        </td>
                                                        <td class="RiskBox" style="width: 10%;text-align: center;">
                                                            <asp:TextBox type="number" step="0.1" ID="txtRisk" runat="server" Style="text-align: center;" CssClass="RiskTextBox" MaxLength="3" OnKeyUp="" Text='<%# DiamondToVRConversionDecimal(DataBinder.Eval(Container.DataItem, "RiskFactor")) %>' data-min='<%# DataBinder.Eval(Container.DataItem, "Minimum") %>' data-max='<%# DataBinder.Eval(Container.DataItem, "Maximum") %>'></asp:TextBox>
                                                        </td>
                                                        <td class="RiskBoxPD" style="width: 15%;text-align: center;">
                                                            <asp:TextBox type="number" step="0.1" ID="txtRiskDual" runat="server" Style="text-align: center;" CssClass="RiskTextBoxDual" MaxLength="3" OnKeyUp="" Text='<%# DiamondToVRConversionDecimal(DataBinder.Eval(Container.DataItem, "RightColumnValue")) %>' data-min='<%# DataBinder.Eval(Container.DataItem, "Minimum") %>' data-max='<%# DataBinder.Eval(Container.DataItem, "Maximum") %>'></asp:TextBox>
                                                        </td>
                                                    </tr>

                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="tblIRPMDesc" runat="server" class="DescriptionTable" style="display: none;">
                                    <tr style="padding: auto 5px;">
                                        <td class="TableDescriptionColumn">
                                            <br />
                                            <asp:Label ID="lblIRPMDescriptionTitle" runat="server" Text="Additional Information"></asp:Label><br />
                                            <asp:TextBox ID="txtIRPMDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="75" MaxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);" Text='<%# DataBinder.Eval(Container.DataItem, "Remark")%>'></asp:TextBox>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                                </asp:panel>
                            </ItemTemplate>
                        </asp:Repeater>
                        <table id="Table3" runat="server" class='questionTable' style="width:100%;">
                            <tr>
                                <td class="" style="width: 150px;">
                                    <asp:Label ID="Label3" runat="server" Text='Total' style="margin-right: 105px;"></asp:Label>
                                </td>
                                <td class="">
                                    <table id="Table4" runat="server" class="TotalDetails" style="width:100%;" >
                                        <tr>
                                            <td style="width: 5%;text-align: center;">
                                                <asp:Label ID="Label4" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px" Style="display: none" CssClass="requiredLabelError"></asp:Label>
                                            </td>
                                            <td style="width: 20%;">
                                                <asp:Label ID="Label5" runat="server" Style="" CssClass="TotalRange" Text='<%# "min " + GetTotalMin() + "% / max " + GetTotalMax() + "%" %>'></asp:Label>
                                            </td>
                                            <td class="RiskBox" style="width: 10%;text-align: center;">
                                                <asp:TextBox  disabled="disabled" ID="TextBox1" runat="server" CssClass="TotalTextBox" OnKeyUp="" Text='0' data-totalmin='<%# GetTotalMin()  %>' data-totalmax='<%# GetTotalMax()  %>'></asp:TextBox>
                                            </td>
                                            <td class="RiskBoxPD"  style="width: 15%;text-align: center;">
                                                <asp:TextBox  disabled="disabled" ID="txtTotalRiskPD" runat="server" CssClass="TotalTextBoxDual" OnKeyUp="" Text='0' data-totalmin='<%# GetTotalMin()  %>' data-totalmax='<%# GetTotalMax()  %>'></asp:TextBox>
                                            </td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </section>
                <br />
                    <asp:Panel runat="server" CssClass="Note">Please note, the total credit cannot exceed <%#"min " + GetTotalMin() + "% / max " + GetTotalMax() %>%, total credits greater than <%# GetTotalMax() %>% will require Underwriting approval.</asp:Panel>
                <br />
                <div align="center">
                    
                    <asp:Button ID="btnSubmitRateDual" runat="server" Text="Save and Re-Rate" CssClass="StandardSaveButton" OnClientClick="return ValidateForm();" Width="150px" TabIndex="500" />
                    <asp:Button ID="btnCancelDual" runat="server" Text="Cancel" CssClass="StandardSaveButton" OnClientClick="" Width="150px" TabIndex="501" />
                    <input type="button" id="Button3" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" /><%--added 12/3/2020 (Interoperability)--%>
                </div>
                <div align="center">
                    <asp:Label ID="lblMsgDual" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
    </div>

</div>
