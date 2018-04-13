
var careconnect;
var careconnect = angular.module("careconnect", ["ui.bootstrap",
'gridshore.c3js.chart',
'chart.js',
'ngRoute',
'ui.select2',
    'ui.mask',
    'ui.select',
    'ngSanitize',
    'clear-input',
'ui.bootstrap.datetimepicker',
'angularMoment',
'LocalStorageModule',
'naif.base64',
'ngCookies',
'ui.tinymce',
'720kb.datepicker',
'ngMask',
'ngScrollbars'
])
.filter('to_trusted', ['$sce', function ($sce) {
        return function (text) {
            return $sce.trustAsHtml(text);
        };
    }])
.filter('tel', function () {
    return function (tel) {
        if (!tel) { return ''; }

        var value = tel.toString().trim().replace(/^\+/, '');

        if (value.match(/[^0-9]/)) {
            return tel;
        }

        var country, city, number;

        switch (value.length) {
            case 10: // +1PPP####### -> C (PPP) ###-####
                country = 1;
                city = value.slice(0, 3);
                number = value.slice(3);
                break;

            case 11: // +CPPP####### -> CCC (PP) ###-####
                country = value[0];
                city = value.slice(1, 4);
                number = value.slice(4);
                break;

            case 12: // +CCCPP####### -> CCC (PP) ###-####
                country = value.slice(0, 3);
                city = value.slice(3, 5);
                number = value.slice(5);
                break;

            default:
                return tel;
        }

        if (country == 1) {
            country = "";
        }

        number = number.slice(0, 3) + '-' + number.slice(3);

        return (country + " (" + city + ") " + number).trim();
    };
})
.filter('dateSuffix', function ($filter) {
    var suffixes = ["th", "st", "nd", "rd"];
    return function (input) {
        if (input == null)
            return null;
        var dtfilter = $filter('date')(input, 'MMM dd');
        var month = dtfilter.slice(0, 3);
        var day = parseInt(dtfilter.slice(-2));
        var relevantDigits = (day < 30) ? day % 20 : day % 30;
        var suffix = (relevantDigits <= 3) ? suffixes[relevantDigits] : suffixes[0];
        return month + " " + day + suffix + " ";
    };
})
.directive('expandTo', function () {
    return {
        restrict: 'A',
        link: function ($scope, $element, $attributes) {
            var expandsize = $attributes['expandTo'] || '50px';
            var original = $element.width();

            $element.on('focus', function () {
                $element.animate({
                    width: expandsize
                }, 500, function () {
                    // Animation complete.
                });
            }).on('blur', function () {
                $element.animate({
                    width: '150px'
                }, 500, function () {
                    // Animation complete.
                });
            });
        }
    }
})
//.filter('timeAgo', ['$interval', function ($interval) {
//    // trigger digest every 60 seconds
//    $interval(function () {

//    }, 60000);

//    function fromNowFilter(time) {
//        return moment(time).fromNow();
//    }

//    fromNowFilter.$stateful = true;
//    return fromNowFilter;
//}]);
//.directive('elemReady', function ($parse) {
//    return {
//        restrict: 'A',
//        link: function ($scope, elem, attrs) {
//            elem.ready(function () {
//                $scope.$apply(function () {
//                    var func = $parse(attrs.elemReady);
//                    func($scope);
//                })
//            })
//        }
//    }
//})
//.directive('jqdatepicker', function ($filter) {
//    return {
//        restrict: 'A',
//        require: '?ngModel',
//        link: function (scope, element, attrs, input) {
//            if (!input) { return; } // If no ngModel then return;

//            element.datepicker(createCalendarOptions());
//            setInitialDateFormatOnInput();

//            function createCalendarOptions() {
//                if (!attrs.rsCalendarPopup) { return addRequiredJqueryFunction(defaultOptions); }
//                return formatOptions();
//            }

//            function formatOptions() {
//                var options = scope.$eval(attrs.rsCalendarPopup);
//                // Turn string into object above.
//                return addRequiredJqueryFunction(options);
//            }

//            function addRequiredJqueryFunction(options) {
//                options.onSelect = changeDate;
//                // add onSelect to passed in object and reference local changeDate function, which will update changes to input.$modelValue.
//                return options;
//            }

//            function changeDate(date) {
//                input.$setViewValue(date);
//            }

//            function setInitialDateFormatOnInput() {
//                setTimeout(function () {
//                    // This timeout is required to delay the directive for the input.modelValue to resolve.
//                    // However, there is no actual timeout time. This is a must to get
//                    // Angular to behave.
//                    element.datepicker("setDate", formatToJqueryUIDateFormat());
//                });
//            }

//            function formatToJqueryUIDateFormat() {
//                return $.datepicker.parseDate('yy-mm-dd', input.$modelValue);
//                // 'yy-mm-dd' is not the format you want the calendar to be
//                // it is the format the original date shows up as.
//                // you set your actual formatting using the calendar options at
//                // the top of this directive or inside the passed in options.
//                // The key is called dateFormat, in this case it's set as
//                // dateFormat: "MM d, yy" which makes June 30, 2015.
//            }
//        } // link
//    } // return
//})
;//'ngAnimate',
//careconnect.config(function ($animateProvider) {

//    // ignore animations for any element with class `ng-animate-disabled`
//    $animateProvider.classNameFilter(/^((?!(ng-animate-disabled)).)*$/);ss

//});

moment.lang('en', {
    relativeTime: {
        future: "in %s",
        past: "%s",
        //s: "%d Seconds",
        //m: " 1 Minute",
        s: 'a few seconds',
        m: 'a minute',
        mm: " %d Minutes",
        h: " 1 Hour",
        hh: " %d Hours",
        d: " 1 Day",
        dd: " %d Days",
        M: " 1 Month",
        MM: " %d Months",
        y: "1 Year",
        yy: "%d Years"
    }
});

function scrollToInvalid() {
    element = document.getElementsByClassName("ng-invalid");
    alignWithTop = true;
    element.scrollIntoView(alignWithTop);
}


$(window).scroll(function () {
    if ($(window).scrollTop() >= 186) {
        $('.lead-top-search').addClass('lead-top-search_fixed');
        $('#lead-list-wrap').addClass('scroll_padding');
    }
    else {
        $('.lead-top-search').removeClass('lead-top-search_fixed');
        $('#lead-list-wrap').removeClass('scroll_padding');
    }
});

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

