(function () {
    'use strict';

    angular.module('UpoladSpreadsheet')

    .controller('UpoladSpreadsheetController',
        ['$scope', 'UpoladSpreadsheetService', 'usSpinnerService', function ($scope, UpoladSpreadsheetService, usSpinnerService) {
            $scope.upload = function () {
                //$scope.dataLoading = true;
                var fd = new FormData();
                //Take the first selected file
                fd.append("merchantkey", $scope.text);
                fd.append("file", $scope.myFile);
                //fd.append("file1" + 0, files[0]);
                console.log($scope.loading);
                UpoladSpreadsheetService.UploadServer($scope.text, $scope.myFile, usSpinnerService);

            };

        }]);

})();