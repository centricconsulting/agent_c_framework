Imports QuickQuote.CommonObjects

Public Class ctlDisplayDiamondRatingErrors
    Inherits VRControlBase

#Region "Empty but required methods"
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function
#End Region

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' do this at INIT so it will be available to check for validation errors at Load of controls
        If Not IsPostBack Then

            If Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkflowSection_qs) IsNot Nothing Then
                If Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkflowSection_qs) = IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString() Then
                    Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                    If ratedQuote IsNot Nothing AndAlso ratedQuote.Success = False Then
                        'show errors from last rate
                        If ratedQuote.ValidationItems IsNot Nothing AndAlso ratedQuote.ValidationItems.Any() Then
                            Me.ValidationHelper.GroupName = "Rate"
                            For Each v In ratedQuote.ValidationItems
                                If v.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError Then
                                    Me.ValidationHelper.AddError(v.Message)
                                End If
                                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                    If v.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationWarning Then
                                        Me.ValidationHelper.AddWarning(v.Message)
                                    End If
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        End If
    End Sub
End Class