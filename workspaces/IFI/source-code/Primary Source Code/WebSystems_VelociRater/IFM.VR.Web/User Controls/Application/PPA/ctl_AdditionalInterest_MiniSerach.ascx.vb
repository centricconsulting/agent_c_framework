Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class ctl_AdditionalInterest_MiniSerach
    Inherits VRControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddVariableLine("var agencyID_AdditionalInterest = '" + Me.AgencyId.ToString() + "'; ")
        'Me.VRScript.AddScriptLine("AdditionalInterest.InitAdditionalInterestMini();") 'Removed 10-30-2015 Un-needed Matt A
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
    End Sub
End Class