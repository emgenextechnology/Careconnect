/// <reference path="../../../Views/Shared/leadPDF.html" />
/// <reference path="../../../Views/Lead/leadDetails.html" />

careconnect.factory('valueService', function ($interval) {
    var service = {
        value: 0,
    };

    return service;
});

careconnect.controller('leadController', ['$scope', '$http', '$route', '$uibModal', '$window', '$rootScope', '$location', 'leadService', 'authService', '$timeout', '$cookies', function ($scope, $http, $route, $uibModal, $window, $rootScope, $location, leadService, authService, $timeout, $cookies) {


    $scope.reloadLeads = function () {
        if ($location.path().indexOf('leads') > 0)
            $route.reload();
        $rootScope.closeModals();
    }
    
    //    modalInstance.result.then(function (flag) {
    //        callback(flag, $http);
    //    }, function () {
    //        callback(false);
    //    });
    //};

    $scope.affectedFilterCount = 0;
    $scope.affectedFilters = [];

    $scope.leadFilter = {
        sortKey: null,
        orderBy: null
    };

    $scope.lookupLeadStatus = [{ Id: true, Value: "Active" }, { Id: false, Value: "Inactive" }];

    $scope.loadLookUps = function () {
        // console.log('$rootScope.lookUps.lookupGroups', $rootScope.lookUps.lookupGroups);
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
        //if ($rootScope.lookUps.lookupPracticeList == null) {
        //    $http.post(baseUrl + '/Practice/All', {}).success(function (data) {
        //        $rootScope.lookupPracticeList = data.Model.List;
        //    });
        //} 
    }

    var mytimeout;
    $scope.startTimeout = function () {
        $scope.startCount = $scope.startCount + 1;
        mytimeout = $timeout(function () { $scope.loadLeads() }, 500);
    }

    $scope.stopTimeout = function () {
        $timeout.cancel(mytimeout);
    }

    var leadWatch;

    $scope.resetFilter = function (isFirstLoading) {

        if (leadWatch) {
            leadWatch();
        }

        // $('#period').val('').trigger("change");

        //$('#period').select2('val', 0);

        //$("select").val('').trigger("change");

        $http.get(baseUrl + '/leads/getfilter').success(function (data) {
            $scope.leadFilter = data;
            $scope.leadFilter.CurrentPage = 1;
            $scope.leadFilter.PageSize = 25;

            leadWatch = $scope.$watch('leadFilter', function (value, oldValue) {
                $scope.affectedFilterCount = 0;
                $scope.affectedFilters = [];
                angular.forEach(value, function (v, key) {
                    if (key == 'Period' || key == 'RepGroupId' || key == 'RepId' || key == 'LeadStatus' || key == 'KeyWords' || key == 'IsActive') {

                        if (v != null && v != [] && v != '' && v >= 0) {
                            $scope.affectedFilterCount = $scope.affectedFilterCount + 1;
                            setFilterText(key, v);
                        }
                    }
                });

                if (!isFirstLoading) {
                    $scope.stopTimeout();
                    $scope.startTimeout();
                }
                isFirstLoading = false;
            }, true);

        });
    };

    /// reload list when there is changes in filter

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
        else if (key == 'LeadStatus') {
            angular.forEach($scope.lookUps.lookupStatus, function (v, k) {
                if (v.Id == value)
                    $scope.affectedFilters.push(v.Value);
            });
        }
        else if (key == 'KeyWords') {
            $scope.affectedFilters.push(value);
        }
    }

    $scope.setStatus = function (currentLead) {

        if (currentLead == undefined)
            if ($scope.currentLead != null)
                currentLead = $scope.currentLead;

        if (currentLead.IsActive) {
            confirm("Do you want to make this account inactive? ", function (flag) {
                if (flag) {
                    $scope.loadingStatusUpdate = true;
                    $http.post(baseUrl + '/leads/togglestatus/' + currentLead.LeadId).success(function (data) {
                        if ($scope.currentLead)
                            $scope.currentLead.IsActive = data.Model;
                        $scope.loadingStatusUpdate = false;
                        $scope.stopTimeout();
                        $scope.startTimeout();
                    });
                }
            });
        }
        else {
            $scope.loadingStatusUpdate = true;
            $http.post(baseUrl + '/leads/togglestatus/' + currentLead.LeadId).success(function (data) {
                if ($scope.currentLead)
                    $scope.currentLead.IsActive = data.Model;
                $scope.loadingStatusUpdate = false;
                $scope.stopTimeout();
                $scope.startTimeout();
            });
        }
    }
    $rootScope.$on("CallParentMethodLead", function (event, id) {
        $scope.showDetailsView(id);
    });
    $scope.showDetailsView = function (id) {
        $window.scrollTo(0, 0);
        $scope.loadingDetails = true;

        $scope.setFlag = function () {
            $scope.loadingFlagUpdate = true;
            $http.post(baseUrl + '/leads/toggleflag/' + id).success(function (data) { $scope.currentLead.HasFlag = data.Model; $scope.loadingFlagUpdate = false; });
        }

        $scope.currentLead = null;

        $http.get(baseUrl + '/leads/getlead/' + id).success(function (data) {
            $scope.currentLead = data.Model;
            $scope.loadingDetails = false;

            $scope.currentPage = 1;
            $scope.currentLead.Notes = null;
            $scope.currentNotePage = 1;
            $scope.loadNotes();
        });

        $scope.showFilter = false;
        $scope.listClass = 'col-lg-3 listbox-hide lead-listbox';
        $scope.showDetails = true;
    }

    $scope.convertToAccount = function (id) {

        $http.get(baseUrl + '/leads/getvmlead/' + id).success(function (data) {
            $rootScope.isConvertFromLead = true;
            $rootScope.currentLead = data.Model;
            $location.path('/accounts');
        });
    }

    $scope.addTask = function (id) {
        $scope.addingToTask = true;
        $http.get(baseUrl + '/leads/getvmlead/' + id).success(function (data) {
            $rootScope.isFromPractice = true;
            $rootScope.taskPracticeId = data.Model.PracticeId;
            $location.path('/tasks');
            $scope.addingToTask = false;
        });
    }

    $scope.init = function () {

        $rootScope.authData = authService.authentication;

        $cookies.put('location', 'leads');
        $scope.loadLeads();
        console.log('-----leadController-----')

        $rootScope.controller = 'Leads';
        $scope.listClass = 'col-lg-12';
        $scope.showFilter = false;
        $scope.showDetails = false;
        $scope.affectedFilters = [];
        $scope.leadFilter = null;

        $scope.show = {
            Age: true,
            Name: true,
            Rep: true,
            Group: true,
            Status: true
        };

        $scope.select2Options = {
            allowClear: true
        }

        $scope.$on('newLeadFloating', function (event, args) {
            $scope.newLead();
        });
        if ($rootScope.showNewLead && $rootScope.showNewLead == true) {
            $rootScope.showNewLead = false;
            $scope.newLead()
        }

        $scope.loadLookUps();
        $scope.resetFilter(true);
        //$scope.$watch('filter.RepGroupIds', function (value) {
        //    $http.post(baseUrl + '/lookup/getallreps', value).success(function (data) {
        //        $scope.lookUps.lookupReps = data.Model.List;
        //    });
        //});
    }

    $scope.$watch('currentLead', function (value) {
        if ($scope.model && $scope.currentLead != undefined) {

            var length = $scope.model.List.length;
            for (var i = 0; i < length; i++) {
                if ($scope.model.List[i].LeadId == $scope.currentLead.LeadId) {
                    $scope.model.List[i] = $scope.currentLead;
                    break;
                }
            }
        }
    });

    $scope.hideDetailsView = function () {
        $scope.showDetails = false;
        $scope.listClass = 'col-lg-12';
        $scope.showFilter = false;
        $scope.currentLead = null;
    }

    //filter view on/off
    $scope.toggleFilter = function () {

        if ($scope.showFilter) {
            $scope.showFilter = false;
            $scope.listClass = 'col-lg-12';
        }
        else {
            $scope.showFilter = true;
            $scope.listClass = 'col-lg-9';
            $scope.showDetails = false;
            $scope.currentLead = null;
        }
    }

    $scope.loadLeads = function () {
        $scope.leadLoading = true;

        $http.post(baseUrl + '/leads/getbyfilter', $scope.leadFilter).success(function (data) {
            $scope.model = data.Model;
            $scope.leadLoading = false;
        });
    }

    $scope.editLead = function (id) {
        $scope.loadingEdit = true;
        $scope.newLead(id);
    }

    $scope.newLead = function (id) {
        if (id == undefined)
            $scope.addingLead = true;

        $http.get(baseUrl + '/leads/getvmlead/' + id).success(function (data) {
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
            $scope.editModel = data.Model;
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/ControlPanel/Views/Lead/addLead.html?4445',
                controller: 'newLeadController',
                backdrop: 'static',
                keyboard: false,
                size: 'lg',
                resolve: {
                    leadId: function () {
                        return id;
                    },
                    lookUps: function () {
                        return $scope.lookUps;
                    },
                    leadData: function () {
                        return data.Model
                    }
                }
            });
            $scope.loadingEdit = false;
            modalInstance.rendered.then(function () {
                $scope.addingLead = false;
            });

            modalInstance.opened.then(function () {
                $scope.addingLead = false;
            });

            modalInstance.result.then(function (result) {

                if (id == undefined) {
                    //$scope.resetFilter();
                    $scope.loadLeads();
                }
                //else if($scope.currentLead!=null)
                {
                    $scope.currentLead = result;

                    $scope.currentNotePage = 1;
                    $scope.loadNotes();
                }
            }, function (e) {
            });

        });
    }

    $scope.note = { Message: '' };
    $scope.addNote = function () {

        if ($scope.note.Message != '') {
            $scope.loadingNotes = true;
            $http.post(baseUrl + '/leads/' + $scope.currentLead.Practice.Id + '/save', { Description: $scope.note.Message, ParentTypeId: 1, ParentId: $scope.currentLead.Practice.Id }).success(function (data) {
                $scope.note.Message = "";
                $scope.currentNotePage = 1;
                $scope.currentLead.Notes = null;
                $scope.loadNotes();
            });
        }
    }

    $scope.moreNotes = false;
    $scope.currentNotePage = 1;
    $scope.loadNotes = function () {
        $scope.loadingNotes = true;
        //$http.post(baseUrl + '/notes/all', { ParentTypeId: 1, ParentId: $scope.currentLead.Practice.Id, CurrentPage: $scope.currentNotePage, PageSize: 10 }).success(function (data) {
        $http.post(baseUrl + '/leads/' + $scope.currentLead.Practice.Id + '/notes', { ParentTypeId: 1, CurrentPage: $scope.currentNotePage, PageSize: 10 }).success(function (data) {
            if ($scope.currentLead.Notes == null)
                $scope.currentLead.Notes = data.Model.List;
            else {
                var oldList = $scope.currentLead.Notes;
                $scope.currentLead.Notes = null;
                $scope.currentLead.Notes = data.Model.List;
                $scope.currentLead.Notes.push.apply($scope.currentLead.Notes, oldList);
            }
            $scope.count = data.Model.Pager.TotalCount;
            $scope.loadingNotes = false;
            $scope.moreNotes = data.Model.Pager.TotalPage > $scope.currentNotePage;
            $scope.currentNotePage++;
        });
    }

    $scope.createpdf = function (Id) {
        //$scope.LeadViewpdf = true;
        //$http.post(baseUrl + '/Home/pdf', { Id: Id, IsAccount: false }, { responseType: 'arraybuffer' }).success(function (data) {
        //    $scope.LeadViewpdf = false;
        //    var file = new Blob([data], { type: 'application/pdf' });
        //    var fileURL = URL.createObjectURL(file);
        //    window.open(fileURL);
        //});

        var win = window.open('', '_blank');
        win.location.href = baseUrl + '/lead/Pdf/' + Id;
    };

    $scope.setSelectedColumn = function (columnName) {
        $scope.leadFilter.orderBy = ($scope.leadFilter.orderBy == null || $scope.leadFilter.orderBy == 'desc') ? 'asc' : ($scope.leadFilter.sortKey == columnName && $scope.leadFilter.orderBy == 'asc') ? 'desc' : 'asc';
        $scope.leadFilter.sortKey = columnName;
    };

    $scope.deleteNote = function (note) {
        confirm("Do you want to delete the note? ", function (flag) {
            if (flag) {
                $scope.loadingDeleteStatus = true;
                $http.post(baseUrl + '/leads/1/delete/' + note.Id).success(function (data) {
                    if (data.IsSuccess) {
                        var index = $scope.currentLead.Notes.indexOf(note);
                        $scope.currentLead.Notes.splice(index, 1);
                    }
                });
            }
        });
    }

}]);

careconnect.controller('notesController', function ($scope, $http, $uibModalInstance, leadId, valueService, lookUps, leadData) {
    $scope.CloseModal = function (e) {
        $uibModalInstance.dismiss('cancel');
    }
})

careconnect.controller('newLeadController', function ($scope, $http, $uibModalInstance, leadId, valueService, lookUps, leadData, $rootScope) {

    $scope.applyPlaceLabel = function () {
    }

    $scope.init = function () {
        //if (leadData.Providers == null || leadData.Providers.length == 0)
        //    $scope.newProvider();
    };
    $scope.lookUps = lookUps;
    $scope.currentLead = leadData;

    $scope.newLocation = function () {
        if ($scope.currentLead.Locations == null) {
            $scope.currentLead.Locations = [];
        }
        $http.get(baseUrl + '/leads/getlocationobject').success(function (data) {
            data.AddressIndex = $scope.currentLead.Locations.length;
            $scope.currentLead.Locations.push(data);
        });
    }

    $scope.removeLocation = function (index) {
        $scope.currentLead.Locations.splice(index, 1);
    }

    $scope.newProvider = function () {

        if ($scope.currentLead.Providers == null) {

            $scope.currentLead.Providers = [];
        }
        $http.get(baseUrl + '/leads/getproviderobject').success(function (data) {
            data.IsPracticeLoc = 1;
            $scope.currentLead.Providers.push(data);
        });
    }

    $scope.ProviderDisable = function () {
        alert("Please enter Primary Location to add a provider");
    }

    $scope.removeProvider = function (index) {
        $scope.currentLead.Providers.splice(index, 1);
    }

    $scope.getLead = function (id) {
        $scope.currentLead = null;
        $http.get(baseUrl + '/leads/getvmlead/' + id).success(function (data) {
            $scope.currentLead = data.Model;

            //if (data.Model.Providers == null || data.Model.Providers.length == 0)
            //    $scope.newProvider();
        });
    }

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

    $scope.submitLead = function () {
        $('.ng-invalid:visible:first').focus();
        if ($scope.leadForm.$valid == false) {
            $scope.errorMessege = [];
            angular.forEach($scope.leadForm.$error, function (value, key) {
                angular.forEach(value, function (value1, key1) {
                    $scope.errorMessege.push(value1.$name + ' is ' + key);
                })
            });
            alert($scope.errorMessege);
            return false;
        }

        if ($scope.currentLead == null) {
            return false;
        }

        $scope.leadSubmitting = true;
        $scope.currentLead.ContactPreferenceId = 1;

        var locationArray = [];
        var addressObj = $scope.currentLead.Locations;
        for (key in addressObj) {
            locationArray.push(addressObj[key]);
        }
        $scope.currentLead.Locations = locationArray;

        var providerArray = [];
        var providerObj = $scope.currentLead.Providers;
        for (key in providerObj) {
            if (providerObj[key].IsPracticeLoc == 1) {
                providerObj[key].Location = null;
            }
            providerArray.push(providerObj[key])
            ;
        }
        $scope.currentLead.Providers = providerArray;

        $http.post(baseUrl + '/leads/save', $scope.currentLead).success(function (data) {
            $scope.leadSubmitting = false;
            if (data.Status == 200 || data.Status == 201) {
                $uibModalInstance.close(data.Model);
            }
            else if (data.Status == 400) {
                $scope.errorMessege = [];
                angular.forEach(data.Model, function (value, key) {
                    $scope.errorMessege.push(value.Message);
                });
                $scope.isArray = ($scope.errorMessege.constructor === Array);
                confirm($scope.errorMessege, function (flag) {
                    if (flag) {
                        $scope.currentLead.PracticeAddressLine1 = null;
                        $scope.currentLead.City = null;
                        $scope.currentLead.StateId = null;
                        $scope.currentLead.Zip = null;
                        $scope.currentLead.PhoneNumber = null;
                        $scope.submitLead();
                    }
                }, true);
            }
            else if (data.Status == 412) {
                $scope.errorMessege = [];
                angular.forEach(data.ResponseMessage, function (value, key) {
                    $scope.errorMessege.push(value.Message);
                });
                $scope.isArray = ($scope.errorMessege.constructor === Array);
                alert($scope.errorMessege);
            }
            else {
                scrollToInvalid();
            }
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

    $rootScope.$on('$routeChangeSuccess',
       function (event, toState, toParams, fromState, fromParams) {
           $uibModalInstance.dismiss('cancel');
       }
    );

    $scope.init();
    //$scope.getLead(leadId);

    //$scope.newProvider();
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

