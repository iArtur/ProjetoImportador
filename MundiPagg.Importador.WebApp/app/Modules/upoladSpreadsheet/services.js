(function () {
    'use strict'

    angular.module('UpoladSpreadsheet')

  .factory('UpoladSpreadsheetService',
        ['$http',
        function ($http) {
            var service = {};
            service.UploadServer = function (merchantKey, file, loading, toast) {
                console.log(file);
                var formData = new FormData();
                formData.append("merchantkey", merchantKey);
                formData.append("file1", file);
                debugger;
                loading.spin('spinner-1');
                $http({
                    method: 'POST',
                    url: 'http://localhost:60225/api/Upload',
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    },
                    data: {
                        merchantkey: merchantKey,
                        file1: file
                    },
                    transformRequest: function (data, headersGetter) {
                        var formData = new FormData();
                        angular.forEach(data, function (value, key) {
                            formData.append(key, value);
                        });

                        var headers = headersGetter();
                        delete headers['Content-Type'];

                        return formData;
                    }
                })
                .success(function (response) {
                    if (response.Data == "OK") {
                        //alert('Importação feita com sucesso!');
                        toastr["success"]("Importação feita com sucesso!");
                        //toast.create('Importação feita com sucesso!');
                    } else if (response.Data == "Error") {
                        loading.stop('spinner-1');
                        toastr["error"]("Erro na importação!");
                    }
                })
                .finally(function () {
                    loading.stop('spinner-1');
                });
            };
            return service;
        }])
})();