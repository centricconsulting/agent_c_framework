<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlResidence.ascx.vb" Inherits="IFM.VR.Web.ctlResidence" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/LocationControls/ctlResidenceCoverages.ascx" TagPrefix="uc1" TagName="ctlResidenceCoverages" %>

<div id="ResidenceDiv" runat="server" class="standardSubSection">
    <h3>
        <asp:Label ID="lblResidence" runat="server" Text=" Residence"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkDeleteResidence" runat="server" OnClick="OnConfirm" OnClientClick="ConfirmDialog('Residence')" ToolTip="Delete Residence" CssClass="RemovePanelLink" Visible="false">Delete</asp:LinkButton>
            <asp:LinkButton ID="lnkClearResidence" runat="server" ToolTip="Clear Residence" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveResidence" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="ResidenceContentDiv">
        <center>
            <div id="divDwellingAgeText" class="informationalText" style="display: none; font-size: 12px;" runat="server">
                This residence is 75 years and older. Policyholders will receive an email invitation to complete a required inspection.
            </div>
        </center>
        <table style="width: 100%">
            <tr>
                <td>
                    <table style="width: 100%" id="tblResidence">
                        <tr runat="server" id="trCoverageForm">
                            <td>
                                <label for="<%=dd_Residence_CoverageForm.ClientID%>">*Coverage Form</label>
                                <br />
                                <asp:DropDownList ID="dd_Residence_CoverageForm" runat="server" Width="90px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblYearBuilt" runat="server" Text="*Year Built"></asp:Label><br />
                                <asp:TextBox ID="txtYearBuilt" MaxLength="4" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblSqrFeet" AssociatedControlID="txtSqrFeet" runat="server" Text="*Square Feet"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtSqrFeet" MaxLength="7" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.ddlNumberOfFamilies.ClientID%>">*Number of Families</label><br />
                                <asp:DropDownList ID="ddlNumberOfFamilies" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trStructureLeft">
                            <td>
                                <label for="<%=Me.ddlStructureLeft.ClientID%>">*Structure</label><br />
                                <asp:DropDownList ID="ddlStructureLeft" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="width: 100%" id="tblResidenceRight">
                        <tr runat="server" id="trStructureRight">
                            <td>
                                <label for="<%=Me.ddlStructure.ClientID%>">*Structure</label><br />
                                <asp:DropDownList ID="ddlStructure" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.ddlOccupancy.ClientID%>">*Occupancy</label>
                                <br />
                                <asp:DropDownList ID="ddlOccupancy" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trRelatedPolicyNumber">
                            <td>
                                <label for="<%=Me.txtRelatedPolicyNumber.ClientID%>">*Primary Homeowner Policy Number</label>
                                <br />
                                <asp:TextBox ID="txtRelatedPolicyNumber" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblConstruction" AssociatedControlID="ddlConstruction" runat="server" Text="*Construction"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlConstruction" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trStyle">
                            <td id="tdStyle" runat="server">
                                <label for="<%=Me.ddStyle.ClientID%>">*Style</label><br />
                                <asp:DropDownList ID="ddStyle" Width="150px" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trUnitsInFireDivision">
                            <td>
                                <label for="<%=Me.ddlUnitsInFireDivision.ClientID%>">*Units in Fire Division</label><br />
                                <asp:DropDownList ID="ddlUnitsInFireDivision" Width="150px" runat="server">
                                    <asp:ListItem Value=""></asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                    <asp:ListItem Value="4">4</asp:ListItem>
                                    <asp:ListItem Value="5">5</asp:ListItem>
                                    <asp:ListItem Value="6">6</asp:ListItem>
                                    <asp:ListItem Value="7">7</asp:ListItem>
                                    <asp:ListItem Value="8">8</asp:ListItem>
                                    <asp:ListItem Value="9">9+</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trDwellingClass">
                            <td>
                                <asp:HyperLink ID="lnkFarDwellingTypeClassification" Target="_blank" runat="server">*Dwelling Classification</asp:HyperLink>
                                <br />
                                <asp:DropDownList ID="ddDwellingClass" Width="150px" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        
                        <tr runat="server" id="trUsageType">
                            <td>
                                <label for="<%=ddlUsageType.ClientID%>">*Usage Type</label>
                                <br />
                                <asp:DropDownList ID="ddlUsageType" Width="150px" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table id="tblResidenceCoverage" runat="server" style="width: 100%">
            <tr>
                <td>
                    <uc1:ctlResidenceCoverages runat="server" ID="ctlResidenceCoverages" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:HiddenField ID="OriginalOccCode" runat="server" />
    </div>
</div>