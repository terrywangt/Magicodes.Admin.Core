import { Component, Injector } from '@angular/core';
import { FeatureTreeEditModel } from '@app/admin/shared/feature-tree-edit.model';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FlatFeatureDto, NameValueDto } from '@shared/service-proxies/service-proxies';
import { ArrayToTreeConverterService } from '@shared/utils/array-to-tree-converter.service';
import { TreeDataHelperService } from '@shared/utils/tree-data-helper.service';
import { TreeNode } from 'primeng/api';
import * as _ from 'lodash';

@Component({
    selector: 'feature-tree',
    templateUrl: './feature-tree.component.html',
    styleUrls: ['./feature-tree.component.less']
})
export class FeatureTreeComponent extends AppComponentBase {

    _editData: FeatureTreeEditModel;

    set editData(val: FeatureTreeEditModel) {
        this._editData = val;
        this.setTreeData(val.features);
        this.setSelectedNodes(val);
    }

    treeData: any;
    selectedFeatures: TreeNode[] = [];

    constructor(
        private _arrayToTreeConverterService: ArrayToTreeConverterService,
        private _treeDataHelperService: TreeDataHelperService,
        injector: Injector
    ) {
        super(injector);
    }

    setTreeData(permissions: FlatFeatureDto[]) {
        this.treeData = this._arrayToTreeConverterService.createTree(permissions, 'parentName', 'name', null, 'children',
            [{
                target: 'label',
                source: 'displayName'
            }, {
                target: 'expandedIcon',
                value: 'fa fa-folder-open m--font-warning'
            },
            {
                target: 'collapsedIcon',
                value: 'fa fa-folder m--font-warning'
            },
            {
                target: 'expanded',
                value: true
            },
            {
                target: 'selectable',
                targetFunction(item) {
                    return item.inputType.name === 'CHECKBOX';
                }
            }]);
    }

    setSelectedNodes(val: FeatureTreeEditModel) {
        _.forEach(val.features, feature => {
            let items = _.filter(val.featureValues, { name: feature.name });
            if (items && items.length === 1) {
                let item = items[0];
                this.setSelectedNode(item.name, item.value);
            } else {
                this.setSelectedNode(feature.name, feature.defaultValue);
            }
        });
    }

    setSelectedNode(featureName, value) {
        let node;

        if (value === 'true') {
            node = this._treeDataHelperService.findNode(this.treeData, { data: { name: featureName } });
            this.selectedFeatures.push(node);
        } else if (value && value !== 'false') {
            node = this._treeDataHelperService.findNode(this.treeData, { data: { name: featureName } });
            node.value = value;
            this.selectedFeatures.push(node);
        }
    }

    getGrantedFeatures(): NameValueDto[] {
        if (!this._editData.features) {
            return [];
        }

        let features: NameValueDto[] = [];

        for (let i = 0; i < this._editData.features.length; i++) {
            let feature = new NameValueDto();
            feature.name = this._editData.features[i].name;
            feature.value = this.getFeatureValueByName(feature.name);

            features.push(feature);
        }

        return features;
    }

    onDropdownChange(node) {
        if (node.value) {
            node.selected = true;
        }
    }

    onInputChange(node) {
        if (node.value) {
            node.selected = true;
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

        let result = true;

        _.forEach(this._editData.features, feature => {
            let value = this.getFeatureValueByName(feature.name);
            if (!this.isFeatureValueValid(feature.name, value)) {
                result = false;
            }
        });

        return result;
    }

    setFeatureValueByName(featureName: string, value: string): void {
        const featureValue = _.find(this._editData.featureValues, f => f.name === featureName);
        if (!featureValue) {
            return;
        }

        featureValue.value = value;
    }

    isFeatureSelected(name: string): boolean {
        let nodes = _.filter(this.selectedFeatures, { data: { name: name } });
        return nodes && nodes.length === 1;
    }

    getFeatureValueByName(featureName: string): string {
        let feature = this._treeDataHelperService.findNode(this.treeData, { data: { name: featureName } });
        if (!feature) {
            return null;
        }

        if (feature.value) {
            return feature.value;
        }

        if (!this.isFeatureSelected(featureName)) {
            return 'false';
        }

        return 'true';
    }

    isFeatureEnabled(featureName: string): boolean {
        const self = this;
        const value = self.findFeatureValueByName(featureName);
        return value.toLowerCase() === 'true';
    }

    nodeSelect(event) {
        let parentNode = this._treeDataHelperService.findParent(this.treeData, { data: { name: event.node.data.name } });

        while (parentNode != null) {
            this.selectedFeatures.push(parentNode);
            parentNode = this._treeDataHelperService.findParent(this.treeData, { data: { name: parentNode.data.name } });
        }
    }
}
