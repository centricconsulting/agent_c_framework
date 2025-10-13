Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Lookup
    Public Module Lookup
        Public Function GetNewsItem(SearchInfo As DCO.Lookup.CityStateZipCountyLookup,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.CityStateZipCountyLookup)
            Dim res As New DCSM.LookupService.CityStateZipCountyLookup.Response
            Dim req As New DCSM.LookupService.CityStateZipCountyLookup.Request

            With req.RequestData
                .SearchInfo = SearchInfo
            End With

            If IFMS.Lookup.CityStateZipCountyLookup(res, req, e, dv) Then
                Return res.ResponseData.CityStateZipCountyLookups
            End If
            Return Nothing
        End Function

        Public Function DoProtectionClassSearch(FeetToHydrant As Integer,
                                                MilesToFireDepartment As Integer,
                                                SearchText As String,
                                                SearchType As DCE.ProtectionClass.PCLookupType,
                                                VersionId As Integer,
                                                Optional ByRef e As System.Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.ProtectionClassLookup)
            Dim res As New DCSM.LookupService.DoProtectionClassSearch.Response
            Dim req As New DCSM.LookupService.DoProtectionClassSearch.Request

            With req.RequestData
                .FeetToHydrant = FeetToHydrant
                .MilesToFireDepartment = MilesToFireDepartment
                .SearchText = SearchText
                .SearchType = SearchType
                .VersionID = VersionId
            End With

            If IFMS.Lookup.DoProtectionClassSearch(res, req, e, dv) Then
                Return res.ResponseData.ProtectionClasses
            End If
            Return Nothing
        End Function

        Public Function DoRiskGradeSearch(SearchText As String,
                                          SearchType As DCE.Lookup.RiskGradeSelectionType,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.RiskGradeLookup)
            Dim res As New DCSM.LookupService.DoRiskGradeSearch.Response
            Dim req As New DCSM.LookupService.DoRiskGradeSearch.Request

            With req.RequestData
                .SearchText = SearchText
                .SearchType = SearchType
            End With

            If IFMS.Lookup.DoRiskGradeSearch(res, req, e, dv) Then
                Return res.ResponseData.RiskGradeLookups
            End If
            Return Nothing
        End Function

        Public Function GetAgencyData(Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.AgencyData)
            Dim res As New DCSM.LookupService.GetAgencyData.Response
            Dim req As New DCSM.LookupService.GetAgencyData.Request

            With req.RequestData
            End With

            If IFMS.Lookup.GetAgencyData(res, req, e, dv) Then
                Return res.ResponseData.Agencies
            End If
            Return Nothing
        End Function

        Public Function GetAgencyDataForAgencyId(AgencyId As Integer,
                                                 Optional ByRef e As System.Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.AgencyData
            Dim res As New DCSM.LookupService.GetAgencyDataForAgencyId.Response
            Dim req As New DCSM.LookupService.GetAgencyDataForAgencyId.Request

            With req.RequestData
                .AgencyId = AgencyId
            End With

            If IFMS.Lookup.GetAgencyDataForAgencyId(res, req, e, dv) Then
                Return res.ResponseData.Agency
            End If
            Return Nothing
        End Function

        Public Function GetAgencyDataForCode(Code As String,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.AgencyData
            Dim res As New DCSM.LookupService.GetAgencyDataForCode.Response
            Dim req As New DCSM.LookupService.GetAgencyDataForCode.Request

            With req.RequestData
                .Code = Code
            End With

            If IFMS.Lookup.GetAgencyDataForCode(res, req, e, dv) Then
                Return res.ResponseData.Agency
            End If
            Return Nothing
        End Function

        Public Function GetAgencyDataForCSL(CompanyId As Integer,
                                            LobId As Integer,
                                            StateId As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.AgencyData)
            Dim res As New DCSM.LookupService.GetAgencyDataForCSL.Response
            Dim req As New DCSM.LookupService.GetAgencyDataForCSL.Request

            With req.RequestData
                .CompanyId = CompanyId
                .LobId = LobId
                .StateId = StateId
            End With

            If IFMS.Lookup.GetAgencyDataForCSL(res, req, e, dv) Then
                Return res.ResponseData.Agencies
            End If
            Return Nothing
        End Function

        Public Function GetCSLA(Optional ByRef e As System.Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetCSLA)
            Dim res As New DCSM.LookupService.GetCSLA.Response
            Dim req As New DCSM.LookupService.GetCSLA.Request

            With req.RequestData
            End With

            If IFMS.Lookup.GetCSLA(res, req, e, dv) Then
                Return res.ResponseData.CSLAs
            End If
            Return Nothing
        End Function

        Public Function GetProducers(Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetProducers)
            Dim res As New DCSM.LookupService.GetProducers.Response
            Dim req As New DCSM.LookupService.GetProducers.Request

            With req.RequestData
            End With

            If IFMS.Lookup.GetProducers(res, req, e, dv) Then
                Return res.ResponseData.Producers
            End If
            Return Nothing
        End Function

        Public Function GetProducersByAgency(AgencyId As Integer,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetProducersByAgency)
            Dim res As New DCSM.LookupService.GetProducersByAgency.Response
            Dim req As New DCSM.LookupService.GetProducersByAgency.Request

            With req.RequestData
                .AgencyId = AgencyId
            End With

            If IFMS.Lookup.GetProducersByAgency(res, req, e, dv) Then
                Return res.ResponseData.Producers
            End If
            Return Nothing
        End Function

        Public Function GetProducersByCSLAEff(AgencyId As Integer,
                                              CompanyId As Integer,
                                              EffectiveDate As Date,
                                              Lobid As Integer,
                                              StateId As Integer,
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetProducersByCSLAEff)
            Dim res As New DCSM.LookupService.GetProducersByCSLAEff.Response
            Dim req As New DCSM.LookupService.GetProducersByCSLAEff.Request

            With req.RequestData
                .AgencyId = AgencyId
                .CompanyId = CompanyId
                .EffectiveDate = EffectiveDate
                .LobId = Lobid
                .StateId = StateId
            End With

            If IFMS.Lookup.GetProducersByCSLAEff(res, req, e, dv) Then
                Return res.ResponseData.Producers
            End If
            Return Nothing
        End Function

        Public Function GetUser(UserId As Integer,
                                Optional ByRef e As System.Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Lookup.GetUser
            Dim res As New DCSM.LookupService.GetUser.Response
            Dim req As New DCSM.LookupService.GetUser.Request

            With req.RequestData
                .UsersId = UserId
            End With

            If IFMS.Lookup.GetUser(res, req, e, dv) Then
                Return res.ResponseData.User
            End If
            Return Nothing
        End Function

        Public Function GetUserAgencies(Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetUserAgency)
            Dim res As New DCSM.LookupService.GetUserAgencies.Response
            Dim req As New DCSM.LookupService.GetUserAgencies.Request

            With req.RequestData
            End With

            If IFMS.Lookup.GetUserAgencies(res, req, e, dv) Then
                Return res.ResponseData.UserAgencies
            End If
            Return Nothing
        End Function

        Public Function GetUserAgencyByUser(UserId As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetUserAgency)
            Dim res As New DCSM.LookupService.GetUserAgencyByUser.Response
            Dim req As New DCSM.LookupService.GetUserAgencyByUser.Request

            With req.RequestData
                .UsersId = UserId
            End With

            If IFMS.Lookup.GetUserAgencyByUser(res, req, e, dv) Then
                Return res.ResponseData.UserAgencies
            End If
            Return Nothing
        End Function

        Public Function GetUsers(Optional ByRef e As System.Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.GetUsers)
            Dim res As New DCSM.LookupService.GetUsers.Response
            Dim req As New DCSM.LookupService.GetUsers.Request

            With req.RequestData
            End With

            If IFMS.Lookup.GetUsers(res, req, e, dv) Then
                Return res.ResponseData.Users
            End If
            Return Nothing
        End Function

        Public Function LoadToAgency(Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.LoadToAgency)
            Dim res As New DCSM.LookupService.LoadToAgency.Response
            Dim req As New DCSM.LookupService.LoadToAgency.Request

            With req.RequestData
            End With

            If IFMS.Lookup.LoadToAgency(res, req, e, dv) Then
                Return res.ResponseData.Agencies
            End If
            Return Nothing
        End Function

        Public Function LookupByCriteria(Criteria As Object,
                                         ExactMatch As Boolean,
                                         Parameters As DCO.InsCollection(Of DCO.Lookup.Advanced.Input),
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Lookup.Advanced.Result)
            Dim res As New DCSM.LookupService.LookupByCriteria.Response
            Dim req As New DCSM.LookupService.LookupByCriteria.Request

            With req.RequestData
                .Criteria = Criteria
                .ExactMatch = ExactMatch
                .Parameters = Parameters

            End With

            If IFMS.Lookup.LookupByCriteria(res, req, e, dv) Then
                Return res.ResponseData.Results
            End If
            Return Nothing
        End Function
    End Module
End Namespace