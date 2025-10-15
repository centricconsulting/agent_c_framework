<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlUWQuestionsFARM.ascx.vb" Inherits="IFM.VR.Web.ctlUWQuestionsFARM" %>

<header>
    <style type="text/css">
        .TableLeftColumn {
            width: 75%;
        }

        .TableRightColumn {
            width: 25%;
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

        .OddRow {
            background-color: lightgray;
        }

        .EvenRow {
            background-color: none;
        }
    </style>

    <script type="text/javascript">
        function FocusControl(ControlClientID) {
            $('html, body').animate({
                scrollTop: $("#" + ControlClientID).offset().top
            }, 2000);
            $("#" + ControlClientID).focus();
        }

        function ShowAlert(msg) {
            alert(msg);
        }
    </script>
    <script src="<%=ResolveClientUrl("~/js/VrFarmUWQuestions.js?dt=")%>"></script>
</header>

<asp:HiddenField ID="hdnFocusField" runat="server"></asp:HiddenField>

<asp:UpdatePanel ID="up1" runat="server">
    <ContentTemplate>
        <div id="UWQuestionsDiv">
            <h3>Underwriting Questions</h3>

            <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td valign="top">
                        <div align="center">
                            <asp:Label ID="lblAnswerAll" runat="server" Text="All Questions Must Be Answered" Font-Bold="true" Font-Size="14px" ForeColor="Red" ClientIDMode="Static"></asp:Label>
                            <br />
                            <br />
                        </div>
                        <table id="tblUWQFarm" runat="server" class="questionTable">
                            <%-- QUESTION 1 --%>
                            <tr id="trQuestion1" runat="server">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ1Text" runat="server" Text="1. Number of losses in last 3 years.  Give date, kind of loss, insured and amount paid."></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion1AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ1AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ1AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ1AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);"  TabIndex="1"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 2 --%>
                            <tr runat="server" id="trQuestion2">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ2Text" runat="server" Text="2. Previous Carrier"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion2AddlInfoRow_DDL">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:DropDownList ID="ddlQ2PreviousCarrier" runat="server" Width="450px" TabIndex="2">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Item1" Value="Item1"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion2AddlInfoRow_TEXTBOX">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Label ID="lblpc2" runat="server" Text="Enter Carrier:"></asp:Label>
                                    <asp:TextBox ID="txtQ2PreviousCarrier" runat="server" Width="360px" TabIndex="2"></asp:TextBox>
                                    <input type="button" id="btnCarrierList" value="Carrier List" />
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 3 --%>
                            <tr runat="server" id="trQuestion3">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ3Text" runat="server" Text="3. What is the Principal type of farming?"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion3AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ3AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ3AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:DropDownList ID="ddlQ3PrincipalTypeOfFarming" runat="server" Width="450px" TabIndex="3" Enabled="false">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Dairy" Value="Dairy"></asp:ListItem>
                                        <asp:ListItem Text="Field crops" Value="Field crops"></asp:ListItem>
                                        <asp:ListItem Text="Fruit" Value="Fruit"></asp:ListItem>
                                        <asp:ListItem Text="Greenhouses" Value="Greenhouses"></asp:ListItem>
                                        <asp:ListItem Text="Hobby Farm" Value="Hobby"></asp:ListItem>
                                        <asp:ListItem Text="Horses" Value="Horses"></asp:ListItem>
                                        <asp:ListItem Text="Livestock" Value="Livestock"></asp:ListItem>
                                        <asp:ListItem Text="Poultry" Value="Poultry"></asp:ListItem>
                                        <asp:ListItem Text="Swine" Value="Swine"></asp:ListItem>
                                        <asp:ListItem Text="Vegetables" Value="Vegetables"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtQ3AddlInfo" runat="server" 
                                                 CssClass="DescriptionTextBox" 
                                                 TextMode="MultiLine" MaxLength="144" 
                                                 OnKeyUp="CheckMaxTextFarm(this, 250);"  
                                                 TabIndex="7" 
                                                 Placeholder="Example. 11 CHICKENS WITH FRESH EGGS, FRESH PRODUCE STAND WITH SWEET CORN"
                                                 ></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                                <%--<td class="TableDescriptionColumn" colspan="2">
                                    <asp:DropDownList ID="ddlQ3PrincipalTypeOfFarming" runat="server" Width="450px" TabIndex="3" Enabled="false">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Dairy" Value="Dairy"></asp:ListItem>
                                        <asp:ListItem Text="Field crops" Value="Field crops"></asp:ListItem>
                                        <asp:ListItem Text="Fruit" Value="Fruit"></asp:ListItem>
                                        <asp:ListItem Text="Greenhouses" Value="Greenhouses"></asp:ListItem>
                                        <asp:ListItem Text="Hobby Farm" Value="Hobby"></asp:ListItem>
                                        <asp:ListItem Text="Horses" Value="Horses"></asp:ListItem>
                                        <asp:ListItem Text="Livestock" Value="Livestock"></asp:ListItem>
                                        <asp:ListItem Text="Poultry" Value="Poultry"></asp:ListItem>
                                        <asp:ListItem Text="Swine" Value="Swine"></asp:ListItem>
                                        <asp:ListItem Text="Vegetables" Value="Vegetables"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                </td>--%>
                            </tr>
                            <%-- QUESTION 4 --%>
                            <tr runat="server" id="trQuestion4">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ4Text" runat="server" Text="4. Farmed by owner?  If no, describe the premises."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table1" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ4Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ4No" runat="server" Text="No" GroupName="Group4" TabIndex="4" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ4Yes" runat="server" Text="Yes" GroupName="Group4" TabIndex="5" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion4AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ4AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ4AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ4AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);"  TabIndex="6"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 5 --%>
                            <tr runat="server" id="trQuestion5">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ5Text" runat="server" Text="5. How long has the applicant been farming?"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion5AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ5AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ5AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ5AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);"  TabIndex="7"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 6 --%>
                            <tr runat="server" id="trQuestion6">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ6Text" runat="server" Text="6. Does the applicant have business other than farming?  If yes, explain."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table3" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ6Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ6No" runat="server" Text="No" GroupName="Group6" TabIndex="8" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ6Yes" runat="server" Text="Yes" GroupName="Group6" TabIndex="9" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion6AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ6AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ6AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ6AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144"  OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="1"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 7 --%>
                            <tr runat="server" id="trQuestion7">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ7Text" runat="server" Text="7. Has any policy been cancelled or non-renewed in the past 5 years?  If yes, explain."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table4" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ7Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ7No" runat="server" Text="No" GroupName="Group7" TabIndex="10" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ7Yes" runat="server" Text="Yes" GroupName="Group7" TabIndex="11" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion7AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ7AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ7AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ7AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="12"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 8 --%>
                            <tr runat="server" id="trQuestion8">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ8Text" runat="server" Text="8. Are there any vacant or unoccupied dwellings located on the premises?  If yes, describe."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table5" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ8Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ8No" runat="server" Text="No" GroupName="Group8" TabIndex="13" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ8Yes" runat="server" Text="Yes" GroupName="Group8" TabIndex="14" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion8AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ8AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ8AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ8AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="15"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 9 --%>
                            <tr runat="server" id="trQuestion9">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ9Text" runat="server" Text="9. Is any part of the farm used or leased for organized recreational use?  This includes hunting for payment or pleasure, or the use of dirtbikes or ATVs for any purpose other than strictly farming use.  If yes, explain."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table6" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ9Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ9No" runat="server" Text="No" GroupName="Group9" TabIndex="16" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ9Yes" runat="server" Text="Yes" GroupName="Group9" TabIndex="17" />
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion9AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ9AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ9AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ9AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="18"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 10 --%>
                            <tr runat="server" id="trQuestion10">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ10Text" runat="server" Text="10. Does insured do any custom farming?  If yes, what are annual receipts?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table7" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ10Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ10No" runat="server" Text="No" GroupName="Group10" TabIndex="19" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ10Yes" runat="server" Text="Yes" GroupName="Group10" TabIndex="20" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion10AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ10AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ10AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ10AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="21"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 10a --%>
                            <tr runat="server" id="trQuestion10a">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ10aText" runat="server" Text="10a. If yes, describe the activities of the custom farming:"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion10aAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ10aAddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ10aAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ10aAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="22"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 10b --%>
                            <tr runat="server" id="trQuestion10b">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ10bText" runat="server" Text="10b. If yes, is coverage to be included for application of herbicides and pesticides?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table21" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ10bAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ10bNo" runat="server" Text="No" GroupName="Group10b" TabIndex="23" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ10bYes" runat="server" Text="Yes" GroupName="Group10b" TabIndex="24" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 10c --%>
                            <tr runat="server" id="trQuestion10c">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ10cText" runat="server" Text="10c. If yes, does the applicant hold any permits required for application?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table22" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ10cAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ10cNo" runat="server" Text="No" GroupName="Group10c" TabIndex="26" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ10cYes" runat="server" Text="Yes" GroupName="Group10c" TabIndex="27" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 11 --%>
                            <tr runat="server" id="trQuestion11">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ11Text" runat="server" Text="11. Are the farm premises open to the public for activities such as roadside stand, auctions, Christmas tree farms, 'U-Pick', any agritainment exposure such as pumpkin patch and associated activities, hay rides, or any spaces used or rented for special occasions??  If yes, describe?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table8" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ11Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ11No" runat="server" Text="No" GroupName="Group11" TabIndex="28" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ11Yes" runat="server" Text="Yes" GroupName="Group11" TabIndex="29" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion11AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ11AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ11AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ11AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="30"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 12 --%>
                            <tr runat="server" id="trQuestion12">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ12Text" runat="server" Text="12. Are any of the following items located on any of the premises described?  Swimming pool?  If yes, attach photo."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table9" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ12Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ12No" runat="server" Text="No" GroupName="Group12" TabIndex="31" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ12Yes" runat="server" Text="Yes" GroupName="Group12" TabIndex="32" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 12a --%>
                            <tr runat="server" id="trQuestion12a">
                                <td class="TableLeftColumn" colspan="2">
                                    <br />
                                    <asp:Label ID="lblQ12aText" runat="server" Text="12a. If yes, describe."></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion12aAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ12aAddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ12aAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ12aAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="33"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 12b --%>
                            <tr runat="server" id="trQuestion12b">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ12bText" runat="server" Text="12b. Above or in-ground?"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion12bAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:DropDownList ID="ddlQ12bAboveOrInGroundPool" runat="server" TabIndex="34">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Above-Ground" Value="Above-Ground"></asp:ListItem>
                                        <asp:ListItem Text="In-Ground" Value="In-Ground"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 12c --%>
                            <tr runat="server" id="trQuestion12c">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ12cText" runat="server" Text="12c. Depth at the deepest part of the pool?"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion12cAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ12cAddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ12cAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ12cAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);"  TabIndex="35"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 12d --%>
                            <tr runat="server" id="trQuestion12d">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ12dText" runat="server" Text="12d. Slide or diving board?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table23" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ12dAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ12dNo" runat="server" Text="No" GroupName="Group12d" TabIndex="36" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ12dYes" runat="server" Text="Yes" GroupName="Group12d" TabIndex="37" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion12dAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:DropDownList ID="ddlQ12dSlideOrDivingBoard" runat="server">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Slide Only" Value="Slide"></asp:ListItem>
                                        <asp:ListItem Text="Diving Board Only" Value="Board"></asp:ListItem>
                                        <asp:ListItem Text="Both Slide and Diving Board" Value="Both"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 12e --%>
                            <tr runat="server" id="trQuestion12e">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ12eText" runat="server" Text="12e. Fully fenced and locked, or locking pool cover?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table24" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ12eAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ12eNo" runat="server" Text="No" GroupName="Group12e" TabIndex="39" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ12eYes" runat="server" Text="Yes" GroupName="Group12e" TabIndex="40" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion12eAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ12eAddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ12eAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ12eAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="41"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 13 --%>
                            <tr runat="server" id="trQuestion13">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ13Text" runat="server" Text="13. Trampoline?  If yes, attach photo."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table10" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ13Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ13No" runat="server" Text="No" GroupName="Group13" TabIndex="42" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ13Yes" runat="server" Text="Yes" GroupName="Group13" TabIndex="43" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 13a --%>
                            <tr id="trQuestion13a" runat="server">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ13aText" runat="server" Text="13a. If yes, is it fully netted and padded?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table11" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ13aAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ13aNo" runat="server" Text="No" GroupName="Group13a" TabIndex="44" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ13aYes" runat="server" Text="Yes" GroupName="Group13a" TabIndex="45" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 14 --%>
                            <tr runat="server" id="trQuestion14">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ14Text" runat="server" Text="14. Any dogs on premises?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table12" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ14Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ14No" runat="server" Text="No" GroupName="Group14" TabIndex="46" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ14Yes" runat="server" Text="Yes" GroupName="Group14" TabIndex="47" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 14a --%>
                            <tr runat="server" id="trQuestion14a">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ14aText" runat="server" Text="14a. If yes, how many?"></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion14aAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ14aAddlInfo" runat="server" >
                                        <br />
                                        <asp:Label ID="lblQ14aAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ14aAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="48"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 14b --%>
                            <tr runat="server" id="trQuestion14b">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ14bText" runat="server" Text="14b. Is any dog present either full or part breed of the following: Pit Bull, Rottweiler, Husky, Wolf Hybrid, German Shepard, Chow or Doberman?  If yes, describe."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table25" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ14bAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ14bNo" runat="server" Text="No" GroupName="Group14b" TabIndex="49" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ14bYes" runat="server" Text="Yes" GroupName="Group14b" TabIndex="50" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion14bAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ14bAddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ14bAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ14bAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="51"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 15 --%>
                            <tr runat="server" id="trQuestion15">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ15Text" runat="server" Text="15. Power generation occurring on the premises (this includes wind, solar, digester, etc.)?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table13" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ15Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ15No" runat="server" Text="No" GroupName="Group15" TabIndex="52" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ15Yes" runat="server" Text="Yes" GroupName="Group15" TabIndex="53" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 15a --%>
                            <tr id="trQuestion15a" runat="server">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ15aText" runat="server" Text="15a. If yes, is excess power sold back to the grid?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table14" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ15aAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ15aNo" runat="server" Text="No" GroupName="Group15a" TabIndex="54" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ15aYes" runat="server" Text="Yes" GroupName="Group15a" TabIndex="55" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 16 --%>
                            <tr runat="server" id="trQuestion16">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ16Text" runat="server" Text="16. Is there any fracking on premises?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table15" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ16Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ16No" runat="server" Text="No" GroupName="Group16" TabIndex="56" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ16Yes" runat="server" Text="Yes" GroupName="Group16" TabIndex="57" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 17 --%>
                            <tr runat="server" id="trQuestion17">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ17Text" runat="server" Text="17. Any other hazard or operation not farm related?  If yes, describe."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table16" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ17Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ17No" runat="server" Text="No" GroupName="Group17" TabIndex="58" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ17Yes" runat="server" Text="Yes" GroupName="Group17" TabIndex="59" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion17AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ17AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ17AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ17AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);"  TabIndex="60"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 18 --%>
                            <tr runat="server" id="trQuestion18">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ18Text" runat="server" Text="18. Are the described insured premises the only premises the applicant owns, rents or operates?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table17" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ18Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ18No" runat="server" Text="No" GroupName="Group18" TabIndex="61" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ18Yes" runat="server" Text="Yes" GroupName="Group18" TabIndex="62" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 19 --%>
                            <tr runat="server" id="trQuestion19">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ19Text" runat="server" Text="19. Are there any horses on premises?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table18" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ19Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ19No" runat="server" Text="No" GroupName="Group19" TabIndex="63" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ19Yes" runat="server" Text="Yes" GroupName="Group19" TabIndex="64" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 19a --%>
                            <tr id="trQuestion19a" runat="server">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ19aText" runat="server" Text="19a. If yes, are they for personal use only with no revenue generation?"></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table19" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ19aAsterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ19aNo" runat="server" Text="No" GroupName="Group19a" TabIndex="65" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ19aYes" runat="server" Text="Yes" GroupName="Group19a" TabIndex="66" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <%-- QUESTION 19b --%>
                            <tr runat="server" id="trQuestion19b">
                                <td class="TableLeftColumn" colspan="2">
                                    <asp:Label ID="lblQ19bText" runat="server" Text="19b. If revenue is generated, please describe any of the following exposures: boarding, breeding, trainingm riding lessons, showing, racing or any other equine exposures."></asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion19bAddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ19bAddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ19bAddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ19bAddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" MaxLength="144" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="67"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <%-- QUESTION 20 --%>
                            <tr runat="server" id="trQuestion20">
                                <td class="TableLeftColumn">
                                    <asp:Label ID="lblQ20Text" runat="server" Text="20. Are there any supporing insurance policies in force with Indiana Farmers Mutual Insurance?  If yes, please list."></asp:Label>
                                </td>
                                <td class="TableRightColumn">
                                    <table id="Table20" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblQ20Asterisk" runat="server" Text="*" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:RadioButton ID="rbQ20No" runat="server" Text="No" GroupName="Group20" TabIndex="68" />
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbQ20Yes" runat="server" Text="Yes" GroupName="Group20" TabIndex="69" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trQuestion20AddlInfoRow">
                                <td class="TableDescriptionColumn" colspan="2">
                                    <asp:Panel ID="pnlQ20AddlInfo" runat="server">
                                        <br />
                                        <asp:Label ID="lblQ20AddlInfo" runat="server" Text="Additional Information Response Required" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:TextBox ID="txtQ20AddlInfo" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" OnKeyUp="CheckMaxTextFarm(this, 250);" TabIndex="70"></asp:TextBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<div align="center">
    <br />
    <asp:Panel ID="pnlErrors" runat="server" Visible="false" BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
        <asp:Label ID="lblErrorHeader" runat="server" Text="Please correct the following errors:" Font-Size="20px" Font-Bold="True"></asp:Label>
        <br />
        <asp:Repeater ID="rptErrors" runat="server">
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="text-align: right;">
                            <asp:Label ID="lblErrorNumber" runat="server" Text="#"></asp:Label>
                        </td>
                        <td style="text-align: left; width: 300px">
                            <asp:Label ID="lblErrorMessage" runat="server" Text="Error Msg"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    <br />
    <asp:Button ID="btnCancel" runat="server" Text="Save" CssClass="StandardSaveButton" Width="150px"  TabIndex="32" />
    <asp:Button ID="btnContinue" runat="server" Text="Application" CssClass="StandardSaveButton" Width="150px" TabIndex="98" />
    <br />
    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true" Font-Size="22px"></asp:Label>
</div>