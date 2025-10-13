<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Comm_ClassificationsList.ascx.vb" Inherits="IFM.VR.Web.ctl_Comm_ClassificationsList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_Classifications.ascx" TagPrefix="uc1" TagName="ctlClassItem"  %>

<div id="divClassificationsList" runat="server">
    <h3>Building Classifications
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Building Classification" CssClass="RemovePanelLink">Add a Building Classification</asp:LinkButton>
        </span>
    </h3>
    <div runat="server" id="divList">
        The "Primary Classification" checkbox must be checked on one classification.
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlClassItem runat="server" ID="ctl_BuildingClassificationItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnAccordList" runat="server" />
</div>    

