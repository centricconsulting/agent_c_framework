Imports Microsoft.VisualBasic
Imports DCSA = Diamond.Common.Services.Messages.AdministrationService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.Administration
    Friend Module Administration
        Public Function AddAuthorityTemplateAuthority(ByRef res As DCSA.AddAuthorityTemplateAuthority.Response,
                                                      ByRef req As DCSA.AddAuthorityTemplateAuthority.Request,
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddAuthorityTemplateAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddAuthorityTemplatesToSecurityGroup(ByRef res As DCSA.AddAuthorityTemplatesToSecurityGroup.Response,
                                                             ByRef req As DCSA.AddAuthorityTemplatesToSecurityGroup.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddAuthorityTemplatesToSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddAuthorityToAllUsersInUserType(ByRef res As DCSA.AddAuthorityToAllUsersInUserType.Response,
                                                             ByRef req As DCSA.AddAuthorityToAllUsersInUserType.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddAuthorityToAllUsersInUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddAuthorityToUserOrUserType(ByRef res As DCSA.AddAuthorityToUserOrUserType.Response,
                                                             ByRef req As DCSA.AddAuthorityToUserOrUserType.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddAuthorityToUserOrUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddSelectedProducer(ByRef res As DCSA.AddSelectedProducer.Response,
                                                             ByRef req As DCSA.AddSelectedProducer.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddSelectedProducer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddTransactionReasonViewableByUserCategory(ByRef res As DCSA.AddTransactionReasonViewableByUserCategory.Response,
                                                             ByRef req As DCSA.AddTransactionReasonViewableByUserCategory.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddTransactionReasonViewableByUserCategory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddUsersAuthority(ByRef res As DCSA.AddUsersAuthority.Response,
                                                             ByRef req As DCSA.AddUsersAuthority.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddUsersAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddUsersToSecurityGroup(ByRef res As DCSA.AddUsersToSecurityGroup.Response,
                                                             ByRef req As DCSA.AddUsersToSecurityGroup.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddUsersToSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddUserTypeAuthority(ByRef res As DCSA.AddUserTypeAuthority.Response,
                                                             ByRef req As DCSA.AddUserTypeAuthority.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddUserTypeAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AgencyLobNonRenewStatus(ByRef res As DCSA.AgencyLobNonRenewStatus.Response,
                                                ByRef req As DCSA.AgencyLobNonRenewStatus.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AgencyLobNonRenewStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CanDeleteCompany(ByRef res As DCSA.CanDeleteCompany.Response,
                                            ByRef req As DCSA.CanDeleteCompany.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanDeleteCompany
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckForPrimaryContact(ByRef res As DCSA.CheckForPrimaryContact.Response,
                                            ByRef req As DCSA.CheckForPrimaryContact.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckForPrimaryContact
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckForValidInstalls(ByRef res As DCSA.CheckForValidInstalls.Response,
                                            ByRef req As DCSA.CheckForValidInstalls.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckForValidInstalls
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CopyAgency(ByRef res As DCSA.CopyAgency.Response,
                                            ByRef req As DCSA.CopyAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CopyAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CopyUserAuthorities(ByRef res As DCSA.CopyUserAuthorities.Response,
                                            ByRef req As DCSA.CopyUserAuthorities.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CopyUserAuthorities
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgency(ByRef res As DCSA.DeleteAgency.Response,
                                            ByRef req As DCSA.DeleteAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyCommission(ByRef res As DCSA.DeleteAgencyCommission.Response,
                                            ByRef req As DCSA.DeleteAgencyCommission.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyCommission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyCommissionDetail(ByRef res As DCSA.DeleteAgencyCommissionDetail.Response,
                                            ByRef req As DCSA.DeleteAgencyCommissionDetail.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyCommissionDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyCommissionDetailType(ByRef res As DCSA.DeleteAgencyCommissionDetailType.Response,
                                            ByRef req As DCSA.DeleteAgencyCommissionDetailType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyCommissionDetailType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyGroup(ByRef res As DCSA.DeleteAgencyGroup.Response,
                                            ByRef req As DCSA.DeleteAgencyGroup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyProducer(ByRef res As DCSA.DeleteAgencyProducer.Response,
                                            ByRef req As DCSA.DeleteAgencyProducer.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyProducer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyReimbursement(ByRef res As DCSA.DeleteAgencyReimbursement.Response,
                                            ByRef req As DCSA.DeleteAgencyReimbursement.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAuthorityTemplate(ByRef res As DCSA.DeleteAuthorityTemplate.Response,
                                            ByRef req As DCSA.DeleteAuthorityTemplate.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAuthorityTemplate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBankAccount(ByRef res As DCSA.DeleteBankAccount.Response,
                                            ByRef req As DCSA.DeleteBankAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBankAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBankAccountCompanyLnk(ByRef res As DCSA.DeleteBankAccountCompanyLnk.Response,
                                            ByRef req As DCSA.DeleteBankAccountCompanyLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBankAccountCompanyLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBankAccountCompanyLnkUse(ByRef res As DCSA.DeleteBankAccountCompanyLnkUse.Response,
                                            ByRef req As DCSA.DeleteBankAccountCompanyLnkUse.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBankAccountCompanyLnkUse
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBillingPayplanBillMethodVersion(ByRef res As DCSA.DeleteBillingPayplanBillMethodVersion.Response,
                                            ByRef req As DCSA.DeleteBillingPayplanBillMethodVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBillingPayplanBillMethodVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBillMethodVersion(ByRef res As DCSA.DeleteBillMethodVersion.Response,
                                            ByRef req As DCSA.DeleteBillMethodVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBillMethodVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBillToVersion(ByRef res As DCSA.DeleteBillToVersion.Response,
                                            ByRef req As DCSA.DeleteBillToVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBillToVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBranch(ByRef res As DCSA.DeleteBranch.Response,
                                            ByRef req As DCSA.DeleteBranch.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBranch
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCBDUserType(ByRef res As DCSA.DeleteCBDUserType.Response,
                                            ByRef req As DCSA.DeleteCBDUserType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCBDUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCompany(ByRef res As DCSA.DeleteCompany.Response,
                                            ByRef req As DCSA.DeleteCompany.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCompany
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCompanyStateLob(ByRef res As DCSA.DeleteCompanyStateLob.Response,
                                            ByRef req As DCSA.DeleteCompanyStateLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCompanyStateLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteConfigAuthority(ByRef res As DCSA.DeleteConfigAuthority.Response,
                                            ByRef req As DCSA.DeleteConfigAuthority.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteConfigAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteDepartment(ByRef res As DCSA.DeleteDepartment.Response,
                                            ByRef req As DCSA.DeleteDepartment.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteDepartment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteDetailVersion(ByRef res As DCSA.DeleteDetailVersion.Response,
                                            ByRef req As DCSA.DeleteDetailVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.deleteDetailVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteDiamondUser(ByRef res As DCSA.DeleteDiamondUser.Response,
                                            ByRef req As DCSA.DeleteDiamondUser.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteDiamondUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteEFTAccount(ByRef res As DCSA.DeleteEFTAccount.Response,
                                            ByRef req As DCSA.DeleteEFTAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteEFTAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteEmail(ByRef res As DCSA.DeleteEmail.Response,
                                            ByRef req As DCSA.DeleteEmail.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteEmail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteEmployee(ByRef res As DCSA.DeleteEmployee.Response,
                                            ByRef req As DCSA.DeleteEmployee.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteEmployee
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteHoliday(ByRef res As DCSA.DeleteHoliday.Response,
                                            ByRef req As DCSA.DeleteHoliday.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteHoliday
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteHurricaneBlackoutDates(ByRef res As DCSA.DeleteHurricaneBlackoutDates.Response,
                                            ByRef req As DCSA.DeleteHurricaneBlackoutDates.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteHurricaneBlackoutDates
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteLob(ByRef res As DCSA.DeleteLob.Response,
                                            ByRef req As DCSA.DeleteLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteLockboxAddress(ByRef res As DCSA.DeleteLockboxAddress.Response,
                                            ByRef req As DCSA.DeleteLockboxAddress.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteLockboxAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteLockboxAddressCompanyStateLobLnk(ByRef res As DCSA.DeleteLockboxAddressCompanyStateLobLnk.Response,
                                            ByRef req As DCSA.DeleteLockboxAddressCompanyStateLobLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteLockboxAddressCompanyStateLobLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteNotesType(ByRef res As DCSA.DeleteNotesType.Response,
                                            ByRef req As DCSA.DeleteNotesType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteNotesType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteOtherCarrier(ByRef res As DCSA.DeleteOtherCarrier.Response,
                                            ByRef req As DCSA.DeleteOtherCarrier.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteOtherCarrier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteOtherCarrierFromAgency(ByRef res As DCSA.DeleteOtherCarrierFromAgency.Response,
                                            ByRef req As DCSA.DeleteOtherCarrierFromAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteOtherCarrierFromAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePayPlan(ByRef res As DCSA.DeletePayplan.Response,
                                            ByRef req As DCSA.DeletePayplan.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePayPlanInstallment(ByRef res As DCSA.DeletePayPlanInstallment.Response,
                                            ByRef req As DCSA.DeletePayPlanInstallment.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePayPlanInstallment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePayrollDeductionEmployer(ByRef res As DCSA.DeletePayrollDeductionEmployer.Response,
                                            ByRef req As DCSA.DeletePayrollDeductionEmployer.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePayrollDeductionEmployer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePolicyTermVersion(ByRef res As DCSA.DeletePolicyTermVersion.Response,
                                            ByRef req As DCSA.DeletePolicyTermVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePolicyTermVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteRatingVersion(ByRef res As DCSA.DeleteRatingVersion.Response,
                                            ByRef req As DCSA.DeleteRatingVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteRatingVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteReimbursement(ByRef res As DCSA.DeleteReimbursement.Response,
                                            ByRef req As DCSA.DeleteReimbursement.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteReinsurance(ByRef res As DCSA.DeleteReinsurance.Response,
                                            ByRef req As DCSA.DeleteReinsurance.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteReinsurance
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteRenewalRollOn(ByRef res As DCSA.DeleteRenewalRollOn.Response,
                                            ByRef req As DCSA.DeleteRenewalRollOn.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteRenewalRollOn
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteReport(ByRef res As DCSA.DeleteReport.Response,
                                            ByRef req As DCSA.DeleteReport.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteReport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteSecurityGroup(ByRef res As DCSA.DeleteSecurityGroup.Response,
                                            ByRef req As DCSA.DeleteSecurityGroup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteTransactionReasonViewableByUserCategory(ByRef res As DCSA.DeleteTransactionReasonViewableByUserCategory.Response,
                                            ByRef req As DCSA.DeleteTransactionReasonViewableByUserCategory.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteTransactionReasonViewableByUserCategory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteUnderwritingVersion(ByRef res As DCSA.DeleteUnderwritingVersion.Response,
                                            ByRef req As DCSA.DeleteUnderwritingVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteUnderwritingVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteUserSecurityQuestion(ByRef res As DCSA.DeleteUserSecurityQuestion.Response,
                                            ByRef req As DCSA.DeleteUserSecurityQuestion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteUserSecurityQuestion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteUserType(ByRef res As DCSA.DeleteUserType.Response,
                                            ByRef req As DCSA.DeleteUserType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteWorkflowQueue(ByRef res As DCSA.DeleteWorkflowQueue.Response,
                                            ByRef req As DCSA.DeleteWorkflowQueue.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteWorkflowQueue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExportPayPlan(ByRef res As DCSA.ExportPayPlan.Response,
                                            ByRef req As DCSA.ExportPayPlan.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ExportPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetMaxModelISOCount(ByRef res As DCSA.GetMaxModelISOCount.Response,
                                            ByRef req As DCSA.GetMaxModelISOCount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMaxModelISOCount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Service is obsolete.. commented out 3/12/14 RA
        'Public Function GetVersionByCompanyStateLOB(ByRef res As DCSA.GetVersionByCompanyStateLOB.Response,
        '                                    ByRef req As DCSA.GetVersionByCompanyStateLOB.Request,
        '                                    Optional ByRef e As Exception = Nothing,
        '                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.AdministrationServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.GetVersionByCompanyStateLOB
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function IsQueueAssignedToUserOrAgency(ByRef res As DCSA.IsQueueAssignedToUserOrAgency.Response,
                                            ByRef req As DCSA.IsQueueAssignedToUserOrAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsQueueAssignedToUserOrAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadActiveUsers(ByRef res As DCSA.LoadActiveUsers.Response,
                                            ByRef req As DCSA.LoadActiveUsers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadActiveUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAddFormsVersion(ByRef res As DCSA.LoadAddFormsVersion.Response,
                                            ByRef req As DCSA.LoadAddFormsVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAddFormsVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdminMHPark(ByRef res As DCSA.LoadAdminMHPark.Response,
                                            ByRef req As DCSA.LoadAdminMHPark.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdminMHPark
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgency(ByRef res As DCSA.LoadAgency.Response,
                                            ByRef req As DCSA.LoadAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyBookTransfers(ByRef res As DCSA.LoadAgencyBookTransfers.Response,
                                            ByRef req As DCSA.LoadAgencyBookTransfers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyBookTransfers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyCommission(ByRef res As DCSA.LoadAgencyCommission.Response,
                                            ByRef req As DCSA.LoadAgencyCommission.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyCommission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyCommissionDetail(ByRef res As DCSA.LoadAgencyCommissionDetail.Response,
                                            ByRef req As DCSA.LoadAgencyCommissionDetail.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyCommissionDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyCommissionDetailType(ByRef res As DCSA.LoadAgencyCommissionDetailType.Response,
                                            ByRef req As DCSA.LoadAgencyCommissionDetailType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyCommissionDetailType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyCompanyStateLob(ByRef res As DCSA.LoadAgencyCompanyStateLob.Response,
                                            ByRef req As DCSA.LoadAgencyCompanyStateLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyCompanyStateLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyEFT(ByRef res As DCSA.LoadAgencyEFT.Response,
                                            ByRef req As DCSA.LoadAgencyEFT.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyEFT
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyEmailLink(ByRef res As DCSA.LoadAgencyEmailLink.Response,
                                            ByRef req As DCSA.LoadAgencyEmailLink.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyEmailLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyExperience(ByRef res As DCSA.LoadAgencyExperience.Response,
                                            ByRef req As DCSA.LoadAgencyExperience.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyExperience
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyExperiencePerDate(ByRef res As DCSA.LoadAgencyExperience.Response,
                                            ByRef req As DCSA.LoadAgencyExperience.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyExperiencePerDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyGroupAddress(ByRef res As DCSA.LoadAgencyGroupAddress.Response,
                                            ByRef req As DCSA.LoadAgencyGroupAddress.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyGroupAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyGroups(ByRef res As DCSA.LoadAgencyGroups.Response,
                                            ByRef req As DCSA.LoadAgencyGroups.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyGroups
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyGroupSetup(ByRef res As DCSA.LoadAgencyGroupSetup.Response,
                                            ByRef req As DCSA.LoadAgencyGroupSetup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyGroupSetup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyLob(ByRef res As DCSA.LoadAgencyLob.Response,
                                            ByRef req As DCSA.LoadAgencyLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyName(ByRef res As DCSA.LoadAgencyName.Response,
                                            ByRef req As DCSA.LoadAgencyName.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyOtherCarrier(ByRef res As DCSA.LoadAgencyOtherCarrier.Response,
                                            ByRef req As DCSA.LoadAgencyOtherCarrier.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyOtherCarrier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyPayPlan(ByRef res As DCSA.LoadAgencyPayPlan.Response,
                                            ByRef req As DCSA.LoadAgencyPayPlan.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyPersonnel(ByRef res As DCSA.LoadAgencyPersonnel.Response,
                                            ByRef req As DCSA.LoadAgencyPersonnel.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyPrinterTypes(ByRef res As DCSA.LoadAgencyPrinterTypes.Response,
                                            ByRef req As DCSA.LoadAgencyPrinterTypes.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyPrinterTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyProducers(ByRef res As DCSA.LoadAgencyProducers.Response,
                                            ByRef req As DCSA.LoadAgencyProducers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyProducers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyReimbursement(ByRef res As DCSA.LoadAgencyReimbursement.Response,
                                            ByRef req As DCSA.LoadAgencyReimbursement.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencySetup(ByRef res As DCSA.LoadAgencySetup.Response,
                                            ByRef req As DCSA.LoadAgencySetup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencySetup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyUserList(ByRef res As DCSA.LoadAgencyUserList.Response,
                                            ByRef req As DCSA.LoadAgencyUserList.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyUserList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyUserName(ByRef res As DCSA.LoadAgencyUserName.Response,
                                            ByRef req As DCSA.LoadAgencyUserName.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyUserName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyWorkflowQueuesLoadAgencyWorkflowQueues(ByRef res As DCSA.LoadAgencyWorkflowQueues.Response,
                                            ByRef req As DCSA.LoadAgencyWorkflowQueues.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyWorkflowQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgyCommDetailType(ByRef res As DCSA.LoadAgyCommDetailType.Response,
                                            ByRef req As DCSA.LoadAgyCommDetailType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgyCommDetailType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllAgencies(ByRef res As DCSA.LoadAllAgencies.Response,
                                            ByRef req As DCSA.LoadAllAgencies.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllAgencies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllAuthorities(ByRef res As DCSA.LoadAllAuthorities.Response,
                                            ByRef req As DCSA.LoadAllAuthorities.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllAuthorities
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllBankAccountCompanyLnk(ByRef res As DCSA.LoadAllBankAccountCompanyLnk.Response,
                                            ByRef req As DCSA.LoadAllBankAccountCompanyLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllBankAccountCompanyLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllBankAccountLockboxAddressLnk(ByRef res As DCSA.LoadAllBankAccountLockboxAddressLnk.Response,
                                            ByRef req As DCSA.LoadAllBankAccountLockboxAddressLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllBankAccountLockboxAddressLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllBankAccounts(ByRef res As DCSA.LoadAllBankAccounts.Response,
                                            ByRef req As DCSA.LoadAllBankAccounts.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllBankAccounts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllBankLockboxAddressLnk(ByRef res As DCSA.LoadAllBankLockboxAddressLnk.Response,
                                            ByRef req As DCSA.LoadAllBankLockboxAddressLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllBankLockboxAddressLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllBranches(ByRef res As DCSA.LoadAllBranches.Response,
                                            ByRef req As DCSA.LoadAllBranches.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllBranches
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllCompany(ByRef res As DCSA.LoadAllCompany.Response,
                                            ByRef req As DCSA.LoadAllCompany.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllCompany
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllCompanyStateLob(ByRef res As DCSA.LoadAllCompanyStateLob.Response,
                                            ByRef req As DCSA.LoadAllCompanyStateLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllCompanyStateLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllDepartments(ByRef res As DCSA.LoadAllDepartments.Response,
                                            ByRef req As DCSA.LoadAllDepartments.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllDepartments
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllEmail(ByRef res As DCSA.LoadAllEmail.Response,
                                            ByRef req As DCSA.LoadAllEmail.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllEmail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllEmployees(ByRef res As DCSA.LoadAllEmployees.Response,
                                            ByRef req As DCSA.LoadAllEmployees.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllEmployees
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllLob(ByRef res As DCSA.LoadAllLob.Response,
                                            ByRef req As DCSA.LoadAllLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllOtherUsers(ByRef res As DCSA.LoadAllOtherUsers.Response,
                                            ByRef req As DCSA.LoadAllOtherUsers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllOtherUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllPhone(ByRef res As DCSA.LoadAllPhone.Response,
                                            ByRef req As DCSA.LoadAllPhone.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllPhone
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllUsers(ByRef res As DCSA.LoadAllUsers.Response,
                                            ByRef req As DCSA.LoadAllUsers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllUserTypes(ByRef res As DCSA.LoadAllUserTypes.Response,
                                            ByRef req As DCSA.LoadAllUserTypes.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllUserTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAuthoritiesPerGroup(ByRef res As DCSA.LoadAuthoritiesPerGroup.Response,
                                            ByRef req As DCSA.LoadAuthoritiesPerGroup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAuthoritiesPerGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAuthority(ByRef res As DCSA.LoadAuthority.Response,
                                            ByRef req As DCSA.LoadAuthority.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAuthorityGroups(ByRef res As DCSA.LoadAuthorityGroups.Response,
                                            ByRef req As DCSA.LoadAuthorityGroups.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAuthorityGroups
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAuthorityTemplateAllAuthorities(ByRef res As DCSA.LoadAuthorityTemplateAllAuthorities.Response,
                                            ByRef req As DCSA.LoadAuthorityTemplateAllAuthorities.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAuthorityTemplateAllAuthorities
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAuthorityTemplates(ByRef res As DCSA.LoadAuthorityTemplates.Response,
                                            ByRef req As DCSA.LoadAuthorityTemplates.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAuthorityTemplates
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAuthorityTemplatesInSecurityGroup(ByRef res As DCSA.LoadAuthorityTemplatesInSecurityGroup.Response,
                                            ByRef req As DCSA.LoadAuthorityTemplatesInSecurityGroup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAuthorityTemplatesInSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAutomaticNonRenewalTransReasonCombo(ByRef res As DCSA.LoadAutomaticNonRenewalTransReasonCombo.Response,
                                            ByRef req As DCSA.LoadAutomaticNonRenewalTransReasonCombo.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAutomaticNonRenewalTransReasonCombo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableAgencyByCompanyStateLOB(ByRef res As DCSA.LoadAvailableAgencyByCompanyStateLOB.Response,
                                            ByRef req As DCSA.LoadAvailableAgencyByCompanyStateLOB.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableAgencyByCompanyStateLOB
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableAgyCommDetailType(ByRef res As DCSA.LoadAvailableAgyCommDetailType.Response,
                                            ByRef req As DCSA.LoadAvailableAgyCommDetailType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableAgyCommDetailType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function LoadAvailableBillingPayplans(ByRef res As DCSA.LoadAvailableBillingPayplans.Response,
        '                                    ByRef req As DCSA.LoadAvailableBillingPayplans.Request,
        '                                    Optional ByRef e As Exception = Nothing,
        '                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.AdministrationServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.LoadAvailableBillingPayplans
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadAvailableBillMethods(ByRef res As DCSA.LoadAvailableBillMethods.Response,
                                            ByRef req As DCSA.LoadAvailableBillMethods.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableBillMethods
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableBillTos(ByRef res As DCSA.LoadAvailableBillTos.Response,
                                            ByRef req As DCSA.LoadAvailableBillTos.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableBillTos
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableCompanyStateLobAgency(ByRef res As DCSA.LoadAvailableCompanyStateLobAgency.Response,
                                            ByRef req As DCSA.LoadAvailableCompanyStateLobAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableCompanyStateLobAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableCompanyStateLobReimbursement(ByRef res As DCSA.LoadAvailableCompanyStateLobReimbursement.Response,
                                            ByRef req As DCSA.LoadAvailableCompanyStateLobReimbursement.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableCompanyStateLobReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailablePolicyTerm(ByRef res As DCSA.LoadAvailablePolicyTerm.Response,
                                            ByRef req As DCSA.LoadAvailablePolicyTerm.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailablePolicyTerm
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableProducers(ByRef res As DCSA.LoadABTProducers.Response,
                                            ByRef req As DCSA.LoadABTProducers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableProducers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableTransferLobAgency(ByRef res As DCSA.LoadAvailableTransferLobAgency.Response,
                                            ByRef req As DCSA.LoadAvailableTransferLobAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableTransferLobAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBankAccount(ByRef res As DCSA.LoadBankAccount.Response,
                                            ByRef req As DCSA.LoadBankAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBankAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBankAccountCompanyLnk(ByRef res As DCSA.LoadBankAccountCompanyLnk.Response,
                                            ByRef req As DCSA.LoadBankAccountCompanyLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBankAccountCompanyLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBankAccountCompanyLnkUse(ByRef res As DCSA.LoadBankAccountCompanyLnkUse.Response,
                                            ByRef req As DCSA.LoadBankAccountCompanyLnkUse.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBankAccountCompanyLnkUse
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBankAccountLockboxAddressLnk(ByRef res As DCSA.LoadBankAccountLockboxAddressLnk.Response,
                                            ByRef req As DCSA.LoadBankAccountLockboxAddressLnk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBankAccountLockboxAddressLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBankAccountUse(ByRef res As DCSA.LoadBankAccountUse.Response,
                                            ByRef req As DCSA.LoadBankAccountUse.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBankAccountUse
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function LoadBillingRenewalPayPlanBillMethodVersion(ByRef res As DCSA.LoadBillingRenewalPayPlanBillMethodVersion.Response,
        '                                    ByRef req As DCSA.LoadBillingRenewalPayPlanBillMethodVersion.Request,
        '                                    Optional ByRef e As Exception = Nothing,
        '                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.AdministrationServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.LoadBillingRenewalPayPlanBillMethodVersion
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadBillMethods(ByRef res As DCSA.LoadBillMethods.Response,
                                            ByRef req As DCSA.LoadBillMethods.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillMethods
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBillMethodVersion(ByRef res As DCSA.LoadBillMethodVersion.Response,
                                            ByRef req As DCSA.LoadBillMethodVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillMethodVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBlackoutNonRenewPolicies(ByRef res As DCSA.LoadBlackoutNonRenewPolicies.Response,
                                            ByRef req As DCSA.LoadBlackoutNonRenewPolicies.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBlackoutNonRenewPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadComBankAccount(ByRef res As DCSA.LoadComBankAccount.Response,
                                            ByRef req As DCSA.LoadComBankAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadComBankAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCommission(ByRef res As DCSA.LoadCommission.Response,
                                            ByRef req As DCSA.LoadCommission.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCommission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCommissionDetail(ByRef res As DCSA.LoadCommissionDetail.Response,
                                            ByRef req As DCSA.LoadCommissionDetail.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCommissionDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCommissions(ByRef res As DCSA.LoadCommissions.Response,
                                            ByRef req As DCSA.LoadCommissions.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCommissions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCommissionsForValidation(ByRef res As DCSA.LoadCommissionsForValidation.Response,
                                            ByRef req As DCSA.LoadCommissionsForValidation.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCommissionsForValidation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCommissionTypes(ByRef res As DCSA.LoadCommissionTypes.Response,
                                            ByRef req As DCSA.LoadCommissionTypes.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCommissionTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompaniesOnAccount(ByRef res As DCSA.LoadCompaniesOnAccount.Response,
                                            ByRef req As DCSA.LoadCompaniesOnAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompaniesOnAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompanyBankAccounts(ByRef res As DCSA.LoadCompanyBankAccounts.Response,
                                            ByRef req As DCSA.LoadCompanyBankAccounts.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompanyBankAccounts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompanyName(ByRef res As DCSA.LoadCompanyName.Response,
                                            ByRef req As DCSA.LoadCompanyName.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompanyName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompanyStateLOB(ByRef res As DCSA.LoadCompanyStateLOB.Response,
                                            ByRef req As DCSA.LoadCompanyStateLOB.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompanyStateLOB
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadConfigAuthority(ByRef res As DCSA.LoadConfigAuthority.Response,
                                            ByRef req As DCSA.LoadConfigAuthority.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadConfigAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCounties(ByRef res As DCSA.LoadCounties.Response,
                                            ByRef req As DCSA.LoadCounties.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCounties
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCoverageCodeVersionRenewalRollOn(ByRef res As DCSA.LoadCoverageCodeVersionRenewalRollOn.Response,
                                            ByRef req As DCSA.LoadCoverageCodeVersionRenewalRollOn.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCoverageCodeVersionRenewalRollOn
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDetailSettingsByCSL(ByRef res As DCSA.LoadDetailSettingsByCSL.Response,
                                            ByRef req As DCSA.LoadDetailSettingsByCSL.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDetailSettingsByCSL
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDiamondUsers(ByRef res As DCSA.LoadDiamondUsers.Response,
                                            ByRef req As DCSA.LoadDiamondUsers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDiamondUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEditBankAccount(ByRef res As DCSA.LoadEditBankAccount.Response,
                                            ByRef req As DCSA.LoadEditBankAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEditBankAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEFTAccount(ByRef res As DCSA.LoadEFTAccount.Response,
                                            ByRef req As DCSA.LoadEFTAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEFTAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEFTTransactionType(ByRef res As DCSA.LoadEFTTransactionType.Response,
                                            ByRef req As DCSA.LoadEFTTransactionType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEFTTransactionType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEmployee(ByRef res As DCSA.LoadEmployee.Response,
                                            ByRef req As DCSA.LoadEmployee.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEmployee
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEntity(ByRef res As DCSA.LoadEntity.Response,
                                            ByRef req As DCSA.LoadEntity.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEntity
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadForAddSubLineLobCoverageCodeProgramTypeLink(ByRef res As DCSA.LoadForAddSubLineLobCoverageCodeProgramTypeLink.Response,
                                            ByRef req As DCSA.LoadForAddSubLineLobCoverageCodeProgramTypeLink.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadForAddSubLineLobCoverageCodeProgramTypeLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadGlobalAIAgencies(ByRef res As DCSA.LoadGlobalAIAgencies.Response,
                                            ByRef req As DCSA.LoadGlobalAIAgencies.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadGlobalAIAgencies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadHolidays(ByRef res As DCSA.LoadHolidays.Response,
                                            ByRef req As DCSA.LoadHolidays.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadHolidays
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadHurricaneBlackoutCounties(ByRef res As DCSA.LoadHurricaneBlackoutCounties.Response,
                                            ByRef req As DCSA.LoadHurricaneBlackoutCounties.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadHurricaneBlackoutCounties
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadHurricaneBlackoutDates(ByRef res As DCSA.LoadHurricaneBlackoutDates.Response,
                                            ByRef req As DCSA.LoadHurricaneBlackoutDates.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadHurricaneBlackoutDates
            res = RunDiamondService(m, req, e, dv)
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadIndividualAgency(ByRef res As DCSA.LoadAgency.Response,
                                            ByRef req As DCSA.LoadAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadIndividualAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadIndividualAgencyLob(ByRef res As DCSA.LoadIndividualAgencyLob.Response,
                                            ByRef req As DCSA.LoadIndividualAgencyLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadIndividualAgencyLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadIndividualAgyCommDetailType(ByRef res As DCSA.LoadIndividualAgyCommDetailType.Response,
                                            ByRef req As DCSA.LoadIndividualAgyCommDetailType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadIndividualAgyCommDetailType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadIndividualAuthorityGroup(ByRef res As DCSA.LoadIndividualAuthorityGroup.Response,
                                            ByRef req As DCSA.LoadIndividualAuthorityGroup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadIndividualAuthorityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadIndividualCompany(ByRef res As DCSA.LoadIndividualCompany.Response,
                                            ByRef req As DCSA.LoadIndividualCompany.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadIndividualCompany
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInvalidInstallmentNumbers(ByRef res As DCSA.LoadInvalidInstallmentNumbers.Response,
                                            ByRef req As DCSA.LoadInvalidInstallmentNumbers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInvalidInstallmentNumbers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadList(ByRef res As DCSA.LoadList.Response,
                                            ByRef req As DCSA.LoadList.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLOBPerCompanyState(ByRef res As DCSA.LoadLOBPerCompanyState.Response,
                                            ByRef req As DCSA.LoadLOBPerCompanyState.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLOBPerCompanyState
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadMobileHomeParkFor(ByRef res As DCSA.LoadMobileHomeParkFor.Response,
                                            ByRef req As DCSA.LoadMobileHomeParkFor.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadMobileHomeParkFor
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadMultipleAgencyBookTransfers(ByRef res As DCSA.LoadMultipleAgencyBookTransfers.Response,
                                            ByRef req As DCSA.LoadMultipleAgencyBookTransfers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadMultipleAgencyBookTransfers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadNotesType(ByRef res As DCSA.LoadNotesType.Response,
                                            ByRef req As DCSA.LoadNotesType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadNotesType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadOtherCarrier(ByRef res As DCSA.LoadOtherCarrier.Response,
                                            ByRef req As DCSA.LoadOtherCarrier.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadOtherCarrier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadOtherCarrierNotInAgency(ByRef res As DCSA.LoadOtherCarrierNotInAgency.Response,
                                            ByRef req As DCSA.LoadOtherCarrierNotInAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadOtherCarrierNotInAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadOtherInstallments(ByRef res As DCSA.LoadOtherInstallments.Response,
                                            ByRef req As DCSA.LoadOtherInstallments.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadOtherInstallments
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayrollDeductionEmployer(ByRef res As DCSA.LoadPayrollDeductionEmployer.Response,
                                            ByRef req As DCSA.LoadPayrollDeductionEmployer.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPayrollDeductionEmployer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayrollDeductionEmployers(ByRef res As DCSA.LoadPayrollDeductionEmployers.Response,
                                            ByRef req As DCSA.LoadPayrollDeductionEmployers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPayrollDeductionEmployers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPolicyTermVersion(ByRef res As DCSA.LoadPolicyTermVersion.Response,
                                            ByRef req As DCSA.LoadPolicyTermVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPolicyTermVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPrimaryAgencyAddress(ByRef res As DCSA.LoadPrimaryAgencyAddress.Response,
                                            ByRef req As DCSA.LoadPrimaryAgencyAddress.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPrimaryAgencyAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadQueues(ByRef res As DCSA.LoadQueues.Response,
                                            ByRef req As DCSA.LoadQueues.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRatingDisk(ByRef res As DCSA.LoadRatingDisk.Response,
                                            ByRef req As DCSA.LoadRatingDisk.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRatingDisk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRatingVersion(ByRef res As DCSA.LoadRatingVersion.Response,
                                            ByRef req As DCSA.LoadRatingVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRatingVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadReimbursement(ByRef res As DCSA.LoadReimbursement.Response,
                                            ByRef req As DCSA.LoadReimbursement.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadReinsurance(ByRef res As DCSA.LoadReinsurance.Response,
                                            ByRef req As DCSA.LoadReinsurance.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReinsurance
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadReinsuranceCoverages(ByRef res As DCSA.LoadReinsuranceCoverages.Response,
                                            ByRef req As DCSA.LoadReinsuranceCoverages.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReinsuranceCoverages
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRenewalRollOn(ByRef res As DCSA.LoadRenewalRollOn.Response,
                                            ByRef req As DCSA.LoadRenewalRollOn.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRenewalRollOn
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadReport(ByRef res As DCSA.LoadReport.Response,
                                            ByRef req As DCSA.LoadReport.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadReportList(ByRef res As DCSA.LoadReportList.Response,
                                            ByRef req As DCSA.LoadReportList.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReportList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSecondaryAgencyAddress(ByRef res As DCSA.LoadSecondaryAgencyAddress.Response,
                                            ByRef req As DCSA.LoadSecondaryAgencyAddress.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSecondaryAgencyAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSecurityGroups(ByRef res As DCSA.LoadSecurityGroups.Response,
                                            ByRef req As DCSA.LoadSecurityGroups.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSecurityGroups
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSecurityGroupsForUser(ByRef res As DCSA.LoadSecurityGroupsForUser.Response,
                                            ByRef req As DCSA.LoadSecurityGroupsForUser.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSecurityGroupsForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSelectedAgency(ByRef res As DCSA.LoadSelectedAgency.Response,
                                            ByRef req As DCSA.LoadSelectedAgency.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSelectedAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSelectedEFTAccount(ByRef res As DCSA.LoadSelectedEFTAccount.Response,
                                            ByRef req As DCSA.LoadSelectedEFTAccount.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSelectedEFTAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSelectedProducers(ByRef res As DCSA.LoadABTProducers.Response,
                                            ByRef req As DCSA.LoadABTProducers.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSelectedProducers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleBillMethodVersion(ByRef res As DCSA.LoadSingleBillMethodVersion.Response,
                                            ByRef req As DCSA.LoadSingleBillMethodVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleBillMethodVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleCompanyStateLob(ByRef res As DCSA.LoadSingleCompanyStateLob.Response,
                                            ByRef req As DCSA.LoadSingleCompanyStateLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleCompanyStateLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleReimbursement(ByRef res As DCSA.LoadSingleReimbursement.Response,
                                            ByRef req As DCSA.LoadSingleReimbursement.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleVersion(ByRef res As DCSA.LoadSingleVersion.Response,
                                            ByRef req As DCSA.LoadSingleVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSoldToAgencyList(ByRef res As DCSA.LoadSoldToAgencyList.Response,
                                            ByRef req As DCSA.LoadSoldToAgencyList.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSoldToAgencyList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadStatePerCompany(ByRef res As DCSA.LoadStatePerCompany.Response,
                                            ByRef req As DCSA.LoadStatePerCompany.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadStatePerCompany
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSubLineLobCoverageCodeProgramTypeLink(ByRef res As DCSA.LoadSubLineLobCoverageCodeProgramTypeLinks.Response,
                                            ByRef req As DCSA.LoadSubLineLobCoverageCodeProgramTypeLinks.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSubLineLobCoverageCodeProgramTypeLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSuspenseAccounts(ByRef res As DCSA.LoadSuspenseAccounts.Response,
                                            ByRef req As DCSA.LoadSuspenseAccounts.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSuspenseAccounts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTransactionReasonViewableByUserCategory(ByRef res As DCSA.LoadTransactionReasonViewableByUserCategory.Response,
                                            ByRef req As DCSA.LoadTransactionReasonViewableByUserCategory.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTransactionReasonViewableByUserCategory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTransactionReasonViewableByUserCategorySetup(ByRef res As DCSA.LoadTransactionReasonViewableByUserCategorySetup.Response,
                                            ByRef req As DCSA.LoadTransactionReasonViewableByUserCategorySetup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTransactionReasonViewableByUserCategorySetup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUnderwritingVersion(ByRef res As DCSA.LoadUnderwritingVersion.Response,
                                            ByRef req As DCSA.LoadUnderwritingVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUnderwritingVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUserAddress(ByRef res As DCSA.LoadUserAddress.Response,
                                            ByRef req As DCSA.LoadUserAddress.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUserAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUserClientName(ByRef res As DCSA.LoadUserClientName.Response,
                                            ByRef req As DCSA.LoadUserClientName.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUserClientName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUserLinks(ByRef res As DCSA.LoadUserLinks.Response,
                                            ByRef req As DCSA.LoadUserLinks.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUserLinks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUserName(ByRef res As DCSA.LoadUserName.Response,
                                            ByRef req As DCSA.LoadUserName.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUserName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUsersAllAuthorities(ByRef res As DCSA.LoadUsersAllAuthorities.Response,
                                            ByRef req As DCSA.LoadUsersAllAuthorities.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUsersAllAuthorities
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUsersAuthorities(ByRef res As DCSA.LoadUsersAuthorities.Response,
                                            ByRef req As DCSA.LoadUsersAuthorities.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUsersAuthorities
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUsersCompanyBranchDeptUserType(ByRef res As DCSA.LoadUsersCompanyBranchDeptUserType.Response,
                                            ByRef req As DCSA.LoadUsersCompanyBranchDeptUserType.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUsersCompanyBranchDeptUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUserSecurityQuestions(ByRef res As DCSA.LoadUserSecurityQuestions.Response,
                                            ByRef req As DCSA.LoadUserSecurityQuestions.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUserSecurityQuestions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUsersInSecurityGroup(ByRef res As DCSA.LoadUsersInSecurityGroup.Response,
                                            ByRef req As DCSA.LoadUsersInSecurityGroup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUsersInSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadUsersUserSecurityQuestionLink(ByRef res As DCSA.LoadUsersUserSecurityQuestionLink.Response,
                                            ByRef req As DCSA.LoadUsersUserSecurityQuestionLink.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadUsersUserSecurityQuestionLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadVersion(ByRef res As DCSA.LoadVersion.Response,
                                            ByRef req As DCSA.LoadVersion.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadvUser(ByRef res As DCSA.LoadvUser.Response,
                                            ByRef req As DCSA.LoadvUser.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadvUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadWorkflowQueues(ByRef res As DCSA.LoadWorkflowQueues.Response,
                                           ByRef req As DCSA.LoadWorkflowQueues.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadWorkflowQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LockBoxDataGrid(ByRef res As DCSA.LockBoxDataGrid.Response,
                                           ByRef req As DCSA.LockBoxDataGrid.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LockBoxDataGrid
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LookupAdminMHPark(ByRef res As DCSA.LookupAdminMHPark.Response,
                                           ByRef req As DCSA.LookupAdminMHPark.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LookupAdminMHPark
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Reinsurance(ByRef res As DCSA.Reinsurance.Response,
                                           ByRef req As DCSA.Reinsurance.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Reinsurance
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveAgencyTransferProducer(ByRef res As DCSA.RemoveAgencyTransferProducer.Response,
                                           ByRef req As DCSA.RemoveAgencyTransferProducer.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveAgencyTransferProducer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveAuthorityFromAllUsersInUserType(ByRef res As DCSA.RemoveAuthorityFromAllUsersInUserType.Response,
                                           ByRef req As DCSA.RemoveAuthorityFromAllUsersInUserType.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveAuthorityFromAllUsersInUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveAuthorityTemplateAuthority(ByRef res As DCSA.RemoveAuthorityTemplateAuthority.Response,
                                           ByRef req As DCSA.RemoveAuthorityTemplateAuthority.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveAuthorityTemplateAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveAuthorityTemplatesFromSecurityGroup(ByRef res As DCSA.RemoveAuthorityTemplatesFromSecurityGroup.Response,
                                           ByRef req As DCSA.RemoveAuthorityTemplatesFromSecurityGroup.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveAuthorityTemplatesFromSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveUsersAuthority(ByRef res As DCSA.RemoveUsersAuthority.Response,
                                           ByRef req As DCSA.RemoveUsersAuthority.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveUsersAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveUsersFromSecurityGroup(ByRef res As DCSA.RemoveUsersFromSecurityGroup.Response,
                                           ByRef req As DCSA.RemoveUsersFromSecurityGroup.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveUsersFromSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveUserTypeAuthority(ByRef res As DCSA.RemoveUserTypeAuthority.Response,
                                           ByRef req As DCSA.RemoveUserTypeAuthority.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RemoveUserTypeAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAddFormsVersion(ByRef res As DCSA.SaveAddFormsVersion.Response,
                                           ByRef req As DCSA.SaveAddFormsVersion.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAddFormsVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAdminMHPark(ByRef res As DCSA.SaveAdminMHPark.Response,
                                           ByRef req As DCSA.SaveAdminMHPark.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAdminMHPark
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgency(ByRef res As DCSA.SaveAgency.Response,
                                           ByRef req As DCSA.SaveAgency.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyBookTransfer(ByRef res As DCSA.SaveAgencyBookTransfer.Response,
                                           ByRef req As DCSA.SaveAgencyBookTransfer.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyBookTransfer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyCommission(ByRef res As DCSA.SaveAgencyCommission.Response,
                                           ByRef req As DCSA.SaveAgencyCommission.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyCommission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyCommissionDetail(ByRef res As DCSA.SaveAgencyCommissionDetail.Response,
                                           ByRef req As DCSA.SaveAgencyCommissionDetail.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyCommissionDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyCommissionDetailType(ByRef res As DCSA.SaveAgencyCommissionDetailType.Response,
                                           ByRef req As DCSA.SaveAgencyCommissionDetailType.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyCommissionDetailType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyGroup(ByRef res As DCSA.SaveAgencyGroup.Response,
                                           ByRef req As DCSA.SaveAgencyGroup.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyLOB(ByRef res As DCSA.SaveAgencyLOB.Response,
                                           ByRef req As DCSA.SaveAgencyLOB.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyLOB
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyLobDownload(ByRef res As DCSA.SaveAgencyLobDownload.Response,
                                           ByRef req As DCSA.SaveAgencyLobDownload.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyLobDownload
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyOtherCarrierNameLink(ByRef res As DCSA.SaveAgencyOtherCarrierNameLink.Response,
                                           ByRef req As DCSA.SaveAgencyOtherCarrierNameLink.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyOtherCarrierNameLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyPersonnel(ByRef res As DCSA.SaveAgencyPersonnel.Response,
                                           ByRef req As DCSA.SaveAgencyPersonnel.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyProducer(ByRef res As DCSA.SaveAgencyProducer.Response,
                                           ByRef req As DCSA.SaveAgencyProducer.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyProducer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyQueues(ByRef res As DCSA.SaveAgencyQueues.Response,
                                           ByRef req As DCSA.SaveAgencyQueues.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyReimbursement(ByRef res As DCSA.SaveAgencyReimbursement.Response,
                                           ByRef req As DCSA.SaveAgencyReimbursement.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyUserName(ByRef res As DCSA.SaveAgencyUserName.Response,
                                           ByRef req As DCSA.SaveAgencyUserName.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyUserName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAuthorityTemplate(ByRef res As DCSA.SaveAuthorityTemplate.Response,
                                           ByRef req As DCSA.SaveAuthorityTemplate.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAuthorityTemplate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAuthorityTemplateAuthority(ByRef res As DCSA.SaveAuthorityTemplateAuthority.Response,
                                           ByRef req As DCSA.SaveAuthorityTemplateAuthority.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAuthorityTemplateAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBankAccount(ByRef res As DCSA.SaveBankAccount.Response,
                                           ByRef req As DCSA.SaveBankAccount.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBankAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBankAccountCompanyLnk(ByRef res As DCSA.SaveBankAccountCompanyLnk.Response,
                                           ByRef req As DCSA.SaveBankAccountCompanyLnk.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBankAccountCompanyLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBankAccountLockboxAddressLnk(ByRef res As DCSA.SaveBankAccountLockboxAddressLnk.Response,
                                           ByRef req As DCSA.SaveBankAccountLockboxAddressLnk.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBankAccountLockboxAddressLnk
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBankAccountUse(ByRef res As DCSA.SaveBankAccountUse.Response,
                                           ByRef req As DCSA.SaveBankAccountUse.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBankAccountUse
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBillingPayPlan(ByRef res As DCSA.SaveBillingPayPlan.Response,
                                           ByRef req As DCSA.SaveBillingPayPlan.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBillingPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBillingPayPlanInstallment(ByRef res As DCSA.SaveBillingPayPlanInstallment.Response,
                                           ByRef req As DCSA.SaveBillingPayPlanInstallment.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBillingPayPlanInstallment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function SaveBillingRenewalPayPlanBillMethodVersion(ByRef res As DCSA.SaveBillingRenewalPayPlanBillMethodVersion.Response,
        '                                   ByRef req As DCSA.SaveBillingRenewalPayPlanBillMethodVersion.Request,
        '                                   Optional ByRef e As Exception = Nothing,
        '                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.AdministrationServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.SaveBillingRenewalPayPlanBillMethodVersion
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function SaveBranch(ByRef res As DCSA.SaveBranch.Response,
                                           ByRef req As DCSA.SaveBranch.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBranch
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCBDUserType(ByRef res As DCSA.SaveCBDUserType.Response,
                                           ByRef req As DCSA.SaveCBDUserType.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCBDUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCompany(ByRef res As DCSA.SaveCompany.Response,
                                           ByRef req As DCSA.SaveCompany.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCompany
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCompanyStateLobReimbursement(ByRef res As DCSA.SaveCompanyStateLobReimbursement.Response,
                                           ByRef req As DCSA.SaveCompanyStateLobReimbursement.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCompanyStateLobReimbursement
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveConfigAuthority(ByRef res As DCSA.SaveConfigAuthority.Response,
                                           ByRef req As DCSA.SaveConfigAuthority.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveConfigAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDepartment(ByRef res As DCSA.SaveDepartment.Response,
                                           ByRef req As DCSA.SaveDepartment.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDepartment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDetailSettingsVersion(ByRef res As DCSA.SaveDetailSettingsVersion.Response,
                                           ByRef req As DCSA.SaveDetailSettingsVersion.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDetailSettingsVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveEFTAccount(ByRef res As DCSA.SaveEFTAccount.Response,
                                           ByRef req As DCSA.SaveEFTAccount.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveEFTAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveEmployee(ByRef res As DCSA.SaveEmployee.Response,
                                           ByRef req As DCSA.SaveEmployee.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveEmployee
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveHoliday(ByRef res As DCSA.SaveHoliday.Response,
                                           ByRef req As DCSA.SaveHoliday.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveHoliday
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveHurricaneBlackoutDates(ByRef res As DCSA.SaveHurricaneBlackoutDates.Response,
                                           ByRef req As DCSA.SaveHurricaneBlackoutDates.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveHurricaneBlackoutDates
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLob(ByRef res As DCSA.SaveLOB.Response,
                                           ByRef req As DCSA.SaveLOB.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLockboxAddress(ByRef res As DCSA.SaveLockboxAddress.Response,
                                           ByRef req As DCSA.SaveLockboxAddress.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLockboxAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveMultipleAgencyBookTransfers(ByRef res As DCSA.SaveMultipleAgencyBookTransfers.Response,
                                           ByRef req As DCSA.SaveMultipleAgencyBookTransfers.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveMultipleAgencyBookTransfers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveNotesType(ByRef res As DCSA.SaveNotesType.Response,
                                           ByRef req As DCSA.SaveNotesType.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveNotesType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveOtherCarrier(ByRef res As DCSA.SaveOtherCarrier.Response,
                                           ByRef req As DCSA.SaveOtherCarrier.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveOtherCarrier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePayrollDeductionEmployer(ByRef res As DCSA.SavePayrollDeductionEmployer.Response,
                                           ByRef req As DCSA.SavePayrollDeductionEmployer.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePayrollDeductionEmployer
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePolicyTermVersion(ByRef res As DCSA.SavePolicyTermVersion.Response,
                                           ByRef req As DCSA.SavePolicyTermVersion.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePolicyTermVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveQueue(ByRef res As DCSA.SaveQueue.Response,
                                           ByRef req As DCSA.SaveQueue.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveQueue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveRatingVersion(ByRef res As DCSA.SaveRatingVersion.Response,
                                           ByRef req As DCSA.SaveRatingVersion.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveRatingVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveRenewalRollOn(ByRef res As DCSA.SaveRenewalRollOn.Response,
                                           ByRef req As DCSA.SaveRenewalRollOn.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveRenewalRollOn
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveReport(ByRef res As DCSA.SaveReport.Response,
                                           ByRef req As DCSA.SaveReport.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveReport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSecurityGroup(ByRef res As DCSA.SaveSecurityGroup.Response,
                                           ByRef req As DCSA.SaveSecurityGroup.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSecurityGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSubLineLobCoverageCodeProgramTypeLink(ByRef res As DCSA.SaveSubLineLobCoverageCodeProgramTypeLink.Response,
                                           ByRef req As DCSA.SaveSubLineLobCoverageCodeProgramTypeLink.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSubLineLobCoverageCodeProgramTypeLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSuspenseAccount(ByRef res As DCSA.SaveSuspenseAccount.Response,
                                           ByRef req As DCSA.SaveSuspenseAccount.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSuspenseAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUnderwritingVersion(ByRef res As DCSA.SaveUnderwritingVersion.Response,
                                           ByRef req As DCSA.SaveUnderwritingVersion.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUnderwritingVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUser(ByRef res As DCSA.SaveUser.Response,
                                           ByRef req As DCSA.SaveUser.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUserQueues(ByRef res As DCSA.SaveUserQueues.Response,
                                       ByRef req As DCSA.SaveUserQueues.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUserQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUsersAuthority(ByRef res As DCSA.SaveUsersAuthority.Response,
                                       ByRef req As DCSA.SaveUsersAuthority.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUsersAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUserSecurityQuestion(ByRef res As DCSA.SaveUserSecurityQuestion.Response,
                                       ByRef req As DCSA.SaveUserSecurityQuestion.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUserSecurityQuestion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUsersUserSecurityQuestionLink(ByRef res As DCSA.SaveUsersUserSecurityQuestionLink.Response,
                                       ByRef req As DCSA.SaveUsersUserSecurityQuestionLink.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUsersUserSecurityQuestionLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUserType(ByRef res As DCSA.SaveUserType.Response,
                                       ByRef req As DCSA.SaveUserType.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUserType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveUserTypeAuthority(ByRef res As DCSA.SaveUserTypeAuthority.Response,
                                       ByRef req As DCSA.SaveUserTypeAuthority.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveUserTypeAuthority
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveVersion(ByRef res As DCSA.SaveVersion.Response,
                                       ByRef req As DCSA.SaveVersion.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveWorkflowQueue(ByRef res As DCSA.SaveWorkflowQueue.Response,
                                       ByRef req As DCSA.SaveWorkflowQueue.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveWorkflowQueue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SetGrantedAuthorityToDefault(ByRef res As DCSA.SetGrantedAuthorityToDefault.Response,
                                       ByRef req As DCSA.SetGrantedAuthorityToDefault.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SetGrantedAuthorityToDefault
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateHurricaneBlackoutPolicies(ByRef res As DCSA.UpdateHurricaneBlackoutPolicies.Response,
                                       ByRef req As DCSA.UpdateHurricaneBlackoutPolicies.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateHurricaneBlackoutPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateNonRenewBlackoutPolicy(ByRef res As DCSA.UpdateNonRenewBlackoutPolicy.Response,
                                       ByRef req As DCSA.UpdateNonRenewBlackoutPolicy.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateNonRenewBlackoutPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UserCodeExists(ByRef res As DCSA.UserCodeExists.Response,
                                       ByRef req As DCSA.UserCodeExists.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UserCodeExists
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidConnectionForClaimExposureVersion(ByRef res As DCSA.ValidConnectionForClaimExposureVersion.Response,
                                                               ByRef req As DCSA.ValidConnectionForClaimExposureVersion.Request,
                                                               Optional ByRef e As Exception = Nothing,
                                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidConnectionForClaimExposureVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

