<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_PolicyholderFarm_AppSide.ascx.vb" Inherits="IFM.VR.Web.ctl_PolicyholderFarm_AppSide" %>

<div runat="server" id="divMain">
    <h3>Policyholder
         <span style="float: right;">
             <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
         </span>
    </h3>
    <div style="text-align: center;">
        <label for="<%=txtDBA.ClientID%>">Doing Business As</label>
        <asp:TextBox ID="txtDBA" runat="server"></asp:TextBox>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />