<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_ENDO_PropertyInOpenList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_ENDO_PropertyInOpenList" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/Locations/ctl_CPR_ENDO_PropertyInOpenItem.ascx" TagPrefix="uc1" TagName="ctl_CPR_ENDO_PropertyInOpenItem" %>


<div id="divClassificationsList" runat="server">
    <h3>Property in the Open
        <span style="float: right;">
            <%--<asp:LinkButton ID="lnkAdd" runat="server" ToolTip="Add a Property in the Open item" CssClass="RemovePanelLink">Add New</asp:LinkButton>--%>
            <%--<asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save" CssClass="RemovePanelLink">Save</asp:LinkButton>--%>
        </span>
    </h3>
    <div>
        <asp:Label ID="lblPioMessage" runat="server" CssClass="informationalText" Width="100%" style="text-align:center;" Text="Please contact your underwriter to adjust Property in the Open coverage." ></asp:Label>
        <div runat="server" id="divList">
        
            <%--<span class="informationalText">Please contact your underwriter to adjust Property in the Open coverage.</span>--%>
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctl_CPR_ENDO_PropertyInOpenItem runat="server" id="ctl_CPR_ENDO_PropertyInOpenItem" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnAccordList" runat="server" />
</div>    
