(function($){
  "use strict";
	
	// on ready function
	jQuery(document).ready(function($) {
   		var $this = $(window);
	
	// Magnific Popup js
		$('.popup-gallery').magnificPopup({
			delegate: 'a',
			type: 'image',
			tLoading: 'Loading image #%curr%...',
			mainClass: 'mfp-img-mobile',
			gallery: {
				enabled: true,
				navigateByImgClick: true,
				preload: [0,1] // Will preload 0 - before current, and 1 after the current image
			},
			image: {
				tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
				titleSrc: function(item) {
					return item.el.attr('title') + '<small></small>';
				}
			}
		});
	
	// sub menu js
	$('.sub-menu').parent().hover(function() {
		var menu = $(this).find("ul");
		var menupos = $(menu).offset();

		if (menupos.left + menu.width() > $(window).width()) {
			var newpos = -$(menu).width();
			menu.css({ left: newpos });    
		}
	});
	
	//revel slider index
	var revapi1061 = jQuery("#rev_slider_1061_1").show().revolution({
		sliderType:"standard",
		jsFileLocation:"//server.local/revslider/wp-content/plugins/revslider/public/assets/js/",
		sliderLayout:"fullscreen",
		dottedOverlay:"none",
		delay:5000,
		navigation: {
			keyboardNavigation:"off",
			keyboard_direction: "horizontal",
			mouseScrollNavigation:"off",
			mouseScrollReverse:"default",
			onHoverStop:"off",
			touch:{
				touchenabled:"on",
				swipe_threshold: 75,
				swipe_min_touches: 50,
				swipe_direction: "horizontal",
				drag_block_vertical: false
			}
			,
			tabs: {
				style:"metis",
				enable:true,
				width:250,
				height:40,
				min_width:249,
				wrapper_padding:0,
				wrapper_color:"",
				wrapper_opacity:"0",
				tmp:'<div class="tp-tab-wrapper"><div class="tp-tab-number">{{param1}}</div><div class="tp-tab-divider"></div><div class="tp-tab-title-mask"><div class="tp-tab-title">{{title}}</div></div></div>',
				visibleAmount: 5,
				hide_onmobile: true,
				hide_under:800,
				hide_onleave:false,
				hide_delay:200,
				direction:"vertical",
				span:true,
				position:"inner",
				space:0,
				h_align:"left",
				v_align:"center",
				h_offset:0,
				v_offset:0
			}
		},
		responsiveLevels:[1240,1024,778,480],
		visibilityLevels:[1240,1024,778,480],
		gridwidth:[1240,1024,778,480],
		gridheight:[868,768,960,720],
		lazyType:"none",
		parallax: {
			type:"3D",
			origo:"slidercenter",
			speed:1000,
			levels:[2,4,6,8,10,12,14,16,45,50,47,48,49,50,0,50],
			type:"3D",
			ddd_shadow:"off",
			ddd_bgfreeze:"on",
			ddd_overflow:"hidden",
			ddd_layer_overflow:"visible",
			ddd_z_correction:100,
		},
		spinner:"off",
		stopLoop:"on",
		stopAfterLoops:0,
		stopAtSlide:1,
		shuffle:"off",
		autoHeight:"off",
		fullScreenAutoWidth:"off",
		fullScreenAlignForce:"off",
		fullScreenOffsetContainer: "",
		fullScreenOffset: "60px",
		disableProgressBar:"on",
		hideThumbsOnMobile:"off",
		hideSliderAtLimit:0,
		hideCaptionAtLimit:0,
		hideAllCaptionAtLilmit:0,
		debugMode:false,
		fallbacks: {
			simplifyAll:"off",
			nextSlideOnWindowFocus:"off",
			disableFocusListener:false,
		}
	});
	
	//revel slider index 2
	var revapi = jQuery("#rev_slider").revolution({
			sliderType:"standard",
			sliderLayout:"fullwidth",
			delay:5000,
			navigation: {
				arrows:{enable:true}				
			},			
			gridwidth:1230,
			gridheight:750		
		});
		
	// for counter 
		$('.timer').appear(function() {
			$(this).countTo();
		});
	
	// video section
		jQuery(function($){
			$('#pro_video').css("display", "none");
			$('.ed_video_section .ed_img_overlay a i').on("click", function(e) {
				e.preventDefault();
				$('.ed_video_section .ed_video').hide();	
				$('#pro_video').css("display", "block");
				$('#pro_video').attr('src',$('#pro_video').attr('src')+'?rel=0&autoplay=1');
			});
		});
		
	// On focus Placeholder css
	var place = '';
		$('input,textarea').focus(function(){
			place = $(this).attr('placeholder');
		$(this).attr('placeholder','');
		}).blur(function(){
		$(this).attr('placeholder',place);
		});
	
	// Menu js for Position fixed
		$(window).scroll(function(){
			var window_top = $(window).scrollTop() + 1; 
				if (window_top > 500) {
					$('.ed_header_bottom').addClass('menu_fixed animated fadeInDown');
				} else {
					$('.ed_header_bottom').removeClass('menu_fixed animated fadeInDown');
				}
		});
	
	//show hide login form js
		$('#login_button').on("click", function(e) {
			$('#login_one').slideToggle(1000);
			e.stopPropagation(); 
		});
	
	$(document).click(function(e){
		if(!(e.target.closest('#login_one'))){	
			$("#login_one").slideUp("slow");   		
		}
   });
	
	//show hide share button
		$('#ed_share_wrapper').on("click", function() {
			$('#ed_social_share').slideToggle(1000);
		});

	//section one slider
		$(".section_one_slider .owl-carousel, .ed_populer_areas_slider .owl-carousel").owlCarousel({
		items:4,
		dots: false,
		nav: true,
		animateIn: 'fadeIn',
		animateOut: 'fadeOut',
		autoHeight: true,
		touchDrag: false,
		mouseDrag: false,
		margin:30,
		loop: true,
		autoplay:false,
		navText:['<i class="fa fa-angle-left"></i>','<i class="fa fa-angle-right"></i>'],
		responsiveClass:true,
		responsive:{
			0:{
				items:1,
				nav:true
			},
			600:{
				items:2,
				nav:true
			},
			992:{
				items:3,
				nav:true
			},
			1200:{
				items:4,
				nav:true
			}
		}
		});	
		
	//section four slider
		$(".section_four_slider .owl-carousel, .ed_mostrecomeded_course_slider .owl-carousel").owlCarousel({
		items:4,
		dots: false,
		nav: true,
		animateIn: 'fadeIn',
		animateOut: 'fadeOut',
		autoHeight: true,
		touchDrag: false,
		mouseDrag: false,
		margin:30,
		loop: true,
		autoplay:false,
		navText:['<i class="fa fa-angle-left"></i>','<i class="fa fa-angle-right"></i>'],
		responsiveClass:true,
		responsive:{
			0:{
				items:1,
				nav:true
			},
			600:{
				items:2,
				nav:true
			},
			992:{
				items:3,
				nav:true
			},
			1200:{
				items:4,
				nav:true
			}
		}
		});	
		
	//section five slider
		$(".section_five_slider .owl-carousel, .ed_latest_news_slider .owl-carousel").owlCarousel({
		items:3,
		dots: false,
		nav: true,
		animateIn: 'fadeIn',
		animateOut: 'fadeOut',
		autoHeight: true,
		touchDrag: false,
		mouseDrag: false,
		margin:30,
		loop: true,
		autoplay:false,
		navText:['<i class="fa fa-angle-left"></i>','<i class="fa fa-angle-right"></i>'],
		responsiveClass:true,
		responsive:{
			0:{
				items:1,
				nav:true
			},
			600:{
				items:2,
				nav:true
			},
			992:{
				items:2,
				nav:true
			}
		}
		});	
	
	//client slider
		$(".ed_clientslider .owl-carousel").owlCarousel({
		items:5,
		dots: false,
		nav: false,
		animateIn: 'fadeIn',
		animateOut: 'fadeOut',
		autoHeight: true,
		touchDrag: false,
		mouseDrag: false,
		margin:0,
		loop: true,
		autoplay:true,
		responsiveClass:true,
		responsive:{
			0:{
				items:1
			},
			600:{
				items:3
			},
			1000:{
				items:5
			}
		}
		});	
		
		
	// Contact Form Submition
		$("#ed_submit").on("click", function() {
        var e = $("#uname").val();
        var t = $("#umail").val();
        var s = $("#sub").val();
        var r = $("#msg").val();
        $.ajax({
            type: "POST",
            url: "ajaxmail.php",
            data: {
                username: e,
                useremail: t,
                useresubject: s,
                mesg: r
            },
            success: function(n) {
                var i = n.split("#");
                if (i[0] == "1") {
                    $("#uname").val("");
                    $("#umail").val("");
                    $("#sub").val("");
                    $("#msg").val("");
                    $("#err").html(i[1]);
                } else {
                    $("#uname").val(e);
                    $("#umail").val(t);
                    $("#sub").val(s);
                    $("#msg").val(r);
                    $("#err").html(i[1]);
                }
            }
        });
		});
	
	// SmoothScroll js
		smoothScroll.init({
			speed: 1000,
			easing: 'easeInOutCubic',
			offset: 0,
			updateURL: true,
			callback: function ( toggle, anchor ) {}
		});
		
	// Video Play js
		function play_utube_video()
		{
			$('#utube_video_ply').attr('src',$('#utube_video_ply').attr('src')+'?rel=0&autoplay=1');
		}
	
	// Calender js
	/* "YYYY-MM[-DD]" => Date */
	function strToDate(str) {
		try {
			var array = str.split('-');
			var year = parseInt(array[0], 10);
			var month = parseInt(array[1], 10);
			var day = array.length > 2? parseInt(array[2], 10): 1 ;
			if (year > 0 && month >= 0) {
				return new Date(year, month - 1, day);
			} else {
				return null;
			}
		} catch (err) {return null; } // just throw any illegal format
	}

	/* Date => "YYYY-MM-DD" */
	function dateToStr(d) {
		/* fix month zero base */
		var year = d.getFullYear();
		var month = d.getMonth();
		return year + "-" + (month + 1) + "-" + d.getDate();
	}

	$.fn.calendar = function (options) {
		var _this = this;
		var opts = $.extend({}, $.fn.calendar.defaults, options);
		var week = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa','Su'];
		var tHead = week.map(function (day) {
			return "<th>" + day + "</th>";
		}).join("");

		_this.init = function () {
			var tpl = '<table class="cal">' +
			'<caption>' +
			'	<span class="prev"><a href="javascript:void(0);">&larr;</a></span>' +
			'	<span class="next"><a href="javascript:void(0);">&rarr;</a></span>' +
			'	<span class="month"><span>' +
			"</caption>" +
			"<thead><tr>" +
			tHead +
			"</tr></thead>" +
			"<tbody>" +
			"</tbody>" + "</table>";
			var html = $(tpl);
			_this.append(html);
		};

		function daysInMonth(d) {
			var newDate = new Date(d);
			newDate.setMonth(newDate.getMonth() + 1);
			newDate.setDate(0);
			return newDate.getDate();
		}

		_this.update = function (date) {
			var mDate = new Date(date);
			mDate.setDate(1); /* start of the month */
			var day = mDate.getDay(); /* value 0~6: 0 -- Sunday, 6 -- Saturday */
			mDate.setDate(mDate.getDate() - day); /* now mDate is the start day of the table */

			function dateToTag(d) {
				var tag = $('<td><a href="javascript:void(0);"></a></td>');
				var a = tag.find('a');
				a.text(d.getDate());
				a.data('date', dateToStr(d));
				if (date.getMonth() != d.getMonth()) { // the bounday month
					tag.addClass('off');
				} else if (_this.data('date') == a.data('date')) { // the select day
					tag.addClass('active');
					_this.data('date', dateToStr(d));
				}
				return tag;
			}

			var tBody = _this.find('tbody');
			tBody.empty(); /* clear previous first */
			var cols = Math.ceil((day + daysInMonth(date))/7);
			for (var i = 0; i < cols; i++) {
				var tr = $('<tr></tr>');
				for (var j = 0; j < 7; j++, mDate.setDate(mDate.getDate() + 1)) {
					tr.append(dateToTag(mDate));
				}
				tBody.append(tr);
			}

			/* set month head */
			var monthStr = dateToStr(date).replace(/-\d+$/, '');
			_this.find('.month').text(monthStr);
		};

		_this.getCurrentDate = function () {
			return _this.data('date');
		};

		_this.init();
		/* in date picker mode, and input date is empty,
		 * should not update 'data-date' field (no selected).
		 */
		var initDate = opts.date? opts.date: new Date();
		if (opts.date || !opts.picker) {
			_this.data('date', dateToStr(initDate));
		}
		_this.update(initDate);

		/* event binding */
		_this.delegate('tbody td', 'click', function () {
			var $this = $(this);
			_this.find('.active').removeClass('active');
			$this.addClass('active');
			_this.data('date', $this.find('a').data('date'));
			/* if the 'off' tag become selected, switch to that month */
			if ($this.hasClass('off')) {
				_this.update(strToDate(_this.data('date')));
			}
			if (opts.picker) {  /* in picker mode, when date selected, panel hide */
				_this.hide();
			}
		});

		function updateTable(monthOffset) {
			var date = strToDate(_this.find('.month').text());
			date.setMonth(date.getMonth() + monthOffset);
			_this.update(date);
		}

		_this.find('.next').on("click", function () {
			updateTable(1);

		});

		_this.find('.prev').on("click", function () {
			updateTable(-1);
		});

		return this;
	};

	$.fn.calendar.defaults = {
		date: new Date(),
		picker: false
	};

	$.fn.datePicker = function () {
		var _this = this;
		var picker = $('<div></div>').
			addClass('picker-container').
			hide().
			calendar({'date': strToDate(_this.val()), 'picker': true});

		_this.after(picker);

		/* event binding */
		// click outside area, make calendar disappear
		$('body').on("click", function() {
			picker.hide();
		});

		// click input should make calendar appear
		_this.on("click", function () {
			picker.show();
			return false; // stop sending event to docment
		});

		// click on calender, update input
		picker.on("click", function () {
			_this.val(picker.getCurrentDate());
			return false;
		});

		return this;
	};

	$(window).load(function () {
		$('.jquery-calendar').each(function () {
			$(this).calendar();
		});
	});
	
	});
})(); 