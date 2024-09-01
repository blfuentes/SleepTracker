const sportTemplate = require("../../../assets/templates/sports.html");

function generateSportContent(sports: Sport[]) {
    const apiResponseContainer = document.getElementById("content");
    const template = sportTemplate.default;
    apiResponseContainer!.innerHTML = template;
    const sportContainer = document.getElementsByClassName("sportContainer")[0];
    
    sports.forEach((sport: Sport) => {
      const sportElement = document.createElement("div");
      sportElement.textContent = `${sport.sportName} - ${sport.sportNotes}`;
      sportContainer!.appendChild(sportElement);
    });
}
