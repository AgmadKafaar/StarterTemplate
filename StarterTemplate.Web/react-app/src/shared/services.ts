import { Client } from "./client";
import axios from "axios";

const { API_URL } = process.env;

let instance = axios.create({
  transformResponse: (data) => data,
});

instance.interceptors.request.use(function (config) {
  const userInfo = localStorage.getItem("redux_localstorage_simple_auth.user");
  if (userInfo) config.headers.UserInfo = userInfo;
  return config;
});

export const client = new Client(API_URL, instance);
