
Imports System.Configuration
Imports System.Data.SqlClient

Namespace IFM.VR.Common.Helpers.WCP
    Public Class QueryHelper


        Public Function GetDiamondClassCodeAndDescription(classificationtype_id As Integer) As DataTable
            Using sproc As New SPManager("connDiamondReports", "usp_get_WcpClassNewData")
                sproc.AddIntegerParamater("@classificationtype_id", classificationtype_id)
                Return sproc.ExecuteSPQuery()
            End Using
        End Function

        Public Function GetDiamondClassificationTypeID(ClassCode As String, dscr As String) As String
            Using sproc As New SPManager("connDiamondReports", "usp_get_WcpClassNewData")
                sproc.AddStringParameter("@ClassCode", ClassCode)
                sproc.AddStringParameter("@dscr", dscr)
                Dim Data = sproc.ExecuteSPQuery()
                Return Data.Rows(0).Item("classificationtype_id").ToString
            End Using
        End Function

    End Class
End Namespace


