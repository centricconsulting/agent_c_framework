Imports Microsoft.VisualBasic
Imports System.Data

Public Class AgencyUserObject
    Implements IDisposable

    Enum UserSystem
        None = 0
        Legacy = 1
        QuoteMaster = 2
        Diamond = 3
        All = 4
    End Enum
    Enum UserLookupType
        None = 0
        UserNameAndPassword = 1
        UserID = 2
    End Enum
    Enum UserErrorType
        None = 0
        Lookup = 1
        SaveDelete = 2
        Both = 3
    End Enum
    Enum UserPasswordValidationType
        LowerCaseLetter = 1
        UpperCaseLetter = 2
        Number = 3
    End Enum

#Region "Var"
    Private MyLegacyDatabaseConnection As String
    Private MyQuoteMasterDatabaseConnection As String
    Private MyDiamondDatabaseConnection As String

    Private MyUserID As String
    Private MyRole As String
    Private MyFullName As String
    Private MyUserName As String
    Private MyPassword As String
    Private MyAuthType As String
    Private MyAgencyID As String
    Private MyHint As String
    Private MyFirstName As String
    Private MyMiddleName As String
    Private MyLastName As String
    Private MyAccess6000Flag As Boolean
    Private MyUpdatedForDiamondFlag As Boolean
    Private MySecurityQuestion1 As String
    Private MySecurityAnswer1 As String
    Private MySecurityQuestion2 As String
    Private MySecurityAnswer2 As String
    Private MySecurityQuestion3 As String
    Private MySecurityAnswer3 As String
    Private MyEmailAddress As String

    Private MyAgencyCode As String
    Private MyAgencyTerritory As String
    Private MyCompletedSurveyFlag As Boolean
    Private MyAgencyName As String

    Private MyHasInfoFlag As Boolean

    Private CheckedLegacy As Boolean
    Private CheckedQuoteMaster As Boolean
    Private CheckedDiamond As Boolean

    Private MyIsInLegacyFlag As Boolean
    Private MyIsInQuoteMasterFlag As Boolean
    Private MyIsInDiamondFlag As Boolean

    Private MyErrorFlag As Boolean
    Private MyErrorType As UserErrorType
    Private MyLookupError As String
    Private MySaveError As String
    Private MySaveErrorSystem As UserSystem '--might not use (would have to add other ENUM options for legacy and Diamond or legacy and QM--as opposed to All)
    Private MyLegacySaveError As String
    Private MyQuoteMasterSaveError As String
    Private MyDiamondSaveError As String

    Private MySQLSelectObject As SQLselectObject
    Private MySQLExecuteObject As SQLexecuteObject

    Private MyDataReader As SqlClient.SqlDataReader

    Private MyDatabaseParameters As ArrayList
    Private MyDatabaseParameter As SqlClient.SqlParameter

    Private IsUserNameOK As Boolean
    Private IsPasswordOK As Boolean
    Private MyUserNameError As String
    Private MyPasswordError As String

    Private holdCount As Integer

    Private MyAgencyCodes As String
    Private MyAgencyIDs As String
    Private MyCheckedAssociateCodesFlag As Boolean

    Private MySourceSystem As UserSystem

    Private MyDiamondAgencyCode As String
    Private My6000Code As String
    Private MyDiamondAgencyID As String
    Private MyQuoteMasterAgencyID As String

    Private QM_existingProducerID As String
    Private QM_st As String
    Private QM_cp As String
    Private QM_autoCredit As String
    Private QM_propCredit As String
    Private QM_mvr As String
    Private QM_autoClue As String
    Private QM_propClue As String

    Private MyOldUserName As String

    Private MyLegacyUserID As String
    Private MyDiamondUserID As String

    Private MySecurityQuestion1ID As String
    Private MySecurityQuestion2ID As String
    Private MySecurityQuestion3ID As String

    Private disposedValue As Boolean = False        ' To detect redundant calls
#End Region

    Public ReadOnly Property UserID() As String
        Get
            Return MyUserID
        End Get
    End Property
    Public Property Role() As String
        Get
            Return MyRole
        End Get
        Set(ByVal value As String)
            MyRole = value
        End Set
    End Property
    Public ReadOnly Property FullName() As String
        Get
            Return MyFullName
        End Get
    End Property
    Public Property UserName() As String
        Get
            Return MyUserName
        End Get
        Set(ByVal value As String)
            MyUserName = value
            ValidateUserName()
        End Set
    End Property
    Public Property Password() As String
        Get
            Return MyPassword
        End Get
        Set(ByVal value As String)
            MyPassword = value
            ValidatePassword()
        End Set
    End Property
    Public Property AuthType() As String
        Get
            Return MyAuthType
        End Get
        Set(ByVal value As String)
            MyAuthType = value
        End Set
    End Property
    Public Property AgencyID() As String
        Get
            Return MyAgencyID
        End Get
        Set(ByVal value As String)
            MyAgencyID = value
        End Set
    End Property
    Public Property Hint() As String
        Get
            Return MyHint
        End Get
        Set(ByVal value As String)
            MyHint = value
        End Set
    End Property
    Public Property FirstName() As String
        Get
            Return MyFirstName
        End Get
        Set(ByVal value As String)
            MyFirstName = value
            UpdateFullName()
        End Set
    End Property
    Public Property MiddleName() As String
        Get
            Return MyMiddleName
        End Get
        Set(ByVal value As String)
            MyMiddleName = value
            UpdateFullName()
        End Set
    End Property
    Public Property LastName() As String
        Get
            Return MyLastName
        End Get
        Set(ByVal value As String)
            MyLastName = value
            UpdateFullName()
        End Set
    End Property
    Public Property Has6000Access() As Boolean
        Get
            Return MyAccess6000Flag
        End Get
        Set(ByVal value As Boolean)
            MyAccess6000Flag = value
        End Set
    End Property
    Public Property IsUpdatedForDiamond() As Boolean
        Get
            Return MyUpdatedForDiamondFlag
        End Get
        Set(ByVal value As Boolean)
            MyUpdatedForDiamondFlag = value
        End Set
    End Property
    Public Property SecurityQuestion1() As String
        Get
            Return MySecurityQuestion1
        End Get
        Set(ByVal value As String)
            MySecurityQuestion1 = value
        End Set
    End Property
    Public Property SecurityAnswer1() As String
        Get
            Return MySecurityAnswer1
        End Get
        Set(ByVal value As String)
            MySecurityAnswer1 = value
        End Set
    End Property
    Public Property SecurityQuestion2() As String
        Get
            Return MySecurityQuestion2
        End Get
        Set(ByVal value As String)
            MySecurityQuestion2 = value
        End Set
    End Property
    Public Property SecurityAnswer2() As String
        Get
            Return MySecurityAnswer2
        End Get
        Set(ByVal value As String)
            MySecurityAnswer2 = value
        End Set
    End Property
    Public Property SecurityQuestion3() As String
        Get
            Return MySecurityQuestion3
        End Get
        Set(ByVal value As String)
            MySecurityQuestion3 = value
        End Set
    End Property
    Public Property SecurityAnswer3() As String
        Get
            Return MySecurityAnswer3
        End Get
        Set(ByVal value As String)
            MySecurityAnswer3 = value
        End Set
    End Property
    Public Property EmailAddress() As String
        Get
            Return MyEmailAddress
        End Get
        Set(ByVal value As String)
            MyEmailAddress = value
        End Set
    End Property

    Public Property AgencyCode() As String
        Get
            Return MyAgencyCode
        End Get
        Set(ByVal value As String)
            MyAgencyCode = value
        End Set
    End Property
    Public ReadOnly Property AgencyTerritory() As String '--readonly because it's only in tbl_agency
        Get
            Return MyAgencyTerritory
        End Get
    End Property
    Public ReadOnly Property HasAgencyCompletedSurvey() As Boolean '--readonly because it's only in tbl_agency
        Get
            Return MyCompletedSurveyFlag
        End Get
    End Property
    Public ReadOnly Property AgencyName() As String '--readonly because it's only in tbl_agency
        Get
            Return MyAgencyName
        End Get
    End Property

    Public ReadOnly Property HasInfo() As Boolean
        Get
            Return MyHasInfoFlag
        End Get
    End Property

    Public ReadOnly Property IsInLegacy() As Boolean
        Get
            If CheckedLegacy = False Then
                '--check legacy
                If MyUserID <> "" Then
                    GetInfo(UserLookupType.UserID)
                ElseIf MyUserName <> "" And MyPassword <> "" Then
                    GetInfo(UserLookupType.UserNameAndPassword)
                End If
            End If
            Return MyIsInLegacyFlag
        End Get
    End Property
    Public ReadOnly Property IsInQuoteMaster() As Boolean
        Get
            If CheckedQuoteMaster = False Then
                '--check QM
            End If
            Return MyIsInQuoteMasterFlag
        End Get
    End Property
    Public ReadOnly Property IsInDiamond() As Boolean
        Get
            If CheckedDiamond = False Then
                '--check Diamond
            End If
            Return MyIsInDiamondFlag
        End Get
    End Property

    Public ReadOnly Property HasError() As Boolean
        Get
            Return MyErrorFlag
        End Get
    End Property
    Public ReadOnly Property ErrorType() As UserErrorType
        Get
            Return MyErrorType
        End Get
    End Property
    Public ReadOnly Property LookupError() As String
        Get
            Return MyLookupError
        End Get
    End Property
    Public ReadOnly Property SaveError() As String
        Get
            Return MySaveError
        End Get
    End Property
    Public ReadOnly Property SaveErrorSystem() As UserSystem
        Get
            Return MySaveErrorSystem
        End Get
    End Property
    Public ReadOnly Property LegacySaveError() As String
        Get
            Return MyLegacySaveError
        End Get
    End Property
    Public ReadOnly Property QuoteMasterSaveError() As String
        Get
            Return MyQuoteMasterSaveError
        End Get
    End Property
    Public ReadOnly Property DiamondSaveError() As String
        Get
            Return MyDiamondSaveError
        End Get
    End Property

    Public ReadOnly Property HasValidUserName() As Boolean
        Get
            Return IsUserNameOK
        End Get
    End Property
    Public ReadOnly Property HasValidPassword() As Boolean
        Get
            Return IsPasswordOK
        End Get
    End Property
    Public ReadOnly Property UserNameValidationError() As String
        Get
            Return MyUserNameError
        End Get
    End Property
    Public ReadOnly Property PasswordValidationError() As String
        Get
            Return MyPasswordError
        End Get
    End Property

    Public ReadOnly Property AgencyCodes() As String
        Get
            CheckAssociateCodes()
            Return MyAgencyCodes
        End Get
    End Property
    Public ReadOnly Property AgencyIDs() As String
        Get
            CheckAssociateCodes()
            Return MyAgencyIDs
        End Get
    End Property
    Public Property SecurityQuestion1ID() As String
        Get
            Return MySecurityQuestion1ID
        End Get
        Set(ByVal value As String)
            MySecurityQuestion1ID = value
        End Set
    End Property
    Public Property SecurityQuestion2ID() As String
        Get
            Return MySecurityQuestion2ID
        End Get
        Set(ByVal value As String)
            MySecurityQuestion2ID = value
        End Set
    End Property
    Public Property SecurityQuestion3ID() As String
        Get
            Return MySecurityQuestion3ID
        End Get
        Set(ByVal value As String)
            MySecurityQuestion3ID = value
        End Set
    End Property


    Public Sub New()
        'for creating new user
        SetDefaults()
    End Sub
    Public Sub New(ByVal user As String, ByVal pass As String)
        'lookup for username and password
        SetDefaults()

        MyUserName = user
        MyPassword = pass

        If MyUserName = "" Or MyPassword = "" Then
            SetError(UserErrorType.Lookup, "Must have username and password for lookup")
        Else
            GetInfo(UserLookupType.UserNameAndPassword)
        End If
    End Sub
    Public Sub New(ByVal ID As String)
        'lookup for userID
        SetDefaults()

        MyUserID = ID

        If MyUserID = "" Then
            SetError(UserErrorType.Lookup, "Must have userID for lookup")
        Else
            GetInfo(UserLookupType.UserID)
        End If
    End Sub

    Private Sub SetDefaults()
        MyLegacyDatabaseConnection = ""
        MyQuoteMasterDatabaseConnection = ""
        MyDiamondDatabaseConnection = ""

        MyUserID = ""
        MyRole = ""
        MyFullName = ""
        MyUserName = ""
        MyPassword = ""
        MyAuthType = ""
        MyAgencyID = ""
        MyHint = ""
        MyFirstName = ""
        MyMiddleName = ""
        MyLastName = ""
        MyAccess6000Flag = False
        MyUpdatedForDiamondFlag = False
        MySecurityQuestion1 = ""
        MySecurityAnswer1 = ""
        MySecurityQuestion2 = ""
        MySecurityAnswer2 = ""
        MySecurityQuestion3 = ""
        MySecurityAnswer3 = ""
        MyEmailAddress = ""

        MyAgencyCode = ""
        MyAgencyTerritory = ""
        MyCompletedSurveyFlag = False
        MyAgencyName = ""

        MyHasInfoFlag = False

        CheckedLegacy = False
        CheckedQuoteMaster = False
        CheckedDiamond = False

        MyIsInLegacyFlag = False
        MyIsInQuoteMasterFlag = False
        MyIsInDiamondFlag = False

        MyErrorFlag = False
        MyErrorType = 0
        MyLookupError = ""
        MySaveError = ""
        MySaveErrorSystem = 0
        MyLegacySaveError = ""
        MyQuoteMasterSaveError = ""
        MyDiamondSaveError = ""

        IsUserNameOK = False
        IsPasswordOK = False
        MyUserNameError = ""
        MyPasswordError = ""

        holdCount = 0

        MyAgencyCodes = ""
        MyAgencyIDs = ""

        MyCheckedAssociateCodesFlag = False

        '--default system for lookups (probably use config key - AgencyUserSourceSystem)
        MySourceSystem = UserSystem.Legacy
        'MySourceSystem = UserSystem.Diamond

        MyDiamondAgencyCode = ""
        My6000Code = ""
        MyDiamondAgencyID = ""
        MyQuoteMasterAgencyID = ""

        QM_existingProducerID = ""
        QM_st = ""
        QM_cp = ""
        QM_autoCredit = ""
        QM_propCredit = ""
        QM_mvr = ""
        QM_autoClue = ""
        QM_propClue = ""

        MyOldUserName = ""

        MyLegacyUserID = ""
        MyDiamondUserID = ""

        MySecurityQuestion1ID = ""
        MySecurityQuestion2ID = ""
        MySecurityQuestion3ID = ""
    End Sub

    Private Sub CheckDatabaseConnections()
        If MyLegacyDatabaseConnection = "" And Not ConfigurationManager.AppSettings("conn") Is Nothing Then
            MyLegacyDatabaseConnection = ConfigurationManager.AppSettings("conn")
        End If
        If MyQuoteMasterDatabaseConnection = "" And Not ConfigurationManager.AppSettings("connQM") Is Nothing Then
            MyQuoteMasterDatabaseConnection = ConfigurationManager.AppSettings("connQM")
        End If
        If MyDiamondDatabaseConnection = "" And Not ConfigurationManager.AppSettings("connDiamond") Is Nothing Then
            MyDiamondDatabaseConnection = ConfigurationManager.AppSettings("connDiamond")
        End If
    End Sub

    Private Sub GetInfo(ByVal lookupType As UserLookupType)
        If lookupType = UserLookupType.None Then
            SetError(UserErrorType.Lookup, "Must have lookup type")
        Else
            CheckDatabaseConnections()

            If MySourceSystem = UserSystem.Diamond Then
                GetDiamondInfo(lookupType)
            Else
                GetLegacyInfo(lookupType)
            End If

        End If
    End Sub

    Private Sub GetLegacyInfo(ByVal lookupType As UserLookupType)
        CheckedLegacy = True
        MySQLSelectObject = New SQLselectObject(MyLegacyDatabaseConnection)

        If lookupType = UserLookupType.UserNameAndPassword Then
            MySQLSelectObject.queryOrStoredProc = "sp_GetAgencyUserInfo"

            MyDatabaseParameters = New ArrayList
            MyDatabaseParameters.Add(New SqlClient.SqlParameter("@user", MyUserName))
            MyDatabaseParameters.Add(New SqlClient.SqlParameter("@pass", MyPassword))
            MySQLSelectObject.parameters = MyDatabaseParameters
        ElseIf lookupType = UserLookupType.UserID Then
            MySQLSelectObject.queryOrStoredProc = "sp_GetAgencyUserInfoByID"

            MyDatabaseParameter = New SqlClient.SqlParameter("@userID", MyUserID)
            MySQLSelectObject.parameter = MyDatabaseParameter
        End If

        MyDataReader = MySQLSelectObject.GetDataReader

        If Not MyDataReader Is Nothing Then
            If MyDataReader.HasRows Then

                MyHasInfoFlag = True
                MyIsInLegacyFlag = True
                MyDataReader.Read()

                '--get tbl_users data
                MyUserID = MyDataReader.Item("user_id").ToString.Trim
                MyLegacyUserID = MyUserID
                MyRole = MyDataReader.Item("role").ToString.Trim
                MyFullName = MyDataReader.Item("full_name").ToString.Trim
                MyUserName = MyDataReader.Item("username").ToString.Trim
                MyOldUserName = MyUserName
                MyPassword = MyDataReader.Item("pass").ToString.Trim
                MyAuthType = MyDataReader.Item("auth_type").ToString.Trim
                MyAgencyID = MyDataReader.Item("agency_id").ToString.Trim
                MyHint = MyDataReader.Item("hint").ToString.Trim
                MyFirstName = MyDataReader.Item("FirstName").ToString.Trim
                MyMiddleName = MyDataReader.Item("MiddleName").ToString.Trim
                MyLastName = MyDataReader.Item("LastName").ToString.Trim
                MyAccess6000Flag = ConvertCharToBoolean(MyDataReader.Item("Access6000").ToString.Trim)
                MyUpdatedForDiamondFlag = ConvertCharToBoolean(MyDataReader.Item("UpdatedForDiamond").ToString.Trim)
                MySecurityQuestion1 = MyDataReader.Item("SecurityQuestion1").ToString.Trim
                MySecurityAnswer1 = MyDataReader.Item("SecurityAnswer1").ToString.Trim
                MySecurityQuestion2 = MyDataReader.Item("SecurityQuestion2").ToString.Trim
                MySecurityAnswer2 = MyDataReader.Item("SecurityAnswer2").ToString.Trim
                MySecurityQuestion3 = MyDataReader.Item("SecurityQuestion3").ToString.Trim
                MySecurityAnswer3 = MyDataReader.Item("SecurityAnswer3").ToString.Trim
                MyEmailAddress = MyDataReader.Item("EmailAddress").ToString.Trim

                If MyFullName = "" Then
                    UpdateFullName()
                End If

                '--get tbl_agency data
                MyAgencyCode = MyDataReader.Item("agency_code").ToString.Trim
                MyAgencyTerritory = MyDataReader.Item("territory").ToString.Trim
                MyCompletedSurveyFlag = ConvertCharToBoolean(MyDataReader.Item("completedSurvey").ToString.Trim)
                MyAgencyName = MyDataReader.Item("agency_name").ToString.Trim

                MyDataReader.Close()
            Else
                If MySQLSelectObject.hasError = True Then
                    'SQL error
                    SetError(UserErrorType.Lookup, "Error querying legacy database")
                Else
                    'no info found
                End If
            End If
        Else
            'error
            SetError(UserErrorType.Lookup, "Error querying legacy database")
        End If

        'dispose of SQL object
        MyDataReader = Nothing
        MySQLSelectObject.Dispose()

        If MyHasInfoFlag = True Then
            '--validate username and password
            ValidateUserName()
            ValidatePassword()
        End If
    End Sub
    Private Sub GetDiamondInfo(ByVal lookupType As UserLookupType)
        CheckedDiamond = True

        '--set MyOldUserName and MyDiamondUserID
    End Sub

    Private Sub CheckAssociateCodes()
        If MyCheckedAssociateCodesFlag = False Then

            If MyAgencyCode <> "" Then
                CheckDatabaseConnections()
                MyCheckedAssociateCodesFlag = True

                'If MyAccess6000Flag = True Then
                '    If MySourceSystem = UserSystem.Diamond Then
                '        GetDiamond6000Codes()
                '    Else
                '        GetLegacy6000Codes()
                '    End If
                'Else
                '    If MySourceSystem = UserSystem.Diamond Then
                '        GetDiamondCodes()
                '    Else
                '        GetLegacyCodes()
                '    End If
                'End If
                If MySourceSystem = UserSystem.Diamond Then
                    GetDiamondAssociateCodes()
                Else
                    If MyAccess6000Flag = True Then
                        GetLegacy6000Codes()
                    Else
                        GetLegacyCodes()
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub GetLegacy6000Codes()
        Dim HasCodes As Boolean = False

        Using sql As New SQLselectObject(MyLegacyDatabaseConnection)
            sql.queryOrStoredProc = "sp_Get6000Agencies"
            sql.parameter = New SqlClient.SqlParameter("@agCode", MyAgencyCode)

            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

            If Not dr Is Nothing Then
                If dr.HasRows Then
                    HasCodes = True

                    While dr.Read
                        If MyAgencyIDs = "" Then
                            MyAgencyIDs = dr.Item("agency_id").ToString.Trim
                            MyAgencyCodes = dr.Item("AgencyCode").ToString.Trim
                        Else
                            MyAgencyIDs = MyAgencyIDs & "," & dr.Item("agency_id").ToString.Trim
                            MyAgencyCodes = MyAgencyCodes & "," & dr.Item("AgencyCode").ToString.Trim
                        End If
                    End While
                Else
                    If sql.hasError Then
                        'sql error
                    Else
                        'no rows
                    End If
                End If
            Else
                'sql error
            End If
        End Using

        'check for regular associate codes if there are no 6000 ties
        If HasCodes = False Then
            GetLegacyCodes()
        End If

    End Sub
    Private Sub GetLegacyCodes()

        Using sql As New SQLselectObject(MyLegacyDatabaseConnection)
            sql.queryOrStoredProc = "sp_GetAssociateAgencies"
            sql.parameter = New SqlClient.SqlParameter("@agCode", MyAgencyCode)

            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

            If Not dr Is Nothing Then
                If dr.HasRows Then

                    While dr.Read
                        If MyAgencyIDs = "" Then
                            MyAgencyIDs = dr.Item("agency_id").ToString.Trim
                            MyAgencyCodes = dr.Item("agency_code").ToString.Trim
                        Else
                            MyAgencyIDs = MyAgencyIDs & "," & dr.Item("agency_id").ToString.Trim
                            MyAgencyCodes = MyAgencyCodes & "," & dr.Item("agency_code").ToString.Trim
                        End If
                    End While
                Else
                    If sql.hasError Then
                        'sql error
                    Else
                        'no rows
                    End If
                End If
            Else
                'sql error
            End If
        End Using

    End Sub

    'Private Sub GetDiamond6000Codes()
    '    Dim HasCodes As Boolean = False

    '    Dim holdCode As String = ""

    '    Using sql As New SQLselectObject(MyDiamondDatabaseConnection)
    '        sql.queryOrStoredProc = "select * from Agency where Agency.agencygroup_id = (select Agency.agencygroup_id from Agency where right(code, 4) = '" & MyAgencyCode & "')"

    '        Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

    '        If Not dr Is Nothing Then
    '            If dr.HasRows Then
    '                HasCodes = True

    '                While dr.Read
    '                    holdCode = dr.Item("code").ToString.Trim
    '                    If holdCode.Contains("-") And Len(holdCode) = 9 Then
    '                        holdCode = Right(holdCode, 4)
    '                    End If

    '                    If MyAgencyIDs = "" Then
    '                        MyAgencyIDs = dr.Item("agency_id").ToString.Trim
    '                        MyAgencyCodes = holdCode
    '                    Else
    '                        MyAgencyIDs = MyAgencyIDs & "," & dr.Item("agency_id").ToString.Trim
    '                        MyAgencyCodes = MyAgencyCodes & "," & holdCode
    '                    End If
    '                End While
    '            Else
    '                If sql.hasError Then
    '                    'sql error
    '                Else
    '                    'no rows
    '                End If
    '            End If
    '        Else
    '            'sql error
    '        End If
    '    End Using

    '    'check for regular associate codes if there are no 6000 ties
    '    If HasCodes = False Then
    '        GetDiamondCodes()
    '    End If

    'End Sub
    'Private Sub GetDiamondCodes()

    '    Dim holdCode As String = ""

    '    Using sql As New SQLselectObject(MyDiamondDatabaseConnection)
    '        sql.queryOrStoredProc = "select * from Agency where Agency.agencygroup_id = (select Agency.agencygroup_id from Agency where right(code, 4) = '" & MyAgencyCode & "' and Agency.primary_agency = 1)"

    '        Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

    '        If Not dr Is Nothing Then
    '            If dr.HasRows Then

    '                While dr.Read
    '                    holdCode = dr.Item("code").ToString.Trim
    '                    If holdCode.Contains("-") And Len(holdCode) = 9 Then
    '                        holdCode = Right(holdCode, 4)
    '                    End If

    '                    If MyAgencyIDs = "" Then
    '                        MyAgencyIDs = dr.Item("agency_id").ToString.Trim
    '                        MyAgencyCodes = holdCode
    '                    Else
    '                        MyAgencyIDs = MyAgencyIDs & "," & dr.Item("agency_id").ToString.Trim
    '                        MyAgencyCodes = MyAgencyCodes & "," & holdCode
    '                    End If
    '                End While
    '            Else
    '                If sql.hasError Then
    '                    'sql error
    '                Else
    '                    'no rows
    '                End If
    '            End If
    '        Else
    '            'sql error
    '        End If
    '    End Using

    'End Sub

    Private Sub GetDiamondAssociateCodes()
        '--only use if SourceSystem is Diamond
        If MyDiamondUserID <> "" Then
            Dim holdCode As String = ""

            Using sql As New SQLselectObject(MyDiamondDatabaseConnection)
                sql.queryOrStoredProc = "select * from users inner join UserAgencyLink as UAL on UAL.users_id = Users.users_id inner join Agency on Agency.agency_id = UAL.agency_id where Users.users_id = '" & MyDiamondUserID & "'"

                Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

                If Not dr Is Nothing Then
                    If dr.HasRows Then

                        While dr.Read
                            holdCode = dr.Item("code").ToString.Trim
                            If holdCode.Contains("-") And Len(holdCode) = 9 Then
                                holdCode = Right(holdCode, 4)
                            End If

                            If MyAgencyIDs = "" Then
                                MyAgencyIDs = dr.Item("agency_id").ToString.Trim
                                MyAgencyCodes = holdCode
                            Else
                                MyAgencyIDs = MyAgencyIDs & "," & dr.Item("agency_id").ToString.Trim
                                MyAgencyCodes = MyAgencyCodes & "," & holdCode
                            End If
                        End While
                    Else
                        If sql.hasError Then
                            'sql error
                        Else
                            'no rows
                        End If
                    End If
                Else
                    'sql error
                End If
            End Using
        End If

    End Sub

    Private Sub UpdateFullName()
        MyFullName = ""

        MyFullName = MyFirstName
        MyFullName = Append(MyFullName, MyMiddleName, " ")
        MyFullName = Append(MyFullName, MyLastName, " ")
    End Sub

    Private Function Append(ByVal curr As String, ByVal add As String, ByVal separator As String) As String
        Append = ""

        If curr = "" Then
            Append = add
        Else
            Append = curr & separator & add
        End If
    End Function

    Private Sub ValidateUserName()
        If MyUserName = "" Then
            IsUserNameOK = False
            MyUserNameError = "Must have UserName"
        ElseIf Len(MyUserName) < 3 Then
            IsUserNameOK = False
            MyUserNameError = "UserName must be at least 3 characters"
        ElseIf Len(MyUserName) > 20 Then
            IsUserNameOK = False
            MyUserNameError = "UserName cannot exceed 20 characters"
        ElseIf MyUserName.Contains("&") = True Then
            IsUserNameOK = False
            MyUserNameError = "UserName cannot contain '&'"
        ElseIf MyUserName.Contains("#") = True Then
            IsUserNameOK = False
            MyUserNameError = "UserName cannot contain '#'"
        ElseIf IsUserNameTaken() = True Then
            IsUserNameOK = False
            MyUserNameError = "UserName is already taken"
        Else
            IsUserNameOK = True
            MyUserNameError = ""

            '--might also check for duplicate user here
        End If
    End Sub
    Private Sub ValidatePassword()
        If MyPassword = "" Then
            IsPasswordOK = False
            MyPasswordError = "Must have Password"
        ElseIf Len(MyPassword) < 8 Then
            IsPasswordOK = False
            MyPasswordError = "Password must be at least 8 characters"
        ElseIf Len(MyPassword) > 20 Then
            IsPasswordOK = False
            MyPasswordError = "Password cannot exceed 20 characters"
        ElseIf MyPassword.Contains("&") = True Then
            IsPasswordOK = False
            MyPasswordError = "Password cannot contain '&'"
        ElseIf MyPassword.Contains("#") = True Then
            IsPasswordOK = False
            MyPasswordError = "Password cannot contain '#'"
        ElseIf MeetsCriteria(UserPasswordValidationType.LowerCaseLetter, MyPassword, 1) = False Then
            IsPasswordOK = False
            MyPasswordError = "Password must contain at least one lowercase letter"
        ElseIf MeetsCriteria(UserPasswordValidationType.UpperCaseLetter, MyPassword, 1) = False Then
            IsPasswordOK = False
            MyPasswordError = "Password must contain at least one uppercase letter"
        ElseIf MeetsCriteria(UserPasswordValidationType.Number, MyPassword, 1) = False Then
            IsPasswordOK = False
            MyPasswordError = "Password must contain at least one number"
        Else
            IsPasswordOK = True
            MyPasswordError = ""

        End If
    End Sub
    Private Function MeetsCriteria(ByVal validationType As UserPasswordValidationType, ByVal evalWord As String, ByVal numberOfOccurrences As Integer) As Boolean
        MeetsCriteria = False
        holdCount = 0

        If numberOfOccurrences < 1 Then
            MeetsCriteria = True
        ElseIf evalWord = "" Then
            MeetsCriteria = False
        Else
            For Each chr As Char In evalWord
                Select Case validationType
                    Case UserPasswordValidationType.LowerCaseLetter
                        If Char.IsLower(chr) = True Then
                            holdCount += 1
                            If holdCount >= numberOfOccurrences Then
                                MeetsCriteria = True
                                Exit For
                            End If
                        End If
                    Case UserPasswordValidationType.UpperCaseLetter
                        If Char.IsUpper(chr) = True Then
                            holdCount += 1
                            If holdCount >= numberOfOccurrences Then
                                MeetsCriteria = True
                                Exit For
                            End If
                        End If
                    Case UserPasswordValidationType.Number
                        If Char.IsNumber(chr) = True Then
                            holdCount += 1
                            If holdCount >= numberOfOccurrences Then
                                MeetsCriteria = True
                                Exit For
                            End If
                        End If
                End Select
            Next
        End If
    End Function
    Private Function IsUserNameTaken() As Boolean
        IsUserNameTaken = False

        CheckDatabaseConnections()

        MySQLSelectObject = New SQLselectObject(MyLegacyDatabaseConnection)

        If MyUserID = "" Then
            '--look for username (this is for a new user)
            MySQLSelectObject.queryOrStoredProc = "sp_ValidateAgencyUser"

            MyDatabaseParameter = New SqlClient.SqlParameter("@username", MyUserName)
            MySQLSelectObject.parameter = MyDatabaseParameter
        Else
            '--look for username and different user_id (this is for an existing user)
            MySQLSelectObject.queryOrStoredProc = "sp_ValidateAgencyUserByID"

            MyDatabaseParameters = New ArrayList
            MyDatabaseParameters.Add(New SqlClient.SqlParameter("@username", MyUserName))
            MyDatabaseParameters.Add(New SqlClient.SqlParameter("@userID", MyUserID))
            MySQLSelectObject.parameters = MyDatabaseParameters
        End If

        MyDataReader = MySQLSelectObject.GetDataReader

        If Not MyDataReader Is Nothing Then
            If MyDataReader.HasRows Then
                IsUserNameTaken = True
            Else
                If MySQLSelectObject.hasError = True Then
                    'SQL error
                    SetError(UserErrorType.Lookup, "Error querying legacy database")
                Else
                    'no info found
                End If
            End If
        Else
            'error
            SetError(UserErrorType.Lookup, "Error querying legacy database")
        End If

        MyDataReader = Nothing
        MySQLSelectObject.Dispose()

    End Function

    Public Sub SaveInfo(ByVal saveSystem As UserSystem, Optional ByVal IgnoreUserNamePasswordValidation As Boolean = False)

        CheckDatabaseConnections()

        If MyUserID <> "" Then
            'update (at least in legacy; may need insert in others)
        Else
            'insert
        End If

        If IsUserNameOK = False AndAlso IgnoreUserNamePasswordValidation = False Then
            SetError(UserErrorType.SaveDelete, "Invalid UserName")
        ElseIf IsPasswordOK = False AndAlso IgnoreUserNamePasswordValidation = False Then
            SetError(UserErrorType.SaveDelete, "Invalid Password")
        Else
            Select Case saveSystem
                Case UserSystem.None
                    SetError(UserErrorType.SaveDelete, "Must have save system")
                Case UserSystem.Legacy
                    SaveLegacy()
                Case UserSystem.QuoteMaster
                    SaveQuoteMaster()
                Case UserSystem.Diamond
                    SaveDiamond()
                Case UserSystem.All
                    SaveLegacy()
                    SaveQuoteMaster()
                    SaveDiamond()
            End Select
        End If

    End Sub

    Private Sub SaveLegacy()
        Using sql As New SQLexecuteObject(MyLegacyDatabaseConnection)
            sql.queryOrStoredProc = "sp_Save_AgencyUser"

            Dim params As New ArrayList

            If MyUserID <> "" Then
                'update (at least in legacy; may need insert in others)
                params.Add(New SqlClient.SqlParameter("@user_id", MyUserID))
            Else
                'insert
                sql.outputParameter = New SqlClient.SqlParameter("@user_id", SqlDbType.Int, 4)
            End If

            params.Add(New SqlClient.SqlParameter("@role", MyRole)) '--might not use
            params.Add(New SqlClient.SqlParameter("@full_name", MyFullName))
            params.Add(New SqlClient.SqlParameter("@username", MyUserName))
            params.Add(New SqlClient.SqlParameter("@pass", MyPassword))
            params.Add(New SqlClient.SqlParameter("@auth_type", MyAuthType))
            params.Add(New SqlClient.SqlParameter("@agency_id", MyAgencyID))
            params.Add(New SqlClient.SqlParameter("@FirstName", MyFirstName))
            params.Add(New SqlClient.SqlParameter("@MiddleName", MyMiddleName))
            params.Add(New SqlClient.SqlParameter("@LastName", MyLastName))
            params.Add(New SqlClient.SqlParameter("@Access6000", ConvertBooleanToChar(MyAccess6000Flag))) '--might not use
            params.Add(New SqlClient.SqlParameter("@UpdatedForDiamond", ConvertBooleanToChar(MyUpdatedForDiamondFlag))) '--might not use
            params.Add(New SqlClient.SqlParameter("@SecurityQuestion1", MySecurityQuestion1))
            params.Add(New SqlClient.SqlParameter("@SecurityAnswer1", MySecurityAnswer1))
            params.Add(New SqlClient.SqlParameter("@SecurityQuestion2", MySecurityQuestion2))
            params.Add(New SqlClient.SqlParameter("@SecurityAnswer2", MySecurityAnswer2))
            params.Add(New SqlClient.SqlParameter("@SecurityQuestion3", MySecurityQuestion3))
            params.Add(New SqlClient.SqlParameter("@SecurityAnswer3", MySecurityAnswer3))
            params.Add(New SqlClient.SqlParameter("@EmailAddress", MyEmailAddress))

            sql.inputParameters = params

            sql.ExecuteStatement()

            If sql.rowsAffected = 0 Then
                MyLegacySaveError = "No Rows Affected"
                SetError(UserErrorType.SaveDelete, "Legacy Save Routine Failed")
            Else
                If MyUserID = "" Then
                    MyUserID = sql.outputParameter.Value
                End If
            End If
        End Using
    End Sub

    Private Sub SaveQuoteMaster()
        If CheckedQuoteMaster = False Then
            CheckQuoteMasterAgency()
        End If

        If MyQuoteMasterAgencyID = "" Then
            MyQuoteMasterSaveError = "Agency " & MyAgencyCode & " is not in QuoteMaster"
            SetError(UserErrorType.SaveDelete, "Missing Agency in QuoteMaster")
        Else 'might also check to see if existing ProducerID has a value
            Using sql As New SQLexecuteObject(MyLegacyDatabaseConnection)
                sql.queryOrStoredProc = "sp_Save_AgencyUser_QM"

                Dim params As New ArrayList

                params.Add(New SqlClient.SqlParameter("@agencyID", MyQuoteMasterAgencyID))
                params.Add(New SqlClient.SqlParameter("@firstName", MyFirstName))
                params.Add(New SqlClient.SqlParameter("@middleName", MyMiddleName))
                params.Add(New SqlClient.SqlParameter("@lastName", MyLastName))
                params.Add(New SqlClient.SqlParameter("@userName", MyUserName))
                params.Add(New SqlClient.SqlParameter("@passWord", MyPassword))

                If MyUserID <> "" Then
                    'update
                    params.Add(New SqlClient.SqlParameter("@oldUserName", MyOldUserName))
                Else
                    'insert
                    params.Add(New SqlClient.SqlParameter("@stateAbrv", QM_st))
                    params.Add(New SqlClient.SqlParameter("@cpNodeID", QM_cp))
                    params.Add(New SqlClient.SqlParameter("@autoCreditAccount", QM_autoCredit))
                    params.Add(New SqlClient.SqlParameter("@propCreditAccount", QM_propCredit))
                    params.Add(New SqlClient.SqlParameter("@mvrAccount", QM_mvr))
                    params.Add(New SqlClient.SqlParameter("@autoClueAccount", QM_autoClue))
                    params.Add(New SqlClient.SqlParameter("@propClueAccount", QM_propClue))
                End If

                sql.inputParameters = params

                sql.ExecuteStatement()

                If sql.rowsAffected = 0 Then
                    MyQuoteMasterSaveError = "No Rows Affected"
                    SetError(UserErrorType.SaveDelete, "QuoteMaster Save Routine Failed")
                Else
                    'might change stored proc to return ID
                End If
            End Using
        End If
    End Sub

    Private Sub SaveDiamond()
        If CheckedDiamond = False Then

        End If
    End Sub

    Private Sub CheckQuoteMasterAgency()
        CheckedQuoteMaster = True

        Using sql As New SQLselectObject(MyQuoteMasterDatabaseConnection)
            'sql.queryOrStoredProc = "Select AgencyID from AgencyTable where AgencyNum = '" & MyAgencyCode & "'"
            sql.queryOrStoredProc = "SELECT AT.AgencyID, PT.ProducerID, CPAT.StateAbrv, CPAT.CPNodeId, CPAT.AutoCreditAccount, CPAT.PropCreditAccount, CPAT.MVRAccount, CPAT.AutoCLUEAccount, CPAT.PropCLUEAccount FROM AgencyTable as AT left join ProducersTable as PT on PT.AgencyID = AT.AgencyID and PT.ProducerID = (SELECT max(PT2.ProducerID) from ProducersTable as PT2 where PT2.AgencyID = AT.AgencyID) left join ChoicePointAccountTable as CPAT on CPAT.ProducerID = PT.ProducerID where AT.AgencyNum = '" & MyAgencyCode & "'"

            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

            If Not dr Is Nothing Then
                If dr.HasRows Then
                    MyIsInQuoteMasterFlag = True

                    MyQuoteMasterAgencyID = dr.Item("AgencyID").ToString.Trim

                    If dr.Item("ProducerID").ToString.Trim <> "" Then
                        QM_existingProducerID = dr.Item("ProducerID").ToString.Trim
                        QM_st = dr.Item("StateAbrv").ToString.Trim
                        QM_cp = dr.Item("CPNodeId").ToString.Trim
                        QM_autoCredit = dr.Item("AutoCreditAccount").ToString.Trim
                        QM_propCredit = dr.Item("PropCreditAccount").ToString.Trim
                        QM_mvr = dr.Item("MVRAccount").ToString.Trim
                        QM_autoClue = dr.Item("AutoCLUEAccount").ToString.Trim
                        QM_propClue = dr.Item("PropCLUEAccount").ToString.Trim
                    Else

                    End If
                Else
                    If sql.hasError = True Then
                        'SQL error
                        SetError(UserErrorType.Lookup, "Error querying QM database")
                    Else
                        'no info found
                    End If
                End If
            Else
                'error
                SetError(UserErrorType.Lookup, "Error querying QM database")
            End If
        End Using
    End Sub

    Private Sub CheckDiamondAgency()
        CheckedDiamond = True

        Using sql As New SQLselectObject(MyDiamondDatabaseConnection)
            sql.queryOrStoredProc = "Select agency_id, code from Agency where right(code, 4) = '" & MyAgencyCode & "'"

            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

            If Not dr Is Nothing Then
                If dr.HasRows Then
                    MyDiamondAgencyID = dr.Item("agency_id").ToString.Trim
                    MyDiamondAgencyCode = dr.Item("code").ToString.Trim

                    If Len(MyDiamondAgencyCode) = 9 And InStr(MyDiamondAgencyCode, "-") Then
                        If My6000Code = "" Then
                            My6000Code = Left(MyDiamondAgencyCode, 4)
                        End If
                    End If
                Else
                    If sql.hasError = True Then
                        'SQL error
                        SetError(UserErrorType.Lookup, "Error querying Diamond database")
                    Else
                        'no info found
                    End If
                End If
            Else
                'error
                SetError(UserErrorType.Lookup, "Error querying Diamond database")
            End If
        End Using
    End Sub

    Public Sub DeleteUser(ByVal deleteSystem As UserSystem)
        CheckDatabaseConnections()

        Select Case deleteSystem
            Case UserSystem.None
                SetError(UserErrorType.SaveDelete, "Must have delete system")
            Case UserSystem.Legacy
                DeleteLegacy()
            Case UserSystem.QuoteMaster
                DeleteQuoteMaster()
            Case UserSystem.Diamond
                DeleteDiamond()
            Case UserSystem.All
                DeleteLegacy()
                DeleteQuoteMaster()
                DeleteDiamond()
        End Select
    End Sub

    Private Sub DeleteLegacy()
        '--won't delete from legacy once SourceSystem is Diamond
        If MyLegacyUserID <> "" Then
            '--check for EFT payment by user id
            If HasLegacyEFTPayment() = False Then

                Using sql As New SQLexecuteObject(MyLegacyDatabaseConnection)
                    sql.queryOrStoredProc = "sp_DeleteAgencyUser"
                    sql.inputParameter = New SqlClient.SqlParameter("@user_id", MyLegacyUserID)

                    sql.ExecuteStatement()

                    If sql.rowsAffected = 0 Then
                        MyLegacySaveError = "No Rows Affected"
                        SetError(UserErrorType.SaveDelete, "Legacy Delete Routine Failed")
                    End If
                End Using
            Else
                MyLegacySaveError = "User ID has a pending EFT payment"
                SetError(UserErrorType.SaveDelete, "Cannot delete Legacy users with pending EFT payments")
            End If
        End If
    End Sub
    Private Sub DeleteQuoteMaster()
        If CheckedQuoteMaster = False Then
            CheckQuoteMasterAgency()
        End If

        If MyIsInQuoteMasterFlag = True Then

            Using sql As New SQLexecuteObject(MyLegacyDatabaseConnection)
                sql.queryOrStoredProc = "sp_Delete_AgencyUser_QM"

                Dim params As New ArrayList

                params.Add(New SqlClient.SqlParameter("@agencyID", MyQuoteMasterAgencyID))
                params.Add(New SqlClient.SqlParameter("@userName", MyUserName))

                sql.inputParameters = params

                sql.ExecuteStatement()

                If sql.rowsAffected = 0 Then
                    MyQuoteMasterSaveError = "No Rows Affected"
                    SetError(UserErrorType.SaveDelete, "QuoteMaster Delete Routine Failed")
                End If
            End Using

        End If
    End Sub
    Private Sub DeleteDiamond()

    End Sub

    Private Function HasLegacyEFTPayment() As Boolean
        HasLegacyEFTPayment = False

        Using sql As New SQLselectObject(MyLegacyDatabaseConnection)
            sql.queryOrStoredProc = "sp_GetAgencyUserEFT"
            sql.parameter = New SqlClient.SqlParameter("@user_id", MyLegacyUserID)

            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

            If Not dr Is Nothing Then
                If dr.HasRows Then
                    HasLegacyEFTPayment = True
                Else
                    If sql.hasError = True Then
                        'SQL error
                        SetError(UserErrorType.Lookup, "Error querying Legacy database")
                    Else
                        'no info found
                    End If
                End If
            Else
                'error
                SetError(UserErrorType.Lookup, "Error querying Legacy database")
            End If
        End Using
    End Function

    Private Sub SetError(ByVal errorType As UserErrorType, ByVal errorMsg As String)
        MyErrorFlag = True

        Select Case errorType
            Case UserErrorType.Lookup
                If MyErrorType = UserErrorType.SaveDelete Then
                    MyErrorType = UserErrorType.Both
                Else
                    MyErrorType = UserErrorType.Lookup
                End If

                If MyLookupError = "" Then
                    MyLookupError = errorMsg
                Else
                    MyLookupError &= "; " & errorMsg
                End If
            Case UserErrorType.SaveDelete
                If MyErrorType = UserErrorType.Lookup Then
                    MyErrorType = UserErrorType.Both
                Else
                    MyErrorType = UserErrorType.SaveDelete
                End If

                If MySaveError = "" Then
                    MySaveError = errorMsg
                Else
                    MySaveError &= "; " & errorMsg
                End If
        End Select
    End Sub

    Private Function ConvertBooleanToChar(ByVal flag As Boolean) As String
        ConvertBooleanToChar = ""

        If flag = True Then
            ConvertBooleanToChar = "Y"
        ElseIf flag = False Then
            ConvertBooleanToChar = "N"
        End If
    End Function

    Private Function ConvertCharToBoolean(ByVal chr As String) As Boolean
        ConvertCharToBoolean = False

        If UCase(chr) = "Y" Then
            ConvertCharToBoolean = True
        ElseIf UCase(chr) = "N" Then
            ConvertCharToBoolean = False
        End If
    End Function

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free unmanaged resources when explicitly called

                If Not MyLegacyDatabaseConnection Is Nothing Then
                    MyLegacyDatabaseConnection = Nothing
                End If
                If Not MyQuoteMasterDatabaseConnection Is Nothing Then
                    MyQuoteMasterDatabaseConnection = Nothing
                End If
                If Not MyDiamondDatabaseConnection Is Nothing Then
                    MyDiamondDatabaseConnection = Nothing
                End If

                If Not MyUserID Is Nothing Then
                    MyUserID = Nothing
                End If
                If Not MyRole Is Nothing Then
                    MyRole = Nothing
                End If
                If Not MyFullName Is Nothing Then
                    MyFullName = Nothing
                End If
                If Not MyUserName Is Nothing Then
                    MyUserName = Nothing
                End If
                If Not MyPassword Is Nothing Then
                    MyPassword = Nothing
                End If
                If Not MyAuthType Is Nothing Then
                    MyAuthType = Nothing
                End If
                If Not MyAgencyID Is Nothing Then
                    MyAgencyID = Nothing
                End If
                If Not MyHint Is Nothing Then
                    MyHint = Nothing
                End If
                If Not MyFirstName Is Nothing Then
                    MyFirstName = Nothing
                End If
                If Not MyMiddleName Is Nothing Then
                    MyMiddleName = Nothing
                End If
                If Not MyLastName Is Nothing Then
                    MyLastName = Nothing
                End If
                If Not MyAccess6000Flag = Nothing Then
                    MyAccess6000Flag = Nothing
                End If
                If Not MyUpdatedForDiamondFlag = Nothing Then
                    MyUpdatedForDiamondFlag = Nothing
                End If
                If Not MySecurityQuestion1 Is Nothing Then
                    MySecurityQuestion1 = Nothing
                End If
                If Not MySecurityAnswer1 Is Nothing Then
                    MySecurityAnswer1 = Nothing
                End If
                If Not MySecurityQuestion2 Is Nothing Then
                    MySecurityQuestion2 = Nothing
                End If
                If Not MySecurityAnswer2 Is Nothing Then
                    MySecurityAnswer2 = Nothing
                End If
                If Not MySecurityQuestion3 Is Nothing Then
                    MySecurityQuestion3 = Nothing
                End If
                If Not MySecurityAnswer3 Is Nothing Then
                    MySecurityAnswer3 = Nothing
                End If
                If Not MyEmailAddress Is Nothing Then
                    MyEmailAddress = Nothing
                End If

                If Not MyAgencyCode Is Nothing Then
                    MyAgencyCode = Nothing
                End If
                If Not MyAgencyTerritory Is Nothing Then
                    MyAgencyTerritory = Nothing
                End If
                If Not MyCompletedSurveyFlag = Nothing Then
                    MyCompletedSurveyFlag = Nothing
                End If
                If Not MyAgencyName Is Nothing Then
                    MyAgencyName = Nothing
                End If

                If Not MyHasInfoFlag = Nothing Then
                    MyHasInfoFlag = Nothing
                End If

                If Not CheckedLegacy = Nothing Then
                    CheckedLegacy = Nothing
                End If
                If Not CheckedQuoteMaster = Nothing Then
                    CheckedQuoteMaster = Nothing
                End If
                If Not CheckedDiamond = Nothing Then
                    CheckedDiamond = Nothing
                End If

                If Not MyIsInLegacyFlag = Nothing Then
                    MyIsInLegacyFlag = Nothing
                End If
                If Not MyIsInQuoteMasterFlag = Nothing Then
                    MyIsInQuoteMasterFlag = Nothing
                End If
                If Not MyIsInDiamondFlag = Nothing Then
                    MyIsInDiamondFlag = Nothing
                End If

                If Not MyErrorFlag = Nothing Then
                    MyErrorFlag = Nothing
                End If
                If Not MyErrorType = Nothing Then
                    MyErrorType = Nothing
                End If
                If Not MyLookupError Is Nothing Then
                    MyLookupError = Nothing
                End If
                If Not MySaveError Is Nothing Then
                    MySaveError = Nothing
                End If
                If Not MySaveErrorSystem = Nothing Then
                    MySaveErrorSystem = Nothing
                End If
                If Not MyLegacySaveError Is Nothing Then
                    MyLegacySaveError = Nothing
                End If
                If Not MyQuoteMasterSaveError Is Nothing Then
                    MyQuoteMasterSaveError = Nothing
                End If
                If Not MyDiamondSaveError Is Nothing Then
                    MyDiamondSaveError = Nothing
                End If

                If Not MySQLSelectObject Is Nothing Then
                    MySQLSelectObject.Dispose()
                    MySQLSelectObject = Nothing
                End If
                If Not MySQLExecuteObject Is Nothing Then
                    MySQLExecuteObject.Dispose()
                    MySQLExecuteObject = Nothing
                End If

                If Not MyDataReader Is Nothing Then
                    If Not MyDataReader.IsClosed Then
                        MyDataReader.Close()
                    End If
                    MyDataReader.Dispose()
                    MyDataReader = Nothing
                End If

                If Not MyDatabaseParameter Is Nothing Then
                    MyDatabaseParameter = Nothing
                End If
                If Not MyDatabaseParameters Is Nothing Then
                    MyDatabaseParameters.Clear()
                    MyDatabaseParameters = Nothing
                End If

                If Not IsUserNameOK = Nothing Then
                    IsUserNameOK = Nothing
                End If
                If Not IsPasswordOK = Nothing Then
                    IsPasswordOK = Nothing
                End If
                If Not MyUserNameError Is Nothing Then
                    MyUserNameError = Nothing
                End If
                If Not MyPasswordError Is Nothing Then
                    MyPasswordError = Nothing
                End If

                If Not holdCount = Nothing Then
                    holdCount = Nothing
                End If

                If Not MyAgencyCodes Is Nothing Then
                    MyAgencyCodes = Nothing
                End If
                If Not MyAgencyIDs Is Nothing Then
                    MyAgencyIDs = Nothing
                End If

                If Not MyCheckedAssociateCodesFlag = Nothing Then
                    MyCheckedAssociateCodesFlag = Nothing
                End If

                If Not MySourceSystem = Nothing Then
                    MySourceSystem = Nothing
                End If

                If Not MyDiamondAgencyCode Is Nothing Then
                    MyDiamondAgencyCode = Nothing
                End If
                If Not My6000Code Is Nothing Then
                    My6000Code = Nothing
                End If
                If Not MyDiamondAgencyID Is Nothing Then
                    MyDiamondAgencyID = Nothing
                End If
                If Not MyQuoteMasterAgencyID Is Nothing Then
                    MyQuoteMasterAgencyID = Nothing
                End If

                If Not QM_existingProducerID Is Nothing Then
                    QM_existingProducerID = Nothing
                End If
                If Not QM_st Is Nothing Then
                    QM_st = Nothing
                End If
                If Not QM_cp Is Nothing Then
                    QM_cp = Nothing
                End If
                If Not QM_autoCredit Is Nothing Then
                    QM_autoCredit = Nothing
                End If
                If Not QM_propCredit Is Nothing Then
                    QM_propCredit = Nothing
                End If
                If Not QM_mvr Is Nothing Then
                    QM_mvr = Nothing
                End If
                If Not QM_autoClue Is Nothing Then
                    QM_autoClue = Nothing
                End If
                If Not QM_propClue Is Nothing Then
                    QM_propClue = Nothing
                End If

                If Not MyOldUserName Is Nothing Then
                    MyOldUserName = Nothing
                End If

                If Not MyLegacyUserID Is Nothing Then
                    MyLegacyUserID = Nothing
                End If
                If Not MyDiamondUserID Is Nothing Then
                    MyDiamondUserID = Nothing
                End If

                If Not MySecurityQuestion1ID Is Nothing Then
                    MySecurityQuestion1ID = Nothing
                End If
                If Not MySecurityQuestion2ID Is Nothing Then
                    MySecurityQuestion2ID = Nothing
                End If
                If Not MySecurityQuestion3ID Is Nothing Then
                    MySecurityQuestion3ID = Nothing
                End If

            End If

            ' TODO: free shared unmanaged resources
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
