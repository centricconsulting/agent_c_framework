<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlMiniClientSearch.ascx.vb" Inherits="IFM.VR.Web.ctlMiniClientSearch" %>

<script type="text/javascript">

    var enabledClientId = "<%=Me.miniClientSearchEnabled.ClientID%>";

    var agencyID = <%=Me.AgencyId%>;
    //var lastClientLookup = null;

    $(document).ready(function () {
        ClientSearch.InitMini();
    });
</script>

<style>
    #divResults div {
        border: solid .4px grey;
        cursor: pointer;
        margin-bottom: 5px;
        color: white;
        background-color: #808080;
    }

        #divResults div:hover {
            background-color: #828782 !important;
            border-color: #66afe9;
            /*box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset, 0 0 8px rgba(102, 175, 233, 0.6);
    outline: 0 none;*/
        }
</style>

<%--<div id="divClientSearch">
        <div id="divResults">
    </div>
    <asp:HiddenField ID="miniClientSearchEnabled" runat="server" Value="true" />
</div>--%>

<div id="divClientSearch" style="width: 400px; height: 500px;" class="ui-dialog ui-widget ui-widget-content ui-corner-all ui-front">
    <div id="divClientSearchHeader" class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
        Search Results (Click on desired customer)
        <button title="Close" onclick="ClientSearch.HideClientSearch();" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-icon-only ui-dialog-titlebar-close ui-state-focus" role="button" type="button"><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span><span class="ui-button-text">Close</span></button>
    </div>
    <div id="divResults" class="ui-dialog-content ui-widget-content" style="height: 90%;"></div>
    <asp:HiddenField ID="miniClientSearchEnabled" runat="server" Value="true" />
</div>