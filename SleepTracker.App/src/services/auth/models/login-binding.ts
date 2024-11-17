import { Login } from "./login";
import { BindingInfo } from "../../common/binding-info";
import { BindingElement } from "../../common/binding-element";

class LoginBinding implements BindingInfo {
    
    targetBindings: Map<string, BindingElement>;
    sourceBindings: Map<string, BindingElement>;
    login: Login;

    LoadBindings(): void {
        this.targetBindings.forEach((binding, key) => {
            const propertyName = key as keyof Login;
            binding.SetValue(String(this.login[propertyName]));
        });
    }
    RefreshBindings(): void {
        this.sourceBindings.forEach((binding, key) => {
            const propertyName = key as keyof Login;
            binding.RefreshValue(String(this.login[propertyName]));
        });
    }
}

export { LoginBinding };