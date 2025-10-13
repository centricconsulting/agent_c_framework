Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.WebUsers
    Public Module WebUsers
        Public Function GetWebUserFromEmail(EmailAddress As String,
                                            WebSiteId As Integer,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Web.WebUser
            Dim req As New DCSM.WebUsersService.GetWebUserFromEmail.Request
            Dim res As New DCSM.WebUsersService.GetWebUserFromEmail.Response

            With req.RequestData
                .EmailAddress = EmailAddress
                .WebSiteId = WebSiteId
            End With

            If (IFMS.WebUsers.GetWebUserFromEmail(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WebUser
                End If
            End If

            Return Nothing
        End Function

        Public Function GetWebUserFromLogin(Password As String,
                                            UserName As String,
                                            WebSiteId As Integer,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Web.WebUser
            Dim req As New DCSM.WebUsersService.GetWebUserFromLogin.Request
            Dim res As New DCSM.WebUsersService.GetWebUserFromLogin.Response

            With req.RequestData
                .Password = Password
                .UserName = UserName
                .WebSiteId = WebSiteId
            End With

            If (IFMS.WebUsers.GetWebUserFromLogin(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WebUser
                End If
            End If

            Return Nothing
        End Function

        Public Function UpdateWebUser(EmailAddress As String,
                                      EmailAddressChanged As Boolean,
                                      Password As String,
                                      PasswordToCompare As String,
                                      SecurityRole As Integer,
                                      StatusCodeId As Integer,
                                      UserName As String,
                                      UsersId As Integer,
                                      WebSiteId As Integer,
                                      WebUsersId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.WebUsersService.UpdateWebUser.Request
            Dim res As New DCSM.WebUsersService.UpdateWebUser.Response

            With req.RequestData
                .EmailAddress = EmailAddress
                .EmailAddressChanged = EmailAddressChanged
                .Password = Password
                .PasswordToCompare = PasswordToCompare
                .SecurityRole = SecurityRole
                .StatusCodeId = StatusCodeId
                .UserName = UserName
                .UsersId = UsersId
                .WebSiteId = WebSiteId
                .WebUsersId = WebUsersId
            End With

            If (IFMS.WebUsers.UpdateWebUser(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WebUsersId
                End If
            End If

            Return Nothing
        End Function

        Public Function XYZ(ClaimControlId As Integer,
                              Optional ByRef e As Exception = Nothing,
                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Claims.Claimant.ClaimantData
            Dim req As New DCSM.WebUsersService.XYZ.Request
            Dim res As New DCSM.WebUsersService.XYZ.Response

            With req.RequestData
                .ClaimControlId = ClaimControlId
            End With

            If (IFMS.WebUsers.XYZ(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ClaimantData
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace