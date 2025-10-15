Namespace IFM.VR.Common.Helpers.PPA
    Public Class MakeModelListLookup

        Public Shared Function GetMakes() As List(Of String)
            Dim results As New List(Of String)
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "usp_Get_VIN_MakesByYear"
                    cmd.CommandType = CommandType.StoredProcedure

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then

                            While reader.Read()

                                results.Add(reader.GetString(0).ToUpper())

                            End While
                        End If
                    End Using
                End Using
            End Using
            Return results
        End Function

        Public Shared Function GetMakes(year As Int32) As List(Of String)
            Dim results As New List(Of String)
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "usp_Get_VIN_MakesByYear"
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@year", year)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                results.Add(reader.GetString(0).ToUpper())
                            End While
                        End If
                    End Using
                End Using
            End Using
            Return results
        End Function



        Public Shared Function GetModels(FullMake As String) As List(Of String)
            Dim results As New List(Of String)
            If String.IsNullOrWhiteSpace(FullMake) = False Then
                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "usp_Get_VIN_ModelsByYearMake"
                        cmd.CommandType = CommandType.StoredProcedure
                        'cmd.CommandText = "SELECT Distinct([Diamond].[dbo].[ModelISORAPA].[full_model]) FROM [Diamond].[dbo].[ModelISORAPA] with (nolock) left join [Diamond].[dbo].[ModelISOMake] with (nolock) on [Diamond].[dbo].[ModelISOMake].isomake_id = [Diamond].[dbo].[ModelISORAPA].isomake_id  where [Diamond].[dbo].[ModelISOMake].dscr like @term  order by [Diamond].[dbo].[ModelISORAPA].[full_model]"

                        cmd.Parameters.AddWithValue("@term", FullMake)

                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    results.Add(reader.GetString(0).ToUpper())
                                End While
                            End If
                        End Using
                    End Using
                End Using
            End If

            Return results
        End Function

        Public Shared Function GetModels(FullMake As String, year As Int32) As List(Of String)
            Dim results As New List(Of String)
            If String.IsNullOrWhiteSpace(FullMake) = False Then
                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "usp_Get_VIN_ModelsByYearMake"
                        cmd.CommandType = CommandType.StoredProcedure
                        'cmd.CommandText = "SELECT Distinct([Diamond].[dbo].[ModelISORAPA].[full_model]) FROM [Diamond].[dbo].[ModelISORAPA] with (nolock) left join [Diamond].[dbo].[ModelISOMake] with (nolock) on [Diamond].[dbo].[ModelISOMake].isomake_id = [Diamond].[dbo].[ModelISORAPA].isomake_id  where [Diamond].[dbo].[ModelISOMake].dscr like @term and [Diamond].[dbo].[ModelISORAPA].year = @year order by [Diamond].[dbo].[ModelISORAPA].[full_model]"

                        cmd.Parameters.AddWithValue("@term", FullMake)
                        cmd.Parameters.AddWithValue("@year", year)

                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    results.Add(reader.GetString(0).ToUpper())
                                End While
                            End If
                        End Using
                    End Using
                End Using
            End If
            Return results
        End Function

        Public Shared Function ModelIsKnownWrong(FullMake As String, FullModel As String) As Boolean

            If String.IsNullOrWhiteSpace(FullMake) = False AndAlso String.IsNullOrWhiteSpace(FullModel) = False Then
                If FullMake.ToUpper() = "Nissan".ToUpper() Then
                    FullMake = "Nissan/Datsun".ToUpper()
                End If

                If FullMake.ToLower().Trim() = "smart" Then ' 10-12-2016 Matt A - make 'Smart' always has a isomakeid of '0' in the rappa table so the join on this stored proc would never match this only applies to Smart for whatever reason
                    Return False
                End If

                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "usp_Get_VIN_ConfirmMakeModel"
                        cmd.CommandType = CommandType.StoredProcedure

                        '                        cmd.CommandText = "SELECT distinct([Diamond].[dbo].[ModelISOMake].dscr) FROM [Diamond].[dbo].[ModelISORAPA] with (nolock) left join [Diamond].[dbo].[ModelISOMake] with (nolock) on [Diamond].[dbo].[ModelISOMake].isomake_id = [Diamond].[dbo].[ModelISORAPA].isomake_id where [Diamond].[dbo].[ModelISORAPA].[full_model] = @term or [Diamond].[dbo].[ModelISORAPA].[full_model] = @term  order by [Diamond].[dbo].[ModelISOMake].dscr"

                        cmd.Parameters.AddWithValue("@make", FullMake)
                        cmd.Parameters.AddWithValue("@term", FullModel)

                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                Return False
                            Else
                                Return True
                            End If
                        End Using
                    End Using
                End Using
            End If

            Return False
        End Function

    End Class
End Namespace