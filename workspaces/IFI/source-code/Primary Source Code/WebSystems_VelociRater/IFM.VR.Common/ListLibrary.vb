Imports System.Collections.Specialized

Namespace IFM.VR.Common

    Public Module ListLibrary

        ''' <summary>
        ''' Sorts the items in a dropdown list
        ''' </summary>
        ''' <param name="ddList"></param>
        ''' <remarks></remarks>
        Public Sub SortDropDownList(ByRef ddList As System.Web.UI.WebControls.DropDownList)
            Dim i As Integer
            Dim count As Integer = ddList.Items.Count
            Dim a As Integer = 0
            While count > 1
                For i = 0 To count - 2
                    If String.Compare(ddList.Items(i).Text, ddList.Items(i + 1).Text) > 0 Then
                        If String.Compare(ddList.Items(i).Text, ddList.Items(i + 1).Text) > 0 Then
                            SwapDropDownListItems(ddList, i, i + 1)
                        End If
                    End If
                Next
                count = count - 1
            End While

        End Sub

        Private Sub SwapDropDownListItems(ByRef ddList As System.Web.UI.WebControls.DropDownList, ByVal index1 As Integer, ByVal index2 As Integer)
            Dim TempLI As New System.Web.UI.WebControls.ListItem
            TempLI = ddList.Items(index2)
            ddList.Items.RemoveAt(index2)
            ddList.Items.Insert(index1, TempLI)
        End Sub

    End Module

End Namespace