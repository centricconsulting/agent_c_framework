Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class TownshipLookup

        Public Shared Function GetTownships(countyName As String, versionId As Int32) As List(Of String())
            'TODO - Matt - IL Expansion need to send in state and update stored proc to accept a state
            ' Prod deployment Reminder to create new column, update stored proc and run any data seeding application
            Dim results As New List(Of String())
            Try
                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        conn.Open()
                        cmd.CommandText = "[usp_Get_Townships_for_CountyName]"
                        cmd.CommandType = CommandType.StoredProcedure
                        'cmd.Parameters.AddWithValue("@CountyName", countyName.Trim().Replace(".", "")) 'Replace . for St. Joseph county
                        cmd.Parameters.AddWithValue("@CountyName", countyName.Trim()) 'CH 2/4/19 removed "." replacement. Unneeded.
                        If versionId > 0 Then ' versionid is new, too retain backward compatibility it is defaulted to 111 in the stored proc
                            cmd.Parameters.AddWithValue("@VersionId", versionId)
                        End If
                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    Try
                                        results.Add({reader.GetInt32(0).ToString(), reader.GetString(1)})
                                    Catch ex As Exception
#If DEBUG Then
                                        Debugger.Break()
#End If
                                    End Try
                                End While
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try

            Return results
        End Function

        Public Shared Function GetTownships(countyName As String, stateId As Int32, lobId As Int32, effDate As String) As List(Of String())
            'TODO - Matt - IL Expansion need to send in state and update stored proc to accept a state
            ' Prod deployment Reminder to create new column, update stored proc and run any data seeding application
            Dim results As New List(Of String())
            Try
                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        conn.Open()
                        cmd.CommandText = "[usp_Get_Townships_for_CountyName]"
                        cmd.CommandType = CommandType.StoredProcedure
                        'cmd.Parameters.AddWithValue("@CountyName", countyName.Trim().Replace(".", "")) 'Replace . for St. Joseph county
                        cmd.Parameters.AddWithValue("@CountyName", countyName.Trim()) 'CH 2/4/19 removed "." replacement. Unneeded.
                        cmd.Parameters.AddWithValue("@StateId", stateId)
                        cmd.Parameters.AddWithValue("@LobId", lobId)
                        cmd.Parameters.AddWithValue("@EffectiveDate", CDate(effDate.Trim()))
                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    Try
                                        results.Add({reader.GetInt32(0).ToString(), reader.GetString(1)})
                                    Catch ex As Exception
#If DEBUG Then
                                        Debugger.Break()
#End If
                                    End Try
                                End While
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try

            Return results
        End Function

    End Class
End Namespace