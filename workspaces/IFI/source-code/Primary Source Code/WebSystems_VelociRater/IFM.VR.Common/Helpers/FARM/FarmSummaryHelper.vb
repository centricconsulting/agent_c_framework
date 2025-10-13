Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Web
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.AllLines
Imports IFM.VR.Common.Helpers.MultiState

Public Class FarmSummaryHelper
    'Public Shared Function CreateCoverageCodeDataTable() As DataTable
    '    Dim dtCoverageCode As New DataTable
    '    dtCoverageCode.Columns.Add("ConversionID", GetType(String))
    '    dtCoverageCode.Columns.Add("Caption", GetType(String))

    '    Return dtCoverageCode
    'End Function

    Public Shared Function GetCoverageCodeCaption(CoverageCode_Id As String) As String
        'Public Shared Function GetCoverageCodeCaption() As DataTable
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim qqhelper As New QuickQuoteHelperClass

        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT coveragecodeversion_id, caption FROM CoverageCodeVersion WHERE CoverageCode_Id = " & qqhelper.IntegerForString(CoverageCode_Id) & " ORDER BY CoverageCodeVersion_id DESC"
            'cmd.CommandText = "SELECT coveragecodeversion_id, caption, coveragecode_id FROM CoverageCodeVersion ORDER BY CoverageCodeVersion_id DESC"
            da.SelectCommand = cmd
            da.Fill(tbl)
            'If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return ""

            Return CleanseString.Clean(tbl(0)("caption").ToString())
            'Return tbl
        Catch ex As Exception
            'HandleError(ClassName, "GetCoverageCodeCaption", ex, lblMsg)
            Return ""
            'Return tbl
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            tbl.Dispose()
            da.Dispose()
        End Try
    End Function

    Public Shared Function GetCoverageCodeCaption(ByVal CoverageCode_Id As String, ByVal quote As QuickQuoteObject, Optional ByVal Location As QuickQuoteLocation = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim desc As String = ""
        Dim qqVersionId As String = ""
        Dim sql As String = ""

        Dim QQHelper = New QuickQuoteHelperClass
        Dim StateQuote = IFM.VR.Common.Helpers.MultiState.General.SubQuoteForLocation(quote.SubQuotes, Location)

        Try
            If StateQuote IsNot Nothing Then
                qqVersionId = StateQuote.VersionId
            End If
            If QQHelper.IsPositiveIntegerString(qqVersionId) = False Then
                qqVersionId = quote.VersionId
            End If

            ' Get the caption
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            sql = "SELECT COALESCE(CCV.caption, CC.dscr) AS CoverageDescription, * FROM CoverageCode AS cc WITH (NOLOCK) "
            sql += "LEFT JOIN CoverageCodeVersion AS ccv WITH (NOLOCK) ON ccv.coveragecode_id = cc.coveragecode_id "
            sql += "AND ccv.version_id = " & QQHelper.IntegerForString(qqVersionId) & " WHERE cc.coveragecode_id = " & QQHelper.IntegerForString(CoverageCode_Id)
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return ""

            desc = tbl(tbl.Rows.Count - 1)("caption").ToString()
            If desc.Contains("–") Then
                desc = desc.Replace("–", "-")
            End If
            If desc.Trim = "" Then
                ' added 6/19/2019 MGB sometimes the caption is not there so use the description if there is one
                If Not IsDBNull(tbl(tbl.Rows.Count - 1)("CoverageDescription")) Then
                    desc = tbl(tbl.Rows.Count - 1)("CoverageDescription").ToString()
                End If
            End If

            Return desc
        Catch ex As Exception
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            tbl.Dispose()
            da.Dispose()
        End Try
    End Function
End Class