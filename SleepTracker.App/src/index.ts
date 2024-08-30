import "./app.scss";
import "./models/sports";
const sportTemplate = require("../assets/templates/sports.html");

function main(): void {
  const button = document.querySelector("#testButton");
  button?.addEventListener("click", testSubmitHandler);
  document.addEventListener("DOMContentLoaded", testImageHandler);
  const apiButton = document.querySelector("#apiButton");
  apiButton?.addEventListener("click", testApiHandler);
}

function testSubmitHandler(e: Event): void {
  e.preventDefault();
  const outputElement = document.querySelector("#output");
  outputElement!.textContent = "Scripts work!";
}

function testImageHandler(e: Event): void {
  e.preventDefault();
  const imageModule = require("../assets/test-image.jpg?as=webp");
  const imageSrc = imageModule.default || imageModule; // Access the default property if it exists

  const image = document.createElement("img");
  image.src = imageSrc;
  image.style.width = "200px";

  const imageWrapper = document.getElementById("imageTest");
  imageWrapper?.appendChild(image);
}

async function loadHTMLContent(url: string): Promise<string> {
  const response = await fetch(url);
  return response.text();
}

async function testApiHandler(e: Event): Promise<void> {
  e.preventDefault();
  try {
    const response = await fetch(process.env.API_URL + `/sport`);
    const json = await response.json();
    const apiResponseContainer = document.getElementById("apiResponse");
    const template = sportTemplate.default;
    apiResponseContainer!.innerHTML = template;
    const sportContainer = document.getElementsByClassName("sportContainer")[0];
    const sports = json as Sport[];
    sports.forEach((sport) => {
      const sportElement = document.createElement("div");
      sportElement.textContent = `${sport.sportName} - ${sport.sportNotes}`;
      sportContainer!.appendChild(sportElement);
    });

    console.log(sports);

  } catch (error) {
    console.error(error);
  }
}

main();
