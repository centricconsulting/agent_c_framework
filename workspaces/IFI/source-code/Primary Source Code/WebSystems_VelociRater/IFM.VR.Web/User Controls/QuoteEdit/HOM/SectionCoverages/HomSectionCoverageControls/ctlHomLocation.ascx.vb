Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods

Public Class ctlHomLocation
    Inherits ctlSectionCoverageControlBase

    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateJSBinding(lnkAddAdress, ctlPageStartupScript.JsEventType.onclick, "$('#{0}').show(); $('#{1}').text('View/Edit Address'); return false;".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID))
        If Me.ctlHomSectionCoverageAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False And Me.ctlHomSectionCoverageAddress.HasSomeAddressinformation = False Then
            Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
        Else

            Me.VRScript.AddScriptLine("$('#{1}').text('View/Edit Address');".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID)) ' run at startup
            If Me.ctlHomSectionCoverageAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False Then
                Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
            End If
        End If
        Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")


    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()

        If Me.IsQuoteReadOnly Then
            lnkDelete.Visible = False
        End If

        Me.ctlHomSectionCoverageAddress.CoverageIndex = Me.CoverageIndex
        Me.ctlHomSectionCoverageAddress.InitFromExisting(Me)

        Me.PopulateChildControls()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ctlHomSectionCoverageAddress.MyAddAddressLinkClientId = Me.lnkAddAdress.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = Me.CoverageName
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        Me.DeleteMySectionCoverage()
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.ctlHomSectionCoverageAddress.ClearControl()
    End Sub
End Class