Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines
Imports System.Globalization
Imports IFM.PrimativeExtensions

Public Class ctlFarmPeakSeason
    Inherits VRControlBase

    Public Event RemovePeakSeasonItem(peakSeason As QuickQuotePeakSeason, propertyType As String)
    Public Event RemoveBlanketPeakSeasonItem(peakSeason As QuickQuotePeakSeason)
    Public Event GetSelectedPropertyType(peakSeason As QuickQuotePeakSeason)
    Public Event RaiseEditedPeakDateList(peakDate As List(Of List(Of QuickQuotePeakSeason)))
    Public Event RefreshPeakSeason()

    Public Property PeakLimit() As String
        Get
            Return txtPeak_LimitData.Text
        End Get
        Set(ByVal value As String)
            txtPeak_LimitData.Text = value
        End Set
    End Property

    Public Property PeakLimitStart() As String
        Get
            Return txtPeak_Start.Text
        End Get
        Set(ByVal value As String)
            txtPeak_Start.Text = value
        End Set
    End Property

    Public Property PeakLimitEnd() As String
        Get
            Return txtPeak_End.Text
        End Get
        Set(ByVal value As String)
            txtPeak_End.Text = value
        End Set
    End Property

    Public Property PeakLimitDesc() As String
        Get
            Return txtPeak_Desc.Text
        End Get
        Set(ByVal value As String)
            txtPeak_Desc.Text = value
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

    Public Property RowNumberPeak As Int32
        Get
            If ViewState("vs_rowNumberPeak") Is Nothing Then
                ViewState("vs_rowNumberPeak") = 0
            End If
            Return CInt(ViewState("vs_rowNumberPeak"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumberPeak") = value
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

    Private _CallingControl As String
    Public Property CallingControl() As String
        Get
            Return _CallingControl
        End Get
        Set(ByVal value As String)
            _CallingControl = value
        End Set
    End Property

    Public peakSeasonDateListofList As New List(Of List(Of QuickQuotePeakSeason))

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        ' #Rounding
        Dim scriptLimitData As String = "FarmRoundValue100(""" + txtPeak_LimitData.ClientID + """);"
        txtPeak_LimitData.Attributes.Add("onblur", scriptLimitData)

        Dim scriptCheckStartDate As String = "ValidateStartPeakDates(""" + txtPeak_Start.ClientID + """, """ + txtPeak_End.ClientID + """, 'Start', """ + Quote.EffectiveDate.ToString() + """);"
        txtPeak_Start.Attributes.Add("onblur", scriptCheckStartDate)
        Dim scriptCheckEndDate As String = "ValidateEndPeakDates(""" + txtPeak_Start.ClientID + """, """ + txtPeak_End.ClientID + """, 'End', """ + Quote.EffectiveDate.ToString() + """);"
        txtPeak_End.Attributes.Add("onblur", scriptCheckEndDate)

        txtPeak_LimitData.Attributes.Add("onfocus", "this.select()")
        txtPeak_Start.Attributes.Add("onfocus", "this.select()")
        txtPeak_End.Attributes.Add("onfocus", "this.select()")
        txtPeak_Desc.Attributes.Add("onfocus", "this.select()")

        VRScript.CreateTextboxMask(txtPeak_Start, "09/09")
        VRScript.CreateTextboxWaterMark(txtPeak_Start, "MM/DD")
        VRScript.CreateTextboxMask(txtPeak_End, "09/09")
        VRScript.CreateTextboxWaterMark(txtPeak_End, "MM/DD")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim farmPersPropSched As QuickQuoteScheduledPersonalPropertyCoverage = Nothing
        'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            'Updated 9/7/18 for multi state MLW - Quote to SubQuoteFirst
            'If SubQuoteFirst IsNot Nothing Then
            If CallingControl <> "ctlBlnkPersProp" Then
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    'Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
                    Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

                    Try
                        farmPersPropSched = schedPPList(RowNumber)
                    Catch ex As Exception
                    End Try

                    If farmPersPropSched IsNot Nothing Then
                        Dim farmPeakSeasons As QuickQuotePeakSeason = Nothing
                        Try
                            farmPeakSeasons = farmPersPropSched.PeakSeasons(RowNumberPeak)
                        Catch ex As Exception
                        End Try
                        If farmPersPropSched.PeakSeasons IsNot Nothing And farmPersPropSched.PeakSeasons.Count > 0 Then
                            txtPeak_LimitData.Text = farmPeakSeasons.IncreasedLimit

                            If farmPeakSeasons.EffectiveDate <> "" Then
                                Try
                                    txtPeak_Start.Text = farmPeakSeasons.EffectiveDate.Substring(0, farmPeakSeasons.EffectiveDate.Length - 5)
                                Catch ex As Exception
                                    txtPeak_Start.Text = farmPeakSeasons.EffectiveDate
                                End Try
                            End If

                            If farmPeakSeasons.ExpirationDate <> "" Then
                                Try
                                    txtPeak_End.Text = farmPeakSeasons.ExpirationDate.Substring(0, farmPeakSeasons.ExpirationDate.Length - 5)
                                Catch ex As Exception
                                    txtPeak_End.Text = farmPeakSeasons.ExpirationDate
                                End Try
                            End If

                            txtPeak_Desc.Text = farmPeakSeasons.Description
                        End If
                    End If
                End If
            Else
                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                        txtPeak_LimitData.Text = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit
                        txtPeak_Start.Text = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate
                        txtPeak_End.Text = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate
                        txtPeak_Desc.Text = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description
                    End If
                End If
                'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                '    If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                '        txtPeak_LimitData.Text = SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit
                '        txtPeak_Start.Text = SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate
                '        txtPeak_End.Text = SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate
                '        txtPeak_Desc.Text = SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description
                '    End If
                'End If
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Dim farmPersPropSched As QuickQuoteScheduledPersonalPropertyCoverage = Nothing
        'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            If CallingControl <> "ctlBlnkPersProp" Then

                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

                    Try
                        farmPersPropSched = schedPPList(RowNumber)

                        If farmPersPropSched IsNot Nothing Then
                            If farmPersPropSched.PeakSeasons Is Nothing Then
                                farmPersPropSched.PeakSeasons = New List(Of QuickQuotePeakSeason)
                            End If

                            'farmPersPropSched.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, schedPPList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)
                            farmPersPropSched.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(schedPPList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber, Quote.QuoteTransactionType)

                            farmPersPropSched.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
                            farmPersPropSched.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
                            farmPersPropSched.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
                            farmPersPropSched.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text.ToUpper()
                            'Return True '9/19/18 removed for multi state MLW
                        End If
                    Catch ex As Exception
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                            If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                                Dim blanketPeakSeasons As List(Of QuickQuotePeakSeason) = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons
                                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
                                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
                                End If

                                'GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1)
                                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1, Quote.QuoteTransactionType)

                                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0 Then 'Added 9/21/18 for multi state MLW
                                    Try
                                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
                                        If HasDateChanged(GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak), txtPeak_Start.Text, txtPeak_End.Text) Then
                                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
                                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
                                        End If
                                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text
                                    Catch

                                    End Try
                                End If
                                'Return True '9/21/18 removed for multi state MLW
                            End If
                            'Return True '9/21/18 removed for multi state MLW
                        End If
                    End Try
                Else
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                            Dim blanketPeakSeasons As List(Of QuickQuotePeakSeason) = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons
                            If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
                                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
                            End If

                            'GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1)
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1, Quote.QuoteTransactionType)

                            Try
                                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
                                If HasDateChanged(GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak), txtPeak_Start.Text, txtPeak_End.Text) Then
                                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
                                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
                                End If
                                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text
                            Catch

                            End Try
                            'Return True '9/21/18 removed for multi state MLW
                        End If
                        'Return True '9/21/18 removed for multi state MLW
                    End If
                End If
            Else
                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                        Dim blanketPeakSeasons As List(Of QuickQuotePeakSeason) = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
                        End If

                        'GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1)
                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1, Quote.QuoteTransactionType)

                        Try
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
                            If HasDateChanged(GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak), txtPeak_Start.Text, txtPeak_End.Text) Then
                                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
                                GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
                            End If
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text
                        Catch

                        End Try
                        'Return True '9/21/18 removed for multi state MLW
                    End If
                    'Return True '9/21/18 removed for multi state MLW
                End If
            End If
            Return True 'Added 9/21/18 for multi state MLW
        End If
        Return False
    End Function

    'Changed to above for Bug 33751 MLW - sq to GoverningStateQuote
    'Public Overrides Function Save() As Boolean
    '    Dim farmPersPropSched As QuickQuoteScheduledPersonalPropertyCoverage = Nothing
    '    'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
    '    If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
    '        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
    '            If CallingControl <> "ctlBlnkPersProp" Then

    '                If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
    '                    Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = sq.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

    '                    Try
    '                        farmPersPropSched = schedPPList(RowNumber)

    '                        If farmPersPropSched IsNot Nothing Then
    '                            If farmPersPropSched.PeakSeasons Is Nothing Then
    '                                farmPersPropSched.PeakSeasons = New List(Of QuickQuotePeakSeason)
    '                            End If

    '                            farmPersPropSched.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, schedPPList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)

    '                            farmPersPropSched.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
    '                            farmPersPropSched.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
    '                            farmPersPropSched.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
    '                            farmPersPropSched.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text.ToUpper()
    '                            'Return True '9/19/18 removed for multi state MLW
    '                        End If
    '                    Catch ex As Exception
    '                        If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
    '                            If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
    '                                Dim blanketPeakSeasons As List(Of QuickQuotePeakSeason) = sq.UnscheduledPersonalPropertyCoverage.PeakSeasons
    '                                If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
    '                                    sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
    '                                End If

    '                                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), sq.UnscheduledPersonalPropertyCoverage, -1)

    '                                If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0 Then 'Added 9/21/18 for multi state MLW
    '                                    Try
    '                                        sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
    '                                        sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
    '                                        sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
    '                                        sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text
    '                                    Catch

    '                                    End Try
    '                                End If
    '                                'Return True '9/21/18 removed for multi state MLW
    '                            End If
    '                            'Return True '9/21/18 removed for multi state MLW
    '                        End If
    '                    End Try
    '                Else
    '                    If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
    '                        If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
    '                            Dim blanketPeakSeasons As List(Of QuickQuotePeakSeason) = sq.UnscheduledPersonalPropertyCoverage.PeakSeasons
    '                            If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
    '                                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
    '                            End If

    '                            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), sq.UnscheduledPersonalPropertyCoverage, -1)

    '                            Try
    '                                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
    '                                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
    '                                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
    '                                sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text
    '                            Catch

    '                            End Try
    '                            'Return True '9/21/18 removed for multi state MLW
    '                        End If
    '                        'Return True '9/21/18 removed for multi state MLW
    '                    End If
    '                End If
    '            Else
    '                If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
    '                    If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
    '                        Dim blanketPeakSeasons As List(Of QuickQuotePeakSeason) = sq.UnscheduledPersonalPropertyCoverage.PeakSeasons
    '                        If sq.UnscheduledPersonalPropertyCoverage.PeakSeasons Is Nothing Then
    '                            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = New List(Of QuickQuotePeakSeason)
    '                        End If

    '                        sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), sq.UnscheduledPersonalPropertyCoverage, -1)

    '                        Try
    '                            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).IncreasedLimit = txtPeak_LimitData.Text
    '                            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).EffectiveDate = txtPeak_Start.Text
    '                            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).ExpirationDate = txtPeak_End.Text
    '                            sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak).Description = txtPeak_Desc.Text
    '                        Catch

    '                        End Try
    '                        'Return True '9/21/18 removed for multi state MLW
    '                    End If
    '                    'Return True '9/21/18 removed for multi state MLW
    '                End If
    '            End If
    '        Next
    '        Return True 'Added 9/21/18 for multi state MLW
    '    End If
    '    Return False
    'End Function

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            Save_FireSaveEvent(False)
            Dim persProptypeList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = New List(Of QuickQuoteScheduledPersonalPropertyCoverage)

            'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    persProptypeList = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
                End If

                ClearControl()

                If persProptypeList IsNot Nothing And persProptypeList.Count > 0 Then
                    If persProptypeList(RowNumber).PeakSeasons IsNot Nothing Then
                        'Dim persPropPeakSeason As List(Of QuickQuotePeakSeason) = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, persProptypeList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)
                        Dim persPropPeakSeason As List(Of QuickQuotePeakSeason) = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(persProptypeList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber, Quote.QuoteTransactionType)

                        Try
                            RaiseEvent RemovePeakSeasonItem(persPropPeakSeason(RowNumberPeak), PersPropType)
                        Catch ex As Exception
                        End Try

                        RaiseEvent RefreshPeakSeason()
                    End If
                Else
                    'GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1)
                    GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(New List(Of QuickQuoteScheduledPersonalPropertyCoverage), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1, Quote.QuoteTransactionType)

                    RaiseEvent RemoveBlanketPeakSeasonItem(GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak))
                End If
            End If
        End If
    End Sub

    'Changed to above for Bug 33751 MLW - sq to GoverningStateQuote
    'Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDelete.Click
    '    Dim confirmValue As String = Request.Form("confirmValue")

    '    If confirmValue = "Yes" Then
    '        Save_FireSaveEvent(False)
    '        Dim persProptypeList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = New List(Of QuickQuoteScheduledPersonalPropertyCoverage)

    '        'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
    '        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
    '            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
    '                If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
    '                    persProptypeList = sq.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
    '                End If

    '                ClearControl()

    '                If persProptypeList IsNot Nothing And persProptypeList.Count > 0 Then
    '                    If persProptypeList(RowNumber).PeakSeasons IsNot Nothing Then
    '                        Dim persPropPeakSeason As List(Of QuickQuotePeakSeason) = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, persProptypeList, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)

    '                        Try
    '                            RaiseEvent RemovePeakSeasonItem(persPropPeakSeason(RowNumberPeak), PersPropType)
    '                        Catch ex As Exception
    '                        End Try

    '                        RaiseEvent RefreshPeakSeason()
    '                    End If
    '                Else
    '                    sq.UnscheduledPersonalPropertyCoverage.PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, New List(Of QuickQuoteScheduledPersonalPropertyCoverage), sq.UnscheduledPersonalPropertyCoverage, -1)

    '                    RaiseEvent RemoveBlanketPeakSeasonItem(sq.UnscheduledPersonalPropertyCoverage.PeakSeasons(RowNumberPeak))
    '                End If
    '            Next
    '        End If
    '    End If
    'End Sub

    Private Function HasDateChanged(peakSeason As QuickQuotePeakSeason, startText As String, endText As String) As Boolean
        If peakSeason IsNot Nothing Then
            If peakSeason.EffectiveDate.IsDate AndAlso peakSeason.ExpirationDate.IsDate Then
                Dim peakStartDate As DateTime = DateTime.Parse(peakSeason.EffectiveDate)
                Dim peakExpireDate As DateTime = DateTime.Parse(peakSeason.ExpirationDate)
                Dim peakStartDateFormatted As String = peakStartDate.ToString("MM/dd", CultureInfo.InvariantCulture)
                Dim peakExpireDateFormatted As String = peakExpireDate.ToString("MM/dd", CultureInfo.InvariantCulture)
                If peakStartDateFormatted.Equals(startText) AndAlso peakExpireDateFormatted.Equals(endText) Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = ValidatorGroupName
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        'Dim divLimit As String = dvPeakSeason.ClientID
        'Dim valList = Nothing

        If ValidatorGroupName IsNot Nothing Then
            If ValidatorGroupName.Substring(0, 7) = "Blanket" Then
                Dim valList = PersonalPropertyValidator.ValidateFARPersPropBlnkt(Quote, RowNumber, RowNumberPeak, QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.None, PersPropType, valArgs.ValidationType)
                ControlValidation(valList, accordList)
            Else
                Dim valList = PersonalPropertyValidator.ValidateFARPersProp(Quote, RowNumber, RowNumberPeak, QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.None, PersPropType, valArgs.ValidationType)
                ControlValidation(valList, accordList)
            End If
        End If
    End Sub

    Private Sub ControlValidation(ByRef valList As Validation.ObjectValidation.ValidationItemList, ByRef accordList As List(Of VRAccordionTogglePair))
        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case PersonalPropertyValidator.MissingPeakLimit
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_LimitData, v, accordList)
                    Case PersonalPropertyValidator.MissingPeakStart
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_Start, v, accordList)
                    Case PersonalPropertyValidator.MissingPeakEnd
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_End, v, accordList)
                    Case PersonalPropertyValidator.MissingPeakDesc
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_Desc, v, accordList)
                    Case PersonalPropertyValidator.InvalidDateRange
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_End, v, accordList)
                    Case PersonalPropertyValidator.InvalidStartPeakMonth
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_Start, v, accordList)
                    Case PersonalPropertyValidator.InvalidStartPeakDay
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_Start, v, accordList)
                    Case PersonalPropertyValidator.InvalidEndPeakMonth
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_End, v, accordList)
                    Case PersonalPropertyValidator.InvalidEndPeakDay
                        ValidationHelper.Val_BindValidationItemToControl(txtPeak_End, v, accordList)
                End Select
            Next
        End If
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        txtPeak_LimitData.Text = ""
        txtPeak_Start.Text = ""
        txtPeak_End.Text = ""
        txtPeak_Desc.Text = ""
    End Sub
End Class