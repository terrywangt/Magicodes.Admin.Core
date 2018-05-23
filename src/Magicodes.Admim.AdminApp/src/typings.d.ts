///<reference path="../node_modules/@types/jquery/index.d.ts"/>
///<reference path="../node_modules/@types/jstree/index.d.ts"/>
///<reference path="../node_modules/abp-web-resources/Abp/Framework/scripts/abp.d.ts"/>
///<reference path="../node_modules/abp-web-resources/Abp/Framework/scripts/libs/abp.jquery.d.ts"/>
///<reference path="../node_modules/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr.d.ts"/>
///<reference path="../node_modules/moment/moment.d.ts"/>
///<reference path="../node_modules/@types/moment-timezone/index.d.ts"/>
///<reference path="../node_modules/@types/bootstrap/index.d.ts"/>
///<reference path="../node_modules/@types/toastr/index.d.ts"/>
///<reference path="../node_modules/@types/tether/index.d.ts"/>

// Typings reference file, see links for more information
// https://github.com/typings/typings
// https://www.typescriptlang.org/docs/handbook/writing-declaration-files.html

declare var System: any;

declare var mApp: any; // Related to Metronic
declare var mLayout: any; // Related to Metronic
declare var mUtil: any; // Related to Metronic

interface JQuery {
    mOffcanvas(...any): any;
}

interface JQuery {
    Jcrop(...any): any;
}

interface JQuery {
    daterangepicker(...any): any;
}

interface JQuery {
    datepicker(...any): any;
}

interface JQuery {
    datetimepicker(...any): any;
}

interface JQuery {
    slimScroll(...any): any;
}

interface JQuery {
    timeago(...any): any;
}

/**
 * jQuery selectpicker
 */

interface JQuery {
    selectpicker(...any): any;
}

/**
 * jQuery sparkline
 */

interface JQuery {
    sparkline(...any): any;
}

/**
 * jQuery Bootstrap Switch
 */

interface JQuery {
    bootstrapSwitch(...any): any;
}

/**
 * Morris
 */

declare namespace morris {
    interface IAreaOptions {
        gridEnabled?: boolean;
        // gridLineColor?: string;
        padding?: number;
    }
}

/**
 * Chartjs
 */

declare var Chart: any;

/**
 * Chartist
 */

declare namespace Chartist {
    interface ChartistStatic {
        Pie: any;
        Svg: any;
    }
}

declare var Chartist: Chartist.ChartistStatic;


/**
 * rtl-detect
 */

declare module 'rtl-detect';

/**
 * horizontal-timeline
 */
interface JQuery {
    horizontalTimeline(...any): any;
}

interface JQuery {
    inputmask(...any): any;
}


