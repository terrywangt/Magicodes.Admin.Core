export class AppMenuItem {
    name = '';
    permissionName = '';
    icon = '';
    route = '';
    items: AppMenuItem[];
    external: boolean;
    parameters: {};

    constructor(name: string, permissionName: string, icon: string, route: string, items?: AppMenuItem[], external?: boolean, parameters?: Object) {
        this.name = name;
        this.permissionName = permissionName;
        this.icon = icon;
        this.route = route;
        this.external = external;
        this.parameters = parameters;

        if (items === undefined) {
            this.items = [];
        } else {
            this.items = items;
        }
    }
}
