Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL

Public Class ctl_CGL_LocationList
    Inherits VRControlBase

    Public Event LocationChanged(ByVal LocIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divNewLocation.Visible Then
            Me.VRScript.CreateAccordion(Me.divNewLocation.ClientID, Nothing, "false", True)
        Else
            If Me.divLocationList.Visible Then
                Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
            End If
        End If

    End Sub
    Private Sub AddHandlers()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_CGL_Location = cntrl.FindControl("ctl_CGL_Location")
            AddHandler LocControl.LocationChanged, AddressOf HandleAddressChange
            AddHandler LocControl.AddLocationRequested, AddressOf AddNewLocation
            AddHandler LocControl.DeleteLocationRequested, AddressOf DeleteLocation
            index += 1
        Next
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub HandleAddressChange(ByVal LocIndex As Integer)
        RaiseEvent LocationChanged(LocIndex)
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            Me.divNewLocation.Visible = False
            Me.divLocationList.Visible = False
            If Me.Quote.Locations.IsLoaded() Then
                Me.divLocationList.Visible = True
                Me.Repeater1.DataSource = Me.Quote.Locations
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                Dim index As Int32 = 0
                For Each Loc As ctl_CGL_Location In Me.GatherChildrenOfType(Of ctl_CGL_Location)
                    Loc.MyLocationIndex = index
                    'Loc.Populate()
                    index += 1
                Next
            Else
                Me.divNewLocation.Visible = True
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandlers()
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divLocationList.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Locations"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        'Dim showval As Boolean = False
        'Dim HasIN As Boolean = False
        'Dim HasIL As Boolean = False
        'Dim msg As String = ""
        'Dim GovState As String = ""

        '' Figure out what the governing state is
        'Dim GovStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = GoverningStateQuote()
        'Select Case GovStateQuote.QuickQuoteState
        '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
        '        GovState = "IL"
        '        Exit Select
        '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
        '        GovState = "IN"
        '        Exit Select
        'End Select

        '' Validate that every state on the quote has a location
        'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
        '    Select Case sq.QuickQuoteState
        '        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
        '            If sq.Locations IsNot Nothing AndAlso sq.Locations.Count >= 1 Then HasIL = True
        '            Exit Select
        '        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
        '            If sq.Locations IsNot Nothing AndAlso sq.Locations.Count >= 1 Then HasIN = True
        '            Exit Select
        '    End Select
        'Next
        'If (Not HasIN) OrElse (Not HasIL) Then
        '    showval = True
        '    If HasIN Then
        '        ' Has Indiana, does not have Illinois
        '        If GovState = "IL" Then
        '            msg = "You have selected Illinois as the Governing State but you have not entered any locations in Illinois.  Please add at least one Illinois location."
        '        Else
        '            msg = "No Locations were entered in Illinois.  Please add location(s) or remove additional non-governing state(s) on the Policyholder page."
        '        End If
        '    Else
        '        ' Has Illinois, does not have Indiana
        '        If GovState = "IN" Then
        '            msg = "You have selected Indiana as the Governing State but you have not entered any locations in Indiana.  Please add at least one Indiana location."
        '        Else
        '            msg = "No Locations were entered in Indiana.  Please add location(s) or remove additional non-governing state(s) on the Policyholder page."
        '        End If
        '    End If
        'End If
        'If showval Then
        '    Me.ValidationHelper.AddError(msg)
        'End If

        'updated 11/16/2018; above logic was always requiring locations in IN and IL regardless of the states on the quote
        If Quote IsNot Nothing Then
            ' Figure out what the governing state is
            Dim govState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = Quote.QuickQuoteState

            ' Validate that every state on the quote has a location
            Dim qqStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates
            Dim qqStatesWithoutLoc As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Nothing
            If qqStates IsNot Nothing AndAlso qqStates.Count > 0 Then
                For Each s As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In qqStates
                    If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), s) = True AndAlso s <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None AndAlso (qqStatesWithoutLoc Is Nothing OrElse qqStatesWithoutLoc.Count = 0 OrElse qqStatesWithoutLoc.Contains(s) = False) Then
                        Dim stateLocs As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, s)
                        If stateLocs Is Nothing OrElse stateLocs.Count = 0 Then
                            QuickQuote.CommonMethods.QuickQuoteHelperClass.AddQuickQuoteStateToList(s, qqStatesWithoutLoc)
                        End If
                    End If
                Next
            End If
            If qqStatesWithoutLoc IsNot Nothing AndAlso qqStatesWithoutLoc.Count > 0 Then
                For Each s As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In qqStatesWithoutLoc
                    If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), s) = True AndAlso s <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        Dim msg As String = ""
                        Dim strState As String = System.Enum.GetName(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), s)
                        If s = govState Then
                            msg = "You have selected " & strState & " as the Governing State but you have not entered any locations in " & strState & ".  Please add at least one " & strState & " location."
                        Else
                            msg = "No Locations were entered in " & strState & ".  Please add location(s) or remove additional non-governing state(s) on the Policyholder page."
                        End If
                        Me.ValidationHelper.AddError(msg)
                    End If
                Next
            End If
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub AddNewLocation()
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.AddNew()
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString
        End If
    End Sub

    Protected Sub DeleteLocation(LocationIndex As Integer)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.RemoveAt(LocationIndex)
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString
        End If
    End Sub
End Class