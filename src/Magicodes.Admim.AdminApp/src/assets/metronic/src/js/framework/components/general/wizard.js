(function($) {
    // plugin setup
    $.fn.mWizard = function(options) {
        //== Main object
        var wizard = this;
        var element = $(this);

        /********************
         ** PRIVATE METHODS
         ********************/
        var Plugin = {
            /**
             * Run
             */
            run: function (options) {
                if (!element.data('wizard')) {                      
                    //== Create instance
                    Plugin.init(options);
                    Plugin.build();
                    
                    //== Assign instance to the element                    
                    element.data('wizard', wizard);
                } else {
                    // get instance from the element
                    wizard = element.data('wizard');
                }               

                return wizard;
            },

            /**
             * Initialize Form Wizard
             */
            init: function(options) {
                //== Elements
                wizard.steps = wizard.find('.m-wizard__step');
                wizard.progress = wizard.find('.m-wizard__progress .progress-bar'); 
                wizard.btnSubmit = wizard.find('[data-wizard-action="submit"]'); 
                wizard.btnNext = wizard.find('[data-wizard-action="next"]'); 
                wizard.btnPrev = wizard.find('[data-wizard-action="prev"]'); 
                wizard.btnLast = wizard.find('[data-wizard-action="last"]'); 
                wizard.btnFirst = wizard.find('[data-wizard-action="first"]');  

                //== Merge default and user defined options
                wizard.options = $.extend(true, {}, $.fn.mWizard.defaults, options);

                //== Variables
                wizard.events = [];
                wizard.currentStep = 1;
                wizard.totalSteps = wizard.steps.length;  

                //== Init current step
                if (wizard.options.startStep > 1) {
                    Plugin.goTo(wizard.options.startStep);
                }       

                //== Init UI
                Plugin.updateUI();
            },

            /**
             * Build Form Wizard
             */
            build: function() {
                //== Next button event handler
                wizard.btnNext.on('click', function (e) {
                    e.preventDefault();
                    Plugin.goNext();
                });

                //== Prev button event handler
                wizard.btnPrev.on('click', function (e) {
                    e.preventDefault();
                    Plugin.goPrev();
                });

                //== First button event handler
                wizard.btnFirst.on('click', function (e) {
                    e.preventDefault();
                    Plugin.goFirst();
                });

                //== Last button event handler
                wizard.btnLast.on('click', function (e) {
                    e.preventDefault();
                    Plugin.goLast();
                });

                wizard.find('.m-wizard__step a.m-wizard__step-number').on('click', function() {
                    var step = $(this).parents('.m-wizard__step');
                    var num;
                    $(this).parents('.m-wizard__steps').find('.m-wizard__step').each(function(index) {
                        if (step.is( $(this) )) {
                            num = (index + 1);
                            return;
                        }
                    });

                    if (num) {
                        Plugin.goTo(num);
                    }                    
                });
            },

            /**
             * Sync object instance
             */
            sync: function () {
                $(element).data('wizard', wizard);
            }, 

            /**
             * Handles wizard click toggle
             */
            goTo: function(number) {
                //== Skip if this step is already shown
                if (number === wizard.currentStep) {
                    return;
                }

                //== Validate step number
                if (number) {
                    number = parseInt(number); 
                } else {
                    number = Plugin.getNextStep();
                }

                //== Before next and prev events
                var callback;

                if (number > wizard.currentStep) {
                    callback = Plugin.eventTrigger('beforeNext');
                } else {
                    callback = Plugin.eventTrigger('beforePrev');
                }

                //== Continue if no exit
                if (callback !== false) {
                    //== Set current step
                    wizard.currentStep = number;

                    //== Update UI
                    Plugin.updateUI();             

                    //== Trigger change event
                    Plugin.eventTrigger('change')       
                }
                
                //== After next and prev events
                if (number > wizard.startStep) {
                    Plugin.eventTrigger('afterNext');
                } else {
                    Plugin.eventTrigger('afterPrev');
                }

                return wizard;
            },

            updateUI: function(argument) {
                //== Update progress bar
                Plugin.updateProgress();

                //== Show current target content
                Plugin.handleTarget();

                //== Set classes
                Plugin.setStepClass();

                //== Apply nav step classes
                wizard.find('.m-wizard__step').removeClass('m-wizard__step--current').removeClass('m-wizard__step--done');
                for (var i = 1; i < wizard.currentStep; i++) {
                    wizard.find('.m-wizard__step').eq(i - 1).addClass('m-wizard__step--done');
                }
                wizard.find('.m-wizard__step').eq(wizard.currentStep - 1).addClass('m-wizard__step--current');
            },

            /**
             * Check last step
             */
            isLastStep: function() {
                return wizard.currentStep === wizard.totalSteps;
            },

            /**
             * Check first step
             */
            isFirstStep: function() {
                return wizard.currentStep === 1;
            },

            /**
             * Check between step
             */
            isBetweenStep: function() {
                return Plugin.isLastStep() === false && Plugin.isFirstStep() === false;
            },

            /**
             * Set step class
             */
            setStepClass: function() {
                if (Plugin.isLastStep()) {
                    element.addClass('m-wizard--step-last');
                } else {
                    element.removeClass('m-wizard--step-last');
                }

                if (Plugin.isFirstStep()) {
                    element.addClass('m-wizard--step-first');
                } else {
                    element.removeClass('m-wizard--step-first');
                }

                if (Plugin.isBetweenStep()) {
                    element.addClass('m-wizard--step-between');
                } else {
                    element.removeClass('m-wizard--step-between');
                }
            },

            /**
             * Go to the next step
             */
            goNext: function() {
                return Plugin.goTo( Plugin.getNextStep() );
            },

            /**
             * Go to the prev step
             */
            goPrev: function() {
                return Plugin.goTo( Plugin.getPrevStep() );
            },

            /**
             * Go to the last step
             */
            goLast: function() {
                return Plugin.goTo( wizard.totalSteps );
            },

            /**
             * Go to the first step
             */
            goFirst: function() {
                return Plugin.goTo( 1 );
            },

            /**
             * Set progress
             */
            updateProgress: function() {
                //== Calculate progress position

                if (!wizard.progress) {
                    return;
                } 

                //== Update progress
                if (element.hasClass('m-wizard--1')) {
                    var width = 100 * ((wizard.currentStep) / (wizard.totalSteps));
                    var offset = element.find('.m-wizard__step-number').width();
                    wizard.progress.css('width', 'calc(' + width + '% + ' + (offset / 2)  + 'px)');
                } else if (element.hasClass('m-wizard--2')) {
                    if (wizard.currentStep === 1) {
                        return;
                    }

                    var step = element.find('.m-wizard__step').eq(0);
                    var progress = (wizard.currentStep - 1) * (100 * (1 / (wizard.totalSteps - 1)));

                    if (mUtil.isInResponsiveRange('minimal-desktop-and-below')) {  
                        wizard.progress.css('height', progress + '%');
                    } else {
                        wizard.progress.css('width', progress + '%');
                    }
                } else {
                    var width = 100 * ((wizard.currentStep) / (wizard.totalSteps));
                    wizard.progress.css('width', width + '%'); 
                }             
            },

            /**
             * Show/hide target content
             */
            handleTarget: function() {
                var step = wizard.steps.eq(wizard.currentStep - 1);
                var target = element.find( step.data('wizard-target') );

                element.find('.m-wizard__form-step--current').removeClass('m-wizard__form-step--current');
                target.addClass('m-wizard__form-step--current');
            },

            /**
             * Get next step
             */
            getNextStep: function() {
                if (wizard.totalSteps >= (wizard.currentStep + 1)) {
                    return wizard.currentStep + 1;
                } else {
                    return wizard.totalSteps;
                } 
            },

            /**
             * Get prev step
             */
            getPrevStep: function() {
                if ((wizard.currentStep - 1) >= 1) {
                    return wizard.currentStep - 1;
                } else {
                    return 1;
                } 
            },

            /**
             * Trigger event
             */
            eventTrigger: function(name) {
                for (i = 0; i < wizard.events.length; i++) {
                    var event = wizard.events[i];
                    if (event.name == name) {
                        if (event.one == true) {
                            if (event.fired == false) {
                                wizard.events[i].fired = true;
                                return event.handler.call(this, wizard);
                            }
                        } else {
                            return  event.handler.call(this, wizard);
                        }
                    }
                }
            },

            /**
             * Register event
             */
            addEvent: function(name, handler, one) {
                wizard.events.push({
                    name: name,
                    handler: handler,
                    one: one,
                    fired: false
                });

                Plugin.sync();
            }
        };

        //== Main variables
        var the = this;
        
        //== Init plugin
        Plugin.run.apply(this, [options]);

        /********************
         ** PUBLIC API METHODS
         ********************/

        /**
         * Go to the next step 
         */
        wizard.goNext =  function () {
            return Plugin.goNext();
        };

        /**
         * Go to the prev step 
         */
        wizard.goPrev =  function () {
            return Plugin.goPrev();
        };

        /**
         * Go to the last step 
         */
        wizard.goLast =  function () {
            return Plugin.goLast();
        };

        /**
         * Go to the first step 
         */
        wizard.goFirst =  function () {
            return Plugin.goFirst();
        };

         /**
         * Go to a step
         */
        wizard.goTo =  function ( number ) {
            return Plugin.goTo( number );
        };

        /**
         * Get current step number 
         */
        wizard.getStep =  function () {
            return wizard.currentStep;
        };

        /**
         * Check last step 
         */
        wizard.isLastStep =  function () {
            return Plugin.isLastStep();
        };

        /**
         * Check first step 
         */
        wizard.isFirstStep =  function () {
            return Plugin.isFirstStep();
        };

        /**
         * Attach event
         * @returns {mwizard}
         */
        wizard.on =  function (name, handler) {
            return Plugin.addEvent(name, handler);
        };

        /**
         * Attach event that will be fired once
         * @returns {mwizard}
         */
        wizard.one =  function (name, handler) {
            return Plugin.addEvent(name, handler, true);
        };   

        return wizard;
    };

    //== Default options
    $.fn.mWizard.defaults = {
        startStep: 1
    }; 
}(jQuery));