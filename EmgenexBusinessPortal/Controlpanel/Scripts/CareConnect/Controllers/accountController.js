/// <reference path="../../../Views/accounts/accountDetails.html" />

careconnect.factory('valueService', function ($interval) {
    var service = {
        value: 0,
    };

    return service;
});

careconnect.controller('accountController', function ($scope, $http, $route, $uibModal, valueService, $rootScope, $window, $routeParams, authService, $location, $filter, $cookies, $timeout) {

    $scope.accountFilter = {
        sortKey: null,
        orderBy: null
    };

    //$rootScope.isCollapsed = false;
    //$rootScope.callStatistics = false;

    $scope.reloadAccounts = function () {
        if ($location.path().indexOf('accounts') > 0)
            $route.reload();

        $rootScope.closeModals();
    }

    $scope.show = { Account: true, EnrolledDate: true, Services: true, Provider: true, RepGroup: true, RepName: true, Status: true, Notes: true };

    $scope.affectedFilterCount = 0;

    var mytimeout;
    $scope.startTimeout = function () {
        $scope.startCount = $scope.startCount + 1;
        mytimeout = $timeout(function () { $scope.loadAccounts() }, 500);
    }

    $scope.stopTimeout = function () {
        $timeout.cancel(mytimeout);
    }

    $scope.loadSalesActivity = function (currentAccount) {
        $scope.slaesLoading = true;
        $http.post(baseUrl + '/sales/getbyfilter', { CurrentPage: 1, PageSize: 6, PracticeId: currentAccount.Practice.Id }).success(function (data) {
            currentAccount.salesActivity = data.Model.List;
            $scope.salesLoading = false;
        });
        $scope.slaesLoading = false;
    }

    $scope.getNameArray = function (usersList) {
        var array = [];
        angular.forEach(usersList, function (value, key) {
            array.push(value.Name);
        })
        return array.join(', ');
    };

    $scope.loadTasks = function (currentAccount) {

        $scope.tasksLoading = true;

        //$scope.tasksFilter.PracticeId = practiceId;
        //$scope.tasksFilter.Take

        $http.post(baseUrl + '/tasks/getbyfilter', { CurrentPage: 1, PageSize: 3, PracticeId: currentAccount.Practice.Id }).success(function (data) {
            //$scope.hitLastPage = $scope.tasksFilter != null && $scope.tasksFilter.CurrentPage == data.Model.Pager.TotalPage;
            currentAccount.Tasks = data.Model.List;
            if (data.Model != null && data.Model.List != null && data.Model.List.length > 0) {
                // $scope.showDetailsView(data.Model.List[0]);
            }
            $scope.tasksLoading = false;
        });
    }

    $scope.resetFilter = function (isFirstLoading) {

        //var multiselects = document.getElementsByClassName("select2-selection__clear");
        //angular.element(multiselects).triggerHandler('click');

        if ($scope.unwatchAccountFilter)
            $scope.unwatchAccountFilter();

        $http.get(baseUrl + '/accounts/getfilter').success(function (data) {
            //  alert('filterLoaded');

            $scope.accountFilter = data;
            $scope.accountFilter.CurrentPage = 1;
            $scope.accountFilter.PageSize = 25;

            $scope.unwatchAccountFilter = $scope.$watch('accountFilter', function (value, oldValue) {
                $scope.affectedFilterCount = 0;
                $scope.affectedFilters = [];
                angular.forEach(value, function (v, key) {
                    if (key == 'Period' || key == 'RepGroupId' || key == 'RepId' || key == 'AccountStatus' || key == 'ProviderId' || key == 'ServiceId' || key == 'KeyWords' || key == 'IsActive') {
                        if (v != null && v != [] && v != '') {
                            $scope.affectedFilterCount = $scope.affectedFilterCount + 1;
                            setFilterText(key, v);
                        }
                    }
                });

                if (!isFirstLoading) {
                    $scope.stopTimeout();
                    $scope.startTimeout();
                }
                else {

                }
                isFirstLoading = false;
            }, true);

        });
    };

    function setFilterText(key, value) {
        if (key == 'Period') {
            angular.forEach($scope.lookUps.lookupPeriods, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'RepGroupId') {
            angular.forEach($scope.lookUps.lookupGroups, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'RepId') {
            angular.forEach($scope.lookUps.lookupReps, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'AccountStatus') {
            angular.forEach($scope.lookUps.lookupStatus, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'ProviderId') {
            angular.forEach($scope.lookUps.lookupProviders, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'ServiceId') {
            angular.forEach($scope.lookUps.lookupServices, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'KeyWords') {
            $scope.affectedFilters.push(value);
        }
    }

    $scope.addTask = function (id) {
        $scope.addingToTask = true;
        $http.get(baseUrl + '/accounts/getvmaccount/' + id).success(function (data) {
            $rootScope.isFromPractice = true;
            $rootScope.taskPracticeId = data.Model.PracticeId;
            $location.path('/tasks');
            $scope.addingToTask = false;
        });
    }

    $scope.showSalesItem = function (reportId) {
        $location.path('/sales/' + reportId);
    }


    $rootScope.$on("CallParentMethodAccount", function (event, id) {
        $scope.showDetailsView(id);
    });
    $scope.showDetailsView = function (id) {

        $window.scrollTo(0, 0);
        $scope.loadingDetails = true;
        $scope.currentAccount = null;

        $scope.setFlag = function () {

            $scope.loadingFlagUpdate = true;
            $http.post(baseUrl + '/accounts/toggleflag/' + id).success(function (data) { $scope.currentAccount.HasFlag = data.Model; $scope.loadingFlagUpdate = false; });
        }

        $http.get(baseUrl + '/accounts/getaccount/' + id).success(function (data) {
            $scope.currentAccount = data.Model;
            $scope.loadingDetails = false;
            $scope.currentPage = 1;
            $scope.currentAccount.Notes = null;
            $scope.currentNotePage = 1;
            $scope.loadNotes();
            $scope.loadTasks($scope.currentAccount);
            $scope.loadSalesActivity($scope.currentAccount);
        });



        $scope.showFilter = false;
        $scope.listClass = 'col-lg-3 listbox-hide account-listbox';
        $scope.showDetails = true;
    }

    $scope.setStatus = function (currentAccount) {

        if (currentAccount == undefined)
            if ($scope.currentAccount != null)
                currentAccount = $scope.currentAccount;


        if (currentAccount.IsActive) {
            confirm("Do you want to make this account inactive? ", function (flag) {
                if (flag) {
                    $scope.loadingStatusUpdate = true;
                    $http.post(baseUrl + '/accounts/togglestatus/' + currentAccount.Id).success(function (data) {
                        if ($scope.currentAccount)
                            $scope.currentAccount.IsActive = data.Model;
                        $scope.loadingStatusUpdate = false;
                        $scope.loadAccounts();
                    });

                }

            });
        }
        else {
            $scope.loadingStatusUpdate = true;
            $http.post(baseUrl + '/accounts/togglestatus/' + currentAccount.Id).success(function (data) {
                if ($scope.currentAccount)
                    $scope.currentAccount.IsActive = data.Model;
                $scope.loadingStatusUpdate = false;
                $scope.loadAccounts();
            });
        }

    }

    $scope.$watch('currentAccount', function () {
        if ($scope.model && $scope.currentAccount != undefined) {

            var length = $scope.model.List.length;
            for (var i = 0; i < length; i++) {
                if ($scope.model.List[i].Id == $scope.currentAccount.Id) {
                    $scope.model.List[i] = $scope.currentAccount;
                    break;
                }
            }
        }

    });

    $scope.hideDetailsView = function () {
        $scope.showDetails = false;
        $scope.listClass = 'col-lg-12';
        $scope.showFilter = false;
        $scope.currentAccount = null;
    }

    $scope.lookupLeadStatus = [{ Id: true, Value: "Active" }, { Id: false, Value: "Inactive" }];

    $scope.loadLookUps = function () {

        if ($rootScope.lookUps.lookupGroups == null) {
            $http.get(baseUrl + '/lookup/getallrepgroups').success(function (data) {
                $rootScope.lookUps.lookupGroups = data.Model.List;
            });
        }

        if ($rootScope.lookUps.lookupReps == null) {
            $http.post(baseUrl + '/lookup/getallreps').success(function (data) {
                $rootScope.lookUps.lookupReps = data.Model.List;
            });
        }

        if ($rootScope.lookUps.lookupPeriods == null) {
            $http.get(baseUrl + '/lookup/getallperiods').success(function (data) {
                $rootScope.lookUps.lookupPeriods = data.Model.List;
            });
        }

        if ($rootScope.lookUps.lookupStatus == null) {
            $http.get(baseUrl + '/lookup/getallfilterstatus').success(function (data) {
                $rootScope.lookUps.lookupStatus = data.Model.List;
            });
        }


        if ($rootScope.lookUps.lookupPracticeType == null) {
            $http.get(baseUrl + '/lookup/getallpracticetypes').success(function (data) {
                $rootScope.lookUps.lookupPracticeType = data.Model.List;
            });

        }

        if ($rootScope.lookUps.lookupSpecialities == null) {
            $http.get(baseUrl + '/lookup/getallspecialities').success(function (data) {
                $rootScope.lookUps.lookupSpecialities = data.Model.List;
            });

        }

        if ($rootScope.lookUps.lookupStates == null) {
            $http.get(baseUrl + '/lookup/getallstates').success(function (data) {
                $rootScope.lookUps.lookupStates = data.Model.List;
            });

        }

        if ($rootScope.lookUps.lookupProviderDegree == null) {
            $http.get(baseUrl + '/lookup/getalldegrees').success(function (data) {
                $rootScope.lookUps.lookupProviderDegree = data.Model.List;
            });

        }
        if ($rootScope.lookUps.lookupLeadSources == null) {
            $http.get(baseUrl + '/lookup/getallleadsources').success(function (data) {
                $rootScope.lookUps.lookupLeadSources = data.Model.List;
            });
        }
        if ($rootScope.lookUps.lookupServices == null) {
            $http.get(baseUrl + '/lookup/getallservices').success(function (data) {
                $rootScope.lookUps.lookupServices = data.Model.List;
            });
        }
        //if ($rootScope.lookUps.lookupProviders == null) {
        //    $http.get(baseUrl + '/lookup/getallproviders').success(function (data) {
        //        $rootScope.lookUps.lookupProviders = data.Model.List;
        //        $timeout(function () { $scope.lookupProviders = $rootScope.lookUps.lookupProviders; }, 3000)
        //    });
        //}
        //else {
        //    $timeout(function () { $scope.lookupProviders = $rootScope.lookUps.lookupProviders; }, 3000)
        //}

        //if ($rootScope.lookUps.lookupPracticeList == null) {
        //    $http.post(baseUrl + '/Practice/All', {}).success(function (data) {
        //        $rootScope.lookupPracticeList = data.Model.List;
        //    });
        //}
    }

    $scope.getallproviders = function (searchKey) {
        if (searchKey != "")
            $scope.ProviderSpinner = true;
        $http.get(baseUrl + '/lookup/getallproviders/' + searchKey).success(function (data) {
            if (data.Model)
                $rootScope.lookUps.lookupProviders = data.Model.List;
            $scope.ProviderSpinner = false;
        });
    }

    $scope.toggleFilter = function () {

        if ($scope.showFilter) {
            $scope.showFilter = false;
            $scope.listClass = 'col-lg-12';
        }
        else {

            $scope.showFilter = true;
            $scope.listClass = 'col-lg-9';
            $scope.showDetails = false;
            $scope.currentAccount = null;
        }
    }

    $scope.loadAccounts = function () {
        $scope.accountLoading = true;

        $http.post(baseUrl + '/accounts/getbyfilter', $scope.accountFilter).success(function (data) {

            //if (data.Status != 200) {
            //    alert('Something unusual happened!');
            //    return false;
            //}
            $scope.model = data.Model;
            $scope.accountLoading = false;
        });
    }

    $scope.init = function () {

        $cookies.put('location', 'accounts');
        $scope.loadAccounts();
        $scope.resetFilter(true);
        $scope.select2Options = {
            allowClear: true
        }
        $scope.select2OptionsDisable = {
            allowClear: false
        };

        $rootScope.authData = authService.authentication;

        $scope.accountLoading = true;
        $rootScope.controller = 'Accounts';
        $scope.listClass = 'col-lg-12';
        $scope.showFilter = false;
        $scope.showDetails = false;

        //$scope.accountFilter = { PageSize: 10 };
        //$scope.lookupPeriods = null;
        //$scope.lookupGroups = null;
        //$scope.lookupReps = null;
        //$scope.lookupStatus = null;
        //$scope.lookupPracticeType = null;
        //$scope.lookupSpecialities = null;
        //$scope.lookupStates = null;
        //$scope.lookupProviderDegree = null;

        $scope.lookupProviders = null;

        $timeout(function () {
            $scope.loadLookUps();
        }, 1000)

        $scope.$on('newAccountFloating', function (event, args) {
            $scope.newAccount();
        });
        if ($rootScope.showNewAccount && $rootScope.showNewAccount == true) {
            $rootScope.showNewAccount = false;
            $scope.newAccount()
        }
    }

    $scope.editAccount = function (id) {
        $scope.loadingEdit = true;
        $scope.newAccount(id);
    }
    $scope.newAccount = function (id) {
        if (id == undefined) {
            $scope.addingAccount = true;
        }
        $http.get(baseUrl + '/accounts/getvmaccount/' + id).success(function (data) {

            if (data != null && data.Model != null && data.Model.Locations) {

                var locatios = data.Model.Locations;
                if (locatios.length > 0 && data.Model.Providers.length > 0) {
                    var log = [];
                    angular.forEach(data.Model.Providers, function (provider) {
                        if (provider.Location != null) {
                            for (var i = 0; i < locatios.length; i++) {

                                if (locatios[i].AddressId == provider.Location.AddressId) {
                                    provider.Location.AddressIndex = i;
                                }
                            }
                        }
                    }, log);
                }
            }

            if (data.Model.EnrolledDate == '' || data.Model.EnrolledDate == null) {
                data.Model.EnrolledDate = new Date();
            }

            $scope.editModel = data.Model;
            openFrom(id, data.Model)

        });
    }

    openFrom = function (id, data) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ControlPanel/Views/accounts/addaccount.html?2345',
            controller: 'newAccountController',
            backdrop: 'static',
            keyboard: false,
            size: 'lg',
            resolve: {
                accountId: function () {
                    return id;
                },
                lookUps: function () {
                    return $rootScope.lookUps;
                },
                accountData: function () {
                    return data
                }
            }
        });

        modalInstance.rendered.then(function () {
            $scope.addingAccount = false;
            $scope.loadingEdit = false;
        });

        modalInstance.opened.then(function () {

        });

        modalInstance.result.then(function (result) {

            if (id == undefined || id == 0) {
                $scope.loadAccounts();
            }
            else if ($scope.currentAccount != null) {
                $scope.currentAccount = result;

                $scope.currentNotePage = 1;
                $scope.loadNotes();
            }
        }, function (e) {
        });

        $scope.addingAccount = false;
        $scope.loadingEdit = false;
    }

    if ($rootScope.isConvertFromLead == true) {
        $rootScope.isConvertFromLead = false;
        $scope.currentLead.LeadId = $scope.currentLead.Id;
        $scope.currentLead.Id = null;
        openFrom(0, $scope.currentLead)
    }

    $scope.note = { Message: '' };
    $scope.addNote = function () {
        if ($scope.note.Message != '') {
            $scope.loadingNotes = true;
            $http.post(baseUrl + '/accounts/' + $scope.currentAccount.Practice.Id + '/save', { Description: $scope.note.Message, ParentTypeId: 1, ParentId: $scope.currentAccount.Practice.Id }).success(function (data) {
                $scope.note.Message = '';
                $scope.currentNotePage = 1;
                $scope.currentAccount.Notes = null;
                $scope.loadNotes();
            });
        }
    }

    $scope.moreNotes = false;
    $scope.currentNotePage = 1;
    $scope.loadNotes = function () {
        $scope.loadingNotes = true;
        //$http.post(baseUrl + '/notes/all', { ParentTypeId: 1, ParentId: $scope.currentAccount.Practice.Id, CurrentPage: $scope.currentNotePage, PageSize: 10 }).success(function (data) {
        $http.post(baseUrl + '/accounts/' + $scope.currentAccount.Practice.Id + '/notes', { ParentTypeId: 1, ParentId: $scope.currentAccount.Practice.Id, CurrentPage: $scope.currentNotePage, PageSize: 10 }).success(function (data) {
            if ($scope.currentAccount.Notes == null)
                $scope.currentAccount.Notes = data.Model.List;
            else {
                var oldList = $scope.currentAccount.Notes;
                $scope.currentAccount.Notes = null;
                $scope.currentAccount.Notes = data.Model.List;
                $scope.currentAccount.Notes.push.apply($scope.currentAccount.Notes, oldList);
            }
            $scope.loadingNotes = false;
            $scope.moreNotes = data.Model.Pager.TotalPage > $scope.currentNotePage;
            $scope.currentNotePage++;
        });
    }

    $scope.createAccountpdf = function (Id) {
        var win = window.open('', '_blank');
        win.location.href = baseUrl + '/account/Pdf/' + Id;
    };

    $scope.setSelectedColumn = function (columnName) {
        $scope.accountFilter.orderBy = ($scope.accountFilter.orderBy == null || $scope.accountFilter.orderBy == 'desc') ? 'asc' : ($scope.accountFilter.sortKey == columnName && $scope.accountFilter.orderBy == 'asc') ? 'desc' : 'asc';
        $scope.accountFilter.sortKey = columnName;
    };

    $scope.deleteNote = function (note) {
        confirm("Do you want to delete the note? ", function (flag) {
            if (flag) {
                $scope.loadingDeleteStatus = true;
                $http.post(baseUrl + '/accounts/1/delete/' + note.Id).success(function (data) {
                    if (data.IsSuccess) {
                        var index = $scope.currentAccount.Notes.indexOf(note);
                        $scope.currentAccount.Notes.splice(index, 1);
                    }
                });
            }
        });
    }
});

careconnect.controller('newAccountController', function ($scope, $http, $uibModalInstance, accountId, valueService, lookUps, accountData, $rootScope) {

    $scope.prefferedMethods = [
       { Value: "Email", Id: 1 },
       { Value: "Phone", Id: 2 }];

    $rootScope.$on('$routeChangeSuccess',
       function (event, toState, toParams, fromState, fromParams) {
           $uibModalInstance.dismiss('cancel');
       }
   );

    $scope.formatAddress = function (selectedItem) {
        var adrs = selectedItem.AddressLine1;
        if (selectedItem.AddressLine2 != null && selectedItem.AddressLine2 != '') {
            adrs = adrs + ', ' + selectedItem.AddressLine2;
        }
        if (selectedItem.City != null && selectedItem.City != '') {
            adrs = adrs + ', ' + selectedItem.City;
        }
        if (selectedItem.StateId != null && selectedItem.StateId != '') {
            adrs = adrs + ', ' + searchInArray($rootScope.lookUps.lookupStates, selectedItem.StateId).Value;
        }

        return adrs;
    }

    $scope.init = function () {
        if (accountData.Providers == null || accountData.Providers.length == 0)
            $scope.newProvider();
    };

    $scope.lookUps = lookUps;

    if (accountData.EnrolledDate == '' || accountData.EnrolledDate == null) {
        accountData.EnrolledDate = new Date();
    }

    accountData.EnrolledDate = dateFix(accountData.EnrolledDate);//datefix tweak

    $scope.currentAccount = accountData;

    $scope.isToday = function (date, mode) {
        var today = new Date();
        today.setHours(12, 0, 0, 0);
        if (today.toDateString() == date.toDateString()) {
            return 'today';
        }
    };

    $scope.newLocation = function () {
        if ($scope.currentAccount.Locations == null) {
            $scope.currentAccount.Locations = [];
        }
        $http.get(baseUrl + '/Accounts/getlocationobject').success(function (data) {
            data.AddressIndex = $scope.currentAccount.Locations.length;
            $scope.currentAccount.Locations.push(data);
        });
    }

    $scope.removeLocation = function (index) {
        $scope.currentAccount.Locations.splice(index, 1);
    }

    $scope.newProvider = function () {
        if ($scope.currentAccount.Providers == null) {
            $scope.currentAccount.Providers = [];
        }
        $http.get(baseUrl + '/Accounts/getproviderobject').success(function (data) {
            data.IsPracticeLoc = 1;
            $scope.currentAccount.Providers.push(data);

        });
    }

    $scope.removeProvider = function (index) {
        $scope.currentAccount.Providers.splice(index, 1);
    }

    $scope.getAccount = function (id) {
        $http.get(baseUrl + '/Accounts/getvmAccount/' + id).success(function (data) {

            if (data.Model.EnrolledDate == '' || data.Model.EnrolledDate == null) {
                data.Model.EnrolledDate = new Date();
            }

            $scope.currentAccount = data.Model;

            if (data.Model.Providers == null || data.Model.Providers.length == 0)
                $scope.newProvider();

        });
    }

    $scope.submitAccount = function () {

        if ($scope.accountForm.$valid == false) {
            $scope.errorMessege = [];
            angular.forEach($scope.accountForm.$error, function (value, key) {
                angular.forEach(value, function (value1, key1) {
                    $scope.errorMessege.push(value1.$name + ' is ' + key);
                })
            });
            alert($scope.errorMessege);
            return false;
        }

        if ($scope.currentAccount == null) {
            return false;
        }

        if ($scope.currentAccount.EnrolledDate == null) {
            return false;
        }

        $scope.accountSubmitting = true;
        $scope.currentAccount.ContactPreferenceId = 1;

        var locationArray = [];
        var addressObj = $scope.currentAccount.Locations;
        for (key in addressObj) {

            locationArray.push(addressObj[key]);
        }
        $scope.currentAccount.Locations = locationArray;


        var providerArray = [];
        var providerObj = $scope.currentAccount.Providers;
        for (key in providerObj) {
            if (providerObj[key].IsPracticeLoc == 1) {
                providerObj[key].Location = null;
            }
            providerArray.push(providerObj[key]);
        }
        $scope.currentAccount.Providers = providerArray;

        $http.post(baseUrl + '/accounts/save', $scope.currentAccount).success(function (data) {

            if (data.Status == 200 || data.Status == 201) {
                $uibModalInstance.close(data.Model);
            }
            else if (data.Status == 412) {
                $scope.errorMessege = [];
                angular.forEach(data.ResponseMessage, function (value, key) {
                    $scope.errorMessege.push(value.Message);
                });
                $scope.isArray = ($scope.errorMessege.constructor === Array);
                alert($scope.errorMessege);
            }
            $scope.accountSubmitting = false;
        });

    }

    $scope.CloseModal = function (e) {
        if ($('.unsaved-form').length > 0) {
            confirm("Are you sure you want to leave this page with unsaved changes?", function (flag) {
                if (flag) {
                    $uibModalInstance.dismiss('cancel');
                };
            });
        }
        else {
            $uibModalInstance.dismiss('cancel');
        }
    }

    $scope.init();

    $scope.getProvider = function (obj) {
        if (obj.NPI != null) {

            if (obj.NPI.length == 10) {
                obj.Loader = true;
                $http.get(baseUrl + '/Provider/getProviderByNPI?NPI=' + obj.NPI).success(function (data) {
                    if (data.Status == 200) {
                        if (data.FromDb) {
                            obj.FirstName = data.model.Model.FirstName;
                            obj.MiddleName = data.model.Model.MiddleName;
                            obj.LastName = data.model.Model.LastName;
                            obj.DegreeId = data.model.Model.DegreeId;
                            obj.Loader = false;
                        }
                        else {
                            $http.get(baseUrl + '/Provider/getGlobalProviderByNPI?NPI=' + obj.NPI).success(function (data) {
                                var response = JSON.parse(data);
                                if (response.result_count > 0) {
                                    var enumerationType = response.results[0].enumeration_type;
                                    switch (enumerationType) {
                                        case "NPI-1":
                                            obj.FirstName = response.results[0].basic.first_name;
                                            obj.MiddleName = response.results[0].basic.middle_name;
                                            obj.LastName = response.results[0].basic.last_name;
                                            obj.DegreeId = 0;
                                            obj.Loader = false;
                                            break;
                                        case "NPI-2":
                                            obj.FirstName = response.results[0].basic.authorized_official_first_name;
                                            obj.MiddleName = response.results[0].basic.authorized_official_middle_name;
                                            obj.LastName = response.results[0].basic.authorized_official_last_name;
                                            obj.DegreeId = 0;
                                            obj.Loader = false;
                                            break;
                                    }

                                }
                                else {
                                    obj.FirstName = "";
                                    obj.MiddleName = "";
                                    obj.LastName = "";
                                    obj.DegreeId = 0;
                                    obj.Loader = false;
                                    obj.ErroMessage = true;
                                }
                            });
                        }
                    }
                });

            }
        }
        obj.FirstName = "";
        obj.MiddleName = "";
        obj.LastName = "";
        obj.DegreeId = 0;
        obj.ErroMessage = false;
    }
});