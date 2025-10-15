Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.LoginService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.Login
    Public Module Login
        Public Function AuthenticateWithEncryptedToken(ByRef res As DCSM.AuthenticateWithEncryptedToken.Response,
                                                       ByRef req As DCSM.AuthenticateWithEncryptedToken.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LoginServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AuthenticateWithEncryptedToken
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetDiamTokenForDomainUsername(ByRef res As DCSM.GetDiamTokenForDomainUsername.Response,
                                                       ByRef req As DCSM.GetDiamTokenForDomainUsername.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LoginServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDiamTokenForDomainUsername
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetDiamTokenForThirdPartyToken(ByRef res As DCSM.GetDiamTokenForThirdPartyToken.Response,
                                                       ByRef req As DCSM.GetDiamTokenForThirdPartyToken.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LoginServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDiamTokenForThirdPartyToken
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetDiamTokenForUsernamePassword(ByRef res As DCSM.GetDiamTokenForUsernamePassword.Response,
                                                       ByRef req As DCSM.GetDiamTokenForUsernamePassword.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LoginServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDiamTokenForUsernamePassword
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetEncryptedUsernameToken(ByRef res As DCSM.GetEncryptedUsernameToken.Response,
                                                       ByRef req As DCSM.GetEncryptedUsernameToken.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LoginServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetEncryptedUsernameToken
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoginGuestUser(ByRef res As DCSM.LoginGuestUser.Response,
                                                       ByRef req As DCSM.LoginGuestUser.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LoginServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoginGuestUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
