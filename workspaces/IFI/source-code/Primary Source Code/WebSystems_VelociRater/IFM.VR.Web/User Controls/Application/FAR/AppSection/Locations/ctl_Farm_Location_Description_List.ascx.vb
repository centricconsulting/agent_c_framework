Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Public Class ctl_Farm_Location_Description_List
    Inherits VRControlBase

    Public Event SaveComplete()

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If MyLocation IsNot Nothing Then
            If MyLocationIndex = 0 Then
                If MyLocation.Acreages IsNot Nothing AndAlso MyLocation.Acreages.Count > 1 Then

                    Dim filteredAcreages As List(Of QuickQuoteAcreage) = MyLocation.Acreages.Where(Function(a) a.LocationAcreageTypeId <> "4").ToList()
                    If filteredAcreages.Count > 1 Then
                        Me.Repeater1.DataSource = filteredAcreages.GetRange(1, filteredAcreages.Count - 1) ' start at 1 not 0
                        Me.Repeater1.DataBind()
                        Me.FindChildVrControls()



                        Dim blanketAcreageindex As Integer = MyLocation.Acreages _
                                .Select(Function(a, i) New With {Key .Acreage = a, Key .Index = i}) _
                                .Where(Function(x) x.Acreage.LocationAcreageTypeId = "4") _
                                .Select(Function(x) x.Index) _
                                .FirstOrDefault()


                        Dim index As Int32 = 1 ' yes this should start at 1 because primary is 0 and this list just shows non primary
                        For Each c In Me.GatherChildrenOfType(Of ctl_Farm_Location_Description)
                            c.MyLocationIndex = Me.MyLocationIndex
                            If (blanketAcreageindex > 0 AndAlso index = blanketAcreageindex) Then
                                index = index + 1
                            End If
                            c.MyAcresIndex = index
                            index += 1
                            c.Populate()
                        Next
                    End If

                Else
                    Me.Repeater1.DataSource = Nothing
                    Me.Repeater1.DataBind()
                    Me.FindChildVrControls()
                End If
            Else
                Me.Visible = False
            End If

        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        RaiseEvent SaveComplete()
        Return False
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        If Me.MyLocation IsNot Nothing Then
            Me.MyLocation.Acreages.AddNew()
            Me.Save_FireSaveEvent(False)
            Me.Populate()
        End If
    End Sub
End Class