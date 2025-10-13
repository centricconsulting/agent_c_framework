<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Summary3column.ascx.vb" Inherits="IFM.VR.Web.Summary3column" %>
<div class="qs_Main_Sections" runat="server" id="Section3column">
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
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowCustomFarmingCoverage">
                        <td id="tdName" runat="server" style="width: 70%;" colspan="2"><%# DataBinder.Eval(Container.DataItem, "coverage") %></td>
                        <td id="tdLimit" runat="server" class="qs_rightJustify qs_padRight"><%# DataBinder.Eval(Container.DataItem, "limit") %></td>
                        <td id="tdPremium" runat="server" class="qs_rightJustify qs_padRight"><%# DataBinder.Eval(Container.DataItem, "premium") %></td>
                    </tr>
                    <tr visible='<%# DataBinder.Eval(Container.DataItem, "showExtraRow") %>' runat="server" id="bufferForExtraRow">
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "showExtraRow") %>' id="rowExtraData">
                        <td id="tdExtra1" runat="server" style="width: 35%;"><%# DataBinder.Eval(Container.DataItem, "extra1") %></td>
                        <td id="tdExtra2" runat="server" style="width: 35%;"><%# DataBinder.Eval(Container.DataItem, "extra2") %></td>
                        <td id="tdExtra3" runat="server" class="qs_rightJustify qs_padRight"><%# DataBinder.Eval(Container.DataItem, "extra3") %></td>
                        <td id="tdExtra4" runat="server" class="qs_rightJustify qs_padRight"><%# DataBinder.Eval(Container.DataItem, "extra4") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
</div>