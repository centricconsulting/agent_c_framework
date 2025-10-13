Imports QuickQuote.CommonObjects

Public Class ctlHomSpecifiedStructureList
    Inherits ctlSectionCoverageControlBase

    Public ReadOnly Property JsClearControl_92049 As String
        Get
            Return ctlHomSpecifiedStructure.ClearJsLogic
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.ctlHomSpecifiedStructure.InitFromExisting(Me) 'always here


        If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures OrElse Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises Then
            Me.lnkAddAddress.Visible = False
            Me.Repeater1.Visible = False
        Else
            Me.Repeater1.DataSource = Me.MySubSectionCoverages
            Me.Repeater1.DataBind()
            Me.FindChildVrControls()
            Dim index As Int32 = 1
            For Each c In Me.GatherChildrenOfType(Of ctlHomSpecifiedStructure)
                If Not c.Equals(ctlHomSpecifiedStructure) Then
                    c.InitFromExisting(Me)
                    c.CoverageIndex = index
                    index += 1
                    'c.Populate() done below
                End If
            Next
        End If

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