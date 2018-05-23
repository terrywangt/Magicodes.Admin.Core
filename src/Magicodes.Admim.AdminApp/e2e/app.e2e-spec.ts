import { AdminPage } from './app.po';
import { browser, by } from 'protractor';

describe('abp-zero-template App', () => {
    let page: AdminPage;

    beforeEach(() => {
        page = new AdminPage();

        browser.driver.manage().deleteAllCookies();

        // Disable waitForAngularEnabled, otherwise test browser will be closed immediately
        browser.waitForAngularEnabled(false);
    });

    it('should login as host admin', async () => {
        // To make username div visible. It is not visible in small size screens
        browser.driver.manage().window().setSize(1200, 1000);

        await page.loginAsHostAdmin();

        await page.waitForItemToBeVisible(by.css(".m-topbar__username"));
        
        page.getUsername().then(value => {
            expect(value.toLocaleLowerCase()).toEqual('\\admin');
        });

        page.getTenancyName().then(value => {
            expect(value).toEqual('\\');
        });
    });

    it('should login as default tenant admin', async () => {
        // To make username div visible. It is not visible in small size screens
        browser.driver.manage().window().setSize(1200, 1000);

        await page.loginAsTenantAdmin();
        
        await page.waitForItemToBeVisible(by.css(".m-topbar__username"));

        page.getUsername().then(value => {
            expect(value.toLocaleLowerCase()).toEqual('default\\admin');
        });

        page.getTenancyName().then(value => {
            expect(value.toLocaleLowerCase()).toEqual('default\\');
        });
    });
});
