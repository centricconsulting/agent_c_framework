<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQsummary_PPA_Vehicle.ascx.vb" Inherits="IFM.VR.Web.ctlQsummary_PPA_Vehicle" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=Me.divQSummaryVItemt.ClientID%>").accordion({ heightStyle: "content", active: <%=Me.hiddenvListActive.Value%>, collapsible: true, activate: function (event, ui) { $("#<%=Me.hiddenvListActive.ClientID%>").val($("#<%=Me.divQSummaryVItemt.ClientID%>").accordion('option','active'));  } });

    });
</script>

<div id="divQSummaryVItemt" runat="server">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Vehicle List"></asp:Label>
        <span style="float: right; margin-right: 10px;">
            <asp:Label ID="lblVehClass" runat="server"></asp:Label>
            <asp:Label ID="lblVehicleSummary" runat="server" Style="margin-left: 15px;"></asp:Label>
        </span>
    </h3>

    <div>
        <table style="width: 100%;">
            <tr style="width: 50%">
                <td style="vertical-align: top">
                    <asp:Panel ID="pnlComp" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Comprehensive
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_Comp_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_Comp_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCollision" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Collision
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_Collision_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_Collision_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlTowing" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Towing and Labor
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_Towing_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_Towing_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlRental" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Transportation Expense
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_Transportation_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_Transportation_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlTrip" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Trip Interruption
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lblTripVal" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lblTripPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlPoliceTrainingFee" runat="server" Visible="false">
                        <table style="width: 100%">
                            <tr>
                                <td Id="tdCovName" runat="server" style="width: 75%" class="tblCovNameLabel">Illinois State Police Training <br />and Academy Fund
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lblPoliceTrainingFeePrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td style="vertical-align: top">
                    <asp:Panel ID="pnlSound" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Sound Equipment
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lblSoundVal" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lblSoundPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlAV" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Audio, Visual, Data Equipment
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_AV_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_AV_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlMedia" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Tapes, Discs and Other Media
                                <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_Media_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_Media_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlLoan" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Loan/Lease
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_Loan_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_Loan_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlUMPD" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">UM PD
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_UMPD_Limit" runat="server" Visible="false"></asp:Label>
                                    <asp:Label CssClass="tblCovValLabel" ID="lbl_UMPD_Val" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lbl_UMPD_Prem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlPollution" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Pollution Liability
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lblPollutionVal" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lblPollutionPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCustom" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Motorcycle Custom Equipment
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lblCustomVal" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lblCustomPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlCustomEquip" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 75%" class="tblCovNameLabel">Customized Equipment
                                    <br />
                                    <asp:Label CssClass="tblCovValLabel" ID="lblCustomEquipVal" runat="server"></asp:Label>
                                </td>
                                <td class="tblPremCell">
                                    <asp:Label CssClass="tblCovPremLabel" ID="lblCustomEquipPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
</div>
<asp:HiddenField ID="hiddenvListActive" Value="0" runat="server" />