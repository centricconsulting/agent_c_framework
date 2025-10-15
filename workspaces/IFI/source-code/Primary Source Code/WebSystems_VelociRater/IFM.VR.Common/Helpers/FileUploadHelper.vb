Imports System.Data.SqlClient
Imports System.Web
Imports IFM.PrimativeExtensions
Imports System.Configuration
Imports IFM.VR.Common.Helpers

Namespace IFM.VR.Common.Helpers
    Public Class FileUploadHelper

        Public Shared ReadOnly Property VrFileUploadAcceptableExtensionList As List(Of String)
            Get
                Return System.Configuration.ConfigurationManager.AppSettings("VrFileUploadAcceptableExtensionList").CSVtoList()
            End Get
        End Property


        ''' <summary>
        ''' Returns a list of files attached to the provided quoteid.
        ''' </summary>
        ''' <param name="agencyid"></param>
        ''' <param name="quoteid"></param>
        ''' <returns></returns>
        Public Shared Function SearchForQuoteFiles(agencyid As String, quoteid As String) As List(Of FileAttachmentSearchResult)
            Dim results As New List(Of FileAttachmentSearchResult)
            If QuickQuoteObjectHelper.UserHasAgencyIdAccess(agencyid) Then ' this protects the searches from agency to agency
                Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        'cmd.Parameters.AddWithValue("@quoteId", quoteid)
                        'cmd.CommandText = "select FileAttachments.id,FileAttachmentsData.inserteddate,FileAttachmentsData.memo,FileAttachmentsData.filename,FileAttachmentsData.filesize,FileAttachments.dataid,FileAttachments.importedToAcrosoft from FileAttachments left join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.quoteId = @quoteId;"
                        'updated 6/26/2019 for Endorsements
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
                            cmd.Parameters.AddWithValue("@policyId", polId)
                            cmd.Parameters.AddWithValue("@policyImageNum", polImgNum)
                            cmd.CommandText = "select FileAttachments.id,FileAttachmentsData.inserteddate,FileAttachmentsData.memo,FileAttachmentsData.filename,FileAttachmentsData.filesize,FileAttachments.dataid,FileAttachments.importedToAcrosoft from FileAttachments left join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.policyId = @policyId and FileAttachments.policyImageNum = @policyImageNum;"
                        Else
                            Dim intQuoteId As Integer = 0
                            Dim result = Int32.TryParse(quoteid, intQuoteId)
                            cmd.Parameters.AddWithValue("@quoteId", intQuoteId)
                            cmd.CommandText = "select FileAttachments.id,FileAttachmentsData.inserteddate,FileAttachmentsData.memo,FileAttachmentsData.filename,FileAttachmentsData.filesize,FileAttachments.dataid,FileAttachments.importedToAcrosoft from FileAttachments left join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.quoteId = @quoteId;"
                        End If
                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    results.Add(New FileAttachmentSearchResult With {.id = reader.GetInt32(0).ToString(),
                                                        .quoteid = quoteid,
                                                        .insertedDate = reader.GetDateTime(1).ToShortDateString(),
                                                        .memo = reader.GetString(2),
                                                        .fileName = reader.GetString(3),
                                                        .fileSize = reader.GetInt64(4).ToString(),
                                                        .dataId = reader.GetInt32(5).ToString(),
                                                        .iconUrl = "",
                                                        .url = "GenHandlers/Vr_Pers/QuoteAttachmentManager.ashx?action=retrieve&quoteid=" + quoteid + "&DataId=" + reader.GetInt32(5).ToString() + "&agencyId=" + agencyid,
                                                        .removeUrl = "GenHandlers/Vr_Pers/QuoteAttachmentManager.ashx?action=remove&quoteid=" + quoteid + "&DataId=" + reader.GetInt32(5).ToString() + "&agencyId=" + agencyid,
                                                        .importedToAcrosoft = reader.GetBoolean(6)
                                                        })
                                End While
                            End If
                        End Using
                    End Using
                End Using
            End If
            Return results
        End Function

        Public Shared Function InsertFileIntoQQDatabase(agencyid As Int32, quoteId As Int32, memo As String, filename As String, fileData As Byte()) As Boolean
            If QuickQuoteObjectHelper.UserHasAgencyIdAccess(agencyid) Then ' this protects the searches from agency to agency
                Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                Dim result = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(quoteId, "", agencyid, "", "", "", "", "", QuickQuoteObjectHelper.IsStaff, True, 0, False, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))).FirstOrDefault()
                If result IsNot Nothing Then ' this makes sure that the quote is accessible by this user
                    Using conn As New SqlConnection(ConfigurationManager.AppSettings("ConnQQ"))
                        conn.Open()
                        Using cmd As New SqlCommand()
                            cmd.Connection = conn
                            If filename.Contains("\") Then
                                filename = filename.Substring(filename.LastIndexOf("\") + 1, filename.Length - (filename.LastIndexOf("\") + 1))
                            End If
                            If filename.Contains("/") Then
                                filename = filename.Substring(filename.LastIndexOf("/") + 1, filename.Length - (filename.LastIndexOf("/") + 1))
                            End If

                            If fileData.Length > 0 Then

                                cmd.CommandText = "insert into FileAttachmentsData (insertedDate,memo,filename,fileSize,data) values (@insertedDate,@memo,@filename,@fileSize,@data);"
                                cmd.Parameters.AddWithValue("@insertedDate", DateTime.Now)
                                cmd.Parameters.AddWithValue("@memo", HttpUtility.UrlDecode(memo).ToMaxLength(250))
                                cmd.Parameters.AddWithValue("@filename", filename.ToMaxLength(250))
                                cmd.Parameters.AddWithValue("@fileSize", fileData.Length)
                                cmd.Parameters.AddWithValue("@data", fileData)

                                cmd.CommandText += "insert into FileAttachments (quoteId,dataId) values (@quoteId,SCOPE_IDENTITY());"
                                cmd.Parameters.AddWithValue("@quoteId", quoteId)
                                'cmd.Parameters.AddWithValue("@dataId", "")
                                cmd.ExecuteNonQuery()
                                Return True
                            End If
                        End Using
                    End Using
                End If
            End If
            Return False
        End Function
        '6/26/2019 - new method for Endorsements
        Public Shared Function InsertPolicyImageFileIntoQQDatabase(agencyid As Int32, policyId As Int32, policyImageNum As Int32, memo As String, filename As String, fileData As Byte()) As Boolean
            If QuickQuoteObjectHelper.UserHasAgencyIdAccess(agencyid) Then ' this protects the searches from agency to agency
                Dim policyLookupQuery As New QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo
                With policyLookupQuery
                    .PolicyLookupType = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.LookupType.ByImage
                    .PolicyId = policyId
                    .PolicyImageNum = policyImageNum
                    .AgencyId = agencyid
                    'note: could also validate status; may also need to account for isStaff (by just sending all agencies the user has access to)
                End With
                Dim policyLookupResult As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo = QuickQuote.CommonMethods.QuickQuoteHelperClass.PolicyResultForLookupInfo(policyLookupQuery)
                If policyLookupResult IsNot Nothing Then ' this makes sure that the quote is accessible by this user
                    Using conn As New SqlConnection(ConfigurationManager.AppSettings("ConnQQ"))
                        conn.Open()
                        Using cmd As New SqlCommand()
                            cmd.Connection = conn
                            If filename.Contains("\") Then
                                filename = filename.Substring(filename.LastIndexOf("\") + 1, filename.Length - (filename.LastIndexOf("\") + 1))
                            End If
                            If filename.Contains("/") Then
                                filename = filename.Substring(filename.LastIndexOf("/") + 1, filename.Length - (filename.LastIndexOf("/") + 1))
                            End If

                            If fileData.Length > 0 Then

                                cmd.CommandText = "insert into FileAttachmentsData (insertedDate,memo,filename,fileSize,data) values (@insertedDate,@memo,@filename,@fileSize,@data);"
                                cmd.Parameters.AddWithValue("@insertedDate", DateTime.Now)
                                cmd.Parameters.AddWithValue("@memo", HttpUtility.UrlDecode(memo).ToMaxLength(250))
                                cmd.Parameters.AddWithValue("@filename", filename.ToMaxLength(250))
                                cmd.Parameters.AddWithValue("@fileSize", fileData.Length)
                                cmd.Parameters.AddWithValue("@data", fileData)

                                cmd.CommandText += "insert into FileAttachments (quoteId,dataId,policyId,policyImageNum) values (@quoteId,SCOPE_IDENTITY(),@policyId,@policyImageNum);"
                                cmd.Parameters.AddWithValue("@quoteId", 0)
                                cmd.Parameters.AddWithValue("@policyId", policyId)
                                cmd.Parameters.AddWithValue("@policyImageNum", policyImageNum)
                                cmd.ExecuteNonQuery()
                                Return True
                            End If
                        End Using
                    End Using
                End If
            End If
            Return False
        End Function

        Public Shared Sub GetFileBytes(agencyid As Int32, quoteid As Int32, dataid As Int32, ByRef filename As String, ByRef data As Byte())
            Dim qqhelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.Parameters.AddWithValue("@quoteid", quoteid)
                    cmd.Parameters.AddWithValue("@DataId", dataid)
                    cmd.CommandText = "select FileAttachmentsData.filename,FileAttachmentsData.data from FileAttachments join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.quoteId = @quoteid and FileAttachments.dataid = @DataId;"
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Dim result = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(quoteid, "", agencyid, "", "", "", "", "", QuickQuoteObjectHelper.IsStaff, True, 0, False, qqhelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))).FirstOrDefault()
                            If result IsNot Nothing Then ' this makes sure that the quote is accessible by this user
                                reader.Read()
                                filename = reader.GetString(0)
                                Dim fileSize As Long = reader.GetBytes(1, 0, Nothing, 0, 0)
                                data = New Byte(fileSize) {}
                                reader.GetBytes(1, 0, data, 0, fileSize)
                            End If
                        End If
                    End Using
                End Using
            End Using
        End Sub
        '6/26/2019 - new method for Endorsements
        Public Shared Sub GetPolicyImageFileBytes(agencyid As Int32, policyId As Int32, policyImageNum As Int32, dataid As Int32, ByRef filename As String, ByRef data As Byte())
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.Parameters.AddWithValue("@DataId", dataid)
                    cmd.Parameters.AddWithValue("@policyId", policyId)
                    cmd.Parameters.AddWithValue("@policyImageNum", policyImageNum)
                    cmd.CommandText = "select FileAttachmentsData.filename,FileAttachmentsData.data from FileAttachments join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.policyId = @policyId and FileAttachments.policyImageNum = @policyImageNum and FileAttachments.dataid = @DataId;"
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Dim policyLookupQuery As New QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo
                            With policyLookupQuery
                                .PolicyLookupType = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.LookupType.ByImage
                                .PolicyId = policyId
                                .PolicyImageNum = policyImageNum
                                .AgencyId = agencyid
                                'note: could also validate status; may also need to account for isStaff (by just sending all agencies the user has access to)
                            End With
                            Dim policyLookupResult As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo = QuickQuote.CommonMethods.QuickQuoteHelperClass.PolicyResultForLookupInfo(policyLookupQuery)
                            If policyLookupResult IsNot Nothing Then ' this makes sure that the quote is accessible by this user
                                reader.Read()
                                filename = reader.GetString(0)
                                Dim fileSize As Long = reader.GetBytes(1, 0, Nothing, 0, 0)
                                data = New Byte(fileSize) {}
                                reader.GetBytes(1, 0, data, 0, fileSize)
                            End If
                        End If
                    End Using
                End Using
            End Using
        End Sub

        Public Shared Function RemoveFileForQuoteId(agencyId As Int32, quoteId As Int32, dataId As Int32) As Boolean
            Dim idToRemove As Int32 = 0
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.Parameters.AddWithValue("@quoteid", quoteId)
                    cmd.Parameters.AddWithValue("@DataId", dataId)
                    cmd.CommandText = "select FileAttachments.id from FileAttachments join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.quoteId = @quoteid and FileAttachments.dataid = @DataId;"
                    'cmd.CommandText = "select FileAttachments.id, FileAttachmentsData.filename,FileAttachmentsData.data from FileAttachments join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.quoteId = @quoteid and FileAttachments.dataid = @DataId;"
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Dim result = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(quoteId, "", agencyId, "", "", "", "", "", QuickQuoteObjectHelper.IsStaff, True, 0, False, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))).FirstOrDefault()
                            If result IsNot Nothing Then ' this makes sure that the quote is accessible by this user
                                reader.Read()
                                idToRemove = reader.GetInt32(0)
                            End If
                        End If
                    End Using
                End Using
                If idToRemove > 0 Then ' you do it this way in case they added the file twice and you only want to delete one of the two of them
                    ' call method to remove
                    Using delCmd As New SqlCommand()
                        delCmd.Connection = conn
                        delCmd.Parameters.AddWithValue("@quoteid", quoteId)
                        delCmd.Parameters.AddWithValue("@dataid", dataId)
                        delCmd.CommandText = "delete from FileAttachments where quoteid = @quoteid and dataid = @dataid;"
                        delCmd.ExecuteNonQuery()
                    End Using
                    Return True
                End If
            End Using


            Return False
        End Function
        '6/26/2019 - new method for Endorsements
        Public Shared Function RemoveFileForPolicyIdAndImageNum(agencyId As Int32, policyId As Int32, policyImageNum As Int32, dataId As Int32) As Boolean
            Dim idToRemove As Int32 = 0
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.Parameters.AddWithValue("@DataId", dataId)
                    cmd.Parameters.AddWithValue("@policyId", policyId)
                    cmd.Parameters.AddWithValue("@policyImageNum", policyImageNum)
                    cmd.CommandText = "select FileAttachments.id from FileAttachments join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.policyId = @policyId and FileAttachments.policyImageNum = @policyImageNum and FileAttachments.dataid = @DataId;"
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Dim policyLookupQuery As New QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo
                            With policyLookupQuery
                                .PolicyLookupType = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.LookupType.ByImage
                                .PolicyId = policyId
                                .PolicyImageNum = policyImageNum
                                .AgencyId = agencyId
                                'note: could also validate status; may also need to account for isStaff (by just sending all agencies the user has access to)
                            End With
                            Dim policyLookupResult As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo = QuickQuote.CommonMethods.QuickQuoteHelperClass.PolicyResultForLookupInfo(policyLookupQuery)
                            If policyLookupResult IsNot Nothing Then ' this makes sure that the quote is accessible by this user
                                reader.Read()
                                idToRemove = reader.GetInt32(0)
                            End If
                        End If
                    End Using
                End Using
                If idToRemove > 0 Then ' you do it this way in case they added the file twice and you only want to delete one of the two of them
                    ' call method to remove
                    Using delCmd As New SqlCommand()
                        delCmd.Connection = conn
                        delCmd.Parameters.AddWithValue("@dataid", dataId)
                        delCmd.Parameters.AddWithValue("@policyId", policyId)
                        delCmd.Parameters.AddWithValue("@policyImageNum", policyImageNum)
                        delCmd.CommandText = "delete from FileAttachments where policyId = @policyId and policyImageNum = @policyImageNum and dataid = @dataid;"
                        delCmd.ExecuteNonQuery()
                    End Using
                    Return True
                End If
            End Using


            Return False
        End Function
        'added 7/12/2019 so records are removed when the image is deleted; removed 7/17/2019 - will now happen automatically from QQ library when image is deleted
        'Public Shared Function RemoveAllFilesForPolicyIdAndImageNum(policyId As Int32, policyImageNum As Int32) As Boolean
        '    Dim idToRemove As Int32 = 0
        '    Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
        '        conn.Open()
        '        Using cmd As New System.Data.SqlClient.SqlCommand()
        '            cmd.Connection = conn
        '            cmd.Parameters.AddWithValue("@policyId", policyId)
        '            cmd.Parameters.AddWithValue("@policyImageNum", policyImageNum)
        '            cmd.CommandText = "select FileAttachments.id from FileAttachments join FileAttachmentsData on FileAttachments.dataid = FileAttachmentsData.id where FileAttachments.policyId = @policyId and FileAttachments.policyImageNum = @policyImageNum;"
        '            Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
        '                If reader.HasRows Then
        '                    'Dim policyLookupQuery As New QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo
        '                    'With policyLookupQuery
        '                    '    .PolicyLookupType = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.LookupType.ByImage
        '                    '    .PolicyId = policyId
        '                    '    .PolicyImageNum = policyImageNum
        '                    '    .AgencyIds = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondAgencyIds()
        '                    '    'note: could also validate status; may also need to account for isStaff (by just sending all agencies the user has access to)
        '                    'End With
        '                    'Dim policyLookupResult As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo = QuickQuote.CommonMethods.QuickQuoteHelperClass.PolicyResultForLookupInfo(policyLookupQuery)
        '                    'If policyLookupResult IsNot Nothing Then ' this makes sure that the quote is accessible by this user
        '                    'note: can't really validate if the image has already been deleted
        '                    reader.Read()
        '                    idToRemove = reader.GetInt32(0)
        '                    'End If
        '                End If
        '            End Using
        '        End Using
        '        If idToRemove > 0 Then ' you do it this way in case they added the file twice and you only want to delete one of the two of them
        '            ' call method to remove
        '            Using delCmd As New SqlCommand()
        '                delCmd.Connection = conn
        '                delCmd.Parameters.AddWithValue("@policyId", policyId)
        '                delCmd.Parameters.AddWithValue("@policyImageNum", policyImageNum)
        '                delCmd.CommandText = "delete from FileAttachments where policyId = @policyId and policyImageNum = @policyImageNum;"
        '                delCmd.ExecuteNonQuery()
        '            End Using
        '            Return True
        '        End If
        '    End Using


        '    Return False
        'End Function

        ''' <summary>
        ''' Used to copy the attachments from one quoteId to another quoteId.
        ''' </summary>
        ''' <param name="copyFromQuoteId"></param>
        ''' <param name="copyToQuoteId"></param>
        Public Shared Sub CopyAttachedFilesToQuoteID(copyFromQuoteId As Int32, copyToQuoteId As Int32)

            Using conn As New SqlConnection(ConfigurationManager.AppSettings("ConnQQ"))
                conn.Open()
                Using cmd As New SqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "usp_Copy_FileAttachements"
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@fromQuoteId", copyFromQuoteId)
                    cmd.Parameters.AddWithValue("@toQuoteId", copyToQuoteId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub




    End Class

    Public Class FileAttachmentSearchResult
        Public Property id As String
        Public Property quoteid As String
        Public Property insertedDate As String
        Public Property memo As String
        Public Property fileName As String
        Public Property fileSize As String
        Public Property dataId As String
        Public Property iconUrl As String
        Public Property url As String
        Public Property removeUrl As String
        Public Property importedToAcrosoft As Boolean
    End Class
End Namespace

