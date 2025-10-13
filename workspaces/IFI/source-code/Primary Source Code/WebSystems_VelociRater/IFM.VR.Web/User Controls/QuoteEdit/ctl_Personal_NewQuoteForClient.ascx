<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Personal_NewQuoteForClient.ascx.vb" Inherits="IFM.VR.Web.ctl_Personal_NewQuoteForClient" %>


<script type="text/javascript">

    var DialogInitedNewQuote = false;
    function InitNewQuote() {
        if (DialogInitedNewQuote == false) {
            DialogInitedNewQuote= true;
            $("#<%=Me.divNewQuote.ClientId%>").dialog({
                title: "New Quote for this Client",
                width: 400,
                height: 325,
                draggable: false,
                autoOpen: true,
                dialogClass: "no-close"
            });
            $("#<%=Me.divNewQuote.ClientId%>").parent().appendTo(jQuery("form:first"));
        }
        else {
            $("#<%=Me.divNewQuote.ClientId%>").dialog("open");
        }
        return false;
    }

    function closeDialog() {
        $("#<%=Me.divNewQuote.ClientId%>").dialog("close");
    }

    function newQuote_ValidateForm() {
        if ($(".divNewQuote input:radio").is(":checked")) {
            return true;
        }
        else {
            if (!$(".divNewQuote div.NewQuoteErrorRow")[0]) {
                ShowError("<%=Me.btnContinue.ClientId%>", "Missing Line of Business");
            }
            return false;
        };
    }


    function ShowError(ControlId, ErrorMsg) {
        $('#' + ControlId).after('<div class="NewQuoteErrorRow"><span style="color: red">' + ErrorMsg + '</span></div>');
    }

</script>

<style>

    .divNewQuote {
        margin-left: auto; 
        margin-right: auto; 
        display:none;
    }

    .divNewQuote table {
        border-collapse: collapse;
        width:100%;
    }

    .divNewQuote .messagespacer {
        text-align:center;
        width:100%;
        font-size:large; 
        margin-bottom: 10px;
    }

    .divNewQuote .listspacer {
        display: inline-block;
        margin: 5px auto 5px 90px;

    }

    .divNewQuote .buttonspacer {
        margin-top: 10px;
    }
</style>

<div id="divNewQuote" class="divNewQuote" runat="server" title="New Quote for this Client">
    <table id="tblNewQuote">
        <tr>
            <td colspan="2" class="messagespacer">Your quote was created successfully. Please select the line of business below.</td>
        </tr>
        <tr>
            <td colspan="2" style="width:100%;">
                <asp:RadioButtonList ID="rblLOBList" GroupName="LOBList" runat="server"
                                     DataTextField="Key" DataValueField="Value" >
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center;">
                <asp:Button ID="btnContinue" CssClass="StandardButton buttonspacer" OnClientClick="return newQuote_ValidateForm();" runat="server" Text="OK" />
                <asp:Button ID="btnClose" CssClass="StandardButton buttonspacer" OnClientClick="closeDialog();" runat="server" Text="Cancel" />
            </td>
        </tr>
    </table>
</div>