import "@fontsource/montserrat";
import "@fontsource/montserrat/100.css";
import "@fontsource/montserrat/200.css";
import "@fontsource/montserrat/300.css";
import "@fontsource/montserrat/400.css";
import "@fontsource/montserrat/500.css";
import "@fontsource/montserrat/600.css";
import "@fontsource/montserrat/700.css";
import "@fontsource/montserrat/800.css";
import "@fontsource/montserrat/900.css";

import { extendTheme, theme as defaultTheme } from "@chakra-ui/react";
export const theme = extendTheme({
  config: {
    initialColorMode: "light",
    useSystemColorMode: false,
  },
  fonts: {
    heading: "Montserrat, sans-serif",
    body: "Montserrat, sans-serif",
  },
  // adjust some base Chakra colorScheme colours to meet WCAG AA minimum contrast requirements
  colors: {
    blue: {
      500: "#2D78BE",
    },
    green: {
      500: "#2E8456",
    },
    orange: {
      500: "#BB5B1B",
    },
    pink: {
      500: "#D33687",
    },
    red: {
      500: "#E22C2C",
    },
    teal: {
      500: "#2A8381",
    },
    brand: {
      50: "#F2F7F5",
      100: "#CCDFD7",
      200: "#99BFAF",
      300: "#669F86",
      400: "#337F5E",
      500: "#2E7E45", // Base Color
      600: "#6E8A56",
      700: "#566B41",
      800: "#005F36",
      900: "#004D2C",
    },
  },
  fontWeights: {
    hairline: 100,
    thin: 200,
    light: 300,
    normal: 400,
    medium: 500,
    semibold: 600,
    bold: 700,
    extrabold: 800,
    black: 900,
  },
  fontSizes: {
    xxs: "0.725rem",
    xs: "0.776rem",
    sm: "0.875rem",
    md: "1rem",
    lg: "1.125rem",
    xl: "1.25rem",
    "2xl": "1.5rem",
    "3xl": "1.875rem",
    "4xl": "2.25rem",
    "5xl": "3rem",
    "6xl": "4rem",
  },
  components: {
    Input: {
      defaultProps: {
        variant: "flushed",
        size: "sm",
      },
    },
    Textarea: {
      defaultProps: {
        variant: "outline",
        size: "sm",
      },
    },
    Container: {
      baseStyle: {
        maxWidth: "100ch",
      },
    },
    Heading: {
      baseStyle: {
        fontWeight: "medium",
      },
    },
    Link: {
      baseStyle: { color: "blue.600" },
    },
    Editable: {
      variants: {
        flushed: {
          input: (p) => defaultTheme.components.Input.variants.flushed(p).field,
        },
      },
    },
    FormLabel: {
      baseStyle: {
        fontSize: "sm",
        fontWeight: "medium",
        display: "flex",
      },
    },
    Menu: {
      baseStyle: {
        groupTitle: {
          fontSize: "xs",
          fontWeight: "medium",
        },
        list: {
          minWidth: "100px",
        },
        item: {
          fontSize: "xs",
        },
      },
    },
    MenuButton: {
      defaultProps: {
        variant: "ghost",
        size: "sm",
      },
      variants: {
        outline: {
          colorScheme: "gray",
        },
      },
    },
    Breadcrumb: {
      baseStyle: {
        item: {
          fontSize: "xs",
        },
      },
    },
  },
  shadows: {
    callout: "0 2px 10px 0 rgba(0,0,0,.132), 0 0.5px 1.8px 0 rgba(0,0,0,.108)",
    "section-h": "0 2px 15px 0 rgba(0,0,0,.2), 0 1px 3px 0 rgba(0,0,0,.5)",
    "section-v": "2px 0 10px 0 rgba(0,0,0,.2), 1px 0 3px 0 rgba(0,0,0,.5)",
  },
});
