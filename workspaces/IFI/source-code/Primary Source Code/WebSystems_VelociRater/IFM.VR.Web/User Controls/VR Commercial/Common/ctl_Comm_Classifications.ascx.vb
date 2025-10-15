Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data
Imports System.Data.SqlClient
Imports PopupMessageClass
Imports IFM.VR.Common.Helpers.CL

Public Class ctl_Comm_Classifications
    Inherits VRControlBase

#Region "Declarations"

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Public Property ClassificationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_ClassificationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_ClassificationIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyClassification As QuickQuote.CommonObjects.QuickQuoteClassification
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) AndAlso Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.HasItemAtIndex(ClassificationIndex) Then
                Return Me.Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.GetItemAtIndex(Me.ClassificationIndex)
            End If
            Return Nothing
        End Get
    End Property
    Private ReadOnly Property MyClassificationCountEqualsOne As Boolean
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) AndAlso Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.IsLoaded Then
                Return Me.Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Count = 1
            End If
            Return False
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.ClassificationIndex
        End Get
    End Property

    Private _tblClasses As DataTable = Nothing
    Private Property tblClasses As DataTable
        Get
            Return _tblClasses
        End Get
        Set(value As DataTable)
            _tblClasses = value
            ViewState("ClassDataTable") = value
        End Set
    End Property

    Public Event BuildingClassificationDeleteRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer)
    Public Event BuildingClassificationClearRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer)
    Public Event BuildingPrimaryClassificationChanged(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer)
    Public Event BuildingClassificationChanged(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer, ByVal NewClassCode As String)

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

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        Try
            ClearInputFields()

            If MyClassification IsNot Nothing Then
                ' Don't show the Delete button on the first classification
                If ClassificationIndex = 0 Then
                    lnkDelete.Visible = False
                Else
                    lnkDelete.Visible = True
                End If

                lblAccordHeader.Text = "Building Classification #" & CInt(ClassificationIndex + 1).ToString

                If MyClassification.ClassCode = "" Then
                    ' New classification - don't populate
                    If MyClassification.PredominantOccupancy Then chkPrimaryClassification.Checked = True
                Else
                    ' Existing Classification, display it
                    Dim PgmAbbrev As String = Nothing
                    Dim PgmName As String = Nothing
                    If Not GetProgramNameAndAbbrevByClassificationTypeId(MyClassification.ClassificationTypeId, PgmName, PgmAbbrev, err) Then Throw New Exception(err)
                    ddlProgram.SelectedValue = PgmAbbrev
                    ddlProgram_SelectedIndexChanged(Me, New EventArgs())
                    Dim id As String = GetBOPClassIDByDiaClassificationType_Id(MyClassification.ClassificationTypeId, err)
                    If id Is Nothing Then Throw New Exception(err)
                    ddlClassification.SelectedValue = id
                    'Updated 12/23/2021 for task 65795 MLW
                    txtAnnualReceipts.Text = Format(QQHelper.DecimalForString(MyClassification.AnnualSalesReceipts), "0")
                    'txtAnnualReceipts.Text = MyClassification.AnnualSalesReceipts
                    txtClassCode.Text = MyClassification.ClassCode
                    'Updated 09/10/2021 for BOP Endorsements Task 63912 MLW
                    If Not IsQuoteReadOnly() AndAlso (txtClassCode.Text = "69161" OrElse txtClassCode.Text = "69171") Then trMotelInfoRow.Attributes.Add("style", "display:''")
                    'If txtClassCode.Text = "69161" OrElse txtClassCode.Text = "69171" Then trMotelInfoRow.Attributes.Add("style", "display:''")
                    txtEmployeePayroll.Text = MyClassification.Payroll
                    txtNumOfficersPartnersIndInsureds.Text = MyClassification.NumberOfExecutiveOfficers
                    chkPrimaryClassification.Checked = MyClassification.PredominantOccupancy
                    ' On Endorsements with only one Classification we want the Primary Classification Checked
                    ' Sometimes in Diamond it is left unselected.
                    If IsQuoteEndorsement() AndAlso MyClassificationCountEqualsOne Then chkPrimaryClassification.Checked = True
                End If

                If HotelMotelRemovedRisks.IsHotelMotelRemovedRisksAvailable(Quote) = True AndAlso IsQuoteEndorsement() = False Then
                    Dim ValueToRemove As String = "Motel"
                    If ddlProgram.Items.FindByText(ValueToRemove) IsNot Nothing Then
                        ddlProgram.Items.Remove(ddlProgram.Items.FindByText(ValueToRemove))
                    End If
                End If
            End If

            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Dim cls As QuickQuote.CommonObjects.QuickQuoteClassification = Nothing
        Dim dia_id As String = Nothing
        Dim err As String = Nothing
        Dim pn As String = Nothing
        Dim pa As String = Nothing

        Try
            'Added 10/18/2021 for BOP Endorsements task 61660 MLW
            Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
            If Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
                lblMsg.Text = "&nbsp;"
                If MyClassification Is Nothing Then
                    If Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications Is Nothing Then Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
                    If Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Count <= 0 Then Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.AddNew
                End If

                If MyClassification IsNot Nothing Then
                    If Not GetProgramNameAndAbbrevByBOPClassID(ddlClassification.SelectedValue, pn, pa, err) Then Throw New Exception(err)
                    'MyClassification.Program = pn
                    'MyClassification.ProgramAbbrev = pa
                    dia_id = GetDiaClassificationType_IDByBOPClassID(ddlClassification.SelectedValue, err)
                    If dia_id Is Nothing Then Throw New Exception(err)
                    MyClassification.ClassificationTypeId = If(dia_id.IsNullEmptyorWhitespace(), "0", dia_id)
                    MyClassification.AnnualSalesReceipts = txtAnnualReceipts.Text
                    MyClassification.Payroll = txtEmployeePayroll.Text
                    MyClassification.NumberOfExecutiveOfficers = txtNumOfficersPartnersIndInsureds.Text
                    If chkPrimaryClassification.Checked Then
                        MyClassification.PredominantOccupancy = True
                    Else
                        MyClassification.PredominantOccupancy = False
                    End If
                End If

                Me.SaveChildControls()
            End If
            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            'Added 10/18/2021 for BOP Endorsements task 61660 MLW
            Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
            If Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
                MyBase.ValidateControl(valArgs)
                Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
                Me.ValidationHelper.GroupName = "Location #" & LocationIndex + 1 & ", Building #" & BuildingIndex + 1.ToString & " Classifications"

                If ddlProgram.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError(ddlProgram, "Missing Program", accordList)
                End If

                If ddlClassification.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError(ddlClassification, "Missing Classification", accordList)
                End If

                If txtClassCode.Text.Trim = String.Empty Then
                    Me.ValidationHelper.AddError(txtClassCode, "Missing Class Code", accordList)
                End If

                ' *** Annual Receipts / Number of Officers... ***
                If ddlProgram.SelectedValue = "CTO" OrElse ddlProgram.SelectedValue = "CTS" Then
                    ' When program is Contractors, only one of 'Employee Payroll' and 'Number of Officers...' is required
                    ' and the value of either one can be zero.  Bug 20930
                    If txtEmployeePayroll.Text.Trim = "" AndAlso txtNumOfficersPartnersIndInsureds.Text.Trim = "" Then
                        Dim txt As String = "When 'Contractors' program is selected at least one of 'Employee Payroll' or '# of Officers, Partners or Individual Insureds' is required."
                        Me.ValidationHelper.AddError(txt)
                        Me.ValidationHelper.AddError(txtEmployeePayroll, "Missing Payroll", accordList)
                        Me.ValidationHelper.AddError(txtNumOfficersPartnersIndInsureds, "Missing # of Officers, Partners, Individual Insureds", accordList)
                    Else
                        If txtEmployeePayroll.Text.Trim <> "" Then
                            If Not IsNumeric(txtEmployeePayroll.Text) OrElse CInt(txtEmployeePayroll.Text) < 0 Then
                                Me.ValidationHelper.AddError(txtEmployeePayroll, "Employee Payroll is invalid", accordList)
                            End If
                        End If
                        If txtNumOfficersPartnersIndInsureds.Text.Trim <> "" Then
                            If Not IsNumeric(txtNumOfficersPartnersIndInsureds.Text) OrElse CInt(txtNumOfficersPartnersIndInsureds.Text) < 0 Then
                                Me.ValidationHelper.AddError(txtNumOfficersPartnersIndInsureds, "# of Officers, Partners, Individual Insureds is invalid", accordList)
                            End If
                        End If
                    End If
                Else
                    ' Other than Contractors - Both fields are required and greater than zero
                    If trEmployeePayrollRow.Visible Then
                        If txtEmployeePayroll.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError(txtEmployeePayroll, "Missing Employee Payroll", accordList)
                        Else
                            If Not IsNumeric(txtEmployeePayroll.Text) OrElse CInt(txtEmployeePayroll.Text) < 1 Then
                                Me.ValidationHelper.AddError(txtEmployeePayroll, "Employee Payroll is invalid", accordList)
                            End If
                        End If
                    End If

                    If trOfficersRow.Visible Then
                        If txtNumOfficersPartnersIndInsureds.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError(txtNumOfficersPartnersIndInsureds, "Missing # of Officers, Partners, Individual Insureds", accordList)
                        Else
                            If Not IsNumeric(txtNumOfficersPartnersIndInsureds.Text) OrElse CInt(txtNumOfficersPartnersIndInsureds.Text) < 1 Then
                                Me.ValidationHelper.AddError(txtNumOfficersPartnersIndInsureds, "# of Officers, Partners, Individual Insureds is invalid", accordList)
                            End If
                        End If
                    End If
                End If

                If trAnnualReceiptsRow.Visible AndAlso Not (ddlProgram.SelectedValue = "RS" AndAlso
                                                                    (Me.Quote.Locations(LocationIndex).Buildings(BuildingIndex).OccupancyId = "16")) Then
                    If txtAnnualReceipts.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtAnnualReceipts, "Missing Annual Receipts", accordList)
                    Else
                        If Not IsNumeric(txtAnnualReceipts.Text) OrElse CInt(txtAnnualReceipts.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtAnnualReceipts, "Annual Receipts is invalid", accordList)
                        End If
                    End If
                End If


                Me.ValidateChildControls(valArgs)
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControls", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub SetControlsBasedOnSelectedProgram()
        Try
            trAnnualReceiptsRow.Visible = False
            trEmployeePayrollRow.Visible = False
            trOfficersRow.Visible = False
            trProgramMessageRow.Visible = False
            lblProgramMessage.Text = String.Empty

            Select Case ddlProgram.SelectedValue
                Case "AP"  ' Apartments
                    'Updated 09/09/2021 for BOP Endorsements Task 63912 MLW
                    If Not IsQuoteReadOnly() Then
                        trProgramMessageRow.Visible = True
                        lblProgramMessage.Text = "Please review the <b><u><a target='_blank' href=""" & AppSettings("BOP_Help_BOPHabitationalGuidelines") & """>Habitational Guidelines</a></u></b> for eligibility.  If Apartment is selected, the Habitational Supplemental Application must be completed and mailed to your underwriter. Click here for the <b><u><a target='_blank' href=""" & AppSettings("BOP_Help_BOPHabitationalSupplementalApplication") & """>Habitational Supplemental Application</a></u></b>."
                    End If
                    Exit Select
                Case "CT"  ' Contractors
                    trEmployeePayrollRow.Visible = True
                    trOfficersRow.Visible = True
                    Exit Select
                Case "CTO"  ' Contractors Office
                    trEmployeePayrollRow.Visible = True
                    trOfficersRow.Visible = True
                    'Updated 09/09/2021 for BOP Endorsements Task 63912 MLW
                    If Not IsQuoteReadOnly() Then
                        trProgramMessageRow.Visible = True
                        lblProgramMessage.Text = "If Contractors is selected, the Contractors Supplemental Application must be completed and mailed to your underwriter.  <b><u><a target='_blank' href=""" & AppSettings("BOP_Help_BOPContractorsSupplementalApp") & """>Click here for the Contractors Applications</a></u></b>."
                    End If
                    Exit Select
                Case "CTS"  ' Contractors Service
                    trEmployeePayrollRow.Visible = True
                    trOfficersRow.Visible = True
                    'Updated 09/09/2021 for BOP Endorsements Task 63912 MLW
                    If Not IsQuoteReadOnly() Then
                        trProgramMessageRow.Visible = True
                        lblProgramMessage.Text = "If Contractors is selected, the Contractors Supplemental Application must be completed and mailed to your underwriter.  <b><u><a target='_blank' href=""" & AppSettings("BOP_Help_BOPContractorsSupplementalApp") & """>Click here for the Contractors Applications</a></u></b>."
                    End If
                    Exit Select
                Case "MO"  ' Motel
                    trAnnualReceiptsRow.Visible = True
                    If Not IsQuoteReadOnly() Then
                        trProgramMessageRow.Visible = True
                        lblProgramMessage.Text = "Review <b><u><a target='_blank' href=""" & AppSettings("CL_Help_HotelGuidelines") & """>Hotel Guidelines</a></u></b>. <b><u><a target='_blank' href=""" & AppSettings("CL_Help_HotelSupplementalApplication") & """>Hotel Supplemental App</a></u></b> is required."
                    End If
                    Exit Select
                Case "OF"  ' Office
                    Exit Select
                Case "RE"  ' Retail
                    Exit Select
                Case "RS"  ' Restaurant
                    trAnnualReceiptsRow.Visible = True
                    'Updated 09/09/2021 for BOP Endorsements Task 63912 MLW
                    If Not IsQuoteReadOnly() Then
                        trProgramMessageRow.Visible = True
                        lblProgramMessage.Text = "If Restaurant is selected, the Restaurant Supplemental Application must be completed and mailed to your underwriter.  <b><u><a target='_blank' href=""" & AppSettings("BOP_Help_BOPRestaurantSupplementalApp") & """>Click here for the Restaurant Supplemental Application</a></u></b>."
                    End If
                    Exit Select
                Case "SE"  ' Service
                    Exit Select
                Case "WH"  ' Wholesale
                    Exit Select
                Case Else
                    Exit Sub
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("SetControlsBasedOnSelectedProgram", ex)
            Exit Sub
        End Try
    End Sub

    Private Function GetBOPClassIDByDiaClassificationType_Id(ByVal DiaClassificationType_Id As String, ByRef err As String) As String
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim rtn As Object = Nothing

        Try

            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Dim data = spQuery.GetBOPClassIdFromClassification(QQHelper.IntegerForString(DiaClassificationType_Id))

            If data IsNot Nothing AndAlso IsNumeric(data) Then
                Return data.ToString
            Else
                Throw New Exception("Class record not found")
            End If

            'conn.ConnectionString = AppSettings("ConnQQ")
            'conn.Open()
            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT BOPClass_Id FROM BOPClassNew WHERE UsedInNewBOP = 1 AND Dia_classificationtype_id = " & DiaClassificationType_Id
            'rtn = cmd.ExecuteScalar()

            'If rtn IsNot Nothing AndAlso IsNumeric(rtn) Then
            '    Return rtn.ToString
            'Else
            '    Throw New Exception("Class record not found")
            'End If
        Catch ex As Exception
            err = ex.Message
            HandleError("GetBOPClassIDByDiaClassificationType_Id", ex)
            Return Nothing
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            'End If
            'cmd.Dispose()
        End Try
    End Function

    Private Function GetProgramNameAndAbbrevByBOPClassID(ByVal BOPClass_Id As String, ByRef ProgramName As String, ByRef ProgramAbbrev As String, ByRef err As String) As Boolean
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim da As New SqlDataAdapter()
        'Dim tbl As New DataTable()
        'Dim rtn As Object = Nothing

        Try
            If BOPClass_Id = Nothing OrElse BOPClass_Id = String.Empty OrElse Not IsNumeric(BOPClass_Id) Then Return True
            'conn.ConnectionString = AppSettings("ConnQQ")
            'conn.Open()
            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT ProgramType FROM BOPClassNew WHERE UsedInNewBOP = 1 AND BOPClass_Id = " & BOPClass_Id
            'rtn = cmd.ExecuteScalar()

            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Dim data = spQuery.GetProgramNameByBOPClassId(QQHelper.IntegerForString(BOPClass_Id))

            If data Is Nothing Then Throw New Exception("Record not found!")

            ProgramName = data

            Select Case ProgramName
                Case "Apartments"
                    ProgramAbbrev = "AP"
                    Exit Select
                Case "Contractors"
                    ProgramAbbrev = "CT"
                    Exit Select
                Case "Contractors - Office"
                    ProgramAbbrev = "CTO"
                    Exit Select
                Case "Contractors - Service"
                    ProgramAbbrev = "CTS"
                    Exit Select
                Case "MO???"
                    ProgramAbbrev = "MO"
                    Exit Select
                Case "Office"
                    ProgramAbbrev = "OF"
                    Exit Select
                Case "Retail"
                    ProgramAbbrev = "RE"
                    Exit Select
                Case "Restaurant"
                    ProgramAbbrev = "RS"
                    Exit Select
                Case "Service"
                    ProgramAbbrev = "SE"
                    Exit Select
                Case "Warehouse"
                    ProgramAbbrev = "WH"
                    Exit Select
            End Select


            'If rtn Is Nothing Then Throw New Exception("Record not found!")

            'ProgramAbbrev = rtn.ToString.ToUpper()
            'Select ProgramAbbrev = ProgramAbbrev
            'ProgramAbbrev = "AP"
            '        ProgramName = "Apartments"
            '        Exit Select
            '    ProgramAbbrev = "CT"
            '        ProgramName = "Contractors"
            '        Exit Select
            '    ProgramAbbrev = "CTO"
            '        ProgramName = "Contractors - Office"
            '        Exit Select
            '    ProgramAbbrev = "CTS"
            '        ProgramName = "Contractors - Service"
            '        Exit Select
            '    ProgramAbbrev = "MO"
            '        ProgramName = "MO???"
            '        Exit Select
            '    ProgramAbbrev = "OF"
            '        ProgramName = "Office"
            '        Exit Select
            '    ProgramAbbrev = "RE"
            '        ProgramName = "Retail"
            '        Exit Select
            '    ProgramAbbrev = "RS"
            '        ProgramName = "Restaurant"
            '        Exit Select
            '    ProgramAbbrev = "SE"
            '        ProgramName = "Service"
            '        Exit Select
            '    ProgramAbbrev = "WH"
            '        ProgramName = "Warehouse"
            '        Exit Select
            'End Select

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError("GetProgramNameAndAbbrevByBOPClassID", ex)
            Return False
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            'End If
            'cmd.Dispose()
        End Try
    End Function

    Private Function GetProgramNameAndAbbrevByClassificationTypeId(ByVal ClassificationType_Id As String, ByRef ProgramName As String, ByRef ProgramAbbrev As String, ByRef err As String) As Boolean
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim da As New SqlDataAdapter()
        'Dim tbl As New DataTable()
        'Dim rtn As Object = Nothing

        Try

            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Dim data = spQuery.GetProgramNameFromClassification(QQHelper.IntegerForString(ClassificationType_Id))

            If data Is Nothing Then Throw New Exception("Record not found!")

            ProgramName = data

            Select Case ProgramName
                Case "Apartments"
                    ProgramAbbrev = "AP"
                    Exit Select
                Case "Contractors"
                    ProgramAbbrev = "CT"
                    Exit Select
                Case "Contractors - Office"
                    ProgramAbbrev = "CTO"
                    Exit Select
                Case "Contractors - Service"
                    ProgramAbbrev = "CTS"
                    Exit Select
                Case "MO???"
                    ProgramAbbrev = "MO"
                    Exit Select
                Case "Office"
                    ProgramAbbrev = "OF"
                    Exit Select
                Case "Retail"
                    ProgramAbbrev = "RE"
                    Exit Select
                Case "Restaurant"
                    ProgramAbbrev = "RS"
                    Exit Select
                Case "Service"
                    ProgramAbbrev = "SE"
                    Exit Select
                Case "Warehouse"
                    ProgramAbbrev = "WH"
                    Exit Select
            End Select
            'conn.ConnectionString = AppSettings("ConnQQ")
            'conn.Open()
            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT ProgramType FROM BOPClassNew WHERE UsedInNewBOP = 1 AND Dia_classificationtype_id = " & ClassificationType_Id
            'rtn = cmd.ExecuteScalar()

            'If rtn Is Nothing Then Throw New Exception("Record not found!")

            'ProgramAbbrev = rtn.ToString.ToUpper()
            'Select Case ProgramAbbrev
            '    ProgramAbbrev = "AP"
            '    ProgramName = "Apartments"
            '    Exit Select
            '    ProgramAbbrev = "CT"
            '    ProgramName = "Contractors"
            '    Exit Select
            '    ProgramAbbrev = "CTO"
            '    ProgramName = "Contractors - Office"
            '    Exit Select
            '    ProgramAbbrev = "CTS"
            '    ProgramName = "Contractors - Service"
            '    Exit Select
            '    ProgramAbbrev = "MO"
            '    ProgramName = "MO???"
            '    Exit Select
            '    ProgramAbbrev = "OF"
            '    ProgramName = "Office"
            '    Exit Select
            '    ProgramAbbrev = "RE"
            '    ProgramName = "Retail"
            '    Exit Select
            '    ProgramAbbrev = "RS"
            '    ProgramName = "Restaurant"
            '    Exit Select
            '    ProgramAbbrev = "SE"
            '    ProgramName = "Service"
            '    Exit Select
            '    ProgramAbbrev = "WH"
            '    ProgramName = "Warehouse"
            '    Exit Select
            'End Select

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError("GetProgramNameAndAbbrevByClassificationTypeId", ex)
            Return False
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            'End If
            'cmd.Dispose()
        End Try
    End Function

    Private Function GetClassCodeByBOPClassID(ByVal BOPClass_Id As String, ByRef err As String) As String
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim rtn As Object = Nothing

        Try
            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Dim data = spQuery.GetClassCodeByBOPClassId(QQHelper.IntegerForString(BOPClass_Id))
            If String.IsNullOrWhiteSpace(data) Then Throw New Exception("Class record not found")
            Return data


            'conn.ConnectionString = AppSettings("ConnQQ")
            'conn.Open()
            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT ClassCode FROM BOPClassNew WHERE UsedInNewBOP = 1 AND BOPClass_Id = " & BOPClass_Id
            'rtn = cmd.ExecuteScalar()

            'If rtn IsNot Nothing AndAlso IsNumeric(rtn) Then
            '    Return rtn.ToString
            'Else
            '    Throw New Exception("Class record not found")
            'End If
        Catch ex As Exception
            err = ex.Message
            HandleError("GetClassCodeByBOPClassID", ex)
            Return Nothing
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            'End If
            'cmd.Dispose()
        End Try
    End Function

    Private Function GetDiaClassificationType_IDByBOPClassID(ByVal BOPClass_Id As String, ByRef err As String) As String
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim rtn As Object = Nothing

        Try
            If BOPClass_Id = Nothing OrElse BOPClass_Id = String.Empty OrElse Not IsNumeric(BOPClass_Id) Then Return ""

            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Dim data = spQuery.GetClassificationtypeByBOPClassId(QQHelper.IntegerForString(BOPClass_Id))
            If String.IsNullOrWhiteSpace(data) Then Throw New Exception("Class record not found")
            Return data

            'conn.ConnectionString = AppSettings("ConnQQ")
            'conn.Open()
            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT Dia_classificationtype_id FROM BOPClassNew WHERE UsedInNewBOP = 1 AND BOPClass_Id = " & BOPClass_Id
            'rtn = cmd.ExecuteScalar()

            'If rtn IsNot Nothing AndAlso IsNumeric(rtn) Then
            '    Return rtn.ToString
            'Else
            '    Throw New Exception("Class record not found")
            'End If
        Catch ex As Exception
            err = ex.Message
            HandleError("GetDiaClassificationType_IDByBOPClassID", ex)
            Return Nothing
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            'End If
            'cmd.Dispose()
        End Try
    End Function

    Private Function GetNextClassTableID() As Integer
        Dim highest As Integer = -1

        Try
            If tblClasses Is Nothing OrElse tblClasses.Rows.Count = 0 Then Return 0

            For Each dr In tblClasses.Rows
                If dr("id") > highest Then highest = CInt(dr("id"))
            Next

            Return highest + 1
        Catch ex As Exception
            HandleError("GetNextClassTableID", ex)
            Return -1
        End Try
    End Function

    Private Sub ClearInputFields()
        Try
            ddlProgram.SelectedIndex = -1
            ddlClassification.SelectedIndex = -1
            txtClassCode.Text = String.Empty
            txtAnnualReceipts.Text = String.Empty
            txtEmployeePayroll.Text = String.Empty
            txtNumOfficersPartnersIndInsureds.Text = String.Empty
            trMotelInfoRow.Attributes.Add("style", "display:none")

            Exit Sub
        Catch ex As Exception
            HandleError("ClearInputFields", ex)
            Exit Sub
        End Try
    End Sub

    Private Function GetClassificationFootnoteText(ByVal BOPClass_id As String, ByRef err As String) As String
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim rtn As Object = Nothing
        Dim msg As String = Nothing

        Try
            ' Some special logic for Lawyers and Doctors classifications...
            ' 246, 247, 248, 249, 1339, 1340, 1341 = Lawyers
            ' 238, 239, 240, 241, 1317, 1318, 1319 = Medical
            Select Case BOPClass_id
                'Case "246", "247", "248", "249", "1339", "1340", "1341", "238", "239", "240", "241", "1317", "1318", "1319"
                Case "1339", "1340", "1341", "1317", "1318", "1319"
                    ' Doctors and lawyers classifications cannot have EPLI coverage on the quote
                    If Quote IsNot Nothing Then
                        If IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLI_Is_Applied(Me.Quote) Then
                            msg = "EPLI Not allowed for this class code, endorsement will be removed from quote"
                            Save_FireSaveEvent(False)
                            IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(Me.Quote, False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                        Else
                            msg = ""
                        End If
                    End If
                    Exit Select
                Case "246", "247", "248", "249", "238", "239", "240", "241"
                    msg = ""
                    Exit Select
                Case Else

                    Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
                    msg = spQuery.GetFootnoteByBOPClassId(QQHelper.IntegerForString(BOPClass_id))
                    If String.IsNullOrWhiteSpace(msg) Then msg = String.Empty


                    '    conn.ConnectionString = AppSettings("connQQ")
                    'conn.Open()

                    'cmd.Connection = conn
                    'cmd.CommandType = CommandType.Text
                    'cmd.CommandText = "SELECT Footnote FROM BOPClassNew WHERE UsedInNewBOP = 1 AND BOPClass_id = " & BOPClass_id
                    'rtn = cmd.ExecuteScalar()

                    '    If rtn Is Nothing Then
                    '    msg = ""
                    'Else
                    '    msg = rtn.ToString()
                    'End If
                    Exit Select
            End Select

            Return msg
        Catch ex As Exception
            HandleError("GetClassificationFootnoteText", ex)
            Return Nothing
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            'End If
            'cmd.Dispose()
        End Try
    End Function

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If ViewState("ClassDataTable") IsNot Nothing Then tblClasses = ViewState("ClassDataTable")

            If Not IsPostBack Then
                'Me.MainAccordionDivId = Me.divBuildingClassification.ClientID
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ddlProgram_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlProgram.SelectedIndexChanged
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim li As New ListItem()

        Try
            ' Load the Classification ddl
            ddlClassification.Items.Clear()

            txtAnnualReceipts.Text = String.Empty
            txtClassCode.Text = String.Empty
            txtEmployeePayroll.Text = String.Empty
            txtNumOfficersPartnersIndInsureds.Text = String.Empty

            If ddlProgram.SelectedIndex <= 0 Then Exit Sub

            If ddlProgram.SelectedValue <> String.Empty Then

                Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
                tbl = spQuery.GetClassDescByProgramType(ddlProgram.SelectedValue)

                'conn.ConnectionString = AppSettings("ConnQQ")
                'conn.Open()
                'cmd.Connection = conn
                'cmd.CommandType = CommandType.Text
                'cmd.CommandText = "SELECT * FROM BOPCLASSNEW WHERE PROGRAMTYPE = '" & ddlProgram.SelectedValue & "' AND UsedInNewBOP = 1 ORDER BY CLASSDESC ASC"
                'da.SelectCommand = cmd
                'da.Fill(tbl)

                If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                    li = New ListItem()
                    li.Text = ""
                    li.Value = ""
                    ddlClassification.Items.Add(li)
                    For Each dr As DataRow In tbl.Rows
                        li = New ListItem()
                        li.Text = dr("ClassDesc").ToString
                        li.Value = dr("BOPClass_Id").ToString
                        'li.Value = dr("ClassCode").ToString() & "-" & dr("ClassDesc").ToString() & "{[" & dr("dia_classificationtype_id").ToString() & "]}"
                        ddlClassification.Items.Add(li)
                    Next
                End If
            End If

            ' Set controls based on selected program
            SetControlsBasedOnSelectedProgram()

            ' Note that we don't send new classification when the program changes
            RaiseEvent BuildingClassificationChanged(LocationIndex, BuildingIndex, ClassificationIndex, "")

            Exit Sub
        Catch ex As Exception
            HandleError("ddlProgram_SelectedIndexChanged", ex)
            Exit Sub
        Finally
            'If conn IsNot Nothing Then
            '    If conn.State = ConnectionState.Open Then conn.Close()
            'End If
            'conn.Dispose()
            'cmd.Dispose()
            'da.Dispose()
            tbl.Dispose()
        End Try
    End Sub

    Private Sub ddlClassification_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlClassification.SelectedIndexChanged
        Dim err As String = Nothing
        Dim cc As String = Nothing
        Dim ShowMsg As Boolean = False
        Dim msg As String = String.Empty

        Try
            If ddlClassification.SelectedIndex <= 0 Then Exit Sub

            cc = GetClassCodeByBOPClassID(ddlClassification.SelectedValue, err)
            If cc Is Nothing Then Throw New Exception(err)

            txtClassCode.Text = cc

            ' MOTEL MESSAGE
            If txtClassCode.Text = "69161" OrElse txtClassCode.Text = "69171" Then
                trMotelInfoRow.Attributes.Add("style", "display:''")
            Else
                trMotelInfoRow.Attributes.Add("style", "display:none")
            End If

            ' CLASS-SPECIFIC POPUPS
            msg = GetClassificationFootnoteText(ddlClassification.SelectedValue, err)
            If msg IsNot Nothing AndAlso msg <> String.Empty Then
                msg = msg.Replace("@BR@", "<BR />")
                ShowMsg = True
            End If

            If ShowMsg Then
                ' Use popup library
                Using popup As New PopupMessageObject(Me.Page, msg, ddlClassification.SelectedItem.Text)
                    With popup
                        .isFixedPositionOnScreen = True
                        .ZIndexOfPopup = 2
                        .isModal = True
                        .Image = PopupMessageObject.ImageOptions.None
                        .hideCloseButton = True
                        .AddButton("OK", True)
                        .CreateDynamicPopUpWindow()
                    End With
                End Using
            End If

            Save_FireSaveEvent(False)
            RaiseEvent BuildingClassificationChanged(LocationIndex, BuildingIndex, ClassificationIndex, txtClassCode.Text)
            'Matt A - The accordion doesn't like these two lines and as far as I can tell they are not absolutely needed
            'Save_FireSaveEvent(False)
            'Populate_FirePopulateEvent()

            Exit Sub
        Catch ex As Exception
            HandleError("ddlClassification_SelectedIndexChanged", ex)
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
        RaiseEvent BuildingClassificationClearRequested(LocationIndex, BuildingIndex, ClassificationIndex)
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent BuildingClassificationDeleteRequested(LocationIndex, BuildingIndex, ClassificationIndex)
    End Sub

    Private Sub chkPrimaryClassification_CheckedChanged(sender As Object, e As EventArgs) Handles chkPrimaryClassification.CheckedChanged
        If chkPrimaryClassification.Checked Then RaiseEvent BuildingPrimaryClassificationChanged(LocationIndex, BuildingIndex, ClassificationIndex)
    End Sub

#End Region

End Class