import { BindingElement } from "../common/binding-element";
import { ValidationIssue } from "../common/validation-issue";
import { Login } from "./models/login";
import { LoginBinding } from "./models/login-binding";
import { TokenResponse } from "./models/token-response";
import { User } from "./models/user";

const authTemplate = require("../../../assets/templates/login.html");

const Email = "email";
const Password = "password";
const ConfirmPassword = "confirmPassword";
const UserData = "UserData";

class AuthService {
  static generateLoginContent(instance: AuthService) {
    const apiResponseContainer = document.getElementById("content");
    const template = authTemplate.default;
    apiResponseContainer!.innerHTML = template;

    const formAddSport = document.getElementById(
      "loginOrRegisterForm",
    ) as HTMLFormElement;

    const welcomeContainer = document.getElementsByClassName(
      "welcome-container",
    )[0] as HTMLDivElement;

    let currentUser : User | undefined | null = undefined;
    let userData = sessionStorage.getItem(UserData);

    if (userData) {
      currentUser = JSON.parse(userData) as User;
    }

    if (currentUser && currentUser.isAuthenticated) {
      formAddSport.style.visibility = "hidden";
      welcomeContainer.style.visibility = "visible";
      updateHome(currentUser);
    } else {
      formAddSport.style.visibility = "visible";
      welcomeContainer.style.visibility = "hidden";

      let confirmPasswordInput = document.getElementById("confirmPassword");
      let isLoginCheck = document.getElementById("existingAccount") as HTMLInputElement;
      isLoginCheck!.addEventListener("change", (event) => 
      {
        confirmPasswordInput!.style.visibility = isLoginCheck.checked ? "hidden" : "visible";
        const confirmPassword = document.getElementById("confirmPassword") as HTMLInputElement;
        if (isLoginCheck.checked) {
          confirmPassword?.removeAttribute("required");
        } else {
          confirmPassword.required = true;
        }
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
      clearErrors();
      const tokenResponse = await response.json() as TokenResponse;
      const currentUser : User = { email: formData.get('Email')!.toString(), isAuthenticated: true };
      storeSession(tokenResponse, currentUser);
      updateHome(currentUser);
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

function storeSession(sessionData: TokenResponse, user: User) {
  sessionStorage.setItem("AccessToken", sessionData.accessToken);
  sessionStorage.setItem("ExpiresIn", sessionData.expiresIn.toString());
  sessionStorage.setItem("RefreshToken", sessionData.refreshToken);
  sessionStorage.setItem("TokenType", sessionData.tokenType);

  sessionStorage.setItem(UserData, JSON.stringify(user));
}

function updateHome(user: User) {
  const contentContainer = document.getElementById("user-name");
  contentContainer!.textContent = user.email;
}

function clearErrors() {
  const errorContainer = document.getElementById("validationMessage");
  errorContainer!.textContent = "";
  errorContainer!.style.visibility = "visible"; 
}

function displayError(message: string) {
  const errorContainer = document.getElementById("validationMessage");
  errorContainer!.textContent = message;
  errorContainer!.style.visibility = "visible";
}


export default AuthService;
