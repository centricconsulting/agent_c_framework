<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Summary4column.ascx.vb" Inherits="IFM.VR.Web.Summary4column" %>
<div class="qs_Main_Sections" runat="server" id="Section4column">
    <span class="qs_section_headers" id="SectionHeader" runat="server">Header</span>
    <div class="qs_Sub_Sections">
        <table class="qa_table_shades">

            <%-- Header Row --%>
            <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns">
                <td style="width: 35%;">Coverage</td>
                <td class="qs_rightJustify qs_padRight " style="width: 35%;"></td>
                <td class="qs_rightJustify qs_padRight ">Limit</td>
                <td class="qs_rightJustify qs_padRight ">Premium</td>
            </tr>

            <%-- Data Rows --%>
            <asp:Repeater ID="repeater" runat="server">
                <ItemTemplate>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowPLFarmCoverage">
                        <td id="tdName" runat="server" style="width: 35%;"><%# DataBinder.Eval(Container.DataItem, "coverage") %></td>
                        <td id="tdAddress" runat="server" style="width: 35%;"><%# DataBinder.Eval(Container.DataItem, "address") %></td>
                        <td id="tdLimit" runat="server" class="qs_rightJustify qs_padRight"><%# DataBinder.Eval(Container.DataItem, "limit") %></td>
                        <td id="tdPremium" runat="server" class="qs_rightJustify qs_padRight"><%# DataBinder.Eval(Container.DataItem, "premium") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
</div>