<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PFQuoteSummary_FAR.aspx.vb" Inherits="IFM.VR.Web.PFQuoteSummary_FAR" %>

<%@ Register Src="~/Reports/ctlPaymentOptions.ascx" TagPrefix="uc1" TagName="ctlPaymentOptions" %>
<%@ Register Src="~/Reports/ctlClientAndAgencyInfo.ascx" TagPrefix="uc1" TagName="ctlClientAndAgencyInfo" %>
<%@ Register Src="~/Reports/FAR/ctlDwelling.ascx" TagPrefix="uc1" TagName="ctlDwelling" %>
<%@ Register Src="~/Reports/FAR/ctlDwellingList.ascx" TagPrefix="uc1" TagName="ctlDwellingList" %>
<%@ Register Src="~/Reports/FAR/ctlBarnsBuildingsList.ascx" TagPrefix="uc1" TagName="ctlBarnsBuildingsList" %>
<%@ Register Src="~/Reports/FAR/ctlPersonalPropertyList.ascx" TagPrefix="uc1" TagName="ctlPersonalPropertyList" %>
<%@ Register Src="~/Reports/FAR/ctlPeakSeasons.ascx" TagPrefix="uc1" TagName="ctlPeakSeasons" %>
<%@ Register Src="~/Reports/FAR/ctlPageBreak.ascx" TagPrefix="uc1" TagName="ctlPageBreak" %>
<%@ Register Src="~/Reports/ctlPFHeader.ascx" TagPrefix="uc1" TagName="ctlPFHeader" %>
<%@ Register Src="~/Reports/FAR/ctlFarmIncidentalLimits.ascx" TagPrefix="uc1" TagName="ctlFarmIncidentalLimits" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/styles/vr.css" rel="stylesheet" type="text/css" />
    <%--<link href="~/styles/DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />--%>
    <title></title>
    <style type="text/css">
        P.farmbreakhere {
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

        .AddlCoverageTitleColumn {
            width: 520px;
            text-align: left;
            vertical-align: top;
        }

        .AddlCoverageDescColumn {
            width: 530px;
            text-align: left;
            vertical-align: top;
        }

        .AddlCoverageLimitColumn {
            width: 160px;
            text-align: right;
            vertical-align: top;
        }

        .AddlCoveragePremColumn {
            width: 240px;
            text-align: right;
            vertical-align: top;
        }

        .GL9TitleColumn {
            width: 331px;
            text-align: left;
            vertical-align: top;
        }

        .GL9DescColumn {
            width: 385px;
            text-align: left;
            vertical-align: top;
        }

        .GL9LimitColumn {
            width: 80px;
            text-align: right;
            vertical-align: top;
        }

        .GL9PremColumn {
            width: 129px;
            text-align: right;
            vertical-align: top;
        }
    </style>
</head>
<body>
    <form id="frmQuoteSummary" runat="server" style="font-family: Calibri; font-size: large">
        <div>
            <div id="QuoteAppPrintSection" runat="server">
                <%--<h3 align="center" style="width:100%">--%>
                <%--<div align="center" class="headline">--%>
                <table style="width: 100%" class="headline">
                    <tr>
                        <td style="min-width: 23%; width: 23%"></td>
                        <td align="center">
                            <asp:Label ID="ifmHeader" runat="server" Text="Indiana Farmers Mutual Insurance Group"></asp:Label>
                        </td>
                        <td style="min-width: 35%; width: 35%"></td>
                    </tr>
                    <%--            </table>
                <table style="width:100%">--%>
                    <tr>
                        <td style="min-width: 23%; width: 23%"></td>
                        <td align="center">
                            <%--                        <table style="width:100%">
                                <tr align="center">
                                    <td>--%>
                            <asp:Label ID="lblSubHeader" runat="server" Text="FARM INSURANCE NEW BUSINESS " Font-Bold="True"></asp:Label>
                            <asp:Label ID="lblHeader" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <%--                            </tr>
                            </table>
                            </td>--%>
                        <td align="right" style="min-width: 35%; width: 35%">
                            <asp:Label ID="lblPolicyType" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>
                <%--</div>--%>
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
                        <td style="width: 50%; vertical-align: top">
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
                            <asp:DataGrid ID="dgApplicants" runat="server" HorizontalAlign="Left" AutoGenerateColumns="false" GridLines="None">
                                <ItemStyle CssClass="GridItem"></ItemStyle>
                                <HeaderStyle CssClass="GridHeader" HorizontalAlign="Left"></HeaderStyle>
                                <Columns>
                                    <asp:BoundColumn DataField="ApplicantNum" SortExpression="ApplicantNum" HeaderText="Applicant #"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="ApplicantName" SortExpression="ApplicantName" HeaderText="Applicant Name" ItemStyle-Width="50%"></asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>
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
                                            <asp:Label ID="lblExpirationText" runat="server" Text="Quote Expires 60 Days from Today's Date"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>

            <div id="EndorsementPrintSection" runat="server" width="100%" style="background-color: #FFFFFF;">
                <uc1:ctlPFHeader runat="server" Visible="false" ID="ctlEndorsementOrChangeHeader"></uc1:ctlPFHeader>
            </div>

            <br />
            <hr />
            <table style="width: 100%" class="table">
                <tr>
                    <td>
                        <asp:Label ID="lblLocationSummary" runat="server" Text="FARM LOCATION SUMMARY"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblTotalAcreage" runat="server" Text="Total Acreage: "></asp:Label>
                        <asp:Label ID="lblTotalAcreageData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <hr />
            <table style="width: 100%" class="table">
                <tr>
                    <td colspan="2">
                        <asp:DataGrid ID="dgLocationSummary" runat="server" HorizontalAlign="Left" AutoGenerateColumns="false" GridLines="None">
                            <ItemStyle CssClass="GridItem"></ItemStyle>
                            <HeaderStyle CssClass="GridHeader" HorizontalAlign="Left"></HeaderStyle>
                            <Columns>
                                <asp:BoundColumn DataField="LocationNum" SortExpression="LocationNum" HeaderText="#" ItemStyle-Width="100px"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LocationAddress" SortExpression="LocationAddress" HeaderText="Location Address" ItemStyle-Width="1000px"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LocationAcreage" SortExpression="LocationAcreage" HeaderText="Acreage"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LocationDesc" SortExpression="LocationDesc" ItemStyle-Width="1000px"></asp:BoundColumn>
                                <asp:BoundColumn DataField="AcreageOnly" SortExpression="AcreageOnly" HeaderText="Location Type" ItemStyle-Width="500px"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
            </table>
            <div id="dvLiability" runat="server">
                <br />
                <hr />
                <asp:Label ID="lblLiabilityHdr" runat="server" Text="LIABILITY COVERAGE LIMITS AND PREMIUMS"></asp:Label>
                <hr />
                <table id="tblLiability" runat="server" style="width: 100%" class="table">
                    <tr style="vertical-align: bottom">
                        <td style="width: 520px">
                            <asp:Label ID="lblLiabilityCov" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
                        </td>
                        <td style="width: 530px"></td>
                        <td align="right" style="width: 160px">
                            <asp:Label ID="lblLiability_Limits" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                        </td>
                        <td align="right" style="width: 240px">
                            <asp:Label ID="lblLiability_Prem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" class="table">
                    <tr>
                        <td style="width: 1050px">
                            <asp:Label ID="lblCovL" runat="server" Text="Coverage L - Liability"></asp:Label>
                        </td>
                        <td align="right" style="width: 160px">
                            <asp:Label ID="lblCovLLimit" runat="server"></asp:Label>
                        </td>
                        <td align="right" style="width: 240px">
                            <asp:Label ID="lblCovLPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <%--                </table>
                <table style="width: 100%" class="table">--%>
                    <tr>
                        <td style="width: 1050px">
                            <asp:Label ID="lblCovM" runat="server" Text="Coverage M - Medical Payments"></asp:Label>
                        </td>
                        <td align="right" style="width: 160px">
                            <asp:Label ID="lblCovMLimit" runat="server"></asp:Label>
                        </td>
                        <td align="right" style="width: 240px">
                            <asp:Label ID="lblCovMPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trStopGapRow" runat="server">
                        <td id="tdStopGapTitleColumn" runat="server" style="width: 1050px">
                            <asp:Label ID="lblStopGapTitle" runat="server" Text="Stop Gap (OH)"></asp:Label>
                        </td>
                        <td id="tdStopGapLimitColumn" runat="server" style="width: 160px; text-align: right;">
                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                        </td>
                        <td id="tdStopGapPremiumColumn" runat="server" style="width: 240px; text-align: right;">
                            <asp:Label ID="lblStopGapPremium" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <%--<tr id="trStopGapRow" runat="server" style="display:none;">
                        <td id="tdStopGapTitleColumn" runat="server" style="width: 1050px">
                            <asp:Label ID="lblStopGapTitle" runat="server" Text="Stop Gap (OH)"></asp:Label>
                        </td>
                        <td id="tdStopGapLimitColumn" runat="server" style="width:160px;text-align:right;">
                            <asp:Label ID="Label3" runat="server" Text="hello"></asp:Label>
                        </td>
                        <td id="tdStopGapPremiumColumn" runat="server" style="width:240px;text-align:right;">
                            <asp:Label ID="lblStopGapPremium" runat="server"></asp:Label>
                        </td>
                    </tr>--%>
                    <tr>
                        <td colspan="3">
                            <hr style="border-color: black" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTotalLiability" runat="server" Text="Total Liability Premium"></asp:Label>
                        </td>
                        <td></td>
                        <td align="right">
                            <asp:Label ID="lblTotalLiab_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="dvDwellings" runat="server">
                <br />
                <hr />
                <asp:Label ID="lblPropertyInfo" runat="server" Text="PROPERTY COVERAGE LIMITS AND PREMIUMS"></asp:Label>
                <hr />
                <%--Primary Dwelling--%>
                <asp:Label ID="lblPrimDwelling" runat="server" Text="" Font-Bold="true"></asp:Label>
                <uc1:ctlDwelling runat="server" ID="ctlDwelling" />
                <p class="farmbreakhere" runat="server" id="AddlDwellingBreak" visible="false"></p>
                <div id="dvAddlDwelling" runat="server">
                    <br />
                    <%--Additional Dwellings--%>
                    <uc1:ctlDwellingList runat="server" ID="ctlDwellingList" />
                </div>
            </div>
            <p class="farmbreakhere" runat="server" id="BuildingsBreak" visible="false"></p>
            <div id="dvBuilding" runat="server">
                <hr />
                <%--Barns and Buildings--%>
                <asp:Label ID="lblBarnsBldngs" runat="server" Text="BARNS AND BUILDINGS"></asp:Label>
                <hr />
                <asp:Label ID="lblNoBuildingsExist" runat="server" Text="N/A"></asp:Label>
                <uc1:ctlBarnsBuildingsList runat="server" ID="ctlBarnsBuildingsList" />
            </div>
            <p class="farmbreakhere" runat="server" id="PersonalPropBreak" visible="false"></p>
            <div id="dvPersProp" runat="server">
                <%--<br />--%>
                <hr />
                <asp:Label ID="lblPersonalProperty" runat="server" Text="FARM PERSONAL PROPERTY"></asp:Label>
                <hr />
                <asp:Label ID="lblNoPersPropExist" runat="server" Text="N/A"></asp:Label>
                <uc1:ctlPersonalPropertyList runat="server" ID="ctlPersonalPropertyList" />
                <br />
            </div>

            <p class="farmbreakhere" runat="server" id="FarmIncidentalLimitsBreak" visible="false"></p>
            <div id="dvFarmIncidentalLimits" runat="server">
                <%--<br />--%>
                <hr />
                <asp:Label ID="lblFarmIncidentalLimits" runat="server" Text="FARM INCIDENTAL LIMITS"></asp:Label>
                <hr />
                <asp:Label ID="lblNoFarmIncidentalLimitsExist" runat="server" Text="N/A"></asp:Label>
                <uc1:ctlFarmIncidentalLimits runat="server" id="ctlFarmIncidentalLimits" />
                <br />
            </div>

            <div id="dvInlandMarine" runat="server">
                <p class="farmbreakhere" runat="server" id="IMBreak" visible="false"></p>
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
                                    <asp:BoundColumn DataField="Limits" SortExpression="Limits" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" HeaderText="Limits <sup>2</sup>" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
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
            </div>
            <div id="dvRVWater" runat="server">
                <p class="farmbreakhere" runat="server" id="RVWBreak" visible="false"></p>
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
                                    <asp:BoundColumn DataField="Limits" SortExpression="Limits" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" HeaderText="Limits <sup>2</sup>" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="true"></asp:BoundColumn>
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
            </div>
            <p class="farmbreakhere" runat="server" id="AddlCoverageBreak" visible="false"></p>
            <hr />
            <asp:Label ID="lblLiabCoverageHdr" runat="server" Text="ADDITIONAL COVERAGES"></asp:Label>
            <hr />
            <asp:Label ID="lblNoAddlCovExist" runat="server" Text="N/A"></asp:Label>
            <div id="dvAddlCoverage" runat="server">
                <table id="tblAdditionalCoverages" runat="server" style="width: 100%;" class="table">
                    <tr id="tblHeaderRow" runat="server">
                        <td class="AddlCoverageTitleColumn" style="font-weight: 700;">Coverage</td>
                        <td class="AddlCoverageDescColumn" style="font-weight: 700;">Description</td>
                        <td class="AddlCoverageLimitColumn" style="font-weight: 700;">Limit</td>
                        <td class="AddlCoveragePremColumn" style="font-weight: 700;">Premium</td>
                    </tr>
                    <tr id="trExtraExpenseRow" runat="server" style="display: none;">
                        <td colspan="2">Extra Expense</td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblExtraExpense_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblExtraExpense_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trRefFoodSpoilageRow" runat="server" style="display: none;">
                        <td colspan="2">Refrigerated Food Spoilage</td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblRefFoodSpoilage_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblRefFoodSpoilage_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trFarmExtenderRow" runat="server" style="display: none;">
                        <td colspan="3">Farm Extender</td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblFarmExtend_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trEmployeeLiabilityRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">Employer's Liability - Farm Employees</td>
                        <td colspan="2">
                            <table style="width: 100%" class="table">
                                <tr id="trFTEmp" runat="server" style="display: none">
                                    <td>
                                        <asp:Label ID="lblFTEmp" runat="server" Text="Full Time Employees(180-365): "></asp:Label>
                                        <asp:Label ID="lblFTEmpData" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr id="trPTEmp179" runat="server" style="display: none">
                                    <td>
                                        <asp:Label ID="lblPTEmp179" runat="server" Text="Part Time Employees(41-179): "></asp:Label>
                                        <asp:Label ID="lblPTEmp179Data" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr id="trPTEmp41" runat="server" style="display: none">
                                    <td>
                                        <asp:Label ID="lblPTEmp41" runat="server" Text="Part Time Employees(<41 days): "></asp:Label>
                                        <asp:Label ID="lblPTEmp41Data" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblEmpLiabPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trIncidentalBusinessPursuitsRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">Incidental Business Pursuits</td>
                        <td colspan="2">
                            <asp:Label ID="lblPursuit" runat="server"></asp:Label>:&nbsp;
                            <asp:Label ID="lblPursuitReceipts" runat="server"></asp:Label>
                            <asp:Label ID="lblPursuitAnnual" runat="server" Text=" Annual Revenue"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblIncidentalPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trFamilyMedPayRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">Family Medical Payments</td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblNumPerson" runat="server" Text="Number of Persons: "></asp:Label>
                            <asp:Label ID="lblNumPersonData" runat="server"></asp:Label>
                        </td>
                        <td colspan="2" style="text-align: right;">
                            <asp:Label ID="lblFamMedPayPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trCustomFarmingRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">Custom Farming</td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblSpray" runat="server"></asp:Label>
                            <asp:Label ID="lblSprayAnnual" runat="server"></asp:Label>
                            <asp:Label ID="lblSprayRevenue" runat="server" Text=" Annual Revenue"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCustomFarmLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCustomFarmPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trLimitedFarmPollutionRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn"><asp:Label ID="lblFarmPollutionCovName" runat="server">Limited Farm Pollution</asp:Label></td>
                        <td class="AddlCoverageDescColumn">Increased Limit</td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblFarmPollutionLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblFarmPollutionPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trAddlInsuredRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">Additional Insured</td>
                        <td class="AddlCoverageDescColumn">
                            <asp:DataGrid ID="dgAddlInsured" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="False">
                                <Columns>
                                    <asp:BoundColumn DataField="RowHdr" SortExpression="RowHdr" ItemStyle-Width="265px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="DisplayName" SortExpression="DisplayName" ItemStyle-Width="265px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblAddlInsuredLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblAddlInsuredPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trIdentityFraudExpenseRow" runat="server" style="display: none;">
                        <td colspan="3">Identity Fraud Expense</td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblFraudPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trSewerRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblSewer" runat="server" Text="Farm All Star"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblSewerIncrease" runat="server" Text="Backup of Sewer or Drain"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblSewerLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblSewerPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trWaterBackupRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblWaterBackup" runat="server">&nbsp;&nbsp; Water Backup</asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblWaterBackupDesc" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblWaterBackupLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblWaterBackupPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trWaterDamageRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblWaterDamage" runat="server">&nbsp;&nbsp; Water Damage</asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblWaterDamageDesc" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblWaterDamageLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblWaterDamagePrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trEquipmentBreakdownRow" runat="server" style="display: none;">
                        <td colspan="3">Equipment Breakdown</td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblBreakdownPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trPollutionCleanupRow" runat="server" style="display: none">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblPollutionClean" runat="server" Text="Pollution Clean up and Removal"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblPollutionBlank" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblPollutionCleanLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblPollutionCleanPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trPersonalLiabilityRow" runat="server" style="display: none;">
                        <td colspan="4">
                            <asp:Repeater ID="rptGL9" runat="server">
                                <ItemTemplate>
                                    <table id="tblGL9" runat="server" style="width: 75%; margin-left: 0px; table-layout: fixed;" cellpadding="0" cellspacing="0">
                                        <tr style="margin-left: 0px;">
                                            <td class="GL9TitleColumn">Personal Liability Coverage (GL-9)</td>
                                            <%--                                            <td style="margin-left:0px;" class="AddlCoverageTitleColumn">Personal Liability Coverage (GL-9)</td>--%>
                                            <td class="GL9DescColumn">
                                                <%--                                            <td style="margin-left:0px;" class="AddlCoverageDescColumn">--%>
                                                <asp:Label ID="lblGL9_Desc" runat="server"></asp:Label>
                                            </td>
                                            <%--                                            <td style="margin-left:0px;" class="AddlCoverageLimitColumn">--%>
                                            <td class="GL9LimitColumn">
                                                <asp:Label ID="lblGL9_Limit" runat="server"></asp:Label>
                                            </td>
                                            <td class="GL9PremColumn">
                                                <%--                                            <td style="margin-left:0px;" class="AddlCoveragePremColumn">--%>
                                                <asp:Label ID="lblGL9_Prem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr id="trContractGrowersRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblContractGrow" runat="server" Text="Contract Growers CCC"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblContractGrowDesc" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblContractGrow_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblContractGrow_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trCustomFeedingRow_swine" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblCFSwine" runat="server" Text="Custom Feeding"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblCFSwineDesc" runat="server" Text="Swine"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCFSwineLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCFSwinePremium" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trCustomFeedingRow_poultry" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblCFPoultry" runat="server" Text="Custom Feeding"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblCFPoultryDesc" runat="server" Text="Poultry"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCFPoultryLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCFPoultryPremium" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trCustomFeedingRow_cattle" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblCFCattle" runat="server" Text="Custom Feeding"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblCFCattleDesc" runat="server" Text="Cattle"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCFCattleLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCFCattlePremium" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trCustomFeedingRow_equine" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblCFEquine" runat="server" Text="Custom Feeding"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblCFEquineDesc" runat="server" Text="Equine"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCFEquineLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCFEquinePremium" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trEPLIRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblEPLI" runat="server"></asp:Label>
                        </td>
                        <%--<td style="width:290px;text-align:left;">--%>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblEPLIDesc" runat="server"></asp:Label>
                        </td>
                        <%--<td style="width:400px;text-align:right;">--%>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblEPLI_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblEPLI_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trLossOfIncomeRow" runat="server" style="display: none;">
                        <td colspan="4">
                            <asp:Repeater ID="rptLossOfIncome" runat="server">
                                <ItemTemplate>
                                    <table id="tblLossOfIncome" runat="server" style="margin-left: 0px; table-layout: fixed;" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="width: 307px;">
                                                <asp:Label ID="lblLossOfIncomeTitle" runat="server" Text="Loss of Income"></asp:Label>
                                            </td>
                                            <td style="width: 343px;">
                                                <asp:Label ID="lblLossOfIncomeDesc" runat="server"></asp:Label>
                                            </td>
                                            <td style="width: 116px; text-align: right;">
                                                <asp:Label ID="lblLossOfIncomeLimit" runat="server"></asp:Label>
                                            </td>
                                            <td style="width: 160px; text-align: right;">
                                                <asp:Label ID="lblLossOfIncomePremium" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr id="trAddlResidenceRentedToOthersRow" runat="server" style="display: none;">
                        <td colspan="4">
                            <asp:Repeater ID="rptAddlResidenceRentedToOthers" runat="server">
                                <ItemTemplate>
                                    <table id="tblAddlResidenceRentedToOthers" runat="server" style="width: 100%; border-style: none; border-collapse: collapse; border-spacing: 0px;">
                                        <tr>
                                            <td style="width: 303px;">Add'l Residence Rented to Others</td>
                                            <td style="width: 469px;">
                                                <asp:Label ID="lblAddlResDesc" runat="server"></asp:Label>
                                            </td>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblAddlResPremium" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr id="trMotorizedVehiclesOHRow" runat="server">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblMotoVehTitle" runat="server" Text="Motorized Vehicle (OH)"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblMotoVehDesc" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblMotoVehLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblMotoVehPremium" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trCanineExclusionRow" runat="server" style="display: none;">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblCanineExclusion" runat="server" Text="Exclusion - Canine"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblCanineExclusionDesc" runat="server" Text=""></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCanineExclusionLimit" runat="server" Text="N/A"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCanineExclusionPrem" runat="server" Text="N/A"></asp:Label>
                        </td>
                    </tr>
                    <%--Property in Transit - Leaving in case BAs change mind - delete after 4/29/2020--%>
                    <%--                    <tr id="trPropertyInTransit" runat="server" style="display:none"> 
                        <td colspan="2">Property in Transit</td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblPropertyInTransitLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblPropertyInTransitPrem" runat="server"></asp:Label>
                        </td>
                    </tr>--%>
                    <tr id="TrTotalAddlCoveragesPremiumRow" style="border-top: 2px solid black">
                        <td colspan="3">
                            <asp:Label ID="lblTotalAddlPrem" runat="server" Text="Total Additional Coverages"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblTotalAddlPremData" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>

                <%--                <table id="pnlCoverageHdr" runat="server" style="width: 100%" class="table">
                    <tr style="vertical-align: bottom">
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblAddlCoverage" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblAddlDesc" runat="server" Text="Description" Font-Bold="true"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblPropLimit" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblPropPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblExtraExpense" runat="server" style="width: 100%; display:none" class="table">
                        <tr>
                            <td style="width: 1050px">
                                <asp:Label ID="lblExtraExpense" runat="server" Text="Extra Expense"></asp:Label>
                            </td>
                            <td class="AddlCoverageLimitColumn">
                                <asp:Label ID="lblExtraExpense_Limit" runat="server"></asp:Label>
                            </td>
                            <td class="AddlCoveragePremColumn">
                                <asp:Label ID="lblExtraExpense_Prem" runat="server"></asp:Label>
                            </td>
                        </tr>
                 </table>--%>
                <%--                <table id="tblFarmExtend" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td style="width:1210px">
                            <asp:Label ID="lblFarmExtend" runat="server" Text="Farm Extender"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblFarmExtend_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <div id="dvEmpLiab" runat="server" style="display:none">
                    <table id="tblEmpLiab" runat="server" style="width: 100%" class="table">
                        <tr>
                            <td class="AddlCoverageTitleColumn">
                                <asp:Label ID="lblEmpLiab" runat="server" Text="Employer's Liability - Farm Employees"></asp:Label>
                            </td>
                            <td style="vertical-align:top; width:690px;">
                                <table style="width: 100%" class="table">
                                    <tr id="trFTEmp" runat="server" style="display:none">
                                        <td>
                                            <asp:Label ID="lblFTEmp" runat="server" Text="Full Time Employees(180-365): "></asp:Label>
                                            <asp:Label ID="lblFTEmpData" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trPTEmp179" runat="server" style="display:none">
                                        <td>
                                            <asp:Label ID="lblPTEmp179" runat="server" Text="Part Time Employees(41-179): "></asp:Label>
                                            <asp:Label ID="lblPTEmp179Data" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trPTEmp41" runat="server" style="display:none">
                                        <td>
                                            <asp:Label ID="lblPTEmp41" runat="server" Text="Part Time Employees(<41 days): "></asp:Label>
                                            <asp:Label ID="lblPTEmp41Data" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="AddlCoveragePremColumn">
                                <asp:Label ID="lblEmpLiabPrem" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>--%>
                <%--                <div id="dvIncidental" runat="server" style="display:none">
                    <table id="tblIncidental" runat="server" style="width: 100%" class="table">
                        <tr>
                            <td class="AddlCoverageTitleColumn">
                                <asp:Label ID="lblIncidental" runat="server" Text="Incidental Business Pursuits"></asp:Label>
                            </td>
                            <td style="width:690px;vertical-align:top;">
                                <asp:Label ID="lblPursuit" runat="server"></asp:Label>:&nbsp;
                                <asp:Label ID="lblPursuitReceipts" runat="server"></asp:Label>
                                <asp:Label ID="lblPursuitAnnual" runat="server" Text=" Annual Revenue"></asp:Label>
                            </td>
                            <td class="AddlCoveragePremColumn">
                                <asp:Label ID="lblIncidentalPrem" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>--%>
                <%--                <table id="tblFamMedPay" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblFamMedPay" runat="server" Text="Family Medical Payments"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblNumPerson" runat="server" Text="Number of Persons: "></asp:Label>
                            <asp:Label ID="lblNumPersonData" runat="server"></asp:Label>
                        </td>
                        <td style="width:400px;text-align:right;">
                            <asp:Label ID="lblFamMedPayPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblCustomFarm" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblCustomFarm" runat="server" Text="Custom Farming"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblSpray" runat="server"></asp:Label>
                            <asp:Label ID="lblSprayAnnual" runat="server"></asp:Label>
                            <asp:Label ID="lblSprayRevenue" runat="server" Text=" Annual Revenue"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblCustomFarmLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblCustomFarmPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblFarmPollution" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblFarmPollution" runat="server" Text="Limited Farm Pollution"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblIncrease" runat="server" Text="Increased Limit"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblFarmPollutionLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblFarmPollutionPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblAddlInsured" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblAddlInsured" runat="server" Text="Additional Insured"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:DataGrid ID="dgAddlInsured" runat="server" HorizontalAlign="Left" CellPadding="4" AutoGenerateColumns="false" GridLines="None" ShowHeader="False">
                                <Columns>
                                    <asp:BoundColumn DataField="RowHdr" SortExpression="RowHdr" ItemStyle-Width="265px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="DisplayName" SortExpression="DisplayName" ItemStyle-Width="265px" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblAddlInsuredLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblAddlInsuredPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblFraud" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td style="width: 1210px">
                            <asp:Label ID="lblFraud" runat="server" Text="Identity Fraud Expense"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblFraudPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblSewer" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblSewer" runat="server" Text="Farm All Star"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblSewerIncrease" runat="server" Text="Backup of Sewer or Drain"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblSewerLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblSewerPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblBreakdown" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td style="width:1210px">
                            <asp:Label ID="lblBreakdown" runat="server" Text="Equipment Breakdown"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblBreakdownPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblPollution" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblPollutionClean" runat="server" Text="Pollution Clean up and Removal"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblPollutionBlank" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblPollutionCleanLimit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblPollutionCleanPrem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblGL9" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td style="width: 1050px">
                            <asp:Label ID="lblGL9" runat="server" Text="Personal Liability Coverage(GL-9)"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblGL9_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblGL9_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblContractGrow" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblContractGrow" runat="server" Text="Contract Growers CCC"></asp:Label>
                        </td>
                        <td class="AddlCoverageDescColumn">
                            <asp:Label ID="lblContractGrowDesc" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoverageLimitColumn">
                            <asp:Label ID="lblContractGrow_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblContractGrow_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <table id="tblEPLI" runat="server" style="width: 100%; display:none" class="table">
                    <tr>
                        <td class="AddlCoverageTitleColumn">
                            <asp:Label ID="lblEPLI" runat="server"></asp:Label>
                        </td>
                        <td style="width:290px;text-align:left;">
                            <asp:Label ID="lblEPLIDesc" runat="server"></asp:Label>
                        </td>
                        <td style="width:400px;text-align:right;">
                            <asp:Label ID="lblEPLI_Limit" runat="server"></asp:Label>
                        </td>
                        <td class="AddlCoveragePremColumn">
                            <asp:Label ID="lblEPLI_Prem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
                <%--                <div id="divAddlResidenceRentedToOthers" runat="server">
                    <asp:Repeater ID="rptAddlResidenceRentedToOthers" runat="server">
                        <ItemTemplate>
                            <table id="tblAddlResidenceRentedToOthers" runat="server" style="width:100%;" class="table">
                                <tr>
                                    <td class="AddlCoverageTitleColumn">
                                        <asp:Label ID="lblAddlRes" runat="server"></asp:Label>
                                    </td>
                                    <td style="width:690px;text-align=left;">
                                        <asp:Label ID="lblAddlResDesc" runat="server"></asp:Label>
                                    </td>
                                    <td class="AddlCoveragePremColumn">
                                        <asp:Label ID="lblAddlResPremium" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>--%>
                <%--                <hr style="border-color: black" />
                <table id="tblTotalAddlPrem" runat="server" style="width: 100%" class="table">
                    <tr>
                        <td>
                            <asp:Label ID="lblTotalAddlPrem" runat="server" Text="Total Additional Coverages"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblTotalAddlPremData" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
            </div>
            <br />
            <p class="farmbreakhere" runat="server" id="AdjBreak" visible="false"></p>
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
            <p class="farmbreakhere" runat="server" id="SuperBreak" visible="false"></p>
            <table style="width: 100%" class="table">
                <tr>
                    <td>
                        <asp:Label ID="lblSupOne" runat="server" Text="<sup>1</sup> Limit values are rounded to the next highest 1000"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSupThree" runat="server" Text="<sup>2</sup> Limit values are rounded to the next highest 100"></asp:Label>
                    </td>
                </tr>
            </table>
            <p class="farmbreakhere" runat="server" id="PaymentOptBreak" visible="false"></p>
            <asp:Panel ID="pnlPaymentOptions" runat="server">
                <uc1:ctlPaymentOptions runat="server" ID="ctlPaymentOptions" />
            </asp:Panel>
            <uc1:ctlPageBreak runat="server" ID="ctlPageBreak" />
        </div>
    </form>
</body>
</html>
