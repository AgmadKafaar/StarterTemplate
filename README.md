# Core Starter Template

This is a starter template for Internal Web Apps or Webafrica Ports. Built using modern architecture and is written in ASP .Net Core 5 and React using Typescript.

* [Core Starter Template](#core-starter-template)
  * [Pre-requisites](#pre-requisites)
    + [Code Editors](#code-editors)
    + [Environment](#environment)
  * [Getting Started](#getting-started)
  * [Architecture](#architecture)
    + [Back-End using .Net Core 5](#back-end-using-net-core-5)
      - [Folder Structure](#folder-structure)
      - [Integrations](#integrations)
    + [Front-End React App](#front-end-react-app)
      - [Folder Structure](#folder-structure-1)
      - [Redux State management](#redux-state-management)
        * [Slices - Reducers & Selectors](#slices---reducers---selectors)

## Pre-requisites

### Code Editors

- [Microsoft Visual Studio Community 2019 ](https://visualstudio.microsoft.com/downloads/) 
- [Visual Studio Code](https://code.visualstudio.com/)

### Environment

- In order to run the .Net Core project, you will need to install [.Net 5 SDK](https://dotnet.microsoft.com/download).

- [NodeJs 14](https://nodejs.org/en/)  is used to run the front-end React application.

- Yarn is used as a package manager. Once you install node, you can install yarn using:

  ```powershell
  npm install --global yarn
  ```

  > 💡 **Tip**: You can install node version manager to manage multiple versions of NodeJs. This is useful if you need to run other projects that require  an older Node version. See more [here](https://docs.microsoft.com/en-us/windows/dev-environment/javascript/nodejs-on-windows)



## Getting Started

You can get up and running with the project by following the steps below

1) Clone the project

```powershell
git clone https://git.webafrica.co.za/Templates/CoreStarterApp
```

2)  cd into the react app folder

```powershell
cd .\CoreStarterApp\CoreStarterApp.Web\react-app\
```

3)  Restore all packages

```powershell
yarn install
```

4)  Run the app

```powershell
yarn start
```

You should now see this:

```
You can now view react-app in the browser.       

  Local:            http://localhost:3005        
  On Your Network:  http://172.30.128.1:3005     

Note that the development build is not optimized.
To create a production build, use yarn build
```

Once this is complete. You are ready to run the .Net 5 API. Open up the solution in visual studio and run TroubleshooterV2.Web project. 

After both apps is running, http://localhost:5000 will launch and will proxy requests to the local react server running on port 3005.

 You can access the Swagger documentation at  http://localhost:5000/swagger/

## Architecture

The main tech stack is a backend API written in .Net Core 5, a frontend app built in React and Typescript and NSwag to integrate the 2 applications by generating a Typescript client based on the API documentation. Both applications are contained in this single repository. This is to make development easier as there is no need to commit to a main project as well as a submodule. 

### Back-End using .Net Core 5

The project is built using an onion architecture similar to the structure of the Livechat. Domain models, Services and API Clients are stored in TroubleshooterV2.Core. Dependency injection is used to access these services from the Web API Controllers in TroubleshooterV2.Web.

#### Folder Structure

```
📦TroubleshooterV2.Core

 ┣ 📂Entities (*All Domain Classes Are stored in this folder*)  
 ┣ 📂Infrastructure  
 ┃ ┣ 📂Api  (*RestSharp API Clients*)  
 ┃ ┣ 📂Data  (*Repositories*)  
 ┃ ┗ 📂Exceptions  
 ┣ 📂Services  
 ┃ ┣ 📂Authentication (*LDAP Authentication Service*)  

 📦TroubleshooterV2.Web  
 ┣ 📂Controllers  
 ┣ 📂Pages  
 ┣ 📂Properties  
 ┣ 🌐react-app  
```

#### Integrations

- Swashbuckle is used to generate Swagger API Documentation
- Serilog is used for Structured Logging

### Front-End React App

[Create-React-App](https://create-react-app.dev/) Typescript was used to generate the base project. It is the most popular tool for scaffolding out React applications. [Chakra-UI](https://chakra-ui.com/) was installed as UI kit. It is nice and simple to work with. It comes with modern tools to help layout and theme your application.

#### Folder Structure

```
📦src  
 ┣ 📂assets  
 ┃ ┗ 📂images  
 ┣ 📂components  
 ┃ ┣ 📂layout  
 ┃ ┃ ┗ 📜Navbar.tsx  
 ┃ ┗ 📂shared  
 ┣ 📂pages  
 ┃ ┣ 📜Home.tsx  
 ┃ ┣ 📜Login.tsx  
 ┃ ┗ 📜PrivateRoute.tsx  
 ┣ 📂redux  
 ┃ ┣ 📂slices  
 ┃ ┃ ┣ 📜authSlice.ts  
 ┃ ┗ 📜store.tsx  
 ┣ 📂shared  
 ┃ ┣ 📜client.ts  
 ┃ ┣ 📜hooks.ts  
 ┃ ┗ 📜services.ts  
 ┣ 📂theme  
 ┃ ┣ 📜colors.ts  
 ┃ ┗ 📜globalTheme.ts  
 ┣ 📜App.tsx  
```

#### Redux State management

Redux Toolkit has been used to set up redux state for the application. This creates a global store where we can store data that can be accessed by all components. The code for this is located in the `redux` folder

##### Slices - Reducers & Selectors

`createSlice()` accepts an initial state and an object with reducer names and functions, and automatically generates action creator functions, action type strings, and a reducer function.

```typescript
// Define a type for the slice state
interface AuthState {
  user: User | null;
  isLoading: boolean;
}

// Define the initial state using that type
const initialState: AuthState = {
  user: null,
  isLoading: false,
};

export const authSlice = createSlice({
  name: "auth",
  // `createSlice` will infer the state type from the `initialState` argument
  initialState,
  reducers: {
    reset: (state) => initialState,
    setUser: (state, action: PayloadAction<User | null>) => {
      state.user = action.payload;
    },
    setIsLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
  },
});

export const { reset, setUser, setIsLoading } = authSlice.actions;
```



## Nswag CodeGen

Nswag was set up to generate the front end code based on your swagger document. The output file gets generated and is stored in `src\shared\client.ts`.

If you want to update this file, make sure your API is running locally and run `yarn api`. This will generate Models, Enums, Error Types and functions that you can use. It will call the API behind the scenes.

