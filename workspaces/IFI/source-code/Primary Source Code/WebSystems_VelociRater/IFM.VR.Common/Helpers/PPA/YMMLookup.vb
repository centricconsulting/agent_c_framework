Imports QuickQuote.CommonMethods
Namespace IFM.VR.Common.Helpers.PPA
    Public Class YMMLookup

        Public Shared Function GetMakeListFromYear(year As Int32) As List(Of YMMLookupResult)
            Dim results As New List(Of YMMLookupResult)

            Try
                QuickQuoteHelperClass.CheckDiamondServicesToken()

                Dim makeRequest As New Diamond.Common.Services.Messages.VinService.LoadMakeModelLookUpCombo.Request
                Dim makeResponse As New Diamond.Common.Services.Messages.VinService.LoadMakeModelLookUpCombo.Response

                With makeRequest
                    With .RequestData
                        .ComboType = Diamond.Common.Enums.Vin.MakeModelLookupComboType.Make ' Value: 2
                        .Year = year
                        .VersionId = 245                      
                        .VehicleInfoLookupTypeId = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi 'Value: 15
                    End With
                End With
                
                Using proxy As New Diamond.Common.Services.Proxies.VinServiceProxy
                    makeResponse = proxy.LoadMakeModelLookUpCombo(makeRequest)
                End Using

                If makeResponse IsNot Nothing Then
                    With makeResponse
                        If .ResponseData IsNot Nothing Then
                            With .ResponseData
                                If .Items IsNot Nothing AndAlso .Items.Count > 0 Then
                                    For Each i In .Items
                                        Dim result As New YMMLookupResult()
                                        result.Year = year
                                        result.Make = i.Description
                                        result.MakeCode = i.Code
                                        results.Add(result)
                                    Next
                                End If
                            End With
                        End If
                    End With
                End If
            Catch ex As System.Exception

            End Try

            Return (From r In results Order By r.Make Select r).ToList()
        End Function

        Public Shared Function GetModelListFromYearMake(year As Int32, make As String) As List(Of YMMLookupResult)
            Dim results As New List(Of YMMLookupResult)

            Try
                QuickQuoteHelperClass.CheckDiamondServicesToken()

                Dim modelRequest As New Diamond.Common.Services.Messages.VinService.LoadMakeModelLookUpCombo.Request
                Dim modelResponse As New Diamond.Common.Services.Messages.VinService.LoadMakeModelLookUpCombo.Response

                Dim makeToUse As String = IsoMakeForMake(make.Trim())
                If String.IsNullOrWhiteSpace(makeToUse) Then
                    makeToUse = make.Trim()
                End If

                With modelRequest
                    With .RequestData
                        .ComboType = Diamond.Common.Enums.Vin.MakeModelLookupComboType.Model 'Value: 3
                        .Year = year
                        .VersionId = 245
                        .Make = makeToUse 'Make should send its "code" string to the model call.
                        .VehicleInfoLookupTypeId = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi 'Value: 15
                    End With
                End With

                Using proxy As New Diamond.Common.Services.Proxies.VinServiceProxy
                    modelResponse = proxy.LoadMakeModelLookUpCombo(modelRequest)
                End Using

                If modelResponse IsNot Nothing Then
                    With modelResponse
                        If .ResponseData IsNot Nothing Then
                            With .ResponseData
                                If .Items IsNot Nothing AndAlso .Items.Count > 0 Then
                                    For Each i In .Items
                                        Dim result As New YMMLookupResult()
                                        result.Year = year
                                        result.Make = make.Trim()
                                        result.Model = i.Description
                                        'result.ModelCode = i.Code
                                        results.Add(result)
                                    Next
                                End If
                            End With
                        End If
                    End With
                End If
            Catch ex As System.Exception

            End Try

            Return (From r In results Order By r.Model Select r).ToList()
        End Function

        Private Shared Function IsoMakeForMake(ByVal make As String) As String
            Dim isoMake As String = ""
            If String.IsNullOrWhiteSpace(make) = False Then
                Dim makeToUse As String = make
                Select Case UCase(make)
                    Case "CHEVY"
                        makeToUse = "Chevrolet"
                End Select

                Using sql As New SQLselectObject(System.Configuration.ConfigurationManager.AppSettings("connDiamond"))
                    sql.queryOrStoredProc = "SELECT I.isomake FROM ModelISOMake as I with (nolock) WHERE I.dscr like '" & makeToUse & "%' ORDER BY I.isomake_id"

                    Using dr As SqlClient.SqlDataReader = sql.GetDataReader
                        If dr IsNot Nothing AndAlso dr.HasRows = True Then
                            dr.Read()
                            isoMake = dr.Item("isomake").ToString.Trim
                        End If
                    End Using
                End Using
            End If
            Return isoMake
        End Function
    End Class

    Public Class YMMLookupResult
        Public Property Year As Int32
        Public Property Make As String = ""
        Public Property Model As String = ""
        Public Property MakeCode As String = ""
        Public Property ModelCode As String = ""
    End Class
End Namespace