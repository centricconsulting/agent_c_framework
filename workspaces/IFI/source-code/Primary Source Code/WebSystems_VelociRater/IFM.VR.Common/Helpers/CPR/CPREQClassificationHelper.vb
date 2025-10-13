Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports System.Configuration.ConfigurationManager

Namespace IFM.VR.Common.Helpers.CPR

    Public Class CPREQClassificationHelper
        Public Shared Function GetClassCodes(SearchTypeid As String, searchString As String) As List(Of CPREQClassificationLookupResult)
            Dim results As New List(Of CPREQClassificationLookupResult)
            Using conn As New System.Data.SqlClient.SqlConnection(AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    conn.Open()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    'cmd.CommandText = "usp_CPR_Search_EarthquakeClassifications"
                    ' Note:  The stored procedure expects 1 for starts with & rateh group and 2 for contains
                    'cmd.Parameters.AddWithValue("@searchtype_id", SearchTypeid)
                    'cmd.Parameters.AddWithValue("@searchstring", searchString)

                    Dim searchtext As String = ""
                    Select Case SearchTypeid
                        Case "1"    ' Starts With
                            searchtext = searchString & "%"
                            Exit Select
                        Case "2"    ' Contains
                            searchtext = "%" & searchString & "%"
                            Exit Select
                    End Select

                    cmd.CommandText = "usp_CPR_Get_PP_EQ_Classifications"
                    cmd.Parameters.AddWithValue("@searchText", searchtext)
                    cmd.Parameters.AddWithValue("@gradeId", Nothing)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim cc As New CPREQClassificationLookupResult()

                                ' ID
                                cc.ID = If(reader.IsDBNull(0) = False, reader.GetInt32(0).ToString(), "")

                                ' Full & Edited Descriptions
                                ' Escape special characters in the description text
                                Dim descText As String = ""
                                If Not reader.IsDBNull(1) Then descText = reader.GetString(1)
                                cc.FullDescription = descText
                                descText = descText.Replace("""", "\" & """")
                                descText = descText.Replace("'", "\'")
                                cc.EditedDescription = descText

                                ' Rate Grade
                                If Not reader.IsDBNull(2) Then cc.RateGrade = reader.GetString(2)

                                If descText <> "" AndAlso descText <> "N/A" Then results.Add(cc)
                            End While
                        End If
                    End Using
                End Using
            End Using

            Return results
        End Function

    End Class

    Public Class CPREQClassificationLookupResult
        Public Property ID As String
        Public Property FullDescription As String
        Public Property EditedDescription As String
        Public Property RateGrade As String
    End Class


End Namespace
