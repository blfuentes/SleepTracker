import { Sport } from "./sport";
import { BindingInfo } from "../../common/binding-info";
import { BindingElement } from "../../common/binding-element";

class SportBinding implements BindingInfo {
    
    targetBindings: Map<string, BindingElement>;
    sourceBindings: Map<string, BindingElement>;
    sport: Sport;

    LoadBindings(): void {
        this.targetBindings.forEach((binding, key) => {
            const propertyName = key as keyof Sport;
            binding.SetValue(String(this.sport[propertyName]));
        });
    }
    RefreshBindings(): void {
        this.sourceBindings.forEach((binding, key) => {
            const propertyName = key as keyof Sport;
            binding.RefreshValue(String(this.sport[propertyName]));
        });
    }
}

export { SportBinding };