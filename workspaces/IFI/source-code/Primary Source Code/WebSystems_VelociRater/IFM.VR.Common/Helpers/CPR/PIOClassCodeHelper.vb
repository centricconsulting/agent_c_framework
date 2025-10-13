Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports System.Configuration.ConfigurationManager

Namespace IFM.VR.Common.Helpers.CPR

    Public Class PIOClassCodeHelper
        Public Shared Function GetClassCodes(SearchTypeid As String, searchString As String, StateId As String, EffectiveDate As String, CompanyId As String) As List(Of PIOClassCodeLookupResult)
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Dim spm As New SPManager("connDiamondReports", "usp_Get_SpecialClassCodeData")

            Select Case SearchTypeid
                Case "1"  ' Item Number
                    spm.AddStringParameter("@SearchItemNum", searchString.Trim)
                    Exit Select
                Case "2"  ' Description Starts with
                    spm.AddStringParameter("@SearchStartsWithDscr", searchString.Trim)
                    Exit Select
                Case Else  ' Description Contains
                    spm.AddStringParameter("@SearchContainsDscr", searchString.Trim)
                    Exit Select
            End Select

            spm.AddStringParameter("@StateId", StateId)
            spm.AddStringParameter("@CompanyId", CompanyId)
            spm.AddStringParameter("@EffectiveDate", EffectiveDate)

            Dim tbl As DataTable = spm.ExecuteSPQuery()
            Dim results As New List(Of PIOClassCodeLookupResult)
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                For Each dr As DataRow In tbl.Rows
                    Dim item As New PIOClassCodeLookupResult
                    item.DIAClass_Id = qqh.IntegerForString(dr(0).ToString)
                    item.Description = dr(1).ToString
                    item.ItemNum = dr(2).ToString
                    item.ClassCode = dr(3).ToString
                    results.Add(item)
                Next
            End If

            'Dim results As New List(Of PIOClassCodeLookupResult)
            'Dim SQL As String = Nothing
            'Using conn As New System.Data.SqlClient.SqlConnection(AppSettings("connDiamond"))
            '    Using cmd As New System.Data.SqlClient.SqlCommand()
            '        cmd.Connection = conn
            '        conn.Open()
            '        Select Case SearchTypeid
            '            Case "1"  ' Item Number
            '                cmd.Parameters.AddWithValue("@searchTerm", searchString)
            '                'SQL = "SELECT [specialclasscodetype_id], [dscr], [item_num], [class_code] FROM [Diamond].[dbo].[SpecialClassCodeType] where [class_code] = @searchTerm And [dscr] <> 'N/A' order by [dscr] asc"
            '                SQL = "SELECT [specialclasscodetype_id], [dscr],[item_num],[class_code] FROM [Diamond].[dbo].[SpecialClassCodeType] where [item_num] = @searchTerm and [dscr] <> 'N/A' order by [dscr] asc"
            '                Exit Select
            '            Case "2"  ' Description Starts with
            '                cmd.Parameters.AddWithValue("@searchTerm", searchString + "%")
            '                SQL = "SELECT [specialclasscodetype_id], [dscr],[item_num],[class_code] FROM [Diamond].[dbo].[SpecialClassCodeType] where [dscr] like @searchTerm and [dscr] <> 'N/A' order by [dscr] asc"
            '                Exit Select
            '            Case Else  ' Description Contains
            '                cmd.Parameters.AddWithValue("@searchTerm", "%" + searchString + "%")
            '                SQL = "SELECT [specialclasscodetype_id], [dscr],[item_num],[class_code] FROM [Diamond].[dbo].[SpecialClassCodeType] where [dscr] like @searchTerm and [dscr] <> 'N/A' order by [dscr] asc"
            '                Exit Select
            '        End Select

            '        cmd.CommandText = SQL

            '        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
            '            If reader.HasRows Then
            '                While reader.Read()
            '                    Dim item As New PIOClassCodeLookupResult
            '                    item.DIAClass_Id = reader.GetInt32(0)
            '                    item.Description = reader.GetString(1)
            '                    item.ItemNum = reader.GetString(2)
            '                    item.ClassCode = reader.GetString(3)
            '                    results.Add(item)
            '                End While
            '            End If
            '        End Using
            '    End Using
            'End Using

            Return results
        End Function
    End Class

    Public Class PIOClassCodeLookupResult
        Public Property DIAClass_Id As Int32
        Public Property Description As String
        Public Property ItemNum As String
        Public Property ClassCode As String

    End Class

End Namespace
