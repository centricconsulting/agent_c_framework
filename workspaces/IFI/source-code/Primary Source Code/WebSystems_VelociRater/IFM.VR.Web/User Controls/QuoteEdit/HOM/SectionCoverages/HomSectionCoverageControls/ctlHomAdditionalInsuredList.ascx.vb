
Public Class ctlHomAdditionalInsuredList
    Inherits ctlSectionCoverageControlBase

    'Added control 4/30/18 for HOM Upgrade Bug 26102 MLW
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.ctlHomAdditionalInsured.InitFromExisting(Me)
        Me.Repeater1.DataSource = Me.MySubSectionCoverages
        Me.Repeater1.DataBind()
        Me.FindChildVrControls()
        Dim index As Int32 = 1
        For Each c In Me.GatherChildrenOfType(Of ctlHomAdditionalInsured)
            If Not c.Equals(ctlHomAdditionalInsured) Then
                c.InitFromExisting(Me)
                c.CoverageIndex = index
                index += 1
                'c.Populate() done below
            End If
        Next

        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            lnkAddAddress.Visible = False
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkAddAddress_Click(sender As Object, e As EventArgs) Handles lnkAddAddress.Click
        Me.Save_FireSaveEvent(False) 'need to save first
        Me.CreateMySectionCoverage()
        'Updated 04/02/2020 for Home Endorsements Bug 44392 MLW
        'Me.Populate_FirePopulateEvent()
        'Me.Save_FireSaveEvent(False) 'need to save again
        Me.Populate()
    End Sub
End Class