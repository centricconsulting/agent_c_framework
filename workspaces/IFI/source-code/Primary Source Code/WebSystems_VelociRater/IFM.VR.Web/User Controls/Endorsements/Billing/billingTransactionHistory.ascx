<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="billingTransactionHistory.ascx.vb" Inherits="IFM.VR.Web.billingTransactionHistory" %>

<!-- #region javascript -->

<script type="text/javascript">

    // Initial Page load.
    $(document).ready(function () {
        // Alternate color backgrounds for table items.
        // $(".documentTable .Docs:odd").css("background-color", "#dddddd");
    });

    // Live functionality
    $(function () {

    });
       
</script>

<!-- #endregion -->

<header>
    <style type="text/css">

        .GridAlternatingRowStyle {
            background-color: #dddddd;
        }

        .BillTransactionHistory {
            clear: both;
        }

        .BillTransactionHistory, table.documentTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        .BillTransactionHistory .DescriptionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
            width: 100%
        }

            .BillTransactionHistory .HeaderText {
                padding-left: 0px !important;
                padding-right: 0px !important;
            }

            .BillTransactionHistory .DocPrintSection {
                margin: 5px 5px;
            }

            .BillTransactionHistory td {
                padding: 4px 4px;
            }

    </style>
</header>

<div id="BillTransactionHistory" class="BillTransactionHistory" runat="server">
    <h3>
        <asp:Label ID="lblAccountSum" runat="server" Text="Billing Transaction History"></asp:Label>
    </h3>
        <div style="padding: 0px !important;">
        <div></div>
            <asp:DataGrid ID="DataGrid_HistoryInfo" runat="Server" Visible="true" AutoGenerateColumns="False" AllowPaging="true" CellPadding="3" ItemStyle-VerticalAlign="Top" BorderStyle="None" ItemStyle-Font-Size="12px" HeaderStyle-Font-Size="12px" PageSize="6" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" CssClass="" style="margin:5px;" SelectedItemStyle-Wrap="False">
                <HeaderStyle CssClass="ui-widget-header" />
                <AlternatingItemStyle CssClass="GridAlternatingRowStyle" />
                <Columns>
                    <asp:BoundColumn HeaderText="Trans Date"  DataField="TranDate" ItemStyle-BorderColor="White" ItemStyle-Width="75px" DataFormatString="{0:d}"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Eff Date"  DataField="EffDate" ItemStyle-BorderColor="White" ItemStyle-Width="75px" DataFormatString="{0:d}"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="User"  DataField="User" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="80px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Transaction Type"  DataField="BillTransType" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="100px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Description"  DataField="Dscr" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="200px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Status"  DataField="Status" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="100px"></asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
    </div>

    <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
</div>
