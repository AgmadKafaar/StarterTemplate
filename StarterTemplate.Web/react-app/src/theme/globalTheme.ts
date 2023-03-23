import { extendTheme } from "@chakra-ui/react";
import { core } from "./colors";

export const coreTheme = extendTheme({
  styles: {
    global: {
      body: {
        bg: "core.gray.dark",
        color: "white",

        scrollbarWidth: "thin",
        "&::-webkit-scrollbar": {
          width: "10px",
          height: "10px",
        },
        "&::-webkit-scrollbar-track": {
          width: "10px",
          height: "10px",
          background: "#1E2025",
        },
        "&::-webkit-scrollbar-thumb": {
          background: core.gray.light,
          borderRadius: "24px",
        },
      },
    },
  },
  colors: {
    core: core,
    gray: core.gray,
  },
  config: {
    initialColorMode: "dark",
    useSystemColorMode: false,
  },
});
