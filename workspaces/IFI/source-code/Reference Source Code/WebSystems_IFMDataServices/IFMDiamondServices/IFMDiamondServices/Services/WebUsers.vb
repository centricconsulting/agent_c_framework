Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.WebUsersService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.WebUsers
    Public Module WebUsers
        Public Function GetWebUserFromEmail(ByRef res As DSCM.GetWebUserFromEmail.Response,
                                             ByRef req As DSCM.GetWebUserFromEmail.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebUsersProxy
            Dim m As Services.common.pMethod = AddressOf p.GetWebUserFromEmail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetWebUserFromLogin(ByRef res As DSCM.GetWebUserFromLogin.Response,
                                            ByRef req As DSCM.GetWebUserFromLogin.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebUsersProxy
            Dim m As Services.common.pMethod = AddressOf p.GetWebUserFromLogin
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UpdateWebUser(ByRef res As DSCM.UpdateWebUser.Response,
                                      ByRef req As DSCM.UpdateWebUser.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebUsersProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdateWebUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function XYZ(ByRef res As DSCM.XYZ.Response,
                              ByRef req As DSCM.XYZ.Request,
                              Optional ByRef e As Exception = Nothing,
                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebUsersProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdateWebUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace