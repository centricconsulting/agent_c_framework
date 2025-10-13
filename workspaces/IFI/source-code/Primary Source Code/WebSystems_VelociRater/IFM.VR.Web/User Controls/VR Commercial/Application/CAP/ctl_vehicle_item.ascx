<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_vehicle_item.ascx.vb" Inherits="IFM.VR.Web.ctl_vehicle_item" %>


<asp:Repeater ID="rptVehicles" runat="server" OnItemDataBound="R1_ItemDataBound">
    <ItemTemplate>
    <table class="cap_qa_table_shades" style="margin-top: 10px;">
        <tbody>
            <tr class="qs_cap_section_grid_headers ui-widget-header">
                <td colspan="4">
                    <table class="cap_qa_table_shades">
                        <tr class="qs_cap_grid_3_columns">
                            <td>Vehicle <%# Container.ItemIndex + 1 %> - <%# DataBinder.Eval(Container.DataItem, "Year")%> <%# DataBinder.Eval(Container.DataItem, "Make")%> <%# DataBinder.Eval(Container.DataItem, "Model")%></td>
                            <td>Class: <%# DataBinder.Eval(Container.DataItem, "ClassCode")%></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "PremiumFullTerm")%></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr class="qs_cap_grid_4_columns">
                <td>Cost New:</td>
                <td><%# DataBinder.Eval(Container.DataItem, "CostNew")%></td>
                <td></td>
                <td></td>
            </tr>

            <asp:Literal ID="tblVehicleInfo" runat="server"></asp:Literal>

           <%-- <tr class="qs_cap_grid_4_columns">
                <td>Comprehensive</td><td>Premium</td>
                <td>Rental</td><td>Premium</td>
            </tr>
            <tr class="qs_cap_grid_4_columns qs_cap_indent">
                <td><%# DataBinder.Eval(Container.DataItem, "ComprehensiveDeductible")%></td><td><%# DataBinder.Eval(Container.DataItem, "ComprehensiveQuotedPremium")%></td>
                <td><%# DataBinder.Eval(Container.DataItem, "RentalReimbursementNumberOfDays")%>/<%# DataBinder.Eval(Container.DataItem, "RentalReimbursementDailyReimbursement")%></td><td><%# DataBinder.Eval(Container.DataItem, "RentalReimbursementQuotedPremium")%></td>
            </tr>
            <tr class="qs_cap_grid_4_columns">
                <td>Collision</td><td>Premium</td>
                <td>Towing and Labor</td><td>Premium</td>
            </tr>
            <tr class="qs_cap_grid_4_columns qs_cap_indent">
                <td><%# DataBinder.Eval(Container.DataItem, "CollisionDeductible")%></td><td><%# DataBinder.Eval(Container.DataItem, "CollisionQuotedPremium")%></td>
                <td></td><td><%# DataBinder.Eval(Container.DataItem, "TowingAndLaborQuotedPremium")%></td>
            </tr>
            <tr class="qs_cap_grid_4_columns">
                <td colspan="4"><%# GetGaragingAddressString(DataBinder.Eval(Container.DataItem, "GaragingAddress"))%></td>
            </tr>--%>
        </tbody>
    </table>
    </ItemTemplate>
</asp:Repeater>