<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlWarnSessionExpiring.ascx.vb" Inherits="IFM.VR.Web.ctlWarnSessionExpiring" %>

<script type="text/javascript">
    // this control is here to help avoid a session time out if user is still looking at the screen
    var resetTime = <%=Me.TimeOut %>;
    var secondsUntilLogOut = resetTime;
    var autoLogOutsuspended = false;
    var alwaysShowRemainingTime = <%=Me.AlwaysShowRemainingTime %>;

    function WarnLoggingOut() {
        if (autoLogOutsuspended) { // suspend if a pop up window is open
            secondsUntilLogOut = resetTime;
            $('#<%=autoLogOutSmallPopUp.ClientID %>').hide();
        }
        else {
            if (secondsUntilLogOut < 0) {
                LogOut();
                return;
            }
            else {

                if (secondsUntilLogOut < 60) {
                    $('#<%=autoLogOutSmallPopUp.ClientID %>').show();
                    $('#<%=btnPostBack.ClientID%>').focus();
                }
                else
                { $('#<%=autoLogOutSmallPopUp.ClientID %>').hide(); }

                if (secondsUntilLogOut <= 60) {
                    $('#<%=autoLogOutPopUp.ClientID %>').show();
                }
                else
                {$('#<%=autoLogOutPopUp.ClientID %>').hide();  }

                if (secondsUntilLogOut < 30) {
                    //$('#<%=autoLogOutSmallPopUp.ClientID %>').fadeTo(200, .6).delay(200).fadeTo(200, 1);
                    $('#<%=autoLogOutPopUp.ClientID%>').fadeTo(200, .6).delay(200).fadeTo(200, 1);
                }

                if (alwaysShowRemainingTime)
                    $('#<%=autoLogOutSmallPopUp.ClientID %>').show();

                secondsUntilLogOut -= 1;
                $('#<%=autoMagicLogout.ClientID %>').html('Logging Out in ' + (secondsUntilLogOut).toString() + ' seconds.');
            }
        }
        // all paths except 'log out' need to pass back over this next line to continue timer
        setTimeout('WarnLoggingOut();', 1000);

    }

    function LogOut() {
        window.parent.location =  "<%=ConfigurationManager.AppSettings("HomeLink").ToString.Trim%>?autologout=yes&source=VRPersonal";
    }

    $(document).ready(function () {
        // This will shut off Timeouts for debugging purposes.
        // set isDebug to false to test timeout warnings.
        var isDebug = <%=Me.IsDebug.ToString.ToLower %>;
        //var isDebug = false;
        if (isDebug == false) {
            WarnLoggingOut();
            $('form').submit(function () { secondsUntilLogOut = resetTime; });
        }

    });

    try
    {
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    }
    catch (err)
    {
        // must be a non-ajax enabled page which is fine
    }

    function EndRequestHandler(sender, args) {
        secondsUntilLogOut = resetTime;
    }
    function BeginRequestHandler(sender, args)
    {
        secondsUntilLogOut = resetTime;
    }
</script>

<div id="autoLogOutPopUp" class="autoLogOutPopUp" runat="server" style="display: none;">
    <div id="autoLogOutSmallPopUp" runat="server" style="display: none; text-align: center; font-size: 11pt;">
        <asp:Label ID="autoMagicLogout" runat="server" Text="Log Out In " ToolTip="Why does this happen? This is for the security of your online account information."></asp:Label>
    </div>
    <br />
    <p>You are about to be logged out due to inactivity.</p>
    <p>
        <asp:Button ID="btnPostBack" OnClientClick="secondsUntilLogOut = resetTime;" CssClass="StandardSaveButton" runat="server" Text="Continue Session" />
    </p>
</div>