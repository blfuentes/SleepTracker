class BindingElement {
    element: HTMLElement;
    constructor(element: HTMLElement){
        this.element = element;
    }

    SetValue(value: string) {
        if (this.element.tagName.toLowerCase() == "input") {
            (this.element as HTMLInputElement).value = value;
            return;
        } 
        if (this.element.tagName.toLowerCase() == "textarea") {
            (this.element as HTMLTextAreaElement).value = value;
            return;
        }  
    }

    RefreshValue(value: string) {
        if (this.element.tagName.toLowerCase() == "div") {
            (this.element as HTMLDivElement).textContent = value;
            return;
        } 
        if (this.element.tagName.toLowerCase() == "textarea") {
            (this.element as HTMLTextAreaElement).textContent = value;
            return;
        }
        (this.element as HTMLElement).textContent = value;
    }
}

export { BindingElement };