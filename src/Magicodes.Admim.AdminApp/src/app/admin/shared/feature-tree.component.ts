import { AfterViewChecked, AfterViewInit, Component, ElementRef, Injector, OnInit } from '@angular/core';
import { FeatureTreeEditModel } from '@app/admin/shared/feature-tree-edit.model';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FlatFeatureDto, NameValueDto } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';

@Component({
    selector: 'feature-tree',
    template: `<div class="feature-tree"></div>`,
    styleUrls: ['./feature-tree.component.less']
})
export class FeatureTreeComponent extends AppComponentBase implements OnInit, AfterViewInit, AfterViewChecked {

    set editData(val: FeatureTreeEditModel) {
        this._editData = val;
        this.refreshTree();
    }

    private _$tree: JQuery;
    private _editData: FeatureTreeEditModel;
    private _createdTreeBefore;

    constructor(private _element: ElementRef,
        injector: Injector
    ) {
        super(injector);
    }

    ngOnInit(): void {

    }

    ngAfterViewInit(): void {
        this._$tree = $(this._element.nativeElement);

        this.refreshTree();
    }

    ngAfterViewChecked(): void {

    }

    getGrantedFeatures(): NameValueDto[] {
        if (!this._$tree || !this._createdTreeBefore) {
            return [];
        }

        const selectedFeatures = this._$tree.jstree('get_selected', true);

        return _.map(this._editData.features, item => {
            const feature = new NameValueDto();

            feature.name = item.name;

            if (!item.inputType || item.inputType.name === 'CHECKBOX') {
                feature.value = _.some(selectedFeatures, { original: { id: item.name } }) ? 'true' : 'false';
            } else {
                feature.value = this.getFeatureValueByName(item.name);
            }

            return feature;
        });
    }

    refreshTree(): void {
        const self = this;

        if (this._createdTreeBefore) {
            this._$tree.jstree('destroy');
        }

        this._createdTreeBefore = false;

        if (!this._editData || !this._$tree) {
            return;
        }

        const treeData = _.map(this._editData.features, item => ({
            id: item.name,
            parent: item.parentName ? item.parentName : '#',
            text: item.displayName,
            state: {
                opened: true,
                selected: _.some(this._editData.featureValues, { name: item.name, value: 'true' })
            }
        }));

        this._$tree
            .on('ready.jstree', () => {
                this.customizeTreeNodes();
            })
            .on('redraw.jstree', () => {
                this.customizeTreeNodes();
            })
            .on('after_open.jstree', () => {
                this.customizeTreeNodes();
            })
            .on('create_node.jstree', () => {
                this.customizeTreeNodes();
            })
            .on('changed.jstree', (e, data) => {
                if (!data.node) {
                    return;
                }

                const wasInTreeChangeEvent = inTreeChangeEvent;
                if (!wasInTreeChangeEvent) {
                    inTreeChangeEvent = true;
                }

                let childrenNodes;

                if (data.node.state.selected) {
                    selectNodeAndAllParents(this._$tree.jstree('get_parent', data.node));

                    childrenNodes = $.makeArray(this._$tree.jstree('get_node', data.node).children);
                    this._$tree.jstree('select_node', childrenNodes);

                } else {
                    childrenNodes = $.makeArray(this._$tree.jstree('get_node', data.node).children);
                    this._$tree.jstree('deselect_node', childrenNodes);
                }

                if (!wasInTreeChangeEvent) {
                    const $nodeLi = this.getNodeLiByFeatureName(data.node.id);
                    const feature = this.findFeatureByName(data.node.id);
                    if (feature && (!feature.inputType || feature.inputType.name === 'CHECKBOX')) {
                        const value = this._$tree.jstree('is_checked', $nodeLi) ? 'true' : 'false';
                        this.setFeatureValueByName(data.node.id, value);
                    }

                    inTreeChangeEvent = false;
                }
            })
            .jstree({
                'core': {
                    data: treeData
                },
                'types': {
                    'default': {
                        'icon': 'fa fa-folder m--font-warning'
                    },
                    'file': {
                        'icon': 'fa fa-file m--font-warning'
                    }
                },
                'checkbox': {
                    keep_selected_style: false,
                    three_state: false,
                    cascade: ''
                },
                plugins: ['checkbox', 'types']
            });

        this._createdTreeBefore = true;

        let inTreeChangeEvent = false;

        function selectNodeAndAllParents(node) {
            self._$tree.jstree('select_node', node, true);
            const parent = self._$tree.jstree('get_parent', node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        }

        this._$tree.on('changed.jstree', (e, data) => {
            if (!data.node) {
                return;
            }

            const wasInTreeChangeEvent = inTreeChangeEvent;
            if (!wasInTreeChangeEvent) {
                inTreeChangeEvent = true;
            }

            let childrenNodes;

            if (data.node.state.selected) {
                selectNodeAndAllParents(this._$tree.jstree('get_parent', data.node));

                childrenNodes = $.makeArray(this._$tree.jstree('get_node', data.node).children);
                this._$tree.jstree('select_node', childrenNodes);

            } else {
                childrenNodes = $.makeArray(this._$tree.jstree('get_node', data.node).children);
                this._$tree.jstree('deselect_node', childrenNodes);
            }

            if (!wasInTreeChangeEvent) {
                inTreeChangeEvent = false;
            }
        });
    }

    customizeTreeNodes(): void {
        const self = this;
        self._$tree.find('.jstree-node').each(function () {
            const $nodeLi = $(this);
            const $nodeA = $nodeLi.find('.jstree-anchor');

            const featureName = $nodeLi.attr('id');
            const feature = self.findFeatureByName(featureName);
            const featureValue = self.findFeatureValueByName(featureName) || '';

            if (!feature || !feature.inputType) {
                return;
            }

            if (feature.inputType.name === 'CHECKBOX') {
                //no change for checkbox
            } else if (feature.inputType.name === 'SINGLE_LINE_STRING') {
                if (!$nodeLi.find('.feature-tree-textbox').length) {
                    $nodeA.find('.jstree-checkbox').hide();

                    let inputType = 'text';
                    const validator = (feature.inputType.validator as any);
                    if (feature.inputType.validator) {
                        if (feature.inputType.validator.name === 'NUMERIC') {
                            inputType = 'number';
                        }
                    }

                    const $textbox = $('<input class="feature-tree-textbox" type="' + inputType + '" />')
                        .val(featureValue);

                    if (inputType === 'number') {
                        $textbox.attr('min', validator.minValue);
                        $textbox.attr('max', validator.maxValue);
                    } else {
                        if (feature.inputType.validator && feature.inputType.validator.name === 'STRING') {
                            if (validator.maxLength > 0) {
                                $textbox.attr('maxlength', validator.maxLength);
                            }
                            if (validator.minLength > 0) {
                                $textbox.attr('required', 'required');
                            }
                            if (validator.regularExpression) {
                                $textbox.attr('pattern', validator.regularExpression);
                            }
                        }
                    }

                    $textbox.on('input propertychange paste', () => {
                        const value = $textbox.val() as string;
                        if (self.isFeatureValueValid(featureName, value)) {
                            self.setFeatureValueByName(featureName, value);
                            $textbox.removeClass('feature-tree-textbox-invalid');
                        } else {
                            $textbox.addClass('feature-tree-textbox-invalid');
                        }
                    });

                    $textbox.appendTo($nodeLi);
                }
            } else if (feature.inputType.name === 'COMBOBOX') {
                if (!$nodeLi.find('.feature-tree-combobox').length) {
                    $nodeA.find('.jstree-checkbox').hide();

                    const $combobox = $('<select class="feature-tree-combobox" />');
                    const inputType = (feature.inputType as any);
                    _.each(inputType.itemSource.items, (opt: any) => {
                        $('<option></option>')
                            .attr('value', opt.value)
                            .text(opt.displayText)
                            .appendTo($combobox);
                    });

                    $combobox
                        .val(featureValue)
                        .on('change', () => {
                            const value = $combobox.val() as string;
                            self.setFeatureValueByName(featureName, value);
                        })
                        .appendTo($nodeLi);
                }
            }
        });
    }

    getNodeLiByFeatureName(featureName: string): JQuery {
        return $('#' + featureName.replace('.', '\\.'));
    }

    selectNodeAndAllParents(node: any): void {
        const self = this;
        self._$tree.jstree('select_node', node, true);
        const parent = self._$tree.jstree('get_parent', node);
        if (parent) {
            self.selectNodeAndAllParents(parent);
        }
    }

    findFeatureByName(featureName: string): FlatFeatureDto {
        const self = this;

        const feature = _.find(self._editData.features, f => f.name === featureName);

        if (!feature) {
            abp.log.warn('Could not find a feature by name: ' + featureName);
        }

        return feature;
    }

    findFeatureValueByName(featureName: string) {
        const self = this;
        const feature = self.findFeatureByName(featureName);
        if (!feature) {
            return '';
        }

        const featureValue = _.find(self._editData.featureValues, f => f.name === featureName);
        if (!featureValue) {
            return feature.defaultValue;
        }

        return featureValue.value;
    }

    isFeatureValueValid(featureName: string, value: string): boolean {
        const self = this;
        const feature = self.findFeatureByName(featureName);
        if (!feature || !feature.inputType || !feature.inputType.validator) {
            return true;
        }

        const validator = (feature.inputType.validator as any);
        if (validator.name === 'STRING') {
            if (value === undefined || value === null) {
                return validator.allowNull;
            }

            if (typeof value !== 'string') {
                return false;
            }

            if (validator.minLength > 0 && value.length < validator.minLength) {
                return false;
            }

            if (validator.maxLength > 0 && value.length > validator.maxLength) {
                return false;
            }

            if (validator.regularExpression) {
                return (new RegExp(validator.regularExpression)).test(value);
            }
        } else if (validator.name === 'NUMERIC') {
            const numValue = parseInt(value);

            if (isNaN(numValue)) {
                return false;
            }

            const minValue = validator.minValue;
            if (minValue > numValue) {
                return false;
            }

            const maxValue = validator.maxValue;
            if (maxValue > 0 && numValue > maxValue) {
                return false;
            }
        }

        return true;
    }

    areAllValuesValid(): boolean {
        const self = this;
        self._$tree.find('.jstree-node').each(function () {
            const $nodeLi = $(this);
            const featureName = $nodeLi.attr('id');
            const feature = self.findFeatureByName(featureName);

            if (feature && (!feature.inputType || feature.inputType.name === 'CHECKBOX')) {
                const value = self._$tree.jstree('is_checked', $nodeLi) ? 'true' : 'false';
                self.setFeatureValueByName(featureName, value);
            }
        });

        return self._$tree.find('.feature-tree-textbox-invalid').length <= 0;
    }

    setFeatureValueByName(featureName: string, value: string): void {
        const featureValue = _.find(this._editData.featureValues, f => f.name === featureName);
        if (!featureValue) {
            return;
        }

        featureValue.value = value;
    }

    getFeatureValueByName(featureName: string): string {
        const featureValue = _.find(this._editData.featureValues, f => f.name === featureName);
        if (!featureValue) {
            return null;
        }

        return featureValue.value;
    }

    isFeatureEnabled(featureName: string): boolean {
        const self = this;
        const value = self.findFeatureValueByName(featureName);
        return value.toLowerCase() === 'true';
    }
}
