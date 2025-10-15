Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports QuickQuote.CommonMethods

Public MustInherit Class VRGenericHandlerBase
    Implements System.Web.IHttpHandler, IRequiresSessionState

    Protected ReadOnly Property IsStaff As Boolean
        Get
            Dim qqHelper As New QuickQuoteHelperClass()

            Return qqHelper.IsHomeOfficeStaffUser()
        End Get
    End Property

    Protected Function UserHasAgencyIdAccess(agencyID As String) As Boolean

        Dim qqHelper As New QuickQuoteHelperClass()
        Return qqHelper.IsAgencyIdOkayForUser(agencyID)

    End Function

    MustOverride Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Protected Function GetjSon(objectToSerialize As Object) As String
        Dim jsSerializer As New JavaScriptSerializer()
        jsSerializer.MaxJsonLength = 2147483647
        Return jsSerializer.Serialize(objectToSerialize)
    End Function

End Class