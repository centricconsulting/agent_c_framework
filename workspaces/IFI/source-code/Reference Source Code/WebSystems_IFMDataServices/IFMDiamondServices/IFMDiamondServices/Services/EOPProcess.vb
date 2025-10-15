'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.EOPProcessService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.EOPProcess
    'not something we should use..
    Public Module EOPProcess
        Public Function AllowRunProcesses(ByRef res As DCSM.AllowRunProcesses.Response,
                                          ByRef req As DCSM.AllowRunProcesses.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.AllowRunProcesses
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BeginProcessing(ByRef res As DCSM.BeginProcessing.Response,
                                          ByRef req As DCSM.BeginProcessing.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.BeginProcessing
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetProcessStatus(ByRef res As DCSM.GetProcessStatus.Response,
                                          ByRef req As DCSM.GetProcessStatus.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetProcessStatus
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsEndOfPeriodRunning(ByRef res As DCSM.IsEndOfPeriodRunning.Response,
                                          ByRef req As DCSM.IsEndOfPeriodRunning.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsEndOfPeriodRunning
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsEOPComplete(ByRef res As DCSM.IsEOPComplete.Response,
                                          ByRef req As DCSM.IsEOPComplete.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsEOPComplete
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function LaunchProcess(ByRef res As DCSM.LaunchProcess.Response,
        '                                  ByRef req As DCSM.LaunchProcess.Request,
        '                                  Optional ByRef e As Exception = Nothing) As Boolean
        '    Dim p As New DCSP.EOPProcessServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.LaunchProcess
        '    res = RunDiamondService(m, req, e)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadInitializationData(ByRef res As DCSM.LoadInitializationData.Response,
                                          ByRef req As DCSM.LoadInitializationData.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadInitializationData
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ResetStatus(ByRef res As DCSM.ResetStatus.Response,
                                          ByRef req As DCSM.ResetStatus.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ResetStatus
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RollSystemDate(ByRef res As DCSM.RollSystemDate.Response,
                                          ByRef req As DCSM.RollSystemDate.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RollSystemDate
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function SaveServiceProcessJob(ByRef res As DCSM.,
        '                                  ByRef req As DCSM.SaveServiceProcessJob.Request,
        '                                  Optional ByRef e As Exception = Nothing) As Boolean
        '    Dim p As New DCSP.EOPProcessServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.SaveServiceProcessJob
        '    res = RunDiamondService(m, req, e)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function UpdateEOPLog(ByRef res As DCSM.UpdateEOPLog.Response,
                                          ByRef req As DCSM.UpdateEOPLog.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EOPProcessServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdateEOPLog
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

