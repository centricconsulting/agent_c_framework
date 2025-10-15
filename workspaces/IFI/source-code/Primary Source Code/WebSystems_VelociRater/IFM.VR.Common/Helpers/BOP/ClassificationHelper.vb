
Imports System.Configuration
Imports System.Data.SqlClient

Namespace IFM.VR.Common.Helpers.BOP
    Public Class ClassificationHelper

        'I don't think the below function is used.  Called from JS and that bind is now commented out.
        Public Shared Function GetClassificationsForProgramTypeId(programType As String) As List(Of KeyValuePair(Of String, String))
            Dim results As New List(Of KeyValuePair(Of String, String))

            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    If (programType.ToLower().Trim() = "cto") Then
                        'cmd.CommandText = "SELECT classcode,classdesc,dia_classificationtype_id FROM BOPCLASS WHERE PROGRAMTYPE = 'CT' AND (CLASSDESC LIKE '%- Office%' or CLASSDESC LIKE '%-Office%')   ORDER BY CLASSDESC ASC"
                        cmd.Parameters.AddWithValue("@programtype", "CT")
                        cmd.Parameters.AddWithValue("@searchstring", "Office")
                    Else
                        If programType.ToLower().Trim() = "cts" Then
                            'cmd.CommandText = "SELECT classcode,classdesc,dia_classificationtype_id FROM BOPCLASS WHERE PROGRAMTYPE = 'CT' AND (CLASSDESC LIKE '%- Shop%' or CLASSDESC LIKE '%-Shop%')  ORDER BY CLASSDESC ASC"
                            cmd.Parameters.AddWithValue("@programtype", "CT")
                            cmd.Parameters.AddWithValue("@searchstring", "Shop")
                        Else
                            'cmd.CommandText = "select classcode,classdesc,dia_classificationtype_id from bopclass where programtype = @programtype order by classdesc asc"
                            cmd.Parameters.AddWithValue("@programtype", programType)
                            cmd.Parameters.AddWithValue("@searchstring", "")
                        End If
                    End If
                    cmd.Connection = conn
                    conn.Open()

                    'cmd.Parameters.AddWithValue("@programtype", programType)
                    cmd.CommandText = "usp_BOPCLASSNEW_ClassCode_Classification_Search"
                    cmd.CommandType = CommandType.StoredProcedure

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                If reader.IsDBNull(0) = False AndAlso reader.IsDBNull(1) = False AndAlso reader.IsDBNull(2) = False Then
                                    Dim text As String = reader.GetString(1)
                                    Dim v As String = reader.GetString(0) + "-" + reader.GetString(1) + "{[" + reader.GetInt32(2).ToString() + "}]"
                                    Dim valPair As New KeyValuePair(Of String, String)(text, v)
                                    results.Add(valPair)
                                End If

                            End While
                        End If

                    End Using
                End Using
            End Using



            Return results
        End Function

        Public Shared Function GetProgramNameByClassCode(ByVal BOPNewClassCode As String) As String

            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Return spQuery.GetProgramNameByClassCode(BOPNewClassCode)


            'Dim ProgramName As String = String.Empty

            'Using conn As New SqlConnection(ConfigurationManager.AppSettings("ConnQQ"))
            '    Using cmd As New SqlCommand()
            '        conn.Open()
            '        cmd.Connection = conn
            '        'cmd.CommandType = CommandType.Text
            '        cmd.Parameters.AddWithValue("@ClassCode", BOPNewClassCode.TrimStart("0"c)) ' Matt A 4-30-17 - Always use Parameters with inline sql statements
            '        'cmd.CommandText = "SELECT ProgramType FROM BOPClassNew WHERE ClassCode = @BOPNewClassCode"
            '        cmd.CommandText = "usp_BOPCLASSNEW_ClassCode_Classification_Search"
            '        cmd.CommandType = CommandType.StoredProcedure
            '        Dim test As String = cmd.ExecuteScalar()("ProgramType").ToString().ToUpper()
            '        Select Case cmd.ExecuteScalar()("ProgramType").ToString().ToUpper() ' Just let it fail
            '            Case "AP"
            '                ProgramName = "Apartments"
            '                Exit Select
            '            Case "CT"
            '                ProgramName = "Contractors"
            '                Exit Select
            '            Case "CTO"
            '                ProgramName = "Contractors - Office"
            '                Exit Select
            '            Case "CTS"
            '                ProgramName = "Contractors - Service"
            '                Exit Select
            '            Case "MO"
            '                ProgramName = "MO???"
            '                Exit Select
            '            Case "OF"
            '                ProgramName = "Office"
            '                Exit Select
            '            Case "RE"
            '                ProgramName = "Retail"
            '                Exit Select
            '            Case "RS"
            '                ProgramName = "Restaurant"
            '                Exit Select
            '            Case "SE"
            '                ProgramName = "Service"
            '                Exit Select
            '            Case "WH"
            '                ProgramName = "Warehouse"
            '                Exit Select
            '        End Select
            '        Return ProgramName
            '    End Using
            'End Using
        End Function



    End Class
End Namespace


