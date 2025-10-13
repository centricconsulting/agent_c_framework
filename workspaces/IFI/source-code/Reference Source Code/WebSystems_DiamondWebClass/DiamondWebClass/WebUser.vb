Public Class WebUser
#Region "Var"
    Private _legacy_Connection As String
    Private _diamond_Connection As String
    Private _quotemaster_Connection As String

    Private _diamond_UserID As Integer
    Private _diamond_LoginDomain As String
    Private _diamond_LoginName As String
    Private _diamond_IsUnderwriter As Boolean
    Private _diamond_Password As String
    Private _diamond_SUId As String
    Private _diamond_IsSupervisor As Boolean
    Private _diamond_UserCategoryID As Enums.UserCategoryID
    Private _diamond_UserCode As String
    Private _diamond_UserEmailAddress As String
    Private _diamond_PrimaryAgencyID As Integer
    Private _diamond_SecondaryAgencyIDs As List(Of Integer)
    Private _diamond_PrimaryAgencyCode As Integer
    Private _diamond_SecondaryAgencyCodes As List(Of Integer)
    Private _diamond_IsAgencyAdmin As Boolean
    Private _diamond_ListOfDiamondSecurityQuestions As List(Of DiamondSecurityQuestion)
    Private _diamond_Territory As Integer
    Private _diamond_MustChangePassword As Boolean
    Private _diamond_Active As Boolean
    Private _diamond_UserType As String
    Private _diamond_Department As String

    Private _legacy_UserID As Integer
    Private _legacy_Username As String
    Private _legacy_OldUsername As String
    Private _legacy_Password As String
    Private _legacy_Hint As String
    Private _legacy_AgencyID As Integer
    Private _legacy_AuthType As String
    Private _legacy_FirstName As String
    Private _legacy_MiddleName As String
    Private _legacy_LastName As String
    Private _legacy_6000Access As Boolean
    Private _legacy_Role As String
    Private _legacy_Territory As Integer
    Private _legacy_PrimaryAgencyCode As Integer
    Private _legacy_SecondaryAgencyCodes As List(Of Integer)
    Private _legacy_ConvertedToDiamond As Boolean
    Private _legacy_EmailAddress As String
    Private _legacy_CompletedSurvey As Boolean
    Private _legacy_AgencyName As String
    Private _legacy_Active As Boolean

    Private _QM_existingProducerID As String
    Private _QM_st As String
    Private _QM_cp As String
    Private _QM_autoCredit As String
    Private _QM_propCredit As String
    Private _QM_mvr As String
    Private _QM_autoClue As String
    Private _QM_propClue As String
    Private _QM_agencyID As Integer

    Public userInLegacy As Boolean = False, userInDiamond As Boolean = False, userInQM As Boolean = False

    Private _SystemsLocated As Enums.UserLocation

    Private _diamond_UserAgencyLink_CaughtError As Boolean 'added 3/21/2022
#End Region

    'diamond v1
    Public Sub New(ByVal user As DCO.Lookup.GetUser, ByVal securityQuestions As List(Of DiamondSecurityQuestion), ByVal userAgencies As Hashtable)
        SetDefaults()

        With user
            diamond_UserID = user.UsersId
            diamond_LoginDomain = .LoginDomain
            diamond_LoginName = .LoginName
            diamond_IsUnderwriter = .NotifyUW
            diamond_Password = String.Empty
            diamond_SUId = String.Empty
            diamond_IsSupervisor = False
            diamond_UserCategoryID = .UserCategoryId
            diamond_UserCode = .Code
            diamond_UserEmailAddress = .EmailAddress
            For Each item As DictionaryEntry In userAgencies
                If item.Value = DCE.UserAgencyRelationType.UserAgencyRelationType_PRIMARY Then diamond_PrimaryAgencyID = item.Key Else diamond_SecondaryAgencyIDs.Add(item.Key)
            Next
            diamond_ListOfDiamondSecurityQuestions = securityQuestions
            diamond_MustChangePassword = checkPWflag(.UsersId)
            diamond_Active = .Active
            getUserTypeAndDept()
        End With
        SystemsLocated = Enums.UserLocation.Diamond
    End Sub

    'diamond v2
    Public Sub New(ByVal dia_Domain As String, ByVal dia_Name As String, ByVal dia_Password As String, ByVal dia_SUID As String, ByVal dia_Code As String, ByVal dia_EmailAddr As String, _
                   ByVal dia_PrimaryAgencyCode As Integer, ByVal dia_SecondaryAgencyCodes As List(Of Integer), ByVal dia_IsSupervisor As Boolean, ByVal dia_IsUnderwriter As Boolean, _
                   ByVal dia_IsAgencyAdmin As Boolean, ByVal dia_ChangePW As Boolean, ByVal dia_CategoryID As Enums.UserCategoryID, ByVal dia_Territory As Integer, _
                   ByVal securityQuestions As List(Of DiamondSecurityQuestion))
        SetDefaults()

        If dia_Domain IsNot Nothing Then diamond_LoginDomain = dia_Domain Else diamond_LoginDomain = String.Empty
        If dia_Name IsNot Nothing Then diamond_LoginName = dia_Name Else diamond_LoginName = String.Empty
        If dia_Password IsNot Nothing Then diamond_Password = dia_Password Else diamond_Password = String.Empty
        If dia_SUID IsNot Nothing Then diamond_SUId = dia_SUID Else diamond_SUId = String.Empty
        If dia_Code IsNot Nothing Then diamond_UserCode = dia_Code Else diamond_UserCode = String.Empty
        If dia_EmailAddr IsNot Nothing Then diamond_UserEmailAddress = dia_EmailAddr Else diamond_UserEmailAddress = String.Empty
        diamond_IsSupervisor = dia_IsSupervisor
        diamond_IsUnderwriter = dia_IsUnderwriter
        diamond_UserCategoryID = dia_CategoryID
        diamond_ListOfDiamondSecurityQuestions = securityQuestions
        diamond_PrimaryAgencyCode = dia_PrimaryAgencyCode
        diamond_PrimaryAgencyID = getPrimaryAgencyID(diamond_PrimaryAgencyCode)
        If dia_SecondaryAgencyCodes IsNot Nothing Then
            diamond_SecondaryAgencyCodes = dia_SecondaryAgencyCodes
            diamond_SecondaryAgencyIDs = getSecondaryAgencyIDs(diamond_SecondaryAgencyCodes)
        End If
        diamond_IsAgencyAdmin = dia_IsAgencyAdmin
        diamond_Territory = dia_Territory
        diamond_MustChangePassword = dia_ChangePW
        diamond_Active = True
        SystemsLocated = Enums.UserLocation.Diamond
    End Sub

    'legacy
    Public Sub New(ByVal leg_UserID As Integer, ByVal leg_Username As String, ByVal leg_Password As String, ByVal leg_Hint As String, ByVal leg_AgencyID As Integer, ByVal leg_AuthType As String, _
                   ByVal leg_FirstName As String, ByVal leg_MiddleName As String, ByVal leg_LastName As String, ByVal leg_6000Access As String, ByVal leg_Role As String, ByVal leg_Territory As Integer, _
                   ByVal leg_PrimaryAgencyCode As Integer, ByVal leg_SecondaryAgencyCodes As List(Of Integer))
        SetDefaults()

        If leg_UserID > 0 Then legacy_UserID = leg_UserID Else legacy_UserID = -1
        If leg_Username IsNot Nothing Then legacy_Username = leg_Username Else legacy_Username = String.Empty
        If leg_Password IsNot Nothing Then legacy_Password = leg_Password Else legacy_Password = String.Empty
        If leg_Hint IsNot Nothing Then legacy_Hint = leg_Hint Else legacy_Hint = String.Empty
        If leg_AgencyID > 0 Then legacy_AgencyID = leg_AgencyID Else legacy_AgencyID = -1
        If leg_AuthType IsNot Nothing Then legacy_AuthType = leg_AuthType Else legacy_AuthType = String.Empty
        If leg_FirstName IsNot Nothing Then legacy_FirstName = leg_FirstName Else legacy_FirstName = String.Empty
        If leg_MiddleName IsNot Nothing Then legacy_MiddleName = leg_MiddleName Else legacy_MiddleName = String.Empty
        If leg_LastName IsNot Nothing Then legacy_LastName = leg_LastName Else legacy_LastName = String.Empty
        If leg_6000Access IsNot Nothing Then legacy_6000Access = leg_6000Access Else legacy_6000Access = String.Empty
        If leg_Role IsNot Nothing Then legacy_Role = leg_Role Else legacy_Role = String.Empty
        If leg_Territory > 0 Then legacy_Territory = leg_Territory Else legacy_Territory = -1
        If leg_PrimaryAgencyCode > 0 Then legacy_PrimaryAgencyCode = leg_PrimaryAgencyCode Else legacy_PrimaryAgencyCode = -1
        If leg_SecondaryAgencyCodes IsNot Nothing Then legacy_SecondaryAgencyCodes = leg_SecondaryAgencyCodes
        legacy_Active = True
        SystemsLocated = Enums.UserLocation.Legacy
    End Sub

    'quotemaster
    Public Sub New(ByVal quote_ExistingProducerID As String, ByVal quote_St As String, ByVal quote_Cp As String, ByVal quote_AutoCredit As String, ByVal quote_PropCredit As String, _
                   ByVal quote_MVR As String, ByVal quote_AutoClue As String, ByVal quote_PropClue As String)
        SetDefaults()

        If quote_ExistingProducerID IsNot Nothing Then QM_existingProducerID = quote_ExistingProducerID Else QM_existingProducerID = String.Empty
        If quote_St IsNot Nothing Then QM_st = quote_St Else QM_st = String.Empty
        If quote_Cp IsNot Nothing Then QM_cp = quote_Cp Else QM_cp = String.Empty
        If quote_AutoCredit IsNot Nothing Then QM_autoCredit = quote_AutoCredit Else QM_autoCredit = String.Empty
        If quote_PropCredit IsNot Nothing Then QM_propCredit = quote_PropCredit Else QM_propCredit = String.Empty
        If quote_MVR IsNot Nothing Then QM_mvr = quote_MVR Else QM_mvr = String.Empty
        If quote_AutoClue IsNot Nothing Then QM_autoClue = quote_AutoClue Else QM_autoClue = String.Empty
        If quote_PropClue IsNot Nothing Then QM_propClue = quote_PropClue Else QM_propClue = String.Empty
        SystemsLocated = Enums.UserLocation.Quotemaster
    End Sub

    'dia/leg v1
    Public Sub New(ByVal user As DCO.Lookup.GetUser, ByVal securityQuestions As List(Of DiamondSecurityQuestion), ByVal userAgencies As Hashtable, _
                   ByVal leg_UserID As Integer, ByVal leg_Username As String, ByVal leg_Password As String, ByVal leg_Hint As String, ByVal leg_AgencyID As Integer, ByVal leg_AuthType As String, _
                   ByVal leg_FirstName As String, ByVal leg_MiddleName As String, ByVal leg_LastName As String, ByVal leg_6000Access As String, ByVal leg_Role As String, ByVal leg_Territory As Integer, _
                   ByVal leg_PrimaryAgencyCode As Integer, ByVal leg_SecondaryAgencyCodes As List(Of Integer))
        SetDefaults()

        With user
            diamond_UserID = user.UsersId
            diamond_LoginDomain = .LoginDomain
            diamond_LoginName = .LoginName
            diamond_IsUnderwriter = .NotifyUW
            diamond_Password = String.Empty
            diamond_SUId = String.Empty
            diamond_IsSupervisor = False
            diamond_UserCategoryID = .UserCategoryId
            diamond_UserCode = .Code
            diamond_UserEmailAddress = .EmailAddress
            For Each item As DictionaryEntry In userAgencies
                If item.Value = DCE.UserAgencyRelationType.UserAgencyRelationType_PRIMARY Then diamond_PrimaryAgencyID = item.Key Else diamond_SecondaryAgencyIDs.Add(item.Key)
            Next
            diamond_ListOfDiamondSecurityQuestions = securityQuestions
            diamond_MustChangePassword = checkPWflag(.UsersId)
            diamond_Active = .Active
            getUserTypeAndDept()
        End With

        If leg_UserID > 0 Then legacy_UserID = leg_UserID Else legacy_UserID = -1
        If leg_Username IsNot Nothing Then legacy_Username = leg_Username Else legacy_Username = String.Empty
        If leg_Password IsNot Nothing Then legacy_Password = leg_Password Else legacy_Password = String.Empty
        If leg_Hint IsNot Nothing Then legacy_Hint = leg_Hint Else legacy_Hint = String.Empty
        If leg_AgencyID > 0 Then legacy_AgencyID = leg_AgencyID Else legacy_AgencyID = -1
        If leg_AuthType IsNot Nothing Then legacy_AuthType = leg_AuthType Else legacy_AuthType = String.Empty
        If leg_FirstName IsNot Nothing Then legacy_FirstName = leg_FirstName Else legacy_FirstName = String.Empty
        If leg_MiddleName IsNot Nothing Then legacy_MiddleName = leg_MiddleName Else legacy_MiddleName = String.Empty
        If leg_LastName IsNot Nothing Then legacy_LastName = leg_LastName Else legacy_LastName = String.Empty
        If leg_6000Access IsNot Nothing Then legacy_6000Access = leg_6000Access Else legacy_6000Access = String.Empty
        If leg_Role IsNot Nothing Then legacy_Role = leg_Role Else legacy_Role = String.Empty
        If leg_Territory > 0 Then legacy_Territory = leg_Territory Else legacy_Territory = -1
        If leg_PrimaryAgencyCode > 0 Then legacy_PrimaryAgencyCode = leg_PrimaryAgencyCode Else legacy_PrimaryAgencyCode = -1
        If leg_SecondaryAgencyCodes IsNot Nothing Then legacy_SecondaryAgencyCodes = leg_SecondaryAgencyCodes
        legacy_Active = True

        SystemsLocated = Enums.UserLocation.DiamondAndLegacy
    End Sub

    'dia/leg v2
    Public Sub New(ByVal dia_Domain As String, ByVal dia_Name As String, ByVal dia_Password As String, ByVal dia_SUID As String, ByVal dia_Code As String, ByVal dia_EmailAddr As String, _
                   ByVal dia_PrimaryAgencyCode As Integer, ByVal dia_SecondaryAgencyCodes As List(Of Integer), ByVal dia_IsSupervisor As Boolean, ByVal dia_IsUnderwriter As Boolean, _
                   ByVal dia_IsAgencyAdmin As Boolean, ByVal dia_ChangePW As Boolean, ByVal dia_CategoryID As Enums.UserCategoryID, ByVal dia_Territory As Integer, ByVal securityQuestions As List(Of DiamondSecurityQuestion), _
                   ByVal leg_UserID As Integer, ByVal leg_Username As String, ByVal leg_Password As String, ByVal leg_Hint As String, ByVal leg_AgencyID As Integer, ByVal leg_AuthType As String, _
                   ByVal leg_FirstName As String, ByVal leg_MiddleName As String, ByVal leg_LastName As String, ByVal leg_6000Access As String, ByVal leg_Role As String, ByVal leg_Territory As Integer, _
                   ByVal leg_PrimaryAgencyCode As Integer, ByVal leg_SecondaryAgencyCodes As List(Of Integer))
        SetDefaults()

        If dia_Domain IsNot Nothing Then diamond_LoginDomain = dia_Domain Else diamond_LoginDomain = String.Empty
        If dia_Name IsNot Nothing Then diamond_LoginName = dia_Name Else diamond_LoginName = String.Empty
        If dia_Password IsNot Nothing Then diamond_Password = dia_Password Else diamond_Password = String.Empty
        If dia_SUID IsNot Nothing Then diamond_SUId = dia_SUID Else diamond_SUId = String.Empty
        If dia_Code IsNot Nothing Then diamond_UserCode = dia_Code Else diamond_UserCode = String.Empty
        If dia_EmailAddr IsNot Nothing Then diamond_UserEmailAddress = dia_EmailAddr Else diamond_UserEmailAddress = String.Empty
        diamond_IsSupervisor = dia_IsSupervisor
        diamond_IsUnderwriter = dia_IsUnderwriter
        diamond_UserCategoryID = dia_CategoryID
        diamond_ListOfDiamondSecurityQuestions = securityQuestions
        diamond_PrimaryAgencyID = getPrimaryAgencyID(dia_PrimaryAgencyCode)
        If dia_SecondaryAgencyCodes IsNot Nothing Then diamond_SecondaryAgencyIDs = getSecondaryAgencyIDs(dia_SecondaryAgencyCodes)
        diamond_IsAgencyAdmin = dia_IsAgencyAdmin
        diamond_Territory = dia_Territory
        diamond_MustChangePassword = dia_ChangePW
        diamond_Active = True

        If leg_UserID > 0 Then legacy_UserID = leg_UserID Else legacy_UserID = -1
        If leg_Username IsNot Nothing Then legacy_Username = leg_Username Else legacy_Username = String.Empty
        If leg_Password IsNot Nothing Then legacy_Password = leg_Password Else legacy_Password = String.Empty
        If leg_Hint IsNot Nothing Then legacy_Hint = leg_Hint Else legacy_Hint = String.Empty
        If leg_AgencyID > 0 Then legacy_AgencyID = leg_AgencyID Else legacy_AgencyID = -1
        If leg_AuthType IsNot Nothing Then legacy_AuthType = leg_AuthType Else legacy_AuthType = String.Empty
        If leg_FirstName IsNot Nothing Then legacy_FirstName = leg_FirstName Else legacy_FirstName = String.Empty
        If leg_MiddleName IsNot Nothing Then legacy_MiddleName = leg_MiddleName Else legacy_MiddleName = String.Empty
        If leg_LastName IsNot Nothing Then legacy_LastName = leg_LastName Else legacy_LastName = String.Empty
        If leg_6000Access IsNot Nothing Then legacy_6000Access = leg_6000Access Else legacy_6000Access = String.Empty
        If leg_Role IsNot Nothing Then legacy_Role = leg_Role Else legacy_Role = String.Empty
        If leg_Territory > 0 Then legacy_Territory = leg_Territory Else legacy_Territory = -1
        If leg_PrimaryAgencyCode > 0 Then legacy_PrimaryAgencyCode = leg_PrimaryAgencyCode Else legacy_PrimaryAgencyCode = -1
        If leg_SecondaryAgencyCodes IsNot Nothing Then legacy_SecondaryAgencyCodes = leg_SecondaryAgencyCodes
        legacy_Active = True

        SystemsLocated = Enums.UserLocation.DiamondAndLegacy
    End Sub

    'dia/qm v1
    Public Sub New(ByVal user As DCO.Lookup.GetUser, ByVal securityQuestions As List(Of DiamondSecurityQuestion), ByVal userAgencies As Hashtable, _
                   ByVal quote_ExistingProducerID As String, ByVal quote_St As String, ByVal quote_Cp As String, ByVal quote_AutoCredit As String, ByVal quote_PropCredit As String, _
                   ByVal quote_MVR As String, ByVal quote_AutoClue As String, ByVal quote_PropClue As String)
        SetDefaults()

        With user
            diamond_UserID = user.UsersId
            diamond_LoginDomain = .LoginDomain
            diamond_LoginName = .LoginName
            diamond_IsUnderwriter = .NotifyUW
            diamond_Password = String.Empty
            diamond_SUId = String.Empty
            diamond_IsSupervisor = False
            diamond_UserCategoryID = .UserCategoryId
            diamond_UserCode = .Code
            diamond_UserEmailAddress = .EmailAddress
            For Each item As DictionaryEntry In userAgencies
                If item.Value = DCE.UserAgencyRelationType.UserAgencyRelationType_PRIMARY Then diamond_PrimaryAgencyID = item.Key Else diamond_SecondaryAgencyIDs.Add(item.Key)
            Next
            diamond_ListOfDiamondSecurityQuestions = securityQuestions
            diamond_MustChangePassword = checkPWflag(.UsersId)
            diamond_Active = .Active
            getUserTypeAndDept()
        End With

        If quote_ExistingProducerID IsNot Nothing Then QM_existingProducerID = quote_ExistingProducerID Else QM_existingProducerID = String.Empty
        If quote_St IsNot Nothing Then QM_st = quote_St Else QM_st = String.Empty
        If quote_Cp IsNot Nothing Then QM_cp = quote_Cp Else QM_cp = String.Empty
        If quote_AutoCredit IsNot Nothing Then QM_autoCredit = quote_AutoCredit Else QM_autoCredit = String.Empty
        If quote_PropCredit IsNot Nothing Then QM_propCredit = quote_PropCredit Else QM_propCredit = String.Empty
        If quote_MVR IsNot Nothing Then QM_mvr = quote_MVR Else QM_mvr = String.Empty
        If quote_AutoClue IsNot Nothing Then QM_autoClue = quote_AutoClue Else QM_autoClue = String.Empty
        If quote_PropClue IsNot Nothing Then QM_propClue = quote_PropClue Else QM_propClue = String.Empty

        SystemsLocated = Enums.UserLocation.DiamondAndQuotemaster
    End Sub

    'dia/qm v2
    Public Sub New(ByVal dia_Domain As String, ByVal dia_Name As String, ByVal dia_Password As String, ByVal dia_SUID As String, ByVal dia_Code As String, ByVal dia_EmailAddr As String, _
                   ByVal dia_PrimaryAgencyCode As Integer, ByVal dia_SecondaryAgencyCodes As List(Of Integer), ByVal dia_IsSupervisor As Boolean, ByVal dia_IsUnderwriter As Boolean, _
                   ByVal dia_IsAgencyAdmin As Boolean, ByVal dia_ChangePW As Boolean, ByVal dia_CategoryID As Enums.UserCategoryID, ByVal dia_Territory As Integer, ByVal securityQuestions As List(Of DiamondSecurityQuestion), _
                   ByVal quote_ExistingProducerID As String, ByVal quote_St As String, ByVal quote_Cp As String, ByVal quote_AutoCredit As String, ByVal quote_PropCredit As String, _
                   ByVal quote_MVR As String, ByVal quote_AutoClue As String, ByVal quote_PropClue As String)
        SetDefaults()

        If dia_Domain IsNot Nothing Then diamond_LoginDomain = dia_Domain Else diamond_LoginDomain = String.Empty
        If dia_Name IsNot Nothing Then diamond_LoginName = dia_Name Else diamond_LoginName = String.Empty
        If dia_Password IsNot Nothing Then diamond_Password = dia_Password Else diamond_Password = String.Empty
        If dia_SUID IsNot Nothing Then diamond_SUId = dia_SUID Else diamond_SUId = String.Empty
        If dia_Code IsNot Nothing Then diamond_UserCode = dia_Code Else diamond_UserCode = String.Empty
        If dia_EmailAddr IsNot Nothing Then diamond_UserEmailAddress = dia_EmailAddr Else diamond_UserEmailAddress = String.Empty
        diamond_IsSupervisor = dia_IsSupervisor
        diamond_IsUnderwriter = dia_IsUnderwriter
        diamond_UserCategoryID = dia_CategoryID
        diamond_ListOfDiamondSecurityQuestions = securityQuestions
        diamond_PrimaryAgencyID = getPrimaryAgencyID(dia_PrimaryAgencyCode)
        If dia_SecondaryAgencyCodes IsNot Nothing Then diamond_SecondaryAgencyIDs = getSecondaryAgencyIDs(dia_SecondaryAgencyCodes)
        diamond_IsAgencyAdmin = dia_IsAgencyAdmin
        diamond_Territory = dia_Territory
        diamond_MustChangePassword = dia_ChangePW
        diamond_Active = True

        If quote_ExistingProducerID IsNot Nothing Then QM_existingProducerID = quote_ExistingProducerID Else QM_existingProducerID = String.Empty
        If quote_St IsNot Nothing Then QM_st = quote_St Else QM_st = String.Empty
        If quote_Cp IsNot Nothing Then QM_cp = quote_Cp Else QM_cp = String.Empty
        If quote_AutoCredit IsNot Nothing Then QM_autoCredit = quote_AutoCredit Else QM_autoCredit = String.Empty
        If quote_PropCredit IsNot Nothing Then QM_propCredit = quote_PropCredit Else QM_propCredit = String.Empty
        If quote_MVR IsNot Nothing Then QM_mvr = quote_MVR Else QM_mvr = String.Empty
        If quote_AutoClue IsNot Nothing Then QM_autoClue = quote_AutoClue Else QM_autoClue = String.Empty
        If quote_PropClue IsNot Nothing Then QM_propClue = quote_PropClue Else QM_propClue = String.Empty
        SystemsLocated = Enums.UserLocation.Quotemaster
    End Sub

    'leg/qm
    Public Sub New(ByVal leg_UserID As Integer, ByVal leg_Username As String, ByVal leg_Password As String, ByVal leg_Hint As String, ByVal leg_AgencyID As Integer, ByVal leg_AuthType As String, _
                   ByVal leg_FirstName As String, ByVal leg_MiddleName As String, ByVal leg_LastName As String, ByVal leg_6000Access As String, ByVal leg_Role As String, ByVal leg_Territory As Integer, _
                   ByVal leg_PrimaryAgencyCode As Integer, ByVal leg_SecondaryAgencyCodes As List(Of Integer), _
                   ByVal quote_ExistingProducerID As String, ByVal quote_St As String, ByVal quote_Cp As String, ByVal quote_AutoCredit As String, ByVal quote_PropCredit As String, _
                   ByVal quote_MVR As String, ByVal quote_AutoClue As String, ByVal quote_PropClue As String)
        SetDefaults()

        If leg_UserID > 0 Then legacy_UserID = leg_UserID Else legacy_UserID = -1
        If leg_Username IsNot Nothing Then legacy_Username = leg_Username Else legacy_Username = String.Empty
        If leg_Password IsNot Nothing Then legacy_Password = leg_Password Else legacy_Password = String.Empty
        If leg_Hint IsNot Nothing Then legacy_Hint = leg_Hint Else legacy_Hint = String.Empty
        If leg_AgencyID > 0 Then legacy_AgencyID = leg_AgencyID Else legacy_AgencyID = -1
        If leg_AuthType IsNot Nothing Then legacy_AuthType = leg_AuthType Else legacy_AuthType = String.Empty
        If leg_FirstName IsNot Nothing Then legacy_FirstName = leg_FirstName Else legacy_FirstName = String.Empty
        If leg_MiddleName IsNot Nothing Then legacy_MiddleName = leg_MiddleName Else legacy_MiddleName = String.Empty
        If leg_LastName IsNot Nothing Then legacy_LastName = leg_LastName Else legacy_LastName = String.Empty
        If leg_6000Access IsNot Nothing Then legacy_6000Access = leg_6000Access Else legacy_6000Access = String.Empty
        If leg_Role IsNot Nothing Then legacy_Role = leg_Role Else legacy_Role = String.Empty
        If leg_Territory > 0 Then legacy_Territory = leg_Territory Else legacy_Territory = -1
        If leg_PrimaryAgencyCode > 0 Then legacy_PrimaryAgencyCode = leg_PrimaryAgencyCode Else legacy_PrimaryAgencyCode = -1
        If leg_SecondaryAgencyCodes IsNot Nothing Then legacy_SecondaryAgencyCodes = leg_SecondaryAgencyCodes
        legacy_Active = True

        If quote_ExistingProducerID IsNot Nothing Then QM_existingProducerID = quote_ExistingProducerID Else QM_existingProducerID = String.Empty
        If quote_St IsNot Nothing Then QM_st = quote_St Else QM_st = String.Empty
        If quote_Cp IsNot Nothing Then QM_cp = quote_Cp Else QM_cp = String.Empty
        If quote_AutoCredit IsNot Nothing Then QM_autoCredit = quote_AutoCredit Else QM_autoCredit = String.Empty
        If quote_PropCredit IsNot Nothing Then QM_propCredit = quote_PropCredit Else QM_propCredit = String.Empty
        If quote_MVR IsNot Nothing Then QM_mvr = quote_MVR Else QM_mvr = String.Empty
        If quote_AutoClue IsNot Nothing Then QM_autoClue = quote_AutoClue Else QM_autoClue = String.Empty
        If quote_PropClue IsNot Nothing Then QM_propClue = quote_PropClue Else QM_propClue = String.Empty

        SystemsLocated = Enums.UserLocation.LegacyAndQuotemaster
    End Sub

    'all v1
    Public Sub New(ByVal user As DCO.Lookup.GetUser, ByVal securityQuestions As List(Of DiamondSecurityQuestion), ByVal userAgencies As Hashtable, _
                   ByVal leg_UserID As Integer, ByVal leg_Username As String, ByVal leg_Password As String, ByVal leg_Hint As String, ByVal leg_AgencyID As Integer, ByVal leg_AuthType As String, _
                   ByVal leg_FirstName As String, ByVal leg_MiddleName As String, ByVal leg_LastName As String, ByVal leg_6000Access As String, ByVal leg_Role As String, ByVal leg_Territory As Integer, _
                   ByVal leg_PrimaryAgencyCode As Integer, ByVal leg_SecondaryAgencyCodes As List(Of Integer), _
                   ByVal quote_ExistingProducerID As String, ByVal quote_St As String, ByVal quote_Cp As String, ByVal quote_AutoCredit As String, ByVal quote_PropCredit As String, _
                   ByVal quote_MVR As String, ByVal quote_AutoClue As String, ByVal quote_PropClue As String)
        SetDefaults()

        With user
            diamond_UserID = user.UsersId
            diamond_LoginDomain = .LoginDomain
            diamond_LoginName = .LoginName
            diamond_IsUnderwriter = .NotifyUW
            diamond_Password = String.Empty
            diamond_SUId = String.Empty
            diamond_IsSupervisor = False
            diamond_UserCategoryID = .UserCategoryId
            diamond_UserCode = .Code
            diamond_UserEmailAddress = .EmailAddress
            For Each item As DictionaryEntry In userAgencies
                If item.Value = DCE.UserAgencyRelationType.UserAgencyRelationType_PRIMARY Then diamond_PrimaryAgencyID = item.Key Else diamond_SecondaryAgencyIDs.Add(item.Key)
            Next
            diamond_ListOfDiamondSecurityQuestions = securityQuestions
            diamond_MustChangePassword = checkPWflag(.UsersId)
            diamond_Active = .Active
            getUserTypeAndDept()
        End With

        If leg_UserID > 0 Then legacy_UserID = leg_UserID Else legacy_UserID = -1
        If leg_Username IsNot Nothing Then legacy_Username = leg_Username Else legacy_Username = String.Empty
        If leg_Password IsNot Nothing Then legacy_Password = leg_Password Else legacy_Password = String.Empty
        If leg_Hint IsNot Nothing Then legacy_Hint = leg_Hint Else legacy_Hint = String.Empty
        If leg_AgencyID > 0 Then legacy_AgencyID = leg_AgencyID Else legacy_AgencyID = -1
        If leg_AuthType IsNot Nothing Then legacy_AuthType = leg_AuthType Else legacy_AuthType = String.Empty
        If leg_FirstName IsNot Nothing Then legacy_FirstName = leg_FirstName Else legacy_FirstName = String.Empty
        If leg_MiddleName IsNot Nothing Then legacy_MiddleName = leg_MiddleName Else legacy_MiddleName = String.Empty
        If leg_LastName IsNot Nothing Then legacy_LastName = leg_LastName Else legacy_LastName = String.Empty
        If leg_6000Access IsNot Nothing Then legacy_6000Access = leg_6000Access Else legacy_6000Access = String.Empty
        If leg_Role IsNot Nothing Then legacy_Role = leg_Role Else legacy_Role = String.Empty
        If leg_Territory > 0 Then legacy_Territory = leg_Territory Else legacy_Territory = -1
        If leg_PrimaryAgencyCode > 0 Then legacy_PrimaryAgencyCode = leg_PrimaryAgencyCode Else legacy_PrimaryAgencyCode = -1
        If leg_SecondaryAgencyCodes IsNot Nothing Then legacy_SecondaryAgencyCodes = leg_SecondaryAgencyCodes
        legacy_Active = True

        If quote_ExistingProducerID IsNot Nothing Then QM_existingProducerID = quote_ExistingProducerID Else QM_existingProducerID = String.Empty
        If quote_St IsNot Nothing Then QM_st = quote_St Else QM_st = String.Empty
        If quote_Cp IsNot Nothing Then QM_cp = quote_Cp Else QM_cp = String.Empty
        If quote_AutoCredit IsNot Nothing Then QM_autoCredit = quote_AutoCredit Else QM_autoCredit = String.Empty
        If quote_PropCredit IsNot Nothing Then QM_propCredit = quote_PropCredit Else QM_propCredit = String.Empty
        If quote_MVR IsNot Nothing Then QM_mvr = quote_MVR Else QM_mvr = String.Empty
        If quote_AutoClue IsNot Nothing Then QM_autoClue = quote_AutoClue Else QM_autoClue = String.Empty
        If quote_PropClue IsNot Nothing Then QM_propClue = quote_PropClue Else QM_propClue = String.Empty

        SystemsLocated = Enums.UserLocation.AllSystems
    End Sub

    'all v2
    Public Sub New(ByVal dia_Domain As String, ByVal dia_Name As String, ByVal dia_Password As String, ByVal dia_SUID As String, ByVal dia_Code As String, ByVal dia_EmailAddr As String, _
                   ByVal dia_PrimaryAgencyCode As Integer, ByVal dia_SecondaryAgencyCodes As List(Of Integer), ByVal dia_IsSupervisor As Boolean, ByVal dia_IsUnderwriter As Boolean, _
                   ByVal dia_IsAgencyAdmin As Boolean, ByVal dia_ChangePW As Boolean, ByVal dia_CategoryID As Enums.UserCategoryID, ByVal dia_Territory As Integer, ByVal securityQuestions As List(Of DiamondSecurityQuestion), _
                   ByVal leg_UserID As Integer, ByVal leg_Username As String, ByVal leg_Password As String, ByVal leg_Hint As String, ByVal leg_AgencyID As Integer, ByVal leg_AuthType As String, _
                   ByVal leg_FirstName As String, ByVal leg_MiddleName As String, ByVal leg_LastName As String, ByVal leg_6000Access As String, ByVal leg_Role As String, ByVal leg_Territory As Integer, _
                   ByVal leg_PrimaryAgencyCode As Integer, ByVal leg_SecondaryAgencyCodes As List(Of Integer), _
                   ByVal quote_ExistingProducerID As String, ByVal quote_St As String, ByVal quote_Cp As String, ByVal quote_AutoCredit As String, ByVal quote_PropCredit As String, _
                   ByVal quote_MVR As String, ByVal quote_AutoClue As String, ByVal quote_PropClue As String)
        SetDefaults()

        If dia_Domain IsNot Nothing Then diamond_LoginDomain = dia_Domain Else diamond_LoginDomain = String.Empty
        If dia_Name IsNot Nothing Then diamond_LoginName = dia_Name Else diamond_LoginName = String.Empty
        If dia_Password IsNot Nothing Then diamond_Password = dia_Password Else diamond_Password = String.Empty
        If dia_SUID IsNot Nothing Then diamond_SUId = dia_SUID Else diamond_SUId = String.Empty
        If dia_Code IsNot Nothing Then diamond_UserCode = dia_Code Else diamond_UserCode = String.Empty
        If dia_EmailAddr IsNot Nothing Then diamond_UserEmailAddress = dia_EmailAddr Else diamond_UserEmailAddress = String.Empty
        diamond_IsSupervisor = dia_IsSupervisor
        diamond_IsUnderwriter = dia_IsUnderwriter
        diamond_UserCategoryID = dia_CategoryID
        diamond_ListOfDiamondSecurityQuestions = securityQuestions
        diamond_PrimaryAgencyID = getPrimaryAgencyID(dia_PrimaryAgencyCode)
        If dia_SecondaryAgencyCodes IsNot Nothing Then diamond_SecondaryAgencyIDs = getSecondaryAgencyIDs(dia_SecondaryAgencyCodes)
        diamond_IsAgencyAdmin = dia_IsAgencyAdmin
        diamond_Territory = dia_Territory
        diamond_MustChangePassword = dia_ChangePW
        diamond_Active = True

        If leg_UserID > 0 Then legacy_UserID = leg_UserID Else legacy_UserID = -1
        If leg_Username IsNot Nothing Then legacy_Username = leg_Username Else legacy_Username = String.Empty
        If leg_Password IsNot Nothing Then legacy_Password = leg_Password Else legacy_Password = String.Empty
        If leg_Hint IsNot Nothing Then legacy_Hint = leg_Hint Else legacy_Hint = String.Empty
        If leg_AgencyID > 0 Then legacy_AgencyID = leg_AgencyID Else legacy_AgencyID = -1
        If leg_AuthType IsNot Nothing Then legacy_AuthType = leg_AuthType Else legacy_AuthType = String.Empty
        If leg_FirstName IsNot Nothing Then legacy_FirstName = leg_FirstName Else legacy_FirstName = String.Empty
        If leg_MiddleName IsNot Nothing Then legacy_MiddleName = leg_MiddleName Else legacy_MiddleName = String.Empty
        If leg_LastName IsNot Nothing Then legacy_LastName = leg_LastName Else legacy_LastName = String.Empty
        If leg_6000Access IsNot Nothing Then legacy_6000Access = leg_6000Access Else legacy_6000Access = String.Empty
        If leg_Role IsNot Nothing Then legacy_Role = leg_Role Else legacy_Role = String.Empty
        If leg_Territory > 0 Then legacy_Territory = leg_Territory Else legacy_Territory = -1
        If leg_PrimaryAgencyCode > 0 Then legacy_PrimaryAgencyCode = leg_PrimaryAgencyCode Else legacy_PrimaryAgencyCode = -1
        If leg_SecondaryAgencyCodes IsNot Nothing Then legacy_SecondaryAgencyCodes = leg_SecondaryAgencyCodes
        legacy_Active = True

        If quote_ExistingProducerID IsNot Nothing Then QM_existingProducerID = quote_ExistingProducerID Else QM_existingProducerID = String.Empty
        If quote_St IsNot Nothing Then QM_st = quote_St Else QM_st = String.Empty
        If quote_Cp IsNot Nothing Then QM_cp = quote_Cp Else QM_cp = String.Empty
        If quote_AutoCredit IsNot Nothing Then QM_autoCredit = quote_AutoCredit Else QM_autoCredit = String.Empty
        If quote_PropCredit IsNot Nothing Then QM_propCredit = quote_PropCredit Else QM_propCredit = String.Empty
        If quote_MVR IsNot Nothing Then QM_mvr = quote_MVR Else QM_mvr = String.Empty
        If quote_AutoClue IsNot Nothing Then QM_autoClue = quote_AutoClue Else QM_autoClue = String.Empty
        If quote_PropClue IsNot Nothing Then QM_propClue = quote_PropClue Else QM_propClue = String.Empty

        SystemsLocated = Enums.UserLocation.AllSystems
    End Sub

    Public Sub New()
        SetDefaults()
    End Sub

    Private Function checkPWflag(ByVal userID As Integer) As Boolean
        'CheckDatabaseConnections()
        'Using Sql As New SQLselectObject(diamond_Connection)
        '    Sql.queryOrStoredProc = "Select password_must_be_changed from Users where users_id = '" & userID & "'"
        '    Using dr As SqlDataReader = Sql.GetDataReader
        '        If dr IsNot Nothing AndAlso dr.HasRows Then
        '            dr.Read()
        '            If dr.Item("password_must_be_changed").ToString.ToLower = "true" Then Return True Else Return False
        '        End If
        '    End Using
        'End Using

        'updated 1/25/2022
        Dim mustChange As Boolean = False

        CheckDatabaseConnections()
        Using Sql As New SQLselectObject(diamond_Connection)
            Sql.queryOrStoredProc = "Select password_must_be_changed from Users where users_id = '" & userID & "'"
            Using dr As SqlDataReader = Sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    dr.Read()
                    Dim chc As New CommonHelperClass
                    mustChange = chc.BooleanForString(dr.Item("password_must_be_changed").ToString.Trim)
                End If
            End Using
        End Using

        Return mustChange
    End Function

    Private Sub SetDefaults()
        diamond_LoginName = String.Empty
        diamond_LoginDomain = "agency"
        diamond_Password = String.Empty
        diamond_SUId = String.Empty
        diamond_UserCode = String.Empty
        diamond_UserEmailAddress = String.Empty
        diamond_IsSupervisor = False
        diamond_IsUnderwriter = False
        diamond_UserCategoryID = Enums.UserCategoryID.Agency
        diamond_ListOfDiamondSecurityQuestions = New List(Of DiamondSecurityQuestion)
        diamond_PrimaryAgencyID = -1
        diamond_SecondaryAgencyIDs = New List(Of Integer)
        diamond_PrimaryAgencyCode = -1
        diamond_SecondaryAgencyCodes = New List(Of Integer)
        diamond_IsAgencyAdmin = False
        diamond_Territory = -1
        diamond_MustChangePassword = False
        diamond_UserID = -1
        diamond_UserType = String.Empty
        diamond_Department = String.Empty
        diamond_Active = False

        legacy_UserID = -1
        legacy_Username = String.Empty
        legacy_OldUsername = String.Empty
        legacy_Password = String.Empty
        legacy_Hint = String.Empty
        legacy_AgencyID = -1
        legacy_AuthType = String.Empty
        legacy_FirstName = String.Empty
        legacy_MiddleName = String.Empty
        legacy_LastName = String.Empty
        legacy_6000Access = False
        legacy_Role = String.Empty
        legacy_Territory = -1
        legacy_PrimaryAgencyCode = -1
        legacy_SecondaryAgencyCodes = New List(Of Integer)
        legacy_EmailAddress = String.Empty
        legacy_Active = False

        QM_existingProducerID = String.Empty
        QM_st = String.Empty
        QM_cp = String.Empty
        QM_autoCredit = String.Empty
        QM_propCredit = String.Empty
        QM_mvr = String.Empty
        QM_propClue = String.Empty
        QM_autoClue = String.Empty

        SystemsLocated = Enums.UserLocation.None

        diamond_UserAgencyLink_CaughtError = False 'added 3/21/2022
    End Sub

    Public Sub FillExistingUser(ByVal user As DCO.Lookup.GetUser, ByVal securityQuestions As List(Of DiamondSecurityQuestion), ByVal userAgencies As Hashtable)
        'With user
        '    diamond_UserID = user.UsersId
        '    If .UserCategoryId = 1 Then diamond_LoginDomain = "ifm.ifmic" Else diamond_LoginDomain = "agency"
        '    'diamond_LoginDomain = .LoginDomain
        '    diamond_LoginName = .LoginName
        '    diamond_IsUnderwriter = .NotifyUW
        '    diamond_Password = String.Empty
        '    diamond_SUId = String.Empty
        '    diamond_IsSupervisor = False
        '    diamond_UserCategoryID = .UserCategoryId
        '    diamond_UserCode = .Code
        '    diamond_UserEmailAddress = .EmailAddress
        '    For Each item As DictionaryEntry In userAgencies
        '        If item.Value = DCE.UserAgencyRelationType.UserAgencyRelationType_PRIMARY Then diamond_PrimaryAgencyID = item.Key Else diamond_SecondaryAgencyIDs.Add(item.Key)
        '    Next
        '    diamond_ListOfDiamondSecurityQuestions = securityQuestions
        '    diamond_MustChangePassword = checkPWflag(.UsersId)
        '    diamond_Active = .Active
        '    getUserTypeAndDept()
        'End With
        'SystemsLocated = Enums.UserLocation.Diamond
        'updated 1/24/2022 to use new method
        FillExistingUser_OptionallySetMustChangePassword(user, securityQuestions, userAgencies, setMustChangePassword:=False)
    End Sub
    Public Sub FillExistingUser_OptionallySetMustChangePassword(ByVal user As DCO.Lookup.GetUser, ByVal securityQuestions As List(Of DiamondSecurityQuestion), ByVal userAgencies As Hashtable, Optional ByVal setMustChangePassword As Boolean = False, Optional ByVal mustChangePasswordVal As Boolean = False, Optional ByVal userAgencyLink_caughtError As Boolean = False)
        With user
            diamond_UserID = user.UsersId
            If .UserCategoryId = 1 Then diamond_LoginDomain = "ifm.ifmic" Else diamond_LoginDomain = "agency"
            'diamond_LoginDomain = .LoginDomain
            diamond_LoginName = .LoginName
            diamond_IsUnderwriter = .NotifyUW
            diamond_Password = String.Empty
            diamond_SUId = String.Empty
            diamond_IsSupervisor = False
            diamond_UserCategoryID = .UserCategoryId
            diamond_UserCode = .Code
            diamond_UserEmailAddress = .EmailAddress
            For Each item As DictionaryEntry In userAgencies
                If item.Value = DCE.UserAgencyRelationType.UserAgencyRelationType_PRIMARY Then diamond_PrimaryAgencyID = item.Key Else diamond_SecondaryAgencyIDs.Add(item.Key)
            Next
            diamond_ListOfDiamondSecurityQuestions = securityQuestions
            If setMustChangePassword = True Then
                diamond_MustChangePassword = mustChangePasswordVal
            Else
                diamond_MustChangePassword = checkPWflag(.UsersId)
            End If
            diamond_Active = .Active
            getUserTypeAndDept()
        End With
        SystemsLocated = Enums.UserLocation.Diamond

        diamond_UserAgencyLink_CaughtError = userAgencyLink_caughtError 'added 3/21/2022
    End Sub

    ''' <summary>
    ''' Set database connections based on config file settings if not already manually set by user
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckDatabaseConnections()
        If legacy_Connection = "" And Not ConfigurationManager.AppSettings("conn") Is Nothing Then
            legacy_Connection = ConfigurationManager.AppSettings("conn")
        End If
        If quotemaster_Connection = "" And Not ConfigurationManager.AppSettings("connQM") Is Nothing Then
            quotemaster_Connection = ConfigurationManager.AppSettings("connQM")
        End If
        If diamond_Connection = "" And Not ConfigurationManager.AppSettings("connDiamond") Is Nothing Then
            diamond_Connection = ConfigurationManager.AppSettings("connDiamond")
        End If
    End Sub

    Private Sub getUserTypeAndDept()
        CheckDatabaseConnections()
        Using Sql As New SQLselectObject(legacy_Connection)
            Sql.queryOrStoredProc = "usp_GetUserTypeAndDept"
            Sql.parameter = New SqlParameter("@userID", diamond_UserID)
            Using dr As SqlDataReader = Sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    dr.Read()
                    diamond_UserType = dr.Item("UserType").ToString.Trim
                    diamond_Department = dr.Item("dept").ToString.Trim
                End If
            End Using
        End Using
    End Sub

    Public Function getPrimaryAgencyID(ByVal agencyCode As Integer) As Integer
        CheckDatabaseConnections()
        Dim primID As Integer = 0
        Using Sql As New SQLselectObject(diamond_Connection)
            Sql.queryOrStoredProc = "Select agency_id from Agency where code like '%" & agencyCode & "%'"
            Using dr As SqlDataReader = Sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    dr.Read()
                    primID = Integer.Parse(dr.Item("agency_id"))
                End If
            End Using
        End Using
        Return primID
    End Function

    Public Function getDiamondAgencyCode(ByVal agencyID As Integer) As Integer
        CheckDatabaseConnections()
        Using Sql As New SQLselectObject(diamond_Connection)
            Sql.queryOrStoredProc = "Select code from Agency where agency_id = '" & agencyID & "'"
            Using dr As SqlDataReader = Sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    dr.Read()

                    If dr.Item("code").ToString.Trim.Contains("-") Then
                        Dim agCode As Array = Split(dr.Item("code").ToString.Trim)
                        If agCode.Length > 0 Then Return agCode(1) Else Return 0
                    Else
                        Return dr.Item("code").ToString.Trim
                    End If
                End If
            End Using
        End Using
    End Function

    Public Function getLegacyAgencyID(ByVal userObj As WebUser) As Integer
        CheckDatabaseConnections()
        Dim agCode As String = String.Empty
        If userObj.diamond_PrimaryAgencyCode > 0 Then
            agCode = userObj.diamond_PrimaryAgencyCode
        ElseIf userObj.diamond_PrimaryAgencyID > 0 Then
            agCode = getDiamondAgencyCode(userObj.diamond_PrimaryAgencyID)
        End If

        If agCode > 0 Then
            CheckDatabaseConnections()
            Using Sql As New SQLselectObject(legacy_Connection)
                Sql.queryOrStoredProc = "sp_GetAgencyInfo"
                Sql.parameter = New SqlParameter("@agCode", agCode)
                Using dr As SqlDataReader = Sql.GetDataReader
                    If dr IsNot Nothing AndAlso dr.HasRows Then
                        dr.Read()
                        Return dr.Item("agency_id")
                    End If
                End Using
            End Using
        Else
            Return 0
        End If
    End Function

    Public Function getSecondaryAgencyIDs(ByVal secondaryCodes As List(Of Integer)) As List(Of Integer)
        Dim secCodes As New List(Of Integer)
        If secondaryCodes IsNot Nothing AndAlso secondaryCodes.Count > 0 Then
            For Each secCode As Integer In secondaryCodes
                Using Sql As New SQLselectObject(diamond_Connection)
                    Sql.queryOrStoredProc = "Select agency_id from Agency where code like '%" & secCode & "%'"
                    Using dr As SqlDataReader = Sql.GetDataReader
                        If dr IsNot Nothing AndAlso dr.HasRows Then
                            dr.Read()
                            secCodes.Add(Integer.Parse(dr.Item("agency_id")))
                        End If
                    End Using
                End Using
            Next
        End If
        Return secCodes
    End Function

#Region "Props"

    Public Property legacy_Connection() As String
        Get
            Return _legacy_Connection
        End Get
        Set(ByVal value As String)
            _legacy_Connection = value
        End Set
    End Property

    Public Property diamond_Connection() As String
        Get
            Return _diamond_Connection
        End Get
        Set(ByVal value As String)
            _diamond_Connection = value
        End Set
    End Property

    Public Property quotemaster_Connection() As String
        Get
            Return _quotemaster_Connection
        End Get
        Set(ByVal value As String)
            _quotemaster_Connection = value
        End Set
    End Property


    Public Property diamond_UserID() As Integer
        Get
            Return _diamond_UserID
        End Get
        Set(ByVal value As Integer)
            _diamond_UserID = value
        End Set
    End Property

    Public Property diamond_LoginDomain() As String
        Get
            Return _diamond_LoginDomain
        End Get
        Set(ByVal value As String)
            _diamond_LoginDomain = value
        End Set
    End Property

    Public Property diamond_LoginName() As String
        Get
            Return _diamond_LoginName
        End Get
        Set(ByVal value As String)
            _diamond_LoginName = value
        End Set
    End Property

    Public Property diamond_Password() As String
        Get
            Return _diamond_Password
        End Get
        Set(ByVal value As String)
            _diamond_Password = value
        End Set
    End Property

    Public Property diamond_SUId() As String
        Get
            Return _diamond_SUId
        End Get
        Set(ByVal value As String)
            _diamond_SUId = value
        End Set
    End Property

    Public Property diamond_UserCode() As String
        Get
            Return _diamond_UserCode
        End Get
        Set(ByVal value As String)
            _diamond_UserCode = value
        End Set
    End Property

    Public Property diamond_UserEmailAddress() As String
        Get
            Return _diamond_UserEmailAddress
        End Get
        Set(ByVal value As String)
            _diamond_UserEmailAddress = value
        End Set
    End Property

    Public Property diamond_IsUnderwriter() As Boolean
        Get
            Return _diamond_IsUnderwriter
        End Get
        Set(ByVal value As Boolean)
            _diamond_IsUnderwriter = value
        End Set
    End Property

    Public Property diamond_IsSupervisor() As Boolean
        Get
            Return _diamond_IsSupervisor
        End Get
        Set(ByVal value As Boolean)
            _diamond_IsSupervisor = value
        End Set
    End Property

    Public Property diamond_UserCategoryID() As Enums.UserCategoryID
        Get
            Return _diamond_UserCategoryID
        End Get
        Set(ByVal value As Enums.UserCategoryID)
            _diamond_UserCategoryID = value
        End Set
    End Property

    Public Property diamond_ListOfDiamondSecurityQuestions() As List(Of DiamondSecurityQuestion)
        Get
            Return _diamond_ListOfDiamondSecurityQuestions
        End Get
        Set(ByVal value As List(Of DiamondSecurityQuestion))
            _diamond_ListOfDiamondSecurityQuestions = value
        End Set
    End Property

    Public Property diamond_PrimaryAgencyID() As Integer
        Get
            Return _diamond_PrimaryAgencyID
        End Get
        Set(ByVal value As Integer)
            _diamond_PrimaryAgencyID = value
        End Set
    End Property

    Public Property diamond_SecondaryAgencyIDs() As List(Of Integer)
        Get
            Return _diamond_SecondaryAgencyIDs
        End Get
        Set(ByVal value As List(Of Integer))
            _diamond_SecondaryAgencyIDs = value
        End Set
    End Property

    Public Property diamond_PrimaryAgencyCode() As Integer
        Get
            Return _diamond_PrimaryAgencyCode
        End Get
        Set(ByVal value As Integer)
            _diamond_PrimaryAgencyCode = value
            If value > 0 Then diamond_PrimaryAgencyID = getPrimaryAgencyID(value)
        End Set
    End Property

    Public Property diamond_SecondaryAgencyCodes() As List(Of Integer)
        Get
            Return _diamond_SecondaryAgencyCodes
        End Get
        Set(ByVal value As List(Of Integer))
            _diamond_SecondaryAgencyCodes = value
            'If diamond_SecondaryAgencyIDs Is Nothing Then diamond_SecondaryAgencyIDs = New List(Of Integer)
            If value.Count > 0 Then diamond_SecondaryAgencyIDs = getSecondaryAgencyIDs(value)
        End Set
    End Property

    Public Property diamond_IsAgencyAdmin() As Boolean
        Get
            Return _diamond_IsAgencyAdmin
        End Get
        Set(ByVal value As Boolean)
            _diamond_IsAgencyAdmin = value
        End Set
    End Property

    Public Property diamond_Territory() As Integer
        Get
            Return _diamond_Territory
        End Get
        Set(ByVal value As Integer)
            _diamond_Territory = value
        End Set
    End Property

    Public Property diamond_MustChangePassword() As Boolean
        Get
            Return _diamond_MustChangePassword
        End Get
        Set(ByVal value As Boolean)
            _diamond_MustChangePassword = value
        End Set
    End Property

    Public Property diamond_UserType() As String
        Get
            Return _diamond_UserType
        End Get
        Set(ByVal value As String)
            _diamond_UserType = value
        End Set
    End Property

    Public Property diamond_Department() As String
        Get
            Return _diamond_Department
        End Get
        Set(ByVal value As String)
            _diamond_Department = value
        End Set
    End Property

    Public Property diamond_Active() As Boolean
        Get
            Return _diamond_Active
        End Get
        Set(ByVal value As Boolean)
            _diamond_Active = value
        End Set
    End Property


    Public Property legacy_UserID() As Integer
        Get
            Return _legacy_UserID
        End Get
        Set(ByVal value As Integer)
            _legacy_UserID = value
        End Set
    End Property

    Public Property legacy_Username() As String
        Get
            Return _legacy_Username
        End Get
        Set(ByVal value As String)
            _legacy_Username = value
        End Set
    End Property

    Public Property legacy_OldUsername() As String
        Get
            Return _legacy_OldUsername
        End Get
        Set(ByVal value As String)
            _legacy_OldUsername = value
        End Set
    End Property


    Public Property legacy_Password() As String
        Get
            Return _legacy_Password
        End Get
        Set(ByVal value As String)
            _legacy_Password = value
        End Set
    End Property

    Public Property legacy_Hint() As String
        Get
            Return _legacy_Hint
        End Get
        Set(ByVal value As String)
            _legacy_Hint = value
        End Set
    End Property

    Public Property legacy_AgencyID() As Integer
        Get
            Return _legacy_AgencyID
        End Get
        Set(ByVal value As Integer)
            _legacy_AgencyID = value
        End Set
    End Property

    Public Property legacy_AuthType() As String
        Get
            Return _legacy_AuthType
        End Get
        Set(ByVal value As String)
            _legacy_AuthType = value
        End Set
    End Property

    Public Property legacy_FirstName() As String
        Get
            Return _legacy_FirstName
        End Get
        Set(ByVal value As String)
            _legacy_FirstName = value
        End Set
    End Property

    Public Property legacy_MiddleName() As String
        Get
            Return _legacy_MiddleName
        End Get
        Set(ByVal value As String)
            _legacy_MiddleName = value
        End Set
    End Property

    Public Property legacy_LastName() As String
        Get
            Return _legacy_LastName
        End Get
        Set(ByVal value As String)
            _legacy_LastName = value
        End Set
    End Property

    Public ReadOnly Property legacy_NameArray() As String()
        Get
            Dim arName() As String = {legacy_FirstName, legacy_MiddleName, legacy_LastName}
            Return arName
        End Get
    End Property

    Public Property legacy_Active() As Boolean
        Get
            Return _legacy_Active
        End Get
        Set(ByVal value As Boolean)
            _legacy_Active = value
        End Set
    End Property


    Public Property legacy_6000Access() As Boolean
        Get
            Return _legacy_6000Access
        End Get
        Set(ByVal value As Boolean)
            _legacy_6000Access = value
        End Set
    End Property

    Public Property legacy_Role() As String
        Get
            Return _legacy_Role
        End Get
        Set(ByVal value As String)
            _legacy_Role = value
        End Set
    End Property

    Public Property legacy_Territory() As Integer
        Get
            Return _legacy_Territory
        End Get
        Set(ByVal value As Integer)
            _legacy_Territory = value
        End Set
    End Property

    Public Property legacy_PrimaryAgencyCode() As Integer
        Get
            Return _legacy_PrimaryAgencyCode
        End Get
        Set(ByVal value As Integer)
            _legacy_PrimaryAgencyCode = value
        End Set
    End Property

    Public Property legacy_SecondaryAgencyCodes() As List(Of Integer)
        Get
            Return _legacy_SecondaryAgencyCodes
        End Get
        Set(ByVal value As List(Of Integer))
            _legacy_SecondaryAgencyCodes = value
        End Set
    End Property

    Public Property legacy_ConvertedToDiamond() As Boolean
        Get
            Return _legacy_ConvertedToDiamond
        End Get
        Set(ByVal value As Boolean)
            _legacy_ConvertedToDiamond = value
        End Set
    End Property

    Public Property legacy_EmailAddress() As String
        Get
            Return _legacy_EmailAddress
        End Get
        Set(ByVal value As String)
            _legacy_EmailAddress = value
        End Set
    End Property

    Public Property legacy_CompletedSurvey() As Boolean
        Get
            Return _legacy_CompletedSurvey
        End Get
        Set(ByVal value As Boolean)
            _legacy_CompletedSurvey = value
        End Set
    End Property

    Public Property legacy_AgencyName() As String
        Get
            Return _legacy_AgencyName
        End Get
        Set(ByVal value As String)
            _legacy_AgencyName = value
        End Set
    End Property


    Public Property QM_existingProducerID() As String
        Get
            Return _QM_existingProducerID
        End Get
        Set(ByVal value As String)
            _QM_existingProducerID = value
        End Set
    End Property

    Public Property QM_st() As String
        Get
            Return _QM_st
        End Get
        Set(ByVal value As String)
            _QM_st = value
        End Set
    End Property

    Public Property QM_cp() As String
        Get
            Return _QM_cp
        End Get
        Set(ByVal value As String)
            _QM_cp = value
        End Set
    End Property

    Public Property QM_autoCredit() As String
        Get
            Return _QM_autoCredit
        End Get
        Set(ByVal value As String)
            _QM_autoCredit = value
        End Set
    End Property

    Public Property QM_propCredit() As String
        Get
            Return _QM_propCredit
        End Get
        Set(ByVal value As String)
            _QM_propCredit = value
        End Set
    End Property

    Public Property QM_mvr() As String
        Get
            Return _QM_mvr
        End Get
        Set(ByVal value As String)
            _QM_mvr = value
        End Set
    End Property

    Public Property QM_autoClue() As String
        Get
            Return _QM_autoClue
        End Get
        Set(ByVal value As String)
            _QM_autoClue = value
        End Set
    End Property

    Public Property QM_propClue() As String
        Get
            Return _QM_propClue
        End Get
        Set(ByVal value As String)
            _QM_propClue = value
        End Set
    End Property

    Public Property QM_agencyID() As Integer
        Get
            Return _QM_agencyID
        End Get
        Set(ByVal value As Integer)
            _QM_agencyID = value
        End Set
    End Property


    Public Property SystemsLocated() As Enums.UserLocation
        Get
            Return _SystemsLocated
        End Get
        Set(ByVal value As Enums.UserLocation)
            _SystemsLocated = value
        End Set
    End Property

    Public Property diamond_UserAgencyLink_CaughtError As Boolean 'added 3/21/2022
        Get
            Return _diamond_UserAgencyLink_CaughtError
        End Get
        Set(value As Boolean)
            _diamond_UserAgencyLink_CaughtError = value
        End Set
    End Property

#End Region
End Class
