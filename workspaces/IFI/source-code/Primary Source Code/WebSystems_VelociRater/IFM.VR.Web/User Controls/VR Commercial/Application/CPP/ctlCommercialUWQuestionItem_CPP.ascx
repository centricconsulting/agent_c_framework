<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCommercialUWQuestionItem_CPP.ascx.vb" Inherits="IFM.VR.Web.ctlCommercialUWQuestionItem_CPP" %>
<script>

</script>

    <asp:Repeater ID="rptUWQ" runat="server">
                    <ItemTemplate>
                            <table id="tblUWQ" runat="server" class='<%# "questionTable " + DataBinder.Eval(Container.DataItem, "PolicyUnderwritingCodeId") + " " + IIf(DataBinder.Eval(Container.DataItem, "IsQuestionRequired") = True, "required", "") %>'>
                                <tr>
                                    <td class="TableLeftColumn">
                                        <asp:Label ID="lblQuestionText" runat="server" Text='<%#IIf(DataBinder.Eval(Container.DataItem, "IsQuestionRequired") = True, "*", "") + DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label>
                                    </td>
                                    <td class="TableRightColumn">
                                        <table id="tblRadioButtons" runat="server" data-diamondcode='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingCodeId")%>' data-lob='<%#IFM.VR.Common.Helpers.LOBHelper.GetAbbreviatedLOBPrefix(Me.Quote.LobType)%>' class='<%# IIf(DataBinder.Eval(Container.DataItem, "NeverShowQuestions") = True, "neverShow", "") %>'>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblAsterisk" runat="server" Text="*" Font-Bold="true" ForeColor="Red" Font-Size="15px" style="display:none" CssClass="requiredLabelError"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:RadioButton ID="rbNo" runat="server" Text="No" GroupName="Group0" data-selection='<%#IIf(DataBinder.Eval(Container.DataItem, "QuestionAnswerNo") = True, "selected", "") %>' />
                                                </td>
                                                <td>
                                                    <asp:RadioButton ID="rbYes" runat="server" Text="Yes" GroupName="Group0" data-selection='<%#IIf(DataBinder.Eval(Container.DataItem, "QuestionAnswerYes") = True, "selected", "") %>' />
                                                </td>
                                            </tr>
                                            
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblUWQDesc" runat="server" class='<%# "DescriptionTable " + DataBinder.Eval(Container.DataItem, "PolicyUnderwritingCodeId") + " " + IIf(DataBinder.Eval(Container.DataItem, "NeverShowDescription") = True, "neverShow", "") + " " + IIf(DataBinder.Eval(Container.DataItem, "AlwaysShowDescription") = True, "alwaysShow", "") + " " + IIf(DataBinder.Eval(Container.DataItem, "DescriptionNotRequired") = True, "NotRequired", "")  %>' data-diamondcode='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingCodeId")%>' style="display:none;">
                                <tr style="padding: auto 5px;">
                                    <td class="TableDescriptionColumn">
                                        <br />
                                        <asp:Label ID="lblUWQDescriptionTitle" runat="server" Text="Additional Information"></asp:Label>
                                        <asp:TextBox ID="txtUWQDescription" runat="server" CssClass="DescriptionTextBox" TextMode="MultiLine" Rows="4" Columns="50" maxLength="125" OnKeyUp="CheckMaxTextNoDisable(this, 125);" OnPaste="CheckMaxTextNoDisable(this, 125);" Text='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingExtraAnswer")%>'></asp:TextBox>  <%--OnKeyUp="CheckMaxText(this, 125);"--%>
                                        <br />
                                        <br />
                                    </td>
                                </tr>
                            </table>
                    </ItemTemplate>
                </asp:Repeater>
    <%--<button ID="btnSaveMeTest" runat="server" Text="Save" class="StandardSaveButton" onserverclick="SaveMeTest" Width="150px" TabIndex="98">Save</button>--%>