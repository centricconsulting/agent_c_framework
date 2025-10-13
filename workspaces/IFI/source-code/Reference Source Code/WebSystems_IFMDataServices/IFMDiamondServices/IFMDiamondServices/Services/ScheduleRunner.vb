'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.ScheduleRunnerService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.ScheduleRunner
    Public Module ScheduleRunnerProxy
        Public Function DeleteJob(ByRef res As DSCM.DeleteJob.Response,
                                  ByRef req As DSCM.DeleteJob.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteSchedule(ByRef res As DSCM.DeleteSchedule.Response,
                                       ByRef req As DSCM.DeleteSchedule.Request,
                                       Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteSchedule
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadEopJob(ByRef res As DSCM.LoadEopJob.Response,
                                   ByRef req As DSCM.LoadEopJob.Request,
                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadEopJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        'Public Function LoadEopJobSupportData(ByRef res As DSCM.LoadEopJobSupportData.Response,
        '                                      ByRef req As DSCM.LoadEopJobSupportData.Request,
        '                                      Optional ByRef e As Exception = Nothing) As Boolean
        '    Dim p As New DCSP.ScheduleRunnerServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.LoadEopJobSupportData
        '   res = RunDiamondService(m, req, e)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        Public Function LoadErrorLogJob(ByRef res As DSCM.LoadErrorLogJob.Response,
                                        ByRef req As DSCM.LoadErrorLogJob.Request,
                                        Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadErrorLogJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadGenericExeJob(ByRef res As DSCM.LoadGenericExeJob.Response,
                                          ByRef req As DSCM.LoadGenericExeJob.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadGenericExeJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadJobList(ByRef res As DSCM.LoadJobList.Response,
                                    ByRef req As DSCM.LoadJobList.Request,
                                    Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadJobList
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyExtractJob(ByRef res As DSCM.LoadPolicyExtractJob.Response,
                                             ByRef req As DSCM.LoadPolicyExtractJob.Request,
                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyExtractJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadRenewalCSLAs(ByRef res As DSCM.LoadRenewalCSLAs.Response,
                                         ByRef req As DSCM.LoadRenewalCSLAs.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadRenewalCSLAs
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        'Public Function LoadRenewalItems(ByRef res As DSCM.LoadRenewalItems.Response,
        '                                 ByRef req As DSCM.LoadRenewalItems.Request,
        '                                 Optional ByRef e As Exception = Nothing) As Boolean
        '    Dim p As New DCSP.ScheduleRunnerServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.LoadRenewalItems
        '   res = RunDiamondService(m, req, e)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        Public Function LoadRenewalJob(ByRef res As DSCM.LoadRenewalJob.Response,
                                       ByRef req As DSCM.LoadRenewalJob.Request,
                                       Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadRenewalJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadSchedule(ByRef res As DSCM.LoadSchedule.Response,
                                     ByRef req As DSCM.LoadSchedule.Request,
                                     Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadSchedule
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadScheduleList(ByRef res As DSCM.LoadScheduleList.Response,
                                         ByRef req As DSCM.LoadScheduleList.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadScheduleList
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessSchedule(ByRef res As DSCM.ProcessSchedule.Response,
                                        ByRef req As DSCM.ProcessSchedule.Request,
                                        Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessSchedule
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function RunScheduledJobs(ByRef res As DSCM.RunScheduledJobs.Response,
                                         ByRef req As DSCM.RunScheduledJobs.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RunScheduledJobs
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveEopJob(ByRef res As DSCM.SaveEopJob.Response,
                                   ByRef req As DSCM.SaveEopJob.Request,
                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveEopJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveErrorLogJob(ByRef res As DSCM.SaveErrorLogJob.Response,
                                        ByRef req As DSCM.SaveErrorLogJob.Request,
                                        Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveErrorLogJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveGenericExeJob(ByRef res As DSCM.SaveGenericExeJob.Response,
                                          ByRef req As DSCM.SaveGenericExeJob.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveGenericExeJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SavePolicyExtractJob(ByRef res As DSCM.SavePolicyExtractJob.Response,
                                             ByRef req As DSCM.SavePolicyExtractJob.Request,
                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SavePolicyExtractJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveRenewalJob(ByRef res As DSCM.SaveRenewalJob.Response,
                                       ByRef req As DSCM.SaveRenewalJob.Request,
                                       Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveRenewalJob
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveSchedule(ByRef res As DSCM.SaveSchedule.Response,
                                     ByRef req As DSCM.SaveSchedule.Request,
                                     Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ScheduleRunnerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveSchedule
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace