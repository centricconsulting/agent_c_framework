<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CoveragesAdditionalInterestsList_HOM_App.ascx.vb" Inherits="IFM.VR.Web.ctl_CoveragesAdditionalInterestsList_HOM_App" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_CoveragesAdditionalInterests_HOM_App.ascx" TagPrefix="uc1" TagName="ctl_CoveragesAdditionalInterests_HOM_App" %>

<div id="divAIList" runat="server">
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <uc1:ctl_CoveragesAdditionalInterests_HOM_App runat="server" id="ctl_CoveragesAdditionalInterests_HOM_App" />
    </ItemTemplate>
</asp:Repeater>
</div>