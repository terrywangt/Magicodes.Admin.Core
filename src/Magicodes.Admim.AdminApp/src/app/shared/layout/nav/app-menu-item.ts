export class AppMenuItem {
    name = '';
    permissionName = '';
    icon = '';
    route = '';
    items: AppMenuItem[];

    constructor(name: string, permissionName: string, icon: string, route: string, items?: AppMenuItem[]) {
        this.name = name;
        this.permissionName = permissionName;
        this.icon = icon;
        this.route = route;

        if (items === undefined) {
            this.items = [];
        } else {
            this.items = items;
        }
    }
}
