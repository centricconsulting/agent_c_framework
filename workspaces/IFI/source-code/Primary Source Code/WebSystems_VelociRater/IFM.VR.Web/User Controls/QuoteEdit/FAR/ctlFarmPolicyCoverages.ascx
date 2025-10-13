<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmPolicyCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlFarmPolicyCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/ctlAdditionalInsuredList.ascx" TagPrefix="uc1" TagName="ctlAdditionalInsuredList" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctlFamilyMedicalPayments_App.ascx" TagPrefix="uc1" TagName="ctlFamilyMedicalPayments_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomMultipleNamesList.ascx" TagPrefix="uc1" TagName="ctlHomMultipleNamesList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/Cov_CanineExclusionList.ascx" TagPrefix="uc1" TagName="Cov_CanineExclusionList" %>





<div id="dvFarmPolicyCoverages" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblMainHeader" runat="server" Text="POLICY LEVEL COVERAGE"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearGeneralInfo" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Policy Level Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Coverage to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveGeneralInfo" runat="server" ToolTip="Save Policy Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="dvFarmPolicyLiabilityCoverage" runat="server">
            <h3>
                <asp:Label ID="lblLiabilityHdr" runat="server" Text="Liability"></asp:Label>
                <span style="float: right">
                    <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" OnClientClick="var confirmed = confirm('Clear Liability Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Liability to Default Values" runat="server">Clear</asp:LinkButton>
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
                        <asp:DropDownList ID="ddlLiabCovType" runat="server" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <div id="dvLiability" runat="server" style="vertical-align: middle">
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 30%">
                                    <asp:Label ID="lblLiability" runat="server" Text="Coverage L (BI & PD)"></asp:Label>&nbsp;
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlLiability" runat="server" CssClass="CovTableItemDropDown"></asp:DropDownList>
                                </td>
                                <td id="PersonalLiabilitylimitTextFarm" runat="server" visible="false">
                                    <span>We require a BI & PD liability limit of<br />
                                        $300,000 when quoting an umbrella.</span>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvMedPay" runat="server" style="vertical-align: middle">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 35%">
                                    <asp:Label ID="lblMedPay" runat="server" Text="Coverage M (Med Payments)"></asp:Label>&nbsp;
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlMedPay" runat="server" CssClass="CovTableItemDropDown"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </div>
                    <div id="dvEmpLiab" runat="server">
                        <asp:CheckBox ID="chkEmpLiab" runat="server" />&nbsp;
                        <asp:Label ID="lblEmpLiab" runat="server" Text="Employer's Liability - Farm Employees"></asp:Label>
                        <div id="dvNumEmployees" runat="server" style="display: none">
                            <div id="dvEmployeeLiab" runat="server">
                                <table style="width: 100%">
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblFTEmp" runat="server" Text="Full Time Emp (180-365 days)"></asp:Label>
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
                    </div>
                    <div id="dvStopGap" runat="server">
                        <asp:CheckBox ID="chkStopGap" runat="server" />&nbsp;
                        <asp:Label ID="lblStopGap" runat="server" Text="Stop Gap (OH)"></asp:Label>
<%--                        &nbsp;
                        <asp:DropDownList ID="ddStopGapLimit" runat="server" Width="100px"></asp:DropDownList>--%>
                        <div id="dvStopGapData" runat="server" style="display: none">
                            <div id="dvStopGapPayroll" runat="server">
                                <table>
                                    <tr>
                                        <%--<td></td>--%>
                                        <td>
                                            <asp:Label ID="lblStopGapLimit" runat="server" Text="Limit"></asp:Label>
                                            <br />
                                            <asp:DropDownList ID="ddStopGapLimit" runat="server" ></asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblStopGapPayroll" runat="server" Text="Ohio Only Payroll"></asp:Label>
                                            <br />
                                            <asp:TextBox ID="txtStopGapPayroll" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div id="dvBusinessPursuits" runat="server">
                        <asp:CheckBox ID="chkBusinessPursuits" runat="server" />&nbsp;
                        <asp:Label ID="lblBusinessPursuits" runat="server" Text="Incidental Business Pursuits"></asp:Label>
                        <div id="dvBPInfo" runat="server" style="display: none">
                            <table style="width: 100%">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblBPType" runat="server" Text="Business Pursuit Type"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="ddlBPType" runat="server" Width="335px"></asp:DropDownList>
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
                        <asp:CheckBox ID="chkFamMedPay" runat="server"/>&nbsp;
                        <asp:Label ID="lblFamMedPay" runat="server" Text="Family Medical Payments"></asp:Label>
                        <div id="dvFMPNumPer" runat="server" style="display: none">
                            <table style="width: 100%">
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
                    <div id="dvFamilyMedPayNames" runat="server" visible="false">
                        <uc1:ctlFamilyMedicalPayments_App runat="server" ID="ctlFamilyMedicalPayments_App" />
                    </div>
                    <div id="dvCustomFarming" runat="server">
                        <asp:CheckBox ID="chkCustomFarming" runat="server" />&nbsp;
                        <asp:Label ID="lblCustomFarming" runat="server" Text="Custom Farming"></asp:Label>
                        <div id="dvCFInfo" runat="server" style="display: none">
                            <table>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblCFType" runat="server" Text="Custom Farming Type"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="ddlCFType" runat="server">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="with spraying" Value="80115"></asp:ListItem>
                                            <asp:ListItem Text="without spraying" Value="70129"></asp:ListItem>
                                        </asp:DropDownList>
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
                        <asp:Label ID="lblFarmPollution" runat="server"></asp:Label>
                        <div id="dvFPIncreasedLimits" runat="server" style="display: none">
                            <table style="width: 100%">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblFPLimit" runat="server" Text="Limit"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="ddlFPLimit" runat="server">
<%--                                            <asp:ListItem Text="25,000 [INC]" Value=""></asp:ListItem>
                                            <asp:ListItem Text="50,000" Value="8"></asp:ListItem>
                                            <asp:ListItem Text="75,000" Value="9"></asp:ListItem>
                                            <asp:ListItem Text="100,000" Value="50"></asp:ListItem>--%>
                                        </asp:DropDownList>
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
                        <asp:CheckBox ID="chkAdditionalIns" runat="server" AutoPostBack="true" />&nbsp;
                        <asp:LinkButton ID="lbtnAdditionalIns" runat="server">Additional Insured</asp:LinkButton>
                        <div id="dvAIInfo" runat="server" style="display: none" class="div">
                                <uc1:ctlAdditionalInsuredList runat="server" ID="ctlAdditionalInsuredList" />
                                <span style="float: right;">
                                    <asp:LinkButton ID="lnkAddAI" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional</asp:LinkButton>
                                </span>
                                <br />
                            
                        </div>
                    </div>
                    <div id="dvAIStateApprovedPesticideHerbicideApplicator" runat="server">
                        <asp:CheckBox ID="chkPesticideHerbicideApplicatorOH" runat="server" />&nbsp;
                        <asp:Label ID="lblPesticideHerbicideApplicatorOH" runat="server" Text="State Approved Pesticide or Herbicide Applicator (OH)"></asp:Label>
                    </div>
                    <%--<uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />--%>
                    <div id="dvIdentityFraud" runat="server">
                        <asp:CheckBox ID="chkIdentityFraud" runat="server" />&nbsp;
                        <asp:Label ID="lblIdentityFraud" runat="server" Text="Identity Fraud Expense"></asp:Label>
                    </div>
                    <div id="dvMotorizedVehicles" runat="server">
                        <asp:CheckBox ID="chkMotorizedVehicles" runat="server" />&nbsp;
                        <asp:Label ID="lblMotorizedVehicles" runat="server" Text="Motorized Vehicle - Ohio"></asp:Label>
                    </div>
                    <div id="dvCanine" runat="server">
                        <uc1:Cov_CanineExclusionList runat="server" id="Cov_CanineExclusionList" />
                    </div>
                    <div id="dvCustomFeeding" runat="server">
                        <div>
                            <asp:CheckBox ID="chkCustomFeeding" runat="server" CssClass="chkCF" />
                            <asp:Label ID="lblCustomFeedingCheckboxLabel" runat="server" Text="Custom Feeding"></asp:Label>
                        </div>
                        <div id="dvCustomFeedingData" runat="server" style="display:none;" class="divCFData">
                            <table id="tblCustomFeedingData" runat="server" style="width:100%;">
                                <tr>
                                    <td class="CFColumn1">
                                        <asp:Label runat="server" ID="lblCFCattleLabel" Text="Cattle Limit"></asp:Label>
                                    </td>
                                    <td class="CFColumn2">
                                        <asp:DropDownList ID="ddCFCattleLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Cattle"></asp:DropDownList>
                                    </td>
                                    <td class="CFColumn3">
                                        <asp:TextBox ID="txtCFCattleDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Cattle"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CFColumn1">
                                        <asp:Label runat="server" ID="lblCFEquineLabel" Text="Equine Limit"></asp:Label>
                                    </td>
                                    <td class="CFColumn2">
                                        <asp:DropDownList ID="ddCFEquineLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Equine"></asp:DropDownList>
                                    </td>
                                    <td class="CFColumn3">
                                        <asp:TextBox ID="txtCFEquineDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Equine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CFColumn1">
                                        <asp:Label runat="server" ID="lblCFPoultryLabel" Text="Poultry Limit"></asp:Label>
                                    </td>
                                    <td class="CFColumn2">
                                        <asp:DropDownList ID="ddCFPoultryLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Poultry"></asp:DropDownList>
                                    </td>
                                    <td class="CFColumn3">
                                        <asp:TextBox ID="txtCFPoultryDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Poultry"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CFColumn1">
                                        <asp:Label runat="server" ID="lblCFSwineLabel" Text="Swine Limit"></asp:Label>
                                    </td>
                                    <td class="CFColumn2">
                                        <asp:DropDownList ID="ddCFSwineLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Swine"></asp:DropDownList>
                                    </td>
                                    <td class="CFColumn3">
                                        <asp:TextBox ID="txtCFSwineDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Swine"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
<%--                    <div id="dvMotorizedVehicles" runat="server">
                        <asp:CheckBox ID="chkMotorizedVehicles" runat="server" />&nbsp;
                        <asp:Label ID="lblMotorizedVehicles" runat="server" Text="Motorized Vehicle - Ohio"></asp:Label>
                    </div>--%>
<%--                    <div id="dvPersLiab" runat="server" class="div">
                        <asp:CheckBox ID="chkPersLiab" runat="server" />&nbsp;
                        <asp:LinkButton ID="lbtnPersLiab" runat="server">Personal Liability Coverage (GL-9)</asp:LinkButton>
                        <div id="dvPersLiabInfo" runat="server" style="display: none" class="div">
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPLFirstName" runat="server" Text="First Name"></asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtPLFirstName" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblPLLastName" runat="server" Text="Last Name"></asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtPLLastName" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>--%>
                    <br />
                    <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
                    <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
                </asp:Panel>
            </div>
            <asp:HiddenField ID="hiddenFarmLiabilityCoverage" runat="server" />
            <asp:HiddenField ID="hiddenFarmPollution" runat="server" Value="false" />
        </div>
        <div id="dvFarmPolicyPropertyCoverage" runat="server">
            <h3>
                <asp:Label ID="lblPropertyHdr" runat="server" Text="Property"></asp:Label>
                <span style="float: right">
                    <asp:LinkButton ID="lnkPropClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" OnClientClick="var confirmed = confirm('Clear Property Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Property to Default Values" runat="server">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkPropSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all" runat="server">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Panel ID="pnlPolicyProperty" runat="server">
                    <div id="dvFarmAllStar" runat="server">
                        <asp:CheckBox ID="chkFarmAllStar" runat="server" />&nbsp;
                        <asp:LinkButton ID="lbtnFarmAllStar" runat="server">Farm All Star</asp:LinkButton>
                        <asp:Label ID="lblFarmAllStar" runat="server" Text=""><a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("FAR_Help_FarmAllStar")%>">Farm All Star</a></asp:Label>
                        <div id="dvBackSewerDrain" runat="server" style="display: none">
                            <table id="tblWaterBUWaterDamage" runat="server">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblBackSewerDrain" runat="server" Text="Backup of Sewer or Drain"></asp:Label>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblBSDLimit" runat="server" Text="Limit"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="ddlBSDLimit" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="trWaterDamage" runat="server">
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblWaterDamage" runat="server" Text="Water Damage"></asp:Label>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr id="trWaterDamageLimit" runat="server">
                                    <td></td>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblWaterDamageLimit" runat="server" Text="Limit"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="ddlWaterDamageLimit" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div id="dvEquipBreak" runat="server">
                        <asp:CheckBox ID="chkEquipBreak" runat="server" Checked="true" />&nbsp;
                        <asp:LinkButton ID="lbtnEquipBreak" runat="server">Equipment Breakdown</asp:LinkButton>
                    </div>
                    <div id="dvExtraExpense" runat="server">
                        <asp:CheckBox ID="chkExtraExpense" runat="server" />&nbsp;
                        <asp:LinkButton ID="lbtnExtraExpense" runat="server">Extra Expense</asp:LinkButton>
                        <div id="dvExtraExpenseLimit" runat="server" style="display: none">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width:5%"></td>
                                    <td>
                                        <asp:Label ID="lblExtraExpenseLimit" runat="server" Text="Limit"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExtraExpenseLimit" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvExtraExpenseIncreasedLimits" runat="server" style="display: none">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblExtraExpenseIncludedLimit" runat="server" Text="Included Limit"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblExtraExpenseIncreasedLimit" runat="server" Text="Increased Limit"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblExtraExpenseTotalLimit" runat="server" Text="Total Limit"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtExtraExpenseIncludedLimit" Width="100" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExtraExpenseIncreasedLimit" Width="100" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExtraExpenseTotalLimit" Width="100" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div id="dvFarmExtend" runat="server">
                        <asp:CheckBox ID="chkFarmExtend" runat="server" />&nbsp;
                        <asp:LinkButton ID="lbtnFarmExtend" runat="server">Farm Extender</asp:LinkButton>
                        <asp:Label ID="lblFarmExtender" runat="server" Text=""><a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("FAR_Help_FarmExtender")%>">Farm Extender</a></asp:Label>
                    </div>
                    <div id="dvPollution" runat="server">
                        <asp:CheckBox ID="chkPollution" runat="server" />&nbsp;
                        <asp:Label ID="lblPollution" runat="server" Text="Pollutant Clean Up and Removal - Increased Limits"></asp:Label>
                    </div>
                    <div id="dvRefFoodSpoilage" runat="server" style="display: none">
                        <asp:CheckBox ID="chkRefFoodSpoilage" runat="server" />&nbsp;
                        <asp:Label ID="lblRefFoodSpoilage" runat="server" Text="Refrigerated Food Spoilage"></asp:Label>
                        <table id="tblRefFoodSpoilage" runat="server" style="display: none">
                            <tr>
                                <td>
                                    <label for="<%=txtRefFoodSpoilageIncludedLimit.ClientID%>">Included Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtRefFoodSpoilageIncludedLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtRefFoodSpoilageIncreaseLimit.ClientID%>">Increased Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtRefFoodSpoilageIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtRefFoodSpoilageTotalLimit.ClientID%>">Total Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtRefFoodSpoilageTotalLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
            </div>
            <asp:HiddenField ID="hiddenFarmPropertyCoverage" runat="server" />
        </div>
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" Visible="false" />
        <asp:HiddenField ID="hiddenPolicyLevel" runat="server" />
    </div>
    <asp:HiddenField ID="hiddenPolicyHolderType" runat="server" />
</div>
<%--<center>
<div class="standardSubSection">
    <asp:Button TabIndex="33" ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" ToolTip="Saves Policy Level Coverages." Text="Save Policy Level Coverages" />
    <asp:Button TabIndex="34" ID="btnSaveGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Locations Page" /><br />
</div>
</center>--%>
<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Saves Policy Level Coverages." Text="Save Policy Level Coverages" />
    <asp:Button ID="btnSaveGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Locations Page" />
    <asp:Button ID="btnRate" ToolTip="Save all Applicant Information and Rate" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Rate this Quote" /><br />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" />
    <asp:Button ID="btnViewGotoDrivers" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Locations Page" />
    <asp:Button ID="btnRate_Endorsements" ToolTip="Save all Applicant Information and Rate" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Rate this Quote" />
</div>
<br />

<div align="center">
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
</div>

<%--Farm All Star Popup--%>
<div id="dvAllStarInfoPopup" style="display: none">
    <div>
        This endorsement provides a "package" of supplemental property and liability coverages including:
        <table style="width: 100%">
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Up to $200 to remove debris from trees, shrubs or plants located within 250 feet of the insured dwelling when the debris is the result of windstorm, hail or weight of ice, snow or sleet
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Replacement cost coverage for well pumps
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Additional peril of landslide for Cov. A, B, C and D (within certain exceptions)
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Up to $100,000 for direct physical loss caused by water that backs up through sewers or drains or overflows from a sump pump
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image5" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Up to $500 for replacement of locks on exterior doors and garage door transmitters if keys or transmitters are lost or stolen
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image6" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Up to $1,000 for open perils coverage on household personal property
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image7" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Personal injury liability coverage
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image8" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Watercraft liability for outboard motors without any horsepower limitation
                </td>
            </tr>
        </table>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnASOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<%--Personal Liability Popup--%>
<%--<div id="dvPersLiabPopup" style="display: none">
    <div>
        If liability coverage is written on the GL-610 (Commercial Liability Coverage Form), endorsement GL-9 may be used to provide personal liability coverage for corporate officers and stockholders who do not have personal liability coverage elsewhere.
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnPLOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>--%>

<%--Additional Insured Popup--%>
<div id="dvAIPopup" style="display: none">
    <div>
        FO-41 Additional Insured - Property Only
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach FO-41 to show the corporation as an additional insured for its ownership interest in any real or farm personal property used in the farming operation.
                </td>
            </tr>
        </table>
        <br />
        GL-108 Additional Insured - Commercial
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach GL-108 to show the farmowner policy includes commercial liability coverage to cover the premises-related liability exposures of co-owners, controlling interests, mortgagees, assignees, and receivers.
                </td>
            </tr>
        </table>
        <br />
        GL-70 Additional Insured - Persons or Organizations
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach GL-70 to show any corporation or any other entities, other than the Named Insured, as additional insureds in order to provide them with premises liability coverage for the farming operations.
                </td>
            </tr>
        </table>
        <br />
        GL-71 Additional Insured - Partners, Corporate Officers or Co-Owners
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach GL-71 to provide liability coverage for the personal activities outside the farming operations for corporate officers and stockholders who are not residents of the Named Insured's household.
                </td>
            </tr>
        </table>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnAIOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<%--Equipment Breakdown Popup--%>
<div id="dvEquipBreakPopup" style="display: none">
    <div>
        Forms FO-3, FO-0005 and FO-6. Provides coverage for equipment breakdown resulting from mechanical, electrical, or pressurized system breakdowns. The following are included:
        <table style="width: 100%">
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image9" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Pays costs to remove pollutants released as a result of a covered loss. 
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image10" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Covers reasonable costs to make temporary repairs, expedite permanent repairs; and/or expedite permanent replacement.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image11" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Loss to perishable goods due to spoilage caused by the lack of power, light, heat, steam or refrigeration caused by "equipment breakdown".
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image12" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Pays for loss from on premises contamination by refrigerant used in refrigeration, cooling, or humidity control as a result of "equipment breakdown".
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image13" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Covers additional costs to repair or replace covered property because of the use or presence of refrigerant containing CFC.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image14" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Pays for loss or damage to your computers caused by "equipment breakdown".
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image15" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Provides up to $25,000 of coverage for the loss of livestock (including poultry) due to "equipment breakdown".
                </td>
            </tr>
        </table>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnEqipBreakOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<%--Extra Expense Popup--%>
<div id="dvExtraExpensePopup" style="display: none">
    <div>
        Covers expenses incurred in order to resume or continue normal farming operations that were interrupted as a result of direct physical loss by a peril insured against to property covered under Coverage E or Coverage F or Coverage G.
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnExtraExpenseOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<%--Farm Extender--%>
<div id="dvFarmExtenderPopup" style="display: none">
    <div>
        Provides a combination of supplemental property and liability coverages including:
        <table style="width: 100%">
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image16" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Collision damage to buildings insured under coverage E. 
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image17" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Additional perils- Livestock other than sheep extends the coverage for livestock OTHER THAN SHEEP to include loss resulting from death caused  by accidental shooting, drowning, attack by dogs, or wild animals, collapse of a building, freezing or smothering in a snowstorm or ice storm or falling through the ice.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image18" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Extension of Damage provides up to $1,000 of coverage to property of Others for livestock in “your” care.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image19" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Farm Fences provides up to $1,000 of coverage for damage from a motor vehicle with stipulations for fences more than 250 feet from the dwelling.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image20" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Well pumps provides up to $1,500 of coverage for direct physical loss to well pumps caused by a peril that applies to Coverage E, F, and G.
                </td>
            </tr>
        </table>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnFarmExtenderOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

    <%--Updated Farm Extender Effective June 2016--%>
    <div id="dvUpdatedFarmExtenderPopup" style="display: none">
    <div>
        The Indiana Farmers Farm Extender Endorsement is specifically designed to enhance the protection provided by the FO-6 Farm Coverage Form. For an annual premium of $25, the following coverages are included:
        <table style="width: 100%">
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image21" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Collision damage to buildings insured under coverage E provides coverage for collision damage to buildings insured under Coverage E, caused by the insured.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px; vertical-align: top" align="right">
                    <asp:Image ID="Image22" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Additional perils- Livestock Other Than Sheep extends the coverage for livestock, other than sheep, to include loss resulting from death caused  by accidental shooting, drowning, attack by dogs or wild animals, collapse of a building, freezing or smothering in a snow storm or ice storm or falling through the ice.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image23" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Extension of Damage to Property of Others provides up to $1,000 of coverage for damage to property of others caused by livestock you own or in your care.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image24" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Farm Fences provides up to $1,000 of coverage for damage from a motor vehicle with stipulations for fences more than 250 feet from the dwelling.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image25" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Well pumps provides up to $1,500 of coverage for direct physical loss to well pumps caused by a peril that applies to Coverage E, F, and G.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image26" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Farm Machinery Towing and Labor Costs Offers up to $250 of coverage for disabled farm machinery. No deductible applies for this coverage.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image27" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Non-depreciation of Partial Losses for Farm Machinery for partial losses on farm machinery five years old or newer, a non-depreciation loss settlement provisions is added.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image28" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Rental Reimbursement Expense for Mobile Farm Machinery and Equipment if mobile farm machinery or equipment is inoperable due to a covered cause of loss, additional coverage up to $250 per day with a maximum of $5000 is available. No deductible applies for this coverage.
                </td>
            </tr>
            <tr style="vertical-align:top">
                <td style="width: 30px" align="right">
                    <asp:Image ID="Image29" runat="server" ImageUrl="~/images/blkdot.png" />
                </td>
                <td>Fire Extinguisher Recharge Expense Affords up to $1500 of coverage for fire extinguishers or systems that were discharged. No Deductible applies for this coverage.
                </td>
            </tr>
        </table>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnUpdFarmExtenderOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>