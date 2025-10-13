<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FUPPUP_MiscPolicyInfo.ascx.vb" Inherits="IFM.VR.Web.ctl_FUPPUP_MiscPolicyInfo" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/FamFarmCorpControls/ctlFamFarmCorpList.ascx" TagPrefix="uc1" TagName="ctlFamFarmCorpList" %>

<asp:Panel ID="pnlMiscLiab" runat="server">
    <style type="text/css">
        .TableLeftColumn {
            /*width: 49%;*/
            display: inline-block;
            padding: 10px 20px;
        }

        .TableRightColumn {
            /*width: 49%;*/
            display: inline-block;
            padding: 10px 20px;
        }
    </style>
        <hr />

    <div style="text-align: center;">
        Please select any of the following miscellaneous liabilities that apply to this quote. 

        <div id="dvSwimmingPool" runat="server" class="TableLeftColumn">
            <asp:CheckBox ID="chkSwimmingPool" runat="server" />&nbsp;
            <asp:Label ID="lblSwimmingPool" runat="server" Text="Swimming Pool"></asp:Label>
        </div>

        <%--<div id="dvAddInsurFamFarmCorp" runat="server" class="TableRightColumn">
            <asp:CheckBox ID="chkAddInsurFamFarmCorp" runat="server" />&nbsp;
            <asp:LinkButton ID="lblAddInsurFamFarmCorp" runat="server" Text="Additional Insured Family Farm Corp"></asp:LinkButton>
        </div>

        <div id="dvAFCNumPer" runat="server" style="display: none">
            <table style="width: 100%">
                <tr>
                    <td></td>
                    <td>
                        <asp:Label ID="lblAFCNumPer" runat="server" CssClass="inlineBlk" Text="How many Additional Insured Family Farm Corp:"></asp:Label>
                        <asp:TextBox ID="txtAFCNumPer" runat="server" CssClass="inlineBlk"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>--%>

        <div id="dvAddInsurFamFarmCorp" runat="server">
            <asp:CheckBox ID="chkAddInsurFamFarmCorp" runat="server" AutoPostBack="true" OnClientClick="return confirm('Delete?');"/>&nbsp;
            <asp:LinkButton ID="lblAddInsurFamFarmCorp" runat="server" Text="Additional Insured Family Farm Corp"></asp:LinkButton>
            <div id="dvAFCNumPer" runat="server" style="display: none" class="div">
                <uc1:ctlFamFarmCorpList runat="server" id="ctlFamFarmCorpList" />
                
                            
            </div>
        </div>

    </div>
    <%--FamFarmCorp Popup--%>
<div id="dvFFCPopup" style="display: none">
    <div>
        <div style="text-align:center">Add Additional Insured when the name of the Additional Insured is Inc, Corp or LLC</div>
        <br />
        GL-108 Additional Insured - Commercial
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach GL-108 to show the farmowner policy includes commercial liability coverage to cover the premises-related liability exposures of co-owners, controlling interests, mortgagees, assignees, and receivers. Used with Commercial Farm Umbrella Policies.
                </td>
            </tr>
        </table>
        <br />
        GL-70 Additional Insured - Persons or Organizations
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach GL-70 to show any corporation or any other entities, other than the Named Insured, as additional insureds in order to provide them with premises liability coverage for the farming operations. Used with Personal Farm Umbrella Policies.
                </td>
            </tr>
        </table>
        <br />
        GL-71 Additional Insured - Partners, Corporate Officers or Co-Owners
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>Attach GL-71 to provide liability coverage for the personal activities outside the farming operations for corporate officers and stockholders who are not residents of the Named Insured's household. Used with Personal Farm Umbrella Policies.
                </td>
            </tr>
        </table>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnFFCOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

</asp:Panel>
