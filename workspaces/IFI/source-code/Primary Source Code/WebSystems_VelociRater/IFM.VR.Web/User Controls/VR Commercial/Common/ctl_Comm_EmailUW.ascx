<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Comm_EmailUW.ascx.vb" Inherits="IFM.VR.Web.ctl_Comm_EmailUW" %>

<script type="text/javascript">

    var DialogInitedEmailUW = false;
    function InitEmailToUW() {
        ClearEmailToUWFields();
        if (DialogInitedEmailUW == false) {
            DialogInitedEmailUW = true;
            $("#<%=Me.divEmailUW.ClientId%>").dialog({
                title: "Email for Underwriting Assistance",
                width: 400,
                height: 500,
                draggable: false,
                autoOpen: true,
                dialogClass: "no-close",
                modal: true //added 1/9/2023
            });
            $("#<%=Me.divEmailUW.ClientId%>").parent().appendTo(jQuery("form:first"));
        }
        else {
            $("#<%=Me.divEmailUW.ClientId%>").dialog("open");
        }
        return false;
    }

    function ClearEmailToUWFields() {
        document.getElementById('<%=Me.txtEmail.ClientId%>').value = '';
        document.getElementById('<%=Me.TxtPhoneNumber.ClientId%>').value = '';
        document.getElementById('<%=Me.txtName.ClientId%>').value = '';
        document.getElementById('<%=Me.txtUserMessage.ClientId%>').value = '';
    }

    function HideEmailToUW() {
        InitEmailToUW();
        $("#<%=Me.divEmailUW.ClientId%>").dialog("close");
    }

    function EmailUW_ValidateForm()
    {
        // Remove all formatting and error messages
        HideErrorsEUW();

        var HasErrors = false;
        // Validate Email Body Text
        if ($("#<%=Me.txtUserMessage.ClientId%>").val() == '') {
            // Show Error
            HasErrors = true;
            document.getElementById('<%=Me.txtUserMessage.ClientId%>').style.border = "solid 1px red";
            ShowErrorEUW('<%=Me.txtUserMessage.ClientId%>', "Email Message is required");
        }
        else {
            if ($("#<%=Me.txtUserMessage.ClientId%>").val().length < 10) {
                HasErrors = true;
                document.getElementById('<%=Me.txtUserMessage.ClientId%>').style.border = "solid 1px red";
                ShowErrorEUW('<%=Me.txtUserMessage.ClientId%>', "Email Message must be at least 10 characters in length");
            }
        }

        // Validate Name
        if ($("#<%=Me.txtName.ClientId%>").val() == '') {
            // Show Error
            HasErrors = true;
            document.getElementById('<%=Me.txtName.ClientId%>').style.border = "solid 1px red";
            ShowErrorEUW('<%=Me.txtName.ClientId%>', "Name is required");
        }
        else {
            if ($("#<%=Me.txtName.ClientId%>").val().length < 3) {
                HasErrors = true;
                document.getElementById('<%=Me.txtName.ClientId%>').style.border = "solid 1px red";
                ShowErrorEUW('<%=Me.txtName.ClientId%>', "Name must be at least 3 characters in length");
            }
        }

        // Validate Phone Number
        if ($("#<%=Me.TxtPhoneNumber.ClientId%>").val() == '') {
            // Show Error
            HasErrors = true;
            document.getElementById('<%=Me.TxtPhoneNumber.ClientId%>').style.border = "solid 1px red";
            ShowErrorEUW('<%=Me.TxtPhoneNumber.ClientId%>', "Phone Number is required");
        }
        else {
            var phval = $("#<%=Me.TxtPhoneNumber.ClientId%>").val();

            var phonefilter = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/;
            if (!phval.match(phonefilter)) {
                HasErrors = true;
                document.getElementById('<%=Me.TxtPhoneNumber.ClientId%>').style.border = "solid 1px red";
                ShowErrorEUW('<%=Me.TxtPhoneNumber.ClientId%>', "Phone Number is invalid.  Use format 999-999-9999.");
            }
        }

        // Validate Email
        if ($("#<%=Me.txtEmail.ClientId%>").val() == '') {
            // Show Error
            HasErrors = true;
            document.getElementById('<%=Me.txtEmail.ClientId%>').style.border = "solid 1px red";
            ShowErrorEUW('<%=Me.txtEmail.ClientId%>', "Email Address is required");
        }
        else {
            var emval = $("#<%=Me.txtEmail.ClientId%>").val();
            var emailfilter = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            //var emailfilter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (!emval.match(emailfilter)) {
                HasErrors = true;
                document.getElementById('<%=Me.txtEmail.ClientId%>').style.border = "solid 1px red";
                ShowErrorEUW('<%=Me.txtEmail.ClientId%>', "Email Address is invalid");
            }
        }

        var btn = document.getElementById('<%=Me.btnContinue.ClientId%>');
        var sendingBtn = document.getElementById('<%=Me.btnSending.ClientId%>');
        if (HasErrors) {
            // Errors found - Show the Continue button, hide the Sending button
            if (btn && sendingBtn) {
                btn.style.display = '';
                sendingBtn.style.display = 'none';
                btn.value = "Continue";
            }
            return false;
        } else {
            // All OK - hide the Continue button, show the Sending button
            if (btn && sendingBtn){
                btn.style.display = 'none';
                sendingBtn.style.display = '';
                btn.value = "Sending...";
            }
            return true;
        }
    }

    function ShowErrorEUW(ControlId, ErrorMsg) {
        $('#' + ControlId).after('<div class="ErrorRow"><span style="color: red">' + ErrorMsg + '</span></div>');
    }

    function HideErrorsEUW() {
        $(".ErrorRow").remove();
        document.getElementById('<%=Me.txtEmail.ClientId%>').style.border = "none";
        document.getElementById('<%=Me.TxtPhoneNumber.ClientId%>').style.border = "none";
        document.getElementById('<%=Me.txtName.ClientId%>').style.border = "none";
        document.getElementById('<%=Me.txtUserMessage.ClientId%>').style.border = "none";
    }

    function checkEmail(emailvalue) {

        //var email = document.getElementById('txtEmail');

        if (!filter.test(emailvalue)) {
            alert('Please provide a valid email address');
            email.focus;
            return false;
        }
    }
</script>

<style>
    .EUWLabelCol {
        text-align:right;
        width:40%;
    }
    .EUWDataCol {
        text-align:left;
        width:60%;
    }
    .EUWTxt {
        width:80%;
    }
</style>
<div id="divEmailUW" runat="server" title="Email for Underwriting Assistance" style="margin-left: auto; margin-right: auto; display:none; ">
    <table id="tblEmailUW" style="border-collapse: collapse;width:100%;">
        <tr>
            <td colspan="2" style="text-align:center;width:100%;font-size:large;">What would you like help with?</td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center;">
                <asp:TextBox ID="txtUserMessage" runat="server" Width="90%" TextMode="MultiLine" Height="200px" Style="text-align:left;"></asp:TextBox>
            </td>
        </tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr>
            <td class="EUWLabelCol">Name:</td>
            <td class="EUWDataCol">
                <asp:TextBox ID="txtName" runat="server" CssClass="EUWTxt" Width="55%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="EUWLabelCol">Phone Number:</td>
            <td class="EUWDataCol">
                <asp:TextBox ID="TxtPhoneNumber" runat="server" CssClass="EUWTxt" Width="55%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="EUWLabelCol">Email:</td>
            <td class="EUWDataCol">
                <asp:TextBox ID="txtEmail" runat="server" CssClass="EUWTxt" Width="86%"></asp:TextBox>
            </td>
        </tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr>
            <td colspan="2" style="text-align:center;">
                <asp:Button ID="btnContinue" CssClass="StandardButton" OnClientClick="return EmailUW_ValidateForm();" runat="server" Text="Continue" />
                <asp:Button ID="btnSending" CssClass="StandardButton" runat="server" Text="Sending..." Enabled="false" style="display:none;" />
                <asp:Button ID="btnCancel" CssClass="StandardButton" OnClientClick="HideEmailToUW(); return false;" runat="server" Text="Cancel" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
</div>