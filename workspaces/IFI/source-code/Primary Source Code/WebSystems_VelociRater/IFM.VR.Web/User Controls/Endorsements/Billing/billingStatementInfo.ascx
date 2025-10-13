<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="billingStatementInfo.ascx.vb" Inherits="IFM.VR.Web.billingStatementInfo" %>

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

        .BillStatementInfo {
            clear: both;
        }

        .BillStatementInfo table.documentTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        .BillStatementInfo .DescriptionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
            width: 100%
        }

            .BillStatementInfo .chkShowAllImgs {
                width: 25px;
            }

            .BillStatementInfo .ShowAllCheckbox {
                text-align: center;
            }


            /*.BillStatementInfo .HeaderText {
                padding-left: 0px !important;
                padding-right: 0px !important;
            }

            .BillStatementInfo .HeaderVer, .BillStatementInfo .TableVer {
                width: 30px !important;
                text-align: center;
            }
                                          
            .BillStatementInfo .HeaderTransDate, .BillStatementInfo .TableTransDate {
                width: 70px !important;
            }

            .BillStatementInfo .HeaderUser span, .BillStatementInfo .TableUser span {
                width: 70px !important;
                white-space:nowrap;
                text-overflow:ellipsis;
                display:block;
                overflow:hidden;
                vertical-align: middle;
             }

            .BillStatementInfo .HeaderDescription, .BillStatementInfo .TableDescription{
                width: 90px !important;
            }

            .BillStatementInfo .HeaderType, .BillStatementInfo .TableType {
                width: 90px !important;
             }

            .BillStatementInfo .HeaderDueDate, .BillStatementInfo .TableDueDate {
                width: 70px !important;
            }
                                          
            .BillStatementInfo .HeaderBilled, .BillStatementInfo .TableBilled {
                width: 70px !important;
                text-align: right;
            }

            .BillStatementInfo .HeaderPaid, .BillStatementInfo .TablePaid {
                width: 70px !important;
                text-align: right;
             }

            .BillStatementInfo .HeaderBalance, .BillStatementInfo .TableBalance {
                width: 70px !important;
                text-align: right;
                padding-right: 2px;
            }*/

            .BillStatementInfo .TableUnitDesc {
                width: 50px;
             }


            .BillStatementInfo .DocPrintSection {
                margin: auto;
                padding: 5px !important;
            }

            .BillStatementInfo td span {
                width: auto;
                margin: 5px;
            }


    </style>
</header>

<div id="BillStatementInfo" class="BillStatementInfo" runat="server">
    <h3>
        <asp:Label ID="lblAccountSum" runat="server" Text="Statement Information"></asp:Label>
    </h3>
    <div style="padding: 0px !important;">
        <div></div>
            <asp:DataGrid ID="DataGrid_StatementInfo" runat="Server" Visible="true" AutoGenerateColumns="False" AllowPaging="true" CellPadding="3" ItemStyle-VerticalAlign="Top" BorderStyle="None" ItemStyle-Font-Size="12px" HeaderStyle-Font-Size="12px" PageSize="6" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" CssClass="" style="margin:5px;" SelectedItemStyle-Wrap="False">
                <HeaderStyle CssClass="ui-widget-header" />
                <AlternatingItemStyle CssClass="GridAlternatingRowStyle" />
                <Columns>
                    <asp:BoundColumn HeaderText="Ver"  DataField="RenewalVer" ItemStyle-BorderColor="White" ItemStyle-Width="30px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Trans Date"  DataField="TransDate" ItemStyle-BorderColor="White" ItemStyle-Width="70px" DataFormatString="{0:d}"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="User"  DataField="User" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="70px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Description"  DataField="Description" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="90px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Type"  DataField="Type" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="90px"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Due Date"  DataField="DueDate" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="70px" DataFormatString="{0:d}"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Billed"  DataField="BilledAmount" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="70px" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Paid"  DataField="PaidAmount" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" DataFormatString="{0:c}" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="Balance"  DataField="Balance" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="70px" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
    </div>
    <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
</div>
