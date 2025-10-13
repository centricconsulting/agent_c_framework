<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PolicyDiscounts.ascx.vb" Inherits="IFM.VR.Web.PolicyDiscounts" %>
<div class="qs_Main_Sections" runat="server" id="divPolicyDiscounts" visible="false">
    <span class="qs_section_headers">Policy Discounts</span>
    <div class="qs_Sub_Sections">

        <asp:ListView ID="lvPolicyDiscounts" runat="server">
            <LayoutTemplate>
                <table class="qa_table_shades" runat="server">
                    <%-- Header Row --%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns" runat="server">
                        <th>Coverage</th>
                        <th></th>
                        <th class="qs_rightJustify qs_padRight ">Premium</th>
                    </tr>
                    <%-- Data Rows --%>
                    <tr runat="server" id="ItemPlaceHolder">
                    </tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                    <td><%# Eval("Description") %></td>
                    <td class="qs_rightJustify"></td>
                    <td class="qs_rightJustify qs_padRight"><%# Eval("Premium", "{0:$#,##0.00}") %></td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </div>
</div>
