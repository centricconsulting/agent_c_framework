

Date.prototype.formatMMDDYYYY = function () {
    return (this.getMonth() + 1).toString() +
    "/" + this.getDate() +
    "/" + this.getFullYear();
}

var VrDateTime = new function () {

    // Returns true if the string can be parsed as a date.
    this.IsDate = function (stringDate) {
        var status = false;
        if (!stringDate || stringDate.length <= 0) {
            status = false;
        } else {
            var result = new Date(stringDate);
            if (result == 'Invalid Date') {
                status = false;
            } else {
                status = true;
            }
        }
        return status;
    };

    // Returns the full month day and year string for a provided Date Object.
    // Usage: getCalendarDate(mew Date());
    // Result: 'November 5 2015'
    this.getCalendarDate = function(jsDateObject) {
        var months = new Array(13);
        months[0] = "January";
        months[1] = "February";
        months[2] = "March";
        months[3] = "April";
        months[4] = "May";
        months[5] = "June";
        months[6] = "July";
        months[7] = "August";
        months[8] = "September";
        months[9] = "October";
        months[10] = "November";
        months[11] = "December";
        
        var monthnumber = jsDateObject.getMonth();
        var monthname = months[monthnumber];
        var monthday = jsDateObject.getDate();
        var year = jsDateObject.getYear();
        if (year < 2000) { year = year + 1900; }
        var dateString = monthname +
                         ' ' +
                         monthday +
                         ', ' +
                         year;
        return dateString;
    } // function getCalendarDate()

    // Returns the current time for the client's machine.
    // Result: '03:06:23 PM'
    this.getCurrentClientClockTime = function() {
        var now = new Date();
        var hour = now.getHours();
        var minute = now.getMinutes();
        var second = now.getSeconds();
        var ap = "AM";
        if (hour > 11) { ap = "PM"; }
        if (hour > 12) { hour = hour - 12; }
        if (hour == 0) { hour = 12; }
        if (hour < 10) { hour = "0" + hour; }
        if (minute < 10) { minute = "0" + minute; }
        if (second < 10) { second = "0" + second; }
        var timeString = hour +
                         ':' +
                         minute +
                         ':' +
                         second +
                         " " +
                         ap;
        return timeString;
    } // function getClockTime()

    // Returns the difference in two dates.
    this.compareDates = function (from, to) {
        var dateResult = to.getTime() - from.getTime();
        var dateObj = {};        
        dateObj.years = Math.round(dateResult / (1000 * 60 * 60 * 24 * 7 * 52));
        dateObj.weeks = Math.round(dateResult / (1000 * 60 * 60 * 24 * 7));
        dateObj.days = Math.ceil(dateResult / (1000 * 60 * 60 * 24));
        dateObj.hours = Math.ceil(dateResult / (1000 * 60 * 60));
        dateObj.minutes = Math.ceil(dateResult / (1000 * 60));
        dateObj.seconds = Math.ceil(dateResult / (1000));
        dateObj.milliseconds = dateResult;
        return dateObj;
    }; // compareDates END

    // Returns true if the numeric year provided is a leap year.
    this.isLeapYear = function(year)
    {
        year = parseInt(year, 10);
        if(year % 4 == 0)
        {
            if(year % 100 != 0)
            {
                return true;
            }
            else
            {
                if(year % 400 == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }; // isLeapYear

    // Checks if the string is even a date and if it is and it is 1000 or later it will return true.
    // Usage: isModernDate('July 3, 98') = False
    // Usage: isModernDate('July 3, 1998') = True
    // Usage: isModernDate('1/7/982') = False
    // Usage: isModernDate('1/7/1982') = True
    this.isModernDate = function (stringDate) {
        var status = false;
        if (this.IsDate(stringDate)) {
            if (!stringDate || stringDate.length <= 0) {
                status = false;
            } else {
                var result = new Date(stringDate);
                if (result == 'Invalid Date') {
                    status = false;
                } else {
                    var year = result.getFullYear();
                    if (year > 1000)
                        status = true;
                }
            }
        }
        return status;
    };

    this.compareDateString = function (effectiveDate, AcreageDate) {
        var status = false;
        if (this.IsDate(effectiveDate) && this.IsDate(AcreageDate)) {
            if (!effectiveDate || effectiveDate.length <= 0 || !AcreageDate || AcreageDate.length <= 0) {
                status = false;
            } else {
                var effectiveDateResult = new Date(effectiveDate);
                var AcreageDateResult = new Date(AcreageDate);
                if (effectiveDateResult == 'Invalid Date' || AcreageDateResult == 'Invalid Date') {
                    status = false;
                } else {
                    if (effectiveDateResult < AcreageDateResult) {
                        status = true;
                    }
                }
            }
        }
        return status;
    };

    // Returns the number of years difference between the date(as a textbox ID) provided and the current date.
    // Usage: yearsDifference("txtBirthDate") = N  (represents the years)
    this.yearsDifference = function (dataTextboxId) {
        var currentDate = new Date();

        var regex = /\d{1,2}\/\d{1,2}\/\d{4}/;
        if (regex.test($("#" + dataTextboxId).val())) {
            var _driverDateBirth = new Date($("#" + dataTextboxId).val());

            if (this.isModernDate(_driverDateBirth)) {
                var yearsOld = this.diffInYearsAndDays(currentDate, _driverDateBirth)[0];
                if (!isNaN(yearsOld))
                { return yearsOld };
            }
        }
        return NaN;
    };

    // Returns an array that contains the years,days difference between the dates(strings) provides.
    // Usage: diffInYearsAndDays('01/01/2014','01/03/2015) = [1,2]
    this.diffInYearsAndDays = function(startDate, endDate) {
        // Copy and normalise dates
        var d0 = new Date(startDate);
        d0.setHours(12, 0, 0, 0);
        var d1 = new Date(endDate);
        d1.setHours(12, 0, 0, 0);

        // Make d0 earlier date
        // Can remember a sign here to make -ve if swapped
        if (d0 > d1) {
            var t = d0;
            d0 = d1;
            d1 = t;
        }

        // Initial estimate of years
        var dY = d1.getFullYear() - d0.getFullYear();

        // Modify start date
        d0.setYear(d0.getFullYear() + dY);

        // Adjust if required
        if (d0 > d1) {
            d0.setYear(d0.getFullYear() - 1);
            --dY;
        }

        // Get remaining difference in days
        var dD = (d1 - d0) / 8.64e7;

        // If sign required, deal with it here
        return [dY, dD];
    }

    // Returns the Diamond system date as a javascript Date object.
    this.systemDate = function () {
        return diamondSystemDate;
    };

    // Returns a short date of a Date object.
    // Usage: dateToShortString(new Date('1/1/2018')) = '1/1/2018'
    this.dateToShortString = function (dateObject) {
        return moment(dateObject).format('L');
    };


}; //VrDate END