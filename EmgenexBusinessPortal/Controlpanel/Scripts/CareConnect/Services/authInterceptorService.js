
careconnect.factory('authInterceptorService', ['$q', '$injector', '$location', 'localStorageService',  function ($q, $injector, $location, localStorageService) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {
        config.headers = config.headers || {};
       
        config.headers.User = currentUserName;
        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }
        return config;
    }

    var _responseError = function (rejection) {

        //debugger;
        if (rejection.status == 401 || rejection.status == 403 || rejection.status == 500) {
            alert("You've been logged out for inactivity. Please refresh page to continue.");
            //window.location = "/login";
        }

        if (rejection == null) {

            //alert('logout');

            authService.logOut();
            authData = null;
            $location.path('/login');
        }

        if (rejection.status === 401) {
            var authService = $injector.get('authService');
            var authData = localStorageService.get('authorizationData');

            if (authData) {
                if (authData.useRefreshTokens) {
                    $location.path('/refresh');
                    return $q.reject(rejection);
                }
            }
            authService.logOut();
            $location.path('/login');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);