Skip to content
Personal Open source Business Explore
Sign upSign inPricingBlogSupport
This repository
Search
Watch 1  Star 32  Fork 15 tushariscoolster/angular-nicescroll
Code  Issues 8  Pull requests 0  Pulse  Graphs
Branch: master Find file Copy pathangular-nicescroll/angular-nicescroll.js
139b037  on Dec 1, 2015
@tushariscoolster tushariscoolster Merge pull request #3 from sufiiiyan/master
2 contributors @tushariscoolster @sufiiiyan
RawBlameHistory     55 lines (39 sloc)  1.37 KB
(function () {
    'use strict';

    angular
        .module('angular-nicescroll', [])
        .directive('ngNicescroll', ngNicescroll);

    ngNicescroll.$inject = ['$rootScope','$parse'];

    /* @ngInject */
    function ngNicescroll($rootScope,$parse) {
        // Usage:
        //
        // Creates:
        //
        var directive = {
            link: link
        };
        return directive;

        function link(scope, element, attrs, controller) {

            var niceOption = scope.$eval(attrs.niceOption)

            var niceScroll = $(element).niceScroll(niceOption);
            var nice = $(element).getNiceScroll();
          
            if (attrs.niceScrollObject)  $parse(attrs.niceScrollObject).assign(scope, nice);
       
            // on scroll end
            niceScroll.onscrollend = function (data) {
                if (this.newscrolly >= this.page.maxh) {
                    if (attrs.niceScrollEnd) scope.$evalAsync(attrs.niceScrollEnd);

                }
                if (data.end.y <= 0) {
                    // at top
                    if (attrs.niceScrollTopEnd) scope.$evalAsync(attrs.niceScrollTopEnd);
                }
            };


            scope.$on('$destroy', function () {
                if (angular.isDefined(niceScroll.version)) {
                    niceScroll.remove();
                }
            })


        }
    }


})();
Contact GitHub API Training Shop Blog About
© 2016 GitHub, Inc. Terms Privacy Security Status Help