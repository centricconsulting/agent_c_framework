Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.VinService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.VinService
    Public Module VinServiceProxy
        Public Function DeleteModelSqlScript(ByRef res As DSCM.DeleteModelSqlScript.Response,
                                             ByRef req As DSCM.DeleteModelSqlScript.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteModelSqlScript
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function EditISOModel(ByRef res As DSCM.EditISOModel.Response,
                                     ByRef req As DSCM.EditISOModel.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.EditISOModel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVehicleInfoLookupMappings(ByRef res As DSCM.GetVehicleInfoLookupMappings.Response,
                                                     ByRef req As DSCM.GetVehicleInfoLookupMappings.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVehicleInfoLookupMappings
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadIndicators(ByRef res As DSCM.LoadIndicators.Response,
                                       ByRef req As DSCM.LoadIndicators.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadIndicators
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadMakes(ByRef res As DSCM.LoadMakes.Response,
                                  ByRef req As DSCM.LoadMakes.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadMakes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadModelSqlScript(ByRef res As DSCM.LoadModelSqlScript.Response,
                                           ByRef req As DSCM.LoadModelSqlScript.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadModelSqlScript
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadVehicleInfoLookupTypes(ByRef res As DSCM.LoadVehicleInfoLookupTypes.Response,
                                                   ByRef req As DSCM.LoadVehicleInfoLookupTypes.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadVehicleInfoLookupTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ModelISOLoad(ByRef res As DSCM.ModelISOLoad.Response,
                                     ByRef req As DSCM.ModelISOLoad.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ModelISOLoad
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ModelISOLoadNextRecords(ByRef res As DSCM.ModelISOLoadNextRecords.Response,
                                                ByRef req As DSCM.ModelISOLoadNextRecords.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ModelISOLoadNextRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ModelISOLoadPrevRecords(ByRef res As DSCM.ModelISOLoadPrevRecords.Response,
                                                ByRef req As DSCM.ModelISOLoadPrevRecords.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ModelISOLoadPrevRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ModelISOSave(ByRef res As DSCM.ModelISOSave.Response,
                                     ByRef req As DSCM.ModelISOSave.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ModelISOSave
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessIsoUpdate(ByRef res As DSCM.ProcessIsoUpdate.Response,
                                         ByRef req As DSCM.ProcessIsoUpdate.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessIsoUpdate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessModelScript(ByRef res As DSCM.ProcessModelScript.Response,
                                           ByRef req As DSCM.ProcessModelScript.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessModelScript
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveModelSqlScript(ByRef res As DSCM.SaveModelSqlScript.Response,
                                           ByRef req As DSCM.SaveModelSqlScript.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveModelSqlScript
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveRedbookEntry(ByRef res As DSCM.SaveRedbookEntry.Response,
                                         ByRef req As DSCM.SaveRedbookEntry.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveRedbookEntry
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VehicleInfoLookup(ByRef res As DSCM.VehicleInfoLookup.Response,
                                          ByRef req As DSCM.VehicleInfoLookup.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean

            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.VehicleInfoLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VehicleSymbolPlanLookup(ByRef res As DSCM.VehicleSymbolPlanLookup.Response,
                                                ByRef req As DSCM.VehicleSymbolPlanLookup.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VinServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.VehicleSymbolPlanLookup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace