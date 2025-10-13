<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_UnderwritingQuestionsByLob.ascx.vb" Inherits="IFM.VR.Web.ctl_UnderwritingQuestionsByLob" ViewStateMode="Enabled" %>
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
            width: 100%;
        }

        .DescriptionTextBox {
            width: 450px;
            height: 50px;
        }

            .DescriptionTextBox.Invalid {
                border: 1px solid red;
            }

        .DescriptionTextBoxLabel.Invalid {
            color: red;
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

        #lblAnswerAll.Invalid {
            color: red;
        }
    </style>
</header>
<script type="text/javascript">
    ; (function ($, window) {
        //contained and not in the global context
        const VALIDATION_FAILED_CSS_CLASS = 'Invalid';
        const DATA_YES = 'Yes';
        const DATA_NO = 'No';
        const DATA_TRUE = "True";
        const DATA_FALSE = "False";

        let uwQuestions = {}
        let initComplete = false

        function UwQuestionAnswered(event) {
            let dataHolder = $(event.target).parent()
            let questionNumber = dataHolder.data('questionNumber');
            let selectedValue = dataHolder.data('questionOption');
            let referToUnderwriting = dataHolder.data('questionOptionUnderwriting');
            let required = dataHolder.data('questionRequired');
            let showDescription = dataHolder.data('questionShowDescription');            
            let confirmRisk = dataHolder.data('questionConfirmRisk');

            if (required === DATA_TRUE) {
                uwQuestions[`q${questionNumber}`] = selectedValue;
            }

            let tblDescription = $(`.DescriptionTable[data-question-number=${questionNumber}]`);
           
            if (confirmRisk === DATA_TRUE) {
                if (promptUserToConfirmRisk(selectedValue)) {
                    ArchiveQuote();
                }
                else {
                    uwQuestions[`q${questionNumber}`] = ""
                    event.target.checked = false;
                    event.preventDefault();
                }
            }

            if (showDescription === DATA_TRUE)
                tblDescription.show();
            else
                tblDescription.hide();

            if (initComplete && referToUnderwriting === DATA_TRUE)
                showReferToUnderwritingAlert();
        }

        function promptUserToConfirmRisk(seletcedValue) {
            return confirm(`The risk is ineligible, if answered '${selectedValue}' in error select 'Cancel'.  If answered correctly, select 'OK'`)
        }
        function showReferToUnderwritingAlert() {
            alert('The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter.');
        }
        function ValidateForm() {
            //all required questions answered
            let allAnswered = true;
            Object.entries(uwQuestions).forEach(function ([key, value], index, allItems) {
                let questionNumber = key.slice(1);
                let validationMarker = $(`.validationRequired[data-question-number=${questionNumber}]`);

                if (value) {
                    validationMarker.hide()
                }
                else {
                    allAnswered = false;
                    validationMarker.show()
                }
            });

            //all visible textboxes are filled
            $('textarea.DescriptionTextBox:visible').each(function (index, textarea) {
                if ($(textarea).val()) {
                    $(textarea).removeClass(VALIDATION_FAILED_CSS_CLASS);
                    $(textarea).siblings('.DescriptionTextBoxLabel').removeClass(VALIDATION_FAILED_CSS_CLASS);
                }
                else {
                    allAnswered = false;
                    $(textarea).addClass(VALIDATION_FAILED_CSS_CLASS);
                    $(textarea).siblings('.DescriptionTextBoxLabel').addClass(VALIDATION_FAILED_CSS_CLASS);
                }
            });

            //set color of top label-based on validation
            if (allAnswered) {
                $('#lblAnswerAll').removeClass(VALIDATION_FAILED_CSS_CLASS);
            }
            else {
                $('#lblAnswerAll').addClass(VALIDATION_FAILED_CSS_CLASS);
            }

            return allAnswered;
        }

        //use this to export the function to the global context (window)
        //using a delegated event handler is not working with the asp.net buttons to preempt postback
        window.ValidateForm = ValidateForm;

        //not currently being used in OH PPA
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
                    if (result === null || result === '') {
                        // no data
                        alert('Unable to change Quote Status, please alert IT');
                    }
                    window.navigate("MyVelocirater.aspx");
                    return result;
                });
            //alert('4');
        }

        $(function () {

            $('#UWQuestionsDiv').accordion({collapsible: false});

            //attach deffered eventHandlers
            $('#UWQuestionsDiv').on('change', 'input:radio', UwQuestionAnswered);


            //hide all expanded sections
            $('table.DescriptionTable').hide();

            //init Underwriting Question array and update display
            $("table.UwQuestionRow").each(function (index, qrow) {
                let questionNumber = $(qrow).data('questionNumber');
                let selectedOption = $(qrow).find('input:radio:checked')[0];

                if ($(qrow).data('questionRequired') === DATA_TRUE) {
                    uwQuestions[`q${questionNumber}`] = selectedOption ? $(selectedOption).data('questionOption') : null;
                }

                if (selectedOption) {
                    $(selectedOption).trigger('change'); //run the event handler for this
                }
            });

            //hide all validation labels
            //if the question isn't required, then ASP.NET shouldn't even generate markup for the validation label
            $('.validationRequired').hide();

            initComplete = true;
        });
    })(jQuery, window);

</script>
<div id="UWQuestionsDiv">
    <h3>Underwriting Questions</h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%;">
        <tr>
            <td valign="top">
                <div align="center">
                    <asp:Label ID="lblAnswerAll" runat="server" Text="All Required Questions Must Be Answered" Font-Bold="true" Font-Size="14px" ClientIDMode="Static"></asp:Label>
                    <br />
                    <br />
                </div>
                <asp:Repeater ID="rptUWQ" runat="server">
                    <ItemTemplate>
                        <table id="tblUWQ" runat="server" class="UwQuestionRow"
                            data-question-number='<%# Eval("QuestionNumber") %>'
                            data-question-required='<%# Eval("IsQuestionRequired") %>'>
                            <tr>
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQuestionText" runat="server" Text='<%# Eval("Description")%>'></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="tblRadioButtons" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAsterisk" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px"
                                                    data-question-number='<%# Eval("QuestionNumber") %>'
                                                    CssClass="validationRequired"
                                                    Visible='<%# Eval("IsQuestionRequired") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbNo" runat="server" Text="No" GroupName='<%# Eval("QuestionNumber", "questionNumber{0}")%>' TabIndex="<%# GetTabIndex()%>" Checked='<%# Eval("QuestionAnswerNo") %>'
                                                    data-question-number='<%# Eval("QuestionNumber") %>'
                                                    data-question-option="No"
                                                    data-question-required='<%# Eval("IsQuestionRequired") %>'
                                                    data-question-show-description='<%# Eval("ShowDescriptionOnNo") %>'
                                                    data-question-option-underwriting='<%# Eval("ReferToUnderwritingOnNo") %>' 
                                                    data-question-confirm-risk='<%# Eval("ConfirmRiskOnNo") %>' />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbYes" runat="server" Text="Yes" GroupName='<%# Eval("QuestionNumber", "questionNumber{0}")%>' TabIndex="<%# GetTabIndex()%>" Checked='<%# Eval("QuestionAnswerYes") %>'
                                                    data-question-number='<%# Eval("QuestionNumber") %>'
                                                    data-question-option="Yes"
                                                    data-question-required='<%# Eval("IsQuestionRequired") %>'
                                                    data-question-show-description='<%# Eval("ShowDescriptionOnYes") %>'
                                                    data-question-option-underwriting='<%# Eval("ReferToUnderwritingOnYes") %>'
                                                    data-question-confirm-risk='<%# Eval("ConfirmRiskOnYes") %>' />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="tblUWQDesc" runat="server" class="DescriptionTable"
                            data-question-number='<%# Eval("QuestionNumber") %>'>
                            <tr>
                                <td class="TableDescriptionColumn">
                                    <br />
                                    <asp:Label ID="lblUWQDescriptionTitle" runat="server" Text='<%# Eval("QuestionNumber", "Question {0} Additional Information Required") %>' CssClass="DescriptionTextBoxLabel"></asp:Label>
                                    <asp:TextBox ID="txtUWQDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="50" MaxLength="250" OnKeyUp="CheckMaxText(this, 250);" TabIndex="<%# GetTabIndex()%>"
                                        Text='<%# Eval("DetailTextOnQuestionYes") %>'
                                        data-question-number='<%# Eval("QuestionNumber") %>'></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>

                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table id="tblUWQ" runat="server" bgcolor="LightGray" class="UwQuestionRow"
                            data-question-number='<%# Eval("QuestionNumber") %>'
                            data-question-required='<%# Eval("IsQuestionRequired") %>'>
                            <tr>
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQuestionText" runat="server" Text='<%# Eval("Description")%>'></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="tblRadioButtons" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAsterisk" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px"
                                                    data-question-number='<%# Eval("QuestionNumber") %>'
                                                    CssClass="validationRequired"
                                                    Visible='<%# Eval("IsQuestionRequired") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbNo" runat="server" Text="No" GroupName='<%# Eval("QuestionNumber", "questionNumber{0}")%>' TabIndex="<%# GetTabIndex()%>" Checked='<%# Eval("QuestionAnswerNo") %>'
                                                    data-question-number='<%# Eval("QuestionNumber") %>'
                                                    data-question-option="No"
                                                    data-question-required='<%# Eval("IsQuestionRequired") %>'
                                                    data-question-show-description='<%# Eval("ShowDescriptionOnNo") %>'
                                                    data-question-option-underwriting='<%# Eval("ReferToUnderwritingOnNo") %>' 
                                                    data-question-confirm-risk='<%# Eval("ConfirmRiskOnNo") %>' />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbYes" runat="server" Text="Yes" GroupName='<%# Eval("QuestionNumber", "questionNumber{0}")%>' TabIndex="<%# GetTabIndex()%>" Checked='<%# Eval("QuestionAnswerYes") %>'
                                                    data-question-number='<%# Eval("QuestionNumber") %>'
                                                    data-question-option="Yes"
                                                    data-question-required='<%# Eval("IsQuestionRequired") %>'
                                                    data-question-show-description='<%# Eval("ShowDescriptionOnYes") %>'
                                                    data-question-option-underwriting='<%# Eval("ReferToUnderwritingOnYes") %>'
                                                    data-question-confirm-risk='<%# Eval("ConfirmRiskOnYes") %>' />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="tblUWQDesc" runat="server" class="DescriptionTable" bgcolor="LightGray"
                            data-question-number='<%# Eval("QuestionNumber") %>'>
                            <tr>
                                <td class="TableDescriptionColumn">
                                    <br />
                                    <asp:Label ID="lblUWQDescriptionTitle" runat="server" Text='<%# Eval("QuestionNumber", "Question {0} Additional Information Required") %>' CssClass="DescriptionTextBoxLabel"></asp:Label>
                                    <asp:TextBox ID="txtUWQDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="50" MaxLength="250" OnKeyUp="CheckMaxText(this, 250);" TabIndex="<%# GetTabIndex()%>"
                                        Text='<%# Eval("DetailTextOnQuestionYes") %>'
                                        data-question-number='<%# Eval("QuestionNumber") %>'></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </AlternatingItemTemplate>
                </asp:Repeater>

                <br />
                <div align="center">
                    <asp:Button ID="btnSave" CommandName="save" runat="server" Text="Save" CssClass="StandardSaveButton" Width="150px" TabIndex="98" OnClientClick='return ValidateForm();' />
                    <asp:Button ID="btnGotoApp" CommandName="gotoApp" runat="server" Text="Application" CssClass="StandardSaveButton" Width="150px" TabIndex="99" OnClientClick='return ValidateForm();' />
                </div>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</div>
