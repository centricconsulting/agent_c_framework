<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CopyVrQuotes.aspx.vb" Inherits="IFM.VR.Web.CopyVrQuotes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Copy VR Quotes</title>
    <script type="text/javascript">
        //function DisableButton(el, msg) {
        //    if (el) {
        //        el.disabled = true;
        //        el.style.pointerEvents = "none";
        //        if (typeof (msg) == "undefined" || msg.length == 0) {
        //            msg = "Processing...";
        //        }
        //        el.text = msg;
        //    }
        //}

        function DisableControl(controlId, submitText) {
            document.getElementById(controlId).disabled = true;
            document.getElementById(controlId).value = submitText;
        }

        function EnableControl(controlId) {
            document.getElementById(controlId).disabled = false;
        }

        function DisableControl_SetTimeout(controlId, submitText, interval) {
            setTimeout("DisableControl('" + controlId + "', '" + submitText + "')", interval);
        }

        function btnSubmit_Click(control, submitText) {
            if ((typeof (submitText) == "undefined") || (submitText == (null || ""))) {
                submitText = 'Submitting...';
            }
            DisableControl_SetTimeout(control.id, submitText, 100);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <b>From Agency Code:</b> <asp:TextBox runat="server" ID="txtFromAgencyCode"></asp:TextBox>
            <br />
            <b>To Agency Code:</b> <asp:TextBox runat="server" ID="txtToAgencyCode"></asp:TextBox>
            <br /><br />
            <asp:Button runat="server" ID="btnLookup" Text="Find Codes" OnClientClick="javascript:btnSubmit_Click(this, 'Searching...');" />
            <br /><br />
            <asp:Label runat="server" ID="lblLookupText"></asp:Label>
            <br /><br />
            <asp:Button runat="server" ID="btnReset" Text="Reset" OnClientClick="javascript:btnSubmit_Click(this, 'Resetting...');" />
            &nbsp;&nbsp;
            <asp:Button runat="server" ID="btnGo" Text="Copy Quotes" OnClientClick="javascript:btnSubmit_Click(this, 'Copying...');" />
            <div runat="server" id="ResultsSection">
                <br /><br />
                <b>Results:</b>
                <br />
                <asp:Label runat="server" ID="lblResults"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
