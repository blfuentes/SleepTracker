import "./app.scss";
import "./services/sport/models/sport";
import { Sport } from "./services/sport/models/sport";
import "./services/sport/models/sport-binding";
import { User } from "./services/auth/models/user";
import AuthService from "./services/auth/auth-service";
import SportService from "./services/sport/sport-service";

let currentUser : User;

async function main(): Promise<void> {
  // add event listeners to all links
  const navbar = document.getElementsByClassName("navbar")[0];
  const links = [...navbar.getElementsByTagName("a")];
  const sportService = new SportService();
  const authService = new AuthService();
  currentUser = { email: "", token: "", isAuthenticated: false }

  AuthService.generateSportContent(authService, currentUser);

  links.forEach((link) => {
    link.addEventListener("click", (e) => {
      e.preventDefault();
      const href = link.getAttribute("href");
      if (href) {
        loadData(href.replace("#", "/api/")).then((data) => {
          if (!data) {
            console.log("No data found");
            return;
          }
          if (href === "#sports") {
            SportService.generateSportContent(sportService, data as Sport[]);
          }
          if (href === "#home") {
            AuthService.generateSportContent(authService, currentUser);
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

main();
