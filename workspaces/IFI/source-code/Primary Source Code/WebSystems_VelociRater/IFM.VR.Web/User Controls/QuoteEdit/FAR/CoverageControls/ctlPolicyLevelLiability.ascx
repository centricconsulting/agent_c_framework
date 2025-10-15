<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPolicyLevelLiability.ascx.vb" Inherits="IFM.VR.Web.ctlPolicyLevelLiability" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/ctlAdditionalInsuredList.ascx" TagPrefix="uc1" TagName="ctlAdditionalInsuredList" %>


<div id="dvFarmPolicyLiabilityCoverage" runat="server">
    <h3>
        <asp:Label ID="lblLiabilityHdr" runat="server" Text="Liability"></asp:Label>
        <span style="float: right">
            <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" OnClientClick="var confirmed = confirm('Clear Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Liability to Default Values" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all" runat="server">Save</asp:LinkButton>            
        </span>
    </h3>
    <div>
        <asp:Panel ID="pnlPolicyLiability" runat="server">
            <div id="dvLiabilityTypeHdr" runat="server">
                <asp:Label ID="lblLiabType" runat="server" Text="Liability Coverage Form"></asp:Label>
            </div>
            <div id="dvLiabilityType" runat="server">
                <asp:Label ID="lblLiabCovType" runat="server"></asp:Label>
            </div>
            <div id="dvLiabilityDropDown" runat="server">
                <asp:DropDownList ID="ddlLiabCovType" runat="server"></asp:DropDownList>
            </div>
            <hr />
            <div id="dvLiability" runat="server" style="vertical-align:middle">
                <asp:Label ID="lblLiability" runat="server" Text="Coverage L (BI & PD)"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlLiability" runat="server"></asp:DropDownList>
            </div>
            <div id="dvMedPay" runat="server" style="vertical-align:middle">
                <asp:Label ID="lblMedPay" runat="server" Text="Med Payments"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlMedPay" runat="server"></asp:DropDownList>
            </div>
            <hr />
            <div id="dvEmpLiab" runat="server">
                <asp:CheckBox ID="chkEmpLiab" runat="server" />&nbsp;
                <asp:Label ID="lblEmpLiab" runat="server" Text="Employer's Liability - Farm Employees"></asp:Label> 
                <div id="dvNumEmployees" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td></td>
                            <td>
                                <asp:Label ID="lblFTEmp" runat="server" Text="Full Time Emp (180-365)"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtFTEmp" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblPT41Days" runat="server" Text="Part Time Emp(41-179 days)"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtPT41Days" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblPT40Days" runat="server" Text="Part Time Emp(< 41 days)"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtPT40Days" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvBusinessPursuits" runat="server">
                <asp:CheckBox ID="chkBusinessPursuits" runat="server" />&nbsp;
                <asp:Label ID="lblBusinessPursuits" runat="server" Text="Incidental Business Pursuits"></asp:Label>
                <div id="dvBPInfo" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td></td>
                            <td>
                                <asp:Label ID="lblBPType" runat="server" Text="Business Pursuit Type"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlBPType" runat="server"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblAnnualReceipts" runat="server" Text="Annual Receipts"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtAnnualReceipts" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvFamilyMedPay" runat="server">
                <asp:CheckBox ID="chkFamMedPay" runat="server" />&nbsp;
                <asp:Label ID="lblFamMedPay" runat="server" Text="Family Medical Payments"></asp:Label>
                <div id="dvFMPNumPer" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td></td>
                            <td>
                                <asp:Label ID="lblFMPNumPer" runat="server" Text="Number of Persons"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtFMPNumPer" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvCustomFarming" runat="server">
                <asp:CheckBox ID="chkCustomFarming" runat="server" />&nbsp;
                <asp:Label ID="lblCustomFarming" runat="server" Text="Custom Farming"></asp:Label>
                <div id="dvCFInfo" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td></td>
                            <td>
                                <asp:Label ID="lblCFType" runat="server" Text="Custom Farming Type"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlCFType" runat="server"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblCFAnnualReceipts" runat="server" Text="Annual Receipts"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtCFAnnualReceipts" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvFarmPollution" runat="server">
                <asp:CheckBox ID="chkFarmPollution" runat="server" />&nbsp;
                <asp:Label ID="lblFarmPollution" runat="server" Text="Limited Farm Pollution (Increased Limits)"></asp:Label>
                <div id="dvFPIncreasedLimits" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td></td>
                            <td>
                                <asp:Label ID="lblFPLimit" runat="server" Text="Limit"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlFPLimit" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvEPLI" runat="server">
                <asp:CheckBox ID="chkEPLI" runat="server" Checked="true" />&nbsp;
                <asp:Label ID="lblEPLI" runat="server" Text="EPLI (non-Underwritten)"></asp:Label>
            </div>
            <div id="dvAdditionalIns" runat="server">
                <asp:CheckBox ID="chkAdditionalIns" runat="server" />&nbsp;
                <asp:Label ID="lblAdditionalIns" runat="server" Text="Additonal Insured"></asp:Label>
                <div id="dvAIInfo" runat="server" style="display:none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 100px">
                                <asp:Label ID="lblAIType" runat="server" Text="Type"></asp:Label>
                            </td>
                            <td style="width: 150px">
                                <asp:Label ID="lblAIBusinessReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                <asp:Label ID="lblAIBusiness" runat="server" Text="Business Name"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblAILastReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                <asp:Label ID="lblAILastName" runat="server" Text="Last Name"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblAIFirstReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                <asp:Label ID="lblAIFirstName" runat="server" Text="First Name"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <uc1:ctlAdditionalInsuredList runat="server" id="ctlAdditionalInsuredList" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddAI" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvIdentityFraud" runat="server">
                <asp:CheckBox ID="chkIdentityFraud" runat="server" />&nbsp;
                <asp:Label ID="lblIdentityFraud" runat="server" Text="Identity Recovery Expense"></asp:Label>
            </div>
            <div id="dvPersLiab" runat="server">
                <asp:CheckBox ID="chkPersLiab" runat="server" />&nbsp;
                <asp:Label ID="lblPersLiab" runat="server" Text="Personal Liability Coverage (GL-9)"></asp:Label>
                <div id="dvPersLiabInfo" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td>
                                <asp:Label ID="lblPLFirstName" runat="server" Text="First Name"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtPLFirstName" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblPLLastName" runat="server" Text="Last Name"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtPLLastName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </asp:Panel>
    </div>
    <asp:HiddenField ID="hiddenFarmLiabilityCoverage" runat="server" />
</div>    