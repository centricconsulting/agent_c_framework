<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPrintHistory.ascx.vb" Inherits="IFM.VR.Web.ctlPrintHistory" %>

<!-- #region javascript -->

<script type="text/javascript">

    // PolicyImage Filter
    function FilterByImage() {
        var PolImgNum = PrintHistoryPolicyImgNum;
        var ShowAll = $('.chkShowAllImgs').find("input").prop('checked')
        if (PolImgNum) {
            $('.documentTable.Docs').each(function () {
                var docImgNum = parseInt($(this).find('.PolicyImage').html(), 10);
                if (docImgNum == PolImgNum || ShowAll) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                    $(this).find("input").prop('checked', false);
                }
            }); 
        }
    };

    // Initial Page load.
    $(document).ready(function () {
        // Alternate color backgrounds for table items.
        $(".documentTable:odd").css("background-color", "#dddddd");

        $(".DocPrintDiv").find(".ViewSelected").prop("title", "No documents were selected to print");
        $(".DocPrintDiv").find(".ViewSelected").prop("disabled", true);
        FilterByImage();
    });

    // Live functionality
    $(function () {
        // find PrintEnable
        $('.chkbox').on('change', function () {
            var AnyUnchecked
            $(".DocPrintDiv").find(".ViewSelected").prop("title", "No documents were selected to print");
            $(".DocPrintDiv").find(".ViewSelected").prop("disabled", true);

            $('.chkbox').each(function () {
                if ($(this).find("input").prop('checked')) {
                    //Change Text Enable
                    $(".DocPrintDiv").find(".ViewSelected").prop("title", "View Selected Documents");
                    $(".DocPrintDiv").find(".ViewSelected").prop("disabled", false);
                    //return false
                }
                else {
                    AnyUnchecked = true
                }
            });

            if (AnyUnchecked) {
                $('.chkboxhdr').find("input").prop('checked', false);
            }

        });

        $('.chkShowAllImgs').on('change', function () {
               FilterByImage()
        });

        // MultiSelect
        $('.chkboxhdr').on('change', function () {

            $(".DocPrintDiv").find(".ViewSelected").prop("title", "No documents were selected to print");
            $(".DocPrintDiv").find(".ViewSelected").prop("disabled", true);

            
            if ($(this).find("input").prop('checked')) {
                // atleast one is selected
                $('.chkbox').each(function () {
                    if ($(this).is(":hidden") == false) {
                        $(this).find("input").prop('checked', true);
                        $(".DocPrintDiv").find(".ViewSelected").prop("title", "View Selected Documents");
                        $(".DocPrintDiv").find(".ViewSelected").prop("disabled", false);
                    }
                });
            }
            else {
                // deselect all
                $('.chkbox').each(function () {
                    $(this).find("input").prop('checked', false);
                    $(".DocPrintDiv").find(".ViewSelected").prop("title", "No documents were selected to print");
                    $(".DocPrintDiv").find(".ViewSelected").prop("disabled", true);
                });
            };
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

            .DocPrintDiv .ViewSelected {

            }    
        
            .DocPrintDiv .TableCheckBox {
                width: 25px;
            }

            .DocPrintDiv .HeaderCheckBox {
                width: 25px;
            }
            
            .DocPrintDiv .chkShowAllImgs {
                width: 25px;
            }

            .DocPrintDiv .ShowAllCheckbox {
                text-align: center;
            }


            .DocPrintDiv .HeaderText {
                padding-left: 0px !important;
                padding-right: 0px !important;
            }

            .DocPrintDiv .HeaderPolicyImage{
                width: 50px;
            }
                                          
            .DocPrintDiv .HeaderFormDesc {
                width: 180px;
            }

            .DocPrintDiv .HeaderDateAdded {
                width: 100px;
             }

            .DocPrintDiv .HeaderFormNumber{
                width: 90px;
            }

            .DocPrintDiv .HeaderUnitDesc {
                width: 110px;
             }

            .DocPrintDiv .TablePolicyImage{
                width: 50px;
                text-align: center;
            }
                                          
            .DocPrintDiv .TableFormDesc {
                width: 180px;
            }

            .DocPrintDiv .TableDateAdded {
                width: 100px;
             }

            .DocPrintDiv .TableFormNumber{
                width: 90px;
            }

            .DocPrintDiv .TableUnitDesc {
                width: 110px;
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
        <asp:Label ID="Label1" runat="server" Text="Print History"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkViewSelection" ToolTip="No documents were selected to print" runat="server" CssClass="RemovePanelLink ViewSelected">View Selected</asp:LinkButton>
        </span>
    </h3>
    <table id="tblMain" runat="server" style="border-collapse: collapse; width: 100%; display: table;">
        <tr>
            <td class="ShowAllCheckbox">
                <asp:CheckBox ID="chkShowAllImgs" class='chkShowAllImgs' runat="server" /> Show All Images
            </td>
        </tr>
        <tr>
            <td valign="top">
                <section class="DocPrintSection">
                    <h3 class="HeaderText">
                        <table id="tblDocPrint" runat="server" class="documentTable">
                            <tr>
                                <td class="HeaderCheckBox">
                                    <asp:CheckBox ID="HeaderCheckBox" class='chkboxhdr' runat="server" />
                                </td>
                                <td class="HeaderPolicyImage">
                                    <asp:Label ID="lblPolicyImage" runat="server" Text="Policy Image"></asp:Label>
                                </td>
                                <td class="HeaderFormDesc">
                                    <asp:Label ID="lblFormDesc" runat="server">Form Description</asp:Label>
                                </td>
                                <td class="HeaderDateAdded">
                                    <asp:Label ID="lblDateAdded" runat="server">Date Added</asp:Label>
                                </td>
                                <td class="HeaderFormNumber">
                                    <asp:Label ID="lblFormNumber" runat="server">Form Number</asp:Label>
                                </td>
                                <td class="HeaderUnitDesc">
                                    <asp:Label ID="lblUnitDesc" runat="server">Unit Description</asp:Label>
                                </td>
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
                                            <td class="TablePolicyImage">
                                                <asp:Label runat="server" ID="PolicyImage" class='PolicyImage' Text='<%# DataBinder.Eval(Container.DataItem, "PolicyImageNum")%>'></asp:Label>
                                            </td>
                                            <td class="TableFormDesc">
                                                <asp:Label runat="server" ID="FormDesc" class='FormDesc' Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label>
                                            </td>
                                            <td class="TableDateAdded">
                                                <asp:Label runat="server" ID="DateAdded" class='DateAdded' Text='<%# DataBinder.Eval(Container.DataItem, "AddedDate", "{0:MM/dd/yyyy}")%>'></asp:Label>
                                            </td>
                                            <td class="TableFormNumber">
                                                <asp:Label runat="server" ID="FormNum" class="FormNum" Text='<%# DataBinder.Eval(Container.DataItem, "FormNumber")%>'></asp:Label>
                                            </td>
                                            <td class="TableUnitDesc">
                                                <asp:Label runat="server" ID="UnitDesc" class="FormNum" Text='<%# DataBinder.Eval(Container.DataItem, "UnitDescription")%>'></asp:Label>
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
                    <asp:Button ID="btnPolicyHistory" runat="server" Text="Policy History" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500"/>
                </div>
                <div align="center">
                    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</div>
