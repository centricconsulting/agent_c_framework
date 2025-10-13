Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data
Imports System.Data.SqlClient
Imports PopupMessageClass
Public Class ctl_WCP_Classification
    Inherits VRControlBase

#Region "Declarations"

    Private Structure ClassIficationItem_enum
        Public Classification As QuickQuote.CommonObjects.QuickQuoteClassification
        Public QuickQuoteState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
    End Structure

    ''' <summary>
    ''' This is the index of the classification on the state quote
    ''' </summary>
    ''' <returns></returns>
    Public Property StateQuoteClassificationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_WCPStateQuoteClassificationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_WCPStateQuoteClassificationIndex") = value
        End Set
    End Property

    ''' <summary>
    ''' This is the index of the classification as it appears on the repeater
    ''' </summary>
    ''' <returns></returns>
    Public Property ClassificationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_WCPClassificationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_WCPClassificationIndex") = value
        End Set
    End Property

    Public ReadOnly Property ClassificationState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Get
            'Select Case ClassificationStateId
            '    Case "15"
            '        Return QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
            '        Exit Select
            '    Case "16"
            '        Return QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
            '        Exit Select
            '    Case Else
            '        Return ""
            'End Select
            'updated 12/28/2018
            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(QQHelper.IntegerForString(ClassificationStateId))
        End Get
    End Property

    Public Property ClassificationStateId As String
        Get
            Return hdnStateId.Value
        End Get
        Set(value As String)
            hdnStateId.Value = value
        End Set
    End Property

    'Private ReadOnly Property NumberOfStatesOnQuote() As Integer
    '    Get
    '        Dim cnt As Integer = 0
    '        Dim states As New List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
    '        states.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
    '        states.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)

    '        If Quote IsNot Nothing Then
    '            If Quote.Locations IsNot Nothing Then
    '                For Each st As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In states
    '                    For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
    '                        If LOC.Address.QuickQuoteState = st Then
    '                            cnt += 1
    '                            Exit For
    '                        End If
    '                    Next
    '                Next
    '                Return cnt
    '            End If
    '        End If
    '        Return 0
    '    End Get
    'End Property

    'Private ReadOnly Property StatesOnQuote() As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
    '    Get
    '        Dim states As New List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
    '        states.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
    '        states.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
    '        Dim soq As New List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)

    '        If Quote IsNot Nothing Then
    '            If Quote.Locations IsNot Nothing Then
    '                For Each st As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In states
    '                    For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
    '                        If LOC.Address.QuickQuoteState = st Then
    '                            soq.Add(LOC.Address.QuickQuoteState)
    '                            Exit For
    '                        End If
    '                    Next
    '                Next
    '            End If
    '        End If
    '        Return soq
    '    End Get
    'End Property

    Private ReadOnly Property MyClassification_NonMultiState As QuickQuote.CommonObjects.QuickQuoteClassification
        Get
            ' Single state
            ' If not a multistate quote then get the classifications from location 0
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing AndAlso Quote.Locations(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
                Return Quote.Locations(0).Classifications(ClassificationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property MyClassification_MultiState(Optional ByVal QQState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = Nothing) As ClassIficationItem_enum
        Get
            ' Multistate
            ' Get a list of all classifications, since that's the way this control was loaded
            Dim Classifications As List(Of ClassIficationItem_enum) = GetMultistateClassifications()
            ' Return the correct classification
            'If Classifications.HasItemAtIndex(ClassificationIndex) Then Return Classifications(ClassificationIndex)
            'updated 12/28/2018
            If Classifications IsNot Nothing AndAlso Classifications.HasItemAtIndex(ClassificationIndex) Then Return Classifications(ClassificationIndex)
            Return New ClassIficationItem_enum()
            'updated 12/28/2018
            'Dim newCls As New ClassIficationItem_enum
            'If newCls.Classification Is Nothing Then
            '    newCls.Classification = New QuickQuote.CommonObjects.QuickQuoteClassification
            'End If
            'Return newCls
        End Get
    End Property

    'Private ReadOnly Property MyClassification As QuickQuote.CommonObjects.QuickQuoteClassification
    '    Get
    '        Dim StateQuote As New QuickQuote.CommonObjects.QuickQuoteObject
    '        Dim NumberOfStates As Integer = NumberOfStatesOnQuote
    '        Dim QuoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = StatesOnQuote

    '        ' Get the classification based on state
    '        If NumberOfStates = 1 Then
    '            ' If only one state then get the classifications from location 0
    '            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing AndAlso Quote.Locations(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
    '                Return Quote.Locations(0).Classifications(ClassificationIndex)
    '            End If
    '        Else
    '            ' If there is more than one state then we need to get the classification for that state
    '            If Quote IsNot Nothing Then
    '                Select Case hdnStateId.Value
    '                    Case "15"  ' Illinois
    '                        StateQuote = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
    '                        Exit Select
    '                    Case "16"  ' Indiana
    '                        StateQuote = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
    '                        Exit Select
    '                End Select
    '                If StateQuote IsNot Nothing AndAlso StateQuote.Locations IsNot Nothing AndAlso StateQuote.Locations.HasItemAtIndex(0) AndAlso StateQuote.Locations(0).Classifications IsNot Nothing AndAlso StateQuote.Locations(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
    '                    Return StateQuote.Locations(0).Classifications(ClassificationIndex)
    '                End If
    '            End If
    '        End If

    '        Return Nothing
    '    End Get
    'End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.ClassificationIndex
        End Get
    End Property

    Public Event AddClassificationRequested(ByVal qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
    Public Event DeleteClassificationRequested(ByVal qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, ByVal ClassificationIndex As Integer)
    Public Event RequestClassificationPopulate()
    'Public Event ClearClassificationRequested(ByVal ClassificationIndex As Integer)
    'Public Event ClassificationChanged(ByVal ClassificationIndex As Integer)

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete?")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Function GetMultistateClassifications() As List(Of ClassIficationItem_enum)
        Dim CLassificationList As New List(Of ClassIficationItem_enum)
        'updated 12/28/2018
        Dim quoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates
        If quoteStates IsNot Nothing AndAlso quoteStates.Count > 0 Then
            For Each qs As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In quoteStates
                Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qs)
                If locsForState IsNot Nothing AndAlso locsForState.Count > 0 AndAlso locsForState(0) IsNot Nothing Then
                    If locsForState(0).Classifications IsNot Nothing AndAlso locsForState(0).Classifications.Count > 0 Then
                        For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In locsForState(0).Classifications
                            Dim clsItem As New ClassIficationItem_enum()
                            clsItem.Classification = cls
                            clsItem.QuickQuoteState = qs
                            CLassificationList.AddItem(clsItem)
                        Next
                    End If
                End If
            Next
        End If

        Return CLassificationList
    End Function

    Private Sub UpdateAccordHeader()
        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' Multistate
            Dim myMultiStateClass As ClassIficationItem_enum = MyClassification_MultiState 'added 12/28/2018
            'If MyClassification_MultiState.IsNotNull Then
            'updated 12/28/2018
            If myMultiStateClass.IsNotNull Then
                Dim dsc As String = Nothing
                Dim st As String = ""
                'Update 9/9/2022 for task 73917 MLW
                If myMultiStateClass.Classification IsNot Nothing
                    If myMultiStateClass.Classification.ClassificationTypeId <> "9999999" OrElse StateQuoteClassificationIndex = 0 Then
                        ' Don't add the state to the header of any new classifications that aren't the 1st classicfication on the state
                        'updated 12/28/2018
                        Dim stAbrev As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForQuickQuoteState(myMultiStateClass.QuickQuoteState)
                        If String.IsNullOrWhiteSpace(stAbrev) = False Then
                            st = " (" & stAbrev & ")"
                        End If
                    End If

                    If (myMultiStateClass.Classification.Description.Trim & st).Length <= 27 Then
                        dsc = myMultiStateClass.Classification.Description.ToUpper & st
                    Else
                        dsc = myMultiStateClass.Classification.Description.ToUpper.Substring(0, 22) & st & "..."
                    End If
                End if
                lblAccordHeader.Text = "Class Code # " & ClassificationIndex + 1.ToString & " " & dsc
            Else
                lblAccordHeader.Text = "Class Code # " & ClassificationIndex + 1.ToString
            End If
        Else
            ' Single State
            If MyClassification_NonMultiState.IsNotNull Then
                Dim dsc As String = Nothing
                Dim st As String = ""

                If (MyClassification_NonMultiState.Description.Trim & st).Length <= 27 Then
                    dsc = MyClassification_NonMultiState.Description.ToUpper & st
                Else
                    dsc = MyClassification_NonMultiState.Description.ToUpper.Substring(0, 22) & st & "..."
                End If
                lblAccordHeader.Text = "Class Code # " & ClassificationIndex + 1.ToString & " " & dsc
            Else
                lblAccordHeader.Text = "Class Code # " & ClassificationIndex + 1.ToString
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        Try
            Me.ctl_ClassCodeLookup.Visible = False
            ClearInputFields()

            ' Only show the delete link when it's not the first classification
            ' Set the delete button - can't delete first classification on each state
            If StateQuoteClassificationIndex = 0 Then
                lnkDelete.Visible = False
                lnkClear.Visible = False
            Else
                lnkDelete.Visible = True
                lnkClear.Visible = True
            End If

            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' Multistate
                Dim myMultiStateClass As ClassIficationItem_enum = MyClassification_MultiState 'added 12/28/2018
                'If MyClassification_MultiState.IsNotNull Then
                'updated 12/28/2018
                If myMultiStateClass.IsNotNull Then
                    'Updated 9/9/2022 for bug 73917 MLW
                    If myMultiStateClass.Classification IsNot Nothing then
                        'updated 12/28/2018
                        If myMultiStateClass.Classification.ClassCode <> "" Then txtClassCode.Text = myMultiStateClass.Classification.ClassCode
                        If myMultiStateClass.Classification.Description <> "" Then txtDescription.Text = myMultiStateClass.Classification.Description
                        If myMultiStateClass.Classification.ClassificationTypeId <> "" Then
                            If myMultiStateClass.Classification.ClassificationTypeId <> "9999999" Then
                                txtID.Text = myMultiStateClass.Classification.ClassificationTypeId
                            Else
                                txtID.Text = ""
                            End If
                        End If
                        If myMultiStateClass.Classification.Payroll <> "" Then txtEmployeePayroll.Text = myMultiStateClass.Classification.Payroll
                    End if    
                    Dim diaStateId As Integer = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(myMultiStateClass.QuickQuoteState)
                    If diaStateId > 0 Then
                        hdnStateId.Value = diaStateId.ToString
                    End If                    
                End If
            Else
                ' Not multistate
                If MyClassification_NonMultiState IsNot Nothing Then
                    If MyClassification_NonMultiState.ClassCode <> "" Then txtClassCode.Text = MyClassification_NonMultiState.ClassCode
                    If MyClassification_NonMultiState.Description <> "" Then txtDescription.Text = MyClassification_NonMultiState.Description
                    If MyClassification_NonMultiState.ClassificationTypeId <> "" Then
                        If MyClassification_NonMultiState.ClassificationTypeId <> "9999999" Then
                            txtID.Text = MyClassification_NonMultiState.ClassificationTypeId
                        Else
                            txtID.Text = ""
                        End If
                    End If
                    If MyClassification_NonMultiState.Payroll <> "" Then txtEmployeePayroll.Text = MyClassification_NonMultiState.Payroll
                    hdnStateId.Value = Nothing
                End If
            End If

            UpdateAccordHeader()
            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    ''' Attempts to find and return the passed classification based on it's QuickQuoteState value
    ''' Also returns the State Abbreviation of the state quote the class code was found on AND the index of the class code on the state quote
    ''' 
    ''' Called when the state is changed on a classification
    ''' 
    ''' Parameters:
    ''' - cls: this is the classification item to search for
    ''' - StateAbbrev: this will return the state abbreviation that the classification was found in.  If the class code wasn't found will be NOTHING.
    ''' - StateClassificationIndex: this will return the classification index of the found class code.  If the class code wan't founf will be -1.
    Private Function GetMultiStateClassificationItem(ByVal cls As ClassIficationItem_enum, ByRef StateAbbrev As String, ByRef StateClassificationIndex As Integer) As QuickQuote.CommonObjects.QuickQuoteClassification
        Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        StateClassificationIndex = -1
        Dim ndx As Integer = -1
        StateAbbrev = Nothing

        'updated 12/28/2018
        Dim myMultiStateClass As ClassIficationItem_enum = MyClassification_MultiState
        If myMultiStateClass.IsNotNull Then
            ' Figure out which state quote to search base on the QuickQuoteState on the myMultiStateClass object
            Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, myMultiStateClass.QuickQuoteState)
            If locsForState IsNot Nothing AndAlso locsForState.HasItemAtIndex(0) AndAlso locsForState(0).Classifications IsNot Nothing Then
                For Each cc As QuickQuote.CommonObjects.QuickQuoteClassification In locsForState(0).Classifications
                    ndx += 1
                    If cc.Equals(myMultiStateClass.Classification) Then
                        ' Found the classification, return it
                        Dim stAbrev As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForQuickQuoteState(myMultiStateClass.QuickQuoteState)
                        If String.IsNullOrWhiteSpace(stAbrev) = False Then
                            StateAbbrev = stAbrev
                        End If
                        StateClassificationIndex = ndx
                        Return cc
                    End If
                Next
            End If
        End If

        ' If we got here the classification wasn't found
        Return Nothing
    End Function

    Public Overrides Function Save() As Boolean
        Dim cls As QuickQuote.CommonObjects.QuickQuoteClassification = Nothing
        Dim dia_id As String = Nothing
        Dim err As String = Nothing
        Dim pn As String = Nothing
        Dim pa As String = Nothing
        Dim StateChanged As Boolean = False
        'Dim NeedToRepopulate As Boolean = False

        Try
            lblMsg.Text = "&nbsp;"

            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' Multistate
                ' Save the Classification to the correct quote based on state
                Dim myMultiStateClass As ClassIficationItem_enum = MyClassification_MultiState 'added 12/28/2018
                'If MyClassification_MultiState.IsNotNull Then
                'updated 12/28/2018
                If myMultiStateClass.IsNotNull Then
                    StateChanged = False
                    'updated 12/28/2018
                    Dim oldState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
                    Dim newState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
                    If QQHelper.IsPositiveIntegerString(hdnStateId.Value) = True Then
                        Dim diaStateId As Integer = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(myMultiStateClass.QuickQuoteState)
                        If diaStateId > 0 Then
                            If QuickQuote.CommonMethods.QuickQuoteHelperClass.isTextMatch(hdnStateId.Value, diaStateId.ToString, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                                'appears that state may have changed
                                oldState = myMultiStateClass.QuickQuoteState
                                newState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(QQHelper.IntegerForString(hdnStateId.Value))
                                If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), oldState) = True AndAlso oldState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None AndAlso System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), newState) = True AndAlso newState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None AndAlso oldState <> newState Then
                                    StateChanged = True
                                End If
                            End If
                        Else
                            Throw New Exception("Invalid State ID!")
                        End If
                    End If

                    ' If the state changed we need to remove the classification from the original state and add it to the new state
                    If StateChanged Then
                        ' STATE CHANGED
                        Dim mycls As QuickQuote.CommonObjects.QuickQuoteClassification = Nothing
                        Dim StateAbbrev As String = Nothing
                        Dim ClsIndex As Integer = -1
                        'mycls = GetMultiStateClassificationItem(MyClassification_MultiState, StateAbbrev, ClsIndex)
                        'updated 12/28/2018
                        mycls = GetMultiStateClassificationItem(myMultiStateClass, StateAbbrev, ClsIndex)

                        If mycls IsNot Nothing Then
                            'updated 12/28/2018; note: could also try to Raise Delete and Add methods like lnkClear_Click does
                            Dim oldLocsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, oldState)
                            Dim newLocsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, newState)
                            If oldLocsForState IsNot Nothing AndAlso oldLocsForState.Count > 0 AndAlso newLocsForState IsNot Nothing AndAlso newLocsForState.Count > 0 AndAlso oldLocsForState(0) IsNot Nothing AndAlso oldLocsForState(0).Classifications IsNot Nothing AndAlso oldLocsForState(0).Classifications.HasItemAtIndex(ClsIndex) AndAlso newLocsForState(0) IsNot Nothing Then
                                'the classification exists on the old location and there is a new location
                                ' Add the classification to the new state
                                If newLocsForState(0).Classifications Is Nothing Then newLocsForState(0).Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
                                Dim newClassification As New QuickQuote.CommonObjects.QuickQuoteClassification()
                                newClassification.ClassificationTypeId = txtID.Text
                                newClassification.Description = txtDescription.Text
                                newClassification.Payroll = txtEmployeePayroll.Text
                                newLocsForState(0).Classifications.Add(newClassification)

                                ' Remove the classification from the old state
                                oldLocsForState(0).Classifications.RemoveAt(ClsIndex)
                            End If

                            RaiseEvent RequestClassificationPopulate()
                            Return True
                            'NeedToRepopulate = True
                        End If
                    Else
                        'updated 12/28/2018
                        If myMultiStateClass.Classification.ClassificationTypeId = "9999999" AndAlso txtID.Text = "" Then
                            ' This is a new classification and hasn't been set or saved yet, don't save anything
                        Else
                            ' Classification was selected and set, save it
                            myMultiStateClass.Classification.ClassificationTypeId = txtID.Text
                            myMultiStateClass.Classification.Description = txtDescription.Text
                            myMultiStateClass.Classification.Payroll = txtEmployeePayroll.Text
                        End If
                    End If
                End If
            Else
                ' Not Multistate
                ' Save the Classification to the correct quote based on state
                If MyClassification_NonMultiState IsNot Nothing Then
                    ' Can't set class code because it's read only
                    MyClassification_NonMultiState.ClassificationTypeId = txtID.Text
                    MyClassification_NonMultiState.Description = txtDescription.Text
                    MyClassification_NonMultiState.Payroll = txtEmployeePayroll.Text
                End If

            End If

            Me.SaveChildControls()  ' Don't try and save the child controls after repopulate due to state change or you'll get an error 

            ' Don't request the repopulate until the saves are complete or you'll get an error
            'If NeedToRepopulate Then RaiseEvent RequestClassificationPopulate()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            MyBase.ValidateControl(valArgs)
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Me.ValidationHelper.GroupName = "Classification #" & ClassificationIndex + 1

            If txtClassCode.Text.Trim = String.Empty Then
                Me.ValidationHelper.AddError(txtClassCode, "Missing Class Code", accordList)
            End If

            If txtEmployeePayroll.Text.Trim = String.Empty Then
                Me.ValidationHelper.AddError(txtEmployeePayroll, "Missing Payroll", accordList)
            End If

            Me.ValidateChildControls(valArgs)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControls", ex)
            Exit Sub
        End Try
    End Sub

    Private Function GetDiamondClassCodeAndDescription(ByVal classificationtype_id As String, ByRef ClassCode As String, ByRef ClassDscr As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = AppSettings("connDiamond")
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM ClassificationType WHERE classificationtype_id = " & classificationtype_id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                ClassCode = tbl.Rows(0)("code").ToString()
                ClassDscr = tbl.Rows(0)("dscr").ToString()
                Return True
            Else
                ClassCode = Nothing
                ClassDscr = Nothing
                Return False
            End If
        Catch ex As Exception
            HandleError("GetDiamondClassificationTypeID", ex)
            ClassCode = Nothing
            ClassDscr = Nothing
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
                conn.Dispose()
            End If
            da.Dispose()
            cmd.Dispose()
            tbl.Dispose()
        End Try
    End Function


    Private Function GetDiamondClassificationTypeID(ByVal ClassCode As String, ByVal dscr As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = AppSettings("connDiamond")
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT classificationtype_id FROM ClassificationType WHERE code = '" & ClassCode & "' AND UPPER(dscr) = '" & dscr.ToUpper & "'"
            rtn = cmd.ExecuteScalar

            If rtn IsNot Nothing Then
                Return rtn
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError("GetDiamondClassificationTypeID", ex)
            Return Nothing
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
                conn.Dispose()
            End If
            cmd.Dispose()
        End Try
    End Function

    Private Sub ClearInputFields()
        Try
            txtClassCode.Text = String.Empty
            txtEmployeePayroll.Text = String.Empty
            txtDescription.Text = String.Empty

            Exit Sub
        Catch ex As Exception
            HandleError("ClearInputFields", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub HandleClassCodeLookup(ByVal ClassCode As String, ByVal Desc As String, ByVal DiaClass_Id As String, ByVal StateId As String)
        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' Multistate
            If MyClassification_MultiState.IsNotNull Then
                txtClassCode.Text = ClassCode
                txtDescription.Text = Desc
                txtID.Text = DiaClass_Id
                hdnStateId.Value = StateId
                txtEmployeePayroll.Focus()

                ' Update the header
                Dim dsc As String = ""
                Dim st As String = ""
                'If hdnStateId.Value = "15" Then st = " (IL)" Else If hdnStateId.Value = "16" Then st = " (IN)"
                'updated 12/28/2018
                If QQHelper.IsPositiveIntegerString(hdnStateId.Value) = True Then
                    Dim stateAbrev As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForDiamondStateId(QQHelper.IntegerForString(hdnStateId.Value))
                    If String.IsNullOrWhiteSpace(stateAbrev) = False Then
                        st = " (" & stateAbrev & ")"
                    End If
                End If

                If (txtDescription.Text.Trim & st).Length <= 27 Then
                    dsc = txtDescription.Text.ToUpper & st
                Else
                    dsc = txtDescription.Text.ToUpper.Substring(0, 22) & st & "..."
                End If

                dsc = "Class Code # " & ClassificationIndex + 1 & " " & dsc
                lblAccordHeader.Text = dsc
            End If
        Else
            ' Not Multistate
            If MyClassification_NonMultiState IsNot Nothing Then
                txtClassCode.Text = ClassCode
                txtDescription.Text = Desc
                txtID.Text = DiaClass_Id
                hdnStateId.Value = StateId
                Save_FireSaveEvent(False)
                Populate()
                txtEmployeePayroll.Focus()
            End If
        End If
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'If ViewState("ClassDataTable") IsNot Nothing Then tblClasses = ViewState("ClassDataTable")

            If Not IsPostBack Then
                Me.MainAccordionDivId = Me.divWCPClass.ClientID
            End If

            AddHandler ctl_ClassCodeLookup.SelectedClassCodeChanged, AddressOf HandleClassCodeLookup

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Try
            Me.Save_FireSaveEvent()
            Exit Sub
        Catch ex As Exception
            HandleError("Save", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        ' On clear we need to remove the existing classification and create a new one
        'updated 12/28/2018
        Dim qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' Multistate
            qqState = ClassificationState
        Else
            ' Not Multistate
            qqState = Quote.QuickQuoteState
        End If
        ' Delete the existing classification
        RaiseEvent DeleteClassificationRequested(qqState, StateQuoteClassificationIndex)
        ' Add a new classification
        RaiseEvent AddClassificationRequested(qqState)

        ' Repopulate the page
        RaiseEvent RequestClassificationPopulate()

        Exit Sub
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        'RaiseEvent DeleteClassificationRequested(ClassificationState, StateQuoteClassificationIndex)

        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' Multistate
            RaiseEvent DeleteClassificationRequested(ClassificationState, StateQuoteClassificationIndex)
        Else
            ' Not Multistate
            RaiseEvent DeleteClassificationRequested(Quote.QuickQuoteState, StateQuoteClassificationIndex)
        End If
    End Sub

    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnClassCodeLookup.Click
        Session("WCP_ClassCodeLookup_" & Session.SessionID) = "1"

        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            Dim myMultiStateClass As ClassIficationItem_enum = MyClassification_MultiState 'added 12/28/2018
            If myMultiStateClass.IsNotNull Then 'added 12/28/2018
                'If (Not MyClassification_MultiState.Equals(New ClassIficationItem_enum)) AndAlso MyClassification_MultiState.Classification.ClassificationTypeId <> "9999999" AndAlso MyClassification_MultiState.Classification.ClassificationTypeId.Trim <> "" Then
                'updated 12/28/2018
                If (Not myMultiStateClass.Equals(New ClassIficationItem_enum)) AndAlso myMultiStateClass.Classification IsNot Nothing AndAlso myMultiStateClass.Classification.ClassificationTypeId <> "9999999" AndAlso myMultiStateClass.Classification.ClassificationTypeId.Trim <> "" Then
                    'updated 12/28/2018
                    Dim diaStateId As Integer = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(myMultiStateClass.QuickQuoteState)
                    If diaStateId > 0 Then
                        Session("WCP_CCLookup_StateId") = diaStateId.ToString  ' If this is set the lookup will disable the state dropdown
                    End If
                Else
                    ' This code executes when you first come to this page and you already have the first state class codes on the page but they haven't been set yet
                    ' By setting the stateid session var we make the class code lookup's state dropdown disabled, which is what we want, state changes aren't allowed
                    ' on classifications.
                    Session("WCP_CCLookup_StateId") = Nothing
                    'updated 12/28/2018
                    If myMultiStateClass.Classification IsNot Nothing AndAlso myMultiStateClass.Classification.ClassificationTypeId = "9999999" AndAlso StateQuoteClassificationIndex = 0 Then
                        ' If it's a new classification and also the first classification on the state, set the session variable so the state dropdown will be disabled on the class code lookup
                        If hdnStateId.Value IsNot Nothing AndAlso hdnStateId.Value <> "" Then
                            Session("WCP_CCLookup_StateId") = hdnStateId.Value  ' If this is set the lookup will disable the state dropdown
                        End If
                    End If
                End If
            End If
        Else
            Session("WCP_CCLookup_StateId") = Nothing
        End If

        Me.ctl_ClassCodeLookup.Populate()  ' This will also show the control if the above session variable is set to anything but nothing   
    End Sub
#End Region

End Class