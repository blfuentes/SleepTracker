import { Sport } from "./models/sport";
import { SportBinding } from "./models/sport-binding";

const sportTemplate = require("../../../assets/templates/sports.html");

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

    const formSportName = document.getElementById("sportName") as HTMLInputElement;
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

  const tBindings = new Map<string, HTMLElement>();
  tBindings.set("sportID", document.getElementById("sportID") as HTMLInputElement);
  tBindings.set("sportName", document.getElementById("sportName") as HTMLInputElement);
  tBindings.set("sportNotes", document.getElementById("sportNotes") as HTMLTextAreaElement);

  const sBindings = new Map<string, HTMLElement>();
  sBindings.set("sportName", sportName);
  sBindings.set("sportNotes", sportElementNotes);

  sportsCollection.push({ sport: sport, targetBindings: tBindings, sourceBindings: sBindings });

  sportElement.addEventListener("click", () => {
    loadSport(sportsCollection, sport.sportID);
  });

  // Add the sportElement to the sportContainer
  sportContainer!.appendChild(sportElement);
}

function loadSport(sportsCollection: SportBinding[], sportId: number) {
  let sportBinding = sportsCollection.find((x) => x.sport.sportID === sportId);
  if (sportBinding) {
    (sportBinding.targetBindings.get("sportID") as any).value = sportBinding.sport.sportID.toString();
    (sportBinding.targetBindings.get("sportName") as any).value = sportBinding.sport.sportName;
    (sportBinding.targetBindings.get("sportNotes") as any).value = sportBinding.sport.sportNotes;
  }
}

function refreshSport(sportsCollection: SportBinding[], sportId: number) {
  let sportBinding = sportsCollection.find((x) => x.sport.sportID === sportId);
  if (sportBinding) {
    (sportBinding.sourceBindings.get("sportName") as any).textContent = sportBinding.sport.sportName;
    (sportBinding.sourceBindings.get("sportNotes") as any).textContent = sportBinding.sport.sportNotes;
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

    const url = isUpdate ? process.env.API_URL + "/sport/" + formData.get("SportID") : process.env.API_URL + "/sport";
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
          if (element.sport.sportID === sport.sportID) {
            element.sport = sport;
          }
        });
        refreshSport(sportsCollection, sport.sportID);
      } else {
        buildSport(sportsCollection, sportContainer, sport);
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
