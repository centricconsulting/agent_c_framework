Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL

Public Class ctl_CGL_Endo_ClassCodeList
    Inherits VRControlBase

    Public ReadOnly Property MyClassCodes As List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
        Get
            Dim ndx As Integer = -1

            ' Build a list of both Policy Level and Location Level class codes
            Dim ccs As New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)

            ' Policy Level Class Codes
            If Quote.GLClassifications IsNot Nothing AndAlso Quote.GLClassifications.Count > 0 Then
                For Each cls As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                    Dim newcls As QuickQuote.CommonObjects.QuickQuoteGLClassification = cls
                    ccs.Add(newcls)
                Next
            End If

            ' Location Level Class Codes
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If LOC.GLClassifications IsNot Nothing AndAlso LOC.GLClassifications.Count > 0 Then
                        For Each cls As QuickQuote.CommonObjects.QuickQuoteGLClassification In LOC.GLClassifications
                            Dim newcls As QuickQuote.CommonObjects.QuickQuoteGLClassification = cls
                            ccs.Add(newcls)
                        Next
                    End If
                Next
            End If

            Return ccs
        End Get
    End Property

    Public ReadOnly Property ClassCodeCount As Integer
        Get
            Dim cnt As Integer = 0
            If Quote.GLClassifications IsNot Nothing Then cnt += Quote.GLClassifications.Count
            If Quote.Locations IsNot Nothing Then
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If LOC.GLClassifications IsNot Nothing Then cnt += LOC.GLClassifications.Count
                Next
            End If
            Return cnt
        End Get
    End Property

    Public ReadOnly Property PolicyClassCodeCount As Integer
        Get
            Dim cnt As Integer = 0
            If Quote.GLClassifications IsNot Nothing Then cnt += Quote.GLClassifications.Count
            Return cnt
        End Get
    End Property

    Public ReadOnly Property LocationClassCodeCount As Integer
        Get
            Dim cnt As Integer = 0
            If Quote.Locations IsNot Nothing Then
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If LOC.GLClassifications IsNot Nothing Then cnt += LOC.GLClassifications.Count
                Next
            End If
            Return cnt
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divAccord.Visible Then
            'Updated 8/15/2022 for task 76303 MLW           
            If String.IsNullOrWhiteSpace(Me.hdnAccord.Value) AndAlso IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Nothing, "false")
            Else
                Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
            End If
            'Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
        End If
    End Sub

    Private Sub AddHandlers()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ClassControl As ctl_CGL_Endo_Classcode = cntrl.FindControl("ctl_CGL_Endo_Classcode")
            AddHandler ClassControl.AddNewClassCode, AddressOf AddNewGLClassCode
            AddHandler ClassControl.DeleteClassCode, AddressOf RemoveClassCode
            AddHandler ClassControl.RepopulateClassCodes, AddressOf Populate
            index += 1
        Next
    End Sub

    Public Sub ReloadLocations()
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ClassControl As ctl_CGL_Endo_Classcode = cntrl.FindControl("ctl_CGL_Endo_Classcode")
            ClassControl.LoadLocationsList()
        Next
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub BindData()
        Dim LocNdx As Integer = -1
        Dim ClassNdx As Integer = -1
        Dim BldNdx As Integer = -1
        Dim ControlIndex As Integer = -1

        If Me.Quote.IsNotNull Then
            Dim classCodes As List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification) = Me.MyClassCodes ' IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetAllPolicyAndLocationClassCodes(Me.Quote)
            Me.divAccord.Visible = False
            If classCodes.IsLoaded() Then
                divAccord.Visible = True
                Me.Repeater1.DataSource = classCodes
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                ' Class code index will be:
                ' Policy level - Index of Quote.GLClassifications
                ' Location level - Quote.Location(x).GLClassifications

                Dim ccIndex As Int32 = 0
                For Each cc In Me.GatherChildrenOfType(Of ctl_CGL_Endo_Classcode)
                    ControlIndex += 1
                    If ControlIndex = 0 Then cc.HideRemoveButton() Else cc.ShowRemoveButton()
                    If classCodes.HasItemAtIndex(ccIndex) Then
                        Dim typ As String = GetClassCodeType(classCodes(ccIndex), ClassNdx, LocNdx, BldNdx)
                        If typ <> "" Then
                            cc.ClassCodeType = typ.Substring(0, 1)
                            Select Case typ.Substring(0, 1)
                                Case "L"  ' Location
                                    cc.LocationIndex = LocNdx
                                    Exit Select
                                Case "P"  ' Policy
                                    Exit Select
                            End Select
                            cc.ClassCodeIndex = ClassNdx
                        Else
                            cc.ClassCodeType = ""
                            cc.ClassCodeIndex = -1
                            cc.LocationIndex = -1
                        End If
                    End If
                    ccIndex += 1
                Next

            Else
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        BindData()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.divAccord.ClientID
        AddHandlers()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        If ClassCodeSwitchedType() Then
            Populate()
        End If
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub AddNewGLClassCode()
        ' Always add the new class code at the policy level
        ' If it's a location level classification we will deal with that on save
        If Quote.GLClassifications Is Nothing Then Quote.GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
        Dim newcc As New QuickQuote.CommonObjects.QuickQuoteGLClassification()
        newcc.ClassCode = "9999999"
        Me.Quote.GLClassifications.Add(newcc)
        Save_FireSaveEvent(False)
        Populate()
        Me.hdnAccord.Value = (PolicyClassCodeCount - 1).ToString
        Dim method = String.Format("$('span:contains(""Class Code #{0}"")').get(0).id", PolicyClassCodeCount)
        Me.VRScript.ScrollToWithOffsetJQuerySelector(method, 0, Me.Page)
    End Sub

    Private Function ClassCodeSwitchedType() As Boolean
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ClassControl As ctl_CGL_Endo_Classcode = cntrl.FindControl("ctl_CGL_Endo_Classcode")
            If ClassControl.SwitchedType Then Return True
            index += 1
        Next
        Return False
    End Function

    Private Sub RemoveClassCode(ByVal PolicyOrLocation As String, ByVal UniqueId As String)
        Dim ndx As Integer = -1

        If Quote IsNot Nothing Then
            If PolicyOrLocation.ToUpper = "P" Then
                ' Policy
                If Quote.GLClassifications IsNot Nothing Then
                    For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                        ndx += 1
                        If cc.UniqueIdentifier = UniqueId Then
                            Quote.GLClassifications.RemoveAt(ndx)
                            Exit For
                        End If
                    Next
                End If
            Else
                ' Location
                If Quote.Locations IsNot Nothing Then
                    ndx = -1
                    For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                        If LOC.GLClassifications IsNot Nothing Then
                            For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In LOC.GLClassifications
                                ndx += 1
                                If cc.UniqueIdentifier = UniqueId Then
                                    LOC.GLClassifications.RemoveAt(ndx)
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        Me.Save_FireSaveEvent(False)
        Populate()
        Me.hdnAccord.Value = (ClassCodeCount - 1).ToString

        Exit Sub
    End Sub

    Private Function GetClassCodeType(ByVal CCObj As QuickQuote.CommonObjects.QuickQuoteGLClassification, ByRef ClassIndex As Integer, ByRef LocIndex As Integer, ByRef BldIndex As Integer) As String
        Dim ndx As Integer = -1
        Dim Lndx As Integer = -1
        Dim BuildingIndex As Integer = -1

        ' Policy
        If Quote.GLClassifications IsNot Nothing Then
            For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                ndx += 1
                If CCObj.UniqueIdentifier = cc.UniqueIdentifier Then
                    ClassIndex = ndx
                    Return "P"
                End If
            Next
        End If

        ' Location
        ndx = -1
        Lndx = -1
        For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
            Lndx += 1
            ndx = -1
            If LOC.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In LOC.GLClassifications
                    ndx += 1
                    If CCObj.UniqueIdentifier = cc.UniqueIdentifier Then
                        LocIndex = Lndx
                        ClassIndex = ndx
                        Return "L"
                    End If
                Next
            End If
        Next

        Return ""
    End Function

    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        If e.Item.ItemIndex = 0 Then
            Dim ctl As ctl_CGL_Endo_Classcode = e.Item.FindControl("ctl_CGL_Endo_Classcode")
            If ctl IsNot Nothing Then
                ctl.HideAddButton()
                ctl.FirstClassificationInList = True
            End If
        End If
    End Sub
End Class