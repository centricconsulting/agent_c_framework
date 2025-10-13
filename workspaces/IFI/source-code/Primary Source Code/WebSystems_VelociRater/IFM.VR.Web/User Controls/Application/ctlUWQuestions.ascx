<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlUWQuestions.ascx.vb" Inherits="IFM.VR.Web.ctlUWQuestions" %>

<script type="text/javascript">

    // This function checks that all required Additional Info textboxes have values.
    // Draws a red border around any required textboxes without a value.
    // Changes the text of the description label to 'Additional Information Response Required' if a required value is not present.
    // Returns true if all are answered, false if not
    function AllOpenTextboxesHaveValues() {
        var allHaveAnAnswers = true;       
        var index = 0;
        var qNo = 0;
        var LOB = $('#<%=hdnLOB.ClientID%>').val();
            $('#UWQuestionsDiv').find('table').first().find('table').each(function () {
                //if (index % 1 == 0) {
                if ($($(this).find('input')[1]).is(':checked') || $($(this).find('input')[0]).is(':checked')) {
                    // yes OR No is selected
                    // Only check the VISIBLE textboxes
                    if ($(this).next().find('textarea').is(':visible')) {
                        if ($(this).next().find('textarea').first() != null) {
                            qNo += 1;                           
                            if ($(this).next().find('textarea').first().val() == '') {
                                allHaveAnAnswers = false;
                                $(this).next().find('textarea').first().attr('style', 'border: 1px solid red;')
                                $(this).next().find('span')[0].innerText = "Additional Information Response Required"
                                $(this).next().find('span').first().css("color", "red")
                            }
                            else {
                                $(this).next().find('textarea').first().attr('style', 'border: none;')
                                $(this).next().find('span').first().css("color", "black")
                                if ($(this).next().find('span')[0] != null) {
                                    if (LOB == "HOM" || LOB == "DFR") {
                                        $(this).next().find('span')[0].innerText = "Additional Information"
                                    }
                                    else {
                                        $(this).next().find('span')[0].innerText = "Question " + qNo + " Additional Information"
                                    }
                                }
                            }
                        }
                    }
                }
                //}
                index += 1;
            });
            //alert('All Required Textboxes Have Answers: ' + allHaveAnAnswers);
            return allHaveAnAnswers;
        }

        /// Form validation
        /// Called from the SUBMIT button OnClientClick event
        function ValidateForm() {
            var a = AllQuestionsAreAnswered();
            var b = AllOpenTextboxesHaveValues();
            //alert('All Answered: ' + a + '; All Textboxes have Values: ' + b);
            return (a && b);
        }

        /// This function checks to see that all underwriting questions have been answered.
        /// Any unanswered question will display a red asterisk next to the radio buttons.
        /// Returns True if all questions have been answered, False if not.
        function AllQuestionsAreAnswered() {
            var allAnswered = true;           
            $('#UWQuestionsDiv').find('table').first().find('table').find('table').each(function () {
                //alert(qno + ': no:' + $($(this).find('input')[0]).is(':checked') + ' yes: ' + $($(this).find('input')[1]).is(':checked'));
                if ($($(this).find('input')[0]).is(':checked') || $($(this).find('input')[1]).is(':checked')) {
                    // atleast one is selected - Normal formatting, no asterisk next to radio buttons
                    $(this).find('span')[0].innerText = " "
                }
                else {
                    // neither is selected - put an asterisk next to the radio buttons
                    //martin();                    
                    allAnswered = false;
                    $(this).find('span')[0].innerText = "*"
                }
            });

            // Set the color of the 'All Questions Must Be Answered' label
            if (allAnswered) {
                $('#lblAnswerAll').css('color', 'black');
            }
            else {
                $('#lblAnswerAll').css('color', 'red');
            }

            //alert('All Questions are Answered: ' + allAnswered);
            return allAnswered;
        }

        // This function is called whenever a radio button is clicked
        // Performs custom functionality for each LOB
        //function HandleRadioButtonClicks(LOB, QuestionIndex) {
        //    var QN = QuestionIndex + 1
        //    var allAnswered = true;
        //    var ndx = -1
        //    var ans = false
        //    var CheckForAddlInput = true

        //    $('#UWQuestionsDiv').find('table').first().find('table').find('table').each(function () {
        //        ndx += 1;
        //        if (ndx == QuestionIndex) {
        //            alert('ndx=' + ndx);
        //            if ($($(this).find('input')[1]).is(':checked')) {
        //                // YES IS CHECKED
        //                switch (LOB) {
        //                    case "HOM":
        //                        // ************************
        //                        // HOME PERSONAL
        //                        // ************************
        //                        // Deal with the Warning Dialogs
        //                        switch (QN) {
        //                            case 5:
        //                            case 6:
        //                            //case 9:
        //                            case 10:
        //                            case 13:
        //                            case 15:
        //                            case 16:
        //                            case 17:
        //                            case 21:
        //                                // Do Nothing, the above questions do not require a dialog
        //                                break;
        //                            case 1:
        //                            case 2:
        //                            case 3:
        //                            case 4:
        //                            case 7:
        //                            case 8:
        //                            case 11:
        //                            case 12:
        //                            case 18:
        //                            case 19:
        //                            case 20:
        //                            case 22:
        //                            case 23:
        //                            case 24:
        //                            case 25:
        //                            case 26:
        //                                // Show the default dialog
        //                                alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
        //                                break;
        //                            case 14:
        //                            case 9:
        //                                // Questions 9 & 14 are special cases - Show a Confirmation, deal with the result
        //                                CheckForAddlInput = false;
        //                                ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
        //                                if (ans == true) {
        //                                    // OK - Archive and navigate back to MyVelocirater
        //                                    //alert('Archive');
        //                                    ArchiveQuote()
        //                                }
        //                                else {
        //                                    //alert('Cancel');
        //                                    // CANCEL - Clear the checkboxes
        //                                    var NO = $(this).find('input')[0];
        //                                    var YES = $(this).find('input')[1];
        //                                    NO.checked = false;
        //                                    YES.checked = false;
        //                                }
        //                                break;
        //                            default:
        //                                alert("QN Not Found!!");
        //                                break;
        //                        }  // end of switch

        //                        // Determine whether or not to show the additional information pane
        //                        switch (QN) {
        //                            case 6:
        //                            case 9:
        //                            case 14:
        //                                // Questions 6, 9, and 14 do not require additional information
        //                                break;
        //                            default:
        //                                // The rest of the questions do require additonal information
        //                                if (CheckForAddlInput) { ShowOrHideAdditionalInputPanel(QuestionIndex, "SHOW"); }
        //                                break;
        //                        } // end of switch

        //                        break;
        //                    case "PPA":
        //                        // ************************
        //                        // PERSONAL AUTO
        //                        // ************************
        //                        if (LOB == "PPA") {
        //                            if ($($(this).find('input')[1]).is(':checked')) {
        //                                switch (QN) {
        //                                    case 5:
        //                                    case 6:
        //                                    case 8:
        //                                    case 9:
        //                                    case 13:
        //                                        // Do nothing, the above questions do not require a warning message
        //                                        break;
        //                                    case 16:
        //                                        // #16 does not require any additional input nor does it require a warning message
        //                                        //alert('#16');
        //                                        CheckForAddlInput = false;
        //                                        break;
        //                                    case 15:
        //                                        // Question 15 is a special case - Show a Confirmation, deal with the result
        //                                        CheckForAddlInput = false;
        //                                        ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
        //                                        if (ans == true) {
        //                                            // OK - Archive and navigate back to MyVelocirater
        //                                            ArchiveQuote();
        //                                        }
        //                                        else {
        //                                            // CANCEL - Clear the checkboxes
        //                                            var NO = $(this).find('input')[0];
        //                                            var YES = $(this).find('input')[1];
        //                                            NO.checked = false;
        //                                            YES.checked = false;
        //                                        }
        //                                        break;
        //                                    default:
        //                                        // The rest of the questions DO require a warning message
        //                                        alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
        //                                        break;
        //                                }
        //                                // show additional input box
        //                                if (CheckForAddlInput) {
        //                                    ShowOrHideAdditionalInputPanel(QuestionIndex, "SHOW");
        //                                }
        //                                else {
        //                                    //alert('hiding panel');
        //                                    ShowOrHideAdditionalInputPanel(QuestionIndex, "HIDE");
        //                                }
        //                            }
        //                            else {
        //                                if ($($(this).find('input')[0]).is(':checked')) {
        //                                    // NO is checked
        //                                    ShowOrHideAdditionalInputPanel(QuestionIndex, "HIDE");
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    default:
        //                        alert('Unknown LOB');
        //                        break;
        //                } // end of switch
        //            } // end of if ($($(this).find('input')[1]).is(':checked'))
        //            else{
        //                // NO is checked
        //                if ($($(this).find('input')[0]).is(':checked')) {
        //                    ShowOrHideAdditionalInputPanel(QuestionIndex, "HIDE");
        //                }
        //            }
        //        }
        //    });
        //}

        // HOME
        // This function is called whenever a radio button is clicked
        // Performs custom functionality for LOB HOM
        function HandleRadioButtonClicksHOM(ItemIndex, HiddenQuestions) {
            var QN = ItemIndex + 1
            var allAnswered = true;
            var ndx = -1
            var ans = false
            var CheckForAddlInput = true
            //Added 2/9/18 for HOM Upgrade MLW
            var LOB = $('#<%=hdnLOB.ClientID%>').val();
            var formTypeId = $('#<%=hdnFormType.ClientID%>').val();

            if (HiddenQuestions == "TRUE" && ItemIndex > 13) {
                QN += 3;
            }

            $('#UWQuestionsDiv').find('table').first().find('table').find('table').each(function () {
                ndx += 1;
                if (ndx == ItemIndex) {
                    //alert("itemindex=" + ItemIndex + "; QN=" + QN);
                    if ($($(this).find('input')[1]).is(':checked')) {
                        //alert('YES');
                        // YES IS CHECKED
                        // Deal with the Warning Dialogs
                        switch (QN) {
                            case 5:
                            case 6:
                                //case 9:
                            case 10:
                            case 13:
                            case 15:
                            case 16:
                            case 17:
                            case 21:
                                // Do Nothing, the above questions do not require a dialog
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 7:
                            case 8:
                            case 11:
                            case 12:
                            case 18:
                            case 19:
                            case 20:
                            case 22:
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                                // Show the default dialog
                                alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                                break;
                            case 14:
                            case 9:
                                // Questions 9 & 14 are special cases - Show a Confirmation, deal with the result
                                //Updated 2/9/18 for HOM Upgrade MLW - 9 has changed for HOM Upgrade - no longer need to archive, can treat as additional info cases
                                if (LOB = "HOM" && QN == "9" && doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") === true && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                                    //do nothing, question does not require a dialog
                                } else {
                                    CheckForAddlInput = false;
                                    ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                                    if (ans == true) {
                                        // OK - Archive and navigate back to MyVelocirater
                                        //alert('Archive');
                                        ArchiveQuote()
                                    }
                                    else {
                                        //alert('Cancel');
                                        // CANCEL - Clear the checkboxes
                                        var NO = $(this).find('input')[0];
                                        var YES = $(this).find('input')[1];
                                        NO.checked = false;
                                        YES.checked = false;
                                    }
                                }
                                break;
                        } // end of QN switch

                        // Determine whether or not to show the additional information pane
                        switch (QN) {
                            case 6:
                            case 9:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 21:
                                //Updated 2/9/18 for HOM Upgrade MLW
                                if (LOB = "HOM" && QN == "9" && doUseNewVersionOfLOB(IFMLOBVersionsEnum.HOM2018Upgrade, ifm.vr.currentQuote.lobId, "7/1/2018") === true && (formTypeId == "22" || formTypeId == "23" || formTypeId == "24" || formTypeId == "25" || formTypeId == "26")) {
                                    if (CheckForAddlInput) { ShowOrHideAdditionalInputPanel(ItemIndex, "SHOW"); }
                                } else {
                                    // Questions 6, 9, 14, 15, 16, 17 and 21 do not require additional information
                                }
                                break;
                            default:
                                // The rest of the questions do require additonal information
                                if (CheckForAddlInput) { ShowOrHideAdditionalInputPanel(ItemIndex, "SHOW"); }
                                //if (CheckForAddlInput) { ShowOrHideAdditionalInputPanel(QuestionIndex, "SHOW"); }
                                break;
                        } // end of addl info switch
                    } // end of if ($($(this).find('input')[1]).is(':checked'))
                    else {
                        // NO is checked
                        if (QN == 5 && HOMExistingAutoHelper.IsHOMExistingAutoAvailable(Quote)) {
                            ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                        }
                        if ($($(this).find('input')[0]).is(':checked')) {
                            ShowOrHideAdditionalInputPanel(ItemIndex, "HIDE");
                            //ShowOrHideAdditionalInputPanel(QuestionIndex, "HIDE");
                        }
                    }
                }
            });
        }

        // DWELLING FIRE
        // This function is called whenever a radio button is clicked
        // Performs custom functionality for LOB DFR
        //Updated 7/26/2022 for task 75803 MLW
        function HandleRadioButtonClicksDFR(ItemIndex, HiddenQuestions, DFRStandaloneAvailable) {
        //function HandleRadioButtonClicksDFR(ItemIndex, HiddenQuestions) {
            var QN = ItemIndex + 1
            var allAnswered = true;
            var ndx = -1
            var ans = false
            var CheckForAddlInput = true

            if (HiddenQuestions == "TRUE" && ItemIndex > 13) {
                QN += 3;
            }

            $('#UWQuestionsDiv').find('table').first().find('table').find('table').each(function () {
                ndx += 1;
                if (ndx == ItemIndex) {
                    if ($($(this).find('input')[1]).is(':checked')) {
                        // YES IS CHECKED
                        // Deal with the Warning Dialogs
                        switch (QN) {
                            case 4:  // Note that questions 4, 5, 6, 10, 12, 13, 15, 16, 17, 21 do not require a dialog on yes MGB 12/10/15 Bug 5283
                            case 5:  
                            case 6:
                            case 10:
                            case 12:
                            case 13:
                            case 15:
                            case 16:
                            case 17:
                            case 21:
                            case 999:
                                // Do Nothing, the above questions do not require a dialog
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 7:
                            case 8:
                            case 11:
                            case 18:
                            case 20:
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                                // Show the default dialog
                                alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                                break;
                            case 9:
                            case 14:
                            case 19:
                            case 22:
                                // KILL QUESTIONS - Show a Confirmation, deal with the result
                                CheckForAddlInput = false;
                                ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                                if (ans == true) {
                                    // OK - Archive and navigate back to MyVelocirater
                                    //alert('Archive');
                                    ArchiveQuote()
                                }
                                else {
                                    //alert('Cancel');
                                    // CANCEL - re-check the NO radio button
                                    var NO = $(this).find('input')[0];
                                    var YES = $(this).find('input')[1];
                                    NO.checked = true;
                                    YES.checked = false;
                                }
                        } // end of QN switch

                        // Determine whether or not to show the additional information pane
                        switch (QN) {
                            case 999:
                                // Above Questions do not require additional information
                                break;
                            default:
                                // The rest of the questions do require additonal information
                                if (CheckForAddlInput) { ShowOrHideAdditionalInputPanel(ItemIndex, "SHOW"); }
                                break;
                        } // end of addl info switch
                    } // end of if ($($(this).find('input')[1]).is(':checked'))
                    else {
                        // NO is checked
                        if ($($(this).find('input')[0]).is(':checked')) {
                            switch (QN) {
                                case 5:  // Question 5 requires dialog on NO and a Addl Info box MGB 12/10/15 Bug 5283
                                    //Updated 7/26/2022 for task 75803 MLW
                                    if (DFRStandaloneAvailable == 'TRUE') {
                                        ShowOrHideAdditionalInputPanel(ItemIndex, "HIDE");
                                    }
                                    else { 
                                        ShowOrHideAdditionalInputPanel(ItemIndex, "SHOW");
                                        alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                                    }
                                    //ShowOrHideAdditionalInputPanel(ItemIndex, "SHOW");
                                    //alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                                    break;
                                default:
                                    ShowOrHideAdditionalInputPanel(ItemIndex, "HIDE");
                            }
                        }
                    }
                }
            });
        }


        // AUTO
        //This function is called whenever a radio button is clicked
        //Performs custom functionality for LOB PPA
        function HandleRadioButtonClicksPPA(QuestionIndex) {
            var QN = QuestionIndex + 1
            var allAnswered = true;
            var ndx = -1
            var ans = false
            var CheckForAddlInput = true

            $('#UWQuestionsDiv').find('table').first().find('table').find('table').each(function () {
                ndx += 1;
                if (ndx == QuestionIndex) {
                    //alert('ndx=' + ndx);
                    // YES IS CHECKED
                    if ($($(this).find('input')[1]).is(':checked')) {
                        switch (QN) {
                            case 5:
                            case 6:
                            case 8:
                            case 9:
                            case 13:
                                // Do nothing, the above questions do not require a warning message
                                break;
                            case 16:
                                // #16 does not require any additional input nor does it require a warning message
                                //alert('#16');
                                CheckForAddlInput = false;
                                break;
                            case 15:
                                // Question 15 is a special case - Show a Confirmation, deal with the result
                                CheckForAddlInput = false;
                                ans = confirm("The risk is ineligible, if answered 'Yes' in error select 'Cancel'.  If answered correctly, select 'OK'")
                                if (ans == true) {
                                    // OK - Archive and navigate back to MyVelocirater
                                    ArchiveQuote();
                                }
                                else {
                                    // CANCEL - Clear the checkboxes
                                    var NO = $(this).find('input')[0];
                                    var YES = $(this).find('input')[1];
                                    NO.checked = false;
                                    YES.checked = false;
                                }
                                break;
                            default:
                                // The rest of the questions DO require a warning message
                                alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
                                break;
                        }
                        // show additional input box
                        if (CheckForAddlInput) {
                            ShowOrHideAdditionalInputPanel(QuestionIndex, "SHOW");
                        }
                        else {
                            //alert('hiding panel');
                            ShowOrHideAdditionalInputPanel(QuestionIndex, "HIDE");
                        }
                    }
                    else {
                        if ($($(this).find('input')[0]).is(':checked')) {
                            // NO is checked
                            ShowOrHideAdditionalInputPanel(QuestionIndex, "HIDE");
                        }
                    }
                }
            });   // end of if ($($(this).find('input')[1]).is(':checked'))
        }

        // Shows or hides the Additional Informaion panel
        function ShowOrHideAdditionalInputPanel(ItemIndex, ShowOrHide) {
            var ndx = -1
            $('.DescriptionTable').each(function () {
                ndx += 1;
                if (ItemIndex == ndx) {
                    var pnl = $(this).find('span');
                    if (pnl == null) { alert('panel not found'); }
                    else {
                        if (ShowOrHide == "SHOW") { pnl.context.style.display = 'block'; }
                        else { pnl.context.style.display = 'none'; }
                    }
                }
            });
        }

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
                window.navigate("MyVelocirater.aspx");
                return result;
            });
            //alert('4');
        }
</script>

<header>
    <style type="text/css">
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
            width: 500px;
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

        .DescriptionTable {
        }
    </style>
</header>
<asp:HiddenField ID="hdnLOB" runat="server" />
<asp:HiddenField ID="hdnFormType" runat="server" /><!-- added 2/9/18 for HOM Upgrade MLW -->
<div id="UWQuestionsDiv">
    <h3>Underwriting Questions</h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%;">
        <tr>
            <td valign="top">
                <div align="center">
                    <asp:Label ID="lblAnswerAll" runat="server" Text="All Questions Must Be Answered" Font-Bold="true" Font-Size="14px" ClientIDMode="Static"></asp:Label>
                    <br />
                    <br />
                </div>
                <asp:Repeater ID="rptUWQ" runat="server">
                    <ItemTemplate>
                        <table id="tblUWQ" runat="server">
                            <tr>
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQuestionText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "QuestionText")%>'></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="tblRadioButtons" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAsterisk" runat="server" Text="&nbsp;" Font-Bold="true" ForeColor="Red" Font-Size="15px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbNo" runat="server" Text="No" GroupName="Group0" TabIndex="<%# GetTabIndex()%>" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbYes" runat="server" Text="Yes" GroupName="Group0" TabIndex="<%# GetTabIndex()%>" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="tblUWQDesc" runat="server" class="DescriptionTable">
                            <tr>
                                <%--                                <td class="TableAddlInfoLabelColumn">
                                    &nbsp;
                                </td>--%>
                                <td class="TableDescriptionColumn">
                                    <br />
                                    <asp:Label ID="lblUWQDescriptionTitle" runat="server" Text="Additional Information"></asp:Label>
                                    <asp:TextBox ID="txtUWQDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="50" maxLength="250" OnKeyUp="CheckMaxText(this, 250);"  TabIndex="<%# GetTabIndex()%>"></asp:TextBox>                                                                    
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
                <br />
                <div align="center">
                    <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="StandardSaveButton" OnClientClick='return ValidateForm();' Width="150px" TabIndex="98" />
                    <asp:Button ID="btnGoToApp" runat="server" Text="Application" OnClientClick='return ValidateForm();' CssClass="StandardSaveButton" Width="150px" TabIndex="99" />
                    <%--<asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="StandardBlueButton"  Width="150px" OnClientClick="return confirm('This will clear any entries you have made and reset the answers to their previous values.  Continue?');" TabIndex="32" />--%>
                </div>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</div>