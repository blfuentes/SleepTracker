import { Sport } from "./sport";

class SportBinding {
    sport: Sport;
    targetBindings: Map<string, HTMLElement>;
    sourceBindings: Map<string, HTMLElement>;
}

export { SportBinding };