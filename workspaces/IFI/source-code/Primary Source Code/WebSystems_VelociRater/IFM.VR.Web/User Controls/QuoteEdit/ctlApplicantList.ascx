<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlApplicantList.ascx.vb" Inherits="IFM.VR.Web.ctlApplicantList" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicant.ascx" TagPrefix="uc1" TagName="ctlApplicant" %>

<div>
    <div id="divApplicantListAccord" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlApplicant runat="server" ID="ctlApplicant" />
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <div id="divActionButtons" runat="server" style="margin-top: 20px; width: 100%; text-align: center;">
        <div style="float: left;">
            <asp:Button ID="bnAddApplicant" ToolTip="Add an Applicant" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Add Applicant" />
        </div>

        <asp:Button ID="btnSubmit" ToolTip="Save all Applicant Information" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Save Applicants" />
        <asp:Button ID="btnSaveAndGotoNextPage" ToolTip="Save all Applicant Information and Navigate to the next page" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Next Page" />
    </div>
</div>
<asp:HiddenField ID="hiddenActiveAccordIndex" runat="server" />