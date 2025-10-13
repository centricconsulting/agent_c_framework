<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_Remarks.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_Remarks" %>

<div runat="server" id="divMain">
    <h3>General Remarks<span style="float: right;">
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
    </span></h3>
    <div>
        <div style="text-align: center;">
            <label for="<%=txtRemarks.ClientID %>">Remarks</label>
            <asp:TextBox ID="txtRemarks" MaxLength="255" runat="server"></asp:TextBox>
        </div>
    </div>
</div>