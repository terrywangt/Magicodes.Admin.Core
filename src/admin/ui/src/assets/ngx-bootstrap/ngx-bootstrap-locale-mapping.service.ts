export class NgxBootstrapLocaleMappingService {
    map(locale: string): string {
        const cultureMap = {
            'zh-Hans': 'zh-cn'
            // Add more here
        };

        if (cultureMap[locale]) {
            return cultureMap[locale];
        }

        return locale;
    }
}
