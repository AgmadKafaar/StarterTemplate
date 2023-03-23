import { configureStore, getDefaultMiddleware } from "@reduxjs/toolkit";
import { load, save } from "redux-localstorage-simple";

import { authSlice } from "./slices/authSlice";

const store = configureStore({
  reducer: {
    auth: authSlice.reducer,
  },
  middleware: getDefaultMiddleware().concat(
    save({
      states: ["auth.user"],
    })
  ),
  preloadedState: load({
    states: ["auth.user"],
  }),
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;

export default store;
