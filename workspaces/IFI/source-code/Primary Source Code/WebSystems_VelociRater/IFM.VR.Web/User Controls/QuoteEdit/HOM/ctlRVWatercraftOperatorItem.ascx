<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRVWatercraftOperatorItem.ascx.vb" Inherits="IFM.VR.Web.ctlRVWatercraftOperatorItem" %>

<h3><a href="#"></a>Operators</h3>
<div>
    <asp:Panel ID="pnlOperators" runat="server">
        <table>
            <tr>
                <td class="LabelColumn">
                    <asp:Label ID="Label1" runat="server" Text="Available Operators"></asp:Label>
                </td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlAvailableOperators" runat="server" TabIndex="1" Width="200px"></asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btnAssign" runat="server" Text="Assign" Width="75px" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <br />
                    <asp:GridView ID="grdAssignedOperators" runat="server" AutoGenerateColumns="false"
                        BorderStyle="Solid" BorderWidth="1px" DataKeyNames="OperatorNum" OnRowCommand="GridCommand"
                        TabIndex="2">
                        <Columns>
                            <asp:ButtonField ButtonType="Link" CommandName="REMOVE" Text="Remove" ItemStyle-Width="75px" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Name.DisplayName" HeaderText="Assigned Operators" ItemStyle-Width="300px" />
                            <asp:BoundField DataField="OperatorNum" HeaderText="OpNum" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="RelationshipTypeId" HeaderText="RelId" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <div align="center">
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
        </div>
    </asp:Panel>
</div>