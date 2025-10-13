
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
/// <reference path="vr.core.js" />



var VRStats = new function () {
    var mostRecentRawStats;
    var mostRecentMonthlyData;
    this.GetVr3Stats = function (divId, startDate, endDate, agencyId, lobsIds) {
        mostRecentRawStats = null;
        mostRecentMonthlyData = null;
        $("#" + divId).empty();
        $("#" + divId).append(CreateBoldText("Loading Data...."));
        $("#" + divId).append('<img src="images/loading.gif" />');

        ifm.vr.vrdata.QuotingStats.GetVrQuoteStats(agencyId, startDate, endDate, lobsIds, function (data) {
            $("#" + divId).empty();
            mostRecentRawStats = data;
            GetMonthData(data);
            Populate(divId, mostRecentRawStats);
        });
    };

    function CreateBoldText(text) {
        return "<b>" + text + "</b>";
    }

    function AddInfoIcon(hoverText) {
        return '<img src="images/infoIcon.png" style="width:15px;height:15px;" title="' + hoverText + '" />';
    }

    function Populate(divId, statData) {
        if (statData != null) {
            var html = "<table>";

            html += "<tr>";
            html += "<td style='width: 220px;'>";
            html += CreateBoldText("Start Date: ") + CreateDateFromJsonDate(statData.StartDate).formatMMDDYYYY();
            html += "</td>";
            html += "<td>";
            html += CreateBoldText("End Date: ") + CreateDateFromJsonDate(statData.EndDate).formatMMDDYYYY();
            html += "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td colspan='2'>";
            html += "</td>";

            html += "</tr>";

            html += "<tr>";
            html += "<td colspan='2'>";
            html += CreateBoldText("Quotes Found: ") + statData.Stats.NewQuoteCount.toString();
            html += "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td>";
            html += CreateBoldText("Total Premium: ") + FormatAsCurrency(statData.Stats.TotalQuotedPremium.toString());
            html += AddInfoIcon("This can be inflated by copied quotes.");
            html += "</td>";
            html += "<td>";
            html += CreateBoldText("Finalized Premium: ") + FormatAsCurrency(statData.Stats.TotalFinalizedPremium.toString());
            html += "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td>";
            html += CreateBoldText("Total Clients: ") + statData.Stats.ClientCount.toString();
            html += "</td>";
            html += "<td>";
            html += CreateBoldText("Unique Clients: ") + statData.Stats.UniqueClientNamesCount.toString();
            html += AddInfoIcon("This is not unique client ids but simply unique Names.");
            html += "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td colspan='2'>";
            html += CreateBoldText("Avg Days from Start to Finalized: ") + statData.Stats.DaysFromStartToFinalized_Avg.toFixed(4).toString();
            html += "</td>";
            html += "</tr>";

            html += "</table>";

            var firstGraphRow = GetRandomChartId();
            html += "<div id='" + firstGraphRow + "' style='vertical-align:top;'></div>";

            var secondGraphRow = GetRandomChartId();
            html += "<div id='" + secondGraphRow + "' style='vertical-align:top;'></div>";

            $("#" + divId).append(html);

            QuoteStatus_Section(firstGraphRow, statData.Stats);
            QuoteByLob_TopLevelSection(firstGraphRow, statData.Stats);

            QuoteByLob_Section(secondGraphRow, statData.Stats);

            var data = {
                labels: ["January", "February", "March", "April", "May", "June", "July"],
                datasets: [
                    {
                        label: "My First dataset",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgba(220,220,220,1)",
                        pointColor: "rgba(220,220,220,1)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [65, 59, 80, 81, 56, 55, 40]
                    },
                    {
                        label: "My Second dataset",
                        fillColor: "rgba(151,187,205,0.2)",
                        strokeColor: "rgba(151,187,205,1)",
                        pointColor: "rgba(151,187,205,1)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(151,187,205,1)",
                        data: [28, 48, 40, 19, 86, 27, 90]
                    }
                ]
            };

            //CreateLineGraph(divId, data, 500, 400);
        }
    }

    function ConvertMonthIntToMonth(monthInt) {
        switch (monthInt) {
            case 0:
                return "January";
                break;
            case 1:
                return "February";
                break;
            case 2:
                return "March";
                break;
            case 3:
                return "April";
                break;
            case 4:
                return "May";
                break;
            case 5:
                return "June";
                break;
            case 6:
                return "July";
                break;
            case 7:
                return "August";
                break;
            case 8:
                return "September";
                break;
            case 9:
                return "October";
                break;
            case 10:
                return "November";
                break;
            case 11:
                return "December";
                break;
            default:
                return "Invalid Month";
        }
    }

    function listContains(list, objectToFind) {
        for (var i = 0; i < list.length; i++) {
            if (list[i] === objectToFind | list[i] == objectToFind) {
                return true;
            }
        }

        return false;
    }

    function CreateDateFromJsonDate(dateFromService) {
        var regxformatdate = /-?\d+/;
        var integerformat = regxformatdate.exec(dateFromService);
        return new Date(parseInt(integerformat));
    }

    Date.prototype.formatMMDDYYYY = function () {
        return (this.getMonth() + 1) +
        "/" + this.getDate() +
        "/" + this.getFullYear();
    }

    function GraphDataSet(keys, dataPoints) {
        this.DataSetKey = keys;
        this.DataPoint = dataPoints;
    }

    function CreateLineGraph(divId, data, width, height) {
        var chartId = GetRandomChartId();
        $("#" + divId).append(' <canvas id="' + chartId + '" width="' + width + '" height="' + height + '"></canvas>');
        var ctx = document.getElementById(chartId).getContext("2d");

        var options = {
            ///Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: true,

            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",

            //Number - Width of the grid lines
            scaleGridLineWidth: 1,

            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,

            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,

            //Boolean - Whether the line is curved between points
            bezierCurve: true,

            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.4,

            //Boolean - Whether to show a dot for each point
            pointDot: true,

            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,

            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,

            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,

            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,

            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,

            //Boolean - Whether to fill the dataset with a colour
            datasetFill: true,

            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].strokeColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
        };

        var myLineChart = new Chart(ctx).Line(data, options);
    }

    function CreatePieGraph(divId, data, width, height) {
        var chartId = GetRandomChartId();
        $("#" + divId).append(' <canvas id="' + chartId + '" width="' + width + '" height="' + height + '" style="margin-top:10px;"></canvas>');
        var ctx = document.getElementById(chartId).getContext("2d");

        var options = {
            //Boolean - Whether we should show a stroke on each segment
            segmentShowStroke: true,

            //String - The colour of each segment stroke
            segmentStrokeColor: "#fff",

            //Number - The width of each segment stroke
            segmentStrokeWidth: 2,

            //Number - The percentage of the chart that we cut out of the middle
            percentageInnerCutout: 50, // This is 0 for Pie charts

            //Number - Amount of animation steps
            animationSteps: 100,

            //String - Animation easing effect
            animationEasing: "easeOutBounce",

            //Boolean - Whether we animate the rotation of the Doughnut
            animateRotate: true,

            //Boolean - Whether we animate scaling the Doughnut from the centre
            animateScale: false,

            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
        };

        var myPieChart = new Chart(ctx).Pie(data, options);
    }

    function CreateBarGraph(divId, data, width, height) {
        var chartId = GetRandomChartId();
        $("#" + divId).append(' <canvas id="' + chartId + '" width="' + width + '" height="' + height + '"></canvas>');
        var ctx = document.getElementById(chartId).getContext("2d");

        var options = {
            //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
            scaleBeginAtZero: true,

            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: true,

            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",

            //Number - Width of the grid lines
            scaleGridLineWidth: 1,

            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,

            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,

            //Boolean - If there is a stroke on each bar
            barShowStroke: true,

            //Number - Pixel width of the bar stroke
            barStrokeWidth: 2,

            //Number - Spacing between each of the X value sets
            barValueSpacing: 5,

            //Number - Spacing between data sets within X values
            barDatasetSpacing: 1,

            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
        };

        var myBarChart = new Chart(ctx).Bar(data, options);
    }

    function GetRandomColor()
    { return '#' + Math.random().toString(16).substr(-6); }

    function GetHighlightedColorOfColor_HexString(colorasHex) {
        var color = new RGBColor(colorasHex);
        if (color.r > 125) {
            color.r += 20;
            if (color.r > 255)
                color.r = 255;
        }
        else {
            color.r -= 20;
            if (color.r < 0)
                color.r = 0;
        }

        return color.toHex();
    }

    function GetRandomChartId()
    { return 'chart_' + Date.now().toString(); }

    function GetRandomDivId()
    { return 'div_' + Date.now().toString(); }

    function QuoteStatus_Section(divId, statData) {
        var mySectionDivId = GetRandomDivId();
        var html = "<div id='" + mySectionDivId + "' class='standardStatSection' style=' text-align: center;width: 250px; '>";
        html += "<span class='standardStatSectionHeader'>Quote Count by Status</span>";
        html += "<table style='width:200px; text-align: left; margin-top: 10px;'>";
        html += "<tr>";
        html += "<td>";
        html += CreateBoldText("Quote: ") + statData.QuoteCountInQuoteStatus.toString();
        html += "</td>";
        html += "<td>";
        html += CreateBoldText("Applications: ") + statData.QuoteCountInAppStatus.toString();
        html += "</td>";
        html += "</tr>";

        html += "<tr>";
        html += "<td>";
        html += CreateBoldText("Finalized: ") + statData.QuoteCountInFinalizedStatus.toString();
        html += "</td>";
        html += "<td>";
        html += CreateBoldText("Routed to UW: ") + statData.QuoteCountInRoutedStatus.toString();
        html += "</td>";
        html += "</tr>";
        html += "</table>";
        html += "</div>";
        $("#" + divId).append(html);

        var rndColor = new RGBColor(GetRandomColor()).toHex();

        var data = [
        {
            value: statData.QuoteCountInQuoteStatus,
            color: rndColor = rndColor = new RGBColor(GetRandomColor()).toHex(),
            highlight: GetHighlightedColorOfColor_HexString(rndColor),
            label: "Quotes " + statData.PercentOfQuotesAsQuote.toString() + "%"
        },
        {
            value: statData.QuoteCountInAppStatus,
            color: rndColor = rndColor = new RGBColor(GetRandomColor()).toHex(),
            highlight: GetHighlightedColorOfColor_HexString(rndColor),
            label: "Applications " + statData.PercentOfQuotesAsApp.toString() + "%"
        },
        {
            value: statData.QuoteCountInRoutedStatus,
            color: rndColor = rndColor = new RGBColor(GetRandomColor()).toHex(),
            highlight: GetHighlightedColorOfColor_HexString(rndColor),
            label: "Routed to UW " + statData.PercentOfQuotesRoutedToUW.toString() + "%"
        }
        ,
        {
            value: statData.QuoteCountInFinalizedStatus,
            color: rndColor = rndColor = new RGBColor(GetRandomColor()).toHex(),
            highlight: GetHighlightedColorOfColor_HexString(rndColor),
            label: "Finalized " + statData.PercentOfQuotesFinalized.toString() + "%"
        }
        ]

        CreatePieGraph(mySectionDivId, data, "200px", "200px");
    }

    function QuoteByLob_TopLevelSection(divId, statData) {
        var mySectionDivId = GetRandomDivId();

        //statData.StatsByLob.Key

        var html = "<div id='" + mySectionDivId + "' class='standardStatSection' style=' text-align: center;width: 250px; '>";
        html += "<span class='standardStatSectionHeader'>Quote Count by Lob</span>";
        html += "<div style='margin-top: 10px;'>";
        for (var gg = 0; gg < statData.StatsByLob.length; gg++) {
            html += CreateBoldText(statData.QuoteCount_ByLob[gg].Key.toString() + " : ") + statData.QuoteCount_ByLob[gg].Value.toString() + "<br />";
        }
        html += "</div>";

        html += "</div>";
        $("#" + divId).append(html);

        var rndColor = new RGBColor(GetRandomColor()).toHex();

        var data = new Array();

        for (var gg = 0; gg < statData.StatsByLob.length; gg++) {
            var item = new Object();
            item.value = statData.QuoteCount_ByLob[gg].Value;
            var rndColor = new RGBColor(GetRandomColor()).toHex();
            item.color = rndColor
            item.highlight = GetHighlightedColorOfColor_HexString(rndColor);
            item.label = statData.QuoteCount_ByLob[gg].Key;
            data.push(item);
        }

        CreatePieGraph(mySectionDivId, data, "200px", "200px");
    }

    function QuoteByLob_Section(divId, statData) {
        var data_TotalPremium = new Array();
        var data_FinalizedPremium = new Array();

        for (var hh = 0; hh < statData.StatsByLob.length; hh++) {
            var statOfLob = statData.StatsByLob[hh].Stats;
            var lobName = statOfLob.QuoteCount_ByLob[0].Key;

            var TotalPremium = new Object();
            TotalPremium.label = lobName;
            TotalPremium.value = parseInt(statOfLob.TotalQuotedPremium);
            var rndColor = new RGBColor(GetRandomColor()).toHex();
            TotalPremium.color = rndColor
            TotalPremium.highlight = GetHighlightedColorOfColor_HexString(rndColor);
            data_TotalPremium.push(TotalPremium);

            var FinalizedPremium = new Object();
            FinalizedPremium.label = lobName;
            FinalizedPremium.value = parseInt(statOfLob.TotalFinalizedPremium);
            var rndColor = new RGBColor(GetRandomColor()).toHex();
            FinalizedPremium.color = rndColor
            FinalizedPremium.highlight = GetHighlightedColorOfColor_HexString(rndColor);
            data_FinalizedPremium.push(FinalizedPremium);
        }

        { // for scope control
            var mySectionDivId = GetRandomDivId();
            var html = "<div id='" + mySectionDivId + "' class='standardStatSection' style=' text-align: center;width: 250px; '>";
            html += "<span class='standardStatSectionHeader'>Quoted Premium by Lob</span>";
            html += AddInfoIcon("These values can be inflated by copied quotes.");
            html += "<div style='margin-top: 10px;'>";
            for (var gg = 0; gg < data_TotalPremium.length; gg++) {
                html += CreateBoldText(data_TotalPremium[gg].label + " : ") + FormatAsCurrency(data_TotalPremium[gg].value.toString()) + "<br />";
            }
            html += "</div>";

            html += "</div>";
            $("#" + divId).append(html);

            CreatePieGraph(mySectionDivId, data_TotalPremium, "200px", "200px");
        } // for scope control

        { // for scope control
            var mySectionDivId = GetRandomDivId();
            var html = "<div id='" + mySectionDivId + "' class='standardStatSection' style=' text-align: center;width: 250px; '>";
            html += "<span class='standardStatSectionHeader'>Finalized Premium by Lob</span>";
            html += "<div style='margin-top: 10px;'>";
            for (var gg = 0; gg < data_FinalizedPremium.length; gg++) {
                html += CreateBoldText(data_FinalizedPremium[gg].label + " : ") + FormatAsCurrency(data_FinalizedPremium[gg].value.toString()) + "<br />";
            }
            html += "</div>";

            html += "</div>";
            $("#" + divId).append(html);

            CreatePieGraph(mySectionDivId, data_FinalizedPremium, "200px", "200px");
        } // for scope control
    }

    function GetMonthData(topLevelData) {
        var monthlyData = new Array();
        for (var aa = 0; aa < topLevelData.SubSlices.length; aa++) {
            var subSlice1 = topLevelData.SubSlices[aa];
            for (var bb = 0; bb < subSlice1.SubSlices.length; bb++) {
                var monthlySlice = subSlice1.SubSlices[bb];
                monthlyData.push(monthlySlice);
            }
        }
        monthlyData.reverse(); // order oldest to newest
        mostRecentMonthlyData = monthlyData;
    }

    /**
     * A class to parse color values
     * @author Stoyan Stefanov <sstoo@gmail.com>
     * @link   http://www.phpied.com/rgb-color-parser-in-javascript/
     * @license Use it if you like it
     */
    function RGBColor(color_string) {
        this.ok = false;

        // strip any leading #
        if (color_string.charAt(0) == '#') { // remove # if any
            color_string = color_string.substr(1, 6);
        }

        color_string = color_string.replace(/ /g, '');
        color_string = color_string.toLowerCase();

        // before getting into regexps, try simple matches
        // and overwrite the input
        var simple_colors = {
            aliceblue: 'f0f8ff',
            antiquewhite: 'faebd7',
            aqua: '00ffff',
            aquamarine: '7fffd4',
            azure: 'f0ffff',
            beige: 'f5f5dc',
            bisque: 'ffe4c4',
            black: '000000',
            blanchedalmond: 'ffebcd',
            blue: '0000ff',
            blueviolet: '8a2be2',
            brown: 'a52a2a',
            burlywood: 'deb887',
            cadetblue: '5f9ea0',
            chartreuse: '7fff00',
            chocolate: 'd2691e',
            coral: 'ff7f50',
            cornflowerblue: '6495ed',
            cornsilk: 'fff8dc',
            crimson: 'dc143c',
            cyan: '00ffff',
            darkblue: '00008b',
            darkcyan: '008b8b',
            darkgoldenrod: 'b8860b',
            darkgray: 'a9a9a9',
            darkgreen: '006400',
            darkkhaki: 'bdb76b',
            darkmagenta: '8b008b',
            darkolivegreen: '556b2f',
            darkorange: 'ff8c00',
            darkorchid: '9932cc',
            darkred: '8b0000',
            darksalmon: 'e9967a',
            darkseagreen: '8fbc8f',
            darkslateblue: '483d8b',
            darkslategray: '2f4f4f',
            darkturquoise: '00ced1',
            darkviolet: '9400d3',
            deeppink: 'ff1493',
            deepskyblue: '00bfff',
            dimgray: '696969',
            dodgerblue: '1e90ff',
            feldspar: 'd19275',
            firebrick: 'b22222',
            floralwhite: 'fffaf0',
            forestgreen: '228b22',
            fuchsia: 'ff00ff',
            gainsboro: 'dcdcdc',
            ghostwhite: 'f8f8ff',
            gold: 'ffd700',
            goldenrod: 'daa520',
            gray: '808080',
            green: '008000',
            greenyellow: 'adff2f',
            honeydew: 'f0fff0',
            hotpink: 'ff69b4',
            indianred: 'cd5c5c',
            indigo: '4b0082',
            ivory: 'fffff0',
            khaki: 'f0e68c',
            lavender: 'e6e6fa',
            lavenderblush: 'fff0f5',
            lawngreen: '7cfc00',
            lemonchiffon: 'fffacd',
            lightblue: 'add8e6',
            lightcoral: 'f08080',
            lightcyan: 'e0ffff',
            lightgoldenrodyellow: 'fafad2',
            lightgrey: 'd3d3d3',
            lightgreen: '90ee90',
            lightpink: 'ffb6c1',
            lightsalmon: 'ffa07a',
            lightseagreen: '20b2aa',
            lightskyblue: '87cefa',
            lightslateblue: '8470ff',
            lightslategray: '778899',
            lightsteelblue: 'b0c4de',
            lightyellow: 'ffffe0',
            lime: '00ff00',
            limegreen: '32cd32',
            linen: 'faf0e6',
            magenta: 'ff00ff',
            maroon: '800000',
            mediumaquamarine: '66cdaa',
            mediumblue: '0000cd',
            mediumorchid: 'ba55d3',
            mediumpurple: '9370d8',
            mediumseagreen: '3cb371',
            mediumslateblue: '7b68ee',
            mediumspringgreen: '00fa9a',
            mediumturquoise: '48d1cc',
            mediumvioletred: 'c71585',
            midnightblue: '191970',
            mintcream: 'f5fffa',
            mistyrose: 'ffe4e1',
            moccasin: 'ffe4b5',
            navajowhite: 'ffdead',
            navy: '000080',
            oldlace: 'fdf5e6',
            olive: '808000',
            olivedrab: '6b8e23',
            orange: 'ffa500',
            orangered: 'ff4500',
            orchid: 'da70d6',
            palegoldenrod: 'eee8aa',
            palegreen: '98fb98',
            paleturquoise: 'afeeee',
            palevioletred: 'd87093',
            papayawhip: 'ffefd5',
            peachpuff: 'ffdab9',
            peru: 'cd853f',
            pink: 'ffc0cb',
            plum: 'dda0dd',
            powderblue: 'b0e0e6',
            purple: '800080',
            red: 'ff0000',
            rosybrown: 'bc8f8f',
            royalblue: '4169e1',
            saddlebrown: '8b4513',
            salmon: 'fa8072',
            sandybrown: 'f4a460',
            seagreen: '2e8b57',
            seashell: 'fff5ee',
            sienna: 'a0522d',
            silver: 'c0c0c0',
            skyblue: '87ceeb',
            slateblue: '6a5acd',
            slategray: '708090',
            snow: 'fffafa',
            springgreen: '00ff7f',
            steelblue: '4682b4',
            tan: 'd2b48c',
            teal: '008080',
            thistle: 'd8bfd8',
            tomato: 'ff6347',
            turquoise: '40e0d0',
            violet: 'ee82ee',
            violetred: 'd02090',
            wheat: 'f5deb3',
            white: 'ffffff',
            whitesmoke: 'f5f5f5',
            yellow: 'ffff00',
            yellowgreen: '9acd32'
        };
        for (var key in simple_colors) {
            if (color_string == key) {
                color_string = simple_colors[key];
            }
        }
        // emd of simple type-in colors

        // array of color definition objects
        var color_defs = [
            {
                re: /^rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$/,
                example: ['rgb(123, 234, 45)', 'rgb(255,234,245)'],
                process: function (bits) {
                    return [
                        parseInt(bits[1]),
                        parseInt(bits[2]),
                        parseInt(bits[3])
                    ];
                }
            },
            {
                re: /^(\w{2})(\w{2})(\w{2})$/,
                example: ['#00ff00', '336699'],
                process: function (bits) {
                    return [
                        parseInt(bits[1], 16),
                        parseInt(bits[2], 16),
                        parseInt(bits[3], 16)
                    ];
                }
            },
            {
                re: /^(\w{1})(\w{1})(\w{1})$/,
                example: ['#fb0', 'f0f'],
                process: function (bits) {
                    return [
                        parseInt(bits[1] + bits[1], 16),
                        parseInt(bits[2] + bits[2], 16),
                        parseInt(bits[3] + bits[3], 16)
                    ];
                }
            }
        ];

        // search through the definitions to find a match
        for (var i = 0; i < color_defs.length; i++) {
            var re = color_defs[i].re;
            var processor = color_defs[i].process;
            var bits = re.exec(color_string);
            if (bits) {
                channels = processor(bits);
                this.r = channels[0];
                this.g = channels[1];
                this.b = channels[2];
                this.ok = true;
            }
        }

        // validate/cleanup values
        this.r = (this.r < 0 || isNaN(this.r)) ? 0 : ((this.r > 255) ? 255 : this.r);
        this.g = (this.g < 0 || isNaN(this.g)) ? 0 : ((this.g > 255) ? 255 : this.g);
        this.b = (this.b < 0 || isNaN(this.b)) ? 0 : ((this.b > 255) ? 255 : this.b);

        // some getters
        this.toRGB = function () {
            return 'rgb(' + this.r + ', ' + this.g + ', ' + this.b + ')';
        }
        this.toHex = function () {
            var r = this.r.toString(16);
            var g = this.g.toString(16);
            var b = this.b.toString(16);
            if (r.length == 1) r = '0' + r;
            if (g.length == 1) g = '0' + g;
            if (b.length == 1) b = '0' + b;
            return '#' + r + g + b;
        }

        // help
        this.getHelpXML = function () {
            var examples = new Array();
            // add regexps
            for (var i = 0; i < color_defs.length; i++) {
                var example = color_defs[i].example;
                for (var j = 0; j < example.length; j++) {
                    examples[examples.length] = example[j];
                }
            }
            // add type-in colors
            for (var sc in simple_colors) {
                examples[examples.length] = sc;
            }

            var xml = document.createElement('ul');
            xml.setAttribute('id', 'rgbcolor-examples');
            for (var i = 0; i < examples.length; i++) {
                try {
                    var list_item = document.createElement('li');
                    var list_color = new RGBColor(examples[i]);
                    var example_div = document.createElement('div');
                    example_div.style.cssText =
                            'margin: 3px; '
                            + 'border: 1px solid black; '
                            + 'background:' + list_color.toHex() + '; '
                            + 'color:' + list_color.toHex()
                    ;
                    example_div.appendChild(document.createTextNode('test'));
                    var list_item_value = document.createTextNode(
                        ' ' + examples[i] + ' -> ' + list_color.toRGB() + ' -> ' + list_color.toHex()
                    );
                    list_item.appendChild(example_div);
                    list_item.appendChild(list_item_value);
                    xml.appendChild(list_item);
                } catch (e) { }
            }
            return xml;
        }
    }

};

