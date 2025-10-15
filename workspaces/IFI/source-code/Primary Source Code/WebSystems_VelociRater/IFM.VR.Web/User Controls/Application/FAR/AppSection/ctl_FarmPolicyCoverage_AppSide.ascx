<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FarmPolicyCoverage_AppSide.ascx.vb" Inherits="IFM.VR.Web.ctl_FarmPolicyCoverage_AppSide" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctlFamilyMedicalPayments_App.ascx" TagPrefix="uc1" TagName="ctlFamilyMedicalPayments_App" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/Cov_CanineExclusionList.ascx" TagPrefix="uc1" TagName="Cov_CanineExclusionList" %>


<div runat="server" id="divMain">
    <h3>Policy Level Coverages
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table>
            <tr runat="server" id="trIncidental">
                <td colspan="2">Incidental Business Pursuits
                    <div style="margin-left: 30px;">
                        <table>
                            <tr>
                                <td>
                                    <label for="<%=ddlBPType.ClientID%>">*Type of Business</label></td>
                                <td>
                                    <asp:DropDownList ID="ddlBPType" runat="server" Width="300px"></asp:DropDownList></td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>

            <%--<tr runat="server" id="trCustom">
                <td colspan="2">Custom Farming
                    <div style="margin-left: 30px;">
                        *Describe Farming
                        <asp:TextBox ID="txtDescribeFarming" runat="server"></asp:TextBox>
                    </div>
                </td>
            </tr>--%>

            <tr runat="server" id="trMedical">
                <td colspan="2">
                    <uc1:ctlFamilyMedicalPayments_App runat="server" ID="ctlFamilyMedicalPayments_App" />
                </td>
            </tr>

            <tr runat="server" id="trCanine">
                <td >
                    <div runat="server" id="CanineSection">
                        <h3>Exclusion - Canine (FAR 4003 01 22)
                        <span style="float: right;">
                            <asp:LinkButton ID="lnkCanineBtnClear" CssClass="RemovePanelLink" ToolTip="Clear" runat="server">Clear</asp:LinkButton>
                            <asp:LinkButton ID="lnkCanineBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
                        </span>
                        </h3>
                        <div>
                            <uc1:Cov_CanineExclusionList runat="server" ID="Cov_CanineExclusionList" />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="HiddenField2" runat="server" />