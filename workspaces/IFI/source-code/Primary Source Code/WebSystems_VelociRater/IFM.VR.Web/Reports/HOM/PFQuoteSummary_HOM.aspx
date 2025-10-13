<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PFQuoteSummary_HOM.aspx.vb" Inherits="IFM.VR.Web.PFQuoteSummary_HOM" %>

<%@ Register Src="~/Reports/ctlPaymentOptions.ascx" TagPrefix="uc1" TagName="ctlPaymentOptions" %>
<%@ Register Src="~/Reports/ctlClientAndAgencyInfo.ascx" TagPrefix="uc1" TagName="ctlClientAndAgencyInfo" %>
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
    <style type="text/css">
        .table {
            border-collapse: collapse;
        }

            .table td {
                margin: 0;
                padding: 0;
            }
        
    </style>
</head>
<body runat="server" class="whiteBackground">
    <form id="frmQuoteSummary" runat="server" style="font-family: Calibri; font-size: large">
        <div>
            <%--<h3 align="center" style="width:100%">--%>
            <div id="QuoteAppPrintSection" runat="server">
                <div align="center" class="headline">
                    <asp:Label ID="lblHeader" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label>
                    <br />
                    <asp:Label ID="lblSubHeader" runat="server" Text="HOMEOWNERS INSURANCE NEW BUSINESS QUOTE" Font-Bold="True"></asp:Label>
                </div>
                <table style="width: 100%; border: solid; border-width: thin" class="table">
                    <tr>
                        <td>
                            <asp:Label ID="lblCaution1" runat="server" Text="Premiums shown are estimates only. They are subject to change based on information that we may receive later in the application process. Quoted premium does not include any service charges that may be applied for monthly, quarterly or semi-annual payment plans. This quote is subject to underwriting approval."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                            <asp:Label ID="lblCaution2" runat="server" Text="This is a proposal to provide insurance coverage. This is not a contract of insurance." Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" class="table">
                    <tr>
                        <td style="width: 50%">
                            <asp:Panel ID="pnlDate" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblToday" runat="server" Text="Today's Date:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblDate" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Panel ID="pnlQuote" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQuoteNo" runat="server" Text="Quote#:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQuoteNumber" runat="server"></asp:Label>
                                            <asp:Label ID="lblQuoteId" runat="server" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" class="table">
                    <tr>
                        <td>
                            <uc1:ctlClientAndAgencyInfo runat="server" ID="ctlClientAndAgencyInfo" />
                        </td>
                    </tr>
                </table>
                <table style="width: 100%; border: solid; border-width: thin" class="table">
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
                </table>
                <br />
                <table style="width: 100%; border: solid; border-width: thin" class="table">
                    <tr>
                        <td style="width: 50%">
                            <asp:Panel ID="pnlEffectiveDate" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEffective" runat="server" Text="Proposed Effective Date:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblEffectiveDate" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Panel ID="pnlTier" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblRatedTier" runat="server" Text="Rated Tier: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTier" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="EndorsementPrintSection" runat="server" width="100%" style="background-color:#FFFFFF;">
                <uc1:ctlPFHeader runat="server" Visible="false" id="ctlEndorsementOrChangeHeader"></uc1:ctlPFHeader>               
            </div>
            <div id="quoteSummaryDetailsContent" runat="server">
            <br />
            <hr />
            <asp:Label ID="lblPropertyInfo" runat="server" Text="PROPERTY COVERAGE LIMITS AND PREMIUMS"></asp:Label>
            <hr />
            <table class="table">
                <tr>
                    <td>
                        <asp:Label ID="lblResidence" runat="server" Text="Residence Premises:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblResidenceData" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblForm" runat="server" Text="Form Number"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblFormData" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr id="trOccupancyCode" runat="server">
                    <td>
                        <asp:Label ID="lblOccupancyCode" runat="server" Text="Occupancy Type"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblOccupancyCodeData" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDeductible" runat="server" Text="Policy Deductible"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblDeductibleData" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblWindHail" runat="server" Text="Wind/Hail"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblWindHailData" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblConstruction" runat="server" Text="Construction"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblConstructionData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <table style="width: 100%" class="table">
                <tr>
                    <td style="vertical-align: top; width: 50%">
                        <table id="pnlCoverageHdr" runat="server" style="width: 100%" class="table">
                            <tr style="vertical-align: bottom">
                                <td style="width: 50%">
                                    <asp:Label ID="lblPropCoverage" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr style="vertical-align: bottom">
                                            <td align="right" style="width: 65%">
                                                <asp:Label ID="lblPropLimit" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                                                <%--<asp:Label ID="lblPropSup" runat="server" Text="<sup>1</sup>" Font-Size="Medium"></asp:Label>--%>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblPropPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovADwelling" runat="server" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 50%">
                                    <asp:Label ID="lblCovA" runat="server" Text="Coverage A - Dwelling"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 65%">
                                                <asp:Label ID="lblCovALimit" runat="server"></asp:Label>
                                                <asp:Label ID="lblCovASup" runat="server" Text="<sup>1</sup>"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovAPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovBStruct" runat="server" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 50%">
                                    <asp:Label ID="lblCovB" runat="server" Text="Coverage B - Other Structures"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 65%">
                                                <asp:Label ID="lblCovBLimit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovBPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table style="width: 100%" class="table">
                            <tr>
                                <td style="width: 50%">
                                    <asp:Label ID="lblCovC" runat="server" Text="Coverage C - Personal Property"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 65%">
                                                <asp:Label ID="lblCovCLimit" runat="server"></asp:Label>
                                                <asp:Label ID="lblCovCSup" runat="server" Text="<sup>1</sup>" Visible="false"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovCPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table style="width: 100%" class="table" runat="server" id="pnlCovD">
                            <tr>
                                <td style="width: 50%">
                                    <asp:Label ID="lblCovD" runat="server" Text="Coverage D - Loss Of Use"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 65%">
                                                <asp:Label ID="lblCovDLimit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovDPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlMinPremAdj" runat="server" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 50%">
                                    <asp:Label ID="lblMinPremAdj" runat="server" Text="Minimum Premium Adjustment"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 65%">
                                                <asp:Label ID="lblMinPremAdjLimit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblMinPremAdjPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <%--Included Coverages--%>
            <span style="font-weight:bold; text-decoration:underline;">Included Coverages</span>
            <br />
            <asp:Literal ID="litIncludedCoverages" runat="server"></asp:Literal>

           
            <p class="breakhere" runat="server" id="PropBreak" visible="false"></p>
            <br id="PropLine" runat="server" />
            <%--Optional Coverages--%>

            <table id="Table1" runat="server" style="width: 100%;" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="Label1" runat="server" Text="Other Property Coverages" Font-Bold="true" Font-Underline="true"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="Label2" Text="Limits" runat="server" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="Label3" Text="Premium" runat="server" Font-Bold="true"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
            <asp:Literal ID="litOtherPropertyCoverages" runat="server"></asp:Literal>

          
            <br />
            <p class="breakhere" runat="server" id="LiabBreak" visible="false"></p>
            <hr />
            <asp:Label ID="lblLiabilityCoverage" runat="server" Text="LIABILITY COVERAGE LIMITS AND PREMIUMS"></asp:Label>
            <hr />
            <table id="pnlLiabilityHdr" runat="server" style="width: 100%" class="table">
                <tr>
                    <td style="width: 70%">
                        <asp:Label ID="lblLiabCoverage" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
                    </td>
                    <td>
                        <table style="width: 100%" class="table">
                            <tr>
                                <td align="right" style="width: 41.5%">
                                    <asp:Label ID="lblLiabLimit" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                                    <asp:Label ID="lblLiabSup" runat="server" Text="<sup>3</sup>"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblLiabPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table id="pnlCovE" runat="server" style="width: 100%" class="table">
                <tr>
                    <td style="width: 70%">
                        <asp:Label ID="lblCovE" runat="server" Text="Coverage E - Personal Liability"></asp:Label>
                    </td>
                    <td>
                        <table style="width: 100%" class="table">
                            <tr>
                                <td align="right" style="width: 41.5%">
                                    <asp:Label ID="lblCovELimit" runat="server"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblCovEPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table id="pnlCovF" runat="server" style="width: 100%" class="table">
                <tr>
                    <td style="width: 70%">
                        <asp:Label ID="lblCovF" runat="server" Text="Coverage F - Medical Payments"></asp:Label>
                    </td>
                    <td>
                        <table style="width: 100%" class="table">
                            <tr>
                                <td align="right" style="width: 41.5%">
                                    <asp:Label ID="lblCovFLimit" runat="server"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblCovFPrem" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
               <br />
            <span style="font-weight:bold; text-decoration:underline;">Other Liability Coverages</span>
            <br />
            <asp:Literal ID="litOtherLiabilityCoverages" runat="server"></asp:Literal>

            <p class="breakhere" runat="server" id="IMBreak" visible="false"></p>
            <hr />
            <asp:Label ID="lblInlandMarine" runat="server" Text="INLAND MARINE"></asp:Label>
            <hr />
            <asp:Label ID="lblNoIMExist" runat="server" Text="N/A"></asp:Label>
            <table id="pnlInlandMarine" runat="server" visible="false" style="width: 100%" class="table">
                <tr>
                    <td colspan="2" style="vertical-align: top">
                        <asp:DataGrid ID="dgInlandMarine" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="True">
                            <Columns>
                                <asp:BoundColumn DataField="Coverage" SortExpression="Coverage" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top" HeaderText="Category" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Description" SortExpression="Description" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top" HeaderText="Description" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Deductible" SortExpression="Deductible" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px" ItemStyle-VerticalAlign="Top" HeaderText="Deductible" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Limits" SortExpression="Limits" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" HeaderText="Limits <sup>3</sup>" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Premium" SortExpression="Premium" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" HeaderText="Premium" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table id="pnlAdditionalPrem" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td>
                                    <asp:Label ID="lblMeetMin" runat="server" Text="Additional Amount to Meet Minimum Premium"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblMeetMinPrem" runat="server" Style="margin-right: 1px;"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr style="border-color: black" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTotalIMPremium" runat="server" Text="Total Inland Marine Premium"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblTotalIMPremData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <p class="breakhere" runat="server" id="RVWBreak" visible="false"></p>
            <hr />
            <asp:Label ID="lblRVWatercraft" runat="server" Text="RECREATIONAL VEHICLES AND WATERCRAFT"></asp:Label>
            <hr />
            <asp:Label ID="lblNoRVWaterExist" runat="server" Text="N/A"></asp:Label>
            <table id="pnlRVWatercraft" runat="server" visible="false" style="width: 100%" class="table">
                <tr>
                    <td colspan="2" style="vertical-align: top">
                        <asp:DataGrid ID="dgRVWatercraft" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="True">
                            <Columns>
                                <asp:BoundColumn DataField="Coverage" SortExpression="Coverage" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top" HeaderText="Category" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CoverageOpt" SortExpression="CoverageOpt" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top" HeaderText="Coverage Options" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Deductible" SortExpression="Deductible" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px" ItemStyle-VerticalAlign="Top" HeaderText="Deductible" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Limits" SortExpression="Limits" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" HeaderText="Limits <sup>3</sup>" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Premium" SortExpression="Premium" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" HeaderText="Premium" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr style="border-color: black" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTotalRVWatercrarftPremium" runat="server" Text="Total RV/Watercraft Premium"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblTotalRVWatercraftPremData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <p class="breakhere" runat="server" id="AdjBreak" visible="false"></p>
            <hr />
            <asp:Label ID="lblCreditSurcharges" runat="server" Text="POLICY CREDITS AND SURCHARGES"></asp:Label>
            <hr />
            <table id="pnlCreditSurcharges" runat="server" style="width: 100%" class="table">
                <tr>
                    <td style="vertical-align: top; width: 50%">
                        <asp:Label ID="lblCredits" runat="server" Text="Credits Applied" Font-Bold="true" Font-Underline="true"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblSurcharges" runat="server" Text="Surcharges Applied" Font-Bold="true" Font-Underline="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; width: 50%">
                        <asp:Label ID="lblNoCredits" runat="server" Text="No Records Exist" Visible="false"></asp:Label>
                        <asp:DataGrid ID="dgCreditList" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="False">
                            <Columns>
                                <asp:BoundColumn DataField="Credit" SortExpression="Credit" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CreditPercent" SortExpression="CreditPercent" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                    <td style="vertical-align: top">
                        <asp:Label ID="lblNoSurchargeExist" runat="server" Text="No Records Exist" Visible="false"></asp:Label>
                        <asp:DataGrid ID="dgSurchargeList" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="False">
                            <Columns>
                                <asp:BoundColumn DataField="Surcharge" SortExpression="Surcharge" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SurchargePercent" SortExpression="SurchargePercent" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; width: 50%"></td>
                    <td></td>
                </tr>
            </table>
            <br />
            <p class="breakhere" runat="server" id="SuperBreak" visible="false"></p>
            <table style="width: 100%" class="table">
                <tr>
                    <td>
                        <asp:Label ID="lblSupOne" runat="server" Text="<sup>1</sup> Limit values are rounded to the next highest 1000"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSupTwo" runat="server" Text="<sup>2</sup> Actual loss sustained within 12 months of a covered loss"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSupThree" runat="server" Text="<sup>3</sup> Limit values are rounded to the next highest 100"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSupFour" runat="server" Text="<sup>4</sup> Coverage E and F limits are extended to this endorsement"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSupFive" runat="server" Text="<sup>5</sup> Coverage E limit is extended to this endorsement"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSupSix" runat="server" Text="<sup>6</sup> Deductible for this coverage is always $500"></asp:Label>
                    </td>
                </tr>
            </table>
            <p class="breakhere"></p>
            <asp:Panel ID="pnlPaymentOptions" runat="server">
                <uc1:ctlPaymentOptions runat="server" ID="ctlPaymentOptions" />
            </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>