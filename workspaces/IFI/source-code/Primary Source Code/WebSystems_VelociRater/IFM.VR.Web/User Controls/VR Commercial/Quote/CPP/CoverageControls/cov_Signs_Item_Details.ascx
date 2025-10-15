<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Signs_Item_Details.ascx.vb" Inherits="IFM.VR.Web.cov_Signs_Item_Details" %>

<asp:Repeater ID="sidRepeater" runat="server">
    <ItemTemplate>
        <tr>
            <td class="" style="vertical-align: top;">
                <span class="">*Limit</span><br>
                <asp:TextBox ID="txtLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Limit")%>' class="form10Em"></asp:TextBox>
            </td>
             <td>
                <asp:CheckBox ID="chkInsideSign" runat="server" class="chkOption" Text="Inside Sign" checked='<%# DataBinder.Eval(Container.DataItem, "IsIndoor")%>'/>
            </td>
            <td style="vertical-align: top; width: 200px;"">
                <span class="">*Description</span><br />
                <asp:TextBox ID="txtLocation" TextMode="MultiLine" Width="175px" runat="server"  Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>' class="form13Em"></asp:TextBox>
            </td>
            <td class="brDeleteButton">
                <asp:LinkButton ID="btnDelete" class="btnCIMDelete" runat="server" CommandName="lnkDelete" CommandArgument='<%# LocationIndex.ToString()  + "|" + BuildingIndex.ToString() %>'>Delete</asp:LinkButton><br />
                
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <tr>
            <td style="padding-top: 1em; padding-right: 1em; text-align: right" colspan="4">
                <asp:LinkButton ID="lnkAdd" CssClass="" CommandName="lnkAdd" CommandArgument='<%# LocationIndex.ToString() + "|" + BuildingIndex.ToString() %>' ToolTip="Add Additional Signs" runat="server">Add Additional Signs</asp:LinkButton>
            </td>
        </tr>
    </FooterTemplate>
</asp:Repeater>
