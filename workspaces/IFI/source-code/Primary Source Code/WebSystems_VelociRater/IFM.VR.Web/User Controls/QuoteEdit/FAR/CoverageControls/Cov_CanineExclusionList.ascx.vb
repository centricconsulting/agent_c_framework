Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Public Class Cov_CanineExclusionList
    Inherits VRControlBase

    Public ReadOnly Property MyLocationZero As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote?.Locations.GetItemAtIndex(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MySectionIICoverages() As List(Of QuickQuoteSectionIICoverage)
        Get
            Return MyLocationZero?.SectionIICoverages
        End Get
    End Property

    Public ReadOnly Property MySubSectionCoverages As List(Of QuickQuoteSectionIICoverage)
        Get
            Dim genericCov As New List(Of QuickQuoteSectionIICoverage)
            If MyLocationZero IsNot Nothing Then
                If MyLocationZero.SectionIICoverages IsNot Nothing Then
                    For Each c In (From sc In MyLocationZero.SectionIICoverages Where sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion Select sc)
                        genericCov.Add(c)
                    Next
                End If
            End If

            Return genericCov
        End Get
    End Property



    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        lblSpecialText.Text = "<br />Form FAR 4003 01 22 (Exclusion Canine) must be completed, signed and returned to Underwriting <span style='color:red;'>AND approved by your underwriter</span> before this policy can be issued. Please click <a href='" + System.Configuration.ConfigurationManager.AppSettings("FAR_Help_CanineForm") + "'  target='_blank' style='color:blue;font-weight:bold;'>here</a> for the form."

        Dim MyList
        If MySubSectionCoverages.IsLoaded Then
            If MySubSectionCoverages.Count = 1 AndAlso MySubSectionCoverages(0).Description = String.Empty AndAlso MySubSectionCoverages(0).Name.FirstName = String.Empty Then
                chkCanine.Checked = False
                dvCanineInfo.Attributes.Add("style", "display:none;")
            Else
                chkCanine.Checked = True
                dvCanineInfo.Attributes.Add("style", "display:block;")
            End If
            MyList = Me.MySubSectionCoverages
        Else
            Dim genericCov As New List(Of QuickQuoteSectionIICoverage)
            genericCov.Add(NewCanineCoverage)
            chkCanine.Checked = False
            dvCanineInfo.Attributes.Add("style", "display:none;")
            MyList = genericCov
        End If


        Me.ceRepeater.DataSource = MyList
        Me.ceRepeater.DataBind()
        Me.FindChildVrControls()
        Dim index As Int32 = 0
        For Each c In Me.GatherChildrenOfType(Of Cov_CanineExclusionItem)
            c.CoverageIndex = index
            index += 1
        Next

        If Me.IsQuoteReadOnly Then
            lnkAddCanine.Visible = False
        End If

        If IsOnAppPage Then
            chkCanine.Visible = False
            lblCanine.Visible = False
        End If

        PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim scriptCanineExclusion As String = "ToggleCanineExclusion(""" + chkCanine.ClientID + """, """ + dvCanineInfo.ClientID + """);"
        chkCanine.Attributes.Add("onclick", scriptCanineExclusion)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If chkCanine.Checked OrElse (IsOnAppPage AndAlso Me.Visible) Then
            SaveChildControls()
        Else
            MySectionIICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
        End If

        Return True
    End Function

    Protected Sub lnkAddCanine_Click(sender As Object, e As EventArgs) Handles lnkAddCanine.Click
        Me.Save_FireSaveEvent(False) 'need to save first
        MySectionIICoverages.Add(NewCanineCoverage)
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False) 'need to save again
    End Sub

    Protected Function NewCanineCoverage() As QuickQuoteSectionIICoverage
        Dim newCoverage = New QuickQuoteSectionIICoverage()
        newCoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion
        Return newCoverage
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Canine Exclusion"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        ValidateChildControls(valArgs)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        MySectionIICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        MyBase.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        'Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, NewEffectiveDate, OldEffectiveDate)
    End Sub

End Class