Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.DFR

Public Class ctlDFROptionalCoverages
    Inherits VRControlBase

    Public Event RateQuote()

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property CurrentForm As String
        Get
            Try
                Return MyLocation.FormTypeId
            Catch ex As Exception
                Return ""
            End Try

        End Get
    End Property

    Public Property SelectedOptCoverageCnt() As Integer
        Get
            Return ViewState("vs_OptCoverageCnt")
        End Get
        Set(ByVal value As Integer)
            ViewState("vs_OptCoverageCnt") = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim scriptEarthquakeDeduct As String = "ToggleEarthquake(""" + chkEarthquake.ClientID + """, """ + dvEarthquakeDeduct.ClientID + """, """ + ddlEarthquakeDeduct.ClientID + """);"
        chkEarthquake.Attributes.Add("onclick", scriptEarthquakeDeduct)

        Dim scriptACV As String = "ToggleCheckboxOnly(""" + chkACV.ClientID + """);"
        chkACV.Attributes.Add("onclick", scriptACV)

        Dim scriptFRC As String = "ToggleCheckboxOnly(""" + chkReplacement.ClientID + """);"
        chkReplacement.Attributes.Add("onclick", scriptFRC)

        'Dim scriptMinSubA As String = "ToggleCheckboxOnly(""" + chkMinSubA.ClientID + """);"
        'chkMinSubA.Attributes.Add("onclick", scriptMinSubA)

        'Dim scriptMinSubAB As String = "ToggleCheckboxOnly(""" + chkMineSubAB.ClientID + """);"
        'chkMineSubAB.Attributes.Add("onclick", scriptMinSubAB)
        Dim scriptMineSub As String = "ToggleMineSub(this, """ + chkMineSubA.ClientID + """, """ + chkMineSubAB.ClientID + """);"
        chkMineSubA.Attributes.Add("onclick", scriptMineSub)
        chkMineSubAB.Attributes.Add("onclick", scriptMineSub)

        Dim scriptSinkhole As String = "ToggleCheckboxOnly(""" + chkSinkhole.ClientID + """);"
        chkSinkhole.Attributes.Add("onclick", scriptSinkhole)
    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddlEarthquakeDeduct, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                SelectedOptCoverageCnt = 0

                ToggleOptCoverageByForm()

                If MyLocation.SectionICoverages IsNot Nothing Then
                    Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage

                    ' Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing
                    sectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)
                    If sectionICoverage IsNot Nothing Then
                        chkACV.Checked = True
                        SelectedOptCoverageCnt += 1
                    End If

                    ' Earthquake
                    sectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake)
                    If sectionICoverage IsNot Nothing Then
                        chkEarthquake.Checked = True
                        ddlEarthquakeDeduct.SelectedValue = sectionICoverage.DeductibleLimitId
                        dvEarthquakeDeduct.Attributes.Add("style", "display:block;")
                        SelectedOptCoverageCnt += 1
                    Else
                        dvEarthquakeDeduct.Attributes.Add("style", "display:none;")
                    End If

                    ' Functional Replacement Cost Loss Settlement
                    sectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment)
                    If sectionICoverage IsNot Nothing Then
                        chkReplacement.Checked = True
                        SelectedOptCoverageCnt += 1
                    End If

                    chkMineSubA.Enabled = True
                    chkMineSubAB.Enabled = True
                    ' Mine Subsidence Cov. A
                    sectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
                    If sectionICoverage IsNot Nothing Then
                        chkMineSubA.Checked = True
                        chkMineSubAB.Enabled = False
                        SelectedOptCoverageCnt += 1
                    End If

                    ' Mine Subsidence - Cov. A & B
                    sectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                    If sectionICoverage IsNot Nothing Then
                        chkMineSubAB.Checked = True
                        chkMineSubA.Enabled = False
                        SelectedOptCoverageCnt += 1
                    End If

                    ' Sinkhole Collapse
                    sectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.SinkholeCollapse)
                    If sectionICoverage IsNot Nothing Then
                        chkSinkhole.Checked = True
                        SelectedOptCoverageCnt += 1
                    End If
                End If
            End If

            'Added 9/18/2019 for DFR Endorsements Project Task 40278 MLW
            If Me.IsQuoteReadOnly Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QQHelper.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                dvOptionalButtons.Visible = False
                dvEndorsementButtons.Visible = True

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                dvEndorsementButtons.Visible = False
                dvOptionalButtons.Visible = True
            End If
        End If
    End Sub

    Private Sub ToggleOptCoverageByForm()
        Select Case CurrentForm
            Case "8", "9", "10"
                dvACV.Attributes.Add("style", "display:none;")
                dvReplacment.Attributes.Add("style", "display:none;")
            Case "11", "12"
                If MyLocation.YearBuilt >= "1947" Then
                    chkReplacement.Checked = False
                    dvReplacment.Attributes.Add("style", "display:none;")
                Else
                    dvReplacment.Attributes.Add("style", "display:block;")
                End If

                dvACV.Attributes.Add("style", "display:block;")
        End Select

        If IFM.VR.Common.Helpers.MineSubsidenceHelper.LocationAllowsMineSubsidence(Quote.Locations(0)) Then
            dvMineSub.Attributes.Add("style", "display:block;")
        Else
            dvMineSub.Attributes.Add("style", "display:none;")
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                If MyLocation.SectionICoverages Is Nothing Then
                    MyLocation.SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                End If

                ' Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing
                If chkACV.Checked Then
                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing).Count <= 0 Then
                        Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage
                        sectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                        MyLocation.SectionICoverages.Add(sectionICoverage)
                    End If
                Else
                    Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)
                    MyLocation.SectionICoverages.Remove(sectionICoverage)
                End If

                ' Earthquake
                If chkEarthquake.Checked Then
                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake).Count <= 0 Then
                        Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage
                        sectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake
                        sectionICoverage.DeductibleLimitId = ddlEarthquakeDeduct.SelectedValue
                        MyLocation.SectionICoverages.Add(sectionICoverage)
                    Else
                        Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake)
                        sectionICoverage.DeductibleLimitId = ddlEarthquakeDeduct.SelectedValue
                    End If
                Else
                    Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake)
                    ddlEarthquakeDeduct.SelectedValue = "0"
                    MyLocation.SectionICoverages.Remove(sectionICoverage)
                End If

                ' Function Replacement Cost Loss Settlement
                If chkReplacement.Checked Then
                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment).Count <= 0 Then
                        Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage
                        sectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment
                        MyLocation.SectionICoverages.Add(sectionICoverage)
                    End If
                Else
                    Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment)
                    MyLocation.SectionICoverages.Remove(sectionICoverage)
                End If

                ' Mine Subsidence Cov. A
                If chkMineSubA.Checked Then
                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA).Count <= 0 Then
                        Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage
                        sectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                        MyLocation.SectionICoverages.Add(sectionICoverage)
                    End If
                Else
                    Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
                    MyLocation.SectionICoverages.Remove(sectionICoverage)
                End If

                ' Mine Subsidence Cov. A & B
                If chkMineSubAB.Checked Then
                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB).Count <= 0 Then
                        Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage
                        sectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                        MyLocation.SectionICoverages.Add(sectionICoverage)
                    End If
                Else
                    Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                    MyLocation.SectionICoverages.Remove(sectionICoverage)
                End If

                ' Sinkhole Collapse
                If chkSinkhole.Checked Then
                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.SinkholeCollapse).Count <= 0 Then
                        Dim sectionICoverage As QuickQuoteSectionICoverage = New QuickQuoteSectionICoverage
                        sectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.SinkholeCollapse
                        MyLocation.SectionICoverages.Add(sectionICoverage)
                    End If
                Else
                    Dim sectionICoverage As QuickQuoteSectionICoverage = MyLocation.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.SinkholeCollapse)
                    MyLocation.SectionICoverages.Remove(sectionICoverage)
                End If
            End If
        End If
        Return False
    End Function

    Protected Sub btnSaveOptionalCoverages_Click(sender As Object, e As EventArgs) Handles btnSaveOptionalCoverages.Click
        Save_FireSaveEvent()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Dwelling Fire Coverage Optional Coverages"
        'Dim divCoverages As String = dvOptionalCoverages.ClientID
        Dim accordList As List(Of VRAccordionTogglePair) = MyAccordionList

        Dim valList = ResidenceCoverageValidator.ValidateDFRLocationResidence(Quote, 0, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case ResidenceCoverageValidator.MissingEarthquake
                        ValidationHelper.Val_BindValidationItemToControl(ddlEarthquakeDeduct, v, accordList)
                End Select
            Next
        End If
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        chkACV.Checked = False
        chkEarthquake.Checked = False
        ddlEarthquakeDeduct.SelectedValue = "0"
        dvEarthquakeDeduct.Attributes.Add("style", "display:none;")
        chkReplacement.Checked = False
        chkMineSubA.Enabled = True
        chkMineSubA.Checked = False
        chkMineSubAB.Enabled = True
        chkMineSubAB.Checked = False
        chkSinkhole.Checked = False
    End Sub

    Protected Sub btnRateOptional_Click(sender As Object, e As EventArgs) Handles btnRateOptional.Click
        RaiseEvent RateQuote()
    End Sub

    'Added 9/18/2019 for DFR Endorsements Project Task 40278 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 9/18/2019 for DFR Endorsements Project Task 40278 MLW
    Protected Sub btnViewGotoNextSection_Click(sender As Object, e As EventArgs) Handles btnViewGotoNextSection.Click
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub
End Class