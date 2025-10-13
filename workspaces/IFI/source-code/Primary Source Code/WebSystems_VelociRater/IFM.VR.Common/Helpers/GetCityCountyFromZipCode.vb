Namespace IFM.VR.Common.Helpers
    Public Class GetCityCountyFromZipCode

        Public Shared Function GetCityCountyFromZipCode(zipcode As String) As List(Of ZipLookupResult)
            Dim zipLookup As New IFM.VR.ZipCodeRef.ZipCode()
            Dim listOfZips As List(Of String) = zipLookup.GetCityCnty(zipcode)
            Return ConvertToListOfResults(listOfZips)
        End Function

        Private Shared Function ConvertToListOfResults(rawResults As List(Of String)) As List(Of ZipLookupResult)
            Dim results As New List(Of ZipLookupResult)
            If rawResults.Any() Then
                Try
                    '0 = zip
                    '1 = county
                    '2 = state
                    '3 > = cities
                    For i As Int32 = 0 To rawResults.Count() - 1 Step 4
                        results.Add(New ZipLookupResult(rawResults(i), rawResults(i + 1), rawResults(i + 2), rawResults(i + 3)))
                    Next
                Catch ex As Exception
#If DEBUG Then
                    Debugger.Break()
#End If
                End Try
            End If
            Return results
        End Function



    End Class


    Public Class ZipLookupResult
        Public Property ZipCode As String
        Public Property County As String
        Public Property StateAbbrev As String
        Public Property City As String
        Public Sub New(zip As String, county As String, state As String, city As String)
            Me.ZipCode = zip
            Me.County = county
            Me.StateAbbrev = state
            Me.City = city
        End Sub

        ' for Serialization Only
        Public Sub New()

        End Sub

    End Class

End Namespace

