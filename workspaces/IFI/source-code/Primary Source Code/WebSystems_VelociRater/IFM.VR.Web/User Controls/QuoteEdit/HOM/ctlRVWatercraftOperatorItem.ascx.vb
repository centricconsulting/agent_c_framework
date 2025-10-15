Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class ctlRVWatercraftOperatorItem
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

#Region "Declarations"
    Private Const ClassName As String = "ctlRVOperatorItem"
    Public Event ItemRemoveRequest(index As Int32)
    Public Event SaveRequested()

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper

    Private _quote As QuickQuoteObject
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            Dim errCreateQSO As String = ""
            If _quote Is Nothing Then
                _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuoteId, errCreateQSO)
            End If
            Return _quote
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Private _operatornumber As Int32 = -1
    Public Property OperatorNumber As Int32
        Get
            Return _operatornumber
        End Get
        Set(value As Int32)
            _operatornumber = value
        End Set
    End Property

    Private _rvwatercraftnumber As Int32 = -1
    Public Property RVWatercraftNumber As Int32
        Get
            Return _rvwatercraftnumber
        End Get
        Set(value As Int32)
            _rvwatercraftnumber = value
        End Set
    End Property

#End Region

#Region "Methods and Functions"
    Private Sub HandleError(ByVal RoutineName As String, ByVal exc As Exception)
        ShowMessage(ClassName & ":" & RoutineName & ": " & exc.Message)
    End Sub

    Private Sub ShowMessage(ByVal msg As String)
        lblMsg.Text = msg
    End Sub

    Private Sub ClearErrorMessage()
        lblMsg.Text = "&nbsp;"
    End Sub

    Private Sub LoadStaticData() Implements IVRUI_P.LoadStaticData
        Exit Sub
    End Sub

    ''' <summary>
    ''' Loads the Available Operators ddl
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadAvailableOperators()
        Dim li As ListItem = Nothing
        Dim foundph1 As Boolean = False
        Dim foundph2 As Boolean = False

        Try
            ddlAvailableOperators.Items.Clear()

            ' Add a blank item
            'li = New ListItem("", "")
            'ddlAvailableOperators.Items.Add(li)

            ' *****************
            ' POLICYHOLDER 1
            ' *****************
            If Quote Is Nothing OrElse Quote.Locations(0) Is Nothing _
                OrElse Quote.Policyholder Is Nothing _
                OrElse Quote.Policyholder.Name Is Nothing _
                OrElse Not IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Policyholder.Name.DisplayName) Then Exit Sub

            ' Determine if Policyholder1 is already assigned
            For Each row As GridViewRow In grdAssignedOperators.Rows
                If row.Cells(1).Text.ToUpper() = Quote.Policyholder.Name.DisplayName Then
                    foundph1 = True
                    Exit For
                End If
            Next

            ' If policyholder 1 was NOT found in the assigned grid, add to the available operators list
            If Not foundph1 Then
                li = New ListItem(Quote.Policyholder.Name.DisplayName, "P1")
                ddlAvailableOperators.Items.Add(li)
            End If

            ' *****************
            ' POLICYHOLDER 2
            ' *****************
            If Quote.Policyholder2 IsNot Nothing AndAlso Quote.Policyholder2.Name IsNot Nothing _
                AndAlso Quote.Policyholder2.Name.DisplayName IsNot Nothing _
                AndAlso IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Policyholder2.Name.DisplayName) Then

                ' Determine if Policyholder1 is already assigned
                For Each row As GridViewRow In grdAssignedOperators.Rows
                    If row.Cells(1).Text.ToUpper() = Quote.Policyholder2.Name.DisplayName Then
                        foundph2 = True
                        Exit For
                    End If
                Next

                ' If policyholder 2 was NOT found in the assigned grid, add to the available operators list
                If Not foundph2 Then
                    li = New ListItem(Quote.Policyholder2.Name.DisplayName, "P2")
                    ddlAvailableOperators.Items.Add(li)
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("LoadAvailableOperators", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        Dim tbl As New DataTable()
        Dim newrow As DataRow = Nothing

        Try
            LoadStaticData()

            If Quote Is Nothing OrElse Quote.Locations(0) Is Nothing OrElse
                Quote.Locations(0).RvWatercrafts Is Nothing _
                OrElse Quote.Locations(0).RvWatercrafts.Count <= 0 _
                OrElse Quote.Locations(0).RvWatercrafts.Count < RVWatercraftNumber - 1 Then Exit Sub

            If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators Is Nothing Then
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators = New List(Of QuickQuoteOperator)
            End If

            grdAssignedOperators.DataSource = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators
            grdAssignedOperators.DataBind()

            LoadAvailableOperators()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Dim NewOP As QuickQuoteOperator = Nothing
        Dim NewOpNum As Integer = 0

        Try

            ' Clear out all existing operators
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators = New List(Of QuickQuoteOperator)

            ' Loop through the grid rows and create operators for each
            For Each row As GridViewRow In grdAssignedOperators.Rows
                NewOpNum += 1
                NewOP = New QuickQuoteOperator()
                ' col 0 = remove link
                ' col 1 = Name
                ' '*No Name Found*' should not be saved that way
                If row.Cells(1).Text.ToUpper() = "*NO NAME FOUND*" Then
                    NewOP.Name.DisplayName = ""
                Else
                    NewOP.Name.DisplayName = row.Cells(1).Text
                End If
                ' col 2 = Operator Number
                NewOP.OperatorNum = NewOpNum.ToString()
                ' col 3 = relationship id
                ' If there's a value in the cell use it, if not default to 0 - 'None'
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(row.Cells(3).Text) Then
                    NewOP.RelationshipTypeId = row.Cells(3).Text
                Else
                    NewOP.RelationshipTypeId = "0"
                End If
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators.Add(NewOP)
            Next

            ' Save the quote
            RaiseEvent SaveRequested()
            ShowMessage("Saved")

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
    End Sub
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then Populate()

            Exit Sub
        Catch ex As Exception
            HandleError("PAGE LOAD", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Handles the grid command buttons
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub GridCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim RowIndex As Integer = -1
        Dim OpNum As Integer = -1
        Dim ndx As Integer = 0
        Dim Nm As String = Nothing
        Dim rel As String = Nothing
        Try
            ' Note that the row index is stored in e.CommandArgument, and the command is stored in e.CommandName
            RowIndex = CInt(e.CommandArgument)
            OpNum = grdAssignedOperators.DataKeys(RowIndex).Value
            Nm = grdAssignedOperators.Rows(e.CommandArgument).Cells(1).Text.ToUpper()
            rel = grdAssignedOperators.Rows(e.CommandArgument).Cells(3).Text.ToUpper()

            Select Case e.CommandName.ToUpper()
                Case "REMOVE"
                    For Each op As QuickQuoteOperator In Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators
                        If op.OperatorNum = OpNum Then
                            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators.RemoveAt(ndx)
                            Exit For
                        End If
                        ndx += 1
                    Next
                    RaiseEvent SaveRequested()
                    ShowMessage("Saved")
                    Populate()
                    Exit Select
                Case Else
                    Exit Sub
            End Select
            Exit Sub
        Catch ex As Exception
            HandleError("GridCommand", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' ASSIGN Button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnAssign_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAssign.Click
        Dim li As New ListItem()
        Dim NewOP As New QuickQuoteOperator()
        Dim OpNum As Integer = -1

        Try
            ClearErrorMessage()
            If ddlAvailableOperators.SelectedIndex = -1 OrElse ddlAvailableOperators.SelectedItem.Text = "" Then Exit Sub

            ' Add the operator to the quote
            If Quote Is Nothing OrElse Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 _
                OrElse Quote.Locations(0).RvWatercrafts Is Nothing _
                OrElse Quote.Locations(0).RvWatercrafts.Count <= 0 Then Throw New Exception("There's a problem with the quote object")

            If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators Is Nothing Then
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators = New List(Of QuickQuoteOperator)
            End If

            OpNum = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators.Count + 1

            Select Case ddlAvailableOperators.SelectedValue.ToUpper()
                Case "P1" ' Policyholder 1
                    NewOP.Name = Quote.Policyholder.Name
                    NewOP.OperatorNum = OpNum.ToString()
                    NewOP.RelationshipTypeId = "8" ' Policyholder
                    Exit Select
                Case "P2" ' Policyholder 2
                    NewOP.Name = Quote.Policyholder2.Name
                    NewOP.OperatorNum = OpNum.ToString()
                    NewOP.RelationshipTypeId = "5" ' Policyholder #2
                    Exit Select
                Case Else ' Someone else
                    NewOP.Name.DisplayName = ddlAvailableOperators.SelectedItem.Text
                    NewOP.OperatorNum = OpNum.ToString()
                    NewOP.RelationshipTypeId = "11" ' Not related to policyholder
                    Exit Select
            End Select

            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Operators.Add(NewOP)

            ' Save
            RaiseEvent SaveRequested()
            ShowMessage("Saved")

            ' Repopulate
            Populate()

            Exit Sub
        Catch ex As Exception
            HandleError("btnAssign_Click", ex)
            Exit Sub
        End Try
    End Sub
#End Region

End Class