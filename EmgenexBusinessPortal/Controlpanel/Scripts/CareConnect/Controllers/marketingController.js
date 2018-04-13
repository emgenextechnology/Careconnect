careconnect.controller('marketingController', function ($scope, $http, $route, $uibModal, $rootScope, $timeout, $window, authService, $cookies, $filter) {

    $scope.reloadPage = function () {
        $rootScope.closeModals();
        $route.reload();
    }

    $rootScope.lookUps = {
        lookupProviders: null,
        lookupGroups: null,
        lookupReps: null
    };

    $scope.MarketingFilter = {
        ServiceId: 0
    };

    $scope.show = {
        RX: true,
        Written: true,
        ReportDt: true,
        Provider: true,
        RepGroup: true,
        RepName: true
    };

    $scope.select2Options = {
        allowClear: true
    };

    $scope.select2OptionsDisable = {
        allowClear: false
    };

    $scope.listClass = 'col-lg-12';
    $scope.affectedFilterCount = 0;
    $scope.affectedFilters = [];
    $scope.uploadingReport = false;

    $scope.toggleFilter = function () {
        if ($scope.showFilter) {
            $scope.showFilter = false;
            $scope.listClass = 'col-lg-12';
        }
        else {
            $scope.showFilter = true;
            $scope.listClass = 'col-lg-9';
            $scope.showDetails = false;
            $scope.currentMarketing = null;
        }
    }

    $scope.loadFilters = function () {
        $scope.stopTimeout();
        $scope.startTimeout();
    }

    $scope.loadLookUps = function () {
        if ($rootScope.lookUps.lookupMarketingCategories == null) {
            $http.get(baseUrl + '/lookup/getallmarketingcategories').success(function (data) {
                $rootScope.lookUps.lookupMarketingCategories = data.Model.List;
            });
        }
        if ($rootScope.lookUps.lookupDocumentTypes == null) {
            $http.get(baseUrl + '/lookup/getalldocumenttypes').success(function (data) {
                $rootScope.lookUps.lookupDocumentTypes = data.Model.List;
            });
        }
        if ($rootScope.lookUps.lookupAddedBy == null) {
            $http.get(baseUrl + '/lookup/getallusersbybusinessId').success(function (data) {
                $rootScope.lookUps.lookupAddedBy = data.Model.List;
            });
        }
    }

    $scope.loadMarketing = function () {
        $scope.MarketingLoading = true;
        $http.post(baseUrl + '/Marketing/getbyfilter', $scope.MarketingFilter).success(function (data) {
            $scope.model = data.Model;
            $scope.MarketingLoading = false;
        });
    }

    $scope.resetFilter = function () {
        if ($scope.unwatchMarketingFilter)
            $scope.unwatchMarketingFilter();

        $http.get(baseUrl + '/Marketing/getfilter').success(function (data) {

            //alert('filterLoaded');

            $scope.MarketingFilter = data;
            $scope.MarketingFilter.CurrentPage = 1;
            $scope.MarketingFilter.PageSize = 10;

            $scope.unwatchMarketingFilter = $scope.$watch('MarketingFilter', function (value, oldValue) {
                //alert("watch");
                $scope.affectedFilterCount = 0;
                $scope.affectedFilters = [];
                angular.forEach(value, function (v, key) {
                    if (key == 'DocumentTypeId' || key == 'CategoryId' || key == 'UserId') {
                        if (v != null && v != [] && v != '') {
                            $scope.affectedFilterCount = $scope.affectedFilterCount + 1;
                            setFilterText(key, v);
                        }
                    }
                });
                $scope.loadMarketing();
            }, true);
        });
    };
    function setFilterText(key, value) {
        if (key == 'DocumentTypeId') {
            angular.forEach($scope.lookUps.lookupDocumentTypes, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'CategoryId') {
            angular.forEach($scope.lookUps.lookupMarketingCategories, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'UserId') {
            angular.forEach($scope.lookUps.lookupAddedBy, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'Keyword') {
            $scope.affectedFilters.push(value);
        }

    }

    var mytimeout;
    $scope.startTimeout = function () {
        $scope.startCount = $scope.startCount + 1;
        mytimeout = $timeout(function () {
            $scope.loadMarketing();
        }, 500);
    }

    $scope.stopTimeout = function () {
        $timeout.cancel(mytimeout);
    }

    $scope.base64Data = [];

    $scope.fileUpload = function (files) {
        $scope.uploadingReport = true;

        for (var i = 0; i < files.length; i++) {
            $scope.base64Data.push(files[i]);
        }

        if ($scope.base64Data.length > 0)
            $scope.currentMarketing.files = $scope.base64Data;
        else {
            $scope.base64Data.push(files);
            $scope.currentMarketing.files = $scope.base64Data;
        }

        $http.post(baseUrl + '/Marketing/Save', $scope.currentMarketing).success(function (data) {
            $scope.uploadingReport = false;
            if (data.Status == 200 || data.Status == 201) {
                $scope.startTimeout();
            }
        });
    }
    //$rootScope.$on("CallParentMethodMarketing", function (event, id) {
    //    $scope.MarketingData = { ReportId: id };
    //    $scope.showDetailsView($scope.MarketingData);
    //});
    $scope.showDetailsView = function (id) {
        $window.scrollTo(0, 0);
        $scope.loadingDetails = true;
        $scope.loadingData = false;

        $http.get(baseUrl + '/Marketing/GetMarketingById/' + id).success(function (data) {
            $scope.currentMarketing = data.Model;
            $scope.loadingDetails = false;
            $scope.loadingData = true;
        });

        $scope.showFilter = false;
        $scope.listClass = 'col-lg-3 listbox-hide';
        $scope.showDetails = true;
    }

    $scope.hideDetailsView = function () {
        $scope.showDetails = false;
        $scope.listClass = 'col-lg-12';
        $scope.showFilter = false;
        $scope.currentMarketing = null;
    }



    $scope.init = function () {

        $rootScope.controller = 'marketing';
        $rootScope.authData = authService.authentication;
        $cookies.put('location', 'Marketing');
        //$scope.stopTimeout();
        //$scope.startTimeout();
        $scope.loadLookUps();
        $scope.loadFilters();
        $scope.resetFilter();
    }
    $scope.editMarketing = function (id) {
        $scope.loadingEdit = true;
        var currentMarketing = "";
        $http.get(baseUrl + '/Marketing/GetMarketingById/' + id).success(function (data) {
            currentMarketing = data.Model;
            $scope.loadingDetails = false;
            $scope.currentMarketing = currentMarketing;
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/ControlPanel/Views/Marketing/addMarketing.html?1623',
                controller: 'newMarketingController',
                backdrop: 'static',
                keyboard: true,
                size: 'lg',
                resolve: {
                    MarketingData: function () {
                        return currentMarketing;
                    },
                    lookUps: function () {
                        return $scope.lookUps;
                    },
                    MarketingId: function () {
                        return 0;
                    }
                }
            });
            $scope.loadingEdit = false;
            modalInstance.result.then(function (model) {

                $scope.currentMarketing = model;
                $scope.loadMarketing();
            }, function () {
                //callback(false);
            });
            //$scope.loadNotes();
        });
    }
    $scope.newMarketing = function (id) {
        var data = { Model: null };
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ControlPanel/Views/Marketing/addMarketing.html?8776',
            controller: 'newMarketingController',
            backdrop: 'static',
            keyboard: true,
            size: 'lg',
            resolve: {
                MarketingId: function () {
                    return id;
                },
                lookUps: function () {
                    return $scope.lookUps;
                },
                MarketingData: function () {
                    return data.Model
                }
            }
        });
        modalInstance.result.then(function (flag) {
            $scope.loadMarketing();
        }, function () {
            //callback(false);
        });
    }
});
careconnect.controller('newMarketingController', function ($scope, $http, $uibModalInstance, MarketingId, valueService, lookUps, $rootScope, authService, MarketingData, $rootScope) {

    $rootScope.$on('$routeChangeSuccess',
      function (event, toState, toParams, fromState, fromParams) {
          $uibModalInstance.dismiss('cancel');
      }
  );

    $scope.toSpinner = false;
    $scope.practiceSpinner = false;
    $scope.setHighpriority = "";

    MarketingDataOld = angular.copy(MarketingData);

    var entity = {};
    if (MarketingData == null || MarketingData == undefined)
        $scope.currentMarketing = {};
    else
        $scope.currentMarketing = MarketingData;

    $scope.$watch("currentMarketing.AssignedTo", function (val) {
    }, true);

    $scope.base64Data = [];

    $scope.fileUpload = function (files) {
        for (var i = 0; i < files.length; i++) {
            $scope.base64Data.push(files[i]);
        }
    }

    $scope.removeAttachmentUp = function (file) {
        confirm("Do you want to delete the file?", function (flag) {
            if (flag) {
                $scope.base64Data.pop(file);
            };
        });
    }

    $scope.sendSpinner = true;
    $rootScope.authData = authService.authentication;

    $rootScope.removeAttachment = function (fileObject) {
        if (fileObject != undefined)
            confirm("Do you want to delete the file? ", function (flag) {
                if (flag) {
                    $http.post(baseUrl + '/Marketing/deletefile/' + fileObject.Id, {}).success(function (data) {
                        $scope.currentMarketing.FilesList.pop(fileObject);
                    });
                };
            });
    }
    $scope.submitMarketing = function () {
        if ($scope.MarketingForm.$valid == false) {
            $scope.errorMessege = [];
            angular.forEach($scope.MarketingForm.$error, function (value, key) {
                angular.forEach(value, function (value1, key1) {
                    $scope.errorMessege.push(value1.$name + ' is ' + key);
                })
            });
            alert($scope.errorMessege);
            return false;
        }
        $scope.sendSpinner = false;
        $scope.currentMarketing.files = $scope.base64Data;
        $http.post(baseUrl + '/Marketing/Save', $scope.currentMarketing).success(function (data) {
            $scope.leadSubmitting = false;
            if (data.Status == 200 || data.Status == 201) {
                $uibModalInstance.close(data.Model);
                $uibModalInstance.close();
            }

            $scope.sendSpinner = true;
        });
    }


    $scope.CloseModal = function (e) {
        confirm("Are you sure you want to leave this page with unsaved changes?", function (flag) {
            if (flag) {
                $scope.currentMarketing.PracticeId = null;
                $uibModalInstance.dismiss('cancel');
            }
        });
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

});