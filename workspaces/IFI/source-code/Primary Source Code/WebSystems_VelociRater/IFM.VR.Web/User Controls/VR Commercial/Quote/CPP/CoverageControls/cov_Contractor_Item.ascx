<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Contractor_Item.ascx.vb" Inherits="IFM.VR.Web.cov_Contractor_Item" %>

<div class="ItemGroup" runat="server">
    <asp:CheckBox ID="chkScheduledTools" runat="server" Text="Scheduled Tools" class="chkOption2 chkScheduledTools"/>
    <div runat="server" ID="divScheduledToolsDetail" class="chkDetail" style="display: none; padding-top: 5px; padding-left: 20px;">
        <asp:Repeater ID="ceRepeater" runat="server">
            <ItemTemplate>
                <table class="qs_grid_4_columns" runat="server">
                <tr>
                    <td>*Limit</td>
                    <td>Valuation</td>
                    <td>*Description</td>
                    <td></td>
                </tr>
                <tr>
                    <td style="vertical-align:top">
                        <asp:TextBox ID="txtLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ManualLimitAmount")%>' class="form4Em"></asp:TextBox>
                    </td>
                    <td style="vertical-align:top">
                        <asp:DropDownList ID="ceValuation" runat="server" class="form13Em"></asp:DropDownList>
                    </td>
                    <td style="vertical-align:top">
                        <asp:TextBox ID="txtLocation" TextMode="MultiLine" class="form13Em" runat="server"  Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:TextBox>
                    </td>
                    <td style="width: 50px;">
                        <asp:LinkButton ID="LinkButton1" class="btnCIMDelete form13Em" runat="server" CommandName="lnkDelete" style="padding-left: 10px;">Delete</asp:LinkButton>
                    </td>
                
                </tr>
                </table>
            </ItemTemplate>
            <FooterTemplate>
                <table class="qs_grid_4_columns" runat="server">
                <tr>
                    <td style="padding-top: 1em; padding-right: 1em; text-align: right" colspan="4">
                        <asp:LinkButton ID="lnkAdd" CssClass="" CommandName="lnkAdd" ToolTip="Add Additional Scheduled Tools" runat="server">Add Additional Scheduled Tools</asp:LinkButton>
                    </td>
                </tr>
                
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <table class="qs_grid_4_columns" runat="server" style="margin-top: 5px;">
            <tr>
                <td colspan="4"> 
                    Equipment Leased/Rented from Others Limit <asp:TextBox ID="txtRentedEquipment" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="padding-right: 2em; text-align: center;">
                    <span class="informationalText">$25,000 Equipment Leased/Rented from Others is automatically included.  Enter the total limit desired for coverage.</span>
                </td>
            </tr>
        </table> 
    </div>
</div>

