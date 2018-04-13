/// <reference path="../../../Views/Shared/ProfileEdit.html" />
/// <reference path="../../../Views/Shared/ProfileEdit.html" />

careconnect.controller('authController', ['$scope', '$location', '$timeout', '$http', '$uibModal', '$rootScope', '$window', '$routeParams', 'authService', '$location', '$cookies',
function ($scope, $location, $timeout, $http, $uibModal, $rootScope, $window, $routeParams, authService, $location, $cookies) {

    //console.log('----authController-----');
    
    $rootScope.templateUrl = function (url) {
        return url + "?i=" + appVersion;
    }

    $rootScope.authData = { isAuth: false };
    $rootScope.lookUps = {
        lookupPeriods: null,
        lookupGroups: null,
        lookupProviders: null,
        lookupReps: null,
        lookupStatus: null,
        lookupPracticeType: null,
        lookupSpecialities: null,
        lookupStates: null,
        lookupServices: null,
        lookupProviderDegree: null
    };

    $rootScope.callStatistics = true;

    $rootScope.applyFormStyle = function (form) {
        $timeout(function () { //alert($(form).find("input").length) 
            $(form).find("input").each(function (index, elem) {
                $(elem).off('change');
                $(elem).on('keyup', function () {

                    //if ($(elem).val() != '') {
                    if ($(elem).hasClass('ng-not-empty')) {
                        if ($(elem).siblings().length == 0) {
                            var lblData = $(elem).attr('placeholder');
                            var lbl = $('<label class="lblWithTxt">');
                            lbl.html(lblData);
                            lbl.insertBefore($(elem).addClass('txtWithLbl'));
                        }
                    }
                    else {
                        $(elem).siblings().remove();
                    }
                });

                if ($(elem).val() != '') {
                    var lblData = $(elem).attr('placeholder');
                    var lbl = $('<label class="lblWithTxt">');
                    lbl.html(lblData);
                    lbl.insertBefore($(elem).addClass('txtWithLbl'));
                }
                else {
                    $(elem).siblings().remove();
                }
            });
        }, 1000);
    }
    
    $rootScope.closeModals = function () {
        //$('.modal').modal('hide');
    }

    $rootScope.currentUser = {};

    $rootScope.logged = false;
    var accessToken;
    var flag = true;
    $scope.message = "";
    $scope.loadingLogin = false;

    $rootScope.authData = authService.authentication;

    if (currentUserName != '') {

        //console.log('currentUserName', currentUserName);
        //console.log('$rootScope.authData', $rootScope.authData)

        if (loginTriggered == true)
            return;

        //if (currentUserName != $rootScope.authData.userName)
        //    authService.logOut();

        //if (!$rootScope.authData.isAuth)

       // console.log('-------------', $rootScope.authData.isAuth)
        if (!$rootScope.authData.isAuth)
        authService.login({}).then(function (response) {
            var Id = response.Model.UserId;
            //$rootScope.authData = response.Model;
            $rootScope.authData = authService.authentication;
            $scope.loadingLogin = false;
            //$rootScope.isAuthorized = $rootScope.authData.roles.length > 0 || $rootScope.authData.departments.length > 0 || $rootScope.authData.privileges.length > 0;
            var historyLocation = $cookies.get('location');
            if ($rootScope.authData.isAuth)
                //$location.path('/leads');
            {
                //console.log('authService.login({})');

                //if (historyLocation && historyLocation != '')
                //    $location.path($cookies.get('location'));
                //else
                   // $location.path('dashboard');
            }
            else {
                $scope.message = 'Access denied!!';
                authService.logOut();
            }
        },
          function (err) {

              $scope.loadingLogin = false;
              $scope.message = err.error_description;
          });
    }

    $scope.login = function () {
        flag = true;
        if ($scope.loginData.userName == "") {
            $scope.validateusername = "This field is required.";
            flag = false;

        }
        if ($scope.loginData.password == "") {
            $scope.validatepswrd = "This field is required.";
            flag = false;
        }
        if (flag) {
            $scope.loadingLogin = true;
            authService.login($scope.loginData).then(function (response) {
                var Id = response.Model.UserId;
                //$rootScope.authData = response.Model;
                $rootScope.authData = authService.authentication;
                $scope.loadingLogin = false;
                //$rootScope.isAuthorized = $rootScope.authData.roles.length > 0 || $rootScope.authData.departments.length > 0 || $rootScope.authData.privileges.length > 0;
                console.log('----login -- auth controller');
                if ($rootScope.authData.isAuth)
                    $location.path('/leads');
                else {
                    $scope.message = 'Access denied!!';
                    authService.logOut();
                }
            },
             function (err) {

                 $scope.loadingLogin = false;
                 $scope.message = err.error_description;
             });
        }
    };

    $rootScope.showSettings = function (searchStr) {

        if ($rootScope.authData == undefined) {
            authService.fillAuthData();
            $rootScope.authData = authService.authentication;
        }
        var roles = $rootScope.authData.roles;
        var departments = $rootScope.authData.departments;

        if (roles.indexOf('BusinessAdmin') > -1) {

            return true;
        }

        var privileges = $rootScope.authData.privileges;
        if (privileges.length == 0)
            return false;
        return (privileges.indexOf(searchStr) > -1)
    }

    $rootScope.showByPrevilege = function (searchStr) {

        // return true;

        if ($rootScope.authData == undefined) {
            authService.fillAuthData();
            $rootScope.authData = authService.authentication;
        }
        var roles = $rootScope.authData.roles;

        if (roles.indexOf('BusinessAdmin') > -1) {
            return true;
        }
        var departments = $rootScope.authData.departments;


        var privileges = $rootScope.authData.privileges;
        if (privileges.length == 0)
            return false;
        return (privileges.indexOf(searchStr) > -1)
    }

    $rootScope.logout = function () {
        authService.logOut();
        $cookies.put('location', '');
        document.getElementById('logoutForm').submit();
    }

    $rootScope.addLead = function () {

        $rootScope.showNewLead = true;
        $location.path('/leads');
        $rootScope.$broadcast('newLeadFloating');
    }

    $rootScope.addTask = function () {

        $rootScope.showNewTask = true;
        $location.path('/tasks');
        $rootScope.$broadcast('newTaskFloating');
    }

    $rootScope.addAccount = function () {
        $rootScope.showNewAccount = true;
        $location.path('/accounts');
        $rootScope.$broadcast('newAccountFloating');
    }

    $rootScope.redirect = function (data) {
        $http.post(baseUrl + '/notification/updateNotification/' + data.NotificationId).success(function (responseData) {
            if (responseData.Status == 200 || responseData.Status == 201) {
                //$('[data-notifyid="' + data.NotificationId + '"]').parent().removeClass('read').addClass('unread');
                //alert(data.NotificationId);
            }
        });
        var type = "";
        if (data.TargetType == "Lead") {
            type = "CallParentMethodLead";
            $location.path('/leads');

        }
        if (data.TargetType == "Account") {
            type = "CallParentMethodAccount";
            $location.path('/accounts');

        } if (data.TargetType == "Task") {
            type = "CallParentMethodTask";
            $location.path('/tasks');

        }
        if (data.TargetType == "Sales") {
            type = "CallParentMethodSales";
            $location.path('/sales');
        }
        var mytimeout;
        mytimeout = $timeout(function () {
            $rootScope.$emit(type, data.TargetId);
        }, 1000);
    }

    $rootScope.applyScroll = function () {
        //  $("html").niceScroll({ horizrailenabled: false });
    }

    $rootScope.EditProfile = function (UserName) {
        var modalInstance = $uibModal.open({
            animation: $rootScope.animationsEnabled,
            templateUrl: '/ControlPanel/Views/Shared/ProfileEdit.html?234',
            controller: 'UserProfileController',
            backdrop: 'static',
            keyboard: true,
            //windowClass: 'modal-content profile-edit-page  inner-width',
            size: 'md'
        });
        modalInstance.result.then(function (model) {
            //$rootScope.currentProfile = model;
        }, function () {
        });
    }

    var locHitPop = false;
    $scope.sessionExpired = function (isClose) {
        if (isClose == true) {
            $('.modal-backdrop').hide();
            $('.modal').hide();
            $cookies.put('_hitPopup', false);
            locHitPop = false;
        }
        else {
            var modalInstance = $uibModal.open({
                animation: $rootScope.animationsEnabled,
                templateUrl: '/ControlPanel/Views/Shared/SessionExpire.html?234',
                backdrop: 'static',
                controller: 'sessionController',
                keyboard: false,
                size: 'md'
            });
            $('.modal-backdrop').show();
            $rootScope.callStatistics = false;
            $cookies.put('_hitPopup', true);
            $http.post(baseUrl + '/Account/ClearSession');
        }
    }

    $cookies.put('_hitPopup', null);
    $scope.logoutIfInactive = function () {
        var IDLE_TIMEOUT = 30 * 60;
        $cookies.put('_idleSecondsCounter', 0);
        document.onclick = function () {
            $cookies.put('_idleSecondsCounter', 0)
        };
        document.onmousemove = function () {
            $cookies.put('_idleSecondsCounter', 0)
        };
        document.onkeypress = function () {
            $cookies.put('_idleSecondsCounter', 0)
        };
        window.setInterval(CheckIdleTime, 1000);

        function CheckIdleTime() {
            var counter = parseInt($cookies.get('_idleSecondsCounter'));
            counter++;
            $cookies.put('_idleSecondsCounter', counter);
            if (parseInt($cookies.get('_idleSecondsCounter')) >= IDLE_TIMEOUT && locHitPop == false) {
                $scope.sessionExpired();
                locHitPop = true;
            }
            else if ($cookies.get('_hitPopup') == 'null') {
                $scope.sessionExpired(true);
            }
        }
    }

    $scope.logoutIfInactive();

    var closeModel = function () {
        $('.modal-backdrop').remove();
        $('.modal').remove();
    }
}]);

careconnect.controller('sessionController', function ($scope, $http, $uibModalInstance, $rootScope) {


});

careconnect.controller('UserProfileController', function ($scope, $http, $uibModalInstance, $rootScope) {

    $rootScope.base64Data = [];

    $rootScope.fileUpload = function (files) {

        $rootScope.base64Data = [];
        $rootScope.base64Data.push(files);

    }

    $rootScope.submitProfile = function () {
        if ($scope.userProfileForm.$valid == false) {
            return false;
        }

        $scope.userProfileSubmitting = true
        $http.post(baseUrl + '/User/save', { FirstName: $rootScope.authData.FirstName, MiddleName: $rootScope.authData.MiddleName, LastName: $rootScope.authData.LastName, PhoneNumber: $rootScope.authData.PhoneNumber, files: $scope.base64Data }).success(function (data) {

            var rand = Math.floor((Math.random() * 3) + 1);
            $rootScope.authData.FilePath = $rootScope.authData.FilePath + "?" + rand;
            $rootScope.authData.fullName = $rootScope.authData.FirstName + " " + $rootScope.authData.LastName;

            $uibModalInstance.close(data.Model);
            $scope.userProfileSubmitting = false

        });
    }

    $rootScope.CloseModal = function (e) {
        $uibModalInstance.dismiss('cancel');
    }
});