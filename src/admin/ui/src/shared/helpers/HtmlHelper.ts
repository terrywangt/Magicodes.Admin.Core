export class HtmlHelper {

    static encodeText(value: string): string {
        let div = document.createElement('div');
        div[('textContent' in div) ? 'textContent' : 'innerText'] = value;
        return div.innerHTML;

    }

    static decodeText(value: string): string {
        let div = document.createElement('div') as any;
        div.innerHTML = value;
        return ('textContent' in div) ? div.textContent : div.innerText;
    }

    static encodeJson(jsonObject: object): string {
        return JSON.parse(this.encodeText(JSON.stringify(jsonObject)));
    }

    static decodeJson(jsonObject: object): string {
        return JSON.parse(this.decodeText(JSON.stringify(jsonObject)));
    }
}
