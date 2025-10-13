Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.ThirdParty
    Public Module ThirdParty
        Public Function AutomaticRenewalCreditScoreOrder(Image As DCO.Policy.Image,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.ThirdPartyService.AutomaticRenewalCreditScoreOrder.Request
            Dim res As New DCSM.ThirdPartyService.AutomaticRenewalCreditScoreOrder.Response

            With req.RequestData
                .Image = Image
            End With

            IFMS.ThirdParty.AutomaticRenewalCreditScoreOrder(res, req, e, dv)

            Return Nothing
        End Function

        Public Function BatchQueue(DriverNum As Integer,
                                   Image As DCO.Policy.Image,
                                   PackagePartNum As Integer,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.ThirdPartyService.BatchQueue.Request
            Dim res As New DCSM.ThirdPartyService.BatchQueue.Response

            With req.RequestData
                .DriverNum = DriverNum
                .Image = Image
                .PackagePartNum = PackagePartNum
            End With

            IFMS.ThirdParty.BatchQueue(res, req, e, dv)

            Return Nothing
        End Function

        Public Function DeleteAccount(Account As DCO.ThirdParty.ChoicePointAccount,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.ThirdPartyService.DeleteAccount.Request
            Dim res As New DCSM.ThirdPartyService.DeleteAccount.Response

            With req.RequestData
                .Account = Account
            End With

            If (IFMS.ThirdParty.DeleteAccount(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function DeleteReport(QuoteBack_guid As String,
                                     Users_id As Integer,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.ThirdPartyService.DeleteReport.Request
            Dim res As New DCSM.ThirdPartyService.DeleteReport.Response

            With req.RequestData
                .QuoteBack_guid = QuoteBack_guid
                .Users_id = Users_id
            End With

            If (IFMS.ThirdParty.DeleteReport(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.OperationSuccessful
                End If
            End If

            Return Nothing
        End Function

        Public Function FillPolicyImageData(PackagePartNum As Integer,
                                            PolicyImage As DCO.Policy.Image,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.FillPolicyImageData.ResponseData
            Dim req As New DCSM.ThirdPartyService.FillPolicyImageData.Request
            Dim res As New DCSM.ThirdPartyService.FillPolicyImageData.Response

            With req.RequestData
                .PackagePartNum = PackagePartNum
                .PolicyImage = PolicyImage
            End With

            If (IFMS.ThirdParty.FillPolicyImageData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function FillSAQData(PolicyImage As DCO.Policy.Image,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim req As New DCSM.ThirdPartyService.FillSAQData.Request
            Dim res As New DCSM.ThirdPartyService.FillSAQData.Response

            With req.RequestData
                .PolicyImage = PolicyImage
            End With

            If (IFMS.ThirdParty.FillSAQData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyImage
                End If
            End If

            Return Nothing
        End Function

        Public Function GatherCLUEAutoRecords(BeginningDate As String,
                                              CompanyId As Integer,
                                              EndDate As String,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ClueReportFile
            Dim req As New DCSM.ThirdPartyService.GatherCLUEAutoRecords.Request
            Dim res As New DCSM.ThirdPartyService.GatherCLUEAutoRecords.Response

            With req.RequestData
                .BeginningDate = BeginningDate
                .CompanyId = CompanyId
                .EndDate = EndDate
            End With

            If (IFMS.ThirdParty.GatherCLUEAutoRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.File
                End If
            End If

            Return Nothing
        End Function

        Public Function GatherCLUECommercialRecords(BeginningDate As String,
                                                    CompanyId As Integer,
                                                    EndDate As String,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ClueReportFile
            Dim req As New DCSM.ThirdPartyService.GatherCLUECommercialRecords.Request
            Dim res As New DCSM.ThirdPartyService.GatherCLUECommercialRecords.Response

            With req.RequestData
                .BeginningDate = BeginningDate
                .CompanyId = CompanyId
                .EndDate = EndDate
            End With

            If (IFMS.ThirdParty.GatherCLUECommercialRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.File
                End If
            End If

            Return Nothing
        End Function

        Public Function GatherCLUEPropRecords(BeginningDate As String,
                                            CompanyId As Integer,
                                            EndDate As String,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ClueReportFile
            Dim req As New DCSM.ThirdPartyService.GatherCLUEPropRecords.Request
            Dim res As New DCSM.ThirdPartyService.GatherCLUEPropRecords.Response

            With req.RequestData
                .BeginningDate = BeginningDate
                .CompanyId = CompanyId
                .EndDate = EndDate
            End With

            If (IFMS.ThirdParty.GatherCLUEPropRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.File
                End If
            End If

            Return Nothing
        End Function

        Public Function GetTestCases(GetCLUEAuto As Boolean,
                                     GetCLUEProperty As Boolean,
                                     GetMVR As Boolean,
                                     GetNCF As Boolean,
                                     WhichMVR As Integer,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As System.Data.DataSet
            Dim req As New DCSM.ThirdPartyService.GetTestCases.Request
            Dim res As New DCSM.ThirdPartyService.GetTestCases.Response

            With req.RequestData
                .GetCLUEAuto = GetCLUEAuto
                .GetCLUEProperty = GetCLUEProperty
                .GetMVR = GetMVR
                .GetNCF = GetNCF
                .WhichMVR = WhichMVR
            End With

            If (IFMS.ThirdParty.GetTestCases(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.TestCases
                End If
            End If

            Return Nothing
        End Function

        Public Function InteractiveRequest(Account As DCO.ThirdParty.ChoicePointAccount,
                                           DriverNum As Integer,
                                           Image As DCO.Policy.Image,
                                           PackagePartNum As Integer,
                                           SaveRequest As Boolean,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.ThirdPartyService.InteractiveRequest.Request
            Dim res As New DCSM.ThirdPartyService.InteractiveRequest.Response

            With req.RequestData
                .Account = Account
                .Image = Image
                .PackagePartNum = PackagePartNum
                .SaveRequest = SaveRequest
            End With

            IFMS.ThirdParty.InteractiveRequest(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ISOPassportLookup(City As String,
                                          County As String,
                                          HouseNumber As String,
                                          State As String,
                                          StreetName As String,
                                          VersionId As String,
                                          Zip As String,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ISOPassportLocationAddress
            Dim req As New DCSM.ThirdPartyService.ISOPassportLookup.Request
            Dim res As New DCSM.ThirdPartyService.ISOPassportLookup.Response

            With req.RequestData
                .City = City
                .County = County
                .HouseNumber = HouseNumber
                .State = State
                .StreetName = StreetName
                .VersionId = VersionId
                .Zip = Zip
            End With

            If (IFMS.ThirdParty.ISOPassportLookup(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LocationInfo
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadAccountList(Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ThirdParty.ChoicePointAccount)
            Dim req As New DCSM.ThirdPartyService.LoadAccountList.Request
            Dim res As New DCSM.ThirdPartyService.LoadAccountList.Response

            With req.RequestData
            End With

            If (IFMS.ThirdParty.LoadAccountList(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Accounts
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadChoicePointTransmissions(PolicyId As Integer,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ThirdParty.ChoicePointTransmission)
            Dim req As New DCSM.ThirdPartyService.LoadChoicePointTransmissions.Request
            Dim res As New DCSM.ThirdPartyService.LoadChoicePointTransmissions.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.ThirdParty.LoadChoicePointTransmissions(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ChoicePointTransmissions
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadCLUEAutoReportObject(ChoicePointTransmissionNum As Integer,
                                                 PolicyId As Integer,
                                                 PolicyImageNum As Integer,
                                                 UnitNum As Integer,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData
            Dim req As New DCSM.ThirdPartyService.LoadCLUEAutoReportObject.Request
            Dim res As New DCSM.ThirdPartyService.LoadCLUEAutoReportObject.Response

            With req.RequestData
                .ChoicePointTransmissionNum = ChoicePointTransmissionNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UnitNum = UnitNum
            End With

            If (IFMS.ThirdParty.LoadCLUEAutoReportObject(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadCLUEPropertyReportObject(ChoicePointTransmissionNum As Integer,
                                                     PolicyId As Integer,
                                                     PolicyImageNum As Integer,
                                                     UnitNum As Integer,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportData
            Dim req As New DCSM.ThirdPartyService.LoadCLUEPropertyReportObject.Request
            Dim res As New DCSM.ThirdPartyService.LoadCLUEPropertyReportObject.Response

            With req.RequestData
                .ChoicePointTransmissionNum = ChoicePointTransmissionNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UnitNum = UnitNum
            End With

            If (IFMS.ThirdParty.LoadCLUEPropertyReportObject(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadExistingBatchReceivedTransmissions(EndDate As DCO.InsDateTime,
                                                               StartDate As DCO.InsDateTime,
                                                               Optional ByRef e As Exception = Nothing,
                                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.LoadExistingBatchReceivedTransmissions.ResponseData
            Dim req As New DCSM.ThirdPartyService.LoadExistingBatchReceivedTransmissions.Request
            Dim res As New DCSM.ThirdPartyService.LoadExistingBatchReceivedTransmissions.Response

            With req.RequestData
                .EndDate = EndDate
                .StartDate = StartDate
            End With

            If (IFMS.ThirdParty.LoadExistingBatchReceivedTransmissions(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        'Matt A 4-8-17
        Public Function LoadLexIdTransmissions(PolicyId As Int32,
                                                               Optional ByRef e As Exception = Nothing,
                                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.LoadLexisNexisTransmissions.ResponseData
            Dim req As New DCSM.ThirdPartyService.LoadLexisNexisTransmissions.Request
            Dim res As New DCSM.ThirdPartyService.LoadLexisNexisTransmissions.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.ThirdParty.LoadLexIdTransmissions(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadMVRReportObject(ChoicePointTransmissionNum As Integer,
                                            PolicyId As Integer,
                                            PolicyImageNum As Integer,
                                            UnitNum As Integer,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.MVR.MVRReportData
            Dim req As New DCSM.ThirdPartyService.LoadMVRReportObject.Request
            Dim res As New DCSM.ThirdPartyService.LoadMVRReportObject.Response

            With req.RequestData
                .ChoicePointTransmissionNum = ChoicePointTransmissionNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UnitNum = UnitNum
            End With

            If (IFMS.ThirdParty.LoadMVRReportObject(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadNCFReportObject(ChoicePointTransmissionNum As Integer,
                                            PolicyId As Integer,
                                            PolicyImageNum As Integer,
                                            UnitNum As Integer,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.NCF.RecordGroup
            Dim req As New DCSM.ThirdPartyService.LoadNCFReportObject.Request
            Dim res As New DCSM.ThirdPartyService.LoadNCFReportObject.Response

            With req.RequestData
                .ChoicePointTransmissionNum = ChoicePointTransmissionNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UnitNum = UnitNum
            End With

            If (IFMS.ThirdParty.LoadNCFReportObject(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadRawReportData(ChoicePointTransmissionNum As Integer,
                                          PolicyId As Integer,
                                          PolicyImageNum As Integer,
                                          UnitNum As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.LoadRawReportData.ResponseData
            Dim req As New DCSM.ThirdPartyService.LoadRawReportData.Request
            Dim res As New DCSM.ThirdPartyService.LoadRawReportData.Response

            With req.RequestData
                .ChoicePointTransmissionNum = ChoicePointTransmissionNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UnitNum = UnitNum
            End With

            If (IFMS.ThirdParty.LoadRAWReportData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadReportListingForClientId(ClientId As Integer,
                                                     RelationshipFilters As DCO.InsCollection(Of DCSM.ThirdPartyService.LoadReportListingForClientId.RelationshipFilter),
                                                     ReportFilters As DCO.InsCollection(Of DCSM.ThirdPartyService.LoadReportListingForClientId.ReportFilter),
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.LoadReportListingForClientId.ResponseData
            Dim req As New DCSM.ThirdPartyService.LoadReportListingForClientId.Request
            Dim res As New DCSM.ThirdPartyService.LoadReportListingForClientId.Response

            With req.RequestData
                .ClientId = ClientId
            End With

            If (IFMS.ThirdParty.LoadReportListingForClientId(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadReportListingForPolicyId(PolicyId As Integer,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.LoadReportListingForPolicyId.ResponseData
            Dim req As New DCSM.ThirdPartyService.LoadReportListingForPolicyId.Request
            Dim res As New DCSM.ThirdPartyService.LoadReportListingForPolicyId.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.ThirdParty.LoadReportListingForPolicyId(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadSAQOrder(PolicyId As Integer,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.LoadSAQOrder.Request
            Dim res As New DCSM.ThirdPartyService.LoadSAQOrder.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.ThirdParty.LoadSAQOrder(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadViewReportsScreenData(PolicyImage As DCO.Policy.Image,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.ThirdPartyService.ReportsScreen.ReportItem)
            Dim req As New DCSM.ThirdPartyService.LoadViewReportsScreenData.Request
            Dim res As New DCSM.ThirdPartyService.LoadViewReportsScreenData.Response

            With req.RequestData
                .PolicyImage = PolicyImage
            End With

            If (IFMS.ThirdParty.LoadViewReportsScreenData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportItems
                End If
            End If

            Return Nothing
        End Function

        Public Function NCFBatchCreateTransRecords(BackgroundWorker As System.ComponentModel.BackgroundWorker,
                                                   Lobid As Integer,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.ThirdPartyService.NCFBatchCreateTransRecords.Request
            Dim res As New DCSM.ThirdPartyService.NCFBatchCreateTransRecords.Response

            With req.RequestData
                .BackgroundWorker = BackgroundWorker
                .LobId = Lobid
            End With

            If (IFMS.ThirdParty.NCFBatchCreateTransRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.NumberOfRecordsProcessed
                End If
            End If

            Return Nothing
        End Function

        Public Function NCFBatchPreProcess(Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.ThirdPartyService.NCFBatchPreProcess.Request
            Dim res As New DCSM.ThirdPartyService.NCFBatchPreProcess.Response

            With req.RequestData
            End With

            If (IFMS.ThirdParty.NCFBatchPreProcess(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.NumberOfRecordsProcessed
                End If
            End If

            Return Nothing
        End Function

        Public Function NCFBatchSubmit(Preview As Boolean,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.NCFBatchSubmit.ResponseData
            Dim req As New DCSM.ThirdPartyService.NCFBatchSubmit.Request
            Dim res As New DCSM.ThirdPartyService.NCFBatchSubmit.Response

            With req.RequestData
                .Preview = Preview
            End With

            If (IFMS.ThirdParty.NCFBatchSubmit(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function NCFBatchSummary(Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.NCFBatchSummary.ResponseData
            Dim req As New DCSM.ThirdPartyService.NCFBatchSummary.Request
            Dim res As New DCSM.ThirdPartyService.NCFBatchSummary.Response

            With req.RequestData
            End With

            If (IFMS.ThirdParty.NCFBatchSummary(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Image"></param>
        ''' <param name="SaveRequest"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OrderAutoDataPrefill(Image As DCO.Policy.Image,
                                             Optional SaveRequest As Boolean = True,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.OrderAutoDataPrefill.Request
            Dim res As New DCSM.ThirdPartyService.OrderAutoDataPrefill.Response

            With req.RequestData
                .PolicyImage = Image
                .SaveRequest = SaveRequest
            End With

            If IFMS.ThirdParty.OrderAutoDataPrefill(res, req, e, dv) Then
                If res.ResponseData.ThirdPartyData IsNot Nothing Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If
            Return Nothing
        End Function

        Public Function OrderClueAuto(AdditionalDriverDiscoverySearch As Boolean,
                                      ADDOnlySearch As Boolean,
                                      BatchOrder As Boolean,
                                      FillData As Boolean,
                                      LastReportDate As Date,
                                      ManualClueSubmission As Boolean,
                                      PackagePartNum As Integer,
                                      PolicyImage As DCO.Policy.Image,
                                      RenewalPreProcess As Boolean,
                                      SaveRequest As Boolean,
                                      SystemGenerated As Boolean,
                                      UnitNum As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.OrderClueAuto.Request
            Dim res As New DCSM.ThirdPartyService.OrderClueAuto.Response

            With req.RequestData
                .AdditionalDriverDiscoverySearch = AdditionalDriverDiscoverySearch
                .ADDOnlySearch = ADDOnlySearch
                .BatchOrder = BatchOrder
                .FillData = FillData
                .LastReportDate = LastReportDate
                .ManualClueSubmission = ManualClueSubmission
                .PackagePartNum = PackagePartNum
                .PolicyImage = PolicyImage
                .RenewalPreProcess = RenewalPreProcess
                .SaveRequest = SaveRequest
                .UnitNum = UnitNum
            End With

            If (IFMS.ThirdParty.OrderClueAuto(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If

            Return Nothing
        End Function

        Public Function OrderCluePropertyReport(BatchOrder As Boolean,
                                                FillData As Boolean,
                                                LastReportDate As Date,
                                                LocationNum As Integer,
                                                ManualClueSubmission As Boolean,
                                                PackagePartNum As Integer,
                                                PolicyImage As DCO.Policy.Image,
                                                RenewalPreProcess As Boolean,
                                                SaveRequest As Boolean,
                                                SystemGenerated As Boolean,
                                                Type As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.OrderCluePropertyReport.Request
            Dim res As New DCSM.ThirdPartyService.OrderCluePropertyReport.Response

            With req.RequestData
                .BatchOrder = BatchOrder
                .FillData = FillData
                .LastReportDate = LastReportDate
                .LocationNum = LocationNum
                .ManualClueSubmission = ManualClueSubmission
                .PackagePartNum = PackagePartNum
                .PolicyImage = PolicyImage
                .RenewalPreProcess = RenewalPreProcess
                .SaveRequest = SaveRequest
                .Type = Type
            End With

            If (IFMS.ThirdParty.OrderCluePropertyReport(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If

            Return Nothing
        End Function

        Public Function OrderCreditReport(BatchOrder As Boolean,
                                          BulkNCFBatch As Boolean,
                                          FillData As Boolean,
                                          PackagePartNum As Integer,
                                          PolicyImage As DCO.Policy.Image,
                                          RenewalPreProcess As Boolean,
                                          SaveRequest As Boolean,
                                          SubjectNums As System.Collections.ArrayList,
                                          SystemGenerated As Boolean,
                                          Type As String,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.OrderCreditReport.ResponseData
            Dim req As New DCSM.ThirdPartyService.OrderCreditReport.Request
            Dim res As New DCSM.ThirdPartyService.OrderCreditReport.Response

            With req.RequestData
                .BatchOrder = BatchOrder
                .BulkNCFBatch = BulkNCFBatch
                .FillData = FillData
                .PackagePartNum = PackagePartNum
                .PolicyImage = PolicyImage
                .RenewalPreProcess = RenewalPreProcess
                .SaveRequest = SaveRequest
                .SubjectNums = SubjectNums
                .SystemGenerated = SystemGenerated
            End With

            If (IFMS.ThirdParty.OrderCreditReport(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        'Matt A 4-8-17
        Public Function OrderLexIdReport(PolicyImage As DCO.Policy.Image,
                                         SubjectInformation As DCO.InsCollection(Of DCSM.ThirdPartyService.OrderLexisNexisComprehensiveADL.SubjectInformation),
                                         SystemGenerated As Boolean,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.OrderLexisNexisComprehensiveADL.ResponseData
            Dim req As New DCSM.ThirdPartyService.OrderLexisNexisComprehensiveADL.Request
            Dim res As New DCSM.ThirdPartyService.OrderLexisNexisComprehensiveADL.Response

            With req.RequestData
                .PolicyImage = PolicyImage
                .SubjectInformation = SubjectInformation
                .SystemGenerated = SystemGenerated
            End With

            If (IFMS.ThirdParty.OrderLexIdReport(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function OrderMultipleReports(ReportRequests As DCO.InsCollection(Of DCSM.ThirdPartyService.OrderMultipleReports.MultipleReportRequestItem),
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ThirdParty.ThirdPartyData)
            Dim req As New DCSM.ThirdPartyService.OrderMultipleReports.Request
            Dim res As New DCSM.ThirdPartyService.OrderMultipleReports.Response

            With req.RequestData
                .ReportRequests = ReportRequests
            End With

            If (IFMS.ThirdParty.OrderMultipleReports(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportItems
                End If
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyImage"></param>
        ''' <param name="DriverNumberList"></param>
        ''' <param name="SaveRequest"></param>
        ''' <param name="FillData"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OrderMVR(PolicyImage As DCO.Policy.Image,
                                 DriverNumberList As ArrayList,
                                 Optional SaveRequest As Boolean = True,
                                 Optional FillData As Boolean = True,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.OrderMvr.Request
            Dim res As New DCSM.ThirdPartyService.OrderMvr.Response

            With req.RequestData
                .PolicyImage = PolicyImage
                .DriverNums = DriverNumberList
                .SaveRequest = SaveRequest
                .FillData = FillData
            End With
            If IFMS.ThirdParty.OrderMvr(res, req, e, dv) Then
                If res.ResponseData.ThirdPartyData IsNot Nothing Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyImage"></param>
        ''' <param name="DriverNumber"></param>
        ''' <param name="SaveRequest"></param>
        ''' <param name="FillData"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OrderMVR(PolicyImage As DCO.Policy.Image,
                                 DriverNumber As Integer,
                                 Optional SaveRequest As Boolean = True,
                                 Optional FillData As Boolean = True,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.OrderMvr.Request
            Dim res As New DCSM.ThirdPartyService.OrderMvr.Response

            With req.RequestData
                .PolicyImage = PolicyImage
                .DriverNums.Add(DriverNumber)
                .SaveRequest = SaveRequest
                .FillData = FillData
            End With
            If IFMS.ThirdParty.OrderMvr(res, req, e, dv) Then
                If res.ResponseData.ThirdPartyData IsNot Nothing Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyImage"></param>
        ''' <param name="DriverNumberList"></param>
        ''' <param name="SaveRequest"></param>
        ''' <param name="FillData"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OrderMVR(PolicyImage As DCO.Policy.Image,
                                 DriverNumberList As List(Of Integer),
                                 Optional SaveRequest As Boolean = True,
                                 Optional FillData As Boolean = True,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData

            Dim DriverNums As New ArrayList
            For Each i In DriverNumberList
                DriverNums.Add(i)
            Next
            Return OrderMVR(PolicyImage, DriverNums, SaveRequest, FillData, e, dv)
        End Function

        Public Function OrderSAQ(BatchOrder As Boolean,
                                 PolicyImage As DCO.Policy.Image,
                                 RenewalPreProcess As Boolean,
                                 SaveRequest As Boolean,
                                 SystemGenerated As Boolean,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ThirdPartyData
            Dim req As New DCSM.ThirdPartyService.OrderSAQ.Request
            Dim res As New DCSM.ThirdPartyService.OrderSAQ.Response

            With req.RequestData
                .BatchOrder = BatchOrder
                .PolicyImage = PolicyImage
                .RenewalPreProcess = RenewalPreProcess
                .SaveRequest = SaveRequest
                .SystemGenerated = SystemGenerated
            End With

            If (IFMS.ThirdParty.OrderSAQ(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ThirdPartyData
                End If
            End If

            Return Nothing
        End Function

        Public Function PPCLookup(Location As DCO.Policy.Location,
                                  VersionId As Integer,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Location
            Dim req As New DCSM.ThirdPartyService.PPCLookupByLocation.Request
            Dim res As New DCSM.ThirdPartyService.PPCLookupByLocation.Response

            With req.RequestData
                .Location = Location
                .VersionId = VersionId
            End With

            If (IFMS.ThirdParty.PPCLookupByLocation(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Location
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessAndImportSpecificBatchTransmission(BatchChoicePointReceivedTransmissionId As Integer,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.ThirdPartyService.ProcessAndImportSpecificBatchTransmission.Request
            Dim res As New DCSM.ThirdPartyService.ProcessAndImportSpecificBatchTransmission.Response

            With req.RequestData
                .BatchChoicePointReceivedTransmissionId = BatchChoicePointReceivedTransmissionId
            End With

            If (IFMS.ThirdParty.ProcessAndImportSpecificBatchTransmission(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Successful
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessCLUEAuto(IncomingDataRaw As String,
                                        PolicyId As Integer,
                                        PolicyImageNum As Integer,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData
            Dim req As New DCSM.ThirdPartyService.ProcessCLUEAuto.Request
            Dim res As New DCSM.ThirdPartyService.ProcessCLUEAuto.Response

            With req.RequestData
                .IncomingDataRaw = IncomingDataRaw
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ThirdParty.ProcessCLUEAuto(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessCLUEProperty(IncomingDataRaw As String,
                                            PolicyId As Integer,
                                            PolicyImageNum As Integer,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportData
            Dim req As New DCSM.ThirdPartyService.ProcessCLUEProperty.Request
            Dim res As New DCSM.ThirdPartyService.ProcessCLUEProperty.Response

            With req.RequestData
                .IncomingDataRaw = IncomingDataRaw
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ThirdParty.ProcessCLUEProperty(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessFullNCF(IncomingDataRaw As String,
                                       PolicyId As Integer,
                                       PolicyImageNum As Integer,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.FullNCF.ReportData
            Dim req As New DCSM.ThirdPartyService.ProcessFullNCF.Request
            Dim res As New DCSM.ThirdPartyService.ProcessFullNCF.Response

            With req.RequestData
                .IncomingDataRaw = IncomingDataRaw
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ThirdParty.ProcessFullNCF(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessMVR(IncomingDataRaw As String,
                                   PolicyId As Integer,
                                   PolicyImageNum As Integer,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.MVR.MVRReportData
            Dim req As New DCSM.ThirdPartyService.ProcessMVR.Request
            Dim res As New DCSM.ThirdPartyService.ProcessMVR.Response

            With req.RequestData
                .IncomingDataRaw = IncomingDataRaw
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ThirdParty.ProcessMVR(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessNCF(IncomingDataRaw As String,
                                   IncomingDataXML As String,
                                   PolicyId As Integer,
                                   PolicyImageNum As Integer,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ThirdParty.ReportObjects.NCF.RecordGroup
            Dim req As New DCSM.ThirdPartyService.ProcessNCF.Request
            Dim res As New DCSM.ThirdPartyService.ProcessNCF.Response

            With req.RequestData
                .IncomingDataRaw = IncomingDataRaw
                .IncomingDataXML = IncomingDataXML
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ThirdParty.ProcessNCF(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportData
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessViewImported(IncomingDataRaw As String,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.ProcessViewImported.ResponseData
            Dim req As New DCSM.ThirdPartyService.ProcessViewImported.Request
            Dim res As New DCSM.ThirdPartyService.ProcessViewImported.Response

            With req.RequestData
                .IncomingDataRaw = IncomingDataRaw
            End With

            If (IFMS.ThirdParty.ProcessViewImported(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveAccount(Account As DCO.ThirdParty.ChoicePointAccount,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.ThirdPartyService.SaveAccount.Request
            Dim res As New DCSM.ThirdPartyService.SaveAccount.Response

            With req.RequestData
                .Account = Account
            End With

            If (IFMS.ThirdParty.SaveAccount(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function UndoProcessingOfBatchFile(BatchChoicePointReceivedTransmissionId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.ThirdPartyService.UndoProcessingOfBatchFile.Request
            Dim res As New DCSM.ThirdPartyService.UndoProcessingOfBatchFile.Response

            With req.RequestData
                .BatchChoicePointReceivedTransmissionId = BatchChoicePointReceivedTransmissionId
            End With

            If (IFMS.ThirdParty.UndoProcessingOfBatchFile(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Successful
                End If
            End If

            Return Nothing
        End Function

        Public Function UpdateAndProduceSAQImports(ImportedDrivers As DCO.InsCollection(Of DCO.ThirdParty.SAQImportedDriver),
                                                   ImportedVehicles As DCO.InsCollection(Of DCO.ThirdParty.SAQImportedVehicle),
                                                   PackagePartNum As Integer,
                                                   PolicyImage As DCO.Policy.Image,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.UpdateAndProduceSAQImports.ResponseData
            Dim req As New DCSM.ThirdPartyService.UpdateAndProduceSAQImports.Request
            Dim res As New DCSM.ThirdPartyService.UpdateAndProduceSAQImports.Response

            With req.RequestData
                .ImportedDrivers = ImportedDrivers
                .ImportedVehicles = ImportedVehicles
                .PackagePartNum = PackagePartNum
                .PolicyImage = PolicyImage
            End With

            If (IFMS.ThirdParty.UpdateAndProduceSAQImports(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function UpdateSAQImports(ImportedDrivers As DCO.InsCollection(Of DCO.ThirdParty.SAQImportedDriver),
                                         ImportedVehicles As DCO.InsCollection(Of DCO.ThirdParty.SAQImportedVehicle),
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.ThirdPartyService.UpdateSAQImports.Request
            Dim res As New DCSM.ThirdPartyService.UpdateSAQImports.Response

            With req.RequestData
                .ImportedDrivers = ImportedDrivers
                .ImportedVehicles = ImportedVehicles
            End With

            If (IFMS.ThirdParty.UpdateSAQImports(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.OperationSuccessful
                End If
            End If

            Return Nothing
        End Function

        Public Function ValuationReturn(PolicyID As Integer,
                                        PolicyImageNum As Integer,
                                        ValuationID As String,
                                        ValuationVersion As Integer,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ReplacementCost.ValuationReturnObject
            Dim req As New DCSM.ThirdPartyService.Valuation.ValuationReturn.Request
            Dim res As New DCSM.ThirdPartyService.Valuation.ValuationReturn.Response

            With req.RequestData
                .PolicyID = PolicyID
                .PolicyImageNum = PolicyImageNum
                .ValuationID = ValuationID
                .ValuationVersion = ValuationVersion
            End With

            If (IFMS.ThirdParty.ValuationReturn(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ValuationReturnObject
                End If
            End If

            Return Nothing
        End Function

        Public Function ValuationStart(Location As DCO.Policy.Location,
                                       Name As DCO.Name,
                                       VersionId As Integer,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ThirdPartyService.Valuation.ValuationStart.ResponseData
            Dim req As New DCSM.ThirdPartyService.Valuation.ValuationStart.Request
            Dim res As New DCSM.ThirdPartyService.Valuation.ValuationStart.Response

            With req.RequestData
                .Location = Location
                .Name = Name
                .VersionId = VersionId
            End With

            If (IFMS.ThirdParty.ValuationStart(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace

