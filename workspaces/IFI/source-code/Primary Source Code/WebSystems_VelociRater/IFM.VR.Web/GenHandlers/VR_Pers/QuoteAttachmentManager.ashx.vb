Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class QuoteAttachmentManager
    Inherits VRGenericHandlerBase



    Overrides Sub ProcessRequest(ByVal context As HttpContext)

        context.Response.ContentType = "text/plain"

        If context.Request.QueryString("action") IsNot Nothing Then
            Select Case context.Request.QueryString("action")
                Case "upload"
                    Dim files = context.Request.Files
                    Dim uploaded As Boolean = False
                    If files.Count > 0 Then
                        If context.Request.QueryString("agencyId") IsNot Nothing AndAlso context.Request.QueryString("quoteId") IsNot Nothing Then
                            Dim description As String = If(context.Request.QueryString("description") IsNot Nothing, context.Request.QueryString("description"), "")
                            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) Then ' this protects the searches from agency to agency
                                Dim fileDataAsByteArray(files(0).InputStream.Length) As Byte
                                files(0).InputStream.Read(fileDataAsByteArray, 0, files(0).InputStream.Length)
                                'If IFM.VR.Common.Helpers.FileUploadHelper.InsertFileIntoQQDatabase(CInt(context.Request.QueryString("agencyId")), CInt(context.Request.QueryString("quoteId")), description, files(0).FileName, fileDataAsByteArray) Then
                                '    uploaded = True
                                'End If
                                'updated 6/26/2019 for Endorsements
                                Dim quoteid As String = context.Request.QueryString("quoteId")
                                If String.IsNullOrWhiteSpace(quoteid) = False AndAlso quoteid.Contains("|") = True Then
                                    Dim polId As Integer = 0
                                    Dim polImgNum As Integer = 0
                                    Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(quoteid, delimiter:="|", positiveOnly:=True)
                                    If intList IsNot Nothing AndAlso intList.Count > 0 Then
                                        polId = intList(0)
                                        If intList.Count > 1 Then
                                            polImgNum = intList(1)
                                        End If
                                    End If
                                    uploaded = IFM.VR.Common.Helpers.FileUploadHelper.InsertPolicyImageFileIntoQQDatabase(CInt(context.Request.QueryString("agencyId")), polId, polImgNum, description, files(0).FileName, fileDataAsByteArray)
                                Else
                                    uploaded = IFM.VR.Common.Helpers.FileUploadHelper.InsertFileIntoQQDatabase(CInt(context.Request.QueryString("agencyId")), CInt(quoteid), description, files(0).FileName, fileDataAsByteArray)
                                End If
                            End If
                        End If
                    End If
                    If uploaded Then
                        context.Response.StatusCode = 200 'completed
                    Else
                        context.Response.StatusCode = 500 'failed
                    End If

                Case "query"
                    Threading.Thread.Sleep(500)
                    context.Response.ContentType = "application/json"
                    context.Response.Write(GetjSon(IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(context.Request.QueryString("agencyId"), context.Request.QueryString("quoteId"))))

                Case "retrieve"
                    Dim fileName As String = ""
                    Dim data() As Byte = Nothing
                    Dim agencyid As String = If(context.Request.QueryString("agencyId") IsNot Nothing, context.Request.QueryString("agencyId"), "")
                    If agencyid.IsNumeric Then
                        If UserHasAgencyIdAccess(agencyid) Then ' this protects the searches from agency to agency
                            Dim quoteid As String = If(context.Request.QueryString("quoteid") IsNot Nothing, context.Request.QueryString("quoteid"), "")
                            Dim dataId As String = If(context.Request.QueryString("DataId") IsNot Nothing, context.Request.QueryString("DataId"), "")
                            'If quoteid.IsNumeric AndAlso dataId.IsNumeric Then
                            '    IFM.VR.Common.Helpers.FileUploadHelper.GetFileBytes(agencyid, quoteid, dataId, fileName, data)
                            'End If
                            'updated 6/26/2019 for Endorsements
                            If dataId.IsNumeric Then
                                If String.IsNullOrWhiteSpace(quoteid) = False AndAlso quoteid.Contains("|") = True Then
                                    Dim polId As Integer = 0
                                    Dim polImgNum As Integer = 0
                                    Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(quoteid, delimiter:="|", positiveOnly:=True)
                                    If intList IsNot Nothing AndAlso intList.Count > 0 Then
                                        polId = intList(0)
                                        If intList.Count > 1 Then
                                            polImgNum = intList(1)
                                        End If
                                    End If
                                    IFM.VR.Common.Helpers.FileUploadHelper.GetPolicyImageFileBytes(CInt(agencyid), polId, polImgNum, CInt(dataId), fileName, data)
                                Else
                                    If quoteid.IsNumeric Then
                                        IFM.VR.Common.Helpers.FileUploadHelper.GetFileBytes(CInt(agencyid), CInt(quoteid), CInt(dataId), fileName, data)
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If data IsNot Nothing AndAlso data.Length > 0 Then
                        context.Response.ContentType = "application/octet-stream"
                        context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName)
                        context.Response.AddHeader("Content-Length", data.Length.ToString())

                        context.Response.BinaryWrite(data)
                    Else
                        context.Response.ContentType = "text/plain"
                        context.Response.Write("No data available. File does not exist or you do not have access to the file specified.")
                    End If
                Case "remove"
                    Dim fileName As String = ""
                    Dim data() As Byte = Nothing
                    Dim agencyid As String = If(context.Request.QueryString("agencyId") IsNot Nothing, context.Request.QueryString("agencyId"), "")
                    If agencyid.IsNumeric Then
                        If UserHasAgencyIdAccess(agencyid) Then ' this protects the searches from agency to agency
                            Dim quoteid As String = If(context.Request.QueryString("quoteid") IsNot Nothing, context.Request.QueryString("quoteid"), "")
                            Dim dataId As String = If(context.Request.QueryString("DataId") IsNot Nothing, context.Request.QueryString("DataId"), "")
                            'If quoteid.IsNumeric AndAlso dataId.IsNumeric Then
                            '    IFM.VR.Common.Helpers.FileUploadHelper.RemoveFileForQuoteId(agencyid, quoteid, dataId)
                            'End If
                            'updated 6/26/2019 for Endorsements
                            If dataId.IsNumeric Then
                                If String.IsNullOrWhiteSpace(quoteid) = False AndAlso quoteid.Contains("|") = True Then
                                    Dim polId As Integer = 0
                                    Dim polImgNum As Integer = 0
                                    Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(quoteid, delimiter:="|", positiveOnly:=True)
                                    If intList IsNot Nothing AndAlso intList.Count > 0 Then
                                        polId = intList(0)
                                        If intList.Count > 1 Then
                                            polImgNum = intList(1)
                                        End If
                                    End If
                                    IFM.VR.Common.Helpers.FileUploadHelper.RemoveFileForPolicyIdAndImageNum(CInt(agencyid), polId, polImgNum, CInt(dataId))
                                Else
                                    If quoteid.IsNumeric Then
                                        IFM.VR.Common.Helpers.FileUploadHelper.RemoveFileForQuoteId(CInt(agencyid), CInt(quoteid), CInt(dataId))
                                    End If
                                End If
                            End If
                        End If
                    End If
            End Select

        End If

        context.Response.End()
    End Sub



End Class