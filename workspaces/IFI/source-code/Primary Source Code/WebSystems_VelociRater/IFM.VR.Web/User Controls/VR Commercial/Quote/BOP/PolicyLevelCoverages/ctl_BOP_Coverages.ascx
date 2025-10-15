<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_Coverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_CTEQList.ascx" TagPrefix="uc1" TagName="ctl_ContEQList" %>

<div runat="server" id="divMain">
    <h3>Policy Level Coverages
         <span style="float: right;">
             <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
         </span>
    </h3>
    <div id="divBOPPolicyLevelCoverages">
        <style type="text/css">
            .SpacerColumn {
                width:3%;
            }
            .LabelColumn {
                text-align:left;
                width:67%;
            }
            .DataColumn {
                text-align:left;
                width:30%;
            }
            .WideControl{width:95%;}
            .MediumControl{width:60%;}
            .NarrowControl{width:25%;}
        </style>
        <table id="tblCoverages" runat="server">
            <tr>
                <td>
                    <asp:CheckBox ID="chkAdditionalInsured" runat="server" Text="Additional Insured" />
                </td>
            </tr>
            <tr id="trAdditionalInsuredsRow" runat="server" style="display:none;">
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                How many of the following Additional Insureds Apply?
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlNumberOfAddlInsureds" runat="server" CssClass="MediumControl">
                                    <asp:ListItem Value="0">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                    <asp:ListItem Value="4">4 or more</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2" >
                                <ul>
                                    <li>Vendors</li>
                                    <li>Designated Person or Organization</li>
                                    <li>Engineers, Architects, or Surveyors not Engaged by the Named Insured</li>
                                    <li>Owners, Lessees or Contractors </li>
                                    <li>Lessor of Leased Equipment </li>
                                    <li>Managers or Lessors of Premises </li>
                                    <%--<li>Owners, Lessees, or Contractors with Additional Insured Requirement in Construction Contract</li>--%>
                                    <li>Grantor of Franchise</li>
                                </ul>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2">
                                <table style="width:100%;">
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            Check the boxes if the policy has any of the following AI:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_TownhouseAssociates" runat="server"/>Townhouse Associates
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_EngineersArchitectsSurveyors" runat="server"/>Engineers, Architects, or Surveyors
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkOwnersLesseesContractorsAutomatic" runat="server"/>Owners, Lessees or Contractors - Automatic w/Completed Ops and Waiver
                                        </td>
                                    </tr>
                                    <tr id="trOwnersLesseesContractorsAutomaticInfoRow" runat="server" style="display:none;">
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2" class="informationalText">
                                            This Additional Insured option requires ‘Owners, Lessees or Contractors – With Additional Insured Requirement of Construction Contract’
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkWaiverOfSubrogation" runat="server"/>Waiver of Subrogation when Required by Written Contract or Agreement
                                        </td>
                                    </tr>
                                    <tr id="trWaiverOfSubroInfoRow" runat="server" style="display:none;">
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2" class="informationalText">
                                            This Additional Insured option requires ‘Owners, Lessees or Contractors – With Additional Insured Requirement of Construction Contract’
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkOwnersLesseesContractorsWithAddlInsuredReq" runat="server"/>Owners, Lessees or Contractors - w/ AI Requirement of Construction Contract
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkOwnersLesseesContractorsCompletedOps" runat="server"/>Owners, Lessees or Contractors - Completed Operation
                                        </td>
                                    </tr>
                                    <tr id="trNumberOfAITextboxRow" runat="server" style="display:none;">
                                        <td class="SpacerColumn"></td>
                                        <td class="LabelColumn">
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            Number of AI (O, L, or C - Comp. Ops.)
                                        </td>
                                        <td class="DataColumn">
                                           <asp:TextBox ID="txtNumOfAI" runat="server" CssClass="NarrowControl" style="margin-left:-5px;" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr id="trOwnersLesseesContractorsCompletedOpsInfoRow" runat="server" style="display:none;">
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2" class="informationalText">
                                            This additional insured option requires an Additional Insured of type ‘Owners, Lessees or Contractors’
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--start--%>
            <tr id="chkCLIWrapper" runat="server">
                <td>
                    <asp:CheckBox ID="chkCLI" Text="Cyber Liability" runat="server" />
                </td>
            </tr>
            <tr id="trCLIRow" runat="server" style="display:none;width:100%;">
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td style="text-align:left;width:24%">
                                Aggregate Limit
                            </td>
                            <td style="text-align:left;width:24%">
                                $50,000
                            </td>
                            <td style="text-align:left;width:24%">
                                Deductible
                            </td>
                            <td style="text-align:left;width:24%">
                                $2,500
                            </td>
                        </tr>
                        <tr id="trCyberLiabilityInfoRow" runat="server">
                            <td colspan="5">
                                <p class="informationalText">*For higher limits please contact your Underwriter</p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--end--%>
            <tr>
                <td>
                    <asp:CheckBox ID="chkEmployeeBenefitsLiability" Text="Employee Benefits Liability" runat="server" />
                </td>
            </tr>
            <tr id="trEmployeeBenefitsRow" runat="server" style="display:none;" >
                <td>
                    <table>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtEBLNumberOfEmployees.ClientID%>">Number of Employees</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtEBLNumberOfEmployees" runat="server" CssClass="NarrowControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trEmployeeBenefitsLinkAndInfoRow" runat="server">
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2" class="informationalText">
                                <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_EmployeeBenefitsApp")%>">Click here for the Employee Benefits Liability Application</a>
                                <br /><br /> 
                                *The Employee Benefits Liability will default to the BOP Occurrence Limit. Please contact your underwriter if a lower Employee Benefits Liability is required.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkEPLI" runat="server" Text="Employment Practices Liability - Claims-Made Basis" />
                </td>
            </tr>
            <tr id="trEPLIRow" runat="server" style="display:none;width:100%;">
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td style="text-align:left;width:24%">
                                Each "Claim" Limit
                            </td>
                            <td style="text-align:left;width:24%">
                                $100,000
                            </td>
                            <td style="text-align:left;width:24%">
                                Aggregate Limit
                            </td>
                            <td style="text-align:left;width:24%">
                                $100,000
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td style="text-align:left;width:24%">
                                Deductible
                            </td>
                            <td style="text-align:left;width:24%">
                                $5,000
                            </td>
                            <td style="text-align:left;width:24%">
                                Retroactive Date
                            </td>
                            <td style="text-align:left;width:24%">
                                <asp:Label ID="lblRetroactiveDate" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr id="trEmploymentPracticesInfoRow" runat="server">
                            <td colspan="5">
                                <p class="informationalText">*For Underwritten product please contact your Underwriter.</p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trStopGapCheckboxRow" runat="server" style="display:none;width:100%;">
                <td>
                    <asp:CheckBox ID="chkStopGap" runat="server" Text="Stop Gap (OH)" />&nbsp;
                </td>
            </tr>
            <tr id="trStopGapDataRow" runat="server" style="width:100%;text-indent:15px;">
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td>
                                <asp:Label ID="lblStopGapLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblStopGapPayroll" runat="server" Text="Ohio Only Payroll"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddStopGapLimit" runat="server" ></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtStopGapPayroll" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkContractorsEquipmentInstallation" Text="Contractors Equipment/Installation" runat="server" />
                </td>
            </tr>
            <tr id="trContractorsRow" runat="server" style="display:none;">
                <td>
                    <table>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=ddlContractorsPropertyLimitAtEachCoveredJobsite.ClientID%>">Contractors Installation Property Limit at each covered site *</label>
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlContractorsPropertyLimitAtEachCoveredJobsite" runat="server" CssClass="MediumControl"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtContractorsToolsAndEquipmentBlanketLimit.ClientID %>">Contractors Tools and Equipment - Blanket Limit **</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtContractorsToolsAndEquipmentBlanketLimit" runat="server" CssClass="MediumControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trContractorsToolsAndEquipmentSubLimitRow" runat="server" style="display:none;">
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=ddlContractorsToolsAndEquipmentBlanketSubLimit.ClientID %>">Contractors Tools and Equipment - Blanket Sub-Limit</label>
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlContractorsToolsAndEquipmentBlanketSubLimit" runat="server">
                                    <asp:ListItem Value="15">Up to $500</asp:ListItem>
                                    <asp:ListItem Value="16" Selected="True">Up to $2,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtContractorsToolsAndEquipmentScheduledLimit.ClientID%>">Contractors Tools and Equipment - Scheduled Limit ***</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtContractorsToolsAndEquipmentScheduledLimit" runat="server" CssClass="MediumControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trContractorsEquipmentScheduled" runat="server">
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2">
                                <uc1:ctl_ContEQList runat="server" ID="ctl_ContractorsEQList" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtContractorsRentedLeasedToolsAndEquipmentLimit.ClientID%>">Rented/Leased Tools and Equipment Limit</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtContractorsRentedLeasedToolsAndEquipmentLimit" runat="server" CssClass="MediumControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtContractorsEmployeesToolsLimit.ClientID%>">Contractors Employees Tools Limit</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtContractorsEmployeesToolsLimit" runat="server" CssClass="MediumControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trContractorsInfoRow" runat="server">
                            <td colspan="3">
                                <p class="informationalText">*The BOP includes coverage for Contractors Installation Property with limits of $3,000 per jobsite. Please select the additional amount of coverage greater than the $3,000 limit included in the endorsement.</p>
                                <p class="informationalText">**Not in excess of $2,000 for any one item</p>
                                <p class="informationalText">***Schedule required to issue</p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkCrime" Text="Crime" runat="server" />
                </td>
            </tr>
            <tr id="trCrimeRow" runat="server" style="display:none;" >
                <td>
                    <table>
                        <tr>
                            <td colspan="3" style="text-align:left;font-weight:bold;width:100%;"><label>Employee Dishonesty:</label></td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtCrimeNumberOfEmployees.ClientID %>">*Number of Employees</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtCrimeNumberOfEmployees" runat="server" CssClass="NarrowControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=txtCrimeNumberOfLocations.ClientID%>">*Number of Additonal Locations</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtCrimeNumberOfLocations" runat="server" CssClass="NarrowControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=ddlCrimeTotalLimit.ClientID %>">*Total Limit</label>
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlCrimeTotalLimit" runat="server" CssClass="MediumControl"></asp:DropDownList>  
                            </td>
                        </tr>
                        <tr id="trForgeryAndAlterationsLimitRow" runat="server" >
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                <label for="<%=chkForgeryAlterationOptionalLimits.ClientID %>">Forgery and Alteration Optional Limits</label>
                            </td>
                            <td class="DataColumn">
                                <asp:CheckBox ID="chkForgeryAlterationOptionalLimits" runat="server" />
<%--                                <asp:DropDownList ID="ddlForgeryAndAlterationsOptionalLimit" runat="server" 
                                    Width="150px">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem Value="15">5,000</asp:ListItem>
                                    <asp:ListItem Value="7">10,000</asp:ListItem>
                                    <asp:ListItem Value="8">25,000</asp:ListItem>
                                    <asp:ListItem Value="9">50,000</asp:ListItem>
                                    <asp:ListItem Value="10">100,000</asp:ListItem>
                                </asp:DropDownList>--%>
                            </td>
                        </tr>
                        <tr id="trCrimeInfoRow" runat="server">
                            <td colspan="3">
                                <p class="informationalText">The BOP Enhancement Endorsement includes coverage for the Employee Dishonesty with limts of $10,000. If a higher limit is desired, please select a total limit. </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkEarthquake" Text="Earthquake" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkHiredAuto" runat="server" Text="Hired Auto"  />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkNonOwned" runat="server" Text="Non-Owned" />
                </td>
            </tr>
            <tr id="trNonOwnedInfoRow" runat="server" style="display:none;" >
                <td>
                    <p class="informationalText">VelociRater will only quote without delivery service. Contact Underwriter if delivery service needed.</p>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkElectronicData" Text="Electronic Data" runat="server" />
                </td>
            </tr>
            <tr id="trElectronicDataRow" runat="server" style="display:none;">
                <td>
                    <table>
                        <tr>
                            <td class="SpacerColumn"></td>
                            <td class="LabelColumn">
                                <label for="<%=txtElectronicDataLimit.ClientID%>">Limit</label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtElectronicDataLimit" runat="server" CssClass="MediumControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trElectronicDataInfoRow" runat="server">
                            <td colspan="3">
                                <p class="informationalText">This coverage includes a limit of 10,000 for Electronic Data damaged by computer virus, harmful code or similar instruction introduced into computer system or network to destroy or disrupt its normal operation. If a higher limit is desired, please enter the total limit. </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trILContractorsHomeRepairAndRemodelingCheckboxRow" runat="server" style="display:none;">
                <td>
                    <asp:CheckBox ID="chkILContractorsHomeRepairAndRemodeling" Text="IL Contractors - Home Repair & Remodeling" runat="server" />
                </td>
            </tr>
            <tr id="trILContractorsHomeRepairAndRemodelingDataRow" runat="server" style="display:none;">
                <td>
                    <table>
                        <tr>
                            <td class="SpacerColumn"></td>
                            <td colspan="2">
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                Limit: $10,000
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hddAccord" runat="server" />
    </div>
</div>
<asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
<asp:Panel ID="pnlBasicButtons" runat="server">
    <table style="width: 100%">
        <tr>
            <td colspan="2">&nbsp;</td>
        </tr>
        <tr>
            <td style="width:50%;text-align:right;">
                <asp:Button ID="btnSavePolicyLevelCoverages" runat="server" Text="Save Policy Level Coverages" CssClass="StandardSaveButton" />
                <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>            
            </td>
            <td style="width:50%;text-align:left;">
                <asp:Button ID="btnLocations" runat="server" Text="Locations" CssClass="StandardSaveButton" />
            </td>
        </tr>
    </table>
</asp:Panel>
