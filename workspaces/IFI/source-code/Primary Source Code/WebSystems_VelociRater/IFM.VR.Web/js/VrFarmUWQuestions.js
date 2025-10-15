// Initial Page load / Live Functionality.
$(document).ready(function () {

    var rdoChoice = {
                no: 1,
                yes: 2
    };

    function GetRadioDiamondCode(element) {
        return $(element).closest('span').data('diamondcode');
    }

    function GetRadioYesOrNo(element) {
        if($(element).val().toLowerCase().indexOf("yes") != -1)
        {
            return rdoChoice.yes;
        }
        else
        {
            return rdoChoice.no;
        }
    }

    function FormatRow(rowName, className) {
        $('[id*=' + rowName).removeClass("OddRow");
        $('[id*=' + rowName).removeClass("EvenRow");
        $('[id*=' + rowName).addClass(className);
    }

    function ColorAlternateRows() {
        FormatRow("trQuestion1", "OddRow")
        FormatRow("trQuestion1AddlInfoRow", "OddRow")
        FormatRow("trQuestion2", "EvenRow")
        FormatRow("trQuestion2AddlInfoRow_DDL", "EvenRow")
        FormatRow("trQuestion2AddlInfoRow_TEXTBOX", "EvenRow")
        FormatRow("trQuestion3", "OddRow")
        FormatRow("trQuestion3AddlInfoRow", "OddRow")
        FormatRow("trQuestion4", "EvenRow")
        FormatRow("trQuestion4AddlInfoRow", "EvenRow")
        FormatRow("trQuestion5", "OddRow")
        FormatRow("trQuestion5AddlInfoRow", "OddRow")
        FormatRow("trQuestion6", "EvenRow")
        FormatRow("trQuestion6AddlInfoRow", "EvenRow")
        FormatRow("trQuestion7", "OddRow")
        FormatRow("trQuestion7AddlInfoRow", "OddRow")
        FormatRow("trQuestion8", "EvenRow")
        FormatRow("trQuestion8AddlInfoRow", "EvenRow")
        FormatRow("trQuestion9", "OddRow")
        FormatRow("trQuestion9AddlInfoRow", "OddRow")
        FormatRow("trQuestion10", "EvenRow")
        FormatRow("trQuestion10AddlInfoRow", "EvenRow")
        FormatRow("trQuestion10a", "OddRow")
        FormatRow("trQuestion10aAddlInfoRow", "OddRow")
        FormatRow("trQuestion10b", "EvenRow")
        FormatRow("trQuestion10c", "OddRow")

        var Q10Yes = $('[id*=rbQ10Yes]:checked').val();
        var Q12Class = "";
        var Q12eClass = "";
        if (Q10Yes) {
            FormatRow("trQuestion11", "EvenRow")
            FormatRow("trQuestion11AddlInfoRow", "EvenRow")
            FormatRow("trQuestion12", "OddRow")
            FormatRow("trQuestion12a", "EvenRow")
            FormatRow("trQuestion12aAddlInfoRow", "EvenRow")
            FormatRow("trQuestion12b", "OddRow")
            FormatRow("trQuestion12bAddlInfoRow", "OddRow")
            FormatRow("trQuestion12c", "EvenRow")
            FormatRow("trQuestion12cAddlInfoRow", "EvenRow")
            FormatRow("trQuestion12d", "OddRow")
            FormatRow("trQuestion12dAddlInfoRow", "OddRow")
            FormatRow("trQuestion12e", "EvenRow")
            FormatRow("trQuestion12eAddlInfoRow", "EvenRow") //currently not in use
            Q12Class = "OddRow";
            Q12eClass = "EvenRow";
        } else {
            FormatRow("trQuestion11", "OddRow")
            FormatRow("trQuestion11AddlInfoRow", "OddRow")
            FormatRow("trQuestion12", "EvenRow")
            FormatRow("trQuestion12a", "OddRow")
            FormatRow("trQuestion12aAddlInfoRow", "OddRow")
            FormatRow("trQuestion12b", "EvenRow")
            FormatRow("trQuestion12bAddlInfoRow", "EvenRow")
            FormatRow("trQuestion12c", "OddRow")
            FormatRow("trQuestion12cAddlInfoRow", "OddRow")
            FormatRow("trQuestion12d", "EvenRow")
            FormatRow("trQuestion12dAddlInfoRow", "EvenRow")
            FormatRow("trQuestion12e", "OddRow")
            FormatRow("trQuestion12eAddlInfoRow", "OddRow") //currently not in use
            Q12Class = "EvenRow";
            Q12eClass = "OddRow";
        }

        var Q12Yes = $('[id*=rbQ12Yes]:checked').val();
        var Q13Class = "";
        var Q13aClass = "";
        if (Q12Yes) {
            if (Q12eClass == "OddRow") {
                FormatRow("trQuestion13", "EvenRow")
                FormatRow("trQuestion13a", "OddRow")
                Q13Class = "EvenRow";
                Q13aClass = "OddRow";
            } else {
                FormatRow("trQuestion13", "OddRow")
                FormatRow("trQuestion13a", "EvenRow")
                Q13Class = "OddRow";
                Q13aClass = "EvenRow";
            }
        } else {
            if (Q12Class == "OddRow") {
                FormatRow("trQuestion13", "EvenRow")
                FormatRow("trQuestion13a", "OddRow")
                Q13Class = "EvenRow";
                Q13aClass = "OddRow";
            } else {
                FormatRow("trQuestion13", "OddRow")
                FormatRow("trQuestion13a", "EvenRow")
                Q13Class = "OddRow";
                Q13aClass = "EvenRow";
            }
        }

        var Q13Yes = $('[id*=rbQ13Yes]:checked').val();
        var Q14Class = "";
        var Q14bClass = "";
        if (Q13Yes) {
            if (Q13aClass == "OddRow") {
                FormatRow("trQuestion14", "EvenRow")
                FormatRow("trQuestion14a", "OddRow")
                FormatRow("trQuestion14aAddlInfoRow", "OddRow")
                FormatRow("trQuestion14b", "EvenRow")
                FormatRow("trQuestion14bAddlInfoRow", "EvenRow")
                Q14Class = "EvenRow";
                Q14bClass = "EvenRow";
            } else {
                FormatRow("trQuestion14", "OddRow")
                FormatRow("trQuestion14a", "EvenRow")
                FormatRow("trQuestion14aAddlInfoRow", "EvenRow")
                FormatRow("trQuestion14b", "OddRow")
                FormatRow("trQuestion14bAddlInfoRow", "OddRow")
                Q14Class = "OddRow";
                Q14bClass = "OddRow";
            }
        } else {
            if (Q13Class == "OddRow") {
                FormatRow("trQuestion14", "EvenRow")
                FormatRow("trQuestion14a", "OddRow")
                FormatRow("trQuestion14aAddlInfoRow", "OddRow")
                FormatRow("trQuestion14b", "EvenRow")
                FormatRow("trQuestion14bAddlInfoRow", "EvenRow")
                Q14Class = "EvenRow";
                Q14bClass = "EvenRow";
            } else {
                FormatRow("trQuestion14", "OddRow")
                FormatRow("trQuestion14a", "EvenRow")
                FormatRow("trQuestion14aAddlInfoRow", "EvenRow")
                FormatRow("trQuestion14b", "OddRow")
                FormatRow("trQuestion14bAddlInfoRow", "OddRow")
                Q14Class = "OddRow";
                Q14bClass = "OddRow";
            }
        }

        var Q14Yes = $('[id*=rbQ14Yes]:checked').val();
        var Q15Class = "";
        var Q15aClass = "";
        if (Q14Yes) {
            if (Q14bClass == "OddRow") {
                FormatRow("trQuestion15", "EvenRow")
                FormatRow("trQuestion15a", "OddRow")
                Q15Class = "EvenRow";
                Q15aClass = "OddRow";
            } else {
                FormatRow("trQuestion15", "OddRow")
                FormatRow("trQuestion15a", "EvenRow")
                Q15Class = "OddRow";
                Q15aClass = "EvenRow";
            }
        } else {
            if (Q14Class == "OddRow") {
                FormatRow("trQuestion15", "EvenRow")
                FormatRow("trQuestion15a", "OddRow")
                Q15Class = "EvenRow";
                Q15aClass = "OddRow";
            } else {
                FormatRow("trQuestion15", "OddRow")
                FormatRow("trQuestion15a", "EvenRow")
                Q15Class = "OddRow";
                Q15aClass = "EvenRow";
            }
        }

        var Q15Yes = $('[id*=rbQ15Yes]:checked').val();
        var Q19Class = "";
        var Q19aClass = "";
        var Q19bClass = "";
        if (Q15Yes) {
            if (Q15aClass == "OddRow") {
                FormatRow("trQuestion16", "EvenRow")
                FormatRow("trQuestion17", "OddRow")
                FormatRow("trQuestion17AddlInfoRow", "OddRow")
                FormatRow("trQuestion18", "EvenRow")
                FormatRow("trQuestion19", "OddRow")
                FormatRow("trQuestion19a", "EvenRow")
                FormatRow("trQuestion19b", "OddRow")
                FormatRow("trQuestion19bAddlInfoRow", "OddRow")
                Q19Class = "OddRow";
                Q19aClass = "EvenRow";
                Q19bClass = "OddRow"
            } else {
                FormatRow("trQuestion16", "OddRow")
                FormatRow("trQuestion17", "EvenRow")
                FormatRow("trQuestion17AddlInfoRow", "EvenRow")
                FormatRow("trQuestion18", "OddRow")
                FormatRow("trQuestion19", "EvenRow")
                FormatRow("trQuestion19a", "OddRow")
                FormatRow("trQuestion19b", "EvenRow")
                FormatRow("trQuestion19bAddlInfoRow", "EvenRow")
                Q19Class = "EvenRow";
                Q19aClass = "OddRow";
                Q19bClass = "EvenRow"
            }
        } else {
            if (Q15Class == "OddRow") {
                FormatRow("trQuestion16", "EvenRow")
                FormatRow("trQuestion17", "OddRow")
                FormatRow("trQuestion17AddlInfoRow", "OddRow")
                FormatRow("trQuestion18", "EvenRow")
                FormatRow("trQuestion19", "OddRow")
                FormatRow("trQuestion19a", "EvenRow")
                FormatRow("trQuestion19b", "OddRow")
                FormatRow("trQuestion19bAddlInfoRow", "OddRow")
                Q19Class = "OddRow";
                Q19aClass = "EvenRow";
                Q19bClass = "OddRow"
            } else {
                FormatRow("trQuestion16", "OddRow")
                FormatRow("trQuestion17", "EvenRow")
                FormatRow("trQuestion17AddlInfoRow", "EvenRow")
                FormatRow("trQuestion18", "OddRow")
                FormatRow("trQuestion19", "EvenRow")
                FormatRow("trQuestion19a", "OddRow")
                FormatRow("trQuestion19b", "EvenRow")
                FormatRow("trQuestion19bAddlInfoRow", "EvenRow")
                Q19Class = "EvenRow";
                Q19aClass = "OddRow";
                Q19bClass = "EvenRow"
            }
        }

        var Q19Yes = $('[id*=rbQ19Yes]:checked').val();
        var Q19aNo = $('[id*=rbQ19aNo]:checked').val();
        if (Q19Yes) {
            if (Q19aNo) {
                if (Q19bClass == "OddRow") {
                    FormatRow("trQuestion20", "EvenRow")
                    FormatRow("trQuestion20AddlInfoRow", "EvenRow")
                } else {
                    FormatRow("trQuestion20", "OddRow")
                    FormatRow("trQuestion20AddlInfoRow", "OddRow")
                }
            } else {
                if (Q19aClass == "OddRow") {
                    FormatRow("trQuestion20", "EvenRow")
                    FormatRow("trQuestion20AddlInfoRow", "EvenRow")
                } else {
                    FormatRow("trQuestion20", "OddRow")
                    FormatRow("trQuestion20AddlInfoRow", "OddRow")
                }
            }            
        } else {
            if (Q19Class == "OddRow") {
                FormatRow("trQuestion20", "EvenRow")
                FormatRow("trQuestion20AddlInfoRow", "EvenRow")
            } else {
                FormatRow("trQuestion20", "OddRow")
                FormatRow("trQuestion20AddlInfoRow", "OddRow")
            }
        }

    }

    // Color Alternate Rows
    ColorAlternateRows();

    //Q2 show drop down list or input textbox for previous carrier
    $('[id*=ddlQ2PreviousCarrier]').on('change', function () {
        selectedValue = $(this).val();
        if (selectedValue == "OTHER") {
            $('[id*=trQuestion2AddlInfoRow_TEXTBOX]').show();
            $('[id*=trQuestion2AddlInfoRow_DDL]').hide();
        } else {
            $('[id*=trQuestion2AddlInfoRow_TEXTBOX]').hide();
            $('[id*=trQuestion2AddlInfoRow_DDL]').show();
        }
    });
    $('[id*=btnCarrierList]').click(function () {
        $('[id*=txtQ2PreviousCarrier]').val('');
        $('[id*=ddlQ2PreviousCarrier]').val('');
        $('[id*=trQuestion2AddlInfoRow_DDL]').show();
        $('[id*=trQuestion2AddlInfoRow_TEXTBOX]').hide();
    });

    // Handle Radio Button Clicks
    $('.questionRow input:radio').click(function(){
        var clickedDiamondCode = GetRadioDiamondCode($(this));
        var clickedChoice = GetRadioYesOrNo($(this));

        switch (clickedDiamondCode) {
            case 9532:
                //Q4
                //This one shows additional info on NO selection (opposite of default)
                if (clickedChoice == rdoChoice.yes) {
                    //clear out additional info
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').val('');
                    //hide
                    $('.descriptionRow.' + clickedDiamondCode).hide();
                    //disable
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').attr('disabled', 'disabled');
                }
                else {
                    //show
                    $('.descriptionRow.' + clickedDiamondCode).show();
                    //enable
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').removeAttr('disabled');
                }
                break;
            case 9540:
            case 9541:
            case 9548:
            case 9550:
            case 9555:
            case 9556:
            case 9558:
                //1, 10b, 10c, 12d, 12e, 13a, 15a, 16, 18 - do nothing
                break;
            case 9538:
                //Q10
                if (clickedChoice == rdoChoice.yes) {
                    //show and enable 10 additional info
                    $('.descriptionRow.' + clickedDiamondCode).show();
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').removeAttr('disabled');
                    //show 10a, 10b, 10c
                    $('.questionRow.9539').show();
                    $('.descriptionRow.9539').show();
                    $('.descriptionRow.9539 textarea').removeAttr('disabled');
                    $('.questionRow.9540').show();
                    $('.questionRow.9541').show();
                }
                else {
                    //hide, clear, disable 10 additional info
                    $('.descriptionRow.' + clickedDiamondCode).hide();
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').val('');
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').attr('disabled', 'disabled');
                    //hide 10a, 10b, 10c and clear out 10a textarea, 10b radio, 10c radio
                    $('.questionRow.9539').hide(); //10a
                    $('.descriptionRow.9539 textarea').val('');
                    $('.descriptionRow.9539').hide();
                    $('.descriptionRow.9539 textarea').attr('disabled', 'disabled');
                    $('.questionRow.9540 input:radio').prop("checked", false);
                    $('.questionRow.9540').hide(); //10b
                    $('.questionRow.9541 input:radio').prop("checked", false);
                    $('.questionRow.9541').hide(); //10c
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            case 9543:
                //Q12
                if (clickedChoice == rdoChoice.yes) {
                    //show 12a-12e
                    $('.questionRow.9544').show();
                    $('.descriptionRow.9544').show();
                    $('.questionRow.9545').show();
                    $('.descriptionRow.9545').show();
                    $('.questionRow.9546').show();
                    $('.descriptionRow.9546').show();
                    $('.questionRow.9547').show(); //drop down shows with yes selection
                    $('.questionRow.9548').show();
                    $('.descriptionRow.9548').hide(); //not using this description row
                    //enable 12a, 12c textareas, 12d drop down, keep 12e disabled
                    $('.descriptionRow.9544 textarea').removeAttr('disabled');
                    $('.descriptionRow.9545 select').removeAttr('disabled');
                    $('.descriptionRow.9546 textarea').removeAttr('disabled');
                    $('.descriptionRow.9547 select').removeAttr('disabled');
                    $('.descriptionRow.9548 textarea').attr('disabled', 'disabled'); //not using this description row
                }
                else {
                    //hide 12a-12e
                    $('.questionRow.9544').hide(); //12a
                    $('.descriptionRow.9544').hide();
                    $('.questionRow.9545').hide(); //12b
                    $('.descriptionRow.9545').hide();
                    $('.questionRow.9546').hide(); //12c
                    $('.descriptionRow.9546').hide();
                    $('.questionRow.9547').hide(); //12d
                    $('.descriptionRow.9547').hide();
                    $('.questionRow.9548').hide(); //12e
                    $('.descriptionRow.9548').hide(); //not using this description row
                    //clear out 12a, 12b, 12c, 12d, 12e values
                    $('.descriptionRow.9544 textarea').val('');
                    $('.descriptionRow.9545 select').val('');
                    $('.descriptionRow.9546 textarea').val('');
                    $('.questionRow.9547 input:radio').prop("checked", false);
                    $('.descriptionRow.9547 select').val('');
                    $('.questionRow.9548 input:radio').prop("checked", false);
                    $('.descriptionRow.9548 textarea').val('');
                    //disable 12a, 12b, 12c, 12e textareas, 12d drop down
                    $('.descriptionRow.9544 textarea').attr('disabled', 'disabled');
                    $('.descriptionRow.9545 select').attr('disabled', 'disabled');
                    $('.descriptionRow.9546 textarea').attr('disabled', 'disabled');
                    $('.descriptionRow.9547 select').attr('disabled', 'disabled');
                    $('.descriptionRow.9548 textarea').attr('disabled', 'disabled'); //not using this description row
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            case 9547:
                //Q12d
                if (clickedChoice == rdoChoice.yes) {
                    //show drop down - this might work as default
                    $('.descriptionRow.9547').show();
                    //enable ddl
                    $('.descriptionRow.' + clickedDiamondCode + ' select').removeAttr('disabled');
                }
                else {
                    //hide drop down
                    $('.descriptionRow.9547').hide();
                    //clear selection
                    $('.descriptionRow.' + clickedDiamondCode + ' select').val('');
                    //disable ddl
                    $('.descriptionRow.' + clickedDiamondCode + ' select').attr('disabled', 'disabled');
                }
                break;
            case 9549:
                //Q13
                //show 13a
                if (clickedChoice == rdoChoice.yes) {
                    //show 13a
                    $('.questionRow.9550').show();
                }
                else {
                    //hide 13a
                    $('.questionRow.9550').hide();
                    //clear out radio value
                    $('.questionRow.9550 input:radio').prop("checked", false);
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            case 9551:
                //Q14
                if (clickedChoice == rdoChoice.yes) {
                    //show 14a
                    $('.questionRow.9552').show();
                    $('.descriptionRow.9552').show();
                    //enable 14a textarea
                    $('.descriptionRow.9552 textarea').removeAttr('disabled');
                    //show 14b
                    $('.questionRow.9553').show(); //description shows with yes response
                }
                else {
                    //hide 14a
                    $('.questionRow.9552').hide();
                    $('.descriptionRow.9552').hide();
                    $('.descriptionRow.9552 textarea').val('');
                    $('.descriptionRow.9552 textarea').attr('disabled', 'disabled');
                    //hide 14b
                    $('.questionRow.9553').hide();
                    //clear out 14b radio value
                    $('.questionRow.9553 input:radio').prop("checked", false);
                    //hide 14b textarea
                    $('.descriptionRow.9553').hide();
                    //clear out 14b textarea
                    $('.descriptionRow.9553 textarea').val('');
                    //disable 14b textarea
                    $('.descriptionRow.9553 textarea').attr('disabled', 'disabled');
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            case 9553:
                //Q14b
                if (clickedChoice == rdoChoice.yes) {
                    //show 14b
                    $('.descriptionRow.9553').show();
                    //enable 14b textarea
                    $('.descriptionRow.9553 textarea').removeAttr('disabled');
                }
                else {
                    //hide 14b
                    $('.descriptionRow.9553').hide();
                    //clear out 14b textarea value
                    $('.descriptionRow.9553 textarea').val('');
                    //disable 14b textarea
                    $('.descriptionRow.9553 textarea').attr('disabled', 'disabled');
                }
                break;
            case 9554:
                //Q15
                if (clickedChoice == rdoChoice.yes) {
                    //show 15a
                    $('.questionRow.9555').show();
                }
                else {
                    //hide 15a
                    $('.questionRow.9555').hide();
                    //clear out 15a radio value
                    $('.questionRow.9555 input:radio').prop("checked", false);
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            case 9559:
                //Q19
                //show 19a
                if (clickedChoice == rdoChoice.yes) {
                    //show 19a
                    $('.questionRow.9560').show();
                }
                else {
                    //hide 19a
                    $('.questionRow.9560').hide();
                    //clear out 19a radio value
                    $('.questionRow.9560 input:radio').prop("checked", false);
                    //hide 19b
                    $('.questionRow.9561').hide();
                    $('.descriptionRow.9561').hide();
                    //clear out 19b textarea value
                    $('.descriptionRow.9561 textarea').val('');
                    //disable 19b textarea
                    $('.descriptionRow.9561 textarea').attr('disabled', 'disabled');
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            case 9560:
                //Q19a
                //show 19b
                if (clickedChoice == rdoChoice.no) {
                    //show 19b
                    $('.questionRow.9561').show();
                    $('.descriptionRow.9561').show();
                    //enable 19b textarea
                    $('.descriptionRow.9561 textarea').removeAttr('disabled');
                }
                else {
                    //hide 19b
                    $('.questionRow.9561').hide();
                    $('.descriptionRow.9561').hide();
                    //clear out 19b textarea value
                    $('.descriptionRow.9561 textarea').val('');
                    //disable 19b textarea
                    $('.descriptionRow.9561 textarea').attr('disabled', 'disabled');
                }
                ColorAlternateRows(); //since number of questions in the list changed, need to reformat the alternating row colors
                break;
            default:
                if (clickedChoice == rdoChoice.yes) {
                    //show additional info
                    $('.descriptionRow.' + clickedDiamondCode).show();
                    //enable additional info
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').removeAttr('disabled');
                }
                else {
                    //hide additional info
                    $('.descriptionRow.' + clickedDiamondCode).hide();
                    //clear textarea additional info
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').val('');
                    //disable textarea additional info
                    $('.descriptionRow.' + clickedDiamondCode + ' textarea').attr('disabled', 'disabled');
                }
                break;
        }
    });

});