<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_RouteToUw.ascx.vb" Inherits="IFM.VR.Web.ctl_RouteToUw" %>

<script type="text/javascript">

    $(document).ready(function () {
        $("#<%= divRouteToUw.clientId%>").dialog({
            title: "Route to Underwriting",
            width: 380,
            height: 320,
            autoOpen: false,
            dialogClass: "no-close",
            open: function (type, data) { $(this).parent().appendTo("form"); },
            modal: true //added 1/9/2023
        });
    });

    function OpenRoutePopup(divId) {
        $("#" + divId).dialog("open");
    }

    function CloseRoutePopup(divId) {
        $("#" + divId).dialog("close");
    }

    <%--var canSkipUwPrompt_<%= divRouteToUw.clientId%> = false;--%>
</script>

<%--<asp:Button ID="btnRouteToUw" ClientIDMode="Static" CssClass="StandardSaveButton" runat="server" Text="Route to Underwriting" />--%>
<input id="btnRouteToUw" class="StandardSaveButton" onclick='var proceed = confirm("Proceed with Route to Underwriting?"); if(proceed){OpenRoutePopup("<%= divRouteToUw.ClientId%>");}' type="button" value="Route to Underwriting" />
<div style="text-align: center; display: none;" id="divRouteToUw" runat="server">
    <br />
    Information or action needed from Underwriting in order to proceed with the quote:
                <br />
    <br />
    <asp:TextBox ID="txtRouteMessage" Width="300px" Height="150px" TextMode="MultiLine" runat="server"></asp:TextBox>
    <br />
    <br />
    <%--<input id="btnContinueRoute" class="StandardSaveButton" onclick='canSkipUwPrompt_<%= divRouteToUw.clientId%> = true; $("#btnRouteToUw").click();' type="button" value="Continue" />--%>
    <asp:Button ID="btnContinueRoute" CssClass="StandardSaveButton" runat="server" Text="Continue" />
    <input id="btnCancel" class="StandardSaveButton" onclick='canSkipUwPrompt_<%= divRouteToUw.clientId%> = false; CloseRoutePopup("<%= divRouteToUw.clientId%>");' type="button" value="Cancel" />
</div>
<asp:HiddenField ID="hdnVehicleInfo" runat="server" />