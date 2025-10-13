<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Producer.ascx.vb" Inherits="IFM.VR.Web.ctl_Producer" %>

<div id="divProducer" runat="server">
    <h3><span style="float: right;">
        <asp:LinkButton ID="lnkClearBase" Style="display: none;" runat="server" CssClass="RemovePanelLink" ToolTip="Clear Agent Producer">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
    </span>
        Producer Assignment</h3>
    <div style="text-align: center;">
        <label for="<%=ddProducer.ClientID%>">Producer</label>
        <asp:DropDownList Width="300" ID="ddProducer" runat="server"></asp:DropDownList>
    </div>
</div>
<asp:HiddenField ID="accordActive" runat="server" />