<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PFQuoteSummary_DFR.aspx.vb" Inherits="IFM.VR.Web.PFQuoteSummary_DFR" %>

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
<body>
    <form id="frmQuoteSummary" runat="server" style="font-family: Calibri; font-size: large">
        <div>
            <%--<h3 align="center" style="width:100%">--%>
            <div id="QuoteAppPrintSection" runat="server">
                <div align="center" class="headline">
                    <asp:Label ID="lblHeader" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label>
                    <br />
                    <asp:Label ID="lblSubHeader" runat="server" Text="DWELLING FIRE NEW BUSINESS QUOTE" Font-Bold="True"></asp:Label>
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
                                            <asp:Label ID="lblRatedTier" Visible="False" runat="server" Text="Rated Tier: "></asp:Label>
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
                        <asp:Label ID="lblResidence" runat="server" Text="Covered Premises:"></asp:Label>
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
                        <asp:Label ID="lblVMM" runat="server" Text="VMM Included"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblVMMData" runat="server" Text="No"></asp:Label>
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
                        <table style="width: 100%" class="table">
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
                    </td>
                </tr>
            </table>
            <br />
            <%--Included Coverages--%>
            <table style="width: 100%" class="table">
                <tr>
                    <td>
                        <asp:Label ID="lblIncludedCoverage" runat="server" Text="Form Type Coverages" Font-Bold="true" Font-Underline="true"></asp:Label>
                        <table id="pnlCovAEC" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovAEC" runat="server" Text="Cov A - Dwelling EC Base Fire Premium"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovAEC_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovAEC_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovAVMM" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovAVMM" runat="server" Text="Cov A - Dwelling V&MM Base Premium"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovAVMM_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovAVMM_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovBEC" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovBEC" runat="server" Text="Cov B - Other Structures EC"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovBEC_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovBEC_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovBVMM" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovBVMM" runat="server" Text="Cov B - Other Structures V&MM"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovBVMM_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovBVMM_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovCEC" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovCEC" runat="server" Text="Cov C - Personal Property EC"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovCEC_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovCEC_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovCVMM" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovCVMM" runat="server" Text="Cov C - Personal Property V&MM"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovCVMM_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovCVMM_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovDEC" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovDEC" runat="server" Text="Cov D - Additional Living Cost/Fair Rental Value EC"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovDEC_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovDEC_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlCovDVMM" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCovDVMM" runat="server" Text="Cov D - Additional Living Cost/Fair Rental Value V&MM"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblCovDVMM_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblCovDVMM_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <p class="breakhere" runat="server" id="LiabBreak" visible="false"></p>
            <br id="Br1" runat="server" />
            <table id="pnlLiabilityHdr" runat="server" style="width: 100%" class="table">
                <tr>
                    <td style="width: 70%">
                        <asp:Label ID="Label1" runat="server" Text="Liability Coverages" Font-Bold="true" Font-Underline="true"></asp:Label>
                    </td>
                    <td>
                        <table style="width: 100%" class="table">
                            <tr>
                                <td align="right" style="width: 41.5%">
                                    <asp:Label ID="lblLiabLimit" runat="server" Text="Limits" Font-Bold="true"></asp:Label>

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
                        <asp:Label ID="lblCovE" runat="server" Text="Coverage L - Personal Liability"></asp:Label>
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
                        <asp:Label ID="lblCovF" runat="server" Text="Coverage M - Medical Payments"></asp:Label>
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
            <p class="breakhere" runat="server" id="PropBreak" visible="false"></p>
            <br id="PropLine" runat="server" />
            <%--Optional Coverages--%>
            <table style="width: 100%" class="table">
                <tr>
                    <td>
                        <table id="pnlOtherPropCoverageHdr" runat="server" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblPropOtherCoverage" runat="server" Text="Other Property Coverages" Font-Bold="true" Font-Underline="true"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblPropLimitsHdr" Text="Limits" Visible="False" runat="server" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblPropPremHdr" Text="Premium" runat="server" Font-Bold="true"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlNoSelectedCoverages" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="Label2" runat="server" Text="N/A"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="Label3" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="Label4" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_132_HO" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl92_132_HO" runat="server" Text="Equipment Breakdown Coverage (92-132 HO)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl92_132_HO_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl92_132_HO_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_290_92_195" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_290_92_195" runat="server" Text="Personal Property Replacement Cost"></asp:Label>
                                    <asp:Label ID="lblPPFormNum" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_290_92_195_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_290_92_195_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_267" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl92_267" runat="server" Text="Homeowner Enhancement Endorsement (92-267)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl92_267_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl92_267_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_173" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl92_173" runat="server" Text="Backup of Sewer or Drain (92-173)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl92_173_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl92_173_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl29_034" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl29_034" runat="server" Text="Cov. A - Specified Additional Amount of Insurance (29-034)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl29_034_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl29_034_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_315B" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_315B" runat="server" Text="Earthquake"></asp:Label>
                                    <asp:Label ID="lblEarthFormNum" runat="server"></asp:Label>
                                    <asp:Label ID="lblHO_315B_Ded" runat="server"></asp:Label>
                                    <asp:Label ID="lblHO_315B_Deductible" runat="server" Text="Deductible"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_315B_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_315B_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_04_81" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_04_81" runat="server" Text="Actual Cash Value Loss Settlement (DP 04 75)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_04_81_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_04_81_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_04_93" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_04_93" runat="server" Text="Actual Cash Value Loss Settlement/Windstorm or Hail to Roof Surfacing (HO-04 93)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_04_93_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_04_93_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_05_30" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_05_30" runat="server" Text="Functional Replacement Cost Loss Settlement (HO-05 30)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_05_30_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_05_30_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_51" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_51" runat="server" Text="Building Additions and Alterations (HO-51)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_51_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_51_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_35" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_35" runat="server" Text="Loss Assessment (HO-35)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_35_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_35_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_35B" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_35B" runat="server" Text="Loss Assessment - Earthquake (HO-35B)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_35B_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_35B_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_074A" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl92_074A" runat="server" Text="Mine Subsidence Cov. A (92-074)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl92_074A_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl92_074A_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_074AB" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl92_074AB" runat="server" Text="Mine Subsidence Cov. A & B (92-074)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl92_074AB_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl92_074AB_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_99" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_99" runat="server" Text="Sinkhole Colapse (HO-99)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_99_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_99_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_127" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td colspan="2" style="width: 550px; vertical-align: top">
                                    <asp:Label ID="lbl92_127" runat="server" Text="Specified Other Structures - Off Premises (92-127)"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 1%"></td>
                                <td style="vertical-align: top">
                                    <asp:DataGrid ID="dgIncreasedLimits" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="false" Width="100%">
                                        <Columns>
                                            <asp:BoundColumn DataField="StructAddress" SortExpression="StructAddress" ItemStyle-Width="55%"></asp:BoundColumn>
                                            <asp:BoundColumn DataField="IncreasedLimit" SortExpression="IncreasedLimit" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15%"></asp:BoundColumn>
                                            <asp:BoundColumn DataField="Premium" SortExpression="Premium" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15%"></asp:BoundColumn>
                                        </Columns>
                                    </asp:DataGrid>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_32" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_32" runat="server" Text="Unit-Owners Coverage A (HO-32)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_32_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_32_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnl92_367" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lbl92_367" runat="server" Text="Theft of Building Materials (92-367)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lbl92_367_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lbl92_367_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <table id="pnlML_25" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblML_25" runat="server" Text="Consent to Move Mobile Home (ML-25)"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblML_25_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblMl_25_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_50_ML_66" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_50_ML_66" runat="server" Text="Personal Property - Other Residence"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblHO_50_ML_66_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_50_ML_66_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlHO_314" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblHO_314" runat="server" Text="Special Computer Coverage"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="right" style="width: 41.5%" class="table">
                                                <asp:Label ID="lblHO_314_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblHO_314_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlML_26" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblML_26" runat="server" Text="Trip Collision"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="right" style="width: 41.5%" class="table">
                                                <asp:Label ID="lblML_26_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblML_26_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="pnlML_27" runat="server" visible="false" style="width: 100%" class="table">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblML_27" runat="server" Text="Vendor's Single Interest"></asp:Label>
                                </td>
                                <td>
                                    <table style="width: 100%" class="table">
                                        <tr>
                                            <td align="right" style="width: 41.5%">
                                                <asp:Label ID="lblML_27_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Label ID="lblML_27_Prem" runat="server"></asp:Label>
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
            <div runat="server" id="divOtherLiabilityCoverages">
                <table style="width: 100%" class="table">
                    <tr>
                        <td>
                            <table id="tblOptionLiability" runat="server" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="Label5" runat="server" Text="Other Liability Coverages" Font-Bold="true" Font-Underline="true"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="Label6" Text="Limits" Visible="False" runat="server" Font-Bold="true"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="Label7" Text="Premium" runat="server" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="pnlNonOwnerOccupiedDwelling" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="Label8" runat="server" Text="Non-Owner Occupied Dwelling"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <sup>See 2</sup>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblNonOwnerOccupiedDwellingPremium" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <p class="breakhere" runat="server" id="AdjBreak" visible="false"></p>
            <hr />
            <asp:Label ID="lblCreditSurcharges" runat="server" Text="POLICY CREDITS"></asp:Label>
            <hr />
            <table id="pnlCreditSurcharges" runat="server" style="width: 100%" class="table">
                <tr>
                    <td style="vertical-align: top; width: 100%">
                        <asp:Label ID="lblCredits" runat="server" Text="Credits Applied" Visible="False" Font-Bold="true" Font-Underline="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; width: 100%">
                        <asp:Label ID="lblNoCredits" runat="server" Text="No Records Exist" Visible="false"></asp:Label>
                        <asp:DataGrid ID="dgCreditList" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="False">
                            <Columns>
                                <asp:BoundColumn DataField="Credit" SortExpression="Credit" ItemStyle-Width="100%" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CreditPercent" SortExpression="CreditPercent" ItemStyle-Width="100%" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
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
                        <asp:Label ID="lblSupTwo" runat="server" Text="<sup>2</sup>  Coverage E and F limits are extended to this endorsement"></asp:Label>
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
