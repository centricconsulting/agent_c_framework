<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDefaultCoverageList.ascx.vb" Inherits="IFM.VR.Web.ctlDefaultCoverageList" %>
<%@ Register Src="~/User Controls/Configuration/PPA/ctlDefaultCoverages.ascx" TagPrefix="uc1" TagName="ctlDefaultCoverages" %>

<asp:DataList ID="DataList1" runat="server" RepeatDirection="Horizontal">
    <ItemTemplate>
        <uc1:ctlDefaultCoverages runat="server" ID="ctlDefaultCoverages" />
    </ItemTemplate>
</asp:DataList>