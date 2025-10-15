Namespace IFM.VR.Common.Helpers
    Public Module EFTHelper

        Public Function GetBankNameFromAbaLookUp(ByVal routingNumber As String) As String
            Dim returnedBankName As String = String.Empty
            Try
                Using sproc As New SPManager("ConnQQ", "usp_get_BankNameFromAba")
                    sproc.AddStringParameter("@routing_number", routingNumber) 'as ABA number
                    Dim Data = sproc.ExecuteSPQuery()
                    returnedBankName = Data.Rows(0).Item("bank_name").ToString
                End Using
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try
            Return returnedBankName
        End Function

    End Module

    '    Public Function GetBankNameFromAbaLookUp(ByVal routingNumber As String) As String
    '            'insert into OMP_AbaBackUpLookup (aba_number,bank_name)
    '            Dim returnedBankName As String = ""
    '            Try
    '                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connDiamond")) ' changed from conn for Bug #1818
    '                    conn.Open()
    '                    Using cmd As New System.Data.SqlClient.SqlCommand()
    '                        cmd.Parameters.AddWithValue("@aba_number", routingNumber)
    '                        cmd.Connection = conn
    '                        'cmd.CommandText = "select bank_name from OMP_AbaBackUpLookup where aba_number = @aba_number"
    '                        cmd.CommandText = "select bank_name from [Diamond].[dbo].[EFTValidation] with (nolock) where routing_number = @aba_number" 'for Bug #1818
    '                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
    '                            If reader.HasRows Then
    '                                reader.Read()
    '                                returnedBankName = reader.GetString(0)
    '                            End If
    '                        End Using
    '                    End Using
    '                End Using
    '            Catch ex As Exception
    '#If DEBUG Then
    '                Debugger.Break()
    '#End If
    '            End Try
    '            Return returnedBankName
    '        End Function

    '    End Module

End Namespace