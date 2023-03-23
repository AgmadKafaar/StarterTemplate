import * as React from "react";

import { Route, BrowserRouter as Router, Switch } from "react-router-dom";

import { ChakraProvider } from "@chakra-ui/react";
import Home from "./pages/Home";
import Login from "./pages/Login";
import { PrivateRoute } from "./pages/PrivateRoute";
import { Provider } from "react-redux";
import store from "./redux/store";
import { coreTheme } from "./theme/globalTheme";

export const App = () => (
  <Provider store={store}>
    <ChakraProvider theme={coreTheme}>
      <Router>
        <Switch>
          <Route path="/login">
            <Login />
          </Route>
          <PrivateRoute path="/">
            <Home />
          </PrivateRoute>
        </Switch>
      </Router>
    </ChakraProvider>
  </Provider>
);
