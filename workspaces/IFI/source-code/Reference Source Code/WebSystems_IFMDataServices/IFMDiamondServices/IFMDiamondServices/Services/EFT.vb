Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.EFTService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.EFT
    Public Module EFT
        Public Function BeginEFTFileProcessing(ByRef res As DCSM.BeginEFTFileProcessing.Response,
                                               ByRef req As DCSM.BeginEFTFileProcessing.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.BeginEFTFileProcessing
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateEFTFile(ByRef res As DCSM.CreateEFTFile.Response,
                                               ByRef req As DCSM.CreateEFTFile.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateEFTFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteEFTGroup(ByRef res As DCSM.DeleteEFTGroup.Response,
                                               ByRef req As DCSM.DeleteEFTGroup.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteEFTGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllEFTGroups(ByRef res As DCSM.LoadAllEFTGroups.Response,
                                               ByRef req As DCSM.LoadAllEFTGroups.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllEFTGroups
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEFTGroup(ByRef res As DCSM.LoadEFTGroup.Response,
                                               ByRef req As DCSM.LoadEFTGroup.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEFTGroup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSetupInfo(ByRef res As DCSM.LoadSetupInfo.Response,
                                               ByRef req As DCSM.LoadSetupInfo.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSetupInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessEFTReturnFile(ByRef res As DCSM.ProcessEFTReturnFile.Response,
                                               ByRef req As DCSM.ProcessEFTReturnFile.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessEFTReturnFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveEFTGroupInfo(ByRef res As DCSM.SaveEFTGroupInfo.Response,
                                               ByRef req As DCSM.SaveEFTGroupInfo.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveEFTGroupInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePolicyEftInfo(ByRef res As DCSM.SavePolicyEftInfo.Response,
                                               ByRef req As DCSM.SavePolicyEftInfo.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePolicyEftInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ValidateRoutingNumberAgainstList(ByRef res As DCSM.ValidateRoutingNumberAgainstList.Response,
                                                         ByRef req As DCSM.ValidateRoutingNumberAgainstList.Request,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.EFTServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateRoutingNumberAgainstList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

