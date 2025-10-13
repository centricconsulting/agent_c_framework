'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.VisualTreeService
Imports DCSP = Diamond.Common.Services.Proxies.VisualTreeServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.VisualTreeService
    Public Module VisualTreeService
        Public Function GetClientByID(ByRef res As DSCM.GetClientByID.Response,
                                        ByRef req As DSCM.GetClientByID.Request,
                                        Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientByID
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientByPolicyID(ByRef res As DSCM.GetClientByPolicyID.Response,
                                        ByRef req As DSCM.GetClientByPolicyID.Request,
                                        Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientByPolicyID
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientByPolicyNumber(ByRef res As DSCM.GetClientByPolicyNumber.Response,
                                                ByRef req As DSCM.GetClientByPolicyNumber.Request,
                                                Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientByPolicyNumber
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientPoliciesByID(ByRef res As DSCM.GetClientPoliciesByID.Response,
                                              ByRef req As DSCM.GetClientPoliciesByID.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientPoliciesByID
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientPolicyByPolicyID(ByRef res As DSCM.GetClientPolicyByPolicyID.Response,
                                                  ByRef req As DSCM.GetClientPolicyByPolicyID.Request,
                                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientPolicyByPolicyID
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientPolicyByPolicyNumber(ByRef res As DSCM.GetClientPolicyByPolicyNumber.Response,
                                                      ByRef req As DSCM.GetClientPolicyByPolicyNumber.Request,
                                                      Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientPolicyByPolicyNumber
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientsByAVSClientCode(ByRef res As DSCM.GetClientsByAVSClientCode.Response,
                                                  ByRef req As DSCM.GetClientsByAVSClientCode.Request,
                                                  Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientsByAVSClientCode
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientsByBillingAcctNumber(ByRef res As DSCM.GetClientsByBillingAcctNumber.Response,
                                                      ByRef req As DSCM.GetClientsByBillingAcctNumber.Request,
                                                      Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientsByBillingAcctNumber
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientsPoliciesByAVSClientCode(ByRef res As DSCM.GetClientsPoliciesByAVSClientCode.Response,
                                                          ByRef req As DSCM.GetClientsPoliciesByAVSClientCode.Request,
                                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientsPoliciesByAVSClientCode
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientsPoliciesByBillingAcctNumber(ByRef res As DSCM.GetClientsPoliciesByBillingAcctNumber.Response,
                                                              ByRef req As DSCM.GetClientsPoliciesByBillingAcctNumber.Request,
                                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientsPoliciesByBillingAcctNumber
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNextClientPolicyRecords(ByRef res As DSCM.GetNextClientPolicyRecords.Response,
                                                   ByRef req As DSCM.GetNextClientPolicyRecords.Request,
                                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNextClientPolicyRecords
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNextClientRecords(ByRef res As DSCM.GetNextClientRecords.Response,
                                             ByRef req As DSCM.GetNextClientRecords.Request,
                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNextClientRecords
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNextClientRecordsByName(ByRef res As DSCM.GetNextClientRecordsByName.Response,
                                                   ByRef req As DSCM.GetNextClientRecordsByName.Request,
                                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNextClientRecordsByName
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyArchivedQuotes(ByRef res As DSCM.GetPolicyArchivedQuotes.Response,
                                                ByRef req As DSCM.GetPolicyArchivedQuotes.Request,
                                                Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyArchivedQuotes
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyHistory(ByRef res As DSCM.GetPolicyHistory.Response,
                                         ByRef req As DSCM.GetPolicyHistory.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyHistory
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyProperties(ByRef res As DSCM.GetPolicyProperties.Response,
                                            ByRef req As DSCM.GetPolicyProperties.Request,
                                            Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyProperties
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyQuotes(ByRef res As DSCM.GetPolicyQuotes.Response,
                                        ByRef req As DSCM.GetPolicyQuotes.Request,
                                        Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyQuotes
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyTopLevel(ByRef res As DSCM.GetPolicyTopLevel.Response,
                                          ByRef req As DSCM.GetPolicyTopLevel.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyTopLevel
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyTopLevelArchivedQuote(ByRef res As DSCM.GetPolicyTopLevelArchivedQuote.Response,
                                                       ByRef req As DSCM.GetPolicyTopLevelArchivedQuote.Request,
                                                       Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyTopLevelArchivedQuote
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPreviousClientRecords(ByRef res As DSCM.GetPreviousClientRecords.Response,
                                                 ByRef req As DSCM.GetPreviousClientRecords.Request,
                                                 Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPreviousClientRecords
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVTreeClaimFeatures(ByRef res As DSCM.GetVTreeClaimFeatures.Response,
                                              ByRef req As DSCM.GetVTreeClaimFeatures.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVTreeClaimFeatures
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVTreeClaims(ByRef res As DSCM.GetVTreeClaims.Response,
                                       ByRef req As DSCM.GetVTreeClaims.Request,
                                       Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVTreeClaims
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVTreePoliciesByBillingAccount(ByRef res As DSCM.GetVTreePoliciesByBillingAccount.Response,
                                                         ByRef req As DSCM.GetVTreePoliciesByBillingAccount.Request,
                                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.VisualTreeServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVTreePoliciesByBillingAccount
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace