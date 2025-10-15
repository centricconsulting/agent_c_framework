Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.WCP

    Public Class WCPClassCodeHelper
        Public Shared Function GetClassCodes(SearchTypeid As String, searchString As String) As List(Of WCPClassCodeLookupResult)
            Dim results As New List(Of WCPClassCodeLookupResult)
            Dim versionId As String = IFM.VR.Common.Helpers.QuickQuoteObjectHelper.GetCurrentVersionId("21")
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    conn.Open()
                    cmd.CommandText = "usp_ClassCode_Search_WCP"
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@searchtype_id", SearchTypeid)
                    cmd.Parameters.AddWithValue("@searchstring", searchString)
                    cmd.Parameters.AddWithValue("@VersionId", versionId)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim r As New WCPClassCodeLookupResult()
                                r.DIAClass_Id = reader.GetInt32(3)
                                r.ClassCode = reader.GetString(2)
                                r.Description = reader.GetString(1)
                                'Need to remove Farm class codes here because Diamond will be retaining these as class code options
                                'The class codes have been put in the "QuickQuote.dbo.WCPClassificationExclude" table and excluded
                                'via an update to the SP
                                results.Add(r)
                            End While
                        End If
                    End Using
                End Using
            End Using

            Return results
        End Function

    End Class

    Public Class WCPClassCodeLookupResult
        Public Property ClassCode As String
        Public Property Description As String

        Public Property DIAClass_Id As Int32

    End Class

End Namespace
