Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_ContractorEndorsement
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Me.Visible = False
        If Quote IsNot Nothing Then

            If GoverningStateQuote.HasContractorsEnhancement Then
                Dim linkString As String = "<a href='xxx' target='_blank' style='color:blue;'>Contractors Inland Marine Endorsement</a>"
                Dim linkSetting As String = System.Configuration.ConfigurationManager.AppSettings("CPP_Help_ContractorsPackageCoverage").ToString
                linkString = linkString.Replace("xxx", linkSetting)
                chkContractorsEndorsement.Text = linkString
                Me.Visible = True
            End If

        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub ClearControl()
    End Sub

    Public Overrides Function hasSetting() As Boolean
        If Me.Visible Then
            Return True
        End If
        Return False
    End Function
End Class