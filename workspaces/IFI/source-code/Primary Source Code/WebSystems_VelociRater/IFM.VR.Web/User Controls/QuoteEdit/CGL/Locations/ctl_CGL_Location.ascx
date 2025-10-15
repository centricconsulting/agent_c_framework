<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_Location.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_Location" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctlProperty_Address" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/ClassCode/ctl_CGL_Classcode.ascx" TagPrefix="uc1" TagName="ctl_CGL_Classcode" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/ClassCode/ctl_CGL_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CGL_ClassCodeList" %>




<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Location"></asp:Label>

    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Location" CssClass="RemovePanelLink">Add Location</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Delete this Building" runat="server">Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div runat="server" id="divContents">
    <uc1:ctlProperty_Address runat="server" ID="ctlProperty_Address" />     
    <uc1:ctl_CGL_ClassCodeList runat="server" ID="ctl_CGL_ClassCodeList" />
</div>



