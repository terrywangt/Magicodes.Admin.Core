export class HtmlHelper {
    static encodeText(value: string): string {
        if (!$) {
            return null;
        }

        return $('<div/>').text(value).html();
    }

    static decodeText(value: string): string {
        if (!$) {
            return null;
        }

        return $('<div/>').html(value).text();
    }

    static encodeJson(jsonObject: object): string {
        if (!$) {
            return null;
        }

        return JSON.parse(this.encodeText(JSON.stringify(jsonObject)));
    }

    static decodeJson(jsonObject: object): string {
        if (!$) {
            return null;
        }

        return JSON.parse(this.decodeText(JSON.stringify(jsonObject)));
    }
}
