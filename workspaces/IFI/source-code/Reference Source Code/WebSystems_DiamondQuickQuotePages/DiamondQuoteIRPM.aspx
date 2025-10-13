<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="" CodeFile="DiamondQuoteIRPM.aspx.vb" Inherits="DiamondQuoteIRPM_QQ" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <title>Diamond Quote IRPM</title>
    <%--<link href="DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />--%><!--moved to master page 2/22/2013-->
    <script type="text/javascript" src="js/CommonFunctions.js"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Application" Runat="Server">
<br />
    <div>
        <table align="center" class="quickQuoteSectionTable" cellpadding="4">
                        <tr>
                            <td align="center" colspan="4" class="tableRowHeaderLarger">IRPM Information (Credits/Debits)</td>
                        </tr>
                        <tr class="tableRowSpacer">
                            <td colspan="4"></td>
                        </tr>
                        <tr class="quickQuoteSectionTableHeaderRow">
                            <td align="left" colspan="2" class="tableRowHeader">Description</td>
                            <td align="left" class="tableRowHeader">Factor</td>
                            <td align="left" class="tableRowHeader">Remark</td>
                        </tr>
                        <tr runat="server" id="CPP_CPR_HeaderRow">
                            <td colspan="4" align="center" class="tableRowHeaderSmaller">--Property--</td>
                        </tr>
                        <tr runat="server" id="IrpmMgmtCoopRow">
                            <td align="left" class="tableFieldHeader">Management/Cooperation</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_MgmtCoop"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_MgmtCoopDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CPR_MgmtRow">
                            <td align="left" class="tableFieldHeader">Management</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.820/max 1.180</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_Mgmt"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_MgmtDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmLocationRow">
                            <td align="left" class="tableFieldHeader">Location</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_Loc"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_LocDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CPR_LocationRow">
                            <td align="left" class="tableFieldHeader">Location</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.820/max 1.180</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_Loc"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_LocDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmBuildingFeaturesRow">
                            <td align="left" class="tableFieldHeader">Building Features</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_BuildFeat"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_BuildFeatDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CPR_BuildingFeaturesRow">
                            <td align="left" class="tableFieldHeader">Building Features</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.800/max 1.200</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_BuildFeat"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_BuildFeatDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmPremisesRow">
                            <td align="left" class="tableFieldHeader">Premises</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_Premises"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_PremisesDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmEquipmentRow">
                            <td align="left" class="tableFieldHeader">Equipment</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_Equipment"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_EquipmentDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CPR_PremisesAndEquipmentRow">
                            <td align="left" class="tableFieldHeader">Premises and Equipment</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.800/max 1.200</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_PremisesAndEquipment"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CPR_PremisesAndEquipmentDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmEmployeesRow">
                            <td align="left" class="tableFieldHeader">Employees</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_Emp"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_EmpDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmProtectionRow">
                            <td align="left" class="tableFieldHeader">Protection</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_Prot"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_ProtDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmCatHazRow">
                            <td align="left" class="tableFieldHeader">Catastrophic Hazards</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CatHaz"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CatHazDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmMgmtExpRow">
                            <td align="left" class="tableFieldHeader">Management Experience</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_MgmtExp"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_MgmtExpDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmMedFacRow">
                            <td align="left" class="tableFieldHeader">Medical Facilities</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_MedFac"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_MedFacDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="IrpmClsPecRow">
                            <td align="left" class="tableFieldHeader">Classification Peculiarities</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_ClsPec"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_ClsPecDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="CPP_CGL_HeaderRow">
                            <td colspan="4" align="center" class="tableRowHeaderSmaller">--General Liability--</td>
                        </tr>
                        <tr runat="server" id="Irpm_GL_MgmtCoopRow">
                            <td align="left" class="tableFieldHeader">Management/Cooperation</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.850/max 1.150</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_MgmtCoop"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_MgmtCoopDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_GL_LocationRow">
                            <td align="left" class="tableFieldHeader">Location</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_Loc"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_LocDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_GL_PremisesRow">
                            <td align="left" class="tableFieldHeader">Premises</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.850/max 1.150</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_Premises"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_PremisesDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_GL_EquipmentRow">
                            <td align="left" class="tableFieldHeader">Equipment</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.850/max 1.150</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_Equipment"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_EquipmentDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_GL_EmployeesRow">
                            <td align="left" class="tableFieldHeader">Employees</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.850/max 1.150</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_Emp"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_EmpDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_GL_ClsPecRow">
                            <td align="left" class="tableFieldHeader">Classification Peculiarities</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_ClsPec"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_GL_ClsPecDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CAP_MgmtRow">
                            <td align="left" class="tableFieldHeader">Management</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_Mgmt"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_MgmtDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CAP_EmployeesRow">
                            <td align="left" class="tableFieldHeader">Employees</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.850/max 1.150</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_Emp"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_EmpDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CAP_EquipmentRow">
                            <td align="left" class="tableFieldHeader">Equipment</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.850/max 1.150</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_Equipment"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_EquipmentDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr runat="server" id="Irpm_CAP_SafetyOrganizationRow">
                            <td align="left" class="tableFieldHeader">Safety Organization</td>
                            <td align="right" class="tableFieldHeaderSmaller">min 0.900/max 1.100</td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_SafetyOrganization"></asp:Textbox></td>
                            <td align="left" class="tableFieldValue">&nbsp;<asp:Textbox runat="server" ID="txtIRPM_CAP_SafetyOrganizationDesc"></asp:Textbox></td>                            
                        </tr>
                        <tr class="tableRowSpacer">
                            <td colspan="4"></td>
                        </tr>
                        <tr runat="server" id="Percent25MessageRow">
                            <td align="center" colspan="4" class="tableFieldHeaderSmaller">
                                Please note, the total credit cannot exceed 25% or a factor of 0.750
                                <br />
                                Total credits greater than 25% will require Underwriting Approval
                            </td>
                        </tr>
                        <tr runat="server" id="Percent10MessageRow">
                            <td align="center" colspan="4" class="tableFieldHeaderSmaller">
                                Please note, the total credit cannot exceed 10% or a factor of 0.900
                                <br />
                                Total credits greater than 10% will require Underwriting Approval
                            </td>
                        </tr>
                        <tr runat="server" id="CPP_Percent25MessageRow">
                            <td align="center" colspan="4" class="tableFieldHeaderSmaller">
                                Please note, the total credit for Property or General Liability cannot exceed 25% or a factor of 0.750
                                <br />
                                Total credits greater than 25% will require Underwriting Approval
                            </td>
                        </tr>
                        <tr class="tableRowSpacer">
                            <td colspan="4"></td>
                        </tr>
                        <tr>
                            <td align="center" colspan="4"><asp:Button runat="server" ID="btnRateAndSave" Text="Rate And Save" CssClass="quickQuoteButton" />&nbsp;&nbsp;<asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="quickQuoteButton" /></td>
                        </tr>
                    </table>

                    <asp:Label runat="server" ID="lblQuoteId" Visible="false"></asp:Label>
                    <asp:Label runat="server" ID="lblRedirectPage" Visible="false"></asp:Label>
        
        
    </div>
</asp:Content>