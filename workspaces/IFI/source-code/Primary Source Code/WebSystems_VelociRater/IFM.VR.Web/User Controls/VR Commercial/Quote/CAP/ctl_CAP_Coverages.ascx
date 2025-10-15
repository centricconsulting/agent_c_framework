<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_Coverages" %>

<div runat="server" id="divMain">
    <h3>Policy Level Coverages
         <span style="float: right;">
             <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
         </span>
    </h3>
    <div id="divPolicyLevelCoverages">
        <style type="text/css">
            .CAPCOVColumn1 {
                text-align:right;
                width:25%;
            }
            .CAPCOVColumn2 {
                text-align:left;
                width:40%;
            }
            .CAPCOVColumn3 {
                text-align:left;
                width:30%;
            }
            .CAPCOVSpacerColumn {
                width:5%;
            }
            .CAPCOVWideControl{width:95%;}
            .CAPCOVMediumControl{width:60%;}
            .CAPCOVMediumControl40{width:40%;}
            .CAPCOVNarrowControl{width:25%;}
            .CAPCOVSubTableLabelRow {
                width:60%;
                text-align:right;
            }
            .CAPCOVSubTableDataRow {
                width:40%;
                text-align:left;
            }
            .CAPGKSubTableLabelRow {
                width:50%;
                text-align:right;
            }
            .CAPGKSubTableDataRow {
                width:50%;
                text-align:left;
            }
        </style>
        <table id="tblCoverages" runat="server" style="width:100%;">
            <tr id="trLiabilityUMUIM_Combined" runat="server" style="display:none;">
                <td class="CAPCOVColumn1">Liability / UM / UIM</td>
                <td class="CAPCOVColumn2">
                    <asp:DropDownList ID="ddlLiabilityUMUIM" runat="server" CssClass="CAPCOVMediumControl"></asp:DropDownList>
                </td>
                <td class="CAPCOVColumn3">&nbsp;</td>                
            </tr>
            <tr id="trLiabilityUMUIM_Separate" style="display:none;">
                <td colspan="3">  <!-- Note: this section replaces the 'Liability / UM / UIM' section above (multistate) MGB 9/25/18 -->
                    <table style="width:100%;">
                        <tr id="PersonalLiabilitylimitTextCAP" runat="server" Visible="false">
                            <td colspan="3" class="informationalText" id="tdMinCombinedSingleLiabilityLimitInfo" runat="server">
                                <span>We require a minimum combined single liability limit of:<br />
                                    $300,000 when quoting a farm umbrella and $500,000 when quoting a commercial umbrella.</span>
                            </td>
                        </tr>
                        <tr>
                            <td id="LiabilityCol" runat="server">
                                Liability
                                <asp:DropDownList ID="ddLiability" runat="server"></asp:DropDownList>
                            </td>
                            <td id="UMCol" runat="server">
                                UM
                                <asp:DropDownList ID="ddUM" runat="server"></asp:DropDownList>
                                <asp:HiddenField ID="hdnUMPDLimitValue" runat="server" />
                            </td>
                            <td id="UIMCol" runat="server">
                                UIM
                                <asp:TextBox ID="txtUIM" runat="server" ReadOnly="true" BackColor="LightGray"></asp:TextBox>
                            </td>
                            <td id="UMPDCol" runat="server">
                                UMPD
                                <asp:DropDownList ID="ddlUMPDLimit" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trUMPDRow" runat="server" Visible="false">
                            <td colspan="3" style="padding-left:172px;">
                                UMPD<asp:CheckBox ID="chkUMPDCov" runat="server" Text="" />
                                <div id="divUMPDDedOptions" runat="server" style="display:none;">UMPD Deductible Options <asp:DropDownList ID="ddUMPDDedOptions" runat="server"></asp:DropDownList></div>
                            </td>
                        </tr>
                        <tr id="trLowerLimitsInfoRow" runat="server" Visible="false">
                            <td colspan="3" class="informationalText txtCnt" id="tdLowerLimitsInfo" runat="server">
                                <span>If you want to select limits lower than shown, please contact your underwriter. Signed lower limit/rejection form will be required.</span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="CAPCOVColumn1">Medical Payments</td>
                <td class="CAPCOVColumn2">
                    <asp:DropDownList ID="ddlMedicalPayments" runat="server" CssClass="CAPCOVMediumControl"></asp:DropDownList>
                </td>
                <td class="CAPCOVColumn3">&nbsp;</td>                
            </tr>
            <tr>
                <td class="CAPCOVColumn1">
                    &nbsp;
                </td>
                <td class="CAPCOVColumn2">
                    <asp:CheckBox ID="chkEnhancement" runat="server" style="margin-right:0px;" />
                    <label style="margin-left:-3px;" for="<%=chkEnhancement.ClientID%>"><a target="_blank" id="lnkCAPHelp_EnhancementEndorsement" runat="server" href="">Enhancement Endorsement</a></label>
                    <%--<a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_EmployeeBenefitsApp")%>">Enhancement Endorsement</a>--%>
                </td>
                <td class="CAPCOVColumn3">&nbsp;</td>
            </tr>
            <tr>
                <td class="CAPCOVColumn1">
                    &nbsp;
                </td>
                <td class="CAPCOVColumn2">
                    <asp:CheckBox ID="chkBlanketWaiverOfSubro" runat="server" Text="Blanket Waiver of Subro" />
                    <%--Blanket Waiver of Subro--%>
                </td>
                <td class="CAPCOVColumn3">&nbsp;</td>
            </tr>
            <tr>
                <td class="CAPCOVColumn1">
                    &nbsp;
                </td>
                <td class="CAPCOVColumn2">
                    <asp:CheckBox ID="chkHiredBorrowedNonOwned" Text="Hired / Borrowed / Non-Owned" runat="server" />
                </td>
                <td class="CAPCOVColumn3"></td>
            </tr>
            <tr id="trHiredNonOwnedInfoRow" runat="server" style="display:none;">
                <td colspan="3" style="text-align:center" class="informationalText">
                    In IL the Hired/Borrowed/Non-Owned coverage includes UM/UIM coverage.
                </td>
            </tr>
            <tr id="trHiredNonOwnedInfoRowOther" runat="server">
                <td colspan="3" style="text-align:center" class="informationalText">
                    Select Hired/Non-Owned Liability when using Covered Auto Symbol 1.
                </td>
            </tr>
            <tr id="trHiredBorrowedNonOwnedDataRow" runat="server">
                <td class="CAPCOVColumn1">&nbsp;</td>
                <td colspan="2">
                    <table style="width:100%;">
                        <tr>
                            <td style="text-align:left;text-indent:15px;">
                                <asp:CheckBox ID="chkNonOwnershipLiability" runat="server" Text="Non-Ownership Liability" />
                            </td>
                        </tr>
                        <tr id="trNonOwnershipLiabilityDataRow" runat="server">
                            <td style="width:100%;">
                                <table style="width:100%;">
                                    <tr>
                                        <td class="CAPCOVSubTableLabelRow">
                                            <label for="<%=txtNonOwnedNumberOfEmployees.ClientID%>">Number of Employees</label>
                                        </td>
                                        <td class="CAPCOVSubTableDataRow">
                                            <asp:TextBox ID="txtNonOwnedNumberOfEmployees" runat="server" CssClass="CAPCOVNarrowControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="CAPCOVSubTableLabelRow">
                                            <label for="<%=ddlNonOwnedExtendedNonOwnedLiability.ClientID%>">Extended Non-Owned Liability</label>
                                        </td>
                                        <td class="CAPCOVSubTableDataRow">
                                            <asp:DropDownList ID="ddlNonOwnedExtendedNonOwnedLiability" runat="server" >
                                                <asp:ListItem Value="0" Text="N/A"></asp:ListItem>
                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left;text-indent:15px;">
                                <asp:CheckBox ID="chkHiredBorrowedLiability" runat="server" Text="Hired/Borrowed/Liability" />
                            </td>
                        </tr>
                        <tr id="trHiredBorrowedLiabilityDataRow" runat="server">
                            <td style="width:100%;">
                                <table style="width:100%;">
                                    <tr>
                                        <td class="CAPCOVSpacerColumn"></td>
                                        <td style="text-indent:15px;">
                                            <asp:CheckBox ID="chkHiredCarPhysicalDamage" runat="server" Text="Hired Car Physical Damage" />
                                            &nbsp;
                                            (50,000 Limit)
                                        </td>
                                    </tr>
                                    <tr id="trHiredCarPhysicalDamageDataRow" runat="server">
                                        <td colspan="2">
                                            <table style="width:100%;">
                                                <tr>
                                                    <td class="CAPCOVSubTableLabelRow">
                                                        <label for="<%=ddlHiredCarComprehensive.ClientID%>">Comprehensive</label>
                                                    </td>
                                                    <td class="CAPCOVSubTableDataRow">
                                                        <asp:DropDownList ID="ddlHiredCarComprehensive" runat="server">
                                                            <%--<asp:ListItem Value="0"></asp:ListItem>--%>
                                                            <asp:ListItem Value="10">Full Coverage</asp:ListItem>
                                                            <asp:ListItem Value="1">50</asp:ListItem>
                                                            <asp:ListItem Value="2">100</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="CAPCOVSubTableLabelRow">
                                                        <label for="<%=ddlHiredCarCollision.ClientID%>">Collision</label>
                                                    </td>
                                                    <td class="CAPCOVSubTableDataRow">
                                                        <asp:DropDownList ID="ddlHiredCarCollision" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="CAPCOVColumn1">
                    &nbsp;
                </td>
                <td class="CAPCOVColumn2">
                    <asp:CheckBox ID="chkFarmPollutionLiability" runat="server" Text="Farm Pollution Liability" />
                </td>
                <td class="CAPCOVColumn3">&nbsp;</td>
            </tr>
            <tr>
                <td class="CAPCOVColumn1">
                    &nbsp;
                </td>
                <td class="CAPCOVColumn2">
                    <asp:CheckBox ID="chkGarageKeepers" runat="server" Text="Garage Keepers" />
                </td>
                <td class="CAPCOVColumn3">&nbsp;</td>
            </tr>
            <tr id="trGarageKeepersDataRow" runat="server">
                <td colspan="3">
                    <table style="width:100%;">
                        <tr>
                            <td class="CAPGKSubTableLabelRow">Basis Type</td>
                            <td class="CAPGKSubTableDataRow">
                                <asp:DropDownList ID="ddlBasisType" runat="server">
                                    <asp:ListItem Value="3" Text="Legal Liability"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Direct Primary" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Direct Excess"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CAPGKSubTableLabelRow">Comprehensive Limit</td>
                            <td class="CAPGKSubTableDataRow">
                                <asp:TextBox ID="txtGKComprehensiveLimit" runat="server" CssClass="CAPCOVNarrowControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CAPGKSubTableLabelRow">Deductible</td>
                            <td class="CAPGKSubTableDataRow">
                                <asp:DropDownList ID="ddlGKComprehensiveDeductible" runat="server" CssClass="CAPCOVMediumControl40"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CAPGKSubTableLabelRow">Collision Limit</td>
                            <td class="CAPGKSubTableDataRow">
                                <asp:TextBox ID="txtGKCollisionLimit" runat="server" CssClass="CAPCOVNarrowControl" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CAPGKSubTableLabelRow">Deductible</td>
                            <td class="CAPGKSubTableDataRow">
                                <asp:DropDownList ID="ddlGKCollisionDeductible" runat="server" CssClass="CAPCOVMediumControl40"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trOHQuestionRow" runat="server">
                <td class="CAPCOVColumn1">
                    <asp:DropDownList ID="ddlOhioQuestion" runat="server">
                        <asp:ListItem Text="N/A" Value="NA"></asp:ListItem>
                        <asp:ListItem Text="No" Value="NO"></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="YES"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td colspan="2" style="text-align:left;">
                    <asp:Label ID="lblOhioQuestion" runat="server" Text="Are you engaged in Sale of Vehicles, Repair Shops, or Public Parking?"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hddAccord" runat="server" />
    </div>
</div>
<asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
<asp:Panel ID="pnlBasicButtons" runat="server">
    <div id="divActionButtons" runat="server">
    <table style="width: 100%">
        <tr>
            <td colspan="2">&nbsp;</td>
        </tr>
        <tr>
            <td style="width:50%;text-align:right;">
                <asp:Button ID="btnSavePolicyLevelCoverages" runat="server" Text="Save Policy Level Coverages" CssClass="StandardSaveButton" />
            </td>
            <td style="width:50%;text-align:left;">
                <asp:Button ID="btnVehicles" runat="server" Text="Vehicles" CssClass="StandardSaveButton" />
            </td>
        </tr>
    </table>
    </div>
    <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
        <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
        <asp:Button ID="btnViewGotoDrivers" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Drivers Page" />
    </div>
</asp:Panel>
