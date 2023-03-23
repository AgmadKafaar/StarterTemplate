import { Redirect, Route, RouteProps } from "react-router";

import React from "react";
import { useAppSelector } from "../shared/hooks";
import { useHistory } from "react-router-dom";

export const PrivateRoute = (props: RouteProps) => {
  const { children, ...rest } = props;
  const user = useAppSelector((state) => state.auth.user);
  let history = useHistory();

  if (!user) {
    history.push("/login");
  }

  return (
    <Route
      {...rest}
      render={({ location }) =>
        user ? (
          children
        ) : (
          <Redirect
            to={{
              pathname: "/login",
              state: { from: location },
            }}
          />
        )
      }
    />
  );
};
