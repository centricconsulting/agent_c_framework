<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/DiamondQuickQuote.master" CodeFile="PendingEndorsements.aspx.vb" Inherits="PendingEndorsements" %>

<asp:Content runat="server" ID="cScripts" ContentPlaceHolderID="Scripts">
    <title>Pending Endorsements</title>
    <script type="text/javascript" src="js/CommonFunctions.js"></script>
</asp:Content>
<asp:Content runat="server" ID="cApplication" ContentPlaceHolderID="Application">
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
            <%--<tr runat="server" id="EndorsementSearchAgencyCodeWarningRow" visible="false">
                <td colspan="2" align="center">*Warning: searching all agency codes for staff could result in a database timeout.</td>
            </tr>--%>
            <tr runat="server" id="EndorsementSearchAgencyCodeRow">
                <td align="left">Agency Codes to Search</td>
                <td align="left">
                    <asp:DropDownList runat="server" ID="ddlEndorsementSearchAgenciesToUse">
                        <asp:ListItem>Just my primary agency code</asp:ListItem>
                        <asp:ListItem>Just my primary and secondary codes</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="EndorsementSearchAgencyCodeSelectionRow" visible="false">
                <td colspan="2" align="center">
                    Select Agency Code<br />
                    <asp:DropDownList runat="server" ID="ddlEndorsementSearchAgencyCodeSelection"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center"><asp:Button runat="server" ID="btnEndorsementSearch" Text="Search For Endorsement" /></td>
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
						</Columns>
		            </asp:datagrid>
		        </td>
		    </tr>
		</table>
    </asp:Panel>

    <br />
</asp:Content>