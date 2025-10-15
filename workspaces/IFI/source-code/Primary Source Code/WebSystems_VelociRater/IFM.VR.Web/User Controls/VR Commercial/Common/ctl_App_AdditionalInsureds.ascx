<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_AdditionalInsureds.ascx.vb" Inherits="IFM.VR.Web.ctl_App_AdditionalInsureds" %>

<style>
    .gridBtn Input {
        /*min-width: 100px;*/
        border: 1px solid #797777;
        color: white;
        background-color: #797777;
        -moz-border-radius: 5px;
        -webkit-border-radius: 5px;
        border-radius: 5px;
        /*height: 25px;*/
}

    .GridAlternatingRowStyle{
        background-color: #DDDDDD
    }

</style>
<%--additional insureds--%>
<div id="AInsuredSection" runat="server">
<div id="aInsdHeader">
    <h3 runat="server">Additional Insureds?&nbsp;&nbsp;<span style="font-size: 12px">(Liability)</span>
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" runat="server" CssClass="RemovePanelLink" ToolTip="Save Data">Save</asp:LinkButton> <%--OnClientClick="return ValidateForm()"--%>
        </span>
    </h3>
    </div>
    <div id="aInsdBody">

    <table border="0" id="additionalInsuredsAREA" runat="server" visible="true" width="100%">
        <%--<tr>
            <td colspan="3">&nbsp;<%--space</td>
        </tr>--%>

        <tr id="addIdgROW" runat="server" visible="true">
            <td colspan="3" align="center">
                <asp:DataGrid ID="DataGrid_additionalInsured" runat="Server" Visible="true" AutoGenerateColumns="False"
                    Width="100%" CellPadding="3" ItemStyle-VerticalAlign="Top" BorderStyle="None" ItemStyle-Font-Size="10px" HeaderStyle-Font-Size="10px">
                    <HeaderStyle CssClass="qs_bop_section_grid_headers ui-widget-header" />
                    <AlternatingItemStyle CssClass="GridAlternatingRowStyle" />
                    <Columns>
                        <asp:BoundColumn DataField="Num" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="NumType" Visible="false" ItemStyle-BorderColor="White"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Type" HeaderText="Type" ItemStyle-Wrap="true" ItemStyle-BorderColor="White"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Name" HeaderText="Name" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="50px"></asp:BoundColumn>
                        <asp:BoundColumn DataField="DesigPrem" HeaderText="Premises" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="50px"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Desc" HeaderText="Description" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="104px"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Waiver" HeaderText="Waiver" ItemStyle-Wrap="true" ItemStyle-BorderColor="White" ItemStyle-Width="33px"></asp:BoundColumn>
                        <asp:ButtonColumn Text="Edit" CommandName="edit" ButtonType="PushButton" Visible="true" ItemStyle-BorderColor="White" ItemStyle-CssClass="gridBtn" ItemStyle-Width="18px"></asp:ButtonColumn>
                        <asp:ButtonColumn Text="Delete" CommandName="delete" ButtonType="PushButton" Visible="true" ItemStyle-BorderColor="White" ItemStyle-CssClass="gridBtn" ItemStyle-Width="30px"></asp:ButtonColumn>
                    </Columns>
                </asp:DataGrid>
            </td>
        </tr>
        <tr id="additionalInsuredAIsWithoutSchedule" runat="server" visible="true">
            <td colspan="3">
                <table>
                    <tr>
                        <td>
                            <label>
                                <input type="checkbox" runat="server" value="" id="chkTownhouseAssociations" />
                                Townhouse Associations</label></td>
                    </tr>
                    <tr>
                        <td>
                            <label>
                                <input type="checkbox" runat="server" value="" id="chkEngineersArchitectsOrSurveyors" />
                                Engineers, Architects, or Surveyors</label></td>
                    </tr>
                    <%--<tr runat="server" visible="true" id="trOwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherPartiesInConstructionContract">
                        <td>
                            <label>
                                <input type="checkbox" runat="server" disabled="disabled" value="false" id="chkOwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherPartiesInConstructionContract" />
                                Owners, Lessees or Contractors - With Additional Insured Requirement for other parties in construction contract</label></td>
                    </tr>--%>
                    <tr runat="server" visible="true" id="trOwnersLesseesOrContractorsAutomaticWithCompletedOpsandWaiver">
                        <td>
                            <label>
                                <input type="checkbox" runat="server" disabled="disabled" value="false" id="chkOwnersLesseesOrContractorsAutomaticWithCompletedOpsandWaiver" />
                                Owners, Lessees or Contractors - Automatic w/ Completed Ops and Waiver</label></td>
                    </tr>
                    <tr runat="server" visible="true" id="trWaiverOfSubrogationWhenRequiredByWrittenContractOrAgreement">
                        <td>
                            <label>
                                <input type="checkbox" runat="server" disabled="disabled" value="false" id="chkWaiverOfSubrogationWhenRequiredByWrittenContractOrAgreement" />
                                Waiver of Subrogation when Required by Written Contract or Agreement</label></td>
                    </tr>
                    <tr runat="server" visible="true" id="trOwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract">
                        <td>
                            <label>
                                <input type="checkbox" runat="server" disabled="disabled" value="false" id="chkOwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract" />
                                Owners, Lessees or Contractors - With Additional Insured Requirement of Construction Contract</label></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td>
        <table class="" border="0" id="EditArea" runat="server" visible="true" width="100%" style="background: white;">
        <tr>
            <td colspan="3" style="text-align: center;"><span id="spnAIStatus" runat="server" visible="false">
                <br />
                <br />
                <span id="spnAIStatusText" runat="server"></span></span></td>
        </tr>
        <tr id="additionalInsuredTYPErow" runat="server" visible="true" align="right" style="height: 50px;">
            <td colspan="2" align="right">*Additional Insured Type&nbsp;</td>
            <td colspan="2" align="left">
                <asp:DropDownList ID="DropDownList_additonalInsuredType" runat="server" Width="300" AutoPostBack="true">
                    <asp:ListItem Value="0">Select</asp:ListItem>
                    <asp:ListItem Value="21018">Co-Owner of Insured Premises</asp:ListItem>
                    <asp:ListItem Value="501">Controlling Interests</asp:ListItem>
                    <asp:ListItem Value="21022">Designated Person or Organization</asp:ListItem>
                    <asp:ListItem Value="21023">Engineers, Architects or Surveyors Not Engaged by the Named Insured</asp:ListItem>
                    <asp:ListItem Value="21144">Grantor of Franchise</asp:ListItem>
                    <asp:ListItem Value="21020">Lessor of Leased Equipment</asp:ListItem>
                    <asp:ListItem Value="21053">Managers or Lessors of Premises</asp:ListItem>
                    <asp:ListItem Value="21054">Mortgagee, Assignee Or Receiver</asp:ListItem>
                    <asp:ListItem Value="21055">Owner or Other Interests From Whom Land has been Leased</asp:ListItem>
                    <asp:ListItem Value="80368">Owners, Lessees or Contractors</asp:ListItem>
                    <%--<asp:ListItem Value="21081">Owners, Lessees or Contractors - Completed Operations</asp:ListItem>--%>
                    <asp:ListItem Value="21025">Owners, Lessees or Contractors - With Additional Insured Requirement in Construction Contract</asp:ListItem>
                    <asp:ListItem Value="21016">State or Political Subdivision - Permits Relating to Premises</asp:ListItem>
                    <asp:ListItem Value="21026">State or Political Subdivisions - Permits</asp:ListItem>
                    <asp:ListItem Value="21021">Vendors</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

        <tr id="addItypeROW" runat="server" visible="false">
            <td colspan="2" align="right">Additional Insured Type:
                <asp:Label ID="Label_addItype" runat="server" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr id="addInameROW" runat="server" visible="false">
            <td colspan="2" align="right">*<span id="spnName" runat="server">Name of Person or Organization</span></td>
            <td align="left">
                <asp:TextBox ID="TextBox_addInameOfOrg" runat="server" Rows="4" Width="200" TextMode="MultiLine" Wrap="true" MaxLength="245"></asp:TextBox></td>
        </tr>
        <tr id="addIpremisesROW" runat="server" visible="false">
            <td colspan="2" align="right"><span id="spnPremises" runat="server">Designation of Premises</span></td>
            <td align="left">
                <asp:TextBox ID="TextBox_addIdesignation" runat="server" Rows="4" Width="200" TextMode="MultiLine" Wrap="true" MaxLength="245"></asp:TextBox></td>
        </tr>
        <tr id="addIDescROW" runat="server" visible="false">
            <td colspan="2" align="right"><span id="spnDesc" runat="server">Description</span></td>
            <td align="left">
                <asp:TextBox ID="TextBox_addIdesc" runat="server" Rows="4" Width="200" TextMode="MultiLine" Wrap="true" MaxLength="245"></asp:TextBox></td>
        </tr>
        <tr id="addIwaiverROW" runat="server" visible="false">
            <td colspan="2" align="right"><span id="spnWaiver" runat="server">Waiver of Subrogation</span></td>
            <td align="left">
                <asp:CheckBox ID="CheckBox_addIwaiver" runat="server" /></td>
        </tr>
        
        <tr id="addIbuttonROW" runat="server" visible="false"><td colspan="3" style="text-align: center;"><br /><asp:Button ID="Button_addInsSaveAdd" runat="server" CssClass="roundedContainer StandardButton" Text="Save"></asp:Button>&nbsp;&nbsp;<asp:Button ID="Button_addInsCancel" runat="server" CssClass="roundedContainer StandardButton" Text="Cancel"></asp:Button></td></tr>
            </table></td>
        </tr>
        <tr id="ErrorMessageRow" runat="server" visible="">
            <td colspan="3" style="text-align: center;"><span id="ErrorMessageItem" runat="server" style="color:red"></span></td>
        </tr>

        

            
    </table>
</div>
</div>
<%--end additional insureds--%>