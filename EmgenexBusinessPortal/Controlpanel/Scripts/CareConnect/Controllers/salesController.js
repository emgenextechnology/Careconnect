
careconnect.controller('salesController', function ($scope, $http, $route, $rootScope, $timeout, $window, authService, $routeParams, $cookies, $location, $filter, $interval) {

    $scope.routeParams = $routeParams;

    $scope.reloadPage = function () {

        if ($location.path().indexOf('sales') > 0) {
            $route.reload();
        }
        $rootScope.closeModals();

    };

    $rootScope.authData = authService.authentication;

    $rootScope.lookUps = {
        lookupProviders: null,
        lookupGroups: null,
        lookupReps: null,
        DashboardPeriods: null
    };

    $scope.salesFilter = {
        ServiceId: 0,
        GroupBy: 0,
        sortKey: null,
        orderBy: null
    };

    $scope.RidColumn = {
        ColumnName: "ReportId"
    }

    $scope.show = new Array();

    $scope.select2Options = {
        allowClear: true
    };

    $scope.select2OptionsDisable = {
        allowClear: false
    };

    $scope.isEditView = false;
    $scope.isAddView = false;
    $scope.toogleFinanceView = false;
    $scope.showLogView = false;

    $scope.listClass = 'col-lg-12';
    $scope.affectedFilterCount = 0;
    $scope.affectedFilters = [];
    $scope.isDisable = false;

    $scope.uploadingReport = false;

    $scope.selectedPeriod = null;

    $scope.toggleFilter = function () {
        if ($scope.showFilter) {
            $scope.showFilter = false;
            $scope.listClass = 'col-lg-12';
        }
        else {
            $scope.showFilter = true;
            $scope.listClass = 'col-lg-9';
            $scope.showDetails = false;
            $scope.currentSales = null;
        }
    }

    var salesFilterWatch;
    var salesColumnWatch;

    $scope.loadFilters = function () {

        $scope.hideDetailsView()

        if (salesFilterWatch)
            salesFilterWatch();
        if (salesColumnWatch)
            salesColumnWatch();

        if ($scope.salesFilter.ServiceId > 0) {

            var serviceId = $scope.salesFilter.ServiceId;
            $scope.model = null;

            $http.get(baseUrl + '/sales/getfilter/' + $scope.salesFilter.ServiceId).success(function (filterResponse) {

                $scope.salesFilter = filterResponse;
                $scope.salesFilter.ServiceId = serviceId;
                $scope.salesFilter.GroupBy = authService.authentication.SalesGroupBy;
                $scope.selectedPeriod = DefaultDateRange;
                if(DefaultDateRange)
                    setDateFiltersByPeriod(DefaultDateRange);

                angular.forEach(filterResponse.DynamicFilters, function (v, key) {
                    var columnName = v.ColumnName;
                    switch (columnName) {
                        case "Provider":
                            $scope.providerPlchldr = v.DisplayName;
                            break;
                        case "SpecimenId":
                            $scope.specimenPlchldr = v.DisplayName;
                            break;
                        case "CollectedDate":
                            $scope.collectedDatePlchldr = v.DisplayName;
                            break;
                        case "ReceivedDate":
                            $scope.receivedDatePlchldr = v.DisplayName;
                            break;
                        case "SalesTeam":
                            $scope.salesTeamPlchldr = v.DisplayName;
                            break;
                        case "RepName":
                            $scope.repPlchldr = v.DisplayName;
                            break;
                        case "Practice":
                            $scope.practicePlchldr = v.DisplayName;
                            break;
                        case "Patient":
                            $scope.patientPlchldr = v.DisplayName;
                            break;
                    }
                });

                salesColumnWatch = $scope.$watch('salesFilter.DynamicFilters', function (value, oldValue) {
                    angular.forEach(value, function (v, key) {
                        $scope.salesFilter.Columns[v.ColumnName] = v.IsVisible;
                    });
                }, true);

                salesFilterWatch = $scope.$watch('salesFilter', function (value, oldValue) {
                    var flag = false;
                    $scope.affectedFilterCount = 0;
                    $scope.affectedFilters = []; //console.log(value);console.log($scope.showLogView)
                    angular.forEach(value, function (v, key) { 
                        if (key != 'Columns' && key != 'PageSize' && key != 'CurrentPage' && key != 'Skip' && key != 'Take' && key != 'ServiceId') {
                            if (v != null && v != [] && v != '' && v >= 0) {
                                if ($scope.showLogView == true) {
                                    if (key == 'LogDateFrom' || key == 'LogDateTo') {
                                        $scope.affectedFilterCount = $scope.affectedFilterCount + 1;
                                        setFilterText(key, v);
                                    }
                                } else {
                                    if (key != 'LogDateFrom' && key != 'LogDateTo') {
                                        $scope.affectedFilterCount = $scope.affectedFilterCount + 1;
                                        setFilterText(key, v);
                                    }
                                }
                            }
                        }
                    });
                    if ($scope.model != null && $scope.model.List)
                        $scope.model.List = null;
                    $scope.stopTimeout();
                    $scope.startTimeout();
                }, true);
            });
        }

        //$scope.select2Options = {
        //    allowClear: true
        //};
    }

    $scope.loadLookUps = function () {

        if ($rootScope.lookUps.lookupGroups == null) {
            $http.get(baseUrl + '/lookup/getallrepgroups').success(function (data) {
                if (data.Model) {
                    $rootScope.lookUps.lookupGroups = data.Model.List;
                }
            });
        }

        if ($rootScope.lookUps.lookupReps == null) {
            $http.post(baseUrl + '/lookup/getallreps').success(function (data) {
                if (data.Model) {
                    $rootScope.lookUps.lookupReps = data.Model.List;
                }
            });
        }

        if ($rootScope.lookUps.lookupServices == null) {
            $http.get(baseUrl + '/lookup/getallservices').success(function (data) {
                var serviceId = data.Model.List[0].Id;
                if (data.Model) {
                    $rootScope.lookUps.lookupServices = data.Model.List;
                }
                $scope.salesFilter.ServiceId = serviceId;
            });
        }
        else {
            $scope.salesFilter.ServiceId = $rootScope.lookUps.lookupServices.Id;
        }

        if ($rootScope.lookUps.lookupSalesGroupbyValues == null) {
            $http.get(baseUrl + '/lookup/getallSalesGroupbyValues').success(function (data) {
                var groupBy = authService.authentication.SalesGroupBy;
                if (data.Model) {
                    $rootScope.lookUps.lookupSalesGroupbyValues = data.Model.List;
                }
            });
        }

        if ($scope.lookUps.DashboardPeriods == null) {
            $http.get(baseUrl + '/lookup/GetAllDashboardPeriods').success(function (data) {
                var list = data.Model.List;
                $scope.lookUps.DashboardPeriods = list;
            });
        }
    }

    $scope.getallproviders = function (searchKey) {
        if (searchKey != "")
            $http.get(baseUrl + '/lookup/getallproviders/' + searchKey).success(function (data) {
                if (data.Model) {
                    $rootScope.lookUps.lookupProviders = data.Model.List;
                }
            });
    }

    //$scope.configColumnVisibility = function (columnName) {
    //    angular.forEach($scope.salesFilter.DynamicFilters, function (column, index) {
    //        $scope.show["'" + column.columnName + "'"] == column.IsVisible;

    //        if (column.IsVisible) {
    //            return true;
    //        }
    //        return false;
    //    });
    //}

    $scope.drpOnSelect = function (drpItem, sales, type) {
        switch (type) {
            case 1://Practice
                sales.Practice = drpItem.Value;
                break;
            case 2://Provider
                sales.Provider = drpItem.Value;
                break;
        }

        $http.get(baseUrl + '/sales/ResolveSalesData/' + type + '/' + drpItem.Id).success(function (response) {
            angular.forEach(response.Model, function (v, key) {
                switch (v.DataType) {
                    case 1://Practice
                        if (v.SelectList.length == 1) {
                            sales.PracticeId = v.SelectList[0].Id;
                            sales.Practice = v.SelectList[0].Value;
                            $rootScope.lookupPracticeList = null;
                        }
                        else if (v.SelectList.length > 1) {
                            $rootScope.lookupPracticeList = v.SelectList;
                        }
                        break;
                    case 2://Provider
                        if (v.SelectList.length == 1) {
                            sales.ProviderId = v.SelectList[0].Id;
                            sales.Provider = v.SelectList[0].Value;
                            $rootScope.lookupProviderList = null;
                        }
                        else if (v.SelectList.length > 1) {
                            $rootScope.lookupProviderList = v.SelectList;
                        }
                        break;
                    case 3:
                        sales.RepId = v.RepId;
                        sales.RepName = v.RepName;
                        sales.RepGroup = v.RepGroupName;
                        break;
                }
            });
        });
    }

    $scope.drpDynColumnOnSelect = function (obj, columnId, value) {
        var reportId = obj.$select.$element[0].offsetParent.attributes["report-id"].value;
        var S1 = angular.element('#sales-row-' + reportId);
        var S2 = angular.element(S1.find('input[data-column-id=' + columnId + ']'));
        S2.val(value);
    }

    $scope.undoChanges = function (sales) {
        sales.Practice = null; sales.PracticeId = 0;
        sales.Provider = null; sales.ProviderId = 0;
        sales.RepGroup = null; sales.RepName = null; sales.RepId = 0
    }

    $scope.getallAccounts = function (searchKey) {
        if (searchKey != "")
            $scope.practiceTypeSpinner = true;
        $http.post(baseUrl + '/Practice/All', { KeyWord: searchKey }).success(function (data) {
            if (data.Model) {
                $rootScope.lookupPracticeList = data.Model.List;
            }
        });
    }

    $scope.getallProviders = function (searchKey) {
        if (searchKey != "")
            $scope.providerTypeSpinner = true;
        $http.get(baseUrl + '/lookup/getallproviders/' + searchKey).success(function (data) {
            if (data.Model) {
                $rootScope.lookupProviderList = data.Model.List;
            }
        });
    }

    $scope.isCritical = function (Message) {
        if (Message.match("Provider") || Message.match("Practice") || Message.match("Rep")) {
            return true
        } else {
            return false
        }
    }

    $scope.submitSalesCompose = function (parentId, isTr) {

        var _this = $('#' + parentId);

        if (_this.hasClass('posting') || $scope.isDisable)
            return;

        var sales = new Array();
        var trs = null;
        if (isTr === true) {
            if (_this.hasClass('invalid-data'))
                return;
            trs = _this;
        }
        else
            trs = _this.find('tbody#editBody>tr.tr-ident.valid-data');
        var objModel = JSON.parse('{}');

        if (isTr)
            _this.addClass('posting');
        $scope.isDisable = true;

        $('a.btn-save', _this).attr('disabled', true);

        $.each(trs, function (i, tr) {
            var inputs = $(tr).find('td>input[type=text],td>input[type=hidden],td>datepicker>input[type=text],td>div>div>input[type=text]');
            var dropDowns = $(tr).find('td>.ui-select-container');
            var dynamicInputs = $(tr).find('td>.dynField>input[type=text]');
            var financeListRows = $(tr).find('td.financeData').find('tr[editable="true"]');


            var salesRow = {};
            var dynamicSalesRow = [];
            var financeListData = [];

            $.each(inputs, function (indx, inpt) {
                salesRow[$(inpt).attr('name')] = $(inpt).val();
            });

            $.each(dropDowns, function (indx, inpt) {
                salesRow[$(inpt).attr('data-name')] = parseInt($(inpt).attr('data-model'));
            });

            $.each(dynamicInputs, function (indx, inpt) {
                var dynId = $(inpt).attr('data-column-id');
                var dynValue = $(inpt).val();
                dynamicSalesRow.push({ 'ColumnId': dynId, 'Value': dynValue });
            });

            $.each(financeListRows, function (indx, financeTr) {
                var financeInputs = $(financeTr).find('td>input[type=text],td>input[type=hidden],td>datepicker>input[type=text],td>div>div>input[type=text]');
                var financeRow = {};
                $.each(financeInputs, function (indx, inpt) {
                    financeRow[$(inpt).attr('name')] = $(inpt).val();
                });

                var dynamicFinInputs = $(financeTr).find('td>.dynFinField>input[type=text]');

                var dynamicFinRow = [];

                $.each(dynamicFinInputs, function (indx, inpt) {
                    var dynId = $(inpt).attr('data-column-id');
                    var dynValue = $(inpt).val();
                    dynamicFinRow.push({ 'ColumnId': dynId, 'Value': dynValue });

                });

                financeRow["FinanceColumnValues"] = dynamicFinRow;
                financeListData.push(financeRow);
            });
            salesRow["ReportColumnValues"] = dynamicSalesRow;
            salesRow["FinanceDataList"] = financeListData;

            sales.push(salesRow);
        });

        objModel = { "SalesList": sales, "ServiceId": $scope.salesFilter.ServiceId }

        $.ajax({
            data: objModel,
            type: 'post',
            url: '/sales/SaveData',
            dataType: "json",
            success: function (data) {
                //callback methods go right here
                $('a.btn-save', _this).attr('disabled', false);
                _this.removeClass('posting');
                if ($('.posting').length == 0)
                    $scope.isDisable = false;
            }
        });
    };

    $scope.NewFinanceRow = function (rowIndex, sales) {

        var rowModel = $scope.model.List[rowIndex];

        if (rowModel.FinanceDataList && rowModel.FinanceDataList.length >= 1) {
            var newFinanceRow = angular.copy(rowModel.FinanceDataList[0]);
            $.each(newFinanceRow, function (key, value) {
                if (key.toLocaleLowerCase().indexOf('date') >= 0) {
                    newFinanceRow[key] = new Date();
                }
                else if (key == 'FinanceColumnValues') {
                    $.each(value, function (key1, value1) {
                        value1["Value"] = '';
                    });
                }
                else {
                    newFinanceRow[key] = '';
                }
            });
            rowModel.FinanceDataList.push(newFinanceRow);

            rowModel.ShowFinanceDataList = true;
            rowModel.FinanceDataRecordCount = rowModel.FinanceDataList.length;

        }
        else {
            $http.get(baseUrl + '/sales/financedata/' + rowModel.ReportId).success(function (data) {

                rowModel.FinanceDataList = data.Model.List;
                if (rowModel.FinanceDataList[0].ReportFinanceId == '')
                    return;
                var newFinanceRow = angular.copy(rowModel.FinanceDataList[0]);

                $.each(newFinanceRow, function (key, value) {
                    if (key.toLocaleLowerCase().indexOf('date') >= 0) {
                        newFinanceRow[key] = new Date();
                    }
                    else if (key == 'FinanceColumnValues') {
                        $.each(value, function (key1, value1) {
                            value1["Value"] = '';
                        });
                    }
                    else {
                        newFinanceRow[key] = '';
                    }
                });
                rowModel.FinanceDataList.push(newFinanceRow);

                rowModel.ShowFinanceDataList = true;
                rowModel.FinanceDataRecordCount = rowModel.FinanceDataList.length;

            });
        }
    }

    $scope.collectionColumnsFilter = function (columnKeyValues) {

        var newList = [];

        $.each(columnKeyValues, function (i, obj) {
            if (obj.ColumnType == 1) {
                newList.push(obj);
            }
        })
        return newList;
    }

    $scope.billingColumnsFilter = function (columnKeyValues) {

        var newList = [];

        $.each(columnKeyValues, function (i, obj) {
            if (obj.ColumnType == 2) {
                newList.push(obj);
            }
        })

        return newList;
    }

    $scope.ShowDatePicker = function (key) {
        return key.toLocaleLowerCase().indexOf('date') > -1;
    };

    $scope.isSpecimenExists = function (objSales) {
        $http.get(baseUrl + '/sales/IsSpecimenExists/' + objSales.SpecimenId).success(function (response) {
            objSales.IsSpecimenExists = response;
        });
    }

    $scope.loadSales = function (filterBy, FilterByValue, groupedItem) {
        $scope.salesLoading = true;
        if ($scope.salesFilter == null) {
            return false;
        }

        if ($scope.salesFilter.GroupBy > 0) {
            $scope.isGroupedData = true;
            $scope.loadGroupedSales();
            return false;
        } else {
            $scope.isGroupedData = false
        }

        $scope.salesFilterCopy = angular.copy($scope.salesFilter);

        $scope.salesFilterCopy.WrittenDateFrom = toLocal($scope.salesFilterCopy.WrittenDateFrom);
        $scope.salesFilterCopy.WrittenDateTo = toLocal($scope.salesFilterCopy.WrittenDateTo);
        $scope.salesFilterCopy.ReportedDateFrom = toLocal($scope.salesFilterCopy.ReportedDateFrom);
        $scope.salesFilterCopy.ReportedDateTo = toLocal($scope.salesFilterCopy.ReportedDateTo);

        $http.get(baseUrl + '/sales/getColumnLookups/' + $scope.salesFilterCopy.ServiceId).success(function (data) {
            $scope.salesColumnLookups = data;
        });

        if ($scope.routeParams.reportid) {
            $scope.showDetailsView($scope.routeParams.reportid);
            $scope.routeParams.reportid = null;
            $scope.salesLoading = false;
            $scope.loadSalesOnHideDetail = true;
            return false;
        }

        $http.post(baseUrl + '/sales/getbyfilter', $scope.salesFilterCopy).success(function (data) {
            $scope.model = data.Model;
            $scope.currentListCount = data.Model.List.length;
            $scope.salesLoading = false;
            $scope.showLogView = false;
        });

        //$scope.select2Options = {
        //    allowClear: true
        //};
    };

    $scope.loadGroupedSales = function () {
        $scope.isGroupedData = true;
        $scope.salesLoading = true;
        if ($scope.salesFilter == null) {
            return false;
        }

        $scope.salesFilterCopy = angular.copy($scope.salesFilter);

        $scope.salesFilterCopy.WrittenDateFrom = toLocal($scope.salesFilterCopy.WrittenDateFrom);
        $scope.salesFilterCopy.WrittenDateTo = toLocal($scope.salesFilterCopy.WrittenDateTo);
        $scope.salesFilterCopy.ReportedDateFrom = toLocal($scope.salesFilterCopy.ReportedDateFrom);
        $scope.salesFilterCopy.ReportedDateTo = toLocal($scope.salesFilterCopy.ReportedDateTo);

        $http.post(baseUrl + '/sales/GetAllGroupedSales', $scope.salesFilterCopy).success(function (data) {
            $scope.model = data.Model;
            $scope.currentListCount = data.Model.List.length;
            $scope.salesLoading = false;
            $scope.showLogView = false;
        });
    };

    $scope.loadGroupedSubSales = function (isLoad, salesItem) {
        $scope.isGroupedData = true;
        $scope.salesLoading = true;
        if ($scope.salesFilter == null) {
            return false;
        }

        $scope.salesFilterCopy = angular.copy($scope.salesFilter);

        if (salesItem.CurrentPage == null) {
            $scope.salesFilterCopy.CurrentPage = 1
            $scope.salesFilterCopy.PageSize = 25;
        }
        if (salesItem != null) {
            $scope.salesFilterCopy.CurrentPage = salesItem.CurrentPage;
            $scope.salesFilterCopy.PageSize = salesItem.PageSize;
            $scope.salesFilterCopy.orderBy = salesItem.orderBy;
            $scope.salesFilterCopy.sortKey = salesItem.sortKey;
        }

        $scope.salesFilterCopy.WrittenDateFrom = toLocal($scope.salesFilterCopy.WrittenDateFrom);
        $scope.salesFilterCopy.WrittenDateTo = toLocal($scope.salesFilterCopy.WrittenDateTo);
        $scope.salesFilterCopy.ReportedDateFrom = toLocal($scope.salesFilterCopy.ReportedDateFrom);
        $scope.salesFilterCopy.ReportedDateTo = toLocal($scope.salesFilterCopy.ReportedDateTo);

        if ($scope.salesFilter.GroupBy > 0) {
            $scope.isGroupedData = true;
            switch ($scope.salesFilter.GroupBy) {
                case 1:
                    $scope.salesFilterCopy.GroupByPracticeId = salesItem.PracticeId;
                    break;

                case 2:
                    $scope.salesFilterCopy.GroupByRepId = salesItem.RepId;
                    break;

                case 3:
                    $scope.salesFilterCopy.GroupByRepGroupId = salesItem.RepGroupId;
                    break;
            }
        }
        else {
            $scope.isGroupedData = false;
        }

        if (isLoad == false || (isLoad && salesItem.GroupedItem == null)) {
            $http.post(baseUrl + '/sales/getbyfilter', $scope.salesFilterCopy).success(function (data) {
                salesItem.GroupedItem = data.Model;
                $scope.currentListCount = data.Model.List.length;
                $scope.salesLoading = false;
                $scope.showLogView = false;

                if (salesItem != null && salesItem.CurrentPage == null) {
                    salesItem.CurrentPage = 1;
                    salesItem.PageSize = 25;
                }
            });
        }
        else {
            salesItem.GroupedItem = null;
            $scope.salesLoading = false;
            $scope.showLogView = false;
        }
    };

    $scope.doSubListPaging = function (groupedSalesItem) {
        $scope.loadGroupedSubSales(false, groupedSalesItem);
    }

    $scope.getSalesLookup = function (columnId) {
        var newTemp = $filter("filter")($scope.salesColumnLookups.Model.List, { ColumnId: columnId });
        return newTemp[0].ColumnLookup;
    }

    $scope.scrollconfig = {
        autoHideScrollbar: false,
        theme: 'light',
        advanced: {
            updateOnContentResize: true
        },
        setHeight: 1200,
        scrollInertia: 0
    };
    $scope.toggleEditView = function () {
        $scope.isEditView = !$scope.isEditView;
        if ($scope.isEditView == false) {
            $scope.resetFilter();
            $scope.model = null;
            $scope.oldModel = null;
        }
        else {
            //$scope.oldModel = angular.copy($scope.model);
        }
        $scope.fitSize();
    };

    $scope.expandFinance = function (isExpandable, sales) {

        if (sales.FinanceDataRecordCount > 1) {
            $http.get(baseUrl + '/sales/financedata/' + sales.ReportId).success(function (data) {
                sales.FinanceDataList = data.Model.List;
            });
        }
        sales.ShowFinanceDataList = !sales.ShowFinanceDataList;

    };

    $scope.fitSize = function () {

        $timeout(function () {
            var maxWidth = $(window).width();
            $("#salesComposeForm").width(maxWidth - 200);
            $("#salesComposeForm").find('tbody#editBody').height($(window).height() - 310);

            var target = $("#SalesAddWrap>tbody");
            var scrollBar = $("#tableScroller")

            scrollBar.find('div').height(target.prop('scrollHeight') + 120);
            scrollBar.scroll(function () {
                target.prop("scrollTop", this.scrollTop)
                        .prop("scrollLeft", this.scrollLeft);
            });
            target.scroll(function () {
                scrollBar.prop("scrollTop", this.scrollTop)
                        .prop("scrollLeft", this.scrollLeft);
            })
        }, 100);

        nice = $("#salesComposeForm").find('.table-responsive').niceScroll({ horizrailenabled: true, railvalign: 'bottom', autohidemode: false });


        $('[rel="popover"]').popover({
            container: 'body',
            html: true,
            content: function () {
                var clone = $($(this).data('popover-content')).clone(true).removeClass('hide');
                return clone;
            }
        }).click(function (e) {
            e.preventDefault();
        });

    };

    $scope.addNewRow = function (isFirstRow) {
        var modelRow = new Object();
        angular.copy($scope.model.List[0], modelRow);

        $.each(modelRow, function (key, value) {
            if (key == 'ReportColumnValues') {
                $.each(value, function (key1, value1) {
                    value1["Value"] = '';
                });
            }
            else {
                if (key.toLocaleLowerCase().indexOf('date') >= 0) {
                    modelRow[key] = new Date();
                }
                else {
                    modelRow[key] = '';
                }
            }
        });
        $scope.isEditView = true;
        $scope.isAddView = true;
        if (isFirstRow)
            $scope.model.List = [];
        $scope.model.List.push(modelRow);

        $scope.fitSize();

    }

    $scope.loadSalesAddForm = function () {
        if ($scope.salesFilter == null) {
            return false;
        }

        $http.post(baseUrl + '/sales/getbyfilter', $scope.salesFilterCopy).success(function (data) {
            $scope.model = data.Model;
            $scope.salesLoading = false;
            $scope.showLogView = false;
        });
    };

    $scope.showLog = function (isToggleView) {
        if ($scope.showLogView == true && isToggleView == true) {
            $scope.showLogView = false;
            $scope.loadSales();
            return;
        }
        
        $scope.logLoading = true;
        $scope.salesLogFilter = {
            ImportedDateFrom: $scope.salesFilter.LogDateFrom,
            ImportedDateTo: $scope.salesFilter.LogDateTo,
            ServiceId: $scope.salesFilter.ServiceId,
            CurrentPage: $scope.salesFilter.CurrentPage,
            PageSize: $scope.salesFilter.PageSize
        };

        $http.post(baseUrl + '/sales/logsummary', $scope.salesLogFilter).success(function (data) {
            $scope.logModel = data.Model;
            $scope.logLoading = false;
            $scope.showLogView = true;
        });
    };

    $scope.showSalesByLogId = function (logId, mode) {
        $scope.showLogView = false;
        $scope.isEditView = mode == 5;
        $scope.salesFilter.LogStatuses = mode;
        $scope.salesFilter.LogId = logId;
    }

    $scope.showParserMessages = function (logId) {
        $http.get(baseUrl + '/sales/ParserMessages/' + logId).success(function (data) {
            $scope.parsermessages = data;
        });
    }

    $scope.exportData = function () {
        $scope.salesExcelFilter = angular.copy($scope.salesFilter);
        $scope.excelExportLoading = true;
        $scope.salesExcelFilter.PageSize = $scope.model.Pager.TotalCount;
        $http.post(baseUrl + '/sales/export', $scope.salesExcelFilter).success(function (data) {
            $scope.excelExportLoading = false;
            var win = window.open(baseUrl + data.Model, '_blank');
        });
    };

    $scope.resetFilter = function () {

        $scope.selectedPeriod = 0;
        var serviceId = $scope.salesFilter.ServiceId;
        $scope.model = null;
        $scope.salesLoading = true;
        $http.get(baseUrl + '/sales/getfilter/' + $scope.salesFilter.ServiceId).success(function (filterResponse) {

            if (DefaultDateRange)
                setDateFiltersByPeriod(DefaultDateRange);

            $scope.salesFilter = filterResponse;
            $scope.salesFilter.ServiceId = serviceId;
            $scope.salesFilter.GroupBy = authService.authentication.SalesGroupBy;

            salesColumnWatch = $scope.$watch('salesFilter.DynamicFilters', function (value, oldValue) {
                angular.forEach(value, function (v, key) {
                    $scope.salesFilter.Columns[v.ColumnName] = v.IsVisible;
                });
            }, true);

            salesFilterWatch = $scope.$watch('salesFilter', function (value, oldValue) {
                var flag = false;
                $scope.affectedFilterCount = 0;
                $scope.affectedFilters = [];
                angular.forEach(value, function (v, key) {
                    if (key != 'Columns' && key != 'PageSize' && key != 'CurrentPage' && key != 'Skip' && key != 'Take' && key != 'ServiceId') {
                        if (v != null && v != [] && v != '' && v >= 0) {
                            $scope.affectedFilterCount = $scope.affectedFilterCount + 1;
                            setFilterText(key, v);
                        }
                    }
                });
                if ($scope.model != null && $scope.model.List)
                    $scope.model.List = null;
                $scope.stopTimeout();

                $scope.startTimeout();
            }, true);

        });
    };

    function setFilterText(key, value) {
        if (key == 'ProviderId') {
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
        else if (key == 'WrittenDateFrom') {
            $scope.affectedFilters.push('Written From:' + $filter('date')(new Date(value), 'MMM dd, yyyy'));
        }
        else if (key == 'WrittenDateTo') {
            $scope.affectedFilters.push('Written To:' + $filter('date')(new Date(value), 'MMM dd, yyyy'));
        }
        else if (key == 'ReportedDateFrom') {
            $scope.affectedFilters.push('Reported From:' + $filter('date')(new Date(value), 'MMM dd, yyyy'));
        }
        else if (key == 'ReportedDateTo') {
            $scope.affectedFilters.push('Reported To:' + $filter('date')(new Date(value), 'MMM dd, yyyy'));
        }
        else if (key == 'Keyword') {
            $scope.affectedFilters.push(value);
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
    };

    var mytimeout;
    $scope.startTimeout = function () {
        $scope.startCount = $scope.startCount + 1;
        mytimeout = $timeout(function () {
            if ($scope.showLogView == false){
                $scope.loadSales();
            }
            else {
                $scope.showLog();
            }
        }, 1000);
    };

    $scope.stopTimeout = function () {
        $timeout.cancel(mytimeout);
    }

    $scope.base64Data = [];

    $scope.fileUpload = function (files, isFinanceFile) {
        $scope.base64Data = [];
        if (isFinanceFile === true)
            $scope.currentSales = { IsFinanceFile: true };

        $scope.uploadingReport = true;
        $scope.StartStatusCheck();

        for (var i = 0; i < files.length; i++) {
            $scope.base64Data.push(files[i]);
        }

        if (!$scope.currentSales) {
            $scope.currentSales = { files: null };
        }

        if ($scope.base64Data.length > 0)
            $scope.currentSales.files = $scope.base64Data;
        else {
            $scope.base64Data.push(files);
            $scope.currentSales.files = $scope.base64Data;
        }
        $scope.currentSales.files = $scope.base64Data;

        $scope.currentSales.ServiceId = $scope.salesFilter.ServiceId;
        $http.post(baseUrl + '/Sales/Save', $scope.currentSales).success(function (data) {
            $scope.currentSales.files = null;
            $scope.uploadingReport = false;
            if (data.Status == 200 || data.Status == 201) {
                $scope.startTimeout();
            }
        });
    }

    $scope.StartStatusCheck = function () {
        $interval(function () {
            if ($scope.uploadingReport == true) {
                $http.get(baseUrl + '/sales/IsParsingCompleted').success(function (data) {
                    $scope.uploadingReport = !data;
                });
            }
        }, 30000);
    }

    $scope.showDetailsView = function (reportId) {
        $window.scrollTo(0, 0);
        $scope.loadingDetails = true;
        $scope.loadingData = false;

        $http.get(baseUrl + '/sales/' + reportId).success(function (data) {
            $scope.currentSales = data.Model;
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
        $scope.currentLead = null;

        if ($scope.loadSalesOnHideDetail) {
            $scope.loadSalesOnHideDetail = false;
            $scope.loadSales();
        }
    }

    $scope.init = function () {
        $rootScope.controller = 'Sales';
        $cookies.put('location', 'sales');
        $scope.loadLookUps();

        $scope.$watch('selectedPeriod', function (newValue, oldValue) {
            if (newValue !== oldValue) {
                setDateFiltersByPeriod(newValue);
            }
        });

        //$scope.selectedPeriod = DefaultDateRange;
    }

    function setDateFiltersByPeriod(v) {
        if (v !== null) {
            var s = v.toString();
            switch (s) {
                case '1':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().startOf('week').toDate();
                        $scope.salesFilter.LogDateTo = moment().toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().startOf('week').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().toDate();
                    //}
                    break;
                case '2':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().subtract(1, 'week').startOf('week').toDate();
                        $scope.salesFilter.LogDateTo = moment().subtract(1, 'week').endOf('week').toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().subtract(1, 'week').startOf('week').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().subtract(1, 'week').endOf('week').toDate();
                    //}
                    break;
                case '3':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().startOf('month').toDate();
                        $scope.salesFilter.LogDateTo = moment().toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().startOf('month').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().toDate();
                    //}
                    break;
                case '4':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().subtract(1, 'month').startOf('month').toDate();
                        $scope.salesFilter.LogDateTo = moment().subtract(1, 'month').endOf('month').toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().subtract(1, 'month').startOf('month').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().subtract(1, 'month').endOf('month').toDate();
                    //}
                    break;
                case '5':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().subtract(2, 'month').startOf('month').toDate();
                        $scope.salesFilter.LogDateTo = moment().toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().subtract(2, 'month').startOf('month').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().toDate();
                    //}
                    break;
                case '6':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().subtract(5, 'month').startOf('month').toDate();
                        $scope.salesFilter.LogDateTo = moment().toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().subtract(5, 'month').startOf('month').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().toDate();
                    //}
                    break;
                case '7':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().subtract(11, 'month').startOf('month').toDate();
                        $scope.salesFilter.LogDateTo = moment().toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().subtract(11, 'month').startOf('month').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().toDate();
                    //}
                    break;
                case '8':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().startOf('year').toDate();
                        $scope.salesFilter.LogDateTo = moment().toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().startOf('year').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().toDate();
                    //}
                    break;
                case '9':
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = moment().subtract(1, 'year').startOf('year').toDate();
                        $scope.salesFilter.LogDateTo = moment().subtract(1, 'year').endOf('year').toDate();
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = moment().subtract(1, 'year').startOf('year').toDate();
                        $scope.salesFilter.ReceivedDateTo = moment().subtract(1, 'year').endOf('year').toDate();
                    //}
                    break;
                default:
                    //if ($scope.showLogView) {
                        $scope.salesFilter.LogDateFrom = "";
                        $scope.salesFilter.LogDateTo = "";
                    //} else {
                        $scope.salesFilter.ReceivedDateFrom = "";
                        $scope.salesFilter.ReceivedDateTo = "";
                    //}
                    break;
            }
        }
    }

    $scope.setSelectedColumn = function (prop) {
        $scope.salesFilter.orderBy = ($scope.salesFilter.orderBy == null || $scope.salesFilter.orderBy == 'desc') ? 'asc' : ($scope.salesFilter.sortKey == prop.ColumnName && $scope.salesFilter.orderBy == 'asc') ? 'desc' : 'asc';
        $scope.salesFilter.sortKey = prop.ColumnName;
    };

    $scope.setSelectedSubListColumn = function (prop, groupedSalesItem) {
        groupedSalesItem.orderBy = (groupedSalesItem.orderBy == null || groupedSalesItem.orderBy == 'desc') ? 'asc' : (groupedSalesItem.sortKey == prop.ColumnName && groupedSalesItem.orderBy == 'asc') ? 'desc' : 'asc';
        groupedSalesItem.sortKey = prop.ColumnName;
        $scope.loadGroupedSubSales(false, groupedSalesItem);
    };

    $scope.deleteFinance = function (financeId) {
        confirm("Do you want to delete the selected finance record? ", function (flag) {
            if (flag) {
                $scope.loadingDeleteStatus = true;
                $http.get(baseUrl + 'sales/finance/delete/' + financeId).success(function (data) {
                    $scope.loadingDeleteStatus = false;
                    if (data.Model) {
                        alert("Deleted");
                    }
                });
            }
        });
    }
});
