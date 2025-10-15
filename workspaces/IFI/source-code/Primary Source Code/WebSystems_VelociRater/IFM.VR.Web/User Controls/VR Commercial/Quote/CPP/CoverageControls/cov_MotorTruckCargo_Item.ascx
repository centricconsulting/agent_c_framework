<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_MotorTruckCargo_Item.ascx.vb" Inherits="IFM.VR.Web.cov_MotorTruckCargo_Item" %>

<asp:Repeater ID="mtRepeater" runat="server">
    <ItemTemplate>
        <tr class="">
            <td class="">
                <span class="">*Vehicle Limit</span><br>
                <asp:TextBox ID="txtLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Limit")%>'></asp:TextBox>
            </td>
            <td class="">
                <span class="">Year</span><br>
                <asp:TextBox ID="txtYear" onkeyup='$(this).val(FormatAsNumberNoCommaFormatting($(this).val()," ,[A-Za-z],$,-"));' MaxLength="4" runat="server" placeholder="YYYY" Text='<%# DataBinder.Eval(Container.DataItem, "Year")%>'></asp:TextBox>
            </td>
            <td>
               <span class=""> Make</span><br>
                <asp:TextBox ID="txtMake" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Make")%>'></asp:TextBox>
            </td>
        </tr>
        <tr class="" style="margin-bottom: 10px">

            <td>
                <span class="">Model</span><br>
                <asp:TextBox ID="txtModel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Model")%>'></asp:TextBox>
            </td>
            <td>
                <span class="">VIN</span><br>
                <asp:TextBox ID="txtVin" MaxLength="17" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "VIN")%>'></asp:TextBox>
            </td>

            <td class="brDeleteButton">
                <asp:LinkButton ID="btnDelete" class="btnCIMDelete" runat="server" CommandName="lnkDelete" >Delete</asp:LinkButton><br />
                
            </td>
        </tr>
        <tr id="Spacer" runat="server" visible="false">
            <td colspan="4">
                <hr style="width:50%" />
            </td>

        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <tr>
            <td style="padding-top: 1em; padding-right: 1em; text-align: right" colspan="4">
                <asp:LinkButton ID="lnkAdd" CssClass="" CommandName="lnkAdd" ToolTip="Add Additional Vehicles" runat="server">Add Additional Vehicles</asp:LinkButton>
            </td>
        </tr>
    </FooterTemplate>
</asp:Repeater>
