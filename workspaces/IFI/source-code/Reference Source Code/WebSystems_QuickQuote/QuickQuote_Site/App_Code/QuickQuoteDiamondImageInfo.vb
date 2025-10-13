Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    Public Class QuickQuoteDiamondImageInfo 'added 3/12/2019 for Endorsements and maybe other images

        Enum DiamondImageType
            None = 0
            NewBusinessQuote = 1
            EndorsementQuote = 2
        End Enum

        Enum ImageInfoType
            None = 0
            QuoteInfo = 1
            RatedQuoteInfo = 2
            RatedQuoteInfoWithoutImage = 3
            AppInfo = 4
            RatedAppInfo = 5
            RatedAppInfoWithoutImage = 6
        End Enum

        Private _DiamondImageInfoId As Integer = 0
        Public ReadOnly Property DiamondImageInfoId As Integer
            Get
                Return _DiamondImageInfoId
            End Get
        End Property

        Private _PolicyId As Integer = 0
        Public ReadOnly Property PolicyId As Integer
            Get
                Return _PolicyId
            End Get
        End Property
        Private _PolicyImageNum As Integer = 0
        Public ReadOnly Property PolicyImageNum As Integer
            Get
                Return _PolicyImageNum
            End Get
        End Property
        Private _PolicyNumber As String = ""
        Public ReadOnly Property PolicyNumber As String
            Get
                Return _PolicyNumber
            End Get
        End Property

        Private _ImageType As DiamondImageType = DiamondImageType.None
        Public ReadOnly Property ImageType As DiamondImageType
            Get
                Return _ImageType
            End Get
        End Property
        Private _InfoType As ImageInfoType = ImageInfoType.None
        Public ReadOnly Property InfoType As ImageInfoType
            Get
                Return _InfoType
            End Get
        End Property

        Private _ImageXml As Byte() = Nothing
        Public ReadOnly Property ImageXml As Byte()
            Get
                Return _ImageXml
            End Get
        End Property

        Private _InfoDateAdded As String = ""
        Public ReadOnly Property InfoDateAdded As String
            Get
                Return _InfoDateAdded
            End Get
        End Property
        Private _InfoDateModified As String = ""
        Public ReadOnly Property InfoDateModified As String
            Get
                Return _InfoDateModified
            End Get
        End Property
        Private _XmlDateAdded As String = ""
        Public ReadOnly Property XmlDateAdded As String
            Get
                Return _XmlDateAdded
            End Get
        End Property
        Private _XmlDateModified As String = ""
        Public ReadOnly Property XmlDateModified As String
            Get
                Return _XmlDateModified
            End Get
        End Property

        Private _DiamondImageXmlId As Integer = 0
        Public ReadOnly Property DiamondImageXmlId As Integer
            Get
                Return _DiamondImageXmlId
            End Get
        End Property

        Private _QuoteStatus As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType = Nothing
        Public ReadOnly Property QuoteStatus As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType
            Get
                Return _QuoteStatus
            End Get
        End Property

        Private _Active As Boolean = True
        Public ReadOnly Property Active As Boolean
            Get
                Return _Active
            End Get
        End Property

        Private _JustInsertOrUpdateDiamondImageInfoRecord As Boolean = False
        Public ReadOnly Property JustInsertOrUpdateDiamondImageInfoRecord As Boolean
            Get
                Return _JustInsertOrUpdateDiamondImageInfoRecord
            End Get
        End Property
        Private _SearchForExistingInfo As Boolean = True
        Public ReadOnly Property SearchForExistingInfo As Boolean
            Get
                Return _SearchForExistingInfo
            End Get
        End Property

        Private _DeactivateOlderInfoRecords As Boolean = False
        Public ReadOnly Property DeactivateOlderInfoRecords As Boolean
            Get
                Return _DeactivateOlderInfoRecords
            End Get
        End Property
        Private _VerifyInsertDateAgainstImageAddedDateOnSearchForExistingInfo As Boolean = True
        Public ReadOnly Property VerifyInsertDateAgainstImageAddedDateOnSearchForExistingInfo As Boolean
            Get
                Return _VerifyInsertDateAgainstImageAddedDateOnSearchForExistingInfo
            End Get
        End Property
        Private _UpdateImageInfoWithXmlIdParameter As Boolean = False
        Public ReadOnly Property UpdateImageInfoWithXmlIdParameter As Boolean
            Get
                Return _UpdateImageInfoWithXmlIdParameter
            End Get
        End Property

        Private _InfoInsertUserId As Integer = 0
        Public ReadOnly Property InfoInsertUserId As Integer
            Get
                Return _InfoInsertUserId
            End Get
        End Property
        Private _InfoUpdateUserId As Integer = 0
        Public ReadOnly Property InfoUpdateUserId As Integer
            Get
                Return _InfoUpdateUserId
            End Get
        End Property
        Private _XmlInsertUserId As Integer = 0
        Public ReadOnly Property XmlInsertUserId As Integer
            Get
                Return _XmlInsertUserId
            End Get
        End Property
        Private _XmlUpdateUserId As Integer = 0
        Public ReadOnly Property XmlUpdateUserId As Integer
            Get
                Return _XmlUpdateUserId
            End Get
        End Property

        Private _IsBillingUpdate As Boolean = False
        Public ReadOnly Property IsBillingUpdate As Boolean
            Get
                Return _IsBillingUpdate
            End Get
        End Property

        Private _endorsementOriginType As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes = QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes.Velocirater
        Public ReadOnly Property EndorsementOriginType As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes
            Get
                Return _endorsementOriginType
            End Get
        End Property

        Private _eSigInfo As New QuickQuoteEsignatureInformation

        Public Property eSigInfo As QuickQuoteEsignatureInformation
            Get
                Return _eSigInfo
            End Get
            Set(value As QuickQuoteEsignatureInformation)
                _eSigInfo = value
            End Set
        End Property

        'added 1/4/2021 (Interoperability)
        Private _LastRulesOverrideRecordModifiedDate As String = ""
        Public ReadOnly Property LastRulesOverrideRecordModifiedDate As String
            Get
                Return _LastRulesOverrideRecordModifiedDate
            End Get
        End Property

        Protected Friend Sub Set_DiamondImageInfoId(ByVal diaImageInfoId As Integer)
            _DiamondImageInfoId = diaImageInfoId
        End Sub

        Protected Friend Sub Set_PolicyId(ByVal polId As Integer)
            _PolicyId = polId
        End Sub
        Protected Friend Sub Set_PolicyImageNum(ByVal imgNum As Integer)
            _PolicyImageNum = imgNum
        End Sub
        Protected Friend Sub Set_PolicyNumber(ByVal polNum As String)
            _PolicyNumber = polNum
        End Sub

        Protected Friend Sub Set_ImageType(ByVal imgType As DiamondImageType)
            _ImageType = imgType
        End Sub
        Protected Friend Sub Set_InfoType(ByVal imgInfoType As ImageInfoType)
            _InfoType = imgInfoType
        End Sub

        Protected Friend Sub Set_ImageXml(ByVal imgXml As Byte())
            _ImageXml = imgXml
        End Sub

        Protected Friend Sub Set_InfoDateAdded(ByVal dtAdded As String)
            _InfoDateAdded = dtAdded
        End Sub
        Protected Friend Sub Set_InfoDateModified(ByVal dtModified As String)
            _InfoDateModified = dtModified
        End Sub
        Protected Friend Sub Set_XmlDateAdded(ByVal dtAdded As String)
            _XmlDateAdded = dtAdded
        End Sub
        Protected Friend Sub Set_XmlDateModified(ByVal dtModified As String)
            _XmlDateModified = dtModified
        End Sub

        Protected Friend Sub Set_DiamondImageXmlId(ByVal diaImageXmlId As Integer)
            _DiamondImageXmlId = diaImageXmlId
        End Sub

        Protected Friend Sub Set_QuoteStatus(ByVal qtStatus As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType)
            _QuoteStatus = qtStatus
        End Sub

        Protected Friend Sub Set_Active(ByVal isActive As Boolean)
            _Active = isActive
        End Sub

        Protected Friend Sub Set_JustInsertOrUpdateDiamondImageInfoRecord(ByVal justInsertOrUpdateInfoRecord As Boolean)
            _JustInsertOrUpdateDiamondImageInfoRecord = justInsertOrUpdateInfoRecord
        End Sub
        Protected Friend Sub Set_SearchForExistingInfo(ByVal searchForExisting As Boolean)
            _SearchForExistingInfo = searchForExisting
        End Sub

        Protected Friend Sub Set_DeactivateOlderInfoRecords(ByVal deactivateOlderInfo As Boolean)
            _DeactivateOlderInfoRecords = deactivateOlderInfo
        End Sub
        Protected Friend Sub Set_VerifyInsertDateAgainstImageAddedDateOnSearchForExistingInfo(ByVal verifyExistingWithInsertDate As Boolean)
            _VerifyInsertDateAgainstImageAddedDateOnSearchForExistingInfo = verifyExistingWithInsertDate
        End Sub
        Protected Friend Sub Set_UpdateImageInfoWithXmlIdParameter(ByVal updImgInfoWithXmlId As Boolean)
            _UpdateImageInfoWithXmlIdParameter = updImgInfoWithXmlId
        End Sub

        Protected Friend Sub Set_InfoInsertUserId(ByVal userId As Integer)
            _InfoInsertUserId = userId
        End Sub
        Protected Friend Sub Set_InfoUpdateUserId(ByVal userId As Integer)
            _InfoUpdateUserId = userId
        End Sub
        Protected Friend Sub Set_XmlInsertUserId(ByVal userId As Integer)
            _XmlInsertUserId = userId
        End Sub
        Protected Friend Sub Set_XmlUpdateUserId(ByVal userId As Integer)
            _XmlUpdateUserId = userId
        End Sub

        Protected Friend Sub Set_ImageXml_From_String(ByVal strXml As String)
            If String.IsNullOrWhiteSpace(strXml) = False Then
                Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                _ImageXml = qqHelper.BytesFromString(strXml)
            End If
        End Sub

        Protected Friend Sub Set_IsBillingUpdate(ByVal isBilling As Boolean)
            _IsBillingUpdate = isBilling
        End Sub

        Protected Friend Sub Set_EndorsementOriginType(ByVal endorsementOriginType As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes)
            _endorsementOriginType = endorsementOriginType
        End Sub

        'added 1/4/2021 (Interoperability)
        Protected Friend Sub Set_LastRulesOverrideRecordModifiedDate(ByVal dtMod As String)
            _LastRulesOverrideRecordModifiedDate = dtMod
        End Sub
    End Class
End Namespace
