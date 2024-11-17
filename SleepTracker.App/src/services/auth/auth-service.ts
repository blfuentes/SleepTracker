import { BindingElement } from "../common/binding-element";
import { ValidationIssue } from "../common/validation-issue";
import { Login } from "./models/login";
import { LoginBinding } from "./models/login-binding";
import { User } from "./models/user";

const authTemplate = require("../../../assets/templates/login.html");

const Email = "email";
const Password = "password";
const ConfirmPassword = "confirmPassword";

class AuthService {
  static generateSportContent(instance: AuthService, user: User) {
    const apiResponseContainer = document.getElementById("content");
    const template = authTemplate.default;
    apiResponseContainer!.innerHTML = template;

    const formAddSport = document.getElementById(
      "loginOrRegisterForm",
    ) as HTMLFormElement;

    if (user.isAuthenticated) {
      formAddSport.style.visibility = "hidden";
    } else {
      let confirmPasswordInput = document.getElementById("confirmPassword");
      let isLoginCheck = document.getElementById("existingAccount") as HTMLInputElement;
      isLoginCheck!.addEventListener("change", (event) => 
      {
        confirmPasswordInput!.style.visibility = isLoginCheck.checked ? "hidden" : "visible";
      });
      formAddSport!.addEventListener("submit", (event) =>
        LoginOrRegister(event),
      );

      const formSportName = document.getElementById(Email) as HTMLInputElement;
      formSportName!.addEventListener("input", () => {
        const errorContainer = document.getElementById("validationMessage");
        errorContainer!.style.visibility = "hidden";
      });
    }
  }
}

async function LoginOrRegister(event: Event): Promise<void> {
  try {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const formData = new FormData(form);
    const formObject = Object.fromEntries(formData.entries());
    const jsonPayload = JSON.stringify(formObject);
    // const urlEncodedData = new URLSearchParams(formData as any).toString();

    const isRegister = formData.get("ExistingAccount") !== 'on';

    const url = process.env.API_URL + (isRegister ? "/api/auth/register" :  "/api/auth/login");
    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: jsonPayload,
    });
    
    if (response.ok) {
      console.log(`${isRegister ? "Register" : "Logged"} successfully`);
      const tokenResponse = await response.json();
      console.log(tokenResponse);
    } else {
        console.error("Failed to register sport");
        switch (response.status) {
            case 409:
                displayError("Email already taken");
                break;
            case 400:
              const validation = await response.json() as ValidationIssue[];
                displayError(validation.map((v, _) => v.description).join("\n"));
            default:
                console.error("Unknown error");
                break;
        }
    }
  } catch(error) {

  }
}

function displayError(message: string) {
  const errorContainer = document.getElementById("validationMessage");
  errorContainer!.textContent = message;
  errorContainer!.style.visibility = "visible";
}


export default AuthService;
