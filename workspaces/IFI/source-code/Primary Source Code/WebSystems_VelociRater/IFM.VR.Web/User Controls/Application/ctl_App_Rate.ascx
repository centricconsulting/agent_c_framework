<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_Rate.ascx.vb" Inherits="IFM.VR.Web.ctl_App_Rate" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>

<%@ Import Namespace="IFM.VR.Web" %>

<script src="<%=ResolveClientUrl("~/js/VR_ctl_App_Rate.js?dt=" + DirectCast(Me.Page.Master, VelociRater).ScriptDT)%>"></script>

<div>
    <center>
                <div class="standardSubSection">
                    <asp:Button ID="btnsave" CssClass="StandardSaveButton" OnClientClick="DisableFormOnSaveRemoves();" runat="server" Text="Save"></asp:Button>
<%--                    <input id="btnShowEffectiveDate" class="StandardSaveButton" type="button" value="Rate" onclick="AppRateEffectiveDate.OpenEffectiveDatePopup();" runat="server" />--%>
                    <input id="btnShowEffectiveDate" class="StandardSaveButton" type="button" value="Rate" runat="server" />
                    <asp:Button ID="btnFinalRate" style="display:none;" ClientIDMode="Static" CssClass="StandardSaveButton" runat="server" Text="Rate" />
                    <span id="spanRouteToUWContainer" style="display:none;"><uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" /></span>
                    </div>
                </center>
</div>

<asp:TextBox Style="display: none;" ID="txtEffectiveDate_Copy" ClientIDMode="Static" runat="server"></asp:TextBox>

<div id="div_EffectiveDate">
    *Effective Date:
                <asp:TextBox ID="txtEffectiveDate" onblur="$(this).val(dateFormat($(this).val()));" ClientIDMode="Static" runat="server"></asp:TextBox>
    <br />
    <br />
    <center>
                <input id="btnEffectiveDateDone" sender="" class="StandardSaveButton" onclick='AppRateEffectiveDate.btnEffectiveDone();' type="button" value="OK" />
                    </center>
    <input id="hdnAppOriginalEffectiveDate" name="hdnAppOriginalEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppMinimumEffectiveDate" name="hdnAppMinimumEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppMaximumEffectiveDate" name="hdnAppMaximumEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppMinimumEffectiveDateAllQuotes" name="hdnAppMinimumEffectiveDateAllQuotes" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppMaximumEffectiveDateAllQuotes" name="hdnAppMaximumEffectiveDateAllQuotes" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppQuoteHasMinimumEffectiveDate" name="hdnAppQuoteHasMinimumEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes" name="hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppBeforeDateMsg" name="hdnAppBeforeDateMsg" type="hidden" ClientIDMode="Static" runat="server" />
    <input id="hdnAppAfterDateMsg" name="hdnAppAfterDateMsg" type="hidden" ClientIDMode="Static" runat="server" />
</div>
<div id="div_blanketAcreage_warning">
    <asp:Label runat="server" ID="lblBlanketAcreageWarning"></asp:Label>
    <br />
    <br />
    <center>
      <input id="btnblanketAcreageWarningOk" sender="" class="StandardSaveButton" onclick='AppRateEffectiveDate.btnBlanketAcreageOk();' type="button" value="OK" />
    </center>

      <input id="hdnBlanketAcreageAvailableDate" name="hdnBlanketAcreageAvailableDate" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnHasBlanketAcreage" name="hdnHasBlanketAcreage" type="hidden" ClientIDMode="Static" runat="server" />
</div>