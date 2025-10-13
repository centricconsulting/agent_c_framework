Imports PopupMessageClass

Public Class testpopup
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Using popup As New PopupMessageObject(Me.Page, "Hello World!", "Classification Name")
            With popup
                .addCSS = True
                .isModal = True
                '.isModal = False
                .Image = PopupMessageObject.ImageOptions.None
                .hideCloseButton = True
                .AddButton("OK", True)
                .CreateDynamicPopUpWindow()
            End With
        End Using

    End Sub

End Class