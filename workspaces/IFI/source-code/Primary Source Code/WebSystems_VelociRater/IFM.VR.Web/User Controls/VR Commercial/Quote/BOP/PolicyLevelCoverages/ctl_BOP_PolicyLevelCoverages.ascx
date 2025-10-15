<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_PolicyLevelCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/PolicyLevelCoverages/ctl_BOP_GeneralInformation.ascx" TagPrefix="uc1" TagName="ctl_BOP_GeneralInformation" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/PolicyLevelCoverages/ctl_BOP_Coverages.ascx" TagPrefix="uc1" TagName="ctl_BOP_Coverages" %>


<uc1:ctl_BOP_GeneralInformation runat="server" id="ctl_BOP_GeneralInformation" />
<uc1:ctl_BOP_Coverages runat="server" id="ctl_BOP_Coverages" />
