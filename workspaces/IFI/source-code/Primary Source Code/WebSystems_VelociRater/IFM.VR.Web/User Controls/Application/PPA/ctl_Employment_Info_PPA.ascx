<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Employment_Info_PPA.ascx.vb" Inherits="IFM.VR.Web.ctl_Employment_Info_PPA" %>

<div runat="server" id="divMainEmployment" class="standardSubSection">
    <h3>Employment Information
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearBase" runat="server" CssClass="RemovePanelLink" ToolTip="Clear Employment Information">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table style="width: 100%">
            <tr>
                <td>
                    <label for="<%=Me.txtEmployerName.ClientID%>">Employer Name</label></td>
                <td>
                    <asp:TextBox ID="txtEmployerName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddIccupation.ClientID%>">Occupation</label></td>
                <td>
                    <asp:DropDownList ID="ddIccupation" runat="server"></asp:DropDownList></td>
            </tr>
        </table>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />