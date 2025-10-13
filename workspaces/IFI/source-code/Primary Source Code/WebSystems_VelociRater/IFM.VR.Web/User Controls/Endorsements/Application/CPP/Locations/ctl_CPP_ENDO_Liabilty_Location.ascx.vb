Imports IFM.PrimativeExtensions

Public Class ctl_CPP_ENDO_Liabilty_Location
    Inherits VRControlBase

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
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divLocationItem.ClientID, Me.hdnAccord, "0")
        Me.VRScript.AddScriptLine("$( '#" + Me.divLocationItem.ClientID + "' ).accordion('option', 'active', 'false');") 'used to collapse all at start, since they do not contain data.
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        UpdateHeader()
        Me.PopulateChildControls()
    End Sub

    Private Sub UpdateHeader()
        Dim txt As String = "Location #" & MyLocationIndex + 1.ToString
        If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then txt += " - " & MyLocation.Address.HouseNum & " " & Me.MyLocation.Address.StreetName & " " & Me.MyLocation.Address.City
        Me.lblAccordHeader.Text = txt.Ellipsis(34)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        UpdateHeader()
        Me.SaveChildControls()

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Me.ValidateChildControls(valArgs)
    End Sub


End Class