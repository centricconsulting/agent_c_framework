Public Class ctlVr3Stats
    Inherits VRControlBase

    Public Property SupportedLob_CSV As String
        Get
            Return IFM.Common.InputValidation.InputHelpers.ListToCSV((From f In IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs() Select Convert.ToString(f.Value)).ToList())
        End Get
        Set(value As String)

        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtstartDate.Text = DateTime.Now.AddDays(-90).ToShortDateString()
        Me.txtEndDate.Text = DateTime.Now.ToShortDateString()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateDatePicker(Me.txtstartDate.ClientID, True)
        Me.VRScript.CreateDatePicker(Me.txtEndDate.ClientID, True)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function
End Class