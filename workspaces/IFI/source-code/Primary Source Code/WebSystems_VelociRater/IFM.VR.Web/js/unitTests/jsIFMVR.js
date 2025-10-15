///<reference path="../3rdParty/qunit-1.20.0.js" />
///<reference path="../vr.core.js" />




QUnit.test("string.prototype.splitCSV", function (assert) {
	var result = "1,2,3,4,5".splitCSV();
	assert.equal(result[0], 1, "First element is 1.");
	assert.equal(result.length, 5, "Only has 5 elements.");
});

QUnit.test("string.prototype.trim", function (assert) {	
	assert.equal(" Matt A ".trim(), "Matt A", "Equals 'Matt A'.");
});

QUnit.test("string.prototype.toFloat", function (assert) {	
	assert.equal("1.25".toFloat(), 1.25, "Equals number 1.25.");
	assert.equal("$1.25".toFloat(), 1.25, "From Currency Equals number 1.25.");
	assert.equal("1.25%".toFloat(), 1.25, "From Percent Equals number 1.25.");
});

QUnit.test("string.prototype.toInt", function (assert) {	
	assert.equal("1.25".toInt(), 1, "Equals number 1.");
	assert.equal("$1.25".toInt(), 1, "From Currency Equals number 1.25.");
	assert.equal("1.25%".toInt(), 1, "From Percent Equals number 1.25.");
});

QUnit.test("string.prototype.isEmptyOrWhiteSpace", function (assert) {
    assert.equal("".isEmptyOrWhiteSpace()(), true, "Is Empty");
    assert.equal("   ".isEmptyOrWhiteSpace(), true, "Is whitespace");
    assert.equal(" abc ".isEmptyOrWhiteSpace(), false, "Is not empty or whitespace");
});


QUnit.test("string.prototype.capitalize", function (assert) {
	assert.equal("matt a".capitalize(), "Matt A", "Equals 'Matt A'.");
});


QUnit.test("string.prototype.toMaxLength", function (assert) {
	assert.equal("matthew".toMaxLength(4), "matt", "Equals 'matt'.");
});


QUnit.test("string.prototype.replaceAll", function (assert) {
    assert.equal("123*456*789".replaceAll("*", ""), "123456789", "Equals '123456789'.");
});

QUnit.test("string.prototype.removeAll", function (assert) {
    assert.equal("123*456*789".removeAll("*"), "123456789", "Equals '123456789'.");
});


QUnit.test("ifm.vr.workflow page urls", function (assert) {
	assert.equal(ifm.vr.workflow.dfr_App.length > 0, true, "DFR App Equals not empty.");
	assert.equal(ifm.vr.workflow.dfr_Input.length > 0, true, "DFR Input Equals not empty.");
	assert.equal(ifm.vr.workflow.far_App.length > 0, true, "FAR App Equals not empty.");
	assert.equal(ifm.vr.workflow.far_Input.length > 0, true, "FAR Input Equals not empty.");
	assert.equal(ifm.vr.workflow.hom_App.length > 0, true, "HOM App Equals not empty.");
	assert.equal(ifm.vr.workflow.hom_Input.length > 0, true, "HOM Input Equals not empty.");
	assert.equal(ifm.vr.workflow.ppa_App.length > 0, true, "PPA App Equals not empty.");
	assert.equal(ifm.vr.workflow.ppa_Input.length > 0, true, "PPA Input Equals not empty.");
	// should expose the all supported LOBids and test them all in loop
});






QUnit.test("ifm.vr.arrays.except test", function (assert) {
	var result = ifm.vr.arrays.except([1, 2, 3, 4], [2, 3, 4]);
	assert.equal(result[0], 1, "First element is 1.");
	assert.equal(result.length, 1, "Only has 1 element.");
});


QUnit.test("ifm.vr.arrays.intersect test", function (assert) {
	var result = ifm.vr.arrays.intersect([1, 2, 3, 4], [2, 3, 4]);
	assert.equal(result[0], 2, "First element is 2.");
	assert.equal(result.length, 3, "Has 3 elements.");
});


QUnit.test("ifm.vr.arrays.union test", function (assert) {
	var result = ifm.vr.arrays.union([1, 2, 3, 4], [2, 3, 4]);
	assert.equal(result[0], 1, "First element is 1.");
	assert.equal(result.length, 4, "Has 4 elements.");
});


QUnit.test("ifm.vr.numbers.RoundUpBy100", function (assert) {
	assert.equal(ifm.vr.numbers.RoundUpBy100("125"), "200", "Equals 200.");
});



QUnit.test("ifm.vr.stringFormating.asAlphabeticNumeric", function (assert) {
	assert.equal(ifm.vr.stringFormating.asAlphabeticNumeric("123 #$%abc"), "123 abc", "Equals '123 abc'.");
});

QUnit.test("ifm.vr.stringFormating.asAlphabeticOnly", function (assert) {
	assert.equal(ifm.vr.stringFormating.asAlphabeticOnly("123#$%a bc"), "a bc", "Equals 'a bc'.");
});

QUnit.test("ifm.vr.stringFormating.asCurrency", function (assert) {
	assert.equal(ifm.vr.stringFormating.asCurrency("1234.50#$%abc"), "$1,234.50", "Equals '$1,234.50'.");
});

QUnit.test("ifm.vr.stringFormating.asCurrencyNoCents", function (assert) {
	assert.equal(ifm.vr.stringFormating.asCurrencyNoCents("1234.50#$%abc"), "$1,234", "Equals '$1,234'.");
});

QUnit.test("ifm.vr.stringFormating.asDate", function (assert) {
	assert.equal(ifm.vr.stringFormating.asDate("12242015"), "12/24/2015", "Equals '12/24/2015'");
	assert.equal(ifm.vr.stringFormating.asDate("12/24/2015"), "12/24/2015", "Equals '12/24/2015'");
	assert.equal(ifm.vr.stringFormating.asDate("12-24-2015"), "12/24/2015", "Equals '12/24/2015'");
});

QUnit.test("ifm.vr.stringFormating.asNumberNoCommas", function (assert) {
	assert.equal(ifm.vr.stringFormating.asNumberNoCommas("1234.50#$%abc"), "1234.50", "Equals '1234.50'.");
});

QUnit.test("ifm.vr.stringFormating.asNumberWithCommas", function (assert) {
	assert.equal(ifm.vr.stringFormating.asNumberWithCommas("1234.50$%#abc"), "1,234.50", "Equals '1,234.50'.");
});

QUnit.test("ifm.vr.stringFormating.asPositiveNumberNoCommas", function (assert) {
	assert.equal(ifm.vr.stringFormating.asPositiveNumberNoCommas("-1234.50#$%abc"), "1234.50", "Equals '1234.50'.");
});



QUnit.test("ifm.vr.vrDateTime.compareDates", function (assert) {
    var result = ifm.vr.vrDateTime.compareDates(new Date('7-20-1979'), new Date('11-06-2015')).years;
	assert.equal(result , 36, "Equals 36 years.");
});

QUnit.test("ifm.vr.vrDateTime.getCalendarDate", function (assert) {
	var result = ifm.vr.vrDateTime.getCalendarDate(new Date('5-01-2015'));
	assert.equal(result, 'May 1, 2015', "Equals 'May 1, 2015'.");
});


QUnit.test("ifm.vr.vrDateTime.IsDate", function (assert) {
    assert.equal(ifm.vr.vrDateTime.IsDate('11-06-2015'), true, "Equals true.");
    assert.equal(ifm.vr.vrDateTime.IsDate('11/06/2015'), true, "Equals true.");
	assert.equal(ifm.vr.vrDateTime.IsDate('today'), false, "Equals false.");
});



QUnit.test("ifm.vr.vrdata.Applicant.GetApplicants test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;

	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.Applicant.GetApplicants(ifm.vr.currentQuote.agencyId, "matt", "amo","", "", function (data) {
		//assert.equal(data != null, true, "Data is not null.");
		assert.equal(data.length > 0, true, "Has results.");
		done();
	});
}); // End Test


QUnit.test("ifm.vr.vrdata.AdditionalInterest.GetAdditionalInterests test", function (assert) {	
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(3);
	var done = assert.async();
	ifm.vr.vrdata.AdditionalInterest.GetAdditionalInterests("bank", "", "", "", function (data) {		
		assert.equal(data != null, true, "Data is not null.");
		assert.equal(data.length > 0, true, "Has results.");

		var allNamesAreCommercial = true;
		for (var i = 0; i < data.length; i++)
		{
			if (data[i].NameTypeId != 2)
				allNamesAreCommercial = false;
		}
		assert.equal(allNamesAreCommercial, true, "Has all commercial names.");
		done();		
	});	
	
}); // End Test


QUnit.test("ifm.vr.vrdata.Client.GetPersonalClients test", function (assert) {	
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.Client.GetPersonalClients(ifm.vr.currentQuote.agencyId, "matt","amo" ,"" , "","", function (data) {		
		assert.equal(data.length > 0, true, "Has results.");
		done();
	});

}); // End Test



QUnit.test("ifm.vr.vrdata.MineSubsidence.MineSubsidenceCapableCountyNames test", function (assert) {
	var data = ifm.vr.vrdata.MineSubsidence.MineSubsidenceCapableCountyNames;
	assert.equal(data.length > 0, true, "Has results.");
}); // End Test


QUnit.test("ifm.vr.vrdata.ProtectionClass.GetProtectionClass test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.ProtectionClass.GetProtectionClass("clinton",false, function (data) {
		assert.equal(data.length > 0, true, "Has results.");
		done();
	});

}); // End Test



QUnit.test("ifm.vr.vrdata.Quotes.GetQuotess test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.Quotes.GetQuotes(179,0,8,"","","","","","","","","", function (data) {
		assert.equal(data.Results.length > 0, true, "Has results.");
		done();
	});

}); // End Test


QUnit.test("ifm.vr.vrdata.RoutingNumber.GetBankNameFromRoutingNumber test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.RoutingNumber.GetBankNameFromRoutingNumber("062000019", function (data) {
		assert.equal(data == "Regions Bank".toUpperCase(), true, "Has results.");
		done();
	});

}); // End Test



QUnit.test("ifm.vr.vrdata.TownShip.GetTownshipsByCountyName test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.TownShip.GetTownshipsByCountyName("Boone","111", function (data) {
		assert.equal(data.length > 0, true, "Has results.");
		done();
	});

}); // End Test


QUnit.test("ifm.vr.vrdata.VIN.GetFromMakeModelYear test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.VIN.GetFromMakeModelYear("Neon","Dodge","1998", function (data) {
		assert.equal(data.length > 0, true, "Has results.");
		done();
	});

}); // End Test


QUnit.test("ifm.vr.vrdata.ZipCode.GetZipCodeInformation test", function (assert) {
	ifm.vr.currentQuote.agencyId = 179;
	assert.expect(1);
	var done = assert.async();
	ifm.vr.vrdata.ZipCode.GetZipCodeInformation("46071", function (data) {
		assert.equal(data.length > 0, true, "Has results.");
		done();
	});

}); // End Test

QUnit.test("ifm.vr.vrdata.ProtectionClass.GetProtectionClass test", function (assert) {
    ifm.vr.currentQuote.agencyId = 179;
    assert.expect(1);
    var done = assert.async();
    ifm.vr.vrdata.ProtectionClass.GetProtectionClass("Boone", false, '16', function (data) {
        assert.equal(data.length > 0, true, "Has results.");
        done();
    });

}); // End Test

QUnit.test("ifm.vr.vrdata.RiskGrade.SearchCommRiskGrades test", function (assert) {
    ifm.vr.currentQuote.agencyId = 179;
    assert.expect(1);
    var done = assert.async();
    ifm.vr.vrdata.RiskGrade.SearchCommRiskGrades("3","sa", "", function (data) {
        assert.equal(data.length > 0, true, "Has results.");
        done();
    });

}); // End Test

QUnit.test("ifm.vr.vrdata.GlClassCodes.searchCodes test", function (assert) {
    ifm.vr.currentQuote.agencyId = 179;
    assert.expect(1);
    var done = assert.async();
    ifm.vr.vrdata.GlClassCodes.searchCodes("0", "sa","4","54", function (data) {
        assert.equal(data.length > 0, true, "Has results.");
        done();
    });

}); // End Test


