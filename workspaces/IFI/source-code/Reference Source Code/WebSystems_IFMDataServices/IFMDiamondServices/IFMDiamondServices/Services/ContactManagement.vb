Imports Microsoft.VisualBasic
Imports DCSC = Diamond.Common.Services.Messages.ContactManagementService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.ContactManagement
    Public Module ContactManagement
        Public Function Import(ByRef res As DCSC.Import.Response,
                               ByRef req As DCSC.Import.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Import
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Load(ByRef res As DCSC.Load.Response,
                               ByRef req As DCSC.Load.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Load
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PurgeContacts(ByRef res As DCSC.PurgeContacts.Response,
                               ByRef req As DCSC.PurgeContacts.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.PurgeContacts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Save(ByRef res As DCSC.Save.Response,
                               ByRef req As DCSC.Save.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Save
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SearchByAddress(ByRef res As DCSC.SearchByAddress.Response,
                               ByRef req As DCSC.SearchByAddress.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SearchByAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SearchByName(ByRef res As DCSC.SearchByName.Response,
                               ByRef req As DCSC.SearchByName.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SearchByName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SearchByPreferredCustomerNumber(ByRef res As DCSC.SearchByPreferredCustomerNumber.Response,
                               ByRef req As DCSC.SearchByPreferredCustomerNumber.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ContactManagementServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SearchByPreferredCustomerNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
     End Module
End Namespace
