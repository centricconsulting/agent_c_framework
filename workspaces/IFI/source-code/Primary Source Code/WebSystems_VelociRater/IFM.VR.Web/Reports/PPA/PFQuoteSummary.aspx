<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PFQuoteSummary.aspx.vb" Inherits="IFM.VR.Web.PFQuoteSummary" %>

<%@ Register Src="~/Reports/PPA/VR3Proposal_ClientAndAgencyInfo.ascx" TagPrefix="uc1" TagName="VR3Proposal_ClientAndAgencyInfo" %>
<%@ Register Src="~/Reports/PPA/VR3Proposal_PaymentOptions.ascx" TagPrefix="uc1" TagName="VR3Proposal_PaymentOptions" %>
<%@ Register Src="~/Reports/PPA/VR3QuoteSummaryVehicleList.ascx" TagPrefix="uc1" TagName="VR3QuoteSummaryVehicleList" %>
<%@ Register Src="~/Reports/ctlPFHeader.ascx" TagPrefix="uc1" TagName="ctlPFHeader" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/styles/vr.css" rel="stylesheet" type="text/css" />
    <%--<link href="~/styles/DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />--%>
    <title></title>
    <style type="text/css">
        P.breakhere {
            page-break-before: always;
        }
    </style>
    <style media="print">
        .hide_print {
            display: none;
        }
    </style>
</head>
<body>
    <form id="frmQuoteSummary" runat="server" style="font-family: Calibri; font-size: large">
        <div>
            <%--<h3 align="center" style="width:100%">--%>
            <div align="center" class="headline" runat="server" id="OldHeaderSection">
                <asp:Label ID="lblHeader" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label>
                <br />
                <asp:Label ID="lblSubHeader" runat="server" Text="PERSONAL AUTO INSURANCE QUOTE" Font-Bold="True"></asp:Label>
            </div>
            <%--</h3>--%>
            <%--<asp:LinkButton ID="lnkBtnPrint" runat="server" OnClientClick="javascript:window.print();" CssClass="hide_print">Print</asp:LinkButton>--%>
            <table style="width: 100%" runat="server" id="OldQuoteNumberSection">
                <tr>
                    <td style="width: 50%">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblQuoteNo" runat="server" Text="Quote#:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblQuoteNumber" runat="server"></asp:Label>
                                    <asp:Label ID="lblQuoteId" runat="server" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblToday" runat="server" Text="Today's Date:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblDate" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table style="width: 100%" runat="server" id="OldClientAndAgencySection">
                <tr>
                    <td>
                        <uc1:VR3Proposal_ClientAndAgencyInfo runat="server" ID="VR3Proposal_ClientAndAgencyInfo" />
                    </td>
                </tr>
            </table>
            <table style="width: 100%" runat="server" id="OldPremSection">
                <tr>
                    <td style="width: 50%">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPrem" runat="server" Text="ANNUAL PREMIUM:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblPremium" runat="server"></asp:Label>
                                </td>
                            </tr>
                            
                        </table>
                    </td>
                    <td>
                        <%--<asp:Label ID="lblExpirationText" runat="server" Text="Quote Expires 60 Days from Today's Date"></asp:Label>--%>
                    </td>
                </tr>
                <tr id="trAnnualPremiumWithFees">
                    <td style="width: 50%">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblAnnualPremiumWithFeesText" runat="server" Text="ANNUAL PREMIUM and Fees:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblAnnualPremiumWithFees" runat="server" Text="Payment Plan Type:"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">Annual Premium is impacted by chosen payment plan. Certain payment plans impact the annual premium.<br /><br /></td>
                </tr>
            </table>
            <table style="width: 100%" runat="server" id="OldPayPlanTypeSection">
                <tr>
                    <td style="width: 50%">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPPType" runat="server" Text="Payment Plan Type:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblPaymentPlanType" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table runat="server" id="OldEffDateSection" style="width: 100%">
                <tr>
                    <td style="width: 50%">
                        <asp:Label ID="lblEffective" runat="server" Text="Proposed Effective Date:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblEffectiveDate" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <uc1:ctlPFHeader runat="server" Visible="false" id="ctlEndorsementOrChangeHeader"></uc1:ctlPFHeader>
            <div id="quoteSummaryDetailsContent" runat="server">
            <br />
            <hr />
            <asp:Label ID="lblDriverInfo" runat="server" Text="Driver Information"></asp:Label>
            <hr />
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:DataGrid ID="dgDrivers" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None">
                            <ItemStyle CssClass="GridItem"></ItemStyle>
                            <HeaderStyle CssClass="GridHeader" HorizontalAlign="Left"></HeaderStyle>
                            <Columns>
                                <asp:BoundColumn DataField="DriverNum" SortExpression="DriverNum" HeaderText="Driver #"></asp:BoundColumn>
                                <asp:BoundColumn DataField="DriverName" SortExpression="DriverName" HeaderText="Driver Name" ItemStyle-Width="30%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="DOB" SortExpression="DOB" HeaderText="Date of Birth"></asp:BoundColumn>                                
                                <asp:BoundColumn DataField="VehicleNum" SortExpression="VehicleNum" HeaderText="Veh"></asp:BoundColumn>                                
                                <asp:BoundColumn DataField="Discounts" SortExpression="Discounts" HeaderText="Driver Discounts" ItemStyle-Width="40%"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <%--<p class="breakhere"></p>--%>
            <hr />
            <asp:Label ID="lblPolicyCoverages" runat="server" Text="Policy Coverages"></asp:Label>
            <hr />
            <table style="width: 100%">
                <tr>
                    <td style="width: 20%">
                        <asp:Label ID="lblAutoEnhance" runat="server" Text="Auto Enhancement"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblAutoEnhancePrem" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr id="trAutoPlusEnhance" runat="server">
                    <td style="width: 20%">
                        <asp:Label ID="lblAutoPlusEnhance" runat="server" Text="Auto Plus Enhancement"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblAutoPlusEnhancePrem" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <hr />
            <asp:Label ID="lblPolicyDiscounts" runat="server" Text="Policy Discounts"></asp:Label>
            <hr />
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Panel ID="pnlNoDiscounts" runat="server" Visible="false">
                            <asp:Label ID="lblNoDiscounts" runat="server" Text="No Records Exist"></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlMuliPolicy" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblMultPolicy" runat="server" Text="Multi-Policy"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblMultiPercent" runat="server" Text="10%"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlMultiLine" runat="server" Visible="false">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblMultiLine" runat="server" Text="Multi-Line"></asp:Label>
                                    </td>
                                    <td>                                        
                                        applied</td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlMarketCredit" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblCredit" runat="server" Text="Select Market Credit"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblCreditPercent" runat="server" Text="2%"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlStudent" runat="server" Visible="false">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblGoodDriver" runat="server" Text="Good Student"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblGoodPercent" runat="server" Text="10%"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlMuliVehicle" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblMultiVehicle" runat="server" Text="Multi-Vehicle"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblMultiVehPercent" runat="server" Text="17%"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlAdvancedQuoteDiscount" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblAdvancedQuoteDiscount" runat="server" Text="Advanced Quote Discount"></asp:Label>
                                    </td>
                                    <td>
                                        applied
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlPayPlanDiscount" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblPayPlanDiscount" runat="server" Text="Pay Plan Discount"></asp:Label>
                                    </td>
                                    <td>
                                        applied
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <br />
            <hr />
            <asp:Label ID="lblSurCharges" runat="server" Text="Policy Surcharges"></asp:Label>
            <hr />
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Panel ID="pnlNoSurchargeExist" runat="server" Visible="false">
                            <asp:Label ID="lblNoSurchargeExist" runat="server" Text="No Records Exist"></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlOOS" runat="server" Visible="false">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblOOS" runat="server" Text="Out of State"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblOOSPercent" runat="server" Text="10%"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlInexpDrvr" runat="server" Visible="false">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblInexpDrvr" runat="server" Text="Inexperienced Driver"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblInexpDrvrPercent" runat="server" Text="20%"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlDriverViolations" runat="server" Visible="false">
                            <table style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td>
                                        <asp:Panel ID="pnlAV" runat="server">
                                            <table>
                                                <tr>
                                                    <td style="vertical-align: top">
                                                        <asp:Label ID="lblViolation" runat="server" Text="Accidents/Violations History"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlViolations" runat="server">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="vertical-align: top; width: 100%">
                                                        <asp:DataGrid ID="dgViolations" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="False" GridLines="None" CssClass="quickQuoteGrid">
                                                            <ItemStyle VerticalAlign="Top"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                            <Columns>
                                                                <asp:BoundColumn DataField="TotalSurcharge" SortExpression="TotalSurcharge" HeaderText="Surcharge" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundColumn>
                                                                <asp:BoundColumn DataField="DriverNum" SortExpression="DriverNum" HeaderText="Driver #" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundColumn>
                                                                <asp:BoundColumn DataField="ViolationDate" SortExpression="ViolationDate" HeaderText="Violation History"></asp:BoundColumn>
                                                            </Columns>
                                                        </asp:DataGrid>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlSurchargeMsg" runat="server" Visible="false">
                            <asp:Label ID="lblSurchargeMsg" runat="server" Text="* Prior to 03/24/2014 two minor accidents/violations per operator equal one major surchargeable event."></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlChildRestraintSurchargeMsg" runat="server" Visible="false">
                            <asp:Label ID="lblChildRestraintSurchargeMsg" runat="server" Text="** After to 03/24/2014 two Child Restraint violations per operator equal one major surchargeable event."></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlAccidents" runat="server" Visible="false">
                            <hr id="hrLine" runat="server" visible="false" />
                            <table style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td>
                                        <asp:Panel ID="pnlAccidentHdr" runat="server">
                                            <table>
                                                <tr>
                                                    <td style="vertical-align: top">
                                                        <asp:Label ID="lblAccidents" runat="server" Text="Loss History"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlAccidentDetails" runat="server">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="vertical-align: top; width: 20%">
                                                        <asp:DataGrid ID="dgAccSurcharge" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="False" GridLines="None" CssClass="quickQuoteGrid">
                                                            <ItemStyle VerticalAlign="Top"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                            <Columns>
                                                                <asp:BoundColumn DataField="TotalSurcharge" SortExpression="TotalSurcharge" HeaderText="Surcharge" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundColumn>
                                                                <asp:BoundColumn DataField="DriverNum" SortExpression="DriverNum" HeaderText="Driver #" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundColumn>
                                                                <asp:BoundColumn DataField="LossDate" SortExpression="LossDate" HeaderText="Loss History"></asp:BoundColumn>
                                                            </Columns>
                                                        </asp:DataGrid>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <%--<p class="breakhere"></p>--%>
            <uc1:VR3QuoteSummaryVehicleList runat="server" ID="VR3QuoteSummaryVehicleList" />
            <p class="breakhere"></p>
            <asp:Panel ID="pnlPaymentOptions" runat="server">
                <uc1:VR3Proposal_PaymentOptions runat="server" ID="VR3Proposal_PaymentOptions" />
            </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>