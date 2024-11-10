import { Sport } from "./models/sport";
import { SportBinding } from "./models/sport-binding";
import { BindingElement } from "../common/binding-element";

const sportTemplate = require("../../../assets/templates/sports.html");

const SportId = "sportId";
const SportName = "sportName";
const SportNotes = "sportNotes";

class SportService {
  
  sportsCollection: SportBinding[] = [];

  static generateSportContent(instance: SportService, sports: Sport[]) {
    const apiResponseContainer = document.getElementById("content");
    const template = sportTemplate.default;
    apiResponseContainer!.innerHTML = template;
    const sportContainer = document.getElementsByClassName("sportContainer")[0];

    sports.forEach((sport: Sport) => {
      buildSport(instance.sportsCollection, sportContainer, sport);
    });

    const formAddSport = document.getElementsByClassName(
      "addSportForm",
    )[0] as HTMLFormElement;
    formAddSport!.addEventListener("submit", (event) => SaveSport(event, instance.sportsCollection));

    const formSportName = document.getElementById(SportName) as HTMLInputElement;
    formSportName!.addEventListener("input", () => {
      const errorContainer = document.getElementById("validationMessage");
      errorContainer!.style.visibility = "hidden";
    });
  };
}

function buildSport(sportsCollection: SportBinding[], sportContainer: Element, sport: Sport) {
  // Create a div element for each sport
  const sportElement = document.createElement("div");
  sportElement.className = "sportCard";
  const sportName = document.createElement("h3");
  sportName.textContent = sport.sportName;
  sportElement.appendChild(sportName);
  const sportElementNotes = document.createElement("p");
  sportElementNotes.textContent = sport.sportNotes;
  sportElement.appendChild(sportElementNotes);

  const tBindings = new Map<string, BindingElement>();
  tBindings.set(SportId, new BindingElement(document.getElementById(SportId) as HTMLElement));
  tBindings.set(SportName, new BindingElement(document.getElementById(SportName) as HTMLElement));
  tBindings.set(SportNotes, new BindingElement(document.getElementById(SportNotes) as HTMLElement));

  const sBindings = new Map<string, BindingElement>();
  sBindings.set(SportName, new BindingElement(sportName));
  sBindings.set(SportNotes, new BindingElement(sportElementNotes));

  const sportBinding = new SportBinding();
  sportBinding.sport = sport;
  sportBinding.targetBindings = tBindings;
  sportBinding.sourceBindings = sBindings;

  sportsCollection.push(sportBinding);

  sportElement.addEventListener("click", () => {
    loadSport(sportsCollection, sport.sportId);
  });

  // Add the sportElement to the sportContainer
  sportContainer!.appendChild(sportElement);
}

function loadSport(sportsCollection: SportBinding[], sportId: number) {
  let sportBinding = sportsCollection.find((x) => x.sport.sportId === sportId);
  if (sportBinding) {
    sportBinding.LoadBindings();
  }
}

function refreshSport(sportsCollection: SportBinding[], sportId: number) {
  let sportBinding = sportsCollection.find((x) => x.sport.sportId === sportId);
  if (sportBinding) {
    sportBinding.RefreshBindings();
  }
}

function displayError(message: string) {
  const errorContainer = document.getElementById("validationMessage");
  errorContainer!.textContent = message;
  errorContainer!.style.visibility = "visible";
}

async function SaveSport(event: Event, sportsCollection: SportBinding[]): Promise<void> {
  try {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const formData = new FormData(form);

    const urlEncodedData = new URLSearchParams(formData as any).toString();

    const isUpdate = formData.get("IsNew") !== 'on';

    const url = isUpdate ? process.env.API_URL + "/api/sports/" + formData.get("SportId") : process.env.API_URL + "/api/sports";
    const response = await fetch(url, {
      method: isUpdate ? "PUT" : "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: urlEncodedData,
    });

    if (response.ok) {
      console.log("Sport saved successfully");
      const sportContainer =
        document.getElementsByClassName("sportContainer")[0];
      const sport = await response.json();
      if (isUpdate) {
        // replace the sport in the collection
        sportsCollection.forEach((element) => {
          if (element.sport.sportId === sport.sportId) {
            element.sport = sport;
          }
        });
        refreshSport(sportsCollection, sport.sportId);
      } else {
        buildSport(sportsCollection, sportContainer, sport);
        loadSport(sportsCollection, sport.sportId);
        (document.getElementById("isNew") as HTMLInputElement).checked = false;
        
      }
    } else {
      console.error("Failed to save sport");
      switch (response.status) {
        case 409:
          displayError("Sport already exists");
          break;
        default:
          console.error("Unknown error");
          break;
      }
    }
  } catch (error) {
    console.error(error);
  }
} 

export default SportService;
