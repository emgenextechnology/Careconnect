
careconnect.controller('tasksController', ['$scope', '$http', '$route', '$uibModal', 'valueService', '$rootScope', '$window', '$routeParams', 'authService', '$location', '$timeout', '$cookies',
    function ($scope, $http, $route, $uibModal, valueService, $rootScope, $window, $routeParams, authService, $location, $timeout, $cookies) {

        $scope.routeParams = $routeParams;
        $scope.TaskOldData=null;
        $scope.hitLastPage = false;

        $scope.reloadPage = function () {
            $rootScope.closeModals();
            if ($location.path().indexOf('tasks') > 0)
            { $route.reload(); }
        }
        $scope.today = new Date().toISOString();
        $scope.isDateFuture = function (value) {
            var date = moment(value)
            var now = moment();

            if (now > date) {
                return true;
            } else {
                return false;
            }
        }

        $rootScope.fileType = function (mimeType) {
            switch (mimeType) {

                case 'image/jpeg':
                case 'jpeg':
                case 'image/png':
                case 'png':
                case 'image/jpg':
                case 'jpg':
                    return '-image-o';

                case 'application/pdf':
                case 'pdf':
                    return '-pdf-o';

                case 'application/vnd.ms-excel':
                case 'xls':
                case 'xlsx':
                    return '-excel-o';

                case 'text/plain':
                case 'txt':
                    return '-text-o';

                default:
                    return '';
            }
        }

        $rootScope.removeAttachment = function (fileObject) {
            if (fileObject != undefined)
                confirm("Do you want to delete the file? ", function (flag) {
                    if (flag) {
                        $http.post(baseUrl + '/Tasks/deletefile/' + fileObject.Id, {}).success(function (data) {
                            $scope.currentTask.FilesList.pop(fileObject);
                        });
                    };
                });
        }

        var taskId = "";
        //{ Periods: ["aaa", "-7"], Testing: ["one", "three"] };

        $rootScope.lookUps = {
            lookupPeriods: null,
            lookupGroups: null,
            lookupTaskUsers: null,
            lookupTaskStatus: null,
            lookupPracticeList: null,
            TaskTypes: null,
            TaskPriorities: null,
        };

        $scope.statusList = [
        { id: 1, name: 'New' },
        { id: 2, name: 'In Progress' },
        { id: 3, name: 'Completed' }
        ];

        $scope.dueDateSpinner = false;
        $scope.alertDateSpinner = false;

        $scope.show = {
            RX: true,
            RequestType: true,
            RequestedBy: true,
            AssignedTo: true,
            Status: true,
            DueOn: true
        };

        $scope.select2Options = {
            allowClear: true
        }

        $scope.affectedFilterCount = 0;
        $scope.affectedFilters = [];

        var mytimeout;
        $scope.startTimeout = function () {
            $scope.startCount = $scope.startCount + 1;
            mytimeout = $timeout(function () { $scope.loadTasks() }, 100);
        }

        $scope.stopTimeout = function () {
            $timeout.cancel(mytimeout);
        }
        var taskWatch;

        $scope.resetFilter = function (isFirstLoading) {
            //$("select").val('').trigger("change");

            if (taskWatch)
                taskWatch();

            $http.get(baseUrl + '/tasks/getfilter').success(function (data) {

                if ($scope.routeParams.taskid) {
                    data.KeyWords = $scope.routeParams.taskid;
                    $scope.routeParams.taskid = null;
                    isFirstLoading = false;
                }

                $scope.tasksFilter = data;
                $scope.tasksFilter.CurrentPage = 1;
                $scope.tasksFilter.PageSize = 25;

                taskWatch = $scope.$watch('tasksFilter', function (value, oldValue) {
                    $scope.affectedFilterCount = 0;
                    $scope.affectedFilters = [];
                    angular.forEach(value, function (v, key) {
                        if (key != 'PageSize' && key != 'CurrentPage' && key != 'Take') {
                            if (key == 'ReferenceNumber') {
                                if (v != null && v != '' && v != ' ') {
                                    setFilterText(key, v);
                                }
                            }
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
        }

        function setFilterText(key, value) {
            if (key == 'ReferenceNumber') {
                $scope.affectedFilters.push(value);
            }
            else if (key == 'Keyword') {
                $scope.affectedFilters.push(value);
            }
            else if (key == 'RequestType') {
                angular.forEach($scope.lookUps.TaskTypes, function (v, k) {
                    if (v.Id == value)
                        $scope.affectedFilters.push(v.Value);
                });
            }
            else if (key == 'Status') {
                angular.forEach($scope.lookUps.lookupTaskStatus, function (v, k) {
                    if (v.Id == value)
                        $scope.affectedFilters.push(v.Value);
                });
            }
            else if (key == 'DueOn') {
                $scope.affectedFilters.push(value);
            }
            else if (key == 'RequestedBy') {
                angular.forEach($scope.lookUps.lookupTaskUsers, function (v, k) {
                    if (v.Id == value)
                        $scope.affectedFilters.push('From:' + v.Value);
                });
            }
            else if (key == 'AssignedTo') {
                angular.forEach($scope.lookUps.lookupTaskUsers, function (v, k) {
                    if (v.Id == value)
                        $scope.affectedFilters.push('To:' + v.Value);
                });
            }
            else if (key == 'Priority') {
                angular.forEach($scope.lookUps.TaskPriorities, function (v, k) {
                    if (v.Id == value)
                        $scope.affectedFilters.push(v.Value);
                });
            }
            else if (key == 'AssignedOrRequest') {
                angular.forEach($scope.lookUps.RequestOrAssigned, function (v, k) {
                    if (v.Id == value)
                        $scope.affectedFilters.push(v.Value);
                });
            }
        }

        $scope.showEdit = function (data) {

            if (data.CurrentUserId == data.RequestedUser.UserId || $rootScope.showByPrevilege('EDTTSKALL')) {
                return true;
            }

            return false;
        }

        $scope.showUpdateStatusSelect = function (data) {
            if (!data)
                return;

            if ($scope.getIdArray(data.AssignedUsersList).indexOf(data.CurrentUserId)) {//data.CurrentUserId == data.RequestedUser.UserId || 
                return true;
            }

            return false;
        }
        $scope.RequestOrAssigned = [{ Id: 1, Value: "Assigned To Me" }, { Id: 2, Value: "Requested By Me" }];
        $scope.loadLookUps = function () {

            if ($rootScope.lookUps.lookupPeriods == null) {
                $http.get(baseUrl + '/lookup/getallperiods').success(function (data) {
                    $rootScope.lookUps.lookupPeriods = data.Model.List;
                });
            }


            if ($rootScope.lookUps.lookupTaskUsers == null) {
                $http.post(baseUrl + '/lookup/getalltaskusers').success(function (data) {
                    //$http.post(baseUrl + '/lookup/getallrepsbysamegroup').success(function (data) {
                    $rootScope.lookUps.lookupTaskUsers = data.Model.List;
                });
            }

            if ($rootScope.lookUps.lookupTaskStatus == null) {
                $http.get(baseUrl + '/lookup/getalltaskstatuses').success(function (data) {
                    $rootScope.lookUps.lookupTaskStatus = data.Model.List;
                });
            }

            if ($rootScope.lookUps.TaskTypes == null) {
                $http.get(baseUrl + '/lookup/getalltasktypes').success(function (data) {
                    if (data.Model.List.length > 0)
                        data.Model.List.push({ Id: -1, Value: "None" });
                    $rootScope.lookUps.TaskTypes = data.Model.List;
                });
            }

            //if ($rootScope.lookUps.TaskPriorities == null) {
            //    $http.get(baseUrl + '/lookup/getalltaskpriorities').success(function (data) {
            //        $rootScope.lookUps.TaskPriorities = data.Model.List;
            //    });
            //}

            if ($rootScope.lookUps.lookupPracticeList == null) {
                $http.post(baseUrl + '/Practice/All', {}).success(function (data) {
                    $rootScope.lookupPracticeList = data.Model.List;
                });
            }
        }

        $scope.loadTasks = function () {
            $scope.tasksLoading = true;

            $http.post(baseUrl + '/tasks/getbyfilter', $scope.tasksFilter).success(function (data) {
                $scope.hitLastPage = $scope.tasksFilter != null && $scope.tasksFilter.CurrentPage == data.Model.Pager.TotalPage;
                $scope.model = data.Model;
                if (data.Model != null && data.Model.List != null && data.Model.List.length > 0) {
                    $scope.showDetailsView(data.Model.List[0]);
                }
                $scope.tasksLoading = false;
            });
        }

        $scope.showDetailsView = function (task) {
            taskId = task.TaskId;
            $window.scrollTo(0, 0);
            $scope.loadingDetails = true;

            $scope.setFlag = function () {
                $scope.loadingFlagUpdate = true;
                $http.post(baseUrl + '/tasks/toggleflag/' + task.TaskId).success(function (data) { $scope.currentTask.HasFlag = data.Model; $scope.loadingFlagUpdate = false; });
            }

            task.AlertDate = dateFix(task.AlertDate);
            task.TargetDate = dateFix(task.TargetDate);

            $scope.currentTask = task;
            var loadingStatusView = false;
            $scope.Notes = null;
            $scope.currentPage = 1;
            $scope.loadNotes();

            $scope.showDetails = true;
        }

        $scope.dueDateChange = function (taskId, targetDate) {
            $scope.dueDateSpinner = true;
            //$http.post(baseUrl + '/tasks/saveduedate', { TargetDate: toLocal(targetDate), TaskId: taskId }).success(function (data) {
            $http.post(baseUrl + '/tasks/saveduedate', { TargetDate: targetDate, TaskId: taskId }).success(function (data) {
                if (data.Status == 200)
                    $scope.dueDateSpinner = false;
            });
        }

        $scope.alertDateChange = function (taskId, alertDate) {
            $scope.alertDateSpinner = true;
            //$http.post(baseUrl + '/tasks/savealertdate', { AlertDate: toLocal(alertDate), TaskId: taskId }).success(function (data) {
            $http.post(baseUrl + '/tasks/savealertdate', { AlertDate: alertDate, TaskId: taskId }).success(function (data) {
                if (data.Status == 200)
                    $scope.alertDateSpinner = false;
            });
        }

        $scope.hideDetailsView = function () {
            $scope.showDetails = false;
            //$scope.listClass = 'col-lg-12';
            $scope.showFilter = false;
            $scope.currentTask = null;

        }

        $scope.updateStatus = function (value) {
            $scope.loadingStatusView = true;
            $http.post(baseUrl + '/tasks/setstatus/' + $scope.currentTask.TaskId + '/' + value).success(function (data) {
                $scope.currentTask.StatusId = value;
                $scope.loadingStatusView = false;

            });
        };

        $scope.deleteTask = function (task) {

            //if (currentAccount == undefined)
            //    if ($scope.currentAccount != null)
            //        currentAccount = $scope.currentAccount;


            //if (currentAccount.IsActive) {
            confirm("Do you want to delete the task? ", function (flag) {
                if (flag) {
                    $scope.loadingDeleteStatus = true;
                    $http.post(baseUrl + '/Tasks/delete/' + task.TaskId).success(function (data) {

                        $scope.loadingDeleteStatus = false;
                        var currentTaskList = $scope.model.List;
                        var length = currentTaskList.length;
                        for (var i = 0; i < length; i++) {
                            if (currentTaskList[i].TaskId == task.TaskId) {

                                $scope.model.List.splice(i, 1);

                                if (currentTaskList[i] != null) {
                                    $scope.currentTask = currentTaskList[i];
                                }
                                else if (currentTaskList[i - 1] != null) {
                                    $scope.currentTask = currentTaskList[i - 1];
                                }
                                else {
                                    $scope.currentTask = null;
                                }
                                break;
                            }
                        }



                        //$scope.loadTasks();
                    });
                }
            });
            //}
            //else {
            //    $scope.loadingDeleteStatus = true;
            //    $http.post(baseUrl + '/accounts/togglestatus/' + currentAccount.Id).success(function (data) {
            //        if ($scope.currentAccount)
            //            $scope.currentAccount.IsActive = data.Model;
            //        $scope.loadingStatusUpdate = false;
            //        $scope.loadAccounts();
            //    });
            //}

        }

        $scope.toggleFilter = function () {

            if ($scope.showFilter) {
                $scope.showFilter = false;
                //$scope.listClass = 'col-lg-12';
                $scope.detailViewClass = 'col-md-9';
            }
            else {
                $scope.detailViewClass = 'col-md-6';
                $scope.showFilter = true;
                //$scope.listClass = 'col-lg-9';
            }

            $timeout(function () { $scope.setHeight(); }, 3000);
        }

        $scope.editTask = function (item) {
            $scope.loadingEdit = true;
            var currentTask = "";
            $http.get(baseUrl + '/Tasks/GetTaskById/' + item.TaskId).success(function (data) {
                if (data != null && data.Model != null) {
                    if (data.Model.TaskRequestTypeId == null)
                        data.Model.TaskRequestTypeId = -1;
                }
                currentTask = data.Model;

                if (data != null && data.Model != null && data.Model.PriorityTypeId != null && data.Model.PriorityTypeId == 1)
                    currentTask.setHighpriority = "btn active";

                //datefix tweak
                currentTask.AlertDate = dateFix(currentTask.AlertDate);
                currentTask.TargetDate = dateFix(currentTask.TargetDate);

                $scope.loadingDetails = false;
                $scope.currentTask = currentTask;

                $scope.TaskOldData = angular.copy(currentTask);
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: '/ControlPanel/Views/Tasks/addTask.html?16',
                    controller: 'newTaskController',
                    backdrop: 'static',
                    keyboard: true,
                    size: 'lg',
                    resolve: {
                        taskData: function () {
                            return currentTask;
                        },
                        lookUps: function () {
                            return $scope.lookUps;
                        },
                        taskId: function () {
                            return 0;
                        }
                    }
                });
                $scope.loadingEdit = false;
                modalInstance.result.then(function (model) {

                    $scope.currentTask = model;
                    $scope.loadTasks();
                }, function () {
                    //callback(false);
                });
                $scope.currentPage = 1;
                $scope.Notes = null;
                $scope.loadNotes();
            });
        }

        $scope.taskLog = function (id) {
            $scope.loadingLog = true;
            $http.get(baseUrl + '/Tasks/gettasklog/' + id).success(function (data) {
                $scope.loadingLog = false;
                //currentTask = data.Model;
                //$scope.loadingEdit = false;
                //$scope.currentTask = currentTask;
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: '/ControlPanel/Views/Tasks/taskLog.html?230',
                    controller: 'taskLogContgroller',
                    backdrop: 'static',
                    keyboard: true,
                    size: 'lg',
                    resolve: {
                        taskLogModel: function () {
                            return data.Model;
                        }
                    }
                });
            });
        }

        $scope.newTask = function (id) {
            $scope.TaskOldData = null;
            var data = { Model: null };
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/ControlPanel/Views/Tasks/addTask.html?533',
                controller: 'newTaskController',
                backdrop: 'static',
                keyboard: true,
                size: 'lg',
                resolve: {
                    taskId: function () {
                        return id;
                    },
                    lookUps: function () {
                        return $scope.lookUps;
                    },
                    taskData: function () {
                        return data.Model
                    }
                }
            });

            modalInstance.rendered.then(function () {
                $scope.$emit('$tinymce:refresh');
            });


            modalInstance.result.then(function (flag) {
                $scope.loadTasks();
            }, function () {
                //callback(false);
            });
        }

        $scope.moreNotes = false;
        $scope.currentPage = 1;
        $scope.loadNotes = function () {
            $scope.loadingNotes = true;
            //$http.post(baseUrl + '/notes/all', { ParentTypeId: 3, ParentId: taskId, CurrentPage: $scope.currentPage, PageSize: 10 }).success(function (data) {
            $http.post(baseUrl + '/tasks/' + taskId + '/notes', { ParentTypeId: 3, CurrentPage: $scope.currentPage, PageSize: 10 }).success(function (data) {

                if ($scope.Notes == null)
                    $scope.Notes = data.Model.List;
                else {
                    var oldList = $scope.Notes;
                    $scope.Notes = null;
                    $scope.Notes = data.Model.List;
                    $scope.Notes.push.apply($scope.Notes, oldList);
                }

                $scope.loadingNotes = false;
                $scope.moreNotes = data.Model.Pager.TotalPage > $scope.currentPage;
                $scope.currentPage++;
            });
        }

        $scope.note = { Message: '' }

        $scope.addNote = function () {

            if ($scope.note.Message != '') {
                $scope.loadingNotes = true;
                //$http.post(baseUrl + '/notes/save', { Description: $scope.note.Message, ParentTypeId: 3, ParentId: $scope.currentTask.TaskId }).success(function (data) {
                $http.post(baseUrl + '/tasks/' + $scope.currentTask.TaskId + '/save', { Description: $scope.note.Message, ParentTypeId: 3, ParentId: $scope.currentTask.TaskId }).success(function (data) {
                    $scope.note.Message = "";
                    $scope.currentPage = 1;
                    $scope.Notes = null;
                    $scope.loadNotes();
                });
            }
        }

        $scope.setPriority = function () {
            $scope.loadingFlagUpdate = true;
            $http.post(baseUrl + '/tasks/togglepriority/' + $scope.currentTask.TaskId).success(function (data) {
                $scope.currentTask.PriorityTypeId = data.Model;
                $scope.loadingFlagUpdate = false;
                $scope.stopTimeout();
                $scope.startTimeout();
            });
        }

        if ($rootScope.isFromPractice == true) {
            $scope.newTask();
        }

        $scope.$watch('currentTask', function (value) {
            if ($scope.model && $scope.currentTask != undefined) {

                var length = $scope.model.List.length;
                for (var i = 0; i < length; i++) {
                    if ($scope.model.List[i].TaskId == $scope.currentTask.TaskId) {
                        $scope.model.List[i] = $scope.currentTask;
                        break;
                    }
                }
            }

        });

        $scope.getNameArray = function (usersList) {
            var array = [];
            angular.forEach(usersList, function (value, key) {
                array.push(value.Name);
            })
            return array.join(', ');
        };

        $scope.getIdArray = function (usersList) {
            var array = [];
            angular.forEach(usersList, function (value, key) {
                array.push(value.UserId);
            })
            return array.join(', ');
        };

        $scope.init = function () {

            if (!$scope.routeParams.taskid)
                $scope.loadTasks();

            $rootScope.authData = authService.authentication;
            $scope.isCollapsed = true;
            $rootScope.controller = 'Tasks';
            //$scope.listClass = 'col-lg-12';
            $scope.detailViewClass = 'col-md-9';
            $scope.showFilter = false;
            $scope.showDetails = false;
            $scope.practiceTypeSpinner = false;
            $scope.affectedFilters = [];
            $cookies.put('location', 'tasks');
            $scope.loadLookUps();

            $scope.resetFilter(true);



            $scope.$on('newTaskFloating', function (event, args) {
               $rootScope.showNewTask = true;
            });

            if ($rootScope.showNewTask == true) {
                $rootScope.showNewTask = false;
                $scope.newTask();
            }

            $scope.taskListCurrentPage = 1;
            $("#taskListView").niceScroll({ cursorborder: "", cursorcolor: "#333" });
            $(".taskFilterView").niceScroll({ cursorborder: "", cursorcolor: "#333" });
            var scrollingDiv = $("#taskListView");
            var lastScroll = scrollingDiv.scrollTop();
            var SCROLL_BUFFER = 50;
            scrollingDiv.scroll(function () {
                var newScroll = scrollingDiv.scrollTop();
                var delta = newScroll - lastScroll;
                lastScroll = newScroll;
                var scrollHeight = scrollingDiv[0].scrollHeight;
                var maxScroll = scrollHeight - scrollingDiv[0].clientHeight;

                if (delta > 0 && maxScroll - newScroll < SCROLL_BUFFER) {
                    if ($scope.tasksLoading != true && $scope.hitLastPage != true) {
                        $scope.tasksLoading = true;
                        $scope.taskListCurrentPage++;
                        $http.post(baseUrl + '/tasks/getbyfilter', { CurrentPage: $scope.taskListCurrentPage, PageSize: 25 }).success(function (data) {
                            $scope.hitLastPage = $scope.taskListCurrentPage == data.Model.Pager.TotalPage;
                            $scope.Notes.push.apply($scope.model.List, data.Model.List);
                            $scope.tasksLoading = false;
                        });
                    }
                }
            });
        }

        $scope.toLocalTime = function toLocalTime(date) {
            //alert(date);
            try {
                var local = new Date(date);
                local.setMinutes(date.getMinutes() - date.getTimezoneOffset());
                return local;
            }
            catch (e) {
                return date;
            }
        }

        $scope.setHeight = function () {
            var bodyHeight = $("body").outerHeight(),
                headerHeight = $("header").outerHeight(),
                taskHeaderHeight = $(".lead-top-search").outerHeight(),
                detailViewHeaderHeight = $(".tsk-dtl-top").outerHeight(),
                      leftSidebarHeight = $('.left-sidebar > ul').outerHeight();

            var taskListHeight = bodyHeight - (headerHeight + taskHeaderHeight),
                taskDetailHeight = bodyHeight - (headerHeight + taskHeaderHeight + detailViewHeaderHeight);

            $('#taskListView').height(taskListHeight - 4);
            $('.taskFilterView').height(taskListHeight - 40);
            $('#taskDetailView').height(taskDetailHeight - 30);
        }

        $scope.initTaskDetail = function () {
            $("#taskDetailView").niceScroll({ cursorborder: "", cursorcolor: "#333" });
            $(window).resize(function () { $scope.setHeight(); });

            $timeout(function () { $scope.setHeight(); }, 3000);
        }

        $scope.deleteNote = function (note) {
            confirm("Do you want to delete the note? ", function (flag) {
                if (flag) {
                    $scope.loadingDeleteStatus = true;
                    $http.post(baseUrl + '/tasks/3/delete/' + note.Id).success(function (data) {
                        if (data.IsSuccess) {
                            var index = $scope.Notes.indexOf(note);
                            $scope.Notes.splice(index, 1);
                        }
                    });
                }
            });
        }

        //$scope.renderHtml = function (htmlCode) {
        //    console.log('------------');
        //    return $sce.trustAsHtml(htmlCode);
        //};
        //, '$sce'
    }]);

careconnect.controller('taskLogContgroller', function ($scope, $http, $uibModalInstance, taskLogModel, $rootScope) {
    $rootScope.$on('$routeChangeSuccess',
      function (event, toState, toParams, fromState, fromParams) {
          $uibModalInstance.dismiss('cancel');
      }
  );
    $scope.data = taskLogModel;

    $scope.parseUserList = function (list) {
        var nameArray = [];
        var jsonArray = JSON.parse(list);
        angular.forEach(jsonArray, function (value, index) {

            nameArray.push(value.Name);

        });
        return nameArray.join(',');
    };

    $scope.CloseModal = function (e) {
        $uibModalInstance.dismiss('cancel');
    }
});

careconnect.controller('newTaskController', function ($scope, $http, $uibModalInstance, taskId, valueService, lookUps, $rootScope, authService, taskData, $rootScope) {
    $scope.TaskOldData = angular.copy(taskData);
    $scope.getallAccounts = function (searchKey) {
        if (searchKey != "")
            $scope.practiceTypeSpinner = true;
        $http.post(baseUrl + '/Practice/All', { KeyWord: searchKey }).success(function (data) {
            $rootScope.lookupPracticeList = data.Model.List;
            $scope.practiceTypeSpinner = false;
        });
    }

    $rootScope.$on('$routeChangeSuccess',
      function (event, toState, toParams, fromState, fromParams) {
          $uibModalInstance.dismiss('cancel');
      }
  );

    $scope.toSpinner = false;
    $scope.practiceSpinner = false;
    $scope.highPriorityClick = false;
    $scope.setHighpriority = "";

    //taskDataOld = angular.copy(taskData);

    var entity = {};
    if (taskData == null || taskData == undefined)
        $scope.currentTask = {};
    else
        $scope.currentTask = taskData;

    $scope.$watch("currentTask.AssignedTo", function (val) {

        //var isValid = true;
        //if (val == undefined)
        //    val = ["-1"];

        //if (isValid )
        //    val.forEach(function (v, i) {
        //        if ($.inArray(v, taskDataOld.AssignedTo) < 0) {
        //            isValid = false;
        //            return;
        //        }
        //        else
        //            isValid = true;
        //    });

        //$scope.taskForm.$pristine = isValid;

    }, true);

    $scope.base64Data = [];

    $scope.fileUpload = function (files) {
        var filesExceedsSize = new Array();
        for (var i = 0; i < files.length; i++) {
            var maxSize = 100000000;
            var fileSize = files[i].filesize;
            if (fileSize > maxSize) {
                //filesExceedsSize = filesExceedsSize == null ? files[i].filename : (filesExceedsSize + ', ' + files[i].filename);
                filesExceedsSize.push(files[i].filename);
            } else {
                $scope.base64Data.push(files[i]);
            }
        }

        if (filesExceedsSize.length > 0) {
            var message = filesExceedsSize.join(", ") + (filesExceedsSize.length == 1 ? ' is' : ' are') + ' exceeds 100 MB file limit.';
            alert(message);
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

    if ($rootScope.isFromPractice) {
        $scope.currentTask.PracticeId = $rootScope.taskPracticeId;
    }

    else if ($rootScope.lookupPracticeList == null) {

        $scope.practiceTypeSpinner = true;

        $http.post(baseUrl + '/Practice/All', {}).success(function (data) {
            $rootScope.lookupPracticeList = data.Model.List;
            $scope.practiceTypeSpinner = false;
        });
    }

    $scope.submitTask = function () {
        if ($scope.currentTask.AssignedTo == null || $scope.currentTask.Subject == null || $scope.currentTask.Subject == '' || $scope.currentTask.TaskDescription == null || $scope.currentTask.TaskDescription == '') {
            console.log("Errors");
            if ($scope.taskForm.$valid == false) {
                $scope.errorMessege = [];
                angular.forEach($scope.taskForm.$error, function (value, key) {
                    angular.forEach(value, function (value1, key1) {
                        $scope.errorMessege.push(value1.$name + ' is ' + key);
                    })
                });
                alert($scope.errorMessege);
            }
            return;
        }

        $scope.sendSpinner = false;
        $scope.currentTask.files = $scope.base64Data;
        $scope.currentTask.IsActive = true;

        if ($rootScope.isFromPractice) {
            $scope.currentTask.PracticeId = $rootScope.taskPracticeId;
        }

        //$scope.currentTask.AlertDate = toLocal($scope.currentTask.AlertDate);
        //$scope.currentTask.TargetDate = toLocal($scope.currentTask.TargetDate);

        if ($scope.currentTask.TaskRequestTypeId == -1) {
            $scope.currentTask.TaskRequestTypeId = null;
        }

        $http.post(baseUrl + '/Tasks/Save', $scope.currentTask).success(function (data) {
            $scope.leadSubmitting = false;
            if (data.Status == 200 || data.Status == 201) {
                $uibModalInstance.close(data.Model);
                $scope.currentTask.PracticeId = null;
                $rootScope.isFromPractice = false;
                $uibModalInstance.close();
            }

            $scope.sendSpinner = true;
        });
    }

    $scope.highpriorityImg = 'warning';

    $scope.setPriority = function () {

        $scope.taskForm.$setDirty();

        $('form[name="taskForm"]').addClass('unsaved-form');

        if ($scope.currentTask == null)
            $scope.currentTask = { PriorityTypeId: 3 }; //console.log($scope.currentTask.setHighpriority == undefined);
        if ($scope.currentTask.setHighpriority == undefined)
            $scope.currentTask.setHighpriority = "";

        if ($scope.currentTask.setHighpriority == "") {
            $scope.currentTask.setHighpriority = "btn active";
            $scope.highpriorityImg = 'white-warning';
            $scope.currentTask.PriorityTypeId = 1;
        }
        else {
            $scope.highpriorityImg = 'warning';
            $scope.currentTask.setHighpriority = "";
            $scope.currentTask.PriorityTypeId = 3;
        }
    }

    $scope.CloseModal = function (e) {
        $scope.currentTask.PracticeId = null;
        $rootScope.isFromPractice = false;

        if ($('.unsaved-form').length > 0) {
            confirm("Are you sure you want to leave this page with unsaved changes?", function (flag) {
                if (flag) {
                    //console.log($scope.TaskOldData);
                    //console.log($scope.currentTask);

                        angular.copy($scope.TaskOldData, $scope.currentTask);
                    //delete TaskOldData;
                    $uibModalInstance.dismiss('cancel');
                };
            });
        }
        else {
            $uibModalInstance.dismiss('cancel');
        }
    }
});
