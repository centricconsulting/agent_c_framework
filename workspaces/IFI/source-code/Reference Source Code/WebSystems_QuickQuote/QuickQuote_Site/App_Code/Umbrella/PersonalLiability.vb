Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    ''' <summary>
    ''' objects used to store underlying personal liability information (on Umbrella Policies)
    ''' </summary>
    ''' <remarks>typically found under QuickQuotePolicyInfo object (<see cref="PolicyInfo"/>) as a list</remarks>
    <Serializable()>
    Public Class PersonalLiability 'added 4/21/2020 for PUP/FUP
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable
        'Implements IDisposable

        'Dim qqHelper As New QuickQuoteHelperClass

        'Private _DetailStatusCode As String
        'Private _PersonalLiabilityNum As String 'for reconciliation
        'Private _Acreage As String 'int
        'Private _Address As QuickQuoteAddress
        'Private _LinkNumber As String
        'Private _NumberOfItems As String 'int
        'Private _PersonalLiabilityTypeId As String 'static data
        'Private _PremiumFullterm As String

        'Public Property DetailStatusCode As String
        '    Get
        '        Return _DetailStatusCode
        '    End Get
        '    Set(value As String)
        '        _DetailStatusCode = value
        '    End Set
        'End Property

        Public Property Acreage As String 'int

        Public Property Address As Address

        'Public Property LinkNumber As String
        '    Get
        '        Return _LinkNumber
        '    End Get
        '    Set(value As String)
        '        _LinkNumber = value
        '    End Set
        'End Property
        'Public Property NumberOfItems As String 'int
        '    Get
        '        Return _NumberOfItems
        '    End Get
        '    Set(value As String)
        '        _NumberOfItems = value
        '    End Set
        'End Property
        'Public Property PersonalLiabilityTypeId As String 'static data
        '    Get
        '        Return _PersonalLiabilityTypeId
        '    End Get
        '    Set(value As String)
        '        _PersonalLiabilityTypeId = value
        '    End Set
        'End Property
        'Public Property PremiumFullterm As String
        '    Get
        '        Return _PremiumFullterm
        '    End Get
        '    Set(value As String)
        '        _PremiumFullterm = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PremiumFullterm)
        '    End Set
        'End Property

        Public Property OccupancyCode As String
        Public Property IncidentalFarmingOnPremises As String
        Public Property IncidentalFarmingOffPremises As String
        'Public Property Description As String



        Public Sub New()
            MyBase.New()
            'SetDefaults()
        End Sub
#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of PersonalLiability)
                Me.Address.InitIfNothing.CopyFrom(.Address)
            End With
        End Sub

        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of PersonalLiability)
                matched = matched AndAlso
                          Me.OccupancyCode = .OccupancyCode AndAlso
                          Me.Address.IsMatchFor(.Address)
            End With
            Return matched
        End Function

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As DCO.InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.PersonalLiability
            End If

            With DirectCast(diaItem, DCO.Policy.PersonalLiability)
                .DetailStatusCode = 1
                Integer.TryParse(Me.TypeId, .PersonalLiabilityTypeId)
                Integer.TryParse(Me.Acreage, .Acreage)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
                Boolean.TryParse(Me.IncidentalFarmingOnPremises, .IncidentalFarmingOnPremises)
                Boolean.TryParse(Me.IncidentalFarmingOffPremises, .FarmLiabilityOffPremises)
                Me.Address.ConvertToDiamondItem(.Address, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As DCO.InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.PersonalLiability)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.PersonalLiabilityNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.PersonalLiabilityNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.PersonalLiabilityTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function

#End Region
#Region "IXmlSeriazable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True

                Select Case reader.Name
                    Case NameOf(Me.Address)
                        Dim xmlSer As New XmlSerializer(GetType(Address))
                        Me.Address = xmlSer.Deserialize(reader)

                    Case NameOf(Me.Acreage)
                        Me.Acreage = reader.ReadElementContentAsString()

                    Case "FarmLiabilityOffPremises"
                        Me.IncidentalFarmingOffPremises = reader.ReadElementContentAsString()
                    Case NameOf(Me.IncidentalFarmingOnPremises)
                        Me.IncidentalFarmingOnPremises = reader.ReadElementContentAsString()
                    Case NameOf(Me.OccupancyCode)
                        Me.OccupancyCode = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)
            With writer
                If Me.Address IsNot Nothing Then
                    Dim xmlSer As New XmlSerializer(GetType(Address))
                    'For Each drv As Driver In Me.Drivers
                    xmlSer.Serialize(writer, Me.Address)
                    'Next
                End If

                .WriteElementString(NameOf(Me.Acreage), Me.Acreage)
                .WriteElementString("FarmLiabilityOffPremises", Me.IncidentalFarmingOffPremises)
                .WriteElementString(NameOf(Me.IncidentalFarmingOnPremises), Me.IncidentalFarmingOnPremises)
                .WriteElementString(NameOf(Me.OccupancyCode), Me.OccupancyCode)
            End With
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.Address)
        '                    Dim xmlSer As New XmlSerializer(GetType(Address))
        '                    Me.Address = xmlSer.Deserialize(reader.ReadSubtree())

        '                Case NameOf(Me.Acreage)
        '                    Me.Acreage = reader.ReadElementContentAsString()

        '                Case NameOf(Me.IncidentalFarmingOffPremises)
        '                    Me.IncidentalFarmingOffPremises = reader.ReadElementContentAsString()
        '                Case NameOf(Me.IncidentalFarmingOnPremises)
        '                    Me.IncidentalFarmingOnPremises = reader.ReadElementContentAsString()
        '                Case NameOf(Me.OccupancyCode)
        '                    Me.OccupancyCode = reader.ReadElementContentAsString()
        '                Case Else

        '            End Select
        '            .ReadEndElement()
        '            .MoveToContent()
        '        End While
        '    End With
        '    reader.ReadEndElement()
        'End Sub

        'Public Overrides Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        '    MyBase.WriteXml(writer)

        '    With writer

        '        If Me.Address IsNot Nothing Then
        '            Dim xmlSer As New XmlSerializer(GetType(Address))
        '            'For Each drv As Driver In Me.Drivers
        '            xmlSer.Serialize(writer, Me.Address)
        '            'Next
        '        End If

        '        .WriteElementString(NameOf(Me.Acreage), Me.Acreage)
        '        .WriteElementString(NameOf(Me.IncidentalFarmingOffPremises), Me.IncidentalFarmingOffPremises)
        '        .WriteElementString(NameOf(Me.IncidentalFarmingOnPremises), Me.IncidentalFarmingOnPremises)
        '        .WriteElementString(NameOf(Me.OccupancyCode), Me.OccupancyCode)
        '    End With
        'End Sub
        'Private Sub SetDefaults()
        '    _DetailStatusCode = ""
        '    _PersonalLiabilityNum = "" 'for reconciliation
        '    _Acreage = "" 'int
        '    _Address = New QuickQuoteAddress
        '    _LinkNumber = ""
        '    _NumberOfItems = "" 'int
        '    _PersonalLiabilityTypeId = "" 'static data
        '    _PremiumFullterm = ""

        'End Sub
        'Public Function HasValidPersonalLiabilityNum() As Boolean 'added for reconciliation purposes
        '    Return qqHelper.IsValidQuickQuoteIdOrNum(_PersonalLiabilityNum)
        'End Function
        'Public Overrides Function ToString() As String
        '    Dim str As String = ""
        '    If Me IsNot Nothing Then
        '        If String.IsNullOrWhiteSpace(Me.LinkNumber) = False Then
        '            str = qqHelper.appendText(str, "LinkNumber: " & Me.LinkNumber, vbCrLf)
        '        End If
        '        If qqHelper.IsPositiveIntegerString(Me.NumberOfItems) = False Then
        '            str = qqHelper.appendText(str, "NumberOfItems: " & Me.NumberOfItems, vbCrLf)
        '        End If
        '        If qqHelper.IsPositiveDecimalString(Me.PremiumFullterm) = False Then
        '            str = qqHelper.appendText(str, "PremiumFullterm: " & Me.PremiumFullterm, vbCrLf)
        '        End If
        '        If Me.Address IsNot Nothing AndAlso Me.Address.HasData = True Then
        '            str = qqHelper.appendText(str, "Address: " & Me.Address.DisplayAddress, vbCrLf)
        '        End If
        '    Else
        '        str = "Nothing"
        '    End If
        '    Return str
        'End Function


        '#Region "IDisposable Support"
        '        Private disposedValue As Boolean ' To detect redundant calls

        '        ' IDisposable
        '        'Protected Overridable Sub Dispose(disposing As Boolean)
        '        Protected Overloads Sub Dispose(disposing As Boolean)
        '            If Not Me.disposedValue Then
        '                If disposing Then
        '                    ' TODO: dispose managed state (managed objects).
        '                    qqHelper.DisposeString(_DetailStatusCode)
        '                    qqHelper.DisposeString(_PersonalLiabilityNum) 'for reconciliation
        '                    qqHelper.DisposeString(_Acreage) 'int
        '                    If _Address IsNot Nothing Then
        '                        _Address.Dispose()
        '                        _Address = Nothing
        '                    End If
        '                    qqHelper.DisposeString(_LinkNumber)
        '                    qqHelper.DisposeString(_NumberOfItems) 'int
        '                    qqHelper.DisposeString(_PersonalLiabilityTypeId) 'static data
        '                    qqHelper.DisposeString(_PremiumFullterm)

        '                    MyBase.Dispose()
        '                End If

        '                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        '                ' TODO: set large fields to null.
        '            End If
        '            Me.disposedValue = True
        '        End Sub

        '        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        '        'Protected Overrides Sub Finalize()
        '        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '        '    Dispose(False)
        '        '    MyBase.Finalize()
        '        'End Sub

        '        ' This code added by Visual Basic to correctly implement the disposable pattern.
        '        'Public Sub Dispose() Implements IDisposable.Dispose
        '        Public Overrides Sub Dispose()
        '            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '            Dispose(True)
        '            GC.SuppressFinalize(Me)
        '        End Sub
        '#End Region
#End Region
    End Class
End Namespace

