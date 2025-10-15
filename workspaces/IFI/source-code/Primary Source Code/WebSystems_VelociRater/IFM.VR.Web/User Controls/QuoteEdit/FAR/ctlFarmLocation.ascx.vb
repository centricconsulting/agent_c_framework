Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.FARM
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlFarmLocation
    Inherits VRControlBase

    Public Event NewLocationRequested()
    Public Event RefreshLocation()
    Public Event RaiseLocationSave()
    Public Event RaiseLocationRate()
    Public Event SetActivePanel(activePanel As String)
    Public Event RequestNavigationToPersonalProperty()
    Public Event GL9Changed(ByVal ClearIMRV As Boolean)

    Public Property LocationAccordionDivId As String
        Get
            If ViewState("vs_LocationAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_LocationAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_LocationAccordionDivId_") = value
        End Set
    End Property

    Public Property ResidenceAccordionDivId As String
        Get
            If ViewState("vs_ResidenceAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_ResidenceAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_ResidenceAccordionDivId_") = value
        End Set
    End Property

    Public Property InitialResidenceAccordionDivId As String
        Get
            If ViewState("vs_InitialResidenceAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_InitialResidenceAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_InitialResidenceAccordionDivId_") = value
        End Set
    End Property

    Public Property InitialBuildingAccordionDivId As String
        Get
            If ViewState("vs_InitialBuildingAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_InitialBuildingAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_InitialBuildingAccordionDivId_") = value
        End Set
    End Property

    Public Property FarmLocationIndex As Int32
        Get
            Return Session("sess_FarmLocationIndex")
        End Get
        Set(value As Int32)
            Session("sess_FarmLocationIndex") = value
        End Set
    End Property

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

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return MyLocationIndex
        End Get
    End Property

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property PolicyHolderType() As String
        Get
            If Quote.Policyholder.Name.TypeId IsNot Nothing Then
                Return Quote.Policyholder.Name.TypeId
            Else
                Return "1"
            End If
        End Get
    End Property

    Public ReadOnly Property ProgramType() As String
        Get
            If MyFarmLocation IsNot Nothing Then
                Return MyFarmLocation(MyLocationIndex).ProgramTypeId
            Else
                Return "6"
            End If
        End Get
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

    Public Property ActiveLocationIndex As String
        Get
            Return hiddenLocation.Value
        End Get
        Set(value As String)
            hiddenLocation.Value = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'MainAccordionDivId = dvFarmLocation.ClientID
        InitialResidenceAccordionDivId = dvInitRes.ClientID
        'ctlPropertyAdditionalQuestions.MyLocationIndex = MyLocationIndex

        If Not IsPostBack Then
            PopulateLocationHeader()
            LoadStaticData()
        End If
        'Removed 12/3/18 for new jQuery mine sub code MLW
        ''Added 10/25/18 for multi state MLW
        'AddHandler ctlProperty_Address.PropertyAddressChanged, AddressOf HandlePropertyAddressChange
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'VRScript.CreateAccordion(MainAccordionDivId, hiddenLocation, "0", False)
        VRScript.CreateAccordion(InitialResidenceAccordionDivId, hiddenActiveResidence, "false", True)
        VRScript.StopEventPropagation(lnkNewLocation.ClientID, True)
        VRScript.StopEventPropagation(lnkDeleteLocation.ClientID, True)
        VRScript.StopEventPropagation(lnkSaveLocation.ClientID, True)
        VRScript.CreateConfirmDialog(lnkClearLocation.ClientID, "Clear Location " & (MyLocationIndex + 1) & "?")
        VRScript.StopEventPropagation(lnkAddNewResidence.ClientID, True)
        Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", MyLocationIndex, Me.lblMainHeader.ClientID)) 'used to set the address text in this header - used by residence_address control
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate() ' Handles ctlProperty_Address.PopulateLocationHeader
        If MyFarmLocation IsNot Nothing Then

            PopulateLocationHeader()
            LoadStaticData()

            ctlPropertyAdditionalQuestions.MyLocationIndex = MyLocationIndex

            If MyLocationIndex > 0 Then
                lnkDeleteLocation.Visible = True
                lblMainHeader.Width = 325
            End If

            If ProgramType = "6" Then ' FO Type
                If Farm_General.hasAdditionalQuestionsForFarmItemNumberOfUnitsUpdate(Quote) = False Then
                    If MyLocationIndex > 0 Then
                        ctlPropertyAdditionalQuestions.Visible = False
                    End If
                End If

                If MyLocationIndex > 0 And (MyFarmLocation(MyLocationIndex).FormTypeId = "13" Or MyFarmLocation(MyLocationIndex).FormTypeId = "") Then
                        dvInitRes.Visible = True
                        ctlResidence.Visible = False
                        ctlResidence.ResidenceExists = False
                    Else
                        dvInitRes.Visible = False
                        ctlResidence.Visible = True
                        ctlResidence.ResidenceExists = True
                    End If

                Else
                    ' 02/11/2021 CAH - Task 59605
                    If IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
                    MyFarmLocation(MyLocationIndex).FormTypeId = "13"
                End If
                dvInitRes.Visible = False
                ctlResidence.Visible = False
                ctlPropertyAdditionalQuestions.Visible = False
            End If

            ' Only show Personal Liability (GL-9) when programtype is 6 or 8 and policy is commercial
            If IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.QuoteIsEligibleForGL9(Quote) Then
                ctlPersLiabGL9.Visible = True
                ctlPersLiabGL9.MyLocationIndex = MyLocationIndex
            Else
                ctlPersLiabGL9.Visible = False
            End If

            ctlResidence.MyLocationIndex = MyLocationIndex
            ctlProperty_Address.MyLocationIndex = MyLocationIndex
            ctl_FarBuildingList.MyLocationIndex = MyLocationIndex
            PopulateChildControls()

            '' Uncomment when ready to work on Buildings BRD
            ''
            'RefreshFarmBuilding()
        End If
    End Sub

    Public Sub UpdateGL9Controls()
        ctlPersLiabGL9.UpdateGL9Confirmations()
    End Sub

    Private Sub HandleGL9Change(ByVal ClearIMRV As Boolean) Handles ctlPersLiabGL9.GL9Changed
        ' When a GL9 changes we need to execute some code in each GL9 control to set their dialog texts and other options
        RaiseEvent GL9Changed(ClearIMRV)  ' just bubble the event up to the locations control so it can execute the update on each location
    End Sub

    Private Sub SetResidenceExistance(state As Boolean) Handles ctlResidence.RaiseResidenceExists
        ctlProperty_Address.ResidenceExists = state
    End Sub

    Public Sub PopulateLocationHeader() Handles ctlProperty_Address.PopulateLocationHeader
        'If MyFarmLocation(MyLocationIndex).AcreageOnly Then
        '    lblMainHeader.Text = String.Format("Acreage Only LOCATION #{0} - {1} {2}, {3}", MyLocationIndex + 1,
        '                   ctlProperty_Address.StreetAddressNum, ctlProperty_Address.StreetAddressName, ctlProperty_Address.City)
        '    ctlProperty_Address.ToggleAcresOnly = True
        'Else

        If MyFarmLocation(MyLocationIndex).Address IsNot Nothing Then
            'Updated 9/12/18 for multi state MLW - throws exception - Throws error on Acreages(0) when acreages count is 0.
            If MyFarmLocation(MyLocationIndex).Acreages IsNot Nothing AndAlso MyFarmLocation(MyLocationIndex).Acreages.Count > 0 Then
                'MyFarmLocation(MyLocationIndex).Acreages IsNot Nothing Then
                lblMainHeader.Text = String.Format("LOCATION #{0} - {1} {2}, {3} S/T/R: {4}/{5}/{6}", MyLocationIndex + 1,
                                                   MyFarmLocation(MyLocationIndex).Address.HouseNum, MyFarmLocation(MyLocationIndex).Address.StreetName, MyFarmLocation(MyLocationIndex).Address.City,
                                                   MyFarmLocation(MyLocationIndex).Acreages(0).Section, MyFarmLocation(MyLocationIndex).Acreages(0).Twp, MyFarmLocation(MyLocationIndex).Acreages(0).Range)
            Else
                lblMainHeader.Text = String.Format("LOCATION #{0} - {1} {2}, {3} S/T/R: {4}/{5}/{6}", MyLocationIndex + 1,
                                   MyFarmLocation(MyLocationIndex).Address.HouseNum, MyFarmLocation(MyLocationIndex).Address.StreetName, MyFarmLocation(MyLocationIndex).Address.City,
                                   "0", "0", "0")
            End If
        End If
        'ctlProperty_Address.ToggleAcresOnly = False
        'End If
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls() ' must always save - always allow these messages to bubble if you need something not to save then add that logic at that level

        Return False

    End Function

    Protected Sub SaveQuote()
        Try
            'Updated 9/7/18 for multi state MLW
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                    If sq.LobType = QuickQuoteObject.QuickQuoteLobType.Farm And sq.EntityTypeId = "" Then
                        sq.EntityTypeId = "1"
                    End If
                Next
            End If
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))
        Catch ex As Exception

        End Try

        Populate()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Location #{0}", MyLocationIndex + 1)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub btnAddLocation_Click(sender As Object, e As EventArgs) Handles btnAddLocation.Click, lnkNewLocation.Click
        Session("valuationValue") = "False"
        RaiseEvent NewLocationRequested()
    End Sub

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDeleteLocation.Click
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            Save_FireSaveEvent(False)

            IFM.VR.Common.Helpers.FARM.FarmBuildingHelper.RemoveFarmLocation(Quote, MyLocationIndex)
            'Dim farmLocation As QuickQuoteLocation = MyFarmLocation(MyLocationIndex)
            'Quote.Locations.Remove(farmLocation)

            RaiseEvent RefreshLocation()

            Save_FireSaveEvent(False)

            RaiseEvent SetActivePanel("false")
        End If
    End Sub

    Protected Sub lnkSaveLocation_Click(sender As Object, e As EventArgs) Handles lnkSaveLocation.Click
        Session("valuationValue") = "False"
        SaveQuote()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Session("valuationValue") = "False"
        SaveQuote()
    End Sub

    Protected Sub lnkClearLocation_Click(sender As Object, e As EventArgs) Handles lnkClearLocation.Click
        ClearControl()
        'force edit mode so they have to save at some point before leaving
        LockTree()
    End Sub

    Protected Sub lnkAddNewResidence_Click(sender As Object, e As EventArgs) Handles lnkAddNewResidence.Click
        Session("valuationValue") = "False"
        If MyFarmLocation IsNot Nothing Then
            ctlResidence.ResidenceExists = True
            ctlResidence.SetCovCReplaceCost = True

            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            dvInitRes.Visible = False
            ctlResidence.Visible = True
            ctlResidence.Populate()
        End If
    End Sub
    'Removed 12/3/18 for new jQuery mine sub code MLW
    ''Added 10/25/18 for multi state MLW
    'Private Sub HandlePropertyAddressChange()
    '    Me.ctlResidence.LoadMineSubCoverages()
    '    PopulateLocationHeader()
    'End Sub

    Private Sub HideDwellingInfo() Handles ctlResidence.HideDwelling
        ctlResidence.Visible = False
        dvInitRes.Visible = True
        ctlResidence.ResidenceExists = False
    End Sub

    Public Sub ClearDwelling()
        Me.ctlResidence.DeleteResidence()
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        Me.ctl_FarBuildingList.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
    End Sub
End Class