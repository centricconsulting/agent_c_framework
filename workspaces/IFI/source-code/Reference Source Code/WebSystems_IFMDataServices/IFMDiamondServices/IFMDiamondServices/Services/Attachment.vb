Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCSA = Diamond.Common.Services.Messages.AttachmentService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Attachment
    Friend Module Attachment
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="FileName"></param>
        ''' <param name="FileBytes"></param>
        ''' <param name="AttachmentLevel"></param>
        ''' <param name="DisplayLevel"></param>
        ''' <param name="Attachment">ByRef sets the value to the diamond attachmentID.</param>
        ''' <param name="Description"></param>
        ''' <param name="e"></param>
        ''' <returns>Boolean value of Success.</returns>
        ''' <remarks>Agency does not appear to have authority to add attachments.</remarks>
        Public Function AddAttachment(PolicyID As Integer,
                                       PolicyImageNum As Integer,
                                       FileName As String,
                                       FileBytes() As Byte,
                                       AttachmentLevel As DC.Enums.Attachments.AttachmentType,
                                       DisplayLevel As DC.Enums.Attachments.LevelDisplay,
                                       Attachment As DCO.Attachments.AttachListItem,
                                       Optional Description As String = "",
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Try
                Dim keys As New DC.Objects.Attachments.AttachKeys
                Dim req As New DC.Services.Messages.AttachmentService.AttachFile.Request
                Dim res As New DC.Services.Messages.AttachmentService.AttachFile.Response
                keys.PolicyId = PolicyID
                keys.PolicyImageNum = PolicyImageNum
                keys.AttachmentLevel = AttachmentLevel
                With req.RequestData
                    .Dscr = Description
                    .FileName = FileName
                    .AttachData = FileBytes
                    .Keys = keys
                End With

                Using p As New DC.Services.Proxies.AttachmentServices.AttachmentServiceProxy
                    res = p.AttachFile(req)
                End Using
                If res.ResponseData.Success Then
                    Attachment = res.ResponseData.Attachment
                    Return res.ResponseData.Success
                Else
                    Dim errMessages As String = String.Empty
                    For Each v As DC.Objects.ValidationItem In res.DiamondValidation.ValidationItems
                        errMessages += v.Message + "   |   "
                    Next
                    e = New Exception(errMessages)
                End If
            Catch ex As Exception
                e = ex
            End Try
            Return False
        End Function
        Public Function AttachFile(ByRef res As DCSA.AttachFile.Response,
                                   ByRef req As DCSA.AttachFile.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AttachFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAttachment(ByRef res As DCSA.DeleteAttachment.Response,
                                   ByRef req As DCSA.DeleteAttachment.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAttachment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteWriteLetterDocument(ByRef res As DCSA.DeleteWriteLetterDocument.Response,
                                   ByRef req As DCSA.DeleteWriteLetterDocument.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteWriteLetterDocument
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteWriteLetterTemplate(ByRef res As DCSA.DeleteWriteLetterTemplate.Response,
                                   ByRef req As DCSA.DeleteWriteLetterTemplate.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteWriteLetterTemplate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DoesPolicyHaveAttachments(ByRef res As DCSA.DoesPolicyHaveAttachments.Response,
                                   ByRef req As DCSA.DoesPolicyHaveAttachments.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DoesPolicyHaveAttachments
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetLetterData(ByRef res As DCSA.GetLetterData.Response,
                                   ByRef req As DCSA.GetLetterData.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetLetterData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllExistingTemplates(ByRef res As DCSA.LoadAllExistingTemplates.Response,
                                   ByRef req As DCSA.LoadAllExistingTemplates.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllExistingTemplates
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAttachment(ByRef res As DCSA.LoadAttachment.Response,
                                   ByRef req As DCSA.LoadAttachment.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAttachment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableTemplateFields(ByRef res As DCSA.LoadAvailableTemplateFields.Response,
                                   ByRef req As DCSA.LoadAvailableTemplateFields.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAvailableTemplateFields
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimAttachmentList(ByRef res As DCSA.LoadClaimAttachmentList.Response,
                                   ByRef req As DCSA.LoadClaimAttachmentList.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimAttachmentList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadExistingTemplateFields(ByRef res As DCSA.LoadExistingTemplateFields.Response,
                                   ByRef req As DCSA.LoadExistingTemplateFields.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadExistingTemplateFields
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadForList(ByRef res As DCSA.LoadForList.Response,
                                   ByRef req As DCSA.LoadForList.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadForList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInfo(ByRef res As DCSA.LoadInfo.Response,
                                   ByRef req As DCSA.LoadInfo.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLetterTemplates(ByRef res As DCSA.LoadLetterTemplate.Response,
                                   ByRef req As DCSA.LoadLetterTemplate.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLetterTemplates
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadWriteLetterDocuments(ByRef res As DCSA.LoadWriteLetterDocuments.Response,
                                   ByRef req As DCSA.LoadWriteLetterDocuments.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadWriteLetterDocuments
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDocumentData(ByRef res As DCSA.SaveDocumentData.Response,
                                   ByRef req As DCSA.SaveDocumentData.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDocumentData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLetterTemplate(ByRef res As DCSA.SaveLetterTemplate.Response,
                                   ByRef req As DCSA.SaveLetterTemplate.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLetterTemplate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateAttachment(ByRef res As DCSA.UpdateAttachment.Response,
                                   ByRef req As DCSA.UpdateAttachment.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AttachmentServices.AttachmentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateAttachment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
