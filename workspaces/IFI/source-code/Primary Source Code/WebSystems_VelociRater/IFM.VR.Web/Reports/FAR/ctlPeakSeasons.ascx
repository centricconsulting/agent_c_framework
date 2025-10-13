<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPeakSeasons.ascx.vb" Inherits="IFM.VR.Web.ctlPeakSeasons" %>

<table id="tblPeakseason" runat="server" style="width: 100%" class="table">
    <tr style="vertical-align: bottom">
        <td style="width: 5%"></td>
        <td>
            <asp:DataGrid ID="dgPeakSeason" runat="server" HorizontalAlign="Left" AutoGenerateColumns="false" GridLines="None" ShowHeader="false" ShowFooter="false" Width="100%">
                <ItemStyle CssClass="GridItem"></ItemStyle>
                <Columns>
                    <asp:BoundColumn DataField="Description" SortExpression="Description" ItemStyle-Width="372px" ItemStyle-HorizontalAlign="Left"></asp:BoundColumn>
                    <asp:BoundColumn DataField="StartDate" SortExpression="StartDate" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                    <asp:BoundColumn DataField="EndDate" SortExpression="EndDate" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Limit" SortExpression="Limit" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Premium" SortExpression="Premium" ItemStyle-Width="223px" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
        </td>
    </tr>
</table>