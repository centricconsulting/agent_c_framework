<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_Building_Information.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_Building_Information" %>
<%--<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProtectionClass_HOM.ascx" TagPrefix="uc1" TagName="ctlProtectionClass_HOM" %>--%>
<%--<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_Classifications.ascx" TagPrefix="uc1" TagName="ctlClassifications" %>--%>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_ClassificationsList.ascx" TagPrefix="uc1" TagName="ctlClassificationsList" %>

<div id="divMain" runat="server">
    <h3>Building Information
         <span style="float: right;">
<%--            <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add a Building">Add New</asp:LinkButton>--%>
            <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Building Information">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <style type="text/css">
            .LabelColumn {
                width:40%;
                text-align:right;
            }
            .DataColumn {
                width:60%;
                text-align:left;
            }
            .StdTextBoxField {
                width:90%;
            }
            .MediumTextBoxField {
                width:50%;
            }
            .SmallTextBoxField {
                width:25%;
            }
            .StdDDLField {
                width:95%;
            }
            .MediumDDLField {
                width:50%;
            }
            .SmallDDLField {
                width:25%;
            }
        </style>
        <table style="width:100%">
            <tr>
                <td class="LabelColumn"><label for="<%=Me.txtDescription.ClientID%>">Description</label></td>
                <td class="DataColumn">
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="StdTextBoxField"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn"><label for="<%=Me.ddlOccupancy.ClientID %>">*Occupancy</label></td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlOccupancy" runat="server" AutoPostBack="true" CssClass="StdDDLField">
                            <asp:ListItem></asp:ListItem>
<%--                            <asp:ListItem Value="16">Non-Owner Occupied Bldg / Lessor's</asp:ListItem>
                            <asp:ListItem Value="17">Owner Occupied Bldg 10% or Less / Lessor's</asp:ListItem>
                            <asp:ListItem Value="18">Owner Occupied Bldg More than 10% / Occupant</asp:ListItem>
                            <asp:ListItem Value="19">Tenant / Occupant</asp:ListItem>--%>
                            <asp:ListItem Value="16">Non-Owner Occupied Bldg / Lessor's</asp:ListItem>
                            <asp:ListItem Value="20">Owner Occupied Bldg 10% or Less / Lessor's</asp:ListItem>
                            <asp:ListItem Value="21">Owner Occupied Bldg More than 10% / Occupant</asp:ListItem>
                            <asp:ListItem Value="19">Tenant / Occupant</asp:ListItem>
                        </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn"><label for="<%=Me.ddlConstruction.ClientID %>">*Construction</label></td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlConstruction" runat="server" CssClass="StdDDLField">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="7">Frame</asp:ListItem>
                            <asp:ListItem Value="12">Joisted Masonry</asp:ListItem>
                            <asp:ListItem Value="13">Non-Combustible</asp:ListItem>
                            <asp:ListItem Value="14">Masonry Non-Combustible</asp:ListItem>
                            <asp:ListItem Value="15">Modified Fire Resistive</asp:ListItem>
                            <asp:ListItem Value="16">Fire Resistive</asp:ListItem>
                        </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn"><label for="<%=ddlAutomaticIncrease.ClientID%>">Automatic Increase</label></td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlAutomaticIncrease" runat="server"  CssClass="SmallDDLField">
                            <asp:ListItem Value="0">N/A</asp:ListItem>
                            <asp:ListItem Value="1">2</asp:ListItem>
                            <asp:ListItem Selected="True" Value="2">4</asp:ListItem>
                            <asp:ListItem Value="3">6</asp:ListItem>
                            <asp:ListItem Value="4">8</asp:ListItem>
                            <asp:ListItem Value="5">10</asp:ListItem>
                            <asp:ListItem Value="6">12</asp:ListItem>
                            <asp:ListItem Value="7">14</asp:ListItem>
                            <asp:ListItem Value="8">16</asp:ListItem>
                        </asp:DropDownList>
                </td>
            </tr> 
            <tr>
                <td class="LabelColumn">
                    <label for="<%=Me.ddlPropertyDeductible.ClientID %>">*Property Deductible</label></td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlPropertyDeductible" runat="server"  CssClass="SmallDDLField">
                        <%--<asp:ListItem Value="21">250</asp:ListItem>
                        <asp:ListItem Value="22" Selected="True">500</asp:ListItem>
                        <asp:ListItem Value="24">1000</asp:ListItem>
                        <asp:ListItem Value="75">2500</asp:ListItem>
                        <asp:ListItem Value="76">5000</asp:ListItem>
                        <asp:ListItem Value="333">7500</asp:ListItem>
                        <asp:ListItem Value="157">10000</asp:ListItem>--%>
                    </asp:DropDownList>
                </td>
            </tr>           
            <tr>
                <td class="LabelColumn">
                    <asp:Label ID="lblBuildingLimit" runat="server" AssociatedControlID="txtBuildingLimit" Text="Building Limit"></asp:Label>
                    <%--<label for="<%=Me.txtBuildingLimit.ClientID %>">*Building Limit</label>--%>
                </td>
                <td class="DataColumn">
                    <asp:TextBox ID="txtBuildingLimit" runat="server" CssClass="MediumTextBoxField" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn">
                    <label for="<%=Me.ddlBuildingValuation.ClientID%>">Building Valuation</label>
                </td>
                <td class="DataColumn">
                     <asp:DropDownList ID="ddlBuildingValuation" runat="server"  CssClass="StdDDLField">
                        <asp:ListItem Value="1">Replacement Cost</asp:ListItem>
                        <asp:ListItem Value="2">Actual Cash Value</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trACVRoofing" runat="server" style="display:none;" >
                <td class="LabelColumn">
                    <label for="<%=Me.chkACVRoofing.ClientID%>">ACV Roofing</label>
                </td>
                <td class="DataColumn">
                    <asp:CheckBox ID="chkACVRoofing" runat="server" Text="&nbsp;" />
                </td>
            </tr>
            <tr id="trLimitationsOnRoofSurfacing" runat="server" style="display:none;">
                <td class="LabelColumn">
                    <label for="<%=Me.chkLimitationsOnRoofSurfacing.ClientID%>">Limitations on Roof Surfacing</label>
                </td>
                <td class="DataColumn">
                    <asp:CheckBox ID="chkLimitationsOnRoofSurfacing" runat="server" Text="&nbsp;" />
                </td>
            </tr>
            <tr id="trRoofSettlementOptionsRow" runat="server" style="display:none;">
                <td class="LabelColumn">
                    <label for="<%=Me.ddRoofSettlementOptions.ClientID%>">Roof Settlement Options</label>
                </td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddRoofSettlementOptions" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn">
                    <label for="<%=Me.chkBuildingValuationIncludedInBlanketRating.ClientID %>">Included in Blanket Rating</label>
                </td>
                <td class="DataColumn">
                    <asp:CheckBox ID="chkBuildingValuationIncludedInBlanketRating" runat="server" />
                </td>
            </tr>
            <tr id="trMineSubsidenceRow" runat="server">
                <td class="LabelColumn">
                    <label for="<%=Me.chkMineSubsidence.ClientID %>">Mine Subsidence</label>
                </td>
                <td class="DataColumn">
                    <asp:CheckBox ID="chkMineSubsidence" runat="server" />
                </td>
            </tr>
            <tr id="trMineSubsidenceNumberOfUnitsRow" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'>
                <td style="text-indent:15px;">
                    Total Number of Units
                </td>
                <td>
                    <asp:TextBox ID="txtMineSubNumberOfUnits" runat="server" Width="30px"></asp:TextBox>
                </td>
            </tr>
            <tr id="trMineSubsidenceInfoForRequiredMineSubsidence_IL" runat="server" style="display:none;" class="informationalText">
                <td colspan="2">
                    Mine Subsidence is required for this location, if you would like to opt out please contact your underwriter.
                </td>
            </tr>
            <tr id="trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL" runat="server" style="display:none;" class="trMineInfo_IL_NotReqd informationalText">
                <td colspan="2">
                    Mine Subsidence must be selected for all IL locations if selected for any IL locations.  It has been added automatically to other IL locations.
                </td>
            </tr>
<%--            <tr id="trMineSubsidenceInfoNOT_OH" runat="server" style="display:none;" class="trMineInfo_OH_NotReqd informationalText">
                <td colspan="2">
                    Mine Subsidence must be selected for all OH locations if selected for any OH locations.  It has been added automatically to other OH locations.
                </td>
            </tr>--%>
            <tr id="trMineSubsidenceInfo_OH" runat="server" style="display:none;" class="informationalText" >
                <td colspan="2">
                    Mine Subsidence is required for this location.
                </td>
            </tr>
            <tr id="trMineSubsidenceLimitInfo" runat="server" style="display:none;" class="informationalText" >
                <td colspan="2">
                    Building limit over 300,000. Mine subsidence limit 300,000.
                </td>
            </tr>
            <tr>
                <td class="LabelColumn">
                    <label for="<%=Me.chkSprinklered.ClientID %>">Sprinklered</label>
                </td>
                <td class="DataColumn">
                    <asp:CheckBox ID="chkSprinklered" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="LabelColumn">
                    <asp:Label ID="lblPersonalPropertyLimit" runat="server" AssociatedControlID="txtPersonalPropertyLimit" Text="Personal Property Limit"></asp:Label>
                    <%--<label for="<%=Me.txtPersonalPropertyLimit.ClientID %>">*Personal Property Limit</label>--%>
                </td>
                <td class="DataColumn">
                    <asp:TextBox ID="txtPersonalPropertyLimit" runat="server"  CssClass="MediumTextBoxField" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn">
                    <label for="<%=Me.ddlValuationMethod.ClientID%>">*Valuation Method</label>
                </td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlValuationMethod" runat="server"  CssClass="StdDDLField" >
                        <asp:ListItem Value="1">Replacement Cost</asp:ListItem>
                        <asp:ListItem Value="2">Actual Cash Value</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="LabelColumn">
                    <label for="<%=Me.chkValuationMethodIncludedInBlanketRating.ClientID %>">Included in Blanket Rating</label>
                </td>
                <td class="DataColumn">
                    <asp:CheckBox ID="chkValuationMethodIncludedInBlanketRating" runat="server" />
                </td>
            </tr>
            <tr id="rowFeetToHydrant" runat="server">
                <td class="LabelColumn">
                    <label for="<%=Me.txtFeetToHydrant.ClientID %>">Feet to Hydrant</label>
                </td>
                <td class="DataColumn">
                    <asp:TextBox ID="txtFeetToHydrant" runat="server"  CssClass="SmallTextBoxField" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="rowMilesToFireDepartment" runat="server">
                <td class="LabelColumn">
                    <label for="<%=Me.txtMilesToFireDepartment.ClientID %>">Miles to Fire Department</label>
                </td>
                <td class="DataColumn">
                    <asp:TextBox ID="txtMilesToFireDepartment" runat="server"  CssClass="SmallTextBoxField" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="rowProtectionClass" runat="server">
                <td class="LabelColumn">
                    <label for="<%=Me.ddlProtectionClass.ClientID %>">*Protection Class</label>
                </td>
                <td class="DataColumn">
                    <asp:DropDownList ID="ddlProtectionClass" runat="server"  CssClass="StdDDLField">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>

        <uc1:ctlClassificationsList ID="ctlClassificationsList" runat="server"></uc1:ctlClassificationsList>
        <%--<uc1:ctlProtectionClass_HOM runat="server" ID="ctlProtectionClass_HOM" />--%>

        <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
    
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>