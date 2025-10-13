Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 8/7/2014

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to property valuation information
    ''' </summary>
    ''' <remarks>currently using e2Value</remarks>
    <Serializable()> _
    Public Class QuickQuotePropertyValuation 'added 8/5/2014
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        'see e2ValueNotes.txt and Portico Integration Instructions 06-10-2013.pdf

        '8/7/2014 note: might move enums to QuickQuotePropertyValuationHelperClass
        Enum ValuationVendor 'needs to match id from PropertyValuationVendor table in QuickQuote database (propertyValuationVendorId)
            None = 0 'not a value in the db table
            e2Value = 1 'db value is the same
            Verisk360 = 2
        End Enum
        Enum ValuationVendorIntegrationType 'added 8/7/2014; needs to match id from PropertyValuationVendorIntegrationType table in QuickQuote database (propertyValuationVendorIntegrationTypeId)
            None = 0 'not a value in the db table
            AdvancedPortico = 1 'db value = Advanced Portico Integration
            Xml = 2 'db value = Xml Integration
        End Enum
        Enum ValuationVendorEstimatorType 'added 8/11/2014; ; needs to match id from PropertyValuationVendorEstimatorType table in QuickQuote database (propertyValuationVendorEstimatorTypeId)
            None = 0 'not a value in the db table
            Residential = 1 'db value = Residential
            ResidentialPro = 2 'db value = Residential Pro
            ExteriorResidential = 3 'db value = Exterior Residential
            FullResidential = 4 'db value = Full Residential
            A_And_A = 5 'db value = A & A
            A_And_APro = 6 'db value = A & A Pro
            AdditionalStructures = 7 'db value = Additional Structures
            QuickCommercial = 8 'db value = Quick Commercial
            QCEPro = 9 'db value = QCE Pro
            ProntoCommercial = 10 'db value = Pronto Commercial
            ProntoLiteCommercial = 11 'db value = Pronto Lite Commercial
            ProntoResidential = 12 'db value = Pronto Residential
            ProntoLiteResidential = 13 'db value = Pronto Lite Residential
            FarmandRanch = 14 'db value = Farm and Ranch
            Homestead = 15 'db value = Homestead
            InVision = 16 'db value = InVision
            InQuest = 17 'db value = InQuest
            ResidentialBanking = 18 'db value = Residential Banking
            FullA_And_A = 19 'db value = Full A & A; added 8/22/2014
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass 'added 7/28/2015

        Private _db_environment As String 'added 8/13/2014
        Private _db_propertyValuationId As String 'added 8/7/2014
        Private _db_propertyValuationRequestId As String 'added 8/7/2014
        Private _db_propertyValuationResponseId As String 'added 8/7/2014
        Private _db_quoteId As String 'added 8/7/2014
        Private _db_locationNum As String 'added 8/7/2014
        Private _db_buildingNum As String 'added 7/28/2015 for Farm
        Private _db_agencyCode As String 'added 8/15/2014
        Private _db_propertyValuationVendorId As String 'added 8/7/2014
        Private _db_propertyValuationVendorIntegrationTypeId As String 'added 8/7/2014
        Private _db_propertyValuationVendorEstimatorTypeId As String 'added 8/11/2014
        Private _Vendor As ValuationVendor 'updated 8/7/2014 from ValuationVendor
        Private _VendorIntegrationType As ValuationVendorIntegrationType 'added 8/7/2014
        Private _VendorEstimatorType As ValuationVendorEstimatorType 'added 8/11/2014
        Private _IFM_UniqueValuationId As String 'id in e2Value redirect URL; return_id in e2Value return URL
        Private _VendorValuationId As String 'propid in e2Value redirect URL; rt_propid in e2Value return URL
        Private _Request As QuickQuotePropertyValuationRequest
        Private _Response As QuickQuotePropertyValuationResponse
        Private _db_inserted As String 'added 8/7/2014
        Private _db_updated As String 'added 8/7/2014
        Private _db_policyId As String '6/1/2023 for endorsements CAH
        Private _db_policyImageNum As String '6/1/2023 for endorsements CAH

        Public Property db_environment As String 'added 8/13/2014
            Get
                Return _db_environment
            End Get
            Set(value As String)
                _db_environment = value
            End Set
        End Property
        Public Property db_propertyValuationId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationId
            End Get
            Set(value As String)
                _db_propertyValuationId = value
            End Set
        End Property
        Public Property db_propertyValuationRequestId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationRequestId
            End Get
            Set(value As String)
                _db_propertyValuationRequestId = value
            End Set
        End Property
        Public Property db_propertyValuationResponseId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationResponseId
            End Get
            Set(value As String)
                _db_propertyValuationResponseId = value
            End Set
        End Property
        Public Property db_quoteId As String 'added 8/7/2014
            Get
                Return _db_quoteId
            End Get
            Set(value As String)
                _db_quoteId = value
            End Set
        End Property
        Public Property db_locationNum As String 'added 8/7/2014
            Get
                Return _db_locationNum
            End Get
            Set(value As String)
                _db_locationNum = value
            End Set
        End Property
        Public Property db_buildingNum As String 'added 7/28/2015 for Farm
            Get
                Return _db_buildingNum
            End Get
            Set(value As String)
                _db_buildingNum = value
            End Set
        End Property
        Public Property db_agencyCode As String 'added 8/15/2014
            Get
                Return _db_agencyCode
            End Get
            Set(value As String)
                _db_agencyCode = value
            End Set
        End Property
        Public Property db_propertyValuationVendorId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationVendorId
            End Get
            Set(value As String)
                _db_propertyValuationVendorId = value
                '_Vendor = ValuationVendor.None
                'If IsNumeric(_db_propertyValuationVendorId) = True AndAlso System.Enum.IsDefined(GetType(ValuationVendor), CInt(_db_propertyValuationVendorId)) = True Then
                '    _Vendor = CInt(_db_propertyValuationVendorId)
                'End If

                If System.Enum.TryParse(Of ValuationVendor)(_db_propertyValuationVendorId, _Vendor) = False Then
                    _Vendor = ValuationVendor.None
                End If
            End Set
        End Property
        Public Property db_propertyValuationVendorIntegrationTypeId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationVendorIntegrationTypeId
            End Get
            Set(value As String)
                _db_propertyValuationVendorIntegrationTypeId = value
                '_VendorIntegrationType = ValuationVendorIntegrationType.None
                'If IsNumeric(_db_propertyValuationVendorIntegrationTypeId) = True AndAlso System.Enum.IsDefined(GetType(ValuationVendorIntegrationType), CInt(_db_propertyValuationVendorIntegrationTypeId)) = True Then
                '    _VendorIntegrationType = CInt(_db_propertyValuationVendorIntegrationTypeId)
                'End If

                If System.Enum.TryParse(Of ValuationVendorIntegrationType)(_db_propertyValuationVendorIntegrationTypeId, _VendorIntegrationType) = False Then
                    _VendorIntegrationType = ValuationVendorIntegrationType.None
                End If
            End Set
        End Property
        Public Property db_propertyValuationVendorEstimatorTypeId As String 'added 8/11/2014
            Get
                Return _db_propertyValuationVendorEstimatorTypeId
            End Get
            Set(value As String)
                _db_propertyValuationVendorEstimatorTypeId = value
                '_VendorEstimatorType = ValuationVendorEstimatorType.None
                'If IsNumeric(_db_propertyValuationVendorEstimatorTypeId) = True AndAlso System.Enum.IsDefined(GetType(ValuationVendorEstimatorType), CInt(_db_propertyValuationVendorEstimatorTypeId)) = True Then
                '    _VendorEstimatorType = CInt(_db_propertyValuationVendorEstimatorTypeId)
                'End If

                If System.Enum.TryParse(Of ValuationVendorEstimatorType)(_db_propertyValuationVendorEstimatorTypeId, _VendorEstimatorType) = False Then
                    _VendorEstimatorType = ValuationVendorEstimatorType.None
                End If
            End Set
        End Property
        Public Property Vendor As ValuationVendor
            Get
                Return _Vendor
            End Get
            Set(value As ValuationVendor)
                _Vendor = value
                _db_propertyValuationVendorId = CInt(_Vendor).ToString
            End Set
        End Property
        Public Property VendorIntegrationType As ValuationVendorIntegrationType 'added 8/7/2014
            Get
                Return _VendorIntegrationType
            End Get
            Set(value As ValuationVendorIntegrationType)
                _VendorIntegrationType = value
                _db_propertyValuationVendorIntegrationTypeId = CInt(_VendorIntegrationType).ToString
            End Set
        End Property
        Public Property VendorEstimatorType As ValuationVendorEstimatorType 'added 8/11/2014
            Get
                Return _VendorEstimatorType
            End Get
            Set(value As ValuationVendorEstimatorType)
                _VendorEstimatorType = value
                _db_propertyValuationVendorEstimatorTypeId = CInt(_VendorEstimatorType).ToString
            End Set
        End Property
        Public Property IFM_UniqueValuationId As String
            Get
                Return _IFM_UniqueValuationId
            End Get
            Set(value As String)
                _IFM_UniqueValuationId = value
            End Set
        End Property
        Public Property VendorValuationId As String
            Get
                Return _VendorValuationId
            End Get
            Set(value As String)
                _VendorValuationId = value
            End Set
        End Property
        Public Property Request As QuickQuotePropertyValuationRequest
            Get
                SetObjectsParent(_Request)
                Return _Request
            End Get
            Set(value As QuickQuotePropertyValuationRequest)
                _Request = value
                SetObjectsParent(_Request)
            End Set
        End Property
        Public Property Response As QuickQuotePropertyValuationResponse
            Get
                SetObjectsParent(_Response)
                Return _Response
            End Get
            Set(value As QuickQuotePropertyValuationResponse)
                _Response = value
                SetObjectsParent(_Response)
            End Set
        End Property
        Public Property db_inserted As String 'added 8/7/2014
            Get
                Return _db_inserted
            End Get
            Set(value As String)
                _db_inserted = value
            End Set
        End Property
        Public Property db_updated As String 'added 8/7/2014
            Get
                Return _db_updated
            End Get
            Set(value As String)
                _db_updated = value
            End Set
        End Property
        Public Property db_policyId As String 'added 8/7/2014
            Get
                Return _db_policyId
            End Get
            Set(value As String)
                _db_policyId = value
            End Set
        End Property

        Public Property db_policyImageNum As String 'added 8/7/2014
            Get
                Return _db_policyImageNum
            End Get
            Set(value As String)
                _db_policyImageNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _db_environment = "" 'added 8/13/2014
            _db_propertyValuationId = "" 'added 8/7/2014
            _db_propertyValuationRequestId = "" 'added 8/7/2014
            _db_propertyValuationResponseId = "" 'added 8/7/2014
            _db_quoteId = "" 'added 8/7/2014
            _db_locationNum = "" 'added 8/7/2014
            _db_buildingNum = "" 'added 7/28/2015 for Farm
            _db_agencyCode = "" 'added 8/15/2014
            _db_propertyValuationVendorId = "" 'added 8/7/2014
            _db_propertyValuationVendorIntegrationTypeId = "" 'added 8/7/2014
            _db_propertyValuationVendorEstimatorTypeId = "" 'added 8/11/2014
            _Vendor = ValuationVendor.None
            _VendorIntegrationType = ValuationVendorIntegrationType.None 'added 8/7/2014
            _VendorEstimatorType = ValuationVendorEstimatorType.None 'added 8/11/2014
            _IFM_UniqueValuationId = ""
            _VendorValuationId = ""
            _Request = New QuickQuotePropertyValuationRequest
            _Response = New QuickQuotePropertyValuationResponse
            _db_inserted = "" 'added 8/7/2014
            _db_updated = "" 'added 8/7/2014
            _db_policyId = "" '6/1/2023 for endorsements CAH
            _db_policyImageNum = "" '6/1/2023 for endorsements CAH
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated for QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _db_environment IsNot Nothing Then 'added 8/13/2014
                        _db_environment = Nothing
                    End If
                    If _db_propertyValuationId IsNot Nothing Then 'added 8/7/2014
                        db_propertyValuationId = Nothing
                    End If
                    If _db_propertyValuationRequestId IsNot Nothing Then 'added 8/7/2014
                        _db_propertyValuationRequestId = Nothing
                    End If
                    If _db_propertyValuationResponseId IsNot Nothing Then 'added 8/7/2014
                        _db_propertyValuationResponseId = Nothing
                    End If
                    If _db_quoteId IsNot Nothing Then 'added 8/7/2014
                        _db_quoteId = Nothing
                    End If
                    If _db_locationNum IsNot Nothing Then 'added 8/7/2014
                        _db_locationNum = Nothing
                    End If
                    qqHelper.DisposeString(_db_buildingNum) 'added 7/28/2015 for Farm
                    If _db_agencyCode IsNot Nothing Then 'added 8/15/2014
                        _db_agencyCode = Nothing
                    End If
                    If _db_propertyValuationVendorId IsNot Nothing Then 'added 8/7/2014
                        _db_propertyValuationVendorId = Nothing
                    End If
                    If _db_propertyValuationVendorIntegrationTypeId IsNot Nothing Then 'added 8/7/2014
                        _db_propertyValuationVendorIntegrationTypeId = Nothing
                    End If
                    If _db_propertyValuationVendorEstimatorTypeId IsNot Nothing Then 'added 8/11/2014
                        _db_propertyValuationVendorEstimatorTypeId = Nothing
                    End If
                    _Vendor = Nothing
                    _VendorIntegrationType = Nothing 'added 8/7/2014
                    _VendorEstimatorType = Nothing 'added 8/11/2014
                    If _IFM_UniqueValuationId IsNot Nothing Then
                        _IFM_UniqueValuationId = Nothing
                    End If
                    If _VendorValuationId IsNot Nothing Then
                        _VendorValuationId = Nothing
                    End If
                    If _Request IsNot Nothing Then
                        _Request.Dispose()
                        _Request = Nothing
                    End If
                    If _Response IsNot Nothing Then
                        _Response = Nothing
                    End If
                    If _db_inserted IsNot Nothing Then 'added 8/7/2014
                        _db_inserted = Nothing
                    End If
                    If _db_updated IsNot Nothing Then 'added 8/7/2014
                        _db_updated = Nothing
                    End If
                    If _db_policyId IsNot Nothing Then '6/1/2023 for endorsements CAH
                        _db_policyId = Nothing
                    End If
                    If _db_policyImageNum IsNot Nothing Then '6/1/2023 for endorsements CAH
                        _db_policyImageNum = Nothing
                    End If
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
        'updated for QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace