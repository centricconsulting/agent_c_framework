Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store top-level base database information for a quote; includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteTopLevelQuoteBase_DatabaseInfo 'added 8/18/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Database_QuoteId As String
        Private _Database_QuoteXmlId As String
        Private _Database_QuoteNumber As String
        Private _Database_LobId As String
        Private _Database_CurrentQuoteXmlId As String
        Private _Database_XmlQuoteId As String
        Private _Database_LastAvailableQuoteNumber As String
        Private _Database_QuoteStatusId As String
        Private _Database_XmlStatusId As String
        Private _Database_IsPolicy As Boolean
        Private _Database_DiamondPolicyNumber As String
        Private _Database_OriginatedInVR As Boolean
        Private _Database_EffectiveDate As String

        'added 9/28/2018
        Private _Database_ActualLobId As String
        Private _Database_GoverningStateId As String
        Private _Database_StateIds As String
        Private _Database_QuoteActualLobId As String
        Private _Database_QuoteGoverningStateId As String
        Private _Database_QuoteStateIds As String
        Private _Database_AppActualLobId As String
        Private _Database_AppGoverningStateId As String
        Private _Database_AppStateIds As String

        'added 3/19/2019
        Private _Database_DiamondImageInfoId As Integer
        Private _Database_DiamondImageXmlId As Integer
        Private _Database_DiamondImageInfoType As QuickQuoteDiamondImageInfo.ImageInfoType
        Private _Database_IsBillingUpdate As Boolean
        Private _Database_EndorsementOrigin As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes
        Private _Database_DevDictionaryID As Integer

        'added 1/4/2020 (Interoperability)
        Private _Database_LastRulesOverrideRecordModifiedDate As String

        'added 5/13/2021
        Private _Database_QuickQuote_Inserted As String
        Private _Database_QuickQuote_Updated As String

        'added 11/27/2022
        Private _Database_CompanyId As String
        Private _Database_QuoteCompanyId As String
        Private _Database_AppCompanyId As String

        Private _Database_DiaCompanyId As String 'added 7/27/2023


        Public Property Database_QuoteId As String
            Get
                Return _Database_QuoteId
            End Get
            Set(value As String)
                _Database_QuoteId = value
            End Set
        End Property
        Public Property Database_QuoteXmlId As String
            Get
                Return _Database_QuoteXmlId
            End Get
            Set(value As String)
                _Database_QuoteXmlId = value
            End Set
        End Property
        Public Property Database_QuoteNumber As String
            Get
                Return _Database_QuoteNumber
            End Get
            Set(value As String)
                _Database_QuoteNumber = value
            End Set
        End Property
        Public Property Database_LobId As String
            Get
                Return _Database_LobId
            End Get
            Set(value As String)
                _Database_LobId = value
            End Set
        End Property
        Public Property Database_CurrentQuoteXmlId As String
            Get
                Return _Database_CurrentQuoteXmlId
            End Get
            Set(value As String)
                _Database_CurrentQuoteXmlId = value
            End Set
        End Property
        Public Property Database_XmlQuoteId As String
            Get
                Return _Database_XmlQuoteId
            End Get
            Set(value As String)
                _Database_XmlQuoteId = value
            End Set
        End Property
        Public Property Database_LastAvailableQuoteNumber As String
            Get
                Return _Database_LastAvailableQuoteNumber
            End Get
            Set(value As String)
                _Database_LastAvailableQuoteNumber = value
            End Set
        End Property
        Public Property Database_QuoteStatusId As String
            Get
                Return _Database_QuoteStatusId
            End Get
            Set(value As String)
                _Database_QuoteStatusId = value
            End Set
        End Property
        Public Property Database_XmlStatusId As String
            Get
                Return _Database_XmlStatusId
            End Get
            Set(value As String)
                _Database_XmlStatusId = value
            End Set
        End Property
        Public Property Database_IsPolicy As Boolean
            Get
                Return _Database_IsPolicy
            End Get
            Set(value As Boolean)
                _Database_IsPolicy = value
            End Set
        End Property
        Public Property Database_DiamondPolicyNumber As String
            Get
                Return _Database_DiamondPolicyNumber
            End Get
            Set(value As String)
                _Database_DiamondPolicyNumber = value
            End Set
        End Property
        Public Property Database_OriginatedInVR As Boolean
            Get
                Return _Database_OriginatedInVR
            End Get
            Set(value As Boolean)
                _Database_OriginatedInVR = value
            End Set
        End Property
        Public Property Database_EffectiveDate As String
            Get
                Return _Database_EffectiveDate
            End Get
            Set(value As String)
                _Database_EffectiveDate = value
            End Set
        End Property

        'added 9/28/2018
        Public Property Database_ActualLobId As String
            Get
                Return _Database_ActualLobId
            End Get
            Set(value As String)
                _Database_ActualLobId = value
            End Set
        End Property
        Public Property Database_GoverningStateId As String
            Get
                Return _Database_GoverningStateId
            End Get
            Set(value As String)
                _Database_GoverningStateId = value
            End Set
        End Property
        Public Property Database_StateIds As String
            Get
                Return _Database_StateIds
            End Get
            Set(value As String)
                _Database_StateIds = value
            End Set
        End Property
        Public Property Database_QuoteActualLobId As String
            Get
                Return _Database_QuoteActualLobId
            End Get
            Set(value As String)
                _Database_QuoteActualLobId = value
            End Set
        End Property
        Public Property Database_QuoteGoverningStateId As String
            Get
                Return _Database_QuoteGoverningStateId
            End Get
            Set(value As String)
                _Database_QuoteGoverningStateId = value
            End Set
        End Property
        Public Property Database_QuoteStateIds As String
            Get
                Return _Database_QuoteStateIds
            End Get
            Set(value As String)
                _Database_QuoteStateIds = value
            End Set
        End Property
        Public Property Database_AppActualLobId As String
            Get
                Return _Database_AppActualLobId
            End Get
            Set(value As String)
                _Database_AppActualLobId = value
            End Set
        End Property
        Public Property Database_AppGoverningStateId As String
            Get
                Return _Database_AppGoverningStateId
            End Get
            Set(value As String)
                _Database_AppGoverningStateId = value
            End Set
        End Property
        Public Property Database_AppStateIds As String
            Get
                Return _Database_AppStateIds
            End Get
            Set(value As String)
                _Database_AppStateIds = value
            End Set
        End Property

        'added 3/19/2019
        Public Property Database_DiamondImageInfoId As Integer
            Get
                Return _Database_DiamondImageInfoId
            End Get
            Set(value As Integer)
                _Database_DiamondImageInfoId = value
            End Set
        End Property
        Public Property Database_DiamondImageXmlId As Integer
            Get
                Return _Database_DiamondImageXmlId
            End Get
            Set(value As Integer)
                _Database_DiamondImageXmlId = value
            End Set
        End Property
        Public Property Database_DiamondImageInfoType As QuickQuoteDiamondImageInfo.ImageInfoType
            Get
                Return _Database_DiamondImageInfoType
            End Get
            Set(value As QuickQuoteDiamondImageInfo.ImageInfoType)
                _Database_DiamondImageInfoType = value
            End Set
        End Property
        Public Property Database_IsBillingUpdate As Boolean
            Get
                Return _Database_IsBillingUpdate
            End Get
            Set(value As Boolean)
                _Database_IsBillingUpdate = value
            End Set
        End Property
        Public Property Database_EndorsementOrigin As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes
            Get
                Return _Database_EndorsementOrigin
            End Get
            Set(value As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes)
                _Database_EndorsementOrigin = value
            End Set
        End Property
        Public Property Database_DevDictionaryID As Integer
            Get
                Return _Database_DevDictionaryID
            End Get
            Set(value As Integer)
                _Database_DevDictionaryID = value
            End Set
        End Property

        'added 1/4/2020 (Interoperability)
        Public Property Database_LastRulesOverrideRecordModifiedDate As String
            Get
                Return _Database_LastRulesOverrideRecordModifiedDate
            End Get
            Set(value As String)
                _Database_LastRulesOverrideRecordModifiedDate = value
            End Set
        End Property

        'added 5/13/2021
        Public Property Database_QuickQuote_Inserted As String
            Get
                Return _Database_QuickQuote_Inserted
            End Get
            Set(value As String)
                _Database_QuickQuote_Inserted = value
            End Set
        End Property
        Public Property Database_QuickQuote_Updated As String
            Get
                Return _Database_QuickQuote_Updated
            End Get
            Set(value As String)
                _Database_QuickQuote_Updated = value
            End Set
        End Property

        'added 11/27/2022
        Public Property Database_CompanyId As String
            Get
                Return _Database_CompanyId
            End Get
            Set(value As String)
                _Database_CompanyId = value
            End Set
        End Property
        Public Property Database_QuoteCompanyId As String
            Get
                Return _Database_QuoteCompanyId
            End Get
            Set(value As String)
                _Database_QuoteCompanyId = value
            End Set
        End Property
        Public Property Database_AppCompanyId As String
            Get
                Return _Database_AppCompanyId
            End Get
            Set(value As String)
                _Database_AppCompanyId = value
            End Set
        End Property

        Public Property Database_DiaCompanyId As String 'added 7/27/2023
            Get
                Return _Database_DiaCompanyId
            End Get
            Set(value As String)
                _Database_DiaCompanyId = value
            End Set
        End Property

        <System.Web.Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property Database_DiaCompany As QuickQuoteHelperClass.QuickQuoteCompany 'added 7/27/2023
            Get
                Return QuickQuoteHelperClass.QuickQuoteCompanyForDiamondCompanyId(qqHelper.IntegerForString(_Database_DiaCompanyId))
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As Object) 'generic, but Parent will likely be TopLevelQuoteInfo
            Me.New()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            Reset_Database_Values()


        End Sub

        Public Sub Reset_Database_Values() 'for Copy Quote functionality so database values (specifically quoteId) aren't tied to new quote... since new validation will now prevent save or rate from completing... also don't want to inadvertently use previous policyId, imageNum, or policyImageId from copied quote when using Diamond services
            _Database_QuoteId = ""
            _Database_QuoteXmlId = ""
            _Database_QuoteNumber = ""
            _Database_LobId = ""
            _Database_CurrentQuoteXmlId = ""
            _Database_XmlQuoteId = ""
            _Database_LastAvailableQuoteNumber = ""
            _Database_QuoteStatusId = ""
            _Database_XmlStatusId = ""
            _Database_IsPolicy = False
            _Database_DiamondPolicyNumber = ""
            _Database_OriginatedInVR = False

            _Database_EffectiveDate = ""

            'added 9/28/2018
            _Database_ActualLobId = ""
            _Database_GoverningStateId = ""
            _Database_StateIds = ""
            _Database_QuoteActualLobId = ""
            _Database_QuoteGoverningStateId = ""
            _Database_QuoteStateIds = ""
            _Database_AppActualLobId = ""
            _Database_AppGoverningStateId = ""
            _Database_AppStateIds = ""

            'added 3/19/2019
            _Database_DiamondImageInfoId = 0
            _Database_DiamondImageXmlId = 0
            _Database_DiamondImageInfoType = QuickQuoteDiamondImageInfo.ImageInfoType.None
            _Database_IsBillingUpdate = False

            'added 1/4/2020 (Interoperability)
            _Database_LastRulesOverrideRecordModifiedDate = ""

            'added 5/13/2021
            _Database_QuickQuote_Inserted = ""
            _Database_QuickQuote_Updated = ""

            'added 11/27/2022
            _Database_CompanyId = ""
            _Database_QuoteCompanyId = ""
            _Database_AppCompanyId = ""

            _Database_DiaCompanyId = "" 'added 7/27/2023
        End Sub
        Protected Friend Sub Set_QuoteStatus(ByVal status As QuickQuoteXML.QuickQuoteStatusType)
            _Database_QuoteStatusId = CInt(status).ToString
        End Sub


        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Database_QuoteId <> "" Then
                    str = qqHelper.appendText(str, "QuoteId: " & Me.Database_QuoteId, vbCrLf)
                End If
                If Me.Database_QuoteXmlId <> "" Then
                    str = qqHelper.appendText(str, "QuoteXmlId: " & Me.Database_QuoteXmlId, vbCrLf)
                End If
                'If Me.QuoteNumber <> "" Then
                '    str = qqHelper.appendText(str, "QuoteNumber: " & Me.QuoteNumber, vbCrLf)
                'End If

            Else
                str = "Nothing"
            End If
            Return str
        End Function



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    qqHelper.DisposeString(_Database_QuoteId)
                    qqHelper.DisposeString(_Database_QuoteXmlId)
                    qqHelper.DisposeString(_Database_QuoteNumber)
                    qqHelper.DisposeString(_Database_LobId)
                    qqHelper.DisposeString(_Database_CurrentQuoteXmlId)
                    qqHelper.DisposeString(_Database_XmlQuoteId)
                    qqHelper.DisposeString(_Database_LastAvailableQuoteNumber)
                    qqHelper.DisposeString(_Database_QuoteStatusId)
                    qqHelper.DisposeString(_Database_XmlStatusId)
                    _Database_IsPolicy = Nothing
                    qqHelper.DisposeString(_Database_DiamondPolicyNumber)
                    _Database_OriginatedInVR = Nothing
                    qqHelper.DisposeString(_Database_EffectiveDate)

                    'added 9/28/2018
                    qqHelper.DisposeString(_Database_ActualLobId)
                    qqHelper.DisposeString(_Database_GoverningStateId)
                    qqHelper.DisposeString(_Database_StateIds)
                    qqHelper.DisposeString(_Database_QuoteActualLobId)
                    qqHelper.DisposeString(_Database_QuoteGoverningStateId)
                    qqHelper.DisposeString(_Database_QuoteStateIds)
                    qqHelper.DisposeString(_Database_AppActualLobId)
                    qqHelper.DisposeString(_Database_AppGoverningStateId)
                    qqHelper.DisposeString(_Database_AppStateIds)

                    'added 3/19/2019
                    _Database_DiamondImageInfoId = Nothing
                    _Database_DiamondImageXmlId = Nothing
                    _Database_DiamondImageInfoType = Nothing
                    _Database_IsBillingUpdate = Nothing

                    'added 1/4/2020 (Interoperability)
                    qqHelper.DisposeString(_Database_LastRulesOverrideRecordModifiedDate)

                    'added 5/13/2021
                    qqHelper.DisposeString(_Database_QuickQuote_Inserted)
                    qqHelper.DisposeString(_Database_QuickQuote_Updated)

                    'added 11/27/2022
                    qqHelper.DisposeString(_Database_CompanyId)
                    qqHelper.DisposeString(_Database_QuoteCompanyId)
                    qqHelper.DisposeString(_Database_AppCompanyId)

                    qqHelper.DisposeString(_Database_DiaCompanyId) 'added 7/27/2023)

                    MyBase.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
