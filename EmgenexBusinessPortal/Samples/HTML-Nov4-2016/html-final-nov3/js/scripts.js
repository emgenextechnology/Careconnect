/* ----- SALES CHART STARTS ----- */
$(function() {
    $("#doughnutChart").drawDoughnutChart([{
        title: "Pharmacogenetics",
        value: 200,
        color: "#85b662"
    }, {
        title: "Toxicology",
        value: 182,
        color: "#fdd400"
    }, {
        title: "Compounding",
        value: 70,
        color: "#f4a81e"
    }, {
        title: "Other",
        value: 500,
        color: "#1e458c"
    }, ]);
});
/*!
 * jquery.drawDoughnutChart.js
 * Version: 0.4.1(Beta)
 * Inspired by Chart.js(http://www.chartjs.org/)
 *
 * Copyright 2014 hiro
 * https://github.com/githiro/drawDoughnutChart
 * Released under the MIT license.
 * 
 */
;
(function($, undefined) {
    $.fn.drawDoughnutChart = function(data, options) {
        var $this = this,
            W = $this.width(),
            H = $this.height(),
            centerX = W / 2,
            centerY = H / 2,
            cos = Math.cos,
            sin = Math.sin,
            PI = Math.PI,
            settings = $.extend({
                edgeOffset: 10, //offset from edge of $this
                percentageInnerCutout: 70,
                animation: true,
                animationSteps: 90,
                animationEasing: "easeInOutExpo",
                animateRotate: true,
                tipOffsetX: -8,
                tipOffsetY: -45,
                tipClass: "doughnutTip",
                summaryClass: "doughnutSummary",
                summaryTitle: "SALES",
                summaryTitleClass: "doughnutSummaryTitle",
                summaryNumberClass: "doughnutSummaryNumber",
                beforeDraw: function() {},
                afterDrawed: function() {},
                onPathEnter: function(e, data) {},
                onPathLeave: function(e, data) {}
            }, options),
            animationOptions = {
                linear: function(t) {
                    return t;
                },
                easeInOutExpo: function(t) {
                    var v = t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
                    return (v > 1) ? 1 : v;
                }
            },
            requestAnimFrame = function() {
                return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function(callback) {
                    window.setTimeout(callback, 1000 / 60);
                };
            }();
        settings.beforeDraw.call($this);
        var $svg = $('<svg width="' + W + '" height="' + H + '" viewBox="0 0 ' + W + ' ' + H + '" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"></svg>').appendTo($this),
            $paths = [],
            easingFunction = animationOptions[settings.animationEasing],
            doughnutRadius = Min([H / 2, W / 2]) - settings.edgeOffset,
            cutoutRadius = doughnutRadius * (settings.percentageInnerCutout / 100),
            segmentTotal = 0;
        //Draw base doughnut
        var baseDoughnutRadius = doughnutRadius + settings.baseOffset,
            baseCutoutRadius = cutoutRadius - settings.baseOffset;
        $(document.createElementNS('http://www.w3.org/2000/svg', 'path')).attr({
            "d": getHollowCirclePath(baseDoughnutRadius, baseCutoutRadius),
            "fill": settings.baseColor
        }).appendTo($svg);
        //Set up pie segments wrapper
        var $pathGroup = $(document.createElementNS('http://www.w3.org/2000/svg', 'g'));
        $pathGroup.attr({
            opacity: 0
        }).appendTo($svg);
        //Set up tooltip
        var $tip = $('<div class="' + settings.tipClass + '" />').appendTo('body').hide(),
            tipW = $tip.width(),
            tipH = $tip.height();
        //Set up center text area
        var summarySize = (cutoutRadius - (doughnutRadius - cutoutRadius)) * 2,
            $summary = $('<div class="' + settings.summaryClass + '" />').appendTo($this).css({
                width: summarySize + "px",
                height: summarySize + "px",
                "margin-left": -(summarySize / 2) + "px",
                "margin-top": -(summarySize / 2) + "px"
            });
        var $summaryNumber = $('<p class="' + settings.summaryNumberClass + '"></p>').appendTo($summary).css({
            opacity: 0
        });
        var $summaryTitle = $('<p class="' + settings.summaryTitleClass + '">' + settings.summaryTitle + '</p>').appendTo($summary);
        for (var i = 0, len = data.length; i < len; i++) {
            segmentTotal += data[i].value;
            $paths[i] = $(document.createElementNS('http://www.w3.org/2000/svg', 'path')).attr({
                "stroke-width": settings.segmentStrokeWidth,
                "stroke": settings.segmentStrokeColor,
                "fill": data[i].color,
                "data-order": i
            }).appendTo($pathGroup).on("mouseenter", pathMouseEnter).on("mouseleave", pathMouseLeave).on("mousemove", pathMouseMove);
        }
        //Animation start
        animationLoop(drawPieSegments);
        //Functions
        function getHollowCirclePath(doughnutRadius, cutoutRadius) {
            //Calculate values for the path.
            //We needn't calculate startRadius, segmentAngle and endRadius, because base doughnut doesn't animate.
            var startRadius = -1.570, // -Math.PI/2
                segmentAngle = 6.2831, // 1 * ((99.9999/100) * (PI*2)),
                endRadius = 4.7131, // startRadius + segmentAngle
                startX = centerX + cos(startRadius) * doughnutRadius,
                startY = centerY + sin(startRadius) * doughnutRadius,
                endX2 = centerX + cos(startRadius) * cutoutRadius,
                endY2 = centerY + sin(startRadius) * cutoutRadius,
                endX = centerX + cos(endRadius) * doughnutRadius,
                endY = centerY + sin(endRadius) * doughnutRadius,
                startX2 = centerX + cos(endRadius) * cutoutRadius,
                startY2 = centerY + sin(endRadius) * cutoutRadius;
            var cmd = ['M', startX, startY, 'A', doughnutRadius, doughnutRadius, 0, 1, 1, endX, endY, //Draw outer circle
                'Z', //Close path
                'M', startX2, startY2, //Move pointer
                'A', cutoutRadius, cutoutRadius, 0, 1, 0, endX2, endY2, //Draw inner circle
                'Z'
            ];
            cmd = cmd.join(' ');
            return cmd;
        };

        function pathMouseEnter(e) {
            var order = $(this).data().order;
            $tip.text(data[order].title + ": " + data[order].value).fadeIn(200);
            settings.onPathEnter.apply($(this), [e, data]);
        }

        function pathMouseLeave(e) {
            $tip.hide();
            settings.onPathLeave.apply($(this), [e, data]);
        }

        function pathMouseMove(e) {
            $tip.css({
                top: e.pageY + settings.tipOffsetY,
                left: e.pageX - $tip.width() / 2 + settings.tipOffsetX
            });
        }

        function drawPieSegments(animationDecimal) {
            var startRadius = -PI / 2, //-90 degree
                rotateAnimation = 1;
            if (settings.animation && settings.animateRotate) rotateAnimation = animationDecimal; //count up between0~1
            drawDoughnutText(animationDecimal, segmentTotal);
            $pathGroup.attr("opacity", animationDecimal);
            //If data have only one value, we draw hollow circle(#1).
            if (data.length === 1 && (4.7122 < (rotateAnimation * ((data[0].value / segmentTotal) * (PI * 2)) + startRadius))) {
                $paths[0].attr("d", getHollowCirclePath(doughnutRadius, cutoutRadius));
                return;
            }
            for (var i = 0, len = data.length; i < len; i++) {
                var segmentAngle = rotateAnimation * ((data[i].value / segmentTotal) * (PI * 2)),
                    endRadius = startRadius + segmentAngle,
                    largeArc = ((endRadius - startRadius) % (PI * 2)) > PI ? 1 : 0,
                    startX = centerX + cos(startRadius) * doughnutRadius,
                    startY = centerY + sin(startRadius) * doughnutRadius,
                    endX2 = centerX + cos(startRadius) * cutoutRadius,
                    endY2 = centerY + sin(startRadius) * cutoutRadius,
                    endX = centerX + cos(endRadius) * doughnutRadius,
                    endY = centerY + sin(endRadius) * doughnutRadius,
                    startX2 = centerX + cos(endRadius) * cutoutRadius,
                    startY2 = centerY + sin(endRadius) * cutoutRadius;
                var cmd = ['M', startX, startY, //Move pointer
                    'A', doughnutRadius, doughnutRadius, 0, largeArc, 1, endX, endY, //Draw outer arc path
                    'L', startX2, startY2, //Draw line path(this line connects outer and innner arc paths)
                    'A', cutoutRadius, cutoutRadius, 0, largeArc, 0, endX2, endY2, //Draw inner arc path
                    'Z' //Cloth path
                ];
                $paths[i].attr("d", cmd.join(' '));
                startRadius += segmentAngle;
            }
        }

        function drawDoughnutText(animationDecimal, segmentTotal) {
            $summaryNumber.css({
                opacity: animationDecimal
            }).text((segmentTotal * animationDecimal).toFixed(1));
        }

        function animateFrame(cnt, drawData) {
            var easeAdjustedAnimationPercent = (settings.animation) ? CapValue(easingFunction(cnt), null, 0) : 1;
            drawData(easeAdjustedAnimationPercent);
        }

        function animationLoop(drawData) {
            var animFrameAmount = (settings.animation) ? 1 / CapValue(settings.animationSteps, Number.MAX_VALUE, 1) : 1,
                cnt = (settings.animation) ? 0 : 1;
            requestAnimFrame(function() {
                cnt += animFrameAmount;
                animateFrame(cnt, drawData);
                if (cnt <= 1) {
                    requestAnimFrame(arguments.callee);
                } else {
                    settings.afterDrawed.call($this);
                }
            });
        }

        function Max(arr) {
            return Math.max.apply(null, arr);
        }

        function Min(arr) {
            return Math.min.apply(null, arr);
        }

        function isNumber(n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }

        function CapValue(valueToCap, maxValue, minValue) {
            if (isNumber(maxValue) && valueToCap > maxValue) return maxValue;
            if (isNumber(minValue) && valueToCap < minValue) return minValue;
            return valueToCap;
        }
        return $this;
    };
})(jQuery);
/* ----- SALES CHART ENDS ----- */
/* ----- TOOLTIP STARTS ----- */
$(document).ready(function() {
    $('[data-toggle="tooltip"]').tooltip();
});
/* ----- TOOLTIP ENDS ----- */
/* ----- DATA TABLE STARTS ----- */
$(document).ready(function() {
    $('#leadtable').DataTable({
        paging: false,
        searching: false,
        info: false
    });
});
/* ----- DATA TABLE ENDS ----- */
/* ----- HEIGHT 100% OF LEADS STARTS ----- */
$(document).ready(function() {
    if ($(window).width() > 1024) {
        var hgt = $(window).innerHeight() - ($('header').outerHeight() + $('.lead-top-search').outerHeight() + 10) + 'px';
        $('.filter-lead').css('min-height', hgt);
        $('.lead-info-wrap').css('min-height', hgt);
        $('.listbox-lead').css('min-height', hgt);
        $('.task-detail-right').css('height', hgt);
        $('.market-table').css('min-height', hgt);
        $('.task-detail-left').css('height', hgt);  
             
    }
});
$(document).ready(function() {
    if ($(window).width() > 767) {
        var hgt = $(window).innerHeight() - ($('header').outerHeight() + $('.lead-top-search').outerHeight() + 10) + 'px';

        $('.task-detail-right').css('height', hgt);

        $('.task-detail-left').css('height', hgt);  
             
    }
});
/* ----- HEIGHT 100% OF LEADS ENDS ----- */
/* ----- TABLE RESPONSIVE IPAD STARTS ----- */
/*$(document).ready(function() {
if($(window).width()<1024){
$('.table').addClass('table-responsive').removeClass('table');
}
});*/
/* ----- TABLE RESPONSIVE IPAD ENDS ----- */
/* ----- HIDING LEADS STARS ----- */
$(document).ready(function() {
    $('.lead-name-lnk').on("click", function(e) {
        $('.lead-name-lnk').parent().removeClass('activelink');
        $(this).parent().addClass('activelink');
        $('#lead-list-wrap').addClass('lead-list-wrap-white');
        if ($(window).width() == 768) {
            $('#listbox-lead').hide();
        } else {
            if ($("#listbox-lead").hasClass("col-lg-12")) {
                $('#listbox-lead').addClass('listbox-hide').removeClass('col-lg-12').addClass('col-lg-3');
            } else {
                $('#listbox-lead').addClass('listbox-hide').removeClass('col-lg-9').addClass('col-lg-3');
                $('#filter-toggle-btn').removeClass("filter-disabled");
            }
        }
        $('.paging-wrap').css("display", "none");
        $('#filter-lead').removeClass('filter-lead-show').addClass('filter-lead-hide');
        $('#lead-info-wrap').css("display", 'block');
    });
    //for task page
    $('.task-name-lnk').on("click", function(e) {
        $('.task-name-lnk').parent().removeClass('activelink');
        $(this).parent().addClass('activelink');
        $('#lead-list-wrap').addClass('lead-list-wrap-white');
        /*if ($("#task-info-wrap").hasClass("col-lg-12")) {

        $('#task-info-wrap').removeClass('col-lg-12');




        }
        */
        $('#task-info-wrap').css("display", "block");
        /*if($('#listbox-lead').hasClass('col-lg-9'))
        {
        $('#filter-lead').removeClass('filter-lead-show').addClass('filter-lead-hide');


        }*/
        $('#listbox-lead').hide();
        $('#lead-info-wrap').css("display", 'block');
    });
    $('#task-info-close').on("click", function(e) {
        $('.task-name-lnk').parent().removeClass('activelink');
        $('#task-info-wrap').css("display", "none");
        //$('#listbox-lead').removeClass('col-lg-12').addClass('col-lg-9');
        /*if($('#listbox-lead').hasClass('col-lg-12')){



        }*/
        /*if($('#listbox-lead').hasClass('col-lg-9')){

        $('#listbox-lead').removeClass('col-lg-9').addClass('col-lg-12')

        }*/
        $('#listbox-lead').show();
    })
    $('#lead-info-close').on("click", function(e) {
        $('.lead-name-lnk').parent().removeClass('activelink');
        if ($(window).width() == 768) {
            $('#listbox-lead').show();
        }
        $('#listbox-lead').removeClass('listbox-hide').removeClass('col-lg-3').addClass('col-lg-12');
        $('#filter-lead').removeClass('filter-lead-hide');
        $('#lead-info-wrap').css("display", 'none');
        $('.paging-wrap').css("display", "block");
    });
    $('#filter-lead-close').on("click", function(e) {
        $('#task-info-wrap').removeClass('col-lg-9').addClass('col-lg-12');
        $('#lead-list-wrap').removeClass('lead-list-wrap-white');
        $('#filter-lead').removeClass('filter-lead-show').addClass('filter-lead-hide');
        $('#listbox-lead').removeClass('col-lg-9').addClass('col-lg-12');
    });
    /*MARKETING PAGE SCRIPT*/
    $('#filter-lead-close').on("click", function(e) {
        $('#marketing-info-wrap').removeClass('col-lg-9').addClass('col-lg-12');
        $('#filter-lead').removeClass('filter-lead-show').addClass('filter-lead-hide');
    });
    $('#filter-toggle-btn').on("click", function(e) {
        if ($('#marketing-info-wrap').hasClass('col-lg-12')) {
            $('#marketing-info-wrap').removeClass('col-lg-12').addClass('col-lg-9');
        }
    });
    /*MARKETING PAGE SCRIPT*/
    $('#filter-toggle-btn').on("click", function(e) {
        $('.lead-name-lnk').parent().removeClass('activelink');
        if ($(window).width() == 768) {
            $('#listbox-lead').show();
        }
        $('#lead-list-wrap').removeClass('lead-list-wrap-white');
        if ($("#listbox-lead").hasClass("col-lg-12")) {
            $('#listbox-lead').removeClass('listbox-hide').removeClass('col-lg-12').addClass('col-lg-9');
        } else {
            $('#listbox-lead').removeClass('listbox-hide').removeClass('col-lg-3').addClass('col-lg-9');
        }
        if ($('#task-info-wrap').hasClass('col-lg-12')) {
            $('#task-info-wrap').removeClass('col-lg-12').addClass('col-lg-9');
        }
        /*if ($('#lead-info-wrap').hasClass('col-lg-12')) {
            $('#lead-info-wrap').removeClass('col-lg-12').addClass('col-lg-9');
        }*/
        $('#filter-lead').addClass('filter-lead-show');
        $('#lead-info-wrap').hide();
    });
});
/* ----- HIDING LEADS STARS ----- */
// Add a new repeating section
var attrs = ['for', 'id', 'name'];

function resetAttributeNames(section) {
    var tags = section.find('input, label'),
        idx = section.index();
    tags.each(function() {
        var $this = $(this);
        $.each(attrs, function(i, attr) {
            var attr_val = $this.attr(attr);
            if (attr_val) {
                $this.attr(attr, attr_val.replace(/_\d+$/, '_' + (idx + 1)))
            }
        })
    })
}
$('.add-location').click(function(e) {
    e.preventDefault();
    var lastRepeatingGroup = $('.repeatingsection-location').last();
    var cloned = lastRepeatingGroup.clone(true)
    cloned.insertAfter(lastRepeatingGroup);
    resetAttributeNames(cloned)
});
// Delete a repeating section
$('.cancel-location').click(function(e) {
    e.preventDefault();
    var current_fight = $(this).parent('div');
    var other_fights = current_fight.siblings('.repeatingsection-location');
    if (other_fights.length === 0) {
        alert("You should atleast have one Location");
        return;
    }
    current_fight.slideUp('slow', function() {
        current_fight.remove();
        // reset fight indexes
        other_fights.each(function() {
            resetAttributeNames($(this));
        })
    })
});
$('.add-provider').click(function(e) {
    e.preventDefault();
    var lastRepeatingGroup = $('.repeatingsection-provider').last();
    var cloned = lastRepeatingGroup.clone(true)
    cloned.insertAfter(lastRepeatingGroup);
    resetAttributeNames(cloned)
});
// Delete a repeating section
$('.cancel-provider').click(function(e) {
    e.preventDefault();
    var current_fight = $(this).parent('div');
    var other_fights = current_fight.siblings('.repeatingsection-provider');
    if (other_fights.length === 0) {
        alert("You should atleast have one Provider");
        return;
    }
    current_fight.slideUp('slow', function() {
        current_fight.remove();
        // reset fight indexes
        other_fights.each(function() {
            resetAttributeNames($(this));
        })
    })
});
/* ------ LEAD DOTS DROPDOWN ------ */
var materialdropdown = document.querySelector('.material-dropdowndot'),
    middle = document.querySelector('.middle'),
    cross = document.querySelector('.cross'),
    dropdown = document.querySelector('.dropdowndot');
materialdropdown.addEventListener('click', function() {
        middle.classList.toggle('active');
        cross.classList.toggle('active');
        dropdown.classList.toggle('active');
    })
    /* ------ LEAD DOTS DROPDOWN ------ */
$("document").ready(function() {
    $('.dropdown-menu').on('click', function(e) {
        if ($(this).hasClass('dropdown-menu-form')) {
            e.stopPropagation();
        }
    });
});