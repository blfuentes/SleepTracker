const sportTemplate = require("../../../assets/templates/sports.html");

class SportService {

  static generateSportContent(sports: Sport[]) {
    const apiResponseContainer = document.getElementById("content");
    const template = sportTemplate.default;
    apiResponseContainer!.innerHTML = template;
    const sportContainer = document.getElementsByClassName("sportContainer")[0];
    
    sports.forEach((sport: Sport) => {
      // Create a div element for each sport
      const sportElement = document.createElement("div");
      sportElement.className = "sportCard";
      const sportName = document.createElement("h3");
      sportName.textContent = sport.sportName;
      sportElement.appendChild(sportName);
      const sportElementNotes = document.createElement("p");
      sportElementNotes.textContent = sport.sportNotes;
      sportElement.appendChild(sportElementNotes);

      // Add the sportElement to the sportContainer
      sportContainer!.appendChild(sportElement);
    });

    const formAddSport = document.getElementsByClassName("addSportForm")[0] as HTMLFormElement;
    formAddSport!.addEventListener("submit", SaveSport);
  }
}

async function SaveSport(event: Event): Promise<void> {
  try {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const formData = new FormData(form);

    const urlEncodedData = new URLSearchParams(formData as any).toString();

    const response = await fetch(process.env.API_URL + "/sport", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded"
      },
      body: urlEncodedData
    });
    const json = await response.json();
    console.log(json);
  } catch (error) {
    console.error(error);
  }
}


export default SportService;