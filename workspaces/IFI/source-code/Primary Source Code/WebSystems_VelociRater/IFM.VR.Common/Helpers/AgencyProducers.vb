Imports System.Data.SqlClient

Namespace IFM.VR.Common.Helpers
    Public Class AgencyProducers

        Public Shared Function GetProducersByAgencyId(ByVal agencyId As Integer) As List(Of AgencyProducer)
            Dim list As New List(Of AgencyProducer)
            If agencyId > 0 Then
                Using sq As New SQLselectObject
                    Dim param As New SqlClient.SqlParameter("@agency_id", agencyId)
                    Dim dr As SqlDataReader
                    dr = sq.GetDataReader(System.Configuration.ConfigurationManager.AppSettings("connQQ"), "usp_AgencyProducer_LoadActiveNonTerminated", param)

                    If dr IsNot Nothing AndAlso dr.HasRows Then
                        While dr.Read
                            Dim item As New AgencyProducer()
                            item.Code = dr.Item("code").ToString()
                            item.Name = dr.Item("name").ToString()
                            item.ProducerID = dr.Item("agencyproducer_id").ToString()
                            list.Add(item)
                        End While
                    End If
                End Using
            End If
            Return list
        End Function

    End Class
    Public Class AgencyProducer
        Public Code As String
        Public Name As String
        Public ProducerID As String
    End Class
End Namespace