import { AfterViewInit, Directive, ElementRef } from '@angular/core';
import * as Tether from 'tether';

@Directive({
    selector: '[normalizePosition]'
})
export class NormalizeDropdownPositionDirective implements AfterViewInit {

    constructor(
        private _element: ElementRef
    ) {
    }

    ngAfterViewInit(): void {
        $(this._element.nativeElement).on('show.bs.dropdown', function (e) {
            const $this = $(this);
            if (!$this.data('_tether')) {
                const $dropdownButton = $this.find('.dropdown-toggle');
                const $dropdownMenu = $this.find('.dropdown-menu');

                $dropdownMenu.css({
                    'display': 'block',
                    'z-index':'99999'
                });

                $this.data('_tether', new Tether({
                    element: $dropdownMenu[0],
                    target: $dropdownButton[0],
                    attachment: 'top left',
                    targetAttachment: 'bottom left',
                    constraints: [{
                        to: 'window',
                        attachment: 'together',
                        pin: true
                    }]
                }));
            }

            let $dropdownMenu = $($this.data('_tether').element);
            $dropdownMenu.css({
                'display': 'block'
            });
        }).on('hidden.bs.dropdown', function (e) {
            let $this = $(this);
            let $dropdownMenu = $($this.data('_tether').element);
            $dropdownMenu.css({
                'display': 'none'
            });
        });
    }
}
