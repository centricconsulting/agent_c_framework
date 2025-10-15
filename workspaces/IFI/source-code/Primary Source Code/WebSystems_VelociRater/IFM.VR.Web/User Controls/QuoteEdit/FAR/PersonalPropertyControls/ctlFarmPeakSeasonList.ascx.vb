Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlFarmPeakSeasonList
    Inherits VRControlBase

    Public Event RemovePeakItem(peakSeasonRow As QuickQuotePeakSeason, persPropType As String, rowPeakDate As Integer)
    Public Event RemovePeakBlanketItem(peakSeasonRow As QuickQuotePeakSeason)
    Public Event RaiseRefreshPeakDates()

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
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

    Private _PeakSeasonGroupValName As String
    Public Property PeakSeasonGroupValName() As String
        Get
            Return _PeakSeasonGroupValName
        End Get
        Set(ByVal value As String)
            _PeakSeasonGroupValName = value
        End Set
    End Property

    Private _UnscheduledPropertyPeakExists As Boolean
    Public Property UnscheduledPropertyPeakExists() As Boolean
        Get
            Return _UnscheduledPropertyPeakExists
        End Get
        Set(ByVal value As Boolean)
            _UnscheduledPropertyPeakExists = value
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
            Populate()
        End If

        AttachCoverageControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            Dim peakSeasonDataSource = Nothing
            Dim farmPersPropSched As QuickQuoteScheduledPersonalPropertyCoverage = Nothing

            Try
                'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
                If GoverningStateQuote() IsNot Nothing Then
                    'Updated 9/7/18 for multi state MLW - Quote to SubQuoteFirst
                    'If SubQuoteFirst IsNot Nothing Then
                    If UnscheduledPropertyPeakExists Then
                        'peakSeasonDataSource = SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons
                        peakSeasonDataSource = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons
                    Else
                        'If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                        If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                            'Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
                            Dim schedPPList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

                            farmPersPropSched = schedPPList.GetItemAtIndex(RowNumber)

                            If farmPersPropSched IsNot Nothing Then
                                If farmPersPropSched.PeakSeasons Is Nothing Then
                                    farmPersPropSched.PeakSeasons = New List(Of QuickQuotePeakSeason)
                                End If
                            End If

                            peakSeasonDataSource = If(farmPersPropSched IsNot Nothing, farmPersPropSched.PeakSeasons, Nothing)
                        End If
                    End If

                    PeakSeasonRepeater.DataSource = peakSeasonDataSource

                    If PeakSeasonRepeater.DataSource IsNot Nothing Then
                        PeakSeasonRepeater.DataBind()
                        FindChildVrControls()

                        For Each child In ChildVrControls
                            If TypeOf child Is ctlFarmPeakSeason Then
                                Dim c As ctlFarmPeakSeason = child
                                c.Populate()
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Protected Sub AttachCoverageControlEvents()
        For Each cntrl As RepeaterItem In PeakSeasonRepeater.Items
            Dim increasedLimitControl As ctlFarmPeakSeason = cntrl.FindControl("ctlFarmPeakSeason")
            AddHandler increasedLimitControl.RemovePeakSeasonItem, AddressOf RemovePeakSeasonItem
            AddHandler increasedLimitControl.RemoveBlanketPeakSeasonItem, AddressOf RemoveBlanketPeakSeasonItem
            AddHandler increasedLimitControl.RaiseEditedPeakDateList, AddressOf CombinePeakSeasonDates
            AddHandler increasedLimitControl.RefreshPeakSeason, AddressOf RefreshPeakDate
        Next
    End Sub

    Private Sub RefreshPeakDate()
        RaiseEvent RaiseRefreshPeakDates()
    End Sub

    Private Sub CombinePeakSeasonDates(peakDateList As List(Of List(Of QuickQuotePeakSeason)))
        Dim temp = peakDateList

        'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            GoverningStateQuote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons = New List(Of QuickQuotePeakSeason)
            For Each itemList As List(Of QuickQuotePeakSeason) In peakDateList
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Count = 0 Then
                    GoverningStateQuote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons = itemList
                Else
                    GoverningStateQuote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Concat(itemList)
                End If
            Next
        End If
        'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
        'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
        '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
        '        sq.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons = New List(Of QuickQuotePeakSeason)
        '        For Each itemList As List(Of QuickQuotePeakSeason) In peakDateList
        '            If sq.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Count = 0 Then
        '                sq.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons = itemList
        '            Else
        '                sq.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Concat(itemList)
        '            End If
        '        Next
        '    Next
        'End If
    End Sub

    Private Sub RemovePeakSeasonItem(peakSeason As QuickQuotePeakSeason, propertyType As String)
        RaiseEvent RemovePeakItem(peakSeason, propertyType, RowNumberPeak)
    End Sub

    Private Sub RemoveBlanketPeakSeasonItem(peakSeason As QuickQuotePeakSeason)
        RaiseEvent RemovePeakBlanketItem(peakSeason)
    End Sub

    Private Sub PeakSeasonRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles PeakSeasonRepeater.ItemDataBound
        Dim increasedLimitControl As ctlFarmPeakSeason = e.Item.FindControl("ctlFarmPeakSeason")
        increasedLimitControl.RowNumber = RowNumber
        RowNumberPeak = e.Item.ItemIndex
        increasedLimitControl.RowNumberPeak = RowNumberPeak
        increasedLimitControl.PersPropType = PersPropType
        increasedLimitControl.CallingControl = CallingControl
        increasedLimitControl.Populate()
        'PopulateChildControls()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        For Each cntrl As RepeaterItem In PeakSeasonRepeater.Items
            Dim increasedLimitControl As ctlFarmPeakSeason = cntrl.FindControl("ctlFarmPeakSeason")
            increasedLimitControl.PersPropType = PersPropType
            increasedLimitControl.ValidatorGroupName = PeakSeasonGroupValName & " - Peak Season"
        Next
        ValidateChildControls(valArgs)
    End Sub
End Class