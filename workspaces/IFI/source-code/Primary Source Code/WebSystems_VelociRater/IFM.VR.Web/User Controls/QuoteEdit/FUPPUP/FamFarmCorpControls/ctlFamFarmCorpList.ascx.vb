Public Class ctlFamFarmCorpList
    Inherits VRControlBase

    Public Property FfcDescriptionList() As List(Of String)
        Get
            If ViewState("Ffc_DescriptionList") Is Nothing Then
                ViewState("Ffc_DescriptionList") = New List(Of String)
            End If
            Return ViewState("Ffc_DescriptionList")
        End Get
        Set(value As List(Of String))
            ViewState("Ffc_DescriptionList") = value
        End Set
    End Property
    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Try
            If FfcDescriptionList.Count = 0 Then
                FfcDescriptionList.Add(String.Empty)
            End If
            aiRepeater.DataSource = FfcDescriptionList

            If aiRepeater.DataSource IsNot Nothing Then
                aiRepeater.DataBind()
                FindChildVrControls()

                For Each child In ChildVrControls
                    If TypeOf child Is ctlFamFarmCorpItem Then
                        Dim c As ctlFamFarmCorpItem = child
                        c.Populate()
                    End If
                Next

            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AttachCoverageControlEvents()
    End Sub

    Private Sub aiRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles aiRepeater.ItemDataBound
        Dim FFCItemControl As ctlFamFarmCorpItem = e.Item.FindControl("ctlFamFarmCorpItem")
        FFCItemControl.RowNumber = CInt(e.Item.ItemIndex)
        FFCItemControl.Description = e.Item.DataItem
        FFCItemControl.Populate()
    End Sub

    Private Sub AddNewItem() Handles lnkAddAI.Click
        Save()
        If FfcDescriptionList.Count = 0 Then
            FfcDescriptionList = New List(Of String)
        End If
        FfcDescriptionList.Add(String.Empty)
        Populate()
    End Sub
    Private Sub RemoveItem(RowNumber As Integer)
        Save()
        If (FfcDescriptionList.Count - 1) >= RowNumber Then
            FfcDescriptionList.RemoveAt(RowNumber)
        End If
        Populate()
    End Sub

    Private Sub AddItem(desc As String)
        RemoveBlankItems()
        If String.IsNullOrWhiteSpace(desc) = False AndAlso FfcDescriptionList.Contains(desc.Trim) = False Then
            FfcDescriptionList.Add(desc.Trim)
        End If
    End Sub

    Private Sub RemoveBlankItems()
        FfcDescriptionList.RemoveAll(Function(x As String) String.IsNullOrEmpty(x))
    End Sub

    Private Sub ClearItems()
        FfcDescriptionList = New List(Of String)
    End Sub

    Protected Sub AttachCoverageControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.aiRepeater.Items
            Dim FFCItem As ctlFamFarmCorpItem = cntrl.FindControl("ctlFamFarmCorpItem")
            AddHandler FFCItem.RemoveFfcItem, AddressOf RemoveItem
            AddHandler FFCItem.AddFfcItem, AddressOf AddItem
            index += 1
        Next
    End Sub

    Public Overrides Function Save() As Boolean
        ClearItems()
        SaveChildControls()
        Return True
    End Function
End Class