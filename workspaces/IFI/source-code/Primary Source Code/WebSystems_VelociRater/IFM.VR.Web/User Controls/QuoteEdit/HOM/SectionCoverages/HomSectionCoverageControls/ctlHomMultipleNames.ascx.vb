Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctlHomMultipleNames
    Inherits ctlSectionCoverageControlBase

    'added control 1/23/18 for HOM Upgrade - MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion Then

                Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

                txtDescription.Attributes.Add("onfocus", "this.select()")
            End If
        End If

        Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            LoadStaticData()
            If MySectionCoverage IsNot Nothing Then
                Me.txtName.Text = MySectionCoverage.Name.FirstName
                Me.txtDescription.Text = MySectionCoverage.Description
            Else
                Me.ClearControl()
            End If

            lblMaxCharCount.Visible = False
            lblMaxChar.Visible = False

            If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion Then
                lblName.Text = "*Canine Name"
                lblDescription.Text = "*Canine Description"
                lblMaxCharCount.Visible = True
                lblMaxChar.Visible = True
                'Updated 7/3/2019 for Home Endorsements Project Task 38906, 38925 MLW
                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                    If Me.txtDescription.Text = "" Then
                        Me.txtDescription.Text = "CANINE #" & (Me.CoverageIndex + 1).ToString()
                    Else
                        If Left(Me.txtDescription.Text, 8) = "CANINE #" Then
                            Me.txtDescription.Text = "CANINE #" & (Me.CoverageIndex + 1).ToString()
                        End If
                    End If
                End If
            End If

            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                lnkDelete.Visible = False
                lblMaxCharCount.Visible = False
                lblMaxChar.Visible = False
            End If

            Me.PopulateChildControls()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Public Overrides Function Save() As Boolean
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            If MySectionCoverage IsNot Nothing Then
                MySectionCoverage.Name.FirstName = Me.txtName.Text.Trim()
                MySectionCoverage.Description = Me.txtDescription.Text.Trim()
            End If

            SaveChildControls()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            If Me.MySectionCoverage IsNot Nothing Then
                Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                If valList.Any() Then
                    For Each v In valList
                        Select Case v.FieldId
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Name
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtName, v, accordList)
                        End Select
                    Next

                End If
            End If
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.Save_FireSaveEvent(False)
            Me.DeleteMySectionCoverage()
            Me.Populate_FirePopulateEvent()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Public Overrides Sub ClearControl()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ClearControl()
            Me.txtName.Text = ""
            Me.txtDescription.Text = ""
        End If
    End Sub
End Class