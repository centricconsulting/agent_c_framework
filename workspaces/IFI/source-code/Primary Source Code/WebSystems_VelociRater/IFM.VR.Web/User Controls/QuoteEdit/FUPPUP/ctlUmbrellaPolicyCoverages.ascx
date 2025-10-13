<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlUmbrellaPolicyCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlUmbrellaPolicyCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_CoverageLimits.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_CoverageLimits" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_UnderlyingPolicies.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_UnderlyingPolicies" %>


<script type="text/javascript">
    // Live functionality
    function ValidateForm() {
        var hasData = false;
        $(".PolNumInput").each(function () {
            var PolItem = $(this)[0];
            if ($.trim($(PolItem).val()) !== "") {
                hasData = true;
                return;
                
            }
        });
        if (hasData == false) {
            alert("At least 1 policy or quote number is required.");
            return false;
        }
        return true;
    }

</script>
<div id="dvUmbrellaPolicyCoverages" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblMainHeader" runat="server" Text="COVERAGES"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearGeneralInfo" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Coverage to Default Values" CssClass="RemovePanelLink">Clear Page</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveGeneralInfo" runat="server" ToolTip="Save Policy Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <uc1:ctl_FUPPUP_CoverageLimits runat="server" id="ctl_FUPPUP_CoverageLimits" />
        <uc1:ctl_FUPPUP_UnderlyingPolicies runat="server" id="ctl_FUPPUP_UnderlyingPolicies" />

        


        <div style="margin-top: 20px; width: 100%; text-align:center;">
            <asp:Button ID="btnGetPolicyInfo" runat="server" Text="Get Underlying Policy Info" CssClass="StandardSaveButton" OnClientClick="return ValidateForm();"/>
            <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" OnClientClick="return ValidateForm();"/>
            <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" visible="false"/>
        </div>

        <br />

        <div align="center">
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
        </div>

        <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />



    </div>

   
</div>

