<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_App_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_App_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_BOP_App_Location.ascx" TagPrefix="uc1" TagName="ctl_BOP_App_Location" %>

<div id="divMainList" runat="server">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_BOP_App_Location runat="server" id="ctl_BOP_App_Location" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>

