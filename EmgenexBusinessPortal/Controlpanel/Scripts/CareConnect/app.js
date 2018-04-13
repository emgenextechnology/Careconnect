
careconnect.config(function ($routeProvider) {

    $routeProvider.when("/leads", {
        controller: "leadController",
        //resolve: resolveController('/Controlpanel/Scripts/CareConnect/Controllers/leadController.js'),
        templateUrl: "/controlpanel/views/lead/index.html"
    });
    $routeProvider.when("/accounts", {
        controller: "accountController",
       // resolve: resolveController('/Controlpanel/Scripts/CareConnect/Controllers/accountController.js'),
        templateUrl: "/controlpanel/views/accounts/index.html"
    });
    $routeProvider.when("/tasks/", {
        controller: "tasksController",
        templateUrl: "/controlpanel/views/tasks/index.html"
    });
    $routeProvider.when("/tasks/:taskid", {
        controller: "tasksController",
        templateUrl: "/controlpanel/views/tasks/index.html"
    });
    $routeProvider.when("/sales", {
        controller: "salesController",
        templateUrl: "/controlpanel/views/sales/index.html"
    });
    $routeProvider.when("/sales/:reportid", {
        controller: "salesController",
        templateUrl: "/controlpanel/views/sales/index.html"
    });
    $routeProvider.when("/marketing", {
        controller: "marketingController",
        templateUrl: "/controlpanel/views/marketing/index.html"
    });
    $routeProvider.when("/login", {
        controller: "authController",
        templateUrl: "/controlpanel/views/Account/login.html"
    });
    $routeProvider.when("/dashboard/", {
        controller: "dashboardController",
        templateUrl: "/controlpanel/views/dashboard/index.html"
    });
    $routeProvider.when("/", {
        controller: "dashboardController",
        templateUrl: "/controlpanel/views/dashboard/index.html"
    });
    //$routeProvider.otherwise({ redirectTo: "/" });
    //$routeProvider.otherwise({
    //    controller: "dashboardController",
    //    templateUrl: "/controlpanel/views/dashboard/index.html"
    //});
});

//careconnect.directive('discardModal', ['$rootScope', '$modalStack',
//    function ($rootScope, $modalStack) {
//        return {
//            restrict: 'A',
//            link: function () {
//                /**
//                * If you are using ui-router, use $stateChangeStart method otherwise use $locationChangeStart
//                 * StateChangeStart will trigger as soon as the user clicks browser back button or keyboard backspace and modal will be removed from modal stack
//                 */
//                $rootScope.$on('$routeChangeStart', function (event) {
                  
//                    $modalStack.dismissAll('route change');
//                });
//            }
//        };
//    }
//]);

//careconnect.angular.module('app', []).run(function ($rootScope, $uibModalStack) {
//    $uibModalStack.dismissAll();
//});
var loginTriggered = false;

careconnect.filter('repfilter', function () {
    return function (items, condition) {
       
        var filtered = [];

        if (condition === null || condition === undefined || condition === '') {
            return items;
        }
        var pid = parseInt(condition.ParentId)
        angular.forEach(items, function (item) {
            if (pid === item.ParentId) {
                filtered.push(item);
            }
        });

        return filtered;
    };
});

careconnect.filter('setDecimal', function ($filter) {
    return function (input, places) {
        if (isNaN(input)) return input;
        // If we want 1 decimal place, we want to mult/div by 10
        // If we want 2 decimal places, we want to mult/div by 100, etc
        // So use the following to create that factor
        var factor = "1" + Array(+(places > 0 && places + 1)).join("0");
        return Math.round(input * factor) / factor;
    };
});

careconnect.directive('placehold', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var elem = element;

            window.setTimeout(function () {

                if ($(elem).hasClass('ng-not-empty') || ($(elem).val() != '' && $(elem).val() != null)) {
                    if ($(elem).siblings().length == 0) {
                        var lblData = $(elem).attr('placeholder');
                        var lbl = $('<label class="lblWithTxt">');
                        lbl.html(lblData);
                        lbl.insertBefore($(elem).addClass('txtWithLbl'));
                    }
                }
                else {
                    $(elem).siblings('.lblWithTxt').remove();
                }
            }, 500);


            $(elem).on("change paste keyup", function () {
                
                if ($(elem).hasClass('ng-not-empty') || ($(elem).val()!='' && $(elem).val()!=null)) {
                    if ($(elem).siblings().length == 0) {
                        var lblData = $(elem).attr('placeholder');
                        var lbl = $('<label class="lblWithTxt">');
                        lbl.html(lblData);
                        lbl.insertBefore($(elem).addClass('txtWithLbl'));
                    }
                }
                else {
                    $(elem).siblings('.lblWithTxt').remove();
                }
            });
        }
    }
});

careconnect.directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return parseInt(val, 10);
            });
            ngModel.$formatters.push(function (val) {
                return '' + val;
            });
        }
    };
});

//careconnect.directive('wbSelect2', function () {
//    return {
//        restrict: 'A',
//        scope: {
//            'selectWidth': '@',
//            'ngModel': '='
//        },
//        link: function (scope, element, attrs) {
//            //Setting default values for attribute params
//            scope.selectWidth = scope.selectWidth || 200;
//            element.select2({
//                width: scope.selectWidth,
//            });
//            scope.$watch('ngModel', function(newValue, oldValue){
//                element.select2().val(newValue);
//            });
//        }
//    };
//});

//careconnect.directive('date', function (dateFilter) {
//    return {
//        require: 'ngModel',
//        link: function (scope, elm, attrs, ctrl) {
//            var dateFormat = attrs['date'] || 'MM-dd-yyyy';
//            console.log(attrs['testattr'])
//            //ctrl.$formatters.unshift(function (modelValue) {
//            //    return modelValue;
//            //});
//        }
//    };
//});

careconnect.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'emgen'
});

careconnect.config(function ($httpProvider) {
    $httpProvider.defaults.withCredentials = true;
    $httpProvider.interceptors.push('authInterceptorService');
});

//myApp.config(['$compileProvider', function ($compileProvider) {
//    $compileProvider.debugInfoEnabled(false);
//    $compileProvider.commentDirectivesEnabled(false);
//    $compileProvider.cssClassDirectivesEnabled(false);
//}]);

careconnect.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

function searchInArray(obj, id) {
    for (var index in obj) {
        if (obj[index].Id == id) {
            return obj[index];
        }
    }

}

function clearSelect() {
    $("select").each(function () {
        if ($(this).val('').length > 0)
            $(this).val('').trigger("change");
        });
}

function getMinutes() {
    var d = new Date();
    var n = d.getMinutes();
    return n;
}

function dateFix(dt) {
    if (dt != null && dt != '')
        return (new Date(dt));

    return null;
}

function toLocal(date) {
    try {
        var local = new Date(date);
        local.setMinutes(date.getMinutes() - date.getTimezoneOffset());
        return local;
    }
    catch (e) {
        return date;
    }
}

function toUtc(date) {
    try {
        var utc = new Date(date);
        utc.setMinutes(date.getMinutes() + date.getTimezoneOffset());
        return utc;
    }
    catch (e) {
        return date;
    }
}

function getDateName(index) {
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
               "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    return months[index];
}
