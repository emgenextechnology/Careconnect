/**
 * Binds a TinyMCE widget to <textarea> elements.
 */
angular.module('ui.tinymce', [])
    .value('uiTinymceConfig', {})
    .directive('uiTinymce', ['uiTinymceConfig', function (uiTinymceConfig) {
        uiTinymceConfig = uiTinymceConfig || {};
        var generatedIds = 0;
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ngModel) {
                var expression, options, tinyInstance;
                // generate an ID if not present
                if (!attrs.id) {
                    attrs.$set('id', 'uiTinymce' + generatedIds++);
                }

                if (tinyMCE.activeEditor != null) {
                    tinymce.EditorManager.execCommand('mceRemoveEditor', true, 'tinymce');
                }
                options = {
                    // Update model when calling setContent (such as from the source editor popup)
                    setup: function (ed) {
                        ed.on('init', function (args) {
                            ngModel.$render();
                        });

                        // Update model on button click
                        ed.on('ExecCommand', function (e) {
                            ed.save();
                            ngModel.$setViewValue(elm.val());
                            if (!scope.$$phase) {
                                scope.$apply();
                            }
                        });
                        // Update model on keypress
                        ed.on('KeyUp', function (e) {
                            //console.log(ed.isDirty());
                            ed.save();
                            ngModel.$setViewValue(elm.val());
                            if (!scope.$$phase) {
                                scope.$apply();
                            }
                        });
                        ed.on("mouseEnter", function (e) {
                            e.target.style.cursor = "text"
                        });
                    },
                    mode: 'exact',
                    statusbar: false,
                    toolbar: "undo redo | styleselect | bold italic underline | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent ",
                    //toolbar1: "bold italic underline | alignleft aligncenter alignright alignjustify | styleselect formatselect fontselect fontsizeselect| forecolor",
                    elements: attrs.id,

                    plugins: "paste",
                    //plugins: "paste,colorpicker,textcolor",
                    paste_retain_style_properties: "color font-size forecolor",

                    //paste_word_valid_elements: "ul,li,b,strong,i,em,h1,h2,table,tr,td,br",
                    //paste_use_dialog: false,
                    //cleanup_on_startup: true,
                    //paste_create_paragraphs: true,
                    //paste_create_linebreaks: true,
                    //paste_retain_style_properties: "all",
                    paste_auto_cleanup_on_paste: true,

                };
                if (attrs.uiTinymce) {
                    expression = scope.$eval(attrs.uiTinymce);
                } else {
                    expression = {};
                }
                angular.extend(options, uiTinymceConfig, expression);
                setTimeout(function () {
                    tinymce.init(options);
                });


                ngModel.$render = function () {
                    if (!tinyInstance) {
                        tinyInstance = tinymce.get(attrs.id);
                    }
                    if (tinyInstance) {
                        tinyInstance.setContent(ngModel.$viewValue || '');
                    }
                };
            }
        };
    }]);