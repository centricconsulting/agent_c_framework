Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlBlnktPersProp
    Inherits VRControlBase

    Public Event RemovePersPropItem(index As Integer)
    Public Event ToggleMoreLessPeak()
    Public Event RefreshBlanketPersProp()
    Public Event RaisePeakButtonPress(button As String)

    Public ReadOnly Property GetBlanketLimitValue() As String
        Get
            Return txtBlanketLimit.Text
        End Get
    End Property

    Public ReadOnly Property GetBlanketLimitClientID() As String
        Get
            Return txtBlanketLimit.ClientID
        End Get
    End Property

    Public Property PersPropType() As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType
        Get
            Return ViewState("vs_PersPropType")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType)
            ViewState("vs_PersPropType") = value
        End Set
    End Property

    Public Property OptionalPersPropType() As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType
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

    Private _PeakButtonPress As String
    Public Property PeakSeasonButtonPress() As String
        Get
            Return _PeakButtonPress
        End Get
        Set(ByVal value As String)
            _PeakButtonPress = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        ' Unscheduled Personal Property Limit
        Dim scriptUnSchedPersProp As String = "ValidateUnSchedPersPropLimit(this, """ + chkBlanketPeak.ClientID + """, """ + hiddenOldLimit.ClientID + """);"
        Dim FarmMachinerySpecialCoverageGClientID As String = DirectCast(Me.ParentVrControl, ctlFarmPersonalProperty).FarmMachinerySpecialCoverageGClientID
        Dim lblFarmMachinerySpecialCoverageGClientID As String = DirectCast(Me.ParentVrControl, ctlFarmPersonalProperty).lblFarmMachinerySpecialCoverageGClientID
        Dim scriptFarmMachinerySpecialCoverageG As String = "ToggleFarmMachinerySpecialCoverageG(this.id, """ + FarmMachinerySpecialCoverageGClientID + """, """ + lblFarmMachinerySpecialCoverageGClientID + """);"
        txtBlanketLimit.Attributes.Add("onblur", scriptUnSchedPersProp + scriptFarmMachinerySpecialCoverageG)

        Dim scriptUnSchedOldLimit As String = "SetUnSchedOldLimit(this, """ + hiddenOldLimit.ClientID + """); this.select()"
        txtBlanketLimit.Attributes.Add("onfocus", scriptUnSchedOldLimit)

        txtBlanketLimit.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val())); ToggleUnSchedPeak(this, """ + chkBlanketPeak.ClientID + """); ToggleUnSchedEarthquake(""" & txtBlanketLimit.ClientID & """,""" & chkEarthquakeBlanket.ClientID & """);")

        lnkDeleteBlanket.OnClientClick = "ConfirmPersPropDialog(this.id, """ + hiddenPeakExists.ClientID + """, """ + hiddenPeakType.ClientID + """);"
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'Updated 11/06/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                txtBlanketLimit.Text = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit
                
                'GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1)
                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1, Quote.QuoteTransactionType)

                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0 Then
                        chkBlanketPeak.Checked = True
                        chkBlanketPeak.Enabled = False
                        dvBlanketPeakSeason.Attributes.Add("style", "display:block;")
                        hiddenPeakExists.Value = "true"
                        ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = True
                        ctlFarmPeakSeasonList.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.None
                    Else
                        chkBlanketPeak.Checked = False
                        chkBlanketPeak.Enabled = True
                        dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
                        hiddenPeakExists.Value = "false"
                        ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
                    End If
                Else
                    chkBlanketPeak.Checked = False
                    chkBlanketPeak.Enabled = True
                    dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
                    hiddenPeakExists.Value = "false"
                    ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
                End If

                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit = "" Or GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit = "0" Then
                    chkBlanketPeak.Checked = False
                    chkBlanketPeak.Enabled = False
                    chkEarthquakeBlanket.Checked = False
                    chkEarthquakeBlanket.Enabled = False
                Else
                    chkEarthquakeBlanket.Enabled = True
                    chkEarthquakeBlanket.Checked = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage
                End If
            Else
                chkBlanketPeak.Checked = False
                chkBlanketPeak.Enabled = False
                dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
                hiddenPeakExists.Value = "false"
                ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
                chkEarthquakeBlanket.Checked = False
                chkEarthquakeBlanket.Enabled = False
            End If
        End If
        ''Updated 9/7/18 for multi state MLW - Quote to SubQuoteFirst
        'If SubQuoteFirst IsNot Nothing Then
        '    If SubQuoteFirst.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
        '        txtBlanketLimit.Text = SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit

        '        SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), SubQuoteFirst.UnscheduledPersonalPropertyCoverage, -1)

        '        If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
        '            If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0 Then
        '                chkBlanketPeak.Checked = True
        '                chkBlanketPeak.Enabled = False
        '                dvBlanketPeakSeason.Attributes.Add("style", "display:block;")
        '                hiddenPeakExists.Value = "true"
        '                ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = True
        '                ctlFarmPeakSeasonList.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.None
        '            Else
        '                chkBlanketPeak.Checked = False
        '                chkBlanketPeak.Enabled = True
        '                dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
        '                hiddenPeakExists.Value = "false"
        '                ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
        '            End If
        '        Else
        '            chkBlanketPeak.Checked = False
        '            chkBlanketPeak.Enabled = True
        '            dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
        '            hiddenPeakExists.Value = "false"
        '            ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
        '        End If

        '        If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit = "" Or SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit = "0" Then
        '            chkBlanketPeak.Checked = False
        '            chkBlanketPeak.Enabled = False

        '            ToggleBlanketEarthquake()
        '        Else
        '            ToggleBlanketEarthquake()
        '        End If
        '    Else
        '        chkBlanketPeak.Checked = False
        '        chkBlanketPeak.Enabled = False
        '        dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
        '        hiddenPeakExists.Value = "false"
        '        ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
        '    End If
        'End If

        If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            Me.PeakNotice.Visible = True
        End If

        ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"
        hiddenPeakType.Value = PersPropType
        PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            ' Need to save to Governing State MGB 2-1-2019 Bug 31175
            If GoverningStateQuote() IsNot Nothing Then
                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage Is Nothing Then
                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage = New QuickQuoteUnscheduledPersonalPropertyCoverage
                End If

                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit = txtBlanketLimit.Text

                ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"
                SaveChildControls()

                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.SplitPeak_EndoReady(Quote, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, PeakSeasonButtonPress, -1, -1, Quote.QuoteTransactionType)
            
                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage = chkEarthquakeBlanket.Checked
            End If

            ''Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            '        If sq.UnscheduledPersonalPropertyCoverage Is Nothing Then
            '            sq.UnscheduledPersonalPropertyCoverage = New QuickQuoteUnscheduledPersonalPropertyCoverage
            '        End If

            '        sq.UnscheduledPersonalPropertyCoverage.IncreasedLimit = txtBlanketLimit.Text

            '        ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"
            '        SaveChildControls()

            '        'Quote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), Quote.UnscheduledPersonalPropertyCoverage, -1)
            '        sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.SplitPeak(Quote, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), sq.UnscheduledPersonalPropertyCoverage, PeakSeasonButtonPress, -1, -1)
            '    Next
            'End If

            Return True
        End If

        Return False
    End Function

    Public Sub SaveEarthquakeToggle(state As Boolean)
        'Updated 11/06/2019 for Bug 33751 MLW - sq to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage = state
            End If
        End If

        ''Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
        'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
        '    For Each sq As QuickQuoteObject In SubQuotes
        '        If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
        '            sq.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage = state
        '        End If
        '    Next
        'End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = ValidatorGroupName
        ctlFarmPeakSeasonList.PeakSeasonGroupValName = "Blanket"

        ValidateChildControls(valArgs)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        txtBlanketLimit.Text = ""
        chkEarthquakeBlanket.Checked = False
        ClearChildControls()
    End Sub

    ''
    ' Peak Season
    ''
    Protected Sub CheckboxAdd(sender As Object, e As EventArgs) Handles lnkAddPeakCovG.Click, chkBlanketPeak.CheckedChanged
        If Quote IsNot Nothing Then
            'Updated 11/06/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage Is Nothing Then
                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage = New QuickQuoteUnscheduledPersonalPropertyCoverage
                End If

                Dim newPeakSeasonItem As New QuickQuotePeakSeason
                'Dim persPropList = Quote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = hiddenPeakType.Value)

                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
                End If
            End If

            ''Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            '        If sq.UnscheduledPersonalPropertyCoverage Is Nothing Then
            '            sq.UnscheduledPersonalPropertyCoverage = New QuickQuoteUnscheduledPersonalPropertyCoverage
            '        End If

            '        Dim newPeakSeasonItem As New QuickQuotePeakSeason
            '        'Dim persPropList = Quote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = hiddenPeakType.Value)

            '        If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
            '            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
            '        End If
            '    Next
            'End If

            'Quote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Add(newPeakSeasonItem)
            chkBlanketPeak.Enabled = False
            dvBlanketPeakSeason.Attributes.Add("style", "display:block;")
            hiddenPeakExists.Value = "true"
            ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = True
            ctlFarmPeakSeasonList.PersPropType = QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.None
            ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"
            'RaiseEvent RaisePeakButtonPress("AddNewRecord!ctlBlnkPersProp!0")
            PeakSeasonButtonPress = "AddNewRecord!ctlBlnkPersProp!0"

            SaveChildControls()
            PopulateChildControls()
            Save()
            Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Private Sub RemoveBlanketItem(peakSeasonRow As QuickQuotePeakSeason) Handles ctlFarmPeakSeasonList.RemovePeakBlanketItem
        If Quote IsNot Nothing Then
            'Updated 11/06/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Remove(peakSeasonRow)

                        ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = True

                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count = 0 Then
                            chkBlanketPeak.Checked = False
                            chkBlanketPeak.Enabled = True
                            dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
                            hiddenPeakExists.Value = "false"
                            ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
                        Else
                            For Each item In GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons
                                'Added 11/7/18 for multi state MLW - fixes exception thrown when a hyphen is not present
                                If item.Description.Contains("-") Then
                                    Dim splitDesc = item.Description.Split("-")
                                    Dim splitDelPS = peakSeasonRow.Description.Split("-")
                                    If splitDesc(splitDesc.Length - 1) >= splitDelPS(splitDelPS.Length - 1) Then
                                        splitDesc(splitDesc.Length - 1) = (Integer.Parse(splitDesc(splitDesc.Length - 1)) - 1).ToString()
                                        item.Description = String.Join("-", splitDesc)
                                    End If
                                End If
                            Next
                        End If

                        ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"
                        PopulateChildControls()
                        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
                        Populate()
                    End If
                End If
            End If

            ''Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            '        If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
            '            If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
            '                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons.Remove(peakSeasonRow)

            '                ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = True

            '                If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count = 0 Then
            '                    chkBlanketPeak.Checked = False
            '                    chkBlanketPeak.Enabled = True
            '                    dvBlanketPeakSeason.Attributes.Add("style", "display:none;")
            '                    hiddenPeakExists.Value = "false"
            '                    ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
            '                Else
            '                    For Each item In sq.UnscheduledPersonalPropertyCoverage.PeakSeasons
            '                        'Added 11/7/18 for multi state MLW - fixes exception thrown when a hyphen is not present
            '                        If item.Description.Contains("-") Then
            '                            Dim splitDesc = item.Description.Split("-")
            '                            Dim splitDelPS = peakSeasonRow.Description.Split("-")
            '                            If splitDesc(splitDesc.Length - 1) >= splitDelPS(splitDelPS.Length - 1) Then
            '                                splitDesc(splitDesc.Length - 1) = (Integer.Parse(splitDesc(splitDesc.Length - 1)) - 1).ToString()
            '                                item.Description = String.Join("-", splitDesc)
            '                            End If
            '                        End If
            '                    Next
            '                End If

            '                ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"
            '                PopulateChildControls()
            '                Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            '                Populate()
            '            End If
            '        End If
            '    Next
            'End If
        End If
    End Sub

    Protected Sub OnBlanketConfirm(sender As Object, e As EventArgs) Handles lnkDeleteBlanket.Click
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            ClearControl()
            'txtBlanketLimit.Text = ""
            'ClearChildControls()
            'Updated 11/06/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                GoverningStateQuote.UnscheduledPersonalPropertyCoverage = Nothing
            End If

            ''Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
            'If SubQuotes IsNot Nothing Then
            '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            '        sq.UnscheduledPersonalPropertyCoverage = Nothing
            '    Next
            'End If

            hiddenPeakExists.Value = "false"
            ctlFarmPeakSeasonList.UnscheduledPropertyPeakExists = False
            ctlFarmPeakSeasonList.CallingControl = "ctlBlnkPersProp"

            PopulateChildControls()
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
        End If
    End Sub
End Class