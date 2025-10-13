Imports Microsoft.VisualBasic
Imports DCSA = Diamond.Common.Services.Messages.AdditionalInterestService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.AdditionalInterest
    Friend Module AdditionalInterest
        Public Function CanDelete(ByRef res As DCSA.CanDelete.Response,
                                  ByRef req As DCSA.CanDelete.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanDelete
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CanEdit(ByRef res As DCSA.CanEdit.Response,
                                  ByRef req As DCSA.CanEdit.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanEdit
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CopyAndUpdateAIForABT(ByRef res As DCSA.CopyAndUpdateAIForABT.Response,
                                  ByRef req As DCSA.CopyAndUpdateAIForABT.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CopyAndUpdateAIForABT
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteGlobalListEntry(ByRef res As DCSA.DeleteGlobalListEntry.Response,
                                  ByRef req As DCSA.DeleteGlobalListEntry.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteGlobalListEntry
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAIHistory(ByRef res As DCSA.LoadAIHistory.Response,
                                  ByRef req As DCSA.LoadAIHistory.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAIHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        ''obsolete
        'Public Function LoadAIListHistory(ByRef res As DCSA.LoadAIListHistory.Response,
        '                          ByRef req As DCSA.LoadAIListHistory.Request,
        '                          Optional ByRef e As Exception = Nothing,Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.AdditionalInterestServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.LoadAIListHistory
        '   res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadForId(ByRef res As DCSA.LoadForId.Response,
                                  ByRef req As DCSA.LoadForId.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadForId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleAIHistory(ByRef res As DCSA.LoadSingleAIHistory.Response,
                                  ByRef req As DCSA.LoadSingleAIHistory.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleAIHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LookupLoad(ByRef res As DCSA.LookupLoad.Response,
                                  ByRef req As DCSA.LookupLoad.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LookupLoad
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAIList(ByRef res As DCSA.SaveAIList.Response,
                                  ByRef req As DCSA.SaveAIList.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAIList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAIListHistory(ByRef res As DCSA.SaveAIListHistory.Response,
                                  ByRef req As DCSA.SaveAIListHistory.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAIListHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidSearchData(ByRef res As DCSA.ValidSearchData.Response,
                                  ByRef req As DCSA.ValidSearchData.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AdditionalInterestServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidSearchData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
