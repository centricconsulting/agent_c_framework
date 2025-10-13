Imports IFM
Imports IFM.IFMErrorLogging
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.VisualBasic

Public Class FNOLCommonLibrary

    '******************************************************************
    '* FNOL COMMON LIBRARY
    '******************************************************************
    '* Used by the FNOL Input Page (TstationTransactions, TS_FNOL) and
    '* the FNOL Claim Assignment website.
    '* 
    '* Contains functions that are used across the claims entry and 
    '* assignment processes.
    '******************************************************************
    '* Change Log
    '* --------------------------------------------------------------
    '* 12/16/2020 - MGB - Class created for Automatic Claim Assignment 
    '*                    Project.
    '******************************************************************

#Region "Declarations"

#Region "Common Declarations"
    ' Common
    Private Const ClassName As String = "FNOLCommonLibrary"
    Dim strConnFNOL As String = AppSettings("connFNOL")
    Dim strConnDiamond As String = AppSettings("connDiamond")

    Public Structure NameFields
        Public FirstName As String
        Public MiddleName As String
        Public LastName As String
    End Structure

    Public NumFormatWithCents As String = "$###,###,###.00"
    Public NumFormatNoCents As String = "$###,###,###"

#End Region

#Region "FNOL Claim Assignment Decalarations"
    ' ------------------------------------------------------
    ' FNOL CLAIM ASSIGNMENT --------------------------
    ' ------------------------------------------------------
    Public Enum AdjusterCountUpdateIndicator
        Increase
        Decrease
    End Enum

    Public Enum EnableDisable_Enum
        Enable
        Disable
    End Enum

    Public Enum LOB_enum
        Auto
        Liability
        Propperty
        None
    End Enum

    Public Enum ShowValue_enum
        ShownOnly
        HiddenOnly
        Both
    End Enum

    Public Enum AssignedToManager_enum
        Assigned
        Unassigned
        All
    End Enum

    Public Enum PermissionName_enum
        SiteAdmin
        AdjusterAdmin
        UnassignedClaimsAccess
        AssignedClaimsAccess
        AssignClaim
        ClaimsManager
        ClaimsAdjuster
    End Enum

    Public Structure FNOLMailObject_Structure
        Public Subject As String
        Public body As String
        Public ID As String
        Public FNOLData As FNOLData_Structure
    End Structure

    Public Structure FNOLData_Structure
        Public DiamondClaimNumber As String
        Public AdjusterName As String
        Public TerritoryNumber As String
        Public ClaimDate As DateTime
        Public PolicyNumber As String
        Public LossDate As DateTime
        Public AgencyName As String
        Public PrimaryContact As String
        Public InsuredFirst As String
        Public InsuredLast As String
        Public PhoneHome As String
        Public PhoneBusiness As String
        Public PhoneCell As String
        Public InsuredAddressStreet As String
        Public InsuredAddressCity As String
        Public InsuredAddressState As String
        Public InsuredAddressZip As String
        Public Description As String
        Public LOB As String
        Public CAT As Boolean
        Public ContractorBusinessName As String
        Public ContractorContactName As String
        Public ContractorBusinessPhone As String
        Public ContractorEmail As String
        Public ContractorRemarks As String
    End Structure

    ''' <summary>
    ''' Structure to hold information required by the SendEmail function
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure EmailInfo_Structure_FNOLCA
        Public ToAddress As String
        Public CCAddress As String
        Public PolicyHolderFirstName As String
        Public PolicyHolderLastName As String
        Public PolicyNumber As String
        Public Body As String
        Public SubjectLine_OPTIONAL As String
        Public FromAddress_OPTIONAL As String
        Public MailHost_OPTIONAL As String
    End Structure

    Public Structure AssignFNOL_ReturnObject
        Public Success As Boolean
        Public ProcessMessages As List(Of String)
        Public ErrorMessages As List(Of String)
        Public WarningMessages As List(Of String)
    End Structure

    Public Enum PersonType_Enum
        OtherVehicleOwner
        OtherVehicleDriver
        Injured
        Witness
    End Enum

#End Region

#Region "FNOL INPUT PAGE DECLARATIONS"
    ' ------------------------------------------------------
    ' FNOL INPUT PAGE --------------------------------------
    ' ------------------------------------------------------
    ''' <summary>
    ''' Structure to hold information required by the SendEmail function
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure EmailInfo_Structure_FNOLInputPage
        Public ConfirmEmail As String
        Public PolicyHolderFirstName As String
        Public PolicyHolderLastName As String
        Public PolicyNumber As String
        Public Body As String
        Public SubjectLine_OPTIONAL As String
        Public FromAddress_OPTIONAL As String
        Public MailHost_OPTIONAL As String
    End Structure

    ''' <summary>
    ''' Parameter structure for sp_Insert_Auto_Loss
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure sp_Insert_Auto_Loss_Parameters
        Public NameID As Integer
        Public VehicleID As Integer
        Public PolicyNumber As String
        Public DateOfLoss As String
        Public DescriptionOfLoss As String
        Public AgencyCode As String
        Public DeductibleAmount As String
        Public InsuredResidencePhone As String
        Public InsuredBusinessPhone As String
        Public ContactName As String 'added 1/22/2013
        Public ContactFirstName As String
        Public ContactMiddleName As String
        Public ContactLastName As String
        Public ContactResidencePhone As String
        Public ContactBusinessPhone As String
        Public LossLocationAdd1 As String
        Public LossLocationCity As String
        Public LossLocationState As String
        Public LossLocationZip As String
        Public DescOfLoss As String
        Public VehicleNumber As String
        Public OwnerNameAddress As String
        Public Damage As String
        Public PlateNumber As String
        Public DescribePropertyDamaged As String
        Public CompanyAgencyName As String
        Public OwnerFirstName As String
        Public OwnerMiddleName As String
        Public OwnerLastName As String
        Public OwnerBusinessPhone As String
        Public OwnerResidencePhone As String
        Public OtherDriverFirstName As String
        Public OtherDriverMiddleName As String
        Public OtherDriverLastName As String
        Public OtherDriverBusinessPhone As String
        Public OtherDriverResidencePhone As String
        Public DescribeDamage As String
        Public InjuredName As String
        Public InjuredPhone As String
        Public InjuredPed As String
        Public InjuredInsVehicle As String
        Public InjuredOtherVehicle As String
        Public InjuredAge As String
        Public InjuredExtent As String
        Public WitnessNameAddress As String
        Public DriverFirst As String
        Public DriverMI As String
        Public DriverLast As String
        Public DriverID As String
        Public Coverages As String
        Public MortgageeID As String
        Public InsuredEmail As String
        Public Comments As String
        Public InsuredCell As String
        Public DriverCell As String
        Public OwnerCell As String
        Public OtherCell As String
        Public VehicleLocation As String
        Public PoliceContacted As String
        Public diaClaimNum As String
    End Structure

    ''' <summary>
    ''' Parameter structure for sp_Insert_Property_Loss
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure sp_Insert_Property_Loss_Parameters
        Public NameID As String
        Public PolicyNumber As String
        Public DateOfLoss As String
        Public KindOfLoss As String
        Public AgencyCode As String
        Public DeductibleAmount As String
        Public InsResPh As String
        Public InsBusPh As String
        Public ContactName As String
        Public ContactFirstName As String
        Public ContactMiddleName As String
        Public ContactLastName As String
        Public ContactResidencePhone As String
        Public ContactBusinessPhone As String
        Public LossLocationAdd1 As String
        Public LossLocationCity As String
        Public LossLocationState As String
        Public LossLocationZip As String
        Public DescOfLoss As String
        Public FirePoliceReported As String
        Public AmountOfLoss As String
        Public OtherInsurance As String
        Public Remarks As String
        Public AdjusterAssigned As String
        Public ReportedBy As String
        Public diaClaimNum As String
        Public ContractorBusinessName As String
        Public ContractorContactName As String
        Public ContractorBusinessPhone As String
        Public ContractorEmail As String
        Public ContractorRemarks As String
    End Structure

    ''' <summary>
    ''' Parameter structure for sp_Insert_General_Loss
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure sp_Insert_General_Loss_Parameters
        Public PolicyNumber As String
        Public DateOfLoss As String
        Public TimeOfLoss As String
        Public AgentCode As String
        Public InsuredID As Integer
        Public InsuredResidencePhone As String
        Public InsuredBusinessPhone As String
        Public InsuredCellPhone As String
        Public InsuredEmailAddress As String
        Public PersonToContact As String
        Public ContactFirstName As String 'added 1/22/2013
        Public ContactMiddleName As String 'added 1/22/2013
        Public ContactLastName As String 'added 1/22/2013
        Public LocationOfLossAddress As String
        Public LocationOfLossCity As String
        Public LocationOfLossState As String
        Public LocationOfLossZipCode As String
        Public DescriptionOfLossEvent As String
        Public AuthoritiesContacted As String
        Public PropertyDamageInjury As String
        Public ReportedBy As String
        Public diaClaimNum As String
    End Structure

    ''' <summary>
    ''' Parameter structure for sp_Insert_ManualDraft
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure sp_Insert_ManualDraft_Parameters
        Public SubmittedBy As String
        Public DraftNumber As String
        Public DraftAmount As Decimal
        Public DraftDate As DateTime
        Public ClaimNumber As String
        Public PolicyNumber As String
        Public UsrID As String
        Public Void As Integer
        Public Payee As String
    End Structure

    Public Structure PersonsRecord_structure
        Public id As String
        Public FirstName As String
        Public MiddleName As String
        Public LastName As String
        Public HomePhone As String
        Public BusinessPhone As String
        Public CellPhone As String
        Public FAX As String
        Public Email As String
        Public Address As String
        Public City As String
        Public State As String
        Public Zip As String
        Public InjuryDescription As String
        Public InjuryType As String
        Public InjuredAge As String
        Public InjuredOccupation As String
        Public InjuredDoing As String
        Public InjuredTaken As String
    End Structure

    Public Enum PersonsType_enum
        Injured
        OtherVehicleOwner
        OtherVehicleDriver
        Witness
    End Enum

    Public Enum SafeliteLOB
        Auto
        Liability
        Propertty
        Invalid
    End Enum

#End Region

#End Region

#Region "Common Methods and Functions"
    ''' <summary>
    ''' Parses a combined name field into it's elements
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseNameField(ByVal strName As String) As NameFields
        Dim SpcPos As Integer = -1
        Dim SpcPos2 As Integer = -1
        Dim rtn As New NameFields()

        Try
            SpcPos = strName.IndexOf(" ", 0)
            If SpcPos < 0 Then
                rtn.LastName = strName
                Return rtn
            End If
            SpcPos2 = strName.IndexOf(" ", SpcPos + 1)

            If SpcPos2 > 0 Then
                rtn.FirstName = strName.Substring(0, SpcPos)
                rtn.MiddleName = strName.Substring(SpcPos + 1, (SpcPos2 - SpcPos) - 1)
                rtn.LastName = strName.Substring(SpcPos2 + 1)
                Return rtn
            End If

            rtn.FirstName = strName.Substring(0, SpcPos)
            rtn.LastName = strName.Substring(SpcPos + 1)
            Return rtn
        Catch ex As Exception
            HandleError(ClassName, "ParseNameField", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' FROM FNOLClaimAssignment
    ''' Standard error handler.
    ''' Displays the message if a label was passed and writes the error log.
    ''' This signature is used by the claim assignment site.
    ''' </summary>
    ''' <param name="ClassName"></param>
    ''' <param name="RoutineName"></param>
    ''' <param name="ex"></param>
    ''' <param name="msglabel"></param>
    ''' <remarks></remarks>
    Public Sub HandleError(ByVal ClassName As String, ByVal RoutineName As String, ByVal ex As Exception, Optional ByRef msglabel As Label = Nothing)
        Dim errRec As New IFM.ErrLog_Parameters_Structure()
        Dim err As String = Nothing

        Try
            ' If a label was passed, set it's text
            If msglabel IsNot Nothing Then msglabel.Text = ClassName & "(" & RoutineName & "): " & ex.Message

            ' IT Error Logging
            errRec.ApplicationName = "FNOL Claim Assignment"
            errRec.ClassName = "FNOLClaimAssignment.aspx.vb"
            errRec.ErrorMessage = ex.Message
            errRec.LogDate = DateTime.Now
            errRec.RoutineName = RoutineName
            errRec.StackTrace = ex.StackTrace
            IFM.WriteErrorLogRecord(errRec, err)

            Exit Sub
        Catch ex1 As Exception
            Exit Sub
        End Try
        Exit Sub
    End Sub

    ''' <summary>
    ''' FROM FNOL INPUT PAGE
    ''' Standard error handler.
    ''' This signature is used by the FNOL Input Page methods and functions and will display the error message (if a label was passed),
    ''' write the error log, and send an error email.
    ''' This signature is used by the FNOL Input Page.
    ''' </summary>
    ''' <param name="strClassName"></param>
    ''' <param name="strRoutineName"></param>
    ''' <param name="exc"></param>
    ''' <remarks></remarks>
    Public Sub HandleError(ByVal strClassName As String, ByVal strRoutineName As String, ByRef exc As Exception, ByRef pg As Page, ByVal PolicyNum As String, Optional ByRef MessageLabel As Label = Nothing, Optional ByRef blSendErrorEmail As Boolean = False)
        Dim strScript As String = "<script language=JavaScript>"
        Dim message As String = Nothing
        Dim rec As New IFM.ErrLog_Parameters_Structure()
        Dim err As String = Nothing

        Try
            ' MGB 4/6/16 Ignore thread abort errors
            If exc.Message.ToUpper.Contains("THREAD WAS BEING ABORTED") Then Exit Sub

            ' Build the message string
            message = "Error Detected in " & strClassName & "(" & strRoutineName & "): " & exc.Message

            ' Update the message label text if one was passed
            If MessageLabel IsNot Nothing Then MessageLabel.Text = message

            ' Write the error log
            rec.ApplicationName = "TstationTransactions4.0 (FNOL)"
            rec.ClassName = strClassName
            rec.ErrorMessage = exc.Message
            rec.LogDate = DateTime.Now
            rec.RoutineName = strRoutineName
            rec.StackTrace = exc.StackTrace
            IFM.WriteErrorLogRecord(rec, err)

            If blSendErrorEmail Then
                SendErrorEmail("Error in TS_FNOL", PolicyNum, GetErrorString(ClassName, strRoutineName, exc, pg, PolicyNum, MessageLabel), pg, PolicyNum, MessageLabel)
            End If

            Exit Sub
        Catch ex As Exception
            ' This will cause an infinite loop if the email server fails, so don't do this!
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "HandleError", ex))
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Checks to see if the indicated field has a value other than string.empty or nothing.  Returns true if a value was found, false if not.
    ''' This signature uses the Claim Assignment site error handling.
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <param name="FieldName"></param>
    ''' <returns></returns>
    Public Function DataFieldHasValue(ByRef dr As DataRow, ByVal FieldName As String) As Boolean
        Try
            If IsDBNull(dr(FieldName)) Then Return False
            If dr(FieldName).ToString.Trim = String.Empty Then Return False

            Return True
        Catch ex As Exception
            ' 04/27/2022 CAH - No User info provided.  Useless logs.
            'HandleError(ClassName, "DataFieldHasValue", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if the indicated field has a value other than string.empty or nothing.  Returns true if a value was found, false if not.
    ''' This signature uses the FNOL Input Page error handling.
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="dr"></param>
    ''' <param name="FieldName"></param>
    ''' <returns></returns>
    Public Function DataFieldHasValue(ByRef pg As Page, ByRef dr As DataRow, ByVal FieldName As String) As Boolean
        Try
            If IsDBNull(dr(FieldName)) Then Return False
            If dr(FieldName).ToString.Trim = String.Empty Then Return False

            Return True
        Catch ex As Exception
            ' 04/27/2022 CAH - No User info provided.  Useless logs.
            'HandleError(ClassName, "DataFieldHasValue", ex, pg, "")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if a table field has a value, and will also check for numeric if requested.
    ''' This signature uses a DataRow.
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="dr"></param>
    ''' <param name="fieldnm"></param>
    ''' <param name="CheckForNumeric"></param>
    ''' <param name="GreaterThanZero"></param>
    ''' <returns></returns>
    Public Function DBFieldHasValue(ByRef pg As Page, ByVal dr As DataRow, ByVal fieldnm As String, Optional CheckForNumeric As Boolean = False, Optional GreaterThanZero As Boolean = False) As Boolean
        Try
            If dr Is Nothing Then Return False
            If IsDBNull(dr(fieldnm)) Then Return False
            If dr(fieldnm).ToString.Trim = String.Empty Then Return False

            If CheckForNumeric Then
                If IsNumeric(dr(fieldnm).ToString) Then
                    If GreaterThanZero Then
                        If CDec(dr(fieldnm)) > 0 Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return True
                    End If
                Else
                    Return False
                End If
            Else
                Return True
            End If
        Catch ex As Exception
            HandleError(ClassName, "DBFieldHasValue", ex, pg, "")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if a table field has a value, and will also check for numeric if requested.
    ''' This signature uses a DataReader.
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="dr"></param>
    ''' <param name="fieldnm"></param>
    ''' <param name="CheckForNumeric"></param>
    ''' <param name="GreaterThanZero"></param>
    ''' <returns></returns>
    Public Function DBFieldHasValue(ByRef pg As Page, ByVal dr As SqlDataReader, ByVal fieldnm As String, Optional CheckForNumeric As Boolean = False, Optional GreaterThanZero As Boolean = False) As Boolean
        Try
            If dr Is Nothing Then Return False
            If IsDBNull(dr(fieldnm)) Then Return False
            If dr(fieldnm).ToString.Trim = String.Empty Then Return False

            If CheckForNumeric Then
                If IsNumeric(dr(fieldnm).ToString) Then
                    If GreaterThanZero Then
                        If CDec(dr(fieldnm)) > 0 Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return True
                    End If
                Else
                    Return False
                End If
            Else
                Return True
            End If
        Catch ex As Exception
            HandleError(ClassName, "DBFieldHasValue", ex, pg, "")
            Return False
        End Try
    End Function


#End Region

#Region "FNOL Claim Assignment Methods and Functions"

#Region "Claim Adjuster Count Methods and Functions"

    ''' <summary>
    ''' Attempts to open the passed connection - used by Claim Adjuster Counts (CAC)
    ''' </summary>
    ''' <param name="cn"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function OpenCACConnection(ByRef cn As SqlConnection) As Boolean
        Try
            cn = New SqlConnection()
            cn.ConnectionString = strConnFNOL

            cn.Open()

            Return True
        Catch ex As Exception
            HandleError(ClassName, "OpenConnection", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Formats a date to MM/DD/YYYY
    ''' </summary>
    ''' <param name="Dt"></param>
    ''' <returns></returns>
    Public Function FormatDate(ByVal Dt As DateTime) As String
        Try
            Return Dt.Month.ToString.PadLeft(2, "0") & "/" & Dt.Day.ToString.PadLeft(2, "0") & "/" & Dt.Year.ToString
        Catch ex As Exception
            HandleError(ClassName, "FormatDate", ex)
            Return Dt
        End Try
    End Function

    ''' <summary>
    ''' Gets the start date for the current week.  The start day is always monday
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCurrentWeekStartDate() As String
        Dim AddDays As Integer = 0
        Dim NewDate As DateTime = Nothing
        Try
            ' Start day is always monday
            Select Case DateTime.Now.DayOfWeek
                Case DayOfWeek.Monday
                    AddDays = 0
                    Exit Select
                Case DayOfWeek.Tuesday
                    AddDays = -1
                    Exit Select
                Case DayOfWeek.Wednesday
                    AddDays = -2
                    Exit Select
                Case DayOfWeek.Thursday
                    AddDays = -3
                    Exit Select
                Case DayOfWeek.Friday
                    AddDays = -4
                    Exit Select
                Case DayOfWeek.Saturday
                    AddDays = -5
                    Exit Select
                Case DayOfWeek.Sunday
                    AddDays = -6
                    Exit Select
                Case Else
                    Return ""
            End Select

            NewDate = DateTime.Now.AddDays(AddDays)
            Return FormatDate(NewDate)
        Catch ex As Exception
            HandleError(ClassName, "GetCurrentWeekStartDate", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Gets the date of the last day of the current week.  The last day is always sunday
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCurrentWeekEndDate() As String
        Dim AddDays As Integer = 0
        Dim NewDate As DateTime = Nothing

        Try
            ' End day is always Sunday
            Select Case DateTime.Now.DayOfWeek
                Case DayOfWeek.Monday
                    AddDays = 6
                    Exit Select
                Case DayOfWeek.Tuesday
                    AddDays = 5
                    Exit Select
                Case DayOfWeek.Wednesday
                    AddDays = 4
                    Exit Select
                Case DayOfWeek.Thursday
                    AddDays = 3
                    Exit Select
                Case DayOfWeek.Friday
                    AddDays = 2
                    Exit Select
                Case DayOfWeek.Saturday
                    AddDays = 1
                    Exit Select
                Case DayOfWeek.Sunday
                    AddDays = 0
                    Exit Select
                Case Else
                    Return ""
            End Select

            NewDate = DateTime.Now.AddDays(AddDays)
            Return FormatDate(NewDate)
        Catch ex As Exception
            HandleError(ClassName, "GetCurrentWeekStartDate", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Gets claim counts
    ''' </summary>
    ''' <param name="StartDate"></param>
    ''' <param name="EndDate"></param>
    ''' <param name="GroupID"></param>
    ''' <param name="AdjusterID"></param>
    ''' <returns></returns>
    Public Function GetClaimAssignmentCounts(ByVal StartDate As String, ByVal EndDate As String, ByVal GroupID As String, Optional ByVal AdjusterID As String = Nothing) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_GetCounts"
            cmd.Parameters.AddWithValue("@StartDate", StartDate)
            cmd.Parameters.AddWithValue("@EndDate", EndDate)
            If AdjusterID IsNot Nothing Then cmd.Parameters.AddWithValue("@AdjusterID", AdjusterID)
            If GroupID IsNot Nothing Then cmd.Parameters.AddWithValue("@GroupID", GroupID)
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count > 0 Then
                tbl.DefaultView.Sort = "Groups_Id ASC"
                Return tbl
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetClaimAssignmentCounts", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a table of all available group records
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetGroupList() As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM Groups ORDER BY GROUPNAME ASC"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count > 0 Then
                Return tbl
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetGroupList", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a table of all groups that a user is assigned to
    ''' </summary>
    ''' <param name="FNOLClaimAssignAdjuster_ID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdjusterGroupsByFNOLClaimAssignAdjusterID(ByVal FNOLClaimAssignAdjuster_ID As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_GetAdjusterGroups"
            cmd.Parameters.AddWithValue("@AdjusterID", FNOLClaimAssignAdjuster_ID)
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count > 0 Then
                Return tbl
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetAdjusterGroupsByFNOLClaimAssignAdjusterID", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetCountRecordsByFNOLClaimAssignAdjusterID(ByVal FNOLClaimAssignAdjuster_ID As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM Counts WHERE FNOLClaimAssignAdjuster_Id = " & FNOLClaimAssignAdjuster_ID
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count > 0 Then
                Return tbl
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetCountRecordsByFNOLClaimAssignAdjusterID", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a list of users assigned to a group
    ''' </summary>
    ''' <param name="GroupID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdjusterGroupsByGroup(ByVal GroupID As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_GetAdjusterGroups"
            cmd.Parameters.AddWithValue("@GroupID", GroupID)
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count > 0 Then
                Return tbl
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetAdjusterGroupsByGroup", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetAllActiveAdjustersInGroup(ByVal GroupId As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_GetAdjustersInGroup"
            cmd.Parameters.AddWithValue("@GroupId", GroupId)
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count > 0 Then
                Return tbl
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetAllActiveAdjustersInGroup", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pass inthe group name, get the id back
    ''' </summary>
    ''' <param name="GroupName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetGroupID(ByVal GroupName As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As String = ""

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection not opened")

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT Groups_Id FROM Groups WHERE GROUPNAME = '" & GroupName & "'"
            rtn = cmd.ExecuteScalar()

            Return rtn
        Catch ex As Exception
            HandleError(ClassName, "GetGroupID", ex)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pass in a user name get the user id back
    ''' </summary>
    ''' <param name="AdjusterName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLClaimAssignAdjuster_ID_ByName(ByVal AdjusterName As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As String = ""

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection not open")

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLClaimAssignAdjuster_ID FROM FNOLClaimAssign_Adjusters WHERE Display_Name = '" & AdjusterName & "' ORDER BY FNOLClaimAssignAdjuster_Id DESC"
            rtn = cmd.ExecuteScalar()

            Return rtn
        Catch ex As Exception
            HandleError(ClassName, "GetFNOLClaimAssignAdjuster_ID_ByName", ex)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pass in a user name get the user id back
    ''' If there are more than 1 records with the passed id, returns the most recent one (with the highest id)
    ''' </summary>
    ''' <param name="ClaimPersonnel_Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLClaimAssignAdjuster_ID_ByClaimPersonnel_Id(ByVal ClaimPersonnel_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As String = ""

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection not open")

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLClaimAssignAdjuster_Id FROM FNOLClaimAssign_Adjusters WHERE claimpersonnel_id = " & ClaimPersonnel_Id & " ORDER BY FNOLClaimAssignAdjuster_Id DESC"
            rtn = cmd.ExecuteScalar()

            Return rtn
        Catch ex As Exception
            HandleError(ClassName, "GetFNOLClaimAssignAdjuster_ID_ByClaimPersonnel_Id", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function


    ''' <summary>
    ''' Returns true if the logged in user is in the config file CSUAuthorizedUsers key, false if not
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UserCanChangeValues(ByRef pg As Page) As Boolean
        Dim ValidUsers() As String = Nothing
        Dim UName As String = Nothing

        Try
            ' Make sure the key exists
            If AppSettings("CACAuthorizedUsers") Is Nothing Then
                Throw New Exception("CACAuthorizedUsers config key Is missing!")
                Return False
            End If

            ' Make sure we can get the windows user
            If pg.User.Identity.Name Is Nothing OrElse pg.User.Identity.Name = "" Then Throw New Exception("Windows user name Not found!!")
            UName = pg.User.Identity.Name.ToUpper().Trim()
            ' Drop the domain from the username
            Dim n() As String = UName.Split("\")
            If n.Length < 2 Then
                UName = UName
            Else
                UName = n(1)
            End If

            ' Split and check the authorized user list
            ValidUsers = AppSettings("CACAuthorizedUsers").Split("|")
            If ValidUsers Is Nothing OrElse ValidUsers.GetLength(0) = 0 Then Throw New Exception("Authorized Users list Is empty!!")

            ' Check the logged in user against the user list
            For Each s As String In ValidUsers
                If s.ToUpper() = UName.ToUpper() Then Return True
            Next

            Return False
        Catch ex As Exception
            HandleError(ClassName, "UserCanChangeValues", ex)
            Return False
        End Try
    End Function

    Public Function UserIsAuthorizedForCAC(ByRef pg As Page) As Boolean
        Dim UName As String = Nothing
        Dim err As String = Nothing

        Try
            ' Get the windows user
            If pg.User.Identity.Name Is Nothing OrElse pg.User.Identity.Name = "" Then Throw New Exception("Windows user name Not found!!")
            UName = pg.User.Identity.Name.ToUpper().Trim()
            ' Drop the domain from the username
            Dim n() As String = UName.Split("\")
            If n.Length < 2 Then
                UName = UName
            Else
                UName = n(1)
            End If

            ' All active claims managers should have access
            Dim tblMgrs As DataTable = GetManagerList(ShowValue_enum.ShownOnly, err)
            If tblMgrs Is Nothing OrElse tblMgrs.Rows.Count <= 0 Then Throw New Exception("No Claims Managers returned!")
            For Each dr As DataRow In tblMgrs.Rows
                If UName.ToUpper = dr("IFM_AD_UserName").ToString.Trim.ToUpper Then Return True
            Next

            ' Admin users should also have access if they have the 'Site Admin' flag
            Dim tblUsers As DataTable = GetUserAccessList()
            If tblUsers IsNot Nothing AndAlso tblUsers.Rows.Count > 0 Then
                For Each dr As DataRow In tblUsers.Rows
                    If UName.ToUpper = dr("userid").ToString.ToUpper Then
                        If CBool(dr("SiteAdmin")) Then Return True
                    End If
                Next
            End If

            Return False
        Catch ex As Exception
            HandleError(ClassName, "UserIsAuthorizedForCAC", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Gets a count of all groups in the CSU_GROUPS table
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetGroupCount() As Integer
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = 0

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection Not opened")

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "Select COUNT(GroupName) FROM Groups"
            rtn = cmd.ExecuteScalar()

            Return rtn
        Catch ex As Exception
            HandleError(ClassName, "GetGroupCount", ex)
            Return 0
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pass in a Adjuster id get the Adjuster name back
    ''' </summary>
    ''' <param name="AdjusterID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdjusterName(ByVal AdjusterID As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As String = ""

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection Not opened")

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "Select Display_Name FROM FNOLClaimAssign_Adjusters WHERE FNOLClaimAssignAdjuster_ID = " & AdjusterID
            rtn = cmd.ExecuteScalar()

            Return rtn
        Catch ex As Exception
            HandleError(ClassName, "GetUserName", ex)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' If a user ID is passed, returns the current week total for the passed user only.
    ''' If no user ID is passed, returns the current week totals for all users
    ''' </summary>
    ''' <param name="AdjusterID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdjusterWeeklyGrandTotals(Optional ByVal AdjusterID As String = Nothing) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim AdjTable As New DataTable()
        Dim CountTable As New DataTable()
        Dim stdt As String = GetCurrentWeekStartDate()
        Dim eddt As String = GetCurrentWeekEndDate()
        Dim err As String = Nothing

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was Not opened")

            ' Initialize the counts table with zeroes for all adjuster counts
            CountTable.Columns.Add("CountTotal")
            CountTable.Columns.Add("FNOLClaimAssignAdjuster_Id")
            CountTable.Columns.Add("Display_Name")
            AdjTable = GetAdjusterList(err)
            If AdjTable Is Nothing OrElse AdjTable.Rows.Count <= 0 Then Throw New Exception("No Adjusters Found!")

            ' Get the counts for each adjuster
            For Each dr As DataRow In AdjTable.Rows
                Dim newrow As DataRow = CountTable.NewRow
                newrow("CountTotal") = "0"
                newrow("FNOLClaimAssignAdjuster_Id") = dr("FNOLClaimAssignAdjuster_Id").ToString
                newrow("Display_Name") = dr("Display_Name").ToString

                cmd = New SqlCommand()
                cmd.Connection = conn
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandText = "usp_GetCounts"
                cmd.Parameters.AddWithValue("@StartDate", stdt)
                cmd.Parameters.AddWithValue("@EndDate", eddt)
                cmd.Parameters.AddWithValue("@AdjusterID", dr("FNOLClaimAssignAdjuster_ID").ToString)
                da = New SqlDataAdapter()
                da.SelectCommand = cmd
                tbl = New DataTable
                da.Fill(tbl)

                ' Get the count record for the user
                Dim val As Object = tbl.Compute("SUM(CountTotal)", "")
                If Not IsDBNull(val) Then
                    newrow("CountTotal") = val.ToString()
                Else
                    newrow("CountTotal") = "0"
                End If

                CountTable.Rows.Add(newrow)
            Next

            Return CountTable
        Catch ex As Exception
            HandleError(ClassName, "GetAdjustersWeeklyGrandTotals", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates the adjuster claim counts when a claim is assigned
    ''' - Increments the claim count for the adjuster whom the claim is being assigned to.
    ''' - If there was a previous adjuster assigned, decrements their count.
    ''' </summary>
    ''' <returns></returns>
    Public Function UpdateAdjusterClaimCountsOnClaimAssignment(ByVal DiamondClaimNumber As String, ByVal FNOLClaimAssignAdjuster_Id As String, ByVal GroupId As String, ByRef StepName As String, ByRef ProcessMessages As List(Of String), ByRef ErrorMessages As List(Of String), ByRef WarningMessages As List(Of String), lstBox As ListBox) As Boolean
        Dim FNOLRec As DataRow = Nothing
        Dim err As String = Nothing
        Dim PrevAdjId As String = Nothing
        Dim PrevGroupId As String = Nothing
        Dim PrevCountDate As String = Nothing
        Dim Ok As Boolean = False
        Dim adjdate As String = FormatDate(DateTime.Today)

        Try
            SetStep("Update Adjuster Claim Counts...", StepName, ProcessMessages, lstBox)

            ' Get the FNOL record
            FNOLRec = GetFNOLRecordByClaimNumber(DiamondClaimNumber, err)
            If FNOLRec Is Nothing Then
                SetStep("No FNOL Record found For Diamond Claim Number '" & DiamondClaimNumber & "'.  Unable to update Adjuster Counts.  Please check the counts and update manually if necessary.", StepName, WarningMessages, lstBox)
                Throw New Exception("No FNOL Record found for Diamond Claim Number '" & DiamondClaimNumber & "'")
            End If

            ' Check for previous adjuster assignment.  If there's an adjuster Id in the record then it's been assigned before.
            If Not IsDBNull(FNOLRec("FNOLClaimAssignAdjuster_Id")) Then
                ' Found previous adjuster
                PrevAdjId = FNOLRec("FNOLClaimAssignAdjuster_Id").ToString()
                PrevCountDate = FormatDate(FNOLRec("DateAssignedToAdjuster").ToString())
                ' Get previous assignment group id
                If (Not IsDBNull(FNOLRec("Groups_Id"))) AndAlso (IsNumeric(FNOLRec("Groups_Id"))) Then
                    PrevGroupId = FNOLRec("Groups_Id").ToString
                Else
                    ' No group id on record so we can't adjust the previous assignment count!
                    SetStep("An adjuster Id was found but no group record id was found.  Unable to adjust previous assignment counts.  Please check the counts and update manually if necessary.", StepName, WarningMessages, lstBox)
                End If
            End If

            ' Increment the current assignment
            ' Don't update the counts if the user, group and date are the same as the previous assignment.
            ' Why would we want to add 1 then subtract 1??
            If (PrevAdjId <> FNOLClaimAssignAdjuster_Id Or PrevGroupId <> GroupId) Then
                Dim AdjusterName As String = GetAdjusterName(FNOLClaimAssignAdjuster_Id)
                Dim GroupName As String = GetGroupName(GroupId)

                Ok = UpdateAdjusterCount(FNOLClaimAssignAdjuster_Id, GroupId, FormatDate(DateTime.Today), AdjusterCountUpdateIndicator.Increase)

                If Ok Then
                    SetStep("Added 1 to claim counts for Adjuster '" & AdjusterName & "', Group '" & GroupName & "'", StepName, ProcessMessages, lstBox)
                    If PrevAdjId IsNot Nothing AndAlso PrevGroupId IsNot Nothing Then
                        Dim PrevAdjusterName As String = GetAdjusterName(PrevAdjId)
                        Dim PrevGroupName As String = GetGroupName(PrevGroupId)
                        ' We found previous adjuster and previous group.  Decrement the previous assignment
                        Ok = UpdateAdjusterCount(PrevAdjId, PrevGroupId, PrevCountDate, AdjusterCountUpdateIndicator.Decrease)
                        If Ok Then
                            SetStep("Subtracted 1 from claim counts for previous Adjuster '" & PrevAdjusterName & "', Group '" & PrevGroupName & "'", StepName, ProcessMessages, lstBox)
                        Else
                            SetStep("There was an error decrementing the previous Adjuster count for '" & PrevAdjusterName & "', Group '" & PrevGroupName & "'.  Please check the count and update manually if necessary.", StepName, WarningMessages, lstBox)
                        End If
                    End If
                Else
                    SetStep("There was an error Incrementing the Adjuster claim count for '" & AdjusterName & "', Group '" & GroupName & "'.  Please check the count and update manually if necessary.", StepName, WarningMessages, lstBox)
                End If
            Else
                SetStep("The Adjuster claim counts were not updated because the previous assignment is the same as the current assignment", StepName, ProcessMessages, lstBox)
            End If

            Return True
        Catch ex As Exception
            HandleError(ClassName, "UpdateAdjusterClaimCountsOnClaimAssignment", ex)
            Return False
        End Try
    End Function

#End Region

    Public Function AdjusterIsActive(ByVal AdjusterName As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As New Object()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Unable to open connection")
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT Active FROM FNOLClaimAssign_Adjusters WHERE FNOLClaimAssign_Adjusters = '" & AdjusterName & "'"
            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing Then
                Return CBool(rtn)
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError(ClassName, "AdjusterIsActive", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Public Function AdjusterIsAvailable(ByVal AdjusterName As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As New Object()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Unable to open connection")
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT Available FROM FNOLClaimAssign_Adjusters WHERE FNOLClaimAssign_Adjusters = '" & AdjusterName & "'"
            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing Then
                Return CBool(rtn)
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError(ClassName, "AdjusterAvailable", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Uses a Diamond Service call to get a list of Diamond Adjuster objects.
    ''' If you pass in an optional Adjuster Id it will filter out all results excpept for that one.
    ''' </summary>
    ''' <param name="DiamondSecurityToken"></param>
    ''' <param name="errstr"></param>
    ''' <param name="ClaimPersonnelID"></param>
    ''' <returns></returns>
    Public Function GetDiamondAdjustersViaDiamond(ByVal DiamondSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByRef errstr As String, Optional ClaimPersonnelID As String = Nothing, Optional ByVal EnabledOnly As Boolean = False) As List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
        Dim request As New Diamond.Common.Services.Messages.ClaimsService.LoadClaimPersonnelDisplayList.Request()
        Dim response As New Diamond.Common.Services.Messages.ClaimsService.LoadClaimPersonnelDisplayList.Response()
        Dim err As String = Nothing
        Dim AdjusterList As New List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
        Dim ClaimPersonnel_List As New List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)

        Try
            ' Set up the request object
            request.DiamondSecurityToken = DiamondSecurityToken

            ' GET THE CLAIMPERSONNEL LIST FROM DIAMOND
            Using proxy As New Diamond.Common.Services.Proxies.ClaimsServiceProxy
                response = proxy.LoadClaimPersonnelDisplayList(request)
            End Using

            If response.ResponseData.ClaimPersonnelDisplayList IsNot Nothing AndAlso response.ResponseData.ClaimPersonnelDisplayList.Count > 0 Then
                ' Load up a list of the returned claimpersonnel objects
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In response.ResponseData.ClaimPersonnelDisplayList
                    ClaimPersonnel_List.Add(adj)
                Next

                ' If an Adjuster ID was passed, return only that adjuster record
                ' This overrides the ENABLED parameter, if adjuster id is passed it will always return that 
                ' adjuster regardless of the enabled property.
                If ClaimPersonnelID IsNot Nothing AndAlso IsNumeric(ClaimPersonnelID) Then
Filter0:
                    For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                        If adj.ClaimPersonnelId.ToString <> ClaimPersonnelID Then
                            ClaimPersonnel_List.Remove(adj)
                            GoTo Filter0
                        End If
                    Next
                    Return ClaimPersonnel_List
                End If

Filter1:
                ' Remove 'Agency Draft', 'FNOL User', and 'Optimal Claims' records
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                    If adj.DisplayName.ToString.ToUpper.Trim = "FNOL USER" _
                        OrElse adj.DisplayName.ToString.ToUpper.Trim = "AGENCY DRAFT" _
                        OrElse adj.DisplayName.ToString.ToUpper.Trim = "OPTIMAL CLAIMS" Then
                        ClaimPersonnel_List.Remove(adj)
                        GoTo Filter1
                    End If
                Next

Filter2:
                ' Select adjusters only
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                    If adj.ClaimPersonnelTypeDescription.ToUpper.Trim <> "ADJUSTER" Then
                        ClaimPersonnel_List.Remove(adj)
                        GoTo Filter2
                    End If
                Next

Filter3:
                ' Select Inside/Outside adjusters only
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                    If adj.ClaimAdjusterTypeDescription.ToString.ToUpper.Trim <> "INSIDE" AndAlso adj.ClaimAdjusterTypeDescription.ToString.ToUpper.Trim <> "OUTSIDE" Then
                        ClaimPersonnel_List.Remove(adj)
                        GoTo Filter3
                    End If
                Next


                ' REMOVE DUPLICATES
                ' - If one record is marked enabled use that one.
                ' - If no records or more than one record is marked enabled use
                '    - Enabled record with the highest claimpersonnel_id value (when more than one enabled record)
                '    - Record with the highest claimpersonnel_id value (when no enabled records)
                Dim nm As String = ""
                Dim id As String = ""
                Dim ct As Integer = 1

                ' Sort by Name ASC and ID DESC
                ClaimPersonnel_List.Sort(New ClaimPersonnelComparer_ByNameAndID())

                ' Get all records with the same adjuster name
                Dim adjname As String = ""
                Dim idsToRemove As New List(Of String)
                Dim SkipToNextName As Boolean = False
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                    Dim dupes As New List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
                    Dim enabledID As String = Nothing
                    Dim cnt As Integer = 0

                    If adjname <> adj.DisplayName Then
                        adjname = adj.DisplayName
                    Else
                        If SkipToNextName Then GoTo NextName
                    End If

                    dupes = GetAdjusterRecsByNameFromAdjusterList(adjname, ClaimPersonnel_List)
                    If dupes IsNot Nothing AndAlso dupes.Count > 1 Then
                        ' We have an adjuster with more than one record.  
                        For Each dupe As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In dupes
                            If dupe.Enabled Then
                                ' Record is enabled.  Store the id.  
                                cnt += 1  ' count of enabled
                                If cnt = 1 Then
                                    ' The first enabled record will have the highest id
                                    ' Only save the first
                                    enabledID = dupe.ClaimPersonnelId.ToString
                                    SkipToNextName = True
                                Else
                                    ' Remove all enabled records but the first one
                                    idsToRemove.Add(dupe.ClaimPersonnelId.ToString)
                                End If
                            Else
                                ' Remove any records that are not enabled
                                idsToRemove.Add(dupe.ClaimPersonnelId.ToString)
                            End If
                        Next

                    End If
NextName:
                Next

                ' Now that we have the list of id's to remove, remove them from the table
                If idsToRemove.Count > 0 Then
                    For Each recid As String In idsToRemove
                        For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                            If adj.ClaimPersonnelId.ToString = recid Then
                                ClaimPersonnel_List.Remove(adj)
                                Exit For
                            End If
                        Next
                    Next
                End If

Filter4:
                ' Remove all dupes but row with the highest id
                nm = ""
                id = ""
                ct = 0
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                    If adj.DisplayName.ToUpper <> nm Then
                        nm = adj.DisplayName.ToUpper
                        id = adj.ClaimPersonnelId.ToString
                        ct = 0
                    End If
                    ct += 1
                    If adj.DisplayName.ToUpper = nm AndAlso ct > 1 Then
                        ClaimPersonnel_List.Remove(adj)
                        GoTo Filter4
                    End If
                Next
            End If

Filter5:
            ' If the EnabledOnly flag was set, remove all but enabled
            If EnabledOnly Then
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In ClaimPersonnel_List
                    If Not adj.Enabled Then
                        ClaimPersonnel_List.Remove(adj)
                        GoTo Filter5
                    End If
                Next
            End If

            Return ClaimPersonnel_List
        Catch ex As Exception
            errstr = "GetDiamondAdjustersViaDiamond: " & ex.Message
            HandleError(ClassName, "GetDiamondAdjustersViaDiamond", ex)
            Return Nothing
        End Try
    End Function

    Private Function GetAdjusterRecsByNameFromAdjusterList(ByVal AdjName As String, ByVal AdjList As List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)) As List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
        Dim NewAdjList As List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel) = Nothing

        If AdjList Is Nothing OrElse AdjList.Count <= 0 Then Return Nothing

        For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In AdjList
            If adj.DisplayName.ToUpper = AdjName.ToUpper Then
                If NewAdjList Is Nothing Then NewAdjList = New List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
                NewAdjList.Add(adj)
            End If
        Next

        Return NewAdjList
    End Function


    Class ClaimPersonnelComparer_ByNameAndID
        Implements IComparer(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
        Public Function Compare(p1 As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel, p2 As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel) As Integer Implements System.Collections.Generic.IComparer(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel).Compare
            If p1.DisplayName = p2.DisplayName AndAlso p1.ClaimPersonnelId = p2.ClaimPersonnelId Then
                Return 0
            ElseIf p1.DisplayName <> p2.DisplayName Then
                Return p1.DisplayName.CompareTo(p2.DisplayName)
            ElseIf p1.ClaimPersonnelId <> p2.ClaimPersonnelId Then
                Return p1.ClaimPersonnelId.CompareTo(p2.ClaimPersonnelId)
            End If
        End Function
    End Class

    ''' <summary>
    ''' Sets the Adjuster's Diamond Out Of Office flag and then re-syncs the adjuster data with Diamond.
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="FNOLAdjusterID"></param>
    ''' <param name="OutOfOffice"></param>
    ''' <param name="errstr"></param>
    ''' <returns></returns>
    Public Function SetAdjusterOutOfOfficeValue(ByRef pg As Page, ByVal FNOLAdjusterID As String, ByVal OutOfOffice As Boolean, ByRef errstr As String, Optional ByVal ForceSyncWithDiamond As Boolean = False) As Boolean
        Try
            Dim show As Boolean = False
            Dim cpid As String = Nothing
            Dim err As String = Nothing

            cpid = GetAdjusterClaimpersonnel_ID_ById(FNOLAdjusterID, err)
            If Not OutOfOffice Then show = True

            If cpid IsNot Nothing Then
                If Not SetDiamondOutOfOfficeFlag(pg, cpid, OutOfOffice, err) Then
                    ' SetDiamondOutOfOfficeFlag failed
                    If err IsNot Nothing Then
                        Throw New Exception("Error setting Diamond Out-Of-Office flag: " & err)
                    Else
                        Throw New Exception("Error setting Diamond Out-Of-Office flag.")
                    End If
                End If
            Else
                ' cpid is nothing   
                If err IsNot Nothing Then
                    Throw New Exception("Error getting ClaimPersonnel_Id: " & err)
                Else
                    Throw New Exception("Error getting ClaimPersonnel_Id.")
                End If
            End If

            ' Only do a full sync with Diamond if the flag was set.  Full sync takes time.
            If ForceSyncWithDiamond Then
                If Not SynchronizeAdjusterTableWithDiamond(pg, err) Then
                    If err IsNot Nothing Then
                        Throw New Exception("Error syncing FNOL adjuster table with Diamond: " & err)
                    Else
                        Throw New Exception("Error syncing FNOL adjuster table with Diamond.")
                    End If
                End If
            End If

            Return True
        Catch ex As Exception
            HandleError(ClassName, "SetAdjusterOutOfOfficeValue", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' This function will return the current out of office flag value in Diamond for the passed adjuster.
    ''' The flag value is returned as the return value of the function.
    ''' </summary>
    ''' <param name="ClaimPersonnel_Id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function GetDiamondOutOfOfficeFlagValueForAdjuster(ByVal ClaimPersonnel_Id As String, ByRef err As String, Optional ByVal lblMsg As Label = Nothing) As Boolean
        Dim DiamondSecurityToken As Diamond.Common.Services.DiamondSecurityToken = Nothing
        Dim adjObj As List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel) = Nothing

        Try
            DiamondSecurityToken = GetDiamondSecurityToken(err)
            If DiamondSecurityToken Is Nothing Then
                If err IsNot Nothing Then
                    Throw New Exception("Error getting Diamond Security Token: " & err)
                Else
                    Throw New Exception("Error getting Diamond Security Token.")
                End If
            End If

            adjObj = GetDiamondAdjustersViaDiamond(DiamondSecurityToken, err, ClaimPersonnel_Id)
            If adjObj Is Nothing OrElse adjObj.Count > 1 Then
                If adjObj Is Nothing Then Throw New Exception("Adjuster Not Found!")
                If adjObj.Count > 1 Then Throw New Exception("More than one adjuster returned, expected exactly 1!")
            End If

            Return adjObj(0).OutOfOffice
        Catch ex As Exception
            err = "GetDiamondOutOfOfficeFlagValueForAdjuster: " & ex.Message
            HandleError(ClassName, "GetDiamondOutOfOfficeFlagValueForAdjuster", ex, lblMsg)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Makes a call to Diamond to set the out of office flag
    ''' </summary>
    ''' <param name="ClaimPersonnel_id"></param>
    ''' <param name="flagvalue"></param>
    ''' <returns></returns>
    Private Function SetDiamondOutOfOfficeFlag(ByRef pg As Page, ByVal ClaimPersonnel_id As String, flagvalue As Boolean, ByRef errstr As String) As Boolean
        Dim request As New Diamond.Common.Services.Messages.ClaimsService.GetClaimPersonnel.Request()
        Dim response As New Diamond.Common.Services.Messages.ClaimsService.GetClaimPersonnel.Response()
        Dim ClaimOfficeId As String = Nothing
        Dim ClaimPersonnelTypeId As String = Nothing
        Dim ClaimAdjusterTypeId As String = Nothing
        Dim err As String = Nothing
        Dim DiamondSecurityToken As Diamond.Common.Services.DiamondSecurityToken = Nothing
        Dim AdjusterList As List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel) = Nothing

        Try
            ' *** CALL GetClaimPersonnel TO GET THE EXISTING CLAIMPERSONNEL RECORD ***
            ' Get the data we need for the call
            If Not GetDiamondClaimPersonnelInfo(ClaimPersonnel_id, ClaimOfficeId, ClaimPersonnelTypeId, ClaimAdjusterTypeId) Then
                Throw New Exception("Error retrieving claim personnel info!")
            End If

            If Not DiamondLogin(pg, err) Then Throw New Exception("Error logging into Diamond!")
            DiamondSecurityToken = GetDiamondSecurityToken(err)
            If DiamondSecurityToken Is Nothing OrElse err IsNot Nothing Then Throw New Exception("Error getting Diamond Security Token!")

            ' Get the adjuster list from Diamond
            err = Nothing
            ' Returns only the adjuster record we want
            AdjusterList = GetDiamondAdjustersViaDiamond(DiamondSecurityToken, err, ClaimPersonnel_id)
            If AdjusterList Is Nothing OrElse AdjusterList.Count <= 0 Then
                If err IsNot Nothing Then
                    Throw New Exception("There was an error getting the adjuster list from Diamond: " & err)
                Else
                    Throw New Exception("There was an error getting the adjuster list from Diamond.")
                End If
            End If

            ' *** CALL SaveClaimPersonnel TO SAVE THE OUT OF OFFICE VALUE USING THE CLAIMPERSONNEL OBJECT WE JUST RETRIEVED ***
            If AdjusterList IsNot Nothing AndAlso AdjusterList.Count > 0 Then
                ' Loop through the returned records and find the adjuster record we want
                Dim adjinfo As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel = Nothing
                adjinfo = AdjusterList(0)

                ' Set the out of office flag
                adjinfo.OutOfOffice = flagvalue

                ' Call the save service
                Using proxy As New Diamond.Common.Services.Proxies.ClaimsServiceProxy
                    Dim saverequest As New Diamond.Common.Services.Messages.ClaimsService.SaveClaimPersonnel.Request()
                    Dim saveresponse As New Diamond.Common.Services.Messages.ClaimsService.SaveClaimPersonnel.Response()
                    saverequest.RequestData.ClaimPersonnel = adjinfo
                    saverequest.DiamondSecurityToken = DiamondSecurityToken
                    saverequest.RequestData.ByPassAutoAssignmentValidation = True   ' Added 3/16/21
                    saveresponse = proxy.SaveClaimPersonnel(saverequest)
                    If saveresponse.ResponseData.Success Then
                        ' SAVE TO DIAMOND SUCCEEDED
                        ' Update the FNOL Adjuster 'Show' value
                        Dim FNOLId As String = GetFNOLAdjusterIDByClaimPersonnelId(ClaimPersonnel_id, err)
                        If FNOLId Is Nothing Then Throw New Exception("Error getting FNOL adjuster ID: " & err.ToString)
                        Dim showval As Boolean = False
                        If Not adjinfo.OutOfOffice Then showval = True
                        If Not UpdateFNOLAdjusterShowFlag(FNOLId, showval, err) Then
                            If err IsNot Nothing Then
                                Throw New Exception("Error updating the FNOL Show value: " & err)
                            Else
                                Throw New Exception("Error updating the FNOL Show value.")
                            End If
                        End If
                        Return True
                    Else
                        ' SAVE TO DIAMOND FAILED
                        If saveresponse.DiamondValidation.ValidationItems IsNot Nothing AndAlso saveresponse.DiamondValidation.ValidationItems.Count > 0 Then
                            ' return any validation items in the err string
                            errstr = "FNOLCommonLibrary.SetDiamondOutOfOfficeFlag:"
                            For Each vi As Diamond.Common.Objects.ValidationItem In saveresponse.DiamondValidation.ValidationItems
                                errstr += vi.Message & ";"
                            Next
                        End If
                        Return False
                    End If
                End Using
            Else
                ' Adjuster list came back as nothing or with no records
                If err IsNot Nothing Then
                    Throw New Exception("There was an error getting the adjuster list from Diamond: " & err)
                Else
                    Throw New Exception("There was an error getting the adjuster list from Diamond.")
                End If
            End If

            Return True
        Catch ex As Exception
            errstr = ex.Message
            HandleError(ClassName, "SetDiamondOutOfOfficeFlag", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Gets ClaimOfficeId, ClaimPersonnelTypeId, and ClaimAdjusterTypeId for the passed claimpersonnel_id
    ''' </summary>
    ''' <param name="ClaimPersonnel_id"></param>
    ''' <param name="ClaimOfficeID"></param>
    ''' <param name="ClaimPersonnelTypeId"></param>
    ''' <param name="ClaimAdjusterTypeId"></param>
    ''' <returns></returns>
    Private Function GetDiamondClaimPersonnelInfo(ByVal ClaimPersonnel_id As String, ByRef ClaimOfficeID As String, ByRef ClaimPersonnelTypeId As String, ByRef ClaimAdjusterTypeId As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = AppSettings("connDiamond")
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT claimpersonnel_id, claimoffice_id, claimpersonneltype_id, claimadjustertype_id FROM ClaimPersonnel WHERE claimpersonnel_id = " & ClaimPersonnel_id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                ClaimOfficeID = tbl.Rows(0)("claimoffice_id").ToString()
                ClaimPersonnelTypeId = tbl.Rows(0)("claimpersonneltype_id").ToString()
                ClaimAdjusterTypeId = tbl.Rows(0)("claimadjustertype_id").ToString()
            End If

            Return True
        Catch ex As Exception
            HandleError(ClassName, "GetDiamondClaimPersonnelInfo", ex)
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
                conn.Dispose()
            End If
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates a Adjuster's ACTIVE status with the passed value
    ''' </summary>
    ''' <param name="AdjusterID"></param>
    ''' <param name="NewActiveStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ChangeAdjusterActiveStatus(ByVal AdjusterID As String, ByVal NewActiveStatus As Boolean) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim cmdtext As String = ""
        Dim rtn As Integer = -1

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection not opened")

            If NewActiveStatus Then
                cmdtext = "UPDATE FNOLClaimAssign_Adjusters SET Active = 1 WHERE FNOLClaimAssignAdjuster_ID = " & AdjusterID
            Else
                cmdtext = "UPDATE FNOLClaimAssign_Adjusters SET Active = 0 WHERE FNOLClaimAssignAdjuster_ID = " & AdjusterID
            End If

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = cmdtext

            rtn = cmd.ExecuteNonQuery()
            If rtn > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError(ClassName, "ChangeAdjusterActiveStatus", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function


    ''' <summary>
    ''' Deletes all USERGROUP and GROUP records belonging to the passed group id
    ''' </summary>
    ''' <param name="GroupID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteGroup(ByVal GroupID As String) As Boolean
        Dim conn As New SqlConnection()
        Dim txn As SqlTransaction = Nothing
        Dim cmd As New SqlCommand()
        Dim rtn As New Object()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection not opened")

            ' Initiate the transaction
            txn = conn.BeginTransaction()

            ' First delete all UserGroups records that contain the passed groupid
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.Transaction = txn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "DELETE FROM AdjusterGroups WHERE Groups_Id = " & GroupID
            rtn = cmd.ExecuteNonQuery()

            ' Next, delete the group record itself
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.Transaction = txn
            cmd.CommandText = "DELETE FROM Groups WHERE Groups_Id = " & GroupID
            rtn = cmd.ExecuteNonQuery()

            txn.Commit()

            Return True
        Catch ex As Exception
            HandleError(ClassName, "DeleteGroup", ex)
            txn.Rollback()
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Inserts a new record into the CSU_Groups table
    ''' </summary>
    ''' <param name="GroupName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertNewGroup(ByVal GroupName As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As New Object()

        Try
            If Not OpenCACConnection(conn) Then Throw New Exception("Connection was not opened")

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "INSERT INTO Groups (GroupName) VALUES('" & GroupName & "')"
            rtn = cmd.ExecuteNonQuery()

            If IsNumeric(rtn) Then
                If CInt(rtn) = 1 Then Return True
            End If

            Return False
        Catch ex As Exception
            HandleError(ClassName, "InsertNewGroup", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Public Function IsValidEmailAddress(ByVal emailaddr As String) As Boolean
        Dim regex As Regex = New Regex("^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
        Try
            Dim isValid As Boolean = regex.IsMatch(emailaddr.Trim)
            Return isValid
        Catch ex As Exception
            HandleError(ClassName, "IsValidEmailAddress", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Sets Assigment process steps for processing log and the Claim Assignment Processing listbox.
    ''' Needs to be in common so common processes can access it.
    ''' </summary>
    ''' <param name="dscr"></param>
    ''' <param name="StepName"></param>
    ''' <param name="ProcessMessages"></param>
    ''' <remarks></remarks>
    Public Sub SetStep(ByVal dscr As String, ByRef StepName As String, ByRef ProcessMessages As List(Of String), ByRef lstBox As ListBox)
        Try
            If ProcessMessages Is Nothing Then ProcessMessages = New List(Of String)
            StepName = dscr
            ProcessMessages.Add(dscr)
            lstBox.Items.Add(dscr)

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "SetStep", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Checks to see if the logged in user has the passed permission.
    ''' Returns True if so, False if not
    ''' </summary>
    ''' <param name="Permission"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckUserAccess(ByRef pg As Page, ByVal Permission As PermissionName_enum, ByRef err As String, Optional ByRef errlbl As Label = Nothing) As Boolean
        'Return True
        Dim UARec As DataRow = Nothing
        Dim UserName As String = Nothing

        Try
            UserName = GetUsername(pg)
            If UserName Is Nothing Then Throw New Exception("There was an error getting the Windows User Name")

            UARec = GetUserAccessListRecordByUserId(pg, UserName, err, errlbl)

            If UARec Is Nothing Then Throw New Exception("UserAccessList record was not found for user '" & UserName & "'")

            Select Case Permission
                Case PermissionName_enum.AdjusterAdmin
                    Return CBool(UARec("AdjusterAdmin"))
                Case PermissionName_enum.AssignClaim
                    Return CBool(UARec("AssignClaim"))
                Case PermissionName_enum.AssignedClaimsAccess
                    Return CBool(UARec("AssignedClaimsAccess"))
                Case PermissionName_enum.ClaimsAdjuster
                    Return CBool(UARec("ClaimsAdjuster"))
                Case PermissionName_enum.ClaimsManager
                    Return CBool(UARec("ClaimsManager"))
                Case PermissionName_enum.SiteAdmin
                    Return CBool(UARec("SiteAdmin"))
                Case PermissionName_enum.UnassignedClaimsAccess
                    Return CBool(UARec("UnassignedClaimsAccess"))
                Case Else
                    Throw New Exception("Unknown Permission!")
            End Select
        Catch ex As Exception
            err = ex.Message
            If errlbl IsNot Nothing Then errlbl.Text = ex.Message
            HandleError(ClassName, "CheckUserAccess", ex, errlbl)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Returns the logged in user name for the passed page
    ''' Returns 'Unknown' if no username found, and '*' if an error occurred
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUsername(ByRef pg As Page) As String
        Dim UName As String = Nothing
        Try
            If pg.User.Identity.Name IsNot Nothing AndAlso pg.User.Identity.Name <> "" Then
                UName = pg.User.Identity.Name.ToUpper().Trim()
            Else
                Return "Unknown"
            End If

            ' Drop the domain from the username
            Dim n() As String = UName.Split("\")
            If n.Length < 2 Then
                If UName.Trim = "" Then Return "Unknown"
                UName = UName
            Else
                UName = n(1)
            End If
            Return UName

        Catch ex As Exception
            HandleError(ClassName, "GetUsername", ex)
            Return "*"
        End Try
    End Function

    ''' <summary>
    ''' Logs the session into Diamond as PrintServices
    ''' </summary>
    ''' <param name="errorlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DiamondLogin(ByRef pg As Page, ByRef err As String, Optional ByRef errorlabel As Label = Nothing) As Boolean
        Dim loginName As String = Nothing
        Dim pwd As String = Nothing

        Try
            ' Check and retrieve the Diamond login name and password config keys
            If AppSettings("FNOLCA_LoginUser") Is Nothing OrElse AppSettings("FNOLCA_LoginUser").Trim = String.Empty Then
                Throw New Exception("Required config key 'FNOLCA_LoginUser' is missing or empty!!")
            Else
                loginName = AppSettings("FNOLCA_LoginUser")
            End If
            If AppSettings("FNOLCA_LoginPwd") Is Nothing OrElse AppSettings("FNOLCA_LoginPwd").Trim = String.Empty Then
                Throw New Exception("Required config key 'FNOLCA_LoginPwd' is missing or empty!!")
            Else
                pwd = AppSettings("FNOLCA_LoginPwd")
            End If

            Using diaweb_security As New DiamondWebClass.DiamondSecurity(loginName, pwd)
                If Diamond.Web.BaseControls.SignedOnUserID <= 0 Then
                    If diaweb_security.Errors IsNot Nothing AndAlso diaweb_security.Errors.Count > 0 Then
                        Dim msg As String = ""
                        For Each er As DiamondWebClass.ErrorObject In diaweb_security.Errors
                            If er.FriendlyErrorMsg IsNot Nothing AndAlso er.FriendlyErrorMsg <> String.Empty Then
                                msg += er.FriendlyErrorMsg & "("
                                If er.TechnicalErrorMsg IsNot Nothing AndAlso er.TechnicalErrorMsg <> String.Empty Then
                                    msg += er.TechnicalErrorMsg
                                End If
                                msg += ");"
                            End If
                        Next
                        Throw New Exception(msg)
                    Else
                        Throw New Exception("Unknown Error logging in Diamond user")
                    End If
                Else
                    diaweb_security.Lookup_GetExistingUserInfo(DiamondWebClass.Enums.UserLocation.Diamond)
                    diaweb_security.Lookup_GetUserAgencies(diaweb_security.ExistingUser, DiamondWebClass.Enums.UserLocation.Diamond)
                    diaweb_security.fillLinkData(diaweb_security.ExistingUser)
                    'pg.Session("DiamondUsername") = diaweb_security.ExistingUser.diamond_LoginName
                    pg.Session("DiamondUsername") = loginName
                    pg.Session("DiamondPassword") = pwd
                End If
            End Using

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "DiamondLogin", ex, errorlabel)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Removes non-safe characters '/\[]{} from a string and returns the result
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RemoveSpecialCharactersFromString(ByVal str As String) As String
        Dim specialchars As String = "'/\[]{}"
        Try
            str = str.Replace("\n", " ")
            str = str.Replace("/n", " ")
            For y As Int16 = 0 To specialchars.Length - 1
                Dim c As String = specialchars.Substring(y)
                str = str.Replace(c, String.Empty)
            Next

            Return str
        Catch ex As Exception
            HandleError(ClassName, "RemoveSpecialCharactersFromString", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Converts claim detail phone numbers into a formatted string
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatClaimDetailPhones(ByVal dr As DataRow) As String
        Dim phn As String = ""
        Try
            If Not IsDBNull(dr("HomePhone")) Then phn += "Home: " & dr("HomePhone").ToString()
            If Not IsDBNull(dr("BusinessPhone")) Then
                If phn <> String.Empty Then phn += " "
                phn += "Bus: " & dr("BusinessPhone").ToString()
            End If
            If Not IsDBNull(dr("CellPhone")) Then
                If phn <> String.Empty Then phn += " "
                phn += "Cell: " & dr("CellPhone").ToString()
            End If
            If Not IsDBNull(dr("FAX")) Then
                If phn <> String.Empty Then phn += " "
                phn += "FAX: " & dr("FAX").ToString()
            End If
            Return phn
        Catch ex As Exception
            HandleError(ClassName, "FormatClaimDetailPhones", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Converts a claim detail name into a formatted name string
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatClaimDetailName(ByVal dr As DataRow) As String
        Dim nm As String = ""
        Try
            If Not IsDBNull(dr("FirstName")) Then nm += dr("FirstName").ToString()
            If Not IsDBNull(dr("MiddleName")) Then
                If nm = String.Empty Then
                    nm += dr("MiddleName").ToString()
                Else
                    nm += " " & dr("MiddleName").ToString()
                End If
            End If
            If Not IsDBNull(dr("LastName")) Then
                If nm = String.Empty Then
                    nm += dr("LastName").ToString()
                Else
                    nm += " " & dr("LastName").ToString()
                End If
            End If
            Return nm
        Catch ex As Exception
            HandleError(ClassName, "FormatClaimDetailName", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Converts claim detail address into a formatted address string
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatClaimDetailAddress(ByVal dr As DataRow) As String
        Dim adr As String = ""
        Dim st As String = ""
        Dim err As String = Nothing

        Try
            If Not IsDBNull(dr("Address")) Then adr += dr("Address").ToString()
            If Not IsDBNull(dr("City")) Then
                If adr <> String.Empty Then adr += " "
                adr += dr("City").ToString()
            End If
            If Not IsDBNull(dr("State")) Then
                If adr <> String.Empty Then adr += ", "
                If IsNumeric(dr("state")) Then
                    st = LookupDiamondStateName(dr("state").ToString(), err)
                    If st = "" AndAlso err <> Nothing AndAlso err <> String.Empty Then Throw New Exception(err)
                    adr += st
                Else
                    adr += dr("State").ToString()
                End If
            End If
            If Not IsDBNull(dr("Zip")) Then
                If adr <> String.Empty Then adr += ", "
                adr += dr("Zip").ToString()
            End If
            Return adr
        Catch ex As Exception
            HandleError(ClassName, "FormatClaimDetailAddress", ex)
            Return ""
        End Try
    End Function


    ''' <summary>
    ''' Sends an email using the commonclasses library
    ''' </summary>
    ''' <param name="InRec"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendEmail(ByRef InRec As EmailInfo_Structure_FNOLCA, ByRef err As String, Optional ByVal AttachmentFileList As List(Of String) = Nothing) As Boolean
        Dim attachments As New ArrayList
        Dim messAtt As System.Net.Mail.Attachment = Nothing
        Dim info As FileInfo = Nothing
        Dim systememail As String = Nothing
        Dim LossType As String = Nothing

        Try
            Using objMail As New EmailObject()
                ' Attach attachment(s) to email
                If AttachmentFileList IsNot Nothing Then
                    For Each fn As String In AttachmentFileList
                        If System.IO.File.Exists(fn) Then 'added IF to make sure file exists before trying to attach
                            messAtt = New System.Net.Mail.Attachment(fn)
                            attachments.Add(messAtt)
                        End If
                    Next
                End If
                If attachments IsNot Nothing AndAlso attachments.Count > 0 Then
                    objMail.EmailAttachments = attachments
                    attachments = Nothing
                End If
                messAtt = Nothing

                ' FROM address - used passed value or default
                If InRec.FromAddress_OPTIONAL IsNot Nothing AndAlso InRec.FromAddress_OPTIONAL <> "" Then
                    objMail.EmailFromAddress = InRec.FromAddress_OPTIONAL
                Else
                    objMail.EmailFromAddress = "LossReporting@IndianaFarmers.com"
                End If

                If InRec.ToAddress <> String.Empty Then
                    objMail.EmailToAddress = InRec.ToAddress
                Else
                    Throw New Exception("Missing TO email address")
                End If

                If InRec.CCAddress IsNot Nothing AndAlso InRec.CCAddress <> String.Empty Then
                    objMail.EmailCCAddress = InRec.CCAddress
                End If

                ' If there is a value in the subjectline field, use it, otherwise use the line shown below
                If InRec.SubjectLine_OPTIONAL IsNot Nothing AndAlso InRec.SubjectLine_OPTIONAL <> "" Then
                    objMail.EmailSubject = InRec.SubjectLine_OPTIONAL
                Else
                    objMail.EmailSubject = LossType & " for " & InRec.PolicyNumber & " - " & InRec.PolicyHolderFirstName & " " & InRec.PolicyHolderLastName
                End If

                ' Set the body of the email
                objMail.EmailBody = InRec.Body

                ' If there is a value in the mailhost field, use it, otherwise use the mail host set in the config file
                If InRec.MailHost_OPTIONAL IsNot Nothing AndAlso InRec.MailHost_OPTIONAL <> "" Then
                    objMail.MailHost = InRec.MailHost_OPTIONAL
                Else
                    objMail.MailHost = System.Configuration.ConfigurationManager.AppSettings("mailhost")
                End If

                objMail.SendEmail()

                If objMail.hasError Then Throw New Exception("Email transmission to " & objMail.EmailToAddress & " failed: " & objMail.errorMsg)

            End Using

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "SendMail", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Gets count of claims (Assigned, Unassigned, or all)
    ''' </summary>
    ''' <param name="AssignedUnassignedOrAll"></param>
    ''' <param name="LOB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetClaimCount(Optional AssignedUnassignedOrAll As String = "ALL", Optional ByVal LOB As LOB_enum = LOB_enum.None) As Integer
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Dim sql As String = Nothing
        Dim count As Integer = 0
        Dim FNOLTypeID As String = Nothing
        Dim err As String = Nothing

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()

            ' Assigned
            If AssignedUnassignedOrAll.ToUpper().Substring(0, 2) = "AS" Then
                cmd = New SqlCommand()
                cmd.Connection = conn
                cmd.CommandType = CommandType.Text
                sql = "SELECT COUNT(fnol_id) FROM tbl_FNOL WHERE Assigned = 1"
                If LOB <> LOB_enum.None Then
                    Select Case LOB
                        Case LOB_enum.Auto
                            ' Need auto and commercial auto
                            FNOLTypeID = GetFNOLTypeID("Auto")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " AND (FNOLType_Id = " & FNOLTypeID
                            FNOLTypeID = GetFNOLTypeID("Commercial Auto")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " OR FNOLType_Id = " & FNOLTypeID & ")"
                            Exit Select
                        Case LOB_enum.Liability
                            FNOLTypeID = GetFNOLTypeID("Liability")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " AND FNOLType_Id = " & FNOLTypeID
                            Exit Select
                        Case LOB_enum.Propperty
                            FNOLTypeID = GetFNOLTypeID("Property")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " AND FNOLType_Id = " & FNOLTypeID
                            Exit Select
                    End Select
                End If
                cmd.CommandText = sql
                rtn = cmd.ExecuteScalar()
                If rtn IsNot Nothing AndAlso IsNumeric(rtn) Then count += CInt(rtn)
            End If

            ' Unassigned
            If AssignedUnassignedOrAll.ToUpper().Substring(0, 2) = "UN" Then
                cmd = New SqlCommand()
                cmd.Connection = conn
                cmd.CommandType = CommandType.Text
                sql = "SELECT COUNT(fnol_id) FROM tbl_FNOL WHERE Assigned = 0"
                If LOB <> LOB_enum.None Then
                    Select Case LOB
                        Case LOB_enum.Auto
                            ' Need auto and commercial auto
                            FNOLTypeID = GetFNOLTypeID("Auto")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " AND (FNOLType_Id = " & FNOLTypeID
                            FNOLTypeID = GetFNOLTypeID("Commercial Auto")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " OR FNOLType_Id = " & FNOLTypeID & ")"
                            Exit Select
                        Case LOB_enum.Liability
                            FNOLTypeID = GetFNOLTypeID("Liability")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " AND FNOLType_Id = " & FNOLTypeID
                            Exit Select
                        Case LOB_enum.Propperty
                            FNOLTypeID = GetFNOLTypeID("Property")
                            If FNOLTypeID Is Nothing Then Throw New Exception("FNOL Type ID is nothing!")
                            sql += " AND FNOLType_Id = " & FNOLTypeID
                            Exit Select
                    End Select
                End If
                cmd.CommandText = sql
                rtn = cmd.ExecuteScalar()
                If rtn IsNot Nothing AndAlso IsNumeric(rtn) Then count += CInt(rtn)
            End If

            Return count
        Catch ex As Exception
            HandleError(ClassName, "GetClaimCount", ex)
            Return -1
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets claim loss type description by id
    ''' </summary>
    ''' <param name="ClaimLossType_Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDiamondClaimLossTypeDescription(ByVal ClaimLossType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnDiamond
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT [dscr] FROM ClaimLossType WHERE [ClaimLossType_Id] = " & ClaimLossType_Id
            rtn = cmd.ExecuteScalar()

            If rtn Is Nothing Then
                Return Nothing
            Else
                Return rtn.ToString()
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetDiamondClaimLossTypeDescription", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a table of all documents attached to an FNOL
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLDocuments(ByVal FNOL_Id As String, ByRef err As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_FNOLDocuments WHERE FNOL_Id = " & FNOL_Id & " ORDER BY FNOL_Documents_Id ASC"
            da.SelectCommand = cmd
            da.Fill(tbl)

            'If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("No Documents Found!")
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing

            Return tbl
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLDocuments", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a table of all the persons associated with a particular FNOL
    ''' If no persons for the FNOL, returns nothing
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLPersonsByType(ByVal FNOL_Id As String, ByVal PersonType As PersonType_Enum, ByRef err As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            ' Get the person type ID
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            Select Case PersonType
                Case PersonType_Enum.Injured
                    cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Injured'"
                    Exit Select
                Case PersonType_Enum.OtherVehicleDriver
                    cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Other Vehicle Driver'"
                    Exit Select
                Case PersonType_Enum.OtherVehicleOwner
                    cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Other Vehicle Owner'"
                    Exit Select
                Case PersonType_Enum.Witness
                    cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Witness'"
                    Exit Select
                Case Else
                    Throw New Exception("Invalid person type passed")
            End Select
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing OrElse Not IsNumeric(rtn) Then Throw New Exception("Error getting person type ID")

            ' Get the desired records
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_Persons WHERE FNOL_Id = " & FNOL_Id & " AND PersonType_Id = " & rtn.ToString() & " ORDER BY Person_Id ASC"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing

            Return tbl
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLPersonsByType", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Private Function GetVirtualAppraisal(ByVal ClaimNumber As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim ccc_consent As String = "0"

        Try

            conn.ConnectionString = AppSettings("connDiamond")
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "select CC.claimcontrol_id, CC.claim_number, V.year, V.vin, V.make, V.model, Q.* from ClaimLnVehicle as V with (nolock)
                               inner join ClaimControl as CC with (nolock) on CC.claimlnmaster_id = V.claimlnmaster_id and CC.claimlnimage_num = V.claimlnimage_num
                               inner join CCCEstimateQualification as Q with (nolock) on Q.cccestimatequalification_id = V.cccestimatequalification_id
                               where CC.claim_number = '" & ClaimNumber & "'"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count > 0 Then
                ccc_consent = tbl.Rows(0)("question1_yes_no_id").ToString()
            End If

        Catch ex As Exception
            HandleError(ClassName, "GetFNOLPersonsByType", ex)
            Return "0"
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try

        Return ccc_consent
    End Function


    ''' <summary>
    ''' Looks up state name in Diamond by id
    ''' </summary>
    ''' <param name="StateID"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LookupDiamondStateName(ByVal StateID As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnDiamond
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT [state] FROM [State] WHERE State_Id = " & StateID
            rtn = cmd.ExecuteScalar()

            If rtn Is Nothing Then Throw New Exception("State Not Found!")

            Return rtn.ToString()
        Catch ex As Exception
            HandleError(ClassName, "LookupDiamondStateName", ex)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets a specific FNOL Record by record Id
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLRecordById(ByVal id As String, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_FNOL WHERE FNOL_Id = " & id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found")

            Return tbl.Rows(0)
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLRecordById", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets a specific FNOL Record by record by claim number
    ''' </summary>
    ''' <param name="ClaimNumber"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLRecordByClaimNumber(ByVal ClaimNumber As String, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_FNOL WHERE DiamondClaimNumber = '" & ClaimNumber & "'"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found")

            Return tbl.Rows(0)
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLRecordByClaimNumber", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns FNOL Type description based on id
    ''' </summary>
    ''' <param name="FNOLType_Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLTypeName(ByVal FNOLType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLType from tbl_FNOLType WHERE FNOLType_Id = " & FNOLType_Id
            rtn = cmd.ExecuteScalar()

            If rtn Is Nothing Then
                Return Nothing
            Else
                Return rtn.ToString()
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetFNOLTypeName", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns FNOL Type Id by description
    ''' </summary>
    ''' <param name="FNOLTypeName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLTypeID(ByVal FNOLTypeName As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLType_Id from tbl_FNOLType WHERE FNOLType = '" & FNOLTypeName & "'"
            rtn = cmd.ExecuteScalar()

            If rtn Is Nothing Then
                Return Nothing
            Else
                Return rtn.ToString()
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetFNOLTypeID", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns Tracked Event Name based on id
    ''' </summary>
    ''' <param name="tbl_FNOL_CATCompany_Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLTrackedEventName(ByVal tbl_FNOL_CATCompany_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT CompanyName from tbl_FNOL_CATCompany WHERE tbl_FNOL_CATCompany_Id = " & tbl_FNOL_CATCompany_Id
            rtn = cmd.ExecuteScalar()

            If rtn Is Nothing Then
                Return Nothing
            Else
                Return rtn.ToString()
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetFNOLTrackedEventName", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns the ID of the N/A record in the internal Managers table
    ''' </summary>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetUnassignedID(ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "Select claimpersonnel_id FROM FNOLClaimAssign_Managers WHERE display_name = 'N/A'"
            rtn = cmd.ExecuteScalar()

            If rtn Is Nothing OrElse Not IsNumeric(rtn.ToString()) Then Return Nothing

            Return rtn.ToString()
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetUnassignedID", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns the settings record.  There's only supposed to be one settings record in existence, and 
    ''' this function will return the first record in the table regardless of how many records are there
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSettingsRecord(ByRef err As String, Optional errlabel As Label = Nothing) As DataRow
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim UserRec As DataRow = Nothing

        Try
            sql = "SELECT * FROM FNOL_ClaimAssign_Settings"
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl.Rows(0)
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetSettingsRecord", ex, errlabel)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates the AssignedClaimsRetentionDays setting in the FNOL_ClaimAssign_Settings table
    ''' </summary>
    ''' <param name="days"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateAssignedClaimsPurgeDays(ByVal days As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1

        Try
            If Not IsNumeric(days) Then Throw New Exception("Passed days value is not numneric!")

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOL_ClaimAssign_Settings SET AssignedClaimsRetentionDays = " & days
            rtn = cmd.ExecuteNonQuery()
            If rtn < 1 Then Throw New Exception("Unknown error updating settings record")
            Return True

        Catch ex As Exception
            HandleError(ClassName, "UpdateAssignedClaimsPurgeDays", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates the SendInsideAdjusterAssignmentEmail and SendOutsideAdjusterAssignmentEmail settings
    ''' in the FNOL_ClaimAssign_Settings table
    ''' </summary>
    ''' <param name="SendInside"></param>
    ''' <param name="SendOutside"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateSendAdjusterAssignmentEmailSettings(ByVal SendInside As Boolean, ByVal SendOutside As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim ia As String = Nothing
        Dim oa As String = Nothing

        Try
            If SendInside Then ia = "1" Else ia = "0"
            If SendOutside Then oa = "1" Else oa = "0"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOL_ClaimAssign_Settings SET SendInsideAdjusterAssignmentEmail = " & ia & ", SendOutsideAdjusterAssignmentEmail = " & oa
            rtn = cmd.ExecuteNonQuery()
            If rtn < 1 Then Throw New Exception("Unknown error updating settings record")
            Return True

        Catch ex As Exception
            HandleError(ClassName, "UpdateSendAdjusterAssignmentEmailSettings", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates the 'AcrosoftEnabled' setting in th FNOL_ClaimAssign_Settings table
    ''' </summary>
    ''' <param name="AcrosoftEnabled"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateAcrosoftEnabledSetting(ByVal AcrosoftEnabled As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim acro As String = Nothing

        Try
            If AcrosoftEnabled Then acro = "1" Else acro = "0"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOL_ClaimAssign_Settings SET AcrosoftEnabled = " & acro
            rtn = cmd.ExecuteNonQuery()
            If rtn < 1 Then Throw New Exception("Unknown error updating settings record")
            Return True

        Catch ex As Exception
            HandleError(ClassName, "UpdateAcrosoftEnabledSetting", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Reads the AcrosoftEnabled setting and returns true if Acrosoft is enabled, false if not
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AcrosoftIsEnabled(ByRef err As String, Optional errlabel As Label = Nothing) As Boolean
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim Rec As DataRow = Nothing

        Try
            ' Note that there should only be ONE settings record, and if there's more than one
            ' we're only going to look at the first one
            sql = "SELECT * FROM FNOL_ClaimAssign_Settings"
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Throw New Exception("No settings record found in FNOL_ClaimAssign_Settings!")
            End If

            Rec = tbl.Rows(0)
            Return CBool(Rec("AcrosoftEnabled"))
        Catch ex As Exception
            HandleError(ClassName, "AcrosoftIsEnabled", ex, errlabel)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Reads the settings file and returns true if Inside Adjusters are enabled, false if not
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsideAdjusterEmailsAreEnabled(ByRef err As String, Optional errlabel As Label = Nothing) As Boolean
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim Rec As DataRow = Nothing

        Try
            ' Note that there should only be ONE settings record, and if there's more than one
            ' we're only going to look at the first one
            sql = "SELECT * FROM FNOL_ClaimAssign_Settings"
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Throw New Exception("No settings record found in FNOL_ClaimAssign_Settings!")
            End If

            Rec = tbl.Rows(0)
            Return CBool(Rec("SendInsideAdjusterAssignmentEmail"))
        Catch ex As Exception
            HandleError(ClassName, "InsideAdjusterEmailsAreEnabled", ex, errlabel)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Reads the settings file and returns true if Outside Adjusters are enabled, false if not
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function OutsideAdjusterEmailsAreEnabled(ByRef err As String, Optional errlabel As Label = Nothing) As Boolean
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim Rec As DataRow = Nothing

        Try
            ' Note that there should only be ONE settings record, and if there's more than one
            ' we're only going to look at the first one
            sql = "SELECT * FROM FNOL_ClaimAssign_Settings"
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Throw New Exception("No settings record found in FNOL_ClaimAssign_Settings!")
            End If

            Rec = tbl.Rows(0)
            Return CBool(Rec("SendOutsideAdjusterAssignmentEmail"))
        Catch ex As Exception
            HandleError(ClassName, "OutsideAdjusterEmailsAreEnabled", ex, errlabel)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns the LastPurgeDateTime value from the settings table
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAssignedClaimsPurgeDays(ByRef err As String, Optional errlabel As Label = Nothing) As String
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim Rec As DataRow = Nothing

        Try
            ' Note that there should only be ONE settings record, and if there's more than one
            ' we're only going to look at the first one
            sql = "SELECT * FROM FNOL_ClaimAssign_Settings"
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Throw New Exception("No settings record found in FNOL_ClaimAssign_Settings!")
            End If

            Rec = tbl.Rows(0)
            If IsDBNull(Rec("AssignedClaimsRetentionDays")) Then
                Return "not set"
            Else
                Return Rec("AssignedClaimsRetentionDays").ToString
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetAssignedClaimsPurgeDays", ex, errlabel)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a user access list record by id
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="UserId"></param>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserAccessListRecordByUserId(ByRef pg As Page, ByVal UserId As String, ByRef err As String, Optional errlabel As Label = Nothing) As DataRow
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim UserRec As DataRow = Nothing

        Try

            sql = "SELECT * FROM FNOL_ClaimAssign_UserAccessList WHERE userid = '" & UserId & "'"
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl.Rows(0)
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetUserAccessList", ex, errlabel)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns the a contents of the FNOL_ClaimAssign_UserAccessList table
    ''' If an id is passed it will return that specific record
    ''' </summary>
    ''' <param name="MsgLabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserAccessList(Optional id As String = Nothing, Optional MsgLabel As Label = Nothing) As DataTable
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If id Is Nothing Then
                sql = "SELECT * FROM FNOL_ClaimAssign_UserAccessList ORDER BY DisplayName ASC"
            Else
                sql = "SELECT * FROM FNOL_ClaimAssign_UserAccessList WHERE id = " & id & " ORDER BY DisplayName ASC"
            End If

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetUserAccessList", ex, MsgLabel)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets a specific FNOL Assignment Log record
    ''' </summary>
    ''' <param name="FNOL_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLAssignmentLogRecordById(ByVal FNOL_id As String, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_FNOL_Assignment_Log WHERE FNOL_Id = " & FNOL_id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found")

            Return tbl.Rows(0)
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLAssignmentLogRecordById", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetFNOLAdjusterIDByClaimPersonnelId(ByVal cpid As String, ByVal err As String) As String
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLClaimAssignAdjuster_ID FROM FNOLClaimAssign_Adjusters WHERE ClaimPersonnel_ID = " & cpid
            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing AndAlso IsNumeric(rtn.ToString) Then
                Return rtn.ToString
            Else
                Return Nothing
            End If
        Catch ex As Exception
            err = "GetFNOLAdjutsterIDByClaimPersonnelId: " & ex.Message
            HandleError(ClassName, "GetFNOLAdjutsterIDByClaimPersonnelId", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets a table of adjusters from the Diamond claimpersonnel table
    ''' </summary>
    ''' <param name="MsgLabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOLCAAdjusterList(ByRef MsgLabel As Label, Optional ByRef IncludeInactive As Boolean = False) As DataTable
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim li As New ListItem()
        Dim txt As String = Nothing

        Try
            sql = "SELECT * FROM FNOLClaimAssign_Adjusters"
            If IncludeInactive Then
                sql += " WHERE Active = 1"
            End If
            sql += " ORDER BY Display_Name ASC"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetFNOLCAAdjusterList", ex, MsgLabel)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Private Function RemoveFNOLAdjusterRecord(ByVal FNOLClaimAssignAdjuster_Id As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "DELETE FROM FNOLClaimAssign_Adjusters WHERE FNOLClaimAssignAdjuster_Id = " & FNOLClaimAssignAdjuster_Id
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If rtn.ToString = "1" Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "RemoveFNOLAdjusterRecord", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function


    ''' <summary>
    ''' Returns a table of Managers from the internal managers table
    ''' </summary>
    ''' <param name="ShowValue"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetManagerList(ByVal ShowValue As ShowValue_enum, ByRef err As String) As DataTable
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim s As String = Nothing

        Try
            Select Case ShowValue
                Case ShowValue_enum.Both
                    sql = "Select * FROM FNOLClaimAssign_Managers ORDER BY display_name"
                    Exit Select
                Case ShowValue_enum.HiddenOnly
                    sql = "Select * FROM FNOLClaimAssign_Managers WHERE show = 0 ORDER BY display_name"
                    Exit Select
                Case ShowValue_enum.ShownOnly
                    sql = "Select * FROM FNOLClaimAssign_Managers WHERE show = 1 ORDER BY display_name"
                    Exit Select
            End Select

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                err = "No Managers Found"
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetManagerList", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Showvalue:  "Shown", "Hidden", "Both"
    ''' default is both
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="ShowValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdjusterList(ByRef err As String, Optional ByVal ActiveOnly As Boolean = True, Optional ByVal ShowValue As ShowValue_enum = ShowValue_enum.Both, Optional ByVal AssignedToMgrValue As AssignedToManager_enum = AssignedToManager_enum.All, Optional ByVal Manager_claimpersonnelID As String = Nothing, Optional ByVal ShowNAAdjusters As Boolean = True) As DataTable
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim s As String = Nothing
        Dim NAID As String = Nothing

        Try
            If ActiveOnly Then
                sql = "Select * FROM FNOLClaimAssign_Adjusters WHERE Active = 1"
            Else
                sql = "Select * FROM FNOLClaimAssign_Adjusters WHERE Active In (0,1)"
            End If

            Select Case ShowValue
                Case ShowValue_enum.Both
                    'sql = "Select * FROM FNOLClaimAssign_Adjusters WHERE"
                    Exit Select
                Case ShowValue_enum.ShownOnly
                    sql = " And show = 1"
                    Exit Select
                Case ShowValue_enum.HiddenOnly
                    sql = "And show = 0"
                    Exit Select
            End Select

            ' Get the N/A ID
            NAID = GetUnassignedID(err)
            If NAID Is Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception(err)
                Else
                    Throw New Exception("N/A ID was Not found!!")
                End If
            End If

            If Manager_claimpersonnelID IsNot Nothing AndAlso IsNumeric(Manager_claimpersonnelID) Then
                If Not sql.Contains("WHERE") Then sql += " WHERE" Else sql += " And"
                If ShowNAAdjusters Then
                    sql += " (manager_claimpersonnel_id = " & Manager_claimpersonnelID & " Or manager_claimpersonnel_id = " & NAID & ")"
                Else
                    sql += " manager_claimpersonnel_id = " & Manager_claimpersonnelID
                End If
            Else
                Select Case AssignedToMgrValue
                    Case AssignedToManager_enum.Assigned
                        If Not sql.Contains("WHERE") Then sql += " WHERE" Else sql += " And"
                        sql += " manager_claimpersonnel_id <> " & NAID
                        Exit Select
                    Case AssignedToManager_enum.Unassigned
                        If Not sql.Contains("WHERE") Then sql += " WHERE" Else sql += " And"
                        sql += " manager_claimpersonnel_id = " & NAID
                        Exit Select
                    Case AssignedToManager_enum.All
                        Exit Select
                End Select
            End If

            sql += " ORDER BY display_name ASC"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                err = "No Adjusters Found"
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetAdjusterList", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetTrackedEventProviderCountByDateRange(ByVal ProviderID As String, ByVal StartDate As DateTime, ByVal EndDate As DateTime) As String
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            sql = "Select COUNT(FNOL_Id) As CountTotal FROM tbl_FNOL F "
            sql += "JOIN tbl_FNOL_CATCompany CC On F.tbl_FNOL_CATCompany_Id = CC.tbl_FNOL_CATCompany_Id "
            sql += "WHERE F.tbl_FNOL_CATCompany_Id = " & ProviderID & " "
            sql += "And CONVERT(DATETIME, DateAssignedToAdjuster) BETWEEN '" & StartDate.ToShortDateString & "' AND '" & EndDate.ToShortDateString & "' "
            sql += "AND CC.CompanyName <> 'NONE'"

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            rtn = cmd.ExecuteScalar()

            If IsNumeric(rtn) Then Return rtn.ToString Else Return "0"

        Catch ex As Exception
            HandleError(ClassName, "GetTrackedEventProviderList", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a datatable of all tracked event providers records
    ''' </summary>
    ''' <param name="IncludeInactive"></param>
    ''' <param name="IncludeNoneRecord"></param>
    ''' <returns></returns>
    Public Function GetTrackedEventProviderList(Optional ByVal IncludeInactive As Boolean = False, Optional ByVal IncludeNoneRecord As Boolean = False) As DataTable
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim s As String = Nothing

        Try
            sql += "SELECT * FROM tbl_FNOL_CatCompany"
            If IncludeInactive And IncludeNoneRecord Then
                ' Include both inactive and the 'none' record
                ' no add'l sql needed
            Else
                If (Not IncludeInactive) And (Not IncludeNoneRecord) Then
                    ' Don't include inactive or the 'none' record
                    sql += " WHERE Active = 1 AND CompanyName <> 'None'"
                ElseIf Not IncludeInactive Then
                    ' Only active records
                    sql += " WHERE Active = 1"
                Else
                    ' All records except 'none'
                    sql += " WHERE CompanyName <> 'None'"
                End If
            End If
            sql += " ORDER BY CompanyName ASC"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetTrackedEventProviderList", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetTrackedEventProviderRecord(ByVal ProviderID As String) As DataRow
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim s As String = Nothing

        Try
            sql += "SELECT * FROM tbl_FNOL_CatCompany WHERE tbl_FNOL_CATCompany_Id = " & ProviderID

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl.Rows(0)
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetTrackedEventProviderRecord", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a list of manager ids of all managers assigned to the passed LOB
    ''' </summary>
    ''' <param name="LOB"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetLOBManagerIDs(ByVal LOB As LOB_enum, ByRef err As String) As List(Of String)
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim ids As New List(Of String)
        Dim txt As String = Nothing

        Try
            Select Case LOB
                Case LOB_enum.Auto
                    txt = "AUTO"
                    Exit Select
                Case LOB_enum.Liability
                    txt = "LIABILITY"
                    Exit Select
                Case LOB_enum.Propperty
                    txt = "PROPERTY"
                    Exit Select
            End Select
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT M.display_name, m.claimpersonnel_id, L.LOB FROM FNOLClaimAssign_Managers M INNER JOIN FNOL_ClaimAssign_ManagerLOBLink LNK ON M.claimpersonnel_id = LNK.manager_claimpersonnel_id INNER JOIN FNOL_ClaimAssign_LOB L ON LNK.FNOLLOB_id = L.FNOLLOB_id WHERE L.LOB = '" & txt & "'"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing

            For Each dr As DataRow In tbl.Rows
                ids.Add(dr("claimpersonnel_id").ToString())
            Next

            Return ids
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetLOBManagerIDs", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns all Adjusters for the passed LOB
    ''' Determines LOB by Adjuster Manager LOB assignments
    ''' </summary>
    ''' <param name="LOB"></param>
    ''' <param name="err"></param>
    ''' <param name="ShowValue"></param>
    ''' <param name="ShowNAAdjusters"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLOBAdjusters(ByVal LOB As LOB_enum, ByRef err As String, Optional ByVal ShowValue As ShowValue_enum = ShowValue_enum.Both, Optional ByVal ShowNAAdjusters As Boolean = True) As DataTable
        Dim sql As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim s As String = Nothing
        Dim NAID As String = Nothing
        Dim mgrids As List(Of String) = Nothing
        Dim inSQL As String = Nothing

        Try
            Select Case ShowValue
                Case ShowValue_enum.Both
                    sql = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE Active = 1"
                    Exit Select
                Case ShowValue_enum.ShownOnly
                    sql = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE ACTIVE = 1 AND show = 1"
                    Exit Select
                Case ShowValue_enum.HiddenOnly
                    sql = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE ACTIVE = 1 AND show = 0"
                    Exit Select
            End Select

            ' Get the N/A ID
            NAID = GetUnassignedID(err)
            If NAID Is Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception(err)
                Else
                    Throw New Exception("N/A ID was not found!!")
                End If
            End If

            ' Get manager ID's of all the managers assigned to the passed LOB
            mgrids = GetLOBManagerIDs(LOB, err)
            If mgrids Is Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception(err)
                Else
                    Throw New Exception("There are no managers assigned to the passed LOB")
                End If
            End If

            ' Build the 'IN' clause parameters with the list of manager Id's
            inSQL = "("
            For i As Integer = 0 To mgrids.Count - 1
                If i > 0 Then inSQL += ","
                inSQL += mgrids(i)
            Next
            inSQL += ")"

            If sql.Contains("WHERE") Then
                sql += " AND"
            Else
                sql += " WHERE"
            End If

            sql += " manager_claimpersonnel_id IN " & inSQL

            sql += " ORDER BY display_name ASC"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                err = "No Adjusters Found"
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetLOBAdjusters", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Synchronizes the Adjuster and Manager data with Diamond 
    ''' </summary>
    Public Sub SynchronizeWithDiamond(ByRef pg As Page)
        Dim err As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            SynchronizeAdjusterTableWithDiamond(pg, err)
            SynchronizeInternalManagerTable(err)

            ' update the sync date
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOL_ClaimAssign_Settings SET LastDiamondSyncDateTime = '" & DateTime.Now.ToShortDateString() & "'"
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Throw New Exception("Error updating LastDiamondSyncDateTime!")

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "SynchronizeWithDiamond", ex)
            Exit Sub
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Sub

    Private Function SynchronizeAdjusterTableWithDiamond(ByRef pg As Page, ByRef errstr As String) As Boolean
        Dim DIAAdjusters As New List(Of Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel)
        Dim FNOLAdjusters As New DataTable()
        Dim DiamondSecurityToken As Diamond.Common.Services.DiamondSecurityToken = Nothing
        Dim err As String = Nothing
        Dim found As Boolean = False

        Try
            ' Get Diamond Security Token
            If Not DiamondLogin(pg, err) Then Throw New Exception("Error logging into Diamond!")
            DiamondSecurityToken = GetDiamondSecurityToken(err)
            If DiamondSecurityToken Is Nothing OrElse err IsNot Nothing Then Throw New Exception("Error getting Diamond Security Token!")

            ' Get list of adjusters from Diamond, include disabled adjusters.
            DIAAdjusters = GetDiamondAdjustersViaDiamond(DiamondSecurityToken, err)
            If DIAAdjusters Is Nothing OrElse DIAAdjusters.Count <= 0 Then
                Throw New Exception("No adjuster objects returned from Diamond.")
            End If

            ' Make sure a matching adjuster record exists in the FNOL adjusters table
            For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In DIAAdjusters
                Dim cpid As String = adj.ClaimPersonnelId
                err = Nothing
                Dim FNOLAdj As DataRow = GetAdjusterRecordByClaimpersonnel_Id(cpid, err)
                If FNOLAdj Is Nothing AndAlso err Is Nothing Then
                    ' No adjuster record found.  Create one.
                    Dim outside As Boolean = False
                    If adj.ClaimAdjusterTypeDescription.ToUpper = "OUTSIDE" Then outside = True
                    If Not InsertInternalAdjusterRecord(cpid, adj.DisplayName, adj.ReportsToClaimPersonnelId.ToString, adj.OutOfOffice, outside, err) Then
                        If err IsNot Nothing Then
                            Throw New Exception("Error inserting a new adjuster record: " & err)
                        Else
                            Throw New Exception("Error inserting a new adjuster record.")
                        End If
                    End If
                Else
                    ' Adjuster record found
                    ' Update the out of office flag with diamond's value
                    Dim show As Boolean = False
                    If Not adj.OutOfOffice Then show = True
                    If Not UpdateFNOLAdjusterShowFlag(FNOLAdj("FNOLClaimAssignAdjuster_ID").ToString, show, err) Then
                        If err IsNot Nothing Then
                            Throw New Exception("Error updating adjuster OOO flag: " & err)
                        Else
                            Throw New Exception("Error updating adjuster OOO flag.")
                        End If
                    End If
                End If
            Next

            ' Remove any FNOL adjusters that are not in the diamond list
            FNOLAdjusters = GetFNOLCAAdjusterList(Nothing, False)
            If FNOLAdjusters Is Nothing OrElse FNOLAdjusters.Rows.Count <= 0 Then Throw New Exception("No FNOL Adjuster Records found!")
            For Each fnoladjdr As DataRow In FNOLAdjusters.Rows
                Dim cpid As String = fnoladjdr("claimpersonnel_id").ToString
                Dim FNOLAdjId As String = fnoladjdr("FNOLClaimAssignAdjuster_ID").ToString
                found = False
                For Each adj As Diamond.Common.Objects.Claims.Personnel.ClaimPersonnel In DIAAdjusters
                    If adj.ClaimPersonnelId.ToString = cpid Then
                        found = True
                        Exit For
                    End If
                Next
                If Not found Then
                    ' No matching Diamond adjuster for the FNOL adjuster.  Remove the FNOL adjuster.

                    ' In order to be able to delete the adjuster record we need to make sure there are no records in the 
                    ' AdjusterGroups and Counts tables with that adjuster id.  If there are records with those id's SQL will not allow us
                    ' to delete the Adjuster record while the Adjuster ID is in the AdjusterGroups and/or Counts tables because of foreign key restraints.

                    ' Delete the adjuster's adjuster groups records
                    Dim AdjGrpRecs As DataTable = GetAdjusterGroupsByFNOLClaimAssignAdjusterID(FNOLAdjId)
                    If AdjGrpRecs IsNot Nothing AndAlso AdjGrpRecs.Rows.Count > 0 Then
                        ' There ARE records is in the AdjusterGroup table with the Adjuster ID. Delete them.
                        If Not DeleteAdjusterGroupsRecordsForAdjuster(FNOLAdjId) Then
                            Throw New Exception("Error deleting adjuster groups records for adjuster id" & FNOLAdjId)
                        End If
                    Else
                        ' There are no records in the AdjusterGroup table with the adjuster id. 
                    End If

                    ' Delete the adjuster's counts
                    Dim CountRecs As DataTable = GetCountRecordsByFNOLClaimAssignAdjusterID(FNOLAdjId)
                    If CountRecs IsNot Nothing AndAlso CountRecs.Rows.Count > 0 Then
                        ' There ARE records is in the Counts table with the Adjuster ID. Delete them.
                        If Not DeleteCountRecordsForAdjuster(FNOLAdjId) Then
                            Throw New Exception("Error deleting Counts records for adjuster id " & FNOLAdjId)
                        End If
                    Else
                        ' There are no records in the Counts table with the adjuster id. 
                    End If

                    ' Finally we can delete the adjuster record itself.
                    If Not RemoveFNOLAdjusterRecord(FNOLAdjId) Then
                        Throw New Exception("Error removing FNOL Adjuster Record " & FNOLAdjId)
                    End If
                End If
            Next

            ' Synchronize the Diamond 'Out Of Office' flag with the FNOL 'Show' flag
            If Not SynchronizeAdjusterOutOfOfficeFlags(pg, err) Then
                If err IsNot Nothing Then
                    Throw New Exception("Error synchronizing the Diamond Out Of Office Flags: " & err)
                Else
                    Throw New Exception("Error synchronizing the Diamond Out Of Office Flags.")
                End If
            End If

            Return True
        Catch ex As Exception
            errstr = "SynchronizeAdjusterTableWithDiamond: " & ex.Message
            HandleError(ClassName, "SynchronizeAdjusterTableWithDiamond", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Synchronizes only the out of office flags.  Much more efficient than the full synchronization above
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <returns></returns>
    Private Function SynchronizeAdjusterOutOfOfficeFlags(ByRef pg As Page, ByRef errstr As String) As Boolean
        Dim FNOLAdjusters As New DataTable()
        Dim DiamondSecurityToken As Diamond.Common.Services.DiamondSecurityToken = Nothing
        Dim err As String = Nothing

        Try
            ' Get Diamond Security Token
            If Not DiamondLogin(pg, err) Then Throw New Exception("Error logging into Diamond!")
            DiamondSecurityToken = GetDiamondSecurityToken(err)
            If DiamondSecurityToken Is Nothing OrElse err IsNot Nothing Then Throw New Exception("Error getting Diamond Security Token!")

            ' Synchronize the Diamond 'Out Of Office' flag with the FNOL 'Show' flag
            FNOLAdjusters = GetFNOLCAAdjusterList(Nothing, False)
            If FNOLAdjusters Is Nothing OrElse FNOLAdjusters.Rows.Count <= 0 Then Throw New Exception("No FNOL Adjuster Records found!")
            For Each fnoladjdr As DataRow In FNOLAdjusters.Rows
                Dim FNOLAdjId As String = fnoladjdr("FNOLClaimAssignAdjuster_ID").ToString
                Dim cpid As String = fnoladjdr("claimpersonnel_id").ToString
                Dim OOO As Boolean = GetDiamondOutOfOfficeFlagValueForAdjuster(cpid, err)
                If (Not OOO) AndAlso err IsNot Nothing Then
                    GoTo nextone
                    'Throw New Exception("Error getting OOO value: " & err)
                End If
                Dim show As Boolean = False
                If fnoladjdr("show").ToString.ToUpper.ToUpper = "TRUE" Then show = True
                If OOO Then
                    If show Then UpdateFNOLAdjusterShowFlag(FNOLAdjId, False, err)
                Else
                    If Not show Then UpdateFNOLAdjusterShowFlag(FNOLAdjId, True, err)
                End If
nextone:
            Next

            Return True
        Catch ex As Exception
            errstr = "SynchronizeAdjusterOutOfOfficeFlags: " & ex.Message
            HandleError(ClassName, "SynchronizeAdjusterOutOfOfficeFlags", ex)
            Return False
        End Try
    End Function

    Private Function UpdateFNOLAdjusterActiveFlag(ByVal FNOLAdjusterID As String, ByVal FlagValue As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Dim blval As String = "0"

        Try
            If FlagValue Then blval = "1"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOLClaimAssign_Adjusters SET Active = " & blval & " WHERE FNOLClaimAssignAdjuster_ID = " & FNOLAdjusterID
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If (Not rtn Is Nothing) AndAlso IsNumeric(rtn) Then Return True Else Return False
        Catch ex As Exception
            err = "UpdateFNOLAdjusterActiveFlag: " & ex.Message
            HandleError(ClassName, "UpdateFNOLAdjusterActiveFlag", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function UpdateFNOLAdjusterShowFlag(ByVal FNOLAdjusterID As String, ByVal FlagValue As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Dim blval As String = "0"

        Try
            If FlagValue Then blval = "1"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOLClaimAssign_Adjusters SET show = " & blval & " WHERE FNOLClaimAssignAdjuster_ID = " & FNOLAdjusterID
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If (Not rtn Is Nothing) AndAlso IsNumeric(rtn) Then Return True Else Return False
        Catch ex As Exception
            err = "UpdateFNOLAdjusterOutOfOfficeFlag: " & ex.Message
            HandleError(ClassName, "UpdateFNOLAdjusterShowFlag", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates all the AdjusterGroups records that have the old adjuster id to the new adjuster id
    ''' </summary>
    ''' <param name="oldFNOLClaimAssignAdjuster_Id"></param>
    ''' <param name="newFNOLClaimAssignAdjuster_Id"></param>
    ''' <returns></returns>
    Private Function ChangeAdjusterGroupsAdjusterId(ByVal oldFNOLClaimAssignAdjuster_Id As String, ByVal newFNOLClaimAssignAdjuster_Id As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE AdjusterGroups SET FNOLClaimAssignAdjuster_ID = " & newFNOLClaimAssignAdjuster_Id & " WHERE FNOLClaimAssignAdjuster_ID = " & oldFNOLClaimAssignAdjuster_Id
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If (Not rtn Is Nothing) AndAlso IsNumeric(rtn) Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "ChangeAdjusterGroupsAdjusterID", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Deletes all adjustergroups records for the specified adjuster from the adjustergroups table 
    ''' </summary>
    ''' <returns></returns>
    Private Function DeleteAdjusterGroupsRecordsForAdjuster(ByVal FNOLAdjusterId As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "DELETE AdjusterGroups WHERE FNOLClaimAssignAdjuster_ID = " & FNOLAdjusterId
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If (Not rtn Is Nothing) AndAlso IsNumeric(rtn) Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "DeleteAdjusterGroupsRecordsForAdjuster", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Deletes all counts records for the specified adjuster from the counts table 
    ''' </summary>
    ''' <returns></returns>
    Private Function DeleteCountRecordsForAdjuster(ByVal FNOLAdjusterId As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "DELETE Counts WHERE FNOLClaimAssignAdjuster_ID = " & FNOLAdjusterId
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If (Not rtn Is Nothing) AndAlso IsNumeric(rtn) Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "DeleteCountRecordsForAdjuster", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates all the Counts records that have the old adjuster id to the new adjuster id
    ''' </summary>
    ''' <param name="oldFNOLClaimAssignAdjuster_Id"></param>
    ''' <param name="newFNOLClaimAssignAdjuster_Id"></param>
    ''' <returns></returns>
    Private Function ChangeCountsAdjusterId(ByVal oldFNOLClaimAssignAdjuster_Id As String, ByVal newFNOLClaimAssignAdjuster_Id As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE Counts SET FNOLClaimAssignAdjuster_ID = " & newFNOLClaimAssignAdjuster_Id & " WHERE FNOLClaimAssignAdjuster_ID = " & oldFNOLClaimAssignAdjuster_Id
            rtn = cmd.ExecuteNonQuery()
            If rtn Is Nothing Then Return False
            If (Not rtn Is Nothing) AndAlso IsNumeric(rtn) Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "ChangeCountsAdjusterID", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Checks for a duplicate record in the adjuster or manager table for the passed name
    ''' Required because some of the names in the Adjuster/Manager tables (in Diamond) are duplicated
    ''' </summary>
    ''' <param name="AdjusterOrManager"></param>
    ''' <param name="NameValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckForDuplicateAdjusterOrManager(ByVal AdjusterOrManager As String, ByVal NameValue As String, CPId As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim sql As String = Nothing
        Dim newname As String = NameValue.Replace("'", "")

        Try
            If AdjusterOrManager Is Nothing OrElse AdjusterOrManager.Length = 0 OrElse
                (AdjusterOrManager.Substring(0, 1).ToUpper() <> "A" AndAlso AdjusterOrManager.Substring(0, 1).ToUpper() <> "M") Then
                Throw New Exception("Invalid or Missing AdjusterOrManager parameter")
            End If

            If AdjusterOrManager.Substring(0, 1).ToUpper() = "A" Then
                'sql = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE display_name = '" & newname & "' OR claimpersonnel_id = " & CPId
                sql = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE claimpersonnel_id = " & CPId
            Else
                'sql = "SELECT * FROM FNOLClaimAssign_Managers WHERE display_name = '" & newname & "' OR claimpersonnal_id = " & CPId
                sql = "SELECT * FROM FNOLClaimAssign_Managers WHERE claimpersonnel_id = " & CPId
            End If

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "CheckForDuplicateAdjusterOrManager", ex)
            ' We return true on error because if the function fails we don't want the possibility of entering a duplicate record
            Return True
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Checks for a duplicate record in the user access table for the passed userid
    ''' Returns true if a duplicate was found, false if not
    ''' </summary>
    ''' <param name="userid"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function CheckForDuplicateUserAccessRecord(ByVal userid As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim sql As String = Nothing

        Try
            sql = "SELECT * FROM FNOL_ClaimAssign_UserAccessList WHERE userid = '" & userid & "'"

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "CheckForDuplicateUserAccessRecord", ex)
            ' We return true on error because if the function fails we don't want the possibility of entering a duplicate record
            Return True
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Checks for an existing tracked event provider record with the passed name.
    ''' Returns true if a duplicate was found, false if not.
    ''' If an id is passed, excludes that record from the search - needed for edit
    ''' </summary>
    ''' <param name="ProviderName"></param>
    ''' <param name="err"></param>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckForDuplicateProviderName(ByVal ProviderName As String, ByRef err As String, Optional ByVal id As String = Nothing) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim sql As String = Nothing

        Try
            If id IsNot Nothing Then
                If id.Trim = String.Empty OrElse (Not IsNumeric(id)) Then id = Nothing
            End If

            sql = "SELECT * FROM tbl_FNOL_CATCompany WHERE CompanyName = '" & ProviderName & "'"
            If id IsNot Nothing Then sql += " AND tbl_FNOL_CATCompany_Id <> " & id

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "CheckForDuplicateProviderName:" & sql, ex)
            ' We return true on error because if the function fails we don't want the possibility of entering a duplicate record
            Return True
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Checks for an existing tracked event provider record with the passed name.
    ''' If an ID is passed, that record will not be included in the search.  Needed when updating an existing record.
    ''' 
    ''' Returns true if a duplicate was found, false if not
    ''' </summary>
    ''' <param name="ProviderAbbrev"></param>
    ''' <param name="err"></param>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckForDuplicateProviderAbbrev(ByVal ProviderAbbrev As String, ByRef err As String, Optional ByVal id As String = Nothing) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim sql As String = Nothing

        Try
            If id IsNot Nothing Then
                If id.Trim = String.Empty OrElse (Not IsNumeric(id)) Then id = Nothing
            End If

            sql = "SELECT * FROM tbl_FNOL_CATCompany WHERE CompanyAbbreviation = '" & ProviderAbbrev & "'"
            If id IsNot Nothing Then sql += " AND tbl_FNOL_CATCompany_Id <> " & id

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "CheckForDuplicateProviderAbbrev", ex)
            ' We return true on error because if the function fails we don't want the possibility of entering a duplicate record
            Return True
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pulls a datatable of adjusters assigned to the passed manager
    ''' </summary>
    ''' <param name="Manager_claimpersonnelID"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetManagerAdjusters(ByVal Manager_claimpersonnelID As String, ByRef err As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE manager_claimpersonnel_id = " & Manager_claimpersonnelID
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetManagerAdjusters", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pulls a datatable of adjusters NOT assigned to the passed manager
    ''' </summary>
    ''' <param name="Manager_claimpersonnelID"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNONManagerAdjusters(ByVal Manager_claimpersonnelID As String, ByRef err As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE manager_claimpersonnel_id <> " & Manager_claimpersonnelID
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetNONManagerAdjusters", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Inserts a record into the FNOLClaimAssign_Personnel table
    ''' </summary>
    ''' <param name="ClaimPersonnel_ID"></param>
    ''' <param name="DisplayName"></param>
    ''' <param name="Show"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertInternalAdjusterRecord(ByVal ClaimPersonnel_ID As String, ByVal DisplayName As String, ByVal Manager_claimpersonnel_id As String, ByVal Show As Boolean, ByVal OutsideAdjuster As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim showval As String = Nothing
        Dim newname As String = Nothing
        Dim OAval As String = "0"

        Try
            If Show Then showval = "1" Else showval = "0"
            If OutsideAdjuster Then OAval = "1"

            ' Scrub any single quotes from name
            newname = DisplayName.Replace("'", "")

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "INSERT INTO FNOLClaimAssign_Adjusters (Display_Name, claimpersonnel_id, show, manager_claimpersonnel_id, OutsideAdjuster) VALUES('" & newname & "', " & ClaimPersonnel_ID & ", " & showval & ", " & Manager_claimpersonnel_id & ", " & OAval & ")"
            rtn = cmd.ExecuteNonQuery()
            If rtn <> 1 Then Throw New Exception("Insert Failed.  Return value: " & rtn.ToString())

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "InsertInternalAdjusterRecord", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates an adjuster record
    ''' </summary>
    ''' <param name="ClaimPersonnel_ID"></param>
    ''' <param name="DisplayName"></param>
    ''' <param name="Manager_claimpersonnel_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateInternalAdjusterRecord(ByVal ClaimPersonnel_ID As String, ByVal DisplayName As String, ByVal Manager_claimpersonnel_id As String, ByVal Active As Boolean, ByVal OutsideAdjuster As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim showval As String = Nothing
        Dim activeval As String = Nothing
        Dim newname As String = Nothing
        Dim sql As String = Nothing

        Try
            If Active Then activeval = 1 Else activeval = 0

            ' Scrub any single quotes from name
            newname = DisplayName.Replace("'", "")

            ' Build the sql
            sql = "UPDATE FNOLClaimAssign_Adjusters SET "
            sql += "claimpersonnel_id = " & ClaimPersonnel_ID
            sql += ", display_name = '" & DisplayName & "'"
            sql += ", active = " & activeval
            sql += ", manager_claimpersonnel_id = " & Manager_claimpersonnel_id
            sql += ", OutsideAdjuster = "
            If OutsideAdjuster Then
                sql += "1"
            Else
                sql += "0"
            End If
            sql += " WHERE claimpersonnel_ID = " & ClaimPersonnel_ID

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            rtn = cmd.ExecuteNonQuery()
            If rtn <> 1 Then Throw New Exception("Update Failed.  Return value: " & rtn.ToString())

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "UpdateInternalAdjusterRecord", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' returns true if the passed adjuster is in the manager table
    ''' </summary>
    ''' <param name="AdjusterName"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AdjusterIsManager(ByVal AdjusterName As String, ByRef err As String) As Boolean
        Dim tblMgr As DataTable = Nothing

        Try
            tblMgr = GetManagerList(ShowValue_enum.Both, err)
            If tblMgr Is Nothing OrElse tblMgr.Rows.Count <= 0 Then
                If err IsNot Nothing AndAlso err <> String.Empty Then Throw New Exception(err)
                Throw New Exception("No Managers Found")
            End If

            For Each mgr As DataRow In tblMgr.Rows
                If mgr("display_name").ToString().ToUpper() = AdjusterName.ToUpper() Then Return True
            Next

            Return False
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "AdjusterIsManager", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Updates a manager record
    ''' </summary>
    ''' <param name="ClaimPersonnel_ID"></param>
    ''' <param name="DisplayName"></param>
    ''' <param name="IFMUserName"></param>
    ''' <param name="LOBs"></param>
    ''' <param name="Show"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateInternalManagerRecord(ByVal ClaimPersonnel_ID As String, ByVal DisplayName As String, ByVal IFMUserName As String, ByVal LOBs As List(Of String), ByVal Show As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim showval As String = Nothing
        Dim newname As String = Nothing
        Dim sql As String = Nothing
        Dim txn As SqlTransaction = Nothing
        Dim newid As Integer = -1
        Dim AutoLOB As String = Nothing
        Dim PropLOB As String = Nothing
        Dim LiabLOB As String = Nothing

        Try
            If Show Then showval = 1 Else showval = 0

            ' Scrub any single quotes from name
            newname = DisplayName.Replace("'", "")

            ' ****************************************************
            ' MULTI-PART UPDATE WITHIN A TRANSACTION
            ' ****************************************************

            ' ************************************
            ' 1.  Update the Manager table
            ' ************************************
            ' Build the sql to update the managers table
            sql = "UPDATE FNOLClaimAssign_Managers SET "
            sql += "claimpersonnel_id = " & ClaimPersonnel_ID
            sql += ", display_name = '" & DisplayName & "'"
            sql += ", IFM_AD_UserName = '" & IFMUserName & "'"
            sql += ", show = " & showval
            sql += " WHERE claimpersonnel_id = " & ClaimPersonnel_ID

            ' Update the manager record
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            txn = conn.BeginTransaction()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            cmd.Transaction = txn
            rtn = cmd.ExecuteNonQuery()
            If rtn <> 1 Then Throw New Exception("Update Failed.  Return value: " & rtn.ToString())
            newid = CInt(rtn)

            ' ************************************
            ' 2.  Update the LOB Links
            ' ************************************
            ' Delete existing LOB links
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "DELETE FROM FNOL_ClaimAssign_ManagerLOBLink WHERE manager_claimpersonnel_id = " & ClaimPersonnel_ID
            cmd.Transaction = txn
            rtn = cmd.ExecuteNonQuery()

            ' Update the lob links
            AutoLOB = GetLOBID("auto", err)
            If AutoLOB = Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception("error getting lob id 1!")
                Else
                    Throw New Exception("LOB 1 Not found!")
                End If
            End If
            PropLOB = GetLOBID("property", err)
            If PropLOB = Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception("error getting lob id 2!")
                Else
                    Throw New Exception("LOB 2 Not found!")
                End If
            End If
            LiabLOB = GetLOBID("liability", err)
            If LiabLOB = Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception("error getting lob id 3!")
                Else
                    Throw New Exception("LOB 3 Not found!")
                End If
            End If
            For Each lob As String In LOBs
                Dim id As String = Nothing
                Select Case lob.ToUpper()
                    Case "AUTO"
                        id = AutoLOB
                        Exit Select
                    Case "PROPERTY"
                        id = PropLOB
                        Exit Select
                    Case "LIABILITY"
                        id = LiabLOB
                        Exit Select
                End Select
                cmd = New SqlCommand()
                cmd.Connection = conn
                cmd.CommandType = CommandType.Text
                sql = "INSERT INTO FNOL_ClaimAssign_ManagerLOBLink (FNOLLOB_id, manager_claimpersonnel_id) "
                sql += " VALUES (" & id & ", " & ClaimPersonnel_ID & ")"
                cmd.CommandText = sql
                cmd.Transaction = txn
                rtn = cmd.ExecuteNonQuery()
                If rtn <> 1 Then Throw New Exception("Error updating LOB links!  Return value is " & rtn.ToString())
            Next

            ' Good to go!
            txn.Commit()

            Return True
        Catch ex As Exception
            txn.Rollback()
            err = ex.Message
            HandleError(ClassName, "UpdateInternalManagerRecord", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns the lob id for the passed LOB
    ''' </summary>
    ''' <param name="LOB"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetLOBID(ByVal LOB As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLLOB_id FROM FNOL_ClaimAssign_LOB WHERE LOB = '" & LOB & "'"
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing OrElse Not IsNumeric(rtn) Then
                Return Nothing
            Else
                Return rtn.ToString()
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetLOBID", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Get adjuster claimpersonnel_id by FNOLClaimAssignAdjuster_Id
    ''' </summary>
    ''' <param name="AdjusterId"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function GetAdjusterClaimpersonnel_ID_ById(ByVal AdjusterId As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE FNOLClaimAssignAdjuster_Id = " & AdjusterId
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing
            Return tbl.Rows(0)("claimpersonnel_id").ToString()
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetAdjusterClaimpersonnel_ID", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a record from FNOLClaimAssign_Adjusters for the passed FNOLClaimAssignAdjuster_Id
    ''' </summary>
    ''' <param name="FNOLClaimAssignAdjuster_Id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function GetAdjusterRecordByFNOLClaimAssignAdjuster_Id(ByVal FNOLClaimAssignAdjuster_Id As String, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE FNOLClaimAssignAdjuster_Id = " & FNOLClaimAssignAdjuster_Id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing
            If tbl.Rows.Count > 1 Then Throw New Exception("More than one row returned!")

            Return tbl.Rows(0)
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetAdjusterRecordByFNOL_Id", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a record from FNOLClaimAssign_Adjusters for the passed claimpersonnel_id
    ''' </summary>
    ''' <param name="claimpersonnel_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdjusterRecordByClaimpersonnel_Id(ByVal claimpersonnel_id As String, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE claimpersonnel_id = " & claimpersonnel_id & " ORDER BY claimpersonnel_id DESC"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing
            'If tbl.Rows.Count > 1 Then Throw New Exception("More than one row returned!")

            ' Always returns the latest row when more than one row is in the table
            Return tbl.Rows(0)
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetAdjusterRecordByClaimpersonnel_Id", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Makes sure that there's a matching record in the FNOLClaimAssign_Managers table for all of the managers found in the Diamond
    ''' claims personnel table
    ''' </summary>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SynchronizeInternalManagerTable(ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim DIAMONDManagers As New DataTable()
        Dim sql As String = Nothing

        Try
            ' Get a DISTINCT list of managers
            sql = "SELECT DISTINCT N2.display_name AS Manager, CP2.claimpersonnel_id FROM ClaimPersonnel AS CP WITH (NOLOCK) "
            sql += "INNER JOIN UserEmployeeLink AS UEL WITH (NOLOCK) on UEL.users_id = CP.users_id "
            sql += "INNER JOIN Employee E with (nolock) ON E.employee_id = UEL.employee_id "
            sql += "INNER JOIN Name N with (nolock) ON E.name_id = N.name_id "
            sql += "INNER JOIN ClaimPersonnel CP2 ON CP.reports_to_claimpersonnel_id = CP2.claimpersonnel_id "
            sql += "INNER JOIN UserEmployeeLink AS UEL2 WITH (NOLOCK) on UEL2.users_id = CP2.users_id "
            sql += "INNER JOIN Employee E2 with (nolock) ON E2.employee_id = UEL2.employee_id "
            sql += "INNER JOIN Name N2 with (nolock) ON E2.name_id = N2.name_id "
            sql += "WHERE CP.claimpersonneltype_id = 3 AND CP.ENABLED = 1 ORDER BY N2.display_name ASC"

            conn = New SqlConnection(strConnDiamond)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(DIAMONDManagers)

            If DIAMONDManagers Is Nothing OrElse DIAMONDManagers.Rows.Count <= 0 Then Throw New Exception("No DIAMOND Managers Found!")

            ' Make sure there's a corresponding record in the internal manager table for each one in the Diamond list
            For Each dr As DataRow In DIAMONDManagers.Rows
                Dim mgrrow As DataRow = GetManagerRecord(dr("claimpersonnel_id").ToString(), err)
                If mgrrow Is Nothing Then
                    If Not CheckForDuplicateAdjusterOrManager("M", dr("Manager").ToString(), dr("claimpersonnel_id").ToString(), err) Then
                        ' No matching record was found in the internal adjuster table.  Create one.
                        If Not InsertInternalManagerRecord(dr("claimpersonnel_id").ToString(), dr("Manager").ToString(), True, err) Then
                            Throw New Exception("Error inserting record into FNOLClaimAssign_Managers: " & err)
                        End If
                    Else
                        If err IsNot Nothing AndAlso err <> String.Empty Then
                            Throw New Exception("Error checking for duplicate record: " & err)
                        End If
                    End If
                End If
            Next

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "SynchronizeInternalManagerTable", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Pulls a record from the manager table by claimpersonnel_id
    ''' </summary>
    ''' <param name="claimpersonnel_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetManagerRecord(ByVal claimpersonnel_id As String, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Managers WHERE claimpersonnel_id = " & claimpersonnel_id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing
            If tbl.Rows.Count > 1 Then Throw New Exception("More than one row returned!")

            Return tbl.Rows(0)
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetManagerRecordByName", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Inserts a record into the adjuster table
    ''' </summary>
    ''' <param name="ClaimPersonnel_ID"></param>
    ''' <param name="DisplayName"></param>
    ''' <param name="Show"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertInternalManagerRecord(ByVal ClaimPersonnel_ID As String, ByVal DisplayName As String, ByVal Show As Boolean, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim showval As String = Nothing
        Dim newname As String = Nothing

        Try
            If Show Then showval = 1 Else showval = 0

            ' Scrub any single quotes from name
            newname = DisplayName.Replace("'", "")

            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "INSERT INTO FNOLClaimAssign_Managers (Display_Name, claimpersonnel_id, show) VALUES('" & newname & "', " & ClaimPersonnel_ID & ", " & showval & ")"
            rtn = cmd.ExecuteNonQuery()
            If rtn <> 1 Then Throw New Exception("Insert Failed.  Return value: " & rtn.ToString())

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "InsertInternalManagerRecord", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets CAC Group Name by Id
    ''' </summary>
    ''' <param name="GroupId"></param>
    ''' <returns></returns>
    Public Function GetGroupName(ByVal GroupId As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            If Not OpenCACConnection(conn) Then Return ""
            cmd.Connection = conn
            cmd.CommandText = "SELECT GroupName FROM Groups WHERE Groups_Id = " & GroupId
            cmd.CommandType = CommandType.Text
            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing Then
                Return rtn.ToString
            Else
                Return ""
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetGroupName", ex)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Assigns the passed adjuster to the passed manager by setting the manager_claimpersonnel_id in the adjuster table
    ''' </summary>
    ''' <param name="Adjuster_claimpersonnel_id"></param>
    ''' <param name="Manager_claimpersonnel_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AssignAdjusterManager(ByVal Adjuster_claimpersonnel_id As String, ByVal Manager_claimpersonnel_id As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim showval As String = Nothing
        Dim newname As String = Nothing

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOLClaimAssign_Adjusters SET manager_claimpersonnel_id = " & Manager_claimpersonnel_id & " WHERE claimpersonnel_id = " & Adjuster_claimpersonnel_id
            rtn = cmd.ExecuteNonQuery()
            If rtn <> 1 Then Throw New Exception("Update of Adjuster Manager field Failed.  Return value: " & rtn.ToString())

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "AssignAdjusterManager", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns a list of all LOBs assigned to a manager
    ''' </summary>
    ''' <param name="manager_claimpersonnel_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetManagerLOBs(ByVal manager_claimpersonnel_id As String, ByRef err As String) As List(Of String)
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim lobs As New List(Of String)

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT M.display_name, m.claimpersonnel_id, L.LOB FROM FNOLClaimAssign_Managers M INNER JOIN FNOL_ClaimAssign_ManagerLOBLink LNK ON M.claimpersonnel_id = LNK.manager_claimpersonnel_id INNER JOIN FNOL_ClaimAssign_LOB L ON LNK.FNOLLOB_id = L.FNOLLOB_id WHERE M.claimpersonnel_id = " & manager_claimpersonnel_id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return Nothing

            For Each dr As DataRow In tbl.Rows
                lobs.Add(dr("LOB").ToString())
            Next

            Return lobs
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetManagerLOBs", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Assigns N/A manager to all Adjusters that are assigned to the passed manager
    ''' </summary>
    ''' <param name="Manager_claimpersonnel_id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ClearManagerAdjusters(ByVal Manager_claimpersonnel_id As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim showval As String = Nothing
        Dim newname As String = Nothing
        Dim NAID As String = Nothing

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()

            ' Get the N/A ID
            NAID = GetUnassignedID(err)
            If NAID Is Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception(err)
                Else
                    Throw New Exception("N/A ID was not found!!")
                End If
            End If

            ' Set all of the passed manager's adjusters to manager N/A
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE FNOLClaimAssign_Adjusters SET manager_claimpersonnel_id = " & NAID.ToString() & " WHERE manager_claimpersonnel_id = " & Manager_claimpersonnel_id
            rtn = cmd.ExecuteNonQuery()

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "ClearManagerAdjusters", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns the Diamond claim control id for a claim
    ''' </summary>
    ''' <param name="DiamondClaimNumber"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetClaimControl_Id(ByVal DiamondClaimNumber As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim tbl As New DataTable()
        Dim da As New SqlDataAdapter()

        Try
            ' Get the ClaimControl record
            conn = New SqlConnection(strConnDiamond)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM ClaimControl WHERE Claim_Number = '" & DiamondClaimNumber & "'"
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Claim Control record not found!!")

            Return tbl.Rows(0)("ClaimControl_Id").ToString
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetClaimControl_Id", ex)
            Return Nothing
        Finally
            If conn IsNot Nothing Then
                If conn.State = ConnectionState.Open Then conn.Close()
                conn.Dispose()
            End If
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets Diamond Security Token for logged in user
    ''' </summary>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDiamondSecurityToken(ByRef err As String) As Diamond.Common.Services.DiamondSecurityToken
        Dim reqLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Request
        Dim rspLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Response
        Dim uName As String = Nothing
        Dim uPwd As String = Nothing

        Try
            If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> String.Empty Then
                uName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
                uPwd = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
            Else
                If AppSettings("TestDiamondTokenUserName") IsNot Nothing AndAlso AppSettings("TestDiamondTokenUserName") <> "" Then
                    uName = AppSettings("TestDiamondTokenUserName")
                End If
                If AppSettings("TestDiamondTokenPassword") IsNot Nothing AndAlso AppSettings("TestDiamondTokenPassword") <> "" Then
                    uPwd = AppSettings("TestDiamondTokenPassword")
                End If
            End If

            With reqLogin.RequestData
                .LoginName = uName
                .Password = uPwd
            End With

            Using loginProxy As New Diamond.Common.Services.Proxies.LoginServiceProxy
                rspLogin = loginProxy.GetDiamTokenForUsernamePassword(reqLogin)
            End Using

            If rspLogin.ResponseData IsNot Nothing Then
                Return rspLogin.ResponseData.DiamondSecurityToken
            Else
                Return Nothing
            End If

        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetDiamondSecurityToken", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' This function uses the Diamond SaveClaimControlPersonnel service
    ''' </summary>
    ''' <param name="Adjuster_Id"></param>
    ''' <param name="Supervisor_Id"></param>
    ''' <param name="DiamondClaimNumber"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AssignDiamondClaimControlPersonnel(ByVal Adjuster_Id As String, ByVal Supervisor_Id As String, ByVal DiamondClaimNumber As String, ByRef err As String) As Boolean
        Dim REQ As New Diamond.Common.Services.Messages.ClaimsService.SaveClaimControlPersonnel.Request
        Dim RESP As New Diamond.Common.Services.Messages.ClaimsService.SaveClaimControlPersonnel.Response
        Dim CCID As String = Nothing
        Dim CompanyID As String = Nothing

        Try
            'System.Threading.Thread.Sleep(120000)  ' sleep for 2 minutes FOR TESTING ONLY!!
            ' Get the company ID and company LOB ID
            CompanyID = GetCompanyID(DiamondClaimNumber, err)
            If CompanyID < 0 Then Throw New Exception(err)

            ' Get the claimcontrol Id
            CCID = GetClaimControl_Id(DiamondClaimNumber, err)
            If CCID Is Nothing Then Throw New Exception(err)

            Using proxy As New Diamond.Common.Services.Proxies.ClaimsServiceProxy
                Dim token As Diamond.Common.Services.DiamondSecurityToken = GetDiamondSecurityToken(err)
                If token Is Nothing Then Throw New Exception(err)
                REQ.DiamondSecurityToken = token
                REQ.RequestData.ClaimControlId = CInt(CCID)
                REQ.RequestData.Personnel = New Diamond.Common.Objects.Claims.PersonnelData
                REQ.RequestData.Personnel.InsideAdjusterId = CInt(Adjuster_Id)
                REQ.RequestData.Personnel.AdministratorId = CInt(Supervisor_Id)
                REQ.RequestData.Personnel.ClaimControlId = CCID
                REQ.RequestData.Personnel.ClaimOfficeId = CompanyID
                RESP = proxy.SaveClaimControlPersonnel(REQ)
                If RESP Is Nothing Then Throw New Exception("SaveClaimControlPersonnel response is nothing!")
                If RESP.ResponseData.Success Then
                    Return True
                Else
                    If RESP.DiamondValidation IsNot Nothing AndAlso RESP.DiamondValidation.ValidationItems IsNot Nothing AndAlso RESP.DiamondValidation.ValidationItems.Count > 0 Then
                        Dim s As String = String.Empty
                        For Each v As Diamond.Common.Objects.ValidationItem In RESP.DiamondValidation.ValidationItems
                            s += v.Message + "; "
                        Next
                        Throw New Exception("SaveClaimControlPersonnel failed: " & s)
                    Else
                        Throw New Exception("SaveClaimControlPersonnel failed!")
                    End If
                End If
            End Using
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "AssignDiamondClaimControlPersonnel", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Gets the IFM Company ID from Diamond 
    ''' </summary>
	''' <param name="DiamondClaimNumber "></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCompanyID(ByVal DiamondClaimNumber As String, ByRef err As String) As String
        Dim LOBAbbrev As String = Nothing
        Dim searchstr As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim dr As DataRow = Nothing

        Try
            ' Get the last record in the Company table and return it's ID
            conn = New SqlConnection(strConnDiamond)
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "select V.company_id, * from ClaimControl as CC with (nolock) inner join PolicyImage as PI with (nolock) on PI.policy_id = CC.policy_id and PI.policyimage_num = CC.policyimage_num inner join Version as V with (nolock) on V.version_id = PI.version_id WHERE Claim_Number = '" & DiamondClaimNumber & "'"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("No Company Records found!")
            Return tbl.Rows(0)("company_id").ToString

        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetCompanyID", ex, Nothing)
            Return -1
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets the IFM Company ID from Diamond
    ''' </summary>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCompanyID(ByRef err As String) As Integer
        Dim LOBAbbrev As String = Nothing
        Dim searchstr As String = Nothing
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim dr As DataRow = Nothing

        Try
            ' Get the last record in the Company table and return it's ID
            conn = New SqlConnection(strConnDiamond)
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM Company ORDER BY Company_id"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("No Company Records found!")

            dr = tbl.Rows(tbl.Rows.Count - 1)
            Return CInt(dr("Company_id"))
        Catch ex As Exception
            err = ex.Message
            HandleError("modCommon", "GetCompanyID", ex)
            Return -1
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets the territory number for the passed policy
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPolicyTerritoryNumber(ByVal PolicyNumber As String, ByRef err As String) As String
        Dim po As PolicyNumberObject = Nothing
        Try
            po = New PolicyNumberObject(PolicyNumber)
            If po.hasError Then
                If po.errorMsg IsNot Nothing AndAlso po.errorMsg <> String.Empty Then
                    Throw New Exception(po.errorMsg)
                Else
                    Throw New Exception("Unable to load policy number object")
                End If
            Else
                po.GetAllAgencyInfo = True
                po.GetPolicyInfo()
                If po.hasPolicyInfo Then
                    If po.AgencyInfo IsNot Nothing AndAlso po.AgencyInfo.AgencyTerritory IsNot Nothing AndAlso po.AgencyInfo.AgencyTerritory <> String.Empty Then
                        Return po.AgencyInfo.AgencyTerritory.PadLeft(2, "0")
                    Else
                        Throw New Exception("Territory was not on policy object (AgencyTerritory)")
                    End If
                Else
                    If po.hasPolicyInfoError Then
                        If po.PolicyInfoError IsNot Nothing AndAlso po.PolicyInfoError <> String.Empty Then
                            Throw New Exception(po.PolicyInfoError)
                        Else
                            Throw New Exception("Unable to load policy info object")
                        End If
                    End If
                End If
            End If
            Return Nothing
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetPolicyTerritoryNumber", ex)
            Return Nothing
        Finally
            If po IsNot Nothing Then po.Dispose()
        End Try
    End Function

    'Private Function UploadToOnbase(data As Byte(), filename As String, claimNumber As String) As Int32
    '    Dim package As New IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload()
    '    package.SetBase64Payload(data)
    '    package.SetExtensionFromFullFileName(filename)

    '    package.SourceSystem = IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload.SourceSystems.FNOLAssignment
    '    package.Keys.Add(New IFM.DataServicesCore.CommonObjects.OnBase.OnBaseKey() With {.Key = IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload.KeyTypes.claimNumber, .Values = {claimNumber}})
    '    package.Keys.Add(New IFM.DataServicesCore.CommonObjects.OnBase.OnBaseKey() With {.Key = IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload.KeyTypes.description, .Values = {"FNOL intranet upload."}})

    '    Using proxy As New IFM.JsonProxyClient.ProxyClient(ConfigurationManager.AppSettings("IFMDataServices_EndPointBaseUrl"))
    '        Dim response = proxy.Post("OnBase/Document/Upload", package)
    '        Dim responseText = proxy.GetResponsePayload(response)
    '        If (response.StatusCode = Net.HttpStatusCode.OK) Then
    '            Dim documentId As Int64 = 0
    '            Int64.TryParse(responseText, documentId)
    '            Return documentId
    '        End If
    '        Return 0
    '    End Using
    'End Function

    Private Function UploadToOnbase(data As Byte(), filename As String, claimNumber As String) As Int32
        Dim package As New IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload()
        package.SetBase64Payload(data)
        package.SetExtensionFromFullFileName(filename)

        package.SourceSystem = DataServices.API.RequestObjects.OnBase.DocumentUpload.SourceSystems.FNOLAssignment
        package.Keys.Add(New IFM.DataServicesCore.CommonObjects.OnBase.OnBaseKey() With {.Key = DataServices.API.RequestObjects.OnBase.DocumentUpload.KeyTypes.claimNumber, .Values = {claimNumber}})
        package.Keys.Add(New IFM.DataServicesCore.CommonObjects.OnBase.OnBaseKey() With {.Key = DataServices.API.RequestObjects.OnBase.DocumentUpload.KeyTypes.description, .Values = {"FNOL intranet upload."}})

        Using proxy As New IFM.JsonProxyClient.ProxyClient(ConfigurationManager.AppSettings("IFMDataServices_EndPointBaseUrl"))
            Dim response = proxy.Post("OnBase/Document/Upload", package)
            Dim responseText = proxy.GetResponsePayload(response)
            If (response.StatusCode = Net.HttpStatusCode.OK) Then
                Dim documentId As Int64 = 0
                Int64.TryParse(responseText, documentId)
                Return documentId
            End If
            Return 0
        End Using
    End Function

    ''' <summary>
    ''' Inserts documents into Acrosoft from an FNOL record
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertFNOLDocumentsIntoOnBaseFromFNOLRecord(ByRef FNOL_Id As String, ByRef err As String) As Boolean
        Dim FNOLRec As DataRow = Nothing
        Dim FNOLDetails As New FNOLData_Structure()
        Dim terr As String = Nothing
        Dim FNOLDocs As DataTable = Nothing
        Dim fn As String = Nothing
        Dim docTitle As String = Nothing
        Dim SubFolder As String = Nothing
        Dim filebytes() As Byte = Nothing
        Dim oFs As FileStream = Nothing

        Dim OnBaseStep As String = String.Empty

        Try
            ' Get the FNOL record
            OnBaseStep = "Get FNOL Record"
            FNOLRec = GetFNOLRecordById(FNOL_Id, err)
            If FNOLRec Is Nothing Then
                If err IsNot Nothing AndAlso err <> String.Empty Then
                    Throw New Exception(err)
                Else
                    Throw New Exception("Unknown error getting FNOL Record")
                End If
            End If

            ' Load up the FNOL Details structure for the create
            OnBaseStep = "Load FNOL Details Structure"
            Dim ft As String = GetFNOLTypeName(FNOLRec("FNOLType_Id").ToString())
            FNOLDetails.LOB = ft.ToUpper.Trim

            If Not IsDBNull(FNOLRec("AdjusterName")) Then FNOLDetails.AdjusterName = FNOLRec("AdjusterName").ToString()
            If Not IsDBNull(FNOLRec("AgencyName")) Then FNOLDetails.AgencyName = FNOLRec("AgencyName").ToString()
            If Not IsDBNull(FNOLRec("EntryDate")) Then FNOLDetails.ClaimDate = FNOLRec("EntryDate").ToString()
            If Not IsDBNull(FNOLRec("LossDescription")) Then FNOLDetails.Description = FNOLRec("LossDescription").ToString()
            If Not IsDBNull(FNOLRec("DiamondClaimNumber")) Then FNOLDetails.DiamondClaimNumber = FNOLRec("DiamondClaimNumber").ToString()
            If Not IsDBNull(FNOLRec("InsuredCity")) Then FNOLDetails.InsuredAddressCity = FNOLRec("InsuredCity").ToString()
            If Not IsDBNull(FNOLRec("InsuredState")) Then FNOLDetails.InsuredAddressState = FNOLRec("InsuredState").ToString()
            If Not IsDBNull(FNOLRec("InsuredAddress")) Then FNOLDetails.InsuredAddressStreet = FNOLRec("InsuredAddress").ToString()
            If Not IsDBNull(FNOLRec("InsuredZip")) Then FNOLDetails.InsuredAddressZip = FNOLRec("InsuredZip").ToString()
            If Not IsDBNull(FNOLRec("InsuredFirst")) Then FNOLDetails.InsuredFirst = FNOLRec("InsuredFirst").ToString()
            If Not IsDBNull(FNOLRec("InsuredLast")) Then FNOLDetails.InsuredLast = FNOLRec("InsuredLast").ToString()
            'FNOLInfo.LOB = "" ' Not really needed
            If Not IsDBNull(FNOLRec("LossDate")) Then FNOLDetails.LossDate = FNOLRec("LossDate").ToString()
            If Not IsDBNull(FNOLRec("InsuredBusinessPhone")) Then FNOLDetails.PhoneBusiness = FNOLRec("InsuredBusinessPhone").ToString()
            If Not IsDBNull(FNOLRec("InsuredCellPhone")) Then FNOLDetails.PhoneCell = FNOLRec("InsuredCellPhone").ToString()
            If Not IsDBNull(FNOLRec("InsuredHomePhone")) Then FNOLDetails.PhoneHome = FNOLRec("InsuredHomePhone").ToString()
            If Not IsDBNull(FNOLRec("PrimaryContact")) Then FNOLDetails.PrimaryContact = FNOLRec("PrimaryContact").ToString()
            If Not IsDBNull(FNOLRec("PolicyNumber")) Then
                FNOLDetails.PolicyNumber = FNOLRec("PolicyNumber").ToString()
                terr = GetPolicyTerritoryNumber(FNOLRec("PolicyNumber").ToString(), err)
                If terr Is Nothing Then
                    If err IsNot Nothing AndAlso err <> String.Empty Then
                        Throw New Exception(err)
                    Else
                        Throw New Exception("Error getting territory number")
                    End If
                End If
                FNOLDetails.TerritoryNumber = terr
            Else
                Throw New Exception("Policy Number is empty!")
            End If

            ' Add CAT 11/18/15 MGB
            If CBool(FNOLRec("CAT")) Then
                FNOLDetails.CAT = True
            Else
                FNOLDetails.CAT = False
            End If

            ' Insert the documents into the folder if there are any
            OnBaseStep = "Insert Documents"
            err = Nothing
            FNOLDocs = GetFNOLDocuments(FNOL_Id, err)
            If FNOLDocs Is Nothing OrElse FNOLDocs.Rows.Count <= 0 Then
                If err <> Nothing AndAlso err <> String.Empty Then
                    ' Error getting the documents
                    Throw New Exception(err)
                Else
                    ' No Documents found don't do anything
                End If
            Else
                ' Documents found, insert them into onbase
                For Each doc As DataRow In FNOLDocs.Rows
                    docTitle = doc("DocumentName").ToString()
                    filebytes = doc("DocumentImage")
                    UploadToOnbase(filebytes, docTitle, FNOLDetails.DiamondClaimNumber)
                Next
            End If

            ' Add a copy of the FNOL Email to the documents folder MGB 5/25/16
            OnBaseStep = "Add FNOL Copy"
            Dim FNOLEmail As String = GetFNOLEmailBody(FNOL_Id, err)
            If FNOLEmail IsNot Nothing AndAlso FNOLEmail <> String.Empty Then
                docTitle = "FNOLDetails_" & FNOLDetails.PolicyNumber & "_" & FNOLDetails.DiamondClaimNumber & ".html"
                filebytes = System.Text.Encoding.Unicode.GetBytes(FNOLEmail)
                UploadToOnbase(filebytes, docTitle, FNOLDetails.DiamondClaimNumber)
            Else
                If err IsNot Nothing Then
                    Throw New Exception("GetEmailBody failed: " & err)
                Else
                    Throw New Exception("GetEmailBody failed.")
                End If
            End If

            Return True
        Catch ex As Exception
            err = ex.Message & " (step: " & OnBaseStep & ")"
            HandleError(ClassName, "InsertFNOLDocumentsIntoAcrosoftFromFNOLRecord (step: " & OnBaseStep & ")", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Sorts the items in the passed listbox
    ''' </summary>
    ''' <param name="lb1"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SortListBox(ByRef lb1 As ListBox, ByRef err As String) As Boolean
        Dim t As New List(Of ListItem)()
        Dim compare As New Comparison(Of ListItem)(AddressOf CompareListItems)
        Try
            For Each lbItem As ListItem In lb1.Items
                t.Add(lbItem)
            Next

            t.Sort(compare)
            lb1.Items.Clear()
            lb1.Items.AddRange(t.ToArray())

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "SortListBox", ex)
            Return False
        End Try
    End Function

    Private Function CompareListItems(ByVal li1 As ListItem, ByVal li2 As ListItem) As Integer
        Return [String].Compare(li1.Text, li2.Text)
    End Function

    ''' <summary>
    ''' Unassigns all or a single claim
    ''' 
    ''' If no FNOL_Id is passed, all claims will be unassigned
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="FNOL_Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UnAssignClaim(ByRef err As String, Optional ByVal FNOL_Id As String = Nothing) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1

        Try
            conn = New SqlConnection(strConnFNOL)
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            If FNOL_Id IsNot Nothing Then
                cmd.CommandText = "UPDATE tbl_FNOL SET Assigned = 0 WHERE FNOL_Id = " & FNOL_Id
            Else
                cmd.CommandText = "UPDATE tbl_FNOL SET Assigned = 0"
            End If
            rtn = cmd.ExecuteNonQuery()

            If rtn = 1 Then Return True Else Throw New Exception("tbl_FNOL Update failed")
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "UnAssignClaim", ex)
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates the Comments field in the TBL_FNOL table
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="CommentsText"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateFNOLComments(ByVal FNOL_Id As String, ByVal CommentsText As String, ByRef err As String, Optional ByVal SetToNull As Boolean = False) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim sql As String = ""

        Try
            If SetToNull Then
                sql = "UPDATE TBL_FNOL SET Comments = NULL WHERE FNOL_Id = " & FNOL_Id
            Else
                sql = "UPDATE TBL_FNOL SET Comments = '" & CommentsText & "' WHERE FNOL_Id = " & FNOL_Id
            End If

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            rtn = cmd.ExecuteNonQuery()
            If rtn <> 1 Then Throw New Exception("Record not updated!")

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "UpdateFNOLComments", ex)
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pass in a semicolon delimited email list and this function will return a list (of  string) of emails from the list.
    ''' </summary>
    ''' <param name="DelimitedEmailList"></param>
    ''' <returns></returns>
    Public Function ParseEmailList(ByVal DelimitedEmailList As String) As List(Of String)
        Dim splitemails() As String = DelimitedEmailList.Split(";")
        Dim emailList As New List(Of String)
        If splitemails IsNot Nothing AndAlso splitemails.Count > 0 Then
            For Each s As String In splitemails
                If IsValidEmail(s) Then emailList.Add(s)
            Next
            If emailList.Count > 0 Then Return emailList Else Return Nothing
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Sends an email to the Adjuster on claim assignment
    ''' If a CAT claim, CC's the *Claims_Admin_Group MGB 3-2-2016
    ''' </summary>
    ''' <param name="AdjusterEmailAddress"></param>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="DiamondClaimNumber"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function SendAssignmentEmail(ByVal AdjusterEmailAddress As String, FNOL_Id As String, ByVal DiamondClaimNumber As String, ByRef err As String) As Boolean
        Dim EmailInfo As New EmailInfo_Structure_FNOLCA()
        Dim atts As New DataTable()
        Dim ListOfFiles As List(Of String) = Nothing
        Dim CAT As Boolean = False
        Dim FNOLRec As DataRow = Nothing
        Dim AgencyEmail As String = ""

        Try
            FNOLRec = GetFNOLRecordById(FNOL_Id, err)
            If FNOLRec Is Nothing Then Throw New Exception("Error getting FNOL Record: " & err)
            CAT = CBool(FNOLRec("CAT"))

            EmailInfo.Body = GetFNOLEmailBody(FNOL_Id, err)
            If EmailInfo.Body Is Nothing Then Throw New Exception(err)

            If AppSettings("TestOrProd").ToUpper() = "PROD" Then
                EmailInfo.ToAddress = AdjusterEmailAddress
                ' CC CSU Group if CAT loss
                If CAT Then EmailInfo.CCAddress = AppSettings("FNOLCA_CSU_Email")
            Else
                ' CC CSU Group if CAT loss - in test we just cc the LossReportingEmailAddress
                EmailInfo.ToAddress = AppSettings("FNOLClaimAssign_LossReportingEmailAddress")
                If CAT Then EmailInfo.CCAddress = AppSettings("FNOLClaimAssign_LossReportingEmailAddress")
            End If

            'AgencyEmail = FNOLRec("ConfirmEmailAddress").ToString()

            ' Send a copy of the adjuster email to the FNOLSubmissions@IndianaFarmers.com
            If AppSettings("FNOLCA_SendCopyOfAssignmentEmail") IsNot Nothing Then
                If AppSettings("FNOLCA_SendCopyOfAssignmentEmail").ToUpper() = "TRUE" Then
                    If Not IsDBNull(FNOLRec("ConfirmEmailAddress")) Then
                        EmailInfo.CCAddress = AppSettings("FNOLCA_CopyOfAssignment_EmailAddress") & ";" & FNOLRec("ConfirmEmailAddress").ToString
                    Else
                        EmailInfo.CCAddress = AppSettings("FNOLCA_CopyOfAssignment_EmailAddress")
                    End If
                End If
            End If

            EmailInfo.SubjectLine_OPTIONAL = "CLAIM ASSIGNMENT - " & FNOLRec("PolicyNumber").ToString() & " - " & DiamondClaimNumber

            ' Attachments - Save each one to the temp folder, attach, then delete 
            atts = GetFNOLDocuments(FNOL_Id, err)

            If atts IsNot Nothing AndAlso atts.Rows.Count > 0 Then
                ' Create the temp files and add them to the file list
                ListOfFiles = New List(Of String)
                For Each dr As DataRow In atts.Rows
                    Dim filepath As String = AppSettings("DECFolder") & dr("DocumentName").ToString
                    Dim fileimage As Byte() = CType(dr("DocumentImage"), Byte())
                    If File.Exists(filepath) Then File.Delete(filepath)
                    Dim fs As New FileStream(filepath, FileMode.Create)
                    Using bw As New BinaryWriter(fs)
                        bw.Write(fileimage)
                        bw.Flush()
                    End Using
                    fs.Close()
                    ListOfFiles.Add(filepath)
                Next
            End If

            ' Send the assignment email to the adjuster
            If Not SendEmail(EmailInfo, err, ListOfFiles) Then
                Throw New Exception("Error sending assignment email: " & err)
            End If

            ' If the CAT email is turned on and this is a CAT claim, send a copy of the assignment email to the CAT handling company
            ' MGB 12/17/2019
            If AppSettings("TestOrProd").ToUpper = "PROD" AndAlso CAT Then
                If AppSettings("FNOLCA_EnableCATThirdPartyEmails") IsNot Nothing AndAlso AppSettings("FNOLCA_EnableCATThirdPartyEmails").ToUpper = "TRUE" Then
                    Dim CompanyRec As DataRow = GetCATCompanyRecord(FNOLRec("tbl_FNOL_CATCompany_Id"))
                    If CompanyRec IsNot Nothing Then
                        Dim CompanyName As String = CompanyRec("CompanyName").ToString
                        Dim CompanyEmail As String = CompanyRec("ClaimAssignmentNotificationEmailAddress")

                        EmailInfo.ToAddress = CompanyEmail
                        EmailInfo.SubjectLine_OPTIONAL = "INDIANA FARMERS MUTUAL INSURANCE NOTICE OF CLAIM ASSIGNMENT - " & FNOLRec("PolicyNumber").ToString() & " - " & DiamondClaimNumber & " (" & CompanyName.ToUpper & ")"

                        If EmailInfo.ToAddress IsNot Nothing Then
                            If Not SendEmail(EmailInfo, err, ListOfFiles) Then
                                Throw New Exception("Error sending CAT email to " & CompanyName & ": " & err)
                            End If
                        End If
                    Else
                        Throw New Exception("There was an error trying to send the CAT Claim email to the CAT company: Company record does not exist.")
                    End If
                End If
            End If

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "SendAssignmentEmail", ex)
            Return False
        Finally
            If ListOfFiles IsNot Nothing Then DeleteOldClaimFiles(ListOfFiles)
        End Try
    End Function

    ''' <summary>
    ''' If any errors or warnings were generated during the claim assignment process, send the claims admin an email message
    ''' so that the issues can be manually corrected.
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="DiamondClaimNumber"></param>
    ''' <param name="WarningMessages"></param>
    ''' <param name="ErrorMessages"></param>
    ''' <returns></returns>
    Public Function SendClaimsAdminWarningAndErrorEmail(ByVal FNOL_Id As String, ByVal DiamondClaimNumber As String, ByVal WarningMessages As List(Of String), ByVal ErrorMessages As List(Of String), ByRef err As String) As Boolean
        'Dim EmailInfo As New EmailInfo_Structure_FNOLCA()
        'Dim atts As New DataTable()
        'Dim ListOfFiles As List(Of String) = Nothing
        'Dim CAT As Boolean = False
        'Dim FNOLRec As DataRow = Nothing
        'Dim AgencyEmail As String = ""

        'Try
        '    FNOLRec = GetFNOLRecordById(FNOL_Id, err)
        '    If FNOLRec Is Nothing Then Throw New Exception("Error getting FNOL Record: " & err)
        '    CAT = CBool(FNOLRec("CAT"))

        '    EmailInfo.Body = GetFNOLEmailBody(FNOL_Id, err, ErrorMessages, WarningMessages)
        '    If EmailInfo.Body Is Nothing Then Throw New Exception(err)

        '    ' The admin email address should be the LssReportingErrorEmail address
        '    If AppSettings("LossReportingErrorEmail") IsNot Nothing Then
        '        EmailInfo.ToAddress = AppSettings("LossReportingErrorEmail")
        '    Else
        '        Throw New Exception("Config key 'LossReportingErrorEmail' is missing!!")
        '    End If

        '    EmailInfo.SubjectLine_OPTIONAL = "*** ERRORS OR WARNINGS ON CLAIM ASSIGNMENT - " & FNOLRec("PolicyNumber").ToString() & " - " & DiamondClaimNumber

        '    ' Attachments - Save each one to the temp folder, attach, then delete 
        '    atts = GetFNOLDocuments(FNOL_Id, err)

        '    If atts IsNot Nothing AndAlso atts.Rows.Count > 0 Then
        '        ' Create the temp files and add them to the file list
        '        ListOfFiles = New List(Of String)
        '        For Each dr As DataRow In atts.Rows
        '            Dim filepath As String = AppSettings("DECFolder") & dr("DocumentName").ToString
        '            Dim fileimage As Byte() = CType(dr("DocumentImage"), Byte())
        '            If File.Exists(filepath) Then File.Delete(filepath)
        '            Dim fs As New FileStream(filepath, FileMode.Create)
        '            Using bw As New BinaryWriter(fs)
        '                bw.Write(fileimage)
        '                bw.Flush()
        '            End Using
        '            fs.Close()
        '            ListOfFiles.Add(filepath)
        '        Next
        '    End If

        '    ' Send the email to the admin
        '    If Not SendEmail(EmailInfo, err, ListOfFiles) Then
        '        Throw New Exception("Error sending assignment email: " & err)
        '    End If

        '    Return True
        'Catch ex As Exception
        '    err = ex.Message
        '    HandleError(ClassName, "SendClaimsAdminWarningAndErrorEmail", ex)
        '    Return False
        'Finally
        '    If ListOfFiles IsNot Nothing Then DeleteOldClaimFiles(ListOfFiles)
        'End Try
        Return SendClaimsAdminWarningAndErrorEmail_OptionallyDeleteAttachmentsAfter(FNOL_Id, DiamondClaimNumber, WarningMessages, ErrorMessages, err)
    End Function
    Public Function SendClaimsAdminWarningAndErrorEmail_OptionallyDeleteAttachmentsAfter(ByVal FNOL_Id As String, ByVal DiamondClaimNumber As String, ByVal WarningMessages As List(Of String), ByVal ErrorMessages As List(Of String), ByRef err As String, Optional ByVal deleteAttachmentsAfter As Boolean = True) As Boolean
        Dim EmailInfo As New EmailInfo_Structure_FNOLCA()
        Dim atts As New DataTable()
        Dim ListOfFiles As List(Of String) = Nothing
        Dim CAT As Boolean = False
        Dim FNOLRec As DataRow = Nothing
        Dim AgencyEmail As String = ""

        Try
            FNOLRec = GetFNOLRecordById(FNOL_Id, err)
            If FNOLRec Is Nothing Then Throw New Exception("Error getting FNOL Record: " & err)
            CAT = CBool(FNOLRec("CAT"))

            EmailInfo.Body = GetFNOLEmailBody(FNOL_Id, err, ErrorMessages, WarningMessages)
            If EmailInfo.Body Is Nothing Then Throw New Exception(err)

            ' The admin email address should be the LssReportingErrorEmail address
            If AppSettings("LossReportingErrorEmail") IsNot Nothing Then
                EmailInfo.ToAddress = AppSettings("LossReportingErrorEmail")
            Else
                Throw New Exception("Config key 'LossReportingErrorEmail' is missing!!")
            End If

            EmailInfo.SubjectLine_OPTIONAL = "*** ERRORS OR WARNINGS ON CLAIM ASSIGNMENT - " & FNOLRec("PolicyNumber").ToString() & " - " & DiamondClaimNumber

            ' Attachments - Save each one to the temp folder, attach, then delete 
            atts = GetFNOLDocuments(FNOL_Id, err)

            If atts IsNot Nothing AndAlso atts.Rows.Count > 0 Then
                ' Create the temp files and add them to the file list
                ListOfFiles = New List(Of String)
                For Each dr As DataRow In atts.Rows
                    Dim filepath As String = AppSettings("DECFolder") & dr("DocumentName").ToString
                    Dim fileimage As Byte() = CType(dr("DocumentImage"), Byte())
                    If File.Exists(filepath) Then File.Delete(filepath)
                    Dim fs As New FileStream(filepath, FileMode.Create)
                    Using bw As New BinaryWriter(fs)
                        bw.Write(fileimage)
                        bw.Flush()
                    End Using
                    fs.Close()
                    ListOfFiles.Add(filepath)
                Next
            End If

            ' Send the email to the admin
            If Not SendEmail(EmailInfo, err, ListOfFiles) Then
                Throw New Exception("Error sending assignment email: " & err)
            End If

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "SendClaimsAdminWarningAndErrorEmail", ex)
            Return False
        Finally
            'If ListOfFiles IsNot Nothing Then DeleteOldClaimFiles(ListOfFiles)
            If ListOfFiles IsNot Nothing Then
                Dim attList As List(Of String) = Nothing
                If deleteAttachmentsAfter = True Then
                    attList = ListOfFiles
                End If
                DeleteOldClaimFiles(attList)
            End If
        End Try
    End Function

    ''' <summary>
    ''' Sends an email when a claim is removed.  
    ''' Sends the email to the email addresses in the FNOLCA_RemovedClaimsEmailNotificationList config file key
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function SendRemovedClaimEmail(FNOL_Id As String, ByRef err As String) As Boolean
        Dim EmailInfo As New EmailInfo_Structure_FNOLCA()
        Dim atts As New DataTable()
        Dim ListOfFiles As List(Of String) = Nothing
        Dim CAT As Boolean = False
        Dim FNOLRec As DataRow = Nothing
        Dim DiamondClaimNumber As String = Nothing

        Try
            FNOLRec = GetFNOLRecordById(FNOL_Id, err)
            If FNOLRec Is Nothing Then Throw New Exception("Error getting FNOL Record: " & err)
            CAT = CBool(FNOLRec("CAT"))
            DiamondClaimNumber = FNOLRec("DiamondClaimNumber").ToString()

            EmailInfo.Body = GetFNOLEmailBody(FNOL_Id, err)
            If EmailInfo.Body Is Nothing Then Throw New Exception(err)

            If AppSettings("FNOLCA_RemovedClaimsEmailNotificationList") Is Nothing Then Throw New Exception("The FNOLCA_RemovedClaimsEmailNotificationList config file key is missing!")
            EmailInfo.ToAddress = AppSettings("FNOLCA_RemovedClaimsEmailNotificationList")

            EmailInfo.SubjectLine_OPTIONAL = "REMOVED CLAIM - " & FNOLRec("PolicyNumber").ToString() & " - " & DiamondClaimNumber

            ' Attachments - Save each one to the temp folder, attach, then delete 
            atts = GetFNOLDocuments(FNOL_Id, err)

            If atts IsNot Nothing AndAlso atts.Rows.Count > 0 Then
                ' Create the temp files and add them to the file list
                ListOfFiles = New List(Of String)
                For Each dr As DataRow In atts.Rows
                    Dim filepath As String = AppSettings("DECFolder") & dr("DocumentName").ToString
                    Dim fileimage As Byte() = CType(dr("DocumentImage"), Byte())
                    If File.Exists(filepath) Then File.Delete(filepath)
                    Dim fs As New FileStream(filepath, FileMode.Create)
                    Using bw As New BinaryWriter(fs)
                        bw.Write(fileimage)
                        bw.Flush()
                    End Using
                    fs.Close()
                    ListOfFiles.Add(filepath)
                Next
            End If

            If Not SendEmail(EmailInfo, err, ListOfFiles) Then
                Throw New Exception("Error sending removed claim email: " & err)
            End If

            DeleteOldClaimFiles(ListOfFiles)

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "SendRemovedClaimEmail", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Deletes:
    ''' - Files in the DECFolder 24 hours old or older
    ''' - Any files passed in the ListOfFiles list
    ''' 
    ''' !! WILL NOT FAIL WHEN A DELETE FAILS ON AN INDIVIDUAL FILE !!
    ''' </summary>
    ''' <param name="ListOfFiles"></param>
    ''' <remarks></remarks>
    Public Function DeleteOldClaimFiles(Optional ByVal ListOfFiles As List(Of String) = Nothing) As Boolean
        Try
            ' Delete files 24 hours old or older
            Dim dirfiles() As String = Directory.GetFiles(AppSettings("DECFolder"))
            If dirfiles IsNot Nothing AndAlso dirfiles.Count > 0 Then
                For Each s As String In dirfiles
                    Dim fi As New FileInfo(s)
                    If fi.CreationTime < Date.Now.AddDays(-1) Then
                        Try
                            File.Delete(s)
                        Catch ex As Exception

                        End Try
                    End If
                Next
            End If

            ' Delete all of the files in the  passed list
            If ListOfFiles IsNot Nothing AndAlso ListOfFiles.Count > 0 Then
                For Each f As String In ListOfFiles
                    Try
                        System.IO.File.Delete(f)
                    Catch ex As Exception

                    End Try
                Next
            End If

            Return True
        Catch ex As Exception
            HandleError(ClassName, "DeleteOldClaimFiles", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Looks up the adjuster email address in Diamond
    ''' </summary>
    ''' <param name="Adjuster_Claimpersonnel_ID"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function GetAdjusterEmailAddress(ByVal Adjuster_Claimpersonnel_ID As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            ' MGB 8-20-2019 don't bother performing the check if the passed id is invalid.  Added this logic to avoid unnecessary errors in the Error Log.
            If Adjuster_Claimpersonnel_ID Is Nothing OrElse Adjuster_Claimpersonnel_ID.Trim = String.Empty OrElse (Not IsNumeric(Adjuster_Claimpersonnel_ID)) Then Return Nothing

            conn.ConnectionString = strConnDiamond
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT US.user_emailaddr FROM ClaimPersonnel CP JOIN users US ON US.Users_id = CP.users_id WHERE CP.claimpersonnel_id = " & Adjuster_Claimpersonnel_ID
            rtn = cmd.ExecuteScalar()
            'If rtn Is Nothing Then Throw New Exception("Adjuster Email Address not found!")
            'If rtn.ToString.Trim = String.Empty Then Throw New Exception("Adjuster does not have an email address set in Diamond.")
            'If rtn Is Nothing Then err = "Adjuster Email Address not found!"
            'If rtn IsNot Nothing AndAlso rtn.ToString.Trim = String.Empty Then err = "Adjuster does not have an email address set in Diamond."
            If rtn Is Nothing Then
                err = "Adjuster Email Address not found"
                If String.IsNullOrWhiteSpace(Adjuster_Claimpersonnel_ID) = False Then
                    err &= " for Adjuster_Claimpersonnel_ID " & Adjuster_Claimpersonnel_ID
                End If
                err &= "!"
            ElseIf rtn.ToString.Trim = String.Empty Then
                err = "Adjuster"
                If String.IsNullOrWhiteSpace(Adjuster_Claimpersonnel_ID) = False Then
                    err &= " (Adjuster_Claimpersonnel_ID " & Adjuster_Claimpersonnel_ID & ")"
                End If
                err &= " does not have an email address set in Diamond."
            End If
            If String.IsNullOrWhiteSpace(err) = False Then
                Return Nothing
            End If

            Return rtn.ToString()
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetAdjusterEmailAddress", ex)
            Return Nothing
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Public Sub GetAdjusterEmailAddressAndPhone(ByVal Adjuster_Claimpersonnel_ID As String, ByRef email As String, ByRef phone As String, ByRef err As String)
        email = ""
        phone = ""
        Dim chc As New CommonHelperClass
        If chc.IsPositiveIntegerString(Adjuster_Claimpersonnel_ID) = True Then
            Using sso As New SQLselectObject(chc.ConfigurationAppSettingValueAsString("connDiamond"))
                With sso
                    .queryOrStoredProc = "SELECT U.user_emailaddr, firstPhone.phone_num"
                    .queryOrStoredProc &= " FROM ClaimPersonnel as CP WITH (NOLOCK)"
                    .queryOrStoredProc &= " LEFT JOIN Users as U WITH (NOLOCK) on U.users_id = CP.users_id"
                    .queryOrStoredProc &= " LEFT JOIN UserEmployeeLink as UEL WITH (NOLOCK) on UEL.users_id = U.users_id"
                    .queryOrStoredProc &= " LEFT JOIN Employee as E WITH (NOLOCK) on E.employee_id = UEL.employee_id"
                    '.queryOrStoredProc &= " LEFT JOIN Name as N WITH (NOLOCK) on N.name_id = E.name_id"
                    .queryOrStoredProc &= " OUTER APPLY (SELECT TOP 1 P.phone_num from EmployeePhoneLink as PL WITH (NOLOCK)"
                    .queryOrStoredProc &= " INNER JOIN Phone as P WITH (NOLOCK) on P.phone_id = PL.phone_id"
                    .queryOrStoredProc &= " WHERE PL.employee_id = E.employee_id and P.phone_num is not null and P.phone_num <> ''"
                    .queryOrStoredProc &= " ORDER BY PL.phone_id) as firstPhone"
                    .queryOrStoredProc &= " WHERE CP.claimpersonnel_id = " & CInt(Adjuster_Claimpersonnel_ID).ToString
                    Using dr As SqlClient.SqlDataReader = .GetDataReader
                        If dr IsNot Nothing AndAlso dr.HasRows = True Then
                            With dr
                                .Read
                                email = .Item("user_emailaddr").ToString.Trim
                                phone = .Item("phone_num").ToString.Trim
                            End With
                        ElseIf .hasError = True AndAlso String.IsNullOrWhiteSpace(.errorMsg) = False Then
                            'could capture db error message if needed
                        End If
                    End Using
                End With
            End Using
        End If            
    End Sub

    ''' <summary>
    ''' Formats the email body using the data in the Assigned FNOL record.
    ''' 1/5/2021 - Added warning and error sections and their optional parameters.  If ErrorMessages or WarningMessages have 
    '''            any contents then the appropriate sections will be printed.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFNOLEmailBody(ByVal FNOL_Id As String, ByRef err As String, Optional ByVal ErrorMessages As List(Of String) = Nothing, Optional ByVal WarningMessages As List(Of String) = Nothing) As String
        Dim strBody As String = ""
        Dim ndx As Integer = -1
        Dim str As String = Nothing
        Dim FNOLType As String = Nothing
        Dim PersonsTable As DataTable = Nothing
        Dim ct As String = "NO"
        Dim trackedEventName As String = Nothing

        Try
            ' Get the FNOL record
            Dim FNOLRec As DataRow = GetFNOLRecordById(FNOL_Id, err)
            If FNOLRec Is Nothing Then Throw New Exception(err)

            ' Get the FNOL Type
            FNOLType = GetFNOLTypeName(FNOLRec("FNOLType_Id").ToString).ToUpper
            If FNOLType Is Nothing OrElse FNOLType = String.Empty Then Throw New Exception("Error getting FNOL Type!")

            ' Get the FNOL tracked event name
            trackedEventName = GetFNOLTrackedEventName(FNOLRec("tbl_FNOL_CATCompany_Id").ToString).Trim
            ' Build the email body HTML

            strBody = strBody & "<html><head><style type='text/css'>.headline{font-family:Verdana;font-size:medium;font-weight:bold;}.subheadline{font-family:Verdana;font-size:small;font-weight:bold;}.normaltext{font-family:Verdana;font-size:smaller;}a:link {color: #093F70;text-decoration:none;font-size: 12px; font-family: Verdana;font-weight:bold;}a:visited {color: #093F70;text-decoration: none; font-size: 12px; font-family: Verdana;font-weight:bold;}a:hover {color: #990000;text-decoration: none; font-size: 12px; font-family: Verdana;font-weight:bold;}a:active {color: #093F70;text-decoration: none; font-size: 12px; font-family: Verdana;font-weight:bold;}</style></head><body><form><table align='center' width='600' id='tblLoss' cellpadding='4' border='1' style='border-color:inherit'>" & vbCrLf

            ' ERRORS AND WARNINGS
            ' Errors
            If ErrorMessages IsNot Nothing AndAlso ErrorMessages.Count > 0 Then
                strBody += "<tr>" & vbCrLf
                strBody += "<td class='subheadline' colspan='2' align='left' style='textdecoration=underline;'>ERRORS:</td>" & vbCrLf
                strBody += "</tr>" & vbCrLf
                For Each errmsg As String In ErrorMessages
                    strBody += "<tr>" & vbCrLf
                    strBody += "<td colspan=2 class='normaltext' align='left' style='Vertical-Align:top;font-weight:700;color:red;'>" & "* " & errmsg & "</td>" & vbCrLf
                    strBody += "</tr>" & vbCrLf
                Next
                strBody += "<tr><td colspan=2>&nbsp;</td></tr>" & vbCrLf
            End If
            ' Warnings
            If WarningMessages IsNot Nothing AndAlso WarningMessages.Count > 0 Then
                strBody += "<tr>" & vbCrLf
                strBody += "<td class='subheadline' colspan='2' align='left' style='textdecoration=underline;'>WARNINGS:</td>" & vbCrLf
                strBody += "</tr>" & vbCrLf
                For Each wmsg As String In WarningMessages
                    strBody += "<tr>" & vbCrLf
                    strBody += "<td colspan=2 class='normaltext' align='left' style='Vertical-Align:top;font-weight:700;color:red;'>" & "* " & wmsg & "</td>" & vbCrLf
                    strBody += "</tr>" & vbCrLf
                Next
                strBody += "<tr><td colspan=2>&nbsp;</td></tr>" & vbCrLf
            End If

            ' COMMENTS SECTION 5-24-16 MGB
            ' UPDATED PER TASK 80232 03/28/23
            If (Not IsDBNull(FNOLRec("CAT")) AndAlso CBool(FNOLRec("CAT"))) OrElse DataFieldHasValue(FNOLRec, "Comments") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Notes To Adjuster</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
                If Not IsDBNull(FNOLRec("CAT")) AndAlso CBool(FNOLRec("CAT")) AndAlso  trackedEventName <> "None"Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right' style='Vertical-Align:top;font-weight:700;color:red;'>Tracked Event Provider</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left' style='font-weight:700;color:red;'>" & trackedEventName & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                Else
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Tracked Event Provider</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & trackedEventName & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                If DataFieldHasValue(FNOLRec, "Comments") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right' style='Vertical-Align:top;font-weight:700;color:red;'>Comments</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left' style='font-weight:700;color:red;'>" & ScrubText(FNOLRec("Comments").ToString) & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                Else
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Comments</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & "" & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' General Information - All this data should be there, it's required
            strBody = strBody & "<tr>" & vbCrLf
            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Basic Information</td>" & vbCrLf
            strBody = strBody & "</tr>" & vbCrLf

            ' Only show Diamond Claim number if we successfully generated one
            If DataFieldHasValue(FNOLRec, "DiamondClaimNumber") Then
                strBody = strBody & "<tr><td class='normaltext' width='50%' align='right'>Diamond Claim Number</td><td class='normaltext' width='50%' align='left'>" & FNOLRec("diamondClaimNumber").ToString & "</td></tr>"
            End If

            If Not IsDBNull(FNOLRec("CAT")) AndAlso CBool(FNOLRec("CAT")) Then
                ' If CAT, make the text RED BOLD
                ct = "YES"
                strBody = strBody & "<tr style=color:red;font-weight:700;>"
            Else
                ct = "NO"
                strBody = strBody & "<tr>" & vbCrLf
            End If
            strBody = strBody & "<td class='normaltext' width='50%' align='right'>CAT</td>" & vbCrLf
            strBody = strBody & "<td class='normaltext' width='50%' align='left'>" & ct & "</td>" & vbCrLf
            strBody = strBody & "</tr>" & vbCrLf

            If DataFieldHasValue(FNOLRec, "EntryDate") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' width='50%' align='right'>Date</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' width='50%' align='left'>" & CDate(FNOLRec("EntryDate")).ToShortDateString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "PolicyNumber") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Policy Number</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PolicyNumber").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossDate") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss Date</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossDate").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "AdjusterName") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Assigned Adjuster</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("AdjusterName").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf

                Dim email As String = ""
                Dim phone As String = ""
                err = ""
                Dim claimadjid As String = Nothing
                claimadjid = GetAdjusterClaimpersonnel_ID_ById(FNOLRec("FNOLClaimAssignAdjuster_Id").ToString, err)
                If claimadjid IsNot Nothing Then
               
                GetAdjusterEmailAddressAndPhone(claimadjId, email, phone, err)
                If String.IsNullOrWhiteSpace(phone) = False Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Adjuster’s Phone Number</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & phone & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                If String.IsNullOrWhiteSpace(email) = False Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Adjuster’s Email</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & email & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                     End If
            End If


            If DataFieldHasValue(FNOLRec, "AssignedBy") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Assigned By</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("AssignedBy").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "DateAssignedToAdjuster") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Date Assigned</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("DateAssignedToAdjuster").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "AgencyName") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Agency Name</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("AgencyName").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "AgencyPhone") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Agency Phone</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("AgencyPhone").ToString() & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "AgencyFAX") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Agency Fax</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("AgencyFAX").ToString() & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            ' Auto Deductibles
            If DataFieldHasValue(FNOLRec, "CompDed") Then
                strBody = strBody & "<tr id='CompDeductRow'>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Comprehensive Deductible</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("CompDed").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "CollDed") Then
                strBody = strBody & "<tr id='CollDeductRow'>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Collision Deductible</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("CollDed").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "RentDed") Then
                strBody = strBody & "<tr id='RentDeductRow'>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Rental Deductible</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("RentDed").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "DeductibleAmount") Then
                strBody = strBody & "<tr id='PropGenDeductRow'>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Deductible Amount</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("DeductibleAmount").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            ' Package Part
            If DataFieldHasValue(FNOLRec, "PackagePart") Then
                strBody = strBody & "<tr id='PackageRow'>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Package Part</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PackagePart").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            ' Insured Info
            strBody = strBody & "<tr>" & vbCrLf
            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Insured Information</td>" & vbCrLf
            strBody = strBody & "</tr>" & vbCrLf

            If DataFieldHasValue(FNOLRec, "PrimaryContact") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Primary Contact</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PrimaryContact").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredFirst") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured First</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredFirst").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredLast") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Last</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredLast").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredHomePhone") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Home Phone</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredHomePhone").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredBusinessPhone") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Business Phone</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredBusinessPhone").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredCellPhone") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Cell Phone</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredCellPhone").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredFAX") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured FAX</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredFAX").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredEmail") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Email</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredEmail").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredAddress") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Address</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredAddress").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredCity") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured City</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredCity").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredState") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured State</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredState").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "InsuredZip") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Insured Zip</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsuredZip").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            ' Contact Info
            If DataFieldHasValue(FNOLRec, "ContactHomePhone") _
                OrElse DataFieldHasValue(FNOLRec, "ContactBusinessPhone") _
                OrElse DataFieldHasValue(FNOLRec, "ContactCellPhone") _
                OrElse DataFieldHasValue(FNOLRec, "ContactTime") Then

                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Contact (If different than Insured)</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf

                If DataFieldHasValue(FNOLRec, "ContactHomePhone") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Home Phone</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContactHomePhone").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                If DataFieldHasValue(FNOLRec, "ContactBusinessPhone") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Business Phone</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContactBusinessPhone").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                If DataFieldHasValue(FNOLRec, "ContactCellPhone") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Cell Phone</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContactCellPhone").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                If DataFieldHasValue(FNOLRec, "ContactTime") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Best Time to Contact</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContactTime").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            If FNOLType = "PROPERTY" Then
                ' Contractor Info
                If DataFieldHasValue(FNOLRec, "ContractorBusinessName") _
                OrElse DataFieldHasValue(FNOLRec, "ContractorContactName") _
                OrElse DataFieldHasValue(FNOLRec, "ContractorBusinessPhone") _
                OrElse DataFieldHasValue(FNOLRec, "ContractorEmail") _
                OrElse DataFieldHasValue(FNOLRec, "ContractorRemarks") Then

                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Contractor Information</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf

                    If DataFieldHasValue(FNOLRec, "ContractorBusinessName") Then
                        strBody = strBody & "<tr>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Business Name</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContractorBusinessName").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                    If DataFieldHasValue(FNOLRec, "ContractorContactName") Then
                        strBody = strBody & "<tr>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Business Contact Name</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContractorContactName").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                    If DataFieldHasValue(FNOLRec, "ContractorBusinessPhone") Then
                        strBody = strBody & "<tr>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Contact Phone</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContractorBusinessPhone").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                    If DataFieldHasValue(FNOLRec, "ContractorEmail") Then
                        strBody = strBody & "<tr>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Contact Email</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContractorEmail").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                    If DataFieldHasValue(FNOLRec, "ContractorRemarks") Then
                        strBody = strBody & "<tr>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Remarks & Additional Details</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ContractorRemarks").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                End If
            End If

            ' Insured Vehicle Owner/Insured Vehicle Driver/Other Vehicle Driver only applies to auto policies
            If FNOLType = "AUTO" Or FNOLType = "COMMERCIAL AUTO" Then
                ' Insured Vehicle Owner
                If DataFieldHasValue(FNOLRec, "InsVehOwnerFirst") AndAlso DataFieldHasValue(FNOLRec, "InsVehOwnerLast") Then
                    strBody = strBody & "<tr id='InsVehOwnerRow'>" & vbCrLf
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Insured Vehicle Owner</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf

                    strBody = strBody & "<tr id='InsVehOwnerNameRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Owner Name</td>" & vbCrLf
                    If DataFieldHasValue(FNOLRec, "InsVehOwnerMiddle") Then
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehOwnerFirst").ToString & " " & FNOLRec("InsVehOwnerMiddle").ToString & " " & FNOLRec("InsVehOwnerLast").ToString & "</td>" & vbCrLf
                    Else
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehOwnerFirst").ToString & " " & FNOLRec("InsVehOwnerLast").ToString & "</td>" & vbCrLf
                    End If
                    strBody = strBody & "</tr>" & vbCrLf

                    If DataFieldHasValue(FNOLRec, "InsVehOwnerAddress") Then
                        strBody = strBody & "<tr id='InsVehOwnerAddRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Owner Address</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehOwnerAddress").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "InsVehOwnerCity") Then
                        strBody = strBody & "<tr id='InsVehOwnerCityRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Owner City</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehOwnerCity").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "InsVehOwnerState") Then
                        strBody = strBody & "<tr id='InsVehOwnerStateRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Owner State</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehOwnerState").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "InsVehOwnerZip") Then
                        strBody = strBody & "<tr id='InsVehOwnerZipRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Owner Zip</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehOwnerZip").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                End If

                ' Insured Vehicle Driver
                strBody = strBody & "<tr id='InsVehDriverRow'>" & vbCrLf
                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Insured Vehicle Driver</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf

                If DataFieldHasValue(FNOLRec, "InsVehDriverFirst") Then
                    strBody = strBody & "<tr id='InsVehDriverFirstRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver First</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverFirst").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverMiddle") Then
                    strBody = strBody & "<tr id='InsVehDriverMiddleRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Middle</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverMiddle").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverLast") Then
                    strBody = strBody & "<tr id='InsVehDriverLastRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Last</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverLast").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverHomePhone") Then
                    strBody = strBody & "<tr id='InsVehDriverHPRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Home Phone</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverHomePhone").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverBusinessPhone") Then
                    strBody = strBody & "<tr id='InsVehDriverBPRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Business Phone</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverBusinessPhone").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverCellPhone") Then
                    strBody = strBody & "<tr id='InsVehDriverCellRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Cell Phone</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverCellPhone").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverAddress") Then
                    strBody = strBody & "<tr id='InsVehDriverAddRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Address</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverAddress").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverCity") Then
                    strBody = strBody & "<tr id='InsVehDriverCityRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver City</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverCity").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverState") Then
                    strBody = strBody & "<tr id='InsVehDriverStateRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver State</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverState").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehDriverZip") Then
                    strBody = strBody & "<tr id='InsVehDriverZipRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Driver Zip</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehDriverZip").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                ' Other Vehicle Owner
                PersonsTable = Nothing
                PersonsTable = GetFNOLPersonsByType(FNOL_Id, PersonType_Enum.OtherVehicleOwner, err)
                If PersonsTable IsNot Nothing AndAlso PersonsTable.Rows.Count > 0 Then
                    ndx = 0
                    For Each ovo As DataRow In PersonsTable.Rows
                        ndx += 1
                        ' Header Row
                        strBody = strBody & "<tr id='OthVehOwner" & ndx.ToString() & "Row'>" & vbCrLf
                        If PersonsTable.Rows.Count = 1 Then
                            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Other Vehicle Owner</td>" & vbCrLf
                        Else
                            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Other Vehicle Owner " & ndx.ToString() & "</td>" & vbCrLf
                        End If
                        strBody = strBody & "</tr>" & vbCrLf

                        ' Format the persons string for display
                        Dim personStr As String = BuildPersonDisplayString(ovo, err)
                        If personStr Is Nothing Then Throw New Exception(err)

                        ' Data Row
                        strBody = strBody & "<tr id='OthVehOwner" & ndx.ToString() & "FirstRow'>" & vbCrLf
                        strBody = strBody & "<td colspan='2' class='normaltext' align='left'>" & personStr & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    Next
                Else
                    ' NO DATA
                    ' Header Row
                    strBody = strBody & "<tr id='OthVehOwnerRow'>" & vbCrLf
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Other Vehicle Owners</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf

                    ' Data Row
                    strBody = strBody & "<tr id='OthVehOwnerFirstRow'>" & vbCrLf
                    strBody = strBody & "<td colspan='2' class='normaltext' align='center'>" & "** No Other Vehicle Owners found **" & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                ' Other Vehicle Drivers
                PersonsTable = Nothing
                PersonsTable = GetFNOLPersonsByType(FNOL_Id, PersonType_Enum.OtherVehicleDriver, err)
                If PersonsTable IsNot Nothing AndAlso PersonsTable.Rows.Count > 0 Then
                    ndx = 0
                    For Each drv As DataRow In PersonsTable.Rows
                        ndx += 1
                        ' Header Row
                        strBody = strBody & "<tr id='OthVehDriver" & ndx.ToString() & "Row'>" & vbCrLf
                        If PersonsTable.Rows.Count = 1 Then
                            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Other Vehicle Driver</td>" & vbCrLf
                        Else
                            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Other Vehicle Driver " & ndx.ToString() & "</td>" & vbCrLf
                        End If
                        strBody = strBody & "</tr>" & vbCrLf

                        ' Format the persons string for display
                        Dim personStr As String = BuildPersonDisplayString(drv, err)
                        If personStr Is Nothing Then Throw New Exception(err)

                        ' Data Row
                        strBody = strBody & "<tr id='OthVehDriver" & ndx.ToString() & "FirstRow'>" & vbCrLf
                        strBody = strBody & "<td colspan='2' class='normaltext' align='left'>" & personStr & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    Next
                Else
                    ' NO DATA
                    ' Header Row
                    strBody = strBody & "<tr id='OthVehDriverRow'>" & vbCrLf
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Other Vehicle Drivers</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf

                    ' Data Row
                    strBody = strBody & "<tr id='OthVehDriverFirstRow'>" & vbCrLf
                    strBody = strBody & "<td colspan='2' class='normaltext' align='center'>" & "** No Other Vehicle Drivers found **" & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' Loss Information
            strBody = strBody & "<tr>" & vbCrLf
            strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Loss Information</td>" & vbCrLf
            strBody = strBody & "</tr>" & vbCrLf

            If DataFieldHasValue(FNOLRec, "LossLocation") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss Location</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossLocation").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossAddress") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss Address</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossAddress").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossCity") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss City</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossCity").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossState") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss State</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossState").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossZip") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss Zip</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossZip").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossDescription") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Loss Description</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossDescription").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            ' Loss kind only applies to property
            If FNOLType = "PROPERTY" AndAlso DataFieldHasValue(FNOLRec, "LossKindDescription") Then
                strBody = strBody & "<tr id='LossKindRow'>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Kind of Loss</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossKindDescription").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If DataFieldHasValue(FNOLRec, "LossAmount") Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Estimated Loss Amount</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("LossAmount").ToString & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            If CBool(FNOLRec("PoliceContacted")) Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='right'>Police Contacted</td>" & vbCrLf
                strBody = strBody & "<td class='normaltext' align='left'>Yes</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
                If DataFieldHasValue(FNOLRec, "DepartmentName") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Department Name</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("DepartmentName").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
                If DataFieldHasValue(FNOLRec, "ReportNumber") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Report Number</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ReportNumber").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' Vehicle Info
            ' Only applies to auto policies
            If FNOLType = "AUTO" OrElse FNOLType = "COMMERCIAL AUTO" Then
                strBody = strBody & "<tr id='VehInfoRow'>" & vbCrLf
                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Vehicle Information</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf

                If DataFieldHasValue(FNOLRec, "InsVehicleMake") Then
                    strBody = strBody & "<tr id='VehMakeRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle Make</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleMake").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehicleModel") Then
                    strBody = strBody & "<tr id='VehModelRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle Model</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleModel").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehicleYear") Then
                    strBody = strBody & "<tr id='VehYearRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle Year</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleYear").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehicleVIN") Then
                    strBody = strBody & "<tr id='VehVINRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle VIN</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleVIN").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehiclePlateNumber") Then
                    strBody = strBody & "<tr id='VehPlateRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle Plate Number</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehiclePlateNumber").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehicleDamage") Then
                    strBody = strBody & "<tr id='VehDamageRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle Damage</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleDamage").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehicleDamageAmount") Then
                    strBody = strBody & "<tr id='VehDamageAmtRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Vehicle Damage Amount</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleDamageAmount").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "InsVehicleLocation") Then
                    strBody = strBody & "<tr id='VehWhereDamageRow'>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Location of Damaged Vehicle</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("InsVehicleLocation").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' Claimant/Injured
            ' Does not apply to Property claims
            If FNOLType <> "PROPERTY" Then
                PersonsTable = Nothing
                PersonsTable = GetFNOLPersonsByType(FNOL_Id, PersonType_Enum.Injured, err)
                If PersonsTable IsNot Nothing AndAlso PersonsTable.Rows.Count > 0 Then
                    ndx = 0
                    For Each clm As DataRow In PersonsTable.Rows
                        ndx += 1
                        ' Header Row
                        strBody = strBody & "<tr id='Claimant" & ndx.ToString() & "Row'>" & vbCrLf
                        If PersonsTable.Rows.Count = 1 Then
                            If FNOLType = "LIABILITY" Then
                                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Claimant Information</td>" & vbCrLf
                            Else
                                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Injured Information </td>" & vbCrLf
                            End If
                        Else
                            If FNOLType = "LIABILITY" Then
                                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Claimant " & ndx.ToString() & "</td>" & vbCrLf
                            Else
                                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Injured " & ndx.ToString() & "</td>" & vbCrLf
                            End If
                        End If
                        strBody = strBody & "</tr>" & vbCrLf

                        ' Format the persons string for display
                        Dim personStr As String = BuildPersonDisplayString(clm, err)
                        If personStr Is Nothing Then Throw New Exception(err)

                        ' Data Row
                        strBody = strBody & "<tr id='Claimant" & ndx.ToString() & "FirstRow'>" & vbCrLf
                        strBody = strBody & "<td colspan='2' class='normaltext' align='left'>" & personStr & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    Next
                Else
                    ' NO DATA
                    'Header Row
                    ndx += 1
                    strBody = strBody & "<tr id='ClaimantInfoRow'>" & vbCrLf
                    If FNOLType = "LIABILITY" Then
                        ' For LIABILITY, we show CLAIMANT info
                        strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Claimant Information</td>" & vbCrLf
                    Else
                        ' For AUTO we show INJURED info
                        strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Injured Information</td>" & vbCrLf
                    End If
                    strBody = strBody & "</tr>" & vbCrLf

                    ' Data Row
                    strBody = strBody & "<tr id='ClaimantFirstRow'>" & vbCrLf
                    If FNOLType = "LIABILITY" Then
                        ' For LIABILITY, we show CLAIMANT info
                        strBody = strBody & "<td colspan='2' class='normaltext' align='center'>" & "** No Claimants found **" & "</td>" & vbCrLf
                    Else
                        ' For AUTO we show INJURED info
                        strBody = strBody & "<td colspan='2' class='normaltext' align='center'>" & "** No Claimants found **" & "</td>" & vbCrLf
                    End If
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' Property Damage/Property Information
            If DataFieldHasValue(FNOLRec, "PropertyDescription") _
                OrElse DataFieldHasValue(FNOLRec, "PropertyDamage") _
                OrElse DataFieldHasValue(FNOLRec, "PropertyDamageAmount") Then
                strBody = strBody & "<tr>" & vbCrLf
                If FNOLType = "LIABILITY" Then
                    ' For LIABILITY the section is PROPERTY DAMAGE
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Property Damage / Injury Information</td>" & vbCrLf
                Else
                    ' For AUTO and PROPERTY the section is PROPERTY INFORMATION
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Property Information</td>" & vbCrLf
                End If
                strBody = strBody & "</tr>" & vbCrLf

                ' Injury description only applies to LIABILITY
                If FNOLType = "LIABILITY" Then
                    strBody = strBody & "<tr id='InjuredDescRow2'>"
                    strBody = strBody & "<td class='normaltext' align='right'>Injury Description (if applicable)</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PropertyInjuryDescription").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "PropertyDescription") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Property Description</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PropertyDescription").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "PropertyDamage") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Property Damage</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PropertyDamage").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "PropertyDamageAmount").ToString Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Property Damage Amount</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PropertyDamageAmount").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "PropertyLocation") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Where Property Damage</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("PropertyLocation").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                ' Other Liability
                ' Only applies to LIABILITY
                If FNOLType = "LIABILITY" Then
                    If DataFieldHasValue(FNOLRec, "OtherLiabilityDescription") Then
                        strBody = strBody & "<tr id='OtherLiabDescRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Description</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityDescription").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiabilityClaimantName") Then
                        strBody = strBody & "<tr id='OtherLiabNameRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Name</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityClaimantName").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiabilityClaimantAddress") Then
                        strBody = strBody & "<tr id='OtherLiabAddRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Address</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityClaimantAddress").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiabilityClaimantContactNumbers") Then
                        strBody = strBody & "<tr id='OtherLiabContactsRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Contact #s</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityClaimantContactNumbers").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    ' Other claimant 2
                    If DataFieldHasValue(FNOLRec, "OtherLiability2Description") Then
                        strBody = strBody & "<tr id='OtherLiabDescRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Description</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiability2Description").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiability2ClaimantName") Then
                        strBody = strBody & "<tr id='OtherLiabNameRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Name</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiability2ClaimantName").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiability2ClaimantAddress") Then
                        strBody = strBody & "<tr id='OtherLiabAddRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Address</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiability2ClaimantAddress").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiability2ClaimantContactNumbers") Then
                        strBody = strBody & "<tr id='OtherLiabContactsRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Contact #s</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiability2ClaimantContactNumbers").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    ' Other claimant 3
                    If DataFieldHasValue(FNOLRec, "OtherLiability3Description") Then
                        strBody = strBody & "<tr id='OtherLiabDescRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Description</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityDescription").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiability3ClaimantName") Then
                        strBody = strBody & "<tr id='OtherLiabNameRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Name</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityClaimantName").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiability3ClaimantAddress") Then
                        strBody = strBody & "<tr id='OtherLiabAddRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Address</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityClaimantAddress").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If

                    If DataFieldHasValue(FNOLRec, "OtherLiability3ClaimantContactNumbers") Then
                        strBody = strBody & "<tr id='OtherLiabContactsRow'>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='right'>Other Liability Claimant Contact #s</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherLiabilityClaimantContactNumbers").ToString & "</td>" & vbCrLf
                        strBody = strBody & "</tr>" & vbCrLf
                    End If
                End If

                If DataFieldHasValue(FNOLRec, "OtherPartyPolicyNumber") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Other Party Policy Number</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherPartyPolicyNumber").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "OtherPartyInsurer") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Other Party Insurer</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("OtherPartyInsurer").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' Witness
            ' Does not apply to Liability  ** YES IT DOES MGB 4/5/16
            ' Other Vehicle Drivers
            PersonsTable = Nothing
            PersonsTable = GetFNOLPersonsByType(FNOL_Id, PersonType_Enum.Witness, err)
            If PersonsTable IsNot Nothing AndAlso PersonsTable.Rows.Count > 0 Then
                ndx = 0
                For Each wit As DataRow In PersonsTable.Rows
                    ndx += 1
                    ' Header Row
                    strBody = strBody & "<tr id='Witness" & ndx.ToString() & "Row'>" & vbCrLf
                    If PersonsTable.Rows.Count = 1 Then
                        strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Witness</td>" & vbCrLf
                    Else
                        strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Witness " & ndx.ToString() & "</td>" & vbCrLf
                    End If
                    strBody = strBody & "</tr>" & vbCrLf

                    ' Format the persons string for display
                    Dim personStr As String = BuildPersonDisplayString(wit, err)
                    If personStr Is Nothing Then Throw New Exception(err)

                    ' Data Row
                    strBody = strBody & "<tr id='Witness" & ndx.ToString() & "FirstRow'>" & vbCrLf
                    strBody = strBody & "<td colspan='2' class='normaltext' align='left'>" & personStr & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                Next
            Else
                ' NO DATA
                ' Header Row
                strBody = strBody & "<tr id='WitnessRow'>" & vbCrLf
                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Witnesses</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf

                ' Data Row
                strBody = strBody & "<tr id='WitnessFirstRow'>" & vbCrLf
                strBody = strBody & "<td colspan='2' class='normaltext' align='center'>" & "** No Witnesses found **" & "</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf
            End If

            ' Misc Info
            If CBool(FNOLRec("ManualDraft")) OrElse DataFieldHasValue(FNOLRec, "ReportedBy") _
                OrElse DataFieldHasValue(FNOLRec, "Comments_AddlInfo") OrElse DataFieldHasValue(FNOLRec, "ConfirmEmailAddress") _
                OrElse (CBool(FNOLRec("AddlDocoMail")) OrElse CBool(FNOLRec("AddlDocoFAX")) OrElse CBool(FNOLRec("AddlDocoEmail"))) Then
                strBody = strBody & "<tr>" & vbCrLf
                strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Misc Information</td>" & vbCrLf
                strBody = strBody & "</tr>" & vbCrLf

                If CBool(FNOLRec("ManualDraft")) Then
                    If DataFieldHasValue(FNOLRec, "ManualDraftPayee") AndAlso DataFieldHasValue(FNOLRec, "ManualDraftCheckAmount") AndAlso DataFieldHasValue(FNOLRec, "ManualDraftCheckNumber") Then
                        strBody = strBody & "<tr><td class='normaltext' align='right'>Manual Draft</td>" & vbCrLf
                        strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ManualDraftPayee").ToString & " #" & FNOLRec("ManualDraftCheckNumber").ToString & " " & FNOLRec("ManualDrafCheckAmount").ToString & "</td></tr>" & vbCrLf
                    End If
                End If

                If DataFieldHasValue(FNOLRec, "ReportedBy") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Reported By</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ReportedBy").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "Comments_AddlInfo") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Comments / Additional Information</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("Comments_AddlInfo").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If CBool(FNOLRec("AddlDocoMail")) OrElse CBool(FNOLRec("AddlDocoFAX")) OrElse CBool(FNOLRec("AddlDocoMail")) Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Additional Documents to be sent by</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & GetBitValues(FNOLRec) & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If

                If DataFieldHasValue(FNOLRec, "ConfirmEmailAddress") Then
                    strBody = strBody & "<tr>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='right'>Confirmation Email Address</td>" & vbCrLf
                    strBody = strBody & "<td class='normaltext' align='left'>" & FNOLRec("ConfirmEmailAddress").ToString & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            If DataFieldHasValue(FNOLRec, "DiamondClaimNumber") Then
                Dim showVirtualAppraisal As String = GetVirtualAppraisal(FNOLRec("diamondClaimNumber").ToString)
                If showVirtualAppraisal = "1" Then
                    ' NO DATA
                    ' Header Row
                    strBody = strBody & "<tr id='VirtualAppraisalRow'>" & vbCrLf
                    strBody = strBody & "<td class='subheadline' colspan='2' align='center'>Virtual Appraisal</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf

                    ' Data Row
                    strBody = strBody & "<tr id='VirtualAppraisalFirstRow'>" & vbCrLf
                    strBody = strBody & "<td colspan='2' class='normaltext' align='center'>" & "The member chose to proceed with the virtual appraisal." & "</td>" & vbCrLf
                    strBody = strBody & "</tr>" & vbCrLf
                End If
            End If

            ' DEC disclaimer
            strBody = strBody & "<tr><td colspan='2'>&nbsp;</td></tr>" & vbCrLf
            strBody = strBody & "<tr>" & vbCrLf
            strBody = strBody & "<td colspan='2'>" & vbCrLf
            strBody = strBody & "<p class='normaltext'>NOTES:</p>" & vbCrLf
            strBody = strBody & "<p class='normaltext'>(1) If this loss was incurred on a previous policy image, you may need to obtain an older version of the Declaration for the policy image that was in force when the loss occurred.</p>" & vbCrLf
            strBody = strBody & "<p class='normaltext'>(2) If the phone numbers provided differ from the phone records we have on file, you should contact your underwriter to update our records with current contact information.</p>" & vbCrLf
            strBody = strBody & "</td>" & vbCrLf
            strBody = strBody & "</tr>" & vbCrLf

            strBody = strBody & "</table></form></body></html>"

            Return strBody
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLEmailBody", ex)
            Return Nothing
        End Try
    End Function


    Public Function GetBitValues(dr As DataRow) As String
        Dim ForwardDocs As String = Nothing

        Try
            If CBool(dr("AddlDocoMail")) Then
                ForwardDocs = "Mail"
            End If
            If CBool(dr("AddlDocoFax")) Then
                If ForwardDocs = "" Then
                    ForwardDocs = "Fax"
                Else
                    ForwardDocs = ForwardDocs & ", Fax"
                End If
            End If
            If CBool(dr("AddlDocoEmail")) Then
                If ForwardDocs = "" Then
                    ForwardDocs = "Email"
                Else
                    ForwardDocs = ForwardDocs & ", Email"
                End If
            End If
            Return ForwardDocs
        Catch ex As Exception
            HandleError(ClassName, "GetBitValues", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Will only run once per day
    ''' Purges all assigned claims after the specified amount of days have passed since the claim was assigned
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PurgeAssignedClaims() As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim DaysBack As String = Nothing
        Dim LastPurge As New DateTime()
        Dim rtn As Integer = 0
        Dim err As String = Nothing
        Dim dy As String = Nothing
        Dim txn As SqlTransaction = Nothing

        Try
            ' Open the connection and begin the transaction
            conn.ConnectionString = strConnFNOL
            conn.Open()
            txn = conn.BeginTransaction()

            ' Get the last purge date/time
            tbl = New DataTable()
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.Transaction = txn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOL_ClaimAssign_Settings"
            da = New SqlDataAdapter()
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count < 0 Then Throw New Exception("Unable to get FNOL settings record")
            If tbl.Rows.Count > 0 AndAlso Not IsDBNull(tbl.Rows(0)("LastPurgeDateTime")) Then LastPurge = CDate(tbl.Rows(0)("LastPurgeDateTime"))

            dy = GetAssignedClaimsPurgeDays(err, Nothing)
            If dy Is Nothing OrElse Not IsNumeric(dy) Then dy = "30"

            ' Only purge once per day
            If LastPurge = New DateTime() OrElse LastPurge.Date < DateTime.Today.Date Then
                ' PURGE RECORDS
                DaysBack = "-" & dy

                ' Get all FNOL records to be deleted
                tbl = New DataTable()
                cmd = New SqlCommand()
                cmd.Transaction = txn
                cmd.Connection = conn
                cmd.CommandType = CommandType.Text
                cmd.CommandText = "SELECT * FROM tbl_FNOL WHERE assigned = 1 AND DateAssignedToAdjuster < DATEADD(DAY, " & DaysBack & ", GETDATE())"
                da = New SqlDataAdapter()
                da.SelectCommand = cmd
                da.Fill(tbl)

                If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                    For Each dr As DataRow In tbl.Rows
                        Dim fnolid As String = dr("FNOL_ID").ToString()
                        ' DELETE PERSONS RECORDS
                        cmd = New SqlCommand()
                        cmd.Transaction = txn
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.Text
                        cmd.CommandText = "DELETE FROM tbl_persons WHERE FNOL_Id = " & fnolid
                        rtn = cmd.ExecuteNonQuery()

                        ' DELETE ATTACHMENTS
                        cmd = New SqlCommand()
                        cmd.Transaction = txn
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.Text
                        cmd.CommandText = "DELETE FROM tbl_FNOLDocuments WHERE FNOL_Id = " & fnolid
                        rtn = cmd.ExecuteNonQuery()

                        ' DELETE FNOL RECORD
                        cmd = New SqlCommand()
                        cmd.Connection = conn
                        cmd.Transaction = txn
                        cmd.CommandType = CommandType.Text
                        cmd.CommandText = "DELETE FROM tbl_FNOL WHERE FNOL_Id = " & fnolid
                        rtn = cmd.ExecuteNonQuery()
                    Next
                End If

                ' ***********************
                ' Update last purge date
                ' ***********************
                ' There should be exactly ONE record in the FNOL_ClaimAssign_Settings table

                ' Count settings records
                cmd = New SqlCommand()
                cmd.Connection = conn
                cmd.Transaction = txn
                cmd.CommandType = CommandType.Text
                cmd.CommandText = "SELECT COUNT(LastPurgeDateTime) FROM FNOL_ClaimAssign_Settings"
                Dim cnt As Integer = CInt(cmd.ExecuteScalar())

                If cnt = 0 Then
                    ' No record exists, add one
                    cmd = New SqlCommand()
                    cmd.Transaction = txn
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "INSERT INTO FNOL_ClaimAssign_Settings (LastPurgeDateTime) VALUES('" & DateTime.Now.ToShortDateString() & " " & DateTime.Now.ToShortTimeString() & "')"
                    rtn = cmd.ExecuteNonQuery()
                    If rtn <> 1 Then Throw New Exception("Error inserting ClaimAssign_Settings record")
                Else
                    ' Update existing record
                    cmd = New SqlCommand()
                    cmd.Connection = conn
                    cmd.Transaction = txn
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "UPDATE FNOL_ClaimAssign_Settings SET LastPurgeDateTime = '" & DateTime.Now.ToShortDateString() & " " & DateTime.Now.ToShortTimeString() & "'"
                    rtn = cmd.ExecuteNonQuery()
                    If rtn <> 1 Then Throw New Exception("Error updating ClaimAssign_Settings record")
                End If
            End If

            txn.Commit()

            Return True
        Catch ex As Exception
            txn.Rollback()
            HandleError(ClassName, "PurgeAssignedClaims", ex)
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            If conn IsNot Nothing Then conn.Dispose()
            If cmd IsNot Nothing Then cmd.Dispose()
            If da IsNot Nothing Then da.Dispose()
            If tbl IsNot Nothing Then tbl.Dispose()
            If txn IsNot Nothing Then txn.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Pass in a FNOL record datarow and this function will attempt to format the Insured Address
    ''' into a line-breaked display address
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function FormatInsuredAddress(ByVal dr As DataRow, ByRef err As String) As String
        Dim formattedAddress As String = String.Empty

        Try
            If Not IsDBNull(dr("InsuredAddress")) AndAlso dr("InsuredAddress").ToString.Trim <> String.Empty Then
                formattedAddress += dr("InsuredAddress").ToString
            End If
            If Not IsDBNull(dr("InsuredCity")) AndAlso dr("InsuredCity").ToString.Trim <> String.Empty Then
                If formattedAddress <> String.Empty Then formattedAddress += vbCrLf
                formattedAddress += dr("InsuredCity").ToString
            End If
            If Not IsDBNull(dr("InsuredState")) AndAlso dr("InsuredState").ToString.Trim <> String.Empty Then
                If Not IsDBNull(dr("InsuredCity")) AndAlso dr("InsuredCity").ToString.Trim <> String.Empty Then
                    formattedAddress += ", "
                Else
                    formattedAddress += vbCrLf
                End If
                formattedAddress += dr("InsuredState").ToString
            End If
            If Not IsDBNull(dr("InsuredZip")) AndAlso dr("InsuredZip").ToString.Trim <> String.Empty Then
                If formattedAddress <> String.Empty Then formattedAddress += vbCrLf
                formattedAddress += dr("InsuredZip").ToString
            End If

            Return formattedAddress
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "FormatInsuredAddress", ex, Nothing)
            Return String.Empty
        End Try
    End Function

    Public Function ScrubText(ByVal TextValue As String) As String
        Dim tmp As String = Nothing
        Try
            ' All quotation marks must be removed
            tmp = TextValue.Replace("'", "")
            tmp = tmp.Replace("""", "")

            ' Remove special characters that will be flagged as illegal
            tmp = tmp.Replace("<", "")
            tmp = tmp.Replace("/>", "")
            tmp = tmp.Replace(">", "")
            tmp = tmp.Replace(":", "")

            Return tmp
        Catch ex As Exception
            HandleError(ClassName, "ScrubText", ex, Nothing)
            Return TextValue
        End Try
    End Function

    ''' <summary>
    ''' This function takes a datarow from the PersonsTable and formats it's data into a displayable string.
    ''' </summary>
    ''' <param name="personsRow"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function BuildPersonDisplayString(ByVal personsRow As DataRow, ByRef err As String) As String
        Dim str As String = String.Empty
        Try
            ' Name
            If (Not IsDBNull(personsRow("FirstName")) AndAlso personsRow("FirstName").ToString <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("MiddleName")) AndAlso personsRow("MiddleName").ToString <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("LastName")) AndAlso personsRow("LastName").ToString <> String.Empty) Then

                str += "Name: "
                If Not IsDBNull(personsRow("FirstName")) AndAlso personsRow("FirstName").ToString <> String.Empty Then str += personsRow("FirstName") & " "
                If Not IsDBNull(personsRow("MiddleName")) AndAlso personsRow("MiddleName").ToString <> String.Empty Then str += personsRow("MiddleName") & " "
                If Not IsDBNull(personsRow("LastName")) AndAlso personsRow("LastName").ToString <> String.Empty Then str += personsRow("LastName") & "<br />"
                str += "<br />"
            End If

            ' Phones
            If (Not IsDBNull(personsRow("HomePhone")) AndAlso personsRow("HomePhone").ToString.Trim <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("BusinessPhone")) AndAlso personsRow("BusinessPhone").ToString.Trim <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("CellPhone")) AndAlso personsRow("CellPhone").ToString.Trim <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("FAX")) AndAlso personsRow("FAX").ToString.Trim <> String.Empty) Then

                str += "Phone Numbers:" & "<br />" & "----------------------" & "<br />"

                If Not IsDBNull(personsRow("HomePhone")) AndAlso personsRow("HomePhone").ToString.Trim <> String.Empty Then
                    str += "Home: " & personsRow("HomePhone").ToString & "<br />"
                End If
                If Not IsDBNull(personsRow("BusinessPhone")) AndAlso personsRow("BusinessPhone").ToString.Trim <> String.Empty Then
                    str += "Business: " & personsRow("BusinessPhone").ToString & "<br />"
                End If
                If Not IsDBNull(personsRow("CellPhone")) AndAlso personsRow("CellPhone").ToString.Trim <> String.Empty Then
                    str += "Cell: " & personsRow("CellPhone").ToString & "<br />"
                End If
                If Not IsDBNull(personsRow("FAX")) AndAlso personsRow("FAX").ToString.Trim <> String.Empty Then
                    str += "FAX: " & personsRow("FAX").ToString & "<br />"
                End If
                str += "<br />"
            End If

            ' Address
            If (Not IsDBNull(personsRow("Address")) AndAlso personsRow("Address").ToString.Trim <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("City")) AndAlso personsRow("City").ToString.Trim <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("State")) AndAlso personsRow("State").ToString.Trim <> String.Empty) _
                OrElse (Not IsDBNull(personsRow("Zip")) AndAlso personsRow("Zip").ToString.Trim <> String.Empty) Then

                str += vbCrLf & "Address: " & "<br />" & "-------------" & "<br />"
                If Not IsDBNull(personsRow("Address")) AndAlso personsRow("Address").ToString.Trim <> String.Empty Then
                    str += personsRow("Address").ToString & "<br />"
                End If
                If Not IsDBNull(personsRow("City")) AndAlso personsRow("City").ToString.Trim <> String.Empty Then
                    str += personsRow("City").ToString
                End If
                If Not IsDBNull(personsRow("State")) AndAlso personsRow("State").ToString.Trim <> String.Empty Then
                    If Not IsDBNull(personsRow("City")) AndAlso personsRow("City").ToString.Trim <> String.Empty Then str += ", "
                    str += GetDiamondStateAbbrev(personsRow("State").ToString)
                End If
                If Not IsDBNull(personsRow("Zip")) AndAlso personsRow("Zip").ToString.Trim <> String.Empty Then
                    If Not (IsDBNull(personsRow("City")) AndAlso personsRow("City").ToString.Trim <> String.Empty) _
                        OrElse (Not IsDBNull(personsRow("State")) AndAlso personsRow("State").ToString.Trim <> String.Empty) Then
                        str += " "
                    End If
                    str += personsRow("Zip").ToString & "<br />"
                End If
                str += "<br />"
            End If

            ' Email
            If Not IsDBNull(personsRow("Email")) AndAlso personsRow("Email").ToString.Trim <> String.Empty Then
                str += "Email: " & personsRow("Email").ToString & "<br />"
                str += "<br />"
            End If

            ' Injury Info
            If (Not IsDBNull(personsRow("InjuryType").ToString) AndAlso personsRow("InjuryType").ToString <> String.Empty) _
                AndAlso (Not IsDBNull(personsRow("InjuredAge").ToString) AndAlso personsRow("InjuredAge").ToString <> String.Empty) _
                AndAlso (Not IsDBNull(personsRow("InjuryDescription").ToString) AndAlso personsRow("InjuryDescription").ToString <> String.Empty) Then

                str += "Injury Information" & "<br />"
                str += "--------------------" & "<br />"

                If (Not IsDBNull(personsRow("InjuryType").ToString) AndAlso personsRow("InjuryType").ToString <> String.Empty) Then
                    str += "Injury Type: " & personsRow("InjuryType").ToString & "<br />"
                End If
                If (Not IsDBNull(personsRow("InjuredAge").ToString) AndAlso personsRow("InjuredAge").ToString <> String.Empty) Then
                    str += "Injured Age: " & personsRow("InjuredAge").ToString & "<br />"
                End If
                If (Not IsDBNull(personsRow("InjuryDescription").ToString) AndAlso personsRow("InjuryDescription").ToString <> String.Empty) Then
                    str += "Injury Description: " & personsRow("InjuryDescription").ToString & "<br />"
                End If
            End If

            Return str
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "BuildPersonDisplayString", ex, Nothing)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns diamond state abbreviation for the passed state id
    ''' </summary>
    ''' <param name="StateId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDiamondStateAbbrev(ByVal StateId As String) As String
        Dim sqsel As SQLselectObject = Nothing
        Dim dt As DataTable = Nothing

        Try
            sqsel = New SQLselectObject(strConnDiamond, "SELECT [state] FROM [State] WHERE state_id = " & StateId)
            dt = sqsel.GetDataTable()

            If sqsel.hasError Then
                Throw New Exception("GetDiamondStateAbbrev Error: " & sqsel.errorMsg)
            Else
                If dt.Rows.Count > 0 Then
                    Return dt.Rows(0).Item("state").ToString
                Else
                    Return "err"
                End If
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetDiamondStateAbbrev", ex, Nothing)
            Return "err"
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if the passed policy is an IFM Employee policy
    ''' Employee policies will have an agency code of 9999 
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsEmployeePolicy(ByVal PolicyNumber As String, ByRef err As String) As Boolean
        Try
            Using polobj As New PolicyNumberObject(PolicyNumber)
                polobj.GetAllAgencyInfo = True
                polobj.GetPolicyInfo()
                If polobj.hasError Then Throw New Exception(polobj.errorMsg)
                If polobj.AgencyCode = "9999" Then Return True Else Return False
            End Using
        Catch ex As Exception
            HandleError(ClassName, "IsEmployeePolicy", ex, Nothing)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Deletes a record from the tbl_FNOL table by id
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function DeleteClaimById(ByVal id As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "DELETE FROM tbl_FNOL WHERE fnol_id = " & id

            rtn = cmd.ExecuteNonQuery()

            If rtn = 1 Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "DeleteClaimById", ex, Nothing)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Marks a claim as removed by setting the value of the 'Status' field to "REMOVED"
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function RemoveClaimById(ByVal id As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim err As String = Nothing
        Dim RemovedId As String = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT tbl_FNOLStatus_id FROM tbl_FNOLStatus WHERE StatusDescription = 'Removed'"
            RemovedId = cmd.ExecuteScalar()
            If RemovedId Is Nothing OrElse (Not IsNumeric(RemovedId)) Then Throw New Exception("Error retrieving FNOL Status Removed ID!")

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE tbl_FNOL set tbl_FNOLStatus_Id = " & RemovedId & " WHERE fnol_id = " & id

            rtn = cmd.ExecuteNonQuery()

            If rtn = 1 Then
                SendRemovedClaimEmail(id, err)
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError(ClassName, "RemoveClaimById", ex, Nothing)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Marks a claim as removed by setting the value of the 'Status' field to "REMOVED"
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function RestoreRemovedClaimById(ByVal id As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim RestoredId As String = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT tbl_FNOLStatus_id FROM tbl_FNOLStatus WHERE StatusDescription = 'Restored'"
            RestoredId = cmd.ExecuteScalar()
            If RestoredId Is Nothing OrElse (Not IsNumeric(RestoredId)) Then Throw New Exception("Error retrieving FNOL Status Restored ID!")

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE tbl_FNOL set tbl_FNOLStatus_Id = " & RestoredId & " WHERE fnol_id = " & id

            rtn = cmd.ExecuteNonQuery()

            If rtn = 1 Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "RestoreRemovedClaimById", ex, Nothing)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Access to the Removed Claims page is controlled by the FNOLCA_RemovedClaimsAccessUserList config file key
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <returns></returns>
    Public Function UserHasAccessToRemovedClaimsPage(ByVal pg As Page) As Boolean
        Dim UserName As String = Nothing
        Dim AuthUsers() As String = Nothing

        Try
            UserName = GetUsername(pg)
            If UserName Is Nothing Then Throw New Exception("There was an error getting the Windows User Name")

            If AppSettings("FNOLCA_RemovedClaimsAccessUserList") Is Nothing Then Return False

            AuthUsers = AppSettings("FNOLCA_RemovedClaimsAccessUserList").Split(",")

            If AuthUsers Is Nothing OrElse AuthUsers.Count = 0 Then Return False

            For Each s As String In AuthUsers
                If s.ToUpper = UserName.ToUpper Then Return True
            Next

            Return False
        Catch ex As Exception
            HandleError(ClassName, "CheckUserAccess", ex)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Returns a datatable of rows from the tbl_FNOL_CatCompany table.  By default it does not return row 0 ('none'), if you want that row 
    ''' returned, set the IncludeNoneRow to true.
    ''' </summary>
    ''' <param name="IncludeNoneRow"></param>
    ''' <returns></returns>
    Public Function GetCATCompanyList(Optional IncludeNoneRow As Boolean = False, Optional IncludeInactiveProviders As Boolean = False) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim tbl2 As New DataTable()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text

            If IncludeInactiveProviders AndAlso IncludeNoneRow Then
                ' Include everything
                cmd.CommandText = "SELECT * FROM tbl_FNOL_CATCompany"
            ElseIf IncludeNoneRow Then
                ' Exclude inactive, include none row
                cmd.CommandText = "SELECT * FROM tbl_FNOL_CATCompany WHERE Active = 1"
            ElseIf IncludeInactiveProviders Then
                ' Exclude none row, include inactive
                cmd.CommandText = "SELECT * FROM tbl_FNOL_CATCompany WHERE CompanyName <> 'None'"
            Else
                ' Exclude none row and exclude inacvtive providers
                cmd.CommandText = "SELECT * FROM tbl_FNOL_CATCompany WHERE CompanyName <> 'None' AND Active = 1"
            End If

            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl.Rows.Count <= 1 Then
                Return tbl
            Else
                Dim blankrow As DataRow = tbl.NewRow()
                tbl.Rows.InsertAt(blankrow, 0)
                Return tbl
            End If

            Return tbl
        Catch ex As Exception
            HandleError(ClassName, "GetCATCompanyList", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetCATCompanyRecord(ByVal id As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            If id Is Nothing OrElse Not IsNumeric(id) Then Throw New Exception("invalid id passed!")

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_FNOL_CATCompany WHERE tbl_FNOL_CATCompany_Id = " & id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                Return tbl.Rows(0)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetCATCompanyRecord", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns true if it's the first workday of the month.  That's the purge schedule.
    ''' I'm not concerned about skipping a purge because of a holiday, the next month should pick it up.
    ''' </summary>
    ''' <returns></returns>
    Public Function PurgeCountsRequired() As Boolean
        Try
            ' First 7 days of the month and Monday
            If DateTime.Now.Date.Day <= 7 And DateTime.Now.DayOfWeek = 1 Then Return True Else Return False
        Catch ex As Exception
            HandleError(ClassName, "PurgeCountsRequired", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Purges records from the Counts table.
    ''' Removes all records older than 370 days.
    ''' This is necessary to keep the system running smoothly.
    ''' </summary>
    ''' <returns></returns>
    Public Function PurgeCountRecords() As Boolean
        Dim CutoffDate As DateTime = DateTime.Now.AddDays(-370)
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Dim sql As String = Nothing

        Try
            sql = "DELETE FROM Counts WHERE CountDate < '" & CutoffDate.ToShortDateString & "'"

            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            rtn = cmd.ExecuteNonQuery()

            Return True
        Catch ex As Exception
            HandleError(ClassName, "PurgeCountRecords", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Returns true if the last Diamond sync was in the previous day.
    ''' Gets the last sync date from the database, table "FNOL_ClaimAssign_Settings"
    ''' </summary>
    ''' <returns></returns>
    Public Function DiamondSynchronizationRequired() As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Dim LastDate As DateTime = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT LastDiamondSyncDateTime FROM FNOL_ClaimAssign_Settings"
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Return True

            If IsDate(rtn) Then
                LastDate = CDate(rtn)
                If DateTime.Now.Date > LastDate.Date Then Return True Else Return False
            Else
                Return True
            End If
        Catch ex As Exception
            HandleError(ClassName, "DiamondSynchronizationRequired", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

#End Region

#Region "FNOL Input Page Methods and Functions (TStationTransactions)"

    ''' <summary>
    ''' Builds a string
    ''' </summary>
    ''' <param name="existingText"></param>
    ''' <param name="addText"></param>
    ''' <param name="splitter"></param>
    ''' <returns></returns>
    Public Function appendText(ByVal existingText As String, ByVal addText As String, Optional ByVal splitter As String = vbCrLf) As String
        Try
            appendText = ""

            If existingText <> "" Then
                appendText = existingText
            End If


            If addText <> "" Then
                If appendText <> "" Then
                    appendText &= splitter & addText
                Else
                    appendText = addText
                End If
            End If
        Catch ex As Exception
            HandleError(ClassName, "appendText", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Formats the passed phone number in agent format
    ''' </summary>
    ''' <param name="num"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AgentPhoneFormat(ByVal num As String) As String
        Dim area As String = Nothing
        Dim phone As String = Nothing

        Try
            If num <> "" Then
                If Len(num) = 10 Then
                    area = Left(num, 3)
                    phone = Right(num, 7)
                    num = "(" & area & ")" & Left(phone, 3) & "-" & Right(phone, 4)
                ElseIf Len(num) = 7 Then
                    num = "(" & area & ")" & Left(num, 3) & "-" & Right(num, 4)
                Else
                    num = num
                End If
            Else : num = num
            End If
            Return num
        Catch ex As Exception
            HandleError(ClassName, "AgentPhoneFormat", ex)
            Return num
        End Try
    End Function

    ''' <summary>
    ''' Inserts a record into the claim assignment log
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="AdjName"></param>
    ''' <param name="ClaimPersonnel_Id"></param>
    ''' <param name="DiamondClaimNumber"></param>
    ''' <param name="ClaimDate"></param>
    ''' <param name="LossDate"></param>
    ''' <param name="AgencyName"></param>
    ''' <param name="LossDesc"></param>
    ''' <param name="WarningMessages"></param>
    ''' <param name="ErrorMessages"></param>
    ''' <param name="ProcessMessages"></param>
    ''' <returns></returns>
    Public Function WriteClaimAssignmentLog(ByVal PolicyNumber As String, ByVal FNOL_Id As String, ByVal AdjName As String, ByVal ClaimPersonnel_Id As String, ByVal DiamondClaimNumber As String, ByVal ClaimDate As String, ByVal LossDate As String, ByVal AgencyName As String, ByVal LossDesc As String, Optional ByVal WarningMessages As List(Of String) = Nothing, Optional ByVal ErrorMessages As List(Of String) = Nothing, Optional ByVal ProcessMessages As List(Of String) = Nothing) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim ndx As Integer = -1
        Dim wm As String = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "sp_Insert_tbl_Assignment_Log"
            cmd.Parameters.AddWithValue("@FNOL_Id", CInt(FNOL_Id))
            cmd.Parameters.AddWithValue("@LogDate", DateTime.Now)
            cmd.Parameters.AddWithValue("@AssignedByUser", "Diamond Automatic")
            cmd.Parameters.AddWithValue("@AssignedToAdjusterName", AdjName)
            cmd.Parameters.AddWithValue("@AssignedToAdjuster_Claimpersonnel_Id", ClaimPersonnel_Id)
            cmd.Parameters.AddWithValue("@AssignedToManagerName", "N/A")
            cmd.Parameters.AddWithValue("@AssignedToManager_Claimpersonnel_Id", "N/A")
            cmd.Parameters.AddWithValue("@DiamondClaimNumber", DiamondClaimNumber)
            cmd.Parameters.AddWithValue("@ClaimDate", CDate(ClaimDate))
            cmd.Parameters.AddWithValue("@PolicyNumber", PolicyNumber)
            cmd.Parameters.AddWithValue("@LossDate", CDate(LossDate))
            cmd.Parameters.AddWithValue("@AgencyName", AgencyName)
            cmd.Parameters.AddWithValue("@LossDescription", LossDesc)

            ' Add messages lists as comma delimited fields
            If WarningMessages IsNot Nothing AndAlso WarningMessages.Count > 0 Then
                ndx = -1
                For Each w As String In WarningMessages
                    ndx += 1
                    wm += w
                    If ndx <> WarningMessages.Count - 1 Then wm += "|"
                Next
                cmd.Parameters.AddWithValue("@WarningMessages", wm)
            End If
            If ErrorMessages IsNot Nothing AndAlso ErrorMessages.Count > 0 Then
                wm = String.Empty
                ndx = -1
                For Each w As String In ErrorMessages
                    ndx += 1
                    wm += w
                    If ndx <> ErrorMessages.Count - 1 Then wm += "|"
                Next
                cmd.Parameters.AddWithValue("@ErrorMessages", wm)
            End If
            If ProcessMessages IsNot Nothing AndAlso ProcessMessages.Count > 0 Then
                wm = String.Empty
                ndx = -1
                For Each w As String In ProcessMessages
                    ndx += 1
                    wm += w
                    If ndx <> ProcessMessages.Count - 1 Then wm += "|"
                Next
                cmd.Parameters.AddWithValue("@ProcessingMessages", wm)
            End If

            rtn = cmd.ExecuteNonQuery()
            If rtn = Nothing OrElse Not IsNumeric(rtn) OrElse CInt(rtn) <> 1 Then
                Throw New Exception("Error inserting record into Assignment Log")
            End If

            Return True
        Catch ex As Exception
            HandleError(ClassName, "WriteClaimAssignmentLog", ex)
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates a TBL_FNOL record to assigned using the passed structure values
    ''' </summary>
    ''' <param name="AutoAssignmentDetails"></param>
    ''' <returns></returns>
    Public Function UpdateFNOLToAssigned(ByVal AutoAssignmentDetails As DiamondWebClass.DiamondClaims.FNOLResponseData_Struct) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_UpdateFNOLToAssigned"
            cmd.Parameters.AddWithValue("@FNOL_ID", AutoAssignmentDetails.FNOL_ID)
            cmd.Parameters.AddWithValue("@DiamondAdjusterID", AutoAssignmentDetails.DiamondAdjuster_Id)
            cmd.Parameters.AddWithValue("@AdjusterName", AutoAssignmentDetails.AdjusterName)
            cmd.Parameters.AddWithValue("@DateAssignedToAdjuster", AutoAssignmentDetails.DateAssigned)
            cmd.Parameters.AddWithValue("@AssignedBy", AutoAssignmentDetails.AssignedBy)
            cmd.Parameters.AddWithValue("@FNOLClaimAssignAdjuster_Id", AutoAssignmentDetails.FNOLAdjusterID)
            If AutoAssignmentDetails.CAT Then
                cmd.Parameters.AddWithValue("@CAT", 1)
            Else
                cmd.Parameters.AddWithValue("@CAT", 0)
            End If
            cmd.Parameters.AddWithValue("@GroupId", AutoAssignmentDetails.Group_Id)

            rtn = cmd.ExecuteNonQuery()

            If rtn > 0 Then Return True Else Return False
        Catch ex As Exception
            'FNOLCommon.HandleError(ClassName, "UpdateFNOLToAssigned", ex, Me, txtPolnum.Text, lblMsg)
            HandleError(ClassName, "UpdateFNOLToAssigned", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates the adjuster claim count
    ''' </summary>
    ''' <param name="AdjusterID"></param>
    ''' <param name="GroupID"></param>
    ''' <param name="CountDate"></param>
    ''' <param name="Ind"></param>
    ''' <returns></returns>
    Public Function UpdateAdjusterCount(ByVal AdjusterID As String, ByVal GroupID As String, ByVal CountDate As String, ByVal Ind As AdjusterCountUpdateIndicator) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As New Object()
        Dim NewVal As String = Nothing
        Dim OldVal As Integer = -1
        Dim drCount As DataRow = Nothing
        Dim tries = 0

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            ' Get the count record
getcount:
            drCount = GetDailyCountRecord(AdjusterID, GroupID, CountDate)
            If drCount Is Nothing Then
                If tries = 0 Then
                    ' No Daily count record found.  Create a new one.
                    If Not CreateNewCountRecord(AdjusterID, GroupID, CountDate) Then Throw New Exception("Error creating count record!")
                    tries = 1
                    GoTo getcount
                Else
                    ' We tried to create a count record but we still can't find it.  Error!
                    Throw New Exception("Error getting count record!")
                End If
            End If
            OldVal = drCount("count").ToString

            ' Calculate the new value
            Select Case Ind
                Case AdjusterCountUpdateIndicator.Decrease
                    NewVal = (OldVal - 1).ToString()
                Case AdjusterCountUpdateIndicator.Increase
                    NewVal = (OldVal + 1).ToString()
            End Select
            If NewVal < 0 Then NewVal = 0

            ' Update the record with the new count.  Use the ID from the count record
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "UPDATE Counts SET [Count] = " & NewVal & " WHERE Counts_Id = " & drCount("Counts_Id").ToString
            rtn = cmd.ExecuteNonQuery()
            If IsNumeric(rtn) Then
                If CInt(rtn) = 1 Then Return True
            End If

            Return False
        Catch ex As Exception
            'FNOLCommon.HandleError(ClassName, "UpdateAdjusterCounts", ex, Me, txtPolnum.Text, lblMsg)
            HandleError(ClassName, "UpdateAdjusterCounts", ex)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Public Function GetDailyCountRecord(ByVal AdjusterId As String, ByVal GroupId As String, ByVal CountDate As String) As DataRow
        Dim conn As New SqlConnection
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim tbl As New DataTable
        Dim sql As String = Nothing
        Dim breakout As Boolean = False

        Try
            ' See if the daily count record exists
            conn.ConnectionString = strConnFNOL
            conn.Open()

tryquery:
            sql = "SELECT * FROM Counts WHERE FNOLClaimAssignAdjuster_ID = " & AdjusterId & " AND Groups_Id = " & GroupId & " AND CountDate = '" & CountDate & "'"
            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                ' No daily count record found.  Create it.
                If CreateNewCountRecord(AdjusterId, GroupId, CountDate) And Not breakout Then
                    ' If we created a new record we need to run the query again to get it.
                    breakout = True ' Only use the goto once to avoid an infinite loop!
                    GoTo tryquery
                Else
                    Throw New Exception("Error updating count record")
                End If

            Else
                Return tbl.Rows(0)
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetDailyCountRecord", ex)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Inserts a new count record
    ''' </summary>
    ''' <param name="AdjusterID"></param>
    ''' <param name="GroupID"></param>
    ''' <param name="CountDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateNewCountRecord(ByVal AdjusterID As String, ByVal GroupID As String, ByVal CountDate As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As New Object()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_InsertNewCountRecord"
            cmd.Parameters.AddWithValue("@UserID", AdjusterID)
            cmd.Parameters.AddWithValue("@GroupID", GroupID)
            cmd.Parameters.AddWithValue("@Date", CountDate)
            rtn = cmd.ExecuteNonQuery()

            If IsNumeric(rtn) Then
                If CInt(rtn) = 1 Then Return True
            End If

            Return False
        Catch ex As Exception
            HandleError(ClassName, "CreateNewCountRecord", ex)
            Return False
        End Try
    End Function


    ''' <summary>
    ''' Gets adjuster id, name and group from the FNOL db by Diamond claimpersonnel_id
    ''' </summary>
    ''' <param name="ClaimPersonnel_Id"></param>
    ''' <param name="FNOLAdjID"></param>
    ''' <param name="AdjName"></param>
    ''' <param name="AdjGroupId"></param>
    ''' <param name="pg"></param>
    ''' <param name="PolNum"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Public Function Get_FNOLAdjusterInfo(ByVal ClaimPersonnel_Id As String, ByRef FNOLAdjID As String, ByRef AdjName As String, ByRef AdjGroupId As String, ByVal pg As Page, ByVal PolNum As String, ByVal err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim tbl As New DataTable()
        Dim da As New SqlDataAdapter()
        Try
            conn.ConnectionString = strConnFNOL

            ' Get Adjuster ID and Name
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE claimpersonnel_id = " & ClaimPersonnel_Id
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                FNOLAdjID = tbl.Rows(0)("FNOLClaimAssignAdjuster_Id").ToString()
                AdjName = tbl.Rows(0)("Display_Name").ToString()
            Else
                'Throw New Exception("Error gettig Adjuster info!")
                Dim errMsgText As String = "Error getting Adjuster info"
                If String.IsNullOrWhiteSpace(ClaimPersonnel_Id) = False Then
                    errMsgText &= " for ClaimPersonnel_Id " & ClaimPersonnel_Id
                End If
                errMsgText &= "!"
                Throw New Exception(errMsgText)
            End If

            ' Get adjuster Group ID
            cmd = New SqlCommand()
            tbl = New DataTable()
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_GetAdjusterGroups"
            cmd.Parameters.AddWithValue("@AdjusterId", FNOLAdjID)
            da = New SqlDataAdapter()
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                AdjGroupId = tbl.Rows(0)("Groups_Id").ToString()
            Else
                'Throw New Exception("Error getting adjuster group!!")
                Dim errMsgText As String = "Error getting adjuster group"
                If String.IsNullOrWhiteSpace(FNOLAdjID) = False Then
                    errMsgText &= " for FNOLAdjID " & FNOLAdjID
                End If
                errMsgText &= "!!"
                Throw New Exception(errMsgText)
            End If

            Return True
        Catch ex As Exception
            HandleError(ClassName, "Get_FNOLAdjusterInfo", ex, pg, PolNum)
            err = ex.Message
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            If conn IsNot Nothing Then conn.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Determines whether or not a policy is a safelite policy.
    ''' 
    ''' It's a safelite policy if:
    ''' * There's a record in the loss reporting table for the policy number and loss date
    '''   -- and --
    ''' * There's no Diamond Claim Number in the loss reporting record
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="PolicyNumber"></param>
    ''' <param name="LossDate"></param>
    ''' <param name="LossType"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsSafelitePolicy(ByRef pg As Page, ByVal PolicyNumber As String, ByVal LossDate As String, ByVal LossType As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim dr As DataRow = Nothing
        Dim InDataTables As Boolean = False
        Dim InSafeliteTable As Boolean = False

        Try
            ' THERE ARE TWO TABLES TO CHECK, THE REGULAR AUTO, PROPERTY OR LIABILITY TABLE AND THE SAFELITE TABLE
            conn.ConnectionString = AppSettings("conn")
            conn.Open()

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            Select Case LossType.ToUpper
                Case "AUTO", "COMMERCIAL AUTO", "COMMERCIALAUTO"
                    cmd.CommandText = "SELECT * FROM tbl_auto_reporting WHERE PolicyNumber = '" & PolicyNumber & "' AND DateOfLoss = '" & LossDate & "'"
                    Exit Select
                Case "LIABILITY"
                    cmd.CommandText = "SELECT * FROM tbl_general_reporting WHERE PolicyNumber = '" & PolicyNumber & "' AND DateOfLoss = '" & LossDate & "'"
                    Exit Select
                Case "PROPERTY"
                    cmd.CommandText = "SELECT * FROM tbl_property_reporting WHERE PolicyNumber = '" & PolicyNumber & "' AND DateOfLoss = '" & LossDate & "'"
                    Exit Select
                Case Else
                    Throw New Exception("Invalid loss type passed")
            End Select
            da.SelectCommand = cmd
            tbl = New DataTable()
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                InDataTables = False
            Else
                ' We should only ever find one matching record for the policy number and loss date
                ' regardless of how many are actually found, we're only going to look at the first one
                dr = tbl.Rows(0)

                ' If there is no Diamond Claim Number on the policy we can assume that the claim is Safelite
                ' otherwise it's not
                If IsDBNull(dr("diaClaimNum")) OrElse (dr("diaClaimNum").ToString = String.Empty) Then
                    InDataTables = True
                Else
                    InDataTables = False
                End If
            End If

            cmd = New SqlCommand()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_safelite_FNOL WHERE polnum = '" & PolicyNumber & "' AND lossdt = '" & LossDate & "'"
            da.SelectCommand = cmd
            tbl = New DataTable()
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                InSafeliteTable = False
            Else
                InSafeliteTable = True
            End If

            If InDataTables Or InSafeliteTable Then Return True Else Return False
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "IsSafelitePolicy", ex, pg, PolicyNumber)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' This function checks the passed PolicyNumberObject and populates it if empty or the policy number has changed
    ''' If it's already populated with the correct policy info it does not repopulate
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <param name="LossDate"></param>
    ''' <param name="pg"></param>
    ''' <param name="PolicyInfoObject"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PopulatePolicyInfo(ByVal PolicyNumber As String, ByVal LossDate As String, ByRef pg As Page, ByRef PolicyInfoObject As PolicyNumberObject, ByRef err As String) As Boolean
        Try
            ' The passed policy number object is populated.  If the policy number is the same as what was passed
            ' we don't need to repopulate the object
            If PolicyInfoObject IsNot Nothing AndAlso (Not PolicyInfoObject.Equals(New PolicyNumberObject)) Then
                If PolicyInfoObject.DiamondPolicyNumber = PolicyNumber Then
                    Return True
                End If
            End If

            ' Either the policy number changed or the passed policy number object is empty, 
            ' make the call to populate the passed policy number object
            'PolicyInfoObject = New PolicyNumberObject(PolicyNumber, strConn) '2/20/2019 note: this is using the connection string for the FNOL db (connFNOL) instead of the ifmtester db (conn key); only works because of other keys to use connDiamondReports
            'updated 2/20/2019 to fix legacy connection string... would cause an error if lookup fails in Diamond, policy has legacyNum, and logic needs to go back and look for active in Legacy
            PolicyInfoObject = New PolicyNumberObject(PolicyNumber, AppSettings("conn"))
            If PolicyInfoObject.hasError Then
                Throw New Exception("This policy number is in an invalid format.")
            End If

            'set evaluation date to loss date to get info for that specific policy term
            PolicyInfoObject.EvaluationDateOrLossDate = LossDate

            PolicyInfoObject.GetAllInsuredInfo = True
            PolicyInfoObject.GetAllAgencyInfo = True
            PolicyInfoObject.GetAllFNOLInfo = True
            PolicyInfoObject.GetPolicyInfo()

            ' Check to see that policy exists
            If Not PolicyInfoObject.hasPolicyInfo Then
                If PolicyInfoObject.hasPolicyInfoError Then
                    Throw New Exception("There was a problem locating this policy's information.")
                Else
                    Throw New Exception("This policy could not be located.")
                End If
            End If

            pg.Session("PolicyInfo") = PolicyInfoObject

            Return True
        Catch ex As Exception
            pg.Session("PolicyInfo") = Nothing
            err = ex.Message
            HandleError(ClassName, "PopulatePolicyInfo", ex, pg, PolicyNumber)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Updates the error string that we will display to the user on validation
    ''' Build a list of errors with line breaks
    ''' </summary>
    ''' <param name="ErrString"></param>
    ''' <param name="msg"></param>
    ''' <remarks></remarks>
    Public Sub UpdateErrorString(ByRef ErrString As String, ByVal msg As String, ByRef pg As Page, ByVal PolicyNumber As String, Optional ByRef lblMsg As Label = Nothing, Optional ByVal ErrMsgs As List(Of String) = Nothing)
        Try
            If ErrMsgs IsNot Nothing Then ErrMsgs.Add(msg)

            'If ErrString Is Nothing Then ErrString = ""
            'If ErrString = "" Then ErrString = "Please correct the following errors:\n\n"
            'ErrString += " * " & msg & "\n"
            'updated 2/20/2019; now always ending w/ 2 breaks (\n) since IE was losing a line on single-line validations
            If ErrString Is Nothing Then ErrString = ""
            If ErrString = "" Then
                ErrString = "Please correct the following errors:\n\n"
            Else
                If Right(ErrString, 4) = "\n\n" Then
                    ErrString = Left(ErrString, Len(ErrString) - 2)
                End If
            End If
            ErrString &= " * " & msg & "\n\n"


            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "UpdateErrorString", ex, pg, PolicyNumber, lblMsg)
            Exit Sub
        End Try
    End Sub

    Public Function IsValidEmail(ByVal Email As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Dim EmailError As String = ""

        Try
            Using RegEx As New RegularExpressionObject
                Return RegEx.IsEmailAddress(Email)
            End Using
        Catch ex As Exception
            HandleError(ClassName, "IsValidEmail", ex, errlabel)
            Return False
        End Try
    End Function


    Public Function IsValidEmail(ByVal Email As String, ByRef pg As Page, ByVal polnum As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Dim EmailError As String = ""

        Try
            Using RegEx As New RegularExpressionObject
                Return RegEx.IsEmailAddress(Email)
            End Using
        Catch ex As Exception
            HandleError(ClassName, "IsValidEmail", ex, pg, polnum)
            Return False
        End Try
    End Function


    '1/10/2013 - added 2 validation methods back in since previous developer moved them to class file where SetFocus doesn't work
    Public Function ValidateEmail(ByVal Email As String, ByVal control As Control, ByRef pg As Page, ByVal polnum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim EmailError As String = ""

        Try
            Using RegEx As New RegularExpressionObject
                If Not RegEx.IsEmailAddress(Email) Then
                    EmailError = "Please enter a valid email address."
                Else
                    EmailError = ""
                End If
            End Using

            Return EmailError
        Catch ex As Exception
            HandleError(ClassName, "ValidateEmail", ex, pg, polnum, errlabel)
            Return "Email Address is Invalid"
        End Try

    End Function

    Public Function ValidateZip(ByVal Zip As String, ByVal control As Control, ByRef pg As Page, ByVal polnum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim ZipError As String = ""

        Try
            Using RegEx As New RegularExpressionObject
                If RegEx.IsZipCode(Zip) = False Then
                    ZipError = "Please enter a valid zip code."
                Else
                    ZipError = ""
                End If
            End Using

            Return ZipError
        Catch ex As Exception
            HandleError(ClassName, "ValidateZip", ex, pg, polnum, errlabel)
            Return "Zip Code is invalid"
        End Try
    End Function

    ''' <summary>
    ''' Builds and returns script alert text for the passed values
    ''' </summary>
    ''' <param name="strClassName"></param>
    ''' <param name="strRoutineName"></param>
    ''' <param name="exc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetErrorScript(ByVal strClassName As String, ByVal strRoutineName As String, ByRef exc As Exception, ByRef pg As Page, ByVal polnum As String, Optional ByRef errLabel As Label = Nothing) As String
        Dim strScript As String = "<script language=JavaScript>"
        Dim message As String = Nothing

        Try
            'Return "<script>alert('Error Detected in " & strClassName & "(" & strRoutineName & "): " & exc.Message & "');</script>"
            'updated 1/21/2013 so the javascript doesn't get an error when the message includes a single quote
            Return "<script>alert('Error Detected in " & strClassName & "(" & strRoutineName & "): " & exc.Message & "');</script>"
            Exit Function
        Catch ex As Exception
            HandleError(ClassName, "GetErrorScript", ex, pg, polnum, errLabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "GetErrorScript", ex))
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Creates DEC files for the passed policy number
    ''' Returns a string collection of the file names created
    ''' If an error occurs, returns nothing
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateDECFiles(ByVal PolicyNumber As String, ByVal PolicyId As String, ByVal PolicyImageNum As String, ByRef pg As Page, ByVal latestDECPolicyID As String, ByVal latestDECImgNum As String, Optional ByRef errlabel As Label = Nothing) As List(Of String)
        Dim prt As New DiamondWebClass.DiamondPrinting
        Dim dt As DataTable = Nothing
        Dim DECByte() As Byte = Nothing
        Dim cn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim TopId As Integer = 0
        Dim drArray() As DataRow = Nothing
        Dim i As Integer = 0
        Dim fname As String = Nothing
        Dim utc As String = Nothing
        Dim filenames As New List(Of String)
        Dim path As String = Nothing
        Dim imgnum As Integer = 0

        Try
            If String.IsNullOrWhiteSpace(latestDECImgNum) Then Throw New Exception("Input parameter latestDECImgNum is Not set.")
            If String.IsNullOrWhiteSpace(latestDECPolicyID) Then Throw New Exception("Input parameter latestDECPolicyID is Not set.")

            ' First, get all of the DECS for the policy
            cn.ConnectionString = AppSettings("connCT")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_PolicyNumberDataCollection_Diamond"
            cmd.Parameters.AddWithValue("@collectionType", "Declarations")
            cmd.Parameters.AddWithValue("@DiamondPolicy", PolicyNumber)
            da.SelectCommand = cmd
            da.Fill(tbl)

            ' NEWEST DEC
            '------------------------------------------------------
            ' Get the DEC with the passed image number, which will be the most recent in-force DEC
            drArray = tbl.Select("[policy id] = '" & latestDECPolicyID & "' AND [policyimage_num] = '" & latestDECImgNum & "'")
            If drArray IsNot Nothing AndAlso drArray.Count > 0 Then
                TopId = CInt(drArray(0)("id").ToString)
            End If

            ' Set the output path and check it
            path = AppSettings("DECFolder")
            If System.IO.Directory.Exists(path) Then
                ' If output folder exists, clean up any existing FNOL DEC files older than 1 hour
                Dim dirfiles() As String = System.IO.Directory.GetFiles(path, "*_FNOL_DEC_*.pdf")
                If dirfiles IsNot Nothing AndAlso dirfiles.Count > 0 Then
                    For Each fn As String In dirfiles
                        Dim fi As New System.IO.FileInfo(fn)
                        Dim MinDiff As Integer = DateDiff(DateInterval.Minute, DateTime.Now, fi.CreationTime)
                        If MinDiff <= -60 Then fi.Delete()
                    Next
                End If
            Else
                ' If output folder does not exist, create it
                System.IO.Directory.CreateDirectory(path)
            End If

            'utc = DateTime.Now.ToUniversalTime().ToString().Replace("/", "-").Replace(":", "")
            utc = DateTime.Now.ToUniversalTime().ToShortDateString().Replace("/", "-")

            ' Get the record with the top id - this is the current DEC
            drArray = Nothing
            drArray = tbl.Select("id = " & TopId)
            Dim matchesLossDateImage As Boolean = False

            Dim DecsAlreadyRun As List(Of String) = New List(Of String)
            For Each row As DataRow In drArray
                If PolicyId = row("Policy Id").ToString AndAlso PolicyImageNum = row("policyImage_num").ToString Then matchesLossDateImage = True
                i += 1
                ' Get the PDF byte array

                If Not DecsAlreadyRun.Contains(row("Form Description").ToString) Then


                    DECByte = Nothing
                    DECByte = prt.printDec(Nothing, row("Policy ID").ToString(), TopId, row("Form Description").ToString())
                    DecsAlreadyRun.Add(row("Form Description").ToString)

                    Dim decDate As Date
                    Dim decDateString As String = String.Empty
                    If Date.TryParse(row("TEff Date"), decDate) Then
                        decDateString = decDate.ToShortDateString.Replace("/", "-")
                    Else
                        decDateString = utc

                    End If

                    ' Build the output file name: <policy>_FNOL_DEC_<#>_<timestamp>.pdf
                    fname = path & PolicyNumber & "_FNOL_DEC_" & i.ToString() & "_" & decDateString & ".pdf"

                    ' Check for existing path and duplicate file name
                    If System.IO.File.Exists(fname) Then System.IO.File.Delete(fname)
                    filenames.Add(fname)

                    ' Now that we have the Byte Array, convert it to a pdf file
                    Dim fs As New FileStream(fname, System.IO.FileMode.Create)
                    fs.Write(DECByte, 0, DECByte.Length)
                    fs.Close()

                End If
                ' On CAP policies there may be multiple copies of the same DEC, exit the loop after the 
                ' first one so we can avoid sending the duplicates.
                ' Exit For
            Next

            ' DEC THAT WAS IN FORCE ON THE LOSS DATE
            '---------------------------------------------------
            If matchesLossDateImage = False Then
                ' Loop through the results, look for the policy image number that was passed
                ' If the loss date falls within the current policy image don't attach the latest dec again
                Dim LossDecsAlreadyRun As List(Of Tuple(Of String, String)) = New List(Of Tuple(Of String, String))
                For Each dr As DataRow In tbl.Rows

                    'If dr("Policy id").ToString = PolicyId AndAlso dr("policyimage_num").ToString = PolicyImageNum AndAlso (imgnum <> dr("policyimage_num").ToString) Then
                    If dr("Policy id").ToString = PolicyId AndAlso dr("policyimage_num").ToString = PolicyImageNum Then

                        Dim tupleToCheck = New Tuple(Of String, String)(dr("printxml_id").ToString(), dr("Form Description").ToString())
                        If Not LossDecsAlreadyRun.Contains(tupleToCheck) Then
                            LossDecsAlreadyRun.Add(tupleToCheck)

                            ' Found the policy image number we were looking for

                            DECByte = Nothing
                            DECByte = prt.printDec(Nothing, dr("Policy ID").ToString(), dr("printxml_id").ToString(), dr("Form Description").ToString())

                            Dim decDate As Date
                            Dim decDateString As String = String.Empty
                            If Date.TryParse(dr("TEff Date"), decDate) Then
                                decDateString = decDate.ToShortDateString.Replace("/", "-")
                            Else
                                decDateString = utc

                            End If

                            ' Build the output file name: <policy>_FNOL_DEC_LossDate_<timestamp>.pdf
                            'Dim ts As String = DateTime.Now.Year.ToString & DateTime.Now.Month.ToString.PadLeft(2, "0") & DateTime.Now.Day.ToString.PadLeft(2, "0") & DateTime.Now.Hour.ToString.PadLeft(2, "0") & DateTime.Now.Minute.ToString.PadLeft(2, "0") & DateTime.Now.Second.ToString.PadLeft(2, "0")
                            fname = path & PolicyNumber & "_FNOL_DEC_(" & dr("Policy ID").ToString & "-" & dr("policyimage_num").ToString & ")_" & decDateString & ".pdf"

                            ' Check for existing path and duplicate file name
                            If System.IO.File.Exists(fname) Then System.IO.File.Delete(fname)
                            filenames.Add(fname)

                            ' Now that we have the Byte Array, convert it to a pdf file
                            Dim fs As New FileStream(fname, System.IO.FileMode.Create)
                            fs.Write(DECByte, 0, DECByte.Length)
                            fs.Close()

                            ' On CAP policies there may be multiple copies of the same DEC, exit the loop after the 
                            ' first one so we can avoid sending the duplicates.
                            'Exit For
                        End If
                    End If
                Next
            End If

            Return filenames
        Catch ex As Exception
            HandleError(ClassName, "CreateDECFiles", ex, pg, PolicyNumber, errlabel)
            Return Nothing
        Finally
            If cn.State = ConnectionState.Open Then cn.Close()
            cn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Sends an email
    ''' </summary>
    ''' <param name="InRec"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendEmail(ByRef InRec As EmailInfo_Structure_FNOLInputPage, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing, Optional ByVal DECAttachmentFileList As List(Of String) = Nothing, Optional ResendWithoutAgentEmail As Boolean = False) As Boolean
        Dim attachments As New ArrayList
        Dim messAtt As System.Net.Mail.Attachment = Nothing
        Dim info As FileInfo = Nothing
        Dim objMail As EmailObject = Nothing
        Dim FileList As String() = Nothing
        Dim systememail As String = Nothing
        Dim LossType As String = Nothing
        Dim resend As Boolean = False

        Try
            objMail = New EmailObject()

            Using objMail
                ' Attach DEC(s) to email
                If DECAttachmentFileList IsNot Nothing Then
                    For Each fn As String In DECAttachmentFileList
                        If System.IO.File.Exists(fn) Then 'added IF to make sure file exists before trying to attach
                            messAtt = New System.Net.Mail.Attachment(fn)
                            attachments.Add(messAtt)
                        End If
                    Next
                End If
                If attachments IsNot Nothing AndAlso attachments.Count > 0 Then
                    objMail.EmailAttachments = attachments
                    attachments = Nothing
                End If
                'end attaching files

                ' FROM address - used passed value or default
                If InRec.FromAddress_OPTIONAL IsNot Nothing AndAlso InRec.FromAddress_OPTIONAL <> "" Then
                    objMail.EmailFromAddress = InRec.FromAddress_OPTIONAL
                Else
                    objMail.EmailFromAddress = "LossReporting@IndianaFarmers.com"
                End If

                ' Confirmation email to agency
                ' Only sent when the TestOrProd switch is NOT "TEST"
                ' 4/16/15 DON'T SEND TO AGENT ON RESEND MGB
                If UCase(System.Configuration.ConfigurationManager.AppSettings("TestOrProd")) = "TEST" OrElse ResendWithoutAgentEmail Then
                    If InStr(UCase(InRec.ConfirmEmail), "@INDIANAFARMERS.COM") > 0 Then
                        '-ok
                        InRec.ConfirmEmail = InRec.ConfirmEmail
                    Else
                        '-don't send agent confirmation while testing, send to the developer
                        InRec.ConfirmEmail = AppSettings("LossReportingEmail")
                    End If
                End If

                objMail.EmailToAddress = InRec.ConfirmEmail

                ' If environment is TEST always send a copy to the email address in the LossReportingEmail key MGB 5/12/16
                If AppSettings("TestOrProd").ToUpper = "TEST" Then
                    If Not (AppSettings("LossReportingEmail").ToUpper.Contains(objMail.EmailToAddress.ToUpper)) Then
                        objMail.EmailCCAddress = AppSettings("LossReportingEmail")
                    End If
                End If

                ' **************************************************************************
                ' Don't send the email to the Loss Reporting Inbox anymore MGB 5/12/2016
                ' **************************************************************************
                'Using polnum As New PolicyNumberObject(InRec.PolicyNumber)
                '    If polnum.IsInDiamondFormat = True Then
                '        'use Diamond email address
                '        systememail = System.Configuration.ConfigurationManager.AppSettings("DiamondLossReportingEmail")
                '    Else
                '        'use legacy email address
                '        systememail = System.Configuration.ConfigurationManager.AppSettings("LossReportingEmail")
                '    End If
                'End Using

                'If InRec.ConfirmEmail <> "" Then
                '    objMail.EmailToAddress = InRec.ConfirmEmail
                '    objMail.EmailCCAddress = systememail
                'Else
                '    objMail.EmailToAddress = systememail
                'End If

                Select Case pg.Session("FNOLtype").ToString.ToUpper
                    Case "AUTO", "COMMERCIAL AUTO", "COMMERCIALAUTO"
                        LossType = "Auto Loss"
                    Case "PROPERTY"
                        LossType = "Property Loss"
                    Case "LIABILITY"
                        LossType = "Liability Loss"
                End Select

                ' If there is a value in the subjectline field, use it, otherwise use the line shown below
                If InRec.SubjectLine_OPTIONAL IsNot Nothing AndAlso InRec.SubjectLine_OPTIONAL <> "" Then
                    objMail.EmailSubject = InRec.SubjectLine_OPTIONAL
                Else
                    objMail.EmailSubject = LossType & " for " & InRec.PolicyNumber & " - " & InRec.PolicyHolderFirstName & " " & InRec.PolicyHolderLastName
                End If

                'set line breaks for each email row'style='border-color:inherit'

                ' Set the body of the email
                objMail.EmailBody = InRec.Body

                ' If there is a value in the mailhost field, use it, otherwise use the mail host set in the config file
                If InRec.MailHost_OPTIONAL IsNot Nothing AndAlso InRec.MailHost_OPTIONAL <> "" Then
                    objMail.MailHost = InRec.MailHost_OPTIONAL
                Else
                    objMail.MailHost = System.Configuration.ConfigurationManager.AppSettings("mailhost")
                End If

                objMail.SendEmail()

                If objMail.hasError Then
                    resend = True
                    pg.Session("EmailError") = "yes"
                    Throw New Exception("objMail has error: " & objMail.errorMsg)
                    'SendErrorEmail("objMail hasError", InRec.PolicyNumber, InRec.Body & vbCrLf & objMail.errorMsg & " Attachment running total " & pg.Session("runningtotal").ToString)
                End If

            End Using

            Return True
        Catch ex As Exception
            If resend Then
                ' If there was an error, try and resend the email without the agent confirmation
                Return SendEmail(InRec, pg, policynum, errlabel, DECAttachmentFileList, True)
            Else
                HandleError(ClassName, "SendEmail", ex, pg, policynum, errlabel)
                Return False
            End If
        End Try
    End Function

    ''' <summary>
    ''' Sends an error email to the address in the appsettings LossReportingErrorEmail parameter
    ''' </summary>
    ''' <param name="subject"></param>
    ''' <param name="polnum"></param>
    ''' <param name="bodyString"></param>
    ''' <remarks></remarks>
    Public Sub SendErrorEmail(ByVal subject As String, ByVal polnum As String, ByVal bodyString As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing)
        Try
            Using em As New EmailObject
                em.MailHost = System.Configuration.ConfigurationManager.AppSettings("mailhost")
                em.EmailSubject = subject & " " & polnum
                If UCase(System.Configuration.ConfigurationManager.AppSettings("TestOrProd")) = "TEST" Then
                    em.EmailFromAddress = "TEST-FNOLconfirmation@IndianaFarmers.com"
                Else
                    em.EmailFromAddress = "FNOLconfirmation@IndianaFarmers.com"
                End If
                em.EmailToAddress = System.Configuration.ConfigurationManager.AppSettings("LossReportingErrorEmail")
                em.EmailBody = bodyString
                em.SendEmail()
            End Using

            Exit Sub
        Catch ex As Exception
            ' Not much to do here if the error email failed
            ' If we tried to put the error handler here it would cause an infinite loop
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Builds and returns an error message string
    ''' </summary>
    ''' <param name="strClassName"></param>
    ''' <param name="strRoutineName"></param>
    ''' <param name="exc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetErrorString(ByVal strClassName As String, ByVal strRoutineName As String, ByRef exc As Exception, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Try
            Return "Error detected in " & strClassName & "(" & strRoutineName & "): " & exc.Message
        Catch ex As Exception
            HandleError(ClassName, "GetErrorString", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "GetErrorString", ex))
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' 12/21/2007-Don Mink (#4899)routine to delete attachments after email is sent
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <remarks></remarks>
    Public Sub DeleteAttachments(ByVal PolicyNumber As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing)
        Dim FileList As String() = Nothing
        Dim FileName As String = Nothing
        Dim FullPath As String = Nothing

        Try
            FileList = Directory.GetFiles(System.Configuration.ConfigurationManager.AppSettings("LossReportingAttachments")) 'Directory.GetFiles("C:\", "*.xml")
            For Each FileName In FileList
                Dim info As FileInfo = New FileInfo(FileName)

                FullPath = info.FullName
                If InStr(FullPath, PolicyNumber) > 0 Then
                    File.Delete(FullPath)
                End If
            Next

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "DeleteAttachments", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "DeleteAttachments", ex))
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Validates the passed phone number
    ''' </summary>
    ''' <param name="phN"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function validatePhoneNumber(ByVal phN As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Dim RegexObj As Regex = New Regex("/^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$/")

        Try
            If RegexObj.IsMatch(phN) Then
                validatePhoneNumber = True
            Else
                validatePhoneNumber = False
            End If

            Return validatePhoneNumber
        Catch ex As Exception
            HandleError(ClassName, "validatePhoneNumber", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "validatePhoneNumber", ex))
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Validates that the passed date is a valid date
    ''' </summary>
    ''' <param name="InDate"></param>
    ''' <remarks></remarks>
    Public Function validdate(ByVal InDate As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Try
            If IsDate(InDate) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError(ClassName, "validdate", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "validdate", ex))
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Formats the passed phone number
    ''' </summary>
    ''' <param name="num"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TSphoneFormat(ByVal num As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim area As String = Nothing
        Dim phone As String = Nothing
        Dim arReturn As Array = Nothing

        Try
            If InStr(num, "-") > 0 Then
                arReturn = Split(num, "-")
                area = arReturn(1).ToString
                phone = arReturn(2).ToString
                If Len(phone) = 7 Then
                    num = "(" & area & ")" & Left(phone, 3) & "-" & Right(phone, 4)
                Else
                    num = "(" & area & ")" & phone
                End If
            Else
                num = num
            End If

            Return num
        Catch ex As Exception
            HandleError(ClassName, "TSphoneformat", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "TSphoneFormat", ex))
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Formats the passed time string
    ''' </summary>
    ''' <param name="time"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TStimeFormat(ByVal time As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim hour As String = Nothing
        Dim minute As String = Nothing
        Dim AMPM As String = Nothing
        Dim arReturn As Array = Nothing

        Try
            If InStr(time, ":") > 0 Then
                arReturn = Split(time, ":")
                hour = arReturn(0).ToString.Trim
                minute = arReturn(1).ToString.Trim
                Select Case hour
                    Case "00"
                        hour = "12"
                        AMPM = "AM"
                    Case "01", "02", "03", "04", "05", "06", "07", "08", "09"
                        hour = Right(hour, 1)
                        AMPM = "AM"
                    Case "10", "11"
                        AMPM = "AM"
                    Case "12"
                        AMPM = "PM"
                    Case Else
                        hour = hour - 12
                        AMPM = "PM"
                End Select
                time = hour & ":" & minute & " " & AMPM
            Else
                time = time
            End If
            Return time
        Catch ex As Exception
            HandleError(ClassName, "TStimeformat", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "TStimeFormat", ex))
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Validates an email address
    ''' </summary>
    ''' <param name="Email"></param>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateEmail(ByVal Email As String, ByVal control As Object, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim EmailError As String = Nothing

        Try
            Using RegEx As New RegularExpressionObject
                If RegEx.IsEmailAddress(Email) = False Then
                    EmailError = "Please enter a valid email address."
                    'SetFocus(control) '1/9/2013 - doesn't work from class file; need to move functionality back to page
                Else
                    EmailError = ""
                End If
            End Using

            Return EmailError
        Catch ex As Exception
            HandleError(ClassName, "ValidateEmail", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "ValidateEmail", ex))
            Return ex.Message
        End Try
    End Function

    ''' <summary>
    ''' Validates a zip code
    ''' </summary>
    ''' <param name="Zip"></param>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateZip(ByVal Zip As String, ByVal control As Object, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim ZipError = ""

        Try
            Using RegEx As New RegularExpressionObject
                If RegEx.IsZipCode(Zip) = False Then
                    ZipError = "Please enter a valid zip code."
                    'SetFocus(control) '1/9/2013 - doesn't work from class file; need to move functionality back to page
                Else
                    ZipError = ""
                End If
            End Using

            Return ZipError
        Catch ex As Exception
            HandleError(ClassName, "ValidateZip", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "ValidateZip", ex))
            Return ex.Message
        End Try
    End Function

    ''' <summary>
    ''' Updates a record in the tbl_auto_reporting table
    ''' </summary>
    ''' <param name="InRec"></param>
    ''' <param name="pg"></param>
    ''' <param name="policynum"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateAuto(ByRef InRec As sp_Insert_Auto_Loss_Parameters, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Dim conn As New SqlConnection()
        Dim cmdSql As SqlCommand = New SqlCommand
        Dim rtn As Integer = -1
        Dim id As String = Nothing
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            ' Open the connection
            conn = New SqlConnection(AppSettings("conn"))
            conn.Open()

            ' Get the id of the existing record
            cmdSql.CommandType = CommandType.Text
            cmdSql.Connection = conn
            cmdSql.CommandText = "SELECT * FROM tbl_auto_reporting WHERE PolicyNumber = '" & policynum & "' AND DateOfLoss = '" & InRec.DateOfLoss & "' AND diaClaimNum IS NULL"
            da.SelectCommand = cmdSql
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found!")
            If tbl.Rows.Count <> 1 Then Throw New Exception("Row count is " & tbl.Rows.Count.ToString & ", expected 1")
            id = tbl.Rows(0)("ID").ToString.Trim
            If Not IsNumeric(id) Then Throw New Exception("ID is not numeric!")

            ' Perform the update
            cmdSql = New SqlCommand()
            cmdSql.Connection = conn
            cmdSql.CommandText = "sp_Update_Auto_Loss"
            cmdSql.CommandType = CommandType.StoredProcedure

            cmdSql.Parameters.AddWithValue("@ID", id)
            cmdSql.Parameters.AddWithValue("@NameID", InRec.NameID.ToString())
            cmdSql.Parameters.AddWithValue("@VehicleID", InRec.VehicleID.ToString())
            cmdSql.Parameters.AddWithValue("@PolicyNumber", InRec.PolicyNumber)
            cmdSql.Parameters.AddWithValue("@DateOfLoss", InRec.DateOfLoss)
            cmdSql.Parameters.AddWithValue("@DescriptionOfLoss", InRec.DescriptionOfLoss)
            cmdSql.Parameters.AddWithValue("@AgencyCode", InRec.AgencyCode)
            cmdSql.Parameters.AddWithValue("@DeductibleAmount", InRec.DeductibleAmount)
            cmdSql.Parameters.AddWithValue("@InsuredResidencePhone", InRec.InsuredResidencePhone)
            cmdSql.Parameters.AddWithValue("@InsuredBusinessPhone", InRec.InsuredBusinessPhone)

            ' INSURED
            If InRec.ContactName IsNot Nothing AndAlso InRec.ContactName.ToString <> "" Then
                cmdSql.Parameters.AddWithValue("@ContactName", InRec.ContactName)
            Else
                Dim contactName As String = ""
                contactName = appendText(InRec.ContactFirstName, InRec.ContactMiddleName, pg, policynum, errlabel, " ")
                contactName = appendText(contactName, InRec.ContactLastName, pg, policynum, errlabel)
                cmdSql.Parameters.AddWithValue("@ContactName", contactName)
            End If

            ' Contact = Insured
            cmdSql.Parameters.AddWithValue("@ContactFirstName", InRec.ContactFirstName)
            cmdSql.Parameters.AddWithValue("@ContactMiddleName", InRec.ContactMiddleName)
            cmdSql.Parameters.AddWithValue("@ContactLastName", InRec.ContactLastName)

            cmdSql.Parameters.AddWithValue("@ContactResidencePhone", InRec.ContactResidencePhone)
            cmdSql.Parameters.AddWithValue("@ContactBusinessPhone", InRec.ContactBusinessPhone)
            cmdSql.Parameters.AddWithValue("@LossLocationAdd1", InRec.LossLocationAdd1)
            cmdSql.Parameters.AddWithValue("@LossLocationCity", InRec.LossLocationCity)
            cmdSql.Parameters.AddWithValue("@LossLocationState", InRec.LossLocationState)
            cmdSql.Parameters.AddWithValue("@LossLocationZip", InRec.LossLocationZip)
            cmdSql.Parameters.AddWithValue("@DescOfLoss", InRec.DescOfLoss)
            cmdSql.Parameters.AddWithValue("@VehicleNumber", InRec.VehicleNumber)
            cmdSql.Parameters.AddWithValue("@OwnerNameAddress", InRec.OwnerNameAddress) ''''''''''veh owner name and address
            cmdSql.Parameters.AddWithValue("@Damage", InRec.Damage)
            cmdSql.Parameters.AddWithValue("@PlateNumber", InRec.PlateNumber)
            cmdSql.Parameters.AddWithValue("@DescribePropertyDamaged", InRec.DescribePropertyDamaged)
            cmdSql.Parameters.AddWithValue("@CompanyAgencyName", InRec.CompanyAgencyName) ''''''''''other insurance comp and policy #

            ' INSURED VEHICLE OWNER
            ' Split vehicle owner name into first, last, middle  MGB 12/19/2011
            Dim ownerName As String = ""
            ownerName = appendText(InRec.OwnerFirstName, InRec.OwnerMiddleName, pg, policynum, errlabel, " ")
            ownerName = appendText(ownerName, InRec.OwnerLastName, pg, policynum, errlabel)
            cmdSql.Parameters.AddWithValue("@OwnerName", ownerName)
            cmdSql.Parameters.AddWithValue("@OwnerFirstName", InRec.OwnerFirstName)
            cmdSql.Parameters.AddWithValue("@OwnerMiddleName", InRec.OwnerMiddleName)
            cmdSql.Parameters.AddWithValue("@OwnerLastName", InRec.OwnerLastName)

            cmdSql.Parameters.AddWithValue("@OwnerBusinessPhone", InRec.OwnerBusinessPhone)
            cmdSql.Parameters.AddWithValue("@OwnerResidencePhone", InRec.OwnerResidencePhone)

            ' OTHER VEHICLE DRIVER
            ' Split other driver name into first, middle, last  MGB 12/19/2011
            cmdSql.Parameters.AddWithValue("@OtherDriverFirstName", InRec.OtherDriverFirstName)
            cmdSql.Parameters.AddWithValue("@OtherDriverMiddleName", InRec.OtherDriverMiddleName)
            cmdSql.Parameters.AddWithValue("@OtherDriverLastName", InRec.OtherDriverLastName)
            Dim otherDriverName As String = ""
            otherDriverName = appendText(InRec.OtherDriverFirstName, InRec.OtherDriverMiddleName, pg, policynum, errlabel, " ")
            otherDriverName = appendText(otherDriverName, InRec.OtherDriverLastName, pg, policynum, errlabel)
            cmdSql.Parameters.AddWithValue("@OtherDriverName", otherDriverName)

            cmdSql.Parameters.AddWithValue("@OtherDriverBusinessPhone", InRec.OtherDriverBusinessPhone)
            cmdSql.Parameters.AddWithValue("@OtherDriverResidencePhone", InRec.OtherDriverResidencePhone)
            cmdSql.Parameters.AddWithValue("@DescribeDamage", InRec.DescribeDamage)
            cmdSql.Parameters.AddWithValue("@InjuredName", InRec.InjuredName) '''''''''''''''injured first and last name
            cmdSql.Parameters.AddWithValue("@InjuredPhone", InRec.InjuredPhone) ''''''''''''either injured home/bus/cell phone
            cmdSql.Parameters.AddWithValue("@InjuredPed", InRec.InjuredPed) ''''''''''''check injured type (True/False)
            cmdSql.Parameters.AddWithValue("@InjuredInsVehicle", InRec.InjuredInsVehicle) ''''''''''''check injured type (True/False)
            cmdSql.Parameters.AddWithValue("@InjuredOtherVehicle", InRec.InjuredOtherVehicle) ''''''''''''check injured type (True/False)
            cmdSql.Parameters.AddWithValue("@InjuredAge", InRec.InjuredAge)
            cmdSql.Parameters.AddWithValue("@InjuredExtent", InRec.InjuredExtent)
            cmdSql.Parameters.AddWithValue("@WitnessNameAddress", InRec.WitnessNameAddress) '''''''''''''witness name and address
            cmdSql.Parameters.AddWithValue("@DriverFirst", InRec.DriverFirst)
            cmdSql.Parameters.AddWithValue("@DriverMI", InRec.DriverMI)
            cmdSql.Parameters.AddWithValue("@DriverLast", InRec.DriverLast)
            cmdSql.Parameters.AddWithValue("@DriverID", InRec.DriverID)
            cmdSql.Parameters.AddWithValue("@Coverages", InRec.Coverages) '''''''''''''''''''''''pre-fill coverages
            cmdSql.Parameters.AddWithValue("@MortgageeID", InRec.MortgageeID) '''''''''?
            cmdSql.Parameters.AddWithValue("@InsuredEmail", InRec.InsuredEmail)
            cmdSql.Parameters.AddWithValue("@Comments", InRec.Comments)
            cmdSql.Parameters.AddWithValue("@InsuredCell", InRec.InsuredCell)
            cmdSql.Parameters.AddWithValue("@DriverCell", InRec.DriverCell)
            cmdSql.Parameters.AddWithValue("@OwnerCell", InRec.OwnerCell)
            cmdSql.Parameters.AddWithValue("@OtherCell", InRec.OtherCell)
            cmdSql.Parameters.AddWithValue("@VehicleLocation", InRec.VehicleLocation)
            cmdSql.Parameters.AddWithValue("@PoliceContacted", InRec.PoliceContacted)
            If InRec.diaClaimNum Is Nothing OrElse InRec.diaClaimNum = "" Then
                cmdSql.Parameters.AddWithValue("@diaClaimNum", "")
            Else
                cmdSql.Parameters.AddWithValue("@diaClaimNum", InRec.diaClaimNum)
            End If

            rtn = cmdSql.ExecuteNonQuery()
            If rtn = 1 Then
                Return True
            Else
                Throw New Exception("No Records updated (return was zero)")
            End If
        Catch ex As Exception
            HandleError(ClassName, "UpdateAuto", ex, pg, policynum, errlabel)
            pg.Session("InsertError") = "yes"
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmdSql.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Calls sp_Insert_Auto_Loss to insert a record into the tbl_auto_reporting table
    ''' Returns a string which is empty if the call was successful, error message if not
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertAuto(ByRef InRec As sp_Insert_Auto_Loss_Parameters, ByRef pg As Page, ByVal policynum As String, ByRef err As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmdSql As SqlCommand = New SqlCommand
        Dim rtn As Integer = -1

        Try
            conn = New SqlConnection(AppSettings("conn"))
            cmdSql.Connection = conn
            cmdSql.CommandText = "sp_Insert_Auto_Loss" '"sp_Insert_TS_FNOL" 'tbl_auto_reporting
            cmdSql.CommandType = CommandType.StoredProcedure

            cmdSql.Parameters.AddWithValue("@NameID", InRec.NameID.ToString())
            cmdSql.Parameters.AddWithValue("@VehicleID", InRec.VehicleID.ToString())
            cmdSql.Parameters.AddWithValue("@PolicyNumber", InRec.PolicyNumber)
            cmdSql.Parameters.AddWithValue("@DateOfLoss", InRec.DateOfLoss)
            cmdSql.Parameters.AddWithValue("@DescriptionOfLoss", InRec.DescriptionOfLoss)
            cmdSql.Parameters.AddWithValue("@AgencyCode", InRec.AgencyCode)
            cmdSql.Parameters.AddWithValue("@DeductibleAmount", InRec.DeductibleAmount)
            cmdSql.Parameters.AddWithValue("@InsuredResidencePhone", InRec.InsuredResidencePhone)
            cmdSql.Parameters.AddWithValue("@InsuredBusinessPhone", InRec.InsuredBusinessPhone)

            ' INSURED
            ' Break down insured name into first, last, middle.  Added middle name.   MGB 12/19/2011
            'cmdSql.Parameters.AddWithValue("@ContactName", GetContactName) ''''''''''''insured first and last
            '1/22/2013 - updated to set ContactName
            If InRec.ContactName IsNot Nothing AndAlso InRec.ContactName.ToString <> "" Then
                cmdSql.Parameters.AddWithValue("@ContactName", InRec.ContactName)
            Else
                Dim contactName As String = ""
                contactName = appendText(InRec.ContactFirstName, InRec.ContactMiddleName, pg, policynum, errlabel, " ")
                contactName = appendText(contactName, InRec.ContactLastName, pg, policynum, errlabel)
                cmdSql.Parameters.AddWithValue("@ContactName", contactName)
            End If
            ' Contact = Insured
            cmdSql.Parameters.AddWithValue("@ContactFirstName", InRec.ContactFirstName)
            cmdSql.Parameters.AddWithValue("@ContactMiddleName", InRec.ContactMiddleName)
            cmdSql.Parameters.AddWithValue("@ContactLastName", InRec.ContactLastName)

            cmdSql.Parameters.AddWithValue("@ContactResidencePhone", InRec.ContactResidencePhone)
            cmdSql.Parameters.AddWithValue("@ContactBusinessPhone", InRec.ContactBusinessPhone)
            cmdSql.Parameters.AddWithValue("@LossLocationAdd1", InRec.LossLocationAdd1)
            cmdSql.Parameters.AddWithValue("@LossLocationCity", InRec.LossLocationCity)
            cmdSql.Parameters.AddWithValue("@LossLocationState", InRec.LossLocationState)
            cmdSql.Parameters.AddWithValue("@LossLocationZip", InRec.LossLocationZip)
            cmdSql.Parameters.AddWithValue("@DescOfLoss", InRec.DescOfLoss)
            cmdSql.Parameters.AddWithValue("@VehicleNumber", InRec.VehicleNumber)
            cmdSql.Parameters.AddWithValue("@OwnerNameAddress", InRec.OwnerNameAddress) ''''''''''veh owner name and address
            cmdSql.Parameters.AddWithValue("@Damage", InRec.Damage)
            cmdSql.Parameters.AddWithValue("@PlateNumber", InRec.PlateNumber)
            cmdSql.Parameters.AddWithValue("@DescribePropertyDamaged", InRec.DescribePropertyDamaged)
            cmdSql.Parameters.AddWithValue("@CompanyAgencyName", InRec.CompanyAgencyName) ''''''''''other insurance comp and policy #

            ' INSURED VEHICLE OWNER
            ' Split vehicle owner name into first, last, middle  MGB 12/19/2011
            'cmdSql.Parameters.Add("@OwnerName", Me.txtInsVehOwnerName.Text)
            'udpated 1/23/2013 to set OwnerName
            Dim ownerName As String = ""
            ownerName = appendText(InRec.OwnerFirstName, InRec.OwnerMiddleName, pg, policynum, errlabel, " ")
            ownerName = appendText(ownerName, InRec.OwnerLastName, pg, policynum, errlabel)
            cmdSql.Parameters.AddWithValue("@OwnerName", ownerName)
            cmdSql.Parameters.AddWithValue("@OwnerFirstName", InRec.OwnerFirstName)
            cmdSql.Parameters.AddWithValue("@OwnerMiddleName", InRec.OwnerMiddleName)
            cmdSql.Parameters.AddWithValue("@OwnerLastName", InRec.OwnerLastName)

            cmdSql.Parameters.AddWithValue("@OwnerBusinessPhone", InRec.OwnerBusinessPhone)
            cmdSql.Parameters.AddWithValue("@OwnerResidencePhone", InRec.OwnerResidencePhone)

            ' OTHER VEHICLE DRIVER
            ' Split other driver name into first, middle, last  MGB 12/19/2011
            cmdSql.Parameters.AddWithValue("@OtherDriverFirstName", InRec.OtherDriverFirstName)
            cmdSql.Parameters.AddWithValue("@OtherDriverMiddleName", InRec.OtherDriverMiddleName)
            cmdSql.Parameters.AddWithValue("@OtherDriverLastName", InRec.OtherDriverLastName)
            'cmdSql.Parameters.Add("@OtherDriverName", GetOtherVehDriverName) '''''''''''other driver first and last name
            'updated 1/23/2013 to set OtherDriverName
            Dim otherDriverName As String = ""
            otherDriverName = appendText(InRec.OtherDriverFirstName, InRec.OtherDriverMiddleName, pg, policynum, errlabel, " ")
            otherDriverName = appendText(otherDriverName, InRec.OtherDriverLastName, pg, policynum, errlabel)
            cmdSql.Parameters.AddWithValue("@OtherDriverName", otherDriverName)

            cmdSql.Parameters.AddWithValue("@OtherDriverBusinessPhone", InRec.OtherDriverBusinessPhone)
            cmdSql.Parameters.AddWithValue("@OtherDriverResidencePhone", InRec.OtherDriverResidencePhone)
            cmdSql.Parameters.AddWithValue("@DescribeDamage", InRec.DescribeDamage)
            cmdSql.Parameters.AddWithValue("@InjuredName", InRec.InjuredName) '''''''''''''''injured first and last name
            cmdSql.Parameters.AddWithValue("@InjuredPhone", InRec.InjuredPhone) ''''''''''''either injured home/bus/cell phone
            cmdSql.Parameters.AddWithValue("@InjuredPed", InRec.InjuredPed) ''''''''''''check injured type (True/False)
            cmdSql.Parameters.AddWithValue("@InjuredInsVehicle", InRec.InjuredInsVehicle) ''''''''''''check injured type (True/False)
            cmdSql.Parameters.AddWithValue("@InjuredOtherVehicle", InRec.InjuredOtherVehicle) ''''''''''''check injured type (True/False)
            cmdSql.Parameters.AddWithValue("@InjuredAge", InRec.InjuredAge)
            cmdSql.Parameters.AddWithValue("@InjuredExtent", InRec.InjuredExtent)
            cmdSql.Parameters.AddWithValue("@WitnessNameAddress", InRec.WitnessNameAddress) '''''''''''''witness name and address
            cmdSql.Parameters.AddWithValue("@DriverFirst", InRec.DriverFirst)
            cmdSql.Parameters.AddWithValue("@DriverMI", InRec.DriverMI)
            cmdSql.Parameters.AddWithValue("@DriverLast", InRec.DriverLast)
            cmdSql.Parameters.AddWithValue("@DriverID", InRec.DriverID)
            cmdSql.Parameters.AddWithValue("@Coverages", InRec.Coverages) '''''''''''''''''''''''pre-fill coverages
            cmdSql.Parameters.AddWithValue("@MortgageeID", InRec.MortgageeID) '''''''''?
            cmdSql.Parameters.AddWithValue("@InsuredEmail", InRec.InsuredEmail)
            cmdSql.Parameters.AddWithValue("@Comments", InRec.Comments)
            cmdSql.Parameters.AddWithValue("@InsuredCell", InRec.InsuredCell)
            cmdSql.Parameters.AddWithValue("@DriverCell", InRec.DriverCell)
            cmdSql.Parameters.AddWithValue("@OwnerCell", InRec.OwnerCell)
            cmdSql.Parameters.AddWithValue("@OtherCell", InRec.OtherCell)
            cmdSql.Parameters.AddWithValue("@VehicleLocation", InRec.VehicleLocation)
            cmdSql.Parameters.AddWithValue("@PoliceContacted", InRec.PoliceContacted)
            If InRec.diaClaimNum Is Nothing OrElse InRec.diaClaimNum = "" Then
                cmdSql.Parameters.AddWithValue("@diaClaimNum", "")
            Else
                cmdSql.Parameters.AddWithValue("@diaClaimNum", InRec.diaClaimNum)
            End If

            conn.Open()
            rtn = cmdSql.ExecuteNonQuery()
            'If rtn = 1 Then
            '    Return ""
            'Else
            '    Return "No Records Added (return was zero)"
            'End If
            Return "" 'updated 1/22/2013 to just return empty string since we only care if the SQL catches an unhandled exception; page will handle Session("InsertError")
        Catch ex As Exception
            err = ex.Message
            'SendErrorEmail("FNOL Error", InRec.PolicyNumber, GetErrorString(ClassName, "InsertAuto", ex))
            'SendErrorEmail("Insert Auto", InRec.PolicyNumber, ex.ToString)
            HandleError(ClassName, "InsertAuto", ex, pg, policynum, errlabel)
            pg.Session("InsertError") = "yes"
            'Return ex.Message
            Return "" 'updated 1/22/2013 to just return empty string since we only care if the SQL catches an unhandled exception; page will handle Session("InsertError")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            cmdSql.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Calls sp_Insert_Property_Loss to insert a property record into the tbl_property_reporting table
    ''' Returns a string which is empty if the call was successful, error message if not
    ''' </summary>
    ''' <remarks></remarks>
    Public Function InsertProperty(ByRef InRec As sp_Insert_Property_Loss_Parameters, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmdSql As SqlCommand = New SqlCommand
        Dim rtn As Integer = -1

        Try
            conn = New SqlConnection(AppSettings("conn"))
            cmdSql.Connection = conn
            cmdSql.CommandText = "sp_Insert_Property_Loss" 'tbl_property_reporting
            cmdSql.CommandType = CommandType.StoredProcedure

            cmdSql.Parameters.AddWithValue("@NameID", InRec.NameID)
            cmdSql.Parameters.AddWithValue("@PolicyNumber", InRec.PolicyNumber)
            cmdSql.Parameters.AddWithValue("@DateOfLoss", InRec.DateOfLoss)
            cmdSql.Parameters.AddWithValue("@KindOfLoss", InRec.KindOfLoss)
            cmdSql.Parameters.AddWithValue("@AgencyCode", InRec.AgencyCode)
            cmdSql.Parameters.AddWithValue("@DeductibleAmount", InRec.DeductibleAmount)
            cmdSql.Parameters.AddWithValue("@InsResPh", InRec.InsResPh)
            cmdSql.Parameters.AddWithValue("@InsBusPh", InRec.InsBusPh)

            ' MGB 12/21/2011  Break down contact name into first, middle, last
            'cmdSql.Parameters.AddWithValue("@ContactName", InRec.ContactName)
            '1/22/2013 - updated to set ContactName using one or the other
            If InRec.ContactName IsNot Nothing AndAlso InRec.ContactName.ToString <> "" Then
                cmdSql.Parameters.AddWithValue("@ContactName", InRec.ContactName)
            Else
                Dim contactName As String = ""
                contactName = appendText(InRec.ContactFirstName, InRec.ContactMiddleName, pg, policynum, errlabel, " ")
                contactName = appendText(contactName, InRec.ContactLastName, pg, policynum, errlabel)
                cmdSql.Parameters.AddWithValue("@ContactName", contactName)
            End If
            cmdSql.Parameters.AddWithValue("@ContactFirstName", InRec.ContactFirstName)
            cmdSql.Parameters.AddWithValue("@ContactMiddleName", InRec.ContactMiddleName)
            cmdSql.Parameters.AddWithValue("@ContactLastName", InRec.ContactLastName)

            cmdSql.Parameters.AddWithValue("@ContactResidencePhone", InRec.ContactResidencePhone)
            cmdSql.Parameters.AddWithValue("@ContactBusinessPhone", InRec.ContactBusinessPhone)
            cmdSql.Parameters.AddWithValue("@LossLocationAdd1", InRec.LossLocationAdd1)
            cmdSql.Parameters.AddWithValue("@LossLocationCity", InRec.LossLocationCity)
            cmdSql.Parameters.AddWithValue("@LossLocationState", InRec.LossLocationState)
            cmdSql.Parameters.AddWithValue("@LossLocationZip", InRec.LossLocationZip)
            cmdSql.Parameters.AddWithValue("@DescOfLoss", InRec.DescOfLoss)
            cmdSql.Parameters.AddWithValue("@FirePoliceReported", InRec.FirePoliceReported)
            cmdSql.Parameters.AddWithValue("@AmountOfLoss", InRec.AmountOfLoss)
            cmdSql.Parameters.AddWithValue("@OtherInsurance", InRec.OtherInsurance)
            cmdSql.Parameters.AddWithValue("@Remarks", InRec.Remarks)
            cmdSql.Parameters.AddWithValue("@ContractorBusinessName", InRec.ContractorBusinessName)
            cmdSql.Parameters.AddWithValue("@ContractorContactName", InRec.ContractorContactName)
            cmdSql.Parameters.AddWithValue("@ContractorBusinessPhone", InRec.ContractorBusinessPhone)
            cmdSql.Parameters.AddWithValue("@ContractorEmail", InRec.ContractorEmail)
            cmdSql.Parameters.AddWithValue("@ContractorRemarks", InRec.ContractorRemarks)

            If InRec.AdjusterAssigned IsNot Nothing OrElse InRec.AdjusterAssigned <> "" Then
                cmdSql.Parameters.AddWithValue("@AdjusterAssigned", InRec.AdjusterAssigned)
            Else
                cmdSql.Parameters.AddWithValue("@AdjusterAssigned", "")
            End If
            cmdSql.Parameters.AddWithValue("@ReportedBy", InRec.ReportedBy)
            cmdSql.Parameters.AddWithValue("@diaClaimNum", InRec.diaClaimNum)

            conn.Open()
            rtn = cmdSql.ExecuteNonQuery()
            'If rtn = 1 Then
            '    Return ""
            'Else
            '    Throw New Exception("No Records Added (return was zero)")
            'End If
            Return "" 'updated 1/22/2013 to just return empty string since we only care if the SQL catches an unhandled exception; page will handle Session("InsertError")
        Catch ex As Exception
            HandleError(ClassName, "InsertProperty", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", InRec.PolicyNumber, GetErrorString(ClassName, "InsertProperty", ex))
            'SendErrorEmail("Insert_Property", InRec.PolicyNumber, ex.ToString)
            pg.Session("InsertError") = "yes"
            'Return ex.Message
            Return "" 'updated 1/22/2013 to just return empty string since we only care if the SQL catches an unhandled exception; page will handle Session("InsertError")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            cmdSql.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates a record in tbl_property_reporting
    ''' </summary>
    ''' <param name="InRec"></param>
    ''' <param name="pg"></param>
    ''' <param name="policynum"></param>
    ''' <param name="err"></param>
    ''' <param name="errlabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateProperty(ByRef InRec As sp_Insert_Property_Loss_Parameters, ByRef pg As Page, ByVal policynum As String, ByRef err As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmdSql As SqlCommand = New SqlCommand
        Dim rtn As Integer = -1
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable
        Dim id As String = Nothing

        Try
            conn = New SqlConnection(AppSettings("conn"))
            conn.Open()

            ' Get the id of the existing record
            cmdSql.CommandType = CommandType.Text
            cmdSql.Connection = conn
            cmdSql.CommandText = "SELECT * FROM tbl_property_reporting WHERE PolicyNumber = '" & policynum & "' AND DateOfLoss = '" & InRec.DateOfLoss & "' AND diaClaimNum IS NULL"
            da.SelectCommand = cmdSql
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found!")
            If tbl.Rows.Count <> 1 Then Throw New Exception("Row count is " & tbl.Rows.Count.ToString & ", expected 1")
            id = tbl.Rows(0)("ID").ToString.Trim
            If Not IsNumeric(id) Then Throw New Exception("ID is not numeric!")

            cmdSql = New SqlCommand()
            cmdSql.Connection = conn
            cmdSql.CommandText = "sp_Update_Property_Loss" 'tbl_property_reporting
            cmdSql.CommandType = CommandType.StoredProcedure

            cmdSql.Parameters.AddWithValue("@ID", id)
            cmdSql.Parameters.AddWithValue("@NameID", InRec.NameID)
            cmdSql.Parameters.AddWithValue("@PolicyNumber", InRec.PolicyNumber)
            cmdSql.Parameters.AddWithValue("@DateOfLoss", InRec.DateOfLoss)
            cmdSql.Parameters.AddWithValue("@KindOfLoss", InRec.KindOfLoss)
            cmdSql.Parameters.AddWithValue("@AgencyCode", InRec.AgencyCode)
            cmdSql.Parameters.AddWithValue("@DeductibleAmount", InRec.DeductibleAmount)
            cmdSql.Parameters.AddWithValue("@InsResPh", InRec.InsResPh)
            cmdSql.Parameters.AddWithValue("@InsBusPh", InRec.InsBusPh)

            If InRec.ContactName IsNot Nothing AndAlso InRec.ContactName.ToString <> "" Then
                cmdSql.Parameters.AddWithValue("@ContactName", InRec.ContactName)
            Else
                Dim contactName As String = ""
                contactName = appendText(InRec.ContactFirstName, InRec.ContactMiddleName, pg, policynum, errlabel, " ")
                contactName = appendText(contactName, InRec.ContactLastName, pg, policynum, errlabel)
                cmdSql.Parameters.AddWithValue("@ContactName", contactName)
            End If
            cmdSql.Parameters.AddWithValue("@ContactFirstName", InRec.ContactFirstName)
            cmdSql.Parameters.AddWithValue("@ContactMiddleName", InRec.ContactMiddleName)
            cmdSql.Parameters.AddWithValue("@ContactLastName", InRec.ContactLastName)

            cmdSql.Parameters.AddWithValue("@ContactResidencePhone", InRec.ContactResidencePhone)
            cmdSql.Parameters.AddWithValue("@ContactBusinessPhone", InRec.ContactBusinessPhone)
            cmdSql.Parameters.AddWithValue("@LossLocationAdd1", InRec.LossLocationAdd1)
            cmdSql.Parameters.AddWithValue("@LossLocationCity", InRec.LossLocationCity)
            cmdSql.Parameters.AddWithValue("@LossLocationState", InRec.LossLocationState)
            cmdSql.Parameters.AddWithValue("@LossLocationZip", InRec.LossLocationZip)
            cmdSql.Parameters.AddWithValue("@DescOfLoss", InRec.DescOfLoss)
            cmdSql.Parameters.AddWithValue("@FirePoliceReported", InRec.FirePoliceReported)
            cmdSql.Parameters.AddWithValue("@AmountOfLoss", InRec.AmountOfLoss)
            cmdSql.Parameters.AddWithValue("@OtherInsurance", InRec.OtherInsurance)
            cmdSql.Parameters.AddWithValue("@Remarks", InRec.Remarks)
            cmdSql.Parameters.AddWithValue("@ContractorBusinessName", InRec.ContractorBusinessName)
            cmdSql.Parameters.AddWithValue("@ContractorContactName", InRec.ContractorContactName)
            cmdSql.Parameters.AddWithValue("@ContractorBusinessPhone", InRec.ContractorBusinessPhone)
            cmdSql.Parameters.AddWithValue("@ContractorEmail", InRec.ContractorEmail)
            cmdSql.Parameters.AddWithValue("@ContractorRemarks", InRec.ContractorRemarks)
            If InRec.AdjusterAssigned IsNot Nothing OrElse InRec.AdjusterAssigned <> "" Then
                cmdSql.Parameters.AddWithValue("@AdjusterAssigned", InRec.AdjusterAssigned)
            Else
                cmdSql.Parameters.AddWithValue("@AdjusterAssigned", "")
            End If
            cmdSql.Parameters.AddWithValue("@ReportedBy", InRec.ReportedBy)
            cmdSql.Parameters.AddWithValue("@diaClaimNum", InRec.diaClaimNum)

            rtn = cmdSql.ExecuteNonQuery()
            If rtn = 1 Then
                Return True
            Else
                Throw New Exception("No Records updated (return was zero)")
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "UpdateProperty", ex, pg, policynum, errlabel)
            pg.Session("InsertError") = "yes"
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmdSql.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Calls sp_Insert_General_Loss to insert a record into the tbl_general_reporting table
    ''' </summary>
    ''' <remarks></remarks>
    Public Function InsertLiability(ByRef InRec As sp_Insert_General_Loss_Parameters, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmdSql As SqlCommand = New SqlCommand
        Dim rtn As Integer = -1

        Try
            conn = New SqlConnection(AppSettings("conn"))
            cmdSql.Connection = conn
            cmdSql.CommandText = "sp_Insert_General_Loss"
            cmdSql.CommandType = CommandType.StoredProcedure

            cmdSql.Parameters.AddWithValue("@PolicyNumber ", InRec.PolicyNumber)
            cmdSql.Parameters.AddWithValue("@DateOfLoss", InRec.DateOfLoss)
            'cmdSql.Parameters.AddWithValue("@TimeOfLoss", Me.txtLossTime.Text)
            cmdSql.Parameters.AddWithValue("@AgentCode", InRec.AgentCode)
            cmdSql.Parameters.AddWithValue("@InsuredID", InRec.InsuredID.ToString())
            cmdSql.Parameters.AddWithValue("@InsuredResidencePhone", InRec.InsuredResidencePhone)
            cmdSql.Parameters.AddWithValue("@InsuredBusinessPhone", InRec.InsuredBusinessPhone)
            cmdSql.Parameters.AddWithValue("@InsuredCellPhone", InRec.InsuredCellPhone)
            cmdSql.Parameters.AddWithValue("@InsuredEmailAddress ", InRec.InsuredEmailAddress)
            'cmdSql.Parameters.AddWithValue("@PersonToContact", InRec.PersonToContact)
            '1/22/2013 - updated to set ContactName using one or the other
            If InRec.PersonToContact IsNot Nothing AndAlso InRec.PersonToContact.ToString <> "" Then
                cmdSql.Parameters.AddWithValue("@PersonToContact", InRec.PersonToContact)
            Else
                Dim contactName As String = ""
                contactName = appendText(InRec.ContactFirstName, InRec.ContactMiddleName, pg, policynum, errlabel, " ")
                contactName = appendText(contactName, InRec.ContactLastName, pg, policynum, errlabel)
                cmdSql.Parameters.AddWithValue("@PersonToContact", contactName)
            End If
            cmdSql.Parameters.AddWithValue("@LocationOfLossAddress", InRec.LocationOfLossAddress)
            cmdSql.Parameters.AddWithValue("@LocationOfLossCity", InRec.LocationOfLossCity)
            cmdSql.Parameters.AddWithValue("@LocationOfLossState", InRec.LocationOfLossState)
            cmdSql.Parameters.AddWithValue("@LocationOfLossZipCode", InRec.LocationOfLossZipCode)
            cmdSql.Parameters.AddWithValue("@DescriptionOfLossEvent", InRec.DescriptionOfLossEvent)
            cmdSql.Parameters.AddWithValue("@AuthoritiesContacted", InRec.AuthoritiesContacted)
            cmdSql.Parameters.AddWithValue("@PropertyDamageInjury", InRec.PropertyDamageInjury)
            cmdSql.Parameters.AddWithValue("@ReportedBy", InRec.ReportedBy)
            cmdSql.Parameters.AddWithValue("@diaClaimNum", InRec.diaClaimNum)

            conn.Open()
            rtn = cmdSql.ExecuteNonQuery()

            'If rtn = 1 Then
            '    Return ""
            'Else
            '    Throw New Exception("No Records Added (return was zero)")
            'End If
            Return "" 'updated 1/22/2013 to just return empty string since we only care if the SQL catches an unhandled exception; page will handle Session("InsertError")
        Catch ex As Exception
            'SendErrorEmail("Insert_Liability", InRec.PolicyNumber, ex.ToString)
            HandleError(ClassName, "InsertLiability", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", InRec.PolicyNumber, GetErrorString(ClassName, "InsertLiability", ex))
            pg.Session("InsertError") = "yes"
            'Return ex.Message
            Return "" 'updated 1/22/2013 to just return empty string since we only care if the SQL catches an unhandled exception; page will handle Session("InsertError")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            cmdSql.Dispose()
        End Try
    End Function

    Public Function UpdateLiability(ByRef InRec As sp_Insert_General_Loss_Parameters, ByRef pg As Page, ByVal policynum As String, ByRef err As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Dim conn As New SqlConnection()
        Dim cmdSql As SqlCommand = New SqlCommand
        Dim rtn As Integer = -1
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable
        Dim id As String = Nothing

        Try
            conn = New SqlConnection(AppSettings("conn"))
            conn.Open()

            ' Get the id of the existing record
            cmdSql.CommandType = CommandType.Text
            cmdSql.Connection = conn
            cmdSql.CommandText = "SELECT * FROM tbl_general_reporting WHERE PolicyNumber = '" & policynum & "' AND DateOfLoss = '" & InRec.DateOfLoss & "' AND diaClaimNum IS NULL"
            da.SelectCommand = cmdSql
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found!")
            If tbl.Rows.Count <> 1 Then Throw New Exception("Row count is " & tbl.Rows.Count.ToString & ", expected 1")
            id = tbl.Rows(0)("ID").ToString.Trim
            If Not IsNumeric(id) Then Throw New Exception("ID is not numeric!")

            cmdSql = New SqlCommand()
            cmdSql.Connection = conn
            cmdSql.CommandText = "sp_Update_General_Loss"
            cmdSql.CommandType = CommandType.StoredProcedure

            cmdSql.Parameters.AddWithValue("@ID ", id)
            cmdSql.Parameters.AddWithValue("@PolicyNumber ", InRec.PolicyNumber)
            cmdSql.Parameters.AddWithValue("@DateOfLoss", InRec.DateOfLoss)
            cmdSql.Parameters.AddWithValue("@AgentCode", InRec.AgentCode)
            cmdSql.Parameters.AddWithValue("@InsuredID", InRec.InsuredID)
            cmdSql.Parameters.AddWithValue("@InsuredResidencePhone", InRec.InsuredResidencePhone)
            cmdSql.Parameters.AddWithValue("@InsuredBusinessPhone", InRec.InsuredBusinessPhone)
            cmdSql.Parameters.AddWithValue("@InsuredCellPhone", InRec.InsuredCellPhone)
            cmdSql.Parameters.AddWithValue("@InsuredEmailAddress ", InRec.InsuredEmailAddress)
            If InRec.PersonToContact IsNot Nothing AndAlso InRec.PersonToContact.ToString <> "" Then
                cmdSql.Parameters.AddWithValue("@PersonToContact", InRec.PersonToContact)
            Else
                Dim contactName As String = ""
                contactName = appendText(InRec.ContactFirstName, InRec.ContactMiddleName, pg, policynum, errlabel, " ")
                contactName = appendText(contactName, InRec.ContactLastName, pg, policynum, errlabel)
                cmdSql.Parameters.AddWithValue("@PersonToContact", contactName)
            End If
            cmdSql.Parameters.AddWithValue("@LocationOfLossAddress", InRec.LocationOfLossAddress)
            cmdSql.Parameters.AddWithValue("@LocationOfLossCity", InRec.LocationOfLossCity)
            cmdSql.Parameters.AddWithValue("@LocationOfLossState", InRec.LocationOfLossState)
            cmdSql.Parameters.AddWithValue("@LocationOfLossZipCode", InRec.LocationOfLossZipCode)
            cmdSql.Parameters.AddWithValue("@DescriptionOfLossEvent", InRec.DescriptionOfLossEvent)
            cmdSql.Parameters.AddWithValue("@AuthoritiesContacted", InRec.AuthoritiesContacted)
            cmdSql.Parameters.AddWithValue("@PropertyDamageInjury", InRec.PropertyDamageInjury)
            cmdSql.Parameters.AddWithValue("@ReportedBy", InRec.ReportedBy)
            cmdSql.Parameters.AddWithValue("@diaClaimNum", InRec.diaClaimNum)

            rtn = cmdSql.ExecuteNonQuery()
            If rtn = 1 Then
                Return True
            Else
                Throw New Exception("No Records updated (return was zero)")
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "UpdateLiability", ex, pg, policynum, errlabel)
            pg.Session("InsertError") = "yes"
            Return False
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmdSql.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Calls sp_Insert_ManualDraft to insert a record into the tbl_ManualDraft_Submissions table
    ''' </summary>
    ''' <param name="InRec"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertDraft(ByRef InRec As sp_Insert_ManualDraft_Parameters, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim params As New ArrayList
        Dim emailinfo As New EmailInfo_Structure_FNOLInputPage()
        Dim draftNumber As SqlClient.SqlParameter = Nothing
        Dim submitBy As SqlClient.SqlParameter = Nothing
        Dim policyNumber As SqlClient.SqlParameter = Nothing
        Dim draftAmount As SqlClient.SqlParameter = Nothing
        Dim userId As SqlClient.SqlParameter = Nothing
        Dim claimNumber As SqlClient.SqlParameter = Nothing
        Dim draftDate As SqlClient.SqlParameter = Nothing
        Dim void As SqlClient.SqlParameter = Nothing
        Dim pay As SqlClient.SqlParameter = Nothing

        Try
            draftNumber = New SqlClient.SqlParameter("@draftNumber", InRec.DraftNumber)
            submitBy = New SqlClient.SqlParameter("@submittedBy", InRec.SubmittedBy)
            policyNumber = New SqlClient.SqlParameter("@policyNumber", InRec.PolicyNumber)
            draftAmount = New SqlClient.SqlParameter("@draftAmount", InRec.DraftAmount)
            userId = New SqlClient.SqlParameter("@usrId", InRec.UsrID)
            claimNumber = New SqlClient.SqlParameter("@claimNumber", InRec.ClaimNumber)
            draftDate = New SqlClient.SqlParameter("@draftDate", Date.Now)
            void = New SqlClient.SqlParameter("@void", 0)
            pay = New SqlClient.SqlParameter("@payee", InRec.Payee)

            params.Add(draftNumber)
            params.Add(submitBy)
            params.Add(policyNumber)
            params.Add(draftAmount)
            params.Add(userId)
            params.Add(claimNumber)
            params.Add(draftDate)
            params.Add(void)
            params.Add(pay)

            Using sqlEx As New SQLexecuteObject
                sqlEx.connection = AppSettings("conn")
                sqlEx.queryOrStoredProc = "sp_insert_ManualDraft"
                sqlEx.inputParameters = params
                sqlEx.ExecuteStatement()

                If sqlEx.hasError = True Then
                    Return sqlEx.errorMsg
                Else
                    Return ""
                End If
            End Using
        Catch ex As Exception
            HandleError(ClassName, "InsertDraft", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", InRec.PolicyNumber, GetErrorString(ClassName, "InsertDraft", ex))
            Return ex.Message
        End Try
    End Function

    ''' <summary>
    ''' Returns diamond state id for the passed state name or abbreviation
    ''' </summary>
    ''' <param name="st"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDiamondStateID(ByVal st As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim sqsel As SQLselectObject = Nothing
        Dim dt As DataTable = Nothing

        Try
            sqsel = New SQLselectObject(strConnDiamond, "SELECT state_id FROM State WHERE state = '" & st & "' OR statename = '" & st & "'")
            dt = sqsel.GetDataTable()

            If sqsel.hasError Then
                Throw New Exception("Get State Error: " & sqsel.errorMsg)
                'SendErrorEmail("Get State ERROR!", "", sqsel.errorMsg)
                Return "0"
            Else
                If dt.Rows.Count > 0 Then
                    Return dt.Rows(0).Item("state_id").ToString
                Else
                    Return "0"
                End If
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetDiamondStateID", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "GetState", ex))
            Return "0"
        End Try
    End Function

    ''' <summary>
    ''' Returns diamond state abbreviation for the passed state id
    ''' </summary>
    ''' <param name="StateId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDiamondStateAbbrev(ByVal StateId As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
        Dim sqsel As SQLselectObject = Nothing
        Dim dt As DataTable = Nothing

        Try
            sqsel = New SQLselectObject(strConnDiamond, "SELECT [state] FROM [State] WHERE state_id = " & StateId)
            dt = sqsel.GetDataTable()

            If sqsel.hasError Then
                Throw New Exception("GetDiamondStateAbbrev Error: " & sqsel.errorMsg)
            Else
                If dt.Rows.Count > 0 Then
                    Return dt.Rows(0).Item("state").ToString
                Else
                    Return "err"
                End If
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetDiamondStateAbbrev", ex, pg, policynum, errlabel)
            Return "err"
        End Try
    End Function

    ''' <summary>
    ''' Validates a phone number
    ''' Returns true string if valid
    ''' Returns false if invalid
    ''' </summary>
    ''' <param name="strPhone"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsValidPhoneNumber(ByVal strPhone As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As Boolean
        Try
            If strPhone = "" Then Return False

            If strPhone.Substring(0, 1) = "(" _
                    AndAlso strPhone.Substring(4, 1) = ")" _
                           AndAlso strPhone.Substring(8, 1) = "-" Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError(ClassName, "IsValidPhoneNumber", ex, pg, policynum, errlabel)
            'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "IsValidPhoneNumber", ex))
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Formats a phone number
    ''' </summary>
    ''' <param name="strPhone"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function FormatPhoneNumber(ByVal strPhone As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing) As String
    '    Dim NumCnt As Integer = 0
    '    Dim DashPos As Integer = 0
    '    Dim DashPos2 As Integer = 0
    '    Dim Paren1Pos As Integer = 0
    '    Dim Paren2Pos As Integer = 0
    '    Dim SpacePos As Integer = 0
    '    Dim SpacePos2 As Integer = 0

    '    Try
    '        ' Make sure something was passed
    '        If strPhone Is Nothing OrElse strPhone = "" Then Return Nothing

    '        strPhone = strPhone.Trim()
    '        ' Make sure we have the minimum amount of digits for a phone number
    '        If strPhone.Length < 7 Then Return Nothing
    '        ' There has to be at least 7 numeric characters
    '        For i As Integer = 0 To strPhone.Length - 1
    '            If IsNumeric(strPhone.Substring(i, 1)) Then NumCnt = NumCnt + 1
    '        Next
    '        If NumCnt < 7 Then Return Nothing

    '        ' Figure out what format the phone number is in
    '        DashPos = strPhone.IndexOf("-")
    '        DashPos2 = strPhone.IndexOf("-", DashPos + 1)
    '        Paren1Pos = strPhone.IndexOf("(")
    '        Paren2Pos = strPhone.IndexOf(")", Paren1Pos + 1)
    '        SpacePos = strPhone.IndexOf(" ")
    '        SpacePos2 = strPhone.IndexOf(" ", SpacePos + 1)

    '        Select Case NumCnt
    '            Case 7
    '                ' 999-9999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And DashPos = 3 And DashPos2 = -1 And SpacePos = -1 And SpacePos2 = -1 Then
    '                    ' No formatting necessary, just add a dummy area code
    '                    Return "(000)" & strPhone
    '                End If
    '                ' 999 9999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And DashPos = -1 And DashPos2 = -1 And SpacePos = 3 And SpacePos2 = -1 Then
    '                    Return "(000)" & strPhone.Substring(0, 3) & "-" & strPhone.Substring(4, 4)
    '                End If
    '                ' 9999999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And DashPos = -1 And DashPos2 = -1 And SpacePos = -1 And SpacePos2 = -1 Then
    '                    Return "(000)" & strPhone.Substring(0, 3) & "-" & strPhone.Substring(3, 4)
    '                End If
    '            Case 10
    '                ' (999)9999999
    '                If Paren1Pos = 0 And Paren2Pos = 4 And SpacePos = -1 And SpacePos2 = -1 And DashPos = -1 And DashPos2 = -1 Then
    '                    Return strPhone.Substring(0, 5) & strPhone.Substring(5, 3) & "-" & strPhone.Substring(8, 4)
    '                End If
    '                ' (999) 9999999
    '                If Paren1Pos = 0 And Paren2Pos = 4 And SpacePos = 5 And SpacePos2 = -1 And DashPos = -1 And DashPos2 = -1 Then
    '                    Return strPhone.Substring(0, 5) & strPhone.Substring(6, 3) & "-" & strPhone.Substring(9, 4)
    '                End If
    '                ' 999-9999999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And SpacePos = -1 And SpacePos2 = -1 And DashPos = 3 And DashPos2 = -1 Then
    '                    Return "(" & strPhone.Substring(0, 3) & ")" & strPhone.Substring(4, 3) & "-" & strPhone.Substring(7, 4)
    '                End If
    '                ' (999) 999-9999
    '                If Paren1Pos = 0 And Paren2Pos = 4 And SpacePos = 5 And SpacePos2 = -1 And DashPos = 9 And DashPos2 = -1 Then
    '                    Return strPhone.Replace(" ", "")
    '                End If
    '                ' (999)999-9999 (this is the correct format)
    '                If Paren1Pos = 0 And Paren2Pos = 4 And SpacePos = -1 And DashPos = 8 And DashPos2 = -1 Then Return strPhone
    '                ' 9999999999
    '                If IsNumeric(strPhone) And strPhone.Length = 10 And DashPos = -1 And DashPos2 = -1 And SpacePos = -1 And Paren1Pos = -1 And Paren2Pos = -1 Then
    '                    Return "(" & strPhone.Substring(0, 3) & ")" & strPhone.Substring(3, 3) & "-" & strPhone.Substring(6, 4)
    '                End If
    '                ' 999-999-9999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And SpacePos = -1 And DashPos = 3 And DashPos2 = 7 Then
    '                    Return "(" & strPhone.Substring(0, 3) & ")" & strPhone.Substring(4, 3) & "-" & strPhone.Substring(8, 4)
    '                End If
    '                ' 999 999-9999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And SpacePos = 3 And DashPos = 7 And DashPos2 = -1 Then
    '                    Return "(" & strPhone.Substring(0, 3) & ")" & strPhone.Substring(4)
    '                End If
    '                ' 999 999 9999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And DashPos = -1 And DashPos2 = -1 And SpacePos = 3 And SpacePos2 = 7 Then
    '                    Return "(" & strPhone.Substring(0, 3) & ")" & strPhone.Substring(4, 3) & "-" & strPhone.Substring(8, 4)
    '                End If
    '                ' 999999-9999
    '                If Paren1Pos = -1 And Paren2Pos = -1 And SpacePos = -1 And SpacePos2 = -1 And DashPos = 6 And DashPos2 = -1 Then
    '                    Return "(" & strPhone.Substring(0, 3) & ")" & strPhone.Substring(3, 3) & "-" & strPhone.Substring(7, 4)
    '                End If
    '        End Select

    '        ' If we got here, the phone number is not in a recognized format
    '        Return Nothing
    '    Catch ex As Exception
    '        HandleError(ClassName, "FormatPhoneNumber", ex, pg, policynum, errlabel)
    '        'SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "FormatPhoneNumber", ex))
    '        Return Nothing
    '    End Try
    'End Function

    'added 1/22/2013
    Public Function appendText(ByVal existingText As String, ByVal addText As String, ByRef pg As Page, ByVal policynum As String, Optional ByRef errlabel As Label = Nothing, Optional ByVal splitter As String = " ", Optional ByVal appendErrorTextInFrontWithBreak As Boolean = False, Optional ByVal errorPrefix As String = "") As String
        Try
            appendText = ""

            If existingText <> "" Then
                appendText = existingText
            ElseIf appendErrorTextInFrontWithBreak = True Then
                If errorPrefix = "" Then
                    errorPrefix = "The following errors were encountered:"
                End If
                errorPrefix = errorPrefix.Replace("<br>", "") & "<br>"
                appendText = errorPrefix
            End If

            If addText <> "" Then
                If appendErrorTextInFrontWithBreak = True AndAlso appendText = errorPrefix Then
                    appendText &= addText
                ElseIf appendText <> "" Then
                    appendText &= splitter & addText
                Else
                    appendText = addText
                End If
            End If

            Return appendText
        Catch ex As Exception
            HandleError(ClassName, "appendText", ex, pg, policynum, errlabel)
            Return ""
        End Try
    End Function

    Public Function Get_FNOLType_Id(ByVal FNOLType As String, ByRef pg As Page, ByVal PolNum As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim id As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT FNOLType_Id FROM tbl_FNOLType WHERE FNOLType = '" & FNOLType & "'"
            id = cmd.ExecuteScalar

            If id Is Nothing Or Not IsNumeric(id) Then
                Throw New Exception("Error getting FNOLType Id")
            Else
                Return id.ToString()
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "Get_FNOLType_Id", ex, pg, PolNum)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Public Function Get_PersonType_Id(ByVal PersonType As PersonsType_enum, ByRef pg As Page, ByVal PolNum As String, ByRef err As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim id As Object = Nothing

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = '"
            Select Case PersonType
                Case PersonsType_enum.Injured
                    cmd.CommandText += "Injured'"
                Case PersonsType_enum.OtherVehicleDriver
                    cmd.CommandText += "Other Vehicle Driver'"
                Case PersonsType_enum.OtherVehicleOwner
                    cmd.CommandText += "Other Vehicle Owner'"
                Case PersonsType_enum.Witness
                    cmd.CommandText += "Witness'"
                Case Else
                    Throw New Exception("Invalid person type passed")
            End Select
            id = cmd.ExecuteScalar

            If id Is Nothing Or Not IsNumeric(id) Then
                Throw New Exception("Error getting PersonType Id")
            Else
                Return id.ToString()
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "Get_PersonType_Id", ex, pg, PolNum)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Public Function InsertPersonsRecord(ByRef rec As PersonsRecord_structure, ByRef FNOL_Id As String, ByRef PersonsType_Id As String, ByRef conn As SqlConnection, ByRef txn As SqlTransaction, ByRef pg As Page, ByRef polnum As String, ByRef err As String) As Boolean
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1

        Try
            If conn Is Nothing OrElse conn.State <> ConnectionState.Open Then Throw New Exception("Connection string has not been initialized")

            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "sp_Insert_tbl_persons"
            cmd.Transaction = txn

            cmd.Parameters.AddWithValue("@PersonType_Id", PersonsType_Id)
            cmd.Parameters.AddWithValue("@FNOL_Id", FNOL_Id)
            cmd.Parameters.AddWithValue("@FirstName", rec.FirstName)
            If rec.MiddleName IsNot Nothing Then cmd.Parameters.AddWithValue("@MiddleName", rec.MiddleName)
            cmd.Parameters.AddWithValue("@LastName", rec.LastName)
            If rec.HomePhone IsNot Nothing Then cmd.Parameters.AddWithValue("@HomePhone", rec.HomePhone)
            If rec.BusinessPhone IsNot Nothing Then cmd.Parameters.AddWithValue("@BusinessPhone", rec.BusinessPhone)
            If rec.CellPhone IsNot Nothing Then cmd.Parameters.AddWithValue("@CellPhone", rec.CellPhone)
            If rec.FAX IsNot Nothing Then cmd.Parameters.AddWithValue("@FAX", rec.FAX)
            If rec.Address IsNot Nothing Then cmd.Parameters.AddWithValue("@Address", rec.Address)
            If rec.City IsNot Nothing Then cmd.Parameters.AddWithValue("@City", rec.City)
            If rec.State IsNot Nothing Then cmd.Parameters.AddWithValue("@State", rec.State)
            If rec.Zip IsNot Nothing Then cmd.Parameters.AddWithValue("@Zip", rec.Zip)
            If rec.Email IsNot Nothing Then cmd.Parameters.AddWithValue("@Email", rec.Email)
            If rec.InjuryType IsNot Nothing Then cmd.Parameters.AddWithValue("@InjuryType", rec.InjuryType)
            If rec.InjuredAge IsNot Nothing Then cmd.Parameters.AddWithValue("@InjuredAge", rec.InjuredAge)
            If rec.InjuryDescription IsNot Nothing Then cmd.Parameters.AddWithValue("@InjuryDescription", rec.InjuryDescription)
            'new injured fields zshanks 05/07/18
            If rec.InjuredOccupation IsNot Nothing Then cmd.Parameters.AddWithValue("@InjuredOccupation", rec.InjuredOccupation)
            If rec.InjuredDoing IsNot Nothing Then cmd.Parameters.AddWithValue("@InjuredDoing", rec.InjuredDoing)
            If rec.InjuredTaken IsNot Nothing Then cmd.Parameters.AddWithValue("@InjuredTaken", rec.InjuredTaken)

            rtn = cmd.ExecuteNonQuery()

            If rtn = 1 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "InsertPersonsRecord", ex, pg, polnum)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Returns a record from the tbl_FNOL table
    ''' </summary>
    ''' <param name="FNOL_Id"></param>
    ''' <param name="PolNum"></param>
    ''' <param name="pg"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFNOL(ByVal FNOL_Id As String, ByVal PolNum As String, ByRef pg As Page, ByRef err As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM tbl_FNOL WHERE FNOL_id = " & FNOL_Id
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl.Rows(0)
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOL", ex, pg, PolNum)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    Public Function GetFNOLPersonsList(ByVal PersonType As PersonsType_enum, ByVal FNOL_Id As String, ByRef pg As Page, ByVal polnum As String, ByRef err As String) As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim ptid As String = Nothing

        Try
            ptid = Get_PersonType_Id(PersonType, pg, polnum, err)
            If ptid Is Nothing Then
                If err IsNot Nothing AndAlso err.Trim <> String.Empty Then
                    Throw New Exception(err)
                Else
                    Throw New Exception("Unknown error getting PersonType_id")
                End If
            End If

            conn.ConnectionString = strConnFNOL
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "sp_GetPersons"
            cmd.Parameters.AddWithValue("@PersonType_id", ptid)
            cmd.Parameters.AddWithValue("@FNOL_id", FNOL_Id)
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
                Return Nothing
            Else
                Return tbl
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "GetFNOLPersonsList", ex, pg, polnum)
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Formats a phone number into the required Diamond format - (111)111-1111
    ''' 
    ''' By default will return NOTHING if an error occurs, set the ReturnNothingOnError flag to false
    ''' if you want to return the original passed phone number instead
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="PhoneNumberToFormat"></param>
    ''' <param name="err"></param>
    ''' <param name="ReturnNothingOnError"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatPhoneNumberForDiamond(ByRef pg As Page, ByVal PhoneNumberToFormat As String, ByRef err As String, Optional ReturnNothingOnError As Boolean = True) As String
        Dim FormattedPh As String = String.Empty
        Dim temp As String = String.Empty

        Try
            ' Validate
            If PhoneNumberToFormat Is Nothing Then Throw New Exception("Passed phone number is NOTHING")
            If PhoneNumberToFormat.Trim.Length < 7 Then Throw New Exception("Passed phone number is less than 7 characters long")

            ' Remove all spaces, parens, dashes, periods, etc
            ' When we're done here we want a 10-digit number
            temp = PhoneNumberToFormat.Replace("+1-", "")
            temp = temp.Replace(" ", "")
            temp = temp.Trim()
            temp = temp.Replace("-", "")
            temp = temp.Replace("(", "")
            temp = temp.Replace(")", "")
            temp = temp.Replace(".", "")

            ' Hopefully we have all numbers now
            If Not IsNumeric(temp) Then Throw New Exception("Passed phone number has unexpected characters in it")

            ' Format according to length.  Diamond has a max length of 13 and likes the format (999)999-9999
            Select Case temp.Length
                Case 10
                    FormattedPh = "(" & temp.Substring(0, 3) & ")" & temp.Substring(3, 3) & "-" & temp.Substring(6, 4)
                    Exit Select
                Case 7
                    FormattedPh = temp.Substring(0, 3) & "-" & temp.Substring(3, 4)
                Case Else
                    Throw New Exception("Passed phone number was in an unexpected format!")
            End Select

            Return FormattedPh
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "FormatPhoneNumberForDiamond", ex, pg, "")
            If ReturnNothingOnError Then
                Return Nothing
            Else
                Return PhoneNumberToFormat
            End If
        End Try
    End Function

    ''' <summary>
    ''' Reads and returns the login ID in Diamond of the person who entered the claim.
    ''' If it's not found or an error is returned from the call, "Unknown" will be returned
    ''' </summary>
    ''' <param name="ClaimNumber"></param>
    ''' <param name="pg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function GetDiamondClaimEntryUser(ByRef ClaimNumber As String, ByVal pg As Page) As String
    '    Dim Nm As String = Nothing
    '    Dim conn As New SqlConnection()
    '    Dim cmd As New SqlCommand()
    '    Dim da As New SqlDataAdapter()
    '    Dim tbl As New DataTable()
    '    Dim sql As String = Nothing

    '    Try
    '        sql = "select CC.claim_number, cc.claimcontrol_id, U.login_name, cca.claimactivitycode_id"
    '        sql += " from ClaimControl as CC with (nolock)"
    '        sql += " left join ClaimControlActivity AS CCA with (nolock) ON CCA.claimcontrol_id = CC.claimcontrol_id"
    '        sql += " left join ClaimActivityCode as CAC with (nolock) on CAC.claimactivitycode_id = CCA.claimactivitycode_id"
    '        sql += " left join Users as U with (nolock) on U.users_id = CCA.users_id"
    '        sql += " left join Name as N with (nolock) on U.name_id = N.name_id"
    '        sql += " where CC.Claim_Number = '" & ClaimNumber & "'"
    '        sql += " and cca.claimactivitycode_id = 1"

    '        conn = New SqlConnection(AppSettings("connDiamond"))
    '        conn.Open()
    '        cmd.Connection = conn
    '        cmd.CommandType = CommandType.Text
    '        cmd.CommandText = sql
    '        da.SelectCommand = cmd
    '        da.Fill(tbl)

    '        If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Claim entry user not found")
    '        If IsDBNull(tbl.Rows(0)("login_name")) OrElse tbl.Rows(0)("login_name").ToString.Trim = String.Empty Then Throw New Exception("Login name is blank or null")

    '        Return tbl.Rows(0)("login_name").ToString.Trim.ToUpper
    '    Catch ex As Exception
    '        HandleError(ClassName, "GetDiamondClaimEntryUser", ex, pg, "", Nothing)
    '        Return "Unknown"
    '    Finally
    '        If conn.State = ConnectionState.Open Then conn.Close()
    '        conn.Dispose()
    '        cmd.Dispose()
    '        da.Dispose()
    '        tbl.Dispose()
    '    End Try
    'End Function

    ''' <summary>
    ''' Reads the session variable "DiamondUsername" to get the logged in Diamond user name
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDiamondClaimEntryUser(ByRef pg As Page) As String
        Try
            If pg.Session("DiamondUsername") IsNot Nothing AndAlso pg.Session("DiamondUsername").ToString <> String.Empty Then
                Return pg.Session("DiamondUsername").ToString.ToUpper
            Else
                Return "*"
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetDiamondClaimEntryUser", ex, pg, "", Nothing)
            Return "Unknown"
        End Try
    End Function

    ''' <summary>
    ''' Converts a contact name into a formatted name string
    ''' Supports tables tbl_Auto_Reporting, tbl_Property_Reporting, and tbl_General_Reporting
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatFNOLReportingContactName(ByRef pg As Page, ByVal dr As DataRow) As String
        Dim nm As String = ""
        Try
            If DataFieldHasValue(pg, dr, "ContactName") Then
                Return dr("ContactName").ToString
            Else
                If DataFieldHasValue(pg, dr, "ContactFirstName") Then nm += dr("ContactFirstName").ToString()
                If DataFieldHasValue(pg, dr, "ContactMiddleName") Then
                    If nm = String.Empty Then
                        nm += dr("ContactMiddleName").ToString()
                    Else
                        nm += " " & dr("ContactMiddleName").ToString()
                    End If
                End If
                If DataFieldHasValue(pg, dr, "ContactLastName") Then
                    If nm = String.Empty Then
                        nm += dr("ContactLastName").ToString()
                    Else
                        nm += " " & dr("ContactLastName").ToString()
                    End If
                End If
                Return nm
            End If
        Catch ex As Exception
            HandleError(ClassName, "FormatFNOLReportingContactName", ex, pg, "")
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Converts phone numbers into a formatted phone string
    ''' Supports tables tbl_Auto_Reporting, tbl_Property_Reporting, and tbl_General_Reporting
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatFNOLReportingContactPhones(ByRef pg As Page, ByVal dr As DataRow) As String
        Dim ph As String = ""
        Dim st As String = ""
        Dim err As String = Nothing

        Try
            If DataFieldHasValue(pg, dr, "ContactResidencePhone") Then ph += dr("ContactResidencePhone").ToString()
            If DataFieldHasValue(pg, dr, "ContactBusinessPhone") Then
                If ph <> String.Empty Then ph += vbCrLf
                ph += dr("ContactBusinessPhone").ToString()
            End If
            Return ph
        Catch ex As Exception
            HandleError(ClassName, "FormatFNOLReportingContactPhones", ex, pg, "")
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Converts a contact name into a formatted name string
    ''' Supports tables tbl_Auto_Reporting, tbl_Property_Reporting, and tbl_General_Reporting
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatSafeliteContactName(ByRef pg As Page, ByVal dr As DataRow) As String
        Dim nm As String = ""
        Try
            If DataFieldHasValue(pg, dr, "InsFirst") Then nm += dr("InsFirst").ToString()
            If DataFieldHasValue(pg, dr, "InsLast") Then
                If nm = String.Empty Then
                    nm += dr("InsLast").ToString()
                Else
                    nm += " " & dr("InsLast").ToString()
                End If
            End If
            Return nm
        Catch ex As Exception
            HandleError(ClassName, "FormatSafeliteContactName", ex, pg, "")
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Converts safelite phone numbers into a formatted phone string
    ''' Supports tables tbl_Auto_Reporting, tbl_Property_Reporting, and tbl_General_Reporting
    ''' </summary>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatSafeliteContactPhones(ByRef pg As Page, ByVal dr As DataRow) As String
        Dim ph As String = ""
        Dim st As String = ""
        Dim err As String = Nothing

        Try
            If DataFieldHasValue(pg, dr, "ContactHomePhone") Then ph += "H:" & dr("ContactHomePhone").ToString()
            If DataFieldHasValue(pg, dr, "ContactBusPhone") Then
                If ph <> String.Empty Then ph += vbCrLf
                ph += "W: " & dr("ContactBusPhone").ToString()
            End If
            If DataFieldHasValue(pg, dr, "ContactCellPhone") Then
                If ph <> String.Empty Then ph += vbCrLf
                ph += "C: " & dr("ContactCellPhone").ToString()
            End If
            Return ph
        Catch ex As Exception
            HandleError(ClassName, "FormatSafeliteContactPhones", ex, pg, "")
            Return ""
        End Try
    End Function

    Public Function GetSafeliteRecord(ByRef pg As Page, ByVal LOB As SafeliteLOB, ByVal id As String) As DataRow
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = AppSettings("conn")
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT * FROM "
            Select Case LOB
                Case SafeliteLOB.Auto
                    cmd.CommandText += "tbl_auto_reporting "
                    Exit Select
                Case SafeliteLOB.Liability
                    cmd.CommandText += "tbl_general_reporting "
                    Exit Select
                Case SafeliteLOB.Propertty
                    cmd.CommandText += "tbl_property_reporting "
                    Exit Select
                Case SafeliteLOB.Invalid
                    cmd.CommandText += "tbl_SafeliteTemp_FNOL "
                    Exit Select
                Case Else
                    Exit Select
            End Select
            cmd.CommandText += "WHERE id = " & id

            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("Record not found!")
            Return tbl.Rows(0)
        Catch ex As Exception
            HandleError(ClassName, "GetDafeliteRecord", ex, pg, "")
            Return Nothing
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            da.Dispose()
            tbl.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Updates one of safelite tables so that a claim shows as processed
    ''' so that it won't show up in the safelite claims lists
    ''' 
    ''' For Auto, Property, and Liability we add the Diamond Claim Number to the record.
    ''' For Invalid claims we set the processed flag to "Y"
    ''' </summary>
    ''' <param name="pg"></param>
    ''' <param name="recId"></param>
    ''' <param name="LOB"></param>
    ''' <param name="DiamondClaimNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateSafeliteRecordProcessed(ByRef pg As Page, ByVal recId As String, ByRef LOB As SafeliteLOB, ByVal DiamondClaimNumber As String, ByVal PolicyNumber As String, ByVal LossDt As String, ByRef err As String) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Integer = -1
        Dim sql As String = Nothing

        Try
            Select Case LOB
                Case SafeliteLOB.Auto
                    sql = "UPDATE tbl_auto_reporting SET PolicyNumber = '" & PolicyNumber & "', DateOfLoss = '" & LossDt & "', diaClaimNum = '" & DiamondClaimNumber & "' WHERE Id = " & recId
                    'sql = "UPDATE tbl_auto_reporting SET diaClaimNum = '" & DiamondClaimNumber & "' WHERE id = " & recId
                    Exit Select
                Case SafeliteLOB.Invalid
                    sql = "UPDATE tbl_SafeliteTemp_FNOL SET polnum = '" & PolicyNumber & "', lossDt = '" & LossDt & "', lossType = '" & pg.Session("FNOLtype") & "', Processed = 'Y' WHERE id = " & recId
                    'sql = "UPDATE tbl_SafeliteTemp_FNOL SET Processed = 'Y' WHERE id = " & recId
                    Exit Select
                Case SafeliteLOB.Liability
                    sql = "UPDATE tbl_general_reporting SET PolicyNumber = '" & PolicyNumber & "', DateOfLoss = '" & LossDt & "', diaClaimNum = '" & DiamondClaimNumber & "' WHERE Id = " & recId
                    'sql = "UPDATE tbl_general_reporting SET diaClaimNum = '" & DiamondClaimNumber & "' WHERE id = " & recId
                    Exit Select
                Case SafeliteLOB.Propertty
                    sql = "UPDATE tbl_property_reporting SET PolicyNumber = '" & PolicyNumber & "', DateOfLoss = '" & LossDt & "', diaClaimNum = '" & DiamondClaimNumber & "' WHERE Id = " & recId
                    'sql = "UPDATE tbl_property_reporting SET diaClaimNum = '" & DiamondClaimNumber & "' WHERE id = " & recId
                    Exit Select
                Case Else
                    Throw New Exception("Invalid LOB passed!")
            End Select

            conn.ConnectionString = AppSettings("conn")
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = sql

            rtn = cmd.ExecuteNonQuery()

            If rtn <> 1 Then Throw New Exception("Safelite record NOT updated!")

            Return True
        Catch ex As Exception
            HandleError(ClassName, "UpdateSafeliteRecordProcessed", ex, pg, "", Nothing, False)
            Return False
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if there's already a claim in Diamond with the passed policy number and loss date
    ''' </summary>
    ''' <param name="PolicyNumber"></param>
    ''' <param name="LossDate"></param>
    ''' <param name="err"></param>
    ''' <param name="pg"></param>
    ''' <param name="lbl"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DuplicateDiamondClaim(ByVal PolicyNumber As String, ByVal LossDate As String, ByRef err As String, ByRef pg As Page, ByRef lbl As Label) As Boolean
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn.ConnectionString = strConnDiamond
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT P.current_policy, CC.claim_number, cc.loss_date FROM ClaimControl CC LEFT JOIN Policy P ON CC.Policy_Id = P.Policy_Id WHERE P.current_policy = '" & PolicyNumber & "' ORDER BY CC.Claim_Number DESC"
            da.SelectCommand = cmd
            da.Fill(tbl)

            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return False

            For Each dr As DataRow In tbl.Rows
                Dim diaMo As Integer = CDate(dr("loss_date")).Month
                Dim diaYr As Integer = CDate(dr("loss_date")).Year
                Dim diaDy As Integer = CDate(dr("loss_date")).Day
                Dim inMo As Integer = CDate(LossDate).Month
                Dim inYr As Integer = CDate(LossDate).Year
                Dim inDy As Integer = CDate(LossDate).Day

                If diaMo = inMo And diaYr = inYr And diaDy = inDy Then
                    Return True
                End If
            Next

            Return False
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "DuplicateDiamondClaim", ex, pg, "", lbl)
            Return False
        End Try
    End Function

    Public Function IsSafeliteAdmin(ByVal UserId As String, ByRef pg As Page, ByRef err As String) As Boolean
        Dim admins() As String = Nothing

        Try
            If AppSettings("FNOLSafeliteAdmins") Is Nothing Then
                Throw New Exception("FNOLSafeliteAdmins config key does not exist")
            Else
                admins = AppSettings("FNOLSafeliteAdmins").Split("|")
                If admins.Length < 1 Then
                    Throw New Exception("No admins found in the FNOLSafeliteAdmins config key")
                Else
                    For Each admin As String In admins
                        If admin.ToUpper() = UserId.ToUpper() Then Return True
                    Next
                    Return False
                End If
            End If
        Catch ex As Exception
            err = ex.Message
            HandleError(ClassName, "IsSafeliteAdmin", ex, pg, "", Nothing, False)
            Return False
        End Try
    End Function


#End Region

#Region "OLD CODE"

    ''' <summary>
    ''' Synchronizes the FNOL Adjuster table with Diamond
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="AdjTable"></param>
    ''' <returns></returns>
    '    Private Function SynchronizeInternalAdjusterTable(ByRef err As String, Optional ByVal AdjTable As DataTable = Nothing) As Boolean
    '        Dim DIAAdjusters As New DataTable()
    '        Dim EditedAdjusterTable As New DataTable()
    '        Dim adjName As String = ""
    '        Dim FNOLAdjusters As New DataTable()

    '        Try
    '            ' Get the list of active adjusters from Diamond
    '            If AdjTable IsNot Nothing Then
    '                DIAAdjusters = AdjTable
    '            Else
    '                DIAAdjusters = GetDIAMONDAdjusterList(New Label())
    '            End If
    '            If DIAAdjusters Is Nothing OrElse DIAAdjusters.Rows.Count <= 0 Then Return True

    '            EditedAdjusterTable.Columns.Add("claimpersonnel_id")
    '            EditedAdjusterTable.Columns.Add("ManagerID")
    '            EditedAdjusterTable.Columns.Add("display_name")
    '            EditedAdjusterTable.Columns.Add("Manager")
    '            EditedAdjusterTable.Columns.Add("out_of_office")
    '            EditedAdjusterTable.Columns.Add("enabled")

    '            ' Remove any duplicate records from the diamond list.
    '            ' Several adjusters have multiple records.
    '            For Each dr As DataRow In DIAAdjusters.Rows
    '                If adjName = dr("display_name").ToString Then GoTo readnext
    '                adjName = dr("display_name").ToString
    '                ' Handle adjuster names with an apostrophe like O'Malley
    '                adjName = adjName.Replace("'", "''")
    '                ' Find all rows with an adjuster with the same name
    '                Dim rows As DataRow() = DIAAdjusters.Select("display_name = '" & adjName & "'", "claimpersonnel_id DESC")
    '                If rows IsNot Nothing Then
    '                    ' Use the most recent record
    '                    Dim newrow As DataRow = EditedAdjusterTable.NewRow()
    '                    newrow("claimpersonnel_id") = rows(0)("claimpersonnel_id").ToString
    '                    newrow("ManagerID") = rows(0)("ManagerID").ToString
    '                    newrow("display_name") = rows(0)("display_name").ToString
    '                    newrow("Manager") = rows(0)("Manager")
    '                    newrow("out_of_office") = rows(0)("out_of_office")
    '                    newrow("enabled") = rows(0)("enabled")
    '                    EditedAdjusterTable.Rows.Add(newrow)
    '                End If
    'readnext:
    '            Next

    '            ' Make sure there's a corresponding record in the internal adjuster table for each one in the edited Diamond list
    '            For Each dr As DataRow In EditedAdjusterTable.Rows
    '                Dim adjrow As DataRow = GetAdjusterRecordByClaimpersonnel_Id(dr("claimpersonnel_id").ToString(), err)
    '                If err IsNot Nothing AndAlso err <> String.Empty Then Throw New Exception(err)
    '                If adjrow Is Nothing Then
    '                    If Not CheckForDuplicateAdjusterOrManager("A", dr("display_name").ToString(), dr("claimpersonnel_id").ToString(), err) Then
    '                        ' No matching record was found in the internal adjuster table.  Create one.
    '                        If Not InsertInternalAdjusterRecord(dr("claimpersonnel_id").ToString(), dr("Display_Name").ToString(), dr("ManagerID"), True, False, err) Then
    '                            Throw New Exception("Error inserting record into FNOLClaimAssign_Adjusters: " & err)
    '                        End If
    '                    Else
    '                        If err IsNot Nothing OrElse err <> String.Empty Then
    '                            Throw New Exception("Error checking for duplicate record: " & err)
    '                        End If
    '                    End If
    '                End If
    '            Next

    '            ' Remove any adjuster records that weren't in the Adjuster list we pulled from Diamond
    '            FNOLAdjusters = GetFNOLCAAdjusterList(Nothing, False)
    '            If FNOLAdjusters Is Nothing OrElse FNOLAdjusters.Rows.Count <= 0 Then Throw New Exception("No FNOL Adjuster Records found!")

    '            For Each dr As DataRow In FNOLAdjusters.Rows
    '                Dim FNOLAdjID As String = dr("FNOLClaimAssignAdjuster_Id").ToString
    '                Dim cpId As String = dr("claimpersonnel_Id").ToString
    '                adjName = dr("display_name").ToString
    '                Dim resultrows As DataRow() = EditedAdjusterTable.Select("claimpersonnel_id = '" & cpId & "'", Nothing)

    '                If resultrows Is Nothing OrElse resultrows.Count <= 0 Then
    '                    ' No corresponding record found in the edited diamond adjusters table.  Delete the Adjuster Record.

    '                    ' In order to be able to delete the adjuster record we need to make sure there are no records in the 
    '                    ' AdjusterGroups and Counts tables with that adjuster id.  If there are records with those id's SQL will not allow us
    '                    ' to delete the Adjuster record while the Adjuster ID is in the AdjusterGroups and/or Counts tables because of foreign key restraints.

    '                    ' Delete the adjuster's adjuster groups records
    '                    Dim AdjGrpRecs As DataTable = GetAdjusterGroupsByFNOLClaimAssignAdjusterID(FNOLAdjID)
    '                    If AdjGrpRecs IsNot Nothing AndAlso AdjGrpRecs.Rows.Count > 0 Then
    '                        ' There ARE records is in the AdjusterGroup table with the Adjuster ID. Need to change the AdjusterGroups records
    '                        ' with that ID to the new ID
    '                        If Not DeleteAdjusterGroupsRecordsForAdjuster(FNOLAdjID) Then
    '                            Throw New Exception("Error deleting adjuster groups records for adjuster id" & FNOLAdjID)
    '                        End If

    '                        ' OLD
    '                        'Dim NewId As String = GetFNOLClaimAssignAdjuster_ID_ByName(adjName)  ' this returns the record with the highest id
    '                        'If NewId IsNot Nothing Then
    '                        '    If Not ChangeAdjusterGroupsAdjusterId(FNOLAdjID, NewId) Then Throw New Exception("Error changing the Adjuster Groups ID!")
    '                        'Else
    '                        '    Throw New Exception("Adjuster Groups: NewID is nothing!")
    '                        'End If
    '                    Else
    '                        ' There are no records in the AdjusterGroup table with the adjuster id. 
    '                    End If

    '                    ' Delete the adjuster's counts
    '                    Dim CountRecs As DataTable = GetCountRecordsByFNOLClaimAssignAdjusterID(FNOLAdjID)
    '                    If CountRecs IsNot Nothing AndAlso CountRecs.Rows.Count > 0 Then
    '                        ' There ARE records is in the Counts table with the Adjuster ID. Need to change the Counts records
    '                        ' with that ID to the new ID

    '                        If Not DeleteCountRecordsForAdjuster(FNOLAdjID) Then
    '                            Throw New Exception("Error deleting Counts records for adjuster id " & FNOLAdjID)
    '                        End If

    '                        ' OLD
    '                        'Dim NewId As String = GetFNOLClaimAssignAdjuster_ID_ByName(adjName)  ' this returns the record with the highest id
    '                        'If NewId IsNot Nothing Then
    '                        '    If Not ChangeCountsAdjusterId(FNOLAdjID, NewId) Then Throw New Exception("Error changing the Counts ID!")
    '                        'Else
    '                        '    Throw New Exception("Counts: NewID is nothing!")
    '                        'End If
    '                    Else
    '                        ' There are no records in the Counts table with the adjuster id. 
    '                    End If

    '                    ' Remove the Adjuster record in FNOL
    '                    If Not RemoveFNOLAdjusterRecord(FNOLAdjID) Then Throw New Exception("Error removing FNOL Adjuster Record " & FNOLAdjID & " (" & dr("display_name").ToString)
    '                End If
    '            Next

    '            ' Update the out of office and Active flags to match Diamond
    '            For Each DiamondAdjRow As DataRow In EditedAdjusterTable.Rows
    '                Dim FNOLAdjRow As DataRow = GetAdjusterRecordByClaimpersonnel_Id(DiamondAdjRow("claimpersonnel_id").ToString, err)
    '                If FNOLAdjRow Is Nothing Then
    '                    If err IsNot Nothing Then
    '                        Throw New Exception("Error getting the FNOL adjuster record: " & err)
    '                    Else
    '                        Throw New Exception("Error getting the FNOL adjuster record.")
    '                    End If
    '                End If
    '                ' Out of Office
    '                ' Get the flag straight because out of office = true is the same as show = false.  It's confusing.
    '                Dim FNOLOutOfOffice As String = "FALSE"
    '                If FNOLAdjRow("show").ToString = "0" OrElse FNOLAdjRow("show").ToString.ToUpper = "FALSE" Then
    '                    FNOLOutOfOffice = "TRUE"
    '                End If
    '                If FNOLOutOfOffice <> DiamondAdjRow("out_of_office").ToString.ToUpper Then
    '                    ' Out of Office flags don't match between Diamond and FNOL.  Update FNOL to match.
    '                    Dim flagvalue As Boolean = False
    '                    If DiamondAdjRow("out_of_office").ToString.Trim = "1" OrElse DiamondAdjRow("out_of_office").ToString.Trim.ToUpper = "TRUE" Then flagvalue = True
    '                    err = Nothing
    '                    If Not UpdateFNOLAdjusterOutOfOfficeFlag(FNOLAdjRow("FNOLClaimAssignAdjuster_ID").ToString, flagvalue, err) Then
    '                        ' out of office flag update failed
    '                        If err IsNot Nothing Then
    '                            Throw New Exception("Error updating the out of office flag: " & err)
    '                        Else
    '                            Throw New Exception("Error updating the out of office flag.")
    '                        End If
    '                    End If
    '                End If
    '                ' Active
    '                If FNOLAdjRow("Active").ToString <> DiamondAdjRow("Enabled").ToString Then
    '                    ' Active flags don't match between Diamond and FNOL.  Update FNOL to match.
    '                    Dim flagvalue As Boolean = False
    '                    If DiamondAdjRow("Enabled").ToString.Trim = "1" OrElse DiamondAdjRow("enabled").ToString.Trim.ToUpper = "TRUE" Then flagvalue = True
    '                    err = Nothing
    '                    If Not UpdateFNOLAdjusterActiveFlag(FNOLAdjRow("FNOLClaimAssignAdjuster_ID").ToString, flagvalue, err) Then
    '                        ' out of office flag update failed
    '                        If err IsNot Nothing Then
    '                            Throw New Exception("Error updating the Active flag: " & err)
    '                        Else
    '                            Throw New Exception("Error updating the Active flag.")
    '                        End If
    '                    End If
    '                End If
    '            Next

    '            Return True
    '        Catch ex As Exception
    '            err = ex.Message
    '            HandleError(ClassName, "SynchronizeInternalAdjusterTable", ex)
    '            Return False
    '        End Try
    '    End Function


    'Public Function UpdateInternalAdjusterRecord(ByVal ClaimPersonnel_ID As String, ByVal DisplayName As String, ByVal Manager_claimpersonnel_id As String, ByVal Show As Boolean, ByVal Active As Boolean, ByVal OutsideAdjuster As Boolean, ByRef err As String) As Boolean
    '    Dim conn As New SqlConnection()
    '    Dim cmd As New SqlCommand()
    '    Dim rtn As Integer = -1
    '    Dim showval As String = Nothing
    '    Dim activeval As String = Nothing
    '    Dim newname As String = Nothing
    '    Dim sql As String = Nothing

    '    Try
    '        If Show Then showval = 1 Else showval = 0
    '        If Active Then activeval = 1 Else activeval = 0

    '        ' Scrub any single quotes from name
    '        newname = DisplayName.Replace("'", "")

    '        ' Build the sql
    '        sql = "UPDATE FNOLClaimAssign_Adjusters SET "
    '        sql += "claimpersonnel_id = " & ClaimPersonnel_ID
    '        sql += ", display_name = '" & DisplayName & "'"
    '        sql += ", show = " & showval
    '        sql += ", active = " & activeval
    '        sql += ", manager_claimpersonnel_id = " & Manager_claimpersonnel_id
    '        sql += ", OutsideAdjuster = "
    '        If OutsideAdjuster Then
    '            sql += "1"
    '        Else
    '            sql += "0"
    '        End If
    '        sql += " WHERE claimpersonnel_ID = " & ClaimPersonnel_ID

    '        conn = New SqlConnection(strConnFNOL)
    '        conn.Open()
    '        cmd.Connection = conn
    '        cmd.CommandType = CommandType.Text
    '        cmd.CommandText = sql
    '        rtn = cmd.ExecuteNonQuery()
    '        If rtn <> 1 Then Throw New Exception("Update Failed.  Return value: " & rtn.ToString())

    '        Return True
    '    Catch ex As Exception
    '        err = ex.Message
    '        HandleError(ClassName, "UpdateInternalAdjusterRecord", ex)
    '        Return False
    '    Finally
    '        If conn.State = ConnectionState.Open Then conn.Close()
    '        conn.Dispose()
    '        cmd.Dispose()
    '    End Try
    'End Function

    ''' <summary>
    ''' Makes sure that there's a matching record in the FNOLClaimAssign_Adjusters table for each record found in the 
    ''' Diamond adjuster list
    ''' </summary>
    ''' <param name="err"></param>
    ''' <param name="AdjTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function CheckInternalAdjusterTableOLD(ByRef err As String, Optional ByVal AdjTable As DataTable = Nothing) As Boolean
    '    Dim DIAAdjusters As New DataTable()
    '    Try
    '        If AdjTable IsNot Nothing Then
    '            DIAAdjusters = AdjTable
    '        Else
    '            DIAAdjusters = GetDIAMONDAdjusterList(New Label())
    '        End If

    '        If DIAAdjusters Is Nothing OrElse DIAAdjusters.Rows.Count <= 0 Then Return True

    '        ' Make sure there's a corresponding record in the internal adjuster table for each one in the Diamond list
    '        For Each dr As DataRow In DIAAdjusters.Rows
    '            Dim adjrow As DataRow = GetAdjusterRecordByClaimpersonnel_Id(dr("claimpersonnel_id").ToString(), err)
    '            If err IsNot Nothing AndAlso err <> String.Empty Then Throw New Exception(err)
    '            'Dim adjrow As DataRow = GetInternalAdjusterRecord(dr("claimpersonnel_id").ToString(), New Label)
    '            If adjrow Is Nothing Then
    '                If Not CheckForDuplicateAdjusterOrManager("A", dr("display_name").ToString(), dr("claimpersonnel_id").ToString(), err) Then
    '                    ' No matching record was found in the internal adjuster table.  Create one.
    '                    If Not InsertInternalAdjusterRecord(dr("claimpersonnel_id").ToString(), dr("Display_Name").ToString(), dr("ManagerID"), True, False, err) Then
    '                        Throw New Exception("Error inserting record into FNOLClaimAssign_Adjusters: " & err)
    '                    End If
    '                Else
    '                    If err IsNot Nothing OrElse err <> String.Empty Then
    '                        Throw New Exception("Error checking for duplicate record: " & err)
    '                    End If
    '                End If
    '            End If
    '        Next

    '        Return True
    '    Catch ex As Exception
    '        err = ex.Message
    '        HandleError(ClassName, "CheckInternalAdjusterTable", ex)
    '        Return False
    '    End Try
    'End Function

    ''' <summary>
    ''' Gets a table of adjusters from the Diamond claimpersonnel table
    ''' </summary>
    ''' <param name="MsgLabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function GetDIAMONDAdjusterList(ByRef MsgLabel As Label, Optional ByVal ExcludeInactive As Boolean = False) As DataTable
    '    Dim sql As String = Nothing
    '    Dim conn As New SqlConnection()
    '    Dim cmd As New SqlCommand()
    '    Dim da As New SqlDataAdapter()
    '    Dim tbl As New DataTable()
    '    Dim li As New ListItem()
    '    Dim txt As String = Nothing

    '    Try
    '        ' New query with manager
    '        sql = "Select CP.claimpersonnel_id, CP.reports_to_claimpersonnel_id As ManagerID, CP.out_of_office, CP.enabled, N.display_name, N2.display_name As Manager "
    '        sql += "FROM ClaimPersonnel As CP With (NOLOCK) "
    '        sql += "INNER JOIN UserEmployeeLink As UEL With (NOLOCK) On UEL.users_id = CP.users_id "
    '        sql += "INNER JOIN Employee E With (nolock) On E.employee_id = UEL.employee_id "
    '        sql += "INNER JOIN Name N With (nolock) On E.name_id = N.name_id "
    '        sql += "INNER JOIN ClaimPersonnel CP2 On CP.reports_to_claimpersonnel_id = CP2.claimpersonnel_id "
    '        sql += "INNER JOIN UserEmployeeLink As UEL2 With (NOLOCK) On UEL2.users_id = CP2.users_id "
    '        sql += "INNER JOIN Employee E2 With (nolock) On E2.employee_id = UEL2.employee_id "
    '        sql += "INNER JOIN Name N2 With (nolock) On E2.name_id = N2.name_id "
    '        sql += "WHERE CP.claimpersonneltype_id = 3 "
    '        If ExcludeInactive Then sql += "And CP.ENABLED = 1 "  ' Only exclude inactive if the optional parameter is set
    '        sql += "And CP.reports_to_claimpersonnel_id <> 0 "    ' Exclude adjusters with no manager
    '        sql += "ORDER BY display_name ASC, claimpersonnel_id DESC"

    '        'sql = "Select CP.claimpersonnel_id, CP.reports_to_claimpersonnel_id As ManagerID, N.display_name, N2.display_name As Manager FROM ClaimPersonnel As CP With (NOLOCK) "
    '        'sql += "INNER JOIN UserEmployeeLink As UEL With (NOLOCK) On UEL.users_id = CP.users_id "
    '        'sql += "INNER JOIN Employee E With (nolock) On E.employee_id = UEL.employee_id  "
    '        'sql += "INNER JOIN Name N With (nolock) On E.name_id = N.name_id "
    '        'sql += "INNER JOIN ClaimPersonnel CP2 On CP.reports_to_claimpersonnel_id = CP2.claimpersonnel_id "
    '        'sql += "INNER JOIN UserEmployeeLink As UEL2 With (NOLOCK) On UEL2.users_id = CP2.users_id "
    '        'sql += "INNER JOIN Employee E2 With (nolock) On E2.employee_id = UEL2.employee_id "
    '        'sql += "INNER JOIN Name N2 With (nolock) On E2.name_id = N2.name_id "
    '        'sql += "WHERE CP.claimpersonneltype_id = 3 And CP.ENABLED = 1 ORDER BY display_name ASC"

    '        conn.ConnectionString = strConnDiamond
    '        conn.Open()
    '        cmd.Connection = conn
    '        cmd.CommandType = CommandType.Text
    '        cmd.CommandText = sql
    '        da.SelectCommand = cmd
    '        da.Fill(tbl)

    '        If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then
    '            Return Nothing
    '        Else
    '            Return tbl
    '        End If
    '    Catch ex As Exception
    '        HandleError(ClassName, "GetDIAMONDAdjusterList", ex, MsgLabel)
    '        Return Nothing
    '    Finally
    '        If conn.State = ConnectionState.Open Then conn.Close()
    '        conn.Dispose()
    '        cmd.Dispose()
    '        da.Dispose()
    '        tbl.Dispose()
    '    End Try
    'End Function

    '''' <summary>
    '''' Gets Diamond Security Token for logged in user
    '''' </summary>
    '''' <param name="err"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function GetDiamondSecurityToken(ByRef err As String) As Diamond.Common.Services.DiamondSecurityToken
    '    Dim reqLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Request
    '    Dim rspLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Response
    '    Dim uName As String = Nothing
    '    Dim uPwd As String = Nothing

    '    Try
    '        If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> String.Empty Then
    '            uName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
    '            uPwd = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
    '        Else
    '            If AppSettings("TestDiamondTokenUserName") IsNot Nothing AndAlso AppSettings("TestDiamondTokenUserName") <> "" Then
    '                uName = AppSettings("TestDiamondTokenUserName")
    '            End If
    '            If AppSettings("TestDiamondTokenPassword") IsNot Nothing AndAlso AppSettings("TestDiamondTokenPassword") <> "" Then
    '                uPwd = AppSettings("TestDiamondTokenPassword")
    '            End If
    '        End If

    '        With reqLogin.RequestData
    '            .LoginName = uName
    '            .Password = uPwd
    '        End With

    '        Using loginProxy As New Diamond.Common.Services.Proxies.LoginServiceProxy
    '            rspLogin = loginProxy.GetDiamTokenForUsernamePassword(reqLogin)
    '        End Using

    '        If rspLogin.ResponseData IsNot Nothing Then
    '            Return rspLogin.ResponseData.DiamondSecurityToken
    '        Else
    '            Return Nothing
    '        End If

    '    Catch ex As Exception
    '        err = ex.Message
    '        FNOLCommon.HandleError(ClassName, "GetDiamondSecurityToken", ex)
    '    End Try
    'End Function

#End Region

End Class
