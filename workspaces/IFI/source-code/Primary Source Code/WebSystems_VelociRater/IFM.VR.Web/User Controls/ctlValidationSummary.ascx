<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlValidationSummary.ascx.vb" Inherits="IFM.VR.Web.ctlValidationSummary" %>

<script type="text/javascript">
    $(document).ready(function () {
        var popUpDiv = $("#divForPopups");
        $("#<%=divValidationSummary.ClientID%>").dialog({
            //title: "Quote was saved. Please check the following items...",
            position: { my: "left top", at: "left+40 top+150", of: popUpDiv },
            height: 300,
            width: 400,
            //draggable: true,
            autoOpen: true
            , appendTo: popUpDiv
            //,dialogClass: "no-close"
            , open: function (type, data) { $(this).parent().css({ "position": "fixed" }); }            //$(this).parent().appendTo("form"); $(this).parent().css({ "position": "fixed" });
        });
       $("#<%=divValidationSummary.ClientID%>").fadeIn();
        
    });
</script>

<div runat="server" id="divValidationSummary" style="display: none;" title="Quote was saved. Please check the following items...">
    <h3></h3>
    <div>
        <asp:Literal ID="litValDisplay" runat="server"></asp:Literal>
    </div>
</div>
<asp:Literal ID="litScript" runat="server"></asp:Literal>