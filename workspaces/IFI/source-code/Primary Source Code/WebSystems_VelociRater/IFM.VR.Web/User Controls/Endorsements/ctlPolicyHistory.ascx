<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPolicyHistory.ascx.vb" Inherits="IFM.VR.Web.ctlPolicyHistory" %>

<script type="text/javascript">

    // Initial Page load.
    $(document).ready(function () {
        // Alternate color backgrounds for table items.
        $(".HistoryTable:odd").css("background-color", "#dddddd");

        $(".HistorySelectionDiv").find(".ViewSelected").prop("title", "No documents were selected to print");
        $(".HistorySelectionDiv").find(".ViewSelected").prop("disabled", true);
    });
       
</script>

<header>
    <style type="text/css">

        .HistorySelectionDiv {
            clear: both;
        }

        .HistorySelectionDiv, table.HistoryTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        table.HistoryTable{
            table-layout:fixed;
            width:100%;
            /*vertical-align:middle;*/ /*not needed*/
        }

        table.HistoryTable td{
            padding:2px;
        }

            .HistorySelectionDiv .HeaderText {
                padding-left: 0px !important;
                padding-right: 0px !important;
            }

            .HistorySelectionDiv .View{
                width: 36px;
            }

            .HistorySelectionDiv .Ver{
                width: 30px;
            }

            .HistorySelectionDiv .VerImgNum{
                width: 56px;
            }
                                          
            .HistorySelectionDiv .User {
                width: 50px;
                white-space:nowrap;
                text-overflow:ellipsis;
                /*display:block;*/
                display:table-cell;
                overflow:hidden;
            }
                                          
            .HistorySelectionDiv .Type {
                width: 100px;
            }
                                          
            .HistorySelectionDiv .Reason {
                width: 75px;
                white-space:nowrap;
                text-overflow:ellipsis;
                display:block;
                overflow:hidden;
            }
                                          
            .HistorySelectionDiv .TypeReason {
                width: 110px;
                white-space:nowrap;
                text-overflow:ellipsis;
                display:table-cell;
                overflow:hidden;
            }
                                          
            .HistorySelectionDiv .ImageNumber {
                width: 75px;
            }
                                          
            .HistorySelectionDiv .Remark {
                width: 115px;
                white-space:nowrap;
                text-overflow:ellipsis;
                display:table-cell;
                overflow:hidden;
            }
                                          
            .HistorySelectionDiv .TEff {
                width: 66px;
            }
                                          
            .HistorySelectionDiv .TExp {
                width: 66px;
            }
                                          
            .HistorySelectionDiv .WPrem {
                width: 66px;
            }
         
    </style>
</header>

<div id="HistorySelectionDiv" class="HistorySelectionDiv">
    <h3>
        <asp:Label ID="lblPolicyHistoryHeader" runat="server" Text="Policy History"></asp:Label>
        <%--<span style="float: right;">
            <asp:LinkButton ID="lnkViewSelection" ToolTip="No documents were selected to print" runat="server" CssClass="RemovePanelLink ViewSelected">View Selected</asp:LinkButton>
        </span>--%>
    </h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td valign="top">
                <%--Repeat for each item--%>
                <section class="HistorySelectionSection">
                    <h3 class="HeaderText">
                        <table id="tblDocPrint" runat="server" class="HistoryTable">
                            <tr>
                                <td class="View"></td>
                                <td class="VerImgNum">Ver-Img#</td>
                                <td class="User">User</td>
                                <td class="TypeReason">Type-Reason</td>
                                <td class="Remark">Remark</td>
                                <td class="TEff">T-Eff</td>
                                <td class="TExp">T-Exp</td>
                                <td class="WPrem">WPrem</td>
                            </tr>
                        </table>
                    </h3>
                    <div style="border-collapse: collapse; width: 100%; display: table;">
                        <asp:Repeater ID="rptPolicyHistory" runat="server">
                            
                            <ItemTemplate>
                                <table id="tblDocPrint" runat="server" class='HistoryTable'>
                                    <tr>
                                        <td class="View"><asp:Label runat="server" ID="lblLink"></asp:Label></td>
                                        <td class="VerImgNum"><asp:Label runat="server" ID="lblVerImgNum"></asp:Label></td>
                                        <td class="User"><asp:Label runat="server" ID="lblUser"></asp:Label></td>
                                        <td class="TypeReason"><asp:Label runat="server" ID="lblTypeReason"></asp:Label></td>
                                        <td class="Remark"><asp:Label runat="server" ID="lblRemark"></asp:Label></td>
                                        <td class="TEff"><asp:Label runat="server" ID="lblTEff"></asp:Label></td>
                                        <td class="TExp"><asp:Label runat="server" ID="lblTExp"></asp:Label></td>
                                        <td class="WPrem"><asp:Label runat="server" ID="lblWPrem"></asp:Label></td>
                                    </tr>                                    
                                </table>
                            </ItemTemplate>
                            
                        </asp:Repeater>
                    </div>
                </section>
                <br />
                <br />
                <%--<div align="center">
                    <asp:Button ID="btnPolicyHistory" runat="server" Text="Policy History" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500"/>
                </div>--%>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</div>
