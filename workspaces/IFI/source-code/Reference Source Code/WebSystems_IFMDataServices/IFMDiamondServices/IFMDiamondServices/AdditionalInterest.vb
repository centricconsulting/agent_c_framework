Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond.AdditionalInterest
Namespace Services
    Public Module AdditionalInterest
        Public Function CanDelete(AdditionalInterestListId As Integer,
                                  Optional ByRef e As System.Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.AdditionalInterestService.CanDelete.Response
            Dim req As New DCSM.AdditionalInterestService.CanDelete.Request

            With req.RequestData
                .AdditionalInterestListId = AdditionalInterestListId
            End With


            If IFMS.AdditionalInterest.CanDelete(res, req, e, dv) Then
                Return res.ResponseData.CanDelete
            End If
            Return Nothing
        End Function

        Public Function CanEdit(AdditionalInterestListId As Integer,
                                PolicyId As Integer,
                                Optional ByRef e As System.Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.AdditionalInterestService.CanEdit.Response
            Dim req As New DCSM.AdditionalInterestService.CanEdit.Request

            With req.RequestData
                .AdditionalInterestListId = AdditionalInterestListId
                .PolicyId = PolicyId
            End With

            If IFMS.AdditionalInterest.CanEdit(res, req, e, dv) Then
                Return res.ResponseData.CanEdit
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DiamondValidation"></param>
        ''' <param name="PolicyId"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="e"></param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks>Whats going on with the diamondvalidation here?</remarks>
        Public Function CopyAndUpdateAIForABT(DiamondValidation As DCO.DiamondValidation,
                                              PolicyId As Integer,
                                              PolicyImageNum As Integer,
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.DiamondValidation
            'TODO:Figure out what is happening with the diamond validation for this service call.
            Dim res As New DCSM.AdditionalInterestService.CopyAndUpdateAIForABT.Response
            Dim req As New DCSM.AdditionalInterestService.CopyAndUpdateAIForABT.Request

            With req.RequestData
                .DiamondValidation = DiamondValidation
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If IFMS.AdditionalInterest.CopyAndUpdateAIForABT(res, req, e, dv) Then
                Return res.ResponseData.DiamondValidation
            End If
            Return Nothing
        End Function

        Public Function DeleteGlobalListEntry(AdditionalInterestListId As Integer,
                                              Optional ByRef e As System.Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.AdditionalInterestService.DeleteGlobalListEntry.Response
            Dim req As New DCSM.AdditionalInterestService.DeleteGlobalListEntry.Request

            With req.RequestData
                .AdditionalInterestListId = AdditionalInterestListId
            End With

            If IFMS.AdditionalInterest.DeleteGlobalListEntry(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function LoadAIHistory(AgencyId As Integer,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.AdditionalInterestListHistory)
            Dim res As New DCSM.AdditionalInterestService.LoadAIHistory.Response
            Dim req As New DCSM.AdditionalInterestService.LoadAIHistory.Request

            With req.RequestData
                .AgencyId = AgencyId
            End With

            If IFMS.AdditionalInterest.LoadAIHistory(res, req, e, dv) Then
                Return res.ResponseData.AdditionalInterestListHistories
            End If
            Return Nothing
        End Function

        Public Function LoadForId(AdditionalInterestListId As Integer,
                                  Optional ByRef e As System.Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.AdditionalInterestList
            Dim res As New DCSM.AdditionalInterestService.LoadForId.Response
            Dim req As New DCSM.AdditionalInterestService.LoadForId.Request

            With req.RequestData
                .AdditionalInterestListId = AdditionalInterestListId
            End With

            If IFMS.AdditionalInterest.LoadForId(res, req, e, dv) Then
                Return res.ResponseData.AdditionalInterestList
            End If
            Return Nothing
        End Function

        Public Function LoadSingleAIHistory(AdditionalInterestListId As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.AdditionalInterestListHistory)
            Dim res As New DCSM.AdditionalInterestService.LoadSingleAIHistory.Response
            Dim req As New DCSM.AdditionalInterestService.LoadSingleAIHistory.Request

            With req.RequestData
                .AdditionalInterestListId = AdditionalInterestListId
            End With

            If IFMS.AdditionalInterest.LoadSingleAIHistory(res, req, e, dv) Then
                Return res.ResponseData.AdditionalInterestListHistories
            End If
            Return Nothing
        End Function

        Public Function LookupLoad(AdditionalInterestGroupTypeId As Integer,
                                   BusinessPhone As String,
                                   City As String,
                                   Name As String,
                                   PolicyId As Integer,
                                   PolicyImageNum As Integer,
                                   StateId As Integer,
                                   Zip As String,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.AdditionalInterestList)
            Dim res As New DCSM.AdditionalInterestService.LookupLoad.Response
            Dim req As New DCSM.AdditionalInterestService.LookupLoad.Request

            With req.RequestData
                .AdditionalInterestGroupTypeId = AdditionalInterestGroupTypeId
                .BusinessPhone = BusinessPhone
                .City = City
                .Name = Name
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .StateId = StateId
                .Zip = Zip
            End With

            If IFMS.AdditionalInterest.LookupLoad(res, req, e, dv) Then
                Return res.ResponseData.AdditionalInterestLists
            End If
            Return Nothing
        End Function

        Public Function SaveAIList(AdditionalInterestList As DCO.Policy.AdditionalInterestList,
                                   AgencyId As Integer,
                                   PolicyId As Integer,
                                   PolicyImageNum As Integer,
                                   SaveList As Boolean,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.AdditionalInterestList
            Dim res As New DCSM.AdditionalInterestService.SaveAIList.Response
            Dim req As New DCSM.AdditionalInterestService.SaveAIList.Request

            With req.RequestData
                .AdditionalInterestList = AdditionalInterestList
                .AgencyId = AgencyId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .SaveAIList = SaveList
            End With

            If IFMS.AdditionalInterest.SaveAIList(res, req, e, dv) Then
                Return res.ResponseData.AdditionalInterestList
            End If
            Return Nothing
        End Function

        Public Function SaveAIListHistory(AdditionalInterestListHistory As DCO.Policy.AdditionalInterestListHistory,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.AdditionalInterestService.SaveAIListHistory.Response
            Dim req As New DCSM.AdditionalInterestService.SaveAIListHistory.Request

            With req.RequestData
                .AdditionalInterestListHistory = AdditionalInterestListHistory
            End With

            If IFMS.AdditionalInterest.SaveAIListHistory(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function ValidSearchData(AdditionalInterestGroupTypeId As Integer,
                                        AgencyId As Integer,
                                        BusinessPhone As String,
                                        City As String,
                                        Name As String,
                                        PolicyId As String,
                                        PolicyImageNum As Integer,
                                        StateId As Integer,
                                        Zip As String,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.DiamondValidation
            Dim res As New DCSM.AdditionalInterestService.ValidSearchData.Response
            Dim req As New DCSM.AdditionalInterestService.ValidSearchData.Request

            With req.RequestData
                .AdditionalInterestGroupTypeId = AdditionalInterestGroupTypeId
                .AgencyId = AgencyId
                .BusinessPhone = BusinessPhone
                .City = City
                .Name = Name
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .StateId = StateId
                .Zip = Zip
            End With

            If IFMS.AdditionalInterest.ValidSearchData(res, req, e, dv) Then
                Return res.ResponseData.DiamondValidation
            End If
            Return Nothing
        End Function
    End Module
End Namespace