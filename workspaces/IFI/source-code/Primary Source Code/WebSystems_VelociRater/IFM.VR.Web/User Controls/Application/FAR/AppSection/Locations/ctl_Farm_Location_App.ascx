<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_Location_App.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_Location_App" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_Location_Description.ascx" TagPrefix="uc1" TagName="ctl_Farm_Location_Description" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_PropertyUpdates_HOM_App.ascx" TagPrefix="uc1" TagName="ctl_PropertyUpdates_HOM_App" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_Structures_App.ascx" TagPrefix="uc1" TagName="ctl_Farm_Structures_App" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_Location_Description_List.ascx" TagPrefix="uc1" TagName="ctl_Farm_Location_Description_List" %>

<h3>
    <asp:Label ID="lblHeader" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <asp:Label ID="lblPrimary" runat="server" Text="Primary"></asp:Label>
    <div style="align-content: center; margin-bottom: 30px;">
        <uc1:ctl_Farm_Location_Description runat="server" ID="ctl_Farm_Location_Description" />
    </div>

    <section  style="clear: both; margin-bottom: 20px;">
         <div runat="server" id="divBlanketAcreage" visible="false">
             <asp:CheckBox ID="chkBlanketAcreage" runat="server" Text="Blanket Acreage" />
             <span runat="server" id="divtxtTotalBlanketAcreage">
                 <label  style="margin-left:30px""  tabindex="12" for="<%=txtTotalBlanketAcreage.ClientID%>">Total Blanket Acreage:</label>
                 <asp:TextBox ID="txtTotalBlanketAcreage" MaxLength="10" TabIndex="12" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46'></asp:TextBox>
             </span>
         </div>
    </section>


    <uc1:ctl_Farm_Location_Description_List runat="server" ID="ctl_Farm_Location_Description_List" />

    <uc1:ctl_PropertyUpdates_HOM_App runat="server" ID="ctl_PropertyUpdates_HOM_App" />
    <uc1:ctl_Farm_Structures_App runat="server" ID="ctl_Farm_Structures_App" />
</div>