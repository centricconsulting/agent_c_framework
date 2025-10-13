Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines
Imports IFM.VR.Common.Helpers.FARM

Public Class ctlFarmScheduledLimit
    Inherits VRControlBase

    Public Event RemovePersPropItem(index As Integer)
    Public Event ToggleMoreLessPeak()
    Public Event RefreshAfterNewPeak()
    Public Event SaveBeforeNewPeak()
    Public Event RaisePeakButtonPress(button As String)
    Public Event RaiseCurrentRowNumber(rowNumber As Integer)

    Public Property ScheduledLimit() As String
        Get
            Return txtSched_LimitData.Text
        End Get
        Set(ByVal value As String)
            txtSched_LimitData.Text = value
        End Set
    End Property

    Public Property ScheduledLimitDesc() As String
        Get
            Return txtSched_Description.Text
        End Get
        Set(ByVal value As String)
            txtSched_Description.Text = value
        End Set
    End Property

    Public Property PersPropType() As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType
        Get
            Return ViewState("vs_PersPropType")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType)
            ViewState("vs_PersPropType") = value
        End Set
    End Property

    Public Property OptPersPropType() As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType
        Get
            Return ViewState("vs_OptPersPropType")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType)
            ViewState("vs_OptPersPropType") = value
        End Set
    End Property

    Public Property RowNumber As Int32
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Private _validatorGroupName As String
    Public Property ValidatorGroupName() As String
        Get
            Return _validatorGroupName
        End Get
        Set(ByVal value As String)
            _validatorGroupName = value
        End Set
    End Property

    Public Property TogglePeakSeason() As Boolean
        Get
            Try
                Boolean.Parse(ViewState("vs_PeakSeason"))
                Return Boolean.Parse(ViewState("vs_PeakSeason"))
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_PeakSeason") = value
        End Set
    End Property

    Public Property EarthquakeState() As Boolean
        Get
            Try
                Return Boolean.Parse(ViewState("vs_EarthquakeState"))
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_EarthquakeState") = value
        End Set
    End Property

    Public ReadOnly Property PeakSeasonCoverages() As List(Of String)
        Get
            Dim peakSeason As List(Of String) = New List(Of String)(New String() _
                                        {QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open})
            Return peakSeason
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return RowNumber
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'ctlFarmPeakSeasonList.PeakButtonPress = Nothing

        If Not IsPostBack Then
            'hiddenPeakType.Value = PersonalPropType
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim scriptLimitData As String = "FarmRoundValue100(""" + txtSched_LimitData.ClientID + """);"
        txtSched_LimitData.Attributes.Add("onblur", scriptLimitData)

        txtSched_LimitData.Attributes.Add("onfocus", "this.select()")
        txtSched_Description.Attributes.Add("onfocus", "this.select()")

        lnkDelete.OnClientClick = "ConfirmPersPropDialog(this.id, """ + hiddenPeakExists.ClientID + """, """ & hiddenPeakType.ClientID + """);"
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        ctlFarmPeakSeasonList.RowNumber = RowNumber
        '        hiddenPeakType.Value = PersonalPropType

        Dim farmPersPropSched As QuickQuoteScheduledPersonalPropertyCoverage = Nothing
        Dim farmPersPropOpt As QuickQuoteOptionalCoverage = Nothing

        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        'Updated 9/7/18 for multi state MLW - Quote to SubQuoteFirst
        'If SubQuoteFirst IsNot Nothing Then
        If GoverningStateQuote() IsNot Nothing Then
            'If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
            If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                dvPeakSeason.Attributes.Add("style", "display:none;")
                'Dim optPPList As List(Of QuickQuoteOptionalCoverage) = SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = OptPersPropType)
                Dim optPPList As List(Of QuickQuoteOptionalCoverage) = GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = OptPersPropType)

                Try
                    farmPersPropOpt = optPPList(RowNumber)
                Catch ex As Exception
                End Try

                If farmPersPropOpt IsNot Nothing AndAlso farmPersPropOpt.CoverageCodeId <> "80362" Then
                    txtSched_LimitData.Text = farmPersPropOpt.IncreasedLimit
                    txtSched_Description.Text = If((Me.IsOnAppPage OrElse IsQuoteEndorsement()) AndAlso farmPersPropOpt.Description.ToUpper().Trim() = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmOptionalCoverage(OptPersPropType), "", farmPersPropOpt.Description) ' Modified to show blank on app side if field contains default text for the coverage Matt A 9-2-15
                    divEarthquakeScheduled.Visible = False
                End If

            End If

            'If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing Then
            If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                'Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
                Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

                Try
                    farmPersPropSched = schedPPList(RowNumber)
                Catch ex As Exception
                End Try

                If farmPersPropSched IsNot Nothing Then
                    txtSched_LimitData.Text = farmPersPropSched.IncreasedLimit
                    txtSched_Description.Text = If((Me.IsOnAppPage OrElse IsQuoteEndorsement()) AndAlso farmPersPropSched.Description.ToUpper().Trim() = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(PersPropType), "", farmPersPropSched.Description) ' Modified to show blank on app side if field contains default text for the coverage Matt A 9-2-15
                    chkEarthquakeScheduled.checked = farmPersPropSched.HasEarthquakeCoverage

                    If Not PeakSeasonCoverages.Contains(farmPersPropSched.CoverageType) Then
                        dvPeakSeason.Attributes.Add("style", "display:none;")
                    Else
                        dvPeakSeason.Attributes.Add("style", "display:block;")

                        'farmPersPropSched.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, schedPPList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)
                        farmPersPropSched.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(schedPPList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber, Quote.QuoteTransactionType)
                        If farmPersPropSched.PeakSeasons IsNot Nothing Then
                            If farmPersPropSched.PeakSeasons.Count > 0 Then
                                chkPeakCovF.Checked = True
                                chkPeakCovF.Enabled = False
                                dvPeakCovF.Attributes.Add("style", "display:block;")
                                hiddenPeakExists.Value = "true"
                                ctlFarmPeakSeasonList.PersPropType = PersPropType
                            End If
                        End If
                    End If
                    If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) AndAlso farmPersPropSched.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.HasFarmExtender Then
                        dvRentedOrBorrowedEquipMsg.Visible = True
                    Else
                        dvRentedOrBorrowedEquipMsg.Visible = False
                    End If
                End If
            End If
        End If

        If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            Me.PeakNotice.Visible = True
        End If
        'ctlFarmPeakSeasonList.PersPropType = PersonalPropType
        ctlFarmPeakSeasonList.CallingControl = "ctlFarmScheduledLimit"

        PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            Dim farmPersPropSched As QuickQuoteScheduledPersonalPropertyCoverage = Nothing
            Dim farmPersPropOpt As QuickQuoteOptionalCoverage = Nothing

            'Updated 11/05/2019 for Bug 33751 MLW - sq to governingStateQuote
            'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            If GoverningStateQuote() IsNot Nothing Then
                'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                'If sq.OptionalCoverages IsNot Nothing Then
                If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                    Dim optPPList As List(Of QuickQuoteOptionalCoverage) = GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = OptPersPropType)
                    Try
                        farmPersPropOpt = optPPList(RowNumber)
                    Catch ex As Exception
                    End Try

                    If farmPersPropOpt IsNot Nothing AndAlso farmPersPropOpt.CoverageCodeId <> "80362" Then
                        farmPersPropOpt.IncreasedLimit = txtSched_LimitData.Text
                        farmPersPropOpt.Description = txtSched_Description.Text
                    End If

                    'If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    'Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = sq.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count > 0 Then
                        Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

                        Try
                            farmPersPropSched = schedPPList(RowNumber)
                        Catch ex As Exception
                        End Try

                        If farmPersPropSched IsNot Nothing Then
                            farmPersPropSched.IncreasedLimit = txtSched_LimitData.Text
                            farmPersPropSched.Description = txtSched_Description.Text
                            'farmPersPropSched.HasEarthquakeCoverage = EarthquakeState
                            farmPersPropSched.HasEarthquakeCoverage = chkEarthquakeScheduled.checked
                        End If
                    End If
                End If
                'Next
            End If
            ctlFarmPeakSeasonList.CallingControl = "ctlFarmScheduledLimit"
            SaveChildControls()
            Return True
        End If

        Return False
    End Function

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            Save_FireSaveEvent(False)
            txtSched_LimitData.Text = ""
            txtSched_Description.Text = ""
            RaiseEvent RemovePersPropItem(RowNumber)
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = ValidatorGroupName
        ctlFarmPeakSeasonList.PeakSeasonGroupValName = ValidatorGroupName
        'Dim divLimit As String = dvSchedLimit.ClientID
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valList = PersonalPropertyValidator.ValidateFARPersProp(Quote, RowNumber, 0, OptPersPropType, PersPropType, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case PersonalPropertyValidator.MissingLimit
                        ValidationHelper.Val_BindValidationItemToControl(txtSched_LimitData, v, accordList)
                    'ValidationHelper.Val_BindValidationItemToControl(txtSched_LimitData, v, divLimit, "0")
                    Case PersonalPropertyValidator.MissingDescription
                        ValidationHelper.Val_BindValidationItemToControl(txtSched_Description, v, accordList)
                        'ValidationHelper.Val_BindValidationItemToControl(txtSched_Description, v, divLimit, "0")
                End Select
            Next
        End If

        ValidateChildControls(valArgs)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        txtSched_LimitData.Text = ""
        txtSched_Description.Text = ""
        chkEarthquakeScheduled.Checked = False
    End Sub

    ''
    ' Peak Season
    ''

    Protected Sub lnkAddPeakCovF_Click(sender As Object, e As EventArgs) Handles lnkAddPeakCovF.Click
        AddNewItem()
    End Sub

    Protected Sub chkPeakCovF_CheckedChanged(sender As Object, e As EventArgs) Handles chkPeakCovF.CheckedChanged
        AddNewItem()
    End Sub

    Private Sub AddNewItem()
        If Quote IsNot Nothing Then
            'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            If GoverningStateQuote() IsNot Nothing Then
                'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                'If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    chkPeakCovF.Enabled = False
                    chkPeakCovF.Attributes.Add("style", "display:block;")
                    hiddenPeakExists.Value = "true"
                    ctlFarmPeakSeasonList.PersPropType = PersPropType
                    ctlFarmPeakSeasonList.CallingControl = "ctlFarmScheduledLimit"
                    RaiseEvent RaisePeakButtonPress("AddNewRecord!" + PersPropType.ToString() + "!" + RowNumber.ToString())
                    RaiseEvent RaiseCurrentRowNumber(RowNumber)
                    Save()
                    Save_FireSaveEvent(False)
                    'ValidateChildControls(New IFM.VR.Web.VRValidationArgs)
                    RaiseEvent RefreshAfterNewPeak()
                    RaiseEvent ToggleMoreLessPeak()
                End If
                'Next

            End If
        End If
    End Sub

    Private Sub RefreshPeakSeasonDates() Handles ctlFarmPeakSeasonList.RaiseRefreshPeakDates
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        'Updated 9/7/18 for multi state MLW - Quote to SubQuoteFirst
        'If SubQuoteFirst IsNot Nothing Then
        If GoverningStateQuote() IsNot Nothing Then
            'Dim schedulePersProp As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
            Dim schedulePersProp As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
            'If Quote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Count = 0 Then
            If schedulePersProp(RowNumber).PeakSeasons.Count = 0 Then
                chkPeakCovF.Checked = False
                chkPeakCovF.Enabled = True
                dvPeakCovF.Attributes.Add("style", "display:none;")
                hiddenPeakExists.Value = ""
            End If
        End If
        PopulateChildControls()
    End Sub

    Private Sub RemoveItem(peakSeasonRowItem As QuickQuotePeakSeason, propType As String, datePeakRow As Integer) Handles ctlFarmPeakSeasonList.RemovePeakItem
        If Quote IsNot Nothing Then
            'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
            'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            If GoverningStateQuote() IsNot Nothing Then
                'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                'If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    'Save_FireSaveEvent(False)
                    'Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = sq.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = propType)
                    Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = propType)

                    If schedPersPropList IsNot Nothing Then
                        Dim ppRowNumber As Integer = 0
                        For Each ppItem In schedPersPropList
                            If schedPersPropList(ppRowNumber).PeakSeasons.Count = 0 Then
                                chkPeakCovF.Checked = False
                                chkPeakCovF.Enabled = True
                                dvPeakCovF.Attributes.Add("style", "display:none;")
                                hiddenPeakExists.Value = ""
                            Else
                                Dim temp = RowNumber
                                'ppItem.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, schedPersPropList, New QuickQuoteUnscheduledPersonalPropertyCoverage, ppRowNumber)
                                ppItem.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(schedPersPropList, New QuickQuoteUnscheduledPersonalPropertyCoverage, ppRowNumber, Quote.QuoteTransactionType)
                                'ppItem.PeakSeasons.Remove(peakSeasonRowItem)
                                'ppItem.PeakSeasons.RemoveAt(Integer.Parse(peakSeasonRowItem.Description.Split("-")(1)) - 1)
                                If IsQuoteEndorsement() Then
                                    Dim PeakToRemove = ppItem.PeakSeasons.Find(Function(p) p.IncreasedLimit = peakSeasonRowItem.IncreasedLimit AndAlso p.Description = peakSeasonRowItem.Description AndAlso p.EffectiveDate = peakSeasonRowItem.EffectiveDate AndAlso p.ExpirationDate = peakSeasonRowItem.ExpirationDate)
                                    ppItem.PeakSeasons.Remove(PeakToRemove)
                                Else
                                    ppItem.PeakSeasons.RemoveAt(Integer.Parse(peakSeasonRowItem.Description.Split("-")(1)) - 1)
                                End If
                                'Dim ppPSDelDescSplit = peakSeasonRowItem.Description.Split("-") 
                                Session("DeleteBlankPeakButton") = "DeleteBlankPeak"

                                For Each item In schedPersPropList(ppRowNumber).PeakSeasons
                                    'Added 11/7/18 for multi state MLW - fixes exception thrown when a hyphen is not present
                                    If item.Description.Contains("-") Then
                                        Dim splitDesc = item.Description.Split("-")
                                        Dim splitDelPS = peakSeasonRowItem.Description.Split("-")
                                        If splitDesc(splitDesc.Length - 1) >= splitDelPS(splitDelPS.Length - 1) Then
                                            splitDesc(splitDesc.Length - 1) = (Integer.Parse(splitDesc(splitDesc.Length - 1)) - 1).ToString()
                                            item.Description = String.Join("-", splitDesc)
                                        End If
                                    End If
                                Next
                            End If

                            ppRowNumber += 1
                        Next

                        ctlFarmPeakSeasonList.CallingControl = "ctlFarmScheduledLimit"
                        'SaveChildControls()
                        PopulateChildControls()
                        'Save()
                        Save_FireSaveEvent(False)
                        Populate()
                        RaiseEvent ToggleMoreLessPeak()
                        Session("DeleteBlankPeakButton") = ""
                    End If

                End If
                'Next
            End If
        End If
    End Sub
End Class