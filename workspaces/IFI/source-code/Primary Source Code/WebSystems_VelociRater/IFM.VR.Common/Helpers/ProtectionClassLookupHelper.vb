Imports System.Data.SqlClient
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers

    Public Class ProtectionClassLookupHelper

        Public Shared Function GetProtectionClassRawData(ByVal countyCityName As String, ByVal isCity As Boolean, ByVal stateId As Int32, Optional isCommLine As Boolean = True) As List(Of ProtectionClassLookupResult)

            Dim results As New List(Of ProtectionClassLookupResult)
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("conndiamond"))
                Using cmd As New SqlCommand()

                    conn.Open()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "assp_ProtectionClass_Search"
                    cmd.Parameters.AddWithValue("@SearchType", Convert.ToInt32(isCity).ToString())     ' 0 = County, 1 = Community '1-31-2013 changed to use IsCity
                    cmd.Parameters.AddWithValue("@SearchText", countyCityName)
                    cmd.Parameters.AddWithValue("@stateID", stateId)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim result As New ProtectionClassLookupResult()
                                result.Township = reader.GetString(1)
                                result.County = reader.GetString(2)
                                result.ProtectionClass = reader.GetString(3).Trim()
                                If isCommLine = False Then
                                    result.ProtectionClass = result.ProtectionClass.ToLower().Replace("8b", "11")
                                End If
                                result.FootNote = reader.GetString(5)
                                If result.ProtectionClass.Contains("/") Then
                                    Dim newpcText = ""
                                    For Each st In result.ProtectionClass.Split("/")
                                        If st.Length = 1 And st <> "**" Then
                                            If isCommLine Then
                                                st = "0" + st
                                            End If
                                        End If
                                        newpcText += st + "/"
                                        result.PC_ID += GetProtectionClassIdFromProtectionClassNumber(st.Trim(), isCommLine) + "|"
                                    Next
                                    result.ProtectionClass = newpcText.TrimEnd("/")
                                    result.PC_ID = result.PC_ID.TrimEnd("|")
                                Else
                                    If result.ProtectionClass.Length = 1 And result.ProtectionClass <> "**" Then
                                        If isCommLine Then
                                            result.ProtectionClass = "0" + result.ProtectionClass
                                        End If
                                    End If
                                    result.PC_ID = GetProtectionClassIdFromProtectionClassNumber(result.ProtectionClass.Trim(), isCommLine)
                                End If
                                results.Add(result)
                            End While
                        End If
                    End Using
                End Using
            End Using
            Return results
        End Function

        Public Shared Function GetProtectionClassIdFromProtectionClassNumber(ByVal num As String, Optional isCommLine As Boolean = True) As String

            If isCommLine Then
                Select Case num.ToLower()
                    Case "01"
                        Return "12"
                    Case "02"
                        Return "13"
                    Case "03"
                        Return "14"
                    Case "04"
                        Return "15"
                    Case "05"
                        Return "16"
                    Case "06"
                        Return "17"
                    Case "07"
                        Return "18"
                    Case "08"
                        Return "19"
                    Case "8b"
                        Return "20"
                    Case "09"
                        Return "21"
                    Case "10", "*", "**"
                        Return "22"
                    Case Else
                        Return ""
                End Select
            Else
                If num.ToLower() = "*" Or num.ToLower() = "**" Then
                    Return "10"
                End If
                Return num.ToLower()
            End If
            Return ""

        End Function

    End Class

    Public Class ProtectionClassLookupResult
        Public Property Township As String
        Public Property County As String
        Public Property ProtectionClass As String
        Public Property FootNote As String
        Public Property PC_ID As String
    End Class

End Namespace