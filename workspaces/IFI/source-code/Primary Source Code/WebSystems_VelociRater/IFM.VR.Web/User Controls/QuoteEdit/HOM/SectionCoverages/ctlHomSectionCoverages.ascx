<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomSectionCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlHomSectionCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/ctlSectionCoverageItem.ascx" TagPrefix="uc1" TagName="ctlSectionCoverageItem" %>

<script>
    // this un-disables checkboxes.. disabled checkboxes do not send their values back to the server
    // so even if the disabled checkbox is checked the server will assume it is unchecked because it doesn't get a value for that control
    $(document).ready(function () {

        $("form").submit(function () {
            $("input:checkbox").removeAttr("disabled");

        });
    });
</script>

<div runat="server" id="divMain">
    <h3>
        Optional Coverages <label id="lblHomOptionCoverageHeader">(N)</label>
         <span style="float: right;">
            <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Inland Marine">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="divOptionalCoveragesContent">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <%--<uc1:ctlSectionCoverageItem SectionCoverageIEnum="<%#Container.DataItem%>" runat="server" ID="ctlSectionCoverageItem" />--%>
                    <uc1:ctlSectionCoverageItem runat="server" ID="ctlSectionCoverageItem" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <hr />
        <asp:LinkButton ID="lnkMoreLess" style="color:blue !important" runat="server">+More</asp:LinkButton>   
        <asp:HiddenField ID="hdnMoreLess" Value="0" runat="server" />     
        <div id="divMoreCoverages" runat="server">
            <div runat="server" id="divSectionI">
                <h3>Section I Coverages</h3>
            <asp:Repeater ID="Repeater2" runat="server">
                <ItemTemplate>
                    <uc1:ctlSectionCoverageItem SectionCoverageIEnum="<%#Container.DataItem%>" runat="server" ID="ctlSectionCoverageItem" />
                </ItemTemplate>
            </asp:Repeater>
            </div>            

            <div runat="server" id="divNASectionII">
                <h3>Section II Coverages</h3>
                <p>N/A</p>
            </div>
            <div runat="server" id="divSectionII">
                <h3>Section II Coverages</h3>
            <asp:Repeater ID="Repeater3" runat="server">
                <ItemTemplate>
                    <uc1:ctlSectionCoverageItem SectionCoverageIIEnum="<%#Container.DataItem%>" runat="server" ID="ctlSectionCoverageItem" />
                </ItemTemplate>
            </asp:Repeater>
            </div>
            
            <div runat="server" id="divNASectionIandII">
                <h3>Section I and II Coverages</h3>
                <p>N/A</p>
            </div>
            <div runat="server" id="divSectionIandII">
                <h3>Section I and II Coverages</h3>
            <asp:Repeater ID="Repeater4" runat="server">
                <ItemTemplate>
                    <uc1:ctlSectionCoverageItem SectionCoverageIAndIIEnum="<%#Container.DataItem%>" runat="server" ID="ctlSectionCoverageItem" />
                </ItemTemplate>
            </asp:Repeater>
            </div>
            

        </div>
        <div align="center" runat="server" id="divSaveRateButtons">
            <br />
            <asp:Button ID="btnSave" runat="server" Text="Save Optional Coverages" CssClass="StandardSaveButton" Style="height: 26px" />&nbsp;
            <asp:Button ID="btnRate" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" Style="height: 26px" />
            <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" Style="height: 26px"/>
            <asp:Button ID="btnViewGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Billing Information Page" Style="height: 26px" />                     
            <div runat="server" id="divWhatsNewLink" style="display:inline;text-align:right;padding-left:50px;">
                <asp:LinkButton ID="lnkWhatsNew" runat="server" OnClientClick="return false;">What's New?</asp:LinkButton><!--Added 1/10/18 for HOM Upgrade MLW-->
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>
