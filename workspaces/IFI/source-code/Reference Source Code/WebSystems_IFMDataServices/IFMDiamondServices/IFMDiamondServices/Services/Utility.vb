Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.UtilityService
Imports DCSP = Diamond.Common.Services.Proxies.Utility
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.Utility
    Public Module Utility
        Public Function AddTaxCodeToLocation(ByRef res As DSCM.AddTaxCodeToLocation.Response,
                                             ByRef req As DSCM.AddTaxCodeToLocation.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.AddTaxCodeToLocation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function AgencyTaxCodeSCLookup(ByRef res As DSCM.AgencyTaxCodeSCLookup.Response,
                                              ByRef req As DSCM.AgencyTaxCodeSCLookup.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.AgencyTaxCodeSCLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GenerateTestError(ByRef res As DSCM.GenerateTestError.Response,
                                          ByRef req As DSCM.GenerateTestError.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GenerateTestError
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetGenericProcessStatus(ByRef res As DSCM.GetGenericProcessStatus.Response,
                                                ByRef req As DSCM.GetGenericProcessStatus.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetGenericProcessStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetProximityTypes(ByRef res As DSCM.GetProximityTypes.Response,
                                          ByRef req As DSCM.GetProximityTypes.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetProximityTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetServerName(ByRef res As DSCM.GetServerName.Response,
                                      ByRef req As DSCM.GetServerName.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetServerName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSystemDate(ByRef res As DSCM.GetSystemDate.Response,
                                      ByRef req As DSCM.GetSystemDate.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSystemDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSystemTime(ByRef res As DSCM.GetSystemTime.Response,
                                      ByRef req As DSCM.GetSystemTime.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSystemTime
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUserOverrideEnables(ByRef res As DSCM.GetUserOverrideEnables.Response,
                                               ByRef req As DSCM.GetUserOverrideEnables.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetUserOverrideEnables
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetWindPools(ByRef res As DSCM.GetWindPools.Response,
                                     ByRef req As DSCM.GetWindPools.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetWindPools
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadErrorLogRecords(ByRef res As DSCM.LoadErrorLogRecords.Response,
                                            ByRef req As DSCM.LoadErrorLogRecords.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadErrorLogRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadServerData(ByRef res As DSCM.LoadServerData.Response,
                                       ByRef req As DSCM.LoadServerData.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadServerData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LookupGeoInfo(ByRef res As DSCM.LookupGeoInfo.Response,
                                      ByRef req As DSCM.LookupGeoInfo.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LookupGeoInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ParseAddress(ByRef res As DSCM.ParseAddress.Response,
                                     ByRef req As DSCM.ParseAddress.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ParseAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SetSystemDate(ByRef res As DSCM.SetSystemDate.Response,
                                      ByRef req As DSCM.SetSystemDate.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SetSystemDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TaxCodeFilterLookup(ByRef res As DSCM.TaxCodeFilterLookup.Response,
                                            ByRef req As DSCM.TaxCodeFilterLookup.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TaxCodeFilterLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TaxCodeLookup(ByRef res As DSCM.TaxCodeLookup.Response,
                                      ByRef req As DSCM.TaxCodeLookup.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TaxCodeLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TaxCodeLookupExtended(ByRef res As DSCM.TaxCodeLookupExtended.Response,
                                              ByRef req As DSCM.TaxCodeLookupExtended.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TaxCodeLookupExtended
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TaxCodeLookupWithParse(ByRef res As DSCM.TaxCodeLookupWithParse.Response,
                                               ByRef req As DSCM.TaxCodeLookupWithParse.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TaxCodeLookupWithParse
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VerifyAddress(ByRef res As DSCM.VerifyAddress.Response,
                                      ByRef req As DSCM.VerifyAddress.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.UtilityServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.VerifyAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace