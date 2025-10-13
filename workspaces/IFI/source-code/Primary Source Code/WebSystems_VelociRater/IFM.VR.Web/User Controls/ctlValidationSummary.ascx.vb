Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers

Public Class ctlValidationSummary
    Inherits System.Web.UI.UserControl

    Dim valList As New System.Collections.Generic.List(Of ControlValidationHelper)
    Private ReadOnly Property ValidationHelperList As System.Collections.Generic.List(Of ControlValidationHelper)
        Get
            Return valList
        End Get
    End Property

    Public ReadOnly Property ValidationItems As List(Of WebValidationItem)
        Get
            'cleared after each populate
            Dim l As New List(Of WebValidationItem)
            For Each v In valList
                For Each v1 In v.GetErrors()
                    l.Add(v1)
                Next
            Next
            Return l
        End Get
    End Property

    Public ReadOnly Property HasErrors As Boolean
        Get
            For Each v In valList
                If v.GetErrors().Any() Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    Public ReadOnly Property HasWarnings As Boolean
        Get
            For Each v In valList
                If v.GetWarnings().Any() Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property
    Public Property Title As String
        Get
            Return WebHelper_Personal.GetAttributeValueForGenericControl(Me.divValidationSummary, "title")
        End Get
        Set(value As String)
            WebHelper_Personal.AddAttributeToGenericControl(Me.divValidationSummary, "title", value)
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.divValidationSummary.Visible = False
    End Sub

    Public Sub Populate()
        Dim sb_Script As New StringBuilder()
        Dim sb_Display As New StringBuilder()
        Dim hasErrors As Boolean = False
        Dim hasWarnings As Boolean = False
        For Each Val As ControlValidationHelper In Me.valList
            If Val.GetErrors().Count() > 0 Or Val.GetWarnings().Count() > 0 Then
                sb_Display.Append("<div style=""margin-left: 30px;"">")

                sb_Display.Append("<span style=""font-weight: bold; font-size: 14px;"">" + Val.GroupName + "</span>")

                'section Div Start
                sb_Display.Append("<div style=""margin-left: 30px;"">")

                'errors
                If Val.GetErrors().Count() > 0 Then
                    sb_Display.Append("<span style=""font-weight: bold; font-size: 12px;"">Critical Warnings</span>")
                    sb_Display.Append("<ul style=""color: red;padding-top: 1px;"">")
                    sb_Script.Append("<script type=""text/javascript"">")
                    sb_Script.Append("$(document).ready(function(){")
                    Dim index As Int32 = 0
                    For Each valItem As WebValidationItem In Val.GetErrors()
                        If (valItem.ScriptText.Length > 0 And Val.ControlWasRendered) Then
                            sb_Display.Append("<li style=""cursor:pointer;text-decoration:underline;"" title=""Click to navigate to error."" onclick='" + valItem.ScriptText + "'>" + valItem.Message + "</li>")
                        Else
                            sb_Display.Append("<li style=""cursor:pointer;"">" + valItem.Message + "</li>")
                        End If

                        If String.IsNullOrWhiteSpace(valItem.SenderClientId) = False Then
                            If valItem.SenderClientId.Contains(" parent") Then
                                sb_Script.Append("$(""#" + valItem.SenderClientId.Split(" ")(0) + """).parent().addClass(""validation-error"");")
                            Else
                                'multi for elements that display multiple messages (ex. gridviews) - CAH 10/24/2017
                                If valItem.SenderClientId.Contains(" multi") Then
                                    sb_Script.Append("$(""#" + valItem.SenderClientId.Split(" ")(0) + """).attr(""style"",""border:1px solid red;"" + $(""#" + valItem.SenderClientId + """).attr(""style""));")
                                Else
                                    sb_Script.Append("$(""#" + valItem.SenderClientId + """).attr(""style"",""border:1px solid red;"" + $(""#" + valItem.SenderClientId + """).attr(""style""));")
                                End If

                            End If

                            'If Not valItem.SupressDisplay Then
                            If valItem.SenderClientId.Contains(" parent") Then
                                sb_Script.Append("$(""#" + valItem.SenderClientId.Split(" ")(0) + """).parent().attr(""title"",""" + valItem.Message + """);")
                                sb_Script.Append("$(""#" + valItem.SenderClientId.Split(" ")(0) + """).parent().append('<br/><span style=""color: red"">" + valItem.Message + "</span>');")
                            Else
                                'multi for elements that display multiple messages (ex. gridviews) - CAH 10/24/2017
                                If valItem.SenderClientId.Contains(" multi") Then
                                    sb_Script.Append("$(""#" + valItem.SenderClientId.Split(" ")(0) + """).parent().attr(""title"",""Contains Errors"");")
                                    sb_Script.Append("$(""#" + valItem.SenderClientId.Split(" ")(0) + """).parent().append('<br/><span style=""color: red"">" + valItem.Message + "</span>');")
                                Else
                                    sb_Script.Append("$(""#" + valItem.SenderClientId + """).attr(""title"",""" + valItem.Message + """);")
                                    sb_Script.Append("$(""#" + valItem.SenderClientId + """).after('<br/><span style=""color: red"">" + valItem.Message + "</span>');")
                                End If
                            End If
                            'End If

                            If index = 0 Then
                                sb_Script.Append("$(""#" + valItem.SenderClientId + """).focus();")
                            End If
                            'formInputError
                        End If

                        If valItem.IsTitleChange AndAlso String.IsNullOrWhiteSpace(valItem.TitleText) = False Then
                            divValidationSummary.Attributes.Item("title") = valItem.TitleText
                        End If

                        index += 1
                    Next
                    sb_Script.Append("});")
                    sb_Script.Append("</script>")
                    sb_Display.Append("</ul>")
                End If

                'warnings
                If Val.GetWarnings().Count > 0 Then
                    sb_Display.Append("<span style=""font-weight: bold; font-size: 12px;"">Additional Information</span>")
                    sb_Display.Append("<ul style=""padding-top: 1px; "">")
                    For Each valItem As WebValidationItem In Val.GetWarnings()
                        sb_Display.Append("<li>" + valItem.Message + "</li>")
                    Next
                    sb_Display.Append("</ul>")

                End If

                'section Div END
                sb_Display.Append("</div>")
                sb_Display.Append("</div>")

                If Val.HasErrros Then
                    hasErrors = True
                End If
                If Val.HasWarnings Then
                    hasWarnings = True
                End If
            End If
        Next
        Me.litScript.Text = sb_Script.ToString()
        Me.litValDisplay.Text = sb_Display.ToString()

        Me.valList.Clear() ' prevents same validation lists from showing next time
        Me.divValidationSummary.Visible = hasErrors Or hasWarnings
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Me.Populate()
    End Sub

    Public Sub RegisterValidationHelper(valItem As ControlValidationHelper)
        If Me.valList.Contains(valItem) = False Then ' keep this from adding the same object twice
            Me.valList.Add(valItem)
        End If

    End Sub

End Class