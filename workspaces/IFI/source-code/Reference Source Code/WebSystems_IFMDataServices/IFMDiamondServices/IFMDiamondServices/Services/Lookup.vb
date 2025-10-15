Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.LookupService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.Lookup
    Public Module Lookup
        Public Function CityStateZipCountyLookup(ByRef res As DCSM.CityStateZipCountyLookup.Response,
                                                 ByRef req As DCSM.CityStateZipCountyLookup.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CityStateZipCountyLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DoProtectionClassSearch(ByRef res As DCSM.DoProtectionClassSearch.Response,
                                                ByRef req As DCSM.DoProtectionClassSearch.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DoProtectionClassSearch
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DoRiskGradeSearch(ByRef res As DCSM.DoRiskGradeSearch.Response,
                                                ByRef req As DCSM.DoRiskGradeSearch.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DoRiskGradeSearch
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyData(ByRef res As DCSM.GetAgencyData.Response,
                                                ByRef req As DCSM.GetAgencyData.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyDataForAgencyId(ByRef res As DCSM.GetAgencyDataForAgencyId.Response,
                                                ByRef req As DCSM.GetAgencyDataForAgencyId.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyDataForAgencyId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyDataForCode(ByRef res As DCSM.GetAgencyDataForCode.Response,
                                                ByRef req As DCSM.GetAgencyDataForCode.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyDataForCode
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyDataForCSL(ByRef res As DCSM.GetAgencyDataForCSL.Response,
                                                ByRef req As DCSM.GetAgencyDataForCSL.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyDataForCSL
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCSLA(ByRef res As DCSM.GetCSLA.Response,
                                                ByRef req As DCSM.GetCSLA.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetCSLA
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetLookupParameters(ByRef res As DCSM.LookupParameters.Response,
                                                ByRef req As DCSM.LookupParameters.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetLookupParameters
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetProducers(ByRef res As DCSM.GetProducers.Response,
                                                ByRef req As DCSM.GetProducers.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetProducers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetProducersByAgency(ByRef res As DCSM.GetProducersByAgency.Response,
                                                ByRef req As DCSM.GetProducersByAgency.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetProducersByAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetProducersByCSLAEff(ByRef res As DCSM.GetProducersByCSLAEff.Response,
                                                ByRef req As DCSM.GetProducersByCSLAEff.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetProducersByCSLAEff
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetUser(ByRef res As DCSM.GetUser.Response,
                                                ByRef req As DCSM.GetUser.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetUserAgencies(ByRef res As DCSM.GetUserAgencies.Response,
                                                ByRef req As DCSM.GetUserAgencies.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUserAgencies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetUserAgencyByUser(ByRef res As DCSM.GetUserAgencyByUser.Response,
                                                ByRef req As DCSM.GetUserAgencyByUser.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUserAgencyByUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetUsers(ByRef res As DCSM.GetUsers.Response,
                                                ByRef req As DCSM.GetUsers.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadToAgency(ByRef res As DCSM.LoadToAgency.Response,
                                                ByRef req As DCSM.LoadToAgency.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadToAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LookupByCriteria(ByRef res As DCSM.LookupByCriteria.Response,
                                                ByRef req As DCSM.LookupByCriteria.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.LookupServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LookupByCriteria
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
