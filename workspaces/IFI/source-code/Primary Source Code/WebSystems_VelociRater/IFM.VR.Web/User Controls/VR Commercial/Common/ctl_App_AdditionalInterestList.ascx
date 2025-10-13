<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_AdditionalInterestList.ascx.vb" Inherits="IFM.VR.Web.ctl_App_AdditionalInterestList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalInterest.ascx" TagPrefix="uc1" TagName="ctl_BOP_AddlInterest" %>

    <div id="divMainList" runat="server">
        <h3>
            <asp:Label ID="lblAccordHeader" runat="server" Text="Additional Interests (0) (Property) "></asp:Label>
                <span style="float: right;">
<%--                <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add an Additional Interest">Add Additional Interest</asp:LinkButton>
                <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>--%>
                <asp:LinkButton ID="lnkNew" runat="server" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add an Additional Interest">Add Additional Interest</asp:LinkButton>
                <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <div id="divList" runat="server">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <uc1:ctl_BOP_AddlInterest runat="server" id="ctl_BOP_Additional_Interest" />
                    </ItemTemplate>
                    </asp:Repeater>
            </div>            
        <asp:HiddenField ID="hdnAccordList" runat="server" />
        </div>        
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
