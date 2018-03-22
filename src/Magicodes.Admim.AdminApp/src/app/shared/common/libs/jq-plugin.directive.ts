import { Directive, ElementRef, AfterViewInit } from '@angular/core';

@Directive({
    selector: '[jq-plugin]'
})
export class JqPluginDirective implements AfterViewInit {

    constructor(
        private _element: ElementRef
    ) {
    }

    ngAfterViewInit(): void {
        const $element = $(this._element.nativeElement);
        const pluginName = $element.attr('jq-plugin');
        const options = $element.attr('jq-options');
        if (!options) {
            $element[pluginName]();
        } else {
            $element[pluginName](eval('(' + options + ')'));
        }
    }
}
