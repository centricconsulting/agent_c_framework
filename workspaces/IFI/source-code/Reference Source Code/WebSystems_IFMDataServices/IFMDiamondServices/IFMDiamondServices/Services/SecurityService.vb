Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.SecurityService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.SecurityService
    Public Module SecurityServiceProxy
        Public Function CanAgencyUserViewPolicy(ByRef res As DSCM.CanAgencyUserViewPolicy.Response,
                                                ByRef req As DSCM.CanAgencyUserViewPolicy.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanAgencyUserViewPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanViewAgency(ByRef res As DSCM.CanViewAgency.Response,
                                      ByRef req As DSCM.CanViewAgency.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanViewAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanViewAgencyGroup(ByRef res As DSCM.CanViewAgencyGroup.Response,
                                           ByRef req As DSCM.CanViewAgencyGroup.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanViewAgencyGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanViewClient(ByRef res As DSCM.CanViewClient.Response,
                                      ByRef req As DSCM.CanViewClient.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanViewClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ChangeUserPassword(ByRef res As DSCM.ChangeUserPassword.Response,
                                           ByRef req As DSCM.ChangeUserPassword.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ChangeUserPassword
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CheckPasswordComplexity(ByRef res As DSCM.CheckPasswordComplexity.Response,
                                                ByRef req As DSCM.CheckPasswordComplexity.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckPasswordComplexity
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClientHasUserAssociated(ByRef res As DSCM.ClientHasUserAssociated.Response,
                                                ByRef req As DSCM.ClientHasUserAssociated.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ClientHasUserAssociated
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DecryptBankAccountNumber(ByRef res As DSCM.DecryptBankAccountNumber.Response,
                                                 ByRef req As DSCM.DecryptBankAccountNumber.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DecryptBankAccountNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DecryptCreditCardNumber(ByRef res As DSCM.DecryptCreditCardNumber.Response,
                                                ByRef req As DSCM.DecryptCreditCardNumber.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DecryptCreditCardNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DecryptEftAccountNumber(ByRef res As DSCM.DecryptEftAccountNumber.Response,
                                                ByRef req As DSCM.DecryptEftAccountNumber.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DecryptEftAccountNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DecryptLicenseNumber(ByRef res As DSCM.DecryptLicenseNumber.Response,
                                             ByRef req As DSCM.DecryptLicenseNumber.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DecryptLicenseNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DecryptMultipleItems(ByRef res As DSCM.DecryptMultipleItems.Response,
                                             ByRef req As DSCM.DecryptMultipleItems.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DecryptMultipleItems
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DecryptTaxNumber(ByRef res As DSCM.DecryptTaxNumber.Response,
                                         ByRef req As DSCM.DecryptTaxNumber.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DecryptTaxNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DoesLoginAndDomainExist(ByRef res As DSCM.DoesLoginAndDomainExist.Response,
                                                ByRef req As DSCM.DoesLoginAndDomainExist.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DoesLoginAndDomainExist
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Encrypt(ByRef res As DSCM.Encrypt.Response,
                                ByRef req As DSCM.Encrypt.Request,
                                Optional ByRef e As Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Encrypt
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function EncryptMultipleItems(ByRef res As DSCM.EncryptMultipleItems.Response,
                                             ByRef req As DSCM.EncryptMultipleItems.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.EncryptMultipleItems
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        'Public Function GetClientIdByDriversLicenseNumber(ByRef res As DSCM.cli.Response,
        '                                                  ByRef req As DSCM.ge.Request,
        '                                                  Optional ByRef e As Exception = Nothing,
        '                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.SecurityServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.GetClientIdByDriversLicenseNumber
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        'Public Function GetClientIdByPolicyNumber(ByRef res As DSCM.GetClientIdByPolicyNumber.Response,
        '                                          ByRef req As DSCM.GetClientIdByPolicyNumber.Request,
        '                                          Optional ByRef e As Exception = Nothing,
        '                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.SecurityServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.GetClientIdByPolicyNumber
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        'Public Function GetClientIdByZipCodeAndBirthdate(ByRef res As DSCM.GetClientIdByZipCodeAndBirthdate.Response,
        '                                                 ByRef req As DSCM.GetClientIdByZipCodeAndBirthdate.Request,
        '                                                 Optional ByRef e As Exception = Nothing,
        '                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.SecurityServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.GetClientIdByZipCodeAndBirthdate
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        Public Function GetSignedOnUser(ByRef res As DSCM.GetSignedOnUser.Response,
                                        ByRef req As DSCM.GetSignedOnUser.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetSignedOnUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSignedOnUserId(ByRef res As DSCM.GetSignedOnUserId.Response,
                                          ByRef req As DSCM.GetSignedOnUserId.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetSignedOnUserId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUser(ByRef res As DSCM.GetUser.Response,
                                ByRef req As DSCM.GetUser.Request,
                                Optional ByRef e As Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUserAgency(ByRef res As DSCM.GetUserAgency.Response,
                                      ByRef req As DSCM.GetUserAgency.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUserAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUserById(ByRef res As DSCM.GetUserById.Response,
                                    ByRef req As DSCM.GetUserById.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUserById
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUserId(ByRef res As DSCM.GetUserId.Response,
                                  ByRef req As DSCM.GetUserId.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUserId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUserLogon(ByRef res As DSCM.GetUserLogon.Response,
                                     ByRef req As DSCM.GetUserLogon.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUserLogon
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetViewableAgencies(ByRef res As DSCM.GetViewableAgencies.Response,
                                            ByRef req As DSCM.GetViewableAgencies.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetViewableAgencies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetViewableUsers(ByRef res As DSCM.GetViewableUsers.Response,
                                         ByRef req As DSCM.GetViewableUsers.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetViewableUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsInsuresoftUser(ByRef res As DSCM.IsInsuresoftUser.Response,
                                         ByRef req As DSCM.IsInsuresoftUser.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsInsuresoftUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsSupervisor(ByRef res As DSCM.IsSupervisor.Response,
                                     ByRef req As DSCM.IsSupervisor.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsSupervisor
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsValidDiamondLogin(ByRef res As DSCM.IsValidDiamondLogin.Response,
                                            ByRef req As DSCM.IsValidDiamondLogin.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsValidDiamondLogin
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsValidLogin(ByRef res As DSCM.IsValidLogin.Response,
                                     ByRef req As DSCM.IsValidLogin.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsValidLogin
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPasswordPolicy(ByRef res As DSCM.LoadPasswordPolicy.Response,
                                           ByRef req As DSCM.LoadPasswordPolicy.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPasswordPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadUsersUserSecurityQuestionLinkForLoginName(ByRef res As DSCM.LoadUsersUserSecurityQuestionLinkForLoginName.Response,
                                                                      ByRef req As DSCM.LoadUsersUserSecurityQuestionLinkForLoginName.Request,
                                                                      Optional ByRef e As Exception = Nothing,
                                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUsersUserSecurityQuestionLinkForLoginName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SavePasswordPolicy(ByRef res As DSCM.SavePasswordPolicy.Response,
                                           ByRef req As DSCM.SavePasswordPolicy.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePasswordPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SignedOnUserLogin(ByRef res As DSCM.SignedOnUserLogin.Response,
                                          ByRef req As DSCM.SignedOnUserLogin.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SignedOnUserLogin
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UserHasAuthority(ByRef res As DSCM.UserHasAuthority.Response,
                                         ByRef req As DSCM.UserHasAuthority.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UserHasAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UserHasAuthorityById(ByRef res As DSCM.UserHasAuthorityById.Response,
                                             ByRef req As DSCM.UserHasAuthorityById.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UserHasAuthorityById
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ValidUser(ByRef res As DSCM.ValidUser.Response,
                                  ByRef req As DSCM.ValidUser.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VerifyAnswersUsersUserSecurityQuestionLinkForLoginName(ByRef res As DSCM.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.Response,
                                                                               ByRef req As DSCM.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName.Request,
                                                                               Optional ByRef e As Exception = Nothing,
                                                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.VerifyAnswersUsersUserSecurityQuestionLinkForLoginName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VerifyUsersEmailAddress(ByRef res As DSCM.VerifyUsersEmailAddress.Response,
                                                ByRef req As DSCM.VerifyUsersEmailAddress.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SecurityServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.VerifyUsersEmailAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace