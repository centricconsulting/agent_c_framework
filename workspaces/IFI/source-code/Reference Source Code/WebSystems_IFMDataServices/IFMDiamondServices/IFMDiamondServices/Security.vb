Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Security
    Public Module Security
        Public Function CanAgencyUserViewPolicy(PolicyId As Integer,
                                                PolicyImageNum As Integer,
                                                UsersId As Integer,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.CanAgencyUserViewPolicy.Request
            Dim res As New DCSM.SecurityService.CanAgencyUserViewPolicy.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UsersId = UsersId
            End With

            If (IFMS.SecurityService.CanAgencyUserViewPolicy(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function CanViewAgency(AgencyId As Integer,
                                      UsersId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.CanViewAgency.Request
            Dim res As New DCSM.SecurityService.CanViewAgency.Response

            With req.RequestData
                .AgencyId = AgencyId
                .UsersId = UsersId
            End With

            If (IFMS.SecurityService.CanViewAgency(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function CanViewAgencyGroup(UsersId As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.CanViewAgencyGroup.Request
            Dim res As New DCSM.SecurityService.CanViewAgencyGroup.Response

            With req.RequestData
                .UsersId = UsersId
            End With

            If (IFMS.SecurityService.CanViewAgencyGroup(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function CanViewClient(ClientId As Integer,
                                      UsersId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.CanViewClient.Request
            Dim res As New DCSM.SecurityService.CanViewClient.Response

            With req.RequestData
                .ClientId = ClientId
                .UsersId = UsersId
            End With

            If (IFMS.SecurityService.CanViewClient(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function ChangeUserPassword(LoginName As String,
                                           NewPassword1 As String,
                                           NewPassword2 As String,
                                           OldPassword As String,
                                           UsersId As String,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.ChangeUserPassword.Request
            Dim res As New DCSM.SecurityService.ChangeUserPassword.Response

            With req.RequestData
                .LoginName = LoginName
                .NewPassword1 = NewPassword1
                .NewPassword2 = NewPassword2
                .OldPassword = OldPassword
                .UsersId = UsersId
            End With

            If (IFMS.SecurityService.ChangeUserPassword(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function CheckPasswordComplexity(NewPassword As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.CheckPasswordComplexity.Request
            Dim res As New DCSM.SecurityService.CheckPasswordComplexity.Response

            With req.RequestData
                .NewPassword = NewPassword
            End With

            If (IFMS.SecurityService.CheckPasswordComplexity(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PasswordIsValid
                End If
            End If

            Return Nothing
        End Function

        Public Function ClientHasUserAssociated(ClientId As Integer,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.ClientHasUserAssociated.Request
            Dim res As New DCSM.SecurityService.ClientHasUserAssociated.Response

            With req.RequestData
                .ClientId = ClientId
            End With

            If (IFMS.SecurityService.ClientHasUserAssociated(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.HasAssociation
                End If
            End If

            Return Nothing
        End Function

        Public Function DecryptBankAccountNumber(AlwaysMask As Boolean,
                                                 EncryptedText As String,
                                                 EncryptionId As Integer,
                                                 EncryptionKeyType As DCE.Security.EncryptionKeyType,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.DecryptBankAccountNumber.Request
            Dim res As New DCSM.SecurityService.DecryptBankAccountNumber.Response

            With req.RequestData
                .AlwaysMask = AlwaysMask
                .EncryptedText = EncryptedText
                .EncryptionId = EncryptionId
                .EncryptionKeyType = EncryptionKeyType
            End With

            If (IFMS.SecurityService.DecryptBankAccountNumber(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PlainText
                End If
            End If

            Return Nothing
        End Function

        Public Function DecryptCreditCardNumber(AlwaysMask As Boolean,
                                                EncryptedText As String,
                                                EncryptionId As Integer,
                                                EncryptionKeyType As DCE.Security.EncryptionKeyType,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.DecryptCreditCardNumber.Request
            Dim res As New DCSM.SecurityService.DecryptCreditCardNumber.Response

            With req.RequestData
                .AlwaysMask = AlwaysMask
                .EncryptedText = EncryptedText
                .EncryptionId = EncryptionId
                .EncryptionKeyType = EncryptionKeyType
            End With

            If (IFMS.SecurityService.DecryptCreditCardNumber(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PlainText
                End If
            End If

            Return Nothing
        End Function

        Public Function DecryptEftAccountNumber(AlwaysMask As Boolean,
                                                EncryptedText As String,
                                                EncryptionId As Integer,
                                                EncryptionKeyType As DCE.Security.EncryptionKeyType,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.DecryptEftAccountNumber.Request
            Dim res As New DCSM.SecurityService.DecryptEftAccountNumber.Response

            With req.RequestData
                .AlwaysMask = AlwaysMask
                .EncryptedText = EncryptedText
                .EncryptionId = EncryptionId
                .EncryptionKeyType = EncryptionKeyType
            End With

            If (IFMS.SecurityService.DecryptEftAccountNumber(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PlainText
                End If
            End If

            Return Nothing
        End Function

        Public Function DecryptLicenseNumber(AlwaysMask As Boolean,
                                             EncryptedText As String,
                                             EncryptionId As Integer,
                                             EncryptionKeyType As DCE.Security.EncryptionKeyType,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.DecryptLicenseNumber.Request
            Dim res As New DCSM.SecurityService.DecryptLicenseNumber.Response

            With req.RequestData
                .AlwaysMask = AlwaysMask
                .EncryptedText = EncryptedText
                .EncryptionId = EncryptionId
                .EncryptionKeyType = EncryptionKeyType
            End With

            If (IFMS.SecurityService.DecryptLicenseNumber(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PlainText
                End If
            End If

            Return Nothing
        End Function

        Public Function DecryptMultipleItems(Items As DCO.InsCollection(Of DCSM.SecurityService.DataItem),
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.SecurityService.DataItem)
            Dim req As New DCSM.SecurityService.DecryptMultipleItems.Request
            Dim res As New DCSM.SecurityService.DecryptMultipleItems.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.DecryptMultipleItems(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Items
                End If
            End If

            Return Nothing
        End Function

        Public Function DecryptTaxNumber(Items As DCO.InsCollection(Of DCSM.SecurityService.DataItem),
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.DecryptTaxNumber.Request
            Dim res As New DCSM.SecurityService.DecryptTaxNumber.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.DecryptTaxNumber(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PlainText
                End If
            End If

            Return Nothing
        End Function

        Public Function DoesLoginAndDomainExist(LoginDomain As String,
                                                LoginName As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.DoesLoginAndDomainExist.Request
            Dim res As New DCSM.SecurityService.DoesLoginAndDomainExist.Response

            With req.RequestData
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.DoesLoginAndDomainExist(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function Encrypt(EncryptionId As Integer,
                                EncryptionKeyType As DCE.Security.EncryptionKeyType,
                                PlainText As String,
                                Optional ByRef e As Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.Encrypt.Request
            Dim res As New DCSM.SecurityService.Encrypt.Response

            With req.RequestData
                .EncryptionId = EncryptionId
                .EncryptionKeyType = EncryptionKeyType
                .PlainText = PlainText
            End With

            If (IFMS.SecurityService.Encrypt(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.EncryptedText
                End If
            End If

            Return Nothing
        End Function

        Public Function EncryptMultipleItems(Items As DCO.InsCollection(Of DCSM.SecurityService.DataItem),
                        Optional ByRef e As Exception = Nothing,
                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.SecurityService.DataItem)
            Dim req As New DCSM.SecurityService.EncryptMultipleItems.Request
            Dim res As New DCSM.SecurityService.EncryptMultipleItems.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.EncryptMultipleItems(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Items
                End If
            End If

            Return Nothing
        End Function

        'Public Function GetClientIdByDriversLicenseNumber(DriversLicenseNumber As String,
        '                                                  LicenseState As DCE.State,
        '                                                  Optional ByRef e As Exception = Nothing,
        '                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
        '    Dim req As New DCSM.SecurityService.GetClientIdByDriversLicenseNumber.Request
        '    Dim res As New DCSM.SecurityService.GetClientIdByDriversLicenseNumber.Response

        '    With req.RequestData
        '        .DriversLicenseNumber = DriversLicenseNumber
        '        .LicenseState = LicenseState
        '    End With

        '    If (IFMS.SecurityService.GetClientIdByDriversLicenseNumber(res, req, e, dv)) Then
        '        If (res.ResponseData IsNot Nothing) Then
        '            Return res.ResponseData.ClientId
        '        End If
        '    End If

        '    Return Nothing
        'End Function

        'Public Function GetClientIdByPolicyNumber(PolicyNumber As String,
        '                                          Optional ByRef e As Exception = Nothing,
        '                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
        '    Dim req As New DCSM.SecurityService.GetClientIdByPolicyNumber.Request
        '    Dim res As New DCSM.SecurityService.GetClientIdByPolicyNumber.Response

        '    With req.RequestData
        '        .PolicyNumber = PolicyNumber
        '    End With

        '    If (IFMS.SecurityService.GetClientIdByPolicyNumber(res, req, e, dv)) Then
        '        If (res.ResponseData IsNot Nothing) Then
        '            Return res.ResponseData.ClientId
        '        End If
        '    End If

        '    Return Nothing
        'End Function

        'Public Function GetClientIdByZipCodeAndBirthdate(BirthDate As DCO.InsDateTime,
        '                                                 ClientId As Integer,
        '                                                 ZipCode As String,
        '                                                 Optional ByRef e As Exception = Nothing,
        '                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
        '    Dim req As New DCSM.SecurityService.GetClientIdByZipCodeAndBirthdate.Request
        '    Dim res As New DCSM.SecurityService.GetClientIdByZipCodeAndBirthdate.Response

        '    With req.RequestData
        '        .BirthDate = BirthDate
        '        .ClientId = ClientId
        '        .ZipCode = ZipCode
        '    End With

        '    If (IFMS.SecurityService.GetClientIdByZipCodeAndBirthdate(res, req, e, dv)) Then
        '        If (res.ResponseData IsNot Nothing) Then
        '            Return res.ResponseData.ClientId
        '        End If
        '    End If

        '    Return Nothing
        'End Function

        Public Function GetSignedOnUser(Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.User
            Dim req As New DCSM.SecurityService.GetSignedOnUser.Request
            Dim res As New DCSM.SecurityService.GetSignedOnUser.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.GetSignedOnUser(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.User
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSignedOnUserId(Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.SecurityService.GetSignedOnUserId.Request
            Dim res As New DCSM.SecurityService.GetSignedOnUserId.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.GetSignedOnUserId(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UserId
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUser(LoginDomain As String,
                                LoginName As String,
                                Optional ByRef e As Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.User
            Dim req As New DCSM.SecurityService.GetUser.Request
            Dim res As New DCSM.SecurityService.GetUser.Response

            With req.RequestData
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.GetUser(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.User
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUserAgency(UserId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.UserAgency
            Dim req As New DCSM.SecurityService.GetUserAgency.Request
            Dim res As New DCSM.SecurityService.GetUserAgency.Response

            With req.RequestData
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.GetUserAgency(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Agency
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUserById(UserId As Integer,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.User
            Dim req As New DCSM.SecurityService.GetUserById.Request
            Dim res As New DCSM.SecurityService.GetUserById.Response

            With req.RequestData
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.GetUserById(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.User
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUserId(LoginDomain As String,
                                  LoginName As String,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.SecurityService.GetUserId.Request
            Dim res As New DCSM.SecurityService.GetUserId.Response

            With req.RequestData
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.GetUserId(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UserId
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUserLogon(LoginDomain As String,
                                     LoginName As String,
                                     UserId As String,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.SecurityService.GetUserLogon.ResponseData
            Dim req As New DCSM.SecurityService.GetUserLogon.Request
            Dim res As New DCSM.SecurityService.GetUserLogon.Response

            With req.RequestData
                .LoginDomain = LoginDomain
                .LoginName = LoginName
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.GetUserLogon(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetViewableAgencies(UserId As Integer,
                             Optional ByRef e As Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.AgencyInfo)
            Dim req As New DCSM.SecurityService.GetViewableAgencies.Request
            Dim res As New DCSM.SecurityService.GetViewableAgencies.Response

            With req.RequestData
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.GetViewableAgencies(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Agencies
                End If
            End If

            Return Nothing
        End Function

        Public Function GetViewableUsers(UserId As Integer,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.User)
            Dim req As New DCSM.SecurityService.GetViewableUsers.Request
            Dim res As New DCSM.SecurityService.GetViewableUsers.Response

            With req.RequestData
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.GetViewableUsers(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Users
                End If
            End If

            Return Nothing
        End Function

        Public Function IsInsuresoftUser(UserId As Integer,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.IsInsuresoftUser.Request
            Dim res As New DCSM.SecurityService.IsInsuresoftUser.Response

            With req.RequestData
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.IsInsuresoftUser(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function IsSupervisor(UserId As Integer,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.IsSupervisor.Request
            Dim res As New DCSM.SecurityService.IsSupervisor.Response

            With req.RequestData
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.IsSupervisor(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UserIsSupervisor
                End If
            End If

            Return Nothing
        End Function

        Public Function IsValidDiamondLogin(LoginDomain As String,
                                            LoginName As String,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.IsValidDiamondLogin.Request
            Dim res As New DCSM.SecurityService.IsValidDiamondLogin.Response

            With req.RequestData
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.IsValidDiamondLogin(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Valid
                End If
            End If

            Return Nothing
        End Function

        Public Function IsValidLogin(LoginDomain As String,
                                     LoginName As String,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.IsValidLogin.Request
            Dim res As New DCSM.SecurityService.IsValidLogin.Response

            With req.RequestData
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.IsValidLogin(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Valid
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadPasswordPolicy(Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.PasswordPolicy
            Dim req As New DCSM.SecurityService.LoadPasswordPolicy.Request
            Dim res As New DCSM.SecurityService.LoadPasswordPolicy.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.LoadPasswordPolicy(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PasswordPolicy
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadUsersUserSecurityQuestionLinkForLoginName(LoginName As String,
                                                                      Optional ByRef e As Exception = Nothing,
                                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLinkForLoginName)
            Dim req As New DCSM.SecurityService.LoadUsersUserSecurityQuestionLinkForLoginName.Request
            Dim res As New DCSM.SecurityService.LoadUsersUserSecurityQuestionLinkForLoginName.Response

            With req.RequestData
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.LoadUsersUserSecurityQuestionLinkForLoginName(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UsersUserSecurityQuestionLinkForLoginName
                End If
            End If

            Return Nothing
        End Function

        Public Function SavePasswordPolicy(PasswordPolicy As DCO.PasswordPolicy,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.SavePasswordPolicy.Request
            Dim res As New DCSM.SecurityService.SavePasswordPolicy.Response

            With req.RequestData
                .PasswordPolicy = PasswordPolicy
            End With

            If (IFMS.SecurityService.SavePasswordPolicy(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function SignedOnUserLogin(Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.SecurityService.SignedOnUserLogin.Request
            Dim res As New DCSM.SecurityService.SignedOnUserLogin.Response

            With req.RequestData
            End With

            If (IFMS.SecurityService.SignedOnUserLogin(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.User
                End If
            End If

            Return Nothing
        End Function

        Public Function UserHasAuthority(Amount As Double,
                                         Authority As String,
                                         Group As String,
                                         ReturnUnauthorizedMessage As Boolean,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.UserHasAuthority.Request
            Dim res As New DCSM.SecurityService.UserHasAuthority.Response

            With req.RequestData
                .Amount = Amount
                .Authority = Authority
                .Group = Group
                .ReturnUnauthorizedMessage = ReturnUnauthorizedMessage
            End With

            If (IFMS.SecurityService.UserHasAuthority(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.HasAuthority
                End If
            End If

            Return Nothing
        End Function

        Public Function UserHasAuthorityById(Amount As Double,
                                             Authority As String,
                                             Group As String,
                                             ReturnUnauthorizedMessage As Boolean,
                                             UserId As Integer,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.UserHasAuthorityById.Request
            Dim res As New DCSM.SecurityService.UserHasAuthorityById.Response

            With req.RequestData
                .Amount = Amount
                .Authority = Authority
                .Group = Group
                .ReturnUnauthorizedMessage = ReturnUnauthorizedMessage
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.UserHasAuthorityById(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.HasAuthority
                End If
            End If

            Return Nothing
        End Function

        Public Function ValidUser(Application As String,
                                  LoginDomain As String,
                                  LoginError As Integer,
                                  LoginName As String,
                                  UserId As Integer,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.SecurityService.ValidUser.ResponseData
            Dim req As New DCSM.SecurityService.ValidUser.Request
            Dim res As New DCSM.SecurityService.ValidUser.Response

            With req.RequestData
                .Application = Application
                .LoginDomain = LoginDomain
                .LoginError = LoginError
                .LoginName = LoginName
                .UsersId = UserId
            End With

            If (IFMS.SecurityService.ValidUser(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function VerifyAnswersUsersUserSecurityQuestionLinkForLoginName(LoginName As String,
                                                                               SuppressEmail As Boolean,
                                                                               UsersUserSecurityQuestionLinkForLoginName As DCO.InsCollection(Of DCO.Administration.UsersUserSecurityQuestionLinkForLoginName),
                                                                               Optional ByRef e As Exception = Nothing,
                                                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.SecurityService.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.ResponseData
            Dim req As New DCSM.SecurityService.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.Request
            Dim res As New DCSM.SecurityService.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.Response

            With req.RequestData
                .LoginName = LoginName
                .SuppressEmail = SuppressEmail
                .UsersUserSecurityQuestionLinkForLoginName = UsersUserSecurityQuestionLinkForLoginName
            End With

            If (IFMS.SecurityService.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function VerifyUsersEmailAddress(EmailAddress As String,
                                                LoginDomain As String,
                                                LoginName As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.SecurityService.VerifyUsersEmailAddress.Request
            Dim res As New DCSM.SecurityService.VerifyUsersEmailAddress.Response

            With req.RequestData
                .EmailAddress = EmailAddress
                .LoginDomain = LoginDomain
                .LoginName = LoginName
            End With

            If (IFMS.SecurityService.VerifyUsersEmailAddress(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Valid
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace