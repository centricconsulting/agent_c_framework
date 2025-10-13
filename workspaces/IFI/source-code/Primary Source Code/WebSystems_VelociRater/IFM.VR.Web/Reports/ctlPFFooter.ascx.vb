Imports Diamond.Common.StaticDataManager.Objects.VersionData
Imports IFM.VR.Web.Helpers
Imports PublicQuotingLib.Models
Imports QuickQuote.CommonObjects
Imports System.Globalization

Public Class ctlPFFooter
    Inherits VRControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.DateToday.Text = Date.Now.ToShortDateString()
        PrintFriendlyExpireNoticeHelper.SetExpireDate(Quote, ExpireContainer, ExpireNotice)
    End Sub

    Public Overrides Sub LoadStaticData()
        'Not Needed
    End Sub

    Public Overrides Sub Populate()
        'Not Needed
    End Sub

    Public Overrides Function Save() As Boolean
        'Not Needed
    End Function

    Public Overrides Sub AddScriptAlways()
        'Not Needed
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Not Needed
    End Sub
End Class