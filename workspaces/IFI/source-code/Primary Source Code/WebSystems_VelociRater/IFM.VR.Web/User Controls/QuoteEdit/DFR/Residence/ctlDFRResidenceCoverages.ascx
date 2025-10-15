<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDFRResidenceCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlDFRResidenceCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/DFR/Residence/ctlDFROptionalCoverages.ascx" TagPrefix="uc1" TagName="ctlDFROptionalCoverages" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>

<style type="text/css">
    .auto-style1 {
        height: 26px;
    }
</style>

<div id="dvCoverages" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblCoverageHdr" runat="server" Text="COVERAGES - "></asp:Label>
        <asp:Label ID="lblCoverageForm" runat="server"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearCoverages" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" OnClientClick="var confirmed = confirm('Clear ALL Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;"  ToolTip="Clear ALL Coverages">Clear Page</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveCoverages" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="dvDeductible" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
            <h3>
                <asp:Label ID="lblDeductiblehDR" runat="server" Text="DEDUCTIBLES"></asp:Label>
                <span style="float: right;">
                    <asp:LinkButton ID="lnkClearDeductibles" runat="server" CssClass="RemovePanelLink" OnClientClick="var confirmed = confirm('Clear Deductibles?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" Style="margin-left: 20px" ToolTip="Clear Deductibles">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveDeductibles" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <div id="dvPolicy" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="vertical-align: top">
                                <table style="width: 100%" id="tblCoveragesGeneralInfo">
                                    <tr>
                                        <td style="width: 150px">
                                            <asp:Label ID="lblPolicyDeduct" runat="server" Text="Policy"></asp:Label>
                                            <br />
                                            <asp:DropDownList ID="ddlDeductible" runat="server" Width="150px" TabIndex="0"></asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <div id="pnlReplacementCC" runat="server">
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnReplacementCC" runat="server" Text="Replacement Cost Calculator" CssClass="StandardButtonWrap" Height="40px" Width="120px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblCalValue" runat="server" Text="Calculated Value" Font-Italic="true"></asp:Label>
                                                <br />
                                                <asp:Label ID="lblReplacementCCValue" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div id="dvBaseCoverage" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
            <h3>
                <asp:Label ID="lblPolicyBaseCoverageHdr" runat="server" Text="POLICY BASE COVERAGES"></asp:Label>
                <span style="float: right;">
                    <asp:LinkButton ID="lnkClearBase" runat="server" OnClientClick="var confirmed = confirm('Clear Policy Base Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Base Coverages">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveBase" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <div id="dvBaseCovContainer" runat="server">
                <div id="dvPolicyCoverage" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 36%">&nbsp;</td>
                            <td>
                                <table>
                                    <tr>
                                        <td align="center" style="width: 80px" class="CovTableColumn">
                                            <asp:Label ID="lblLimit" runat="server" Text="Limit" CssClass="CovTableHeaderLabel"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 80px" class="CovTableColumn">
                                            <asp:Label ID="lblChange" runat="server" Text="Change" CssClass="CovTableHeaderLabel"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 80px" class="CovTableColumn">
                                            <asp:Label ID="lblTotal" runat="server" Text="Total" CssClass="CovTableHeaderLabel"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvCoverageA" runat="server">
                    <table style="width: 100%">
                        <tr id="trDwelling" runat="server">
                            <td style="width: 36%">
                                <asp:Label ID="lblDwellingReq" runat="server" Text="*"></asp:Label>
                                <asp:Label ID="lblDwelling" runat="server" Text="A - Dwelling"></asp:Label>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtDwLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtDwellingChangeInLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtDwellingTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvCoverageB" runat="server">
                    <table style="width: 100%">
                        <tr id="trStructures" runat="server">
                            <td style="width: 36%">
                                <asp:Label ID="lblOtherStructures" runat="server" Text="B - Other Structures"></asp:Label>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td class="CovTableColumn">
                                            <asp:TextBox ID="txtOtherLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td class="CovTableColumn">
                                            <asp:TextBox ID="txtOtherChgInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                        </td>
                                        <td class="CovTableColumn">
                                            <asp:TextBox ID="txtOtherTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvCoverageC" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 36%">
                                <asp:Label ID="lblPersonalProp" runat="server" Text="C - Personal Property"></asp:Label>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td class="CovTableColumn">
                                            <asp:TextBox ID="txtPPLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td class="CovTableColumn">
                                            <asp:TextBox ID="txtPPChgInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                        </td>
                                        <td class="CovTableColumn">
                                            <asp:TextBox ID="txtPPTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvCoverageD" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 36%">
                                <asp:Label ID="lblLivingCost" runat="server" Text="D - Additional Living Cost/Fair Rental Value"></asp:Label>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td id="tdLossLimit" runat="server" class="CovTableColumn">
                                            <asp:TextBox ID="txtLivingLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td id="tdLossChange" runat="server" class="CovTableColumn">
                                            <asp:TextBox ID="txtLivingChgInLimit" runat="server" CssClass="CovTableItem"></asp:TextBox>
                                        </td>
                                        <td id="tdLossTotal" runat="server" class="CovTableColumn">
                                            <asp:TextBox ID="txtLivingTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                <div id="dvLiabCov" runat="server" style="width:500px">
                    <table id="tblLiabCov" runat="server" style="width: 100%">
                        <tr>
                            <td>
                                <asp:Label ID="lblPersonalLiab" runat="server" Text="Personal Liability"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlPersonalLiability" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                            <td id="PersonalLiabilitylimitTextDFR" runat="server" Visible="false">
                                <span>We require a Personal Liability limit of<br />
                                    $300,000 when quoting an umbrella.</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblMedPymts" runat="server" Text="Medical Payments"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlMedicalPayments" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                </div>
                <div id="dvBasicButtons" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:Button ID="btnSaveBase" runat="server" Text="Save Base Policy Coverages" CssClass="StandardSaveButton" />
                                <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>                                
                            </td>
                            <td>
                                <asp:Button ID="btnRateBase" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" />
                                <asp:Button ID="btnViewGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Billing Information Page" />
                                <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" /><br />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <div id="dvOptionalCoverages" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
            <h3>
                <asp:Label ID="lblOptionalCoverages" runat="server" Text="OPTIONAL COVERAGES ("></asp:Label>
                <asp:Label ID="lblOptionalChosen" runat="server"></asp:Label>
                )
                <span style="float: right;">
                    <asp:LinkButton ID="lnkClearOptional" runat="server" CssClass="RemovePanelLink" OnClientClick="var confirmed = confirm('Clear Optional Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" Style="margin-left: 20px" ToolTip="Clear Optional Coverages">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveOptional" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <uc1:ctlDFROptionalCoverages runat="server" id="ctlDFROptionalCoverages" />
            </div>
        </div>
    <asp:HiddenField ID="hiddenSelectedForm" runat="server" />

    <asp:HiddenField ID="hiddenCovALimit" runat="server" />
    <asp:HiddenField ID="hiddenCovAChange" runat="server" Value="0" />
    <asp:HiddenField ID="hiddenCovATotal" runat="server" />
    <asp:HiddenField ID="hiddenCovBLimit" runat="server" />
    <asp:HiddenField ID="hiddenCovBChange" runat="server" />
    <asp:HiddenField ID="hiddenCovBTotal" runat="server" />
    <asp:HiddenField ID="hiddenCovCLimit" runat="server" />
    <asp:HiddenField ID="hiddenCovCChange" runat="server" />
    <asp:HiddenField ID="hiddenCovCTotal" runat="server" />
    <asp:HiddenField ID="hiddenCovDLimit" runat="server" />
    <asp:HiddenField ID="hiddenCovDChange" runat="server" />
    <asp:HiddenField ID="hiddenCovDTotal" runat="server" />

    <%--Tracking active panels--%>
    <asp:HiddenField ID="hiddenCoverage" runat="server" />
    <asp:HiddenField ID="hiddenDeductibles" runat="server" />
    <asp:HiddenField ID="hiddenBase" runat="server" />
    <asp:HiddenField ID="hiddenOptional" runat="server" />
    </div>
</div>