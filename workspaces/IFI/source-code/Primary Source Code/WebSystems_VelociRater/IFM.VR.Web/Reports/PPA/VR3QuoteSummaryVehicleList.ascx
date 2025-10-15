<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VR3QuoteSummaryVehicleList.ascx.vb" Inherits="IFM.VR.Web.VR3QuoteSummaryVehicleList" %>
<%@ Register Src="~/Reports/PPA/VR3QuoteSummaryVehicle.ascx" TagPrefix="uc1" TagName="VR3QuoteSummaryVehicle" %>

<%--<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <td style="width:20%">
            <uc1:VR3QuoteSummaryVehicle runat="server" ID="VR3QuoteSummaryVehicle" />
        </td>
    </ItemTemplate>
</asp:Repeater>--%>
<asp:DataList ID="DataList1" runat="server" RepeatColumns="3" RepeatDirection="Horizontal">
    <ItemTemplate>
        <uc1:VR3QuoteSummaryVehicle runat="server" ID="VR3QuoteSummaryVehicle" />
    </ItemTemplate>
</asp:DataList>