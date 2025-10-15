<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_ScheduledPropertyFloater_Item.ascx.vb" Inherits="IFM.VR.Web.cov_ScheduledPropertyFloater_Item" %>

<asp:Repeater ID="spRepeater" runat="server">
    <ItemTemplate>
        <tr>
            <td class="" style="vertical-align: top;">
                <span class="">*Limit</span><br>
                <asp:TextBox ID="txtLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Limit")%>' class="form10Em"></asp:TextBox>
            </td>
            <td style="vertical-align: top; width: 200px;"">
                <span class="">*Description</span><br />
                <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="175px" runat="server"  Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>' class="form13Em"></asp:TextBox>
            </td>
            <td class="brDeleteButton">
                <asp:LinkButton ID="btnDelete" class="btnCIMDelete" runat="server" CommandName="lnkDelete" >Delete</asp:LinkButton><br />
                
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <tr>
            <td style="padding-top: 1em; padding-right: 1em; text-align: right" colspan="4">
                <asp:LinkButton ID="lnkAdd" CssClass="" CommandName="lnkAdd" ToolTip="Add Additional Scheduled Property" runat="server">Add Additional Scheduled Property</asp:LinkButton>
            </td>
        </tr>
    </FooterTemplate>
</asp:Repeater>
