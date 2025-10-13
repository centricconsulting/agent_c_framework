<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlProposalSelection.ascx.vb" Inherits="IFM.VR.Web.ctlProposalSelection" %>


<!-- #region javascript -->

<script type="text/javascript">

    // Initial Page load.
    $(function () {
        // Alternate color backgrounds for table items.
        $(".documentTable:odd").css("background-color", "#dddddd");

        $('.chkbox').each(function () {
            if ($(this).find("input").prop('checked')) {
                //Change Text Enable
                $(".PropPrintDiv").find(".printBtn").val('Generate Proposal');
                $(".PropPrintDiv").find(".printBtn").prop("disabled", false);
                return false
            }
        });
    });

    // Live functionality
    $(function () {
        // find printBtn
        $('.chkbox').on('change', function () {

            $(".PropPrintDiv").find(".printBtn").val('No documents were selected to print');
            $(".PropPrintDiv").find(".printBtn").prop("disabled", true);

            $('.chkbox').each(function () {
                if ($(this).find("input").prop('checked')) {
                    // atleast one is selected
                    //Change Text Enable
                    $(".PropPrintDiv").find(".printBtn").val('Generate Proposal');
                    $(".PropPrintDiv").find(".printBtn").prop("disabled", false);
                    return false
                }
            });

        });

    });

</script>

<!-- #endregion -->

<header>
    <style type="text/css">
        .PropPrintDiv {
            clear: both;
        }

        .PropPrintDiv, table.documentTable {
            border-spacing: 2px;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            padding: 4px;
        }

        .PropPrintDiv .TableCheckBox {
            width: 25px;
        }

        .PropPrintDiv .TableHidden {
            display: none;
        }

            .PropPrintDiv .HeaderQuoteNumber {
                width: 100px;
            }

            .PropPrintDiv .HeaderType {
                width: 40px;
            }

            .PropPrintDiv .HeaderDate {
                width: 75px;
            }

            .PropPrintDiv .HeaderPremium {
                padding-right: 4px;
                text-align: right;
                width: 85px;
            }

            .PropPrintDiv .HeaderDescription {
                width: 100px;
                padding-left: 4px;
            }

            .PropPrintDiv .TableQuoteNumber {
                width: 100px;
            }

            .PropPrintDiv .TableType {
                width: 40px;
            }

            .PropPrintDiv .TableDate {
                width: 75px;
            }

            .PropPrintDiv .TablePremium {
                padding-right: 4px;
                text-align: right;
                width: 85px;
            }

            .PropPrintDiv .TableDescription {
                padding-left: 4px;
            }


        .PropPrintDiv .NormalBorder {
            border: none;
        }

        .PropPrintDiv .PropPrintSection {
            margin: auto 30px;
        }

        .PropPrintDiv .HelpTextSection {
            margin: auto 30px;
        }

        .PropPrintDiv td {
            /*padding: 4px 4px;*/
        }

        .PropPrintDiv .Note {
            margin: auto 60px;
            text-align: center;
            width: 400px;
        }

        .PropPrintDiv .lblAnswerAll {
            clear: both;
            margin: 10px 0 10px 30px;
        }

        .PropPrintDiv .truncated {
            display: inline-block;
            white-space: nowrap; /* forces text to single line */
            overflow: hidden;
            text-overflow: ellipsis;
            width: 183px;

        }

</style>
</header>

<div id="PropPrintDiv" class="PropPrintDiv">
    <h3>
        <asp:Label ID="Label1" runat="server" Text="Proposal Selection"></asp:Label>
    </h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td valign="top">
                <div class="lblAnswerAll">
                    <asp:Label ID="lblAnswerAll" runat="server" Text="Select Quotes to include in the Proposal"></asp:Label>
                </div>
                <%--Repeat for each item--%>
                <section class="PropPrintSection">
                    <h3>
                        <table id="tblPropPrint" runat="server" class="documentTable">
                            <tr>
                                <td class="HeaderQuoteNumber">
                                    <asp:Label ID="lblAccordHeaderQuoteNumber" runat="server" Text="Quote Number"></asp:Label>
                                </td>
                                <td class="HeaderType">
                                    <asp:Label ID="lblAccordHeaderType" runat="server">Type</asp:Label>
                                </td>
                                <td class="HeaderDate">
                                    <asp:Label ID="lblAccordHeaderDate" runat="server" Text="Date"></asp:Label>
                                </td>
                                <td class="HeaderPremium">
                                    <asp:Label ID="lblAccordHeaderPremium" runat="server">Premium</asp:Label>
                                </td>
                                <td class="HeaderDescription">
                                    <asp:Label ID="lblAccordHeaderDescription" runat="server" Text="Description"></asp:Label>
                                </td>

                            </tr>
                        </table>
                    </h3>
                    <div style="border-collapse: collapse; width: 100%; display: table;">
                        <asp:Repeater ID="rptPropPrint" runat="server">
                            <ItemTemplate>
                                <table id="tblPropPrint" runat="server" class='documentTable Props'>
                                    <tr>
                                        <td class="TableCheckBox">
                                            <asp:CheckBox ID="CheckBox" class='chkbox' runat="server" />
                                        </td>
                                        <td class="TableHidden">
                                            <asp:Label runat="server" ID="quoteID" class='quoteID' Text='<%# DataBinder.Eval(Container.DataItem, "quoteid")%>'></asp:Label>
                                        </td>
                                        <td class="TableHidden">
                                            <asp:Label runat="server" ID="clientID" class="clientID" Text='<%# DataBinder.Eval(Container.DataItem, "clientid")%>'></asp:Label>
                                        </td>
                                        <td class="TableQuoteNumber">
                                            <asp:Label runat="server" ID="QuoteNumber" class='QuoteNumber' Text='<%# DataBinder.Eval(Container.DataItem, "quotenumber")%>'></asp:Label>
                                        </td>
                                        <td class="TableType">
                                            <asp:Label runat="server" ID="Type" class="Type" Text='<%# getLobType(DataBinder.Eval(Container.DataItem, "quotenumber"))%>'></asp:Label>
                                        </td>
                                        <td class="TableDate">
                                            <asp:Label runat="server" ID="Date" class="Date" Text='<%#CDate(DataBinder.Eval(Container.DataItem, "updated")).ToString("MM/dd/yyyy")%>'></asp:Label>
                                        </td>
                                        <td class="TablePremium">
                                            <asp:Label runat="server" ID="Premium" class='Premium' Text='<%# DataBinder.Eval(Container.DataItem, "premium")%>'></asp:Label>
                                        </td>
                                        <td class="TableDescription">
                                            <asp:Label runat="server" ID="Description" class="Description truncated" Text='<%# DataBinder.Eval(Container.DataItem, "qxmldesc")%>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </section>
                <br />
                <br />
                <div align="center">
                    <asp:Button ID="btnPrint" runat="server" Text="No quotes were selected" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500" disabled="disabled" />
                    <asp:Button ID="btnReturn" runat="server" Text="Return to Quote Summary" CssClass="StandardSaveButton" min-Width="150px" TabIndex="501" />
                </div>

                <br />

                <table class="HelpTextSection">
                    <tr>
                        <td align="left">In order to print Proposals that look as uniform as possible we recommend the following:
                            <ul>
                                <li>Use the most current version of your web browser.</li>
                                <li>We recommend you set your web browser to display no headers during printing.</li>
                                <li>We recommend you print the page number and date as your footers.</li>
                            </ul>

                            Can't find the quote you are looking for? 
                            <ul>
                                <li>Quotes will only display on this page if they have been generated for the EXACT SAME client record. If you have added this client in the past then you must select the client from the client search results on the policyholder page.</li>
                                <li>Quotes will only be displayed on the above list if they were rated in the last 60 days</li>
                                <li>Quotes will only be displayed in the above list if you have completed the quoting process to the point the successful quote summary page is displayed.</li>
                                <li>Still having issues? Please call or email your marketing contact for additional assistance.</li>
                            </ul>

                            Logo issues? 
                            <ul>
                                <li>Your agency logo can be displayed on the cover sheet of the proposal. If it is not displaying or if you want to make logo changes please call or email your Marketing contact for additional assistance.</li>
                            </ul>

                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
