Public Class ctlHomAdditionalResidenceList
    Inherits ctlSectionCoverageControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.ctlHomAdditionalResidence.InitFromExisting(Me)
        Me.Repeater1.DataSource = Me.MySubSectionCoverages
        Me.Repeater1.DataBind()
        Me.FindChildVrControls()
        Dim index As Int32 = 1
        For Each c In Me.GatherChildrenOfType(Of ctlHomAdditionalResidence)
            If Not c.Equals(ctlHomAdditionalResidence) Then
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
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False) 'need to save again
    End Sub
End Class