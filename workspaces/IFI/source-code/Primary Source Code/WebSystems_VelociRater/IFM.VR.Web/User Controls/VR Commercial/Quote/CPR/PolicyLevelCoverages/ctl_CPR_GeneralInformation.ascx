<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_GeneralInformation.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_GeneralInformation" %>

<div runat="server" id="divMain">
    <h3>General Information
         <span style="float: right;">        
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
    </h3>
    <div>
        <style type="text/css">
            .CPR_GI_LabelColumn {
                width:50%;
                text-align:left;
            }
            .CPR_GI_DataColumn {
                width:50%;
                text-align:left;
            }
        </style>
        <table id="tblGeneralInfo" runat="server" style="width:100%">
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddProgramType.ClientID%>">*Program Type</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddProgramType" runat="server" >
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddOccuranceLibLimit.ClientID%>">*Occurrence Liability Limit</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddOccuranceLibLimit" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddGeneralAgg.ClientID%>">*General Aggregate</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddGeneralAgg" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=txtRented.ClientID%>">Damage to Premises Rented to You</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:TextBox ID="txtRented" Enabled="false" runat="server" value="100,000"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddOperationsAgg.ClientID%>">*Product/Completed Operations Aggregate</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddOperationsAgg" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=txtMedicalExpense.ClientID%>">Medical Expenses</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:TextBox ID="txtMedicalExpense" runat="server" Enabled="False" Width="130px" Text="5,000"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddPersonalInjury.ClientID%>">*Personal and Advertising Injury</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddPersonalInjury" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPR_Help_BusinessMasterSummary")%>"><font style="color:blue;" >General Liability Enhancement Endorsement</font></a>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:CheckBox ID="chkGLEnhancement" runat="server" Text="&nbsp;" />
                </td>
            </tr>
            <tr id="trAddGLBlanketWaiverOfSubroRow" runat="server">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddlAddlBlanketOfSubroOptions.ClientID%>">Add Blanket Waiver of Subrogation?</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropdownList ID="ddlAddlBlanketOfSubroOptions" runat="server">
                        <asp:ListItem Text="No" Value=""></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Yes with Completed Ops" Value="2"></asp:ListItem>                 
                    </asp:DropdownList>                
                </td>
            </tr>
            <tr id="trEnhancementMessageRow" runat="server">
                <td colspan="2">
                    <p class="informationalText">If General Liability Enhancement Endorsement is selected, and Blanket Waiver of Subrogation is yes the Contractors Supplemental Application must be comnpleted and mailed to your underwriter to bind coverage.  Please click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPR_Help_ContractorsApplication")%>"><font style="color:blue;" >Contractors Application</font></a>.</p>
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=chkAddaGeneralLiabilityDeductible.ClientID%>">Add a General Liability Deductible?</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:CheckBox ID="chkAddaGeneralLiabilityDeductible" runat="server" Text="&nbsp;" />
                </td>
            </tr>
        </table>

        <div id="divDeductibles" runat="server">
            <h3>
                Deductibles
                 <span style="float: right;">
                     <asp:LinkButton ID="btnSaveDeductible" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
                 </span>
            </h3>
            <div runat="server" id="divDeductiblesData">
                <table>
                    <tr>
                        <td class="CPR_GI_LabelColumn">
                            <label for="<%=ddType.ClientID%>">*Type</label>
                        </td>
                        <td class="CPR_GI_DataColumn">
                            <asp:DropDownList ID="ddType" runat="server" Width="230px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="CPR_GI_LabelColumn">
                            <label for="<%=ddAmount.ClientID%>">*Amount</label>
                        </td>
                        <td class="CPR_GI_DataColumn">
                            <asp:DropDownList ID="ddAmount" runat="server" Width="230px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="CPR_GI_LabelColumn">
                            <label for="<%=ddBasis.ClientID%>">*Basis</label>
                        </td>
                        <td class="CPR_GI_DataColumn">
                            <asp:DropDownList ID="ddBasis" runat="server" Width="230px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdnAccord" runat="server" />
<asp:HiddenField ID="hdnDeductibleAccord" runat="server" />

