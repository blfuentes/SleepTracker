import { Sport } from "./sport";
import { BindingInfo } from "../../common/binding-info";

class SportBinding implements BindingInfo {
    targetBindings: Map<string, HTMLElement>;
    sourceBindings: Map<string, HTMLElement>;
    sport: Sport;
}

export { SportBinding };