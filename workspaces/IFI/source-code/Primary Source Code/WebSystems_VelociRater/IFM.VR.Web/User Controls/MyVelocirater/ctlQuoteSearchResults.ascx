<%@ Control Language="vb" AutoEventWireup="true" CodeBehind="ctlQuoteSearchResults.ascx.vb" Inherits="IFM.VR.Web.ctlQuoteSearchResults" %>

<script type="text/javascript">

    $(document).ready(function () {
        //   ShowHideSearchResultsDiv("<%=divSubResults.ClientID%>", "<%=hdnSearchResults.ClientID%>");
        //ShowHideSearchResultsDiv('<%=divSubResults.ClientID%>', '<%=hdnSearchResults.ClientID%>');
    });
</script>

<style>
    .ui-menu {
        position: absolute;
        width: 190px;
    }
</style>

<div style="width: 100%; margin-top: 10px;">
    <div style="float: right; margin-left: 15px;">
        <span title="Expand/Collapse <%=lblLobName.Text%>" onclick='ToggleSearchResultsViewResults("<%=divSubResults.ClientID%>","<%=hdnSearchResults.ClientID%>");' class="ExpandCollapseTriangle ui-accordion-header-icon ui-icon ui-icon-triangle-1-s"></span>
    </div>

    <%=GetSplitButtonHtml()%>

    <div title="Expand/Collapse <%=lblLobName.Text%>" style="cursor: pointer;"
        onclick='ToggleSearchResultsViewResults("<%=divSubResults.ClientID%>","<%=hdnSearchResults.ClientID%>");'>
        <%--<asp:Image ID="imgLobType" Width="25" Height="26" runat="server" ImageUrl="" />--%>
        <span id="spnImage" class="" style="font-size: 28px;display:inline-block;width:40px;text-align:center;" runat="server"></span>
        <asp:Label ID="lblLobName" runat="server" Text="Auto Quotes"></asp:Label>
        <asp:Label ID="lblLobNameAdditional" style="color:red;" runat="server" Text="" ></asp:Label>
    </div>
    <hr />

    <div id="divSubResults" runat="server" style="margin-left: 15px;">

        <asp:GridView CssClass="clientList" ID="gridQuoteResults" AutoGenerateColumns="False" runat="server" BackColor="White" BorderColor="White" BorderStyle="None" BorderWidth="0px" CellPadding="4" GridLines="None" AllowPaging="True" AllowSorting="True">
            <Columns>
                <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="90px">
                    <ItemTemplate>
                        <%--<asp:DropDownList ID="ddActionList" Width="90px" runat="server" onchange='if($(this).children("option").filter(":selected").text() == "Archive"){if(confirm("Archive this quote?")){DisableFormOnSaveRemoves();Post(this,'');}else{$(this).removeAttr("selected").find("option:first").attr("selected", "selected");} return true;}else {DisableFormOnSaveRemoves();Post(this,'');}'></asp:DropDownList>                        --%>
                        <asp:DropDownList ID="ddActionList" Width="90px" runat="server"></asp:DropDownList>
                    </ItemTemplate>

                    <HeaderStyle Width="90px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="QuoteNumAndDescription" SortExpression="QuoteNumAndDescription" HeaderStyle-Width="90px" HeaderText="Quote #">
                    <HeaderStyle Width="90px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="ClientName" SortExpression="ClientName" HeaderText="Customer" />
                <asp:TemplateField HeaderText="Policy Term"><ItemTemplate></ItemTemplate></asp:TemplateField>
                <asp:BoundField DataField="Premium" SortExpression="Premium" DataFormatString="{0:c2}" HeaderStyle-Width="90px" HeaderText="Premium">
                    <HeaderStyle Width="90px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="FriendlyStatus" SortExpression="FriendlyStatus" HeaderStyle-Width="90px" HeaderText="Status">
                    <HeaderStyle Width="90px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="LastModified" SortExpression="LastModified" HeaderText="Last Modified" />
                <asp:BoundField DataField="LobName" SortExpression="Lob" HeaderText="LOB" />
                <asp:BoundField DataField="AgencyCode" SortExpression="AgencyCode" HeaderText="Agency Code" />
                <asp:BoundField DataField="QuoteId" HeaderText="Quote Id" />
                <asp:BoundField DataField="ItemType" HeaderText="ItemType" />
            </Columns>
            <HeaderStyle CssClass="ui-widget-header" />
            <PagerSettings PageButtonCount="5" Mode="NumericFirstLast" />
            <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#4B4B4B" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#242121" />
        </asp:GridView>

        <asp:Label ID="lblNoResults" runat="server" Text="No quotes found. Try changing your search criteria and search again." Visible="false"></asp:Label>
    </div>
    <asp:HiddenField ID="hdnSearchResults" Value="0" runat="server" />
</div>