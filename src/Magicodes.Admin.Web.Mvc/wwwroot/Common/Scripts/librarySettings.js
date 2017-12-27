(function () {

    //Set Moment Timezone
    if (abp.clock.provider.supportsMultipleTimezone && window.moment) {
        moment.tz.setDefault(abp.timing.timeZoneInfo.iana.timeZoneId);
    }

    //Localize Sweet Alert
    if (abp.libs.sweetAlert) {
        abp.libs.sweetAlert.config.info.buttons = [app.localize("Ok")];
        abp.libs.sweetAlert.config.success.buttons = [app.localize("Ok")];
        abp.libs.sweetAlert.config.warn.buttons = [app.localize("Ok")];
        abp.libs.sweetAlert.config.error.buttons = [app.localize("Ok")];

        abp.libs.sweetAlert.config.confirm.buttons = [app.localize("Cancel"), app.localize("Yes")];
    }

})();