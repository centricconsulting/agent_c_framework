<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQsummary_PPA.ascx.vb" Inherits="IFM.VR.Web.ctlQsummary_PPA" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlQsummary_PPA_Vehicle_List.ascx" TagPrefix="uc1" TagName="ctlQsummary_PPA_Vehicle_List" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlPayPlanOptions.ascx" TagPrefix="uc1" TagName="ctlPayPlanOptions" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#divqSummary").accordion({ heightStyle: "content", active: <%=Me.hiddenActiveqSummary.Value%>, collapsible: true, activate: function (event, ui) { $("#<%=Me.hiddenActiveqSummary.ClientID%>").val($("#divqSummary").accordion('option','active'));  } });

    });
</script>

<style>
    .tblRowAlternating {
        background-color: white;
    }

    .tblCovNameLabel {
        font-weight: bold;
        font-size: larger;
    }

    .tblCovValLabel {
        margin-left: 40px;
        color: grey;
    }

    .tblCovPremLabel {
        color: grey;
    }

    .tblPremCell {
        text-align: right;
        vertical-align: bottom;
        padding-right: 25px;
    }
</style>

<div id="divWholeCov">
    <asp:Panel ID="pnlCoverage" runat="server">
        <div id="divqSummary">
            <h3>
                <span style="float: right;">
                    <asp:Label ID="lblQuoteSummary" runat="server"></asp:Label>
                    <asp:LinkButton ID="lnkPrint" CssClass="RemovePanelLink" Style="margin-left: 20px;" runat="server">Printer Friendly</asp:LinkButton></span>
                <asp:Label ID="lblHeader" runat="server"></asp:Label>
                <span runat="server" id="ImageDateAndPremChangeLine" visible="false">
                    <br />
                    <asp:Label ID="lblTranEffDate" runat="server"></asp:Label>
                    <asp:Label ID="lblAnnualPremChg" runat="server" Style="margin-left: 20px;"></asp:Label>
                </span>
            </h3>
            <div>
                <table style="width: 100%; margin-bottom: 30px">
                    <tr id="tblSplitLimit" style="width: 50%" runat="server">
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlBodilyInjury" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">Bodily Injury
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_SL_BodiliyInjury" runat="server"></asp:Label>
                                        </td>
                                        <td class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_SL_Prem_BodilyInjury" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlPropDamage" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">Property Damage
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_SL_PropertyDamage" runat="server"></asp:Label>
                                        </td>
                                        <td class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_SL_Prem_PropertyDamage" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlUMBI" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">UM/UIM BI
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_SL_UMUIM_BI" runat="server"></asp:Label>
                                        </td>
                                        <td class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_SL_Prem_UMUIM_BI" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlUMPD" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">UM/UIM PD
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_SL_UMPD" runat="server"></asp:Label>
                                        </td>
                                        <td class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_SL_Prem_UMPD" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <%--                </table>
                <table id="tblSLL" runat="server" style="width:100%">  --%>
                    <tr id="tblSLL" style="width: 50%" runat="server">
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlSLL" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">Single Limit Liability
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_SLL_SLLCov" runat="server"></asp:Label>
                                        </td>
                                        <td align="right" class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_SLL_SLLCov_Prem" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlUMCSL" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">UM/UIM CSL
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_SLL_UMUIM_CSL" runat="server"></asp:Label>
                                        </td>
                                        <td align="right" class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_SLL_UMUIM_CSL_PREM" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <%--                </table>
                <table style="width:100%; margin-bottom:30px">--%>
                    <tr style="width: 50%">
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlMedPay" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">Medical Payments
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_Med_Pay" runat="server"></asp:Label>
                                        </td>
                                        <td align="right" class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_Med_Pay_Prem" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlUMPDDed" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">UM PD Deductible
                                            <br />
                                            <asp:Label CssClass="tblCovValLabel" ID="lbl_UMPD_Deduc" runat="server"></asp:Label>
                                        </td>
                                        <td align="right" class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lbl_UMPD_Deduc_Prem" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%; margin-bottom: 30px;">
                    <tr style="width: 100%">
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlAutoEnhance" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 75%" class="tblCovNameLabel">Auto Enhancement Endorsement</td>
                                        <td class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lblEnhancementPrem" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trAutoPlusEnhance" runat="server">
                                        <td style="width: 75%" class="tblCovNameLabel">Auto Plus Enhancement Endorsement</td>
                                        <td class="tblPremCell">
                                            <asp:Label CssClass="tblCovPremLabel" ID="lblAutoPlusEnhancePrem" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <div class="informationalText" style="font-size:smaller;margin-left:25px;margin-right:25px;">
                    <asp:Label id="lblFeeText" runat="server"></asp:Label>
                </div>
            </div>
        </div>
    </asp:Panel>

    <uc1:ctlPayPlanOptions runat="server" ID="ctlPayPlanOptions" />

    <uc1:ctlQsummary_PPA_Vehicle_List runat="server" ID="ctlQsummary_PPA_Vehicle_List" />

    <uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />
</div>

<asp:HiddenField ID="hiddenActiveqSummary" Value="0" runat="server" />