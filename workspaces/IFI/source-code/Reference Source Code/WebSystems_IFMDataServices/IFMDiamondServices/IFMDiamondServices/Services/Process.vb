'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.ProcessService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Process
    Public Module Process
        Public Function BatchEndorsementsLoadCompanyStateLob(ByRef res As DCSM.BatchEndorsementsLoadCompanyStateLob.Response,
                                                             ByRef req As DCSM.BatchEndorsementsLoadCompanyStateLob.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.BatchEndorsementsLoadCompanyStateLob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BatchEndorsementsLoadPolicies(ByRef res As DCSM.BatchEndorsementsLoadPolicies.Response,
                                                             ByRef req As DCSM.BatchEndorsementsLoadPolicies.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.BatchEndorsementsLoadPolicies
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BatchEndorsementsLoadRatingVersion(ByRef res As DCSM.BatchEndorsementsLoadRatingVersion.Response,
                                                             ByRef req As DCSM.BatchEndorsementsLoadRatingVersion.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.BatchEndorsementsLoadRatingVersion
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BatchEndorsementsProcessEndorsement(ByRef res As DCSM.BatchEndorsementsProcessEndorsement.Response,
                                                             ByRef req As DCSM.BatchEndorsementsProcessEndorsement.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.BatchEndorsementsProcessEndorsement
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreatePayrollDeductionFiles(ByRef res As DCSM.CreatePayrollDeductionFiles.Response,
                                                             ByRef req As DCSM.CreatePayrollDeductionFiles.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreatePayrollDeductionFiles
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteXSLMap(ByRef res As DCSM.DeleteXSLMap.Response,
                                                             ByRef req As DCSM.DeleteXSLMap.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteXSLMap
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExecutePeoplesoft(ByRef res As DCSM.ExecutePeoplesoft.Response,
                                                             ByRef req As DCSM.ExecutePeoplesoft.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExecutePeoplesoft
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExecuteUpload(ByRef res As DCSM.ExecuteUpload.Response,
                                                             ByRef req As DCSM.ExecuteUpload.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExecuteUpload
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllXSLMaps(ByRef res As DCSM.LoadAllXSLMaps.Response,
                                                             ByRef req As DCSM.LoadAllXSLMaps.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAllXSLMaps
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadXSLMapFromFile(ByRef res As DCSM.LoadXSLMapFromFile.Response,
                                                             ByRef req As DCSM.LoadXSLMapFromFile.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadXSLMapFromFile
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadXSLMapFromTable(ByRef res As DCSM.LoadXSLMapFromTable.Response,
                                                             ByRef req As DCSM.LoadXSLMapFromTable.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadXSLMapFromTable
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveXSLMap(ByRef res As DCSM.SaveXSLMap.Response,
                                                             ByRef req As DCSM.SaveXSLMap.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveXSLMap
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveXSLMapsVersion(ByRef res As DCSM.SaveXSLMapsVersion.Response,
                                                             ByRef req As DCSM.SaveXSLMapsVersion.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveXSLMapsVersion
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveXSLMapToFile(ByRef res As DCSM.SaveXSLMapToFile.Response,
                                                             ByRef req As DCSM.SaveXSLMapToFile.Request,
                                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveXSLMapToFile
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
