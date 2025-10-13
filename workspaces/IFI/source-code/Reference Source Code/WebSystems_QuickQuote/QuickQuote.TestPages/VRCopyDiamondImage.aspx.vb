Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Partial Class VRCopyDiamondImage
    Inherits System.Web.UI.Page

    Dim qqXml As New QuickQuoteXML
    Dim qqHelper As New QuickQuoteHelperClass

    Private Sub btnCopyImage_Click(sender As Object, e As EventArgs) Handles btnCopyImage.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Dim msgToShow As String = ""
        If qqHelper.IsPositiveIntegerString(Me.txtPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.txtPolicyImageNum.Text) = True Then

            If String.IsNullOrWhiteSpace(msgToShow) = True Then
                Dim newQuoteId As String = ""
                Dim newQuoteErrorMsg As String = ""
                Dim newQuoteDescription As String = ""
                Dim newDiaAgencyCode As String = Me.txtAgencyCode.Text
                Dim policyIdToCopy As Integer = qqHelper.IntegerForString(Me.txtPolicyId.Text)
                Dim policyImageNumToCopy As Integer = qqHelper.IntegerForString(Me.txtPolicyImageNum.Text)
                qqXml.CreateNewQuickQuoteFromDiamondPolicyImage(policyIdToCopy, policyImageNumToCopy, newQuoteId, newDiaAgencyCode:=newDiaAgencyCode, errorMsg:=newQuoteErrorMsg, newQuoteDescription:=newQuoteDescription)

                If qqHelper.IsPositiveIntegerString(newQuoteId) = True Then
                    'success
                    msgToShow = "Your new quote was successfully created (quoteId " & newQuoteId & ")"
                    If String.IsNullOrWhiteSpace(newQuoteDescription) = False Then
                        msgToShow &= " with the following description: " & newQuoteDescription
                    End If
                Else
                    msgToShow = "problem creating new quote"
                    If String.IsNullOrWhiteSpace(newQuoteErrorMsg) = False Then
                        msgToShow &= "; " & newQuoteErrorMsg
                    End If
                End If
            End If
        Else
            msgToShow = "invalid format for policyId and/or policyImageNumber"
        End If
        ShowError(msgToShow)

    End Sub

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load
        Page.MaintainScrollPositionOnPostBack = True

        If Page.IsPostBack = False Then
            Dim agentsLinkHref As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
            If String.IsNullOrWhiteSpace(agentsLinkHref) = False Then
                Me.AgentsLink.HRef = agentsLinkHref
                Me.AgentsLinkSection.Visible = True
            Else
                Me.AgentsLinkSection.Visible = False
            End If

            Me.btnCopyImage.Attributes.Add("onclick", "btnSubmit_Click(this, 'Creating...');") 'for disable button and server-side logic

            SetFocus(Me.txtPolicyId)
        End If
    End Sub

    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, "<br />", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub
End Class
