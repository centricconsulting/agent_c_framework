<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_GeneralRemarks.ascx.vb" Inherits="IFM.VR.Web.ctl_App_GeneralRemarks" %>

<div id="divGeneralRemarks" runat="server">
    <h3>
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
        General Remarks
    </h3>
    <div style="text-align: center;">
        <label for="<%=txtRemarks.ClientID%>">Remarks</label>
        <asp:TextBox runat="server" ID="txtRemarks" Width="60%"></asp:TextBox>
    </div>
</div>
<asp:HiddenField ID="accordActive" runat="server" />