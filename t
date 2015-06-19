(function () {
    var CandidateTable1 = function ($http) {
        factory ={};
        factory.getCandidateTable1 = function (eventid) {
            return $http({				  
                url: msApiUrl + "api/InterviewFeedback/CandidatesDetailForHistory/?eid="+ eventid,
                method: "GET",
                crossDomain: true,
                headers: { 'UserToken': GetCookie("CIUserToken") },
                dataType: "json",
                contentType: "application/json"
            })
        };
        return factory;
    }

    function GetCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    };

    angular.module('main').factory('CandidateTable1', CandidateTable1);

}());
