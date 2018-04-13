
careconnect.controller('headerController', ['$scope', '$location', '$timeout', '$http', '$uibModal', '$rootScope', '$window', '$routeParams', 'authService', '$location', '$cookies', '$interval',

function ($scope, $location, $timeout, $http, $uibModal, $rootScope, $window, $routeParams, authService, $location, $cookies, $interval) {

    var skip = 0;
    var take = 5;
    $scope.seemore = false;
    $scope.newlist = [];
    $scope.currentpage = 1;
    var reloadNotification = loadNotification = true;
    $scope.Stats = {
        NotificationCount: 0
    }

    ////stats
    $interval(function () {
        if ($rootScope.callStatistics == true)
            $scope.triggerNotifications(loadNotification);
    }, 30000);

    $scope.triggerNotifications = function (isOnload) {
        $http.get(baseUrl + '/notification/UnReadNotificationCount').success(function (data) {
            $scope.Stats = data.Model;
            reloadNotification = ($scope.Stats.NotificationCount != null && $scope.Stats.NotificationCount > 0) || isOnload == true;
        });
    };
    $scope.triggerNotifications(true);

    $scope.getNotificationList = function () {
        if (reloadNotification == true) {
            $scope.notificationLoading = true;
            $http.get(baseUrl + '/notification/all/' + 0 + '/' + 5).success(function (data) {
                skip = take;
                $scope.notificationmodel = data.Model;
                if (data.Model.NotificationList.Pager.TotalCount > 0 && data.Model.NotificationList.Pager.TotalCount > 5) {
                    $scope.seemore = true;
                }
                reloadNotification = loadNotification = false;
                $scope.Stats.NotificationCount = 0;
                $scope.notificationLoading = false;
            });
        }
    }

    $scope.loadmore = function () {
        $scope.currentpage = $scope.currentpage + 1;
        $scope.notificationLoading = true;

        $http.get(baseUrl + '/notification/all/' + skip + '/' + take).success(function (data) {
            skip = take;
            $scope.newlist = data.Model.NotificationList.List;
            $scope.notificationmodel.NotificationList.List.push.apply($scope.notificationmodel.NotificationList.List, $scope.newlist);
            if ($scope.currentpage > 1) {
                $scope.count = "";
            }
            else {
                $scope.count = data.Model.NotificationList.Pager.TotalCount;
            }
            if ($scope.currentpage >= data.Model.NotificationList.Pager.TotalPage) {
                $scope.seemore = false;
            }
            $scope.notificationLoading = false;
        });
    }

    $scope.reset = function () {
        $scope.count = "";
    }
    
    window.confirm = function (query, callback, isLead) {
        var template = isLead === true ? '/ControlPanel/Views/shared/confirmLead.html' : '/ControlPanel/Views/shared/confirm.html';
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: template,
            controller: 'confirmController',
            size: 'md',
            resolve: {
                message: function () {
                    return query;
                },
            }
        });

        modalInstance.result.then(function (flag) {
            callback(flag);
        }, function () {
            callback(false);
        });
    };
    
    window.alert = function (query, callback) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ControlPanel/Views/shared/alert.html',
            controller: 'alertconfirmController',
            size: 'md',
            resolve: {
                message: function () {
                    return query;
                },
            }
        });
    };

}]);

careconnect.controller('confirmController', function ($scope, $uibModalInstance, message, $uibModal, authService) {
    $scope.isArray = (message.constructor === Array);
    $scope.message = message;

    $scope.yes = function () {
        $uibModalInstance.close(true);
    }

    $scope.no = function () {
        $uibModalInstance.close(false);
    }
});

careconnect.controller('alertconfirmController', function ($scope, $uibModalInstance, message, $uibModal, authService) {
    $scope.isArray = (message.constructor === Array);
    $scope.message = message;
    $scope.OK = function () {
        $uibModalInstance.close(true);
    }
    $scope.CloseModal = function (e) {
        $uibModalInstance.dismiss('cancel');
    }
});