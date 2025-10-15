'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.GlobalAdditionalInterestListManagementService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.GlobalAdditionalInterestListManagement
    Public Module GlobalAdditionalInterestListManagement
        Public Function CanDelete(ByRef res As DCSM.CanDelete.Response,
                                  ByRef req As DCSM.CanDelete.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CanDelete
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CanEdit(ByRef res As DCSM.CanEdit.Response,
                                  ByRef req As DCSM.CanEdit.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CanEdit
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Delete(ByRef res As DCSM.Delete.Response,
                                  ByRef req As DCSM.Delete.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Delete
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Load(ByRef res As DCSM.Load.Response,
                                  ByRef req As DCSM.Load.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Load
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadHistoryEntries(ByRef res As DCSM.LoadHistoryEntries.Response,
                                  ByRef req As DCSM.LoadHistoryEntries.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadHistoryEntries
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadHistoryEntriesForListId(ByRef res As DCSM.LoadHistoryEntriesForListId.Response,
                                  ByRef req As DCSM.LoadHistoryEntriesForListId.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadHistoryEntriesForListId
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Lookup(ByRef res As DCSM.Lookup.Response,
                                  ByRef req As DCSM.Lookup.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Lookup
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Save(ByRef res As DCSM.Save.Response,
                                  ByRef req As DCSM.Save.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Save
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveHistory(ByRef res As DCSM.SaveHistory.Response,
                                  ByRef req As DCSM.SaveHistory.Request,
                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.GlobalAdditionalInterestListManagementServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveHistory
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
     End Module
End Namespace
