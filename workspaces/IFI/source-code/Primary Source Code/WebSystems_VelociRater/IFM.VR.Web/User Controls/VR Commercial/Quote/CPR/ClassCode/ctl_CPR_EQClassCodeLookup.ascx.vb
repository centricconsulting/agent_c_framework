Imports IFM.PrimativeExtensions
Public Class ctl_CPR_EQClassCodeLookup
    Inherits VRControlBase

    Public Property ParentClassCodeTextboxId As String
        Get
            Return ViewState("vs_ParentCCTxtId")
        End Get
        Set(value As String)
            ViewState("vs_ParentCCTxtId") = value
        End Set
    End Property

    Public Property ParentHdnId As String
        Get
            Return ViewState("vs_ParentIDHdnId")
        End Get
        Set(value As String)
            ViewState("vs_ParentIDHdnId") = value
        End Set
    End Property

    Public Property ParentHdnRateGroupId As String
        Get
            Return ViewState("vs_ParentHdnRateGroup")
        End Get
        Set(value As String)
            ViewState("vs_ParentHdnRateGroup") = value
        End Set
    End Property

    Public Property ParentEQPPCDataRow1Id As String
        Get
            Return ViewState("vs_ParentEQPPCDataRow1ID")
        End Get
        Set(value As String)
            ViewState("vs_ParentEQPPCDataRow1ID") = value
        End Set
    End Property

    Public Property ParentEQPPODataRow1Id As String
        Get
            Return ViewState("vs_ParentEQPPODataRow1ID")
        End Get
        Set(value As String)
            ViewState("vs_ParentEQPPODataRow1ID") = value
        End Set
    End Property

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    ''' <summary>
    ''' BUILDING NUMBER is the number of the building in a list of all buildings.
    ''' This is used when setting the defaults on the building coverages
    ''' ZERO BASED
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property BuildingNumber As Int32
        Get
            If Quote IsNot Nothing Then
                Dim LocNdx As Integer = -1
                Dim BldNdx As Integer = -1
                Dim BldNumber As Integer = -1

                ' Loop through all the buildings on the quote and number them
                If Quote.Locations IsNot Nothing Then
                    For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                        BldNdx = -1
                        LocNdx += 1
                        If L.Buildings IsNot Nothing Then
                            For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                BldNdx += 1
                                BldNumber += 1
                                If LocationIndex = LocNdx AndAlso BuildingIndex = BldNdx Then
                                    ' Return the number of the building that matches this control's building
                                    Return BldNumber
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                    ' If we got here we didn't find the building
                    Return -1
                Else
                    ' Locations is nothing
                    Return -1
                End If
            Else
                ' Quote is nothing
                Return -1
            End If
        End Get
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property

    Public Property LookupType As String
        Get
            Return ViewState("vs_LookupType")
        End Get
        Set(value As String)
            ViewState("vs_LookupType") = value
        End Set
    End Property

    Public Property sender As String
        Get
            Return ViewState("vs_sender")
        End Get
        Set(value As String)
            ViewState("vs_sender") = value
        End Set
    End Property

    Public Event SelectedClassCodeChanged(ByVal ClassCode As String, ByVal Desc As String, ByVal DiaClass_Id As String)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim pgmtype As String = ""
        If Not IsNumeric(Me.Quote.ProgramTypeId.IsNumeric) Then pgmtype = "60" Else pgmtype = Me.Quote.ProgramTypeId

        Me.VRScript.AddVariableLine("Cpr.EQUiBindings.push(new Cpr.BuildingEQClassificationUIBinding('" & Me.LookupType & "'," & Me.LocationIndex.ToString & "," & Me.BuildingIndex.ToString & ",'" & Me.ParentClassCodeTextboxId & "','" & Me.ParentHdnId & "','" & Me.ParentHdnRateGroupId & "','" & Me.hdnDIAClass_Id.ClientID & "','" & Me.hdnRateGrade.ClientID & "','" & Me.hdnDescription.ClientID & "','" & Me.ParentEQPPCDataRow1Id & "','" & Me.ParentEQPPODataRow1Id & "'));")

        Me.VRScript.CreatePopupForm(Me.divMain.ClientID, "Earthquake Personal Property Classification Lookup", 750, 550, True, True, False, Me.txtFilterValue.ClientID, "")

        Me.VRScript.CreateJSBinding(Me.btnSearch.ClientID, "click", "VRClassCode.PerformCPREQClassificationLookup('" & sender & "'," & BuildingNumber & "," & Me.LocationIndex.ToString & "," & Me.BuildingIndex.ToString & ",'#" & Me.ddlFilterBy.ClientID & "','#" & Me.txtFilterValue.ClientID & "','#" + Me.divResults.ClientID & "','" & Me.hdnDIAClass_Id.ClientID & "','" & Me.hdnDescription.ClientID & "','" & Me.hdnRateGrade.ClientID & "'); return false;")

        ' These variables will all be used by the Class Code selection script
        ' Hidden Fields
        Me.VRScript.AddVariableLine("var hdnBLDGClassCode = '" & hdnClassCode.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnBLDGDescription = '" & hdnDescription.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnBLDGClassCodeID = '" & hdnDIAClass_Id.ClientID & "';")

        ' Close routine
        Me.VRScript.AddVariableLine("function CloseEQCCLookupForm(){$('#" + Me.btnClose.ClientID + "').click();}")

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Hide()

        ' Clear the fields
        Me.txtFilterValue.Text = ""
        Me.ddlFilterBy.SelectedIndex = 0

        PopulateChildControls()
    End Sub

    Public Sub Show(ByVal mysender As String)
        Session("PopulateAfterEQLookup") = "1"
        sender = mysender
        Populate()
        Me.Visible = True
    End Sub

    Public Sub Hide()
        Me.Visible = False
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Hide()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Hide()
    End Sub
End Class