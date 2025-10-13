<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EndorsementTestingApp.aspx.vb" Inherits="EndorsementTestingApp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Endorsement Testing</title>
    <script type="text/javascript" src="js/CommonFunctions.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p class="normaltext" align="center"><a href="EndorsementTesting.aspx">Reload Page</a></p>
        <br />
        <p class="normaltext" align="center" runat="server" id="AgentsLinkSection" visible="false"><a runat="server" id="AgentsLink">Agents Only Site</a></p>
        <br />
        <p class="normaltext" align="center">
            <asp:DropDownList runat="server" ID="ddlAction" AutoPostBack="true"><%-- CssClass="normaltext"--%>
                <asp:ListItem>--Select Action--</asp:ListItem>
                <asp:ListItem>Endorsement Search</asp:ListItem>
                <asp:ListItem>Policy Search</asp:ListItem>
            </asp:DropDownList>
        </p>
        <br />
        <br />        
        <asp:Panel ID="pnlEndorsementSearch" runat="server" Visible="false" CssClass="normaltext">
            <br />
            <table width="600px" align="center">
                <tr>
                    <td align="left">
                        Search By: 
                        <asp:DropDownList runat="server" ID="ddlEndorsementSearchBy">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem>Policy Number</asp:ListItem>
                            <asp:ListItem>Quote Number</asp:ListItem>
                            <asp:ListItem>Policy Id</asp:ListItem>
                            <asp:ListItem>Display Name (Exact Match)</asp:ListItem>
                            <asp:ListItem>Display Name (Match Beginning)</asp:ListItem>
                            <asp:ListItem>Display Name (Match End)</asp:ListItem>
                            <asp:ListItem>Display Name (Match Middle)</asp:ListItem>
                            <asp:ListItem>Last Name (Exact Match)</asp:ListItem>
                            <asp:ListItem>Last Name (Match Beginning)</asp:ListItem>
                            <asp:ListItem>Last Name (Match End)</asp:ListItem>
                            <asp:ListItem>Last Name (Match Middle)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Exact Match)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Match Beginning)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Match End)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Match Middle)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="left"><asp:TextBox runat="server" ID="txtEndorsementSearchFor"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="left">Policy or Image Results</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlEndorsementSearchByPolicyOrImage">
                            <asp:ListItem>By Image</asp:ListItem>
                            <asp:ListItem>By Policy</asp:ListItem>                            
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td align="left">Filter by Lob Id?</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlEndorsementSearchByLobId">
                            <asp:ListItem Value="-1">All (-1)</asp:ListItem>
                            <asp:ListItem Value="0">Not Assigned (0)</asp:ListItem>
                            <asp:ListItem Value="1">Auto Personal (1)</asp:ListItem>
                            <asp:ListItem Value="2">Home Personal (2)</asp:ListItem>
                            <asp:ListItem Value="3">Dwelling Fire Personal (3)</asp:ListItem>
                            <asp:ListItem Value="4">Watercraft Personal (4)</asp:ListItem>
                            <asp:ListItem Value="6">Specialty Auto (6)</asp:ListItem>
                            <asp:ListItem Value="7">Personal Snowmobile (7)</asp:ListItem>
                            <asp:ListItem Value="8">Mobile Home (8)</asp:ListItem>
                            <asp:ListItem Value="9">Commercial General Liability (9)</asp:ListItem>
                            <asp:ListItem Value="10">Commercial Directors and Officers (10)</asp:ListItem>
                            <asp:ListItem Value="11">Commercial General Partner (11)</asp:ListItem>
                            <asp:ListItem Value="12">Commercial Combined Lines (12)</asp:ListItem>
                            <asp:ListItem Value="13">Specialty Home (13)</asp:ListItem>
                            <asp:ListItem Value="14">Umbrella Personal (14)</asp:ListItem>
                            <asp:ListItem Value="15">Commercial NonTraditional (15)</asp:ListItem>
                            <asp:ListItem Value="16">Inland Marine Personal (16)</asp:ListItem>
                            <asp:ListItem Value="17">Farm (17)</asp:ListItem>
                            <asp:ListItem Value="18">Personal Condominium (18)</asp:ListItem>
                            <asp:ListItem Value="19">Personal Tenant (19)</asp:ListItem>
                            <asp:ListItem Value="20">Commercial Auto (20)</asp:ListItem>
                            <asp:ListItem Value="21">Workers Comp (21)</asp:ListItem>
                            <asp:ListItem Value="22">Complete Auto (22)</asp:ListItem>
                            <asp:ListItem Value="23">Commercial Package (23)</asp:ListItem>
                            <asp:ListItem Value="24">Commercial Garage (24)</asp:ListItem>
                            <asp:ListItem Value="25">Commercial BOP (25)</asp:ListItem>
                            <asp:ListItem Value="26">Commercial Crime (26)</asp:ListItem>
                            <asp:ListItem Value="27">Commercial Umbrella (27)</asp:ListItem>
                            <asp:ListItem Value="28">Commercial Property (28)</asp:ListItem>
                            <asp:ListItem Value="29">Commercial Inland Marine (29)</asp:ListItem>
                            <asp:ListItem Value="30">Home (30)</asp:ListItem>
                            <asp:ListItem Value="31">Personal Auto (31)</asp:ListItem>
                            <asp:ListItem Value="32">Farm (32)</asp:ListItem>
                            <asp:ListItem Value="33">Dwelling Fire (33)</asp:ListItem>
                            <asp:ListItem Value="34">Workers Comp (34)</asp:ListItem>
                            <asp:ListItem Value="35">Automobile Liability (35)</asp:ListItem>
                            <asp:ListItem Value="36">Garage Owners Liability (36)</asp:ListItem>
                            <asp:ListItem Value="37">Business Owners Liability (37)</asp:ListItem>
                            <asp:ListItem Value="38">Professional Liability (38)</asp:ListItem>
                            <asp:ListItem Value="39">Workers Comp Liability (39)</asp:ListItem>
                            <asp:ListItem Value="40">Commercial Farm Liability (40)</asp:ListItem>
                            <asp:ListItem Value="41">Commercial Auto (41)</asp:ListItem>
                            <asp:ListItem Value="42">General Liability (42)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="EndorsementSearchAgencyCodeWarningRow" visible="false">
                    <td colspan="2" align="center">*Warning: searching all agency codes for staff could result in a database timeout.</td>
                </tr>
                <tr runat="server" id="EndorsementSearchAgencyCodeRow">
                    <td align="left">Agency Codes to Search</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlEndorsementSearchAgenciesToUse">
                            <asp:ListItem>Just my primary agency code</asp:ListItem>
                            <asp:ListItem>Just my primary and secondary codes</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <%--<tr runat="server" id="EndorsementSearchAgencyCodeSelectionRow" visible="false">
                    <td align="left">Select Agency Code</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlEndorsementSearchAgencyCodeSelection"></asp:DropDownList>
                    </td>
                </tr>--%>
                <tr runat="server" id="EndorsementSearchAgencyCodeSelectionRow" visible="false">
                    <td colspan="2" align="center">
                        Select Agency Code<br />
                        <asp:DropDownList runat="server" ID="ddlEndorsementSearchAgencyCodeSelection"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center"><asp:CheckBox runat="server" ID="cbEndorsementSearchReturnPH1Name" Text="Force Return of PH1 Name?" /></td>
                </tr>
                <tr>
                    <td colspan="2" align="center"><asp:Button runat="server" ID="btnEndorsementSearch" Text="Search For Endorsement" /></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlPolicySearch" runat="server" Visible="false" CssClass="normaltext">
            <br />
            <table width="600px" align="center">
                <%--<tr>
                    <td colspan="2" align="center"style="font-weight:bold; font-style:italic;">Current Info</td>
                </tr>--%>
                <tr>
                    <td align="left">
                        Search By: 
                        <asp:DropDownList runat="server" ID="ddlPolicySearchBy">
                            <asp:ListItem>Policy Number</asp:ListItem>
                            <asp:ListItem>Quote Number</asp:ListItem>
                            <asp:ListItem>Policy Id</asp:ListItem>
                            <asp:ListItem>Display Name (Exact Match)</asp:ListItem>
                            <asp:ListItem>Display Name (Match Beginning)</asp:ListItem>
                            <asp:ListItem>Display Name (Match End)</asp:ListItem>
                            <asp:ListItem>Display Name (Match Middle)</asp:ListItem>
                            <asp:ListItem>Last Name (Exact Match)</asp:ListItem>
                            <asp:ListItem>Last Name (Match Beginning)</asp:ListItem>
                            <asp:ListItem>Last Name (Match End)</asp:ListItem>
                            <asp:ListItem>Last Name (Match Middle)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Exact Match)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Match Beginning)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Match End)</asp:ListItem>
                            <asp:ListItem>Commercial Name 1 (Match Middle)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="left"><asp:TextBox runat="server" ID="txtPolicySearchFor"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="left">Policy or Image Results</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlPolicySearchByPolicyOrImage">
                            <asp:ListItem>By Policy</asp:ListItem>
                            <asp:ListItem>By Image</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td align="left">Filter by Lob Id?</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlPolicySearchByLobId">
                            <asp:ListItem Value="-1">All (-1)</asp:ListItem>
                            <asp:ListItem Value="0">Not Assigned (0)</asp:ListItem>
                            <asp:ListItem Value="1">Auto Personal (1)</asp:ListItem>
                            <asp:ListItem Value="2">Home Personal (2)</asp:ListItem>
                            <asp:ListItem Value="3">Dwelling Fire Personal (3)</asp:ListItem>
                            <asp:ListItem Value="4">Watercraft Personal (4)</asp:ListItem>
                            <asp:ListItem Value="6">Specialty Auto (6)</asp:ListItem>
                            <asp:ListItem Value="7">Personal Snowmobile (7)</asp:ListItem>
                            <asp:ListItem Value="8">Mobile Home (8)</asp:ListItem>
                            <asp:ListItem Value="9">Commercial General Liability (9)</asp:ListItem>
                            <asp:ListItem Value="10">Commercial Directors and Officers (10)</asp:ListItem>
                            <asp:ListItem Value="11">Commercial General Partner (11)</asp:ListItem>
                            <asp:ListItem Value="12">Commercial Combined Lines (12)</asp:ListItem>
                            <asp:ListItem Value="13">Specialty Home (13)</asp:ListItem>
                            <asp:ListItem Value="14">Umbrella Personal (14)</asp:ListItem>
                            <asp:ListItem Value="15">Commercial NonTraditional (15)</asp:ListItem>
                            <asp:ListItem Value="16">Inland Marine Personal (16)</asp:ListItem>
                            <asp:ListItem Value="17">Farm (17)</asp:ListItem>
                            <asp:ListItem Value="18">Personal Condominium (18)</asp:ListItem>
                            <asp:ListItem Value="19">Personal Tenant (19)</asp:ListItem>
                            <asp:ListItem Value="20">Commercial Auto (20)</asp:ListItem>
                            <asp:ListItem Value="21">Workers Comp (21)</asp:ListItem>
                            <asp:ListItem Value="22">Complete Auto (22)</asp:ListItem>
                            <asp:ListItem Value="23">Commercial Package (23)</asp:ListItem>
                            <asp:ListItem Value="24">Commercial Garage (24)</asp:ListItem>
                            <asp:ListItem Value="25">Commercial BOP (25)</asp:ListItem>
                            <asp:ListItem Value="26">Commercial Crime (26)</asp:ListItem>
                            <asp:ListItem Value="27">Commercial Umbrella (27)</asp:ListItem>
                            <asp:ListItem Value="28">Commercial Property (28)</asp:ListItem>
                            <asp:ListItem Value="29">Commercial Inland Marine (29)</asp:ListItem>
                            <asp:ListItem Value="30">Home (30)</asp:ListItem>
                            <asp:ListItem Value="31">Personal Auto (31)</asp:ListItem>
                            <asp:ListItem Value="32">Farm (32)</asp:ListItem>
                            <asp:ListItem Value="33">Dwelling Fire (33)</asp:ListItem>
                            <asp:ListItem Value="34">Workers Comp (34)</asp:ListItem>
                            <asp:ListItem Value="35">Automobile Liability (35)</asp:ListItem>
                            <asp:ListItem Value="36">Garage Owners Liability (36)</asp:ListItem>
                            <asp:ListItem Value="37">Business Owners Liability (37)</asp:ListItem>
                            <asp:ListItem Value="38">Professional Liability (38)</asp:ListItem>
                            <asp:ListItem Value="39">Workers Comp Liability (39)</asp:ListItem>
                            <asp:ListItem Value="40">Commercial Farm Liability (40)</asp:ListItem>
                            <asp:ListItem Value="41">Commercial Auto (41)</asp:ListItem>
                            <asp:ListItem Value="42">General Liability (42)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="PolicySearchAgencyCodeWarningRow" visible="false">
                    <td colspan="2" align="center">*Warning: searching all agency codes for staff could result in a database timeout.</td>
                </tr>
                <tr runat="server" id="PolicySearchAgencyCodeRow">
                    <td align="left">Agency Codes to Search</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlPolicySearchAgenciesToUse">                            
                            <asp:ListItem>Just my primary agency code</asp:ListItem>
                            <asp:ListItem>Just my primary and secondary codes</asp:ListItem>
                            <%--<asp:ListItem>All Codes (for testing validation)</asp:ListItem>--%>
                        </asp:DropDownList>
                    </td>
                </tr>
                <%--<tr runat="server" id="PolicySearchAgencyCodeSelectionRow" visible="false">
                    <td align="left">Select Agency Code</td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlPolicySearchAgencyCodeSelection"></asp:DropDownList>
                    </td>
                </tr>--%>
                <tr runat="server" id="PolicySearchAgencyCodeSelectionRow" visible="false">
                    <td colspan="2" align="center">
                        Select Agency Code<br />
                        <asp:DropDownList runat="server" ID="ddlPolicySearchAgencyCodeSelection"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center"><asp:CheckBox runat="server" ID="cbPolicySearchReturnPH1Name" Text="Force Return of PH1 Name?" /></td>
                </tr>
                <tr>
                    <td colspan="2" align="center"><asp:Button runat="server" ID="btnPolicySearch" Text="Search For Policy" /></td>
                </tr>
            </table>
        </asp:Panel>        
        <asp:Panel ID="pnlSearchResults" runat="server" Visible="false" CssClass="normaltext">
            <br />
            <table id="SearchResultsTable" align="center" style="BACKGROUND-COLOR: whitesmoke; font-weight:bold; padding:0; border:solid 1px InactiveCaptionText" class="normaltext" runat="server">
		        <tr>
		            <td align="left">&nbsp;<IMG src="images/redtab.jpg" />&nbsp;<asp:Label ID="lblData" runat="server" Text=""></asp:Label></td>
		            <td align="right"><asp:Label ID="lblPage" runat="server" Text="" Visible="false"></asp:Label></td>
		        </tr>
		        <tr>
		            <td colspan="2">
                        <asp:datagrid id="dgrdSearchResults" runat="server" AllowSorting="true" HorizontalAlign="Center" CellPadding="3" BorderStyle="None" BorderColor="#999999" BackColor="White" BorderWidth="1px" AutoGenerateColumns="false">
		                    <FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="RoyalBlue"></SelectedItemStyle>
							<AlternatingItemStyle Font-Size="Smaller" BackColor="#B6C7E1"></AlternatingItemStyle>
							<ItemStyle Font-Size="Smaller" ForeColor="Black" BackColor="#D3E4FE"></ItemStyle>
							<HeaderStyle Font-Size="Smaller" Font-Bold="True" HorizontalAlign="Center" ForeColor="Black" BackColor="#CCD1E3"></HeaderStyle>
							<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
							<Columns>
								<%--<asp:ButtonColumn Text="Select" ButtonType="PushButton" CommandName="Select"></asp:ButtonColumn>--%>
                                <asp:TemplateColumn>
										    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
										    <ItemStyle HorizontalAlign="Center"></ItemStyle>
										    <ItemTemplate>
											    <asp:Button id="btnSelect" runat="server" Text="Select" CommandName="Select"></asp:Button>
                                                <span runat="server" id="DeleteImageSection" visible="false">
                                                    &nbsp;&nbsp;
                                                    <asp:Button id="btnDelete" runat="server" Text="Delete" CommandName="Delete"></asp:Button>
                                                </span>
										    </ItemTemplate>
									    </asp:TemplateColumn>
								<asp:BoundColumn DataField="polNum" SortExpression="polNum, polId, polImgNum" HeaderText="Policy #"></asp:BoundColumn>
                                <asp:BoundColumn DataField="quoteNum" SortExpression="quoteNum, polNum, polId, polImgNum" HeaderText="Quote #"></asp:BoundColumn>
								<asp:BoundColumn DataField="polId" SortExpression="polId, polImgNum" HeaderText="Policy Id"></asp:BoundColumn>
								<asp:BoundColumn DataField="polImgNum" SortExpression="polImgNum, polNum, polId" HeaderText="Policy Image #"></asp:BoundColumn>
                                <asp:BoundColumn DataField="transType" SortExpression="transType, polNum, polId, polImgNum" HeaderText="Image TransType"></asp:BoundColumn>
                                <asp:BoundColumn DataField="polStatusCode" SortExpression="polStatusCode, polNum, polId, polImgNum" HeaderText="Image Status"></asp:BoundColumn>
                                <asp:BoundColumn DataField="agCode" SortExpression="agCode, polNum, polId, polImgNum" HeaderText="Agency Code"></asp:BoundColumn>
                                <asp:BoundColumn DataField="effDate" SortExpression="effDate, polNum, polId, polImgNum" HeaderText="Eff Date" DataFormatString="{0:d}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="expDate" SortExpression="expDate, polNum, polId, polImgNum" HeaderText="Exp Date" Visible="false" DataFormatString="{0:d}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="teffDate" SortExpression="teffDate, polNum, polId, polImgNum" HeaderText="Tran Eff Date" DataFormatString="{0:d}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="texpDate" SortExpression="texpDate, polNum, polId, polImgNum" HeaderText="Tran Exp Date" Visible="false" DataFormatString="{0:d}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="ph1Name" SortExpression="ph1SortName, polNum, polId, polImgNum" HeaderText="PH1 Name" Visible="false"></asp:BoundColumn>
                                <asp:BoundColumn DataField="agId" SortExpression="agId, polNum, polId, polImgNum" HeaderText="Agency Id" Visible="false"></asp:BoundColumn>
							</Columns>
		                </asp:datagrid>
		            </td>
		        </tr>
		    </table>
        </asp:Panel>        
        <br />
        <asp:Panel ID="pnlPolicyInfo" runat="server" Visible="false" CssClass="normaltext">
            <br />
            <table width="400px" align="center" runat="server" id="tblCurrentInfo" cellpadding="1" border="1">
                <tr>
                    <td colspan="2" align="center" style="font-weight:bold; font-style:italic;">Current Info</td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Policy Number: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolicyNumber"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Quote Number: </td>
                    <td align="left"><asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Policy Id: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolicyId"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Policy Image Number: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolicyImageNum"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Image TransType: </td>
                    <td align="left"><asp:Label runat="server" ID="lblTransType"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Image Status: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolStatusCode"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Agency Code: </td>
                    <td align="left"><asp:Label runat="server" ID="lblAgencyCode"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Term Dates: </td>
                    <td align="left"><asp:Label runat="server" ID="lblTermDates"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Tran Dates: </td>
                    <td align="left"><asp:Label runat="server" ID="lblTranDates"></asp:Label></td>
                </tr>
            </table>
            <br />
            <table width="400px" align="center" runat="server" id="tblSelectedInfo" cellpadding="1" border="1">
                <tr>
                    <td colspan="2" align="center" style="font-weight:bold; font-style:italic;">Selected Info</td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Policy Number: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolicyNumber_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Quote Number: </td>
                    <td align="left"><asp:Label runat="server" ID="lblQuoteNumber_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Policy Id: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolicyId_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Policy Image Number: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolicyImageNum_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Image TransType: </td>
                    <td align="left"><asp:Label runat="server" ID="lblTransType_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Image Status: </td>
                    <td align="left"><asp:Label runat="server" ID="lblPolStatusCode_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Agency Code: </td>
                    <td align="left"><asp:Label runat="server" ID="lblAgencyCode_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Term Dates: </td>
                    <td align="left"><asp:Label runat="server" ID="lblTermDates_selected"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Tran Dates: </td>
                    <td align="left"><asp:Label runat="server" ID="lblTranDates_selected"></asp:Label></td>
                </tr>
            </table>
            <br />
            <p align="center"><asp:Button runat="server" ID="btnPolicyInfoNewEndorsement" Text="New Endorsement?" Visible="false" /><span runat="server" id="ReadOnlyButtonSection" visible="false">&nbsp;&nbsp;<asp:Button runat="server" ID="btnViewReadOnly" Text="View ReadOnly in VR" /></span></p>
        </asp:Panel>
        <br />
        <asp:Panel ID="pnlNewEndorsement" runat="server" Visible="false" CssClass="normaltext">
            <br />
            <table width="600px" align="center" cellpadding="1" border="1">
                <tr>
                    <td colspan="2" align="center"><b>New Endorsement</b></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">New Endorsement Policy #: </td>
                    <td align="left"><asp:Label runat="server" ID="lblNewEndorsementPolicyNum"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">New Endorsement PolicyId: </td>
                    <td align="left"><asp:Label runat="server" ID="lblNewEndorsementPolicyId"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement Transaction Effective Date: </td>
                    <td align="left"><asp:TextBox runat="server" ID="txtNewEndorsementTranEffDate"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement Remarks: </td>
                    <td align="left"><asp:TextBox runat="server" ID="txtNewEndorsementRemarks" Width="250px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2" align="center"><asp:Button runat="server" ID="btnNewEndorsementCreate" Text="Create New Endorsement" /></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlExistingEndorsement" runat="server" Visible="false" CssClass="normaltext">
            <br />
            <table width="600px" align="center" cellpadding="1" border="1">
                <tr>
                    <td colspan="2" align="center"><b>Existing Endorsement</b></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement Policy #: </td>
                    <td align="left"><asp:Label runat="server" ID="lblExistingEndorsementPolicyNum"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement PolicyId: </td>
                    <td align="left"><asp:Label runat="server" ID="lblExistingEndorsementPolicyId"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement PolicyImageNum: </td>
                    <td align="left"><asp:Label runat="server" ID="lblExistingEndorsementPolicyImageNum"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement Transaction Effective Date: </td>
                    <td align="left"><asp:Label runat="server" ID="lblExistingEndorsementTranEffDate"></asp:Label><asp:TextBox runat="server" ID="txtExistingEndorsementTranEffDate" Visible="false"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Endorsement Remarks: </td>
                    <td align="left"><asp:TextBox runat="server" ID="txtExistingEndorsementRemarks" Width="250px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="right" style="font-weight:bold;">Transaction Reason: </td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlExistingEndorsementTransReason">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="10018">Add Add'l Interest</asp:ListItem>
                            <asp:ListItem Value="10135">Add Addl Interest</asp:ListItem>
                            <asp:ListItem Value="10103">Add Auto Loan/Lease Coverage</asp:ListItem>
                            <asp:ListItem Value="10090">Add Collision Coverage</asp:ListItem>
                            <asp:ListItem Value="10089">Add Comprehensive Coverage</asp:ListItem>
                            <asp:ListItem Value="10010">Add Coverage</asp:ListItem>
                            <asp:ListItem Value="10014">Add Credits</asp:ListItem>
                            <asp:ListItem Value="10081">Add Driver</asp:ListItem>
                            <asp:ListItem Value="10023">Add Fee</asp:ListItem>
                            <asp:ListItem Value="10091">Add Good Student Discount</asp:ListItem>
                            <asp:ListItem Value="10092">Add Loss Payee</asp:ListItem>
                            <asp:ListItem Value="10120">Add Mortgagee</asp:ListItem>
                            <asp:ListItem Value="10102">Add Select Market Credit</asp:ListItem>
                            <asp:ListItem Value="10098">Add Towing Coverage</asp:ListItem>
                            <asp:ListItem Value="10100">Add Transportation Expense Coverage</asp:ListItem>
                            <asp:ListItem Value="10083">Add Vehicle</asp:ListItem>
                            <asp:ListItem Value="10086">Address Change</asp:ListItem>
                            <asp:ListItem Value="10093">Amend Liability Limit</asp:ListItem>
                            <asp:ListItem Value="10138">Amend Med Pay Limit</asp:ListItem>
                            <asp:ListItem Value="10094">Amend Physical Damage Deductible</asp:ListItem>
                            <asp:ListItem Value="10099">Amend Towing Coverage</asp:ListItem>
                            <asp:ListItem Value="10101">Amend Transportation Exp. Coverage</asp:ListItem>
                            <asp:ListItem Value="10095">Amend UM/UIM Limit</asp:ListItem>
                            <asp:ListItem Value="10141">Anniversary Rated</asp:ListItem>
                            <asp:ListItem Value="10134">Birthday Credit</asp:ListItem>
                            <asp:ListItem Value="10174">Certified Policy</asp:ListItem>
                            <asp:ListItem Value="10020">Change Add'l Interest</asp:ListItem>
                            <asp:ListItem Value="10016">Change Address</asp:ListItem>
                            <asp:ListItem Value="10055">Change Bill To</asp:ListItem>
                            <asp:ListItem Value="10123">Change Bill To Information</asp:ListItem>
                            <asp:ListItem Value="10012">Change Coverage</asp:ListItem>
                            <asp:ListItem Value="10124">Change Deductible</asp:ListItem>
                            <asp:ListItem Value="10096">Change Driver Information</asp:ListItem>
                            <asp:ListItem Value="10127">Change Dwelling Limit</asp:ListItem>
                            <asp:ListItem Value="10148">Change Experience Mods</asp:ListItem>
                            <asp:ListItem Value="10088">Change Insured's Address</asp:ListItem>
                            <asp:ListItem Value="10125">Change Liability Limit</asp:ListItem>
                            <asp:ListItem Value="10057">Change Mailing Address</asp:ListItem>
                            <asp:ListItem Value="10097">Change Marital Status</asp:ListItem>
                            <asp:ListItem Value="10121">Change Mortgagee</asp:ListItem>
                            <asp:ListItem Value="10056">Change Name</asp:ListItem>
                            <asp:ListItem Value="10022">Change Payplan</asp:ListItem>
                            <asp:ListItem Value="10085">Change Vehicle</asp:ListItem>
                            <asp:ListItem Value="10183">Claims Made OERP</asp:ListItem>
                            <asp:ListItem Value="10013">Correct Coverage</asp:ListItem>
                            <asp:ListItem Value="10017">Correct Dec</asp:ListItem>
                            <asp:ListItem Value="10058">Correct New Business</asp:ListItem>
                            <asp:ListItem Value="10054">Decrease Coverage</asp:ListItem>
                            <asp:ListItem Value="10019">Delete Add'l Interest</asp:ListItem>
                            <asp:ListItem Value="10011">Delete Coverage</asp:ListItem>
                            <asp:ListItem Value="10015">Delete Credits</asp:ListItem>
                            <asp:ListItem Value="10082">Delete Driver</asp:ListItem>
                            <asp:ListItem Value="10024">Delete Fee</asp:ListItem>
                            <asp:ListItem Value="10122">Delete Mortgagee</asp:ListItem>
                            <asp:ListItem Value="10084">Delete Vehicle</asp:ListItem>
                            <asp:ListItem Value="10002">Endorsement</asp:ListItem>
                            <asp:ListItem Value="10169">Endorsement Change Dec and Full Revised Dec</asp:ListItem>
                            <asp:ListItem Value="10168">Endorsement Change Dec Only</asp:ListItem>
                            <asp:ListItem Value="32">Experience Modification Factor Applied</asp:ListItem>
                            <asp:ListItem Value="10053">Increase Coverage</asp:ListItem>
                            <asp:ListItem Value="10119">Internal Endorsement</asp:ListItem>
                            <asp:ListItem Value="10126">Miscellaneous Change</asp:ListItem>
                            <asp:ListItem Value="10075">Misrepresentation - CLUE</asp:ListItem>
                            <asp:ListItem Value="10074">Misrepresentation - MVR & CLUE</asp:ListItem>
                            <asp:ListItem Value="10021">Multiple Changes</asp:ListItem>
                            <asp:ListItem Value="38">Package Part Cancellation</asp:ListItem>
                            <asp:ListItem Value="33">Rate Status Update </asp:ListItem>
                            <asp:ListItem Value="10068">Suppress Printing</asp:ListItem>
                            <asp:ListItem Value="10087">Tier Change</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="PPA_HasTestDriverAndVehicleRow" visible="false">
                    <td colspan="2" align="center"><asp:Label runat="server" ID="lblExistingEndorsementHasTestDriverAndVehicle" Visible="false"></asp:Label><asp:CheckBox runat="server" ID="cbExistingEndorsementUseTestDriverAndVehicle" Text="Add/Maintain Test Driver and Vehicle?" /></td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button runat="server" ID="btnExistingEndorsementSave" Text="Save Endorsement" Enabled="false" />
                        &nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnExistingEndorsementRate" Text="Rate Endorsement" Enabled="false" />
                        &nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnExistingEndorsementDelete" Text="Delete Endorsement" Enabled="false" />
                    </td>
                </tr>
            </table>
            <br />
            <p class="normaltext" align="center" runat="server" id="ExistingEndorsementIssuanceSection" visible="false">
                <span>This endorsement was successfully rated from this page recently. Click the button below to attempt issuance using the information currently saved in Diamond.</span>
                <br /><br />
                <asp:Button runat="server" ID="btnExistingEndorsementIssuance" Text="Issue Endorsement" />
            </p>
        </asp:Panel>
        <br />
        <br />

    </div>
    </form>
</body>
</html>
