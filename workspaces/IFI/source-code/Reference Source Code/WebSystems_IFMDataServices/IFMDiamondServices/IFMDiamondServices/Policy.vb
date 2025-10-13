Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Policy
    Public Module Policy

        Public Function AcceptRejectQuote(AcceptQuote As Boolean,
                                          EmailAddress As String,
                                          PolicyID As Integer,
                                          PolicyImageNum As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.AcceptRejectQuote.Response
            Dim req As New DCSM.PolicyService.AcceptRejectQuote.Request

            With req.RequestData
                .AcceptQuote = AcceptQuote
                .EmailAddress = EmailAddress
                .PolicyId = PolicyID
                .PolicyImageNum = PolicyImageNum
            End With
            'TODO: Check returns for possible nullreference exceptions. IE: res.ResponseData is Nothing
            If IFMS.Policy.AcceptRejectQuote(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function AcquirePendingImage(PolicyID As Integer,
                                            PolicyImageNum As Integer,
                                            UserId As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.AcquirePendingImage.Response
            Dim req As New DCSM.PolicyService.AcquirePendingImage.Request

            With req.RequestData
                .PolicyId = PolicyID
                .PolicyImageNum = PolicyImageNum
                .UsersId = UserId
            End With

            If IFMS.Policy.AcquirePendingImage(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function ApplyExperienceModificationFactor(LookupResult As DCO.Policy.QuickLookup,
                                                          Record As DCO.ExperienceModifications.ExperienceModificationFileRecord,
                                                          Optional ByRef e As System.Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ExperienceModifications.ExperienceModificationFileRecord
            Dim res As New DCSM.PolicyService.ApplyExperienceModificationFactor.Response
            Dim req As New DCSM.PolicyService.ApplyExperienceModificationFactor.Request

            With req.RequestData
                .LookupResult = LookupResult
                .Record = Record
            End With

            If IFMS.Policy.ApplyExperienceModificationFactor(res, req, e, dv) Then
                Return res.ResponseData.Record
            End If
            Return Nothing
        End Function

        Public Function AttachForms(PolicyId As Integer,
                                    PolicyImageNumber As Integer,
                                    VersionId As Integer,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.PolicyForms.PolicyFormView)
            Dim res As New DCSM.PolicyService.AttachForms.Response
            Dim req As New DCSM.PolicyService.AttachForms.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNumber = PolicyImageNumber
                .VersionId = VersionId
            End With

            If IFMS.Policy.AttachForms(res, req, e, dv) Then
                Return res.ResponseData.Forms
            End If
            Return Nothing
        End Function

        Public Function Binder(Bind As Boolean,
                               BindToDate As Date,
                               BindToTime As Date,
                               PolicyId As Integer,
                               PolicyImageNum As Integer,
                               Remark As String,
                               TransactionReasonId As Integer,
                               TransactionTypeId As Integer,
                               Optional ByRef e As System.Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.Binder.Response
            Dim req As New DCSM.PolicyService.Binder.Request

            With req.RequestData
                .Bind = Bind
                .BindToDate = BindToDate
                .BindToTime = BindToTime
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .Remark = Remark
                .TransactionReasonId = TransactionReasonId
                .TransactionTypeId = TransactionTypeId
            End With

            If IFMS.Policy.Binder(res, req, e, dv) Then
                Return res.ResponseData.OperationSuccessful
            End If
            Return Nothing
        End Function

        Public Function ConvertPolicyForRenewal(PolicyId As Integer,
                                                PolicyImageNum As Integer,
                                                Reapplying As Boolean,
                                                TransType As DCE.TransType,
                                                VersionId As Integer,
                                                VersionInvalid As Boolean,
                                                Optional ByRef e As System.Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.ConvertPolicyForRenewal.Response
            Dim req As New DCSM.PolicyService.ConvertPolicyForRenewal.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .Reapplying = Reapplying
                .TransType = TransType
                .VersionId = VersionId
                .VersionInvalid = VersionInvalid
            End With

            If IFMS.Policy.ConvertPolicyForRenewal(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function ConvertPolicyFromQuoteToApplication(Image As DCO.Policy.Image,
                                                            Optional ByRef e As System.Exception = Nothing,
                                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.ConvertPolicyFromQuoteToApplication.Response
            Dim req As New DCSM.PolicyService.ConvertPolicyFromQuoteToApplication.Request

            With req.RequestData
                .Image = Image
            End With

            If IFMS.Policy.ConvertPolicyFromQuoteToApplication(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
            Return Nothing
        End Function

        Public Function CopyQuoteToNewQuote(PolicyId As Integer,
                                            PolicyImageNum As Integer,
                                            UserId As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.CopyQuoteToNewQuote.Response
            Dim req As New DCSM.PolicyService.CopyQuoteToNewQuote.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UsersId = UserId
            End With

            If IFMS.Policy.CopyQuoteToNewQuote(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function CoverageDeleteWarning(EditCollection As DCO.InsCollection(Of DCO.Coverage),
                                              EditScheduledCollection As DCO.InsCollection(Of DCO.Policy.ScheduledCoverage),
                                              IsDebugIdMode As Boolean,
                                              IsDeleteCoverage As Boolean,
                                              LobData As DCO.Policy.LOB,
                                              VersionId As Integer,
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.CoverageDeleteWarning.ResponseData
            Dim res As New DCSM.PolicyService.CoverageDeleteWarning.Response
            Dim req As New DCSM.PolicyService.CoverageDeleteWarning.Request

            With req.RequestData
                .EditCollection = EditCollection
                .EditScheduledCollection = EditScheduledCollection
                .IsDebugIdMode = IsDebugIdMode
                .IsDeleteCoverage = IsDeleteCoverage
                .LobData = LobData
                .VersionId = VersionId
            End With

            If IFMS.Policy.CoverageDeleteWarning(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function CreateNewPackageParts(PackagePartTypeIds As Integer(),
                                              PolicyId As Integer,
                                              PolicyImageNum As Integer,
                                              SaveToDB As Boolean,
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.CreateNewPackageParts.ResponseData
            Dim res As New DCSM.PolicyService.CreateNewPackageParts.Response
            Dim req As New DCSM.PolicyService.CreateNewPackageParts.Request

            With req.RequestData
                .PackagePartTypeIds = PackagePartTypeIds
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .SaveToDB = SaveToDB
            End With

            If IFMS.Policy.CreateNewPackageParts(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function DataFill(Criteria As DCSM.PolicyService.DataFill.ChoicePointDataFillCriteria,
                                 Image As DCO.Policy.Image,
                                 Optional ByRef e As System.Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.DataFill.Response
            Dim req As New DCSM.PolicyService.DataFill.Request

            With req.RequestData
                .Criteria = Criteria
                .Image = Image
            End With

            If IFMS.Policy.DataFill(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
                Return Nothing
        End Function

        Public Function DefaultFirstLocation(PolicyImage As DCO.Policy.Image,
                                             ShouldClearAndDefault As Boolean,
                                             UseAsDefaultForFirstAtRiskLevel As Boolean,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.DefaultFirstLocation.Response
            Dim req As New DCSM.PolicyService.DefaultFirstLocation.Request

            With req.RequestData
                .PolicyImage = PolicyImage
                .ShouldClearAndDefault = ShouldClearAndDefault
                .UseAsDefaultForFirstAtRiskLevel = UseAsDefaultForFirstAtRiskLevel
            End With

            If IFMS.Policy.DefaultFirstLocation(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function DefaultSubmitInfo(Image As DCO.Policy.Image,
                                          VersionId As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.DefaultSubmitInfo.Response
            Dim req As New DCSM.PolicyService.DefaultSubmitInfo.Request

            With req.RequestData
                .Image = Image
                .VersionId = VersionId
            End With

            If IFMS.Policy.DefaultSubmitInfo(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
            Return Nothing
        End Function

        Public Function DeleteClient(ClientId As Integer,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.DeleteClient.Response
            Dim req As New DCSM.PolicyService.DeleteClient.Request

            With req.RequestData
                .ClientId = ClientId
            End With

            If IFMS.Policy.DeleteClient(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function DeleteFutureEvents(DeleteByAutomaticProcess As Boolean,
                                           FutureEventsNum As Integer,
                                           FutureEventsTypeId As Integer,
                                           PolicyId As Integer,
                                           udtError As Object,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.DeleteFutureEvents.DataItem)
            Dim res As New DCSM.PolicyService.DeleteFutureEvents.Response
            Dim req As New DCSM.PolicyService.DeleteFutureEvents.Request

            With req.RequestData
                .DeleteByAutomaticProcess = DeleteByAutomaticProcess
                .FutureEventsNum = FutureEventsNum
                .FutureEventsTypeId = FutureEventsTypeId
                .PolicyId = PolicyId
                .udtError = udtError
            End With

            If IFMS.Policy.DeleteFutureEvents(res, req, e, dv) Then
                Return res.ResponseData.DataItems
            End If
            Return Nothing
        End Function

        Public Function DeleteOutsideAuditor(OutsideAuditorId As Integer,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.DeleteOutsideAuditor.Response
            Dim req As New DCSM.PolicyService.DeleteOutsideAuditor.Request

            With req.RequestData
                .OutsideAuditorId = OutsideAuditorId
            End With

            If IFMS.Policy.DeleteOutsideAuditor(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function DeletePendingImage(PolicyId As Integer,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.DeletePendingImage.Response
            Dim req As New DCSM.PolicyService.DeletePendingImage.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.DeletePendingImage(res, req, e, dv) Then
                Return res.ResponseData.Result
            End If
            Return Nothing
        End Function

        Public Function DeletePolicySearchTypeViewableByUserCategory(Item As DCO.Administration.PolicySearchType,
                                                                     Optional ByRef e As System.Exception = Nothing,
                                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.DeletePolicySearchTypeViewableByUserCategory.Response
            Dim req As New DCSM.PolicyService.DeletePolicySearchTypeViewableByUserCategory.Request

            With req.RequestData
                .Item = Item
            End With

            If IFMS.Policy.DeletePolicySearchTypeViewableByUserCategory(res, req, e, dv) Then
                Return res.ResponseData.Result
            End If
            Return Nothing
        End Function

        Public Function DeleteQuote(LoadLastImage As Boolean,
                                    PolicyId As Integer,
                                    PolicyImageNum As Integer,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.DeleteQuote.Response
            Dim req As New DCSM.PolicyService.DeleteQuote.Request

            With req.RequestData
                .LoadLastImage = LoadLastImage
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.DeleteQuote(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function DemotePendingToQuote(LoadDemoteImage As Boolean,
                                             PolicyId As Integer,
                                             PolicyImageNum As Integer,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.DemotePendingToQuote.Response
            Dim req As New DCSM.PolicyService.DemotePendingToQuote.Request

            With req.RequestData
                .LoadDemotedImage = LoadDemoteImage
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.DemotePendingToQuote(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function GetActiveRisksByClientId(ClientId As Integer,
                                                 IncludeDrivers As Boolean,
                                                 IncludeLocations As Boolean,
                                                 IncludeVehicles As Boolean,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim res As New DCSM.PolicyService.GetActiveRisksByClientId.Response
            Dim req As New DCSM.PolicyService.GetActiveRisksByClientId.Request

            With req.RequestData
                .ClientId = ClientId
                .IncludeDrivers = IncludeDrivers
                .IncludeLocations = IncludeLocations
                .IncludeVehicles = IncludeVehicles
            End With

            If IFMS.Policy.GetActiveRisksByClientId(res, req, e, dv) Then
                Return res.ResponseData.RisksXml
            End If
            Return Nothing
        End Function

        Public Function GetAddress(NameAddressSourceId As DCE.NameAddressSource,
                                   PolicyId As Integer,
                                   PolicyImageNum As Integer,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Address)
            Dim res As New DCSM.PolicyService.GetAddress.Response
            Dim req As New DCSM.PolicyService.GetAddress.Request

            With req.RequestData
                .NameAddressSourceId = NameAddressSourceId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.GetAddress(res, req, e, dv) Then
                Return res.ResponseData.Addresses
            End If
            Return Nothing
        End Function

        Public Function GetAgencyData(AgencyId As Integer,
                                      Code As String,
                                      CompanyId As Integer,
                                      LobId As Integer,
                                      StateId As Integer,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.AgencyData)
            Dim res As New DCSM.PolicyService.GetAgencyData.Response
            Dim req As New DCSM.PolicyService.GetAgencyData.Request

            With req.RequestData
                .AgencyId = AgencyId
                .Code = Code
                .CompanyId = CompanyId
                .LobId = LobId
                .StateId = StateId
            End With

            If IFMS.Policy.GetAgencyData(res, req, e, dv) Then
                Return res.ResponseData.Agencies
            End If
            Return Nothing
        End Function

        Public Function GetClientPolicies(PolicyId As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.GetClientPolicies.Policy)
            Dim res As New DCSM.PolicyService.GetClientPolicies.Response
            Dim req As New DCSM.PolicyService.GetClientPolicies.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.GetClientPolicies(res, req, e, dv) Then
                Return res.ResponseData.Policies
            End If
            Return Nothing
        End Function

        Public Function GetClientPolicyInfo(PolicyImage As DCO.Policy.Image,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.GetClientPolicyInfo.Response
            Dim req As New DCSM.PolicyService.GetClientPolicyInfo.Request

            With req.RequestData
                .PolicyImage = PolicyImage
            End With

            If IFMS.Policy.GetClientPolicyInfo(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function GetFirstWrittenDate(PolicyNumber As String,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Date
            Dim res As New DCSM.PolicyService.GetFirstWrittenDate.Response
            Dim req As New DCSM.PolicyService.GetFirstWrittenDate.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.GetFirstWrittenDate(res, req, e, dv) Then
                Return res.ResponseData.FirstWrittenDate
            End If
            Return Nothing
        End Function

        Public Function GetNAICSData(FieldName As String,
                                     FieldValue As String,
                                     LookupTypeId As Integer,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.GetNAICSData.ResponseData
            Dim res As New DCSM.PolicyService.GetNAICSData.Response
            Dim req As New DCSM.PolicyService.GetNAICSData.Request

            With req.RequestData
                .Fieldname = FieldName
                .Fieldvalue = FieldValue
                .LookupTypeId = LookupTypeId
            End With

            If IFMS.Policy.GetNAICSData(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function GetNewPolicyLinkNumber(PolicyId As Integer,
                                               PolicyTypeId As Integer,
                                               Optional ByRef e As System.Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim res As New DCSM.PolicyService.GetNewPolicyLinkNumber.Response
            Dim req As New DCSM.PolicyService.GetNewPolicyLinkNumber.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyTypeId = PolicyTypeId
            End With

            If IFMS.Policy.GetNewPolicyLinkNumber(res, req, e, dv) Then
                Return res.ResponseData.NewPolicyLinkNumber
            End If
            Return Nothing
        End Function

        Public Function GetNewPolicyOrQuoteNumber(CompanyId As Integer,
                                                  EffectiveDate As Date,
                                                  IsQuote As Boolean,
                                                  LobId As Integer,
                                                  StateId As Integer,
                                                  TransactionTypeId As Integer,
                                                  Optional ByRef e As System.Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim res As New DCSM.PolicyService.GetNewPolicyOrQuoteNumber.Response
            Dim req As New DCSM.PolicyService.GetNewPolicyOrQuoteNumber.Request

            With req.RequestData
                .CompanyId = CompanyId
                .EffectiveDate = EffectiveDate
                .IsQuote = IsQuote
                .LobId = LobId
                .StateId = StateId
                .TransactionTypeId = TransactionTypeId
            End With

            If IFMS.Policy.GetNewPolicyOrQuoteNumber(res, req, e, dv) Then
                Return res.ResponseData.PolicyOrQuoteNumber
            End If
            Return Nothing
        End Function

        Public Function GetPendingOrQuoteImageByPolicyIdAndOrImageNum(ImageNumber As Integer,
                                                                      PolicyId As Integer,
                                                                      Optional ByRef e As System.Exception = Nothing,
                                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.GetPendingOrQuoteImageByPolicyIdAndOrImageNum.Response
            Dim req As New DCSM.PolicyService.GetPendingOrQuoteImageByPolicyIdAndOrImageNum.Request

            With req.RequestData
                .ImageNumber = ImageNumber
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.GetPendingOrQuoteImageByPolicyIdAndOrImageNum(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
            Return Nothing
        End Function

        Public Function GetPolicyHistory(GetPreviousPolicyHistory As Boolean,
                                         PolicyId As Integer,
                                         SortResultsInReversChronologicalOrder As Boolean,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.PolicyHistoryFilter
            Dim res As New DCSM.PolicyService.GetPolicyHistory.Response
            Dim req As New DCSM.PolicyService.GetPolicyHistory.Request

            With req.RequestData
                .GetPreviousPolicyHistory = GetPreviousPolicyHistory
                .PolicyId = PolicyId
                .SortResultsInReverseChronologicalOrder = SortResultsInReversChronologicalOrder
            End With

            If IFMS.Policy.GetPolicyHistory(res, req, e, dv) Then
                Return res.ResponseData.Data
            End If
            Return Nothing
        End Function

        Public Function GetPolicyIdAndNumForBillingAccount(BillingAccountId As String,
                                                           Optional ByRef e As System.Exception = Nothing,
                                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.QuickLookup)
            Dim res As New DCSM.PolicyService.GetPolicyIdAndNumForBillingAccount.Response
            Dim req As New DCSM.PolicyService.GetPolicyIdAndNumForBillingAccount.Request

            With req.RequestData
                .BillingAccountId = BillingAccountId
            End With

            If IFMS.Policy.GetPolicyIdAndNumForBillingAccount(res, req, e, dv) Then
                Return res.ResponseData.Policies
            End If
            Return Nothing
        End Function

        Public Function GetPolicyIdAndNumForPolicyNumber(PolicyNumber As String,
                                                         Optional ByRef e As System.Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.QuickLookup)
            Dim res As New DCSM.PolicyService.GetPolicyIdAndNumForPolicyNumber.Response
            Dim req As New DCSM.PolicyService.GetPolicyIdAndNumForPolicyNumber.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.GetPolicyIdAndNumForPolicyNumber(res, req, e, dv) Then
                Return res.ResponseData.Policies
            End If
            Return Nothing
        End Function

        Public Function GetPolicyMessage(PolicyId As String,
                                         PolicyImageNum As Integer,
                                         ReturnEnhancedStatusDetails As Boolean,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.GetPolicyMessage.ResponseData
            Dim res As New DCSM.PolicyService.GetPolicyMessage.Response
            Dim req As New DCSM.PolicyService.GetPolicyMessage.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .ReturnEnhancedStatusDetails = ReturnEnhancedStatusDetails
            End With

            If IFMS.Policy.GetPolicyMessage(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function GetPreviousPolicyHistory(PolicyId As Integer,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.History)
            Dim res As New DCSM.PolicyService.GetPreviousPolicyHistory.Response
            Dim req As New DCSM.PolicyService.GetPreviousPolicyHistory.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.GetPreviousPolicyHistory(res, req, e, dv) Then
                Return res.ResponseData.Histories
            End If
            Return Nothing
        End Function

        Public Function GetTerritoryData(FieldName As String,
                                         FieldValue As String,
                                         VersionId As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.Territory)
            Dim res As New DCSM.PolicyService.GetTerritoryData.Response
            Dim req As New DCSM.PolicyService.GetTerritoryData.Request

            With req.RequestData
                .Fieldname = FieldName
                .Fieldvalue = FieldValue
                .VersionId = VersionId
            End With

            If IFMS.Policy.GetTerritoryData(res, req, e, dv) Then
                Return res.ResponseData.Territories
            End If
            Return Nothing
        End Function

        Public Function GetValidPackagePartTypes(PolicyId As Integer,
                                                 PolicyImageNum As Integer,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DC.StaticDataManager.Objects.SystemData.PackagePartType)
            Dim res As New DCSM.PolicyService.GetValidPackagePartTypes.Response
            Dim req As New DCSM.PolicyService.GetValidPackagePartTypes.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.GetValidPackagePartTypes(res, req, e, dv) Then
                Return res.ResponseData.PackagePartTypes
            End If
            Return Nothing
        End Function

        Public Function GetVersionIdByPolicyId(PolicyId As Integer,
                                               Optional ByRef e As System.Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.GetVersionIdByPolicyId.ResponseData
            Dim res As New DCSM.PolicyService.GetVersionIdByPolicyId.Response
            Dim req As New DCSM.PolicyService.GetVersionIdByPolicyId.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.GetVersionIdByPolicyId(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function GetVersionIdByPolicyNumber(PolicyNumber As String,
                                                   Optional ByRef e As System.Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.GetVersionIdByPolicyNumber.ResponseData
            Dim res As New DCSM.PolicyService.GetVersionIdByPolicyNumber.Response
            Dim req As New DCSM.PolicyService.GetVersionIdByPolicyNumber.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.GetVersionIdByPolicyNumber(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function GetVersionIdForPackagePart(PackPartNum As Integer,
                                                   PolicyId As Integer,
                                                   PolicyImageNum As Integer,
                                                   Optional ByRef e As System.Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim res As New DCSM.PolicyService.GetVersionIdForPackagePart.Response
            Dim req As New DCSM.PolicyService.GetVersionIdForPackagePart.Request

            With req.RequestData
                .PackPartNum = PackPartNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.GetVersionIdForPackagePart(res, req, e, dv) Then
                Return res.ResponseData.VersionId
            End If
            Return Nothing
        End Function

        Public Function GetVersionToSubmit(SubmitVersion As DCO.Transactions.SubmitVersion,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.StaticDataManager.Objects.SystemData.TransactionVersion
            Dim res As New DCSM.PolicyService.GetVersionToSubmit.Response
            Dim req As New DCSM.PolicyService.GetVersionToSubmit.Request

            With req.RequestData
                .SubmitVersion = SubmitVersion
            End With

            If IFMS.Policy.GetVersionToSubmit(res, req, e, dv) Then
                Return res.ResponseData.TransactionVersion
            End If
            Return Nothing
        End Function

        Public Function ImportPackagePart(Image As DCO.Policy.Image,
                                          ImportFromPolicyId As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.ImportPackagePart.ResponseData
            Dim res As New DCSM.PolicyService.ImportPackagePart.Response
            Dim req As New DCSM.PolicyService.ImportPackagePart.Request

            With req.RequestData
                .Image = Image
                .ImportFromPolicyId = ImportFromPolicyId
            End With

            If IFMS.Policy.ImportPackagePart(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function ImportUnderlyingPolicies(Image As DCO.Policy.Image,
                                                 LookupResults As DCO.InsCollection(Of DCO.UnderlyingPolicy.LookupResult),
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.ImportUnderlyingPolicies.Response
            Dim req As New DCSM.PolicyService.ImportUnderlyingPolicies.Request

            With req.RequestData
                .Image = Image
                .LookupResults = LookupResults
            End With

            If IFMS.Policy.ImportUnderlyingPolicies(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
            Return Nothing
        End Function

        Public Function IsExternalPolicy(PolicyId As Integer,
                                         PolicyImageNum As Integer,
                                         PolicyNumber As String,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.IsExternalPolicy.Response
            Dim req As New DCSM.PolicyService.IsExternalPolicy.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.IsExternalPolicy(res, req, e, dv) Then
                Return res.ResponseData.IsExternal
            End If
            Return Nothing
        End Function

        Public Function IsNewTEffDateValid(Errors As DCO.DiamondValidation,
                                           PolicyId As Integer,
                                           PolicyImageNum As Integer,
                                           TransactionEffectiveDate As Date,
                                           TransactionEffectiveTime As Date,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim res As New DCSM.PolicyService.IsNewTEffDateValid.Response
            Dim req As New DCSM.PolicyService.IsNewTEffDateValid.Request

            With req.RequestData
                .Errors = Errors
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .TransactionEffectiveDate = TransactionEffectiveDate
                .TransactionEffectiveTime = TransactionEffectiveTime
            End With

            If IFMS.Policy.IsNewTEffDateValid(res, req, e, dv) Then
                Return res.ResponseData.PolicyImageNum
            End If
            Return Nothing
        End Function

        Public Function IssueByPolicyId(ImageNumber As Integer,
                                        IsAutomaticTrans As Boolean,
                                        PaymentInformation As DCO.Billing.ApplyCash,
                                        PolicyId As Integer,
                                        Rate As Boolean,
                                        TransferCash As Boolean,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As String()
            Dim res As New DCSM.PolicyService.IssueByPolicyId.Response
            Dim req As New DCSM.PolicyService.IssueByPolicyId.Request

            With req.RequestData
                .ImageNumber = ImageNumber
                .IsAutomaticTrans = IsAutomaticTrans
                .PaymentInformation = PaymentInformation
                .PolicyId = PolicyId
                .Rate = Rate
                .TransferCash = TransferCash
            End With

            If IFMS.Policy.IssueByPolicyId(res, req, e, dv) Then
                Return res.ResponseData.printGUIDs
            End If
            Return Nothing
        End Function

        Public Function LoadClient(ClientId As Integer,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Client
            Dim res As New DCSM.PolicyService.LoadClient.Response
            Dim req As New DCSM.PolicyService.LoadClient.Request

            With req.RequestData
                .ClientId = ClientId
            End With

            If IFMS.Policy.LoadClient(res, req, e, dv) Then
                Return res.ResponseData.Client
            End If
            Return Nothing
        End Function

        Public Function LoadCoveragePlanDefaults(AgencyId As Integer,
                                                 CoveragePlanId As Integer,
                                                 VersionId As Integer,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.LoadCoveragePlanDefaults.PlanDetail)
            Dim res As New DCSM.PolicyService.LoadCoveragePlanDefaults.Response
            Dim req As New DCSM.PolicyService.LoadCoveragePlanDefaults.Request

            With req.RequestData
                .AgencyId = AgencyId
                .CoveragePlanId = CoveragePlanId
                .VersionId = VersionId
            End With

            If IFMS.Policy.LoadCoveragePlanDefaults(res, req, e, dv) Then
                Return res.ResponseData.PlanDetails
            End If
            Return Nothing
        End Function

        Public Function LoadCoveragePlans(PolicyId As Integer,
                                          PolicyImageNum As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.CoveragePlan)
            Dim res As New DCSM.PolicyService.LoadCoveragePlans.Response
            Dim req As New DCSM.PolicyService.LoadCoveragePlans.Request

            With req.RequestData
                .policyId = PolicyId
                .policyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.LoadCoveragePlans(res, req, e, dv) Then
                Return res.ResponseData.CoveragePlans
            End If
            Return Nothing
        End Function

        'Public Function LoadDetail(ClientId As Integer,
        '                           Optional ByRef e As System.Exception = Nothing,
        '                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ClientDetail
        '    Dim res As New DCSM.PolicyService.LoadDetail.Response
        '    Dim req As New DCSM.PolicyService.LoadDetail.Request

        '    With req.RequestData
        '        .ClientId = ClientId
        '    End With

        '    If IFMS.Policy.LoadDetail(res, req, e, dv) Then
        '        Return res.ResponseData.ClientDetail
        '    End If
        '    Return Nothing
        'End Function

        Public Function LoadExperienceModification(ExperienceModificationNum As Integer,
                                                   PolicyId As Integer,
                                                   Optional ByRef e As System.Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.ExperienceModification)
            Dim res As New DCSM.PolicyService.LoadExperienceModification.Response
            Dim req As New DCSM.PolicyService.LoadExperienceModification.Request

            With req.RequestData
                .ExperienceModificationNum = ExperienceModificationNum
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.LoadExperienceModification(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadFilingHistory(PolicyId As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.FilingHistory)
            Dim res As New DCSM.PolicyService.LoadFilingHistory.Response
            Dim req As New DCSM.PolicyService.LoadFilingHistory.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.LoadFilingHistory(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadFilingInfo(PolicyId As Integer,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.LoadFilingInfo.ResponseData
            Dim res As New DCSM.PolicyService.LoadFilingInfo.Response
            Dim req As New DCSM.PolicyService.LoadFilingInfo.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.LoadFilingInfo(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function LoadFutureEvents(PolicyId As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.LoadFutureEvents.DataItem)
            Dim res As New DCSM.PolicyService.LoadFutureEvents.Response
            Dim req As New DCSM.PolicyService.LoadFutureEvents.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.LoadFutureEvents(res, req, e, dv) Then
                Return res.ResponseData.DataItems
            End If
            Return Nothing
        End Function

        Public Function LoadImagesByPolicyNumber(PolicyNumber As String,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.Image)
            Dim res As New DCSM.PolicyService.LoadImagesByPolicyNumber.Response
            Dim req As New DCSM.PolicyService.LoadImagesByPolicyNumber.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.LoadImagesByPolicyNumber(res, req, e, dv) Then
                Return res.ResponseData.Images
            End If
            Return Nothing
        End Function

        Public Function LoadInspection(PolicyId As Integer,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.LoadInspection.ResponseData
            Dim res As New DCSM.PolicyService.LoadInspection.Response
            Dim req As New DCSM.PolicyService.LoadInspection.Request

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.LoadInspection(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function LoadMiscPolicyStatic(PolicyId As Integer,
                                             PolicyImageNum As Integer,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.MiscPolicyStatic
            Dim res As New DCSM.PolicyService.LoadMiscPolicyStatic.Response
            Dim req As New DCSM.PolicyService.LoadMiscPolicyStatic.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.LoadMiscPolicyStatic(res, req, e, dv) Then
                Return res.ResponseData.MiscPolicyStatic
            End If
            Return Nothing
        End Function

        Public Function LoadOutsideAuditor(OutsideAuditorId As Integer,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Administration.OutsideAuditor
            Dim res As New DCSM.PolicyService.LoadOutsideAuditor.Response
            Dim req As New DCSM.PolicyService.LoadOutsideAuditor.Request

            With req.RequestData
                .OutsideAuditorId = OutsideAuditorId
            End With

            If IFMS.Policy.LoadOutsideAuditor(res, req, e, dv) Then
                Return res.ResponseData.OutsideAuditor
            End If
            Return Nothing
        End Function

        Public Function LoadOutsideAuditorList(Optional ByRef e As System.Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.OutsideAuditorListData)
            Dim res As New DCSM.PolicyService.LoadOutsideAuditorList.Response
            Dim req As New DCSM.PolicyService.LoadOutsideAuditorList.Request

            With req.RequestData
            End With

            If IFMS.Policy.LoadOutsideAuditorList(res, req, e, dv) Then
                Return res.ResponseData.OutsideAuditors
            End If
            Return Nothing
        End Function

        Public Function LoadOutsideAuditorNames(County As String,
                                                OutsideAuditorId As Integer,
                                                StateId As Integer,
                                                Optional ByRef e As System.Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Administration.OutsideAuditorName)
            Dim res As New DCSM.PolicyService.LoadOutsideAuditorNames.Response
            Dim req As New DCSM.PolicyService.LoadOutsideAuditorNames.Request

            With req.RequestData
                .County = County
                .OutsideAuditorId = OutsideAuditorId
                .StateId = StateId
            End With

            If IFMS.Policy.LoadOutsideAuditorNames(res, req, e, dv) Then
                Return res.ResponseData.OutsideAuditorNames
            End If
            Return Nothing
        End Function

        Public Function LoadPackagePartList(CalledFromClaims As Boolean,
                                            PolicyId As Integer,
                                            PolicyImageNum As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.LoadPackagePartList.DataItem)
            Dim res As New DCSM.PolicyService.LoadPackagePartList.Response
            Dim req As New DCSM.PolicyService.LoadPackagePartList.Request

            With req.RequestData
                .CalledFromClaims = CalledFromClaims
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.LoadPackagePartList(res, req, e, dv) Then
                Return res.ResponseData.DataItems
            End If
            Return Nothing
        End Function

        Public Function LoadPolicyAuditList(PolicyAuditNum As Integer,
                                            PolicyId As Integer,
                                            RenewalVer As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.PolicyAuditList)
            Dim res As New DCSM.PolicyService.LoadPolicyAuditList.Response
            Dim req As New DCSM.PolicyService.LoadPolicyAuditList.Request

            With req.RequestData
                .PolicyAuditNum = PolicyAuditNum
                .PolicyId = PolicyId
                .RenewalVer = RenewalVer
            End With

            If IFMS.Policy.LoadPolicyAuditList(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadPolicyAudits(PolicyAuditNum As Integer,
                                         PolicyId As Integer,
                                         RenewalVer As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.PolicyAudit)
            Dim res As New DCSM.PolicyService.LoadPolicyAudits.Response
            Dim req As New DCSM.PolicyService.LoadPolicyAudits.Request

            With req.RequestData
                .PolicyAuditNum = PolicyAuditNum
                .PolicyId = PolicyId
                .RenewalVer = RenewalVer
            End With

            If IFMS.Policy.LoadPolicyAudits(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadPolicyDetail(PolicyId As Integer,
                                         PolicyImageNum As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.LoadPolicyDetail.Response
            Dim req As New DCSM.PolicyService.LoadPolicyDetail.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.Policy.LoadPolicyDetail(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function LoadPolicySearchTypeViewableByUserCategoryList(Optional ByRef e As System.Exception = Nothing,
                                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Administration.PolicySearchType)
            Dim res As New DCSM.PolicyService.LoadPolicySearchTypeViewableByUserCategoryList.Response
            Dim req As New DCSM.PolicyService.LoadPolicySearchTypeViewableByUserCategoryList.Request

            With req.RequestData
            End With

            If IFMS.Policy.LoadPolicySearchTypeViewableByUserCategoryList(res, req, e, dv) Then
                Return res.ResponseData.Results
            End If
            Return Nothing
        End Function

        Public Function LoadProblemPolicyAccounts(ClientId As Integer,
                                                  PolicyIds As Integer(),
                                                  Optional ByRef e As System.Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.ProblemPolicyAccount)
            Dim res As New DCSM.PolicyService.LoadProblemPolicyAccounts.Response
            Dim req As New DCSM.PolicyService.LoadProblemPolicyAccounts.Request

            With req.RequestData
                .ClientId = ClientId
                .PolicyIds = PolicyIds
            End With

            If IFMS.Policy.LoadProblemPolicyAccounts(res, req, e, dv) Then
                Return res.ResponseData.ProblemPolicyAccounts
            End If
            Return Nothing
        End Function

        Public Function LoadRecentBillingListForUser(Optional ByRef e As System.Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.LoadRecentBillingListForUser.ResponseData
            Dim res As New DCSM.PolicyService.LoadRecentBillingListForUser.Response
            Dim req As New DCSM.PolicyService.LoadRecentBillingListForUser.Request

            With req.RequestData
            End With

            If IFMS.Policy.LoadRecentBillingListForUser(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function LoadRecentPolicyListForUser(Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.QuickLookup)
            Dim res As New DCSM.PolicyService.LoadRecentPolicyListForUser.Response
            Dim req As New DCSM.PolicyService.LoadRecentPolicyListForUser.Request

            With req.RequestData
            End With

            If IFMS.Policy.LoadRecentPolicyListForUser(res, req, e, dv) Then
                Return res.ResponseData.PolicyNumbers
            End If
            Return Nothing
        End Function

        Public Function LoadRenewalUnderwriting(PolciyId As Integer,
                                                PolicyStatusCodeId As Integer,
                                                RenewalVersion As Integer,
                                                Optional ByRef e As System.Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.RenewalUnderwriting)
            Dim res As New DCSM.PolicyService.LoadRenewalUnderwriting.Response
            Dim req As New DCSM.PolicyService.LoadRenewalUnderwriting.Request

            With req.RequestData
                .PolicyId = PolciyId
                .PolicyStatusCodeId = PolicyStatusCodeId
                .RenewalVersion = RenewalVersion
            End With

            If IFMS.Policy.LoadRenewalUnderwriting(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadSummaryOfChanges(PolciyId As Integer,
                                             PolicyImageNumber As Integer,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Transactions.EndorsementChange)
            Dim res As New DCSM.PolicyService.LoadSummaryOfChanges.Response
            Dim req As New DCSM.PolicyService.LoadSummaryOfChanges.Request

            With req.RequestData
                .PolicyId = PolciyId
                .PolicyImageNumber = PolicyImageNumber
            End With

            If IFMS.Policy.LoadSummaryOfChanges(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadTransactions(ClientId As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ClientTransactionView)
            Dim res As New DCSM.PolicyService.LoadTransactions.Response
            Dim req As New DCSM.PolicyService.LoadTransactions.Request

            With req.RequestData
                .ClientId = ClientId
            End With

            If IFMS.Policy.LoadTransactions(res, req, e, dv) Then
                Return res.ResponseData.Items
            End If
            Return Nothing
        End Function

        Public Function LoadValidation(PolicyId As Integer,
                                       PolicyImageNum As Integer,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.PolicyService.LoadValidation.Response
            Dim req As New DCSM.PolicyService.LoadValidation.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            IFMS.Policy.LoadValidation(res, req, e, dv)

            Return Nothing
        End Function

        Public Function LookupMultiPolicyDiscountPolicies(LookupCriteria As DCO.MultiPolicyDiscount.LookupCriteria,
                                                          Optional ByRef e As System.Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.MultiPolicyDiscount.LookupResult)
            Dim res As New DCSM.PolicyService.LookupMultiPolicyDiscountPolicies.Response
            Dim req As New DCSM.PolicyService.LookupMultiPolicyDiscountPolicies.Request

            With req.RequestData
                .LookupCriteria = LookupCriteria
            End With

            If IFMS.Policy.LookupMultiPolicyDiscountPolicies(res, req, e, dv) Then
                Return res.ResponseData.LookupResults
            End If
            Return Nothing
        End Function

        Public Function LookupUnderlyingPolicies(LookupCriteria As DCO.UnderlyingPolicy.LookupCriteria,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.UnderlyingPolicy.LookupResult)
            Dim res As New DCSM.PolicyService.LookupUnderlyingPolicies.Response
            Dim req As New DCSM.PolicyService.LookupUnderlyingPolicies.Request

            With req.RequestData
                .LookupCriteria = LookupCriteria
            End With

            If IFMS.Policy.LookupUnderlyingPolicies(res, req, e, dv) Then
                Return res.ResponseData.LookupResults
            End If
            Return Nothing
        End Function

        Public Function ProcessAACS(IsRating As Boolean,
                                    PolicyImage As DCO.Policy.Image,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.ProcessAACS.ResponseData
            Dim res As New DCSM.PolicyService.ProcessAACS.Response
            Dim req As New DCSM.PolicyService.ProcessAACS.Request

            With req.RequestData
                .IsRating = IsRating
                .PolicyImage = PolicyImage
            End With

            If IFMS.Policy.ProcessAACS(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function QueryForBillingAccountNumber(BillingAccountNumber As String,
                                                     Optional ByRef e As System.Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.QuickLookup)
            Dim res As New DCSM.PolicyService.QueryForBillingAccountNumber.Response
            Dim req As New DCSM.PolicyService.QueryForBillingAccountNumber.Request

            With req.RequestData
                .BillingAccountNumber = BillingAccountNumber
            End With

            If IFMS.Policy.QueryForBillingAccountNumber(res, req, e, dv) Then
                Return res.ResponseData.DataItems
            End If
            Return Nothing
        End Function

        Public Function QueryForPolicyNumber(IsLegacyPolicyNumber As Boolean,
                                             OnlyReturnViewableItems As Boolean,
                                             PolicyNumber As String,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.QuickLookup)
            Dim res As New DCSM.PolicyService.QueryForPolicyNumber.Response
            Dim req As New DCSM.PolicyService.QueryForPolicyNumber.Request

            With req.RequestData
                .IsLegacyPolicyNumber = IsLegacyPolicyNumber
                .OnlyReturnViewableItems = OnlyReturnViewableItems
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.QueryForPolicyNumber(res, req, e, dv) Then
                Return res.ResponseData.DataItems
            End If
            Return Nothing
        End Function

        Public Function QueuePolicyForExport(PolicyIds As System.Collections.ArrayList,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.PolicyService.QueuePolicyForExport.ProcessedItem)
            Dim res As New DCSM.PolicyService.QueuePolicyForExport.Response
            Dim req As New DCSM.PolicyService.QueuePolicyForExport.Request

            With req.RequestData
            End With

            If IFMS.Policy.QueuePolicyForExport(res, req, e, dv) Then
                Return res.ResponseData.ProcessedItems
            End If
            Return Nothing
        End Function

        Public Function RouteQuoteToUnderwriting(PolicyId As Integer,
                                                 PolicyImageNum As Integer,
                                                 UserId As Integer,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.RouteQuoteToUnderwriting.Response
            Dim req As New DCSM.PolicyService.RouteQuoteToUnderwriting.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UsersId = UserId
            End With

            If IFMS.Policy.RouteQuoteToUnderwriting(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function SaveClientInfo(AlreadyValidated As Boolean,
                                       Client As DCO.Client,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Client
            Dim res As New DCSM.PolicyService.SaveClientInfo.Response
            Dim req As New DCSM.PolicyService.SaveClientInfo.Request

            With req.RequestData
                .AlreadyValidated = AlreadyValidated
                .Client = Client
            End With

            If IFMS.Policy.SaveClientInfo(res, req, e, dv) Then
                Return res.ResponseData.Client
            End If
            Return Nothing
        End Function

        Public Function SaveExperienceModification(Item As DCO.Policy.ExperienceModification,
                                                   PolicyId As Integer,
                                                   Optional ByRef e As System.Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.ExperienceModification
            Dim res As New DCSM.PolicyService.SaveExperienceModification.Response
            Dim req As New DCSM.PolicyService.SaveExperienceModification.Request

            With req.RequestData
                .Item = Item
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.SaveExperienceModification(res, req, e, dv) Then
                Return res.ResponseData.Item
            End If
            Return Nothing
        End Function

        Public Function SaveFilingHistory(Item As DCO.Policy.FilingHistory,
                                          PolicyId As Integer,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.FilingHistory
            Dim res As New DCSM.PolicyService.SaveFilingHistory.Response
            Dim req As New DCSM.PolicyService.SaveFilingHistory.Request

            With req.RequestData
                .Item = Item
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.SaveFilingHistory(res, req, e, dv) Then
                Return res.ResponseData.Item
            End If
            Return Nothing
        End Function

        Public Function SaveFilingInfo(Item As DCO.Policy.FilingInfo,
                                       PolicyId As Integer,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.FilingInfo
            Dim res As New DCSM.PolicyService.SaveFilingInfo.Response
            Dim req As New DCSM.PolicyService.SaveFilingInfo.Request

            With req.RequestData
                .Item = Item
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.SaveFilingInfo(res, req, e, dv) Then
                Return res.ResponseData.Item
            End If
            Return Nothing
        End Function

        Public Function SaveInspection(Item As DCO.Policy.Inspection,
                               PolicyId As Integer,
                               Optional ByRef e As System.Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Inspection
            Dim res As New DCSM.PolicyService.SaveInspection.Response
            Dim req As New DCSM.PolicyService.SaveInspection.Request

            With req.RequestData
                .Item = Item
                .PolicyId = PolicyId
            End With

            If IFMS.Policy.SaveInspection(res, req, e, dv) Then
                Return res.ResponseData.Item
            End If
            Return Nothing
        End Function

        Public Function SaveOutsideAuditor(OutsideAuditor As DCO.Administration.OutsideAuditor,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SaveOutsideAuditor.Response
            Dim req As New DCSM.PolicyService.SaveOutsideAuditor.Request

            With req.RequestData
                .OutsideAuditor = OutsideAuditor
            End With

            If IFMS.Policy.SaveOutsideAuditor(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function SavePolicyAudits(Item As DCO.Policy.PolicyAudit,
                                         PolicyId As Integer,
                                         RenewalYear As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.PolicyAudit
            Dim res As New DCSM.PolicyService.SavePolicyAudits.Response
            Dim req As New DCSM.PolicyService.SavePolicyAudits.Request

            With req.RequestData
                .Item = Item
                .PolicyId = PolicyId
                .RenewalVer = RenewalYear
            End With

            If IFMS.Policy.SavePolicyAudits(res, req, e, dv) Then
                Return res.ResponseData.Item
            End If
            Return Nothing
        End Function

        Public Function SavePolicySearchTypeViewableByUserCategory(Item As DCO.Administration.PolicySearchType,
                                                                   Optional ByRef e As System.Exception = Nothing,
                                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SavePolicySearchTypeViewableByUserCategory.Response
            Dim req As New DCSM.PolicyService.SavePolicySearchTypeViewableByUserCategory.Request

            With req.RequestData
                .Item = Item
            End With

            If IFMS.Policy.SavePolicySearchTypeViewableByUserCategory(res, req, e, dv) Then
                Return res.ResponseData.Result
            End If
            Return Nothing
        End Function

        Public Function SaveProblemPolicyAccounts(ProblemPolicyAccounts As DCO.InsCollection(Of DCO.Policy.ProblemPolicyAccount),
                                                  Optional ByRef e As System.Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.PolicyService.SaveProblemPolicyAccounts.Response
            Dim req As New DCSM.PolicyService.SaveProblemPolicyAccounts.Request

            With req.RequestData
                .ProblemPolicyAccounts = ProblemPolicyAccounts
            End With

            IFMS.Policy.SaveProblemPolicyAccounts(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SaveRateIssue(Image As DCO.Policy.Image,
                                      PackagePartNumsToSave As System.Collections.Generic.List(Of DCO.IdValue),
                                      PaymentInformation As DCO.Billing.ApplyCash,
                                      ReturnIssuedImage As Boolean,
                                      TransferCash As Boolean,
                                      UpdatedPackagePart As DCO.Policy.PackagePart,
                                      ValidationScreenType As Integer,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyService.SaveRateIssue.ResponseData
            Dim res As New DCSM.PolicyService.SaveRateIssue.Response
            Dim req As New DCSM.PolicyService.SaveRateIssue.Request

            With req.RequestData
                .Image = Image
                .PackagePartNumsToSave = PackagePartNumsToSave
                .PaymentInformation = PaymentInformation
                .ReturnIssuedImage = ReturnIssuedImage
                .TransferCash = TransferCash
                .UpdatedPackagePart = UpdatedPackagePart
                .ValidationScreenType = ValidationScreenType
            End With

            If IFMS.Policy.SaveRateIssue(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function SaveRecentBillingForUser(BillingNumber As String,
                                                 QuickLookupTypeID As Integer,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SaveRecentBillingForUser.Response
            Dim req As New DCSM.PolicyService.SaveRecentBillingForUser.Request

            With req.RequestData
                .BillingNumber = BillingNumber
                .QuickLookupTypeID = QuickLookupTypeID
            End With

            If IFMS.Policy.SaveRecentBillingForUser(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function SaveRecentPolicyForUser(PolicyNumber As String,
                                                Optional ByRef e As System.Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SaveRecentPolicyForUser.Response
            Dim req As New DCSM.PolicyService.SaveRecentPolicyForUser.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.SaveRecentPolicyForUser(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function SaveTransRemark(PolicyId As Integer,
                                        PolicyImageNum As Integer,
                                        RemarkTxt As String,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SaveTransRemark.Response
            Dim req As New DCSM.PolicyService.SaveTransRemark.Request

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .RemarkTxt = RemarkTxt
            End With

            If IFMS.Policy.SaveTransRemark(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function SearchByAdditionalPolicyHolder(Name As String,
                                                       Optional ByRef e As System.Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByAdditionalPolicyHolder.Response
            Dim req As New DCSM.PolicyService.SearchByAdditionalPolicyHolder.Request

            With req.RequestData
                .Name = Name
            End With

            IFMS.Policy.SearchByAdditionalPolicyHolder(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByAddress(ApartmentOrSuiteNumber As String,
                                        City As String,
                                        CountryId As Integer,
                                        HouseNumber As String,
                                        PostOfficeBox As String,
                                        StateId As Integer,
                                        StreetName As String,
                                        ZipCode As String,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByAddress.Response
            Dim req As New DCSM.PolicyService.SearchByAddress.Request

            With req.RequestData
                .AparmentOrSuiteNumber = ApartmentOrSuiteNumber
                .City = City
                .CountryId = CountryId
                .HouseNumber = HouseNumber
                .PostOfficeBox = PostOfficeBox
                .StateId = StateId
                .StreetName = StreetName
                .ZipCode = ZipCode
            End With

            IFMS.Policy.SearchByAddress(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByClient(PolicyId As Integer,
                                       ValidCompanyStateLobs As System.Collections.Generic.List(Of Integer),
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByClient.Response
            Dim req As New DCSM.PolicyService.SearchByClient.Request

            With req.RequestData
                .PolicyId = PolicyId
                .ValidCompanyStateLobs = ValidCompanyStateLobs
            End With

            IFMS.Policy.SearchByClient(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByDoingBusinessAs(Name As String,
                                                Optional ByRef e As System.Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByDoingBusinessAs.Response
            Dim req As New DCSM.PolicyService.SearchByDoingBusinessAs.Request

            With req.RequestData
                .Name = Name
            End With

            IFMS.Policy.SearchByDoingBusinessAs(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByFullName(Name As String,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByFullName.Response
            Dim req As New DCSM.PolicyService.SearchByFullName.Request

            With req.RequestData
                .Name = Name
            End With

            IFMS.Policy.SearchByFullName(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByManuallyCreatedClientIdentifier(ClientIdentifier As String,
                                                                Optional ByRef e As System.Exception = Nothing,
                                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByManuallyCreatedClientIdentifier.Response
            Dim req As New DCSM.PolicyService.SearchByManuallyCreatedClientIdentifier.Request

            With req.RequestData
                .ClientIdentifier = ClientIdentifier
            End With

            IFMS.Policy.SearchByManuallyCreatedClientIdentifier(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByName(FirstName As String,
                                     LastName As String,
                                     MiddleName As String,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByName.Response
            Dim req As New DCSM.PolicyService.SearchByName.Request

            With req.RequestData
                .FirstName = FirstName
                .LastName = LastName
                .MiddleName = MiddleName
            End With

            IFMS.Policy.SearchByName(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByPhoneNumber(PhoneNumber As String,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByPhoneNumber.Response
            Dim req As New DCSM.PolicyService.SearchByPhoneNumber.Request

            With req.RequestData
                .PhoneNumber = PhoneNumber
            End With

            IFMS.Policy.SearchByPhoneNumber(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByPolicyNumber(IsLegacyPolicyNumber As Boolean,
                                             PolicyNumber As String,
                                             ShowNewestItemsFirst As Boolean,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByPolicyNumber.Response
            Dim req As New DCSM.PolicyService.SearchByPolicyNumber.Request

            With req.RequestData
                .IsLegacyPolicyNumber = IsLegacyPolicyNumber
                .PolicyNumber = PolicyNumber
                .ShowNewestItemsFirst = ShowNewestItemsFirst
            End With

            IFMS.Policy.SearchByPolicyNumber(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByQuoteNumber(QuoteNumber As String,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SearchByQuoteNumber.Response
            Dim req As New DCSM.PolicyService.SearchByQuoteNumber.Request

            With req.RequestData
                .QuoteNumber = QuoteNumber
            End With

            IFMS.Policy.SearchByQuoteNumber(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SetPolicyLock(AutomaticTransaction As Boolean,
                                      PolicyId As Integer,
                                      Reason As String,
                                      Value As Boolean,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SetPolicyLock.Response
            Dim req As New DCSM.PolicyService.SetPolicyLock.Request

            With req.RequestData
                .AutomaticTransaction = AutomaticTransaction
                .PolicyId = PolicyId
                .Reason = Reason
                .Value = Value
            End With

            If IFMS.Policy.SetPolicyLock(res, req, e, dv) Then
                Return res.ResponseData.Result
            End If
            Return Nothing
        End Function

        Public Function SmallLoad(ParentDataValues As Integer(),
                                  ProcessingData As DCO.Persistence.LoadProcessingData,
                                  ShallowLoad As Boolean,
                                  TypeAssembleyQualifiedName As String,
                                  Optional ByRef e As System.Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Interfaces.IInsCollection
            Dim res As New DCSM.PolicyService.SmallLoad.Response
            Dim req As New DCSM.PolicyService.SmallLoad.Request

            With req.RequestData
                .ParentDataValues = ParentDataValues
                .ProcessingData = ProcessingData
                .ShallowLoad = ShallowLoad
                .TypeAssemblyQualifiedName = TypeAssembleyQualifiedName
            End With

            If IFMS.Policy.SmallLoad(res, req, e, dv) Then
                Return res.ResponseData.InsObjects
            End If
            Return Nothing
        End Function

        Public Function SmallSave(InsObjects As DCO.Interfaces.IInsCollection,
                                  ParentDataValues As Integer(),
                                  ProcessingData As DCO.Persistence.SaveProcessingData,
                                  Reload As Boolean,
                                  ShallowSave As Boolean,
                                  TypeAssemblyQualifiedName As String,
                                  Optional ByRef e As System.Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Interfaces.IInsCollection
            Dim res As New DCSM.PolicyService.SmallSave.Response
            Dim req As New DCSM.PolicyService.SmallSave.Request

            With req.RequestData
                .InsObjects = InsObjects
                .ParentDataValues = ParentDataValues
                .ProcessingData = ProcessingData
                .Reload = Reload
                .ShallowSave = ShallowSave
                .TypeAssemblyQualifiedName = TypeAssemblyQualifiedName
            End With

            If IFMS.Policy.SmallSave(res, req, e, dv) Then
                Return res.ResponseData.InsObjects
            End If
            Return Nothing
        End Function

        Public Function SR2226Import(Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SR2226Import.Response
            Dim req As New DCSM.PolicyService.SR2226Import.Request

            With req.RequestData
            End With

            If IFMS.Policy.SR2226Import(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadImage(PolicyID As Integer,
                                  PolicyImageNum As Integer,
                                  Optional ByRef e As System.Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing
                                  ) As DCO.Policy.Image

            Dim res As New DCSM.PolicyService.LoadImage.Response
            Dim req As New DCSM.PolicyService.LoadImage.Request
            req.RequestData.PolicyId = PolicyID
            req.RequestData.ImageNumber = PolicyImageNum

            If IFMS.Policy.LoadImage(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyNumber"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadImages(PolicyNumber As String,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As List(Of DCO.Policy.Image)
            Dim res As New DCSM.PolicyService.LoadImagesByPolicyNumber.Response
            Dim req As New DCSM.PolicyService.LoadImagesByPolicyNumber.Request
            Dim retList As New List(Of DCO.Policy.Image)
            req.RequestData.PolicyNumber = PolicyNumber
            If IFMS.Policy.LoadImagesByPolicyNumber(res, req, e, dv) Then
                If res.ResponseData.Images IsNot Nothing Then
                    retList = (res.ResponseData.Images).ToList
                End If
            End If
            Return retList
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyImage"></param>
        ''' <param name="CompanyID"></param>
        ''' <param name="StateID"></param>
        ''' <param name="LOBID"></param>
        ''' <param name="TransactionEffectiveDate"></param>
        ''' <param name="GuaranteedRatePeriodEffectiveDate"></param>
        ''' <param name="TransTypeID"></param>
        ''' <param name="IsQuote"></param>
        ''' <param name="AutoCreateAdditionalInterest"></param>
        ''' <param name="SaveAncillaryChoicePointDate"></param>
        ''' <param name="PersistQuote"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubmitRate(PolicyImage As DCO.Policy.Image, CompanyID As Integer, StateID As Integer, LOBID As Integer,
                                   TransactionEffectiveDate As Date, GuaranteedRatePeriodEffectiveDate As Date, TransTypeID As Integer,
                                   Optional IsQuote As Boolean = True,
                                   Optional AutoCreateAdditionalInterest As Boolean = False,
                                   Optional SaveAncillaryChoicePointDate As Boolean = True,
                                   Optional PersistQuote As Boolean = True,
                                   Optional ByRef Success As Boolean = False,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.SubmitRate.Response
            Dim req As New DCSM.PolicyService.SubmitRate.Request
            With req.RequestData
                .PolicyImage = PolicyImage
                .CompanyId = CompanyID
                .StateId = StateID
                .LOBId = LOBID
                .TransactionEffectiveDate = TransactionEffectiveDate
                .GuaranteedRatePeriodEffectiveDate = GuaranteedRatePeriodEffectiveDate
                .TransTypeId = TransTypeID
                .IsQuote = IsQuote
                .AutoCreateAdditionalInterest = AutoCreateAdditionalInterest
                .SaveAncillaryChoicePointData = SaveAncillaryChoicePointDate
                .PersistQuote = PersistQuote
            End With

            If IFMS.Policy.SubmitRate(res, req, e, dv) Then
                Success = res.ResponseData.Success
                Return res.ResponseData.PolicyImage
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="QuoteImage"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveRate(QuoteImage As DCO.Policy.Image, ByRef SaveSuccessful As Boolean,
                                 ByRef RateSuccessful As Boolean,
                                 Optional ByRef e As System.Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim req As New DCSM.PolicyService.SaveRate.Request
            Dim res As New DCSM.PolicyService.SaveRate.Response

            req.RequestData.Image = QuoteImage
            If IFMS.Policy.SaveRate(res, req, e, dv) Then
                If res.ResponseData.Image IsNot Nothing Then
                    SaveSuccessful = res.ResponseData.SaveSuccessful
                    RateSuccessful = res.ResponseData.RateSuccessful
                    Return res.ResponseData.Image
                End If
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyImage"></param>
        ''' <param name="PolicyID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="Rate"></param>
        ''' <param name="SubmitVersion"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubmitApplication(PolicyImage As DCO.Policy.Image,
                                          ByRef PolicyID As Integer,
                                          ByRef PolicyImageNum As Integer,
                                          Optional Rate As Boolean = False,
                                          Optional SubmitVersion As DCO.Transactions.SubmitVersion = Nothing,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.SubmitApplication.Response
            Dim req As New DCSM.PolicyService.SubmitApplication.Request

            With req.RequestData
                .PolicyImage = PolicyImage
                .AlreadyValidated = False
                .IsQuote = True
                .Rate = Rate
                .ShouldClearAndDefault = False
                .SupportTransaction = False
                .SubmitVersion = SubmitVersion
            End With

            If IFMS.Policy.SubmitApplication(res, req, e, dv) Then
                If res.ResponseData IsNot Nothing Then
                    PolicyID = res.ResponseData.PolicyId
                    PolicyImageNum = res.ResponseData.PolicyImageNum
                    Return res.ResponseData.Success
                End If
            End If
            Return False
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="img"></param>
        ''' <param name="returnImg"></param>
        ''' <param name="SubmitVersion"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks>Returns successful, but can't find the data in diamondqa</remarks>
        Public Function RateOnly(img As DCO.Policy.Image,
                         ByRef returnImg As DCO.Policy.Image,
                         Optional SubmitVersion As DCO.Transactions.SubmitVersion = Nothing,
                         Optional ByRef e As System.Exception = Nothing,
                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As DCSM.PolicyService.RateOnly.Response = Nothing
            Dim req As New DCSM.PolicyService.RateOnly.Request
            Using p As New DCSP.PolicyServices.PolicyServiceProxy
                req.RequestData.PolicyImage = img
                req.RequestData.AlreadyValidated = False
                req.RequestData.ResetIsRated = True
                req.RequestData.SubmitVersion = SubmitVersion
            End Using
            If IFMS.Policy.RateOnly(res, req, e, dv) Then
                If res IsNot Nothing AndAlso res.ResponseData IsNot Nothing AndAlso res.ResponseData.Success Then
                    returnImg = res.ResponseData.PolicyImage
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Rate(img As DCO.Policy.Image,
                         ByRef returnImg As DCO.Policy.Image,
                         Optional ByRef e As System.Exception = Nothing,
                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As DCSM.PolicyService.Rate.Response = Nothing
            Dim req As New DCSM.PolicyService.Rate.Request
            Using p As New DCSP.PolicyServices.PolicyServiceProxy
                req.RequestData.PolicyImage = img
                req.RequestData.AlreadyValidated = False
                req.RequestData.ResetIsRated = True
            End Using
            If IFMS.Policy.Rate(res, req, e, dv) Then
                If res IsNot Nothing AndAlso res.ResponseData.Success Then
                    returnImg = res.ResponseData.PolicyImage
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function SaveClient(img As DCO.Policy.Image,
                                   ByRef rtnImage As DCO.Policy.Image,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As DCSM.PolicyService.SaveClient.Response = Nothing
            Dim req As New DCSM.PolicyService.SaveClient.Request

            Using p As New DCSP.PolicyServices.PolicyServiceProxy
                req.RequestData.PolicyImage = img
                req.RequestData.PolicyAction = Global.Diamond.Common.Enums.PolicyAction.Save

                If IFMS.Policy.SaveClient(res, req, e, dv) Then
                    If res IsNot Nothing AndAlso res.ResponseData.OperationSuccessful Then
                        rtnImage = res.ResponseData.PolicyImage
                        Return True
                    End If
                End If
            End Using

            Return False
        End Function

        Public Function Issue(img As DCO.Policy.Image,
                              Optional ByRef e As System.Exception = Nothing,
                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As DCSM.PolicyService.Issue.Response = Nothing
            Dim req As New DCSM.PolicyService.Issue.Request

            Using p As New DCSP.PolicyServices.PolicyServiceProxy
                req.RequestData.PolicyImage = img
                If IFMS.Policy.Issue(res, req, e, dv) Then
                    If res IsNot Nothing AndAlso res.ResponseData.OperationSuccessful Then
                        Return True
                    End If
                End If
            End Using

            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="UserID"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PromoteQuoteToPending(PolicyID As Integer,
                                              PolicyImageNum As Integer,
                                              UserID As Integer,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.PromoteQuoteToPending.Response
            Dim req As New DCSM.PolicyService.PromoteQuoteToPending.Request
            req.RequestData.UsersId = UserID
            req.RequestData.PolicyId = PolicyID
            req.RequestData.PolicyImageNum = PolicyImageNum

            If IFMS.Policy.PromoteQuoteToPending(res, req, e, dv) Then
                If res.ResponseData IsNot Nothing Then
                    Return res.ResponseData.PolicyImage
                End If
            End If

            Return Nothing
        End Function

        Public Function PromoteQuoteToPending(img As DCO.Policy.Image,
                                              ByRef rtnImg As DCO.Policy.Image,
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As DCSM.PolicyService.PromoteQuoteToPending.Response = Nothing
            Dim req As New DCSM.PolicyService.PromoteQuoteToPending.Request

            Using p As New DCSP.PolicyServices.PolicyServiceProxy
                req.RequestData.PolicyImageNum = img.PolicyImageNum
                req.RequestData.PolicyId = img.PolicyId
                If IFMS.Policy.PromoteQuoteToPending(res, req, e, dv) Then
                    If res IsNot Nothing Then
                        rtnImg = res.ResponseData.PolicyImage
                        Return True
                    End If
                End If
            End Using
            Return False
        End Function

        Public Function Tiering(CompanyId As Integer,
                                EffectiveDate As Date,
                                LobId As Integer,
                                PolicyId As Integer,
                                PolicyImageNum As Integer,
                                StateId As Integer,
                                Optional ByRef e As System.Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.Tiering.Response
            Dim req As New DCSM.PolicyService.Tiering.Request

            With req.RequestData
            End With

            If IFMS.Policy.Tiering(res, req, e, dv) Then
                Return res.ResponseData.PolicyImage
            End If
            Return Nothing
        End Function

        Public Function TransferPolicyToClient(ClientId As Integer,
                                               PolicyId As Integer,
                                               Optional ByRef e As System.Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.TransferPolicyToClient.Response
            Dim req As New DCSM.PolicyService.TransferPolicyToClient.Request

            With req.RequestData
            End With

            IFMS.Policy.TransferPolicyToClient(res, req, e, dv)

            Return Nothing
        End Function

        Public Function UndenyPolicy(PolicyNumber As String,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.UndenyPolicy.Response
            Dim req As New DCSM.PolicyService.UndenyPolicy.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If IFMS.Policy.UndenyPolicy(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function UpdateReferredBy(ApplyWithVersionChange As Boolean,
                                         Image As DCO.Policy.Image,
                                         QuoteSourceId As Integer,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim res As New DCSM.PolicyService.UpdateReferredBy.Response
            Dim req As New DCSM.PolicyService.UpdateReferredBy.Request

            With req.RequestData
                .ApplyWithVersionChange = ApplyWithVersionChange
                .Image = Image
                .QuoteSourceId = QuoteSourceId
            End With

            If IFMS.Policy.UpdateReferredBy(res, req, e, dv) Then
                Return res.ResponseData.Image
            End If
            Return Nothing
        End Function

        Public Function ValidateApplication(Image As DCO.Policy.Image,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.PolicyService.ValidateApplication.Response
            Dim req As New DCSM.PolicyService.ValidateApplication.Request

            With req.RequestData
                .Image = Image
            End With

            If IFMS.Policy.ValidateApplication(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function
    End Module
End Namespace

