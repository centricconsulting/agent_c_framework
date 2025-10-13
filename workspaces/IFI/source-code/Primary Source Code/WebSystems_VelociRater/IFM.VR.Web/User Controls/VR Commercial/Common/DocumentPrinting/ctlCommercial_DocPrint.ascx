<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCommercial_DocPrint.ascx.vb" Inherits="IFM.VR.Web.ctlCommercial_DocPrint" %>


<!-- #region javascript -->

<script type="text/javascript">

    // Initial Page load.
    $(function () {
        // Alternate color backgrounds for table items.
        $(".documentTable:odd").css("background-color", "#dddddd");

        $('.chkbox').each(function () {
            if ($(this).find("input").prop('checked')) {
                // atleast one is selected - Normal formatting, no asterisk next to radio buttons
                //Change Text Enable
                $(".DocPrintDiv").find(".printBtn").val('Print Selected Documents');
                $(".DocPrintDiv").find(".printBtn").prop("disabled", false);
                return false
            }
        });
    });

    // Live functionality
    $(function () {
        // find printBtn
        $('.chkbox').on('change', function () {

            $(".DocPrintDiv").find(".printBtn").val('No documents were selected to print');
            $(".DocPrintDiv").find(".printBtn").prop("disabled", true);

            $('.chkbox').each(function () {
                if ($(this).find("input").prop('checked')) {
                    // atleast one is selected - Normal formatting, no asterisk next to radio buttons
                    //Change Text Enable
                    $(".DocPrintDiv").find(".printBtn").val('Print Selected Documents');
                    $(".DocPrintDiv").find(".printBtn").prop("disabled", false);
                    return false
                }
            });

        });

        // Open Scheduled Contractor Equipment in new tab if checked.  Found by Form Number equalling "N/A"
        $('.printBtn').on('click', function () {
            $('.Docs').each(function () {
                if ($(this).find('.FormNum')[0].innerHTML == "N/A" && $($(this).find('.chkbox > input')[0]).is(':checked')) {
                    var win = window.open('PF_ContractorEquipment.aspx?quoteid=' + master_quoteID, '_blank');
                    if (win) {
                        //Browser has allowed it to be opened
                        win.opener.focus();
                    } else {
                        //Browser has blocked it
                        alert('Please allow popups for this website');
                    }
                }
            });



        });
        
    });
       
</script>

<!-- #endregion -->

<header>
    <style type="text/css">

        .DocPrintDiv {
            clear: both;
        }

        .DocPrintDiv, table.documentTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        .DocPrintDiv .DescriptionTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
            width: 100%
        }

            .DocPrintDiv .TableCheckBox {
                width: 25px;
            }

            .DocPrintDiv .TableCategory {
                width: 450px;
                padding-left: 4px;
            }
                                          
            .DocPrintDiv .TableFormNumber {
                width: 105px;
            }

            .DocPrintDiv .TableFormVer {
                width: 89px;
             }

            .DocPrintDiv .HeaderCategory {
                width: 450px;
                padding-left: 4px;
            }
                                          
            .DocPrintDiv .HeaderFormNumber {
                width: 100px;
            }

            .DocPrintDiv .HeaderFormVer {
                width: 85px;
             }

            .DocPrintDiv .NormalBorder {
                border: none;
            }

            .DocPrintDiv .DocPrintSection {
                margin: auto 30px;
            }

            .DocPrintDiv td {
                /*padding: 4px 4px;*/
            }

            .DocPrintDiv .Note {
                margin: auto 60px;
                text-align: center;
                width: 400px;
            }

            .DocPrintDiv .lblAnswerAll {
                clear: both;
                margin: 10px 0 10px 30px;
            }
    </style>
</header>

<div id="DocPrintDiv" class="DocPrintDiv">
    <h3>
        <asp:Label ID="Label1" runat="server" Text="Document Printing"></asp:Label>
    </h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td valign="top">
                <div class="lblAnswerAll">
                    <asp:Label ID="lblAnswerAll" runat="server" Text="Select Documents to Print"></asp:Label>
                </div>
                <%--Repeat for each item--%>
                <section class="DocPrintSection">
                    <h3>
                        <table id="tblDocPrint" runat="server" class="documentTable">
                            <tr>
                                <td class="HeaderCategory">
                                    <asp:Label ID="lblAccordHeader" runat="server" Text="Form Description"></asp:Label>
                                </td>
                                <td class="HeaderFormNumber">
                                    <asp:Label ID="lblAccordHeader2" runat="server">Form #</asp:Label>
                                </td>
                                <%--<td class="HeaderFormVer">
                                    <asp:Label ID="Label2" runat="server">Version</asp:Label>
                                </td>--%>
                            </tr>
                        </table>
                    </h3>
                    <div style="border-collapse: collapse; width: 100%; display: table;">
                        <asp:Repeater ID="rptDocPrint" runat="server">
                            <ItemTemplate>
                                    <table id="tblDocPrint" runat="server" class='documentTable Docs'>
                                        <tr>
                                            <td class="TableCheckBox">
                                                <asp:CheckBox ID="CheckBox" class='chkbox' runat="server" />
                                            </td>
                                            <td class="TableCategory">
                                                <asp:Label runat="server" ID="Description" class='Description' Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label>
                                            </td>
                                            <td class="TableFormNumber">
                                                <asp:Label runat="server" ID="FormNum" class="FormNum" Text='<%# DataBinder.Eval(Container.DataItem, "FormNumber")%>'></asp:Label>
                                            </td>
                                            <%--<td class="TableFormVer">
                                                <%# DataBinder.Eval(Container.DataItem, "FormVersionID")%>
                                            </td>--%>
                                        </tr>
                                    </table>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </section>
                <br />
                <br />
                <div align="center">
                    <asp:Button ID="btnPrint" runat="server" Text="No documents were selected to print" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500" disabled="disabled"/>
                    <asp:Button ID="btnReturn" runat="server" Text="Return to App Summary" CssClass="StandardSaveButton" min-Width="150px" TabIndex="501" />
                </div>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</div>
