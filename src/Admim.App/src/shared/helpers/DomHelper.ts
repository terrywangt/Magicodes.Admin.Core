export class DomHelper {

    static waitUntilElementIsReady(selector: string, callback: any, checkPeriod?: number): void {
        if (!$) {
            return;
        }

        let elementCount = selector.split(',').length;

        if (!checkPeriod) {
            checkPeriod = 100;
        }

        let checkExist = setInterval(() => {
            if ($(selector).length >= elementCount) {
                clearInterval(checkExist);
                callback();
            }
        }, checkPeriod);
    }

}
