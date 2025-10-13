<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_PropertyInOpenList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_PropertyInOpenList" %>
<%@ Register src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_PropertyInOpenItem.ascx" TagPrefix="uc1" TagName="ctl_PropInOpenItem"  %>

<div id="divClassificationsList" runat="server">
    <h3>Property in the Open
        <span style="float: right;">
            <asp:LinkButton ID="lnkAdd" runat="server" ToolTip="Add a Property in the Open item" CssClass="RemovePanelLink">Add New</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div runat="server" id="divList">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctl_PropInOpenItem runat="server" ID="ctl_CPR_PropInOpenItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnAccordList" runat="server" />
</div>    
