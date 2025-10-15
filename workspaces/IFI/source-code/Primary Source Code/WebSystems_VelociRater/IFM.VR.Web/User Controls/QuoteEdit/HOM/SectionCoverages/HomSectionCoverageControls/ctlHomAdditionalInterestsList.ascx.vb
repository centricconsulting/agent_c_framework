Imports QuickQuote.CommonObjects

Public Class ctlHomAdditionalInterestsList
    Inherits ctlSectionCoverageControlBase

    'Added 2/2/18 control for HOM Upgrade MLW

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'If Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence Then
        '    lnkAddAddress.Text = "Add Another Student"
        'End If
        If Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
            lnkAddAddress.Text = "Add Additional Relative"
        End If
        If MySectionCoverage IsNot Nothing Then
            If MySectionCoverage.AdditionalInterests IsNot Nothing Then
                Me.Repeater1.DataSource = Me.MySectionCoverage.AdditionalInterests
            Else
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                Me.Repeater1.DataSource = Me.MySectionCoverage.AdditionalInterests
            End If
        Else
            'need to create a dummy list to show the initial fields since the control is not initializing it like the other controls - not a part of the coverage, but part of a list
            Dim dummyList As New List(Of QuickQuoteAdditionalInterest)
            Dim dummyItem As New QuickQuoteAdditionalInterest
            'added 4/16/18 for Bug 26103 MLW
            If Me.SectionCoverageIAndIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
                dummyItem.Address.StateId = "0" 'Need to initially save to blank, will save in child control - added 4/16/18 for Bug 26103
            Else
                dummyItem.Address.StateId = "16" 'Need assisted living state to default to IN - added 4/16/18 for Bug 26103
            End If
            dummyItem.TypeId = "81" 'Relative
            dummyList.Add(dummyItem)
                Me.Repeater1.DataSource = dummyList
            End If
            Me.Repeater1.DataBind()
        Me.FindChildVrControls()
        Dim index As Int32 = 0
        For Each c In Me.GatherChildrenOfType(Of ctlHomAdditionalInterests)
            c.InitFromExisting(Me)
            c.aiCounter = index
            index += 1
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
        If MySectionCoverage Is Nothing Then
            Me.CreateMySectionCoverage()
        End If
        'Updated 03/24/2020 for Home Endorsement Bug 44392 MLW
        'If MySectionCoverage IsNot Nothing Then
        '    MySectionCoverage.AdditionalInterests = Nothing
        '    MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
        'End If
        If MySectionCoverage.AdditionalInterests Is Nothing Then
            MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
        End If
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkAddAddress_Click(sender As Object, e As EventArgs) Handles lnkAddAddress.Click
        If MySectionCoverage Is Nothing Then
            'coverage could be nothing since we are not init the coverage in the control - not being passed, need to create it
            Me.CreateMySectionCoverage()
        End If
        Me.Save_FireSaveEvent(False) 'need to save first
        Dim ai As QuickQuoteAdditionalInterest = New QuickQuoteAdditionalInterest
        ai.Name.CommercialName1 = ""
        ai.Description = ""
        ai.TypeId = 81
        If MySectionCoverage.AdditionalInterests IsNot Nothing Then
            'added 4/16/18 for Bug 26103 MLW
            If ai.Address IsNot Nothing Then
                If Me.SectionCoverageIAndIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
                    ai.Address.StateId = "0" 'Need to initially save to blank, will save in child control - added 4/16/18 for Bug 26103
                Else
                    ai.Address.StateId = "16" 'Need assisted living state to default to IN - added 4/16/18 for Bug 26103
                End If
            End If
            MySectionCoverage.AdditionalInterests.Add(ai)
        Else
            MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            'added 4/16/18 for Bug 26103 MLW
            If ai.Address IsNot Nothing Then
                If Me.SectionCoverageIAndIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
                    ai.Address.StateId = "0" 'Need to initially save to blank, will save in child control - added 4/16/18 for Bug 26103
                Else
                    ai.Address.StateId = "16" 'Need assisted living state to default to IN - added 4/16/18 for Bug 26103
                End If
            End If
            MySectionCoverage.AdditionalInterests.Add(ai)
        End If
        'Updated 04/01/2020 for Home Endorsements Bug 44329 MLW
        'Me.Populate_FirePopulateEvent()
        'Me.Save_FireSaveEvent(False) 'need to save again
        Me.Populate()
    End Sub

End Class