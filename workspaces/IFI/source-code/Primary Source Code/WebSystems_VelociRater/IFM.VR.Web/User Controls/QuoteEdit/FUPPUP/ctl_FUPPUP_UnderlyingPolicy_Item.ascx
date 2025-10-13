<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FUPPUP_UnderlyingPolicy_Item.ascx.vb" Inherits="IFM.VR.Web.ctl_FUPPUP_UnderlyingPolicy_Item" %>

<script type="text/javascript">

    

    // Live functionality
    $(function () {

        $('.PolNumInput').on('keydown', function (event) {
            CheckLobTypes($(this));
            CheckAddAnotherDisplay($(this));
            //CheckDeleteDisplay($(this));
        });


        $("[id*='btnAddAnother']").on('click', function (event) {
            var ParentLobSection = $(this).closest('table').closest('td');
            $(ParentLobSection).find("a[id*='btnAddAnother']").slice(1).remove()
        });

    });

    

     // Initial Page load.
    $(function () {
        $("div[id*='divUmbrellaUnderlyingPolicyItem']").each(function (index) {
            $(this).find("a[id*='btnAddAnother']").slice(1).remove()

        });

        CheckFirstTextboxForAddAnotherOnLoad();
    });

    // debounce is in vrPersonal.js
    // Delay text checking until one half second after typing ends
    this.CheckLobTypes = debounce(function (element) {
        CheckLobTypeCorrectness(element);
    }, 500);

    this.CheckAddAnotherDisplay = debounce(function (element) {
        CheckFirstTextboxForAddAnother(element);
    }, 500);

    this.CheckDeleteDisplay = debounce(function (element) {
        CheckForDelete(element);
    }, 500);

    function CheckLobTypeCorrectness(element) {
            var ParentLobTable = $(element).closest('table');
            var LobType = $(element).first().data('lob');
            var EnteredNum = $(element).val().trim();
            var regex = new RegExp("^(" + "Q" + LobType.toUpperCase() + "\\d{5,6}|" + LobType.toUpperCase() + "\\d{7})$", "g");
            //const regex = /^(QHOM\d{6}|HOM\d{7})$/g;

            RemovePolicyElementError(element);

            if (EnteredNum.length >= 3) {
            

                if (regex.test(EnteredNum.toUpperCase()) == false) {
                    SetPolicyElementError(element, "Number must start with Q" + LobType.toUpperCase() + " or " + LobType.toUpperCase());
                };

            }
            else {
                if (EnteredNum.length > 0) {
                    SetPolicyElementError(element, "Number must start with Q" + LobType.toUpperCase() + " or " + LobType.toUpperCase());
                };
            
            }
    }

    function CheckFirstTextboxForAddAnotherOnLoad() {
        $("div[id*='divUmbrellaUnderlyingPolicyItem']").find("input[id*='txtPolicyNumber']").each(function (index) {
            CheckFirstTextboxForAddAnother($(this));
            //CheckForDelete($(this));
        });
    }

    function CheckFirstTextboxForAddAnother(element) {
        var LinkElement = $(element).parent().next().find("a[id*='btnAddAnother_0']")
        if ($(element).val() != "" && isSingleLOBException(LinkElement) == false) {
            $(LinkElement).show();
        }
        else {
            $(LinkElement).hide();
        }

    }

    function CheckForDelete(element) {
        if ($(element).val() != "") {
            $(element).parent().next().next().find("a[id*='btnDelete']").show();
        }
        else {
            $(element).parent().next().next().find("a[id*='btnDelete']").hide();
        }

    }

    function isSingleLOBException(LinkElement) {
        var LinkId = $(LinkElement).attr('id')
        if (LinkId) {
            if (LinkId.indexOf('wcp') != -1 ||
                LinkId.indexOf('ppa') != -1 ||
                LinkId.indexOf('cap') != -1) {
                return true;
            }
        }
        return false;
    };

    function SetPolicyElementError(element, message) {
        var ParentLobTable = $(element).closest('table')[0];
        var ErrorMessageText = $(ParentLobTable).find($("[id*='PolicyErrorMessage']"))[0];
        $(element).addClass("polnum-validation-error")
        $(ErrorMessageText).text(message)

    }

    function RemovePolicyElementError(element) {
        var ParentLobTable = $(element).closest('table')[0];
        var ErrorMessageText = $(ParentLobTable).find($("[id*='PolicyErrorMessage']"))[0];
        $(element).removeClass("polnum-validation-error")
        $(ErrorMessageText).text('')
    }
       
</script>

<style>
    input[type='text'].polnum-validation-error {
        background: #FFF8F8;
        border: 1px solid #a00000;
        border-radius: 4px;
        color: #a00000;
        padding: 4px;
        outline: none;
    }
</style>

<div runat="server" id="divUmbrellaUnderlyingPolicyItem">
    <table>
        <tr>
            <td  style="margin-left: 10%;padding-top: 6px;width:40%;text-align:left;vertical-align: top;>
                <asp:Label ID="lblLobCategoryText" runat="server" />

            </td>
            <td>
                <asp:Repeater ID="rptPolicyList" runat="server">
                    <ItemTemplate>
                        <table id="tblPolicyList" runat="server" style="width: 75%; margin-left: 0px; table-layout: fixed;" cellpadding="0" cellspacing="0">
                            <tr>
                                <td id="txtPolicyNumberCell"  style="width:12em;text-align:left;">
                                    <asp:TextBox ID="txtPolicyNumber" runat="server" data-lob="" CssClass="PolNumInput"></asp:TextBox>
                                </td>
                                <td id="btnAddAnotherCell" runat="server" visible="false" style="width:8em;">
                                    <asp:LinkButton ID="btnAddAnother" CommandName="btnAdd" runat="server" ToolTip="Add Another Policy" CssClass="RemovePanelLink">Add Another</asp:LinkButton>
                                </td>
                                <td id="btnDeleteCell" runat="server" visible="false" style="text-align:right;">
                                    <asp:LinkButton ID="btnDelete" CommandName="btnDelete" runat="server" ToolTip="Delete" CssClass="RemovePanelLink">Delete</asp:LinkButton>
                                </td>
                            </tr>
                            <tr  id="PolicyNameRow" runat="server" visible="false">
                                <td style="text-align:left; padding: 2px;" colspan="3">Policyholder:
                                    <asp:Label ID="PolicyholderName" runat="server" />
                                </td>
                            </tr>
                            <tr id="PolicyErrorRow" runat="server">
                                <td style="text-align:left; padding: 2px;" colspan="3">
                                    <asp:Label ID="PolicyErrorMessage" runat="server" CssClass="informationalTextRed" />
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
        </tr>

    </table>
    <asp:Label ID="lblLobCategoryMsg" runat="server" class="informationalText" />
</div>

