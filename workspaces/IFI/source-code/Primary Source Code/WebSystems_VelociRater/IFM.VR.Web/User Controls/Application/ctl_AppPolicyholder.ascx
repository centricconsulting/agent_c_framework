<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppPolicyholder.ascx.vb" Inherits="IFM.VR.Web.ctl_AppPolicyholder" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlInsured.ascx" TagPrefix="uc1" TagName="ctlInsured" %>

<div>
<div runat="server" id="divMain">
    <uc1:ctlInsured runat="server" IsPolicyHolderNum1="true" ID="ctlInsured" />    
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
    </div>
