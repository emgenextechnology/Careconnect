
(function () {
    'use strict';

    angular.module('clear-input', []);

    angular.module('clear-input').directive('clearable', clearable);

    function clearable() {
        var directive = {
            restrict: 'A',
            require: 'ngModel',
            link: link
        };
        return directive;

        function link(scope, elem, attrs, ctrl) {
            elem.addClass('clearable');

            elem.bind('input', function () {
                elem[toggleClass(elem.val())]('x');
            });

            elem.on('mousemove', function (e) {
                if (elem.hasClass('x')) {
                    elem[toggleClass(this.offsetWidth - 25 < e.clientX - this.getBoundingClientRect().left)]('onX');
                }
            });

            elem.on('click', function (e) {
                if (elem.hasClass('onX')) {
                    elem.removeClass('x onX').val(undefined);
                    ctrl.$setViewValue(undefined);
                    ctrl.$render();
                    scope.$digest();
                }
            });

            function toggleClass(v) {
                return v ? 'addClass' : 'removeClass';
            }
        }
    }



    var style = document.createElement('style');
    style.type = 'text/css';
    style.innerHTML =
        '.clearable {' +
        'background-image: url(data:image/gif;base64,R0lGODlhBwAHAIAAAP///5KSkiH5BAAAAAAALAAAAAAHAAcAAAIMTICmsGrIXnLxuDMLADs=);' +
        'background-repeat: no-repeat;' +
        'background-attachment: initial;' +
        'background-position: right -10px center;' +
        '}' +
        '.clearable.x {' +
        'transition: background 0.4s;' +
        'background-position: right 10px center;' +
        '}' +
        '.clearable.onX {' +
        'cursor: pointer;' +
        '}';

    document.getElementsByTagName('head')[0].appendChild(style);


})();