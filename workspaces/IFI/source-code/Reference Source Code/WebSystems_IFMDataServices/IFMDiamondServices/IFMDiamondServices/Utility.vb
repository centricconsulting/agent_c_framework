Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Utility
    Public Module Utility
        Public Function AddTaxCodeToLocation(GuaranteedRatePeriodEffectiveDate As DCO.InsDateTime,
                                             Location As DCO.Policy.Location,
                                             TransactionEffectiveDate As DCO.InsDateTime,
                                             VersionId As Integer,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.UtilityService.AddTaxCodeToLocation.ResponseData
            Dim req As New DCSM.UtilityService.AddTaxCodeToLocation.Request
            Dim res As New DCSM.UtilityService.AddTaxCodeToLocation.Response

            With req.RequestData
                .GuaranteedRatePeriodEffectiveDate = GuaranteedRatePeriodEffectiveDate
                .Location = Location
                .TransactionEffectiveDate = TransactionEffectiveDate
                .VersionId = VersionId
            End With

            If (IFMS.Utility.AddTaxCodeToLocation(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function AgencyTaxCodeSCLookup(Address As DCO.Address,
                                              LobCategoryId As Integer,
                                              PolicyId As Integer,
                                              PolicyImageNum As Integer,
                                              StateAbbreviation As String,
                                              VersionId As Integer,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Address
            Dim req As New DCSM.UtilityService.AgencyTaxCodeSCLookup.Request
            Dim res As New DCSM.UtilityService.AgencyTaxCodeSCLookup.Response

            With req.RequestData
                .Address = Address
                .LobCategoryId = LobCategoryId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .StateAbbreviation = StateAbbreviation
                .VersionId = VersionId
            End With

            If (IFMS.Utility.AgencyTaxCodeSCLookup(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Address
                End If
            End If

            Return Nothing
        End Function

        Public Function GetGenericProcessStatus(GenericProcessLogId As Integer,
                                                UserId As Integer,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Processes.GenericProcessLog
            Dim req As New DCSM.UtilityService.GetGenericProcessStatus.Request
            Dim res As New DCSM.UtilityService.GetGenericProcessStatus.Response

            With req.RequestData
                .GenericProcessLogId = GenericProcessLogId
                .UserId = UserId
            End With

            If (IFMS.Utility.GetGenericProcessStatus(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LogEntry
                End If
            End If

            Return Nothing
        End Function

        Public Function GetProximityTypes(Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.GeoCode.ProximityType)
            Dim req As New DCSM.UtilityService.GetProximityTypes.Request
            Dim res As New DCSM.UtilityService.GetProximityTypes.Response

            With req.RequestData
            End With

            If (IFMS.Utility.GetProximityTypes(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ProximityTypes
                End If
            End If

            Return Nothing
        End Function

        Public Function GetServerName(Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.UtilityService.GetServerName.Request
            Dim res As New DCSM.UtilityService.GetServerName.Response

            With req.RequestData
            End With

            If (IFMS.Utility.GetServerName(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ServerName
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSystemDate(Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Date
            Dim req As New DCSM.UtilityService.GetSystemDate.Request
            Dim res As New DCSM.UtilityService.GetSystemDate.Response

            With req.RequestData
            End With

            If (IFMS.Utility.GetSystemDate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.SystemDate
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSystemTime(Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As System.TimeSpan
            Dim req As New DCSM.UtilityService.GetSystemTime.Request
            Dim res As New DCSM.UtilityService.GetSystemTime.Response

            With req.RequestData
            End With

            If (IFMS.Utility.GetSystemTime(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.SystemTime
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUserOverrideEnables(StateId As Integer,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.GeoCode.UserOverrideEnable)
            Dim req As New DCSM.UtilityService.GetUserOverrideEnables.Request
            Dim res As New DCSM.UtilityService.GetUserOverrideEnables.Response

            With req.RequestData
                .StateId = StateId
            End With

            If (IFMS.Utility.GetUserOverrideEnables(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UserOverrideEnables
                End If
            End If

            Return Nothing
        End Function

        Public Function GetWindPools(Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.GeoCode.WindPool)
            Dim req As New DCSM.UtilityService.GetWindPools.Request
            Dim res As New DCSM.UtilityService.GetWindPools.Response

            With req.RequestData
            End With

            If (IFMS.Utility.GetWindPools(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WindPools
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadErrorLogRecords(ErrorCategoryTypeId As DCE.ErrorLog.ErrorCategory,
                                            ErrorSeverityId As DCE.ErrorLog.ErrorSeverity,
                                            ErrorTypeId As DCE.ErrorLog.ErrorType,
                                            FilterTypeId As DCE.ErrorLog.ErrorLogFilterType,
                                            FromDate As DCO.InsDateTime,
                                            PolicyId As Integer,
                                            ToDate As DCO.InsDateTime,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ErrorLog)
            Dim req As New DCSM.UtilityService.LoadErrorLogRecords.Request
            Dim res As New DCSM.UtilityService.LoadErrorLogRecords.Response

            With req.RequestData
                .ErrorCategoryTypeId = ErrorCategoryTypeId
                .ErrorSeverityId = ErrorSeverityId
                .ErrorTypeId = ErrorTypeId
                .FilterTypeId = FilterTypeId
                .FromDate = FromDate
                .PolicyId = PolicyId
                .ToDate = ToDate
            End With

            If (IFMS.Utility.LoadErrorLogRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Records
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadServerData(Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.UtilityService.LoadServerData.ResponseData
            Dim req As New DCSM.UtilityService.LoadServerData.Request
            Dim res As New DCSM.UtilityService.LoadServerData.Response

            With req.RequestData
            End With

            If (IFMS.Utility.LoadServerData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LookupGeoInfo(Address As DCO.Address,
                                      AddressTypeId As DCE.AddressType,
                                      LobId As Integer,
                                      PolicyId As Integer,
                                      PolicyImageNum As Integer,
                                      RiskAddedDate As DCO.InsDateTime,
                                      VersionId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Address
            Dim req As New DCSM.UtilityService.LookupGeoInfo.Request
            Dim res As New DCSM.UtilityService.LookupGeoInfo.Response

            With req.RequestData
                .Address = Address
                .AddressTypeId = AddressTypeId
                .LobId = LobId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .RiskAddedDate = RiskAddedDate
                .VersionId = VersionId
            End With

            If (IFMS.Utility.LookupGeoInfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Address
                End If
            End If

            Return Nothing
        End Function

        Public Function ParseAddress(Address As DCO.Address,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ParsedAddress)
            Dim req As New DCSM.UtilityService.ParseAddress.Request
            Dim res As New DCSM.UtilityService.ParseAddress.Response

            With req.RequestData
                .Address = Address
            End With

            If (IFMS.Utility.ParseAddress(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ParsedAddresses
                End If
            End If

            Return Nothing
        End Function

        Public Function SetSystemDate(SystemDate As Date,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.UtilityService.SetSystemDate.Request
            Dim res As New DCSM.UtilityService.SetSystemDate.Response

            With req.RequestData
                .SystemDate = SystemDate
            End With

            IFMS.Utility.SetSystemDate(res, req, e, dv)

            Return Nothing
        End Function

        Public Function TaxCodeFilterLookup(Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.TaxCodeFilter)
            Dim req As New DCSM.UtilityService.TaxCodeFilterLookup.Request
            Dim res As New DCSM.UtilityService.TaxCodeFilterLookup.Response

            With req.RequestData
            End With

            If (IFMS.Utility.TaxCodeFilterLookup(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Filters
                End If
            End If

            Return Nothing
        End Function

        Public Function TaxCodeLookup(Filter As String,
                                      PolicyId As Integer,
                                      PolicyImageNum As Integer,
                                      SearchString As Integer,
                                      State As String,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.TaxCode)
            Dim req As New DCSM.UtilityService.TaxCodeLookup.Request
            Dim res As New DCSM.UtilityService.TaxCodeLookup.Response

            With req.RequestData
                .Filter = Filter
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .SearchString = SearchString
                .State = State
            End With

            If (IFMS.Utility.TaxCodeLookup(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.TaxCodes
                End If
            End If

            Return Nothing
        End Function

        Public Function TaxCodeLookupExtended(GuaranteedRatePeriodEffectiveDate As DCO.InsDateTime,
                                              RiskAddedDate As DCO.InsDateTime,
                                              StateId As Integer,
                                              StreetName As String,
                                              StreetNumber As String,
                                              TransactionEffectiveDate As DCO.InsDateTime,
                                              ZipCode As String,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.TaxCode)
            Dim req As New DCSM.UtilityService.TaxCodeLookupExtended.Request
            Dim res As New DCSM.UtilityService.TaxCodeLookupExtended.Response

            With req.RequestData
                .GuaranteedRatePeriodEffectiveDate = GuaranteedRatePeriodEffectiveDate
                .RiskAddedDate = RiskAddedDate
                .StateId = StateId
                .StreetName = StreetName
                .StreetNumber = StreetNumber
                .TransactionEffectiveDate = TransactionEffectiveDate
                .ZipCode = ZipCode
            End With

            If (IFMS.Utility.TaxCodeLookupExtended(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.TaxCodes
                End If
            End If

            Return Nothing
        End Function

        Public Function TaxCodeLookupWithParse(Address As DCO.Address,
                                               GuaranteedRatePeriodEffectiveDate As DCO.InsDateTime,
                                               RiskAddedDate As DCO.InsDateTime,
                                               TransactionEffectiveDate As DCO.InsDateTime,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.TaxCode)
            Dim req As New DCSM.UtilityService.TaxCodeLookupWithParse.Request
            Dim res As New DCSM.UtilityService.TaxCodeLookupWithParse.Response

            With req.RequestData
                .Address = Address
                .GuaranteedRatePeriodEffectiveDate = GuaranteedRatePeriodEffectiveDate
                .RiskAddedDate = RiskAddedDate
                .TransactionEffectiveDate = TransactionEffectiveDate
            End With

            If (IFMS.Utility.TaxCodeLookupWithParse(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.TaxCodes
                End If
            End If

            Return Nothing
        End Function

        Public Function VerifyAddress(Criteria As DCO.AddressVerification.VerifiedAddressCriteria,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.AddressVerification.VerifiedAddressCriteria
            Dim req As New DCSM.UtilityService.VerifyAddress.Request
            Dim res As New DCSM.UtilityService.VerifyAddress.Response

            With req.RequestData
                .Criteria = Criteria
            End With

            If (IFMS.Utility.VerifyAddress(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Criteria
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace