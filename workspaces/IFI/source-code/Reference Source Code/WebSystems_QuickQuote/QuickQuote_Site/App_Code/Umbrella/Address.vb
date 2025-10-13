Imports System.Configuration
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Namespace QuickQuote.CommonObjects.Umbrella
    Public Class Address
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Private _Zip As String
        Public Property AddressId As String
        Public Property City As String
        Public Property CountryId As String
        Public Property County As String
        Public Property StateId As String
        Public Property NameAddressSourceId As String
        Public Property HouseNumber As String
        Public Property StreetName As String
        Public Property ApartmentNumber As String
        Public Property Zip As String
            Get
                Return _Zip
            End Get
            Set
                _Zip = _qqHelper.ValidatedZipCode(Value)
            End Set
        End Property

        Public Property DisplayAddress As String
        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(qqAddress As QuickQuoteAddress)
            MyBase.New()
            With qqAddress
                Me.StreetName = .StreetName
                Me.HouseNumber = .HouseNum
                Me.ApartmentNumber = .ApartmentNumber
                Me.City = .City
                Me.StateId = .StateId
                Me.Zip = .Zip
                Me.County = .County
                Me.AddressId = .AddressId
                Me.PolicyId = .PolicyId
                Me.PolicyImageNum = .PolicyImageNum
                Me.DisplayAddress = qqAddress.DisplayAddress
            End With

        End Sub

#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, setItemNumbers, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Address)
                Me.AddressId = .AddressId
                Me.DisplayAddress = .DisplayAddress
                Me.HouseNumber = .HouseNumber
                Me.StreetName = .StreetName
                Me.ApartmentNumber = .ApartmentNumber
                Me.City = .City
                Me.Zip = .Zip
                Me.StateId = .StateId
                Me.County = .County
                Me.CountryId = .CountryId
            End With
        End Sub

        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)

            With src.AsInstance(Of Address)
                matched = matched AndAlso
                          _strCompare.Equals(Me.StreetName, .StreetName) AndAlso
                          _strCompare.Equals(Me.HouseNumber, .HouseNumber) AndAlso
                          _strCompare.Equals(Me.ApartmentNumber, .ApartmentNumber) AndAlso
                          _strCompare.Equals(Me.City, .City) AndAlso
                          _strCompare.Equals(Me.Zip, .Zip) AndAlso
                          _strCompare.Equals(Me.StateId, .StateId)
            End With
            Return matched
        End Function
        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As DCO.InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Address

            End If

            With DirectCast(diaItem, DCO.Address)
                .DetailStatusCode = 1 'TODO: verifiy if need this
                .HouseNumber = Me.HouseNumber?.ToUpper()
                .StreetName = Me.StreetName?.ToUpper()
                .ApartmentNumber = Me.ApartmentNumber.ToUpper()
                .City = Me.City?.ToUpper()
                Integer.TryParse(Me.StateId, .StateId)
                .Zip = IIf(String.IsNullOrWhiteSpace(Me.Zip), "00000-0000", Me.Zip)
                .County = Me.County?.ToUpper()
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As DCO.InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Address)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.AddressNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.AddressNum.Id Then
                        matched = True
                    End If
                Else
                    If Me.HouseNumber.ToUpper() = diaObj.HouseNumber AndAlso
                       Me.StreetName.ToUpper() = diaObj.StreetName.ToUpper() AndAlso
                       Me.ApartmentNumber.ToUpper() = diaObj.ApartmentNumber.ToUpper() AndAlso
                       Me.City = diaObj.City() AndAlso
                       Me.Zip = diaObj.Zip Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
#End Region
#Region "IXmlSerializable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)

            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Me.AddressId)
                        If reader.ReadToDescendant("InternalValue") Then
                            Me.AddressId = reader.ReadElementContentAsString()
                        End If
                        reader.ReadEndElement()

                    Case NameOf(Me.City)
                        Me.City = reader.ReadElementContentAsString()
                    Case NameOf(Me.CountryId)
                        Me.CountryId = reader.ReadElementContentAsString()
                    Case NameOf(Me.County)
                        Me.County = reader.ReadElementContentAsString()
                    Case NameOf(Me.StateId)
                        Me.StateId = reader.ReadElementContentAsString()
                    Case NameOf(Me.NameAddressSourceId)
                        Me.NameAddressSourceId = reader.ReadElementContentAsString()
                    Case NameOf(Me.HouseNumber)
                        Me.HouseNumber = reader.ReadElementContentAsString()
                    Case NameOf(Me.StreetName)
                        Me.StreetName = reader.ReadElementContentAsString()
                    Case NameOf(Me.ApartmentNumber)
                        Me.ApartmentNumber = reader.ReadElementContentAsString()
                    Case NameOf(Me.Zip)
                        Me.Zip = reader.ReadElementContentAsString()
                    Case NameOf(Me.DisplayAddress)
                        Me.DisplayAddress = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)

            writer.WriteElementString(NameOf(Me.City), Me.City)
            writer.WriteElementString(NameOf(Me.CountryId), Me.CountryId)
            writer.WriteElementString(NameOf(Me.County), Me.County)
            writer.WriteElementString(NameOf(Me.StateId), Me.StateId)
            writer.WriteElementString(NameOf(Me.NameAddressSourceId), Me.NameAddressSourceId)
            writer.WriteElementString(NameOf(Me.HouseNumber), Me.HouseNumber)
            writer.WriteElementString(NameOf(Me.StreetName), Me.StreetName)
            writer.WriteElementString(NameOf(Me.ApartmentNumber), Me.ApartmentNumber)
            writer.WriteElementString(NameOf(Me.Zip), Me.Zip)

            'display address
            If String.IsNullOrWhiteSpace(Me.DisplayAddress) = False Then
                'added 1/15/2013 to optionally remove vbCrLf Carriage Return Line Feeds to keep display address on 1 line
                Dim dAdd As String = Me.DisplayAddress
                If ConfigurationManager.AppSettings("QuickQuote_DisplayAddresses_ReplaceCarriageReturnLineFeedsWithSpace") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_DisplayAddresses_ReplaceCarriageReturnLineFeedsWithSpace").ToString) = "YES" Then
                    dAdd = dAdd.Replace(vbCrLf, " ")
                End If
                '.WriteElementString("DisplayAddress", UCase(ad.DisplayAddress)) '*ucase
                'updated 1/15/2013 to use new variable
                writer.WriteElementString(NameOf(Me.DisplayAddress), dAdd.ToUpper()) '*ucase
            End If

            'If Not String.IsNullOrWhiteSpace(Me.AddressId) Then
            '    writer.WriteStartElement(NameOf(Me.AddressId))
            '    writer.WriteElementString("InternalValue", Me.AddressId)
            '    writer.WriteEndElement()
            'End If
        End Sub


        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())
        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.AddressId)
        '                    If reader.ReadToDescendant("InternalValue") Then
        '                        Me.AddressId = reader.ReadElementContentAsString()
        '                        .ReadEndElement()
        '                    End If

        '                Case NameOf(Me.City)
        '                    Me.City = reader.ReadElementContentAsString()
        '                Case NameOf(Me.CountryId)
        '                    Me.CountryId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.County)
        '                    Me.County = reader.ReadElementContentAsString()
        '                Case NameOf(Me.StateId)
        '                    Me.StateId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.NameAddressSourceId)
        '                    Me.NameAddressSourceId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.HouseNumber)
        '                    Me.HouseNumber = reader.ReadElementContentAsString()
        '                Case NameOf(Me.StreetName)
        '                    Me.StreetName = reader.ReadElementContentAsString()
        '                Case NameOf(Me.Zip)
        '                    Me.Zip = reader.ReadElementContentAsString()
        '                Case NameOf(Me.DisplayAddress)
        '                    Me.DisplayAddress = reader.ReadElementContentAsString()
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
        '    writer.WriteElementString(NameOf(Me.City), Me.City)
        '    writer.WriteElementString(NameOf(Me.CountryId), Me.CountryId)
        '    writer.WriteElementString(NameOf(Me.County), Me.County)
        '    writer.WriteElementString(NameOf(Me.StateId), Me.StateId)
        '    writer.WriteElementString(NameOf(Me.NameAddressSourceId), Me.NameAddressSourceId)
        '    writer.WriteElementString(NameOf(Me.HouseNumber), Me.HouseNumber)
        '    writer.WriteElementString(NameOf(Me.StreetName), Me.StreetName)
        '    writer.WriteElementString(NameOf(Me.Zip), Me.Zip)
        '    writer.WriteElementString(NameOf(Me.DisplayAddress), Me.DisplayAddress)

        '    writer.WriteStartElement(NameOf(Me.AddressId))
        '    writer.WriteElementString("InternalValue", Me.AddressId)
        '    writer.WriteEndElement()
        'End Sub
#End Region
    End Class
End Namespace
