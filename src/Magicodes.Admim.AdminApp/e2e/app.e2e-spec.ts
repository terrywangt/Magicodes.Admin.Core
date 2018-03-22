import { AdminPage } from './app.po';

describe('abp-zero-template App', function () {
    let page: AdminPage;

    beforeEach(() => {
        page = new AdminPage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        page.getCopyright().then(value => {
            expect(value).toEqual(new Date().getFullYear() + ' © Admin.');
        });
    });
});
