<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_LocationList" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/Locations/ctl_CGL_Location.ascx" TagPrefix="uc1" TagName="ctl_CGL_Location" %>

<div id="divNewLocation" runat="server">
<uc1:ctl_CGL_Location runat="server" HideFromParent="True" MyLocationIndex="-1" id="ctl_CGL_Location" />
    </div>

<div id="divLocationList" runat="server">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_CGL_Location runat="server" id="ctl_CGL_Location" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:HiddenField ID="hddnAccord" runat="server" />
