import "./app.scss";
import "./services/sport/models/sport";
import "./services/sport/models/sport-binding";

import SportService from "./services/sport/sport-service";

async function main(): Promise<void> {
  // add event listeners to all links
  const navbar = document.getElementsByClassName("navbar")[0];
  const links = [...navbar.getElementsByTagName("a")];
  const sportService = new SportService();
  links.forEach((link) => {
    link.addEventListener("click", (e) => {
      e.preventDefault();
      const href = link.getAttribute("href");
      if (href) {
        loadData(href.replace("#", "/")).then((data) => {
          if (!data) {
            console.log("No data found");
            return;
          }
          if (href === "#sport") {
            SportService.generateSportContent(sportService, data as Sport[]);
          }
        });
      }
    });
  });
}

async function loadData(href: string): Promise<Object> {
  try {
    const response = await fetch(process.env.API_URL + href);
    const json = await response.json();
    const data = json as any;
    if (!data) {
      console.log("No data found");
      return {};
    }
    return data;
  } catch (error) {
    console.error(error);
    return {}
  }
}


// async function testApiHandler(e: Event): Promise<void> {
//   e.preventDefault();
//   try {
//     // get data from api
//     const response = await fetch(process.env.API_URL + `/sport`);
//     const json = await response.json();
//     const sports = json as Sport[];
//     if (!sports) {
//       console.log("No sports found");
//       return;
//     }

//     const apiResponseContainer = document.getElementById("content");
//     const template = sportTemplate.default;
//     apiResponseContainer!.innerHTML = template;
//     const sportContainer = document.getElementsByClassName("sportContainer")[0];

//     sports.forEach((sport) => {
//       const sportElement = document.createElement("div");
//       sportElement.textContent = `${sport.sportName} - ${sport.sportNotes}`;
//       sportContainer!.appendChild(sportElement);
//     });

//     console.log(sports);

//   } catch (error) {
//     console.error(error);
//   }
// }

main();
