Imports QuickQuote.CommonObjects
Public Class ctlHomMultipleNamesList
    Inherits ctlSectionCoverageControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion Then
            lnkAddAddress.Text = "Add Additional Canine"
            'Added 11/18/2019 for bug 27734 MLW
            divSpecialText.Visible = True
            lblSpecialText.Text = "<br />Form 2477 10 17 (Canine Liability Exclusion Endorsement) must be completed, signed and returned to Underwriting <span style='color:red;'>AND approved by your underwriter</span> before this policy can be issued. Please click <a href='" + System.Configuration.ConfigurationManager.AppSettings("HOM_Help_CanineForm") + "'  target='_blank' style='color:blue;font-weight:bold;'>here</a> for the form."
        Else
            divSpecialText.Visible = False 'Added 11/18/2019 for bug 27734 MLW
        End If
        Me.ctlHomMultipleNames.InitFromExisting(Me)
        Me.Repeater1.DataSource = Me.MySubSectionCoverages
        Me.Repeater1.DataBind()
        Me.FindChildVrControls()
        Dim index As Int32 = 1
        For Each c In Me.GatherChildrenOfType(Of ctlHomMultipleNames)
            If Not c.Equals(ctlHomMultipleNames) Then
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