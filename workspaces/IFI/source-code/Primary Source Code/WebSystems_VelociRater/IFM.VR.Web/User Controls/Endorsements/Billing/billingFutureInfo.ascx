<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="billingFutureInfo.ascx.vb" Inherits="IFM.VR.Web.billingFutureInfo" %>

<!-- #region javascript -->

<script type="text/javascript">

    // Initial Page load.
    $(document).ready(function () {
        // Alternate color backgrounds for table items.
        // $(".documentTable .Docs:odd").css("background-color", "#dddddd");
        if ($("input[id*= 'chkCombine']").prop('checked')) {
            $("table.FutureInfo").hide();
            $("table.FutureCombined").show();
        }
        else {
            $("table.FutureInfo").show();
            $("table.FutureCombined").hide();
        }
    });

    // Live functionality
    $(function () {
        $("input[id*= 'chkCombine']").on('change', function () {
            if ($(this).prop('checked')) {
                $("table.FutureInfo").hide();
                $("table.FutureCombined").show();
            }
            else {
                $("table.FutureInfo").show();
                $("table.FutureCombined").hide();
            }
        });
    });
       
</script>

<!-- #endregion -->

<header>
    <style type="text/css">

        .GridAlternatingRowStyle {
            background-color: #dddddd;
        }

        .BillFutureInfo {
            clear: both;
        }

        .BillFutureInfo, table.documentTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        .BillFutureInfo .DescriptionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
            width: 100%
        }


            .BillFutureInfo .NormalBorder {
                border: none;
            }

            .BillFutureInfo .DocPrintSection {
                margin: 5px 5px;
            }

            .BillFutureInfo td {
                /*padding: 4px 4px;*/
            }

            .BillFutureInfo .futureContents {
                padding: 4px 4px !important;
                
            }

            .BillFutureInfo .futureContainer {
                padding: 0px 8px;
                width: 580px;
            }

            .BillFutureInfo .futurePremiumItem {
                width: 180px;
                display: inline-block;
            }

            .BillFutureInfo .futureMiscItem {
                width: 180px;
                display: inline-block;
            }

            .BillFutureInfo .futureFeesItem {
                width: 195px;
                display: inline-block;
            }

    </style>
</header>

<div id="BillFutureInfo" class="BillFutureInfo" runat="server">
    <h3>
        <asp:Label ID="lblAccountSum" runat="server" Text="Future Information"></asp:Label>
    </h3>
    <div style="padding: 0px !important;">
        <div class="futureContainer">
            <div class="futurePremiumItem"><asp:Label ID="Label1" runat="server">Future Premium: </asp:Label><asp:Label ID="txtFuturePremium" runat="server"></asp:Label></div>
            <div class="futureMiscItem"><asp:Label ID="Label3" runat="server">Future Misc Charges: </asp:Label><asp:Label ID="txtFutureMiscCharges" runat="server"></asp:Label></div>
            <div class="futureFeesItem"><asp:CheckBox AutoPostBack="false" ID="chkCombine" Text=" Combine Installments and Fees" ToolTip="Combine Installments and Fees" runat="server" /></div>
        </div>
        <asp:DataGrid ID="DataGrid_FutureInfo" runat="Server" Visible="true" AutoGenerateColumns="False" AllowPaging="true" CellPadding="3" ItemStyle-VerticalAlign="Top" BorderStyle="None" ItemStyle-Font-Size="12px" HeaderStyle-Font-Size="12px" PageSize="6" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" CssClass="FutureInfo" style="margin:5px;" SelectedItemStyle-Wrap="False">
            <HeaderStyle CssClass="ui-widget-header" />
            <AlternatingItemStyle CssClass="GridAlternatingRowStyle" />
            <Columns>
                <asp:BoundColumn HeaderText="Ver"  DataField="RenewalVer" ItemStyle-BorderColor="White" ItemStyle-Width="50px"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Trans Date"  DataField="TranDate" ItemStyle-BorderColor="White" ItemStyle-Width="100px" DataFormatString="{0:d}"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Due Date"  DataField="DueDate" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="100px" DataFormatString="{0:d}"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Amount"  DataField="Amount" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="100px" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Description"  DataField="Description" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="245px"></asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
        <asp:DataGrid ID="DataGrid_FutureCombined" runat="Server" Visible="true" AutoGenerateColumns="False" AllowPaging="true" CellPadding="3" ItemStyle-VerticalAlign="Top" BorderStyle="None" ItemStyle-Font-Size="12px" HeaderStyle-Font-Size="12px" PageSize="6" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" CssClass="FutureCombined" style="margin:5px;" SelectedItemStyle-Wrap="False">
            <HeaderStyle CssClass="ui-widget-header" />
            <AlternatingItemStyle CssClass="GridAlternatingRowStyle" />
            <Columns>
                <asp:BoundColumn HeaderText="Ver"  DataField="RenewalVer" ItemStyle-BorderColor="White" ItemStyle-Width="50px"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Trans Date"  DataField="TranDate" ItemStyle-BorderColor="White" ItemStyle-Width="100px" DataFormatString="{0:d}"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Due Date"  DataField="DueDate" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="100px" DataFormatString="{0:d}"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Amount"  DataField="Amount" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="100px" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Description"  DataField="Description" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="245px"></asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
    </div>

        <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
</div>
