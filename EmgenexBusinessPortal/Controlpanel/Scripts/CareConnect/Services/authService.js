
careconnect.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', '$cookies', function ($http, $q, localStorageService, ngAuthSettings, $cookies) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {};
    localStorageService.remove('authorizationData');

    var _authentication = {
       
        isAuth: false,
        userName: "",
        useRefreshTokens: false,
        fullName: "",
        ProfilepicUrl: "",
        userId:"",
        accessToken:'',
        privileges: [],
        roles:[],
        departments: [],
        businessName: '',
        businessUrl: '',
        businessDomain: ''
    };
    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };
    
    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {
        
        console.log('___login___');

        if (loginTriggered == true)
            return;
        //console.log('___ loginTriggered ___');
        //var authData = localStorageService.get('authorizationData');

        //if (authData) {
        //    return;
        //}
        loginTriggered = true;

        console.log(currentUserName);

        var data = {} 
        if (currentUserName == '')
            data = { "granttype": "password", 'UserName': loginData.userName, 'Password': loginData.password };

        //var data = "grant_type=password&UserName=" + loginData.userName + "&Password=" + loginData.password;

        if (loginData.useRefreshTokens) {
            data = data + "&client_id=" + ngAuthSettings.clientId;
        }

        var deferred = $q.defer();

        
        var url = getAppUrl('api/account/tokens');// serviceBase + ;
        if (currentUserName != '') {
            url = getAppUrl('api/profile');

            //url = serviceBase + 'api/account/cookie?userName='+currentUserName;
        }

        $http.get(url, data, { headers: { 'Content-Type': 'application/json' } }, { withCredentials: true }).success(function (response) {
            loginTriggered = false;
            if (!response.IsSuccess) {
                  alert("login failed");
                _logOut();
                deferred.reject({ error_description: response.Message });
                return;
            }

            if (!response.Model) {
                  //alert("afetr response");
                _logOut();
                deferred.reject({ error_description: response.Message });
            }

            if (loginData.useRefreshTokens) {
                  //alert("refresh token");
                localStorageService.set('authorizationData', {
                    token: response.Model.AccessToken,
                    userName: loginData.userName,
                    fullName: response.Model.FullName,
                    businessName: response.Model.Business,
                    domainUrl: response.Model.DomainUrl,
                    relativeUrl: response.Model.RelativeUrl,
                    profilepicUrl: "",
                    privileges: response.Model.UserPrivilages,
                    FirstName: response.Model.FirstName,
                    MiddleName: response.Model.MiddleName,
                    LastName: response.Model.LastName,
                    PhoneNumber: response.Model.PhoneNumber,
                    SalesGroupBy: response.Model.SalesGroupBy,
                    FilePath: response.Model.FilePath
                });
            }
            else {
                 //alert("else refresh token");
                localStorageService.set('authorizationData', {
                    token: response.Model.AccessToken,
                    userName: loginData.userName,
                    fullName: response.Model.FullName,
                    businessName: response.Model.Business,
                    domainUrl: response.Model.DomainUrl,
                    relativeUrl: response.Model.RelativeUrl,
                    profilepicUrl: "",
                    privileges: response.Model.UserPrivilages,
                    FirstName: response.Model.FirstName,
                    MiddleName: response.Model.MiddleName,
                    LastName: response.Model.LastName,
                    PhoneNumber: response.Model.PhoneNumber,
                    SalesGroupBy: response.Model.SalesGroupBy,
                    FilePath: response.Model.FilePath,
                    roles: response.Model.UserRoles,
                    departments: response.Model.UserDepartments,
                    defaultDateRange: response.Model.DefaultDateRange
                });
            }

            
          //alert(2344)
            
            _authentication.isAuth = response.Model.UserRoles.length>0||response.Model.UserDepartments.length>0|| response.Model.UserPrivilages.length>0;
            _authentication.userName = loginData.userName;
            _authentication.useRefreshTokens = loginData.useRefreshTokens;
            _authentication.fullName = response.Model.FullName;
            _authentication.userId = response.Model.UserId;
            _authentication.privileges = response.Model.UserPrivilages;
            _authentication.roles = response.Model.UserRoles;
            _authentication.departments = response.Model.UserDepartments;
            _authentication.businessName = response.Model.Business;
            _authentication.defaultDateRange = response.Model.DefaultDateRange;
            _authentication.domainUrl = response.Model.DomainUrl;
            _authentication.relativeUrl = response.Model.RelativeUrl;

            _authentication.FirstName = response.Model.FirstName;
            _authentication.MiddleName = response.Model.MiddleName;
            _authentication.LastName = response.Model.LastName;
            _authentication.PhoneNumber = response.Model.PhoneNumber;
            _authentication.SalesGroupBy = response.Model.SalesGroupBy;
            _authentication.FilePath = response.Model.FilePath;


            deferred.resolve(response);
            //alert(23445568)
        }).error(function (err, status) {
            _logOut();
        });

        return deferred.promise;

    };

    var _logOut = function () {
        console.log('-------Logout');
        localStorageService.remove('authorizationData');

        var _authentication = {

            isAuth: false,
            userName: "",
            useRefreshTokens: false,
            fullName: "",
            ProfilepicUrl: "",
            userId: "",
            accessToken: '',
            privileges: [],
            roles: [],
            departments: [],
            businessName: '',
            businessUrl: '',
            businessDomain: '',
            defaultDateRange:''
        };

        //_authentication.isAuth = false;
        //_authentication.userName = "";
        //_authentication.useRefreshTokens = false;
        //_authentication.fullName = "";
        //_authentication.ProfilepicUrl = "";
        //_authentication.userId = "";
        //_authentication.businessName= '',
        //_authentication.businessUrl= '',
        //_authentication.businessDomain= ''
        
    };

    var _fillAuthData = function () {
        var authData = localStorageService.get('authorizationData');      

        if (authData) {
            _authentication.isAuth = ((authData.roles && authData.roles.length > 0) || (authData.departments&&authData.departments.length > 0) || (authData.privileges&&authData.privileges.length > 0));
            _authentication.userName = authData.userName;
            _authentication.useRefreshTokens = authData.useRefreshTokens;
            _authentication.fullName = authData.fullName;
            _authentication.ProfilepicUrl = authData.ProfilepicUrl;
            _authentication.userId=authData.userId
            _authentication.privileges = authData.privileges;
            _authentication.roles = authData.roles;
            _authentication.departments = authData.departments;
            _authentication.businessName = authData.business;
            _authentication.domainUrl = authData.domainUrl;
            _authentication.relativeUrl = authData.relativeUrl;
            _authentication.defaultDateRange = authData.defaultDateRange;
            _authentication.FirstName = authData.FirstName;
            _authentication.MiddleName = authData.MiddleName;
            _authentication.LastName = authData.LastName;
            _authentication.PhoneNumber = authData.PhoneNumber;
            _authentication.SalesGroupBy = authData.SalesGroupBy;
            _authentication.FilePath = authData.FilePath;
        }
    };

    var _refreshToken = function () {
        var deferred = $q.defer();
        var authData = localStorageService.get('authorizationData');

        if (authData) {

            if (authData.useRefreshTokens) {

                var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + ngAuthSettings.clientId;

                localStorageService.remove('authorizationData');

                $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                    localStorageService.set('authorizationData', { token: response.Model.AccessToken, userName: response.userName, refreshToken: response.refresh_token, useRefreshTokens: true, fullNmae: response.Model.FirstName + " " + response.Model.LastName, ProfilepicUrl: "" });

                    deferred.resolve(response);

                }).error(function (err, status) {
                    _logOut();
                    deferred.reject(err);
                });
            }
        }

        return deferred.promise;
    };

    var _obtainAccessToken = function (externalData) {
      
        var deferred = $q.defer();

        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {

            localStorageService.set('authorizationData', { token: response.Model.AccessToken, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).success(function (response) {

            localStorageService.set('authorizationData', { token: response.Model.AccessToken, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;


            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    ///external login

    var _Externallogin = function (FacebookUserInfo) {
        
        var deferred = $q.defer();

        $http.post(serviceBase + 'api/Account/LoginWithExternal', FacebookUserInfo, { headers: { 'Content-Type': 'application/json' } }).success(function (response) {
          
            localStorageService.set('authorizationData', { token: response.Model.BearerToken, userName: response.Model.UserName, refreshToken: "", useRefreshTokens: false, fullName: response.Model.FirstName + " " + response.Model.LastName, ProfilepicUrl: response.Model.ProfilePicPath, userId: response.Model.UserId });
            
            _authentication.isAuth = true;
            _authentication.userName = response.Model.UserName;
            _authentication.useRefreshTokens = false;
            _authentication.fullName = response.Model.FirstName + " " + response.Model.LastName;
            _authentication.ProfilepicUrl = response.Model.ProfilePicPath;
            _authentication.userId = response.Model.UserId;
            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };




    //
    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.refreshToken = _refreshToken;

    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.registerExternal = _registerExternal;
    authServiceFactory.Externallogin = _Externallogin;


    return authServiceFactory;
}]);