Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CAP

    Public Class CAPDriverListHelper

        ''' <summary>
        ''' Removes any invalid drivers from the passed quote object.  This function was written for CAP because if the user goes to the app side and does not complete
        ''' and save a driver, then returns to the quote side they will be unable to rate or return to the app side to correct the incomplete driver.  This was written
        ''' in response to Bug 39337.   MGB 7/29/2019
        ''' 
        ''' This should work for PPA as well.
        ''' 
        ''' Note that for multistate quotes the driver list is stored on the GoverningStateQuote so that's where this function looks for the driver list.
        ''' YOU ALWAYS WANT TO PASS IN THE TOP QUOTE OBJECT!
        ''' 
        ''' What makes an invalid driver?
        ''' - No First and/or Last name 
        ''' - No birth date
        ''' - No Driver License Date, License Number, or License State
        ''' - No valid address
        ''' 
        ''' !! RETURNS THE NUMBER OF INVALID DRIVER RECORDS REMOVED !!
        ''' </summary>
        ''' <param name="qt"></param>
        ''' <returns></returns>
        Public Shared Function RemoveInvalidDriversFromQuote(ByVal QuoteId As String, ByRef qt As QuickQuote.CommonObjects.QuickQuoteObject) As Integer
            Dim remcnt As Integer = 0
            Dim err As String = ""

            If qt Is Nothing Then Return remcnt
            If qt.SubQuotes Is Nothing OrElse qt.SubQuotes.Count <= 0 Then Return remcnt
            Dim firstquote As QuickQuote.CommonObjects.QuickQuoteObject = qt.SubQuotes(0)  ' governing state quote
            If firstquote.Drivers Is Nothing Then Return remcnt
            If firstquote.Drivers.Count = 0 Then Return remcnt

driverLoop:
            For Each drv As QuickQuote.CommonObjects.QuickQuoteDriver In firstquote.Drivers   ' Note that the first subquote is the governing state quote
                If Not QuickQuoteDriverIsValid(drv) Then
                    firstquote.Drivers.Remove(drv)
                    remcnt += 1
                    GoTo driverLoop
                End If
            Next

            If remcnt > 0 Then IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(QuoteId, qt, err)

            Return remcnt
        End Function

        ''' <summary>
        ''' Checks to see if the passed driver is complete/valid
        ''' </summary>
        ''' <param name="drv"></param>
        ''' <returns></returns>
        Private Shared Function QuickQuoteDriverIsValid(ByVal drv As QuickQuote.CommonObjects.QuickQuoteDriver) As Boolean
            ' Check the driver list and it's sub-objects
            If drv Is Nothing Then Return False
            If drv.Equals(New QuickQuote.CommonObjects.QuickQuoteDriver) Then Return False
            If drv.Name Is Nothing Then Return False
            'If drv.Address Is Nothing Then Return False   'address info for drivers is not entered in VR

            ' Required Name Info
            If drv.Name.FirstName Is Nothing OrElse drv.Name.FirstName.Trim = "" Then Return False
            If drv.Name.LastName Is Nothing OrElse drv.Name.LastName.Trim = "" Then Return False
            If drv.Name.BirthDate Is Nothing OrElse drv.Name.BirthDate.Trim = "" Then Return False
            'If drv.Name.DriversLicenseDate Is Nothing OrElse drv.Name.DriversLicenseDate.Trim = "" Then Return False   ' Drivers licence date is not entered in VR
            If drv.Name.DriversLicenseNumber Is Nothing OrElse drv.Name.DriversLicenseNumber.Trim = "" Then Return False
            If drv.Name.DriversLicenseStateId Is Nothing OrElse drv.Name.DriversLicenseStateId.Trim = "" Then Return False

            ' Required Address Info
            ' address info for drivers is not entered in VR
            'If drv.Address.HouseNum Is Nothing OrElse drv.Address.HouseNum.Trim = "" Then Return False
            'If drv.Address.StreetName Is Nothing OrElse drv.Address.StreetName.Trim = "" Then Return False
            'If drv.Address.City Is Nothing OrElse drv.Address.City.Trim = "" Then Return False
            'If drv.Address.Zip Is Nothing OrElse drv.Address.Zip.Trim = "" Then Return False
            'If drv.Address.StateId Is Nothing OrElse drv.Address.StateId.Trim = "" Then Return False

            Return True
        End Function

    End Class

End Namespace
