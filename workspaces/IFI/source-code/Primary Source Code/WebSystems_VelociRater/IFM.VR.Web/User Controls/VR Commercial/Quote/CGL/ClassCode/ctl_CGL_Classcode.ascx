<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_Classcode.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_Classcode" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Class Code"></asp:Label>

    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Class Code" CssClass="RemovePanelLink">Add Class Code</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" ToolTip="Clear Class Code" CssClass="RemovePanelLink">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Delete this Class code" runat="server">Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div>
    <div runat="server" id="divContents">
        <style>
            .CGL_CC_LeftColumn {
                width:50%;
            }
            .CGL_CC_RightColumn {
                width:50%;
            }
        </style>
        <asp:HiddenField ID="hdnClassCode" runat="server" />
        <asp:HiddenField ID="hdnCheckAratePrem" runat="server" />
        <asp:HiddenField ID="hdnCheckArateProd" runat="server" />
        <asp:HiddenField ID="hdnRemoveEPLI" runat="server" />
        <asp:HiddenField ID="hdnStateId" runat="server" />

        <%-- Gasoline Sales dialog --%>
        <div id="dialog" title="Gasoline Sales" style="display:none;">
            <p>
                To write Gasoline sales you must meet all the underwriting guidelines:<br />
                1. Copy of the Dec for separate pollution policy in force with a $1 million limit. <br />
                2. For operations over 15 years old, proof of replaced tanks.<br />
                3. Proof of release detection equipment and the frequency of its testing.<br />
                4. Documentation for regular Inspection of tanks, piping, fittings, and related equipment.
            </p>
        </div>

        <%-- EPLI dialog --%>
        <div id="dialog_ccCheck" title="EPLI Not Allowed" style="display:none;">
            <p>
                EPLI Not allowed for this Class Code, endorsement will be removed from quote.
            </p>
        </div>

        <div runat="server" id="divSearch" style="margin-bottom: 20px;">
            <table style="width:100%;">
                <tr>
                    <td style="width:30%;">
                        <label for="<%=ddSearchType.ClientID%>">Search Type</label>
                    </td>
                    <td style="width:30%;">
                        <label for="<%=ddState.ClientID%>">State</label>
                    </td>
                    <td style="width:30%;">
                        <label for="<%=txtSearchClassCode.ClientID%>">Search Value</label>
                    </td>
                </tr>
                <tr>
                    <td style="width:30%;">
                        <asp:DropDownList ID="ddSearchType" runat="server" Width="100%">
                            <asp:ListItem Value="0">Description</asp:ListItem>
                            <asp:ListItem Value="1">Description Contains</asp:ListItem>
                            <asp:ListItem Value="2">Class Code</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td style="width:30%;">
                        <asp:DropDownList ID="ddState" runat="server" Width="100%"></asp:DropDownList>
                    </td>
                    <td style="width:30%;">
                        <asp:TextBox ID="txtSearchClassCode" runat="server" Width="65%"></asp:TextBox>
                        <asp:Button ID="btnSearch" runat="server" Text="Find" Width="25%"/>
                    </td>
                </tr>
            </table>

            <div id="divSearchResults" runat="server">
                <table>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="divClassInfo" runat="server">
            <table id="tblClassInfo" runat="server" style="width: 100%">
                <tr>
                    <td colspan="2">
                        Class Code
                        <br />
                        <asp:TextBox ID="txtClassCode" runat="server" Width="15%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="vertical-align: bottom;">
                        Description
                        <br />
                        <asp:TextBox ID="txtClassCodeDescription" Width="100%" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="CGL_CC_LeftColumn">
                        *Class Code Assignment
                        <br />
                        <asp:DropDownList ID="ddAssignment" runat="server" Width="50%">
                            <asp:ListItem Value="1">Policy</asp:ListItem>
                            <asp:ListItem Value="2">Location</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="CGL_CC_RightColumn">
                        <div id="divLoc" runat="server">
                            Location
                            <br />
                            <asp:DropDownList ID="ddAssignmentLocation" runat="server" Width="100%"></asp:DropDownList>
                        </div>
                    </td>
                </tr>
                <tr id="trAssignmentInfoRow" runat="server" style="display:none;">
                    <td colspan="2" class="informationalText">
                        *Locations must be saved before they can be assigned in the Class Code section.
                    </td>
                </tr>
                <tr>
                    <td class="CGL_CC_LeftColumn"><label for="<%=txtExposure.ClientID%>">*Premium Exposure</label>
                    <br />
                        <asp:TextBox ID="txtExposure" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        <br />
                        <%--<asp:Label ID="lblpremBaseShort" runat="server" Visible="False"></asp:Label>--%>
                        <asp:TextBox ID="lblpremBaseShort" runat="server" style="display:none;"></asp:TextBox>
                    </td>
                    <td class="CGL_CC_RightColumn" style="vertical-align: top;">
                        Premium Exposure Description
                        <br />
                        <asp:TextBox ID="txtPremiumDescription" Width="100%" runat="server" ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr style="display:none;">
                    <td colspan="2" style="vertical-align: top;">
                        Class Code Basis
                        <br />
                        <asp:TextBox ID="txtBasis" Width="100%" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trAratingInfoRow" runat="server" style="display:none">
                    <td colspan="2">
                        <span class="informationalText">This class requires a manual rate. Contact the underwriter for the appropriate A-Rates. Risks with manual rates cannot be quoted or bound without Underwriter approval.</span>
                    </td>
                </tr>
                <tr id="trAratingRow" runat="server">
                    <td id="tdAratePremColumn" runat="server" style="text-align: right; padding-right:20px;">
                        <label for="<%=txtARatePrem.ClientID%>">*Premises/Ops Manual Rate</label>
                        <br />
                        <asp:TextBox ID="txtARatePrem" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46'></asp:TextBox>
                    </td>
                    <td id="tdArateProdColumn" runat="server" style="padding-left:20px;">
                        <label for="<%=txtARateProd.ClientID%>">*Product/Comp Manual Rate</label>
                        <br />
                        <asp:TextBox ID="txtARateProd" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46'></asp:TextBox>
                    </td>
               </tr>                    
                <tr>
                    <td colspan="2">FootNote
                        <br />
                        <div id="divFootnote" runat="server" style="height:200px;width:102%;background-color:white;border:1px solid black;"></div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
         
</div>
