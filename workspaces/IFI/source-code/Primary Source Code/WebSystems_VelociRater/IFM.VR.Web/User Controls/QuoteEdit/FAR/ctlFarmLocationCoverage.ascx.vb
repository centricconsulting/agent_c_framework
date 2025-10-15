Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlFarmLocationCoverage
    Inherits VRControlBase

    Public Event QuoteSaveRequested()
    Public Event QuoteRateRequested()
    Public Event QuotePopulateRequested()
    Public Event ReqNavFarmPP()
    Public Event RequestIMNavigation()
    Public Event GL9Changed(ByVal ClearIMRV As Boolean)

    Public Property ActiveLocationIndex As String
        Get
            Return hiddenActiveLocation.Value
        End Get
        Set(value As String)
            hiddenActiveLocation.Value = value
            ctlFarmLocationList.ActiveFarmLocationIndex = value
        End Set
    End Property

    Public Property RouteToUwIsVisible As Boolean 'added 1/7/2020 (Interoperability)
        Get
            Return Me.ctlFarmLocationList.RouteToUwIsVisible
        End Get
        Set(value As Boolean)
            Me.ctlFarmLocationList.RouteToUwIsVisible = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctlFarmLocationList.GL9Changed, AddressOf HandleGL9Changed
    End Sub

    Private Sub HandleGL9Changed(ByVal ClearIMRV As Boolean)
        RaiseEvent GL9Changed(ClearIMRV)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate() Handles ctlFarmLocationList.RefreshFarmLoc
        PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Private Sub SaveFarmLocation() Handles ctlFarmLocationList.SaveFarmLoc
        SaveQuote()
    End Sub

    Protected Sub SaveQuote()
        Try
            RaiseEvent QuoteSaveRequested()
            UnlockTree()
            'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            If ActiveLocationIndex = "" Then
                ActiveLocationIndex = "false"
            End If

            SetActivePanel(ActiveLocationIndex)
        Catch ex As Exception

        End Try

        RaiseEvent QuotePopulateRequested()
        'Populate()
    End Sub

    Private Sub SetActivePanel(activePanel As String) Handles ctlFarmLocationList.ActivePanel
        ActiveLocationIndex = activePanel
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidateChildControls(valArgs)
    End Sub

    Private Sub RateFarmQuote() Handles ctlFarmLocationList.RateFarmLoc
        RaiseEvent QuoteRateRequested()
    End Sub

    Private Sub NavFarmPP() Handles ctlFarmLocationList.NavigatePersonalProp
        RaiseEvent ReqNavFarmPP()
    End Sub

    Public Sub CreateNewLocationFromTree()
        ctlFarmLocationList.LocationControlNewLocation()
    End Sub

    Public Sub ClearDwellingFromTree(locationIdx As Integer)
        ctlFarmLocationList.ClearDwellingbyIndex(locationIdx)
    End Sub

    Private Sub RequestIMNavication() Handles ctlFarmLocationList.RequestIMNav
        RaiseEvent RequestIMNavigation()
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        Me.ctlFarmLocationList.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Exit Sub
    End Sub

End Class