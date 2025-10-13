<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WCP_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_WCP_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/WCP/ctl_WCP_Location.ascx" TagPrefix="uc1" TagName="ctlWCPLocation" %>

<div>
    <div id="divMainList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlWCPLocation runat="server" id="ctl_WCP_LocationItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />

<%--    <div style="margin-top: 20px; width: 100%; text-align:center;">
        <asp:Button ID="btnAddAnotherLocation" runat="server" Text="Add Another Location" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveLocation" runat="server" Text="Save Location" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" />
    </div>--%>
</div>