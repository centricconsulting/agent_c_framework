Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Login
    Public Module Login
        Public Function AuthenticateWithEncryptedToken(EncryptedToken As String,
                                                       IncludeFailuresInResponse As Boolean,
                                                       InsuresoftEncryption As Boolean,
                                                       Optional ByRef e As System.Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCS.DiamondSecurityToken
            Dim res As New DCSM.LoginService.AuthenticateWithEncryptedToken.Response
            Dim req As New DCSM.LoginService.AuthenticateWithEncryptedToken.Request

            With req.RequestData
                .EncryptedToken = EncryptedToken
                .IncludeFailuresInResponse = IncludeFailuresInResponse
                .InsuresoftEncryption = InsuresoftEncryption
            End With

            If IFMS.Login.AuthenticateWithEncryptedToken(res, req, e, dv) Then
                Return res.ResponseData.SecurityToken
            End If
            Return Nothing
        End Function

        Public Function GetDiamTokenForDomainUsername(BusinessInterfaceSourceId As DCE.Security.BusinessInterfaceSourceType,
                                                      LoginDomain As String,
                                                      LoginName As String,
                                                      Optional ByRef e As System.Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCS.DiamondSecurityToken
            Dim res As New DCSM.LoginService.GetDiamTokenForDomainUsername.Response
            Dim req As New DCSM.LoginService.GetDiamTokenForDomainUsername.Request

            With req.RequestData
                .BusinessInterfaceSourceId = BusinessInterfaceSourceId
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If IFMS.Login.GetDiamTokenForDomainUsername(res, req, e, dv) Then
                Return res.ResponseData.diamondSecurityToken
            End If
            Return Nothing
        End Function

        Public Function GetDiamTokenForThirdPartyToken(BusinessInterfaceSourceId As DCE.Security.BusinessInterfaceSourceType,
                                                       NetworkOrigin As String,
                                                       TokenArray As String(),
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCS.DiamondSecurityToken
            Dim res As New DCSM.LoginService.GetDiamTokenForThirdPartyToken.Response
            Dim req As New DCSM.LoginService.GetDiamTokenForThirdPartyToken.Request

            With req.RequestData
                .BusinessInterfaceSourceId = BusinessInterfaceSourceId
                .NetworkOrigin = NetworkOrigin
                .TokenArray = TokenArray
            End With

            If IFMS.Login.GetDiamTokenForThirdPartyToken(res, req, e, dv) Then
                Return res.ResponseData.diamondSecurityToken
            End If
            Return Nothing
        End Function

        Public Function GetDiamTokenForUsernamePassword(LoginName As String,
                                                Password As String,
                                                Optional e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCS.DiamondSecurityToken
            Dim req As New DCSM.LoginService.GetDiamTokenForUsernamePassword.Request
            Dim res As New DCSM.LoginService.GetDiamTokenForUsernamePassword.Response

            With req.RequestData
                .LoginName = LoginName
                .Password = Password
            End With

            If IFMS.Login.GetDiamTokenForUsernamePassword(res, req, e, dv) Then
                If res IsNot Nothing Then
                    Return res.ResponseData.DiamondSecurityToken
                End If
            End If
            Return Nothing

        End Function

        Public Function GetDiamTokenForUsernamePassword(BusinessInterfaceSourceid As DCE.Security.BusinessInterfaceSourceType,
                                                        LoginName As String,
                                                        Password As String,
                                                        Optional e As Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCS.DiamondSecurityToken
            Dim req As New DCSM.LoginService.GetDiamTokenForUsernamePassword.Request
            Dim res As New DCSM.LoginService.GetDiamTokenForUsernamePassword.Response

            With req.RequestData
                .BusinessInterfaceSourceId = BusinessInterfaceSourceid
                .LoginName = LoginName
                .Password = Password
            End With

            If IFMS.Login.GetDiamTokenForUsernamePassword(res, req, e, dv) Then
                If res IsNot Nothing Then
                    Return res.ResponseData.DiamondSecurityToken
                End If
            End If
            Return Nothing

        End Function

        Public Function GetEncryptedUsernameToken(BusinessInterfaceSourceid As DCE.Security.BusinessInterfaceSourceType,
                                                  LoginName As String,
                                                  Originator As String,
                                                  Password As String,
                                                  Optional e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.LoginService.GetEncryptedUsernameToken.Request
            Dim res As New DCSM.LoginService.GetEncryptedUsernameToken.Response

            With req.RequestData
                .BusinessInterfaceSourceId = BusinessInterfaceSourceid
                .LoginName = LoginName
                .Originator = Originator
                .Password = Password
            End With

            If IFMS.Login.GetEncryptedUsernameToken(res, req, e, dv) Then
                If res IsNot Nothing Then
                    Return res.ResponseData.SecurityToken
                End If
            End If
            Return Nothing

        End Function

        Public Function LoginGuestUser(Optional e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.LoginService.LoginGuestUser.ResponseData
            Dim req As New DCSM.LoginService.LoginGuestUser.Request
            Dim res As New DCSM.LoginService.LoginGuestUser.Response

            With req.RequestData
            End With

            If IFMS.Login.LoginGuestUser(res, req, e, dv) Then
                If res IsNot Nothing Then
                    Return res.ResponseData
                End If
            End If
            Return Nothing

        End Function
    End Module
End Namespace

