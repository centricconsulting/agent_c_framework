Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.ReplacementCostService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.ReplacementCost
    Public Module ReplacementCost
        Public Function CreateCostEstimate(ByRef res As DCSM.CreateCostEstimate.Response,
                                           ByRef req As DCSM.CreateCostEstimate.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReplacementCostServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateCostEstimate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Import(ByRef res As DCSM.Import.Response,
                                           ByRef req As DCSM.Import.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReplacementCostServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Import
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLocationCostEstimate(ByRef res As DCSM.LoadLocationCostEstimate.Response,
                                           ByRef req As DCSM.LoadLocationCostEstimate.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReplacementCostServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLocationCostEstimate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLocationCostEstimate(ByRef res As DCSM.SaveLocationCostEstimate.Response,
                                           ByRef req As DCSM.SaveLocationCostEstimate.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReplacementCostServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLocationCostEstimate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdatePolicyNumber(ByRef res As DCSM.UpdatePolicyNumber.Response,
                                           ByRef req As DCSM.UpdatePolicyNumber.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReplacementCostServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdatePolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
