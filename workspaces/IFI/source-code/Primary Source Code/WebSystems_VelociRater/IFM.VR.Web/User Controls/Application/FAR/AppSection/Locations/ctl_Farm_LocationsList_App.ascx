<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_LocationsList_App.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_LocationsList_App" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_Location_App.ascx" TagPrefix="uc1" TagName="ctl_Farm_Location_App" %>

<div runat="server" id="mainDiv">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_Farm_Location_App runat="server" ID="ctl_Farm_Location_App" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:HiddenField ID="hddnAccord" runat="server" />