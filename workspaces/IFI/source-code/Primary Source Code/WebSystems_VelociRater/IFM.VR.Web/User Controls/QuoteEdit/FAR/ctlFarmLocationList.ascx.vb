Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlFarmLocationList
    Inherits VRControlBase

    Public Event RefreshFarmLoc()
    Public Event SaveFarmLoc()
    Public Event RateFarmLoc()
    Public Event ActivePanel(panel As String)
    Public Event NavigatePersonalProp()
    Public Event RequestIMNav()
    Public Event GL9Changed(ByVal ClearIMRV As Boolean)

    Public Property ActiveFarmLocationIndex As String
        Get
            Return hiddenLocationListActive.Value
        End Get
        Set(value As String)
            hiddenLocationListActive.Value = value
        End Set
    End Property

    Private ReadOnly Property MyFarmLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ProgramType() As String
        Get
            If MyFarmLocation IsNot Nothing Then
                Return MyFarmLocation.ProgramTypeId
            Else
                Return "6"
            End If
        End Get
    End Property

    Public Property RouteToUwIsVisible As Boolean 'added 12/3/2020 (Interoperability)
        Get
            Return Me.ctl_RouteToUw.Visible
        End Get
        Set(value As Boolean)
            Me.ctl_RouteToUw.Visible = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ListAccordionDivId = dvFarmLocations.ClientID
            LoadStaticData()
            'Populate() If you need these at startup then you are doing something wrong. These cause needless double, triple or more Populate() calls.
        End If

        AttachFarmLocationControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.AddVariableLine("var FarmLocationListControlDivTopMost = '" + hiddenLocationListActive.ClientID + "';")
        VRScript.AddVariableLine("function ShowFarmLocation(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}")
        VRScript.CreateAccordion(ListAccordionDivId, hiddenLocationListActive, "0")

        ' Checks to see of this is a FL policy
        If ProgramType = "8" Then
            btnPersonalProperty.Visible = False
            btnIMRVPage.Visible = False
        End If
    End Sub

    Protected Sub AttachFarmLocationControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In FarmLocationRepeater.Items
            Dim locationControl As ctlFarmLocation = cntrl.FindControl("ctlFarmLocationItem")
            AddHandler locationControl.NewLocationRequested, AddressOf LocationControlNewLocation
            AddHandler locationControl.RefreshLocation, AddressOf RefreshLocation
            AddHandler locationControl.RaiseLocationSave, AddressOf SaveLocation
            AddHandler locationControl.RaiseLocationRate, AddressOf RateLocation
            AddHandler locationControl.SetActivePanel, AddressOf SetActivePanel
            AddHandler locationControl.RequestNavigationToPersonalProperty, AddressOf NavPersonalProp
            AddHandler locationControl.GL9Changed, AddressOf HandleGL9Change
        Next
    End Sub

    Public Sub LocationControlNewLocation()
        If Quote IsNot Nothing Then
            If Quote.Locations Is Nothing Then
                Quote.Locations = New List(Of QuickQuoteLocation)()
            End If

            Dim newLocation As New QuickQuoteLocation()
            newLocation.ProgramTypeId = Quote.Locations(0).ProgramTypeId

            'Updated 10/4/2022 for bug 75312
            If Quote.Locations(0).HobbyFarmCredit = False OrElse (Quote.Locations(0).HobbyFarmCredit = True AndAlso Quote.Locations(0).FormTypeId <> "17") Then
                newLocation.FormTypeId = Quote.Locations(0).FormTypeId
            End If
            ''Added 01/15/2019 for bug 41281 MLW
            'newLocation.FormTypeId = Quote.Locations(0).FormTypeId
            Dim SectionICoverage As QuickQuoteSectionICoverage = Nothing
            If newLocation.SectionICoverages Is Nothing Then
                newLocation.SectionICoverages = New List(Of QuickQuoteSectionICoverage)
            End If
            ' 70117 - Replacement Cost - Cov C
            If newLocation.FormTypeId <> "13" Then
                If newLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_).Count <= 0 Then
                    SectionICoverage = New QuickQuoteSectionICoverage()
                    SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_
                    newLocation.SectionICoverages.Add(SectionICoverage)
                End If
            End If


            Quote.Locations.Add(newLocation)
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
            hiddenLocationListActive.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Private Sub NavPersonalProp()
        RaiseEvent NavigatePersonalProp()
    End Sub

    Public Sub RefreshLocation()
        RaiseEvent RefreshFarmLoc()
    End Sub

    Private Sub SaveLocation()
        RaiseEvent SaveFarmLoc()
    End Sub

    Private Sub RateLocation()
        RaiseEvent RateFarmLoc()
    End Sub

    Private Sub SetActivePanel(activePanel As String)
        RaiseEvent activePanel(activePanel)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub FarmLocationRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles FarmLocationRepeater.ItemDataBound
        'This was causing double populations - this logic is done below in Populate()
        'If MyFarmLocation IsNot Nothing Then
        '    If e.Item.ItemIndex < Quote.Locations.Count Then
        '        Dim locationControl As ctlFarmLocation = e.Item.FindControl("ctlFarmLocation")
        '        locationControl.MyLocationIndex = e.Item.ItemIndex
        '        locationControl.Populate()
        '    End If
        'End If
    End Sub

    Public Overrides Sub Populate()
        FarmLocationRepeater.DataSource = Nothing

        If Quote.Locations IsNot Nothing Then
            FarmLocationRepeater.DataSource = Quote.Locations
            FarmLocationRepeater.DataBind()

            FindChildVrControls() ' finds the just added controls do to the binding
            Dim index As Int32 = 0
            For Each child In Me.ChildVrControls
                If TypeOf child Is ctlFarmLocation Then
                    Dim c As ctlFarmLocation = child
                    c.MyLocationIndex = index
                    c.Populate()
                    index += 1
                End If
            Next
        End If

        If IsQuoteReadOnly() Then
            btnRateLocation.Visible = False
            btnMakeAChange.Visible = True
            Dim policyNumber As String = Me.Quote.PolicyNumber
            Dim imageNum As Integer = 0
            Dim policyId As Integer = 0
            Dim toolTip As String = "Make a change to this policy"
            'Dim qqHelper As New QuickQuoteHelperClass
            Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
            'QuickQuoteHelperClass.configAppSettingValueAsString("")  'Unused CAH 07/21/2020
            If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
            End If

            btnMakeAChange.Visible = True

            btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
            readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
            btnMakeAChange.ToolTip = toolTip
            btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
        Else
            btnRateLocation.Visible = True
            btnMakeAChange.Visible = False
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        'FarmLocationIndex = 0
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Locations"
        Me.ValidateChildControls(valArgs)
    End Sub

    'Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
    '    Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    'End Sub

    Private Sub HandleGL9Change(ByVal ClearIMRV As Boolean)
        For Each ri As RepeaterItem In FarmLocationRepeater.Items
            Dim locationControl As ctlFarmLocation = ri.FindControl("ctlFarmLocationItem")
            If locationControl IsNot Nothing Then locationControl.UpdateGL9Controls()
        Next
        RaiseEvent GL9Changed(ClearIMRV)
    End Sub

    Protected Sub btnPersonalProperty_Click(sender As Object, e As EventArgs) Handles btnPersonalProperty.Click
        If IsQuoteReadOnly() Then RaiseEvent NavigatePersonalProp() : Exit Sub
        Session("valuationValue") = "False"
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))

        If ValidationSummmary.HasErrors = False Then
            RaiseEvent NavigatePersonalProp()
        End If
    End Sub

    Protected Sub btnRateLocation_Click(sender As Object, e As EventArgs) Handles btnRateLocation.Click
        Session("valuationValue") = "False"
        'RaiseEvent SaveFarmLoc()
        RaiseEvent RateFarmLoc()
    End Sub

    Public Sub ClearDwellingbyIndex(locationIdx As Integer)
        If Me.ChildVrControls IsNot Nothing AndAlso Me.ChildVrControls(locationIdx) IsNot Nothing AndAlso TypeOf Me.ChildVrControls(locationIdx) Is ctlFarmLocation Then
            Dim ctlLoc As ctlFarmLocation = CType(Me.ChildVrControls(locationIdx), ctlFarmLocation)

            If ctlLoc IsNot Nothing Then
                ctlLoc.ClearDwelling()
            End If
        End If
    End Sub

    Protected Sub btnIMRVPage_Click(sender As Object, e As EventArgs) Handles btnIMRVPage.Click
        If IsQuoteReadOnly() Then RaiseEvent RequestIMNav() : Exit Sub

        Session("valuationValue") = "False"
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))

        If sender Is btnIMRVPage Then
            If ValidationSummmary.HasErrors = False Then
                RaiseEvent RequestIMNav()
            End If
        Else
            Populate()
        End If
        End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        For Each ri As RepeaterItem In FarmLocationRepeater.Items
            Dim ctlLoc As ctlFarmLocation = ri.FindControl("ctlFarmLocationItem")
            If ctlLoc IsNot Nothing Then
                ctlLoc.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
            End If
        Next
        Exit Sub
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub


End Class