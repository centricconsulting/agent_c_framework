Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCST = Diamond.Common.Services.Messages.ThirdPartyService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.ThirdParty
    Public Module ThirdParty
        Public Function AutomaticRenewalCreditScoreOrder(ByRef res As DCST.AutomaticRenewalCreditScoreOrder.Response,
                                                         ByRef req As DCST.AutomaticRenewalCreditScoreOrder.Request,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AutomaticRenewalCreditScoreOrder
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BatchQueue(ByRef res As DCST.BatchQueue.Response,
                                   ByRef req As DCST.BatchQueue.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.BatchQueue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAccount(ByRef res As DCST.DeleteAccount.Response,
                                      ByRef req As DCST.DeleteAccount.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteReport(ByRef res As DCST.DeleteReport.Response,
                                     ByRef req As DCST.DeleteReport.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteReport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function FillPolicyImageData(ByRef res As DCST.FillPolicyImageData.Response,
                                            ByRef req As DCST.FillPolicyImageData.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.FillPolicyImageData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function FillSAQData(ByRef res As DCST.FillSAQData.Response,
                                    ByRef req As DCST.FillSAQData.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.FillSAQData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GatherCLUEAutoRecords(ByRef res As DCST.GatherCLUEAutoRecords.Response,
                                              ByRef req As DCST.GatherCLUEAutoRecords.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GatherCLUEAutoRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GatherCLUECommercialRecords(ByRef res As DCST.GatherCLUECommercialRecords.Response,
                                                    ByRef req As DCST.GatherCLUECommercialRecords.Request,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GatherCLUECommercialRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GatherCLUEPropRecords(ByRef res As DCST.GatherCLUEPropRecords.Response,
                                              ByRef req As DCST.GatherCLUEPropRecords.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GatherCLUEPropRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetTestCases(ByRef res As DCST.GetTestCases.Response,
                                     ByRef req As DCST.GetTestCases.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetTestCases
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function InteractiveRequest(ByRef res As DCST.InteractiveRequest.Response,
                                           ByRef req As DCST.InteractiveRequest.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.InteractiveRequest
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ISOPassportLookup(ByRef res As DCST.ISOPassportLookup.Response,
                                          ByRef req As DCST.ISOPassportLookup.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ISOPassportLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAccountList(ByRef res As DCST.LoadAccountList.Response,
                                        ByRef req As DCST.LoadAccountList.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAccountList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadChoicePointTransmissions(ByRef res As DCST.LoadChoicePointTransmissions.Response,
                                                     ByRef req As DCST.LoadChoicePointTransmissions.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadChoicePointTransmissions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadCLUEAutoReportObject(ByRef res As DCST.LoadCLUEAutoReportObject.Response,
                                                 ByRef req As DCST.LoadCLUEAutoReportObject.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCLUEAutoReportObject
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadCLUEPropertyReportObject(ByRef res As DCST.LoadCLUEPropertyReportObject.Response,
                                                     ByRef req As DCST.LoadCLUEPropertyReportObject.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCLUEPropertyReportObject
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadExistingBatchReceivedTransmissions(ByRef res As DCST.LoadExistingBatchReceivedTransmissions.Response,
                                                               ByRef req As DCST.LoadExistingBatchReceivedTransmissions.Request,
                                                               Optional ByRef e As Exception = Nothing,
                                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadExistingBatchReceivedTransmissions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        'Matt A 4-8-17
        Public Function LoadLexIdTransmissions(ByRef res As DCST.LoadLexisNexisTransmissions.Response,
                                            ByRef req As DCST.LoadLexisNexisTransmissions.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLexisNexisTransmissions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function


        Public Function LoadMVRReportObject(ByRef res As DCST.LoadMVRReportObject.Response,
                                            ByRef req As DCST.LoadMVRReportObject.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadMVRReportObject
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadNCFReportObject(ByRef res As DCST.LoadNCFReportObject.Response,
                                            ByRef req As DCST.LoadNCFReportObject.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadNCFReportObject
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadRAWReportData(ByRef res As DCST.LoadRawReportData.Response,
                                          ByRef req As DCST.LoadRawReportData.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRAWReportData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadReportListingForClientId(ByRef res As DCST.LoadReportListingForClientId.Response,
                                                     ByRef req As DCST.LoadReportListingForClientId.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReportListingForClientId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadReportListingForPolicyId(ByRef res As DCST.LoadReportListingForPolicyId.Response,
                                                     ByRef req As DCST.LoadReportListingForPolicyId.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReportListingForPolicyId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadSAQOrder(ByRef res As DCST.LoadSAQOrder.Response,
                                     ByRef req As DCST.LoadSAQOrder.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSAQOrder
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadViewReportsScreenData(ByRef res As DCST.LoadViewReportsScreenData.Response,
                                                  ByRef req As DCST.LoadViewReportsScreenData.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadViewReportsScreenData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoginFailure(ByRef res As DCST.ComparativeRater.LoginFailure.Response,
                                     ByRef req As DCST.ComparativeRater.LoginFailure.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoginFailure
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function NCFBatchCreateTransRecords(ByRef res As DCST.NCFBatchCreateTransRecords.Response,
                                                   ByRef req As DCST.NCFBatchCreateTransRecords.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.NCFBatchCreateTransRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function NCFBatchPreProcess(ByRef res As DCST.NCFBatchPreProcess.Response,
                                           ByRef req As DCST.NCFBatchPreProcess.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.NCFBatchPreProcess
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function NCFBatchSubmit(ByRef res As DCST.NCFBatchSubmit.Response,
                                       ByRef req As DCST.NCFBatchSubmit.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.NCFBatchSubmit
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function NCFBatchSummary(ByRef res As DCST.NCFBatchSummary.Response,
                                        ByRef req As DCST.NCFBatchSummary.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.NCFBatchSummary
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderAutoDataPrefill(ByRef res As DCST.OrderAutoDataPrefill.Response,
                                             ByRef req As DCST.OrderAutoDataPrefill.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderAutoDataPrefill
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderClueAuto(ByRef res As DCST.OrderClueAuto.Response,
                                      ByRef req As DCST.OrderClueAuto.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderClueAuto
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderCluePropertyReport(ByRef res As DCST.OrderCluePropertyReport.Response,
                                                ByRef req As DCST.OrderCluePropertyReport.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderCluePropertyReport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderCreditReport(ByRef res As DCST.OrderCreditReport.Response,
                                          ByRef req As DCST.OrderCreditReport.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderCreditReport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        'Matt A 4-8-17
        Public Function OrderLexIdReport(ByRef res As DCST.OrderLexisNexisComprehensiveADL.Response,
                                          ByRef req As DCST.OrderLexisNexisComprehensiveADL.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderLexisNexisComprehensiveADL
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderMultipleReports(ByRef res As DCST.OrderMultipleReports.Response,
                                             ByRef req As DCST.OrderMultipleReports.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderMultipleReports
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderMvr(ByRef res As DCST.OrderMvr.Response,
                                 ByRef req As DCST.OrderMvr.Request,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderMvr
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function OrderSAQ(ByRef res As DCST.OrderSAQ.Response,
                                 ByRef req As DCST.OrderSAQ.Request,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OrderSAQ
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function PPCLookupByLocation(ByRef res As DCST.PPCLookupByLocation.Response,
                                  ByRef req As DCST.PPCLookupByLocation.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.PPCLookupByLocation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessAndImportSpecificBatchTransmission(ByRef res As DCST.ProcessAndImportSpecificBatchTransmission.Response,
                                                                  ByRef req As DCST.ProcessAndImportSpecificBatchTransmission.Request,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessAndImportSpecificBatchTransmission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessCLUEAuto(ByRef res As DCST.ProcessCLUEAuto.Response,
                                        ByRef req As DCST.ProcessCLUEAuto.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessCLUEAuto
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessCLUEProperty(ByRef res As DCST.ProcessCLUEProperty.Response,
                                            ByRef req As DCST.ProcessCLUEProperty.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessCLUEProperty
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessFullNCF(ByRef res As DCST.ProcessFullNCF.Response,
                                       ByRef req As DCST.ProcessFullNCF.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessFullNCF
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessMVR(ByRef res As DCST.ProcessMVR.Response,
                                   ByRef req As DCST.ProcessMVR.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessMVR
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessNCF(ByRef res As DCST.ProcessNCF.Response,
                                   ByRef req As DCST.ProcessNCF.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessNCF
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessViewImported(ByRef res As DCST.ProcessViewImported.Response,
                                            ByRef req As DCST.ProcessViewImported.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessViewImported
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveAccount(ByRef res As DCST.SaveAccount.Response,
                                    ByRef req As DCST.SaveAccount.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SubmitAndRate(ByRef res As DCST.ComparativeRater.SubmitAndRate.Response,
                                      ByRef req As DCST.ComparativeRater.SubmitAndRate.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SubmitAndRate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UndoProcessingOfBatchFile(ByRef res As DCST.UndoProcessingOfBatchFile.Response,
                                                  ByRef req As DCST.UndoProcessingOfBatchFile.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UndoProcessingOfBatchFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UpdateAndProduceSAQImports(ByRef res As DCST.UpdateAndProduceSAQImports.Response,
                                                   ByRef req As DCST.UpdateAndProduceSAQImports.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateAndProduceSAQImports
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UpdateSAQImports(ByRef res As DCST.UpdateSAQImports.Response,
                                         ByRef req As DCST.UpdateSAQImports.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateSAQImports
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ValuationReturn(ByRef res As DCST.Valuation.ValuationReturn.Response,
                                        ByRef req As DCST.Valuation.ValuationReturn.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValuationReturn
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ValuationStart(ByRef res As DCST.Valuation.ValuationStart.Response,
                                       ByRef req As DCST.Valuation.ValuationStart.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValuationStart
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VehicleInformationLookup(ByRef res As DCST.ComparativeRater.VehicleInformationLookup.Response,
                                                 ByRef req As DCST.ComparativeRater.VehicleInformationLookup.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ThirdPartyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.VehicleInformationLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

