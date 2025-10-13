<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCommercialUWQuestionList.ascx.vb" Inherits="IFM.VR.Web.ctlCommercialUWQuestionList" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctlCommercialUWQuestionItem.ascx" TagPrefix="uc1" TagName="ctlCommercialUWQuestionItem" %>

<!-- #region javascript -->

<script type="text/javascript">

    // GloabalFunctions
    function CheckConstants() {
        $('.neverShow').hide();
        $('.alwaysShow').show();
    };

    // This function will show or hide a description (additional info) element
    function ShowOrHideAdditionalInputPanel(diamondcode, ShowOrHide) {
        if (ShowOrHide == "SHOW") {
            if (!$('.DescriptionTable.' + diamondcode).hasClass("neverShow")) {
                $('.DescriptionTable.' + diamondcode).show();
            }
        }
        else {
            if (!$('.DescriptionTable.' + diamondcode).hasClass("alwaysShow")) {
                $('.DescriptionTable.' + diamondcode).hide();
                $('.DescriptionTable.' + diamondcode + ' textarea').val('')
                $('.DescriptionTable.' + diamondcode + ' textarea').attr('style', 'border: none;');
                $('.DescriptionTable.' + diamondcode + ' span').text('Additional Information')
                $('.DescriptionTable.' + diamondcode + ' span').css("color", "black");
            }
        }
        CheckConstants();
    }

    // This function will select radio buttons based off diamondcode
    // selection: "rbYes", "rbNo", "none"
    function selectRadioButton(diamondcode, selection) {
        $('.questionTable.' + diamondcode).each(function () {
            rbNo = $(this).find('input[value="rbNo"]')
            rbYes = $(this).find('input[value="rbYes"]')
            //rbNo.prop('checked', false);
            //rbYes.prop('checked', false);
            rbNo.prop('checked', false);
            rbYes.prop('checked', false);
            if (selection == 'rbYes') {
                rbYes.prop('checked', true);
                ShowOrHideAdditionalInputPanel(diamondcode, "SHOW")
            }
            else if (selection == 'rbNo') {
                rbNo.prop('checked', true);
                ShowOrHideAdditionalInputPanel(diamondcode, "HIDE")
            }
            else {
                ShowOrHideAdditionalInputPanel(diamondcode, "HIDE")

            }
            CheckConstants()
        });
    }

    // This function will return a radio button's selected value based off diamondcode
    function whatIsRadioButtonSelection(diamondcode) {
        selection = "none";
        $('.questionTable.' + diamondcode).each(function () {
            rbNo = $(this).find('input[value="rbNo"]');
            rbYes = $(this).find('input[value="rbYes"]');
            if (rbYes.prop('checked')) {
                selection = "yes";
                return false;
            }
            else if (rbNo.prop('checked')) {
                selection = "no";
                return false;
            }
            else {
                selection = "none"
                return false;
            }
        });
        return selection
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


    // This function checks that all required Additional Info textboxes have values.
    // Draws a red border around any required textboxes without a value.
    // Changes the text of the description label to 'Additional Information Response Required' if a required value is not present.
    // Returns true if all are answered, false if not
    // Opens collapsed accordions if they contain errors
    function AllOpenTextboxesHaveValues() {
        var allHaveAnAnswers = true;
        var maxLength = 125;
        $('.questionTable').each(function () {
            var addInfo = $($(this).next('.DescriptionTable').first())
            var textArea = $(addInfo.find('textarea').first())
            var spanArea = $(addInfo.find('span').first())
            var accordArea = $($(this).closest('.ui-accordion'))
            if (($(this).find('input').eq(1).is(':checked') || addInfo.hasClass("alwaysShow")) && !addInfo.hasClass("neverShow")) {
                if (textArea.val() == '') {
                    allHaveAnAnswers = false;
                    textArea.attr('style', 'border: 1px solid red;');
                    spanArea.text("Additional Information Response Required");
                    spanArea.css("color", "red");
                    accordArea.accordion('option', 'active', 0);
                }
                else if (!CheckMaxTextNoDisable(textArea, maxLength)) {
                    allHaveAnAnswers = false;
                    accordArea.accordion('option', 'active', 0);
                }
                else {
                    textArea.attr('style', 'border: none;');
                    spanArea.css("color", "black");
                    spanArea.text("Additional Information");
                }

            };
            //CheckMaxTextNoDisable(textArea, maxLength)

        });

        //alert('All Required Textboxes Have Answers: ' + allHaveAnAnswers);
        return allHaveAnAnswers;
    }

    /// Form validation
    /// Called from the SUBMIT button OnClientClick event
    function ValidateForm() {
        var a = AllQuestionsAreAnswered();
        var b = AllOpenTextboxesHaveValues();
        return (a && b);
    }

    /// This function checks to see that all underwriting questions have been answered.
    /// Any unanswered question will display a red asterisk next to the radio buttons.
    /// Returns True if all questions have been answered, False if not.
    // Opens collapsed accordions if they contain errors
    function AllQuestionsAreAnswered() {
        var allAnswered = true;
        $('.questionTable.required').each(function () {
            var accordArea = $($(this).closest('.ui-accordion'))
            //alert('no: ' + $($(this).find('input')[0]).is(':checked') + ' yes: ' + $($(this).find('input')[1]).is(':checked'));
            if ($($(this).find('input')[0]).is(':checked') || $($(this).find('input')[1]).is(':checked')) {
                // atleast one is selected - Normal formatting, no asterisk next to radio buttons
                $(this).find('.requiredLabelError').hide()
            }
            else {
                // neither is selected - put an asterisk next to the radio buttons
                //martin();                    
                allAnswered = false;
                accordArea.accordion('option', 'active', 0);
                $(this).find('.requiredLabelError').show()
            }
        });

        // Set the color of the 'All Questions Must Be Answered' label
        if (allAnswered) {
            $('.QuestionsAnsweredError').hide();
        }
        else {
            $('.QuestionsAnsweredError').show();
        }

        //alert('All Questions are Answered: ' + allAnswered);
        return allAnswered;
    }

    // LOB = "BOP" question handling for special cases/events.
    function HandleRadioButtonClicksBOP(diamondcode, selection) {
        // Handle specific code by diamondcode
        if (selection == 'rbYes') {
            // Process if yes is selected
            switch (diamondcode) {
                case 9008:
                    // Question 7 is a special case - Show a Confirmation, deal with the result
                    CheckForAddlInput = false;
                    ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                    if (ans == true) {
                        // OK - Archive and navigate back to MyVelocirater
                        ArchiveQuote();
                    }
                    else {
                        // CANCEL - Clear the checkboxes
                        selectRadioButton(diamondcode, "rbNo")
                    }
                    break;

                default:
                    // The rest of the questions DO require a warning message -- No Message for Commercial
                    //alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                    break;
            }
        }
        else {
            // Process if no is selected
            switch (diamondcode) {
                //case 5:
                //case 6:
                //    break;
                default:
                    break;
            }
        }
    }

    // LOB = "CAP" question handling for special cases/events.
    function HandleRadioButtonClicksCAP(diamondcode, selection) {
        // Handle specific code by diamondcode
        if (selection == 'rbYes') {
            // Process if yes is selected
            switch (diamondcode) {
                //    break;
                case 9400:
                    // Question 7 is a special case - Show a Confirmation, deal with the result
                    CheckForAddlInput = false;
                    ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                    if (ans == true) {
                        // OK - Archive and navigate back to MyVelocirater
                        ArchiveQuote();
                    }
                    else {
                        // CANCEL - Clear the checkboxes
                        selectRadioButton(diamondcode, "rbNo")
                    }
                    break;

                default:
                    // The rest of the questions DO require a warning message -- No Message for Commercial
                    //alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                    break;
            }
        }
        else {
            // Process if no is selected
            switch (diamondcode) {
                //case 5:
                //case 6:
                //    break;
                default:
                    break;
            }
        }
    }

    // LOB = "WCP" question handling for special cases/events.
    function HandleRadioButtonClicksWCP(diamondcode, selection) {
        // Handle specific code by diamondcode
        if (selection == 'rbYes') {
            // Process if yes is selected
            switch (diamondcode) {
                case 9107:
                    // Question 7 is a special case - Show a Confirmation, deal with the result
                    CheckForAddlInput = false;
                    ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                    if (ans == true) {
                        // OK - Archive and navigate back to MyVelocirater
                        ArchiveQuote();
                    }
                    else {
                        // CANCEL - Clear the checkboxes
                        selectRadioButton(diamondcode, "rbNo")
                    }
                    break;

                default:
                    // The rest of the questions DO require a warning message -- No Message for Commercial
                    //alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                    break;
            }
        }
        else {
            // Process if no is selected
            switch (diamondcode) {
                //case 5:
                //case 6:
                //    break;
                default:
                    break;
            }
        }
    }

    // LOB = "CGL" question handling for special cases/events.
    function HandleRadioButtonClicksCGL(diamondcode, selection) {
        // Handle specific code by diamondcode
        if (selection == 'rbYes') {
            // Process if yes is selected
            switch (diamondcode) {
                case 9002:
                    var copyToDiamondCode = 9255;
                    // check if 9255 has no selection -- Update 01/11/2018 Force the change and disable 9255
                    //if (whatIsRadioButtonSelection(copyToDiamondCode) == "none") {
                        selectRadioButton(copyToDiamondCode, "rbYes")
                    //}
                default:
                    break;
            }
        }
        else {
            // Process if no is selected
            switch (diamondcode) {
                case 9002:
                    var copyToDiamondCode = 9255;
                    // if 9255 has no selection -- Update 01/11/2018 Force the change and disable 9255
                    //if (whatIsRadioButtonSelection(copyToDiamondCode) == "none") {
                        selectRadioButton(copyToDiamondCode, "rbNo")
                    //}
                default: ;
                    break;
            }
        }
    }

    // LOB = "CGL" question handling for special cases/events.
    function HandleTextareaChangeCGL(diamondcode) {
        switch (diamondcode) {
            case 9002:
                var copyToDiamondCode = 9255;
                // check; if text area empty then default 9255 to 9002 -- Update 01/11/2018 Force the change and disable 9255
                //if ($.trim($('.DescriptionTable.' + copyToDiamondCode).find('textarea').val()).length < 1) {
                $('.DescriptionTable.' + copyToDiamondCode).find('textarea').val($('.DescriptionTable.' + diamondcode).find('textarea').val())
            //}
            default:
                break;
        }
    }

    // LOB = "CPR" question handling for special cases/events.
    function HandleRadioButtonClicksCPR(diamondcode, selection) {
        // Handle specific code by diamondcode
        if (selection == 'rbYes') {
            // Process if yes is selected
            switch (diamondcode) {
                case 9008:
                    // Question 7 is a special case - Show a Confirmation, deal with the result
                    CheckForAddlInput = false;
                    ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                    if (ans == true) {
                        // OK - Archive and navigate back to MyVelocirater
                        ArchiveQuote();
                    }
                    else {
                        // CANCEL - Clear the checkboxes
                        selectRadioButton(diamondcode, "rbNo")
                    }
                    break;

                default:
                    // The rest of the questions DO require a warning message -- No Message for Commercial
                    //alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                    break;
            }
        }
        else {
            // Process if no is selected
            switch (diamondcode) {
                //case 5:
                //case 6:
                //    break;
                default:
                    break;
            }
        }
    }

    // For Disabling question items by diamondcode
    function DisableItemElements(diamondcode) {
        $('.questionTable.' + diamondcode).each(function () {
            rbNo = $(this).find('input[value="rbNo"]');
            rbYes = $(this).find('input[value="rbYes"]');
            rbNo.prop('disabled', true);
            rbYes.prop('disabled', true);
        });

        $('.DescriptionTable.' + diamondcode + ' textarea').prop('disabled', true);
    }


    // Initial Page load set radio's if selected, show description if selected, set always/never description conditions.
    $(function () {

        //Check for items that should always/never display
        CheckConstants();

        // Select radio button based off selected choice
        $('.questionTable input:radio').each(function () {
            if ($(this).closest("span").data('selection') === "selected") {
                var code = $(this).closest("table").data('diamondcode')
                selectRadioButton(code, $(this).attr('value'))
            }

        });

        $(".questionTable:odd, .DescriptionTable:odd").css("background-color", "#dddddd");

        // Perform actions on on Page Ready by LOB
        switch (ctlCommercialUWQuestionList_LOB) {
            case "CGL":
                DisableItemElements(9255);
                break;

            default:
                break;
        }

    });

    // Live functionality: Get Radio selection; call to LOB specific settings
    $(function () {

        // Radio button Selection
        $('.questionTable input:radio').on('click', function () {
            var selection = $(this).attr('value');
            var code = $(this).closest("table").data('diamondcode')
            //alert('Worked! Type:' + $(this).attr('type') + '- Value: ' + selection + '- Code: ' + code);
            if (selection == 'rbYes') {
                ShowOrHideAdditionalInputPanel(code, 'SHOW')
            }
            else {
                ShowOrHideAdditionalInputPanel(code, 'HIDE')
            }
            // Check for Additional logic/processing
            switch (ctlCommercialUWQuestionList_LOB) {
                case "BOP":
                    HandleRadioButtonClicksBOP(code, selection);
                    break;
                case "CAP":
                    HandleRadioButtonClicksCAP(code, selection);
                    break;
                case "WCP":
                    HandleRadioButtonClicksWCP(code, selection);
                    break;
                case "CGL":
                    HandleRadioButtonClicksCGL(code, selection);
                    break;
                case "CPR":
                    HandleRadioButtonClicksCPR(code, selection);
                    break;
                default:
                    break;
            }

        });

        // TextArea change
        $('.DescriptionTable textarea').on('change', function () {
            var code = $(this).closest("table").data('diamondcode')
            // Check for Additional logic/processing
            switch (ctlCommercialUWQuestionList_LOB) {
                case "CGL":
                    HandleTextareaChangeCGL(code);
                    break;

                default:
                    break;
            }

        });


        //($($(this).find('input')[1]).is(':checked') || $($(this).find('input')[0]).is(':checked'))

    });


    function ArchiveQuote() {
        var result = false
        var qid = "<% = QuoteId%>";
        var genHandler = 'GenHandlers/UWHandler.ashx?quoteId=' + encodeURIComponent(qid)

        //alert('1');

        $.getJSON(genHandler, {
            dataType: "json",
            data: "",
            format: "json",
            async: false
        })
            .fail(function (jqxhr, textStatus, error) {
                //alert('2');
                alert("Call failed: " + textStatus + ", " + error);
            })
            .done(function (data) {
                //alert('3');
                result = data;
                if (result == null || result == '') {
                    // no data
                    alert('Unable to change Quote Status, please alert IT');
                }
                window.location = "MyVelocirater.aspx";
                return result;
            });
        //alert('4');
    }
</script>

<!-- #endregion -->

<header>
    <style type="text/css">
        table#UWQuestionsDiv, table.questionTable, table.DescriptionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }


        .TableLeftColumn {
            width: 500px;
        }

        .TableRightColumn {
            width: 132px;
        }

        .TableAddlInfoLabelColumn {
            width: 100px;
            vertical-align: top;
            text-align: right;
        }

        .TableDescriptionColumn {
            width: 100%;
        }

        .DescriptionTextBox {
            width: 450px;
            height: 50px;
        }

        .RadioButtonFormat {
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            border-radius: 4px;
            height: 20px;
        }

        .ErrorBorder {
            border: solid;
            border-width: 1px;
            border-color: red;
        }

        .NormalBorder {
            border: none;
        }

        .DescriptionTable td {
            padding: 0 4px;
        }

        .trHeaderRow {
            height:30px;
            vertical-align:central;
            background-color:white;
            font-weight: 700;
        }

        .UWQuestionsSection {
            margin: auto 30px;
        }

        .questionTable td {
            padding: 4px 4px;
        }
    </style>
</header>

<div id="UWQuestionsDiv">
    <h3>Underwriting Questions</h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td valign="top">
                <div align="center">
                    <asp:Label ID="lblAnswerAll" runat="server" Text="All required questions must be answered." Font-Bold="true" Font-Size="14px" ClientIDMode="Static" ForeColor="Red" Style="display: none" CssClass="QuestionsAnsweredError"></asp:Label>
                    <br />
                    <br />
                </div>
                <%--Repeat for each section--%>

                <asp:Repeater ID="rptUWQ" runat="server">
                    <ItemTemplate>
                        <section class="UWQuestionsSection" style="margin: auto 30px;">
                            <h3>
                                <asp:Label ID="lblAccordHeader" runat="server" Text="Section"></asp:Label>
                            </h3>
                            <div style="border-collapse: collapse; width: 100%; display: table;">
                                <%--Add each section's questions--%>
                                <uc1:ctlCommercialUWQuestionItem runat="server" ID="ctlCommercialUWQuestionItem" />
                            </div>
                        </section>
                    </ItemTemplate>
                </asp:Repeater>

                <br />
                <div align="center">
                    <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="StandardSaveButton" OnClientClick="return ValidateForm()" Width="150px" TabIndex="500" />
                    <asp:Button ID="btnGoToApp" runat="server" Text="Application" CssClass="StandardSaveButton" OnClientClick="return ValidateForm()" Width="150px" TabIndex="501" />
                </div>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>

</div>

