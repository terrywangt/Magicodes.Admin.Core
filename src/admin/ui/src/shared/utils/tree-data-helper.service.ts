import { Injectable } from '@angular/core';
import * as _ from 'lodash';

@Injectable()
export class TreeDataHelperService {

    findNode(data, selector): any {
        let nodes = _.filter(data, selector);
        if (nodes && nodes.length === 1) {
            return nodes[0];
        }

        let foundNode = null;

        _.forEach(data, d => {
            if (!foundNode) {
                foundNode = this.findNode(d.children, selector);
            }
        });

        return foundNode;
    }
}
