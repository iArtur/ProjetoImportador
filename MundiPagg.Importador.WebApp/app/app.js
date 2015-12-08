(function () {
    'use strict';

    // declare modules
    angular.module('Authentication', []);

    angular.module('UpoladSpreadsheet', []);

    angular.module('app', [
        'Authentication',
        'UpoladSpreadsheet',
        'ngRoute',
        'ngCookies',
        'ui.bootstrap',
        'angularSpinner'
    ])

     .directive('fileModel', ['$parse', function ($parse) {
         return {
             restrict: 'A',
             link: function (scope, element, attrs) {
                 var model = $parse(attrs.fileModel);
                 var modelSetter = model.assign;

                 element.bind('change', function () {
                     scope.$apply(function () {
                         modelSetter(scope, element[0].files[0]);
                     });
                 });
             }
         };
     }])

    .config(['$routeProvider', function ($routeProvider) {

        $routeProvider
            .when('/login', {
                controller: 'LoginController',
                templateUrl: 'app/modules/authentication/views/Login.html'
            })

            .when('/', {
                controller: 'UpoladSpreadsheetController',
                templateUrl: 'app/modules/upoladSpreadsheet/views/Upload.html'
            })

            .otherwise({ redirectTo: '/login' });
    }])

    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.useXDomain = true;
        delete $httpProvider.defaults.headers.common['X-Requested-With'];
    }
    ])

    .config(['usSpinnerConfigProvider', function (usSpinnerConfigProvider) {
        usSpinnerConfigProvider.setDefaults({color: 'blue'});
    }
    ])

    .run(['$rootScope', '$location', '$cookieStore', '$http',
        function ($rootScope, $location, $cookieStore, $http) {
            // keep user logged in after page refresh
            $rootScope.globals = $cookieStore.get('globals') || {};
            if ($rootScope.globals.currentUser) {
                $http.defaults.headers.common['Authorization'] = 'Basic ' + $rootScope.globals.currentUser.authdata; // jshint ignore:line
            }

            $rootScope.$on('$locationChangeStart', function (event, next, current) {
                // redirect to login page if not logged in
                if ($location.path() !== '/login' && !$rootScope.globals.currentUser) {
                    $location.path('/login');
                }
            });
        }]);
})();