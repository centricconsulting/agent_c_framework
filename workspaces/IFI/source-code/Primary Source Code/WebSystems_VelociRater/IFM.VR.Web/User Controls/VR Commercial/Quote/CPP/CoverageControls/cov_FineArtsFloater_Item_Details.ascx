<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_FineArtsFloater_Item_Details.ascx.vb" Inherits="IFM.VR.Web.cov_FineArtsFloater_Item_Details" %>

<asp:Repeater ID="fadRepeater" runat="server">
    <ItemTemplate>
        <tr>
            <td class="" style="vertical-align: top;">
                <span class="">*Limit</span><br>
                <asp:TextBox ID="txtLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Limit")%>' class="form10Em"></asp:TextBox>
            </td>
            <td style="vertical-align: top; width: 200px;"">
                <span class="">*Jobsite Location Description</span><br />
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
                <asp:LinkButton ID="lnkAdd" CssClass="" CommandName="lnkAdd" CommandArgument='<%# LocationIndex.ToString() + "|" + BuildingIndex.ToString() %>' ToolTip="Add Additional Fine Arts" runat="server">Add Additional Fine Arts</asp:LinkButton>
            </td>
        </tr>
        
    </FooterTemplate>
</asp:Repeater>
<tr>
    <td colspan="4"> 
        Total Limit 
        <asp:TextBox ID="txtTotalLimit" runat="server" Enabled="false"></asp:TextBox>
    </td>
</tr>