Imports IFM.PrimativeExtensions
Public Class ctl_BOP_App_AdditionalServices
    Inherits VRControlBase

    'Added 6/26/2019 for Bug 33828 MLW

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.LocationIndex
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'If SubQuoteFirst IsNot Nothing Then
        Dim additionalServices As String = ""
        If SubQuotes IsNot Nothing Then
            For Each subquote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                If subquote.HasBeauticiansProfessionalLiability Then
                    additionalServices = UCase(subquote.BeauticiansProfessionalLiabilityDescription)
                    chkManicures.Checked = additionalServices.Contains("MANICURES")
                    chkPedicures.Checked = additionalServices.Contains("PEDICURES")
                    chkWaxes.Checked = additionalServices.Contains("WAXES")
                    chkThreading.Checked = additionalServices.Contains("THREADING")
                    chkHairExt.Checked = additionalServices.Contains("HAIR EXTENSIONS")
                    chkCosmetology.Checked = additionalServices.Contains("COSMETOLOGY SERVICES")
                    Exit For
                End If
            Next
        End If

        'Dim additionalServices As String = UCase(SubQuoteFirst.BeauticiansProfessionalLiabilityDescription)
        '    chkManicures.Checked = additionalServices.Contains("MANICURES")
        '    chkPedicures.Checked = additionalServices.Contains("PEDICURES")
        '    chkWaxes.Checked = additionalServices.Contains("WAXES")
        '    chkThreading.Checked = additionalServices.Contains("THREADING")
        '    chkHairExt.Checked = additionalServices.Contains("HAIR EXTENSIONS")
        '    chkCosmetology.Checked = additionalServices.Contains("COSMETOLOGY SERVICES")
        ' End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divBeauticiansAdditionalServices.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        Dim additionalServices As String = ""
        If chkManicures.Checked Then
            additionalServices += "Manicures"
        End If
        If chkPedicures.Checked Then
            If additionalServices <> "" Then
                additionalServices += ", "
            End If
            additionalServices += "Pedicures"
        End If
        If chkWaxes.Checked Then
            If additionalServices <> "" Then
                additionalServices += ", "
            End If
            additionalServices += "Waxes"
        End If
        If chkThreading.Checked Then
            If additionalServices <> "" Then
                additionalServices += ", "
            End If
            additionalServices += "Threading"
        End If
        If chkHairExt.Checked Then
            If additionalServices <> "" Then
                additionalServices += ", "
            End If
            additionalServices += "Hair Extensions"
        End If
        If chkCosmetology.Checked Then
            If additionalServices <> "" Then
                additionalServices += ", "
            End If
            additionalServices += "Cosmetology Services"
        End If

        If SubQuotes IsNot Nothing Then
            For Each subquote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                If subquote.HasBeauticiansProfessionalLiability Then
                    subquote.BeauticiansProfessionalLiabilityDescription = additionalServices
                End If
            Next
        End If

        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub
End Class