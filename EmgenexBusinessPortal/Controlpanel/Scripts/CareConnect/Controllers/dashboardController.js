careconnect.controller('dashboardController', ['$scope', '$http', '$route', '$uibModal', 'valueService', '$rootScope', '$window', '$routeParams', 'authService', '$location', '$cookies', '$timeout',
    function ($scope, $http, $route, $uibModal, valueService, $rootScope, $window, $routeParams, authService, $location, $cookies, $timeout) {

        $scope.Loader = {
            PeriodicData: true,
            SalesPerformance: true,
            TopReps: true,
            TopAccounts: true,
            LatestLeads: true,
            LatestAccounts: true,
            LeadSummery: true,
            LeadFunnel: true
        }

        $rootScope.controller = 'Dashboard';
        $cookies.put('location', 'dashboard');
        $scope.user = {};
        if ($rootScope.taskId != null) {
            $location.path('/tasks');
        }
        $scope.logged = false;
        var accessToken;
        var flag = true;
        $scope.message = "";
        $scope.reloadPage = function () {
            $route.reload();
        }

        $scope.accountFilter = {
            ServiceId: 0
        };

        $scope.repFilter = {
            ServiceId: 0
        };

        $scope.periodicTrends = {
            ServiceId: 0
        };

        $scope.serviceGraphFilter = {
            ServiceId: 0
        };

        $scope.PeriodicTreandsFilter = {
            ViewBy: 2,
            DateType: "ReceivedDate",
            Total: "Sales",
        };

        $scope.selectedPeriod = 0;
        $scope.Loader.PeriodicData = true;

        $scope.DateRangeFilter = {
            DateFrom: "",
            DateTo: ""
        };

        $rootScope.lookUps = { lookupServices: null };

        $scope.datapoints = [];

        $scope.datacolumns = [];
        $scope.datax = { "id": "x" };

        $scope.chartCallback = function (chartObj) {
            $scope.trendzChart = chartObj;
        }

        $scope.tickFormatFunction = function (value) {
            var tickDt = new Date(value);

            switch ($scope.PeriodicTreandsFilter.ViewBy) {
                case 1:
                    return tickDt.getFullYear();
                    break;
                case 2:
                    return getDateName(tickDt.getMonth()) + '-' + tickDt.getFullYear();
                    break;
                case 3:
                    return tickDt.getDate() + ' ' + getDateName(tickDt.getMonth()) + '-' + tickDt.getFullYear();
                    break;
                case 4:
                    return tickDt.getDate() + ' ' + getDateName(tickDt.getMonth()) + '-' + tickDt.getFullYear();
                    break;
            }

        };

        $scope.$watch('selectedPeriod', function (newValue, oldValue) {
            if (newValue !== oldValue) {
                setDateFiltersByPeriod(newValue);
            }
        });

        function setDateFiltersByPeriod(v) {
            if (v !== null) {
                var s = v.toString();
                switch (s) {
                    case '1':
                        $scope.DateRangeFilter.DateFrom = moment().startOf('week').toDate();
                        $scope.DateRangeFilter.DateTo = moment().toDate();
                        break;
                    case '2':
                        $scope.DateRangeFilter.DateFrom = moment().subtract(1, 'week').startOf('week').toDate();
                        $scope.DateRangeFilter.DateTo = moment().subtract(1, 'week').endOf('week').toDate();
                        break;
                    case '3':
                        $scope.DateRangeFilter.DateFrom = moment().startOf('month').toDate();
                        $scope.DateRangeFilter.DateTo = moment().toDate();
                        break;
                    case '4':
                        $scope.DateRangeFilter.DateFrom = moment().subtract(1, 'month').startOf('month').toDate();
                        $scope.DateRangeFilter.DateTo = moment().subtract(1, 'month').endOf('month').toDate();
                        break;
                    case '5':
                        $scope.DateRangeFilter.DateFrom = moment().subtract(2, 'month').startOf('month').toDate();
                        $scope.DateRangeFilter.DateTo = moment().toDate();
                        break;
                    case '6':
                        $scope.DateRangeFilter.DateFrom = moment().subtract(5, 'month').startOf('month').toDate();
                        $scope.DateRangeFilter.DateTo = moment().toDate();
                        break;
                    case '7':
                        $scope.DateRangeFilter.DateFrom = moment().subtract(11, 'month').startOf('month').toDate();
                        $scope.DateRangeFilter.DateTo = moment().toDate();
                        break;
                    case '8':
                        $scope.DateRangeFilter.DateFrom = moment().startOf('year').toDate();
                        $scope.DateRangeFilter.DateTo = moment().toDate();
                        break;
                    case '9':
                        $scope.DateRangeFilter.DateFrom = moment().subtract(1, 'year').startOf('year').toDate();
                        $scope.DateRangeFilter.DateTo = moment().subtract(1, 'year').endOf('year').toDate();
                        break;
                    default:
                        $scope.DateRangeFilter.DateFrom = "";
                        $scope.DateRangeFilter.DateTo = "";
                        break;
                }
            }
        }

        function applyWatches(isPageLoad) {

            if (DateRangeFilterWatch)
                DateRangeFilterWatch();

            var DateRangeFilterWatch = $scope.$watch('DateRangeFilter', function (newValue, oldValue) {
                $scope.Loader.PeriodicData = true;
                if (newValue !== oldValue) {
                    if (isPageLoad) {
                        isPageLoad = false;
                        return;
                    }
                    onpageload();
                    loadGraph($scope);
                    //initWatches();
                }
            }, true);

        }

        function initWatches() {

            $scope.Loader.PeriodicData =
                    $scope.Loader.SalesPerformance =
                    $scope.Loader.TopReps =
                    $scope.Loader.TopAccounts =
                    $scope.Loader.LatestLeads =
                    $scope.Loader.LatestAccounts =
                    $scope.Loader.LeadSummery =
                    $scope.Loader.LeadFunnel = true;

            //Note : This section is repeating - need to generalize

            if (unwatchAccountFilter)
                unwatchAccountFilter();

            if (unwatchRepFilter)
                unwatchRepFilter();

            if (unwatchPeriodicTrends)
                unwatchPeriodicTrends();

            if (serviceGraphFilter)
                serviceGraphFilter();

            var unwatchAccountFilter = $scope.$watch('accountFilter.ServiceId', function (newValue, oldValue) {

                if (newValue !== oldValue) {
                    loadTopAccounts();
                }
            }, true);

            var unwatchRepFilter = $scope.$watch('repFilter.ServiceId', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    loadTopReps();
                }
            }, true);

            var unwatchPeriodicTrends = $scope.$watch('periodicTrends.ServiceId', function (newValue, oldValue) {
                alert(11111)
                if (newValue !== oldValue) {
                    LoadPeriodicTrends();
                }
            }, true);

            var serviceGraphFilter = $scope.$watch('serviceGraphFilter.ServiceId', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    if ($scope.serviceGraphFilter.ServiceId) {
                        loadGraph($scope);
                    }
                }
            }, true);



            $scope.$watch('PeriodicTreandsFilter', function (newValue, oldValue) {

                if (newValue !== oldValue) {
                    $scope.DateRangeFiltermodel = {
                        DateFrom: toLocal($scope.DateRangeFilter.DateFrom),
                        DateTo: toLocal($scope.DateRangeFilter.DateTo),
                        ViewBy: $scope.PeriodicTreandsFilter.ViewBy,
                        DateType: $scope.PeriodicTreandsFilter.DateType,
                        Total: $scope.PeriodicTreandsFilter.Total
                    };
                    loadGraph($scope);
                }


            }, true);

            //Note End

        }

        var periodicTreandsFilterWatch;

        $scope.loadLookUps = function () {
            $scope.DateRangeFiltermodel = {
                DateFrom: toLocal($scope.DateRangeFilter.DateFrom),
                DateTo: toLocal($scope.DateRangeFilter.DateTo),
                ViewBy: $scope.PeriodicTreandsFilter.ViewBy,
                DateType: $scope.PeriodicTreandsFilter.DateType,
                Total: $scope.PeriodicTreandsFilter.Total
            };

            if ($rootScope.lookUps.lookupServices === null) {
                $http.get(baseUrl + '/lookup/getallservices').success(function (data) {
                    if (data.Model.List.length > 0) {
                        var serviceId = data.Model.List[0].Id;
                        $rootScope.lookUps.lookupServices = data.Model.List;

                        var unwatchAccountFilter = $scope.$watch('accountFilter.ServiceId', function (newValue, oldValue) {
                            if (newValue !== oldValue) {
                                serviceGraphFilter.ServiceId = $scope.accountFilter.ServiceId;
                                loadTopAccounts();
                            }
                        }, true);

                        var unwatchRepFilter = $scope.$watch('repFilter.ServiceId', function (newValue, oldValue) {
                            if (newValue !== oldValue) {
                                loadTopReps();
                            }

                        }, true);

                        var unwatchPeriodicTrends = $scope.$watch('periodicTrends.ServiceId', function (newValue, oldValue) {
                            if (newValue !== oldValue) {
                                LoadPeriodicTrends();
                            }

                        }, true);

                        var serviceGraphFilter = $scope.$watch('serviceGraphFilter.ServiceId', function (newValue, oldValue) {

                            if (newValue !== oldValue) {


                                //if ($scope.serviceGraphFilter.ServiceId == null) {
                                //    $scope.serviceGraphFilter.ServiceId = 0;
                                //}

                                if ($scope.serviceGraphFilter.ServiceId !== null) {
                                    $scope.Loader.PeriodicData = true;
                                    console.log(111111444444)
                                    $http.get(baseUrl + '/stats/getdashboardsalesdatetype/' + $scope.serviceGraphFilter.ServiceId).success(function (data) {
                                        if (data != null) {
                                            var list = data.List;
                                            $scope.lookUps.DashboardSalesDateType = list;
                                            //$.each(list, function (i, obj) {

                                            //    if (obj.IsSelected == true) {
                                            //        $scope.PeriodicTreandsFilter.DateType = obj.Value;
                                            //        //$scope.DateRangeFiltermodel.DateType = obj.Value;
                                            //    }
                                            //});
                                        }


                                        $http.get(baseUrl + '/stats/getdashboardsalestotal/' + $scope.serviceGraphFilter.ServiceId).success(function (data) {
                                            var list = data.List;
                                            $scope.lookUps.DashboardSalesTotal = list;
                                            //$.each($scope.lookUps.DashboardSalesTotal, function (i, obj) {

                                            //    if (obj.IsSelected == true) {
                                            //        $scope.PeriodicTreandsFilter.Total = obj.Value;
                                            //        //$scope.DateRangeFiltermodel.Total = obj.Value;
                                            //    }
                                            //});

                                            //if (!periodicTreandsFilterWatch)
                                            loadGraph($scope);

                                            //if ($scope.serviceGraphFilter.ServiceId >= 0) {
                                            //    console.log(66666661111)
                                            //    loadGraph($scope);
                                            //}
                                            //else {



                                            // }

                                        });
                                    });
                                    if (periodicTreandsFilterWatch)
                                        periodicTreandsFilterWatch();
                                    periodicTreandsFilterWatch = $scope.$watch('PeriodicTreandsFilter', function (newValue, oldValue) {
                                        if (newValue !== oldValue) {
                                            $scope.DateRangeFiltermodel = {
                                                DateFrom: toLocal($scope.DateRangeFilter.DateFrom),
                                                DateTo: toLocal($scope.DateRangeFilter.DateTo),
                                                ViewBy: $scope.PeriodicTreandsFilter.ViewBy,
                                                DateType: $scope.PeriodicTreandsFilter.DateType,
                                                Total: $scope.PeriodicTreandsFilter.Total
                                            };
                                            console.log(6666666)
                                            loadGraph($scope);
                                        }


                                    }, true);
                                    //loadGraph($scope);
                                }
                            }
                        }, true);
                    }
                });
            }
        }

        $scope.initDashboard = function () {

            $scope.select2OptionsService = {
                placeholder: "Select Service"
            };

            $scope.select2OptionsViewBy = {
                placeholder: "View By"
            };
            $scope.select2OptionsDateType = {
                placeholder: "Date Type"
            };
            $scope.select2OptionsTotal = {
                placeholder: "Total"
            };
            $scope.selectedPeriod = DefaultDateRange;

            setDateFiltersByPeriod($scope.selectedPeriod);
            $http.get(baseUrl + '/lookup/GetAllDashboardPeriods').success(function (data) {
                var list = data.Model.List;
                $scope.lookUps.DashboardPeriods = list;
            });

            $http.get(baseUrl + '/lookup/getdashboardsalesviewby').success(function (data) {
                var list = data.Model.List;
                $scope.lookUps.DashboardSalesViewBy = list;
            });

            $http.get(baseUrl + '/stats/getdashboardsalesdatetype/0').success(function (data) {
                if (data != null) {
                    var list = data.List;
                    $scope.lookUps.DashboardSalesDateType = list;
                    $.each(list, function (i, obj) {
                        if (obj.IsSelected == true) {
                            $scope.PeriodicTreandsFilter.DateType = obj.Value;
                        }
                    });
                }
            });
            $http.get(baseUrl + '/stats/getdashboardsalestotal/0').success(function (data) {
                var list = data.List;
                $scope.lookUps.DashboardSalesTotal = list;
                $.each($scope.lookUps.DashboardSalesTotal, function (i, obj) {

                    if (obj.IsSelected == true) {
                        $scope.PeriodicTreandsFilter.Total = obj.Value;
                    }
                });
            });

            $scope.loadLookUps();

            applyWatches(false);

            onpageload();

            $scope.select2Options = {
                allowClear: true
            };
        };

        function rgb2hex(rgb) {
            rgb = rgb.match(/^rgba?[\s+]?\([\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?/i);
            return (rgb && rgb.length === 4) ? "#" +
             ("0" + parseInt(rgb[1], 10).toString(16)).slice(-2) +
             ("0" + parseInt(rgb[2], 10).toString(16)).slice(-2) +
             ("0" + parseInt(rgb[3], 10).toString(16)).slice(-2) : '';
        }

        var randomColorFactor = function () {
            return Math.round(Math.random() * 255);
        };

        var randomColor = function (opacity) {
            return rgb2hex('rgba(' + randomColorFactor() + ',' + randomColorFactor() + ',' + randomColorFactor() + ',' + (opacity || '.3') + ')');
        };

        var onpageload = function () {


            $scope.DateRangeFiltermodel = {
                DateFrom: toLocal($scope.DateRangeFilter.DateFrom),
                DateTo: toLocal($scope.DateRangeFilter.DateTo),
                ViewBy: $scope.PeriodicTreandsFilter.ViewBy,
                DateType: $scope.PeriodicTreandsFilter.DateType,
                Total: $scope.PeriodicTreandsFilter.Total
            };



            /* DO NOT REMOVE - KIRA
            $http.get(baseUrl + '/stats/AccountHighlight').success(function (data) {
                $scope.AccountHighlightModel = data.Model;
                $scope.AccountHighlightlabels = data.Model.ThisWeekdays;
                $scope.AccountHighlightseries = ['This Week', 'Last Week'];
                $scope.AccountHighlightdata = [
                  data.Model.ThisWeekCounts,
                  data.Model.LastWeekCounts
                ];
                $scope.AccountHighlightcolors = ['#498be1', '#686868'],
                $scope.AccountHighlightdatasetOverride = [{ borderColor: "rgba(73, 139,225, 1)", pointBorderColor: "rgba(73, 139,225, 1)", pointBackgroundColor: "#fff", yAxisID: 'y-axis-1' }];
                $scope.AccountHighlightoptions = {

                    datasetFill: false,
                    scales: {
                        yAxes: [
                          {
                              id: 'y-axis-1',
                              type: 'linear',
                              display: true,
                              position: 'left'
                          }
                        ]
                    },
                    legend: {
                        display: true,
                        labels: {
                            padding: 50,
                            boxWidth: 20,
                        }
                    },

                    elements: { line: { tension: 0 } },
                };
            });
            */
            $http.get(baseUrl + '/stats/LeadSummary').success(function (data) {
                $scope.LeadSummaryModel = data.Model;
                $scope.Leadlabels = data.Model.ThisWeekdays;
                $scope.Leadseries = ['This Week', 'Last Week'];
                $scope.Leaddata = [
                  data.Model.ThisWeekCounts,
                  data.Model.LastWeekCounts
                ];
                $scope.Leadcolors = ['#498be1', '#686868'],
                $scope.LeaddatasetOverride = [{ borderColor: "rgba(73, 139,225, 1)", pointBorderColor: "rgba(73, 139,225, 1)", pointBackgroundColor: "#fff", yAxisID: 'y-axis-1' }];
                $scope.Leadoptions = {

                    datasetFill: false,
                    scales: {
                        yAxes: [
                          {
                              id: 'y-axis-1',
                              type: 'linear',
                              display: true,
                              position: 'left'
                          }
                        ]
                    },
                    legend: {
                        display: true,
                        labels: {
                            padding: 50,
                            boxWidth: 20,
                        }
                    },

                    elements: { line: { tension: 0 } },
                };
                $scope.Loader.LeadSummery = false;
            });

            $http.post(baseUrl + '/stats/LeadFunnel', $scope.DateRangeFiltermodel).success(function (data) {
                $scope.LeadFunnelModel = data.Model;

                if (data.Model != null) {

                    $scope.LeadFunnelModel.NewWidth = 0;
                    $scope.LeadFunnelModel.ActiveWidth = 0;
                    $scope.LeadFunnelModel.DormantWidth = 0;

                    if ($scope.LeadFunnelModel.New != 0 || $scope.LeadFunnelModel.New >= 12) {
                        if ($scope.LeadFunnelModel.New >= 20) {
                            $scope.LeadFunnelModel.NewWidth = ($scope.LeadFunnelModel.New - 12) << 0;
                        }
                        else
                            $scope.LeadFunnelModel.NewWidth = $scope.LeadFunnelModel.New << 0;
                    }

                    if ($scope.LeadFunnelModel.Active != 0 || $scope.LeadFunnelModel.Active >= 12) {
                        if ($scope.LeadFunnelModel.Active >= 20) {
                            $scope.LeadFunnelModel.ActiveWidth = ($scope.LeadFunnelModel.Active - 12) << 0;
                        }
                        else
                            $scope.LeadFunnelModel.ActiveWidth = $scope.LeadFunnelModel.Active << 0;
                    }

                    if ($scope.LeadFunnelModel.Dormant != 0 || $scope.LeadFunnelModel.Dormant >= 12) {
                        if ($scope.LeadFunnelModel.Dormant >= 20) {
                            $scope.LeadFunnelModel.DormantWidth = ($scope.LeadFunnelModel.Dormant - 12) << 0;
                        }
                        else
                            $scope.LeadFunnelModel.DormantWidth = $scope.LeadFunnelModel.Dormant << 0;
                    }

                    var totalWidth = $scope.LeadFunnelModel.NewWidth + $scope.LeadFunnelModel.ActiveWidth + $scope.LeadFunnelModel.DormantWidth;
                    if (totalWidth < 98) {

                        var arrayZeroValueItems = new Array();
                        if ($scope.LeadFunnelModel.NewWidth !== 0) {
                            arrayZeroValueItems.push("NewWidth");
                        }
                        if ($scope.LeadFunnelModel.ActiveWidth !== 0) {
                            arrayZeroValueItems.push("ActiveWidth");
                        }
                        if ($scope.LeadFunnelModel.DormantWidth !== 0) {
                            arrayZeroValueItems.push("DormantWidth");
                        }

                        remainingValue = 98 - totalWidth - ((3 - arrayZeroValueItems.length) * 11.5);

                        if (arrayZeroValueItems.length > 0) {
                            angular.forEach(arrayZeroValueItems, function (value, key) {
                                $scope.LeadFunnelModel[value] = $scope.LeadFunnelModel[value] + (remainingValue / arrayZeroValueItems.length);
                            })
                        }
                    }
                }

                $scope.Loader.LeadFunnel = false;
            });

            $http.post(baseUrl + '/stats/getLead', $scope.DateRangeFiltermodel).success(function (data) {
                $scope.leadmodel = data.Model;
                $scope.Loader.LatestLeads = false;
            });

            $http.post(baseUrl + '/stats/getAccount', $scope.DateRangeFiltermodel).success(function (data) {
                $scope.accountmodel = data.Model;
                $scope.Loader.LatestAccounts = false;
            });


            loadTopAccounts();
            loadTopReps();
        }

        /* Bindable functions - Date range selector
        -----------------------------------------------*/
        $scope.endDateBeforeRender = endDateBeforeRender
        $scope.endDateOnSetTime = endDateOnSetTime
        $scope.startDateBeforeRender = startDateBeforeRender
        $scope.startDateOnSetTime = startDateOnSetTime

        function startDateOnSetTime() {
            $scope.$broadcast('start-date-changed');
        }

        function endDateOnSetTime() {
            $scope.$broadcast('end-date-changed');
        }

        function startDateBeforeRender($dates) {

            /* disable future dates */
            for (var i = 0; i < $dates.length; i++) {
                if (new Date().getTime() < $dates[i].utcDateValue) {
                    $dates[i].selectable = false;
                }
            }


            if ($scope.DateRangeFilter.DateTo) {
                var activeDate = moment($scope.DateRangeFilter.DateTo);

                $dates.filter(function (date) {
                    return date.localDateValue() >= activeDate.valueOf()
                }).forEach(function (date) {
                    date.selectable = false;
                })
            }
        }

        function endDateBeforeRender($view, $dates) {

            /* disable future dates */
            for (var i = 0; i < $dates.length; i++) {
                if (new Date().getTime() < $dates[i].utcDateValue) {
                    $dates[i].selectable = false;
                }
            }

            if ($scope.DateRangeFilter.DateFrom) {
                var activeDate = moment($scope.DateRangeFilter.DateFrom).subtract(1, $view).add(1, 'minute');

                $dates.filter(function (date) {
                    return date.localDateValue() <= activeDate.valueOf()
                }).forEach(function (date) {
                    date.selectable = false;
                })
            }
        }


        function loadGraph($scope, firstLoad) {

            //if ($scope.lookUps.DashboardSalesDateType == null) {
            //    //$http.get(baseUrl + '/stats/getdashboardsalesdatetype/' + $scope.serviceGraphFilter.ServiceId).success(function (data) {
            //    //    if (data != null) {
            //    //        var list = data.List;
            //    //        $scope.lookUps.DashboardSalesDateType = list;
            //    //        $.each($scope.lookUps.DashboardSalesDateType, function (i, obj) {

            //    //            if (obj.IsSelected == true) {
            //    //                $scope.PeriodicTreandsFilter.ViewBy = obj.Value;
            //    //                $scope.DateRangeFiltermodel.DateType = obj.Value;
            //    //            }
            //    //        });

            //    //    }


            //    //    $http.get(baseUrl + '/stats/getdashboardsalestotal/' + $scope.serviceGraphFilter.ServiceId).success(function (data) {
            //    //        var list = data.List;
            //    //        $scope.lookUps.DashboardSalesTotal = list;

            //    //        $.each($scope.lookUps.DashboardSalesTotal, function (i, obj) {

            //    //            if (obj.IsSelected == true) {
            //    //                $scope.PeriodicTreandsFilter.Total = obj.Value;
            //    //                $scope.DateRangeFiltermodel.Total = obj.Value;
            //    //            }
            //    //        });

            //    //        //loadGraph($scope, true);

            //    //    });
            //    //});


            //}

            //else 
            {

                //console.log('$scope.lookUps.DashboardSalesDateType', $scope.lookUps.DashboardSalesDateType);
                //console.log('$scope.lookUps.DashboardSalesTotal', $scope.lookUps.DashboardSalesTotal);


                $scope.DateRangeFiltermodel = {
                    DateFrom: toLocal($scope.DateRangeFilter.DateFrom),
                    DateTo: toLocal($scope.DateRangeFilter.DateTo),
                    ViewBy: $scope.PeriodicTreandsFilter.ViewBy,


                    DateType: $scope.PeriodicTreandsFilter.DateType == null ? "ReceivedDate" : $scope.PeriodicTreandsFilter.DateType,
                    Total: $scope.PeriodicTreandsFilter.Total

                    //DateType: $scope.PeriodicTreandsFilter.DateType == null ? "PaidDate" : $scope.PeriodicTreandsFilter.DateType,
                    //Total: $scope.PeriodicTreandsFilter.PaidAmount

                };





                if ($scope.serviceGraphFilter.ServiceId != null) {

                    $timeout(function () {

                        $('.graphFilter').each(function (i, elm) {
                            $(elm).prop('disabled', true)

                        });

                    }, 10)

                    //console.log('$scope.DateRangeFiltermodel', $scope.DateRangeFiltermodel);

                    $http.post(baseUrl + '/stats/SalesPeriodicTrends/' + $scope.serviceGraphFilter.ServiceId, $scope.DateRangeFiltermodel).success(function (data) {
                        //console.log(data);
                        $scope.datapoints = JSON.parse(data.Model.DataPoints);
                        $scope.datacolumns = JSON.parse(data.Model.DataColumns);
                        $scope.dataColors = data.Model.ColumnColors;
                        $scope.totalSales = data.Model.TotalSales;
                        $scope.totalBilled = data.Model.SummaryTotals.TotalBilled;
                        $scope.totalPaid = data.Model.SummaryTotals.TotalPaid;
                        if ($scope.trendzChart) {
                            $scope.trendzChart.load(
                                {
                                    colors: JSON.parse(data.Model.ColumnColors),
                                }
                                );
                            //console.log(' $scope.trendzChart.flush();');
                            $scope.trendzChart.flush();
                        }
                        $scope.datax = { "id": "Month" };
                        $scope.Loader.PeriodicData = false;

                        $timeout(function () {

                            $('.graphFilter').each(function (i, elm) {
                                $(elm).prop('disabled', false)

                            });

                        }, 10)

                    }).error(function (data, status, headers, config) {
                        $scope.Loader.PeriodicData = false;
                    });

                }
            }

        }

        function loadTopReps() {
            if ($scope.repFilter.ServiceId !== null && $scope.repFilter.ServiceId != 0) {
                $scope.Loader.TopReps = true;
                $http.post(baseUrl + '/stats/TopReps/' + $scope.repFilter.ServiceId + '?v130', $scope.DateRangeFiltermodel).success(function (data) {
                    $scope.TopRepsmodel = data.Model;
                    $scope.Loader.TopReps = false;
                });
            }
            else {
                $scope.Loader.TopReps = false;
                $scope.TopRepsmodel = null;
            }
        }

        function LoadPeriodicTrends() {
            console.log($scope.periodicTrends.ServiceId);
            if ($scope.periodicTrends.ServiceId !== null && $scope.periodicTrends.ServiceId != 0) {
                $scope.Loader.DataTrends = true;
                $http.post(baseUrl + '/stats/PeriodicDataTrends/' + $scope.periodicTrends.ServiceId + '?v130').success(function (data) {
                    console.log(data);
                    $scope.DataTrendsmodel = data.Model;
                    $scope.Loader.DataTrends = false;
                });
            }
            else {
                $scope.Loader.DataTrends = false;
                $scope.DataTrendsmodel = null;
            }
        }

        function loadTopAccounts() {
            if ($scope.accountFilter.ServiceId !== 0) {
                $scope.Loader.TopAccounts = true;
                $http.post(baseUrl + '/stats/TopAccounts/' + $scope.accountFilter.ServiceId + '?v130', $scope.DateRangeFiltermodel).success(function (data) {
                    $scope.Topaccountmodel = data.Model;
                    $scope.Loader.TopAccounts = false;
                });
            }
            else {
                $scope.Loader.TopAccounts = false;
                $scope.Topaccountmodel = null;
            }
        }

        function loadLatestLeads() {

        }

        function loadLatestAccounts() {

        }

        function loadLeadSummary() {

        }

        function loadLeadFunnal() {

        }

    }]);
