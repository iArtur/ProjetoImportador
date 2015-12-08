(function () {
    'use strict';
    var app = angular.module('app');
    app.constant('routes', getRoutes());
    app.config(['$routeProvider', 'routes', routeConfigurator]);
    function routeConfigurator($routeProvider, routes) {
        routes.forEach(function (r) {
            $routeProvider.when(r.url, r.config);
        });
    }

    function getRoutes() {
        return [
            {
                url: '/login',
                config: {
                    title: 'login',
                    templateUrl: '.app/Modules/authentication/views/Login.html',
                    settings: {
                        nav: 3,
                        content: 'Login'
                    }
                },
                showOnMenu: true
            }
        ];
    }
})();