Public Class DiamondSecurity
    Implements IDisposable

#Region "Var"
    Private _legacy_Connection As String
    Private _diamond_Connection As String
    Private _quotemaster_Connection As String

    Private _CurrentSecurityQuestion As DCO.Administration.UserSecurityQuestion
    Private _CurrentUserSecurityQuestionAnswer As DCO.Administration.UsersUserSecurityQuestionLink
    Private _CurrentUserSecurityQuestionAnswerForLogin As DCO.Administration.UsersUserSecurityQuestionLinkForLoginName
    Private _ListOfAllSecurityQuestions As DCO.InsCollection(Of DCO.Administration.UserSecurityQuestion)
    Private _ListOfUserSecurityQuestionsAnswers As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLink)
    Private _ListOfUserSecurityQuestionsAnswersForLogin As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLinkForLoginName)

    Private _ExistingUser As WebUser
    Private _UserAgenciesHT As Hashtable  'Agency ID/Agency Relationship Type (1 = Primary, 2 = Secondary)
    Private _UserAgenciesDT As DataTable

    Private _ListOfDiamondSecurityQuestions As List(Of DiamondSecurityQuestion)

    Private _DiamondUserID As Integer
    Private _DiamondUserLogin As String

    Private _LegacyUserID As Integer
    Private _LegacyUserLogin As String
    Private _LegacyUserPassword As String

    Private _Errors As New List(Of ErrorObject)

    Private _convertResult = Nothing

    Private _UseTestToken As Boolean = False
    Private _TestTokenUserName As String = ""
    Private _TestTokenPassword As String = ""
#End Region

    ''' <summary>
    ''' Blank instantiation of DiamondSecurity.  Properties need to be set manually.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Errors.Clear()
        Try
            If System.Web.HttpContext.Current.Session("username") IsNot Nothing Then DiamondUserLogin = System.Web.HttpContext.Current.Session("username")
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error establishing username", Enums.ErrorLevel.Minor, Enums.UserLocation.AllSystems))
        End Try
    End Sub

    ''' <summary>Single system instantiation taking in only User ID and System enumeration</summary>
    ''' <param name="UserID">ID value for this user in the system to be checked</param>
    ''' <param name="systemsToCheck">Enumeration of the system to check</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal UserID As Integer, ByVal systemsToCheck As Enums.UserLocation)
        Errors.Clear()
        Try
            If System.Web.HttpContext.Current.Session("username") IsNot Nothing Then DiamondUserLogin = System.Web.HttpContext.Current.Session("username")
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error establishing username", Enums.ErrorLevel.Minor, Enums.UserLocation.AllSystems))
        End Try
        Select Case systemsToCheck
            Case Is = Enums.UserLocation.Diamond
                DiamondUserID = UserID
                Lookup_GetExistingUserInfo(systemsToCheck)
            Case Is = Enums.UserLocation.Legacy
                LegacyUserID = UserID
                Lookup_GetExistingUserInfo(systemsToCheck)
            Case Is = Enums.UserLocation.Quotemaster
                LegacyUserID = UserID
                Lookup_GetExistingUserInfo(Enums.UserLocation.Legacy)
                CheckQuoteMasterAgency(ExistingUser)
            Case Else
                Errors.Add(New ErrorObject("This instantiation is only for a single system population.  To instantiate with multiple systems, use a multi-system instantiation (will have multiple user variables for each system).", _
                                           "Error encountered during user information lookup.", Enums.ErrorLevel.Programmer, systemsToCheck))
        End Select
    End Sub

    ''' <summary>Single system instantiation taking in only User Login and System enumeration</summary>
    ''' <param name="UserLogin">Login value for this user in the system to be checked</param>
    ''' <param name="systemsToCheck">Enumeration of the system to check</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal UserLogin As String, ByVal systemsToCheck As Enums.UserLocation)
        Errors.Clear()
        Select Case systemsToCheck
            Case Is = Enums.UserLocation.Diamond
                DiamondUserLogin = UserLogin
                Lookup_GetExistingUserInfo(systemsToCheck)
            Case Is = Enums.UserLocation.Legacy
                LegacyUserLogin = UserLogin
                Lookup_GetExistingUserInfo(systemsToCheck)
            Case Is = Enums.UserLocation.Quotemaster
                LegacyUserLogin = UserLogin
                Lookup_GetExistingUserInfo(Enums.UserLocation.Legacy)
                CheckQuoteMasterAgency(ExistingUser)
            Case Else
                Errors.Add(New ErrorObject("This instantiation is only for a single system population.  To instantiate with multiple systems, use a multi-system instantiation (will have multiple user variables for each system).", _
                                                           "Error encountered during user information lookup.", Enums.ErrorLevel.Programmer, systemsToCheck))
        End Select
    End Sub

    ''' <summary> Single system instantiation taking in User ID and User Login as well as System enumeration </summary>
    ''' <param name="UserID">ID value for this user in the system to be checked</param>
    ''' <param name="UserLogin">Login value for this user in the system to be checked</param>
    ''' <param name="systemsToCheck">Enumeration of the system to check</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal UserID As Integer, ByVal UserLogin As String, ByVal systemsToCheck As Enums.UserLocation)
        Errors.Clear()
        Select Case systemsToCheck
            Case Is = Enums.UserLocation.Diamond
                DiamondUserID = UserID
                DiamondUserLogin = UserLogin
                Lookup_GetExistingUserInfo(systemsToCheck)
            Case Is = Enums.UserLocation.Legacy
                LegacyUserID = UserID
                LegacyUserLogin = UserLogin
                Lookup_GetExistingUserInfo(systemsToCheck)
            Case Is = Enums.UserLocation.Quotemaster

            Case Else
                Errors.Add(New ErrorObject("This instantiation is only for a single system population.  To instantiate with multiple systems, use a multi-system instantiation (will have multiple user variables for each system).", _
                                           "Error encountered during user information lookup.", Enums.ErrorLevel.Programmer, systemsToCheck))
        End Select
    End Sub

    ''' <summary>
    ''' Diamond specific instantiation taking in username and password.  This instantiation will automatically log the user in to the Diamond system and cache Diamond values.
    ''' </summary>
    ''' <param name="loginName">Diamond user name</param>
    ''' <param name="loginPass">Diamond user password</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal loginName As String, ByVal loginPass As String)
        DiamondUserLogin = loginName
        LoginDiamond(loginName, loginPass)
    End Sub

    ''' <summary>
    ''' Diamond specific instantiation taking in a Diamond Security Token
    ''' </summary>
    ''' <param name="diaToken">Diamond Security Token received after a successful log in to the Diamond system</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal diaToken As Diamond.Common.Services.DiamondSecurityToken)
        Errors.Clear()
        Try
            If System.Web.HttpContext.Current.Session("username") IsNot Nothing Then DiamondUserLogin = System.Web.HttpContext.Current.Session("username")
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error establishing username", Enums.ErrorLevel.Minor, Enums.UserLocation.AllSystems))
        End Try

        If diaToken IsNot Nothing Then
            DiamondUserID = diaToken.DiamUserId
        End If
    End Sub

    ''' <summary>
    ''' Multi system instantiation taking in Legacy and Diamond User ID's as well as system enumeration.
    ''' </summary>
    ''' <param name="LegUserID">ID value for this user in the legacy system.</param>
    ''' <param name="DiaUserID">ID value for this user in the Diamond system.</param>
    ''' <param name="systemsToCheck">Enumeration of the system(s) to check</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal LegUserID As Integer, ByVal DiaUserID As Integer, ByVal systemsToCheck As Enums.UserLocation)
        Errors.Clear()
        Try
            If System.Web.HttpContext.Current.Session("username") IsNot Nothing Then DiamondUserLogin = System.Web.HttpContext.Current.Session("username")
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error establishing username", Enums.ErrorLevel.Minor, Enums.UserLocation.AllSystems))
        End Try

        LegacyUserID = LegUserID
        DiamondUserID = DiaUserID
        Lookup_GetExistingUserInfo(systemsToCheck)
    End Sub

    ''' <summary>
    ''' Multi system instantiation taking in Legacy and Diamond User Logins as well as system enumeration
    ''' </summary>
    ''' <param name="LegUserLogin">Login value for this user in the legacy system.</param>
    ''' <param name="DiaUserLogin">Login value for this user in the Diamond system.</param>
    ''' <param name="systemsToCheck">Enumeration of the system(s) to check.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal LegUserLogin As String, ByVal DiaUserLogin As String, ByVal systemsToCheck As Enums.UserLocation)
        Errors.Clear()
        LegacyUserLogin = LegUserLogin
        DiamondUserLogin = DiaUserLogin
        Lookup_GetExistingUserInfo(systemsToCheck)
    End Sub

    ''' <summary>
    ''' Log in to the Diamond system.  This is required for certain web service calls.
    ''' </summary>
    ''' <param name="loginName">Diamond user name</param>
    ''' <param name="loginPass">Diamond user password</param>
    ''' <remarks></remarks>
    Public Sub LoginDiamond(ByVal loginName As String, ByVal loginPass As String)
        Dim errText As String = String.Empty
        'Diamond.Web.BaseControls.LoginDiamondUser(loginName, loginPass, errText)
        'updated 9/26/2019 for 534; reverted back since using new call results in the following error: Unable to cast object of type 'ASP.alogin_aspx' to type 'Diamond.Web.BaseControls.InsPage'.
        'at Diamond.Web.BaseControls.Common.get_CurrentPage() in C:\TFS2010\Base\Code\Versions\534\534.004\Diamond\ASPNET\BaseControls\Common.vb:line 82 at Diamond.Web.Security.LoginUtility.ContinueLogin(Boolean cacheStaticData) in C:\TFS2010\Base\Code\Versions\534\534.004\Diamond\ASPNET\Security\LoginControls\LoginUtility.vb:line 342 at Diamond.Web.Security.LoginUtility.ProcessLoginResult(LoginResult& result, Boolean cacheStaticData, DiamondValidation diamondValidation) in C:\TFS2010\Base\Code\Versions\534\534.004\Diamond\ASPNET\Security\LoginControls\LoginUtility.vb:line 316 at Diamond.Web.Security.LoginUtility.Login(String loginName, String password, Boolean rememberMe, Int32 rememberMeExpirationDays, Boolean cacheStaticData) in C:\TFS2010\Base\Code\Versions\534\534.004\Diamond\ASPNET\Security\LoginControls\LoginUtility.vb:line 205 at Diamond.Web.Security.LoginUtility.Login(String loginName, String password, Boolean rememberMe) in C:\TFS2010\Base\Code\Versions\534\534.004\Diamond\ASPNET\Security\LoginControls\LoginUtility.vb:line 180 at Diamond.Web.Security.LoginUtility.Login(String loginName, String password) in C:\TFS2010\Base\Code\Versions\534\534.004\Diamond\ASPNET\Security\LoginControls\LoginUtility.vb:line 176 at DiamondWebClass.DiamondSecurity.LoginDiamond(String loginName, String loginPass) at ALogin05.btnLogin_Click(Object sender, EventArgs e) at System.Web.UI.WebControls.Button.OnClick(EventArgs e) at System.Web.UI.WebControls.Button.RaisePostBackEvent(String eventArgument) at System.Web.UI.Page.ProcessRequestMain(Boolean includeStagesBeforeAsyncPoint, Boolean includeStagesAfterAsyncPoint)
        'Dim result As Diamond.Web.Security.LoginUtility.LoginResult = Diamond.Web.Security.LoginUtility.Login(loginName, loginPass)
        'If Not result.Equals(Nothing) Then
        '    errText = result.Message
        'End If
        'updated 10/8/2019
        'Dim res As New DCS.Messages.LoginService.GetDiamTokenForUsernamePassword.Response
        Dim result As Diamond.Web.Security.LoginUtility.LoginResult = DiamondLoginFunctionality(loginName, loginPass)
        If Not result.Equals(Nothing) Then
            errText = result.Message

            'added 1/24/2022
            If Diamond.Web.BaseControls.SignedOnUserID > 0 OrElse result.PasswordMustBeChanged = True Then
                If ExistingUser Is Nothing Then
                    ExistingUser = New WebUser()
                End If
                ExistingUser.diamond_LoginName = loginName
                ExistingUser.diamond_MustChangePassword = result.PasswordMustBeChanged
            End If
        End If

        'note: don't need the additional cookies stuff at the end since we handle our own RememberMe functionality

        If Diamond.Web.BaseControls.SignedOnUserID <= 0 Then Errors.Add(New ErrorObject(errText, "An error was encountered while logging in to the Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
        'If Diamond.Web.BaseControls.SignedOnUser.UsersId <= 0 Then Errors.Add(New ErrorObject(errText, "An error was encountered while logging in to the Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
    End Sub
    'added 10/8/2019 for 534
    Private Function DiamondLoginFunctionality(ByVal loginName As String, ByVal password As String, Optional ByVal rememberMe As Boolean = False,
                Optional ByVal rememberMeExpirationDays As Int32 = 0, Optional ByVal cacheStaticData As Boolean = True) As Diamond.Web.Security.LoginUtility.LoginResult
        Dim result As New Diamond.Web.Security.LoginUtility.LoginResult()

        Dim loginResponse As DCS.Messages.LoginService.GetDiamTokenForUsernamePassword.Response = Nothing
        Try
            loginResponse = Diamond.Web.BaseControls.Common.UpdateSignedOnUser(loginName, password) 'note: calls Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword; also appears to set ProxyBase.DiamondSecurityToken
        Catch ex As Exception
            ' Find a meaningful exception in the login exception object if possible
            result.Message = String.Format("Login Failed: {0}", DiamondGetOriginalExceptionMessage(ex))
        End Try

        If loginResponse IsNot Nothing Then
            If loginResponse.ResponseData IsNot Nothing Then
                result.SecurityToken = loginResponse.ResponseData.DiamondSecurityToken
                result.PasswordMustBeChanged = loginResponse.ResponseData.PasswordMustBeChanged
                'note: these properties can only be used when referencing 534 assemblies
                'result.PasswordHasExpired = loginResponse.ResponseData.PasswordHasExpired
                'result.PasswordWarning = loginResponse.ResponseData.PasswordWarning
                'result.PasswordExpiresDate = loginResponse.ResponseData.PasswordExpiresDate
            End If

            'DiamondProcessLoginResult(result, cacheStaticData, loginResponse.DiamondValidation)
            'note: not going to do all of the Diamond functionality
            DCS.Proxies.ProxyBase.DiamondSecurityToken = result.SecurityToken

            If loginResponse.DiamondValidation IsNot Nothing AndAlso loginResponse.DiamondValidation.HasAnyItems() Then
                Dim text As New System.Text.StringBuilder("Login Failed: ")
                If loginResponse.DiamondValidation.ValidationItems IsNot Nothing AndAlso loginResponse.DiamondValidation.ValidationItems.Count > 0 Then
                    Dim count As Int32 = (loginResponse.DiamondValidation.ValidationItems.Count - 1)
                    For index As Int32 = 0 To count
                        text.Append(loginResponse.DiamondValidation.ValidationItems(index).Message)
                        If index < count Then text.Append(" ")
                    Next
                End If
                result.Message = text.ToString()
            End If

            'note: may also need to manually call DiamondSaveUserLogin; would be called from DiamondProcessLoginResult via DiamondContinueLogin, but we're not doing all of that... can't see SaveUserLogin in existing 531 Diamond code, but it's somehow being logged in Users table - still correctly updates login timestamps in Users table w/o call
        End If

        'note: not going to do all of the Diamond functionality
        'If result.Successful Then
        '    If rememberMe Then
        '        If rememberMeExpirationDays <= 0 Then
        '            rememberMeExpirationDays = 30
        '        End If
        '        DiamondSetRememberMeCookie(loginName, password, rememberMeExpirationDays)
        '    Else
        '        DiamondResetRememberMeCookie()
        '    End If
        'End If

        Return result
    End Function
    Private Sub DiamondProcessLoginResult(ByRef result As Diamond.Web.Security.LoginUtility.LoginResult,
                            cacheStaticData As Boolean,
                            diamondValidation As DCO.DiamondValidation)

        'If Not result.PasswordMustBeChanged _
        '        AndAlso Not result.PasswordHasExpired _
        '        AndAlso Not result.PasswordWarning Then
        'note: PasswordHasExpired and PasswordWarning Properties can only be used when referencing 534 assemblies
        If Not result.PasswordMustBeChanged Then
            DCS.Proxies.ProxyBase.DiamondSecurityToken = result.SecurityToken
            result.Successful = (DCS.Proxies.ProxyBase.DiamondSecurityToken IsNot Nothing)

            If result.Successful Then
                DiamondContinueLogin(cacheStaticData)
            Else
                'note: this method can only be used when referencing 534 assemblies
                'Diamond.Web.BaseControls.CookieUtility.DeleteLoggedInCookie()
            End If
        End If

        If diamondValidation IsNot Nothing AndAlso diamondValidation.HasAnyItems() Then
            Dim text As New System.Text.StringBuilder("Login Failed: ")
            If diamondValidation IsNot Nothing AndAlso diamondValidation.ValidationItems IsNot Nothing AndAlso diamondValidation.ValidationItems.Count > 0 Then
                'Dim count As Int32 = (diamondValidation.ValidationItems.SafeCount() - 1)
                Dim count As Int32 = (diamondValidation.ValidationItems.Count - 1)
                For index As Int32 = 0 To count
                    text.Append(diamondValidation.ValidationItems(index).Message)
                    If index < count Then text.Append(" ")
                Next
            End If
            result.Message = text.ToString()
        End If
    End Sub
    Private Sub DiamondContinueLogin(cacheStaticData As Boolean)

        ' Generate a new session ID after a successful login
        DiamondRegenerateSessionId()
        'note: this method can only be used when referencing 534 assemblies
        'Diamond.Web.BaseControls.CookieUtility.SetLoggedInCookie()

        DiamondSaveBrowserInformation()
        DiamondSaveUserLogin()

        If Diamond.Web.BaseControls.Common.CurrentPage.Session.IsCookieless Then
            ' If we are using cookieless session, we need to store the user in a cookie so we can validate diamondsecuritytoken usersid in session with usersid in the cookie
            'Diamond.Web.BaseControls.CookieUtility.Add(String.Format("{0}_DSTU", Diamond.Web.BaseControls.Common.CurrentPage.Session.SessionID),
            '                  DCS.Proxies.ProxyBase.DiamondSecurityToken.DiamUserId,
            '                  DateTime.Now.AddDays(1),
            '                  DCE.Security.CookieEncryptionType.Standard)
            'note: DCE.Security.CookieEncryptionType can only be used when referencing 534 assemblies; 531 uses Diamond.Web.BaseControls.CookieUtility.EncryptionType
            'Diamond.Web.BaseControls.CookieUtility.Add(String.Format("{0}_DSTU", Diamond.Web.BaseControls.Common.CurrentPage.Session.SessionID),
            '                  DCS.Proxies.ProxyBase.DiamondSecurityToken.DiamUserId,
            '                  DateTime.Now.AddDays(1),
            '                  Diamond.Web.BaseControls.CookieUtility.EncryptionType.Standard)
        End If

        If cacheStaticData Then
            Diamond.Web.BaseControls.ConfigCommon.CacheStaticData()
        End If

    End Sub
    Private Sub DiamondRegenerateSessionId()
        If Diamond.Web.BaseControls.Common.CurrentSession.IsCookieless Then
            ' The code in this method doesn't work for cookieless sessions
            Return
        End If

        Dim manager As New Web.SessionState.SessionIDManager
        Dim context As Web.HttpContext = Web.HttpContext.Current
        Dim originalSessionId As String = manager.GetSessionID(context)
        Dim newSessionId As String = manager.CreateSessionID(context)

        ' Set the new session ID
        Dim redirected As Boolean = False, cookieAdded As Boolean = False
        manager.SaveSessionID(context, newSessionId, redirected, cookieAdded)

        ' Have to also set the new session ID in the internal objects and remove the old session object
        Dim state As Web.SessionState.SessionStateModule = DirectCast(context.ApplicationInstance.Modules.Get("Session"), Web.SessionState.SessionStateModule)
        Dim store As Web.SessionState.SessionStateStoreProviderBase = DirectCast(DiamondGetSessionFieldValue(state, "_store"), Web.SessionState.SessionStateStoreProviderBase)
        Dim rqIdField As System.Reflection.FieldInfo = DiamondGetSessionField(state, "_rqId")
        Dim rqItem As Web.SessionState.SessionStateStoreData = DirectCast(DiamondGetSessionFieldValue(state, "_rqItem"), Web.SessionState.SessionStateStoreData)
        Dim rqLockIdField As System.Reflection.FieldInfo = DiamondGetSessionField(state, "_rqLockId")
        Dim rqSessionStateNotFoundField As System.Reflection.FieldInfo = DiamondGetSessionField(state, "_rqSessionStateNotFound")

        If store IsNot Nothing Then
            Dim lockId As Object = Nothing
            If rqLockIdField IsNot Nothing Then
                lockId = rqLockIdField.GetValue(state)
                If lockId IsNot Nothing AndAlso Not String.IsNullOrEmpty(originalSessionId) Then
                    store.ReleaseItemExclusive(context, originalSessionId, lockId)
                    store.RemoveItem(context, originalSessionId, lockId, rqItem)
                End If
            End If

            If rqSessionStateNotFoundField IsNot Nothing Then
                rqSessionStateNotFoundField.SetValue(state, True)
            End If
            If rqIdField IsNot Nothing Then
                rqIdField.SetValue(state, newSessionId)
            End If
        End If
    End Sub
    Private Function DiamondGetSessionField(ByVal state As Web.SessionState.SessionStateModule, ByVal fieldName As String) As System.Reflection.FieldInfo
        Dim field As System.Reflection.FieldInfo = Nothing
        If state IsNot Nothing Then
            field = state.GetType.GetField(fieldName, System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        End If
        Return field
    End Function

    Private Function DiamondGetSessionFieldValue(ByVal state As Web.SessionState.SessionStateModule, ByVal fieldName As String) As Object
        Dim value As Object = Nothing
        Dim field As System.Reflection.FieldInfo = DiamondGetSessionField(state, fieldName)
        If field IsNot Nothing Then
            value = field.GetValue(state)
        End If
        Return value
    End Function

    Private Sub DiamondResetRememberMeCookie()

        ' We will get the browser to delete the cookie for us.  We will do this by creating a new cookie with the same name
        ' but with an expiration date in the past.
        'Diamond.Web.BaseControls.CookieUtility.Add(DiamondGetRememberMeCookieKey, String.Empty,
        '        DateTime.Now.AddDays(-1), DCE.Security.CookieEncryptionType.TripleDES)
        'note: DCE.Security.CookieEncryptionType can only be used when referencing 534 assemblies; 531 uses Diamond.Web.BaseControls.CookieUtility.EncryptionType
        'Diamond.Web.BaseControls.CookieUtility.Add(DiamondGetRememberMeCookieKey, String.Empty,
        '        DateTime.Now.AddDays(-1), Diamond.Web.BaseControls.CookieUtility.EncryptionType.TripleDES)

    End Sub
    Private Sub DiamondSaveBrowserInformation()

        Dim currentRequest As Web.HttpRequest = Web.HttpContext.Current.Request

        Dim saveBrowserInfoRequest As New DCS.Messages.WebSiteService.SaveBrowserInfo.Request()
        saveBrowserInfoRequest.RequestData.BrowserName = currentRequest.Browser.Browser
        saveBrowserInfoRequest.RequestData.BrowserVersion = currentRequest.Browser.Version
        saveBrowserInfoRequest.RequestData.ClientPlatform = currentRequest.Browser.Platform
        saveBrowserInfoRequest.RequestData.IsMobileDevice = currentRequest.Browser.IsMobileDevice
        saveBrowserInfoRequest.RequestData.IPAddress = currentRequest.UserHostAddress
        saveBrowserInfoRequest.RequestData.ForComposer = Diamond.Web.BaseControls.ComposerCommon.IsComposerSystem
        Using proxy As New DCS.Proxies.WebSiteServiceProxy()
            Try
                Dim saveBrowserInfoResponse As DCS.Messages.WebSiteService.SaveBrowserInfo.Response =
                    proxy.SaveBrowserInfo(saveBrowserInfoRequest)
            Catch ex As Exception

            End Try
        End Using

    End Sub
    Private Sub DiamondSaveUserLogin()

        ' TODO: remove this if statement if logging Diamond users is added
        If Not Diamond.Web.BaseControls.ComposerCommon.IsComposerSystem Then
            Return
        End If

        Dim saveUserLoginRequest As New DCS.Messages.WebSiteService.SaveUserLogin.Request()
        saveUserLoginRequest.RequestData.UsersId = Diamond.Web.BaseControls.SignedOnUser.UsersId
        saveUserLoginRequest.RequestData.SessionId = Web.HttpContext.Current.Session.SessionID
        saveUserLoginRequest.RequestData.ForComposer = Diamond.Web.BaseControls.ComposerCommon.IsComposerSystem
        Using proxy As New DCS.Proxies.WebSiteServiceProxy()
            Try
                Dim saveUserLoginResponse As DCS.Messages.WebSiteService.SaveUserLogin.Response =
                    proxy.SaveUserLogin(saveUserLoginRequest)
            Catch ex As Exception

            End Try
        End Using

    End Sub
    Private Sub DiamondSetRememberMeCookie(loginName As String, password As String, rememberMeExpirationDays As Int32)

        Dim token As New DCS.DiamondSecurityToken(String.Empty, 0, 0, DateTime.Now, DateTime.Now.AddDays(rememberMeExpirationDays))
        token.ExtendedProperties("loginName") = loginName
        token.ExtendedProperties("password") = password
        'Diamond.Web.BaseControls.CookieUtility.Update(DiamondGetRememberMeCookieKey, token.ToDataContractXML(),
        '        token.ValidTo, DCE.Security.CookieEncryptionType.TripleDES)
        'note: DCE.Security.CookieEncryptionType can only be used when referencing 534 assemblies; 531 uses Diamond.Web.BaseControls.CookieUtility.EncryptionType
        'Diamond.Web.BaseControls.CookieUtility.Update(DiamondGetRememberMeCookieKey, token.ToDataContractXML(),
        '        token.ValidTo, Diamond.Web.BaseControls.CookieUtility.EncryptionType.TripleDES)

    End Sub
    Private Function DiamondGetRememberMeCookieValue() As Diamond.Web.Security.LoginUtility.RememberMeSettings

        Dim result As New Diamond.Web.Security.LoginUtility.RememberMeSettings()
        If DCS.Proxies.ProxyBase.DiamondSecurityToken Is Nothing Then
            Try
                'Dim tokenXML As String =
                '        Diamond.Web.BaseControls.CookieUtility.RetrieveValue(DiamondGetRememberMeCookieKey, DCE.Security.CookieEncryptionType.TripleDES)
                'note: DCE.Security.CookieEncryptionType can only be used when referencing 534 assemblies; 531 uses Diamond.Web.BaseControls.CookieUtility.EncryptionType
                'Dim tokenXML As String =
                '        Diamond.Web.BaseControls.CookieUtility.RetrieveValue(DiamondGetRememberMeCookieKey, Diamond.Web.BaseControls.CookieUtility.EncryptionType.TripleDES)
                ''If tokenXML.SafeLength() > 0 Then
                'If String.IsNullOrWhiteSpace(tokenXML) = False AndAlso Len(tokenXML) > 0 Then
                '    Dim securityToken As DCS.DiamondSecurityToken =
                '            DCO.InsObject.FromDataContract(Of DCS.DiamondSecurityToken)(tokenXML)
                '    If securityToken IsNot Nothing AndAlso securityToken.ValidTo >= DateTime.Today Then
                '        If securityToken.ExtendedProperties.Contains("loginName") Then
                '            result.Login = securityToken.ExtendedProperties("loginName")
                '        End If
                '        If securityToken.ExtendedProperties.Contains("password") Then
                '            result.Password = securityToken.ExtendedProperties("password")
                '        End If
                '    End If
                'End If
            Catch
                ' Do nothing. Assume token invalid if code above fails. Typically due to deserializing the security token.
            End Try
        End If
        Return result

    End Function
    Private Function DiamondGetRememberMeCookieKey() As String
        If Diamond.Web.BaseControls.RouteConfiguration.CurrentRouteId > 0 Then
            Return String.Format("{0}_{1}",
                            RememberMeCookieKey,
                            Diamond.Web.BaseControls.RouteConfiguration.CurrentRouteId.ToString)
        Else
            Return RememberMeCookieKey
        End If
    End Function
    Private Const RememberMeCookieKey As String = "diamToken"
    Private Function DiamondGetOriginalExceptionMessage(ex As Exception) As String

        If ex IsNot Nothing Then
            If ex.InnerException IsNot Nothing Then
                Return DiamondGetOriginalExceptionMessage(ex.InnerException)
            Else
                Return ex.Message
            End If
        Else
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' Log out of the Diamond system to clear the Diamond web cache.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LogoutDiamond()
        'Diamond.Web.BaseControls.LogoutDiamondUser()
        'updated 9/26/2019 for 534
        'Diamond.Web.Security.LoginUtility.Logout()
        'updated 10/8/2019
        DiamondLogoutFunctionality()
        If Diamond.Web.BaseControls.SignedOnUserID > 0 Then Errors.Add(New ErrorObject("An error was encountered while logging out of the Diamond system.", "An error was encountered while logging out of the Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
        'If Diamond.Web.BaseControls.SignedOnUser.UsersId > 0 Then Errors.Add(New ErrorObject("An error was encountered while logging out of the Diamond system.", "An error was encountered while logging out of the Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
    End Sub
    'added 10/8/2019 for 534
    Private Sub DiamondLogoutFunctionality(Optional ByVal retainSessionId As Boolean = False)
        Try
            'note: not going to do all of the Diamond functionality; not even manually calling DiamondSaveUserLogin right now, but it's still happening; not sure what would be saved on Logout (couldn't find field in Users table)
            'DiamondSaveUserLogout()

            DCS.Proxies.ProxyBase.DiamondSecurityToken = Nothing

            If Not retainSessionId Then
                'note: we also call Session.Abandon from our AgentPort Logout functionality
                System.Web.HttpContext.Current.Session.Abandon()
                'note: not going to do all of the Diamond functionality; we call Session.Abandon on our Logout functionality
                'If Not Web.HttpContext.Current.Session.IsCookieless Then
                '    'note: these methods can only be used when referencing 534 assemblies
                '    'Diamond.Web.BaseControls.CookieUtility.DeleteLoggedInCookie()
                '    'Diamond.Web.BaseControls.CookieUtility.DeleteSessionIdCookie()
                'Else
                '    ' TT 210331 Cookieless session issue
                '    'RedirectToPage(String.Format("{0}", RouteConfiguration.CurrentRouteValue), False)
                'End If
            Else
                System.Web.HttpContext.Current.Session.Clear()
            End If
        Catch ex As Exception
            'Dim rethrow As Boolean =
            '        Common.ExceptionManagement.Utility.HandleException(ex,
            '                    Common.ExceptionManagement.Policies.ClientPassExceptionPolicy)
            'If (rethrow) Then Throw New Exception(String.Format("Logout: {0}", ex.Message), ex)
        Finally

        End Try

    End Sub
    Private Sub DiamondSaveUserLogout()

        ' TODO: remove this if statement if logging Diamond users is added
        If Not Diamond.Web.BaseControls.ComposerCommon.IsComposerSystem Then
            Return
        End If

        Dim saveUserLogoutRequest As New DCS.Messages.WebSiteService.SaveUserLogout.Request()
        If Diamond.Web.BaseControls.Common.SignedOnUser IsNot Nothing Then
            saveUserLogoutRequest.RequestData.UsersId = Diamond.Web.BaseControls.Common.SignedOnUser.UsersId
        End If
        saveUserLogoutRequest.RequestData.SessionId = Web.HttpContext.Current.Session.SessionID
        saveUserLogoutRequest.RequestData.ForComposer = Diamond.Web.BaseControls.ComposerCommon.IsComposerSystem
        Using proxy As New DCS.Proxies.WebSiteServiceProxy()
            Try
                Dim saveUserLogoutResponse As DCS.Messages.WebSiteService.SaveUserLogout.Response =
                    proxy.SaveUserLogout(saveUserLogoutRequest)
            Catch ex As Exception

            End Try
        End Using

    End Sub

    ''' <summary>
    ''' This procedure loads all available security questions
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Admin_LoadUserSecurityQuestions()
        Errors.Clear()
        Dim request As New DCS.Messages.AdministrationService.LoadUserSecurityQuestions.Request
        Dim response As New DCS.Messages.AdministrationService.LoadUserSecurityQuestions.Response

        Try
            Using proxy As New Proxies.AdministrationServiceProxy
                response = proxy.LoadUserSecurityQuestions(request)
            End Using

            If response IsNot Nothing AndAlso response.ResponseData.UserSecurityQuestions IsNot Nothing Then
                ListOfAllSecurityQuestions = response.ResponseData.UserSecurityQuestions

                ListOfDiamondSecurityQuestions = New List(Of DiamondSecurityQuestion)
                For Each diaSQ As DCO.Administration.UserSecurityQuestion In ListOfAllSecurityQuestions
                    ListOfDiamondSecurityQuestions.Add(New DiamondSecurityQuestion(diaSQ))
                Next

                If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                    For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                        Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                    Next
                End If
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error loading security questions.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    ''' <summary>
    ''' Delete a specific security question
    ''' </summary>
    ''' <param name="questionToDelete">The text value of the question to be deleted.</param>
    ''' <remarks></remarks>
    Public Sub Admin_DeleteUserSecurityQuestion(ByVal questionToDelete As String)
        Errors.Clear()
        Try
            If questionToDelete IsNot Nothing Then
                CurrentSecurityQuestion = New DCO.Administration.UserSecurityQuestion

                Dim request As New DCS.Messages.AdministrationService.DeleteUserSecurityQuestion.Request
                Dim response As New DCS.Messages.AdministrationService.DeleteUserSecurityQuestion.Response

                CurrentSecurityQuestion.Question = questionToDelete
                request.RequestData.UserSecurityQuestion = CurrentSecurityQuestion

                Using Proxy As New Proxies.AdministrationServiceProxy
                    response = Proxy.DeleteUserSecurityQuestion(request)

                    If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                        For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                            Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        Next
                    End If
                End Using
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error deleting existing security question.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    ''' <summary>
    ''' Add a new security question
    ''' </summary>
    ''' <param name="questionToAdd">The text value of the question to be added</param>
    ''' <remarks></remarks>
    Public Sub Admin_AddUserSecurityQuestion(ByVal questionToAdd As String)
        Errors.Clear()
        Try
            If questionToAdd IsNot Nothing Then
                CurrentSecurityQuestion = New DCO.Administration.UserSecurityQuestion

                Dim request As New DCS.Messages.AdministrationService.SaveUserSecurityQuestion.Request
                Dim response As New DCS.Messages.AdministrationService.SaveUserSecurityQuestion.Response

                CurrentSecurityQuestion.Question = questionToAdd

                request.RequestData.UserSecurityQuestion = CurrentSecurityQuestion

                Using proxy As New Proxies.AdministrationServiceProxy
                    response = proxy.SaveUserSecurityQuestion(request)

                    If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                        For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                            Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        Next
                    End If
                End Using
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error adding new security question.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    ''' <summary>
    ''' Sets the User ID for the specified login name.  User ID property will be set to the corresponding system.
    ''' </summary>
    ''' <param name="loginName">Login value for this user in the system to be checked.</param>
    ''' <param name="loginDomain">Required field for Diamond User ID lookup.  Web users are typically "agency", employees are "ifm.ifmic".</param>
    ''' <param name="systemLocated">Enumeration of the system to check.</param>
    ''' <remarks></remarks>
    Public Sub Admin_GetUserID(ByRef userObject As WebUser, ByVal loginName As String, ByVal systemLocated As Enums.UserLocation, Optional ByVal loginDomain As String = "agency")
        Try
            Select Case systemLocated
                Case Is = Enums.UserLocation.Diamond
                    Dim requestValid As New DCS.Messages.SecurityService.IsValidDiamondLogin.Request
                    Dim responseValid As New DCS.Messages.SecurityService.IsValidDiamondLogin.Response

                    requestValid.RequestData.LoginDomain = loginDomain
                    requestValid.RequestData.LoginName = loginName

                    Using proxy As New Proxies.SecurityServiceProxy
                        responseValid = proxy.IsValidDiamondLogin(requestValid)
                    End Using

                    If responseValid IsNot Nothing AndAlso responseValid.ResponseData.Valid = True Then
                        Dim request As New DCS.Messages.SecurityService.GetUser.Request
                        Dim response As New DCS.Messages.SecurityService.GetUser.Response

                        request.RequestData.LoginDomain = loginDomain
                        request.RequestData.LoginName = loginName

                        Using proxy As New Proxies.SecurityServiceProxy
                            response = proxy.GetUser(request)
                        End Using

                        If response IsNot Nothing AndAlso response.ResponseData.User IsNot Nothing Then
                            If response.DiamondValidation.ValidationItems.Count > 0 Then
                                For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                                    Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                                Next
                            End If
                            userObject.diamond_UserID = response.ResponseData.User.UsersId
                            DiamondUserID = response.ResponseData.User.UsersId
                        End If
                    End If
                Case Is = Enums.UserLocation.Legacy
                    CheckDatabaseConnections()
                    Using sqlUserIDLookup As New SQLselectObject(legacy_Connection)
                        sqlUserIDLookup.queryOrStoredProc = "Select user_id from tbl_users where username = '" & loginName & "'"
                        Using dr As SqlDataReader = sqlUserIDLookup.GetDataReader
                            If dr IsNot Nothing AndAlso dr.HasRows Then
                                dr.Read()
                                userObject.legacy_UserID = Convert.ToInt16(dr.Item("user_id").ToString)
                                LegacyUserID = Convert.ToInt16(dr.Item("user_id").ToString)
                            Else
                                Errors.Add(New ErrorObject("User not found in Legacy system.", "User not found in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                            End If
                        End Using
                        If sqlUserIDLookup.hasError Then Errors.Add(New ErrorObject(sqlUserIDLookup.errorMsg, "Error finding existing user's ID in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                    End Using
            End Select


        Catch ex As Exception
            'Errors.Add(New ErrorObject(ex.ToString, "Error finding existing user's ID.", Enums.ErrorLevel.Severe, systemLocated))
        End Try
    End Sub

    Private Sub AddDiamondUser(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation)
        If Admin_VerifyUniqueLogin(userObject, Enums.UserLocation.Diamond) = True _
                    AndAlso (IsValid(userObject.diamond_LoginName, Enums.Validation.Username) And IsValid(userObject.diamond_Password, Enums.Validation.Password)) Then

            'check for sufficient data

            Dim request As New DCS.Messages.AdministrationService.SaveUser.Request
            Dim response As New DCS.Messages.AdministrationService.SaveUser.Response

            With request.RequestData
                .UsersRecord = New DCO.Administration.Users
                With .UsersRecord
                    .Active = True
                    .LoginDomain = userObject.diamond_LoginDomain
                    .LoginName = userObject.diamond_LoginName
                    .NotifyUW = userObject.diamond_IsUnderwriter
                    Dim encoder As New System.Text.ASCIIEncoding()
                    Dim md5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
                    .Password = encoder.GetString(md5Hasher.ComputeHash(encoder.GetBytes(userObject.diamond_Password)))
                    .SUId = userObject.diamond_SUId
                    .Supervisor = userObject.diamond_IsSupervisor
                    .UserCategoryId = userObject.diamond_UserCategoryID
                    .UserCode = userObject.diamond_UserCode
                    .UserEmailAddr = userObject.diamond_UserEmailAddress
                    .PasswordMustBeChanged = userObject.diamond_MustChangePassword
                End With
                If userObject.diamond_PrimaryAgencyID <= 0 AndAlso userObject.diamond_PrimaryAgencyCode > 0 Then
                    userObject.diamond_PrimaryAgencyID = userObject.getPrimaryAgencyID(userObject.diamond_PrimaryAgencyCode)
                End If

                If (userObject.diamond_SecondaryAgencyIDs Is Nothing Or userObject.diamond_SecondaryAgencyIDs.Count = 0) AndAlso userObject.diamond_SecondaryAgencyCodes IsNot Nothing Then
                    userObject.diamond_SecondaryAgencyIDs = userObject.getSecondaryAgencyIDs(userObject.diamond_SecondaryAgencyCodes)
                End If

                If userObject.diamond_PrimaryAgencyID > 0 Then
                    Dim agencyLink As New DCO.Administration.UserAgencyLink
                    agencyLink.AgencyId = userObject.diamond_PrimaryAgencyID
                    agencyLink.IsAgencyAdministrator = userObject.diamond_IsAgencyAdmin
                    agencyLink.UserAgencyRelationTypeId = Diamond.Common.Enums.UserAgencyRelationType.UserAgencyRelationType_PRIMARY
                    .UserLinkRecords.Add(agencyLink)
                End If

                If userObject.diamond_SecondaryAgencyIDs IsNot Nothing AndAlso userObject.diamond_SecondaryAgencyIDs.Count > 0 Then
                    For Each secID As Integer In userObject.diamond_SecondaryAgencyIDs
                        Dim agencyLink As New DCO.Administration.UserAgencyLink
                        agencyLink.AgencyId = secID
                        agencyLink.IsAgencyAdministrator = userObject.diamond_IsAgencyAdmin
                        agencyLink.UserAgencyRelationTypeId = Diamond.Common.Enums.UserAgencyRelationType.UserAgencyRelationType_SECONDARY
                        .UserLinkRecords.Add(agencyLink)
                    Next
                End If
            End With

            Using proxy As New Proxies.AdministrationServiceProxy
                If _UseTestToken = True Then
                    '5/26/2011 Don Mink - modification for 520
                    If AppSettings("SetProxyBaseSecurityTokenWhenNeeded") IsNot Nothing AndAlso UCase(AppSettings("SetProxyBaseSecurityTokenWhenNeeded")) = "YES" Then
                        Proxies.ProxyBase.DiamondSecurityToken = GetTestToken()
                    Else
                        request.DiamondSecurityToken = GetTestToken()
                    End If
                End If

                response = proxy.SaveUser(request)
            End Using

            If response IsNot Nothing AndAlso response.ResponseData.Success = True Then
                Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond)
                If userObject.diamond_ListOfDiamondSecurityQuestions IsNot Nothing AndAlso userObject.diamond_ListOfDiamondSecurityQuestions.Count > 0 Then
                    Dim userRequest As New DCS.Messages.SecurityService.GetUserId.Request
                    Dim userResponse As New DCS.Messages.SecurityService.GetUserId.Response

                    userRequest.RequestData.LoginDomain = userObject.diamond_LoginDomain
                    userRequest.RequestData.LoginName = userObject.diamond_LoginName

                    Using proxy As New Proxies.SecurityServiceProxy
                        userResponse = proxy.GetUserId(userRequest)
                    End Using

                    If userResponse IsNot Nothing AndAlso userResponse.ResponseData.UserId > 0 Then
                        userObject.diamond_UserID = userResponse.ResponseData.UserId
                        User_SaveUserSecurityQuestionsAnswers(userObject.diamond_ListOfDiamondSecurityQuestions)
                    End If
                End If
            ElseIf response IsNot Nothing AndAlso response.DiamondValidation.ValidationItems.Count > 0 Then
                For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                    Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                Next
            End If

            Dim cbdUserTypeRequest As New DCS.Messages.AdministrationService.SaveCBDUserType.Request
            Dim cbdUserTypeResponse As New DCS.Messages.AdministrationService.SaveCBDUserType.Response

            With cbdUserTypeRequest.RequestData
                .CBDUserTypeRecord = New DCO.Administration.UsersCompanyBranchDeptUserType
                With .CBDUserTypeRecord
                    .BranchId = AppSettings("DefaultWebUserBranchId") '42
                    .CompanyId = AppSettings("DefaultWebUserCompanyId") '1
                    .DepartmentId = AppSettings("DefaultWebUserDepartmentId") '76
                    .UserTypeId = AppSettings("DefaultWebUserUserTypeId") '230
                    .UsersId = userObject.diamond_UserID
                End With
            End With

            Using proxy As New Proxies.AdministrationServiceProxy
                cbdUserTypeResponse = proxy.SaveCBDUserType(cbdUserTypeRequest)
            End Using

            If cbdUserTypeResponse IsNot Nothing AndAlso cbdUserTypeResponse.ResponseData.Success = True Then
                Dim userAuthRequest As New DCS.Messages.AdministrationService.CopyUserAuthorities.Request
                Dim userAuthResponse As New DCS.Messages.AdministrationService.CopyUserAuthorities.Response

                With userAuthRequest.RequestData
                    .DestinationUsersCompanyBranchDeptUserTypeId = cbdUserTypeResponse.ResponseData.CBDUserTypeRecord.UsersCompanyBranchDeptUserTypeId
                    .DestinationUsersId = userObject.diamond_UserID
                    .SourceUsersId = AppSettings("DefaultWebUserId")
                End With

                Using proxy As New Proxies.AdministrationServiceProxy
                    userAuthResponse = proxy.CopyUserAuthorities(userAuthRequest)
                End Using
            End If

            If systemLocated = Enums.UserLocation.Diamond Then
                userObject.legacy_Username = userObject.diamond_LoginName
                userObject.legacy_Password = "Test*1234"
                If userObject.legacy_PrimaryAgencyCode <= 0 Then
                    If userObject.diamond_PrimaryAgencyCode > 0 Then
                        userObject.legacy_PrimaryAgencyCode = userObject.diamond_PrimaryAgencyCode
                    ElseIf userObject.diamond_PrimaryAgencyID > 0 Then
                        userObject.legacy_PrimaryAgencyCode = userObject.getDiamondAgencyCode(userObject.diamond_PrimaryAgencyID)
                    End If
                End If
                AddLegacyUser(userObject, systemLocated, True)
                If UseQuoteMaster() = True Then '7/15/2013 - added IF
                    AddQuotemasterUser(userObject, systemLocated)
                End If
            End If
        End If
    End Sub

    Private Sub AddLegacyUser(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation, Optional ByVal dummyUser As Boolean = False)
        If Admin_VerifyUniqueLogin(userObject, Enums.UserLocation.Legacy) = True _
                    AndAlso (IsValid(userObject.legacy_Username, Enums.Validation.Username) And IsValid(userObject.legacy_Password, Enums.Validation.Password)) Then
            Using sql As New SQLexecuteObject(legacy_Connection)

                'check for sufficient data

                sql.queryOrStoredProc = "sp_Save_AgencyUser"

                sql.inputParameters = New ArrayList

                If LegacyUserID <= 0 AndAlso userObject.legacy_UserID > 0 Then LegacyUserID = userObject.legacy_UserID

                If LegacyUserID <= 0 Then
                    sql.outputParameter = New SqlParameter("@user_id", SqlDbType.Int, 4)
                Else
                    sql.inputParameters.Add(New SqlParameter("@user_id", LegacyUserID))
                End If

                sql.inputParameters.Add(New SqlClient.SqlParameter("@role", If(dummyUser = False, userObject.legacy_Role, "")))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@full_name", String.Join(" ", userObject.legacy_NameArray)))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@username", If(dummyUser = False, userObject.legacy_Username, userObject.diamond_LoginName)))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@pass", If(dummyUser = False, userObject.legacy_Password, "Test*1234")))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@auth_type", If(dummyUser = False, userObject.legacy_AuthType, "f")))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@agency_id", If(dummyUser = False, userObject.legacy_AgencyID, userObject.getLegacyAgencyID(userObject)))) 'todo: if null, grab from code/session
                sql.inputParameters.Add(New SqlClient.SqlParameter("@FirstName", userObject.legacy_FirstName))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@MiddleName", userObject.legacy_MiddleName))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@LastName", userObject.legacy_LastName))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@Access6000", If(userObject.legacy_6000Access = True, "Y", "N")))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@UpdatedForDiamond", If(dummyUser = False, If(userObject.legacy_ConvertedToDiamond = True, "Y", "N"), "Y")))
                'sql.inputParameters.Add(New SqlClient.SqlParameter("@EmailAddress", If(dummyUser = False, userObject.legacy_EmailAddress, "")))

                sql.ExecuteStatement()

                If sql.rowsAffected = 0 Then
                    Errors.Add(New ErrorObject("Zero rows affected during sp_Save_AgencyUser procedure. - " & sql.errorMsg, "Error saving new web user to Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                Else
                    If LegacyUserID <= 0 Then
                        LegacyUserID = sql.outputParameter.Value
                        userObject.legacy_UserID = sql.outputParameter.Value
                    End If
                End If
            End Using

            If UseQuoteMaster() = True Then '7/15/2013 - added IF
                If systemLocated = Enums.UserLocation.Legacy Then AddQuotemasterUser(userObject, Enums.UserLocation.Quotemaster)
            End If
        End If
    End Sub

    Private Sub AddQuotemasterUser(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation)
        If CheckQuoteMasterAgency(userObject) = False Then
            Errors.Add(New ErrorObject("Agency " & userObject.legacy_PrimaryAgencyCode & " is not in QuoteMaster.", "Error saving new web user to Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
        Else
            Using sql As New SQLexecuteObject(legacy_Connection)
                sql.queryOrStoredProc = "sp_Save_AgencyUser_QM"

                sql.inputParameters = New ArrayList

                sql.inputParameters.Add(New SqlClient.SqlParameter("@agencyID", userObject.QM_agencyID))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@firstName", userObject.legacy_FirstName))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@middleName", userObject.legacy_MiddleName))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@lastName", userObject.legacy_LastName))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@userName", userObject.legacy_Username))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@passWord", userObject.legacy_Password))

                'insert
                sql.inputParameters.Add(New SqlClient.SqlParameter("@stateAbrv", userObject.QM_st))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@cpNodeID", userObject.QM_cp))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@autoCreditAccount", userObject.QM_autoCredit))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@propCreditAccount", userObject.QM_propCredit))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@mvrAccount", userObject.QM_mvr))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@autoClueAccount", userObject.QM_autoClue))
                sql.inputParameters.Add(New SqlClient.SqlParameter("@propClueAccount", userObject.QM_propClue))

                sql.ExecuteStatement()

                If sql.rowsAffected = 0 Then
                    Errors.Add(New ErrorObject(sql.errorMsg, "Error saving new web user to Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                End If
            End Using
        End If
    End Sub

    Public Sub Admin_UpgradeWebUser(ByVal userObject As WebUser)
        Errors.Clear()
        CheckDatabaseConnections()
        Try
            AddDiamondUser(userObject, Enums.UserLocation.AllSystems)

            If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(userObject.legacy_Username) Then
                Admin_GetUserID(userObject, userObject.legacy_Username, Enums.UserLocation.Legacy)
                If userObject.legacy_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Legacy system.", "User does not exist in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
            End If

            'Admin_ModifyWebUser(userObject, Enums.UserLocation.LegacyAndQuotemaster)

            If DiamondUserID > 0 AndAlso LegacyUserID > 0 Then
                InsertLink(userObject)
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error upgrading web user.", Enums.ErrorLevel.Severe, Enums.UserLocation.AllSystems))
        End Try
    End Sub

    ''' <summary>
    ''' Add a new user to any or all systems
    ''' </summary>
    ''' <param name="userObject">Contains all data pertaining to the user to be added.</param>
    ''' <param name="systemLocated">Enumeration of the system(s) to add this user to.</param>
    ''' <remarks></remarks>
    Public Sub Admin_AddNewWebUser(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation)
        Errors.Clear()
        CheckDatabaseConnections()
        Try
            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.Diamond Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster _
                Or systemLocated = Enums.UserLocation.DiamondAndLegacy Then 'check Diamond

                AddDiamondUser(userObject, systemLocated)

            End If

            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndLegacy Or systemLocated = Enums.UserLocation.Legacy _
                Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster Then 'check Legacy

                AddLegacyUser(userObject, systemLocated)

            End If

            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster _
                Or systemLocated = Enums.UserLocation.Quotemaster Then 'check QM

                If UseQuoteMaster() = True Then '7/15/2013 - added IF
                    AddQuotemasterUser(userObject, systemLocated)
                End If

            End If

            If DiamondUserID > 0 AndAlso LegacyUserID > 0 Then
                InsertLink(userObject)
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error saving new web user.", Enums.ErrorLevel.Severe, systemLocated))
        End Try
    End Sub

    ''' <summary>
    ''' Ensure agency exists in QuoteMaster
    ''' </summary>
    ''' <param name="userObject">If exists, set properties within WebUser object from agency record.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckQuoteMasterAgency(ByRef userObject As WebUser) As Boolean
        CheckDatabaseConnections()
        Using sql As New SQLselectObject(quotemaster_Connection)
            sql.queryOrStoredProc = "SELECT AT.AgencyID, PT.ProducerID, CPAT.StateAbrv, CPAT.CPNodeId, CPAT.AutoCreditAccount, CPAT.PropCreditAccount, CPAT.MVRAccount, CPAT.AutoCLUEAccount, CPAT.PropCLUEAccount FROM AgencyTable as AT left join ProducersTable as PT on PT.AgencyID = AT.AgencyID and PT.ProducerID = (SELECT max(PT2.ProducerID) from ProducersTable as PT2 where PT2.AgencyID = AT.AgencyID) left join ChoicePointAccountTable as CPAT on CPAT.ProducerID = PT.ProducerID where AT.AgencyNum = '" & userObject.legacy_PrimaryAgencyCode & "'"

            Dim dr As SqlClient.SqlDataReader = sql.GetDataReader

            If Not dr Is Nothing Then
                If dr.HasRows Then
                    dr.Read()
                    userObject.QM_agencyID = dr.Item("AgencyID").ToString.Trim

                    If dr.Item("ProducerID").ToString.Trim <> "" Then
                        userObject.QM_existingProducerID = dr.Item("ProducerID").ToString.Trim
                        userObject.QM_st = dr.Item("StateAbrv").ToString.Trim
                        userObject.QM_cp = dr.Item("CPNodeId").ToString.Trim
                        userObject.QM_autoCredit = dr.Item("AutoCreditAccount").ToString.Trim
                        userObject.QM_propCredit = dr.Item("PropCreditAccount").ToString.Trim
                        userObject.QM_mvr = dr.Item("MVRAccount").ToString.Trim
                        userObject.QM_autoClue = dr.Item("AutoCLUEAccount").ToString.Trim
                        userObject.QM_propClue = dr.Item("PropCLUEAccount").ToString.Trim

                        'Return True
                    Else
                        'Return False
                    End If
                    Return True 'True as long as agency is there; don't worry about existing producer info
                Else
                    If sql.hasError = True Then
                        Errors.Add(New ErrorObject(sql.errorMsg, "Error loading Quotemaster information", Enums.ErrorLevel.Warning, Enums.UserLocation.Quotemaster))
                    Else
                        'no info found
                    End If
                End If
            Else
                'error
                Errors.Add(New ErrorObject("Datareader is set to nothing - " & sql.errorMsg, "Error loading Quotemaster information", Enums.ErrorLevel.Warning, Enums.UserLocation.Quotemaster))
            End If
        End Using

        Return False
    End Function

    ''' <summary>
    ''' Validation for checking login name uniqueness for a target system
    ''' </summary>
    ''' <param name="userObject">WebUser object containing login name and domain</param>
    ''' <param name="systemLocated">Enumeration of the system to check</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Admin_VerifyUniqueLogin(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation) As Boolean
        If systemLocated = Enums.UserLocation.Diamond Then
            Try
                Dim request As New DCS.Messages.SecurityService.DoesLoginAndDomainExist.Request
                Dim response As DCS.Messages.SecurityService.DoesLoginAndDomainExist.Response

                request.RequestData.LoginDomain = userObject.diamond_LoginDomain
                request.RequestData.LoginName = userObject.diamond_LoginName

                Using proxy As New Diamond.Common.Services.Proxies.SecurityServiceProxy
                    response = proxy.DoesLoginAndDomainExist(request)
                End Using

                If response IsNot Nothing Then
                    If response.ResponseData.Success = True Then Errors.Add(New ErrorObject("This login name is not unique to the Diamond system.", "This login name is not unique to the Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                    Return Not response.ResponseData.Success
                End If

                Return False
            Catch ex As Exception
                Errors.Add(New ErrorObject(ex.ToString, "Error verifying unique login for Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
            End Try
        ElseIf systemLocated = Enums.UserLocation.Legacy Then
            Using Sql As New SQLselectObject(legacy_Connection)
                Sql.queryOrStoredProc = "Select  user_id from tbl_users where username = '" & userObject.legacy_Username & "'"
                Using dr As SqlDataReader = Sql.GetDataReader
                    If dr Is Nothing Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "Error verifying unique login for Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                        Return False
                    End If
                    If dr.HasRows Then
                        Errors.Add(New ErrorObject("This login name is not unique to the Legacy system.", "This login name is not unique to the Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                        Return False
                    End If
                End Using
                Sql.queryOrStoredProc = "Select user_id from tbl_users where username = '" & userObject.diamond_LoginName & "'"
                Using dr As SqlDataReader = Sql.GetDataReader
                    If dr Is Nothing Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "Error verifying unique login for Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                        Return False
                    End If
                    If dr.HasRows Then
                        Errors.Add(New ErrorObject("This login name is not unique to the Legacy system.", "This login name is not unique to the Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                        Return False
                    Else
                        Return True
                    End If
                End Using
            End Using
        End If
    End Function

    ''' <summary>
    ''' Delete a user from any or all systems
    ''' </summary>
    ''' <param name="userObject">Contains all data pertaining to the user to be deleted.  User ID, Login Name and Login Domain required for Diamond deletion.</param>
    ''' <param name="systemLocated">Enumeration of the system(s) to delete this user from.</param>
    ''' <remarks></remarks>
    Public Sub Admin_DeleteWebUser(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation)
        Errors.Clear()
        If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.Diamond Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster _
                Or systemLocated = Enums.UserLocation.DiamondAndLegacy Then 'check Diamond

            If userObject.diamond_UserID <= 0 AndAlso (Not String.IsNullOrEmpty(DiamondUserLogin) Or Not String.IsNullOrEmpty(userObject.diamond_LoginName)) Then
                If String.IsNullOrEmpty(userObject.diamond_LoginName) Then userObject.diamond_LoginName = DiamondUserLogin
                Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "agency")
                If userObject.diamond_UserID <= 0 Then Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "ifm.ifmic")
                If userObject.diamond_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
            End If

            If DiamondUserID > 0 Then
                userObject.diamond_UserID = DiamondUserID
                'CheckDatabaseConnections()
                'Using sqlEx As New SQLexecuteObject(diamond_Connection)
                '    sqlEx.queryOrStoredProc = "assp_Diadmin_UsersUserSecurityQuestions_Delete"
                '    sqlEx.inputParameter = New SqlParameter("@users_id", userObject.diamond_UserID)
                '    sqlEx.ExecuteStatement()
                'End Using

                'If userObject.diamond_ListOfDiamondSecurityQuestions.Count = 0 Then
                '    Admin_LoadUserSecurityQuestions()
                '    userObject.diamond_ListOfDiamondSecurityQuestions = ListOfDiamondSecurityQuestions
                'End If

                'If userObject.diamond_ListOfDiamondSecurityQuestions.Count > 0 Then
                '    For Each diaQuestion As DiamondSecurityQuestion In userObject.diamond_ListOfDiamondSecurityQuestions
                '        Dim uqRequest As New DCS.Messages.UtilityService
                '    Next
                'End If


                'Dim request As New DCS.Messages.AdministrationService.DeleteDiamondUser.Request
                'Dim response As New DCS.Messages.AdministrationService.DeleteDiamondUser.Response
                'Try
                '    request.RequestData.UserRecord = New DCO.Administration.DiamondUsers
                '    With request.RequestData.UserRecord
                '        .UsersID = DiamondUserID
                '        .LoginName = userObject.diamond_LoginName
                '        .LoginDomain = userObject.diamond_LoginDomain
                '    End With

                '    Using proxy As New Proxies.AdministrationServiceProxy
                '        response = proxy.DeleteDiamondUser(request)
                '    End Using

                '    If response IsNot Nothing Then
                '        If response.DiamondValidation.HasAnyItems Then
                '            For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                '                Errors.Add(New ErrorObject(vItem.Message, "Error deleting web user from Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
                '            Next
                '        End If
                '    End If

                Try
                    Dim request As New DCS.Messages.AdministrationService.SaveUser.Request
                    Dim response As New DCS.Messages.AdministrationService.SaveUser.Response

                    request.RequestData.UsersRecord = New DCO.Administration.Users
                    request.RequestData.UsersRecord.SetIsNewValue(False)
                    With request.RequestData
                        .UsersRecord = New DCO.Administration.Users
                        With .UsersRecord
                            .UsersId = userObject.diamond_UserID
                            .Active = False
                            .LoginDomain = userObject.diamond_LoginDomain
                            .LoginName = userObject.diamond_LoginName
                            .NotifyUW = userObject.diamond_IsUnderwriter
                            Dim encoder As New System.Text.ASCIIEncoding()
                            Dim md5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
                            .Password = encoder.GetString(md5Hasher.ComputeHash(encoder.GetBytes(userObject.diamond_Password)))
                            .SUId = userObject.diamond_SUId
                            .Supervisor = userObject.diamond_IsSupervisor
                            .UsercategoryId = userObject.diamond_UserCategoryID
                            .UserCode = userObject.diamond_UserCode
                            .UserEmailAddr = userObject.diamond_UserEmailAddress
                            .PasswordMustBeChanged = userObject.diamond_MustChangePassword
                        End With
                        If userObject.diamond_PrimaryAgencyID <= 0 AndAlso userObject.diamond_PrimaryAgencyCode > 0 Then
                            userObject.diamond_PrimaryAgencyID = userObject.getPrimaryAgencyID(userObject.diamond_PrimaryAgencyCode)
                        End If

                        If (userObject.diamond_SecondaryAgencyIDs Is Nothing Or userObject.diamond_SecondaryAgencyIDs.Count = 0) AndAlso userObject.diamond_SecondaryAgencyCodes IsNot Nothing Then
                            userObject.diamond_SecondaryAgencyIDs = userObject.getSecondaryAgencyIDs(userObject.diamond_SecondaryAgencyCodes)
                        End If

                        If userObject.diamond_PrimaryAgencyID > 0 Then
                            Dim agencyLink As New DCO.Administration.UserAgencyLink
                            agencyLink.AgencyId = userObject.diamond_PrimaryAgencyID
                            agencyLink.IsAgencyAdministrator = userObject.diamond_IsAgencyAdmin
                            agencyLink.UserAgencyRelationTypeId = Diamond.Common.Enums.UserAgencyRelationType.UserAgencyRelationType_PRIMARY
                            .UserLinkRecords.Add(agencyLink)
                        End If

                        If userObject.diamond_SecondaryAgencyIDs IsNot Nothing AndAlso userObject.diamond_SecondaryAgencyIDs.Count > 0 Then
                            For Each secID As Integer In userObject.diamond_SecondaryAgencyIDs
                                Dim agencyLink As New DCO.Administration.UserAgencyLink
                                agencyLink.AgencyId = secID
                                agencyLink.IsAgencyAdministrator = userObject.diamond_IsAgencyAdmin
                                agencyLink.UserAgencyRelationTypeId = Diamond.Common.Enums.UserAgencyRelationType.UserAgencyRelationType_SECONDARY
                                .UserLinkRecords.Add(agencyLink)
                            Next
                        End If
                    End With

                    Using proxy As New Proxies.AdministrationServiceProxy
                        response = proxy.SaveUser(request)
                    End Using

                    If response IsNot Nothing Then
                        If response.DiamondValidation.HasAnyItems Then
                            For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                                Errors.Add(New ErrorObject(vItem.Message, "Error deleting web user from Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
                            Next
                        End If
                    End If

                    DeleteLink(userObject, Enums.UserLocation.Diamond)
                Catch ex As Exception
                    Errors.Add(New ErrorObject(ex.ToString, "Error deleting web user from Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
                End Try
            End If
        End If

        If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndLegacy Or systemLocated = Enums.UserLocation.Legacy _
                Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster Then 'check Legacy

            If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(userObject.legacy_Username) Then
                Admin_GetUserID(userObject, userObject.legacy_Username, Enums.UserLocation.Legacy)
                If userObject.legacy_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Legacy system.", "User does not exist in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
            End If

            If LegacyUserID >= 0 Then
                userObject.legacy_UserID = LegacyUserID
                If HasLegacyEFTPayment() = False Then
                    Using sql As New SQLexecuteObject(legacy_Connection)
                        sql.queryOrStoredProc = "sp_DeleteAgencyUser"
                        sql.inputParameter = New SqlClient.SqlParameter("@user_id", userObject.legacy_UserID)

                        sql.ExecuteStatement()

                        If sql.rowsAffected = 0 Then
                            Errors.Add(New ErrorObject(sql.errorMsg, "Error deleting web user from Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                        End If

                        DeleteLink(userObject, Enums.UserLocation.Legacy)
                    End Using
                Else
                    Errors.Add(New ErrorObject("Cannot delete Legacy users with pending EFT payments", "Cannot delete Legacy users with pending EFT payments", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                End If
            End If
        End If

        If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster _
                Or systemLocated = Enums.UserLocation.Quotemaster Then 'check QM

            If UseQuoteMaster() = True Then '7/15/2013 - added IF
                If userObject.legacy_PrimaryAgencyCode <= 0 Then
                    If userObject.diamond_PrimaryAgencyCode > 0 Then
                        userObject.legacy_PrimaryAgencyCode = userObject.diamond_PrimaryAgencyCode
                    ElseIf userObject.diamond_PrimaryAgencyID > 0 Then
                        userObject.legacy_PrimaryAgencyCode = userObject.getDiamondAgencyCode(userObject.diamond_PrimaryAgencyID)
                    End If
                End If

                If CheckQuoteMasterAgency(userObject) = False Then
                    Errors.Add(New ErrorObject("Agency " & userObject.legacy_PrimaryAgencyCode & " is not in QuoteMaster", "Error deleting user from Quotemaster system", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                Else
                    Using sql As New SQLexecuteObject(legacy_Connection)
                        sql.queryOrStoredProc = "sp_Delete_AgencyUser_QM"

                        Dim params As New ArrayList

                        params.Add(New SqlClient.SqlParameter("@agencyID", userObject.QM_agencyID))
                        params.Add(New SqlClient.SqlParameter("@userName", userObject.legacy_Username))

                        sql.inputParameters = params

                        sql.ExecuteStatement()

                        If sql.rowsAffected = 0 Then
                            Errors.Add(New ErrorObject("Zero rows affected by sp_Delete_AgencyUser_QM procedure - " & sql.errorMsg, "Error deleting user from Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                        End If
                    End Using
                End If
            End If
        End If
    End Sub

    Public Sub Admin_ReactivateUser(ByVal userObject As WebUser, ByVal systemLocation As Enums.UserLocation)
        CheckDatabaseConnections()
        If systemLocation = Enums.UserLocation.Diamond Then
            If userObject.diamond_UserID <= 0 AndAlso (Not String.IsNullOrEmpty(DiamondUserLogin) Or Not String.IsNullOrEmpty(userObject.diamond_LoginName)) Then
                If String.IsNullOrEmpty(userObject.diamond_LoginName) Then userObject.diamond_LoginName = DiamondUserLogin
                Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "agency")
                If userObject.diamond_UserID <= 0 Then Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "ifm.ifmic")
                If userObject.diamond_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
            End If

            If userObject.diamond_UserID > 0 Then
                Using Sql As New SQLexecuteObject(diamond_Connection)
                    Sql.queryOrStoredProc = "Update Users set active = '1' where users_id = '" & userObject.diamond_UserID & "'"
                    Sql.ExecuteStatement()
                    If Sql.hasError Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "An error was encountered while reactivating this user in the Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
                    End If

                    Sql.connection = legacy_Connection
                    Sql.queryOrStoredProc = "Update tbl_diamond_users set active = '1' where diamondid = '" & userObject.diamond_UserID & "'"
                    Sql.ExecuteStatement()
                    If Sql.hasError Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "An error was encountered while reactivating this user's Diamond to legacy link.", Enums.ErrorLevel.Warning, Enums.UserLocation.DiamondAndLegacy))
                    End If
                End Using
            End If
        ElseIf systemLocation = Enums.UserLocation.Legacy Then
            If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(userObject.legacy_Username) Then
                Admin_GetUserID(userObject, userObject.legacy_Username, Enums.UserLocation.Legacy)
                If userObject.legacy_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Legacy system.", "User does not exist in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
            End If

            If userObject.legacy_UserID > 0 Then
                Using Sql As New SQLexecuteObject(legacy_Connection)
                    Sql.queryOrStoredProc = "Update tbl_users set active = '1' where user_id = '" & userObject.legacy_UserID & "'"
                    Sql.ExecuteStatement()
                    If Sql.hasError Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "An error was encountered while reactivating this user in the legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
                    End If
                End Using

                '--also reactivate in QM
                If UseQuoteMaster() = True Then '7/15/2013 - added IF
                    If CheckQuoteMasterAgency(userObject) = False Then
                        'Errors.Add(New ErrorObject("Agency " & userObject.legacy_PrimaryAgencyCode & " is not in QuoteMaster", "Error deactivating user from Quotemaster system", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                    Else
                        Using sql As New SQLexecuteObject(legacy_Connection)
                            'sql.queryOrStoredProc = "sp_Delete_AgencyUser_QM"
                            Dim newAgId As String = userObject.QM_agencyID & "9999"
                            sql.queryOrStoredProc = "UPDATE QUOTEMASTER.IndianaFarmers.dbo.ProducersTable set AgencyID = '" & userObject.QM_agencyID & "' where ID = '" & userObject.legacy_Username & "' and AgencyID = '" & newAgId & "'"

                            'Dim params As New ArrayList

                            'params.Add(New SqlClient.SqlParameter("@agencyID", userObject.QM_agencyID))
                            'params.Add(New SqlClient.SqlParameter("@userName", userObject.legacy_Username))

                            'sql.inputParameters = params

                            sql.ExecuteStatement()

                            If sql.rowsAffected = 0 Then
                                'Errors.Add(New ErrorObject("Zero rows affected by QM reactivate query - " & sql.errorMsg, "Error deleting user from Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                            End If
                        End Using
                    End If
                End If
            End If
        Else
            Errors.Add(New ErrorObject("Invalid user location passed to Admin_ReactivateUser", "This user cannot be reactivated at this time.", Enums.ErrorLevel.Programmer, Enums.UserLocation.AllSystems))
        End If
    End Sub

    Public Sub Admin_DeactivateUser(ByVal userObject As WebUser, ByVal systemLocation As Enums.UserLocation)
        CheckDatabaseConnections()
        If systemLocation = Enums.UserLocation.Diamond Then
            If userObject.diamond_UserID <= 0 AndAlso (Not String.IsNullOrEmpty(DiamondUserLogin) Or Not String.IsNullOrEmpty(userObject.diamond_LoginName)) Then
                If String.IsNullOrEmpty(userObject.diamond_LoginName) Then userObject.diamond_LoginName = DiamondUserLogin
                Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "agency")
                If userObject.diamond_UserID <= 0 Then Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "ifm.ifmic")
                If userObject.diamond_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
            End If

            If userObject.diamond_UserID > 0 Then
                Using Sql As New SQLexecuteObject(diamond_Connection)
                    Sql.queryOrStoredProc = "Update Users set active = '0' where users_id = '" & userObject.diamond_UserID & "'"
                    Sql.ExecuteStatement()
                    If Sql.hasError Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "An error was encountered while deactivating this user from the Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
                    End If

                    Sql.connection = legacy_Connection
                    Sql.queryOrStoredProc = "Update tbl_diamond_users set active = '0' where diamondid = '" & userObject.diamond_UserID & "'"
                    Sql.ExecuteStatement()
                    If Sql.hasError Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "An error was encountered while deactivating this user's Diamond to legacy link.", Enums.ErrorLevel.Warning, Enums.UserLocation.DiamondAndLegacy))
                    End If
                End Using
            End If
        ElseIf systemLocation = Enums.UserLocation.Legacy Then
            If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(userObject.legacy_Username) Then
                Admin_GetUserID(userObject, userObject.legacy_Username, Enums.UserLocation.Legacy)
                If userObject.legacy_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Legacy system.", "User does not exist in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
            End If

            If userObject.legacy_UserID > 0 Then
                Using Sql As New SQLexecuteObject(legacy_Connection)
                    Sql.queryOrStoredProc = "Update tbl_users set active = '0' where user_id = '" & userObject.legacy_UserID & "'"
                    Sql.ExecuteStatement()
                    If Sql.hasError Then
                        Errors.Add(New ErrorObject(Sql.errorMsg, "An error was encountered while deactivating this user from the legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
                    End If
                End Using

                '--also deactivate in QM
                If UseQuoteMaster() = True Then '7/15/2013 - added IF
                    If CheckQuoteMasterAgency(userObject) = False Then
                        'Errors.Add(New ErrorObject("Agency " & userObject.legacy_PrimaryAgencyCode & " is not in QuoteMaster", "Error deactivating user from Quotemaster system", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                    Else
                        Using sql As New SQLexecuteObject(legacy_Connection)
                            'sql.queryOrStoredProc = "sp_Delete_AgencyUser_QM"
                            Dim newAgId As String = userObject.QM_agencyID & "9999"
                            sql.queryOrStoredProc = "UPDATE QUOTEMASTER.IndianaFarmers.dbo.ProducersTable set AgencyID = '" & newAgId & "' where ID = '" & userObject.legacy_Username & "' and AgencyID = '" & userObject.QM_agencyID & "'"

                            'Dim params As New ArrayList

                            'params.Add(New SqlClient.SqlParameter("@agencyID", userObject.QM_agencyID))
                            'params.Add(New SqlClient.SqlParameter("@userName", userObject.legacy_Username))

                            'sql.inputParameters = params

                            sql.ExecuteStatement()

                            If sql.rowsAffected = 0 Then
                                'Errors.Add(New ErrorObject("Zero rows affected by QM deactivate query - " & sql.errorMsg, "Error deleting user from Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                            End If
                        End Using
                    End If
                End If
            End If
        Else
            Errors.Add(New ErrorObject("Invalid user location passed to Admin_DeactivateUser", "This user cannot be deactivated at this time.", Enums.ErrorLevel.Programmer, Enums.UserLocation.AllSystems))
        End If
    End Sub

    ''' <summary>
    ''' Modify a web user in any or all systems
    ''' </summary>
    ''' <param name="userObject">Contains all data pertaining to the user to be modified.</param>
    ''' <param name="systemLocated">Enumeration of the system(s) to modify this user in.</param>
    ''' <remarks></remarks>
    Public Sub Admin_ModifyWebUser(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation, Optional ByVal JustUpdateLink As Boolean = False, Optional ByVal IgnorePasswordValidation As Boolean = False)
        Errors.Clear()
        CheckDatabaseConnections()
        Try
            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.Diamond Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster _
                Or systemLocated = Enums.UserLocation.DiamondAndLegacy Then 'check Diamond

                If userObject.diamond_UserID <= 0 AndAlso (Not String.IsNullOrEmpty(DiamondUserLogin) Or Not String.IsNullOrEmpty(userObject.diamond_LoginName)) Then
                    If String.IsNullOrEmpty(userObject.diamond_LoginName) Then userObject.diamond_LoginName = DiamondUserLogin
                    Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "agency")
                    If userObject.diamond_UserID <= 0 Then Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "ifm.ifmic")
                    If userObject.diamond_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                End If

                If JustUpdateLink = False Then
                    If IgnorePasswordValidation = True OrElse IsValid(userObject.diamond_Password, Enums.Validation.Password) Then
                        'If userObject.diamond_UserID <= 0 AndAlso (Not String.IsNullOrEmpty(DiamondUserLogin) Or Not String.IsNullOrEmpty(userObject.diamond_LoginName)) Then
                        '    If String.IsNullOrEmpty(userObject.diamond_LoginName) Then userObject.diamond_LoginName = DiamondUserLogin
                        '    Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "agency")
                        '    If userObject.diamond_UserID <= 0 Then Admin_GetUserID(userObject, userObject.diamond_LoginName, Enums.UserLocation.Diamond, "ifm.ifmic")
                        '    If userObject.diamond_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        'End If
                        If DiamondUserID > 0 Then
                            userObject.diamond_UserID = userObject.diamond_UserID
                            Dim request As New DCS.Messages.AdministrationService.SaveUser.Request
                            Dim response As New DCS.Messages.AdministrationService.SaveUser.Response

                            With request.RequestData
                                .UsersRecord = New DCO.Administration.Users
                                With .UsersRecord
                                    .SetIsNewValue(False)
                                    .Active = True
                                    .LoginDomain = userObject.diamond_LoginDomain
                                    .LoginName = userObject.diamond_LoginName
                                    .NotifyUW = userObject.diamond_IsUnderwriter
                                    Dim encoder As New System.Text.ASCIIEncoding()
                                    Dim md5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
                                    .Password = encoder.GetString(md5Hasher.ComputeHash(encoder.GetBytes(userObject.diamond_Password)))
                                    '.Password = userObject.Password
                                    .PasswordMustBeChanged = False
                                    .SUId = userObject.diamond_SUId
                                    .Supervisor = userObject.diamond_IsSupervisor
                                    .UsercategoryId = userObject.diamond_UserCategoryID
                                    .UserCode = userObject.diamond_UserCode
                                    .UserEmailAddr = userObject.diamond_UserEmailAddress
                                    .UsersId = userObject.diamond_UserID
                                    .PasswordMustBeChanged = userObject.diamond_MustChangePassword
                                End With

                                If userObject.diamond_PrimaryAgencyID > 0 Then
                                    Dim agencyLink As New DCO.Administration.UserAgencyLink
                                    agencyLink.AgencyId = userObject.diamond_PrimaryAgencyID
                                    agencyLink.IsAgencyAdministrator = userObject.diamond_IsAgencyAdmin
                                    agencyLink.UserAgencyRelationTypeId = Diamond.Common.Enums.UserAgencyRelationType.UserAgencyRelationType_PRIMARY
                                    .UserLinkRecords.Add(agencyLink)
                                End If

                                If userObject.diamond_SecondaryAgencyIDs IsNot Nothing AndAlso userObject.diamond_SecondaryAgencyIDs.Count > 0 Then
                                    For Each secID As Integer In userObject.diamond_SecondaryAgencyIDs
                                        Dim agencyLink As New DCO.Administration.UserAgencyLink
                                        agencyLink.AgencyId = secID
                                        agencyLink.IsAgencyAdministrator = userObject.diamond_IsAgencyAdmin
                                        agencyLink.UserAgencyRelationTypeId = Diamond.Common.Enums.UserAgencyRelationType.UserAgencyRelationType_SECONDARY
                                        .UserLinkRecords.Add(agencyLink)
                                    Next
                                End If
                            End With

                            Using proxy As New Proxies.AdministrationServiceProxy
                                response = proxy.SaveUser(request)
                            End Using

                            'added 3/24/2022
                            Dim okayToCheckSecurityQuestions As Boolean = False
                            If response IsNot Nothing AndAlso response.ResponseData IsNot Nothing Then
                                If response.ResponseData.Success = True Then
                                    okayToCheckSecurityQuestions = True
                                Else
                                    If response.DiamondValidation Is Nothing OrElse response.DiamondValidation.ValidationItems Is Nothing OrElse response.DiamondValidation.ValidationItems.Count = 0 Then
                                        If String.IsNullOrWhiteSpace(userObject.diamond_LoginDomain) = False AndAlso UCase(userObject.diamond_LoginDomain) = "IFM.IFMIC" Then
                                            If Admin_ModifyWebUser_ContinueToSecurityQuestionsForStaffOnSaveUserWithNoValidations() = True Then
                                                okayToCheckSecurityQuestions = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            'If response IsNot Nothing AndAlso response.ResponseData.Success = True Then
                            'updated 3/24/2022
                            If okayToCheckSecurityQuestions = True Then
                                If userObject.diamond_ListOfDiamondSecurityQuestions IsNot Nothing AndAlso userObject.diamond_ListOfDiamondSecurityQuestions.Count > 0 Then
                                    Dim userRequest As New DCS.Messages.SecurityService.GetUserId.Request
                                    Dim userResponse As New DCS.Messages.SecurityService.GetUserId.Response

                                    userRequest.RequestData.LoginDomain = userObject.diamond_LoginDomain
                                    userRequest.RequestData.LoginName = userObject.diamond_LoginName

                                    Using proxy As New Proxies.SecurityServiceProxy
                                        userResponse = proxy.GetUserId(userRequest)
                                    End Using

                                    If userResponse IsNot Nothing AndAlso userResponse.ResponseData.UserId > 0 Then
                                        userObject.diamond_UserID = userResponse.ResponseData.UserId
                                        User_SaveUserSecurityQuestionsAnswers(userObject.diamond_ListOfDiamondSecurityQuestions)
                                    ElseIf userResponse IsNot Nothing AndAlso userResponse.DiamondValidation.ValidationItems.Count > 0 Then
                                        For Each vItem As DCO.ValidationItem In userResponse.DiamondValidation.ValidationItems
                                            Errors.Add(New ErrorObject(vItem.Message, "Error modifying existing web user's security questions in the Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
                                        Next
                                    End If
                                End If
                            ElseIf response IsNot Nothing AndAlso response.DiamondValidation.ValidationItems.Count > 0 Then
                                For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                                    Errors.Add(New ErrorObject(vItem.Message, "Error modifying existing web user data in the Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
                                Next
                            End If
                        End If

                        If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(userObject.legacy_Username) Then
                            Admin_GetUserID(userObject, userObject.legacy_Username, Enums.UserLocation.Legacy)
                            If userObject.legacy_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Legacy system.", "User does not exist in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                        End If

                        If userObject.diamond_UserID >= 0 AndAlso userObject.legacy_UserID >= 0 Then UpdateLink(userObject)
                        Exit Sub
                    End If
                End If
            End If

            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndLegacy Or systemLocated = Enums.UserLocation.Legacy _
                Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster Then 'check Legacy

                If IgnorePasswordValidation = True OrElse IsValid(userObject.legacy_Password, Enums.Validation.Password) Then

                    If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(userObject.legacy_Username) Then
                        Admin_GetUserID(userObject, userObject.legacy_Username, Enums.UserLocation.Legacy)
                        If userObject.legacy_UserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Legacy system.", "User does not exist in Legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                    End If

                    If LegacyUserID > 0 Then
                        userObject.legacy_UserID = LegacyUserID
                        Using sql As New SQLexecuteObject(legacy_Connection)
                            sql.queryOrStoredProc = "sp_Save_AgencyUser"

                            sql.inputParameters = New ArrayList

                            If LegacyUserID <= 0 Then
                                sql.outputParameter = New SqlParameter("@user_id", SqlDbType.Int, 4)
                            Else
                                sql.inputParameters.Add(New SqlParameter("@user_id", userObject.legacy_UserID))
                            End If

                            sql.inputParameters.Add(New SqlClient.SqlParameter("@role", userObject.legacy_Role))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@full_name", String.Join(" ", userObject.legacy_NameArray)))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@username", userObject.legacy_Username))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@pass", userObject.legacy_Password))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@auth_type", userObject.legacy_AuthType))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@agency_id", userObject.legacy_AgencyID))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@FirstName", userObject.legacy_FirstName))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@MiddleName", userObject.legacy_MiddleName))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@LastName", userObject.legacy_LastName))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@Access6000", If(userObject.legacy_6000Access = True, "Y", "N")))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@UpdatedForDiamond", If(userObject.legacy_ConvertedToDiamond = True, "Y", "N")))
                            'sql.inputParameters.Add(New SqlClient.SqlParameter("@EmailAddress", userObject.legacy_EmailAddress))

                            sql.ExecuteStatement()

                            If sql.rowsAffected = 0 Then
                                Errors.Add(New ErrorObject("Zero rows affected by sp_Save_AgencyUser procedure", "Error modifying existing web user data in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                                If sql.hasError Then Errors.Add(New ErrorObject(sql.errorMsg, "Error modifying existing web user data in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                            Else
                                If LegacyUserID <= 0 Then
                                    LegacyUserID = sql.outputParameter.Value
                                End If
                                userObject.legacy_UserID = LegacyUserID
                            End If
                        End Using
                    End If
                End If
            End If

            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster _
                Or systemLocated = Enums.UserLocation.Quotemaster Then 'check QM

                If UseQuoteMaster() = True Then '7/15/2013 - added IF
                    If CheckQuoteMasterAgency(userObject) = False Then
                        Errors.Add(New ErrorObject("Agency " & userObject.legacy_PrimaryAgencyCode & " is not in QuoteMaster.", "Error saving new web user to Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Quotemaster))
                    Else
                        Using sql As New SQLexecuteObject(legacy_Connection)
                            sql.queryOrStoredProc = "sp_Save_AgencyUser_QM"

                            sql.inputParameters = New ArrayList

                            sql.inputParameters.Add(New SqlClient.SqlParameter("@agencyID", userObject.QM_agencyID))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@firstName", userObject.legacy_FirstName))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@middleName", userObject.legacy_MiddleName))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@lastName", userObject.legacy_LastName))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@userName", userObject.legacy_Username))
                            sql.inputParameters.Add(New SqlClient.SqlParameter("@passWord", userObject.legacy_Password))

                            If LegacyUserID > 0 Then
                                'update
                                If userObject.legacy_OldUsername = String.Empty Then userObject.legacy_OldUsername = userObject.legacy_Username
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@oldUserName", userObject.legacy_OldUsername))
                            Else
                                'insert
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@stateAbrv", userObject.QM_st))
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@cpNodeID", userObject.QM_cp))
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@autoCreditAccount", userObject.QM_autoCredit))
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@propCreditAccount", userObject.QM_propCredit))
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@mvrAccount", userObject.QM_mvr))
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@autoClueAccount", userObject.QM_autoClue))
                                sql.inputParameters.Add(New SqlClient.SqlParameter("@propClueAccount", userObject.QM_propClue))
                            End If

                            sql.ExecuteStatement()

                            If sql.rowsAffected = 0 Then
                                Errors.Add(New ErrorObject("Zero rows affected during Quotemaster new user save routine.", "Error saving new web user to Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                                If sql.hasError Then Errors.Add(New ErrorObject(sql.errorMsg, "Error modifying existing web user data in Quotemaster system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                            End If
                        End Using
                    End If
                End If
            End If

            If userObject.diamond_UserID >= 0 AndAlso userObject.legacy_UserID >= 0 Then UpdateLink(userObject)
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error modifying existing web user data.", Enums.ErrorLevel.Severe, systemLocated))
        End Try
    End Sub

    ''' <summary>
    ''' Add an agency producer to the Diamond system.
    ''' </summary>
    ''' <param name="agencyProducer">DiamondSecurityProducer object holding new agency producer details.</param>
    ''' <remarks></remarks>
    Public Sub Admin_AddAgencyProducer(ByVal agencyProducer As DiamondSecurityProducer)
        Errors.Clear()
        Dim request As New DCS.Messages.AdministrationService.SaveAgencyProducer.Request
        Dim response As New DCS.Messages.AdministrationService.SaveAgencyProducer.Response
        Try
            request.RequestData.AgencyProducer = New DCO.Policy.Agency.AgencyProducer
            With request.RequestData.AgencyProducer
                .NameAddressSourceId = Diamond.Common.Enums.NameAddressSource.AgencyProducer
                .AgencyId = agencyProducer.AgencyID
                .IsPrimaryContact = agencyProducer.IsPrimaryContact
                .Name = New DCO.Name
                With .Name
                    .PrefixName = agencyProducer.Prefix
                    .FirstName = agencyProducer.FirstName
                    .MiddleName = agencyProducer.MiddleName
                    .LastName = agencyProducer.LastName
                    .SuffixName = agencyProducer.Suffix
                End With
                If Not String.IsNullOrEmpty(agencyProducer.EmailAddress) Then .Emails.Add(New DCO.Email)
                .Emails(0).Address = agencyProducer.EmailAddress
                If Not String.IsNullOrEmpty(agencyProducer.PhoneNumber) Then .Phones.Add(New DCO.Phone)
                .Phones(0).Number = agencyProducer.PhoneNumber
                .ProducerCode = agencyProducer.ProducerCode
                .Status = agencyProducer.Status
            End With

            Using proxy As New Proxies.AdministrationServiceProxy
                response = proxy.SaveAgencyProducer(request)
            End Using

            If response IsNot Nothing Then
                If response.DiamondValidation.HasAnyItems Then
                    For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                        Errors.Add(New ErrorObject(vItem.Message, vItem.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                    Next
                End If
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error adding new agency producer to the Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    ''' <summary>
    ''' Delete an existing agency producer from the Diamond system.
    ''' </summary>
    ''' <param name="agencyProducerID">ID value of the agency producer to be deleted.</param>
    ''' <remarks></remarks>
    Public Sub Admin_DeleteAgencyProducer(ByVal agencyProducerID As Integer)
        Errors.Clear()
        Dim request As New DCS.Messages.AdministrationService.DeleteAgencyProducer.Request
        Dim response As New DCS.Messages.AdministrationService.DeleteAgencyProducer.Response
        Try
            request.RequestData.AgencyProducerID = agencyProducerID

            Using proxy As New Proxies.AdministrationServiceProxy
                response = proxy.DeleteAgencyProducer(request)
            End Using

            If response IsNot Nothing Then
                If response.DiamondValidation.HasAnyItems Then
                    For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                        Errors.Add(New ErrorObject(vItem.Message, vItem.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                    Next
                End If
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error deleting agency producer from Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    Public Sub Admin_SetPasswordRules()
        Errors.Clear()
        Try
            Dim request As New DCS.Messages.SecurityService.SavePasswordPolicy.Request
            Dim response As New DCS.Messages.SecurityService.SavePasswordPolicy.Response

            request.RequestData.PasswordPolicy = New DCO.PasswordPolicy
            With request.RequestData.PasswordPolicy
                .MinimumLength = 8
                .MinimumLowercase = 1
                .MinimumUppercase = 1
                .MinimumNumeric = 1
            End With

            Using proxy As New Proxies.SecurityServiceProxy
                response = proxy.SavePasswordPolicy(request)
            End Using

            If response IsNot Nothing Then

            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Based on User ID, populates UserAgenciesHT (hash table) and UserAgenciesDT (data table) with agency ID and relationship values.  Relationship value of 1 = Primary, 2 = Secondary.
    ''' </summary>
    ''' <param name="systemLocated">Enumeration of the system(s) to retrieve user agency from.</param>
    ''' <remarks></remarks>
    Public Sub Lookup_GetUserAgencies(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation)
        'If systemLocated = Enums.UserLocation.Diamond Then
        '    Try
        '        Dim request As New DCS.Messages.LookupService.GetUserAgencyByUser.Request
        '        Dim response As New DCS.Messages.LookupService.GetUserAgencyByUser.Response

        '        request.RequestData.UsersId = userObject.diamond_UserID

        '        UserAgenciesHT = New Hashtable
        '        UserAgenciesDT = New DataTable("UserAgencies")
        '        UserAgenciesDT.Columns.Add("AgencyID")
        '        UserAgenciesDT.Columns.Add("AgencyRelationship")
        '        If response IsNot Nothing AndAlso response.ResponseData.UserAgencies IsNot Nothing Then
        '            For Each agency As DCO.Lookup.GetUserAgency In response.ResponseData.UserAgencies
        '                UserAgenciesHT.Add(agency.AgencyId, agency.UserAgencyRelationTypeId)
        '                UserAgenciesDT.Rows.Add(agency.AgencyId, agency.UserAgencyRelationTypeId)
        '            Next
        '        End If
        '    Catch ex As Exception
        '        If Errors IsNot Nothing Then Errors.Add(New ErrorObject(ex.ToString, "Error loading user agencies from Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
        '    End Try
        'ElseIf systemLocated = Enums.UserLocation.Legacy Then
        '    If userObject.legacy_6000Access = True Then
        '        Using sql As New SQLselectObject(legacy_Connection)
        '            sql.queryOrStoredProc = "sp_Get6000Agencies"
        '            sql.parameter = New SqlClient.SqlParameter("@agCode", userObject.legacy_PrimaryAgencyCode)

        '            Using dr As SqlClient.SqlDataReader = sql.GetDataReader
        '                If Not dr Is Nothing Then
        '                    If dr.HasRows Then
        '                        While dr.Read
        '                            If userObject.legacy_SecondaryAgencyCodes Is Nothing Then userObject.legacy_SecondaryAgencyCodes = New List(Of Integer)
        '                            userObject.legacy_SecondaryAgencyCodes.Add(Int16.Parse(dr.Item("AgencyCode")))
        '                        End While
        '                    Else
        '                        If sql.hasError Then
        '                            Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
        '                        End If
        '                    End If
        '                Else
        '                    Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
        '                End If
        '            End Using
        '        End Using
        '    End If
        '    If userObject.legacy_SecondaryAgencyCodes.Count <= 0 Then
        '        Using sql As New SQLselectObject(legacy_Connection)
        '            sql.queryOrStoredProc = "sp_GetAssociateAgencies"
        '            sql.parameter = New SqlClient.SqlParameter("@agCode", userObject.legacy_PrimaryAgencyCode)

        '            Using dr As SqlClient.SqlDataReader = sql.GetDataReader
        '                If Not dr Is Nothing Then
        '                    If dr.HasRows Then
        '                        While dr.Read
        '                            If userObject.legacy_SecondaryAgencyCodes Is Nothing Then userObject.legacy_SecondaryAgencyCodes = New List(Of Integer)
        '                            userObject.legacy_SecondaryAgencyCodes.Add(Int16.Parse(dr.Item("agency_code")))
        '                        End While
        '                    Else
        '                        If sql.hasError Then
        '                            Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
        '                        End If
        '                    End If
        '                Else
        '                    Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
        '                End If
        '            End Using
        '        End Using
        '    End If
        'End If

        'updated 3/21/2022 to call new method
        Lookup_GetUserAgencies_WithResults(userObject, systemLocated)
    End Sub
    'added 3/21/2022 to be called by original method
    Public Sub Lookup_GetUserAgencies_WithResults(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation, Optional ByRef caughtError As Boolean = False, Optional ByRef caughtServiceError As Boolean = False, Optional ByRef caughtDatabaseError As Boolean = False)
        caughtError = False
        caughtServiceError = False
        caughtDatabaseError = False

        If systemLocated = Enums.UserLocation.Diamond Then
            Try
                Dim request As New DCS.Messages.LookupService.GetUserAgencyByUser.Request
                Dim response As New DCS.Messages.LookupService.GetUserAgencyByUser.Response

                request.RequestData.UsersId = userObject.diamond_UserID

                '3/21/2022 note: wasn't being called before
                If UseNewLogicToPopulateDiamondUserAgencyInfo() = True Then
                    Using proxy As New Diamond.Common.Services.Proxies.LookupServiceProxy
                        response = proxy.GetUserAgencyByUser(request)
                    End Using
                End If

                UserAgenciesHT = New Hashtable
                UserAgenciesDT = New DataTable("UserAgencies")
                UserAgenciesDT.Columns.Add("AgencyID")
                UserAgenciesDT.Columns.Add("AgencyRelationship")

                'new 3/21/2022
                Dim hasRecords As Boolean = False
                Dim hasUserAgencyRelationTypeId As Boolean = False

                If response IsNot Nothing AndAlso response.ResponseData IsNot Nothing AndAlso response.ResponseData.UserAgencies IsNot Nothing Then
                    hasRecords = True 'new 3/21/2022
                    For Each agency As DCO.Lookup.GetUserAgency In response.ResponseData.UserAgencies
                        If agency.UserAgencyRelationTypeId > 0 Then 'new 3/21/2022
                            hasUserAgencyRelationTypeId = True
                        End If
                        UserAgenciesHT.Add(agency.AgencyId, agency.UserAgencyRelationTypeId)
                        UserAgenciesDT.Rows.Add(agency.AgencyId, agency.UserAgencyRelationTypeId)
                    Next
                End If

                'new 3/21/2022
                If hasRecords = True AndAlso hasUserAgencyRelationTypeId = False Then
                    UserAgenciesHT.Clear()
                    UserAgenciesDT.Clear()

                    Using sso As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
                        With sso
                            .queryOrStoredProc = "SELECT L.agency_id, L.useragencyrelationtype_id"
                            .queryOrStoredProc &= " FROM UserAgencyLink as L WITH (NOLOCK)"
                            .queryOrStoredProc &= " WHERE L.users_id = " & userObject.diamond_UserID
                            .queryOrStoredProc &= " ORDER BY L.useragencyrelationtype_id, L.agency_id"

                            Using dr As Data.SqlClient.SqlDataReader = .GetDataReader
                                If dr IsNot Nothing AndAlso dr.HasRows = True Then
                                    With dr
                                        While dr.Read
                                            UserAgenciesHT.Add(.Item("agency_id"), .Item("useragencyrelationtype_id"))
                                            UserAgenciesDT.Rows.Add(.Item("agency_id"), .Item("useragencyrelationtype_id"))
                                        End While
                                    End With
                                ElseIf .hasError = True Then
                                    'database error
                                    caughtDatabaseError = True
                                Else
                                    'nothing found
                                End If
                            End Using
                        End With
                    End Using
                End If
            Catch ex As Exception
                caughtServiceError = True 'new 3/21/2022
                If Errors IsNot Nothing Then Errors.Add(New ErrorObject(ex.ToString, "Error loading user agencies from Diamond system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Diamond))
            End Try
        ElseIf systemLocated = Enums.UserLocation.Legacy Then
            If userObject.legacy_6000Access = True Then
                Using sql As New SQLselectObject(legacy_Connection)
                    sql.queryOrStoredProc = "sp_Get6000Agencies"
                    sql.parameter = New SqlClient.SqlParameter("@agCode", userObject.legacy_PrimaryAgencyCode)

                    Using dr As SqlClient.SqlDataReader = sql.GetDataReader
                        If Not dr Is Nothing Then
                            If dr.HasRows Then
                                While dr.Read
                                    If userObject.legacy_SecondaryAgencyCodes Is Nothing Then userObject.legacy_SecondaryAgencyCodes = New List(Of Integer)
                                    userObject.legacy_SecondaryAgencyCodes.Add(Int16.Parse(dr.Item("AgencyCode")))
                                End While
                            Else
                                If sql.hasError Then
                                    Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
                                End If
                            End If
                        Else
                            Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
                        End If
                    End Using
                End Using
            End If
            If userObject.legacy_SecondaryAgencyCodes.Count <= 0 Then
                Using sql As New SQLselectObject(legacy_Connection)
                    sql.queryOrStoredProc = "sp_GetAssociateAgencies"
                    sql.parameter = New SqlClient.SqlParameter("@agCode", userObject.legacy_PrimaryAgencyCode)

                    Using dr As SqlClient.SqlDataReader = sql.GetDataReader
                        If Not dr Is Nothing Then
                            If dr.HasRows Then
                                While dr.Read
                                    If userObject.legacy_SecondaryAgencyCodes Is Nothing Then userObject.legacy_SecondaryAgencyCodes = New List(Of Integer)
                                    userObject.legacy_SecondaryAgencyCodes.Add(Int16.Parse(dr.Item("agency_code")))
                                End While
                            Else
                                If sql.hasError Then
                                    Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
                                End If
                            End If
                        Else
                            Errors.Add(New ErrorObject(sql.errorMsg, "Error loading user agencies from Legacy system.", Enums.ErrorLevel.Warning, Enums.UserLocation.Legacy))
                        End If
                    End Using
                End Using
            End If
        End If

        'new 3/21/2022
        If caughtServiceError = True OrElse caughtDatabaseError = True Then
            caughtError = True
        End If
    End Sub

    'Public Sub Changes()
    '    Dim req As New DCS.Messages.ClaimsService.
    'End Sub

    ''' <summary>
    ''' Gets existing user information.  Populates ExistingUser as a WebUser object.
    ''' </summary>
    ''' <param name="systemLocated">Enumeration of the system(s) to retrieve user data from.</param>
    ''' <remarks></remarks>
    Public Sub Lookup_GetExistingUserInfo(ByVal systemLocated As Enums.UserLocation, Optional ByVal login As Boolean = False)
        'Errors.Clear()
        'CheckDatabaseConnections()
        'Try
        '    'added 1/24/2022
        '    Dim prevUsername As String = ""
        '    Dim setMustChangePassword As Boolean = False
        '    Dim prevMustChangePassword As Boolean = False
        '    If ExistingUser IsNot Nothing Then
        '        prevUsername = ExistingUser.diamond_LoginName
        '        prevMustChangePassword = ExistingUser.diamond_MustChangePassword
        '        If UsePasswordMustBeChangedFlagFromDiamondLoginResponseInsteadOfDatabase() = True AndAlso String.IsNullOrWhiteSpace(DiamondUserLogin) = False AndAlso String.IsNullOrWhiteSpace(ExistingUser.diamond_LoginName) = False AndAlso UCase(DiamondUserLogin) = UCase(ExistingUser.diamond_LoginName) Then
        '            setMustChangePassword = True
        '        End If
        '    End If

        '    ExistingUser = New WebUser()
        '    If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.Diamond Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster _
        '        Or systemLocated = Enums.UserLocation.DiamondAndLegacy Then 'check Diamond
        '        If DiamondUserID <= 0 AndAlso Not String.IsNullOrEmpty(DiamondUserLogin) Then
        '            Admin_GetUserID(ExistingUser, DiamondUserLogin, Enums.UserLocation.Diamond, "agency")
        '            If DiamondUserID <= 0 Then Admin_GetUserID(ExistingUser, DiamondUserLogin, Enums.UserLocation.Diamond, "ifm.ifmic")
        '            If DiamondUserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
        '        End If
        '        If DiamondUserID > 0 Then
        '            Dim request As New DCS.Messages.LookupService.GetUser.Request
        '            Dim response As New DCS.Messages.LookupService.GetUser.Response

        '            request.RequestData.UsersId = DiamondUserID

        '            Using proxy As New Proxies.LookupServiceProxy
        '                response = proxy.GetUser(request)
        '            End Using

        '            If response IsNot Nothing AndAlso response.ResponseData.User IsNot Nothing Then
        '                User_LoadUserSecurityQuestionsAnswers()
        '                Lookup_GetUserAgencies(ExistingUser, Enums.UserLocation.Diamond)
        '                'ExistingUser.FillExistingUser(response.ResponseData.User, ListOfDiamondSecurityQuestions, UserAgenciesHT)
        '                'updated 1/24/2022
        '                ExistingUser.FillExistingUser_OptionallySetMustChangePassword(response.ResponseData.User, ListOfDiamondSecurityQuestions, UserAgenciesHT, setMustChangePassword:=setMustChangePassword, mustChangePasswordVal:=prevMustChangePassword)
        '                ExistingUser.userInDiamond = True
        '            Else
        '                ExistingUser.userInDiamond = False
        '            End If
        '        End If
        '    End If

        '    If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndLegacy Or systemLocated = Enums.UserLocation.Legacy _
        '        Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster Then 'check Legacy

        '        If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(LegacyUserLogin) Then
        '            Admin_GetUserID(New WebUser, LegacyUserLogin, Enums.UserLocation.Legacy)
        '            If LegacyUserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in legacy system.", "User does not exist in legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
        '        End If
        '        If LegacyUserID > 0 And login = False Then
        '            Using sqlLookup As New SQLselectObject(legacy_Connection)
        '                sqlLookup.queryOrStoredProc = "sp_GetAgencyUserInfoByID"
        '                sqlLookup.parameter = New SqlClient.SqlParameter("@userID", LegacyUserID)
        '                Using dr As SqlDataReader = sqlLookup.GetDataReader
        '                    If dr IsNot Nothing AndAlso dr.HasRows Then
        '                        SetLegacyData(ExistingUser, dr)
        '                        ExistingUser.userInLegacy = True
        '                    End If
        '                End Using
        '            End Using
        '        ElseIf (Not String.IsNullOrEmpty(LegacyUserLogin) AndAlso Not String.IsNullOrEmpty(LegacyUserPassword)) _
        '            Or (Not String.IsNullOrEmpty(ExistingUser.legacy_Username) AndAlso Not String.IsNullOrEmpty(ExistingUser.legacy_Password)) Then
        '            Using sqlLookup As New SQLselectObject(legacy_Connection)
        '                sqlLookup.queryOrStoredProc = "sp_GetAgencyUserInfo"

        '                sqlLookup.parameters = New ArrayList
        '                sqlLookup.parameters.Add(New SqlClient.SqlParameter("@user", If(Not String.IsNullOrEmpty(LegacyUserLogin), LegacyUserLogin, ExistingUser.legacy_Username)))
        '                sqlLookup.parameters.Add(New SqlClient.SqlParameter("@pass", If(Not String.IsNullOrEmpty(LegacyUserPassword), LegacyUserPassword, ExistingUser.legacy_Password)))
        '                Using dr As SqlDataReader = sqlLookup.GetDataReader
        '                    If dr IsNot Nothing AndAlso dr.HasRows Then
        '                        SetLegacyData(ExistingUser, dr)
        '                        ExistingUser.userInLegacy = True
        '                    End If
        '                End Using
        '            End Using
        '        Else
        '            Errors.Add(New ErrorObject("User ID or login/password do not exist in legacy system.", "User ID or login/password do not exist in legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
        '            ExistingUser.userInLegacy = False
        '        End If
        '    End If

        '    If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster _
        '        Or systemLocated = Enums.UserLocation.Quotemaster Then 'check QM

        '        If UseQuoteMaster() = True Then '7/15/2013 - added IF
        '            CheckQuoteMasterAgency(ExistingUser)
        '        End If
        '    End If

        '    SetSystemFlag(ExistingUser)
        'Catch ex As Exception
        '    Errors.Add(New ErrorObject(ex.ToString, "Error loading existing user information", Enums.ErrorLevel.Severe, systemLocated))
        'End Try
        'updated 1/26/2022 to call new method
        Lookup_GetExistingUserInfo_OptionallyMaintainPasswordMustBeChangedFlag(systemLocated, login:=login)
    End Sub
    'added 1/26/2022
    Public Sub Lookup_GetExistingUserInfo_OptionallyMaintainPasswordMustBeChangedFlag(ByVal systemLocated As Enums.UserLocation, Optional ByVal login As Boolean = False, Optional ByVal maintainPasswordMustBeChangedFlag As CommonHelperClass.YesNoOrMaybe = CommonHelperClass.YesNoOrMaybe.Maybe)
        Errors.Clear()
        CheckDatabaseConnections()
        Try
            'added 1/24/2022
            Dim prevUsername As String = ""
            Dim setMustChangePassword As Boolean = False
            Dim prevMustChangePassword As Boolean = False
            If ExistingUser IsNot Nothing Then
                prevUsername = ExistingUser.diamond_LoginName
                prevMustChangePassword = ExistingUser.diamond_MustChangePassword
                If (maintainPasswordMustBeChangedFlag = CommonHelperClass.YesNoOrMaybe.Yes OrElse (maintainPasswordMustBeChangedFlag = CommonHelperClass.YesNoOrMaybe.Maybe AndAlso UsePasswordMustBeChangedFlagFromDiamondLoginResponseInsteadOfDatabase() = True)) AndAlso String.IsNullOrWhiteSpace(DiamondUserLogin) = False AndAlso String.IsNullOrWhiteSpace(ExistingUser.diamond_LoginName) = False AndAlso UCase(DiamondUserLogin) = UCase(ExistingUser.diamond_LoginName) Then
                    setMustChangePassword = True
                End If
            End If

            ExistingUser = New WebUser()
            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.Diamond Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster _
                Or systemLocated = Enums.UserLocation.DiamondAndLegacy Then 'check Diamond
                If DiamondUserID <= 0 AndAlso Not String.IsNullOrEmpty(DiamondUserLogin) Then
                    Admin_GetUserID(ExistingUser, DiamondUserLogin, Enums.UserLocation.Diamond, "agency")
                    If DiamondUserID <= 0 Then Admin_GetUserID(ExistingUser, DiamondUserLogin, Enums.UserLocation.Diamond, "ifm.ifmic")
                    If DiamondUserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                End If
                If DiamondUserID > 0 Then
                    Dim request As New DCS.Messages.LookupService.GetUser.Request
                    Dim response As New DCS.Messages.LookupService.GetUser.Response

                    request.RequestData.UsersId = DiamondUserID

                    Using proxy As New Proxies.LookupServiceProxy
                        response = proxy.GetUser(request)
                    End Using

                    If response IsNot Nothing AndAlso response.ResponseData.User IsNot Nothing Then
                        User_LoadUserSecurityQuestionsAnswers()
                        'Lookup_GetUserAgencies(ExistingUser, Enums.UserLocation.Diamond)
                        'updated 3/21/2022
                        Dim caughtError As Boolean = False
                        Lookup_GetUserAgencies_WithResults(ExistingUser, Enums.UserLocation.Diamond, caughtError:=caughtError)
                        'ExistingUser.FillExistingUser(response.ResponseData.User, ListOfDiamondSecurityQuestions, UserAgenciesHT)
                        'updated 1/24/2022
                        'ExistingUser.FillExistingUser_OptionallySetMustChangePassword(response.ResponseData.User, ListOfDiamondSecurityQuestions, UserAgenciesHT, setMustChangePassword:=setMustChangePassword, mustChangePasswordVal:=prevMustChangePassword)
                        'updated 3/21/2022
                        ExistingUser.FillExistingUser_OptionallySetMustChangePassword(response.ResponseData.User, ListOfDiamondSecurityQuestions, UserAgenciesHT, setMustChangePassword:=setMustChangePassword, mustChangePasswordVal:=prevMustChangePassword, userAgencyLink_caughtError:=caughtError)
                        ExistingUser.userInDiamond = True
                    Else
                        ExistingUser.userInDiamond = False
                    End If
                End If
            End If

            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndLegacy Or systemLocated = Enums.UserLocation.Legacy _
                Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster Then 'check Legacy

                If LegacyUserID <= 0 AndAlso Not String.IsNullOrEmpty(LegacyUserLogin) Then
                    Admin_GetUserID(New WebUser, LegacyUserLogin, Enums.UserLocation.Legacy)
                    If LegacyUserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in legacy system.", "User does not exist in legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                End If
                If LegacyUserID > 0 And login = False Then
                    Using sqlLookup As New SQLselectObject(legacy_Connection)
                        sqlLookup.queryOrStoredProc = "sp_GetAgencyUserInfoByID"
                        sqlLookup.parameter = New SqlClient.SqlParameter("@userID", LegacyUserID)
                        Using dr As SqlDataReader = sqlLookup.GetDataReader
                            If dr IsNot Nothing AndAlso dr.HasRows Then
                                SetLegacyData(ExistingUser, dr)
                                ExistingUser.userInLegacy = True
                            End If
                        End Using
                    End Using
                ElseIf (Not String.IsNullOrEmpty(LegacyUserLogin) AndAlso Not String.IsNullOrEmpty(LegacyUserPassword)) _
                    Or (Not String.IsNullOrEmpty(ExistingUser.legacy_Username) AndAlso Not String.IsNullOrEmpty(ExistingUser.legacy_Password)) Then
                    Using sqlLookup As New SQLselectObject(legacy_Connection)
                        sqlLookup.queryOrStoredProc = "sp_GetAgencyUserInfo"

                        sqlLookup.parameters = New ArrayList
                        sqlLookup.parameters.Add(New SqlClient.SqlParameter("@user", If(Not String.IsNullOrEmpty(LegacyUserLogin), LegacyUserLogin, ExistingUser.legacy_Username)))
                        sqlLookup.parameters.Add(New SqlClient.SqlParameter("@pass", If(Not String.IsNullOrEmpty(LegacyUserPassword), LegacyUserPassword, ExistingUser.legacy_Password)))
                        Using dr As SqlDataReader = sqlLookup.GetDataReader
                            If dr IsNot Nothing AndAlso dr.HasRows Then
                                SetLegacyData(ExistingUser, dr)
                                ExistingUser.userInLegacy = True
                            End If
                        End Using
                    End Using
                Else
                    Errors.Add(New ErrorObject("User ID or login/password do not exist in legacy system.", "User ID or login/password do not exist in legacy system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Legacy))
                    ExistingUser.userInLegacy = False
                End If
            End If

            If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.DiamondAndQuotemaster Or systemLocated = Enums.UserLocation.LegacyAndQuotemaster _
                Or systemLocated = Enums.UserLocation.Quotemaster Then 'check QM

                If UseQuoteMaster() = True Then '7/15/2013 - added IF
                    CheckQuoteMasterAgency(ExistingUser)
                End If
            End If

            SetSystemFlag(ExistingUser)
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error loading existing user information", Enums.ErrorLevel.Severe, systemLocated))
        End Try
    End Sub

    'added 1/25/2022
    Public Shared Function UsePasswordMustBeChangedFlagFromDiamondLoginResponseInsteadOfDatabase() As Boolean
        Dim isOkay As Boolean = True 'default to True; key required to turn to False

        Dim chc As New CommonHelperClass
        Dim keyExists As Boolean = False
        Dim keyVal As Boolean = chc.ConfigurationAppSettingValueAsBoolean("UsePasswordMustBeChangedFlagFromDiamondLoginResponseInsteadOfDatabase", configurationAppSettingExists:=keyExists)

        If keyExists = True Then
            isOkay = keyVal
        End If

        Return isOkay
    End Function
    'added 3/21/2022
    Public Shared Function UseNewLogicToPopulateDiamondUserAgencyInfo() As Boolean
        Dim isOkay As Boolean = True 'default to True; key required to turn to False

        Dim chc As New CommonHelperClass
        Dim keyExists As Boolean = False
        Dim keyVal As Boolean = chc.ConfigurationAppSettingValueAsBoolean("UseNewLogicToPopulateDiamondUserAgencyInfo", configurationAppSettingExists:=keyExists)

        If keyExists = True Then
            isOkay = keyVal
        End If

        Return isOkay
    End Function
    'added 3/24/2022
    Public Shared Function Admin_ModifyWebUser_ContinueToSecurityQuestionsForStaffOnSaveUserWithNoValidations() As Boolean
        Dim isOkay As Boolean = True 'default to True; key required to turn to False

        Dim chc As New CommonHelperClass
        Dim keyExists As Boolean = False
        Dim keyVal As Boolean = chc.ConfigurationAppSettingValueAsBoolean("DiamondWebClassDiamondSecurity_Admin_ModifyWebUser_ContinueToSecurityQuestionsForStaffOnSaveUserWithNoValidations", configurationAppSettingExists:=keyExists)

        If keyExists = True Then
            isOkay = keyVal
        End If

        Return isOkay
    End Function

    Private Function Lookup_GetPassword(ByVal userID As Integer) As String
        CheckDatabaseConnections()
        Using Sql As New SQLselectObject(diamond_Connection)
            Sql.queryOrStoredProc = "Select password from users where users_id = '" & userID & "'"
            Using dr As SqlDataReader = Sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    dr.Read()
                    Return dr.Item("password").ToString
                Else
                    Return String.Empty
                End If
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Change a user's password in a single system, based on User ID and User Login values.
    ''' </summary>
    ''' <param name="oldPassword">User's old password.</param>
    ''' <param name="newPassword">User's new password.</param>
    ''' <param name="systemLocated">Enumeration of the system where the password will be changed.</param>
    ''' <remarks></remarks>
    Public Sub User_ChangePassword(ByVal oldPassword As String, ByVal newPassword As String, ByVal systemLocated As Enums.UserLocation, Optional ByVal answeredQuestions As Boolean = False)
        Errors.Clear()
        If systemLocated = Enums.UserLocation.Diamond Then
            Dim request As New DCS.Messages.SecurityService.ChangeUserPassword.Request
            Dim response As New DCS.Messages.SecurityService.ChangeUserPassword.Response

            If DiamondUserID <= 0 AndAlso (Not String.IsNullOrEmpty(DiamondUserLogin)) Then
                Admin_GetUserID(New WebUser, DiamondUserLogin, Enums.UserLocation.Diamond, "agency")
                If DiamondUserID <= 0 Then Admin_GetUserID(New WebUser, DiamondUserLogin, Enums.UserLocation.Diamond, "ifm.ifmic")
                If DiamondUserID <= 0 Then Errors.Add(New ErrorObject("User does not exist in Diamond system.", "User does not exist in Diamond system.", Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
            End If

            Try
                request.RequestData.LoginName = DiamondUserLogin
                request.RequestData.UsersId = DiamondUserID
                Dim encoder As New System.Text.ASCIIEncoding()
                Dim md5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
                request.RequestData.NewPassword1 = newPassword 'encoder.GetString(md5Hasher.ComputeHash(encoder.GetBytes(newPassword)))
                request.RequestData.NewPassword2 = newPassword 'encoder.GetString(md5Hasher.ComputeHash(encoder.GetBytes(newPassword)))
                'If answeredQuestions = False Then
                request.RequestData.OldPassword = oldPassword 'encoder.GetString(md5Hasher.ComputeHash(encoder.GetBytes(oldPassword)))
                'Else
                'request.RequestData.OldPassword = Lookup_GetPassword(DiamondUserID)
                'End If

                Using proxy As New Proxies.SecurityServiceProxy
                    response = proxy.ChangeUserPassword(request)
                End Using

                If response IsNot Nothing Then
                    If response.DiamondValidation.HasAnyItems Then
                        For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                            Errors.Add(New ErrorObject(vItem.Message, vItem.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        Next
                    End If
                End If
            Catch ex As Exception
                Errors.Add(New ErrorObject(ex.ToString, "Error updating password in Diamond system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
            End Try
        ElseIf systemLocated = Enums.UserLocation.Legacy Then
            If LegacyUserID >= 0 Then
                Using sqlEx As New SQLexecuteObject(legacy_Connection)
                    sqlEx.queryOrStoredProc = "Update tbl_users set pass = '" & newPassword & "' where user_id = '" & LegacyUserID & "' and pass = '" & oldPassword & "'"
                    sqlEx.ExecuteStatement()
                    If sqlEx.hasError Then Errors.Add(New ErrorObject(sqlEx.errorMsg, "Error updating password in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                End Using
            End If
        ElseIf systemLocated = Enums.UserLocation.Quotemaster Then
            If UseQuoteMaster() = True Then '7/15/2013 - added IF
                If ExistingUser IsNot Nothing AndAlso ExistingUser.QM_agencyID > 0 Then
                    Using sqlEx As New SQLexecuteObject(quotemaster_Connection)
                        'sqlEx.queryOrStoredProc = "Update tbl_users set pass = '" & newPassword & "' where user_id = '" & LegacyUserID & "' and pass = '" & oldPassword & "'"
                        sqlEx.queryOrStoredProc = "Update ProducersTable set password = '" & newPassword & "' where agencyid = '" & ExistingUser.QM_agencyID & "' and ID = '" & LegacyUserLogin & "' and password = '" & oldPassword & "'"
                        sqlEx.ExecuteStatement()
                        If sqlEx.hasError Then Errors.Add(New ErrorObject(sqlEx.errorMsg, "Error updating password in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                    End Using
                Else
                    Errors.Add(New ErrorObject("User object was not passed to change password routine.", "", Enums.ErrorLevel.Programmer, systemLocated))
                End If
            End If
        Else
            Errors.Add(New ErrorObject("Please use a single system enumeration for the change password feature.", "", Enums.ErrorLevel.Programmer, systemLocated))
        End If
    End Sub

    ''' <summary>
    ''' Load a user's security questions from Diamond based on the User ID.  Will be populated within ListOfDiamondSecurityQuestions property.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub User_LoadUserSecurityQuestionsAnswers()
        Errors.Clear()
        Dim request As New DCS.Messages.AdministrationService.LoadUsersUserSecurityQuestionLink.Request
        Dim response As New DCS.Messages.AdministrationService.LoadUsersUserSecurityQuestionLink.Response

        Try
            request.RequestData.UsersId = DiamondUserID
            Using proxy As New Proxies.AdministrationServiceProxy
                response = proxy.LoadUsersUserSecurityQuestionLink(request)
            End Using

            If response IsNot Nothing AndAlso response.ResponseData.UsersUserSecurityQuestionLink IsNot Nothing Then
                ListOfUserSecurityQuestionsAnswers = response.ResponseData.UsersUserSecurityQuestionLink

                ListOfDiamondSecurityQuestions = New List(Of DiamondSecurityQuestion)
                For Each diaSQ As DCO.Administration.UsersUserSecurityQuestionLink In ListOfUserSecurityQuestionsAnswers
                    ListOfDiamondSecurityQuestions.Add(New DiamondSecurityQuestion(diaSQ))
                Next

                If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                    For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                        Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                    Next
                End If
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error loading user security questions.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    ''' <summary>
    ''' Save set of user security questions based on User ID within Diamond.
    ''' </summary>
    ''' <param name="userQuestions">List of DiamondSecurityQuestion objects required for save.</param>
    ''' <remarks></remarks>
    Public Sub User_SaveUserSecurityQuestionsAnswers(ByVal userQuestions As List(Of DiamondSecurityQuestion))
        Errors.Clear()
        Dim request As New DCS.Messages.AdministrationService.SaveUsersUserSecurityQuestionLink.Request
        Dim response As New DCS.Messages.AdministrationService.SaveUsersUserSecurityQuestionLink.Response

        Try
            If DiamondUserID > 0 Then
                ListOfUserSecurityQuestionsAnswers = New DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLink)
                For Each question As DiamondSecurityQuestion In userQuestions
                    CurrentUserSecurityQuestionAnswer = New DCO.Administration.UsersUserSecurityQuestionLink
                    CurrentUserSecurityQuestionAnswer.UsersId = DiamondUserID
                    CurrentUserSecurityQuestionAnswer.UserSecurityQuestionID = question.QuestionID
                    CurrentUserSecurityQuestionAnswer.Answer = question.AnswerText
                    ListOfUserSecurityQuestionsAnswers.Add(CurrentUserSecurityQuestionAnswer)
                    CurrentUserSecurityQuestionAnswer = Nothing
                Next

                request.RequestData.UsersUserSecurityQuestionLink = ListOfUserSecurityQuestionsAnswers
                Using proxy As New Proxies.AdministrationServiceProxy
                    response = proxy.SaveUsersUserSecurityQuestionLink(request)

                    If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                        For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                            Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        Next
                    End If
                End Using
            Else
                Errors.Add(New ErrorObject("Invalid diamondUserID - must be greater than zero.", "", Enums.ErrorLevel.Programmer, Enums.UserLocation.Diamond))
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error saving user security questions.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    ''' <summary>
    ''' Load a user's security questions from Diamond based on the User Login.  Will be populated within ListOfDiamondSecurityQuestions property.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub User_LoadUserSecurityQuestionsForLoginName()
        Errors.Clear()
        Dim request As New DCS.Messages.SecurityService.LoadUsersUserSecurityQuestionLinkForLoginName.Request
        Dim response As New DCS.Messages.SecurityService.LoadUsersUserSecurityQuestionLinkForLoginName.Response

        Try
            If DiamondUserLogin IsNot Nothing AndAlso Not String.IsNullOrEmpty(DiamondUserLogin) Then
                request.RequestData.LoginName = DiamondUserLogin
                Using proxy As New Proxies.SecurityServiceProxy
                    response = proxy.LoadUsersUserSecurityQuestionLinkForLoginName(request)
                End Using

                If response IsNot Nothing AndAlso response.ResponseData.UsersUserSecurityQuestionLinkForLoginName IsNot Nothing Then
                    ListOfUserSecurityQuestionsAnswersForLogin = response.ResponseData.UsersUserSecurityQuestionLinkForLoginName

                    ListOfDiamondSecurityQuestions = New List(Of DiamondSecurityQuestion)
                    For Each diaSQ As DCO.Administration.UsersUserSecurityQuestionLinkForLoginName In ListOfUserSecurityQuestionsAnswersForLogin
                        ListOfDiamondSecurityQuestions.Add(New DiamondSecurityQuestion(diaSQ))
                    Next

                    If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                        For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                            Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        Next
                    End If
                End If
            Else
                Errors.Add(New ErrorObject("Invalid diamondUserLogin property value", "", Enums.ErrorLevel.Programmer, Enums.UserLocation.Diamond))
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error loading user security questions.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    Public Sub getMessages()
        Dim request As New DCS.Messages.PolicyService.GetPolicyMessage.Request
        Dim response As New DCS.Messages.PolicyService.GetPolicyMessage.Response

        request.RequestData.PolicyId = 14341
        request.RequestData.PolicyImageNum = 2

        Using proxy As New Proxies.PolicyServices.PolicyServiceProxy
            response = proxy.GetPolicyMessage(request)
        End Using

        Dim test = "test"
    End Sub

    ''' <summary>
    ''' Validates answers to user security questions within Diamond.  Upon sucessful validation, an email with the user's new password will automatically be sent.
    ''' </summary>
    ''' <param name="questions">List of DiamondSecurityQuestion objects required for validation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function User_VerifyAnswersForLoginName(ByVal questions As List(Of DiamondSecurityQuestion)) As String
        Errors.Clear()
        Dim request As New DCS.Messages.SecurityService.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.Request
        Dim response As New DCS.Messages.SecurityService.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.Response
        User_VerifyAnswersForLoginName = String.Empty

        Try
            If DiamondUserID <= 0 Then
                Errors.Add(New ErrorObject("Invalid diamondUserID - must be greater than zero.", "", Enums.ErrorLevel.Programmer, Enums.UserLocation.Diamond))
            ElseIf DiamondUserLogin Is Nothing Or String.IsNullOrEmpty(DiamondUserLogin) Then
                Errors.Add(New ErrorObject("Invalid diamondUserLogin property value", "", Enums.ErrorLevel.Programmer, Enums.UserLocation.Diamond))
            Else
                ListOfUserSecurityQuestionsAnswersForLogin = New DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLinkForLoginName)
                For Each question As DiamondSecurityQuestion In questions
                    CurrentUserSecurityQuestionAnswerForLogin = New DCO.Administration.UsersUserSecurityQuestionLinkForLoginName
                    With CurrentUserSecurityQuestionAnswerForLogin
                        .UsersId = DiamondUserID
                        .UserSecurityQuestionID = question.QuestionID
                        .Question = question.QuestionText
                        .Answer = question.AnswerText
                    End With
                    ListOfUserSecurityQuestionsAnswersForLogin.Add(CurrentUserSecurityQuestionAnswerForLogin)
                    CurrentUserSecurityQuestionAnswerForLogin = Nothing
                Next

                With request.RequestData
                    .LoginName = DiamondUserLogin
                    .UsersUserSecurityQuestionLinkForLoginName = ListOfUserSecurityQuestionsAnswersForLogin
                    .SuppressEmail = True
                End With

                Using proxy As New Proxies.SecurityServiceProxy
                    response = proxy.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName(request)
                End Using

                If response IsNot Nothing Then
                    If response.ResponseData.Success = True Then
                        User_VerifyAnswersForLoginName = GenerateNewTempPassword(DiamondUserLogin) 'DJG - 2021/04/13 - The original Diamond API call no longer supplies the password upon security question answer verification. Have to generate a temp password to retain the same functionality.
                    End If

                    If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing Then
                        For Each diaValidation As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                            Errors.Add(New ErrorObject(diaValidation.Message, diaValidation.Message, Enums.ErrorLevel.Validation, Enums.UserLocation.Diamond))
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            If ex.ToString.Contains("The specified string is not in the form required for an e-mail address.") Then
                Errors.Add(New ErrorObject("The specified string is not in the form required for an e-mail address.", "Invalid e-mail address stored for this user.  E-mail cannot be sent.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
            ElseIf ex.ToString.Contains("The remote name could not be resolved") Then
                Errors.Add(New ErrorObject("Failure sending mail. The mail server name could not be resolved.", "Error while attempting to send e-mail.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
            Else
                Errors.Add(New ErrorObject(ex.ToString, "Error while attempting to validate user security questions and answers.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
            End If
        End Try
    End Function

    Private Function GenerateNewTempPassword(DiamondUserLogin As String) As String
        If String.IsNullOrWhiteSpace(DiamondUserLogin) = False Then
            Dim request As New DCS.Messages.SecurityService.GenerateTemporaryPassword.Request
            Dim response As New DCS.Messages.SecurityService.GenerateTemporaryPassword.Response
            request.RequestData.LoginName = DiamondUserLogin
            Using proxy As New Proxies.SecurityServiceProxy
                response = proxy.GenerateTemporaryPassword(request)
            End Using
            If response IsNot Nothing Then
                If String.IsNullOrWhiteSpace(response.ResponseData.Password) = False Then
                    Return response.ResponseData.Password
                End If
            End If
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Sets the system located flag based on boolean properties set earlier
    ''' </summary>
    ''' <param name="currUser">WebUser object to set the property within</param>
    ''' <remarks></remarks>
    Private Sub SetSystemFlag(ByRef currUser As WebUser)
        With currUser
            If .userInDiamond = True Then
                If .userInLegacy = True Then
                    If .userInQM = True Then
                        .SystemsLocated = Enums.UserLocation.AllSystems
                    Else
                        .SystemsLocated = Enums.UserLocation.DiamondAndLegacy
                    End If
                ElseIf .userInQM = True Then
                    .SystemsLocated = Enums.UserLocation.DiamondAndQuotemaster
                Else
                    .SystemsLocated = Enums.UserLocation.Diamond
                End If
            ElseIf .userInLegacy = True Then
                If .userInQM = True Then
                    .SystemsLocated = Enums.UserLocation.LegacyAndQuotemaster
                Else
                    .SystemsLocated = Enums.UserLocation.Legacy
                End If
            ElseIf .userInQM = True Then
                .SystemsLocated = Enums.UserLocation.Quotemaster
            Else
                .SystemsLocated = Enums.UserLocation.None
            End If
        End With
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

    ''' <summary>
    ''' Set base legacy information from data reader
    ''' </summary>
    ''' <param name="exUser">WebUser object to set properties within</param>
    ''' <param name="dr">SqlDataReader object to grab value from</param>
    ''' <remarks></remarks>
    Private Sub SetLegacyData(ByRef exUser As WebUser, ByVal dr As SqlDataReader)
        If dr IsNot Nothing AndAlso dr.HasRows Then
            'added 7/16/2013 to verify if linked columns are present (fyi... if the same column name is present multiple times, the reader will return the 1st one in the result set... condition existed for FirstName, LastName when dr was being populated for linked user)
            Dim columnNames As New List(Of String)
            For i As Integer = 0 To dr.FieldCount - 1
                columnNames.Add(dr.GetName(i).ToString)
            Next
            dr.Read()
            With exUser
                If columnNames.Contains("LinkFirst") = True AndAlso columnNames.Contains("LinkMiddle") = True AndAlso columnNames.Contains("LinkLast") = True AndAlso columnNames.Contains("LinkAuthType") = True Then
                    .legacy_FirstName = GetReaderValue(dr, "LinkFirst", Type.GetType("System.String", True, True))
                    .legacy_MiddleName = GetReaderValue(dr, "LinkMiddle", Type.GetType("System.String", True, True))
                    .legacy_LastName = GetReaderValue(dr, "LinkLast", Type.GetType("System.String", True, True))
                    .legacy_AuthType = GetReaderValue(dr, "LinkAuthType", Type.GetType("System.String", True, True))
                Else
                    .legacy_FirstName = GetReaderValue(dr, "FirstName", Type.GetType("System.String", True, True))
                    .legacy_MiddleName = GetReaderValue(dr, "MiddleName", Type.GetType("System.String", True, True))
                    .legacy_LastName = GetReaderValue(dr, "LastName", Type.GetType("System.String", True, True))
                    .legacy_AuthType = GetReaderValue(dr, "auth_type", Type.GetType("System.String", True, True))
                End If

                .legacy_UserID = GetReaderValue(dr, "user_id", Type.GetType("System.Int32", True, True))
                .legacy_Role = GetReaderValue(dr, "role", Type.GetType("System.String", True, True))
                '.legacy_FirstName = GetReaderValue(dr, "FirstName", Type.GetType("System.String", True, True))'commented 7/16/2013; being done above
                '.legacy_MiddleName = GetReaderValue(dr, "MiddleName", Type.GetType("System.String", True, True))'commented 7/16/2013; being done above
                '.legacy_LastName = GetReaderValue(dr, "LastName", Type.GetType("System.String", True, True))'commented 7/16/2013; being done above
                .legacy_Username = GetReaderValue(dr, "username", Type.GetType("System.String", True, True))
                .legacy_Password = GetReaderValue(dr, "pass", Type.GetType("System.String", True, True))
                '.legacy_AuthType = GetReaderValue(dr, "auth_type", Type.GetType("System.String", True, True))'commented 7/16/2013; being done above
                .legacy_AgencyID = GetReaderValue(dr, "agency_id", Type.GetType("System.Int32", True, True))
                .legacy_Hint = GetReaderValue(dr, "hint", Type.GetType("System.String", True, True))
                .legacy_6000Access = GetReaderValue(dr, "Access6000", Type.GetType("System.Boolean", True, True))
                .legacy_ConvertedToDiamond = GetReaderValue(dr, "updatedfordiamond", Type.GetType("System.Boolean", True, True))
                '.legacy_EmailAddress = GetReaderValue(dr, "EmailAddress", Type.GetType("System.String", True, True))
                .legacy_PrimaryAgencyCode = GetReaderValue(dr, "agency_code", Type.GetType("System.String", True, True))
                .legacy_Territory = GetReaderValue(dr, "territory", Type.GetType("System.String", True, True))
                .legacy_CompletedSurvey = GetReaderValue(dr, "completedSurvey", Type.GetType("System.Boolean", True, True))
                .legacy_AgencyName = GetReaderValue(dr, "agency_name", Type.GetType("System.String", True, True))

                LegacyUserID = .legacy_UserID
                LegacyUserLogin = .legacy_Username
            End With
        End If
    End Sub

    ''' <summary>
    ''' Insert legacy to Diamond link record.
    ''' </summary>
    ''' <param name="userObject">WebUser object to set properties within</param>
    ''' <remarks>Will added to Errors list if SQL query catches error.</remarks>
    Private Sub InsertLink(ByVal userObject As WebUser)
        CheckDatabaseConnections()
        Using sqlEx As New SQLexecuteObject(legacy_Connection)
            'sqlEx.queryOrStoredProc = "Insert into tbl_Diamond_Users (DiamondID, LegacyID, FirstName, MiddleName, LastName, AuthType) values ('" & userObject.diamond_UserID & "', '" & _
            '    userObject.legacy_UserID & "', '" & userObject.legacy_FirstName & "', '" & userObject.legacy_MiddleName & "', '" & userObject.legacy_LastName & "', '" & userObject.legacy_AuthType & "')"
            'updated 3/24/2022
            Dim firstName As String = ""
            Dim middleName As String = ""
            Dim lastName As String = ""
            PrepNameFieldsForDatabaseInsertUpdate(userObject, firstName:=firstName, middleName:=middleName, lastName:=lastName)
            sqlEx.queryOrStoredProc = "Insert into tbl_Diamond_Users (DiamondID, LegacyID, FirstName, MiddleName, LastName, AuthType) values ('" & userObject.diamond_UserID & "', '" &
                userObject.legacy_UserID & "', '" & firstName & "', '" & middleName & "', '" & lastName & "', '" & userObject.legacy_AuthType & "')"
            sqlEx.ExecuteStatement()
            If sqlEx.hasError = True Then Errors.Add(New ErrorObject(sqlEx.errorMsg, "Error adding Legacy to Diamond link record.", Enums.ErrorLevel.Programmer, Enums.UserLocation.DiamondAndLegacy))
        End Using
    End Sub

    Private Sub UpdateLink(ByVal userObject As WebUser)
        CheckDatabaseConnections()
        Using sqlEx As New SQLexecuteObject(legacy_Connection)
            'sqlEx.queryOrStoredProc = "Update tbl_Diamond_Users set FirstName = '" & userObject.legacy_FirstName & "', MiddleName = '" & userObject.legacy_MiddleName & "', LastName = '" & userObject.legacy_LastName & "', AuthType = '" & userObject.legacy_AuthType & "' where DiamondID = '" & userObject.diamond_UserID & "' and LegacyID = '" & userObject.legacy_UserID & "'"
            'updated 3/24/2022
            Dim firstName As String = ""
            Dim middleName As String = ""
            Dim lastName As String = ""
            PrepNameFieldsForDatabaseInsertUpdate(userObject, firstName:=firstName, middleName:=middleName, lastName:=lastName)
            sqlEx.queryOrStoredProc = "Update tbl_Diamond_Users set FirstName = '" & firstName & "', MiddleName = '" & middleName & "', LastName = '" & lastName & "', AuthType = '" & userObject.legacy_AuthType & "' where DiamondID = '" & userObject.diamond_UserID & "' and LegacyID = '" & userObject.legacy_UserID & "'"
            sqlEx.ExecuteStatement()
            If sqlEx.hasError = True Then Errors.Add(New ErrorObject(sqlEx.errorMsg, "Error updating Legacy to Diamond link record.", Enums.ErrorLevel.Programmer, Enums.UserLocation.DiamondAndLegacy))
        End Using
    End Sub
    'added 3/24/2022
    Private Sub PrepNameFieldsForDatabaseInsertUpdate(ByVal userObject As WebUser, ByRef firstName As String, ByRef middleName As String, ByRef lastName As String)
        firstName = ""
        middleName = ""
        lastName = ""
        If userObject IsNot Nothing Then
            With userObject
                firstName = .legacy_FirstName
                middleName = .legacy_MiddleName
                lastName = .legacy_LastName
                If String.IsNullOrWhiteSpace(firstName) = False AndAlso firstName.Contains("'") = True Then
                    firstName = Replace(firstName, "'", "''")
                End If
                If String.IsNullOrWhiteSpace(middleName) = False AndAlso middleName.Contains("'") = True Then
                    middleName = Replace(middleName, "'", "''")
                End If
                If String.IsNullOrWhiteSpace(lastName) = False AndAlso lastName.Contains("'") = True Then
                    lastName = Replace(lastName, "'", "''")
                End If
            End With
        End If
    End Sub

    Private Sub DeleteLink(ByVal userObject As WebUser, ByVal systemLocated As Enums.UserLocation)
        CheckDatabaseConnections()
        Using SqlEx As New SQLexecuteObject(legacy_Connection)
            If systemLocated = Enums.UserLocation.Diamond Then
                SqlEx.queryOrStoredProc = "Delete from tbl_Diamond_Users where DiamondID = '" & userObject.diamond_UserID & "'"
            Else
                SqlEx.queryOrStoredProc = "Delete from tbl_Diamond_Users where LegacyID = '" & userObject.legacy_UserID & "'"
            End If
            SqlEx.ExecuteStatement()
            If SqlEx.hasError = True Then Errors.Add(New ErrorObject(SqlEx.errorMsg, "Error deleting Legacy to Diamond link record.", Enums.ErrorLevel.Programmer, systemLocated))
        End Using
    End Sub

    Public Function linkExists(ByVal legacyUserID As Integer) As Boolean
        CheckDatabaseConnections()
        Using Sql As New SQLselectObject(legacy_Connection)
            Sql.queryOrStoredProc = "Select * from tbl_Diamond_Users where legacyid = '" & legacyUserID & "'"
            Using dr As SqlDataReader = Sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    Return True
                Else
                    Return False
                End If
            End Using
        End Using
    End Function

    Public Sub fillLinkData(ByRef userObject As WebUser)
        CheckDatabaseConnections()
        Using Sql As New SQLselectObject(legacy_Connection)
            If userObject.diamond_UserID > 0 Then
                'Sql.queryOrStoredProc = "select * from tbl_Diamond_Users inner join tbl_users on tbl_users.user_id = legacyid inner join tbl_agency on tbl_users.agency_id = tbl_agency.agency_id where diamondID = '" & userObject.diamond_UserID & "'"
                'updated 7/16/2013 to specifically pull out link table fields
                Sql.queryOrStoredProc = "select tbl_diamond_users.FirstName as LinkFirst, tbl_diamond_users.MiddleName as LinkMiddle, tbl_diamond_users.LastName as LinkLast, tbl_diamond_users.authtype as LinkAuthType, * from tbl_Diamond_Users inner join tbl_users on tbl_users.user_id = legacyid inner join tbl_agency on tbl_users.agency_id = tbl_agency.agency_id where diamondID = '" & userObject.diamond_UserID & "'"
                Using dr As SqlDataReader = Sql.GetDataReader
                    If dr IsNot Nothing AndAlso dr.HasRows Then
                        'dr.Read()
                        SetLegacyData(userObject, dr)
                    End If
                End Using
            Else

            End If
        End Using
    End Sub

    ''' <summary>
    ''' Grab a specific type of data from SqlDataReader object
    ''' </summary>
    ''' <param name="dr">SqlDataReader to be used for obtaining information</param>
    ''' <param name="itemName">Specific item within the SqlDataReader holding value</param>
    ''' <param name="returnType">System.Type value to be returned</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetReaderValue(ByVal dr As SqlDataReader, ByVal itemName As String, ByVal returnType As System.Type) As Object
        If dr.Item(itemName) IsNot Nothing Then
            Try
                Select Case returnType.Name.ToLower
                    Case Is = "boolean"
                        Try
                            Return Boolean.Parse(dr.Item(itemName))
                        Catch ex As Exception
                            If dr.Item(itemName).ToString.Substring(0, 1) = "Y" Then
                                Return True
                            Else
                                Return False
                            End If
                        End Try
                    Case Is = "byte"
                        Byte.Parse(dr.Item(itemName))
                    Case Is = "char"
                        Char.Parse(dr.Item(itemName))
                    Case Is = "datetime"
                        DateTime.Parse(dr.Item(itemName))
                    Case Is = "decimal"
                        Decimal.Parse(dr.Item(itemName))
                    Case Is = "double"
                        Double.Parse(dr.Item(itemName))
                    Case Is = "int16"
                        Try
                            Return Int16.Parse(dr.Item(itemName))
                        Catch ex As Exception
                            Return 0
                        End Try
                    Case Is = "int32"
                        Try
                            Return Int32.Parse(dr.Item(itemName))
                        Catch ex As Exception
                            Return 0
                        End Try
                    Case Is = "int64"
                        Try
                            Return Int64.Parse(dr.Item(itemName))
                        Catch ex As Exception
                            Return 0
                        End Try
                    Case Is = "sbyte"
                        SByte.Parse(dr.Item(itemName))
                    Case Is = "single"
                        Single.Parse(dr.Item(itemName))
                    Case Is = "string"
                        Return Convert.ToString(dr.Item(itemName))
                End Select
            Catch
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Before deleting a legacy user, check for pending EFT payment.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function HasLegacyEFTPayment() As Boolean
        Using sql As New SQLselectObject(legacy_Connection)
            sql.queryOrStoredProc = "sp_GetAgencyUserEFT"
            sql.parameter = New SqlClient.SqlParameter("@user_id", LegacyUserID)

            Using dr As SqlClient.SqlDataReader = sql.GetDataReader
                If Not dr Is Nothing Then
                    If dr.HasRows Then
                        Return True
                    Else
                        If sql.hasError = True Then
                            Errors.Add(New ErrorObject(sql.errorMsg, "Error checking for existing EFT payment in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                        Else
                            Return False
                        End If
                    End If
                Else
                    Errors.Add(New ErrorObject(sql.errorMsg, "Error checking for existing EFT payment in Legacy system.", Enums.ErrorLevel.Severe, Enums.UserLocation.Legacy))
                End If
            End Using
        End Using
    End Function

    ''' <summary>
    ''' String validation
    ''' </summary>
    ''' <param name="strToValidate">String to validate</param>
    ''' <param name="vType">Type of validation to perform.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsValid(ByVal strToValidate As String, ByVal vType As Enums.Validation) As Boolean
        Select Case vType
            Case Is = Enums.Validation.Username
                If strToValidate = "" Then
                    Errors.Add(New ErrorObject("Username is missing.", "Username is missing.", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf Len(strToValidate) < 3 Then
                    Errors.Add(New ErrorObject("UserName must be at least 3 characters", "UserName must be at least 3 characters", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf Len(strToValidate) > 20 Then
                    Errors.Add(New ErrorObject("UserName cannot exceed 20 characters", "UserName cannot exceed 20 characters", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf strToValidate.Contains("&") = True Then
                    Errors.Add(New ErrorObject("UserName cannot contain '&'", "UserName cannot contain '&'", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf strToValidate.Contains("#") = True Then
                    Errors.Add(New ErrorObject("UserName cannot contain '#'", "UserName cannot contain '#'", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                Else
                    Return True
                End If
            Case Is = Enums.Validation.Password
                If strToValidate = "" Then
                    Errors.Add(New ErrorObject("Password is missing.", "Password is missing.", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf Len(strToValidate) < 8 Then
                    Errors.Add(New ErrorObject("Password must be at least 8 characters", "Password must be at least 8 characters", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf Len(strToValidate) > 20 Then
                    Errors.Add(New ErrorObject("Password cannot exceed 20 characters", "Password cannot exceed 20 characters", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf strToValidate.Contains("&") = True Then
                    Errors.Add(New ErrorObject("Password cannot contain '&'", "Password cannot contain '&'", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf strToValidate.Contains("#") = True Then
                    Errors.Add(New ErrorObject("Password cannot contain '#'", "Password cannot contain '#'", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf MeetsCriteria(Enums.Criteria.LowercaseLetter, strToValidate, 1) = False Then
                    Errors.Add(New ErrorObject("Password must contain at least one lowercase letter", "Password must contain at least one lowercase letter", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf MeetsCriteria(Enums.Criteria.UppercaseLetter, strToValidate, 1) = False Then
                    Errors.Add(New ErrorObject("Password must contain at least one uppercase letter", "Password must contain at least one uppercase letter", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                ElseIf MeetsCriteria(Enums.Criteria.Number, strToValidate, 1) = False Then
                    Errors.Add(New ErrorObject("Password must contain at least one number", "Password must contain at least one number", Enums.ErrorLevel.Validation, Enums.UserLocation.AllSystems))
                    Return False
                Else
                    Return True
                End If
        End Select
    End Function

    ''' <summary>
    ''' Check string to validate certain criteria (lowercase, uppercase, number)
    ''' </summary>
    ''' <param name="validationType">Type of validation criteria.</param>
    ''' <param name="evalWord">String to validate.</param>
    ''' <param name="numberOfOccurrences">Number of occurrences to check for specific criteria/</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MeetsCriteria(ByVal validationType As Enums.Criteria, ByVal evalWord As String, ByVal numberOfOccurrences As Integer) As Boolean
        Dim holdCount As Integer = 0

        If numberOfOccurrences < 1 Then
            Return True
        ElseIf evalWord = "" Then
            Return False
        Else
            For Each chr As Char In evalWord
                Select Case validationType
                    Case Enums.Criteria.LowercaseLetter
                        If Char.IsLower(chr) = True Then
                            holdCount += 1
                            If holdCount >= numberOfOccurrences Then
                                Return True
                            End If
                        End If
                    Case Enums.Criteria.UppercaseLetter
                        If Char.IsUpper(chr) = True Then
                            holdCount += 1
                            If holdCount >= numberOfOccurrences Then
                                Return True
                            End If
                        End If
                    Case Enums.Criteria.Number
                        If Char.IsNumber(chr) = True Then
                            holdCount += 1
                            If holdCount >= numberOfOccurrences Then
                                Return True
                            End If
                        End If
                End Select
            Next
        End If

        Return False
    End Function

    Public Sub Admin_DownloadOnDemand()
        Errors.Clear()
        Try
            Dim request As New DCS.Messages.AgencyAdministrationService.BeginDownloadAgencyListProcess.Request
            Dim response As New DCS.Messages.AgencyAdministrationService.BeginDownloadAgencyListProcess.Response

            Dim systemData As DCSDM.SystemData = DUSDM.SystemDataManager.SystemData
            Dim submitData As DCSDM.SubmitData = DUSDM.SubmitDataManager.SubmitData
            Dim versionData As DCSDM.VersionData = DUSDM.VersionDataManager.VersionData(1, 16, 1, DUU.SystemDate.GetSystemDate, DUU.SystemDate.GetSystemDate)

            With request.RequestData
                Dim agDownload As New DCO.Policy.Agency.Agency
                agDownload.AgencyId = 14
                .Agencies.Add(agDownload)
                .DownloadStatusId = 0
                .FromDate = #1/1/2009#
                .InForceOnly = False
                .ToDate = #1/1/2011#
            End With

            Using proxy As New Proxies.AgencyAdministrationServiceProxy
                response = proxy.BeginDownloadAgencyListProcess(request)
            End Using

            If response IsNot Nothing Then
                'success
            End If
        Catch ex As Exception
            Errors.Add(New ErrorObject(ex.ToString, "Error adding policy to download routine.", Enums.ErrorLevel.Severe, Enums.UserLocation.Diamond))
        End Try
    End Sub

    Public Sub Admin_ModifyAgencyDetails(ByVal agencyObject As Agency, ByVal systemLocated As Enums.UserLocation)
        If systemLocated = Enums.UserLocation.AllSystems Or systemLocated = Enums.UserLocation.Diamond Or systemLocated = Enums.UserLocation.DiamondAndLegacy Or _
            systemLocated = Enums.UserLocation.DiamondAndQuotemaster Then

            Try
                Dim request As New DCS.Messages.AgencyAdministrationService.SaveAgencyData.Request
                Dim response As New DCS.Messages.AgencyAdministrationService.SaveAgencyData.Response

                With request.RequestData.AgencyInfo
                    .SetIsNewValue(False)
                    With .Agency
                        .AgencyId = 0
                        .Code = 0
                        .Phones.Clear()
                        .Phones.Add(New DCO.Phone)
                        .Emails.Clear()
                        .Emails.Add(New DCO.Email)
                        With .Address
                            'address
                        End With
                        With .Name

                        End With
                        .NameAddressSourceId = Diamond.Common.Enums.NameAddressSource.Agency

                    End With
                End With
            Catch ex As Exception

            End Try
        End If
        Try

        Catch ex As Exception

        End Try
    End Sub

    Private Function GetTestToken() As Diamond.Common.Services.DiamondSecurityToken
        Dim reqLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Request
        Dim rspLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Response

        If _TestTokenUserName = "" Then
            'updated 3/13/2013 to not send new credentials to Diamond; should only use test token stuff for non-upgraded admin users who can't get into portal anyway
            If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername").ToString <> "" AndAlso System.Web.HttpContext.Current.Session("DiamondPassword") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> "" Then
                _TestTokenUserName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
                _TestTokenPassword = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
            Else
                If AppSettings("TestDiamondTokenUserName") IsNot Nothing AndAlso AppSettings("TestDiamondTokenUserName") <> "" Then
                    _TestTokenUserName = AppSettings("TestDiamondTokenUserName")
                End If
                If AppSettings("TestDiamondTokenPassword") IsNot Nothing AndAlso AppSettings("TestDiamondTokenPassword") <> "" Then
                    _TestTokenPassword = AppSettings("TestDiamondTokenPassword")
                End If
            End If
        End If

        With reqLogin.RequestData
            .LoginName = _TestTokenUserName
            .Password = _TestTokenPassword
        End With

        Using loginProxy As New Diamond.Common.Services.Proxies.LoginServiceProxy
            rspLogin = loginProxy.GetDiamTokenForUsernamePassword(reqLogin)
        End Using

        If rspLogin.ResponseData IsNot Nothing Then
            Return rspLogin.ResponseData.DiamondSecurityToken
        Else
            Return Nothing
        End If

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

    Public Property ListOfAllSecurityQuestions() As DCO.InsCollection(Of DCO.Administration.UserSecurityQuestion)
        Get
            Return _ListOfAllSecurityQuestions
        End Get
        Set(ByVal value As DCO.InsCollection(Of DCO.Administration.UserSecurityQuestion))
            _ListOfAllSecurityQuestions = value
        End Set
    End Property

    Public Property CurrentSecurityQuestion() As DCO.Administration.UserSecurityQuestion
        Get
            Return _CurrentSecurityQuestion
        End Get
        Set(ByVal value As DCO.Administration.UserSecurityQuestion)
            _CurrentSecurityQuestion = value
        End Set
    End Property

    Public Property ListOfUserSecurityQuestionsAnswers() As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLink)
        Get
            Return _ListOfUserSecurityQuestionsAnswers
        End Get
        Set(ByVal value As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLink))
            _ListOfUserSecurityQuestionsAnswers = value
        End Set
    End Property

    Public Property ListOfUserSecurityQuestionsAnswersForLogin() As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLinkForLoginName)
        Get
            Return _ListOfUserSecurityQuestionsAnswersForLogin
        End Get
        Set(ByVal value As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLinkForLoginName))
            _ListOfUserSecurityQuestionsAnswersForLogin = value
        End Set
    End Property

    Public Property CurrentUserSecurityQuestionAnswer() As DCO.Administration.UsersUserSecurityQuestionLink
        Get
            Return _CurrentUserSecurityQuestionAnswer
        End Get
        Set(ByVal value As DCO.Administration.UsersUserSecurityQuestionLink)
            _CurrentUserSecurityQuestionAnswer = value
        End Set
    End Property

    Public Property CurrentUserSecurityQuestionAnswerForLogin() As DCO.Administration.UsersUserSecurityQuestionLinkForLoginName
        Get
            Return _CurrentUserSecurityQuestionAnswerForLogin
        End Get
        Set(ByVal value As DCO.Administration.UsersUserSecurityQuestionLinkForLoginName)
            _CurrentUserSecurityQuestionAnswerForLogin = value
        End Set
    End Property

    Public Property Errors() As List(Of ErrorObject)
        Get
            Return _Errors
        End Get
        Set(ByVal value As List(Of ErrorObject))
            _Errors = value
        End Set
    End Property

    Public Property DiamondUserID() As Integer
        Get
            Return _DiamondUserID
        End Get
        Set(ByVal value As Integer)
            _DiamondUserID = value
        End Set
    End Property

    Public Property DiamondUserLogin() As String
        Get
            Return _DiamondUserLogin
        End Get
        Set(ByVal value As String)
            _DiamondUserLogin = value
        End Set
    End Property

    Public Property LegacyUserID() As Integer
        Get
            Return _LegacyUserID
        End Get
        Set(ByVal value As Integer)
            _LegacyUserID = value
        End Set
    End Property

    Public Property LegacyUserLogin() As String
        Get
            Return _LegacyUserLogin
        End Get
        Set(ByVal value As String)
            _LegacyUserLogin = value
        End Set
    End Property

    Public Property LegacyUserPassword() As String
        Get
            Return _LegacyUserPassword
        End Get
        Set(ByVal value As String)
            _LegacyUserPassword = value
        End Set
    End Property


    Public Property ListOfDiamondSecurityQuestions() As List(Of DiamondSecurityQuestion)
        Get
            Return _ListOfDiamondSecurityQuestions
        End Get
        Set(ByVal value As List(Of DiamondSecurityQuestion))
            _ListOfDiamondSecurityQuestions = value
        End Set
    End Property

    Public Property ExistingUser() As WebUser
        Get
            Return _ExistingUser
        End Get
        Set(ByVal value As WebUser)
            _ExistingUser = value
        End Set
    End Property

    Public Property UserAgenciesHT() As Hashtable
        Get
            Return _UserAgenciesHT
        End Get
        Set(ByVal value As Hashtable)
            _UserAgenciesHT = value
        End Set
    End Property

    Public Property UserAgenciesDT() As DataTable
        Get
            Return _UserAgenciesDT
        End Get
        Set(ByVal value As DataTable)
            _UserAgenciesDT = value
        End Set
    End Property

    Public WriteOnly Property UseTestToken(Optional ByVal userName As String = "", Optional ByVal passWord As String = "") As Boolean
        Set(ByVal value As Boolean)
            _UseTestToken = value

            _TestTokenUserName = userName
            _TestTokenPassword = passWord
        End Set
    End Property




#End Region

#Region "Dispose"
    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If ListOfAllSecurityQuestions IsNot Nothing Then ListOfAllSecurityQuestions = Nothing
                If CurrentSecurityQuestion IsNot Nothing Then CurrentSecurityQuestion = Nothing
                If ListOfUserSecurityQuestionsAnswers IsNot Nothing Then ListOfUserSecurityQuestionsAnswers = Nothing
                If ListOfUserSecurityQuestionsAnswersForLogin IsNot Nothing Then ListOfUserSecurityQuestionsAnswersForLogin = Nothing
                If CurrentUserSecurityQuestionAnswer IsNot Nothing Then CurrentUserSecurityQuestionAnswer = Nothing
                If CurrentUserSecurityQuestionAnswerForLogin IsNot Nothing Then CurrentUserSecurityQuestionAnswerForLogin = Nothing
                If ListOfDiamondSecurityQuestions IsNot Nothing Then ListOfDiamondSecurityQuestions = Nothing
                If Errors IsNot Nothing Then Errors = Nothing
                If ExistingUser IsNot Nothing Then ExistingUser = Nothing
                If DiamondUserID > 0 Then DiamondUserID = 0
                If DiamondUserLogin IsNot Nothing Then DiamondUserLogin = Nothing
            End If
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

#End Region


    Public Sub Testing()
        Dim saveReq As New DCS.Messages.PolicyService.SaveClient.Request

        With saveReq.RequestData
            .PolicyAction = Diamond.Common.Enums.PolicyAction.Change

        End With
    End Sub

    Public Function UseQuoteMaster() As Boolean '7/15/2013
        If AppSettings("UseQuoteMaster") IsNot Nothing AndAlso AppSettings("UseQuoteMaster").ToString <> "" AndAlso UCase(AppSettings("UseQuoteMaster").ToString) = "YES" Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
