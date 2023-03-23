import { AnyAction, PayloadAction, ThunkAction, createSlice } from "@reduxjs/toolkit";

import { History } from "history";
import type { RootState } from "../store";
import { User } from "../../shared/client";
import { client } from "../../shared/services";

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

// Other code such as selectors can use the imported `RootState` type
export const selectUser = (state: RootState) => state.auth.user;

export const thunkLogin =
  (username: string, password: string, history: History): ThunkAction<void, RootState, unknown, AnyAction> =>
  async (dispatch) => {
    dispatch(setIsLoading(true));
    const user = await client.login({ username: username.trim(), password: password });
    if (user) {
      dispatch(setUser(user));
      history.push("/");
    }
    dispatch(setIsLoading(false));
  };

export const thunkLogout =
  (history: History): ThunkAction<void, RootState, unknown, AnyAction> =>
  async (dispatch) => {
    dispatch(reset());
    history.push("/login");
  };

export default authSlice;
