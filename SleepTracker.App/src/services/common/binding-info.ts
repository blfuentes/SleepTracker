import { BindingElement } from "./binding-element";

interface BindingInfo {
    targetBindings: Map<string, BindingElement>;
    sourceBindings: Map<string, BindingElement>;

    LoadBindings(): void;
    RefreshBindings(): void;
}

export { BindingInfo };